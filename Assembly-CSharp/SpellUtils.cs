using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003E4 RID: 996
public class SpellUtils
{
	// Token: 0x06003364 RID: 13156 RVA: 0x000FFE44 File Offset: 0x000FE044
	public static SpellClassTag ConvertClassTagToSpellEnum(TAG_CLASS classTag)
	{
		switch (classTag)
		{
		case TAG_CLASS.DEATHKNIGHT:
			return SpellClassTag.DEATHKNIGHT;
		case TAG_CLASS.DRUID:
			return SpellClassTag.DRUID;
		case TAG_CLASS.HUNTER:
			return SpellClassTag.HUNTER;
		case TAG_CLASS.MAGE:
			return SpellClassTag.MAGE;
		case TAG_CLASS.PALADIN:
			return SpellClassTag.PALADIN;
		case TAG_CLASS.PRIEST:
			return SpellClassTag.PRIEST;
		case TAG_CLASS.ROGUE:
			return SpellClassTag.ROGUE;
		case TAG_CLASS.SHAMAN:
			return SpellClassTag.SHAMAN;
		case TAG_CLASS.WARLOCK:
			return SpellClassTag.WARLOCK;
		case TAG_CLASS.WARRIOR:
			return SpellClassTag.WARRIOR;
		default:
			return SpellClassTag.NONE;
		}
	}

	// Token: 0x06003365 RID: 13157 RVA: 0x000FFEA0 File Offset: 0x000FE0A0
	public static Player.Side ConvertSpellSideToPlayerSide(Spell spell, SpellPlayerSide spellSide)
	{
		Card sourceCard = spell.GetSourceCard();
		Entity entity = sourceCard.GetEntity();
		switch (spellSide)
		{
		case SpellPlayerSide.FRIENDLY:
			return Player.Side.FRIENDLY;
		case SpellPlayerSide.OPPONENT:
			return Player.Side.OPPOSING;
		case SpellPlayerSide.SOURCE:
			if (entity.IsControlledByFriendlySidePlayer())
			{
				return Player.Side.FRIENDLY;
			}
			return Player.Side.OPPOSING;
		case SpellPlayerSide.TARGET:
			if (entity.IsControlledByFriendlySidePlayer())
			{
				return Player.Side.OPPOSING;
			}
			return Player.Side.FRIENDLY;
		default:
			return Player.Side.NEUTRAL;
		}
	}

	// Token: 0x06003366 RID: 13158 RVA: 0x000FFF00 File Offset: 0x000FE100
	public static List<Zone> FindZonesFromTag(SpellZoneTag zoneTag)
	{
		ZoneMgr zoneMgr = ZoneMgr.Get();
		if (zoneMgr == null)
		{
			return null;
		}
		switch (zoneTag)
		{
		case SpellZoneTag.PLAY:
			return zoneMgr.FindZonesOfType<Zone, ZonePlay>();
		case SpellZoneTag.HERO:
			return zoneMgr.FindZonesOfType<Zone, ZoneHero>();
		case SpellZoneTag.HERO_POWER:
			return zoneMgr.FindZonesOfType<Zone, ZoneHeroPower>();
		case SpellZoneTag.WEAPON:
			return zoneMgr.FindZonesOfType<Zone, ZoneWeapon>();
		case SpellZoneTag.DECK:
			return zoneMgr.FindZonesOfType<Zone, ZoneDeck>();
		case SpellZoneTag.HAND:
			return zoneMgr.FindZonesOfType<Zone, ZoneHand>();
		case SpellZoneTag.GRAVEYARD:
			return zoneMgr.FindZonesOfType<Zone, ZoneGraveyard>();
		case SpellZoneTag.SECRET:
			return zoneMgr.FindZonesOfType<Zone, ZoneSecret>();
		default:
			Debug.LogWarning(string.Format("SpellUtils.FindZonesFromTag() - unhandled zoneTag {0}", zoneTag));
			return null;
		}
	}

