using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizbin.Kinect.Gestures;
using Microsoft.Kinect;

namespace Remo
{
    public class clickSegment1 : IsemiRelativeGestureSegment
    {
        float _z;
        public float Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }

        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            return CheckGesture(skeleton, 0);
        }
        
        public GesturePartResult CheckGesture(Skeleton skeleton, float Z)
        {
            //right hand in front of right shoulder
            if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X)
            {
                //right hand under head
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y)
                {
                    if (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ShoulderRight].Position.Z)
                    {
                        Z = skeleton.Joints[JointType.HandRight].Position.Z;
                        return GesturePartResult.Succeed;
                    }

                    return GesturePartResult.Pausing;
                }

                return GesturePartResult.Fail;
            }

            return GesturePartResult.Fail;
        }
    }

    class clickSegment2 : IsemiRelativeGestureSegment
    {

        float _z;
        public float Z
        { 
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            } 
        }

        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            return CheckGesture(skeleton, 0);
        }

        public GesturePartResult CheckGesture(Skeleton skeleton, float prevZ)
        {
            //right hand in front of right shoulder
            if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X)
            {
                //right hand under head
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y)
                {
                    if (skeleton.Joints[JointType.HandRight].Position.Z < prevZ - 0.3)
                    {
                        Z = skeleton.Joints[JointType.HandRight].Position.Z;
                        return GesturePartResult.Succeed;
                    }

                    return GesturePartResult.Pausing;
                }

                return GesturePartResult.Fail;
            }

            return GesturePartResult.Fail;
        }
    }

    class clickSegment3 : IsemiRelativeGestureSegment
    {
        float _z;
        public float Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }

        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            return CheckGesture(skeleton, 0);
        }

        public GesturePartResult CheckGesture(Skeleton skeleton, float prevZ)
        {
            //right hand in front of right shoulder
            if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X)
            {
                //right hand under head
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y)
                {
                    if (skeleton.Joints[JointType.HandRight].Position.Z > prevZ + 0.2)
                    {
                        Z = skeleton.Joints[JointType.HandRight].Position.Z;
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
