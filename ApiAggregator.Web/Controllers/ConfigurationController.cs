using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApiAggregator.Core.Data;
using ApiAggregator.Core.Models;
using ApiAggregator.Core.Services;
using ApiAggregator.Web.ViewModels;

namespace ApiAggregator.Web.Controllers
{
    [Authorize]
    public class ConfigurationController : BaseController<ConfigurationController>
    {
        private readonly IConfigurationRepository _repo;
        private readonly IHashingService _hasher;

        public ConfigurationController(IConfigurationRepository repo, IHashingService hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public ActionResult Index()
        {
            var option = _repo.All().Single();
            var model = new ConfigurationModel
            {
                ApiKey = option.Apikey,
                SecurityOption = option.SecurityOption,
                RequireLogin = option.RequireLogin,
                RequireAuthenticator = option.RequireAuthenticator,
                Username = option.Username,
                Password = option.Password,
                DescribeAtRoot = option.DescribeAtRoot
            };

            AddOptionsToViewBag();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ConfigurationModel model)
        {
            if(ModelState.IsValid)
            {
                var option = _repo.All().Single();
                option.SecurityOption = model.SecurityOption;
                option.RequireLogin = model.RequireLogin;
                option.RequireAuthenticator = model.RequireAuthenticator;
                option.DescribeAtRoot = model.DescribeAtRoot;
                option.Username = model.Username;
                if(!string.IsNullOrWhiteSpace(model.Password))
                {
                    option.Password = _hasher.HashPassword(model.Password, _hasher.GenerateSalt());
                }

                _repo.Update(option);

                Flash(FlashType.Success, "Security options successfully saved!");
                return RedirectToAction<HomeController>(c => c.Index());
            }

            AddOptionsToViewBag();
            Flash(FlashType.Danger, "Please correct the errors below!");
            return View("Index", model);
        }

        [HttpPost]
        public JsonResult GenerateApiKey()
        {
            var option = _repo.All().Single();
            option.Apikey = _hasher.GenerateApiKey();

            _repo.Update(option);

            return Json(true, option.Apikey);
        }

        private void AddOptionsToViewBag()
        {
            var options = new List<SecurityOptionModel>
            {
                new SecurityOptionModel { Option = SecurityOption.None, Name="None", Description ="API key is not required."}, 
                new SecurityOptionModel { Option = SecurityOption.KeyInQueryString, Name="Query String", Description ="API key must be passed in as the 'apikey' query string parameter."},
                new SecurityOptionModel { Option = SecurityOption.KeyInRequestHeader, Name="Request Header", Description ="API key must be the parameter in the Authorization header, with the scheme of 'Apikey'."},
                new SecurityOptionModel { Option = SecurityOption.SignedRequest, Name="Signed Request", Description="Click 'Request Signing' for more information."}
            };

            ViewBag.Options = options;
        }
    }
}