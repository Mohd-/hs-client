using System;
using UnityEngine;

// Token: 0x02000F42 RID: 3906
public class StageTester : MonoBehaviour
{
	// Token: 0x06007407 RID: 29703 RVA: 0x002229A4 File Offset: 0x00220BA4
	private void Start()
	{
	}

	// Token: 0x06007408 RID: 29704 RVA: 0x002229A8 File Offset: 0x00220BA8
	private void OnMouseDown()
	{
		switch (this.stage)
		{
		case 0:
			this.Highlighted();
			break;
		case 1:
			this.Selected();
			break;
		case 2:
			this.ManaUsed();
			break;
		case 3:
			this.Released();
			break;
		}
		this.stage++;
	}

	// Token: 0x06007409 RID: 29705 RVA: 0x00222A14 File Offset: 0x00220C14
	private void Highlighted()
	{
		this.highlightBase.GetComponent<Animation>().Play();
		this.highlightEdge.GetComponent<Animation>().Play();
	}

	// Token: 0x0600740A RID: 29706 RVA: 0x00222A44 File Offset: 0x00220C44
	private void Selected()
	{
		this.highlightBase.GetComponent<Animation>().CrossFade("AllyInHandActiveBaseSelected", 0.3f);
		this.fxEmitterA.GetComponent<Animation>().Play();
	}

	// Token: 0x0600740B RID: 29707 RVA: 0x00222A7C File Offset: 0x00220C7C
	private void ManaUsed()
	{
		this.highlightBase.GetComponent<Animation>().CrossFade("AllyInHandActiveBaseMana", 0.3f);
		this.fxEmitterA.GetComponent<Animation>().CrossFade("AllyInHandFXUnHighlight", 0.3f);
	}

	// Token: 0x0600740C RID: 29708 RVA: 0x00222AC0 File Offset: 0x00220CC0
	private void Released()
	{
		this.rays.GetComponent<Animation>().Play("AllyInHandRaysUp");
		this.flash.GetComponent<Animation>().Play("AllyInHandGlowOn");
		this.entireObj.GetComponent<Animation>().Play("AllyInHandDeath");
		this.inplayObj.GetComponent<Animation>().Play("AllyInPlaySpawn");
	}

	// Token: 0x0600740D RID: 29709 RVA: 0x00222B25 File Offset: 0x00220D25
	public void PlayEmitterB()
	{
		this.fxEmitterB.GetComponent<ParticleEmitter>().emit = true;
	}

	// Token: 0x0600740E RID: 29710 RVA: 0x00222B38 File Offset: 0x00220D38
	private void Update()
	{
	}

	// Token: 0x04005EA0 RID: 24224
	public GameObject highlightBase;

	// Token: 0x04005EA1 RID: 24225
	public GameObject highlightEdge;

	// Token: 0x04005EA2 RID: 24226
	public GameObject entireObj;

	// Token: 0x04005EA3 RID: 24227
	public GameObject inplayObj;

	// Token: 0x04005EA4 RID: 24228
	public GameObject rays;

	// Token: 0x04005EA5 RID: 24229
	public GameObject flash;

	// Token: 0x04005EA6 RID: 24230
	public GameObject fxEmitterA;

	// Token: 0x04005EA7 RID: 24231
	public GameObject fxEmitterB;

	// Token: 0x04005EA8 RID: 24232
	private int stage;
}
