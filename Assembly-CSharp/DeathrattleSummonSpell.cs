using System;
using UnityEngine;

// Token: 0x02000ABA RID: 2746
public class DeathrattleSummonSpell : Spell
{
	// Token: 0x06005F02 RID: 24322 RVA: 0x001C7024 File Offset: 0x001C5224
	protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		if (power.Type != Network.PowerType.FULL_ENTITY)
		{
			return null;
		}
		Network.HistFullEntity histFullEntity = (Network.HistFullEntity)power;
		Network.Entity entity = histFullEntity.Entity;
		Entity entity2 = GameState.Get().GetEntity(entity.ID);
		if (entity2 == null)
		{
			string text = string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", this, entity.ID);
			Debug.LogWarning(text);
			return null;
		}
		return entity2.GetCard();
	}

	// Token: 0x06005F03 RID: 24323 RVA: 0x001C7094 File Offset: 0x001C5294
	protected override void OnAction(SpellStateType prevStateType)
	{
		Card sourceCard = base.GetSourceCard();
		foreach (GameObject gameObject in this.m_targets)
		{
			Card component = gameObject.GetComponent<Card>();
			component.transform.position = sourceCard.transform.position;
			float num = 0.2f;
			component.transform.localScale = new Vector3(num, num, num);
			component.SetTransitionStyle(ZoneTransitionStyle.VERY_SLOW);
			component.SetDoNotWarpToNewZone(true);
		}
		base.OnBirth(prevStateType);
		this.OnSpellFinished();
	}
}
