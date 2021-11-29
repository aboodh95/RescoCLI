using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Resco.Cloud.Client.WebService;
using RescoCLI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RescoCLI.Tasks
{

    [Command(Name = "rc", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    [Subcommand(typeof(ConnectionsCmd),  typeof(ProjectsCmd), typeof(CodeCmd),typeof(PluginCmd),typeof(OfflineHTMLCmd),typeof(WorkflowLogsCmd))]
    public class HARTBCmd : HARTBBase
    {
        [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "ConfigPath", Description = "Override the Config File Path (It will be updated in the app settings)", ValueName = "Config Path", ShowInHelpText = true)]
        [FileExists]
        public string ConfigFile { get; set; }

        public HARTBCmd(ILogger<HARTBCmd> logger, IConsole console)
        {
            _logger = logger;
            _console = console;
          
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return await Task.FromResult(0);

        }

        private static string GetVersion()
            => typeof(HARTBCmd).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }

}
