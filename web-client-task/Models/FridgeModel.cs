using System.ComponentModel.DataAnnotations;

namespace web_client_task.Models
{
    public class FridgeModel
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Year { get; set; }
    }
}
