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
        private Graphics pingGraphics;
        private Bitmap pingImage;
        private Graphics controlGraphics;
        private Bitmap controlImage;
        private Ping ping = new Ping();
        private List<PingInfo> pingReplies = new List<PingInfo>();
        private string hostname;
        private bool pingRunning = false;
        private Timer pingTimer = new Timer();
        private Timer scaleTimer = new Timer();
        private Control targetControl;
        private int scaledImageWidth;
        private int scaledImageHeight;
        private int origImageWidth;
        private int origImageHeight;
        private bool mouseIsScrolling = false;
        private int topIndex = 0;
        private MouseOverInfo mouseOverBar;
        private long lastDrawTime = 0;
        private List<PingBar> scrollBarList;
        private float targetScale;
        private float currentScale;

        private const int timeOut = 1000;
        private const int maxBadPing = 300; // Ping time at which the bar color will be fully red.
        private const int goodPingInterval = 1000;
        private const int noPingInterval = 3000;
        private int currentPingInterval = goodPingInterval;

        private const int maxDrawScale = 10;
        private const float barGap = 0;
        private const int maxBars = 10;
        private const int minBarLength = 1;
        private const float barTopPadding = 0;
        private const float barBottomPadding = 4;

        private const int maxStoredResults = 1000000;
        private const int maxDrawRatePerMilliseconds = 10;
        private int imageScaleMulti = 5;

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
            pingImage = new Bitmap(scaledImageWidth, scaledImageHeight, PixelFormat.Format32bppPArgb);
            pingGraphics = Graphics.FromImage(pingImage);

            controlImage = new Bitmap(origImageWidth, origImageHeight);
            controlImage.SetResolution(pingImage.HorizontalResolution, pingImage.VerticalResolution);

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
            scaleTimer.Interval = 100;
            scaleTimer.Enabled = true;
        }

        private void ScaleTimer_Tick(object sender, EventArgs e)
        {
            if (this.disposedValue) return;
            EaseScaleChange();
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
                }

                targetScale = newScale;
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
                        // Set to final scale
                        currentScale = targetScale;
                    }
                    DrawBars(targetControl, GetPingBars());
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
                if (!mouseIsScrolling && !this.disposedValue) DrawBars(targetControl, GetPingBars(), mouseOverBar);
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
            mouseOverBar = null;
            if (scrollBarList != null) scrollBarList.Clear();
            DrawBars(targetControl, GetPingBars(), mouseOverBar);
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
                        DrawBars(targetControl, GetPingBars());
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
                    DrawBars(targetControl, GetPingBars());
                }
            }
        }

        private void ControlMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsScrolling)
            {
                mouseOverBar = GetMouseOverPingBar(e.Location);
                DrawBars(targetControl, scrollBarList, mouseOverBar);
            }
        }

        private MouseOverInfo GetMouseOverPingBar(Point mPos)
        {
            var mScalePoint = new Point(mPos.X * imageScaleMulti, mPos.Y * imageScaleMulti);
            scrollBarList = GetPingBars();
            foreach (PingBar bar in scrollBarList)
            {
                if (bar.Rectangle.Contains(mScalePoint))
                {
                    bar.Brush = new SolidBrush(Color.FromArgb(128, Color.Navy)); // Highlight mouse over bar.
                    return new MouseOverInfo(mScalePoint, bar.PingInfo);
                }
            }
            return null;
        }

        private void DrawBars(Control targetControl, List<PingBar> bars, MouseOverInfo mouseOverBar = null)
        {
            if (pingReplies.Count < 1 || !CanDraw(Environment.TickCount))
            {
                return;
            }
            else
            {
                if (targetControl != null && targetControl.FindForm() != null)
                {
                    if (targetControl.FindForm().WindowState == FormWindowState.Minimized) return;
                }
            }

            pingGraphics.SmoothingMode = SmoothingMode.None;

            if (!mouseIsScrolling)
            {
                pingGraphics.Clear(targetControl.BackColor);
            }
            else
            {
                pingGraphics.Clear(Color.FromArgb(48, 53, 61));
            }
            DrawScaleLines(pingGraphics);
            DrawPingBars(pingGraphics, bars);
            DrawPingText(pingGraphics, mouseOverBar);
            DrawScrollBar(pingGraphics);
            TrimPingList();
            ResizeImage();
            SetControlImage(targetControl, controlImage);
            DisposeBarList(bars);
        }

        private void DrawScaleLines(Graphics gfx)
        {
            float scaleXPos = 0;
            float stepSize = (int)(currentScale * 15);
            int numOfLines = (int)(scaledImageWidth / stepSize);

            for (int a = 0; a < numOfLines; a++)
            {
                gfx.DrawLine(Pens.White, new PointF(scaleXPos, 0), new PointF(scaleXPos, scaledImageHeight));
                scaleXPos += stepSize;
            }
        }

        private void DrawPingBars(Graphics gfx, List<PingBar> bars)
        {
            foreach (var bar in bars)
            {
                gfx.FillRectangle(bar.Brush, bar.Rectangle);
            }
        }

        private void DrawPingText(Graphics gfx, MouseOverInfo mouseOverBar = null)
        {
            float infoFontSize = 8 * imageScaleMulti;
            float overInfoFontSize = 7 * imageScaleMulti;

            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            gfx.TextContrast = 0;

            if (mouseOverBar != null)
            {
                string overInfoText = GetReplyStatusText(mouseOverBar.PingReply);
                using (Font overFont = new Font("Tahoma", overInfoFontSize, FontStyle.Regular))
                {
                    SizeF textSize = gfx.MeasureString(overInfoText, overFont);
                    gfx.DrawString(overInfoText, overFont, new SolidBrush(Color.FromArgb(240, Color.White)), new PointF(mouseOverBar.MouseLoc.X + (textSize.Width / 2f), mouseOverBar.MouseLoc.Y - (textSize.Height / 2f)));
                }
            }
            if (!mouseIsScrolling)
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
                using (var scrollBrush = new SolidBrush(Color.White))
                {
                    float scrollLocation = 0;
                    if (topIndex > 0)
                    {
                        scrollLocation = (scaledImageHeight / (float)(pingReplies.Count / (float)topIndex));
                    }

                    gfx.FillRectangle(scrollBrush, new RectangleF(scaledImageWidth - (20 + imageScaleMulti), scrollLocation, 10 + imageScaleMulti, 5 + imageScaleMulti));
                }
            }
        }

        private void ResizeImage()
        {
            var destRect = new Rectangle(0, 0, origImageWidth, origImageHeight);
            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                controlGraphics.DrawImage(pingImage, destRect, 0, 0, pingImage.Width, pingImage.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        private void DisposeBarList(List<PingBar> bars)
        {
            foreach (var bar in bars)
            {
                // Those brushes are memory heavy.
                bar.Brush.Dispose();
            }
            bars.Clear();
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

        private List<PingBar> GetPingBars()
        {
            var newBars = new List<PingBar>();
            float currentYPos = barTopPadding;
            float barHeight = (scaledImageHeight - barBottomPadding - barTopPadding - (barGap * maxBars)) / (float)maxBars;

            SetScale();

            foreach (PingInfo result in CurrentDisplayResults())
            {
                float barLen;
                Brush barBrush;
                if (result.Status == IPStatus.Success)
                {
                    barBrush = GetBarBrush(result.RoundTripTime);
                    barLen = result.RoundTripTime * currentScale;
                    if (barLen < minBarLength) barLen = minBarLength * currentScale;
                }
                else
                {
                    barBrush = new SolidBrush(Color.FromArgb(200, Color.Red));
                    barLen = scaledImageWidth - 2;
                }
                newBars.Add(new PingBar(barLen, barBrush, new RectangleF(1, currentYPos, barLen, barHeight), currentYPos, result));
                currentYPos += barHeight + barGap;
            }

            return newBars;
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

        private Brush GetBarBrush(long roundTripTime)
        {
            Color barColor;
            Color lowColor, highColor;
            long r1, g1, b1, r2, g2, b2;

            lowColor = Color.Green; // Low ping color
            r1 = lowColor.R;
            g1 = lowColor.G;
            b1 = lowColor.B;

            highColor = Color.Red; // High ping color
            r2 = highColor.R;
            g2 = highColor.G;
            b2 = highColor.B;

            int maxIntensity = 255;
            int intensity = 0;
            if (roundTripTime > 0)
            {
                // Compute the intensity of the high ping color.
                intensity = (int)(maxIntensity / (maxBadPing / (float)roundTripTime));
            }

            // Clamp the intensity within the max.
            if (intensity > maxIntensity) intensity = maxIntensity;

            // Calculate the new RGB values from the intensity.
            int newR, newG, newB;
            newR = (int)(r1 + (r2 - r1) / (float)maxIntensity * intensity);
            newG = (int)(g1 + (g2 - g1) / (float)maxIntensity * intensity);
            newB = (int)(b1 + (b2 - b1) / (float)maxIntensity * intensity);

            barColor = Color.FromArgb(200, newR, newG, newB);
            return new SolidBrush(barColor);
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
                    // if (but.Image != null) but.Image.Dispose();
                    but.Image = image;
                    but.Invalidate();
                }
                else if (targetControl is PictureBox)
                {
                    var pic = (PictureBox)targetControl;
                    if (pic.Image != null) pic.Image.Dispose();
                    pic.Image = image;
                    pic.Refresh();
                }
            }
            else
            {
                if (targetControl.BackgroundImage != null) targetControl.BackgroundImage.Dispose();
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

        private class PingBar
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
        }

        private class MouseOverInfo
        {
            public Point MouseLoc { get; set; }
            public PingInfo PingReply { get; set; }

            public MouseOverInfo(Point mouseLoc, PingInfo pingInfo)
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

                    pingImage.Dispose();
                    pingGraphics.Dispose();

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