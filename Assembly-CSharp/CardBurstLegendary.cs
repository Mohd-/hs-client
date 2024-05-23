using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E49 RID: 3657
public class CardBurstLegendary : Spell
{
	// Token: 0x06006F30 RID: 28464 RVA: 0x0020A124 File Offset: 0x00208324
	protected override void OnBirth(SpellStateType prevStateType)
	{
		if (this.m_RenderPlane)
		{
			this.m_RenderPlane.SetActive(true);
		}
		if (this.m_RaysMask)
		{
			this.m_RaysMask.SetActive(true);
		}
		if (this.m_EdgeGlow)
		{
			this.m_EdgeGlow.GetComponent<Renderer>().enabled = true;
			this.m_EdgeGlow.GetComponent<Animation>().Play(this.m_EdgeGlowBirthAnimation, 4);
		}
		if (this.m_Shockwave)
		{
			this.m_Shockwave.Play();
		}
		if (this.m_Bang)
		{
			this.m_Bang.Play();
		}
		this.OnSpellFinished();
	}

	// Token: 0x06006F31 RID: 28465 RVA: 0x0020A1E0 File Offset: 0x002083E0
	protected override void OnDeath(SpellStateType prevStateType)
	{
		if (this.m_EdgeGlow)
		{
			this.m_EdgeGlow.GetComponent<Animation>().Play(this.m_EdgeGlowDeathAnimation, 4);
		}
		base.StartCoroutine(this.DeathState());
	}

	// Token: 0x06006F32 RID: 28466 RVA: 0x0020A224 File Offset: 0x00208424
	private IEnumerator DeathState()
	{
		yield return new WaitForSeconds(0.2f);
		if (this.m_EdgeGlow)
		{
			this.m_EdgeGlow.GetComponent<Renderer>().enabled = false;
		}
		this.OnSpellFinished();
		yield break;
	}

	// Token: 0x04005849 RID: 22601
	public GameObject m_RenderPlane;

	// Token: 0x0400584A RID: 22602
	public GameObject m_RaysMask;

	// Token: 0x0400584B RID: 22603
	public GameObject m_EdgeGlow;

	// Token: 0x0400584C RID: 22604
	public string m_EdgeGlowBirthAnimation = "StandardEdgeGlowFade_Forge";

	// Token: 0x0400584D RID: 22605
	public ParticleSystem m_Shockwave;

	// Token: 0x0400584E RID: 22606
	public ParticleSystem m_Bang;

	// Token: 0x0400584F RID: 22607
	public string m_EdgeGlowDeathAnimation = "StandardEdgeGlowFadeOut_Forge";
}
