using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Remo
{
    public class RemoScheduler
    {

        private const int waitInterval = 2000;

        Timer swipeUpTimer;
        Timer swipeDownTimer;
        Timer leftRightTimer;
        Timer upDownTimer;

        Timer volumeTimer;


        public bool canDoLeftRight;
        public bool canDoUpDown;
        public bool canDoSwipeUp;
        public bool canDoSwipeDown;
        public bool volumeMode;

        public RemoScheduler()
        {
            swipeUpTimer = new Timer(waitInterval);
            swipeDownTimer = new Timer(waitInterval);
            volumeTimer = new Timer(waitInterval);

            swipeUpTimer.Elapsed += swipeUpTimer_Elapsed;
            swipeDownTimer.Elapsed += swipeDownTimer_Elapsed;
            volumeTimer.Elapsed += volumeTimer_Elapsed;

            volumeMode = false;
            canDoSwipeDown = true;
            canDoSwipeUp = true;

            leftRightTimer = new Timer(waitInterval);
            upDownTimer = new Timer(waitInterval);

            leftRightTimer.Elapsed += leftRightTimer_Elapsed;
            upDownTimer.Elapsed += upDownTimer_Elapsed;

            canDoLeftRight = true;
            canDoUpDown = true;
        }

        void volumeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            volumeMode = false;
            volumeTimer.Stop();
        }



        public void swipeUpOccured()
        {
            //Console.WriteLine("up timer started");
            swipeUpTimer.Start();
            canDoSwipeDown = false;
        }

        public void swipeDownOccured()
        {
            //Console.WriteLine("down timer started");
            swipeDownTimer.Start();
            canDoSwipeUp = false;
        }


        void swipeUpTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("up timer ended");
            canDoSwipeDown = true;
            swipeUpTimer.Stop();
        }

        void swipeDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("down timer ended");
            canDoSwipeUp = true;
            swipeDownTimer.Stop();
        }

        public void leftRightOccured()
        {
            leftRightTimer.Start();
            canDoUpDown = false;
            canDoSwipeUp = false;
            canDoSwipeDown = false;
        }

        public void upDownOccured()
        {
            upDownTimer.Start();
            canDoLeftRight = false;
            canDoSwipeUp = false;
            canDoSwipeDown = false;
        }

        void upDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            canDoLeftRight = true;
            canDoSwipeUp = true;
            canDoSwipeDown = true;
            upDownTimer.Stop();
        }

        void leftRightTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            canDoUpDown = true;
            canDoSwipeUp = true;
            canDoSwipeDown = true;
            leftRightTimer.Stop();
        }

        public void enterVolumeMode()
        {
            volumeMode = true;
            volumeTimer.Start();
        }

        public void extendVolumeMode()
        {
            volumeTimer.Stop();
        }

    }
}
