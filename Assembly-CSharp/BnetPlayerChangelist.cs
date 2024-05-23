using System;
using System.Collections.Generic;
using bgs;

// Token: 0x020004D9 RID: 1241
public class BnetPlayerChangelist
{
	// Token: 0x06003A80 RID: 14976 RVA: 0x0011AD5B File Offset: 0x00118F5B
	public List<BnetPlayerChange> GetChanges()
	{
		return this.m_changes;
	}

	// Token: 0x06003A81 RID: 14977 RVA: 0x0011AD63 File Offset: 0x00118F63
	public void AddChange(BnetPlayerChange change)
	{
		this.m_changes.Add(change);
	}

	// Token: 0x06003A82 RID: 14978 RVA: 0x0011AD74 File Offset: 0x00118F74
	public bool HasChange(BnetGameAccountId id)
	{
		BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(id);
		return this.HasChange(player);
	}

	// Token: 0x06003A83 RID: 14979 RVA: 0x0011AD94 File Offset: 0x00118F94
	public bool HasChange(BnetAccountId id)
	{
		BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(id);
		return this.HasChange(player);
	}

	// Token: 0x06003A84 RID: 14980 RVA: 0x0011ADB4 File Offset: 0x00118FB4
	public bool HasChange(BnetPlayer player)
	{
		return this.FindChange(player) != null;
	}

	// Token: 0x06003A85 RID: 14981 RVA: 0x0011ADC4 File Offset: 0x00118FC4
	public BnetPlayerChange FindChange(BnetGameAccountId id)
	{
		BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(id);
		return this.FindChange(player);
	}

	// Token: 0x06003A86 RID: 14982 RVA: 0x0011ADE4 File Offset: 0x00118FE4
	public BnetPlayerChange FindChange(BnetAccountId id)
	{
		BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(id);
		return this.FindChange(player);
	}

	// Token: 0x06003A87 RID: 14983 RVA: 0x0011AE04 File Offset: 0x00119004
	public BnetPlayerChange FindChange(BnetPlayer player)
	{
		if (player == null)
		{
			return null;
		}
		return this.m_changes.Find((BnetPlayerChange change) => change.GetPlayer() == player);
	}

	// Token: 0x0400254E RID: 9550
	private List<BnetPlayerChange> m_changes = new List<BnetPlayerChange>();
}
