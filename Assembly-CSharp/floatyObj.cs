using System;
using UnityEngine;

// Token: 0x02000F52 RID: 3922
public class floatyObj : MonoBehaviour
{
	// Token: 0x060074B1 RID: 29873 RVA: 0x00226FA5 File Offset: 0x002251A5
	private void Start()
	{
		this.m_interval = Random.Range(this.frequencyMin, this.frequencyMax);
	}

	// Token: 0x060074B2 RID: 29874 RVA: 0x00226FC0 File Offset: 0x002251C0
	private void Update()
	{
		float num = Mathf.Sin(Time.time * this.m_interval) * this.magnitude;
		Vector3 vector;
		vector..ctor(num, num, num);
		base.transform.position += vector;
		base.transform.eulerAngles += vector;
	}

	// Token: 0x04005F18 RID: 24344
	public float frequencyMin = 0.0001f;

	// Token: 0x04005F19 RID: 24345
	public float frequencyMax = 0.001f;

	// Token: 0x04005F1A RID: 24346
	public float magnitude = 0.0001f;

	// Token: 0x04005F1B RID: 24347
	private float m_interval;
}
