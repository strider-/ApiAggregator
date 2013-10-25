using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ApiAggregator.Core.Data;
using ApiAggregator.Core.Models;
using ApiAggregator.Core.Services;
using ApiAggregator.Web.ViewModels;

namespace ApiAggregator.Web.Controllers
{
    public class AccountController : BaseController<AccountController>
    {
        private readonly IConfigurationRepository _repo;
        private readonly IAuthenticationService _authService;
        private readonly IHashingService _hasher;

        public AccountController(IConfigurationRepository repo, IAuthenticationService authService, IHashingService hasher)
        {
            _repo = repo;
            _authService = authService;
            _hasher = hasher;
        }

        [HttpGet]
        public ActionResult Setup()
        {
            var option = _repo.All().Single();
            if(SetupCompleted(option))
            {
                return RedirectToAction<HomeController>(c => c.Index());
            }

            return View(new SetupModel());
        }

        [HttpPost]
        public ActionResult Setup(SetupModel model)
        {
            var option = _repo.All().Single();
            if(SetupCompleted(option))
            {
                return RedirectToAction<HomeController>(c => c.Index());
            }

            if(ModelState.IsValid)
            {
                option.Username = model.Username;
                option.Password = _hasher.HashPassword(model.Password, _hasher.GenerateSalt());
                _repo.Update(option);

                CompleteLogin(option.Username);
                return RedirectToAction<HomeController>(c => c.Index());
            }

            return View("Setup", model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            var option = _repo.All().Single();
            if(Request.IsAuthenticated || !option.RequireLogin)
            {
                CompleteLogin(option.Username);
                return RedirectToAction<HomeController>(c => c.Index());
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if(ModelState.IsValid)
            {
                if(_authService.Authenticate(model.Username, model.Password))
                {
                    CompleteLogin(model.Username);
                    if(Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction<HomeController>(c => c.Index());
                    }
                }
                else
                {
                    ViewBag.Message = "The username and/or password are invalid.";
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.User = null;
            FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }

        [ChildActionOnly]
        public PartialViewResult LogoutButton()
        {
            // If we've got an option in the ViewBag from the EnsureSetup filter,
            // then lets use that instead of going back to the db.
            var option = ViewBag.Option ?? _repo.All().Single();
            var model = option.RequireLogin && Request.IsAuthenticated;

            return PartialView(model);
        }

        private bool SetupCompleted(Configuration option)
        {
            return !string.IsNullOrWhiteSpace(option.Username) && !string.IsNullOrWhiteSpace(option.Password);
        }

        private void CompleteLogin(string userName)
        {
            HttpContext.User = new GenericPrincipal(new GenericIdentity(userName), null);
            FormsAuthentication.SetAuthCookie(userName, false);
        }
    }
}
