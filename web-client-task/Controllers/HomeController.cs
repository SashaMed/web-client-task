using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using web_client_task.Models;
using web_client_task.Models.Dtos;
using web_client_task.ViewModels;

namespace web_client_task.Controllers
{
    public class HomeController : Controller
    {
        static HttpClient client = new HttpClient();
        private readonly ILogger<HomeController> _logger;
        string Url { get => "http://localhost:5249"; }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };


        public HomeController(ILogger<HomeController> logger)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }


        public async Task<IActionResult> Get()
        {
            var url = $"{Url}/api/products";
            string result = await client.GetStringAsync(Url);
            var products =  JsonSerializer.Deserialize<IEnumerable<ProductDto>>(result, options);
            return View(new GetProductsViewModel { ProductDtos = products });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}