using System;

// Token: 0x02000849 RID: 2121
public class ZoneHeroPower : Zone
{
	// Token: 0x06005181 RID: 20865 RVA: 0x00185C64 File Offset: 0x00183E64
	private void Awake()
	{
	}

	// Token: 0x06005182 RID: 20866 RVA: 0x00185C66 File Offset: 0x00183E66
	public override string ToString()
	{
		return string.Format("{0} (Hero Power)", base.ToString());
	}

	// Token: 0x06005183 RID: 20867 RVA: 0x00185C78 File Offset: 0x00183E78
	public override bool CanAcceptTags(int controllerId, TAG_ZONE zoneTag, TAG_CARDTYPE cardType)
	{
		return base.CanAcceptTags(controllerId, zoneTag, cardType) && cardType == TAG_CARDTYPE.HERO_POWER;
	}
}
