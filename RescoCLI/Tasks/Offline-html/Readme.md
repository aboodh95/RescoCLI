# Offline HTML

## How it works

The logic behind updating the Offline HTML is to export the project, add the files to the zip file and then import it <br />
In terms of development I'm following the process of writing the code in TS then importing the JS files <br />
I have follow the structure from this blog posts how I setup my project and base on it https://www.oliverflint.co.uk/2020/03/07/D365-Typescript-Webresources-Part-1/ <br/>

## This is my though

This is just the way I'm doing it, feel free to do it in the way you like's it, if you don't want TypeScript or any thing from the hustle above, you just need to set the path to your working directory (JS Files) and then it will be pushed to the offline html directory

## Folder Naming

There is an option in the configuration file, to provide a name for the folder where the Files will be uploaded, so inside the Offline HTML the path of the files will be www\[FolderName]\file.js<br/>
You can make the folder name as XYZ\\JSFiles so this will be considered as sub folder once it's uploaded
