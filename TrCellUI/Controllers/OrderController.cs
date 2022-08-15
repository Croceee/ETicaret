using EFCoreData.Models;
using Legalport.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TrCellUI.Models;

namespace TrCellUI.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private ApiClientFactory apiClientFactory;

        public OrderController(ILogger<HomeController> logger, ApiClientFactory apiClientFactory)
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
        public IActionResult MyPage()
        {
            return View();
        }


        public async Task<JsonResult> AddOrder(string order)
        {
            try
            {

                Order order_ = JsonConvert.DeserializeObject<Order>(order);

                var factory = new ConnectionFactory() { HostName = "localhost", UserName = "admin", Password = "admin" };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var message = order;// JsonConvert.SerializeObject(order);

                        channel.QueueDeclare(
                            queue: "Order",
                            durable: true,
                            exclusive: false,
                            arguments: null,
                            autoDelete:false

                            );

                        channel.BasicPublish(exchange: "",
                                             routingKey: "Order",
                                             basicProperties: null,
                                             body: Encoding.UTF8.GetBytes(message)
                                             );

                    }
                }
                TransactionResult<User> result = new TransactionResult<User>();
                result.Result = true;
   
                _logger.LogWarning($"AddOrder {order_.OrderGuid.ToString()} keyli siparişte değişiklik yapıldı.");

                return Json(new
                {
                    response = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} in AddOrder");

                return Json(new
                {
                    result = false
                });

            }

        }
        public async Task<JsonResult> GetTempOrder(string userid)
        {
            try
            {
                string querystring = "";
                string query = "Order/TempOrderByUser/"+ userid;

                var requesturl = apiClientFactory.Instance.CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, query), querystring);

                var response = await apiClientFactory.Instance.GetFirstAsync<Order>(requesturl, "", "", 0);

                _logger.LogWarning($"GetTempOrder {userid} userId'si için geçici siparişler sorgulandı.");

                return Json(new
                {
                    response = response
                });

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} in GetTempOrder");

                return Json(new
                {
                    result = false
                });
            }
        }
        public async Task<JsonResult> GetOrderByUserId(string userid)
        {
            try
            {
                string querystring = "";
                string query = "Order/GetOrderByUserId/" + userid;

                var requesturl = apiClientFactory.Instance.CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture, query), querystring);

                var response = await apiClientFactory.Instance.GetAsync<Order>(requesturl, "", "", 0);

                _logger.LogWarning($"GetTempOrder {userid} userId'si için geçici siparişler sorgulandı.");

                return Json(new
                {
                    response = response
                });

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} in GetTempOrder");

                return Json(new
                {
                    result = false
                });
            }
        }

        public IActionResult CreateOrder()
        {
            return View();
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
