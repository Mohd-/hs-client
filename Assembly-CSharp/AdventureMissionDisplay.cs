using System;
using System.Collections;
using System.Collections.Generic;
using PegasusUtil;
using UnityEngine;

// Token: 0x020003A5 RID: 933
[CustomEditClass]
public class AdventureMissionDisplay : MonoBehaviour
{
	// Token: 0x170003DD RID: 989
	// (get) Token: 0x06003125 RID: 12581 RVA: 0x000F6B1E File Offset: 0x000F4D1E
	// (set) Token: 0x06003126 RID: 12582 RVA: 0x000F6B26 File Offset: 0x000F4D26
	[CustomEditField(Sections = "Boss Layout Settings")]
	public float BossWingHeight
	{
		get
		{
			return this.m_BossWingHeight;
		}
		set
		{
			this.m_BossWingHeight = value;
			this.UpdateWingPositions();
		}
	}

	// Token: 0x170003DE RID: 990
	// (get) Token: 0x06003127 RID: 12583 RVA: 0x000F6B35 File Offset: 0x000F4D35
	// (set) Token: 0x06003128 RID: 12584 RVA: 0x000F6B3D File Offset: 0x000F4D3D
	[CustomEditField(Sections = "Boss Layout Settings")]
	public Vector3 BossWingOffset
	{
		get
		{
			return this.m_BossWingOffset;
		}
		set
		{
			this.m_BossWingOffset = value;
			this.UpdateWingPositions();
		}
	}

	// Token: 0x06003129 RID: 12585 RVA: 0x000F6B4C File Offset: 0x000F4D4C
	public static AdventureMissionDisplay Get()
	{
		return AdventureMissionDisplay.s_instance;
	}

