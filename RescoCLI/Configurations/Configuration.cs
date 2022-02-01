using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RescoCLI.Configurations
{
    public class Configuration
    {
        public Configuration()
        {
            Connections = new List<Connection>();
        }
        /// <summary>
        /// The path of the configuration folder
        /// </summary>
        public static string ConfigurationFolderPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RescoCLI");
        /// <summary>
        /// The path of the configuration file
        /// </summary>
        public static string ConfigurationFilePath { get; set; } = Path.Combine(ConfigurationFolderPath, "RescoCLI.json");
        /// <summary>
        /// Save the Configuration File
        /// </summary>
        /// <returns></returns>
        public async Task SaveConfigurationAsync()
        {
            await File.WriteAllTextAsync(ConfigurationFilePath, JsonConvert.SerializeObject(this));
        }
        /// <summary>
        /// Get the Configuration File
        /// </summary>
        /// <returns>Configuration File</returns>
        public static async Task<Configuration> GetConfigrationAsync()
        {
            Configuration _configuration = null;
            if (!File.Exists(Configuration.ConfigurationFilePath))
            {
                if (!Directory.Exists(Configuration.ConfigurationFolderPath))
                {
                    Directory.CreateDirectory(Configuration.ConfigurationFolderPath);
                }
                _configuration = new Configuration
                {
                    OfflineHTMLConfigurations = new List<OfflineHTMLConfiguration>(),
                    CodeGenerationConfiguration=  new CodeGenerationConfiguration(),
                };
                await _configuration.SaveConfigurationAsync();
            }
            else
            {
                _configuration = JsonConvert.DeserializeObject<Configuration>(await File.ReadAllTextAsync(ConfigurationFilePath));
            }
            return _configuration;
        }
        /// <summary>
        /// List of all Connections
        /// </summary>
        public List<Connection> Connections { get; set; }
        /// <summary>
        /// The Configuration for Code Generation Command
        /// </summary>
        public CodeGenerationConfiguration CodeGenerationConfiguration { get; set; }
        /// <summary>
        /// The Configuration for the Offline HTML commands
        /// </summary>
        public List<OfflineHTMLConfiguration> OfflineHTMLConfigurations { get; set; }
        /// <summary>
        /// The folder where you have pointed Resco to get the Offline HTML from it
        /// </summary>
        public string LocalOfflineHTMLFolder { get; set; }
    }
}
