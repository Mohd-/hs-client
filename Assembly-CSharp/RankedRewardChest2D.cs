using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000531 RID: 1329
public class RankedRewardChest2D : PegUIElement
{
	// Token: 0x06003DC5 RID: 15813 RVA: 0x0012AD4C File Offset: 0x00128F4C
	protected override void Awake()
	{
		base.Awake();
		foreach (GameObject gameObject in this.m_rewardChests)
		{
			gameObject.SetActive(false);
		}
		this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ChestOver));
		this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.ChestOut));
	}

	// Token: 0x06003DC6 RID: 15814 RVA: 0x0012ADD4 File Offset: 0x00128FD4
	public void ChestOver(UIEvent e)
	{
		int chestIndexFromRank = RankedRewardChest.GetChestIndexFromRank(this.m_rank);
		string headline;
		string bodytext;
		if (chestIndexFromRank >= 0)
		{
			headline = RankedRewardChest.GetChestNameFromRank(this.m_rank);
			bodytext = GameStrings.Format("GLUE_QUEST_LOG_CHEST_TOOLTIP_BODY", new object[0]);
		}
		else
		{
			headline = GameStrings.Format("GLUE_QUEST_LOG_NO_CHEST", new object[0]);
			bodytext = GameStrings.Format("GLUE_QUEST_LOG_CHEST_TOOLTIP_BODY_NO_CHEST", new object[0]);
		}
		base.gameObject.GetComponent<TooltipZone>().ShowLayerTooltip(headline, bodytext);
	}

	// Token: 0x06003DC7 RID: 15815 RVA: 0x0012AE4B File Offset: 0x0012904B
	private void ChestOut(UIEvent e)
	{
		base.gameObject.GetComponent<TooltipZone>().HideTooltip();
	}

	// Token: 0x06003DC8 RID: 15816 RVA: 0x0012AE60 File Offset: 0x00129060
	public void SetRank(int rank)
	{
		this.m_rank = rank;
		int chestIndexFromRank = RankedRewardChest.GetChestIndexFromRank(rank);
		if (chestIndexFromRank >= 0)
		{
			GameObject gameObject = this.m_rewardChests[chestIndexFromRank];
			gameObject.SetActive(true);
			this.m_emptyRewardChest.SetActive(false);
			this.m_rewardChestRankText.Text = rank.ToString();
		}
		else
		{
			this.m_emptyRewardChest.SetActive(true);
			this.m_rewardChestRankText.TextAlpha = 0.2f;
			this.m_rewardChestRankText.Text = 20.ToString();
		}
		bool flag = this.m_rank == 0;
		this.m_legendaryGem.SetActive(flag);
		this.m_rewardChestRankText.gameObject.SetActive(!flag);
	}

	// Token: 0x04002758 RID: 10072
	public List<GameObject> m_rewardChests;

	// Token: 0x04002759 RID: 10073
	public GameObject m_emptyRewardChest;

	// Token: 0x0400275A RID: 10074
	public UberText m_rewardChestDescriptionText;

	// Token: 0x0400275B RID: 10075
	public UberText m_rewardChestRankText;

	// Token: 0x0400275C RID: 10076
	public GameObject m_legendaryGem;

	// Token: 0x0400275D RID: 10077
	private int m_rank;
}
