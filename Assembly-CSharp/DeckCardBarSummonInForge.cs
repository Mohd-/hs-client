using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E59 RID: 3673
public class DeckCardBarSummonInForge : SpellImpl
{
	// Token: 0x06006F7C RID: 28540 RVA: 0x0020B811 File Offset: 0x00209A11
	protected override void OnBirth(SpellStateType prevStateType)
	{
		base.StartCoroutine(this.BirthState());
	}

	// Token: 0x06006F7D RID: 28541 RVA: 0x0020B820 File Offset: 0x00209A20
	private IEnumerator BirthState()
	{
		base.InitActorVariables();
		base.SetAnimationTime(this.m_echoQuad, "Secret_AbilityEchoOut_Forge", 0f);
		base.SetVisibility(this.m_echoQuad, true);
		Material echoMaterial = base.GetMaterial(this.m_echoQuad, this.m_echoQuadMaterial, false, 0);
		switch (this.m_actor.GetRarity())
		{
		case TAG_RARITY.RARE:
			base.SetMaterialColor(this.m_echoQuad, echoMaterial, "_Color", DeckCardBarSummonInForge.RARE_COLOR, 0);
			base.SetMaterialColor(this.m_fxEvaporate, this.m_fxEvaporateMaterial, "_TintColor", DeckCardBarSummonInForge.RARE_TINT_COLOR, 0);
			goto IL_22A;
		case TAG_RARITY.EPIC:
			base.SetMaterialColor(this.m_echoQuad, echoMaterial, "_Color", DeckCardBarSummonInForge.EPIC_COLOR, 0);
			base.SetMaterialColor(this.m_fxEvaporate, this.m_fxEvaporateMaterial, "_TintColor", DeckCardBarSummonInForge.EPIC_TINT_COLOR, 0);
			goto IL_22A;
		case TAG_RARITY.LEGENDARY:
			base.SetMaterialColor(this.m_echoQuad, echoMaterial, "_Color", DeckCardBarSummonInForge.LEGENDARY_COLOR, 0);
			base.SetMaterialColor(this.m_fxEvaporate, this.m_fxEvaporateMaterial, "_TintColor", DeckCardBarSummonInForge.LEGENDARY_TINT_COLOR, 0);
			goto IL_22A;
		}
		base.SetMaterialColor(this.m_echoQuad, echoMaterial, "_Color", DeckCardBarSummonInForge.COMMON_COLOR, 0);
		base.SetMaterialColor(this.m_fxEvaporate, this.m_fxEvaporateMaterial, "_TintColor", DeckCardBarSummonInForge.COMMON_TINT_COLOR, 0);
		IL_22A:
		base.SetActorVisibility(true, true);
		base.PlayParticles(this.m_fxEvaporate, false);
		base.SetAnimationSpeed(this.m_echoQuad, "Secret_AbilityEchoOut_Forge", 0.2f);
		base.PlayAnimation(this.m_echoQuad, "Secret_AbilityEchoOut_Forge", 4, 0f);
		this.OnSpellFinished();
		yield return new WaitForSeconds(1f);
		base.SetVisibility(this.m_echoQuad, false);
		yield break;
	}

	// Token: 0x04005898 RID: 22680
	public GameObject m_echoQuad;

	// Token: 0x04005899 RID: 22681
	public Material m_echoQuadMaterial;

	// Token: 0x0400589A RID: 22682
	public GameObject m_fxEvaporate;

	// Token: 0x0400589B RID: 22683
	public Material m_fxEvaporateMaterial;

	// Token: 0x0400589C RID: 22684
	private static Color COMMON_COLOR = new Color(1f, 1f, 1f);

	// Token: 0x0400589D RID: 22685
	private static Color COMMON_TINT_COLOR = new Color(0.92156863f, 0.94509804f, 1f);

	// Token: 0x0400589E RID: 22686
	private static Color RARE_COLOR = new Color(0.16470589f, 0.40784314f, 1f);

	// Token: 0x0400589F RID: 22687
	private static Color RARE_TINT_COLOR = new Color(0.16470589f, 0.40784314f, 1f);

	// Token: 0x040058A0 RID: 22688
	private static Color EPIC_COLOR = new Color(0.41568628f, 0.16470589f, 1f);

	// Token: 0x040058A1 RID: 22689
	private static Color EPIC_TINT_COLOR = new Color(0.41568628f, 0.16470589f, 0.99215686f);

	// Token: 0x040058A2 RID: 22690
	private static Color LEGENDARY_COLOR = new Color(0.76862746f, 0.5411765f, 0.14901961f);

	// Token: 0x040058A3 RID: 22691
	private static Color LEGENDARY_TINT_COLOR = new Color(0.6666667f, 0.4745098f, 0.12941177f);
}
