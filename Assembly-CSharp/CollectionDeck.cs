using System;
using System.Collections.Generic;
using System.Text;
using PegasusShared;
using UnityEngine;

// Token: 0x02000339 RID: 825
public class CollectionDeck
{
	// Token: 0x06002AFE RID: 11006 RVA: 0x000D594C File Offset: 0x000D3B4C
	public override string ToString()
	{
		return string.Format("Deck [id={0} name=\"{1}\" heroCardId={2} heroCardFlair={3} cardBackId={4} cardBackOverridden={5} heroOverridden={6} slotCount={7} needsName={8} sortOrder={9}]", new object[]
		{
			this.ID,
			this.Name,
			this.HeroCardID,
			this.HeroPremium,
			this.CardBackID,
			this.CardBackOverridden,
			this.HeroOverridden,
			this.GetSlotCount(),
			this.NeedsName,
			this.SortOrder
		});
	}

	// Token: 0x17000386 RID: 902
	// (get) Token: 0x06002AFF RID: 11007 RVA: 0x000D59ED File Offset: 0x000D3BED
	// (set) Token: 0x06002B00 RID: 11008 RVA: 0x000D59F5 File Offset: 0x000D3BF5
	public string Name
	{
		get
		{
			return this.m_name;
		}
		set
		{
			if (value == null)
			{
				Debug.LogError(string.Format("CollectionDeck.SetName() - null name given for deck {0}", this));
				return;
			}
			if (value.Equals(this.m_name, 3))
			{
				return;
			}
			this.m_name = value;
		}
	}

	// Token: 0x17000387 RID: 903
	// (get) Token: 0x06002B01 RID: 11009 RVA: 0x000D5A28 File Offset: 0x000D3C28
	// (set) Token: 0x06002B02 RID: 11010 RVA: 0x000D5A3C File Offset: 0x000D3C3C
	public bool IsWild
	{
		get
		{
			return GameUtils.IsAnythingRotated() && this.m_isWild;
		}
		set
		{
			this.m_isWild = value;
		}
	}

	// Token: 0x17000388 RID: 904
	// (get) Token: 0x06002B03 RID: 11011 RVA: 0x000D5A48 File Offset: 0x000D3C48
	public bool IsTourneyValid
	{
		get
		{
			if (!this.m_netContentsLoaded)
			{
				return false;
			}
			DeckRuleset ruleset = this.GetRuleset();
			return ruleset.IsDeckValid(this);
		}
	}

	// Token: 0x06002B04 RID: 11012 RVA: 0x000D5A72 File Offset: 0x000D3C72
	public void MarkNetworkContentsLoaded()
	{
		this.m_netContentsLoaded = true;
	}

	// Token: 0x06002B05 RID: 11013 RVA: 0x000D5A7B File Offset: 0x000D3C7B
	public bool NetworkContentsLoaded()
	{
		return this.m_netContentsLoaded;
	}

	// Token: 0x06002B06 RID: 11014 RVA: 0x000D5A83 File Offset: 0x000D3C83
	public void MarkBeingDeleted()
	{
		this.m_isBeingDeleted = true;
	}

	// Token: 0x06002B07 RID: 11015 RVA: 0x000D5A8C File Offset: 0x000D3C8C
	public bool IsBeingDeleted()
	{
		return this.m_isBeingDeleted;
	}

	// Token: 0x06002B08 RID: 11016 RVA: 0x000D5A94 File Offset: 0x000D3C94
	public bool IsSavingChanges()
	{
		return this.m_isSavingNameChanges || this.m_isSavingContentChanges;
	}

