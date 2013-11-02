using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizbin.Kinect.Gestures;
using Microsoft.Kinect;

namespace Fizbin.Kinect.Gestures
{
    public interface ISemiRelativeGestureSegment
    {
        int pausedFrameCount{get;}
        GesturePartResult CheckGesture(Skeleton skeleton, float prevPartZ);

    }
}
