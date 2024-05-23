using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F26 RID: 3878
public class RandomPickPlaymakerEvent : MonoBehaviour
{
	// Token: 0x06007387 RID: 29575 RVA: 0x00220980 File Offset: 0x0021EB80
	private void Awake()
	{
		this.m_Collider = base.GetComponent<Collider>();
		if (this.m_AwakeStateIndex > -1)
		{
			this.m_CurrentState = this.m_State[this.m_AwakeStateIndex];
			this.m_LastEventIndex = this.m_AwakeStateIndex;
			this.m_StateActive = true;
		}
	}

	// Token: 0x06007388 RID: 29576 RVA: 0x002209D0 File Offset: 0x0021EBD0
	public void RandomPickEvent()
	{
		Log.Kyle.Print("RandomPickEvent {0} {1}", new object[]
		{
			this.m_StartAnimationFinished,
			this.m_EndAnimationFinished
		});
		if (!this.m_StartAnimationFinished || !this.m_EndAnimationFinished)
		{
			return;
		}
		Log.Kyle.Print("RandomPickEvent m_StateActive={0}", new object[]
		{
			this.m_StateActive
		});
		if (this.m_StateActive && this.m_CurrentState.m_EndEvent != string.Empty && this.m_CurrentState.m_FSM != null)
		{
			this.m_CurrentState.m_FSM.SendEvent(this.m_CurrentState.m_EndEvent);
			this.m_EndAnimationFinished = false;
			this.m_StateActive = false;
			base.StartCoroutine(this.WaitForEndAnimation());
			return;
		}
		if (this.m_AlternateState.Count > 0)
		{
			if (this.m_AlternateEventState)
			{
				this.SendRandomEvent();
			}
			else
			{
				this.SendAlternateRandomEvent();
			}
		}
		else
		{
			this.SendRandomEvent();
		}
	}

	// Token: 0x06007389 RID: 29577 RVA: 0x00220AF2 File Offset: 0x0021ECF2
	public void StartAnimationFinished()
	{
		this.m_StartAnimationFinished = true;
	}

	// Token: 0x0600738A RID: 29578 RVA: 0x00220AFB File Offset: 0x0021ECFB
	public void EndAnimationFinished()
	{
		this.m_EndAnimationFinished = true;
	}

	// Token: 0x0600738B RID: 29579 RVA: 0x00220B04 File Offset: 0x0021ED04
	private void SendRandomEvent()
	{
		this.m_StateActive = true;
		this.m_AlternateEventState = false;
		List<int> list = new List<int>();
		if (this.m_State.Count == 1)
		{
			list.Add(0);
		}
		else
		{
			for (int i = 0; i < this.m_State.Count; i++)
			{
				if (i != this.m_LastEventIndex)
				{
					list.Add(i);
				}
			}
		}
		int num = Random.Range(0, list.Count);
		RandomPickPlaymakerEvent.PickEvent pickEvent = this.m_State[list[num]];
		this.m_CurrentState = pickEvent;
		this.m_LastEventIndex = list[num];
		this.m_StartAnimationFinished = false;
		base.StartCoroutine(this.WaitForStartAnimation());
		pickEvent.m_FSM.SendEvent(pickEvent.m_StartEvent);
	}

	// Token: 0x0600738C RID: 29580 RVA: 0x00220BD0 File Offset: 0x0021EDD0
	private void SendAlternateRandomEvent()
	{
		this.m_StateActive = true;
		this.m_AlternateEventState = true;
		List<int> list = new List<int>();
		if (this.m_AlternateState.Count == 1)
		{
			list.Add(0);
		}
		else
		{
			for (int i = 0; i < this.m_AlternateState.Count; i++)
			{
				if (i != this.m_LastAlternateIndex)
				{
					list.Add(i);
				}
			}
		}
		int num = Random.Range(0, list.Count);
		RandomPickPlaymakerEvent.PickEvent pickEvent = this.m_AlternateState[list[num]];
		this.m_CurrentState = pickEvent;
		this.m_LastAlternateIndex = list[num];
		this.m_StartAnimationFinished = false;
		base.StartCoroutine(this.WaitForStartAnimation());
		pickEvent.m_FSM.SendEvent(pickEvent.m_StartEvent);
	}

	// Token: 0x0600738D RID: 29581 RVA: 0x00220C9C File Offset: 0x0021EE9C
	private IEnumerator WaitForStartAnimation()
	{
		while (!this.m_StartAnimationFinished)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600738E RID: 29582 RVA: 0x00220CB8 File Offset: 0x0021EEB8
	private IEnumerator WaitForEndAnimation()
	{
		while (!this.m_EndAnimationFinished)
		{
			yield return null;
		}
		this.m_CurrentState = null;
		if (!this.m_AllowNoneState)
		{
			while (!this.m_StartAnimationFinished)
			{
				yield return null;
			}
			this.RandomPickEvent();
		}
		yield break;
	}

	// Token: 0x0600738F RID: 29583 RVA: 0x00220CD3 File Offset: 0x0021EED3
	private void EnableCollider()
	{
		if (this.m_Collider != null)
		{
			this.m_Collider.enabled = true;
		}
	}

	// Token: 0x06007390 RID: 29584 RVA: 0x00220CF2 File Offset: 0x0021EEF2
	private void DisableCollider()
	{
		if (this.m_Collider != null)
		{
			this.m_Collider.enabled = false;
		}
	}

	// Token: 0x04005E19 RID: 24089
	public int m_AwakeStateIndex = -1;

	// Token: 0x04005E1A RID: 24090
	public bool m_AllowNoneState = true;

	// Token: 0x04005E1B RID: 24091
	public List<RandomPickPlaymakerEvent.PickEvent> m_State;

	// Token: 0x04005E1C RID: 24092
	public List<RandomPickPlaymakerEvent.PickEvent> m_AlternateState;

	// Token: 0x04005E1D RID: 24093
	private bool m_StateActive;

	// Token: 0x04005E1E RID: 24094
	private RandomPickPlaymakerEvent.PickEvent m_CurrentState;

	// Token: 0x04005E1F RID: 24095
	private Collider m_Collider;

	// Token: 0x04005E20 RID: 24096
	private bool m_AlternateEventState;

	// Token: 0x04005E21 RID: 24097
	private int m_LastEventIndex;

	// Token: 0x04005E22 RID: 24098
	private int m_LastAlternateIndex;

	// Token: 0x04005E23 RID: 24099
	private bool m_StartAnimationFinished = true;

	// Token: 0x04005E24 RID: 24100
	private bool m_EndAnimationFinished = true;

	// Token: 0x02000F27 RID: 3879
	[Serializable]
	public class PickEvent
	{
		// Token: 0x04005E25 RID: 24101
		public PlayMakerFSM m_FSM;

		// Token: 0x04005E26 RID: 24102
		public string m_StartEvent;

		// Token: 0x04005E27 RID: 24103
		public string m_EndEvent;

		// Token: 0x04005E28 RID: 24104
		[HideInInspector]
		public int m_CurrentItemIndex;
	}
}
