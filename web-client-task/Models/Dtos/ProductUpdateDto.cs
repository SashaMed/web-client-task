namespace web_client_task.Models.Dtos
{
    public class ProductUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int DefaultQuantity { get; set; }
        public string ImagePath { get; set; }
    }
}
