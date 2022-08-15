using EFCoreData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TRCellAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<DefinitionsController> _logger;
        ETicaretContext _context;
        public OrderController(ILogger<DefinitionsController> logger, ETicaretContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public  IActionResult GetOrder()
        {
           // var list = _context.TblProducts;
            var orderList =  _context.Set<Order>().Include(p => p.OrderDetails).Include(u => u.User).Select(x => new
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                State = x.State,
                LastUpdateDate =x.LastUpdateDate,
                Address = x.Address,
                OrderGuid = x.OrderGuid,
                UserId = x.UserId,
                OrderDetails = x.OrderDetails.ToArray(),
                User=x.User
            
            });
         
            return new JsonResult(orderList);
        }

        [HttpGet("TempOrderByUser/{userid}")]
        public IActionResult GetOrderByGuid([FromRoute] int userid)
        {

            var orderList = _context.Set<Order>().Include(p => p.OrderDetails).ThenInclude(o=>o.Product).Include(u => u.User).Where(y=>y.UserId == userid && y.State==0).Select(x => new
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                State = x.State,
                LastUpdateDate = x.LastUpdateDate,
                Address = x.Address,
                OrderGuid = x.OrderGuid,
                UserId = x.UserId,
                OrderDetails = x.OrderDetails.ToArray(),
                User = x.User

            }).OrderByDescending(z=>z.LastUpdateDate).FirstOrDefault();

            return new JsonResult(orderList);
        }

        [HttpGet("{orderId}")]
        public IActionResult GetOrderById([FromRoute] int orderId)
        {
            // var list = _context.TblProducts;
            var order = _context.Set<Order>().Include(p => p.OrderDetails).Include(u => u.User).Where(y => y.OrderId == orderId).Select(x => new
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                State = x.State,
                LastUpdateDate = x.LastUpdateDate,
                Address = x.Address,
                OrderGuid = x.OrderGuid,
                UserId = x.UserId,
                OrderDetails = x.OrderDetails.ToArray(),
                User = x.User

            });

            return new JsonResult(order);
        }

        [HttpGet("GetOrderByUserId/{userid}")]
        public IActionResult GetOrderByUserId([FromRoute] int userid)
        {

            var orderList = _context.Set<Order>().Include(p => p.OrderDetails).ThenInclude(o => o.Product).Include(u => u.User).Where(y => y.UserId == userid && y.State > 0).Select(x => new
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                State = x.State,
                LastUpdateDate = x.LastUpdateDate,
                Address = x.Address,
                OrderGuid = x.OrderGuid,
                UserId = x.UserId,
                OrderDetails = x.OrderDetails.ToArray(),
                User = x.User

            }).OrderByDescending(z => z.LastUpdateDate);

            return new JsonResult(orderList);
        }

        [HttpPost]
        public IActionResult SaveOrder(Order order)
        {
            try
            {
                Order newOrder = new Order();

                List<OrderDetail> lstOrderDetail = new List<OrderDetail>();

                var oldOrder = _context.Set<Order>().Where(y => y.OrderGuid == order.OrderGuid).FirstOrDefault();
                //Önceki tamamlanmamış siparişler iptal edilsin.
                var oldTempOrder = _context.Set<Order>().Where(y => y.UserId == order.UserId && y.State == 0 && y.OrderGuid!=order.OrderGuid);
                if (oldTempOrder != null)
                {
                    foreach (var oldTemp in oldTempOrder)
                    {
                        oldTemp.State = -1;
                        _context.Orders.Update(oldTemp);
                    }
                }
                //Siparişin eski detayları silinsin
                if (oldOrder != null )
                {
                    var oldOrders = _context.Set<OrderDetail>().Where(y => y.OrderId == oldOrder.OrderId);

                    foreach (var oldOrderDetail in oldOrders) {
                        _context.OrderDetails.Remove(oldOrderDetail);
                    }                 

                }

                var newOrderDetails = order.OrderDetails.GroupBy(x => x.ProductId).Select(s => new
                {
                    ProductId = s.Min(m => m.ProductId),
                    Count = s.Sum(m=>m.Count)
                });

                foreach (var newOrderDetail in newOrderDetails)
                {
                    OrderDetail orderDetail=new OrderDetail();
                    orderDetail.ProductId = newOrderDetail.ProductId;
                    orderDetail.Count=newOrderDetail.Count;

                    lstOrderDetail.Add(orderDetail);
                }

                newOrder.OrderDate = System.DateTime.Now;
                newOrder.LastUpdateDate = System.DateTime.Now;
                newOrder.Address = order.Address;
                newOrder.OrderGuid = order.OrderGuid;//nerede oluşacak?
                newOrder.UserId = order.UserId;
                newOrder.OrderDetails = (ICollection<OrderDetail>)lstOrderDetail;
                newOrder.State = order.State;
                if (oldOrder != null)
                {
                    oldOrder.Address = order.Address;
                    oldOrder.OrderDate = System.DateTime.Now;
                    oldOrder.LastUpdateDate = System.DateTime.Now;
                    oldOrder.State = order.State;
                    oldOrder.OrderDetails = (ICollection<OrderDetail>)lstOrderDetail;

                    _context.Orders.Update(oldOrder);
                    _logger.LogWarning($"DataBase'e {newOrder.OrderGuid.ToString()} keyli sipariş güncellendi.");

                }
                else
                {
                    _context.Orders.Add(newOrder);
                    _logger.LogWarning($"DataBase'e {newOrder.OrderGuid.ToString()} keyli sipariş eklendi.");


                }
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new
                {
                    message = "Success"
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} in SaveOrder");

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = e.Message
                });
            }

        }

    }
}
