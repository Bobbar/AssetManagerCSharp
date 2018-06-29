using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingVisualizer
{
    public class PingVis : IDisposable
    {
        private ManualResetEvent renderEvent = new ManualResetEvent(false);
        private ManualResetEvent disposeEvent = new ManualResetEvent(false);
        private Task renderTask;

        private Graphics upscaledGraphics;
        private Bitmap upscaledImage;
        private int imageUpscaleMulti = 5;
        private Size upscaledImageSize;

        private Graphics downsampleGraphics;
        private Bitmap downsampledImage;
        private Size downsampleImageSize;

        private Ping ping = new Ping();
        private List<PingInfo> pingReplies = new List<PingInfo>();
        private bool pingRunning = false;
        private System.Timers.Timer pingTimer;

        private string hostname;
        private Control targetControl;

        private float targetViewScale;
        private float currentViewScale;
        private System.Timers.Timer scaleEaseTimer;
        private int scaleEaseTimerInterval;

        private float infoFontSize;
        private float overInfoFontSize;
        private Font mousePingInfoFont;
        private Font pingInfoFont;
        private Brush mouseOverTextBrush = new SolidBrush(Color.FromArgb(240, Color.White));
        private Brush mouseOverBarBrush = new SolidBrush(Color.FromArgb(128, Color.Navy));
        private Color mouseScrollingBackColor = Color.FromArgb(48, 53, 61);
        private MouseOverInfo mouseOverInfo = null;
        private bool mouseIsOverBars = false;
        private PointF mouseLocationScaled;
        private bool mouseIsScrolling = false;
        private int mouseMoves = 0;

        private int topIndex = 0;
        private List<PingBar> currentBarList = new List<PingBar>();

        private const int pingTimeOut = 1000;
        private const int maxBadPing = 300; // Ping time at which the bar color will be fully red.
        private const int maxViewScaleLines = 30; // Scale lines will fade out and stop being drawn after this is reached.
        private const int goodPingInterval = 1000;
        private const int noPingInterval = 3000;
        private int currentPingInterval = goodPingInterval;

        private const int maxViewScale = 15; // Max "zoom" level for very low latency ping bars.
        private const float barGap = 0; // Y coord gap between bars.
        private const int maxBars = 15; // Number of bars to display.
        private const int minBarLength = 1; // Min X coord length.
        private const float barTopPadding = 0; // Padding added to top of bar list.
        private const float barBottomPadding = 6; // Padding added to bottom of bar list.

        private const int maxStoredResults = 1000000; // Number of results to store before culling occurs.
        private const int maxDrawRateFPS = 100; // Max FPS allowed.
        private long lastDrawTime = 0; // Tick count of last render. Used for FPS limiting.

        public event EventHandler<PingEventArgs> NewPingResult;

        private int TopIndex
        {
            get
            {
                var barCount = pingReplies.Count;

                if (!mouseIsScrolling)
                {
                    if (barCount <= maxBars)
                    {
                        topIndex = 0;
                        return topIndex;
                    }
                    else
                    {
                        topIndex = barCount - maxBars;
                        return topIndex;
                    }
                }
                else
                {
                    return topIndex;
                }
            }

            set
            {
                topIndex = value;
            }
        }

        protected void OnNewPingResult(PingInfo pingReply)
        {
            RaiseEventOnUIThread(NewPingResult, new object[] { this, new PingEventArgs(pingReply) });
        }

        public PingInfo CurrentResult
        {
            get
            {
                if (pingReplies.Count > 0)
                {
                    return pingReplies.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        public PingVis(Control targetControl, string hostName)
        {
            InitControl(targetControl);
            this.hostname = hostName;
            InitGraphics();
            InitScaleTimer();
            InitPing();

            // Start the rendering loop on a new task.
            renderTask = new Task(RenderLoop, TaskCreationOptions.LongRunning);
            renderTask.Start();
        }

        public void ClearResults()
        {
            pingReplies.Clear();
            currentViewScale = 1;
        }

        private void InitGraphics()
        {
            upscaledImageSize = new Size(targetControl.ClientSize.Width * imageUpscaleMulti, targetControl.ClientSize.Height * imageUpscaleMulti);
            downsampleImageSize = targetControl.ClientSize;

            upscaledImage = new Bitmap(upscaledImageSize.Width, upscaledImageSize.Height, PixelFormat.Format32bppPArgb);
            upscaledGraphics = Graphics.FromImage(upscaledImage);
            upscaledGraphics.SmoothingMode = SmoothingMode.None;
            upscaledGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            upscaledGraphics.TextContrast = 0;

            downsampledImage = new Bitmap(downsampleImageSize.Width, downsampleImageSize.Height, PixelFormat.Format32bppPArgb);
            downsampledImage.SetResolution(upscaledImage.HorizontalResolution, upscaledImage.VerticalResolution);
            downsampleGraphics = Graphics.FromImage(downsampledImage);
            downsampleGraphics.CompositingMode = CompositingMode.SourceCopy;
            downsampleGraphics.CompositingQuality = CompositingQuality.HighSpeed;
            downsampleGraphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            downsampleGraphics.SmoothingMode = SmoothingMode.None;
            downsampleGraphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

            overInfoFontSize = 7 * imageUpscaleMulti;
            infoFontSize = 8 * imageUpscaleMulti;
            mousePingInfoFont = new Font("Tahoma", overInfoFontSize, FontStyle.Regular);
            pingInfoFont = new Font("Tahoma", infoFontSize, FontStyle.Bold);
        }

        private void InitControl(Control targetControl)
        {
            this.targetControl = targetControl;

            targetControl.VisibleChanged -= TargetControl_VisibleChanged;
            targetControl.VisibleChanged += TargetControl_VisibleChanged;

            targetControl.MouseWheel -= TargetControl_MouseWheel;
            targetControl.MouseWheel += TargetControl_MouseWheel;

            targetControl.MouseLeave -= TargetControl_MouseLeave;
            targetControl.MouseLeave += TargetControl_MouseLeave;

            targetControl.MouseMove -= TargetControl_MouseMove;
            targetControl.MouseMove += TargetControl_MouseMove;
        }

        private void InitPing()
        {
            ServicePointManager.DnsRefreshTimeout = 0;
            InitPingTimer();
            StartPing();
        }

        private void InitPingTimer()
        {
            if (pingTimer == null)
            {
                pingTimer = new System.Timers.Timer();
            }
            pingTimer.Elapsed += PingTimer_Elapsed;
            pingTimer.Interval = currentPingInterval;
            pingTimer.Start();
        }

        private void PingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!this.disposedValue)
            {
                StartPing();
            }
        }

        private void InitScaleTimer()
        {
            scaleEaseTimerInterval = 1000 / maxDrawRateFPS;
            scaleEaseTimer = new System.Timers.Timer();
            scaleEaseTimer.Elapsed += ScaleEaseTimer_Elapsed;
            scaleEaseTimer.Interval = scaleEaseTimerInterval;
        }

        private void ScaleEaseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!this.disposedValue)
            {
                EaseScaleChange();
            }
        }

        private void RaiseEventOnUIThread(Delegate theEvent, object[] args)
        {
            try
            {
                foreach (Delegate d in theEvent.GetInvocationList())
                {
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
                    {
                        d.DynamicInvoke(args);
                    }
                    else
                    {
                        syncer.BeginInvoke(d, args);
                    }
                }
            }
            catch (Exception ex)
            {
                // InvalidOperationExceptions can occur here occasionally . Silently print them to the console.
                Console.WriteLine(ex.ToString());
            }
        }

        private void SetScale()
        {
            // Fraction width multiplier.
            // Determines the Y location of the highest peak.
            // 0.5 = middle of the image, 1 = all the way to the right.
            float scaleWidthFraction = 0.55f;

            var currentResults = CurrentDisplayResults();

            if (currentResults.Count > 0)
            {
                long maxPing = currentResults.OrderByDescending(result => result.RoundTripTime).FirstOrDefault().RoundTripTime;

                if (maxPing <= 0) maxPing = 1;

                float newScale = ((upscaledImageSize.Width * scaleWidthFraction) / maxPing);

                if (newScale > maxViewScale) newScale = maxViewScale;

                // Update the target scale if the new scale is different.
                if (targetViewScale != newScale)
                {
                    targetViewScale = newScale;

                    // If we are scrolling, set the current scale immediately.
                    if (mouseIsScrolling)
                    {
                        currentViewScale = targetViewScale;
                    }
                }

                // Start the scale timer if needed.
                if (currentViewScale != targetViewScale) scaleEaseTimer.Start();
            }
        }

        /// <summary>
        /// Smoothly eases between scale changes.
        /// </summary>
        private void EaseScaleChange()
        {
            if (currentViewScale != targetViewScale)
            {
                if (!mouseIsScrolling)
                {
                    // Get the diffence between the current and target.
                    float diff = currentViewScale - targetViewScale;
                    float diffAbs = Math.Abs(diff);

                    // If the absolute difference is above a certain amount begin/continue easing.
                    if (diffAbs > 0.02f)
                    {
                        // Simple easing calulation.
                        if (currentViewScale > targetViewScale)
                        {
                            currentViewScale -= (diffAbs / 7f);
                        }
                        else if (currentViewScale < targetViewScale)
                        {
                            currentViewScale += (diffAbs / 7f);
                        }
                    }
                    else
                    {
                        // Set to final scale and stop timer.
                        currentViewScale = targetViewScale;
                        scaleEaseTimer.Stop();
                    }
                    Render();
                }
                else
                {
                    currentViewScale = targetViewScale;
                    scaleEaseTimer.Stop();
                }
            }
        }

        private async void StartPing()
        {
            try
            {
                if (!pingRunning)
                {
                    var reply = await GetPingReply(hostname);

                    if (this.disposedValue) return;

                    if (reply.Status == IPStatus.Success)
                    {
                        SetPingInterval(goodPingInterval);
                    }
                    else
                    {
                        SetPingInterval(noPingInterval);
                    }
                    var pingInfo = new PingInfo(reply);
                    AddPingReply(pingInfo);
                    OnNewPingResult(pingInfo);
                }
            }
            catch (Exception)
            {
                if (!this.disposedValue)
                {
                    AddPingReply(new PingInfo());
                    OnNewPingResult(new PingInfo());
                    SetPingInterval(noPingInterval);
                }
            }
            finally
            {
                if (!this.disposedValue)
                {
                    if (!mouseIsScrolling)
                    {
                        Render(true, true);
                    }
                    else
                    {
                        Render(true, false);
                    }
                }
            }
        }

        /// <summary>
        /// Adds ping reply to the reply collection if it's a success or there were previous successes.
        /// </summary>
        /// <param name="reply"></param>
        private void AddPingReply(PingInfo reply)
        {
            // No sense in accumulating when we start with the device offline.
            if (pingReplies != null)
            {
                if (pingReplies.Count == 0 && reply.Status == IPStatus.Success)
                {
                    pingReplies.Add(reply);
                }
                else if (pingReplies.Count > 0)
                {
                    pingReplies.Add(reply);
                }
            }
        }

        private async Task<PingReply> GetPingReply(string hostname)
        {
            try
            {
                pingRunning = true;
                var options = new PingOptions();
                options.DontFragment = true;
                byte[] buff = Encoding.ASCII.GetBytes("pingpingpingping");

                return await Task.Run(() => { return ping.Send(hostname, pingTimeOut, buff, options); });
            }
            finally
            {
                pingRunning = false;
            }
        }

        private void SetPingInterval(int interval)
        {
            if (currentPingInterval != interval)
            {
                currentPingInterval = interval;
            }

            if (!this.disposedValue) pingTimer.Interval = currentPingInterval;
        }

        private void TargetControl_VisibleChanged(object sender, EventArgs e)
        {
            if (targetControl.Visible)
                Render(false, true);
        }

        private void TargetControl_MouseLeave(object sender, EventArgs e)
        {
            mouseIsScrolling = false;
            Render(true, true);
        }

        private void TargetControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pingReplies.Count > maxBars)
            {
                int newIdx = 0;

                if (e.Delta < 0) // Scroll up.
                {
                    newIdx = TopIndex + 1;

                    //if the scroll index returns to the end (bottom) of the results, disable scrolling and return to normal display
                    if (newIdx >= pingReplies.Count - maxBars)
                    {
                        newIdx = pingReplies.Count - maxBars;

                        if (mouseIsScrolling)
                        {
                            mouseIsScrolling = false;
                            Render(true, true);
                        }
                    }
                }
                else if (e.Delta > 0) // Scroll down.
                {
                    newIdx = TopIndex - 1;
                    if (newIdx < 0) // Clamp the index to never be negative.
                    {
                        newIdx = 0;
                    }
                }
                if (TopIndex != newIdx)
                {
                    mouseIsScrolling = true;
                    TopIndex = newIdx;
                    Render(true, true);
                }
            }
        }

        private void TargetControl_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMoves++;
            mouseLocationScaled = new PointF(e.Location.X * imageUpscaleMulti, e.Location.Y * imageUpscaleMulti);

            if (mouseIsScrolling)
            {
                // Limit the rate of Render calls.
                // This event can fire very rapidly which can flood the Renderer
                // preventing other calls from getting past the frame rate limiter.
                if ((mouseMoves >= 2))
                {
                    mouseMoves = 0;

                    // We don't want to render after every single mouse movement, so we make
                    // sure we only render if the mouse is actually over a bar. Then when
                    // the mouse leaves a bar, we render once to reset any highlighted bars.
                    // Future movements will not cause a render unless the mouse is back over a bar.

                    if (MouseIsOverBars())
                    {
                        mouseIsOverBars = true;
                        Render();
                    }
                    else
                    {
                        // Only render if previous movement was over a bar.
                        // Unset the bool so that this render only occurs once.
                        if (mouseIsOverBars)
                        {
                            mouseIsOverBars = false;
                            Render();
                        }
                    }
                }
            }
        }

        private bool MouseIsOverBars()
        {
            for (int i = 0; i < currentBarList.Count; i++)
            {
                if (currentBarList[i].Rectangle.Contains(mouseLocationScaled))
                    return true;
            }
            return false;
        }

        private bool MouseIsOverBar(PingBar bar)
        {
            if (bar.Rectangle.Contains(mouseLocationScaled))
            {
                return true;
            }
            return false;
        }

        private void Render(bool forceDraw = false, bool refreshPingBars = false)
        {
            if (pingReplies.Count < 1) return;

            // Do not render if the parent form is minimized, just to reduce wasted cycles.
            if (targetControl != null & targetControl.FindForm() != null)
            {
                if (targetControl.FindForm().WindowState == FormWindowState.Minimized) return;
                if (!targetControl.Visible) return;
            }

            // Framerate limiter with override.
            if (!forceDraw && !CanDraw(DateTime.Now.Ticks))
            {
                return;
            }

            // Only render if we are not currently in the middle of a render cycle.
            if (!renderEvent.WaitOne(0))
            {
                // This call will draw with updated ping bars, otherwise we're just redrawing the existing ones for scale changes an whatnot.
                if (refreshPingBars) RefreshPingBars();

                // Set the render event to trigger a new rendering.
                renderEvent.Set();
            }
        }

        private void RenderLoop()
        {
            try
            {
                while (!this.disposedValue)
                {
                    // Wait until a render event is triggered.
                    renderEvent.WaitOne(Timeout.Infinite);

                    // Check for diposal event and break from loop if needed.
                    if (disposeEvent.WaitOne(0))
                        break;

                    // Perform the drawing methods.
                    // Change the background to indicate scrolling is active.
                    if (!mouseIsScrolling)
                    {
                        upscaledGraphics.Clear(Color.Black);
                    }
                    else
                    {
                        upscaledGraphics.Clear(mouseScrollingBackColor);
                    }

                    // Draw all the elements from back to front.
                    DrawScaleLines();
                    DrawPingBars();
                    DrawPingText();
                    DrawScrollBar();

                    // Cull the ping list if it exceeds the stored result limit.
                    TrimPingList();

                    // Resample the image to the original size.
                    DownsampleImage();

                    // Set the target control image.
                    SetControlImage();

                    // Reset the render event, if not disposing, to wait until another render is triggered.
                    if (!disposeEvent.WaitOne(0))
                        renderEvent.Reset();
                }
            }
            catch (Exception ex)
            {
                // We want any exceptions to be silent, but log them to the console for debugging purposes.
                Console.WriteLine(ex.ToString());
            }
        }

        private void DrawScaleLines()
        {
            float stepSize = currentViewScale * 15;
            int numOfLines = (int)(upscaledImageSize.Width / stepSize);
            if (numOfLines < maxViewScaleLines)
            {
                float scaleXPos = stepSize;
                using (var pen = new Pen(GetVariableBrush(Color.White, Color.Black, maxViewScaleLines, numOfLines), 2))
                {
                    for (int a = 0; a < numOfLines; a++)
                    {
                        upscaledGraphics.DrawLine(pen, new PointF(scaleXPos, 0), new PointF(scaleXPos, upscaledImageSize.Height));
                        scaleXPos += stepSize;
                    }
                }
            }
        }

        private void DrawPingBars()
        {
            // Is set to true if mouse is over any bars.
            bool isMouseOverBars = false;

            foreach (var bar in currentBarList)
            {
                if (mouseIsScrolling && MouseIsOverBar(bar))
                {
                    // If mouse is over a bar, set the mouseOverInfo to be drawn in the DrawPingText method.
                    isMouseOverBars = true;
                    mouseOverInfo = new MouseOverInfo(mouseLocationScaled, bar.PingInfo);
                    upscaledGraphics.FillRectangle(mouseOverBarBrush, new RectangleF(bar.Rectangle.Location, new SizeF(bar.Length * currentViewScale, bar.Rectangle.Height)));
                }
                else
                {
                    upscaledGraphics.FillRectangle(bar.Brush, new RectangleF(bar.Rectangle.Location, new SizeF(bar.Length * currentViewScale, bar.Rectangle.Height)));
                }
            }

            // If mouse is not over any bars, null the mouseOverInfo.
            if (!isMouseOverBars)
                mouseOverInfo = null;
        }

        private void DrawPingText()
        {
            if (mouseIsScrolling)
            {
                // Draw ping info for the highlighted bar.
                if (mouseOverInfo != null)
                {
                    string overInfoText = GetReplyStatusText(mouseOverInfo.PingReply);
                    SizeF textSize = upscaledGraphics.MeasureString(overInfoText, mousePingInfoFont);
                    upscaledGraphics.DrawString(overInfoText, mousePingInfoFont, mouseOverTextBrush, new PointF(mouseOverInfo.MouseLoc.X + (textSize.Width * 0.5f), mouseOverInfo.MouseLoc.Y - (textSize.Height * 0.5f)));
                }
            }
            else
            {
                string infoText = GetReplyStatusText(pingReplies.Last());
                SizeF textSize = upscaledGraphics.MeasureString(infoText, pingInfoFont);
                upscaledGraphics.DrawString(infoText, pingInfoFont, Brushes.White, new PointF((upscaledImageSize.Width - 5) - (textSize.Width), upscaledImageSize.Height - (textSize.Height + 5)));
            }
        }

        private void DrawScrollBar()
        {
            if (mouseIsScrolling)
            {
                var scrollYPos = upscaledImageSize.Height / (pingReplies.Count / ((float)TopIndex + (maxBars / 2)));

                upscaledGraphics.FillRectangle(Brushes.White, new RectangleF(upscaledImageSize.Width - (20 + imageUpscaleMulti), scrollYPos, 10 + imageUpscaleMulti, 5 + imageUpscaleMulti));
            }
        }

        private void DownsampleImage()
        {
            var destRect = new Rectangle(0, 0, downsampleImageSize.Width, downsampleImageSize.Height);
            downsampleGraphics.DrawImage(upscaledImage, destRect);
        }

        private Brush GetVariableBrush(Color startColor, Color endColor, int maxValue, long currentValue, bool translucent = false)
        {
            const int maxIntensity = 255;
            int intensity = 0;
            long r1, g1, b1, r2, g2, b2;

            r1 = startColor.R;
            g1 = startColor.G;
            b1 = startColor.B;

            r2 = endColor.R;
            g2 = endColor.G;
            b2 = endColor.B;

            if (currentValue > 0)
            {
                // Compute the intensity of the end color.
                intensity = (int)(maxIntensity / (maxValue / (float)currentValue));
            }

            // Clamp the intensity within the max.
            if (intensity > maxIntensity) intensity = maxIntensity;

            // Calculate the new RGB values from the intensity.
            int newR, newG, newB;
            newR = (int)(r1 + (r2 - r1) / (float)maxIntensity * intensity);
            newG = (int)(g1 + (g2 - g1) / (float)maxIntensity * intensity);
            newB = (int)(b1 + (b2 - b1) / (float)maxIntensity * intensity);

            if (translucent)
            {
                return new SolidBrush(Color.FromArgb(220, newR, newG, newB));
            }
            else
            {
                return new SolidBrush(Color.FromArgb(newR, newG, newB));
            }
        }

        private void DisposeBarList(List<PingBar> bars)
        {
            if (bars != null)
            {
                foreach (var bar in bars)
                {
                    bar.Dispose();
                }
                bars.Clear();
            }
            else
            {
                bars = new List<PingBar>();
            }
        }

        private bool CanDraw(long timeTick)
        {
            long elapTime = (timeTick - lastDrawTime) / 10000;
            float minFrameDelay = 1000 / (float)maxDrawRateFPS;
            if (elapTime >= minFrameDelay)
            {
                lastDrawTime = timeTick;
                return true;
            }
            return false;
        }

        private string GetReplyStatusText(PingInfo reply)
        {
            switch (reply.Status)
            {
                case IPStatus.Success:
                    return reply.RoundTripTime + "ms";

                case IPStatus.TimedOut:
                    return "T/O";

                default:
                    return "ERR";
            }
        }

        private void RefreshPingBars()
        {
            DisposeBarList(currentBarList);

            SetScale();

            float currentYPos = barTopPadding;
            float barHeight = (upscaledImageSize.Height - barBottomPadding - barTopPadding - (barGap * maxBars)) / maxBars;

            foreach (PingInfo result in CurrentDisplayResults())
            {
                float barLen;
                Brush barBrush;
                if (result.Status == IPStatus.Success)
                {
                    barBrush = GetVariableBrush(Color.Green, Color.Red, maxBadPing, result.RoundTripTime, true);
                    barLen = result.RoundTripTime + minBarLength;
                    if (barLen < minBarLength) barLen = minBarLength;
                }
                else
                {
                    barBrush = new SolidBrush(Color.FromArgb(200, Color.Red));
                    barLen = pingTimeOut * maxViewScale;
                }
                currentBarList.Add(new PingBar(barLen, barBrush, new RectangleF(1, currentYPos, barLen * currentViewScale, barHeight), currentYPos, result));
                currentYPos += barHeight + barGap;
            }
        }

        private void TrimPingList()
        {
            if (pingReplies.Count > maxStoredResults)
            {
                pingReplies = pingReplies.GetRange(pingReplies.Count - maxStoredResults, maxStoredResults);
            }
        }

        private List<PingInfo> CurrentDisplayResults()
        {
            if (pingReplies.Count > maxBars)
            {
                return pingReplies.GetRange(TopIndex, maxBars);
            }
            else
            {
                return pingReplies;
            }
        }

        private void SetControlImage()
        {
            if (targetControl.InvokeRequired)
            {
                var del = new Action(() => SetControlImage());
                targetControl.BeginInvoke(del);
            }
            else
            {
                if (!(targetControl is Form))
                {
                    if (targetControl is Button)
                    {
                        var but = (Button)targetControl;
                        but.Image = downsampledImage;
                        but.Invalidate();
                    }
                    else if (targetControl is PictureBox)
                    {
                        var pic = (PictureBox)targetControl;
                        pic.Image = downsampledImage;
                        pic.Refresh();
                    }
                }
                else
                {
                    targetControl.BackgroundImage = downsampledImage;
                    targetControl.Invalidate();
                }
            }
        }

        public class PingInfo
        {
            private IPStatus status;
            private long roundTripTime;
            private IPAddress address;

            public IPStatus Status
            {
                get
                {
                    return status;
                }
            }

            public long RoundTripTime
            {
                get
                {
                    return roundTripTime;
                }
            }

            public IPAddress Address
            {
                get
                {
                    return address;
                }
            }

            public PingInfo()
            {
                status = IPStatus.Unknown;
                roundTripTime = 0;
                address = null;
            }

            public PingInfo(PingReply reply)
            {
                status = reply.Status;
                roundTripTime = reply.RoundtripTime;
                address = reply.Address;
            }
        }

        private class PingBar : IDisposable
        {
            private float length;
            private Brush brush;
            private RectangleF rectangle;
            private float positionY;
            private PingInfo pingInfo;

            public float Length
            {
                get
                {
                    return length;
                }
            }

            public Brush Brush
            {
                get
                {
                    return brush;
                }
                set
                {
                    brush = value;
                }
            }

            public RectangleF Rectangle
            {
                get
                {
                    return rectangle;
                }
            }

            public float PositionY
            {
                get
                {
                    return positionY;
                }
            }

            public PingInfo PingInfo
            {
                get
                {
                    return pingInfo;
                }
            }

            public PingBar()
            {
            }

            public PingBar(float length, Brush brush, RectangleF rectangle, float positionY, PingInfo pingInfo)
            {
                this.length = length;
                this.brush = brush;
                this.rectangle = rectangle;
                this.positionY = positionY;
                this.pingInfo = pingInfo;
            }

            public void Dispose()
            {
                ((IDisposable)brush).Dispose();
            }
        }

        private class MouseOverInfo
        {
            public PointF MouseLoc { get; set; }
            public PingInfo PingReply { get; set; }

            public MouseOverInfo(PointF mouseLoc, PingInfo pingInfo)
            {
                MouseLoc = mouseLoc;
                PingReply = pingInfo;
            }
        }

        public class PingEventArgs : EventArgs
        {
            public PingInfo PingReply { get; set; }

            public PingEventArgs(PingInfo pingReply)
            {
                PingReply = pingReply;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    targetControl.MouseWheel -= TargetControl_MouseWheel;
                    targetControl.MouseLeave -= TargetControl_MouseLeave;
                    targetControl.MouseMove -= TargetControl_MouseMove;
                    targetControl.VisibleChanged -= TargetControl_VisibleChanged;

                    pingTimer.Stop();

                    disposeEvent.Set();
                    renderEvent.Set();

                    renderTask.Wait();
                    renderTask.Dispose();

                    scaleEaseTimer.Stop();
                    scaleEaseTimer.Dispose();
                    scaleEaseTimer = null;

                    upscaledImage.Dispose();
                    upscaledGraphics.Dispose();

                    downsampledImage.Dispose();
                    downsampleGraphics.Dispose();

                    if (currentBarList == null)
                    {
                        DisposeBarList(currentBarList);
                        currentBarList = null;
                    }

                    pingTimer.Dispose();
                    pingTimer = null;

                    ping.Dispose();
                    pingReplies.Clear();
                    pingReplies = null;

                    disposeEvent.Dispose();
                    renderEvent.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}