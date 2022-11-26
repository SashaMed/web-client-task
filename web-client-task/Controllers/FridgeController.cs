using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using web_client_task.Models;
using web_client_task.Models.Dtos;
using web_client_task.Models.RequestObjects;
using web_client_task.Models.ResponceObjects;
using web_client_task.ViewModels;
using web_client_task.ViewModels.Fridges;

namespace web_client_task.Controllers
{
    public class FridgeController : Controller
    {
        private const int pageSize = 3;
        static HttpClient client = new HttpClient();
        string Url { get => "https://localhost:44382"; }

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

        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> Create(FridgeCreateViewModel viewModel)
        {
			AddTokenToRequestHeader();
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
				return RedirectToAction("RequestError", "Home",
				new { statusCode = (int)res.StatusCode, message = "Failed to create fridge" });
			}
            return RedirectToAction("Index", "Fridge");
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await client.GetAsync($"{Url}/api/fridges/{id.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                new { statusCode = (int)res.StatusCode, message = "Failed to get fridge" });
            }
            var fridgeString = await res.Content.ReadAsStringAsync();
            var fridge = JsonSerializer.Deserialize<GetFridgeDetailsResponce>(fridgeString, options);
            fridge.Fridge.Products = fridge.Products;
            var responce = new FridgeDetailsViewModel { Fridge = fridge.Fridge };
            return View(responce);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePost(Guid id)
        {
			AddTokenToRequestHeader();
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


        [Authorize]
        public async Task<IActionResult> AddProducts(Guid fridgeId, int pageNumber = 1)
        {

			var res = await client.GetAsync($"{Url}/api/fridges/{fridgeId.ToString()}/products/outside" +
                $"?PageNumber={pageNumber.ToString()}&PageSize={pageSize.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                new { statusCode = (int)res.StatusCode, message = "Failed to get fridge" });
            }
            var responceString = await res.Content.ReadAsStringAsync();
            var serverResponce = JsonSerializer.Deserialize<GetProductsNotInFridgeResponce>(responceString, options);
            var responce = new FridgeAddProductViewModel
            {
                Fridge = serverResponce.Fridge,
                Products = (List<ProductDto>)serverResponce.Products,
                PageViewModel = new PageViewModel(serverResponce.ProductsCount,pageNumber,pageSize)
            };
            return View(responce);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProducts(Guid fridgeId, FridgeAddProductViewModel fridgeAddProductViewModel)
        {
            AddTokenToRequestHeader();

			var request = CreateAddProductsRequest(fridgeAddProductViewModel);
            var res = await client.PostAsync($"{Url}/api/fridges/{fridgeId.ToString()}/products", new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8, "application/json"));
                if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                new { statusCode = (int)res.StatusCode, message = "Failed to add products" });
            }
            return RedirectToAction("Details", "Fridge", new {id = fridgeId});
        }

        private AddProductsRequest CreateAddProductsRequest(FridgeAddProductViewModel vm)
        {
            var quantityes = new List<int>();
            var guids = new List<Guid>();
            for (int i = 0; i < vm.CheckBoxes.Count(); i++)
            {
                if (vm.CheckBoxes[i])
                {
                    quantityes.Add(vm.QuantityList[i]);
                    guids.Add(vm.InputProducts[i]);
                }
            }
            return new AddProductsRequest { Guids = guids, QuantityList = quantityes };
        }

        private void AddTokenToRequestHeader()
        {
            if (client.DefaultRequestHeaders.Authorization == null)
            {
				var token = Request.Cookies["jwtToken"];
				client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
			}
        }
    }
}
