namespace web_client_task.Models.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public int DefaultQuantity { get; set; }
    }
}
