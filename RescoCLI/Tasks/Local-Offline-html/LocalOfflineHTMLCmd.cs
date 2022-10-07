

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

    [Command(Name = "local-offline-html", Description = "Manage all Local Offline HTML files", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(typeof(LocalOfflineHTMLUpdaterCmd))]
    class LocalOfflineHTMLCmd : RescoCLIBase
    {
        public LocalOfflineHTMLCmd(ILogger<RescoCLICmd> logger, IConsole console)
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
            if (configuration.OfflineHTMLConfigurations.Count == 0)
            {
                throw new Exception("Default Project is not selected");
            }
            for (int i = 0; i < configuration.OfflineHTMLConfigurations.Count; i++)
            {
                Console.WriteLine($"{i} - {configuration.OfflineHTMLConfigurations[i].FolderName}");
            }

            return await Task.FromResult(0);
        }

    }

}