	// Token: 0x06003367 RID: 13159 RVA: 0x000FFFA0 File Offset: 0x000FE1A0
	public static List<Zone> FindZonesFromTag(Spell spell, SpellZoneTag zoneTag, SpellPlayerSide spellSide)
	{
		if (ZoneMgr.Get() == null)
		{
			return null;
		}
		if (spellSide == SpellPlayerSide.NEUTRAL)
		{
			return null;
		}
		if (spellSide == SpellPlayerSide.BOTH)
		{
			return SpellUtils.FindZonesFromTag(zoneTag);
		}
		Player.Side side = SpellUtils.ConvertSpellSideToPlayerSide(spell, spellSide);
		switch (zoneTag)
		{
		case SpellZoneTag.PLAY:
			return ZoneMgr.Get().FindZonesOfType<Zone, ZonePlay>(side);
		case SpellZoneTag.HERO:
			return ZoneMgr.Get().FindZonesOfType<Zone, ZoneHero>(side);
		case SpellZoneTag.HERO_POWER:
			return ZoneMgr.Get().FindZonesOfType<Zone, ZoneHeroPower>(side);
		case SpellZoneTag.WEAPON:
			return ZoneMgr.Get().FindZonesOfType<Zone, ZoneWeapon>(side);
		case SpellZoneTag.DECK:
			return ZoneMgr.Get().FindZonesOfType<Zone, ZoneDeck>(side);
		case SpellZoneTag.HAND:
			return ZoneMgr.Get().FindZonesOfType<Zone, ZoneHand>(side);
		case SpellZoneTag.GRAVEYARD:
			return ZoneMgr.Get().FindZonesOfType<Zone, ZoneGraveyard>(side);
		case SpellZoneTag.SECRET:
			return ZoneMgr.Get().FindZonesOfType<Zone, ZoneSecret>(side);
		default:
			Debug.LogWarning(string.Format("SpellUtils.FindZonesFromTag() - Unhandled zoneTag {0}. spellSide={1} playerSide={2}", zoneTag, spellSide, side));
			return null;
		}
	}

	// Token: 0x06003368 RID: 13160 RVA: 0x00100090 File Offset: 0x000FE290
	public static Transform GetLocationTransform(Spell spell)
	{
		GameObject locationObject = SpellUtils.GetLocationObject(spell);
		return (!(locationObject == null)) ? locationObject.transform : null;
	}

