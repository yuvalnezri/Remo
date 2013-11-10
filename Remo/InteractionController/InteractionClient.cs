using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect.Toolkit.Interaction;

namespace Interactions
{
    class InteractionClient : IInteractionClient
    {
        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            InteractionInfo ret = new InteractionInfo();

            ret.IsGripTarget = true;
            ret.IsPressTarget = true;
            ret.PressAttractionPointX = 0.5;
            ret.PressAttractionPointY = 0.5;
            ret.PressTargetControlId = 1;

            return ret;
        }
    }
}
