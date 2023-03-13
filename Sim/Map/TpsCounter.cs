using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Sim.Events;

namespace Sim.Map
{
    public class TpsCounter
    {

        private readonly System.Timers.Timer StableClock = new System.Timers.Timer() { AutoReset = true, };

        public uint StableTicksDone = 0;

        public uint TimerTicksDone = 0;

        public double Coefficient = 1;

        public EventHandler CoefficientUpdated;

        public TpsCounter(DispatcherTimer timer)
        {
            timer.Tick += Timer_Tick;
            StableClock.Interval = timer.Interval.TotalMilliseconds;
            StableClock.Elapsed += StableClock_Tick;
            StableClock.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimerTicksDone++;
        }

        private void StableClock_Tick(object sender, EventArgs e)
        {
            StableTicksDone++;
            if (StableTicksDone >= 150)
            {
                Coefficient = (double)TimerTicksDone / StableTicksDone;
                CoefficientUpdated.Invoke(this, new TpsCounterCoefficientUpdated(Coefficient));
                StableTicksDone = 0;
                TimerTicksDone = 0;
            }
        }
    }
}
