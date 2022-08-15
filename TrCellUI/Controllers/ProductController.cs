using EFCoreData.Models;
using Legalport.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TrCellUI.Models;

namespace TrCellUI.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ILogger<ProductController> _logger;
        private ApiClientFactory apiClientFactory;

        public ProductController(ILogger<ProductController> logger, ApiClientFactory apiClientFactory)
        {
            _logger = logger;
            this.apiClientFactory = apiClientFactory;
        }

        public IActionResult Index()
        {
          
             return Redirect("~/Order/Order");
        }
        public IActionResult Order()
        {
            return View();
        }

        public async Task<JsonResult> GetProducts()
        {
            try
            {
                string querystring = "";
                string query = "Definitions/Product";

                var requesturl = apiClientFactory.Instance.CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, query), querystring);

                var response = await apiClientFactory.Instance.GetAsync<Product>(requesturl, "", "", 0);

                return Json(new
                {
                    response = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} in GetProducts");

                return Json(new
                {
                    result = false
                });
            }

        }

        public IActionResult LiveOrder()
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
