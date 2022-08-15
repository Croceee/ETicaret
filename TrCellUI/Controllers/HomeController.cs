using EFCoreData.Models;
using Legalport.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrCellUI.Models;

namespace TrCellUI.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public async Task<JsonResult> DoLogin()
        {
            try
            {
                //Login yapısında kullanıcı üzerinden gelebilir
                List<Claim> userClaims = new List<Claim>();
                userClaims.Add(new Claim("UserId", "1"));
                userClaims.Add(new Claim("UserName", "Test User"));
                userClaims.Add(new Claim("OrderGuid", Guid.NewGuid().ToString()));
                userClaims.Add(new Claim("Usertype","1"));
       

                ClaimsIdentity identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                AuthenticationProperties properties = new AuthenticationProperties();
                properties.IsPersistent = false;
                properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(30);
                await HttpContext.SignInAsync(principal, properties);

                TransactionResult<User> result = new TransactionResult<User>();
                result.Result = true;

                return Json(new
                {
                    response = result
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    result = false
                });
            }
           



        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
