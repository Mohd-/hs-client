using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200061B RID: 1563
public class HistoryManager : MonoBehaviour
{
	// Token: 0x06004411 RID: 17425 RVA: 0x00146A08 File Offset: 0x00144C08
	private void Awake()
	{
		HistoryManager.s_instance = this;
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 0.15f, base.transform.position.z);
	}

	// Token: 0x06004412 RID: 17426 RVA: 0x00146A6A File Offset: 0x00144C6A
	private void OnDestroy()
	{
		HistoryManager.s_instance = null;
	}

	// Token: 0x06004413 RID: 17427 RVA: 0x00146A72 File Offset: 0x00144C72
	private void Start()
	{
		base.StartCoroutine(this.WaitForBoardLoadedAndSetPaths());
	}

	// Token: 0x06004414 RID: 17428 RVA: 0x00146A81 File Offset: 0x00144C81
	public static HistoryManager Get()
	{
		return HistoryManager.s_instance;
	}

	// Token: 0x06004415 RID: 17429 RVA: 0x00146A88 File Offset: 0x00144C88
	public void DisableHistory()
	{
		this.m_historyDisabled = true;
	}

	// Token: 0x06004416 RID: 17430 RVA: 0x00146A94 File Offset: 0x00144C94
	private Entity CreatePreTransformedEntity(Entity entity)
	{
		int tag = entity.GetTag(GAME_TAG.TRANSFORMED_FROM_CARD);
		if (tag == 0)
		{
			return null;
		}
		string text = GameUtils.TranslateDbIdToCardId(tag);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		Entity entity2 = new Entity();
		EntityDef entityDef = DefLoader.Get().GetEntityDef(text);
		entity2.InitCard();
		entity2.ReplaceTags(entityDef.GetTags());
		entity2.LoadCard(text);
		entity2.SetTag(GAME_TAG.CONTROLLER, entity.GetControllerId());
		entity2.SetTag<TAG_ZONE>(GAME_TAG.ZONE, TAG_ZONE.HAND);
		entity2.SetTag<TAG_PREMIUM>(GAME_TAG.PREMIUM, entity.GetPremiumType());
		return entity2;
	}

	// Token: 0x06004417 RID: 17431 RVA: 0x00146B1C File Offset: 0x00144D1C
	public void CreatePlayedTile(Entity playedEntity, Entity targetedEntity)
	{
		if (this.m_historyDisabled)
		{
			return;
		}
		HistoryManager.TileEntry tileEntry = new HistoryManager.TileEntry();
		this.m_queuedEntries.Add(tileEntry);
		tileEntry.SetCardPlayed(playedEntity);
		tileEntry.SetCardTargeted(targetedEntity);
		if (tileEntry.m_lastCardPlayed.GetDuplicatedEntity() == null)
		{
			base.StartCoroutine("WaitForCardLoadedAndDuplicateInfo", tileEntry.m_lastCardPlayed);
		}
	}

	// Token: 0x06004418 RID: 17432 RVA: 0x00146B78 File Offset: 0x00144D78
	public void CreateTriggerTile(Entity triggeredEntity)
	{
		if (this.m_historyDisabled)
		{
			return;
		}
		HistoryManager.TileEntry tileEntry = new HistoryManager.TileEntry();
		this.m_queuedEntries.Add(tileEntry);
		tileEntry.SetCardTriggered(triggeredEntity);
	}

	// Token: 0x06004419 RID: 17433 RVA: 0x00146BAC File Offset: 0x00144DAC
	public void CreateAttackTile(Entity attacker, Entity defender, PowerTaskList taskList)
	{
		if (this.m_historyDisabled)
		{
			return;
		}
		HistoryManager.TileEntry tileEntry = new HistoryManager.TileEntry();
		this.m_queuedEntries.Add(tileEntry);
		tileEntry.SetAttacker(attacker);
		tileEntry.SetDefender(defender);
		Entity duplicatedEntity = tileEntry.m_lastAttacker.GetDuplicatedEntity();
		Entity duplicatedEntity2 = tileEntry.m_lastDefender.GetDuplicatedEntity();
		int entityId = attacker.GetEntityId();
		int entityId2 = defender.GetEntityId();
		int num = -1;
		List<PowerTask> taskList2 = taskList.GetTaskList();
		for (int i = 0; i < taskList2.Count; i++)
		{
			PowerTask powerTask = taskList2[i];
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.META_DATA)
			{
				Network.HistMetaData histMetaData = (Network.HistMetaData)power;
				if (histMetaData.MetaType == 1)
				{
					if (histMetaData.Info.Contains(entityId2))
					{
						num = i;
						break;
					}
				}
			}
		}
		for (int j = 0; j < num; j++)
		{
			PowerTask powerTask2 = taskList2[j];
			Network.PowerHistory power2 = powerTask2.GetPower();
			switch (power2.Type)
			{
			case Network.PowerType.SHOW_ENTITY:
			{
				Network.HistShowEntity histShowEntity = (Network.HistShowEntity)power2;
				if (entityId == histShowEntity.Entity.ID)
				{
					GameUtils.ApplyShowEntity(duplicatedEntity, histShowEntity);
				}
				if (entityId2 == histShowEntity.Entity.ID)
				{
					GameUtils.ApplyShowEntity(duplicatedEntity2, histShowEntity);
				}
				break;
			}
			case Network.PowerType.HIDE_ENTITY:
			{
				Network.HistHideEntity histHideEntity = (Network.HistHideEntity)power2;
				if (entityId == histHideEntity.Entity)
				{
					GameUtils.ApplyHideEntity(duplicatedEntity, histHideEntity);
				}
				if (entityId2 == histHideEntity.Entity)
				{
					GameUtils.ApplyHideEntity(duplicatedEntity2, histHideEntity);
				}
				break;
			}
			case Network.PowerType.TAG_CHANGE:
			{
				Network.HistTagChange histTagChange = (Network.HistTagChange)power2;
				if (entityId == histTagChange.Entity)
				{
					GameUtils.ApplyTagChange(duplicatedEntity, histTagChange);
				}
				if (entityId2 == histTagChange.Entity)
				{
					GameUtils.ApplyTagChange(duplicatedEntity2, histTagChange);
				}
				break;
			}
			}
		}
	}

	// Token: 0x0600441A RID: 17434 RVA: 0x00146D98 File Offset: 0x00144F98
	public void CreateFatigueTile()
	{
		if (this.m_historyDisabled)
		{
			return;
		}
		HistoryManager.TileEntry tileEntry = new HistoryManager.TileEntry();
		this.m_queuedEntries.Add(tileEntry);
		tileEntry.SetFatigue();
	}

	// Token: 0x0600441B RID: 17435 RVA: 0x00146DCC File Offset: 0x00144FCC
	public void MarkCurrentHistoryEntryAsCompleted()
	{
		if (this.m_historyDisabled)
		{
			return;
		}
		HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
		currentHistoryEntry.m_complete = true;
		this.LoadNextHistoryEntry();
	}

	// Token: 0x0600441C RID: 17436 RVA: 0x00146DF9 File Offset: 0x00144FF9
	public bool HasHistoryEntry()
	{
		return this.GetCurrentHistoryEntry() != null;
	}

	// Token: 0x0600441D RID: 17437 RVA: 0x00146E08 File Offset: 0x00145008
	public void NotifyDamageChanged(Entity entity, int damage)
	{
		if (entity == null)
		{
			return;
		}
		if (this.m_historyDisabled)
		{
			return;
		}
		HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
		int damageChangeAmount = damage - entity.GetDamage();
		if (this.IsEntityTheLastCardPlayed(entity))
		{
			currentHistoryEntry.m_lastCardPlayed.m_damageChangeAmount = damageChangeAmount;
			return;
		}
		if (this.IsEntityTheLastAttacker(entity))
		{
			currentHistoryEntry.m_lastAttacker.m_damageChangeAmount = damageChangeAmount;
			return;
		}
		if (this.IsEntityTheLastDefender(entity))
		{
			currentHistoryEntry.m_lastDefender.m_damageChangeAmount = damageChangeAmount;
			return;
		}
		if (this.IsEntityTheLastCardTargeted(entity))
		{
			currentHistoryEntry.m_lastCardTargeted.m_damageChangeAmount = damageChangeAmount;
			return;
		}
		for (int i = 0; i < currentHistoryEntry.m_affectedCards.Count; i++)
		{
			if (this.IsEntityTheAffectedCard(entity, i))
			{
				currentHistoryEntry.m_affectedCards[i].m_damageChangeAmount = damageChangeAmount;
				return;
			}
		}
		this.NotifyEntityAffected(entity, false);
		this.NotifyDamageChanged(entity, damage);
	}

	// Token: 0x0600441E RID: 17438 RVA: 0x00146EE8 File Offset: 0x001450E8
	public void NotifyArmorChanged(Entity entity, int newArmor)
	{
		if (this.m_historyDisabled)
		{
			return;
		}
		int num = entity.GetArmor() - newArmor;
		if (num <= 0)
		{
			return;
		}
		if (this.IsEntityTheLastCardPlayed(entity))
		{
			return;
		}
		HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
		if (this.IsEntityTheLastAttacker(entity))
		{
			currentHistoryEntry.m_lastAttacker.m_armorChangeAmount = num;
			return;
		}
		if (this.IsEntityTheLastDefender(entity))
		{
			currentHistoryEntry.m_lastDefender.m_armorChangeAmount = num;
			return;
		}
		if (this.IsEntityTheLastCardTargeted(entity))
		{
			currentHistoryEntry.m_lastCardTargeted.m_armorChangeAmount = num;
			return;
		}
		for (int i = 0; i < currentHistoryEntry.m_affectedCards.Count; i++)
		{
			if (this.IsEntityTheAffectedCard(entity, i))
			{
				currentHistoryEntry.m_affectedCards[i].m_armorChangeAmount = num;
				return;
			}
		}
		this.NotifyEntityAffected(entity, false);
		this.NotifyDamageChanged(entity, newArmor);
	}

	// Token: 0x0600441F RID: 17439 RVA: 0x00146FC0 File Offset: 0x001451C0
	public void NotifyEntityAffected(int entityId, bool allowDuplicates = false)
	{
		Entity entity = GameState.Get().GetEntity(entityId);
		this.NotifyEntityAffected(entity, allowDuplicates);
	}

	// Token: 0x06004420 RID: 17440 RVA: 0x00146FE4 File Offset: 0x001451E4
	public void NotifyEntityAffected(Entity entity, bool allowDuplicates = false)
	{
		if (this.m_historyDisabled)
		{
			return;
		}
		if (entity.IsEnchantment())
		{
			return;
		}
		HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
		if (!allowDuplicates)
		{
			if (this.IsEntityTheLastAttacker(entity))
			{
				return;
			}
			if (this.IsEntityTheLastDefender(entity))
			{
				return;
			}
			if (this.IsEntityTheLastCardTargeted(entity))
			{
				return;
			}
			if (currentHistoryEntry.m_lastCardPlayed != null && entity == currentHistoryEntry.m_lastCardPlayed.GetOriginalEntity())
			{
				return;
			}
			for (int i = 0; i < currentHistoryEntry.m_affectedCards.Count; i++)
			{
				if (this.IsEntityTheAffectedCard(entity, i))
				{
					return;
				}
			}
		}
		HistoryInfo historyInfo = new HistoryInfo();
		historyInfo.SetOriginalEntity(entity);
		currentHistoryEntry.m_affectedCards.Add(historyInfo);
	}

	// Token: 0x06004421 RID: 17441 RVA: 0x001470A0 File Offset: 0x001452A0
	public void NotifyEntityDied(int entityId)
	{
		Entity entity = GameState.Get().GetEntity(entityId);
		this.NotifyEntityDied(entity);
	}

	// Token: 0x06004422 RID: 17442 RVA: 0x001470C0 File Offset: 0x001452C0
	public void NotifyEntityDied(Entity entity)
	{
		if (this.m_historyDisabled)
		{
			return;
		}
		if (entity.IsEnchantment())
		{
			return;
		}
		if (this.IsEntityTheLastCardPlayed(entity))
		{
			return;
		}
		HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
		if (this.IsEntityTheLastAttacker(entity))
		{
			currentHistoryEntry.m_lastAttacker.SetDied(true);
			return;
		}
		if (this.IsEntityTheLastDefender(entity))
		{
			currentHistoryEntry.m_lastDefender.SetDied(true);
			return;
		}
		if (this.IsEntityTheLastCardTargeted(entity))
		{
			currentHistoryEntry.m_lastCardTargeted.SetDied(true);
			return;
		}
		for (int i = 0; i < currentHistoryEntry.m_affectedCards.Count; i++)
		{
			if (this.IsEntityTheAffectedCard(entity, i))
			{
				currentHistoryEntry.m_affectedCards[i].SetDied(true);
				return;
			}
		}
		if (this.IsDeadInLaterHistoryEntry(entity))
		{
			return;
		}
		this.NotifyEntityAffected(entity, false);
		this.NotifyEntityDied(entity);
	}

	// Token: 0x06004423 RID: 17443 RVA: 0x0014719C File Offset: 0x0014539C
	public void NotifyOfInput(float zPosition)
	{
		if (this.m_historyTiles.Count == 0)
		{
			this.CheckForMouseOff();
			return;
		}
		float num = 1000f;
		float num2 = -1000f;
		float num3 = 1000f;
		HistoryCard historyCard = null;
		foreach (HistoryCard historyCard2 in this.m_historyTiles)
		{
			if (historyCard2.HasBeenShown())
			{
				Collider tileCollider = historyCard2.GetTileCollider();
				if (!(tileCollider == null))
				{
					float num4 = tileCollider.bounds.center.z - tileCollider.bounds.extents.z;
					float num5 = tileCollider.bounds.center.z + tileCollider.bounds.extents.z;
					if (num4 < num)
					{
						num = num4;
					}
					if (num5 > num2)
					{
						num2 = num5;
					}
					float num6 = Mathf.Abs(zPosition - num4);
					if (num6 < num3)
					{
						num3 = num6;
						historyCard = historyCard2;
					}
					float num7 = Mathf.Abs(zPosition - num5);
					if (num7 < num3)
					{
						num3 = num7;
						historyCard = historyCard2;
					}
				}
			}
		}
		if (zPosition < num || zPosition > num2)
		{
			this.CheckForMouseOff();
			return;
		}
		if (historyCard == null)
		{
			this.CheckForMouseOff();
			return;
		}
		this.m_SoundDucker.StartDucking();
		if (historyCard == this.m_currentlyMousedOverTile)
		{
			return;
		}
		if (this.m_currentlyMousedOverTile != null)
		{
			this.m_currentlyMousedOverTile.NotifyMousedOut();
		}
		else
		{
			this.FadeVignetteIn();
		}
		this.m_currentlyMousedOverTile = historyCard;
		historyCard.NotifyMousedOver();
	}

	// Token: 0x06004424 RID: 17444 RVA: 0x00147374 File Offset: 0x00145574
	public void NotifyOfMouseOff()
	{
		this.CheckForMouseOff();
	}

	// Token: 0x06004425 RID: 17445 RVA: 0x0014737C File Offset: 0x0014557C
	public void UpdateLayout()
	{
		if (this.UserIsMousedOverAHistoryTile())
		{
			return;
		}
		float num = 0f;
		Vector3 topTilePosition = this.GetTopTilePosition();
		for (int i = this.m_historyTiles.Count - 1; i >= 0; i--)
		{
			int num2 = 0;
			if (this.m_historyTiles[i].IsHalfSize())
			{
				num2 = 1;
			}
			Collider tileCollider = this.m_historyTiles[i].GetTileCollider();
			float num3 = 0f;
			if (tileCollider != null)
			{
				num3 = tileCollider.bounds.size.z / 2f;
			}
			Vector3 position;
			position..ctor(topTilePosition.x, topTilePosition.y, topTilePosition.z - num + (float)num2 * num3);
			this.m_historyTiles[i].MarkAsShown();
			iTween.MoveTo(this.m_historyTiles[i].gameObject, position, 1f);
			if (tileCollider != null)
			{
				num += tileCollider.bounds.size.z + 0.15f;
			}
		}
		this.DestroyHistoryTilesThatFallOffTheEnd();
	}

	// Token: 0x06004426 RID: 17446 RVA: 0x001474A9 File Offset: 0x001456A9
	public void SetBigTileSize(float size)
	{
		this.m_sizeOfBigTile = size;
	}

	// Token: 0x06004427 RID: 17447 RVA: 0x001474B2 File Offset: 0x001456B2
	public int GetNumHistoryTiles()
	{
		return this.m_historyTiles.Count;
	}

	// Token: 0x06004428 RID: 17448 RVA: 0x001474C0 File Offset: 0x001456C0
	public int GetIndexForTile(HistoryCard tile)
	{
		for (int i = 0; i < this.m_historyTiles.Count; i++)
		{
			if (this.m_historyTiles[i] == tile)
			{
				return i;
			}
		}
		Debug.LogWarning("HistoryManager.GetIndexForTile() - that Tile doesn't exist!");
		return -1;
	}

	// Token: 0x06004429 RID: 17449 RVA: 0x00147510 File Offset: 0x00145710
	private void LoadNextHistoryEntry()
	{
		if (this.m_queuedEntries.Count == 0)
		{
			return;
		}
		if (!this.m_queuedEntries[0].m_complete)
		{
			return;
		}
		base.StartCoroutine(this.LoadNextHistoryEntryWhenLoaded());
	}

	// Token: 0x0600442A RID: 17450 RVA: 0x00147554 File Offset: 0x00145754
	private IEnumerator LoadNextHistoryEntryWhenLoaded()
	{
		HistoryManager.TileEntry currentEntry = this.m_queuedEntries[0];
		this.m_queuedEntries.RemoveAt(0);
		while (!currentEntry.CanDuplicateAllEntities())
		{
			yield return null;
		}
		currentEntry.DuplicateAllEntities();
		HistoryInfo sourceInfo = currentEntry.GetSourceInfo();
		this.CreateTransformTile(sourceInfo);
		List<HistoryInfo> infos = new List<HistoryInfo>();
		infos.Add(sourceInfo);
		HistoryInfo targetInfo = currentEntry.GetTargetInfo();
		if (targetInfo != null)
		{
			infos.Add(targetInfo);
		}
		if (currentEntry.m_affectedCards.Count > 0)
		{
			infos.AddRange(currentEntry.m_affectedCards);
		}
		AssetLoader.Get().LoadActor("HistoryCard", new AssetLoader.GameObjectCallback(this.TileLoadedCallback), infos, false);
		yield break;
	}

	// Token: 0x0600442B RID: 17451 RVA: 0x00147570 File Offset: 0x00145770
	private void CreateTransformTile(HistoryInfo sourceInfo)
	{
		if (sourceInfo.m_infoType == HistoryInfoType.FATIGUE)
		{
			return;
		}
		Entity duplicatedEntity = sourceInfo.GetDuplicatedEntity();
		Entity originalEntity = sourceInfo.GetOriginalEntity();
		if (duplicatedEntity == null || originalEntity == null)
		{
			return;
		}
		int tag = duplicatedEntity.GetTag(GAME_TAG.TRANSFORMED_FROM_CARD);
		if (tag == 0)
		{
			return;
		}
		string text = GameUtils.TranslateDbIdToCardId(tag);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		Entity originalEntity2 = this.CreatePreTransformedEntity(duplicatedEntity);
		HistoryInfo historyInfo = new HistoryInfo();
		historyInfo.SetOriginalEntity(originalEntity2);
		HistoryInfo historyInfo2 = new HistoryInfo();
		historyInfo2.SetOriginalEntity(originalEntity);
		List<HistoryInfo> list = new List<HistoryInfo>();
		list.Add(historyInfo);
		list.Add(historyInfo2);
		AssetLoader.Get().LoadActor("HistoryCard", new AssetLoader.GameObjectCallback(this.TileLoadedCallback), list, false);
	}

	// Token: 0x0600442C RID: 17452 RVA: 0x0014762C File Offset: 0x0014582C
	private IEnumerator WaitForCardLoadedAndDuplicateInfo(HistoryInfo info)
	{
		while (!info.CanDuplicateEntity())
		{
			yield return null;
		}
		info.DuplicateEntity();
		yield break;
	}

	// Token: 0x0600442D RID: 17453 RVA: 0x00147650 File Offset: 0x00145850
	private bool IsEntityTheLastCardTargeted(Entity entity)
	{
		HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
		return currentHistoryEntry.m_lastCardTargeted != null && entity == currentHistoryEntry.m_lastCardTargeted.GetOriginalEntity();
	}

	// Token: 0x0600442E RID: 17454 RVA: 0x00147680 File Offset: 0x00145880
	private bool IsEntityTheLastAttacker(Entity entity)
	{
		HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
		return currentHistoryEntry.m_lastAttacker != null && entity == currentHistoryEntry.m_lastAttacker.GetOriginalEntity();
	}

	// Token: 0x0600442F RID: 17455 RVA: 0x001476B0 File Offset: 0x001458B0
	private bool IsEntityTheLastCardPlayed(Entity entity)
	{
		HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
		return currentHistoryEntry.m_lastCardPlayed != null && entity == currentHistoryEntry.m_lastCardPlayed.GetOriginalEntity();
	}

	// Token: 0x06004430 RID: 17456 RVA: 0x001476E0 File Offset: 0x001458E0
	private bool IsEntityTheLastDefender(Entity entity)
	{
		HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
		return currentHistoryEntry.m_lastDefender != null && entity == currentHistoryEntry.m_lastDefender.GetOriginalEntity();
	}

	// Token: 0x06004431 RID: 17457 RVA: 0x00147710 File Offset: 0x00145910
	private bool IsEntityTheAffectedCard(Entity entity, int index)
	{
		HistoryManager.TileEntry currentHistoryEntry = this.GetCurrentHistoryEntry();
		return currentHistoryEntry.m_affectedCards[index] != null && entity == currentHistoryEntry.m_affectedCards[index].GetOriginalEntity();
	}

	// Token: 0x06004432 RID: 17458 RVA: 0x0014774C File Offset: 0x0014594C
	private HistoryManager.TileEntry GetCurrentHistoryEntry()
	{
		if (this.m_queuedEntries.Count == 0)
		{
			return null;
		}
		for (int i = this.m_queuedEntries.Count - 1; i >= 0; i--)
		{
			if (!this.m_queuedEntries[i].m_complete)
			{
				return this.m_queuedEntries[i];
			}
		}
		return null;
	}

	// Token: 0x06004433 RID: 17459 RVA: 0x001477B0 File Offset: 0x001459B0
	private bool IsDeadInLaterHistoryEntry(Entity entity)
	{
		bool result = false;
		for (int i = this.m_queuedEntries.Count - 1; i >= 0; i--)
		{
			HistoryManager.TileEntry tileEntry = this.m_queuedEntries[i];
			if (!tileEntry.m_complete)
			{
				return result;
			}
			for (int j = 0; j < tileEntry.m_affectedCards.Count; j++)
			{
				HistoryInfo historyInfo = tileEntry.m_affectedCards[j];
				if (historyInfo.GetOriginalEntity() == entity && (historyInfo.HasDied() || historyInfo.GetSplatAmount() >= entity.GetRemainingHealth()))
				{
					result = true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004434 RID: 17460 RVA: 0x00147850 File Offset: 0x00145A50
	private void TileLoadedCallback(string actorName, GameObject actorObject, object callbackData)
	{
		List<HistoryInfo> list = (List<HistoryInfo>)callbackData;
		HistoryInfo historyInfo = list[0];
		list.RemoveAt(0);
		HistoryTileInitInfo historyTileInitInfo = new HistoryTileInitInfo();
		historyTileInitInfo.m_type = historyInfo.m_infoType;
		historyTileInitInfo.m_childInfos = list;
		if (historyTileInitInfo.m_type == HistoryInfoType.FATIGUE)
		{
			historyTileInitInfo.m_fatigueTexture = this.m_FatigueTexture;
		}
		else
		{
			Entity duplicatedEntity = historyInfo.GetDuplicatedEntity();
			Card card = duplicatedEntity.GetCard();
			CardDef cardDef = card.GetCardDef();
			historyTileInitInfo.m_entity = duplicatedEntity;
			historyTileInitInfo.m_portraitTexture = this.DeterminePortraitTextureForTiles(duplicatedEntity, cardDef);
			historyTileInitInfo.m_portraitGoldenMaterial = cardDef.GetPremiumPortraitMaterial();
			historyTileInitInfo.m_fullTileMaterial = cardDef.GetHistoryTileFullPortrait();
			historyTileInitInfo.m_halfTileMaterial = cardDef.GetHistoryTileHalfPortrait();
			historyTileInitInfo.m_splatAmount = historyInfo.GetSplatAmount();
			historyTileInitInfo.m_dead = historyInfo.HasDied();
		}
		HistoryCard component = actorObject.GetComponent<HistoryCard>();
		this.m_historyTiles.Add(component);
		component.LoadTile(historyTileInitInfo);
		this.SetAsideTileAndTryToUpdate(component);
		this.LoadNextHistoryEntry();
	}

	// Token: 0x06004435 RID: 17461 RVA: 0x00147944 File Offset: 0x00145B44
	private Texture DeterminePortraitTextureForTiles(Entity entity, CardDef cardDef)
	{
		Texture result;
		if (entity.IsSecret() && entity.IsHidden() && entity.IsControlledByConcealedPlayer())
		{
			if (entity.GetClass() == TAG_CLASS.PALADIN)
			{
				result = this.m_paladinSecretTexture;
			}
			else if (entity.GetClass() == TAG_CLASS.HUNTER)
			{
				result = this.m_hunterSecretTexture;
			}
			else
			{
				result = this.m_mageSecretTexture;
			}
		}
		else if (entity.GetController() != null && !entity.GetController().IsFriendlySide() && entity.IsObfuscated())
		{
			result = this.m_paladinSecretTexture;
		}
		else
		{
			result = cardDef.GetPortraitTexture();
		}
		return result;
	}

	// Token: 0x06004436 RID: 17462 RVA: 0x001479EA File Offset: 0x00145BEA
	private void CheckForMouseOff()
	{
		if (this.m_currentlyMousedOverTile == null)
		{
			return;
		}
		this.m_currentlyMousedOverTile.NotifyMousedOut();
		this.m_currentlyMousedOverTile = null;
		this.m_SoundDucker.StopDucking();
		this.FadeVignetteOut();
	}

	// Token: 0x06004437 RID: 17463 RVA: 0x00147A24 File Offset: 0x00145C24
	private void DestroyHistoryTilesThatFallOffTheEnd()
	{
		if (this.m_sizeOfBigTile <= 0f)
		{
			return;
		}
		float num = 0f;
		float z = base.GetComponent<Collider>().bounds.size.z;
		for (int i = 0; i < this.m_historyTiles.Count; i++)
		{
			if (this.m_historyTiles[i].IsHalfSize())
			{
				num += this.m_sizeOfBigTile / 2f;
			}
			else
			{
				num += this.m_sizeOfBigTile;
			}
		}
		num += 0.15f * (float)(this.m_historyTiles.Count - 1);
		while (num > z)
		{
			if (this.m_historyTiles[0].IsHalfSize())
			{
				num -= this.m_sizeOfBigTile / 2f;
			}
			else
			{
				num -= this.m_sizeOfBigTile;
			}
			num -= 0.15f;
			Object.Destroy(this.m_historyTiles[0].gameObject);
			this.m_historyTiles.RemoveAt(0);
		}
	}

	// Token: 0x06004438 RID: 17464 RVA: 0x00147B34 File Offset: 0x00145D34
	private void SetAsideTileAndTryToUpdate(HistoryCard tile)
	{
		Vector3 topTilePosition = this.GetTopTilePosition();
		tile.transform.position = new Vector3(topTilePosition.x - 20f, topTilePosition.y, topTilePosition.z);
		this.UpdateLayout();
	}

	// Token: 0x06004439 RID: 17465 RVA: 0x00147B7C File Offset: 0x00145D7C
	private Vector3 GetTopTilePosition()
	{
		return new Vector3(base.transform.position.x, base.transform.position.y - 0.15f, base.transform.position.z);
	}

	// Token: 0x0600443A RID: 17466 RVA: 0x00147BD0 File Offset: 0x00145DD0
	private bool UserIsMousedOverAHistoryTile()
	{
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(GameLayer.Default.LayerBit(), out raycastHit) && raycastHit.transform.GetComponentInChildren<HistoryManager>() == null && raycastHit.transform.GetComponentInChildren<HistoryCard>() == null)
		{
			return false;
		}
		float z = raycastHit.point.z;
		float num = 1000f;
		float num2 = -1000f;
		foreach (HistoryCard historyCard in this.m_historyTiles)
		{
			if (historyCard.HasBeenShown())
			{
				Collider tileCollider = historyCard.GetTileCollider();
				if (!(tileCollider == null))
				{
					float num3 = tileCollider.bounds.center.z - tileCollider.bounds.extents.z;
					float num4 = tileCollider.bounds.center.z + tileCollider.bounds.extents.z;
					if (num3 < num)
					{
						num = num3;
					}
					if (num4 > num2)
					{
						num2 = num4;
					}
				}
			}
		}
		return z >= num && z <= num2;
	}

	// Token: 0x0600443B RID: 17467 RVA: 0x00147D4C File Offset: 0x00145F4C
	private void FadeVignetteIn()
	{
		foreach (HistoryCard historyCard in this.m_historyTiles)
		{
			if (!(historyCard.m_tileActor == null))
			{
				SceneUtils.SetLayer(historyCard.m_tileActor.gameObject, GameLayer.IgnoreFullScreenEffects);
			}
		}
		SceneUtils.SetLayer(base.gameObject, GameLayer.IgnoreFullScreenEffects);
		FullScreenEffects component = Camera.main.GetComponent<FullScreenEffects>();
		component.VignettingEnable = true;
		component.DesaturationEnabled = true;
		this.AnimateVignetteIn();
	}

	// Token: 0x0600443C RID: 17468 RVA: 0x00147DF4 File Offset: 0x00145FF4
	private void FadeVignetteOut()
	{
		foreach (HistoryCard historyCard in this.m_historyTiles)
		{
			if (!(historyCard.m_tileActor == null))
			{
				SceneUtils.SetLayer(historyCard.GetTileCollider().gameObject, GameLayer.Default);
			}
		}
		SceneUtils.SetLayer(base.gameObject, GameLayer.CardRaycast);
		this.AnimateVignetteOut();
	}

	// Token: 0x0600443D RID: 17469 RVA: 0x00147E80 File Offset: 0x00146080
	private void AnimateVignetteIn()
	{
		FullScreenEffects component = Camera.main.GetComponent<FullScreenEffects>();
		this.m_animatingVignette = component.VignettingEnable;
		if (this.m_animatingVignette)
		{
			Hashtable args = iTween.Hash(new object[]
			{
				"from",
				component.VignettingIntensity,
				"to",
				0.6f,
				"time",
				0.4f,
				"easetype",
				iTween.EaseType.easeInOutQuad,
				"onupdate",
				"OnUpdateVignetteVal",
				"onupdatetarget",
				base.gameObject,
				"name",
				"historyVig",
				"oncomplete",
				"OnVignetteInFinished",
				"oncompletetarget",
				base.gameObject
			});
			iTween.StopByName(Camera.main.gameObject, "historyVig");
			iTween.ValueTo(Camera.main.gameObject, args);
		}
		this.m_animatingDesat = component.DesaturationEnabled;
		if (this.m_animatingDesat)
		{
			Hashtable args2 = iTween.Hash(new object[]
			{
				"from",
				component.Desaturation,
				"to",
				1f,
				"time",
				0.4f,
				"easetype",
				iTween.EaseType.easeInOutQuad,
				"onupdate",
				"OnUpdateDesatVal",
				"onupdatetarget",
				base.gameObject,
				"name",
				"historyDesat",
				"oncomplete",
				"OnDesatInFinished",
				"oncompletetarget",
				base.gameObject
			});
			iTween.StopByName(Camera.main.gameObject, "historyDesat");
			iTween.ValueTo(Camera.main.gameObject, args2);
		}
	}

	// Token: 0x0600443E RID: 17470 RVA: 0x00148080 File Offset: 0x00146280
	private void AnimateVignetteOut()
	{
		FullScreenEffects component = Camera.main.GetComponent<FullScreenEffects>();
		this.m_animatingVignette = component.VignettingEnable;
		if (this.m_animatingVignette)
		{
			Hashtable args = iTween.Hash(new object[]
			{
				"from",
				component.VignettingIntensity,
				"to",
				0f,
				"time",
				0.4f,
				"easetype",
				iTween.EaseType.easeInOutQuad,
				"onupdate",
				"OnUpdateVignetteVal",
				"onupdatetarget",
				base.gameObject,
				"name",
				"historyVig",
				"oncomplete",
				"OnVignetteOutFinished",
				"oncompletetarget",
				base.gameObject
			});
			iTween.StopByName(Camera.main.gameObject, "historyVig");
			iTween.ValueTo(Camera.main.gameObject, args);
		}
		this.m_animatingDesat = component.DesaturationEnabled;
		if (this.m_animatingDesat)
		{
			Hashtable args2 = iTween.Hash(new object[]
			{
				"from",
				component.Desaturation,
				"to",
				0f,
				"time",
				0.4f,
				"easetype",
				iTween.EaseType.easeInOutQuad,
				"onupdate",
				"OnUpdateDesatVal",
				"onupdatetarget",
				base.gameObject,
				"name",
				"historyDesat",
				"oncomplete",
				"OnDesatOutFinished",
				"oncompletetarget",
				base.gameObject
			});
			iTween.StopByName(Camera.main.gameObject, "historyDesat");
			iTween.ValueTo(Camera.main.gameObject, args2);
		}
	}

	// Token: 0x0600443F RID: 17471 RVA: 0x00148280 File Offset: 0x00146480
	private void OnUpdateVignetteVal(float val)
	{
		FullScreenEffects component = Camera.main.GetComponent<FullScreenEffects>();
		component.VignettingIntensity = val;
	}

	// Token: 0x06004440 RID: 17472 RVA: 0x001482A0 File Offset: 0x001464A0
	private void OnUpdateDesatVal(float val)
	{
		FullScreenEffects component = Camera.main.GetComponent<FullScreenEffects>();
		component.Desaturation = val;
	}

	// Token: 0x06004441 RID: 17473 RVA: 0x001482BF File Offset: 0x001464BF
	private void OnVignetteInFinished()
	{
		this.m_animatingVignette = false;
	}

	// Token: 0x06004442 RID: 17474 RVA: 0x001482C8 File Offset: 0x001464C8
	private void OnDesatInFinished()
	{
		this.m_animatingDesat = false;
	}

	// Token: 0x06004443 RID: 17475 RVA: 0x001482D4 File Offset: 0x001464D4
	private void OnVignetteOutFinished()
	{
		this.m_animatingVignette = false;
		FullScreenEffects component = Camera.main.GetComponent<FullScreenEffects>();
		component.VignettingEnable = false;
		this.OnFullScreenEffectOutFinished();
	}

	// Token: 0x06004444 RID: 17476 RVA: 0x00148300 File Offset: 0x00146500
	private void OnDesatOutFinished()
	{
		this.m_animatingDesat = false;
		FullScreenEffects component = Camera.main.GetComponent<FullScreenEffects>();
		component.DesaturationEnabled = false;
		this.OnFullScreenEffectOutFinished();
	}

	// Token: 0x06004445 RID: 17477 RVA: 0x0014832C File Offset: 0x0014652C
	private void OnFullScreenEffectOutFinished()
	{
		if (this.m_animatingDesat || this.m_animatingVignette)
		{
			return;
		}
		FullScreenEffects component = Camera.main.GetComponent<FullScreenEffects>();
		component.Disable();
		foreach (HistoryCard historyCard in this.m_historyTiles)
		{
			if (!(historyCard.m_tileActor == null))
			{
				SceneUtils.SetLayer(historyCard.m_tileActor.gameObject, GameLayer.Default);
			}
		}
	}

	// Token: 0x06004446 RID: 17478 RVA: 0x001483D0 File Offset: 0x001465D0
	public bool HasBigCard()
	{
		return this.m_currentBigCard != null;
	}

	// Token: 0x06004447 RID: 17479 RVA: 0x001483DE File Offset: 0x001465DE
	public HistoryCard GetCurrentBigCard()
	{
		return this.m_currentBigCard;
	}

	// Token: 0x06004448 RID: 17480 RVA: 0x001483E6 File Offset: 0x001465E6
	public Entity GetPendingBigCardEntity()
	{
		if (this.m_pendingBigCardEntry == null)
		{
			return null;
		}
		return this.m_pendingBigCardEntry.m_info.GetOriginalEntity();
	}

	// Token: 0x06004449 RID: 17481 RVA: 0x00148408 File Offset: 0x00146608
	public void CreatePlayedBigCard(Entity entity, HistoryManager.BigCardFinishedCallback callback, bool fromMetaData, bool countered)
	{
		if (!GameState.Get().GetGameEntity().ShouldShowBigCard())
		{
			callback();
			return;
		}
		base.StopCoroutine("WaitForCardLoadedAndCreateBigCard");
		HistoryManager.BigCardEntry bigCardEntry = new HistoryManager.BigCardEntry();
		bigCardEntry.m_info = new HistoryInfo();
		bigCardEntry.m_info.SetOriginalEntity(entity);
		if (entity.IsWeapon())
		{
			bigCardEntry.m_info.m_infoType = HistoryInfoType.WEAPON_PLAYED;
		}
		else
		{
			bigCardEntry.m_info.m_infoType = HistoryInfoType.CARD_PLAYED;
		}
		bigCardEntry.m_finishedCallback = callback;
		bigCardEntry.m_fromMetaData = fromMetaData;
		bigCardEntry.m_countered = countered;
		base.StartCoroutine("WaitForCardLoadedAndCreateBigCard", bigCardEntry);
	}

	// Token: 0x0600444A RID: 17482 RVA: 0x001484A4 File Offset: 0x001466A4
	public void CreateTriggeredBigCard(Entity entity, HistoryManager.BigCardFinishedCallback callback, bool fromMetaData, bool isSecret)
	{
		if (!GameState.Get().GetGameEntity().ShouldShowBigCard())
		{
			callback();
			return;
		}
		base.StopCoroutine("WaitForCardLoadedAndCreateBigCard");
		HistoryManager.BigCardEntry bigCardEntry = new HistoryManager.BigCardEntry();
		bigCardEntry.m_info = new HistoryInfo();
		bigCardEntry.m_info.SetOriginalEntity(entity);
		bigCardEntry.m_info.m_infoType = HistoryInfoType.TRIGGER;
		bigCardEntry.m_fromMetaData = fromMetaData;
		bigCardEntry.m_finishedCallback = callback;
		bigCardEntry.m_waitForSecretSpell = isSecret;
		base.StartCoroutine("WaitForCardLoadedAndCreateBigCard", bigCardEntry);
	}

	// Token: 0x0600444B RID: 17483 RVA: 0x00148523 File Offset: 0x00146723
	public void NotifyOfSecretSpellFinished()
	{
		this.m_bigCardWaitingForSecret = false;
	}

	// Token: 0x0600444C RID: 17484 RVA: 0x0014852C File Offset: 0x0014672C
	public void HandleClickOnBigCard(HistoryCard card)
	{
		if (this.m_currentBigCard && this.m_currentBigCard != card)
		{
			return;
		}
		this.OnCurrentBigCardClicked();
	}

	// Token: 0x0600444D RID: 17485 RVA: 0x00148564 File Offset: 0x00146764
	private IEnumerator WaitForBoardLoadedAndSetPaths()
	{
		while (ZoneMgr.Get() == null)
		{
			yield return null;
		}
		Transform bigCardPathPoint = Board.Get().FindBone("BigCardPathPoint");
		if (bigCardPathPoint == null)
		{
			yield break;
		}
		this.m_bigCardPath = new Vector3[3];
		this.m_bigCardPath[1] = bigCardPathPoint.position;
		this.m_bigCardPath[2] = this.GetBigCardPosition();
		yield break;
	}

	// Token: 0x0600444E RID: 17486 RVA: 0x00148580 File Offset: 0x00146780
	private Vector3 GetBigCardPosition()
	{
		Transform transform = Board.Get().FindBone("BigCardPosition");
		return transform.position;
	}

	// Token: 0x0600444F RID: 17487 RVA: 0x001485A4 File Offset: 0x001467A4
	private IEnumerator WaitForCardLoadedAndCreateBigCard(HistoryManager.BigCardEntry bigCardEntry)
	{
		this.m_pendingBigCardEntry = bigCardEntry;
		HistoryInfo info = bigCardEntry.m_info;
		while (!info.CanDuplicateEntity())
		{
			yield return null;
		}
		info.DuplicateEntity();
		this.m_pendingBigCardEntry = null;
		AssetLoader.Get().LoadActor("HistoryCard", new AssetLoader.GameObjectCallback(this.BigCardLoadedCallback), bigCardEntry, false);
		yield break;
	}

	// Token: 0x06004450 RID: 17488 RVA: 0x001485D0 File Offset: 0x001467D0
	private void BigCardLoadedCallback(string actorName, GameObject actorObject, object callbackData)
	{
		HistoryManager.BigCardEntry bigCardEntry = (HistoryManager.BigCardEntry)callbackData;
		HistoryInfo info = bigCardEntry.m_info;
		Entity entity = info.GetDuplicatedEntity();
		Card card = entity.GetCard();
		CardDef cardDef = card.GetCardDef();
		if (entity.GetCardType() == TAG_CARDTYPE.SPELL || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
		{
			actorObject.transform.position = card.transform.position;
			actorObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		}
		else
		{
			actorObject.transform.position = this.GetBigCardPosition();
		}
		Entity entity2 = this.CreatePreTransformedEntity(entity);
		Entity postTransformedEntity = null;
		if (entity2 != null)
		{
			postTransformedEntity = entity;
			entity = entity2;
			card = entity.GetCard();
			cardDef = card.GetCardDef();
		}
		HistoryBigCardInitInfo historyBigCardInitInfo = new HistoryBigCardInitInfo();
		historyBigCardInitInfo.m_entity = entity;
		historyBigCardInitInfo.m_portraitTexture = cardDef.GetPortraitTexture();
		historyBigCardInitInfo.m_portraitGoldenMaterial = cardDef.GetPremiumPortraitMaterial();
		historyBigCardInitInfo.m_finishedCallback = bigCardEntry.m_finishedCallback;
		historyBigCardInitInfo.m_countered = bigCardEntry.m_countered;
		historyBigCardInitInfo.m_waitForSecretSpell = bigCardEntry.m_waitForSecretSpell;
		historyBigCardInitInfo.m_fromMetaData = bigCardEntry.m_fromMetaData;
		historyBigCardInitInfo.m_postTransformedEntity = postTransformedEntity;
		HistoryCard component = actorObject.GetComponent<HistoryCard>();
		component.LoadBigCard(historyBigCardInitInfo);
		if (this.m_currentBigCard)
		{
			this.InterruptCurrentBigCard();
		}
		this.m_currentBigCard = component;
		base.StartCoroutine("WaitThenShowBigCard");
	}

	// Token: 0x06004451 RID: 17489 RVA: 0x00148730 File Offset: 0x00146930
	private IEnumerator WaitThenShowBigCard()
	{
		if (this.m_currentBigCard.IsBigCardWaitingForSecret())
		{
			this.m_bigCardWaitingForSecret = true;
			this.m_currentBigCard.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
			while (this.m_bigCardWaitingForSecret)
			{
				yield return null;
			}
			this.m_currentBigCard.ShowBigCard(this.m_bigCardPath);
			base.StartCoroutine("WaitThenDestroyBigCard");
		}
		else if (this.m_currentBigCard.HasBigCardPostTransformedEntity())
		{
			this.m_currentBigCard.ShowBigCard(this.m_bigCardPath);
			base.StartCoroutine("WaitThenDestroyBigCard");
			while (this.m_bigCardTransformState == HistoryManager.BigCardTransformState.PRE_TRANSFORM || this.m_bigCardTransformState == HistoryManager.BigCardTransformState.TRANSFORM)
			{
				yield return null;
			}
		}
		else
		{
			this.m_currentBigCard.ShowBigCard(this.m_bigCardPath);
			base.StartCoroutine("WaitThenDestroyBigCard");
		}
		yield return new WaitForSeconds(1f);
		this.m_currentBigCard.RunBigCardFinishedCallback();
		yield break;
	}

	// Token: 0x06004452 RID: 17490 RVA: 0x0014874C File Offset: 0x0014694C
	private IEnumerator WaitThenDestroyBigCard()
	{
		float timeToWait = 0f;
		if (this.m_currentBigCard.GetEntity() != null)
		{
			TAG_CARDTYPE curCardType = this.m_currentBigCard.GetEntity().GetCardType();
			if (curCardType == TAG_CARDTYPE.SPELL)
			{
				timeToWait = 4f + GameState.Get().GetGameEntity().GetAdditionalTimeToWaitForSpells();
			}
			else if (curCardType == TAG_CARDTYPE.HERO_POWER)
			{
				timeToWait = 4f + GameState.Get().GetGameEntity().GetAdditionalTimeToWaitForSpells();
			}
			else
			{
				timeToWait = 3f;
			}
		}
		else
		{
			timeToWait = 4f;
		}
		if (this.m_currentBigCard.HasBigCardPostTransformedEntity())
		{
			this.m_bigCardTransformState = HistoryManager.BigCardTransformState.PRE_TRANSFORM;
			timeToWait *= 0.5f;
		}
		if (this.m_currentBigCard.IsBigCardFromMetaData())
		{
			timeToWait *= 0.375f;
		}
		yield return new WaitForSeconds(timeToWait);
		this.DestroyBigCard();
		yield break;
	}

	// Token: 0x06004453 RID: 17491 RVA: 0x00148768 File Offset: 0x00146968
	private void DestroyBigCard()
	{
		if (this.m_currentBigCard == null)
		{
			return;
		}
		if (this.m_currentBigCard.m_mainCardActor == null)
		{
			this.RunFinishedCallbackAndDestroyBigCard();
			return;
		}
		if (this.m_currentBigCard.WasBigCardCountered())
		{
			this.PlayBigCardCounteredEffects();
		}
		else if (this.m_currentBigCard.HasBigCardPostTransformedEntity())
		{
			this.PlayBigCardTransformEffects();
		}
		else
		{
			this.RunFinishedCallbackAndDestroyBigCard();
		}
	}

	// Token: 0x06004454 RID: 17492 RVA: 0x001487E0 File Offset: 0x001469E0
	private void RunFinishedCallbackAndDestroyBigCard()
	{
		if (this.m_currentBigCard == null)
		{
			return;
		}
		this.m_currentBigCard.RunBigCardFinishedCallback();
		Object.Destroy(this.m_currentBigCard.gameObject);
	}

	// Token: 0x06004455 RID: 17493 RVA: 0x00148810 File Offset: 0x00146A10
	private void PlayBigCardCounteredEffects()
	{
		Spell.StateFinishedCallback callback = delegate(Spell s, SpellStateType prevStateType, object userData)
		{
			if (s.GetActiveState() != SpellStateType.NONE)
			{
				return;
			}
			HistoryCard historyCard = (HistoryCard)userData;
			Object.Destroy(historyCard.gameObject);
		};
		Spell spell = this.m_currentBigCard.m_mainCardActor.GetSpell(SpellType.DEATH);
		if (spell == null)
		{
			this.RunFinishedCallbackAndDestroyBigCard();
		}
		else
		{
			spell.AddStateFinishedCallback(callback, this.m_currentBigCard);
			this.m_currentBigCard.RunBigCardFinishedCallback();
			this.m_currentBigCard = null;
			spell.Activate();
		}
	}

	// Token: 0x06004456 RID: 17494 RVA: 0x00148889 File Offset: 0x00146A89
	private void PlayBigCardTransformEffects()
	{
		base.StartCoroutine(this.PlayBigCardTransformEffectsWithTiming());
	}

	// Token: 0x06004457 RID: 17495 RVA: 0x00148898 File Offset: 0x00146A98
	private IEnumerator PlayBigCardTransformEffectsWithTiming()
	{
		if (this.m_bigCardTransformState == HistoryManager.BigCardTransformState.PRE_TRANSFORM)
		{
			this.m_bigCardTransformState = HistoryManager.BigCardTransformState.TRANSFORM;
			yield return base.StartCoroutine(this.PlayBigCardTransformSpell());
		}
		if (this.m_bigCardTransformState == HistoryManager.BigCardTransformState.TRANSFORM)
		{
			this.m_bigCardTransformState = HistoryManager.BigCardTransformState.POST_TRANSFORM;
			yield return base.StartCoroutine(this.WaitForBigCardPostTransform());
		}
		if (this.m_bigCardTransformState == HistoryManager.BigCardTransformState.POST_TRANSFORM)
		{
			this.m_bigCardTransformState = HistoryManager.BigCardTransformState.INVALID;
			this.RunFinishedCallbackAndDestroyBigCard();
		}
		yield break;
	}

	// Token: 0x06004458 RID: 17496 RVA: 0x001488B4 File Offset: 0x00146AB4
	private IEnumerator PlayBigCardTransformSpell()
	{
		this.m_bigCardTransformSpell = Object.Instantiate<Spell>(this.m_TransformSpell);
		if (this.m_bigCardTransformSpell == null)
		{
			yield break;
		}
		Card card = this.m_currentBigCard.GetEntity().GetCard();
		this.m_bigCardTransformSpell.SetSource(card.gameObject);
		this.m_bigCardTransformSpell.AddTarget(card.gameObject);
		this.m_bigCardTransformSpell.m_SetParentToLocation = true;
		this.m_bigCardTransformSpell.UpdateTransform();
		this.m_bigCardTransformSpell.SetPosition(this.m_currentBigCard.m_mainCardActor.transform.position);
		Spell.StateFinishedCallback stateFinishedCallback = delegate(Spell s, SpellStateType prevStateType, object userData)
		{
			if (s.GetActiveState() != SpellStateType.NONE)
			{
				return;
			}
			Object.Destroy(s.gameObject);
		};
		this.m_bigCardTransformSpell.AddStateFinishedCallback(stateFinishedCallback);
		this.m_bigCardTransformSpell.Activate();
		while (this.m_bigCardTransformSpell && !this.m_bigCardTransformSpell.IsFinished())
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06004459 RID: 17497 RVA: 0x001488D0 File Offset: 0x00146AD0
	private IEnumerator WaitForBigCardPostTransform()
	{
		Actor preTransformedActor = this.m_currentBigCard.m_mainCardActor;
		preTransformedActor.Hide(true);
		this.m_currentBigCard.LoadBigCardPostTransformedEntity();
		TransformUtil.CopyLocal(this.m_currentBigCard.m_mainCardActor, preTransformedActor);
		yield return new WaitForSeconds(2f);
		yield break;
	}

	// Token: 0x0600445A RID: 17498 RVA: 0x001488EB File Offset: 0x00146AEB
	private void OnCurrentBigCardClicked()
	{
		if (this.m_currentBigCard.HasBigCardPostTransformedEntity())
		{
			this.ForceNextBigCardTransformState();
		}
		else
		{
			this.InterruptCurrentBigCard();
		}
	}

	// Token: 0x0600445B RID: 17499 RVA: 0x00148910 File Offset: 0x00146B10
	private void ForceNextBigCardTransformState()
	{
		switch (this.m_bigCardTransformState)
		{
		case HistoryManager.BigCardTransformState.PRE_TRANSFORM:
			this.m_bigCardTransformState = HistoryManager.BigCardTransformState.TRANSFORM;
			this.StopWaitingThenDestroyBigCard();
			break;
		case HistoryManager.BigCardTransformState.TRANSFORM:
			if (this.m_bigCardTransformSpell)
			{
				Object.Destroy(this.m_bigCardTransformSpell.gameObject);
			}
			break;
		case HistoryManager.BigCardTransformState.POST_TRANSFORM:
			this.InterruptCurrentBigCard();
			break;
		}
	}

	// Token: 0x0600445C RID: 17500 RVA: 0x0014897F File Offset: 0x00146B7F
	private void StopWaitingThenDestroyBigCard()
	{
		base.StopCoroutine("WaitThenDestroyBigCard");
		this.DestroyBigCard();
	}

	// Token: 0x0600445D RID: 17501 RVA: 0x00148992 File Offset: 0x00146B92
	private void InterruptCurrentBigCard()
	{
		base.StopCoroutine("WaitThenShowBigCard");
		if (this.m_currentBigCard.HasBigCardPostTransformedEntity())
		{
			this.CutoffBigCardTransformEffects();
		}
		else
		{
			this.StopWaitingThenDestroyBigCard();
		}
	}

	// Token: 0x0600445E RID: 17502 RVA: 0x001489C0 File Offset: 0x00146BC0
	private void CutoffBigCardTransformEffects()
	{
		if (this.m_bigCardTransformSpell)
		{
			Object.Destroy(this.m_bigCardTransformSpell.gameObject);
		}
		base.StopCoroutine("PlayBigCardTransformEffectsWithTiming");
		this.m_bigCardTransformState = HistoryManager.BigCardTransformState.INVALID;
		this.RunFinishedCallbackAndDestroyBigCard();
	}

	// Token: 0x04002B3E RID: 11070
	private const float BIG_CARD_POWER_PROCESSOR_DELAY_TIME = 1f;

	// Token: 0x04002B3F RID: 11071
	private const float BIG_CARD_SPELL_DISPLAY_TIME = 4f;

	// Token: 0x04002B40 RID: 11072
	private const float BIG_CARD_MINION_DISPLAY_TIME = 3f;

	// Token: 0x04002B41 RID: 11073
	private const float BIG_CARD_HERO_POWER_DISPLAY_TIME = 4f;

	// Token: 0x04002B42 RID: 11074
	private const float BIG_CARD_SECRET_DISPLAY_TIME = 4f;

	// Token: 0x04002B43 RID: 11075
	private const float BIG_CARD_POST_TRANSFORM_DISPLAY_TIME = 2f;

	// Token: 0x04002B44 RID: 11076
	private const float BIG_CARD_META_DATA_DISPLAY_TIME_SCALAR = 0.375f;

	// Token: 0x04002B45 RID: 11077
	private const float SPACE_BETWEEN_TILES = 0.15f;

	// Token: 0x04002B46 RID: 11078
	public Texture m_mageSecretTexture;

	// Token: 0x04002B47 RID: 11079
	public Texture m_paladinSecretTexture;

	// Token: 0x04002B48 RID: 11080
	public Texture m_hunterSecretTexture;

	// Token: 0x04002B49 RID: 11081
	public Texture m_FatigueTexture;

	// Token: 0x04002B4A RID: 11082
	public SoundDucker m_SoundDucker;

	// Token: 0x04002B4B RID: 11083
	public Spell m_TransformSpell;

	// Token: 0x04002B4C RID: 11084
	private static HistoryManager s_instance;

	// Token: 0x04002B4D RID: 11085
	private bool m_historyDisabled;

	// Token: 0x04002B4E RID: 11086
	private List<HistoryCard> m_historyTiles = new List<HistoryCard>();

	// Token: 0x04002B4F RID: 11087
	private HistoryCard m_currentlyMousedOverTile;

	// Token: 0x04002B50 RID: 11088
	private float m_sizeOfBigTile;

	// Token: 0x04002B51 RID: 11089
	private List<HistoryManager.TileEntry> m_queuedEntries = new List<HistoryManager.TileEntry>();

	// Token: 0x04002B52 RID: 11090
	private bool m_animatingVignette;

	// Token: 0x04002B53 RID: 11091
	private bool m_animatingDesat;

	// Token: 0x04002B54 RID: 11092
	private Vector3[] m_bigCardPath;

	// Token: 0x04002B55 RID: 11093
	private HistoryManager.BigCardEntry m_pendingBigCardEntry;

	// Token: 0x04002B56 RID: 11094
	private HistoryCard m_currentBigCard;

	// Token: 0x04002B57 RID: 11095
	private bool m_bigCardWaitingForSecret;

	// Token: 0x04002B58 RID: 11096
	private HistoryManager.BigCardTransformState m_bigCardTransformState;

	// Token: 0x04002B59 RID: 11097
	private Spell m_bigCardTransformSpell;

	// Token: 0x020008C3 RID: 2243
	// (Invoke) Token: 0x06005471 RID: 21617
	public delegate void BigCardFinishedCallback();

	// Token: 0x02000912 RID: 2322
	private class BigCardEntry
	{
		// Token: 0x04003D05 RID: 15621
		public HistoryInfo m_info;

		// Token: 0x04003D06 RID: 15622
		public HistoryManager.BigCardFinishedCallback m_finishedCallback;

		// Token: 0x04003D07 RID: 15623
		public bool m_fromMetaData;

		// Token: 0x04003D08 RID: 15624
		public bool m_countered;

		// Token: 0x04003D09 RID: 15625
		public bool m_waitForSecretSpell;
	}

	// Token: 0x02000914 RID: 2324
	private enum BigCardTransformState
	{
		// Token: 0x04003D15 RID: 15637
		INVALID,
		// Token: 0x04003D16 RID: 15638
		PRE_TRANSFORM,
		// Token: 0x04003D17 RID: 15639
		TRANSFORM,
		// Token: 0x04003D18 RID: 15640
		POST_TRANSFORM
	}

	// Token: 0x02000915 RID: 2325
	private class TileEntry
	{
		// Token: 0x0600566A RID: 22122 RVA: 0x0019EF9A File Offset: 0x0019D19A
		public void SetAttacker(Entity attacker)
		{
			this.m_lastAttacker = new HistoryInfo();
			this.m_lastAttacker.m_infoType = HistoryInfoType.ATTACK;
			this.m_lastAttacker.SetOriginalEntity(attacker);
		}

		// Token: 0x0600566B RID: 22123 RVA: 0x0019EFBF File Offset: 0x0019D1BF
		public void SetDefender(Entity defender)
		{
			this.m_lastDefender = new HistoryInfo();
			this.m_lastDefender.SetOriginalEntity(defender);
		}

		// Token: 0x0600566C RID: 22124 RVA: 0x0019EFD8 File Offset: 0x0019D1D8
		public void SetCardPlayed(Entity entity)
		{
			this.m_lastCardPlayed = new HistoryInfo();
			if (entity.IsWeapon())
			{
				this.m_lastCardPlayed.m_infoType = HistoryInfoType.WEAPON_PLAYED;
			}
			else
			{
				this.m_lastCardPlayed.m_infoType = HistoryInfoType.CARD_PLAYED;
			}
			this.m_lastCardPlayed.SetOriginalEntity(entity);
		}

		// Token: 0x0600566D RID: 22125 RVA: 0x0019F024 File Offset: 0x0019D224
		public void SetCardTargeted(Entity entity)
		{
			if (entity == null)
			{
				return;
			}
			this.m_lastCardTargeted = new HistoryInfo();
			this.m_lastCardTargeted.SetOriginalEntity(entity);
		}

		// Token: 0x0600566E RID: 22126 RVA: 0x0019F044 File Offset: 0x0019D244
		public void SetCardTriggered(Entity entity)
		{
			if (entity.IsGame())
			{
				return;
			}
			if (entity.IsPlayer())
			{
				return;
			}
			if (entity.IsHero())
			{
				return;
			}
			this.m_lastCardTriggered = new HistoryInfo();
			this.m_lastCardTriggered.m_infoType = HistoryInfoType.TRIGGER;
			this.m_lastCardTriggered.SetOriginalEntity(entity);
		}

		// Token: 0x0600566F RID: 22127 RVA: 0x0019F098 File Offset: 0x0019D298
		public void SetFatigue()
		{
			this.m_fatigueInfo = new HistoryInfo();
			this.m_fatigueInfo.m_infoType = HistoryInfoType.FATIGUE;
		}

		// Token: 0x06005670 RID: 22128 RVA: 0x0019F0B4 File Offset: 0x0019D2B4
		public bool CanDuplicateAllEntities()
		{
			HistoryInfo sourceInfo = this.GetSourceInfo();
			if (this.ShouldDuplicateEntity(sourceInfo) && !sourceInfo.CanDuplicateEntity())
			{
				return false;
			}
			HistoryInfo targetInfo = this.GetTargetInfo();
			if (this.ShouldDuplicateEntity(targetInfo) && !targetInfo.CanDuplicateEntity())
			{
				return false;
			}
			for (int i = 0; i < this.m_affectedCards.Count; i++)
			{
				HistoryInfo historyInfo = this.m_affectedCards[i];
				if (this.ShouldDuplicateEntity(historyInfo))
				{
					if (!historyInfo.CanDuplicateEntity())
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06005671 RID: 22129 RVA: 0x0019F14C File Offset: 0x0019D34C
		public void DuplicateAllEntities()
		{
			HistoryInfo sourceInfo = this.GetSourceInfo();
			if (this.ShouldDuplicateEntity(sourceInfo))
			{
				sourceInfo.DuplicateEntity();
			}
			HistoryInfo targetInfo = this.GetTargetInfo();
			if (this.ShouldDuplicateEntity(targetInfo))
			{
				targetInfo.DuplicateEntity();
			}
			for (int i = 0; i < this.m_affectedCards.Count; i++)
			{
				HistoryInfo historyInfo = this.m_affectedCards[i];
				if (this.ShouldDuplicateEntity(historyInfo))
				{
					historyInfo.DuplicateEntity();
				}
			}
		}

		// Token: 0x06005672 RID: 22130 RVA: 0x0019F1CB File Offset: 0x0019D3CB
		public bool ShouldDuplicateEntity(HistoryInfo info)
		{
			return info != null && info != this.m_fatigueInfo;
		}

		// Token: 0x06005673 RID: 22131 RVA: 0x0019F1E4 File Offset: 0x0019D3E4
		public HistoryInfo GetSourceInfo()
		{
			if (this.m_lastCardPlayed != null)
			{
				return this.m_lastCardPlayed;
			}
			if (this.m_lastAttacker != null)
			{
				return this.m_lastAttacker;
			}
			if (this.m_lastCardTriggered != null)
			{
				return this.m_lastCardTriggered;
			}
			if (this.m_fatigueInfo != null)
			{
				return this.m_fatigueInfo;
			}
			Debug.LogError("HistoryEntry.GetSourceInfo() - no source info");
			return null;
		}

		// Token: 0x06005674 RID: 22132 RVA: 0x0019F244 File Offset: 0x0019D444
		public HistoryInfo GetTargetInfo()
		{
			if (this.m_lastCardPlayed != null && this.m_lastCardTargeted != null)
			{
				return this.m_lastCardTargeted;
			}
			if (this.m_lastAttacker != null && this.m_lastDefender != null)
			{
				return this.m_lastDefender;
			}
			return null;
		}

		// Token: 0x04003D19 RID: 15641
		public HistoryInfo m_lastAttacker;

		// Token: 0x04003D1A RID: 15642
		public HistoryInfo m_lastDefender;

		// Token: 0x04003D1B RID: 15643
		public HistoryInfo m_lastCardPlayed;

		// Token: 0x04003D1C RID: 15644
		public HistoryInfo m_lastCardTriggered;

		// Token: 0x04003D1D RID: 15645
		public HistoryInfo m_lastCardTargeted;

		// Token: 0x04003D1E RID: 15646
		public List<HistoryInfo> m_affectedCards = new List<HistoryInfo>();

		// Token: 0x04003D1F RID: 15647
		public HistoryInfo m_fatigueInfo;

		// Token: 0x04003D20 RID: 15648
		public bool m_complete;
	}
}
