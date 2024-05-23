using System;
using System.Collections.Generic;

// Token: 0x020004D2 RID: 1234
public class BnetFriendChangelist
{
	// Token: 0x06003A1E RID: 14878 RVA: 0x0011A0BA File Offset: 0x001182BA
	public List<BnetPlayer> GetAddedFriends()
	{
		return this.m_friendsAdded;
	}

	// Token: 0x06003A1F RID: 14879 RVA: 0x0011A0C2 File Offset: 0x001182C2
	public List<BnetPlayer> GetRemovedFriends()
	{
		return this.m_friendsRemoved;
	}

	// Token: 0x06003A20 RID: 14880 RVA: 0x0011A0CA File Offset: 0x001182CA
	public List<BnetInvitation> GetAddedReceivedInvites()
	{
		return this.m_receivedInvitesAdded;
	}

	// Token: 0x06003A21 RID: 14881 RVA: 0x0011A0D2 File Offset: 0x001182D2
	public List<BnetInvitation> GetRemovedReceivedInvites()
	{
		return this.m_receivedInvitesRemoved;
	}

	// Token: 0x06003A22 RID: 14882 RVA: 0x0011A0DA File Offset: 0x001182DA
	public List<BnetInvitation> GetAddedSentInvites()
	{
		return this.m_sentInvitesAdded;
	}

	// Token: 0x06003A23 RID: 14883 RVA: 0x0011A0E2 File Offset: 0x001182E2
	public List<BnetInvitation> GetRemovedSentInvites()
	{
		return this.m_sentInvitesRemoved;
	}

	// Token: 0x06003A24 RID: 14884 RVA: 0x0011A0EC File Offset: 0x001182EC
	public bool IsEmpty()
	{
		return (this.m_friendsAdded == null || this.m_friendsAdded.Count <= 0) && (this.m_friendsRemoved == null || this.m_friendsRemoved.Count <= 0) && (this.m_receivedInvitesAdded == null || this.m_receivedInvitesAdded.Count <= 0) && (this.m_receivedInvitesRemoved == null || this.m_receivedInvitesRemoved.Count <= 0) && (this.m_sentInvitesAdded == null || this.m_sentInvitesAdded.Count <= 0) && (this.m_sentInvitesRemoved == null || this.m_sentInvitesRemoved.Count <= 0);
	}

	// Token: 0x06003A25 RID: 14885 RVA: 0x0011A1B0 File Offset: 0x001183B0
	public void Clear()
	{
		this.ClearAddedFriends();
		this.ClearRemovedFriends();
		this.ClearAddedReceivedInvites();
		this.ClearRemovedReceivedInvites();
		this.ClearAddedSentInvites();
		this.ClearRemovedSentInvites();
	}

	// Token: 0x06003A26 RID: 14886 RVA: 0x0011A1E1 File Offset: 0x001183E1
	public bool AddAddedFriend(BnetPlayer friend)
	{
		if (this.m_friendsAdded == null)
		{
			this.m_friendsAdded = new List<BnetPlayer>();
		}
		else if (this.m_friendsAdded.Contains(friend))
		{
			return false;
		}
		this.m_friendsAdded.Add(friend);
		return true;
	}

	// Token: 0x06003A27 RID: 14887 RVA: 0x0011A21E File Offset: 0x0011841E
	public bool RemoveAddedFriend(BnetPlayer friend)
	{
		return this.m_friendsAdded != null && this.m_friendsAdded.Remove(friend);
	}

	// Token: 0x06003A28 RID: 14888 RVA: 0x0011A239 File Offset: 0x00118439
	public void ClearAddedFriends()
	{
		this.m_friendsAdded = null;
	}

	// Token: 0x06003A29 RID: 14889 RVA: 0x0011A242 File Offset: 0x00118442
	public bool AddRemovedFriend(BnetPlayer friend)
	{
		if (this.m_friendsRemoved == null)
		{
			this.m_friendsRemoved = new List<BnetPlayer>();
		}
		else if (this.m_friendsRemoved.Contains(friend))
		{
			return false;
		}
		this.m_friendsRemoved.Add(friend);
		return true;
	}

	// Token: 0x06003A2A RID: 14890 RVA: 0x0011A27F File Offset: 0x0011847F
	public bool RemoveRemovedFriend(BnetPlayer friend)
	{
		return this.m_friendsRemoved != null && this.m_friendsRemoved.Remove(friend);
	}

	// Token: 0x06003A2B RID: 14891 RVA: 0x0011A29A File Offset: 0x0011849A
	public void ClearRemovedFriends()
	{
		this.m_friendsRemoved = null;
	}

