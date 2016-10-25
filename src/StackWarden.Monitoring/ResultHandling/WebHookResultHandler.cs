using System;
using System.Net;
using System.Collections.Generic;
using log4net;
using StackWarden.Core.Extensions;

namespace StackWarden.Monitoring.ResultHandling
{
    public abstract class WebHookResultHandler : IResultHandler
    {
        private readonly string _hookAddress;

        protected ILog Log { get; }
        protected Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public string Name { get; set; }

        protected WebHookResultHandler(ILog log, string hookAddress)
        {
            Log = log.ThrowIfNull(nameof(log));
            _hookAddress = hookAddress.ThrowIfNullOrWhiteSpace(nameof(hookAddress));
        }

        protected abstract string FormatResult(Result result);

        protected virtual bool ShouldHandle(Result result)
        {
            return result != null;
        }

        public bool Handle(Result result)
        {
            if (!ShouldHandle(result))
                return false;

            try
            {
                var formattedResult = FormatResult(result);

                using (var webClient = new WebClient { UseDefaultCredentials = true })
                {
                    foreach (var currentPair in Headers)
                        webClient.Headers[currentPair.Key] = currentPair.Value;

                    webClient.UploadString(_hookAddress, formattedResult);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to call web hook at '{_hookAddress}'. Exception:{ex.Message}", ex);
            }

            return false;
        }
    }
}