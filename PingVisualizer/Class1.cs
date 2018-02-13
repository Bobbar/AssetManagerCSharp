using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PingVisualizer
{
    public class PingVis
    {
        private Ping ping;
        private List<PingInfo> pingReplies;
        private string hostname;
        private bool pingRunning = false;
        private Timer pingTimer;
        private Control targetControl;
        private int scaledImageWidth;
        private int scaledImageHeight;
        private int origImageWidth;
        private int origImageHeight;
        private bool mouseIsScrolling = false;
        private int topIndex = 0;
        private MouseOverInfoStruct mouseOverBar;
        private long lastDrawTime = 0;
        private List<PingBar> scrollBarList;


        private const int timeOut = 1000;
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
                    return pingReplies.Last;
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
            this.hostname = hostname;
            InitPing();
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
            InitTimer();
            StartPing();
        }

        private void InitTimer()
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

        private void PingTimer_Tick(object sender, EventArgs e)
        {
            StartPing();
            pingTimer.Interval = currentPingInterval;
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

        // Do this in property?
        private void SetPingInterval(int interval)
        {
            if (currentPingInterval != interval)
            {
                currentPingInterval = interval;
                InitTimer();
            }
        }

        private void ControlMouseLeave(object sender, EventArgs e)
        {
            mouseIsScrolling = false;
            mouseOverBar = null;
            if (scrollBarList != null) scrollBarList.Clear();
            DrawBars(targetControl, GetPingBars, mouseOverBar);

        }

        private void ControlMouseWheel(object sender, MouseEventArgs e)
        {
            if (pingReplies.Count > maxBars)
            {
                int newIdx = 0;
                mouseIsScrolling = true;
                if (e.Delta < 0) // Scroll up.
                {
                    newIdx = pingReplies.Count - maxBars;
                    mouseIsScrolling = false;
                    DrawBars(targetControl, GetPingBars); // <--- #1
                }
                else if (e.Delta > 0) // Scroll down.
                {
                    newIdx = topIndex - 1;
                    if (newIdx < 0) // Clamp the index to never be negative; 
                    {
                        newIdx = 0;
                    }
                }
                if (topIndex != newIdx)
                {
                    topIndex = newIdx;
                    DrawBars(targetControl, GetPingBars); // <--- #2  Drawing twice?
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

        private MouseOverInfoStruct GetMouseOverPingBar(Point mPos)
        {
            var mScalePoint = new Point(mPos.X * imageScaleMulti, mPos.Y * imageScaleMulti);
            scrollBarList = GetPingBars();
            foreach (PingBar bar in scrollBarList)
            {
                if (bar.Rectangle.Contains(mScalePoint))
                {
                    bar.Brush = new SolidBrush(Color.FromArgb(128, Color.Navy)); // Highlight mouse over bar.
                    return new MouseOverInfoStruct(mScalePoint, bar.PingResult);
                }
            }
            return null;
        }

        private void DrawBars(Control targetControl, List<PingBar> bars, MouseOverInfoStruct mouseOverBar = null)
        {
            if (pingReplies.Count < 1 || !CanDraw(Environment.TickCount))
            {

                return;

            }
            else
            {
                if (targetControl != null && targetControl.FindForm != null)
                {
                    if (targetControl.FindForm().WindowState == FormWindowState.Minimized) return;
                }
            }
            //try
            //{
            using (var bm = new Bitmap(scaledImageWidth, scaledImageHeight, PixelFormat.Format32bppPArgb))
            using (var gfx = Graphics.FromImage(bm))
            {
                gfx.SmoothingMode = SmoothingMode.None;

                if (!mouseIsScrolling)
                {
                    gfx.Clear(targetControl.BackColor);
                }
                else
                {
                    gfx.Clear(Color.FromArgb(48, 53, 61));
                }
                DrawScaleLines(gfx);
                DrawPingBars(gfx, bars);
                DrawPingText(gfx, mouseOverBar);
                DrawScrollBar(gfx);
                TrimPingList();

                var resizedBitmap = ResizeImage(bm, origImageWidth, origImageHeight);
                SetControlImage(targetControl, resizedBitmap);

            }
            //}
            //catch
            //{

            //}
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

        private void DrawScrollBar(Graphics gfx)
        {
            if (mouseIsScrolling)
            {
                using (var scrollBrush = new SolidBrush(Color.White))
                {
                    int scrollLocation = (scaledImageHeight / (pingReplies.Count / topIndex));
                    gfx.FillRectangle(scrollBrush, new RectangleF(scaledImageWidth - (20 + imageScaleMulti), scrollLocation, 10 + imageScaleMulti, 5 + imageScaleMulti));
                }
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

        private void DrawPingText(Graphics gfx, MouseOverInfoStruct mouseOverBar = null)
        {
            float infoFontSize = (float)8 * imageScaleMulti;
            float overInfoFontSize = 7 * imageScaleMulti;

            if (mouseOverBar != null)
            {
                string overInfoText = GetReplyStatusText(mouseOverBar.PingReply);

                using (Font overFont = new Font("Tahoma", overInfoFontSize, FontStyle.Regular))
                {
                    SizeF textSize = gfx.MeasureString(overInfoText, overFont);
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit; // < Duplicates
                    gfx.TextContrast = 0; // Duplicate
                    gfx.DrawString(overInfoText, overFont, new SolidBrush(Color.FromArgb(240, Color.White)), new PointF(mouseOverBar.MouseLoc.X + (textSize.Width / 2), mouseOverBar.MouseLoc.Y - (textSize.Height / 2)));
                    // ^ Watch this shit for correctness.
                }
            }
            if (!mouseIsScrolling)
            {
                string infoText = GetReplyStatusText(pingReplies.Last);
                using (Font infoFont = new Font("Tahoma", infoFontSize, FontStyle.Bold))
                {
                    SizeF textSize = gfx.MeasureString(infoText, infoFont);
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit; // < Dupliucates
                    gfx.TextContrast = 0; // Duplicate
                    gfx.DrawString(infoText, infoFont, Brushes.White, new PointF((scaledImageWidth - 5) - (textSize.Width), scaledImageHeight - (textSize.Height + 5)));
                }
            }
        }
    }
}
