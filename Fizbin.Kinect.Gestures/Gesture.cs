using Microsoft.Kinect;
using System;

namespace Fizbin.Kinect.Gestures
{
    public abstract class Gesture
    {
        /// <summary>
        /// The current gesture part that we are matching against
        /// </summary>
        protected int currentGesturePart = 0;

        /// <summary>
        /// the number of frames to pause for when a pause is initiated
        /// </summary>
        public int pausedFrameCount = 10;

        /// <summary>
        /// The current frame that we are on
        /// </summary>
        protected int frameCount = 0;

        /// <summary>
        /// Are we paused?
        /// </summary>
        protected bool paused = false;

        /// <summary>
        /// The name of gesture that this is
        /// </summary>
        public string name;

        /// <summary>
        /// Occurs when [gesture recognised].
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureRecognized;

        /// <summary>
        /// Updates the gesture.
        /// </summary>
        /// <param name="data">The skeleton data.</param>
        public abstract void UpdateGesture(Skeleton data);

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public virtual void Reset()
        {
            this.currentGesturePart = 0;
            this.frameCount = 0;
            this.pausedFrameCount = 5;
            this.paused = true;
        }

        protected virtual void gestureRecognizedInvoker(GestureEventArgs e)
        {
            if (this.GestureRecognized != null)
            {
                this.GestureRecognized(this, e);
            }
        }
    }
}
