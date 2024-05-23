using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using PegasusShared;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class GameUtils
{
	// Token: 0x06000343 RID: 835 RVA: 0x0000F814 File Offset: 0x0000DA14
	public static string TranslateDbIdToCardId(int dbId)
	{
		CardDbfRecord record = GameDbf.Card.GetRecord(dbId);
		if (record == null)
		{
			GameUtils.ShowDetailedCardError("GameUtils.TranslateDbIdToCardId() - Failed to find card with database id {0} in the Card DBF.", new object[]
			{
				dbId
			});
			return null;
		}
		string noteMiniGuid = record.NoteMiniGuid;
		if (noteMiniGuid == null)
		{
			GameUtils.ShowDetailedCardError("GameUtils.TranslateDbIdToCardId() - Card with database id {0} has no NOTE_MINI_GUID field in the Card DBF.", new object[]
			{
				dbId
			});
			return null;
		}
		return noteMiniGuid;
	}

	// Token: 0x06000344 RID: 836 RVA: 0x0000F878 File Offset: 0x0000DA78
	public static int TranslateCardIdToDbId(string cardId)
	{
		CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardId);
		if (cardRecord == null)
		{
			GameUtils.ShowDetailedCardError("GameUtils.TranslateCardIdToDbId() - There is no card with NOTE_MINI_GUID {0} in the Card DBF.", new object[]
			{
				cardId
			});
			return 0;
		}
		return cardRecord.ID;
	}

	// Token: 0x06000345 RID: 837 RVA: 0x0000F8AE File Offset: 0x0000DAAE
	public static long CardUID(string cardStringId, TAG_PREMIUM premium)
	{
		return GameUtils.CardUID(GameUtils.TranslateCardIdToDbId(cardStringId), premium);
	}

	// Token: 0x06000346 RID: 838 RVA: 0x0000F8BC File Offset: 0x0000DABC
	public static long CardUID(int cardDbId, TAG_PREMIUM premium)
	{
		long num = (premium != TAG_PREMIUM.GOLDEN) ? 0L : 4294967296L;
		long num2 = (long)cardDbId;
		return num | num2;
	}

	// Token: 0x06000347 RID: 839 RVA: 0x0000F8E9 File Offset: 0x0000DAE9
	public static long ClientCardUID(string cardStringId, TAG_PREMIUM premium, bool owned)
	{
		return GameUtils.ClientCardUID(GameUtils.TranslateCardIdToDbId(cardStringId), premium, owned);
	}

	// Token: 0x06000348 RID: 840 RVA: 0x0000F8F8 File Offset: 0x0000DAF8
	public static long ClientCardUID(int cardDbId, TAG_PREMIUM premium, bool owned)
	{
		return GameUtils.CardUID(cardDbId, premium) | ((!owned) ? 0L : 8589934592L);
	}

	// Token: 0x06000349 RID: 841 RVA: 0x0000F918 File Offset: 0x0000DB18
	public static bool IsCardCollectible(string cardId)
	{
		CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardId);
		return cardRecord != null && cardRecord.IsCollectible;
	}

	// Token: 0x0600034A RID: 842 RVA: 0x0000F93C File Offset: 0x0000DB3C
	public static bool IsAnythingRotated()
	{
		HashSet<AdventureDbId> rotatedAdventures = GameUtils.GetRotatedAdventures();
		if (rotatedAdventures.Count > 0)
		{
			return true;
		}
		HashSet<BoosterDbId> rotatedBoosters = GameUtils.GetRotatedBoosters();
		if (rotatedBoosters.Count > 0)
		{
			return true;
		}
		HashSet<TAG_CARD_SET> rotatedSets = GameUtils.GetRotatedSets();
		if (rotatedSets.Count > 0)
		{
			return true;
		}
		HashSet<int> rotatedIndividualCards = GameUtils.GetRotatedIndividualCards();
		return rotatedIndividualCards.Count > 0;
	}

	// Token: 0x0600034B RID: 843 RVA: 0x0000F99C File Offset: 0x0000DB9C
	public static HashSet<AdventureDbId> GetRotatedAdventures()
	{
		if (GameUtils.s_cachedRotatedAdventures.Value == null || Time.realtimeSinceStartup >= GameUtils.s_cachedRotatedAdventures.Key + 1f)
		{
			HashSet<AdventureDbId> hashSet = new HashSet<AdventureDbId>();
			SpecialEventManager specialEventManager = SpecialEventManager.Get();
			foreach (RotatedItemDbfRecord rotatedItemDbfRecord in GameDbf.RotatedItem.GetRecords())
			{
				if (rotatedItemDbfRecord.ItemType == 2)
				{
					AdventureDbId adventureDbId;
					if (EnumUtils.TryCast<AdventureDbId>(rotatedItemDbfRecord.ItemId, out adventureDbId))
					{
						if (specialEventManager.IsEventActive(rotatedItemDbfRecord.RotationEvent, false))
						{
							hashSet.Add(adventureDbId);
						}
					}
				}
			}
			GameUtils.s_cachedRotatedAdventures = new KeyValuePair<float, HashSet<AdventureDbId>>(Time.realtimeSinceStartup, hashSet);
		}
		return GameUtils.s_cachedRotatedAdventures.Value;
	}

	// Token: 0x0600034C RID: 844 RVA: 0x0000FA90 File Offset: 0x0000DC90
	public static bool IsAdventureRotated(AdventureDbId adventureID)
	{
		HashSet<AdventureDbId> rotatedAdventures = GameUtils.GetRotatedAdventures();
		return rotatedAdventures.Contains(adventureID);
	}

	// Token: 0x0600034D RID: 845 RVA: 0x0000FAAC File Offset: 0x0000DCAC
	public static HashSet<BoosterDbId> GetRotatedBoosters()
	{
		if (GameUtils.s_cachedRotatedBoosters.Value == null || Time.realtimeSinceStartup >= GameUtils.s_cachedRotatedBoosters.Key + 1f)
		{
			HashSet<BoosterDbId> hashSet = new HashSet<BoosterDbId>();
			SpecialEventManager specialEventManager = SpecialEventManager.Get();
			foreach (RotatedItemDbfRecord rotatedItemDbfRecord in GameDbf.RotatedItem.GetRecords())
			{
				if (rotatedItemDbfRecord.ItemType == 1)
				{
					BoosterDbId boosterDbId;
					if (EnumUtils.TryCast<BoosterDbId>(rotatedItemDbfRecord.ItemId, out boosterDbId))
					{
						if (specialEventManager.IsEventActive(rotatedItemDbfRecord.RotationEvent, false))
						{
							hashSet.Add(boosterDbId);
						}
					}
				}
			}
			GameUtils.s_cachedRotatedBoosters = new KeyValuePair<float, HashSet<BoosterDbId>>(Time.realtimeSinceStartup, hashSet);
		}
		return GameUtils.s_cachedRotatedBoosters.Value;
	}

	// Token: 0x0600034E RID: 846 RVA: 0x0000FBA0 File Offset: 0x0000DDA0
	public static bool IsBoosterRotated(BoosterDbId boosterID)
	{
		HashSet<BoosterDbId> rotatedBoosters = GameUtils.GetRotatedBoosters();
		return rotatedBoosters.Contains(boosterID);
	}

	// Token: 0x0600034F RID: 847 RVA: 0x0000FBBC File Offset: 0x0000DDBC
	public static HashSet<TAG_CARD_SET> GetRotatedSets()
	{
		if (GameUtils.s_cachedRotatedCardSets.Value == null || Time.realtimeSinceStartup >= GameUtils.s_cachedRotatedCardSets.Key + 1f)
		{
			HashSet<TAG_CARD_SET> hashSet = new HashSet<TAG_CARD_SET>();
			SpecialEventManager specialEventManager = SpecialEventManager.Get();
			foreach (RotatedItemDbfRecord rotatedItemDbfRecord in GameDbf.RotatedItem.GetRecords())
			{
				switch (rotatedItemDbfRecord.ItemType)
				{
				case 1:
				case 2:
				case 3:
					if (rotatedItemDbfRecord.CardSetId != 0)
					{
						if (specialEventManager.IsEventActive(rotatedItemDbfRecord.RotationEvent, false))
						{
							hashSet.Add((TAG_CARD_SET)rotatedItemDbfRecord.CardSetId);
						}
					}
					break;
				}
			}
			GameUtils.s_cachedRotatedCardSets = new KeyValuePair<float, HashSet<TAG_CARD_SET>>(Time.realtimeSinceStartup, hashSet);
		}
		return GameUtils.s_cachedRotatedCardSets.Value;
	}

	// Token: 0x06000350 RID: 848 RVA: 0x0000FCC4 File Offset: 0x0000DEC4
	public static bool IsSetRotated(TAG_CARD_SET set)
	{
		HashSet<TAG_CARD_SET> rotatedSets = GameUtils.GetRotatedSets();
		return rotatedSets.Contains(set);
	}

	// Token: 0x06000351 RID: 849 RVA: 0x0000FCE0 File Offset: 0x0000DEE0
	private static HashSet<int> GetRotatedIndividualCards()
	{
		if (GameUtils.s_cachedRotatedIndividualCards.Value == null || Time.realtimeSinceStartup >= GameUtils.s_cachedRotatedCardSets.Key + 1f)
		{
			HashSet<int> hashSet = new HashSet<int>();
			SpecialEventManager specialEventManager = SpecialEventManager.Get();
			foreach (RotatedItemDbfRecord rotatedItemDbfRecord in GameDbf.RotatedItem.GetRecords())
			{
				if (rotatedItemDbfRecord.ItemType == 4)
				{
					if (rotatedItemDbfRecord.CardId != 0)
					{
						if (specialEventManager.IsEventActive(rotatedItemDbfRecord.RotationEvent, false))
						{
							hashSet.Add(rotatedItemDbfRecord.CardId);
						}
					}
				}
			}
			GameUtils.s_cachedRotatedIndividualCards = new KeyValuePair<float, HashSet<int>>(Time.realtimeSinceStartup, hashSet);
		}
		return GameUtils.s_cachedRotatedIndividualCards.Value;
	}

	// Token: 0x06000352 RID: 850 RVA: 0x0000FDCC File Offset: 0x0000DFCC
	public static bool IsSetAnAdventure(TAG_CARD_SET cardSet)
	{
		return cardSet == TAG_CARD_SET.BRM || cardSet == TAG_CARD_SET.FP1 || cardSet == TAG_CARD_SET.LOE;
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0000FDE8 File Offset: 0x0000DFE8
	public static bool IsCardRotated(string cardId)
	{
		EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
		return GameUtils.IsCardRotated(entityDef);
	}

	// Token: 0x06000354 RID: 852 RVA: 0x0000FE08 File Offset: 0x0000E008
	public static bool IsCardRotated(EntityDef def)
	{
		TAG_CARD_SET tag = def.GetTag<TAG_CARD_SET>(GAME_TAG.CARD_SET);
		if (GameUtils.IsSetRotated(tag))
		{
			return true;
		}
		int num = GameUtils.TranslateCardIdToDbId(def.GetCardId());
		HashSet<int> rotatedIndividualCards = GameUtils.GetRotatedIndividualCards();
		return rotatedIndividualCards.Contains(num);
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0000FE50 File Offset: 0x0000E050
	public static TAG_CARD_SET[] GetStandardSets()
	{
		List<TAG_CARD_SET> list = new List<TAG_CARD_SET>();
		foreach (TAG_CARD_SET tag_CARD_SET in CollectionManager.Get().GetDisplayableCardSets())
		{
			if (!GameUtils.IsSetRotated(tag_CARD_SET))
			{
				list.Add(tag_CARD_SET);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0000FEC8 File Offset: 0x0000E0C8
	public static int CountAllCollectibleCards()
	{
		return GameDbf.GetIndex().GetCollectibleCardCount();
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0000FED4 File Offset: 0x0000E0D4
	public static List<string> GetAllCardIds()
	{
		return GameDbf.GetIndex().GetAllCardIds();
	}

	// Token: 0x06000358 RID: 856 RVA: 0x0000FEE0 File Offset: 0x0000E0E0
	public static List<string> GetAllCollectibleCardIds()
	{
		return GameDbf.GetIndex().GetCollectibleCardIds();
	}

	// Token: 0x06000359 RID: 857 RVA: 0x0000FEEC File Offset: 0x0000E0EC
	public static List<int> GetAllCollectibleCardDbIds()
	{
		return GameDbf.GetIndex().GetCollectibleCardDbIds();
	}

	// Token: 0x0600035A RID: 858 RVA: 0x0000FEF8 File Offset: 0x0000E0F8
	public static int CountNonHeroCollectibleCards()
	{
		int num = 0;
		foreach (string cardId in GameUtils.GetAllCollectibleCardIds())
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
			if (entityDef != null)
			{
				if (entityDef.GetCardType() != TAG_CARDTYPE.HERO)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x0600035B RID: 859 RVA: 0x0000FF7C File Offset: 0x0000E17C
	public static List<string> GetNonHeroCollectibleCardIds()
	{
		List<string> list = new List<string>();
		foreach (string text in GameUtils.GetAllCollectibleCardIds())
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(text);
			if (entityDef != null)
			{
				if (entityDef.GetCardType() != TAG_CARDTYPE.HERO)
				{
					list.Add(text);
				}
			}
		}
		return list;
	}

	// Token: 0x0600035C RID: 860 RVA: 0x00010004 File Offset: 0x0000E204
	public static List<string> GetNonHeroAllCardIds()
	{
		List<string> list = new List<string>();
		foreach (string text in GameUtils.GetAllCardIds())
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(text);
			if (entityDef != null)
			{
				if (entityDef.GetCardType() != TAG_CARDTYPE.HERO)
				{
					if (entityDef.GetCardType() != TAG_CARDTYPE.HERO_POWER)
					{
						if (entityDef.GetCardType() != TAG_CARDTYPE.ENCHANTMENT)
						{
							list.Add(text);
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x0600035D RID: 861 RVA: 0x000100B0 File Offset: 0x0000E2B0
	public static CardDbfRecord GetCardRecord(string cardId)
	{
		if (cardId == null)
		{
			return null;
		}
		return GameDbf.GetIndex().GetCardRecord(cardId);
	}

	// Token: 0x0600035E RID: 862 RVA: 0x000100C8 File Offset: 0x0000E2C8
	public static string GetHeroPowerCardIdFromHero(string heroCardId)
	{
		CardDbfRecord cardRecord = GameUtils.GetCardRecord(heroCardId);
		if (cardRecord == null)
		{
			Debug.LogError(string.Format("GameUtils.GetHeroPowerCardIdFromHero() - failed to find record for heroCardId {0}", heroCardId));
			return string.Empty;
		}
		int heroPowerId = cardRecord.HeroPowerId;
		return GameUtils.TranslateDbIdToCardId(heroPowerId);
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00010108 File Offset: 0x0000E308
	public static string GetHeroPowerCardIdFromHero(int heroDbId)
	{
		CardDbfRecord record = GameDbf.Card.GetRecord(heroDbId);
		if (record == null)
		{
			Debug.LogError(string.Format("GameUtils.GetHeroPowerCardIdFromHero() - failed to find record for heroDbId {0}", heroDbId));
			return string.Empty;
		}
		int heroPowerId = record.HeroPowerId;
		return GameUtils.TranslateDbIdToCardId(heroPowerId);
	}

	// Token: 0x06000360 RID: 864 RVA: 0x00010154 File Offset: 0x0000E354
	public static TAG_CARD_SET GetCardSetFromCardID(string cardID)
	{
		EntityDef entityDef = DefLoader.Get().GetEntityDef(cardID);
		if (entityDef == null)
		{
			Debug.LogError(string.Format("Null EntityDef in GetCardSetFromCardID() for {0}", cardID));
			return TAG_CARD_SET.INVALID;
		}
		return entityDef.GetCardSet();
	}

	// Token: 0x06000361 RID: 865 RVA: 0x0001018C File Offset: 0x0000E38C
	public static string GetBasicHeroCardIdFromClass(TAG_CLASS classTag)
	{
		switch (classTag)
		{
		case TAG_CLASS.DRUID:
			return "HERO_06";
		case TAG_CLASS.HUNTER:
			return "HERO_05";
		case TAG_CLASS.MAGE:
			return "HERO_08";
		case TAG_CLASS.PALADIN:
			return "HERO_04";
		case TAG_CLASS.PRIEST:
			return "HERO_09";
		case TAG_CLASS.ROGUE:
			return "HERO_03";
		case TAG_CLASS.SHAMAN:
			return "HERO_02";
		case TAG_CLASS.WARLOCK:
			return "HERO_07";
		case TAG_CLASS.WARRIOR:
			return "HERO_01";
		default:
			Debug.LogError(string.Format("GameUtils.GetBasicHeroCardIdFromClass() - unsupported class tag {0}", classTag));
			return string.Empty;
		}
	}

	// Token: 0x06000362 RID: 866 RVA: 0x0001021C File Offset: 0x0000E41C
	public static NetCache.HeroLevel GetHeroLevel(TAG_CLASS heroClass)
	{
		NetCache.NetCacheHeroLevels netObject = NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>();
		if (netObject == null)
		{
			Debug.LogError("GameUtils.GetHeroLevel() - NetCache.NetCacheHeroLevels is null");
		}
		return netObject.Levels.Find((NetCache.HeroLevel obj) => obj.Class == heroClass);
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0001026A File Offset: 0x0000E46A
	public static int CardPremiumSortComparisonAsc(TAG_PREMIUM premium1, TAG_PREMIUM premium2)
	{
		return premium1 - premium2;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0001026F File Offset: 0x0000E46F
	public static int CardPremiumSortComparisonDesc(TAG_PREMIUM premium1, TAG_PREMIUM premium2)
	{
		return premium2 - premium1;
	}

	// Token: 0x06000365 RID: 869 RVA: 0x00010274 File Offset: 0x0000E474
	private static void ShowDetailedCardError(string format, params object[] formatArgs)
	{
		string text = string.Format(format, formatArgs);
		StackTrace stackTrace = new StackTrace();
		StackFrame frame = stackTrace.GetFrame(2);
		MethodBase method = frame.GetMethod();
		Error.AddDevWarning("Error", "{0}\nCalled by {1}.{2}", new object[]
		{
			text,
			method.ReflectedType,
			method.Name
		});
	}

	// Token: 0x06000366 RID: 870 RVA: 0x000102CC File Offset: 0x0000E4CC
	public static Card GetJoustWinner(Network.HistMetaData metaData)
	{
		if (metaData == null)
		{
			return null;
		}
		if (metaData.MetaType != 3)
		{
			return null;
		}
		Entity entity = GameState.Get().GetEntity(metaData.Data);
		if (entity == null)
		{
			return null;
		}
		return entity.GetCard();
	}

	// Token: 0x06000367 RID: 871 RVA: 0x00010310 File Offset: 0x0000E510
	public static bool IsHistoryDeathTagChange(Network.HistTagChange tagChange)
	{
		if (tagChange.Tag != 360 || tagChange.Value != 1)
		{
			return false;
		}
		Entity entity = GameState.Get().GetEntity(tagChange.Entity);
		return entity != null && !entity.IsEnchantment() && entity.GetCardType() != TAG_CARDTYPE.INVALID;
	}

	// Token: 0x06000368 RID: 872 RVA: 0x00010370 File Offset: 0x0000E570
	public static bool IsCharacterDeathTagChange(Network.HistTagChange tagChange)
	{
		if (tagChange.Tag != 49)
		{
			return false;
		}
		if (tagChange.Value != 4)
		{
			return false;
		}
		Entity entity = GameState.Get().GetEntity(tagChange.Entity);
		return entity != null && entity.IsCharacter();
	}

	// Token: 0x06000369 RID: 873 RVA: 0x000103C4 File Offset: 0x0000E5C4
	public static bool IsGameOverTag(int entityId, int tag, int val)
	{
		Entity entity = GameState.Get().GetEntity(entityId);
		Player player = entity as Player;
		return GameUtils.IsGameOverTag(player, tag, val);
	}

	// Token: 0x0600036A RID: 874 RVA: 0x000103EC File Offset: 0x0000E5EC
	public static bool IsGameOverTag(Player player, int tag, int val)
	{
		if (player == null)
		{
			return false;
		}
		if (tag != 17)
		{
			return false;
		}
		if (!player.IsFriendlySide())
		{
			return false;
		}
		switch (val)
		{
		case 4:
		case 5:
		case 6:
			return true;
		default:
			return false;
		}
	}

	// Token: 0x0600036B RID: 875 RVA: 0x00010438 File Offset: 0x0000E638
	public static bool IsFriendlyConcede(Network.HistTagChange tagChange)
	{
		if (tagChange.Tag != 17)
		{
			return false;
		}
		Entity entity = GameState.Get().GetEntity(tagChange.Entity);
		Player player = entity as Player;
		if (player == null)
		{
			return false;
		}
		if (!player.IsFriendlySide())
		{
			return false;
		}
		TAG_PLAYSTATE value = (TAG_PLAYSTATE)tagChange.Value;
		return value == TAG_PLAYSTATE.CONCEDED;
	}

	// Token: 0x0600036C RID: 876 RVA: 0x0001048C File Offset: 0x0000E68C
	public static bool IsBeginPhase(TAG_STEP step)
	{
		switch (step)
		{
		case TAG_STEP.INVALID:
		case TAG_STEP.BEGIN_FIRST:
		case TAG_STEP.BEGIN_SHUFFLE:
		case TAG_STEP.BEGIN_DRAW:
		case TAG_STEP.BEGIN_MULLIGAN:
			return true;
		default:
			return false;
		}
	}

	// Token: 0x0600036D RID: 877 RVA: 0x000104BD File Offset: 0x0000E6BD
	public static bool IsPastBeginPhase(TAG_STEP step)
	{
		return !GameUtils.IsBeginPhase(step);
	}

	// Token: 0x0600036E RID: 878 RVA: 0x000104C8 File Offset: 0x0000E6C8
	public static bool IsMainPhase(TAG_STEP step)
	{
		switch (step)
		{
		case TAG_STEP.MAIN_BEGIN:
		case TAG_STEP.MAIN_READY:
		case TAG_STEP.MAIN_RESOURCE:
		case TAG_STEP.MAIN_DRAW:
		case TAG_STEP.MAIN_START:
		case TAG_STEP.MAIN_ACTION:
		case TAG_STEP.MAIN_COMBAT:
		case TAG_STEP.MAIN_END:
		case TAG_STEP.MAIN_NEXT:
		case TAG_STEP.MAIN_CLEANUP:
		case TAG_STEP.MAIN_START_TRIGGERS:
			return true;
		}
		return false;
	}

	// Token: 0x0600036F RID: 879 RVA: 0x0001051C File Offset: 0x0000E71C
	public static void ApplyPower(Entity entity, Network.PowerHistory power)
	{
		switch (power.Type)
		{
		case Network.PowerType.SHOW_ENTITY:
			GameUtils.ApplyShowEntity(entity, (Network.HistShowEntity)power);
			break;
		case Network.PowerType.HIDE_ENTITY:
			GameUtils.ApplyHideEntity(entity, (Network.HistHideEntity)power);
			break;
		case Network.PowerType.TAG_CHANGE:
			GameUtils.ApplyTagChange(entity, (Network.HistTagChange)power);
			break;
		}
	}

	// Token: 0x06000370 RID: 880 RVA: 0x0001057C File Offset: 0x0000E77C
	public static void ApplyShowEntity(Entity entity, Network.HistShowEntity showEntity)
	{
		foreach (Network.Entity.Tag tag in showEntity.Entity.Tags)
		{
			entity.SetTag(tag.Name, tag.Value);
		}
	}

	// Token: 0x06000371 RID: 881 RVA: 0x000105E8 File Offset: 0x0000E7E8
	public static void ApplyHideEntity(Entity entity, Network.HistHideEntity hideEntity)
	{
		entity.SetTag(GAME_TAG.ZONE, hideEntity.Zone);
	}

	// Token: 0x06000372 RID: 882 RVA: 0x000105F8 File Offset: 0x0000E7F8
	public static void ApplyTagChange(Entity entity, Network.HistTagChange tagChange)
	{
		entity.SetTag(tagChange.Tag, tagChange.Value);
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0001060C File Offset: 0x0000E80C
	public static TAG_ZONE GetFinalZoneForEntity(Entity entity)
	{
		PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
		List<PowerTaskList> list = new List<PowerTaskList>();
		if (powerProcessor.GetCurrentTaskList() != null)
		{
			list.Add(powerProcessor.GetCurrentTaskList());
		}
		list.AddRange(powerProcessor.GetPowerQueue().GetList());
		for (int i = list.Count - 1; i >= 0; i--)
		{
			List<PowerTask> taskList = list[i].GetTaskList();
			for (int j = taskList.Count - 1; j >= 0; j--)
			{
				PowerTask powerTask = taskList[j];
				Network.HistTagChange histTagChange = powerTask.GetPower() as Network.HistTagChange;
				if (histTagChange != null)
				{
					if (histTagChange.Entity == entity.GetEntityId() && histTagChange.Tag == 49)
					{
						return (TAG_ZONE)histTagChange.Value;
					}
				}
			}
		}
		return entity.GetZone();
	}

	// Token: 0x06000374 RID: 884 RVA: 0x000106E8 File Offset: 0x0000E8E8
	public static bool IsEntityHiddenAfterCurrentTasklist(Entity entity)
	{
		if (!entity.IsHidden())
		{
			return false;
		}
		PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
		if (powerProcessor.GetCurrentTaskList() != null)
		{
			foreach (PowerTask powerTask in powerProcessor.GetCurrentTaskList().GetTaskList())
			{
				Network.HistShowEntity histShowEntity = powerTask.GetPower() as Network.HistShowEntity;
				if (histShowEntity != null)
				{
					if (histShowEntity.Entity.ID == entity.GetEntityId() && !string.IsNullOrEmpty(histShowEntity.Entity.CardID))
					{
						return false;
					}
				}
			}
			return true;
		}
		return true;
	}

	// Token: 0x06000375 RID: 885 RVA: 0x000107B4 File Offset: 0x0000E9B4
	public static void DoDamageTasks(PowerTaskList powerTaskList, Card sourceCard, Card targetCard)
	{
		List<PowerTask> taskList = powerTaskList.GetTaskList();
		if (taskList == null)
		{
			return;
		}
		if (taskList.Count == 0)
		{
			return;
		}
		Entity entity = sourceCard.GetEntity();
		int entityId = entity.GetEntityId();
		Entity entity2 = targetCard.GetEntity();
		int entityId2 = entity2.GetEntityId();
		foreach (PowerTask powerTask in taskList)
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.META_DATA)
			{
				Network.HistMetaData histMetaData = (Network.HistMetaData)power;
				if (histMetaData.MetaType == 1 || histMetaData.MetaType == 2)
				{
					foreach (int num in histMetaData.Info)
					{
						if (num == entityId || num == entityId2)
						{
							powerTask.DoTask();
						}
					}
				}
			}
			else if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = (Network.HistTagChange)power;
				if (histTagChange.Entity == entityId || histTagChange.Entity == entityId2)
				{
					GAME_TAG tag = (GAME_TAG)histTagChange.Tag;
					if (tag == GAME_TAG.DAMAGE || tag == GAME_TAG.EXHAUSTED)
					{
						powerTask.DoTask();
					}
				}
			}
		}
	}

	// Token: 0x06000376 RID: 886 RVA: 0x00010950 File Offset: 0x0000EB50
	public static AdventureDbfRecord GetAdventureRecord(int missionId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		if (record == null)
		{
			return null;
		}
		int adventureId = record.AdventureId;
		return GameDbf.Adventure.GetRecord(adventureId);
	}

	// Token: 0x06000377 RID: 887 RVA: 0x00010984 File Offset: 0x0000EB84
	public static bool IsWingComplete(int advId, int modeId, int wingId)
	{
		List<ScenarioDbfRecord> records = GameDbf.Scenario.GetRecords();
		foreach (ScenarioDbfRecord scenarioDbfRecord in records)
		{
			int adventureId = scenarioDbfRecord.AdventureId;
			int modeId2 = scenarioDbfRecord.ModeId;
			int wingId2 = scenarioDbfRecord.WingId;
			if (adventureId == advId && modeId2 == modeId && wingId2 == wingId && !AdventureProgressMgr.Get().HasDefeatedScenario(scenarioDbfRecord.ID))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000378 RID: 888 RVA: 0x00010A2C File Offset: 0x0000EC2C
	public static WingDbfRecord GetWingRecord(int missionId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		if (record == null)
		{
			return null;
		}
		return GameDbf.Wing.GetRecord(record.WingId);
	}

	// Token: 0x06000379 RID: 889 RVA: 0x00010A60 File Offset: 0x0000EC60
	public static AdventureDataDbfRecord GetAdventureDataRecord(int adventureId, int modeId)
	{
		foreach (AdventureDataDbfRecord adventureDataDbfRecord in GameDbf.AdventureData.GetRecords())
		{
			int adventureId2 = adventureDataDbfRecord.AdventureId;
			if (adventureId2 == adventureId)
			{
				int modeId2 = adventureDataDbfRecord.ModeId;
				if (modeId2 == modeId)
				{
					return adventureDataDbfRecord;
				}
			}
		}
		return null;
	}

	// Token: 0x0600037A RID: 890 RVA: 0x00010AEC File Offset: 0x0000ECEC
	public static List<ScenarioDbfRecord> GetClassChallengeRecords(int adventureId, int wingId)
	{
		List<ScenarioDbfRecord> list = new List<ScenarioDbfRecord>();
		foreach (ScenarioDbfRecord scenarioDbfRecord in GameDbf.Scenario.GetRecords())
		{
			if (scenarioDbfRecord.ModeId == 4)
			{
				if (scenarioDbfRecord.AdventureId == adventureId)
				{
					if (scenarioDbfRecord.WingId == wingId)
					{
						list.Add(scenarioDbfRecord);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00010B84 File Offset: 0x0000ED84
	public static TAG_CLASS GetClassChallengeHeroClass(ScenarioDbfRecord rec)
	{
		if (rec.ModeId != 4)
		{
			return TAG_CLASS.INVALID;
		}
		int player1HeroCardId = rec.Player1HeroCardId;
		EntityDef entityDef = DefLoader.Get().GetEntityDef(player1HeroCardId);
		if (entityDef == null)
		{
			return TAG_CLASS.INVALID;
		}
		return entityDef.GetClass();
	}

	// Token: 0x0600037C RID: 892 RVA: 0x00010BC0 File Offset: 0x0000EDC0
	public static List<TAG_CLASS> GetClassChallengeHeroClasses(int adventureId, int wingId)
	{
		List<ScenarioDbfRecord> classChallengeRecords = GameUtils.GetClassChallengeRecords(adventureId, wingId);
		List<TAG_CLASS> list = new List<TAG_CLASS>();
		foreach (ScenarioDbfRecord rec in classChallengeRecords)
		{
			list.Add(GameUtils.GetClassChallengeHeroClass(rec));
		}
		return list;
	}

	// Token: 0x0600037D RID: 893 RVA: 0x00010C2C File Offset: 0x0000EE2C
	public static bool IsAIMission(int missionId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		return record != null && record.Players == 1;
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00010C5C File Offset: 0x0000EE5C
	public static bool IsCoopMission(int missionId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		return record != null && record.IsCoop;
	}

	// Token: 0x0600037F RID: 895 RVA: 0x00010C84 File Offset: 0x0000EE84
	public static string GetMissionHeroCardId(int missionId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		if (record == null)
		{
			return null;
		}
		int num = record.ClientPlayer2HeroCardId;
		if (num == 0)
		{
			num = record.Player2HeroCardId;
		}
		return GameUtils.TranslateDbIdToCardId(num);
	}

	// Token: 0x06000380 RID: 896 RVA: 0x00010CC4 File Offset: 0x0000EEC4
	public static string GetMissionHeroName(int missionId)
	{
		string missionHeroCardId = GameUtils.GetMissionHeroCardId(missionId);
		if (missionHeroCardId == null)
		{
			return null;
		}
		EntityDef entityDef = DefLoader.Get().GetEntityDef(missionHeroCardId);
		if (entityDef == null)
		{
			Debug.LogError(string.Format("GameUtils.GetMissionHeroName() - hero {0} for mission {1} has no EntityDef", missionHeroCardId, missionId));
			return null;
		}
		return entityDef.GetName();
	}

	// Token: 0x06000381 RID: 897 RVA: 0x00010D10 File Offset: 0x0000EF10
	public static string GetMissionHeroPowerCardId(int missionId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		if (record == null)
		{
			return null;
		}
		int num = record.ClientPlayer2HeroCardId;
		if (num == 0)
		{
			num = record.Player2HeroCardId;
		}
		CardDbfRecord record2 = GameDbf.Card.GetRecord(num);
		if (record2 == null)
		{
			return null;
		}
		int heroPowerId = record2.HeroPowerId;
		return GameUtils.TranslateDbIdToCardId(heroPowerId);
	}

	// Token: 0x06000382 RID: 898 RVA: 0x00010D6C File Offset: 0x0000EF6C
	public static bool IsMissionForAdventure(int missionId, int adventureId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		return record != null && adventureId == record.AdventureId;
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00010D96 File Offset: 0x0000EF96
	public static bool IsTutorialMission(int missionId)
	{
		return GameUtils.IsMissionForAdventure(missionId, 1);
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00010D9F File Offset: 0x0000EF9F
	public static bool IsPracticeMission(int missionId)
	{
		return GameUtils.IsMissionForAdventure(missionId, 2);
	}

	// Token: 0x06000385 RID: 901 RVA: 0x00010DA8 File Offset: 0x0000EFA8
	public static bool IsClassicMission(int missionId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		if (record == null)
		{
			return false;
		}
		int adventureId = record.AdventureId;
		if (adventureId == 0)
		{
			return false;
		}
		AdventureDbId adventureDbId = (AdventureDbId)adventureId;
		return adventureDbId == AdventureDbId.TUTORIAL || adventureDbId == AdventureDbId.PRACTICE;
	}

	// Token: 0x06000386 RID: 902 RVA: 0x00010DF0 File Offset: 0x0000EFF0
	public static bool IsExpansionMission(int missionId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		if (record == null)
		{
			return false;
		}
		int adventureId = record.AdventureId;
		if (adventureId == 0)
		{
			return false;
		}
		AdventureDbId adventureDbId = (AdventureDbId)adventureId;
		return adventureDbId != AdventureDbId.TUTORIAL && adventureDbId != AdventureDbId.PRACTICE;
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00010E38 File Offset: 0x0000F038
	public static bool IsExpansionAdventure(AdventureDbId adventureId)
	{
		switch (adventureId)
		{
		case AdventureDbId.NAXXRAMAS:
		case AdventureDbId.BRM:
		case AdventureDbId.LOE:
			return true;
		}
		return false;
	}

	// Token: 0x06000388 RID: 904 RVA: 0x00010E70 File Offset: 0x0000F070
	public static AdventureDbId GetAdventureId(int missionId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		if (record == null)
		{
			return AdventureDbId.INVALID;
		}
		return (AdventureDbId)record.AdventureId;
	}

	// Token: 0x06000389 RID: 905 RVA: 0x00010E9C File Offset: 0x0000F09C
	public static AdventureModeDbId GetAdventureModeId(int missionId)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		if (record == null)
		{
			return AdventureModeDbId.INVALID;
		}
		return (AdventureModeDbId)record.ModeId;
	}

	// Token: 0x0600038A RID: 906 RVA: 0x00010EC4 File Offset: 0x0000F0C4
	public static bool IsHeroicAdventureMission(int missionId)
	{
		AdventureModeDbId adventureModeId = GameUtils.GetAdventureModeId(missionId);
		return adventureModeId == AdventureModeDbId.HEROIC;
	}

	// Token: 0x0600038B RID: 907 RVA: 0x00010EDC File Offset: 0x0000F0DC
	public static bool IsClassChallengeMission(int missionId)
	{
		AdventureModeDbId adventureModeId = GameUtils.GetAdventureModeId(missionId);
		return adventureModeId == AdventureModeDbId.CLASS_CHALLENGE;
	}

	// Token: 0x0600038C RID: 908 RVA: 0x00010EF4 File Offset: 0x0000F0F4
	public static bool AreAllTutorialsComplete(TutorialProgress progress)
	{
		return DemoMgr.Get().GetMode() != DemoMode.APPLE_STORE && progress == TutorialProgress.ILLIDAN_COMPLETE;
	}

	// Token: 0x0600038D RID: 909 RVA: 0x00010F0D File Offset: 0x0000F10D
	public static bool AreAllTutorialsComplete(long campaignProgress)
	{
		return GameUtils.AreAllTutorialsComplete((TutorialProgress)campaignProgress);
	}

	// Token: 0x0600038E RID: 910 RVA: 0x00010F18 File Offset: 0x0000F118
	public static bool AreAllTutorialsComplete()
	{
		NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
		return netObject != null && GameUtils.AreAllTutorialsComplete(netObject.CampaignProgress);
	}

	// Token: 0x0600038F RID: 911 RVA: 0x00010F4C File Offset: 0x0000F14C
	public static int GetNextTutorial(TutorialProgress progress)
	{
		if (progress == TutorialProgress.NOTHING_COMPLETE)
		{
			return 3;
		}
		if (progress == TutorialProgress.HOGGER_COMPLETE)
		{
			return 4;
		}
		if (progress == TutorialProgress.MILLHOUSE_COMPLETE)
		{
			return 249;
		}
		if (progress == TutorialProgress.CHO_COMPLETE)
		{
			return 181;
		}
		if (progress == TutorialProgress.MUKLA_COMPLETE)
		{
			return 201;
		}
		if (progress == TutorialProgress.NESINGWARY_COMPLETE)
		{
			return 248;
		}
		return 0;
	}

	// Token: 0x06000390 RID: 912 RVA: 0x00010FA4 File Offset: 0x0000F1A4
	public static int GetNextTutorial()
	{
		NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
		if (netObject == null)
		{
			return 0;
		}
		return GameUtils.GetNextTutorial(netObject.CampaignProgress);
	}

	// Token: 0x06000391 RID: 913 RVA: 0x00010FD0 File Offset: 0x0000F1D0
	public static string GetTutorialCardRewardDetails(int missionId)
	{
		if (missionId == 3)
		{
			return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL01");
		}
		if (missionId == 4)
		{
			return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL02");
		}
		if (missionId == 248)
		{
			return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL05");
		}
		if (missionId == 249)
		{
			return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL06");
		}
		if (missionId == 181)
		{
			return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL03");
		}
		if (missionId != 201)
		{
			Debug.LogWarning(string.Format("GameUtils.GetTutorialCardRewardDetails(): no card reward details for mission {0}", missionId));
			return string.Empty;
		}
		return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL04");
	}

	// Token: 0x06000392 RID: 914 RVA: 0x0001107A File Offset: 0x0000F27A
	public static string GetCurrentTutorialCardRewardDetails()
	{
		return GameUtils.GetTutorialCardRewardDetails(GameMgr.Get().GetMissionId());
	}

	// Token: 0x06000393 RID: 915 RVA: 0x0001108B File Offset: 0x0000F28B
	public static int MissionSortComparison(ScenarioDbfRecord rec1, ScenarioDbfRecord rec2)
	{
		return rec1.SortOrder - rec2.SortOrder;
	}

	// Token: 0x06000394 RID: 916 RVA: 0x0001109A File Offset: 0x0000F29A
	public static List<FixedRewardActionDbfRecord> GetFixedActionRecords(FixedActionType actionType)
	{
		return GameDbf.GetIndex().GetFixedActionRecordsForType(actionType);
	}

	// Token: 0x06000395 RID: 917 RVA: 0x000110A8 File Offset: 0x0000F2A8
	public static FixedRewardDbfRecord GetFixedRewardForCard(string cardID, TAG_PREMIUM premium)
	{
		int num = GameUtils.TranslateCardIdToDbId(cardID);
		foreach (FixedRewardDbfRecord fixedRewardDbfRecord in GameDbf.FixedReward.GetRecords())
		{
			int cardId = fixedRewardDbfRecord.CardId;
			if (num == cardId)
			{
				int cardPremium = fixedRewardDbfRecord.CardPremium;
				if (premium == (TAG_PREMIUM)cardPremium)
				{
					return fixedRewardDbfRecord;
				}
			}
		}
		return null;
	}

	// Token: 0x06000396 RID: 918 RVA: 0x0001113C File Offset: 0x0000F33C
	public static List<FixedRewardMapDbfRecord> GetFixedRewardMapRecordsForAction(int actionID)
	{
		return GameDbf.GetIndex().GetFixedRewardMapRecordsForAction(actionID);
	}

	// Token: 0x06000397 RID: 919 RVA: 0x0001114C File Offset: 0x0000F34C
	public static bool IsMatchmadeGameType(GameType gameType)
	{
		switch (gameType)
		{
		case 5:
		case 7:
		case 8:
			return true;
		default:
			return gameType == 16 && (TavernBrawlManager.Get().CurrentMission() != null || true);
		}
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00011198 File Offset: 0x0000F398
	public static bool ShouldShowRankedMedals()
	{
		GameType gameType = GameMgr.Get().GetGameType();
		return GameUtils.ShouldShowRankedMedals(gameType);
	}

	// Token: 0x06000399 RID: 921 RVA: 0x000111B6 File Offset: 0x0000F3B6
	public static bool ShouldShowRankedMedals(GameType gameType)
	{
		return !DemoMgr.Get().IsExpoDemo() && gameType == 7;
	}

	// Token: 0x0600039A RID: 922 RVA: 0x000111D0 File Offset: 0x0000F3D0
	public static bool IsAnyTransitionActive()
	{
		SceneMgr sceneMgr = SceneMgr.Get();
		if (sceneMgr != null && sceneMgr.IsTransitionNowOrPending())
		{
			return true;
		}
		Box box = Box.Get();
		if (box != null && box.IsTransitioningToSceneMode())
		{
			return true;
		}
		LoadingScreen loadingScreen = LoadingScreen.Get();
		return loadingScreen != null && loadingScreen.IsTransitioning();
	}

	// Token: 0x0600039B RID: 923 RVA: 0x0001123C File Offset: 0x0000F43C
	public static void LogoutConfirmation()
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get((!Network.ShouldBeConnectedToAurora()) ? "GLOBAL_LOGIN_CONFIRM_TITLE" : "GLOBAL_LOGOUT_CONFIRM_TITLE");
		popupInfo.m_text = GameStrings.Get((!Network.ShouldBeConnectedToAurora()) ? "GLOBAL_LOGIN_CONFIRM_MESSAGE" : "GLOBAL_LOGOUT_CONFIRM_MESSAGE");
		popupInfo.m_alertTextAlignment = UberText.AlignmentOptions.Center;
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(GameUtils.OnLogoutConfirmationResponse);
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x0600039C RID: 924 RVA: 0x000112C9 File Offset: 0x0000F4C9
	private static void OnLogoutConfirmationResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CANCEL)
		{
			return;
		}
		GameMgr.Get().SetPendingAutoConcede(true);
		if (Network.ShouldBeConnectedToAurora())
		{
			WebAuth.ClearLoginData();
		}
		ApplicationMgr.Get().ResetAndForceLogin();
	}

	// Token: 0x0600039D RID: 925 RVA: 0x000112F8 File Offset: 0x0000F4F8
	public static int GetBoosterCount()
	{
		NetCache.NetCacheBoosters netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
		if (netObject == null)
		{
			return 0;
		}
		return netObject.GetTotalNumBoosters();
	}

	// Token: 0x0600039E RID: 926 RVA: 0x00011320 File Offset: 0x0000F520
	public static int GetBoosterCount(int boosterStackId)
	{
		NetCache.NetCacheBoosters netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
		if (netObject == null)
		{
			return 0;
		}
		NetCache.BoosterStack boosterStack = netObject.GetBoosterStack(boosterStackId);
		if (boosterStack == null)
		{
			return 0;
		}
		return boosterStack.Count;
	}

	// Token: 0x0600039F RID: 927 RVA: 0x00011356 File Offset: 0x0000F556
	public static bool HaveBoosters()
	{
		return GameUtils.GetBoosterCount() > 0;
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x00011360 File Offset: 0x0000F560
	public static PackOpeningRarity GetPackOpeningRarity(TAG_RARITY tag)
	{
		switch (tag)
		{
		case TAG_RARITY.COMMON:
			return PackOpeningRarity.COMMON;
		case TAG_RARITY.FREE:
			return PackOpeningRarity.COMMON;
		case TAG_RARITY.RARE:
			return PackOpeningRarity.RARE;
		case TAG_RARITY.EPIC:
			return PackOpeningRarity.EPIC;
		case TAG_RARITY.LEGENDARY:
			return PackOpeningRarity.LEGENDARY;
		default:
			return PackOpeningRarity.NONE;
		}
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x0001139B File Offset: 0x0000F59B
	public static List<BoosterDbfRecord> GetPackRecordsWithStorePrefab()
	{
		return GameDbf.Booster.GetRecords((BoosterDbfRecord r) => !string.IsNullOrEmpty(r.StorePrefab));
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x000113C4 File Offset: 0x0000F5C4
	public static List<AdventureDbfRecord> GetSortedAdventureRecordsWithStorePrefab()
	{
		List<AdventureDbfRecord> records = GameDbf.Adventure.GetRecords((AdventureDbfRecord r) => !string.IsNullOrEmpty(r.StorePrefab));
		records.Sort((AdventureDbfRecord l, AdventureDbfRecord r) => r.SortOrder - l.SortOrder);
		return records;
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x0001141D File Offset: 0x0000F61D
	public static List<AdventureDbfRecord> GetAdventureRecordsWithDefPrefab()
	{
		return GameDbf.Adventure.GetRecords((AdventureDbfRecord r) => !string.IsNullOrEmpty(r.AdventureDefPrefab));
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x00011446 File Offset: 0x0000F646
	public static List<AdventureDataDbfRecord> GetAdventureDataRecordsWithSubDefPrefab()
	{
		return GameDbf.AdventureData.GetRecords((AdventureDataDbfRecord r) => !string.IsNullOrEmpty(r.AdventureSubDefPrefab));
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x00011470 File Offset: 0x0000F670
	public static IEnumerable<int> GetSortedPackIds(bool ascending = true)
	{
		IEnumerable<BoosterDbfRecord> enumerable;
		if (ascending)
		{
			enumerable = Enumerable.OrderBy<BoosterDbfRecord, int>(GameDbf.Booster.GetRecords(), (BoosterDbfRecord b) => b.SortOrder);
		}
		else
		{
			enumerable = Enumerable.OrderByDescending<BoosterDbfRecord, int>(GameDbf.Booster.GetRecords(), (BoosterDbfRecord b) => b.SortOrder);
		}
		return Enumerable.Select<BoosterDbfRecord, int>(enumerable, (BoosterDbfRecord b) => b.ID);
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x00011505 File Offset: 0x0000F705
	public static bool IsFakePackOpeningEnabled()
	{
		return ApplicationMgr.IsInternal() && Options.Get().GetBool(Option.FAKE_PACK_OPENING);
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x0001151F File Offset: 0x0000F71F
	public static int GetFakePackCount()
	{
		if (!ApplicationMgr.IsInternal())
		{
			return 0;
		}
		return Options.Get().GetInt(Option.FAKE_PACK_COUNT);
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x0001153C File Offset: 0x0000F73C
	public static int GetBoardIdFromAssetName(string name)
	{
		foreach (BoardDbfRecord boardDbfRecord in GameDbf.Board.GetRecords())
		{
			string text = FileUtils.GameAssetPathToName(boardDbfRecord.Prefab);
			if (!(name != text))
			{
				return boardDbfRecord.ID;
			}
		}
		return 0;
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x000115C0 File Offset: 0x0000F7C0
	public static Object Instantiate(GameObject original, GameObject parent, bool withRotation = false)
	{
		if (original == null)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(original);
		GameUtils.SetParent(gameObject, parent, withRotation);
		return gameObject;
	}

	// Token: 0x060003AA RID: 938 RVA: 0x000115EC File Offset: 0x0000F7EC
	public static Object Instantiate(Component original, GameObject parent, bool withRotation = false)
	{
		if (original == null)
		{
			return null;
		}
		Component component = Object.Instantiate<Component>(original);
		GameUtils.SetParent(component, parent, withRotation);
		return component;
	}

	// Token: 0x060003AB RID: 939 RVA: 0x00011618 File Offset: 0x0000F818
	public static Object Instantiate(Object original)
	{
		if (original == null)
		{
			return null;
		}
		return Object.Instantiate(original);
	}

	// Token: 0x060003AC RID: 940 RVA: 0x0001163C File Offset: 0x0000F83C
	public static Object InstantiateGameObject(string name, GameObject parent = null, bool withRotation = false)
	{
		if (name == null)
		{
			return null;
		}
		GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(name), true, false);
		if (parent != null)
		{
			GameUtils.SetParent(gameObject, parent, withRotation);
		}
		return gameObject;
	}

	// Token: 0x060003AD RID: 941 RVA: 0x00011679 File Offset: 0x0000F879
	public static void SetParent(Component child, Component parent, bool withRotation = false)
	{
		GameUtils.SetParent(child.transform, parent.transform, withRotation);
	}

	// Token: 0x060003AE RID: 942 RVA: 0x0001168D File Offset: 0x0000F88D
	public static void SetParent(GameObject child, Component parent, bool withRotation = false)
	{
		GameUtils.SetParent(child.transform, parent.transform, withRotation);
	}

	// Token: 0x060003AF RID: 943 RVA: 0x000116A1 File Offset: 0x0000F8A1
	public static void SetParent(Component child, GameObject parent, bool withRotation = false)
	{
		GameUtils.SetParent(child.transform, parent.transform, withRotation);
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x000116B5 File Offset: 0x0000F8B5
	public static void SetParent(GameObject child, GameObject parent, bool withRotation = false)
	{
		GameUtils.SetParent(child.transform, parent.transform, withRotation);
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x000116CC File Offset: 0x0000F8CC
	private static void SetParent(Transform child, Transform parent, bool withRotation)
	{
		Vector3 localScale = child.localScale;
		Quaternion localRotation = child.localRotation;
		child.parent = parent;
		child.localPosition = Vector3.zero;
		child.localScale = localScale;
		if (withRotation)
		{
			child.localRotation = localRotation;
		}
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x00011710 File Offset: 0x0000F910
	public static void ResetTransform(GameObject obj)
	{
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localScale = Vector3.one;
		obj.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0001174D File Offset: 0x0000F94D
	public static void ResetTransform(Component comp)
	{
		GameUtils.ResetTransform(comp.gameObject);
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x0001175C File Offset: 0x0000F95C
	public static T LoadGameObjectWithComponent<T>(string assetPath) where T : Component
	{
		GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(assetPath), true, false);
		if (gameObject == null)
		{
			return (T)((object)null);
		}
		T component = gameObject.GetComponent<T>();
		if (component == null)
		{
			Debug.LogError(string.Format("{0} object does not contain {1} component.", assetPath, typeof(T)));
			Object.Destroy(gameObject);
			return (T)((object)null);
		}
		return component;
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x000117D0 File Offset: 0x0000F9D0
	public static void PlayCardEffectDefSounds(CardEffectDef cardEffectDef)
	{
		if (cardEffectDef == null)
		{
			return;
		}
		List<string> soundSpellPaths = cardEffectDef.m_SoundSpellPaths;
		foreach (string name2 in soundSpellPaths)
		{
			AssetLoader.Get().LoadSpell(name2, delegate(string name, GameObject go, object data)
			{
				if (go == null)
				{
					Debug.LogError(string.Format("Unable to load spell object: {0}", name));
					return;
				}
				GameObject destroyObj = go;
				CardSoundSpell component = go.GetComponent<CardSoundSpell>();
				if (component == null)
				{
					Debug.LogError(string.Format("Card sound spell component not found: {0}", name));
					Object.Destroy(destroyObj);
					return;
				}
				component.AddStateFinishedCallback(delegate(Spell spell, SpellStateType prevStateType, object userData)
				{
					if (spell.GetActiveState() == SpellStateType.NONE)
					{
						Object.Destroy(destroyObj);
					}
				});
				component.ForceDefaultAudioSource();
				component.Activate();
			}, null, false);
		}
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x00011858 File Offset: 0x0000FA58
	public static bool LoadCardDefEmoteSound(CardDef cardDef, EmoteType type, GameUtils.EmoteSoundLoaded callback)
	{
		if (callback == null)
		{
			Debug.LogError("No callback provided for LoadEmote!");
			return false;
		}
		if (cardDef == null || cardDef.m_EmoteDefs == null)
		{
			return false;
		}
		EmoteEntryDef emoteEntryDef = cardDef.m_EmoteDefs.Find((EmoteEntryDef e) => e.m_emoteType == type);
		if (emoteEntryDef == null)
		{
			return false;
		}
		AssetLoader.Get().LoadSpell(emoteEntryDef.m_emoteSoundSpellPath, delegate(string name, GameObject go, object data)
		{
			if (go == null)
			{
				callback(null);
				return;
			}
			callback(go.GetComponent<CardSoundSpell>());
		}, null, false);
		return true;
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x000118EC File Offset: 0x0000FAEC
	public static void SetAutomationName(GameObject obj, params object[] formatArgs)
	{
		string text = obj.name;
		for (int i = 0; i < formatArgs.Length; i++)
		{
			text = text + "_" + formatArgs[i].ToString();
		}
		obj.name = text;
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x0001192F File Offset: 0x0000FB2F
	public static bool HasSeenStandardModeTutorial()
	{
		return Options.Get().GetBool(Option.HAS_SEEN_STANDARD_MODE_TUTORIAL, false);
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x00011944 File Offset: 0x0000FB44
	public static bool ShouldShowSetRotationIntro()
	{
		Options options = Options.Get();
		if (options == null)
		{
			Debug.LogError("ShouldShowSetRotationIntro: Options is NULL!");
			return false;
		}
		if (options.HasOption(Option.SHOW_SET_ROTATION_INTRO_VISUALS) && options.GetBool(Option.SHOW_SET_ROTATION_INTRO_VISUALS))
		{
			return true;
		}
		SpecialEventManager specialEventManager = SpecialEventManager.Get();
		if (specialEventManager == null)
		{
			Debug.LogError("ShouldShowSetRotationIntro: SpecialEventManager is NULL!");
			return false;
		}
		if (specialEventManager.IsEventActive(SpecialEventType.SPECIAL_EVENT_SET_ROTATION_2016, false) || specialEventManager.IsEventActive(SpecialEventType.SPECIAL_EVENT_SET_ROTATION_2017, false))
		{
			CollectionManager collectionManager = CollectionManager.Get();
			if (collectionManager == null)
			{
				Debug.LogError("ShouldShowSetRotationIntro: CollectionManager is NULL!");
				return false;
			}
			if (!collectionManager.ShouldAccountSeeStandardWild())
			{
				return false;
			}
			if (options.GetInt(Option.SET_ROTATION_INTRO_PROGRESS, 0) == 0 || !GameUtils.HasSeenStandardModeTutorial())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400015B RID: 347
	private static KeyValuePair<float, HashSet<TAG_CARD_SET>> s_cachedRotatedCardSets = new KeyValuePair<float, HashSet<TAG_CARD_SET>>(0f, null);

	// Token: 0x0400015C RID: 348
	private static KeyValuePair<float, HashSet<int>> s_cachedRotatedIndividualCards = new KeyValuePair<float, HashSet<int>>(0f, null);

	// Token: 0x0400015D RID: 349
	private static KeyValuePair<float, HashSet<AdventureDbId>> s_cachedRotatedAdventures = new KeyValuePair<float, HashSet<AdventureDbId>>(0f, null);

	// Token: 0x0400015E RID: 350
	private static KeyValuePair<float, HashSet<BoosterDbId>> s_cachedRotatedBoosters = new KeyValuePair<float, HashSet<BoosterDbId>>(0f, null);

	// Token: 0x02000185 RID: 389
	// (Invoke) Token: 0x06001661 RID: 5729
	public delegate void EmoteSoundLoaded(CardSoundSpell emoteObj);
}
