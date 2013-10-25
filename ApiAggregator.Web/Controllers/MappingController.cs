using System.Linq;
using System.Web.Mvc;
using ApiAggregator.Core.Data;
using ApiAggregator.Web.ViewModels;
using ApiAggregator.Web.Extensions;

namespace ApiAggregator.Web.Controllers
{
    [Authorize]
    public class MappingController : BaseController<MappingController>
    {
        private readonly IApiMappingRepository _apiRepo;

        public MappingController(IApiMappingRepository apiRepo)
        {
            _apiRepo = apiRepo;
        }

        public ActionResult Index()
        {
            var mappings = _apiRepo.All().Select(m => m.ToViewModel());
            ViewBag.AvailableServices = _apiRepo.AvailableServices().Select(s => s.Value);
            return View(mappings);
        }

        [HttpGet]
        public ActionResult Add(int serviceId = 0)
        {
            return ViewWithState("Add", new MappingModel
            {
                ServiceId = serviceId,
                Enabled = true
            });
        }

        [HttpPost]
        public ActionResult Add(MappingModel model)
        {
            if(ModelState.IsValid)
            {
                var mapping = model.ToDomainModel();
                _apiRepo.Add(mapping);
                _apiRepo.SubmitChanges();

                Flash(FlashType.Success, string.Format("The '{0}' mapping has been created!", model.Name));
                return RedirectToAction(c => c.Index());
            }

            Flash(FlashType.Danger, "Please correct the errors below.");
            return ViewWithState("Add", model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var mapping = _apiRepo.FindById(id);
            if(mapping == null)
            {
                return RedirectToAction(c => c.Index());
            }

            return ViewWithState("Edit", mapping.ToViewModel());
        }

        [HttpPost]
        public ActionResult Edit(MappingModel model)
        {
            if(ModelState.IsValid)
            {
                var mapping = _apiRepo.FindById(model.Id);
                model.ToDomainModel(mapping);

                _apiRepo.Update(mapping);
                _apiRepo.SubmitChanges();

                Flash(FlashType.Success, string.Format("The '{0}' mapping has been updated!", model.Name));
                return RedirectToAction(c => c.Index());
            }

            Flash(FlashType.Danger, "Please correct the errors below.");
            return ViewWithState("Edit", model);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var mapping = _apiRepo.FindById(id);
            if(mapping != null)
            {
                _apiRepo.Delete(mapping);
                _apiRepo.SubmitChanges();
                return Json(true, "");
            }
            return Json(false, "Mapping could not be found.");
        }

        private ViewResult ViewWithState(string viewName, MappingModel model)
        {
            var services = _apiRepo.AvailableServices().Select(kvp => new { Id = kvp.Key, Name = kvp.Value });
            ViewBag.AvailableServices = new SelectList(services, "Id", "Name");

            var methods = new string[] { "GET", "POST", "PUT", "DELETE", "HEAD", "TRACE", "OPTIONS" }.OrderBy(s => s);
            ViewBag.AvailableMethods = new SelectList(methods, "GET");

            return View(viewName, model);
        }
    }
}