	// Token: 0x06003369 RID: 13161 RVA: 0x001000BC File Offset: 0x000FE2BC
	public static GameObject GetLocationObject(Spell spell)
	{
		SpellLocation location = spell.GetLocation();
		if (location == SpellLocation.NONE)
		{
			return null;
		}
		GameObject gameObject = null;
		if (location == SpellLocation.SOURCE)
		{
			gameObject = spell.GetSource();
		}
		else if (location == SpellLocation.SOURCE_AUTO)
		{
			gameObject = SpellUtils.FindSourceAutoObjectForSpell(spell);
		}
		else if (location == SpellLocation.SOURCE_HERO)
		{
			Card sourceCard = spell.GetSourceCard();
			Card card = SpellUtils.FindHeroCard(sourceCard);
			if (card == null)
			{
				return null;
			}
			gameObject = card.gameObject;
		}
		else if (location == SpellLocation.SOURCE_HERO_POWER)
		{
			Card sourceCard2 = spell.GetSourceCard();
			Card card2 = SpellUtils.FindHeroPowerCard(sourceCard2);
			if (card2 == null)
			{
				return null;
			}
			gameObject = card2.gameObject;
		}
		else if (location == SpellLocation.SOURCE_PLAY_ZONE)
		{
			Card sourceCard3 = spell.GetSourceCard();
			if (sourceCard3 == null)
			{
				return null;
			}
			Entity entity = sourceCard3.GetEntity();
			Player controller = entity.GetController();
			ZonePlay zonePlay = ZoneMgr.Get().FindZoneOfType<ZonePlay>(controller.GetSide());
			if (zonePlay == null)
			{
				return null;
			}
			gameObject = zonePlay.gameObject;
		}
		else if (location == SpellLocation.TARGET)
		{
			gameObject = spell.GetVisualTarget();
		}
		else if (location == SpellLocation.TARGET_AUTO)
		{
			gameObject = SpellUtils.FindTargetAutoObjectForSpell(spell);
		}
		else if (location == SpellLocation.TARGET_HERO)
		{
			Card visualTargetCard = spell.GetVisualTargetCard();
			Card card3 = SpellUtils.FindHeroCard(visualTargetCard);
			if (card3 == null)
			{
				return null;
			}
			gameObject = card3.gameObject;
		}
		else if (location == SpellLocation.TARGET_HERO_POWER)
		{
			Card visualTargetCard2 = spell.GetVisualTargetCard();
			Card card4 = SpellUtils.FindHeroPowerCard(visualTargetCard2);
			if (card4 == null)
			{
				return null;
			}
			gameObject = card4.gameObject;
		}
		else if (location == SpellLocation.TARGET_PLAY_ZONE)
		{
			Card visualTargetCard3 = spell.GetVisualTargetCard();
			if (visualTargetCard3 == null)
			{
				return null;
			}
			Entity entity2 = visualTargetCard3.GetEntity();
			Player controller2 = entity2.GetController();
			ZonePlay zonePlay2 = ZoneMgr.Get().FindZoneOfType<ZonePlay>(controller2.GetSide());
			if (zonePlay2 == null)
			{
				return null;
			}
			gameObject = zonePlay2.gameObject;
		}
		else if (location == SpellLocation.BOARD)
		{
			if (Board.Get() == null)
			{
				return null;
			}
			gameObject = Board.Get().gameObject;
		}
		else if (location == SpellLocation.FRIENDLY_HERO)
		{
			Player player = SpellUtils.FindFriendlyPlayer(spell);
			if (player == null)
			{
				return null;
			}
			Card heroCard = player.GetHeroCard();
			if (!heroCard)
			{
				return null;
			}
			gameObject = heroCard.gameObject;
		}
		else if (location == SpellLocation.FRIENDLY_HERO_POWER)
		{
			Player player2 = SpellUtils.FindFriendlyPlayer(spell);
			if (player2 == null)
			{
				return null;
			}
			Card heroPowerCard = player2.GetHeroPowerCard();
			if (!heroPowerCard)
			{
				return null;
			}
			gameObject = heroPowerCard.gameObject;
		}
		else if (location == SpellLocation.FRIENDLY_PLAY_ZONE)
		{
			ZonePlay zonePlay3 = SpellUtils.FindFriendlyPlayZone(spell);
			if (!zonePlay3)
			{
				return null;
			}
			gameObject = zonePlay3.gameObject;
		}
		else if (location == SpellLocation.OPPONENT_HERO)
		{
			Player player3 = SpellUtils.FindOpponentPlayer(spell);
			if (player3 == null)
			{
				return null;
			}
			Card heroCard2 = player3.GetHeroCard();
			if (!heroCard2)
			{
				return null;
			}
			gameObject = heroCard2.gameObject;
		}
		else if (location == SpellLocation.OPPONENT_HERO_POWER)
		{
			Player player4 = SpellUtils.FindOpponentPlayer(spell);
			if (player4 == null)
			{
				return null;
			}
			Card heroPowerCard2 = player4.GetHeroPowerCard();
			if (!heroPowerCard2)
			{
				return null;
			}
			gameObject = heroPowerCard2.gameObject;
		}
		else if (location == SpellLocation.OPPONENT_PLAY_ZONE)
		{
			ZonePlay zonePlay4 = SpellUtils.FindOpponentPlayZone(spell);
			if (!zonePlay4)
			{
				return null;
			}
			gameObject = zonePlay4.gameObject;
		}
		else if (location == SpellLocation.CHOSEN_TARGET)
		{
			Card powerTargetCard = spell.GetPowerTargetCard();
			if (powerTargetCard == null)
			{
				return null;
			}
			gameObject = powerTargetCard.gameObject;
		}
		if (gameObject == null)
		{
			return null;
		}
		string locationTransformName = spell.GetLocationTransformName();
		if (!string.IsNullOrEmpty(locationTransformName))
		{
			GameObject gameObject2 = SceneUtils.FindChildBySubstring(gameObject, locationTransformName);
			if (gameObject2 != null)
			{
				return gameObject2;
			}
		}
		return gameObject;
	}

	// Token: 0x0600336A RID: 13162 RVA: 0x00100489 File Offset: 0x000FE689
	public static bool SetPositionFromLocation(Spell spell)
	{
		return SpellUtils.SetPositionFromLocation(spell, false);
	}

