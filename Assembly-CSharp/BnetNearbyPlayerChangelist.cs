using System;
using System.Collections.Generic;

// Token: 0x02000587 RID: 1415
public class BnetNearbyPlayerChangelist
{
	// Token: 0x06004038 RID: 16440 RVA: 0x0013742A File Offset: 0x0013562A
	public List<BnetPlayer> GetAddedPlayers()
	{
		return this.m_playersAdded;
	}

	// Token: 0x06004039 RID: 16441 RVA: 0x00137432 File Offset: 0x00135632
	public List<BnetPlayer> GetUpdatedPlayers()
	{
		return this.m_playersUpdated;
	}

	// Token: 0x0600403A RID: 16442 RVA: 0x0013743A File Offset: 0x0013563A
	public List<BnetPlayer> GetRemovedPlayers()
	{
		return this.m_playersRemoved;
	}

	// Token: 0x0600403B RID: 16443 RVA: 0x00137442 File Offset: 0x00135642
	public List<BnetPlayer> GetAddedFriends()
	{
		return this.m_friendsAdded;
	}

	// Token: 0x0600403C RID: 16444 RVA: 0x0013744A File Offset: 0x0013564A
	public List<BnetPlayer> GetUpdatedFriends()
	{
		return this.m_friendsUpdated;
	}

	// Token: 0x0600403D RID: 16445 RVA: 0x00137452 File Offset: 0x00135652
	public List<BnetPlayer> GetRemovedFriends()
	{
		return this.m_friendsRemoved;
	}

	// Token: 0x0600403E RID: 16446 RVA: 0x0013745A File Offset: 0x0013565A
	public List<BnetPlayer> GetAddedStrangers()
	{
		return this.m_strangersAdded;
	}

	// Token: 0x0600403F RID: 16447 RVA: 0x00137462 File Offset: 0x00135662
	public List<BnetPlayer> GetUpdatedStrangers()
	{
		return this.m_strangersUpdated;
	}

	// Token: 0x06004040 RID: 16448 RVA: 0x0013746A File Offset: 0x0013566A
	public List<BnetPlayer> GetRemovedStrangers()
	{
		return this.m_strangersRemoved;
	}

	// Token: 0x06004041 RID: 16449 RVA: 0x00137474 File Offset: 0x00135674
	public bool IsEmpty()
	{
		return (this.m_playersAdded == null || this.m_playersAdded.Count <= 0) && (this.m_playersUpdated == null || this.m_playersUpdated.Count <= 0) && (this.m_playersRemoved == null || this.m_playersRemoved.Count <= 0) && (this.m_friendsAdded == null || this.m_friendsAdded.Count <= 0) && (this.m_friendsUpdated == null || this.m_friendsUpdated.Count <= 0) && (this.m_friendsRemoved == null || this.m_friendsRemoved.Count <= 0) && (this.m_strangersAdded == null || this.m_strangersAdded.Count <= 0) && (this.m_strangersUpdated == null || this.m_strangersUpdated.Count <= 0) && (this.m_strangersRemoved == null || this.m_strangersRemoved.Count <= 0);
	}

	// Token: 0x06004042 RID: 16450 RVA: 0x00137590 File Offset: 0x00135790
	public void Clear()
	{
		this.ClearAddedPlayers();
		this.ClearUpdatedPlayers();
		this.ClearRemovedPlayers();
		this.ClearAddedFriends();
		this.ClearUpdatedFriends();
		this.ClearRemovedFriends();
		this.ClearAddedStrangers();
		this.ClearUpdatedStrangers();
		this.ClearRemovedStrangers();
	}

	// Token: 0x06004043 RID: 16451 RVA: 0x001375D3 File Offset: 0x001357D3
	public bool AddAddedPlayer(BnetPlayer player)
	{
		if (this.m_playersAdded == null)
		{
			this.m_playersAdded = new List<BnetPlayer>();
		}
		else if (this.m_playersAdded.Contains(player))
		{
			return false;
		}
		this.m_playersAdded.Add(player);
		return true;
	}

