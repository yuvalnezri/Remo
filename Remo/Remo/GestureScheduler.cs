using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Remo
{
    class GestureScheduler
    {
        public bool canDoSwipeUp;
        public bool canDoSwipeDown;

        private const int waitInterval = 2000;

        Timer swipeUpTimer;
        Timer swipeDownTimer;

        public GestureScheduler()
        {
            swipeUpTimer = new Timer(waitInterval);
            swipeDownTimer = new Timer(waitInterval);


            swipeUpTimer.Elapsed += swipeUpTimer_Elapsed;
            swipeDownTimer.Elapsed += swipeDownTimer_Elapsed;


            canDoSwipeDown = true;
            canDoSwipeUp = true;
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

    }
}
