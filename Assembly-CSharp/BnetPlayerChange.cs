using System;

// Token: 0x0200054A RID: 1354
public class BnetPlayerChange
{
	// Token: 0x06003E56 RID: 15958 RVA: 0x0012DE96 File Offset: 0x0012C096
	public BnetPlayer GetOldPlayer()
	{
		return this.m_oldPlayer;
	}

	// Token: 0x06003E57 RID: 15959 RVA: 0x0012DE9E File Offset: 0x0012C09E
	public void SetOldPlayer(BnetPlayer player)
	{
		this.m_oldPlayer = player;
	}

	// Token: 0x06003E58 RID: 15960 RVA: 0x0012DEA7 File Offset: 0x0012C0A7
	public BnetPlayer GetNewPlayer()
	{
		return this.m_newPlayer;
	}

	// Token: 0x06003E59 RID: 15961 RVA: 0x0012DEAF File Offset: 0x0012C0AF
	public void SetNewPlayer(BnetPlayer player)
	{
		this.m_newPlayer = player;
	}

	// Token: 0x06003E5A RID: 15962 RVA: 0x0012DEB8 File Offset: 0x0012C0B8
	public BnetPlayer GetPlayer()
	{
		return this.m_newPlayer;
	}

	// Token: 0x040027E5 RID: 10213
	private BnetPlayer m_oldPlayer;

	// Token: 0x040027E6 RID: 10214
	private BnetPlayer m_newPlayer;
}
