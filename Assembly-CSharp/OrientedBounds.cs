using System;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class OrientedBounds
{
	// Token: 0x06001D25 RID: 7461 RVA: 0x00088F49 File Offset: 0x00087149
	public Vector3 GetTrueCenterPosition()
	{
		return this.Origin + this.CenterOffset;
	}

	// Token: 0x04000F9C RID: 3996
	public Vector3[] Extents;

	// Token: 0x04000F9D RID: 3997
	public Vector3 Origin;

	// Token: 0x04000F9E RID: 3998
	public Vector3 CenterOffset;
}
