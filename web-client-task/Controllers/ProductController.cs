using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using web_client_task.Models.Dtos;
using web_client_task.Models;
using web_client_task.ViewModels.Fridges;
using web_client_task.ViewModels.Products;
using System.Text;
using web_client_task.Models.ResponceObjects;
using web_client_task.ViewModels;
using web_client_task.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace web_client_task.Controllers
{
    public class ProductController : Controller
    {
        private const int pageSize = 9;
        HttpClient client = new HttpClient();
        private readonly IPhotoService _photoService;

        string Url { get => "https://localhost:44382"; }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public ProductController(IPhotoService photoService)
        {
            _photoService = photoService;
            client.DefaultRequestHeaders.Add("Accept", "application/json");
		}


        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var res = await client.GetAsync($"{Url}/api/products?PageNumber={pageNumber.ToString()}&PageSize={pageSize.ToString()}");

            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                new { statusCode = (int)res.StatusCode, message = "Failed to get all products" });
            }
            var productsString = await res.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<GetAllProductsResponce>(productsString, options);
            var page = new PageViewModel(product.ProductsCount, pageNumber, pageSize);
            var responce = new ProductsIndexViewModel
            {
                Products = (List<ProductDto>)product.Products,
                PageViewModel = page
            };

            return View(responce);
        }
        

        public async Task<IActionResult> Details(Guid id)
        {
            var request = new RequestParameters { PageNumber = 1, PageSize = 10 };
            var res = await client.GetAsync($"{Url}/api/products/{id.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                    new { statusCode = res.StatusCode, ErrorMessage = "Failed to get product" });
            }
            var fridgeString = await res.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(fridgeString, options);
            var responce = new ProductDetailsViewModel { Product = product };
            return View(responce);
        }

		[Authorize]
		public async Task<IActionResult> Update(Guid id)
        {
            var res = await client.GetAsync($"{Url}/api/products/{id.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                    new { statusCode = res.StatusCode, ErrorMessage = "Failed to get product" });
            }
            var fridgeString = await res.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(fridgeString, options);
            var responce = new ProductUpdateViewModel
            {
                Name = product.Name,
                DefaultQuantity = product.DefaultQuantity,
                Description = product.Description,
                ImagePath = product.ImagePath,
            };
            return View(responce);
        }

        [HttpPost]
		[Authorize]
		public async Task<IActionResult> Update(Guid id, ProductUpdateViewModel viewModel)
        {
			AddTokenToRequestHeader();
            var imageResult = await _photoService.AddPhotoAsync(viewModel.Image);
            var request = new ProductUpdateDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                DefaultQuantity = viewModel.DefaultQuantity,
                ImagePath = imageResult.Url.ToString()
            };
            var res = await client.PutAsync($"{Url}/api/products/{id.ToString()}",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
            if (!res.IsSuccessStatusCode)
            {
				ModelState.AddModelError(string.Empty, res.StatusCode.ToString());
				ModelState.AddModelError(string.Empty, "Update failed");
                return View(viewModel);
            }


            return RedirectToAction("Details", "Product", new { id = id});
        }

		[Authorize]
		public async Task<IActionResult> DeleteProductFromFridge(Guid productId, Guid fridgeId)
        {
            var res = await client.GetAsync($"{Url}/api/products/{productId.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                    new { statusCode = res.StatusCode, ErrorMessage = "Failed to get product" });
            }
            var fridgeString = await res.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(fridgeString, options);
            var responce = new ProductDeleteViewModel
            {
                Product = product,
                FridgeId = fridgeId
            };
            return View(responce);
        }

        [HttpPost]
		[Authorize]
		public async Task<IActionResult> DeleteProductFromFridgePost(Guid productId, Guid fridgeId)
        {
			AddTokenToRequestHeader();
			var res = await client.DeleteAsync($"{Url}/api/fridges/{fridgeId.ToString()}/products/{productId.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                    new { statusCode = res.StatusCode, ErrorMessage = "Failed to delete product from fridge" });
            }
            return RedirectToAction("Details", "Fridge", new { id = fridgeId });
        }



		[Authorize]
		public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var res = await client.GetAsync($"{Url}/api/products/{productId.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                    new { statusCode = res.StatusCode, ErrorMessage = "Failed to get product" });
            }
            var fridgeString = await res.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductDto>(fridgeString, options);
            var responce = new ProductDeleteViewModel
            {
                Product = product
            };
            return View(responce);
        }



        [HttpPost]
		[Authorize]
		public async Task<IActionResult> DeleteProductPost(Guid productId)
        {
			AddTokenToRequestHeader();
			var res = await client.DeleteAsync($"{Url}/api/products/{productId.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                    new { statusCode = res.StatusCode, ErrorMessage = "Failed to delete product" });
            }

            return RedirectToAction("Index", "Product");
        }


		[Authorize]
		public IActionResult Create(Guid fridgeId)
        {
            var responce = new ProductCreateViewModel() { FridgeId = fridgeId };
            return View(responce);
        }


        [HttpPost]
		[Authorize]
		public async Task<IActionResult> Create(Guid fridgeId, ProductCreateViewModel viewModel)
        {
            AddTokenToRequestHeader();

			if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "invalid input");
                return View(viewModel);
            }
            var imageResult = await _photoService.AddPhotoAsync(viewModel.Image);
            var request = new ProductCreationDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                DefaultQuantity = viewModel.DefaultQuantity,
                Quantity = viewModel.Quantity,
                ImagePath = imageResult.Url.ToString()
            };
            var res = await client.PostAsync($"{Url}/api/products/{fridgeId.ToString()}",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
            if (!res.IsSuccessStatusCode)
            {
				ModelState.AddModelError(string.Empty, res.StatusCode.ToString());
				ModelState.AddModelError(string.Empty, "Creation failed");
                return View(viewModel);
            }

            return RedirectToAction("Details", "Fridge", new { id = fridgeId });
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
