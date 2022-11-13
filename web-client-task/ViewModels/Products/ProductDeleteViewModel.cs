using web_client_task.Models.Dtos;

namespace web_client_task.ViewModels.Products
{
    public class ProductDeleteViewModel
    {
        public Guid FridgeId { get; set; }
        public ProductDto Product { get; set; }
    }
}
