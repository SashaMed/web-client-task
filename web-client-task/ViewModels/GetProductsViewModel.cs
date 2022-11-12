using web_client_task.Models.Dtos;

namespace web_client_task.ViewModels
{
    public class GetProductsViewModel
    {
        public IEnumerable<ProductDto> ProductDtos { get; set; }
    }
}
