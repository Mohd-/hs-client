using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E47 RID: 3655
public class CardBurstEpic : Spell
{
	// Token: 0x06006F26 RID: 28454 RVA: 0x00209F18 File Offset: 0x00208118
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
		if (this.m_BurstFlare)
		{
			this.m_BurstFlare.Play();
		}
		if (this.m_Bang)
		{
			this.m_Bang.Play();
		}
		if (this.m_BangLinger)
		{
			this.m_BangLinger.Play();
		}
		this.OnSpellFinished();
	}

	// Token: 0x06006F27 RID: 28455 RVA: 0x00209FF0 File Offset: 0x002081F0
	protected override void OnDeath(SpellStateType prevStateType)
	{
		if (this.m_EdgeGlow)
		{
			this.m_EdgeGlow.GetComponent<Animation>().Play(this.m_EdgeGlowDeathAnimation, 4);
		}
		base.StartCoroutine(this.DeathState());
	}

	// Token: 0x06006F28 RID: 28456 RVA: 0x0020A034 File Offset: 0x00208234
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

	// Token: 0x0400583E RID: 22590
	public GameObject m_RenderPlane;

	// Token: 0x0400583F RID: 22591
	public GameObject m_RaysMask;

	// Token: 0x04005840 RID: 22592
	public GameObject m_EdgeGlow;

	// Token: 0x04005841 RID: 22593
	public string m_EdgeGlowBirthAnimation = "StandardEdgeGlowFade";

	// Token: 0x04005842 RID: 22594
	public ParticleSystem m_BurstFlare;

	// Token: 0x04005843 RID: 22595
	public ParticleSystem m_Bang;

	// Token: 0x04005844 RID: 22596
	public ParticleSystem m_BangLinger;

	// Token: 0x04005845 RID: 22597
	public string m_EdgeGlowDeathAnimation = "StandardEdgeGlowFadeOut";
}
