using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Fizbin.Kinect.Gestures
{
    class RelativeGesture : Gesture
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Gesture"/> class.
        /// </summary>
        /// <param name="type">The type of gesture.</param>
        /// <param name="gestureParts">The gesture parts.</param>
        public RelativeGesture(string name, IRelativeGestureSegment[] gestureParts)
        {
            this.gestureParts = gestureParts;
            this.name = name;
        }
        
        
        /// <summary>
        /// The parts that make up this gesture
        /// </summary>
        protected IRelativeGestureSegment[] gestureParts;

                /// <summary>
        /// Updates the gesture.
        /// </summary>
        /// <param name="data">The skeleton data.</param>
        public override void UpdateGesture(Skeleton data)
        {
            GesturePartResult result;
            if (this.paused)
            {
                if (this.frameCount == this.pausedFrameCount)
                {
                    this.paused = false;
                }

                this.frameCount++;
            }

            result = this.gestureParts[this.currentGesturePart].CheckGesture(data);
            if (result == GesturePartResult.Succeed)
            {
                if (this.currentGesturePart + 1 < this.gestureParts.Length)
                {
                    this.currentGesturePart++;
                    this.frameCount = 0;
                    this.pausedFrameCount = 10;
                    this.paused = true;
                }
                else
                {
                    gestureRecognizedInvoker(new GestureEventArgs(this.name, data.TrackingId));
                    this.Reset();
                }
            }
            else if (result == GesturePartResult.Fail || this.frameCount == 50)
            {
                this.currentGesturePart = 0;
                this.frameCount = 0;
                this.pausedFrameCount = 5;
                this.paused = true;
            }
            else
            {
                this.frameCount++;
                this.pausedFrameCount = 5;
                this.paused = true;
            }
        }


    }
}