	// Token: 0x06002B09 RID: 11017 RVA: 0x000D5AAC File Offset: 0x000D3CAC
	public bool IsBasicDeck()
	{
		if (this.IsWild)
		{
			return false;
		}
		if (this.SourceType != 3)
		{
			return false;
		}
		foreach (CollectionDeckSlot collectionDeckSlot in this.m_slots)
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(collectionDeckSlot.CardID);
			if (entityDef.GetCardSet() != TAG_CARD_SET.CORE)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002B0A RID: 11018 RVA: 0x000D5B44 File Offset: 0x000D3D44
	public int GetTotalCardCount()
	{
		int num = 0;
		foreach (CollectionDeckSlot collectionDeckSlot in this.m_slots)
		{
			num += collectionDeckSlot.Count;
		}
		return num;
	}

	// Token: 0x06002B0B RID: 11019 RVA: 0x000D5BA4 File Offset: 0x000D3DA4
	public int GetTotalOwnedCardCount()
	{
		if (!this.IsCardOwnershipUnique())
		{
			return this.GetTotalCardCount();
		}
		int num = 0;
		foreach (CollectionDeckSlot collectionDeckSlot in this.m_slots)
		{
			if (collectionDeckSlot.Owned)
			{
				num += collectionDeckSlot.Count;
			}
		}
		return num;
	}

	// Token: 0x06002B0C RID: 11020 RVA: 0x000D5C20 File Offset: 0x000D3E20
	public int GetTotalValidCardCount()
	{
		int num = 0;
		foreach (CollectionDeckSlot collectionDeckSlot in this.m_slots)
		{
			if (this.IsValidSlot(collectionDeckSlot))
			{
				num += collectionDeckSlot.Count;
			}
		}
		return num;
	}

	// Token: 0x06002B0D RID: 11021 RVA: 0x000D5C8C File Offset: 0x000D3E8C
	public int GetTotalInvalidCardCount()
	{
		int num = 0;
		foreach (CollectionDeckSlot collectionDeckSlot in this.m_slots)
		{
			if (!this.IsValidSlot(collectionDeckSlot))
			{
				num += collectionDeckSlot.Count;
			}
		}
		return num;
	}

	// Token: 0x06002B0E RID: 11022 RVA: 0x000D5CF8 File Offset: 0x000D3EF8
	public int GetTotalUnownedCardCount()
	{
		int num = 0;
		foreach (CollectionDeckSlot collectionDeckSlot in this.m_slots)
		{
			if (!collectionDeckSlot.Owned)
			{
				num += collectionDeckSlot.Count;
			}
		}
		return num;
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x000D5D64 File Offset: 0x000D3F64
	public List<CollectionDeckSlot> GetSlots()
	{
		return this.m_slots;
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x000D5D6C File Offset: 0x000D3F6C
	public int GetSlotCount()
	{
		return this.m_slots.Count;
	}

	// Token: 0x06002B11 RID: 11025 RVA: 0x000D5D7C File Offset: 0x000D3F7C
	public bool IsValidSlot(CollectionDeckSlot slot)
	{
		if (this.IsWild)
		{
			return slot.Owned;
		}
		return slot.Owned && !GameUtils.IsCardRotated(slot.CardID);
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x000D5DB8 File Offset: 0x000D3FB8
	public bool HasReplaceableSlot()
	{
		for (int i = 0; i < this.m_slots.Count; i++)
		{
			if (!this.IsValidSlot(this.m_slots[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002B13 RID: 11027 RVA: 0x000D5DFB File Offset: 0x000D3FFB
	public CollectionDeckSlot GetSlotByIndex(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= this.GetSlotCount())
		{
			return null;
		}
		return this.m_slots[slotIndex];
	}

	// Token: 0x06002B14 RID: 11028 RVA: 0x000D5E20 File Offset: 0x000D4020
	public CollectionDeckSlot GetSlotByUID(long uid)
	{
		return this.m_slots.Find((CollectionDeckSlot slot) => slot.GetUID(this.Type) == uid);
	}

	// Token: 0x06002B15 RID: 11029 RVA: 0x000D5E58 File Offset: 0x000D4058
	public DeckRuleset GetRuleset()
	{
		DeckRuleset deckRuleset = null;
		switch (this.Type)
		{
		case 1:
		case 5:
			deckRuleset = ((!this.IsWild) ? DeckRuleset.GetStandardRuleset() : DeckRuleset.GetWildRuleset());
			break;
		case 6:
			deckRuleset = TavernBrawlManager.Get().GetDeckRuleset();
			break;
		}
		if (deckRuleset == null)
		{
			deckRuleset = DeckRuleset.GetWildRuleset();
		}
		return deckRuleset;
	}

	// Token: 0x06002B16 RID: 11030 RVA: 0x000D5ED0 File Offset: 0x000D40D0
	public bool IsValidForFormat(bool isFormatWild)
	{
		return !this.IsWild || isFormatWild;
	}

	// Token: 0x06002B17 RID: 11031 RVA: 0x000D5EE1 File Offset: 0x000D40E1
	public bool InsertSlotByDefaultSort(CollectionDeckSlot slot)
	{
		return this.InsertSlot(this.GetInsertionIdxByDefaultSort(slot), slot);
	}

	// Token: 0x06002B18 RID: 11032 RVA: 0x000D5EF4 File Offset: 0x000D40F4
	public void CopyFrom(CollectionDeck otherDeck)
	{
		this.ID = otherDeck.ID;
		this.Type = otherDeck.Type;
		this.m_name = otherDeck.m_name;
		this.HeroCardID = otherDeck.HeroCardID;
		this.HeroPremium = otherDeck.HeroPremium;
		this.HeroOverridden = otherDeck.HeroOverridden;
		this.CardBackID = otherDeck.CardBackID;
		this.CardBackOverridden = otherDeck.CardBackOverridden;
		this.NeedsName = otherDeck.NeedsName;
		this.SeasonId = otherDeck.SeasonId;
		this.IsWild = otherDeck.IsWild;
		this.SortOrder = otherDeck.SortOrder;
		this.SourceType = otherDeck.SourceType;
		this.m_slots.Clear();
		for (int i = 0; i < otherDeck.GetSlotCount(); i++)
		{
			CollectionDeckSlot slotByIndex = otherDeck.GetSlotByIndex(i);
			CollectionDeckSlot collectionDeckSlot = new CollectionDeckSlot();
			collectionDeckSlot.CopyFrom(slotByIndex);
			this.m_slots.Add(collectionDeckSlot);
		}
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x000D5FE0 File Offset: 0x000D41E0
	public void CopyContents(CollectionDeck otherDeck)
	{
		this.HeroCardID = otherDeck.HeroCardID;
		this.HeroPremium = otherDeck.HeroPremium;
		this.m_slots.Clear();
		for (int i = 0; i < otherDeck.GetSlotCount(); i++)
		{
			CollectionDeckSlot slotByIndex = otherDeck.GetSlotByIndex(i);
			for (int j = 0; j < slotByIndex.Count; j++)
			{
				this.AddCard(slotByIndex.CardID, slotByIndex.Premium, false);
			}
		}
	}

	// Token: 0x06002B1A RID: 11034 RVA: 0x000D605C File Offset: 0x000D425C
	public void FillFromTemplateDeck(CollectionManager.TemplateDeck tplDeck)
	{
		this.ClearSlotContents();
		this.Name = tplDeck.m_title;
		foreach (KeyValuePair<string, int> keyValuePair in tplDeck.m_cardIds)
		{
			int num = 0;
			int num2 = 0;
			CollectionManager.Get().GetOwnedCardCount(keyValuePair.Key, out num2, out num);
			int value = keyValuePair.Value;
			int num3 = Mathf.Min(num, value);
			int num4 = value - num3;
			for (int i = 0; i < num3; i++)
			{
				this.AddCard(keyValuePair.Key, TAG_PREMIUM.GOLDEN, false);
			}
			for (int j = 0; j < num4; j++)
			{
				this.AddCard(keyValuePair.Key, TAG_PREMIUM.NORMAL, false);
			}
		}
	}

	// Token: 0x06002B1B RID: 11035 RVA: 0x000D6144 File Offset: 0x000D4344
	public int GetUnownedCardIdCount(string cardID)
	{
		int num = 0;
		foreach (CollectionDeckSlot collectionDeckSlot in this.m_slots)
		{
			if (collectionDeckSlot.CardID.Equals(cardID))
			{
				if (!collectionDeckSlot.Owned)
				{
					num += collectionDeckSlot.Count;
				}
			}
		}
		return num;
	}

	// Token: 0x06002B1C RID: 11036 RVA: 0x000D61CC File Offset: 0x000D43CC
	public int GetInvalidCardIdCount(string cardID)
	{
		int num = 0;
		foreach (CollectionDeckSlot collectionDeckSlot in this.m_slots)
		{
			if (collectionDeckSlot.CardID.Equals(cardID))
			{
				if (!this.IsValidSlot(collectionDeckSlot))
				{
					num += collectionDeckSlot.Count;
				}
			}
		}
		return num;
	}

	// Token: 0x06002B1D RID: 11037 RVA: 0x000D6254 File Offset: 0x000D4454
	public int GetCardIdCount(string cardID, bool includeUnowned = true)
	{
		int num = 0;
		foreach (CollectionDeckSlot collectionDeckSlot in this.m_slots)
		{
			if (collectionDeckSlot.CardID.Equals(cardID))
			{
				if (includeUnowned || collectionDeckSlot.Owned)
				{
					num += collectionDeckSlot.Count;
				}
			}
		}
		return num;
	}

	// Token: 0x06002B1E RID: 11038 RVA: 0x000D62E0 File Offset: 0x000D44E0
	public int GetCardCount(string cardID, TAG_PREMIUM type)
	{
		CollectionDeckSlot collectionDeckSlot = this.FindSlotByCardId(cardID, type);
		return (collectionDeckSlot != null) ? collectionDeckSlot.Count : 0;
	}

	// Token: 0x06002B1F RID: 11039 RVA: 0x000D6308 File Offset: 0x000D4508
	public int GetValidCardCount(string cardID, TAG_PREMIUM type, bool valid = true)
	{
		CollectionDeckSlot collectionDeckSlot = this.FindValidSlotByCardId(cardID, type, valid);
		return (collectionDeckSlot != null) ? collectionDeckSlot.Count : 0;
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x000D6334 File Offset: 0x000D4534
	public int GetOwnedCardCount(string cardID, TAG_PREMIUM type, bool owned = true)
	{
		CollectionDeckSlot collectionDeckSlot = this.FindOwnedSlotByCardId(cardID, type, owned);
		return (collectionDeckSlot != null) ? collectionDeckSlot.Count : 0;
	}

	// Token: 0x06002B21 RID: 11041 RVA: 0x000D6360 File Offset: 0x000D4560
	public int GetCardCountInSet(HashSet<string> set, bool isNot)
	{
		int num = 0;
		for (int i = 0; i < this.m_slots.Count; i++)
		{
			CollectionDeckSlot collectionDeckSlot = this.m_slots[i];
			if (set.Contains(collectionDeckSlot.CardID) == !isNot)
			{
				num += collectionDeckSlot.Count;
			}
		}
		return num;
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x000D63B8 File Offset: 0x000D45B8
	public int CountFilterMisses(DeckRuleset deckRuleset)
	{
		int num = 0;
		for (int i = 0; i < this.m_slots.Count; i++)
		{
			CollectionDeckSlot collectionDeckSlot = this.m_slots[i];
			EntityDef entityDef = DefLoader.Get().GetEntityDef(collectionDeckSlot.CardID);
			if (!deckRuleset.Filter(entityDef))
			{
				num += collectionDeckSlot.Count;
			}
		}
		return num;
	}

	// Token: 0x06002B23 RID: 11043 RVA: 0x000D6418 File Offset: 0x000D4618
	public List<CollectionDeckSlot> GetFilterMisses(DeckRuleset deckRuleset)
	{
		List<CollectionDeckSlot> list = new List<CollectionDeckSlot>();
		for (int i = 0; i < this.m_slots.Count; i++)
		{
			CollectionDeckSlot collectionDeckSlot = this.m_slots[i];
			EntityDef entityDef = DefLoader.Get().GetEntityDef(collectionDeckSlot.CardID);
			if (!deckRuleset.Filter(entityDef))
			{
				list.Add(collectionDeckSlot);
			}
		}
		return list;
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x000D6479 File Offset: 0x000D4679
	public void ClearSlotContents()
	{
		this.m_slots.Clear();
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x000D6488 File Offset: 0x000D4688
	public bool CanAddOwnedCard(string cardID, TAG_PREMIUM premium)
	{
		int num = 0;
		int num2 = 0;
		CollectionManager.Get().GetOwnedCardCount(cardID, out num, out num2);
		int num3 = (premium != TAG_PREMIUM.NORMAL) ? num2 : num;
		int ownedCardCount = this.GetOwnedCardCount(cardID, premium, true);
		return num3 > ownedCardCount;
	}

	// Token: 0x06002B26 RID: 11046 RVA: 0x000D64C4 File Offset: 0x000D46C4
	public bool AddCard(EntityDef cardEntityDef, TAG_PREMIUM premium, bool exceedMax = false)
	{
		return this.AddCard(cardEntityDef.GetCardId(), premium, exceedMax);
	}

	// Token: 0x06002B27 RID: 11047 RVA: 0x000D64D4 File Offset: 0x000D46D4
	public bool AddCard(string cardID, TAG_PREMIUM premium, bool exceedMax = false)
	{
		if (!exceedMax && !this.CanInsertCard(cardID, premium))
		{
			return false;
		}
		bool flag = this.CanAddOwnedCard(cardID, premium);
		CollectionDeckSlot collectionDeckSlot;
		if (!flag)
		{
			collectionDeckSlot = this.FindOwnedSlotByCardId(cardID, premium, flag);
		}
		else
		{
			collectionDeckSlot = this.FindValidSlotByCardId(cardID, premium, true);
		}
		if (collectionDeckSlot != null)
		{
			collectionDeckSlot.Count++;
			return true;
		}
		collectionDeckSlot = new CollectionDeckSlot
		{
			CardID = cardID,
			Count = 1,
			Premium = premium,
			Owned = flag
		};
		return this.InsertSlotByDefaultSort(collectionDeckSlot);
	}

	// Token: 0x06002B28 RID: 11048 RVA: 0x000D6568 File Offset: 0x000D4768
	public bool RemoveCard(string cardID, TAG_PREMIUM premium, bool valid = true, bool removeAllCopies = false)
	{
		CollectionDeckSlot collectionDeckSlot = this.FindValidSlotByCardId(cardID, premium, valid);
		if (collectionDeckSlot == null)
		{
			return false;
		}
		if (removeAllCopies)
		{
			collectionDeckSlot.Count = 0;
		}
		else
		{
			collectionDeckSlot.Count--;
		}
		return true;
	}

	// Token: 0x06002B29 RID: 11049 RVA: 0x000D65A9 File Offset: 0x000D47A9
	public void OnContentChangesComplete()
	{
		this.m_isSavingContentChanges = false;
	}

	// Token: 0x06002B2A RID: 11050 RVA: 0x000D65B2 File Offset: 0x000D47B2
	public void OnNameChangeComplete()
	{
		this.m_isSavingNameChanges = false;
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x000D65BC File Offset: 0x000D47BC
	public void SendChanges()
	{
		CollectionDeck baseDeck = CollectionManager.Get().GetBaseDeck(this.ID);
		if (this == baseDeck)
		{
			Debug.LogError(string.Format("CollectionDeck.Send() - {0} is a base deck. You cannot send a base deck to the network.", baseDeck));
			return;
		}
		string text;
		this.GenerateNameDiff(baseDeck, out text);
		List<Network.CardUserData> list = this.GenerateContentChanges(baseDeck);
		int newHeroAssetID;
		TAG_PREMIUM newHeroCardPremium;
		bool flag = this.GenerateHeroDiff(baseDeck, out newHeroAssetID, out newHeroCardPremium);
		int newCardBackID;
		bool flag2 = this.GenerateCardBackDiff(baseDeck, out newCardBackID);
		bool flag3 = baseDeck.IsWild != this.IsWild;
		Network network = Network.Get();
		if (text != null)
		{
			this.m_isSavingNameChanges = true;
			network.RenameDeck(this.ID, text);
		}
		if (list.Count > 0 || flag || flag2 || flag3)
		{
			this.m_isSavingContentChanges = true;
			network.SetDeckContents(this.ID, list, newHeroAssetID, newHeroCardPremium, newCardBackID, this.IsWild);
		}
	}

	// Token: 0x06002B2C RID: 11052 RVA: 0x000D6694 File Offset: 0x000D4894
	private bool CanInsertCard(string cardID, TAG_PREMIUM premium)
	{
		if (this.Type == 4)
		{
			return true;
		}
		DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset();
		EntityDef entityDef = DefLoader.Get().GetEntityDef(cardID);
		if (deckRuleset == null)
		{
			return true;
		}
		List<DeckRule.RuleType> list = new List<DeckRule.RuleType>();
		list.Add(DeckRule.RuleType.PLAYER_OWNS_EACH_COPY);
		string text;
		DeckRule deckRule;
		return deckRuleset.CanAddToDeck(entityDef, premium, this, out text, out deckRule, list);
	}

	// Token: 0x06002B2D RID: 11053 RVA: 0x000D66EC File Offset: 0x000D48EC
	private bool InsertSlot(int slotIndex, CollectionDeckSlot slot)
	{
		if (slotIndex < 0 || slotIndex > this.GetSlotCount())
		{
			Log.Rachelle.Print(string.Format("CollectionDeck.InsertSlot(): inserting slot {0} failed; invalid slot index {1}.", slot, slotIndex), new object[0]);
			return false;
		}
		long uid = slot.GetUID(this.Type);
		CollectionDeckSlot slotByUID = this.GetSlotByUID(uid);
		if (slotByUID != null)
		{
			Debug.LogWarningFormat("CollectionDeck.InsertSlot: slot with uid={0} already exists in deckId={1} cardId={2} cardDbId={3} premium={4} owned={5} existingCount={6} slotIndex={7}", new object[]
			{
				uid,
				this.ID,
				slot.CardID,
				GameUtils.TranslateCardIdToDbId(slot.CardID),
				slot.Premium,
				slot.Owned,
				slotByUID.Count,
				slotIndex
			});
			return false;
		}
		slot.OnSlotEmptied = (CollectionDeckSlot.DelOnSlotEmptied)Delegate.Combine(slot.OnSlotEmptied, new CollectionDeckSlot.DelOnSlotEmptied(this.OnSlotEmptied));
		slot.Index = slotIndex;
		this.m_slots.Insert(slotIndex, slot);
		this.UpdateSlotIndices(slotIndex, this.GetSlotCount() - 1);
		return true;
	}

	// Token: 0x06002B2E RID: 11054 RVA: 0x000D6808 File Offset: 0x000D4A08
	public void ForceRemoveSlot(CollectionDeckSlot slot)
	{
		this.RemoveSlot(slot);
	}

	// Token: 0x06002B2F RID: 11055 RVA: 0x000D6814 File Offset: 0x000D4A14
	private void RemoveSlot(CollectionDeckSlot slot)
	{
		slot.OnSlotEmptied = (CollectionDeckSlot.DelOnSlotEmptied)Delegate.Remove(slot.OnSlotEmptied, new CollectionDeckSlot.DelOnSlotEmptied(this.OnSlotEmptied));
		int index = slot.Index;
		this.m_slots.RemoveAt(index);
		this.UpdateSlotIndices(index, this.GetSlotCount() - 1);
	}

	// Token: 0x06002B30 RID: 11056 RVA: 0x000D6868 File Offset: 0x000D4A68
	private void OnSlotEmptied(CollectionDeckSlot slot)
	{
		if (this.GetSlotByUID(slot.GetUID(this.Type)) == null)
		{
			Log.Rachelle.Print(string.Format("CollectionDeck.OnSlotCountUpdated(): Trying to remove slot {0}, but it does not exist in deck {1}", slot, this), new object[0]);
			return;
		}
		this.RemoveSlot(slot);
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x000D68B4 File Offset: 0x000D4AB4
	private void UpdateSlotIndices(int indexA, int indexB)
	{
		if (this.GetSlotCount() == 0)
		{
			return;
		}
		int num;
		int num2;
		if (indexA < indexB)
		{
			num = indexA;
			num2 = indexB;
		}
		else
		{
			num = indexB;
			num2 = indexA;
		}
		num = Math.Max(0, num);
		num2 = Math.Min(num2, this.GetSlotCount() - 1);
		for (int i = num; i <= num2; i++)
		{
			CollectionDeckSlot slotByIndex = this.GetSlotByIndex(i);
			slotByIndex.Index = i;
		}
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x000D691C File Offset: 0x000D4B1C
	public CollectionDeckSlot FindSlotByCardId(string cardID, TAG_PREMIUM premium)
	{
		return this.m_slots.Find((CollectionDeckSlot slot) => slot.CardID.Equals(cardID) && slot.Premium == premium);
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x000D6954 File Offset: 0x000D4B54
	private CollectionDeckSlot FindOwnedSlotByCardId(string cardID, TAG_PREMIUM premium, bool owned)
	{
		if (!this.IsCardOwnershipUnique())
		{
			return this.FindSlotByCardId(cardID, premium);
		}
		return this.m_slots.Find((CollectionDeckSlot slot) => slot.CardID.Equals(cardID) && slot.Premium == premium && slot.Owned == owned);
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x000D69B4 File Offset: 0x000D4BB4
	private CollectionDeckSlot FindValidSlotByCardId(string cardID, TAG_PREMIUM premium, bool valid)
	{
		return this.m_slots.Find((CollectionDeckSlot slot) => slot.CardID.Equals(cardID) && slot.Premium == premium && this.IsValidSlot(slot) == valid);
	}

	// Token: 0x06002B35 RID: 11061 RVA: 0x000D69FC File Offset: 0x000D4BFC
	private void GenerateNameDiff(CollectionDeck baseDeck, out string deckName)
	{
		deckName = null;
		if (!this.Name.Equals(baseDeck.Name))
		{
			deckName = this.Name;
		}
	}

	// Token: 0x06002B36 RID: 11062 RVA: 0x000D6A2C File Offset: 0x000D4C2C
	private bool GenerateHeroDiff(CollectionDeck baseDeck, out int heroAssetID, out TAG_PREMIUM heroCardPremium)
	{
		heroAssetID = ConnectAPI.SEND_DECK_DATA_NO_HERO_ASSET_CHANGE;
		heroCardPremium = TAG_PREMIUM.NORMAL;
		if (!this.HeroOverridden)
		{
			return false;
		}
		bool flag = this.HeroCardID == baseDeck.HeroCardID && this.HeroPremium == baseDeck.HeroPremium;
		if (baseDeck.HeroOverridden && flag)
		{
			return false;
		}
		heroAssetID = GameUtils.TranslateCardIdToDbId(this.HeroCardID);
		heroCardPremium = this.HeroPremium;
		return true;
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x000D6AA4 File Offset: 0x000D4CA4
	private bool GenerateCardBackDiff(CollectionDeck baseDeck, out int cardBackID)
	{
		cardBackID = ConnectAPI.SEND_DECK_DATA_NO_CARD_BACK_CHANGE;
		if (!this.CardBackOverridden)
		{
			return false;
		}
		bool flag = this.CardBackID == baseDeck.CardBackID;
		if (baseDeck.CardBackOverridden && flag)
		{
			return false;
		}
		cardBackID = this.CardBackID;
		return true;
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x000D6AF0 File Offset: 0x000D4CF0
	private Network.CardUserData CardUserDataFromSlot(CollectionDeckSlot deckSlot, bool deleted)
	{
		return new Network.CardUserData
		{
			DbId = GameUtils.TranslateCardIdToDbId(deckSlot.CardID),
			Count = ((!deleted) ? deckSlot.Count : 0),
			Premium = deckSlot.Premium
		};
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x000D6B3C File Offset: 0x000D4D3C
	private List<Network.CardUserData> GenerateContentChanges(CollectionDeck baseDeck)
	{
		SortedDictionary<long, CollectionDeckSlot> sortedDictionary = new SortedDictionary<long, CollectionDeckSlot>();
		foreach (CollectionDeckSlot collectionDeckSlot in baseDeck.GetSlots())
		{
			CollectionDeckSlot collectionDeckSlot2 = null;
			if (sortedDictionary.TryGetValue(collectionDeckSlot.UID, ref collectionDeckSlot2))
			{
				collectionDeckSlot2.Count += collectionDeckSlot.Count;
			}
			else
			{
				collectionDeckSlot2 = new CollectionDeckSlot();
				collectionDeckSlot2.CopyFrom(collectionDeckSlot);
				sortedDictionary.Add(collectionDeckSlot2.UID, collectionDeckSlot2);
			}
		}
		SortedDictionary<long, CollectionDeckSlot> sortedDictionary2 = new SortedDictionary<long, CollectionDeckSlot>();
		foreach (CollectionDeckSlot collectionDeckSlot3 in this.GetSlots())
		{
			CollectionDeckSlot collectionDeckSlot4 = null;
			if (sortedDictionary2.TryGetValue(collectionDeckSlot3.UID, ref collectionDeckSlot4))
			{
				collectionDeckSlot4.Count += collectionDeckSlot3.Count;
			}
			else
			{
				collectionDeckSlot4 = new CollectionDeckSlot();
				collectionDeckSlot4.CopyFrom(collectionDeckSlot3);
				sortedDictionary2.Add(collectionDeckSlot4.UID, collectionDeckSlot4);
			}
		}
		SortedDictionary<long, CollectionDeckSlot>.Enumerator enumerator3 = sortedDictionary.GetEnumerator();
		SortedDictionary<long, CollectionDeckSlot>.Enumerator enumerator4 = sortedDictionary2.GetEnumerator();
		List<Network.CardUserData> list = new List<Network.CardUserData>();
		bool flag = enumerator3.MoveNext();
		bool flag2 = enumerator4.MoveNext();
		while (flag && flag2)
		{
			KeyValuePair<long, CollectionDeckSlot> keyValuePair = enumerator3.Current;
			CollectionDeckSlot value = keyValuePair.Value;
			KeyValuePair<long, CollectionDeckSlot> keyValuePair2 = enumerator4.Current;
			CollectionDeckSlot value2 = keyValuePair2.Value;
			if (value.GetUID(this.Type) == value2.GetUID(this.Type))
			{
				if (value.Count != value2.Count)
				{
					list.Add(this.CardUserDataFromSlot(value2, 0 == value2.Count));
				}
				flag = enumerator3.MoveNext();
				flag2 = enumerator4.MoveNext();
			}
			else if (value.GetUID(this.Type) < value2.GetUID(this.Type))
			{
				list.Add(this.CardUserDataFromSlot(value, true));
				flag = enumerator3.MoveNext();
			}
			else
			{
				list.Add(this.CardUserDataFromSlot(value2, false));
				flag2 = enumerator4.MoveNext();
			}
		}
		while (flag)
		{
			KeyValuePair<long, CollectionDeckSlot> keyValuePair3 = enumerator3.Current;
			CollectionDeckSlot value3 = keyValuePair3.Value;
			list.Add(this.CardUserDataFromSlot(value3, true));
			flag = enumerator3.MoveNext();
		}
		while (flag2)
		{
			KeyValuePair<long, CollectionDeckSlot> keyValuePair4 = enumerator4.Current;
			CollectionDeckSlot value4 = keyValuePair4.Value;
			list.Add(this.CardUserDataFromSlot(value4, false));
			flag2 = enumerator4.MoveNext();
		}
		return list;
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x000D6E04 File Offset: 0x000D5004
	private int GetInsertionIdxByDefaultSort(CollectionDeckSlot slot)
	{
		EntityDef entityDef = DefLoader.Get().GetEntityDef(slot.CardID);
		if (entityDef == null)
		{
			Log.Rachelle.Print(string.Format("CollectionDeck.GetInsertionIdxByDefaultSort(): could not get entity def for {0}", slot.CardID), new object[0]);
			return -1;
		}
		int i;
		for (i = 0; i < this.GetSlotCount(); i++)
		{
			CollectionDeckSlot slotByIndex = this.GetSlotByIndex(i);
			EntityDef entityDef2 = DefLoader.Get().GetEntityDef(slotByIndex.CardID);
			if (entityDef2 == null)
			{
				Log.Rachelle.Print(string.Format("CollectionDeck.GetInsertionIdxByDefaultSort(): entityDef is null at slot index {0}", i), new object[0]);
				break;
			}
			int num = CollectionManager.Get().EntityDefSortComparison(entityDef, entityDef2);
			if (num < 0)
			{
				break;
			}
			if (num <= 0)
			{
				if (slot.Premium <= slotByIndex.Premium)
				{
					if (!this.IsCardOwnershipUnique() || slot.Owned == slotByIndex.Owned)
					{
						break;
					}
				}
			}
		}
		return i;
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x000D6F0C File Offset: 0x000D510C
	public TAG_CLASS GetClass()
	{
		EntityDef entityDef = DefLoader.Get().GetEntityDef(this.HeroCardID);
		return entityDef.GetClass();
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x000D6F34 File Offset: 0x000D5134
	public string ToDeckString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		EntityDef entityDef = DefLoader.Get().GetEntityDef(this.HeroCardID);
		string name = BnetPresenceMgr.Get().GetMyPlayer().GetBattleTag().GetName();
		string text = entityDef.GetClass().ToString().ToLower();
		text = text.Substring(0, 1).ToUpper() + text.Substring(1, text.Length - 1);
		stringBuilder.Append(string.Format("# Hearthstone {0} Deck: \"{1}\" saved by {2}\n", text, this.Name, name));
		stringBuilder.Append("#\n");
		foreach (CollectionDeckSlot collectionDeckSlot in this.m_slots)
		{
			EntityDef entityDef2 = DefLoader.Get().GetEntityDef(collectionDeckSlot.CardID);
			stringBuilder.Append(string.Format("# {0}x ({1}) {2}\n", collectionDeckSlot.Count, entityDef2.GetCost(), entityDef2.GetName()));
			stringBuilder2.Append(string.Format("{0},{1};", collectionDeckSlot.Count, collectionDeckSlot.CardID));
		}
		stringBuilder.Append("#\n");
		stringBuilder.Append(stringBuilder2.ToString());
		return stringBuilder.ToString();
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x000D70A8 File Offset: 0x000D52A8
	public List<DeckMaker.DeckFill> GetDeckFillFromString(string deckString)
	{
		List<DeckMaker.DeckFill> list = new List<DeckMaker.DeckFill>();
		foreach (string text in deckString.Split(new char[]
		{
			'\n'
		}))
		{
			string text2 = text.Trim();
			if (!text2.StartsWith("#"))
			{
				try
				{
					string[] array2 = text2.Split(new char[]
					{
						';'
					});
					foreach (string text3 in array2)
					{
						try
						{
							string[] array4 = text3.Split(new char[]
							{
								','
							});
							int num;
							bool flag = int.TryParse(array4[0], ref num);
							if (flag && num >= 0 && num <= 10)
							{
								string cardId = array4[1];
								EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
								if (entityDef != null)
								{
									for (int k = 0; k < num; k++)
									{
										list.Add(new DeckMaker.DeckFill
										{
											m_addCard = entityDef
										});
									}
								}
							}
						}
						catch
						{
						}
					}
				}
				catch
				{
				}
			}
		}
		return list;
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x000D7208 File Offset: 0x000D5408
	private bool IsCardOwnershipUnique()
	{
		return this.Type == 1;
	}

	// Token: 0x04001A16 RID: 6678
	private string m_name;

	// Token: 0x04001A17 RID: 6679
	private List<CollectionDeckSlot> m_slots = new List<CollectionDeckSlot>();

	// Token: 0x04001A18 RID: 6680
	private bool m_netContentsLoaded;

	// Token: 0x04001A19 RID: 6681
	private bool m_isSavingContentChanges;

	// Token: 0x04001A1A RID: 6682
	private bool m_isSavingNameChanges;

	// Token: 0x04001A1B RID: 6683
	private bool m_isBeingDeleted;

	// Token: 0x04001A1C RID: 6684
	public long ID;

	// Token: 0x04001A1D RID: 6685
	public DeckType Type = 1;

	// Token: 0x04001A1E RID: 6686
	public string HeroCardID = string.Empty;

	// Token: 0x04001A1F RID: 6687
	public TAG_PREMIUM HeroPremium;

	// Token: 0x04001A20 RID: 6688
	public bool HeroOverridden;

	// Token: 0x04001A21 RID: 6689
	public int CardBackID;

	// Token: 0x04001A22 RID: 6690
	public bool CardBackOverridden;

	// Token: 0x04001A23 RID: 6691
	public int SeasonId;

	// Token: 0x04001A24 RID: 6692
	public bool NeedsName;

	// Token: 0x04001A25 RID: 6693
	public long SortOrder;

	// Token: 0x04001A26 RID: 6694
	public DeckSourceType SourceType;

	// Token: 0x04001A27 RID: 6695
	private bool m_isWild;
}
