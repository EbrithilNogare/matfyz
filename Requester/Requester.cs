﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Web;

[assembly: InternalsVisibleTo("Requester.Tests")]
namespace Requester
{
	public class Requester
	{
		private readonly HttpClient client;

		public Requester()
		{
			client = new HttpClient();
		}

		public async Task<RequestResponse> Send(string method, string url, string body, Dictionary<string, string> headerParams = null)
		{
			// URL
			Uri uri = new UriBuilder(url).Uri;
			
			// Content
			StringContent requestBody = new StringContent(body);

			// Headers
			if (headerParams != null)
				SetHeaders(headerParams, requestBody);

			// Send request
			HttpResponseMessage response;
			var stopWatch = Stopwatch.StartNew();
			switch (method.ToUpper())
			{
				case "POST":
					response = await client.PostAsync(uri, requestBody);
					break;
				case "GET":
					response = await client.GetAsync(uri);
					break;
				default:
					throw new Exception("Invalid Method");
			}
			var timeSpan = stopWatch.Elapsed;

			// Parse response
			RequestResponse output = new RequestResponse();
			output.content = await response.Content.ReadAsStringAsync();
			output.header = response.Headers.ToString();
			output.statusCode = new KeyValuePair<int, string>((int)response.StatusCode, response.StatusCode.ToString());
			output.timing = (int)timeSpan.TotalMilliseconds;

			return output;
		}

		public void SetHeaders(Dictionary<string, string> headerParams, StringContent content)
		{
			foreach (var param in headerParams)
			{
				// because content has default header
				if (content.Headers.Contains(param.Key))
				{
					content.Headers.Remove(param.Key);
					content.Headers.Add(param.Key, param.Value);
				}
				else
				{
					content.Headers.Add(param.Key, param.Value);
				}
			}
		}
		
		public string ApplyParamsToUrl(string url, Dictionary<string, string> parameters)
		{
			var uriBuilder = new UriBuilder(url);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			foreach (KeyValuePair<string, string> item in parameters)
			{
				query[item.Key] = item.Value;
			}
			uriBuilder.Query = query.ToString();
			return uriBuilder.ToString();
		}
	}

	public struct RequestResponse
	{
		public KeyValuePair<int, string> statusCode;
		public string header;
		public string content;
		public int timing;
	}
	public struct savedDataFormat
	{
		public string url { get; set; }
		public string method { get; set; }
		public List<DataGridValue> parameters { get; set; }
		public List<DataGridValue> header { get; set; }
		public string content { get; set; }
	}
	public class TemplateLoader
	{
		public void Save(string path, savedDataFormat data)
		{

			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.Write(JsonConvert.SerializeObject(data));
			}
		}
		public savedDataFormat Load(string path)
		{
			return JsonConvert.DeserializeObject<savedDataFormat>(System.IO.File.ReadAllText(path));
		}
		public savedDataFormat New()
		{
			var data = new savedDataFormat();

			data.url = "";
			data.method = "get";
			data.header = new List<DataGridValue>() { new DataGridValue(true, "Content-Type", "application/json") };
			data.parameters = new List<DataGridValue>();
			data.content = "";

			return data;
		}
	}
	public class DataGridValue
	{
		public bool active { get; set; }
		public string key { get; set; }
		public string value { get; set; }
		/// <summary>
		/// default constructor for GUI
		/// </summary>
		public DataGridValue()
		{
			this.active = true;
		}
		/// <summary>
		/// constructor for in-code purposes
		/// </summary>
		/// <param name="active"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public DataGridValue(bool active, string key, string value)
		{
			this.active = active;
			this.key = key;
			this.value = value;
		}
	}
}
