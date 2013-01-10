using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using SmoresIdleService.Models;

namespace SmoresIdleService.Hubs
{
	[HubName("status")]
	public class StatusHub : Hub
	{
		public StatusHub()
		{
			// get existing subscriptions if they exist
			Subscriptions = (SubscriptionService) HttpRuntime.Cache.Get(UserSubscriptionsCacheKey) ?? new SubscriptionService();
			HttpRuntime.Cache.Insert(UserSubscriptionsCacheKey, Subscriptions);
		}

		internal SubscriptionService Subscriptions { get; private set; }

		public override Task OnConnected()
		{
			if (!Subscriptions.UserSubscriptions.ContainsKey(Context.ConnectionId))
				Clients.Caller.ReSubscribe();

			return base.OnConnected();
		}

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

		public void RemoveUserSubscriptions(string[] userTokens)
		{
			if (userTokens == null || userTokens.Length == 0)
				return;

			foreach (string userToken in userTokens)
			{
				if (string.IsNullOrWhiteSpace(userToken))
					continue;

				Subscriptions.Remove(Context.ConnectionId, userToken);
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