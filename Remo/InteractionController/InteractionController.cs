using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Kinect;

namespace Interactions
{
    public class InteractionController
    {
        KinectSensor _sensor;

        //TODO: where to keep??
        const bool hInertiaEnabled = true;
        const bool vInertiaEnabled = true;

        private Skeleton[] _skeletons; //the skeletons 
        private UserInfo[] _userInfos; //the information about the interactive users

        //variables that hold last MOVE event locations 
        Point prevLeftHandMoveLocation;
        Point prevRightHandMoveLocation;

        Point prevFrameLeftLocation;
        Point prevFrameRightLocation;

        DateTime lastHLeftMoveTime;
        DateTime lastHRightMoveTime;
        DateTime lastVLeftMoveTime;
        DateTime lastVRightMoveTime;

        const double horizontalSensitivity = 0.3;
        const double verticalSensitivity = 0.15;

        InteractionStream interactionStream;

        InertiaScroller inertiaScroller;

        InertiaScroller.ScrollType scrollType;

        public event GripHandler handGrip;
        public event GripReleaseHandler handGripRelease;
        public event HandMovedHandler handMoved;


        public InteractionController()
        {
            _skeletons = new Skeleton[6];
            _userInfos = new UserInfo[6];
            inertiaScroller = new InertiaScroller();
        }

        public void setSensor(KinectSensor sensor)
        {
            _sensor = sensor;
        }

        public void start()
        {
            if (_sensor == null)
                return;

            //ad check if skeleton and depth streams enabled

            interactionStream = new InteractionStream(_sensor, new InteractionClient());

            _sensor.SkeletonFrameReady += SensorOnSkeletonFrameReady;
            _sensor.DepthFrameReady += SensorOnDepthFrameReady;
            interactionStream.InteractionFrameReady += InteractionStreamOnInteractionFrameReady;

        }

        private void SensorOnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs skeletonFrameReadyEventArgs)
        {
            using (SkeletonFrame skeletonFrame = skeletonFrameReadyEventArgs.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;

                try
                {
                    skeletonFrame.CopySkeletonDataTo(_skeletons);
                    var accelerometerReading = _sensor.AccelerometerGetCurrentReading();
                    interactionStream.ProcessSkeleton(_skeletons, accelerometerReading, skeletonFrame.Timestamp);
                }
                catch (InvalidOperationException)
                {
                    // SkeletonFrame functions may throw when the sensor gets
                    // into a bad state.  Ignore the frame in that case.
                }
            }
        }

        private void SensorOnDepthFrameReady(object sender, DepthImageFrameReadyEventArgs depthImageFrameReadyEventArgs)
        {
            using (DepthImageFrame depthFrame = depthImageFrameReadyEventArgs.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                    return;

                try
                {
                    interactionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
                }
                catch (InvalidOperationException)
                {
                    // DepthFrame functions may throw when the sensor gets
                    // into a bad state.  Ignore the frame in that case.
                }
            }
        }


        private void InteractionStreamOnInteractionFrameReady(object sender, InteractionFrameReadyEventArgs args)
        {
            using (var iaf = args.OpenInteractionFrame()) //dispose as soon as possible
            {
                if (iaf == null)
                    return;

                iaf.CopyInteractionDataTo(_userInfos);
            }


            foreach (var userInfo in _userInfos)
            {
                var hands = userInfo.HandPointers;
                foreach (var hand in hands)
                {
                    if (!hand.IsActive)
                        continue;
                    if (hand.HandType == InteractionHandType.None)
                        continue;

                    analyzeHandPointer(hand);
                }
            }

        }

        public void analyzeHandPointer(InteractionHandPointer hand)
        {
            saveHandLocation(hand);

            if (hand.HandEventType == InteractionHandEventType.Grip)
            {
                fireGripEvent(hand);
            }

            if (hand.HandEventType == InteractionHandEventType.GripRelease)
            {
                fireGripReleaseEvent(hand);
            }
            
            //handles all movement events(left/right/horizontal/vertical)
            handleHandMovement(hand);

        }



        public void saveHandLocation(InteractionHandPointer hand)
        {
            if (hand.HandType == InteractionHandType.Left)
            {
                prevFrameLeftLocation.X = hand.X;
                prevFrameLeftLocation.Y = hand.Y;
            }
            if (hand.HandType == InteractionHandType.Right)
            {
                prevFrameRightLocation.X = hand.X;
                prevFrameRightLocation.Y = hand.Y;
            }
        }

