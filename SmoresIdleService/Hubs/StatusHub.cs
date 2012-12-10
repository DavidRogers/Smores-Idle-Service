using System;
using System.Collections.Concurrent;
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
			m_userSubscriptions = (ConcurrentDictionary<string, List<string>>) HttpRuntime.Cache.Get(UserSubscriptionsCacheKey) ?? new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
			m_reverseUserSubscriptions = (ConcurrentDictionary<string, List<string>>) HttpRuntime.Cache.Get(ReverseUserSubscriptionsCacheKey) ?? new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

			HttpRuntime.Cache.Insert(UserSubscriptionsCacheKey, m_userSubscriptions);
			HttpRuntime.Cache.Insert(ReverseUserSubscriptionsCacheKey, m_reverseUserSubscriptions);
		}

		public override Task OnDisconnected()
		{
			// clean up subscription list
			List<string> subscriptions;
			if (m_userSubscriptions.TryRemove(Context.ConnectionId, out subscriptions))
			{
				foreach (string subscription in subscriptions)
				{
					List<string> reverseSubscriptions;
					if (m_reverseUserSubscriptions.TryGetValue(subscription, out reverseSubscriptions))
						reverseSubscriptions.Remove(Context.ConnectionId);
				}
			}
			return base.OnDisconnected();
		}

		public UserStatusModel AddUserSubscription(string userToken)
		{
			if (string.IsNullOrWhiteSpace(userToken))
				return null;

			List<string> subscription = m_userSubscriptions.GetOrAdd(Context.ConnectionId, s => new List<string>());
			if (!subscription.Contains(userToken))
			{
				subscription.Add(userToken);
				List<string> reverseSubscription = m_reverseUserSubscriptions.GetOrAdd(userToken, s => new List<string>());
				if (!reverseSubscription.Contains(Context.ConnectionId))
					reverseSubscription.Add(Context.ConnectionId);
			}

			return StatusHelper.GetStatus(userToken);
		}

		public UserStatusModel[] AddUserSubscriptions(string[] userTokens)
		{
			if (userTokens == null || userTokens.Length == 0)
				return null;

			List<UserStatusModel> models = new List<UserStatusModel>();
			List<string> subscription = m_userSubscriptions.GetOrAdd(Context.ConnectionId, s => new List<string>());
			foreach (string userToken in userTokens)
			{
				if (!subscription.Contains(userToken))
				{
					subscription.Add(userToken);
					List<string> reverseSubscription = m_reverseUserSubscriptions.GetOrAdd(userToken, s => new List<string>());
					if (!reverseSubscription.Contains(Context.ConnectionId))
						reverseSubscription.Add(Context.ConnectionId);
				}
				models.Add(StatusHelper.GetStatus(userToken));
			}

			return models.ToArray();
		}

		public bool RemoveUserSubscription(string userToken)
		{
			if (string.IsNullOrWhiteSpace(userToken))
				return false;

			List<string> subscription;
			if (m_userSubscriptions.TryGetValue(Context.ConnectionId, out subscription) && subscription.Contains(userToken))
			{
				subscription.Remove(userToken);
				List<string> reverseSubscription;
				if (m_reverseUserSubscriptions.TryGetValue(userToken, out reverseSubscription) && reverseSubscription.Contains(Context.ConnectionId))
					reverseSubscription.Remove(Context.ConnectionId);
			}
			return true;
		}

		public void UpdateStatus(UserStatusModel userStatus)
		{
			if (!StatusHelper.UpdateStatus(userStatus))
				return;

			StatusHelper.NotifyStatusSubscribers(Clients, userStatus);
		}

		public string[] GetSubscriptions()
		{
			List<string> subscriptions;
			return m_userSubscriptions.TryGetValue(Context.ConnectionId, out subscriptions) ? subscriptions.ToArray() : null;
		}

		public int GetSubscriberCount()
		{
			List<string> subscriptions;
			return m_reverseUserSubscriptions.TryGetValue(Context.ConnectionId, out subscriptions) ? subscriptions.Count : 0;
		}

		internal const string UserSubscriptionsCacheKey = "UserSubscriptions";
		internal const string ReverseUserSubscriptionsCacheKey = "ReverseUserSubscriptions";

		// a user can have many subscribers attached
		// this is used to remember a user and all the users they want notifications for
		readonly ConcurrentDictionary<string, List<string>> m_userSubscriptions;

		// contains the reverse lookup of user subscriptions
		// this is used when a status has changed, the changed user is the key and all their subscribers are listed as the value
		readonly ConcurrentDictionary<string, List<string>> m_reverseUserSubscriptions;
	}
}