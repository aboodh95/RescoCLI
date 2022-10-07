

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Resco.Cloud.Client;
using Resco.Cloud.Client.Data.Fetch;
using Resco.Cloud.Client.Metadata;
using RescoCLI.Configurations;
using RescoCLI.Helpers;
using RestSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RescoCLI.Tasks
{
    [Command(Name = "c#", Description = "Create classes from entities of the selected org", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    public class CSharpGeneratorUtilCmd : RescoCLIBase
    {
        [Option(CommandOptionType.SingleValue, ShortName = "f", LongName = "folderPath", Description = "The path of the folder to extract entities to it, default on in configuration", ValueName = "folder path", ShowInHelpText = true)]
        public string FolderPath { get; set; }
        [Option(CommandOptionType.SingleValue, ShortName = "n", LongName = "namespace", Description = "Code Files name space", ValueName = "Resco", ShowInHelpText = true)]
        public string Namespace { get; set; }

        public CSharpGeneratorUtilCmd(ILogger<RescoCLICmd> logger, IConsole console)
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
            Namespace = string.IsNullOrEmpty(Namespace) ? "Resco" : Namespace;
            if (string.IsNullOrEmpty(FolderPath))
            {
                FolderPath = configuration.CodeGenerationConfiguration.CSharpEntitiesFolderPath ?? "";
            }
            if (!Directory.Exists(FolderPath))
            {
                throw new DirectoryNotFoundException(FolderPath);
            }
            var connection = configuration.Connections.FirstOrDefault(x => x.IsSelected);
            Generat(FolderPath, connection.URL, new NetworkCredential(connection.UserName, connection.Password), Namespace);
            return 0;
        }



        public void Generat(string folderName, string url, NetworkCredential credentials, string _namespace)
        {
            var optionSets = new List<string>();
            var dataService = new Resco.Cloud.Client.WebService.DataService(url);
            dataService.Credentials = credentials;

            var metadataService = new Resco.Cloud.Client.WebService.MetadataService(url);
            metadataService.Credentials = credentials;
            var entities = new List<MetadataEntity>();
            entities.AddRange(metadataService.RetrieveEntities());

            var localizations = GetLocalization(dataService);
            Console.WriteLine($"Total Entities: {entities.Count}");
            foreach (var entity in entities)
            {
                string displayName = entity.Name;
                CodeCompileUnit targetUnit = new CodeCompileUnit();
                CodeNamespace codeNamespace = new CodeNamespace(_namespace);
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("XRMServer.Data"));
                CodeTypeDeclaration targetClass = new(displayName)
                {
                    IsClass = true,
                    TypeAttributes = TypeAttributes.Public
                };
                targetClass.BaseTypes.Add("BaseEntity");

                CodeMemberField EntityLogicalNameField = new CodeMemberField
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    Name = "EntityLogicalName",
                    InitExpression = new CodePrimitiveExpression(entity.Name),
                    Type = new CodeTypeReference(typeof(String))
                };
                targetClass.Members.Add(EntityLogicalNameField);

                CodeMemberField PrimaryNameAttributeField = new CodeMemberField
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    Name = "PrimaryNameAttribute",
                    InitExpression = new CodePrimitiveExpression(entity.PrimaryFieldName),
                    Type = new CodeTypeReference(typeof(String))
                };
                targetClass.Members.Add(PrimaryNameAttributeField);

                CodeMemberField PrimaryIdAttributeField = new CodeMemberField
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    Name = "PrimaryIdAttribute",
                    InitExpression = new CodePrimitiveExpression(entity.PrimaryKeyName),
                    Type = new CodeTypeReference(typeof(String))
                };
                targetClass.Members.Add(PrimaryIdAttributeField);

                var fieldClass = new CodeTypeDeclaration("Fields");
                fieldClass.IsClass = true;
                fieldClass.TypeAttributes = TypeAttributes.Public;

                foreach (var attribute in entity.Attributes)
                {
                    CodeMemberField field = new CodeMemberField
                    {
                        Attributes = MemberAttributes.Public | MemberAttributes.Const,
                        Name = attribute.Name,
                        InitExpression = new CodePrimitiveExpression(attribute.Name),
                        Type = new CodeTypeReference(typeof(String))
                    };
                    fieldClass.Members.Add(field);
                }
                targetClass.Members.Add(fieldClass);

                CodeConstructor constructor = new CodeConstructor();
                constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("EntityLogicalName"));
                targetClass.Members.Add(constructor);

                foreach (var attribute in entity.Attributes)
                {
                    if (attribute.Name == "id")
                    {
                        continue;
                    }
                    CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
                    codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    codeMemberProperty.Name = attribute.Name;
                    codeMemberProperty.HasGet = true;
                    codeMemberProperty.HasSet = true;
                    codeMemberProperty.Comments.Add(new CodeCommentStatement(AddSummary(attribute.Description), true));

                    switch (attribute.Type)
                    {
                        case XrmType.UniqueIdentifier:
                            codeMemberProperty.Type = new CodeTypeReference(typeof(Guid?));
                            codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"GetPropertyValue<Guid?>(\"{attribute.Name }\")")));
                            codeMemberProperty.SetStatements.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"Add(\"{attribute.Name }\", value)"));
                            break;
                        case XrmType.String:
                        case XrmType.Binary:
                            codeMemberProperty.Type = new CodeTypeReference(typeof(string));
                            codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"GetPropertyValue<string>(\"{attribute.Name }\")")));
                            codeMemberProperty.SetStatements.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"Add(\"{attribute.Name }\", value)"));
                            break;
                        case XrmType.Float:
                            codeMemberProperty.Type = new CodeTypeReference(typeof(double?));
                            codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"GetPropertyValue<System.Nullable<double>>(\"{attribute.Name }\")")));
                            codeMemberProperty.SetStatements.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"Add(\"{attribute.Name }\", value)"));
                            break;
                        case XrmType.Money:
                        case XrmType.Decimal:
                            codeMemberProperty.Type = new CodeTypeReference(typeof(decimal?));
                            codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"GetPropertyValue<System.Nullable<decimal>>(\"{attribute.Name }\")")));
                            codeMemberProperty.SetStatements.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"Add(\"{attribute.Name }\", value)"));
                            break;
                        case XrmType.DateTime:
                            codeMemberProperty.Type = new CodeTypeReference(typeof(DateTime?));
                            codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"GetPropertyValue<System.Nullable<DateTime>>(\"{attribute.Name }\")")));
                            codeMemberProperty.SetStatements.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"Add(\"{attribute.Name }\", value)"));
                            break;
                        case XrmType.Picklist:
                        case XrmType.PicklistMap:
                            optionSets.Add($"{entity.Name}.{attribute.Name}");
                            codeMemberProperty.Type = new CodeTypeReference($"{entity.Name}_{attribute.Name}?");
                            codeMemberProperty.GetStatements.Add(new CodeVariableDeclarationStatement(typeof(Nullable<int>), "value", new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"GetPropertyValue<Nullable<int>>(\"{attribute.Name }\")")));
                            CodeConditionStatement checkIfNull = new CodeConditionStatement();
                            checkIfNull.Condition = new CodeSnippetExpression("value != null");
                            checkIfNull.TrueStatements.Add(new CodeSnippetExpression($"return ({entity.Name}_{attribute.Name})value"));
                            checkIfNull.FalseStatements.Add(new CodeSnippetExpression("return null"));
                            codeMemberProperty.GetStatements.Add(checkIfNull);
                            codeMemberProperty.SetStatements.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"Add(\"{attribute.Name }\", (int)value)"));
                            break;
                        case XrmType.Boolean:
                            codeMemberProperty.Type = new CodeTypeReference(typeof(bool?));
                            codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"GetPropertyValue<System.Nullable<bool>>(\"{attribute.Name }\")")));
                            codeMemberProperty.SetStatements.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"Add(\"{attribute.Name }\", value)"));
                            break;
                        case XrmType.Lookup:
                        case XrmType.PartyList:
                            codeMemberProperty.Type = new CodeTypeReference("IEntityReference");
                            codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"GetPropertyValue<IEntityReference>(\"{attribute.Name }\")")));

                            codeMemberProperty.SetStatements.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"Add(\"{attribute.Name }\", value)"));
                            break;
                        case XrmType.Integer:
                            codeMemberProperty.Type = new CodeTypeReference(typeof(int?));
                            codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"GetPropertyValue<System.Nullable<int>>(\"{attribute.Name }\")")));
                            codeMemberProperty.SetStatements.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"Add(\"{attribute.Name }\", value)"));
                            break;
                        case XrmType.BigInt:
                        case XrmType.RowVersion:
                            codeMemberProperty.Type = new CodeTypeReference(typeof(long?));
                            codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"GetPropertyValue<System.Nullable<long>>(\"{attribute.Name }\")")));
                            codeMemberProperty.SetStatements.Add(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"Add(\"{attribute.Name }\", value)"));
                            break;
                        default:
                            break;
                    }
                    targetClass.Members.Add(codeMemberProperty);

                }

                codeNamespace.Types.Add(targetClass);
                targetUnit.Namespaces.Add(codeNamespace);
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";
                var fileName = Path.Combine(folderName, $"{displayName}.cs");
                using (StreamWriter sourceWriter = new StreamWriter(fileName))
                {
                    provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
                }
                var classString1 = File.ReadAllText(fileName);
                classString1 = classString1.Replace("public class Fields", "public static class Fields");
                File.WriteAllText(fileName, classString1);
            }
            var optionSetUnit = new CodeCompileUnit();
            CodeNamespace optionSetCodeNamespace = new CodeNamespace(_namespace);
            optionSetCodeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            foreach (var item in optionSets)
            {
                var targetClass = new CodeTypeDeclaration(item.Replace(".", "_"));
                targetClass.IsEnum = true;
                targetClass.TypeAttributes = TypeAttributes.Public;
                var options = localizations.DisplayName.Where(x => x.Name.StartsWith($"{item}.")).ToList();
                var addedValues = new List<string>();
                foreach (var option in options)
                {
                    var value = ClearDisplayName(option.Text ?? "");
                    while (addedValues.Any(x => x == value))
                    {
                        value = $"_{value}";
                    }
                    addedValues.Add(value);
                    CodeMemberField field = new ()
                    {
                        Name = value,
                        InitExpression = new CodePrimitiveExpression(Convert.ToInt32(option.Name.Split('.')[2])),
                    };
                    targetClass.Members.Add(field);
                }
                optionSetCodeNamespace.Types.Add(targetClass);
            }
            optionSetUnit.Namespaces.Add(optionSetCodeNamespace);

            CodeDomProvider providerOptionSet = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions optionsOptionSet = new CodeGeneratorOptions();
            optionsOptionSet.BracingStyle = "C";
            var fileName1 = Path.Combine(folderName, $"OptionSet.cs");
            using (StreamWriter sourceWriter = new StreamWriter(fileName1))
            {
                providerOptionSet.GenerateCodeFromCompileUnit(optionSetUnit, sourceWriter, optionsOptionSet);
            }
            // File.WriteAllText(Path.Combine(folderName, $"OptionSet.cs"), optionSetFile);


        }

        public static string AddSummary(string summary)
        {
            if (!string.IsNullOrEmpty(summary))
            {
                return $@"<summary>
{summary}
</summary>";
            }
            return "";
        }
        private static LocalizationResult GetLocalization(Resco.Cloud.Client.WebService.DataService dataService)
        {
            var cred = dataService.Credentials.GetCredential(new Uri(dataService.Url), "");
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{cred.UserName}:{cred.Password}");
            
            var client = new RestClient($"{dataService.Url}/rest/v1/metadata/$localizations?lcid=1033");
            var request = new RestRequest("",Method.Get);
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
            fieldName = fieldName
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("'", "")
                .Replace("/", "")
                .Replace("&", "")
                .Replace(":", "")
                .Replace("%", "")
                .Replace("\\", "");
            if (fieldName.Length == 0)
            {
                return "_";
            }
            char stringFirstCharacter = fieldName.ToCharArray().ElementAt(0);
            if (char.IsNumber(stringFirstCharacter))
            {
                fieldName = $"_{fieldName}";
            }

            return fieldName;
        }
    }
}
