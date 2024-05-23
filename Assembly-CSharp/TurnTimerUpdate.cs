using System;

// Token: 0x02000599 RID: 1433
public class TurnTimerUpdate
{
	// Token: 0x0600409E RID: 16542 RVA: 0x00137D17 File Offset: 0x00135F17
	public float GetSecondsRemaining()
	{
		return this.m_secondsRemaining;
	}

	// Token: 0x0600409F RID: 16543 RVA: 0x00137D1F File Offset: 0x00135F1F
	public void SetSecondsRemaining(float sec)
	{
		this.m_secondsRemaining = sec;
	}

	// Token: 0x060040A0 RID: 16544 RVA: 0x00137D28 File Offset: 0x00135F28
	public float GetEndTimestamp()
	{
		return this.m_endTimestamp;
	}

	// Token: 0x060040A1 RID: 16545 RVA: 0x00137D30 File Offset: 0x00135F30
	public void SetEndTimestamp(float timestamp)
	{
		this.m_endTimestamp = timestamp;
	}

	// Token: 0x060040A2 RID: 16546 RVA: 0x00137D39 File Offset: 0x00135F39
	public bool ShouldShow()
	{
		return this.m_show;
	}

	// Token: 0x060040A3 RID: 16547 RVA: 0x00137D41 File Offset: 0x00135F41
	public void SetShow(bool show)
	{
		this.m_show = show;
	}

	// Token: 0x04002934 RID: 10548
	private float m_secondsRemaining;

	// Token: 0x04002935 RID: 10549
	private float m_endTimestamp;

	// Token: 0x04002936 RID: 10550
	private bool m_show;
}
