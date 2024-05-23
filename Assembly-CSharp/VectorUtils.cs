using System;
using UnityEngine;

// Token: 0x020004AE RID: 1198
public static class VectorUtils
{
	// Token: 0x0600392C RID: 14636 RVA: 0x00116C15 File Offset: 0x00114E15
	public static Vector2 Abs(Vector2 vector)
	{
		return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
	}

	// Token: 0x0600392D RID: 14637 RVA: 0x00116C34 File Offset: 0x00114E34
	public static Vector2 CreateFromAngle(float degrees)
	{
		float num = 0.017453292f * degrees;
		return new Vector2(Mathf.Cos(num), Mathf.Sin(num));
	}

	// Token: 0x0600392E RID: 14638 RVA: 0x00116C5A File Offset: 0x00114E5A
	public static Vector3 Abs(Vector3 vector)
	{
		return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
	}
}
