using System;
using System.Collections;
using PegasusShared;
using UnityEngine;

// Token: 0x0200039F RID: 927
[CustomEditClass]
public class TavernBrawlDisplay : MonoBehaviour
{
	// Token: 0x060030A1 RID: 12449 RVA: 0x000F45E0 File Offset: 0x000F27E0
	private void Awake()
	{
		TavernBrawlDisplay.s_instance = this;
		base.gameObject.transform.position = Vector3.zero;
		base.gameObject.transform.localScale = Vector3.one;
		TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
		RewardType rewardType = tavernBrawlMission.rewardType;
		RewardTrigger rewardTrigger;
		if (rewardType != 5)
		{
			if (rewardType == 6)
			{
				rewardTrigger = tavernBrawlMission.rewardTrigger;
				if (rewardTrigger != 2)
				{
					if (rewardTrigger == 3)
					{
						this.m_rewardsText.Text = GameStrings.Get("GLUE_TAVERN_BRAWL_REWARD_DESC_FINISH_CARDBACK");
						goto IL_A3;
					}
				}
				this.m_rewardsText.Text = GameStrings.Get("GLUE_TAVERN_BRAWL_REWARD_DESC_CARDBACK");
				IL_A3:
				goto IL_FB;
			}
		}
		rewardTrigger = tavernBrawlMission.rewardTrigger;
		if (rewardTrigger != 2)
		{
			if (rewardTrigger == 3)
			{
				this.m_rewardsText.Text = GameStrings.Get("GLUE_TAVERN_BRAWL_REWARD_DESC_FINISH");
				goto IL_F6;
			}
		}
		this.m_rewardsText.Text = GameStrings.Get("GLUE_TAVERN_BRAWL_REWARD_DESC");
		IL_F6:
		IL_FB:
		this.m_rewardsScale = this.m_rewardsPreview.transform.localScale;
		this.m_rewardsPreview.transform.localScale = Vector3.one * 0.01f;
		if (this.m_editDeckButton != null)
		{
			this.m_editDeckButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.EditOrDeleteDeck));
		}
		if (this.m_createDeckButton != null)
		{
			this.m_createDeckButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.CreateDeck();
			});
		}
		if (this.m_rewardOffClickCatcher != null)
		{
			this.m_rewardChest.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ShowReward));
			this.m_rewardOffClickCatcher.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.HideReward));
		}
		else
		{
			this.m_rewardChest.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.ShowReward));
			this.m_rewardChest.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.HideReward));
		}
		this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.StartGame));
		CollectionManager.Get().RegisterDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreated));
		CollectionManager.Get().RegisterDeckDeletedListener(new CollectionManager.DelOnDeckDeleted(this.OnDeckDeleted));
		CollectionManager.Get().RegisterDeckContentsListener(new CollectionManager.DelOnDeckContents(this.OnDeckContents));
		FriendChallengeMgr.Get().AddChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
		NetCache.Get().RegisterUpdatedListener(typeof(NetCache.NetCacheTavernBrawlRecord), new Action(this.NetCache_OnTavernBrawlRecord));
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		this.SetUIForFriendlyChallenge(FriendChallengeMgr.Get().IsChallengeTavernBrawl());
		if (this.m_backButton != null)
		{
			this.m_backButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.OnBackButton();
			});
		}
		if (tavernBrawlMission == null || !tavernBrawlMission.canEditDeck)
		{
			Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
		}
	}

	// Token: 0x060030A2 RID: 12450 RVA: 0x000F48EF File Offset: 0x000F2AEF
	private void OnBackButton()
	{
		Navigation.GoBack();
	}

	// Token: 0x060030A3 RID: 12451 RVA: 0x000F48F8 File Offset: 0x000F2AF8
	private void Start()
	{
		this.m_tavernBrawlTray.ToggleTraySlider(true, null, false);
		Enum[] status = PresenceMgr.Get().GetStatus();
		if (status == null || status.Length <= 0 || (PresenceStatus)status[0] != PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING)
		{
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.TAVERN_BRAWL_SCREEN
			});
		}
		this.RefreshStateBasedUI(false);
		this.RefreshDataBasedUI(this.m_wipeAnimStartDelay);
		MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_TavernBrawl);
		if (TavernBrawlManager.Get().CurrentMission() != null)
		{
			bool flag = UserAttentionManager.CanShowAttentionGrabber("TavernBrawlDisplay.Show");
			int @int = Options.Get().GetInt(Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD);
			if (@int == 0)
			{
				this.m_doWipeAnimation = true;
				if (flag && !NotificationManager.Get().HasSoundPlayedThisSession("VO_INNKEEPER_TAVERNBRAWL_WELCOME1_27"))
				{
					NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_TAVERNBRAWL_WELCOME1_27"), "VO_INNKEEPER_TAVERNBRAWL_WELCOME1_27", 0f, null);
					NotificationManager.Get().ForceAddSoundToPlayedList("VO_INNKEEPER_TAVERNBRAWL_WELCOME1_27");
				}
			}
			else if (@int < TavernBrawlManager.Get().CurrentMission().seasonId)
			{
				this.m_doWipeAnimation = true;
				int num = Options.Get().GetInt(Option.TIMES_SEEN_TAVERNBRAWL_CRAZY_RULES_QUOTE);
				if (flag && !NotificationManager.Get().HasSoundPlayedThisSession("VO_INNKEEPER_TAVERNBRAWL_DESC2_30") && num < 3)
				{
					NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_TAVERNBRAWL_DESC2_30"), "VO_INNKEEPER_TAVERNBRAWL_DESC2_30", 0f, null);
					NotificationManager.Get().ForceAddSoundToPlayedList("VO_INNKEEPER_TAVERNBRAWL_DESC2_30");
					num++;
					Options.Get().SetInt(Option.TIMES_SEEN_TAVERNBRAWL_CRAZY_RULES_QUOTE, num);
				}
			}
			if (flag && @int != TavernBrawlManager.Get().CurrentMission().seasonId)
			{
				Options.Get().SetInt(Option.LATEST_SEEN_TAVERNBRAWL_SEASON_CHALKBOARD, TavernBrawlManager.Get().CurrentMission().seasonId);
			}
		}
		if (TavernBrawlManager.Get().RewardProgress() == 0)
		{
			this.m_rewardHighlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
		base.StartCoroutine(this.UpdateQuestsWhenReady());
	}

	// Token: 0x060030A4 RID: 12452 RVA: 0x000F4AE5 File Offset: 0x000F2CE5
	private void OnDestroy()
	{
		TavernBrawlDisplay.s_instance = null;
	}

	// Token: 0x060030A5 RID: 12453 RVA: 0x000F4AED File Offset: 0x000F2CED
	public static TavernBrawlDisplay Get()
	{
		return TavernBrawlDisplay.s_instance;
	}

	// Token: 0x060030A6 RID: 12454 RVA: 0x000F4AF4 File Offset: 0x000F2CF4
	public void Unload()
	{
		CollectionManager.Get().RemoveDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreated));
		CollectionManager.Get().RemoveDeckDeletedListener(new CollectionManager.DelOnDeckDeleted(this.OnDeckDeleted));
		CollectionManager.Get().RemoveDeckContentsListener(new CollectionManager.DelOnDeckContents(this.OnDeckContents));
		FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
		NetCache.Get().RemoveUpdatedListener(typeof(NetCache.NetCacheTavernBrawlRecord), new Action(this.NetCache_OnTavernBrawlRecord));
		GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		if (FriendChallengeMgr.Get().IsChallengeTavernBrawl() && !SceneMgr.Get().IsInGame() && !SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FRIENDLY))
		{
			FriendChallengeMgr.Get().CancelChallenge();
		}
		if (this.IsInDeckEditMode())
		{
			Navigation.Pop();
		}
	}

	// Token: 0x060030A7 RID: 12455 RVA: 0x000F4BDC File Offset: 0x000F2DDC
	public void RefreshDataBasedUI(float animDelay = 0f)
	{
		this.RefreshTavernBrawlInfo(animDelay);
		this.NetCache_OnTavernBrawlRecord();
	}

	// Token: 0x060030A8 RID: 12456 RVA: 0x000F4BEB File Offset: 0x000F2DEB
	public bool IsInDeckEditMode()
	{
		return this.m_deckBeingEdited > 0L;
	}

	// Token: 0x060030A9 RID: 12457 RVA: 0x000F4BF8 File Offset: 0x000F2DF8
	public bool BackFromDeckEdit(bool animate)
	{
		if (!this.IsInDeckEditMode())
		{
			return false;
		}
		if (animate)
		{
			PresenceMgr.Get().SetPrevStatus();
		}
		if (CollectionManagerDisplay.Get().GetViewMode() != CollectionManagerDisplay.ViewMode.CARDS)
		{
			if (TavernBrawlManager.Get().CurrentDeck() == null)
			{
				CollectionManagerDisplay.Get().SetViewMode(CollectionManagerDisplay.ViewMode.CARDS, null);
			}
			else
			{
				CollectionManagerDisplay.Get().m_pageManager.JumpToCollectionClassPage(TavernBrawlManager.Get().CurrentDeck().GetClass());
			}
		}
		this.m_tavernBrawlTray.ToggleTraySlider(true, null, animate);
		this.RefreshStateBasedUI(animate);
		this.m_deckBeingEdited = 0L;
		BnetBar.Get().m_currencyFrame.RefreshContents();
		FriendChallengeMgr.Get().UpdateMyAvailability();
		this.UpdateEditOrCreate();
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_editDeckButton.SetText(GameStrings.Get("GLUE_EDIT"));
			if (this.m_editIcon != null)
			{
				this.m_editIcon.SetActive(true);
			}
			if (this.m_deleteIcon != null)
			{
				this.m_deleteIcon.SetActive(false);
			}
		}
		CollectionDeckTray.Get().ExitEditDeckModeForTavernBrawl();
		return true;
	}

	// Token: 0x060030AA RID: 12458 RVA: 0x000F4D18 File Offset: 0x000F2F18
	public static bool IsTavernBrawlOpen()
	{
		return SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL && !(TavernBrawlDisplay.s_instance == null);
	}

	// Token: 0x060030AB RID: 12459 RVA: 0x000F4D4B File Offset: 0x000F2F4B
	public static bool IsTavernBrawlEditing()
	{
		return TavernBrawlDisplay.IsTavernBrawlOpen() && TavernBrawlDisplay.s_instance.IsInDeckEditMode();
	}

	// Token: 0x060030AC RID: 12460 RVA: 0x000F4D64 File Offset: 0x000F2F64
	public static bool IsTavernBrawlViewing()
	{
		return TavernBrawlDisplay.IsTavernBrawlOpen() && !TavernBrawlDisplay.s_instance.IsInDeckEditMode();
	}

	// Token: 0x060030AD RID: 12461 RVA: 0x000F4D80 File Offset: 0x000F2F80
	public void ValidateDeck()
	{
		TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
		if (tavernBrawlMission == null)
		{
			this.m_playButton.Disable();
		}
		else if (tavernBrawlMission.canCreateDeck)
		{
			if (TavernBrawlManager.Get().HasValidDeck())
			{
				this.m_playButton.Enable();
				this.m_editDeckHighlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
			}
			else
			{
				this.m_playButton.Disable();
				this.m_editDeckHighlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
		}
	}

	// Token: 0x060030AE RID: 12462 RVA: 0x000F4E00 File Offset: 0x000F3000
	public void EnablePlayButton()
	{
		TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
		if (tavernBrawlMission == null || tavernBrawlMission.canCreateDeck)
		{
			this.ValidateDeck();
		}
		else
		{
			this.m_playButton.Enable();
		}
	}

	// Token: 0x060030AF RID: 12463 RVA: 0x000F4E40 File Offset: 0x000F3040
	private void RefreshTavernBrawlInfo(float animDelay)
	{
		this.UpdateEditOrCreate();
		TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
		if (tavernBrawlMission == null || tavernBrawlMission.missionId < 0)
		{
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLUE_TAVERN_BRAWL_HAS_ENDED_HEADER");
			popupInfo.m_text = GameStrings.Get("GLUE_TAVERN_BRAWL_HAS_ENDED_TEXT");
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.RefreshTavernBrawlInfo_ConfirmEnded);
			popupInfo.m_offset = new Vector3(0f, 104f, 0f);
			DialogManager.Get().ShowPopup(popupInfo);
			return;
		}
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(tavernBrawlMission.missionId);
		this.m_chalkboardHeader.Text = record.Name;
		this.m_chalkboardInfo.Text = record.Description;
		base.CancelInvoke("UpdateTimeText");
		base.InvokeRepeating("UpdateTimeText", 0.1f, 0.1f);
		this.UpdateTimeText();
		if (this.m_chalkboard != null && this.m_chalkboard.GetComponent<MeshRenderer>() != null && this.m_chalkboard.GetComponent<MeshRenderer>().material != null)
		{
			Material material = this.m_chalkboard.GetComponent<MeshRenderer>().material;
			string text = record.TbTexture;
			Vector2 vector = Vector2.zero;
			if (PlatformSettings.Screen == ScreenCategory.Phone)
			{
				text = record.TbTexturePhone;
				vector.y = record.TbTexturePhoneOffsetY;
			}
			Texture texture = (!string.IsNullOrEmpty(text)) ? AssetLoader.Get().LoadTexture(FileUtils.GameAssetPathToName(text), false) : null;
			if (texture == null)
			{
				bool canCreateDeck = TavernBrawlManager.Get().CurrentMission().canCreateDeck;
				text = ((!canCreateDeck) ? TavernBrawlDisplay.DEFAULT_CHALKBOARD_TEXTURE_NAME_NO_DECK : TavernBrawlDisplay.DEFAULT_CHALKBOARD_TEXTURE_NAME_WITH_DECK);
				vector = ((!canCreateDeck) ? TavernBrawlDisplay.DEFAULT_CHALKBOARD_TEXTURE_OFFSET_NO_DECK : TavernBrawlDisplay.DEFAULT_CHALKBOARD_TEXTURE_OFFSET_WITH_DECK);
				texture = AssetLoader.Get().LoadTexture(text, false);
			}
			if (texture != null)
			{
				material.SetTexture("_TopTex", texture);
				material.SetTextureOffset("_MainTex", vector);
			}
			base.StartCoroutine(this.WaitThenPlayWipeAnim((!this.m_doWipeAnimation) ? 0f : animDelay));
		}
	}

	// Token: 0x060030B0 RID: 12464 RVA: 0x000F5091 File Offset: 0x000F3291
	private void RefreshTavernBrawlInfo_ConfirmEnded(AlertPopup.Response response, object userData)
	{
		if (TavernBrawlDisplay.s_instance == null)
		{
			return;
		}
		Navigation.Clear();
		this.OnNavigateBack();
	}

	// Token: 0x060030B1 RID: 12465 RVA: 0x000F50B0 File Offset: 0x000F32B0
	private void SetUIForFriendlyChallenge(bool isTavernBrawlChallenge)
	{
		string key = "GLUE_BRAWL";
		if (TavernBrawlManager.Get().SelectHeroBeforeMission())
		{
			key = "GLUE_CHOOSE";
		}
		else if (isTavernBrawlChallenge)
		{
			key = "GLUE_BRAWL_FRIEND";
		}
		this.m_playButton.SetText(GameStrings.Get(key));
		this.m_rewardChest.gameObject.SetActive(!isTavernBrawlChallenge);
		this.m_winsBanner.SetActive(!isTavernBrawlChallenge);
		if (this.m_editDeckButton != null)
		{
			if (this.m_originalEditTextColor == null)
			{
				this.m_originalEditTextColor = new Color?(this.m_editText.TextColor);
			}
			if (isTavernBrawlChallenge)
			{
				this.m_editText.TextColor = this.m_disabledTextColor;
				this.m_editDeckButton.SetEnabled(false);
			}
			else
			{
				this.m_editText.TextColor = this.m_originalEditTextColor.Value;
				this.m_editDeckButton.SetEnabled(true);
			}
			if (this.m_editIcon != null)
			{
				if (this.m_originalEditIconColor == null)
				{
					this.m_originalEditIconColor = new Color?(this.m_editIcon.GetComponent<Renderer>().material.color);
				}
				if (isTavernBrawlChallenge)
				{
					this.m_editIcon.GetComponent<Renderer>().material.color = this.m_disabledTextColor;
				}
				else
				{
					this.m_editIcon.GetComponent<Renderer>().material.color = this.m_originalEditIconColor.Value;
				}
			}
		}
	}

	// Token: 0x060030B2 RID: 12466 RVA: 0x000F5228 File Offset: 0x000F3428
	private void UpdateTimeText()
	{
		int num = (TavernBrawlManager.Get().CurrentMission() != null) ? TavernBrawlManager.Get().CurrentTavernBrawlSeasonEnd : -1;
		if (num < 0)
		{
			base.CancelInvoke("UpdateTimeText");
			return;
		}
		TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet
		{
			m_seconds = "GLUE_TAVERN_BRAWL_LABEL_ENDING_SECONDS",
			m_minutes = "GLUE_TAVERN_BRAWL_LABEL_ENDING_MINUTES",
			m_hours = "GLUE_TAVERN_BRAWL_LABEL_ENDING_HOURS",
			m_days = "GLUE_TAVERN_BRAWL_LABEL_ENDING_DAYS",
			m_weeks = "GLUE_TAVERN_BRAWL_LABEL_ENDING_WEEKS",
			m_monthAgo = "GLUE_TAVERN_BRAWL_LABEL_ENDING_OVER_1_MONTH"
		};
		string elapsedTimeString = TimeUtils.GetElapsedTimeString(num, stringSet);
		this.m_chalkboardEndInfo.Text = elapsedTimeString;
	}

	// Token: 0x060030B3 RID: 12467 RVA: 0x000F52D0 File Offset: 0x000F34D0
	private void NetCache_OnTavernBrawlRecord()
	{
		this.m_numWins.Text = TavernBrawlManager.Get().GamesWon().ToString();
		if (TavernBrawlManager.Get().RewardProgress() > 0)
		{
			this.m_rewardChest.GetComponent<Renderer>().material = this.m_chestOpenMaterial;
			this.m_rewardHighlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
			this.m_rewardChest.SetEnabled(false);
		}
	}

	// Token: 0x060030B4 RID: 12468 RVA: 0x000F533A File Offset: 0x000F353A
	private bool OnNavigateBack()
	{
		this.m_tavernBrawlTray.m_animateBounce = false;
		this.m_tavernBrawlTray.ShowTray();
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
		return true;
	}

	// Token: 0x060030B5 RID: 12469 RVA: 0x000F535F File Offset: 0x000F355F
	private void RefreshStateBasedUI(bool animate)
	{
		this.UpdateDeckPanels(animate);
		this.ValidateDeck();
	}

	// Token: 0x060030B6 RID: 12470 RVA: 0x000F5370 File Offset: 0x000F3570
	private IEnumerator UpdateQuestsWhenReady()
	{
		while (!AchieveManager.Get().IsReady())
		{
			yield return null;
		}
		if (AchieveManager.Get().HasActiveQuests(true))
		{
			WelcomeQuests.Show(UserAttentionBlocker.NONE, false, null, false);
		}
		else
		{
			GameToastMgr.Get().UpdateQuestProgressToasts();
		}
		yield break;
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x000F5384 File Offset: 0x000F3584
	private void UpdateEditOrCreate()
	{
		TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
		bool flag = tavernBrawlMission != null && tavernBrawlMission.canCreateDeck;
		bool flag2 = tavernBrawlMission != null && tavernBrawlMission.canEditDeck;
		bool flag3 = TavernBrawlManager.Get().HasCreatedDeck();
		bool flag4 = flag && !flag3;
		bool active = flag2 && flag && flag3;
		if (this.m_editDeckButton != null)
		{
			this.m_editDeckButton.gameObject.SetActive(active);
			if (this.m_editIcon != null)
			{
				this.m_editIcon.SetActive(true);
			}
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			if (this.m_createDeckButton != null)
			{
				this.m_createDeckButton.gameObject.SetActive(flag4);
			}
		}
		else
		{
			if (this.m_panelWithCreateDeck != null)
			{
				this.m_panelWithCreateDeck.SetActive(flag4);
			}
			if (this.m_fullPanel != null)
			{
				this.m_fullPanel.SetActive(!flag4);
			}
		}
		if (this.m_createDeckHighlight != null)
		{
			if (!this.m_createDeckHighlight.gameObject.activeInHierarchy && flag4)
			{
				Debug.LogWarning("Attempting to activate m_createDeckHighlight, but it is inactive! This will not behave correctly!");
			}
			this.m_createDeckHighlight.ChangeState((!flag4) ? ActorStateType.HIGHLIGHT_OFF : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
	}

	// Token: 0x060030B8 RID: 12472 RVA: 0x000F54F4 File Offset: 0x000F36F4
	private void UpdateDeckPanels(bool animate = true)
	{
		TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
		this.UpdateDeckPanels(tavernBrawlMission != null && tavernBrawlMission.canCreateDeck && TavernBrawlManager.Get().HasCreatedDeck(), animate);
	}

	// Token: 0x060030B9 RID: 12473 RVA: 0x000F5534 File Offset: 0x000F3734
	private void UpdateDeckPanels(bool hasDeck, bool animate)
	{
		if (this.m_cardListPanel != null)
		{
			bool flag = !hasDeck;
			if (animate && !flag)
			{
				this.m_createDeckButton.gameObject.SetActive(false);
				this.m_createDeckHighlight.gameObject.SetActive(false);
			}
			else if (flag)
			{
				this.m_createDeckButton.gameObject.SetActive(true);
				this.m_createDeckHighlight.gameObject.SetActive(true);
			}
			this.m_cardListPanel.ToggleTraySlider(flag, null, animate);
		}
		if (this.m_cardCountPanelAnim != null && this.m_cardCountPanelAnimOpen != hasDeck)
		{
			this.m_cardCountPanelAnim.Play((!hasDeck) ? this.CARD_COUNT_PANEL_CLOSE_ANIM : this.CARD_COUNT_PANEL_OPEN_ANIM);
			this.m_cardCountPanelAnimOpen = hasDeck;
		}
	}

	// Token: 0x060030BA RID: 12474 RVA: 0x000F5607 File Offset: 0x000F3807
	private void CreateDeck()
	{
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.TAVERN_BRAWL_DECKEDITOR
		});
		CollectionManagerDisplay.Get().EnterSelectNewDeckHeroMode();
	}

	// Token: 0x060030BB RID: 12475 RVA: 0x000F5630 File Offset: 0x000F3830
	private void EditOrDeleteDeck(UIEvent e)
	{
		if (this.IsInDeckEditMode())
		{
			this.OnDeleteButtonPressed();
		}
		else
		{
			PresenceMgr.Get().SetStatus(new Enum[]
			{
				PresenceStatus.TAVERN_BRAWL_DECKEDITOR
			});
			if (!this.SwitchToEditDeckMode(TavernBrawlManager.Get().CurrentDeck()))
			{
				return;
			}
		}
	}

	// Token: 0x060030BC RID: 12476 RVA: 0x000F5684 File Offset: 0x000F3884
	private bool SwitchToEditDeckMode(CollectionDeck deck)
	{
		if (CollectionManagerDisplay.Get() == null || deck == null)
		{
			return false;
		}
		this.m_tavernBrawlTray.HideTray();
		this.UpdateDeckPanels(true, true);
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_editDeckButton.gameObject.SetActive(TavernBrawlManager.Get().CurrentMission().canEditDeck);
			this.m_editDeckButton.SetText(GameStrings.Get("GLUE_COLLECTION_DECK_DELETE"));
			if (this.m_editIcon != null)
			{
				this.m_editIcon.SetActive(false);
			}
			if (this.m_deleteIcon != null)
			{
				this.m_deleteIcon.SetActive(true);
			}
			this.m_editDeckHighlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		}
		this.m_deckBeingEdited = deck.ID;
		BnetBar.Get().m_currencyFrame.RefreshContents();
		CollectionDeckTray.Get().EnterEditDeckModeForTavernBrawl();
		FriendChallengeMgr.Get().UpdateMyAvailability();
		return true;
	}

	// Token: 0x060030BD RID: 12477 RVA: 0x000F5778 File Offset: 0x000F3978
	private void ShowReward(UIEvent e)
	{
		if (TavernBrawlManager.Get().CurrentMission() == null)
		{
			return;
		}
		RewardType rewardType = TavernBrawlManager.Get().CurrentMission().rewardType;
		if (rewardType != 5)
		{
			if (rewardType != 6)
			{
				Debug.LogErrorFormat("Tavern Brawl reward type currently not supported! Add type {0} to TaverBrawlDisplay.ShowReward().", new object[]
				{
					TavernBrawlManager.Get().CurrentMission().rewardType
				});
				return;
			}
			if (this.m_rewardObject == null)
			{
				int num = (int)TavernBrawlManager.Get().CurrentMission().RewardData1;
				CardBackManager.LoadCardBackData loadCardBackData = CardBackManager.Get().LoadCardBackByIndex(num, false, "Card_Hidden");
				if (loadCardBackData == null)
				{
					Debug.LogErrorFormat("TavernBrawlDisplay.ShowReward() - Could not load cardback ID {0}!", new object[]
					{
						num
					});
					return;
				}
				this.m_rewardObject = loadCardBackData.m_GameObject;
				GameUtils.SetParent(this.m_rewardObject, this.m_rewardContainer, false);
				this.m_rewardObject.transform.localScale = Vector3.one * 5.92f;
			}
		}
		else if (this.m_rewardObject == null)
		{
			int num2 = (int)TavernBrawlManager.Get().CurrentMission().RewardData1;
			BoosterDbfRecord record = GameDbf.Booster.GetRecord(num2);
			if (record == null)
			{
				Debug.LogErrorFormat("TavernBrawlDisplay.ShowReward() - no record found for booster {0}!", new object[]
				{
					num2
				});
				return;
			}
			string packOpeningPrefab = record.PackOpeningPrefab;
			if (string.IsNullOrEmpty(packOpeningPrefab))
			{
				Debug.LogErrorFormat("TavernBrawlDisplay.ShowReward() - no prefab found for booster {0}!", new object[]
				{
					num2
				});
				return;
			}
			GameObject gameObject = AssetLoader.Get().LoadActor(FileUtils.GameAssetPathToName(packOpeningPrefab), false, false);
			if (gameObject == null)
			{
				Debug.LogError(string.Format("TavernBrawlDisplay.ShowReward() - failed to load prefab {0} for booster {1}!", packOpeningPrefab, num2));
				return;
			}
			this.m_rewardObject = gameObject;
			UnopenedPack component = gameObject.GetComponent<UnopenedPack>();
			if (component == null)
			{
				Debug.LogError(string.Format("TavernBrawlDisplay.ShowReward() - No UnopenedPack script found on prefab {0} for booster {1}!", packOpeningPrefab, num2));
				return;
			}
			GameUtils.SetParent(this.m_rewardObject, this.m_rewardContainer, false);
			component.AddBooster();
		}
		this.m_rewardsPreview.SetActive(true);
		iTween.Stop(this.m_rewardsPreview);
		iTween.ScaleTo(this.m_rewardsPreview, iTween.Hash(new object[]
		{
			"scale",
			this.m_rewardsScale,
			"time",
			0.15f
		}));
	}

	// Token: 0x060030BE RID: 12478 RVA: 0x000F59D8 File Offset: 0x000F3BD8
	private void HideReward(UIEvent e)
	{
		iTween.Stop(this.m_rewardsPreview);
		iTween.ScaleTo(this.m_rewardsPreview, iTween.Hash(new object[]
		{
			"scale",
			Vector3.one * 0.01f,
			"time",
			0.15f,
			"oncomplete",
			delegate(object o)
			{
				this.m_rewardsPreview.SetActive(false);
			}
		}));
	}

	// Token: 0x060030BF RID: 12479 RVA: 0x000F5A54 File Offset: 0x000F3C54
	private void StartGame(UIEvent e)
	{
		TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
		if (tavernBrawlMission == null)
		{
			this.RefreshDataBasedUI(0f);
			return;
		}
		if (TavernBrawlManager.Get().SelectHeroBeforeMission())
		{
			if (!(HeroPickerDisplay.Get() == null))
			{
				Log.JMac.PrintWarning("Attempting to load HeroPickerDisplay a second time!", new object[0]);
				return;
			}
			AssetLoader.Get().LoadActor("HeroPicker", false, false);
		}
		else if (tavernBrawlMission.canCreateDeck)
		{
			if (!TavernBrawlManager.Get().HasValidDeck())
			{
				Debug.LogError("Attempting to start a Tavern Brawl game without having a valid deck!");
				return;
			}
			CollectionDeck collectionDeck = TavernBrawlManager.Get().CurrentDeck();
			if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
			{
				FriendChallengeMgr.Get().SelectDeck(collectionDeck.ID);
				FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_TAVERN_BRAWL_OPPONENT_WAITING_READY", new AlertPopup.ResponseCallback(this.OnFriendChallengeWaitingForOpponentDialogResponse));
			}
			else
			{
				TavernBrawlManager.Get().StartGame(collectionDeck.ID);
			}
		}
		else if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
		{
			FriendChallengeMgr.Get().SkipDeckSelection();
			FriendlyChallengeHelper.Get().StartChallengeOrWaitForOpponent("GLOBAL_FRIEND_CHALLENGE_TAVERN_BRAWL_OPPONENT_WAITING_READY", new AlertPopup.ResponseCallback(this.OnFriendChallengeWaitingForOpponentDialogResponse));
		}
		else
		{
			TavernBrawlManager.Get().StartGame(0L);
		}
		this.m_playButton.SetEnabled(false);
	}

	// Token: 0x060030C0 RID: 12480 RVA: 0x000F5BAC File Offset: 0x000F3DAC
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		switch (eventData.m_state)
		{
		case FindGameState.CLIENT_CANCELED:
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_QUEUE_CANCELED:
		case FindGameState.BNET_ERROR:
		case FindGameState.SERVER_GAME_CANCELED:
			this.HandleGameStartupFailure();
			break;
		case FindGameState.SERVER_GAME_STARTED:
			FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnFriendChallengeChanged));
			break;
		}
		return false;
	}

	// Token: 0x060030C1 RID: 12481 RVA: 0x000F5C1D File Offset: 0x000F3E1D
	private void HandleGameStartupFailure()
	{
		if (!TavernBrawlManager.Get().SelectHeroBeforeMission())
		{
			this.EnablePlayButton();
		}
	}

	// Token: 0x060030C2 RID: 12482 RVA: 0x000F5C34 File Offset: 0x000F3E34
	private void OnDeleteButtonPressed()
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_HEADER");
		popupInfo.m_text = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_DESC");
		popupInfo.m_alertTextAlignment = UberText.AlignmentOptions.Center;
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnDeleteButtonConfirmationResponse);
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x060030C3 RID: 12483 RVA: 0x000F5C99 File Offset: 0x000F3E99
	private void OnDeleteButtonConfirmationResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CANCEL)
		{
			return;
		}
		CollectionDeckTray.Get().DeleteEditingDeck(true);
		if (CollectionManagerDisplay.Get())
		{
			CollectionManagerDisplay.Get().OnDoneEditingDeck();
		}
	}

	// Token: 0x060030C4 RID: 12484 RVA: 0x000F5CC8 File Offset: 0x000F3EC8
	private void OnDeckCreated(long deckID)
	{
		CollectionDeck collectionDeck = TavernBrawlManager.Get().CurrentDeck();
		if (collectionDeck != null && deckID == collectionDeck.ID)
		{
			this.SwitchToEditDeckMode(collectionDeck);
		}
	}

	// Token: 0x060030C5 RID: 12485 RVA: 0x000F5CFA File Offset: 0x000F3EFA
	private void OnDeckDeleted(long deckID)
	{
		if (deckID == this.m_deckBeingEdited && TavernBrawlDisplay.IsTavernBrawlOpen())
		{
			base.StartCoroutine(this.WaitThenCreateDeck());
		}
	}

	// Token: 0x060030C6 RID: 12486 RVA: 0x000F5D20 File Offset: 0x000F3F20
	private IEnumerator WaitThenPlayWipeAnim(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (this.m_chalkboard != null && TavernBrawlManager.Get().IsTavernBrawlActive && TavernBrawlManager.Get().IsScenarioDataReady)
		{
			PlayMakerFSM fsm = this.m_chalkboard.GetComponent<PlayMakerFSM>();
			fsm.SendEvent((!this.m_doWipeAnimation) ? "QuickShow" : "Wipe");
		}
		yield break;
	}

	// Token: 0x060030C7 RID: 12487 RVA: 0x000F5D4C File Offset: 0x000F3F4C
	private IEnumerator WaitThenCreateDeck()
	{
		yield return new WaitForEndOfFrame();
		this.CreateDeck();
		yield return new WaitForSeconds(0.4f);
		this.BackFromDeckEdit(false);
		yield break;
	}

	// Token: 0x060030C8 RID: 12488 RVA: 0x000F5D68 File Offset: 0x000F3F68
	private void OnDeckContents(long deckID)
	{
		CollectionDeck collectionDeck = TavernBrawlManager.Get().CurrentDeck();
		if (collectionDeck != null && deckID == collectionDeck.ID && TavernBrawlDisplay.IsTavernBrawlOpen())
		{
			this.ValidateDeck();
		}
	}

	// Token: 0x060030C9 RID: 12489 RVA: 0x000F5DA4 File Offset: 0x000F3FA4
	private void OnFriendChallengeWaitingForOpponentDialogResponse(AlertPopup.Response response, object userData)
	{
		if (response != AlertPopup.Response.CANCEL)
		{
			return;
		}
		FriendChallengeMgr.Get().DeselectDeck();
		FriendlyChallengeHelper.Get().StopWaitingForFriendChallenge();
		if (!TavernBrawlManager.Get().SelectHeroBeforeMission())
		{
			this.EnablePlayButton();
		}
	}

	// Token: 0x060030CA RID: 12490 RVA: 0x000F5DE4 File Offset: 0x000F3FE4
	private void OnFriendChallengeChanged(FriendChallengeEvent challengeEvent, BnetPlayer player, object userData)
	{
		if (challengeEvent == FriendChallengeEvent.OPPONENT_ACCEPTED_CHALLENGE || challengeEvent == FriendChallengeEvent.I_ACCEPTED_CHALLENGE)
		{
			this.SetUIForFriendlyChallenge(true);
		}
		else if (challengeEvent == FriendChallengeEvent.SELECTED_DECK)
		{
			if (player == BnetPresenceMgr.Get().GetMyPlayer())
			{
				return;
			}
			if (!FriendChallengeMgr.Get().DidISelectDeck())
			{
				return;
			}
			FriendlyChallengeHelper.Get().HideFriendChallengeWaitingForOpponentDialog();
			FriendlyChallengeHelper.Get().WaitForFriendChallengeToStart();
		}
		else if (challengeEvent == FriendChallengeEvent.I_RESCINDED_CHALLENGE || challengeEvent == FriendChallengeEvent.OPPONENT_DECLINED_CHALLENGE || challengeEvent == FriendChallengeEvent.OPPONENT_RESCINDED_CHALLENGE)
		{
			this.SetUIForFriendlyChallenge(false);
		}
		else if (challengeEvent == FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE || challengeEvent == FriendChallengeEvent.OPPONENT_REMOVED_FROM_FRIENDS)
		{
			this.SetUIForFriendlyChallenge(false);
			FriendlyChallengeHelper.Get().StopWaitingForFriendChallenge();
		}
	}

	// Token: 0x04001E4E RID: 7758
	[CustomEditField(Sections = "Buttons")]
	public UIBButton m_createDeckButton;

	// Token: 0x04001E4F RID: 7759
	[CustomEditField(Sections = "Buttons")]
	public UIBButton m_editDeckButton;

	// Token: 0x04001E50 RID: 7760
	[CustomEditField(Sections = "Buttons")]
	public PlayButton m_playButton;

	// Token: 0x04001E51 RID: 7761
	[CustomEditField(Sections = "Buttons")]
	public UIBButton m_backButton;

	// Token: 0x04001E52 RID: 7762
	[CustomEditField(Sections = "Buttons")]
	public PegUIElement m_rewardChest;

	// Token: 0x04001E53 RID: 7763
	[CustomEditField(Sections = "Strings")]
	public UberText m_chalkboardHeader;

	// Token: 0x04001E54 RID: 7764
	[CustomEditField(Sections = "Strings")]
	public UberText m_chalkboardInfo;

	// Token: 0x04001E55 RID: 7765
	[CustomEditField(Sections = "Strings")]
	public UberText m_chalkboardEndInfo;

	// Token: 0x04001E56 RID: 7766
	[CustomEditField(Sections = "Strings")]
	public UberText m_numWins;

	// Token: 0x04001E57 RID: 7767
	[CustomEditField(Sections = "Animating Elements")]
	public SlidingTray m_tavernBrawlTray;

	// Token: 0x04001E58 RID: 7768
	[CustomEditField(Sections = "Animating Elements")]
	public SlidingTray m_cardListPanel;

	// Token: 0x04001E59 RID: 7769
	[CustomEditField(Sections = "Animating Elements")]
	public Animation m_cardCountPanelAnim;

	// Token: 0x04001E5A RID: 7770
	[CustomEditField(Sections = "Animating Elements")]
	public GameObject m_rewardsPreview;

	// Token: 0x04001E5B RID: 7771
	[CustomEditField(Sections = "Animating Elements")]
	public GameObject m_rewardContainer;

	// Token: 0x04001E5C RID: 7772
	[CustomEditField(Sections = "Animating Elements")]
	public UberText m_rewardsText;

	// Token: 0x04001E5D RID: 7773
	[CustomEditField(Sections = "Highlights")]
	public HighlightState m_createDeckHighlight;

	// Token: 0x04001E5E RID: 7774
	[CustomEditField(Sections = "Highlights")]
	public HighlightState m_rewardHighlight;

	// Token: 0x04001E5F RID: 7775
	[CustomEditField(Sections = "Highlights")]
	public HighlightState m_editDeckHighlight;

	// Token: 0x04001E60 RID: 7776
	public GameObject m_winsBanner;

	// Token: 0x04001E61 RID: 7777
	public GameObject m_panelWithCreateDeck;

	// Token: 0x04001E62 RID: 7778
	public GameObject m_fullPanel;

	// Token: 0x04001E63 RID: 7779
	public GameObject m_chalkboard;

	// Token: 0x04001E64 RID: 7780
	public Material m_chestOpenMaterial;

	// Token: 0x04001E65 RID: 7781
	public float m_wipeAnimStartDelay;

	// Token: 0x04001E66 RID: 7782
	public PegUIElement m_rewardOffClickCatcher;

	// Token: 0x04001E67 RID: 7783
	public GameObject m_editIcon;

	// Token: 0x04001E68 RID: 7784
	public GameObject m_deleteIcon;

	// Token: 0x04001E69 RID: 7785
	public UberText m_editText;

	// Token: 0x04001E6A RID: 7786
	public Color m_disabledTextColor = new Color(0.5f, 0.5f, 0.5f);

	// Token: 0x04001E6B RID: 7787
	private static TavernBrawlDisplay s_instance;

	// Token: 0x04001E6C RID: 7788
	private bool m_doWipeAnimation;

	// Token: 0x04001E6D RID: 7789
	private long m_deckBeingEdited;

	// Token: 0x04001E6E RID: 7790
	private GameObject m_rewardObject;

	// Token: 0x04001E6F RID: 7791
	private Vector3 m_rewardsScale;

	// Token: 0x04001E70 RID: 7792
	private readonly string CARD_COUNT_PANEL_OPEN_ANIM = "TavernBrawl_DecksNumberCoverUp_Open";

	// Token: 0x04001E71 RID: 7793
	private readonly string CARD_COUNT_PANEL_CLOSE_ANIM = "TavernBrawl_DecksNumberCoverUp_Close";

	// Token: 0x04001E72 RID: 7794
	private bool m_cardCountPanelAnimOpen;

	// Token: 0x04001E73 RID: 7795
	private Color? m_originalEditTextColor;

	// Token: 0x04001E74 RID: 7796
	private Color? m_originalEditIconColor;

	// Token: 0x04001E75 RID: 7797
	private static readonly PlatformDependentValue<string> DEFAULT_CHALKBOARD_TEXTURE_NAME_NO_DECK = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "TavernBrawl_Chalkboard_Default_NoBorders",
		Phone = "TavernBrawl_Chalkboard_Default_phone"
	};

	// Token: 0x04001E76 RID: 7798
	private static readonly PlatformDependentValue<string> DEFAULT_CHALKBOARD_TEXTURE_NAME_WITH_DECK = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "TavernBrawl_Chalkboard_Default_Borders",
		Phone = "TavernBrawl_Chalkboard_Default_phone"
	};

	// Token: 0x04001E77 RID: 7799
	private static readonly PlatformDependentValue<Vector2> DEFAULT_CHALKBOARD_TEXTURE_OFFSET_NO_DECK = new PlatformDependentValue<Vector2>(PlatformCategory.Screen)
	{
		PC = Vector2.zero,
		Phone = Vector2.zero
	};

	// Token: 0x04001E78 RID: 7800
	private static readonly PlatformDependentValue<Vector2> DEFAULT_CHALKBOARD_TEXTURE_OFFSET_WITH_DECK = new PlatformDependentValue<Vector2>(PlatformCategory.Screen)
	{
		PC = Vector2.zero,
		Phone = new Vector2(0f, -0.389f)
	};
}
