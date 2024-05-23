using System;
using UnityEngine;

// Token: 0x02000AAD RID: 2733
public class MountReward : Reward
{
	// Token: 0x06005EB5 RID: 24245 RVA: 0x001C5CFE File Offset: 0x001C3EFE
	protected override void InitData()
	{
		base.SetData(new MountRewardData(), false);
	}

	// Token: 0x06005EB6 RID: 24246 RVA: 0x001C5D0C File Offset: 0x001C3F0C
	protected override void ShowReward(bool updateCacheValues)
	{
		if (!(base.Data is MountRewardData))
		{
			Debug.LogWarning(string.Format("MountReward.ShowReward() - Data {0} is not MountRewardData", base.Data));
			return;
		}
		this.m_root.SetActive(true);
		Vector3 localScale = this.m_mount.transform.localScale;
		this.m_mount.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		iTween.ScaleTo(this.m_mount.gameObject, iTween.Hash(new object[]
		{
			"scale",
			localScale,
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.easeOutElastic
		}));
	}

	// Token: 0x06005EB7 RID: 24247 RVA: 0x001C5DD5 File Offset: 0x001C3FD5
	protected override void HideReward()
	{
		base.HideReward();
		this.m_root.SetActive(false);
	}

	// Token: 0x06005EB8 RID: 24248 RVA: 0x001C5DEC File Offset: 0x001C3FEC
	protected override void OnDataSet(bool updateVisuals)
	{
		if (!updateVisuals)
		{
			return;
		}
		MountRewardData mountRewardData = base.Data as MountRewardData;
		if (mountRewardData == null)
		{
			return;
		}
		string headline = string.Empty;
		MountRewardData.MountType mount = mountRewardData.Mount;
		if (mount != MountRewardData.MountType.WOW_HEARTHSTEED)
		{
			if (mount == MountRewardData.MountType.HEROES_MAGIC_CARPET_CARD)
			{
				headline = GameStrings.Get("GLOBAL_REWARD_HEROES_CARD_MOUNT_HEADLINE");
			}
		}
		else
		{
			headline = GameStrings.Get("GLOBAL_REWARD_HEARTHSTEED_HEADLINE");
		}
		base.SetRewardText(headline, string.Empty, string.Empty);
	}

	// Token: 0x0400462F RID: 17967
	public GameObject m_mount;
}
