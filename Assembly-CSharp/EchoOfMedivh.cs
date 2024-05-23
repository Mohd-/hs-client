using System;

// Token: 0x02000E5F RID: 3679
public class EchoOfMedivh : SpawnToHandSpell
{
	// Token: 0x06006F9E RID: 28574 RVA: 0x0020C060 File Offset: 0x0020A260
	protected override void OnAction(SpellStateType prevStateType)
	{
		Card sourceCard = base.GetSourceCard();
		Entity entity = sourceCard.GetEntity();
		Player controller = entity.GetController();
		ZonePlay battlefieldZone = controller.GetBattlefieldZone();
		if (controller.IsRevealed())
		{
			for (int i = 0; i < this.m_targets.Count; i++)
			{
				string cardIdForTarget = base.GetCardIdForTarget(i);
				for (int j = 0; j < battlefieldZone.GetCardCount(); j++)
				{
					Card cardAtIndex = battlefieldZone.GetCardAtIndex(j);
					if (cardAtIndex.GetPredictedZonePosition() == 0)
					{
						Entity entity2 = cardAtIndex.GetEntity();
						string cardId = entity2.GetCardId();
						if (!(cardIdForTarget != cardId))
						{
							if (base.AddUniqueOriginForTarget(i, cardAtIndex))
							{
								break;
							}
						}
					}
				}
			}
		}
		else
		{
			int num = 0;
			for (int k = 0; k < this.m_targets.Count; k++)
			{
				Card cardAtIndex2 = battlefieldZone.GetCardAtIndex(num);
				base.AddOriginForTarget(k, cardAtIndex2);
				num++;
			}
		}
		base.OnAction(prevStateType);
	}
}
