using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000331 RID: 817
public class ActorStateMgr : MonoBehaviour
{
	// Token: 0x06002A6C RID: 10860 RVA: 0x000D07F8 File Offset: 0x000CE9F8
	private void Start()
	{
		this.m_HighlightState = this.FindHightlightObject();
		this.BuildStateMap();
		if (this.m_activeStateType == ActorStateType.NONE)
		{
			this.HideImpl();
			return;
		}
		if (this.m_shown)
		{
			this.ShowImpl();
		}
		else
		{
			this.HideImpl();
		}
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x000D0845 File Offset: 0x000CEA45
	public Map<ActorStateType, List<ActorState>> GetStateMap()
	{
		return this.m_actorStateMap;
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x000D084D File Offset: 0x000CEA4D
	public ActorStateType GetActiveStateType()
	{
		return this.m_activeStateType;
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x000D0858 File Offset: 0x000CEA58
	public List<ActorState> GetActiveStateList()
	{
		List<ActorState> result = null;
		if (!this.m_actorStateMap.TryGetValue(this.m_activeStateType, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x06002A70 RID: 10864 RVA: 0x000D0884 File Offset: 0x000CEA84
	public float GetMaximumAnimationTimeOfActiveStates()
	{
		if (this.GetActiveStateList() == null)
		{
			return 0f;
		}
		float num = 0f;
		foreach (ActorState actorState in this.GetActiveStateList())
		{
			num = Mathf.Max(actorState.GetAnimationDuration(), num);
		}
		return num;
	}

	// Token: 0x06002A71 RID: 10865 RVA: 0x000D08FC File Offset: 0x000CEAFC
	public bool ChangeState(ActorStateType stateType)
	{
		return this.ChangeState_NewState(stateType) || this.ChangeState_LegacyState(stateType);
	}

	// Token: 0x06002A72 RID: 10866 RVA: 0x000D091C File Offset: 0x000CEB1C
	public bool ChangeState_NewState(ActorStateType stateType)
	{
		if (!this.m_HighlightState)
		{
			return false;
		}
		ActorStateType activeStateType = this.m_activeStateType;
		this.m_activeStateType = stateType;
		return activeStateType == ActorStateType.NONE || activeStateType == stateType || this.m_HighlightState.ChangeState(stateType);
	}

	// Token: 0x06002A73 RID: 10867 RVA: 0x000D0968 File Offset: 0x000CEB68
	public bool ChangeState_LegacyState(ActorStateType stateType)
	{
		List<ActorState> list = null;
		this.m_actorStateMap.TryGetValue(stateType, out list);
		ActorStateType activeStateType = this.m_activeStateType;
		this.m_activeStateType = stateType;
		if (activeStateType != ActorStateType.NONE)
		{
			List<ActorState> list2;
			if (this.m_actorStateMap.TryGetValue(activeStateType, out list2))
			{
				foreach (ActorState actorState in list2)
				{
					actorState.Stop(list);
				}
			}
		}
		else if (stateType != ActorStateType.NONE && this.m_ObjectContainer != null)
		{
			this.m_ObjectContainer.SetActive(true);
		}
		if (stateType == ActorStateType.NONE)
		{
			if (activeStateType != ActorStateType.NONE && this.m_ObjectContainer != null)
			{
				this.m_ObjectContainer.SetActive(false);
			}
			return true;
		}
		if (list != null)
		{
			foreach (ActorState actorState2 in list)
			{
				actorState2.Play();
			}
			return true;
		}
		return false;
	}

	// Token: 0x06002A74 RID: 10868 RVA: 0x000D0A9C File Offset: 0x000CEC9C
	public void ShowStateMgr()
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_shown = true;
		this.ShowImpl();
	}

	// Token: 0x06002A75 RID: 10869 RVA: 0x000D0AB7 File Offset: 0x000CECB7
	public void HideStateMgr()
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		this.HideImpl();
	}

	// Token: 0x06002A76 RID: 10870 RVA: 0x000D0AD2 File Offset: 0x000CECD2
	public void RefreshStateMgr()
	{
		if (this.m_HighlightState)
		{
			this.m_HighlightState.SetDirty();
		}
	}

	// Token: 0x06002A77 RID: 10871 RVA: 0x000D0AF0 File Offset: 0x000CECF0
	private HighlightState FindHightlightObject()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			HighlightState component = transform.gameObject.GetComponent<HighlightState>();
			if (component)
			{
				return component;
			}
		}
		return null;
	}

	// Token: 0x06002A78 RID: 10872 RVA: 0x000D0B70 File Offset: 0x000CED70
	private void BuildStateMap()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			ActorState component = transform.gameObject.GetComponent<ActorState>();
			if (!(component == null))
			{
				ActorStateType stateType = component.m_StateType;
				if (stateType != ActorStateType.NONE)
				{
					List<ActorState> list;
					if (!this.m_actorStateMap.TryGetValue(stateType, out list))
					{
						list = new List<ActorState>();
						this.m_actorStateMap.Add(stateType, list);
					}
					list.Add(component);
				}
			}
		}
	}

	// Token: 0x06002A79 RID: 10873 RVA: 0x000D0C30 File Offset: 0x000CEE30
	private void ShowImpl()
	{
		if (this.m_HighlightState)
		{
			this.m_HighlightState.ChangeState(this.m_activeStateType);
		}
		if (this.m_activeStateType != ActorStateType.NONE && this.m_ObjectContainer != null)
		{
			this.m_ObjectContainer.SetActive(true);
		}
		List<ActorState> activeStateList = this.GetActiveStateList();
		if (activeStateList != null)
		{
			foreach (ActorState actorState in activeStateList)
			{
				actorState.ShowState();
			}
		}
	}

	// Token: 0x06002A7A RID: 10874 RVA: 0x000D0CDC File Offset: 0x000CEEDC
	private void HideImpl()
	{
		if (this.m_HighlightState)
		{
			this.m_HighlightState.ChangeState(ActorStateType.NONE);
		}
		List<ActorState> activeStateList = this.GetActiveStateList();
		if (activeStateList != null)
		{
			foreach (ActorState actorState in activeStateList)
			{
				actorState.HideState();
			}
		}
		if (this.m_activeStateType != ActorStateType.NONE && this.m_ObjectContainer != null)
		{
			this.m_ObjectContainer.SetActive(false);
		}
	}

	// Token: 0x0400196A RID: 6506
	public GameObject m_ObjectContainer;

	// Token: 0x0400196B RID: 6507
	private Map<ActorStateType, List<ActorState>> m_actorStateMap = new Map<ActorStateType, List<ActorState>>();

	// Token: 0x0400196C RID: 6508
	private ActorStateType m_activeStateType;

	// Token: 0x0400196D RID: 6509
	private bool m_shown = true;

	// Token: 0x0400196E RID: 6510
	private HighlightState m_HighlightState;
}
