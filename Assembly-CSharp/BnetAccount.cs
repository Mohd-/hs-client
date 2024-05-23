using System;
using bgs;

// Token: 0x020004AF RID: 1199
public class BnetAccount
{
	// Token: 0x06003930 RID: 14640 RVA: 0x00116C90 File Offset: 0x00114E90
	public BnetAccount Clone()
	{
		BnetAccount bnetAccount = (BnetAccount)base.MemberwiseClone();
		if (this.m_id != null)
		{
			bnetAccount.m_id = this.m_id.Clone();
		}
		if (this.m_battleTag != null)
		{
			bnetAccount.m_battleTag = this.m_battleTag.Clone();
		}
		return bnetAccount;
	}

	// Token: 0x06003931 RID: 14641 RVA: 0x00116CEE File Offset: 0x00114EEE
	public BnetAccountId GetId()
	{
		return this.m_id;
	}

	// Token: 0x06003932 RID: 14642 RVA: 0x00116CF6 File Offset: 0x00114EF6
	public void SetId(BnetAccountId id)
	{
		this.m_id = id;
	}

	// Token: 0x06003933 RID: 14643 RVA: 0x00116CFF File Offset: 0x00114EFF
	public string GetFullName()
	{
		return this.m_fullName;
	}

	// Token: 0x06003934 RID: 14644 RVA: 0x00116D07 File Offset: 0x00114F07
	public void SetFullName(string fullName)
	{
		this.m_fullName = fullName;
	}

	// Token: 0x06003935 RID: 14645 RVA: 0x00116D10 File Offset: 0x00114F10
	public BnetBattleTag GetBattleTag()
	{
		return this.m_battleTag;
	}

	// Token: 0x06003936 RID: 14646 RVA: 0x00116D18 File Offset: 0x00114F18
	public void SetBattleTag(BnetBattleTag battleTag)
	{
		this.m_battleTag = battleTag;
	}

	// Token: 0x06003937 RID: 14647 RVA: 0x00116D21 File Offset: 0x00114F21
	public ulong GetLastOnlineMicrosec()
	{
		return this.m_lastOnlineMicrosec;
	}

	// Token: 0x06003938 RID: 14648 RVA: 0x00116D29 File Offset: 0x00114F29
	public void SetLastOnlineMicrosec(ulong microsec)
	{
		this.m_lastOnlineMicrosec = microsec;
	}

	// Token: 0x06003939 RID: 14649 RVA: 0x00116D32 File Offset: 0x00114F32
	public bool IsAway()
	{
		return this.m_away;
	}

	// Token: 0x0600393A RID: 14650 RVA: 0x00116D3A File Offset: 0x00114F3A
	public void SetAway(bool away)
	{
		this.m_away = away;
	}

	// Token: 0x0600393B RID: 14651 RVA: 0x00116D43 File Offset: 0x00114F43
	public ulong GetAwayTimeMicrosec()
	{
		return this.m_awayTimeMicrosec;
	}

	// Token: 0x0600393C RID: 14652 RVA: 0x00116D4B File Offset: 0x00114F4B
	public void SetAwayTimeMicrosec(ulong awayTimeMicrosec)
	{
		this.m_awayTimeMicrosec = awayTimeMicrosec;
	}

	// Token: 0x0600393D RID: 14653 RVA: 0x00116D54 File Offset: 0x00114F54
	public bool IsBusy()
	{
		return this.m_busy;
	}

	// Token: 0x0600393E RID: 14654 RVA: 0x00116D5C File Offset: 0x00114F5C
	public void SetBusy(bool busy)
	{
		this.m_busy = busy;
	}

	// Token: 0x0600393F RID: 14655 RVA: 0x00116D68 File Offset: 0x00114F68
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		BnetAccount bnetAccount = obj as BnetAccount;
		return bnetAccount != null && this.m_id.Equals(bnetAccount.m_id);
	}

	// Token: 0x06003940 RID: 14656 RVA: 0x00116D9D File Offset: 0x00114F9D
	public bool Equals(BnetAccountId other)
	{
		return other != null && this.m_id.Equals(other);
	}

	// Token: 0x06003941 RID: 14657 RVA: 0x00116DB3 File Offset: 0x00114FB3
	public override int GetHashCode()
	{
		return this.m_id.GetHashCode();
	}

	// Token: 0x06003942 RID: 14658 RVA: 0x00116DC0 File Offset: 0x00114FC0
	public override string ToString()
	{
		if (this.m_id == null)
		{
			return "UNKNOWN ACCOUNT";
		}
		return string.Format("[id={0} m_fullName={1} battleTag={2} lastOnline={3}]", new object[]
		{
			this.m_id,
			this.m_fullName,
			this.m_battleTag,
			TimeUtils.ConvertEpochMicrosecToDateTime(this.m_lastOnlineMicrosec)
		});
	}

	// Token: 0x06003943 RID: 14659 RVA: 0x00116E22 File Offset: 0x00115022
	public static bool operator ==(BnetAccount a, BnetAccount b)
	{
		return object.ReferenceEquals(a, b) || (a != null && b != null && a.m_id == b.m_id);
	}

	// Token: 0x06003944 RID: 14660 RVA: 0x00116E51 File Offset: 0x00115051
	public static bool operator !=(BnetAccount a, BnetAccount b)
	{
		return !(a == b);
	}

	// Token: 0x040024AB RID: 9387
	private BnetAccountId m_id;

	// Token: 0x040024AC RID: 9388
	private string m_fullName;

	// Token: 0x040024AD RID: 9389
	private BnetBattleTag m_battleTag;

	// Token: 0x040024AE RID: 9390
	private ulong m_lastOnlineMicrosec;

	// Token: 0x040024AF RID: 9391
	private bool m_away;

	// Token: 0x040024B0 RID: 9392
	private ulong m_awayTimeMicrosec;

	// Token: 0x040024B1 RID: 9393
	private bool m_busy;
}
