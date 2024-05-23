using System;
using System.Collections.Generic;
using bgs;
using SpectatorProto;

// Token: 0x020004B0 RID: 1200
public class BnetGameAccount
{
	// Token: 0x06003946 RID: 14662 RVA: 0x00116E70 File Offset: 0x00115070
	public BnetGameAccount Clone()
	{
		BnetGameAccount bnetGameAccount = (BnetGameAccount)base.MemberwiseClone();
		if (this.m_id != null)
		{
			bnetGameAccount.m_id = this.m_id.Clone();
		}
		if (this.m_ownerId != null)
		{
			bnetGameAccount.m_ownerId = this.m_ownerId.Clone();
		}
		if (this.m_programId != null)
		{
			bnetGameAccount.m_programId = this.m_programId.Clone();
		}
		if (this.m_battleTag != null)
		{
			bnetGameAccount.m_battleTag = this.m_battleTag.Clone();
		}
		bnetGameAccount.m_gameFields = new Map<uint, object>();
		foreach (KeyValuePair<uint, object> keyValuePair in this.m_gameFields)
		{
			bnetGameAccount.m_gameFields.Add(keyValuePair.Key, keyValuePair.Value);
		}
		return bnetGameAccount;
	}

	// Token: 0x06003947 RID: 14663 RVA: 0x00116F7C File Offset: 0x0011517C
	public BnetGameAccountId GetId()
	{
		return this.m_id;
	}

	// Token: 0x06003948 RID: 14664 RVA: 0x00116F84 File Offset: 0x00115184
	public void SetId(BnetGameAccountId id)
	{
		this.m_id = id;
	}

	// Token: 0x06003949 RID: 14665 RVA: 0x00116F8D File Offset: 0x0011518D
	public BnetAccountId GetOwnerId()
	{
		return this.m_ownerId;
	}

	// Token: 0x0600394A RID: 14666 RVA: 0x00116F95 File Offset: 0x00115195
	public void SetOwnerId(BnetAccountId id)
	{
		this.m_ownerId = id;
	}

	// Token: 0x0600394B RID: 14667 RVA: 0x00116F9E File Offset: 0x0011519E
	public BnetProgramId GetProgramId()
	{
		return this.m_programId;
	}

	// Token: 0x0600394C RID: 14668 RVA: 0x00116FA6 File Offset: 0x001151A6
	public void SetProgramId(BnetProgramId programId)
	{
		this.m_programId = programId;
	}

	// Token: 0x0600394D RID: 14669 RVA: 0x00116FAF File Offset: 0x001151AF
	public BnetBattleTag GetBattleTag()
	{
		return this.m_battleTag;
	}

	// Token: 0x0600394E RID: 14670 RVA: 0x00116FB7 File Offset: 0x001151B7
	public void SetBattleTag(BnetBattleTag battleTag)
	{
		this.m_battleTag = battleTag;
	}

	// Token: 0x0600394F RID: 14671 RVA: 0x00116FC0 File Offset: 0x001151C0
	public bool IsAway()
	{
		return this.m_away;
	}

	// Token: 0x06003950 RID: 14672 RVA: 0x00116FC8 File Offset: 0x001151C8
	public void SetAway(bool away)
	{
		this.m_away = away;
	}

	// Token: 0x06003951 RID: 14673 RVA: 0x00116FD1 File Offset: 0x001151D1
	public ulong GetAwayTimeMicrosec()
	{
		return this.m_awayTimeMicrosec;
	}

	// Token: 0x06003952 RID: 14674 RVA: 0x00116FD9 File Offset: 0x001151D9
	public void SetAwayTimeMicrosec(ulong awayTimeMicrosec)
	{
		this.m_awayTimeMicrosec = awayTimeMicrosec;
	}

	// Token: 0x06003953 RID: 14675 RVA: 0x00116FE2 File Offset: 0x001151E2
	public bool IsBusy()
	{
		return this.m_busy;
	}

	// Token: 0x06003954 RID: 14676 RVA: 0x00116FEA File Offset: 0x001151EA
	public void SetBusy(bool busy)
	{
		this.m_busy = busy;
	}

	// Token: 0x06003955 RID: 14677 RVA: 0x00116FF3 File Offset: 0x001151F3
	public bool IsOnline()
	{
		return this.m_online;
	}

