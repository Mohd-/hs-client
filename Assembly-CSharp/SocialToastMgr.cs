using System;
using System.Collections;
using System.Collections.Generic;
using bgs;
using bgs.types;
using UnityEngine;

// Token: 0x020004F3 RID: 1267
public class SocialToastMgr : MonoBehaviour
{
	// Token: 0x06003B54 RID: 15188 RVA: 0x0011F86C File Offset: 0x0011DA6C
	private void Awake()
	{
		SocialToastMgr.s_instance = this;
		this.m_toast = Object.Instantiate<SocialToast>(this.m_socialToastPrefab);
		RenderUtils.SetAlpha(this.m_toast.gameObject, 0f);
		this.m_toast.gameObject.SetActive(false);
		this.m_toast.transform.parent = BnetBar.Get().m_socialToastBone.transform;
		this.m_toast.transform.localRotation = Quaternion.Euler(new Vector3(90f, 180f, 0f));
		this.m_toast.transform.localScale = this.TOAST_SCALE;
		this.m_toast.transform.position = BnetBar.Get().m_socialToastBone.transform.position;
		BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		BnetPresenceMgr.Get().OnGameAccountPresenceChange += new Action<PresenceUpdate[]>(this.OnPresenceChanged);
		BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
		Network.Get().SetShutdownHandler(new Network.ShutdownHandler(this.ShutdownHandler));
		SoundManager.Get().Load("UI_BnetToast");
	}

	// Token: 0x06003B55 RID: 15189 RVA: 0x0011F9A8 File Offset: 0x0011DBA8
	private void OnDestroy()
	{
		BnetPresenceMgr.Get().OnGameAccountPresenceChange -= new Action<PresenceUpdate[]>(this.OnPresenceChanged);
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		BnetFriendMgr.Get().RemoveChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
		this.m_lastKnownMedals.Clear();
		SocialToastMgr.s_instance = null;
	}

	// Token: 0x06003B56 RID: 15190 RVA: 0x0011FA0A File Offset: 0x0011DC0A
	public static SocialToastMgr Get()
	{
		return SocialToastMgr.s_instance;
	}

	// Token: 0x06003B57 RID: 15191 RVA: 0x0011FA14 File Offset: 0x0011DC14
	public void Reset()
	{
		iTween.Stop(this.m_toast.gameObject, true);
		iTween.Stop(base.gameObject, true);
		RenderUtils.SetAlpha(this.m_toast.gameObject, 0f);
		this.DeactivateToast();
	}

	// Token: 0x06003B58 RID: 15192 RVA: 0x0011FA59 File Offset: 0x0011DC59
	public void AddToast(UserAttentionBlocker blocker, string textArg)
	{
		this.AddToast(blocker, textArg, SocialToastMgr.TOAST_TYPE.DEFAULT, 2f, true);
	}

	// Token: 0x06003B59 RID: 15193 RVA: 0x0011FA6A File Offset: 0x0011DC6A
	public void AddToast(UserAttentionBlocker blocker, string textArg, SocialToastMgr.TOAST_TYPE toastType)
	{
		this.AddToast(blocker, textArg, toastType, 2f, true);
	}

	// Token: 0x06003B5A RID: 15194 RVA: 0x0011FA7B File Offset: 0x0011DC7B
	public void AddToast(UserAttentionBlocker blocker, string textArg, SocialToastMgr.TOAST_TYPE toastType, bool playSound)
	{
		this.AddToast(blocker, textArg, toastType, 2f, playSound);
	}

	// Token: 0x06003B5B RID: 15195 RVA: 0x0011FA8D File Offset: 0x0011DC8D
	public void AddToast(UserAttentionBlocker blocker, string textArg, SocialToastMgr.TOAST_TYPE toastType, float displayTime)
	{
		this.AddToast(blocker, textArg, toastType, displayTime, true);
	}

