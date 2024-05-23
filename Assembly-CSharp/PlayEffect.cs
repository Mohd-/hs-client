using System;
using UnityEngine;

// Token: 0x02000F1F RID: 3871
public class PlayEffect : MonoBehaviour
{
	// Token: 0x06007367 RID: 29543 RVA: 0x0021FFB1 File Offset: 0x0021E1B1
	private void Start()
	{
	}

	// Token: 0x06007368 RID: 29544 RVA: 0x0021FFB3 File Offset: 0x0021E1B3
	public void PlayEmitter1()
	{
		this.fxEmitter1.GetComponent<ParticleEmitter>().emit = true;
	}

	// Token: 0x06007369 RID: 29545 RVA: 0x0021FFC6 File Offset: 0x0021E1C6
	public void StopEmitter1()
	{
		this.fxEmitter1.GetComponent<ParticleEmitter>().emit = false;
	}

	// Token: 0x0600736A RID: 29546 RVA: 0x0021FFD9 File Offset: 0x0021E1D9
	private void Update()
	{
	}

	// Token: 0x04005DF9 RID: 24057
	public GameObject fxEmitter1;
}
