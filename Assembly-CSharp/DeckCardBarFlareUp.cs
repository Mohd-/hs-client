using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E54 RID: 3668
public class DeckCardBarFlareUp : SpellImpl
{
	// Token: 0x06006F5C RID: 28508 RVA: 0x0020B0E4 File Offset: 0x002092E4
	protected override void OnBirth(SpellStateType prevStateType)
	{
		if (base.gameObject.activeSelf)
		{
			base.StartCoroutine(this.BirthState());
		}
	}

	// Token: 0x06006F5D RID: 28509 RVA: 0x0020B110 File Offset: 0x00209310
	private IEnumerator BirthState()
	{
		base.SetVisibility(this.m_fuseQuad, true);
		base.PlayParticles(this.m_fxSparks, false);
		base.PlayAnimation(this.m_fuseQuad, "DeckCardBar_FuseInOut", 4, 0f);
		this.OnSpellFinished();
		yield return new WaitForSeconds(2f);
		base.SetVisibility(this.m_fuseQuad, false);
		yield break;
	}

	// Token: 0x0400588A RID: 22666
	public GameObject m_fuseQuad;

	// Token: 0x0400588B RID: 22667
	public GameObject m_fxSparks;
}
