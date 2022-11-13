using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using web_client_task.Models;
using web_client_task.Models.Dtos;
using web_client_task.Models.ResponceObjects;
using web_client_task.ViewModels;
using web_client_task.ViewModels.Fridges;

namespace web_client_task.Controllers
{
    public class FridgeController : Controller
    {
        private const int pageSize = 3;
        static HttpClient client = new HttpClient();
        string Url { get => "http://localhost:5249"; }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public FridgeController()
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<IActionResult> Index()
        {
            var res = await client.GetAsync($"{Url}/api/fridges");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home", 
                    new { statusCode = res.StatusCode, ErrorMessage = "Failed to get all fridges" });

            }
            var fridgeString = await res.Content.ReadAsStringAsync();
            var fridges = JsonSerializer.Deserialize<IEnumerable<FridgeDto>>(fridgeString, options);
            var responce = new FridgesIndexViewModel
            {
                Fridges = (List<FridgeDto>)fridges
            };
            return View(responce);
        }

        public async Task<IActionResult> Create()
        {
            var res = await client.GetAsync($"{Url}/api/fridges/models");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                new { statusCode = (int)res.StatusCode, message = "Failed to get fridge models" });
            }
            var fridgeString = await res.Content.ReadAsStringAsync();
            var models = JsonSerializer.Deserialize<IEnumerable<FridgeModel>>(fridgeString, options);
            var responce = new FridgeCreateViewModel
            {
                FridgeModels = (List<FridgeModel>)models,
                FridgeStates = new List<bool>(models.Count())
            };
            return View(responce);
        }


        [HttpPost]
        public async Task<IActionResult> Create(FridgeCreateViewModel viewModel)
        {
            var request = new FridgeCreationDto
            {
                Name = viewModel.Name,
                OwnerName = viewModel.Owner,
                FridgeModelId =  new Guid(viewModel.Guid)
            };
            var res = await client.PostAsync($"{Url}/api/fridges", new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8, "application/json"));
            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Creation failed");
                return View(viewModel);
            }
            return RedirectToAction("Index", "Fridge");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await client.GetAsync($"{Url}/api/fridges/{id.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                new { statusCode = (int)res.StatusCode, message = "Failed to get fridge" });
            }
            var fridgeString = await res.Content.ReadAsStringAsync();
            var fridge = JsonSerializer.Deserialize<FridgeDto>(fridgeString, options);
            var responce = new FridgeDetailsViewModel { Fridge = fridge };
            return View(responce);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var res = await client.DeleteAsync($"{Url}/api/fridges/{id.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                new { statusCode = (int)res.StatusCode, message = "Failed to delete fridge" });
            }

            return RedirectToAction("Index", "Fridge");
        }

        public async Task<IActionResult> Details(Guid id, int pageNumber = 1)
        {
            var res = await client.GetAsync($"{Url}/api/fridges/{id.ToString()}?PageNumber={pageNumber.ToString()}&PageSize={pageSize.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                new { statusCode = (int)res.StatusCode, message = "Failed to get fridge" });
            }
            var responceString = await res.Content.ReadAsStringAsync();
            var serverResponce = JsonSerializer.Deserialize<GetFridgeDetailsResponce>(responceString, options);
            FridgeDto fridgeDto = serverResponce.Fridge;
            fridgeDto.Products = serverResponce.Products;
            var responce = new FridgeDetailsViewModel 
            { 
                Fridge = fridgeDto,
                PageViewModel = new PageViewModel(serverResponce.ProductsCount, pageNumber, pageSize)                
            };
            return View(responce);
        }
    }
}