        public void fireGripEvent(InteractionHandPointer hand)
        {
            if (handGrip != null)
            {
                //stop inertia scrolling
                if (inertiaScroller.status == InertiaScroller.ScrollStatus.scrolling)
                    inertiaScroller.stopScrolling();

                //fire grip event
                handGrip(this, new HandGripEventArgs(hand.HandType));
            }
        }

        public void fireGripReleaseEvent(InteractionHandPointer hand)
        {
            if (handGripRelease != null)
            {
                handGripRelease(this, new HandGripReleaseEventArgs(hand.HandType));
            }

            startInertiaScroll(hand);

            //clear scroll Type
            scrollType = InertiaScroller.ScrollType.none;
        }

        public void startInertiaScroll(InteractionHandPointer hand)
        {
            //TODO: make sure assignment is by reference!!
            var prevMoveLocation = hand.HandType == InteractionHandType.Left ? prevLeftHandMoveLocation : prevRightHandMoveLocation; 
            
            //start horizontal inertia scrolling
            if (hInertiaEnabled && scrollType == InertiaScroller.ScrollType.horizontal)
            {
                //with *1000 factor speed will be ~ 1<speed<8
                double speed = (1000 * (hand.X - prevMoveLocation.X) / (DateTime.UtcNow.Subtract(lastHLeftMoveTime).Milliseconds));
                inertiaScroller.keepScrolling(speed, hand.HandType, scrollType);
                //Debug
                //Console.WriteLine("speed: {0} type: {1}", speed, scrollType);
            }

            //keep vertical inertia scrolling
            if (vInertiaEnabled && scrollType == InertiaScroller.ScrollType.vertical)
            {

                //with *1000 factor speed will be ~ 1<speed<8
                double speed = (1000 * (hand.Y - prevMoveLocation.Y) / (DateTime.UtcNow.Subtract(lastVLeftMoveTime).Milliseconds));
                inertiaScroller.keepScrolling(speed, hand.HandType, scrollType);
                //Debug
                //Console.WriteLine("speed: {0} type: {1}", speed, scrollType);
            }
        }

        public void handleHandMovement(InteractionHandPointer hand)
        {
            if (handMoved == null)
                return;

            Point prevMoveLocation;
            DateTime lastHMoveTime, lastVMoveTime;
            if (hand.HandType == InteractionHandType.Left)
            {
                //TODO: make sure assignment is by reference!!
                prevMoveLocation = prevLeftHandMoveLocation;
                lastHMoveTime = lastHLeftMoveTime;
                lastVMoveTime = lastVLeftMoveTime;
            }
            else
            {
                prevMoveLocation = prevRightHandMoveLocation;
                lastHMoveTime = lastHRightMoveTime;
                lastVMoveTime = lastVRightMoveTime;
            }
            
            //handle horizontal hand movement
            if (Math.Abs(hand.X - prevMoveLocation.X) > horizontalSensitivity && scrollType != InertiaScroller.ScrollType.vertical)
            {
                scrollType = InertiaScroller.ScrollType.horizontal;
                lastHMoveTime = DateTime.UtcNow;
                HandMovedDirection dir = (hand.X - prevMoveLocation.X) > 0 ? HandMovedDirection.right : HandMovedDirection.left;
                var e = new HandMovedEventArgs(dir, MovementType.grip,hand.HandType);
                handMoved(this, e);
                prevMoveLocation.X = hand.X;
            }

            //handle vertical hand movement
            if (Math.Abs(hand.Y - prevMoveLocation.Y) > verticalSensitivity && scrollType != InertiaScroller.ScrollType.horizontal)
            {
                scrollType = InertiaScroller.ScrollType.vertical;
                lastVMoveTime = DateTime.UtcNow;
                HandMovedDirection dir = (hand.Y - prevMoveLocation.Y) > 0 ? HandMovedDirection.down : HandMovedDirection.up;
                var e = new HandMovedEventArgs(dir, MovementType.grip, hand.HandType);
                handMoved(this, e);
                prevMoveLocation.Y = hand.Y;
            }
        }

        public void addInertiaMoveHandler(HandMovedHandler handler)
        {
            inertiaScroller.InertiaMove += handler;
        }


    }
}
