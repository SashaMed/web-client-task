using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using web_client_task.Models.Dtos;
using web_client_task.Models;
using web_client_task.ViewModels.Fridges;
using web_client_task.ViewModels.Products;
using System.Text;
using web_client_task.Models.ResponceObjects;
using web_client_task.ViewModels;

namespace web_client_task.Controllers
{
    public class ProductController : Controller
    {
        private const int pageSize = 9;
        static HttpClient client = new HttpClient();
        string Url { get => "http://localhost:5249"; }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public ProductController()
        {
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
                Description = product.Description
            };
            return View(responce);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid id, ProductUpdateViewModel viewModel)
        {
            var request = new ProductCreationDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                DefaultQuantity = viewModel.DefaultQuantity
            };
            var res = await client.PutAsync($"{Url}/api/products/{id.ToString()}",
                new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8, "application/json"));
            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Update failed");
                return View(viewModel);
            }


            return RedirectToAction("Details", "Product", new { id = id});
        }


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

        [HttpDelete]
        public async Task<IActionResult> DeleteProductFromFridgePost(Guid productId, Guid fridgeId)
        {
            var res = await client.DeleteAsync($"{Url}/api/fridges/{fridgeId.ToString()}/products/{productId.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                    new { statusCode = res.StatusCode, ErrorMessage = "Failed to delete product from fridge" });
            }
            return RedirectToAction("Details", "Fridge", new { id = fridgeId });
        }


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
        public async Task<IActionResult> DeleteProductPost(Guid productId)
        {
            var res = await client.DeleteAsync($"{Url}/api/products/{productId.ToString()}");
            if (!res.IsSuccessStatusCode)
            {
                return RedirectToAction("RequestError", "Home",
                    new { statusCode = res.StatusCode, ErrorMessage = "Failed to delete product" });
            }

            return RedirectToAction("Index", "Product");
        }

        public IActionResult Create(Guid fridgeId)
        {
            var responce = new ProductCreateViewModel() { FridgeId = fridgeId };
            return View(responce);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid fridgeId, ProductCreateViewModel viewModel)
        {
            var request = new ProductCreationDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                DefaultQuantity = viewModel.DefaultQuantity,
                Quantity = viewModel.Quantity
            };
            var res = await client.PostAsync($"{Url}/api/products/{fridgeId.ToString()}",
                new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8, "application/json"));
            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Creation failed");
                return View(viewModel);
            }

            return RedirectToAction("Details", "Fridge", new { id = fridgeId });
        }
    }
    
}