	// Token: 0x06003A2C RID: 14892 RVA: 0x0011A2A3 File Offset: 0x001184A3
	public bool AddAddedReceivedInvite(BnetInvitation invite)
	{
		if (this.m_receivedInvitesAdded == null)
		{
			this.m_receivedInvitesAdded = new List<BnetInvitation>();
		}
		else if (this.m_receivedInvitesAdded.Contains(invite))
		{
			return false;
		}
		this.m_receivedInvitesAdded.Add(invite);
		return true;
	}

	// Token: 0x06003A2D RID: 14893 RVA: 0x0011A2E0 File Offset: 0x001184E0
	public bool RemoveAddedReceivedInvite(BnetInvitation invite)
	{
		return this.m_receivedInvitesAdded != null && this.m_receivedInvitesAdded.Remove(invite);
	}

	// Token: 0x06003A2E RID: 14894 RVA: 0x0011A2FB File Offset: 0x001184FB
	public void ClearAddedReceivedInvites()
	{
		this.m_receivedInvitesAdded = null;
	}

	// Token: 0x06003A2F RID: 14895 RVA: 0x0011A304 File Offset: 0x00118504
	public bool AddRemovedReceivedInvite(BnetInvitation invite)
	{
		if (this.m_receivedInvitesRemoved == null)
		{
			this.m_receivedInvitesRemoved = new List<BnetInvitation>();
		}
		else if (this.m_receivedInvitesRemoved.Contains(invite))
		{
			return false;
		}
		this.m_receivedInvitesRemoved.Add(invite);
		return true;
	}

	// Token: 0x06003A30 RID: 14896 RVA: 0x0011A341 File Offset: 0x00118541
	public bool RemoveRemovedReceivedInvite(BnetInvitation invite)
	{
		return this.m_receivedInvitesRemoved != null && this.m_receivedInvitesRemoved.Remove(invite);
	}

	// Token: 0x06003A31 RID: 14897 RVA: 0x0011A35C File Offset: 0x0011855C
	public void ClearRemovedReceivedInvites()
	{
		this.m_receivedInvitesRemoved = null;
	}

	// Token: 0x06003A32 RID: 14898 RVA: 0x0011A365 File Offset: 0x00118565
	public bool AddAddedSentInvite(BnetInvitation invite)
	{
		if (this.m_sentInvitesAdded == null)
		{
			this.m_sentInvitesAdded = new List<BnetInvitation>();
		}
		else if (this.m_sentInvitesAdded.Contains(invite))
		{
			return false;
		}
		this.m_sentInvitesAdded.Add(invite);
		return true;
	}

	// Token: 0x06003A33 RID: 14899 RVA: 0x0011A3A2 File Offset: 0x001185A2
	public bool RemoveAddedSentInvite(BnetInvitation invite)
	{
		return this.m_sentInvitesAdded != null && this.m_sentInvitesAdded.Remove(invite);
	}

	// Token: 0x06003A34 RID: 14900 RVA: 0x0011A3BD File Offset: 0x001185BD
	public void ClearAddedSentInvites()
	{
		this.m_sentInvitesAdded = null;
	}

	// Token: 0x06003A35 RID: 14901 RVA: 0x0011A3C6 File Offset: 0x001185C6
	public bool AddRemovedSentInvite(BnetInvitation invite)
	{
		if (this.m_sentInvitesRemoved == null)
		{
			this.m_sentInvitesRemoved = new List<BnetInvitation>();
		}
		else if (this.m_sentInvitesRemoved.Contains(invite))
		{
			return false;
		}
		this.m_sentInvitesRemoved.Add(invite);
		return true;
	}

	// Token: 0x06003A36 RID: 14902 RVA: 0x0011A403 File Offset: 0x00118603
	public bool RemoveRemovedSentInvite(BnetInvitation invite)
	{
		return this.m_sentInvitesRemoved != null && this.m_sentInvitesRemoved.Remove(invite);
	}

	// Token: 0x06003A37 RID: 14903 RVA: 0x0011A41E File Offset: 0x0011861E
	public void ClearRemovedSentInvites()
	{
		this.m_sentInvitesRemoved = null;
	}

	// Token: 0x04002537 RID: 9527
	private List<BnetPlayer> m_friendsAdded;

	// Token: 0x04002538 RID: 9528
	private List<BnetPlayer> m_friendsRemoved;

	// Token: 0x04002539 RID: 9529
	private List<BnetInvitation> m_receivedInvitesAdded;

	// Token: 0x0400253A RID: 9530
	private List<BnetInvitation> m_receivedInvitesRemoved;

	// Token: 0x0400253B RID: 9531
	private List<BnetInvitation> m_sentInvitesAdded;

	// Token: 0x0400253C RID: 9532
	private List<BnetInvitation> m_sentInvitesRemoved;
}
