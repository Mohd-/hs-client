using System;
using System.Collections.Generic;

// Token: 0x02000E13 RID: 3603
[Serializable]
public class ClassSpecificVoData
{
	// Token: 0x06006E78 RID: 28280 RVA: 0x0020688C File Offset: 0x00204A8C
	public ClassSpecificVoData()
	{
		List<SpellZoneTag> list = new List<SpellZoneTag>();
		list.Add(SpellZoneTag.HERO);
		this.m_ZonesToSearch = list;
		base..ctor();
	}

	// Token: 0x04005708 RID: 22280
	public List<ClassSpecificVoLine> m_Lines = new List<ClassSpecificVoLine>();

	// Token: 0x04005709 RID: 22281
	public SpellPlayerSide m_SideToSearch = SpellPlayerSide.TARGET;

	// Token: 0x0400570A RID: 22282
	public List<SpellZoneTag> m_ZonesToSearch;
}
