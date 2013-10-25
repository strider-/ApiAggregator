using System;
using System.Web;
using System.Web.Mvc;
using ApiAggregator.Core.Data;
using ApiAggregator.Core.Services;
using ApiAggregator.Web.ViewModels;
using ApiAggregator.Web.Extensions;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiAggregator.Web.Controllers
{
    [Authorize]
    public class TestController : BaseController<TestController> {
        
        private readonly IApiMappingRepository _apiRepo;
        private readonly IMatchingService _service;
        private readonly IContextGenerator _generator;

        public TestController(IApiMappingRepository apiRepo, IMatchingService service, IContextGenerator generator)
        {
            _apiRepo = apiRepo;
            _service = service;
            _generator = generator;
        }

        [HttpGet]
        public ActionResult Mapping(int id)
        {
            var mapping = _apiRepo.FindById(id);
            if(mapping == null)
            {
                return RedirectToAction<MappingController>(c => c.Index());
            }

            var url = string.Format("{0}/{1}", VirtualPathUtility.RemoveTrailingSlash(mapping.Service.RootUrl), mapping.Api.TrimStart('/'));
            var model = new TestModel
            {
                Mapping = mapping.ToViewModel(),
                VariableNames = _service.GetVariableNames(mapping),
                ServiceUrl = url
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Mapping(ApiTestModel model)
        {
            var mapping = _apiRepo.FindById(model.Id);
            var ep = _service.GetInterpolatedEndpoint(mapping, model.Pairs);
            var context = _generator.GetMappingContext(mapping, ep);

            if(context == null)
            {
                return Json(false, "Internal error attempting to resolve mapping context.");
            }

            if(model.Debug)
            {
                return Request.DebugResponse(context, Url);
            }
            else
            {
                try
                {
                    var result = await context.Execute();
                    var json = await result.Content.ReadAsStringAsync();
                    return Content(json, "application/json");
                }
                catch(HttpRequestException ex)
                {
                    return Json(false, ex.InnerException.Message);
                }
            }
        }
    }
}