	// Token: 0x06003956 RID: 14678 RVA: 0x00116FFB File Offset: 0x001151FB
	public void SetOnline(bool online)
	{
		this.m_online = online;
	}

	// Token: 0x06003957 RID: 14679 RVA: 0x00117004 File Offset: 0x00115204
	public ulong GetLastOnlineMicrosec()
	{
		return this.m_lastOnlineMicrosec;
	}

	// Token: 0x06003958 RID: 14680 RVA: 0x0011700C File Offset: 0x0011520C
	public void SetLastOnlineMicrosec(ulong microsec)
	{
		this.m_lastOnlineMicrosec = microsec;
	}

	// Token: 0x06003959 RID: 14681 RVA: 0x00117015 File Offset: 0x00115215
	public string GetRichPresence()
	{
		return this.m_richPresence;
	}

	// Token: 0x0600395A RID: 14682 RVA: 0x0011701D File Offset: 0x0011521D
	public void SetRichPresence(string richPresence)
	{
		this.m_richPresence = richPresence;
	}

	// Token: 0x0600395B RID: 14683 RVA: 0x00117026 File Offset: 0x00115226
	public Map<uint, object> GetGameFields()
	{
		return this.m_gameFields;
	}

	// Token: 0x0600395C RID: 14684 RVA: 0x0011702E File Offset: 0x0011522E
	public bool HasGameField(uint fieldId)
	{
		return this.m_gameFields.ContainsKey(fieldId);
	}

	// Token: 0x0600395D RID: 14685 RVA: 0x0011703C File Offset: 0x0011523C
	public void SetGameField(uint fieldId, object val)
	{
		this.m_gameFields[fieldId] = val;
	}

	// Token: 0x0600395E RID: 14686 RVA: 0x0011704B File Offset: 0x0011524B
	public bool RemoveGameField(uint fieldId)
	{
		return this.m_gameFields.Remove(fieldId);
	}

	// Token: 0x0600395F RID: 14687 RVA: 0x00117059 File Offset: 0x00115259
	public bool TryGetGameField(uint fieldId, out object val)
	{
		return this.m_gameFields.TryGetValue(fieldId, out val);
	}

	// Token: 0x06003960 RID: 14688 RVA: 0x00117068 File Offset: 0x00115268
	public bool TryGetGameFieldBool(uint fieldId, out bool val)
	{
		val = false;
		object obj = null;
		if (!this.m_gameFields.TryGetValue(fieldId, out obj))
		{
			return false;
		}
		val = (bool)obj;
		return true;
	}

	// Token: 0x06003961 RID: 14689 RVA: 0x00117098 File Offset: 0x00115298
	public bool TryGetGameFieldInt(uint fieldId, out int val)
	{
		val = 0;
		object obj = null;
		if (!this.m_gameFields.TryGetValue(fieldId, out obj))
		{
			return false;
		}
		val = (int)obj;
		return true;
	}

	// Token: 0x06003962 RID: 14690 RVA: 0x001170C8 File Offset: 0x001152C8
	public bool TryGetGameFieldString(uint fieldId, out string val)
	{
		val = null;
		object obj = null;
		if (!this.m_gameFields.TryGetValue(fieldId, out obj))
		{
			return false;
		}
		val = (string)obj;
		return true;
	}

	// Token: 0x06003963 RID: 14691 RVA: 0x001170F8 File Offset: 0x001152F8
	public bool TryGetGameFieldBytes(uint fieldId, out byte[] val)
	{
		val = null;
		object obj = null;
		if (!this.m_gameFields.TryGetValue(fieldId, out obj))
		{
			return false;
		}
		val = (byte[])obj;
		return true;
	}

	// Token: 0x06003964 RID: 14692 RVA: 0x00117128 File Offset: 0x00115328
	public object GetGameField(uint fieldId)
	{
		object result = null;
		this.m_gameFields.TryGetValue(fieldId, out result);
		return result;
	}

	// Token: 0x06003965 RID: 14693 RVA: 0x00117148 File Offset: 0x00115348
	public bool GetGameFieldBool(uint fieldId)
	{
		object obj = null;
		return this.m_gameFields.TryGetValue(fieldId, out obj) && (bool)obj;
	}

