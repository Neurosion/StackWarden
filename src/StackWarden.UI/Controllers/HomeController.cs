using System;
using System.Web.Mvc;
using System.Linq;
using log4net;
using StackWarden.Core.Extensions;
using StackWarden.Core.Persistence;
using StackWarden.Monitoring;
using StackWarden.Tools;

using Monitor = StackWarden.UI.Models.Monitor;

namespace StackWarden.UI.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private readonly ILog _log;
        private readonly IRepository _repository;

        public HomeController(ILog log, IRepository repository)
        {
            _log = log.ThrowIfNull(nameof(log));
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        [Route("")]
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        [Route("Dashboard")]
        public ActionResult Dashboard()
        {
            var results = _repository.Query<Result>()
                                     .GroupBy(x => x.Source != null ? x.Source.Name : null)
                                     .Select(x => x.OrderByDescending(r => r.SentOn).First())
                                     .Select(x => (Monitor)x)
                                     .ToList();

            return View(results);
        }

        [Route("home/tagged")]
        public ActionResult Category(string[] tags)
        {
            var monitorResults = _repository.Query<Result>()
                                            .Where(x => !tags.Except(x.Tags).Any())
                                            .GroupBy(x => x.Source != null ? x.Source.Name : null)
                                            .Select(x => x.OrderByDescending(r => r.SentOn).First())
                                            .ToList();
            var results = monitorResults.Select(m =>
                                        {
                                            var monitor = (Monitor)m;
                                            //monitor.Tools = _repository.Query<ITool>()
                                            //                     .Where(t => t.Category == m.SourceCategory)
                                            //                     .Select(t => new Models.Tool
                                            //                     {
                                            //                        Name = t.Name,
                                            //                        Description = t.Description
                                            //                     }).ToList();

                                            return monitor;
                                        });

            return View(results);
        }
    }
}