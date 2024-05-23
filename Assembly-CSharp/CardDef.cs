using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A6 RID: 422
[CustomEditClass]
public class CardDef : MonoBehaviour
{
	// Token: 0x06001BDA RID: 7130 RVA: 0x00083284 File Offset: 0x00081484
	public void Awake()
	{
		if (string.IsNullOrEmpty(this.m_PortraitTexturePath))
		{
			this.m_portraitQuality.TextureQuality = 3;
			this.m_portraitQuality.LoadPremium = true;
		}
		else if (string.IsNullOrEmpty(this.m_PremiumPortraitMaterialPath))
		{
			this.m_portraitQuality.LoadPremium = true;
		}
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x000832DA File Offset: 0x000814DA
	public virtual string DetermineActorNameForZone(Entity entity, TAG_ZONE zoneTag)
	{
		return ActorNames.GetZoneActor(entity, zoneTag);
	}

	// Token: 0x06001BDC RID: 7132 RVA: 0x000832E4 File Offset: 0x000814E4
	public virtual SpellType DetermineSummonInSpell_HandToPlay(Card card)
	{
		Entity entity = card.GetEntity();
		EntityDef entityDef = entity.GetEntityDef();
		int cost = entityDef.GetCost();
		TAG_PREMIUM premiumType = entity.GetPremiumType();
		bool flag = entity.GetController().IsFriendlySide();
		if (cost >= 7)
		{
			TAG_PREMIUM tag_PREMIUM = premiumType;
			if (tag_PREMIUM != TAG_PREMIUM.NORMAL)
			{
				if (tag_PREMIUM != TAG_PREMIUM.GOLDEN)
				{
					Debug.LogWarning(string.Format("CardDef.DetermineSummonInSpell_HandToPlay() - unexpected premium type {0}", premiumType));
				}
				else
				{
					if (flag)
					{
						return SpellType.SUMMON_IN_LARGE_PREMIUM;
					}
					return SpellType.SUMMON_IN_OPPONENT_LARGE_PREMIUM;
				}
			}
			if (flag)
			{
				return SpellType.SUMMON_IN_LARGE;
			}
			return SpellType.SUMMON_IN_OPPONENT_LARGE;
		}
		else if (cost >= 4)
		{
			TAG_PREMIUM tag_PREMIUM = premiumType;
			if (tag_PREMIUM != TAG_PREMIUM.NORMAL)
			{
				if (tag_PREMIUM != TAG_PREMIUM.GOLDEN)
				{
					Debug.LogWarning(string.Format("CardDef.DetermineSummonInSpell_HandToPlay() - unexpected premium type {0}", premiumType));
				}
				else
				{
					if (flag)
					{
						return SpellType.SUMMON_IN_MEDIUM_PREMIUM;
					}
					return SpellType.SUMMON_IN_OPPONENT_MEDIUM_PREMIUM;
				}
			}
			if (flag)
			{
				return SpellType.SUMMON_IN_MEDIUM;
			}
			return SpellType.SUMMON_IN_OPPONENT_MEDIUM;
		}
		else
		{
			TAG_PREMIUM tag_PREMIUM = premiumType;
			if (tag_PREMIUM != TAG_PREMIUM.NORMAL)
			{
				if (tag_PREMIUM != TAG_PREMIUM.GOLDEN)
				{
					Debug.LogWarning(string.Format("CardDef.DetermineSummonInSpell_HandToPlay() - unexpected premium type {0}", premiumType));
				}
				else
				{
					if (flag)
					{
						return SpellType.SUMMON_IN_PREMIUM;
					}
					return SpellType.SUMMON_IN_OPPONENT_PREMIUM;
				}
			}
			if (flag)
			{
				return SpellType.SUMMON_IN;
			}
			return SpellType.SUMMON_IN_OPPONENT;
		}
	}

	// Token: 0x06001BDD RID: 7133 RVA: 0x00083408 File Offset: 0x00081608
	public virtual SpellType DetermineSummonOutSpell_HandToPlay(Card card)
	{
		Entity entity = card.GetEntity();
		if (!entity.GetController().IsFriendlySide())
		{
			return SpellType.SUMMON_OUT;
		}
		EntityDef entityDef = entity.GetEntityDef();
		int cost = entityDef.GetCost();
		TAG_PREMIUM premiumType = entity.GetPremiumType();
		TAG_PREMIUM tag_PREMIUM;
		if (cost >= 7)
		{
			tag_PREMIUM = premiumType;
			if (tag_PREMIUM != TAG_PREMIUM.NORMAL)
			{
				if (tag_PREMIUM == TAG_PREMIUM.GOLDEN)
				{
					return SpellType.SUMMON_OUT_PREMIUM;
				}
				Debug.LogWarning(string.Format("CardDef.DetermineSummonOutSpell_HandToPlay(): unexpected premium type {0}", premiumType));
			}
			return SpellType.SUMMON_OUT_LARGE;
		}
		if (cost >= 4)
		{
			tag_PREMIUM = premiumType;
			if (tag_PREMIUM != TAG_PREMIUM.NORMAL)
			{
				if (tag_PREMIUM == TAG_PREMIUM.GOLDEN)
				{
					return SpellType.SUMMON_OUT_PREMIUM;
				}
				Debug.LogWarning(string.Format("CardDef.DetermineSummonOutSpell_HandToPlay(): unexpected premium type {0}", premiumType));
			}
			return SpellType.SUMMON_OUT_MEDIUM;
		}
		tag_PREMIUM = premiumType;
		if (tag_PREMIUM != TAG_PREMIUM.NORMAL)
		{
			if (tag_PREMIUM == TAG_PREMIUM.GOLDEN)
			{
				return SpellType.SUMMON_OUT_PREMIUM;
			}
			Debug.LogWarning(string.Format("CardDef.DetermineSummonOutSpell_HandToPlay(): unexpected premium type {0}", premiumType));
		}
		return SpellType.SUMMON_OUT;
	}

	// Token: 0x06001BDE RID: 7134 RVA: 0x000834F4 File Offset: 0x000816F4
	private static void SetTextureIfNotNull(Material baseMat, ref Material targetMat, Texture tex)
	{
		if (baseMat == null)
		{
			return;
		}
		if (targetMat == null)
		{
			targetMat = Object.Instantiate<Material>(baseMat);
		}
		targetMat.mainTexture = tex;
	}

	// Token: 0x06001BDF RID: 7135 RVA: 0x0008352C File Offset: 0x0008172C
	public void OnPortraitLoaded(Texture portrait, int quality)
	{
		if (quality <= this.m_portraitQuality.TextureQuality)
		{
			Debug.LogWarning(string.Format("Loaded texture of quality lower or equal to what was was already available ({0} <= {1}), texture={2}", quality, this.m_portraitQuality, portrait));
			return;
		}
		this.m_portraitQuality.TextureQuality = quality;
		this.m_LoadedPortraitTexture = portrait;
		if (this.m_LoadedPremiumPortraitMaterial != null && string.IsNullOrEmpty(this.m_PremiumPortraitTexturePath))
		{
			this.m_LoadedPremiumPortraitMaterial.mainTexture = portrait;
			this.m_portraitQuality.LoadPremium = true;
		}
		CardDef.SetTextureIfNotNull(this.m_DeckCardBarPortrait, ref this.m_LoadedDeckCardBarPortrait, portrait);
		CardDef.SetTextureIfNotNull(this.m_EnchantmentPortrait, ref this.m_LoadedEnchantmentPortrait, portrait);
		CardDef.SetTextureIfNotNull(this.m_HistoryTileFullPortrait, ref this.m_LoadedHistoryTileFullPortrait, portrait);
		CardDef.SetTextureIfNotNull(this.m_HistoryTileHalfPortrait, ref this.m_LoadedHistoryTileHalfPortrait, portrait);
		CardDef.SetTextureIfNotNull(this.m_CustomDeckPortrait, ref this.m_LoadedCustomDeckPortrait, portrait);
		CardDef.SetTextureIfNotNull(this.m_DeckPickerPortrait, ref this.m_LoadedDeckPickerPortrait, portrait);
		CardDef.SetTextureIfNotNull(this.m_PracticeAIPortrait, ref this.m_LoadedPracticeAIPortrait, portrait);
		CardDef.SetTextureIfNotNull(this.m_DeckBoxPortrait, ref this.m_LoadedDeckBoxPortrait, portrait);
	}

	// Token: 0x06001BE0 RID: 7136 RVA: 0x00083650 File Offset: 0x00081850
	public void OnPremiumMaterialLoaded(Material material, Texture portrait)
	{
		if (this.m_LoadedPremiumPortraitMaterial != null)
		{
			Debug.LogWarning(string.Format("Loaded premium material twice: {0}", material));
			return;
		}
		if (material != null)
		{
			this.m_LoadedPremiumPortraitMaterial = Object.Instantiate<Material>(material);
		}
		this.m_LoadedPremiumPortraitTexture = portrait;
		if (string.IsNullOrEmpty(this.m_PremiumPortraitTexturePath))
		{
			if (this.m_LoadedPortraitTexture != null)
			{
				if (this.m_LoadedPremiumPortraitMaterial != null)
				{
					this.m_LoadedPremiumPortraitMaterial.mainTexture = this.m_LoadedPortraitTexture;
				}
				this.m_portraitQuality.LoadPremium = true;
			}
		}
		else if (this.m_LoadedPremiumPortraitTexture != null)
		{
			if (this.m_LoadedPremiumPortraitMaterial != null)
			{
				this.m_LoadedPremiumPortraitMaterial.mainTexture = this.m_LoadedPremiumPortraitTexture;
			}
			this.m_portraitQuality.LoadPremium = true;
		}
	}

	// Token: 0x06001BE1 RID: 7137 RVA: 0x00083731 File Offset: 0x00081931
	public CardPortraitQuality GetPortraitQuality()
	{
		return this.m_portraitQuality;
	}

	// Token: 0x06001BE2 RID: 7138 RVA: 0x00083739 File Offset: 0x00081939
	public Texture GetPortraitTexture()
	{
		return this.m_LoadedPortraitTexture;
	}

	// Token: 0x06001BE3 RID: 7139 RVA: 0x00083741 File Offset: 0x00081941
	public bool IsPremiumLoaded()
	{
		return this.m_portraitQuality.LoadPremium;
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x0008374E File Offset: 0x0008194E
	public Material GetPremiumPortraitMaterial()
	{
		return this.m_LoadedPremiumPortraitMaterial;
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x00083756 File Offset: 0x00081956
	public Material GetDeckCardBarPortrait()
	{
		return this.m_LoadedDeckCardBarPortrait;
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x0008375E File Offset: 0x0008195E
	public Material GetEnchantmentPortrait()
	{
		return this.m_LoadedEnchantmentPortrait;
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x00083766 File Offset: 0x00081966
	public Material GetHistoryTileFullPortrait()
	{
		return this.m_LoadedHistoryTileFullPortrait;
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x0008376E File Offset: 0x0008196E
	public Material GetHistoryTileHalfPortrait()
	{
		return this.m_LoadedHistoryTileHalfPortrait;
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x00083776 File Offset: 0x00081976
	public Material GetCustomDeckPortrait()
	{
		return this.m_LoadedCustomDeckPortrait;
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x0008377E File Offset: 0x0008197E
	public Material GetDeckPickerPortrait()
	{
		return this.m_LoadedDeckPickerPortrait;
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x00083786 File Offset: 0x00081986
	public Material GetPracticeAIPortrait()
	{
		return this.m_LoadedPracticeAIPortrait;
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x0008378E File Offset: 0x0008198E
	public Material GetDeckBoxPortrait()
	{
		return this.m_LoadedDeckBoxPortrait;
	}

	// Token: 0x04000E6B RID: 3691
	protected const int LARGE_MINION_COST = 7;

	// Token: 0x04000E6C RID: 3692
	protected const int MEDIUM_MINION_COST = 4;

	// Token: 0x04000E6D RID: 3693
	[CustomEditField(Sections = "Portrait", T = EditType.CARD_TEXTURE)]
	public string m_PortraitTexturePath;

	// Token: 0x04000E6E RID: 3694
	[CustomEditField(Sections = "Portrait", T = EditType.MATERIAL)]
	public string m_PremiumPortraitMaterialPath;

	// Token: 0x04000E6F RID: 3695
	[CustomEditField(Sections = "Portrait", T = EditType.CARD_TEXTURE)]
	public string m_PremiumPortraitTexturePath;

	// Token: 0x04000E70 RID: 3696
	[CustomEditField(Sections = "Portrait")]
	public Material m_DeckCardBarPortrait;

	// Token: 0x04000E71 RID: 3697
	[CustomEditField(Sections = "Portrait")]
	public Material m_EnchantmentPortrait;

	// Token: 0x04000E72 RID: 3698
	[CustomEditField(Sections = "Portrait")]
	public Material m_HistoryTileHalfPortrait;

	// Token: 0x04000E73 RID: 3699
	[CustomEditField(Sections = "Portrait")]
	public Material m_HistoryTileFullPortrait;

	// Token: 0x04000E74 RID: 3700
	[CustomEditField(Sections = "Portrait")]
	public Material_MobileOverride m_CustomDeckPortrait;

	// Token: 0x04000E75 RID: 3701
	[CustomEditField(Sections = "Portrait")]
	public Material_MobileOverride m_DeckPickerPortrait;

	// Token: 0x04000E76 RID: 3702
	[CustomEditField(Sections = "Portrait")]
	public Material m_PracticeAIPortrait;

	// Token: 0x04000E77 RID: 3703
	[CustomEditField(Sections = "Portrait")]
	public Material m_DeckBoxPortrait;

	// Token: 0x04000E78 RID: 3704
	[CustomEditField(Sections = "Portrait")]
	public bool m_AlwaysRenderPremiumPortrait;

	// Token: 0x04000E79 RID: 3705
	[CustomEditField(Sections = "Play")]
	public CardEffectDef m_PlayEffectDef;

	// Token: 0x04000E7A RID: 3706
	[CustomEditField(Sections = "Attack")]
	public CardEffectDef m_AttackEffectDef;

	// Token: 0x04000E7B RID: 3707
	[CustomEditField(Sections = "Death")]
	public CardEffectDef m_DeathEffectDef;

	// Token: 0x04000E7C RID: 3708
	[CustomEditField(Sections = "Lifetime")]
	public CardEffectDef m_LifetimeEffectDef;

	// Token: 0x04000E7D RID: 3709
	[CustomEditField(Sections = "Trigger")]
	public List<CardEffectDef> m_TriggerEffectDefs;

	// Token: 0x04000E7E RID: 3710
	[CustomEditField(Sections = "SubOption")]
	public List<CardEffectDef> m_SubOptionEffectDefs;

	// Token: 0x04000E7F RID: 3711
	[CustomEditField(Sections = "Custom", T = EditType.SPELL)]
	public string m_CustomSummonSpellPath;

	// Token: 0x04000E80 RID: 3712
	[CustomEditField(Sections = "Custom", T = EditType.SPELL)]
	public string m_GoldenCustomSummonSpellPath;

	// Token: 0x04000E81 RID: 3713
	[CustomEditField(Sections = "Custom", T = EditType.SPELL)]
	public string m_CustomSpawnSpellPath;

	// Token: 0x04000E82 RID: 3714
	[CustomEditField(Sections = "Custom", T = EditType.SPELL)]
	public string m_GoldenCustomSpawnSpellPath;

	// Token: 0x04000E83 RID: 3715
	[CustomEditField(Sections = "Custom", T = EditType.SPELL)]
	public string m_CustomDeathSpellPath;

	// Token: 0x04000E84 RID: 3716
	[CustomEditField(Sections = "Custom", T = EditType.SPELL)]
	public string m_GoldenCustomDeathSpellPath;

	// Token: 0x04000E85 RID: 3717
	[CustomEditField(Sections = "Custom", T = EditType.SPELL)]
	public string m_CustomKeywordSpellPath;

	// Token: 0x04000E86 RID: 3718
	[CustomEditField(Sections = "Hero", T = EditType.SPELL)]
	public string m_CustomHeroArmorSpell;

	// Token: 0x04000E87 RID: 3719
	[CustomEditField(Sections = "Hero", T = EditType.SPELL)]
	public string m_SocketInEffectFriendly;

	// Token: 0x04000E88 RID: 3720
	[CustomEditField(Sections = "Hero", T = EditType.SPELL)]
	public string m_SocketInEffectOpponent;

	// Token: 0x04000E89 RID: 3721
	[CustomEditField(Sections = "Hero", T = EditType.SPELL)]
	public string m_SocketInEffectFriendlyPhone;

	// Token: 0x04000E8A RID: 3722
	[CustomEditField(Sections = "Hero", T = EditType.SPELL)]
	public string m_SocketInEffectOpponentPhone;

	// Token: 0x04000E8B RID: 3723
	[CustomEditField(Sections = "Hero")]
	public bool m_SocketInOverrideHeroAnimation;

	// Token: 0x04000E8C RID: 3724
	[CustomEditField(Sections = "Hero")]
	public bool m_SocketInParentEffectToHero = true;

	// Token: 0x04000E8D RID: 3725
	[CustomEditField(Sections = "Hero", T = EditType.TEXTURE)]
	public string m_CustomHeroTray;

	// Token: 0x04000E8E RID: 3726
	[CustomEditField(Sections = "Hero")]
	public List<Board.CustomTraySettings> m_CustomHeroTraySettings;

	// Token: 0x04000E8F RID: 3727
	[CustomEditField(Sections = "Hero", T = EditType.TEXTURE)]
	public string m_CustomHeroPhoneTray;

	// Token: 0x04000E90 RID: 3728
	[CustomEditField(Sections = "Hero", T = EditType.TEXTURE)]
	public string m_CustomHeroPhoneManaGem;

	// Token: 0x04000E91 RID: 3729
	[CustomEditField(Sections = "Hero", T = EditType.SOUND_PREFAB)]
	public string m_AnnouncerLinePath;

	// Token: 0x04000E92 RID: 3730
	[CustomEditField(Sections = "Hero")]
	public List<EmoteEntryDef> m_EmoteDefs;

	// Token: 0x04000E93 RID: 3731
	[CustomEditField(Sections = "Misc")]
	public bool m_SuppressDeathrattleDeath;

	// Token: 0x04000E94 RID: 3732
	private Material m_LoadedPremiumPortraitMaterial;

	// Token: 0x04000E95 RID: 3733
	private Material m_LoadedDeckCardBarPortrait;

	// Token: 0x04000E96 RID: 3734
	private Material m_LoadedEnchantmentPortrait;

	// Token: 0x04000E97 RID: 3735
	private Material m_LoadedHistoryTileFullPortrait;

	// Token: 0x04000E98 RID: 3736
	private Material m_LoadedHistoryTileHalfPortrait;

	// Token: 0x04000E99 RID: 3737
	private Material m_LoadedCustomDeckPortrait;

	// Token: 0x04000E9A RID: 3738
	private Material m_LoadedDeckPickerPortrait;

	// Token: 0x04000E9B RID: 3739
	private Material m_LoadedPracticeAIPortrait;

	// Token: 0x04000E9C RID: 3740
	private Material m_LoadedDeckBoxPortrait;

	// Token: 0x04000E9D RID: 3741
	private CardPortraitQuality m_portraitQuality = CardPortraitQuality.GetUnloaded();

	// Token: 0x04000E9E RID: 3742
	private Texture m_LoadedPortraitTexture;

	// Token: 0x04000E9F RID: 3743
	private Texture m_LoadedPremiumPortraitTexture;
}
