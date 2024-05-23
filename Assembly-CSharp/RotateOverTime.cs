using System;
using UnityEngine;

// Token: 0x020006B4 RID: 1716
public class RotateOverTime : MonoBehaviour
{
	// Token: 0x060047AC RID: 18348 RVA: 0x00157F60 File Offset: 0x00156160
	private void Start()
	{
		if (this.RandomStartX)
		{
			base.transform.Rotate(Vector3.left, (float)Random.Range(0, 360));
		}
		if (this.RandomStartY)
		{
			base.transform.Rotate(Vector3.up, (float)Random.Range(0, 360));
		}
		if (this.RandomStartZ)
		{
			base.transform.Rotate(Vector3.forward, (float)Random.Range(0, 360));
		}
	}

	// Token: 0x060047AD RID: 18349 RVA: 0x00157FE4 File Offset: 0x001561E4
	private void Update()
	{
		base.transform.Rotate(Vector3.left, Time.deltaTime * this.RotateSpeedX, 1);
		base.transform.Rotate(Vector3.up, Time.deltaTime * this.RotateSpeedY, 1);
		base.transform.Rotate(Vector3.forward, Time.deltaTime * this.RotateSpeedZ, 1);
	}

	// Token: 0x04002F18 RID: 12056
	public float RotateSpeedX;

	// Token: 0x04002F19 RID: 12057
	public float RotateSpeedY;

	// Token: 0x04002F1A RID: 12058
	public float RotateSpeedZ;

	// Token: 0x04002F1B RID: 12059
	public bool RandomStartX;

	// Token: 0x04002F1C RID: 12060
	public bool RandomStartY;

	// Token: 0x04002F1D RID: 12061
	public bool RandomStartZ;
}
