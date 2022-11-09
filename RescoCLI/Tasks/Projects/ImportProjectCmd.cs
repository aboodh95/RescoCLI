
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
    public class ImportProjectCmd : RescoCLIBase
    {
        Resco.Cloud.Client.WebService.DataService _service;

        [Option(CommandOptionType.SingleValue, ShortName = "pid", LongName = "projectId", Description = "The Id of the project to export", ValueName = "project id", ShowInHelpText = true)]
        public string ProjectId { get; set; }
        [Option(CommandOptionType.SingleValue, ShortName = "pname", LongName = "projectName", Description = "The name of the project to export", ValueName = "project name", ShowInHelpText = true)]
        public string ProjectName { get; set; }
        [Option(CommandOptionType.SingleValue, ShortName = "p", LongName = "path", Description = "The path of the ZIP file or folder", ValueName = "project path", ShowInHelpText = true)]
        public string ProjectPath { get; set; }
        [Option(CommandOptionType.NoValue, ShortName = "publish", LongName = "publish", Description = "Publish the project", ValueName = "publish", ShowInHelpText = true)]
        public bool Publish { get; set; } = false;
        public ImportProjectCmd(ILogger<RescoCLICmd> logger, IConsole console)
        {

        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            await base.OnExecute(app);
            var configuration = await Configuration.GetConfigrationAsync();
            var selectedConnections = configuration.Connections.FirstOrDefault(x => x.IsSelected);
            if (selectedConnections == null)
            {
                throw new Exception("No connection do exists");
            }
            _service = new Resco.Cloud.Client.WebService.DataService(selectedConnections.URL)
            {
                Credentials = new NetworkCredential(selectedConnections.UserName, selectedConnections.Password)
            };


            var fetch = new Fetch("mobileproject");
            fetch.Entity.AddAttribute("name");
            fetch.Entity.AddAttribute("id");
            fetch.Entity.AddAttribute("resco_appid");
            fetch.Entity.Filter = new Filter();
            if (!string.IsNullOrEmpty(ProjectId))
            {
                fetch.Entity.Filter.Where("id", "eq", ProjectId);
            }
            else
            {
                fetch.Entity.Filter.Where("name", "eq", ProjectName);
            }
            var projects = _service.Fetch(fetch).Entities;
            if (projects.Count == 0)
            {
                Console.WriteLine("Cannot find project with provided name");
                return 1;
            }
            if (ProjectPath.EndsWith("\\"))
            {
                ProjectPath = ProjectPath.Remove(ProjectPath.Length - 1);
            }
            string zipPath = "";
            FileAttributes attr = File.GetAttributes(ProjectPath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                 zipPath = $"{ProjectPath}.zip";
                Console.WriteLine($"Importing {projects[0]["name"]}");

                ZipFile.CreateFromDirectory(ProjectPath, zipPath);
                ProjectPath = zipPath;
            }

            await _service.ImportProjectAsync(projects[0]["id"].ToString(), Publish, ProjectPath);
            if (zipPath.Length != 0)
            {
                File.Delete(zipPath);
            }
            return 0;
        }


    }
}
