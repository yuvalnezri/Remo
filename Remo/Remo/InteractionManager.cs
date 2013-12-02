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

        private double verticalDistance { get { return Math.Abs(leftHandLocation.Y - rightHandLocation.Y); } }
        private double horizontalDistance { get { return Math.Abs(leftHandLocation.X - rightHandLocation.X); } }


        public InteractionManager(KinectSensor sensor)
        {
            leftHandLocation = new Point();
            rightHandLocation = new Point();

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
            this.Start();

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
            if ((args.handType == InteractionHandType.Left  && (leftHandGripped && !rightHandGripped)) ||
                (args.handType == InteractionHandType.Right && (!leftHandGripped && rightHandGripped)) ||
                (args.movementType == MovementType.inertia)) 
            {
                switch (args.direction)
                {
                    case HandMovedDirection.left:
                        Console.WriteLine("left");
                        SendKeys.SendWait("{LEFT}");
                        break;
                    case HandMovedDirection.right:
                        Console.WriteLine("right");
                        SendKeys.SendWait("{RIGHT}");
                        break;
                    case HandMovedDirection.up:
                        Console.WriteLine("up");
                        SendKeys.SendWait("{UP}");
                        break;
                    case HandMovedDirection.down:
                        Console.WriteLine("down");
                        SendKeys.SendWait("{DOWN}");
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
            //Debug
            Console.WriteLine("hand grip");
        }

        private void OnHandGripRelease(object sender, HandGripReleaseEventArgs args)
        {
            if (args.handType == InteractionHandType.Left)
                leftHandGripped = false;
            if (args.handType == InteractionHandType.Right)
                rightHandGripped = false;

            //Debug
            Console.WriteLine("hand release");
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
    }
}
