using System.ComponentModel;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    public class HotTrackSplitContainer : SplitContainer
    {
        private bool hotTracking = false;

        /// <summary>
        /// Changes to the splitter bar position will be updated and drawn immediately.
        /// </summary>
        [Browsable(true)]
        [Description("Changes to the splitter bar position will be updated and drawn immediately.")]
        public bool HotTracking
        {
            get
            {
                return hotTracking;
            }

            set
            {
                hotTracking = value;

                if (hotTracking)
                {
                    this.MouseMove -= HotTrackSplitContainer_MouseMove;
                    this.MouseDown -= HotTrackSplitContainer_MouseDown;
                    this.MouseUp -= HotTrackSplitContainer_MouseUp;

                    this.MouseMove += HotTrackSplitContainer_MouseMove;
                    this.MouseDown += HotTrackSplitContainer_MouseDown;
                    this.MouseUp += HotTrackSplitContainer_MouseUp;
                }
                else
                {
                    this.MouseMove -= HotTrackSplitContainer_MouseMove;
                    this.MouseDown -= HotTrackSplitContainer_MouseDown;
                    this.MouseUp -= HotTrackSplitContainer_MouseUp;
                }
            }
        }

        public HotTrackSplitContainer() : base()
        {
            this.DoubleBuffered = true;
        }

        //CREDIT TO: http://dotnetpulse.blogspot.com/2006/08/how-can-i-make-splitcontainer.html
        // AND
        // https://stackoverflow.com/questions/6521731/refresh-the-panels-of-a-splitcontainer-as-the-splitter-moves

        private void HotTrackSplitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            // This disables the normal move behavior
            this.IsSplitterFixed = true;
        }

        private void HotTrackSplitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            // This allows the splitter to be moved normally again
            this.IsSplitterFixed = false;
        }

        private void HotTrackSplitContainer_MouseMove(object sender, MouseEventArgs e)
        {
            // Check to make sure the splitter won't be updated by the
            // normal move behavior also
            if (this.IsSplitterFixed)
            {
                // Make sure that the button used to move the splitter
                // is the left mouse button
                if (e.Button.Equals(MouseButtons.Left))
                {
                    // Checks to see if the splitter is aligned Vertically
                    if (this.Orientation.Equals(Orientation.Vertical))
                    {
                        // Only move the splitter if the mouse is within
                        // the appropriate bounds
                        if (e.X > 0 && e.X < this.Width)
                        {
                            // Move the splitter & force a visual refresh
                            this.SplitterDistance = e.X;
                            this.Invalidate();
                        }
                    }
                    // If it isn't aligned vertically then it must be
                    // horizontal
                    else
                    {
                        // Only move the splitter if the mouse is within
                        // the appropriate bounds
                        if (e.Y > 0 && e.Y < this.Height)
                        {
                            // Move the splitter & force a visual refresh
                            this.SplitterDistance = e.Y;
                            this.Invalidate();
                        }
                    }
                }
                // If a button other than left is pressed or no button
                // at all
                else
                {
                    // This allows the splitter to be moved normally again
                    this.IsSplitterFixed = false;
                }
            }
        }
    }
}