	// Token: 0x0600336B RID: 13163 RVA: 0x00100494 File Offset: 0x000FE694
	public static bool SetPositionFromLocation(Spell spell, bool setParent)
	{
		Transform locationTransform = SpellUtils.GetLocationTransform(spell);
		if (locationTransform == null)
		{
			return false;
		}
		if (setParent)
		{
			spell.transform.parent = locationTransform;
		}
		spell.transform.position = locationTransform.position;
		return true;
	}

	// Token: 0x0600336C RID: 13164 RVA: 0x001004DC File Offset: 0x000FE6DC
	public static bool SetOrientationFromFacing(Spell spell)
	{
		SpellFacing facing = spell.GetFacing();
		if (facing == SpellFacing.NONE)
		{
			return false;
		}
		SpellFacingOptions spellFacingOptions = spell.GetFacingOptions();
		if (spellFacingOptions == null)
		{
			spellFacingOptions = new SpellFacingOptions();
		}
		if (facing == SpellFacing.SAME_AS_SOURCE)
		{
			GameObject source = spell.GetSource();
			if (source == null)
			{
				return false;
			}
			SpellUtils.FaceSameAs(spell, source, spellFacingOptions);
		}
		else if (facing == SpellFacing.SAME_AS_SOURCE_AUTO)
		{
			GameObject gameObject = SpellUtils.FindSourceAutoObjectForSpell(spell);
			if (gameObject == null)
			{
				return false;
			}
			SpellUtils.FaceSameAs(spell, gameObject, spellFacingOptions);
		}
		else if (facing == SpellFacing.SAME_AS_SOURCE_HERO)
		{
			Card sourceCard = spell.GetSourceCard();
			Card card = SpellUtils.FindHeroCard(sourceCard);
			if (card == null)
			{
				return false;
			}
			SpellUtils.FaceSameAs(spell, card, spellFacingOptions);
		}
		else if (facing == SpellFacing.TOWARDS_SOURCE)
		{
			GameObject source2 = spell.GetSource();
			if (source2 == null)
			{
				return false;
			}
			SpellUtils.FaceTowards(spell, source2, spellFacingOptions);
		}
		else if (facing == SpellFacing.TOWARDS_SOURCE_AUTO)
		{
			GameObject gameObject2 = SpellUtils.FindSourceAutoObjectForSpell(spell);
			if (gameObject2 == null)
			{
				return false;
			}
			SpellUtils.FaceTowards(spell, gameObject2, spellFacingOptions);
		}
		else if (facing == SpellFacing.TOWARDS_SOURCE_HERO)
		{
			Card sourceCard2 = spell.GetSourceCard();
			Card card2 = SpellUtils.FindHeroCard(sourceCard2);
			if (card2 == null)
			{
				return false;
			}
			SpellUtils.FaceTowards(spell, card2, spellFacingOptions);
		}
		else if (facing == SpellFacing.TOWARDS_TARGET)
		{
			GameObject visualTarget = spell.GetVisualTarget();
			if (visualTarget == null)
			{
				return false;
			}
			SpellUtils.FaceTowards(spell, visualTarget, spellFacingOptions);
		}
		else if (facing == SpellFacing.TOWARDS_TARGET_HERO)
		{
			Card card3 = SpellUtils.FindBestTargetCard(spell);
			Card card4 = SpellUtils.FindHeroCard(card3);
			if (card4 == null)
			{
				return false;
			}
			SpellUtils.FaceTowards(spell, card4, spellFacingOptions);
		}
		else if (facing == SpellFacing.TOWARDS_CHOSEN_TARGET)
		{
			Card powerTargetCard = spell.GetPowerTargetCard();
			if (powerTargetCard == null)
			{
				return false;
			}
			SpellUtils.FaceTowards(spell, powerTargetCard, spellFacingOptions);
		}
		else if (facing == SpellFacing.OPPOSITE_OF_SOURCE)
		{
			GameObject source3 = spell.GetSource();
			if (source3 == null)
			{
				return false;
			}
			SpellUtils.FaceOppositeOf(spell, source3, spellFacingOptions);
		}
		else if (facing == SpellFacing.OPPOSITE_OF_SOURCE_AUTO)
		{
			GameObject gameObject3 = SpellUtils.FindSourceAutoObjectForSpell(spell);
			if (gameObject3 == null)
			{
				return false;
			}
			SpellUtils.FaceOppositeOf(spell, gameObject3, spellFacingOptions);
		}
		else
		{
			if (facing != SpellFacing.OPPOSITE_OF_SOURCE_HERO)
			{
				return false;
			}
			Card sourceCard3 = spell.GetSourceCard();
			Card card5 = SpellUtils.FindHeroCard(sourceCard3);
			if (card5 == null)
			{
				return false;
			}
			SpellUtils.FaceOppositeOf(spell, card5, spellFacingOptions);
		}
		return true;
	}

