using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using SmoresIdleService.Models;

namespace SmoresIdleService.Hubs
{
	[HubName("status")]
	public class StatusHub : Hub
	{
		public StatusHub()
		{
			Subscriptions = new SubscriptionService();
		}

		internal SubscriptionService Subscriptions { get; private set; }

		public override Task OnDisconnected()
		{
			// clean up subscription list
			Subscriptions.Remove(Context.ConnectionId);
			return base.OnDisconnected();
		}

		public UserStatusModel AddUserSubscription(string userToken)
		{
			if (string.IsNullOrWhiteSpace(userToken))
				return null;

			Subscriptions.Add(Context.ConnectionId, userToken);
			return StatusService.GetStatus(userToken);
		}

		public UserStatusModel[] AddUserSubscriptions(string[] userTokens)
		{
			if (userTokens == null || userTokens.Length == 0)
				return null;

			List<UserStatusModel> models = new List<UserStatusModel>();
			foreach (string userToken in userTokens)
			{
				Subscriptions.Add(Context.ConnectionId, userToken);
				models.Add(StatusService.GetStatus(userToken));
			}

			return models.ToArray();
		}

		public bool RemoveUserSubscription(string userToken)
		{
			if (string.IsNullOrWhiteSpace(userToken))
				return false;

			Subscriptions.Remove(Context.ConnectionId, userToken);
			return true;
		}

		public void UpdateStatus(UserStatusModel userStatus)
		{
			if (!StatusService.UpdateStatus(userStatus))
				return;

			StatusService.NotifyStatusSubscribers(Clients, userStatus);
		}

		internal const string UserSubscriptionsCacheKey = "UserSubscriptions";
		internal const string ReverseUserSubscriptionsCacheKey = "ReverseUserSubscriptions";
	}
}