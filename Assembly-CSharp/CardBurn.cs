using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E44 RID: 3652
public class CardBurn : Spell
{
	// Token: 0x06006F1B RID: 28443 RVA: 0x00209D3B File Offset: 0x00207F3B
	protected override void OnBirth(SpellStateType prevStateType)
	{
		base.StartCoroutine(this.BirthAction());
	}

	// Token: 0x06006F1C RID: 28444 RVA: 0x00209D4C File Offset: 0x00207F4C
	private IEnumerator BirthAction()
	{
		if (this.m_BurnCardQuad)
		{
			this.m_BurnCardQuad.GetComponent<Renderer>().enabled = true;
			this.m_BurnCardQuad.GetComponent<Animation>().Play(this.m_BurnCardAnim, 4);
		}
		if (this.m_EdgeEmbers)
		{
			this.m_EdgeEmbers.Play();
		}
		yield return new WaitForSeconds(0.15f);
		Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(base.gameObject);
		if (actor == null)
		{
			yield break;
		}
		actor.Hide();
		this.OnSpellFinished();
		yield break;
	}

	// Token: 0x04005835 RID: 22581
	public GameObject m_BurnCardQuad;

	// Token: 0x04005836 RID: 22582
	public string m_BurnCardAnim = "CardBurnUpFire";

	// Token: 0x04005837 RID: 22583
	public ParticleSystem m_EdgeEmbers;
}
