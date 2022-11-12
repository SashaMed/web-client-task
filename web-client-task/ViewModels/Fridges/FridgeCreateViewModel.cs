using web_client_task.Models;

namespace web_client_task.ViewModels.Fridges
{
	public class FridgeCreateViewModel
	{
		public string Name { get; set; }
		public string Owner { get; set; }
		public string Guid { get; set; }
		public List<FridgeModel> FridgeModels { get; set; }

		public List<bool> FridgeStates { get; set; }
	}
}
