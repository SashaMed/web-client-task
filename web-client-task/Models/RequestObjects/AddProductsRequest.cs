namespace web_client_task.Models.RequestObjects
{
    public class AddProductsRequest
    {
        public IEnumerable<Guid> Guids { get; set; }
        public IEnumerable<int> QuantityList { get; set; }
    }
}
