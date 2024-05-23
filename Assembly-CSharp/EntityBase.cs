using System;
using System.Collections.Generic;

// Token: 0x0200018F RID: 399
public abstract class EntityBase
{
	// Token: 0x060016FD RID: 5885 RVA: 0x0006C34F File Offset: 0x0006A54F
	public bool HasTag(GAME_TAG tag)
	{
		return this.GetTag(tag) > 0;
	}

	// Token: 0x060016FE RID: 5886 RVA: 0x0006C35B File Offset: 0x0006A55B
	public TagSet GetTags()
	{
		return this.m_tags;
	}

	// Token: 0x060016FF RID: 5887 RVA: 0x0006C363 File Offset: 0x0006A563
	public int GetTag(int tag)
	{
		return this.m_tags.GetTag(tag);
	}

	// Token: 0x06001700 RID: 5888 RVA: 0x0006C374 File Offset: 0x0006A574
	public int GetTag(GAME_TAG enumTag)
	{
		return this.m_tags.GetTag((int)enumTag);
	}

	// Token: 0x06001701 RID: 5889 RVA: 0x0006C390 File Offset: 0x0006A590
	public TagEnum GetTag<TagEnum>(GAME_TAG enumTag)
	{
		int tag = this.GetTag(enumTag);
		return (TagEnum)((object)Enum.ToObject(typeof(TagEnum), tag));
	}

	// Token: 0x06001702 RID: 5890 RVA: 0x0006C3BA File Offset: 0x0006A5BA
	public void SetTag(int tag, int tagValue)
	{
		this.m_tags.SetTag(tag, tagValue);
	}

