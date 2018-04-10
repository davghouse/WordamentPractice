using System;
using System.Windows.Threading;

namespace WordamentPractice.Utilities
{
    public class UITimer
    {
        private readonly TimeSpan _interval;
        private readonly EventHandler _callback;
        private readonly string _format;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private int _intervalCount;

        public UITimer(TimeSpan interval, EventHandler callback, string format = null)
        {
            _interval = interval;
            _callback = callback;
            _format = format;

            _timer.Interval = interval;
            _timer.Tick += _timer_Tick;
            _timer.Tick += callback;
        }

        private void _timer_Tick(object sender, EventArgs e)
            => ++_intervalCount;

        public TimeSpan Elapsed
            => TimeSpan.FromTicks(_intervalCount * _timer.Interval.Ticks);

        public void Start()
        {
            _intervalCount = 0;
            _callback(this, EventArgs.Empty);
            _timer.Start();
        }

        public void Pause()
            => _timer.Stop();

        public void Unpause()
            => _timer.Start();

        public void Stop()
        {
            _timer.Stop();
            _intervalCount = 0;
            _callback(this, EventArgs.Empty);
        }

        public override string ToString()
        {
            TimeSpan elapsed = Elapsed;

            if (_format != null) return elapsed.ToString(_format);
            else if (elapsed.TotalHours < 1) return elapsed.ToString("m\\:ss");
            else return elapsed.ToString();
        }
    }
}