	// Token: 0x0600336D RID: 13165 RVA: 0x00100740 File Offset: 0x000FE940
	public static Player FindFriendlyPlayer(Spell spell)
	{
		if (spell == null)
		{
			return null;
		}
		Card sourceCard = spell.GetSourceCard();
		if (sourceCard == null)
		{
			return null;
		}
		Entity entity = sourceCard.GetEntity();
		return entity.GetController();
	}

	// Token: 0x0600336E RID: 13166 RVA: 0x00100780 File Offset: 0x000FE980
	public static Player FindOpponentPlayer(Spell spell)
	{
		Player player = SpellUtils.FindFriendlyPlayer(spell);
		if (player == null)
		{
			return null;
		}
		return GameState.Get().GetFirstOpponentPlayer(player);
	}

	// Token: 0x0600336F RID: 13167 RVA: 0x001007A8 File Offset: 0x000FE9A8
	public static ZonePlay FindFriendlyPlayZone(Spell spell)
	{
		Player player = SpellUtils.FindFriendlyPlayer(spell);
		if (player == null)
		{
			return null;
		}
		return ZoneMgr.Get().FindZoneOfType<ZonePlay>(player.GetSide());
	}

	// Token: 0x06003370 RID: 13168 RVA: 0x001007D4 File Offset: 0x000FE9D4
	public static ZonePlay FindOpponentPlayZone(Spell spell)
	{
		Player player = SpellUtils.FindOpponentPlayer(spell);
		if (player == null)
		{
			return null;
		}
		return ZoneMgr.Get().FindZoneOfType<ZonePlay>(player.GetSide());
	}

	// Token: 0x06003371 RID: 13169 RVA: 0x00100800 File Offset: 0x000FEA00
	public static Zone FindTargetZone(Spell spell)
	{
		Card targetCard = spell.GetTargetCard();
		if (targetCard == null)
		{
			return null;
		}
		Entity entity = targetCard.GetEntity();
		return ZoneMgr.Get().FindZoneForEntity(entity);
	}

	// Token: 0x06003372 RID: 13170 RVA: 0x00100834 File Offset: 0x000FEA34
	public static Actor GetParentActor(Spell spell)
	{
		return SceneUtils.FindComponentInThisOrParents<Actor>(spell.gameObject);
	}

	// Token: 0x06003373 RID: 13171 RVA: 0x00100844 File Offset: 0x000FEA44
	public static GameObject GetParentRootObject(Spell spell)
	{
		Actor parentActor = SpellUtils.GetParentActor(spell);
		if (parentActor == null)
		{
			return null;
		}
		return parentActor.GetRootObject();
	}

	// Token: 0x06003374 RID: 13172 RVA: 0x0010086C File Offset: 0x000FEA6C
	public static MeshRenderer GetParentRootObjectMesh(Spell spell)
	{
		Actor parentActor = SpellUtils.GetParentActor(spell);
		if (parentActor == null)
		{
			return null;
		}
		return parentActor.GetMeshRenderer();
	}

	// Token: 0x06003375 RID: 13173 RVA: 0x00100894 File Offset: 0x000FEA94
	public static bool IsNonMetaTaskListInMetaBlock(PowerTaskList taskList)
	{
		return taskList.DoesBlockHaveEffectTimingMetaData() && !taskList.HasEffectTimingMetaData();
	}

	// Token: 0x06003376 RID: 13174 RVA: 0x001008B1 File Offset: 0x000FEAB1
	public static bool CanAddPowerTargets(PowerTaskList taskList)
	{
		return !SpellUtils.IsNonMetaTaskListInMetaBlock(taskList) && (taskList.HasTasks() || taskList.IsEndOfBlock());
	}

