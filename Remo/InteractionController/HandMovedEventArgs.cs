using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactions
{
    public delegate void GripReleaseHandler(object Sender, HandGripReleaseEventArgs args);
    public delegate void HandMovedHandler(object Sender, HandMovedEventArgs args);
    public delegate void GripHandler(object Sender, HandGripEventArgs args);
    
    public enum HandMovedDirection
    {
        left,
        right,
        up,
        down
    }

    public enum MovementType
    {
        grip,
        inertia
    }

    public class HandMovedEventArgs : EventArgs
    {
        public HandMovedDirection direction;
        public string debugData;
        public MovementType movementType;
        public InteractionHandType handType;
             
        public HandMovedEventArgs(HandMovedDirection dir,MovementType _movementType, InteractionHandType _handType)
        {
            direction = dir;
            movementType = _movementType;
            handType = _handType;
        }
    }

    public class HandGripEventArgs : EventArgs
    {
        public InteractionHandType handType;
        public HandGripEventArgs(InteractionHandType _handType)
        {
            handType = _handType;
        }

    }

    public class HandGripReleaseEventArgs : EventArgs
    {
        public InteractionHandType handType;
        public HandGripReleaseEventArgs(InteractionHandType _handType)
        {
            handType = _handType;
        }
    }
}
