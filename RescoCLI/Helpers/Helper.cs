﻿using Resco.Cloud.Client.WebService;
using RestSharp;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RescoCLI
{
    public static class Helper
    {
		public static async Task<string> ExportProjectAsync(this DataService dataService ,string id)
		{
			Console.WriteLine("Attach Debugger");
			Console.Read();
			var ZIP_PATH = $"{Path.GetTempPath()}\\{Guid.NewGuid()}.zip";
			var client = new RestClient($"{dataService.Url}/rest/v1/data/ExportProject?$id={id}")
			{
				Timeout = -1
			};
			var request = new RestRequest(Method.POST);
			var Credentials = dataService.Credentials.GetCredential(new Uri(dataService.Url),"");
			var AuthorizationToken = $"{Credentials.UserName}:{Credentials.Password}";
			request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(AuthorizationToken))}");
			IRestResponse response = client.Execute(request);
			var file = client.DownloadData(request);
			await File.WriteAllBytesAsync(ZIP_PATH, file);
			return ZIP_PATH;

		}

		public static async Task ImportProjectAsync(this DataService dataService, string id,bool publish, string filePath)
		{
			var data = await File.ReadAllBytesAsync(filePath);
			var client = new RestClient($"{dataService.Url}/rest/v1/data/ImportProject?$id={id}&$publish={publish}")
			{
				Timeout = -1
			};
			var request = new RestRequest(Method.POST);
			var Credentials = dataService.Credentials.GetCredential(new Uri(dataService.Url), "");
			var AuthorizationToken = $"{Credentials.UserName}:{Credentials.Password}";
			request.AddHeader("Content-Type", "application/zip");
			request.AddParameter("application/zip", data, ParameterType.RequestBody);
			request.AddHeader("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(AuthorizationToken))}");
			var resporces = await client.ExecuteAsync(request);
		}
	}
}
