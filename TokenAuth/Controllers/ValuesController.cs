using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TokenAuth.Routes;

namespace TokenAuth.Controllers
{
    [Route(ValuesRoutes.Prefix)]
    public class ValuesController : Controller
    {
        readonly MiddlewareAuth.Config.ConfigurationManager _configManager;
        public ValuesController(MiddlewareAuth.Config.ConfigurationManager configManager)
        {
            _configManager = configManager;
        }
        // GET api/values
        [HttpGet(ValuesRoutes.Get)]
        public IEnumerable<string> Get()
        {
            var sdf = _configManager.Appsettings<int>("Logging:Console:LogLevel:Count");
            return new [] { "value1", "value2", sdf.ToString() };
        }

        // GET api/values/5
        [HttpGet(ValuesRoutes.GetById)]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet(ValuesRoutes.GetByIdGuid)]
        public string GetByGuid(Guid id, string manafort, Guid manaforts)
        {
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
