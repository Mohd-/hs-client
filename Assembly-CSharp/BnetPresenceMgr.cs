using System;
using System.Collections.Generic;
using System.Linq;
using bgs;
using bgs.types;

// Token: 0x02000112 RID: 274
public class BnetPresenceMgr
{
	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06000CA3 RID: 3235 RVA: 0x00031B37 File Offset: 0x0002FD37
	// (remove) Token: 0x06000CA4 RID: 3236 RVA: 0x00031B50 File Offset: 0x0002FD50
	public event Action<PresenceUpdate[]> OnGameAccountPresenceChange;

	// Token: 0x06000CA5 RID: 3237 RVA: 0x00031B6C File Offset: 0x0002FD6C
	public static BnetPresenceMgr Get()
	{
		if (BnetPresenceMgr.s_instance == null)
		{
			BnetPresenceMgr.s_instance = new BnetPresenceMgr();
			ApplicationMgr.Get().WillReset += delegate()
			{
				BnetPresenceMgr bnetPresenceMgr = BnetPresenceMgr.s_instance;
				BnetPresenceMgr.s_instance = new BnetPresenceMgr();
				BnetPresenceMgr.s_instance.m_playersChangedListeners = bnetPresenceMgr.m_playersChangedListeners;
				BnetPresenceMgr.s_instance.OnGameAccountPresenceChange = bnetPresenceMgr.OnGameAccountPresenceChange;
			};
		}
		return BnetPresenceMgr.s_instance;
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x00031BBC File Offset: 0x0002FDBC
	public void Initialize()
	{
		Network.Get().SetPresenceHandler(new Network.PresenceHandler(this.OnPresenceUpdate));
		BnetEventMgr.Get().AddChangeListener(new BnetEventMgr.ChangeCallback(this.OnBnetEventOccurred));
		EntityId myGameAccountId = BattleNet.GetMyGameAccountId();
		this.m_myGameAccountId = BnetGameAccountId.CreateFromEntityId(myGameAccountId);
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x00031C08 File Offset: 0x0002FE08
	public void Shutdown()
	{
		Network.Get().SetPresenceHandler(null);
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x00031C15 File Offset: 0x0002FE15
	public BnetGameAccountId GetMyGameAccountId()
	{
		return this.m_myGameAccountId;
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x00031C1D File Offset: 0x0002FE1D
	public BnetPlayer GetMyPlayer()
	{
		return this.m_myPlayer;
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x00031C28 File Offset: 0x0002FE28
	public BnetAccount GetAccount(BnetAccountId id)
	{
		if (id == null)
		{
			return null;
		}
		BnetAccount result = null;
		this.m_accounts.TryGetValue(id, out result);
		return result;
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x00031C58 File Offset: 0x0002FE58
	public BnetGameAccount GetGameAccount(BnetGameAccountId id)
	{
		if (id == null)
		{
			return null;
		}
		BnetGameAccount result = null;
		this.m_gameAccounts.TryGetValue(id, out result);
		return result;
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x00031C88 File Offset: 0x0002FE88
	public BnetPlayer GetPlayer(BnetAccountId id)
	{
		if (id == null)
		{
			return null;
		}
		BnetPlayer result = null;
		this.m_players.TryGetValue(id, out result);
		return result;
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x00031CB8 File Offset: 0x0002FEB8
	public BnetPlayer GetPlayer(BnetGameAccountId id)
	{
		BnetGameAccount gameAccount = this.GetGameAccount(id);
		if (gameAccount == null)
		{
			return null;
		}
		return this.GetPlayer(gameAccount.GetOwnerId());
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x00031CE8 File Offset: 0x0002FEE8
	public BnetPlayer RegisterPlayer(BnetAccountId id)
	{
		BnetPlayer bnetPlayer = this.GetPlayer(id);
		if (bnetPlayer != null)
		{
			return bnetPlayer;
		}
		bnetPlayer = new BnetPlayer();
		bnetPlayer.SetAccountId(id);
		this.m_players[id] = bnetPlayer;
		BnetPlayerChange bnetPlayerChange = new BnetPlayerChange();
		bnetPlayerChange.SetNewPlayer(bnetPlayer);
		BnetPlayerChangelist bnetPlayerChangelist = new BnetPlayerChangelist();
		bnetPlayerChangelist.AddChange(bnetPlayerChange);
		this.FirePlayersChangedEvent(bnetPlayerChangelist);
		return bnetPlayer;
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x00031D44 File Offset: 0x0002FF44
	public bool SetGameField(uint fieldId, bool val)
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			Error.AddDevFatal("Caller should check for Battle.net connection before calling SetGameField {0}={1}", new object[]
			{
				fieldId,
				val
			});
			return false;
		}
		BnetGameAccount bnetGameAccount;
		if (!this.ShouldUpdateGameField(fieldId, val, out bnetGameAccount))
		{
			return false;
		}
		if (fieldId == 2U)
		{
			bnetGameAccount.SetBusy(val);
			int num = (!val) ? 0 : 1;
			BattleNet.SetPresenceInt(fieldId, (long)num);
		}
		else
		{
			BattleNet.SetPresenceBool(fieldId, val);
		}
		BnetPlayerChangelist changelist = this.ChangeGameField(bnetGameAccount, fieldId, val);
		if (fieldId != 2U)
		{
			if (fieldId == 10U)
			{
				if (val)
				{
					bnetGameAccount.SetBusy(false);
				}
			}
		}
		else if (val)
		{
			bnetGameAccount.SetAway(false);
		}
		this.FirePlayersChangedEvent(changelist);
		return true;
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x00031E18 File Offset: 0x00030018
	public bool SetGameField(uint fieldId, int val)
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			Error.AddDevFatal("Caller should check for Battle.net connection before calling SetGameField {0}={1}", new object[]
			{
				fieldId,
				val
			});
			return false;
		}
		BnetGameAccount hsGameAccount;
		if (!this.ShouldUpdateGameField(fieldId, val, out hsGameAccount))
		{
			return false;
		}
		BattleNet.SetPresenceInt(fieldId, (long)val);
		BnetPlayerChangelist changelist = this.ChangeGameField(hsGameAccount, fieldId, val);
		this.FirePlayersChangedEvent(changelist);
		return true;
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x00031E88 File Offset: 0x00030088
	public bool SetGameField(uint fieldId, string val)
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			Error.AddDevFatal("Caller should check for Battle.net connection before calling SetGameField {0}={1}", new object[]
			{
				fieldId,
				val
			});
			return false;
		}
		BnetGameAccount hsGameAccount;
		if (!this.ShouldUpdateGameField(fieldId, val, out hsGameAccount))
		{
			return false;
		}
		BattleNet.SetPresenceString(fieldId, val);
		BnetPlayerChangelist changelist = this.ChangeGameField(hsGameAccount, fieldId, val);
		this.FirePlayersChangedEvent(changelist);
		return true;
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x00031EE8 File Offset: 0x000300E8
	public bool SetGameField(uint fieldId, byte[] val)
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			Error.AddDevFatal("Caller should check for Battle.net connection before calling SetGameField {0}=[{1}]", new object[]
			{
				fieldId,
				(val != null) ? val.Length.ToString() : string.Empty
			});
			return false;
		}
		BnetGameAccount hsGameAccount;
		if (!this.ShouldUpdateGameFieldBlob(fieldId, val, out hsGameAccount))
		{
			return false;
		}
		BattleNet.SetPresenceBlob(fieldId, val);
		BnetPlayerChangelist changelist = this.ChangeGameField(hsGameAccount, fieldId, val);
		this.FirePlayersChangedEvent(changelist);
		return true;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x00031F64 File Offset: 0x00030164
	public bool SetGameFieldBlob(uint fieldId, IProtoBuf protoMessage)
	{
		byte[] val = (protoMessage != null) ? ProtobufUtil.ToByteArray(protoMessage) : null;
		return this.SetGameField(fieldId, val);
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x00031F8C File Offset: 0x0003018C
	public bool SetRichPresence(Enum[] richPresence)
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			string message = "Caller should check for Battle.net connection before calling SetRichPresence {0}";
			object[] array = new object[1];
			int num = 0;
			object obj;
			if (richPresence == null)
			{
				obj = string.Empty;
			}
			else
			{
				obj = string.Join(", ", Enumerable.ToArray<string>(Enumerable.Select<Enum, string>(richPresence, (Enum x) => x.ToString())));
			}
			array[num] = obj;
			Error.AddDevFatal(message, array);
			return false;
		}
		if (richPresence == null)
		{
			return false;
		}
		if (richPresence.Length == 0)
		{
			return false;
		}
		RichPresenceUpdate[] array2 = new RichPresenceUpdate[richPresence.Length];
		for (int i = 0; i < richPresence.Length; i++)
		{
			Enum @enum = richPresence[i];
			Type type = @enum.GetType();
			FourCC fourCC = RichPresence.s_streamIds[type];
			RichPresenceUpdate richPresenceUpdate = default(RichPresenceUpdate);
			richPresenceUpdate.presenceFieldIndex = (ulong)((i != 0) ? (458752 + i) : 0);
			richPresenceUpdate.programId = BnetProgramId.HEARTHSTONE.GetValue();
			richPresenceUpdate.streamId = fourCC.GetValue();
			richPresenceUpdate.index = Convert.ToUInt32(@enum);
			array2[i] = richPresenceUpdate;
		}
		BattleNet.SetRichPresence(array2);
		return true;
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x000320A5 File Offset: 0x000302A5
	public bool AddPlayersChangedListener(BnetPresenceMgr.PlayersChangedCallback callback)
	{
		return this.AddPlayersChangedListener(callback, null);
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x000320B0 File Offset: 0x000302B0
	public bool AddPlayersChangedListener(BnetPresenceMgr.PlayersChangedCallback callback, object userData)
	{
		BnetPresenceMgr.PlayersChangedListener playersChangedListener = new BnetPresenceMgr.PlayersChangedListener();
		playersChangedListener.SetCallback(callback);
		playersChangedListener.SetUserData(userData);
		if (this.m_playersChangedListeners.Contains(playersChangedListener))
		{
			return false;
		}
		this.m_playersChangedListeners.Add(playersChangedListener);
		return true;
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x000320F1 File Offset: 0x000302F1
	public bool RemovePlayersChangedListener(BnetPresenceMgr.PlayersChangedCallback callback)
	{
		return this.RemovePlayersChangedListener(callback, null);
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x000320FC File Offset: 0x000302FC
	public bool RemovePlayersChangedListener(BnetPresenceMgr.PlayersChangedCallback callback, object userData)
	{
		BnetPresenceMgr.PlayersChangedListener playersChangedListener = new BnetPresenceMgr.PlayersChangedListener();
		playersChangedListener.SetCallback(callback);
		playersChangedListener.SetUserData(userData);
		return this.m_playersChangedListeners.Remove(playersChangedListener);
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x0003212C File Offset: 0x0003032C
	private void OnPresenceUpdate(PresenceUpdate[] updates)
	{
		BnetPlayerChangelist changelist = new BnetPlayerChangelist();
		IEnumerable<PresenceUpdate> enumerable = Enumerable.Where<PresenceUpdate>(updates, (PresenceUpdate u) => u.programId == BnetProgramId.BNET && u.groupId == 2U && u.fieldId == 7U);
		foreach (PresenceUpdate update in enumerable)
		{
			BnetGameAccountId bnetGameAccountId = BnetGameAccountId.CreateFromEntityId(update.entityId);
			BnetAccountId bnetAccountId = BnetAccountId.CreateFromEntityId(update.entityIdVal);
			if (!bnetAccountId.IsEmpty())
			{
				BnetAccount account = this.GetAccount(bnetAccountId);
				if (account == null)
				{
					PresenceUpdate update2 = default(PresenceUpdate);
					BnetPlayerChangelist changelist2 = new BnetPlayerChangelist();
					this.CreateAccount(bnetAccountId, update2, changelist2);
				}
				if (!bnetGameAccountId.IsEmpty())
				{
					BnetGameAccount gameAccount = this.GetGameAccount(bnetGameAccountId);
					if (gameAccount == null)
					{
						this.CreateGameAccount(bnetGameAccountId, update, changelist);
					}
				}
			}
		}
		List<PresenceUpdate> list = null;
		foreach (PresenceUpdate presenceUpdate in updates)
		{
			if (presenceUpdate.programId == BnetProgramId.BNET)
			{
				if (presenceUpdate.groupId == 1U)
				{
					this.OnAccountUpdate(presenceUpdate, changelist);
				}
				else if (presenceUpdate.groupId == 2U)
				{
					this.OnGameAccountUpdate(presenceUpdate, changelist);
				}
			}
			else if (presenceUpdate.programId == BnetProgramId.HEARTHSTONE)
			{
				this.OnGameUpdate(presenceUpdate, changelist);
			}
			if ((presenceUpdate.programId == BnetProgramId.HEARTHSTONE || (presenceUpdate.programId == BnetProgramId.BNET && presenceUpdate.groupId == 2U)) && this.OnGameAccountPresenceChange != null)
			{
				if (list == null)
				{
					list = new List<PresenceUpdate>();
				}
				list.Add(presenceUpdate);
			}
		}
		if (list != null)
		{
			this.OnGameAccountPresenceChange.Invoke(list.ToArray());
		}
		this.FirePlayersChangedEvent(changelist);
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x00032348 File Offset: 0x00030548
	private void OnBnetEventOccurred(BattleNet.BnetEvent bnetEvent, object userData)
	{
		if (bnetEvent == null)
		{
			foreach (BnetAccount obj in this.m_accounts.Values)
			{
				GeneralUtils.DeepReset<BnetAccount>(obj);
			}
			foreach (BnetGameAccount obj2 in this.m_gameAccounts.Values)
			{
				GeneralUtils.DeepReset<BnetGameAccount>(obj2);
			}
		}
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x000323FC File Offset: 0x000305FC
	private void OnAccountUpdate(PresenceUpdate update, BnetPlayerChangelist changelist)
	{
		BnetAccountId bnetAccountId = BnetAccountId.CreateFromEntityId(update.entityId);
		BnetAccount account = null;
		if (!this.m_accounts.TryGetValue(bnetAccountId, out account))
		{
			this.CreateAccount(bnetAccountId, update, changelist);
		}
		else
		{
			this.UpdateAccount(account, update, changelist);
		}
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x00032444 File Offset: 0x00030644
	private void CreateAccount(BnetAccountId id, PresenceUpdate update, BnetPlayerChangelist changelist)
	{
		BnetAccount bnetAccount = new BnetAccount();
		this.m_accounts.Add(id, bnetAccount);
		bnetAccount.SetId(id);
		BnetPlayer bnetPlayer = null;
		if (!this.m_players.TryGetValue(id, out bnetPlayer))
		{
			bnetPlayer = new BnetPlayer();
			this.m_players.Add(id, bnetPlayer);
			BnetPlayerChange bnetPlayerChange = new BnetPlayerChange();
			bnetPlayerChange.SetNewPlayer(bnetPlayer);
			changelist.AddChange(bnetPlayerChange);
		}
		bnetPlayer.SetAccount(bnetAccount);
		this.UpdateAccount(bnetAccount, update, changelist);
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x000324B8 File Offset: 0x000306B8
	private void UpdateAccount(BnetAccount account, PresenceUpdate update, BnetPlayerChangelist changelist)
	{
		BnetPlayer player = this.m_players[account.GetId()];
		if (update.fieldId == 7U)
		{
			bool boolVal = update.boolVal;
			if (boolVal == account.IsAway())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			account.SetAway(boolVal);
			if (boolVal)
			{
				account.SetBusy(false);
			}
		}
		else if (update.fieldId == 8U)
		{
			ulong intVal = (ulong)update.intVal;
			if (intVal == account.GetAwayTimeMicrosec())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			account.SetAwayTimeMicrosec(intVal);
		}
		else if (update.fieldId == 11U)
		{
			bool boolVal2 = update.boolVal;
			if (boolVal2 == account.IsBusy())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			account.SetBusy(boolVal2);
			if (boolVal2)
			{
				account.SetAway(false);
			}
		}
		else if (update.fieldId == 4U)
		{
			BnetBattleTag bnetBattleTag = BnetBattleTag.CreateFromString(update.stringVal);
			if (bnetBattleTag == account.GetBattleTag())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			account.SetBattleTag(bnetBattleTag);
		}
		else if (update.fieldId == 1U)
		{
			string stringVal = update.stringVal;
			if (stringVal == null)
			{
				Error.AddDevFatal("BnetPresenceMgr.UpdateAccount() - Failed to convert full name to native string for {0}.", new object[]
				{
					account
				});
				return;
			}
			if (stringVal == account.GetFullName())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			account.SetFullName(stringVal);
		}
		else if (update.fieldId == 6U)
		{
			ulong intVal2 = (ulong)update.intVal;
			if (intVal2 == account.GetLastOnlineMicrosec())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			account.SetLastOnlineMicrosec(intVal2);
		}
		else if (update.fieldId == 3U)
		{
		}
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00032678 File Offset: 0x00030878
	private void OnGameAccountUpdate(PresenceUpdate update, BnetPlayerChangelist changelist)
	{
		BnetGameAccountId bnetGameAccountId = BnetGameAccountId.CreateFromEntityId(update.entityId);
		BnetGameAccount gameAccount = null;
		if (!this.m_gameAccounts.TryGetValue(bnetGameAccountId, out gameAccount))
		{
			this.CreateGameAccount(bnetGameAccountId, update, changelist);
		}
		else
		{
			this.UpdateGameAccount(gameAccount, update, changelist);
		}
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x000326C0 File Offset: 0x000308C0
	private void CreateGameAccount(BnetGameAccountId id, PresenceUpdate update, BnetPlayerChangelist changelist)
	{
		BnetGameAccount bnetGameAccount = new BnetGameAccount();
		this.m_gameAccounts.Add(id, bnetGameAccount);
		bnetGameAccount.SetId(id);
		this.UpdateGameAccount(bnetGameAccount, update, changelist);
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x000326F0 File Offset: 0x000308F0
	private void UpdateGameAccount(BnetGameAccount gameAccount, PresenceUpdate update, BnetPlayerChangelist changelist)
	{
		BnetPlayer player = null;
		BnetAccountId ownerId = gameAccount.GetOwnerId();
		if (ownerId != null)
		{
			this.m_players.TryGetValue(ownerId, out player);
		}
		if (update.fieldId == 2U)
		{
			int num = (!gameAccount.IsBusy()) ? 0 : 1;
			int num2 = (int)update.intVal;
			if (num2 == num)
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			bool flag = num2 == 1;
			gameAccount.SetBusy(flag);
			if (flag)
			{
				gameAccount.SetAway(false);
			}
			this.HandleGameAccountChange(player, update);
		}
		else if (update.fieldId == 10U)
		{
			bool boolVal = update.boolVal;
			if (boolVal == gameAccount.IsAway())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetAway(boolVal);
			if (boolVal)
			{
				gameAccount.SetBusy(false);
			}
			this.HandleGameAccountChange(player, update);
		}
		else if (update.fieldId == 11U)
		{
			ulong intVal = (ulong)update.intVal;
			if (intVal == gameAccount.GetAwayTimeMicrosec())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetAwayTimeMicrosec(intVal);
			this.HandleGameAccountChange(player, update);
		}
		else if (update.fieldId == 5U)
		{
			BnetBattleTag bnetBattleTag = BnetBattleTag.CreateFromString(update.stringVal);
			if (bnetBattleTag == gameAccount.GetBattleTag())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetBattleTag(bnetBattleTag);
			this.HandleGameAccountChange(player, update);
		}
		else if (update.fieldId == 1U)
		{
			bool boolVal2 = update.boolVal;
			if (boolVal2 == gameAccount.IsOnline())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetOnline(boolVal2);
			this.HandleGameAccountChange(player, update);
		}
		else if (update.fieldId == 3U)
		{
			BnetProgramId bnetProgramId = new BnetProgramId(update.stringVal);
			if (bnetProgramId == gameAccount.GetProgramId())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetProgramId(bnetProgramId);
			this.HandleGameAccountChange(player, update);
		}
		else if (update.fieldId == 4U)
		{
			ulong intVal2 = (ulong)update.intVal;
			if (intVal2 == gameAccount.GetLastOnlineMicrosec())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetLastOnlineMicrosec(intVal2);
			this.HandleGameAccountChange(player, update);
		}
		else if (update.fieldId == 7U)
		{
			BnetAccountId bnetAccountId = BnetAccountId.CreateFromEntityId(update.entityIdVal);
			if (bnetAccountId == gameAccount.GetOwnerId())
			{
				return;
			}
			this.UpdateGameAccountOwner(bnetAccountId, gameAccount, changelist);
		}
		else if (update.fieldId == 9U)
		{
			if (!update.valCleared)
			{
				return;
			}
			if (gameAccount.GetRichPresence() == null)
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetRichPresence(null);
			this.HandleGameAccountChange(player, update);
		}
		else if (update.fieldId == 1000U)
		{
			string text = update.stringVal;
			if (text == null)
			{
				text = string.Empty;
			}
			if (text == gameAccount.GetRichPresence())
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetRichPresence(text);
			this.HandleGameAccountChange(player, update);
		}
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00032A00 File Offset: 0x00030C00
	private void UpdateGameAccountOwner(BnetAccountId ownerId, BnetGameAccount gameAccount, BnetPlayerChangelist changelist)
	{
		BnetPlayer bnetPlayer = null;
		BnetAccountId ownerId2 = gameAccount.GetOwnerId();
		if (ownerId2 != null && this.m_players.TryGetValue(ownerId2, out bnetPlayer))
		{
			bnetPlayer.RemoveGameAccount(gameAccount.GetId());
			this.AddChangedPlayer(bnetPlayer, changelist);
		}
		BnetPlayer bnetPlayer2 = null;
		if (this.m_players.TryGetValue(ownerId, out bnetPlayer2))
		{
			this.AddChangedPlayer(bnetPlayer2, changelist);
		}
		else
		{
			bnetPlayer2 = new BnetPlayer();
			this.m_players.Add(ownerId, bnetPlayer2);
			BnetPlayerChange bnetPlayerChange = new BnetPlayerChange();
			bnetPlayerChange.SetNewPlayer(bnetPlayer2);
			changelist.AddChange(bnetPlayerChange);
		}
		gameAccount.SetOwnerId(ownerId);
		bnetPlayer2.AddGameAccount(gameAccount);
		this.CacheMyself(gameAccount, bnetPlayer2);
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x00032AA9 File Offset: 0x00030CA9
	private void HandleGameAccountChange(BnetPlayer player, PresenceUpdate update)
	{
		if (player == null)
		{
			return;
		}
		player.OnGameAccountChanged(update.fieldId);
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x00032AC0 File Offset: 0x00030CC0
	private void OnGameUpdate(PresenceUpdate update, BnetPlayerChangelist changelist)
	{
		BnetGameAccountId bnetGameAccountId = BnetGameAccountId.CreateFromEntityId(update.entityId);
		BnetGameAccount gameAccount = null;
		if (!this.m_gameAccounts.TryGetValue(bnetGameAccountId, out gameAccount))
		{
			this.CreateGameInfo(bnetGameAccountId, update, changelist);
		}
		else
		{
			this.UpdateGameInfo(gameAccount, update, changelist);
		}
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x00032B08 File Offset: 0x00030D08
	private void CreateGameInfo(BnetGameAccountId id, PresenceUpdate update, BnetPlayerChangelist changelist)
	{
		BnetGameAccount bnetGameAccount = new BnetGameAccount();
		this.m_gameAccounts.Add(id, bnetGameAccount);
		bnetGameAccount.SetId(id);
		this.UpdateGameInfo(bnetGameAccount, update, changelist);
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x00032B38 File Offset: 0x00030D38
	private void UpdateGameInfo(BnetGameAccount gameAccount, PresenceUpdate update, BnetPlayerChangelist changelist)
	{
		BnetPlayer player = null;
		BnetAccountId ownerId = gameAccount.GetOwnerId();
		if (ownerId != null)
		{
			this.m_players.TryGetValue(ownerId, out player);
		}
		if (update.valCleared)
		{
			if (gameAccount.HasGameField(update.fieldId))
			{
				this.AddChangedPlayer(player, changelist);
				gameAccount.RemoveGameField(update.fieldId);
				this.HandleGameAccountChange(player, update);
			}
			return;
		}
		switch (update.fieldId)
		{
		case 1U:
			if (update.boolVal == gameAccount.GetGameFieldBool(update.fieldId))
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetGameField(update.fieldId, update.boolVal);
			this.HandleGameAccountChange(player, update);
			break;
		case 2U:
		case 3U:
		case 4U:
		case 19U:
		case 20U:
			if (update.stringVal == gameAccount.GetGameFieldString(update.fieldId))
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetGameField(update.fieldId, update.stringVal);
			this.HandleGameAccountChange(player, update);
			break;
		case 5U:
		case 6U:
		case 7U:
		case 8U:
		case 9U:
		case 10U:
		case 11U:
		case 12U:
		case 13U:
		case 14U:
		case 15U:
		case 16U:
			if ((int)update.intVal == gameAccount.GetGameFieldInt(update.fieldId))
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetGameField(update.fieldId, (int)update.intVal);
			this.HandleGameAccountChange(player, update);
			break;
		case 17U:
		case 18U:
		case 21U:
			if (GeneralUtils.AreBytesEqual(update.blobVal, gameAccount.GetGameFieldBytes(update.fieldId)))
			{
				return;
			}
			this.AddChangedPlayer(player, changelist);
			gameAccount.SetGameField(update.fieldId, update.blobVal);
			this.HandleGameAccountChange(player, update);
			break;
		}
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x00032D31 File Offset: 0x00030F31
	private void CacheMyself(BnetGameAccount gameAccount, BnetPlayer player)
	{
		if (player == this.m_myPlayer)
		{
			return;
		}
		if (gameAccount.GetId() != this.m_myGameAccountId)
		{
			return;
		}
		this.m_myPlayer = player;
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x00032D60 File Offset: 0x00030F60
	private void AddChangedPlayer(BnetPlayer player, BnetPlayerChangelist changelist)
	{
		if (player == null)
		{
			return;
		}
		if (changelist.HasChange(player))
		{
			return;
		}
		BnetPlayerChange bnetPlayerChange = new BnetPlayerChange();
		bnetPlayerChange.SetOldPlayer(player.Clone());
		bnetPlayerChange.SetNewPlayer(player);
		changelist.AddChange(bnetPlayerChange);
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x00032DA4 File Offset: 0x00030FA4
	private void FirePlayersChangedEvent(BnetPlayerChangelist changelist)
	{
		if (changelist == null)
		{
			return;
		}
		if (changelist.GetChanges().Count == 0)
		{
			return;
		}
		BnetPresenceMgr.PlayersChangedListener[] array = this.m_playersChangedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire(changelist);
		}
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00032DF4 File Offset: 0x00030FF4
	private bool ShouldUpdateGameField(uint fieldId, object val, out BnetGameAccount hsGameAccount)
	{
		hsGameAccount = null;
		if (this.m_myPlayer == null)
		{
			return true;
		}
		hsGameAccount = this.m_myPlayer.GetHearthstoneGameAccount();
		if (hsGameAccount == null)
		{
			return true;
		}
		if (hsGameAccount.HasGameField(fieldId))
		{
			object gameField = hsGameAccount.GetGameField(fieldId);
			if (val == null)
			{
				if (gameField == null)
				{
					return false;
				}
			}
			else if (val.Equals(gameField))
			{
				return false;
			}
		}
		else if (val == null)
		{
			return false;
		}
		return true;
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x00032E74 File Offset: 0x00031074
	private bool ShouldUpdateGameFieldBlob(uint fieldId, byte[] val, out BnetGameAccount hsGameAccount)
	{
		hsGameAccount = null;
		if (this.m_myPlayer == null)
		{
			return true;
		}
		hsGameAccount = this.m_myPlayer.GetHearthstoneGameAccount();
		if (hsGameAccount == null)
		{
			return true;
		}
		if (hsGameAccount.HasGameField(fieldId))
		{
			byte[] gameFieldBytes = hsGameAccount.GetGameFieldBytes(fieldId);
			if (GeneralUtils.AreArraysEqual<byte>(val, gameFieldBytes))
			{
				return false;
			}
		}
		else if (val == null)
		{
			return false;
		}
		return true;
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x00032EE0 File Offset: 0x000310E0
	private BnetPlayerChangelist ChangeGameField(BnetGameAccount hsGameAccount, uint fieldId, object val)
	{
		if (hsGameAccount == null)
		{
			return null;
		}
		BnetPlayerChange bnetPlayerChange = new BnetPlayerChange();
		bnetPlayerChange.SetOldPlayer(this.m_myPlayer.Clone());
		bnetPlayerChange.SetNewPlayer(this.m_myPlayer);
		hsGameAccount.SetGameField(fieldId, val);
		BnetPlayerChangelist bnetPlayerChangelist = new BnetPlayerChangelist();
		bnetPlayerChangelist.AddChange(bnetPlayerChange);
		return bnetPlayerChangelist;
	}

	// Token: 0x040006F4 RID: 1780
	private static BnetPresenceMgr s_instance;

	// Token: 0x040006F5 RID: 1781
	private Map<BnetAccountId, BnetAccount> m_accounts = new Map<BnetAccountId, BnetAccount>();

	// Token: 0x040006F6 RID: 1782
	private Map<BnetGameAccountId, BnetGameAccount> m_gameAccounts = new Map<BnetGameAccountId, BnetGameAccount>();

	// Token: 0x040006F7 RID: 1783
	private Map<BnetAccountId, BnetPlayer> m_players = new Map<BnetAccountId, BnetPlayer>();

	// Token: 0x040006F8 RID: 1784
	private BnetGameAccountId m_myGameAccountId;

	// Token: 0x040006F9 RID: 1785
	private BnetPlayer m_myPlayer;

	// Token: 0x040006FA RID: 1786
	private List<BnetPresenceMgr.PlayersChangedListener> m_playersChangedListeners = new List<BnetPresenceMgr.PlayersChangedListener>();

	// Token: 0x020004DA RID: 1242
	// (Invoke) Token: 0x06003A89 RID: 14985
	public delegate void PlayersChangedCallback(BnetPlayerChangelist changelist, object userData);

	// Token: 0x02000549 RID: 1353
	private class PlayersChangedListener : EventListener<BnetPresenceMgr.PlayersChangedCallback>
	{
		// Token: 0x06003E54 RID: 15956 RVA: 0x0012DE7A File Offset: 0x0012C07A
		public void Fire(BnetPlayerChangelist changelist)
		{
			this.m_callback(changelist, this.m_userData);
		}
	}
}
