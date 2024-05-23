using System;
using bgs;
using bgs.types;

// Token: 0x020004D4 RID: 1236
public class BnetInvitation
{
	// Token: 0x06003A3B RID: 14907 RVA: 0x0011A44C File Offset: 0x0011864C
	public static BnetInvitation CreateFromFriendsUpdate(FriendsUpdate src)
	{
		return new BnetInvitation
		{
			m_id = new BnetInvitationId(src.long1),
			m_inviterId = src.entity1.Clone(),
			m_inviteeId = src.entity2.Clone(),
			m_inviterName = src.string1,
			m_inviteeName = src.string2,
			m_message = src.string3,
			m_creationTimeMicrosec = src.long2,
			m_expirationTimeMicrosec = src.long3
		};
	}

	// Token: 0x06003A3C RID: 14908 RVA: 0x0011A4D7 File Offset: 0x001186D7
	public BnetInvitationId GetId()
	{
		return this.m_id;
	}

	// Token: 0x06003A3D RID: 14909 RVA: 0x0011A4DF File Offset: 0x001186DF
	public void SetId(BnetInvitationId id)
	{
		this.m_id = id;
	}

	// Token: 0x06003A3E RID: 14910 RVA: 0x0011A4E8 File Offset: 0x001186E8
	public BnetEntityId GetInviterId()
	{
		return this.m_inviterId;
	}

	// Token: 0x06003A3F RID: 14911 RVA: 0x0011A4F0 File Offset: 0x001186F0
	public void SetInviterId(BnetEntityId id)
	{
		this.m_inviterId = id;
	}

	// Token: 0x06003A40 RID: 14912 RVA: 0x0011A4F9 File Offset: 0x001186F9
	public string GetInviterName()
	{
		return this.m_inviterName;
	}

	// Token: 0x06003A41 RID: 14913 RVA: 0x0011A501 File Offset: 0x00118701
	public void SetInviterName(string name)
	{
		this.m_inviterName = name;
	}

	// Token: 0x06003A42 RID: 14914 RVA: 0x0011A50A File Offset: 0x0011870A
	public BnetEntityId GetInviteeId()
	{
		return this.m_inviteeId;
	}

	// Token: 0x06003A43 RID: 14915 RVA: 0x0011A512 File Offset: 0x00118712
	public void SetInviteeId(BnetEntityId id)
	{
		this.m_inviteeId = id;
	}

	// Token: 0x06003A44 RID: 14916 RVA: 0x0011A51B File Offset: 0x0011871B
	public string GetInviteeName()
	{
		return this.m_inviteeName;
	}

	// Token: 0x06003A45 RID: 14917 RVA: 0x0011A523 File Offset: 0x00118723
	public void SetInviteeName(string name)
	{
		this.m_inviteeName = name;
	}

	// Token: 0x06003A46 RID: 14918 RVA: 0x0011A52C File Offset: 0x0011872C
	public string GetMessage()
	{
		return this.m_message;
	}

	// Token: 0x06003A47 RID: 14919 RVA: 0x0011A534 File Offset: 0x00118734
	public void SetMessage(string message)
	{
		this.m_message = message;
	}

	// Token: 0x06003A48 RID: 14920 RVA: 0x0011A53D File Offset: 0x0011873D
	public ulong GetCreationTimeMicrosec()
	{
		return this.m_creationTimeMicrosec;
	}

	// Token: 0x06003A49 RID: 14921 RVA: 0x0011A545 File Offset: 0x00118745
	public void SetCreationTimeMicrosec(ulong microsec)
	{
		this.m_creationTimeMicrosec = microsec;
	}

	// Token: 0x06003A4A RID: 14922 RVA: 0x0011A54E File Offset: 0x0011874E
	public ulong GetExpirationTimeMicrosec()
	{
		return this.m_expirationTimeMicrosec;
	}

	// Token: 0x06003A4B RID: 14923 RVA: 0x0011A556 File Offset: 0x00118756
	public void SetExpirationTimeMicroSec(ulong microsec)
	{
		this.m_expirationTimeMicrosec = microsec;
	}

	// Token: 0x06003A4C RID: 14924 RVA: 0x0011A560 File Offset: 0x00118760
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		BnetInvitation bnetInvitation = obj as BnetInvitation;
		return bnetInvitation != null && this.m_id.Equals(bnetInvitation.m_id);
	}

	// Token: 0x06003A4D RID: 14925 RVA: 0x0011A595 File Offset: 0x00118795
	public bool Equals(BnetInvitationId other)
	{
		return other != null && this.m_id.Equals(other);
	}

	// Token: 0x06003A4E RID: 14926 RVA: 0x0011A5AB File Offset: 0x001187AB
	public override int GetHashCode()
	{
		return this.m_id.GetHashCode();
	}

	// Token: 0x06003A4F RID: 14927 RVA: 0x0011A5B8 File Offset: 0x001187B8
	public override string ToString()
	{
		if (this.m_id == null)
		{
			return "UNKNOWN INVITATION";
		}
		return string.Format("[id={0} inviterId={1} inviterName={2} inviteeId={3} inviteeName={4} message={5}]", new object[]
		{
			this.m_id,
			this.m_inviterId,
			this.m_inviterName,
			this.m_inviteeId,
			this.m_inviteeName,
			this.m_message
		});
	}

	// Token: 0x06003A50 RID: 14928 RVA: 0x0011A622 File Offset: 0x00118822
	public static bool operator ==(BnetInvitation a, BnetInvitation b)
	{
		return object.ReferenceEquals(a, b) || (a != null && b != null && a.m_id == b.m_id);
	}

	// Token: 0x06003A51 RID: 14929 RVA: 0x0011A651 File Offset: 0x00118851
	public static bool operator !=(BnetInvitation a, BnetInvitation b)
	{
		return !(a == b);
	}

	// Token: 0x0400253D RID: 9533
	private BnetInvitationId m_id;

	// Token: 0x0400253E RID: 9534
	private BnetEntityId m_inviterId;

	// Token: 0x0400253F RID: 9535
	private string m_inviterName;

	// Token: 0x04002540 RID: 9536
	private BnetEntityId m_inviteeId;

	// Token: 0x04002541 RID: 9537
	private string m_inviteeName;

	// Token: 0x04002542 RID: 9538
	private string m_message;

	// Token: 0x04002543 RID: 9539
	private ulong m_creationTimeMicrosec;

	// Token: 0x04002544 RID: 9540
	private ulong m_expirationTimeMicrosec;
}
