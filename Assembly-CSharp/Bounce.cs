using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E4E RID: 3662
public class Bounce : MonoBehaviour
{
	// Token: 0x06006F4A RID: 28490 RVA: 0x0020ABFB File Offset: 0x00208DFB
	private void Start()
	{
		if (this.m_PlayOnStart)
		{
			this.StartAnimation();
		}
	}

	// Token: 0x06006F4B RID: 28491 RVA: 0x0020AC0E File Offset: 0x00208E0E
	private void Update()
	{
	}

	// Token: 0x06006F4C RID: 28492 RVA: 0x0020AC10 File Offset: 0x00208E10
	public void StartAnimation()
	{
		this.m_BounceAmountOverTime = this.m_BounceAmount;
		this.m_StartingPosition = base.transform.position;
		base.StartCoroutine("BounceAnimation");
	}

	// Token: 0x06006F4D RID: 28493 RVA: 0x0020AC3C File Offset: 0x00208E3C
	private IEnumerator BounceAnimation()
	{
		yield return new WaitForSeconds(this.m_Delay);
		for (int c = 0; c < this.m_BounceCount; c++)
		{
			float time = 0f;
			for (float i = 0f; i < 1f; i += Time.deltaTime * this.m_BounceSpeed)
			{
				time += Time.deltaTime * this.m_BounceSpeed;
				Vector3 pos = this.m_StartingPosition;
				float bounce = Mathf.Sin(time * 3.1415927f);
				if (bounce < 0f)
				{
					break;
				}
				base.transform.position = new Vector3(pos.x, pos.y + bounce * this.m_BounceAmountOverTime, pos.z);
				yield return null;
			}
			this.m_BounceAmountOverTime *= this.m_Bounceness;
			yield return null;
		}
		base.transform.position = this.m_StartingPosition;
		yield break;
	}

	// Token: 0x0400586E RID: 22638
	public float m_BounceSpeed = 3.5f;

	// Token: 0x0400586F RID: 22639
	public float m_BounceAmount = 3f;

	// Token: 0x04005870 RID: 22640
	public int m_BounceCount = 3;

	// Token: 0x04005871 RID: 22641
	public float m_Bounceness = 0.2f;

	// Token: 0x04005872 RID: 22642
	public float m_Delay;

	// Token: 0x04005873 RID: 22643
	public bool m_PlayOnStart;

	// Token: 0x04005874 RID: 22644
	private Vector3 m_StartingPosition;

	// Token: 0x04005875 RID: 22645
	private float m_BounceAmountOverTime;
}
