using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using SignalRHubs;

namespace LearningSignalR2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHubContext<LearningHub> hubContext;

        public IndexModel(IHubContext<LearningHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public void OnGet()
        {
            hubContext.Clients.All.SendAsync("ReceiveMessage", "Client connected to a secondary web application");
        }
    }
}
