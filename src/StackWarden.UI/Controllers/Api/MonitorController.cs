using System.Web.Http;
using Microsoft.AspNet.SignalR;
using log4net;
using StackWarden.Monitoring;
using StackWarden.Core.Extensions;
using StackWarden.Core.Persistence;
using StackWarden.UI.Hubs;

namespace StackWarden.UI.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api/monitor")]
    public class MonitorController : ApiController
    {
        private readonly ILog _log;
        //private readonly IMonitorService _monitorService;

        public MonitorController(ILog log/*, IMonitorService monitorService*/)
        {
            _log = log.ThrowIfNull(nameof(log));
            //_monitorService = monitorService.ThrowIfNull(nameof(monitorService));
        }

        [HttpPost]
        [Route("result/hook")]
        public void ResultHook(MonitorResult result)
        {
          //  _monitorService.Save(result);
            NotifyResultAdded(result);
        }

        private void NotifyResultAdded(MonitorResult result)
        {
            var resultModel = (Models.Monitor)result;

            GlobalHost.ConnectionManager
                      .GetHubContext<NotificationHub>()
                      .Clients.All.addMonitorResult(resultModel);
        }
    }
}