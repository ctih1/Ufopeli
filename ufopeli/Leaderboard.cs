using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System.Xml;
using System.Net.Http;
using System.Net;
using System.Net.Mime;

namespace ufopeli
{
	public class Leaderboard
	{
		HttpClient httpClient = new HttpClient();
		HttpResponseMessage response;
		HttpRequestMessage request;
		public void StartServer()
		{
			
			request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri("https://ufopeliserver.ctih.repl.co/"),
			};
			response = httpClient.Send(request);

		}
		public Boolean SaveData(String user, int score)
		{
			// Settings TLS 1.2
			System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(String.Format("https://ufopeliserver.ctih.repl.co/save?name={0}&score={1}",user,score)),
			};
			response = httpClient.Send(request);
			return response.IsSuccessStatusCode;
		} 

	}
}
