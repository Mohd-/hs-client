using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000AA9 RID: 2729
public class CardBackReward : Reward
{
	// Token: 0x06005E9D RID: 24221 RVA: 0x001C50D2 File Offset: 0x001C32D2
	protected override void InitData()
	{
		base.SetData(new CardBackRewardData(), false);
	}

	// Token: 0x06005E9E RID: 24222 RVA: 0x001C50E0 File Offset: 0x001C32E0
	protected override void ShowReward(bool updateCacheValues)
	{
		CardBackRewardData cardBackRewardData = base.Data as CardBackRewardData;
		if (cardBackRewardData == null)
		{
			Debug.LogWarning(string.Format("CardBackReward.ShowReward() - Data {0} is not CardBackRewardData", base.Data));
			return;
		}
		if (!cardBackRewardData.IsDummyReward && updateCacheValues)
		{
			CardBackManager.Get().AddNewCardBack(cardBackRewardData.CardBackID);
		}
		this.m_root.SetActive(true);
		this.m_cardbackBone.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
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
		iTween.RotateAdd(this.m_cardbackBone.gameObject, args);
	}

	// Token: 0x06005E9F RID: 24223 RVA: 0x001C51DF File Offset: 0x001C33DF
	protected override void HideReward()
	{
		base.HideReward();
		this.m_root.SetActive(false);
	}

	// Token: 0x06005EA0 RID: 24224 RVA: 0x001C51F4 File Offset: 0x001C33F4
	protected override void OnDataSet(bool updateVisuals)
	{
		if (!updateVisuals)
		{
			return;
		}
		string headline = GameStrings.Get("GLOBAL_REWARD_CARD_BACK_HEADLINE");
		base.SetRewardText(headline, string.Empty, string.Empty);
		CardBackRewardData cardBackRewardData = base.Data as CardBackRewardData;
		if (cardBackRewardData == null)
		{
			Debug.LogWarning(string.Format("CardBackReward.OnDataSet() - Data {0} is not CardBackRewardData", base.Data));
			return;
		}
		base.SetReady(false);
		CardBackManager.Get().LoadCardBackByIndex(cardBackRewardData.CardBackID, new CardBackManager.LoadCardBackData.LoadCardBackCallback(this.OnFrontCardBackLoaded), true, "Card_Hidden");
		CardBackManager.Get().LoadCardBackByIndex(cardBackRewardData.CardBackID, new CardBackManager.LoadCardBackData.LoadCardBackCallback(this.OnBackCardBackLoaded), true, "Card_Hidden");
	}

	// Token: 0x06005EA1 RID: 24225 RVA: 0x001C529C File Offset: 0x001C349C
	private void OnFrontCardBackLoaded(CardBackManager.LoadCardBackData cardbackData)
	{
		GameObject gameObject = cardbackData.m_GameObject;
		gameObject.transform.parent = this.m_cardbackBone.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		gameObject.transform.localScale = Vector3.one;
		SceneUtils.SetLayer(gameObject, base.gameObject.layer);
		this.m_numCardBacksLoaded++;
		if (this.m_numCardBacksLoaded == 2)
		{
			base.SetReady(true);
		}
	}

	// Token: 0x06005EA2 RID: 24226 RVA: 0x001C5330 File Offset: 0x001C3530
	private void OnBackCardBackLoaded(CardBackManager.LoadCardBackData cardbackData)
	{
		GameObject gameObject = cardbackData.m_GameObject;
		gameObject.transform.parent = this.m_cardbackBone.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
		gameObject.transform.localScale = Vector3.one;
		SceneUtils.SetLayer(gameObject, base.gameObject.layer);
		this.m_numCardBacksLoaded++;
		if (this.m_numCardBacksLoaded == 2)
		{
			base.SetReady(true);
		}
	}

	// Token: 0x04004627 RID: 17959
	public GameObject m_cardbackBone;

	// Token: 0x04004628 RID: 17960
	private int m_numCardBacksLoaded;
}
