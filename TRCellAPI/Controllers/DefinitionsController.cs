using EFCoreData.Models;
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
    public class DefinitionsController : ControllerBase
    {

        private readonly ILogger<DefinitionsController> _logger;
        ETicaretContext _context;
        public DefinitionsController(ILogger<DefinitionsController> logger, ETicaretContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("Product")]
        public  IActionResult GetProduct()
        {
           // var list = _context.TblProducts;
            var productList =  _context.Set<Product>().Include(p => p.ProductType).Select(x => new
            {
                ProductId = x.ProductId,
                Name =x.Name,
                Code =x.Code,
                Deleted = x.Deleted,
                LastUpdateDate =x.LastUpdateDate,
                ImageUrl = x.ImageUrl,
                ProductTypeId = x.ProductType.ProductTypeId,
                ProductType = x.ProductType,
                Price =x.Price
            
            });
         
            return new JsonResult(productList);
        }
        [HttpGet("Product/{productId}")]
        public IActionResult GetProductById([FromRoute] int productId)
        {
            // var list = _context.TblProducts;
            var product = _context.Set<Product>().Include(p => p.ProductType).Where(y => y.ProductId == productId).Select(x => new
            {
                ProductId=x.ProductId,
                Name = x.Name,
                Code = x.Code,
                Deleted = x.Deleted,
                LastUpdateDate = x.LastUpdateDate,
                ImageUrl = x.ImageUrl,
                ProductTypeId = x.ProductType.ProductTypeId,
                ProductType = x.ProductType,
                Price = x.Price

            });

            return new JsonResult(product);
        }
        [HttpGet("ProductType")]
        public IActionResult GetProductType([FromRoute] int userid)
        {
            var list = _context.ProductTypes;

            //SqlDataBaseLayer dbLayer = SqlDataBaseLayer.CreateFromConnectionString(connectionString);
            //dbLayer.OpenConnection();
            //Dashboard dashboard = new Dashboard(dbLayer);
            //dashboard.User_ID = userid;
            //dashboard.GetOfficeDashboard();
            //dbLayer.CloseConnection();
            //return error;
            return new JsonResult(list);
        }
       
    }
}