	// Token: 0x06003966 RID: 14694 RVA: 0x00117174 File Offset: 0x00115374
	public int GetGameFieldInt(uint fieldId)
	{
		object obj = null;
		if (this.m_gameFields.TryGetValue(fieldId, out obj))
		{
			return (int)obj;
		}
		return 0;
	}

	// Token: 0x06003967 RID: 14695 RVA: 0x001171A0 File Offset: 0x001153A0
	public string GetGameFieldString(uint fieldId)
	{
		object obj = null;
		if (this.m_gameFields.TryGetValue(fieldId, out obj))
		{
			return (string)obj;
		}
		return null;
	}

	// Token: 0x06003968 RID: 14696 RVA: 0x001171CC File Offset: 0x001153CC
	public byte[] GetGameFieldBytes(uint fieldId)
	{
		object obj = null;
		if (this.m_gameFields.TryGetValue(fieldId, out obj))
		{
			return (byte[])obj;
		}
		return null;
	}

	// Token: 0x06003969 RID: 14697 RVA: 0x001171F6 File Offset: 0x001153F6
	public bool CanBeInvitedToGame()
	{
		return this.GetGameFieldBool(1U);
	}

	// Token: 0x0600396A RID: 14698 RVA: 0x001171FF File Offset: 0x001153FF
	public string GetClientVersion()
	{
		return this.GetGameFieldString(19U);
	}

	// Token: 0x0600396B RID: 14699 RVA: 0x00117209 File Offset: 0x00115409
	public string GetClientEnv()
	{
		return this.GetGameFieldString(20U);
	}

	// Token: 0x0600396C RID: 14700 RVA: 0x00117213 File Offset: 0x00115413
	public string GetDebugString()
	{
		return this.GetGameFieldString(2U);
	}

	// Token: 0x0600396D RID: 14701 RVA: 0x0011721C File Offset: 0x0011541C
	public string GetArenaRecord()
	{
		return this.GetGameFieldString(3U);
	}

	// Token: 0x0600396E RID: 14702 RVA: 0x00117225 File Offset: 0x00115425
	public string GetCardsOpened()
	{
		return this.GetGameFieldString(4U);
	}

	// Token: 0x0600396F RID: 14703 RVA: 0x0011722E File Offset: 0x0011542E
	public int GetDruidLevel()
	{
		return this.GetGameFieldInt(5U);
	}

	// Token: 0x06003970 RID: 14704 RVA: 0x00117237 File Offset: 0x00115437
	public int GetHunterLevel()
	{
		return this.GetGameFieldInt(6U);
	}

	// Token: 0x06003971 RID: 14705 RVA: 0x00117240 File Offset: 0x00115440
	public int GetMageLevel()
	{
		return this.GetGameFieldInt(7U);
	}

	// Token: 0x06003972 RID: 14706 RVA: 0x00117249 File Offset: 0x00115449
	public int GetPaladinLevel()
	{
		return this.GetGameFieldInt(8U);
	}

	// Token: 0x06003973 RID: 14707 RVA: 0x00117252 File Offset: 0x00115452
	public int GetPriestLevel()
	{
		return this.GetGameFieldInt(9U);
	}

	// Token: 0x06003974 RID: 14708 RVA: 0x0011725C File Offset: 0x0011545C
	public int GetRogueLevel()
	{
		return this.GetGameFieldInt(10U);
	}

	// Token: 0x06003975 RID: 14709 RVA: 0x00117266 File Offset: 0x00115466
	public int GetShamanLevel()
	{
		return this.GetGameFieldInt(11U);
	}

	// Token: 0x06003976 RID: 14710 RVA: 0x00117270 File Offset: 0x00115470
	public int GetWarlockLevel()
	{
		return this.GetGameFieldInt(12U);
	}

	// Token: 0x06003977 RID: 14711 RVA: 0x0011727A File Offset: 0x0011547A
	public int GetWarriorLevel()
	{
		return this.GetGameFieldInt(13U);
	}

	// Token: 0x06003978 RID: 14712 RVA: 0x00117284 File Offset: 0x00115484
	public int GetGainMedal()
	{
		return this.GetGameFieldInt(14U);
	}

	// Token: 0x06003979 RID: 14713 RVA: 0x0011728E File Offset: 0x0011548E
	public int GetTutorialBeaten()
	{
		return this.GetGameFieldInt(15U);
	}

