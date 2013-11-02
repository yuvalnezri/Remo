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
        private float prevPartRightHandZ;

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

            result = this.gestureParts[this.currentGesturePart].CheckGesture(data, prevPartRightHandZ);
            //if part succeeded
            if (result == GesturePartResult.Succeed)
            {
                if (this.currentGesturePart + 1 < this.gestureParts.Length)
                {
                    this.currentGesturePart++;
                    this.prevPartRightHandZ = data.Joints[JointType.HandRight].Position.Z;
                    this.frameCount = 0;
                    this.paused = true;
                }
                else
                {
                    gestureRecognizedInvoker(new GestureEventArgs(this.name, data.TrackingId));
                    this.Reset();
                    
                }
            }
            else if (result == GesturePartResult.Pausing)
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

        protected override void gestureRecognizedInvoker(GestureEventArgs e)
        {
            base.gestureRecognizedInvoker(e);
        }

        public override void Reset()
        {
            base.Reset();
            this.prevPartRightHandZ = 0;
        }

        public string getGestureData()
        {
            return String.Format("name: {0}\ncurrent gesture part:{1}\nframe count:{2}\npaused frame count:{3}\nis paused: {4}\nprev z:{5}"
                , this.name, this.currentGesturePart, this.frameCount, this.pausedFrameCount, this.paused, prevPartRightHandZ);
            
        }

    }


}
