using System;

// Token: 0x02000E7D RID: 3709
public class PearlOfTidesSpell : SuperSpell
{
	// Token: 0x06007068 RID: 28776 RVA: 0x00210DA4 File Offset: 0x0020EFA4
	protected override void OnAction(SpellStateType prevStateType)
	{
		foreach (PowerTask powerTask in this.m_taskList.GetTaskList())
		{
			Network.PowerHistory power = powerTask.GetPower();
			Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
			if (histFullEntity != null)
			{
				Entity entity = GameState.Get().GetEntity(histFullEntity.Entity.ID);
				Card card = entity.GetCard();
				card.SuppressPlaySounds(true);
			}
		}
		base.OnAction(prevStateType);
	}
}
