using System;
using System.Collections.Generic;
using System.ServiceProcess;
using log4net;
using StackWarden.Core.Extensions;
using StackWarden.Monitoring;

namespace StackWarden.Server
{
    public partial class ServerService : ServiceBase
    {
        private readonly ILog _log;
        private readonly List<IMonitor> _monitors;

        public ServerService(ILog log, IMonitor[] monitors)
        {
            _log = log.ThrowIfNull(nameof(log));
            _monitors = new List<IMonitor>(monitors ?? new IMonitor[0]);

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            foreach (var currentMonitor in _monitors)
            {
                try
                {
                    currentMonitor.Start();
                }
                catch (Exception ex)
                {
                    _log.Error($"Failed to start monitor {currentMonitor?.Name ?? null}.", ex);
                }
            }
        }

        protected override void OnStop()
        {
            foreach (var currentMonitor in _monitors)
            {
                try
                {
                    currentMonitor.Stop();
                }
                catch (Exception ex)
                {
                    _log.Error($"Failed to stop monitor {currentMonitor?.Name ?? null}.", ex);
                }
            }
        }
    }
}