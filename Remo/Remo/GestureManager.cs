using Fizbin.Kinect.Gestures;
using Fizbin.Kinect.Gestures.Segments;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remo
{
    public class GestureManager
    {
        // skeleton gesture recognizer
        private GestureController gestureController;

        private InteractionManager interactionManager;

        private KinectSensor _sensor;

        private Skeleton[] skeletons = new Skeleton[0];

        public delegate void GestureRecognizedEventHandler(object sender, GestureEventArgs args);

        public event GestureRecognizedEventHandler gestureRecognized;

        public bool isPaused { get; set; }

        RemoScheduler remoScheduler;


        public GestureManager(KinectSensor sensor, InteractionManager _interactionManager,RemoScheduler _remoScheduler)
        {
            _sensor = sensor;
            interactionManager = _interactionManager;

            remoScheduler = _remoScheduler;


            // initialize the gesture recognizer
            gestureController = new GestureController();

            RegisterGestures();
            isPaused = true;

            gestureController.GestureRecognized += OnGestureRecognized;
            sensor.SkeletonFrameReady += OnSkeletonFrameReady;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;

                // resize the skeletons array if needed
                if (skeletons.Length != frame.SkeletonArrayLength)
                    skeletons = new Skeleton[frame.SkeletonArrayLength];

                // get the skeleton data
                frame.CopySkeletonDataTo(skeletons);

                foreach (var skeleton in skeletons)
                {
                    // skip the skeleton if it is not being tracked
                    if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                        continue;

                    // update the gesture controller
                    gestureController.UpdateAllGestures(skeleton);

                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Gesture event arguments.</param>
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {

            if (isPaused && e.GestureName == "WaveRight")
            {
                isPaused = false;
                gestureRecognized(this, e);
                return;
            }
            if (!isPaused && e.GestureName == "JoinedHands")
            {
                isPaused = true;
                gestureRecognized(this, e);
                return;
            }

            if (isPaused)
                return;

            switch (e.GestureName)
            {
                case "Menu":
                    break;
                case "WaveRight":
                    SendKeys.SendWait("{ESC}");
                    gestureRecognized(this, e);
                    break;
                case "WaveLeft":
                    break;
                case "JoinedHands":
                    break;
                case "ZoomIn":
                    break;
                case "ZoomOut":
                    break;
                case "SwipeLeft":
                    break;
                case "SwipeRight":
                    SendKeys.SendWait("{ADD}");
                    remoScheduler.enterVolumeMode();
                    gestureRecognized(this, e);
                    break;
                case "SwipeUp":
                    if (!remoScheduler.canDoSwipeUp || interactionManager.isRightHandGripped)
                        return;
                    remoScheduler.swipeUpOccured();
                    gestureRecognized(this, e);
                    break;
                case "SwipeDown":
                    if (!remoScheduler.canDoSwipeDown || interactionManager.isRightHandGripped)
                        return;
                    remoScheduler.swipeDownOccured();
                    gestureRecognized(this, e);
                    break;
                case "Click":
                    if (interactionManager.isRightHandGripped)
                        return;
                    SendKeys.SendWait("{ENTER}");
                    gestureRecognized(this, e);
                    break;

                default:
                    break;
            }


        }


        /// <summary>
        /// Helper function to register all available 
        /// </summary>
        private void RegisterGestures()
        {
            // define the gestures for the app

            IRelativeGestureSegment[] joinedhandsSegments = new IRelativeGestureSegment[20];
            JoinedHandsSegment1 joinedhandsSegment = new JoinedHandsSegment1();
            for (int i = 0; i < 20; i++)
            {
                // gesture consists of the same thing 10 times 
                joinedhandsSegments[i] = joinedhandsSegment;
            }
            gestureController.AddRelativeGesture("JoinedHands", joinedhandsSegments);

            IRelativeGestureSegment[] menuSegments = new IRelativeGestureSegment[20];
            MenuSegment1 menuSegment = new MenuSegment1();
            for (int i = 0; i < 20; i++)
            {
                // gesture consists of the same thing 20 times 
                menuSegments[i] = menuSegment;
            }
            //gestureController.AddRelativeGesture("Menu", menuSegments);

            IRelativeGestureSegment[] waveRightSegments = new IRelativeGestureSegment[6];
            WaveRightSegment1 waveRightSegment1 = new WaveRightSegment1();
            WaveRightSegment2 waveRightSegment2 = new WaveRightSegment2();
            waveRightSegments[0] = waveRightSegment1;
            waveRightSegments[1] = waveRightSegment2;
            waveRightSegments[2] = waveRightSegment1;
            waveRightSegments[3] = waveRightSegment2;
            waveRightSegments[4] = waveRightSegment1;
            waveRightSegments[5] = waveRightSegment2;
            gestureController.AddRelativeGesture("WaveRight", waveRightSegments);

            IRelativeGestureSegment[] waveLeftSegments = new IRelativeGestureSegment[6];
            WaveLeftSegment1 waveLeftSegment1 = new WaveLeftSegment1();
            WaveLeftSegment2 waveLeftSegment2 = new WaveLeftSegment2();
            waveLeftSegments[0] = waveLeftSegment1;
            waveLeftSegments[1] = waveLeftSegment2;
            waveLeftSegments[2] = waveLeftSegment1;
            waveLeftSegments[3] = waveLeftSegment2;
            waveLeftSegments[4] = waveLeftSegment1;
            waveLeftSegments[5] = waveLeftSegment2;
            //gestureController.AddRelativeGesture("WaveLeft", waveLeftSegments);

            IRelativeGestureSegment[] zoomInSegments = new IRelativeGestureSegment[3];
            zoomInSegments[0] = new ZoomSegment1();
            zoomInSegments[1] = new ZoomSegment2();
            zoomInSegments[2] = new ZoomSegment3();
            //gestureController.AddRelativeGesture("ZoomIn", zoomInSegments);

            IRelativeGestureSegment[] zoomOutSegments = new IRelativeGestureSegment[3];
            zoomOutSegments[0] = new ZoomSegment3();
            zoomOutSegments[1] = new ZoomSegment2();
            zoomOutSegments[2] = new ZoomSegment1();
            //gestureController.AddRelativeGesture("ZoomOut", zoomOutSegments);

            IRelativeGestureSegment[] swipeleftSegments = new IRelativeGestureSegment[3];
            swipeleftSegments[0] = new SwipeLeftSegment1();
            swipeleftSegments[1] = new SwipeLeftSegment2();
            swipeleftSegments[2] = new SwipeLeftSegment3();
            //gestureController.AddRelativeGesture("SwipeLeft", swipeleftSegments);

            IRelativeGestureSegment[] swiperightSegments = new IRelativeGestureSegment[3];
            swiperightSegments[0] = new SwipeRightSegment1();
            swiperightSegments[1] = new SwipeRightSegment2();
            swiperightSegments[2] = new SwipeRightSegment3();
            gestureController.AddRelativeGesture("SwipeRight", swiperightSegments);

            IRelativeGestureSegment[] swipeUpSegments = new IRelativeGestureSegment[3];
            swipeUpSegments[0] = new SwipeUpSegment1();
            swipeUpSegments[1] = new SwipeUpSegment2();
            swipeUpSegments[2] = new SwipeUpSegment3();
            gestureController.AddRelativeGesture("SwipeUp", swipeUpSegments);

            IRelativeGestureSegment[] swipeDownSegments = new IRelativeGestureSegment[3];
            swipeDownSegments[0] = new SwipeDownSegment1();
            swipeDownSegments[1] = new SwipeDownSegment2();
            swipeDownSegments[2] = new SwipeDownSegment3();
            gestureController.AddRelativeGesture("SwipeDown", swipeDownSegments);


            ISemiRelativeGestureSegment[] clickSegments = new ISemiRelativeGestureSegment[3];
            clickSegments[0] = new clickSegment1();
            clickSegments[1] = new clickSegment2();
            clickSegments[2] = new clickSegment3();
            gestureController.AddSemiRelativeGesture("Click", clickSegments);

            gestureController.setGesturePauseCount(20, "Click");

        }


    }



}
