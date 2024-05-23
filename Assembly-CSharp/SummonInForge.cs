using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E9B RID: 3739
public class SummonInForge : SpellImpl
{
	// Token: 0x060070EA RID: 28906 RVA: 0x002146EC File Offset: 0x002128EC
	protected override void OnBirth(SpellStateType prevStateType)
	{
		base.StartCoroutine(this.BirthState());
	}

	// Token: 0x060070EB RID: 28907 RVA: 0x002146FC File Offset: 0x002128FC
	private IEnumerator BirthState()
	{
		base.InitActorVariables();
		base.SetActorVisibility(false, true);
		base.SetVisibility(this.m_burnIn, true);
		base.SetAnimationSpeed(this.m_burnIn, "AllyInHandScryLines_Forge", this.m_burnInAnimationSpeed);
		base.PlayAnimation(this.m_burnIn, "AllyInHandScryLines_Forge", 4, 0f);
		base.PlayParticles(this.m_smokePuff, false);
		base.PlayParticles(this.m_blackBits, false);
		yield return new WaitForSeconds(0.2f);
		base.SetActorVisibility(true, true);
		if (this.m_isHeroActor)
		{
			GameObject attackObject = base.GetActorObject("AttackObject");
			GameObject healthObject = base.GetActorObject("HealthObject");
			base.SetVisibilityRecursive(attackObject, false);
			base.SetVisibilityRecursive(healthObject, false);
		}
		yield return new WaitForSeconds(0.2f);
		this.OnSpellFinished();
		yield break;
	}

	// Token: 0x04005A66 RID: 23142
	public GameObject m_burnIn;

	// Token: 0x04005A67 RID: 23143
	public GameObject m_blackBits;

	// Token: 0x04005A68 RID: 23144
	public GameObject m_smokePuff;

	// Token: 0x04005A69 RID: 23145
	public float m_burnInAnimationSpeed = 1f;

	// Token: 0x04005A6A RID: 23146
	public bool m_isHeroActor;
}
