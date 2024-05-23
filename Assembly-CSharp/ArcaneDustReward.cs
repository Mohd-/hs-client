using System;
using UnityEngine;

// Token: 0x02000AA7 RID: 2727
public class ArcaneDustReward : Reward
{
	// Token: 0x06005E94 RID: 24212 RVA: 0x001C4D2E File Offset: 0x001C2F2E
	protected override void InitData()
	{
		base.SetData(new ArcaneDustRewardData(), false);
	}

	// Token: 0x06005E95 RID: 24213 RVA: 0x001C4D3C File Offset: 0x001C2F3C
	protected override void ShowReward(bool updateCacheValues)
	{
		ArcaneDustRewardData arcaneDustRewardData = base.Data as ArcaneDustRewardData;
		if (arcaneDustRewardData == null)
		{
			Debug.LogWarning(string.Format("ArcaneDustReward.ShowReward() - Data {0} is not ArcaneDustRewardData", base.Data));
			return;
		}
		if (!arcaneDustRewardData.IsDummyReward && updateCacheValues)
		{
			NetCache.Get().OnArcaneDustBalanceChanged((long)arcaneDustRewardData.Amount);
			if (CraftingManager.Get() != null)
			{
				CraftingManager.Get().AdjustLocalArcaneDustBalance(arcaneDustRewardData.Amount);
				CraftingManager.Get().UpdateBankText();
			}
		}
		this.m_root.SetActive(true);
		this.m_dustCount.Text = arcaneDustRewardData.Amount.ToString();
		Vector3 localScale = this.m_dustJar.transform.localScale;
		this.m_dustJar.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		iTween.ScaleTo(this.m_dustJar.gameObject, iTween.Hash(new object[]
		{
			"scale",
			localScale,
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.easeOutElastic
		}));
	}

	// Token: 0x06005E96 RID: 24214 RVA: 0x001C4E6A File Offset: 0x001C306A
	protected override void HideReward()
	{
		base.HideReward();
		this.m_root.SetActive(false);
	}

	// Token: 0x06005E97 RID: 24215 RVA: 0x001C4E80 File Offset: 0x001C3080
	protected override void OnDataSet(bool updateVisuals)
	{
		if (!updateVisuals)
		{
			return;
		}
		string headline = GameStrings.Get("GLOBAL_REWARD_ARCANE_DUST_HEADLINE");
		base.SetRewardText(headline, string.Empty, string.Empty);
	}

	// Token: 0x04004622 RID: 17954
	public GameObject m_dustJar;

	// Token: 0x04004623 RID: 17955
	public UberText m_dustCount;
}
