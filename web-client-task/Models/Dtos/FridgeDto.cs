namespace web_client_task.Models.Dtos
{
    public class FridgeDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OwnerName { get; set; }

        public Guid FridgeModelId { get; set; }

        public FridgeModel FridgeModel { get; set; }

        public IEnumerable<Product> Products { get; set; }

    }
}
