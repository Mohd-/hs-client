using System;
using bgs;

// Token: 0x0200050C RID: 1292
public class PlayerChatInfo
{
	// Token: 0x06003BE6 RID: 15334 RVA: 0x00121CF2 File Offset: 0x0011FEF2
	public BnetPlayer GetPlayer()
	{
		return this.m_player;
	}

	// Token: 0x06003BE7 RID: 15335 RVA: 0x00121CFA File Offset: 0x0011FEFA
	public void SetPlayer(BnetPlayer player)
	{
		this.m_player = player;
	}

	// Token: 0x06003BE8 RID: 15336 RVA: 0x00121D03 File Offset: 0x0011FF03
	public float GetLastFocusTime()
	{
		return this.m_lastFocusTime;
	}

	// Token: 0x06003BE9 RID: 15337 RVA: 0x00121D0B File Offset: 0x0011FF0B
	public void SetLastFocusTime(float time)
	{
		this.m_lastFocusTime = time;
	}

	// Token: 0x06003BEA RID: 15338 RVA: 0x00121D14 File Offset: 0x0011FF14
	public BnetWhisper GetLastSeenWhisper()
	{
		return this.m_lastSeenWhisper;
	}

	// Token: 0x06003BEB RID: 15339 RVA: 0x00121D1C File Offset: 0x0011FF1C
	public void SetLastSeenWhisper(BnetWhisper whisper)
	{
		this.m_lastSeenWhisper = whisper;
	}

	// Token: 0x0400264D RID: 9805
	private BnetPlayer m_player;

	// Token: 0x0400264E RID: 9806
	private float m_lastFocusTime;

	// Token: 0x0400264F RID: 9807
	private BnetWhisper m_lastSeenWhisper;
}
