using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200047D RID: 1149
public class SeasonEndDialog : DialogBase
{
	// Token: 0x060037C4 RID: 14276 RVA: 0x00111CD4 File Offset: 0x0010FED4
	protected override void Awake()
	{
		base.Awake();
		Object.DontDestroyOnLoad(base.gameObject);
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.m_rewardChestInstructions.Text = GameStrings.Format("GLOBAL_SEASON_END_CHEST_INSTRUCTIONS_TOUCH", new object[0]);
		}
		if (this.TESTING)
		{
			return;
		}
		this.m_okayButton.SetText(GameStrings.Get("GLOBAL_BUTTON_NEXT"));
		this.m_okayButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OkayButtonReleased));
	}

	// Token: 0x060037C5 RID: 14277 RVA: 0x00111D6C File Offset: 0x0010FF6C
	private void Start()
	{
		this.m_medal = this.m_medalContainer.PrefabGameObject().GetComponent<TournamentMedal>();
		SceneUtils.SetLayer(this.m_medal, GameLayer.PerspectiveUI);
		this.m_medal.SetEnabled(false);
	}

	// Token: 0x060037C6 RID: 14278 RVA: 0x00111DA0 File Offset: 0x0010FFA0
	public void Init(SeasonEndDialog.SeasonEndInfo info)
	{
		this.m_seasonEndInfo = info;
		this.m_header.Text = this.GetSeasonName(info.m_seasonID);
		if (info.m_rankedRewards != null && info.m_rankedRewards.Count > 0)
		{
			this.m_earnedRewardChest = true;
		}
		else
		{
			this.m_earnedRewardChest = false;
		}
		this.m_medal.SetMedal(new MedalInfoTranslator(info.m_rank, info.m_legendIndex, info.m_rank, info.m_legendIndex), false);
		this.m_medal.SetFormat(info.m_isWild);
		this.m_rankName.Text = this.m_medal.GetMedal().name;
		this.m_bonusStars = info.m_bonusStars;
		string rankPercentile = SeasonEndDialog.GetRankPercentile(this.m_seasonEndInfo.m_rank);
		if (rankPercentile.Length > 0)
		{
			this.m_rankPercentile.gameObject.SetActive(true);
			this.m_rankPercentile.Text = GameStrings.Format("GLOBAL_SEASON_END_PERCENTILE_LABEL", new object[]
			{
				rankPercentile
			});
		}
		else
		{
			this.m_rankPercentile.gameObject.SetActive(false);
		}
		foreach (PegUIElement pegUIElement in this.m_rewardChests)
		{
			pegUIElement.gameObject.SetActive(false);
		}
		int chestIndexFromRank = RankedRewardChest.GetChestIndexFromRank(this.m_seasonEndInfo.m_chestRank);
		if (chestIndexFromRank >= 0)
		{
			this.m_rewardChest = this.m_rewardChests[chestIndexFromRank];
			this.m_rewardChest.gameObject.SetActive(true);
			FsmGameObject fsmGameObject = this.m_medalPlayMaker.FsmVariables.GetFsmGameObject("RankChest");
			fsmGameObject.Value = this.m_rewardChest.gameObject;
			UberText[] componentsInChildren = this.m_rewardChest.GetComponentsInChildren<UberText>(true);
			if (componentsInChildren.Length > 0)
			{
				componentsInChildren[0].Text = info.m_chestRank.ToString();
			}
			this.m_rewardChestHeader.Text = RankedRewardChest.GetChestEarnedFromRank(this.m_seasonEndInfo.m_chestRank);
		}
		this.m_rewardChest.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ChestButtonReleased));
	}

	// Token: 0x060037C7 RID: 14279 RVA: 0x00111FD4 File Offset: 0x001101D4
	public void OnDestroy()
	{
		if (SceneMgr.Get())
		{
			SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		}
	}

	// Token: 0x060037C8 RID: 14280 RVA: 0x00112007 File Offset: 0x00110207
	public void ShowMedal()
	{
		this.m_medal.gameObject.SetActive(true);
	}

	// Token: 0x060037C9 RID: 14281 RVA: 0x0011201A File Offset: 0x0011021A
	public void HideMedal()
	{
		this.m_medal.gameObject.SetActive(false);
	}

	// Token: 0x060037CA RID: 14282 RVA: 0x00112030 File Offset: 0x00110230
	public void ShowRewardChest()
	{
		if (this.m_seasonEndInfo.m_rank == 0)
		{
			this.m_legendaryGem.SetActive(true);
		}
		this.m_rewardChestPage.SetActive(true);
		this.m_leftFiligree.transform.position = this.m_rewardChestLeftFiligreeBone.transform.position;
		this.m_rightFiligree.transform.position = this.m_rewardChestRightFiligreeBone.transform.position;
		iTween.FadeTo(this.m_leftFiligree.gameObject, 1f, 0.5f);
		iTween.FadeTo(this.m_rightFiligree.gameObject, 1f, 0.5f);
	}

	// Token: 0x060037CB RID: 14283 RVA: 0x001120D9 File Offset: 0x001102D9
	public void HideRewardChest()
	{
		this.m_rewardChestPage.SetActive(false);
	}

	// Token: 0x060037CC RID: 14284 RVA: 0x001120E7 File Offset: 0x001102E7
	public void HideBonusStarText()
	{
		this.m_bonusStarText.gameObject.SetActive(false);
	}

	// Token: 0x060037CD RID: 14285 RVA: 0x001120FC File Offset: 0x001102FC
	public void MedalAnimationFinished()
	{
		if (this.m_earnedRewardChest)
		{
			this.m_currentMode = SeasonEndDialog.MODE.CHEST_EARNED;
			this.m_medalPlayMaker.SendEvent("RevealRewardChest");
			iTween.FadeTo(this.m_rankAchieved.gameObject, 0f, 0.5f);
		}
		else
		{
			this.GotoBonusStarsOrWelcome();
			this.m_okayButton.SetEnabled(true);
		}
	}

	// Token: 0x060037CE RID: 14286 RVA: 0x0011215C File Offset: 0x0011035C
	public void GotoBonusStarsOrWelcome()
	{
		if (this.m_bonusStars > 0)
		{
			this.GotoBonusStars();
		}
		else
		{
			this.GotoSeasonWelcome();
		}
	}

	// Token: 0x060037CF RID: 14287 RVA: 0x0011217C File Offset: 0x0011037C
	public void GotoBonusStars()
	{
		this.m_currentMode = SeasonEndDialog.MODE.BONUS_STARS;
		this.m_medalPlayMaker.SendEvent("PageTear");
		this.m_rewardChestPage.SetActive(false);
		this.m_welcomeItems.SetActive(false);
		this.m_bonusStarItems.SetActive(true);
		this.m_bonusStarText.Text = this.m_bonusStars.ToString();
		GameStrings.PluralNumber[] pluralNumbers = new GameStrings.PluralNumber[]
		{
			new GameStrings.PluralNumber
			{
				m_number = this.m_bonusStars
			}
		};
		this.m_bonusStarLabel.Text = GameStrings.FormatPlurals("GLOBAL_SEASON_END_BONUS_STARS_LABEL", pluralNumbers, new object[0]);
		this.m_bonusStarTitle.Text = GameStrings.Get("GLOBAL_SEASON_END_BONUS_STAR_TITLE");
	}

	// Token: 0x060037D0 RID: 14288 RVA: 0x00112228 File Offset: 0x00110428
	public void GotoBoostedMedal()
	{
		this.m_currentMode = SeasonEndDialog.MODE.BOOSTED_WELCOME;
		this.m_starPlayMaker.SendEvent("Burst Big");
		this.m_medal.transform.position = this.m_boostedMedalBone.transform.position;
		this.m_medal.SetMedal(new MedalInfoTranslator(this.m_seasonEndInfo.m_boostedRank, 0, this.m_seasonEndInfo.m_boostedRank, 0), false);
		this.m_leftFiligree.transform.position = this.m_boostedMedalLeftFiligreeBone.transform.position;
		this.m_rightFiligree.transform.position = this.m_boostedMedalRightFiligreeBone.transform.position;
	}

	// Token: 0x060037D1 RID: 14289 RVA: 0x001122D8 File Offset: 0x001104D8
	public void StarBurstFinished()
	{
		if (this.m_medal.GetMedal().rank == 0)
		{
			this.m_medalPlayMaker.SendEvent("JustMedalIn");
		}
		else
		{
			this.m_medalPlayMaker.SendEvent("MedalBannerIn");
		}
		this.m_bonusStarText.gameObject.SetActive(false);
		this.m_bonusStarLabel.Text = this.m_medal.GetMedal().name;
		NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
		string inlineSeasonName = this.GetInlineSeasonName(netObject.Season);
		this.m_bonusStarTitle.Text = GameStrings.Format("GLOBAL_SEASON_END_NEW_SEASON", new object[]
		{
			inlineSeasonName
		});
	}

	// Token: 0x060037D2 RID: 14290 RVA: 0x00112384 File Offset: 0x00110584
	public void GotoSeasonWelcome()
	{
		this.m_currentMode = SeasonEndDialog.MODE.SEASON_WELCOME;
		this.m_medalPlayMaker.SendEvent("PageTear");
		this.m_welcomeItems.SetActive(true);
		this.m_bonusStarItems.SetActive(false);
		NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
		string seasonName = this.GetSeasonName(netObject.Season);
		this.m_header.Text = seasonName;
		this.m_welcomeDetails.Text = GameStrings.Format("GLOBAL_SEASON_END_NEW_SEASON", new object[]
		{
			seasonName
		});
	}

	// Token: 0x060037D3 RID: 14291 RVA: 0x00112404 File Offset: 0x00110604
	public void PageTearFinished()
	{
		if (this.m_currentMode == SeasonEndDialog.MODE.SEASON_WELCOME)
		{
			this.m_okayButton.SetText("GLOBAL_DONE");
		}
		this.m_okayButton.SetEnabled(true);
	}

	// Token: 0x060037D4 RID: 14292 RVA: 0x00112439 File Offset: 0x00110639
	public void MedalInFinished()
	{
		this.m_okayButton.SetText("GLOBAL_DONE");
		this.m_okayButton.SetEnabled(true);
	}

	// Token: 0x060037D5 RID: 14293 RVA: 0x00112458 File Offset: 0x00110658
	public override void Show()
	{
		this.FadeEffectsIn();
		base.Show();
		base.DoShowAnimation();
		UniversalInputManager.Get().SetGameDialogActive(true);
		SoundManager.Get().LoadAndPlay("rank_window_expand");
	}

	// Token: 0x060037D6 RID: 14294 RVA: 0x00112491 File Offset: 0x00110691
	public override void Hide()
	{
		base.Hide();
		this.FadeEffectsOut();
		SoundManager.Get().LoadAndPlay("rank_window_shrink");
	}

	// Token: 0x060037D7 RID: 14295 RVA: 0x001124AE File Offset: 0x001106AE
	protected override void OnHideAnimFinished()
	{
		UniversalInputManager.Get().SetGameDialogActive(false);
		base.OnHideAnimFinished();
	}

	// Token: 0x060037D8 RID: 14296 RVA: 0x001124C4 File Offset: 0x001106C4
	private void OkayButtonReleased(UIEvent e)
	{
		if (this.m_currentMode == SeasonEndDialog.MODE.SEASON_WELCOME || this.m_currentMode == SeasonEndDialog.MODE.BOOSTED_WELCOME)
		{
			this.Hide();
			foreach (long id in this.m_seasonEndInfo.m_noticesToAck)
			{
				Network.AckNotice(id);
			}
			this.m_okayButton.SetEnabled(false);
			return;
		}
		if (this.m_currentMode == SeasonEndDialog.MODE.RANK_EARNED)
		{
			this.m_ribbon.GetComponent<Renderer>().material = this.m_transparentMaterial;
			this.m_nameFlourish.GetComponent<Renderer>().material = this.m_transparentMaterial;
			iTween.FadeTo(this.m_nameFlourish.gameObject, 0f, 0.5f);
			iTween.FadeTo(this.m_rankName.gameObject, 0f, 0.5f);
			iTween.FadeTo(this.m_rankAchieved.gameObject, 0f, 0.5f);
			iTween.FadeTo(this.m_leftFiligree.gameObject, 0f, 0.5f);
			iTween.FadeTo(this.m_rightFiligree.gameObject, 0f, 0.5f);
			if (this.m_medal.GetMedal().rank == 0)
			{
				this.m_medalPlayMaker.SendEvent("JustMedal");
			}
			else
			{
				this.m_medalPlayMaker.SendEvent("MedalBanner");
			}
			this.m_okayButton.SetEnabled(false);
			this.m_rankPercentile.gameObject.SetActive(false);
			return;
		}
		if (this.m_currentMode == SeasonEndDialog.MODE.BONUS_STARS)
		{
			this.GotoBoostedMedal();
			return;
		}
	}

	// Token: 0x060037D9 RID: 14297 RVA: 0x00112674 File Offset: 0x00110874
	private void ChestButtonReleased(UIEvent e)
	{
		if (this.m_chestOpened)
		{
			return;
		}
		this.m_chestOpened = true;
		this.m_rewardChest.GetComponent<PlayMakerFSM>().SendEvent("StartAnim");
	}

	// Token: 0x060037DA RID: 14298 RVA: 0x001126AC File Offset: 0x001108AC
	private void OpenRewards()
	{
		AssetLoader.GameObjectCallback callback = delegate(string name, GameObject go, object callbackData)
		{
			if (SoundManager.Get() != null)
			{
				SoundManager.Get().LoadAndPlay("card_turn_over_legendary");
			}
			RewardBoxesDisplay component = go.GetComponent<RewardBoxesDisplay>();
			component.SetRewards(this.m_seasonEndInfo.m_rankedRewards);
			component.m_playBoxFlyoutSound = false;
			component.SetLayer(GameLayer.PerspectiveUI);
			component.UseDarkeningClickCatcher(true);
			component.RegisterDoneCallback(delegate
			{
				this.m_rewardChest.GetComponent<PlayMakerFSM>().SendEvent("SummonOut");
			});
			component.transform.localPosition = this.m_rewardBoxesBone.transform.localPosition;
			component.transform.localRotation = this.m_rewardBoxesBone.transform.localRotation;
			component.transform.localScale = this.m_rewardBoxesBone.transform.localScale;
			component.AnimateRewards();
		};
		AssetLoader.Get().LoadGameObject("RewardBoxes", callback, null, false);
		iTween.FadeTo(this.m_rewardChestInstructions.gameObject, 0f, 0.5f);
	}

	// Token: 0x060037DB RID: 14299 RVA: 0x001126F4 File Offset: 0x001108F4
	private void FadeEffectsIn()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.SetBlurBrightness(1f);
		fullScreenFXMgr.SetBlurDesaturation(0f);
		fullScreenFXMgr.Vignette(0.4f, 0.4f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.Blur(1f, 0.4f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x060037DC RID: 14300 RVA: 0x00112744 File Offset: 0x00110944
	private void FadeEffectsOut()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x060037DD RID: 14301 RVA: 0x00112774 File Offset: 0x00110974
	private string GetSeasonName(int seasonID)
	{
		SeasonDbfRecord record = GameDbf.Season.GetRecord(seasonID);
		if (record == null)
		{
			Debug.LogError(string.Format("SeasonEndDialog.GetSeasonName() - There is no Season DBF record for ID {0}", seasonID));
			return "NO RECORD FOUND";
		}
		return record.Name;
	}

	// Token: 0x060037DE RID: 14302 RVA: 0x001127BC File Offset: 0x001109BC
	private string GetInlineSeasonName(int seasonID)
	{
		SeasonDbfRecord record = GameDbf.Season.GetRecord(seasonID);
		if (record == null)
		{
			Debug.LogError(string.Format("SeasonEndDialog.GetInlineSeasonName() - There is no Season DBF record for ID {0}", seasonID));
			return "NO RECORD FOUND";
		}
		return record.SeasonStartName;
	}

	// Token: 0x060037DF RID: 14303 RVA: 0x00112804 File Offset: 0x00110A04
	public static string GetRankPercentile(int rank)
	{
		int num = rank - 1;
		if (num < 0)
		{
			num = 0;
		}
		if (num >= SeasonEndDialog.s_percentiles.Length)
		{
			return string.Empty;
		}
		if (Localization.DoesLocaleUseDecimalPoint(Localization.GetLocale()))
		{
			return SeasonEndDialog.s_percentiles[num];
		}
		return SeasonEndDialog.s_percentiles[num].Replace(".", ",");
	}

	// Token: 0x060037E0 RID: 14304 RVA: 0x0011285E File Offset: 0x00110A5E
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		if (mode != SceneMgr.Mode.HUB)
		{
			this.Hide();
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040023AC RID: 9132
	public UIBButton m_okayButton;

	// Token: 0x040023AD RID: 9133
	public Vector3 m_loadPosition;

	// Token: 0x040023AE RID: 9134
	public Vector3 m_showPosition;

	// Token: 0x040023AF RID: 9135
	public GameObject m_boostedMedalBone;

	// Token: 0x040023B0 RID: 9136
	public GameObject m_boostedMedalLeftFiligreeBone;

	// Token: 0x040023B1 RID: 9137
	public GameObject m_boostedMedalRightFiligreeBone;

	// Token: 0x040023B2 RID: 9138
	public GameObject m_rewardChestPage;

	// Token: 0x040023B3 RID: 9139
	public PegUIElement m_rewardChest;

	// Token: 0x040023B4 RID: 9140
	public UberText m_rewardChestHeader;

	// Token: 0x040023B5 RID: 9141
	public UberText m_rewardChestInstructions;

	// Token: 0x040023B6 RID: 9142
	public GameObject m_rewardChestLeftFiligreeBone;

	// Token: 0x040023B7 RID: 9143
	public GameObject m_rewardChestRightFiligreeBone;

	// Token: 0x040023B8 RID: 9144
	public GameObject m_rewardBoxesBone;

	// Token: 0x040023B9 RID: 9145
	public NestedPrefab m_medalContainer;

	// Token: 0x040023BA RID: 9146
	public UberText m_header;

	// Token: 0x040023BB RID: 9147
	public UberText m_rankAchieved;

	// Token: 0x040023BC RID: 9148
	public UberText m_rankName;

	// Token: 0x040023BD RID: 9149
	public UberText m_rankPercentile;

	// Token: 0x040023BE RID: 9150
	public GameObject m_ribbon;

	// Token: 0x040023BF RID: 9151
	public GameObject m_nameFlourish;

	// Token: 0x040023C0 RID: 9152
	public GameObject m_welcomeItems;

	// Token: 0x040023C1 RID: 9153
	public GameObject m_leftFiligree;

	// Token: 0x040023C2 RID: 9154
	public GameObject m_rightFiligree;

	// Token: 0x040023C3 RID: 9155
	public UberText m_welcomeDetails;

	// Token: 0x040023C4 RID: 9156
	public UberText m_welcomeTitle;

	// Token: 0x040023C5 RID: 9157
	public GameObject m_shieldIcon;

	// Token: 0x040023C6 RID: 9158
	public GameObject m_bonusStarItems;

	// Token: 0x040023C7 RID: 9159
	public UberText m_bonusStarTitle;

	// Token: 0x040023C8 RID: 9160
	public UberText m_bonusStarLabel;

	// Token: 0x040023C9 RID: 9161
	public GameObject m_bonusStar;

	// Token: 0x040023CA RID: 9162
	public UberText m_bonusStarText;

	// Token: 0x040023CB RID: 9163
	public GameObject m_bonusStarFlourish;

	// Token: 0x040023CC RID: 9164
	public Material m_transparentMaterial;

	// Token: 0x040023CD RID: 9165
	public PlayMakerFSM m_medalPlayMaker;

	// Token: 0x040023CE RID: 9166
	public PlayMakerFSM m_starPlayMaker;

	// Token: 0x040023CF RID: 9167
	public GameObject m_legendaryGem;

	// Token: 0x040023D0 RID: 9168
	public List<PegUIElement> m_rewardChests;

	// Token: 0x040023D1 RID: 9169
	private SeasonEndDialog.SeasonEndInfo m_seasonEndInfo;

	// Token: 0x040023D2 RID: 9170
	private bool m_earnedRewardChest;

	// Token: 0x040023D3 RID: 9171
	private SeasonEndDialog.MODE m_currentMode;

	// Token: 0x040023D4 RID: 9172
	private bool TESTING;

	// Token: 0x040023D5 RID: 9173
	private int m_bonusStars;

	// Token: 0x040023D6 RID: 9174
	private bool m_chestOpened;

	// Token: 0x040023D7 RID: 9175
	private TournamentMedal m_medal;

	// Token: 0x040023D8 RID: 9176
	private static string[] s_percentiles = new string[]
	{
		"0.25",
		"0.33",
		"0.5",
		"1",
		"2",
		"3",
		"4",
		"5",
		"7",
		"9",
		"12",
		"15",
		"20",
		"25",
		"30",
		"40",
		"45",
		"50"
	};

	// Token: 0x02000486 RID: 1158
	public class SeasonEndInfo
	{
		// Token: 0x040023FB RID: 9211
		public int m_seasonID;

		// Token: 0x040023FC RID: 9212
		public int m_rank;

		// Token: 0x040023FD RID: 9213
		public int m_chestRank;

		// Token: 0x040023FE RID: 9214
		public int m_legendIndex;

		// Token: 0x040023FF RID: 9215
		public int m_bonusStars;

		// Token: 0x04002400 RID: 9216
		public int m_boostedRank;

		// Token: 0x04002401 RID: 9217
		public List<RewardData> m_rankedRewards;

		// Token: 0x04002402 RID: 9218
		public List<long> m_noticesToAck = new List<long>();

		// Token: 0x04002403 RID: 9219
		public bool m_isWild;

		// Token: 0x04002404 RID: 9220
		public bool m_isFake;
	}

	// Token: 0x02000EBC RID: 3772
	private enum MODE
	{
		// Token: 0x04005AE7 RID: 23271
		RANK_EARNED,
		// Token: 0x04005AE8 RID: 23272
		CHEST_EARNED,
		// Token: 0x04005AE9 RID: 23273
		SEASON_WELCOME,
		// Token: 0x04005AEA RID: 23274
		BONUS_STARS,
		// Token: 0x04005AEB RID: 23275
		BOOSTED_WELCOME
	}
}
