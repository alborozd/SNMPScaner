﻿using Lextm.SharpSnmpLib;

namespace DomainModel
{
	public class Notification
	{
		public Notification(long subscriptionItemId, ISnmpData newValue, ISnmpData oldValue)
		{
			SubscriptionItemId = subscriptionItemId;
			NewValue = newValue;
			OldValue = oldValue;
		}
		public long SubscriptionItemId { get; private set; }

		public ISnmpData NewValue { get; private set; }
		public ISnmpData OldValue { get; private set; }

		private static readonly Notification _empty = new Notification(0 , null, null);
		public static Notification Empty
		{
			get { return _empty; }
		}
	}
}
