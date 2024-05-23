using System;
using System.Collections.Generic;

// Token: 0x0200019B RID: 411
public class PowerTask
{
	// Token: 0x06001AED RID: 6893 RVA: 0x0007E6A9 File Offset: 0x0007C8A9
	public Network.PowerHistory GetPower()
	{
		return this.m_power;
	}

	// Token: 0x06001AEE RID: 6894 RVA: 0x0007E6B1 File Offset: 0x0007C8B1
	public void SetPower(Network.PowerHistory power)
	{
		this.m_power = power;
	}

	// Token: 0x06001AEF RID: 6895 RVA: 0x0007E6BA File Offset: 0x0007C8BA
	public bool IsCompleted()
	{
		return this.m_completed;
	}

	// Token: 0x06001AF0 RID: 6896 RVA: 0x0007E6C2 File Offset: 0x0007C8C2
	public void SetCompleted(bool complete)
	{
		this.m_completed = complete;
	}

	// Token: 0x06001AF1 RID: 6897 RVA: 0x0007E6CC File Offset: 0x0007C8CC
	public void DoRealTimeTask(List<Network.PowerHistory> powerList, int index)
	{
		GameState gameState = GameState.Get();
		switch (this.m_power.Type)
		{
		case Network.PowerType.FULL_ENTITY:
		{
			Network.HistFullEntity fullEntity = (Network.HistFullEntity)this.m_power;
			gameState.OnRealTimeFullEntity(fullEntity);
			break;
		}
		case Network.PowerType.SHOW_ENTITY:
		{
			Network.HistShowEntity showEntity = (Network.HistShowEntity)this.m_power;
			gameState.OnRealTimeShowEntity(showEntity);
			break;
		}
		case Network.PowerType.TAG_CHANGE:
		{
			Network.HistTagChange change = (Network.HistTagChange)this.m_power;
			gameState.OnRealTimeTagChange(change);
			break;
		}
		case Network.PowerType.CREATE_GAME:
		{
			Network.HistCreateGame createGame = (Network.HistCreateGame)this.m_power;
			gameState.OnRealTimeCreateGame(powerList, index, createGame);
			break;
		}
		case Network.PowerType.CHANGE_ENTITY:
		{
			Network.HistChangeEntity changeEntity = (Network.HistChangeEntity)this.m_power;
			gameState.OnRealTimeChangeEntity(changeEntity);
			break;
		}
		}
	}

	// Token: 0x06001AF2 RID: 6898 RVA: 0x0007E7A0 File Offset: 0x0007C9A0
	public void DoTask()
	{
		if (this.m_completed)
		{
			return;
		}
		GameState gameState = GameState.Get();
		switch (this.m_power.Type)
		{
		case Network.PowerType.FULL_ENTITY:
		{
			Network.HistFullEntity fullEntity = (Network.HistFullEntity)this.m_power;
			gameState.OnFullEntity(fullEntity);
			break;
		}
		case Network.PowerType.SHOW_ENTITY:
		{
			Network.HistShowEntity showEntity = (Network.HistShowEntity)this.m_power;
			gameState.OnShowEntity(showEntity);
			break;
		}
		case Network.PowerType.HIDE_ENTITY:
		{
			Network.HistHideEntity hideEntity = (Network.HistHideEntity)this.m_power;
			gameState.OnHideEntity(hideEntity);
			break;
		}
		case Network.PowerType.TAG_CHANGE:
		{
			Network.HistTagChange netChange = (Network.HistTagChange)this.m_power;
			gameState.OnTagChange(netChange);
			break;
		}
		case Network.PowerType.META_DATA:
		{
			Network.HistMetaData metaData = (Network.HistMetaData)this.m_power;
			gameState.OnMetaData(metaData);
			break;
		}
		case Network.PowerType.CHANGE_ENTITY:
		{
			Network.HistChangeEntity changeEntity = (Network.HistChangeEntity)this.m_power;
			gameState.OnChangeEntity(changeEntity);
			break;
		}
		}
		this.m_completed = true;
	}

