﻿using System;
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
        Hold
    }

    public partial class SliderLabel
    {
        #region Fields

        private const int maxMessages = 10;
        private const int defaultDisplayTime = 4;
        private const int animationTimerInterval = 15;

        private const SlideDirection defaultSlideInDirection = SlideDirection.Right;
        private const SlideDirection defaultSlideOutDirection = SlideDirection.Down;

        private float slideAcceleration = 0.5F;

        private Queue<MessageParameters> messageQueue = new Queue<MessageParameters>();
        private MessageParameters currentMessage = new MessageParameters();
        private CancellationTokenSource pauseCancel;

        private System.Timers.Timer slideTimer;
        private RectangleF lastPositionRect;

        #endregion Fields

        #region Constructors

        public SliderLabel()
        {
            InitializeComponent();

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

        #region Events

        public event EventHandler<MessageEventArgs> NewMessageDisplayed;

        #endregion Events

        #region Properties

        public int DisplayTime
        {
            get
            {
                return currentMessage.DisplayTime;
            }
            set
            {
                currentMessage.DisplayTime = value;
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

        private void OnNewMessageDisplayed(MessageParameters message)
        {
            NewMessageDisplayed?.Invoke(this, new MessageEventArgs(message));
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
        /// <param name="displayTime">How long (in seconds) the text will be displayed before sliding out. 0 = forever.</param>
        public void QueueMessage(string text, int displayTime)
        {
            QueueMessage(text, this.ForeColor, defaultSlideInDirection, defaultSlideOutDirection, displayTime);
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
            if (messageQueue.Count <= maxMessages)
            {
                // Clamp display time.
                if (displayTime < 0) displayTime = defaultDisplayTime;

                messageQueue.Enqueue(new MessageParameters(text, color, slideInDirection, slideOutDirection, displayTime));
                ProcessQueue();
            }
        }

        /// <summary>
        /// Clears currently displayed and queued messages.
        /// </summary>
        public void Clear()
        {
            messageQueue.Clear();
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
            }
            var stripSlider = new ToolStripControlHost(this);
            stripSlider.AutoSize = false;
            return stripSlider;
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
                this.Invalidate();
                this.Update();
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
                    OnNewMessageDisplayed(currentMessage);
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
            this.BackColor = this.Parent.BackColor;
            if (this.AutoSize)
            {
                this.Size = message.TextSize.ToSize();
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
            currentMessage.SlideVelocity = 0;
            currentMessage.AnimationComplete = false;
            switch (currentMessage.SlideInDirection)
            {
                case SlideDirection.DefaultSlide:
                case SlideDirection.Up:
                    currentMessage.StartPosition.Y = currentMessage.TextSize.Height;
                    currentMessage.Position = currentMessage.StartPosition;
                    currentMessage.EndPosition.Y = 0;
                    break;

                case SlideDirection.Down:
                    currentMessage.StartPosition.Y = -currentMessage.TextSize.Height;
                    currentMessage.Position = currentMessage.StartPosition;
                    currentMessage.EndPosition.Y = 0;
                    break;

                case SlideDirection.Left:
                    currentMessage.StartPosition.X = currentMessage.TextSize.Width;
                    currentMessage.Position = currentMessage.StartPosition;
                    currentMessage.EndPosition.X = 0;
                    break;

                case SlideDirection.Right:
                    currentMessage.StartPosition.X = -currentMessage.TextSize.Width;
                    currentMessage.Position = currentMessage.StartPosition;
                    currentMessage.EndPosition.X = 0;
                    break;
            }
            slideTimer.Start();
        }

        /// <summary>
        /// Sets states, current positions and ending positions for a slide-out animation.
        /// </summary>
        private void StartSlideOutAnimation()
        {
            currentMessage.Direction = currentMessage.SlideOutDirection;
            currentMessage.SlideState = SlideState.SlideOut;
            currentMessage.SlideVelocity = 0;
            currentMessage.AnimationComplete = false;
            switch (currentMessage.SlideOutDirection)
            {
                case SlideDirection.DefaultSlide:
                case SlideDirection.Up:
                    currentMessage.EndPosition.Y = -currentMessage.TextSize.Height;
                    break;

                case SlideDirection.Down:
                    currentMessage.EndPosition.Y = currentMessage.TextSize.Height;
                    break;

                case SlideDirection.Left:
                    currentMessage.EndPosition.X = -currentMessage.TextSize.Width;
                    break;

                case SlideDirection.Right:
                    currentMessage.EndPosition.X = currentMessage.TextSize.Width;
                    break;
            }
            slideTimer.Start();
        }

        private void SliderTextBoxPaint(object sender, PaintEventArgs e)
        {
            DrawText(e.Graphics);
        }

        private delegate void UpdateTextDelegate();

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
                    UpdateTextDelegate d = new UpdateTextDelegate(UpdateTextPosition);
                    this.BeginInvoke(d);
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
            // Check current direction and change X,Y positions/speeds accordingly using an accumulating acceleration.
            switch (currentMessage.Direction)
            {
                case SlideDirection.DefaultSlide:
                case SlideDirection.Up:
                    if (currentMessage.Position.Y + currentMessage.SlideVelocity > currentMessage.EndPosition.Y)
                    {
                        currentMessage.SlideVelocity -= slideAcceleration;
                        currentMessage.Position.Y += currentMessage.SlideVelocity;
                    }
                    else
                    {
                        currentMessage.Position.Y = currentMessage.EndPosition.Y;
                        currentMessage.AnimationComplete = true;
                    }
                    break;

                case SlideDirection.Down:
                    if (currentMessage.Position.Y + currentMessage.SlideVelocity < currentMessage.EndPosition.Y)
                    {
                        currentMessage.SlideVelocity += slideAcceleration;
                        currentMessage.Position.Y += currentMessage.SlideVelocity;
                    }
                    else
                    {
                        currentMessage.Position.Y = currentMessage.EndPosition.Y;
                        currentMessage.AnimationComplete = true;
                    }
                    break;

                case SlideDirection.Left:
                    if (currentMessage.Position.X + currentMessage.SlideVelocity > currentMessage.EndPosition.X)
                    {
                        currentMessage.SlideVelocity -= slideAcceleration;
                        currentMessage.Position.X += currentMessage.SlideVelocity;
                    }
                    else
                    {
                        currentMessage.Position.X = currentMessage.EndPosition.X;
                        currentMessage.AnimationComplete = true;
                    }
                    break;

                case SlideDirection.Right:
                    if (currentMessage.Position.X + currentMessage.SlideVelocity < currentMessage.EndPosition.X)
                    {
                        currentMessage.SlideVelocity += slideAcceleration;
                        currentMessage.Position.X += currentMessage.SlideVelocity;
                    }
                    else
                    {
                        currentMessage.Position.X = currentMessage.EndPosition.X;
                        currentMessage.AnimationComplete = true;
                    }
                    break;
            }

            //Trigger redraw.
            lastPositionRect.Inflate(10, 5);
            using (var updateRegion = new Region(lastPositionRect))
            {
                this.Invalidate(updateRegion);
                this.Update();
            }

            if (currentMessage.AnimationComplete) ProcessNextState();

        }

        private async void ProcessNextState()
        {
            // Current slide animation complete.

            // Reset speed.
            currentMessage.SlideVelocity = 0;
            // If current state is slide-in and display time is not forever.
            if (currentMessage.SlideState == SlideState.SlideIn & currentMessage.DisplayTime > 0)
            {
                // Stop the animation timer, change state to paused, and pause for the specified display time.
                slideTimer.Stop();
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
                    // If the forever displayed message state is slide-out, then the forever message is being replaced with a new message, so change the state to done.
                    if (currentMessage.SlideState == SlideState.SlideOut)
                    {
                        currentMessage.SlideState = SlideState.Done;
                    }
                    else
                    {
                        // Otherwise, change the forever displayed message state to hold to keep it visible.
                        currentMessage.SlideState = SlideState.Hold;
                    }
                }
                else
                {
                    // If the message has a display time, set state to done.
                    currentMessage.SlideState = SlideState.Done;
                }

                // Stop the animation timer.
                slideTimer.Stop();

                // Add pause between messages if desired.
                //Await Pause(1)
            }

            // Check the queue for new messages.
            ProcessQueue();
        }

        private void SliderLabel_Disposed(object sender, EventArgs e)
        {
            messageQueue.Clear();
            slideTimer.Stop();
            pauseCancel.Dispose();
        }

        #endregion Methods

        #region Structs



        #endregion Structs
    }
}