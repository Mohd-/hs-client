using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E4B RID: 3659
public class CardBurstRare : Spell
{
	// Token: 0x06006F3A RID: 28474 RVA: 0x0020A314 File Offset: 0x00208514
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
		if (this.m_BurstMotes)
		{
			this.m_BurstMotes.Play();
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

	// Token: 0x06006F3B RID: 28475 RVA: 0x0020A3EC File Offset: 0x002085EC
	protected override void OnDeath(SpellStateType prevStateType)
	{
		if (this.m_EdgeGlow)
		{
			this.m_EdgeGlow.GetComponent<Animation>().Play(this.m_EdgeGlowDeathAnimation, 4);
		}
		base.StartCoroutine(this.DeathState());
	}

	// Token: 0x06006F3C RID: 28476 RVA: 0x0020A430 File Offset: 0x00208630
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

	// Token: 0x04005853 RID: 22611
	public GameObject m_RenderPlane;

	// Token: 0x04005854 RID: 22612
	public GameObject m_RaysMask;

	// Token: 0x04005855 RID: 22613
	public GameObject m_EdgeGlow;

	// Token: 0x04005856 RID: 22614
	public string m_EdgeGlowBirthAnimation = "StandardEdgeGlowFade_Forge";

	// Token: 0x04005857 RID: 22615
	public ParticleSystem m_Bang;

	// Token: 0x04005858 RID: 22616
	public ParticleSystem m_BangLinger;

	// Token: 0x04005859 RID: 22617
	public ParticleSystem m_BurstMotes;

	// Token: 0x0400585A RID: 22618
	public string m_EdgeGlowDeathAnimation = "StandardEdgeGlowFadeOut_Forge";
}
