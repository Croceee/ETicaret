using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TrCellUI.Models;

using EFCoreData.Models;
using Legalport.Controllers;

namespace TrCellUI.Controllers
{
    public class DefinitionsController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private ApiClientFactory apiClientFactory;

        public DefinitionsController(ILogger<HomeController> logger, ApiClientFactory apiClientFactory)
        {
            _logger = logger;
            this.apiClientFactory = apiClientFactory;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<JsonResult> GetProduct()
        {
            try
            {
                string querystring = "";
                string query = "Product";

                var requesturl = apiClientFactory.Instance.CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, query), querystring);

                var response = await apiClientFactory.Instance.GetAsync<Product>(requesturl, "", "", 0);

                return Json(new
                {
                    response = response
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
        public async Task<JsonResult> GetProductById(string productId)
        {
            try
            {
                string querystring = "";
                string query = "Product\"+productId";

                var requesturl = apiClientFactory.Instance.CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, query), querystring);

                var response = await apiClientFactory.Instance.GetFirstAsync<Product>(requesturl, "", "", 0);

                return Json(new
                {
                    response = response
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
