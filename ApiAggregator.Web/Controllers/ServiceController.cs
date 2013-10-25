using System.Linq;
using System.Web.Mvc;
using ApiAggregator.Core.Data;
using ApiAggregator.Web.ViewModels;
using ApiAggregator.Web.Extensions;
using ApiAggregator.Core.Services;
using ApiAggregator.Web.Framework;
using System.Collections.Generic;

namespace ApiAggregator.Web.Controllers
{
    [Authorize]
    public class ServiceController : BaseController<ServiceController>
    {
        private readonly IServiceRepository _serviceRepo;
        private readonly IConfigurationProvider _config;

        public ServiceController(IServiceRepository serviceRepo, IConfigurationProvider config)
        {
            _serviceRepo = serviceRepo;
            _config = config;
        }

        public ActionResult Index()
        {
            var services = _serviceRepo.All().Select(s => s.ToViewModel());
            return View(services);
        }

        [HttpGet]
        public ActionResult Add()
        {
            AddTemplatesToViewBag();
            return View(new ServiceModel { Enabled = true });
        }

        [HttpPost]
        public ActionResult Add(ServiceModel model)
        {
            if(ModelState.IsValid)
            {
                var service = model.ToDomainModel();
                _serviceRepo.Add(service);
                _serviceRepo.SubmitChanges();

                Flash(FlashType.Success, string.Format("The '{0}' service has been added!", model.Name));
                return RedirectToAction(c => c.Index());
            }

            Flash(FlashType.Danger, "Please correct the errors below.");
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var service = _serviceRepo.FindById(id);
            if(service == null)
            {
                return RedirectToAction(c => c.Index());
            }

            return View(service.ToViewModel());
        }

        [HttpPost]
        public ActionResult Edit(ServiceModel model)
        {
            if(ModelState.IsValid)
            {
                var service = _serviceRepo.FindById(model.Id);
                model.ToDomainModel(service);

                _serviceRepo.Update(service);
                _serviceRepo.SubmitChanges();

                Flash(FlashType.Success, string.Format("The '{0}' service has been updated!", model.Name));
                return RedirectToAction(c => c.Index());
            }

            Flash(FlashType.Danger, "Please correct the errors below.");
            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var service = _serviceRepo.FindById(id);
            if(service != null)
            {
                _serviceRepo.Delete(service);
                _serviceRepo.SubmitChanges();
                return Json(true, "");
            }
            return Json(false, "Service could not be found.");
        }

        private void AddTemplatesToViewBag()
        {
            var templates = _config.GetSection<ServiceTemplateSection>("serviceTemplates");
            var result = templates.Templates.OfType<ServiceTemplateElement>()
                .Select(e => new ServiceModel
                {
                    Name = e.Name,
                    RootUrl = e.RootUrl,
                    Headers = e.Headers.OfType<ServiceTemplatePairElement>().Select(h => new ServiceHeaderModel { Header = h.Name, Value = h.Value }).ToList(),
                    QueryStringAppends = e.QueryStrings.OfType<ServiceTemplatePairElement>().Select(h => new ServiceQueryStringModel { Name = h.Name, Value = h.Value }).ToList()
                });

            ViewBag.ServiceTemplates = result;
        }
    }
}