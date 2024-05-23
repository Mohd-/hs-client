using System;
using System.Collections;
using System.Collections.Generic;
using PegasusUtil;
using UnityEngine;

// Token: 0x020003AA RID: 938
[CustomEditClass]
public class AdventureWing : MonoBehaviour
{
	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x06003173 RID: 12659 RVA: 0x000F8DD7 File Offset: 0x000F6FD7
	// (set) Token: 0x06003174 RID: 12660 RVA: 0x000F8DDF File Offset: 0x000F6FDF
	public float CoinSpacing
	{
		get
		{
			return this.m_CoinSpacing;
		}
		set
		{
			this.m_CoinSpacing = value;
			this.UpdateCoinPositions();
		}
	}

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x06003175 RID: 12661 RVA: 0x000F8DEE File Offset: 0x000F6FEE
	// (set) Token: 0x06003176 RID: 12662 RVA: 0x000F8DF6 File Offset: 0x000F6FF6
	public Vector3 CoinsOffset
	{
		get
		{
			return this.m_CoinsOffset;
		}
		set
		{
			this.m_CoinsOffset = value;
			this.UpdateCoinPositions();
		}
	}

	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x06003177 RID: 12663 RVA: 0x000F8E05 File Offset: 0x000F7005
	// (set) Token: 0x06003178 RID: 12664 RVA: 0x000F8E0D File Offset: 0x000F700D
	public Vector3 CoinsChestOffset
	{
		get
		{
			return this.m_CoinsChestOffset;
		}
		set
		{
			this.m_CoinsChestOffset = value;
			this.UpdateCoinPositions();
		}
	}

