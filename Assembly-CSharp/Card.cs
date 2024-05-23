using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class Card : MonoBehaviour
{
	// Token: 0x06001767 RID: 5991 RVA: 0x0006C9F0 File Offset: 0x0006ABF0
	public override string ToString()
	{
		return (this.m_entity != null) ? this.m_entity.ToString() : "UNKNOWN CARD";
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x0006CA1D File Offset: 0x0006AC1D
	public Entity GetEntity()
	{
		return this.m_entity;
	}

	// Token: 0x06001769 RID: 5993 RVA: 0x0006CA25 File Offset: 0x0006AC25
	public void SetEntity(Entity entity)
	{
		this.m_entity = entity;
	}

	// Token: 0x0600176A RID: 5994 RVA: 0x0006CA30 File Offset: 0x0006AC30
	public void Destroy()
	{
		if (this.m_actor != null)
		{
			this.m_actor.Destroy();
		}
		this.DestroyCardDefAssets();
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x0006CA6A File Offset: 0x0006AC6A
	public Player GetController()
	{
		if (this.m_entity == null)
		{
			return null;
		}
		return this.m_entity.GetController();
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x0006CA84 File Offset: 0x0006AC84
	public Player.Side GetControllerSide()
	{
		if (this.m_entity == null)
		{
			return Player.Side.NEUTRAL;
		}
		return this.m_entity.GetControllerSide();
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x0006CAA0 File Offset: 0x0006ACA0
	public Entity GetHero()
	{
		Player controller = this.GetController();
		if (controller == null)
		{
			return null;
		}
		return controller.GetHero();
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x0006CAC4 File Offset: 0x0006ACC4
	public Card GetHeroCard()
	{
		Entity hero = this.GetHero();
		if (hero == null)
		{
			return null;
		}
		return hero.GetCard();
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x0006CAE8 File Offset: 0x0006ACE8
	public Entity GetHeroPower()
	{
		Player controller = this.GetController();
		if (controller == null)
		{
			return null;
		}
		return controller.GetHeroPower();
	}

	// Token: 0x06001770 RID: 6000 RVA: 0x0006CB0C File Offset: 0x0006AD0C
	public Card GetHeroPowerCard()
	{
		Entity heroPower = this.GetHeroPower();
		if (heroPower == null)
		{
			return null;
		}
		return heroPower.GetCard();
	}

	// Token: 0x06001771 RID: 6001 RVA: 0x0006CB2E File Offset: 0x0006AD2E
	public TAG_PREMIUM GetPremium()
	{
		if (this.m_entity == null)
		{
			return TAG_PREMIUM.NORMAL;
		}
		return this.m_entity.GetPremiumType();
	}

	// Token: 0x06001772 RID: 6002 RVA: 0x0006CB48 File Offset: 0x0006AD48
	public bool IsOverPlayfield()
	{
		return this.m_overPlayfield;
	}

	// Token: 0x06001773 RID: 6003 RVA: 0x0006CB50 File Offset: 0x0006AD50
	public void NotifyOverPlayfield()
	{
		this.m_overPlayfield = true;
		this.UpdateActorState();
	}

	// Token: 0x06001774 RID: 6004 RVA: 0x0006CB5F File Offset: 0x0006AD5F
	public void NotifyLeftPlayfield()
	{
		this.m_overPlayfield = false;
		this.UpdateActorState();
	}

	// Token: 0x06001775 RID: 6005 RVA: 0x0006CB70 File Offset: 0x0006AD70
	public void NotifyMousedOver()
	{
		this.m_mousedOver = true;
		this.UpdateActorState();
		this.UpdateProposedManaUsage();
		if (RemoteActionHandler.Get() && TargetReticleManager.Get())
		{
			RemoteActionHandler.Get().NotifyOpponentOfMouseOverEntity(this.GetEntity().GetCard());
		}
		if (GameState.Get() != null)
		{
			GameState.Get().GetGameEntity().NotifyOfCardMousedOver(this.GetEntity());
		}
		if (this.m_zone is ZoneHand)
		{
			Spell actorSpell = this.GetActorSpell(SpellType.SPELL_POWER_HINT_BURST, true);
			if (actorSpell != null)
			{
				actorSpell.Deactivate();
			}
			Spell actorSpell2 = this.GetActorSpell(SpellType.SPELL_POWER_HINT_IDLE, true);
			if (actorSpell2 != null)
			{
				actorSpell2.Deactivate();
			}
			if (GameState.Get().IsMulliganManagerActive())
			{
				SoundManager.Get().LoadAndPlay("collection_manager_card_mouse_over", base.gameObject);
			}
		}
		if (this.m_entity.IsControlledByFriendlySidePlayer() && (this.m_entity.IsHero() || this.m_zone is ZonePlay) && !this.m_transitioningZones)
		{
			bool flag = this.m_entity.HasSpellPower() || this.m_entity.HasSpellPowerDouble();
			bool flag2 = this.m_entity.HasHeroPowerDamage();
			if (flag || flag2)
			{
				Spell actorSpell3 = this.GetActorSpell(SpellType.SPELL_POWER_HINT_BURST, true);
				if (actorSpell3 != null)
				{
					actorSpell3.Reactivate();
				}
				if (flag)
				{
					ZoneHand zoneHand = ZoneMgr.Get().FindZoneOfType<ZoneHand>(this.m_zone.m_Side);
					zoneHand.OnSpellPowerEntityMousedOver();
				}
			}
		}
		if (this.m_entity.IsWeapon() && this.m_entity.IsExhausted() && this.m_actor != null && this.m_actor.GetAttackObject() != null)
		{
			this.m_actor.GetAttackObject().Enlarge(1f);
		}
	}

	// Token: 0x06001776 RID: 6006 RVA: 0x0006CD60 File Offset: 0x0006AF60
	public void NotifyMousedOut()
	{
		this.m_mousedOver = false;
		this.UpdateActorState();
		this.UpdateProposedManaUsage();
		if (RemoteActionHandler.Get())
		{
			RemoteActionHandler.Get().NotifyOpponentOfMouseOut();
		}
		if (KeywordHelpPanelManager.Get())
		{
			KeywordHelpPanelManager.Get().HideKeywordHelp();
		}
		if (CardTypeBanner.Get())
		{
			CardTypeBanner.Get().Hide(this.m_actor);
		}
		if (GameState.Get() != null)
		{
			GameState.Get().GetGameEntity().NotifyOfCardMousedOff(this.GetEntity());
		}
		if (this.m_entity.HasSpellPower() && this.m_entity.IsControlledByFriendlySidePlayer() && (this.m_entity.IsHero() || this.m_zone is ZonePlay) && !this.m_transitioningZones)
		{
			ZoneHand zoneHand = ZoneMgr.Get().FindZoneOfType<ZoneHand>(this.m_zone.m_Side);
			zoneHand.OnSpellPowerEntityMousedOut();
		}
		if (this.m_entity.IsWeapon() && this.m_entity.IsExhausted() && this.m_actor != null && this.m_actor.GetAttackObject() != null)
		{
			this.m_actor.GetAttackObject().ScaleToZero();
		}
	}

	// Token: 0x06001777 RID: 6007 RVA: 0x0006CEAE File Offset: 0x0006B0AE
	public bool IsMousedOver()
	{
		return this.m_mousedOver;
	}

	// Token: 0x06001778 RID: 6008 RVA: 0x0006CEB6 File Offset: 0x0006B0B6
	public void NotifyOpponentMousedOverThisCard()
	{
		this.m_mousedOverByOpponent = true;
		this.UpdateActorState();
	}

	// Token: 0x06001779 RID: 6009 RVA: 0x0006CEC5 File Offset: 0x0006B0C5
	public void NotifyOpponentMousedOffThisCard()
	{
		this.m_mousedOverByOpponent = false;
		this.UpdateActorState();
	}

	// Token: 0x0600177A RID: 6010 RVA: 0x0006CED4 File Offset: 0x0006B0D4
	public void NotifyPickedUp()
	{
		this.m_transitioningZones = false;
		this.CutoffFriendlyCardDraw();
	}

	// Token: 0x0600177B RID: 6011 RVA: 0x0006CEE4 File Offset: 0x0006B0E4
	public void NotifyTargetingCanceled()
	{
		if (this.m_entity.IsCharacter() && !this.IsAttacking())
		{
			Spell actorAttackSpellForInput = this.GetActorAttackSpellForInput();
			if (actorAttackSpellForInput != null)
			{
				if (this.m_entity.HasTag(GAME_TAG.IMMUNE_WHILE_ATTACKING) && !this.m_entity.IsImmune())
				{
					this.GetActor().DeactivateSpell(SpellType.IMMUNE);
				}
				SpellStateType activeState = actorAttackSpellForInput.GetActiveState();
				if (activeState != SpellStateType.NONE && activeState != SpellStateType.CANCEL)
				{
					actorAttackSpellForInput.ActivateState(SpellStateType.CANCEL);
				}
			}
		}
		this.ActivateHandStateSpells();
	}

	// Token: 0x0600177C RID: 6012 RVA: 0x0006CF72 File Offset: 0x0006B172
	public bool IsInputEnabled()
	{
		return this.m_inputEnabled;
	}

	// Token: 0x0600177D RID: 6013 RVA: 0x0006CF7A File Offset: 0x0006B17A
	public void SetInputEnabled(bool enabled)
	{
		this.m_inputEnabled = enabled;
		this.UpdateActorState();
	}

	// Token: 0x0600177E RID: 6014 RVA: 0x0006CF8C File Offset: 0x0006B18C
	public bool IsAllowedToShowTooltip()
	{
		return !(this.m_zone == null) && (this.m_zone.m_ServerTag == TAG_ZONE.PLAY || this.m_zone.m_ServerTag == TAG_ZONE.SECRET || this.m_zone.m_ServerTag != TAG_ZONE.HAND || this.m_zone.m_Side == Player.Side.OPPOSING) && (GameState.Get() == null || !this.m_entity.IsHero() || GameState.Get().GetGameEntity().ShouldShowHeroTooltips());
	}

	// Token: 0x0600177F RID: 6015 RVA: 0x0006D023 File Offset: 0x0006B223
	public bool IsAbleToShowTooltip()
	{
		return this.m_entity != null && !(this.m_actor == null) && !(BigCard.Get() == null);
	}

	// Token: 0x06001780 RID: 6016 RVA: 0x0006D058 File Offset: 0x0006B258
	public bool GetShouldShowTooltip()
	{
		return this.m_shouldShowTooltip;
	}

	// Token: 0x06001781 RID: 6017 RVA: 0x0006D060 File Offset: 0x0006B260
	public void SetShouldShowTooltip()
	{
		if (!this.IsAllowedToShowTooltip())
		{
			return;
		}
		if (this.m_shouldShowTooltip)
		{
			return;
		}
		this.m_shouldShowTooltip = true;
	}

	// Token: 0x06001782 RID: 6018 RVA: 0x0006D081 File Offset: 0x0006B281
	public void ShowTooltip()
	{
		if (this.m_showTooltip)
		{
			return;
		}
		this.m_showTooltip = true;
		this.UpdateTooltip();
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x0006D09C File Offset: 0x0006B29C
	public void HideTooltip()
	{
		this.m_shouldShowTooltip = false;
		if (!this.m_showTooltip)
		{
			return;
		}
		this.m_showTooltip = false;
		this.UpdateTooltip();
	}

	// Token: 0x06001784 RID: 6020 RVA: 0x0006D0C9 File Offset: 0x0006B2C9
	public bool IsShowingTooltip()
	{
		return this.m_showTooltip;
	}

	// Token: 0x06001785 RID: 6021 RVA: 0x0006D0D4 File Offset: 0x0006B2D4
	public void UpdateTooltip()
	{
		bool flag = this.GetShouldShowTooltip() && this.IsAllowedToShowTooltip() && this.IsAbleToShowTooltip();
		if (flag && this.m_showTooltip)
		{
			if (BigCard.Get() != null)
			{
				BigCard.Get().Show(this);
			}
		}
		else
		{
			this.m_showTooltip = false;
			this.m_shouldShowTooltip = false;
			if (BigCard.Get() != null)
			{
				BigCard.Get().Hide(this);
			}
		}
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x0006D15C File Offset: 0x0006B35C
	public bool IsAttacking()
	{
		return this.m_attacking;
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x0006D164 File Offset: 0x0006B364
	public void EnableAttacking(bool enable)
	{
		this.m_attacking = enable;
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x0006D16D File Offset: 0x0006B36D
	public bool WillIgnoreDeath()
	{
		return this.m_ignoreDeath;
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x0006D175 File Offset: 0x0006B375
	public void IgnoreDeath(bool ignore)
	{
		this.m_ignoreDeath = ignore;
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x0006D17E File Offset: 0x0006B37E
	public bool WillSuppressDeathEffects()
	{
		return this.m_suppressDeathEffects;
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x0006D186 File Offset: 0x0006B386
	public void SuppressDeathEffects(bool suppress)
	{
		this.m_suppressDeathEffects = suppress;
	}

	// Token: 0x0600178C RID: 6028 RVA: 0x0006D18F File Offset: 0x0006B38F
	public bool WillSuppressDeathSounds()
	{
		return this.m_suppressDeathSounds;
	}

	// Token: 0x0600178D RID: 6029 RVA: 0x0006D197 File Offset: 0x0006B397
	public void SuppressDeathSounds(bool suppress)
	{
		this.m_suppressDeathSounds = suppress;
	}

	// Token: 0x0600178E RID: 6030 RVA: 0x0006D1A0 File Offset: 0x0006B3A0
	public bool WillSuppressKeywordDeaths()
	{
		return this.m_suppressKeywordDeaths;
	}

	// Token: 0x0600178F RID: 6031 RVA: 0x0006D1A8 File Offset: 0x0006B3A8
	public void SuppressKeywordDeaths(bool suppress)
	{
		this.m_suppressKeywordDeaths = suppress;
	}

	// Token: 0x06001790 RID: 6032 RVA: 0x0006D1B1 File Offset: 0x0006B3B1
	public float GetKeywordDeathDelaySec()
	{
		return this.m_keywordDeathDelaySec;
	}

	// Token: 0x06001791 RID: 6033 RVA: 0x0006D1B9 File Offset: 0x0006B3B9
	public void SetKeywordDeathDelaySec(float sec)
	{
		this.m_keywordDeathDelaySec = sec;
	}

	// Token: 0x06001792 RID: 6034 RVA: 0x0006D1C2 File Offset: 0x0006B3C2
	public bool WillSuppressActorTriggerSpell()
	{
		return this.m_suppressActorTriggerSpell;
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x0006D1CA File Offset: 0x0006B3CA
	public void SuppressActorTriggerSpell(bool suppress)
	{
		this.m_suppressActorTriggerSpell = suppress;
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x0006D1D3 File Offset: 0x0006B3D3
	public bool WillSuppressPlaySounds()
	{
		return this.m_suppressPlaySounds;
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x0006D1DB File Offset: 0x0006B3DB
	public void SuppressPlaySounds(bool suppress)
	{
		this.m_suppressPlaySounds = suppress;
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x0006D1E4 File Offset: 0x0006B3E4
	public bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x0006D1EC File Offset: 0x0006B3EC
	public void ShowCard()
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_shown = true;
		this.ShowImpl();
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x0006D207 File Offset: 0x0006B407
	private void ShowImpl()
	{
		if (this.m_actor == null)
		{
			return;
		}
		this.m_actor.Show();
		this.RefreshActor();
	}

	// Token: 0x06001799 RID: 6041 RVA: 0x0006D22C File Offset: 0x0006B42C
	public void HideCard()
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		this.HideImpl();
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x0006D247 File Offset: 0x0006B447
	private void HideImpl()
	{
		if (this.m_actor == null)
		{
			return;
		}
		this.m_actor.Hide();
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x0006D268 File Offset: 0x0006B468
	public void SetBattleCrySource(bool source)
	{
		this.m_isBattleCrySource = source;
		if (this.m_actor != null)
		{
			if (source)
			{
				SceneUtils.SetLayer(this.m_actor.gameObject, GameLayer.IgnoreFullScreenEffects);
			}
			else
			{
				SceneUtils.SetLayer(this.m_actor.gameObject, GameLayer.Default);
				SceneUtils.SetLayer(this.m_actor.GetMeshRenderer().gameObject, GameLayer.CardRaycast);
			}
		}
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x0006D2D1 File Offset: 0x0006B4D1
	public void DoTauntNotification()
	{
		iTween.PunchScale(this.m_actor.gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x0006D2FC File Offset: 0x0006B4FC
	public void UpdateProposedManaUsage()
	{
		if (GameState.Get().GetSelectedOption() != -1)
		{
			return;
		}
		Player player = GameState.Get().GetPlayer(this.GetEntity().GetControllerId());
		if (!player.IsFriendlySide())
		{
			return;
		}
		if (!player.HasTag(GAME_TAG.CURRENT_PLAYER))
		{
			return;
		}
		if (this.m_mousedOver)
		{
			bool flag = this.m_entity.GetZone() == TAG_ZONE.HAND;
			bool flag2 = this.m_entity.IsHeroPower();
			if (!flag && !flag2)
			{
				return;
			}
			if (!GameState.Get().IsOption(this.m_entity))
			{
				return;
			}
			if (this.m_entity.IsSpell() && player.HasTag(GAME_TAG.SPELLS_COST_HEALTH))
			{
				return;
			}
			player.ProposeManaCrystalUsage(this.m_entity);
		}
		else
		{
			player.CancelAllProposedMana(this.m_entity);
		}
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x0006D3D1 File Offset: 0x0006B5D1
	public CardDef GetCardDef()
	{
		return this.m_cardDef;
	}

	// Token: 0x0600179F RID: 6047 RVA: 0x0006D3DC File Offset: 0x0006B5DC
	public void LoadCardDef(CardDef cardDef)
	{
		if (this.m_cardDef == cardDef)
		{
			return;
		}
		this.m_cardDef = cardDef;
		this.InitCardDefAssets();
		if (this.m_actor != null)
		{
			this.m_actor.SetCardDef(this.m_cardDef);
			this.m_actor.UpdateAllComponents();
		}
	}

	// Token: 0x060017A0 RID: 6048 RVA: 0x0006D438 File Offset: 0x0006B638
	public void PurgeSpells()
	{
		foreach (CardEffect cardEffect in this.m_allEffects)
		{
			cardEffect.PurgeSpells();
		}
	}

	// Token: 0x060017A1 RID: 6049 RVA: 0x0006D494 File Offset: 0x0006B694
	private bool ShouldPreloadCardAssets()
	{
		return !ApplicationMgr.IsPublic() && Options.Get().GetBool(Option.PRELOAD_CARD_ASSETS, false);
	}

	// Token: 0x060017A2 RID: 6050 RVA: 0x0006D4AF File Offset: 0x0006B6AF
	public void OverrideCustomSpawnSpell(Spell spell)
	{
		this.m_customSpawnSpellOverride = this.SetupOverrideSpell(this.m_customSpawnSpellOverride, spell);
	}

	// Token: 0x060017A3 RID: 6051 RVA: 0x0006D4C4 File Offset: 0x0006B6C4
	public void OverrideCustomDeathSpell(Spell spell)
	{
		this.m_customDeathSpellOverride = this.SetupOverrideSpell(this.m_customDeathSpellOverride, spell);
	}

	// Token: 0x060017A4 RID: 6052 RVA: 0x0006D4DC File Offset: 0x0006B6DC
	public Texture GetPortraitTexture()
	{
		return (!(this.m_cardDef == null)) ? this.m_cardDef.GetPortraitTexture() : null;
	}

	// Token: 0x060017A5 RID: 6053 RVA: 0x0006D50C File Offset: 0x0006B70C
	public Material GetGoldenMaterial()
	{
		return (!(this.m_cardDef == null)) ? this.m_cardDef.GetPremiumPortraitMaterial() : null;
	}

	// Token: 0x060017A6 RID: 6054 RVA: 0x0006D53B File Offset: 0x0006B73B
	public CardEffect GetPlayEffect()
	{
		return this.m_playEffect;
	}

	// Token: 0x060017A7 RID: 6055 RVA: 0x0006D543 File Offset: 0x0006B743
	public Spell GetPlaySpell(bool loadIfNeeded = true)
	{
		if (this.m_playEffect == null)
		{
			return null;
		}
		return this.m_playEffect.GetSpell(loadIfNeeded);
	}

	// Token: 0x060017A8 RID: 6056 RVA: 0x0006D55E File Offset: 0x0006B75E
	public List<CardSoundSpell> GetPlaySoundSpells(bool loadIfNeeded = true)
	{
		if (this.m_playEffect == null)
		{
			return null;
		}
		return this.m_playEffect.GetSoundSpells(loadIfNeeded);
	}

	// Token: 0x060017A9 RID: 6057 RVA: 0x0006D579 File Offset: 0x0006B779
	public Spell GetAttackSpell(bool loadIfNeeded = true)
	{
		if (this.m_attackEffect == null)
		{
			return null;
		}
		return this.m_attackEffect.GetSpell(loadIfNeeded);
	}

	// Token: 0x060017AA RID: 6058 RVA: 0x0006D594 File Offset: 0x0006B794
	public List<CardSoundSpell> GetAttackSoundSpells(bool loadIfNeeded = true)
	{
		if (this.m_attackEffect == null)
		{
			return null;
		}
		return this.m_attackEffect.GetSoundSpells(loadIfNeeded);
	}

	// Token: 0x060017AB RID: 6059 RVA: 0x0006D5AF File Offset: 0x0006B7AF
	public List<CardSoundSpell> GetDeathSoundSpells(bool loadIfNeeded = true)
	{
		if (this.m_deathEffect == null)
		{
			return null;
		}
		return this.m_deathEffect.GetSoundSpells(loadIfNeeded);
	}

	// Token: 0x060017AC RID: 6060 RVA: 0x0006D5CA File Offset: 0x0006B7CA
	public Spell GetLifetimeSpell(bool loadIfNeeded = true)
	{
		if (this.m_lifetimeEffect == null)
		{
			return null;
		}
		return this.m_lifetimeEffect.GetSpell(loadIfNeeded);
	}

	// Token: 0x060017AD RID: 6061 RVA: 0x0006D5E5 File Offset: 0x0006B7E5
	public List<CardSoundSpell> GetLifetimeSoundSpells(bool loadIfNeeded = true)
	{
		if (this.m_lifetimeEffect == null)
		{
			return null;
		}
		return this.m_lifetimeEffect.GetSoundSpells(loadIfNeeded);
	}

	// Token: 0x060017AE RID: 6062 RVA: 0x0006D600 File Offset: 0x0006B800
	public CardEffect GetSubOptionEffect(int index)
	{
		if (this.m_subOptionEffects == null)
		{
			return null;
		}
		if (index < 0)
		{
			return null;
		}
		if (index >= this.m_subOptionEffects.Count)
		{
			return null;
		}
		return this.m_subOptionEffects[index];
	}

	// Token: 0x060017AF RID: 6063 RVA: 0x0006D638 File Offset: 0x0006B838
	public Spell GetSubOptionSpell(int index, bool loadIfNeeded = true)
	{
		CardEffect subOptionEffect = this.GetSubOptionEffect(index);
		if (subOptionEffect == null)
		{
			return null;
		}
		return subOptionEffect.GetSpell(loadIfNeeded);
	}

	// Token: 0x060017B0 RID: 6064 RVA: 0x0006D65C File Offset: 0x0006B85C
	public List<CardSoundSpell> GetSubOptionSoundSpells(int index, bool loadIfNeeded = true)
	{
		CardEffect subOptionEffect = this.GetSubOptionEffect(index);
		if (subOptionEffect == null)
		{
			return null;
		}
		return subOptionEffect.GetSoundSpells(loadIfNeeded);
	}

	// Token: 0x060017B1 RID: 6065 RVA: 0x0006D680 File Offset: 0x0006B880
	public CardEffect GetTriggerEffect(int index)
	{
		if (this.m_triggerEffects == null)
		{
			return null;
		}
		if (index < 0)
		{
			return null;
		}
		if (index >= this.m_triggerEffects.Count)
		{
			return null;
		}
		return this.m_triggerEffects[index];
	}

	// Token: 0x060017B2 RID: 6066 RVA: 0x0006D6B8 File Offset: 0x0006B8B8
	public Spell GetTriggerSpell(int index, bool loadIfNeeded = true)
	{
		CardEffect triggerEffect = this.GetTriggerEffect(index);
		if (triggerEffect == null)
		{
			return null;
		}
		return triggerEffect.GetSpell(loadIfNeeded);
	}

	// Token: 0x060017B3 RID: 6067 RVA: 0x0006D6DC File Offset: 0x0006B8DC
	public List<CardSoundSpell> GetTriggerSoundSpells(int index, bool loadIfNeeded = true)
	{
		CardEffect triggerEffect = this.GetTriggerEffect(index);
		if (triggerEffect == null)
		{
			return null;
		}
		return triggerEffect.GetSoundSpells(loadIfNeeded);
	}

	// Token: 0x060017B4 RID: 6068 RVA: 0x0006D700 File Offset: 0x0006B900
	public Spell GetCustomSummonSpell()
	{
		return this.m_customSummonSpell;
	}

	// Token: 0x060017B5 RID: 6069 RVA: 0x0006D708 File Offset: 0x0006B908
	public Spell GetCustomSpawnSpell()
	{
		return this.m_customSpawnSpell;
	}

	// Token: 0x060017B6 RID: 6070 RVA: 0x0006D710 File Offset: 0x0006B910
	public Spell GetCustomSpawnSpellOverride()
	{
		return this.m_customSpawnSpellOverride;
	}

	// Token: 0x060017B7 RID: 6071 RVA: 0x0006D718 File Offset: 0x0006B918
	public Spell GetCustomDeathSpell()
	{
		return this.m_customDeathSpell;
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x0006D720 File Offset: 0x0006B920
	public Spell GetCustomDeathSpellOverride()
	{
		return this.m_customDeathSpellOverride;
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x0006D728 File Offset: 0x0006B928
	public AudioSource GetAnnouncerLine()
	{
		return this.m_announcerLine.GetAudio();
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x0006D738 File Offset: 0x0006B938
	public EmoteEntry GetEmoteEntry(EmoteType emoteType)
	{
		if (this.m_emotes == null)
		{
			return null;
		}
		if (SpecialEventManager.Get().IsEventActive(SpecialEventType.LUNAR_NEW_YEAR, false))
		{
			if (emoteType == EmoteType.GREETINGS)
			{
				foreach (EmoteEntry emoteEntry in this.m_emotes)
				{
					if (emoteEntry.GetEmoteType() == EmoteType.EVENT_LUNAR_NEW_YEAR)
					{
						return emoteEntry;
					}
				}
			}
		}
		else if (SpecialEventManager.Get().IsEventActive(SpecialEventType.FEAST_OF_WINTER_VEIL, false) && emoteType == EmoteType.GREETINGS)
		{
			foreach (EmoteEntry emoteEntry2 in this.m_emotes)
			{
				if (emoteEntry2.GetEmoteType() == EmoteType.EVENT_WINTER_VEIL)
				{
					return emoteEntry2;
				}
			}
		}
		foreach (EmoteEntry emoteEntry3 in this.m_emotes)
		{
			if (emoteEntry3.GetEmoteType() == emoteType)
			{
				return emoteEntry3;
			}
		}
		return null;
	}

	// Token: 0x060017BB RID: 6075 RVA: 0x0006D89C File Offset: 0x0006BA9C
	public Spell GetBestSummonSpell()
	{
		bool flag;
		return this.GetBestSummonSpell(out flag);
	}

	// Token: 0x060017BC RID: 6076 RVA: 0x0006D8B4 File Offset: 0x0006BAB4
	public Spell GetBestSummonSpell(out bool standard)
	{
		if (this.m_customSummonSpell != null)
		{
			standard = false;
			return this.m_customSummonSpell;
		}
		standard = true;
		SpellType spellType = this.m_cardDef.DetermineSummonInSpell_HandToPlay(this);
		return this.GetActorSpell(spellType, true);
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x0006D8F4 File Offset: 0x0006BAF4
	public Spell GetBestSpawnSpell()
	{
		bool flag;
		return this.GetBestSpawnSpell(out flag);
	}

	// Token: 0x060017BE RID: 6078 RVA: 0x0006D90C File Offset: 0x0006BB0C
	public Spell GetBestSpawnSpell(out bool standard)
	{
		standard = false;
		if (this.m_customSpawnSpellOverride)
		{
			return this.m_customSpawnSpellOverride;
		}
		if (this.m_customSpawnSpell)
		{
			return this.m_customSpawnSpell;
		}
		standard = true;
		if (this.m_entity.IsControlledByFriendlySidePlayer())
		{
			return this.GetActorSpell(SpellType.FRIENDLY_SPAWN_MINION, true);
		}
		return this.GetActorSpell(SpellType.OPPONENT_SPAWN_MINION, true);
	}

	// Token: 0x060017BF RID: 6079 RVA: 0x0006D970 File Offset: 0x0006BB70
	public Spell GetBestDeathSpell()
	{
		bool flag;
		return this.GetBestDeathSpell(out flag);
	}

	// Token: 0x060017C0 RID: 6080 RVA: 0x0006D985 File Offset: 0x0006BB85
	public Spell GetBestDeathSpell(out bool standard)
	{
		return this.GetBestDeathSpell(this.m_actor, out standard);
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x0006D994 File Offset: 0x0006BB94
	private Spell GetBestDeathSpell(Actor actor)
	{
		bool flag;
		return this.GetBestDeathSpell(actor, out flag);
	}

	// Token: 0x060017C2 RID: 6082 RVA: 0x0006D9AC File Offset: 0x0006BBAC
	private Spell GetBestDeathSpell(Actor actor, out bool standard)
	{
		standard = false;
		if (!this.m_entity.IsSilenced())
		{
			if (this.m_customDeathSpellOverride)
			{
				return this.m_customDeathSpellOverride;
			}
			if (this.m_customDeathSpell)
			{
				return this.m_customDeathSpell;
			}
		}
		standard = true;
		return actor.GetSpell(SpellType.DEATH);
	}

	// Token: 0x060017C3 RID: 6083 RVA: 0x0006DA04 File Offset: 0x0006BC04
	public void ActivateCharacterPlayEffects()
	{
		if (!this.m_suppressPlaySounds)
		{
			this.ActivateSoundSpellList(this.m_playEffect.GetSoundSpells(true));
			this.m_suppressPlaySounds = false;
		}
		this.ActivateLifetimeEffects();
	}

	// Token: 0x060017C4 RID: 6084 RVA: 0x0006DA31 File Offset: 0x0006BC31
	public void ActivateCharacterAttackEffects()
	{
		this.ActivateSoundSpellList(this.m_attackEffect.GetSoundSpells(true));
	}

	// Token: 0x060017C5 RID: 6085 RVA: 0x0006DA48 File Offset: 0x0006BC48
	public void ActivateCharacterDeathEffects()
	{
		if (this.m_suppressDeathEffects)
		{
			return;
		}
		if (!this.m_suppressDeathSounds)
		{
			int num;
			if (this.m_emotes == null)
			{
				num = -1;
			}
			else
			{
				num = this.m_emotes.FindIndex((EmoteEntry e) => e != null && e.GetEmoteType() == EmoteType.DEATH_LINE);
			}
			int num2 = num;
			if (num2 >= 0)
			{
				this.PlayEmote(EmoteType.DEATH_LINE);
			}
			else
			{
				this.ActivateSoundSpellList(this.m_deathEffect.GetSoundSpells(true));
			}
			this.m_suppressDeathSounds = false;
		}
		this.DeactivateLifetimeEffects();
	}

	// Token: 0x060017C6 RID: 6086 RVA: 0x0006DADC File Offset: 0x0006BCDC
	public void ActivateLifetimeEffects()
	{
		if (this.m_lifetimeEffect == null)
		{
			return;
		}
		if (this.m_entity.IsSilenced())
		{
			return;
		}
		Spell spell = this.m_lifetimeEffect.GetSpell(true);
		if (spell != null)
		{
			spell.Deactivate();
			spell.ActivateState(SpellStateType.BIRTH);
		}
		if (this.m_lifetimeEffect.GetSoundSpells(true) != null)
		{
			this.ActivateSoundSpellList(this.m_lifetimeEffect.GetSoundSpells(true));
		}
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x0006DB50 File Offset: 0x0006BD50
	public void DeactivateLifetimeEffects()
	{
		if (this.m_lifetimeEffect == null)
		{
			return;
		}
		Spell spell = this.m_lifetimeEffect.GetSpell(true);
		if (spell != null)
		{
			SpellStateType activeState = spell.GetActiveState();
			if (activeState != SpellStateType.NONE && activeState != SpellStateType.DEATH)
			{
				spell.ActivateState(SpellStateType.DEATH);
			}
		}
	}

	// Token: 0x060017C8 RID: 6088 RVA: 0x0006DBA0 File Offset: 0x0006BDA0
	public void ActivateCustomKeywordEffect()
	{
		if (this.m_customKeywordSpell == null)
		{
			return;
		}
		Spell spell = this.m_customKeywordSpell.GetSpell(true);
		if (spell == null)
		{
			Debug.LogWarning(string.Format("Card.ActivateCustomKeywordEffect() -- failed to load custom keyword spell for card {0}", this.ToString()));
			return;
		}
		spell.transform.parent = this.m_actor.transform;
		spell.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x0006DC08 File Offset: 0x0006BE08
	public void DeactivateCustomKeywordEffect()
	{
		if (this.m_customKeywordSpell == null)
		{
			return;
		}
		Spell spell = this.m_customKeywordSpell.GetSpell(false);
		if (spell == null)
		{
			return;
		}
		if (!spell.IsActive())
		{
			return;
		}
		spell.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x060017CA RID: 6090 RVA: 0x0006DC50 File Offset: 0x0006BE50
	public bool ActivateSoundSpellList(List<CardSoundSpell> soundSpells)
	{
		if (soundSpells == null)
		{
			return false;
		}
		if (soundSpells.Count == 0)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < soundSpells.Count; i++)
		{
			CardSoundSpell soundSpell = soundSpells[i];
			this.ActivateSoundSpell(soundSpell);
			result = true;
		}
		return result;
	}

	// Token: 0x060017CB RID: 6091 RVA: 0x0006DCA0 File Offset: 0x0006BEA0
	public bool ActivateSoundSpell(CardSoundSpell soundSpell)
	{
		if (soundSpell == null)
		{
			return false;
		}
		GameEntity gameEntity = GameState.Get().GetGameEntity();
		if (gameEntity.ShouldDelayCardSoundSpells())
		{
			base.StartCoroutine(this.WaitThenActivateSoundSpell(soundSpell));
		}
		else
		{
			soundSpell.Reactivate();
		}
		return true;
	}

	// Token: 0x060017CC RID: 6092 RVA: 0x0006DCEC File Offset: 0x0006BEEC
	public bool HasActiveEmoteSound()
	{
		if (this.m_emotes == null)
		{
			return false;
		}
		foreach (EmoteEntry emoteEntry in this.m_emotes)
		{
			CardSoundSpell spell = emoteEntry.GetSpell(false);
			if (spell != null && spell.IsActive())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060017CD RID: 6093 RVA: 0x0006DD78 File Offset: 0x0006BF78
	public CardSoundSpell PlayEmote(EmoteType emoteType)
	{
		return this.PlayEmote(emoteType, Notification.SpeechBubbleDirection.None);
	}

	// Token: 0x060017CE RID: 6094 RVA: 0x0006DD84 File Offset: 0x0006BF84
	public CardSoundSpell PlayEmote(EmoteType emoteType, Notification.SpeechBubbleDirection overrideDirection)
	{
		EmoteEntry emoteEntry = this.GetEmoteEntry(emoteType);
		CardSoundSpell cardSoundSpell = (emoteEntry != null) ? emoteEntry.GetSpell(true) : null;
		if (!this.m_entity.IsHero())
		{
			return null;
		}
		if (this.m_actor == null)
		{
			return null;
		}
		if (cardSoundSpell != null)
		{
			cardSoundSpell.Reactivate();
			if (cardSoundSpell.IsActive())
			{
				for (int i = 0; i < this.m_emotes.Count; i++)
				{
					EmoteEntry emoteEntry2 = this.m_emotes[i];
					if (emoteEntry2 != emoteEntry)
					{
						Spell spell = emoteEntry2.GetSpell(false);
						if (spell)
						{
							spell.Deactivate();
						}
					}
				}
			}
			GameState.Get().GetGameEntity().OnEmotePlayed(this, emoteType, cardSoundSpell);
		}
		Notification.SpeechBubbleDirection direction = Notification.SpeechBubbleDirection.BottomLeft;
		if (this.GetEntity().IsControlledByOpposingSidePlayer())
		{
			direction = Notification.SpeechBubbleDirection.TopRight;
		}
		if (overrideDirection != Notification.SpeechBubbleDirection.None)
		{
			direction = overrideDirection;
		}
		string text = (!(cardSoundSpell == null)) ? string.Empty : null;
		if (emoteEntry != null && !string.IsNullOrEmpty(emoteEntry.GetGameStringKey()))
		{
			text = GameStrings.Get(emoteEntry.GetGameStringKey());
		}
		if (text != null)
		{
			Notification notification = NotificationManager.Get().CreateSpeechBubble(text, direction, this.m_actor, true, true);
			float num = 1.5f;
			if (cardSoundSpell)
			{
				AudioSource activeAudioSource = cardSoundSpell.GetActiveAudioSource();
				if (activeAudioSource && num < activeAudioSource.clip.length)
				{
					num = activeAudioSource.clip.length;
				}
			}
			NotificationManager.Get().DestroyNotification(notification, num);
		}
		return cardSoundSpell;
	}

	// Token: 0x060017CF RID: 6095 RVA: 0x0006DF28 File Offset: 0x0006C128
	private void InitCardDefAssets()
	{
		this.InitEffect(this.m_cardDef.m_PlayEffectDef, ref this.m_playEffect);
		this.InitEffect(this.m_cardDef.m_AttackEffectDef, ref this.m_attackEffect);
		this.InitEffect(this.m_cardDef.m_DeathEffectDef, ref this.m_deathEffect);
		this.InitEffect(this.m_cardDef.m_LifetimeEffectDef, ref this.m_lifetimeEffect);
		this.InitEffect(this.m_cardDef.m_CustomKeywordSpellPath, ref this.m_customKeywordSpell);
		this.InitEffectList(this.m_cardDef.m_SubOptionEffectDefs, ref this.m_subOptionEffects);
		this.InitEffectList(this.m_cardDef.m_TriggerEffectDefs, ref this.m_triggerEffects);
		this.InitSound(this.m_cardDef.m_AnnouncerLinePath, ref this.m_announcerLine);
		this.InitEmoteList();
	}

	// Token: 0x060017D0 RID: 6096 RVA: 0x0006DFF4 File Offset: 0x0006C1F4
	private void InitEffect(CardEffectDef effectDef, ref CardEffect effect)
	{
		this.DestroyCardEffect(ref effect);
		if (effectDef == null)
		{
			return;
		}
		effect = new CardEffect(effectDef, this);
		if (this.m_allEffects == null)
		{
			this.m_allEffects = new List<CardEffect>();
		}
		this.m_allEffects.Add(effect);
		if (this.ShouldPreloadCardAssets())
		{
			effect.LoadAll();
		}
	}

	// Token: 0x060017D1 RID: 6097 RVA: 0x0006E050 File Offset: 0x0006C250
	private void InitEffect(string spellPath, ref CardEffect effect)
	{
		this.DestroyCardEffect(ref effect);
		if (spellPath == null)
		{
			return;
		}
		effect = new CardEffect(spellPath, this);
		if (this.m_allEffects == null)
		{
			this.m_allEffects = new List<CardEffect>();
		}
		this.m_allEffects.Add(effect);
		if (this.ShouldPreloadCardAssets())
		{
			effect.LoadAll();
		}
	}

	// Token: 0x060017D2 RID: 6098 RVA: 0x0006E0AC File Offset: 0x0006C2AC
	private void InitEffectList(List<CardEffectDef> effectDefs, ref List<CardEffect> effects)
	{
		this.DestroyCardEffectList(ref effects);
		if (effectDefs == null)
		{
			return;
		}
		effects = new List<CardEffect>();
		for (int i = 0; i < effectDefs.Count; i++)
		{
			CardEffectDef cardEffectDef = effectDefs[i];
			CardEffect cardEffect = null;
			if (cardEffectDef != null)
			{
				cardEffect = new CardEffect(cardEffectDef, this);
				if (this.m_allEffects == null)
				{
					this.m_allEffects = new List<CardEffect>();
				}
				this.m_allEffects.Add(cardEffect);
				if (this.ShouldPreloadCardAssets())
				{
					cardEffect.LoadAll();
				}
			}
			effects.Add(cardEffect);
		}
	}

	// Token: 0x060017D3 RID: 6099 RVA: 0x0006E138 File Offset: 0x0006C338
	private void InitSound(string path, ref CardAudio audio)
	{
		this.DestroyCardAudio(ref audio);
		audio = new CardAudio(path);
		if (this.ShouldPreloadCardAssets())
		{
			audio.GetAudio();
		}
	}

	// Token: 0x060017D4 RID: 6100 RVA: 0x0006E168 File Offset: 0x0006C368
	private void InitEmoteList()
	{
		this.DestroyEmoteList();
		if (this.m_cardDef.m_EmoteDefs == null)
		{
			return;
		}
		this.m_emotes = new List<EmoteEntry>();
		for (int i = 0; i < this.m_cardDef.m_EmoteDefs.Count; i++)
		{
			EmoteEntryDef emoteEntryDef = this.m_cardDef.m_EmoteDefs[i];
			EmoteEntry emoteEntry = new EmoteEntry(emoteEntryDef.m_emoteType, emoteEntryDef.m_emoteSoundSpellPath, emoteEntryDef.m_emoteGameStringKey, this);
			if (this.ShouldPreloadCardAssets())
			{
				emoteEntry.GetSpell(true);
			}
			this.m_emotes.Add(emoteEntry);
		}
	}

	// Token: 0x060017D5 RID: 6101 RVA: 0x0006E202 File Offset: 0x0006C402
	private Spell SetupOverrideSpell(Spell existingSpell, Spell spell)
	{
		if (existingSpell != null)
		{
			Object.Destroy(existingSpell.gameObject);
		}
		SpellUtils.SetupSpell(spell, this);
		return spell;
	}

	// Token: 0x060017D6 RID: 6102 RVA: 0x0006E224 File Offset: 0x0006C424
	private AudioSource SetupSound(AudioSource sound)
	{
		sound.transform.parent = base.transform;
		TransformUtil.Identity(sound.transform);
		return sound;
	}

	// Token: 0x060017D7 RID: 6103 RVA: 0x0006E250 File Offset: 0x0006C450
	private void DestroyCardDefAssets()
	{
		this.DestroyCardEffect(ref this.m_playEffect);
		this.DestroyCardEffect(ref this.m_attackEffect);
		this.DestroyCardEffect(ref this.m_deathEffect);
		this.DestroyCardEffect(ref this.m_lifetimeEffect);
		this.DestroyCardEffectList(ref this.m_subOptionEffects);
		this.DestroyCardEffectList(ref this.m_triggerEffects);
		this.DestroyCardEffect(ref this.m_customKeywordSpell);
		this.DestroyCardAudio(ref this.m_announcerLine);
		this.DestroyEmoteList();
		this.DestroyCardAsset<Spell>(ref this.m_customSummonSpell);
		this.DestroyCardAsset<Spell>(ref this.m_customSpawnSpell);
		this.DestroyCardAsset<Spell>(ref this.m_customSpawnSpellOverride);
		this.DestroyCardAsset<Spell>(ref this.m_customDeathSpell);
		this.DestroyCardAsset<Spell>(ref this.m_customDeathSpellOverride);
	}

	// Token: 0x060017D8 RID: 6104 RVA: 0x0006E2FF File Offset: 0x0006C4FF
	private void DestroyCardEffect(ref CardEffect effect)
	{
		if (effect == null)
		{
			return;
		}
		effect.Clear();
		effect = null;
	}

	// Token: 0x060017D9 RID: 6105 RVA: 0x0006E313 File Offset: 0x0006C513
	private void DestroyCardAudio(ref CardAudio cardAudio)
	{
		if (cardAudio == null)
		{
			return;
		}
		cardAudio.Clear();
		cardAudio = null;
	}

	// Token: 0x060017DA RID: 6106 RVA: 0x0006E328 File Offset: 0x0006C528
	private void DestroyCardEffectList(ref List<CardEffect> effects)
	{
		if (effects == null)
		{
			return;
		}
		foreach (CardEffect cardEffect in effects)
		{
			cardEffect.Clear();
		}
		effects = null;
	}

	// Token: 0x060017DB RID: 6107 RVA: 0x0006E388 File Offset: 0x0006C588
	private void DestroyCardAsset<T>(ref T asset) where T : Component
	{
		if (asset == null)
		{
			return;
		}
		Object.Destroy(asset.gameObject);
		asset = (T)((object)null);
	}

	// Token: 0x060017DC RID: 6108 RVA: 0x0006E3CC File Offset: 0x0006C5CC
	private void DestroyCardAsset<T>(T asset) where T : Component
	{
		if (asset == null)
		{
			return;
		}
		Object.Destroy(asset.gameObject);
	}

	// Token: 0x060017DD RID: 6109 RVA: 0x0006E400 File Offset: 0x0006C600
	private void DestroySpellList<T>(List<T> spells) where T : Spell
	{
		if (spells == null)
		{
			return;
		}
		for (int i = 0; i < spells.Count; i++)
		{
			this.DestroyCardAsset<T>(spells[i]);
		}
		spells = null;
	}

	// Token: 0x060017DE RID: 6110 RVA: 0x0006E43C File Offset: 0x0006C63C
	private void DestroyEmoteList()
	{
		if (this.m_emotes == null)
		{
			return;
		}
		for (int i = 0; i < this.m_emotes.Count; i++)
		{
			this.m_emotes[i].Clear();
		}
		this.m_emotes = null;
	}

	// Token: 0x060017DF RID: 6111 RVA: 0x0006E48C File Offset: 0x0006C68C
	private void CancelActiveSpells()
	{
		this.CancelActiveSpell(this.m_playEffect.GetSpell(false));
		if (this.m_triggerEffects != null)
		{
			foreach (CardEffect cardEffect in this.m_triggerEffects)
			{
				this.CancelActiveSpell(cardEffect.GetSpell(false));
			}
		}
	}

	// Token: 0x060017E0 RID: 6112 RVA: 0x0006E50C File Offset: 0x0006C70C
	private void CancelActiveSpell(Spell spell)
	{
		if (spell == null)
		{
			return;
		}
		SpellStateType activeState = spell.GetActiveState();
		if (activeState == SpellStateType.NONE)
		{
			return;
		}
		if (activeState == SpellStateType.CANCEL)
		{
			return;
		}
		spell.ActivateState(SpellStateType.CANCEL);
	}

	// Token: 0x060017E1 RID: 6113 RVA: 0x0006E544 File Offset: 0x0006C744
	private IEnumerator WaitThenActivateSoundSpell(CardSoundSpell soundSpell)
	{
		GameEntity gameEntity = GameState.Get().GetGameEntity();
		while (gameEntity.ShouldDelayCardSoundSpells())
		{
			yield return null;
		}
		soundSpell.Reactivate();
		yield break;
	}

	// Token: 0x060017E2 RID: 6114 RVA: 0x0006E568 File Offset: 0x0006C768
	public void OnTagsChanged(TagDeltaSet changeSet)
	{
		bool flag = false;
		int i = 0;
		while (i < changeSet.Size())
		{
			TagDelta tagDelta = changeSet[i];
			GAME_TAG tag = (GAME_TAG)tagDelta.tag;
			switch (tag)
			{
			case GAME_TAG.HEALTH:
			case GAME_TAG.ATK:
			case GAME_TAG.COST:
				goto IL_4C;
			default:
				if (tag == GAME_TAG.DURABILITY || tag == GAME_TAG.ARMOR)
				{
					goto IL_4C;
				}
				this.OnTagChanged(tagDelta);
				break;
			}
			IL_5F:
			i++;
			continue;
			IL_4C:
			flag = true;
			goto IL_5F;
		}
		if (flag && !this.m_entity.IsLoadingAssets() && this.IsActorReady())
		{
			this.UpdateActorComponents();
		}
	}

	// Token: 0x060017E3 RID: 6115 RVA: 0x0006E60C File Offset: 0x0006C80C
	public void OnMetaData(Network.HistMetaData metaData)
	{
		if (metaData.MetaType == 1 || metaData.MetaType == 2)
		{
			if (!this.CanShowActorVisuals())
			{
				return;
			}
			if (this.m_entity.GetZone() != TAG_ZONE.PLAY)
			{
				return;
			}
			Spell actorSpell = this.GetActorSpell(SpellType.DAMAGE, true);
			if (actorSpell == null)
			{
				this.UpdateActorComponents();
				return;
			}
			actorSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_UpdateActorComponents));
			if (this.m_entity.IsCharacter())
			{
				int damage = (metaData.MetaType != 2) ? metaData.Data : (-metaData.Data);
				DamageSplatSpell damageSplatSpell = (DamageSplatSpell)actorSpell;
				damageSplatSpell.SetDamage(damage);
				actorSpell.ActivateState(SpellStateType.ACTION);
				BoardEvents boardEvents = BoardEvents.Get();
				if (boardEvents != null)
				{
					if (metaData.MetaType == 2)
					{
						boardEvents.HealEvent(this, (float)(-(float)metaData.Data));
					}
					else
					{
						boardEvents.DamageEvent(this, (float)metaData.Data);
					}
				}
			}
			else
			{
				actorSpell.Activate();
			}
		}
	}

	// Token: 0x060017E4 RID: 6116 RVA: 0x0006E70C File Offset: 0x0006C90C
	public void OnTagChanged(TagDelta change)
	{
		GAME_TAG tag = (GAME_TAG)change.tag;
		switch (tag)
		{
		case GAME_TAG.DURABILITY:
			goto IL_779;
		case GAME_TAG.SILENCED:
			if (!this.CanShowActorVisuals())
			{
				return;
			}
			if (change.newValue == 1)
			{
				this.m_actor.ActivateSpell(SpellType.SILENCE);
				this.DeactivateLifetimeEffects();
			}
			else
			{
				this.m_actor.DeactivateSpell(SpellType.SILENCE);
				this.ActivateLifetimeEffects();
			}
			return;
		case GAME_TAG.WINDFURY:
		{
			if (!this.CanShowActorVisuals())
			{
				return;
			}
			if (change.newValue >= 1)
			{
				this.m_actor.ActivateSpell(SpellType.WINDFURY_BURST);
			}
			Spell spell = this.m_actor.GetSpell(SpellType.WINDFURY_IDLE);
			if (spell != null)
			{
				if (change.newValue >= 1)
				{
					spell.ActivateState(SpellStateType.BIRTH);
				}
				else
				{
					spell.ActivateState(SpellStateType.CANCEL);
				}
			}
			return;
		}
		case GAME_TAG.TAUNT:
			break;
		case GAME_TAG.STEALTH:
			goto IL_545;
		case GAME_TAG.SPELLPOWER:
			goto IL_60C;
		default:
			switch (tag)
			{
			case GAME_TAG.EXHAUSTED:
				if (!this.CanShowActorVisuals())
				{
					return;
				}
				if (change.newValue == change.oldValue)
				{
					return;
				}
				if (GameState.Get().IsTurnStartManagerActive() && this.m_entity.IsControlledByFriendlySidePlayer())
				{
					TurnStartManager.Get().NotifyOfExhaustedChange(this, change);
				}
				else
				{
					this.ShowExhaustedChange(change.newValue);
				}
				return;
			case GAME_TAG.DAMAGE:
				if (!this.CanShowActorVisuals())
				{
					return;
				}
				if (this.m_entity.IsEnraged())
				{
					Spell spell2 = this.m_actor.GetSpell(SpellType.ENRAGE);
					if (!spell2.IsActive())
					{
						this.m_actor.ActivateSpell(SpellType.ENRAGE);
					}
				}
				else
				{
					this.m_actor.DeactivateSpell(SpellType.ENRAGE);
				}
				this.UpdateActorComponents();
				return;
			case GAME_TAG.HEALTH:
			case GAME_TAG.ATK:
			case GAME_TAG.COST:
				goto IL_779;
			default:
				switch (tag)
				{
				case GAME_TAG.AURA:
					if (!this.CanShowActorVisuals())
					{
						return;
					}
					if (change.newValue == 1)
					{
						this.m_actor.ActivateSpell(SpellType.AURA);
					}
					else
					{
						this.m_actor.DeactivateSpell(SpellType.AURA);
					}
					return;
				case GAME_TAG.POISONOUS:
					if (!this.CanShowActorVisuals())
					{
						return;
					}
					this.ToggleBauble(change.newValue == 1, SpellType.POISONOUS);
					return;
				default:
					switch (tag)
					{
					case GAME_TAG.OBFUSCATED:
						if (!this.CanShowActorVisuals())
						{
							return;
						}
						if (this.m_entity.GetController() != null && this.m_entity.GetController().IsFriendlySide())
						{
							return;
						}
						this.UpdateActor(false);
						return;
					case GAME_TAG.BURNING:
						if (!this.CanShowActorVisuals())
						{
							return;
						}
						if (change.oldValue == 0 && change.newValue > 0)
						{
							this.m_actor.ActivateSpell(SpellType.BURNING);
						}
						else if (change.oldValue > 0 && change.newValue == 0)
						{
							this.m_actor.DeactivateSpell(SpellType.BURNING);
							if (!this.m_entity.IsSilenced())
							{
								this.m_actor.ActivateSpell(SpellType.EXPLODE);
							}
						}
						return;
					default:
						switch (tag)
						{
						case GAME_TAG.EVIL_GLOW:
							if (!this.CanShowActorVisuals())
							{
								return;
							}
							if (change.newValue == 1)
							{
								this.m_actor.ActivateSpell(SpellType.EVIL_GLOW);
							}
							else
							{
								this.m_actor.DeactivateSpell(SpellType.EVIL_GLOW);
							}
							return;
						default:
							switch (tag)
							{
							case GAME_TAG.ELECTRIC_CHARGE_LEVEL:
								if (!this.CanShowActorVisuals())
								{
									return;
								}
								if (change.newValue == 1)
								{
									this.m_actor.DeactivateSpell(SpellType.ELECTRIC_CHARGE_LEVEL_SMALL);
									this.m_actor.ActivateSpell(SpellType.ELECTRIC_CHARGE_LEVEL_MEDIUM);
								}
								else if (change.newValue == 2)
								{
									this.m_actor.DeactivateSpell(SpellType.ELECTRIC_CHARGE_LEVEL_SMALL);
									this.m_actor.DeactivateSpell(SpellType.ELECTRIC_CHARGE_LEVEL_MEDIUM);
									this.m_actor.ActivateSpell(SpellType.ELECTRIC_CHARGE_LEVEL_LARGE);
								}
								else
								{
									this.m_actor.DeactivateSpell(SpellType.ELECTRIC_CHARGE_LEVEL_LARGE);
								}
								return;
							case GAME_TAG.HEAVILY_ARMORED:
								if (!this.CanShowActorVisuals())
								{
									return;
								}
								if (change.newValue == 1)
								{
									this.m_actor.ActivateSpell(SpellType.HEAVILY_ARMORED);
								}
								else
								{
									this.m_actor.DeactivateSpell(SpellType.HEAVILY_ARMORED);
								}
								return;
							case GAME_TAG.DONT_SHOW_IMMUNE:
								if (!this.CanShowActorVisuals())
								{
									return;
								}
								if (change.newValue == 0 && this.m_entity.IsImmune())
								{
									this.m_actor.ActivateSpell(SpellType.IMMUNE);
								}
								else
								{
									this.m_actor.DeactivateSpell(SpellType.IMMUNE);
								}
								return;
							default:
								if (tag != GAME_TAG.TAUNT_READY)
								{
									if (tag == GAME_TAG.STEALTH_READY)
									{
										goto IL_545;
									}
									if (tag != GAME_TAG.TRIGGER_VISUAL)
									{
										if (tag != GAME_TAG.ENRAGED)
										{
											if (tag != GAME_TAG.DEATHRATTLE)
											{
												if (tag == GAME_TAG.CANT_PLAY)
												{
													if (change.newValue == 1)
													{
														this.CancelActiveSpells();
													}
													return;
												}
												if (tag != GAME_TAG.CANT_BE_DAMAGED)
												{
													if (tag != GAME_TAG.FROZEN)
													{
														if (tag != GAME_TAG.NUM_TURNS_IN_PLAY)
														{
															if (tag == GAME_TAG.ARMOR)
															{
																goto IL_779;
															}
															if (tag != GAME_TAG.CANT_BE_TARGETED_BY_HERO_POWERS)
															{
																if (tag != GAME_TAG.HEALTH_MINIMUM)
																{
																	if (tag != GAME_TAG.CUSTOM_KEYWORD_EFFECT)
																	{
																		if (tag != GAME_TAG.SHIFTING)
																		{
																			return;
																		}
																		if (!this.CanShowActorVisuals())
																		{
																			return;
																		}
																		if (change.newValue == 1)
																		{
																			this.m_actor.ActivateSpell(SpellType.SHIFTING);
																		}
																		else
																		{
																			this.m_actor.DeactivateSpell(SpellType.SHIFTING);
																		}
																		return;
																	}
																	else
																	{
																		if (!this.CanShowActorVisuals())
																		{
																			return;
																		}
																		if (change.newValue == 1)
																		{
																			this.ActivateCustomKeywordEffect();
																		}
																		else
																		{
																			this.DeactivateCustomKeywordEffect();
																		}
																		return;
																	}
																}
																else
																{
																	if (!this.CanShowActorVisuals())
																	{
																		return;
																	}
																	if (change.newValue > 0)
																	{
																		this.m_actor.ActivateSpell(SpellType.SHOUT_BUFF);
																	}
																	else
																	{
																		this.m_actor.DeactivateSpell(SpellType.SHOUT_BUFF);
																	}
																	return;
																}
															}
															else
															{
																if (!this.CanShowActorVisuals())
																{
																	return;
																}
																if (this.m_entity.IsStealthed())
																{
																	return;
																}
																if (change.newValue == 1)
																{
																	this.m_actor.ActivateSpell(SpellType.UNTARGETABLE);
																}
																else
																{
																	this.m_actor.DeactivateSpell(SpellType.UNTARGETABLE);
																}
																return;
															}
														}
														else
														{
															if (!this.CanShowActorVisuals())
															{
																return;
															}
															if (this.m_entity.IsAsleep())
															{
																this.m_actor.ActivateSpell(SpellType.Zzz);
															}
															return;
														}
													}
													else
													{
														if (!this.CanShowActorVisuals())
														{
															return;
														}
														if (change.oldValue == 0 && change.newValue > 0)
														{
															SoundManager.Get().LoadAndPlay("FrostBoltHit1");
															this.m_actor.ActivateSpell(SpellType.FROZEN);
														}
														else if (change.oldValue > 0 && change.newValue == 0)
														{
															SoundManager.Get().LoadAndPlay("FrostArmorTarget1");
															this.m_actor.DeactivateSpell(SpellType.FROZEN);
														}
														return;
													}
												}
												else
												{
													if (!this.CanShowActorVisuals())
													{
														return;
													}
													if (change.newValue == 1 && !this.m_entity.DontShowImmune())
													{
														this.m_actor.ActivateSpell(SpellType.IMMUNE);
													}
													else
													{
														this.m_actor.DeactivateSpell(SpellType.IMMUNE);
													}
													return;
												}
											}
											else
											{
												if (!this.CanShowActorVisuals())
												{
													return;
												}
												this.ToggleDeathrattle(change.newValue == 1);
												return;
											}
										}
										else
										{
											if (!this.CanShowActorVisuals())
											{
												return;
											}
											if (change.newValue == 1)
											{
												this.m_actor.ActivateSpell(SpellType.ENRAGE);
											}
											else
											{
												this.m_actor.DeactivateSpell(SpellType.ENRAGE);
											}
											return;
										}
									}
									else
									{
										if (this.m_entity.IsEnchantment())
										{
											Entity entity = GameState.Get().GetEntity(this.m_entity.GetAttached());
											if (entity != null && entity.GetCard() != null && entity.GetCard().CanShowActorVisuals())
											{
												entity.GetCard().ToggleBauble(change.newValue == 1, SpellType.TRIGGER);
											}
										}
										if (!this.CanShowActorVisuals())
										{
											return;
										}
										this.ToggleBauble(change.newValue == 1, SpellType.TRIGGER);
										return;
									}
								}
								break;
							}
							break;
						case GAME_TAG.INSPIRE:
							if (!this.CanShowActorVisuals())
							{
								return;
							}
							this.ToggleBauble(change.newValue == 1, SpellType.INSPIRE);
							return;
						}
						break;
					case GAME_TAG.HEROPOWER_DAMAGE:
						goto IL_60C;
					}
					break;
				case GAME_TAG.AI_MUST_PLAY:
					if (!this.CanShowActorVisuals())
					{
						return;
					}
					if (change.newValue == 1)
					{
						this.m_actor.ActivateSpell(SpellType.AUTO_CAST);
					}
					else
					{
						this.m_actor.DeactivateSpell(SpellType.AUTO_CAST);
					}
					return;
				}
				break;
			case GAME_TAG.CONTROLLER:
				if (!this.CanShowActorVisuals())
				{
					return;
				}
				if (!this.m_entity.HasCharge())
				{
					this.m_actor.ActivateSpell(SpellType.Zzz);
				}
				return;
			}
			break;
		case GAME_TAG.DIVINE_SHIELD:
			if (!this.CanShowActorVisuals())
			{
				return;
			}
			if (this.m_entity.GetController() != null && !this.m_entity.GetController().IsFriendlySide() && this.m_entity.IsObfuscated())
			{
				return;
			}
			if (change.newValue == 1)
			{
				this.m_actor.ActivateSpell(SpellType.DIVINE_SHIELD);
			}
			else
			{
				this.m_actor.DeactivateSpell(SpellType.DIVINE_SHIELD);
			}
			return;
		case GAME_TAG.CHARGE:
		{
			if (!this.CanShowActorVisuals())
			{
				return;
			}
			Spell actorSpell = this.GetActorSpell(SpellType.Zzz, true);
			if (actorSpell != null)
			{
				actorSpell.ActivateState(SpellStateType.DEATH);
			}
			if (change.newValue == 1)
			{
				this.m_actor.ActivateSpell(SpellType.CHARGE);
			}
			return;
		}
		}
		if (!this.CanShowActorVisuals())
		{
			return;
		}
		if (change.newValue == 1)
		{
			this.m_actor.ActivateTaunt();
		}
		else
		{
			this.m_actor.DeactivateTaunt();
		}
		return;
		IL_545:
		if (!this.CanShowActorVisuals())
		{
			return;
		}
		if (change.newValue == 1)
		{
			this.m_actor.ActivateSpell(SpellType.STEALTH);
			this.m_actor.DeactivateSpell(SpellType.UNTARGETABLE);
		}
		else
		{
			this.m_actor.DeactivateSpell(SpellType.STEALTH);
			if (this.m_entity.HasTag(GAME_TAG.CANT_BE_TARGETED_BY_HERO_POWERS))
			{
				this.m_actor.ActivateSpell(SpellType.UNTARGETABLE);
			}
		}
		if (this.m_entity.HasTaunt())
		{
			this.m_actor.ActivateTaunt();
		}
		return;
		IL_60C:
		if (!this.CanShowActorVisuals())
		{
			return;
		}
		if (change.newValue > 0)
		{
			this.m_actor.ActivateSpell(SpellType.SPELL_POWER);
		}
		else
		{
			this.m_actor.DeactivateSpell(SpellType.SPELL_POWER);
		}
		return;
		IL_779:
		if (!this.CanShowActorVisuals())
		{
			return;
		}
		this.UpdateActorComponents();
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x0006F1C4 File Offset: 0x0006D3C4
	public bool CanShowActorVisuals()
	{
		return !this.m_entity.IsLoadingAssets() && !(this.m_actor == null) && this.m_actor.IsShown();
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x0006F20C File Offset: 0x0006D40C
	public void ActivateStateSpells()
	{
		if (this.m_entity.GetController() != null && !this.m_entity.GetController().IsFriendlySide() && this.m_entity.IsObfuscated())
		{
			return;
		}
		if (this.m_entity.HasCharge())
		{
			this.m_actor.ActivateSpell(SpellType.CHARGE);
		}
		if (this.m_entity.HasTaunt())
		{
			this.m_actor.ActivateTaunt();
		}
		if (this.m_entity.IsStealthed())
		{
			this.m_actor.ActivateSpell(SpellType.STEALTH);
		}
		else if (this.m_entity.HasTag(GAME_TAG.CANT_BE_TARGETED_BY_HERO_POWERS))
		{
			this.m_actor.ActivateSpell(SpellType.UNTARGETABLE);
		}
		if (this.m_entity.HasDivineShield())
		{
			this.m_actor.ActivateSpell(SpellType.DIVINE_SHIELD);
		}
		if (this.m_entity.HasSpellPower())
		{
			this.m_actor.ActivateSpell(SpellType.SPELL_POWER);
		}
		if (this.m_entity.HasHeroPowerDamage())
		{
			this.m_actor.ActivateSpell(SpellType.SPELL_POWER);
		}
		if (this.m_entity.IsImmune() && !this.m_entity.DontShowImmune())
		{
			this.m_actor.ActivateSpell(SpellType.IMMUNE);
		}
		if (this.m_entity.HasHealthMin())
		{
			this.m_actor.ActivateSpell(SpellType.SHOUT_BUFF);
		}
		if (this.m_entity.IsAsleep())
		{
			this.m_actor.ActivateSpell(SpellType.Zzz);
		}
		if (this.m_entity.IsEnraged())
		{
			this.m_actor.ActivateSpell(SpellType.ENRAGE);
		}
		if (this.m_entity.HasWindfury())
		{
			Spell spell = this.m_actor.GetSpell(SpellType.WINDFURY_IDLE);
			if (spell != null)
			{
				spell.ActivateState(SpellStateType.BIRTH);
			}
		}
		if (this.m_entity.HasDeathrattle())
		{
			this.m_actor.ActivateSpell(SpellType.DEATHRATTLE_IDLE);
		}
		if (this.m_entity.IsSilenced())
		{
			this.m_actor.ActivateSpell(SpellType.SILENCE);
		}
		SpellType spellType = this.PrioritizedBauble();
		if (spellType != SpellType.NONE)
		{
			this.m_actor.ActivateSpell(spellType);
		}
		if (this.m_entity.HasAura())
		{
			this.m_actor.ActivateSpell(SpellType.AURA);
		}
		if (this.m_entity.HasTag(GAME_TAG.AI_MUST_PLAY))
		{
			this.m_actor.ActivateSpell(SpellType.AUTO_CAST);
		}
		if (this.m_entity.IsFrozen())
		{
			this.m_actor.ActivateSpell(SpellType.FROZEN);
		}
		if (this.m_entity.HasTag(GAME_TAG.HEAVILY_ARMORED))
		{
			this.m_actor.ActivateSpell(SpellType.HEAVILY_ARMORED);
		}
		Player controller = this.m_entity.GetController();
		if (controller != null)
		{
			if (this.m_entity.IsHeroPower())
			{
				if (controller.HasTag(GAME_TAG.STEADY_SHOT_CAN_TARGET))
				{
					this.m_actor.ActivateSpell(SpellType.STEADY_SHOT_CAN_TARGET);
				}
				if (controller.HasTag(GAME_TAG.CURRENT_HEROPOWER_DAMAGE_BONUS) && controller.IsHeroPowerAffectedByBonusDamage())
				{
					this.m_actor.ActivateSpell(SpellType.CURRENT_HEROPOWER_DAMAGE_BONUS);
				}
				if (this.m_entity.HasTag(GAME_TAG.ELECTRIC_CHARGE_LEVEL))
				{
					int tag = this.m_entity.GetTag(GAME_TAG.ELECTRIC_CHARGE_LEVEL);
					if (tag == 1)
					{
						this.m_actor.ActivateSpell(SpellType.ELECTRIC_CHARGE_LEVEL_MEDIUM);
					}
					else if (tag == 2)
					{
						this.m_actor.ActivateSpell(SpellType.ELECTRIC_CHARGE_LEVEL_LARGE);
					}
				}
			}
			if (this.m_entity.IsHero())
			{
				if (controller.HasTag(GAME_TAG.LOCK_AND_LOAD))
				{
					this.m_actor.ActivateSpell(SpellType.LOCK_AND_LOAD);
				}
				if (controller.HasTag(GAME_TAG.SHADOWFORM))
				{
					this.m_actor.ActivateSpell(SpellType.SHADOWFORM);
				}
				if (controller.HasTag(GAME_TAG.EMBRACE_THE_SHADOW))
				{
					this.m_actor.ActivateSpell(SpellType.EMBRACE_THE_SHADOW);
				}
			}
		}
		TAG_ZONE zone = this.m_entity.GetZone();
		if (zone == TAG_ZONE.HAND)
		{
			this.ActivateHandStateSpells();
		}
		else if (zone == TAG_ZONE.PLAY || zone == TAG_ZONE.SECRET)
		{
			if (this.m_entity.HasCustomKeywordEffect())
			{
				this.ActivateCustomKeywordEffect();
			}
			this.ShowExhaustedChange(this.m_entity.IsExhausted());
		}
	}

	// Token: 0x060017E7 RID: 6119 RVA: 0x0006F63C File Offset: 0x0006D83C
	public void ActivateHandStateSpells()
	{
		Player controller = this.m_entity.GetController();
		if ((this.m_entity.IsHeroPower() || this.m_entity.IsSpell()) && this.m_playEffect != null)
		{
			Spell spell = this.m_playEffect.GetSpell(false);
			SpellUtils.ActivateCancelIfNecessary(spell);
		}
		if (this.m_entity.IsSpell())
		{
			Spell actorSpell = this.GetActorSpell(SpellType.POWER_UP, false);
			SpellUtils.ActivateCancelIfNecessary(actorSpell);
		}
		if (this.m_entity.HasTag(GAME_TAG.EVIL_GLOW))
		{
			Spell actorSpell2 = this.GetActorSpell(SpellType.EVIL_GLOW, true);
			SpellUtils.ActivateBirthIfNecessary(actorSpell2);
		}
		if (this.m_entity.HasTag(GAME_TAG.SHIFTING))
		{
			Spell actorSpell3 = this.GetActorSpell(SpellType.SHIFTING, true);
			SpellUtils.ActivateBirthIfNecessary(actorSpell3);
		}
		if (controller != null && controller.HasTag(GAME_TAG.SPELLS_COST_HEALTH) && this.m_entity.IsSpell())
		{
			Spell actorSpell4 = this.GetActorSpell(SpellType.SPELLS_COST_HEALTH, true);
			SpellUtils.ActivateBirthIfNecessary(actorSpell4);
		}
		if (controller != null && controller.HasTag(GAME_TAG.CHOOSE_BOTH) && this.m_entity.HasTag(GAME_TAG.CHOOSE_ONE) && GameState.Get().IsOption(this.m_entity))
		{
			Spell actorSpell5 = this.GetActorSpell(SpellType.CHOOSE_BOTH, true);
			SpellUtils.ActivateBirthIfNecessary(actorSpell5);
		}
	}

	// Token: 0x060017E8 RID: 6120 RVA: 0x0006F790 File Offset: 0x0006D990
	public void DeactivateHandStateSpells()
	{
		Player controller = this.m_entity.GetController();
		if (this.m_entity.HasTag(GAME_TAG.EVIL_GLOW))
		{
			Spell actorSpell = this.GetActorSpell(SpellType.EVIL_GLOW, false);
			SpellUtils.ActivateDeathIfNecessary(actorSpell);
		}
		if (this.m_entity.HasTag(GAME_TAG.SHIFTING))
		{
			Spell actorSpell2 = this.GetActorSpell(SpellType.SHIFTING, false);
			SpellUtils.ActivateDeathIfNecessary(actorSpell2);
		}
		if (controller != null && controller.HasTag(GAME_TAG.SPELLS_COST_HEALTH) && this.m_entity.IsSpell())
		{
			Spell actorSpell3 = this.GetActorSpell(SpellType.SPELLS_COST_HEALTH, false);
			SpellUtils.ActivateDeathIfNecessary(actorSpell3);
		}
		if (controller != null && controller.HasTag(GAME_TAG.CHOOSE_BOTH) && this.m_entity.HasTag(GAME_TAG.CHOOSE_ONE) && GameState.Get().IsOption(this.m_entity))
		{
			Spell actorSpell4 = this.GetActorSpell(SpellType.CHOOSE_BOTH, false);
			SpellUtils.ActivateDeathIfNecessary(actorSpell4);
		}
	}

	// Token: 0x060017E9 RID: 6121 RVA: 0x0006F880 File Offset: 0x0006DA80
	private void ToggleDeathrattle(bool on)
	{
		if (on)
		{
			this.m_actor.ActivateSpell(SpellType.DEATHRATTLE_IDLE);
		}
		else
		{
			this.m_actor.DeactivateSpell(SpellType.DEATHRATTLE_IDLE);
		}
	}

	// Token: 0x060017EA RID: 6122 RVA: 0x0006F8B4 File Offset: 0x0006DAB4
	private void ToggleBauble(bool on, SpellType spellType)
	{
		if (on)
		{
			this.DeactivateBaubles();
			this.m_actor.ActivateSpell(spellType);
		}
		else
		{
			SpellType spellType2 = this.PrioritizedBauble();
			if (spellType != spellType2)
			{
				this.m_actor.DeactivateSpell(spellType);
				if (spellType2 != SpellType.NONE)
				{
					this.m_actor.ActivateSpell(spellType2);
				}
			}
		}
	}

	// Token: 0x060017EB RID: 6123 RVA: 0x0006F90C File Offset: 0x0006DB0C
	public void DeactivateBaubles()
	{
		Spell actorSpell = this.GetActorSpell(SpellType.TRIGGER, false);
		SpellUtils.ActivateDeathIfNecessary(actorSpell);
		actorSpell = this.GetActorSpell(SpellType.POISONOUS, false);
		SpellUtils.ActivateDeathIfNecessary(actorSpell);
		actorSpell = this.GetActorSpell(SpellType.INSPIRE, false);
		SpellUtils.ActivateDeathIfNecessary(actorSpell);
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x0006F94C File Offset: 0x0006DB4C
	private SpellType PrioritizedBauble()
	{
		if (this.m_entity.HasTriggerVisual() || this.m_entity.DoEnchantmentsHaveTriggerVisuals())
		{
			return SpellType.TRIGGER;
		}
		if (this.m_entity.IsPoisonous())
		{
			return SpellType.POISONOUS;
		}
		if (this.m_entity.HasInspire())
		{
			return SpellType.INSPIRE;
		}
		return SpellType.NONE;
	}

	// Token: 0x060017ED RID: 6125 RVA: 0x0006F9A4 File Offset: 0x0006DBA4
	public void ShowExhaustedChange(int val)
	{
		bool exhausted = val == 1;
		this.ShowExhaustedChange(exhausted);
	}

	// Token: 0x060017EE RID: 6126 RVA: 0x0006F9C0 File Offset: 0x0006DBC0
	public void ShowExhaustedChange(bool exhausted)
	{
		if (this.m_entity.IsHeroPower())
		{
			base.StopCoroutine("PlayHeroPowerAnimation");
			if (exhausted)
			{
				base.StartCoroutine("PlayHeroPowerAnimation", (!UniversalInputManager.UsePhoneUI) ? "HeroPower_Used" : "HeroPower_Used_phone");
				SoundManager.Get().LoadAndPlay("hero_power_icon_flip_off");
			}
			else
			{
				base.StartCoroutine("PlayHeroPowerAnimation", (!UniversalInputManager.UsePhoneUI) ? "HeroPower_Restore" : "HeroPower_Restore_phone");
				SoundManager.Get().LoadAndPlay("hero_power_icon_flip_on");
			}
		}
		else if (this.m_entity.IsWeapon())
		{
			if (exhausted)
			{
				this.SheatheWeapon();
			}
			else
			{
				this.UnSheatheWeapon();
			}
		}
		else if (this.m_entity.IsSecret())
		{
			base.StartCoroutine(this.ShowSecretExhaustedChange(exhausted));
		}
	}

	// Token: 0x060017EF RID: 6127 RVA: 0x0006FAB4 File Offset: 0x0006DCB4
	private IEnumerator PlayHeroPowerAnimation(string animation)
	{
		this.SetInputEnabled(false);
		MinionShake shake = this.m_actor.gameObject.GetComponentInChildren<MinionShake>();
		if (shake == null)
		{
			yield break;
		}
		while (shake.isShaking())
		{
			yield return null;
		}
		this.m_actor.GetComponent<Animation>().Play(animation);
		Spell spell = this.GetPlaySpell(true);
		if (spell != null)
		{
			while (spell.GetActiveState() != SpellStateType.NONE)
			{
				yield return null;
			}
		}
		this.SetInputEnabled(true);
		if (animation.Contains("Used") && GameState.Get().IsOption(this.m_entity))
		{
			this.SetInputEnabled(false);
		}
		yield break;
	}

	// Token: 0x060017F0 RID: 6128 RVA: 0x0006FADD File Offset: 0x0006DCDD
	private void SheatheWeapon()
	{
		this.m_actor.GetAttackObject().ScaleToZero();
		this.ActivateActorSpell(SpellType.SHEATHE);
	}

	// Token: 0x060017F1 RID: 6129 RVA: 0x0006FAF8 File Offset: 0x0006DCF8
	private void UnSheatheWeapon()
	{
		this.m_actor.GetAttackObject().Enlarge(1f);
		this.ActivateActorSpell(SpellType.UNSHEATHE);
	}

	// Token: 0x060017F2 RID: 6130 RVA: 0x0006FB18 File Offset: 0x0006DD18
	private IEnumerator ShowSecretExhaustedChange(bool exhausted)
	{
		while (!this.m_actorReady)
		{
			yield return null;
		}
		Spell spell = this.m_actor.GetComponent<Spell>();
		while (spell.GetActiveState() != SpellStateType.NONE)
		{
			yield return null;
		}
		if (!this.CanShowSecret())
		{
			yield break;
		}
		if (exhausted)
		{
			this.SheatheSecret(spell);
		}
		else
		{
			this.UnSheatheSecret(spell);
		}
		yield break;
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x0006FB41 File Offset: 0x0006DD41
	private void SheatheSecret(Spell spell)
	{
		if (this.m_secretSheathed)
		{
			return;
		}
		if (!this.m_entity.IsExhausted())
		{
			return;
		}
		this.m_secretSheathed = true;
		spell.ActivateState(SpellStateType.IDLE);
	}

	// Token: 0x060017F4 RID: 6132 RVA: 0x0006FB6E File Offset: 0x0006DD6E
	private void UnSheatheSecret(Spell spell)
	{
		if (!this.m_secretSheathed)
		{
			return;
		}
		if (this.m_entity.IsExhausted())
		{
			return;
		}
		this.m_secretSheathed = false;
		spell.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x060017F5 RID: 6133 RVA: 0x0006FB9C File Offset: 0x0006DD9C
	public void OnEnchantmentAdded(int oldEnchantmentCount, Entity enchantment)
	{
		Spell spell = null;
		TAG_ENCHANTMENT_VISUAL enchantmentBirthVisual = enchantment.GetEnchantmentBirthVisual();
		if (enchantmentBirthVisual == TAG_ENCHANTMENT_VISUAL.POSITIVE)
		{
			spell = this.GetActorSpell(SpellType.ENCHANT_POSITIVE, true);
		}
		else if (enchantmentBirthVisual == TAG_ENCHANTMENT_VISUAL.NEGATIVE)
		{
			spell = this.GetActorSpell(SpellType.ENCHANT_NEGATIVE, true);
		}
		else if (enchantmentBirthVisual == TAG_ENCHANTMENT_VISUAL.NEUTRAL)
		{
			spell = this.GetActorSpell(SpellType.ENCHANT_NEUTRAL, true);
		}
		if (spell == null)
		{
			this.UpdateEnchantments();
			this.UpdateTooltip();
			return;
		}
		if (enchantment.HasTriggerVisual())
		{
			this.ToggleBauble(true, SpellType.TRIGGER);
		}
		spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnEnchantmentSpellStateFinished));
		spell.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x060017F6 RID: 6134 RVA: 0x0006FC34 File Offset: 0x0006DE34
	public void OnEnchantmentRemoved(int oldEnchantmentCount, Entity enchantment)
	{
		Spell spell = null;
		TAG_ENCHANTMENT_VISUAL enchantmentBirthVisual = enchantment.GetEnchantmentBirthVisual();
		if (enchantmentBirthVisual == TAG_ENCHANTMENT_VISUAL.POSITIVE)
		{
			spell = this.GetActorSpell(SpellType.ENCHANT_POSITIVE, true);
		}
		else if (enchantmentBirthVisual == TAG_ENCHANTMENT_VISUAL.NEGATIVE)
		{
			spell = this.GetActorSpell(SpellType.ENCHANT_NEGATIVE, true);
		}
		else if (enchantmentBirthVisual == TAG_ENCHANTMENT_VISUAL.NEUTRAL)
		{
			spell = this.GetActorSpell(SpellType.ENCHANT_NEUTRAL, true);
		}
		if (spell == null)
		{
			this.UpdateEnchantments();
			this.UpdateTooltip();
			return;
		}
		if (enchantment.HasTriggerVisual() && !this.m_entity.DoEnchantmentsHaveTriggerVisuals())
		{
			this.ToggleBauble(false, SpellType.TRIGGER);
		}
		spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnEnchantmentSpellStateFinished));
		spell.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x0006FCDC File Offset: 0x0006DEDC
	private void OnEnchantmentSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (prevStateType != SpellStateType.BIRTH && prevStateType != SpellStateType.DEATH)
		{
			return;
		}
		spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnEnchantmentSpellStateFinished));
		this.UpdateEnchantments();
		this.UpdateTooltip();
	}

	// Token: 0x060017F8 RID: 6136 RVA: 0x0006FD18 File Offset: 0x0006DF18
	public void UpdateEnchantments()
	{
		List<Entity> enchantments = this.m_entity.GetEnchantments();
		Spell actorSpell = this.GetActorSpell(SpellType.ENCHANT_POSITIVE, true);
		Spell actorSpell2 = this.GetActorSpell(SpellType.ENCHANT_NEGATIVE, true);
		Spell actorSpell3 = this.GetActorSpell(SpellType.ENCHANT_NEUTRAL, true);
		Spell spell = null;
		if (actorSpell != null && actorSpell.GetActiveState() == SpellStateType.IDLE)
		{
			spell = actorSpell;
		}
		else if (actorSpell2 != null && actorSpell2.GetActiveState() == SpellStateType.IDLE)
		{
			spell = actorSpell2;
		}
		else if (actorSpell3 != null && actorSpell3.GetActiveState() == SpellStateType.IDLE)
		{
			spell = actorSpell3;
		}
		if (enchantments.Count == 0)
		{
			if (spell != null)
			{
				spell.ActivateState(SpellStateType.DEATH);
			}
			return;
		}
		int num = 0;
		bool flag = false;
		foreach (Entity entity in enchantments)
		{
			TAG_ENCHANTMENT_VISUAL enchantmentIdleVisual = entity.GetEnchantmentIdleVisual();
			if (enchantmentIdleVisual == TAG_ENCHANTMENT_VISUAL.POSITIVE)
			{
				num++;
			}
			else if (enchantmentIdleVisual == TAG_ENCHANTMENT_VISUAL.NEGATIVE)
			{
				num--;
			}
			if (enchantmentIdleVisual != TAG_ENCHANTMENT_VISUAL.INVALID)
			{
				flag = true;
			}
		}
		Spell spell2 = null;
		if (num > 0)
		{
			spell2 = actorSpell;
		}
		else if (num < 0)
		{
			spell2 = actorSpell2;
		}
		else if (flag)
		{
			spell2 = actorSpell3;
		}
		if (spell != null && spell != spell2)
		{
			spell.Deactivate();
		}
		if (spell2 != null)
		{
			spell2.ActivateState(SpellStateType.IDLE);
		}
	}

	// Token: 0x060017F9 RID: 6137 RVA: 0x0006FEB0 File Offset: 0x0006E0B0
	public Spell GetActorSpell(SpellType spellType, bool loadIfNeeded = true)
	{
		if (this.m_actor == null)
		{
			return null;
		}
		Spell result;
		if (loadIfNeeded)
		{
			result = this.m_actor.GetSpell(spellType);
		}
		else
		{
			result = this.m_actor.GetSpellIfLoaded(spellType);
		}
		return result;
	}

	// Token: 0x060017FA RID: 6138 RVA: 0x0006FEF6 File Offset: 0x0006E0F6
	public Spell ActivateActorSpell(SpellType spellType)
	{
		return this.ActivateActorSpell(this.m_actor, spellType, null, null);
	}

	// Token: 0x060017FB RID: 6139 RVA: 0x0006FF07 File Offset: 0x0006E107
	public Spell ActivateActorSpell(SpellType spellType, Spell.FinishedCallback finishedCallback)
	{
		return this.ActivateActorSpell(this.m_actor, spellType, finishedCallback, null);
	}

	// Token: 0x060017FC RID: 6140 RVA: 0x0006FF18 File Offset: 0x0006E118
	public Spell ActivateActorSpell(SpellType spellType, Spell.FinishedCallback finishedCallback, Spell.StateFinishedCallback stateFinishedCallback)
	{
		return this.ActivateActorSpell(this.m_actor, spellType, finishedCallback, stateFinishedCallback);
	}

	// Token: 0x060017FD RID: 6141 RVA: 0x0006FF29 File Offset: 0x0006E129
	private Spell ActivateActorSpell(Actor actor, SpellType spellType)
	{
		return this.ActivateActorSpell(actor, spellType, null, null);
	}

	// Token: 0x060017FE RID: 6142 RVA: 0x0006FF35 File Offset: 0x0006E135
	private Spell ActivateActorSpell(Actor actor, SpellType spellType, Spell.FinishedCallback finishedCallback)
	{
		return this.ActivateActorSpell(actor, spellType, finishedCallback, null);
	}

	// Token: 0x060017FF RID: 6143 RVA: 0x0006FF44 File Offset: 0x0006E144
	private Spell ActivateActorSpell(Actor actor, SpellType spellType, Spell.FinishedCallback finishedCallback, Spell.StateFinishedCallback stateFinishedCallback)
	{
		if (actor == null)
		{
			Log.Mike.Print(string.Format("{0}.ActivateActorSpell() - actor IS NULL spellType={1}", this, spellType), new object[0]);
			return null;
		}
		Spell spell = actor.GetSpell(spellType);
		if (spell == null)
		{
			Log.Mike.Print(string.Format("{0}.ActivateActorSpell() - spell IS NULL actor={1} spellType={2}", this, actor, spellType), new object[0]);
			return null;
		}
		this.ActivateSpell(spell, finishedCallback, stateFinishedCallback);
		return spell;
	}

	// Token: 0x06001800 RID: 6144 RVA: 0x0006FFC3 File Offset: 0x0006E1C3
	private void ActivateSpell(Spell spell, Spell.FinishedCallback finishedCallback)
	{
		this.ActivateSpell(spell, finishedCallback, null, null, null);
	}

	// Token: 0x06001801 RID: 6145 RVA: 0x0006FFD0 File Offset: 0x0006E1D0
	private void ActivateSpell(Spell spell, Spell.FinishedCallback finishedCallback, Spell.StateFinishedCallback stateFinishedCallback)
	{
		this.ActivateSpell(spell, finishedCallback, null, stateFinishedCallback, null);
	}

	// Token: 0x06001802 RID: 6146 RVA: 0x0006FFDD File Offset: 0x0006E1DD
	private void ActivateSpell(Spell spell, Spell.FinishedCallback finishedCallback, object finishedUserData, Spell.StateFinishedCallback stateFinishedCallback)
	{
		this.ActivateSpell(spell, finishedCallback, finishedUserData, stateFinishedCallback, null);
	}

	// Token: 0x06001803 RID: 6147 RVA: 0x0006FFEB File Offset: 0x0006E1EB
	private void ActivateSpell(Spell spell, Spell.FinishedCallback finishedCallback, object finishedUserData, Spell.StateFinishedCallback stateFinishedCallback, object stateFinishedUserData)
	{
		if (finishedCallback != null)
		{
			spell.AddFinishedCallback(finishedCallback, finishedUserData);
		}
		if (stateFinishedCallback != null)
		{
			spell.AddStateFinishedCallback(stateFinishedCallback, stateFinishedUserData);
		}
		if (spell.GetActiveState() == SpellStateType.NONE)
		{
			spell.Activate();
		}
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x00070020 File Offset: 0x0006E220
	public Spell GetActorAttackSpellForInput()
	{
		if (this.m_actor == null)
		{
			Log.Mike.Print("{0}.GetActorAttackSpellForInput() - m_actor IS NULL", new object[]
			{
				this
			});
			return null;
		}
		if (this.m_zone == null)
		{
			Log.Mike.Print("{0}.GetActorAttackSpellForInput() - m_zone IS NULL", new object[]
			{
				this
			});
			return null;
		}
		Spell spell = this.m_actor.GetSpell(SpellType.FRIENDLY_ATTACK);
		if (spell == null)
		{
			Log.Mike.Print("{0}.GetActorAttackSpellForInput() - {1} spell is null", new object[]
			{
				this,
				SpellType.FRIENDLY_ATTACK
			});
			return null;
		}
		return spell;
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x000700C8 File Offset: 0x0006E2C8
	private Spell ActivateDeathSpell(Actor actor)
	{
		bool flag;
		Spell bestDeathSpell = this.GetBestDeathSpell(actor, out flag);
		if (bestDeathSpell == null)
		{
			Debug.LogError(string.Format("{0}.ActivateDeathSpell() - {1} is null", this, SpellType.DEATH));
			return null;
		}
		this.CleanUpCustomSpell(bestDeathSpell, ref this.m_customDeathSpell);
		this.CleanUpCustomSpell(bestDeathSpell, ref this.m_customDeathSpellOverride);
		this.m_activeDeathEffectCount++;
		if (flag)
		{
			if (this.m_actor != actor)
			{
				bestDeathSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
			}
		}
		else
		{
			bestDeathSpell.SetSource(base.gameObject);
			if (this.m_actor != actor)
			{
				bestDeathSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_CustomDeath));
			}
			SpellUtils.SetCustomSpellParent(bestDeathSpell, actor);
		}
		bestDeathSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_Death));
		bestDeathSpell.Activate();
		BoardEvents boardEvents = BoardEvents.Get();
		if (boardEvents != null)
		{
			boardEvents.DeathEvent(this);
		}
		return bestDeathSpell;
	}

	// Token: 0x06001806 RID: 6150 RVA: 0x000701C4 File Offset: 0x0006E3C4
	private Spell ActivateHandSpawnSpell()
	{
		if (this.m_customSpawnSpellOverride == null)
		{
			return this.ActivateDefaultSpawnSpell(new Spell.FinishedCallback(this.OnSpellFinished_DefaultHandSpawn));
		}
		Entity creator = this.m_entity.GetCreator();
		Card card = null;
		if (creator != null && creator.IsMinion())
		{
			card = creator.GetCard();
		}
		if (card != null)
		{
			TransformUtil.CopyWorld(base.transform, card.transform);
		}
		this.ActivateCustomHandSpawnSpell(this.m_customSpawnSpellOverride, card);
		return this.m_customSpawnSpellOverride;
	}

	// Token: 0x06001807 RID: 6151 RVA: 0x0007024C File Offset: 0x0006E44C
	private Spell ActivateDefaultSpawnSpell(Spell.FinishedCallback finishedCallback)
	{
		this.m_inputEnabled = false;
		this.m_actor.ToggleForceIdle(true);
		SpellType spellType;
		if (this.m_entity.IsWeapon() && this.m_zone is ZoneWeapon)
		{
			spellType = ((!this.m_entity.IsControlledByFriendlySidePlayer()) ? SpellType.SUMMON_IN_OPPONENT : SpellType.SUMMON_IN_FRIENDLY);
		}
		else
		{
			spellType = SpellType.SUMMON_IN;
		}
		Spell spell = this.ActivateActorSpell(spellType, finishedCallback);
		if (spell == null)
		{
			Debug.LogError(string.Format("{0}.ActivateDefaultSpawnSpell() - {1} is null", this, spellType));
			return null;
		}
		return spell;
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x000702DC File Offset: 0x0006E4DC
	private void ActivateCustomHandSpawnSpell(Spell spell, Card creatorCard)
	{
		GameObject source = (!(creatorCard == null)) ? creatorCard.gameObject : base.gameObject;
		spell.SetSource(source);
		spell.RemoveAllTargets();
		spell.AddTarget(base.gameObject);
		spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroySpell));
		SpellUtils.SetCustomSpellParent(spell, this.m_actor);
		spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_CustomHandSpawn));
		spell.Activate();
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x00070358 File Offset: 0x0006E558
	private void ActivateMinionSpawnEffects()
	{
		Entity creator = this.m_entity.GetCreator();
		Card card = null;
		if (creator != null && creator.IsMinion())
		{
			card = creator.GetCard();
		}
		if (card != null)
		{
			TransformUtil.CopyWorld(base.transform, card.transform);
		}
		bool flag;
		Spell bestSpawnSpell = this.GetBestSpawnSpell(out flag);
		if (flag)
		{
			if (card == null)
			{
				this.ActivateStandardSpawnMinionSpell();
			}
			else
			{
				base.StartCoroutine(this.ActivateCreatorSpawnMinionSpell(creator, card));
			}
		}
		else
		{
			this.ActivateCustomSpawnMinionSpell(bestSpawnSpell, card);
		}
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x000703EC File Offset: 0x0006E5EC
	private IEnumerator ActivateCreatorSpawnMinionSpell(Entity creator, Card creatorCard)
	{
		while (creator.IsLoadingAssets() || !creatorCard.IsActorReady())
		{
			yield return 0;
		}
		Spell creatorSpell = creatorCard.ActivateCreatorSpawnMinionSpell();
		if (creatorSpell != null)
		{
			yield return new WaitForSeconds(0.9f);
		}
		this.ActivateStandardSpawnMinionSpell();
		yield break;
	}

	// Token: 0x0600180B RID: 6155 RVA: 0x00070423 File Offset: 0x0006E623
	private Spell ActivateCreatorSpawnMinionSpell()
	{
		if (this.m_entity.IsControlledByFriendlySidePlayer())
		{
			return this.ActivateActorSpell(SpellType.FRIENDLY_SPAWN_MINION);
		}
		return this.ActivateActorSpell(SpellType.OPPONENT_SPAWN_MINION);
	}

	// Token: 0x0600180C RID: 6156 RVA: 0x00070448 File Offset: 0x0006E648
	private void ActivateStandardSpawnMinionSpell()
	{
		if (this.m_entity.IsControlledByFriendlySidePlayer())
		{
			this.ActivateActorSpell(SpellType.FRIENDLY_SPAWN_MINION, new Spell.FinishedCallback(this.OnSpellFinished_StandardSpawnMinion));
		}
		else
		{
			this.ActivateActorSpell(SpellType.OPPONENT_SPAWN_MINION, new Spell.FinishedCallback(this.OnSpellFinished_StandardSpawnMinion));
		}
		this.ActivateCharacterPlayEffects();
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x0007049C File Offset: 0x0006E69C
	private void ActivateCustomSpawnMinionSpell(Spell spell, Card creatorCard)
	{
		GameObject source = (!(creatorCard == null)) ? creatorCard.gameObject : base.gameObject;
		spell.SetSource(source);
		spell.RemoveAllTargets();
		spell.AddTarget(base.gameObject);
		spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroySpell));
		SpellUtils.SetCustomSpellParent(spell, this.m_actor);
		spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_CustomSpawnMinion));
		spell.Activate();
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x00070518 File Offset: 0x0006E718
	private IEnumerator ActivateReviveSpell()
	{
		while (this.m_activeDeathEffectCount > 0)
		{
			yield return 0;
		}
		this.ActivateStandardSpawnMinionSpell();
		yield break;
	}

	// Token: 0x0600180F RID: 6159 RVA: 0x00070534 File Offset: 0x0006E734
	private IEnumerator ActivateActorBattlecrySpell()
	{
		Spell battlecrySpell = this.GetActorSpell(SpellType.BATTLECRY, true);
		if (battlecrySpell == null)
		{
			yield break;
		}
		if (!(this.m_zone is ZonePlay))
		{
			yield break;
		}
		if (InputManager.Get() == null)
		{
			yield break;
		}
		if (InputManager.Get().GetBattlecrySourceCard() != this)
		{
			yield break;
		}
		yield return new WaitForSeconds(0.01f);
		if (InputManager.Get() == null)
		{
			yield break;
		}
		if (InputManager.Get().GetBattlecrySourceCard() != this)
		{
			yield break;
		}
		if (battlecrySpell.GetActiveState() == SpellStateType.NONE)
		{
			battlecrySpell.ActivateState(SpellStateType.BIRTH);
		}
		Spell playSpell = this.GetPlaySpell(true);
		if (playSpell)
		{
			playSpell.ActivateState(SpellStateType.BIRTH);
		}
		yield break;
	}

	// Token: 0x06001810 RID: 6160 RVA: 0x00070550 File Offset: 0x0006E750
	private void CleanUpCustomSpell(Spell chosenSpell, ref Spell customSpell)
	{
		if (!customSpell)
		{
			return;
		}
		if (chosenSpell == customSpell)
		{
			customSpell = null;
		}
		else
		{
			Object.Destroy(customSpell.gameObject);
		}
	}

	// Token: 0x06001811 RID: 6161 RVA: 0x0007058C File Offset: 0x0006E78C
	private void OnSpellFinished_StandardSpawnMinion(Spell spell, object userData)
	{
		this.m_actorReady = true;
		this.m_inputEnabled = true;
		this.m_actor.Show();
		this.ActivateStateSpells();
		this.RefreshActor();
		this.UpdateActorComponents();
		BoardEvents boardEvents = BoardEvents.Get();
		if (boardEvents != null)
		{
			boardEvents.SummonedEvent(this);
		}
	}

	// Token: 0x06001812 RID: 6162 RVA: 0x000705DD File Offset: 0x0006E7DD
	private void OnSpellFinished_CustomSpawnMinion(Spell spell, object userData)
	{
		this.OnSpellFinished_StandardSpawnMinion(spell, userData);
		this.CleanUpCustomSpell(spell, ref this.m_customSpawnSpell);
		this.CleanUpCustomSpell(spell, ref this.m_customSpawnSpellOverride);
		this.ActivateCharacterPlayEffects();
	}

	// Token: 0x06001813 RID: 6163 RVA: 0x00070607 File Offset: 0x0006E807
	private void OnSpellFinished_DefaultHandSpawn(Spell spell, object userData)
	{
		this.m_actor.ToggleForceIdle(false);
		this.m_inputEnabled = true;
		this.ActivateStateSpells();
		this.RefreshActor();
		this.UpdateActorComponents();
	}

	// Token: 0x06001814 RID: 6164 RVA: 0x0007062E File Offset: 0x0006E82E
	private void OnSpellFinished_CustomHandSpawn(Spell spell, object userData)
	{
		this.OnSpellFinished_DefaultHandSpawn(spell, userData);
		this.CleanUpCustomSpell(spell, ref this.m_customSpawnSpellOverride);
	}

	// Token: 0x06001815 RID: 6165 RVA: 0x00070645 File Offset: 0x0006E845
	private void OnSpellFinished_DefaultPlaySpawn(Spell spell, object userData)
	{
		this.m_actor.ToggleForceIdle(false);
		this.m_inputEnabled = true;
		this.ActivateStateSpells();
		this.RefreshActor();
		this.UpdateActorComponents();
	}

	// Token: 0x06001816 RID: 6166 RVA: 0x0007066C File Offset: 0x0006E86C
	private void OnSpellFinished_StandardCardSummon(Spell spell, object userData)
	{
		this.m_actorReady = true;
		this.m_inputEnabled = true;
		this.ActivateStateSpells();
		this.RefreshActor();
		this.UpdateActorComponents();
	}

	// Token: 0x06001817 RID: 6167 RVA: 0x00070699 File Offset: 0x0006E899
	private void OnSpellFinished_UpdateActorComponents(Spell spell, object userData)
	{
		this.UpdateActorComponents();
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x000706A4 File Offset: 0x0006E8A4
	private void OnSpellFinished_Death(Spell spell, object userData)
	{
		this.m_suppressKeywordDeaths = false;
		this.m_keywordDeathDelaySec = 0.6f;
		this.m_activeDeathEffectCount--;
		GameState.Get().ClearCardBeingDrawn(this);
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x000706E0 File Offset: 0x0006E8E0
	private void OnSpellStateFinished_DestroyActor(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		if (this.m_zone is ZoneGraveyard)
		{
			this.PurgeSpells();
		}
		Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(spell.gameObject);
		if (actor == null)
		{
			Debug.LogWarning(string.Format("Card.OnSpellStateFinished_DestroyActor() - spell {0} on Card {1} has no Actor ancestor", spell, this));
			return;
		}
		actor.Destroy();
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x0007073F File Offset: 0x0006E93F
	private void OnSpellStateFinished_DestroySpell(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		Object.Destroy(spell.gameObject);
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x00070758 File Offset: 0x0006E958
	private void OnSpellStateFinished_CustomDeath(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(spell.gameObject);
		if (actor == null)
		{
			Debug.LogWarning(string.Format("Card.OnSpellStateFinished_CustomDeath() - spell {0} on Card {1} has no Actor ancestor", spell, this));
			return;
		}
		actor.Destroy();
	}

	// Token: 0x0600181C RID: 6172 RVA: 0x000707A4 File Offset: 0x0006E9A4
	public void UpdateActorState()
	{
		if (this.m_actor == null)
		{
			return;
		}
		if (!this.m_shown)
		{
			return;
		}
		if (this.m_entity.IsBusy())
		{
			return;
		}
		if (this.m_zone is ZoneGraveyard)
		{
			return;
		}
		if (!this.m_inputEnabled || (this.m_zone != null && !this.m_zone.IsInputEnabled()))
		{
			this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
			return;
		}
		if (this.m_overPlayfield)
		{
			this.m_actor.SetActorState(ActorStateType.CARD_OVER_PLAYFIELD);
			return;
		}
		GameState gameState = GameState.Get();
		if (gameState != null && gameState.IsEntityInputEnabled(this.m_entity))
		{
			switch (gameState.GetResponseMode())
			{
			case GameState.ResponseMode.OPTION:
				if (this.DoOptionHighlight(gameState))
				{
					return;
				}
				break;
			case GameState.ResponseMode.SUB_OPTION:
				if (this.DoSubOptionHighlight(gameState))
				{
					return;
				}
				break;
			case GameState.ResponseMode.OPTION_TARGET:
				if (this.DoOptionTargetHighlight(gameState))
				{
					return;
				}
				break;
			case GameState.ResponseMode.CHOICE:
				if (this.DoChoiceHighlight(gameState))
				{
					return;
				}
				break;
			}
		}
		if (this.m_mousedOver && !(this.m_zone is ZoneHand))
		{
			this.m_actor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
		}
		else if (this.m_mousedOverByOpponent)
		{
			this.m_actor.SetActorState(ActorStateType.CARD_OPPONENT_MOUSE_OVER);
		}
		else
		{
			this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
		}
	}

	// Token: 0x0600181D RID: 6173 RVA: 0x00070928 File Offset: 0x0006EB28
	private bool DoChoiceHighlight(GameState state)
	{
		List<Entity> chosenEntities = state.GetChosenEntities();
		if (chosenEntities.Contains(this.m_entity))
		{
			if (this.m_mousedOver)
			{
				this.m_actor.SetActorState(ActorStateType.CARD_PLAYABLE_MOUSE_OVER);
			}
			else
			{
				this.m_actor.SetActorState(ActorStateType.CARD_SELECTED);
			}
			return true;
		}
		int entityId = this.m_entity.GetEntityId();
		Network.EntityChoices friendlyEntityChoices = state.GetFriendlyEntityChoices();
		if (friendlyEntityChoices.Entities.Contains(entityId))
		{
			if (GameState.Get().IsMulliganManagerActive())
			{
				this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
			}
			else
			{
				this.m_actor.SetActorState(ActorStateType.CARD_SELECTABLE);
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600181E RID: 6174 RVA: 0x000709CC File Offset: 0x0006EBCC
	private bool DoOptionHighlight(GameState state)
	{
		if (!GameState.Get().IsOption(this.m_entity))
		{
			return false;
		}
		bool flag = this.m_entity.GetZone() == TAG_ZONE.HAND;
		bool flag2 = this.m_entity.GetController().IsRealTimeComboActive();
		if (flag && flag2 && this.m_entity.HasTag(GAME_TAG.COMBO))
		{
			this.m_actor.SetActorState(ActorStateType.CARD_COMBO);
			return true;
		}
		bool realTimePoweredUp = this.m_entity.GetRealTimePoweredUp();
		if (flag && realTimePoweredUp)
		{
			this.m_actor.SetActorState(ActorStateType.CARD_POWERED_UP);
			return true;
		}
		if (!flag && this.m_mousedOver)
		{
			this.m_actor.SetActorState(ActorStateType.CARD_PLAYABLE_MOUSE_OVER);
			return true;
		}
		this.m_actor.SetActorState(ActorStateType.CARD_PLAYABLE);
		return true;
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x00070A94 File Offset: 0x0006EC94
	private bool DoSubOptionHighlight(GameState state)
	{
		Network.Options.Option selectedNetworkOption = state.GetSelectedNetworkOption();
		int entityId = this.m_entity.GetEntityId();
		foreach (Network.Options.Option.SubOption subOption in selectedNetworkOption.Subs)
		{
			if (entityId == subOption.ID)
			{
				if (this.m_mousedOver)
				{
					this.m_actor.SetActorState(ActorStateType.CARD_PLAYABLE_MOUSE_OVER);
				}
				else
				{
					this.m_actor.SetActorState(ActorStateType.CARD_PLAYABLE);
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001820 RID: 6176 RVA: 0x00070B40 File Offset: 0x0006ED40
	private bool DoOptionTargetHighlight(GameState state)
	{
		Network.Options.Option.SubOption selectedNetworkSubOption = state.GetSelectedNetworkSubOption();
		int entityId = this.m_entity.GetEntityId();
		if (selectedNetworkSubOption.Targets.Contains(entityId))
		{
			if (this.m_mousedOver)
			{
				this.m_actor.SetActorState(ActorStateType.CARD_VALID_TARGET_MOUSE_OVER);
			}
			else
			{
				this.m_actor.SetActorState(ActorStateType.CARD_VALID_TARGET);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06001821 RID: 6177 RVA: 0x00070B9E File Offset: 0x0006ED9E
	public Actor GetActor()
	{
		return this.m_actor;
	}

	// Token: 0x06001822 RID: 6178 RVA: 0x00070BA6 File Offset: 0x0006EDA6
	public void SetActor(Actor actor)
	{
		this.m_actor = actor;
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x00070BAF File Offset: 0x0006EDAF
	public string GetActorName()
	{
		return this.m_actorName;
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x00070BB7 File Offset: 0x0006EDB7
	public void SetActorName(string actorName)
	{
		this.m_actorName = actorName;
	}

	// Token: 0x06001825 RID: 6181 RVA: 0x00070BC0 File Offset: 0x0006EDC0
	public bool IsActorReady()
	{
		return this.m_actorReady;
	}

	// Token: 0x06001826 RID: 6182 RVA: 0x00070BC8 File Offset: 0x0006EDC8
	public bool IsActorLoading()
	{
		return this.m_actorLoading;
	}

	// Token: 0x06001827 RID: 6183 RVA: 0x00070BD0 File Offset: 0x0006EDD0
	public void UpdateActorComponents()
	{
		if (this.m_actor == null)
		{
			return;
		}
		this.m_actor.UpdateAllComponents();
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x00070BEF File Offset: 0x0006EDEF
	public void RefreshActor()
	{
		this.UpdateActorState();
		if (this.m_entity.IsEnchanted())
		{
			this.UpdateEnchantments();
		}
		this.UpdateTooltip();
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x00070C13 File Offset: 0x0006EE13
	public Zone GetZone()
	{
		return this.m_zone;
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x00070C1B File Offset: 0x0006EE1B
	public Zone GetPrevZone()
	{
		return this.m_prevZone;
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x00070C23 File Offset: 0x0006EE23
	public void SetZone(Zone zone)
	{
		this.m_zone = zone;
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x00070C2C File Offset: 0x0006EE2C
	public int GetZonePosition()
	{
		return this.m_zonePosition;
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x00070C34 File Offset: 0x0006EE34
	public void SetZonePosition(int pos)
	{
		this.m_zonePosition = pos;
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x00070C3D File Offset: 0x0006EE3D
	public int GetPredictedZonePosition()
	{
		return this.m_predictedZonePosition;
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x00070C45 File Offset: 0x0006EE45
	public void SetPredictedZonePosition(int pos)
	{
		this.m_predictedZonePosition = pos;
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x00070C4E File Offset: 0x0006EE4E
	public ZoneTransitionStyle GetTransitionStyle()
	{
		return this.m_transitionStyle;
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x00070C56 File Offset: 0x0006EE56
	public void SetTransitionStyle(ZoneTransitionStyle style)
	{
		this.m_transitionStyle = style;
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x00070C5F File Offset: 0x0006EE5F
	public bool IsTransitioningZones()
	{
		return this.m_transitioningZones;
	}

	// Token: 0x06001833 RID: 6195 RVA: 0x00070C67 File Offset: 0x0006EE67
	public void EnableTransitioningZones(bool enable)
	{
		this.m_transitioningZones = enable;
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x00070C70 File Offset: 0x0006EE70
	public bool HasBeenGrabbedByEnemyActionHandler()
	{
		return this.m_hasBeenGrabbedByEnemyActionHandler;
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x00070C78 File Offset: 0x0006EE78
	public void MarkAsGrabbedByEnemyActionHandler(bool enable)
	{
		Log.FaceDownCard.Print("Card.MarkAsGrabbedByEnemyActionHandler() - card={0} enable={1}", new object[]
		{
			this,
			enable
		});
		this.m_hasBeenGrabbedByEnemyActionHandler = enable;
	}

	// Token: 0x06001836 RID: 6198 RVA: 0x00070CA3 File Offset: 0x0006EEA3
	public bool IsDoNotSort()
	{
		return this.m_doNotSort;
	}

	// Token: 0x06001837 RID: 6199 RVA: 0x00070CAC File Offset: 0x0006EEAC
	public void SetDoNotSort(bool on)
	{
		if (this.m_entity.IsControlledByOpposingSidePlayer())
		{
			Log.FaceDownCard.Print("Card.SetDoNotSort() - card={0} on={1}", new object[]
			{
				this,
				on
			});
		}
		this.m_doNotSort = on;
	}

	// Token: 0x06001838 RID: 6200 RVA: 0x00070CF2 File Offset: 0x0006EEF2
	public bool IsDoNotWarpToNewZone()
	{
		return this.m_doNotWarpToNewZone;
	}

	// Token: 0x06001839 RID: 6201 RVA: 0x00070CFA File Offset: 0x0006EEFA
	public void SetDoNotWarpToNewZone(bool on)
	{
		this.m_doNotWarpToNewZone = on;
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x00070D03 File Offset: 0x0006EF03
	public float GetTransitionDelay()
	{
		return this.m_transitionDelay;
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x00070D0B File Offset: 0x0006EF0B
	public void SetTransitionDelay(float delay)
	{
		this.m_transitionDelay = delay;
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x00070D14 File Offset: 0x0006EF14
	public bool IsHoldingForLinkedCardSwitch()
	{
		return this.m_holdingForLinkedCardSwitch;
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x00070D1C File Offset: 0x0006EF1C
	public void SetHoldingForLinkedCardSwitch(bool hold)
	{
		this.m_holdingForLinkedCardSwitch = hold;
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x00070D28 File Offset: 0x0006EF28
	public void UpdateZoneFromTags()
	{
		this.m_zonePosition = this.m_entity.GetZonePosition();
		Zone zone = ZoneMgr.Get().FindZoneForEntity(this.m_entity);
		this.TransitionToZone(zone);
		if (zone != null)
		{
			zone.UpdateLayout();
		}
	}

	// Token: 0x0600183F RID: 6207 RVA: 0x00070D70 File Offset: 0x0006EF70
	public void TransitionToZone(Zone zone)
	{
		if (this.m_zone == zone)
		{
			Log.Mike.Print("Card.TransitionToZone() - card={0} already in target zone", new object[]
			{
				this
			});
			return;
		}
		if (!(zone == null))
		{
			this.m_prevZone = this.m_zone;
			this.m_zone = zone;
			if (this.m_prevZone != null)
			{
				this.m_prevZone.RemoveCard(this);
			}
			this.m_zone.AddCard(this);
			if (this.m_zone is ZoneGraveyard && GameState.Get().IsBeingDrawn(this))
			{
				this.m_actorReady = true;
				this.DiscardCardBeingDrawn();
			}
			else if (this.m_zone is ZoneGraveyard && this.m_ignoreDeath)
			{
				this.m_actorReady = true;
			}
			else
			{
				this.m_actorReady = false;
				this.LoadActorAndSpells();
			}
			return;
		}
		this.m_zone.RemoveCard(this);
		this.m_prevZone = this.m_zone;
		this.m_zone = null;
		if (this.SwitchOutLinkedDrawnCard())
		{
			return;
		}
		this.DeactivateLifetimeEffects();
		this.DeactivateCustomKeywordEffect();
		if (this.m_prevZone is ZoneHand)
		{
			this.DeactivateHandStateSpells();
		}
		this.DoNullZoneVisuals();
	}

	// Token: 0x06001840 RID: 6208 RVA: 0x00070EB0 File Offset: 0x0006F0B0
	public void UpdateActor(bool forceIfNullZone = false)
	{
		if (!forceIfNullZone && this.m_zone == null)
		{
			return;
		}
		TAG_ZONE zone = this.m_entity.GetZone();
		string text = this.m_cardDef.DetermineActorNameForZone(this.m_entity, zone);
		if (this.m_actor != null && this.m_actorName == text)
		{
			return;
		}
		GameObject gameObject = AssetLoader.Get().LoadActor(text, false, false);
		if (!gameObject)
		{
			Debug.LogWarningFormat("Card.UpdateActor() - FAILED to load actor \"{0}\"", new object[]
			{
				text
			});
			return;
		}
		Actor component = gameObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarningFormat("Card.UpdateActor() - ERROR actor \"{0}\" has no Actor component", new object[]
			{
				text
			});
			return;
		}
		if (this.m_actor != null)
		{
			this.m_actor.Destroy();
		}
		this.m_actor = component;
		this.m_actorName = text;
		this.m_actor.SetEntity(this.m_entity);
		this.m_actor.SetCard(this);
		this.m_actor.SetCardDef(this.m_cardDef);
		this.m_actor.UpdateAllComponents();
		if (this.m_shown)
		{
			this.ShowImpl();
		}
		else
		{
			this.HideImpl();
		}
		this.RefreshActor();
	}

	// Token: 0x06001841 RID: 6209 RVA: 0x00070FF4 File Offset: 0x0006F1F4
	private void LoadActorAndSpells()
	{
		this.m_actorLoading = true;
		List<Card.SpellLoadRequest> list = new List<Card.SpellLoadRequest>();
		if (this.m_prevZone is ZoneHand && this.m_zone is ZonePlay)
		{
			Card.SpellLoadRequest spellLoadRequest = this.MakeCustomSpellLoadRequest(this.m_cardDef.m_CustomSummonSpellPath, this.m_cardDef.m_GoldenCustomSummonSpellPath, new AssetLoader.GameObjectCallback(this.OnCustomSummonSpellLoaded));
			if (spellLoadRequest != null)
			{
				list.Add(spellLoadRequest);
			}
		}
		if (!this.m_customDeathSpell && (this.m_zone is ZoneHand || this.m_zone is ZonePlay))
		{
			Card.SpellLoadRequest spellLoadRequest2 = this.MakeCustomSpellLoadRequest(this.m_cardDef.m_CustomDeathSpellPath, this.m_cardDef.m_GoldenCustomDeathSpellPath, new AssetLoader.GameObjectCallback(this.OnCustomDeathSpellLoaded));
			if (spellLoadRequest2 != null)
			{
				list.Add(spellLoadRequest2);
			}
		}
		if (!this.m_customSpawnSpell && this.m_zone is ZonePlay)
		{
			Card.SpellLoadRequest spellLoadRequest3 = this.MakeCustomSpellLoadRequest(this.m_cardDef.m_CustomSpawnSpellPath, this.m_cardDef.m_GoldenCustomSpawnSpellPath, new AssetLoader.GameObjectCallback(this.OnCustomSpawnSpellLoaded));
			if (spellLoadRequest3 != null)
			{
				list.Add(spellLoadRequest3);
			}
		}
		this.m_spellLoadCount = list.Count;
		if (list.Count == 0)
		{
			this.LoadActor();
		}
		else
		{
			foreach (Card.SpellLoadRequest spellLoadRequest4 in list)
			{
				AssetLoader.Get().LoadSpell(spellLoadRequest4.m_path, spellLoadRequest4.m_loadCallback, null, false);
			}
		}
	}

	// Token: 0x06001842 RID: 6210 RVA: 0x000711A4 File Offset: 0x0006F3A4
	private Card.SpellLoadRequest MakeCustomSpellLoadRequest(string customPath, string goldenCustomPath, AssetLoader.GameObjectCallback loadCallback)
	{
		string text = customPath;
		TAG_PREMIUM premiumType = this.m_entity.GetPremiumType();
		if (premiumType == TAG_PREMIUM.GOLDEN && !string.IsNullOrEmpty(goldenCustomPath))
		{
			text = goldenCustomPath;
		}
		else if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return new Card.SpellLoadRequest
		{
			m_path = text,
			m_loadCallback = loadCallback
		};
	}

	// Token: 0x06001843 RID: 6211 RVA: 0x000711FC File Offset: 0x0006F3FC
	private void OnCustomSummonSpellLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Error.AddDevFatal("Card.OnCustomSummonSpellLoaded() - FAILED to load \"{0}\" for card {1}", new object[]
			{
				name,
				this
			});
			this.FinishSpellLoad();
			return;
		}
		this.m_customSummonSpell = go.GetComponent<Spell>();
		if (this.m_customSummonSpell == null)
		{
			this.FinishSpellLoad();
			return;
		}
		SpellUtils.SetupSpell(this.m_customSummonSpell, this);
		this.FinishSpellLoad();
	}

	// Token: 0x06001844 RID: 6212 RVA: 0x0007126C File Offset: 0x0006F46C
	private void OnCustomDeathSpellLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Error.AddDevFatal("Card.OnCustomDeathSpellLoaded() - FAILED to load \"{0}\" for card {1}", new object[]
			{
				name,
				this
			});
			this.FinishSpellLoad();
			return;
		}
		this.m_customDeathSpell = go.GetComponent<Spell>();
		if (this.m_customDeathSpell == null)
		{
			this.FinishSpellLoad();
			return;
		}
		SpellUtils.SetupSpell(this.m_customDeathSpell, this);
		this.FinishSpellLoad();
	}

	// Token: 0x06001845 RID: 6213 RVA: 0x000712DC File Offset: 0x0006F4DC
	private void OnCustomSpawnSpellLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Error.AddDevFatal("Card.OnCustomSpawnSpellLoaded() - FAILED to load \"{0}\" for card {1}", new object[]
			{
				name,
				this
			});
			this.FinishSpellLoad();
			return;
		}
		this.m_customSpawnSpell = go.GetComponent<Spell>();
		if (this.m_customSpawnSpell == null)
		{
			this.FinishSpellLoad();
			return;
		}
		SpellUtils.SetupSpell(this.m_customSpawnSpell, this);
		this.FinishSpellLoad();
	}

	// Token: 0x06001846 RID: 6214 RVA: 0x0007134A File Offset: 0x0006F54A
	private void FinishSpellLoad()
	{
		this.m_spellLoadCount--;
		if (this.m_spellLoadCount > 0)
		{
			return;
		}
		this.LoadActor();
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x00071370 File Offset: 0x0006F570
	private void LoadActor()
	{
		string text = this.m_cardDef.DetermineActorNameForZone(this.m_entity, this.m_zone.m_ServerTag);
		if (this.m_actorName == text || text == null)
		{
			this.m_actorName = text;
			this.FinishActorLoad(this.m_actor);
			return;
		}
		AssetLoader.Get().LoadActor(text, new AssetLoader.GameObjectCallback(this.OnActorLoaded), null, false);
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x000713E0 File Offset: 0x0006F5E0
	private void OnActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("Card.OnActorLoaded() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		Actor component = actorObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("Card.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		Actor actor = this.m_actor;
		this.m_actor = component;
		this.m_actorName = actorName;
		this.m_actor.SetEntity(this.m_entity);
		this.m_actor.SetCard(this);
		this.m_actor.SetCardDef(this.m_cardDef);
		this.m_actor.UpdateAllComponents();
		this.FinishActorLoad(actor);
	}

	// Token: 0x06001849 RID: 6217 RVA: 0x00071484 File Offset: 0x0006F684
	private void FinishActorLoad(Actor oldActor)
	{
		this.m_actorLoading = false;
		this.OnZoneChanged();
		this.OnActorChanged(oldActor);
		if (this.m_isBattleCrySource)
		{
			SceneUtils.SetLayer(this.m_actor.gameObject, GameLayer.IgnoreFullScreenEffects);
		}
		this.RefreshActor();
	}

	// Token: 0x0600184A RID: 6218 RVA: 0x000714C8 File Offset: 0x0006F6C8
	public void ForceLoadHandActor()
	{
		string text = this.m_cardDef.DetermineActorNameForZone(this.m_entity, TAG_ZONE.HAND);
		if (this.m_actor != null && this.m_actorName == text)
		{
			this.ShowCard();
			this.m_actor.Show();
			this.RefreshActor();
			return;
		}
		GameObject gameObject = AssetLoader.Get().LoadActor(text, false, false);
		if (gameObject == null)
		{
			Debug.LogWarningFormat("Card.ForceLoadHandActor() - FAILED to load actor \"{0}\"", new object[]
			{
				text
			});
			return;
		}
		Actor component = gameObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarningFormat("Card.ForceLoadHandActor() - ERROR actor \"{0}\" has no Actor component", new object[]
			{
				text
			});
			return;
		}
		if (this.m_actor != null)
		{
			this.m_actor.Destroy();
		}
		this.m_actor = component;
		this.m_actorName = text;
		this.m_actor.SetEntity(this.m_entity);
		this.m_actor.SetCard(this);
		this.m_actor.SetCardDef(this.m_cardDef);
		this.m_actor.UpdateAllComponents();
		if (this.m_shown)
		{
			this.ShowImpl();
		}
		else
		{
			this.HideImpl();
		}
		this.RefreshActor();
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x00071600 File Offset: 0x0006F800
	private void OnZoneChanged()
	{
		if (this.m_prevZone is ZoneHand && this.m_zone is ZoneGraveyard)
		{
			this.DoDiscardAnimation();
		}
		else if (this.m_zone is ZoneGraveyard)
		{
			if (this.m_entity.IsHero())
			{
				this.m_doNotSort = true;
			}
		}
		else if (this.m_zone is ZoneHand)
		{
			if (!this.m_doNotSort)
			{
				this.ShowCard();
			}
			if (this.m_prevZone is ZoneGraveyard && this.m_entity.IsSpell())
			{
				this.m_actor.Hide();
				this.ActivateActorSpell(SpellType.SUMMON_IN);
			}
		}
		else if ((this.m_prevZone is ZoneGraveyard || this.m_prevZone is ZoneDeck) && this.m_zone.m_ServerTag == TAG_ZONE.PLAY)
		{
			this.ShowCard();
		}
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x000716F4 File Offset: 0x0006F8F4
	private void OnActorChanged(Actor oldActor)
	{
		this.HideTooltip();
		bool flag = false;
		bool flag2 = GameState.Get().IsGameCreating();
		if (oldActor == null)
		{
			bool flag3 = GameState.Get().IsMulliganPhaseNowOrPending();
			if (this.m_zone is ZoneHand && GameState.Get().IsBeginPhase())
			{
				bool flag4 = this.m_entity.GetCardId() == "GAME_005";
				if (flag3 && !GameState.Get().HasTheCoinBeenSpawned())
				{
					if (flag4)
					{
						GameState.Get().NotifyOfCoinSpawn();
						this.m_actor.TurnOffCollider();
						this.m_actor.Hide();
						this.m_actorReady = true;
						flag = true;
						base.transform.position = Vector3.zero;
						this.m_doNotWarpToNewZone = true;
						this.m_doNotSort = true;
					}
					else
					{
						Player controller = this.m_entity.GetController();
						if (controller.IsOpposingSide() && this == this.m_zone.GetLastCard() && !controller.HasTag(GAME_TAG.FIRST_PLAYER))
						{
							GameState.Get().NotifyOfCoinSpawn();
							this.m_actor.TurnOffCollider();
							this.m_actorReady = true;
							flag = true;
						}
					}
				}
				if (!flag4)
				{
					ZoneDeck zoneDeck = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(this.m_zone.m_Side);
					zoneDeck.SetCardToInDeckState(this);
				}
			}
			else if (flag2)
			{
				TransformUtil.CopyWorld(base.transform, this.m_zone.transform);
				if (this.m_zone is ZonePlay)
				{
					this.ActivateLifetimeEffects();
				}
			}
			else
			{
				if (!this.m_doNotWarpToNewZone)
				{
					TransformUtil.CopyWorld(base.transform, this.m_zone.transform);
				}
				if (this.m_zone is ZoneHand)
				{
					if (!this.m_doNotWarpToNewZone)
					{
						ZoneHand zoneHand = (ZoneHand)this.m_zone;
						base.transform.localScale = zoneHand.GetCardScale(this);
						base.transform.localEulerAngles = zoneHand.GetCardRotation(this);
						base.transform.position = zoneHand.GetCardPosition(this);
					}
					if (this.m_entity.HasTag(GAME_TAG.LINKED_ENTITY))
					{
						int tag = this.m_entity.GetTag(GAME_TAG.LINKED_ENTITY);
						Entity entity = GameState.Get().GetEntity(tag);
						if (entity != null && entity.GetCard() != null)
						{
							this.m_actor.Hide();
							this.m_doNotSort = true;
							flag = true;
						}
					}
					else
					{
						this.m_actorReady = true;
						this.m_shown = true;
						if (!this.m_doNotWarpToNewZone)
						{
							this.m_actor.Hide();
							this.ActivateHandSpawnSpell();
							flag = true;
						}
					}
				}
				if (this.m_prevZone == null && this.m_zone is ZonePlay)
				{
					if (!this.m_doNotWarpToNewZone)
					{
						ZonePlay zonePlay = (ZonePlay)this.m_zone;
						base.transform.position = zonePlay.GetCardPosition(this);
					}
					if (this.m_entity.HasTag(GAME_TAG.LINKED_ENTITY))
					{
						this.m_transitionStyle = ZoneTransitionStyle.INSTANT;
						this.ActivateCharacterPlayEffects();
						this.OnSpellFinished_StandardSpawnMinion(null, null);
					}
					else
					{
						this.m_actor.Hide();
						this.ActivateMinionSpawnEffects();
					}
					flag = true;
				}
				else if (!flag3 && (this.m_zone is ZoneHeroPower || this.m_zone is ZoneWeapon) && this.IsShown())
				{
					this.ActivateDefaultSpawnSpell(new Spell.FinishedCallback(this.OnSpellFinished_DefaultPlaySpawn));
					flag = true;
					this.m_actorReady = true;
				}
			}
		}
		else if (this.m_prevZone == null && (this.m_zone is ZoneHeroPower || this.m_zone is ZoneWeapon))
		{
			oldActor.Destroy();
			TransformUtil.CopyWorld(base.transform, this.m_zone.transform);
			this.m_transitionStyle = ZoneTransitionStyle.INSTANT;
			this.ActivateDefaultSpawnSpell(new Spell.FinishedCallback(this.OnSpellFinished_DefaultPlaySpawn));
			flag = true;
			this.m_actorReady = true;
		}
		else if (this.m_prevZone is ZoneHand && this.m_zone is ZonePlay)
		{
			bool flag5 = this.ActivateActorSpells_HandToPlay(oldActor);
			if (flag5)
			{
				this.ActivateCharacterPlayEffects();
				this.m_actor.Hide();
				flag = true;
				if (CardTypeBanner.Get() != null && CardTypeBanner.Get().GetCardDef() != null && CardTypeBanner.Get().GetCardDef() == this.GetCardDef())
				{
					CardTypeBanner.Get().Hide();
				}
			}
		}
		else if (this.m_prevZone is ZoneHand && this.m_zone is ZoneWeapon)
		{
			bool flag6 = this.ActivateActorSpells_HandToWeapon(oldActor);
			if (flag6)
			{
				this.m_actor.Hide();
				flag = true;
				if (CardTypeBanner.Get() != null && CardTypeBanner.Get().GetCardDef() != null && CardTypeBanner.Get().GetCardDef() == this.GetCardDef())
				{
					CardTypeBanner.Get().Hide();
				}
			}
		}
		else if (this.m_prevZone is ZonePlay && this.m_zone is ZoneHand)
		{
			if (this.DoPlayToHandTransition(oldActor, false))
			{
				flag = true;
			}
		}
		else if (this.m_prevZone != null && (this.m_prevZone is ZonePlay || this.m_prevZone is ZoneWeapon || this.m_prevZone is ZoneHeroPower) && this.m_zone is ZoneGraveyard)
		{
			if (this.m_mousedOver && this.m_entity.HasSpellPower() && this.m_entity.IsControlledByFriendlySidePlayer() && this.m_prevZone is ZonePlay)
			{
				ZoneHand zoneHand2 = ZoneMgr.Get().FindZoneOfType<ZoneHand>(this.m_prevZone.m_Side);
				zoneHand2.OnSpellPowerEntityMousedOut();
			}
			if (this.m_entity.HasTag(GAME_TAG.DEATHRATTLE_RETURN_ZONE) && this.DoesCardReturnFromGraveyard())
			{
				this.m_prevZone.AddLayoutBlocker();
				TAG_ZONE tag2 = this.m_entity.GetTag<TAG_ZONE>(GAME_TAG.DEATHRATTLE_RETURN_ZONE);
				Zone zone = ZoneMgr.Get().FindZoneForEntityAndZoneTag(this.m_entity, tag2);
				if (zone is ZoneDeck)
				{
					zone.AddLayoutBlocker();
				}
				this.m_actorWaitingToBeReplaced = oldActor;
				this.m_actor.Hide();
				flag = true;
				this.m_actorReady = true;
			}
			else if (this.HandlePlayActorDeath(oldActor))
			{
				flag = true;
			}
		}
		else if (this.m_prevZone is ZoneDeck && this.m_zone is ZoneHand)
		{
			if (this.m_zone.m_Side == Player.Side.FRIENDLY)
			{
				if (GameState.Get().IsPastBeginPhase())
				{
					this.m_actorWaitingToBeReplaced = oldActor;
					if (!TurnStartManager.Get().IsCardDrawHandled(this))
					{
						this.DrawFriendlyCard();
					}
					flag = true;
				}
				else
				{
					this.m_actor.TurnOffCollider();
					this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
				}
			}
			else if (GameState.Get().IsPastBeginPhase())
			{
				if (oldActor != null)
				{
					oldActor.Destroy();
				}
				this.DrawOpponentCard();
				flag = true;
			}
		}
		else if (this.m_prevZone is ZoneSecret && this.m_zone is ZoneGraveyard)
		{
			flag = true;
			this.m_actorReady = true;
			if (UniversalInputManager.UsePhoneUI)
			{
				this.m_shown = false;
				this.m_actor.Hide();
			}
			else
			{
				this.ShowSecretDeath(oldActor);
			}
		}
		else if (this.m_prevZone is ZoneGraveyard && this.m_zone is ZonePlay)
		{
			this.m_actor.Hide();
			base.StartCoroutine(this.ActivateReviveSpell());
			flag = true;
		}
		else if (this.m_prevZone is ZoneDeck && this.m_zone is ZoneGraveyard)
		{
			this.MillCard();
			flag = true;
		}
		else if (this.m_prevZone is ZoneDeck && this.m_zone is ZonePlay)
		{
			this.m_doNotSort = true;
			if (oldActor != null)
			{
				oldActor.Destroy();
			}
			this.AnimateDeckToPlay();
			flag = true;
		}
		else if (this.m_prevZone is ZonePlay && this.m_zone is ZoneDeck)
		{
			this.m_prevZone.AddLayoutBlocker();
			ZoneDeck zoneDeck2 = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(this.m_zone.m_Side);
			zoneDeck2.AddLayoutBlocker();
			this.DoPlayToDeckTransition(oldActor);
			flag = true;
		}
		else if (this.m_prevZone is ZoneGraveyard && this.m_zone is ZoneDeck)
		{
			if (this.HandleGraveyardToDeck(oldActor))
			{
				flag = true;
			}
		}
		else if (this.m_prevZone is ZoneGraveyard && this.m_zone is ZoneHand && this.HandleGraveyardToHand(oldActor))
		{
			flag = true;
		}
		if (!flag && oldActor == this.m_actor)
		{
			if (this.m_prevZone != null && this.m_prevZone.m_Side != this.m_zone.m_Side && this.m_prevZone is ZoneSecret && this.m_zone is ZoneSecret)
			{
				base.StartCoroutine(this.SwitchSecretSides());
				flag = true;
			}
			if (!flag)
			{
				this.m_actorReady = true;
			}
			return;
		}
		if (!flag && this.m_zone is ZoneSecret)
		{
			this.m_shown = true;
			if (oldActor)
			{
				oldActor.Destroy();
			}
			this.m_transitionStyle = ZoneTransitionStyle.INSTANT;
			this.ShowSecretBirth();
			flag = true;
			this.m_actorReady = true;
			if (flag2)
			{
				this.ActivateStateSpells();
			}
		}
		if (!flag)
		{
			if (oldActor)
			{
				oldActor.Destroy();
			}
			bool flag7 = this.m_zone.m_ServerTag == TAG_ZONE.PLAY || this.m_zone.m_ServerTag == TAG_ZONE.SECRET || this.m_zone.m_ServerTag == TAG_ZONE.HAND;
			if (this.IsShown() && flag7)
			{
				this.ActivateStateSpells();
			}
			this.m_actorReady = true;
			if (this.IsShown())
			{
				this.ShowImpl();
			}
			else
			{
				this.HideImpl();
			}
		}
	}

	// Token: 0x0600184D RID: 6221 RVA: 0x0007214E File Offset: 0x0007034E
	private bool HandleGraveyardToDeck(Actor oldActor)
	{
		if (this.m_actorWaitingToBeReplaced)
		{
			if (oldActor)
			{
				oldActor.Destroy();
			}
			oldActor = this.m_actorWaitingToBeReplaced;
			this.m_actorWaitingToBeReplaced = null;
			this.DoPlayToDeckTransition(oldActor);
			return true;
		}
		return false;
	}

	// Token: 0x0600184E RID: 6222 RVA: 0x0007218C File Offset: 0x0007038C
	private bool HandleGraveyardToHand(Actor oldActor)
	{
		if (this.m_actorWaitingToBeReplaced)
		{
			if (oldActor && oldActor != this.m_actor)
			{
				oldActor.Destroy();
			}
			oldActor = this.m_actorWaitingToBeReplaced;
			this.m_actorWaitingToBeReplaced = null;
			if (this.DoPlayToHandTransition(oldActor, true))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600184F RID: 6223 RVA: 0x000721EA File Offset: 0x000703EA
	public bool CardStandInIsInteractive()
	{
		return this.m_cardStandInInteractive;
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x000721F2 File Offset: 0x000703F2
	public void DrawFriendlyCard()
	{
		base.StartCoroutine(this.DrawFriendlyCardWithTiming());
	}

	// Token: 0x06001851 RID: 6225 RVA: 0x00072204 File Offset: 0x00070404
	private IEnumerator DrawFriendlyCardWithTiming()
	{
		this.m_doNotSort = true;
		this.m_transitionStyle = ZoneTransitionStyle.SLOW;
		this.m_cardStandInInteractive = false;
		this.m_actor.Hide();
		while (GameState.Get().GetFriendlyCardBeingDrawn())
		{
			yield return null;
		}
		GameState.Get().SetFriendlyCardBeingDrawn(this);
		Actor standIn = Gameplay.Get().GetCardDrawStandIn();
		standIn.transform.parent = this.m_actor.transform.parent;
		standIn.transform.localPosition = Vector3.zero;
		standIn.transform.localScale = Vector3.one;
		standIn.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
		standIn.Show();
		CardBackDisplay cbDisplay = standIn.GetRootObject().GetComponentInChildren<CardBackDisplay>();
		cbDisplay.SetCardBack(true);
		if (this.m_actorWaitingToBeReplaced != null)
		{
			this.m_actorWaitingToBeReplaced.Destroy();
			this.m_actorWaitingToBeReplaced = null;
		}
		Transform bone = Board.Get().FindBone("FriendlyDrawCard");
		Vector3[] drawPath = new Vector3[]
		{
			base.gameObject.transform.position,
			base.gameObject.transform.position + Card.ABOVE_DECK_OFFSET,
			bone.position
		};
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"path",
			drawPath,
			"time",
			1.5f,
			"easetype",
			iTween.EaseType.easeInSineOutExpo
		}));
		base.gameObject.transform.localEulerAngles = new Vector3(270f, 270f, 0f);
		iTween.RotateTo(base.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 0f, 357f),
			"time",
			1.35f,
			"delay",
			0.15f
		}));
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			bone.localScale,
			"time",
			0.75f,
			"delay",
			0.15f
		}));
		SoundManager.Get().LoadAndPlay("draw_card_1", base.gameObject);
		standIn.transform.parent = null;
		standIn.Hide();
		this.m_actor.Show();
		this.m_actor.TurnOffCollider();
		while (iTween.Count(base.gameObject) > 0)
		{
			yield return null;
		}
		this.m_actorReady = true;
		ZoneHand zoneHand = (ZoneHand)this.m_zone;
		if (this.ShouldCardDrawWaitForTurnStartSpells())
		{
			yield return base.StartCoroutine(this.WaitForCardDrawBlockingTurnStartSpells());
		}
		else if (this.ShouldCardDrawWaitForTaskLists())
		{
			yield return base.StartCoroutine(this.WaitForCardDrawBlockingTaskLists());
		}
		while (this.m_holdingForLinkedCardSwitch)
		{
			yield return null;
		}
		this.m_doNotSort = false;
		GameState.Get().ClearCardBeingDrawn(this);
		if (this.m_zone == zoneHand)
		{
			SoundManager.Get().LoadAndPlay("add_card_to_hand_1", base.gameObject);
			this.ActivateStateSpells();
			this.RefreshActor();
			this.m_zone.UpdateLayout();
			yield return new WaitForSeconds(0.3f);
			this.m_cardStandInInteractive = true;
			zoneHand.MakeStandInInteractive(this);
		}
		yield break;
	}

	// Token: 0x06001852 RID: 6226 RVA: 0x0007221F File Offset: 0x0007041F
	public bool IsBeingDrawnByOpponent()
	{
		return this.m_beingDrawnByOpponent;
	}

	// Token: 0x06001853 RID: 6227 RVA: 0x00072227 File Offset: 0x00070427
	private void DrawOpponentCard()
	{
		base.StartCoroutine(this.DrawOpponentCardWithTiming());
	}

	// Token: 0x06001854 RID: 6228 RVA: 0x00072238 File Offset: 0x00070438
	private IEnumerator DrawOpponentCardWithTiming()
	{
		this.m_doNotSort = true;
		this.m_beingDrawnByOpponent = true;
		while (GameState.Get().GetOpponentCardBeingDrawn())
		{
			yield return null;
		}
		GameState.Get().SetOpponentCardBeingDrawn(this);
		ZoneHand handZone = (ZoneHand)this.m_zone;
		handZone.UpdateLayout();
		if (this.m_entity.HasTag(GAME_TAG.REVEALED) || this.m_entity.HasTag(GAME_TAG.TOPDECK))
		{
			base.StartCoroutine(this.DrawKnownOpponentCard(handZone));
		}
		else
		{
			base.StartCoroutine(this.DrawUnknownOpponentCard(handZone));
		}
		yield break;
	}

	// Token: 0x06001855 RID: 6229 RVA: 0x00072254 File Offset: 0x00070454
	private IEnumerator DrawUnknownOpponentCard(ZoneHand handZone)
	{
		SoundManager.Get().LoadAndPlay("draw_card_and_add_to_hand_opp_1", base.gameObject);
		base.gameObject.transform.rotation = Card.IN_DECK_HIDDEN_ROTATION;
		Transform bone = Board.Get().FindBone("OpponentDrawCard");
		Vector3[] drawPath = new Vector3[]
		{
			base.gameObject.transform.position,
			base.gameObject.transform.position + Card.ABOVE_DECK_OFFSET,
			bone.position,
			handZone.GetCardPosition(this)
		};
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"path",
			drawPath,
			"time",
			1.75f,
			"easetype",
			iTween.EaseType.easeInOutQuart
		}));
		iTween.RotateTo(base.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			handZone.GetCardRotation(this),
			"time",
			0.7f,
			"delay",
			0.8f,
			"easetype",
			iTween.EaseType.easeInOutCubic
		}));
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			handZone.GetCardScale(this),
			"time",
			0.7f,
			"delay",
			0.8f,
			"easetype",
			iTween.EaseType.easeInOutQuint
		}));
		yield return new WaitForSeconds(0.2f);
		this.m_actorReady = true;
		yield return new WaitForSeconds(0.6f);
		while (iTween.Count(base.gameObject) > 0)
		{
			yield return null;
		}
		this.m_doNotSort = false;
		this.m_beingDrawnByOpponent = false;
		GameState.Get().SetOpponentCardBeingDrawn(null);
		handZone.UpdateLayout();
		yield break;
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x00072280 File Offset: 0x00070480
	private IEnumerator DrawKnownOpponentCard(ZoneHand handZone)
	{
		Actor handActor = null;
		bool loadingActor = true;
		AssetLoader.GameObjectCallback actorLoadedCallback = delegate(string name, GameObject go, object callbackData)
		{
			loadingActor = false;
			if (go == null)
			{
				Error.AddDevFatal("Card.DrawKnownOpponentCard() - failed to load {0}", new object[]
				{
					name
				});
				return;
			}
			handActor = go.GetComponent<Actor>();
			if (handActor == null)
			{
				Error.AddDevFatal("Card.DrawKnownOpponentCard() - instance of {0} has no Actor component", new object[]
				{
					name
				});
				return;
			}
		};
		string actorName = ActorNames.GetHandActor(this.m_entity);
		AssetLoader.Get().LoadActor(actorName, actorLoadedCallback, null, false);
		while (loadingActor)
		{
			yield return null;
		}
		if (handActor)
		{
			handActor.SetEntity(this.m_entity);
			handActor.SetCardDef(this.m_cardDef);
			handActor.UpdateAllComponents();
			base.StartCoroutine(this.RevealDrawnOpponentCard(actorName, handActor, handZone));
		}
		else
		{
			base.StartCoroutine(this.DrawUnknownOpponentCard(handZone));
		}
		yield break;
	}

	// Token: 0x06001857 RID: 6231 RVA: 0x000722AC File Offset: 0x000704AC
	private IEnumerator RevealDrawnOpponentCard(string handActorName, Actor handActor, ZoneHand handZone)
	{
		SoundManager.Get().LoadAndPlay("draw_card_1", base.gameObject);
		handActor.transform.parent = this.m_actor.transform.parent;
		TransformUtil.CopyLocal(handActor, this.m_actor);
		this.m_actor.Hide();
		base.gameObject.transform.localEulerAngles = new Vector3(270f, 90f, 0f);
		string boneName = "OpponentDrawCardAndReveal";
		if (UniversalInputManager.UsePhoneUI)
		{
			boneName += "_phone";
		}
		Transform bone = Board.Get().FindBone(boneName);
		Vector3[] drawPath = new Vector3[]
		{
			base.gameObject.transform.position,
			base.gameObject.transform.position + Card.ABOVE_DECK_OFFSET,
			bone.position
		};
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"path",
			drawPath,
			"time",
			1.75f,
			"easetype",
			iTween.EaseType.easeInOutQuart
		}));
		iTween.RotateTo(base.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			bone.eulerAngles,
			"time",
			0.7f,
			"delay",
			0.8f,
			"easetype",
			iTween.EaseType.easeInOutCubic
		}));
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			bone.localScale,
			"time",
			0.7f,
			"delay",
			0.8f,
			"easetype",
			iTween.EaseType.easeInOutQuint
		}));
		yield return new WaitForSeconds(1.75f);
		this.m_actorReady = true;
		this.m_beingDrawnByOpponent = false;
		string actorName = this.m_actorName;
		this.m_actorWaitingToBeReplaced = this.m_actor;
		this.m_actorName = handActorName;
		this.m_actor = handActor;
		if (this.ShouldCardDrawWaitForTaskLists())
		{
			yield return base.StartCoroutine(this.WaitForCardDrawBlockingTaskLists());
		}
		while (this.m_holdingForLinkedCardSwitch)
		{
			yield return null;
		}
		if (this.m_zone != handZone)
		{
			this.m_doNotSort = false;
			GameState.Get().ClearCardBeingDrawn(this);
			yield break;
		}
		this.m_actor = this.m_actorWaitingToBeReplaced;
		this.m_actorName = actorName;
		this.m_actorWaitingToBeReplaced = null;
		this.m_beingDrawnByOpponent = true;
		yield return base.StartCoroutine(this.HideRevealedOpponentCard(handActor));
		yield break;
	}

	// Token: 0x06001858 RID: 6232 RVA: 0x000722F4 File Offset: 0x000704F4
	private IEnumerator HideRevealedOpponentCard(Actor handActor)
	{
		float flipSec = 0.5f;
		float revealSec = 0.525f * flipSec;
		if (!this.GetController().IsRevealed())
		{
			float handActorRotation = 180f;
			TransformUtil.SetEulerAngleZ(this.m_actor.gameObject, -handActorRotation);
			iTween.RotateAdd(handActor.gameObject, iTween.Hash(new object[]
			{
				"z",
				handActorRotation,
				"time",
				flipSec,
				"easetype",
				iTween.EaseType.easeInOutCubic
			}));
			iTween.RotateAdd(this.m_actor.gameObject, iTween.Hash(new object[]
			{
				"z",
				handActorRotation,
				"time",
				flipSec,
				"easetype",
				iTween.EaseType.easeInOutCubic
			}));
		}
		Action<object> changeActorsCallback = delegate(object obj)
		{
			Object.Destroy(handActor.gameObject);
			this.m_actor.Show();
		};
		iTween.Timer(this.m_actor.gameObject, iTween.Hash(new object[]
		{
			"time",
			revealSec,
			"oncomplete",
			changeActorsCallback
		}));
		yield return new WaitForSeconds(flipSec);
		this.m_doNotSort = false;
		this.m_beingDrawnByOpponent = false;
		GameState.Get().SetOpponentCardBeingDrawn(null);
		SoundManager.Get().LoadAndPlay("add_card_to_hand_1", base.gameObject);
		this.ActivateStateSpells();
		this.RefreshActor();
		this.m_zone.UpdateLayout();
		yield break;
	}

	// Token: 0x06001859 RID: 6233 RVA: 0x00072320 File Offset: 0x00070520
	private void AnimateDeckToPlay()
	{
		string handActor = ActorNames.GetHandActor(this.m_entity);
		GameObject gameObject = AssetLoader.Get().LoadActor(handActor, false, false);
		Actor component = gameObject.GetComponent<Actor>();
		this.SetupDeckToPlayActor(component, gameObject);
		SpellType spellType = this.m_cardDef.DetermineSummonOutSpell_HandToPlay(this);
		Spell spell = component.GetSpell(spellType);
		GameObject gameObject2 = AssetLoader.Get().LoadActor("Card_Hidden", false, false);
		Actor component2 = gameObject2.GetComponent<Actor>();
		this.SetupDeckToPlayActor(component2, gameObject2);
		base.StartCoroutine(this.AnimateDeckToPlay(component, spell, component2));
	}

	// Token: 0x0600185A RID: 6234 RVA: 0x000723A4 File Offset: 0x000705A4
	private void SetupDeckToPlayActor(Actor actor, GameObject actorObject)
	{
		actor.SetEntity(this.m_entity);
		actor.SetCardDef(this.m_cardDef);
		actor.UpdateAllComponents();
		actorObject.transform.parent = base.transform;
		actorObject.transform.localPosition = Vector3.zero;
		actorObject.transform.localScale = Vector3.one;
		actorObject.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x0600185B RID: 6235 RVA: 0x00072410 File Offset: 0x00070610
	private IEnumerator AnimateDeckToPlay(Actor cardFaceActor, Spell outSpell, Actor hiddenActor)
	{
		cardFaceActor.Hide();
		this.m_actor.Hide();
		hiddenActor.Hide();
		this.m_inputEnabled = false;
		SoundManager.Get().LoadAndPlay("draw_card_into_play", base.gameObject);
		base.gameObject.transform.localEulerAngles = new Vector3(270f, 90f, 0f);
		iTween.MoveTo(base.gameObject, base.gameObject.transform.position + Card.ABOVE_DECK_OFFSET, 0.6f);
		iTween.RotateTo(base.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 0f, 0f),
			"time",
			0.7f,
			"delay",
			0.6f,
			"easetype",
			iTween.EaseType.easeInOutCubic,
			"islocal",
			true
		}));
		hiddenActor.Show();
		yield return new WaitForSeconds(0.4f);
		iTween.MoveTo(hiddenActor.gameObject, iTween.Hash(new object[]
		{
			"position",
			new Vector3(0f, 3f, 0f),
			"time",
			1f,
			"delay",
			0f,
			"islocal",
			true
		}));
		this.m_doNotSort = false;
		ZonePlay zonePlay = (ZonePlay)this.m_zone;
		zonePlay.SetTransitionTime(1.6f);
		zonePlay.UpdateLayout();
		yield return new WaitForSeconds(0.2f);
		float cardFlipTime = 0.35f;
		iTween.RotateTo(hiddenActor.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 0f, -90f),
			"time",
			cardFlipTime,
			"delay",
			0f,
			"easetype",
			iTween.EaseType.easeInCubic,
			"islocal",
			true
		}));
		yield return new WaitForSeconds(cardFlipTime);
		hiddenActor.Destroy();
		cardFaceActor.Show();
		cardFaceActor.gameObject.transform.localPosition = new Vector3(0f, 3f, 0f);
		cardFaceActor.gameObject.transform.Rotate(new Vector3(0f, 0f, 90f));
		iTween.RotateTo(cardFaceActor.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 0f, 0f),
			"time",
			cardFlipTime,
			"delay",
			0f,
			"easetype",
			iTween.EaseType.easeOutCubic,
			"islocal",
			true
		}));
		this.m_actor.gameObject.transform.localPosition = new Vector3(0f, 2.86f, 0f);
		cardFaceActor.gameObject.transform.localPosition = new Vector3(0f, 2.86f, 0f);
		iTween.MoveTo(hiddenActor.gameObject, iTween.Hash(new object[]
		{
			"position",
			Vector3.zero,
			"time",
			1f,
			"delay",
			0f,
			"islocal",
			true
		}));
		this.ActivateSpell(outSpell, new Spell.FinishedCallback(this.OnSpellFinished_HandToPlay_SummonOut), null, new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
		this.m_actor.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		yield break;
	}

	// Token: 0x0600185C RID: 6236 RVA: 0x00072455 File Offset: 0x00070655
	private void MillCard()
	{
		base.StartCoroutine(this.MillCardWithTiming());
	}

	// Token: 0x0600185D RID: 6237 RVA: 0x00072464 File Offset: 0x00070664
	private IEnumerator MillCardWithTiming()
	{
		Player cardOwner = this.m_entity.GetController();
		bool friendly = cardOwner.IsFriendlySide();
		string boneName;
		if (friendly)
		{
			while (GameState.Get().GetFriendlyCardBeingDrawn())
			{
				yield return null;
			}
			GameState.Get().SetFriendlyCardBeingDrawn(this);
			boneName = "FriendlyMillCard";
		}
		else
		{
			while (GameState.Get().GetOpponentCardBeingDrawn())
			{
				yield return null;
			}
			GameState.Get().SetOpponentCardBeingDrawn(this);
			boneName = "OpponentMillCard";
		}
		int turn = GameState.Get().GetTurn();
		if (turn != GameState.Get().GetLastTurnRemindedOfFullHand() && cardOwner.GetHandZone().GetCardCount() >= 10)
		{
			GameState.Get().SetLastTurnRemindedOfFullHand(turn);
			cardOwner.GetHeroCard().PlayEmote(EmoteType.ERROR_HAND_FULL);
		}
		this.m_actor.Show();
		this.m_actor.TurnOffCollider();
		Transform bone = Board.Get().FindBone(boneName);
		Vector3[] drawPath = new Vector3[]
		{
			base.gameObject.transform.position,
			base.gameObject.transform.position + Card.ABOVE_DECK_OFFSET,
			bone.position
		};
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"path",
			drawPath,
			"time",
			1.5f,
			"easetype",
			iTween.EaseType.easeInSineOutExpo
		}));
		base.gameObject.transform.localEulerAngles = new Vector3(270f, 270f, 0f);
		iTween.RotateTo(base.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 0f, 357f),
			"time",
			1.35f,
			"delay",
			0.15f
		}));
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			bone.localScale,
			"time",
			0.75f,
			"delay",
			0.15f
		}));
		while (iTween.Count(base.gameObject) > 0)
		{
			yield return null;
		}
		this.m_actorReady = true;
		this.RefreshActor();
		Spell handfullSpell = this.m_actor.GetSpell(SpellType.HANDFULL);
		handfullSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
		handfullSpell.Activate();
		GameState.Get().ClearCardBeingDrawn(this);
		yield break;
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x00072480 File Offset: 0x00070680
	private bool ActivateActorSpells_HandToPlay(Actor oldActor)
	{
		if (oldActor == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_HandToPlay() - oldActor=null", this));
			return false;
		}
		if (this.m_cardDef == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_HandToPlay() - m_cardDef=null", this));
			return false;
		}
		if (this.m_actor == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_HandToPlay() - m_actor=null", this));
			return false;
		}
		SpellType spellType = this.m_cardDef.DetermineSummonOutSpell_HandToPlay(this);
		Spell spell = oldActor.GetSpell(spellType);
		if (spell == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_HandToPlay() - outSpell=null outSpellType={1}", this, spellType));
			return false;
		}
		bool flag;
		Spell bestSummonSpell = this.GetBestSummonSpell(out flag);
		if (bestSummonSpell == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_HandToPlay() - inSpell=null standard={1}", this, flag));
			return false;
		}
		this.m_inputEnabled = false;
		this.ActivateSpell(spell, new Spell.FinishedCallback(this.OnSpellFinished_HandToPlay_SummonOut), null, new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
		return true;
	}

	// Token: 0x0600185F RID: 6239 RVA: 0x00072580 File Offset: 0x00070780
	private void OnSpellFinished_HandToPlay_SummonOut(Spell spell, object userData)
	{
		this.m_actor.Show();
		bool flag;
		Spell bestSummonSpell = this.GetBestSummonSpell(out flag);
		if (!flag)
		{
			bestSummonSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroySpell));
			SpellUtils.SetCustomSpellParent(bestSummonSpell, this.m_actor);
		}
		bestSummonSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished_HandToPlay_SummonIn));
		bestSummonSpell.Activate();
	}

	// Token: 0x06001860 RID: 6240 RVA: 0x000725E0 File Offset: 0x000707E0
	private void OnSpellFinished_HandToPlay_SummonIn(Spell spell, object userData)
	{
		this.m_actorReady = true;
		this.m_inputEnabled = true;
		this.ActivateStateSpells();
		this.RefreshActor();
		if ((this.m_entity.HasSpellPower() || this.m_entity.HasSpellPowerDouble()) && this.m_entity.IsControlledByFriendlySidePlayer())
		{
			ZoneHand zoneHand = ZoneMgr.Get().FindZoneOfType<ZoneHand>(this.m_zone.m_Side);
			zoneHand.OnSpellPowerEntityEnteredPlay();
		}
		if (this.m_entity.HasWindfury())
		{
			this.ActivateActorSpell(SpellType.WINDFURY_BURST);
		}
		base.StartCoroutine(this.ActivateActorBattlecrySpell());
		BoardEvents boardEvents = BoardEvents.Get();
		if (boardEvents != null)
		{
			boardEvents.SummonedEvent(this);
		}
	}

	// Token: 0x06001861 RID: 6241 RVA: 0x00072694 File Offset: 0x00070894
	private bool ActivateActorSpells_HandToWeapon(Actor oldActor)
	{
		if (oldActor == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_HandToWeapon() - oldActor=null", this));
			return false;
		}
		if (this.m_actor == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_HandToWeapon() - m_actor=null", this));
			return false;
		}
		SpellType spellType = SpellType.SUMMON_OUT_WEAPON;
		Spell spell = oldActor.GetSpell(spellType);
		if (spell == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_HandToWeapon() - outSpell=null outSpellType={1}", this, spellType));
			return false;
		}
		SpellType spellType2 = (!this.m_entity.IsControlledByFriendlySidePlayer()) ? SpellType.SUMMON_IN_OPPONENT : SpellType.SUMMON_IN_FRIENDLY;
		Spell actorSpell = this.GetActorSpell(spellType2, true);
		if (actorSpell == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_HandToWeapon() - inSpell=null inSpellType={1}", this, spellType2));
			return false;
		}
		this.m_inputEnabled = false;
		this.ActivateSpell(spell, new Spell.FinishedCallback(this.OnSpellFinished_HandToWeapon_SummonOut), actorSpell, new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
		return true;
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x00072780 File Offset: 0x00070980
	private void OnSpellFinished_HandToWeapon_SummonOut(Spell spell, object userData)
	{
		this.m_actor.Show();
		Spell spell2 = (Spell)userData;
		this.ActivateSpell(spell2, new Spell.FinishedCallback(this.OnSpellFinished_StandardCardSummon));
	}

	// Token: 0x06001863 RID: 6243 RVA: 0x000727B4 File Offset: 0x000709B4
	private void DiscardCardBeingDrawn()
	{
		if (this == GameState.Get().GetOpponentCardBeingDrawn())
		{
			this.m_actorWaitingToBeReplaced.Destroy();
			this.m_actorWaitingToBeReplaced = null;
		}
		if (this.m_actor.IsShown())
		{
			this.ActivateDeathSpell(this.m_actor);
		}
		else
		{
			GameState.Get().ClearCardBeingDrawn(this);
		}
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x00072818 File Offset: 0x00070A18
	private void DoDiscardAnimation()
	{
		this.m_doNotSort = true;
		iTween.Stop(base.gameObject);
		float num = 3f;
		if (this.GetEntity().IsControlledByOpposingSidePlayer())
		{
			num = -num;
		}
		Vector3 position;
		position..ctor(base.transform.position.x, base.transform.position.y, base.transform.position.z + num);
		iTween.MoveTo(base.gameObject, position, 3f);
		iTween.ScaleTo(base.gameObject, base.transform.localScale * 1.5f, 3f);
		base.StartCoroutine(this.ActivateGraveyardActorDeathSpellAfterDelay());
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x000728D8 File Offset: 0x00070AD8
	private bool SwitchOutLinkedDrawnCard()
	{
		if (!(this.m_prevZone is ZoneHand))
		{
			return false;
		}
		int tag = this.m_entity.GetTag(GAME_TAG.LINKED_ENTITY);
		Entity entity = GameState.Get().GetEntity(tag);
		if (entity == null)
		{
			GameState.Get().ClearCardBeingDrawn(this);
			return false;
		}
		Card card = entity.GetCard();
		if (card == null)
		{
			GameState.Get().ClearCardBeingDrawn(this);
			return false;
		}
		TransformUtil.CopyWorld(card, this);
		card.m_actorReady = true;
		if (!GameState.Get().IsBeingDrawn(this))
		{
			this.m_doNotSort = false;
			this.DoNullZoneVisuals();
			card.m_actor.Show();
			card.m_doNotSort = false;
			this.m_prevZone.UpdateLayout();
			return true;
		}
		if (this.m_entity.IsControlledByFriendlySidePlayer())
		{
			card.StartCoroutine(card.SwitchOutFriendlyLinkedDrawnCard(this));
		}
		else
		{
			card.StartCoroutine(card.SwitchOutOpponentLinkedDrawnCard(this));
		}
		return true;
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x000729C8 File Offset: 0x00070BC8
	private IEnumerator SwitchOutFriendlyLinkedDrawnCard(Card oldCard)
	{
		oldCard.DoNullZoneVisuals();
		this.m_actor.Show();
		while (this.m_holdingForLinkedCardSwitch)
		{
			yield return null;
		}
		this.m_doNotSort = false;
		GameState.Get().SetFriendlyCardBeingDrawn(null);
		SoundManager.Get().LoadAndPlay("add_card_to_hand_1", base.gameObject);
		this.ActivateStateSpells();
		this.RefreshActor();
		this.m_zone.UpdateLayout();
		yield break;
	}

	// Token: 0x06001867 RID: 6247 RVA: 0x000729F4 File Offset: 0x00070BF4
	private IEnumerator SwitchOutOpponentLinkedDrawnCard(Card oldCard)
	{
		this.m_beingDrawnByOpponent = true;
		Actor handActor = null;
		bool loadingActor = true;
		AssetLoader.GameObjectCallback actorLoadedCallback = delegate(string name, GameObject go, object callbackData)
		{
			loadingActor = false;
			if (go == null)
			{
				Error.AddDevFatal("Card.SwitchOutOpponentLinkedDrawnCard() - failed to load {0}", new object[]
				{
					name
				});
				return;
			}
			handActor = go.GetComponent<Actor>();
			if (handActor == null)
			{
				Error.AddDevFatal("Card.SwitchOutOpponentLinkedDrawnCard() - instance of {0} has no Actor component", new object[]
				{
					name
				});
				return;
			}
		};
		string actorName = ActorNames.GetHandActor(this.m_entity);
		AssetLoader.Get().LoadActor(actorName, actorLoadedCallback, null, false);
		while (loadingActor)
		{
			yield return null;
		}
		oldCard.m_actorWaitingToBeReplaced.Destroy();
		oldCard.m_actorWaitingToBeReplaced = null;
		oldCard.DoNullZoneVisuals();
		if (!handActor)
		{
			this.m_doNotSort = false;
			this.m_beingDrawnByOpponent = false;
			GameState.Get().SetOpponentCardBeingDrawn(null);
			yield break;
		}
		handActor.SetEntity(this.m_entity);
		handActor.SetCardDef(this.m_cardDef);
		handActor.UpdateAllComponents();
		handActor.transform.parent = this.m_actor.transform.parent;
		TransformUtil.CopyLocal(handActor, this.m_actor);
		while (this.m_holdingForLinkedCardSwitch)
		{
			yield return null;
		}
		yield return base.StartCoroutine(this.HideRevealedOpponentCard(handActor));
		yield break;
	}

	// Token: 0x06001868 RID: 6248 RVA: 0x00072A20 File Offset: 0x00070C20
	private bool DoPlayToHandTransition(Actor oldActor, bool wasInGraveyard = false)
	{
		bool flag = this.ActivateActorSpells_PlayToHand(oldActor, wasInGraveyard);
		if (flag)
		{
			this.m_actor.Hide();
		}
		return flag;
	}

	// Token: 0x06001869 RID: 6249 RVA: 0x00072A48 File Offset: 0x00070C48
	private bool ActivateActorSpells_PlayToHand(Actor oldActor, bool wasInGraveyard)
	{
		if (oldActor == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_PlayToHand() - oldActor=null", this));
			return false;
		}
		if (this.m_actor == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_PlayToHand() - m_actor=null", this));
			return false;
		}
		SpellType spellType = SpellType.BOUNCE_OUT;
		Spell spell2 = oldActor.GetSpell(spellType);
		if (spell2 == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_PlayToHand() - outSpell=null outSpellType={1}", this, spellType));
			return false;
		}
		SpellType spellType2 = SpellType.BOUNCE_IN;
		Spell actorSpell = this.GetActorSpell(spellType2, true);
		if (actorSpell == null)
		{
			Debug.LogError(string.Format("{0}.ActivateActorSpells_PlayToHand() - inSpell=null inSpellType={1}", this, spellType2));
			return false;
		}
		this.m_inputEnabled = false;
		if (this.m_entity.IsControlledByFriendlySidePlayer())
		{
			Spell.FinishedCallback finishedCallback = (!wasInGraveyard) ? new Spell.FinishedCallback(this.OnSpellFinished_PlayToHand_SummonOut) : new Spell.FinishedCallback(this.OnSpellFinished_PlayToHand_SummonOut_FromGraveyard);
			this.ActivateSpell(spell2, finishedCallback, actorSpell, new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
		}
		else
		{
			if (this.m_entity.IsControlledByOpposingSidePlayer())
			{
				Log.FaceDownCard.Print("Card.ActivateActorSpells_PlayToHand() - {0} - {1} on {2}", new object[]
				{
					this,
					spellType,
					oldActor
				});
				Log.FaceDownCard.Print("Card.ActivateActorSpells_PlayToHand() - {0} - {1} on {2}", new object[]
				{
					this,
					spellType2,
					this.m_actor
				});
			}
			Spell.FinishedCallback finishedCallback2 = (!wasInGraveyard) ? null : delegate(Spell spell, object userData)
			{
				this.ResumeLayoutForPlayZone();
			};
			this.ActivateSpell(spell2, finishedCallback2, null, new Spell.StateFinishedCallback(this.OnSpellStateFinished_PlayToHand_OldActor_SummonOut));
			this.ActivateSpell(actorSpell, new Spell.FinishedCallback(this.OnSpellFinished_PlayToHand_SummonIn));
		}
		return true;
	}

	// Token: 0x0600186A RID: 6250 RVA: 0x00072BF4 File Offset: 0x00070DF4
	private void OnSpellFinished_PlayToHand_SummonOut(Spell spell, object userData)
	{
		Spell spell2 = (Spell)userData;
		this.ActivateSpell(spell2, new Spell.FinishedCallback(this.OnSpellFinished_StandardCardSummon));
	}

	// Token: 0x0600186B RID: 6251 RVA: 0x00072C1B File Offset: 0x00070E1B
	private void OnSpellFinished_PlayToHand_SummonOut_FromGraveyard(Spell spell, object userData)
	{
		this.OnSpellFinished_PlayToHand_SummonOut(spell, userData);
		this.ResumeLayoutForPlayZone();
	}

	// Token: 0x0600186C RID: 6252 RVA: 0x00072C2C File Offset: 0x00070E2C
	private void ResumeLayoutForPlayZone()
	{
		ZonePlay zonePlay = ZoneMgr.Get().FindZoneOfType<ZonePlay>(this.m_zone.m_Side);
		zonePlay.RemoveLayoutBlocker();
		zonePlay.UpdateLayout();
	}

	// Token: 0x0600186D RID: 6253 RVA: 0x00072C5C File Offset: 0x00070E5C
	private void OnSpellStateFinished_PlayToHand_OldActor_SummonOut(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (this.m_entity.IsControlledByOpposingSidePlayer())
		{
			Log.FaceDownCard.Print("Card.OnSpellStateFinished_PlayToHand_OldActor_SummonOut() - {0} stateType={1}", new object[]
			{
				this,
				spell.GetActiveState()
			});
		}
		this.OnSpellStateFinished_DestroyActor(spell, prevStateType, userData);
	}

	// Token: 0x0600186E RID: 6254 RVA: 0x00072CA9 File Offset: 0x00070EA9
	private void OnSpellFinished_PlayToHand_SummonIn(Spell spell, object userData)
	{
		if (this.m_entity.IsControlledByOpposingSidePlayer())
		{
			Log.FaceDownCard.Print("Card.OnSpellFinished_PlayToHand_SummonIn() - {0}", new object[]
			{
				this
			});
		}
		this.OnSpellFinished_StandardCardSummon(spell, userData);
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x00072CDC File Offset: 0x00070EDC
	private void DoPlayToDeckTransition(Actor playActor)
	{
		this.m_doNotSort = true;
		this.m_actor.Hide();
		base.StartCoroutine(this.AnimatePlayToDeck(playActor));
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x00072D0C File Offset: 0x00070F0C
	private IEnumerator AnimatePlayToDeck(Actor playActor)
	{
		Actor handActor = null;
		bool loadingActor = true;
		string actorName = ActorNames.GetHandActor(this.m_entity);
		AssetLoader.GameObjectCallback actorLoadedCallback = delegate(string name, GameObject go, object callbackData)
		{
			loadingActor = false;
			if (go == null)
			{
				Error.AddDevFatal("Card.AnimatePlayToGraveyardToDeck() - failed to load {0}", new object[]
				{
					name
				});
				return;
			}
			handActor = go.GetComponent<Actor>();
			if (handActor == null)
			{
				Error.AddDevFatal("Card.AnimatePlayToGraveyardToDeck() - instance of {0} has no Actor component", new object[]
				{
					name
				});
				return;
			}
		};
		AssetLoader.Get().LoadActor(actorName, actorLoadedCallback, null, false);
		while (loadingActor)
		{
			yield return null;
		}
		if (handActor == null)
		{
			playActor.Destroy();
			yield break;
		}
		handActor.SetEntity(this.m_entity);
		handActor.SetCardDef(this.m_cardDef);
		handActor.UpdateAllComponents();
		handActor.transform.parent = playActor.transform.parent;
		TransformUtil.Identity(handActor);
		handActor.Hide();
		SpellType outSpellType = SpellType.SUMMON_OUT;
		Spell outSpell = playActor.GetSpell(outSpellType);
		if (outSpell == null)
		{
			Error.AddDevFatal("{0}.AnimatePlayToGraveyardToDeck() - outSpell=null outSpellType={1}", new object[]
			{
				this,
				outSpellType
			});
			yield break;
		}
		SpellType inSpellType = SpellType.SUMMON_IN;
		Spell inSpell = handActor.GetSpell(inSpellType);
		if (inSpell == null)
		{
			Error.AddDevFatal("{0}.AnimatePlayToGraveyardToDeck() - inSpell=null inSpellType={1}", new object[]
			{
				this,
				inSpellType
			});
			yield break;
		}
		bool waitForSpells = true;
		Spell.FinishedCallback inFinishCallback = delegate(Spell spell, object userData)
		{
			waitForSpells = false;
		};
		Spell.StateFinishedCallback outStateFinishCallback = delegate(Spell spell, SpellStateType prevStateType, object userData)
		{
			if (spell.GetActiveState() != SpellStateType.NONE)
			{
				return;
			}
			playActor.Destroy();
		};
		Spell.FinishedCallback outFinishCallback = delegate(Spell spell, object userData)
		{
			inSpell.Activate();
			this.ResumeLayoutForPlayZone();
		};
		inSpell.AddFinishedCallback(inFinishCallback);
		outSpell.AddFinishedCallback(outFinishCallback);
		outSpell.AddStateFinishedCallback(outStateFinishCallback);
		this.PrepareForDeathAnimation(playActor);
		outSpell.Activate();
		while (waitForSpells)
		{
			yield return 0;
		}
		ZoneDeck deckZone = (ZoneDeck)this.m_zone;
		yield return base.StartCoroutine(this.AnimatePlayToDeck(base.gameObject, deckZone, false));
		handActor.Destroy();
		this.m_actorReady = true;
		this.m_doNotSort = false;
		deckZone.RemoveLayoutBlocker();
		deckZone.UpdateLayout();
		yield break;
	}

	// Token: 0x06001871 RID: 6257 RVA: 0x00072D38 File Offset: 0x00070F38
	public IEnumerator AnimatePlayToDeck(GameObject mover, ZoneDeck deckZone, bool hideBackSide = false)
	{
		SoundManager.Get().LoadAndPlay("MinionToDeck_transition");
		GameObject targetThickness = deckZone.GetThicknessForLayout();
		Vector3 finalPos = targetThickness.GetComponent<Renderer>().bounds.center + Card.IN_DECK_OFFSET;
		Vector3 intermedPos = finalPos + Card.ABOVE_DECK_OFFSET;
		Vector3 intermedRot = new Vector3(0f, Card.IN_DECK_ANGLES.y, 0f);
		Vector3 finalRot = Card.IN_DECK_ANGLES;
		Vector3 finalScale = Card.IN_DECK_SCALE;
		float finalRotSec = 0.3f;
		if (hideBackSide)
		{
			intermedRot.y = (finalRot.y = -Card.IN_DECK_ANGLES.y);
			finalRotSec = 0.5f;
		}
		iTween.MoveTo(mover, iTween.Hash(new object[]
		{
			"position",
			intermedPos,
			"delay",
			0f,
			"time",
			0.7f,
			"easetype",
			iTween.EaseType.easeInOutCubic
		}));
		iTween.RotateTo(mover, iTween.Hash(new object[]
		{
			"rotation",
			intermedRot,
			"delay",
			0f,
			"time",
			0.2f,
			"easetype",
			iTween.EaseType.easeInOutCubic
		}));
		iTween.MoveTo(mover, iTween.Hash(new object[]
		{
			"position",
			finalPos,
			"delay",
			0.7f,
			"time",
			0.7f,
			"easetype",
			iTween.EaseType.easeOutCubic
		}));
		iTween.ScaleTo(mover, iTween.Hash(new object[]
		{
			"scale",
			finalScale,
			"delay",
			0.7f,
			"time",
			0.6f,
			"easetype",
			iTween.EaseType.easeInCubic
		}));
		iTween.RotateTo(mover, iTween.Hash(new object[]
		{
			"rotation",
			finalRot,
			"delay",
			0.2f,
			"time",
			finalRotSec,
			"easetype",
			iTween.EaseType.easeOutCubic
		}));
		while (iTween.HasTween(mover))
		{
			yield return 0;
		}
		yield break;
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x00072D76 File Offset: 0x00070F76
	public void SetSecretTriggered(bool set)
	{
		this.m_secretTriggered = set;
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x00072D7F File Offset: 0x00070F7F
	public bool WasSecretTriggered()
	{
		return this.m_secretTriggered;
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x00072D87 File Offset: 0x00070F87
	public bool CanShowSecretTrigger()
	{
		return !UniversalInputManager.UsePhoneUI || this.m_zone.IsOnlyCard(this);
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x00072DB0 File Offset: 0x00070FB0
	public void ShowSecretTrigger()
	{
		Spell component = this.m_actor.GetComponent<Spell>();
		component.ActivateState(SpellStateType.ACTION);
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x00072DD0 File Offset: 0x00070FD0
	private bool CanShowSecret()
	{
		return !UniversalInputManager.UsePhoneUI || this == this.m_zone.GetCardAtIndex(0);
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x00072E00 File Offset: 0x00071000
	private void ShowSecretBirth()
	{
		Spell component = this.m_actor.GetComponent<Spell>();
		if (!this.CanShowSecret())
		{
			Spell.StateFinishedCallback callback = delegate(Spell thisSpell, SpellStateType prevStateType, object userData)
			{
				if (thisSpell.GetActiveState() != SpellStateType.NONE)
				{
					return;
				}
				if (!this.CanShowSecret())
				{
					this.HideCard();
				}
			};
			component.AddStateFinishedCallback(callback);
		}
		component.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x06001878 RID: 6264 RVA: 0x00072E40 File Offset: 0x00071040
	public bool CanShowSecretDeath()
	{
		return !UniversalInputManager.UsePhoneUI || this.m_prevZone.GetCardCount() == 0;
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x00072E74 File Offset: 0x00071074
	public void ShowSecretDeath(Actor oldActor)
	{
		Spell component = oldActor.GetComponent<Spell>();
		if (this.m_secretTriggered)
		{
			this.m_secretTriggered = false;
			if (component.GetActiveState() == SpellStateType.NONE)
			{
				oldActor.Destroy();
			}
			else
			{
				component.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
			}
			return;
		}
		component.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnSpellStateFinished_DestroyActor));
		component.ActivateState(SpellStateType.ACTION);
		oldActor.transform.parent = null;
		if (UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		this.m_doNotSort = true;
		iTween.Stop(base.gameObject);
		this.m_actor.Hide();
		base.StartCoroutine(this.WaitAndThenShowDestroyedSecret());
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x00072F24 File Offset: 0x00071124
	private IEnumerator WaitAndThenShowDestroyedSecret()
	{
		yield return new WaitForSeconds(0.5f);
		float slideAmount = 2f;
		if (this.GetEntity().IsControlledByOpposingSidePlayer())
		{
			slideAmount = -slideAmount;
		}
		Vector3 newPos = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z + slideAmount);
		this.m_actor.Show();
		iTween.MoveTo(base.gameObject, newPos, 3f);
		base.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		base.transform.localEulerAngles = new Vector3(0f, 0f, 357f);
		iTween.ScaleTo(base.gameObject, new Vector3(1.25f, 0.2f, 1.25f), 3f);
		base.StartCoroutine(this.ActivateGraveyardActorDeathSpellAfterDelay());
		yield break;
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x00072F40 File Offset: 0x00071140
	private IEnumerator SwitchSecretSides()
	{
		this.m_doNotSort = true;
		Actor newActor = null;
		bool loadingActor = true;
		AssetLoader.GameObjectCallback actorLoadedCallback = delegate(string name, GameObject go, object callbackData)
		{
			loadingActor = false;
			if (go == null)
			{
				Error.AddDevFatal("Card.SwitchSecretSides() - failed to load {0}", new object[]
				{
					name
				});
				return;
			}
			newActor = go.GetComponent<Actor>();
			if (newActor == null)
			{
				Error.AddDevFatal("Card.SwitchSecretSides() - instance of {0} has no Actor component", new object[]
				{
					name
				});
				return;
			}
		};
		AssetLoader.Get().LoadActor(this.m_actorName, actorLoadedCallback, null, false);
		while (loadingActor)
		{
			yield return null;
		}
		if (newActor)
		{
			Actor oldActor = this.m_actor;
			this.m_actor = newActor;
			this.m_actor.SetEntity(this.m_entity);
			this.m_actor.SetCard(this);
			this.m_actor.SetCardDef(this.m_cardDef);
			this.m_actor.UpdateAllComponents();
			this.m_actor.transform.parent = oldActor.transform.parent;
			TransformUtil.Identity(this.m_actor);
			this.m_actor.Hide();
			if (!this.CanShowSecretDeath())
			{
				oldActor.Destroy();
			}
			else
			{
				oldActor.transform.parent = base.transform.parent;
				this.m_transitionStyle = ZoneTransitionStyle.INSTANT;
				bool oldActorFinished = false;
				Spell.FinishedCallback onOldSpellFinished = delegate(Spell spell, object userData)
				{
					oldActorFinished = true;
				};
				Spell.StateFinishedCallback onOldSpellStateFinished = delegate(Spell spell, SpellStateType prevStateType, object userData)
				{
					if (spell.GetActiveState() == SpellStateType.NONE)
					{
						oldActor.Destroy();
					}
				};
				Spell oldSpell = oldActor.GetComponent<Spell>();
				oldSpell.AddFinishedCallback(onOldSpellFinished);
				oldSpell.AddStateFinishedCallback(onOldSpellStateFinished);
				oldSpell.ActivateState(SpellStateType.ACTION);
				while (!oldActorFinished)
				{
					yield return null;
				}
			}
			this.m_shown = true;
			this.m_actor.Show();
			this.ShowSecretBirth();
		}
		this.m_actorReady = true;
		this.m_doNotSort = false;
		this.m_zone.UpdateLayout();
		this.ActivateStateSpells();
		yield break;
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x00072F5C File Offset: 0x0007115C
	private bool ShouldCardDrawWaitForTurnStartSpells()
	{
		SpellController spellController = TurnStartManager.Get().GetSpellController();
		return !(spellController == null) && (spellController.IsSource(this) || spellController.IsTarget(this));
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x00072FA0 File Offset: 0x000711A0
	private IEnumerator WaitForCardDrawBlockingTurnStartSpells()
	{
		while (this.ShouldCardDrawWaitForTurnStartSpells())
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x00072FBC File Offset: 0x000711BC
	private bool ShouldCardDrawWaitForTaskLists()
	{
		PowerQueue powerQueue = GameState.Get().GetPowerProcessor().GetPowerQueue();
		if (powerQueue.Count == 0)
		{
			return false;
		}
		PowerTaskList powerTaskList = powerQueue.Peek();
		if (this.DoesTaskListBlockCardDraw(powerTaskList))
		{
			return true;
		}
		PowerTaskList parent = powerTaskList.GetParent();
		return this.DoesTaskListBlockCardDraw(parent);
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x00073014 File Offset: 0x00071214
	private IEnumerator WaitForCardDrawBlockingTaskLists()
	{
		PowerQueue powerQueue = GameState.Get().GetPowerProcessor().GetPowerQueue();
		while (powerQueue.Count > 0)
		{
			PowerTaskList taskList = powerQueue.Peek();
			PowerTaskList parentTaskList = taskList.GetParent();
			PowerTaskList blockingTaskList = null;
			if (this.DoesTaskListBlockCardDraw(taskList))
			{
				blockingTaskList = taskList;
			}
			else if (this.DoesTaskListBlockCardDraw(parentTaskList))
			{
				blockingTaskList = parentTaskList;
			}
			if (blockingTaskList == null)
			{
				break;
			}
			while (this.DoesTaskListBlockCardDraw(blockingTaskList))
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x00073030 File Offset: 0x00071230
	private bool DoesTaskListBlockCardDraw(PowerTaskList taskList)
	{
		if (taskList == null)
		{
			return false;
		}
		Network.HistBlockStart blockStart = taskList.GetBlockStart();
		if (blockStart == null)
		{
			return false;
		}
		switch (blockStart.BlockType)
		{
		case 3:
		case 5:
		{
			if (!taskList.IsComplete())
			{
				Entity sourceEntity = taskList.GetSourceEntity();
				if (this.m_entity == sourceEntity)
				{
					return true;
				}
				int entityId = this.m_entity.GetEntityId();
				List<PowerTask> taskList2 = taskList.GetTaskList();
				for (int i = 0; i < taskList2.Count; i++)
				{
					PowerTask powerTask = taskList2[i];
					Network.PowerHistory power = powerTask.GetPower();
					int num = 0;
					switch (power.Type)
					{
					case Network.PowerType.SHOW_ENTITY:
					{
						Network.HistShowEntity histShowEntity = (Network.HistShowEntity)power;
						if (histShowEntity.Entity.ID == entityId)
						{
							Network.Entity.Tag tag = histShowEntity.Entity.Tags.Find((Network.Entity.Tag currTag) => currTag.Name == 49);
							if (tag != null)
							{
								num = tag.Value;
							}
						}
						break;
					}
					case Network.PowerType.HIDE_ENTITY:
					{
						Network.HistHideEntity histHideEntity = (Network.HistHideEntity)power;
						if (histHideEntity.Entity == entityId)
						{
							num = histHideEntity.Zone;
						}
						break;
					}
					case Network.PowerType.TAG_CHANGE:
					{
						Network.HistTagChange histTagChange = (Network.HistTagChange)power;
						if (histTagChange.Entity == entityId && histTagChange.Tag == 49)
						{
							num = histTagChange.Value;
						}
						break;
					}
					}
					if (num != 0 && num != 3)
					{
						return true;
					}
				}
			}
			PowerTaskList next = taskList.GetNext();
			return this.DoesTaskListBlockCardDraw(next);
		}
		}
		return false;
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x000731E4 File Offset: 0x000713E4
	private void CutoffFriendlyCardDraw()
	{
		if (this.m_actorReady)
		{
			return;
		}
		if (this.m_actorWaitingToBeReplaced != null)
		{
			this.m_actorWaitingToBeReplaced.Destroy();
			this.m_actorWaitingToBeReplaced = null;
		}
		this.m_actor.Show();
		this.m_actor.TurnOffCollider();
		this.m_doNotSort = false;
		this.m_actorReady = true;
		this.ActivateStateSpells();
		this.RefreshActor();
		GameState.Get().ClearCardBeingDrawn(this);
		this.m_zone.UpdateLayout();
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x00073268 File Offset: 0x00071468
	private IEnumerator WaitAndPrepareForDeathAnimation(Actor dyingActor)
	{
		yield return new WaitForSeconds(this.m_keywordDeathDelaySec);
		this.PrepareForDeathAnimation(dyingActor);
		yield break;
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x00073294 File Offset: 0x00071494
	private void PrepareForDeathAnimation(Actor dyingActor)
	{
		dyingActor.ToggleCollider(false);
		dyingActor.ToggleForceIdle(true);
		dyingActor.SetActorState(ActorStateType.CARD_IDLE);
		dyingActor.DeactivateAllPreDeathSpells();
		this.DeactivateCustomKeywordEffect();
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x000732C4 File Offset: 0x000714C4
	private IEnumerator ActivateGraveyardActorDeathSpellAfterDelay()
	{
		this.m_actor.DeactivateAllPreDeathSpells();
		yield return new WaitForSeconds(1f);
		this.ActivateActorSpell(SpellType.DEATH);
		yield return new WaitForSeconds(4f);
		this.m_doNotSort = false;
		yield break;
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x000732E0 File Offset: 0x000714E0
	private bool HandlePlayActorDeath(Actor oldActor)
	{
		bool result = false;
		if (!this.m_cardDef.m_SuppressDeathrattleDeath && this.m_entity.HasDeathrattle())
		{
			this.ActivateActorSpell(oldActor, SpellType.DEATHRATTLE_DEATH);
		}
		if (this.m_suppressDeathEffects)
		{
			if (oldActor)
			{
				oldActor.Destroy();
			}
			if (this.IsShown())
			{
				this.ShowImpl();
			}
			else
			{
				this.HideImpl();
			}
			result = true;
			this.m_actorReady = true;
		}
		else
		{
			if (!this.m_suppressKeywordDeaths)
			{
				base.StartCoroutine(this.WaitAndPrepareForDeathAnimation(oldActor));
			}
			Spell spell = this.ActivateDeathSpell(oldActor);
			if (spell != null)
			{
				this.m_actor.Hide();
				result = true;
				this.m_actorReady = true;
			}
		}
		return result;
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x000733A4 File Offset: 0x000715A4
	private bool DoesCardReturnFromGraveyard()
	{
		PowerQueue powerQueue = GameState.Get().GetPowerProcessor().GetPowerQueue();
		foreach (PowerTaskList taskList in powerQueue)
		{
			if (this.DoesTaskListReturnCardFromGraveyard(taskList))
			{
				Log.JMac.PrintWarning("Found the task for returning entity {0} from graveyard!", new object[]
				{
					this.m_entity
				});
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x00073434 File Offset: 0x00071634
	private bool DoesTaskListReturnCardFromGraveyard(PowerTaskList taskList)
	{
		if (!taskList.IsTriggerBlock())
		{
			return false;
		}
		List<PowerTask> taskList2 = taskList.GetTaskList();
		foreach (PowerTask powerTask in taskList2)
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = power as Network.HistTagChange;
				if (histTagChange.Tag == 49)
				{
					if (histTagChange.Entity == this.m_entity.GetEntityId())
					{
						if (histTagChange.Value == 6)
						{
							return false;
						}
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06001888 RID: 6280 RVA: 0x00073504 File Offset: 0x00071704
	private void DoNullZoneVisuals()
	{
		this.HideCard();
	}

	// Token: 0x04000C7E RID: 3198
	public const float DEFAULT_KEYWORD_DEATH_DELAY_SEC = 0.6f;

	// Token: 0x04000C7F RID: 3199
	public static readonly Vector3 ABOVE_DECK_OFFSET = new Vector3(0f, 3.6f, 0f);

	// Token: 0x04000C80 RID: 3200
	public static readonly Vector3 IN_DECK_OFFSET = new Vector3(0f, 0f, 0.1f);

	// Token: 0x04000C81 RID: 3201
	public static readonly Vector3 IN_DECK_SCALE = new Vector3(0.81f, 0.81f, 0.81f);

	// Token: 0x04000C82 RID: 3202
	public static readonly Vector3 IN_DECK_ANGLES = new Vector3(-90f, 270f, 0f);

	// Token: 0x04000C83 RID: 3203
	public static readonly Quaternion IN_DECK_ROTATION = Quaternion.Euler(Card.IN_DECK_ANGLES);

	// Token: 0x04000C84 RID: 3204
	public static readonly Vector3 IN_DECK_HIDDEN_ANGLES = new Vector3(270f, 90f, 0f);

	// Token: 0x04000C85 RID: 3205
	public static readonly Quaternion IN_DECK_HIDDEN_ROTATION = Quaternion.Euler(Card.IN_DECK_HIDDEN_ANGLES);

	// Token: 0x04000C86 RID: 3206
	protected Entity m_entity;

	// Token: 0x04000C87 RID: 3207
	protected CardDef m_cardDef;

	// Token: 0x04000C88 RID: 3208
	protected CardEffect m_playEffect;

	// Token: 0x04000C89 RID: 3209
	protected CardEffect m_attackEffect;

	// Token: 0x04000C8A RID: 3210
	protected CardEffect m_deathEffect;

	// Token: 0x04000C8B RID: 3211
	protected CardEffect m_lifetimeEffect;

	// Token: 0x04000C8C RID: 3212
	protected List<CardEffect> m_subOptionEffects;

	// Token: 0x04000C8D RID: 3213
	protected List<CardEffect> m_triggerEffects;

	// Token: 0x04000C8E RID: 3214
	protected List<CardEffect> m_allEffects;

	// Token: 0x04000C8F RID: 3215
	protected CardEffect m_customKeywordSpell;

	// Token: 0x04000C90 RID: 3216
	protected CardAudio m_announcerLine;

	// Token: 0x04000C91 RID: 3217
	protected List<EmoteEntry> m_emotes;

	// Token: 0x04000C92 RID: 3218
	protected Spell m_customSummonSpell;

	// Token: 0x04000C93 RID: 3219
	protected Spell m_customSpawnSpell;

	// Token: 0x04000C94 RID: 3220
	protected Spell m_customSpawnSpellOverride;

	// Token: 0x04000C95 RID: 3221
	protected Spell m_customDeathSpell;

	// Token: 0x04000C96 RID: 3222
	protected Spell m_customDeathSpellOverride;

	// Token: 0x04000C97 RID: 3223
	private int m_spellLoadCount;

	// Token: 0x04000C98 RID: 3224
	protected string m_actorName;

	// Token: 0x04000C99 RID: 3225
	protected Actor m_actor;

	// Token: 0x04000C9A RID: 3226
	protected Actor m_actorWaitingToBeReplaced;

	// Token: 0x04000C9B RID: 3227
	private bool m_actorReady = true;

	// Token: 0x04000C9C RID: 3228
	private bool m_actorLoading;

	// Token: 0x04000C9D RID: 3229
	private bool m_transitioningZones;

	// Token: 0x04000C9E RID: 3230
	private bool m_hasBeenGrabbedByEnemyActionHandler;

	// Token: 0x04000C9F RID: 3231
	private Zone m_zone;

	// Token: 0x04000CA0 RID: 3232
	private Zone m_prevZone;

	// Token: 0x04000CA1 RID: 3233
	private int m_zonePosition;

	// Token: 0x04000CA2 RID: 3234
	private int m_predictedZonePosition;

	// Token: 0x04000CA3 RID: 3235
	private bool m_doNotSort;

	// Token: 0x04000CA4 RID: 3236
	private bool m_beingDrawnByOpponent;

	// Token: 0x04000CA5 RID: 3237
	private bool m_cardStandInInteractive = true;

	// Token: 0x04000CA6 RID: 3238
	private ZoneTransitionStyle m_transitionStyle;

	// Token: 0x04000CA7 RID: 3239
	private bool m_doNotWarpToNewZone;

	// Token: 0x04000CA8 RID: 3240
	private float m_transitionDelay;

	// Token: 0x04000CA9 RID: 3241
	private bool m_holdingForLinkedCardSwitch;

	// Token: 0x04000CAA RID: 3242
	protected bool m_shouldShowTooltip;

	// Token: 0x04000CAB RID: 3243
	protected bool m_showTooltip;

	// Token: 0x04000CAC RID: 3244
	protected bool m_overPlayfield;

	// Token: 0x04000CAD RID: 3245
	protected bool m_mousedOver;

	// Token: 0x04000CAE RID: 3246
	protected bool m_mousedOverByOpponent;

	// Token: 0x04000CAF RID: 3247
	protected bool m_shown = true;

	// Token: 0x04000CB0 RID: 3248
	private bool m_inputEnabled = true;

	// Token: 0x04000CB1 RID: 3249
	protected bool m_attacking;

	// Token: 0x04000CB2 RID: 3250
	private int m_activeDeathEffectCount;

	// Token: 0x04000CB3 RID: 3251
	private bool m_ignoreDeath;

	// Token: 0x04000CB4 RID: 3252
	private bool m_suppressDeathEffects;

	// Token: 0x04000CB5 RID: 3253
	private bool m_suppressDeathSounds;

	// Token: 0x04000CB6 RID: 3254
	private bool m_suppressKeywordDeaths;

	// Token: 0x04000CB7 RID: 3255
	private float m_keywordDeathDelaySec = 0.6f;

	// Token: 0x04000CB8 RID: 3256
	private bool m_suppressActorTriggerSpell;

	// Token: 0x04000CB9 RID: 3257
	private bool m_suppressPlaySounds;

	// Token: 0x04000CBA RID: 3258
	private bool m_isBattleCrySource;

	// Token: 0x04000CBB RID: 3259
	private bool m_secretTriggered;

	// Token: 0x04000CBC RID: 3260
	private bool m_secretSheathed;

	// Token: 0x02000827 RID: 2087
	private class SpellLoadRequest
	{
		// Token: 0x04003711 RID: 14097
		public string m_path;

		// Token: 0x04003712 RID: 14098
		public AssetLoader.GameObjectCallback m_loadCallback;
	}
}
