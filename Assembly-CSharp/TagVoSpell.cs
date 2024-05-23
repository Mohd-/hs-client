using System;
using UnityEngine;

// Token: 0x02000E19 RID: 3609
public class TagVoSpell : CardSoundSpell
{
	// Token: 0x06006E83 RID: 28291 RVA: 0x00206B2F File Offset: 0x00204D2F
	public override AudioSource DetermineBestAudioSource()
	{
		if (this.CanPlayTagVo())
		{
			return this.m_TagVoData.m_AudioSource;
		}
		return base.DetermineBestAudioSource();
	}

	// Token: 0x06006E84 RID: 28292 RVA: 0x00206B50 File Offset: 0x00204D50
	private bool CanPlayTagVo()
	{
		if (this.m_TagVoData.m_TagRequirements.Count == 0)
		{
			return false;
		}
		Card sourceCard = base.GetSourceCard();
		if (sourceCard == null)
		{
			return false;
		}
		Entity entity = sourceCard.GetEntity();
		foreach (TagVoRequirement tagVoRequirement in this.m_TagVoData.m_TagRequirements)
		{
			int tag = entity.GetTag(tagVoRequirement.m_Tag);
			if (tag != tagVoRequirement.m_Value)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04005711 RID: 22289
	public TagVoData m_TagVoData = new TagVoData();
}
