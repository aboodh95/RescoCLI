﻿
using Resco.Cloud.Client.Data.Fetch;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using RescoCLI.Configurations;
using System.Linq;
using System.IO.Compression;
using RestSharp;
using System.Net;
using RescoCLI.Helpers;

namespace RescoCLI.Tasks
{
    [Command(Name = "plugin", Description = "Update the plugin dll from local file", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    public class PluginCmd : RescoCLIBase
    {
        Resco.Cloud.Client.WebService.DataService _service;

        [Option(CommandOptionType.SingleValue, ShortName = "p", LongName = "path", Description = "The path of the dll", ValueName = "plugin path", ShowInHelpText = true)]
        public string DllPath { get; set; }

        public PluginCmd(ILogger<RescoCLICmd> logger, IConsole console)
        {
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {

            await base.OnExecute(app);
            var configuration = await Configuration.GetConfigrationAsync();
            var selectedConnections = configuration.Connections.FirstOrDefault(x => x.IsSelected);
            if (selectedConnections == null)
            {
                throw new Exception("No connection do exists");
            }

            _service = new Resco.Cloud.Client.WebService.DataService(selectedConnections.URL)
            {
                Credentials = new NetworkCredential(selectedConnections.UserName, selectedConnections.Password)
            };
            if (!CheckIfDLLExist())
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cannot find DLL file with the provided path");
                Console.ForegroundColor = color;
                return 1;
            }

            FileInfo fileInfo = new(DllPath);
            var folder = Environment.CurrentDirectory;
            var fetch = new Fetch("resco_pluginassembly");
            fetch.Entity.AddAttribute("name");
            fetch.Entity.AddAttribute("description");
            fetch.Entity.AddAttribute("id");
            var plugin = _service.Fetch(fetch).Entities.FirstOrDefault(x => x["name"].ToString() == fileInfo.Name.Replace(".dll", ""));
            if (plugin == null)
            {
                throw new Exception("Cannot find plugin in Resco Server, Please create it manually first in order to update it with the tool");
            }
            Console.WriteLine($"Updating Plug-in {plugin["name"].ToString()}");
            var client = new RestClient($"{selectedConnections.URL}/rest/v1/data/RegisterPlugin");
            var request = new RestRequest("", Method.Post);
            var AuthorizationToken = $"{selectedConnections.UserName}:{selectedConnections.Password}";
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(AuthorizationToken))}");
            var bytes = await File.ReadAllBytesAsync(DllPath);
            var data = new PluginRegistrationRequest
            {
                Name = plugin["name"].ToString(),
                Id = new Guid(plugin["id"].ToString()),
                Description = plugin["description"].ToString(),
                PluginData = Convert.ToBase64String(bytes),
            };
            request.AddHeader("Content-Type", "application/xml");
            request.AddParameter("application/xml", data.ToXML(), ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            return 0;
        }
        private bool CheckIfDLLExist()
        {
            DllPath ??= "";
            if (!File.Exists(DllPath))
            {
                var folder = Environment.CurrentDirectory;
                DllPath = Path.Combine(folder, DllPath);
                if (!File.Exists(DllPath))
                {
                    return false;
                }
                return true;
            }
            return true;
        }


    }
}
