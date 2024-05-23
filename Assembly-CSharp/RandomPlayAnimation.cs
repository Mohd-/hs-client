using System;
using UnityEngine;

// Token: 0x02000F2A RID: 3882
public class RandomPlayAnimation : MonoBehaviour
{
	// Token: 0x0600739F RID: 29599 RVA: 0x00220EA6 File Offset: 0x0021F0A6
	private void Start()
	{
		this.m_animation = base.gameObject.GetComponent<Animation>();
	}

	// Token: 0x060073A0 RID: 29600 RVA: 0x00220EBC File Offset: 0x0021F0BC
	private void Update()
	{
		if (this.m_animation == null)
		{
			base.enabled = false;
		}
		if (this.m_waitTime < 0f)
		{
			if (this.m_MinWaitTime < 0f)
			{
				this.m_MinWaitTime = 0f;
			}
			if (this.m_MaxWaitTime < 0f)
			{
				this.m_MaxWaitTime = 0f;
			}
			if (this.m_MaxWaitTime < this.m_MinWaitTime)
			{
				this.m_MaxWaitTime = this.m_MinWaitTime;
			}
			this.m_waitTime = Random.Range(this.m_MinWaitTime, this.m_MaxWaitTime);
			this.m_startTime = Time.time;
		}
		if (Time.time - this.m_startTime > this.m_waitTime)
		{
			this.m_waitTime = -1f;
			this.m_animation.Play();
		}
	}

	// Token: 0x04005E2F RID: 24111
	public float m_MinWaitTime;

	// Token: 0x04005E30 RID: 24112
	public float m_MaxWaitTime = 10f;

	// Token: 0x04005E31 RID: 24113
	private float m_waitTime = -1f;

	// Token: 0x04005E32 RID: 24114
	private float m_startTime;

	// Token: 0x04005E33 RID: 24115
	private Animation m_animation;
}
