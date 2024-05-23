using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001AE RID: 430
[CustomEditClass]
public class StateEventTable : MonoBehaviour
{
	// Token: 0x06001C25 RID: 7205 RVA: 0x000849DC File Offset: 0x00082BDC
	public void TriggerState(string eventName, bool saveLastState = true, string nameOverride = null)
	{
		StateEventTable.StateEvent stateEvent = this.GetStateEvent(eventName);
		if (stateEvent == null)
		{
			Debug.LogError(string.Format("{0} not defined in event table.", eventName), base.gameObject);
			return;
		}
		this.m_QueuedEvents.Enqueue(new StateEventTable.QueueStateEvent
		{
			m_StateEvent = stateEvent,
			m_NameOverride = nameOverride,
			m_SaveAsLastState = saveLastState
		});
		if (this.m_QueuedEvents.Count == 1)
		{
			this.StartNextQueuedState(null);
		}
	}

	// Token: 0x06001C26 RID: 7206 RVA: 0x00084A50 File Offset: 0x00082C50
	public bool HasState(string eventName)
	{
		return this.m_Events.Find((StateEventTable.StateEvent e) => e.m_Name == eventName) != null;
	}

	// Token: 0x06001C27 RID: 7207 RVA: 0x00084A87 File Offset: 0x00082C87
	public void CancelQueuedStates()
	{
		this.m_QueuedEvents.Clear();
	}

	// Token: 0x06001C28 RID: 7208 RVA: 0x00084A94 File Offset: 0x00082C94
	public Spell GetSpellEvent(string eventName)
	{
		StateEventTable.StateEvent stateEvent = this.GetStateEvent(eventName);
		if (stateEvent != null)
		{
			return stateEvent.m_Event;
		}
		return null;
	}

	// Token: 0x06001C29 RID: 7209 RVA: 0x00084AB7 File Offset: 0x00082CB7
	public string GetLastState()
	{
		return this.m_LastState;
	}

	// Token: 0x06001C2A RID: 7210 RVA: 0x00084ABF File Offset: 0x00082CBF
	public void AddStateEventStartListener(string eventName, StateEventTable.StateEventTrigger dlg, bool once = false)
	{
		this.AddStateEventListener((!once) ? this.m_StateEventStartListeners : this.m_StateEventStartOnceListeners, eventName, dlg);
	}

	// Token: 0x06001C2B RID: 7211 RVA: 0x00084AE0 File Offset: 0x00082CE0
	public void RemoveStateEventStartListener(string eventName, StateEventTable.StateEventTrigger dlg)
	{
		this.RemoveStateEventListener(this.m_StateEventStartListeners, eventName, dlg);
	}

	// Token: 0x06001C2C RID: 7212 RVA: 0x00084AF0 File Offset: 0x00082CF0
	public void AddStateEventEndListener(string eventName, StateEventTable.StateEventTrigger dlg, bool once = false)
	{
		this.AddStateEventListener((!once) ? this.m_StateEventEndListeners : this.m_StateEventEndOnceListeners, eventName, dlg);
	}

	// Token: 0x06001C2D RID: 7213 RVA: 0x00084B11 File Offset: 0x00082D11
	public void RemoveStateEventEndListener(string eventName, StateEventTable.StateEventTrigger dlg)
	{
		this.RemoveStateEventListener(this.m_StateEventEndListeners, eventName, dlg);
	}

	// Token: 0x06001C2E RID: 7214 RVA: 0x00084B24 File Offset: 0x00082D24
	public PlayMakerFSM GetFSMFromEvent(string evtName)
	{
		Spell spellEvent = this.GetSpellEvent(evtName);
		if (spellEvent != null)
		{
			return spellEvent.GetComponent<PlayMakerFSM>();
		}
		return null;
	}

	// Token: 0x06001C2F RID: 7215 RVA: 0x00084B50 File Offset: 0x00082D50
	public void SetFloatVar(string eventName, string varName, float value)
	{
		PlayMakerFSM fsmfromEvent = this.GetFSMFromEvent(eventName);
		if (fsmfromEvent == null)
		{
			return;
		}
		fsmfromEvent.FsmVariables.GetFsmFloat(varName).Value = value;
	}

	// Token: 0x06001C30 RID: 7216 RVA: 0x00084B84 File Offset: 0x00082D84
	public void SetIntVar(string eventName, string varName, int value)
	{
		PlayMakerFSM fsmfromEvent = this.GetFSMFromEvent(eventName);
		if (fsmfromEvent == null)
		{
			return;
		}
		fsmfromEvent.FsmVariables.GetFsmInt(varName).Value = value;
	}

