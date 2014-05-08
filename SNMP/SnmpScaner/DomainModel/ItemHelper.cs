﻿using System;
using System.Collections.Generic;
using Lextm.SharpSnmpLib;

namespace DomainModel
{
	public static class ItemHelper
	{
		/// <summary>
		/// Список с типами, для которых вычисляется разность значений
		/// </summary>
		public static readonly List<SnmpType> ValueTypes = new List<SnmpType>
		{
			SnmpType.Counter32, SnmpType.Counter64, SnmpType.Gauge32,SnmpType.Integer32, SnmpType.TimeTicks
		};

		/// <summary>
		/// Проверяет нужно ли делать обновление
		/// </summary>
		/// <param name="timestamp">Старая метка времени</param>
		/// <param name="newTimestamp">новая метка времени</param>
		/// <param name="timeDelta">Необходимая для обновления разница в секундах</param>
		/// <param name="value">Старое значение</param>
		/// <param name="newValue">Новое значение</param>
		/// <param name="valueDelta">Необходимая для обновления разница</param>
		/// <returns></returns>
		public static bool NeedUpdate(DateTime timestamp, DateTime newTimestamp, long timeDelta, ISnmpData value, ISnmpData newValue,
			ISnmpData valueDelta)
		{
			if ((newTimestamp - timestamp).TotalSeconds < timeDelta) return false;
			return NeedUpdate(value, newValue, valueDelta);
		}
		public static bool NeedUpdate(ISnmpData value, ISnmpData newValue, ISnmpData diff)
		{
			if (newValue == null) return false;
			if (value == null) return true;
			if (value.Equals(newValue)) return false;
			if (diff == null) return true;

			if (value.TypeCode != newValue.TypeCode || newValue.TypeCode != diff.TypeCode) return true;
			if (!ValueTypes.Contains(newValue.TypeCode)) return true;

			switch (value.TypeCode)
			{
				case SnmpType.Counter32:
					return NeedUpdate(value as Counter32, newValue as Counter32, diff as Counter32);
				case SnmpType.Gauge32:
					return NeedUpdate(value as Gauge32, newValue as Gauge32, diff as Gauge32);
				case SnmpType.Integer32:
					return NeedUpdate(value as Integer32, newValue as Integer32, diff as Integer32);
				case SnmpType.Counter64:
					return NeedUpdate(value as Counter64, newValue as Counter64, diff as Counter64);
				case SnmpType.TimeTicks:
					return NeedUpdate(value as TimeTicks, newValue as TimeTicks, diff as TimeTicks);
				default:
					return true;
			}
		}
		private static bool NeedUpdate(Counter32 value, Counter32 newValue, Counter32 diff)
		{
			if (newValue == null) return false;
			if (value == null) return true;
			if (diff == null) return true;
			return Math.Abs(newValue.ToUInt32() - value.ToUInt32()) > diff.ToUInt32();
		}
		private static bool NeedUpdate(Gauge32 value, Gauge32 newValue, Gauge32 diff)
		{
			if (newValue == null) return false;
			if (value == null) return true;
			if (diff == null) return true;
			return Math.Abs(newValue.ToUInt32() - value.ToUInt32()) > diff.ToUInt32();
		}
		private static bool NeedUpdate(Integer32 value, Integer32 newValue, Integer32 diff)
		{
			if (newValue == null) return false;
			if (value == null) return true;
			if (diff == null) return true;
			return Math.Abs(newValue.ToInt32() - value.ToInt32()) > diff.ToInt32();
		}
		private static bool NeedUpdate(Counter64 value, Counter64 newValue, Counter64 diff)
		{
			if (newValue == null) return false;
			if (value == null) return true;
			if (diff == null) return true;
			return Math.Abs((decimal)(newValue.ToUInt64() - value.ToUInt64())) > diff.ToUInt64();
		}
		private static bool NeedUpdate(TimeTicks value, TimeTicks newValue, TimeTicks diff)
		{
			if (newValue == null) return false;
			if (value == null) return true;
			if (diff == null) return true;
			return Math.Abs(newValue.ToUInt32() - value.ToUInt32()) > diff.ToUInt32();
		}
	}
}
