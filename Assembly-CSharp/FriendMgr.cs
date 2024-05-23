using System;
using System.Collections.Generic;
using bgs;

// Token: 0x020004D6 RID: 1238
public class FriendMgr
{
	// Token: 0x06003A5F RID: 14943 RVA: 0x0011A8E1 File Offset: 0x00118AE1
	public static FriendMgr Get()
	{
		if (FriendMgr.s_instance == null)
		{
			FriendMgr.s_instance = new FriendMgr();
			FriendMgr.s_instance.Initialize();
		}
		return FriendMgr.s_instance;
	}

	// Token: 0x06003A60 RID: 14944 RVA: 0x0011A906 File Offset: 0x00118B06
	public BnetPlayer GetSelectedFriend()
	{
		return this.m_selectedFriend;
	}

	// Token: 0x06003A61 RID: 14945 RVA: 0x0011A90E File Offset: 0x00118B0E
	public void SetSelectedFriend(BnetPlayer friend)
	{
		this.m_selectedFriend = friend;
	}

	// Token: 0x06003A62 RID: 14946 RVA: 0x0011A917 File Offset: 0x00118B17
	public bool IsFriendListScrollEnabled()
	{
		return this.m_friendListScrollEnabled;
	}

	// Token: 0x06003A63 RID: 14947 RVA: 0x0011A91F File Offset: 0x00118B1F
	public void SetFriendListScrollEnabled(bool enabled)
	{
		this.m_friendListScrollEnabled = enabled;
	}

	// Token: 0x06003A64 RID: 14948 RVA: 0x0011A928 File Offset: 0x00118B28
	public float GetFriendListScrollCamPosY()
	{
		return this.m_friendListScrollCamPosY;
	}

	// Token: 0x06003A65 RID: 14949 RVA: 0x0011A930 File Offset: 0x00118B30
	public void SetFriendListScrollCamPosY(float y)
	{
		this.m_friendListScrollCamPosY = y;
	}

	// Token: 0x06003A66 RID: 14950 RVA: 0x0011A939 File Offset: 0x00118B39
	public BnetPlayer GetRecentOpponent()
	{
		return this.m_recentOpponent;
	}

	// Token: 0x06003A67 RID: 14951 RVA: 0x0011A944 File Offset: 0x00118B44
	private void UpdateRecentOpponent()
	{
		if (SpectatorManager.Get().IsInSpectatorMode())
		{
			return;
		}
		if (GameState.Get() == null)
		{
			return;
		}
		Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
		if (opposingSidePlayer == null)
		{
			return;
		}
		BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(opposingSidePlayer.GetGameAccountId());
		if (player == null)
		{
			return;
		}
		if (this.m_recentOpponent == player)
		{
			return;
		}
		this.m_recentOpponent = player;
		this.FireRecentOpponentEvent(this.m_recentOpponent);
	}

	// Token: 0x06003A68 RID: 14952 RVA: 0x0011A9B8 File Offset: 0x00118BB8
	public void AddRecentOpponentListener(FriendMgr.RecentOpponentCallback callback)
	{
		FriendMgr.RecentOpponentListener recentOpponentListener = new FriendMgr.RecentOpponentListener();
		recentOpponentListener.SetCallback(callback);
		recentOpponentListener.SetUserData(null);
		if (this.m_recentOpponentListeners.Contains(recentOpponentListener))
		{
			return;
		}
		this.m_recentOpponentListeners.Add(recentOpponentListener);
	}

	// Token: 0x06003A69 RID: 14953 RVA: 0x0011A9F8 File Offset: 0x00118BF8
	public bool RemoveRecentOpponentListener(FriendMgr.RecentOpponentCallback callback)
	{
		FriendMgr.RecentOpponentListener recentOpponentListener = new FriendMgr.RecentOpponentListener();
		recentOpponentListener.SetCallback(callback);
		recentOpponentListener.SetUserData(null);
		return this.m_recentOpponentListeners.Remove(recentOpponentListener);
	}

