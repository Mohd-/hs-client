using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000615 RID: 1557
public class SpellController : MonoBehaviour
{
	// Token: 0x0600433D RID: 17213 RVA: 0x0014210C File Offset: 0x0014030C
	public Card GetSource()
	{
		return this.m_source;
	}

	// Token: 0x0600433E RID: 17214 RVA: 0x00142114 File Offset: 0x00140314
	public void SetSource(Card card)
	{
		this.m_source = card;
	}

	// Token: 0x0600433F RID: 17215 RVA: 0x0014211D File Offset: 0x0014031D
	public bool IsSource(Card card)
	{
		return this.m_source == card;
	}

	// Token: 0x06004340 RID: 17216 RVA: 0x0014212B File Offset: 0x0014032B
	public void RemoveSource()
	{
		this.m_source = null;
	}

	// Token: 0x06004341 RID: 17217 RVA: 0x00142134 File Offset: 0x00140334
	public List<Card> GetTargets()
	{
		return this.m_targets;
	}

	// Token: 0x06004342 RID: 17218 RVA: 0x0014213C File Offset: 0x0014033C
	public Card GetTarget()
	{
		return (this.m_targets.Count != 0) ? this.m_targets[0] : null;
	}

	// Token: 0x06004343 RID: 17219 RVA: 0x0014216B File Offset: 0x0014036B
	public void AddTarget(Card card)
	{
		this.m_targets.Add(card);
	}

	// Token: 0x06004344 RID: 17220 RVA: 0x00142179 File Offset: 0x00140379
	public void RemoveTarget(Card card)
	{
		this.m_targets.Remove(card);
	}

	// Token: 0x06004345 RID: 17221 RVA: 0x00142188 File Offset: 0x00140388
	public void RemoveAllTargets()
	{
		this.m_targets.Clear();
	}

