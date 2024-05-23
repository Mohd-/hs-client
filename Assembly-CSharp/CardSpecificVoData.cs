using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E10 RID: 3600
[Serializable]
public class CardSpecificVoData
{
	// Token: 0x06006E72 RID: 28274 RVA: 0x002066AC File Offset: 0x002048AC
	public CardSpecificVoData()
	{
		List<SpellZoneTag> list = new List<SpellZoneTag>();
		list.Add(SpellZoneTag.PLAY);
		list.Add(SpellZoneTag.HERO);
		list.Add(SpellZoneTag.HERO_POWER);
		list.Add(SpellZoneTag.WEAPON);
		this.m_ZonesToSearch = list;
		base..ctor();
	}

	// Token: 0x04005701 RID: 22273
	public string m_CardId;

	// Token: 0x04005702 RID: 22274
	public AudioSource m_AudioSource;

	// Token: 0x04005703 RID: 22275
	public SpellPlayerSide m_SideToSearch = SpellPlayerSide.TARGET;

	// Token: 0x04005704 RID: 22276
	public List<SpellZoneTag> m_ZonesToSearch;
}
