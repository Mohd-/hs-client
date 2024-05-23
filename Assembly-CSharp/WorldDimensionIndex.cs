using System;

// Token: 0x020001C4 RID: 452
public struct WorldDimensionIndex
{
	// Token: 0x06001D26 RID: 7462 RVA: 0x00088F5C File Offset: 0x0008715C
	public WorldDimensionIndex(float dimension, int index)
	{
		this.Dimension = dimension;
		this.Index = index;
	}

	// Token: 0x04000F9F RID: 3999
	public float Dimension;

	// Token: 0x04000FA0 RID: 4000
	public int Index;
}
