
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
    [Command(Name = "export-all", Description = "Export Woodford Project from Resco cloud", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    public class ExportAllProjectsCmd : RescoCLIBase
    {
        Resco.Cloud.Client.WebService.DataService _service;

        public ExportAllProjectsCmd(ILogger<RescoCLICmd> logger, IConsole console)
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
            Spinner spinner = new Spinner();
            spinner.Start();
            var folder = Environment.CurrentDirectory;
            var fetch = new Fetch("mobileproject");
            fetch.Entity.AddAttribute("name");
            fetch.Entity.AddAttribute("id");
            fetch.Entity.AddAttribute("resco_appid");
            fetch.Entity.Filter = new Filter();
            var projects = _service.Fetch(fetch).Entities;
            foreach (var project in projects)
            {
                Console.WriteLine($"Exporting Projects: {project["name"]}");
                var projectZipFile = await _service.ExportProjectAsync(project["id"].ToString());
                var projectFolder = Path.Combine(folder, $"{project["name"]}");
                if (!Directory.Exists(projectFolder))
                {
                    Directory.CreateDirectory(projectFolder);
                }
                ZipFile.ExtractToDirectory(projectZipFile, projectFolder, true);
            }
            spinner.Stop(); 
            return 0;
        }
    }
}
