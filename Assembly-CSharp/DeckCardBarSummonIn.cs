using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E57 RID: 3671
public class DeckCardBarSummonIn : SpellImpl
{
	// Token: 0x06006F71 RID: 28529 RVA: 0x0020B57C File Offset: 0x0020977C
	private void OnDisable()
	{
		this.m_echoQuad.GetComponent<Renderer>().material.color = Color.clear;
		this.m_fxEvaporate.GetComponent<ParticleSystem>().Clear();
	}

	// Token: 0x06006F72 RID: 28530 RVA: 0x0020B5B3 File Offset: 0x002097B3
	protected override void OnBirth(SpellStateType prevStateType)
	{
		base.StartCoroutine(this.BirthState());
	}

	// Token: 0x06006F73 RID: 28531 RVA: 0x0020B5C4 File Offset: 0x002097C4
	private IEnumerator BirthState()
	{
		base.InitActorVariables();
		GameObject frame = base.GetActorObject("Frame");
		base.SetVisibilityRecursive(frame, false);
		base.SetVisibility(this.m_echoQuad, true);
		base.SetVisibilityRecursive(frame, true);
		base.PlayParticles(this.m_fxEvaporate, false);
		base.SetAnimationSpeed(this.m_echoQuad, "Secret_AbilityEchoFade", 0.5f);
		base.PlayAnimation(this.m_echoQuad, "Secret_AbilityEchoFade", 4, 0f);
		yield return new WaitForSeconds(1f);
		this.OnSpellFinished();
		base.SetVisibility(this.m_echoQuad, false);
		yield break;
	}

	// Token: 0x04005892 RID: 22674
	public GameObject m_echoQuad;

	// Token: 0x04005893 RID: 22675
	public GameObject m_fxEvaporate;
}
