using Microsoft.Kinect;

namespace Fizbin.Kinect.Gestures.Segments
{
    public class JoinedHandsSegment1 : IRelativeGestureSegment
    {
        const int _pausedFrameCount = 20;
        public int pausedFrameCount
        {
            get { return _pausedFrameCount; }
        }

        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // Right and Left Hand in front of Elbows
            if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ElbowRight].Position.Z)
            {
                // Hands between shoulder and hip
                //we removed skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y && skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y
                //because of seated mode

                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderCenter].Position.Y && 
                    skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderCenter].Position.Y )
                {
                    // Hands between shoulders
                    if (skeleton.Joints[JointType.HandRight].Position.X < skeleton.Joints[JointType.ShoulderRight].Position.X && skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X &&
                        skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X && skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderRight].Position.X)
                    {
                        // Hands very close
                        if (skeleton.Joints[JointType.HandRight].Position.X - skeleton.Joints[JointType.HandLeft].Position.X < 0)
                        {
                            return GesturePartResult.Succeed;
                        }

                        return GesturePartResult.Pausing;
                    }

                    return GesturePartResult.Fail;
                }

                return GesturePartResult.Fail;
            }

            return GesturePartResult.Fail;
        }

    }
}
