using System;
using UnityEngine;

// Token: 0x02000E46 RID: 3654
public class CardBurstCommon : Spell
{
	// Token: 0x06006F24 RID: 28452 RVA: 0x00209EA8 File Offset: 0x002080A8
	protected override void OnBirth(SpellStateType prevStateType)
	{
		if (this.m_BurstMotes)
		{
			this.m_BurstMotes.Play();
		}
		if (this.m_EdgeGlow)
		{
			this.m_EdgeGlow.GetComponent<Renderer>().enabled = true;
		}
		this.OnSpellFinished();
	}

	// Token: 0x0400583C RID: 22588
	public ParticleSystem m_BurstMotes;

	// Token: 0x0400583D RID: 22589
	public GameObject m_EdgeGlow;
}
