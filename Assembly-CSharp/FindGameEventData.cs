using System;
using bgs.types;

// Token: 0x020002A4 RID: 676
public class FindGameEventData
{
	// Token: 0x040015B1 RID: 5553
	public FindGameState m_state;

	// Token: 0x040015B2 RID: 5554
	public GameServerInfo m_gameServer;

	// Token: 0x040015B3 RID: 5555
	public Network.GameCancelInfo m_cancelInfo;

	// Token: 0x040015B4 RID: 5556
	public int m_queueMinSeconds;

	// Token: 0x040015B5 RID: 5557
	public int m_queueMaxSeconds;
}
