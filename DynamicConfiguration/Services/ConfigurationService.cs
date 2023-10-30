using DynamicConfiguration.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfiguration.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private IMongoCollection<ConfigurationDocument> _configurationCollection;
        public ConfigurationService(string mongoDbConnectionString)
        {
            var mongoClient = new MongoClient(mongoDbConnectionString);

            var database = mongoClient.GetDatabase("configurationdb");

            _configurationCollection = database.GetCollection<ConfigurationDocument>("configurations");
        }

        public async Task<string> GetValueByNameAsync(string applicationName, string name)
        {
            var filter = Builders<ConfigurationDocument>.Filter.Eq(n => n.IsActive, true)
             & Builders<ConfigurationDocument>.Filter.Eq(n => n.ApplicationName, applicationName)
             & Builders<ConfigurationDocument>.Filter.Eq(n => n.Name, name);

            var configuration = await _configurationCollection.Find(filter).FirstOrDefaultAsync();

            return configuration?.Value;
        }

        public Task CreateConfigurationAsync(ConfigurationDocument configurationDocument)
        {
            return _configurationCollection.InsertOneAsync(configurationDocument);
        }

        public Task RemoveByApplicationName(string applicationName)
        {
            return _configurationCollection.DeleteManyAsync(x => x.ApplicationName == applicationName);
        }

        public Task<ConfigurationDocument> FindAndUpdateConfigurationAsync(string applicationName, string name,  string value)
        {
            var filter = Builders<ConfigurationDocument>.Filter.Eq(n => n.ApplicationName, applicationName)
                & Builders<ConfigurationDocument>.Filter.Eq(n => n.Name, name);

            var updateDefn = Builders<ConfigurationDocument>.Update
                .Set(x => x.Value, value);

            return _configurationCollection.FindOneAndUpdateAsync(filter, updateDefn);
        }
    }
}
