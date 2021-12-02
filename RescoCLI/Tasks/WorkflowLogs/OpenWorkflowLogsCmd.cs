

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Resco.Cloud.Client.Data;
using Resco.Cloud.Client.Data.Fetch;
using Resco.Cloud.Client.WebService;
using RescoCLI.Configurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace RescoCLI.Tasks
{


    [Command(Name = "open", Description = "Open the logs as per the last loading of the logs", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    class OpenWorkflowLogsCmd : RescoCLIBase
    {


        [Option(CommandOptionType.SingleValue, ShortName = "i", LongName = "index", Description = "The index of the logs to open", ValueName = "Index", ShowInHelpText = true)]
        public int Index { get; set; }

        public OpenWorkflowLogsCmd(ILogger<RescoCLICmd> logger, IConsole console)
        {


        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            var folderPath = Path.Combine(Path.GetTempPath(), "RescoCLI Workflow Logs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var file = Directory.GetFiles(folderPath).OrderByDescending(x => new FileInfo(x).CreationTime).FirstOrDefault();
            if (file == null)
            {
                Console.WriteLine("No logs was founded, please run the 'rc logs' command first");
                return 0;
            }
            var Logs = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(await File.ReadAllTextAsync(file));
            var log = Logs.ElementAtOrDefault(Index);
            if (log != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Name: ");
                Console.ResetColor();
                Console.WriteLine($"{log["name"]}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Status: ");
                Console.ResetColor();

                Console.WriteLine($"{log["statuscode"]}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Started On: ");
                Console.ResetColor();
                Console.WriteLine($"{log["startedon"]}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Completed On: ");
                Console.ResetColor();
                Console.WriteLine($"{log["completedon"]}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Regarding: ");
                Console.ResetColor();
                Console.WriteLine($"{log["regardingobjectid"]}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Message: ");
                Console.ResetColor();
                Console.WriteLine($"{log["messagelog"]}");
            }


            return 0;
        }

    }

}

