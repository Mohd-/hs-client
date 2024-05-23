using System;

// Token: 0x020001E0 RID: 480
public struct iTweenIterator
{
	// Token: 0x06001DAD RID: 7597 RVA: 0x0008A7A4 File Offset: 0x000889A4
	public iTweenIterator(iTweenCollection collection)
	{
		this.TweenCollection = collection;
		this.Index = 0;
	}

	// Token: 0x06001DAE RID: 7598 RVA: 0x0008A7B4 File Offset: 0x000889B4
	public iTween GetNext()
	{
		if (this.TweenCollection == null)
		{
			return null;
		}
		while (this.Index < this.TweenCollection.LastIndex)
		{
			iTween iTween = this.TweenCollection.Tweens[this.Index];
			this.Index++;
			if (iTween != null)
			{
				return iTween;
			}
		}
		return null;
	}

	// Token: 0x04001061 RID: 4193
	private iTweenCollection TweenCollection;

	// Token: 0x04001062 RID: 4194
	private int Index;
}