	// Token: 0x06004044 RID: 16452 RVA: 0x00137610 File Offset: 0x00135810
	public bool RemoveAddedPlayer(BnetPlayer player)
	{
		return this.m_playersAdded != null && this.m_playersAdded.Remove(player);
	}

	// Token: 0x06004045 RID: 16453 RVA: 0x0013762B File Offset: 0x0013582B
	public void ClearAddedPlayers()
	{
		this.m_playersAdded = null;
	}

	// Token: 0x06004046 RID: 16454 RVA: 0x00137634 File Offset: 0x00135834
	public bool AddUpdatedPlayer(BnetPlayer player)
	{
		if (this.m_playersUpdated == null)
		{
			this.m_playersUpdated = new List<BnetPlayer>();
		}
		else if (this.m_playersUpdated.Contains(player))
		{
			return false;
		}
		this.m_playersUpdated.Add(player);
		return true;
	}

	// Token: 0x06004047 RID: 16455 RVA: 0x00137671 File Offset: 0x00135871
	public bool RemoveUpdatedPlayer(BnetPlayer player)
	{
		return this.m_playersUpdated != null && this.m_playersUpdated.Remove(player);
	}

	// Token: 0x06004048 RID: 16456 RVA: 0x0013768C File Offset: 0x0013588C
	public void ClearUpdatedPlayers()
	{
		this.m_playersUpdated = null;
	}

	// Token: 0x06004049 RID: 16457 RVA: 0x00137695 File Offset: 0x00135895
	public bool AddRemovedPlayer(BnetPlayer player)
	{
		if (this.m_playersRemoved == null)
		{
			this.m_playersRemoved = new List<BnetPlayer>();
		}
		else if (this.m_playersRemoved.Contains(player))
		{
			return false;
		}
		this.m_playersRemoved.Add(player);
		return true;
	}

	// Token: 0x0600404A RID: 16458 RVA: 0x001376D2 File Offset: 0x001358D2
	public bool RemoveRemovedPlayer(BnetPlayer player)
	{
		return this.m_playersRemoved != null && this.m_playersRemoved.Remove(player);
	}

	// Token: 0x0600404B RID: 16459 RVA: 0x001376ED File Offset: 0x001358ED
	public void ClearRemovedPlayers()
	{
		this.m_playersRemoved = null;
	}

	// Token: 0x0600404C RID: 16460 RVA: 0x001376F6 File Offset: 0x001358F6
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

	// Token: 0x0600404D RID: 16461 RVA: 0x00137733 File Offset: 0x00135933
	public bool RemoveAddedFriend(BnetPlayer friend)
	{
		return this.m_friendsAdded != null && this.m_friendsAdded.Remove(friend);
	}

	// Token: 0x0600404E RID: 16462 RVA: 0x0013774E File Offset: 0x0013594E
	public void ClearAddedFriends()
	{
		this.m_friendsAdded = null;
	}

	// Token: 0x0600404F RID: 16463 RVA: 0x00137757 File Offset: 0x00135957
	public bool AddUpdatedFriend(BnetPlayer friend)
	{
		if (this.m_friendsUpdated == null)
		{
			this.m_friendsUpdated = new List<BnetPlayer>();
		}
		else if (this.m_friendsUpdated.Contains(friend))
		{
			return false;
		}
		this.m_friendsUpdated.Add(friend);
		return true;
	}

	// Token: 0x06004050 RID: 16464 RVA: 0x00137794 File Offset: 0x00135994
	public bool RemoveUpdatedFriend(BnetPlayer friend)
	{
		return this.m_friendsUpdated != null && this.m_friendsUpdated.Remove(friend);
	}

	// Token: 0x06004051 RID: 16465 RVA: 0x001377AF File Offset: 0x001359AF
	public void ClearUpdatedFriends()
	{
		this.m_friendsUpdated = null;
	}

	// Token: 0x06004052 RID: 16466 RVA: 0x001377B8 File Offset: 0x001359B8
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

	// Token: 0x06004053 RID: 16467 RVA: 0x001377F5 File Offset: 0x001359F5
	public bool RemoveRemovedFriend(BnetPlayer friend)
	{
		return this.m_friendsRemoved != null && this.m_friendsRemoved.Remove(friend);
	}