	// Token: 0x06003377 RID: 13175 RVA: 0x001008DC File Offset: 0x000FEADC
	public static void SetCustomSpellParent(Spell spell, Component c)
	{
		if (spell == null)
		{
			return;
		}
		if (c == null)
		{
			return;
		}
		spell.transform.parent = c.transform;
		spell.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06003378 RID: 13176 RVA: 0x00100924 File Offset: 0x000FEB24
	public static void SetupSpell(Spell spell, Component c)
	{
		if (spell == null)
		{
			return;
		}
		if (c == null)
		{
			return;
		}
		spell.SetSource(c.gameObject);
	}

	// Token: 0x06003379 RID: 13177 RVA: 0x00100958 File Offset: 0x000FEB58
	public static void SetupSoundSpell(CardSoundSpell spell, Component c)
	{
		if (spell == null)
		{
			return;
		}
		if (c == null)
		{
			return;
		}
		spell.SetSource(c.gameObject);
		spell.transform.parent = c.transform;
		TransformUtil.Identity(spell.transform);
	}

	// Token: 0x0600337A RID: 13178 RVA: 0x001009A8 File Offset: 0x000FEBA8
	public static bool ActivateBirthIfNecessary(Spell spell)
	{
		if (spell == null)
		{
			return false;
		}
		SpellStateType activeState = spell.GetActiveState();
		if (activeState == SpellStateType.BIRTH)
		{
			return false;
		}
		if (activeState == SpellStateType.IDLE)
		{
			return false;
		}
		spell.ActivateState(SpellStateType.BIRTH);
		return true;
	}

	// Token: 0x0600337B RID: 13179 RVA: 0x001009E4 File Offset: 0x000FEBE4
	public static bool ActivateDeathIfNecessary(Spell spell)
	{
		if (spell == null)
		{
			return false;
		}
		SpellStateType activeState = spell.GetActiveState();
		if (activeState == SpellStateType.DEATH)
		{
			return false;
		}
		if (activeState == SpellStateType.NONE)
		{
			return false;
		}
		spell.ActivateState(SpellStateType.DEATH);
		return true;
	}

	// Token: 0x0600337C RID: 13180 RVA: 0x00100A20 File Offset: 0x000FEC20
	public static bool ActivateCancelIfNecessary(Spell spell)
	{
		if (spell == null)
		{
			return false;
		}
		SpellStateType activeState = spell.GetActiveState();
		if (activeState == SpellStateType.CANCEL)
		{
			return false;
		}
		if (activeState == SpellStateType.NONE)
		{
			return false;
		}
		spell.ActivateState(SpellStateType.CANCEL);
		return true;
	}

	// Token: 0x0600337D RID: 13181 RVA: 0x00100A5C File Offset: 0x000FEC5C
	public static void PurgeSpell(Spell spell)
	{
		if (spell == null)
		{
			return;
		}
		if (!spell.CanPurge())
		{
			return;
		}
		Object.Destroy(spell.gameObject);
	}

	// Token: 0x0600337E RID: 13182 RVA: 0x00100A90 File Offset: 0x000FEC90
	public static void PurgeSpells<T>(List<T> spells) where T : Spell
	{
		if (spells == null)
		{
			return;
		}
		if (spells.Count == 0)
		{
			return;
		}
		for (int i = 0; i < spells.Count; i++)
		{
			SpellUtils.PurgeSpell(spells[i]);
		}
	}

	// Token: 0x0600337F RID: 13183 RVA: 0x00100AD8 File Offset: 0x000FECD8
	private static GameObject FindSourceAutoObjectForSpell(Spell spell)
	{
		GameObject source = spell.GetSource();
		Card sourceCard = spell.GetSourceCard();
		if (sourceCard == null)
		{
			return source;
		}
		Entity entity = sourceCard.GetEntity();
		TAG_CARDTYPE cardType = entity.GetCardType();
		PowerTaskList powerTaskList = spell.GetPowerTaskList();
		if (powerTaskList != null)
		{
			EntityDef effectEntityDef = powerTaskList.GetEffectEntityDef();
			if (effectEntityDef != null)
			{
				cardType = effectEntityDef.GetCardType();
			}
		}
		return SpellUtils.FindAutoObjectForSpell(entity, sourceCard, cardType);
	}

	// Token: 0x06003380 RID: 13184 RVA: 0x00100B40 File Offset: 0x000FED40
	private static GameObject FindTargetAutoObjectForSpell(Spell spell)
	{
		GameObject visualTarget = spell.GetVisualTarget();
		if (visualTarget == null)
		{
			return null;
		}
		Card component = visualTarget.GetComponent<Card>();
		if (component == null)
		{
			return visualTarget;
		}
		Entity entity = component.GetEntity();
		TAG_CARDTYPE cardType = entity.GetCardType();
		return SpellUtils.FindAutoObjectForSpell(entity, component, cardType);
	}

	// Token: 0x06003381 RID: 13185 RVA: 0x00100B90 File Offset: 0x000FED90
	private static GameObject FindAutoObjectForSpell(Entity entity, Card card, TAG_CARDTYPE cardType)
	{
		if (cardType == TAG_CARDTYPE.SPELL)
		{
			Player controller = entity.GetController();
			Card heroCard = controller.GetHeroCard();
			if (heroCard == null)
			{
				return card.gameObject;
			}
			return heroCard.gameObject;
		}
		else
		{
			if (cardType != TAG_CARDTYPE.HERO_POWER)
			{
				return card.gameObject;
			}
			Player controller2 = entity.GetController();
			Card heroPowerCard = controller2.GetHeroPowerCard();
			if (heroPowerCard == null)
			{
				return card.gameObject;
			}
			return heroPowerCard.gameObject;
		}
	}

	// Token: 0x06003382 RID: 13186 RVA: 0x00100C04 File Offset: 0x000FEE04
	private static Card FindBestTargetCard(Spell spell)
	{
		Card sourceCard = spell.GetSourceCard();
		if (sourceCard == null)
		{
			return spell.GetVisualTargetCard();
		}
		Player controller = sourceCard.GetEntity().GetController();
		if (controller == null)
		{
			return spell.GetVisualTargetCard();
		}
		Player.Side side = controller.GetSide();
		List<GameObject> visualTargets = spell.GetVisualTargets();
		for (int i = 0; i < visualTargets.Count; i++)
		{
			Card component = visualTargets[i].GetComponent<Card>();
			if (!(component == null))
			{
				Player controller2 = component.GetEntity().GetController();
				if (controller2.GetSide() != side)
				{
					return component;
				}
			}
		}
		return spell.GetVisualTargetCard();
	}

	// Token: 0x06003383 RID: 13187 RVA: 0x00100CB4 File Offset: 0x000FEEB4
	private static Card FindHeroCard(Card card)
	{
		if (card == null)
		{
			return null;
		}
		Entity entity = card.GetEntity();
		Player controller = entity.GetController();
		if (controller == null)
		{
			return null;
		}
		return controller.GetHeroCard();
	}

	// Token: 0x06003384 RID: 13188 RVA: 0x00100CEC File Offset: 0x000FEEEC
	private static Card FindHeroPowerCard(Card card)
	{
		if (card == null)
		{
			return null;
		}
		Entity entity = card.GetEntity();
		Player controller = entity.GetController();
		if (controller == null)
		{
			return null;
		}
		return controller.GetHeroPowerCard();
	}

	// Token: 0x06003385 RID: 13189 RVA: 0x00100D23 File Offset: 0x000FEF23
	private static void FaceSameAs(GameObject source, GameObject target, SpellFacingOptions options)
	{
		SpellUtils.FaceSameAs(source.transform, target.transform, options);
	}

	// Token: 0x06003386 RID: 13190 RVA: 0x00100D37 File Offset: 0x000FEF37
	private static void FaceSameAs(GameObject source, Component target, SpellFacingOptions options)
	{
		SpellUtils.FaceSameAs(source.transform, target.transform, options);
	}

	// Token: 0x06003387 RID: 13191 RVA: 0x00100D4B File Offset: 0x000FEF4B
	private static void FaceSameAs(Component source, GameObject target, SpellFacingOptions options)
	{
		SpellUtils.FaceSameAs(source.transform, target.transform, options);
	}

	// Token: 0x06003388 RID: 13192 RVA: 0x00100D5F File Offset: 0x000FEF5F
	private static void FaceSameAs(Component source, Component target, SpellFacingOptions options)
	{
		SpellUtils.FaceSameAs(source.transform, target.transform, options);
	}

	// Token: 0x06003389 RID: 13193 RVA: 0x00100D74 File Offset: 0x000FEF74
	private static void FaceSameAs(Transform source, Transform target, SpellFacingOptions options)
	{
		SpellUtils.SetOrientation(source, target.position, target.position + target.forward, options);
	}

	// Token: 0x0600338A RID: 13194 RVA: 0x00100D9F File Offset: 0x000FEF9F
	private static void FaceOppositeOf(GameObject source, GameObject target, SpellFacingOptions options)
	{
		SpellUtils.FaceOppositeOf(source.transform, target.transform, options);
	}

	// Token: 0x0600338B RID: 13195 RVA: 0x00100DB3 File Offset: 0x000FEFB3
	private static void FaceOppositeOf(GameObject source, Component target, SpellFacingOptions options)
	{
		SpellUtils.FaceOppositeOf(source.transform, target.transform, options);
	}

	// Token: 0x0600338C RID: 13196 RVA: 0x00100DC7 File Offset: 0x000FEFC7
	private static void FaceOppositeOf(Component source, GameObject target, SpellFacingOptions options)
	{
		SpellUtils.FaceOppositeOf(source.transform, target.transform, options);
	}

	// Token: 0x0600338D RID: 13197 RVA: 0x00100DDB File Offset: 0x000FEFDB
	private static void FaceOppositeOf(Component source, Component target, SpellFacingOptions options)
	{
		SpellUtils.FaceOppositeOf(source.transform, target.transform, options);
	}

	// Token: 0x0600338E RID: 13198 RVA: 0x00100DF0 File Offset: 0x000FEFF0
	private static void FaceOppositeOf(Transform source, Transform target, SpellFacingOptions options)
	{
		SpellUtils.SetOrientation(source, target.position, target.position - target.forward, options);
	}

	// Token: 0x0600338F RID: 13199 RVA: 0x00100E1B File Offset: 0x000FF01B
	private static void FaceTowards(GameObject source, GameObject target, SpellFacingOptions options)
	{
		SpellUtils.FaceTowards(source.transform, target.transform, options);
	}

	// Token: 0x06003390 RID: 13200 RVA: 0x00100E2F File Offset: 0x000FF02F
	private static void FaceTowards(GameObject source, Component target, SpellFacingOptions options)
	{
		SpellUtils.FaceTowards(source.transform, target.transform, options);
	}

	// Token: 0x06003391 RID: 13201 RVA: 0x00100E43 File Offset: 0x000FF043
	private static void FaceTowards(Component source, GameObject target, SpellFacingOptions options)
	{
		SpellUtils.FaceTowards(source.transform, target.transform, options);
	}

	// Token: 0x06003392 RID: 13202 RVA: 0x00100E57 File Offset: 0x000FF057
	private static void FaceTowards(Component source, Component target, SpellFacingOptions options)
	{
		SpellUtils.FaceTowards(source.transform, target.transform, options);
	}

	// Token: 0x06003393 RID: 13203 RVA: 0x00100E6C File Offset: 0x000FF06C
	private static void FaceTowards(Transform source, Transform target, SpellFacingOptions options)
	{
		SpellUtils.SetOrientation(source, source.position, target.position, options);
	}

	// Token: 0x06003394 RID: 13204 RVA: 0x00100E8C File Offset: 0x000FF08C
	private static void SetOrientation(Transform source, Vector3 sourcePosition, Vector3 targetPosition, SpellFacingOptions options)
	{
		if (!options.m_RotateX || !options.m_RotateY)
		{
			if (options.m_RotateX)
			{
				targetPosition.x = sourcePosition.x;
			}
			else
			{
				if (!options.m_RotateY)
				{
					return;
				}
				targetPosition.y = sourcePosition.y;
			}
		}
		Vector3 vector = targetPosition - sourcePosition;
		if (vector.sqrMagnitude > Mathf.Epsilon)
		{
			source.rotation = Quaternion.LookRotation(vector);
		}
	}
}