	// Token: 0x06004346 RID: 17222 RVA: 0x00142198 File Offset: 0x00140398
	public bool IsTarget(Card card)
	{
		foreach (Card card2 in this.m_targets)
		{
			if (card2 == card)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004347 RID: 17223 RVA: 0x00142204 File Offset: 0x00140404
	public void AddFinishedTaskListCallback(SpellController.FinishedTaskListCallback callback)
	{
		if (this.m_finishedTaskListListeners.Contains(callback))
		{
			return;
		}
		this.m_finishedTaskListListeners.Add(callback);
	}

	// Token: 0x06004348 RID: 17224 RVA: 0x00142224 File Offset: 0x00140424
	public void AddFinishedCallback(SpellController.FinishedCallback callback)
	{
		if (this.m_finishedListeners.Contains(callback))
		{
			return;
		}
		this.m_finishedListeners.Add(callback);
	}

	// Token: 0x06004349 RID: 17225 RVA: 0x00142244 File Offset: 0x00140444
	public bool IsProcessingTaskList()
	{
		return this.m_processingTaskList;
	}

	// Token: 0x0600434A RID: 17226 RVA: 0x0014224C File Offset: 0x0014044C
	public PowerTaskList GetPowerTaskList()
	{
		return this.m_taskList;
	}

	// Token: 0x0600434B RID: 17227 RVA: 0x00142254 File Offset: 0x00140454
	public bool AttachPowerTaskList(PowerTaskList taskList)
	{
		if (this.m_taskList != taskList)
		{
			this.DetachPowerTaskList();
			this.m_taskList = taskList;
		}
		return this.AddPowerSourceAndTargets(taskList);
	}

	// Token: 0x0600434C RID: 17228 RVA: 0x00142277 File Offset: 0x00140477
	public void SetPowerTaskList(PowerTaskList taskList)
	{
		if (this.m_taskList == taskList)
		{
			return;
		}
		this.DetachPowerTaskList();
		this.m_taskList = taskList;
	}

	// Token: 0x0600434D RID: 17229 RVA: 0x00142294 File Offset: 0x00140494
	public PowerTaskList DetachPowerTaskList()
	{
		PowerTaskList taskList = this.m_taskList;
		this.RemoveSource();
		this.RemoveAllTargets();
		this.m_taskList = null;
		return taskList;
	}

	// Token: 0x0600434E RID: 17230 RVA: 0x001422BC File Offset: 0x001404BC
	public void DoPowerTaskList()
	{
		this.m_processingTaskList = true;
		base.gameObject.SetActive(true);
		GameState.Get().AddServerBlockingSpellController(this);
		base.StartCoroutine(this.WaitForCardsThenDoTaskList());
	}

	// Token: 0x0600434F RID: 17231 RVA: 0x001422F4 File Offset: 0x001404F4
	public void ForceKill()
	{
		this.OnFinishedTaskList();
	}

	// Token: 0x06004350 RID: 17232 RVA: 0x001422FC File Offset: 0x001404FC
	protected virtual void OnProcessTaskList()
	{
		this.OnFinishedTaskList();
		this.OnFinished();
	}

	// Token: 0x06004351 RID: 17233 RVA: 0x0014230A File Offset: 0x0014050A
	protected virtual void OnFinishedTaskList()
	{
		GameState.Get().RemoveServerBlockingSpellController(this);
		this.m_processingTaskList = false;
		this.FireFinishedTaskListCallbacks();
		if (this.m_pendingFinish)
		{
			this.m_pendingFinish = false;
			this.OnFinished();
		}
	}

	// Token: 0x06004352 RID: 17234 RVA: 0x0014233D File Offset: 0x0014053D
	protected virtual void OnFinished()
	{
		if (this.m_processingTaskList)
		{
			this.m_pendingFinish = true;
			return;
		}
		base.gameObject.SetActive(false);
		this.FireFinishedCallbacks();
	}

	// Token: 0x06004353 RID: 17235 RVA: 0x00142364 File Offset: 0x00140564
	protected virtual bool AddPowerSourceAndTargets(PowerTaskList taskList)
	{
		if (!this.HasSourceCard(taskList))
		{
			return false;
		}
		if (!SpellUtils.CanAddPowerTargets(taskList))
		{
			return false;
		}
		Entity sourceEntity = taskList.GetSourceEntity();
		Card card = sourceEntity.GetCard();
		this.SetSource(card);
		List<PowerTask> taskList2 = this.m_taskList.GetTaskList();
		for (int i = 0; i < taskList2.Count; i++)
		{
			PowerTask task = taskList2[i];
			Card targetCardFromPowerTask = this.GetTargetCardFromPowerTask(task);
			if (!(targetCardFromPowerTask == null))
			{
				if (!(card == targetCardFromPowerTask))
				{
					if (!this.IsTarget(targetCardFromPowerTask))
					{
						this.AddTarget(targetCardFromPowerTask);
					}
				}
			}
		}
		return card != null || this.m_targets.Count > 0;
	}

	// Token: 0x06004354 RID: 17236 RVA: 0x00142438 File Offset: 0x00140638
	protected virtual bool HasSourceCard(PowerTaskList taskList)
	{
		Entity sourceEntity = taskList.GetSourceEntity();
		if (sourceEntity == null)
		{
			return false;
		}
		Card card = sourceEntity.GetCard();
		return !(card == null);
	}

	// Token: 0x06004355 RID: 17237 RVA: 0x0014246C File Offset: 0x0014066C
	private IEnumerator WaitForCardsThenDoTaskList()
	{
		Card sourceCard = this.GetSource();
		if (sourceCard != null)
		{
			while (this.IsCardBusy(sourceCard))
			{
				yield return null;
			}
		}
		foreach (Card targetCard in this.m_targets)
		{
			if (!(targetCard == null))
			{
				while (this.IsCardBusy(targetCard))
				{
					yield return null;
				}
			}
		}
		this.OnProcessTaskList();
		yield break;
	}

	// Token: 0x06004356 RID: 17238 RVA: 0x00142488 File Offset: 0x00140688
	protected bool IsCardBusy(Card card)
	{
		Entity entity = card.GetEntity();
		return !this.WillEntityLoadCard(entity) && (entity.IsLoadingAssets() || ((!TurnStartManager.Get() || !TurnStartManager.Get().IsCardDrawHandled(card)) && !card.IsActorReady()));
	}

	// Token: 0x06004357 RID: 17239 RVA: 0x001424E8 File Offset: 0x001406E8
	private bool WillEntityLoadCard(Entity entity)
	{
		int entityId = entity.GetEntityId();
		foreach (PowerTask powerTask in this.m_taskList.GetTaskList())
		{
			Network.PowerHistory power = powerTask.GetPower();
			Network.PowerType type = power.Type;
			if (type == Network.PowerType.FULL_ENTITY)
			{
				Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
				if (entityId == histFullEntity.Entity.ID)
				{
					return true;
				}
			}
			else if (type == Network.PowerType.SHOW_ENTITY)
			{
				Network.HistShowEntity histShowEntity = power as Network.HistShowEntity;
				if (entityId == histShowEntity.Entity.ID)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004358 RID: 17240 RVA: 0x001425B8 File Offset: 0x001407B8
	private void FireFinishedTaskListCallbacks()
	{
		SpellController.FinishedTaskListCallback[] array = this.m_finishedTaskListListeners.ToArray();
		this.m_finishedTaskListListeners.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			array[i](this);
		}
	}

	// Token: 0x06004359 RID: 17241 RVA: 0x001425FC File Offset: 0x001407FC
	private void FireFinishedCallbacks()
	{
		SpellController.FinishedCallback[] array = this.m_finishedListeners.ToArray();
		this.m_finishedListeners.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			array[i](this);
		}
	}

	// Token: 0x0600435A RID: 17242 RVA: 0x00142640 File Offset: 0x00140840
	protected Card GetTargetCardFromPowerTask(PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		if (power.Type != Network.PowerType.TAG_CHANGE)
		{
			return null;
		}
		Network.HistTagChange histTagChange = power as Network.HistTagChange;
		Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
		if (entity == null)
		{
			string text = string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", this, histTagChange.Entity);
			Debug.LogWarning(text);
			return null;
		}
		return entity.GetCard();
	}

	// Token: 0x04002AA7 RID: 10919
	public const float FINISH_FUDGE_SEC = 10f;

	// Token: 0x04002AA8 RID: 10920
	private List<SpellController.FinishedTaskListCallback> m_finishedTaskListListeners = new List<SpellController.FinishedTaskListCallback>();

	// Token: 0x04002AA9 RID: 10921
	private List<SpellController.FinishedCallback> m_finishedListeners = new List<SpellController.FinishedCallback>();

	// Token: 0x04002AAA RID: 10922
	protected Card m_source;

	// Token: 0x04002AAB RID: 10923
	protected List<Card> m_targets = new List<Card>();

	// Token: 0x04002AAC RID: 10924
	protected PowerTaskList m_taskList;

	// Token: 0x04002AAD RID: 10925
	protected bool m_processingTaskList;

	// Token: 0x04002AAE RID: 10926
	protected bool m_pendingFinish;

	// Token: 0x020008CC RID: 2252
	// (Invoke) Token: 0x060054FD RID: 21757
	public delegate void FinishedTaskListCallback(SpellController spellController);

	// Token: 0x020008CD RID: 2253
	// (Invoke) Token: 0x06005501 RID: 21761
	public delegate void FinishedCallback(SpellController spellController);
}
