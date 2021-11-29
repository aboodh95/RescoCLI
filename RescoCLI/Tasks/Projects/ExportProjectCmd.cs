
using Resco.Cloud.Client.Data.Fetch;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using RescoCLI.Helpers;
using System.Linq;
using System.IO.Compression;
using System.Net;

namespace RescoCLI.Tasks
{
    [Command(Name = "export",Description ="Export Woodford Project from Resco cloud", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    public class ExportProjectCmd : HARTBBase
    {
        Resco.Cloud.Client.WebService.DataService _service;

        [Option(CommandOptionType.SingleValue, ShortName = "pid", LongName = "projectId", Description = "The Id of the project to export", ValueName = "project id", ShowInHelpText = true)]
        public string ProjectId { get; set; }
        [Option(CommandOptionType.SingleValue, ShortName = "pname", LongName = "projectName", Description = "The name of the project to export", ValueName = "project name", ShowInHelpText = true)]
        public string ProjectName { get; set; }
        [Option(CommandOptionType.NoValue, ShortName = "e", LongName = "extract", Description = "extract files from zip", ValueName = "extract files from zip", ShowInHelpText = true)]
        public bool Extract { get; set; } = true;

        public ExportProjectCmd(ILogger<HARTBCmd> logger, IConsole console)
        {

            _logger = logger;
            _console = console;
            var configuration =  Configuration.GetConfigrationAsync().Result;
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
            if (string.IsNullOrEmpty(ProjectName) && string.IsNullOrEmpty(ProjectId))
            {
                Console.WriteLine("Project Id or Name should be passed");
                return 0;
            }
            var folder = Environment.CurrentDirectory;
            var fetch = new Fetch("mobileproject");
            fetch.Entity.AddAttribute("name");
            fetch.Entity.AddAttribute("id");
            fetch.Entity.AddAttribute("resco_appid");
            fetch.Entity.Filter = new Filter();
            if (!string.IsNullOrEmpty(ProjectId) && Guid.TryParse(ProjectId,out Guid id))
            {
                fetch.Entity.Filter.Where("id", "eq", id);
            }
            else
            {
                fetch.Entity.Filter.Where("name", "eq", ProjectName);
            }
            var project = _service.Fetch(fetch).Entities.FirstOrDefault();
            Console.WriteLine("Exporting Project");

            var tempPath = await _service.ExportProjectAsync(project["id"].ToString());
            var projectZipFile = Path.Combine(folder, $"{project["name"]}.zip");
            var projectFolder = Path.Combine(folder, $"{project["name"]}");
            File.Move(tempPath, projectZipFile,true);
            if (Extract)
            {
                if (!Directory.Exists(projectFolder))
                {
                    Directory.CreateDirectory(projectFolder);
                }
                ZipFile.ExtractToDirectory(projectZipFile, projectFolder);
            }
            return 0;

        }


    }
}
