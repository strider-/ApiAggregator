using System.Configuration;

namespace ApiAggregator.Core.Services
{
    public interface IConfigurationProvider
    {
        string GetSetting(string key);
        T GetSection<T>(string name);
    }
}
