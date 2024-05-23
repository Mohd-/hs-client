using System;
using System.Collections;
using System.Collections.Generic;
using PegasusGame;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class PowerTaskList
{
	// Token: 0x06001A97 RID: 6807 RVA: 0x0007C960 File Offset: 0x0007AB60
	public int GetId()
	{
		return this.m_id;
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x0007C968 File Offset: 0x0007AB68
	public void SetId(int id)
	{
		this.m_id = id;
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x0007C974 File Offset: 0x0007AB74
	public bool IsEmpty()
	{
		PowerTaskList origin = this.GetOrigin();
		return origin.m_blockStart == null && origin.m_blockEnd == null && origin.m_tasks.Count <= 0;
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x0007C9B6 File Offset: 0x0007ABB6
	public bool IsOrigin()
	{
		return this.m_previous == null;
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x0007C9C4 File Offset: 0x0007ABC4
	public PowerTaskList GetOrigin()
	{
		PowerTaskList powerTaskList = this;
		while (powerTaskList.m_previous != null)
		{
			powerTaskList = powerTaskList.m_previous;
		}
		return powerTaskList;
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x0007C9EB File Offset: 0x0007ABEB
	public PowerTaskList GetPrevious()
	{
		return this.m_previous;
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x0007C9F3 File Offset: 0x0007ABF3
	public void SetPrevious(PowerTaskList taskList)
	{
		this.m_previous = taskList;
		taskList.m_next = this;
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x0007CA03 File Offset: 0x0007AC03
	public PowerTaskList GetNext()
	{
		return this.m_next;
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x0007CA0C File Offset: 0x0007AC0C
	public PowerTaskList GetLast()
	{
		PowerTaskList powerTaskList = this;
		while (powerTaskList.m_next != null)
		{
			powerTaskList = powerTaskList.m_next;
		}
		return powerTaskList;
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x0007CA34 File Offset: 0x0007AC34
	public Network.HistBlockStart GetBlockStart()
	{
		PowerTaskList origin = this.GetOrigin();
		return origin.m_blockStart;
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x0007CA4E File Offset: 0x0007AC4E
	public void SetBlockStart(Network.HistBlockStart blockStart)
	{
		this.m_blockStart = blockStart;
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x0007CA57 File Offset: 0x0007AC57
	public Network.HistBlockEnd GetBlockEnd()
	{
		return this.m_blockEnd;
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x0007CA5F File Offset: 0x0007AC5F
	public void SetBlockEnd(Network.HistBlockEnd blockEnd)
	{
		this.m_blockEnd = blockEnd;
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x0007CA68 File Offset: 0x0007AC68
	public PowerTaskList GetParent()
	{
		PowerTaskList origin = this.GetOrigin();
		return origin.m_parent;
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x0007CA82 File Offset: 0x0007AC82
	public void SetParent(PowerTaskList parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x06001AA6 RID: 6822 RVA: 0x0007CA8C File Offset: 0x0007AC8C
	public bool IsBlock()
	{
		PowerTaskList origin = this.GetOrigin();
		return origin.m_blockStart != null;
	}

	// Token: 0x06001AA7 RID: 6823 RVA: 0x0007CAAC File Offset: 0x0007ACAC
	public bool IsStartOfBlock()
	{
		return this.IsBlock() && this.m_blockStart != null;
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x0007CAC7 File Offset: 0x0007ACC7
	public bool IsEndOfBlock()
	{
		return this.IsBlock() && this.m_blockEnd != null;
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x0007CAE4 File Offset: 0x0007ACE4
	public bool DoesBlockHaveEndAction()
	{
		PowerTaskList last = this.GetLast();
		return last.m_blockEnd != null;
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x0007CB04 File Offset: 0x0007AD04
	public bool IsBlockUnended()
	{
		return this.IsBlock() && !this.DoesBlockHaveEndAction();
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x0007CB24 File Offset: 0x0007AD24
	public bool IsEarlierInBlockThan(PowerTaskList taskList)
	{
		if (taskList == null)
		{
			return false;
		}
		for (PowerTaskList previous = taskList.m_previous; previous != null; previous = previous.m_previous)
		{
			if (this == previous)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x0007CB5C File Offset: 0x0007AD5C
	public bool IsLaterInBlockThan(PowerTaskList taskList)
	{
		if (taskList == null)
		{
			return false;
		}
		for (PowerTaskList next = taskList.m_next; next != null; next = next.m_next)
		{
			if (this == next)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x0007CB94 File Offset: 0x0007AD94
	public bool IsInBlock(PowerTaskList taskList)
	{
		return this == taskList || this.IsEarlierInBlockThan(taskList) || this.IsLaterInBlockThan(taskList);
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x0007CBBC File Offset: 0x0007ADBC
	public bool IsDescendantOfBlock(PowerTaskList taskList)
	{
		if (taskList == null)
		{
			return false;
		}
		if (this.IsInBlock(taskList))
		{
			return true;
		}
		PowerTaskList origin = taskList.GetOrigin();
		for (PowerTaskList parent = this.GetParent(); parent != null; parent = parent.m_parent)
		{
			if (parent == origin)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x0007CC09 File Offset: 0x0007AE09
	public List<PowerTask> GetTaskList()
	{
		return this.m_tasks;
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x0007CC11 File Offset: 0x0007AE11
	public bool HasTasks()
	{
		return this.m_tasks.Count > 0;
	}

	// Token: 0x06001AB1 RID: 6833 RVA: 0x0007CC24 File Offset: 0x0007AE24
	public PowerTask CreateTask(Network.PowerHistory netPower)
	{
		PowerTask powerTask = new PowerTask();
		powerTask.SetPower(netPower);
		this.m_tasks.Add(powerTask);
		return powerTask;
	}

	// Token: 0x06001AB2 RID: 6834 RVA: 0x0007CC4C File Offset: 0x0007AE4C
	public bool HasTasksOfType(Network.PowerType powType)
	{
		foreach (PowerTask powerTask in this.m_tasks)
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == powType)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001AB3 RID: 6835 RVA: 0x0007CCBC File Offset: 0x0007AEBC
	public Entity GetSourceEntity()
	{
		Network.HistBlockStart blockStart = this.GetBlockStart();
		if (blockStart == null)
		{
			return null;
		}
		int entity = blockStart.Entity;
		Entity entity2 = GameState.Get().GetEntity(entity);
		if (entity2 == null)
		{
			string text = string.Format("PowerProcessor.GetSourceEntity() - task list {0} has a source entity with id {1} but there is no entity with that id", this.m_id, entity);
			Debug.LogWarning(text);
			return null;
		}
		return entity2;
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x0007CD18 File Offset: 0x0007AF18
	public string GetEffectCardId()
	{
		Network.HistBlockStart blockStart = this.GetBlockStart();
		if (blockStart == null)
		{
			return null;
		}
		string effectCardId = blockStart.EffectCardId;
		if (!string.IsNullOrEmpty(effectCardId))
		{
			return effectCardId;
		}
		Entity sourceEntity = this.GetSourceEntity();
		if (sourceEntity == null)
		{
			return null;
		}
		return sourceEntity.GetCardId();
	}

	// Token: 0x06001AB5 RID: 6837 RVA: 0x0007CD60 File Offset: 0x0007AF60
	public EntityDef GetEffectEntityDef()
	{
		string effectCardId = this.GetEffectCardId();
		if (string.IsNullOrEmpty(effectCardId))
		{
			return null;
		}
		return DefLoader.Get().GetEntityDef(effectCardId);
	}

	// Token: 0x06001AB6 RID: 6838 RVA: 0x0007CD90 File Offset: 0x0007AF90
	public CardDef GetEffectCardDef()
	{
		string effectCardId = this.GetEffectCardId();
		if (string.IsNullOrEmpty(effectCardId))
		{
			return null;
		}
		return DefLoader.Get().GetCardDef(effectCardId, null);
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x0007CDC0 File Offset: 0x0007AFC0
	public Entity GetTargetEntity()
	{
		Network.HistBlockStart blockStart = this.GetBlockStart();
		if (blockStart == null)
		{
			return null;
		}
		int target = blockStart.Target;
		Entity entity = GameState.Get().GetEntity(target);
		if (entity == null)
		{
			string text = string.Format("PowerProcessor.GetTargetEntity() - task list {0} has a target entity with id {1} but there is no entity with that id", this.m_id, target);
			Debug.LogWarning(text);
			return null;
		}
		return entity;
	}

	// Token: 0x06001AB8 RID: 6840 RVA: 0x0007CE1C File Offset: 0x0007B01C
	public bool HasTargetEntity()
	{
		Network.HistBlockStart blockStart = this.GetBlockStart();
		if (blockStart == null)
		{
			return false;
		}
		int target = blockStart.Target;
		Entity entity = GameState.Get().GetEntity(target);
		return entity != null;
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x0007CE54 File Offset: 0x0007B054
	public bool HasMetaDataTasks()
	{
		foreach (PowerTask powerTask in this.m_tasks)
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.META_DATA)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001ABA RID: 6842 RVA: 0x0007CEC4 File Offset: 0x0007B0C4
	public bool DoesBlockHaveMetaDataTasks()
	{
		for (PowerTaskList powerTaskList = this.GetOrigin(); powerTaskList != null; powerTaskList = powerTaskList.m_next)
		{
			if (powerTaskList.HasMetaDataTasks())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001ABB RID: 6843 RVA: 0x0007CEF8 File Offset: 0x0007B0F8
	public bool HasEffectTimingMetaData()
	{
		foreach (PowerTask powerTask in this.m_tasks)
		{
			Network.HistMetaData histMetaData = powerTask.GetPower() as Network.HistMetaData;
			if (histMetaData != null)
			{
				if (histMetaData.MetaType == null)
				{
					return true;
				}
				if (histMetaData.MetaType == 4 && histMetaData.Data == 2)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x0007CF98 File Offset: 0x0007B198
	public bool DoesBlockHaveEffectTimingMetaData()
	{
		for (PowerTaskList powerTaskList = this.GetOrigin(); powerTaskList != null; powerTaskList = powerTaskList.m_next)
		{
			if (powerTaskList.HasEffectTimingMetaData())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x0007CFCC File Offset: 0x0007B1CC
	public HistoryBlock.Type GetBlockType()
	{
		Network.HistBlockStart blockStart = this.GetBlockStart();
		if (blockStart == null)
		{
			return 0;
		}
		return blockStart.BlockType;
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x0007CFF0 File Offset: 0x0007B1F0
	public bool IsBlockType(HistoryBlock.Type type)
	{
		Network.HistBlockStart blockStart = this.GetBlockStart();
		return blockStart != null && blockStart.BlockType == type;
	}

	// Token: 0x06001ABF RID: 6847 RVA: 0x0007D015 File Offset: 0x0007B215
	public bool IsPlayBlock()
	{
		return this.IsBlockType(7);
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x0007D01E File Offset: 0x0007B21E
	public bool IsPowerBlock()
	{
		return this.IsBlockType(3);
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x0007D027 File Offset: 0x0007B227
	public bool IsTriggerBlock()
	{
		return this.IsBlockType(5);
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x0007D030 File Offset: 0x0007B230
	public bool IsDeathBlock()
	{
		return this.IsBlockType(6);
	}

	// Token: 0x06001AC3 RID: 6851 RVA: 0x0007D039 File Offset: 0x0007B239
	public bool IsRitualBlock()
	{
		return this.IsBlockType(9);
	}

	// Token: 0x06001AC4 RID: 6852 RVA: 0x0007D043 File Offset: 0x0007B243
	public void DoTasks(int startIndex, int count)
	{
		this.DoTasks(startIndex, count, null, null);
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x0007D04F File Offset: 0x0007B24F
	public void DoTasks(int startIndex, int count, PowerTaskList.CompleteCallback callback)
	{
		this.DoTasks(startIndex, count, callback, null);
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x0007D05C File Offset: 0x0007B25C
	public void DoTasks(int startIndex, int count, PowerTaskList.CompleteCallback callback, object userData)
	{
		bool flag = false;
		int num = -1;
		int num2 = Mathf.Min(startIndex + count - 1, this.m_tasks.Count - 1);
		for (int i = startIndex; i <= num2; i++)
		{
			PowerTask powerTask = this.m_tasks[i];
			if (!powerTask.IsCompleted())
			{
				if (num < 0)
				{
					num = i;
				}
				Network.PowerHistory power = powerTask.GetPower();
				if (ZoneMgr.IsHandledPower(power))
				{
					flag = true;
					break;
				}
			}
		}
		if (num < 0)
		{
			num = startIndex;
		}
		if (flag)
		{
			PowerTaskList.ZoneChangeCallbackData zoneChangeCallbackData = new PowerTaskList.ZoneChangeCallbackData();
			zoneChangeCallbackData.m_startIndex = startIndex;
			zoneChangeCallbackData.m_count = count;
			zoneChangeCallbackData.m_taskListCallback = callback;
			zoneChangeCallbackData.m_taskListUserData = userData;
			this.m_zoneChangeList = ZoneMgr.Get().AddServerZoneChanges(this, num, num2, new ZoneMgr.ChangeCompleteCallback(this.OnZoneChangeComplete), zoneChangeCallbackData);
			if (this.m_zoneChangeList != null)
			{
				return;
			}
		}
		if (Gameplay.Get() != null)
		{
			Gameplay.Get().StartCoroutine(this.WaitForGameStateAndDoTasks(num, num2, startIndex, count, callback, userData));
		}
		else
		{
			this.DoTasks(num, num2, startIndex, count, callback, userData);
		}
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x0007D179 File Offset: 0x0007B379
	public void DoAllTasks(PowerTaskList.CompleteCallback callback, object userData)
	{
		this.DoTasks(0, this.m_tasks.Count, callback, userData);
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x0007D18F File Offset: 0x0007B38F
	public void DoAllTasks(PowerTaskList.CompleteCallback callback)
	{
		this.DoTasks(0, this.m_tasks.Count, callback, null);
	}

	// Token: 0x06001AC9 RID: 6857 RVA: 0x0007D1A5 File Offset: 0x0007B3A5
	public void DoAllTasks()
	{
		this.DoTasks(0, this.m_tasks.Count, null, null);
	}

	// Token: 0x06001ACA RID: 6858 RVA: 0x0007D1BC File Offset: 0x0007B3BC
	public void DoEarlyConcedeTasks()
	{
		for (int i = 0; i < this.m_tasks.Count; i++)
		{
			PowerTask powerTask = this.m_tasks[i];
			powerTask.DoEarlyConcedeTask();
		}
	}

	// Token: 0x06001ACB RID: 6859 RVA: 0x0007D1F8 File Offset: 0x0007B3F8
	public bool IsComplete()
	{
		return this.AreTasksComplete() && this.AreZoneChangesComplete();
	}

	// Token: 0x06001ACC RID: 6860 RVA: 0x0007D218 File Offset: 0x0007B418
	public bool AreTasksComplete()
	{
		foreach (PowerTask powerTask in this.m_tasks)
		{
			if (!powerTask.IsCompleted())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001ACD RID: 6861 RVA: 0x0007D280 File Offset: 0x0007B480
	public bool IsTaskPartOfMetaData(int taskIndex, HistoryMeta.Type metaType)
	{
		for (int i = taskIndex; i >= 0; i--)
		{
			PowerTask powerTask = this.m_tasks[i];
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.META_DATA)
			{
				Network.HistMetaData histMetaData = (Network.HistMetaData)power;
				if (histMetaData.MetaType == metaType)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001ACE RID: 6862 RVA: 0x0007D2E0 File Offset: 0x0007B4E0
	public int FindEarlierIncompleteTaskIndex(int taskIndex)
	{
		for (int i = taskIndex - 1; i >= 0; i--)
		{
			PowerTask powerTask = this.m_tasks[i];
			if (!powerTask.IsCompleted())
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06001ACF RID: 6863 RVA: 0x0007D31C File Offset: 0x0007B51C
	public bool HasEarlierIncompleteTask(int taskIndex)
	{
		int num = this.FindEarlierIncompleteTaskIndex(taskIndex);
		return num >= 0;
	}

	// Token: 0x06001AD0 RID: 6864 RVA: 0x0007D338 File Offset: 0x0007B538
	public bool HasZoneChanges()
	{
		return this.m_zoneChangeList != null;
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x0007D346 File Offset: 0x0007B546
	public bool AreZoneChangesComplete()
	{
		return this.m_zoneChangeList == null || this.m_zoneChangeList.IsComplete();
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x0007D360 File Offset: 0x0007B560
	public AttackInfo GetAttackInfo()
	{
		this.BuildAttackData();
		return this.m_attackInfo;
	}

	// Token: 0x06001AD3 RID: 6867 RVA: 0x0007D36E File Offset: 0x0007B56E
	public AttackType GetAttackType()
	{
		this.BuildAttackData();
		return this.m_attackType;
	}

	// Token: 0x06001AD4 RID: 6868 RVA: 0x0007D37C File Offset: 0x0007B57C
	public Entity GetAttacker()
	{
		this.BuildAttackData();
		return this.m_attacker;
	}

	// Token: 0x06001AD5 RID: 6869 RVA: 0x0007D38A File Offset: 0x0007B58A
	public Entity GetDefender()
	{
		this.BuildAttackData();
		return this.m_defender;
	}

	// Token: 0x06001AD6 RID: 6870 RVA: 0x0007D398 File Offset: 0x0007B598
	public Entity GetProposedDefender()
	{
		this.BuildAttackData();
		return this.m_proposedDefender;
	}

	// Token: 0x06001AD7 RID: 6871 RVA: 0x0007D3A8 File Offset: 0x0007B5A8
	public bool HasGameOver()
	{
		for (int i = 0; i < this.m_tasks.Count; i++)
		{
			PowerTask powerTask = this.m_tasks[i];
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = (Network.HistTagChange)power;
				if (GameUtils.IsGameOverTag(histTagChange.Entity, histTagChange.Tag, histTagChange.Value))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001AD8 RID: 6872 RVA: 0x0007D41C File Offset: 0x0007B61C
	public bool HasFriendlyConcede()
	{
		for (int i = 0; i < this.m_tasks.Count; i++)
		{
			PowerTask powerTask = this.m_tasks[i];
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange tagChange = (Network.HistTagChange)power;
				if (GameUtils.IsFriendlyConcede(tagChange))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001AD9 RID: 6873 RVA: 0x0007D480 File Offset: 0x0007B680
	public PowerTaskList.DamageInfo GetDamageInfo(Entity entity)
	{
		if (entity == null)
		{
			return null;
		}
		int entityId = entity.GetEntityId();
		foreach (PowerTask powerTask in this.m_tasks)
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = power as Network.HistTagChange;
				if (histTagChange.Tag == 44)
				{
					if (histTagChange.Entity == entityId)
					{
						PowerTaskList.DamageInfo damageInfo = new PowerTaskList.DamageInfo();
						damageInfo.m_entity = GameState.Get().GetEntity(histTagChange.Entity);
						damageInfo.m_damage = histTagChange.Value - damageInfo.m_entity.GetDamage();
						return damageInfo;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06001ADA RID: 6874 RVA: 0x0007D570 File Offset: 0x0007B770
	public void SetWillCompleteHistoryEntry(bool set)
	{
		this.m_willCompleteHistoryEntry = set;
	}

	// Token: 0x06001ADB RID: 6875 RVA: 0x0007D579 File Offset: 0x0007B779
	public bool WillCompleteHistoryEntry()
	{
		return this.m_willCompleteHistoryEntry;
	}

	// Token: 0x06001ADC RID: 6876 RVA: 0x0007D584 File Offset: 0x0007B784
	public bool WillBlockCompleteHistoryEntry()
	{
		for (PowerTaskList powerTaskList = this.GetOrigin(); powerTaskList != null; powerTaskList = powerTaskList.m_next)
		{
			if (powerTaskList.WillCompleteHistoryEntry())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001ADD RID: 6877 RVA: 0x0007D5B8 File Offset: 0x0007B7B8
	public Entity GetRitualEntityClone()
	{
		return this.m_ritualEntityClone;
	}

	// Token: 0x06001ADE RID: 6878 RVA: 0x0007D5C0 File Offset: 0x0007B7C0
	public void SetRitualEntityClone(Entity ent)
	{
		this.m_ritualEntityClone = ent;
	}

	// Token: 0x06001ADF RID: 6879 RVA: 0x0007D5CC File Offset: 0x0007B7CC
	public bool WasThePlayedSpellCountered(Entity entity)
	{
		foreach (PowerTask powerTask in this.m_tasks)
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = power as Network.HistTagChange;
				if (histTagChange.Entity == entity.GetEntityId() && histTagChange.Tag == 231 && histTagChange.Value == 1)
				{
					return true;
				}
			}
		}
		PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
		PowerQueue powerQueue = powerProcessor.GetPowerQueue();
		List<PowerTaskList> list = powerQueue.GetList();
		foreach (PowerTaskList powerTaskList in list)
		{
			foreach (PowerTask powerTask2 in powerTaskList.GetTaskList())
			{
				Network.PowerHistory power2 = powerTask2.GetPower();
				if (power2.Type == Network.PowerType.TAG_CHANGE)
				{
					Network.HistTagChange histTagChange2 = power2 as Network.HistTagChange;
					if (histTagChange2.Entity == entity.GetEntityId() && histTagChange2.Tag == 231 && histTagChange2.Value == 1)
					{
						return true;
					}
				}
			}
			if (powerTaskList.GetBlockEnd() != null && powerTaskList.GetBlockStart().BlockType == 7)
			{
				return false;
			}
		}
		return false;
	}

	// Token: 0x06001AE0 RID: 6880 RVA: 0x0007D7A0 File Offset: 0x0007B9A0
	public void NotifyHistoryOfAdditionalTargets()
	{
		if (this.NotifyHistoryOfCastRandomSpellsTargets())
		{
			return;
		}
		Network.HistBlockStart blockStart = this.GetBlockStart();
		int num = (blockStart != null) ? blockStart.Entity : 0;
		List<int> list = new List<int>();
		bool flag = true;
		foreach (PowerTask powerTask in this.GetTaskList())
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.META_DATA)
			{
				Network.HistMetaData histMetaData = (Network.HistMetaData)power;
				if (histMetaData.MetaType == null)
				{
					for (int i = 0; i < histMetaData.Info.Count; i++)
					{
						HistoryManager.Get().NotifyEntityAffected(histMetaData.Info[i], false);
					}
				}
				else if (histMetaData.MetaType == 1 || histMetaData.MetaType == 2)
				{
					flag = false;
				}
			}
			else if (power.Type == Network.PowerType.SHOW_ENTITY)
			{
				Network.HistShowEntity histShowEntity = (Network.HistShowEntity)power;
				bool flag2 = false;
				bool flag3 = false;
				foreach (Network.Entity.Tag tag in histShowEntity.Entity.Tags)
				{
					if (tag.Name == 202 && tag.Value == 6)
					{
						flag2 = true;
						break;
					}
					if (tag.Name == 49 && tag.Value == 4)
					{
						flag3 = true;
					}
				}
				if (!flag2)
				{
					if (flag3)
					{
						HistoryManager.Get().NotifyEntityDied(histShowEntity.Entity.ID);
					}
					else
					{
						HistoryManager.Get().NotifyEntityAffected(histShowEntity.Entity.ID, false);
					}
				}
			}
			else if (power.Type == Network.PowerType.FULL_ENTITY)
			{
				Network.HistFullEntity histFullEntity = (Network.HistFullEntity)power;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				foreach (Network.Entity.Tag tag2 in histFullEntity.Entity.Tags)
				{
					if (tag2.Name == 202 && tag2.Value == 6)
					{
						flag4 = true;
						break;
					}
					if (tag2.Name == 49 && (tag2.Value == 1 || tag2.Value == 7))
					{
						flag5 = true;
					}
					else if (tag2.Name == 385 && tag2.Value == num)
					{
						flag6 = true;
					}
				}
				if (!flag4)
				{
					if (flag5 || flag6)
					{
						HistoryManager.Get().NotifyEntityAffected(histFullEntity.Entity.ID, false);
					}
				}
			}
			else if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = (Network.HistTagChange)power;
				Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
				if (histTagChange.Tag == 44)
				{
					if (!list.Contains(histTagChange.Entity) && !flag)
					{
						HistoryManager.Get().NotifyDamageChanged(entity, histTagChange.Value);
						flag = true;
					}
				}
				else if (histTagChange.Tag == 292)
				{
					if (!list.Contains(histTagChange.Entity))
					{
						HistoryManager.Get().NotifyArmorChanged(entity, histTagChange.Value);
					}
				}
				else if (histTagChange.Tag == 318)
				{
					HistoryManager.Get().NotifyEntityAffected(entity, false);
				}
				else if (histTagChange.Tag == 385 && histTagChange.Value == num)
				{
					HistoryManager.Get().NotifyEntityAffected(entity, false);
				}
				else if (histTagChange.Tag == 262)
				{
					HistoryManager.Get().NotifyEntityAffected(entity, false);
				}
				if (GameUtils.IsHistoryDeathTagChange(histTagChange))
				{
					HistoryManager.Get().NotifyEntityDied(entity);
					list.Add(histTagChange.Entity);
				}
			}
		}
	}

	// Token: 0x06001AE1 RID: 6881 RVA: 0x0007DC20 File Offset: 0x0007BE20
	private bool NotifyHistoryOfCastRandomSpellsTargets()
	{
		Network.HistBlockStart blockStart = this.GetBlockStart();
		if (blockStart == null)
		{
			return false;
		}
		Entity entity = GameState.Get().GetEntity(blockStart.Entity);
		PowerTaskList powerTaskList = null;
		PowerTaskList powerTaskList2 = null;
		for (PowerTaskList powerTaskList3 = this.GetOrigin(); powerTaskList3 != null; powerTaskList3 = powerTaskList3.GetParent())
		{
			if (powerTaskList3.IsPlayBlock())
			{
				powerTaskList = powerTaskList3;
				if (powerTaskList2 == null)
				{
					powerTaskList2 = powerTaskList3;
				}
			}
		}
		if (powerTaskList == null)
		{
			return false;
		}
		if (powerTaskList == powerTaskList2)
		{
			return false;
		}
		Entity sourceEntity = powerTaskList.GetSourceEntity();
		if (sourceEntity == null)
		{
			return false;
		}
		if (!sourceEntity.HasTag(GAME_TAG.CAST_RANDOM_SPELLS))
		{
			return false;
		}
		if (entity != null && blockStart.BlockType == 7)
		{
			HistoryManager.Get().NotifyEntityAffected(entity, false);
		}
		return true;
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x0007DCE0 File Offset: 0x0007BEE0
	public bool ShouldCreatePlayBlockHistoryTile()
	{
		if (!this.IsPlayBlock())
		{
			return false;
		}
		PowerTaskList parent = this.GetParent();
		if (parent == null)
		{
			return true;
		}
		Entity sourceEntity = parent.GetSourceEntity();
		return !sourceEntity.HasTag(GAME_TAG.CAST_RANDOM_SPELLS);
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x0007DD23 File Offset: 0x0007BF23
	public void DebugDump()
	{
		this.DebugDump(Log.Power);
	}

	// Token: 0x06001AE4 RID: 6884 RVA: 0x0007DD30 File Offset: 0x0007BF30
	public void DebugDump(Logger logger)
	{
		if (!logger.CanPrint())
		{
			return;
		}
		GameState gameState = GameState.Get();
		string text = string.Empty;
		int num = (this.m_parent != null) ? this.m_parent.GetId() : 0;
		int num2 = (this.m_previous != null) ? this.m_previous.GetId() : 0;
		logger.Print("PowerTaskList.DebugDump() - ID={0} ParentID={1} PreviousID={2} TaskCount={3}", new object[]
		{
			this.m_id,
			num,
			num2,
			this.m_tasks.Count
		});
		if (this.m_blockStart == null)
		{
			logger.Print("PowerTaskList.DebugDump() - {0}Block Start=(null)", new object[]
			{
				text
			});
			text += "    ";
		}
		else
		{
			gameState.DebugPrintPower(logger, "PowerTaskList", this.m_blockStart, ref text);
		}
		for (int i = 0; i < this.m_tasks.Count; i++)
		{
			PowerTask powerTask = this.m_tasks[i];
			Network.PowerHistory power = powerTask.GetPower();
			gameState.DebugPrintPower(logger, "PowerTaskList", power, ref text);
		}
		if (this.m_blockEnd == null)
		{
			if (text.Length >= "    ".Length)
			{
				text = text.Remove(text.Length - "    ".Length);
			}
			logger.Print("PowerTaskList.DebugDump() - {0}Block End=(null)", new object[]
			{
				text
			});
		}
		else
		{
			gameState.DebugPrintPower(logger, "PowerTaskList", this.m_blockEnd, ref text);
		}
	}

	// Token: 0x06001AE5 RID: 6885 RVA: 0x0007DEC8 File Offset: 0x0007C0C8
	public override string ToString()
	{
		return string.Format("id={0} tasks={1} prevId={2} nextId={3} parentId={4}", new object[]
		{
			this.m_id,
			this.m_tasks.Count,
			(this.m_previous != null) ? this.m_previous.GetId() : 0,
			(this.m_next != null) ? this.m_next.GetId() : 0,
			(this.m_parent != null) ? this.m_parent.GetId() : 0
		});
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x0007DF74 File Offset: 0x0007C174
	private void OnZoneChangeComplete(ZoneChangeList changeList, object userData)
	{
		PowerTaskList.ZoneChangeCallbackData zoneChangeCallbackData = (PowerTaskList.ZoneChangeCallbackData)userData;
		if (zoneChangeCallbackData.m_taskListCallback != null)
		{
			zoneChangeCallbackData.m_taskListCallback(this, zoneChangeCallbackData.m_startIndex, zoneChangeCallbackData.m_count, zoneChangeCallbackData.m_taskListUserData);
		}
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x0007DFB4 File Offset: 0x0007C1B4
	private IEnumerator WaitForGameStateAndDoTasks(int incompleteStartIndex, int endIndex, int startIndex, int count, PowerTaskList.CompleteCallback callback, object userData)
	{
		for (int i = incompleteStartIndex; i <= endIndex; i++)
		{
			PowerTask task = this.m_tasks[i];
			task.DoTask();
			while (GameState.Get().IsBusy())
			{
				yield return null;
			}
			while (GameState.Get().IsMulliganBusy())
			{
				yield return null;
			}
		}
		if (callback != null)
		{
			callback(this, startIndex, count, userData);
		}
		yield break;
	}

	// Token: 0x06001AE8 RID: 6888 RVA: 0x0007E02C File Offset: 0x0007C22C
	private void DoTasks(int incompleteStartIndex, int endIndex, int startIndex, int count, PowerTaskList.CompleteCallback callback, object userData)
	{
		for (int i = incompleteStartIndex; i <= endIndex; i++)
		{
			PowerTask powerTask = this.m_tasks[i];
			powerTask.DoTask();
		}
		if (callback != null)
		{
			callback(this, startIndex, count, userData);
		}
	}

	// Token: 0x06001AE9 RID: 6889 RVA: 0x0007E074 File Offset: 0x0007C274
	private void BuildAttackData()
	{
		if (this.m_attackDataBuilt)
		{
			return;
		}
		this.m_attackInfo = this.BuildAttackInfo();
		AttackInfo attackInfo;
		this.m_attackType = this.DetermineAttackType(out attackInfo);
		this.m_attacker = null;
		this.m_defender = null;
		this.m_proposedDefender = null;
		switch (this.m_attackType)
		{
		case AttackType.REGULAR:
			this.m_attacker = attackInfo.m_attacker;
			this.m_defender = attackInfo.m_defender;
			break;
		case AttackType.PROPOSED:
			this.m_attacker = attackInfo.m_proposedAttacker;
			this.m_defender = attackInfo.m_proposedDefender;
			this.m_proposedDefender = attackInfo.m_proposedDefender;
			break;
		case AttackType.CANCELED:
			this.m_attacker = this.m_previous.GetAttacker();
			this.m_proposedDefender = this.m_previous.GetProposedDefender();
			break;
		case AttackType.ONLY_ATTACKER:
			this.m_attacker = attackInfo.m_attacker;
			break;
		case AttackType.ONLY_DEFENDER:
			this.m_defender = attackInfo.m_defender;
			break;
		case AttackType.ONLY_PROPOSED_ATTACKER:
			this.m_attacker = attackInfo.m_proposedAttacker;
			break;
		case AttackType.ONLY_PROPOSED_DEFENDER:
			this.m_proposedDefender = attackInfo.m_proposedDefender;
			this.m_defender = attackInfo.m_proposedDefender;
			break;
		case AttackType.WAITING_ON_PROPOSED_ATTACKER:
		case AttackType.WAITING_ON_PROPOSED_DEFENDER:
		case AttackType.WAITING_ON_ATTACKER:
		case AttackType.WAITING_ON_DEFENDER:
			this.m_attacker = this.m_previous.GetAttacker();
			this.m_defender = this.m_previous.GetDefender();
			break;
		}
		this.m_attackDataBuilt = true;
	}

	// Token: 0x06001AEA RID: 6890 RVA: 0x0007E1EC File Offset: 0x0007C3EC
	private AttackInfo BuildAttackInfo()
	{
		GameState gameState = GameState.Get();
		AttackInfo attackInfo = new AttackInfo();
		bool flag = false;
		foreach (PowerTask powerTask in this.GetTaskList())
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = power as Network.HistTagChange;
				if (histTagChange.Tag == 36)
				{
					attackInfo.m_defenderTagValue = new int?(histTagChange.Value);
					if (histTagChange.Value == 1)
					{
						attackInfo.m_defender = gameState.GetEntity(histTagChange.Entity);
					}
					flag = true;
				}
				else if (histTagChange.Tag == 38)
				{
					attackInfo.m_attackerTagValue = new int?(histTagChange.Value);
					if (histTagChange.Value == 1)
					{
						attackInfo.m_attacker = gameState.GetEntity(histTagChange.Entity);
					}
					flag = true;
				}
				else if (histTagChange.Tag == 39)
				{
					attackInfo.m_proposedAttackerTagValue = new int?(histTagChange.Value);
					if (histTagChange.Value != 0)
					{
						attackInfo.m_proposedAttacker = gameState.GetEntity(histTagChange.Value);
					}
					flag = true;
				}
				else if (histTagChange.Tag == 37)
				{
					attackInfo.m_proposedDefenderTagValue = new int?(histTagChange.Value);
					if (histTagChange.Value != 0)
					{
						attackInfo.m_proposedDefender = gameState.GetEntity(histTagChange.Value);
					}
					flag = true;
				}
			}
		}
		if (flag)
		{
			return attackInfo;
		}
		return null;
	}

	// Token: 0x06001AEB RID: 6891 RVA: 0x0007E39C File Offset: 0x0007C59C
	private AttackType DetermineAttackType(out AttackInfo info)
	{
		info = this.m_attackInfo;
		GameState gameState = GameState.Get();
		GameEntity gameEntity = gameState.GetGameEntity();
		Entity entity = gameState.GetEntity(gameEntity.GetTag(GAME_TAG.PROPOSED_ATTACKER));
		Entity entity2 = gameState.GetEntity(gameEntity.GetTag(GAME_TAG.PROPOSED_DEFENDER));
		AttackType attackType = AttackType.INVALID;
		Entity entity3 = null;
		Entity entity4 = null;
		if (this.m_previous != null)
		{
			attackType = this.m_previous.GetAttackType();
			entity3 = this.m_previous.GetAttacker();
			entity4 = this.m_previous.GetDefender();
		}
		if (this.m_attackInfo != null)
		{
			if (this.m_attackInfo.m_attacker != null || this.m_attackInfo.m_defender != null)
			{
				if (this.m_attackInfo.m_attacker == null)
				{
					if (attackType == AttackType.ONLY_ATTACKER || attackType == AttackType.WAITING_ON_DEFENDER)
					{
						info = new AttackInfo();
						info.m_attacker = entity3;
						info.m_defender = this.m_attackInfo.m_defender;
						return AttackType.REGULAR;
					}
					return AttackType.ONLY_DEFENDER;
				}
				else
				{
					if (this.m_attackInfo.m_defender != null)
					{
						return AttackType.REGULAR;
					}
					if (attackType == AttackType.ONLY_DEFENDER || attackType == AttackType.WAITING_ON_ATTACKER)
					{
						info = new AttackInfo();
						info.m_attacker = this.m_attackInfo.m_attacker;
						info.m_defender = entity4;
						return AttackType.REGULAR;
					}
					return AttackType.ONLY_ATTACKER;
				}
			}
			else if (this.m_attackInfo.m_proposedAttacker != null || this.m_attackInfo.m_proposedDefender != null)
			{
				if (this.m_attackInfo.m_proposedAttacker == null)
				{
					if (entity != null)
					{
						info = new AttackInfo();
						info.m_proposedAttacker = entity;
						info.m_proposedDefender = this.m_attackInfo.m_proposedDefender;
						return AttackType.PROPOSED;
					}
					return AttackType.ONLY_PROPOSED_DEFENDER;
				}
				else
				{
					if (this.m_attackInfo.m_proposedDefender != null)
					{
						return AttackType.PROPOSED;
					}
					if (entity2 != null)
					{
						info = new AttackInfo();
						info.m_proposedAttacker = this.m_attackInfo.m_proposedAttacker;
						info.m_proposedDefender = entity2;
						return AttackType.PROPOSED;
					}
					return AttackType.ONLY_PROPOSED_ATTACKER;
				}
			}
			else if (attackType == AttackType.REGULAR || attackType == AttackType.INVALID)
			{
				return AttackType.INVALID;
			}
		}
		if (attackType == AttackType.PROPOSED)
		{
			if ((entity != null && entity.GetZone() != TAG_ZONE.PLAY) || (entity2 != null && entity2.GetZone() != TAG_ZONE.PLAY))
			{
				return AttackType.CANCELED;
			}
			if (entity3 != entity || entity4 != entity2)
			{
				info = new AttackInfo();
				info.m_proposedAttacker = entity;
				info.m_proposedDefender = entity2;
				return AttackType.PROPOSED;
			}
			if (entity != null && entity2 != null && !this.IsEndOfBlock())
			{
				info = new AttackInfo();
				info.m_proposedAttacker = entity;
				info.m_proposedDefender = entity2;
				return AttackType.PROPOSED;
			}
			return AttackType.CANCELED;
		}
		else
		{
			if (attackType == AttackType.CANCELED)
			{
				return AttackType.INVALID;
			}
			if (this.IsEndOfBlock())
			{
				if (attackType == AttackType.ONLY_ATTACKER || attackType == AttackType.WAITING_ON_DEFENDER)
				{
					return AttackType.CANCELED;
				}
				Debug.LogWarningFormat("AttackSpellController.DetermineAttackType() - INVALID ATTACK prevAttackType={0} prevAttacker={1} prevDefender={2}", new object[]
				{
					attackType,
					entity3,
					entity4
				});
				return AttackType.INVALID;
			}
			else
			{
				if (attackType == AttackType.ONLY_PROPOSED_ATTACKER || attackType == AttackType.WAITING_ON_PROPOSED_DEFENDER)
				{
					return AttackType.WAITING_ON_PROPOSED_DEFENDER;
				}
				if (attackType == AttackType.ONLY_PROPOSED_DEFENDER || attackType == AttackType.WAITING_ON_PROPOSED_ATTACKER)
				{
					return AttackType.WAITING_ON_PROPOSED_ATTACKER;
				}
				if (attackType == AttackType.ONLY_ATTACKER || attackType == AttackType.WAITING_ON_DEFENDER)
				{
					return AttackType.WAITING_ON_DEFENDER;
				}
				if (attackType == AttackType.ONLY_DEFENDER || attackType == AttackType.WAITING_ON_ATTACKER)
				{
					return AttackType.WAITING_ON_ATTACKER;
				}
				return AttackType.INVALID;
			}
		}
	}

	// Token: 0x04000D48 RID: 3400
	private int m_id;

	// Token: 0x04000D49 RID: 3401
	private Network.HistBlockStart m_blockStart;

	// Token: 0x04000D4A RID: 3402
	private Network.HistBlockEnd m_blockEnd;

	// Token: 0x04000D4B RID: 3403
	private List<PowerTask> m_tasks = new List<PowerTask>();

	// Token: 0x04000D4C RID: 3404
	private ZoneChangeList m_zoneChangeList;

	// Token: 0x04000D4D RID: 3405
	private PowerTaskList m_previous;

	// Token: 0x04000D4E RID: 3406
	private PowerTaskList m_next;

	// Token: 0x04000D4F RID: 3407
	private PowerTaskList m_parent;

	// Token: 0x04000D50 RID: 3408
	private bool m_attackDataBuilt;

	// Token: 0x04000D51 RID: 3409
	private AttackInfo m_attackInfo;

	// Token: 0x04000D52 RID: 3410
	private AttackType m_attackType;

	// Token: 0x04000D53 RID: 3411
	private Entity m_attacker;

	// Token: 0x04000D54 RID: 3412
	private Entity m_defender;

	// Token: 0x04000D55 RID: 3413
	private Entity m_proposedDefender;

	// Token: 0x04000D56 RID: 3414
	private bool m_willCompleteHistoryEntry;

	// Token: 0x04000D57 RID: 3415
	private Entity m_ritualEntityClone;

	// Token: 0x020003E6 RID: 998
	// (Invoke) Token: 0x0600339A RID: 13210
	public delegate void CompleteCallback(PowerTaskList taskList, int startIndex, int count, object userData);

	// Token: 0x020008BE RID: 2238
	public class DamageInfo
	{
		// Token: 0x04003AB5 RID: 15029
		public Entity m_entity;

		// Token: 0x04003AB6 RID: 15030
		public int m_damage;
	}

	// Token: 0x020008BF RID: 2239
	private class ZoneChangeCallbackData
	{
		// Token: 0x04003AB7 RID: 15031
		public int m_startIndex;

		// Token: 0x04003AB8 RID: 15032
		public int m_count;

		// Token: 0x04003AB9 RID: 15033
		public PowerTaskList.CompleteCallback m_taskListCallback;

		// Token: 0x04003ABA RID: 15034
		public object m_taskListUserData;
	}
}
