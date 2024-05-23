using System;

// Token: 0x02000922 RID: 2338
public class HistoryBigCardInitInfo : HistoryItemInitInfo
{
	// Token: 0x04003D5D RID: 15709
	public HistoryManager.BigCardFinishedCallback m_finishedCallback;

	// Token: 0x04003D5E RID: 15710
	public bool m_countered;

	// Token: 0x04003D5F RID: 15711
	public bool m_waitForSecretSpell;

	// Token: 0x04003D60 RID: 15712
	public bool m_fromMetaData;

	// Token: 0x04003D61 RID: 15713
	public Entity m_postTransformedEntity;
}
