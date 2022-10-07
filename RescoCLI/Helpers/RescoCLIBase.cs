﻿using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RescoCLI.Configurations;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace RescoCLI
{

    [HelpOption("--help")]
    public abstract class RescoCLIBase
    {
        [Option(CommandOptionType.SingleValue,ShortName  = "configuration-path", LongName = "configuration-path", Description = "The path to load the RescoCLI.Json", ShowInHelpText = true)]
        public string ConfigurationPath { get; set; }
        public RescoCLIBase()
        {
        }
        protected virtual Task<int> OnExecute(CommandLineApplication app)
        {
            if (!string.IsNullOrEmpty(ConfigurationPath) && ConfigFileExist(ConfigurationPath))
            {
                Configuration.ConfigurationFilePath = ConfigurationPath;
            }
            else
            {
                Configuration.ConfigurationFilePath = Path.Combine(Configuration.ConfigurationFolderPath, "RescoCLI.json");
            }
            return Task.FromResult(0);
        }

        protected String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }


        protected void OnException(Exception ex)
        {
            OutputError(ex.Message);
        }


        protected void OutputError(string message)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static bool ConfigFileExist(string ConfigFile)
        {
            if (!File.Exists(ConfigFile))
            {
                var folder = Environment.CurrentDirectory;
                ConfigFile = Path.Combine(folder, ConfigFile);
                if (!File.Exists(ConfigFile))
                {
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}
