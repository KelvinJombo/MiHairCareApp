using Microsoft.Extensions.Configuration;
namespace MiHairCareApp.Commons
{
    public static class ConfigurationHelper
    {
        private static IConfiguration _configuration;
        public static void InstantiateConfiguration(IConfiguration configuration) => _configuration = configuration;

        public static IConfiguration GetConfigurationInstance() => _configuration;
    }
}
