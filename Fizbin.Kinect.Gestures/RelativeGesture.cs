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

            result = this.gestureParts[this.currentGesturePart].CheckGesture(data);
            if (result == GesturePartResult.Succeed)
            {
                if (this.name == "JoinedHands")
                    Console.WriteLine("part:{0}", this.currentGesturePart);
                if (this.currentGesturePart + 1 < this.gestureParts.Length)
                {
                    this.currentGesturePart++;
                    this.frameCount = 0;
                    this.paused = true;
                    
                }
                else
                {
                    gestureRecognizedInvoker(new GestureEventArgs(this.name, data.TrackingId));
                    this.Reset();
                }
            }
            else if (result == GesturePartResult.Pausing )
            {
                if (this.frameCount > this.gestureParts[this.currentGesturePart].pausedFrameCount)
                {
                    this.Reset();
                    return;
                }
                this.frameCount++;
                this.paused = true;
            }
            //if part failed
            else
            {
                this.Reset();
                return;
            }
        }

        public string getGestureData()
        {
            return String.Format("name: {0}\ncurrent gesture part:{1}\nframe count:{2}\npaused frame count:{3}\nis paused: {4}\n"
                , this.name, this.currentGesturePart, this.frameCount, this.pausedFrameCount, this.paused);

        }
    }
}
