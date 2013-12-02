using Fizbin.Kinect.Gestures;
using Fizbin.Kinect.Gestures.Segments;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Interactions;

namespace Remo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensorChooser sensorChooser;

        private InteractionManager interactionManager;

        private GestureManager gestureManager;

        private System.Timers.Timer _clearTimer;


        public MainWindow()
        {
            InitializeComponent();

            // add timer for clearing last detected gesture
            _clearTimer = new System.Timers.Timer(1000);
            _clearTimer.Elapsed += new ElapsedEventHandler(clearTimer_Elapsed);

            Loaded += OnLoaded;

            this.quitButton.Click += OnQuitButtonClick;

      
        }

        #region EventHandlers

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">kinect event arguments.</param>
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            bool error = false;
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();



                    try
                    {

                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                        args.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    error = true;
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (!error)
            {
                kinectRegion.KinectSensor = args.NewSensor;

                interactionManager = new InteractionManager(args.NewSensor);
                interactionManager.start();

                gestureManager = new GestureManager(args.NewSensor,interactionManager);
                gestureManager.gestureRecognized += OnGestureRecognized;

            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">event arguments.</param>
        private void OnQuitButtonClick(object sender, RoutedEventArgs e)
        {
            if (kinectRegion.KinectSensor != null)
                kinectRegion.KinectSensor.Stop();

            System.Windows.Application.Current.Shutdown();
            
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();
        }

        /// <summary>
        /// Clear text after some time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke( () => detectedGestureLabel.Content = "");
            }
            else
            {
                detectedGestureLabel.Content = "";
            }
            _clearTimer.Stop();
        }

        public void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            if (interactionManager.isPaused && e.GestureName == "WaveRight")
            {
                NotificationWindow wind = new NotificationWindow("Hello");
                wind.Show();
                interactionManager.Start();
               
            }
            if (!interactionManager.isPaused && e.GestureName == "JoinedHands")
            {
                NotificationWindow wind = new NotificationWindow("Bye bye..");
                wind.Show();
                interactionManager.Pause();
                
            }
            
            
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => detectedGestureLabel.Content = e.GestureName);
            }
            else
            {
                detectedGestureLabel.Content = e.GestureName;
            }

            _clearTimer.Start();
        }

        #endregion
    }
}
