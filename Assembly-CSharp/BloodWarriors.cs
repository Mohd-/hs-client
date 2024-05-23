using System;

// Token: 0x02000E3A RID: 3642
public class BloodWarriors : SpawnToHandSpell
{
	// Token: 0x06006EED RID: 28397 RVA: 0x0020886C File Offset: 0x00206A6C
	protected override void OnAction(SpellStateType prevStateType)
	{
		Card sourceCard = base.GetSourceCard();
		Entity entity = sourceCard.GetEntity();
		Player controller = entity.GetController();
		ZonePlay battlefieldZone = controller.GetBattlefieldZone();
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			for (int j = 0; j < battlefieldZone.GetCardCount(); j++)
			{
				Card cardAtIndex = battlefieldZone.GetCardAtIndex(j);
				if (cardAtIndex.GetPredictedZonePosition() == 0)
				{
					Entity entity2 = cardAtIndex.GetEntity();
					if (entity2.GetDamage() != 0)
					{
						if (base.AddUniqueOriginForTarget(i, cardAtIndex))
						{
							break;
						}
					}
				}
			}
		}
		base.OnAction(prevStateType);
	}
}
