using System.Linq;
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
        private readonly IRepository _repository;

        public MonitorController(ILog log, IRepository repository)
        {
            _log = log.ThrowIfNull(nameof(log));
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        [HttpPost]
        [Route("result/hook")]
        public void ResultHook(MonitorResult result)
        {
            var existingResults = _repository.Query<MonitorResult>();

            foreach (var currentExistingResult in existingResults.Where(x => x.SourceName == result.SourceName))
                _repository.Delete(currentExistingResult);

            _repository.Save(result);
            NotifyResultAdded(result);
        }

        private static void NotifyResultAdded(MonitorResult result)
        {
            var resultModel = (Models.Monitor)result;

            GlobalHost.ConnectionManager
                      .GetHubContext<NotificationHub>()
                      .Clients.All.addMonitorResult(resultModel);
        }
    }
}