using System;
using System.Collections.Generic;
using PegasusGame;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class PowerProcessor
{
	// Token: 0x06001A62 RID: 6754 RVA: 0x0007B5CD File Offset: 0x000797CD
	public void Clear()
	{
		this.m_powerQueue.Clear();
		this.m_currentTaskList = null;
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x0007B5E1 File Offset: 0x000797E1
	public bool IsBuildingTaskList()
	{
		return this.m_buildingTaskList;
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x0007B5E9 File Offset: 0x000797E9
	public PowerTaskList GetCurrentTaskList()
	{
		return this.m_currentTaskList;
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x0007B5F1 File Offset: 0x000797F1
	public PowerQueue GetPowerQueue()
	{
		return this.m_powerQueue;
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x0007B5F9 File Offset: 0x000797F9
	public bool IsHistoryBlocking()
	{
		return this.m_historyBlocking;
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x0007B601 File Offset: 0x00079801
	public PowerTaskList GetHistoryBlockingTaskList()
	{
		return this.m_historyBlockingTaskList;
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x0007B609 File Offset: 0x00079809
	public void ForceStopHistoryBlocking()
	{
		this.m_historyBlocking = false;
		this.m_historyBlockingTaskList = null;
	}

	// Token: 0x06001A69 RID: 6761 RVA: 0x0007B61C File Offset: 0x0007981C
	public PowerTaskList GetLatestUnendedTaskList()
	{
		int count = this.m_powerQueue.Count;
		if (count == 0)
		{
			return this.m_currentTaskList;
		}
		return this.m_powerQueue[count - 1];
	}

	// Token: 0x06001A6A RID: 6762 RVA: 0x0007B650 File Offset: 0x00079850
	public PowerTaskList GetLastTaskList()
	{
		int count = this.m_powerQueue.Count;
		if (count > 0)
		{
			return this.m_powerQueue[count - 1];
		}
		return this.m_currentTaskList;
	}

	// Token: 0x06001A6B RID: 6763 RVA: 0x0007B685 File Offset: 0x00079885
	public PowerTaskList GetEarlyConcedeTaskList()
	{
		return this.m_earlyConcedeTaskList;
	}

	// Token: 0x06001A6C RID: 6764 RVA: 0x0007B68D File Offset: 0x0007988D
	public bool HasEarlyConcedeTaskList()
	{
		return this.m_earlyConcedeTaskList != null;
	}

	// Token: 0x06001A6D RID: 6765 RVA: 0x0007B69B File Offset: 0x0007989B
	public PowerTaskList GetGameOverTaskList()
	{
		return this.m_gameOverTaskList;
	}

	// Token: 0x06001A6E RID: 6766 RVA: 0x0007B6A3 File Offset: 0x000798A3
	public bool HasGameOverTaskList()
	{
		return this.m_gameOverTaskList != null;
	}

	// Token: 0x06001A6F RID: 6767 RVA: 0x0007B6B4 File Offset: 0x000798B4
	public void ForEachTaskList(Action<int, PowerTaskList> predicate)
	{
		if (this.m_currentTaskList != null)
		{
			predicate.Invoke(-1, this.m_currentTaskList);
		}
		for (int i = 0; i < this.m_powerQueue.Count; i++)
		{
			predicate.Invoke(i, this.m_powerQueue[i]);
		}
	}

	// Token: 0x06001A70 RID: 6768 RVA: 0x0007B708 File Offset: 0x00079908
	public bool HasTaskLists()
	{
		return this.m_currentTaskList != null || this.m_powerQueue.Count > 0;
	}

	// Token: 0x06001A71 RID: 6769 RVA: 0x0007B72C File Offset: 0x0007992C
	public bool HasTaskList(PowerTaskList taskList)
	{
		return taskList != null && (this.m_currentTaskList == taskList || this.m_powerQueue.Contains(taskList));
	}

	// Token: 0x06001A72 RID: 6770 RVA: 0x0007B764 File Offset: 0x00079964
	public void OnPowerHistory(List<Network.PowerHistory> powerList)
	{
		this.m_buildingTaskList = true;
		for (int i = 0; i < powerList.Count; i++)
		{
			PowerTaskList powerTaskList = new PowerTaskList();
			if (this.m_previousStack.Count > 0)
			{
				powerTaskList.SetPrevious(this.m_previousStack.Pop());
				this.m_previousStack.Push(powerTaskList);
			}
			this.BuildTaskList(powerList, ref i, powerTaskList);
		}
		this.m_buildingTaskList = false;
	}

	// Token: 0x06001A73 RID: 6771 RVA: 0x0007B7D4 File Offset: 0x000799D4
	public void ProcessPowerQueue()
	{
		while (GameState.Get().CanProcessPowerQueue())
		{
			if (this.m_busyTaskList != null)
			{
				this.m_busyTaskList = null;
			}
			else
			{
				PowerTaskList powerTaskList = this.m_powerQueue.Peek();
				if (this.m_historyBlockingTaskList != null)
				{
					if (!powerTaskList.IsDescendantOfBlock(this.m_historyBlockingTaskList))
					{
						break;
					}
				}
				else if (HistoryManager.Get() != null && HistoryManager.Get().GetCurrentBigCard() != null)
				{
					break;
				}
				this.OnWillProcessTaskList(powerTaskList);
				if (GameState.Get().IsBusy())
				{
					this.m_busyTaskList = powerTaskList;
					break;
				}
			}
			if (this.CanEarlyConcede())
			{
				if (this.m_earlyConcedeTaskList == null && !this.m_handledFirstEarlyConcede)
				{
					this.DoEarlyConcedeVisuals();
					this.m_handledFirstEarlyConcede = true;
				}
				while (this.m_powerQueue.Count > 0)
				{
					this.m_currentTaskList = this.m_powerQueue.Dequeue();
					this.m_currentTaskList.DebugDump();
					this.CancelSpellsForEarlyConcede(this.m_currentTaskList);
					this.m_currentTaskList.DoEarlyConcedeTasks();
					this.m_currentTaskList = null;
				}
				break;
			}
			this.m_currentTaskList = this.m_powerQueue.Dequeue();
			this.m_currentTaskList.DebugDump();
			this.OnProcessTaskList();
			this.StartCurrentTaskList();
		}
	}

	// Token: 0x06001A74 RID: 6772 RVA: 0x0007B938 File Offset: 0x00079B38
	private int GetNextTaskListId()
	{
		int nextTaskListId = this.m_nextTaskListId;
		this.m_nextTaskListId = ((this.m_nextTaskListId != int.MaxValue) ? (this.m_nextTaskListId + 1) : 1);
		return nextTaskListId;
	}

	// Token: 0x06001A75 RID: 6773 RVA: 0x0007B974 File Offset: 0x00079B74
	private void BuildTaskList(List<Network.PowerHistory> powerList, ref int index, PowerTaskList taskList)
	{
		while (index < powerList.Count)
		{
			Network.PowerHistory powerHistory = powerList[index];
			Network.PowerType type = powerHistory.Type;
			if (type == Network.PowerType.BLOCK_START)
			{
				if (!taskList.IsEmpty())
				{
					this.EnqueueTaskList(taskList);
				}
				PowerTaskList powerTaskList = new PowerTaskList();
				powerTaskList.SetBlockStart((Network.HistBlockStart)powerHistory);
				PowerTaskList origin = taskList.GetOrigin();
				if (origin.IsStartOfBlock())
				{
					powerTaskList.SetParent(origin);
				}
				this.m_previousStack.Push(powerTaskList);
				index++;
				this.BuildTaskList(powerList, ref index, powerTaskList);
				return;
			}
			if (type == Network.PowerType.BLOCK_END)
			{
				taskList.SetBlockEnd((Network.HistBlockEnd)powerHistory);
				if (this.m_previousStack.Count > 0)
				{
					this.m_previousStack.Pop();
					this.EnqueueTaskList(taskList);
					return;
				}
				break;
			}
			else
			{
				PowerTask powerTask = taskList.CreateTask(powerHistory);
				powerTask.DoRealTimeTask(powerList, index);
				index++;
			}
		}
		if (!taskList.IsEmpty())
		{
			this.EnqueueTaskList(taskList);
		}
	}

	// Token: 0x06001A76 RID: 6774 RVA: 0x0007BA6C File Offset: 0x00079C6C
	private void EnqueueTaskList(PowerTaskList taskList)
	{
		taskList.SetId(this.GetNextTaskListId());
		this.m_powerQueue.Enqueue(taskList);
		if (taskList.HasFriendlyConcede())
		{
			this.m_earlyConcedeTaskList = taskList;
		}
		if (taskList.HasGameOver())
		{
			this.m_gameOverTaskList = taskList;
		}
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x0007BAB8 File Offset: 0x00079CB8
	private void OnWillProcessTaskList(PowerTaskList taskList)
	{
		if (ThinkEmoteManager.Get())
		{
			ThinkEmoteManager.Get().NotifyOfActivity();
		}
		if (taskList.IsStartOfBlock())
		{
			Network.HistBlockStart blockStart = taskList.GetBlockStart();
			if (blockStart.BlockType == 7)
			{
				Entity entity = GameState.Get().GetEntity(blockStart.Entity);
				bool flag = entity.GetController().IsOpposingSide();
				if (flag)
				{
					string text = entity.GetCardId();
					if (string.IsNullOrEmpty(text))
					{
						text = this.FindRevealedCardId(taskList);
					}
					GameState.Get().GetGameEntity().NotifyOfOpponentWillPlayCard(text);
				}
			}
		}
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x0007BB4C File Offset: 0x00079D4C
	private string FindRevealedCardId(PowerTaskList taskList)
	{
		Network.HistBlockStart blockStart = taskList.GetBlockStart();
		List<PowerTask> taskList2 = taskList.GetTaskList();
		for (int i = 0; i < taskList2.Count; i++)
		{
			PowerTask powerTask = taskList2[i];
			Network.PowerHistory power = powerTask.GetPower();
			Network.HistShowEntity histShowEntity = power as Network.HistShowEntity;
			if (histShowEntity != null)
			{
				if (histShowEntity.Entity.ID == blockStart.Entity)
				{
					return histShowEntity.Entity.CardID;
				}
			}
		}
		return null;
	}

	// Token: 0x06001A79 RID: 6777 RVA: 0x0007BBD0 File Offset: 0x00079DD0
	private void OnProcessTaskList()
	{
		if (this.m_currentTaskList.IsStartOfBlock())
		{
			Network.HistBlockStart blockStart = this.m_currentTaskList.GetBlockStart();
			if (blockStart.BlockType == 7)
			{
				Entity entity = GameState.Get().GetEntity(blockStart.Entity);
				bool flag = entity.IsControlledByFriendlySidePlayer();
				if (flag)
				{
					GameState.Get().GetGameEntity().NotifyOfFriendlyPlayedCard(entity);
				}
				else
				{
					GameState.Get().GetGameEntity().NotifyOfOpponentPlayedCard(entity);
				}
			}
		}
		this.PrepareHistoryForCurrentTaskList();
	}

	// Token: 0x06001A7A RID: 6778 RVA: 0x0007BC50 File Offset: 0x00079E50
	private void PrepareHistoryForCurrentTaskList()
	{
		Log.Power.Print("PowerProcessor.PrepareHistoryForCurrentTaskList() - m_currentTaskList={0}", new object[]
		{
			this.m_currentTaskList.GetId()
		});
		Network.HistBlockStart blockStart = this.m_currentTaskList.GetBlockStart();
		if (blockStart == null)
		{
			return;
		}
		HistoryBlock.Type blockType = blockStart.BlockType;
		if (blockType == 1)
		{
			AttackType attackType = this.m_currentTaskList.GetAttackType();
			Entity entity = null;
			Entity entity2 = null;
			switch (attackType)
			{
			case AttackType.REGULAR:
				entity = this.m_currentTaskList.GetAttacker();
				entity2 = this.m_currentTaskList.GetDefender();
				break;
			case AttackType.CANCELED:
				entity = this.m_currentTaskList.GetAttacker();
				entity2 = this.m_currentTaskList.GetProposedDefender();
				break;
			}
			if (entity != null && entity2 != null)
			{
				HistoryManager.Get().CreateAttackTile(entity, entity2, this.m_currentTaskList);
				this.m_currentTaskList.SetWillCompleteHistoryEntry(true);
				this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
			}
		}
		else if (blockType == 7)
		{
			Entity entity3 = GameState.Get().GetEntity(blockStart.Entity);
			if (entity3 == null)
			{
				return;
			}
			if (this.m_currentTaskList.IsStartOfBlock())
			{
				if (this.m_currentTaskList.ShouldCreatePlayBlockHistoryTile())
				{
					Entity entity4 = GameState.Get().GetEntity(blockStart.Target);
					HistoryManager.Get().CreatePlayedTile(entity3, entity4);
					this.m_currentTaskList.SetWillCompleteHistoryEntry(true);
				}
				Entity entity5;
				bool fromMetaData;
				if (this.ShouldShowPlayedBigCard(entity3, out entity5, out fromMetaData))
				{
					bool countered = this.m_currentTaskList.WasThePlayedSpellCountered(entity3);
					this.m_historyBlocking = true;
					if (this.m_historyBlockingTaskList == null)
					{
						this.m_historyBlockingTaskList = this.m_currentTaskList;
					}
					HistoryManager.Get().CreatePlayedBigCard(entity5, new HistoryManager.BigCardFinishedCallback(this.OnBigCardFinished), fromMetaData, countered);
				}
			}
			this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
		}
		else if (blockType == 3)
		{
			this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
		}
		else if (blockType == 2)
		{
			this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
		}
		else if (blockType == 5)
		{
			Entity entity6 = GameState.Get().GetEntity(blockStart.Entity);
			if (entity6 == null)
			{
				return;
			}
			if (entity6.IsSecret())
			{
				if (this.m_currentTaskList.IsStartOfBlock())
				{
					HistoryManager.Get().CreateTriggerTile(entity6);
					this.m_currentTaskList.SetWillCompleteHistoryEntry(true);
					bool fromMetaData2;
					Entity bigCardEntity = this.GetBigCardEntity(entity6, out fromMetaData2);
					this.m_historyBlocking = true;
					if (this.m_historyBlockingTaskList == null)
					{
						this.m_historyBlockingTaskList = this.m_currentTaskList;
					}
					HistoryManager.Get().CreateTriggeredBigCard(bigCardEntity, new HistoryManager.BigCardFinishedCallback(this.OnBigCardFinished), fromMetaData2, true);
				}
				this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
			}
			else
			{
				bool flag = false;
				if (!this.m_currentTaskList.IsStartOfBlock())
				{
					PowerTaskList triggerTaskListThatShouldCompleteHistoryEntry = this.GetTriggerTaskListThatShouldCompleteHistoryEntry();
					flag = triggerTaskListThatShouldCompleteHistoryEntry.WillCompleteHistoryEntry();
				}
				else
				{
					PowerHistoryInfo powerHistoryInfo = null;
					string effectCardId = this.m_currentTaskList.GetEffectCardId();
					if (!string.IsNullOrEmpty(effectCardId))
					{
						EntityDef entityDef = DefLoader.Get().GetEntityDef(effectCardId);
						powerHistoryInfo = entityDef.GetPowerHistoryInfo(blockStart.EffectIndex);
					}
					if (powerHistoryInfo != null && powerHistoryInfo.ShouldShowInHistory())
					{
						if (entity6.HasTag(GAME_TAG.HISTORY_PROXY))
						{
							Entity entity7 = GameState.Get().GetEntity(entity6.GetTag(GAME_TAG.HISTORY_PROXY));
							HistoryManager.Get().CreatePlayedTile(entity7, null);
							if (entity6.GetController() != GameState.Get().GetFriendlySidePlayer() || !entity6.HasTag(GAME_TAG.HISTORY_PROXY_NO_BIG_CARD))
							{
								this.m_historyBlocking = true;
								if (this.m_historyBlockingTaskList == null)
								{
									this.m_historyBlockingTaskList = this.m_currentTaskList;
								}
								HistoryManager.Get().CreateTriggeredBigCard(entity7, new HistoryManager.BigCardFinishedCallback(this.OnBigCardFinished), false, false);
							}
						}
						else
						{
							Entity entity8;
							bool fromMetaData3;
							if (this.ShouldShowTriggeredBigCard(entity6, out entity8, out fromMetaData3))
							{
								this.m_historyBlocking = true;
								if (this.m_historyBlockingTaskList == null)
								{
									this.m_historyBlockingTaskList = this.m_currentTaskList;
								}
								HistoryManager.Get().CreateTriggeredBigCard(entity8, new HistoryManager.BigCardFinishedCallback(this.OnBigCardFinished), fromMetaData3, false);
							}
							HistoryManager.Get().CreateTriggerTile(entity6);
						}
						PowerTaskList triggerTaskListThatShouldCompleteHistoryEntry2 = this.GetTriggerTaskListThatShouldCompleteHistoryEntry();
						triggerTaskListThatShouldCompleteHistoryEntry2.SetWillCompleteHistoryEntry(true);
						flag = true;
					}
				}
				if (flag)
				{
					this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
				}
			}
		}
		else if (blockType == 8)
		{
			if (this.m_currentTaskList.IsStartOfBlock())
			{
				HistoryManager.Get().CreateFatigueTile();
				this.m_currentTaskList.SetWillCompleteHistoryEntry(true);
			}
			this.m_currentTaskList.NotifyHistoryOfAdditionalTargets();
		}
	}

	// Token: 0x06001A7B RID: 6779 RVA: 0x0007C0C1 File Offset: 0x0007A2C1
	private void OnBigCardFinished()
	{
		this.m_historyBlocking = false;
	}

	// Token: 0x06001A7C RID: 6780 RVA: 0x0007C0CC File Offset: 0x0007A2CC
	private bool ShouldShowPlayedBigCard(Entity sourceEntity, out Entity bigCardEntity, out bool metaDataHasBigCard)
	{
		bigCardEntity = this.GetBigCardEntity(sourceEntity, out metaDataHasBigCard);
		return metaDataHasBigCard || GameMgr.Get().IsSpectator() || sourceEntity.IsControlledByOpposingSidePlayer();
	}

	// Token: 0x06001A7D RID: 6781 RVA: 0x0007C10C File Offset: 0x0007A30C
	private bool ShouldShowTriggeredBigCard(Entity sourceEntity, out Entity bigCardEntity, out bool metaDataHasBigCard)
	{
		bigCardEntity = this.GetBigCardEntity(sourceEntity, out metaDataHasBigCard);
		return metaDataHasBigCard || (sourceEntity.GetZone() == TAG_ZONE.HAND && sourceEntity.HasTriggerVisual());
	}

	// Token: 0x06001A7E RID: 6782 RVA: 0x0007C148 File Offset: 0x0007A348
	private Entity GetBigCardEntity(Entity sourceEntity, out bool metaDataHasBigCard)
	{
		metaDataHasBigCard = false;
		foreach (PowerTask powerTask in this.m_currentTaskList.GetTaskList())
		{
			Network.PowerHistory power = powerTask.GetPower();
			Network.HistMetaData histMetaData = power as Network.HistMetaData;
			if (histMetaData != null)
			{
				if (histMetaData.MetaType == 4)
				{
					if (histMetaData.Data == 1)
					{
						if (histMetaData.Info.Count >= 1)
						{
							int id = histMetaData.Info[0];
							Entity entity = GameState.Get().GetEntity(id);
							if (entity == null)
							{
								return null;
							}
							metaDataHasBigCard = true;
							return entity;
						}
					}
				}
			}
		}
		return sourceEntity;
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x0007C230 File Offset: 0x0007A430
	private PowerTaskList GetTriggerTaskListThatShouldCompleteHistoryEntry()
	{
		HistoryBlock.Type blockType = this.m_currentTaskList.GetBlockType();
		if (blockType != 5)
		{
			return null;
		}
		PowerTaskList parent = this.m_currentTaskList.GetParent();
		if (parent != null)
		{
			HistoryBlock.Type blockType2 = parent.GetBlockType();
			if (blockType2 == 9)
			{
				return parent;
			}
		}
		return this.m_currentTaskList;
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x0007C27C File Offset: 0x0007A47C
	private bool CanEarlyConcede()
	{
		if (this.m_earlyConcedeTaskList != null)
		{
			return true;
		}
		if (GameState.Get().IsGameOver())
		{
			return false;
		}
		if (GameState.Get().WasConcedeRequested())
		{
			Network.HistTagChange realTimeGameOverTagChange = GameState.Get().GetRealTimeGameOverTagChange();
			if (realTimeGameOverTagChange != null && realTimeGameOverTagChange.Value != 4)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x0007C2D8 File Offset: 0x0007A4D8
	private void DoEarlyConcedeVisuals()
	{
		Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
		if (friendlySidePlayer != null)
		{
			friendlySidePlayer.PlayConcedeEmote();
		}
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x0007C2FC File Offset: 0x0007A4FC
	private void CancelSpellsForEarlyConcede(PowerTaskList taskList)
	{
		Entity sourceEntity = taskList.GetSourceEntity();
		if (sourceEntity == null)
		{
			return;
		}
		Card card = sourceEntity.GetCard();
		if (!card)
		{
			return;
		}
		Network.HistBlockStart blockStart = taskList.GetBlockStart();
		if (blockStart.BlockType != 3)
		{
			return;
		}
		Spell playSpell = card.GetPlaySpell(true);
		if (!playSpell)
		{
			return;
		}
		SpellStateType activeState = playSpell.GetActiveState();
		if (activeState != SpellStateType.NONE && activeState != SpellStateType.CANCEL)
		{
			playSpell.ActivateState(SpellStateType.CANCEL);
		}
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x0007C370 File Offset: 0x0007A570
	private void StartCurrentTaskList()
	{
		GameState gameState = GameState.Get();
		Network.HistBlockStart blockStart = this.m_currentTaskList.GetBlockStart();
		if (blockStart == null)
		{
			this.DoCurrentTaskList();
			return;
		}
		int entity = blockStart.Entity;
		Entity entity2 = gameState.GetEntity(entity);
		if (entity2 == null)
		{
			Debug.LogErrorFormat("PowerProcessor.StartCurrentTaskList() - WARNING got a power with a null source entity (ID={0})", new object[]
			{
				entity
			});
			this.DoCurrentTaskList();
			return;
		}
		if (!this.DoTaskListWithSpellController(gameState, this.m_currentTaskList, entity2))
		{
			this.DoCurrentTaskList();
			return;
		}
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x0007C3EB File Offset: 0x0007A5EB
	private void DoCurrentTaskList()
	{
		this.m_currentTaskList.DoAllTasks(delegate(PowerTaskList taskList, int startIndex, int count, object userData)
		{
			this.EndCurrentTaskList();
		});
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x0007C404 File Offset: 0x0007A604
	private void EndCurrentTaskList()
	{
		Log.Power.Print("PowerProcessor.EndCurrentTaskList() - m_currentTaskList={0}", new object[]
		{
			(this.m_currentTaskList != null) ? this.m_currentTaskList.GetId().ToString() : "null"
		});
		if (this.m_currentTaskList == null)
		{
			GameState.Get().OnTaskListEnded(null);
			return;
		}
		Network.HistBlockEnd blockEnd = this.m_currentTaskList.GetBlockEnd();
		if (blockEnd != null)
		{
			if (this.m_currentTaskList.IsInBlock(this.m_historyBlockingTaskList))
			{
				this.m_historyBlockingTaskList = null;
			}
			if (this.m_currentTaskList.IsRitualBlock() && HistoryManager.Get().HasHistoryEntry())
			{
				this.AddCthunToHistory();
			}
			if (this.m_currentTaskList.WillBlockCompleteHistoryEntry())
			{
				HistoryManager.Get().MarkCurrentHistoryEntryAsCompleted();
			}
		}
		GameState.Get().OnTaskListEnded(this.m_currentTaskList);
		this.m_currentTaskList = null;
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x0007C4F0 File Offset: 0x0007A6F0
	private void AddCthunToHistory()
	{
		PowerTaskList origin = this.m_currentTaskList.GetOrigin();
		Entity ritualEntityClone = origin.GetRitualEntityClone();
		if (ritualEntityClone == null)
		{
			return;
		}
		HistoryManager.Get().NotifyEntityAffected(ritualEntityClone, true);
		Entity sourceEntity = this.m_currentTaskList.GetSourceEntity();
		Player controller = sourceEntity.GetController();
		int tag = controller.GetTag(GAME_TAG.PROXY_CTHUN);
		Entity entity = GameState.Get().GetEntity(tag);
		if (entity == null)
		{
			return;
		}
		if (entity.GetTag(GAME_TAG.ATK) != ritualEntityClone.GetTag(GAME_TAG.ATK) || entity.GetTag(GAME_TAG.HEALTH) != ritualEntityClone.GetTag(GAME_TAG.HEALTH) || entity.GetTag(GAME_TAG.TAUNT) != ritualEntityClone.GetTag(GAME_TAG.TAUNT))
		{
			HistoryManager.Get().NotifyEntityAffected(entity, true);
		}
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x0007C5B0 File Offset: 0x0007A7B0
	private bool DoTaskListWithSpellController(GameState state, PowerTaskList taskList, Entity sourceEntity)
	{
		HistoryBlock.Type blockType = taskList.GetBlockType();
		if (blockType == 1)
		{
			AttackSpellController spellController = this.CreateAttackSpellController(taskList);
			if (!this.DoTaskListUsingController(spellController, taskList))
			{
				this.DestroySpellController(spellController);
				return false;
			}
			return true;
		}
		else if (blockType == 3)
		{
			PowerSpellController spellController2 = this.CreatePowerSpellController(taskList);
			if (!this.DoTaskListUsingController(spellController2, taskList))
			{
				this.DestroySpellController(spellController2);
				return false;
			}
			return true;
		}
		else
		{
			if (blockType == 5)
			{
				if (sourceEntity.IsSecret())
				{
					SecretSpellController spellController3 = this.CreateSecretSpellController(taskList);
					if (!this.DoTaskListUsingController(spellController3, taskList))
					{
						this.DestroySpellController(spellController3);
						return false;
					}
				}
				else
				{
					TriggerSpellController triggerSpellController = this.CreateTriggerSpellController(taskList);
					Card card = sourceEntity.GetCard();
					if (TurnStartManager.Get().IsCardDrawHandled(card))
					{
						if (!triggerSpellController.AttachPowerTaskList(taskList))
						{
							this.DestroySpellController(triggerSpellController);
							return false;
						}
						triggerSpellController.AddFinishedTaskListCallback(new SpellController.FinishedTaskListCallback(this.OnSpellControllerFinishedTaskList));
						triggerSpellController.AddFinishedCallback(new SpellController.FinishedCallback(this.OnSpellControllerFinished));
						TurnStartManager.Get().NotifyOfSpellController(triggerSpellController);
					}
					else if (!this.DoTaskListUsingController(triggerSpellController, taskList))
					{
						this.DestroySpellController(triggerSpellController);
						return false;
					}
				}
				return true;
			}
			if (blockType == 6)
			{
				DeathSpellController spellController4 = this.CreateDeathSpellController(taskList);
				if (!this.DoTaskListUsingController(spellController4, taskList))
				{
					this.DestroySpellController(spellController4);
					return false;
				}
				return true;
			}
			else if (blockType == 8)
			{
				FatigueSpellController fatigueSpellController = this.CreateFatigueSpellController(taskList);
				if (!fatigueSpellController.AttachPowerTaskList(taskList))
				{
					this.DestroySpellController(fatigueSpellController);
					return false;
				}
				fatigueSpellController.AddFinishedTaskListCallback(new SpellController.FinishedTaskListCallback(this.OnSpellControllerFinishedTaskList));
				fatigueSpellController.AddFinishedCallback(new SpellController.FinishedCallback(this.OnSpellControllerFinished));
				if (state.IsTurnStartManagerActive())
				{
					TurnStartManager.Get().NotifyOfSpellController(fatigueSpellController);
				}
				else
				{
					fatigueSpellController.DoPowerTaskList();
				}
				return true;
			}
			else if (blockType == 2)
			{
				JoustSpellController spellController5 = this.CreateJoustSpellController(taskList);
				if (!this.DoTaskListUsingController(spellController5, taskList))
				{
					this.DestroySpellController(spellController5);
					return false;
				}
				return true;
			}
			else
			{
				if (blockType != 9)
				{
					Log.Power.Print("PowerProcessor.DoTaskListForCard() - unhandled BlockType {0} for sourceEntity {1}", new object[]
					{
						blockType,
						sourceEntity
					});
					return false;
				}
				RitualSpellController spellController6 = this.CreateRitualSpellController(taskList);
				if (!this.DoTaskListUsingController(spellController6, taskList))
				{
					this.DestroySpellController(spellController6);
					return false;
				}
				return true;
			}
		}
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x0007C7E8 File Offset: 0x0007A9E8
	private bool DoTaskListUsingController(SpellController spellController, PowerTaskList taskList)
	{
		if (spellController == null)
		{
			Log.Power.Print("PowerProcessor.DoTaskListUsingController() - spellController=null", new object[0]);
			return false;
		}
		if (!spellController.AttachPowerTaskList(taskList))
		{
			return false;
		}
		spellController.AddFinishedTaskListCallback(new SpellController.FinishedTaskListCallback(this.OnSpellControllerFinishedTaskList));
		spellController.AddFinishedCallback(new SpellController.FinishedCallback(this.OnSpellControllerFinished));
		spellController.DoPowerTaskList();
		return true;
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x0007C851 File Offset: 0x0007AA51
	private void OnSpellControllerFinishedTaskList(SpellController spellController)
	{
		spellController.DetachPowerTaskList();
		if (this.m_currentTaskList == null)
		{
			return;
		}
		this.DoCurrentTaskList();
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x0007C86C File Offset: 0x0007AA6C
	private void OnSpellControllerFinished(SpellController spellController)
	{
		this.DestroySpellController(spellController);
	}

	// Token: 0x06001A8B RID: 6795 RVA: 0x0007C875 File Offset: 0x0007AA75
	private AttackSpellController CreateAttackSpellController(PowerTaskList taskList)
	{
		return this.CreateSpellController<AttackSpellController>(taskList, "AttackSpellController");
	}

	// Token: 0x06001A8C RID: 6796 RVA: 0x0007C883 File Offset: 0x0007AA83
	private SecretSpellController CreateSecretSpellController(PowerTaskList taskList)
	{
		return this.CreateSpellController<SecretSpellController>(taskList, "SecretSpellController");
	}

	// Token: 0x06001A8D RID: 6797 RVA: 0x0007C891 File Offset: 0x0007AA91
	private PowerSpellController CreatePowerSpellController(PowerTaskList taskList)
	{
		return this.CreateSpellController<PowerSpellController>(taskList, null);
	}

	// Token: 0x06001A8E RID: 6798 RVA: 0x0007C89B File Offset: 0x0007AA9B
	private TriggerSpellController CreateTriggerSpellController(PowerTaskList taskList)
	{
		return this.CreateSpellController<TriggerSpellController>(taskList, null);
	}

	// Token: 0x06001A8F RID: 6799 RVA: 0x0007C8A5 File Offset: 0x0007AAA5
	private DeathSpellController CreateDeathSpellController(PowerTaskList taskList)
	{
		return this.CreateSpellController<DeathSpellController>(taskList, null);
	}

	// Token: 0x06001A90 RID: 6800 RVA: 0x0007C8AF File Offset: 0x0007AAAF
	private FatigueSpellController CreateFatigueSpellController(PowerTaskList taskList)
	{
		return this.CreateSpellController<FatigueSpellController>(taskList, null);
	}

	// Token: 0x06001A91 RID: 6801 RVA: 0x0007C8B9 File Offset: 0x0007AAB9
	private JoustSpellController CreateJoustSpellController(PowerTaskList taskList)
	{
		return this.CreateSpellController<JoustSpellController>(taskList, "JoustSpellController");
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x0007C8C7 File Offset: 0x0007AAC7
	private RitualSpellController CreateRitualSpellController(PowerTaskList taskList)
	{
		return this.CreateSpellController<RitualSpellController>(taskList, "RitualSpellController");
	}

	// Token: 0x06001A93 RID: 6803 RVA: 0x0007C8D8 File Offset: 0x0007AAD8
	private T CreateSpellController<T>(PowerTaskList taskList, string prefabName = null) where T : SpellController
	{
		GameObject gameObject;
		T result;
		if (prefabName == null)
		{
			gameObject = new GameObject();
			result = gameObject.AddComponent<T>();
		}
		else
		{
			gameObject = AssetLoader.Get().LoadGameObject(prefabName, true, false);
			result = gameObject.GetComponent<T>();
		}
		gameObject.name = string.Format("{0} [taskListId={1}]", typeof(T), taskList.GetId());
		return result;
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x0007C938 File Offset: 0x0007AB38
	private void DestroySpellController(SpellController spellController)
	{
		Object.Destroy(spellController.gameObject);
	}

	// Token: 0x04000D39 RID: 3385
	private const string ATTACK_SPELL_CONTROLLER_PREFAB_NAME = "AttackSpellController";

	// Token: 0x04000D3A RID: 3386
	private const string SECRET_SPELL_CONTROLLER_PREFAB_NAME = "SecretSpellController";

	// Token: 0x04000D3B RID: 3387
	private const string JOUST_SPELL_CONTROLLER_PREFAB_NAME = "JoustSpellController";

	// Token: 0x04000D3C RID: 3388
	private const string RITUAL_SPELL_CONTROLLER_PREFAB_NAME = "RitualSpellController";

	// Token: 0x04000D3D RID: 3389
	private int m_nextTaskListId = 1;

	// Token: 0x04000D3E RID: 3390
	private bool m_buildingTaskList;

	// Token: 0x04000D3F RID: 3391
	private Stack<PowerTaskList> m_previousStack = new Stack<PowerTaskList>();

	// Token: 0x04000D40 RID: 3392
	private PowerQueue m_powerQueue = new PowerQueue();

	// Token: 0x04000D41 RID: 3393
	private PowerTaskList m_currentTaskList;

	// Token: 0x04000D42 RID: 3394
	private bool m_historyBlocking;

	// Token: 0x04000D43 RID: 3395
	private PowerTaskList m_historyBlockingTaskList;

	// Token: 0x04000D44 RID: 3396
	private PowerTaskList m_busyTaskList;

	// Token: 0x04000D45 RID: 3397
	private PowerTaskList m_earlyConcedeTaskList;

	// Token: 0x04000D46 RID: 3398
	private bool m_handledFirstEarlyConcede;

	// Token: 0x04000D47 RID: 3399
	private PowerTaskList m_gameOverTaskList;
}
