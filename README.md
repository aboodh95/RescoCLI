# RescoCLI

CLI for Resco Cloud Platform

## Description

This project is created to help developer working on Resco Cloud Platform<br/>
It do provide set of commands that can accelerate the process of development

## Getting Started

So far, the tool has these set of commands <br />

### Commands

| Command      | Sub-Commands        | Description                                                                                                                      |
| ------------ | ------------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| Connections  |                     | Manage the Connections to Resco Orgs                                                                                             |
|              | add                 | Add connection to the connection list                                                                                            |
|              | remove              | Remove connection from the connection list                                                                                       |
| Plug-ins     |                     | Update plugin assembly in Resco Cloud                                                                                            |
| Projects     |                     | Manage the Woodford Projects                                                                                                     |
|              | Export              | Export Woodford Project                                                                                                          |
|              | Import              | Import Woodford Project                                                                                                          |
|              | Set-Default-Project | Set the default project you are currently working on (For the Offline-HTML Command)                                              |
| logs         |                     | Display list of recent logs (default 10)                                                                                         |
|              | open                | Open the logs as per the last loading of the logs                                                                                |
| Offline-html |                     | Manage the Offline HTML File                                                                                                     |
|              | update              | Push the offline html files to the selected woodford project (Please read [Offline-HTML](RescoCLI/Tasks/Offline-html/Readme.md)) |

|
| | set-default-project-path | Set the path of the folder that have all the offline-html you are working on |

### Executing program

after you build the app set the path in Environment Variable, then you can call it

```
rc connections
rc projects
rc logs
```

## Authors

[@AboodAlhamwi](https://twitter.com/Aboodalhamwi1) - HA Consultancy

## Version History

- 1.0.0.0  - Initial Release

## License

This project is licensed under the MIT License - see the LICENSE.md file for details
