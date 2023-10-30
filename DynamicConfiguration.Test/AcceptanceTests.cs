using System.Threading.Tasks;
using Xunit;
using DynamicConfiguration.Services;
using DynamicConfiguration.Models;

namespace DynamicConfiguration.Test
{
    public class AcceptanceTests
    {
        ConfigurationService configurationService;
        CacheService cacheService;
        public AcceptanceTests()
        {
            configurationService = new ConfigurationService("mongodb://dbadmin:WlhsS2FHSkhZMmx@127.0.0.1:27017");
            cacheService = new CacheService("127.0.0.1:6379,password=WlhsS2FHSkhZMmx");
        }

        private async Task InitData()
        {
            await configurationService.CreateConfigurationAsync(new ConfigurationDocument
            {
                ApplicationName = "SERVICE-C",
                Name = "SiteName",
                Type = "String",
                Value = "beymen.com.tr",
                IsActive = true
            });

            await configurationService.CreateConfigurationAsync(new ConfigurationDocument
            {
                ApplicationName = "SERVICE-C",
                Name = "MaxItemCount",
                Type = "Int",
                Value = "5",
                IsActive = true
            });

            await configurationService.CreateConfigurationAsync(new ConfigurationDocument
            {
                ApplicationName = "SERVICE-C",
                Name = "IsBasketEnabled",
                Type = "Boolean",
                Value = "true",
                IsActive = true
            });

            await configurationService.CreateConfigurationAsync(new ConfigurationDocument
            {
                ApplicationName = "SERVICE-C",
                Name = "Price",
                Type = "Double",
                Value = "34,5",
                IsActive = true
            });

            await configurationService.CreateConfigurationAsync(new ConfigurationDocument
            {
                ApplicationName = "SERVICE-C",
                Name = "Url",
                Type = "String",
                Value = "google.com.tr",
                IsActive = false
            });
        }

        private async Task RemoveData()
        {
            await configurationService.RemoveByApplicationName("SERVICE-C");
            await cacheService.KeyDeleteAsync("SERVICE-C:SiteName");
            await cacheService.KeyDeleteAsync("SERVICE-C:MaxItemCount");
            await cacheService.KeyDeleteAsync("SERVICE-C:IsBasketEnabled");
            await cacheService.KeyDeleteAsync("SERVICE-C:Price");
            await cacheService.KeyDeleteAsync("SERVICE-C:Url");
        }

        [Fact]
        public async Task Test_GetValue_Should_Return_Configurations_Successfully()
        {
            var configurationReader = new ConfigurationReader("SERVICE-C", configurationService, cacheService);

            await InitData();

            string siteName = await configurationReader.GetValue<string>("SiteName");
            int maxItemCount = await configurationReader.GetValue<int>("MaxItemCount");
            bool isBasketEnabled = await configurationReader.GetValue<bool>("IsBasketEnabled");
            double price = await configurationReader.GetValue<double>("Price");
            string url = await configurationReader.GetValue<string>("Url");

            await RemoveData();

            Assert.Equal("beymen.com.tr", siteName);
            Assert.Equal(5, maxItemCount);
            Assert.True(isBasketEnabled);
            Assert.Equal(34.5, price);
            Assert.Null(url);
        }
    }
}