	// Token: 0x06003179 RID: 12665 RVA: 0x000F8E1C File Offset: 0x000F701C
	private void Awake()
	{
		if (this.m_BigChest != null)
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				this.m_BigChest.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ShowBigChestRewards));
			}
			else
			{
				this.m_BigChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowBigChestRewards));
				this.m_BigChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HideBigChestRewards));
			}
		}
	}

	// Token: 0x0600317A RID: 12666 RVA: 0x000F8E99 File Offset: 0x000F7099
	private void OnDestroy()
	{
		if (StoreManager.Get() != null)
		{
			StoreManager.Get().RemoveStatusChangedListener(new StoreManager.StatusChangedCallback(this.UpdateBuyButton));
		}
	}

	// Token: 0x0600317B RID: 12667 RVA: 0x000F8EBC File Offset: 0x000F70BC
	private void Update()
	{
		if (!this.m_ContentsContainer.activeSelf && !this.m_EventStartDetected && !AdventureScene.Get().IsDevMode)
		{
			bool flag = AdventureProgressMgr.Get().OwnsWing((int)this.m_WingDef.GetWingId());
			bool flag2 = AdventureProgressMgr.Get().IsWingOpen((int)this.m_WingDef.GetWingId());
			if (flag2 && !flag)
			{
				this.UpdatePlateState();
			}
		}
		if (!AdventureScene.Get().IsDevMode)
		{
			return;
		}
		if (Input.GetKeyDown(49))
		{
			this.m_ContentsContainer.SetActive(false);
			this.m_LockPlate.SetActive(true);
			this.m_WingEventTable.PlateKey(true);
		}
		if (Input.GetKeyDown(50))
		{
			this.m_ContentsContainer.SetActive(false);
			this.m_LockPlate.SetActive(true);
			this.m_WingEventTable.PlateBuy(true);
		}
		if (Input.GetKeyDown(51))
		{
			this.m_ContentsContainer.SetActive(false);
			this.m_LockPlate.SetActive(true);
			this.m_WingEventTable.PlateInitialText();
		}
		if (Input.GetKeyDown(52))
		{
			this.m_ContentsContainer.SetActive(true);
			this.m_LockPlate.SetActive(false);
		}
		if (Input.GetKeyDown(53))
		{
			this.m_ContentsContainer.SetActive(false);
			this.m_LockPlate.SetActive(true);
			AdventureWingUnlockSpell component = this.m_UnlockSpell.GetComponent<AdventureWingUnlockSpell>();
			this.m_WingEventTable.PlateOpen((!(component != null)) ? 0f : component.m_UnlockDelay);
		}
		if (Input.GetKeyDown(54))
		{
			foreach (AdventureWing.Boss boss in this.m_BossCoins)
			{
				boss.m_Chest.SlamInCheckmark();
			}
		}
	}

	// Token: 0x0600317C RID: 12668 RVA: 0x000F90AC File Offset: 0x000F72AC
	public void Initialize(AdventureWingDef wingDef, AdventureWingDef dependsOnWingDef)
	{
		this.m_WingDef = wingDef;
		this.m_DependsOnWingDef = dependsOnWingDef;
		GameUtils.SetAutomationName(base.gameObject, new object[]
		{
			(int)wingDef.GetWingId()
		});
		foreach (UberText uberText in this.m_WingTitles)
		{
			if (uberText != null)
			{
				uberText.Text = this.m_WingDef.GetWingName();
			}
		}
		if (!string.IsNullOrEmpty(wingDef.m_UnlockSpellPrefab) && this.m_LockPlateFXContainer != null)
		{
			GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(wingDef.m_UnlockSpellPrefab), true, false);
			this.m_UnlockSpell = gameObject.GetComponent<Spell>();
			GameUtils.SetParent(this.m_UnlockSpell, this.m_LockPlateFXContainer, false);
			this.m_UnlockSpell.gameObject.SetActive(false);
		}
		this.SetAccent(wingDef.m_AccentPrefab);
		this.m_Owned = AdventureProgressMgr.Get().OwnsWing((int)wingDef.GetWingId());
		this.m_Playable = (this.m_Owned && AdventureProgressMgr.Get().IsWingOpen((int)wingDef.GetWingId()));
		this.UpdatePurchasedBanner();
		bool flag = AdventureConfig.Get().GetSelectedMode() == AdventureModeDbId.HEROIC;
		int num;
		AdventureProgressMgr.Get().GetWingAck((int)this.m_WingDef.GetWingId(), out num);
		bool flag2 = num > 0;
		if (AdventureScene.Get().IsDevMode)
		{
			int devModeSetting = AdventureScene.Get().DevModeSetting;
			this.m_WingEventTable.PlateActivate();
			if (devModeSetting == 1)
			{
				this.m_WingEventTable.PlateKey(true);
			}
			else if (devModeSetting == 2)
			{
				this.m_WingEventTable.PlateInitialText();
			}
			if (this.m_ReleaseLabelText != null)
			{
				this.m_ReleaseLabelText.Text = this.m_WingDef.GetComingSoonLabel();
			}
		}
		else if (this.m_Playable && (flag2 || flag))
		{
			this.m_WingEventTable.PlateDeactivate();
		}
		else
		{
			bool flag3 = AdventureProgressMgr.Get().IsWingLocked((int)wingDef.GetWingId());
			this.UpdateBuyButton(StoreManager.Get().IsOpen(), null);
			StoreManager.Get().RegisterStatusChangedListener(new StoreManager.StatusChangedCallback(this.UpdateBuyButton));
			this.m_WingEventTable.PlateActivate();
			if (!flag3)
			{
				bool flag4 = dependsOnWingDef == null || AdventureProgressMgr.Get().OwnsWing((int)dependsOnWingDef.GetWingId());
				this.m_EventStartDetected = AdventureProgressMgr.Get().IsWingOpen((int)this.m_WingDef.GetWingId());
				if (!this.m_EventStartDetected)
				{
					if (this.m_ReleaseLabelText != null)
					{
						this.m_ReleaseLabelText.Text = this.m_WingDef.GetComingSoonLabel();
					}
				}
				else if (!this.m_Owned && flag4)
				{
					this.m_WingEventTable.PlateBuy(true);
				}
				else if (this.m_Owned && num == 0)
				{
					this.m_WingEventTable.PlateKey(true);
				}
				else if (this.m_ReleaseLabelText != null)
				{
					this.m_ReleaseLabelText.Text = this.m_WingDef.GetRequiresLabel();
				}
			}
			else if (this.m_ReleaseLabelText != null)
			{
				this.m_ReleaseLabelText.Text = ((!this.m_Owned) ? GameStrings.Get(this.m_WingDef.m_LockedPurchaseLocString) : GameStrings.Get(this.m_WingDef.m_LockedLocString));
			}
		}
		this.m_UnlockButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.UnlockPlate));
		this.m_UnlockButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnUnlockButtonOut));
		this.m_UnlockButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnUnlockButtonOver));
		this.m_BuyButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			if (AdventureScene.Get().IsDevMode)
			{
				this.m_WingEventTable.PlateKey(false);
			}
			else
			{
				this.FireTryPurchaseWingEvent();
			}
		});
		if (this.m_RewardsPreviewButton != null)
		{
			this.m_RewardsPreviewButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.FireShowRewardsPreviewEvent();
			});
		}
	}

	// Token: 0x0600317D RID: 12669 RVA: 0x000F94DC File Offset: 0x000F76DC
	public AdventureWingDef GetWingDef()
	{
		return this.m_WingDef;
	}

	// Token: 0x0600317E RID: 12670 RVA: 0x000F94E4 File Offset: 0x000F76E4
	public WingDbId GetWingId()
	{
		return this.m_WingDef.GetWingId();
	}

	// Token: 0x0600317F RID: 12671 RVA: 0x000F94F4 File Offset: 0x000F76F4
	public List<AdventureRewardsChest> GetChests()
	{
		List<AdventureRewardsChest> list = new List<AdventureRewardsChest>();
		foreach (AdventureWing.Boss boss in this.m_BossCoins)
		{
			list.Add(boss.m_Chest);
		}
		return list;
	}

	// Token: 0x06003180 RID: 12672 RVA: 0x000F955C File Offset: 0x000F775C
	public AdventureDbId GetAdventureId()
	{
		return this.m_WingDef.GetAdventureId();
	}

	// Token: 0x06003181 RID: 12673 RVA: 0x000F9569 File Offset: 0x000F7769
	public ProductType GetProductType()
	{
		return StoreManager.GetAdventureProductType(this.GetAdventureId());
	}

	// Token: 0x06003182 RID: 12674 RVA: 0x000F9576 File Offset: 0x000F7776
	public int GetProductData()
	{
		return (int)this.GetWingId();
	}

	// Token: 0x06003183 RID: 12675 RVA: 0x000F957E File Offset: 0x000F777E
	public string GetWingName()
	{
		return this.m_WingDef.GetWingName();
	}

	// Token: 0x06003184 RID: 12676 RVA: 0x000F958B File Offset: 0x000F778B
	public void AddBossSelectedListener(AdventureWing.BossSelected dlg)
	{
		this.m_BossSelectedListeners.Add(dlg);
	}

	// Token: 0x06003185 RID: 12677 RVA: 0x000F9599 File Offset: 0x000F7799
	public void AddOpenPlateStartListener(AdventureWing.OpenPlateStart dlg)
	{
		this.m_OpenPlateStartListeners.Add(dlg);
	}

	// Token: 0x06003186 RID: 12678 RVA: 0x000F95A7 File Offset: 0x000F77A7
	public void AddOpenPlateEndListener(AdventureWing.OpenPlateEnd dlg)
	{
		this.m_OpenPlateEndListeners.Add(dlg);
	}

	// Token: 0x06003187 RID: 12679 RVA: 0x000F95B5 File Offset: 0x000F77B5
	public void AddShowCardRewardsListener(AdventureWing.ShowCardRewards dlg)
	{
		this.m_ShowCardRewardsListeners.Add(dlg);
	}

	// Token: 0x06003188 RID: 12680 RVA: 0x000F95C3 File Offset: 0x000F77C3
	public void AddHideCardRewardsListener(AdventureWing.HideCardRewards dlg)
	{
		this.m_HideCardRewardsListeners.Add(dlg);
	}

	// Token: 0x06003189 RID: 12681 RVA: 0x000F95D1 File Offset: 0x000F77D1
	public void AddShowRewardsPreviewListeners(AdventureWing.ShowRewardsPreview dlg)
	{
		this.m_ShowRewardsPreviewListeners.Add(dlg);
	}

	// Token: 0x0600318A RID: 12682 RVA: 0x000F95DF File Offset: 0x000F77DF
	public void AddTryPurchaseWingListener(AdventureWing.TryPurchaseWing dlg)
	{
		this.m_TryPurchaseWingListeners.Add(dlg);
	}

	// Token: 0x0600318B RID: 12683 RVA: 0x000F95F0 File Offset: 0x000F77F0
	public AdventureBossCoin CreateBoss(string coinPrefab, string rewardsPrefab, ScenarioDbId mission, bool enabled)
	{
		AdventureBossCoin newcoin = GameUtils.LoadGameObjectWithComponent<AdventureBossCoin>(coinPrefab);
		AdventureRewardsChest newchest = GameUtils.LoadGameObjectWithComponent<AdventureRewardsChest>(rewardsPrefab);
		GameUtils.SetAutomationName(newcoin.gameObject, new object[]
		{
			mission
		});
		if (newchest != null)
		{
			GameUtils.SetAutomationName(newchest.gameObject, new object[]
			{
				mission
			});
			this.UpdateBossChest(newchest, mission);
		}
		if (this.m_CoinContainer != null)
		{
			GameUtils.SetParent(newcoin, this.m_CoinContainer, false);
			if (newchest != null)
			{
				GameUtils.SetParent(newchest, this.m_CoinContainer, false);
				TransformUtil.SetLocalPosY(newchest.transform, 0.01f);
			}
		}
		newcoin.Enable(enabled, false);
		newcoin.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.FireBossSelectedEvent(newcoin, mission);
		});
		if (UniversalInputManager.UsePhoneUI)
		{
			newchest.Enable(false);
			if (newcoin.m_DisabledCollider != null)
			{
				newcoin.m_DisabledCollider.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
				{
					this.ShowBossRewards(mission, newcoin.transform.position);
				});
			}
		}
		else
		{
			newchest.AddChestEventListener(UIEventType.ROLLOVER, delegate(UIEvent e)
			{
				this.ShowBossRewards(mission, newchest.transform.position);
			});
			newchest.AddChestEventListener(UIEventType.ROLLOUT, delegate(UIEvent e)
			{
				this.HideBossRewards(mission);
			});
		}
		if (this.m_BossCoins.Count == 0)
		{
			newcoin.ShowConnector(false);
		}
		this.m_BossCoins.Add(new AdventureWing.Boss
		{
			m_Mission = mission,
			m_Coin = newcoin,
			m_Chest = newchest
		});
		this.UpdateCoinPositions();
		return newcoin;
	}

	// Token: 0x0600318C RID: 12684 RVA: 0x000F97F8 File Offset: 0x000F79F8
	public void SetAccent(string accentPrefab)
	{
		if (this.m_WallAccentObject != null)
		{
			Object.Destroy(this.m_WallAccentObject);
		}
		if (this.m_PlateAccentObject != null)
		{
			Object.Destroy(this.m_PlateAccentObject);
		}
		if (!string.IsNullOrEmpty(accentPrefab))
		{
			if (this.m_WallAccentContainer != null)
			{
				this.m_WallAccentObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(accentPrefab), true, false);
				GameUtils.SetParent(this.m_WallAccentObject, this.m_WallAccentContainer, false);
			}
			if (this.m_PlateAccentContainer != null)
			{
				this.m_PlateAccentObject = Object.Instantiate<GameObject>(this.m_WallAccentObject);
				GameUtils.SetParent(this.m_PlateAccentObject, this.m_PlateAccentContainer, false);
			}
		}
	}

	// Token: 0x0600318D RID: 12685 RVA: 0x000F98B7 File Offset: 0x000F7AB7
	public void SetBringToFocusCallback(AdventureWing.BringToFocusCallback dlg)
	{
		this.m_BringToFocusCallback = dlg;
	}

	// Token: 0x0600318E RID: 12686 RVA: 0x000F98C0 File Offset: 0x000F7AC0
	public void OpenBigChest()
	{
		this.m_WingEventTable.BigChestOpen();
		if (this.m_BigChest != null)
		{
			this.m_BigChest.RemoveEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ShowBigChestRewards));
			this.m_BigChest.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowBigChestRewards));
			this.m_BigChest.RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HideBigChestRewards));
		}
	}

	// Token: 0x0600318F RID: 12687 RVA: 0x000F9934 File Offset: 0x000F7B34
	public void HideBigChest()
	{
		this.m_WingEventTable.BigChestCover();
	}

	// Token: 0x06003190 RID: 12688 RVA: 0x000F9941 File Offset: 0x000F7B41
	public void BigChestStayOpen()
	{
		this.m_WingEventTable.BigChestStayOpen();
	}

	// Token: 0x06003191 RID: 12689 RVA: 0x000F9950 File Offset: 0x000F7B50
	public void SetBigChestRewards(WingDbId wingId)
	{
		if (AdventureConfig.Get().GetSelectedMode() != AdventureModeDbId.NORMAL)
		{
			return;
		}
		HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
		hashSet.Add(RewardVisualTiming.ADVENTURE_CHEST);
		HashSet<RewardVisualTiming> rewardTimings = hashSet;
		List<CardRewardData> cardRewardsForWing = AdventureProgressMgr.Get().GetCardRewardsForWing((int)wingId, rewardTimings);
		if (this.m_BigChest != null)
		{
			this.m_BigChest.SetData(cardRewardsForWing);
		}
	}

	// Token: 0x06003192 RID: 12690 RVA: 0x000F99A8 File Offset: 0x000F7BA8
	public List<CardRewardData> GetBigChestRewards()
	{
		return (!(this.m_BigChest != null)) ? null : ((List<CardRewardData>)this.m_BigChest.GetData());
	}

	// Token: 0x06003193 RID: 12691 RVA: 0x000F99D1 File Offset: 0x000F7BD1
	public bool HasBigChestRewards()
	{
		return this.m_BigChest != null && this.m_BigChest.GetData() != null;
	}

	// Token: 0x06003194 RID: 12692 RVA: 0x000F99F8 File Offset: 0x000F7BF8
	public bool UpdateAndAnimateCoinsAndChests(float startDelay, bool forceCoinAnimation, AdventureWing.DelOnCoinAnimateCallback dlg)
	{
		if (this.m_WingEventTable.IsPlateInOrGoingToAnActiveState())
		{
			return false;
		}
		List<AdventureWing.Boss> list = new List<AdventureWing.Boss>();
		AdventureConfig adventureConfig = AdventureConfig.Get();
		List<KeyValuePair<int, int>> list2 = new List<KeyValuePair<int, int>>();
		foreach (AdventureWing.Boss boss in this.m_BossCoins)
		{
			int num = 0;
			int num2 = 0;
			bool flag = adventureConfig.IsMissionNewlyAvailableAndGetReqs((int)boss.m_Mission, ref num, ref num2);
			if (forceCoinAnimation || flag)
			{
				if (!forceCoinAnimation || AdventureProgressMgr.Get().CanPlayScenario((int)boss.m_Mission))
				{
					list2.Add(new KeyValuePair<int, int>(num, num2));
					AdventureWing.Boss boss2 = new AdventureWing.Boss();
					boss2.m_Mission = boss.m_Mission;
					boss2.m_Coin = boss.m_Coin;
					if (AdventureProgressMgr.Get().GetImmediateCardRewardsForDefeatingScenario((int)boss.m_Mission).Count > 0)
					{
						boss2.m_Chest = boss.m_Chest;
					}
					list.Add(boss2);
				}
			}
		}
		foreach (KeyValuePair<int, int> keyValuePair in list2)
		{
			adventureConfig.SetWingAckIfGreater(keyValuePair.Key, keyValuePair.Value);
		}
		if (list.Count > 0)
		{
			base.StartCoroutine(this.AnimateCoinsAndChests(list, startDelay, dlg));
			return true;
		}
		return false;
	}

	// Token: 0x06003195 RID: 12693 RVA: 0x000F9B90 File Offset: 0x000F7D90
	public void UpdatePlateState()
	{
		this.UpdatePurchasedBanner();
		bool flag = AdventureProgressMgr.Get().IsWingLocked((int)this.m_WingDef.GetWingId());
		bool flag2 = AdventureProgressMgr.Get().OwnsWing((int)this.m_WingDef.GetWingId());
		bool flag3 = flag2 && AdventureProgressMgr.Get().IsWingOpen((int)this.m_WingDef.GetWingId());
		if (flag3 && this.m_Playable)
		{
			return;
		}
		if (flag3 != this.m_Playable && !flag)
		{
			this.m_WingEventTable.PlateKey(false);
		}
		else if (!this.m_WingEventTable.IsPlateBuy())
		{
			if (!flag)
			{
				bool flag4 = this.m_DependsOnWingDef == null || AdventureProgressMgr.Get().OwnsWing((int)this.m_DependsOnWingDef.GetWingId());
				this.m_EventStartDetected = AdventureProgressMgr.Get().IsWingOpen((int)this.m_WingDef.GetWingId());
				if (!this.m_EventStartDetected)
				{
					if (this.m_ReleaseLabelText != null)
					{
						this.m_ReleaseLabelText.Text = this.m_WingDef.GetComingSoonLabel();
					}
				}
				else if (!flag2 && flag4)
				{
					this.m_WingEventTable.PlateBuy(false);
				}
				else if (this.m_ReleaseLabelText != null)
				{
					this.m_ReleaseLabelText.Text = this.m_WingDef.GetRequiresLabel();
				}
			}
			else if (this.m_ReleaseLabelText != null)
			{
				this.m_ReleaseLabelText.Text = ((!flag2) ? GameStrings.Get(this.m_WingDef.m_LockedPurchaseLocString) : GameStrings.Get(this.m_WingDef.m_LockedLocString));
			}
		}
		this.m_Playable = flag3;
		this.m_Owned = flag2;
	}

	// Token: 0x06003196 RID: 12694 RVA: 0x000F9D53 File Offset: 0x000F7F53
	public void UpdateRewardsPreviewCover()
	{
		if (!this.HasRewards())
		{
			this.m_WingEventTable.PlateCoverPreviewChest();
		}
	}

	// Token: 0x06003197 RID: 12695 RVA: 0x000F9D6C File Offset: 0x000F7F6C
	public bool HasRewards()
	{
		List<CardRewardData> bigChestRewards = this.GetBigChestRewards();
		if (bigChestRewards != null && bigChestRewards.Count > 0)
		{
			return true;
		}
		foreach (AdventureWing.Boss boss in this.m_BossCoins)
		{
			if (AdventureProgressMgr.Get().GetImmediateCardRewardsForDefeatingScenario((int)boss.m_Mission).Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003198 RID: 12696 RVA: 0x000F9E00 File Offset: 0x000F8000
	public void RandomizeBackground()
	{
		if (this.m_BackgroundOffsets.Count == 0)
		{
			return;
		}
		int num;
		do
		{
			num = Random.Range(0, this.m_BackgroundOffsets.Count);
		}
		while (AdventureWing.s_LastRandomNumbers.Contains(num));
		AdventureWing.s_LastRandomNumbers.Add(num);
		if (AdventureWing.s_LastRandomNumbers.Count >= this.m_BackgroundOffsets.Count)
		{
			AdventureWing.s_LastRandomNumbers.RemoveAt(0);
		}
		foreach (AdventureWing.BackgroundRandomization backgroundRandomization in this.m_BackgroundRenderers)
		{
			if (!(backgroundRandomization.m_backgroundRenderer == null) && !string.IsNullOrEmpty(backgroundRandomization.m_materialTextureName))
			{
				Material material = backgroundRandomization.m_backgroundRenderer.materials[0];
				Vector2 textureOffset = material.GetTextureOffset(backgroundRandomization.m_materialTextureName);
				textureOffset.y = this.m_BackgroundOffsets[num];
				material.SetTextureOffset(backgroundRandomization.m_materialTextureName, textureOffset);
			}
		}
	}

	// Token: 0x06003199 RID: 12697 RVA: 0x000F9F1C File Offset: 0x000F811C
	private IEnumerator AnimateCoinsAndChests(List<AdventureWing.Boss> thingsToFlip, float delaySeconds, AdventureWing.DelOnCoinAnimateCallback dlg)
	{
		if (delaySeconds > 0f)
		{
			yield return new WaitForSeconds(delaySeconds);
		}
		if (dlg != null)
		{
			dlg(thingsToFlip[0].m_Coin.transform.position);
		}
		for (int i = 0; i < thingsToFlip.Count; i++)
		{
			AdventureWing.Boss boss = thingsToFlip[i];
			base.StartCoroutine(this.AnimateOneCoinAndChest(boss));
			yield return new WaitForSeconds(0.2f);
		}
		yield return new WaitForSeconds(0.5f);
		yield break;
	}

	// Token: 0x0600319A RID: 12698 RVA: 0x000F9F64 File Offset: 0x000F8164
	private IEnumerator AnimateOneCoinAndChest(AdventureWing.Boss boss)
	{
		if (boss.m_Chest != null && !AdventureProgressMgr.Get().HasDefeatedScenario((int)boss.m_Mission))
		{
			boss.m_Chest.BlinkChest();
		}
		yield return new WaitForSeconds(0.5f);
		boss.m_Coin.Enable(true, true);
		yield return new WaitForSeconds(1f);
		boss.m_Coin.ShowNewLookGlow();
		yield break;
	}

	// Token: 0x0600319B RID: 12699 RVA: 0x000F9F88 File Offset: 0x000F8188
	private void UpdateCoinPositions()
	{
		int num = 0;
		foreach (AdventureWing.Boss boss in this.m_BossCoins)
		{
			boss.m_Coin.transform.localPosition = this.m_CoinsOffset;
			TransformUtil.SetLocalPosX(boss.m_Coin, this.m_CoinsOffset.x + (float)num * this.m_CoinSpacing);
			if (boss.m_Chest != null)
			{
				boss.m_Chest.transform.localPosition = this.m_CoinsOffset;
				TransformUtil.SetLocalPosX(boss.m_Chest, this.m_CoinsOffset.x + (float)num * this.m_CoinSpacing);
				boss.m_Chest.transform.localPosition += this.m_CoinsChestOffset;
			}
			num++;
		}
	}

	// Token: 0x0600319C RID: 12700 RVA: 0x000FA080 File Offset: 0x000F8280
	private void FireBossSelectedEvent(AdventureBossCoin coin, ScenarioDbId mission)
	{
		AdventureWing.BossSelected[] array = this.m_BossSelectedListeners.ToArray();
		foreach (AdventureWing.BossSelected bossSelected in array)
		{
			bossSelected(coin, mission);
		}
	}

	// Token: 0x0600319D RID: 12701 RVA: 0x000FA0BC File Offset: 0x000F82BC
	private void FireOpenPlateStartEvent()
	{
		AdventureWing.OpenPlateStart[] array = this.m_OpenPlateStartListeners.ToArray();
		foreach (AdventureWing.OpenPlateStart openPlateStart in array)
		{
			openPlateStart(this);
		}
	}

	// Token: 0x0600319E RID: 12702 RVA: 0x000FA0F8 File Offset: 0x000F82F8
	private void FireOpenPlateEndEvent(Spell s)
	{
		if (this.m_UnlockSpell != null)
		{
			this.m_UnlockSpell.gameObject.SetActive(false);
		}
		AdventureWing.OpenPlateEnd[] array = this.m_OpenPlateEndListeners.ToArray();
		foreach (AdventureWing.OpenPlateEnd openPlateEnd in array)
		{
			openPlateEnd(this);
		}
	}

	// Token: 0x0600319F RID: 12703 RVA: 0x000FA154 File Offset: 0x000F8354
	private void OnUnlockButtonOut(UIEvent e)
	{
		if (this.m_UnlockButtonHighlightMesh_LOE == null)
		{
			return;
		}
		this.m_UnlockButtonHighlightMesh_LOE.material.SetFloat("_Intensity", this.m_UnlockButtonHighlightIntensityOut);
	}

	// Token: 0x060031A0 RID: 12704 RVA: 0x000FA183 File Offset: 0x000F8383
	private void OnUnlockButtonOver(UIEvent e)
	{
		if (this.m_UnlockButtonHighlightMesh_LOE == null)
		{
			return;
		}
		this.m_UnlockButtonHighlightMesh_LOE.material.SetFloat("_Intensity", this.m_UnlockButtonHighlightIntensityOver);
	}

	// Token: 0x060031A1 RID: 12705 RVA: 0x000FA1B4 File Offset: 0x000F83B4
	private void UnlockPlate(UIEvent e)
	{
		this.m_UnlockButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.UnlockPlate));
		float num = 0f;
		if (this.m_BringToFocusCallback != null)
		{
			num = 0.5f;
			this.m_BringToFocusCallback(this.m_UnlockButton.transform.position, num);
		}
		base.StartCoroutine(this.DoUnlockPlate(num));
	}

	// Token: 0x060031A2 RID: 12706 RVA: 0x000FA21C File Offset: 0x000F841C
	private IEnumerator DoUnlockPlate(float startDelay)
	{
		this.FireOpenPlateStartEvent();
		if (startDelay > 0f)
		{
			yield return new WaitForSeconds(startDelay);
		}
		this.m_WingEventTable.AddOpenPlateEndEventListener(new StateEventTable.StateEventTrigger(this.FireOpenPlateEndEvent), true);
		this.m_UnlockButton.GetComponent<Collider>().enabled = false;
		float unlockDelay = 0f;
		if (this.m_UnlockSpell != null)
		{
			AdventureWingUnlockSpell wingUnlockSpell = this.m_UnlockSpell.GetComponent<AdventureWingUnlockSpell>();
			unlockDelay = ((!(wingUnlockSpell != null)) ? 0f : wingUnlockSpell.m_UnlockDelay);
		}
		this.m_WingEventTable.PlateOpen(unlockDelay);
		this.m_ContentsContainer.SetActive(true);
		if (this.m_UnlockSpell != null)
		{
			this.m_UnlockSpell.gameObject.SetActive(true);
			this.m_UnlockSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnUnlockSpellFinished));
			this.m_UnlockSpell.Activate();
		}
		else
		{
			this.OnUnlockSpellFinished(null, null);
		}
		this.m_UnlockButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.UnlockPlate));
		yield break;
	}

	// Token: 0x060031A3 RID: 12707 RVA: 0x000FA248 File Offset: 0x000F8448
	private void OnUnlockSpellFinished(Spell spell, object userData)
	{
		Vector3 position = (!UniversalInputManager.UsePhoneUI) ? NotificationManager.ALT_ADVENTURE_SCREEN_POS : NotificationManager.PHONE_CHARACTER_POS;
		if (this.m_WingDef != null && !string.IsNullOrEmpty(this.m_WingDef.m_OpenQuoteVOLine))
		{
			string text = this.m_WingDef.m_OpenQuotePrefab;
			if (string.IsNullOrEmpty(text))
			{
				AdventureDef adventureDef = AdventureScene.Get().GetAdventureDef(this.GetAdventureId());
				text = adventureDef.m_DefaultQuotePrefab;
			}
			bool allowRepeatDuringSession = AdventureScene.Get() != null && AdventureScene.Get().IsDevMode;
			NotificationManager.Get().CreateCharacterQuote(FileUtils.GameAssetPathToName(text), position, GameStrings.Get(this.m_WingDef.m_OpenQuoteVOLine), this.m_WingDef.m_OpenQuoteVOLine, allowRepeatDuringSession, 0f, null, CanvasAnchor.BOTTOM_LEFT);
		}
	}

	// Token: 0x060031A4 RID: 12708 RVA: 0x000FA31C File Offset: 0x000F851C
	private void ShowBigChestRewards(UIEvent e)
	{
		List<CardRewardData> bigChestRewards = this.GetBigChestRewards();
		if (bigChestRewards == null)
		{
			return;
		}
		this.FireShowCardRewardsEvent(bigChestRewards, this.m_BigChest.transform.position);
	}

	// Token: 0x060031A5 RID: 12709 RVA: 0x000FA350 File Offset: 0x000F8550
	private void HideBigChestRewards(UIEvent e)
	{
		List<CardRewardData> bigChestRewards = this.GetBigChestRewards();
		if (bigChestRewards == null)
		{
			return;
		}
		this.FireHideCardRewardsEvent(bigChestRewards);
	}

	// Token: 0x060031A6 RID: 12710 RVA: 0x000FA374 File Offset: 0x000F8574
	private void ShowBossRewards(ScenarioDbId mission, Vector3 origin)
	{
		List<CardRewardData> immediateCardRewardsForDefeatingScenario = AdventureProgressMgr.Get().GetImmediateCardRewardsForDefeatingScenario((int)mission);
		this.FireShowCardRewardsEvent(immediateCardRewardsForDefeatingScenario, origin);
	}

	// Token: 0x060031A7 RID: 12711 RVA: 0x000FA398 File Offset: 0x000F8598
	private void HideBossRewards(ScenarioDbId mission)
	{
		List<CardRewardData> immediateCardRewardsForDefeatingScenario = AdventureProgressMgr.Get().GetImmediateCardRewardsForDefeatingScenario((int)mission);
		this.FireHideCardRewardsEvent(immediateCardRewardsForDefeatingScenario);
	}

	// Token: 0x060031A8 RID: 12712 RVA: 0x000FA3B8 File Offset: 0x000F85B8
	private void FireShowCardRewardsEvent(List<CardRewardData> rewards, Vector3 origin)
	{
		AdventureWing.ShowCardRewards[] array = this.m_ShowCardRewardsListeners.ToArray();
		foreach (AdventureWing.ShowCardRewards showCardRewards in array)
		{
			showCardRewards(rewards, origin);
		}
	}

	// Token: 0x060031A9 RID: 12713 RVA: 0x000FA3F4 File Offset: 0x000F85F4
	private void FireHideCardRewardsEvent(List<CardRewardData> rewards)
	{
		AdventureWing.HideCardRewards[] array = this.m_HideCardRewardsListeners.ToArray();
		foreach (AdventureWing.HideCardRewards hideCardRewards in array)
		{
			hideCardRewards(rewards);
		}
	}

	// Token: 0x060031AA RID: 12714 RVA: 0x000FA430 File Offset: 0x000F8630
	private void FireShowRewardsPreviewEvent()
	{
		AdventureWing.ShowRewardsPreview[] array = this.m_ShowRewardsPreviewListeners.ToArray();
		foreach (AdventureWing.ShowRewardsPreview showRewardsPreview in array)
		{
			showRewardsPreview();
		}
	}

	// Token: 0x060031AB RID: 12715 RVA: 0x000FA46C File Offset: 0x000F866C
	private void FireTryPurchaseWingEvent()
	{
		AdventureWing.TryPurchaseWing[] array = this.m_TryPurchaseWingListeners.ToArray();
		foreach (AdventureWing.TryPurchaseWing tryPurchaseWing in array)
		{
			tryPurchaseWing();
		}
	}

	// Token: 0x060031AC RID: 12716 RVA: 0x000FA4A8 File Offset: 0x000F86A8
	private void UpdateBossChest(AdventureRewardsChest chest, ScenarioDbId mission)
	{
		AdventureConfig adventureConfig = AdventureConfig.Get();
		if (adventureConfig.IsScenarioDefeatedAndInitCache(mission))
		{
			if (adventureConfig.IsScenarioJustDefeated(mission))
			{
				chest.SlamInCheckmark();
			}
			else
			{
				chest.ShowCheckmark();
			}
		}
		else if (AdventureProgressMgr.Get().GetImmediateCardRewardsForDefeatingScenario((int)mission).Count == 0)
		{
			chest.HideAll();
		}
	}

	// Token: 0x060031AD RID: 12717 RVA: 0x000FA504 File Offset: 0x000F8704
	private void UpdatePurchasedBanner()
	{
		if (this.m_PurchasedBanner != null)
		{
			bool flag = AdventureProgressMgr.Get().OwnsWing((int)this.m_WingDef.GetWingId());
			bool flag2 = AdventureProgressMgr.Get().IsWingOpen((int)this.m_WingDef.GetWingId());
			this.m_PurchasedBanner.SetActive(flag && !flag2);
		}
	}

	// Token: 0x060031AE RID: 12718 RVA: 0x000FA568 File Offset: 0x000F8768
	private void UpdateBuyButton(bool isOpen, object userData)
	{
		float num = 0f;
		bool enabled = true;
		string gameStringText = "GLUE_STORE_MONEY_BUTTON_TOOLTIP_HEADLINE";
		if (!isOpen)
		{
			num = 1f;
			enabled = false;
			gameStringText = "GLUE_ADVENTURE_LABEL_SHOP_CLOSED";
		}
		this.m_BuyButtonMesh.materials[0].SetFloat("_Desaturate", num);
		this.m_BuyButton.GetComponent<Collider>().enabled = enabled;
		this.m_BuyButtonText.SetGameStringText(gameStringText);
	}

	// Token: 0x04001ED2 RID: 7890
	[CustomEditField(Sections = "Wing Event Table")]
	public AdventureWingEventTable m_WingEventTable;

	// Token: 0x04001ED3 RID: 7891
	[CustomEditField(Sections = "Containers & Bones")]
	public GameObject m_ContentsContainer;

	// Token: 0x04001ED4 RID: 7892
	[CustomEditField(Sections = "Containers & Bones")]
	public GameObject m_CoinContainer;

	// Token: 0x04001ED5 RID: 7893
	[CustomEditField(Sections = "Containers & Bones")]
	public GameObject m_WallAccentContainer;

	// Token: 0x04001ED6 RID: 7894
	[CustomEditField(Sections = "Containers & Bones")]
	public GameObject m_PlateAccentContainer;

	// Token: 0x04001ED7 RID: 7895
	[CustomEditField(Sections = "Lock Plate")]
	public GameObject m_LockPlate;

	// Token: 0x04001ED8 RID: 7896
	[CustomEditField(Sections = "Lock Plate")]
	public GameObject m_LockPlateFXContainer;

	// Token: 0x04001ED9 RID: 7897
	[CustomEditField(Sections = "UI")]
	public List<UberText> m_WingTitles = new List<UberText>();

	// Token: 0x04001EDA RID: 7898
	[CustomEditField(Sections = "UI")]
	public PegUIElement m_UnlockButton;

	// Token: 0x04001EDB RID: 7899
	[CustomEditField(Sections = "UI")]
	public PegUIElement m_BuyButton;

	// Token: 0x04001EDC RID: 7900
	[CustomEditField(Sections = "UI")]
	public MeshRenderer m_BuyButtonMesh;

	// Token: 0x04001EDD RID: 7901
	[CustomEditField(Sections = "UI")]
	public UberText m_BuyButtonText;

	// Token: 0x04001EDE RID: 7902
	[CustomEditField(Sections = "UI")]
	public UberText m_ReleaseLabelText;

	// Token: 0x04001EDF RID: 7903
	[CustomEditField(Sections = "UI")]
	public PegUIElement m_RewardsPreviewButton;

	// Token: 0x04001EE0 RID: 7904
	[CustomEditField(Sections = "UI")]
	public GameObject m_PurchasedBanner;

	// Token: 0x04001EE1 RID: 7905
	[CustomEditField(Sections = "Wing Rewards")]
	public PegUIElement m_BigChest;

	// Token: 0x04001EE2 RID: 7906
	[CustomEditField(Sections = "Random Background Properties", ListTable = true)]
	public List<AdventureWing.BackgroundRandomization> m_BackgroundRenderers = new List<AdventureWing.BackgroundRandomization>();

	// Token: 0x04001EE3 RID: 7907
	[CustomEditField(Sections = "Random Background Properties")]
	public List<float> m_BackgroundOffsets = new List<float>();

	// Token: 0x04001EE4 RID: 7908
	[CustomEditField(Sections = "Special UI/LOE")]
	public MeshRenderer m_UnlockButtonHighlightMesh_LOE;

	// Token: 0x04001EE5 RID: 7909
	[CustomEditField(Sections = "Special UI/LOE")]
	public float m_UnlockButtonHighlightIntensityOut = 1.52f;

	// Token: 0x04001EE6 RID: 7910
	[CustomEditField(Sections = "Special UI/LOE")]
	public float m_UnlockButtonHighlightIntensityOver = 2f;

	// Token: 0x04001EE7 RID: 7911
	[SerializeField]
	private float m_CoinSpacing = 25f;

	// Token: 0x04001EE8 RID: 7912
	[SerializeField]
	private Vector3 m_CoinsOffset = Vector3.zero;

	// Token: 0x04001EE9 RID: 7913
	[SerializeField]
	private Vector3 m_CoinsChestOffset = Vector3.zero;

	// Token: 0x04001EEA RID: 7914
	private AdventureWingDef m_WingDef;

	// Token: 0x04001EEB RID: 7915
	private AdventureWingDef m_DependsOnWingDef;

	// Token: 0x04001EEC RID: 7916
	private Spell m_UnlockSpell;

	// Token: 0x04001EED RID: 7917
	private GameObject m_WallAccentObject;

	// Token: 0x04001EEE RID: 7918
	private GameObject m_PlateAccentObject;

	// Token: 0x04001EEF RID: 7919
	private List<AdventureWing.Boss> m_BossCoins = new List<AdventureWing.Boss>();

	// Token: 0x04001EF0 RID: 7920
	private AdventureWing.BringToFocusCallback m_BringToFocusCallback;

	// Token: 0x04001EF1 RID: 7921
	private bool m_Owned;

	// Token: 0x04001EF2 RID: 7922
	private bool m_Playable;

	// Token: 0x04001EF3 RID: 7923
	private bool m_EventStartDetected;

	// Token: 0x04001EF4 RID: 7924
	private List<AdventureWing.BossSelected> m_BossSelectedListeners = new List<AdventureWing.BossSelected>();

	// Token: 0x04001EF5 RID: 7925
	private List<AdventureWing.OpenPlateStart> m_OpenPlateStartListeners = new List<AdventureWing.OpenPlateStart>();

	// Token: 0x04001EF6 RID: 7926
	private List<AdventureWing.OpenPlateEnd> m_OpenPlateEndListeners = new List<AdventureWing.OpenPlateEnd>();

	// Token: 0x04001EF7 RID: 7927
	private List<AdventureWing.ShowCardRewards> m_ShowCardRewardsListeners = new List<AdventureWing.ShowCardRewards>();

	// Token: 0x04001EF8 RID: 7928
	private List<AdventureWing.HideCardRewards> m_HideCardRewardsListeners = new List<AdventureWing.HideCardRewards>();

	// Token: 0x04001EF9 RID: 7929
	private List<AdventureWing.ShowRewardsPreview> m_ShowRewardsPreviewListeners = new List<AdventureWing.ShowRewardsPreview>();

	// Token: 0x04001EFA RID: 7930
	private List<AdventureWing.TryPurchaseWing> m_TryPurchaseWingListeners = new List<AdventureWing.TryPurchaseWing>();

	// Token: 0x04001EFB RID: 7931
	private static List<int> s_LastRandomNumbers = new List<int>();

	// Token: 0x020003AB RID: 939
	// (Invoke) Token: 0x060031B2 RID: 12722
	public delegate void DelOnCoinAnimateCallback(Vector3 coinPosition);

	// Token: 0x020003BD RID: 957
	// (Invoke) Token: 0x0600323C RID: 12860
	public delegate void BossSelected(AdventureBossCoin coin, ScenarioDbId mission);

	// Token: 0x020003BE RID: 958
	// (Invoke) Token: 0x06003240 RID: 12864
	public delegate void OpenPlateStart(AdventureWing wing);

	// Token: 0x020003BF RID: 959
	// (Invoke) Token: 0x06003244 RID: 12868
	public delegate void OpenPlateEnd(AdventureWing wing);

	// Token: 0x020003C0 RID: 960
	// (Invoke) Token: 0x06003248 RID: 12872
	public delegate void TryPurchaseWing();

	// Token: 0x020003C1 RID: 961
	// (Invoke) Token: 0x0600324C RID: 12876
	public delegate void ShowCardRewards(List<CardRewardData> rewards, Vector3 origin);

	// Token: 0x020003C2 RID: 962
	// (Invoke) Token: 0x06003250 RID: 12880
	public delegate void HideCardRewards(List<CardRewardData> rewards);

	// Token: 0x020003C3 RID: 963
	// (Invoke) Token: 0x06003254 RID: 12884
	public delegate void ShowRewardsPreview();

	// Token: 0x020003C4 RID: 964
	// (Invoke) Token: 0x06003258 RID: 12888
	public delegate void BringToFocusCallback(Vector3 position, float time);

	// Token: 0x020003CE RID: 974
	[Serializable]
	public class BackgroundRandomization
	{
		// Token: 0x04001F9C RID: 8092
		public MeshRenderer m_backgroundRenderer;

		// Token: 0x04001F9D RID: 8093
		public string m_materialTextureName = "_MainTex";
	}

	// Token: 0x020003CF RID: 975
	protected class Boss
	{
		// Token: 0x04001F9E RID: 8094
		public ScenarioDbId m_Mission;

		// Token: 0x04001F9F RID: 8095
		public AdventureBossCoin m_Coin;

		// Token: 0x04001FA0 RID: 8096
		public AdventureRewardsChest m_Chest;
	}
}
