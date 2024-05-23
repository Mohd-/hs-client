using System;
using UnityEngine;

// Token: 0x02000499 RID: 1177
public class UnopenedPack : PegUIElement
{
	// Token: 0x06003840 RID: 14400 RVA: 0x00113A3D File Offset: 0x00111C3D
	protected override void Awake()
	{
		base.Awake();
		this.UpdateState();
	}

	// Token: 0x06003841 RID: 14401 RVA: 0x00113A4B File Offset: 0x00111C4B
	public NetCache.BoosterStack GetBoosterStack()
	{
		return this.m_boosterStack;
	}

	// Token: 0x06003842 RID: 14402 RVA: 0x00113A53 File Offset: 0x00111C53
	public void AddBoosters(int numNewBoosters)
	{
		this.m_boosterStack.Count += numNewBoosters;
		this.UpdateState();
	}

	// Token: 0x06003843 RID: 14403 RVA: 0x00113A6E File Offset: 0x00111C6E
	public void AddBooster()
	{
		this.AddBoosters(1);
	}

	// Token: 0x06003844 RID: 14404 RVA: 0x00113A77 File Offset: 0x00111C77
	public void SetBoosterStack(NetCache.BoosterStack boosterStack)
	{
		this.m_boosterStack = boosterStack;
		this.UpdateState();
	}

	// Token: 0x06003845 RID: 14405 RVA: 0x00113A88 File Offset: 0x00111C88
	public void RemoveBooster()
	{
		this.m_boosterStack.Count--;
		if (this.m_boosterStack.Count < 0)
		{
			Debug.LogWarning("UnopenedPack.RemoveBooster(): Removed a booster pack from a stack with no boosters");
			this.m_boosterStack.Count = 0;
		}
		this.UpdateState();
	}

	// Token: 0x06003846 RID: 14406 RVA: 0x00113AD8 File Offset: 0x00111CD8
	public UnopenedPack AcquireDraggedPack()
	{
		if (this.m_boosterStack.Id == 10 && !Options.Get().GetBool(Option.HAS_HEARD_TGT_PACK_VO, false))
		{
			Options.Get().SetBool(Option.HAS_HEARD_TGT_PACK_VO, true);
			NotificationManager.Get().CreateTirionQuote("VO_TIRION_INTRO_02", "VO_TIRION_INTRO_02", true);
		}
		if (this.m_draggedPack != null)
		{
			return this.m_draggedPack;
		}
		this.m_draggedPack = (UnopenedPack)Object.Instantiate(this, base.transform.position, base.transform.rotation);
		TransformUtil.CopyWorldScale(this.m_draggedPack, this);
		this.m_draggedPack.transform.parent = base.transform.parent;
		UIBScrollableItem component = this.m_draggedPack.GetComponent<UIBScrollableItem>();
		if (component != null)
		{
			component.m_active = UIBScrollableItem.ActiveState.Inactive;
		}
		this.m_draggedPack.m_creatorPack = this;
		DragRotator dragRotator = this.m_draggedPack.gameObject.AddComponent<DragRotator>();
		dragRotator.SetInfo(this.m_DragRotatorInfo);
		this.m_draggedPack.m_DragStartEvent.Activate();
		return this.m_draggedPack;
	}

