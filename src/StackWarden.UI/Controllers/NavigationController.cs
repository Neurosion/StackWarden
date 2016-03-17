using System.Web.Mvc;
using System.Linq;
using StackWarden.Core.Extensions;
using StackWarden.Tools.Services;
using StackWarden.UI.Models;

namespace StackWarden.UI.Controllers
{
    public class NavigationController : Controller
    {
        public ActionResult Index()
        {
            //var toolSets = _toolSetService.Get();
            //var categories = toolSets.SelectMany(x => x.Categories)
            //                         .Distinct();
            var categories = new string[0];
            var model = new NavigationCategories();

            model.Categories.AddRange(categories);

            return PartialView(model);
        }
    }
}