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
            interactionController.handGrip += OnHandGrip;
            interactionController.handMoved += OnHandMoved;
            interactionController.handGripRelease += OnHandGripRelease;

            //TODO:see if theres a better way
            interactionController.addInertiaMoveHandler(OnHandMoved);
        }

        private void OnHandMoved(object sender, HandMovedEventArgs args)
        {
            //check if its one hand movement or inertia scroll
            if ((args.handType == InteractionHandType.Left  && (leftHandGripped && !rightHandGripped)) ||
                (args.handType == InteractionHandType.Right && (!leftHandGripped && rightHandGripped)) ||
                (args.movementType == MovementType.inertia)) 
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
                //Console.WriteLine("left hand moved");
            }
        }


        private void OnHandGrip(object sender, HandGripEventArgs args)
        {
            if (args.handType == InteractionHandType.Left)
                leftHandGripped = true;
            if (args.handType == InteractionHandType.Right)
                rightHandGripped = true;
            //Debug
            //Console.WriteLine("left hand grip");
        }

        private void OnHandGripRelease(object sender, HandGripReleaseEventArgs args)
        {
            if (args.handType == InteractionHandType.Left)
                leftHandGripped = false;
            if (args.handType == InteractionHandType.Right)
                rightHandGripped = false;

            //Debug
            //Console.WriteLine("left hand release");
        }
    }
}
