using System;
using UnityEngine;

// Token: 0x0200032A RID: 810
public class ActorNames
{
	// Token: 0x06002A27 RID: 10791 RVA: 0x000CE78C File Offset: 0x000CC98C
	public static string GetZoneActor(TAG_CARDTYPE cardType, TAG_CLASS classTag, TAG_ZONE zoneTag, Player controller, TAG_PREMIUM premium)
	{
		switch (zoneTag)
		{
		case TAG_ZONE.PLAY:
			if (cardType == TAG_CARDTYPE.MINION)
			{
				return ActorNames.GetNameWithPremiumType("Card_Play_Ally", premium);
			}
			if (cardType == TAG_CARDTYPE.WEAPON)
			{
				return ActorNames.GetNameWithPremiumType("Card_Play_Weapon", premium);
			}
			if (cardType == TAG_CARDTYPE.SPELL)
			{
				return "Card_Invisible";
			}
			if (cardType == TAG_CARDTYPE.HERO)
			{
				return "Card_Play_Hero";
			}
			if (cardType == TAG_CARDTYPE.HERO_POWER)
			{
				return ActorNames.GetNameWithPremiumType("Card_Play_HeroPower", premium);
			}
			if (cardType == TAG_CARDTYPE.ENCHANTMENT)
			{
				return "Card_Play_Enchantment";
			}
			break;
		case TAG_ZONE.DECK:
		case TAG_ZONE.REMOVEDFROMGAME:
		case TAG_ZONE.SETASIDE:
			return "Card_Invisible";
		case TAG_ZONE.HAND:
			if (controller == null || (!controller.IsFriendlySide() && !controller.HasTag(GAME_TAG.HAND_REVEALED) && !SpectatorManager.Get().IsSpectatingOpposingSide()))
			{
				return "Card_Hidden";
			}
			if (cardType == TAG_CARDTYPE.MINION)
			{
				return ActorNames.GetNameWithPremiumType("Card_Hand_Ally", premium);
			}
			if (cardType == TAG_CARDTYPE.WEAPON)
			{
				return ActorNames.GetNameWithPremiumType("Card_Hand_Weapon", premium);
			}
			if (cardType == TAG_CARDTYPE.SPELL)
			{
				return ActorNames.GetNameWithPremiumType("Card_Hand_Ability", premium);
			}
			if (cardType == TAG_CARDTYPE.HERO_POWER)
			{
				return ActorNames.GetNameWithPremiumType("History_HeroPower", premium);
			}
			break;
		case TAG_ZONE.GRAVEYARD:
			if (cardType == TAG_CARDTYPE.MINION)
			{
				return ActorNames.GetNameWithPremiumType("Card_Hand_Ally", premium);
			}
			if (cardType == TAG_CARDTYPE.WEAPON)
			{
				return ActorNames.GetNameWithPremiumType("Card_Hand_Weapon", premium);
			}
			if (cardType == TAG_CARDTYPE.SPELL)
			{
				return ActorNames.GetNameWithPremiumType("Card_Hand_Ability", premium);
			}
			if (cardType == TAG_CARDTYPE.HERO)
			{
				return "Card_Play_Hero";
			}
			break;
		case TAG_ZONE.SECRET:
			if (classTag == TAG_CLASS.HUNTER)
			{
				return "Card_Play_Secret_Hunter";
			}
			if (classTag == TAG_CLASS.MAGE)
			{
				return "Card_Play_Secret_Mage";
			}
			if (classTag == TAG_CLASS.PALADIN)
			{
				return "Card_Play_Secret_Paladin";
			}
			return "Card_Play_Secret_Mage";
		}
		Debug.LogWarningFormat("ActorNames.GetZoneActor() - Can't determine actor for {0}. Returning {1} instead.", new object[]
		{
			cardType,
			"Card_Hidden"
		});
		return "Card_Hidden";
	}

	// Token: 0x06002A28 RID: 10792 RVA: 0x000CE968 File Offset: 0x000CCB68
	private static bool ShouldObfuscate(Entity entity)
	{
		return entity.GetController() != null && !entity.GetController().IsFriendlySide() && entity.IsObfuscated();
	}

	// Token: 0x06002A29 RID: 10793 RVA: 0x000CE9A0 File Offset: 0x000CCBA0
	public static string GetZoneActor(Entity entity, TAG_ZONE zoneTag)
	{
		if (ActorNames.ShouldObfuscate(entity) && zoneTag == TAG_ZONE.PLAY)
		{
			return "Card_Play_Obfuscated";
		}
		return ActorNames.GetZoneActor(entity.GetCardType(), entity.GetClass(), zoneTag, entity.GetController(), entity.GetPremiumType());
	}

