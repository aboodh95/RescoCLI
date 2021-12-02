
using Resco.Cloud.Client.Data.Fetch;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using RescoCLI.Configurations;
using System.Linq;
using System.IO.Compression;
using System.Net;

namespace RescoCLI.Tasks
{
    [Command(Name = "import", Description = "Import woodford project to Resco cloud", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    public class ImportProjectCmd : HARTBBase
    {
        Resco.Cloud.Client.WebService.DataService _service;

        [Option(CommandOptionType.SingleValue, ShortName = "pid", LongName = "projectId", Description = "The Id of the project to import", ValueName = "project id", ShowInHelpText = true)]
        public string ProjectId { get; set; }
        [Option(CommandOptionType.SingleValue, ShortName = "p", LongName = "path", Description = "The path of the ZIP file", ValueName = "project path", ShowInHelpText = true)]
        public string ProjectPath { get; set; }
        [Option(CommandOptionType.NoValue, ShortName = "publish", LongName = "publish", Description = "Publish the project", ValueName = "publish", ShowInHelpText = true)]
        public bool Publish { get; set; } = false;
        public ImportProjectCmd(ILogger<HARTBCmd> logger, IConsole console)
        {



            var configuration = Configuration.GetConfigrationAsync().Result;
            var selectedConnections = configuration.Connections.FirstOrDefault(x => x.IsSelected);
            if (selectedConnections == null)
            {
                throw new Exception("No connection do exists");
            }
            _service = new Resco.Cloud.Client.WebService.DataService(selectedConnections.URL)
            {
                Credentials = new NetworkCredential(selectedConnections.UserName, selectedConnections.Password)
            };

        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            if (string.IsNullOrEmpty(ProjectId))
            {
                Console.WriteLine("Project Id or Name should be passed");
                return 1;
            }
            if (!File.Exists(ProjectPath))
            {
                throw new FileNotFoundException("Cannot find project file");
            }
            await _service.ImportProjectAsync(ProjectId, Publish, ProjectPath);
            return 0;
        }


    }
}