	// Token: 0x0600397A RID: 14714 RVA: 0x00117298 File Offset: 0x00115498
	public int GetCollectionEvent()
	{
		return this.GetGameFieldInt(16U);
	}

	// Token: 0x0600397B RID: 14715 RVA: 0x001172A4 File Offset: 0x001154A4
	public JoinInfo GetSpectatorJoinInfo()
	{
		byte[] gameFieldBytes = this.GetGameFieldBytes(21U);
		if (gameFieldBytes != null && gameFieldBytes.Length > 0)
		{
			return ProtobufUtil.ParseFrom<JoinInfo>(gameFieldBytes, 0, -1);
		}
		return null;
	}

	// Token: 0x0600397C RID: 14716 RVA: 0x001172D8 File Offset: 0x001154D8
	public bool IsSpectatorSlotAvailable()
	{
		JoinInfo spectatorJoinInfo = this.GetSpectatorJoinInfo();
		return BnetGameAccount.IsSpectatorSlotAvailable(spectatorJoinInfo);
	}

	// Token: 0x0600397D RID: 14717 RVA: 0x001172F4 File Offset: 0x001154F4
	public static bool IsSpectatorSlotAvailable(JoinInfo info)
	{
		if (info == null)
		{
			return false;
		}
		if (!info.HasPartyId)
		{
			if (!info.HasServerIpAddress || !info.HasSecretKey)
			{
				return false;
			}
			if (string.IsNullOrEmpty(info.SecretKey))
			{
				return false;
			}
		}
		return (!info.HasIsJoinable || info.IsJoinable) && (!info.HasMaxNumSpectators || !info.HasCurrentNumSpectators || info.CurrentNumSpectators < info.MaxNumSpectators);
	}

	// Token: 0x0600397E RID: 14718 RVA: 0x00117380 File Offset: 0x00115580
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		BnetGameAccount bnetGameAccount = obj as BnetGameAccount;
		return bnetGameAccount != null && this.m_id.Equals(bnetGameAccount.m_id);
	}

	// Token: 0x0600397F RID: 14719 RVA: 0x001173B5 File Offset: 0x001155B5
	public bool Equals(BnetGameAccountId other)
	{
		return other != null && this.m_id.Equals(other);
	}

	// Token: 0x06003980 RID: 14720 RVA: 0x001173CB File Offset: 0x001155CB
	public override int GetHashCode()
	{
		return this.m_id.GetHashCode();
	}

	// Token: 0x06003981 RID: 14721 RVA: 0x001173D8 File Offset: 0x001155D8
	public override string ToString()
	{
		if (this.m_id == null)
		{
			return "UNKNOWN GAME ACCOUNT";
		}
		return string.Format("[id={0} programId={1} battleTag={2} online={3}]", new object[]
		{
			this.m_id,
			this.m_programId,
			this.m_battleTag,
			this.m_online
		});
	}

	// Token: 0x06003982 RID: 14722 RVA: 0x00117435 File Offset: 0x00115635
	public static bool operator ==(BnetGameAccount a, BnetGameAccount b)
	{
		return object.ReferenceEquals(a, b) || (a != null && b != null && a.m_id == b.m_id);
	}

	// Token: 0x06003983 RID: 14723 RVA: 0x00117464 File Offset: 0x00115664
	public static bool operator !=(BnetGameAccount a, BnetGameAccount b)
	{
		return !(a == b);
	}

	// Token: 0x040024B2 RID: 9394
	private BnetGameAccountId m_id;

	// Token: 0x040024B3 RID: 9395
	private BnetAccountId m_ownerId;

	// Token: 0x040024B4 RID: 9396
	private BnetProgramId m_programId;

	// Token: 0x040024B5 RID: 9397
	private BnetBattleTag m_battleTag;

	// Token: 0x040024B6 RID: 9398
	private bool m_away;

	// Token: 0x040024B7 RID: 9399
	private ulong m_awayTimeMicrosec;

	// Token: 0x040024B8 RID: 9400
	private bool m_busy;

	// Token: 0x040024B9 RID: 9401
	private bool m_online;

	// Token: 0x040024BA RID: 9402
	private ulong m_lastOnlineMicrosec;

	// Token: 0x040024BB RID: 9403
	private string m_richPresence;

	// Token: 0x040024BC RID: 9404
	private Map<uint, object> m_gameFields = new Map<uint, object>();
}
