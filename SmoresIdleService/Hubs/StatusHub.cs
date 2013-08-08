using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SmoresIdleService.Models;

namespace SmoresIdleService.Hubs
{
	[HubName("status")]
	public class StatusHub : Hub
	{
		public StatusHub()
		{
		}

		public override Task OnConnected()
		{
			return base.OnConnected();
		}

		public override Task OnDisconnected()
		{
			// clean up subscription list
			return base.OnDisconnected();
		}

		public override Task OnReconnected()
		{
			return base.OnReconnected();
		}

		public UserStatusModel AddUserSubscription(string userToken)
		{
			if (string.IsNullOrWhiteSpace(userToken))
				return null;

			Groups.Add(Context.ConnectionId, userToken);
			return StatusService.GetStatus(userToken);
		}

		public UserStatusModel[] AddUserSubscriptions(string[] userTokens)
		{
			if (userTokens == null || userTokens.Length == 0)
				return null;

			List<UserStatusModel> models = new List<UserStatusModel>();
			foreach (string userToken in userTokens)
			{
				Groups.Add(Context.ConnectionId, userToken);
				models.Add(StatusService.GetStatus(userToken));
			}

			return models.ToArray();
		}

		public bool RemoveUserSubscription(string userToken)
		{
			if (string.IsNullOrWhiteSpace(userToken))
				return false;

			Groups.Remove(Context.ConnectionId, userToken);
			return true;
		}

		public void RemoveUserSubscriptions(string[] userTokens)
		{
			if (userTokens == null || userTokens.Length == 0)
				return;

			foreach (string userToken in userTokens)
			{
				if (string.IsNullOrWhiteSpace(userToken))
					continue;

				Groups.Remove(Context.ConnectionId, userToken);
			}
		}

		public bool UpdateStatus(UserStatusModel userStatus)
		{
			if (!StatusService.UpdateStatus(userStatus))
				return false;

			StatusService.NotifyStatusSubscribers(Clients, userStatus);
			return true;
		}

		internal const string UserSubscriptionsCacheKey = "UserSubscriptions";
	}
}