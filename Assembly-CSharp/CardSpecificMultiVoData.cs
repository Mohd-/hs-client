using System;
using System.Collections.Generic;

// Token: 0x02000E0C RID: 3596
[Serializable]
public class CardSpecificMultiVoData
{
	// Token: 0x06006E5C RID: 28252 RVA: 0x002061E8 File Offset: 0x002043E8
	public CardSpecificMultiVoData()
	{
		List<SpellZoneTag> list = new List<SpellZoneTag>();
		list.Add(SpellZoneTag.PLAY);
		list.Add(SpellZoneTag.HERO);
		list.Add(SpellZoneTag.HERO_POWER);
		list.Add(SpellZoneTag.WEAPON);
		this.m_ZonesToSearch = list;
		base..ctor();
	}

	// Token: 0x040056F3 RID: 22259
	public string m_CardId;

	// Token: 0x040056F4 RID: 22260
	public SpellPlayerSide m_SideToSearch = SpellPlayerSide.TARGET;

	// Token: 0x040056F5 RID: 22261
	public List<SpellZoneTag> m_ZonesToSearch;

	// Token: 0x040056F6 RID: 22262
	public CardSpecificMultiVoLine[] m_Lines;
}
