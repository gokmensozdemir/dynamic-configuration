using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfiguration.Services
{
    public interface IConfigurationService
    {
        Task<string> GetValueByNameAsync(string applicationName, string name);
    }
}
