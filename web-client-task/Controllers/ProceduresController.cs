using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using web_client_task.Interfaces;
using web_client_task.Models.Dtos;
using web_client_task.Models.ResponceObjects;
using web_client_task.Services;
using web_client_task.ViewModels.Products;
using web_client_task.ViewModels;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace web_client_task.Controllers
{
    public class ProceduresController : Controller
    {
        static HttpClient client = new HttpClient();

        string Url { get => "https://localhost:44382"; }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public ProceduresController()
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
		[Authorize]
		public async Task<IActionResult> Call6thTaskMethod()
        {
            var res = await client.PostAsync($"{Url}/api/procedures/procedure", null);
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                new { statusCode = (int)res.StatusCode, message = "Failed to call procedures" });
            }
            var serverResponceString = await res.Content.ReadAsStringAsync();
            var responce = JsonSerializer.Deserialize<string>(serverResponceString, options);

            ViewData["Message"] = responce;
            return View();
        }
    }
}
