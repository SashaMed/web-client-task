using System.ComponentModel.DataAnnotations;

namespace web_client_task.Models.Dtos
{
    public class FridgeCreationDto
    {
        [Required(ErrorMessage = "Fridge name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Owner name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Owner name is 30 characters.")]
        public string OwnerName { get; set; }


        public Guid FridgeModelId { get; set; }
    }
}
