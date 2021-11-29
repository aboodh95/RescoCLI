using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Resco.Cloud.Client.WebService;
using RescoCLI.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RescoCLI.Tasks
{

    [Command(Name = "code",Description = "Create classes for entities in C# or TS", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
   [Subcommand(typeof(CSharpGeneratorUtilCmd),typeof(TSGeneratorUtil))]
    class CodeCmd : HARTBBase
    {
        public CodeCmd(ILogger<HARTBCmd> logger, IConsole console)
        {
            _logger = logger;
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            return await Task.FromResult(0);

        }

    }

}
