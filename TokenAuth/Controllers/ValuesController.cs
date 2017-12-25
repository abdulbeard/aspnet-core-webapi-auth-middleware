using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TokenAuth.Routes;

namespace TokenAuth.Controllers
{
    [Route(ValuesRoutes.Prefix)]
    public class ValuesController : Controller
    {
        MiddlewareAuth.Config.ConfigurationManager configManager;
        public ValuesController(MiddlewareAuth.Config.ConfigurationManager configManager)
        {
            var routeData = RouteData;
            this.configManager = configManager;
        }
        // GET api/values
        [HttpGet(ValuesRoutes.Get)]
        public IEnumerable<string> Get()
        {
            //var sdf = Program.Configuration["Logging:Console:LogLevel:Default"];
            var sdf = configManager.Appsettings<int>("Logging:Console:LogLevel:Count");
            var context = HttpContext;
            var routeData = RouteData;
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet(ValuesRoutes.GetById)]
        public string Get(int idklsjdfkljsldjfskldf)
        {
            var context = HttpContext;
            var routeData = RouteData;
            return "value";
        }

        [HttpGet(ValuesRoutes.GetByIdGuid)]
        public string GetByGuid(Guid id, string manafort, Guid manaforts)
        {
            var context = HttpContext;
            var routeData = RouteData;
            return "value";
        }

        // POST api/values
        [HttpPost(ValuesRoutes.Post)]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut(ValuesRoutes.Put)]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete(ValuesRoutes.Delete)]
        public void Delete(int id)
        {
        }
    }
}
