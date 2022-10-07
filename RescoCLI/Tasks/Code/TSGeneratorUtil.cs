using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Resco.Cloud.Client.Metadata;
using System.CodeDom;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;
using Resco.Cloud.Client.Data.Fetch;
using Resco.Cloud.Client;
using Resco.Cloud.Client.Data;
using System.Collections.ObjectModel;
using System.Net;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using RescoCLI.Configurations;
using System.Threading.Tasks;
using RescoCLI.Helpers;
using System.Xml.Serialization;
using RestSharp;

namespace RescoCLI.Tasks
{
    [Command(Name = "ts", Description = "Create classes from entities of the selected org", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    public class TSGeneratorUtil : RescoCLIBase
    {
        [Option(CommandOptionType.SingleValue, ShortName = "f", LongName = "folderPath", Description = "The path of the folder to extract entities to it, default on in configuration", ValueName = "folder path", ShowInHelpText = true)]
        public string FolderPath { get; set; }
        public TSGeneratorUtil(ILogger<RescoCLICmd> logger, IConsole console)
        {
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            await base.OnExecute(app);
            Configuration configuration = await Configuration.GetConfigrationAsync();
            var selectedConnections = configuration.Connections.FirstOrDefault(x => x.IsSelected);
            if (selectedConnections == null)
            {
                throw new Exception("No connection do exists");
            }
            if (string.IsNullOrEmpty(FolderPath))
            {
                FolderPath = configuration.CodeGenerationConfiguration.TSEntitiesFolderPath;
            }
            if (!Directory.Exists(FolderPath))
            {
                throw new DirectoryNotFoundException(FolderPath);
            }
            var connection = configuration.Connections.FirstOrDefault(x => x.IsSelected);
            Generat(FolderPath, connection.URL, new NetworkCredential(connection.UserName, connection.Password));

            return 0;
        }


        public static void Generat(string folderName, string url, NetworkCredential credentials)
        {
            var optionSets = new List<string>();
            var dataService = new Resco.Cloud.Client.WebService.DataService(url);
            dataService.Credentials = credentials;

            var metadataService = new Resco.Cloud.Client.WebService.MetadataService(url);
            metadataService.Credentials = credentials;
            // retrieve all entities
            var entities = new List<MetadataEntity>();
            entities.AddRange(metadataService.RetrieveEntities());

            var localizations = GetLocalization(dataService);
            var MetadataTypesFile = @"export class MetadataTypes {
                                        public static Types = {}
                                    }";
            foreach (var entity in entities)
            {
                var displayName = entity.Name;
                var classString = $@"import {{Entity, EntityReference }} from ""../Types"";
                                     import {{ MetadataTypes }} from ""./MetadataTypes"";
                export class {displayName} extends Entity
						{{
                             constructor() {{
                                  super('{entity.Name}');
                                }}
							public static fields =
							{{
							";
                foreach (var attribute in entity.Attributes)
                {
                    classString += $"{attribute.Name} : \"{attribute.Name}\",\n";
                }
                classString += "}\n";
                classString += $@"
		        public static entityLogicalName = ""{entity.Name}"";
		        primaryIdAttribute = ""{entity.PrimaryKeyName}"";
		        primaryNameAttribute = ""{entity.PrimaryFieldName}"";";
                HashSet<string> Imports = new HashSet<string>();
                foreach (var attribute in entity.Attributes)
                {
                    if (attribute.Name == "id")
                    {
                        continue;
                    }

                    switch (attribute.Type)
                    {
                        case XrmType.UniqueIdentifier:
                        case XrmType.String:
                            classString += $@"public get {attribute.Name}(): string {{
														return this.getAttributeValue<string>(""{attribute.Name}"");
											  }}
                                              public set {attribute.Name}(v: string) {{
														this.attributes[""{attribute.Name}""] = v;
											  }}
												";
                            break;
                        case XrmType.Float:
                        case XrmType.Money:
                        case XrmType.Decimal:
                        case XrmType.RowVersion:
                        case XrmType.Integer:
                        case XrmType.BigInt:
                            classString += $@"public get {attribute.Name}(): number {{
														return this.getAttributeValue<number>(""{attribute.Name}"");
											  }}
                                              public set {attribute.Name}(v: number) {{
														this.attributes[""{attribute.Name}""] = v;
											  }}
												";
                            break;
                        case XrmType.DateTime:
                            classString += $@"public get {attribute.Name}(): Date {{
														return this.getAttributeValue<Date>(""{attribute.Name}"");
											  }}
                                              public set {attribute.Name}(v: Date) {{
														this.attributes[""{attribute.Name}""] = v;
											  }}
												";
                            break;
                        case XrmType.Boolean:
                        case XrmType.Binary:
                            classString += $@"public get {attribute.Name}(): boolean {{
														return this.getAttributeValue<boolean>(""{attribute.Name}"");
											  }}
                                              public set {attribute.Name}(v: boolean) {{
														this.attributes[""{attribute.Name}""] = v;
											  }}
												";
                            break;

                        case XrmType.Picklist:
                            optionSets.Add($"{entity.Name}.{attribute.Name}");
                            Imports.Add($"{entity.Name}_{attribute.Name}");
                            classString += $@"public get {attribute.Name}(): {entity.Name}_{attribute.Name} {{
														return this.getAttributeValue<{entity.Name}_{attribute.Name}>(""{attribute.Name}"");
											  }}
                                              public set {attribute.Name}(v: {entity.Name}_{attribute.Name}) {{
														this.attributes[""{attribute.Name}""] = v;
											  }}
												";
                            break;
                        case XrmType.PicklistMap:
                            optionSets.Add($"{entity.Name}.{attribute.Name}");
                            Imports.Add($"{entity.Name}_{attribute.Name}");
                            classString += $@"public get {attribute.Name}(): {entity.Name}_{attribute.Name} {{
														return this.getAttributeValue<{entity.Name}_{attribute.Name}>(""{attribute.Name}"");
											  }}
                                              public set {attribute.Name}(v: {entity.Name}_{attribute.Name}) {{
														this.attributes[""{attribute.Name}""] = v;
											  }}
												";
                            break;

                        case XrmType.Lookup:
                        case XrmType.PartyList:
                            classString += $@"public get {attribute.Name}(): EntityReference {{
														return this.getAttributeValue<EntityReference>(""{attribute.Name}"");
											  }}
                                              public set {attribute.Name}(v: EntityReference) {{
														this.attributes[""{attribute.Name}""] = v;
											  }}
												";
                            break;

                        default:
                            break;
                    }
                }
                classString += @"}";
                classString += $@"
                MetadataTypes.Types['{entity.Name}'] = new {entity.Name}();
