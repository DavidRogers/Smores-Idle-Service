using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SmoresIdleService
{
	internal class SubscriptionService
	{
		internal SubscriptionService()
		{
			m_userSubscriptions = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
			m_reverseUserSubscriptions = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
		}

		internal ConcurrentDictionary<string, List<string>> UserSubscriptions { get { return m_userSubscriptions; } }
		internal ConcurrentDictionary<string, List<string>> ReverseUserSubscriptions { get { return m_reverseUserSubscriptions; } }

		internal void Add(string connectionId, string userToken)
		{
			List<string> subscription = UserSubscriptions.GetOrAdd(connectionId, s => new List<string>());
			if (!subscription.Contains(userToken))
			{
				subscription.Add(userToken);
				List<string> reverseSubscription = ReverseUserSubscriptions.GetOrAdd(userToken, s => new List<string>());
				if (!reverseSubscription.Contains(connectionId))
					reverseSubscription.Add(connectionId);
			}
		}

		internal void Remove(string connectionId)
		{
			List<string> subscriptions;
			if (UserSubscriptions.TryRemove(connectionId, out subscriptions))
			{
				foreach (string subscription in subscriptions)
				{
					List<string> reverseSubscriptions;
					if (ReverseUserSubscriptions.TryGetValue(subscription, out reverseSubscriptions))
						reverseSubscriptions.Remove(connectionId);
				}
			}
		}

		internal void Remove(string connectionId, string userToken)
		{
			List<string> subscription;
			if (UserSubscriptions.TryGetValue(connectionId, out subscription) && subscription.Contains(userToken))
			{
				subscription.Remove(userToken);
				List<string> reverseSubscription;
				if (ReverseUserSubscriptions.TryGetValue(userToken, out reverseSubscription) && reverseSubscription.Contains(connectionId))
					reverseSubscription.Remove(connectionId);
			}
		}

		// a user can have many subscribers attached
		// this is used to remember a user and all the users they want notifications for
		readonly ConcurrentDictionary<string, List<string>> m_userSubscriptions;

		// contains the reverse lookup of user subscriptions
		// this is used when a status has changed, the changed user is the key and all their subscribers are listed as the value
		readonly ConcurrentDictionary<string, List<string>> m_reverseUserSubscriptions;
	}
}