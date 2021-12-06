using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Resco.Cloud.Client.WebService;
using RescoCLI.Configurations;
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
    [Subcommand(typeof(ConnectionsCmd), typeof(ProjectsCmd), typeof(CodeCmd), typeof(PluginCmd), typeof(OfflineHTMLCmd), typeof(WorkflowLogsCmd))]
    public class RescoCLICmd : RescoCLIBase
    {
      
        public RescoCLICmd(ILogger<RescoCLICmd> logger, IConsole console)
        {

        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return await Task.FromResult(0);
        }

        private static string GetVersion()
            => typeof(RescoCLICmd).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }

}
