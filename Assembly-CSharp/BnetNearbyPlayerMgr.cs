using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using bgs;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class BnetNearbyPlayerMgr
{
	// Token: 0x06000A0A RID: 2570 RVA: 0x0002C036 File Offset: 0x0002A236
	public static BnetNearbyPlayerMgr Get()
	{
		if (BnetNearbyPlayerMgr.s_instance == null)
		{
			BnetNearbyPlayerMgr.s_instance = new BnetNearbyPlayerMgr();
			ApplicationMgr.Get().WillReset += new Action(BnetNearbyPlayerMgr.s_instance.Clear);
		}
		return BnetNearbyPlayerMgr.s_instance;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0002C06C File Offset: 0x0002A26C
	public void Initialize()
	{
		this.m_bnetVersion = BattleNet.GetVersion();
		this.m_bnetEnvironment = BattleNet.GetEnvironment();
		this.UpdateEnabled();
		Options.Get().RegisterChangedListener(Option.NEARBY_PLAYERS, new Options.ChangedCallback(this.OnEnabledOptionChanged));
		BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x0002C0C8 File Offset: 0x0002A2C8
	public void Shutdown()
	{
		if (this.m_listening)
		{
			this.m_client.Close();
		}
		Options.Get().UnregisterChangedListener(Option.NEARBY_PLAYERS, new Options.ChangedCallback(this.OnEnabledOptionChanged));
		BnetFriendMgr.Get().RemoveChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x0002C11B File Offset: 0x0002A31B
	public bool IsEnabled()
	{
		return Options.Get().GetBool(Option.NEARBY_PLAYERS) && this.m_enabled;
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x0002C13E File Offset: 0x0002A33E
	public void SetEnabled(bool enabled)
	{
		this.m_enabled = enabled;
		this.UpdateEnabled();
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x0002C150 File Offset: 0x0002A350
	public bool GetNearbySessionStartTime(BnetPlayer bnetPlayer, out ulong sessionStartTime)
	{
		sessionStartTime = 0UL;
		if (bnetPlayer == null)
		{
			return false;
		}
		BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer = null;
		object mutex = this.m_mutex;
		lock (mutex)
		{
			nearbyPlayer = this.m_nearbyPlayers.Find((BnetNearbyPlayerMgr.NearbyPlayer obj) => obj.m_bnetPlayer.GetAccountId() == bnetPlayer.GetAccountId());
		}
		if (nearbyPlayer == null)
		{
			return false;
		}
		sessionStartTime = nearbyPlayer.m_sessionStartTime;
		return true;
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x0002C1D0 File Offset: 0x0002A3D0
	public bool HasNearbyStrangers()
	{
		bool result;
		if (this.m_nearbyStrangers.Count > 0)
		{
			result = Enumerable.Any<BnetPlayer>(this.m_nearbyStrangers, (BnetPlayer p) => p != null && p.IsOnline());
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x0002C219 File Offset: 0x0002A419
	public List<BnetPlayer> GetNearbyPlayers()
	{
		return this.m_nearbyBnetPlayers;
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x0002C221 File Offset: 0x0002A421
	public List<BnetPlayer> GetNearbyFriends()
	{
		return this.m_nearbyFriends;
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0002C229 File Offset: 0x0002A429
	public List<BnetPlayer> GetNearbyStrangers()
	{
		return this.m_nearbyStrangers;
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x0002C231 File Offset: 0x0002A431
	public bool IsNearbyPlayer(BnetPlayer player)
	{
		return this.FindNearbyPlayer(player) != null;
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x0002C240 File Offset: 0x0002A440
	public bool IsNearbyPlayer(BnetGameAccountId id)
	{
		return this.FindNearbyPlayer(id) != null;
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x0002C24F File Offset: 0x0002A44F
	public bool IsNearbyPlayer(BnetAccountId id)
	{
		return this.FindNearbyPlayer(id) != null;
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x0002C25E File Offset: 0x0002A45E
	public bool IsNearbyFriend(BnetPlayer player)
	{
		return this.FindNearbyFriend(player) != null;
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x0002C26D File Offset: 0x0002A46D
	public bool IsNearbyFriend(BnetGameAccountId id)
	{
		return this.FindNearbyFriend(id) != null;
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x0002C27C File Offset: 0x0002A47C
	public bool IsNearbyFriend(BnetAccountId id)
	{
		return this.FindNearbyFriend(id) != null;
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0002C28B File Offset: 0x0002A48B
	public bool IsNearbyStranger(BnetPlayer player)
	{
		return this.FindNearbyStranger(player) != null;
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x0002C29A File Offset: 0x0002A49A
	public bool IsNearbyStranger(BnetGameAccountId id)
	{
		return this.FindNearbyStranger(id) != null;
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0002C2A9 File Offset: 0x0002A4A9
	public bool IsNearbyStranger(BnetAccountId id)
	{
		return this.FindNearbyStranger(id) != null;
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x0002C2B8 File Offset: 0x0002A4B8
	public BnetPlayer FindNearbyPlayer(BnetPlayer player)
	{
		return this.FindNearbyPlayer(player, this.m_nearbyBnetPlayers);
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x0002C2C7 File Offset: 0x0002A4C7
	public BnetPlayer FindNearbyPlayer(BnetGameAccountId id)
	{
		return this.FindNearbyPlayer(id, this.m_nearbyBnetPlayers);
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x0002C2D6 File Offset: 0x0002A4D6
	public BnetPlayer FindNearbyPlayer(BnetAccountId id)
	{
		return this.FindNearbyPlayer(id, this.m_nearbyBnetPlayers);
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x0002C2E5 File Offset: 0x0002A4E5
	public BnetPlayer FindNearbyFriend(BnetGameAccountId id)
	{
		return this.FindNearbyPlayer(id, this.m_nearbyFriends);
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x0002C2F4 File Offset: 0x0002A4F4
	public BnetPlayer FindNearbyFriend(BnetPlayer player)
	{
		return this.FindNearbyPlayer(player, this.m_nearbyFriends);
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x0002C303 File Offset: 0x0002A503
	public BnetPlayer FindNearbyFriend(BnetAccountId id)
	{
		return this.FindNearbyPlayer(id, this.m_nearbyFriends);
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x0002C312 File Offset: 0x0002A512
	public BnetPlayer FindNearbyStranger(BnetPlayer player)
	{
		return this.FindNearbyPlayer(player, this.m_nearbyStrangers);
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x0002C321 File Offset: 0x0002A521
	public BnetPlayer FindNearbyStranger(BnetGameAccountId id)
	{
		return this.FindNearbyPlayer(id, this.m_nearbyStrangers);
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x0002C330 File Offset: 0x0002A530
	public BnetPlayer FindNearbyStranger(BnetAccountId id)
	{
		return this.FindNearbyPlayer(id, this.m_nearbyStrangers);
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0002C33F File Offset: 0x0002A53F
	public bool GetAvailability()
	{
		return this.m_availability;
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x0002C347 File Offset: 0x0002A547
	public void SetAvailability(bool av)
	{
		this.m_availability = av;
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x0002C350 File Offset: 0x0002A550
	public bool AddChangeListener(BnetNearbyPlayerMgr.ChangeCallback callback)
	{
		return this.AddChangeListener(callback, null);
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0002C35C File Offset: 0x0002A55C
	public bool AddChangeListener(BnetNearbyPlayerMgr.ChangeCallback callback, object userData)
	{
		BnetNearbyPlayerMgr.ChangeListener changeListener = new BnetNearbyPlayerMgr.ChangeListener();
		changeListener.SetCallback(callback);
		changeListener.SetUserData(userData);
		if (this.m_changeListeners.Contains(changeListener))
		{
			return false;
		}
		this.m_changeListeners.Add(changeListener);
		return true;
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0002C39D File Offset: 0x0002A59D
	public bool RemoveChangeListener(BnetNearbyPlayerMgr.ChangeCallback callback)
	{
		return this.RemoveChangeListener(callback, null);
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0002C3A8 File Offset: 0x0002A5A8
	public bool RemoveChangeListener(BnetNearbyPlayerMgr.ChangeCallback callback, object userData)
	{
		BnetNearbyPlayerMgr.ChangeListener changeListener = new BnetNearbyPlayerMgr.ChangeListener();
		changeListener.SetCallback(callback);
		changeListener.SetUserData(userData);
		return this.m_changeListeners.Remove(changeListener);
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x0002C3D8 File Offset: 0x0002A5D8
	private void BeginListening()
	{
		if (this.m_listening)
		{
			return;
		}
		this.m_listening = true;
		IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Any, 1228);
		UdpClient udpClient = new UdpClient();
		udpClient.Client.SetSocketOption(65535, 4, true);
		udpClient.Client.Bind(ipendPoint);
		this.m_port = 1228;
		this.m_client = udpClient;
		BnetNearbyPlayerMgr.UdpState udpState = new BnetNearbyPlayerMgr.UdpState();
		udpState.e = ipendPoint;
		udpState.u = this.m_client;
		this.m_lastCallTime = Time.realtimeSinceStartup;
		this.m_client.BeginReceive(new AsyncCallback(this.OnUdpReceive), udpState);
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x0002C47C File Offset: 0x0002A67C
	private void OnUdpReceive(IAsyncResult ar)
	{
		UdpClient u = ((BnetNearbyPlayerMgr.UdpState)ar.AsyncState).u;
		IPEndPoint e = ((BnetNearbyPlayerMgr.UdpState)ar.AsyncState).e;
		byte[] array = u.EndReceive(ar, ref e);
		u.BeginReceive(new AsyncCallback(this.OnUdpReceive), ar.AsyncState);
		string @string = Encoding.UTF8.GetString(array);
		string[] array2 = @string.Split(new char[]
		{
			','
		});
		ulong hi = 0UL;
		ulong lo = 0UL;
		ulong hi2 = 0UL;
		ulong num = 0UL;
		int number = 0;
		bool flag = false;
		ulong sessionStartTime = 0UL;
		int num2 = 0;
		if (num2 >= array2.Length)
		{
			return;
		}
		if (!ulong.TryParse(array2[num2++], ref hi))
		{
			return;
		}
		if (num2 >= array2.Length)
		{
			return;
		}
		if (!ulong.TryParse(array2[num2++], ref lo))
		{
			return;
		}
		if (num2 >= array2.Length)
		{
			return;
		}
		if (!ulong.TryParse(array2[num2++], ref hi2))
		{
			return;
		}
		if (num2 >= array2.Length)
		{
			return;
		}
		if (!ulong.TryParse(array2[num2++], ref num))
		{
			return;
		}
		if (this.m_myGameAccountLo == num)
		{
			return;
		}
		if (num2 >= array2.Length)
		{
			return;
		}
		string name = array2[num2++];
		if (num2 >= array2.Length)
		{
			return;
		}
		if (!int.TryParse(array2[num2++], ref number))
		{
			return;
		}
		if (num2 >= array2.Length)
		{
			return;
		}
		string text = array2[num2++];
		if (string.IsNullOrEmpty(text) || text != this.m_bnetVersion)
		{
			return;
		}
		if (num2 >= array2.Length)
		{
			return;
		}
		string text2 = array2[num2++];
		if (string.IsNullOrEmpty(text2) || text2 != this.m_bnetEnvironment)
		{
			return;
		}
		if (num2 >= array2.Length)
		{
			return;
		}
		string text3 = array2[num2++];
		if (text3 == "1")
		{
			flag = true;
		}
		else
		{
			if (!(text3 == "0"))
			{
				return;
			}
			flag = false;
		}
		if (num2 >= array2.Length)
		{
			return;
		}
		if (!ulong.TryParse(array2[num2++], ref sessionStartTime))
		{
			return;
		}
		BnetBattleTag bnetBattleTag = new BnetBattleTag();
		bnetBattleTag.SetName(name);
		bnetBattleTag.SetNumber(number);
		BnetAccountId bnetAccountId = new BnetAccountId();
		bnetAccountId.SetHi(hi);
		bnetAccountId.SetLo(lo);
		BnetAccount bnetAccount = new BnetAccount();
		bnetAccount.SetId(bnetAccountId);
		bnetAccount.SetBattleTag(bnetBattleTag);
		BnetGameAccountId bnetGameAccountId = new BnetGameAccountId();
		bnetGameAccountId.SetHi(hi2);
		bnetGameAccountId.SetLo(num);
		BnetGameAccount bnetGameAccount = new BnetGameAccount();
		bnetGameAccount.SetId(bnetGameAccountId);
		bnetGameAccount.SetBattleTag(bnetBattleTag);
		bnetGameAccount.SetOnline(true);
		bnetGameAccount.SetProgramId(BnetProgramId.HEARTHSTONE);
		bnetGameAccount.SetGameField(1U, flag);
		bnetGameAccount.SetGameField(19U, text);
		bnetGameAccount.SetGameField(20U, text2);
		BnetPlayer bnetPlayer = new BnetPlayer();
		bnetPlayer.SetAccount(bnetAccount);
		bnetPlayer.AddGameAccount(bnetGameAccount);
		BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer = new BnetNearbyPlayerMgr.NearbyPlayer();
		nearbyPlayer.m_bnetPlayer = bnetPlayer;
		nearbyPlayer.m_availability = flag;
		nearbyPlayer.m_sessionStartTime = sessionStartTime;
		object mutex = this.m_mutex;
		lock (mutex)
		{
			if (this.m_listening)
			{
				foreach (BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer2 in this.m_nearbyAdds)
				{
					if (nearbyPlayer2.Equals(nearbyPlayer))
					{
						this.UpdateNearbyPlayer(nearbyPlayer2, flag, sessionStartTime);
						return;
					}
				}
				foreach (BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer3 in this.m_nearbyUpdates)
				{
					if (nearbyPlayer3.Equals(nearbyPlayer))
					{
						this.UpdateNearbyPlayer(nearbyPlayer3, flag, sessionStartTime);
						return;
					}
				}
				foreach (BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer4 in this.m_nearbyPlayers)
				{
					if (nearbyPlayer4.Equals(nearbyPlayer))
					{
						this.UpdateNearbyPlayer(nearbyPlayer4, flag, sessionStartTime);
						this.m_nearbyUpdates.Add(nearbyPlayer4);
						return;
					}
				}
				this.m_nearbyAdds.Add(nearbyPlayer);
			}
		}
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0002C970 File Offset: 0x0002AB70
	private void StopListening()
	{
		if (!this.m_listening)
		{
			return;
		}
		this.m_listening = false;
		this.m_client.Close();
		BnetNearbyPlayerChangelist bnetNearbyPlayerChangelist = new BnetNearbyPlayerChangelist();
		object mutex = this.m_mutex;
		lock (mutex)
		{
			foreach (BnetPlayer player in this.m_nearbyBnetPlayers)
			{
				bnetNearbyPlayerChangelist.AddRemovedPlayer(player);
			}
			foreach (BnetPlayer friend in this.m_nearbyFriends)
			{
				bnetNearbyPlayerChangelist.AddRemovedFriend(friend);
			}
			foreach (BnetPlayer stranger in this.m_nearbyStrangers)
			{
				bnetNearbyPlayerChangelist.AddRemovedStranger(stranger);
			}
			this.m_nearbyPlayers.Clear();
			this.m_nearbyBnetPlayers.Clear();
			this.m_nearbyFriends.Clear();
			this.m_nearbyStrangers.Clear();
			this.m_nearbyAdds.Clear();
			this.m_nearbyUpdates.Clear();
		}
		this.FireChangeEvent(bnetNearbyPlayerChangelist);
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x0002CB2C File Offset: 0x0002AD2C
	public void Update()
	{
		if (!this.m_listening)
		{
			return;
		}
		this.CacheMyAccountInfo();
		this.CheckIntervalAndBroadcast();
		this.ProcessPlayerChanges();
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x0002CB50 File Offset: 0x0002AD50
	private void Clear()
	{
		object mutex = this.m_mutex;
		lock (mutex)
		{
			this.m_nearbyPlayers.Clear();
			this.m_nearbyBnetPlayers.Clear();
			this.m_nearbyFriends.Clear();
			this.m_nearbyStrangers.Clear();
			this.m_nearbyAdds.Clear();
			this.m_nearbyUpdates.Clear();
		}
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0002CBC8 File Offset: 0x0002ADC8
	private void UpdateEnabled()
	{
		bool flag = this.IsEnabled();
		if (flag == this.m_listening)
		{
			return;
		}
		if (flag)
		{
			this.BeginListening();
		}
		else
		{
			this.StopListening();
		}
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0002CC00 File Offset: 0x0002AE00
	private void FireChangeEvent(BnetNearbyPlayerChangelist changelist)
	{
		if (changelist.IsEmpty())
		{
			return;
		}
		foreach (BnetNearbyPlayerMgr.ChangeListener changeListener in this.m_changeListeners.ToArray())
		{
			changeListener.Fire(changelist);
		}
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0002CC44 File Offset: 0x0002AE44
	private void CacheMyAccountInfo()
	{
		if (this.m_idString != null)
		{
			return;
		}
		BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
		if (myGameAccountId == null)
		{
			return;
		}
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		if (myPlayer == null)
		{
			return;
		}
		BnetAccountId accountId = myPlayer.GetAccountId();
		if (accountId == null)
		{
			return;
		}
		BnetBattleTag battleTag = myPlayer.GetBattleTag();
		if (battleTag == null)
		{
			return;
		}
		this.m_myGameAccountLo = myGameAccountId.GetLo();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(accountId.GetHi());
		stringBuilder.Append(',');
		stringBuilder.Append(accountId.GetLo());
		stringBuilder.Append(',');
		stringBuilder.Append(myGameAccountId.GetHi());
		stringBuilder.Append(',');
		stringBuilder.Append(myGameAccountId.GetLo());
		stringBuilder.Append(',');
		stringBuilder.Append(battleTag.GetName());
		stringBuilder.Append(',');
		stringBuilder.Append(battleTag.GetNumber());
		stringBuilder.Append(',');
		stringBuilder.Append(BattleNet.GetVersion());
		stringBuilder.Append(',');
		stringBuilder.Append(BattleNet.GetEnvironment());
		this.m_idString = stringBuilder.ToString();
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x0002CD84 File Offset: 0x0002AF84
	private void ProcessPlayerChanges()
	{
		BnetNearbyPlayerChangelist changelist = new BnetNearbyPlayerChangelist();
		object mutex = this.m_mutex;
		lock (mutex)
		{
			this.ProcessAddedPlayers(changelist);
			this.ProcessUpdatedPlayers(changelist);
			this.RemoveInactivePlayers(changelist);
		}
		this.FireChangeEvent(changelist);
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0002CDDC File Offset: 0x0002AFDC
	private void ProcessAddedPlayers(BnetNearbyPlayerChangelist changelist)
	{
		if (this.m_nearbyAdds.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.m_nearbyAdds.Count; i++)
		{
			BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer = this.m_nearbyAdds[i];
			nearbyPlayer.m_lastReceivedTime = Time.realtimeSinceStartup;
			this.m_nearbyPlayers.Add(nearbyPlayer);
			this.m_nearbyBnetPlayers.Add(nearbyPlayer.m_bnetPlayer);
			changelist.AddAddedPlayer(nearbyPlayer.m_bnetPlayer);
			if (nearbyPlayer.IsFriend())
			{
				this.m_nearbyFriends.Add(nearbyPlayer.m_bnetPlayer);
				changelist.AddAddedFriend(nearbyPlayer.m_bnetPlayer);
			}
			else
			{
				this.m_nearbyStrangers.Add(nearbyPlayer.m_bnetPlayer);
				changelist.AddAddedStranger(nearbyPlayer.m_bnetPlayer);
			}
		}
		this.m_nearbyAdds.Clear();
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x0002CEB0 File Offset: 0x0002B0B0
	private void ProcessUpdatedPlayers(BnetNearbyPlayerChangelist changelist)
	{
		if (this.m_nearbyUpdates.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.m_nearbyUpdates.Count; i++)
		{
			BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer = this.m_nearbyUpdates[i];
			nearbyPlayer.m_lastReceivedTime = Time.realtimeSinceStartup;
			changelist.AddUpdatedPlayer(nearbyPlayer.m_bnetPlayer);
			if (nearbyPlayer.IsFriend())
			{
				changelist.AddUpdatedFriend(nearbyPlayer.m_bnetPlayer);
			}
			else
			{
				changelist.AddUpdatedStranger(nearbyPlayer.m_bnetPlayer);
			}
		}
		this.m_nearbyUpdates.Clear();
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0002CF44 File Offset: 0x0002B144
	private void RemoveInactivePlayers(BnetNearbyPlayerChangelist changelist)
	{
		List<BnetNearbyPlayerMgr.NearbyPlayer> list = null;
		for (int i = 0; i < this.m_nearbyPlayers.Count; i++)
		{
			BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer = this.m_nearbyPlayers[i];
			float num = Time.realtimeSinceStartup - nearbyPlayer.m_lastReceivedTime;
			if (num >= 60f)
			{
				if (list == null)
				{
					list = new List<BnetNearbyPlayerMgr.NearbyPlayer>();
				}
				list.Add(nearbyPlayer);
			}
		}
		if (list != null)
		{
			foreach (BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer2 in list)
			{
				this.m_nearbyPlayers.Remove(nearbyPlayer2);
				if (this.m_nearbyBnetPlayers.Remove(nearbyPlayer2.m_bnetPlayer))
				{
					changelist.AddRemovedPlayer(nearbyPlayer2.m_bnetPlayer);
				}
				if (this.m_nearbyFriends.Remove(nearbyPlayer2.m_bnetPlayer))
				{
					changelist.AddRemovedFriend(nearbyPlayer2.m_bnetPlayer);
				}
				if (this.m_nearbyStrangers.Remove(nearbyPlayer2.m_bnetPlayer))
				{
					changelist.AddRemovedStranger(nearbyPlayer2.m_bnetPlayer);
				}
			}
		}
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x0002D070 File Offset: 0x0002B270
	private bool CheckIntervalAndBroadcast()
	{
		float num = Time.realtimeSinceStartup - this.m_lastCallTime;
		if (num < 12f)
		{
			return false;
		}
		this.m_lastCallTime = Time.realtimeSinceStartup;
		this.Broadcast();
		return true;
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0002D0AC File Offset: 0x0002B2AC
	private void Broadcast()
	{
		string text = this.CreateBroadcastString();
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Broadcast, this.m_port);
		UdpClient udpClient = new UdpClient();
		udpClient.EnableBroadcast = true;
		try
		{
			udpClient.Send(bytes, bytes.Length, ipendPoint);
		}
		catch
		{
		}
		finally
		{
			udpClient.Close();
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0002D124 File Offset: 0x0002B324
	private string CreateBroadcastString()
	{
		ulong sessionStartTime = HealthyGamingMgr.Get().GetSessionStartTime();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.m_idString);
		stringBuilder.Append(',');
		stringBuilder.Append((!this.m_availability) ? "0" : "1");
		stringBuilder.Append(',');
		stringBuilder.Append(sessionStartTime);
		return stringBuilder.ToString();
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0002D194 File Offset: 0x0002B394
	private int FindNearbyPlayerIndex(BnetPlayer bnetPlayer, List<BnetPlayer> bnetPlayers)
	{
		if (bnetPlayer == null)
		{
			return -1;
		}
		BnetAccountId accountId = bnetPlayer.GetAccountId();
		if (accountId != null)
		{
			return this.FindNearbyPlayerIndex(accountId, bnetPlayers);
		}
		BnetGameAccountId hearthstoneGameAccountId = bnetPlayer.GetHearthstoneGameAccountId();
		return this.FindNearbyPlayerIndex(hearthstoneGameAccountId, bnetPlayers);
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0002D1D4 File Offset: 0x0002B3D4
	private int FindNearbyPlayerIndex(BnetGameAccountId id, List<BnetPlayer> bnetPlayers)
	{
		if (id == null)
		{
			return -1;
		}
		for (int i = 0; i < bnetPlayers.Count; i++)
		{
			BnetPlayer bnetPlayer = bnetPlayers[i];
			if (id == bnetPlayer.GetHearthstoneGameAccountId())
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0002D224 File Offset: 0x0002B424
	private int FindNearbyPlayerIndex(BnetAccountId id, List<BnetPlayer> bnetPlayers)
	{
		if (id == null)
		{
			return -1;
		}
		for (int i = 0; i < bnetPlayers.Count; i++)
		{
			BnetPlayer bnetPlayer = bnetPlayers[i];
			if (id == bnetPlayer.GetAccountId())
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x0002D274 File Offset: 0x0002B474
	private BnetPlayer FindNearbyPlayer(BnetPlayer bnetPlayer, List<BnetPlayer> bnetPlayers)
	{
		if (bnetPlayer == null)
		{
			return null;
		}
		BnetAccountId accountId = bnetPlayer.GetAccountId();
		if (accountId != null)
		{
			return this.FindNearbyPlayer(accountId, bnetPlayers);
		}
		BnetGameAccountId hearthstoneGameAccountId = bnetPlayer.GetHearthstoneGameAccountId();
		return this.FindNearbyPlayer(hearthstoneGameAccountId, bnetPlayers);
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0002D2B4 File Offset: 0x0002B4B4
	private BnetPlayer FindNearbyPlayer(BnetGameAccountId id, List<BnetPlayer> bnetPlayers)
	{
		int num = this.FindNearbyPlayerIndex(id, bnetPlayers);
		if (num < 0)
		{
			return null;
		}
		return bnetPlayers[num];
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0002D2DC File Offset: 0x0002B4DC
	private BnetPlayer FindNearbyPlayer(BnetAccountId id, List<BnetPlayer> bnetPlayers)
	{
		int num = this.FindNearbyPlayerIndex(id, bnetPlayers);
		if (num < 0)
		{
			return null;
		}
		return bnetPlayers[num];
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0002D304 File Offset: 0x0002B504
	private void UpdateNearbyPlayer(BnetNearbyPlayerMgr.NearbyPlayer player, bool available, ulong sessionStartTime)
	{
		BnetGameAccount gameAccount = player.GetGameAccount();
		gameAccount.SetGameField(1U, available);
		player.m_sessionStartTime = sessionStartTime;
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0002D32C File Offset: 0x0002B52C
	private void OnEnabledOptionChanged(Option option, object prevValue, bool existed, object userData)
	{
		this.UpdateEnabled();
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0002D334 File Offset: 0x0002B534
	private void OnFriendsChanged(BnetFriendChangelist friendChangelist, object userData)
	{
		List<BnetPlayer> addedFriends = friendChangelist.GetAddedFriends();
		List<BnetPlayer> removedFriends = friendChangelist.GetRemovedFriends();
		bool flag = addedFriends != null && addedFriends.Count > 0;
		bool flag2 = removedFriends != null && removedFriends.Count > 0;
		if (!flag && !flag2)
		{
			return;
		}
		BnetNearbyPlayerChangelist bnetNearbyPlayerChangelist = new BnetNearbyPlayerChangelist();
		object mutex = this.m_mutex;
		lock (mutex)
		{
			if (addedFriends != null)
			{
				foreach (BnetPlayer bnetPlayer in addedFriends)
				{
					int num = this.FindNearbyPlayerIndex(bnetPlayer, this.m_nearbyStrangers);
					if (num >= 0)
					{
						BnetPlayer bnetPlayer2 = this.m_nearbyStrangers[num];
						this.m_nearbyStrangers.RemoveAt(num);
						this.m_nearbyFriends.Add(bnetPlayer2);
						bnetNearbyPlayerChangelist.AddAddedFriend(bnetPlayer2);
						bnetNearbyPlayerChangelist.AddRemovedStranger(bnetPlayer2);
					}
				}
			}
			if (removedFriends != null)
			{
				foreach (BnetPlayer bnetPlayer3 in removedFriends)
				{
					int num2 = this.FindNearbyPlayerIndex(bnetPlayer3, this.m_nearbyFriends);
					if (num2 >= 0)
					{
						BnetPlayer bnetPlayer4 = this.m_nearbyFriends[num2];
						this.m_nearbyFriends.RemoveAt(num2);
						this.m_nearbyStrangers.Add(bnetPlayer4);
						bnetNearbyPlayerChangelist.AddAddedStranger(bnetPlayer4);
						bnetNearbyPlayerChangelist.AddRemovedFriend(bnetPlayer4);
					}
				}
			}
		}
		this.FireChangeEvent(bnetNearbyPlayerChangelist);
	}

	// Token: 0x040004D6 RID: 1238
	private const int UDP_PORT = 1228;

	// Token: 0x040004D7 RID: 1239
	private const float UPDATE_INTERVAL = 12f;

	// Token: 0x040004D8 RID: 1240
	private const float INACTIVITY_TIMEOUT = 60f;

	// Token: 0x040004D9 RID: 1241
	private static BnetNearbyPlayerMgr s_instance;

	// Token: 0x040004DA RID: 1242
	private bool m_enabled = true;

	// Token: 0x040004DB RID: 1243
	private bool m_listening;

	// Token: 0x040004DC RID: 1244
	private ulong m_myGameAccountLo;

	// Token: 0x040004DD RID: 1245
	private string m_bnetVersion;

	// Token: 0x040004DE RID: 1246
	private string m_bnetEnvironment;

	// Token: 0x040004DF RID: 1247
	private string m_idString;

	// Token: 0x040004E0 RID: 1248
	private bool m_availability;

	// Token: 0x040004E1 RID: 1249
	private UdpClient m_client;

	// Token: 0x040004E2 RID: 1250
	private int m_port;

	// Token: 0x040004E3 RID: 1251
	private float m_lastCallTime;

	// Token: 0x040004E4 RID: 1252
	private List<BnetNearbyPlayerMgr.NearbyPlayer> m_nearbyPlayers = new List<BnetNearbyPlayerMgr.NearbyPlayer>();

	// Token: 0x040004E5 RID: 1253
	private List<BnetPlayer> m_nearbyBnetPlayers = new List<BnetPlayer>();

	// Token: 0x040004E6 RID: 1254
	private List<BnetPlayer> m_nearbyFriends = new List<BnetPlayer>();

	// Token: 0x040004E7 RID: 1255
	private List<BnetPlayer> m_nearbyStrangers = new List<BnetPlayer>();

	// Token: 0x040004E8 RID: 1256
	private object m_mutex = new object();

	// Token: 0x040004E9 RID: 1257
	private List<BnetNearbyPlayerMgr.NearbyPlayer> m_nearbyAdds = new List<BnetNearbyPlayerMgr.NearbyPlayer>();

	// Token: 0x040004EA RID: 1258
	private List<BnetNearbyPlayerMgr.NearbyPlayer> m_nearbyUpdates = new List<BnetNearbyPlayerMgr.NearbyPlayer>();

	// Token: 0x040004EB RID: 1259
	private List<BnetNearbyPlayerMgr.ChangeListener> m_changeListeners = new List<BnetNearbyPlayerMgr.ChangeListener>();

	// Token: 0x02000588 RID: 1416
	// (Invoke) Token: 0x0600405F RID: 16479
	public delegate void ChangeCallback(BnetNearbyPlayerChangelist changelist, object userData);

	// Token: 0x020005CF RID: 1487
	private class ChangeListener : EventListener<BnetNearbyPlayerMgr.ChangeCallback>
	{
		// Token: 0x0600425D RID: 16989 RVA: 0x001402F6 File Offset: 0x0013E4F6
		public void Fire(BnetNearbyPlayerChangelist changelist)
		{
			this.m_callback(changelist, this.m_userData);
		}
	}

	// Token: 0x020005D0 RID: 1488
	private class NearbyPlayer : IEquatable<BnetNearbyPlayerMgr.NearbyPlayer>
	{
		// Token: 0x0600425F RID: 16991 RVA: 0x00140312 File Offset: 0x0013E512
		public bool Equals(BnetNearbyPlayerMgr.NearbyPlayer other)
		{
			return other != null && this.GetGameAccountId() == other.GetGameAccountId();
		}

		// Token: 0x06004260 RID: 16992 RVA: 0x0014032D File Offset: 0x0013E52D
		public BnetAccountId GetAccountId()
		{
			return this.m_bnetPlayer.GetAccountId();
		}

		// Token: 0x06004261 RID: 16993 RVA: 0x0014033A File Offset: 0x0013E53A
		public BnetGameAccountId GetGameAccountId()
		{
			return this.m_bnetPlayer.GetHearthstoneGameAccountId();
		}

		// Token: 0x06004262 RID: 16994 RVA: 0x00140347 File Offset: 0x0013E547
		public BnetGameAccount GetGameAccount()
		{
			return this.m_bnetPlayer.GetHearthstoneGameAccount();
		}

		// Token: 0x06004263 RID: 16995 RVA: 0x00140354 File Offset: 0x0013E554
		public bool IsFriend()
		{
			BnetAccountId accountId = this.GetAccountId();
			return BnetFriendMgr.Get().IsFriend(accountId);
		}

		// Token: 0x04002A3D RID: 10813
		public float m_lastReceivedTime;

		// Token: 0x04002A3E RID: 10814
		public BnetPlayer m_bnetPlayer;

		// Token: 0x04002A3F RID: 10815
		public bool m_availability;

		// Token: 0x04002A40 RID: 10816
		public ulong m_sessionStartTime;
	}

	// Token: 0x020005D1 RID: 1489
	private class UdpState
	{
		// Token: 0x04002A41 RID: 10817
		public UdpClient u;

		// Token: 0x04002A42 RID: 10818
		public IPEndPoint e;
	}
}
