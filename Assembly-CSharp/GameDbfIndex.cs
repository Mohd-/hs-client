using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class GameDbfIndex
{
	// Token: 0x06001138 RID: 4408 RVA: 0x0004A08B File Offset: 0x0004828B
	public GameDbfIndex()
	{
		this.Initialize();
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x0004A09C File Offset: 0x0004829C
	public void Initialize()
	{
		this.m_cardsByCardId = new Map<string, CardDbfRecord>();
		this.m_allCardIds = new List<string>();
		this.m_allCardDbIds = new List<int>();
		this.m_collectibleCardIds = new List<string>();
		this.m_collectibleCardDbIds = new List<int>();
		this.m_collectibleCardCount = 0;
		this.m_fixedRewardsByAction = new Map<int, List<FixedRewardMapDbfRecord>>();
		this.m_fixedActionRecordsByType = new Map<FixedActionType, List<FixedRewardActionDbfRecord>>();
		this.m_subsetsReferencedByRuleId = new Map<int, List<int>>();
		this.m_subsetCards = new Map<int, HashSet<string>>();
		this.m_rulesByDeckRulesetId = new Map<int, HashSet<int>>();
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x0004A120 File Offset: 0x00048320
	public void OnCardAdded(CardDbfRecord cardRecord)
	{
		int id = cardRecord.ID;
		bool isCollectible = cardRecord.IsCollectible;
		string noteMiniGuid = cardRecord.NoteMiniGuid;
		this.m_cardsByCardId[noteMiniGuid] = cardRecord;
		this.m_allCardDbIds.Add(id);
		this.m_allCardIds.Add(noteMiniGuid);
		if (isCollectible)
		{
			this.m_collectibleCardCount++;
			this.m_collectibleCardIds.Add(noteMiniGuid);
			this.m_collectibleCardDbIds.Add(id);
		}
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x0004A194 File Offset: 0x00048394
	public void OnCardRemoved(List<CardDbfRecord> removedRecords)
	{
		HashSet<int> removedCardDbIds = new HashSet<int>();
		HashSet<string> removedCardIds = new HashSet<string>();
		foreach (CardDbfRecord cardDbfRecord in removedRecords)
		{
			removedCardDbIds.Add(cardDbfRecord.ID);
			if (cardDbfRecord.NoteMiniGuid != null)
			{
				removedCardIds.Add(cardDbfRecord.NoteMiniGuid);
				this.m_cardsByCardId.Remove(cardDbfRecord.NoteMiniGuid);
			}
		}
		if (removedCardDbIds.Count > 0)
		{
			this.m_allCardDbIds.RemoveAll((int cardDbId) => removedCardDbIds.Contains(cardDbId));
			this.m_collectibleCardDbIds.RemoveAll((int cardDbId) => this.m_collectibleCardDbIds.Contains(cardDbId));
		}
		if (removedCardIds.Count > 0)
		{
			this.m_allCardIds.RemoveAll((string cardId) => removedCardIds.Contains(cardId));
			this.m_collectibleCardIds.RemoveAll((string cardId) => removedCardIds.Contains(cardId));
		}
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x0004A2C8 File Offset: 0x000484C8
	public void OnFixedRewardMapAdded(FixedRewardMapDbfRecord record)
	{
		int actionId = record.ActionId;
		List<FixedRewardMapDbfRecord> list;
		if (!this.m_fixedRewardsByAction.TryGetValue(actionId, out list))
		{
			list = new List<FixedRewardMapDbfRecord>();
			this.m_fixedRewardsByAction.Add(actionId, list);
		}
		list.Add(record);
	}

	// Token: 0x0600113D RID: 4413 RVA: 0x0004A30C File Offset: 0x0004850C
	public void OnFixedRewardMapRemoved(List<FixedRewardMapDbfRecord> removedRecords)
	{
		HashSet<int> removedIds = new HashSet<int>(Enumerable.Select<FixedRewardMapDbfRecord, int>(removedRecords, (FixedRewardMapDbfRecord r) => r.ID));
		HashSet<int> hashSet = new HashSet<int>(Enumerable.Select<FixedRewardMapDbfRecord, int>(removedRecords, (FixedRewardMapDbfRecord r) => r.ActionId));
		foreach (int key in hashSet)
		{
			List<FixedRewardMapDbfRecord> list;
			if (this.m_fixedRewardsByAction.TryGetValue(key, out list))
			{
				list.RemoveAll((FixedRewardMapDbfRecord r) => removedIds.Contains(r.ID));
			}
		}
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x0004A3E0 File Offset: 0x000485E0
	public void OnFixedRewardActionAdded(FixedRewardActionDbfRecord record)
	{
		string type = record.Type;
		FixedActionType @enum;
		try
		{
			@enum = EnumUtils.GetEnum<FixedActionType>(type);
		}
		catch
		{
			Debug.LogErrorFormat("Error parsing FixedRewardAction.Type, type did not match a FixedRewardType: {0}", new object[]
			{
				type
			});
			return;
		}
		List<FixedRewardActionDbfRecord> list;
		if (!this.m_fixedActionRecordsByType.TryGetValue(@enum, out list))
		{
			list = new List<FixedRewardActionDbfRecord>();
			this.m_fixedActionRecordsByType.Add(@enum, list);
		}
		list.Add(record);
	}

	// Token: 0x0600113F RID: 4415 RVA: 0x0004A45C File Offset: 0x0004865C
	public void OnFixedRewardActionRemoved(List<FixedRewardActionDbfRecord> removedRecords)
	{
		HashSet<int> removedIds = new HashSet<int>(Enumerable.Select<FixedRewardActionDbfRecord, int>(removedRecords, (FixedRewardActionDbfRecord r) => r.ID));
		HashSet<FixedActionType> hashSet = null;
		try
		{
			hashSet = new HashSet<FixedActionType>(Enumerable.Select<FixedRewardActionDbfRecord, FixedActionType>(removedRecords, (FixedRewardActionDbfRecord r) => EnumUtils.GetEnum<FixedActionType>(r.Type)));
		}
		catch
		{
			string text = "Error parsing FixedRewardAction.Type, type did not match a FixedRewardType: {0}";
			object[] array = new object[1];
			array[0] = string.Join(", ", Enumerable.ToArray<string>(Enumerable.Select<FixedRewardActionDbfRecord, string>(removedRecords, (FixedRewardActionDbfRecord r) => r.Type)));
			Debug.LogErrorFormat(text, array);
			hashSet = new HashSet<FixedActionType>();
		}
		foreach (FixedActionType key in hashSet)
		{
			List<FixedRewardActionDbfRecord> list;
			if (this.m_fixedActionRecordsByType.TryGetValue(key, out list))
			{
				list.RemoveAll((FixedRewardActionDbfRecord r) => removedIds.Contains(r.ID));
			}
		}
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x0004A594 File Offset: 0x00048794
	public void OnDeckRulesetRuleSubsetAdded(DeckRulesetRuleSubsetDbfRecord record)
	{
		int deckRulesetRuleId = record.DeckRulesetRuleId;
		int subsetId = record.SubsetId;
		List<int> list;
		if (!this.m_subsetsReferencedByRuleId.TryGetValue(deckRulesetRuleId, out list))
		{
			list = new List<int>();
			this.m_subsetsReferencedByRuleId[deckRulesetRuleId] = list;
		}
		list.Add(subsetId);
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x0004A5DC File Offset: 0x000487DC
	public void OnDeckRulesetRuleSubsetRemoved(List<DeckRulesetRuleSubsetDbfRecord> removedRecords)
	{
		DeckRulesetRuleSubsetDbfRecord rec;
		foreach (DeckRulesetRuleSubsetDbfRecord rec2 in removedRecords)
		{
			rec = rec2;
			List<int> list;
			if (this.m_subsetsReferencedByRuleId.TryGetValue(rec.DeckRulesetRuleId, out list))
			{
				list.RemoveAll((int subsetId) => subsetId == rec.SubsetId);
			}
		}
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x0004A668 File Offset: 0x00048868
	public void OnSubsetCardAdded(SubsetCardDbfRecord record)
	{
		int subsetId = record.SubsetId;
		int cardId = record.CardId;
		CardDbfRecord record2 = GameDbf.Card.GetRecord(cardId);
		if (record2 == null)
		{
			return;
		}
		HashSet<string> hashSet;
		if (!this.m_subsetCards.TryGetValue(subsetId, out hashSet))
		{
			hashSet = new HashSet<string>();
			this.m_subsetCards[subsetId] = hashSet;
		}
		hashSet.Add(record2.NoteMiniGuid);
	}

	// Token: 0x06001143 RID: 4419 RVA: 0x0004A6CC File Offset: 0x000488CC
	public void OnSubsetCardRemoved(List<SubsetCardDbfRecord> removedRecords)
	{
		foreach (SubsetCardDbfRecord subsetCardDbfRecord in removedRecords)
		{
			HashSet<string> hashSet;
			if (this.m_subsetCards.TryGetValue(subsetCardDbfRecord.SubsetId, out hashSet) && hashSet != null)
			{
				CardDbfRecord record = GameDbf.Card.GetRecord(subsetCardDbfRecord.CardId);
				if (record != null && record.NoteMiniGuid != null)
				{
					hashSet.Remove(record.NoteMiniGuid);
				}
			}
		}
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x0004A768 File Offset: 0x00048968
	public void OnDeckRulesetRuleAdded(DeckRulesetRuleDbfRecord record)
	{
		HashSet<int> hashSet;
		if (!this.m_rulesByDeckRulesetId.TryGetValue(record.DeckRulesetId, out hashSet))
		{
			hashSet = new HashSet<int>();
			this.m_rulesByDeckRulesetId[record.DeckRulesetId] = hashSet;
		}
		hashSet.Add(record.ID);
	}

	// Token: 0x06001145 RID: 4421 RVA: 0x0004A7B4 File Offset: 0x000489B4
	public void OnDeckRulesetRuleRemoved(List<DeckRulesetRuleDbfRecord> removedRecords)
	{
		foreach (DeckRulesetRuleDbfRecord deckRulesetRuleDbfRecord in removedRecords)
		{
			HashSet<int> hashSet;
			if (this.m_rulesByDeckRulesetId.TryGetValue(deckRulesetRuleDbfRecord.DeckRulesetId, out hashSet))
			{
				hashSet.Remove(deckRulesetRuleDbfRecord.ID);
			}
		}
	}

	// Token: 0x06001146 RID: 4422 RVA: 0x0004A828 File Offset: 0x00048A28
	public CardDbfRecord GetCardRecord(string cardId)
	{
		CardDbfRecord result = null;
		this.m_cardsByCardId.TryGetValue(cardId, out result);
		return result;
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x0004A847 File Offset: 0x00048A47
	public int GetCollectibleCardCount()
	{
		return this.m_collectibleCardCount;
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x0004A84F File Offset: 0x00048A4F
	public List<string> GetAllCardIds()
	{
		return this.m_allCardIds;
	}

	// Token: 0x06001149 RID: 4425 RVA: 0x0004A857 File Offset: 0x00048A57
	public List<int> GetAllCardDbIds()
	{
		return this.m_allCardDbIds;
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x0004A85F File Offset: 0x00048A5F
	public List<string> GetCollectibleCardIds()
	{
		return this.m_collectibleCardIds;
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x0004A867 File Offset: 0x00048A67
	public List<int> GetCollectibleCardDbIds()
	{
		return this.m_collectibleCardDbIds;
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x0004A870 File Offset: 0x00048A70
	public List<FixedRewardMapDbfRecord> GetFixedRewardMapRecordsForAction(int actionId)
	{
		List<FixedRewardMapDbfRecord> result = null;
		if (!this.m_fixedRewardsByAction.TryGetValue(actionId, out result))
		{
			result = new List<FixedRewardMapDbfRecord>();
		}
		return result;
	}

	// Token: 0x0600114D RID: 4429 RVA: 0x0004A89C File Offset: 0x00048A9C
	public List<FixedRewardActionDbfRecord> GetFixedActionRecordsForType(FixedActionType type)
	{
		List<FixedRewardActionDbfRecord> result = null;
		if (!this.m_fixedActionRecordsByType.TryGetValue(type, out result))
		{
			result = new List<FixedRewardActionDbfRecord>();
		}
		return result;
	}

	// Token: 0x0600114E RID: 4430 RVA: 0x0004A8C8 File Offset: 0x00048AC8
	public List<HashSet<string>> GetSubsetsForRule(int ruleId)
	{
		List<HashSet<string>> list = new List<HashSet<string>>();
		List<int> list2;
		if (this.m_subsetsReferencedByRuleId.TryGetValue(ruleId, out list2))
		{
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(this.GetSubsetById(list2[i]));
			}
		}
		return list;
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x0004A91C File Offset: 0x00048B1C
	public DeckRulesetRuleDbfRecord[] GetRulesForDeckRuleset(int deckRulesetId)
	{
		HashSet<int> hashSet;
		if (!this.m_rulesByDeckRulesetId.TryGetValue(deckRulesetId, out hashSet))
		{
			hashSet = new HashSet<int>();
		}
		IEnumerable<DeckRulesetRuleDbfRecord> enumerable = Enumerable.Select(Enumerable.Where(Enumerable.Select(hashSet, (int ruleId) => new
		{
			ruleId = ruleId,
			ruleDbf = GameDbf.DeckRulesetRule.GetRecord(ruleId)
		}), <>__TranspIdent1 => <>__TranspIdent1.ruleDbf != null), <>__TranspIdent1 => <>__TranspIdent1.ruleDbf);
		return Enumerable.ToArray<DeckRulesetRuleDbfRecord>(enumerable);
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x0004A9B4 File Offset: 0x00048BB4
	public HashSet<string> GetSubsetById(int id)
	{
		HashSet<string> result = null;
		if (!this.m_subsetCards.TryGetValue(id, out result))
		{
			result = new HashSet<string>();
		}
		return result;
	}

	// Token: 0x0400092E RID: 2350
	private Map<string, CardDbfRecord> m_cardsByCardId;

	// Token: 0x0400092F RID: 2351
	private List<string> m_allCardIds;

	// Token: 0x04000930 RID: 2352
	private List<int> m_allCardDbIds;

	// Token: 0x04000931 RID: 2353
	private List<string> m_collectibleCardIds;

	// Token: 0x04000932 RID: 2354
	private List<int> m_collectibleCardDbIds;

	// Token: 0x04000933 RID: 2355
	private int m_collectibleCardCount;

	// Token: 0x04000934 RID: 2356
	private Map<int, List<FixedRewardMapDbfRecord>> m_fixedRewardsByAction;

	// Token: 0x04000935 RID: 2357
	private Map<FixedActionType, List<FixedRewardActionDbfRecord>> m_fixedActionRecordsByType;

	// Token: 0x04000936 RID: 2358
	private Map<int, List<int>> m_subsetsReferencedByRuleId;

	// Token: 0x04000937 RID: 2359
	private Map<int, HashSet<string>> m_subsetCards;

	// Token: 0x04000938 RID: 2360
	private Map<int, HashSet<int>> m_rulesByDeckRulesetId;
}
