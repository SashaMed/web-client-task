using Microsoft.AspNetCore.Mvc;
using web_client_task.Models.Dtos;

namespace web_client_task.ViewModels.Fridges
{
    //[BindProperties]
    public class FridgeAddProductViewModel
    {
        //[FromBody]
        public FridgeDto Fridge { get; set; }
        //[FromBody]
        public List<ProductDto> Products { get; set; }

        public List<Guid> InputProducts { get; set; }
        //[FromBody]
        public List<bool> CheckBoxes { get; set; }
        //[FromBody]
        public List<int> QuantityList { get; set; }
        //[FromBody]
        public PageViewModel PageViewModel { get; set; }
    }
}