	// Token: 0x06001C31 RID: 7217 RVA: 0x00084BB8 File Offset: 0x00082DB8
	public void SetBoolVar(string eventName, string varName, bool value)
	{
		PlayMakerFSM fsmfromEvent = this.GetFSMFromEvent(eventName);
		if (fsmfromEvent == null)
		{
			return;
		}
		fsmfromEvent.FsmVariables.GetFsmBool(varName).Value = value;
	}

	// Token: 0x06001C32 RID: 7218 RVA: 0x00084BEC File Offset: 0x00082DEC
	public void SetGameObjectVar(string eventName, string varName, GameObject value)
	{
		PlayMakerFSM fsmfromEvent = this.GetFSMFromEvent(eventName);
		if (fsmfromEvent == null)
		{
			return;
		}
		fsmfromEvent.FsmVariables.GetFsmGameObject(varName).Value = value;
	}

	// Token: 0x06001C33 RID: 7219 RVA: 0x00084C20 File Offset: 0x00082E20
	public void SetGameObjectVar(string eventName, string varName, Component value)
	{
		PlayMakerFSM fsmfromEvent = this.GetFSMFromEvent(eventName);
		if (fsmfromEvent == null)
		{
			return;
		}
		fsmfromEvent.FsmVariables.GetFsmGameObject(varName).Value = value.gameObject;
	}

	// Token: 0x06001C34 RID: 7220 RVA: 0x00084C5C File Offset: 0x00082E5C
	public void SetVector3Var(string eventName, string varName, Vector3 value)
	{
		PlayMakerFSM fsmfromEvent = this.GetFSMFromEvent(eventName);
		if (fsmfromEvent == null)
		{
			return;
		}
		fsmfromEvent.FsmVariables.GetFsmVector3(varName).Value = value;
	}

	// Token: 0x06001C35 RID: 7221 RVA: 0x00084C90 File Offset: 0x00082E90
	public void SetVar(string eventName, string varName, object value)
	{
		if (value is GameObject)
		{
			this.SetGameObjectVar(eventName, varName, (GameObject)value);
		}
		else if (value is Component)
		{
			this.SetGameObjectVar(eventName, varName, (Component)value);
		}
		else
		{
			Map<Type, Action> map = new Map<Type, Action>
			{
				{
					typeof(float),
					delegate()
					{
						this.SetFloatVar(eventName, varName, (float)value);
					}
				},
				{
					typeof(int),
					delegate()
					{
						this.SetIntVar(eventName, varName, (int)value);
					}
				},
				{
					typeof(bool),
					delegate()
					{
						this.SetBoolVar(eventName, varName, (bool)value);
					}
				}
			};
			Action action;
			if (map.TryGetValue(value.GetType(), out action))
			{
				action.Invoke();
			}
			else
			{
				Debug.LogError(string.Format("Set var type ({0}) not supported.", value.GetType()));
			}
		}
	}

	// Token: 0x06001C36 RID: 7222 RVA: 0x00084DBC File Offset: 0x00082FBC
	protected StateEventTable.StateEvent GetStateEvent(string eventName)
	{
		return this.m_Events.Find((StateEventTable.StateEvent e) => e.m_Name == eventName);
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x00084DF0 File Offset: 0x00082FF0
	private void StartNextQueuedState(StateEventTable.QueueStateEvent lastEvt)
	{
		if (this.m_QueuedEvents.Count == 0)
		{
			if (lastEvt != null)
			{
				this.FireStateEventFinishedEvent(this.m_StateEventEndListeners, lastEvt, false);
				this.FireStateEventFinishedEvent(this.m_StateEventEndOnceListeners, lastEvt, true);
			}
			return;
		}
		StateEventTable.QueueStateEvent queueStateEvent = this.m_QueuedEvents.Peek();
		StateEventTable.StateEvent stateEvent = queueStateEvent.m_StateEvent;
		if (queueStateEvent.m_SaveAsLastState)
		{
			this.m_LastState = queueStateEvent.GetEventName();
		}
		stateEvent.m_Event.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.QueueNextState), queueStateEvent);
		this.FireStateEventFinishedEvent(this.m_StateEventStartListeners, queueStateEvent, false);
		this.FireStateEventFinishedEvent(this.m_StateEventStartOnceListeners, queueStateEvent, true);
		stateEvent.m_Event.Activate();
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x00084E99 File Offset: 0x00083099
	private void QueueNextState(Spell spell, SpellStateType prevStateType, object thisStateEvent)
	{
		if (this.m_QueuedEvents.Count == 0)
		{
			return;
		}
		this.m_QueuedEvents.Dequeue();
		this.StartNextQueuedState((StateEventTable.QueueStateEvent)thisStateEvent);
	}

