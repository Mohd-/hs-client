using System;

// Token: 0x02000AAF RID: 2735
public class iTweenCollection
{
	// Token: 0x06005EBB RID: 24251 RVA: 0x001C5E84 File Offset: 0x001C4084
	public void Add(iTween tween)
	{
		if (tween == null)
		{
			return;
		}
		if (this.LastIndex >= this.Tweens.Length)
		{
			Array.Resize<iTween>(ref this.Tweens, this.Tweens.Length * 2);
		}
		this.Tweens[this.LastIndex] = tween;
		this.LastIndex++;
		this.Count++;
	}

	// Token: 0x06005EBC RID: 24252 RVA: 0x001C5EEC File Offset: 0x001C40EC
	public void Remove(iTween tween)
	{
		if (tween == null || tween.destroyed)
		{
			return;
		}
		for (int i = 0; i < this.LastIndex; i++)
		{
			if (this.Tweens[i] == tween)
			{
				this.Tweens[i].destroyed = true;
				this.Tweens[i] = null;
				this.Count--;
				this.DeletedCount++;
				break;
			}
		}
	}

	// Token: 0x06005EBD RID: 24253 RVA: 0x001C5F68 File Offset: 0x001C4168
	public iTweenIterator GetIterator()
	{
		return new iTweenIterator(this);
	}

	// Token: 0x06005EBE RID: 24254 RVA: 0x001C5F70 File Offset: 0x001C4170
	public void CleanUp()
	{
		if (this.DeletedCount == 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < this.LastIndex; i++)
		{
			if (this.Tweens[i] != null)
			{
				this.Tweens[num] = this.Tweens[i];
				num++;
			}
		}
		this.LastIndex -= this.DeletedCount;
		this.DeletedCount = 0;
	}

	// Token: 0x04004633 RID: 17971
	public int LastIndex;

	// Token: 0x04004634 RID: 17972
	public int Count;

	// Token: 0x04004635 RID: 17973
	public iTween[] Tweens = new iTween[256];

	// Token: 0x04004636 RID: 17974
	public int DeletedCount;
}
