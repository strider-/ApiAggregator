using System.Configuration;

namespace ApiAggregator.Core.Services.Concrete
{
    public class ConfigFileConfigurationProvider : IConfigurationProvider
    {
        public string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public T GetSection<T>(string name)
        {
            return (T)ConfigurationManager.GetSection(name);
        }
    }
}