	// Token: 0x06001C39 RID: 7225 RVA: 0x00084EC4 File Offset: 0x000830C4
	private void AddStateEventListener(Map<string, List<StateEventTable.StateEventTrigger>> listenerDict, string eventName, StateEventTable.StateEventTrigger dlg)
	{
		List<StateEventTable.StateEventTrigger> list;
		if (!listenerDict.TryGetValue(eventName, out list))
		{
			list = new List<StateEventTable.StateEventTrigger>();
			listenerDict[eventName] = list;
		}
		list.Add(dlg);
	}

	// Token: 0x06001C3A RID: 7226 RVA: 0x00084EF4 File Offset: 0x000830F4
	private void RemoveStateEventListener(Map<string, List<StateEventTable.StateEventTrigger>> listenerDict, string eventName, StateEventTable.StateEventTrigger dlg)
	{
		List<StateEventTable.StateEventTrigger> list;
		if (listenerDict.TryGetValue(eventName, out list))
		{
			list.Remove(dlg);
		}
	}

	// Token: 0x06001C3B RID: 7227 RVA: 0x00084F18 File Offset: 0x00083118
	private void FireStateEventFinishedEvent(Map<string, List<StateEventTable.StateEventTrigger>> listenerDict, StateEventTable.QueueStateEvent stateEvt, bool clear = false)
	{
		List<StateEventTable.StateEventTrigger> list;
		if (!listenerDict.TryGetValue(stateEvt.GetEventName(), out list))
		{
			return;
		}
		StateEventTable.StateEventTrigger[] array = list.ToArray();
		foreach (StateEventTable.StateEventTrigger stateEventTrigger in array)
		{
			stateEventTrigger(stateEvt.m_StateEvent.m_Event);
		}
		if (clear)
		{
			list.Clear();
		}
	}

	// Token: 0x04000EC2 RID: 3778
	[CustomEditField(Sections = "Event Table", ListTable = true)]
	public List<StateEventTable.StateEvent> m_Events = new List<StateEventTable.StateEvent>();

	// Token: 0x04000EC3 RID: 3779
	private Map<string, List<StateEventTable.StateEventTrigger>> m_StateEventStartListeners = new Map<string, List<StateEventTable.StateEventTrigger>>();

	// Token: 0x04000EC4 RID: 3780
	private Map<string, List<StateEventTable.StateEventTrigger>> m_StateEventEndListeners = new Map<string, List<StateEventTable.StateEventTrigger>>();

	// Token: 0x04000EC5 RID: 3781
	private Map<string, List<StateEventTable.StateEventTrigger>> m_StateEventStartOnceListeners = new Map<string, List<StateEventTable.StateEventTrigger>>();

	// Token: 0x04000EC6 RID: 3782
	private Map<string, List<StateEventTable.StateEventTrigger>> m_StateEventEndOnceListeners = new Map<string, List<StateEventTable.StateEventTrigger>>();

	// Token: 0x04000EC7 RID: 3783
	private QueueList<StateEventTable.QueueStateEvent> m_QueuedEvents = new QueueList<StateEventTable.QueueStateEvent>();

	// Token: 0x04000EC8 RID: 3784
	private string m_LastState;

	// Token: 0x020001B8 RID: 440
	// (Invoke) Token: 0x06001CFC RID: 7420
	public delegate void StateEventTrigger(Spell evt);

	// Token: 0x020001B9 RID: 441
	[Serializable]
	public class StateEvent
	{
		// Token: 0x04000F24 RID: 3876
		public string m_Name;

		// Token: 0x04000F25 RID: 3877
		public Spell m_Event;
	}

	// Token: 0x020001BA RID: 442
	protected class QueueStateEvent
	{
		// Token: 0x06001D01 RID: 7425 RVA: 0x000882DC File Offset: 0x000864DC
		public string GetEventName()
		{
			return (!string.IsNullOrEmpty(this.m_NameOverride)) ? this.m_NameOverride : this.m_StateEvent.m_Name;
		}

		// Token: 0x04000F26 RID: 3878
		public StateEventTable.StateEvent m_StateEvent;

		// Token: 0x04000F27 RID: 3879
		public string m_NameOverride;

		// Token: 0x04000F28 RID: 3880
		public bool m_SaveAsLastState = true;
	}
}
