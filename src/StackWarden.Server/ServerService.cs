using System;
using System.Collections.Generic;
using System.ServiceProcess;
using log4net;
using StackWarden.Core.Extensions;
using StackWarden.Monitoring;

namespace StackWarden.Server
{
    public partial class StackWardenServerService : ServiceBase
    {
        private readonly ILog _log;
        private readonly IMonitor[] _monitors;

        public StackWardenServerService(ILog log, IMonitor[] monitors)
        {
            _log = log.ThrowIfNull(nameof(log));
            _monitors = monitors;

            InitializeComponent();
            ServiceName = "StackWarden.Server";
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
                    _log.Error($"Failed to start monitor {currentMonitor?.Name ?? "null"}.", ex);
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
                    _log.Error($"Failed to stop monitor {currentMonitor?.Name ?? "null"}.", ex);
                }
            }
        }
    }
}