using System;

// Token: 0x020001F6 RID: 502
public class PlatformSetting<T>
{
	// Token: 0x06001E3F RID: 7743 RVA: 0x0008D2DE File Offset: 0x0008B4DE
	public void Set(T val)
	{
		this.Setting = val;
		this.WasSet = true;
	}

	// Token: 0x06001E40 RID: 7744 RVA: 0x0008D2EE File Offset: 0x0008B4EE
	public T Get()
	{
		return this.Setting;
	}

	// Token: 0x040010D8 RID: 4312
	public T Setting = default(T);

	// Token: 0x040010D9 RID: 4313
	public bool WasSet;
}
