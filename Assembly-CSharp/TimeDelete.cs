using System;
using UnityEngine;

// Token: 0x02000EEA RID: 3818
public class TimeDelete : MonoBehaviour
{
	// Token: 0x0600724F RID: 29263 RVA: 0x0021980F File Offset: 0x00217A0F
	private void Start()
	{
		this.m_StartTime = Time.time;
	}

	// Token: 0x06007250 RID: 29264 RVA: 0x0021981C File Offset: 0x00217A1C
	private void Update()
	{
		if (Time.time > this.m_StartTime + this.m_SecondsToDelete)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04005C68 RID: 23656
	public float m_SecondsToDelete = 10f;

	// Token: 0x04005C69 RID: 23657
	private float m_StartTime;
}
