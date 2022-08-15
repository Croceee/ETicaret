using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TrCellUI.Models;

using EFCoreData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using TrCellUI;

namespace Legalport.Controllers
{
    public class BaseController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

      
        private ApiClientFactory apiClientFactory;

        public string currentUserId = "";
        public string currentUserName = "";
        public string currentUsertype = "";
        public string currentOrderGuid = "";


        public BaseController( )
        {
         
          

        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var currentUserClaims = context.HttpContext.User.Claims;
            currentUserId = currentUserClaims.FirstOrDefault(x => x.Type == "UserId") == null ? " " : currentUserClaims.FirstOrDefault(x => x.Type == "UserId").Value;          
            currentUserName = currentUserClaims.FirstOrDefault(x => x.Type == "UserName") == null ? " " : currentUserClaims.FirstOrDefault(x => x.Type == "UserName").Value;
            currentUsertype = currentUserClaims.FirstOrDefault(x => x.Type == "Usertype") == null ? " " : currentUserClaims.FirstOrDefault(x => x.Type == "Usertype").Value;
            currentOrderGuid = currentUserClaims.FirstOrDefault(x => x.Type == "OrderGuid") == null ? " " : currentUserClaims.FirstOrDefault(x => x.Type == "OrderGuid").Value;

            base.OnActionExecuting(context);

        }
    }
}
