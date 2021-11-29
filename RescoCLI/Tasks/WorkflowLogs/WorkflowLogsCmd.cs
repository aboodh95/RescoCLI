

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Resco.Cloud.Client.Data.Fetch;
using Resco.Cloud.Client.WebService;
using RescoCLI.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace RescoCLI.Tasks
{
   

    [Command(Name = "logs", Description = "Manage the recent workflow logs", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(typeof(OpenWorkflowLogsCmd))]
    class WorkflowLogsCmd : HARTBBase
    {
        Resco.Cloud.Client.WebService.DataService _service;


        [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "count", Description = "The count of the logs to retrieve", ValueName = "Count", ShowInHelpText = true)]
        public int Count { get; set; } = 10;

        public WorkflowLogsCmd(ILogger<HARTBCmd> logger, IConsole console) 
        {
            _logger = logger;
            _console = console;
            var configuration = Configuration.GetConfigrationAsync().Result;
            var selectedConnections = configuration.Connections.FirstOrDefault(x => x.IsSelected);
            if (selectedConnections == null)
            {
                throw new Exception("No connection do exists");
            }
            _service = new Resco.Cloud.Client.WebService.DataService(selectedConnections.URL)
            {
                Credentials = new NetworkCredential(selectedConnections.UserName, selectedConnections.Password)
            };
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            Console.WriteLine("Loading Logs...");
            var fetch = new Fetch("resco_workflowlog");
            fetch.Entity.AddAttribute("startedon");
            fetch.Entity.AddAttribute("id");
            fetch.Entity.AddAttribute("name");
            fetch.Entity.AddAttribute("typename");
            fetch.Entity.AddAttribute("completedon");
            fetch.Entity.AddAttribute("statecode");
            fetch.Entity.AddAttribute("statuscode");
            fetch.Entity.AddAttribute("regardingobjectid");
            fetch.Entity.AddAttribute("messagelog");
            fetch.Entity.AddAttribute("modifiedby");
            fetch.Entity.OrderBy("startedon", true);
            fetch.Count = Count;

           var Logs =  _service.Fetch(fetch).Entities;

            int index = 0;
            DataTable LogsTable = new();
            LogsTable.Columns.Add("Index", typeof(int));
            LogsTable.Columns.Add("Name", typeof(string));
            LogsTable.Columns.Add("Status", typeof(string));
            LogsTable.Columns.Add("StartedOn", typeof(DateTime));
            LogsTable.Columns.Add("CompledtedOn", typeof(DateTime));

            Logs.ForEach(x => LogsTable.Rows.Add(index++, x["name"], x["statuscode"].ToString(),x["startedon"], x["completedon"]));

            LogsTable.Print("Index", "Name", "Status", "StartedOn", "CompledtedOn");
            var folderPath = Path.Combine(Path.GetTempPath(), "RescoCLI Workflow Logs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var jsonString = JsonConvert.SerializeObject(Logs);
            await File.WriteAllTextAsync(Path.Combine(folderPath, $"{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.json"),jsonString);

            return 0;
        }

    }

}