	// Token: 0x06002A2A RID: 10794 RVA: 0x000CE9E4 File Offset: 0x000CCBE4
	public static string GetZoneActor(EntityDef entityDef, TAG_ZONE zoneTag)
	{
		return ActorNames.GetZoneActor(entityDef.GetCardType(), entityDef.GetClass(), zoneTag, null, TAG_PREMIUM.NORMAL);
	}

	// Token: 0x06002A2B RID: 10795 RVA: 0x000CEA08 File Offset: 0x000CCC08
	public static string GetZoneActor(EntityDef entityDef, TAG_ZONE zoneTag, TAG_PREMIUM premium)
	{
		return ActorNames.GetZoneActor(entityDef.GetCardType(), entityDef.GetClass(), zoneTag, null, premium);
	}

	// Token: 0x06002A2C RID: 10796 RVA: 0x000CEA2C File Offset: 0x000CCC2C
	public static string GetHandActor(TAG_CARDTYPE cardType, TAG_PREMIUM premiumType)
	{
		switch (cardType)
		{
		case TAG_CARDTYPE.HERO:
			return "History_Hero";
		case TAG_CARDTYPE.MINION:
			return ActorNames.GetNameWithPremiumType("Card_Hand_Ally", premiumType);
		case TAG_CARDTYPE.SPELL:
			return ActorNames.GetNameWithPremiumType("Card_Hand_Ability", premiumType);
		case TAG_CARDTYPE.WEAPON:
			return ActorNames.GetNameWithPremiumType("Card_Hand_Weapon", premiumType);
		case TAG_CARDTYPE.HERO_POWER:
			return "History_HeroPower";
		}
		return "Card_Hidden";
	}

	// Token: 0x06002A2D RID: 10797 RVA: 0x000CEA9D File Offset: 0x000CCC9D
	public static string GetHandActor(TAG_CARDTYPE cardType)
	{
		return ActorNames.GetHandActor(cardType, TAG_PREMIUM.NORMAL);
	}

	// Token: 0x06002A2E RID: 10798 RVA: 0x000CEAA6 File Offset: 0x000CCCA6
	public static string GetHandActor(Entity entity)
	{
		return ActorNames.GetHandActor(entity.GetCardType(), entity.GetPremiumType());
	}

	// Token: 0x06002A2F RID: 10799 RVA: 0x000CEAB9 File Offset: 0x000CCCB9
	public static string GetHandActor(EntityDef entityDef)
	{
		return ActorNames.GetHandActor(entityDef.GetCardType(), TAG_PREMIUM.NORMAL);
	}

