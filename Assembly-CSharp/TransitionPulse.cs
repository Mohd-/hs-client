using System;
using UnityEngine;

// Token: 0x02000F4D RID: 3917
public class TransitionPulse : MonoBehaviour
{
	// Token: 0x0600748E RID: 29838 RVA: 0x00225789 File Offset: 0x00223989
	private void Start()
	{
		this.m_interval = Random.Range(this.frequencyMin, this.frequencyMax);
	}

	// Token: 0x0600748F RID: 29839 RVA: 0x002257A4 File Offset: 0x002239A4
	private void Update()
	{
		float num = Mathf.Sin(Time.time * this.m_interval) * this.magnitude;
		base.gameObject.GetComponent<Renderer>().material.SetFloat("_Transistion", num);
	}

	// Token: 0x04005EF5 RID: 24309
	public float frequencyMin = 0.0001f;

	// Token: 0x04005EF6 RID: 24310
	public float frequencyMax = 1f;

	// Token: 0x04005EF7 RID: 24311
	public float magnitude = 0.0001f;

	// Token: 0x04005EF8 RID: 24312
	private float m_interval;
}
