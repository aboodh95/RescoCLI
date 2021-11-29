using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RescoCLI.Helpers
{
    public class Configuration
    {
        public Configuration()
        {
            Connections = new List<Connection>();
        }
        public static string ConfigurationFolderPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RescoCLI");
        public static string ConfigurationFilePath { get; set; } = Path.Combine(ConfigurationFolderPath, "RescoCLI.json");
        public List<Connection> Connections { get; set; }
        public string SelectedProjectId { get; set; }
        public async Task SaveConfigurationAsync()
        {
            await File.WriteAllTextAsync(ConfigurationFilePath, JsonConvert.SerializeObject(this));
        }
        public static async Task<Configuration> GetConfigrationAsync()
        {
            Configuration _configuration = null;
            if (!File.Exists(Configuration.ConfigurationFilePath))
            {
                if (!Directory.Exists(Configuration.ConfigurationFolderPath))
                {
                    Directory.CreateDirectory(Configuration.ConfigurationFolderPath);
                }

                 _configuration = new Configuration();
                await _configuration.SaveConfigurationAsync();
            }
            else
            {
                _configuration = JsonConvert.DeserializeObject<Configuration>(await File.ReadAllTextAsync(ConfigurationFilePath));
            }
            return _configuration;
        }

        public string CSharpEntitiesFolderPath { get; set; }
        public string TSEntitiesFolderPath { get; set; }
        /// <summary>
        /// Active Project Form Libraries Path
        /// </summary>
        public string ActiveProjectFormLibrariesPath { get; set; }

    }
}
