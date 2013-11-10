using Interactions;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Remo
{
    public class InteractionManager
    {
        private InteractionController interactionController;
        bool rightHandGripped = false;
        bool leftHandGripped = false;

        public InteractionManager(KinectSensor sensor)
        {
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
            //interactionController.leftHandGrip += OnLeftHandGrip;
            //interactionController.leftHandMoved += OnLeftHandMoved;
            //interactionController.leftHandRelease += OnLeftHandRelease;
            interactionController.rightHandGrip += OnRightHandGrip;
            interactionController.rightHandMoved += OnRightHandMoved;
            interactionController.rightHandRelease += OnRightHandRelease;

            //TODO:see if theres a better way
            //interactionController.addLeftHandInertiaHandler(OnLeftHandMoved);
            interactionController.addRightHandInertiaHandler(OnRightHandMoved);
        }

        private void OnLeftHandMoved(object sender, HandMovedEventArgs args)
        {
            if ((leftHandGripped && !rightHandGripped) || args.type == HandMovedType.inertia) 
            {
                switch (args.direction)
                {
                    case HandMovedDirection.left:
                        SendKeys.SendWait("{LEFT}");
                        break;
                    case HandMovedDirection.right:
                        SendKeys.SendWait("{RIGHT}");
                        break;
                    case HandMovedDirection.up:
                        SendKeys.SendWait("{UP}");
                        break;
                    case HandMovedDirection.down:
                        SendKeys.SendWait("{DOWN}");
                        break;
                    default:
                        break;
                }

                //Debug
                Console.WriteLine("left hand moved");
            }
        }

        private void OnRightHandMoved(object sender, HandMovedEventArgs args)
        {
            if (!leftHandGripped && rightHandGripped || args.type == HandMovedType.inertia)
            {
                switch (args.direction)
                {
                    case HandMovedDirection.left:
                        SendKeys.SendWait("{LEFT}");
                        break;
                    case HandMovedDirection.right:
                        SendKeys.SendWait("{RIGHT}");
                        break;
                    case HandMovedDirection.up:
                        SendKeys.SendWait("{UP}");
                        break;
                    case HandMovedDirection.down:
                        SendKeys.SendWait("{DOWN}");
                        break;
                    default:
                        break;
                }


                //Debug
                Console.WriteLine("right hand moved");
            }

        }

        private void OnLeftHandGrip(object sender, HandGripEventArgs args)
        {
            leftHandGripped = true;

            //Debug
            Console.WriteLine("left hand grip");
        }

        private void OnRightHandGrip(object sender, HandGripEventArgs args)
        {
            rightHandGripped = true;

            //Debug
            Console.WriteLine("right hand grip");
        }

        private void OnLeftHandRelease(object sender, HandReleaseEventArgs args)
        {
            leftHandGripped = false;

            //Debug
            Console.WriteLine("left hand release");
        }

        private void OnRightHandRelease(object sender, HandReleaseEventArgs args)
        {
            rightHandGripped = false;

            //Debug
            Console.WriteLine("right hand release");
        }

        private string getDebugData()
        {
            String ret = String.Format("rightGripped: {0}\n leftGripped: {1}\n",rightHandGripped,leftHandGripped);
            if (interactionController != null)
            {
                ret = ret + interactionController.getDebugData();
            }

            return ret;
        }

    }
}
