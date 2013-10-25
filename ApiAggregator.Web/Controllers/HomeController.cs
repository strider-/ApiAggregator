using System.Linq;
using System.Web.Mvc;
using ApiAggregator.Core.Data;
using ApiAggregator.Web.ViewModels;

namespace ApiAggregator.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController<HomeController>
    {
        private readonly IApiMappingRepository _apiRepo;

        public HomeController(IApiMappingRepository apiRepo)
        {
            _apiRepo = apiRepo;
        }

        public ActionResult Index()
        {
            var recentItems = _apiRepo.RecentlyAdded(10)
                .Select(r => new RecentItemModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Type = r.Type,
                    Url = Url.Action("Edit", r.Type, new { id = r.Id }),
                    Icon = DetermineIcon(r.Type)
                });
            return View(recentItems);
        }

        public ActionResult NotFound()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult ServicesAndMappings()
        {
            ViewBag.Test = 10;
            return PartialView(new NavMenuModel
            {
                Services = _apiRepo.AvailableServices(),
                Mappings = _apiRepo.AvailableMappings()
            });
        }

        private string DetermineIcon(string type)
        {
            switch(type.ToLower())
            {
                case "service":
                    return "globe";
                case "mapping":
                    return "link";
                default:
                    return null;
            }
        }
    }
}
