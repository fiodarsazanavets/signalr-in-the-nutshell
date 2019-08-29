using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LearningSignalR2.Pages
{
    public class IndexModel : PageModel
    {
        private const string SignalRHubUrl = "https://<instance-name>.service.signalr.net/api/v1/hubs/learningHub";

        public async Task OnGet()
        {
            using (HttpClient client = new HttpClient())
            {
                var payloadMessage = new
                {
                    Target = "ReceiveMessage",
                    Arguments = new[]
                    {
                        "Client connected to a secondary web application"
                    }
                };

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new UriBuilder(SignalRHubUrl).Uri);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(JsonConvert.SerializeObject(payloadMessage), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Failure sending SignalR message.");
            }

            //hubContext.Clients.All.SendAsync("ReceiveMessage", "Client connected to a secondary web application");
        }
    }
}
