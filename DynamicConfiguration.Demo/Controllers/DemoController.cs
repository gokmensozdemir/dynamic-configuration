using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamicConfiguration.Controllers
{
    [ApiController]
    [Route("")]
    public class DemoController : ControllerBase
    {
        private readonly ConfigurationReader configurationReader;

        public DemoController(ConfigurationReader configurationReader)
        {
            this.configurationReader = configurationReader;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(new Dictionary<string, object>
            {
                { "SiteName", await configurationReader.GetValue<string>("SiteName") },
                { "MaxItemCount", await configurationReader.GetValue<string>("MaxItemCount") },
                { "IsBasketEnabled", await configurationReader.GetValue<string>("IsBasketEnabled") },
                { "Price", await configurationReader.GetValue<string>("Price") }
            });
        }
    }
}
