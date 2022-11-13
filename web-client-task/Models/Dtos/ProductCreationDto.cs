using System.ComponentModel.DataAnnotations;

namespace web_client_task.Models.Dtos
{
    public class ProductCreationDto
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public int DefaultQuantity { get; set; }
        public int Quantity { get; set; }
    }
}
