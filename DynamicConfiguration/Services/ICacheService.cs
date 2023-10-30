using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfiguration.Services
{
    public interface ICacheService
    {
        event KeyInvalidateHandler KeyInvalidated;
        Task<string> GetOrUpdateAsync(string key, Func<Task<string>> valueFactory, TimeSpan? expiry = null);
    }
}
