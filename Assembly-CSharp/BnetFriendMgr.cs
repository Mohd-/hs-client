using System;
using System.Collections.Generic;
using bgs;
using bgs.types;

// Token: 0x020004AA RID: 1194
public class BnetFriendMgr
{
	// Token: 0x060038F4 RID: 14580 RVA: 0x00116093 File Offset: 0x00114293
	public static BnetFriendMgr Get()
	{
		if (BnetFriendMgr.s_instance == null)
		{
			BnetFriendMgr.s_instance = new BnetFriendMgr();
			ApplicationMgr.Get().WillReset += new Action(BnetFriendMgr.s_instance.Clear);
		}
		return BnetFriendMgr.s_instance;
	}

	// Token: 0x060038F5 RID: 14581 RVA: 0x001160C8 File Offset: 0x001142C8
	public void Initialize()
	{
		FriendMgr.Get();
		BnetEventMgr.Get().AddChangeListener(new BnetEventMgr.ChangeCallback(this.OnBnetEventOccurred));
		Network.Get().SetFriendsHandler(new Network.FriendsHandler(this.OnFriendsUpdate));
		Network.Get().AddBnetErrorListener(1, new Network.BnetErrorCallback(this.OnBnetError));
		this.InitMaximums();
	}

	// Token: 0x060038F6 RID: 14582 RVA: 0x00116125 File Offset: 0x00114325
	public void Shutdown()
	{
		Network.Get().RemoveBnetErrorListener(1, new Network.BnetErrorCallback(this.OnBnetError));
		Network.Get().SetFriendsHandler(null);
	}

	// Token: 0x060038F7 RID: 14583 RVA: 0x0011614A File Offset: 0x0011434A
	public int GetMaxFriends()
	{
		return this.m_maxFriends;
	}

	// Token: 0x060038F8 RID: 14584 RVA: 0x00116152 File Offset: 0x00114352
	public int GetMaxReceivedInvites()
	{
		return this.m_maxReceivedInvites;
	}

	// Token: 0x060038F9 RID: 14585 RVA: 0x0011615A File Offset: 0x0011435A
	public int GetMaxSentInvites()
	{
		return this.m_maxSentInvites;
	}

	// Token: 0x060038FA RID: 14586 RVA: 0x00116164 File Offset: 0x00114364
	public BnetPlayer FindFriend(BnetAccountId id)
	{
		BnetPlayer bnetPlayer = this.FindNonPendingFriend(id);
		if (bnetPlayer != null)
		{
			return bnetPlayer;
		}
		bnetPlayer = this.FindPendingFriend(id);
		if (bnetPlayer != null)
		{
			return bnetPlayer;
		}
		return null;
	}

	// Token: 0x060038FB RID: 14587 RVA: 0x00116194 File Offset: 0x00114394
	public BnetPlayer FindFriend(BnetGameAccountId id)
	{
		BnetPlayer bnetPlayer = this.FindNonPendingFriend(id);
		if (bnetPlayer != null)
		{
			return bnetPlayer;
		}
		bnetPlayer = this.FindPendingFriend(id);
		if (bnetPlayer != null)
		{
			return bnetPlayer;
		}
		return null;
	}

	// Token: 0x060038FC RID: 14588 RVA: 0x001161C2 File Offset: 0x001143C2
	public bool IsFriend(BnetPlayer player)
	{
		return this.IsNonPendingFriend(player) || this.IsPendingFriend(player);
	}

	// Token: 0x060038FD RID: 14589 RVA: 0x001161E1 File Offset: 0x001143E1
	public bool IsFriend(BnetAccountId id)
	{
		return this.IsNonPendingFriend(id) || this.IsPendingFriend(id);
	}

	// Token: 0x060038FE RID: 14590 RVA: 0x00116200 File Offset: 0x00114400
	public bool IsFriend(BnetGameAccountId id)
	{
		return this.IsNonPendingFriend(id) || this.IsPendingFriend(id);
	}

	// Token: 0x060038FF RID: 14591 RVA: 0x0011621F File Offset: 0x0011441F
	public List<BnetPlayer> GetFriends()
	{
		return this.m_friends;
	}

