using System;
using Newtonsoft.Json;

namespace SmoresIdleService.Models
{
	public enum UserStatusEnum
	{
		Unknown = 0,
		Idle = 1,
		Busy = 3,
		Away = 4,
		Active = 5
	}

	public class UserStatusModel
	{
		[JsonProperty(Required = Required.Always)]
		public string Token { get; set; }

		[JsonProperty]
		public int Status { get; set; }

		[JsonProperty]
		public DateTime LastUpdated { get; set; }
	}
}