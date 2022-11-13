namespace web_client_task.ViewModels.Products
{
	public class ProductCreateViewModel
	{
        public Guid FridgeId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public int DefaultQuantity { get; set; }
        public int Quantity { get; set; }
    }
}
