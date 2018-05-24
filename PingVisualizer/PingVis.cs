using System;
using System.Collections.Generic;
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
        private Thread renderThread;

        private Graphics upscaledGraphics;
        private Bitmap upscaledImage;
        private int imageUpscaleMulti = 5;
        private Size upscaledImageSize;

        private Graphics downsampleGraphics;
        private Bitmap downsampleImage;
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
        private MouseOverInfo mouseOverInfo = null;
        private Point mouseLocation;
        private bool mouseIsScrolling = false;

        private int topIndex = 0;
        private List<PingBar> currentBarList = new List<PingBar>();

        private const int pingTimeOut = 1000;
        private const int maxBadPing = 300; // Ping time at which the bar color will be fully red.
        private const int maxViewScaleLines = 30; // Scale lines will fade out and stop being drawn after this is reached.
        private const int goodPingInterval = 1000;
        private const int noPingInterval = 3000;
        private int currentPingInterval = goodPingInterval;

        private const int maxViewScale = 15;
        private const float barGap = 0;
        private const int maxBars = 15;
        private const int minBarLength = 1;
        private const float barTopPadding = 0;
        private const float barBottomPadding = 6;

        private const int maxStoredResults = 1000000;
        private int maxDrawRateFPS = 100;
        private long lastDrawTime = 0;

        private object drawThreadLockObject = new object();

        public event EventHandler<PingEventArgs> NewPingResult;

        private delegate void SetControlImageDelegate(Control targetControl, Bitmap image);

        public void OnNewPingResult(PingInfo pingReply)
        {
            NewPingResult?.Invoke(this, new PingEventArgs(pingReply));
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

        public void ClearResults()
        {
            pingReplies.Clear();
        }

        public PingVis(Control targetControl, string hostName)
        {
            InitControl(targetControl);
            this.hostname = hostName;
            InitGraphics();
            InitScaleTimer();
            InitPing();

            // Start the rendering loop on a new thread.
            renderThread = new Thread(RenderLoop);
            renderThread.Start();
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

            downsampleImage = new Bitmap(downsampleImageSize.Width, downsampleImageSize.Height, PixelFormat.Format32bppPArgb);
            downsampleImage.SetResolution(upscaledImage.HorizontalResolution, upscaledImage.VerticalResolution);
            downsampleGraphics = Graphics.FromImage(downsampleImage);
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

            targetControl.MouseWheel -= ControlMouseWheel;
            targetControl.MouseWheel += ControlMouseWheel;

            targetControl.MouseLeave -= ControlMouseLeave;
            targetControl.MouseLeave += ControlMouseLeave;

            targetControl.MouseMove -= ControlMouseMove;
            targetControl.MouseMove += ControlMouseMove;
        }

        private void InitPing()
        {
            ServicePointManager.DnsRefreshTimeout = 0;
            InitPingTimer();
        }

        private void InitPingTimer()
        {
            if (pingTimer == null)
            {
                pingTimer = new System.Timers.Timer();
            }
            pingTimer.Elapsed += PingTimer_Elapsed;
            pingTimer.SynchronizingObject = targetControl;
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
            scaleEaseTimer.SynchronizingObject = targetControl;
            scaleEaseTimer.Interval = scaleEaseTimerInterval;
        }

        private void ScaleEaseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!this.disposedValue)
            {
                EaseScaleChange();
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

                if (targetViewScale != newScale)
                {
                    targetViewScale = newScale;

                    if (mouseIsScrolling)
                    {
                        currentViewScale = targetViewScale;
                    }
                    else
                    {
                        scaleEaseTimer.Interval = scaleEaseTimerInterval;
                        scaleEaseTimer.Start();
                    }
                }
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
                return await ping.SendPingAsync(hostname, pingTimeOut, buff, options);
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

        private void ControlMouseLeave(object sender, EventArgs e)
        {
            mouseIsScrolling = false;
            Render(true, true);
        }

        private void ControlMouseWheel(object sender, MouseEventArgs e)
        {
            if (pingReplies.Count > maxBars)
            {
                int newIdx = 0;
                mouseIsScrolling = true;
                if (e.Delta < 0) // Scroll up.
                {
                    newIdx = topIndex + 1;
                    //if the scroll index returns to the end (bottom) of the results, disable scrolling and return to normal display
                    if (newIdx >= pingReplies.Count - maxBars)
                    {
                        newIdx = pingReplies.Count - maxBars;
                        if (mouseIsScrolling)
                        {
                            mouseIsScrolling = false;
                        }
                    }
                }
                else if (e.Delta > 0) // Scroll down.
                {
                    newIdx = topIndex - 1;
                    if (newIdx < 0) // Clamp the index to never be negative.
                    {
                        newIdx = 0;
                    }
                }
                if (topIndex != newIdx)
                {
                    topIndex = newIdx;
                    Render(true, true);
                }
            }
        }

        private int moves = 0;

        private void ControlMouseMove(object sender, MouseEventArgs e)
        {
            moves++;
            mouseLocation = e.Location;
            if (mouseIsScrolling)
            {
                // Limit the rate of Render calls.
                // This event can fire very rapidly which can flood the Renderer
                // preventing other calls from getting past the frame rate limiter.
                if ((moves >= 2))
                {
                    moves = 0;

                    // We don't want to render after every single mouse movement, so we make
                    // sure we only render if the mouse is actually over a bar. Then when
                    // the mouse leaves a bar, we render once to reset any highlighted bars.

                    // GetMouseOverInfo returns null when the mouse is not over a bar.
                    // Get the mouse over info.
                    var newMouseOverInfo = GetMouseOverInfo();

                    // If the mouse is over a bar, set the class member and render.
                    if (newMouseOverInfo != null)
                    {
                        mouseOverInfo = newMouseOverInfo;
                        Render();
                    }
                    else
                    {
                        // If the mouse is not over a bar, and the class member is not null,
                        // null the member and render once. Future calls will not render unless
                        // the mouse is again over a bar.

                        if (mouseOverInfo != null)
                        {
                            mouseOverInfo = null;
                            Render();
                        }
                    }
                }
            }
        }

        private MouseOverInfo GetMouseOverInfo()
        {
            var mScalePoint = new PointF(mouseLocation.X * imageUpscaleMulti, mouseLocation.Y * imageUpscaleMulti);

            foreach (PingBar bar in currentBarList)
            {
                if (bar.Rectangle.Contains(mScalePoint))
                {
                    return new MouseOverInfo(mScalePoint, bar.PingInfo);
                }
            }
            return null;
        }

        private bool MouseIsOverBar(PingBar bar)
        {
            var mScalePoint = new PointF(mouseLocation.X * imageUpscaleMulti, mouseLocation.Y * imageUpscaleMulti);
            if (bar.Rectangle.Contains(mScalePoint))
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
            while (true)
            {
                // Block until a render event is triggered.
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
                    upscaledGraphics.Clear(Color.FromArgb(48, 53, 61));
                }

                // Draw all the elements from back to front.
                DrawScaleLines(upscaledGraphics);
                DrawPingBars(upscaledGraphics, currentBarList);
                DrawPingText(upscaledGraphics);
                DrawScrollBar(upscaledGraphics);
                TrimPingList();

                // Resample the image to the original size.
                DownsampleImage();

                // Set the target control image.
                SetControlImage(targetControl, downsampleImage);

                // Reset the render event to wait until another render is triggered.
                renderEvent.Reset();
            }
        }

        private void DrawScaleLines(Graphics gfx)
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
                        gfx.DrawLine(pen, new PointF(scaleXPos, 0), new PointF(scaleXPos, upscaledImageSize.Height));
                        scaleXPos += stepSize;
                    }
                }
            }
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

        private void DrawPingBars(Graphics gfx, List<PingBar> bars)
        {
            foreach (var bar in bars)
            {
                if (mouseIsScrolling && MouseIsOverBar(bar))
                {
                    gfx.FillRectangle(mouseOverBarBrush, new RectangleF(bar.Rectangle.Location, new SizeF(bar.Length * currentViewScale, bar.Rectangle.Height)));
                }
                else
                {
                    gfx.FillRectangle(bar.Brush, new RectangleF(bar.Rectangle.Location, new SizeF(bar.Length * currentViewScale, bar.Rectangle.Height)));
                }
            }
        }

        private void DrawPingText(Graphics gfx)
        {
            if (mouseIsScrolling)
            {
                if (mouseOverInfo != null)
                {
                    string overInfoText = GetReplyStatusText(mouseOverInfo.PingReply);
                    SizeF textSize = gfx.MeasureString(overInfoText, mousePingInfoFont);
                    gfx.DrawString(overInfoText, mousePingInfoFont, mouseOverTextBrush, new PointF(mouseOverInfo.MouseLoc.X + (textSize.Width * 0.5f), mouseOverInfo.MouseLoc.Y - (textSize.Height * 0.5f)));
                }
            }
            else
            {
                string infoText = GetReplyStatusText(pingReplies.Last());
                SizeF textSize = gfx.MeasureString(infoText, pingInfoFont);
                gfx.DrawString(infoText, pingInfoFont, Brushes.White, new PointF((upscaledImageSize.Width - 5) - (textSize.Width), upscaledImageSize.Height - (textSize.Height + 5)));
            }
        }

        private void DrawScrollBar(Graphics gfx)
        {
            if (mouseIsScrolling)
            {
                float scrollLocation = 0;
                if (topIndex > 0)
                {
                    scrollLocation = (upscaledImageSize.Height / (pingReplies.Count / (float)topIndex));
                }
                gfx.FillRectangle(Brushes.White, new RectangleF(upscaledImageSize.Width - (20 + imageUpscaleMulti), scrollLocation, 10 + imageUpscaleMulti, 5 + imageUpscaleMulti));
            }
        }

        private void DownsampleImage()
        {
            var destRect = new Rectangle(0, 0, downsampleImageSize.Width, downsampleImageSize.Height);
            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                downsampleGraphics.DrawImage(upscaledImage, destRect, 0, 0, upscaledImage.Width, upscaledImage.Height, GraphicsUnit.Pixel, wrapMode);
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

        private int FirstDrawIndex(int barCount)
        {
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
                return pingReplies.GetRange(FirstDrawIndex(pingReplies.Count), maxBars);
            }
            else
            {
                return pingReplies;
            }
        }

        private void SetControlImage(Control targetControl, Bitmap image)
        {
            if (targetControl.InvokeRequired)
            {
                var del = new SetControlImageDelegate(SetControlImage);
                targetControl.BeginInvoke(del, new object[] { targetControl, image });
            }
            else
            {
                if (!(targetControl is Form))
                {
                    if (targetControl is Button)
                    {
                        var but = (Button)targetControl;
                        but.Image = image;
                        but.Invalidate();
                    }
                    else if (targetControl is PictureBox)
                    {
                        var pic = (PictureBox)targetControl;
                        pic.Image = image;
                        pic.Refresh();
                    }
                }
                else
                {
                    targetControl.BackgroundImage = image;
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
                    disposeEvent.Set();
                    renderEvent.Set();

                    pingTimer.Stop();
                    pingTimer.Dispose();
                    pingTimer = null;

                    scaleEaseTimer.Stop();
                    scaleEaseTimer.Dispose();
                    scaleEaseTimer = null;

                    targetControl.MouseWheel -= ControlMouseWheel;
                    targetControl.MouseLeave -= ControlMouseLeave;
                    targetControl.MouseMove -= ControlMouseMove;

                    ping.Dispose();
                    pingReplies.Clear();
                    pingReplies = null;

                    upscaledImage.Dispose();
                    upscaledGraphics.Dispose();

                    downsampleImage.Dispose();
                    downsampleGraphics.Dispose();

                    if (currentBarList == null)
                    {
                        DisposeBarList(currentBarList);
                        currentBarList = null;
                    }
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