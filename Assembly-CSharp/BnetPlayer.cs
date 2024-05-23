using System;
using System.Collections.Generic;
using bgs;

// Token: 0x0200038D RID: 909
public class BnetPlayer
{
	// Token: 0x06002F83 RID: 12163 RVA: 0x000EF210 File Offset: 0x000ED410
	public BnetPlayer Clone()
	{
		BnetPlayer bnetPlayer = (BnetPlayer)base.MemberwiseClone();
		if (this.m_accountId != null)
		{
			bnetPlayer.m_accountId = this.m_accountId.Clone();
		}
		if (this.m_account != null)
		{
			bnetPlayer.m_account = this.m_account.Clone();
		}
		if (this.m_hsGameAccount != null)
		{
			bnetPlayer.m_hsGameAccount = this.m_hsGameAccount.Clone();
		}
		if (this.m_bestGameAccount != null)
		{
			bnetPlayer.m_bestGameAccount = this.m_bestGameAccount.Clone();
		}
		bnetPlayer.m_gameAccounts = new Map<BnetGameAccountId, BnetGameAccount>();
		foreach (KeyValuePair<BnetGameAccountId, BnetGameAccount> keyValuePair in this.m_gameAccounts)
		{
			bnetPlayer.m_gameAccounts.Add(keyValuePair.Key.Clone(), keyValuePair.Value.Clone());
		}
		return bnetPlayer;
	}

