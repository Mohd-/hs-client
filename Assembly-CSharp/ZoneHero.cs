using System;

// Token: 0x020006B1 RID: 1713
public class ZoneHero : Zone
{
	// Token: 0x060047A5 RID: 18341 RVA: 0x00157DC4 File Offset: 0x00155FC4
	public override string ToString()
	{
		return string.Format("{0} (Hero)", base.ToString());
	}

	// Token: 0x060047A6 RID: 18342 RVA: 0x00157DD6 File Offset: 0x00155FD6
	public override bool CanAcceptTags(int controllerId, TAG_ZONE zoneTag, TAG_CARDTYPE cardType)
	{
		return base.CanAcceptTags(controllerId, zoneTag, cardType) && cardType == TAG_CARDTYPE.HERO;
	}
}
