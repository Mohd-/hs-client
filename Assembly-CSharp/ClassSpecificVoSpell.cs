using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E14 RID: 3604
public class ClassSpecificVoSpell : CardSoundSpell
{
	// Token: 0x06006E7A RID: 28282 RVA: 0x002068D8 File Offset: 0x00204AD8
	public override AudioSource DetermineBestAudioSource()
	{
		AudioSource audioSource = this.SearchForClassSpecificVo();
		if (audioSource)
		{
			return audioSource;
		}
		return base.DetermineBestAudioSource();
	}

	// Token: 0x06006E7B RID: 28283 RVA: 0x00206900 File Offset: 0x00204B00
	private AudioSource SearchForClassSpecificVo()
	{
		foreach (SpellZoneTag zoneTag in this.m_ClassSpecificVoData.m_ZonesToSearch)
		{
			List<Zone> zones = SpellUtils.FindZonesFromTag(this, zoneTag, this.m_ClassSpecificVoData.m_SideToSearch);
			AudioSource audioSource = this.SearchForClassSpecificVo(zones);
			if (audioSource)
			{
				return audioSource;
			}
		}
		return null;
	}

	// Token: 0x06006E7C RID: 28284 RVA: 0x0020698C File Offset: 0x00204B8C
	private AudioSource SearchForClassSpecificVo(List<Zone> zones)
	{
		if (zones == null)
		{
			return null;
		}
		foreach (Zone zone in zones)
		{
			foreach (Card card in zone.GetCards())
			{
				Entity entity = card.GetEntity();
				SpellClassTag spellClassTag = SpellUtils.ConvertClassTagToSpellEnum(entity.GetClass());
				if (spellClassTag != SpellClassTag.NONE)
				{
					foreach (ClassSpecificVoLine classSpecificVoLine in this.m_ClassSpecificVoData.m_Lines)
					{
						if (classSpecificVoLine.m_Class == spellClassTag)
						{
							return classSpecificVoLine.m_AudioSource;
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x0400570B RID: 22283
	public ClassSpecificVoData m_ClassSpecificVoData = new ClassSpecificVoData();
}
