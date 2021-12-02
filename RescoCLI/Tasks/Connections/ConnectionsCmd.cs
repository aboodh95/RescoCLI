using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Resco.Cloud.Client.WebService;
using RescoCLI.Configurations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RescoCLI.Tasks
{

    [Command(Name = "connections", Description = "Manage the connections to Resco cloud server", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(typeof(AddConnectionCmd), typeof(RemoveConnectionCmd))]
    class ConnectionsCmd : RescoCLIBase
    {
        public ConnectionsCmd(ILogger<RescoCLICmd> logger, IConsole console)
        {


        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            var configuration = await Configuration.GetConfigrationAsync();
            for (int i = 0; i < configuration.Connections.Count; i++)
            {
                var item = configuration.Connections[i];
                Console.WriteLine($"{i} URL: {item.URL} User Name: {item.UserName} {(item.IsSelected ? "*" : "")}");
            }
            return 0;
        }

    }

}
