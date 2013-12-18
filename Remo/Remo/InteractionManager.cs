using Interactions;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Windows;

namespace Remo
{
    public class InteractionManager
    {
        public bool isPaused { get; set; }

        public bool isRightHandGripped { get { return rightHandGripped; } }
        public bool isLeftHandGripped { get { return leftHandGripped; } }

        private InteractionController interactionController;
        bool rightHandGripped = false;
        bool leftHandGripped = false;

        Point leftHandLocation;
        Point rightHandLocation;

        RemoScheduler remoScheduler;

        private double verticalDistance { get { return Math.Abs(leftHandLocation.Y - rightHandLocation.Y); } }
        private double horizontalDistance { get { return Math.Abs(leftHandLocation.X - rightHandLocation.X); } }


        public InteractionManager(KinectSensor sensor, RemoScheduler _remoScheduler)
        {
            leftHandLocation = new Point();
            rightHandLocation = new Point();
            remoScheduler = _remoScheduler;

            interactionController = new InteractionController();
            interactionController.setSensor(sensor);
        }

        public void start()
        {
            if (interactionController == null)
                return; //TODO: throw exception

            initializeManager();
            interactionController.start();
        }


        private void initializeManager()
        {
            isPaused = true;
            //only at initialization we want ispaused=true
            //TODO:see if theres a better way
            interactionController.addInertiaMoveHandler(OnHandMoved);
        }


        private void OnHandMoved(object sender, HandMovedEventArgs args)
        {
            double prevVerticalDistance = verticalDistance;
            double prevHorizontalDistance = horizontalDistance;



            if (args.handType == InteractionHandType.Left)
            {
                leftHandLocation = args.location;
            }
            if (args.handType == InteractionHandType.Right)
            {
                rightHandLocation = args.location;
            }

            if (isPaused)
                return;

            //if (leftHandGripped && rightHandGripped && prevVerticalDistance > verticalDistance && (args.direction == HandMovedDirection.up || args.direction == HandMovedDirection.down))
            //{
            //    Console.WriteLine("vol minus");
            //    SendKeys.SendWait("{SUBTRACT}");
            //}
            //if (leftHandGripped && rightHandGripped && prevVerticalDistance < verticalDistance && (args.direction == HandMovedDirection.up || args.direction == HandMovedDirection.down))
            //{
            //    Console.WriteLine("vol plus");
            //    SendKeys.SendWait("{ADD}");
            //}
            //if (leftHandGripped && rightHandGripped && prevHorizontalDistance > horizontalDistance && (args.direction == HandMovedDirection.left || args.direction == HandMovedDirection.right))
            //{
            //    Console.WriteLine("rewind");
            //    SendKeys.SendWait("R");
            //}
            //if (leftHandGripped && rightHandGripped && prevHorizontalDistance < horizontalDistance && (args.direction == HandMovedDirection.left || args.direction == HandMovedDirection.right))
            //{
            //    Console.WriteLine("forward");
            //    SendKeys.SendWait("F");
            //}
            //check if its one hand movement or inertia scroll

            //DEBUG


            if ((args.handType == InteractionHandType.Left && (leftHandGripped && !rightHandGripped)) ||
                (args.handType == InteractionHandType.Right && (!leftHandGripped && rightHandGripped)) ||
                (args.movementType == MovementType.inertia))
            {
                switch (args.direction)
                {
                    case HandMovedDirection.left:
                        if (remoScheduler.canDoLeftRight)
                        {
                            //Console.WriteLine("left");
                            remoScheduler.leftRightOccured();
                            if (remoScheduler.volumeMode)
                                volDown();
                            else
                                sendClicks(args.direction, args.speed);
                        }
                        break;
                    case HandMovedDirection.right:
                        if (remoScheduler.canDoLeftRight)
                        {
                            //Console.WriteLine("right");
                            remoScheduler.leftRightOccured();
                            if (remoScheduler.volumeMode)
                                volUp();
                            else
                                sendClicks(args.direction, args.speed);
                        }
                        break;
                    case HandMovedDirection.up:
                        if (remoScheduler.canDoUpDown)
                        {
                            //Console.WriteLine("up");
                            remoScheduler.upDownOccured();
                            sendClicks(args.direction, args.speed);
                        }
                        break;
                    case HandMovedDirection.down:
                        if (remoScheduler.canDoUpDown)
                        {
                            // Console.WriteLine("down");
                            remoScheduler.upDownOccured();
                            sendClicks(args.direction, args.speed);
                        }
                        break;
                    default:
                        break;
                }

                //Debug
                //Console.WriteLine("left hand moved");
            }
        }

        private void OnHandPressed(object sender, HandPressedEventArgs args)
        {
            SendKeys.SendWait("{ENTER}");
        }

        private void OnHandGrip(object sender, HandGripEventArgs args)
        {
            if (args.handType == InteractionHandType.Left)
                leftHandGripped = true;
            if (args.handType == InteractionHandType.Right)
                rightHandGripped = true;
            if (remoScheduler.volumeMode)
                remoScheduler.extendVolumeMode();
            //Debug
            //Console.WriteLine("hand grip");
        }

        private void OnHandGripRelease(object sender, HandGripReleaseEventArgs args)
        {
            if (args.handType == InteractionHandType.Left)
                leftHandGripped = false;
            if (args.handType == InteractionHandType.Right)
                rightHandGripped = false;
            if (remoScheduler.volumeMode)
                remoScheduler.enterVolumeMode();
            //Debug
            //Console.WriteLine("hand release");
        }

        public void Pause()
        {
            isPaused = true;

            interactionController.handGrip -= OnHandGrip;
            interactionController.handMoved -= OnHandMoved;
            interactionController.handGripRelease -= OnHandGripRelease;

        }

        public void Start()
        {
            isPaused = false;

            interactionController.handGrip += OnHandGrip;
            interactionController.handMoved += OnHandMoved;
            interactionController.handGripRelease += OnHandGripRelease;

        }

        private void volUp()
        {
            for (int i = 0; i < 10; i++)
            {
                SendKeys.SendWait("{ADD}");
            }
        }

        private void volDown()
        {
            for (int i = 0; i < 10; i++)
            {
                SendKeys.SendWait("{SUBTRACT}");
            }
        }


        private void sendClicks(HandMovedDirection direction, double speed)
        {
            const int factor = 1000;
            speed = speed * factor;
            int clicks = speed < 1 ? 1 : (int)Math.Round(speed);
            string keyToSend = "";
            switch (direction)
            {
                case HandMovedDirection.left:
                    keyToSend = "{LEFT}";
                    break;
                case HandMovedDirection.right:
                    keyToSend = "{RIGHT}";
                    break;
                case HandMovedDirection.up:
                    keyToSend = "{UP}";
                    break;
                case HandMovedDirection.down:
                    keyToSend = "{DOWN}";
                    break;
                default:
                    break;
            }

            for (int i = 0; i < clicks; i++)
            {
                if(keyToSend!="")
                    SendKeys.SendWait(keyToSend);   
            }
        }
    }
}
