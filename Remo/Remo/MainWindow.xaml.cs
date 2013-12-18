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
using System.Drawing;

namespace Remo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensorChooser sensorChooser;

        //tray icon declerations
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Drawing.Icon redIcon;
        private System.Drawing.Icon greenIcon;

        private InteractionManager interactionManager;

        private GestureManager gestureManager;

        private KinectSensor currentSensor;

        private RemoScheduler remoScheduler;
        private System.Timers.Timer _clearTimer;


        public MainWindow()
        {
            InitializeComponent();

            InitializeTrayIcon();

            remoScheduler = new RemoScheduler();

            // add timer for clearing last detected gesture
            _clearTimer = new System.Timers.Timer(1000);
            _clearTimer.Elapsed += new ElapsedEventHandler(clearTimer_Elapsed);

            Loaded += OnLoaded;
     
        }

        public void InitializeTrayIcon()
        {
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            redIcon = new System.Drawing.Icon("R3.ico");
            greenIcon = new System.Drawing.Icon("G2.ico");

            // Initialize contextMenu1 
            this.contextMenu.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { this.menuItem1, this.menuItem2 });

            // Initialize menuItem1 
            this.menuItem1.Index = 1;
            this.menuItem1.Text = "E&xit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);

            // Initialize menuItem1 
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "Enable";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = redIcon;
            notifyIcon.Text = "Remo";
            notifyIcon.Visible = true;
            // The ContextMenu property sets the menu that will 
            // appear when the systray icon is right clicked.
            notifyIcon.ContextMenu = this.contextMenu;


        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            if (interactionManager.isPaused == false)
                disableRemo();
            else
                enableRemo();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (kinectRegion.KinectSensor != null)
                kinectRegion.KinectSensor.Stop();
            notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
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
                currentSensor = args.NewSensor;
                kinectRegion.KinectSensor = currentSensor;

                interactionManager = new InteractionManager(currentSensor, remoScheduler);
                interactionManager.start();

                gestureManager = new GestureManager(currentSensor, interactionManager, remoScheduler);
                gestureManager.gestureRecognized += OnGestureRecognized;

            }
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
                enableRemo();
            if (!interactionManager.isPaused && e.GestureName == "JoinedHands")
                disableRemo();
            
            
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

        public void enableRemo()
        {
            NotificationWindow wind = new NotificationWindow("Hello");
            wind.Show();
            this.userViewerUI.Visibility = Visibility.Visible;
            interactionManager.Start();
            this.menuItem2.Text = "Disable";
            notifyIcon.Icon = greenIcon;
        }

        public void disableRemo()
        {
            NotificationWindow wind = new NotificationWindow("Bye bye..");
            wind.Show();
            this.userViewerUI.Visibility = Visibility.Hidden;
            interactionManager.Pause();
            this.menuItem2.Text = "Enable";
            notifyIcon.Icon = redIcon;
        }

        #endregion
    }
}
