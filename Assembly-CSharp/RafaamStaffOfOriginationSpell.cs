using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E8C RID: 3724
public class RafaamStaffOfOriginationSpell : Spell
{
	// Token: 0x0600709D RID: 28829 RVA: 0x00212AE4 File Offset: 0x00210CE4
	public override bool AddPowerTargets()
	{
		if (!this.m_taskList.DoesBlockHaveMetaDataTasks())
		{
			return false;
		}
		this.m_spawnTaskIndex = -1;
		bool flag = false;
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		for (int i = 0; i < taskList.Count; i++)
		{
			PowerTask powerTask = taskList[i];
			Network.PowerHistory power = powerTask.GetPower();
			Network.HistTagChange histTagChange = power as Network.HistTagChange;
			if (histTagChange != null && histTagChange.Tag == 420)
			{
				flag = true;
			}
			else
			{
				Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
				if (histFullEntity != null)
				{
					if (flag)
					{
						Entity entity = GameState.Get().GetEntity(histFullEntity.Entity.ID);
						Card card = entity.GetCard();
						if (!(card == null))
						{
							this.m_targets.Add(card.gameObject);
							this.m_spawnTaskIndex = i;
							break;
						}
					}
				}
			}
		}
		return this.m_spawnTaskIndex >= 0;
	}

	// Token: 0x0600709E RID: 28830 RVA: 0x00212BE2 File Offset: 0x00210DE2
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		this.ApplyCustomSpawnOverride();
		this.DoTasksUntilSpawn();
	}

	// Token: 0x0600709F RID: 28831 RVA: 0x00212BF8 File Offset: 0x00210DF8
	private void ApplyCustomSpawnOverride()
	{
		foreach (GameObject gameObject in this.m_targets)
		{
			Card component = gameObject.GetComponent<Card>();
			Spell spell = Object.Instantiate<Spell>(this.m_CustomSpawnSpell);
			component.OverrideCustomSpawnSpell(spell);
		}
	}

	// Token: 0x060070A0 RID: 28832 RVA: 0x00212C68 File Offset: 0x00210E68
	private void DoTasksUntilSpawn()
	{
		PowerTaskList.CompleteCallback callback = delegate(PowerTaskList taskList, int startIndex, int count, object userData)
		{
			base.StartCoroutine(this.WaitThenFinish());
		};
		this.m_taskList.DoTasks(0, this.m_spawnTaskIndex, callback);
	}

	// Token: 0x060070A1 RID: 28833 RVA: 0x00212C98 File Offset: 0x00210E98
	private IEnumerator WaitThenFinish()
	{
		List<PowerTask> tasks = this.m_taskList.GetTaskList();
		PowerTask spawnTask = tasks[this.m_spawnTaskIndex];
		Network.HistFullEntity fullEntity = (Network.HistFullEntity)spawnTask.GetPower();
		Entity spawnedEntity = GameState.Get().GetEntity(fullEntity.Entity.ID);
		Card heroPowerCard = spawnedEntity.GetHeroPowerCard();
		Spell electricSpell = heroPowerCard.GetActorSpell(SpellType.ELECTRIC_CHARGE_LEVEL_LARGE, true);
		while (!electricSpell.IsFinished())
		{
			yield return null;
		}
		this.OnStateFinished();
		yield break;
	}

	// Token: 0x040059EE RID: 23022
	public Spell m_CustomSpawnSpell;

	// Token: 0x040059EF RID: 23023
	private int m_spawnTaskIndex;
}
