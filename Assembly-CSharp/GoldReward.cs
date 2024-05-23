using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000AAC RID: 2732
public class GoldReward : Reward
{
	// Token: 0x06005EB0 RID: 24240 RVA: 0x001C59C5 File Offset: 0x001C3BC5
	protected override void InitData()
	{
		base.SetData(new GoldRewardData(), false);
	}

	// Token: 0x06005EB1 RID: 24241 RVA: 0x001C59D4 File Offset: 0x001C3BD4
	protected override void ShowReward(bool updateCacheValues)
	{
		GoldRewardData goldRewardData = base.Data as GoldRewardData;
		if (!goldRewardData.IsDummyReward)
		{
			bool flag;
			if (base.Data.Origin == NetCache.ProfileNotice.NoticeOrigin.BETA_REIMBURSE)
			{
				NetCache.NetCacheGoldBalance netObject = NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>();
				flag = (netObject.GetTotal() == 0L);
			}
			else
			{
				flag = updateCacheValues;
			}
			if (flag)
			{
				NetCache.Get().RefreshNetObject<NetCache.NetCacheGoldBalance>();
			}
		}
		this.m_root.SetActive(true);
		Vector3 localScale = this.m_coin.transform.localScale;
		this.m_coin.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		iTween.ScaleTo(this.m_coin.gameObject, iTween.Hash(new object[]
		{
			"scale",
			localScale,
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.easeOutElastic
		}));
		this.m_coin.transform.localEulerAngles = new Vector3(0f, 180f, 180f);
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
		iTween.RotateAdd(this.m_coin.gameObject, args);
	}

	// Token: 0x06005EB2 RID: 24242 RVA: 0x001C5B6B File Offset: 0x001C3D6B
	protected override void HideReward()
	{
		base.HideReward();
		this.m_root.SetActive(false);
	}

	// Token: 0x06005EB3 RID: 24243 RVA: 0x001C5B80 File Offset: 0x001C3D80
	protected override void OnDataSet(bool updateVisuals)
	{
		if (!updateVisuals)
		{
			return;
		}
		GoldRewardData goldRewardData = base.Data as GoldRewardData;
		if (goldRewardData == null)
		{
			Debug.LogWarning(string.Format("goldRewardData.SetData() - data {0} is not GoldRewardData", base.Data));
			return;
		}
		string headline = GameStrings.Get("GLOBAL_REWARD_GOLD_HEADLINE");
		string details = goldRewardData.Amount.ToString();
		string source = string.Empty;
		UberText uberText = this.m_coin.GetComponentsInChildren<UberText>(true)[0];
		if (uberText != null)
		{
			this.m_rewardBanner.m_detailsText = uberText;
			this.m_rewardBanner.AlignHeadlineToCenterBone();
		}
		NetCache.ProfileNotice.NoticeOrigin origin = base.Data.Origin;
		switch (origin)
		{
		case NetCache.ProfileNotice.NoticeOrigin.BETA_REIMBURSE:
			headline = GameStrings.Get("GLOBAL_BETA_REIMBURSEMENT_HEADLINE");
			source = GameStrings.Get("GLOBAL_BETA_REIMBURSEMENT_DETAILS");
			break;
		default:
			if (origin == NetCache.ProfileNotice.NoticeOrigin.IGR)
			{
				if (goldRewardData.Date != null)
				{
					string text = GameStrings.Format("GLOBAL_CURRENT_DATE", new object[]
					{
						goldRewardData.Date
					});
					source = GameStrings.Format("GLOBAL_REWARD_GOLD_SOURCE_IGR_DATED", new object[]
					{
						text
					});
				}
				else
				{
					source = GameStrings.Get("GLOBAL_REWARD_GOLD_SOURCE_IGR");
				}
			}
			break;
		case NetCache.ProfileNotice.NoticeOrigin.TOURNEY:
		{
			NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
			source = GameStrings.Format("GLOBAL_REWARD_GOLD_SOURCE_TOURNEY", new object[]
			{
				netObject.WinsPerGold
			});
			break;
		}
		}
		base.SetRewardText(headline, details, source);
	}

	// Token: 0x0400462E RID: 17966
	public GameObject m_coin;
}
