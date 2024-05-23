using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200069E RID: 1694
[CustomEditClass]
public class BoosterPackReward : Reward
{
	// Token: 0x0600475F RID: 18271 RVA: 0x001566D0 File Offset: 0x001548D0
	protected override void InitData()
	{
		base.SetData(new BoosterPackRewardData(), false);
	}

	// Token: 0x06004760 RID: 18272 RVA: 0x001566E0 File Offset: 0x001548E0
	protected override void ShowReward(bool updateCacheValues)
	{
		this.m_root.SetActive(true);
		SceneUtils.SetLayer(this.m_root, this.m_Layer);
		Vector3 localScale = this.m_unopenedPack.transform.localScale;
		this.m_unopenedPack.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		iTween.ScaleTo(this.m_unopenedPack.gameObject, iTween.Hash(new object[]
		{
			"scale",
			localScale,
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.easeOutElastic
		}));
		this.m_unopenedPack.transform.localEulerAngles = new Vector3(this.m_unopenedPack.transform.localEulerAngles.x, this.m_unopenedPack.transform.localEulerAngles.y, 70f);
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			new Vector3(0f, 0f, -120f),
			"time",
			0.2f,
			"easeType",
			iTween.EaseType.easeOutQuad,
			"space",
			1
		});
		iTween.RotateAdd(this.m_unopenedPack.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"amount",
			new Vector3(0f, 0f, 70f),
			"time",
			0.2f,
			"delay",
			0.2f,
			"easeType",
			iTween.EaseType.easeInOutQuad,
			"space",
			1
		});
		iTween.RotateAdd(this.m_unopenedPack.gameObject, args2);
		Hashtable args3 = iTween.Hash(new object[]
		{
			"amount",
			new Vector3(0f, 0f, -30f),
			"delay",
			0.4f,
			"time",
			1.5f,
			"easeType",
			iTween.EaseType.easeOutElastic,
			"space",
			1
		});
		iTween.RotateAdd(this.m_unopenedPack.gameObject, args3);
	}

	// Token: 0x06004761 RID: 18273 RVA: 0x00156975 File Offset: 0x00154B75
	protected override void HideReward()
	{
		base.HideReward();
		this.m_root.SetActive(false);
	}

	// Token: 0x06004762 RID: 18274 RVA: 0x0015698C File Offset: 0x00154B8C
	protected override void OnDataSet(bool updateVisuals)
	{
		if (!updateVisuals)
		{
			return;
		}
		this.m_BoosterPackBone.gameObject.SetActive(false);
		BoosterPackRewardData boosterPackRewardData = base.Data as BoosterPackRewardData;
		string headline = string.Empty;
		string empty = string.Empty;
		string source = string.Empty;
		if (base.Data.Origin == NetCache.ProfileNotice.NoticeOrigin.OUT_OF_BAND_LICENSE)
		{
			headline = GameStrings.Get("GLOBAL_REWARD_BOOSTER_HEADLINE_OUT_OF_BAND");
			source = GameStrings.Format("GLOBAL_REWARD_BOOSTER_DETAILS_OUT_OF_BAND", new object[]
			{
				boosterPackRewardData.Count
			});
		}
		else if (boosterPackRewardData.Count <= 1)
		{
			string key = "GLOBAL_REWARD_BOOSTER_HEADLINE_GENERIC";
			headline = GameStrings.Get(key);
		}
		else
		{
			headline = GameStrings.Format("GLOBAL_REWARD_BOOSTER_HEADLINE_MULTIPLE", new object[]
			{
				boosterPackRewardData.Count
			});
		}
		base.SetRewardText(headline, empty, source);
		BoosterDbfRecord record = GameDbf.Booster.GetRecord(boosterPackRewardData.Id);
		if (record != null)
		{
			base.SetReady(false);
			string cardName = FileUtils.GameAssetPathToName(record.PackOpeningPrefab);
			AssetLoader.Get().LoadActor(cardName, new AssetLoader.GameObjectCallback(this.OnUnopenedPackPrefabLoaded), null, false);
		}
	}

	// Token: 0x06004763 RID: 18275 RVA: 0x00156AA4 File Offset: 0x00154CA4
	private void OnUnopenedPackPrefabLoaded(string name, GameObject go, object callbackData)
	{
		go.transform.parent = this.m_BoosterPackBone.transform.parent;
		go.transform.position = this.m_BoosterPackBone.transform.position;
		go.transform.rotation = this.m_BoosterPackBone.transform.rotation;
		go.transform.localScale = this.m_BoosterPackBone.transform.localScale;
		this.m_unopenedPack = go.GetComponent<UnopenedPack>();
		this.UpdatePackStacks();
		base.SetReady(true);
	}

	// Token: 0x06004764 RID: 18276 RVA: 0x00156B38 File Offset: 0x00154D38
	private void UpdatePackStacks()
	{
		BoosterPackRewardData boosterPackRewardData = base.Data as BoosterPackRewardData;
		if (boosterPackRewardData == null)
		{
			Debug.LogWarning(string.Format("BoosterPackReward.UpdatePackStacks() - Data {0} is not CardRewardData", base.Data));
			return;
		}
		NetCache.BoosterStack boosterStack = new NetCache.BoosterStack();
		boosterStack.Id = boosterPackRewardData.Id;
		boosterStack.Count = boosterPackRewardData.Count;
		this.m_unopenedPack.SetBoosterStack(boosterStack);
		bool flag = boosterPackRewardData.Count > 1;
		this.m_unopenedPack.m_SingleStack.m_RootObject.SetActive(!flag);
		this.m_unopenedPack.m_MultipleStack.m_RootObject.SetActive(flag);
		this.m_unopenedPack.m_MultipleStack.m_AmountText.enabled = flag;
		if (flag)
		{
			this.m_unopenedPack.m_MultipleStack.m_AmountText.Text = boosterPackRewardData.Count.ToString();
		}
	}

	// Token: 0x04002E5E RID: 11870
	public GameObject m_BoosterPackBone;

	// Token: 0x04002E5F RID: 11871
	public GameLayer m_Layer = GameLayer.IgnoreFullScreenEffects;

	// Token: 0x04002E60 RID: 11872
	private UnopenedPack m_unopenedPack;
}
