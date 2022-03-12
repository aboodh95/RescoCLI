# RescoCLI

CLI for Resco Cloud Platform

## Description

This project is created to help developer working on Resco Cloud Platform<br/>
It do provide set of commands that can accelerate the process of development

## Getting Started

So far, the tool has these set of commands <br />

### Commands

| Command      | Sub-Commands             | Description                                                                                                                      |
| ------------ | ------------------------ | -------------------------------------------------------------------------------------------------------------------------------- |
| Connections  |                          | Manage the Connections to Resco Orgs                                                                                             |
|              | add                      | Add connection to the connection list                                                                                            |
|              | remove                   | Remove connection from the connection list                                                                                       |
| Code         |                          | Create classes as per your Entities                                                                                              |
|              | c#                       | Create C# Classes                                                                                                                |
|              | ts                       | Create TypeScript Classes                                                                                                        |
| Plug-ins     |                          | Update plugin assembly in Resco Cloud                                                                                            |
| Projects     |                          | Manage the Woodford Projects                                                                                                     |
|              | Export                   | Export Woodford Project                                                                                                          |
|              | Export-All               | Export All Woodford Projects                                                                                                     |
|              | Import                   | Import Woodford Project                                                                                                          |
| logs         |                          | Display list of recent logs (default 10)                                                                                         |
|              | open                     | Open the logs as per the last loading of the logs                                                                                |
| Offline-html |                          | Manage the Offline HTML File                                                                                                     |
|              | update                   | Push the offline html files to the selected woodford project by index(Please read [Offline-HTML](RescoCLI/Tasks/Offline-html/Readme.md)) |


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
- 1.1.0.0  - Adding the support for entities creation as classes for C# and TypeScript

## License

This project is licensed under the MIT License - see the LICENSE.md file for details
