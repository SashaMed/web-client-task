using web_client_task.Models.Dtos;

namespace web_client_task.ViewModels.Products
{
    public class ProductsIndexViewModel
    {
        public List<ProductDto> Products { get; set; }

        public PageViewModel PageViewModel { get; set; }
    }
}
