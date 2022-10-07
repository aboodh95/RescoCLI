using RescoCLI.Tasks;
using RescoCLI.Configurations;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace RescoCLI
{
    class Program
    {
        private static async Task Main(string[] args)
        {



            var builder = new HostBuilder();
            var appSettings = new ConfigurationBuilder()
                              .AddJsonFile(AppDomain.CurrentDomain.BaseDirectory + "\\appsettings.json", optional: true, reloadOnChange: true)
                              .Build();
            var configFilePath = appSettings["OverrideConfiguration:configFilePath"];
            if (!string.IsNullOrEmpty(configFilePath) && RescoCLIBase.ConfigFileExist(configFilePath))
            {
                Configuration.ConfigurationFilePath = configFilePath;
            }
            else
            {
                Configuration.ConfigurationFilePath = Path.Combine(Configuration.ConfigurationFolderPath, "RescoCLI.json");
            }

            await builder.RunCommandLineApplicationAsync<RescoCLICmd>(args);

        }

       


    }
}
