using web_client_task.Models.Dtos;

namespace web_client_task.Models.ResponceObjects
{
	public class GetAllProductsResponce
	{
        public int ProductsCount { get; set; }

        public IEnumerable<ProductDto> Products { get; set; }
    }
}
