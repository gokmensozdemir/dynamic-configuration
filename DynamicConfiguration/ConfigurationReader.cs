using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicConfiguration.Services;

namespace DynamicConfiguration
{
    public class ConfigurationReader
    {        
        private string _applicationName;

        private ICacheService _cacheService;

        private IConfigurationService _configurationService;

        private ConcurrentDictionary<string, string> _configurations;

        public ConfigurationReader(string applicationName, IConfigurationService configurationService, ICacheService cacheService)
        {
            _applicationName = applicationName;

            _configurations = new ConcurrentDictionary<string, string>();

            _configurationService = configurationService;

            _cacheService = cacheService;

            _cacheService.KeyInvalidated += CacheService_KeyInvalidated;
        }

        private void CacheService_KeyInvalidated(string applicationName, string name)
        {
            if (applicationName == _applicationName)
            {
                var result = _configurations.Remove(name, out _);

                if (!result)
                {
                    throw new Exception("In memory variables are not removed.");
                }
            }
        }

        public async Task<T> GetValue<T>(string key)
        {
            string value;

            if (_configurations.TryGetValue(key, out value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            value = await _cacheService.GetOrUpdateAsync($"{_applicationName}:{key}", () =>
            {
                return _configurationService.GetValueByNameAsync(_applicationName, key);
            });

            if (value != null)
            {
                _configurations.AddOrUpdate(key, value, (key, oldValue) => value);

                return (T)Convert.ChangeType(value, typeof(T));
            }

            return default(T);
        }
    }
}