	// Token: 0x06002A30 RID: 10800 RVA: 0x000CEAC7 File Offset: 0x000CCCC7
	public static string GetHandActor(EntityDef entityDef, TAG_PREMIUM premiumType)
	{
		return ActorNames.GetHandActor(entityDef.GetCardType(), premiumType);
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x000CEAD5 File Offset: 0x000CCCD5
	public static string GetHeroSkinOrHandActor(TAG_CARDTYPE type, TAG_PREMIUM premium)
	{
		if (type == TAG_CARDTYPE.HERO)
		{
			return "Card_Hero_Skin";
		}
		return ActorNames.GetHandActor(type, premium);
	}

	// Token: 0x06002A32 RID: 10802 RVA: 0x000CEAEB File Offset: 0x000CCCEB
	public static string GetBigCardActor(Entity entity)
	{
		return ActorNames.GetHistoryActor(entity);
	}

	// Token: 0x06002A33 RID: 10803 RVA: 0x000CEAF4 File Offset: 0x000CCCF4
	public static string GetHistoryActor(Entity entity)
	{
		if (entity.IsSecret() && entity.IsHidden())
		{
			return ActorNames.GetHistorySecretActor(entity);
		}
		if (ActorNames.ShouldObfuscate(entity))
		{
			return "History_Obfuscated";
		}
		TAG_CARDTYPE cardType = entity.GetCardType();
		TAG_PREMIUM premiumType = entity.GetPremiumType();
		TAG_CARDTYPE tag_CARDTYPE = cardType;
		if (tag_CARDTYPE == TAG_CARDTYPE.HERO)
		{
			return "History_Hero";
		}
		if (tag_CARDTYPE != TAG_CARDTYPE.HERO_POWER)
		{
			return ActorNames.GetHandActor(entity);
		}
		if (entity.GetController().IsFriendlySide())
		{
			return ActorNames.GetNameWithPremiumType("History_HeroPower", premiumType);
		}
		return ActorNames.GetNameWithPremiumType("History_HeroPower_Opponent", premiumType);
	}

	// Token: 0x06002A34 RID: 10804 RVA: 0x000CEB88 File Offset: 0x000CCD88
	public static string GetHistorySecretActor(Entity entity)
	{
		TAG_CLASS @class = entity.GetClass();
		if (@class == TAG_CLASS.HUNTER)
		{
			return "History_Secret_Hunter";
		}
		if (@class == TAG_CLASS.MAGE)
		{
			return "History_Secret_Mage";
		}
		if (@class == TAG_CLASS.PALADIN)
		{
			return "History_Secret_Paladin";
		}
		Debug.LogWarning(string.Format("ActorNames.GetHistorySecretActor() - No actor for class {0}. Returning {1} instead.", @class, "History_Secret_Mage"));
		return "History_Secret_Mage";
	}

	// Token: 0x06002A35 RID: 10805 RVA: 0x000CEBE2 File Offset: 0x000CCDE2
	public static string GetNameWithPremiumType(string actorName, TAG_PREMIUM premiumType)
	{
		if (premiumType == TAG_PREMIUM.GOLDEN)
		{
			return string.Format("{0}_Premium", actorName);
		}
		return actorName;
	}

	// Token: 0x04001896 RID: 6294
	public const string INVISIBLE = "Card_Invisible";

	// Token: 0x04001897 RID: 6295
	public const string HIDDEN = "Card_Hidden";

	// Token: 0x04001898 RID: 6296
	public const string HAND_MINION = "Card_Hand_Ally";

	// Token: 0x04001899 RID: 6297
	public const string HAND_SPELL = "Card_Hand_Ability";

	// Token: 0x0400189A RID: 6298
	public const string HAND_WEAPON = "Card_Hand_Weapon";

	// Token: 0x0400189B RID: 6299
	public const string HAND_FATIGUE = "Card_Hand_Fatigue";

	// Token: 0x0400189C RID: 6300
	public const string PLAY_MINION = "Card_Play_Ally";

	// Token: 0x0400189D RID: 6301
	public const string PLAY_WEAPON = "Card_Play_Weapon";

	// Token: 0x0400189E RID: 6302
	public const string PLAY_HERO = "Card_Play_Hero";

	// Token: 0x0400189F RID: 6303
	public const string PLAY_HERO_POWER = "Card_Play_HeroPower";

	// Token: 0x040018A0 RID: 6304
	public const string PLAY_ENCHANTMENT = "Card_Play_Enchantment";

	// Token: 0x040018A1 RID: 6305
	public const string PLAY_OBFUSCATED = "Card_Play_Obfuscated";

	// Token: 0x040018A2 RID: 6306
	public const string SECRET = "Card_Play_Secret_Mage";

	// Token: 0x040018A3 RID: 6307
	public const string SECRET_HUNTER = "Card_Play_Secret_Hunter";

	// Token: 0x040018A4 RID: 6308
	public const string SECRET_MAGE = "Card_Play_Secret_Mage";

	// Token: 0x040018A5 RID: 6309
	public const string SECRET_PALADIN = "Card_Play_Secret_Paladin";

	// Token: 0x040018A6 RID: 6310
	public const string TOOLTIP = "CardTooltip";

	// Token: 0x040018A7 RID: 6311
	public const string HISTORY_HERO = "History_Hero";

	// Token: 0x040018A8 RID: 6312
	public const string HISTORY_HERO_POWER = "History_HeroPower";

	// Token: 0x040018A9 RID: 6313
	public const string HISTORY_HERO_POWER_OPPONENT = "History_HeroPower_Opponent";

	// Token: 0x040018AA RID: 6314
	public const string HISTORY_SECRET_HUNTER = "History_Secret_Hunter";

	// Token: 0x040018AB RID: 6315
	public const string HISTORY_SECRET_MAGE = "History_Secret_Mage";

	// Token: 0x040018AC RID: 6316
	public const string HISTORY_SECRET_PALADIN = "History_Secret_Paladin";

	// Token: 0x040018AD RID: 6317
	public const string HISTORY_OBFUSCATED = "History_Obfuscated";

	// Token: 0x040018AE RID: 6318
	public const string HEROPICKER_HERO = "HeroPicker_Hero";

	// Token: 0x040018AF RID: 6319
	public const string COLLECTION_CARD_BACK = "Collection_Card_Back";

	// Token: 0x040018B0 RID: 6320
	public const string COLLECTION_DECK_TILE = "DeckCardBar";

	// Token: 0x040018B1 RID: 6321
	public const string COLLECTION_DECK_TILE_PHONE = "DeckCardBar_phone";

	// Token: 0x040018B2 RID: 6322
	public const string HERO_SKIN = "Card_Hero_Skin";
}
