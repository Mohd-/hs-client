using System;
using UnityEngine;

// Token: 0x02000F34 RID: 3892
public class RotateOverTimePingPong : MonoBehaviour
{
	// Token: 0x060073C2 RID: 29634 RVA: 0x00221854 File Offset: 0x0021FA54
	private void Start()
	{
		if (this.RandomStartX)
		{
			base.transform.Rotate(Vector3.left, Random.Range(this.RotateRangeXmin, this.RotateRangeXmax));
		}
		if (this.RandomStartY)
		{
			base.transform.Rotate(Vector3.up, Random.Range(this.RotateRangeYmin, this.RotateRangeYmax));
		}
		if (this.RandomStartZ)
		{
			base.transform.Rotate(Vector3.forward, Random.Range(this.RotateRangeZmin, this.RotateRangeZmax));
		}
	}

	// Token: 0x060073C3 RID: 29635 RVA: 0x002218E8 File Offset: 0x0021FAE8
	private void Update()
	{
		float num = Mathf.Sin(Time.time) * this.RotateRangeZmax;
		float y = base.gameObject.transform.localRotation.y;
		iTween.RotateUpdate(base.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, y, num),
			"isLocal",
			true,
			"time",
			0
		}));
	}

	// Token: 0x04005E58 RID: 24152
	public float RotateSpeedX;

	// Token: 0x04005E59 RID: 24153
	public float RotateSpeedY;

	// Token: 0x04005E5A RID: 24154
	public float RotateSpeedZ;

	// Token: 0x04005E5B RID: 24155
	public bool RandomStartX = true;

	// Token: 0x04005E5C RID: 24156
	public bool RandomStartY = true;

	// Token: 0x04005E5D RID: 24157
	public bool RandomStartZ = true;

	// Token: 0x04005E5E RID: 24158
	public float RotateRangeXmin;

	// Token: 0x04005E5F RID: 24159
	public float RotateRangeXmax = 10f;

	// Token: 0x04005E60 RID: 24160
	public float RotateRangeYmin;

	// Token: 0x04005E61 RID: 24161
	public float RotateRangeYmax = 10f;

	// Token: 0x04005E62 RID: 24162
	public float RotateRangeZmin;

	// Token: 0x04005E63 RID: 24163
	public float RotateRangeZmax = 10f;
}
