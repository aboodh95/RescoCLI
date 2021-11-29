
using Resco.Cloud.Client.Data.Fetch;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using RescoCLI.Helpers;
using System.Linq;
using System.IO.Compression;

namespace RescoCLI.Tasks
{
    [Command(Name = "set-default",Description ="Set the id of the default woodford project", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    public class SetDefaultProjectCmd : HARTBBase
    {

        [Option(CommandOptionType.SingleValue, ShortName = "pid", LongName = "projectId", Description = "The Id of the project to set as default", ValueName = "project id", ShowInHelpText = true)]
        public string ProjectId { get; set; }

        public SetDefaultProjectCmd(ILogger<HARTBCmd> logger, IConsole console)
        {

            _logger = logger;
            _console = console;

        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            if (string.IsNullOrEmpty(ProjectId))
            {
                Console.WriteLine("Project Id or Name should be passed");
                return 0;
            }
            var configuration = Configuration.GetConfigrationAsync().Result;
            configuration.SelectedProjectId = ProjectId;
           await configuration.SaveConfigurationAsync();

            return 0;
        }

        
    }
}
