using System;
using UnityEngine;

// Token: 0x02000793 RID: 1939
public class RandomTransform : MonoBehaviour
{
	// Token: 0x06004CCE RID: 19662 RVA: 0x0016D52F File Offset: 0x0016B72F
	public void Start()
	{
		if (this.m_applyOnStart)
		{
			this.Apply();
		}
	}

	// Token: 0x06004CCF RID: 19663 RVA: 0x0016D544 File Offset: 0x0016B744
	public void Apply()
	{
		Vector3 vector;
		vector..ctor(Random.Range(this.positionMin.x, this.positionMax.x), Random.Range(this.positionMin.y, this.positionMax.y), Random.Range(this.positionMin.z, this.positionMax.z));
		Vector3 localPosition = base.transform.localPosition + vector;
		base.transform.localPosition = localPosition;
		Vector3 vector2;
		vector2..ctor(Random.Range(this.rotationMin.x, this.rotationMax.x), Random.Range(this.rotationMin.y, this.rotationMax.y), Random.Range(this.rotationMin.z, this.rotationMax.z));
		Vector3 localEulerAngles = base.transform.localEulerAngles + vector2;
		base.transform.localEulerAngles = localEulerAngles;
		Vector3 vector3;
		vector3..ctor(Random.Range(this.scaleMin.x, this.scaleMax.x), Random.Range(this.scaleMin.y, this.scaleMax.y), Random.Range(this.scaleMin.z, this.scaleMax.z));
		Vector3 localScale = vector3;
		vector3.Scale(base.transform.localScale);
		base.transform.localScale = localScale;
	}

	// Token: 0x04003395 RID: 13205
	public bool m_applyOnStart;

	// Token: 0x04003396 RID: 13206
	public Vector3 positionMin;

	// Token: 0x04003397 RID: 13207
	public Vector3 positionMax;

	// Token: 0x04003398 RID: 13208
	public Vector3 rotationMin;

	// Token: 0x04003399 RID: 13209
	public Vector3 rotationMax;

	// Token: 0x0400339A RID: 13210
	public Vector3 scaleMin = Vector3.one;

	// Token: 0x0400339B RID: 13211
	public Vector3 scaleMax = Vector3.one;
}
