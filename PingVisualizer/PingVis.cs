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
        private ManualResetEvent refreshBarsEvent = new ManualResetEvent(false);

        private Task renderTask;

        private Graphics upscaledGraphics;
        private Bitmap upscaledImage;
        private Size upscaledImageSize;

        private int imageSuperSampleMulti = 5;
        private Graphics supersampleGraphics;
        private Bitmap supersampledImage;
        private Size supersampleImageSize;

        private Ping ping = new Ping();
        private List<PingInfo> pingReplies = new List<PingInfo>();
        private System.Diagnostics.Stopwatch pingLoopTimer = new System.Diagnostics.Stopwatch();

        private string hostname;
        private Control targetControl;
        private Form targetControlForm;

        private float targetViewScale;
        private float currentViewScale;
        private float scaleEaseStartValue;
        private float prevTargetScaleValue;

        private System.Timers.Timer scaleEaseTimer;
        private System.Diagnostics.Stopwatch scaleEaseStopwatch = new System.Diagnostics.Stopwatch();

        private float infoFontSize;
        private float overInfoFontSize;
        private Font mousePingInfoFont;
        private Font pingInfoFont;
        private Brush mouseOverTextBrush = new SolidBrush(Color.FromArgb(240, Color.White));
        private Brush mouseOverBarBrush = new SolidBrush(Color.FromArgb(128, Color.Navy));
        private Color mouseScrollingBackColor = Color.FromArgb(48, 53, 61);
        private MouseOverInfo mouseOverInfo;
        private bool mouseIsOverBars = false;
        private PointF mouseLocationScaled;
        private bool mouseIsScrolling = false;

        private int topIndex = 0;
        private List<PingBar> currentBarList = new List<PingBar>();

        private const int pingTimeOut = 1000;
        private const int maxBadPing = 300; // Ping time at which the bar color will be fully red.
        private const int maxViewScaleLines = 30; // Scale lines will fade out and stop being drawn after this is reached.
        private const int goodPingInterval = 500;
        private const int noPingInterval = 3000;
        private int currentPingInterval = goodPingInterval;

        private const int maxViewScale = 15; // Max "zoom" level for very low latency ping bars.
        private const float barGap = 0; // Y coord gap between bars.
        private const int maxBars = 15; // Number of bars to display.
        private const int minBarLength = 1; // Min X coord length.
        private const float barTopPadding = 0; // Padding added to top of bar list.
        private const float barBottomPadding = 6; // Padding added to bottom of bar list.
        private float calcBarHeight;

        private const int maxStoredResults = 1000000; // Number of results to store before culling occurs.
        private const int maxDrawRateFPS = 100; // Max FPS allowed.
        private int minFrameTime;
        private System.Diagnostics.Stopwatch fpsTimer = new System.Diagnostics.Stopwatch();

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
            minFrameTime = 1000 / maxDrawRateFPS;

            upscaledImageSize = new Size(targetControl.ClientSize.Width * imageSuperSampleMulti, targetControl.ClientSize.Height * imageSuperSampleMulti);
            supersampleImageSize = targetControl.ClientSize;

            calcBarHeight = (upscaledImageSize.Height - barBottomPadding - barTopPadding - (barGap * maxBars)) / maxBars;

            upscaledImage = new Bitmap(upscaledImageSize.Width, upscaledImageSize.Height, PixelFormat.Format32bppPArgb);
            upscaledGraphics = Graphics.FromImage(upscaledImage);
            upscaledGraphics.SmoothingMode = SmoothingMode.None;
            upscaledGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            upscaledGraphics.TextContrast = 0;

            supersampledImage = new Bitmap(supersampleImageSize.Width, supersampleImageSize.Height, PixelFormat.Format32bppPArgb);
            supersampledImage.SetResolution(upscaledImage.HorizontalResolution, upscaledImage.VerticalResolution);
            supersampleGraphics = Graphics.FromImage(supersampledImage);
            supersampleGraphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

            overInfoFontSize = 7 * imageSuperSampleMulti;
            infoFontSize = 8 * imageSuperSampleMulti;
            mousePingInfoFont = new Font("Tahoma", overInfoFontSize, FontStyle.Regular);
            pingInfoFont = new Font("Tahoma", infoFontSize, FontStyle.Bold);
        }

        private void InitControl(Control targetControl)
        {
            this.targetControl = targetControl;
            this.targetControlForm = targetControl.FindForm();

            targetControl.VisibleChanged -= TargetControl_VisibleChanged;
            targetControl.VisibleChanged += TargetControl_VisibleChanged;

            targetControl.MouseWheel -= TargetControl_MouseWheel;
            targetControl.MouseWheel += TargetControl_MouseWheel;

            targetControl.MouseLeave -= TargetControl_MouseLeave;
            targetControl.MouseLeave += TargetControl_MouseLeave;

            targetControl.MouseMove -= TargetControl_MouseMove;
            targetControl.MouseMove += TargetControl_MouseMove;
        }

        private void InitScaleTimer()
        {
            scaleEaseTimer = new System.Timers.Timer();
            scaleEaseTimer.Elapsed += ScaleEaseTimer_Elapsed;
            scaleEaseTimer.Interval = minFrameTime;
        }

        private void ScaleEaseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!this.isDisposing)
                EaseScaleChange();
        }

        private void InitPing()
        {
            ServicePointManager.DnsRefreshTimeout = 0;
            DoPingLoop();
        }

        /// <summary>
        /// Starts the adaptive wait ping loop.
        /// </summary>
        /// <returns></returns>
        private async Task DoPingLoop()
        {
            // Start a loop within an async task.
            await Task.Run(() =>
             {
                 do
                 {
                     try
                     {
                         // Start/Reset the loop timer.
                         pingLoopTimer.Restart();

                         // Get a new ping reply.
                         var reply = GetPingReply(hostname);

                         // Add the new reply.
                         AddPingReply(new PingInfo(reply));
                     }
                     catch (Exception)
                     {
                         // If theres and exception during the ping, add an empty reply.
                         // Empty replies display with an "ERR" message.
                         if (!this.isDisposing)
                         {
                             AddPingReply(new PingInfo());
                         }
                     }

                     // Determine how long we need to wait to meet the current ping interval.
                     // A longer ping time = shorter wait period.
                     var waitTime = currentPingInterval - (int)pingLoopTimer.ElapsedMilliseconds;

                     if (this.isDisposing)
                         return;

                     // If we have a positive wait time, pause the thread.
                     if (waitTime > 0)
                         Thread.Sleep(waitTime);

                     //Fire off a new render event.
                     if (!this.isDisposing)
                     {
                         if (!mouseIsScrolling)
                         {
                             Render(true);
                         }
                         else
                         {
                             Render(false);
                         }
                     }
                 }
                 while (!this.isDisposing);
             });
        }

        private void SetPingInterval(int interval)
        {
            if (this.isDisposing) return;

            if (currentPingInterval != interval)
            {
                currentPingInterval = interval;
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
                // If we have no replies yet, only add a new one if it was a success.
                if (pingReplies.Count == 0 && reply.Status == IPStatus.Success)
                {
                    pingReplies.Add(reply);
                    OnNewPingResult(reply);
                }
                // Otherwise, always add.
                else if (pingReplies.Count > 0)
                {
                    pingReplies.Add(reply);
                    OnNewPingResult(reply);
                }
            }

            // Set the ping interval according to the reply status.
            // Failed replies will increase the interval.
            if (reply.Status == IPStatus.Success)
            {
                SetPingInterval(goodPingInterval);
            }
            else
            {
                SetPingInterval(noPingInterval);
            }
        }

        private PingReply GetPingReply(string hostname)
        {
            var options = new PingOptions();
            options.DontFragment = true;
            byte[] buff = Encoding.ASCII.GetBytes("ping");

            return ping.Send(hostname, pingTimeOut, buff, options);
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
                    targetViewScale = newScale;

                // Start the scale timer if needed.
                if (currentViewScale != targetViewScale)
                    scaleEaseTimer.Start();
            }
        }

        /// <summary>
        /// Smoothly eases between scale changes.
        /// </summary>
        private void EaseScaleChange()
        {
            // How long we want the ease operation to take.
            float duration = 1000f;

            if (currentViewScale != targetViewScale)
            {
                // If no ease operation in progress, set the scale parameters and start the stopwatch.
                // If the target scale is changed during an easing operation, reset the parameters and stopwatch.
                if (!scaleEaseStopwatch.IsRunning || targetViewScale != prevTargetScaleValue)
                {
                    scaleEaseStartValue = currentViewScale;
                    prevTargetScaleValue = targetViewScale;

                    scaleEaseStopwatch.Restart();
                }

                // Calculate the absolute diffence between the current and target scale.
                float diffAbs = Math.Abs(currentViewScale - targetViewScale);

                // Calculate the current progress or position of the ease operation.
                float position = scaleEaseStopwatch.ElapsedMilliseconds / duration;

                // Apply an easing function to the position to get the factor.
                double factor = EaseQuinticOut(position);

                // Apply the factor to the starting and target values to get the next scale value.
                float newScale = (float)(scaleEaseStartValue + (targetViewScale - scaleEaseStartValue) * factor);

                // If the elapsed time of this current ease operation is less than the desired duration, apply the new scale value.
                if (scaleEaseStopwatch.ElapsedMilliseconds < duration && !isDisposing)
                {
                    currentViewScale = newScale;
                }
                else
                {
                    // Ease operation complete, set to final scale and stop the operation.
                    currentViewScale = targetViewScale;
                    scaleEaseStopwatch.Stop();
                    scaleEaseStopwatch.Reset();
                    scaleEaseTimer.Stop();
                }

                // Fire a new render event.
                Render();
            }
        }

        #region "Easing Functions"

        private double EaseCircleIn(float k)
        {
            return 1f - Math.Sqrt(1f - (k * k));
        }

        private double EaseCircleOut(float k)
        {
            return Math.Sqrt(1f - ((k -= 1f) * k));
        }

        private double EaseCircleInOut(float k)
        {
            if ((k *= 2f) < 1f) return -0.5f * (Math.Sqrt(1f - k * k) - 1);
            return 0.5f * (Math.Sqrt(1f - (k -= 2f) * k) + 1f);
        }

        private double EaseQuinticOut(float k)
        {
            return 1f + ((k -= 1f) * Math.Pow(k, 4));
        }

        private double EaseElasticOut(float time)
        {
            if (time == 0) return 0;
            if (time == 1) return 1;
            return Math.Pow(2f, -10f * time) * Math.Sin((time - 0.1f) * (2f * Math.PI) / 0.4f) + 1f;
        }

        #endregion "Easing Functions"

        private void TargetControl_VisibleChanged(object sender, EventArgs e)
        {
            if (targetControl.Visible)
                Render(true);
        }

        private void TargetControl_MouseLeave(object sender, EventArgs e)
        {
            mouseIsScrolling = false;
            Render(true);
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
                            Render(true);
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
                    Render(true);
                }
            }
        }

        private void TargetControl_MouseMove(object sender, MouseEventArgs e)
        {
            mouseLocationScaled = new PointF(e.Location.X * imageSuperSampleMulti, e.Location.Y * imageSuperSampleMulti);

            if (mouseIsScrolling)
            {
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

        private bool MouseIsOverBars()
        {
            for (int i = 0; i < currentBarList.Count; i++)
            {
                if (currentBarList[i].Rectangle.Contains(mouseLocationScaled))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Triggers a new rendering event. This method sets the events which control the <see cref="RenderLoop"/>.
        /// </summary>
        /// <param name="refreshPingBars">When true this call will trigger a refresh of the ping bars, which will display the most current results.</param>
        private void Render(bool refreshPingBars = false)
        {
            if (pingReplies.Count < 1)
                return;

            // Only render if the target control is visible.
            if (targetControl != null & targetControlForm != null)
            {
                if (targetControlForm.WindowState == FormWindowState.Minimized) return;
                if (!targetControl.Visible) return;
            }

            // If a refresh bars is requested, set the event.
            if (refreshPingBars)
                refreshBarsEvent.Set();

            // Set the render event to trigger a new rendering.
            renderEvent.Set();
        }

        /// <summary>
        /// The main rendering loop. Performs the rendering methods in a tight loop using <see cref="ManualResetEvent"/> to control the cycles.
        /// </summary>
        private void RenderLoop()
        {
            try
            {
                while (!this.isDisposing)
                {
                    // Wait until a render event is triggered.
                    renderEvent.WaitOne(Timeout.Infinite);

                    // Check for diposal event and break from loop if needed.
                    if (disposeEvent.WaitOne(0))
                        break;

                    // Reset the render event only if we made it past the disposal check.
                    renderEvent.Reset();

                    // Delay this rendering as needed to stay within the maximum FPS limit.
                    WaitDraw();

                    // Check the refresh bars event and perform if needed.
                    if (refreshBarsEvent.WaitOne(0))
                    {
                        RefreshPingBars();
                        refreshBarsEvent.Reset();
                    }

                    // Perform the drawing methods:

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

                    // One last disposal check.
                    if (disposeEvent.WaitOne(0))
                        break;
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
            // How many milliseconds a scale line will denote.
            int msPerStep = 15;

            // Calculate the drawing distance between each scale line.
            float stepSize = currentViewScale * msPerStep;

            // Calculate the number of scale lines that will fit within the current viewport.
            int numOfLines = (int)(upscaledImageSize.Width / stepSize);

            // Only draw the scale lines if we are below the max number allowed.
            if (numOfLines < maxViewScaleLines)
            {
                // Set the initial X position.
                float scaleXPos = stepSize;
                Color backColor;

                // Change the back color depending on the scrolling status.
                if (mouseIsScrolling)
                {
                    backColor = mouseScrollingBackColor;
                }
                else
                {
                    backColor = Color.Black;
                }

                // Create a new pen with a variable color:
                // The pen color is gradually blended with the back color as the number of scale lines approaches the max allowed.
                // This gives the effect of the the scale lines fading out as the scale increases.
                using (var pen = new Pen(GetVariableColor(Color.White, backColor, maxViewScaleLines, numOfLines), 2))
                {
                    for (int a = 0; a < numOfLines; a++)
                    {
                        // Draw a vertical scale line.
                        upscaledGraphics.DrawLine(pen, new PointF(scaleXPos, 0), new PointF(scaleXPos, upscaledImageSize.Height));

                        // Increment the X position by the step size.
                        scaleXPos += stepSize;
                    }
                }
            }
        }

        private void DrawPingBars()
        {
            foreach (var bar in currentBarList)
            {
                if (mouseIsScrolling && bar.Rectangle.Contains(mouseLocationScaled))
                {
                    // If mouse is over a bar, set the mouseOverInfo to be drawn in the DrawPingText method.
                    mouseOverInfo = new MouseOverInfo(mouseLocationScaled, bar.PingInfo);
                    upscaledGraphics.FillRectangle(mouseOverBarBrush, new RectangleF(bar.Rectangle.Location, new SizeF(bar.Length * currentViewScale, bar.Rectangle.Height)));
                }
                else
                {
                    using (var barBrush = new SolidBrush(bar.Color))
                    {
                        upscaledGraphics.FillRectangle(barBrush, new RectangleF(bar.Rectangle.Location, new SizeF(bar.Length * currentViewScale, bar.Rectangle.Height)));
                    }
                }
            }
        }

        private void DrawPingText()
        {
            if (mouseIsScrolling)
            {
                // Draw ping info for the highlighted bar.
                if (mouseIsOverBars)
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

                upscaledGraphics.FillRectangle(Brushes.White, new RectangleF(upscaledImageSize.Width - (20 + imageSuperSampleMulti), scrollYPos, 10 + imageSuperSampleMulti, 5 + imageSuperSampleMulti));
            }
        }

        /// <summary>
        /// Resamples the upscaled image down to the final size using high quality interpolation.
        /// </summary>
        /// <remarks>
        ///  This effectively "supersamples" the image to produce smooth, legible graphics at a very small size.
        /// </remarks>
        private void DownsampleImage()
        {
            var destRect = new Rectangle(0, 0, supersampleImageSize.Width, supersampleImageSize.Height);
            supersampleGraphics.DrawImage(upscaledImage, destRect);
        }

        private Color GetVariableColor(Color startColor, Color endColor, int maxValue, long currentValue, bool translucent = false)
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
                return Color.FromArgb(220, newR, newG, newB);
            }
            else
            {
                return Color.FromArgb(newR, newG, newB);
            }
        }

        private void WaitDraw()
        {
            if (fpsTimer.IsRunning)
            {
                long elapTime = fpsTimer.ElapsedMilliseconds;
                fpsTimer.Reset();

                if (elapTime >= minFrameTime)
                {
                    return;
                }
                else
                {
                    var waitTime = (int)(minFrameTime - elapTime);
                    Thread.Sleep(waitTime);
                    return;
                }
            }
            else
            {
                fpsTimer.Start();
                return;
            }
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
            currentBarList?.Clear();

            SetScale();

            float currentYPos = barTopPadding;

            foreach (PingInfo result in CurrentDisplayResults())
            {
                float barLen;
                Color barColor;

                if (result.Status == IPStatus.Success)
                {
                    barColor = GetVariableColor(Color.Green, Color.Red, maxBadPing, result.RoundTripTime, true);
                    barLen = result.RoundTripTime + minBarLength;
                    if (barLen < minBarLength) barLen = minBarLength;
                }
                else
                {
                    barColor = Color.FromArgb(200, Color.Red);
                    barLen = pingTimeOut * maxViewScale;
                }

                currentBarList.Add(new PingBar(barLen, barColor, new RectangleF(1, currentYPos, barLen * currentViewScale, calcBarHeight), result));
                currentYPos += calcBarHeight + barGap;
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
                var asyncResult = targetControl.BeginInvoke(del);
                asyncResult.AsyncWaitHandle.WaitOne(minFrameTime);
            }
            else
            {
                if (!(targetControl is Form))
                {
                    if (targetControl is Button)
                    {
                        var but = (Button)targetControl;
                        but.Image = supersampledImage;
                        but.Invalidate();
                    }
                    else if (targetControl is PictureBox)
                    {
                        var pic = (PictureBox)targetControl;
                        pic.Image = supersampledImage;
                        pic.Invalidate();
                    }
                }
                else
                {
                    targetControl.BackgroundImage = supersampledImage;
                    targetControl.Invalidate();
                }
            }
        }

        public class PingInfo
        {
            public IPStatus Status { get; }
            public long RoundTripTime { get; }
            public IPAddress Address { get; }

            public PingInfo()
            {
                Status = IPStatus.Unknown;
                RoundTripTime = 0;
                Address = null;
            }

            public PingInfo(PingReply reply)
            {
                Status = reply.Status;
                RoundTripTime = reply.RoundtripTime;
                Address = reply.Address;
            }
        }

        private struct PingBar
        {
            public float Length { get; set; }
            public Color Color { get; set; }
            public RectangleF Rectangle { get; set; }
            public PingInfo PingInfo { get; set; }

            public PingBar(float length, Color color, RectangleF rectangle, PingInfo pingInfo)
            {
                this.Length = length;
                this.Color = color;
                this.Rectangle = rectangle;
                this.PingInfo = pingInfo;
            }
        }

        private struct MouseOverInfo
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
        private bool isDisposing = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    isDisposing = true;

                    targetControl.MouseWheel -= TargetControl_MouseWheel;
                    targetControl.MouseLeave -= TargetControl_MouseLeave;
                    targetControl.MouseMove -= TargetControl_MouseMove;
                    targetControl.VisibleChanged -= TargetControl_VisibleChanged;

                    disposeEvent.Set();
                    renderEvent.Set();

                    renderTask?.Wait();
                    renderTask?.Dispose();

                    scaleEaseTimer.Stop();
                    scaleEaseTimer.Dispose();
                    scaleEaseTimer = null;

                    upscaledImage.Dispose();
                    upscaledGraphics.Dispose();

                    supersampledImage.Dispose();
                    supersampleGraphics.Dispose();

                    if (currentBarList != null)
                    {
                        currentBarList.Clear();
                        currentBarList = null;
                    }

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