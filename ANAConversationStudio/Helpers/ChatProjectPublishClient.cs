using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ANAConversationStudio.Helpers
{
	public class ChatProjectPublishClient
	{
		private const string PUBLISH_API = "/api/publish";
		private const string CHECK_PROJECT_EXISTS_API = "/api/exists?business_id?={business_id}";

		private PublishServer publishServer;
		private string authHeader;
		public ChatProjectPublishClient(PublishServer server)
		{
			publishServer = server;
			if (!string.IsNullOrWhiteSpace(publishServer.Key) && !string.IsNullOrWhiteSpace(publishServer.Secret))
				authHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(publishServer.Key + ":" + publishServer.Secret));
		}

		public async Task<(bool Status, string Msg)> Publish(PublishChatProject project, string flowUrl)
		{
			try
			{
				var resp = await HitPostAsync(PUBLISH_API, new
				{
					business_id = project.Id,
					business_name = project.Name,
					flow_url = flowUrl
				});
				return (true, ""); //If 200;
			}
			catch (Exception ex)
			{
				Logger.Write(ex);
				return (false, ex.Message);
			}
		}

		public async Task<(bool? Status, string Msg)> ProjectExists(PublishChatProject project)
		{
			try
			{
				var resp = await HitGetAsync(CHECK_PROJECT_EXISTS_API.Replace("{business_id}", project.Id));
				return (resp.exists, ""); //If 200;
			}
			catch (Exception ex)
			{
				Logger.Write(ex);
				return (null, ex.Message);
			}
		}

		private async Task<dynamic> HitGetAsync(string api)
		{
			using (var wc = new WebClient())
			{
				wc.Headers[HttpRequestHeader.Authorization] = authHeader;
				wc.Headers[HttpRequestHeader.Accept] = "application/json";
				var resp = await wc.DownloadStringTaskAsync(publishServer.Url + api);
				return JsonConvert.DeserializeObject(resp) as dynamic;
			}
		}

		private async Task<dynamic> HitPostAsync<T>(string api, T data)
		{
			using (var wc = new WebClient())
			{
				wc.Headers[HttpRequestHeader.Authorization] = authHeader;
				wc.Headers[HttpRequestHeader.ContentType] = "application/json";
				wc.Headers[HttpRequestHeader.Accept] = "application/json";
				var resp = await wc.UploadStringTaskAsync(publishServer.Url + api, JsonConvert.SerializeObject(data));
				return JsonConvert.DeserializeObject(resp) as dynamic;
			}
		}
	}
}
