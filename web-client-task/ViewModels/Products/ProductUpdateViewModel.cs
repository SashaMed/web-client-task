using web_client_task.Models.Dtos;

namespace web_client_task.ViewModels.Products
{
    public class ProductUpdateViewModel
    {
        //public ProductDto Product { get; set; }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public int DefaultQuantity { get; set; }
    }
}
