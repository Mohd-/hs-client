using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003A2 RID: 930
public class CollectibleCard
{
	// Token: 0x060030E2 RID: 12514 RVA: 0x000F6029 File Offset: 0x000F4229
	public CollectibleCard(CardDbfRecord cardRecord, EntityDef refEntityDef, TAG_PREMIUM premiumType)
	{
		this.m_CardDbId = cardRecord.ID;
		this.m_EntityDef = refEntityDef;
		this.m_PremiumType = premiumType;
		this.m_CardRecord = cardRecord;
	}

	// Token: 0x170003BD RID: 957
	// (get) Token: 0x060030E3 RID: 12515 RVA: 0x000F6066 File Offset: 0x000F4266
	public int CardDbId
	{
		get
		{
			return this.m_CardDbId;
		}
	}

	// Token: 0x170003BE RID: 958
	// (get) Token: 0x060030E4 RID: 12516 RVA: 0x000F606E File Offset: 0x000F426E
	public string CardId
	{
		get
		{
			return this.m_EntityDef.GetCardId();
		}
	}

	// Token: 0x170003BF RID: 959
	// (get) Token: 0x060030E5 RID: 12517 RVA: 0x000F607B File Offset: 0x000F427B
	public string Name
	{
		get
		{
			return this.m_EntityDef.GetName();
		}
	}

	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x060030E6 RID: 12518 RVA: 0x000F6088 File Offset: 0x000F4288
	public string CardInHandText
	{
		get
		{
			return this.m_EntityDef.GetCardTextInHand();
		}
	}

	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x060030E7 RID: 12519 RVA: 0x000F6095 File Offset: 0x000F4295
	public string ArtistName
	{
		get
		{
			return this.m_EntityDef.GetArtistName();
		}
	}

	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x060030E8 RID: 12520 RVA: 0x000F60A2 File Offset: 0x000F42A2
	public int ManaCost
	{
		get
		{
			return this.m_EntityDef.GetCost();
		}
	}

	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x060030E9 RID: 12521 RVA: 0x000F60AF File Offset: 0x000F42AF
	public int Attack
	{
		get
		{
			return this.m_EntityDef.GetATK();
		}
	}

	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x060030EA RID: 12522 RVA: 0x000F60BC File Offset: 0x000F42BC
	public int Health
	{
		get
		{
			return this.m_EntityDef.GetHealth();
		}
	}

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x060030EB RID: 12523 RVA: 0x000F60C9 File Offset: 0x000F42C9
	public TAG_CARD_SET Set
	{
		get
		{
			return this.m_EntityDef.GetCardSet();
		}
	}

	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x060030EC RID: 12524 RVA: 0x000F60D6 File Offset: 0x000F42D6
	public TAG_CLASS Class
	{
		get
		{
			return this.m_EntityDef.GetClass();
		}
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x060030ED RID: 12525 RVA: 0x000F60E3 File Offset: 0x000F42E3
	public TAG_RARITY Rarity
	{
		get
		{
			return this.m_EntityDef.GetRarity();
		}
	}

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x060030EE RID: 12526 RVA: 0x000F60F0 File Offset: 0x000F42F0
	public TAG_RACE Race
	{
		get
		{
			return this.m_EntityDef.GetRace();
		}
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x060030EF RID: 12527 RVA: 0x000F60FD File Offset: 0x000F42FD
	public TAG_CARDTYPE CardType
	{
		get
		{
			return this.m_EntityDef.GetCardType();
		}
	}

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x060030F0 RID: 12528 RVA: 0x000F610A File Offset: 0x000F430A
	public bool IsHero
	{
		get
		{
			return this.m_EntityDef.IsHero();
		}
	}

	// Token: 0x170003CB RID: 971
	// (get) Token: 0x060030F1 RID: 12529 RVA: 0x000F6117 File Offset: 0x000F4317
	public TAG_PREMIUM PremiumType
	{
		get
		{
			return this.m_PremiumType;
		}
	}

	// Token: 0x170003CC RID: 972
	// (get) Token: 0x060030F2 RID: 12530 RVA: 0x000F611F File Offset: 0x000F431F
	// (set) Token: 0x060030F3 RID: 12531 RVA: 0x000F6127 File Offset: 0x000F4327
	public int SeenCount { get; set; }

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x060030F4 RID: 12532 RVA: 0x000F6130 File Offset: 0x000F4330
	// (set) Token: 0x060030F5 RID: 12533 RVA: 0x000F6138 File Offset: 0x000F4338
	public int OwnedCount { get; set; }

	// Token: 0x170003CE RID: 974
	// (get) Token: 0x060030F6 RID: 12534 RVA: 0x000F6141 File Offset: 0x000F4341
	public int DisenchantCount
	{
		get
		{
			return Mathf.Max(this.OwnedCount - this.MaxCopiesPerDeck, 0);
		}
	}

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x060030F7 RID: 12535 RVA: 0x000F6156 File Offset: 0x000F4356
	public int MaxCopiesPerDeck
	{
		get
		{
			return (!this.m_EntityDef.IsElite()) ? 2 : 1;
		}
	}

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x060030F8 RID: 12536 RVA: 0x000F616F File Offset: 0x000F436F
	// (set) Token: 0x060030F9 RID: 12537 RVA: 0x000F6177 File Offset: 0x000F4377
	public int CraftBuyCost { get; set; }

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x060030FA RID: 12538 RVA: 0x000F6180 File Offset: 0x000F4380
	// (set) Token: 0x060030FB RID: 12539 RVA: 0x000F6188 File Offset: 0x000F4388
	public int CraftSellCost { get; set; }

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x060030FC RID: 12540 RVA: 0x000F6194 File Offset: 0x000F4394
	public bool IsCraftable
	{
		get
		{
			return FixedRewardsMgr.Get().CanCraftCard(this.CardId, this.PremiumType) && !this.IsHero;
		}
	}

	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x060030FD RID: 12541 RVA: 0x000F61C8 File Offset: 0x000F43C8
	public bool IsNewCard
	{
		get
		{
			return this.OwnedCount > 0 && this.SeenCount < this.OwnedCount && this.SeenCount < this.MaxCopiesPerDeck;
		}
	}

	// Token: 0x170003D4 RID: 980
	// (get) Token: 0x060030FE RID: 12542 RVA: 0x000F6203 File Offset: 0x000F4403
	public int SuggestWeight
	{
		get
		{
			return this.m_CardRecord.SuggestionWeight;
		}
	}

	// Token: 0x170003D5 RID: 981
	// (get) Token: 0x060030FF RID: 12543 RVA: 0x000F6210 File Offset: 0x000F4410
	public bool IsManaCostChanged
	{
		get
		{
			return this.m_CardRecord.ChangedManaCost;
		}
	}

	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x06003100 RID: 12544 RVA: 0x000F621D File Offset: 0x000F441D
	public bool IsHealthChanged
	{
		get
		{
			return this.m_CardRecord.ChangedHealth;
		}
	}

	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x06003101 RID: 12545 RVA: 0x000F622A File Offset: 0x000F442A
	public bool IsAttackChanged
	{
		get
		{
			return this.m_CardRecord.ChangedAttack;
		}
	}

	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x06003102 RID: 12546 RVA: 0x000F6237 File Offset: 0x000F4437
	public bool IsCardInHandTextChanged
	{
		get
		{
			return this.m_CardRecord.ChangedCardTextInHand;
		}
	}

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x06003103 RID: 12547 RVA: 0x000F6244 File Offset: 0x000F4444
	public int ChangeVersion
	{
		get
		{
			return this.m_CardRecord.ChangeVersion;
		}
	}

	// Token: 0x170003DA RID: 986
	// (get) Token: 0x06003104 RID: 12548 RVA: 0x000F6251 File Offset: 0x000F4451
	// (set) Token: 0x06003105 RID: 12549 RVA: 0x000F6259 File Offset: 0x000F4459
	public DateTime LatestInsertDate
	{
		get
		{
			return this.m_LatestInsertDate;
		}
		set
		{
			if (value > this.m_LatestInsertDate)
			{
				this.m_LatestInsertDate = value;
			}
		}
	}

	// Token: 0x06003106 RID: 12550 RVA: 0x000F6274 File Offset: 0x000F4474
	public HashSet<string> GetSearchableTokens()
	{
		if (this.m_SearchableTokens == null)
		{
			this.m_SearchableTokens = new HashSet<string>();
			CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARD_SET>(this.Set, new Func<TAG_CARD_SET, bool>(GameStrings.HasCardSetName), new Func<TAG_CARD_SET, string>(GameStrings.GetCardSetName), this.m_SearchableTokens);
			CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARD_SET>(this.Set, new Func<TAG_CARD_SET, bool>(GameStrings.HasCardSetNameShortened), new Func<TAG_CARD_SET, string>(GameStrings.GetCardSetNameShortened), this.m_SearchableTokens);
			CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARD_SET>(this.Set, new Func<TAG_CARD_SET, bool>(GameStrings.HasCardSetNameInitials), new Func<TAG_CARD_SET, string>(GameStrings.GetCardSetNameInitials), this.m_SearchableTokens);
			CollectibleCardFilter.AddSearchableTokensToSet<TAG_CLASS>(this.Class, new Func<TAG_CLASS, bool>(GameStrings.HasClassName), new Func<TAG_CLASS, string>(GameStrings.GetClassName), this.m_SearchableTokens);
			CollectibleCardFilter.AddSearchableTokensToSet<TAG_RARITY>(this.Rarity, new Func<TAG_RARITY, bool>(GameStrings.HasRarityText), new Func<TAG_RARITY, string>(GameStrings.GetRarityText), this.m_SearchableTokens);
			CollectibleCardFilter.AddSearchableTokensToSet<TAG_RACE>(this.Race, new Func<TAG_RACE, bool>(GameStrings.HasRaceName), new Func<TAG_RACE, string>(GameStrings.GetRaceName), this.m_SearchableTokens);
			CollectibleCardFilter.AddSearchableTokensToSet<TAG_CARDTYPE>(this.CardType, new Func<TAG_CARDTYPE, bool>(GameStrings.HasCardTypeName), new Func<TAG_CARDTYPE, string>(GameStrings.GetCardTypeName), this.m_SearchableTokens);
		}
		return this.m_SearchableTokens;
	}

	// Token: 0x06003107 RID: 12551 RVA: 0x000F63BC File Offset: 0x000F45BC
	public bool FindTextInCard(string searchStr)
	{
		searchStr = searchStr.Trim();
		HashSet<string> searchableTokens = this.GetSearchableTokens();
		if (searchableTokens.Contains(searchStr))
		{
			return true;
		}
		if (this.m_LongSearchableName == null)
		{
			this.m_LongSearchableName = (this.Name + " " + this.CardInHandText).Trim().ToLower();
			this.m_LongSearchableNameNonEuropean = CollectibleCardFilter.ConvertEuropeanCharacters(this.m_LongSearchableName);
			this.m_LongSearchableNameNoDiacritics = CollectibleCardFilter.RemoveDiacritics(this.m_LongSearchableName);
		}
		return this.m_LongSearchableName.Contains(searchStr, 5) || this.m_LongSearchableNameNonEuropean.Contains(searchStr, 5) || this.m_LongSearchableNameNoDiacritics.Contains(searchStr, 5);
	}

	// Token: 0x06003108 RID: 12552 RVA: 0x000F6470 File Offset: 0x000F4670
	public static bool FindTextInternational(string searchStr, string stringToSearch)
	{
		string str = CollectibleCardFilter.ConvertEuropeanCharacters(stringToSearch);
		string str2 = CollectibleCardFilter.RemoveDiacritics(stringToSearch);
		return stringToSearch.Contains(searchStr, 5) || str.Contains(searchStr, 5) || str2.Contains(searchStr, 5);
	}

	// Token: 0x06003109 RID: 12553 RVA: 0x000F64B0 File Offset: 0x000F46B0
	public void AddCounts(int addOwnedCount, int addSeenCount, DateTime latestInsertDate)
	{
		this.OwnedCount += addOwnedCount;
		this.SeenCount += addSeenCount;
		this.LatestInsertDate = latestInsertDate;
	}

	// Token: 0x0600310A RID: 12554 RVA: 0x000F64E0 File Offset: 0x000F46E0
	public void RemoveCounts(int removeOwnedCount)
	{
		this.OwnedCount = Mathf.Max(this.OwnedCount - removeOwnedCount, 0);
	}

	// Token: 0x0600310B RID: 12555 RVA: 0x000F64F6 File Offset: 0x000F46F6
	public EntityDef GetEntityDef()
	{
		return this.m_EntityDef;
	}

	// Token: 0x0600310C RID: 12556 RVA: 0x000F6500 File Offset: 0x000F4700
	public override bool Equals(object obj)
	{
		return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (this.CardDbId == ((CollectibleCard)obj).CardDbId && this.PremiumType == ((CollectibleCard)obj).PremiumType));
	}

	// Token: 0x0600310D RID: 12557 RVA: 0x000F6558 File Offset: 0x000F4758
	public override int GetHashCode()
	{
		return (int)(this.CardId.GetHashCode() + this.PremiumType);
	}

	// Token: 0x0600310E RID: 12558 RVA: 0x000F6578 File Offset: 0x000F4778
	public bool IsCardChanged()
	{
		return this.m_CardRecord.ChangedManaCost || this.m_CardRecord.ChangedHealth || this.m_CardRecord.ChangedAttack || this.m_CardRecord.ChangedCardTextInHand;
	}

	// Token: 0x04001E7C RID: 7804
	private int m_CardDbId = -1;

	// Token: 0x04001E7D RID: 7805
	private DateTime m_LatestInsertDate = new DateTime(0L);

	// Token: 0x04001E7E RID: 7806
	private HashSet<string> m_SearchableTokens;

	// Token: 0x04001E7F RID: 7807
	private string m_LongSearchableName;

	// Token: 0x04001E80 RID: 7808
	private string m_LongSearchableNameNonEuropean;

	// Token: 0x04001E81 RID: 7809
	private string m_LongSearchableNameNoDiacritics;

	// Token: 0x04001E82 RID: 7810
	private EntityDef m_EntityDef;

	// Token: 0x04001E83 RID: 7811
	private TAG_PREMIUM m_PremiumType;

	// Token: 0x04001E84 RID: 7812
	private CardDbfRecord m_CardRecord;
}
