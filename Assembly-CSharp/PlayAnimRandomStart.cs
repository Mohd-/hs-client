using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F1D RID: 3869
public class PlayAnimRandomStart : MonoBehaviour
{
	// Token: 0x0600735E RID: 29534 RVA: 0x0021FE4B File Offset: 0x0021E04B
	private void Start()
	{
		base.StartCoroutine(this.PlayRandomBubbles());
	}

	// Token: 0x0600735F RID: 29535 RVA: 0x0021FE5C File Offset: 0x0021E05C
	private IEnumerator PlayRandomBubbles()
	{
		for (;;)
		{
			yield return new WaitForSeconds(Random.Range(this.minWait, this.maxWait));
			int bubbleIndex = Random.Range(0, this.m_Bubbles.Count);
			GameObject bubbles = this.m_Bubbles[bubbleIndex];
			if (!(bubbles == null))
			{
				bubbles.GetComponent<Animation>().Play();
				bubbles.GetComponent<Animation>()[this.animName].speed = Random.Range(this.MinSpeed, this.MaxSpeed);
			}
		}
		yield break;
	}

	// Token: 0x04005DEE RID: 24046
	public List<GameObject> m_Bubbles;

	// Token: 0x04005DEF RID: 24047
	public float minWait;

	// Token: 0x04005DF0 RID: 24048
	public float maxWait = 10f;

	// Token: 0x04005DF1 RID: 24049
	public float MinSpeed = 0.2f;

	// Token: 0x04005DF2 RID: 24050
	public float MaxSpeed = 1.1f;

	// Token: 0x04005DF3 RID: 24051
	public string animName = "Bubble1";
}
