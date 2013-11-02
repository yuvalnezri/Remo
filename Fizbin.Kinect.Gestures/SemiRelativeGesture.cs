using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Fizbin.Kinect.Gestures;

namespace Fizbin.Kinect.Gestures
{
    public class SemiRelativeGesture : Gesture
    {
        private Skeleton prevSkeleton;

        protected  ISemiRelativeGestureSegment[] gestureParts;

        /// <summary>
        /// Initializes a new instance of the <see cref="Gesture"/> class.
        /// </summary>
        /// <param name="type">The type of gesture.</param>
        /// <param name="gestureParts">The gesture parts.</param>
        public SemiRelativeGesture(string name, ISemiRelativeGestureSegment[] gestureParts)
        {
            this.gestureParts = gestureParts;
            this.name = name;
        }

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

            result = this.gestureParts[this.currentGesturePart].CheckGesture(data, prevSkeleton);

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

        protected override void gestureRecognizedInvoker(GestureEventArgs e)
        {
            base.gestureRecognizedInvoker(e);
        }

    }


}
