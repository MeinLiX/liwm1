namespace webAPI.Extensions;

public static class ConfigurationExtensions
{
    public static void ConfigureAppConfiguration(this ConfigurationManager configurationManager, IWebHostEnvironment environment, string[] args = null)
    {
        var builder = configurationManager
                  .AddJsonFile("Shared/sharedsettings.json", optional: true)
                  .AddJsonFile($"Shared/sharedsettings.{environment.EnvironmentName}.json", optional: true)
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();
        if (args != null)
        {
            builder.AddCommandLine(args);
        }
    }
}