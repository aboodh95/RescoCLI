namespace RescoCLI.Configurations
{
    public class OfflineHTMLConfiguration
    {
        /// <summary>
        /// Active Project Form Libraries Path
        /// </summary>
        public string FolderPath { get; set; }
        /// <summary>
        /// The selected project id where to push these files to it
        /// </summary>
        public string SelectedProjectId { get; set; }
        /// <summary>
        /// The name of the folder that will be created inside the Offline HTML zip file (Read more about it in Offline HTML Readme)
        /// </summary>
        public string FolderName { get; set; }
    }
}