	// Token: 0x06002F84 RID: 12164 RVA: 0x000EF328 File Offset: 0x000ED528
	public BnetAccountId GetAccountId()
	{
		if (this.m_accountId != null)
		{
			return this.m_accountId;
		}
		BnetGameAccount firstGameAccount = this.GetFirstGameAccount();
		if (firstGameAccount != null)
		{
			return firstGameAccount.GetOwnerId();
		}
		return null;
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x000EF368 File Offset: 0x000ED568
	public void SetAccountId(BnetAccountId accountId)
	{
		this.m_accountId = accountId;
	}

	// Token: 0x06002F86 RID: 12166 RVA: 0x000EF371 File Offset: 0x000ED571
	public BnetAccount GetAccount()
	{
		return this.m_account;
	}

	// Token: 0x06002F87 RID: 12167 RVA: 0x000EF379 File Offset: 0x000ED579
	public void SetAccount(BnetAccount account)
	{
		this.m_account = account;
		this.m_accountId = account.GetId();
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x000EF390 File Offset: 0x000ED590
	public string GetFullName()
	{
		return (!(this.m_account == null)) ? this.m_account.GetFullName() : null;
	}

	// Token: 0x06002F89 RID: 12169 RVA: 0x000EF3C0 File Offset: 0x000ED5C0
	public BnetBattleTag GetBattleTag()
	{
		if (this.m_account != null && this.m_account.GetBattleTag() != null)
		{
			return this.m_account.GetBattleTag();
		}
		BnetGameAccount firstGameAccount = this.GetFirstGameAccount();
		if (firstGameAccount != null)
		{
			return firstGameAccount.GetBattleTag();
		}
		return null;
	}

	// Token: 0x06002F8A RID: 12170 RVA: 0x000EF41C File Offset: 0x000ED61C
	public BnetGameAccount GetGameAccount(BnetGameAccountId id)
	{
		BnetGameAccount result = null;
		this.m_gameAccounts.TryGetValue(id, out result);
		return result;
	}

	// Token: 0x06002F8B RID: 12171 RVA: 0x000EF43B File Offset: 0x000ED63B
	public Map<BnetGameAccountId, BnetGameAccount> GetGameAccounts()
	{
		return this.m_gameAccounts;
	}

	// Token: 0x06002F8C RID: 12172 RVA: 0x000EF443 File Offset: 0x000ED643
	public bool HasGameAccount(BnetGameAccountId id)
	{
		return this.m_gameAccounts.ContainsKey(id);
	}

	// Token: 0x06002F8D RID: 12173 RVA: 0x000EF454 File Offset: 0x000ED654
	public void AddGameAccount(BnetGameAccount gameAccount)
	{
		BnetGameAccountId id = gameAccount.GetId();
		if (this.m_gameAccounts.ContainsKey(id))
		{
			return;
		}
		this.m_gameAccounts.Add(id, gameAccount);
		this.CacheSpecialGameAccounts();
	}

	// Token: 0x06002F8E RID: 12174 RVA: 0x000EF48D File Offset: 0x000ED68D
	public bool RemoveGameAccount(BnetGameAccountId id)
	{
		if (!this.m_gameAccounts.Remove(id))
		{
			return false;
		}
		this.CacheSpecialGameAccounts();
		return true;
	}

	// Token: 0x06002F8F RID: 12175 RVA: 0x000EF4A9 File Offset: 0x000ED6A9
	public BnetGameAccount GetHearthstoneGameAccount()
	{
		return this.m_hsGameAccount;
	}

	// Token: 0x06002F90 RID: 12176 RVA: 0x000EF4B1 File Offset: 0x000ED6B1
	public BnetGameAccountId GetHearthstoneGameAccountId()
	{
		if (this.m_hsGameAccount == null)
		{
			return null;
		}
		return this.m_hsGameAccount.GetId();
	}

	// Token: 0x06002F91 RID: 12177 RVA: 0x000EF4D1 File Offset: 0x000ED6D1
	public BnetGameAccount GetBestGameAccount()
	{
		return this.m_bestGameAccount;
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x000EF4D9 File Offset: 0x000ED6D9
	public BnetGameAccountId GetBestGameAccountId()
	{
		if (this.m_bestGameAccount == null)
		{
			return null;
		}
		return this.m_bestGameAccount.GetId();
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x000EF4F9 File Offset: 0x000ED6F9
	public bool IsDisplayable()
	{
		return this.GetBestName() != null;
	}

	// Token: 0x06002F94 RID: 12180 RVA: 0x000EF508 File Offset: 0x000ED708
	public BnetGameAccount GetFirstGameAccount()
	{
		using (Map<BnetGameAccountId, BnetGameAccount>.ValueCollection.Enumerator enumerator = this.m_gameAccounts.Values.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return null;
	}

	// Token: 0x06002F95 RID: 12181 RVA: 0x000EF56C File Offset: 0x000ED76C
	public long GetPersistentGameId()
	{
		return 0L;
	}

	// Token: 0x06002F96 RID: 12182 RVA: 0x000EF570 File Offset: 0x000ED770
	public string GetBestName()
	{
		if (this != BnetPresenceMgr.Get().GetMyPlayer())
		{
			if (this.m_account != null)
			{
				string fullName = this.m_account.GetFullName();
				if (fullName != null)
				{
					return fullName;
				}
				if (this.m_account.GetBattleTag() != null)
				{
					return this.m_account.GetBattleTag().GetName();
				}
			}
			foreach (KeyValuePair<BnetGameAccountId, BnetGameAccount> keyValuePair in this.m_gameAccounts)
			{
				if (keyValuePair.Value.GetBattleTag() != null)
				{
					return keyValuePair.Value.GetBattleTag().GetName();
				}
			}
			return null;
		}
		if (this.m_hsGameAccount == null)
		{
			return null;
		}
		return (!(this.m_hsGameAccount.GetBattleTag() == null)) ? this.m_hsGameAccount.GetBattleTag().GetName() : null;
	}

	// Token: 0x06002F97 RID: 12183 RVA: 0x000EF690 File Offset: 0x000ED890
	public BnetProgramId GetBestProgramId()
	{
		if (this.m_bestGameAccount == null)
		{
			return null;
		}
		return this.m_bestGameAccount.GetProgramId();
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x000EF6B0 File Offset: 0x000ED8B0
	public string GetBestRichPresence()
	{
		if (this.m_bestGameAccount == null)
		{
			return null;
		}
		return this.m_bestGameAccount.GetRichPresence();
	}

	// Token: 0x06002F99 RID: 12185 RVA: 0x000EF6D0 File Offset: 0x000ED8D0
	public bool IsOnline()
	{
		foreach (BnetGameAccount bnetGameAccount in this.m_gameAccounts.Values)
		{
			if (bnetGameAccount.IsOnline())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002F9A RID: 12186 RVA: 0x000EF740 File Offset: 0x000ED940
	public bool IsAway()
	{
		return (this.m_account != null && this.m_account.IsAway()) || (this.m_bestGameAccount != null && this.m_bestGameAccount.IsAway());
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x000EF794 File Offset: 0x000ED994
	public bool IsBusy()
	{
		return (this.m_account != null && this.m_account.IsBusy()) || (this.m_bestGameAccount != null && this.m_bestGameAccount.IsBusy());
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x000EF7E8 File Offset: 0x000ED9E8
	public ulong GetBestAwayTimeMicrosec()
	{
		ulong num = 0UL;
		if (this.m_account != null && this.m_account.IsAway())
		{
			num = Math.Max(this.m_account.GetAwayTimeMicrosec(), this.m_account.GetLastOnlineMicrosec());
			if (num != 0UL)
			{
				return num;
			}
		}
		if (this.m_bestGameAccount != null && this.m_bestGameAccount.IsAway())
		{
			num = Math.Max(this.m_bestGameAccount.GetAwayTimeMicrosec(), this.m_bestGameAccount.GetLastOnlineMicrosec());
			if (num != 0UL)
			{
				return num;
			}
		}
		return num;
	}

	// Token: 0x06002F9D RID: 12189 RVA: 0x000EF884 File Offset: 0x000EDA84
	public ulong GetBestLastOnlineMicrosec()
	{
		ulong num = 0UL;
		if (this.m_account != null)
		{
			num = this.m_account.GetLastOnlineMicrosec();
			if (num != 0UL)
			{
				return num;
			}
		}
		if (this.m_bestGameAccount != null)
		{
			num = this.m_bestGameAccount.GetLastOnlineMicrosec();
			if (num != 0UL)
			{
				return num;
			}
		}
		return num;
	}

	// Token: 0x06002F9E RID: 12190 RVA: 0x000EF8E0 File Offset: 0x000EDAE0
	public bool HasMultipleOnlineGameAccounts()
	{
		bool flag = false;
		foreach (BnetGameAccount bnetGameAccount in this.m_gameAccounts.Values)
		{
			if (bnetGameAccount.IsOnline())
			{
				if (flag)
				{
					return true;
				}
				flag = true;
			}
		}
		return false;
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x000EF95C File Offset: 0x000EDB5C
	public int GetNumOnlineGameAccounts()
	{
		int num = 0;
		foreach (BnetGameAccount bnetGameAccount in this.m_gameAccounts.Values)
		{
			if (bnetGameAccount.IsOnline())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x000EF9CC File Offset: 0x000EDBCC
	public List<BnetGameAccount> GetOnlineGameAccounts()
	{
		List<BnetGameAccount> list = new List<BnetGameAccount>();
		foreach (BnetGameAccount bnetGameAccount in this.m_gameAccounts.Values)
		{
			if (bnetGameAccount.IsOnline())
			{
				list.Add(bnetGameAccount);
			}
		}
		return list;
	}

	// Token: 0x06002FA1 RID: 12193 RVA: 0x000EFA44 File Offset: 0x000EDC44
	public bool HasAccount(BnetEntityId id)
	{
		if (id == null)
		{
			return false;
		}
		if (this.m_accountId == id)
		{
			return true;
		}
		foreach (BnetGameAccountId bnetGameAccountId in this.m_gameAccounts.Keys)
		{
			if (bnetGameAccountId == id)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002FA2 RID: 12194 RVA: 0x000EFAD4 File Offset: 0x000EDCD4
	public void OnGameAccountChanged(uint fieldId)
	{
		if (fieldId == 3U || fieldId == 1U || fieldId == 4U)
		{
			this.CacheSpecialGameAccounts();
		}
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x000EFAF4 File Offset: 0x000EDCF4
	public override string ToString()
	{
		BnetAccountId accountId = this.GetAccountId();
		BnetBattleTag battleTag = this.GetBattleTag();
		if (accountId == null && battleTag == null)
		{
			return "UNKNOWN PLAYER";
		}
		return string.Format("[account={0} battleTag={1} numGameAccounts={2}]", accountId, battleTag, this.m_gameAccounts.Count);
	}

	// Token: 0x06002FA4 RID: 12196 RVA: 0x000EFB4C File Offset: 0x000EDD4C
	private void CacheSpecialGameAccounts()
	{
		this.m_hsGameAccount = null;
		this.m_bestGameAccount = null;
		ulong num = 0UL;
		foreach (BnetGameAccount bnetGameAccount in this.m_gameAccounts.Values)
		{
			BnetProgramId programId = bnetGameAccount.GetProgramId();
			if (!(programId == null))
			{
				if (programId == BnetProgramId.HEARTHSTONE)
				{
					this.m_hsGameAccount = bnetGameAccount;
					if (bnetGameAccount.IsOnline() || !BnetFriendMgr.Get().IsFriend(bnetGameAccount.GetId()))
					{
						this.m_bestGameAccount = bnetGameAccount;
					}
					break;
				}
				if (this.m_bestGameAccount == null)
				{
					this.m_bestGameAccount = bnetGameAccount;
					num = this.m_bestGameAccount.GetLastOnlineMicrosec();
				}
				else
				{
					BnetProgramId programId2 = this.m_bestGameAccount.GetProgramId();
					if (programId.IsGame() && !programId2.IsGame())
					{
						this.m_bestGameAccount = bnetGameAccount;
						num = this.m_bestGameAccount.GetLastOnlineMicrosec();
					}
					else if (bnetGameAccount.IsOnline() && programId.IsGame() && programId2.IsGame())
					{
						ulong lastOnlineMicrosec = bnetGameAccount.GetLastOnlineMicrosec();
						if (lastOnlineMicrosec > num)
						{
							this.m_bestGameAccount = bnetGameAccount;
							num = lastOnlineMicrosec;
						}
					}
				}
			}
		}
	}

	// Token: 0x04001D88 RID: 7560
	private BnetAccountId m_accountId;

	// Token: 0x04001D89 RID: 7561
	private BnetAccount m_account;

	// Token: 0x04001D8A RID: 7562
	private Map<BnetGameAccountId, BnetGameAccount> m_gameAccounts = new Map<BnetGameAccountId, BnetGameAccount>();

	// Token: 0x04001D8B RID: 7563
	private BnetGameAccount m_hsGameAccount;

	// Token: 0x04001D8C RID: 7564
	private BnetGameAccount m_bestGameAccount;
}
