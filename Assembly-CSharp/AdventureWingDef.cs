using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000233 RID: 563
[CustomEditClass]
public class AdventureWingDef : MonoBehaviour
{
	// Token: 0x06002142 RID: 8514 RVA: 0x000A3244 File Offset: 0x000A1444
	public void Init(WingDbfRecord wingRecord)
	{
		this.m_AdventureId = (AdventureDbId)wingRecord.AdventureId;
		this.m_WingId = (WingDbId)wingRecord.ID;
		this.m_OwnershipPrereq = (WingDbId)wingRecord.OwnershipPrereqWingId;
		this.m_SortOrder = wingRecord.SortOrder;
		this.m_WingName = wingRecord.Name;
		this.m_ComingSoonLabel = wingRecord.ComingSoonLabel;
		this.m_RequiresLabel = wingRecord.RequiresLabel;
	}

	// Token: 0x06002143 RID: 8515 RVA: 0x000A32B4 File Offset: 0x000A14B4
	public AdventureDbId GetAdventureId()
	{
		return this.m_AdventureId;
	}

	// Token: 0x06002144 RID: 8516 RVA: 0x000A32BC File Offset: 0x000A14BC
	public WingDbId GetWingId()
	{
		return this.m_WingId;
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x000A32C4 File Offset: 0x000A14C4
	public WingDbId GetOwnershipPrereqId()
	{
		return this.m_OwnershipPrereq;
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x000A32CC File Offset: 0x000A14CC
	public int GetSortOrder()
	{
		return this.m_SortOrder;
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x000A32D4 File Offset: 0x000A14D4
	public string GetWingName()
	{
		return this.m_WingName;
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x000A32DC File Offset: 0x000A14DC
	public string GetComingSoonLabel()
	{
		return this.m_ComingSoonLabel;
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x000A32E4 File Offset: 0x000A14E4
	public string GetRequiresLabel()
	{
		return this.m_RequiresLabel;
	}

	// Token: 0x0400128A RID: 4746
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_WingPrefab;

	// Token: 0x0400128B RID: 4747
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_CoinPrefab;

	// Token: 0x0400128C RID: 4748
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_RewardsPrefab;

	// Token: 0x0400128D RID: 4749
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_UnlockSpellPrefab;

	// Token: 0x0400128E RID: 4750
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_AccentPrefab;

	// Token: 0x0400128F RID: 4751
	[CustomEditField(Sections = "Opening Quote")]
	public string m_OpenQuotePrefab;

	// Token: 0x04001290 RID: 4752
	[CustomEditField(Sections = "Opening Quote")]
	public string m_OpenQuoteVOLine;

	// Token: 0x04001291 RID: 4753
	[CustomEditField(Sections = "Wing Open Popup", T = EditType.GAME_OBJECT)]
	public string m_WingOpenPopup;

	// Token: 0x04001292 RID: 4754
	[CustomEditField(Sections = "Complete Quote")]
	public string m_CompleteQuotePrefab;

	// Token: 0x04001293 RID: 4755
	[CustomEditField(Sections = "Complete Quote")]
	public string m_CompleteQuoteVOLine;

	// Token: 0x04001294 RID: 4756
	[CustomEditField(Sections = "Rewards Preview")]
	public List<string> m_SpecificRewardsPreviewCards;

	// Token: 0x04001295 RID: 4757
	[CustomEditField(Sections = "Rewards Preview")]
	public int m_HiddenRewardsPreviewCount;

	// Token: 0x04001296 RID: 4758
	[CustomEditField(Sections = "Loc Strings")]
	public string m_LockedLocString;

	// Token: 0x04001297 RID: 4759
	[CustomEditField(Sections = "Loc Strings")]
	public string m_LockedPurchaseLocString;

	// Token: 0x04001298 RID: 4760
	private AdventureDbId m_AdventureId;

	// Token: 0x04001299 RID: 4761
	private WingDbId m_WingId;

	// Token: 0x0400129A RID: 4762
	private WingDbId m_OwnershipPrereq;

	// Token: 0x0400129B RID: 4763
	private int m_SortOrder;

	// Token: 0x0400129C RID: 4764
	private string m_WingName;

	// Token: 0x0400129D RID: 4765
	private string m_ComingSoonLabel;

	// Token: 0x0400129E RID: 4766
	private string m_RequiresLabel;
}
