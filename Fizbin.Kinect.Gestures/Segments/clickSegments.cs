using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Fizbin.Kinect.Gestures
{
    public class clickSegment1 : ISemiRelativeGestureSegment
    {
        const int _pausedFrameCount = 20;
        public int pausedFrameCount
        {
            get { return _pausedFrameCount; }
        }
        public GesturePartResult CheckGesture(Skeleton skeleton, float[,] prevJointData)
        {
            //right hand in front of right shoulder
            if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X)
            {
                //right hand under head
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y)
                {
                    if (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ShoulderRight].Position.Z)
                    {
                        return GesturePartResult.Succeed;
                    }

                    return GesturePartResult.Pausing;
                }

                return GesturePartResult.Fail;
            }

            return GesturePartResult.Fail;
        }
    }

    public class clickSegment2 : ISemiRelativeGestureSegment
    {
        const int _pausedFrameCount = 20;
        public int pausedFrameCount
        {
            get { return _pausedFrameCount; }
        }
        public GesturePartResult CheckGesture(Skeleton skeleton, float[,] prevJointData)
        {
            //right hand in front of right shoulder and hand didnt move too much in X axis
            if ((skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X) &&
                (prevJointData[(int)JointType.HandRight, 0] - 0.1 < skeleton.Joints[JointType.HandRight].Position.X) && (skeleton.Joints[JointType.HandRight].Position.X < prevJointData[(int)JointType.HandRight, 0] + 0.1))
            {
                //right hand under head and hand didnt move too much in Y axis
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y &&
                (prevJointData[(int)JointType.HandRight, 1] - 0.1 < skeleton.Joints[JointType.HandRight].Position.Y) && (skeleton.Joints[JointType.HandRight].Position.Y < prevJointData[(int)JointType.HandRight, 1] + 0.1))
                {
                    if (skeleton.Joints[JointType.HandRight].Position.Z < prevJointData[(int)JointType.HandRight,2] - 0.07)
                       
                    {
                        return GesturePartResult.Succeed;
                    }

                    return GesturePartResult.Pausing;
                }

                return GesturePartResult.Fail;
            }

            return GesturePartResult.Fail;
        }
    }

    public class clickSegment3 : ISemiRelativeGestureSegment
    {
        const int _pausedFrameCount = 20;
        public int pausedFrameCount
        {
            get { return _pausedFrameCount; }
        }
        public GesturePartResult CheckGesture(Skeleton skeleton, float[,] prevJointData)
        {
            //right hand in front of right shoulder
            if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X &&
                (prevJointData[(int)JointType.HandRight, 0] - 0.1 < skeleton.Joints[JointType.HandRight].Position.X) && (skeleton.Joints[JointType.HandRight].Position.X < prevJointData[(int)JointType.HandRight, 0] + 0.1))
            {
                //right hand under head
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y &&
                (prevJointData[(int)JointType.HandRight, 1] - 0.1 < skeleton.Joints[JointType.HandRight].Position.Y) && (skeleton.Joints[JointType.HandRight].Position.Y < prevJointData[(int)JointType.HandRight, 1] + 0.1))
                {
                    if (skeleton.Joints[JointType.HandRight].Position.Z > prevJointData[(int)JointType.HandRight, 2] + 0.07)
                    {
                        return GesturePartResult.Succeed;
                    }

                    return GesturePartResult.Pausing;
                }

                return GesturePartResult.Fail;
            }

            return GesturePartResult.Fail;
        }
    }
}
