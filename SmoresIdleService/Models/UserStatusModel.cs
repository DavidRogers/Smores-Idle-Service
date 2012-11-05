﻿using System;
using Newtonsoft.Json;

namespace SmoresIdleService.Models
{
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