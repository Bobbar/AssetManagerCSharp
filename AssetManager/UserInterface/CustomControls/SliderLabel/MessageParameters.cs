using System.Drawing;

namespace AssetManager.UserInterface.CustomControls
{
    /// <summary>
    /// Parameters for messages to be queued.
    /// </summary>
    public class MessageParameters
    {
        #region Fields

        public int DisplayTime { get; set; }
        public string Text { get; set; }
        public SizeF TextSize { get; set; }
        public Color TextColor { get; set; }
        public SlideDirection SlideInDirection { get; set; }
        public SlideDirection SlideOutDirection { get; set; }
        public SlideDirection Direction { get; set; }
        public SlideState SlideState { get; set; }
        public float SlideVelocity { get; set; }
        /// <summary>
        /// Current position.
        /// </summary>
        public PointF Position;
        public PointF StartPosition;
        public PointF EndPosition;
        public bool AnimationComplete { get; set; }

        #endregion Fields

        #region Constructors

        public MessageParameters(string text, Color color, SlideDirection slideInDirection, SlideDirection slideOutDirection, int displayTime)
        {
            this.Text = text;
            this.DisplayTime = displayTime;
            this.SlideInDirection = slideInDirection;
            this.SlideOutDirection = slideOutDirection;

            Direction = SlideDirection.DefaultSlide;
            SlideState = SlideState.Done;
            SlideVelocity = 0;
            Position = new PointF();
            StartPosition = new PointF();
            EndPosition = new PointF();
            AnimationComplete = false;
            TextColor = color;
        }

        #endregion Constructors
    }
}
