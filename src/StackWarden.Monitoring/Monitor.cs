using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Extensions;
using StackWarden.Monitoring.Configuration;

namespace StackWarden.Monitoring
{
    public abstract class Monitor : IMonitor
    {
        private readonly Timer _timer;
        private double _interval;

        protected ILog Log { get; }

        public event Action<Result> Updated;

        public List<string> Tags { get; } = new List<string>();

        public double Interval
        {
            get { return _interval; }
            set
            {
                if (_interval.Equals(value))
                    return;
                
                _interval = value;


                if (_timer == null)
                    return;

                var isEnabled = _timer.Enabled;

                if (isEnabled)
                    Stop();

                _timer.Interval = _interval;

                if (isEnabled)
                    Start();
            }
        }

        public string Name { get; set; }
        
        protected Monitor(ILog log, string name)
        {
            Log = log.ThrowIfNull(nameof(log));
            Name = name.ThrowIfNullOrWhiteSpace(nameof(name));

            Interval = Constants.DefaultInterval;

            _timer = new Timer(Interval);
            _timer.Elapsed += (s, e) => HandleUpdate();
        }

        public void Start()
        {
            if (_timer.Enabled)
                return;

            _timer.Enabled = true;
            Task.Run(() => HandleUpdate());
        }

        public void Stop()
        {
            _timer.Enabled = false;
        }

        protected Result CreateResult()
        {
            return new Result
            {
                Source =
                {
                    Type = GetType(),
                    Name = Name
                },
                Target =
                {
                    State = SeverityState.Normal,

                },
                Tags = Tags.ToArray(),
                TimeToLive = new TimeSpan(0, 0, 0, 0, (int)Interval)
            };
        }

        protected virtual void HandleUpdate()
        {
            HandleResult(Update());
        }

        private Result Update()
        {
            var result = CreateResult();

            try
            {
                Update(result);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to update.", ex);
                result.Target.State = SeverityState.Warning;
                result.Message = $"Failed to update monitor. Reason: {ex.Message}";
            }

            return result;
        }

        protected abstract void Update(Result result);
        
        protected void HandleResult(Result result)
        {
            try
            {
                Updated?.Invoke(result);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to raise {nameof(Updated)} event.", ex);
            }
        }
    }
}