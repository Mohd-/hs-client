using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000EA7 RID: 3751
public class VarianWrynn : SuperSpell
{
	// Token: 0x06007116 RID: 28950 RVA: 0x00215624 File Offset: 0x00213824
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		base.StartCoroutine(this.DoVariansCoolThing());
	}

	// Token: 0x06007117 RID: 28951 RVA: 0x00215654 File Offset: 0x00213854
	private IEnumerator DoVariansCoolThing()
	{
		Card sourceCard = this.m_taskList.GetSourceEntity().GetCard();
		List<GameObject> fxObjects = new List<GameObject>();
		if (this.m_varianSpellPrefab != null && this.m_taskList.IsOrigin())
		{
			Spell spell = Object.Instantiate<Spell>(this.m_varianSpellPrefab);
			fxObjects.Add(spell.gameObject);
			spell.SetSource(sourceCard.gameObject);
			spell.Activate();
		}
		List<PowerTask> tasks = this.m_taskList.GetTaskList();
		bool foundTarget = false;
		bool lastWasMinion = false;
		for (int i = 0; i < tasks.Count; i++)
		{
			Network.PowerHistory power = tasks[i].GetPower();
			if (power.Type == Network.PowerType.SHOW_ENTITY)
			{
				Network.HistShowEntity showEntity = (Network.HistShowEntity)power;
				if (!foundTarget)
				{
					Entity entity = GameState.Get().GetEntity(showEntity.Entity.ID);
					Card targetCard = entity.GetCard();
					foundTarget = true;
					if (this.m_deckSpellPrefab != null && this.m_taskList.IsOrigin())
					{
						Spell spell2 = Object.Instantiate<Spell>(this.m_deckSpellPrefab);
						fxObjects.Add(spell2.gameObject);
						spell2.SetSource(targetCard.gameObject);
						spell2.Activate();
						while (!spell2.IsFinished())
						{
							yield return null;
						}
					}
				}
				bool complete = false;
				PowerTaskList.CompleteCallback completeCallback = delegate(PowerTaskList taskList, int startIndex, int count, object userData)
				{
					complete = true;
				};
				this.m_taskList.DoTasks(0, i, completeCallback);
				if (lastWasMinion)
				{
					yield return new WaitForSeconds(this.m_spellLeadTime);
				}
				lastWasMinion = this.IsMinion(showEntity);
				while (!complete)
				{
					yield return null;
				}
			}
		}
		foreach (GameObject fxObj in fxObjects)
		{
			Object.Destroy(fxObj);
		}
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
		yield break;
	}

	// Token: 0x06007118 RID: 28952 RVA: 0x00215670 File Offset: 0x00213870
	private bool IsMinion(Network.HistShowEntity showEntity)
	{
		for (int i = 0; i < showEntity.Entity.Tags.Count; i++)
		{
			Network.Entity.Tag tag = showEntity.Entity.Tags[i];
			if (tag.Name == 202)
			{
				return tag.Value == 4;
			}
		}
		return false;
	}

	// Token: 0x04005AA9 RID: 23209
	public string m_perMinionSound;

	// Token: 0x04005AAA RID: 23210
	public Spell m_varianSpellPrefab;

	// Token: 0x04005AAB RID: 23211
	public Spell m_deckSpellPrefab;

	// Token: 0x04005AAC RID: 23212
	public float m_spellLeadTime = 1f;
}