	// Token: 0x06001AF3 RID: 6899 RVA: 0x0007E8A4 File Offset: 0x0007CAA4
	public void DoEarlyConcedeTask()
	{
		if (this.m_completed)
		{
			return;
		}
		GameState gameState = GameState.Get();
		switch (this.m_power.Type)
		{
		case Network.PowerType.SHOW_ENTITY:
		{
			Network.HistShowEntity showEntity = (Network.HistShowEntity)this.m_power;
			gameState.OnEarlyConcedeShowEntity(showEntity);
			break;
		}
		case Network.PowerType.HIDE_ENTITY:
		{
			Network.HistHideEntity hideEntity = (Network.HistHideEntity)this.m_power;
			gameState.OnEarlyConcedeHideEntity(hideEntity);
			break;
		}
		case Network.PowerType.TAG_CHANGE:
		{
			Network.HistTagChange netChange = (Network.HistTagChange)this.m_power;
			gameState.OnEarlyConcedeTagChange(netChange);
			break;
		}
		case Network.PowerType.CHANGE_ENTITY:
		{
			Network.HistChangeEntity changeEntity = (Network.HistChangeEntity)this.m_power;
			gameState.OnEarlyConcedeChangeEntity(changeEntity);
			break;
		}
		}
		this.m_completed = true;
	}

	// Token: 0x06001AF4 RID: 6900 RVA: 0x0007E96C File Offset: 0x0007CB6C
	public override string ToString()
	{
		string text = "null";
		if (this.m_power != null)
		{
			switch (this.m_power.Type)
			{
			case Network.PowerType.FULL_ENTITY:
			{
				Network.HistFullEntity histFullEntity = (Network.HistFullEntity)this.m_power;
				text = string.Format("type={0} entity={1} tags={2}", this.m_power.Type, this.GetPrintableEntity(histFullEntity.Entity), histFullEntity.Entity.Tags);
				break;
			}
			case Network.PowerType.SHOW_ENTITY:
			{
				Network.HistShowEntity histShowEntity = (Network.HistShowEntity)this.m_power;
				text = string.Format("type={0} entity={1} tags={2}", this.m_power.Type, this.GetPrintableEntity(histShowEntity.Entity), histShowEntity.Entity.Tags);
				break;
			}
			case Network.PowerType.HIDE_ENTITY:
			{
				Network.HistHideEntity histHideEntity = (Network.HistHideEntity)this.m_power;
				text = string.Format("type={0} entity={1} zone={2}", this.m_power.Type, this.GetPrintableEntity(histHideEntity.Entity), histHideEntity.Zone);
				break;
			}
			case Network.PowerType.TAG_CHANGE:
			{
				Network.HistTagChange histTagChange = (Network.HistTagChange)this.m_power;
				text = string.Format("type={0} entity={1} {2}", this.m_power.Type, this.GetPrintableEntity(histTagChange.Entity), Tags.DebugTag(histTagChange.Tag, histTagChange.Value));
				break;
			}
			case Network.PowerType.CREATE_GAME:
			{
				Network.HistCreateGame histCreateGame = (Network.HistCreateGame)this.m_power;
				text = histCreateGame.ToString();
				break;
			}
			case Network.PowerType.META_DATA:
			{
				Network.HistMetaData histMetaData = (Network.HistMetaData)this.m_power;
				text = histMetaData.ToString();
				break;
			}
			case Network.PowerType.CHANGE_ENTITY:
			{
				Network.HistChangeEntity histChangeEntity = (Network.HistChangeEntity)this.m_power;
				text = string.Format("type={0} entity={1} tags={2}", this.m_power.Type, this.GetPrintableEntity(histChangeEntity.Entity), histChangeEntity.Entity.Tags);
				break;
			}
			}
		}
		return string.Format("power=[{0}] complete={1}", text, this.m_completed);
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x0007EB70 File Offset: 0x0007CD70
	private string GetPrintableEntity(int entityId)
	{
		Entity entity = GameState.Get().GetEntity(entityId);
		if (entity == null)
		{
			return entityId.ToString();
		}
		string name = entity.GetName();
		if (name == null)
		{
			return string.Format("[id={0} cardId={1}]", entityId, entity.GetCardId());
		}
		return string.Format("[id={0} cardId={1} name={2}]", entityId, entity.GetCardId(), name);
	}

	// Token: 0x06001AF6 RID: 6902 RVA: 0x0007EBD4 File Offset: 0x0007CDD4
	private string GetPrintableEntity(Network.Entity netEntity)
	{
		Entity entity = GameState.Get().GetEntity(netEntity.ID);
		string text = (entity != null) ? entity.GetName() : null;
		if (text == null)
		{
			return string.Format("[id={0} cardId={2}]", netEntity.ID, netEntity.CardID);
		}
		return string.Format("[id={0} cardId={1} name={2}]", netEntity.ID, netEntity.CardID, text);
	}

	// Token: 0x04000D58 RID: 3416
	private Network.PowerHistory m_power;

	// Token: 0x04000D59 RID: 3417
	private bool m_completed;
}
