using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingVisualizer
{
    public class PingVis : IDisposable
    {
        private Graphics scaledGraphics;

        private Bitmap scaledImage;
        private int imageScaleMulti = 5;
        private int scaledImageWidth;
        private int scaledImageHeight;

        private Graphics controlGraphics;
        private Bitmap controlImage;
        private int origImageWidth;
        private int origImageHeight;

        private Ping ping = new Ping();
        private List<PingInfo> pingReplies = new List<PingInfo>();
        private bool pingRunning = false;
        private Timer pingTimer = new Timer();

        private string hostname;
        private Control targetControl;

        private float targetScale;
        private float currentScale;
        private Timer scaleTimer = new Timer();

        private Brush mouseOverTextBrush = new SolidBrush(Color.FromArgb(240, Color.White));
        private Brush mouseOverBarBrush = new SolidBrush(Color.FromArgb(128, Color.Navy));
        private Point mouseLocation;
        private bool mouseIsScrolling = false;

        private int topIndex = 0;
        private List<PingBar> currentBarList = new List<PingBar>();

        private const int timeOut = 1000;
        private const int maxBadPing = 300; // Ping time at which the bar color will be fully red.
        private const int maxScaleLines = 30; // Scale lines will fade out and stop being drawn after this is reached.
        private const int goodPingInterval = 1000;
        private const int noPingInterval = 3000;
        private int currentPingInterval = goodPingInterval;

        private const int maxDrawScale = 10;
        private const float barGap = 0;
        private const int maxBars = 10;
        private const int minBarLength = 1;
        private const float barTopPadding = 0;
        private const float barBottomPadding = 6;

        private const int maxStoredResults = 1000000;
        private const int maxDrawRatePerMilliseconds = 10;
        private long lastDrawTime = 0;

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
        }

        private void InitGraphics()
        {
            scaledImage = new Bitmap(scaledImageWidth, scaledImageHeight, PixelFormat.Format32bppPArgb);
            scaledGraphics = Graphics.FromImage(scaledImage);

            controlImage = new Bitmap(origImageWidth, origImageHeight, PixelFormat.Format32bppPArgb);
            controlImage.SetResolution(scaledImage.HorizontalResolution, scaledImage.VerticalResolution);

            controlGraphics = Graphics.FromImage(controlImage);
            controlGraphics.CompositingMode = CompositingMode.SourceCopy;
            controlGraphics.CompositingQuality = CompositingQuality.HighSpeed;
            controlGraphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            controlGraphics.SmoothingMode = SmoothingMode.None;
            controlGraphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        }

        private void InitControl(Control targetControl)
        {
            this.targetControl = targetControl;
            scaledImageWidth = targetControl.ClientSize.Width * imageScaleMulti;
            scaledImageHeight = targetControl.ClientSize.Height * imageScaleMulti;

            origImageWidth = targetControl.ClientSize.Width;
            origImageHeight = targetControl.ClientSize.Height;

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
            StartPing();
        }

        private void InitPingTimer()
        {
            if (!this.disposedValue)
            {
                if (pingTimer != null)
                {
                    pingTimer.Tick -= PingTimer_Tick;
                    pingTimer.Dispose();
                    pingTimer = null;
                    pingTimer = new Timer();
                }
                pingTimer.Interval = currentPingInterval;
                pingTimer.Enabled = true;
                pingTimer.Tick += PingTimer_Tick;
            }
        }

        private void InitScaleTimer()
        {
            scaleTimer.Tick += ScaleTimer_Tick;
            scaleTimer.Interval = 50;
            scaleTimer.Enabled = true;
        }

        private void ScaleTimer_Tick(object sender, EventArgs e)
        {
            if (!this.disposedValue)
            {
                EaseScaleChange();
            }
        }

        private void PingTimer_Tick(object sender, EventArgs e)
        {
            StartPing();
            pingTimer.Interval = currentPingInterval;
        }

        private void SetScale()
        {
            if (CurrentDisplayResults().Count > 0)
            {
                long maxPing = CurrentDisplayResults().OrderByDescending(p => p.RoundTripTime).FirstOrDefault().RoundTripTime;
                if (maxPing <= 0) maxPing = 1;
                float newScale = ((scaledImageWidth / 2f) / maxPing);
                if (newScale > maxDrawScale) newScale = maxDrawScale;

                if (targetScale != newScale)
                {
                    targetScale = newScale;
                    scaleTimer.Enabled = true;
                }
                if (mouseIsScrolling) currentScale = targetScale;
            }
        }

        /// <summary>
        /// Smoothly eases between scale changes.
        /// </summary>
        private void EaseScaleChange()
        {
            if (currentScale != targetScale)
            {
                if (!mouseIsScrolling)
                {
                    // Get the diffence between the current and target.
                    float diff = currentScale - targetScale;
                    float diffAbs = Math.Abs(diff);

                    // If the absolute difference above a certain amount begin/continue easing.
                    if (diffAbs > 0.02)
                    {
                        // Simple easing calulation.
                        if (currentScale > targetScale)
                        {
                            currentScale -= (diffAbs / 5f);
                        }
                        else if (currentScale < targetScale)
                        {
                            currentScale += (diffAbs / 5f);
                        }
                    }
                    else
                    {
                        // Set to final scale and stop timer.
                        currentScale = targetScale;
                        scaleTimer.Enabled = false;
                    }
                    DrawBars();
                }
                else
                {
                    currentScale = targetScale;
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
                    if (reply.Status == IPStatus.Success)
                    {
                        SetPingInterval(goodPingInterval);
                    }
                    else
                    {
                        SetPingInterval(noPingInterval);
                    }
                    pingReplies.Add(new PingInfo(reply));
                }
            }
            catch (Exception)
            {
                if (!this.disposedValue)
                {
                    pingReplies.Add(new PingInfo());
                    SetPingInterval(noPingInterval);
                }
                else
                {
                    this.Dispose();
                }
            }
            finally
            {
                if (!this.disposedValue)
                {
                    if (!mouseIsScrolling)
                    {
                        RefreshPingBars();
                    }

                    DrawBars(true);
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
                return await ping.SendPingAsync(hostname, timeOut, buff, options);
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
                InitPingTimer();
            }
        }

        private void ControlMouseLeave(object sender, EventArgs e)
        {
            mouseIsScrolling = false;
            RefreshPingBars();
            DrawBars();
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
                    if (newIdx > pingReplies.Count - maxBars)
                    {
                        newIdx = pingReplies.Count - maxBars;
                        mouseIsScrolling = false;
                        RefreshPingBars();
                        DrawBars();
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
                    RefreshPingBars();
                    DrawBars();
                }
            }
        }

        private void ControlMouseMove(object sender, MouseEventArgs e)
        {
            mouseLocation = e.Location;
            if (mouseIsScrolling)
            {
                DrawBars();
            }
        }

        private MouseOverInfo GetMouseOverPingBar()
        {
            var mScalePoint = new PointF(mouseLocation.X * imageScaleMulti, mouseLocation.Y * imageScaleMulti);
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
            var mScalePoint = new PointF(mouseLocation.X * imageScaleMulti, mouseLocation.Y * imageScaleMulti);
            if (bar.Rectangle.Contains(mScalePoint))
            {
                return true;
            }
            return false;
        }

        private void DrawBars(bool forceDraw = false)
        {
            if (pingReplies.Count < 1)
            {
                return;
            }
            else
            {
                if (targetControl != null && targetControl.FindForm() != null)
                {
                    if (targetControl.FindForm().WindowState == FormWindowState.Minimized) return;
                }

                if (!forceDraw && !CanDraw(Environment.TickCount))
                {
                    return;
                }
            }

            SetScale();

            scaledGraphics.SmoothingMode = SmoothingMode.None;

            if (!mouseIsScrolling)
            {
                scaledGraphics.Clear(targetControl.BackColor);
            }
            else
            {
                scaledGraphics.Clear(Color.FromArgb(48, 53, 61));
            }

            DrawScaleLines(scaledGraphics);
            DrawPingBars(scaledGraphics, currentBarList);
            DrawPingText(scaledGraphics);
            DrawScrollBar(scaledGraphics);
            TrimPingList();
            ResizeImage();
            SetControlImage(targetControl, controlImage);
        }

        private void DrawScaleLines(Graphics gfx)
        {
            float stepSize = currentScale * 15;
            int numOfLines = (int)(scaledImageWidth / stepSize);
            if (numOfLines < maxScaleLines)
            {
                float scaleXPos = stepSize;
                using (var pen = new Pen(GetVariableBrush(Color.White, Color.Black, maxScaleLines, numOfLines), 2))
                {
                    for (int a = 0; a < numOfLines; a++)
                    {
                        gfx.DrawLine(pen, new PointF(scaleXPos, 0), new PointF(scaleXPos, scaledImageHeight));
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
                // Compute the intensity of the high ping color.
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
                    gfx.FillRectangle(mouseOverBarBrush, new RectangleF(bar.Rectangle.Location, new SizeF(bar.Length * currentScale, bar.Rectangle.Height)));
                }
                else
                {
                    gfx.FillRectangle(bar.Brush, new RectangleF(bar.Rectangle.Location, new SizeF(bar.Length * currentScale, bar.Rectangle.Height)));
                }
            }
        }

        private void DrawPingText(Graphics gfx)
        {
            float infoFontSize = 8 * imageScaleMulti;
            float overInfoFontSize = 7 * imageScaleMulti;

            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            gfx.TextContrast = 0;

            if (mouseIsScrolling)
            {
                var mouseOverBar = GetMouseOverPingBar();

                if (mouseOverBar != null)
                {
                    string overInfoText = GetReplyStatusText(mouseOverBar.PingReply);
                    using (Font overFont = new Font("Tahoma", overInfoFontSize, FontStyle.Regular))
                    {
                        SizeF textSize = gfx.MeasureString(overInfoText, overFont);
                        gfx.DrawString(overInfoText, overFont, mouseOverTextBrush, new PointF(mouseOverBar.MouseLoc.X + (textSize.Width / 2f), mouseOverBar.MouseLoc.Y - (textSize.Height / 2f)));
                    }
                }
            }
            else
            {
                string infoText = GetReplyStatusText(pingReplies.Last());
                using (Font infoFont = new Font("Tahoma", infoFontSize, FontStyle.Bold))
                {
                    SizeF textSize = gfx.MeasureString(infoText, infoFont);
                    gfx.DrawString(infoText, infoFont, Brushes.White, new PointF((scaledImageWidth - 5) - (textSize.Width), scaledImageHeight - (textSize.Height + 5)));
                }
            }
        }

        private void DrawScrollBar(Graphics gfx)
        {
            if (mouseIsScrolling)
            {
                float scrollLocation = 0;
                if (topIndex > 0)
                {
                    scrollLocation = (scaledImageHeight / (pingReplies.Count / (float)topIndex));
                }
                gfx.FillRectangle(Brushes.White, new RectangleF(scaledImageWidth - (20 + imageScaleMulti), scrollLocation, 10 + imageScaleMulti, 5 + imageScaleMulti));
            }
        }

        private void ResizeImage()
        {
            var destRect = new Rectangle(0, 0, origImageWidth, origImageHeight);
            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                controlGraphics.DrawImage(scaledImage, destRect, 0, 0, scaledImage.Width, scaledImage.Height, GraphicsUnit.Pixel, wrapMode);
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
            long elapTime = timeTick - lastDrawTime;
            if (elapTime >= maxDrawRatePerMilliseconds)
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

            float currentYPos = barTopPadding;
            float barHeight = (scaledImageHeight - barBottomPadding - barTopPadding - (barGap * maxBars)) / maxBars;

            foreach (PingInfo result in CurrentDisplayResults())
            {
                float barLen;
                Brush barBrush;
                if (result.Status == IPStatus.Success)
                {
                    barBrush = GetVariableBrush(Color.Green, Color.Red, maxBadPing, result.RoundTripTime, true);
                    barLen = result.RoundTripTime;
                    if (barLen < minBarLength) barLen = minBarLength; ;
                }
                else
                {
                    barBrush = new SolidBrush(Color.FromArgb(200, Color.Red));
                    barLen = scaledImageWidth - 2;
                }
                currentBarList.Add(new PingBar(barLen, barBrush, new RectangleF(1, currentYPos, barLen * currentScale, barHeight), currentYPos, result));
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

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    pingTimer.Enabled = false;
                    pingTimer.Tick -= PingTimer_Tick;
                    pingTimer.Dispose();
                    pingTimer = null;

                    scaleTimer.Enabled = false;
                    scaleTimer.Tick -= ScaleTimer_Tick;
                    scaleTimer.Dispose();
                    scaleTimer = null;

                    targetControl.MouseWheel -= ControlMouseWheel;
                    targetControl.MouseLeave -= ControlMouseLeave;
                    targetControl.MouseMove -= ControlMouseMove;

                    ping.Dispose();
                    pingReplies.Clear();
                    pingReplies = null;

                    scaledImage.Dispose();
                    scaledGraphics.Dispose();

                    controlImage.Dispose();
                    controlGraphics.Dispose();

                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PingVis() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}