	// Token: 0x06003A6A RID: 14954 RVA: 0x0011AA28 File Offset: 0x00118C28
	public void FireRecentOpponentEvent(BnetPlayer recentOpponent)
	{
		foreach (FriendMgr.RecentOpponentListener recentOpponentListener in this.m_recentOpponentListeners.ToArray())
		{
			recentOpponentListener.Fire(recentOpponent);
		}
	}

	// Token: 0x06003A6B RID: 14955 RVA: 0x0011AA60 File Offset: 0x00118C60
	private void Initialize()
	{
		BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
		Network.Get().AddBnetErrorListener(1, new Network.BnetErrorCallback(this.OnBnetError));
		BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
	}

	// Token: 0x06003A6C RID: 14956 RVA: 0x0011AAC8 File Offset: 0x00118CC8
	private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
	{
		List<BnetPlayer> removedFriends = changelist.GetRemovedFriends();
		if (removedFriends == null)
		{
			return;
		}
		if (removedFriends.Contains(this.m_selectedFriend))
		{
			this.m_selectedFriend = null;
		}
	}

	// Token: 0x06003A6D RID: 14957 RVA: 0x0011AAFC File Offset: 0x00118CFC
	private bool OnBnetError(BnetErrorInfo info, object userData)
	{
		BnetFeature feature = info.GetFeature();
		BnetFeatureEvent featureEvent = info.GetFeatureEvent();
		if (feature == 1 && featureEvent == 9)
		{
			BattleNetErrors error = info.GetError();
			if (error == 0)
			{
				string message = GameStrings.Get("GLOBAL_ADDFRIEND_SENT_CONFIRMATION");
				UIStatus.Get().AddInfo(message);
				return true;
			}
			if (error == 5003)
			{
				string message = GameStrings.Get("GLOBAL_ADDFRIEND_ERROR_ALREADY_FRIEND");
				UIStatus.Get().AddError(message);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003A6E RID: 14958 RVA: 0x0011AB7C File Offset: 0x00118D7C
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		BnetPlayerChange bnetPlayerChange = changelist.FindChange(this.m_selectedFriend);
		if (bnetPlayerChange == null)
		{
			return;
		}
		BnetPlayer oldPlayer = bnetPlayerChange.GetOldPlayer();
		BnetPlayer newPlayer = bnetPlayerChange.GetNewPlayer();
		if (oldPlayer == null || oldPlayer.IsOnline() != newPlayer.IsOnline())
		{
			this.m_selectedFriend = null;
		}
	}

	// Token: 0x06003A6F RID: 14959 RVA: 0x0011ABC9 File Offset: 0x00118DC9
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		if (mode == SceneMgr.Mode.GAMEPLAY)
		{
			GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver), null);
		}
	}

	// Token: 0x06003A70 RID: 14960 RVA: 0x0011ABEA File Offset: 0x00118DEA
	private void OnGameOver(object userData)
	{
		GameState.Get().UnregisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver), null);
		this.UpdateRecentOpponent();
	}

	// Token: 0x04002546 RID: 9542
	private static FriendMgr s_instance;

	// Token: 0x04002547 RID: 9543
	private BnetPlayer m_selectedFriend;

	// Token: 0x04002548 RID: 9544
	private BnetPlayer m_recentOpponent;

	// Token: 0x04002549 RID: 9545
	private bool m_friendListScrollEnabled;

	// Token: 0x0400254A RID: 9546
	private float m_friendListScrollCamPosY;

	// Token: 0x0400254B RID: 9547
	private List<FriendMgr.RecentOpponentListener> m_recentOpponentListeners = new List<FriendMgr.RecentOpponentListener>();

	// Token: 0x0200058B RID: 1419
	// (Invoke) Token: 0x0600406B RID: 16491
	public delegate void RecentOpponentCallback(BnetPlayer recentOpponent, object userData);

	// Token: 0x02000648 RID: 1608
	private class RecentOpponentListener : EventListener<FriendMgr.RecentOpponentCallback>
	{
		// Token: 0x06004557 RID: 17751 RVA: 0x0014D15C File Offset: 0x0014B35C
		public void Fire(BnetPlayer recentOpponent)
		{
			this.m_callback(recentOpponent, this.m_userData);
		}
	}
}
