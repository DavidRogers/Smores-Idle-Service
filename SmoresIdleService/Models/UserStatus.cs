using Newtonsoft.Json;

namespace SmoresIdleService.Models
{
	public class UserStatus
	{
		[JsonProperty]
		public string Token { get; set; }

		[JsonProperty]
		public int Status { get; set; }
	}
}