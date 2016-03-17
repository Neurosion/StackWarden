using System;
using System.Net;
using log4net;
using StackWarden.Core.Extensions;
using System.Collections.Generic;

namespace StackWarden.Monitoring.ResultHandling
{
    public abstract class WebHookResultHandler : IMonitorResultHandler
    {
        private readonly string _hookAddress;

        protected ILog Log { get; }
        protected Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        protected WebHookResultHandler(ILog log, string hookAddress)
        {
            Log = log.ThrowIfNull(nameof(log));
            _hookAddress = hookAddress.ThrowIfNullOrWhiteSpace(nameof(hookAddress));
        }

        protected abstract string FormatResult(MonitorResult result);

        protected virtual bool ShouldHandle(MonitorResult result)
        {
            return true;
        }

        public void Handle(MonitorResult result)
        {
            try
            {
                var formattedResult = FormatResult(result);

                using (var webClient = new WebClient { UseDefaultCredentials = true })
                {
                    foreach (var currentPair in Headers)
                        webClient.Headers[currentPair.Key] = currentPair.Value;

                    webClient.UploadString(_hookAddress, formattedResult);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to call web hook at '{_hookAddress}'. Exception:{ex.Message}", ex);
            }
        }
    }
}