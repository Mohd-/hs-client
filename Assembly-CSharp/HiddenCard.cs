using System;

// Token: 0x020006B2 RID: 1714
public class HiddenCard : CardDef
{
	// Token: 0x060047A8 RID: 18344 RVA: 0x00157DFA File Offset: 0x00155FFA
	public override string DetermineActorNameForZone(Entity entity, TAG_ZONE zoneTag)
	{
		if (zoneTag == TAG_ZONE.DECK || zoneTag == TAG_ZONE.GRAVEYARD || zoneTag == TAG_ZONE.REMOVEDFROMGAME || zoneTag == TAG_ZONE.SETASIDE)
		{
			return "Card_Invisible";
		}
		if (zoneTag == TAG_ZONE.SECRET)
		{
			return base.DetermineActorNameForZone(entity, zoneTag);
		}
		return "Card_Hidden";
	}
}
