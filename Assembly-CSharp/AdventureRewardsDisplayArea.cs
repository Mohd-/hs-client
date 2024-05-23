using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003B7 RID: 951
[CustomEditClass]
public class AdventureRewardsDisplayArea : MonoBehaviour
{
	// Token: 0x0600320D RID: 12813 RVA: 0x000FB5F0 File Offset: 0x000F97F0
	private void Awake()
	{
		if (this.m_FullscreenModeOffClicker != null)
		{
			this.m_FullscreenModeOffClicker.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
			{
				this.HideCardRewards();
			});
		}
		if (this.m_FullscreenDisableScrollBar != null)
		{
			this.m_FullscreenDisableScrollBar.AddTouchScrollStartedListener(new UIBScrollable.OnTouchScrollStarted(this.HideCardRewards));
		}
	}

	// Token: 0x0600320E RID: 12814 RVA: 0x000FB64F File Offset: 0x000F984F
	private void OnDestroy()
	{
		this.DisableFullscreen();
	}

	// Token: 0x0600320F RID: 12815 RVA: 0x000FB657 File Offset: 0x000F9857
	public bool IsShowing()
	{
		return this.m_Showing;
	}

	// Token: 0x06003210 RID: 12816 RVA: 0x000FB660 File Offset: 0x000F9860
	public void ShowCardsNoFullscreen(List<CardRewardData> rewards, Vector3 finalPosition, Vector3? origin = null)
	{
		List<string> list = new List<string>();
		foreach (CardRewardData cardRewardData in rewards)
		{
			list.Add(cardRewardData.CardID);
		}
		this.ShowCardsNoFullscreen(list, finalPosition, origin);
	}

	// Token: 0x06003211 RID: 12817 RVA: 0x000FB6CC File Offset: 0x000F98CC
	public void ShowCards(List<CardRewardData> rewards, Vector3 finalPosition, Vector3? origin = null)
	{
		List<string> list = new List<string>();
		foreach (CardRewardData cardRewardData in rewards)
		{
			list.Add(cardRewardData.CardID);
		}
		this.ShowCards(list, finalPosition, origin);
	}

	// Token: 0x06003212 RID: 12818 RVA: 0x000FB738 File Offset: 0x000F9938
	public void ShowCardsNoFullscreen(List<string> cardIds, Vector3 finalPosition, Vector3? origin = null)
	{
		this.DoShowCardRewards(cardIds, new Vector3?(finalPosition), origin, true);
	}

	// Token: 0x06003213 RID: 12819 RVA: 0x000FB74C File Offset: 0x000F994C
	public void ShowCards(List<string> cardIds, Vector3 finalPosition, Vector3? origin = null)
	{
		if (this.m_Showing)
		{
			return;
		}
		this.m_Showing = true;
		if (this.m_EnableFullscreenMode)
		{
			this.DoShowCardRewards(cardIds, default(Vector3?), origin, false);
		}
		else
		{
			this.DoShowCardRewards(cardIds, new Vector3?(finalPosition), origin, false);
		}
	}

	// Token: 0x06003214 RID: 12820 RVA: 0x000FB7A0 File Offset: 0x000F99A0
	public void HideCardRewards()
	{
		this.m_Showing = false;
		foreach (Actor actor in this.m_CurrentCardRewards)
		{
			if (actor != null)
			{
				Object.Destroy(actor.gameObject);
			}
		}
		this.m_CurrentCardRewards.Clear();
		this.DisableFullscreen();
	}

	// Token: 0x06003215 RID: 12821 RVA: 0x000FB824 File Offset: 0x000F9A24
	private void DoShowCardRewards(List<string> cardIds, Vector3? finalPosition, Vector3? origin, bool disableFullscreen)
	{
		int num = 0;
		int count = cardIds.Count;
		foreach (string cardId in cardIds)
		{
			FullDef fullDef = DefLoader.Get().GetFullDef(cardId, null);
			GameObject gameObject = AssetLoader.Get().LoadActor(ActorNames.GetHandActor(fullDef.GetEntityDef(), TAG_PREMIUM.NORMAL), false, false);
			Actor component = gameObject.GetComponent<Actor>();
			component.SetCardDef(fullDef.GetCardDef());
			component.SetEntityDef(fullDef.GetEntityDef());
			if (component.m_cardMesh != null)
			{
				BoxCollider component2 = component.m_cardMesh.GetComponent<BoxCollider>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
			}
			this.m_CurrentCardRewards.Add(component);
			GameUtils.SetParent(component, this.m_RewardsCardArea, false);
			this.ShowCardRewardsObject(gameObject, finalPosition, origin, num, count);
			num++;
		}
		this.EnableFullscreen(disableFullscreen);
	}

	// Token: 0x06003216 RID: 12822 RVA: 0x000FB934 File Offset: 0x000F9B34
	private void ShowCardRewardsObject(GameObject obj, Vector3? finalPosition, Vector3? origin, int index, int totalCount)
	{
		Vector3 position;
		if (finalPosition != null)
		{
			Vector3 min = this.m_RewardsCardArea.GetComponent<Collider>().bounds.min;
			Vector3 max = this.m_RewardsCardArea.GetComponent<Collider>().bounds.max;
			position = finalPosition.Value + this.m_RewardsCardOffset;
			float num = (float)index * this.m_RewardsCardSpacing;
			position.z = Mathf.Clamp(position.z, min.z, max.z);
			if (position.x + this.m_RewardsCardMouseOffset > max.x)
			{
				position.x -= this.m_RewardsCardMouseOffset + num;
			}
			else
			{
				position.x += this.m_RewardsCardMouseOffset + num;
			}
		}
		else
		{
			position = this.m_RewardsCardArea.transform.position + this.m_RewardsCardOffset;
			float num2 = (float)index * this.m_RewardsCardSpacing;
			position.x += num2;
			position.x -= (float)(totalCount - 1) * this.m_RewardsCardSpacing * 0.5f;
		}
		obj.transform.localScale = this.m_RewardsCardScale;
		obj.transform.position = position;
		obj.SetActive(true);
		if (this.m_EnableFullscreenMode)
		{
			SceneUtils.SetLayer(obj, GameLayer.IgnoreFullScreenEffects);
			if (this.m_FullscreenModeOffClicker != null)
			{
				SceneUtils.SetLayer(this.m_FullscreenModeOffClicker, GameLayer.IgnoreFullScreenEffects);
			}
		}
		iTween.StopByName(obj, "REWARD_SCALE_UP");
		iTween.ScaleFrom(obj, iTween.Hash(new object[]
		{
			"scale",
			Vector3.one * 0.05f,
			"time",
			0.15f,
			"easeType",
			iTween.EaseType.easeOutQuart,
			"name",
			"REWARD_SCALE_UP"
		}));
		if (origin != null)
		{
			iTween.StopByName(obj, "REWARD_MOVE_FROM_ORIGIN");
			iTween.MoveFrom(obj, iTween.Hash(new object[]
			{
				"position",
				origin.Value,
				"time",
				0.15f,
				"easeType",
				iTween.EaseType.easeOutQuart,
				"name",
				"REWARD_MOVE_FROM_ORIGIN",
				"oncomplete",
				delegate(object o)
				{
					if (this.m_RewardsCardDriftAmount != Vector3.zero)
					{
						AnimationUtil.DriftObject(obj, this.m_RewardsCardDriftAmount);
					}
				}
			}));
		}
		else if (this.m_RewardsCardDriftAmount != Vector3.zero)
		{
			AnimationUtil.DriftObject(obj, this.m_RewardsCardDriftAmount);
		}
		if (!string.IsNullOrEmpty(this.m_CardPreviewAppearSound))
		{
			string soundName = FileUtils.GameAssetPathToName(this.m_CardPreviewAppearSound);
			SoundManager.Get().LoadAndPlay(soundName);
		}
	}

	// Token: 0x06003217 RID: 12823 RVA: 0x000FBC5C File Offset: 0x000F9E5C
	private void EnableFullscreen(bool disableFullscreen)
	{
		if (this.m_EnableFullscreenMode && !disableFullscreen)
		{
			FullScreenFXMgr.Get().StartStandardBlurVignette(0.25f);
			if (this.m_FullscreenModeOffClicker != null)
			{
				this.m_FullscreenModeOffClicker.gameObject.SetActive(true);
			}
			this.m_FullscreenEnabled = true;
		}
	}

	// Token: 0x06003218 RID: 12824 RVA: 0x000FBCB4 File Offset: 0x000F9EB4
	private void DisableFullscreen()
	{
		if (!this.m_FullscreenEnabled)
		{
			return;
		}
		if (FullScreenFXMgr.Get() != null)
		{
			FullScreenFXMgr.Get().EndStandardBlurVignette(0.25f, null);
		}
		if (this.m_FullscreenModeOffClicker != null)
		{
			this.m_FullscreenModeOffClicker.gameObject.SetActive(false);
		}
		this.m_FullscreenEnabled = false;
	}

	// Token: 0x04001F45 RID: 8005
	[CustomEditField(Sections = "UI")]
	public GameObject m_RewardsCardArea;

	// Token: 0x04001F46 RID: 8006
	[CustomEditField(Sections = "UI")]
	public Vector3 m_RewardsCardOffset;

	// Token: 0x04001F47 RID: 8007
	[CustomEditField(Sections = "UI")]
	public float m_RewardsCardMouseOffset;

	// Token: 0x04001F48 RID: 8008
	[CustomEditField(Sections = "UI")]
	public Vector3 m_RewardsCardScale;

	// Token: 0x04001F49 RID: 8009
	[CustomEditField(Sections = "UI")]
	public float m_RewardsCardSpacing = 10f;

	// Token: 0x04001F4A RID: 8010
	[CustomEditField(Sections = "UI")]
	public Vector3 m_RewardsCardDriftAmount;

	// Token: 0x04001F4B RID: 8011
	[CustomEditField(Sections = "UI")]
	public bool m_EnableFullscreenMode;

	// Token: 0x04001F4C RID: 8012
	[CustomEditField(Sections = "UI", Parent = "m_EnableFullscreenMode")]
	public PegUIElement m_FullscreenModeOffClicker;

	// Token: 0x04001F4D RID: 8013
	[CustomEditField(Sections = "UI", Parent = "m_EnableFullscreenMode")]
	public UIBScrollable m_FullscreenDisableScrollBar;

	// Token: 0x04001F4E RID: 8014
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public string m_CardPreviewAppearSound;

	// Token: 0x04001F4F RID: 8015
	private List<Actor> m_CurrentCardRewards = new List<Actor>();

	// Token: 0x04001F50 RID: 8016
	private bool m_FullscreenEnabled;

	// Token: 0x04001F51 RID: 8017
	private bool m_Showing;
}
