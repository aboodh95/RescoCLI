

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

    [Command(Name = "offline-html", Description = "Manage all Offline HTML files", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand( typeof(OfflineHTMLUpdaterCmd))]
    class OfflineHTMLCmd : RescoCLIBase
    {
        Resco.Cloud.Client.WebService.DataService _service;
        public OfflineHTMLCmd(ILogger<RescoCLICmd> logger, IConsole console)
        {



            var configuration = Configuration.GetConfigrationAsync().Result;
            var selectedConnections = configuration.Connections.FirstOrDefault(x => x.IsSelected);
            if (selectedConnections == null)
            {
                throw new Exception("No connection do exists");
            }
            if (configuration.OfflineHTMLConfigurations.Count == 0)
            {
                throw new Exception("Default Project is not selected");
            }
            _service = new Resco.Cloud.Client.WebService.DataService(selectedConnections.URL)
            {
                Credentials = new NetworkCredential(selectedConnections.UserName, selectedConnections.Password)
            };

        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            return await Task.FromResult(0);
        }

    }

}