	// Token: 0x06003847 RID: 14407 RVA: 0x00113BF8 File Offset: 0x00111DF8
	public void ReleaseDraggedPack()
	{
		if (this.m_draggedPack == null)
		{
			return;
		}
		UnopenedPack draggedPack = this.m_draggedPack;
		this.m_draggedPack = null;
		draggedPack.m_DragStopEvent.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnDragStopSpellStateFinished), draggedPack);
		draggedPack.m_DragStopEvent.Activate();
		this.UpdateState();
	}

	// Token: 0x06003848 RID: 14408 RVA: 0x00113C4E File Offset: 0x00111E4E
	public UnopenedPack GetDraggedPack()
	{
		return this.m_draggedPack;
	}

	// Token: 0x06003849 RID: 14409 RVA: 0x00113C56 File Offset: 0x00111E56
	public UnopenedPack GetCreatorPack()
	{
		return this.m_creatorPack;
	}

	// Token: 0x0600384A RID: 14410 RVA: 0x00113C5E File Offset: 0x00111E5E
	public void PlayAlert()
	{
		this.m_AlertEvent.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x0600384B RID: 14411 RVA: 0x00113C6C File Offset: 0x00111E6C
	public void StopAlert()
	{
		this.m_AlertEvent.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x0600384C RID: 14412 RVA: 0x00113C7C File Offset: 0x00111E7C
	public bool CanOpenPack()
	{
		NetCache.BoosterStack boosterStack = this.GetBoosterStack();
		if (boosterStack == null)
		{
			return false;
		}
		BoosterDbfRecord record = GameDbf.Booster.GetRecord(boosterStack.Id);
		if (record == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(record.OpenPackEvent))
		{
			return false;
		}
		string openPackEvent = record.OpenPackEvent;
		SpecialEventType specialEventType;
		return EnumUtils.TryGetEnum<SpecialEventType>(openPackEvent, out specialEventType) && (specialEventType == SpecialEventType.IGNORE || SpecialEventManager.Get().IsEventActive(specialEventType, false));
	}

	// Token: 0x0600384D RID: 14413 RVA: 0x00113CF0 File Offset: 0x00111EF0
	private void UpdateState()
	{
		bool flag = this.CanOpenPack();
		if (this.m_LockRibbon != null)
		{
			this.m_LockRibbon.SetActive(!flag);
		}
		bool flag2 = this.m_boosterStack.Count == 0;
		bool flag3 = this.m_boosterStack.Count > 1 && flag;
		this.m_SingleStack.m_RootObject.SetActive(!flag3 && !flag2);
		this.m_MultipleStack.m_RootObject.SetActive(flag3 && !flag2);
		this.m_MultipleStack.m_AmountText.enabled = flag3;
		if (flag3)
		{
			this.m_MultipleStack.m_AmountText.Text = this.m_boosterStack.Count.ToString();
		}
	}

	// Token: 0x0600384E RID: 14414 RVA: 0x00113DC0 File Offset: 0x00111FC0
	private void OnDragStopSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		UnopenedPack unopenedPack = (UnopenedPack)userData;
		Object.Destroy(unopenedPack.gameObject);
	}

	// Token: 0x04002427 RID: 9255
	public UnopenedPackStack m_SingleStack;

	// Token: 0x04002428 RID: 9256
	public UnopenedPackStack m_MultipleStack;

	// Token: 0x04002429 RID: 9257
	public GameObject m_LockRibbon;

	// Token: 0x0400242A RID: 9258
	public Spell m_AlertEvent;

	// Token: 0x0400242B RID: 9259
	public Spell m_DragStartEvent;

	// Token: 0x0400242C RID: 9260
	public Spell m_DragStopEvent;

	// Token: 0x0400242D RID: 9261
	public DragRotatorInfo m_DragRotatorInfo = new DragRotatorInfo
	{
		m_PitchInfo = new DragRotatorAxisInfo
		{
			m_ForceMultiplier = 3f,
			m_MinDegrees = -55f,
			m_MaxDegrees = 55f,
			m_RestSeconds = 2f
		},
		m_RollInfo = new DragRotatorAxisInfo
		{
			m_ForceMultiplier = 4.5f,
			m_MinDegrees = -60f,
			m_MaxDegrees = 60f,
			m_RestSeconds = 2f
		}
	};

	// Token: 0x0400242E RID: 9262
	private NetCache.BoosterStack m_boosterStack = new NetCache.BoosterStack
	{
		Id = 0,
		Count = 0
	};

	// Token: 0x0400242F RID: 9263
	private UnopenedPack m_draggedPack;

	// Token: 0x04002430 RID: 9264
	private UnopenedPack m_creatorPack;
}
