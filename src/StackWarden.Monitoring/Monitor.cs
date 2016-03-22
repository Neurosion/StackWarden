using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Extensions;
using StackWarden.Monitoring.Configuration;
using StackWarden.Monitoring.ResultHandling;

namespace StackWarden.Monitoring
{
    public abstract class Monitor : IMonitor
    {
        private readonly Timer _timer;
        private double _interval;
        private bool _isProcessing;
        private object _isProcesingLock = new object();

        public event Action<MonitorResult> Updated;

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

        protected ILog Log { get; }

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

        protected MonitorResult CreateResult()
        {
            return new MonitorResult
            {
                SourceType = GetType(),
                SourceName = Name,
                Tags = Tags.ToArray(),
                TargetState = SeverityState.Normal,
                TimeToLive = Interval
            };
        }

        protected virtual void HandleUpdate()
        {
            if (_isProcessing)
            {
                Log.Warn("Failed to update, still processing last update.");
                return;
            }

            lock (_isProcesingLock)
            {
                _isProcessing = true;
                HandleResult(Update());
                _isProcessing = false;
            }
        }

        private MonitorResult Update()
        {
            var result = CreateResult();

            try
            {
                Update(result);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to update.", ex);
            }

            return result;
        }

        protected abstract void Update(MonitorResult result);
        
        protected void HandleResult(MonitorResult result)
        {
            try
            {
                Updated?.Invoke(result);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to raise Updated event.", ex);
            }
        }
    }
}