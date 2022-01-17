# Configuration File

This file is created to host the configuration for running the CLI and to not repeat some of the parameters

```
{
    //Connection List
    "Connections": [
        {
            //The URL of your org
            "URL": "https://rescocrm.com",
            //Resco User Name
            "UserName": "YourUserName",
            //Resco Password
            "Password": "YourPassword",
            //Is this connection selected to be used by default
            "IsSelected": true
        }
    ],
    //Code Generation Configuration
    "CodeGenerationConfiguration": {
        //The path where the cs Classes will be created
        "CSharpEntitiesFolderPath": "FolderPath",
        //The path where the ts Classes will be created
        "TSEntitiesFolderPath": "FolderPath"
    },
    //Offline HTML Configurations
    //Array of configuration that you can select to update the you can select to push the changes
    "OfflineHTMLConfigurations": [
        {
        //The path where your JS & HTML files are stored
        "FolderPath": "FolderPath",
        //The selected project id where to push these files to it
        "SelectedProjectId": "df4c0729-XXXX-45c6-XXXX-4fff1b44bcee",
        //The name of the folder that will be created inside the Offline HTML zip file (Read more about it in Offline HTML Readme)
        "FolderName": "FormLibraries"
    }
    ]
}
```

By default the location of the RescoCLI JSON file will be in AppData folder, you can override these settings from the AppSettings.json file where you have build the solution (Or downloaded the files)
