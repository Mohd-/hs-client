using System;
using bgs;
using UnityEngine;

// Token: 0x02000575 RID: 1397
public class FriendListBaseFriendFrame : MonoBehaviour
{
	// Token: 0x06003FDB RID: 16347 RVA: 0x00135BF8 File Offset: 0x00133DF8
	protected virtual void Awake()
	{
		BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		BnetWhisperMgr.Get().AddWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
		ChatMgr.Get().AddPlayerChatInfoChangedListener(new ChatMgr.PlayerChatInfoChangedCallback(this.OnPlayerChatInfoChanged));
		RecruitListMgr.Get().AddRecruitsChangedListener(new RecruitListMgr.RecruitsChangedCallback(this.OnRecruitsChanged));
		this.m_RecruitUI = Object.Instantiate<FriendListRecruitUI>(this.m_Prefabs.recruitUI);
		this.m_RecruitUI.transform.parent = base.gameObject.transform;
		this.m_RecruitUI.gameObject.SetActive(false);
		if (this.m_rankMedalSpawner == null)
		{
			this.m_rankMedal = Object.Instantiate<TournamentMedal>(this.m_rankMedalPrefab);
			this.m_rankMedal.transform.parent = base.transform;
			this.m_rankMedal.transform.localScale = new Vector3(20f, 1f, 20f);
			this.m_rankMedal.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
		}
		else
		{
			this.m_rankMedal = this.m_rankMedalSpawner.Spawn<TournamentMedal>();
		}
		this.m_rankMedal.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.m_rankMedal.MedalOver));
		this.m_rankMedal.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.RankMedalOver));
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_rankMedal.GetComponent<Collider>().enabled = false;
		}
		this.m_rankMedal.gameObject.SetActive(false);
		SceneUtils.SetLayer(this.m_rankMedal, GameLayer.BattleNetFriendList);
	}

	// Token: 0x06003FDC RID: 16348 RVA: 0x00135DB4 File Offset: 0x00133FB4
	protected virtual void OnDestroy()
	{
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		BnetWhisperMgr.Get().RemoveWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
		RecruitListMgr.Get().RemoveRecruitsChangedListener(new RecruitListMgr.RecruitsChangedCallback(this.OnRecruitsChanged));
		if (ChatMgr.Get() != null)
		{
			ChatMgr.Get().RemovePlayerChatInfoChangedListener(new ChatMgr.PlayerChatInfoChangedCallback(this.OnPlayerChatInfoChanged));
		}
	}

	// Token: 0x06003FDD RID: 16349 RVA: 0x00135E2D File Offset: 0x0013402D
	public BnetPlayer GetFriend()
	{
		return this.m_player;
	}

	// Token: 0x06003FDE RID: 16350 RVA: 0x00135E35 File Offset: 0x00134035
	public Network.RecruitInfo GetRecruitInfo()
	{
		return this.m_recruitInfo;
	}

	// Token: 0x06003FDF RID: 16351 RVA: 0x00135E3D File Offset: 0x0013403D
	public virtual bool SetFriend(BnetPlayer player)
	{
		if (this.m_player == player)
		{
			return false;
		}
		this.m_player = player;
		this.m_PlayerIcon.SetPlayer(player);
		this.m_ChatIcon.SetPlayer(player);
		this.UpdateFriend();
		return true;
	}

	// Token: 0x06003FE0 RID: 16352 RVA: 0x00135E78 File Offset: 0x00134078
	public virtual void UpdateFriend()
	{
		this.m_ChatIcon.UpdateIcon();
		if (this.m_player == null)
		{
			return;
		}
		Color textColor;
		if (this.m_player.IsOnline())
		{
			if (this.m_player.IsAway())
			{
				textColor = FriendListBaseFriendFrame.TEXT_COLOR_AWAY;
			}
			else if (this.m_player.IsBusy())
			{
				textColor = FriendListBaseFriendFrame.TEXT_COLOR_BUSY;
			}
			else
			{
				textColor = FriendListBaseFriendFrame.TEXT_COLOR_NORMAL;
			}
		}
		else
		{
			textColor = FriendListBaseFriendFrame.TEXT_COLOR_OFFLINE;
		}
		this.m_StatusText.TextColor = textColor;
		BnetGameAccount hearthstoneGameAccount = this.m_player.GetHearthstoneGameAccount();
		this.m_medal = ((!(hearthstoneGameAccount == null)) ? RankMgr.Get().GetRankPresenceField(hearthstoneGameAccount) : null);
		if (this.m_medal == null || this.m_medal.GetCurrentMedal(this.m_medal.IsBestCurrentRankWild()).rank == 25)
		{
			this.m_rankMedal.gameObject.SetActive(false);
			this.m_PlayerIcon.Show();
			return;
		}
		this.m_PlayerIcon.Hide();
		this.m_rankMedal.gameObject.SetActive(true);
		this.m_rankMedal.SetMedal(this.m_medal, false);
		this.m_rankMedal.SetFormat(this.m_rankMedal.IsBestCurrentRankWild());
	}

	// Token: 0x06003FE1 RID: 16353 RVA: 0x00135FBC File Offset: 0x001341BC
	private void RankMedalOver(UIEvent e)
	{
		string name = this.m_medal.GetCurrentMedal(this.m_medal.IsBestCurrentRankWild()).name;
		KeywordHelpPanel keywordHelpPanel = this.m_rankMedal.GetComponent<TooltipZone>().ShowTooltip(name, string.Empty, 0.7f, true);
		SceneUtils.SetLayer(keywordHelpPanel.gameObject, GameLayer.BattleNetFriendList);
		keywordHelpPanel.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		keywordHelpPanel.transform.localScale = new Vector3(3f, 3f, 3f);
		TransformUtil.SetPoint(keywordHelpPanel, Anchor.LEFT, this.m_rankMedal.gameObject, Anchor.RIGHT, new Vector3(1f, 0f, 0f));
	}

	// Token: 0x06003FE2 RID: 16354 RVA: 0x00136074 File Offset: 0x00134274
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		if (!changelist.HasChange(this.m_player))
		{
			return;
		}
		this.UpdateFriend();
	}

	// Token: 0x06003FE3 RID: 16355 RVA: 0x00136090 File Offset: 0x00134290
	private void OnRecruitsChanged()
	{
		if (this.m_player != null)
		{
			Network.RecruitInfo recruitInfoFromAccountId = RecruitListMgr.Get().GetRecruitInfoFromAccountId(this.m_player.GetAccountId());
			if (recruitInfoFromAccountId != null)
			{
				this.UpdateFriend();
			}
		}
	}

	// Token: 0x06003FE4 RID: 16356 RVA: 0x001360CC File Offset: 0x001342CC
	private void OnWhisper(BnetWhisper whisper, object userData)
	{
		if (this.m_player == null)
		{
			return;
		}
		if (!WhisperUtil.IsSpeakerOrReceiver(this.m_player, whisper))
		{
			return;
		}
		this.UpdateFriend();
	}

	// Token: 0x06003FE5 RID: 16357 RVA: 0x001360FD File Offset: 0x001342FD
	private void OnPlayerChatInfoChanged(PlayerChatInfo chatInfo, object userData)
	{
		if (this.m_player != chatInfo.GetPlayer())
		{
			return;
		}
		this.UpdateFriend();
	}

	// Token: 0x06003FE6 RID: 16358 RVA: 0x00136118 File Offset: 0x00134318
	protected void UpdateOnlineStatus()
	{
		if (this.m_player.IsAway())
		{
			this.m_StatusText.Text = FriendUtils.GetAwayTimeString(this.m_player.GetBestAwayTimeMicrosec());
			return;
		}
		if (this.m_player.IsBusy())
		{
			this.m_StatusText.Text = GameStrings.Get("GLOBAL_FRIENDLIST_BUSYSTATUS");
			return;
		}
		string statusText = PresenceMgr.Get().GetStatusText(this.m_player);
		if (statusText != null)
		{
			this.m_StatusText.Text = statusText;
			return;
		}
		BnetProgramId bestProgramId = this.m_player.GetBestProgramId();
		if (bestProgramId != null)
		{
			this.m_StatusText.Text = BnetUtils.GetNameForProgramId(bestProgramId);
			return;
		}
		this.m_StatusText.Text = string.Empty;
	}

	// Token: 0x06003FE7 RID: 16359 RVA: 0x001361D8 File Offset: 0x001343D8
	protected void UpdateOfflineStatus()
	{
		ulong bestLastOnlineMicrosec = this.m_player.GetBestLastOnlineMicrosec();
		this.m_StatusText.Text = FriendUtils.GetLastOnlineElapsedTimeString(bestLastOnlineMicrosec);
	}

	// Token: 0x040028DE RID: 10462
	private static readonly Color TEXT_COLOR_NORMAL = Color.white;

	// Token: 0x040028DF RID: 10463
	private static readonly Color TEXT_COLOR_AWAY = Color.yellow;

	// Token: 0x040028E0 RID: 10464
	private static readonly Color TEXT_COLOR_BUSY = Color.red;

	// Token: 0x040028E1 RID: 10465
	private static readonly Color TEXT_COLOR_OFFLINE = Color.grey;

	// Token: 0x040028E2 RID: 10466
	public PlayerIcon m_PlayerIcon;

	// Token: 0x040028E3 RID: 10467
	public UberText m_PlayerNameText;

	// Token: 0x040028E4 RID: 10468
	public UberText m_StatusText;

	// Token: 0x040028E5 RID: 10469
	public FriendListFrameBasePrefabs m_Prefabs;

	// Token: 0x040028E6 RID: 10470
	public FriendListChatIcon m_ChatIcon;

	// Token: 0x040028E7 RID: 10471
	public TournamentMedal m_rankMedalPrefab;

	// Token: 0x040028E8 RID: 10472
	public Spawner m_rankMedalSpawner;

	// Token: 0x040028E9 RID: 10473
	public TournamentMedal m_rankMedal;

	// Token: 0x040028EA RID: 10474
	protected BnetPlayer m_player;

	// Token: 0x040028EB RID: 10475
	protected MedalInfoTranslator m_medal;

	// Token: 0x040028EC RID: 10476
	protected Network.RecruitInfo m_recruitInfo;

	// Token: 0x040028ED RID: 10477
	protected FriendListRecruitUI m_RecruitUI;
}
