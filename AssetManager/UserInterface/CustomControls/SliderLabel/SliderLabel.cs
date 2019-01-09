using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    public enum SlideDirection
    {
        DefaultSlide,
        Up,
        Down,
        Left,
        Right
    }

    public enum SlideState
    {
        SlideIn,
        SlideOut,
        Paused,
        Done,
        Hold,
        Queued
    }

    public partial class SliderLabel
    {
        #region Fields

        private const int defaultDisplayTime = 4;
        private const int animationTimerInterval = 15;
        private const int leftPadding = 10;
        private const int animationPadding = 60;
        private const float slideInDuration = 1000f;
        private const float slideOutDuration = 300f;

        private const SlideDirection defaultSlideInDirection = SlideDirection.Right;
        private const SlideDirection defaultSlideOutDirection = SlideDirection.Down;

        private System.Diagnostics.Stopwatch slideStopWatch = new System.Diagnostics.Stopwatch();

        private Queue<MessageParameters> messageQueue = new Queue<MessageParameters>();
        private MessageParameters currentMessage = null;
        private CancellationTokenSource pauseCancel;

        private ManualResetEvent flashReset = new ManualResetEvent(false);
        private bool flashOnNew = false;
        private ToolStripControlHost stripSlider = null;
        private Color defaultBackColor;

        private System.Timers.Timer slideTimer;
        private RectangleF lastPositionRect;

        #endregion Fields

        #region Constructors

        public SliderLabel()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            slideTimer = new System.Timers.Timer();
            slideTimer.Interval = animationTimerInterval;
            slideTimer.Stop();
            slideTimer.Elapsed += new ElapsedEventHandler(Tick);

            this.Disposed += SliderLabel_Disposed;

            pauseCancel = new CancellationTokenSource();
        }

        #endregion Constructors

        #region Properties

        public bool FlashStripOnNewMessage
        {
            get
            {
                return flashOnNew;
            }

            set
            {
                flashOnNew = value;
            }
        }

        public string SlideText
        {
            get
            {
                return currentMessage.Text;
            }
            set
            {
                AddMessageToQueue(value, this.ForeColor, defaultSlideInDirection, defaultSlideOutDirection, defaultDisplayTime);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds new message to queue.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        /// <param name="color">Forecolor of the text.</param>
        /// <param name="slideInDirection">Slide in direction.</param>
        /// <param name="slideOutDirection">Slide out direction.</param>
        /// <param name="displayTime">How long (in seconds) the text will be displayed before sliding out. 0 = forever.</param>
        public void QueueMessage(string text, Color color, SlideDirection slideInDirection = defaultSlideInDirection, SlideDirection slideOutDirection = defaultSlideOutDirection, int displayTime = defaultDisplayTime)
        {
            AddMessageToQueue(text, color, slideInDirection, slideOutDirection, displayTime);
        }

        /// <summary>
        /// Adds new message to queue.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        /// <param name="slideInDirection">Slide in direction.</param>
        /// <param name="slideOutDirection">Slide out direction.</param>
        /// <param name="displayTime">How long (in seconds) the text will be displayed before sliding out. 0 = forever.</param>
        public void QueueMessage(string text, SlideDirection slideInDirection = defaultSlideInDirection, SlideDirection slideOutDirection = defaultSlideOutDirection, int displayTime = defaultDisplayTime)
        {
            QueueMessage(text, this.ForeColor, slideInDirection, slideOutDirection, displayTime);
        }

        /// <summary>
        /// Adds new message to queue.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        /// <param name="color">Forecolor of the text.</param>
        /// <param name="displayTime">How long (in seconds) the text will be displayed before sliding out. 0 = forever.</param>
        public void QueueMessage(string text, Color color, int displayTime)
        {
            QueueMessage(text, color, defaultSlideInDirection, defaultSlideOutDirection, displayTime);
        }

        /// <summary>
        /// Adds new message to queue.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        /// <param name="displayTime">How long (in seconds) the text will be displayed before sliding out. 0 = forever.</param>
        public void QueueMessage(string text, int displayTime)
        {
            QueueMessage(text, this.ForeColor, defaultSlideInDirection, defaultSlideOutDirection, displayTime);
        }

        /// <summary>
        /// Adds new message to queue.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        /// <param name="color">Forecolor of the text.</param>
        public void QueueMessage(string text, Color color)
        {
            QueueMessage(text, color, defaultSlideInDirection, defaultSlideOutDirection, defaultDisplayTime);
        }

        /// <summary>
        /// Adds new message to queue.
        /// </summary>
        /// <param name="text">Text to be displayed.</param>
        public void QueueMessage(string text)
        {
            QueueMessage(text, this.ForeColor, defaultSlideInDirection, defaultSlideOutDirection, defaultDisplayTime);
        }

        private void AddMessageToQueue(string text, Color color, SlideDirection slideInDirection, SlideDirection slideOutDirection, int displayTime)
        {
            // Clamp display time.
            if (displayTime < 0) displayTime = defaultDisplayTime;

            messageQueue.Enqueue(new MessageParameters(text, color, slideInDirection, slideOutDirection, displayTime));
            ProcessQueue();
        }

        /// <summary>
        /// Clears currently displayed and queued messages.
        /// </summary>
        public void Clear()
        {
            messageQueue.Clear();

            if (currentMessage == null)
                return;

            if (currentMessage.SlideState == SlideState.Paused)
            {
                pauseCancel.Cancel();
            }
            else if (currentMessage.SlideState == SlideState.Hold)
            {
                StartSlideOutAnimation();
            }
        }

        /// <summary>
        /// Returns a <see cref="ToolStripControlHost"/> of this control for insertion into tool strips/status strips.
        /// </summary>
        /// <param name="parentControl">Target strip for this control. For inheriting font and color.</param>
        /// <returns></returns>
        public ToolStripControlHost ToToolStripControl(Control parentControl = null)
        {
            this.AutoSize = true;
            if (parentControl != null)
            {
                this.Font = parentControl.Font;
                this.BackColor = parentControl.BackColor;
                this.defaultBackColor = this.BackColor;
            }
            stripSlider = new ToolStripControlHost(this);
            stripSlider.AutoSize = false;
            return stripSlider;
        }

        /// <summary>
        /// Flashes the parent toolstrip with lighter alpha blended variation of the message text color.
        /// </summary>
        private async void FlashStrip()
        {
            if (flashOnNew & stripSlider != null)
            {
                // Trigger the flash reset event to cancel any current flash operation.
                flashReset.Set();

                await Task.Run(() =>
                 {
                     int flashDelay = 200; // How long to display each color.
                     int flashes = 3; // Number of flashes.
                     bool flashToggle = false;
                     var currentColor = currentMessage.TextColor;
                     var blendColor = Color.White;
                     var flashColor = Color.FromArgb(((currentColor.A + blendColor.A) / 2),
                                           ((currentColor.R + blendColor.R) / 2),
                                           ((currentColor.G + blendColor.G) / 2),
                                           ((currentColor.B + blendColor.B) / 2));

                     // Reset the flash reset event.
                     flashReset.Reset();

                     for (int i = 0; i != flashes * 2; i++)
                     {
                         // Flip flop the flash colors.
                         if (flashToggle)
                         {
                             SetParentStripColor(flashColor);
                         }
                         else
                         {
                             SetParentStripColor(defaultBackColor);
                         }

                         flashToggle = !flashToggle;

                         // Block until the flash reset event is triggered or times out.
                         // A timeout is a normal loop.
                         // A triggered event means this operation is being canceled
                         // by another call.
                         if (flashReset.WaitOne(flashDelay) || flashReset.SafeWaitHandle.IsClosed)
                         {
                             // Flash operation canceled. Set default color and leave the method.
                             SetParentStripColor(defaultBackColor);
                             return;
                         }
                     }
                     // Flash loop complete. Make sure default color is set.
                     SetParentStripColor(defaultBackColor);
                 });
            }
        }

        private void SetParentStripColor(Color color)
        {
            var parentStrip = stripSlider.GetCurrentParent();

            if (parentStrip == null) return;

            var del = new Action(() =>
            {
                parentStrip.BackColor = color;
                parentStrip.Invalidate();
            });

            parentStrip.BeginInvoke(del);
        }

        /// <summary>
        /// Displays the specified text and starts the animation.
        /// </summary>
        /// <param name="message"></param>
        private void StartNewSlide(MessageParameters message)
        {
            if (!string.IsNullOrEmpty(message.Text))
            {
                currentMessage = message;
                currentMessage.TextSize = GetTextSize(message.Text);
                SetControlSize(currentMessage);
                StartSlideInAnimation();
                TriggerPaint();
                FlashStrip();
            }
        }

        /// <summary>
        /// Measures the graphical size of the specified text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private SizeF GetTextSize(string text)
        {
            try
            {
                using (var gfx = this.CreateGraphics())
                {
                    return gfx.MeasureString(text, this.Font);
                }
            }
            catch (ObjectDisposedException)
            {
                return new SizeF();
                // We've been disposed. Do nothing.
            }
        }

        /// <summary>
        /// Async pause task.
        /// </summary>
        /// <param name="pauseTime"></param>
        /// <returns></returns>
        private Task Pause(int pauseTime)
        {
            if (pauseCancel != null) pauseCancel.Dispose();
            pauseCancel = new CancellationTokenSource();
            return Task.Delay(pauseTime * 1000, pauseCancel.Token);
        }

        /// <summary>
        /// Handles the message queue. Messages are queued until their animation is complete. Messages with a display time of 0 are moved to the slide out animation status, then replaced with the next message when complete.
        /// </summary>
        private void ProcessQueue()
        {
            if (messageQueue.Count > 0)
            {
                // If state is done, then we can display the next message.
                if (currentMessage == null || currentMessage.SlideState == SlideState.Done)
                {
                    StartNewSlide(messageQueue.Dequeue());
                }
                // If the state is hold, then a permanent message is currently displayed.
                // Trigger a slide out animation, which will change the state to done once complete.
                else if (currentMessage.SlideState == SlideState.Hold)
                {
                    StartSlideOutAnimation();
                }
            }
        }

        /// <summary>
        /// If autosize set to true, sets the control size to fit the text.
        /// </summary>
        private void SetControlSize(MessageParameters message)
        {
            if (this.InvokeRequired)
            {
                var del = new Action(() => SetControlSize(message));
                this.BeginInvoke(del);
            }
            else
            {
                if (this.AutoSize)
                {
                    // Add some padding to the control size to fit animations.
                    this.Size = new Size(message.TextSize.ToSize().Width + animationPadding, message.TextSize.ToSize().Height);
                }
            }
        }

        /// <summary>
        /// Sets states, current positions and ending positions for a slide-in animation.
        /// </summary>
        private void StartSlideInAnimation()
        {
            currentMessage.Direction = currentMessage.SlideInDirection;
            currentMessage.SlideState = SlideState.SlideIn;
            currentMessage.Position = new PointF();
            currentMessage.AnimationComplete = false;

            switch (currentMessage.SlideInDirection)
            {
                case SlideDirection.DefaultSlide:
                case SlideDirection.Up:
                    currentMessage.StartPosition.X = leftPadding;
                    currentMessage.StartPosition.Y = currentMessage.TextSize.Height;
                    currentMessage.EndPosition.X = currentMessage.StartPosition.X;
                    currentMessage.EndPosition.Y = 0;
                    currentMessage.Position = currentMessage.StartPosition;
                    break;

                case SlideDirection.Down:
                    currentMessage.StartPosition.X = leftPadding;
                    currentMessage.StartPosition.Y = -currentMessage.TextSize.Height;
                    currentMessage.EndPosition.X = currentMessage.StartPosition.X;
                    currentMessage.EndPosition.Y = 0;
                    currentMessage.Position = currentMessage.StartPosition;
                    break;

                case SlideDirection.Left:
                    currentMessage.StartPosition.X = currentMessage.TextSize.Width;
                    currentMessage.StartPosition.Y = 0;
                    currentMessage.EndPosition.X = leftPadding;
                    currentMessage.EndPosition.Y = currentMessage.StartPosition.Y;
                    currentMessage.Position = currentMessage.StartPosition;
                    break;

                case SlideDirection.Right:
                    currentMessage.StartPosition.X = -currentMessage.TextSize.Width;
                    currentMessage.StartPosition.Y = 0;
                    currentMessage.EndPosition.X = leftPadding;
                    currentMessage.EndPosition.Y = currentMessage.StartPosition.Y;
                    currentMessage.Position = currentMessage.StartPosition;
                    break;
            }

            // Start stopwatch and animation timer.
            slideStopWatch.Reset();
            slideStopWatch.Start();
            slideTimer.Start();
        }

        /// <summary>
        /// Sets states, current positions and ending positions for a slide-out animation.
        /// </summary>
        private void StartSlideOutAnimation()
        {
            currentMessage.Direction = currentMessage.SlideOutDirection;
            currentMessage.SlideState = SlideState.SlideOut;
            currentMessage.AnimationComplete = false;
            currentMessage.StartPosition = currentMessage.Position;

            switch (currentMessage.SlideOutDirection)
            {
                case SlideDirection.DefaultSlide:
                case SlideDirection.Up:
                    currentMessage.EndPosition.X = leftPadding;
                    currentMessage.EndPosition.Y = -this.Size.Height;
                    break;

                case SlideDirection.Down:
                    currentMessage.EndPosition.X = leftPadding;
                    currentMessage.EndPosition.Y = this.Size.Height;
                    break;

                case SlideDirection.Left:
                    currentMessage.EndPosition.X = -this.Size.Width;
                    currentMessage.EndPosition.Y = 0;
                    break;

                case SlideDirection.Right:
                    currentMessage.EndPosition.X = this.Size.Width;
                    currentMessage.EndPosition.Y = 0;
                    break;
            }

            // Start stopwatch and animation timer.
            slideStopWatch.Reset();
            slideStopWatch.Start();
            slideTimer.Start();
        }

        private void SliderTextBoxPaint(object sender, PaintEventArgs e)
        {
            DrawText(e.Graphics);
        }

        /// <summary>
        /// Primary text renderer.
        /// </summary>
        /// <param name="canvas"></param>
        private void DrawText(Graphics canvas)
        {
            if (currentMessage == null) return;

            canvas.Clear(this.Parent.BackColor);

            using (var textBrush = new SolidBrush(currentMessage.TextColor))
            {
                canvas.DrawString(currentMessage.Text, this.Font, textBrush, currentMessage.Position.X, currentMessage.Position.Y);
            }

            lastPositionRect = new RectangleF(currentMessage.Position.X, currentMessage.Position.Y, currentMessage.TextSize.Width, currentMessage.TextSize.Height);
        }

        /// <summary>
        /// Timer tick event for animation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tick(object sender, EventArgs e)
        {
            try
            {
                if (!this.Disposing && !this.IsDisposed)
                {
                    UpdateTextPosition();
                }
                else
                {
                    slideTimer.Stop();
                    slideTimer.Dispose();
                }
            }
            catch (ObjectDisposedException)
            {
                //We've been disposed. Do nothing.
            }
        }

        /// <summary>
        /// Primary animation routine. Messages are animated per their current state and specified directions.
        /// </summary>
        private void UpdateTextPosition()
        {
            if (currentMessage.SlideState == SlideState.SlideIn || currentMessage.SlideState == SlideState.SlideOut)
            {
                float duration; // How long the animation will take.
                double factor; // Applied to positions to calculate the next position.
                float slidePosition; // How far along the current animation is. (elapsed time / duration)
                long elapsed = slideStopWatch.ElapsedMilliseconds; // Current elapsed time.

                // Calculate a factor using a time based easing function.
                // Different easing functions for slide in/slide out.
                if (currentMessage.SlideState == SlideState.SlideIn)
                {
                    duration = slideInDuration;
                    slidePosition = elapsed / duration;
                    factor = EaseElasticOut(slidePosition);
                }
                else
                {
                    duration = slideOutDuration;
                    slidePosition = elapsed / duration;
                    factor = EaseBackIn(slidePosition);
                }

                // Apply the ease factor to the X,Y positions.
                if (elapsed < duration)
                {
                    currentMessage.Position.X = (float)(currentMessage.StartPosition.X + (currentMessage.EndPosition.X - currentMessage.StartPosition.X) * factor);
                    currentMessage.Position.Y = (float)(currentMessage.StartPosition.Y + (currentMessage.EndPosition.Y - currentMessage.StartPosition.Y) * factor);
                }
                else // Animation complete. Set final position and set the complete switch.
                {
                    currentMessage.Position = currentMessage.EndPosition;
                    currentMessage.AnimationComplete = true;
                }

                //Trigger redraw.
                TriggerPaint();

                // Stop and reset timers and process the next state.
                if (currentMessage.AnimationComplete)
                {
                    slideStopWatch.Stop();
                    slideStopWatch.Reset();
                    slideTimer.Stop();
                    ProcessNextState();
                }
            }
        }

        private double EaseElasticOut(float k)
        {
            if (k == 0) return 0;
            if (k == 1) return 1;
            return Math.Pow(2f, -10f * k) * Math.Sin((k - 0.1f) * (2f * Math.PI) / 0.4f) + 1f;
        }

        private double EaseQuinticOut(float k)
        {
            return 1f + ((k -= 1f) * Math.Pow(k, 4));
        }

        private double EaseBackIn(float k)
        {
            float s = 1.70158f;
            return k * k * ((s + 1f) * k - s);
        }

        private void TriggerPaint()
        {
            if (this.InvokeRequired)
            {
                var del = new Action(TriggerPaint);
                this.BeginInvoke(del);
            }
            else
            {
                lastPositionRect.Inflate(20, 5);
                using (var updateRegion = new Region(lastPositionRect))
                {
                    this.Invalidate(updateRegion);
                    this.Update();
                }
            }
        }

        private async void ProcessNextState()
        {
            // If current state is slide-in and display time is not forever.
            if (currentMessage.SlideState == SlideState.SlideIn & currentMessage.DisplayTime > 0)
            {
                // Change state to paused, and pause for the specified display time.
                currentMessage.SlideState = SlideState.Paused;

                try
                {
                    // Asynchronous wait task. (Keeps UI alive)
                    await Pause(currentMessage.DisplayTime);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException is TaskCanceledException)
                    {
                        // Task canceled exception is expected.
                    }
                }

                // Once the wait is complete, start the slide out animation.
                StartSlideOutAnimation();
            }
            else
            {
                // If the display time is forever
                if (currentMessage.DisplayTime == 0)
                {
                    // If the forever displayed (sticky) message state is slide-out, then the sticky message is being replaced with a new message, so change the state to done.
                    if (currentMessage.SlideState == SlideState.SlideOut)
                    {
                        currentMessage.SlideState = SlideState.Done;

                        // Check the queue for new messages.
                        ProcessQueue();
                    }
                    else
                    {
                        // Otherwise, change the sticky message state to hold to keep it visible.
                        currentMessage.SlideState = SlideState.Hold;
                        
                        // Immediately process the queue whenever a sticky message is NOT the last one in the queue.
                        // This ensures that sticky messages will not stay if they were queued before a temp message prior to being displayed.
                        if (messageQueue.Count > 0)
                        {
                            ProcessQueue();
                        }
                    }
                }
                else
                {
                    // If the message has a display time, set state to done.
                    currentMessage.SlideState = SlideState.Done;

                    // Check the queue for new messages.
                    ProcessQueue();
                }
            }
        }

        private void SliderLabel_Disposed(object sender, EventArgs e)
        {
            messageQueue.Clear();
            slideTimer.Stop();
            pauseCancel.Dispose();

            if (!flashReset.SafeWaitHandle.IsClosed)
            {
                flashReset.Dispose();
            }
        }

        #endregion Methods
    }
}