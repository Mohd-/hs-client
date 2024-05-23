using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000AAB RID: 2731
[Obsolete("use ArenaTicketReward")]
public class ForgeTicketReward : Reward
{
	// Token: 0x06005EAC RID: 24236 RVA: 0x001C5869 File Offset: 0x001C3A69
	protected override void InitData()
	{
		base.SetData(new ForgeTicketRewardData(), false);
	}

	// Token: 0x06005EAD RID: 24237 RVA: 0x001C5878 File Offset: 0x001C3A78
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
		else
		{
			headline = GameStrings.Get("GLOBAL_REWARD_FORGE_UNLOCKED_HEADLINE");
			source = GameStrings.Get("GLOBAL_REWARD_FORGE_UNLOCKED_SOURCE");
		}
		base.SetRewardText(headline, empty, source);
		this.m_root.SetActive(true);
		this.m_rotateParent.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
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
		iTween.RotateAdd(this.m_rotateParent, args);
	}

	// Token: 0x06005EAE RID: 24238 RVA: 0x001C59A9 File Offset: 0x001C3BA9
	protected override void HideReward()
	{
		base.HideReward();
		this.m_root.SetActive(false);
	}

	// Token: 0x0400462D RID: 17965
	public GameObject m_rotateParent;
}
