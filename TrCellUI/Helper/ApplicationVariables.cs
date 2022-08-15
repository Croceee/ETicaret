using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrCellUI
{
    public class ApplicationVariables
    {
        public string Version { get; set; }  
        public string ApplicationTitle { get; set; }
        public string WebApiUrl { get; set; }
        public string SqlDataBaseLayerConnectionString { get; set; }
        public string SitePath { get; set; } 

        public string AuthenticationPrivateKey { get; set; }
        public bool UseAuthentication { get; set; }
        public string WebApiUser { get; set; }
        public string WebApiPassword { get; set; }
   


    }
}
