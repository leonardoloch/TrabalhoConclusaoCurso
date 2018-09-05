using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Models
{
    public class ServidorExterno : ApiController
    {
        // GET api/values
        public string Get()
        {
            System.Threading.Thread.Sleep(5000);
            return "Oh ! at last long process finish";
        }
    }
}
