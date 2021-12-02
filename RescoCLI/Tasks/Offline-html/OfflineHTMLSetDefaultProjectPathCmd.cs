

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Resco.Cloud.Client.Data.Fetch;
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

    [Command(Name = "set-default-project-path", Description = "Set the path of the default project currently working on", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(typeof(ExportProjectCmd), typeof(ImportProjectCmd), typeof(SetDefaultProjectCmd))]
    class OfflineHTMLSetDefaultProjectPathCmd : HARTBBase
    {

        [Option(CommandOptionType.SingleValue, ShortName = "p", LongName = "path", Description = "The path of the default project form libraries", ValueName = "project id", ShowInHelpText = true)]
        public string FolderPath { get; set; }

        public OfflineHTMLSetDefaultProjectPathCmd(ILogger<HARTBCmd> logger, IConsole console)
        {

        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            if (string.IsNullOrEmpty(FolderPath))
            {
                Console.WriteLine("Project Id or Name should be passed");
                return 0;
            }


            var configuration = await Configuration.GetConfigrationAsync();
            FolderPath = FolderPath.TrimStart('.');
            FolderPath = FolderPath.TrimStart('\\');
            configuration.OfflineHTMLConfiguration.FolderPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), FolderPath); ;
            await configuration.SaveConfigurationAsync();

            return 0;
        }

    }

}
