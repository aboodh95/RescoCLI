using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Resco.Cloud.Client.WebService;
using RescoCLI.Configurations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace RescoCLI.Tasks
{


    [Command(Name = "remove", Description = "Remove connection from the connection list", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    class RemoveConnectionCmd : RescoCLIBase
    {
        [Option(CommandOptionType.SingleValue, ShortName = "i", LongName = "index", Description = "Index of the Connection", ValueName = "0", ShowInHelpText = true)]
        public int? Index { get; set; }
        public RemoveConnectionCmd(ILogger<RescoCLICmd> logger, IConsole console)
        {


        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            await base.OnExecute(app);
            if (Index == null)
            {
                Index = Prompt.GetInt("connection index:", Index);
            }

            try
            {
                var configuration = await Configuration.GetConfigrationAsync();
                if (configuration.Connections.ElementAtOrDefault(Index.Value) == null)
                {
                    Console.WriteLine("Cannot find connection with provided index");
                    return 0;
                }
                configuration.Connections.RemoveAt(Index.Value);
                await configuration.SaveConfigurationAsync();
                return 0;
            }
            catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }
        }

    }
}
