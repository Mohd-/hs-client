using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E60 RID: 3680
public class EchoingOozeSpell : Spell
{
	// Token: 0x06006FA0 RID: 28576 RVA: 0x0020C180 File Offset: 0x0020A380
	protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
		if (histFullEntity == null)
		{
			return null;
		}
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

	// Token: 0x06006FA1 RID: 28577 RVA: 0x0020C1E8 File Offset: 0x0020A3E8
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		Card targetCard = base.GetTargetCard();
		if (targetCard == null)
		{
			this.OnStateFinished();
			return;
		}
		this.DoEffect(targetCard);
	}

	// Token: 0x06006FA2 RID: 28578 RVA: 0x0020C220 File Offset: 0x0020A420
	private void DoEffect(Card targetCard)
	{
		Spell spell = Object.Instantiate<Spell>(this.m_CustomSpawnSpell);
		targetCard.OverrideCustomSpawnSpell(spell);
		this.DoTasksUntilSpawn(targetCard);
		base.StartCoroutine(this.WaitThenFinish());
	}

	// Token: 0x06006FA3 RID: 28579 RVA: 0x0020C254 File Offset: 0x0020A454
	private void DoTasksUntilSpawn(Card targetCard)
	{
		Entity entity = targetCard.GetEntity();
		int entityId = entity.GetEntityId();
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		int num = 0;
		for (int i = 0; i < taskList.Count; i++)
		{
			PowerTask powerTask = taskList[i];
			Network.PowerHistory power = powerTask.GetPower();
			Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
			if (histFullEntity != null)
			{
				if (histFullEntity.Entity.ID == entityId)
				{
					num = i;
					break;
				}
			}
		}
		this.m_taskList.DoTasks(0, num + 1);
	}

	// Token: 0x06006FA4 RID: 28580 RVA: 0x0020C2F0 File Offset: 0x0020A4F0
	private IEnumerator WaitThenFinish()
	{
		float delaySec = Random.Range(this.m_PostSpawnDelayMin, this.m_PostSpawnDelayMax);
		if (!object.Equals(delaySec, 0f))
		{
			yield return new WaitForSeconds(delaySec);
		}
		this.OnStateFinished();
		yield break;
	}

	// Token: 0x040058B6 RID: 22710
	public Spell m_CustomSpawnSpell;

	// Token: 0x040058B7 RID: 22711
	public float m_PostSpawnDelayMin;

	// Token: 0x040058B8 RID: 22712
	public float m_PostSpawnDelayMax;
}
