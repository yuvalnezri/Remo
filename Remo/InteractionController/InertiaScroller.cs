using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;


namespace Interactions
{
    public class InertiaScroller
    {
        double tau;
        const double minSpeed = 1;
        const double maxSpeed = 8;

        const int maxInterval = 1500;
        const int minInterval = 100;
        const int speedToClickConst = 3;

        int totalClicks;
        int clickNo;

        public ScrollStatus status;
        public ScrollType type;

        public bool hInertiaEnabled { get; set; }
        public bool vInertiaEnabled { get; set; }

        private System.Timers.Timer timer;

        InteractionHandType handType;
        HandMovedDirection scrollDirection;


        public event HandMovedHandler leftInertia;
        public event HandMovedHandler rightInertia;

        public InertiaScroller()
        {
            timer = new System.Timers.Timer(50);
            timer.AutoReset = false;
            timer.Elapsed += timer_Elapsed;

        }



        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            clickNo++;

            switch (handType)
            {
                case InteractionHandType.Left:
                    if (leftInertia != null)
                        leftInertia(this, new HandMovedEventArgs(scrollDirection, HandMovedType.inertia));
                    break;
                case InteractionHandType.Right:
                    if (rightInertia != null)
                        rightInertia(this, new HandMovedEventArgs(scrollDirection, HandMovedType.inertia));
                    break;
                case InteractionHandType.None:
                    break;
                default:
                    break;
            }
            int interval = timeInterval();
            //Debug
            Console.WriteLine("interval: {0}\n", interval);
            if (interval < maxInterval)
            {
                timer.Interval = interval;
                timer.Start();
            }
            else
            {
                status = ScrollStatus.stop;
            }
        }


        public void keepScrolling(double speed, InteractionHandType handType,ScrollType type)
        {
            //determine hand movement direction
            switch (type)
            {
                case ScrollType.vertical:
                    scrollDirection = speed > 0 ? HandMovedDirection.down : HandMovedDirection.up;
                    break;
                case ScrollType.horizontal:
                    scrollDirection = speed > 0 ? HandMovedDirection.right : HandMovedDirection.left;
                    break;
                case ScrollType.none:
                    return;
                default:
                    break;
            }

            //make sure speed is in bounds
            speed = Math.Abs(speed);
            if (speed < minSpeed || speed > maxSpeed)
            {
                return;
            }

            //reset params and start timer
            this.handType = handType;
            clickNo = 1;
            totalClicks = (int)(speed * speedToClickConst);
            tau = minInterval/(Math.Log((double)totalClicks/(double)(totalClicks-1)));
            timer.Interval=minInterval;
            status = ScrollStatus.scrolling;
            timer.Start();

        }

        public void stopScrolling()
        {
            status = ScrollStatus.stop;
            timer.Stop();
        }

        private int timeInterval()
        {
            if (totalClicks - clickNo <= 1)
                return maxInterval;

            int interval = (int)(tau * Math.Log((double)(totalClicks - clickNo) / (double)(totalClicks - clickNo - 1)));
            interval = Math.Min(interval, maxInterval);
            return interval;
        }

        public enum ScrollStatus
        {
            scrolling,
            stop
        }

        public enum ScrollType
        {
            vertical,
            horizontal,
            none
        }
    }

    
}
