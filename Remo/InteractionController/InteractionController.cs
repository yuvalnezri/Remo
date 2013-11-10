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
        Point prevMoveLeftLocation;
        Point prevMoveRightLocation;

        Point prevFrameLeftLocation;
        Point prevFrameRightLocation;

        DateTime lastHLeftMoveTime;
        DateTime lastHRightMoveTime;
        DateTime lastVLeftMoveTime;
        DateTime lastVRightMoveTime;

        const double horizontalSensitivity = 0.3;
        const double verticalSensitivity = 0.25;

        InteractionStream interactionStream;

        InertiaScroller inertiaScroller;

        InertiaScroller.ScrollType scrollType;

        public event GripHandler leftHandGrip;
        public event ReleaseHandler leftHandRelease;
        public event HandMovedHandler leftHandMoved;
        public event HandMovedHandler leftHandIntretia;
        public event GripHandler rightHandGrip;
        public event ReleaseHandler rightHandRelease;
        public event HandMovedHandler rightHandMoved;
        public event HandMovedHandler rightHandInertia;

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
                        break;
                    if (hand.HandType == InteractionHandType.None)
                        break;
                    //handle left hand
                    if (hand.HandType == InteractionHandType.Left)
                    {
                        prevFrameLeftLocation.X = hand.X;
                        prevFrameLeftLocation.Y = hand.Y;

                        //fire right grip event
                        if (leftHandGrip != null && hand.HandEventType == InteractionHandEventType.Grip)
                        {
                            //stop scrolling
                            if (inertiaScroller.status == InertiaScroller.ScrollStatus.scrolling)
                                inertiaScroller.stopScrolling();
                            //fire left grip event
                            leftHandGrip(this, new HandGripEventArgs());

                        }
                        
                        if (hand.HandEventType == InteractionHandEventType.GripRelease)
                        {
                            //fire right release event
                            if(leftHandRelease!=null)
                                leftHandRelease(this, new HandReleaseEventArgs());

                            //keep horizontal inertia scrolling
                            if (hInertiaEnabled && scrollType == InertiaScroller.ScrollType.horizontal)
                            {

                                //with *1000 factor speed will be ~ 1<speed<8
                                double speed = (1000 * (hand.X - prevMoveLeftLocation.X) / (DateTime.UtcNow.Subtract(lastHLeftMoveTime).Milliseconds));
                                inertiaScroller.keepScrolling(speed, hand.HandType,scrollType);
                                //debug
                                Console.WriteLine("speed: {0} type: {1}", speed, scrollType);
                            }

                            //keep vertical inertia scrolling
                            if (vInertiaEnabled && scrollType == InertiaScroller.ScrollType.vertical)
                            {

                                //with *1000 factor speed will be ~ 1<speed<8
                                double speed = (1000 * (hand.Y - prevMoveLeftLocation.Y) / (DateTime.UtcNow.Subtract(lastVLeftMoveTime).Milliseconds));
                                inertiaScroller.keepScrolling(speed, hand.HandType, scrollType);
                                //debug
                                Console.WriteLine("speed: {0} type: {1}", speed, scrollType);
                            }

                            //reset scroll Direction
                            scrollType = InertiaScroller.ScrollType.none;
                        }

                        //handle horizontal hand movement
                        if (leftHandMoved!=null &&  Math.Abs(hand.X - prevMoveLeftLocation.X) > horizontalSensitivity && scrollType != InertiaScroller.ScrollType.vertical )
                        {
                            scrollType = InertiaScroller.ScrollType.horizontal;
                            lastHLeftMoveTime = DateTime.UtcNow;
                            HandMovedDirection dir = (hand.X - prevMoveLeftLocation.X) > 0 ? HandMovedDirection.right : HandMovedDirection.left;
                            var e = new HandMovedEventArgs(dir, HandMovedType.grip);
                            leftHandMoved(this, e);
                            prevMoveLeftLocation.X = hand.X;
                        }

                        //handle vertical hand movement
                        if (leftHandMoved != null && Math.Abs(hand.Y - prevMoveLeftLocation.Y) > verticalSensitivity && scrollType != InertiaScroller.ScrollType.horizontal)
                        {
                            scrollType = InertiaScroller.ScrollType.vertical;
                            lastVLeftMoveTime = DateTime.UtcNow;
                            HandMovedDirection dir = (hand.Y - prevMoveLeftLocation.Y) > 0 ? HandMovedDirection.down : HandMovedDirection.up;
                            var e = new HandMovedEventArgs(dir, HandMovedType.grip);
                            leftHandMoved(this, e);
                            prevMoveLeftLocation.Y = hand.Y;
                        }
                    }
                    //handle right hand
                    if (hand.HandType == InteractionHandType.Right)
                    {
                        prevFrameRightLocation.X = hand.X;
                        prevFrameRightLocation.Y = hand.Y;

                        if (rightHandGrip != null && hand.HandEventType == InteractionHandEventType.Grip)
                        {
                            //stop scrolling
                            if (inertiaScroller.status == InertiaScroller.ScrollStatus.scrolling)
                                inertiaScroller.stopScrolling();
                            //fire right grip event
                            rightHandGrip(this, new HandGripEventArgs());
                        }

                        if (rightHandRelease != null && hand.HandEventType == InteractionHandEventType.GripRelease)
                        {
                            //fire right release event
                            if (rightHandRelease != null)
                                rightHandRelease(this, new HandReleaseEventArgs());

                            //keep horizontal inertia scrolling
                            if (hInertiaEnabled && scrollType == InertiaScroller.ScrollType.horizontal )
                            {
                                //with *1000 factor speed will be ~ 1<speed<8
                                double speed = (1000 * (hand.X - prevMoveRightLocation.X) / (DateTime.UtcNow.Subtract(lastHRightMoveTime).Milliseconds));
                                inertiaScroller.keepScrolling(speed, hand.HandType,scrollType);
                                //debug
                                Console.WriteLine("speed: {0} type: {1}", speed,scrollType);
                            }

                            //keep vertical inertia scrolling
                            if (vInertiaEnabled && scrollType == InertiaScroller.ScrollType.vertical)
                            {
                                //with *1000 factor speed will be ~ 1<speed<8
                                double speed = (1000 * (hand.Y - prevMoveRightLocation.Y) / (DateTime.UtcNow.Subtract(lastVRightMoveTime).Milliseconds));
                                inertiaScroller.keepScrolling(speed, hand.HandType, scrollType);
                                //debug
                                Console.WriteLine("speed: {0} type: {1}", speed, scrollType);
                            }

                            //reset scroll Direction
                            scrollType = InertiaScroller.ScrollType.none;
                        }

                        //handle horizontal hand movement
                        if (rightHandMoved != null && Math.Abs(hand.X - prevMoveRightLocation.X) > horizontalSensitivity && scrollType != InertiaScroller.ScrollType.vertical)
                        {
                            scrollType = InertiaScroller.ScrollType.horizontal;
                            lastHRightMoveTime = DateTime.UtcNow;
                            HandMovedDirection dir = (hand.X - prevMoveRightLocation.X) > 0 ? HandMovedDirection.right : HandMovedDirection.left;
                            var e = new HandMovedEventArgs(dir, HandMovedType.grip);
                            rightHandMoved(this, e);
                            prevMoveRightLocation.X = hand.X;
                        }

                        //handle vertical hand movement
                        if (rightHandMoved != null && Math.Abs(hand.Y - prevMoveRightLocation.Y) > verticalSensitivity && scrollType != InertiaScroller.ScrollType.horizontal)
                        {
                            scrollType = InertiaScroller.ScrollType.vertical;
                            lastVRightMoveTime = DateTime.UtcNow;
                            HandMovedDirection dir = (hand.Y - prevMoveRightLocation.Y) > 0 ? HandMovedDirection.down : HandMovedDirection.up;
                            var e = new HandMovedEventArgs(dir, HandMovedType.grip);
                            rightHandMoved(this, e);
                            prevMoveRightLocation.Y = hand.Y;
                        }
                    }
                }
            }
        }

        public string getDebugData()
        {
            return string.Format("rightPrevLoc: {0}\nleftPrevLoc: {1}", prevMoveRightLocation, prevMoveLeftLocation);
        }

        public void addLeftHandInertiaHandler(HandMovedHandler handler)
        {
            inertiaScroller.leftInertia += handler;
        }

        public void addRightHandInertiaHandler(HandMovedHandler handler)
        {
            inertiaScroller.rightInertia += handler;
        }

    }
}