	// Token: 0x06003B5C RID: 15196 RVA: 0x0011FA9C File Offset: 0x0011DC9C
	public void AddToast(UserAttentionBlocker blocker, string textArg, SocialToastMgr.TOAST_TYPE toastType, float displayTime, bool playSound)
	{
		if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "SocialToastMgr.AddToast:" + toastType))
		{
			return;
		}
		string text;
		switch (toastType)
		{
		case SocialToastMgr.TOAST_TYPE.DEFAULT:
			text = textArg;
			goto IL_189;
		case SocialToastMgr.TOAST_TYPE.FRIEND_ONLINE:
			text = GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ONLINE", new object[]
			{
				"5ecaf0ff",
				textArg
			});
			goto IL_189;
		case SocialToastMgr.TOAST_TYPE.FRIEND_OFFLINE:
			text = GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_OFFLINE", new object[]
			{
				"999999ff",
				textArg
			});
			goto IL_189;
		case SocialToastMgr.TOAST_TYPE.FRIEND_INVITE:
			text = GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_REQUEST", new object[]
			{
				"5ecaf0ff",
				textArg
			});
			goto IL_189;
		case SocialToastMgr.TOAST_TYPE.HEALTHY_GAMING:
			text = GameStrings.Format("GLOBAL_HEALTHY_GAMING_TOAST", new object[]
			{
				textArg
			});
			goto IL_189;
		case SocialToastMgr.TOAST_TYPE.HEALTHY_GAMING_OVER_THRESHOLD:
			text = GameStrings.Format("GLOBAL_HEALTHY_GAMING_TOAST_OVER_THRESHOLD", new object[]
			{
				textArg
			});
			goto IL_189;
		case SocialToastMgr.TOAST_TYPE.SPECTATOR_INVITE_SENT:
			text = GameStrings.Format("GLOBAL_SOCIAL_TOAST_SPECTATOR_INVITE_SENT", new object[]
			{
				"5ecaf0ff",
				textArg
			});
			goto IL_189;
		case SocialToastMgr.TOAST_TYPE.SPECTATOR_INVITE_RECEIVED:
			text = GameStrings.Format("GLOBAL_SOCIAL_TOAST_SPECTATOR_INVITE_RECEIVED", new object[]
			{
				"5ecaf0ff",
				textArg
			});
			goto IL_189;
		case SocialToastMgr.TOAST_TYPE.SPECTATOR_ADDED:
			text = GameStrings.Format("GLOBAL_SOCIAL_TOAST_SPECTATOR_ADDED", new object[]
			{
				"5ecaf0ff",
				textArg
			});
			goto IL_189;
		case SocialToastMgr.TOAST_TYPE.SPECTATOR_REMOVED:
			text = GameStrings.Format("GLOBAL_SOCIAL_TOAST_SPECTATOR_REMOVED", new object[]
			{
				"5ecaf0ff",
				textArg
			});
			goto IL_189;
		}
		text = string.Empty;
		IL_189:
		if (this.m_toastIsShown)
		{
			iTween.Stop(this.m_toast.gameObject, true);
			iTween.Stop(base.gameObject, true);
			RenderUtils.SetAlpha(this.m_toast.gameObject, 0f);
		}
		this.m_toast.gameObject.SetActive(true);
		this.m_toast.SetText(text);
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			1f,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeInCubic,
			"oncomplete",
			"FadeOutToast",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			displayTime,
			"name",
			"fade"
		});
		iTween.StopByName(base.gameObject, "fade");
		iTween.FadeTo(this.m_toast.gameObject, args);
		this.m_toastIsShown = true;
		if (playSound)
		{
			SoundManager.Get().LoadAndPlay("UI_BnetToast");
		}
	}

	// Token: 0x06003B5D RID: 15197 RVA: 0x0011FD5C File Offset: 0x0011DF5C
	private void FadeOutToast(float displayTime)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			0f,
			"delay",
			displayTime,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeInCubic,
			"oncomplete",
			"DeactivateToast",
			"oncompletetarget",
			base.gameObject,
			"name",
			"fade"
		});
		iTween.FadeTo(this.m_toast.gameObject, args);
	}

	// Token: 0x06003B5E RID: 15198 RVA: 0x0011FE09 File Offset: 0x0011E009
	private void DeactivateToast()
	{
		this.m_toast.gameObject.SetActive(false);
		this.m_toastIsShown = false;
	}

	// Token: 0x06003B5F RID: 15199 RVA: 0x0011FE24 File Offset: 0x0011E024
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		if (!DemoMgr.Get().IsSocialEnabled())
		{
			return;
		}
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		foreach (BnetPlayerChange bnetPlayerChange in changelist.GetChanges())
		{
			if (bnetPlayerChange.GetPlayer() != null && bnetPlayerChange.GetNewPlayer() != null)
			{
				if (bnetPlayerChange != null)
				{
					if (bnetPlayerChange.GetPlayer().IsDisplayable())
					{
						if (bnetPlayerChange.GetPlayer() != myPlayer)
						{
							if (BnetFriendMgr.Get().IsFriend(bnetPlayerChange.GetPlayer()))
							{
								BnetPlayer oldPlayer = bnetPlayerChange.GetOldPlayer();
								BnetPlayer newPlayer = bnetPlayerChange.GetNewPlayer();
								this.CheckForOnlineStatusChanged(oldPlayer, newPlayer);
								if (oldPlayer != null)
								{
									BnetGameAccount hearthstoneGameAccount = newPlayer.GetHearthstoneGameAccount();
									BnetGameAccount hearthstoneGameAccount2 = oldPlayer.GetHearthstoneGameAccount();
									if (!(hearthstoneGameAccount2 == null) && !(hearthstoneGameAccount == null))
									{
										this.CheckForCardOpened(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
										this.CheckForDruidLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
										this.CheckForHunterLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
										this.CheckForMageLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
										this.CheckForPaladinLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
										this.CheckForPriestLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
										this.CheckForRogueLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
										this.CheckForShamanLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
										this.CheckForWarlockLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
										this.CheckForWarriorLevelChanged(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
										this.CheckForMissionComplete(hearthstoneGameAccount2, hearthstoneGameAccount, newPlayer);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003B60 RID: 15200 RVA: 0x0011FFDC File Offset: 0x0011E1DC
	private void OnPresenceChanged(PresenceUpdate[] updates)
	{
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		foreach (PresenceUpdate presenceUpdate in updates)
		{
			if (!(presenceUpdate.programId != BnetProgramId.HEARTHSTONE))
			{
				BnetGameAccountId id = BnetGameAccountId.CreateFromEntityId(presenceUpdate.entityId);
				BnetPlayer player = BnetUtils.GetPlayer(id);
				if (player != null && player != myPlayer && player.IsDisplayable() && BnetFriendMgr.Get().IsFriend(player))
				{
					uint fieldId = presenceUpdate.fieldId;
					if (fieldId != 17U)
					{
						if (fieldId != 18U)
						{
							if (fieldId == 3U)
							{
								this.CheckArenaRecordChanged(player);
							}
						}
						else
						{
							this.CheckForNewRank(player);
						}
					}
					else
					{
						this.CheckArenaGameStarted(player);
					}
				}
			}
		}
	}

	// Token: 0x06003B61 RID: 15201 RVA: 0x001200CC File Offset: 0x0011E2CC
	private void CheckForOnlineStatusChanged(BnetPlayer oldPlayer, BnetPlayer newPlayer)
	{
		if (oldPlayer != null && oldPlayer.IsOnline() == newPlayer.IsOnline())
		{
			return;
		}
		ulong bestLastOnlineMicrosec = newPlayer.GetBestLastOnlineMicrosec();
		ulong bestLastOnlineMicrosec2 = BnetPresenceMgr.Get().GetMyPlayer().GetBestLastOnlineMicrosec();
		if (bestLastOnlineMicrosec == 0UL || bestLastOnlineMicrosec2 == 0UL || bestLastOnlineMicrosec2 > bestLastOnlineMicrosec)
		{
			return;
		}
		SocialToastMgr.LastOnlineTracker lastOnlineTracker = null;
		float fixedTime = Time.fixedTime;
		int hashCode = newPlayer.GetAccountId().GetHashCode();
		if (!this.m_lastOnlineTracker.TryGetValue(hashCode, out lastOnlineTracker))
		{
			lastOnlineTracker = new SocialToastMgr.LastOnlineTracker();
			this.m_lastOnlineTracker[hashCode] = lastOnlineTracker;
		}
		if (newPlayer.IsOnline())
		{
			if (lastOnlineTracker.m_callback != null)
			{
				ApplicationMgr.Get().CancelScheduledCallback(lastOnlineTracker.m_callback, null);
			}
			lastOnlineTracker.m_callback = null;
			if (fixedTime - lastOnlineTracker.m_localLastOnlineTime >= 5f)
			{
				this.AddToast(UserAttentionBlocker.NONE, newPlayer.GetBestName(), SocialToastMgr.TOAST_TYPE.FRIEND_ONLINE);
			}
		}
		else
		{
			lastOnlineTracker.m_localLastOnlineTime = fixedTime;
			lastOnlineTracker.m_callback = delegate(object data)
			{
				if (newPlayer != null && !newPlayer.IsOnline())
				{
					this.AddToast(UserAttentionBlocker.NONE, newPlayer.GetBestName(), SocialToastMgr.TOAST_TYPE.FRIEND_OFFLINE, false);
				}
			};
			ApplicationMgr.Get().ScheduleCallback(5f, false, lastOnlineTracker.m_callback, null);
		}
	}

	// Token: 0x06003B62 RID: 15202 RVA: 0x00120214 File Offset: 0x0011E414
	private void CheckArenaGameStarted(BnetPlayer player)
	{
		if (PresenceMgr.Get().GetStatus(player) != PresenceStatus.ARENA_GAME)
		{
			return;
		}
		BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
		if (hearthstoneGameAccount == null)
		{
			return;
		}
		ArenaRecord arenaRecord;
		if (!ArenaRecord.TryParse(hearthstoneGameAccount.GetArenaRecord(), out arenaRecord))
		{
			return;
		}
		if (arenaRecord.wins >= 8)
		{
			this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ARENA_START_WITH_MANY_WINS", new object[]
			{
				"5ecaf0ff",
				player.GetBestName(),
				arenaRecord.wins
			}));
		}
	}

	// Token: 0x06003B63 RID: 15203 RVA: 0x001202A0 File Offset: 0x0011E4A0
	private void CheckArenaRecordChanged(BnetPlayer player)
	{
		BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
		if (hearthstoneGameAccount == null)
		{
			return;
		}
		ArenaRecord arenaRecord;
		if (!ArenaRecord.TryParse(hearthstoneGameAccount.GetArenaRecord(), out arenaRecord))
		{
			return;
		}
		if (arenaRecord.isFinished)
		{
			if (arenaRecord.wins >= 3)
			{
				this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ARENA_COMPLETE", new object[]
				{
					"5ecaf0ff",
					player.GetBestName(),
					arenaRecord.wins,
					arenaRecord.losses
				}));
			}
		}
		else if (arenaRecord.wins == 0 && arenaRecord.losses == 0)
		{
			this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ARENA_START", new object[]
			{
				"5ecaf0ff",
				player.GetBestName()
			}));
		}
	}

	// Token: 0x06003B64 RID: 15204 RVA: 0x00120378 File Offset: 0x0011E578
	private void CheckForCardOpened(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (newPlayerAccount.GetCardsOpened() == oldPlayerAccount.GetCardsOpened())
		{
			return;
		}
		string cardsOpened = newPlayerAccount.GetCardsOpened();
		if (string.IsNullOrEmpty(cardsOpened))
		{
			return;
		}
		string[] array = cardsOpened.Split(new char[]
		{
			','
		});
		if (array.Length != 2)
		{
			return;
		}
		EntityDef entityDef = DefLoader.Get().GetEntityDef(array[0]);
		if (entityDef == null)
		{
			return;
		}
		if (array[1] == "1")
		{
			this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_GOLDEN_LEGENDARY", new object[]
			{
				"5ecaf0ff",
				newPlayer.GetBestName(),
				entityDef.GetName(),
				"ffd200"
			}));
		}
		else
		{
			this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_LEGENDARY", new object[]
			{
				"5ecaf0ff",
				newPlayer.GetBestName(),
				entityDef.GetName(),
				"ff9c00"
			}));
		}
	}

	// Token: 0x06003B65 RID: 15205 RVA: 0x0012046C File Offset: 0x0011E66C
	private void CheckForNewRank(BnetPlayer player)
	{
		MedalInfoTranslator rankPresenceField = RankMgr.Get().GetRankPresenceField(player);
		if (rankPresenceField == null)
		{
			return;
		}
		BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
		int rank = rankPresenceField.GetCurrentMedal(false).rank;
		int rank2 = rankPresenceField.GetCurrentMedal(true).rank;
		if (!this.m_lastKnownMedals.ContainsKey(hearthstoneGameAccountId))
		{
			SocialToastMgr.LastKnownMedalTracker lastKnownMedalTracker = new SocialToastMgr.LastKnownMedalTracker();
			lastKnownMedalTracker.m_standardRank = rank;
			lastKnownMedalTracker.m_wildRank = rank2;
			this.m_lastKnownMedals[hearthstoneGameAccountId] = lastKnownMedalTracker;
			return;
		}
		SocialToastMgr.LastKnownMedalTracker lastKnownMedalTracker2 = this.m_lastKnownMedals[hearthstoneGameAccountId];
		if (rank <= 10 && rank < lastKnownMedalTracker2.m_standardRank)
		{
			lastKnownMedalTracker2.m_standardRank = rank;
			if (rank == 0)
			{
				this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_RANK_LEGEND", new object[]
				{
					"5ecaf0ff",
					player.GetBestName()
				}));
			}
			else
			{
				this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_GAINED_RANK", new object[]
				{
					"5ecaf0ff",
					player.GetBestName(),
					rankPresenceField.GetCurrentMedal(false).rank
				}));
			}
		}
		if (rank2 <= 10 && rank2 < lastKnownMedalTracker2.m_wildRank)
		{
			lastKnownMedalTracker2.m_wildRank = rank2;
			if (rank2 == 0)
			{
				this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_RANK_LEGEND_WILD", new object[]
				{
					"5ecaf0ff",
					player.GetBestName()
				}));
			}
			else
			{
				this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_GAINED_RANK_WILD", new object[]
				{
					"5ecaf0ff",
					player.GetBestName(),
					rankPresenceField.GetCurrentMedal(true).rank
				}));
			}
		}
	}

	// Token: 0x06003B66 RID: 15206 RVA: 0x0012060C File Offset: 0x0011E80C
	private void CheckForMissionComplete(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (newPlayerAccount.GetTutorialBeaten() == oldPlayerAccount.GetTutorialBeaten())
		{
			return;
		}
		if (newPlayerAccount.GetTutorialBeaten() != 1)
		{
			return;
		}
		this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ILLIDAN_COMPLETE", new object[]
		{
			"5ecaf0ff",
			newPlayer.GetBestName()
		}));
	}

	// Token: 0x06003B67 RID: 15207 RVA: 0x00120660 File Offset: 0x0011E860
	private void CheckForMageLevelChanged(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (!this.ShouldToastThisLevel(oldPlayerAccount.GetMageLevel(), newPlayerAccount.GetMageLevel()))
		{
			return;
		}
		this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_MAGE_LEVEL", new object[]
		{
			"5ecaf0ff",
			newPlayer.GetBestName(),
			newPlayerAccount.GetMageLevel()
		}));
	}

	// Token: 0x06003B68 RID: 15208 RVA: 0x001206BC File Offset: 0x0011E8BC
	private void CheckForPaladinLevelChanged(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (!this.ShouldToastThisLevel(oldPlayerAccount.GetPaladinLevel(), newPlayerAccount.GetPaladinLevel()))
		{
			return;
		}
		this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_PALADIN_LEVEL", new object[]
		{
			"5ecaf0ff",
			newPlayer.GetBestName(),
			newPlayerAccount.GetPaladinLevel()
		}));
	}

	// Token: 0x06003B69 RID: 15209 RVA: 0x00120718 File Offset: 0x0011E918
	private void CheckForDruidLevelChanged(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (!this.ShouldToastThisLevel(oldPlayerAccount.GetDruidLevel(), newPlayerAccount.GetDruidLevel()))
		{
			return;
		}
		this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_DRUID_LEVEL", new object[]
		{
			"5ecaf0ff",
			newPlayer.GetBestName(),
			newPlayerAccount.GetDruidLevel()
		}));
	}

	// Token: 0x06003B6A RID: 15210 RVA: 0x00120774 File Offset: 0x0011E974
	private void CheckForRogueLevelChanged(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (!this.ShouldToastThisLevel(oldPlayerAccount.GetRogueLevel(), newPlayerAccount.GetRogueLevel()))
		{
			return;
		}
		this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_ROGUE_LEVEL", new object[]
		{
			"5ecaf0ff",
			newPlayer.GetBestName(),
			newPlayerAccount.GetRogueLevel()
		}));
	}

	// Token: 0x06003B6B RID: 15211 RVA: 0x001207D0 File Offset: 0x0011E9D0
	private void CheckForHunterLevelChanged(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (!this.ShouldToastThisLevel(oldPlayerAccount.GetHunterLevel(), newPlayerAccount.GetHunterLevel()))
		{
			return;
		}
		this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_HUNTER_LEVEL", new object[]
		{
			"5ecaf0ff",
			newPlayer.GetBestName(),
			newPlayerAccount.GetHunterLevel()
		}));
	}

	// Token: 0x06003B6C RID: 15212 RVA: 0x0012082C File Offset: 0x0011EA2C
	private void CheckForShamanLevelChanged(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (!this.ShouldToastThisLevel(oldPlayerAccount.GetShamanLevel(), newPlayerAccount.GetShamanLevel()))
		{
			return;
		}
		this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_SHAMAN_LEVEL", new object[]
		{
			"5ecaf0ff",
			newPlayer.GetBestName(),
			newPlayerAccount.GetShamanLevel()
		}));
	}

	// Token: 0x06003B6D RID: 15213 RVA: 0x00120888 File Offset: 0x0011EA88
	private void CheckForWarriorLevelChanged(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (!this.ShouldToastThisLevel(oldPlayerAccount.GetWarriorLevel(), newPlayerAccount.GetWarriorLevel()))
		{
			return;
		}
		this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_WARRIOR_LEVEL", new object[]
		{
			"5ecaf0ff",
			newPlayer.GetBestName(),
			newPlayerAccount.GetWarriorLevel()
		}));
	}

	// Token: 0x06003B6E RID: 15214 RVA: 0x001208E4 File Offset: 0x0011EAE4
	private void CheckForWarlockLevelChanged(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (!this.ShouldToastThisLevel(oldPlayerAccount.GetWarlockLevel(), newPlayerAccount.GetWarlockLevel()))
		{
			return;
		}
		this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_WARLOCK_LEVEL", new object[]
		{
			"5ecaf0ff",
			newPlayer.GetBestName(),
			newPlayerAccount.GetWarlockLevel()
		}));
	}

	// Token: 0x06003B6F RID: 15215 RVA: 0x00120940 File Offset: 0x0011EB40
	private void CheckForPriestLevelChanged(BnetGameAccount oldPlayerAccount, BnetGameAccount newPlayerAccount, BnetPlayer newPlayer)
	{
		if (!this.ShouldToastThisLevel(oldPlayerAccount.GetPriestLevel(), newPlayerAccount.GetPriestLevel()))
		{
			return;
		}
		this.AddToast(UserAttentionBlocker.NONE, GameStrings.Format("GLOBAL_SOCIAL_TOAST_FRIEND_PRIEST_LEVEL", new object[]
		{
			"5ecaf0ff",
			newPlayer.GetBestName(),
			newPlayerAccount.GetPriestLevel()
		}));
	}

	// Token: 0x06003B70 RID: 15216 RVA: 0x0012099C File Offset: 0x0011EB9C
	private bool ShouldToastThisLevel(int oldLevel, int newLevel)
	{
		return oldLevel != newLevel && (newLevel == 20 || newLevel == 30 || newLevel == 40 || newLevel == 50 || newLevel == 60);
	}

	// Token: 0x06003B71 RID: 15217 RVA: 0x001209E0 File Offset: 0x0011EBE0
	private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
	{
		if (!DemoMgr.Get().IsSocialEnabled())
		{
			return;
		}
		List<BnetInvitation> addedReceivedInvites = changelist.GetAddedReceivedInvites();
		if (addedReceivedInvites == null)
		{
			return;
		}
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		if (myPlayer != null && myPlayer.IsBusy())
		{
			return;
		}
		foreach (BnetInvitation bnetInvitation in addedReceivedInvites)
		{
			BnetPlayer recentOpponent = FriendMgr.Get().GetRecentOpponent();
			if (recentOpponent != null && recentOpponent.HasAccount(bnetInvitation.GetInviterId()))
			{
				this.AddToast(UserAttentionBlocker.NONE, GameStrings.Get("GLOBAL_SOCIAL_TOAST_RECENT_OPPONENT_FRIEND_REQUEST"));
			}
			else
			{
				this.AddToast(UserAttentionBlocker.NONE, bnetInvitation.GetInviterName(), SocialToastMgr.TOAST_TYPE.FRIEND_INVITE);
			}
		}
	}

	// Token: 0x06003B72 RID: 15218 RVA: 0x00120AB4 File Offset: 0x0011ECB4
	private void ShutdownHandler(int minutes)
	{
		this.AddToast(UserAttentionBlocker.ALL, GameStrings.Format("GLOBAL_SHUTDOWN_TOAST", new object[]
		{
			"f61f1fff",
			minutes
		}), SocialToastMgr.TOAST_TYPE.DEFAULT, 3.5f);
	}

	// Token: 0x040025ED RID: 9709
	private const float FADE_IN_TIME = 0.25f;

	// Token: 0x040025EE RID: 9710
	private const float FADE_OUT_TIME = 0.5f;

	// Token: 0x040025EF RID: 9711
	private const float HOLD_TIME = 2f;

	// Token: 0x040025F0 RID: 9712
	private const float SHUTDOWN_MESSAGE_TIME = 3.5f;

	// Token: 0x040025F1 RID: 9713
	private const float OFFLINE_TOAST_DELAY = 5f;

	// Token: 0x040025F2 RID: 9714
	public SocialToast m_socialToastPrefab;

	// Token: 0x040025F3 RID: 9715
	private static SocialToastMgr s_instance;

	// Token: 0x040025F4 RID: 9716
	private SocialToast m_toast;

	// Token: 0x040025F5 RID: 9717
	private bool m_toastIsShown;

	// Token: 0x040025F6 RID: 9718
	private PlatformDependentValue<Vector3> TOAST_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(235f, 1f, 235f),
		Phone = new Vector3(470f, 1f, 470f)
	};

	// Token: 0x040025F7 RID: 9719
	private Map<BnetGameAccountId, SocialToastMgr.LastKnownMedalTracker> m_lastKnownMedals = new Map<BnetGameAccountId, SocialToastMgr.LastKnownMedalTracker>();

	// Token: 0x040025F8 RID: 9720
	private Map<int, SocialToastMgr.LastOnlineTracker> m_lastOnlineTracker = new Map<int, SocialToastMgr.LastOnlineTracker>();

	// Token: 0x020004F4 RID: 1268
	public enum TOAST_TYPE
	{
		// Token: 0x040025FA RID: 9722
		DEFAULT,
		// Token: 0x040025FB RID: 9723
		FRIEND_ONLINE,
		// Token: 0x040025FC RID: 9724
		FRIEND_OFFLINE,
		// Token: 0x040025FD RID: 9725
		FRIEND_INVITE,
		// Token: 0x040025FE RID: 9726
		HEALTHY_GAMING,
		// Token: 0x040025FF RID: 9727
		HEALTHY_GAMING_OVER_THRESHOLD,
		// Token: 0x04002600 RID: 9728
		FRIEND_ARENA_COMPLETE,
		// Token: 0x04002601 RID: 9729
		SPECTATOR_INVITE_SENT,
		// Token: 0x04002602 RID: 9730
		SPECTATOR_INVITE_RECEIVED,
		// Token: 0x04002603 RID: 9731
		SPECTATOR_ADDED,
		// Token: 0x04002604 RID: 9732
		SPECTATOR_REMOVED
	}

	// Token: 0x02000652 RID: 1618
	private class LastKnownMedalTracker
	{
		// Token: 0x04002C69 RID: 11369
		public int m_standardRank;

		// Token: 0x04002C6A RID: 11370
		public int m_wildRank;
	}

	// Token: 0x02000653 RID: 1619
	private class LastOnlineTracker
	{
		// Token: 0x04002C6B RID: 11371
		public float m_localLastOnlineTime;

		// Token: 0x04002C6C RID: 11372
		public ApplicationMgr.ScheduledCallback m_callback;
	}
}
