using System;
using UnityEngine;

// Token: 0x0200086C RID: 2156
public class UberFloaty : MonoBehaviour
{
	// Token: 0x060052C8 RID: 21192 RVA: 0x0018AF00 File Offset: 0x00189100
	private void Start()
	{
		this.m_interval.x = Random.Range(this.frequencyMin, this.frequencyMax);
		this.m_interval.y = Random.Range(this.frequencyMin, this.frequencyMax);
		this.m_interval.z = Random.Range(this.frequencyMin, this.frequencyMax);
		this.m_offset.x = 0.5f * Random.Range(-this.m_interval.x, this.m_interval.x);
		this.m_offset.y = 0.5f * Random.Range(-this.m_interval.y, this.m_interval.y);
		this.m_offset.z = 0.5f * Random.Range(-this.m_interval.z, this.m_interval.z);
		this.m_rotationInterval.x = Random.Range(this.frequencyMinRot, this.frequencyMaxRot);
		this.m_rotationInterval.y = Random.Range(this.frequencyMinRot, this.frequencyMaxRot);
		this.m_rotationInterval.z = Random.Range(this.frequencyMinRot, this.frequencyMaxRot);
	}

	// Token: 0x060052C9 RID: 21193 RVA: 0x0018B03C File Offset: 0x0018923C
	private void Update()
	{
		Vector3 vector;
		vector.x = Mathf.Sin(Time.time * this.m_interval.x + this.m_offset.x) * this.magnitude.x * this.m_interval.x;
		vector.y = Mathf.Sin(Time.time * this.m_interval.y + this.m_offset.y) * this.magnitude.y * this.m_interval.y;
		vector.z = Mathf.Sin(Time.time * this.m_interval.z + this.m_offset.z) * this.magnitude.z * this.m_interval.z;
		if (this.localSpace)
		{
			base.transform.localPosition += vector;
		}
		else
		{
			base.transform.position += vector;
		}
		Vector3 vector2;
		vector2.x = Mathf.Sin(Time.time * this.m_rotationInterval.x + this.m_offset.x) * this.magnitudeRot.x * this.m_rotationInterval.x;
		vector2.y = Mathf.Sin(Time.time * this.m_rotationInterval.y + this.m_offset.y) * this.magnitudeRot.y * this.m_rotationInterval.y;
		vector2.z = Mathf.Sin(Time.time * this.m_rotationInterval.z + this.m_offset.z) * this.magnitudeRot.z * this.m_rotationInterval.z;
		base.transform.eulerAngles += vector2;
	}

	// Token: 0x040038FF RID: 14591
	public float frequencyMin = 1f;

	// Token: 0x04003900 RID: 14592
	public float frequencyMax = 3f;

	// Token: 0x04003901 RID: 14593
	public Vector3 magnitude = new Vector3(0.001f, 0.001f, 0.001f);

	// Token: 0x04003902 RID: 14594
	public float frequencyMinRot = 1f;

	// Token: 0x04003903 RID: 14595
	public float frequencyMaxRot = 3f;

	// Token: 0x04003904 RID: 14596
	public Vector3 magnitudeRot = new Vector3(0f, 0f, 0f);

	// Token: 0x04003905 RID: 14597
	public bool localSpace = true;

	// Token: 0x04003906 RID: 14598
	private Vector3 m_interval;

	// Token: 0x04003907 RID: 14599
	private Vector3 m_offset;

	// Token: 0x04003908 RID: 14600
	private Vector3 m_rotationInterval;
}
