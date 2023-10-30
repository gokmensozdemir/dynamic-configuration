using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DynamicConfiguration.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            string siteName = await configurationReader.GetValue<string>("SiteName");
            int maxItemCount = await configurationReader.GetValue<int>("MaxItemCount");
            bool isBasketEnabled = await configurationReader.GetValue<bool>("IsBasketEnabled");
            double price = await configurationReader.GetValue<double>("Price");

            return Ok(new
            {
                siteName,
                maxItemCount,
                isBasketEnabled,
                price
            });
        }
    }
}
