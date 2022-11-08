

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Resco.Cloud.Client.Data.Fetch;
using Resco.Cloud.Client.WebService;
using RescoCLI.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RescoCLI.Tasks
{

    [Command(Name = "projects", Description = "Manage woodford Projects in Resco cloud", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(typeof(ExportProjectCmd), typeof(ImportProjectCmd), typeof(ImportAllProjectsCmd), typeof(ExportAllProjectsCmd))]
    class ProjectsCmd : RescoCLIBase
    {
        Resco.Cloud.Client.WebService.DataService _service;
        public ProjectsCmd(ILogger<RescoCLICmd> logger, IConsole console)
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
            var folder = System.AppDomain.CurrentDomain.BaseDirectory;
            var fetch = new Fetch("mobileproject");
            fetch.Entity.AddAttribute("name");
            fetch.Entity.AddAttribute("id");
            fetch.Entity.AddAttribute("resco_appid");
            fetch.Entity.AddAttribute("resco_parents");
            fetch.Entity.OrderBy("createdon", false);

            var projects = _service.Fetch(fetch).Entities;

            foreach (var item in projects)
            {
                Console.WriteLine($"Id: {item["id"]}");
                Console.WriteLine($"Name: {item["name"]}");
                if (item.HasValue("resco_parents"))
                {
                    Console.WriteLine($"Parent: {item["resco_parents"]}");
                }
                Console.WriteLine("==========");
            }
            return await Task.FromResult(0);
        }

    }

}