	// Token: 0x06003900 RID: 14592 RVA: 0x00116227 File Offset: 0x00114427
	public int GetFriendCount()
	{
		return this.m_friends.Count;
	}

	// Token: 0x06003901 RID: 14593 RVA: 0x00116234 File Offset: 0x00114434
	public bool HasOnlineFriends()
	{
		foreach (BnetPlayer bnetPlayer in this.m_friends)
		{
			if (bnetPlayer.IsOnline())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003902 RID: 14594 RVA: 0x0011629C File Offset: 0x0011449C
	public int GetOnlineFriendCount()
	{
		int num = 0;
		foreach (BnetPlayer bnetPlayer in this.m_friends)
		{
			if (bnetPlayer.IsOnline())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06003903 RID: 14595 RVA: 0x00116304 File Offset: 0x00114504
	public int GetActiveOnlineFriendCount()
	{
		int num = 0;
		foreach (BnetPlayer bnetPlayer in this.m_friends)
		{
			if (bnetPlayer.IsOnline())
			{
				if (!(bnetPlayer.GetBestProgramId() == null))
				{
					if (bnetPlayer.GetBestProgramId().IsPhoenix())
					{
						ulong bestAwayTimeMicrosec = bnetPlayer.GetBestAwayTimeMicrosec();
						if (bestAwayTimeMicrosec > 0UL)
						{
							continue;
						}
						if (bnetPlayer.IsBusy())
						{
							continue;
						}
					}
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06003904 RID: 14596 RVA: 0x001163B8 File Offset: 0x001145B8
	public BnetPlayer FindNonPendingFriend(BnetAccountId id)
	{
		foreach (BnetPlayer bnetPlayer in this.m_friends)
		{
			if (bnetPlayer.GetAccountId() == id)
			{
				return bnetPlayer;
			}
		}
		return null;
	}

	// Token: 0x06003905 RID: 14597 RVA: 0x00116428 File Offset: 0x00114628
	public BnetPlayer FindNonPendingFriend(BnetGameAccountId id)
	{
		foreach (BnetPlayer bnetPlayer in this.m_friends)
		{
			if (bnetPlayer.HasGameAccount(id))
			{
				return bnetPlayer;
			}
		}
		return null;
	}

	// Token: 0x06003906 RID: 14598 RVA: 0x00116494 File Offset: 0x00114694
	public bool IsNonPendingFriend(BnetPlayer player)
	{
		if (player == null)
		{
			return false;
		}
		if (this.m_friends.Contains(player))
		{
			return true;
		}
		BnetAccountId accountId = player.GetAccountId();
		if (accountId != null)
		{
			return this.IsFriend(accountId);
		}
		foreach (BnetGameAccountId id in player.GetGameAccounts().Keys)
		{
			if (this.IsFriend(id))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003907 RID: 14599 RVA: 0x00116538 File Offset: 0x00114738
	public bool IsNonPendingFriend(BnetAccountId id)
	{
		return this.FindNonPendingFriend(id) != null;
	}

	// Token: 0x06003908 RID: 14600 RVA: 0x00116547 File Offset: 0x00114747
	public bool IsNonPendingFriend(BnetGameAccountId id)
	{
		return this.FindNonPendingFriend(id) != null;
	}

	// Token: 0x06003909 RID: 14601 RVA: 0x00116556 File Offset: 0x00114756
	public BnetPlayer FindPendingFriend(BnetAccountId id)
	{
		return this.m_pendingChangelist.FindFriend(id);
	}

	// Token: 0x0600390A RID: 14602 RVA: 0x00116564 File Offset: 0x00114764
	public BnetPlayer FindPendingFriend(BnetGameAccountId id)
	{
		return this.m_pendingChangelist.FindFriend(id);
	}

	// Token: 0x0600390B RID: 14603 RVA: 0x00116572 File Offset: 0x00114772
	public bool IsPendingFriend(BnetPlayer player)
	{
		return this.m_pendingChangelist.IsFriend(player);
	}

	// Token: 0x0600390C RID: 14604 RVA: 0x00116580 File Offset: 0x00114780
	public bool IsPendingFriend(BnetAccountId id)
	{
		return this.m_pendingChangelist.IsFriend(id);
	}

	// Token: 0x0600390D RID: 14605 RVA: 0x0011658E File Offset: 0x0011478E
	public bool IsPendingFriend(BnetGameAccountId id)
	{
		return this.m_pendingChangelist.IsFriend(id);
	}

	// Token: 0x0600390E RID: 14606 RVA: 0x0011659C File Offset: 0x0011479C
	public List<BnetPlayer> GetPendingFriends()
	{
		return this.m_pendingChangelist.GetFriends();
	}

	// Token: 0x0600390F RID: 14607 RVA: 0x001165A9 File Offset: 0x001147A9
	public List<BnetInvitation> GetReceivedInvites()
	{
		return this.m_receivedInvites;
	}

	// Token: 0x06003910 RID: 14608 RVA: 0x001165B1 File Offset: 0x001147B1
	public List<BnetInvitation> GetSentInvites()
	{
		return this.m_sentInvites;
	}

	// Token: 0x06003911 RID: 14609 RVA: 0x001165B9 File Offset: 0x001147B9
	public void AcceptInvite(BnetInvitationId inviteId)
	{
		Network.AcceptFriendInvite(inviteId);
	}

	// Token: 0x06003912 RID: 14610 RVA: 0x001165C1 File Offset: 0x001147C1
	public void DeclineInvite(BnetInvitationId inviteId)
	{
		Network.DeclineFriendInvite(inviteId);
	}

	// Token: 0x06003913 RID: 14611 RVA: 0x001165C9 File Offset: 0x001147C9
	public void IgnoreInvite(BnetInvitationId inviteId)
	{
		Network.IgnoreFriendInvite(inviteId);
	}

	// Token: 0x06003914 RID: 14612 RVA: 0x001165D1 File Offset: 0x001147D1
	public void RevokeInvite(BnetInvitationId inviteId)
	{
		Network.RevokeFriendInvite(inviteId);
	}

	// Token: 0x06003915 RID: 14613 RVA: 0x001165D9 File Offset: 0x001147D9
	public void SendInvite(string name)
	{
		if (name.Contains("@"))
		{
			this.SendInviteByEmail(name);
		}
		else
		{
			this.SendInviteByBattleTag(name);
		}
	}

	// Token: 0x06003916 RID: 14614 RVA: 0x00116600 File Offset: 0x00114800
	public void SendInviteByEmail(string email)
	{
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		Network.SendFriendInviteByEmail(myPlayer.GetFullName(), email);
	}

	// Token: 0x06003917 RID: 14615 RVA: 0x00116624 File Offset: 0x00114824
	public void SendInviteByBattleTag(string battleTagString)
	{
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		Network.SendFriendInviteByBattleTag(myPlayer.GetBattleTag().GetString(), battleTagString);
	}

	// Token: 0x06003918 RID: 14616 RVA: 0x0011664D File Offset: 0x0011484D
	public bool RemoveFriend(BnetPlayer friend)
	{
		if (!this.m_friends.Contains(friend))
		{
			return false;
		}
		Network.RemoveFriend(friend.GetAccountId());
		return true;
	}

	// Token: 0x06003919 RID: 14617 RVA: 0x0011666E File Offset: 0x0011486E
	public bool AddChangeListener(BnetFriendMgr.ChangeCallback callback)
	{
		return this.AddChangeListener(callback, null);
	}

	// Token: 0x0600391A RID: 14618 RVA: 0x00116678 File Offset: 0x00114878
	public bool AddChangeListener(BnetFriendMgr.ChangeCallback callback, object userData)
	{
		BnetFriendMgr.ChangeListener changeListener = new BnetFriendMgr.ChangeListener();
		changeListener.SetCallback(callback);
		changeListener.SetUserData(userData);
		if (this.m_changeListeners.Contains(changeListener))
		{
			return false;
		}
		this.m_changeListeners.Add(changeListener);
		return true;
	}

	// Token: 0x0600391B RID: 14619 RVA: 0x001166B9 File Offset: 0x001148B9
	public bool RemoveChangeListener(BnetFriendMgr.ChangeCallback callback)
	{
		return this.RemoveChangeListener(callback, null);
	}

	// Token: 0x0600391C RID: 14620 RVA: 0x001166C4 File Offset: 0x001148C4
	public bool RemoveChangeListener(BnetFriendMgr.ChangeCallback callback, object userData)
	{
		BnetFriendMgr.ChangeListener changeListener = new BnetFriendMgr.ChangeListener();
		changeListener.SetCallback(callback);
		changeListener.SetUserData(userData);
		return this.m_changeListeners.Remove(changeListener);
	}

	// Token: 0x0600391D RID: 14621 RVA: 0x001166F4 File Offset: 0x001148F4
	private void InitMaximums()
	{
		FriendsInfo friendsInfo = default(FriendsInfo);
		BattleNet.GetFriendsInfo(ref friendsInfo);
		this.m_maxFriends = friendsInfo.maxFriends;
		this.m_maxReceivedInvites = friendsInfo.maxRecvInvites;
		this.m_maxSentInvites = friendsInfo.maxSentInvites;
	}

	// Token: 0x0600391E RID: 14622 RVA: 0x00116738 File Offset: 0x00114938
	private void ProcessPendingFriends()
	{
		bool flag = false;
		foreach (BnetPlayer bnetPlayer in this.m_pendingChangelist.GetFriends())
		{
			if (bnetPlayer.IsDisplayable())
			{
				flag = true;
				this.m_friends.Add(bnetPlayer);
			}
		}
		if (flag)
		{
			this.FirePendingFriendsChangedEvent();
		}
	}

	// Token: 0x0600391F RID: 14623 RVA: 0x001167B8 File Offset: 0x001149B8
	private void OnBnetEventOccurred(BattleNet.BnetEvent bnetEvent, object userData)
	{
		if (bnetEvent == null)
		{
			this.Clear();
		}
	}

	// Token: 0x06003920 RID: 14624 RVA: 0x001167C8 File Offset: 0x001149C8
	private void OnFriendsUpdate(FriendsUpdate[] updates)
	{
		BnetFriendChangelist bnetFriendChangelist = new BnetFriendChangelist();
		foreach (FriendsUpdate src in updates)
		{
			FriendsUpdate.Action action = src.action;
			if (action == 1)
			{
				BnetAccountId id = BnetAccountId.CreateFromBnetEntityId(src.entity1);
				BnetPlayer bnetPlayer = BnetPresenceMgr.Get().RegisterPlayer(id);
				if (bnetPlayer.IsDisplayable())
				{
					this.m_friends.Add(bnetPlayer);
					bnetFriendChangelist.AddAddedFriend(bnetPlayer);
				}
				else
				{
					this.AddPendingFriend(bnetPlayer);
				}
			}
			else if (action == 2)
			{
				BnetAccountId id2 = BnetAccountId.CreateFromBnetEntityId(src.entity1);
				BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(id2);
				this.m_friends.Remove(player);
				bnetFriendChangelist.AddRemovedFriend(player);
				this.RemovePendingFriend(player);
			}
			else if (action != 8)
			{
				if (action != 7)
				{
					if (action != 9)
					{
						if (action != 10)
						{
							if (action == 3)
							{
								BnetInvitation bnetInvitation = BnetInvitation.CreateFromFriendsUpdate(src);
								this.m_receivedInvites.Add(bnetInvitation);
								bnetFriendChangelist.AddAddedReceivedInvite(bnetInvitation);
							}
							else if (action == 4)
							{
								BnetInvitation bnetInvitation2 = BnetInvitation.CreateFromFriendsUpdate(src);
								this.m_receivedInvites.Remove(bnetInvitation2);
								bnetFriendChangelist.AddRemovedReceivedInvite(bnetInvitation2);
							}
							else if (action == 5)
							{
								BnetInvitation bnetInvitation3 = BnetInvitation.CreateFromFriendsUpdate(src);
								this.m_sentInvites.Add(bnetInvitation3);
								bnetFriendChangelist.AddAddedSentInvite(bnetInvitation3);
							}
							else if (action == 6)
							{
								BnetInvitation bnetInvitation4 = BnetInvitation.CreateFromFriendsUpdate(src);
								this.m_sentInvites.Remove(bnetInvitation4);
								bnetFriendChangelist.AddRemovedSentInvite(bnetInvitation4);
							}
						}
					}
				}
			}
		}
		if (!bnetFriendChangelist.IsEmpty())
		{
			this.FireChangeEvent(bnetFriendChangelist);
		}
	}

	// Token: 0x06003921 RID: 14625 RVA: 0x00116998 File Offset: 0x00114B98
	private void OnPendingPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		this.ProcessPendingFriends();
	}

	// Token: 0x06003922 RID: 14626 RVA: 0x001169A0 File Offset: 0x00114BA0
	private bool OnBnetError(BnetErrorInfo info, object userData)
	{
		Log.Mike.Print("BnetFriendMgr.OnBnetError() - event={0} error={1}", new object[]
		{
			info.GetFeatureEvent(),
			info.GetError()
		});
		return true;
	}

	// Token: 0x06003923 RID: 14627 RVA: 0x001169E0 File Offset: 0x00114BE0
	private void Clear()
	{
		this.m_friends.Clear();
		this.m_receivedInvites.Clear();
		this.m_sentInvites.Clear();
		this.m_pendingChangelist.Clear();
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPendingPlayersChanged));
	}

	// Token: 0x06003924 RID: 14628 RVA: 0x00116A30 File Offset: 0x00114C30
	private void FireChangeEvent(BnetFriendChangelist changelist)
	{
		foreach (BnetFriendMgr.ChangeListener changeListener in this.m_changeListeners.ToArray())
		{
			changeListener.Fire(changelist);
		}
	}

	// Token: 0x06003925 RID: 14629 RVA: 0x00116A68 File Offset: 0x00114C68
	private void AddPendingFriend(BnetPlayer friend)
	{
		if (!this.m_pendingChangelist.Add(friend))
		{
			return;
		}
		if (this.m_pendingChangelist.GetCount() == 1)
		{
			BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPendingPlayersChanged));
		}
	}

	// Token: 0x06003926 RID: 14630 RVA: 0x00116AB0 File Offset: 0x00114CB0
	private void RemovePendingFriend(BnetPlayer friend)
	{
		if (!this.m_pendingChangelist.Remove(friend))
		{
			return;
		}
		if (this.m_pendingChangelist.GetCount() == 0)
		{
			BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPendingPlayersChanged));
		}
		else
		{
			this.ProcessPendingFriends();
		}
	}

	// Token: 0x06003927 RID: 14631 RVA: 0x00116B04 File Offset: 0x00114D04
	private void FirePendingFriendsChangedEvent()
	{
		BnetFriendChangelist changelist = this.m_pendingChangelist.CreateChangelist();
		if (this.m_pendingChangelist.GetCount() == 0)
		{
			BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPendingPlayersChanged));
		}
		this.FireChangeEvent(changelist);
	}

	// Token: 0x04002487 RID: 9351
	private static BnetFriendMgr s_instance;

	// Token: 0x04002488 RID: 9352
	private int m_maxFriends;

	// Token: 0x04002489 RID: 9353
	private int m_maxReceivedInvites;

	// Token: 0x0400248A RID: 9354
	private int m_maxSentInvites;

	// Token: 0x0400248B RID: 9355
	private List<BnetPlayer> m_friends = new List<BnetPlayer>();

	// Token: 0x0400248C RID: 9356
	private List<BnetInvitation> m_receivedInvites = new List<BnetInvitation>();

	// Token: 0x0400248D RID: 9357
	private List<BnetInvitation> m_sentInvites = new List<BnetInvitation>();

	// Token: 0x0400248E RID: 9358
	private List<BnetFriendMgr.ChangeListener> m_changeListeners = new List<BnetFriendMgr.ChangeListener>();

	// Token: 0x0400248F RID: 9359
	private PendingBnetFriendChangelist m_pendingChangelist = new PendingBnetFriendChangelist();

	// Token: 0x020004D1 RID: 1233
	// (Invoke) Token: 0x06003A1A RID: 14874
	public delegate void ChangeCallback(BnetFriendChangelist changelist, object userData);

	// Token: 0x020004D3 RID: 1235
	private class ChangeListener : EventListener<BnetFriendMgr.ChangeCallback>
	{
		// Token: 0x06003A39 RID: 14905 RVA: 0x0011A42F File Offset: 0x0011862F
		public void Fire(BnetFriendChangelist changelist)
		{
			this.m_callback(changelist, this.m_userData);
		}
	}
}