	// Token: 0x06001703 RID: 5891 RVA: 0x0006C3C9 File Offset: 0x0006A5C9
	public void SetTag(GAME_TAG tag, int tagValue)
	{
		this.SetTag((int)tag, tagValue);
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x0006C3D3 File Offset: 0x0006A5D3
	public void SetTag<TagEnum>(GAME_TAG tag, TagEnum tagValue)
	{
		this.SetTag((int)tag, Convert.ToInt32(tagValue));
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x0006C3E7 File Offset: 0x0006A5E7
	public void SetTags(Map<GAME_TAG, int> tagMap)
	{
		this.m_tags.SetTags(tagMap);
	}

	// Token: 0x06001706 RID: 5894 RVA: 0x0006C3F5 File Offset: 0x0006A5F5
	public void SetTags(List<Network.Entity.Tag> tags)
	{
		this.m_tags.SetTags(tags);
	}

	// Token: 0x06001707 RID: 5895 RVA: 0x0006C403 File Offset: 0x0006A603
	public void ReplaceTags(TagSet tags)
	{
		this.m_tags.Replace(tags);
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x0006C411 File Offset: 0x0006A611
	public void ReplaceTags(List<Network.Entity.Tag> tags)
	{
		this.m_tags.Replace(tags);
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x0006C41F File Offset: 0x0006A61F
	public bool HasReferencedTag(GAME_TAG enumTag)
	{
		return this.GetReferencedTag(enumTag) > 0;
	}

	// Token: 0x0600170A RID: 5898 RVA: 0x0006C42B File Offset: 0x0006A62B
	public bool HasReferencedTag(int tag)
	{
		return this.GetReferencedTag(tag) > 0;
	}

	// Token: 0x0600170B RID: 5899 RVA: 0x0006C437 File Offset: 0x0006A637
	public int GetReferencedTag(GAME_TAG enumTag)
	{
		return this.GetReferencedTag((int)enumTag);
	}

	// Token: 0x0600170C RID: 5900
	public abstract int GetReferencedTag(int tag);

	// Token: 0x0600170D RID: 5901 RVA: 0x0006C440 File Offset: 0x0006A640
	public bool HasStringTag(GAME_TAG enumTag)
	{
		return this.GetStringTag(enumTag) != null;
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x0006C44F File Offset: 0x0006A64F
	public bool HasStringTag(int tag)
	{
		return this.GetStringTag(tag) != null;
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x0006C45E File Offset: 0x0006A65E
	public string GetStringTag(GAME_TAG enumTag)
	{
		return this.GetStringTag((int)enumTag);
	}

	// Token: 0x06001710 RID: 5904
	public abstract string GetStringTag(int tag);

	// Token: 0x06001711 RID: 5905 RVA: 0x0006C467 File Offset: 0x0006A667
	public bool HasCharge()
	{
		return this.HasTag(GAME_TAG.CHARGE);
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x0006C474 File Offset: 0x0006A674
	public bool ReferencesCharge()
	{
		return this.HasReferencedTag(GAME_TAG.CHARGE);
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x0006C481 File Offset: 0x0006A681
	public bool HasBattlecry()
	{
		return this.HasTag(GAME_TAG.BATTLECRY);
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x0006C48E File Offset: 0x0006A68E
	public bool ReferencesBattlecry()
	{
		return this.HasReferencedTag(GAME_TAG.BATTLECRY);
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x0006C49B File Offset: 0x0006A69B
	public bool CanBeTargetedByAbilities()
	{
		return !this.HasTag(GAME_TAG.CANT_BE_TARGETED_BY_ABILITIES);
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x0006C4AB File Offset: 0x0006A6AB
	public bool CanBeTargetedByHeroPowers()
	{
		return !this.HasTag(GAME_TAG.CANT_BE_TARGETED_BY_HERO_POWERS);
	}

	// Token: 0x06001717 RID: 5911 RVA: 0x0006C4BB File Offset: 0x0006A6BB
	public bool CanBeTargetedByBattlecries()
	{
		return !this.HasTag(GAME_TAG.CANT_BE_TARGETED_BY_BATTLECRIES);
	}

	// Token: 0x06001718 RID: 5912 RVA: 0x0006C4CB File Offset: 0x0006A6CB
	public bool HasTriggerVisual()
	{
		return this.HasTag(GAME_TAG.TRIGGER_VISUAL);
	}

	// Token: 0x06001719 RID: 5913 RVA: 0x0006C4D5 File Offset: 0x0006A6D5
	public bool HasInspire()
	{
		return this.HasTag(GAME_TAG.INSPIRE);
	}

	// Token: 0x0600171A RID: 5914 RVA: 0x0006C4E2 File Offset: 0x0006A6E2
	public bool IsImmune()
	{
		return this.HasTag(GAME_TAG.CANT_BE_DAMAGED);
	}

	// Token: 0x0600171B RID: 5915 RVA: 0x0006C4EF File Offset: 0x0006A6EF
	public bool DontShowImmune()
	{
		return this.HasTag(GAME_TAG.DONT_SHOW_IMMUNE);
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x0006C4FC File Offset: 0x0006A6FC
	public bool IsPoisonous()
	{
		return this.HasTag(GAME_TAG.POISONOUS);
	}

	// Token: 0x0600171D RID: 5917 RVA: 0x0006C509 File Offset: 0x0006A709
	public bool HasAura()
	{
		return this.HasTag(GAME_TAG.AURA);
	}

	// Token: 0x0600171E RID: 5918 RVA: 0x0006C516 File Offset: 0x0006A716
	public bool HasHealthMin()
	{
		return this.GetTag(GAME_TAG.HEALTH_MINIMUM) > 0;
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x0006C526 File Offset: 0x0006A726
	public bool ReferencesImmune()
	{
		return this.HasReferencedTag(GAME_TAG.CANT_BE_DAMAGED);
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x0006C533 File Offset: 0x0006A733
	public bool IsEnraged()
	{
		return this.HasTag(GAME_TAG.ENRAGED) && this.GetDamage() > 0;
	}

	// Token: 0x06001721 RID: 5921 RVA: 0x0006C551 File Offset: 0x0006A751
	public bool IsFreeze()
	{
		return this.HasTag(GAME_TAG.FREEZE);
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x0006C55E File Offset: 0x0006A75E
	public int GetDamage()
	{
		return this.GetTag(GAME_TAG.DAMAGE);
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x0006C568 File Offset: 0x0006A768
	public bool IsFrozen()
	{
		return this.HasTag(GAME_TAG.FROZEN);
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x0006C575 File Offset: 0x0006A775
	public bool IsAsleep()
	{
		return this.GetNumTurnsInPlay() == 0 && !this.HasCharge();
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x0006C58E File Offset: 0x0006A78E
	public bool IsStealthed()
	{
		return this.HasTag(GAME_TAG.STEALTH);
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x0006C59B File Offset: 0x0006A79B
	public bool ReferencesStealth()
	{
		return this.HasReferencedTag(GAME_TAG.STEALTH);
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x0006C5A8 File Offset: 0x0006A7A8
	public bool HasTaunt()
	{
		return this.HasTag(GAME_TAG.TAUNT);
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x0006C5B5 File Offset: 0x0006A7B5
	public bool ReferencesTaunt()
	{
		return this.HasReferencedTag(GAME_TAG.TAUNT);
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x0006C5C2 File Offset: 0x0006A7C2
	public bool HasDivineShield()
	{
		return this.HasTag(GAME_TAG.DIVINE_SHIELD);
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x0006C5CF File Offset: 0x0006A7CF
	public bool ReferencesDivineShield()
	{
		return this.HasReferencedTag(GAME_TAG.DIVINE_SHIELD);
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x0006C5DC File Offset: 0x0006A7DC
	public bool IsHero()
	{
		return this.GetTag(GAME_TAG.CARDTYPE) == 3;
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x0006C5EC File Offset: 0x0006A7EC
	public bool IsHeroPower()
	{
		return this.GetTag(GAME_TAG.CARDTYPE) == 10;
	}

	// Token: 0x0600172D RID: 5933 RVA: 0x0006C5FD File Offset: 0x0006A7FD
	public bool IsMinion()
	{
		return this.GetTag(GAME_TAG.CARDTYPE) == 4;
	}

	// Token: 0x0600172E RID: 5934 RVA: 0x0006C60D File Offset: 0x0006A80D
	public bool IsSpell()
	{
		return this.GetTag(GAME_TAG.CARDTYPE) == 5;
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x0006C61D File Offset: 0x0006A81D
	public bool IsWeapon()
	{
		return this.GetTag(GAME_TAG.CARDTYPE) == 7;
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x0006C62D File Offset: 0x0006A82D
	public bool IsElite()
	{
		return this.GetTag(GAME_TAG.ELITE) > 0;
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x0006C63A File Offset: 0x0006A83A
	public TAG_CARD_SET GetCardSet()
	{
		return (TAG_CARD_SET)this.GetTag(GAME_TAG.CARD_SET);
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x0006C647 File Offset: 0x0006A847
	public TAG_CARDTYPE GetCardType()
	{
		return (TAG_CARDTYPE)this.GetTag(GAME_TAG.CARDTYPE);
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x0006C654 File Offset: 0x0006A854
	public bool IsGame()
	{
		return this.GetTag(GAME_TAG.CARDTYPE) == 1;
	}

	// Token: 0x06001734 RID: 5940 RVA: 0x0006C664 File Offset: 0x0006A864
	public bool IsPlayer()
	{
		return this.GetTag(GAME_TAG.CARDTYPE) == 2;
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x0006C674 File Offset: 0x0006A874
	public bool IsEnchantment()
	{
		return this.GetTag(GAME_TAG.CARDTYPE) == 6;
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x0006C684 File Offset: 0x0006A884
	public bool IsExhausted()
	{
		return this.HasTag(GAME_TAG.EXHAUSTED);
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x0006C68E File Offset: 0x0006A88E
	public bool IsAttached()
	{
		return this.HasTag(GAME_TAG.ATTACHED);
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x0006C698 File Offset: 0x0006A898
	public bool IsRecentlyArrived()
	{
		return this.HasTag(GAME_TAG.RECENTLY_ARRIVED);
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x0006C6A2 File Offset: 0x0006A8A2
	public bool IsObfuscated()
	{
		return this.HasTag(GAME_TAG.OBFUSCATED);
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x0006C6AF File Offset: 0x0006A8AF
	public bool IsSecret()
	{
		return this.HasTag(GAME_TAG.SECRET);
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x0006C6BC File Offset: 0x0006A8BC
	public bool ReferencesSecret()
	{
		return this.HasReferencedTag(GAME_TAG.SECRET);
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x0006C6C9 File Offset: 0x0006A8C9
	public bool CanAttack()
	{
		return !this.HasTag(GAME_TAG.CANT_ATTACK);
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x0006C6D9 File Offset: 0x0006A8D9
	public bool CannotAttackHeroes()
	{
		return this.HasTag(GAME_TAG.CANNOT_ATTACK_HEROES);
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x0006C6E6 File Offset: 0x0006A8E6
	public bool CanBeAttacked()
	{
		return !this.HasTag(GAME_TAG.CANT_BE_ATTACKED);
	}

	// Token: 0x0600173F RID: 5951 RVA: 0x0006C6F6 File Offset: 0x0006A8F6
	public bool CanBeTargetedByOpponents()
	{
		return !this.HasTag(GAME_TAG.CANT_BE_TARGETED_BY_OPPONENTS);
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x0006C706 File Offset: 0x0006A906
	public bool IsMagnet()
	{
		return this.HasTag(GAME_TAG.MAGNET);
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x0006C713 File Offset: 0x0006A913
	public int GetNumTurnsInPlay()
	{
		return this.GetTag(GAME_TAG.NUM_TURNS_IN_PLAY);
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x0006C720 File Offset: 0x0006A920
	public int GetNumAttacksThisTurn()
	{
		return this.GetTag(GAME_TAG.NUM_ATTACKS_THIS_TURN);
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x0006C72D File Offset: 0x0006A92D
	public int GetSpellPower()
	{
		return this.GetTag(GAME_TAG.SPELLPOWER);
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x0006C73A File Offset: 0x0006A93A
	public bool HasSpellPower()
	{
		return this.HasTag(GAME_TAG.SPELLPOWER);
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x0006C747 File Offset: 0x0006A947
	public bool HasHeroPowerDamage()
	{
		return this.HasTag(GAME_TAG.HEROPOWER_DAMAGE);
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x0006C754 File Offset: 0x0006A954
	public bool IsAffectedBySpellPower()
	{
		return this.HasTag(GAME_TAG.AFFECTED_BY_SPELL_POWER);
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x0006C761 File Offset: 0x0006A961
	public bool HasSpellPowerDouble()
	{
		return this.HasTag(GAME_TAG.SPELLPOWER_DOUBLE);
	}

	// Token: 0x06001748 RID: 5960 RVA: 0x0006C76E File Offset: 0x0006A96E
	public bool ReferencesSpellPower()
	{
		return this.HasReferencedTag(GAME_TAG.SPELLPOWER);
	}

	// Token: 0x06001749 RID: 5961 RVA: 0x0006C77B File Offset: 0x0006A97B
	public int GetCost()
	{
		return this.GetTag(GAME_TAG.COST);
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x0006C785 File Offset: 0x0006A985
	public int GetATK()
	{
		return this.GetTag(GAME_TAG.ATK);
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x0006C78F File Offset: 0x0006A98F
	public bool IsDamaged()
	{
		return this.GetDamage() > 0;
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x0006C79A File Offset: 0x0006A99A
	public int GetAttached()
	{
		return this.GetTag(GAME_TAG.ATTACHED);
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x0006C7A4 File Offset: 0x0006A9A4
	public int GetDurability()
	{
		return this.GetTag(GAME_TAG.DURABILITY);
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x0006C7B1 File Offset: 0x0006A9B1
	public TAG_ZONE GetZone()
	{
		return (TAG_ZONE)this.GetTag(GAME_TAG.ZONE);
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x0006C7BB File Offset: 0x0006A9BB
	public int GetZonePosition()
	{
		return this.GetTag(GAME_TAG.ZONE_POSITION);
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x0006C7C8 File Offset: 0x0006A9C8
	public int GetArmor()
	{
		return this.GetTag(GAME_TAG.ARMOR);
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x0006C7D5 File Offset: 0x0006A9D5
	public int GetCreatorId()
	{
		return this.GetTag(GAME_TAG.CREATOR);
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x0006C7E2 File Offset: 0x0006A9E2
	public int GetControllerId()
	{
		return this.GetTag(GAME_TAG.CONTROLLER);
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x0006C7EC File Offset: 0x0006A9EC
	public int GetFatigue()
	{
		return this.GetTag(GAME_TAG.FATIGUE);
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x0006C7F6 File Offset: 0x0006A9F6
	public int GetWindfury()
	{
		return this.GetTag(GAME_TAG.WINDFURY);
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x0006C803 File Offset: 0x0006AA03
	public bool HasWindfury()
	{
		return this.GetTag(GAME_TAG.WINDFURY) > 0;
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x0006C813 File Offset: 0x0006AA13
	public bool ReferencesWindfury()
	{
		return this.HasReferencedTag(GAME_TAG.WINDFURY);
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x0006C820 File Offset: 0x0006AA20
	public int GetExtraAttacksThisTurn()
	{
		return this.GetTag(GAME_TAG.EXTRA_ATTACKS_THIS_TURN);
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x0006C82D File Offset: 0x0006AA2D
	public bool HasCombo()
	{
		return this.HasTag(GAME_TAG.COMBO);
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x0006C83A File Offset: 0x0006AA3A
	public bool HasOverload()
	{
		return this.HasTag(GAME_TAG.OVERLOAD);
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x0006C847 File Offset: 0x0006AA47
	public bool HasDeathrattle()
	{
		return this.HasTag(GAME_TAG.DEATHRATTLE);
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x0006C854 File Offset: 0x0006AA54
	public bool ReferencesDeathrattle()
	{
		return this.HasReferencedTag(GAME_TAG.DEATHRATTLE);
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x0006C861 File Offset: 0x0006AA61
	public bool IsSilenced()
	{
		return this.HasTag(GAME_TAG.SILENCED);
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x0006C86E File Offset: 0x0006AA6E
	public int GetEntityId()
	{
		return this.GetTag(GAME_TAG.ENTITY_ID);
	}

	// Token: 0x0600175E RID: 5982 RVA: 0x0006C878 File Offset: 0x0006AA78
	public bool IsCharacter()
	{
		return this.IsHero() || this.IsMinion();
	}

	// Token: 0x0600175F RID: 5983 RVA: 0x0006C88E File Offset: 0x0006AA8E
	public bool IsItem()
	{
		return this.GetTag(GAME_TAG.CARDTYPE) == 8;
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x0006C89E File Offset: 0x0006AA9E
	public bool IsToken()
	{
		return this.GetTag(GAME_TAG.CARDTYPE) == 9;
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x0006C8AF File Offset: 0x0006AAAF
	public int GetHealth()
	{
		if (this.IsWeapon())
		{
			return this.GetTag(GAME_TAG.DURABILITY);
		}
		return this.GetTag(GAME_TAG.HEALTH);
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x0006C8D0 File Offset: 0x0006AAD0
	public bool HasCustomKeywordEffect()
	{
		return this.HasTag(GAME_TAG.CUSTOM_KEYWORD_EFFECT);
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x0006C8DD File Offset: 0x0006AADD
	public int GetDisplayedCreatorId()
	{
		return this.GetTag(GAME_TAG.DISPLAYED_CREATOR);
	}

	// Token: 0x06001764 RID: 5988 RVA: 0x0006C8EC File Offset: 0x0006AAEC
	public bool IsBasicCardUnlock()
	{
		if (this.IsHero())
		{
			return false;
		}
		TAG_CARD_SET tag = this.GetTag<TAG_CARD_SET>(GAME_TAG.CARD_SET);
		return tag == TAG_CARD_SET.CORE;
	}

	// Token: 0x04000B81 RID: 2945
	protected TagSet m_tags = new TagSet();
}
