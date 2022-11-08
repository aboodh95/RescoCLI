
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
using System.Collections.Immutable;

namespace RescoCLI.Tasks
{
    [Command(Name = "import-all", Description = "Import all projects in this folder", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    public class ImportAllProjectsCmd : RescoCLIBase
    {
        Resco.Cloud.Client.WebService.DataService _service;

        [Option(CommandOptionType.NoValue, ShortName = "publish", LongName = "publish", Description = "Publish the project", ValueName = "publish", ShowInHelpText = true)]
        public bool Publish { get; set; } = true;
        public ImportAllProjectsCmd(ILogger<RescoCLICmd> logger, IConsole console)
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

            var currentFolder = Environment.CurrentDirectory;
            Spinner spinner = new Spinner();
            spinner.Start();
            var fetch = new Fetch("mobileproject");
            fetch.Entity.AddAttribute("name");
            fetch.Entity.AddAttribute("id");
            fetch.Entity.AddAttribute("resco_appid");
            fetch.Entity.Filter = new Filter();
            var projects = _service.Fetch(fetch).Entities;
            foreach (var item in projects)
            {
                var directoryPath = $"{currentFolder}\\{item["name"]}";
                var zipPath = $"{currentFolder}\\{item["name"]}.zip";
                if (!Directory.Exists(directoryPath))
                {
                    continue;
                }
                Console.WriteLine($"Importing {item["name"]}");

                ZipFile.CreateFromDirectory(directoryPath, zipPath);
                var ProjectId = item["id"].ToString();

                try
                {
                    await _service.ImportProjectAsync(ProjectId, Publish, zipPath);
                }
                catch
                {
                }
                File.Delete(zipPath);

            }
            spinner.Stop();
            return 0;
        }


    }
}
