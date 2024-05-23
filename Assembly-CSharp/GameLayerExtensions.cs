using System;

// Token: 0x020001ED RID: 493
public static class GameLayerExtensions
{
	// Token: 0x06001DC1 RID: 7617 RVA: 0x0008AB8B File Offset: 0x00088D8B
	public static int LayerBit(this GameLayer gameLayer)
	{
		return 1 << (int)gameLayer;
	}
}
