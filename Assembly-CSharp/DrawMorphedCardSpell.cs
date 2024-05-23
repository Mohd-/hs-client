using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E5C RID: 3676
public class DrawMorphedCardSpell : SuperSpell
{
	// Token: 0x06006F87 RID: 28551 RVA: 0x0020BC08 File Offset: 0x00209E08
	public override bool AddPowerTargets()
	{
		this.m_revealTaskIndex = -1;
		if (!base.CanAddPowerTargets())
		{
			return false;
		}
		this.FindOldAndNewCards();
		return this.m_oldCard && this.m_newCard;
	}

	// Token: 0x06006F88 RID: 28552 RVA: 0x0020BC52 File Offset: 0x00209E52
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_oldCard.SetHoldingForLinkedCardSwitch(true);
		this.m_newCard.SetHoldingForLinkedCardSwitch(true);
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
	}

	// Token: 0x06006F89 RID: 28553 RVA: 0x0020BC81 File Offset: 0x00209E81
	protected override void DoActionNow()
	{
		if (this.m_revealTaskIndex < 0)
		{
			this.BeginEffects();
		}
		else
		{
			this.m_taskList.DoTasks(0, this.m_revealTaskIndex + 1, new PowerTaskList.CompleteCallback(this.OnRevealTasksComplete));
		}
	}

	// Token: 0x06006F8A RID: 28554 RVA: 0x0020BCBC File Offset: 0x00209EBC
	private void FindOldAndNewCards()
	{
		int revealTaskIndex = -1;
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		for (int i = 0; i < taskList.Count; i++)
		{
			PowerTask powerTask = taskList[i];
			Network.PowerHistory power = powerTask.GetPower();
			Network.PowerType type = power.Type;
			if (type != Network.PowerType.FULL_ENTITY)
			{
				if (type == Network.PowerType.SHOW_ENTITY)
				{
					Network.HistShowEntity histShowEntity = (Network.HistShowEntity)power;
					Entity entity = GameState.Get().GetEntity(histShowEntity.Entity.ID);
					if (entity != null)
					{
						Card card = entity.GetCard();
						if (!(card == null))
						{
							if (entity.GetZone() == TAG_ZONE.DECK)
							{
								if (histShowEntity.Entity.Tags.Find((Network.Entity.Tag tag) => tag.Name == 49 && tag.Value == 3) != null)
								{
									this.m_oldCard = card;
									revealTaskIndex = i;
								}
							}
						}
					}
				}
			}
			else
			{
				Network.HistFullEntity histFullEntity = (Network.HistFullEntity)power;
				Entity entity2 = GameState.Get().GetEntity(histFullEntity.Entity.ID);
				if (entity2 != null)
				{
					Card card2 = entity2.GetCard();
					if (!(card2 == null))
					{
						this.m_newCard = card2;
					}
				}
			}
		}
		if (!this.m_oldCard || !this.m_newCard)
		{
			return;
		}
		this.m_revealTaskIndex = revealTaskIndex;
		this.AddTarget(this.m_oldCard.gameObject);
	}

	// Token: 0x06006F8B RID: 28555 RVA: 0x0020BE4F File Offset: 0x0020A04F
	private void OnRevealTasksComplete(PowerTaskList taskList, int startIndex, int count, object userData)
	{
		this.BeginEffects();
	}

	// Token: 0x06006F8C RID: 28556 RVA: 0x0020BE57 File Offset: 0x0020A057
	private void BeginEffects()
	{
		base.DoActionNow();
		base.StartCoroutine(this.HoldOldCard());
	}

	// Token: 0x06006F8D RID: 28557 RVA: 0x0020BE6C File Offset: 0x0020A06C
	private IEnumerator HoldOldCard()
	{
		yield return new WaitForSeconds(this.m_OldCardHoldTime);
		this.m_taskList.DoAllTasks(new PowerTaskList.CompleteCallback(this.OnAllTasksComplete));
		yield break;
	}

	// Token: 0x06006F8E RID: 28558 RVA: 0x0020BE87 File Offset: 0x0020A087
	private void OnAllTasksComplete(PowerTaskList taskList, int startIndex, int count, object userData)
	{
		base.StartCoroutine(this.HoldNewCard());
	}

	// Token: 0x06006F8F RID: 28559 RVA: 0x0020BE98 File Offset: 0x0020A098
	private IEnumerator HoldNewCard()
	{
		yield return new WaitForSeconds(this.m_NewCardHoldTime);
		this.m_oldCard.SetHoldingForLinkedCardSwitch(false);
		this.m_newCard.SetHoldingForLinkedCardSwitch(false);
		this.m_oldCard = null;
		this.m_newCard = null;
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
		yield break;
	}

	// Token: 0x040058AA RID: 22698
	public float m_OldCardHoldTime;

	// Token: 0x040058AB RID: 22699
	public float m_NewCardHoldTime;

	// Token: 0x040058AC RID: 22700
	private Card m_oldCard;

	// Token: 0x040058AD RID: 22701
	private int m_revealTaskIndex = -1;

	// Token: 0x040058AE RID: 22702
	private Card m_newCard;
}
