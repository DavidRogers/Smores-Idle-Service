using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using SmoresIdleService.Models;

namespace SmoresIdleService.Hubs
{
	[HubName("status")]
	public class StatusHub : Hub
	{
		public bool Register()
		{
			return true;
		}

		public override Task OnDisconnected()
		{
			// clean up subscription list
			List<string> subscriptions;
			if (s_userSubscriptions.TryRemove(Context.ConnectionId, out subscriptions))
			{
				foreach (string subscription in subscriptions)
				{
					List<string> reverseSubscriptions;
					if (s_reverseUserSubscriptions.TryGetValue(subscription, out reverseSubscriptions))
						reverseSubscriptions.Remove(Context.ConnectionId);
				}
			}
			return base.OnDisconnected();
		}

		public UserStatusModel AddUserSubscription(string userToken)
		{
			if (string.IsNullOrWhiteSpace(userToken))
				return null;

			List<string> subscription = s_userSubscriptions.GetOrAdd(Context.ConnectionId, s => new List<string>());
			if (!subscription.Contains(userToken))
			{
				subscription.Add(userToken);
				List<string> reverseSubscription = s_reverseUserSubscriptions.GetOrAdd(userToken, s => new List<string>());
				if (!reverseSubscription.Contains(Context.ConnectionId))
					reverseSubscription.Add(Context.ConnectionId);
			}

			return StatusHelper.GetStatus(userToken);
		}

		public bool RemoveUserSubscription(string userToken)
		{
			if (string.IsNullOrWhiteSpace(userToken))
				return false;

			List<string> subscription;
			if (s_userSubscriptions.TryGetValue(Context.ConnectionId, out subscription) && subscription.Contains(userToken))
			{
				subscription.Remove(userToken);
				List<string> reverseSubscription;
				if (s_reverseUserSubscriptions.TryGetValue(userToken, out reverseSubscription) && reverseSubscription.Contains(Context.ConnectionId))
					reverseSubscription.Remove(Context.ConnectionId);
			}
			return true;
		}

		public void StatusChanged(UserStatusModel userStatus)
		{
			if (!StatusHelper.UpdateStatus(userStatus))
				return;

			List<string> subsciptions;
			if (s_reverseUserSubscriptions.TryGetValue(userStatus.Token, out subsciptions))
			{
				foreach (string callerId in subsciptions)
					Clients.Client(callerId).StatusChanged(userStatus);
			}
		}

		// a user can have many subscribers attached
		// this is used to remember a user and all the users they want notifications for
		static readonly ConcurrentDictionary<string, List<string>> s_userSubscriptions = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

		// contains the reverse lookup of user subscriptions
		// this is used when a status has changed, the changed user is the key and all their subscribers are listed as the value
		static readonly ConcurrentDictionary<string, List<string>> s_reverseUserSubscriptions = new ConcurrentDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
	}
}