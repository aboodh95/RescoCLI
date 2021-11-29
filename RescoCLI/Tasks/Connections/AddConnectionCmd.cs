using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Resco.Cloud.Client.WebService;
using RescoCLI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace RescoCLI.Tasks
{
  

    [Command(Name = "add", Description ="Add new connection", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    class AddConnectionCmd : HARTBBase
    {
    
        [Option(CommandOptionType.SingleValue, ShortName = "u", LongName = "username", Description = "Resco login username", ValueName = "login username", ShowInHelpText = true)]
        public string Username { get; set; }
        [Option(CommandOptionType.SingleValue, ShortName = "url", LongName = "url", Description = "Resco login url", ValueName = "login url", ShowInHelpText = true)]
        public string URL { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "p", LongName = "password", Description = "Resco login password", ValueName = "login password", ShowInHelpText = true)]
        public string Password { get; set; }

        [Option(CommandOptionType.NoValue, ShortName =  "s", LongName = "Selected", Description = "is this connection selected", ValueName = "is selected", ShowInHelpText = true)]
        public bool Selected { get; set; } = false;

        public AddConnectionCmd(ILogger<HARTBCmd> logger, IConsole console)
        {
            _logger = logger;
            _console = console;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                URL = Prompt.GetString("login url:", URL);
                Username = Prompt.GetString("login Username:", Username);
                Password = SecureStringToString(Prompt.GetPasswordAsSecureString("iStrada Password:"));
                Selected = Prompt.GetYesNo("is selected?  ", Selected);
            }

            try
            {
                URL = URL.TrimEnd('/').ToLower();
                Connection connection = new()
                {
                    IsSelected = Selected,
                    URL = URL,
                    Password = Password,
                    UserName = Username
                };
                if (string.IsNullOrEmpty(URL))
                {
                    Console.WriteLine("Missing URL, Adding new Connection require URL");
                    return 0;
                }
            
                if (string.IsNullOrEmpty(Password))
                {
                    Console.WriteLine("Missing Password, Adding new Connection require Password");
                    return 0;
                }
                if (string.IsNullOrEmpty(Username))
                {
                    Console.WriteLine("Missing User Name, Adding new Connection require User Name");
                    return 0;
                }

                var configuration = await Configuration.GetConfigrationAsync();

                if (configuration.Connections.Any(x => x.URL == connection.URL))
                {
                    Console.WriteLine("Connection Already Exist");
                    return 0;
                }
                connection.IsSelected = connection.IsSelected || configuration.Connections.Count == 0;
                if (connection.IsSelected) //Mark others as not selected
                {
                    configuration.Connections.ForEach(x => x.IsSelected = false);
                }
                configuration.Connections.Add(connection);
                await configuration.SaveConfigurationAsync();
                return 0;
            }
            catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }
        }

    }
}
