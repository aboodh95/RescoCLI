

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Resco.Cloud.Client.Data.Fetch;
using Resco.Cloud.Client.WebService;
using RescoCLI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RescoCLI.Tasks
{

    [Command(Name = "set-default-project-path",Description = "Set the path of the default project currently working on", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
   [Subcommand(typeof(ExportProjectCmd),typeof(ImportProjectCmd),typeof(SetDefaultProjectCmd))]
    class OfflineHTML_SetDefaultProjectPathCmd : HARTBBase
    {

        [Option(CommandOptionType.SingleValue, ShortName = "p", LongName = "path", Description = "The path of the default project form libraries", ValueName = "project id", ShowInHelpText = true)]
        public string FolderPath { get; set; }

        public OfflineHTML_SetDefaultProjectPathCmd(ILogger<HARTBCmd> logger, IConsole console)
        {

            _logger = logger;
            _console = console;

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
            configuration.ActiveProjectFormLibrariesPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), FolderPath); ;
            await configuration.SaveConfigurationAsync();

            return 0;
        }

    }

}
