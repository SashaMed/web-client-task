using System.ComponentModel.DataAnnotations;

namespace web_client_task.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DefaultQuantity { get; set; }
    }
}
