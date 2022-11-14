using web_client_task.Models.Dtos;

namespace web_client_task.Models.ResponceObjects
{
    public class GetProductsNotInFridgeResponce
    {
        public FridgeDto Fridge { get; set; }
        public int ProductsCount { get; set; }
        public IEnumerable<ProductDto> Products { get; set; }
    }
}
