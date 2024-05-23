using System;
using UnityEngine;

// Token: 0x02000F53 RID: 3923
public class floatyObj2 : MonoBehaviour
{
	// Token: 0x060074B4 RID: 29876 RVA: 0x00227060 File Offset: 0x00225260
	private void Start()
	{
		this.m_interval = Random.Range(this.frequencyMin, this.frequencyMax);
		this.m_rotationInterval = Random.Range(this.frequencyMinRot, this.frequencyMaxRot);
	}

	// Token: 0x060074B5 RID: 29877 RVA: 0x0022709C File Offset: 0x0022529C
	private void Update()
	{
		float num = Mathf.Sin(Time.time * this.m_interval) * this.magnitude;
		base.transform.position += new Vector3(num, num, num);
		float num2 = Mathf.Sin(Time.time * this.m_rotationInterval) * this.magnitudeRot;
		base.transform.eulerAngles += new Vector3(num2, num2, num2);
	}

	// Token: 0x04005F1C RID: 24348
	public float frequencyMin = 0.0001f;

	// Token: 0x04005F1D RID: 24349
	public float frequencyMax = 0.001f;

	// Token: 0x04005F1E RID: 24350
	public float magnitude = 0.0001f;

	// Token: 0x04005F1F RID: 24351
	public float frequencyMinRot = 0.0001f;

	// Token: 0x04005F20 RID: 24352
	public float frequencyMaxRot = 0.001f;

	// Token: 0x04005F21 RID: 24353
	public float magnitudeRot;

	// Token: 0x04005F22 RID: 24354
	private float m_interval;

	// Token: 0x04005F23 RID: 24355
	private float m_rotationInterval;
}
