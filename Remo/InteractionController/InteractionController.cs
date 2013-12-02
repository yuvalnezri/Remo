using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Kinect;
using System.Diagnostics;

namespace Interactions
{
    public class InteractionController
    {
        KinectSensor _sensor;

        //TODO: where to keep??
        const bool hInertiaEnabled = true;
        const bool vInertiaEnabled = true;

        private bool IsInInterationFrame;

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

        int framesSinceRightHandGrip;
        int framesSinceRightHandGripRelease;
        bool countingRightGripFrames;
        bool countingRightGripReleaseFrames;

        bool rightHandPressed = false;
        bool leftHandPressed = false;

        bool rightHandGripped;
        bool leftHandGripped;

        InteractionHandType currentHandType;

        Point prevMoveLocation
        {
            get
            {
                if (currentHandType == InteractionHandType.Left)
                    return prevLeftHandMoveLocation;
                else
                    return prevRightHandMoveLocation;
            }
            set
            {
                if (currentHandType == InteractionHandType.Left)
                    prevLeftHandMoveLocation = value;
                else
                    prevRightHandMoveLocation = value;
            }
        }

        DateTime lastHMoveTime
        {
            get
            {
                if (currentHandType == InteractionHandType.Left)
                    return lastHLeftMoveTime;
                else
                    return lastHRightMoveTime;
            }
            set
            {
                if (currentHandType == InteractionHandType.Left)
                    lastHLeftMoveTime = value;
                else
                    lastHRightMoveTime = value;
            }
        }

        DateTime lastVMoveTime
        {
            get
            {
                if (currentHandType == InteractionHandType.Left)
                    return lastVLeftMoveTime;
                else
                    return lastVRightMoveTime;
            }
            set
            {
                if (currentHandType == InteractionHandType.Left)
                    lastVLeftMoveTime = value;
                else
                    lastVRightMoveTime = value;
            }
        }

        const double horizontalSensitivity = 0.2;
        const double verticalSensitivity = 0.2;

        InteractionStream interactionStream;

        InertiaScroller inertiaScroller;

        InertiaScroller.ScrollType scrollType;

        public event GripHandler handGrip;
        public event GripReleaseHandler handGripRelease;
        public event HandMovedHandler handMoved;
        public event HandPressedHandler handPressed;



