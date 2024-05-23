using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000AA8 RID: 2728
public class ArenaTicketReward : Reward
{
	// Token: 0x06005E99 RID: 24217 RVA: 0x001C4EB8 File Offset: 0x001C30B8
	protected override void InitData()
	{
		base.SetData(new ForgeTicketRewardData(), false);
	}

	// Token: 0x06005E9A RID: 24218 RVA: 0x001C4EC8 File Offset: 0x001C30C8
	protected override void ShowReward(bool updateCacheValues)
	{
		string headline = string.Empty;
		string empty = string.Empty;
		string source = string.Empty;
		if (base.Data.Origin == NetCache.ProfileNotice.NoticeOrigin.OUT_OF_BAND_LICENSE)
		{
			ForgeTicketRewardData forgeTicketRewardData = base.Data as ForgeTicketRewardData;
			headline = GameStrings.Get("GLOBAL_REWARD_FORGE_HEADLINE");
			source = GameStrings.Format("GLOBAL_REWARD_BOOSTER_DETAILS_OUT_OF_BAND", new object[]
			{
				forgeTicketRewardData.Quantity
			});
		}
		else if (base.Data.Origin == NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT)
		{
			headline = GameStrings.Get("GLOBAL_REWARD_ARENA_TICKET_HEADLINE");
		}
		else
		{
			headline = GameStrings.Get("GLOBAL_REWARD_FORGE_UNLOCKED_HEADLINE");
			source = GameStrings.Get("GLOBAL_REWARD_FORGE_UNLOCKED_SOURCE");
		}
		base.SetRewardText(headline, empty, source);
		if (this.m_countLabel != null)
		{
			ForgeTicketRewardData forgeTicketRewardData2 = base.Data as ForgeTicketRewardData;
			this.m_countLabel.Text = forgeTicketRewardData2.Quantity.ToString();
		}
		if (this.m_playerNameLabel != null)
		{
			BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
			if (myPlayer != null)
			{
				this.m_playerNameLabel.Text = myPlayer.GetBattleTag().GetName();
			}
		}
		this.m_root.SetActive(true);
		this.m_ticketVisual.transform.localEulerAngles = new Vector3(this.m_ticketVisual.transform.localEulerAngles.x, this.m_ticketVisual.transform.localEulerAngles.y, 180f);
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			new Vector3(0f, 0f, 540f),
			"time",
			1.5f,
			"easeType",
			iTween.EaseType.easeOutElastic,
			"space",
			1
		});
		iTween.RotateAdd(this.m_ticketVisual, args);
	}

	// Token: 0x06005E9B RID: 24219 RVA: 0x001C50B6 File Offset: 0x001C32B6
	protected override void HideReward()
	{
		base.HideReward();
		this.m_root.SetActive(false);
	}

	// Token: 0x04004624 RID: 17956
	public GameObject m_ticketVisual;

	// Token: 0x04004625 RID: 17957
	public UberText m_countLabel;

	// Token: 0x04004626 RID: 17958
	public UberText m_playerNameLabel;
}
