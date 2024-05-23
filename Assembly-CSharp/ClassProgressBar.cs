using System;
using UnityEngine;

// Token: 0x0200052B RID: 1323
public class ClassProgressBar : PegUIElement
{
	// Token: 0x06003D9D RID: 15773 RVA: 0x00129F8C File Offset: 0x0012818C
	protected override void Awake()
	{
		base.Awake();
		this.m_lockedText.Text = GameStrings.Get("GLUE_QUEST_LOG_CLASS_LOCKED");
		this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnProgressBarOver));
		this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnProgressBarOut));
	}

	// Token: 0x06003D9E RID: 15774 RVA: 0x00129FDC File Offset: 0x001281DC
	public void Init()
	{
		this.m_classNameText.Text = GameStrings.GetClassName(this.m_class);
	}

	// Token: 0x06003D9F RID: 15775 RVA: 0x00129FF4 File Offset: 0x001281F4
	public void SetNextReward(NetCache.HeroLevel.NextLevelReward nextLevelReward)
	{
		this.m_nextLevelReward = nextLevelReward;
	}

	// Token: 0x06003DA0 RID: 15776 RVA: 0x0012A000 File Offset: 0x00128200
	private void ShowTooltip()
	{
		TooltipZone component = base.gameObject.GetComponent<TooltipZone>();
		KeywordHelpPanel keywordHelpPanel = component.ShowLayerTooltip(GameStrings.Format("GLOBAL_HERO_LEVEL_NEXT_REWARD_TITLE", new object[]
		{
			this.m_nextLevelReward.Level
		}), this.m_rewardText);
		keywordHelpPanel.m_name.WordWrap = false;
		keywordHelpPanel.m_name.UpdateNow();
	}

	// Token: 0x06003DA1 RID: 15777 RVA: 0x0012A060 File Offset: 0x00128260
	private void OnProgressBarOver(UIEvent e)
	{
		if (this.m_rewardText != null)
		{
			this.ShowTooltip();
			return;
		}
		if (this.m_nextLevelReward == null)
		{
			return;
		}
		RewardData reward = this.m_nextLevelReward.Reward;
		if (reward == null)
		{
			return;
		}
		this.m_rewardText = RewardUtils.GetRewardText(reward);
		this.ShowTooltip();
	}

	// Token: 0x06003DA2 RID: 15778 RVA: 0x0012A0B0 File Offset: 0x001282B0
	private void OnProgressBarOut(UIEvent e)
	{
		TooltipZone component = base.gameObject.GetComponent<TooltipZone>();
		component.HideTooltip();
	}

	// Token: 0x04002725 RID: 10021
	public TAG_CLASS m_class;

	// Token: 0x04002726 RID: 10022
	public UberText m_classNameText;

	// Token: 0x04002727 RID: 10023
	public UberText m_levelText;

	// Token: 0x04002728 RID: 10024
	public GameObject m_classLockedGO;

	// Token: 0x04002729 RID: 10025
	public UberText m_lockedText;

	// Token: 0x0400272A RID: 10026
	public ProgressBar m_progressBar;

	// Token: 0x0400272B RID: 10027
	public GameObject m_classIcon;

	// Token: 0x0400272C RID: 10028
	private NetCache.HeroLevel.NextLevelReward m_nextLevelReward;

	// Token: 0x0400272D RID: 10029
	private string m_rewardText;
}
