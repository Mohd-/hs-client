using System;
using System.Collections.Generic;
using System.Text;
using PegasusUtil;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class SpecialEventManager
{
	// Token: 0x06000318 RID: 792 RVA: 0x0000ECC7 File Offset: 0x0000CEC7
	private SpecialEventManager()
	{
	}

	// Token: 0x0600031A RID: 794 RVA: 0x0000ECDC File Offset: 0x0000CEDC
	public static SpecialEventManager Get()
	{
		if (SpecialEventManager.s_instance == null)
		{
			SpecialEventManager.s_instance = new SpecialEventManager();
		}
		return SpecialEventManager.s_instance;
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0000ECF8 File Offset: 0x0000CEF8
	public void InitEventTiming(IList<SpecialEventTiming> serverEventTimingList)
	{
		if (this.m_eventTimings.Count > 0)
		{
			Debug.LogWarning("SpecialEventManager.InitEventTiming(): m_eventTimings was not empty; clearing it first");
			this.m_eventTimings.Clear();
		}
		DateTime now = DateTime.Now;
		bool flag = false;
		int i = 0;
		while (i < serverEventTimingList.Count)
		{
			SpecialEventTiming specialEventTiming = serverEventTimingList[i];
			SpecialEventType @enum;
			try
			{
				@enum = EnumUtils.GetEnum<SpecialEventType>(specialEventTiming.Event);
			}
			catch (ArgumentException ex)
			{
				Error.AddDevWarning("GetEnum Error", ex.Message, new object[0]);
				flag = true;
				goto IL_147;
			}
			goto IL_74;
			IL_147:
			i++;
			continue;
			IL_74:
			if (this.m_eventTimings.ContainsKey(@enum))
			{
				Debug.LogWarning(string.Format("SpecialEventManager.InitEventTiming duplicate entry for event {0} received", @enum));
				flag = true;
				goto IL_147;
			}
			DateTime? startTime = default(DateTime?);
			if (specialEventTiming.HasStart)
			{
				if (specialEventTiming.Start > 0UL)
				{
					startTime = new DateTime?(now.AddSeconds(specialEventTiming.Start));
				}
				else
				{
					startTime = new DateTime?(now);
				}
			}
			DateTime? endTime = default(DateTime?);
			if (specialEventTiming.HasEnd)
			{
				if (specialEventTiming.End > 0UL)
				{
					endTime = new DateTime?(now.AddSeconds(specialEventTiming.End));
				}
				else
				{
					endTime = new DateTime?(now);
				}
			}
			this.m_eventTimings[@enum] = new SpecialEventManager.EventTiming(startTime, endTime);
			goto IL_147;
		}
		if (flag && ApplicationMgr.IsInternal())
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < serverEventTimingList.Count; j++)
			{
				SpecialEventTiming specialEventTiming2 = serverEventTimingList[j];
				stringBuilder.Append("\n   serverEvent=").Append(specialEventTiming2.Event);
				stringBuilder.Append(" start=").Append((!specialEventTiming2.HasStart) ? "null" : specialEventTiming2.Start.ToString());
				stringBuilder.Append(" end=").Append((!specialEventTiming2.HasEnd) ? "null" : specialEventTiming2.End.ToString());
			}
			foreach (SpecialEventType specialEventType in this.m_eventTimings.Keys)
			{
				SpecialEventManager.EventTiming eventTiming = this.m_eventTimings[specialEventType];
				stringBuilder.Append("\n   mgrEvent=").Append(specialEventType);
				stringBuilder.Append(" start=").Append((eventTiming.StartTime == null) ? "none" : eventTiming.StartTime.Value.ToString());
				stringBuilder.Append(" end=").Append((eventTiming.EndTime == null) ? "none" : eventTiming.EndTime.Value.ToString());
			}
			Debug.LogWarning(string.Format("EventTiming dump: {0}", stringBuilder.ToString()));
		}
	}

	// Token: 0x0600031C RID: 796 RVA: 0x0000F050 File Offset: 0x0000D250
	public DateTime? GetEventLocalStartTime(SpecialEventType eventType)
	{
		if (!this.m_eventTimings.ContainsKey(eventType))
		{
			return default(DateTime?);
		}
		return this.m_eventTimings[eventType].StartTime;
	}

	// Token: 0x0600031D RID: 797 RVA: 0x0000F08C File Offset: 0x0000D28C
	public DateTime? GetEventLocalEndTime(SpecialEventType eventType)
	{
		if (!this.m_eventTimings.ContainsKey(eventType))
		{
			return default(DateTime?);
		}
		return this.m_eventTimings[eventType].EndTime;
	}

	// Token: 0x0600031E RID: 798 RVA: 0x0000F0C8 File Offset: 0x0000D2C8
	public bool HasEventStarted(SpecialEventType eventType)
	{
		return this.ForceEventActive(eventType) || (this.m_eventTimings.ContainsKey(eventType) && this.m_eventTimings[eventType].HasStarted());
	}

	// Token: 0x0600031F RID: 799 RVA: 0x0000F108 File Offset: 0x0000D308
	public bool HasEventEnded(SpecialEventType eventType)
	{
		return !this.ForceEventActive(eventType) && (!this.m_eventTimings.ContainsKey(eventType) || this.m_eventTimings[eventType].HasEnded());
	}

	// Token: 0x06000320 RID: 800 RVA: 0x0000F148 File Offset: 0x0000D348
	public bool IsEventActive(SpecialEventType eventType, bool activeIfDoesNotExist)
	{
		if (this.ForceEventActive(eventType))
		{
			return true;
		}
		if (!this.m_eventTimings.ContainsKey(eventType))
		{
			return activeIfDoesNotExist;
		}
		return this.m_eventTimings[eventType].IsActiveNow();
	}

	// Token: 0x06000321 RID: 801 RVA: 0x0000F188 File Offset: 0x0000D388
	public bool IsEventActive(string eventName, bool activeIfDoesNotExist)
	{
		if ("always" == eventName)
		{
			return true;
		}
		SpecialEventType eventType = SpecialEventManager.GetEventType(eventName, SpecialEventType.UNKNOWN);
		if (eventType == SpecialEventType.UNKNOWN)
		{
			Debug.LogWarning(string.Format("SpecialEventManager.IsEventActive could not find SpecialEventType record for event '{0}'", eventName));
			return activeIfDoesNotExist;
		}
		return this.IsEventActive(eventType, activeIfDoesNotExist);
	}

	// Token: 0x06000322 RID: 802 RVA: 0x0000F1D0 File Offset: 0x0000D3D0
	public static SpecialEventType GetEventType(string eventName, SpecialEventType defaultIfNotExists = SpecialEventType.UNKNOWN)
	{
		SpecialEventType result;
		if (!EnumUtils.TryGetEnum<SpecialEventType>(eventName, out result))
		{
			Debug.LogWarning(string.Format("SpecialEventManager.GetEventType could not find SpecialEventType record for event '{0}'", eventName));
			return defaultIfNotExists;
		}
		return result;
	}

	// Token: 0x06000323 RID: 803 RVA: 0x0000F1FD File Offset: 0x0000D3FD
	private void OnReset()
	{
		this.m_eventTimings.Clear();
		this.m_forcedActiveEvents.Clear();
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0000F218 File Offset: 0x0000D418
	private bool ForceEventActive(SpecialEventType eventType)
	{
		if (!ApplicationMgr.IsInternal())
		{
			return false;
		}
		if (this.m_forcedActiveEvents == null)
		{
			this.m_forcedActiveEvents = new HashSet<SpecialEventType>();
			string str = Vars.Key("Events.ForceActive").GetStr(null);
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			string[] array = str.Split(new char[]
			{
				' ',
				',',
				';'
			}, 1);
			for (int i = 0; i < array.Length; i++)
			{
				SpecialEventType specialEventType;
				if (EnumUtils.TryGetEnum<SpecialEventType>(array[i], 5, out specialEventType))
				{
					this.m_forcedActiveEvents.Add(specialEventType);
				}
			}
		}
		return this.m_forcedActiveEvents.Contains(eventType);
	}

	// Token: 0x04000148 RID: 328
	private static SpecialEventManager s_instance;

	// Token: 0x04000149 RID: 329
	private Map<SpecialEventType, SpecialEventManager.EventTiming> m_eventTimings = new Map<SpecialEventType, SpecialEventManager.EventTiming>();

	// Token: 0x0400014A RID: 330
	private HashSet<SpecialEventType> m_forcedActiveEvents;

	// Token: 0x02000179 RID: 377
	private class EventTiming
	{
		// Token: 0x060015D7 RID: 5591 RVA: 0x0006783E File Offset: 0x00065A3E
		public EventTiming(DateTime? startTime, DateTime? endTime)
		{
			this.StartTime = startTime;
			this.EndTime = endTime;
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060015D8 RID: 5592 RVA: 0x00067854 File Offset: 0x00065A54
		// (set) Token: 0x060015D9 RID: 5593 RVA: 0x0006785C File Offset: 0x00065A5C
		public DateTime? StartTime { get; private set; }

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060015DA RID: 5594 RVA: 0x00067865 File Offset: 0x00065A65
		// (set) Token: 0x060015DB RID: 5595 RVA: 0x0006786D File Offset: 0x00065A6D
		public DateTime? EndTime { get; private set; }

		// Token: 0x060015DC RID: 5596 RVA: 0x00067878 File Offset: 0x00065A78
		public bool HasStarted()
		{
			return this.StartTime == null || DateTime.Now >= this.StartTime.Value;
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x000678B4 File Offset: 0x00065AB4
		public bool HasEnded()
		{
			return this.EndTime != null && DateTime.Now > this.EndTime.Value;
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x000678F0 File Offset: 0x00065AF0
		public bool IsActiveNow()
		{
			return (this.StartTime == null || this.EndTime == null || !(this.EndTime.Value < this.StartTime.Value)) && this.HasStarted() && !this.HasEnded();
		}
	}
}
