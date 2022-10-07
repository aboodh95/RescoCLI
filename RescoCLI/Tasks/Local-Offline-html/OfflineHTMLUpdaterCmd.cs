
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
using System.Net;

namespace RescoCLI.Tasks
{
    [Command(Name = "update", Description = "Update the project form libraries code from the solution code base", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    public class LocalOfflineHTMLUpdaterCmd : RescoCLIBase
    {

        [Option(CommandOptionType.NoValue, ShortName = "d", LongName = "isDevelopment", Description = "Pushing To Development", ValueName = "isDevelopment", ShowInHelpText = true)]
        public bool isDevelopment { get; set; } = true;

        [Option(CommandOptionType.NoValue, ShortName = "a", LongName = "All", Description = "Update All Projects", ValueName = "All", ShowInHelpText = true)]
        public bool UpdateAll { get; set; } = false;

        [Option(CommandOptionType.SingleValue, ShortName = "i", LongName = "ProjectIndex", Description = "The index of project configuration", ValueName = "ProjectIndex", ShowInHelpText = true)]
        public int projectIndex { get; set; }

        public LocalOfflineHTMLUpdaterCmd(ILogger<RescoCLICmd> logger, IConsole console)
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

            Dictionary<string, string> FolderNameAndPath = new Dictionary<string, string>();
            if (UpdateAll)
            {
                var configurationByProject = configuration.OfflineHTMLConfigurations.GroupBy(x => x.SelectedProjectId);
                foreach (var item in configurationByProject)
                {
                    FolderNameAndPath = item.ToDictionary(x => x.FolderName, x => x.FolderPath);
                    selectedConnections = configuration.Connections.FirstOrDefault(x => x.IsSelected);
                    await PushFiles(selectedConnections.URL, new NetworkCredential(selectedConnections.UserName, selectedConnections.Password), item.Key, FolderNameAndPath, isDevelopment);
                }

            }
            else
            {
                OfflineHTMLConfiguration offlineHTMLConfiguration = configuration.OfflineHTMLConfigurations.ElementAtOrDefault(projectIndex);
                if (offlineHTMLConfiguration == null)
                {
                    throw new Exception($"Configuration with index: {projectIndex} doesn't exists");
                }
                FolderNameAndPath.Add(offlineHTMLConfiguration.FolderName, offlineHTMLConfiguration.FolderPath);
                selectedConnections = configuration.Connections.FirstOrDefault(x => x.IsSelected);
                await PushFiles(selectedConnections.URL, new NetworkCredential(selectedConnections.UserName, selectedConnections.Password), offlineHTMLConfiguration.SelectedProjectId, FolderNameAndPath, isDevelopment);
            }

            return 0;
        }

        public async Task PushFiles(string url, NetworkCredential networkCredential, string projectId, Dictionary<string, string> FolderNameAndPath, bool isDevelopment)
        {
            var configuration = await Configuration.GetConfigrationAsync();
            Spinner spinner = new Spinner();
            spinner.Start();
            var localOfflineHTMLFolder = configuration.LocalOfflineHTMLFolder;
            foreach (var item in FolderNameAndPath)
            {
                var distPath = Path.Combine(localOfflineHTMLFolder, item.Key);
                if (Directory.Exists(distPath))
                {
                    Directory.Delete(distPath, true);
                }
                Directory.CreateDirectory(distPath);
                CopyFoldersAndFiles(item.Value, distPath, isDevelopment);
            }

            spinner.Stop();
        }

        private void CopyFoldersAndFiles(string folderPath, string distPath, bool isDevelopment)
        {
            var files = Directory.GetFiles(folderPath);
            var folders = Directory.GetDirectories(folderPath);
            foreach (var item in files)
            {
                FileInfo fileInfo = new(item);
                if (fileInfo.Extension.ToLower() == ".js")
                {
                    if (isDevelopment)
                    {
                        if (item.EndsWith(".min.js"))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!item.EndsWith(".min.js"))
                        {
                            continue;
                        }
                    }
                }
                File.Copy(item, Path.Combine(distPath, fileInfo.Name));
            }
            foreach (var item in folders)
            {
                var folderName = item.Split('\\').Last();
                var newFolderPath = Path.Combine(distPath, folderName);
                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                }
                CopyFoldersAndFiles(item, newFolderPath, isDevelopment);
            }
        }
    }
}