";
                if (Imports.Count != 0)
                {
                    classString = $@"import {{{ string.Join(", ", Imports)}}} from ""./OptionSets"";
                                    {classString}";
                }
                File.WriteAllText(Path.Combine(folderName, $"{displayName}.ts"), classString);
            }
            var optionSetFile = @"";
            foreach (var item in optionSets)
            {
                var options = localizations.DisplayName.Where(x => x.Name.StartsWith($"{item}.")).Distinct().ToList();
                optionSetFile += $@"export enum {item.Replace(".", "_")}{{";
                var addedValues = new List<string>();
                foreach (var option in options)
                {
                    var value = ClearDisplayName(option.Text ?? "");
                    while (addedValues.Any(x => x == value))
                    {
                        value = $"_{value}";
                    }
                    addedValues.Add(value);
                    optionSetFile += $"{value} = {option.Name.Split('.')[2]},\n";
                }
                optionSetFile += "}\n";

            }
            optionSetFile += "\n";
            File.WriteAllText(Path.Combine(folderName, "MetadataTypes.ts"), MetadataTypesFile);
            File.WriteAllText(Path.Combine(folderName, "OptionSets.ts"), optionSetFile);
        }

        public static string AddSummary(string summary)
        {
            if (!string.IsNullOrEmpty(summary))
            {
                return $@"/// <summary>
		/// {summary}
		/// </summary>";
            }
            return "";
        }
        private static LocalizationResult GetLocalization(Resco.Cloud.Client.WebService.DataService dataService)
        {
            var cred = dataService.Credentials.GetCredential(new Uri(dataService.Url), "");
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{cred.UserName}:{cred.Password}");

            var client = new RestClient($"{dataService.Url}/rest/v1/metadata/$localizations?lcid=1033");
            var request = new RestRequest("", Method.Get);
            request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(plainTextBytes)}");
            var body = @"";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            XmlSerializer serializer = new XmlSerializer(typeof(LocalizationResult));
            using (StringReader reader = new StringReader(response.Content))
            {
                var localizationResult = (LocalizationResult)serializer.Deserialize(reader);
                return localizationResult;
            }

        }

        private static string ClearDisplayName(string fieldName)
        {
            return fieldName
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(".", "")
                .Replace("'", "")
                .Replace("/", "")
                .Replace("&", "_")
                .Replace("\\", "");
        }
    }
}
