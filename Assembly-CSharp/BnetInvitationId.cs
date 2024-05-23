using System;

// Token: 0x020000C5 RID: 197
public class BnetInvitationId
{
	// Token: 0x06000AB5 RID: 2741 RVA: 0x0002EE3F File Offset: 0x0002D03F
	public BnetInvitationId(ulong val)
	{
		this.m_val = val;
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0002EE4E File Offset: 0x0002D04E
	public ulong GetVal()
	{
		return this.m_val;
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x0002EE56 File Offset: 0x0002D056
	public void SetVal(ulong val)
	{
		this.m_val = val;
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x0002EE60 File Offset: 0x0002D060
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		BnetInvitationId bnetInvitationId = obj as BnetInvitationId;
		return bnetInvitationId != null && this.m_val == bnetInvitationId.m_val;
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0002EE92 File Offset: 0x0002D092
	public bool Equals(BnetInvitationId other)
	{
		return other != null && this.m_val == other.m_val;
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x0002EEAA File Offset: 0x0002D0AA
	public override int GetHashCode()
	{
		return this.m_val.GetHashCode();
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x0002EEB7 File Offset: 0x0002D0B7
	public override string ToString()
	{
		return this.m_val.ToString();
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x0002EEC4 File Offset: 0x0002D0C4
	public static bool operator ==(BnetInvitationId a, BnetInvitationId b)
	{
		return object.ReferenceEquals(a, b) || (a != null && b != null && a.m_val == b.m_val);
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0002EEFB File Offset: 0x0002D0FB
	public static bool operator !=(BnetInvitationId a, BnetInvitationId b)
	{
		return !(a == b);
	}

	// Token: 0x0400052C RID: 1324
	private ulong m_val;
}
