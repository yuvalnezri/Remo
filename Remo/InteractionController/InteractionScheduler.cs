using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Interactions
{
    public class InteractionScheduler
    {
        public bool canDoLeftRight;
        public bool canDoUpDown;

        private const int waitInterval = 2000;

        Timer leftRightTimer;
        Timer upDownTimer;

        public InteractionScheduler()
        {
            leftRightTimer = new Timer(waitInterval);
            upDownTimer = new Timer(waitInterval);

            leftRightTimer.Elapsed += leftRightTimer_Elapsed;
            upDownTimer.Elapsed += upDownTimer_Elapsed;

            canDoLeftRight = true;
            canDoUpDown = true;
        }

        public void leftRightOccured()
        {
            leftRightTimer.Start();
            canDoUpDown = false;
        }

        public void upDownOccured()
        {
            upDownTimer.Start();
            canDoLeftRight = false;
        }

        void upDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            canDoLeftRight = true;
            upDownTimer.Stop();
        }

        void leftRightTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            canDoUpDown = true;
            leftRightTimer.Stop();
        }



    }
}
