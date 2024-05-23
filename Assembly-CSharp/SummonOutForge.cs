using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E9D RID: 3741
public class SummonOutForge : SpellImpl
{
	// Token: 0x060070F4 RID: 28916 RVA: 0x0021494D File Offset: 0x00212B4D
	protected override void OnBirth(SpellStateType prevStateType)
	{
		base.StartCoroutine(this.BirthState());
	}

	// Token: 0x060070F5 RID: 28917 RVA: 0x0021495C File Offset: 0x00212B5C
	private IEnumerator BirthState()
	{
		base.InitActorVariables();
		base.SetActorVisibility(true, false);
		base.SetVisibility(this.m_scryLines, true);
		switch (this.m_actor.GetRarity())
		{
		case TAG_RARITY.RARE:
			this.m_scryLinesMaterial.SetColor("_TintColor", SummonOutForge.RARE_COLOR);
			this.m_scryLines.GetComponent<Renderer>().material.SetColor("_TintColor", SummonOutForge.RARE_COLOR);
			goto IL_19A;
		case TAG_RARITY.EPIC:
			this.m_scryLinesMaterial.SetColor("_TintColor", SummonOutForge.EPIC_COLOR);
			this.m_scryLines.GetComponent<Renderer>().material.SetColor("_TintColor", SummonOutForge.EPIC_COLOR);
			goto IL_19A;
		case TAG_RARITY.LEGENDARY:
			this.m_scryLinesMaterial.SetColor("_TintColor", SummonOutForge.LEGENDARY_COLOR);
			this.m_scryLines.GetComponent<Renderer>().material.SetColor("_TintColor", SummonOutForge.LEGENDARY_COLOR);
			goto IL_19A;
		}
		this.m_scryLinesMaterial.SetColor("_TintColor", SummonOutForge.COMMON_COLOR);
		this.m_scryLines.GetComponent<Renderer>().material.SetColor("_TintColor", SummonOutForge.COMMON_COLOR);
		IL_19A:
		base.PlayAnimation(this.m_scryLines, "AllyInHandScryLines_ForgeOut", 4, 0f);
		base.PlayParticles(this.m_burstMotes, false);
		yield return new WaitForSeconds(0.16f);
		base.SetVisibilityRecursive(this.m_rootObject, false);
		this.OnSpellFinished();
		yield break;
	}

	// Token: 0x04005A70 RID: 23152
	public GameObject m_scryLines;

	// Token: 0x04005A71 RID: 23153
	public Material m_scryLinesMaterial;

	// Token: 0x04005A72 RID: 23154
	public GameObject m_burstMotes;

	// Token: 0x04005A73 RID: 23155
	private static Color COMMON_COLOR = new Color(0.73333335f, 0.8235294f, 1f);

	// Token: 0x04005A74 RID: 23156
	private static Color RARE_COLOR = new Color(0.2f, 0.4745098f, 1f);

	// Token: 0x04005A75 RID: 23157
	private static Color EPIC_COLOR = new Color(0.54509807f, 0.23137255f, 1f);

	// Token: 0x04005A76 RID: 23158
	private static Color LEGENDARY_COLOR = new Color(1f, 0.6666667f, 0.2f);
}
