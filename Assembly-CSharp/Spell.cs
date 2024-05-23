using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class Spell : MonoBehaviour
{
	// Token: 0x06001679 RID: 5753 RVA: 0x0006A61C File Offset: 0x0006881C
	protected virtual void Awake()
	{
		this.BuildSpellStateMap();
		this.m_fsm = base.GetComponent<PlayMakerFSM>();
		if (!string.IsNullOrEmpty(this.m_LocationTransformName))
		{
			this.m_LocationTransformName = this.m_LocationTransformName.Trim();
		}
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x0006A65C File Offset: 0x0006885C
	protected virtual void OnDestroy()
	{
		if (base.gameObject != null)
		{
			Object.Destroy(base.gameObject);
		}
		if (this.m_ObjectContainer != null)
		{
			Object.Destroy(this.m_ObjectContainer);
			this.m_ObjectContainer = null;
		}
	}

	// Token: 0x0600167B RID: 5755 RVA: 0x0006A6A8 File Offset: 0x000688A8
	protected virtual void Start()
	{
		if (this.m_activeStateType == SpellStateType.NONE)
		{
			this.ActivateObjectContainer(false);
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

	// Token: 0x0600167C RID: 5756 RVA: 0x0006A6DC File Offset: 0x000688DC
	private void Update()
	{
		if (this.m_fsmReady)
		{
			return;
		}
		if (this.m_fsm == null)
		{
			this.m_fsmReady = true;
			return;
		}
		if (!this.m_fsmSkippedFirstFrame)
		{
			this.m_fsmSkippedFirstFrame = true;
			return;
		}
		if (!this.m_fsm.enabled)
		{
			return;
		}
		this.BuildFsmStateMap();
		this.m_fsmReady = true;
	}

	// Token: 0x0600167D RID: 5757 RVA: 0x0006A73F File Offset: 0x0006893F
	public SpellType GetSpellType()
	{
		return this.m_spellType;
	}

	// Token: 0x0600167E RID: 5758 RVA: 0x0006A747 File Offset: 0x00068947
	public void SetSpellType(SpellType spellType)
	{
		this.m_spellType = spellType;
	}

	// Token: 0x0600167F RID: 5759 RVA: 0x0006A750 File Offset: 0x00068950
	public bool DoesBlockServerEvents()
	{
		return GameState.Get() != null && this.m_BlockServerEvents;
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x0006A764 File Offset: 0x00068964
	public SuperSpell GetSuperSpellParent()
	{
		if (base.transform.parent == null)
		{
			return null;
		}
		return base.transform.parent.GetComponent<SuperSpell>();
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x0006A799 File Offset: 0x00068999
	public PowerTaskList GetPowerTaskList()
	{
		return this.m_taskList;
	}

	// Token: 0x06001682 RID: 5762 RVA: 0x0006A7A1 File Offset: 0x000689A1
	public Entity GetPowerSource()
	{
		if (this.m_taskList == null)
		{
			return null;
		}
		return this.m_taskList.GetSourceEntity();
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x0006A7BC File Offset: 0x000689BC
	public Card GetPowerSourceCard()
	{
		Entity powerSource = this.GetPowerSource();
		return (powerSource != null) ? powerSource.GetCard() : null;
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x0006A7E2 File Offset: 0x000689E2
	public Entity GetPowerTarget()
	{
		if (this.m_taskList == null)
		{
			return null;
		}
		return this.m_taskList.GetTargetEntity();
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x0006A7FC File Offset: 0x000689FC
	public Card GetPowerTargetCard()
	{
		Entity powerTarget = this.GetPowerTarget();
		return (powerTarget != null) ? powerTarget.GetCard() : null;
	}

	// Token: 0x06001686 RID: 5766 RVA: 0x0006A822 File Offset: 0x00068A22
	public virtual bool CanPurge()
	{
		return !this.IsActive();
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x0006A82D File Offset: 0x00068A2D
	public SpellLocation GetLocation()
	{
		return this.m_Location;
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x0006A835 File Offset: 0x00068A35
	public string GetLocationTransformName()
	{
		return this.m_LocationTransformName;
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x0006A83D File Offset: 0x00068A3D
	public SpellFacing GetFacing()
	{
		return this.m_Facing;
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x0006A845 File Offset: 0x00068A45
	public SpellFacingOptions GetFacingOptions()
	{
		return this.m_FacingOptions;
	}

	// Token: 0x0600168B RID: 5771 RVA: 0x0006A84D File Offset: 0x00068A4D
	public void SetPosition(Vector3 position)
	{
		base.transform.position = position;
		this.m_positionDirty = false;
	}

	// Token: 0x0600168C RID: 5772 RVA: 0x0006A862 File Offset: 0x00068A62
	public void SetLocalPosition(Vector3 position)
	{
		base.transform.localPosition = position;
		this.m_positionDirty = false;
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x0006A877 File Offset: 0x00068A77
	public void SetOrientation(Quaternion orientation)
	{
		base.transform.rotation = orientation;
		this.m_orientationDirty = false;
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x0006A88C File Offset: 0x00068A8C
	public void SetLocalOrientation(Quaternion orientation)
	{
		base.transform.localRotation = orientation;
		this.m_orientationDirty = false;
	}

	// Token: 0x0600168F RID: 5775 RVA: 0x0006A8A1 File Offset: 0x00068AA1
	public void UpdateTransform()
	{
		this.UpdatePosition();
		this.UpdateOrientation();
	}

	// Token: 0x06001690 RID: 5776 RVA: 0x0006A8B0 File Offset: 0x00068AB0
	public void UpdatePosition()
	{
		if (!this.m_positionDirty)
		{
			return;
		}
		SpellUtils.SetPositionFromLocation(this, this.m_SetParentToLocation);
		this.m_positionDirty = false;
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x0006A8DD File Offset: 0x00068ADD
	public void UpdateOrientation()
	{
		if (!this.m_orientationDirty)
		{
			return;
		}
		SpellUtils.SetOrientationFromFacing(this);
		this.m_orientationDirty = false;
	}

	// Token: 0x06001692 RID: 5778 RVA: 0x0006A8F9 File Offset: 0x00068AF9
	public GameObject GetSource()
	{
		return this.m_source;
	}

	// Token: 0x06001693 RID: 5779 RVA: 0x0006A901 File Offset: 0x00068B01
	public virtual void SetSource(GameObject go)
	{
		this.m_source = go;
	}

	// Token: 0x06001694 RID: 5780 RVA: 0x0006A90A File Offset: 0x00068B0A
	public virtual void RemoveSource()
	{
		this.m_source = null;
	}

	// Token: 0x06001695 RID: 5781 RVA: 0x0006A913 File Offset: 0x00068B13
	public bool IsSource(GameObject go)
	{
		return this.m_source == go;
	}

	// Token: 0x06001696 RID: 5782 RVA: 0x0006A921 File Offset: 0x00068B21
	public Card GetSourceCard()
	{
		if (this.m_source == null)
		{
			return null;
		}
		return this.m_source.GetComponent<Card>();
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x0006A941 File Offset: 0x00068B41
	public List<GameObject> GetTargets()
	{
		return this.m_targets;
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x0006A94C File Offset: 0x00068B4C
	public GameObject GetTarget()
	{
		return (this.m_targets.Count != 0) ? this.m_targets[0] : null;
	}

	// Token: 0x06001699 RID: 5785 RVA: 0x0006A97B File Offset: 0x00068B7B
	public virtual void AddTarget(GameObject go)
	{
		this.m_targets.Add(go);
	}

	// Token: 0x0600169A RID: 5786 RVA: 0x0006A989 File Offset: 0x00068B89
	public virtual void AddTargets(List<GameObject> targets)
	{
		this.m_targets.AddRange(targets);
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x0006A997 File Offset: 0x00068B97
	public virtual bool RemoveTarget(GameObject go)
	{
		return this.m_targets.Remove(go);
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x0006A9A5 File Offset: 0x00068BA5
	public virtual void RemoveAllTargets()
	{
		this.m_targets.Clear();
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x0006A9B2 File Offset: 0x00068BB2
	public bool IsTarget(GameObject go)
	{
		return this.m_targets.Contains(go);
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x0006A9C0 File Offset: 0x00068BC0
	public Card GetTargetCard()
	{
		GameObject target = this.GetTarget();
		if (target == null)
		{
			return null;
		}
		return target.GetComponent<Card>();
	}

	// Token: 0x0600169F RID: 5791 RVA: 0x0006A9E8 File Offset: 0x00068BE8
	public virtual List<GameObject> GetVisualTargets()
	{
		return this.GetTargets();
	}

	// Token: 0x060016A0 RID: 5792 RVA: 0x0006A9F0 File Offset: 0x00068BF0
	public virtual GameObject GetVisualTarget()
	{
		return this.GetTarget();
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x0006A9F8 File Offset: 0x00068BF8
	public virtual void AddVisualTarget(GameObject go)
	{
		this.AddTarget(go);
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x0006AA01 File Offset: 0x00068C01
	public virtual void AddVisualTargets(List<GameObject> targets)
	{
		this.AddTargets(targets);
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x0006AA0A File Offset: 0x00068C0A
	public virtual bool RemoveVisualTarget(GameObject go)
	{
		return this.RemoveTarget(go);
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x0006AA13 File Offset: 0x00068C13
	public virtual void RemoveAllVisualTargets()
	{
		this.RemoveAllTargets();
	}

	// Token: 0x060016A5 RID: 5797 RVA: 0x0006AA1B File Offset: 0x00068C1B
	public virtual bool IsVisualTarget(GameObject go)
	{
		return this.IsTarget(go);
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x0006AA24 File Offset: 0x00068C24
	public virtual Card GetVisualTargetCard()
	{
		return this.GetTargetCard();
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x0006AA2C File Offset: 0x00068C2C
	public bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x0006AA34 File Offset: 0x00068C34
	public void Show()
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_shown = true;
		if (this.m_activeStateType != SpellStateType.NONE)
		{
			this.OnExitedNoneState();
		}
		this.ShowImpl();
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x0006AA6C File Offset: 0x00068C6C
	public void Hide()
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		this.HideImpl();
		if (this.m_activeStateType != SpellStateType.NONE)
		{
			this.OnEnteredNoneState();
		}
	}

	// Token: 0x060016AA RID: 5802 RVA: 0x0006AAA3 File Offset: 0x00068CA3
	public void ActivateObjectContainer(bool enable)
	{
		if (this.m_ObjectContainer == null)
		{
			return;
		}
		SceneUtils.EnableRenderers(this.m_ObjectContainer, enable);
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x0006AAC3 File Offset: 0x00068CC3
	public bool IsActive()
	{
		return this.m_activeStateType != SpellStateType.NONE;
	}

	// Token: 0x060016AC RID: 5804 RVA: 0x0006AAD4 File Offset: 0x00068CD4
	public void Activate()
	{
		SpellStateType spellStateType = this.GuessNextStateType();
		if (spellStateType == SpellStateType.NONE)
		{
			this.Deactivate();
			return;
		}
		this.ChangeState(spellStateType);
	}

	// Token: 0x060016AD RID: 5805 RVA: 0x0006AAFC File Offset: 0x00068CFC
	public void Reactivate()
	{
		SpellStateType spellStateType = this.GuessNextStateType(SpellStateType.NONE);
		if (spellStateType == SpellStateType.NONE)
		{
			this.Deactivate();
			return;
		}
		this.ChangeState(spellStateType);
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x0006AB25 File Offset: 0x00068D25
	public void Deactivate()
	{
		if (this.m_activeStateType == SpellStateType.NONE)
		{
			return;
		}
		this.ForceDeactivate();
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x0006AB39 File Offset: 0x00068D39
	public void ForceDeactivate()
	{
		this.ChangeState(SpellStateType.NONE);
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x0006AB42 File Offset: 0x00068D42
	public void ActivateState(SpellStateType stateType)
	{
		if (!this.HasUsableState(stateType))
		{
			this.Deactivate();
			return;
		}
		this.ChangeState(stateType);
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x0006AB5E File Offset: 0x00068D5E
	public void SafeActivateState(SpellStateType stateType)
	{
		if (!this.HasUsableState(stateType))
		{
			this.ForceDeactivate();
			return;
		}
		this.ChangeState(stateType);
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x0006AB7C File Offset: 0x00068D7C
	public virtual bool HasUsableState(SpellStateType stateType)
	{
		return stateType != SpellStateType.NONE && (this.HasStateContent(stateType) || this.HasOverriddenStateMethod(stateType) || (this.m_activeStateType == SpellStateType.NONE && this.m_ZonesToDisable != null && this.m_ZonesToDisable.Count > 0));
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x0006ABD7 File Offset: 0x00068DD7
	public SpellStateType GetActiveState()
	{
		return this.m_activeStateType;
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x0006ABE0 File Offset: 0x00068DE0
	public SpellState GetFirstSpellState(SpellStateType stateType)
	{
		if (this.m_spellStateMap == null)
		{
			return null;
		}
		List<SpellState> list = null;
		if (!this.m_spellStateMap.TryGetValue(stateType, out list))
		{
			return null;
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list[0];
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x0006AC28 File Offset: 0x00068E28
	public List<SpellState> GetActiveStateList()
	{
		if (this.m_spellStateMap == null)
		{
			return null;
		}
		List<SpellState> result = null;
		if (!this.m_spellStateMap.TryGetValue(this.m_activeStateType, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x0006AC5F File Offset: 0x00068E5F
	public bool IsFinished()
	{
		return this.m_finished;
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x0006AC67 File Offset: 0x00068E67
	public void AddFinishedCallback(Spell.FinishedCallback callback)
	{
		this.AddFinishedCallback(callback, null);
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x0006AC74 File Offset: 0x00068E74
	public void AddFinishedCallback(Spell.FinishedCallback callback, object userData)
	{
		Spell.FinishedListener finishedListener = new Spell.FinishedListener();
		finishedListener.SetCallback(callback);
		finishedListener.SetUserData(userData);
		if (this.m_finishedListeners.Contains(finishedListener))
		{
			return;
		}
		this.m_finishedListeners.Add(finishedListener);
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x0006ACB3 File Offset: 0x00068EB3
	public bool RemoveFinishedCallback(Spell.FinishedCallback callback)
	{
		return this.RemoveFinishedCallback(callback, null);
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x0006ACC0 File Offset: 0x00068EC0
	public bool RemoveFinishedCallback(Spell.FinishedCallback callback, object userData)
	{
		Spell.FinishedListener finishedListener = new Spell.FinishedListener();
		finishedListener.SetCallback(callback);
		finishedListener.SetUserData(userData);
		return this.m_finishedListeners.Remove(finishedListener);
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x0006ACED File Offset: 0x00068EED
	public void AddStateFinishedCallback(Spell.StateFinishedCallback callback)
	{
		this.AddStateFinishedCallback(callback, null);
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x0006ACF8 File Offset: 0x00068EF8
	public void AddStateFinishedCallback(Spell.StateFinishedCallback callback, object userData)
	{
		Spell.StateFinishedListener stateFinishedListener = new Spell.StateFinishedListener();
		stateFinishedListener.SetCallback(callback);
		stateFinishedListener.SetUserData(userData);
		if (this.m_stateFinishedListeners.Contains(stateFinishedListener))
		{
			return;
		}
		this.m_stateFinishedListeners.Add(stateFinishedListener);
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x0006AD37 File Offset: 0x00068F37
	public bool RemoveStateFinishedCallback(Spell.StateFinishedCallback callback)
	{
		return this.RemoveStateFinishedCallback(callback, null);
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x0006AD44 File Offset: 0x00068F44
	public bool RemoveStateFinishedCallback(Spell.StateFinishedCallback callback, object userData)
	{
		Spell.StateFinishedListener stateFinishedListener = new Spell.StateFinishedListener();
		stateFinishedListener.SetCallback(callback);
		stateFinishedListener.SetUserData(userData);
		return this.m_stateFinishedListeners.Remove(stateFinishedListener);
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x0006AD71 File Offset: 0x00068F71
	public void AddStateStartedCallback(Spell.StateStartedCallback callback)
	{
		this.AddStateStartedCallback(callback, null);
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x0006AD7C File Offset: 0x00068F7C
	public void AddStateStartedCallback(Spell.StateStartedCallback callback, object userData)
	{
		Spell.StateStartedListener stateStartedListener = new Spell.StateStartedListener();
		stateStartedListener.SetCallback(callback);
		stateStartedListener.SetUserData(userData);
		if (this.m_stateStartedListeners.Contains(stateStartedListener))
		{
			return;
		}
		this.m_stateStartedListeners.Add(stateStartedListener);
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x0006ADBB File Offset: 0x00068FBB
	public bool RemoveStateStartedCallback(Spell.StateStartedCallback callback)
	{
		return this.RemoveStateStartedCallback(callback, null);
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x0006ADC8 File Offset: 0x00068FC8
	public bool RemoveStateStartedCallback(Spell.StateStartedCallback callback, object userData)
	{
		Spell.StateStartedListener stateStartedListener = new Spell.StateStartedListener();
		stateStartedListener.SetCallback(callback);
		stateStartedListener.SetUserData(userData);
		return this.m_stateStartedListeners.Remove(stateStartedListener);
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x0006ADF5 File Offset: 0x00068FF5
	public void AddSpellEventCallback(Spell.SpellEventCallback callback)
	{
		this.AddSpellEventCallback(callback, null);
	}

	// Token: 0x060016C4 RID: 5828 RVA: 0x0006AE00 File Offset: 0x00069000
	public void AddSpellEventCallback(Spell.SpellEventCallback callback, object userData)
	{
		Spell.SpellEventListener spellEventListener = new Spell.SpellEventListener();
		spellEventListener.SetCallback(callback);
		spellEventListener.SetUserData(userData);
		if (this.m_spellEventListeners.Contains(spellEventListener))
		{
			return;
		}
		this.m_spellEventListeners.Add(spellEventListener);
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x0006AE3F File Offset: 0x0006903F
	public bool RemoveSpellEventCallback(Spell.SpellEventCallback callback)
	{
		return this.RemoveSpellEventCallback(callback, null);
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x0006AE4C File Offset: 0x0006904C
	public bool RemoveSpellEventCallback(Spell.SpellEventCallback callback, object userData)
	{
		Spell.SpellEventListener spellEventListener = new Spell.SpellEventListener();
		spellEventListener.SetCallback(callback);
		spellEventListener.SetUserData(userData);
		return this.m_spellEventListeners.Remove(spellEventListener);
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x0006AE79 File Offset: 0x00069079
	public virtual void ChangeState(SpellStateType stateType)
	{
		this.ChangeStateImpl(stateType);
		if (this.m_activeStateType != stateType)
		{
			return;
		}
		this.ChangeFsmState(stateType);
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x0006AE96 File Offset: 0x00069096
	public SpellStateType GuessNextStateType()
	{
		return this.GuessNextStateType(this.m_activeStateType);
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x0006AEA4 File Offset: 0x000690A4
	public SpellStateType GuessNextStateType(SpellStateType stateType)
	{
		switch (stateType)
		{
		case SpellStateType.NONE:
			if (this.HasUsableState(SpellStateType.BIRTH))
			{
				return SpellStateType.BIRTH;
			}
			if (this.HasUsableState(SpellStateType.IDLE))
			{
				return SpellStateType.IDLE;
			}
			if (this.HasUsableState(SpellStateType.ACTION))
			{
				return SpellStateType.ACTION;
			}
			if (this.HasUsableState(SpellStateType.DEATH))
			{
				return SpellStateType.DEATH;
			}
			if (this.HasUsableState(SpellStateType.CANCEL))
			{
				return SpellStateType.CANCEL;
			}
			break;
		case SpellStateType.BIRTH:
			if (this.HasUsableState(SpellStateType.IDLE))
			{
				return SpellStateType.IDLE;
			}
			break;
		case SpellStateType.IDLE:
			if (this.HasUsableState(SpellStateType.ACTION))
			{
				return SpellStateType.ACTION;
			}
			break;
		case SpellStateType.ACTION:
			if (this.HasUsableState(SpellStateType.DEATH))
			{
				return SpellStateType.DEATH;
			}
			break;
		}
		return SpellStateType.NONE;
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x0006AF58 File Offset: 0x00069158
	public bool AttachPowerTaskList(PowerTaskList taskList)
	{
		this.m_taskList = taskList;
		this.RemoveAllTargets();
		if (!this.AddPowerTargets())
		{
			return false;
		}
		this.OnAttachPowerTaskList();
		return true;
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x0006AF7B File Offset: 0x0006917B
	public virtual bool AddPowerTargets()
	{
		return this.CanAddPowerTargets() && this.AddMultiplePowerTargets();
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x0006AF90 File Offset: 0x00069190
	public PowerTaskList GetLastHandledTaskList(PowerTaskList taskList)
	{
		if (taskList == null)
		{
			return null;
		}
		Spell spell = Object.Instantiate<Spell>(this);
		PowerTaskList result = null;
		for (PowerTaskList powerTaskList = taskList.GetLast(); powerTaskList != null; powerTaskList = powerTaskList.GetPrevious())
		{
			spell.m_taskList = powerTaskList;
			spell.RemoveAllTargets();
			if (spell.AddPowerTargets())
			{
				result = powerTaskList;
				break;
			}
		}
		Object.DestroyImmediate(spell);
		return result;
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x0006AFF4 File Offset: 0x000691F4
	public bool IsHandlingLastTaskList()
	{
		PowerTaskList lastHandledTaskList = this.GetLastHandledTaskList(this.m_taskList);
		return lastHandledTaskList == this.m_taskList;
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x0006B018 File Offset: 0x00069218
	public virtual void OnStateFinished()
	{
		SpellStateType stateType = this.GuessNextStateType();
		this.ChangeState(stateType);
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x0006B034 File Offset: 0x00069234
	public virtual void OnSpellFinished()
	{
		this.m_finished = true;
		this.m_positionDirty = true;
		this.m_orientationDirty = true;
		if (this.DoesBlockServerEvents())
		{
			GameState.Get().RemoveServerBlockingSpell(this);
		}
		this.BlockZones(false);
		if (this.m_UseFastActorTriggers && GameState.Get() != null && this.IsHandlingLastTaskList())
		{
			GameState.Get().SetUsingFastActorTriggers(false);
		}
		this.FireFinishedCallbacks();
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x0006B0A5 File Offset: 0x000692A5
	public virtual void OnSpellEvent(string eventName, object eventData)
	{
		this.FireSpellEventCallbacks(eventName, eventData);
	}

	// Token: 0x060016D1 RID: 5841 RVA: 0x0006B0AF File Offset: 0x000692AF
	public virtual void OnFsmStateStarted(FsmState state, SpellStateType stateType)
	{
		if (this.m_activeStateChange == stateType)
		{
			return;
		}
		this.ChangeStateImpl(stateType);
	}

	// Token: 0x060016D2 RID: 5842 RVA: 0x0006B0C8 File Offset: 0x000692C8
	protected virtual void OnAttachPowerTaskList()
	{
		if (this.m_UseFastActorTriggers && this.m_taskList.IsStartOfBlock())
		{
			GameState.Get().SetUsingFastActorTriggers(true);
		}
	}

	// Token: 0x060016D3 RID: 5843 RVA: 0x0006B0FB File Offset: 0x000692FB
	protected virtual void OnBirth(SpellStateType prevStateType)
	{
		this.UpdateTransform();
		this.FireStateStartedCallbacks(prevStateType);
	}

	// Token: 0x060016D4 RID: 5844 RVA: 0x0006B10A File Offset: 0x0006930A
	protected virtual void OnIdle(SpellStateType prevStateType)
	{
		this.FireStateStartedCallbacks(prevStateType);
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x0006B113 File Offset: 0x00069313
	protected virtual void OnAction(SpellStateType prevStateType)
	{
		this.UpdateTransform();
		this.FireStateStartedCallbacks(prevStateType);
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x0006B122 File Offset: 0x00069322
	protected virtual void OnCancel(SpellStateType prevStateType)
	{
		this.FireStateStartedCallbacks(prevStateType);
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x0006B12B File Offset: 0x0006932B
	protected virtual void OnDeath(SpellStateType prevStateType)
	{
		this.FireStateStartedCallbacks(prevStateType);
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x0006B134 File Offset: 0x00069334
	protected virtual void OnNone(SpellStateType prevStateType)
	{
		this.FireStateStartedCallbacks(prevStateType);
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x0006B140 File Offset: 0x00069340
	private void BuildSpellStateMap()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			SpellState component = transform.gameObject.GetComponent<SpellState>();
			if (!(component == null))
			{
				SpellStateType stateType = component.m_StateType;
				if (stateType != SpellStateType.NONE)
				{
					if (this.m_spellStateMap == null)
					{
						this.m_spellStateMap = new Map<SpellStateType, List<SpellState>>();
					}
					List<SpellState> list;
					if (!this.m_spellStateMap.TryGetValue(stateType, out list))
					{
						list = new List<SpellState>();
						this.m_spellStateMap.Add(stateType, list);
					}
					list.Add(component);
				}
			}
		}
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x0006B214 File Offset: 0x00069414
	private void BuildFsmStateMap()
	{
		if (this.m_fsm == null)
		{
			return;
		}
		List<FsmState> list = this.GenerateSpellFsmStateList();
		if (list.Count > 0)
		{
			this.m_fsmStateMap = new Map<SpellStateType, FsmState>();
		}
		Map<SpellStateType, int> map = new Map<SpellStateType, int>();
		foreach (object obj in Enum.GetValues(typeof(SpellStateType)))
		{
			SpellStateType key = (SpellStateType)((int)obj);
			map[key] = 0;
		}
		Map<SpellStateType, int> map2 = new Map<SpellStateType, int>();
		foreach (object obj2 in Enum.GetValues(typeof(SpellStateType)))
		{
			SpellStateType key2 = (SpellStateType)((int)obj2);
			map2[key2] = 0;
		}
		FsmTransition[] fsmGlobalTransitions = this.m_fsm.FsmGlobalTransitions;
		int i = 0;
		while (i < fsmGlobalTransitions.Length)
		{
			FsmTransition fsmTransition = fsmGlobalTransitions[i];
			SpellStateType @enum;
			try
			{
				@enum = EnumUtils.GetEnum<SpellStateType>(fsmTransition.EventName);
			}
			catch (ArgumentException)
			{
				goto IL_1D6;
			}
			goto IL_124;
			IL_1D6:
			i++;
			continue;
			IL_124:
			Map<SpellStateType, int> map4;
			Map<SpellStateType, int> map3 = map4 = map2;
			SpellStateType key4;
			SpellStateType key3 = key4 = @enum;
			int num = map4[key4];
			map3[key3] = num + 1;
			foreach (FsmState fsmState in list)
			{
				if (fsmTransition.ToState.Equals(fsmState.Name))
				{
					Map<SpellStateType, int> map6;
					Map<SpellStateType, int> map5 = map6 = map;
					SpellStateType key5 = key4 = @enum;
					num = map6[key4];
					map5[key5] = num + 1;
					if (!this.m_fsmStateMap.ContainsKey(@enum))
					{
						this.m_fsmStateMap.Add(@enum, fsmState);
					}
				}
			}
			goto IL_1D6;
		}
		foreach (KeyValuePair<SpellStateType, int> keyValuePair in map)
		{
			if (keyValuePair.Value > 1)
			{
				Debug.LogWarning(string.Format("{0}.BuildFsmStateMap() - Found {1} states for SpellStateType {2}. There should be 1.", this, keyValuePair.Value, keyValuePair.Key));
			}
		}
		foreach (KeyValuePair<SpellStateType, int> keyValuePair2 in map2)
		{
			if (keyValuePair2.Value > 1)
			{
				Debug.LogWarning(string.Format("{0}.BuildFsmStateMap() - Found {1} transitions for SpellStateType {2}. There should be 1.", this, keyValuePair2.Value, keyValuePair2.Key));
			}
			if (keyValuePair2.Value > 0 && map[keyValuePair2.Key] == 0)
			{
				Debug.LogWarning(string.Format("{0}.BuildFsmStateMap() - SpellStateType {1} is missing a SpellStateAction.", this, keyValuePair2.Key));
			}
		}
		if (this.m_fsmStateMap != null && this.m_fsmStateMap.Values.Count == 0)
		{
			this.m_fsmStateMap = null;
		}
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x0006B58C File Offset: 0x0006978C
	private List<FsmState> GenerateSpellFsmStateList()
	{
		List<FsmState> list = new List<FsmState>();
		foreach (FsmState fsmState in this.m_fsm.FsmStates)
		{
			SpellStateAction spellStateAction = null;
			int num = 0;
			for (int j = 0; j < fsmState.Actions.Length; j++)
			{
				FsmStateAction fsmStateAction = fsmState.Actions[j];
				SpellStateAction spellStateAction2 = fsmStateAction as SpellStateAction;
				if (spellStateAction2 != null)
				{
					num++;
					if (spellStateAction == null)
					{
						spellStateAction = spellStateAction2;
					}
				}
			}
			if (spellStateAction != null)
			{
				list.Add(fsmState);
			}
			if (num > 1)
			{
				string text = string.Format("{0}.GenerateSpellFsmStateList() - State \"{1}\" has {2} SpellStateActions. There should be 1.", this, fsmState.Name, num);
				Debug.LogWarning(text);
			}
		}
		return list;
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x0006B650 File Offset: 0x00069850
	protected void ChangeStateImpl(SpellStateType stateType)
	{
		this.m_activeStateChange = stateType;
		SpellStateType activeStateType = this.m_activeStateType;
		this.m_activeStateType = stateType;
		if (stateType == SpellStateType.NONE)
		{
			this.FinishIfNecessary();
		}
		List<SpellState> list = null;
		if (this.m_spellStateMap != null)
		{
			this.m_spellStateMap.TryGetValue(stateType, out list);
		}
		if (activeStateType != SpellStateType.NONE)
		{
			List<SpellState> list2;
			if (this.m_spellStateMap != null && this.m_spellStateMap.TryGetValue(activeStateType, out list2))
			{
				foreach (SpellState spellState in list2)
				{
					spellState.Stop(list);
				}
			}
			this.FireStateFinishedCallbacks(activeStateType);
		}
		else if (stateType != SpellStateType.NONE)
		{
			this.m_finished = false;
			this.OnExitedNoneState();
		}
		if (list != null)
		{
			foreach (SpellState spellState2 in list)
			{
				spellState2.Play();
			}
		}
		this.CallStateFunction(activeStateType, stateType);
		if (activeStateType != SpellStateType.NONE && stateType == SpellStateType.NONE)
		{
			this.OnEnteredNoneState();
		}
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x0006B790 File Offset: 0x00069990
	protected void ChangeFsmState(SpellStateType stateType)
	{
		if (this.m_fsm == null)
		{
			return;
		}
		base.StartCoroutine(this.WaitThenChangeFsmState(stateType));
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x0006B7C0 File Offset: 0x000699C0
	private IEnumerator WaitThenChangeFsmState(SpellStateType stateType)
	{
		while (!this.m_fsmReady)
		{
			yield return null;
		}
		if (this.m_activeStateType != stateType)
		{
			yield break;
		}
		this.ChangeFsmStateNow(stateType);
		yield break;
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x0006B7EC File Offset: 0x000699EC
	private void ChangeFsmStateNow(SpellStateType stateType)
	{
		if (this.m_fsmStateMap == null)
		{
			Debug.LogError(string.Format("Spell.ChangeFsmStateNow() - stateType {0} was requested but the m_fsmStateMap is null", stateType));
			return;
		}
		FsmState fsmState = null;
		if (!this.m_fsmStateMap.TryGetValue(stateType, out fsmState))
		{
			return;
		}
		this.m_fsm.SendEvent(EnumUtils.GetString<SpellStateType>(stateType));
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x0006B841 File Offset: 0x00069A41
	protected void FinishIfNecessary()
	{
		if (this.m_finished)
		{
			return;
		}
		this.OnSpellFinished();
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x0006B858 File Offset: 0x00069A58
	protected void CallStateFunction(SpellStateType prevStateType, SpellStateType stateType)
	{
		switch (stateType)
		{
		case SpellStateType.BIRTH:
			this.OnBirth(prevStateType);
			break;
		case SpellStateType.IDLE:
			this.OnIdle(prevStateType);
			break;
		case SpellStateType.ACTION:
			this.OnAction(prevStateType);
			break;
		case SpellStateType.CANCEL:
			this.OnCancel(prevStateType);
			break;
		case SpellStateType.DEATH:
			this.OnDeath(prevStateType);
			break;
		default:
			this.OnNone(prevStateType);
			break;
		}
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x0006B8D0 File Offset: 0x00069AD0
	protected void FireFinishedCallbacks()
	{
		Spell.FinishedListener[] array = this.m_finishedListeners.ToArray();
		this.m_finishedListeners.Clear();
		foreach (Spell.FinishedListener finishedListener in array)
		{
			finishedListener.Fire(this);
		}
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x0006B918 File Offset: 0x00069B18
	protected void FireStateFinishedCallbacks(SpellStateType prevStateType)
	{
		Spell.StateFinishedListener[] array = this.m_stateFinishedListeners.ToArray();
		if (this.m_activeStateType == SpellStateType.NONE)
		{
			this.m_stateFinishedListeners.Clear();
		}
		foreach (Spell.StateFinishedListener stateFinishedListener in array)
		{
			stateFinishedListener.Fire(this, prevStateType);
		}
	}

	// Token: 0x060016E4 RID: 5860 RVA: 0x0006B96C File Offset: 0x00069B6C
	protected void FireStateStartedCallbacks(SpellStateType prevStateType)
	{
		Spell.StateStartedListener[] array = this.m_stateStartedListeners.ToArray();
		if (this.m_activeStateType == SpellStateType.NONE)
		{
			this.m_stateStartedListeners.Clear();
		}
		foreach (Spell.StateStartedListener stateStartedListener in array)
		{
			stateStartedListener.Fire(this, prevStateType);
		}
	}

	// Token: 0x060016E5 RID: 5861 RVA: 0x0006B9C0 File Offset: 0x00069BC0
	protected void FireSpellEventCallbacks(string eventName, object eventData)
	{
		Spell.SpellEventListener[] array = this.m_spellEventListeners.ToArray();
		foreach (Spell.SpellEventListener spellEventListener in array)
		{
			spellEventListener.Fire(eventName, eventData);
		}
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x0006B9FC File Offset: 0x00069BFC
	protected bool HasStateContent(SpellStateType stateType)
	{
		if (this.m_spellStateMap != null && this.m_spellStateMap.ContainsKey(stateType))
		{
			return true;
		}
		if (!this.m_fsmReady)
		{
			if (this.m_fsm != null && this.m_fsm.Fsm.HasEvent(EnumUtils.GetString<SpellStateType>(stateType)))
			{
				return true;
			}
		}
		else if (this.m_fsmStateMap != null && this.m_fsmStateMap.ContainsKey(stateType))
		{
			return true;
		}
		return false;
	}

	// Token: 0x060016E7 RID: 5863 RVA: 0x0006BA84 File Offset: 0x00069C84
	protected bool HasOverriddenStateMethod(SpellStateType stateType)
	{
		string stateMethodName = this.GetStateMethodName(stateType);
		if (stateMethodName == null)
		{
			return false;
		}
		Type type = base.GetType();
		Type typeFromHandle = typeof(Spell);
		MethodInfo method = typeFromHandle.GetMethod(stateMethodName, 36);
		MethodInfo method2 = type.GetMethod(stateMethodName, 36);
		return GeneralUtils.IsOverriddenMethod(method2, method);
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x0006BAD0 File Offset: 0x00069CD0
	protected string GetStateMethodName(SpellStateType stateType)
	{
		switch (stateType)
		{
		case SpellStateType.BIRTH:
			return "OnBirth";
		case SpellStateType.IDLE:
			return "OnIdle";
		case SpellStateType.ACTION:
			return "OnAction";
		case SpellStateType.CANCEL:
			return "OnCancel";
		case SpellStateType.DEATH:
			return "OnDeath";
		default:
			return null;
		}
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x0006BB1F File Offset: 0x00069D1F
	protected bool CanAddPowerTargets()
	{
		return SpellUtils.CanAddPowerTargets(this.m_taskList);
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x0006BB2C File Offset: 0x00069D2C
	protected bool AddSinglePowerTarget()
	{
		Card sourceCard = this.GetSourceCard();
		if (sourceCard == null)
		{
			Log.Power.PrintWarning("{0}.AddSinglePowerTarget() - a source card was never added", new object[]
			{
				this
			});
			return false;
		}
		Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
		if (blockStart == null)
		{
			Log.Power.PrintError("{0}.AddSinglePowerTarget() - got a task list with no block start", new object[]
			{
				this
			});
			return false;
		}
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		return this.AddSinglePowerTarget_FromBlockStart(blockStart) || this.AddSinglePowerTarget_FromMetaData(taskList) || this.AddSinglePowerTarget_FromAnyPower(sourceCard, taskList);
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x0006BBCC File Offset: 0x00069DCC
	protected bool AddSinglePowerTarget_FromBlockStart(Network.HistBlockStart blockStart)
	{
		Entity entity = GameState.Get().GetEntity(blockStart.Target);
		if (entity == null)
		{
			return false;
		}
		Card card = entity.GetCard();
		if (card == null)
		{
			Log.Power.Print("{0}.AddSinglePowerTarget_FromSourceAction() - FAILED Target {1} in blockStart has no Card", new object[]
			{
				this,
				blockStart.Target
			});
			return false;
		}
		this.AddTarget(card.gameObject);
		return true;
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x0006BC3C File Offset: 0x00069E3C
	protected bool AddSinglePowerTarget_FromMetaData(List<PowerTask> tasks)
	{
		GameState gameState = GameState.Get();
		for (int i = 0; i < tasks.Count; i++)
		{
			PowerTask powerTask = tasks[i];
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.META_DATA)
			{
				Network.HistMetaData histMetaData = (Network.HistMetaData)power;
				if (histMetaData.MetaType == null)
				{
					if (histMetaData.Info == null || histMetaData.Info.Count == 0)
					{
						Debug.LogError(string.Format("{0}.AddSinglePowerTarget_FromMetaData() - META_DATA at index {1} has no Info", this, i));
					}
					else
					{
						for (int j = 0; j < histMetaData.Info.Count; j++)
						{
							Entity entity = gameState.GetEntity(histMetaData.Info[j]);
							if (entity != null)
							{
								Card card = entity.GetCard();
								this.AddTargetFromMetaData(i, card);
								return true;
							}
							Debug.LogError(string.Format("{0}.AddSinglePowerTarget_FromMetaData() - Entity is null for META_DATA at index {1} Info index {2}", this, i, j));
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060016ED RID: 5869 RVA: 0x0006BD4C File Offset: 0x00069F4C
	protected bool AddSinglePowerTarget_FromAnyPower(Card sourceCard, List<PowerTask> tasks)
	{
		for (int i = 0; i < tasks.Count; i++)
		{
			PowerTask task = tasks[i];
			Card targetCardFromPowerTask = this.GetTargetCardFromPowerTask(i, task);
			if (!(targetCardFromPowerTask == null))
			{
				if (!(sourceCard == targetCardFromPowerTask))
				{
					this.AddTarget(targetCardFromPowerTask.gameObject);
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x0006BDB4 File Offset: 0x00069FB4
	protected bool AddMultiplePowerTargets()
	{
		Card sourceCard = this.GetSourceCard();
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		if (this.AddMultiplePowerTargets_FromMetaData(taskList))
		{
			return true;
		}
		this.AddMultiplePowerTargets_FromAnyPower(sourceCard, taskList);
		return true;
	}

	// Token: 0x060016EF RID: 5871 RVA: 0x0006BDEC File Offset: 0x00069FEC
	protected bool AddMultiplePowerTargets_FromMetaData(List<PowerTask> tasks)
	{
		int count = this.m_targets.Count;
		GameState gameState = GameState.Get();
		for (int i = 0; i < tasks.Count; i++)
		{
			PowerTask powerTask = tasks[i];
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.META_DATA)
			{
				Network.HistMetaData histMetaData = (Network.HistMetaData)power;
				if (histMetaData.MetaType == null)
				{
					if (histMetaData.Info == null || histMetaData.Info.Count == 0)
					{
						Debug.LogError(string.Format("{0}.AddMultiplePowerTargets_FromMetaData() - META_DATA at index {1} has no Info", this, i));
					}
					else
					{
						for (int j = 0; j < histMetaData.Info.Count; j++)
						{
							Entity entity = gameState.GetEntity(histMetaData.Info[j]);
							if (entity == null)
							{
								Debug.LogError(string.Format("{0}.AddMultiplePowerTargets_FromMetaData() - Entity is null for META_DATA at index {1} Info index {2}", this, i, j));
							}
							else
							{
								Card card = entity.GetCard();
								this.AddTargetFromMetaData(i, card);
							}
						}
					}
				}
			}
		}
		return this.m_targets.Count != count;
	}

	// Token: 0x060016F0 RID: 5872 RVA: 0x0006BF20 File Offset: 0x0006A120
	protected void AddMultiplePowerTargets_FromAnyPower(Card sourceCard, List<PowerTask> tasks)
	{
		for (int i = 0; i < tasks.Count; i++)
		{
			PowerTask task = tasks[i];
			Card targetCardFromPowerTask = this.GetTargetCardFromPowerTask(i, task);
			if (!(targetCardFromPowerTask == null))
			{
				if (!(sourceCard == targetCardFromPowerTask))
				{
					if (!this.IsTarget(targetCardFromPowerTask.gameObject))
					{
						this.AddTarget(targetCardFromPowerTask.gameObject);
					}
				}
			}
		}
	}

	// Token: 0x060016F1 RID: 5873 RVA: 0x0006BF9C File Offset: 0x0006A19C
	protected virtual Card GetTargetCardFromPowerTask(int index, PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		if (power.Type != Network.PowerType.TAG_CHANGE)
		{
			return null;
		}
		Network.HistTagChange histTagChange = power as Network.HistTagChange;
		Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
		if (entity == null)
		{
			string text = string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", this, histTagChange.Entity);
			Debug.LogWarning(text);
			return null;
		}
		return entity.GetCard();
	}

	// Token: 0x060016F2 RID: 5874 RVA: 0x0006C001 File Offset: 0x0006A201
	protected virtual void AddTargetFromMetaData(int metaDataIndex, Card targetCard)
	{
		this.AddTarget(targetCard.gameObject);
	}

	// Token: 0x060016F3 RID: 5875 RVA: 0x0006C00F File Offset: 0x0006A20F
	protected bool CompleteMetaDataTasks(int metaDataIndex)
	{
		return this.CompleteMetaDataTasks(metaDataIndex, null, null);
	}

	// Token: 0x060016F4 RID: 5876 RVA: 0x0006C01A File Offset: 0x0006A21A
	protected bool CompleteMetaDataTasks(int metaDataIndex, PowerTaskList.CompleteCallback completeCallback)
	{
		return this.CompleteMetaDataTasks(metaDataIndex, completeCallback, null);
	}

	// Token: 0x060016F5 RID: 5877 RVA: 0x0006C028 File Offset: 0x0006A228
	protected bool CompleteMetaDataTasks(int metaDataIndex, PowerTaskList.CompleteCallback completeCallback, object callbackData)
	{
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		int num = 1;
		for (int i = metaDataIndex + 1; i < taskList.Count; i++)
		{
			PowerTask powerTask = taskList[i];
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.META_DATA)
			{
				Network.HistMetaData histMetaData = (Network.HistMetaData)power;
				if (histMetaData.MetaType == null)
				{
					break;
				}
			}
			num++;
		}
		if (num == 0)
		{
			Debug.LogError(string.Format("{0}.CompleteMetaDataTasks() - there are no tasks to complete for meta data {1}", this, metaDataIndex));
			return false;
		}
		this.m_taskList.DoTasks(metaDataIndex, num, completeCallback, callbackData);
		return true;
	}

	// Token: 0x060016F6 RID: 5878 RVA: 0x0006C0C8 File Offset: 0x0006A2C8
	protected virtual void ShowImpl()
	{
		List<SpellState> activeStateList = this.GetActiveStateList();
		if (activeStateList != null)
		{
			foreach (SpellState spellState in activeStateList)
			{
				spellState.ShowState();
			}
		}
	}

	// Token: 0x060016F7 RID: 5879 RVA: 0x0006C12C File Offset: 0x0006A32C
	protected virtual void HideImpl()
	{
		List<SpellState> activeStateList = this.GetActiveStateList();
		if (activeStateList != null)
		{
			foreach (SpellState spellState in activeStateList)
			{
				spellState.HideState();
			}
		}
	}

	// Token: 0x060016F8 RID: 5880 RVA: 0x0006C190 File Offset: 0x0006A390
	protected void OnExitedNoneState()
	{
		if (this.DoesBlockServerEvents())
		{
			GameState.Get().AddServerBlockingSpell(this);
		}
		this.ActivateObjectContainer(true);
		this.BlockZones(true);
		if (ZoneMgr.Get() != null)
		{
			ZoneMgr.Get().RequestNextDeathBlockLayoutDelaySec(this.m_ZoneLayoutDelayForDeaths);
		}
	}

	// Token: 0x060016F9 RID: 5881 RVA: 0x0006C1E1 File Offset: 0x0006A3E1
	protected void OnEnteredNoneState()
	{
		if (this.DoesBlockServerEvents())
		{
			GameState.Get().RemoveServerBlockingSpell(this);
		}
		this.ActivateObjectContainer(false);
	}

	// Token: 0x060016FA RID: 5882 RVA: 0x0006C204 File Offset: 0x0006A404
	protected void BlockZones(bool block)
	{
		if (this.m_ZonesToDisable == null)
		{
			return;
		}
		foreach (SpellZoneTag zoneTag in this.m_ZonesToDisable)
		{
			List<Zone> list = SpellUtils.FindZonesFromTag(zoneTag);
			if (list != null)
			{
				foreach (Zone zone in list)
				{
					zone.BlockInput(block);
				}
			}
		}
	}

	// Token: 0x060016FB RID: 5883 RVA: 0x0006C2BC File Offset: 0x0006A4BC
	public void OnLoad()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			SpellState component = transform.gameObject.GetComponent<SpellState>();
			if (!(component == null))
			{
				component.OnLoad();
			}
		}
	}

	// Token: 0x04000B3E RID: 2878
	public bool m_BlockServerEvents;

	// Token: 0x04000B3F RID: 2879
	public GameObject m_ObjectContainer;

	// Token: 0x04000B40 RID: 2880
	public SpellLocation m_Location = SpellLocation.SOURCE_AUTO;

	// Token: 0x04000B41 RID: 2881
	public string m_LocationTransformName;

	// Token: 0x04000B42 RID: 2882
	public bool m_SetParentToLocation;

	// Token: 0x04000B43 RID: 2883
	public SpellFacing m_Facing;

	// Token: 0x04000B44 RID: 2884
	public SpellFacingOptions m_FacingOptions;

	// Token: 0x04000B45 RID: 2885
	public TARGET_RETICLE_TYPE m_TargetReticle;

	// Token: 0x04000B46 RID: 2886
	public List<SpellZoneTag> m_ZonesToDisable;

	// Token: 0x04000B47 RID: 2887
	public float m_ZoneLayoutDelayForDeaths;

	// Token: 0x04000B48 RID: 2888
	public bool m_UseFastActorTriggers;

	// Token: 0x04000B49 RID: 2889
	protected SpellType m_spellType;

	// Token: 0x04000B4A RID: 2890
	private Map<SpellStateType, List<SpellState>> m_spellStateMap;

	// Token: 0x04000B4B RID: 2891
	protected SpellStateType m_activeStateType;

	// Token: 0x04000B4C RID: 2892
	protected SpellStateType m_activeStateChange;

	// Token: 0x04000B4D RID: 2893
	private List<Spell.FinishedListener> m_finishedListeners = new List<Spell.FinishedListener>();

	// Token: 0x04000B4E RID: 2894
	private List<Spell.StateFinishedListener> m_stateFinishedListeners = new List<Spell.StateFinishedListener>();

	// Token: 0x04000B4F RID: 2895
	private List<Spell.StateStartedListener> m_stateStartedListeners = new List<Spell.StateStartedListener>();

	// Token: 0x04000B50 RID: 2896
	private List<Spell.SpellEventListener> m_spellEventListeners = new List<Spell.SpellEventListener>();

	// Token: 0x04000B51 RID: 2897
	protected GameObject m_source;

	// Token: 0x04000B52 RID: 2898
	protected List<GameObject> m_targets = new List<GameObject>();

	// Token: 0x04000B53 RID: 2899
	protected PowerTaskList m_taskList;

	// Token: 0x04000B54 RID: 2900
	protected bool m_shown = true;

	// Token: 0x04000B55 RID: 2901
	private PlayMakerFSM m_fsm;

	// Token: 0x04000B56 RID: 2902
	private Map<SpellStateType, FsmState> m_fsmStateMap;

	// Token: 0x04000B57 RID: 2903
	private bool m_fsmSkippedFirstFrame;

	// Token: 0x04000B58 RID: 2904
	private bool m_fsmReady;

	// Token: 0x04000B59 RID: 2905
	protected bool m_positionDirty = true;

	// Token: 0x04000B5A RID: 2906
	protected bool m_orientationDirty = true;

	// Token: 0x04000B5B RID: 2907
	protected bool m_finished;

	// Token: 0x020001A7 RID: 423
	// (Invoke) Token: 0x06001BEE RID: 7150
	public delegate void StateFinishedCallback(Spell spell, SpellStateType prevStateType, object userData);

	// Token: 0x02000343 RID: 835
	// (Invoke) Token: 0x06002BB1 RID: 11185
	public delegate void StateStartedCallback(Spell spell, SpellStateType prevStateType, object userData);

	// Token: 0x020003D4 RID: 980
	// (Invoke) Token: 0x060032DA RID: 13018
	public delegate void FinishedCallback(Spell spell, object userData);

	// Token: 0x020003D7 RID: 983
	// (Invoke) Token: 0x060032E7 RID: 13031
	public delegate void SpellEventCallback(string eventName, object eventData, object userData);

	// Token: 0x020003D8 RID: 984
	private class FinishedListener : EventListener<Spell.FinishedCallback>
	{
		// Token: 0x060032EB RID: 13035 RVA: 0x000FD9A7 File Offset: 0x000FBBA7
		public void Fire(Spell spell)
		{
			this.m_callback(spell, this.m_userData);
		}
	}

	// Token: 0x020003D9 RID: 985
	private class StateFinishedListener : EventListener<Spell.StateFinishedCallback>
	{
		// Token: 0x060032ED RID: 13037 RVA: 0x000FD9C3 File Offset: 0x000FBBC3
		public void Fire(Spell spell, SpellStateType prevStateType)
		{
			this.m_callback(spell, prevStateType, this.m_userData);
		}
	}

	// Token: 0x020003DA RID: 986
	private class StateStartedListener : EventListener<Spell.StateStartedCallback>
	{
		// Token: 0x060032EF RID: 13039 RVA: 0x000FD9E0 File Offset: 0x000FBBE0
		public void Fire(Spell spell, SpellStateType prevStateType)
		{
			this.m_callback(spell, prevStateType, this.m_userData);
		}
	}

	// Token: 0x020003DB RID: 987
	private class SpellEventListener : EventListener<Spell.SpellEventCallback>
	{
		// Token: 0x060032F1 RID: 13041 RVA: 0x000FD9FD File Offset: 0x000FBBFD
		public void Fire(string eventName, object eventData)
		{
			this.m_callback(eventName, eventData, this.m_userData);
		}
	}
}
