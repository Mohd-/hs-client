using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E11 RID: 3601
public class CardSpecificVoSpell : CardSoundSpell
{
	// Token: 0x06006E74 RID: 28276 RVA: 0x00206702 File Offset: 0x00204902
	public override AudioSource DetermineBestAudioSource()
	{
		if (this.SearchForCard())
		{
			return this.m_CardSpecificVoData.m_AudioSource;
		}
		return base.DetermineBestAudioSource();
	}

	// Token: 0x06006E75 RID: 28277 RVA: 0x00206724 File Offset: 0x00204924
	private bool SearchForCard()
	{
		if (string.IsNullOrEmpty(this.m_CardSpecificVoData.m_CardId))
		{
			return false;
		}
		foreach (SpellZoneTag zoneTag in this.m_CardSpecificVoData.m_ZonesToSearch)
		{
			List<Zone> zones = SpellUtils.FindZonesFromTag(this, zoneTag, this.m_CardSpecificVoData.m_SideToSearch);
			if (this.IsCardInZones(zones))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06006E76 RID: 28278 RVA: 0x002067BC File Offset: 0x002049BC
	private bool IsCardInZones(List<Zone> zones)
	{
		if (zones == null)
		{
			return false;
		}
		foreach (Zone zone in zones)
		{
			foreach (Card card in zone.GetCards())
			{
				Entity entity = card.GetEntity();
				if (entity.GetCardId() == this.m_CardSpecificVoData.m_CardId)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04005705 RID: 22277
	public CardSpecificVoData m_CardSpecificVoData = new CardSpecificVoData();
}