	// Token: 0x06004054 RID: 16468 RVA: 0x00137810 File Offset: 0x00135A10
	public void ClearRemovedFriends()
	{
		this.m_friendsRemoved = null;
	}

	// Token: 0x06004055 RID: 16469 RVA: 0x00137819 File Offset: 0x00135A19
	public bool AddAddedStranger(BnetPlayer stranger)
	{
		if (this.m_strangersAdded == null)
		{
			this.m_strangersAdded = new List<BnetPlayer>();
		}
		else if (this.m_strangersAdded.Contains(stranger))
		{
			return false;
		}
		this.m_strangersAdded.Add(stranger);
		return true;
	}

	// Token: 0x06004056 RID: 16470 RVA: 0x00137856 File Offset: 0x00135A56
	public bool RemoveAddedStranger(BnetPlayer stranger)
	{
		return this.m_strangersAdded != null && this.m_strangersAdded.Remove(stranger);
	}

	// Token: 0x06004057 RID: 16471 RVA: 0x00137871 File Offset: 0x00135A71
	public void ClearAddedStrangers()
	{
		this.m_strangersAdded = null;
	}

	// Token: 0x06004058 RID: 16472 RVA: 0x0013787A File Offset: 0x00135A7A
	public bool AddUpdatedStranger(BnetPlayer stranger)
	{
		if (this.m_strangersUpdated == null)
		{
			this.m_strangersUpdated = new List<BnetPlayer>();
		}
		else if (this.m_strangersUpdated.Contains(stranger))
		{
			return false;
		}
		this.m_strangersUpdated.Add(stranger);
		return true;
	}

	// Token: 0x06004059 RID: 16473 RVA: 0x001378B7 File Offset: 0x00135AB7
	public bool RemoveUpdatedStranger(BnetPlayer stranger)
	{
		return this.m_strangersUpdated != null && this.m_strangersUpdated.Remove(stranger);
	}

	// Token: 0x0600405A RID: 16474 RVA: 0x001378D2 File Offset: 0x00135AD2
	public void ClearUpdatedStrangers()
	{
		this.m_strangersUpdated = null;
	}

	// Token: 0x0600405B RID: 16475 RVA: 0x001378DB File Offset: 0x00135ADB
	public bool AddRemovedStranger(BnetPlayer stranger)
	{
		if (this.m_strangersRemoved == null)
		{
			this.m_strangersRemoved = new List<BnetPlayer>();
		}
		else if (this.m_strangersRemoved.Contains(stranger))
		{
			return false;
		}
		this.m_strangersRemoved.Add(stranger);
		return true;
	}

	// Token: 0x0600405C RID: 16476 RVA: 0x00137918 File Offset: 0x00135B18
	public bool RemoveRemovedStranger(BnetPlayer stranger)
	{
		return this.m_strangersRemoved != null && this.m_strangersRemoved.Remove(stranger);
	}

	// Token: 0x0600405D RID: 16477 RVA: 0x00137933 File Offset: 0x00135B33
	public void ClearRemovedStrangers()
	{
		this.m_strangersRemoved = null;
	}

	// Token: 0x0400290F RID: 10511
	private List<BnetPlayer> m_playersAdded;

	// Token: 0x04002910 RID: 10512
	private List<BnetPlayer> m_playersUpdated;

	// Token: 0x04002911 RID: 10513
	private List<BnetPlayer> m_playersRemoved;

	// Token: 0x04002912 RID: 10514
	private List<BnetPlayer> m_friendsAdded;

	// Token: 0x04002913 RID: 10515
	private List<BnetPlayer> m_friendsUpdated;

	// Token: 0x04002914 RID: 10516
	private List<BnetPlayer> m_friendsRemoved;

	// Token: 0x04002915 RID: 10517
	private List<BnetPlayer> m_strangersAdded;

	// Token: 0x04002916 RID: 10518
	private List<BnetPlayer> m_strangersUpdated;

	// Token: 0x04002917 RID: 10519
	private List<BnetPlayer> m_strangersRemoved;
}
