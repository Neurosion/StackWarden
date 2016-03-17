using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using log4net;
using StackWarden.Core;
using StackWarden.Core.Extensions;

namespace StackWarden.Monitoring.Http
{
    public class AvailabilityMonitor : Monitor
    {
        private readonly string _address;

        public Dictionary<SeverityState, List<HttpStatusCode>> SeverityStatusCodes { get; } = new Dictionary<SeverityState, List<HttpStatusCode>>();
        
        public AvailabilityMonitor(ILog log, string address)
            :base(log, $"Availability monitor for {address}")
        {
            _address = address.ThrowIfNullOrWhiteSpace(nameof(address));
            InitializeDefaultStatusCodes();
            Tags.Add(Core.Configuration.Constants.Categories.Web);
        }

        private void InitializeDefaultStatusCodes()
        {
            SeverityStatusCodes.Add(SeverityState.Error,
                new List<HttpStatusCode>
                {
                    HttpStatusCode.BadRequest,
                    HttpStatusCode.BadGateway,
                    HttpStatusCode.Forbidden,
                    HttpStatusCode.GatewayTimeout,
                    HttpStatusCode.Gone,
                    HttpStatusCode.InternalServerError,
                    HttpStatusCode.MethodNotAllowed,
                    HttpStatusCode.NotFound,
                    HttpStatusCode.RequestTimeout,
                    HttpStatusCode.RequestEntityTooLarge,
                    HttpStatusCode.ServiceUnavailable,
                    HttpStatusCode.Unauthorized,
                    HttpStatusCode.NotImplemented
                });
            SeverityStatusCodes.Add(SeverityState.Normal,
                new List<HttpStatusCode>
                {
                    HttpStatusCode.OK,
                    HttpStatusCode.Accepted,
                    HttpStatusCode.Continue,
                    HttpStatusCode.Created,
                    HttpStatusCode.Found,
                    HttpStatusCode.PartialContent,
                    HttpStatusCode.Redirect,
                    HttpStatusCode.RedirectKeepVerb,
                    HttpStatusCode.RedirectMethod
                });
            SeverityStatusCodes.Add(SeverityState.Warning,
                Enum.GetValues(typeof(HttpStatusCode))
                    .OfType<HttpStatusCode>()
                    .Where(statusCode => SeverityStatusCodes.Values
                                                            .SelectMany(codeList => codeList)
                                                            .All(existingCode => existingCode != statusCode))
                    .ToList());
        }

        protected override void Update(MonitorResult result)
        {
            result.TargetName = _address;

            try
            {
                var request = WebRequest.Create(_address);
                request.Method = "HEAD";

                using (var response = request.GetResponse())
                {
                    Update(result, response as HttpWebResponse);
                    response.Close();       
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    Update(result, ex.Response as HttpWebResponse);
                }
                else
                {
                    result.TargetState = SeverityState.Error;
                    result.FriendlyMessage = ex.Message;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Update failed.", ex);
                result.TargetState = SeverityState.Error;
                result.FriendlyMessage = ex.ToDetailString();
            }
        }

        private void Update(MonitorResult result, HttpWebResponse response)
        {
            if (response == null)
            {
                result.TargetState = SeverityState.Error;
                result.FriendlyMessage = "No response received.";
            }
            else
            {
                var code = response.StatusCode;

                result.Details.Add("Status Code", code.ToExpandedString());
                result.Details.Add("Status Description", response.StatusDescription);

                result.TargetState = SeverityStatusCodes.Where(x => x.Value.Contains(code))
                                                        .Select(x => x.Key)
                                                        .FirstOrDefault();
                result.FriendlyMessage = $"{(int)code}: {response.StatusDescription}";
            }
        }
    }
}