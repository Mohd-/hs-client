using System;
using UnityEngine;

// Token: 0x020001CF RID: 463
[CustomEditClass]
public class AdventureChooserSubButton : AdventureGenericButton
{
	// Token: 0x06001D67 RID: 7527 RVA: 0x00089CDB File Offset: 0x00087EDB
	public void SetAdventure(AdventureDbId id, AdventureModeDbId mode)
	{
		this.m_TargetAdventure = id;
		this.m_TargetMode = mode;
		this.ShowRemainingProgressCount();
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x00089CF1 File Offset: 0x00087EF1
	public AdventureDbId GetAdventure()
	{
		return this.m_TargetAdventure;
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x00089CF9 File Offset: 0x00087EF9
	public AdventureModeDbId GetMode()
	{
		return this.m_TargetMode;
	}

	// Token: 0x06001D6A RID: 7530 RVA: 0x00089D04 File Offset: 0x00087F04
	public void SetHighlight(bool enable)
	{
		UIBHighlightStateControl component = base.GetComponent<UIBHighlightStateControl>();
		if (component != null)
		{
			if (this.m_Glow)
			{
				component.Select(true, true);
			}
			else
			{
				component.Select(enable, false);
			}
		}
		UIBHighlight component2 = base.GetComponent<UIBHighlight>();
		if (component2 != null)
		{
			component2.AlwaysOver = enable;
		}
	}

	// Token: 0x06001D6B RID: 7531 RVA: 0x00089D60 File Offset: 0x00087F60
	public void SetNewGlow(bool enable)
	{
		this.m_Glow = enable;
		UIBHighlightStateControl component = base.GetComponent<UIBHighlightStateControl>();
		if (component != null)
		{
			component.Select(enable, true);
		}
	}

	// Token: 0x06001D6C RID: 7532 RVA: 0x00089D8F File Offset: 0x00087F8F
	public void Flash()
	{
		this.m_StateTable.TriggerState("Flash", true, null);
	}

	// Token: 0x06001D6D RID: 7533 RVA: 0x00089DA4 File Offset: 0x00087FA4
	public bool IsReady()
	{
		UIBHighlightStateControl component = base.GetComponent<UIBHighlightStateControl>();
		return component != null && component.IsReady();
	}

	// Token: 0x06001D6E RID: 7534 RVA: 0x00089DD0 File Offset: 0x00087FD0
	private void ShowRemainingProgressCount()
	{
		int num = 0;
		if (this.m_TargetMode == AdventureModeDbId.CLASS_CHALLENGE)
		{
			num = AdventureProgressMgr.Get().GetPlayableClassChallenges(this.m_TargetAdventure, this.m_TargetMode);
		}
		if (this.m_TargetMode == AdventureModeDbId.NORMAL || this.m_TargetMode == AdventureModeDbId.HEROIC)
		{
			num = AdventureProgressMgr.Get().GetPlayableAdventureScenarios(this.m_TargetAdventure, this.m_TargetMode);
		}
		if (this.m_TargetMode == AdventureModeDbId.HEROIC)
		{
			if (num > 0)
			{
				this.m_heroicSkull.SetActive(true);
			}
			else
			{
				this.m_heroicSkull.SetActive(false);
			}
			this.m_progressCounter.SetActive(false);
			return;
		}
		this.m_heroicSkull.SetActive(false);
		if (num > 0)
		{
			this.m_progressCounter.SetActive(true);
			this.m_progressCounterText.Text = num.ToString();
		}
		else
		{
			this.m_progressCounter.SetActive(false);
		}
	}

	// Token: 0x04001003 RID: 4099
	private const string s_EventFlash = "Flash";

	// Token: 0x04001004 RID: 4100
	[CustomEditField(Sections = "Progress UI")]
	public GameObject m_progressCounter;

	// Token: 0x04001005 RID: 4101
	[CustomEditField(Sections = "Progress UI")]
	public UberText m_progressCounterText;

	// Token: 0x04001006 RID: 4102
	[CustomEditField(Sections = "Progress UI")]
	public GameObject m_heroicSkull;

	// Token: 0x04001007 RID: 4103
	[CustomEditField(Sections = "Event Table")]
	public StateEventTable m_StateTable;

	// Token: 0x04001008 RID: 4104
	private AdventureDbId m_TargetAdventure;

	// Token: 0x04001009 RID: 4105
	private AdventureModeDbId m_TargetMode;

	// Token: 0x0400100A RID: 4106
	private bool m_Glow;
}