	// Token: 0x0600312A RID: 12586 RVA: 0x000F6B54 File Offset: 0x000F4D54
	private void Awake()
	{
		AdventureMissionDisplay.s_instance = this;
		this.m_mainMusic = MusicManager.Get().GetCurrentPlaylist();
		AdventureConfig adventureConfig = AdventureConfig.Get();
		AdventureDbId selectedAdventure = adventureConfig.GetSelectedAdventure();
		AdventureModeDbId selectedMode = adventureConfig.GetSelectedMode();
		AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int)selectedAdventure, (int)selectedMode);
		string text = adventureDataRecord.Name;
		this.m_AdventureTitle.Text = text;
		List<AdventureMissionDisplay.WingCreateParams> list = this.BuildWingCreateParamsList();
		this.m_WingsToGiveBigChest.Clear();
		AdventureDef adventureDef = AdventureScene.Get().GetAdventureDef(selectedAdventure);
		AdventureSubDef subDef = adventureDef.GetSubDef(selectedMode);
		if (!string.IsNullOrEmpty(adventureDef.m_WingBottomBorderPrefab))
		{
			this.m_BossWingBorder = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(adventureDef.m_WingBottomBorderPrefab), true, false);
			GameUtils.SetParent(this.m_BossWingBorder, this.m_BossWingContainer, false);
		}
		this.AddAssetToLoad(3);
		foreach (AdventureMissionDisplay.WingCreateParams wingCreateParams in list)
		{
			this.AddAssetToLoad(wingCreateParams.m_BossCreateParams.Count * 2);
		}
		this.m_TotalBosses = 0;
		this.m_TotalBossesDefeated = 0;
		int num = 0;
		int num2 = list.Count - 1;
		if (!string.IsNullOrEmpty(adventureDef.m_ProgressDisplayPrefab))
		{
			this.m_progressDisplay = GameUtils.LoadGameObjectWithComponent<AdventureWingProgressDisplay>(adventureDef.m_ProgressDisplayPrefab);
			if (this.m_progressDisplay != null)
			{
				num++;
				num2++;
				if (this.m_BossWingContainer != null)
				{
					GameUtils.SetParent(this.m_progressDisplay, this.m_BossWingContainer, false);
				}
			}
		}
		foreach (AdventureMissionDisplay.WingCreateParams wingCreateParams2 in list)
		{
			WingDbId wingId = wingCreateParams2.m_WingDef.GetWingId();
			AdventureWingDef wingDef = wingCreateParams2.m_WingDef;
			AdventureWing wing = GameUtils.LoadGameObjectWithComponent<AdventureWing>(wingDef.m_WingPrefab);
			if (!(wing == null))
			{
				if (this.m_BossWingContainer != null)
				{
					GameUtils.SetParent(wing, this.m_BossWingContainer, false);
				}
				AdventureWingDef wingDef2 = AdventureScene.Get().GetWingDef(wingCreateParams2.m_WingDef.GetOwnershipPrereqId());
				wing.Initialize(wingDef, wingDef2);
				wing.SetBigChestRewards(wingId);
				wing.AddBossSelectedListener(delegate(AdventureBossCoin c, ScenarioDbId m)
				{
					this.OnBossSelected(c, m, true);
				});
				wing.AddOpenPlateStartListener(new AdventureWing.OpenPlateStart(this.OnStartUnlockPlate));
				wing.AddOpenPlateEndListener(new AdventureWing.OpenPlateEnd(this.OnEndUnlockPlate));
				wing.AddTryPurchaseWingListener(delegate
				{
					this.ShowAdventureStore(wing);
				});
				wing.AddShowCardRewardsListener(delegate(List<CardRewardData> r, Vector3 o)
				{
					this.m_RewardsDisplay.ShowCards(r, o, default(Vector3?));
				});
				wing.AddHideCardRewardsListener(delegate(List<CardRewardData> r)
				{
					this.m_RewardsDisplay.HideCardRewards();
				});
				List<int> wingScenarios = new List<int>();
				int num3 = 0;
				foreach (AdventureMissionDisplay.BossCreateParams bossCreateParams in wingCreateParams2.m_BossCreateParams)
				{
					bool enabled = adventureConfig.IsMissionAvailable((int)bossCreateParams.m_MissionId);
					AdventureBossCoin coin = wing.CreateBoss(wingDef.m_CoinPrefab, wingDef.m_RewardsPrefab, bossCreateParams.m_MissionId, enabled);
					AdventureConfig.Get().LoadBossDef(bossCreateParams.m_MissionId, delegate(AdventureBossDef bossDef, bool y)
					{
						if (bossDef != null && bossDef.m_CoinPortraitMaterial != null)
						{
							coin.SetPortraitMaterial(bossDef.m_CoinPortraitMaterial);
						}
						this.AssetLoadCompleted();
					});
					if (AdventureConfig.Get().GetLastSelectedMission() == bossCreateParams.m_MissionId)
					{
						base.StartCoroutine(this.RememberLastBossSelection(coin, bossCreateParams.m_MissionId));
					}
					if (AdventureProgressMgr.Get().HasDefeatedScenario((int)bossCreateParams.m_MissionId))
					{
						num3++;
						this.m_TotalBossesDefeated++;
					}
					this.m_TotalBosses++;
					DefLoader.Get().LoadFullDef(bossCreateParams.m_CardDefId, new DefLoader.LoadDefCallback<FullDef>(this.OnHeroFullDefLoaded), bossCreateParams.m_MissionId);
					wingScenarios.Add((int)bossCreateParams.m_MissionId);
				}
				int wingBossesDefeated = adventureConfig.GetWingBossesDefeated(selectedAdventure, selectedMode, wingId, num3);
				if (wingBossesDefeated != num3)
				{
					this.m_BossJustDefeated = true;
				}
				bool flag = num3 == wingCreateParams2.m_BossCreateParams.Count || AdventureScene.Get().IsDevMode;
				if (!wing.HasBigChestRewards())
				{
					wing.HideBigChest();
				}
				else if (flag)
				{
					if (wingBossesDefeated != num3)
					{
						this.m_WingsToGiveBigChest.Add(wing);
					}
					else
					{
						wing.BigChestStayOpen();
					}
				}
				if (this.m_progressDisplay != null)
				{
					bool normalComplete = GameUtils.IsWingComplete((int)selectedAdventure, 1, (int)wingId);
					this.m_progressDisplay.UpdateProgress(wingCreateParams2.m_WingDef.GetWingId(), normalComplete);
				}
				adventureConfig.UpdateWingBossesDefeated(selectedAdventure, selectedMode, wingId, num3);
				wing.AddShowRewardsPreviewListeners(delegate
				{
					this.ShowRewardsPreview(wing, wingScenarios.ToArray(), wing.GetBigChestRewards(), wing.GetWingName());
				});
				wing.UpdateRewardsPreviewCover();
				wing.RandomizeBackground();
				float focusScrollPos = (float)num++ / (float)num2;
				wing.SetBringToFocusCallback(delegate(Vector3 position, float time)
				{
					this.BringWingToFocus(focusScrollPos);
				});
				this.m_BossWings.Add(wing);
			}
		}
		AssetLoader.Get().LoadActor("Card_Play_Hero", new AssetLoader.GameObjectCallback(this.OnHeroActorLoaded), null, false);
		AssetLoader.Get().LoadActor("Card_Play_HeroPower", new AssetLoader.GameObjectCallback(this.OnHeroPowerActorLoaded), null, false);
		AssetLoader.Get().LoadActor("History_HeroPower_Opponent", new AssetLoader.GameObjectCallback(this.OnHeroPowerBigCardLoaded), null, false);
		if (this.m_BossPowerHoverArea != null)
		{
			this.m_BossPowerHoverArea.AddEventListener(UIEventType.ROLLOVER, delegate(UIEvent e)
			{
				this.ShowHeroPowerBigCard();
			});
			this.m_BossPowerHoverArea.AddEventListener(UIEventType.ROLLOUT, delegate(UIEvent e)
			{
				this.HideHeroPowerBigCard();
			});
		}
		this.m_BackButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonPress));
		this.m_ChooseButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.ChangeToDeckPicker();
		});
		this.UpdateWingPositions();
		this.m_ChooseButton.Disable();
		this.DoAutoPurchaseWings(selectedAdventure, selectedMode);
		StoreManager.Get().RegisterStoreShownListener(new StoreManager.StoreShownCallback(this.OnStoreShown));
		StoreManager.Get().RegisterStoreHiddenListener(new StoreManager.StoreHiddenCallback(this.OnStoreHidden));
		AdventureProgressMgr.Get().RegisterProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdate));
		if (this.m_ScrollBar != null)
		{
			this.m_ScrollBar.LoadScroll(AdventureConfig.Get().GetSelectedAdventureAndModeString());
		}
		if (this.m_WatermarkIcon != null && !string.IsNullOrEmpty(subDef.m_WatermarkTexture))
		{
			string name = FileUtils.GameAssetPathToName(subDef.m_WatermarkTexture);
			Texture texture = AssetLoader.Get().LoadTexture(name, false);
			if (texture != null)
			{
				this.m_WatermarkIcon.material.mainTexture = texture;
			}
			else
			{
				Debug.LogWarning(string.Format("Adventure Watermark texture is null: {0}", subDef.m_WatermarkTexture));
			}
		}
		else
		{
			Debug.LogWarning(string.Format("Adventure Watermark texture is null: m_WatermarkIcon: {0},  advSubDef.m_WatermarkTexture: {1}", this.m_WatermarkIcon, subDef.m_WatermarkTexture));
		}
		Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureMissionDisplay.OnNavigateBack));
		this.m_BackButton.gameObject.SetActive(true);
		this.m_PreviewPane.AddHideListener(new AdventureRewardsPreview.OnHide(this.OnHideRewardsPreview));
	}

	// Token: 0x0600312B RID: 12587 RVA: 0x000F7370 File Offset: 0x000F5570
	private void OnDestroy()
	{
		AdventureProgressMgr.Get().RemoveProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdate));
		StoreManager.Get().RemoveStoreHiddenListener(new StoreManager.StoreHiddenCallback(this.OnStoreHidden));
		StoreManager.Get().RemoveStoreShownListener(new StoreManager.StoreShownCallback(this.OnStoreShown));
		if (this.m_ScrollBar != null && AdventureConfig.Get() != null)
		{
			this.m_ScrollBar.SaveScroll(AdventureConfig.Get().GetSelectedAdventureAndModeString());
		}
		AdventureMissionDisplay.s_instance = null;
	}

	// Token: 0x0600312C RID: 12588 RVA: 0x000F7400 File Offset: 0x000F5600
	private void Update()
	{
		if (!AdventureScene.Get().IsDevMode || AdventureScene.Get().DevModeSetting != 2)
		{
			return;
		}
		if (Input.GetKeyDown(122))
		{
			base.StartCoroutine(this.AnimateFancyCheckmarksEffects());
		}
		if (Input.GetKeyDown(120))
		{
			this.ShowAdventureComplete();
		}
	}

	// Token: 0x0600312D RID: 12589 RVA: 0x000F7458 File Offset: 0x000F5658
	public bool IsDisabledSelection()
	{
		return this.m_DisableSelectionCount > 0;
	}

	// Token: 0x0600312E RID: 12590 RVA: 0x000F7464 File Offset: 0x000F5664
	private void UpdateWingPositions()
	{
		int num = 0;
		if (this.m_progressDisplay != null)
		{
			this.m_progressDisplay.transform.localPosition = this.m_BossWingOffset;
			TransformUtil.SetLocalPosZ(this.m_progressDisplay, this.m_BossWingOffset.z - (float)num++ * this.m_BossWingHeight);
		}
		foreach (AdventureWing adventureWing in this.m_BossWings)
		{
			adventureWing.transform.localPosition = this.m_BossWingOffset;
			TransformUtil.SetLocalPosZ(adventureWing, this.m_BossWingOffset.z - (float)num++ * this.m_BossWingHeight);
		}
		if (this.m_BossWingBorder != null)
		{
			this.m_BossWingBorder.transform.localPosition = this.m_BossWingOffset;
			TransformUtil.SetLocalPosZ(this.m_BossWingBorder, this.m_BossWingOffset.z - (float)num++ * this.m_BossWingHeight);
		}
	}

	// Token: 0x0600312F RID: 12591 RVA: 0x000F7580 File Offset: 0x000F5780
	private void OnHeroFullDefLoaded(string cardId, FullDef def, object userData)
	{
		if (def == null)
		{
			Debug.LogError(string.Format("Unable to load {0} hero def for Adventure boss.", cardId), base.gameObject);
			this.AssetLoadCompleted();
			return;
		}
		ScenarioDbId scenarioDbId = (ScenarioDbId)((int)userData);
		this.m_BossPortraitDefCache[scenarioDbId] = def;
		string heroPowerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(def.GetEntityDef().GetCardId());
		if (heroPowerCardIdFromHero == null)
		{
			Debug.LogError(string.Format("Unable to load hero power ID from {0} (ID: {1}) for ScenarioDbId={2} ({3}).", new object[]
			{
				cardId,
				def.GetEntityDef().GetCardId(),
				scenarioDbId,
				(int)scenarioDbId
			}), base.gameObject);
		}
		else
		{
			this.AddAssetToLoad(1);
			DefLoader.Get().LoadFullDef(heroPowerCardIdFromHero, new DefLoader.LoadDefCallback<FullDef>(this.OnHeroPowerFullDefLoaded), scenarioDbId);
		}
		this.AssetLoadCompleted();
	}

	// Token: 0x06003130 RID: 12592 RVA: 0x000F764C File Offset: 0x000F584C
	private void OnHeroPowerFullDefLoaded(string cardId, FullDef def, object userData)
	{
		if (def == null)
		{
			Debug.LogError(string.Format("Unable to load {0} hero power def for Adventure boss.", cardId), base.gameObject);
			this.AssetLoadCompleted();
			return;
		}
		ScenarioDbId key = (ScenarioDbId)((int)userData);
		this.m_BossPowerDefCache[key] = def;
		this.AssetLoadCompleted();
	}

	// Token: 0x06003131 RID: 12593 RVA: 0x000F7698 File Offset: 0x000F5898
	private void OnHeroActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		this.m_BossActor = this.OnActorLoaded(actorName, actorObject, this.m_BossPortraitContainer);
		if (this.m_BossActor != null && this.m_BossActor.GetHealthObject() != null)
		{
			this.m_BossActor.GetHealthObject().Hide();
		}
		this.AssetLoadCompleted();
	}

	// Token: 0x06003132 RID: 12594 RVA: 0x000F76F6 File Offset: 0x000F58F6
	private void OnHeroPowerActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		this.m_BossPowerActor = this.OnActorLoaded(actorName, actorObject, this.m_BossPowerContainer);
		this.AssetLoadCompleted();
	}

	// Token: 0x06003133 RID: 12595 RVA: 0x000F7714 File Offset: 0x000F5914
	private void OnHeroPowerBigCardLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		this.m_BossPowerBigCard = this.OnActorLoaded(actorName, actorObject, (!(this.m_BossPowerActor == null)) ? this.m_BossPowerActor.gameObject : null);
		if (this.m_BossPowerBigCard != null)
		{
			this.m_BossPowerBigCard.TurnOffCollider();
		}
		this.AssetLoadCompleted();
	}

	// Token: 0x06003134 RID: 12596 RVA: 0x000F7774 File Offset: 0x000F5974
	private Actor OnActorLoaded(string actorName, GameObject actorObject, GameObject container)
	{
		Actor component = actorObject.GetComponent<Actor>();
		if (component != null)
		{
			if (container != null)
			{
				GameUtils.SetParent(component, container, false);
			}
			SceneUtils.SetLayer(component, container.layer);
			component.SetUnlit();
			component.Hide();
		}
		else
		{
			Debug.LogWarning(string.Format("ERROR actor \"{0}\" has no Actor component", actorName));
		}
		return component;
	}

	// Token: 0x06003135 RID: 12597 RVA: 0x000F77D6 File Offset: 0x000F59D6
	private void AddAssetToLoad(int assetCount = 1)
	{
		this.m_AssetsLoading += assetCount;
	}

	// Token: 0x06003136 RID: 12598 RVA: 0x000F77E8 File Offset: 0x000F59E8
	private void AssetLoadCompleted()
	{
		if (this.m_AssetsLoading > 0)
		{
			this.m_AssetsLoading--;
			if (this.m_AssetsLoading == 0)
			{
				if (this.m_BossPowerBigCard != null && this.m_BossPowerActor != null && this.m_BossPowerBigCard.transform.parent != this.m_BossPowerActor.transform)
				{
					GameUtils.SetParent(this.m_BossPowerBigCard, this.m_BossPowerActor.gameObject, false);
				}
				this.OnSubSceneLoaded();
			}
		}
		else
		{
			Debug.LogError("AssetLoadCompleted() called when no assets left.", base.gameObject);
		}
	}

	// Token: 0x06003137 RID: 12599 RVA: 0x000F7894 File Offset: 0x000F5A94
	private void OnSubSceneLoaded()
	{
		AdventureSubScene component = base.GetComponent<AdventureSubScene>();
		if (component != null)
		{
			component.AddSubSceneTransitionFinishedListener(new AdventureSubScene.SubSceneTransitionFinished(this.OnSubSceneTransitionComplete));
			component.SetIsLoaded(true);
		}
	}

	// Token: 0x06003138 RID: 12600 RVA: 0x000F78D0 File Offset: 0x000F5AD0
	private void OnSubSceneTransitionComplete()
	{
		AdventureSubScene component = base.GetComponent<AdventureSubScene>();
		if (component != null)
		{
			component.RemoveSubSceneTransitionFinishedListener(new AdventureSubScene.SubSceneTransitionFinished(this.OnSubSceneTransitionComplete));
		}
		base.StartCoroutine(this.UpdateAndAnimateWingCoinsAndChests(this.m_BossWings, true, false));
	}

	// Token: 0x06003139 RID: 12601 RVA: 0x000F7917 File Offset: 0x000F5B17
	private static bool OnNavigateBack()
	{
		AdventureConfig.Get().ChangeToLastSubScene(true);
		return true;
	}

	// Token: 0x0600313A RID: 12602 RVA: 0x000F7925 File Offset: 0x000F5B25
	private void OnBackButtonPress(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x0600313B RID: 12603 RVA: 0x000F792D File Offset: 0x000F5B2D
	private void OnZeroCostTransactionStoreExit(bool authorizationBackButtonPressed, object userData)
	{
		if (!authorizationBackButtonPressed)
		{
			return;
		}
		this.OnBackButtonPress(null);
	}

	// Token: 0x0600313C RID: 12604 RVA: 0x000F7940 File Offset: 0x000F5B40
	private void OnBossSelected(AdventureBossCoin coin, ScenarioDbId mission, bool showDetails = true)
	{
		if (this.IsDisabledSelection())
		{
			return;
		}
		if (this.m_SelectedCoin != null)
		{
			this.m_SelectedCoin.Select(false);
		}
		this.m_SelectedCoin = coin;
		this.m_SelectedCoin.Select(true);
		if (this.m_ChooseButton != null)
		{
			if (!this.m_ChooseButton.IsEnabled())
			{
				this.m_ChooseButton.Enable();
			}
			string text = GameStrings.Get((!AdventureConfig.Get().DoesMissionRequireDeck(mission)) ? "GLOBAL_PLAY" : "GLUE_CHOOSE");
			this.m_ChooseButton.SetText(text);
		}
		this.ShowBossFrame(mission);
		AdventureConfig.Get().SetMission(mission, showDetails);
		AdventureBossDef bossDef = AdventureConfig.Get().GetBossDef(mission);
		if (bossDef.m_MissionMusic != MusicPlaylistType.Invalid && !MusicManager.Get().StartPlaylist(bossDef.m_MissionMusic))
		{
			this.ResumeMainMusic();
		}
		if (bossDef.m_IntroLinePlayTime == AdventureBossDef.IntroLinePlayTime.MissionSelect)
		{
			this.PlayMissionQuote(bossDef, this.DetermineCharacterQuotePos(coin.gameObject));
		}
	}

	// Token: 0x0600313D RID: 12605 RVA: 0x000F7A4C File Offset: 0x000F5C4C
	private void PlayMissionQuote(AdventureBossDef bossDef, Vector3 position)
	{
		if (bossDef == null || string.IsNullOrEmpty(bossDef.m_IntroLine))
		{
			return;
		}
		AdventureDef adventureDef = AdventureScene.Get().GetAdventureDef(AdventureConfig.Get().GetSelectedAdventure());
		string text = null;
		if (adventureDef != null)
		{
			text = adventureDef.m_DefaultQuotePrefab;
		}
		if (!string.IsNullOrEmpty(bossDef.m_quotePrefabOverride))
		{
			text = bossDef.m_quotePrefabOverride;
		}
		if (!string.IsNullOrEmpty(text))
		{
			bool allowRepeatDuringSession = AdventureScene.Get() != null && AdventureScene.Get().IsDevMode;
			NotificationManager.Get().CreateCharacterQuote(FileUtils.GameAssetPathToName(text), position, GameStrings.Get(bossDef.m_IntroLine), bossDef.m_IntroLine, allowRepeatDuringSession, 0f, null, CanvasAnchor.BOTTOM_LEFT);
		}
	}

	// Token: 0x0600313E RID: 12606 RVA: 0x000F7B0C File Offset: 0x000F5D0C
	private Vector3 DetermineCharacterQuotePos(GameObject coin)
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			return NotificationManager.PHONE_CHARACTER_POS;
		}
		Bounds boundsOfChildren = TransformUtil.GetBoundsOfChildren(coin);
		Vector3 center = boundsOfChildren.center;
		center.y -= boundsOfChildren.extents.y;
		Camera camera = Box.Get().GetCamera();
		Vector3 vector = camera.WorldToScreenPoint(center);
		float num = 0.4f * (float)camera.pixelHeight;
		if (vector.y < num)
		{
			return NotificationManager.ALT_ADVENTURE_SCREEN_POS;
		}
		return NotificationManager.DEFAULT_CHARACTER_POS;
	}

	// Token: 0x0600313F RID: 12607 RVA: 0x000F7B98 File Offset: 0x000F5D98
	private void ShowBossFrame(ScenarioDbId mission)
	{
		AdventureMissionDisplay.BossInfo bossInfo;
		if (this.m_BossInfoCache.TryGetValue(mission, out bossInfo))
		{
			this.m_BossTitle.Text = bossInfo.m_Title;
			if (this.m_BossDescription != null)
			{
				this.m_BossDescription.Text = bossInfo.m_Description;
			}
		}
		FullDef fullDef;
		if (this.m_BossPortraitDefCache.TryGetValue(mission, out fullDef))
		{
			this.m_BossActor.SetPremium(TAG_PREMIUM.NORMAL);
			this.m_BossActor.SetEntityDef(fullDef.GetEntityDef());
			this.m_BossActor.SetCardDef(fullDef.GetCardDef());
			this.m_BossActor.UpdateAllComponents();
			this.m_BossActor.SetUnlit();
			this.m_BossActor.Show();
		}
		if (this.m_BossPowerDefCache.TryGetValue(mission, out fullDef))
		{
			this.m_BossPowerActor.SetPremium(TAG_PREMIUM.NORMAL);
			this.m_BossPowerActor.SetEntityDef(fullDef.GetEntityDef());
			this.m_BossPowerActor.SetCardDef(fullDef.GetCardDef());
			this.m_BossPowerActor.UpdateAllComponents();
			this.m_BossPowerActor.SetUnlit();
			this.m_BossPowerActor.Show();
			this.m_SelectedHeroPowerFullDef = fullDef;
			if (this.m_BossPowerContainer != null && !this.m_BossPowerContainer.activeSelf)
			{
				this.m_BossPowerContainer.SetActive(true);
			}
		}
	}

	// Token: 0x06003140 RID: 12608 RVA: 0x000F7CE4 File Offset: 0x000F5EE4
	private void UnselectBoss()
	{
		if (this.m_BossTitle != null)
		{
			this.m_BossTitle.Text = string.Empty;
		}
		if (this.m_BossDescription != null)
		{
			this.m_BossDescription.Text = string.Empty;
		}
		this.m_BossActor.Hide();
		if (this.m_BossPowerContainer != null)
		{
			this.m_BossPowerContainer.SetActive(false);
		}
		if (this.m_SelectedCoin != null)
		{
			this.m_SelectedCoin.Select(false);
		}
		this.m_SelectedCoin = null;
		AdventureConfig.Get().SetMission(ScenarioDbId.INVALID, true);
		if (this.m_ChooseButton.IsEnabled())
		{
			this.m_ChooseButton.Disable();
		}
	}

	// Token: 0x06003141 RID: 12609 RVA: 0x000F7DA8 File Offset: 0x000F5FA8
	private void ShowHeroPowerBigCard()
	{
		FullDef selectedHeroPowerFullDef = this.m_SelectedHeroPowerFullDef;
		if (selectedHeroPowerFullDef == null)
		{
			return;
		}
		CardDef cardDef = selectedHeroPowerFullDef.GetCardDef();
		if (cardDef == null)
		{
			return;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			NotificationManager.Get().DestroyActiveQuote(0.2f);
		}
		this.m_BossPowerBigCard.SetCardDef(cardDef);
		this.m_BossPowerBigCard.SetEntityDef(selectedHeroPowerFullDef.GetEntityDef());
		this.m_BossPowerBigCard.UpdateAllComponents();
		this.m_BossPowerBigCard.Show();
		this.m_BossPowerBigCard.transform.localScale = Vector3.one * this.m_BossPowerCardScale;
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_BossPowerBigCard.transform.localPosition = new Vector3(-7.77f, 1.56f, 0.39f);
			this.TweenPower(this.m_BossPowerBigCard.gameObject, new Vector3?(this.m_BossPowerActor.gameObject.transform.position));
		}
		else
		{
			this.m_BossPowerBigCard.transform.localPosition = ((!UniversalInputManager.Get().IsTouchMode()) ? new Vector3(0.019f, 0.54f, -1.12f) : new Vector3(-3.18f, 0.54f, 0.1f));
			this.TweenPower(this.m_BossPowerBigCard.gameObject, default(Vector3?));
		}
	}

	// Token: 0x06003142 RID: 12610 RVA: 0x000F7F14 File Offset: 0x000F6114
	private void TweenPower(GameObject go, Vector3? origin = null)
	{
		Vector3 vector = (!UniversalInputManager.Get().IsTouchMode() || UniversalInputManager.UsePhoneUI) ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(0f, 0.1f, 0.1f);
		if (origin == null)
		{
			iTween.ScaleFrom(go, go.transform.localScale * 0.5f, 0.15f);
			iTween.MoveTo(go, iTween.Hash(new object[]
			{
				"position",
				go.transform.localPosition + vector,
				"isLocal",
				true,
				"time",
				10
			}));
		}
		else
		{
			Vector3 vector2 = TransformUtil.ComputeWorldScale(go.transform.parent);
			Vector3 driftOffset = Vector3.Scale(vector, vector2);
			AnimationUtil.GrowThenDrift(go, origin.Value, driftOffset);
		}
	}

	// Token: 0x06003143 RID: 12611 RVA: 0x000F801D File Offset: 0x000F621D
	private void HideHeroPowerBigCard()
	{
		iTween.Stop(this.m_BossPowerBigCard.gameObject);
		this.m_BossPowerBigCard.Hide();
	}

	// Token: 0x06003144 RID: 12612 RVA: 0x000F803C File Offset: 0x000F623C
	private void ChangeToDeckPicker()
	{
		ScenarioDbId mission = AdventureConfig.Get().GetMission();
		AdventureBossDef bossDef = AdventureConfig.Get().GetBossDef(mission);
		if (bossDef != null && bossDef.m_IntroLinePlayTime == AdventureBossDef.IntroLinePlayTime.MissionStart)
		{
			this.PlayMissionQuote(bossDef, this.DetermineCharacterQuotePos(this.m_ChooseButton.gameObject));
		}
		if (AdventureConfig.Get().DoesSelectedMissionRequireDeck())
		{
			this.m_ChooseButton.Disable();
			this.DisableSelection(true);
			AdventureConfig.Get().ChangeSubScene(AdventureSubScenes.MissionDeckPicker);
		}
		else
		{
			GameMgr.Get().FindGame(1, (int)AdventureConfig.Get().GetMission(), 0L, 0L);
		}
	}

	// Token: 0x06003145 RID: 12613 RVA: 0x000F80DC File Offset: 0x000F62DC
	private void DisableSelection(bool yes)
	{
		if (this.m_ClickBlocker == null)
		{
			return;
		}
		this.m_DisableSelectionCount += ((!yes) ? -1 : 1);
		bool flag = this.IsDisabledSelection();
		if (this.m_ClickBlocker.gameObject.activeSelf != flag)
		{
			this.m_ClickBlocker.gameObject.SetActive(flag);
			this.m_ScrollBar.Enable(!flag);
		}
	}

	// Token: 0x06003146 RID: 12614 RVA: 0x000F8154 File Offset: 0x000F6354
	private IEnumerator UpdateAndAnimateWingCoinsAndChests(List<AdventureWing> wings, bool scrollToCoin, bool forceCoinAnimation = false)
	{
		this.DisableSelection(true);
		if (AdventureScene.Get().IsInitialScreen())
		{
			yield return new WaitForSeconds(1.8f);
		}
		int wingsUpdated = 0;
		int wingIdx = 0;
		int wingIdxMax = wings.Count - 1;
		if (this.m_progressDisplay != null)
		{
			wingIdx++;
			wingIdxMax++;
		}
		foreach (AdventureWing wing in wings)
		{
			AdventureWing.DelOnCoinAnimateCallback func = null;
			if (scrollToCoin)
			{
				int num;
				wingIdx = (num = wingIdx) + 1;
				float scrollPos = (float)num / (float)wingIdxMax;
				func = delegate(Vector3 p)
				{
					this.m_ScrollBar.SetScroll(scrollPos, false, true);
				};
			}
			if (wing.UpdateAndAnimateCoinsAndChests(this.m_CoinFlipDelayTime * (float)wingsUpdated, forceCoinAnimation, func))
			{
				wingsUpdated++;
			}
		}
		if (wingsUpdated > 0)
		{
			yield return new WaitForSeconds(this.m_CoinFlipDelayTime * (float)wingsUpdated + this.m_CoinFlipAnimationTime);
		}
		this.DisableSelection(false);
		yield return base.StartCoroutine(this.AnimateWingBigChestsAndProgressDisplay());
		yield break;
	}

	// Token: 0x06003147 RID: 12615 RVA: 0x000F819C File Offset: 0x000F639C
	private IEnumerator AnimateWingBigChestsAndProgressDisplay()
	{
		if (this.m_WingsToGiveBigChest.Count != 0)
		{
			this.DisableSelection(true);
			if (AdventureScene.Get().IsInitialScreen())
			{
				yield return new WaitForSeconds(1.8f);
			}
			int animDone = 0;
			foreach (AdventureWing wing in this.m_WingsToGiveBigChest)
			{
				animDone++;
				wing.m_WingEventTable.AddOpenChestEndEventListener(delegate(Spell s)
				{
					animDone--;
				}, true);
				wing.OpenBigChest();
				this.m_ScrollBar.CenterWorldPosition(wing.transform.position);
			}
			while (animDone > 0)
			{
				yield return null;
			}
			base.StartCoroutine(this.PlayWingNotifications());
			List<int> wingIds = new List<int>();
			foreach (AdventureWing wing2 in this.m_WingsToGiveBigChest)
			{
				wingIds.Add((int)wing2.GetWingId());
			}
			HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
			hashSet.Add(RewardVisualTiming.ADVENTURE_CHEST);
			HashSet<RewardVisualTiming> rewardTimings = hashSet;
			if (UserAttentionManager.CanShowAttentionGrabber("AdventureMissionDisplay.ShowFixedRewards"))
			{
				this.m_WaitingForClassChallengeUnlocks = true;
				if (!FixedRewardsMgr.Get().ShowFixedRewards(UserAttentionBlocker.NONE, rewardTimings, delegate(object userData)
				{
					this.ShowClassChallengeUnlock(wingIds);
				}, new FixedRewardsMgr.DelPositionNonToastReward(this.PositionReward), AdventureMissionDisplay.REWARD_PUNCH_SCALE, AdventureMissionDisplay.REWARD_SCALE))
				{
					this.ShowClassChallengeUnlock(wingIds);
				}
			}
			while (this.m_WaitingForClassChallengeUnlocks)
			{
				yield return null;
			}
			foreach (AdventureWing wing3 in this.m_WingsToGiveBigChest)
			{
				if (!string.IsNullOrEmpty(wing3.GetWingDef().m_CompleteQuotePrefab) && !string.IsNullOrEmpty(wing3.GetWingDef().m_CompleteQuoteVOLine))
				{
					NotificationManager.Get().CreateCharacterQuote(FileUtils.GameAssetPathToName(wing3.GetWingDef().m_CompleteQuotePrefab), GameStrings.Get(wing3.GetWingDef().m_CompleteQuoteVOLine), wing3.GetWingDef().m_CompleteQuoteVOLine, true, 0f, CanvasAnchor.BOTTOM_LEFT);
				}
				wing3.BigChestStayOpen();
			}
			this.m_WingsToGiveBigChest.Clear();
			this.DisableSelection(false);
		}
		base.StartCoroutine(this.AnimateProgressDisplay());
		yield break;
	}

	// Token: 0x06003148 RID: 12616 RVA: 0x000F81B8 File Offset: 0x000F63B8
	private IEnumerator AnimateProgressDisplay()
	{
		if (this.m_progressDisplay != null)
		{
			while (this.m_progressDisplay.HasProgressAnimationToPlay())
			{
				this.m_ScrollBar.SetScroll(0f, false, true);
				this.DisableSelection(true);
				bool isAnimComplete = false;
				this.m_progressDisplay.PlayProgressAnimation(delegate
				{
					this.DisableSelection(false);
					isAnimComplete = true;
				});
				while (!isAnimComplete)
				{
					yield return null;
				}
			}
		}
		base.StartCoroutine(this.AnimateFancyCheckmarksEffects());
		yield break;
	}

	// Token: 0x06003149 RID: 12617 RVA: 0x000F81D4 File Offset: 0x000F63D4
	private IEnumerator AnimateFancyCheckmarksEffects()
	{
		if (this.m_TotalBosses != this.m_TotalBossesDefeated || !this.m_BossJustDefeated)
		{
			yield break;
		}
		List<KeyValuePair<AdventureRewardsChest, float>> chestAnimates = new List<KeyValuePair<AdventureRewardsChest, float>>();
		float animateTime = 0.7f;
		float totalAnimTime = 0f;
		foreach (AdventureWing wing in this.m_BossWings)
		{
			List<AdventureRewardsChest> chests = wing.GetChests();
			foreach (AdventureRewardsChest chest in chests)
			{
				animateTime *= 0.9f;
				if (animateTime < 0.1f)
				{
					animateTime = 0.1f;
				}
				totalAnimTime += animateTime;
				chestAnimates.Add(new KeyValuePair<AdventureRewardsChest, float>(chest, animateTime));
			}
		}
		this.DisableSelection(true);
		float startScroll = 0f;
		if (this.m_progressDisplay != null)
		{
			totalAnimTime -= animateTime;
			startScroll = 1f / (float)this.m_BossWings.Count;
		}
		this.m_ScrollBar.SetScroll(startScroll, iTween.EaseType.easeOutSine, 0.25f, true, true);
		yield return new WaitForSeconds(0.3f);
		this.m_ScrollBar.SetScroll(1f, iTween.EaseType.easeInQuart, totalAnimTime - 0.1f, true, true);
		foreach (KeyValuePair<AdventureRewardsChest, float> kvpair in chestAnimates)
		{
			kvpair.Key.BurstCheckmark();
			yield return new WaitForSeconds(kvpair.Value);
		}
		this.DisableSelection(false);
		this.ShowAdventureComplete();
		yield break;
	}

	// Token: 0x0600314A RID: 12618 RVA: 0x000F81F0 File Offset: 0x000F63F0
	public void ShowClassChallengeUnlock(List<int> classChallengeUnlocks)
	{
		if (classChallengeUnlocks == null || classChallengeUnlocks.Count == 0)
		{
			this.m_WaitingForClassChallengeUnlocks = false;
			return;
		}
		foreach (int wingID in classChallengeUnlocks)
		{
			this.m_ClassChallengeUnlockShowing++;
			ClassChallengeUnlockData classChallengeUnlockData = new ClassChallengeUnlockData(wingID);
			classChallengeUnlockData.LoadRewardObject(delegate(Reward reward, object data)
			{
				reward.RegisterHideListener(delegate(object userData)
				{
					this.m_ClassChallengeUnlockShowing--;
					if (this.m_ClassChallengeUnlockShowing == 0)
					{
						this.m_WaitingForClassChallengeUnlocks = false;
					}
				});
				this.OnRewardObjectLoaded(reward, data);
			});
		}
	}

	// Token: 0x0600314B RID: 12619 RVA: 0x000F8280 File Offset: 0x000F6480
	private void ShowAdventureComplete()
	{
		AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
		AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
		bool isBadlyHurt = false;
		AdventureDef adventureDef = AdventureScene.Get().GetAdventureDef(selectedAdventure);
		AdventureModeDbId adventureModeDbId = selectedMode;
		if (adventureModeDbId == AdventureModeDbId.HEROIC || adventureModeDbId == AdventureModeDbId.CLASS_CHALLENGE)
		{
			isBadlyHurt = true;
		}
		this.DisableSelection(true);
		AdventureSubDef subDef = adventureDef.GetSubDef(selectedMode);
		AdventureDef.BannerRewardType bannerRewardType = adventureDef.m_BannerRewardType;
		if (bannerRewardType != AdventureDef.BannerRewardType.AdventureCompleteReward)
		{
			if (bannerRewardType == AdventureDef.BannerRewardType.BannerManagerPopup)
			{
				BannerManager.Get().ShowCustomBanner(adventureDef.m_BannerRewardPrefab, subDef.GetCompleteBannerText(), delegate
				{
					this.DisableSelection(false);
				});
			}
		}
		else
		{
			AdventureCompleteRewardData adventureCompleteRewardData = new AdventureCompleteRewardData(FileUtils.GameAssetPathToName(adventureDef.m_BannerRewardPrefab), subDef.GetCompleteBannerText(), isBadlyHurt);
			adventureCompleteRewardData.LoadRewardObject(delegate(Reward reward, object data)
			{
				reward.RegisterHideListener(delegate(object userData)
				{
					this.DisableSelection(false);
				});
				this.OnRewardObjectLoaded(reward, data);
			});
		}
	}

	// Token: 0x0600314C RID: 12620 RVA: 0x000F8357 File Offset: 0x000F6557
	private void PositionReward(Reward reward)
	{
		GameUtils.SetParent(reward, base.transform, false);
	}

	// Token: 0x0600314D RID: 12621 RVA: 0x000F8366 File Offset: 0x000F6566
	private void OnRewardObjectLoaded(Reward reward, object callbackData)
	{
		this.PositionReward(reward);
		reward.Show(false);
	}

	// Token: 0x0600314E RID: 12622 RVA: 0x000F8378 File Offset: 0x000F6578
	private List<AdventureMissionDisplay.WingCreateParams> BuildWingCreateParamsList()
	{
		AdventureConfig adventureConfig = AdventureConfig.Get();
		AdventureDbId selectedAdventure = adventureConfig.GetSelectedAdventure();
		AdventureModeDbId selectedMode = adventureConfig.GetSelectedMode();
		int adventureDbId = (int)selectedAdventure;
		int modeDbId = (int)selectedMode;
		List<AdventureMissionDisplay.WingCreateParams> list = new List<AdventureMissionDisplay.WingCreateParams>();
		int num = 0;
		List<ScenarioDbfRecord> records = GameDbf.Scenario.GetRecords((ScenarioDbfRecord r) => r.AdventureId == adventureDbId && r.ModeId == modeDbId);
		foreach (ScenarioDbfRecord scenarioDbfRecord in records)
		{
			int id = scenarioDbfRecord.ID;
			ScenarioDbId scenarioDbId;
			if (EnumUtils.TryCast<ScenarioDbId>(id, out scenarioDbId))
			{
				WingDbId wingId;
				if (EnumUtils.TryCast<WingDbId>(scenarioDbfRecord.WingId, out wingId))
				{
					int num2 = scenarioDbfRecord.ClientPlayer2HeroCardId;
					if (num2 == 0)
					{
						num2 = scenarioDbfRecord.Player2HeroCardId;
					}
					AdventureWingDef wingDef = AdventureScene.Get().GetWingDef(wingId);
					if (wingDef == null)
					{
						Debug.LogError(string.Format("Unable to find wing record for scenario {0} with ID: {1}", id, wingId));
					}
					else
					{
						CardDbfRecord record = GameDbf.Card.GetRecord(num2);
						AdventureMissionDisplay.WingCreateParams wingCreateParams = list.Find((AdventureMissionDisplay.WingCreateParams currParams) => wingId == currParams.m_WingDef.GetWingId());
						if (wingCreateParams == null)
						{
							wingCreateParams = new AdventureMissionDisplay.WingCreateParams();
							wingCreateParams.m_WingDef = wingDef;
							if (wingCreateParams.m_WingDef == null)
							{
								Error.AddDevFatal("AdventureDisplay.BuildWingCreateParamsMap() - failed to find a WingDef for adventure {0} wing {1}", new object[]
								{
									selectedAdventure,
									wingId
								});
								continue;
							}
							list.Add(wingCreateParams);
						}
						AdventureMissionDisplay.BossCreateParams bossCreateParams = new AdventureMissionDisplay.BossCreateParams();
						bossCreateParams.m_ScenarioRecord = scenarioDbfRecord;
						bossCreateParams.m_MissionId = scenarioDbId;
						bossCreateParams.m_CardDefId = record.NoteMiniGuid;
						if (!this.m_BossInfoCache.ContainsKey(scenarioDbId))
						{
							AdventureMissionDisplay.BossInfo value = new AdventureMissionDisplay.BossInfo
							{
								m_Title = scenarioDbfRecord.ShortName,
								m_Description = scenarioDbfRecord.Description
							};
							this.m_BossInfoCache[scenarioDbId] = value;
						}
						wingCreateParams.m_BossCreateParams.Add(bossCreateParams);
						num++;
					}
				}
			}
		}
		if (num == 0)
		{
			Debug.LogError(string.Format("Unable to find any bosses associated with wing {0} and mode {1}.\nCheck if the scenario DBF has valid entries!", selectedAdventure, selectedMode));
		}
		list.Sort(new Comparison<AdventureMissionDisplay.WingCreateParams>(this.WingCreateParamsSortComparison));
		foreach (AdventureMissionDisplay.WingCreateParams wingCreateParams2 in list)
		{
			wingCreateParams2.m_BossCreateParams.Sort(new Comparison<AdventureMissionDisplay.BossCreateParams>(this.BossCreateParamsSortComparison));
		}
		return list;
	}

	// Token: 0x0600314F RID: 12623 RVA: 0x000F8674 File Offset: 0x000F6874
	private int WingCreateParamsSortComparison(AdventureMissionDisplay.WingCreateParams params1, AdventureMissionDisplay.WingCreateParams params2)
	{
		return params1.m_WingDef.GetSortOrder() - params2.m_WingDef.GetSortOrder();
	}

	// Token: 0x06003150 RID: 12624 RVA: 0x000F868D File Offset: 0x000F688D
	private int BossCreateParamsSortComparison(AdventureMissionDisplay.BossCreateParams params1, AdventureMissionDisplay.BossCreateParams params2)
	{
		return GameUtils.MissionSortComparison(params1.m_ScenarioRecord, params2.m_ScenarioRecord);
	}

	// Token: 0x06003151 RID: 12625 RVA: 0x000F86A0 File Offset: 0x000F68A0
	private void ShowAdventureStore(AdventureWing selectedWing)
	{
		StoreManager.Get().StartAdventureTransaction(selectedWing.GetProductType(), selectedWing.GetProductData(), null, null);
	}

	// Token: 0x06003152 RID: 12626 RVA: 0x000F86C5 File Offset: 0x000F68C5
	private void OnStoreShown(object userData)
	{
		this.DisableSelection(true);
	}

	// Token: 0x06003153 RID: 12627 RVA: 0x000F86CE File Offset: 0x000F68CE
	private void OnStoreHidden(object userData)
	{
		this.DisableSelection(false);
	}

	// Token: 0x06003154 RID: 12628 RVA: 0x000F86D8 File Offset: 0x000F68D8
	private void OnAdventureProgressUpdate(bool isStartupAction, AdventureMission.WingProgress oldProgress, AdventureMission.WingProgress newProgress, object userData)
	{
		bool flag = oldProgress != null && oldProgress.IsOwned();
		bool flag2 = newProgress != null && newProgress.IsOwned();
		if (flag == flag2)
		{
			return;
		}
		base.StartCoroutine(this.UpdateWingPlateStates());
	}

	// Token: 0x06003155 RID: 12629 RVA: 0x000F871C File Offset: 0x000F691C
	private IEnumerator UpdateWingPlateStates()
	{
		while (StoreManager.Get().IsShown())
		{
			yield return null;
		}
		foreach (AdventureWing wing in this.m_BossWings)
		{
			wing.UpdatePlateState();
		}
		yield break;
	}

	// Token: 0x06003156 RID: 12630 RVA: 0x000F8738 File Offset: 0x000F6938
	private void ShowRewardsPreview(AdventureWing wing, int[] scenarioids, List<CardRewardData> wingRewards, string wingName)
	{
		if (this.m_ShowingRewardsPreview)
		{
			return;
		}
		if (this.m_ClickBlocker != null)
		{
			this.m_ClickBlocker.SetActive(true);
		}
		this.m_ShowingRewardsPreview = true;
		this.m_PreviewPane.Reset();
		this.m_PreviewPane.SetHeaderText(wingName);
		List<string> specificRewardsPreviewCards = wing.GetWingDef().m_SpecificRewardsPreviewCards;
		int hiddenRewardsPreviewCount = wing.GetWingDef().m_HiddenRewardsPreviewCount;
		if (specificRewardsPreviewCards != null && specificRewardsPreviewCards.Count > 0)
		{
			this.m_PreviewPane.AddSpecificCards(specificRewardsPreviewCards);
		}
		else
		{
			foreach (int scenarioId in scenarioids)
			{
				this.m_PreviewPane.AddCardBatch(scenarioId);
			}
			if (wingRewards != null && wingRewards.Count > 0)
			{
				this.m_PreviewPane.AddCardBatch(wingRewards);
			}
		}
		this.m_PreviewPane.SetHiddenCardCount(hiddenRewardsPreviewCount);
		this.m_PreviewPane.Show(true);
	}

	// Token: 0x06003157 RID: 12631 RVA: 0x000F882C File Offset: 0x000F6A2C
	private void OnHideRewardsPreview()
	{
		if (this.m_ClickBlocker != null)
		{
			this.m_ClickBlocker.SetActive(false);
		}
		this.m_ShowingRewardsPreview = false;
	}

	// Token: 0x06003158 RID: 12632 RVA: 0x000F885D File Offset: 0x000F6A5D
	private void OnStartUnlockPlate(AdventureWing wing)
	{
		this.UnselectBoss();
		this.DisableSelection(true);
	}

	// Token: 0x06003159 RID: 12633 RVA: 0x000F886C File Offset: 0x000F6A6C
	private void OnEndUnlockPlate(AdventureWing wing)
	{
		this.DisableSelection(false);
		if (!string.IsNullOrEmpty(wing.GetWingDef().m_WingOpenPopup))
		{
			AdventureWingOpenBanner adventureWingOpenBanner = GameUtils.LoadGameObjectWithComponent<AdventureWingOpenBanner>(wing.GetWingDef().m_WingOpenPopup);
			if (adventureWingOpenBanner != null)
			{
				adventureWingOpenBanner.ShowBanner(delegate
				{
					MonoBehaviour <>f__this = this;
					AdventureMissionDisplay <>f__this2 = this;
					List<AdventureWing> list2 = new List<AdventureWing>();
					list2.Add(wing);
					<>f__this.StartCoroutine(<>f__this2.UpdateAndAnimateWingCoinsAndChests(list2, false, true));
				});
			}
		}
		else
		{
			List<AdventureWing> list = new List<AdventureWing>();
			list.Add(wing);
			base.StartCoroutine(this.UpdateAndAnimateWingCoinsAndChests(list, false, true));
		}
	}

	// Token: 0x0600315A RID: 12634 RVA: 0x000F890C File Offset: 0x000F6B0C
	private void BringWingToFocus(float scrollPos)
	{
		if (this.m_ScrollBar == null)
		{
			return;
		}
		this.m_ScrollBar.SetScroll(scrollPos, false, true);
	}

	// Token: 0x0600315B RID: 12635 RVA: 0x000F893C File Offset: 0x000F6B3C
	private IEnumerator RememberLastBossSelection(AdventureBossCoin coin, ScenarioDbId mission)
	{
		while (this.m_AssetsLoading > 0)
		{
			yield return null;
		}
		this.OnBossSelected(coin, mission, false);
		yield break;
	}

	// Token: 0x0600315C RID: 12636 RVA: 0x000F8974 File Offset: 0x000F6B74
	private IEnumerator PlayWingNotifications()
	{
		yield return new WaitForSeconds(3f);
		foreach (AdventureWing wing in this.m_WingsToGiveBigChest)
		{
			if (wing.GetAdventureId() == AdventureDbId.NAXXRAMAS && wing.GetWingId() == WingDbId.NAXX_ARACHNID)
			{
				NotificationManager.Get().CreateKTQuote("VO_KT_MAEXXNA5_50", "VO_KT_MAEXXNA5_50", true);
			}
		}
		yield break;
	}

	// Token: 0x0600315D RID: 12637 RVA: 0x000F8990 File Offset: 0x000F6B90
	private void DoAutoPurchaseWings(AdventureDbId selectedAdv, AdventureModeDbId selectedMode)
	{
		if (selectedMode == AdventureModeDbId.NORMAL)
		{
			ProductType adventureProductType = StoreManager.GetAdventureProductType(selectedAdv);
			if (adventureProductType != null)
			{
				StoreManager.Get().DoZeroCostTransactionIfPossible(StoreType.ADVENTURE_STORE, new Store.ExitCallback(this.OnZeroCostTransactionStoreExit), null, adventureProductType, 0, 0);
			}
		}
	}

	// Token: 0x0600315E RID: 12638 RVA: 0x000F89CD File Offset: 0x000F6BCD
	private void ResumeMainMusic()
	{
		if (this.m_mainMusic != MusicPlaylistType.Invalid)
		{
			MusicManager.Get().StartPlaylist(this.m_mainMusic);
		}
	}

	// Token: 0x04001E8F RID: 7823
	private const float s_ScreenBackTransitionDelay = 1.8f;

	// Token: 0x04001E90 RID: 7824
	[CustomEditField(Sections = "Boss Layout Settings")]
	public GameObject m_BossWingContainer;

	// Token: 0x04001E91 RID: 7825
	[CustomEditField(Sections = "Boss Info")]
	public UberText m_BossTitle;

	// Token: 0x04001E92 RID: 7826
	[CustomEditField(Sections = "Boss Info")]
	public UberText m_BossDescription;

	// Token: 0x04001E93 RID: 7827
	[CustomEditField(Sections = "UI")]
	public UberText m_AdventureTitle;

	// Token: 0x04001E94 RID: 7828
	[CustomEditField(Sections = "UI")]
	public PegUIElement m_BackButton;

	// Token: 0x04001E95 RID: 7829
	[CustomEditField(Sections = "UI")]
	public PlayButton m_ChooseButton;

	// Token: 0x04001E96 RID: 7830
	[CustomEditField(Sections = "UI")]
	public GameObject m_BossPortraitContainer;

	// Token: 0x04001E97 RID: 7831
	[CustomEditField(Sections = "UI")]
	public GameObject m_BossPowerContainer;

	// Token: 0x04001E98 RID: 7832
	[CustomEditField(Sections = "UI")]
	public PegUIElement m_BossPowerHoverArea;

	// Token: 0x04001E99 RID: 7833
	[CustomEditField(Sections = "UI")]
	public float m_BossPowerCardScale = 1f;

	// Token: 0x04001E9A RID: 7834
	[CustomEditField(Sections = "UI", T = EditType.GAME_OBJECT)]
	public MeshRenderer m_WatermarkIcon;

	// Token: 0x04001E9B RID: 7835
	[CustomEditField(Sections = "UI")]
	public AdventureRewardsDisplayArea m_RewardsDisplay;

	// Token: 0x04001E9C RID: 7836
	[CustomEditField(Sections = "UI")]
	public GameObject m_ClickBlocker;

	// Token: 0x04001E9D RID: 7837
	[CustomEditField(Sections = "UI/Animation")]
	public float m_CoinFlipDelayTime = 1.25f;

	// Token: 0x04001E9E RID: 7838
	[CustomEditField(Sections = "UI/Animation")]
	public float m_CoinFlipAnimationTime = 1f;

	// Token: 0x04001E9F RID: 7839
	[CustomEditField(Sections = "UI/Scroll Bar")]
	public UIBScrollable m_ScrollBar;

	// Token: 0x04001EA0 RID: 7840
	[CustomEditField(Sections = "UI/Preview Pane")]
	public AdventureRewardsPreview m_PreviewPane;

	// Token: 0x04001EA1 RID: 7841
	[SerializeField]
	private float m_BossWingHeight = 15f;

	// Token: 0x04001EA2 RID: 7842
	[SerializeField]
	private Vector3 m_BossWingOffset = Vector3.zero;

	// Token: 0x04001EA3 RID: 7843
	private static AdventureMissionDisplay s_instance;

	// Token: 0x04001EA4 RID: 7844
	private AdventureWingProgressDisplay m_progressDisplay;

	// Token: 0x04001EA5 RID: 7845
	private List<AdventureWing> m_BossWings = new List<AdventureWing>();

	// Token: 0x04001EA6 RID: 7846
	private GameObject m_BossWingBorder;

	// Token: 0x04001EA7 RID: 7847
	private AdventureBossCoin m_SelectedCoin;

	// Token: 0x04001EA8 RID: 7848
	private Map<ScenarioDbId, FullDef> m_BossPortraitDefCache = new Map<ScenarioDbId, FullDef>();

	// Token: 0x04001EA9 RID: 7849
	private Map<ScenarioDbId, FullDef> m_BossPowerDefCache = new Map<ScenarioDbId, FullDef>();

	// Token: 0x04001EAA RID: 7850
	private int m_AssetsLoading;

	// Token: 0x04001EAB RID: 7851
	private Actor m_BossActor;

	// Token: 0x04001EAC RID: 7852
	private Actor m_BossPowerActor;

	// Token: 0x04001EAD RID: 7853
	private Actor m_BossPowerBigCard;

	// Token: 0x04001EAE RID: 7854
	private FullDef m_SelectedHeroPowerFullDef;

	// Token: 0x04001EAF RID: 7855
	private int m_DisableSelectionCount;

	// Token: 0x04001EB0 RID: 7856
	private List<AdventureWing> m_WingsToGiveBigChest = new List<AdventureWing>();

	// Token: 0x04001EB1 RID: 7857
	private bool m_ShowingRewardsPreview;

	// Token: 0x04001EB2 RID: 7858
	private int m_TotalBosses;

	// Token: 0x04001EB3 RID: 7859
	private int m_TotalBossesDefeated;

	// Token: 0x04001EB4 RID: 7860
	private bool m_BossJustDefeated;

	// Token: 0x04001EB5 RID: 7861
	private bool m_WaitingForClassChallengeUnlocks;

	// Token: 0x04001EB6 RID: 7862
	private int m_ClassChallengeUnlockShowing;

	// Token: 0x04001EB7 RID: 7863
	private static readonly Vector3 REWARD_SCALE = Vector3.one;

	// Token: 0x04001EB8 RID: 7864
	private static readonly Vector3 REWARD_PUNCH_SCALE = new Vector3(1.2f, 1.2f, 1.2f);

	// Token: 0x04001EB9 RID: 7865
	private Map<ScenarioDbId, AdventureMissionDisplay.BossInfo> m_BossInfoCache = new Map<ScenarioDbId, AdventureMissionDisplay.BossInfo>();

	// Token: 0x04001EBA RID: 7866
	private MusicPlaylistType m_mainMusic;

	// Token: 0x020003A6 RID: 934
	protected class BossCreateParams
	{
		// Token: 0x04001EBB RID: 7867
		public ScenarioDbfRecord m_ScenarioRecord;

		// Token: 0x04001EBC RID: 7868
		public ScenarioDbId m_MissionId;

		// Token: 0x04001EBD RID: 7869
		public string m_CardDefId;
	}

	// Token: 0x020003A7 RID: 935
	protected class WingCreateParams
	{
		// Token: 0x04001EBE RID: 7870
		public AdventureWingDef m_WingDef;

		// Token: 0x04001EBF RID: 7871
		[CustomEditField(ListTable = true)]
		public List<AdventureMissionDisplay.BossCreateParams> m_BossCreateParams = new List<AdventureMissionDisplay.BossCreateParams>();
	}

	// Token: 0x020003A8 RID: 936
	protected class BossInfo
	{
		// Token: 0x04001EC0 RID: 7872
		public string m_Title;

		// Token: 0x04001EC1 RID: 7873
		public string m_Description;
	}
}
