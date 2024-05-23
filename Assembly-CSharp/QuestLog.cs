using System;
using System.Collections.Generic;
using PegasusShared;
using UnityEngine;

// Token: 0x02000252 RID: 594
[CustomEditClass]
public class QuestLog : UIBPopup
{
	// Token: 0x060021C3 RID: 8643 RVA: 0x000A5584 File Offset: 0x000A3784
	private void Awake()
	{
		QuestLog.s_instance = this;
		this.m_presencePrevStatus = PresenceMgr.Get().GetStatus();
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.QUESTLOG
		});
		if (this.m_closeButton != null)
		{
			this.m_closeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCloseButtonReleased));
		}
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x000A55EC File Offset: 0x000A37EC
	private void Start()
	{
		if (this.m_classProgressPrefab != null)
		{
			for (int i = 0; i < this.m_classProgressInfos.Count; i++)
			{
				ClassProgressInfo classProgressInfo = this.m_classProgressInfos[i];
				TAG_CLASS @class = classProgressInfo.m_class;
				ClassProgressBar classProgressBar = (ClassProgressBar)GameUtils.Instantiate(this.m_classProgressPrefab, classProgressInfo.m_bone, true);
				SceneUtils.SetLayer(classProgressBar, classProgressInfo.m_bone.layer);
				TransformUtil.Identity(classProgressBar.transform);
				classProgressBar.m_class = @class;
				classProgressBar.m_classIcon.GetComponent<Renderer>().material = classProgressInfo.m_iconMaterial;
				classProgressBar.Init();
				this.m_classProgressBars.Add(classProgressBar);
			}
		}
		this.m_offClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnQuestLogCloseEvent));
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x000A56B8 File Offset: 0x000A38B8
	private void OnDestroy()
	{
		if (ShownUIMgr.Get() != null)
		{
			ShownUIMgr.Get().ClearShownUI();
		}
		if (AchieveManager.Get() != null)
		{
			AchieveManager.Get().RemoveActiveAchievesUpdatedListener(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnActiveAchievesUpdated));
			AchieveManager.Get().RemoveQuestCanceledListener(new AchieveManager.AchieveCanceledCallback(this.OnQuestCanceled));
		}
		if (Network.IsRunning())
		{
			PresenceMgr.Get().SetPrevStatus();
		}
		QuestLog.s_instance = null;
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x000A5732 File Offset: 0x000A3932
	public static QuestLog Get()
	{
		return QuestLog.s_instance;
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x000A573C File Offset: 0x000A393C
	public override void Show()
	{
		AchieveManager.Get().RegisterQuestCanceledListener(new AchieveManager.AchieveCanceledCallback(this.OnQuestCanceled));
		this.UpdateData();
		FullScreenFXMgr.Get().StartStandardBlurVignette(0.1f);
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
		base.Show();
		if (UniversalInputManager.UsePhoneUI)
		{
			OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.WIDTH);
		}
	}

	// Token: 0x060021C8 RID: 8648 RVA: 0x000A57B0 File Offset: 0x000A39B0
	protected override void Hide(bool animate)
	{
		if (this.m_presencePrevStatus == null)
		{
			this.m_presencePrevStatus = new Enum[]
			{
				PresenceStatus.HUB
			};
		}
		PresenceMgr.Get().SetStatus(this.m_presencePrevStatus);
		if (ShownUIMgr.Get() != null)
		{
			ShownUIMgr.Get().ClearShownUI();
		}
		base.DoHideAnimation(!animate, delegate()
		{
			AchieveManager.Get().RemoveQuestCanceledListener(new AchieveManager.AchieveCanceledCallback(this.OnQuestCanceled));
			this.DeleteQuests();
			FullScreenFXMgr.Get().EndStandardBlurVignette(0.1f, null);
			this.m_shown = false;
		});
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x000A5820 File Offset: 0x000A3A20
	private void DeleteQuests()
	{
		if (this.m_currentQuests == null || this.m_currentQuests.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.m_currentQuests.Count; i++)
		{
			if (!(this.m_currentQuests[i] == null))
			{
				Object.Destroy(this.m_currentQuests[i].gameObject);
			}
		}
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x000A5897 File Offset: 0x000A3A97
	private void OnQuestLogCloseEvent(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x000A589F File Offset: 0x000A3A9F
	private bool OnNavigateBack()
	{
		this.Hide(true);
		return true;
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x000A58AC File Offset: 0x000A3AAC
	private void UpdateData()
	{
		this.UpdateClassProgress();
		this.UpdateActiveQuests();
		this.UpdateCurrentMedal();
		this.UpdateBestArenaMedal();
		this.UpdateTotalWins();
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x000A58D8 File Offset: 0x000A3AD8
	private void UpdateTotalWins()
	{
		int num = 0;
		int num2 = 0;
		foreach (NetCache.PlayerRecord playerRecord in NetCache.Get().GetNetObject<NetCache.NetCachePlayerRecords>().Records)
		{
			if (playerRecord.Data == 0)
			{
				GameType recordType = playerRecord.RecordType;
				switch (recordType)
				{
				case 5:
					num2 += playerRecord.Wins;
					continue;
				default:
					if (recordType != 16)
					{
						continue;
					}
					break;
				case 7:
				case 8:
					break;
				}
				num += playerRecord.Wins;
			}
		}
		this.m_winsCountText.Text = num.ToString();
		this.m_forgeRecordCountText.Text = num2.ToString();
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x000A59B8 File Offset: 0x000A3BB8
	private void UpdateBestArenaMedal()
	{
		NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
		if (this.m_arenaMedal == null)
		{
			this.m_arenaMedal = (ArenaMedal)GameUtils.Instantiate(this.m_arenaMedalPrefab, this.m_arenaMedalBone.gameObject, true);
			SceneUtils.SetLayer(this.m_arenaMedal, this.m_arenaMedalBone.gameObject.layer);
			this.m_arenaMedal.transform.localScale = Vector3.one;
		}
		if (netObject.LastForgeDate != 0L)
		{
			this.m_arenaMedal.gameObject.SetActive(true);
			this.m_arenaMedal.SetMedal(netObject.BestForgeWins);
		}
		else
		{
			this.m_arenaMedal.gameObject.SetActive(false);
		}
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x000A5A78 File Offset: 0x000A3C78
	private void UpdateCurrentMedal()
	{
		NetCache.NetCacheMedalInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMedalInfo>();
		if (this.m_currentMedal == null)
		{
			this.m_currentMedal = (TournamentMedal)GameUtils.Instantiate(this.m_medalPrefab, this.m_medalBone.gameObject, true);
			SceneUtils.SetLayer(this.m_currentMedal, base.gameObject.layer);
			this.m_currentMedal.transform.localScale = Vector3.one;
		}
		this.m_currentMedal.SetMedal(netObject, false);
		this.m_currentMedal.SetFormat(this.m_currentMedal.IsBestCurrentRankWild());
		int num = Math.Max(netObject.Standard.BestStarLevel, netObject.Wild.BestStarLevel);
		int rank = 26 - num;
		this.m_rewardChest.SetRank(rank);
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x000A5B40 File Offset: 0x000A3D40
	private void UpdateClassProgress()
	{
		if (this.m_classProgressBars.Count == 0)
		{
			return;
		}
		int num = 0;
		List<Achievement> achievesInGroup = AchieveManager.Get().GetAchievesInGroup(Achievement.AchType.UNLOCK_HERO, true);
		NetCache.NetCacheHeroLevels netObject = NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>();
		ClassProgressBar classProgress;
		foreach (ClassProgressBar classProgress2 in this.m_classProgressBars)
		{
			classProgress = classProgress2;
			if (!(classProgress.m_classLockedGO == null))
			{
				Achievement achievement = achievesInGroup.Find((Achievement obj) => obj.ClassRequirement != null && obj.ClassRequirement.Value == classProgress.m_class);
				if (achievement != null)
				{
					classProgress.m_classLockedGO.SetActive(false);
					NetCache.HeroLevel heroLevel = netObject.Levels.Find((NetCache.HeroLevel obj) => obj.Class == classProgress.m_class);
					classProgress.m_levelText.Text = heroLevel.CurrentLevel.Level.ToString();
					if (heroLevel.CurrentLevel.IsMaxLevel())
					{
						classProgress.m_progressBar.SetProgressBar(1f);
					}
					else
					{
						classProgress.m_progressBar.SetProgressBar((float)heroLevel.CurrentLevel.XP / (float)heroLevel.CurrentLevel.MaxXP);
					}
					classProgress.SetNextReward(heroLevel.NextReward);
					num += heroLevel.CurrentLevel.Level;
				}
				else
				{
					classProgress.m_levelText.Text = string.Empty;
					classProgress.m_classLockedGO.SetActive(true);
				}
			}
		}
		if (this.m_totalLevelsText != null)
		{
			this.m_totalLevelsText.Text = string.Format(GameStrings.Get("GLUE_QUEST_LOG_TOTAL_LEVELS"), num);
		}
	}

	// Token: 0x060021D1 RID: 8657 RVA: 0x000A5D3C File Offset: 0x000A3F3C
	private void UpdateActiveQuests()
	{
		List<Achievement> activeQuests = AchieveManager.Get().GetActiveQuests(false);
		Log.Ben.Print(string.Format("Found {0} activeQuests! I should do something awesome with them here.", activeQuests.Count), new object[0]);
		this.m_currentQuests = new List<QuestTile>();
		for (int i = 0; i < activeQuests.Count; i++)
		{
			if (i < 3)
			{
				this.AddCurrentQuestTile(activeQuests[i], i);
			}
		}
		if (this.m_currentQuests.Count == 0)
		{
			this.m_noQuestText.gameObject.SetActive(true);
			if (AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.DAILY_QUESTS))
			{
				this.m_noQuestText.Text = GameStrings.Get("GLUE_QUEST_LOG_NO_QUESTS_DAILIES_UNLOCKED");
				if (!Options.Get().GetBool(Option.HAS_RUN_OUT_OF_QUESTS, false) && UserAttentionManager.CanShowAttentionGrabber("QuestLog.UpdateActiveQuests:" + Option.HAS_RUN_OUT_OF_QUESTS))
				{
					NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_OUT_OF_QUESTS"), "VO_INNKEEPER_OUT_OF_QUESTS", 0f, null);
					Options.Get().SetBool(Option.HAS_RUN_OUT_OF_QUESTS, true);
				}
			}
			else
			{
				this.m_noQuestText.Text = GameStrings.Get("GLUE_QUEST_LOG_NO_QUESTS");
			}
		}
		else
		{
			this.m_noQuestText.gameObject.SetActive(false);
		}
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x000A5E9C File Offset: 0x000A409C
	private void AddCurrentQuestTile(Achievement achieveQuest, int slot)
	{
		GameObject gameObject = (GameObject)GameUtils.Instantiate(this.m_questTilePrefab, this.m_questBones[slot].gameObject, true);
		SceneUtils.SetLayer(gameObject, this.m_questBones[slot].gameObject.layer);
		gameObject.transform.localScale = Vector3.one;
		QuestTile component = gameObject.GetComponent<QuestTile>();
		component.SetupTile(achieveQuest);
		component.SetCanShowCancelButton(true);
		this.m_currentQuests.Add(component);
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x000A5F1C File Offset: 0x000A411C
	private void OnQuestCanceled(int achieveID, bool canceled, object userData)
	{
		Log.Rachelle.Print("QuestLog.OnQuestCanceled({0},{1})", new object[]
		{
			achieveID,
			canceled
		});
		if (!canceled)
		{
			return;
		}
		this.m_justCanceledQuestID = achieveID;
		AchieveManager.Get().UpdateActiveAchieves(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnActiveAchievesUpdated));
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x000A5F74 File Offset: 0x000A4174
	private void OnActiveAchievesUpdated(object userData)
	{
		int justCanceledQuest = this.m_justCanceledQuestID;
		this.m_justCanceledQuestID = 0;
		QuestTile questTile = this.m_currentQuests.Find((QuestTile obj) => obj.GetQuestID() == justCanceledQuest);
		if (questTile == null)
		{
			Debug.LogError(string.Format("QuestLog.OnActiveAchievesUpdated(): could not find tile for just canceled quest (quest ID {0})", justCanceledQuest));
			this.Hide();
			return;
		}
		List<Achievement> activeQuests = AchieveManager.Get().GetActiveQuests(true);
		if (activeQuests.Count < 1 || (activeQuests.Count > 1 && !Vars.Key("Quests.CanCancelManyTimes").GetBool(false) && !Vars.Key("Quests.CancelGivesManyNewQuests").GetBool(false)))
		{
			Debug.LogError(string.Format("QuestLog.OnActiveAchievesUpdated(): expecting ONE new active quest after a quest cancel but received {0}", activeQuests.Count));
			this.Hide();
			return;
		}
		questTile.SetupTile(activeQuests[0]);
		questTile.PlayBirth();
		for (int i = 1; i < activeQuests.Count; i++)
		{
			int count = this.m_currentQuests.Count;
			if (count >= this.m_questBones.Count)
			{
				break;
			}
			this.AddCurrentQuestTile(activeQuests[i], count);
		}
		foreach (QuestTile questTile2 in this.m_currentQuests)
		{
			questTile2.UpdateCancelButtonVisibility();
		}
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x000A60FC File Offset: 0x000A42FC
	private void OnCloseButtonReleased(UIEvent e)
	{
		this.OnNavigateBack();
	}

	// Token: 0x04001328 RID: 4904
	public const int QUEST_LOG_MAX_COUNT = 3;

	// Token: 0x04001329 RID: 4905
	public GameObject m_root;

	// Token: 0x0400132A RID: 4906
	public UberText m_winsCountText;

	// Token: 0x0400132B RID: 4907
	public UberText m_forgeRecordCountText;

	// Token: 0x0400132C RID: 4908
	public UberText m_totalLevelsText;

	// Token: 0x0400132D RID: 4909
	public Transform m_medalBone;

	// Token: 0x0400132E RID: 4910
	public TournamentMedal m_medalPrefab;

	// Token: 0x0400132F RID: 4911
	public Transform m_arenaMedalBone;

	// Token: 0x04001330 RID: 4912
	public ArenaMedal m_arenaMedalPrefab;

	// Token: 0x04001331 RID: 4913
	public PegUIElement m_offClickCatcher;

	// Token: 0x04001332 RID: 4914
	public List<ClassProgressBar> m_classProgressBars;

	// Token: 0x04001333 RID: 4915
	public List<ClassProgressInfo> m_classProgressInfos;

	// Token: 0x04001334 RID: 4916
	public ClassProgressBar m_classProgressPrefab;

	// Token: 0x04001335 RID: 4917
	public RankedRewardChest2D m_rewardChest;

	// Token: 0x04001336 RID: 4918
	public GameObject m_questTilePrefab;

	// Token: 0x04001337 RID: 4919
	public List<Transform> m_questBones;

	// Token: 0x04001338 RID: 4920
	public UberText m_noQuestText;

	// Token: 0x04001339 RID: 4921
	public UIBButton m_closeButton;

	// Token: 0x0400133A RID: 4922
	[CustomEditField(Sections = "Aspect Ratio Positioning")]
	public float m_yPosRoot3to2;

	// Token: 0x0400133B RID: 4923
	[CustomEditField(Sections = "Aspect Ratio Positioning")]
	public float m_yPosRoot16to9 = 0.475f;

	// Token: 0x0400133C RID: 4924
	private List<QuestTile> m_currentQuests;

	// Token: 0x0400133D RID: 4925
	private static QuestLog s_instance;

	// Token: 0x0400133E RID: 4926
	private int m_justCanceledQuestID;

	// Token: 0x0400133F RID: 4927
	private TournamentMedal m_currentMedal;

	// Token: 0x04001340 RID: 4928
	private ArenaMedal m_arenaMedal;

	// Token: 0x04001341 RID: 4929
	private Enum[] m_presencePrevStatus;
}
