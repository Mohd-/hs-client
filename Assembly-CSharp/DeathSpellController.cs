using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008C8 RID: 2248
public class DeathSpellController : SpellController
{
	// Token: 0x060054C7 RID: 21703 RVA: 0x00195A20 File Offset: 0x00193C20
	protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
	{
		this.AddDeadCardsToTargetList(taskList);
		return this.m_targets.Count != 0;
	}

	// Token: 0x060054C8 RID: 21704 RVA: 0x00195A3C File Offset: 0x00193C3C
	protected override void OnProcessTaskList()
	{
		int num = this.PickDeathSoundCardIndex();
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			Card card = this.m_targets[i];
			card.SuppressDeathSounds(i != num);
			card.ActivateCharacterDeathEffects();
		}
		base.OnProcessTaskList();
	}

	// Token: 0x060054C9 RID: 21705 RVA: 0x00195A94 File Offset: 0x00193C94
	private void AddDeadCardsToTargetList(PowerTaskList taskList)
	{
		List<PowerTask> taskList2 = this.m_taskList.GetTaskList();
		for (int i = 0; i < taskList2.Count; i++)
		{
			Network.PowerHistory power = taskList2[i].GetPower();
			if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = power as Network.HistTagChange;
				if (GameUtils.IsCharacterDeathTagChange(histTagChange))
				{
					Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
					Card card = entity.GetCard();
					if (this.CanAddTarget(entity, card))
					{
						base.AddTarget(card);
					}
				}
			}
		}
	}

	// Token: 0x060054CA RID: 21706 RVA: 0x00195B30 File Offset: 0x00193D30
	private bool CanAddTarget(Entity entity, Card card)
	{
		return !card.WillSuppressDeathEffects();
	}

	// Token: 0x060054CB RID: 21707 RVA: 0x00195B40 File Offset: 0x00193D40
	private int PickDeathSoundCardIndex()
	{
		if (this.m_targets.Count != 1)
		{
			if (this.m_targets.Count == 2)
			{
				Card card = this.m_targets[0];
				Card card2 = this.m_targets[1];
				Entity entity = card.GetEntity();
				Entity entity2 = card2.GetEntity();
				if (this.WasAttackedBy(entity, entity2))
				{
					if (this.CanPlayDeathSound(entity))
					{
						return 0;
					}
					return 1;
				}
				else if (this.WasAttackedBy(entity2, entity))
				{
					if (this.CanPlayDeathSound(entity2))
					{
						return 1;
					}
					return 0;
				}
			}
			return this.PickRandomDeathSoundCardIndex();
		}
		Card card3 = this.m_targets[0];
		Entity entity3 = card3.GetEntity();
		if (this.CanPlayDeathSound(entity3))
		{
			return 0;
		}
		return -1;
	}

	// Token: 0x060054CC RID: 21708 RVA: 0x00195C04 File Offset: 0x00193E04
	private bool WasAttackedBy(Entity defender, Entity attacker)
	{
		return attacker.HasTag(GAME_TAG.ATTACKING) && defender.HasTag(GAME_TAG.DEFENDING) && defender.GetTag(GAME_TAG.LAST_AFFECTED_BY) == attacker.GetEntityId();
	}

	// Token: 0x060054CD RID: 21709 RVA: 0x00195C48 File Offset: 0x00193E48
	private int PickRandomDeathSoundCardIndex()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			Card card = this.m_targets[i];
			Entity entity = card.GetEntity();
			if (this.CanPlayDeathSound(entity))
			{
				list.Add(i);
			}
		}
		if (list.Count == 0)
		{
			return -1;
		}
		return list[Random.Range(0, list.Count)];
	}

	// Token: 0x060054CE RID: 21710 RVA: 0x00195CC6 File Offset: 0x00193EC6
	private bool CanPlayDeathSound(Entity entity)
	{
		return !entity.HasTag(GAME_TAG.DEATHRATTLE_RETURN_ZONE);
	}
}
