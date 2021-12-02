
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
    public class OfflineHTMLUpadetorCmd : HARTBBase
    {

        [Option(CommandOptionType.NoValue, ShortName = "d", LongName = "isDevlopment", Description = "Pushing To Development", ValueName = "isDevlopment", ShowInHelpText = true)]
        public bool isDevlopment { get; set; } = true;

        public OfflineHTMLUpadetorCmd(ILogger<HARTBCmd> logger, IConsole console)
        {



            var configuration = Configuration.GetConfigrationAsync().Result;
            var selectedConnections = configuration.Connections.FirstOrDefault(x => x.IsSelected);
            if (selectedConnections == null)
            {
                throw new Exception("No connection do exists");
            }

        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            var configuration =await Configuration.GetConfigrationAsync();
            var selectedConnections = configuration.Connections.FirstOrDefault(x => x.IsSelected);
            await PushFormLibraries(selectedConnections.URL, new NetworkCredential(selectedConnections.UserName, selectedConnections.Password), configuration.OfflineHTMLConfiguration.SelectedProjectId, configuration.OfflineHTMLConfiguration.FolderPath, configuration.OfflineHTMLConfiguration.FolderName, isDevlopment);
            return 0;
        }

        public async Task PushFormLibraries(string url, NetworkCredential networkCredential, string projectId, string folderPath, string folderName, bool isDevlopment)
        {
            var dataService = new Resco.Cloud.Client.WebService.DataService(url)
            {
                Credentials = networkCredential
            };
            var zipFilePath = await dataService.ExportProjectAsync(projectId);
            var zipFile = ZipFile.Open(zipFilePath, ZipArchiveMode.Update);

            var zipFolderPath = zipFilePath.Replace(".zip", "");
            var distPath = Path.Combine(zipFolderPath, "www", folderName);
            zipFile.ExtractToDirectory(zipFolderPath);

            if (Directory.Exists(distPath))
            {
                Directory.Delete(distPath, true);
            }
            Directory.CreateDirectory(distPath);


            var files = Directory.GetFiles(folderPath);
            foreach (var item in files)
            {
                FileInfo fileInfo = new(item);
                File.Copy(item, Path.Combine(distPath, fileInfo.Name));
            }
            var newZipPath = $"{Path.GetTempPath()}\\{Guid.NewGuid()}.zip";

            ZipFile.CreateFromDirectory(zipFolderPath, newZipPath);
            await dataService.ImportProjectAsync(projectId, true, newZipPath);
        }

    }
}
