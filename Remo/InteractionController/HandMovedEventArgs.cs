using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactions
{
    public delegate void ReleaseHandler(object Sender, HandReleaseEventArgs args);
    public delegate void HandMovedHandler(object Sender, HandMovedEventArgs args);
    public delegate void GripHandler(object Sender, HandGripEventArgs args);
    
    public enum HandMovedDirection
    {
        left,
        right,
        up,
        down
    }

    public enum HandMovedType
    {
        grip,
        inertia
    }

    public class HandMovedEventArgs : EventArgs
    {
        public HandMovedDirection direction;
        public string debugData;
        public HandMovedType type;
        public HandMovedEventArgs(HandMovedDirection dir,HandMovedType _type)
        {
            direction = dir;
            type = _type;
        }
    }

    public class HandGripEventArgs : EventArgs
    {

    }

    public class HandReleaseEventArgs : EventArgs
    {

    }
}