        public InteractionController()
        {
            _skeletons = new Skeleton[6];
            _userInfos = new UserInfo[6];
            inertiaScroller = new InertiaScroller();
            IsInInterationFrame = false;

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

            if (_userInfos == null)
                return;

            //this.BeginInteractionFrame();

            try
            {

                foreach (var userInfo in _userInfos)
                {
                    if (userInfo.SkeletonTrackingId == 0)
                        continue;

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
            finally
            {
                //this.EndInteractionFrame();
            }

        }


        private void BeginInteractionFrame()
        {
            Debug.Assert(!IsInInterationFrame);

            IsInInterationFrame = true;
        }

        private void EndInteractionFrame()
        {
            Debug.Assert(IsInInterationFrame);

            IsInInterationFrame = false;
        }

        public void analyzeHandPointer(InteractionHandPointer hand)
        {
            saveHandLocation(hand);

            switch (hand.HandEventType)
            {
                case InteractionHandEventType.Grip:
                    if (hand.HandType == InteractionHandType.Right)
                    {
                        if (countingRightGripReleaseFrames)
                        {
                            countingRightGripReleaseFrames = false;
                            break;
                        }
                        countingRightGripFrames = true;
                        framesSinceRightHandGrip = 0;
                    }
                    if (hand.HandType == InteractionHandType.Left)
                        fireGripEvent(hand);
                    break;
                case InteractionHandEventType.GripRelease:
                    if (hand.HandType == InteractionHandType.Right)
                    {
                        if (countingRightGripFrames)
                        {
                            countingRightGripFrames = false;
                            break;
                        }
                        countingRightGripReleaseFrames = true;
                        framesSinceRightHandGripRelease = 0;
                    }
                    if (hand.HandType == InteractionHandType.Left)
                        fireGripReleaseEvent(hand);
                    break;
                case InteractionHandEventType.None:
                    if (countingRightGripFrames)
                    {
                        framesSinceRightHandGrip++;
                        if (framesSinceRightHandGrip > 3)
                        {
                            countingRightGripFrames = false;
                            fireGripEvent(hand);
                        }
                    }
                    if (countingRightGripReleaseFrames)
                    {
                        framesSinceRightHandGripRelease++;
                        if (framesSinceRightHandGripRelease > 3)
                        {
                            countingRightGripReleaseFrames = false;
                            fireGripReleaseEvent(hand);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (hand.HandEventType == InteractionHandEventType.Grip)
            {
                Console.WriteLine("{0} Grip frame", hand.HandType);
                fireGripEvent(hand);
            }

            if (hand.HandEventType == InteractionHandEventType.GripRelease)
            {
                Console.WriteLine("{0} GripRelease frame", hand.HandType);
                fireGripReleaseEvent(hand);
            }
            //handle clicks
            handleHandPress(hand);
            //handles all movement events(left/right/horizontal/vertical)
            handleHandMovement(hand);

        }


        public void handleHandPress(InteractionHandPointer hand)
        {
            if (hand.HandType == InteractionHandType.Left && hand.IsPressed != leftHandPressed)
            {
                if (leftHandPressed == true && handPressed!=null)
                    handPressed(this, new HandPressedEventArgs(hand.HandType));
                leftHandPressed = hand.IsPressed;
            }
            if (hand.HandType == InteractionHandType.Right && hand.IsPressed != rightHandPressed)
            {
                if (rightHandPressed == true && handPressed != null)
                    handPressed(this, new HandPressedEventArgs(hand.HandType));
                rightHandPressed = hand.IsPressed;
            }
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

            //startInertiaScroll(hand);

            //clear scroll Type
            scrollType = InertiaScroller.ScrollType.none;
        }

        public void startInertiaScroll(InteractionHandPointer hand)
        {
            currentHandType = hand.HandType;

            //start horizontal inertia scrolling
            if (hInertiaEnabled && scrollType == InertiaScroller.ScrollType.horizontal)
            {
                //with *1000 factor speed will be ~ 1<speed<8
                double speed = (1000 * (hand.X - prevMoveLocation.X) / (DateTime.UtcNow.Subtract(lastHMoveTime).Milliseconds));
                inertiaScroller.keepScrolling(speed, hand.HandType, scrollType);
                //Debug
                Console.WriteLine("speed: {0} type: {1}", speed, scrollType);
            }

            //keep vertical inertia scrolling
            if (vInertiaEnabled && scrollType == InertiaScroller.ScrollType.vertical)
            {

                //with *1000 factor speed will be ~ 1<speed<8
                double speed = (1000 * (hand.Y - prevMoveLocation.Y) / (DateTime.UtcNow.Subtract(lastVMoveTime).Milliseconds));
                inertiaScroller.keepScrolling(speed, hand.HandType, scrollType);
                //Debug
                Console.WriteLine("speed: {0} type: {1}", speed, scrollType);
            }
        }

        public void handleHandMovement(InteractionHandPointer hand)
        {
            if (handMoved == null)
                return;

            currentHandType = hand.HandType;

            //handle horizontal hand movement
            if (Math.Abs(hand.X - prevMoveLocation.X) > horizontalSensitivity && scrollType != InertiaScroller.ScrollType.vertical)
            {
                scrollType = InertiaScroller.ScrollType.horizontal;
                lastHMoveTime = DateTime.UtcNow;
                HandMovedDirection dir = (hand.X - prevMoveLocation.X) > 0 ? HandMovedDirection.right : HandMovedDirection.left;
                var e = new HandMovedEventArgs(dir, MovementType.grip, hand.HandType, new Point(hand.X, hand.Y));
                handMoved(this, e);
                prevMoveLocation = new Point(hand.X, hand.Y);
            }

            //handle vertical hand movement
            if (Math.Abs(hand.Y - prevMoveLocation.Y) > verticalSensitivity && scrollType != InertiaScroller.ScrollType.horizontal)
            {
                scrollType = InertiaScroller.ScrollType.vertical;
                lastVMoveTime = DateTime.UtcNow;
                HandMovedDirection dir = (hand.Y - prevMoveLocation.Y) > 0 ? HandMovedDirection.down : HandMovedDirection.up;
                var e = new HandMovedEventArgs(dir, MovementType.grip, hand.HandType, new Point(hand.X, hand.Y));
                handMoved(this, e);
                prevMoveLocation = new Point(hand.X, hand.Y);
            }


        }

        public void addInertiaMoveHandler(HandMovedHandler handler)
        {
            inertiaScroller.InertiaMove += handler;
        }


    }
}
