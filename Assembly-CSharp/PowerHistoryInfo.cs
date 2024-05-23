using System;

// Token: 0x02000346 RID: 838
public class PowerHistoryInfo
{
	// Token: 0x06002BD0 RID: 11216 RVA: 0x000D9FDB File Offset: 0x000D81DB
	public PowerHistoryInfo()
	{
	}

	// Token: 0x06002BD1 RID: 11217 RVA: 0x000D9FEA File Offset: 0x000D81EA
	public PowerHistoryInfo(int index, bool show)
	{
		this.mEffectIndex = index;
		this.mShowInHistory = show;
	}

	// Token: 0x06002BD2 RID: 11218 RVA: 0x000DA007 File Offset: 0x000D8207
	public bool ShouldShowInHistory()
	{
		return this.mShowInHistory;
	}

	// Token: 0x06002BD3 RID: 11219 RVA: 0x000DA00F File Offset: 0x000D820F
	public int GetEffectIndex()
	{
		return this.mEffectIndex;
	}

	// Token: 0x04001A7A RID: 6778
	private bool mShowInHistory;

	// Token: 0x04001A7B RID: 6779
	private int mEffectIndex = -1;
}
