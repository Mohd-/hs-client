using System;
using System.Collections.Generic;
using bgs;

// Token: 0x020004D5 RID: 1237
public class PendingBnetFriendChangelist
{
	// Token: 0x06003A53 RID: 14931 RVA: 0x0011A670 File Offset: 0x00118870
	public List<BnetPlayer> GetFriends()
	{
		return this.m_friends;
	}

	// Token: 0x06003A54 RID: 14932 RVA: 0x0011A678 File Offset: 0x00118878
	public bool Add(BnetPlayer friend)
	{
		if (this.m_friends.Contains(friend))
		{
			return false;
		}
		this.m_friends.Add(friend);
		return true;
	}

	// Token: 0x06003A55 RID: 14933 RVA: 0x0011A6A5 File Offset: 0x001188A5
	public bool Remove(BnetPlayer friend)
	{
		return this.m_friends.Remove(friend);
	}

	// Token: 0x06003A56 RID: 14934 RVA: 0x0011A6B3 File Offset: 0x001188B3
	public void Clear()
	{
		this.m_friends.Clear();
	}

	// Token: 0x06003A57 RID: 14935 RVA: 0x0011A6C0 File Offset: 0x001188C0
	public int GetCount()
	{
		return this.m_friends.Count;
	}

	// Token: 0x06003A58 RID: 14936 RVA: 0x0011A6D0 File Offset: 0x001188D0
	public BnetPlayer FindFriend(BnetAccountId id)
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

	// Token: 0x06003A59 RID: 14937 RVA: 0x0011A740 File Offset: 0x00118940
	public BnetPlayer FindFriend(BnetGameAccountId id)
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

	// Token: 0x06003A5A RID: 14938 RVA: 0x0011A7AC File Offset: 0x001189AC
	public bool IsFriend(BnetPlayer player)
	{
		if (this.m_friends.Contains(player))
		{
			return true;
		}
		if (player == null)
		{
			return false;
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

	// Token: 0x06003A5B RID: 14939 RVA: 0x0011A850 File Offset: 0x00118A50
	public bool IsFriend(BnetAccountId id)
	{
		return this.FindFriend(id) != null;
	}

	// Token: 0x06003A5C RID: 14940 RVA: 0x0011A85F File Offset: 0x00118A5F
	public bool IsFriend(BnetGameAccountId id)
	{
		return this.FindFriend(id) != null;
	}

	// Token: 0x06003A5D RID: 14941 RVA: 0x0011A870 File Offset: 0x00118A70
	public BnetFriendChangelist CreateChangelist()
	{
		BnetFriendChangelist bnetFriendChangelist = new BnetFriendChangelist();
		for (int i = this.m_friends.Count - 1; i >= 0; i--)
		{
			BnetPlayer bnetPlayer = this.m_friends[i];
			if (bnetPlayer.IsDisplayable())
			{
				bnetFriendChangelist.AddAddedFriend(bnetPlayer);
				this.m_friends.RemoveAt(i);
			}
		}
		return bnetFriendChangelist;
	}

	// Token: 0x04002545 RID: 9541
	private List<BnetPlayer> m_friends = new List<BnetPlayer>();
}
