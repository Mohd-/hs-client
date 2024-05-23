using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class Entity : EntityBase
{
	// Token: 0x0600188E RID: 6286 RVA: 0x0007358F File Offset: 0x0007178F
	public override string ToString()
	{
		return this.GetDebugName();
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x00073598 File Offset: 0x00071798
	public virtual void OnRealTimeFullEntity(Network.HistFullEntity fullEntity)
	{
		base.SetTags(fullEntity.Entity.Tags);
		this.InitRealTimeValues(fullEntity.Entity);
		if (!base.IsEnchantment())
		{
			this.InitCard();
		}
		this.LoadEntityDef(fullEntity.Entity.CardID);
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x000735E8 File Offset: 0x000717E8
	public void OnFullEntity(Network.HistFullEntity fullEntity)
	{
		this.m_loadState = Entity.LoadState.PENDING;
		this.LoadCard(fullEntity.Entity.CardID);
		int tag = base.GetTag(GAME_TAG.ATTACHED);
		if (tag != 0)
		{
			Entity entity = GameState.Get().GetEntity(tag);
			entity.AddAttachment(this);
		}
		int tag2 = base.GetTag(GAME_TAG.PARENT_CARD);
		if (tag2 != 0)
		{
			Entity entity2 = GameState.Get().GetEntity(tag2);
			entity2.AddSubCard(this);
		}
		if (base.GetZone() == TAG_ZONE.PLAY)
		{
			if (base.IsHero())
			{
				Player controller = this.GetController();
				controller.SetHero(this);
			}
			else if (base.IsHeroPower())
			{
				Player controller2 = this.GetController();
				controller2.SetHeroPower(this);
			}
		}
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x0007369B File Offset: 0x0007189B
	public virtual void OnRealTimeShowEntity(Network.HistShowEntity showEntity)
	{
		this.HandleRealTimeEntityChange(showEntity.Entity);
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x000736A9 File Offset: 0x000718A9
	public void OnShowEntity(Network.HistShowEntity showEntity)
	{
		this.HandleEntityChange(showEntity.Entity);
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x000736B7 File Offset: 0x000718B7
	public virtual void OnRealTimeChangeEntity(Network.HistChangeEntity changeEntity)
	{
		this.HandleRealTimeEntityChange(changeEntity.Entity);
	}

	// Token: 0x06001894 RID: 6292 RVA: 0x000736C5 File Offset: 0x000718C5
	public void OnChangeEntity(Network.HistChangeEntity changeEntity)
	{
		this.HandleEntityChange(changeEntity.Entity);
	}

	// Token: 0x06001895 RID: 6293 RVA: 0x000736D4 File Offset: 0x000718D4
	public virtual void OnRealTimeTagChanged(Network.HistTagChange change)
	{
		GAME_TAG tag = (GAME_TAG)change.Tag;
		switch (tag)
		{
		case GAME_TAG.DAMAGE:
			this.SetRealTimeDamage(change.Value);
			return;
		case GAME_TAG.HEALTH:
			break;
		default:
			if (tag != GAME_TAG.DURABILITY)
			{
				if (tag == GAME_TAG.ARMOR)
				{
					this.SetRealTimeArmor(change.Value);
					return;
				}
				if (tag != GAME_TAG.POWERED_UP)
				{
					return;
				}
				this.SetRealTimePoweredUp(change.Value);
				return;
			}
			break;
		case GAME_TAG.ATK:
			this.SetRealTimeAttack(change.Value);
			return;
		case GAME_TAG.COST:
			this.SetRealTimeCost(change.Value);
			return;
		}
		this.SetRealTimeHealth(change.Value);
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x00073798 File Offset: 0x00071998
	public virtual void OnTagsChanged(TagDeltaSet changeSet)
	{
		bool flag = false;
		for (int i = 0; i < changeSet.Size(); i++)
		{
			TagDelta change = changeSet[i];
			if (this.IsNameChange(change))
			{
				flag = true;
			}
			this.HandleTagChange(change);
		}
		if (this.m_card == null)
		{
			return;
		}
		if (flag)
		{
			this.UpdateCardName();
		}
		this.m_card.OnTagsChanged(changeSet);
	}

	// Token: 0x06001897 RID: 6295 RVA: 0x00073808 File Offset: 0x00071A08
	public virtual void OnTagChanged(TagDelta change)
	{
		this.HandleTagChange(change);
		if (this.m_card == null)
		{
			return;
		}
		if (this.IsNameChange(change))
		{
			this.UpdateCardName();
		}
		this.m_card.OnTagChanged(change);
	}

	// Token: 0x06001898 RID: 6296 RVA: 0x0007384C File Offset: 0x00071A4C
	public virtual void OnMetaData(Network.HistMetaData metaData)
	{
		if (this.m_card == null)
		{
			return;
		}
		this.m_card.OnMetaData(metaData);
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x0007386C File Offset: 0x00071A6C
	private void HandleRealTimeEntityChange(Network.Entity netEntity)
	{
		this.InitRealTimeValues(netEntity);
		if (base.IsEnchantment() && this.m_card)
		{
			this.m_card.Destroy();
		}
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x0007389C File Offset: 0x00071A9C
	private void HandleEntityChange(Network.Entity netEntity)
	{
		TagDeltaSet changeSet = this.m_tags.CreateDeltas(netEntity.Tags);
		base.SetTags(netEntity.Tags);
		this.LoadCard(netEntity.CardID);
		this.OnTagsChanged(changeSet);
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x000738DC File Offset: 0x00071ADC
	private void HandleTagChange(TagDelta change)
	{
		GAME_TAG tag = (GAME_TAG)change.tag;
		if (tag != GAME_TAG.ATTACHED)
		{
			if (tag != GAME_TAG.ZONE)
			{
				if (tag == GAME_TAG.PARENT_CARD)
				{
					Entity entity = GameState.Get().GetEntity(change.oldValue);
					if (entity != null)
					{
						entity.RemoveSubCard(this);
					}
					Entity entity2 = GameState.Get().GetEntity(change.newValue);
					if (entity2 != null)
					{
						entity2.AddSubCard(this);
					}
				}
			}
			else
			{
				this.UpdateUseBattlecryFlag(false);
				if (GameState.Get().IsTurnStartManagerActive() && change.oldValue == 2 && change.newValue == 3)
				{
					PowerTaskList currentTaskList = GameState.Get().GetPowerProcessor().GetCurrentTaskList();
					if (currentTaskList != null)
					{
						Entity sourceEntity = currentTaskList.GetSourceEntity();
						if (sourceEntity == GameState.Get().GetFriendlySidePlayer())
						{
							TurnStartManager.Get().NotifyOfCardDrawn(this);
						}
					}
				}
				if (change.newValue == 1)
				{
					if (base.IsHero())
					{
						Player controller = this.GetController();
						controller.SetHero(this);
					}
					else if (base.IsHeroPower())
					{
						Player controller2 = this.GetController();
						controller2.SetHeroPower(this);
					}
				}
			}
		}
		else
		{
			Entity entity3 = GameState.Get().GetEntity(change.oldValue);
			if (entity3 != null)
			{
				entity3.RemoveAttachment(this);
			}
			Entity entity4 = GameState.Get().GetEntity(change.newValue);
			if (entity4 != null)
			{
				entity4.AddAttachment(this);
			}
		}
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x00073A50 File Offset: 0x00071C50
	private bool IsNameChange(TagDelta change)
	{
		GAME_TAG tag = (GAME_TAG)change.tag;
		return tag == GAME_TAG.ZONE || tag == GAME_TAG.ENTITY_ID || tag == GAME_TAG.ZONE_POSITION;
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x00073A87 File Offset: 0x00071C87
	public EntityDef GetEntityDef()
	{
		return this.m_entityDef;
	}

	// Token: 0x0600189E RID: 6302 RVA: 0x00073A90 File Offset: 0x00071C90
	public Card InitCard()
	{
		GameObject gameObject = new GameObject();
		this.m_card = gameObject.AddComponent<Card>();
		this.m_card.SetEntity(this);
		this.UpdateCardName();
		return this.m_card;
	}

	// Token: 0x0600189F RID: 6303 RVA: 0x00073AC7 File Offset: 0x00071CC7
	public Card GetCard()
	{
		return this.m_card;
	}

	// Token: 0x060018A0 RID: 6304 RVA: 0x00073ACF File Offset: 0x00071CCF
	public void SetCard(Card card)
	{
		this.m_card = card;
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x00073AD8 File Offset: 0x00071CD8
	public string GetCardId()
	{
		return this.m_cardId;
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x00073AE0 File Offset: 0x00071CE0
	public void SetCardId(string cardId)
	{
		this.m_cardId = cardId;
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x00073AE9 File Offset: 0x00071CE9
	public void Destroy()
	{
		if (this.m_card != null)
		{
			this.m_card.Destroy();
		}
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x00073B07 File Offset: 0x00071D07
	public Entity.LoadState GetLoadState()
	{
		return this.m_loadState;
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x00073B0F File Offset: 0x00071D0F
	public bool IsLoadingAssets()
	{
		return this.m_loadState == Entity.LoadState.LOADING;
	}

	// Token: 0x060018A6 RID: 6310 RVA: 0x00073B1A File Offset: 0x00071D1A
	public bool IsBusy()
	{
		return this.IsLoadingAssets() || (this.m_card != null && !this.m_card.IsActorReady());
	}

	// Token: 0x060018A7 RID: 6311 RVA: 0x00073B4D File Offset: 0x00071D4D
	public bool IsHidden()
	{
		return string.IsNullOrEmpty(this.m_cardId);
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x00073B5A File Offset: 0x00071D5A
	public void ClearTags()
	{
		this.m_tags.Clear();
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x00073B67 File Offset: 0x00071D67
	public void SetTagAndHandleChange<TagEnum>(GAME_TAG tag, TagEnum tagValue)
	{
		this.SetTagAndHandleChange((int)tag, Convert.ToInt32(tagValue));
	}

	// Token: 0x060018AA RID: 6314 RVA: 0x00073B7C File Offset: 0x00071D7C
	public TagDelta SetTagAndHandleChange(int tag, int tagValue)
	{
		int tag2 = this.m_tags.GetTag(tag);
		base.SetTag(tag, tagValue);
		TagDelta tagDelta = new TagDelta();
		tagDelta.tag = tag;
		tagDelta.oldValue = tag2;
		tagDelta.newValue = tagValue;
		this.OnTagChanged(tagDelta);
		return tagDelta;
	}

	// Token: 0x060018AB RID: 6315 RVA: 0x00073BC4 File Offset: 0x00071DC4
	public void SetTagsAndHandleChanges(Map<GAME_TAG, int> tagMap)
	{
		TagDeltaSet changeSet = this.m_tags.CreateDeltas(tagMap);
		this.m_tags.SetTags(tagMap);
		this.OnTagsChanged(changeSet);
	}

	// Token: 0x060018AC RID: 6316 RVA: 0x00073BF1 File Offset: 0x00071DF1
	public override int GetReferencedTag(int tag)
	{
		return this.m_entityDef.GetReferencedTag(tag);
	}

	// Token: 0x060018AD RID: 6317 RVA: 0x00073BFF File Offset: 0x00071DFF
	public override string GetStringTag(int tag)
	{
		return this.m_entityDef.GetStringTag(tag);
	}

	// Token: 0x060018AE RID: 6318 RVA: 0x00073C0D File Offset: 0x00071E0D
	public int GetOriginalCost()
	{
		return this.m_entityDef.GetCost();
	}

	// Token: 0x060018AF RID: 6319 RVA: 0x00073C1A File Offset: 0x00071E1A
	public int GetOriginalATK()
	{
		return this.m_entityDef.GetATK();
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x00073C27 File Offset: 0x00071E27
	public int GetOriginalHealth()
	{
		return this.m_entityDef.GetHealth();
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x00073C34 File Offset: 0x00071E34
	public int GetOriginalDurability()
	{
		return this.m_entityDef.GetDurability();
	}

	// Token: 0x060018B2 RID: 6322 RVA: 0x00073C41 File Offset: 0x00071E41
	public bool GetOriginalCharge()
	{
		return this.m_entityDef.HasTag(GAME_TAG.CHARGE);
	}

	// Token: 0x060018B3 RID: 6323 RVA: 0x00073C53 File Offset: 0x00071E53
	public TAG_RACE GetRace()
	{
		return this.m_entityDef.GetRace();
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x00073C60 File Offset: 0x00071E60
	public TAG_CLASS GetClass()
	{
		if (base.IsSecret())
		{
			return base.GetTag<TAG_CLASS>(GAME_TAG.CLASS);
		}
		return this.m_entityDef.GetClass();
	}

	// Token: 0x060018B5 RID: 6325 RVA: 0x00073C84 File Offset: 0x00071E84
	public TAG_ENCHANTMENT_VISUAL GetEnchantmentBirthVisual()
	{
		return this.m_entityDef.GetEnchantmentBirthVisual();
	}

	// Token: 0x060018B6 RID: 6326 RVA: 0x00073C91 File Offset: 0x00071E91
	public TAG_ENCHANTMENT_VISUAL GetEnchantmentIdleVisual()
	{
		return this.m_entityDef.GetEnchantmentIdleVisual();
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x00073C9E File Offset: 0x00071E9E
	public TAG_RARITY GetRarity()
	{
		return this.m_entityDef.GetRarity();
	}

	// Token: 0x060018B8 RID: 6328 RVA: 0x00073CAB File Offset: 0x00071EAB
	public new TAG_CARD_SET GetCardSet()
	{
		return this.m_entityDef.GetCardSet();
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x00073CB8 File Offset: 0x00071EB8
	public TAG_PREMIUM GetPremiumType()
	{
		return (TAG_PREMIUM)base.GetTag(GAME_TAG.PREMIUM);
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x00073CC4 File Offset: 0x00071EC4
	public bool CanBeDamaged()
	{
		return !base.HasDivineShield() && !base.IsImmune() && !base.HasTag(GAME_TAG.CANT_BE_DAMAGED);
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x00073CF8 File Offset: 0x00071EF8
	public int GetDefense()
	{
		return base.GetHealth() + base.GetArmor();
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x00073D07 File Offset: 0x00071F07
	public int GetRemainingHealth()
	{
		return this.GetDefense() - base.GetDamage();
	}

	// Token: 0x060018BD RID: 6333 RVA: 0x00073D16 File Offset: 0x00071F16
	public Player GetController()
	{
		return GameState.Get().GetPlayer(base.GetControllerId());
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x00073D28 File Offset: 0x00071F28
	public Player.Side GetControllerSide()
	{
		Player controller = this.GetController();
		if (controller == null)
		{
			return Player.Side.NEUTRAL;
		}
		return controller.GetSide();
	}

	// Token: 0x060018BF RID: 6335 RVA: 0x00073D4A File Offset: 0x00071F4A
	public bool IsControlledByLocalUser()
	{
		return this.GetController().IsLocalUser();
	}

	// Token: 0x060018C0 RID: 6336 RVA: 0x00073D57 File Offset: 0x00071F57
	public bool IsControlledByFriendlySidePlayer()
	{
		return this.GetController().IsFriendlySide();
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x00073D64 File Offset: 0x00071F64
	public bool IsControlledByOpposingSidePlayer()
	{
		return this.GetController().IsOpposingSide();
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x00073D71 File Offset: 0x00071F71
	public bool IsControlledByRevealedPlayer()
	{
		return this.GetController().IsRevealed();
	}

	// Token: 0x060018C3 RID: 6339 RVA: 0x00073D7E File Offset: 0x00071F7E
	public bool IsControlledByConcealedPlayer()
	{
		return !this.IsControlledByRevealedPlayer();
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x00073D89 File Offset: 0x00071F89
	public Entity GetCreator()
	{
		return GameState.Get().GetEntity(base.GetCreatorId());
	}

	// Token: 0x060018C5 RID: 6341 RVA: 0x00073D9B File Offset: 0x00071F9B
	public Entity GetDisplayedCreator()
	{
		return GameState.Get().GetEntity(base.GetDisplayedCreatorId());
	}

	// Token: 0x060018C6 RID: 6342 RVA: 0x00073DB0 File Offset: 0x00071FB0
	public virtual Entity GetHero()
	{
		if (base.IsHero())
		{
			return this;
		}
		Player controller = this.GetController();
		if (controller == null)
		{
			return null;
		}
		return controller.GetHero();
	}

	// Token: 0x060018C7 RID: 6343 RVA: 0x00073DE0 File Offset: 0x00071FE0
	public virtual Card GetHeroCard()
	{
		if (base.IsHero())
		{
			return this.GetCard();
		}
		Player controller = this.GetController();
		if (controller == null)
		{
			return null;
		}
		return controller.GetHeroCard();
	}

	// Token: 0x060018C8 RID: 6344 RVA: 0x00073E14 File Offset: 0x00072014
	public virtual Entity GetHeroPower()
	{
		if (base.IsHeroPower())
		{
			return this;
		}
		Player controller = this.GetController();
		if (controller == null)
		{
			return null;
		}
		return controller.GetHeroPower();
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x00073E44 File Offset: 0x00072044
	public virtual Card GetHeroPowerCard()
	{
		if (base.IsHeroPower())
		{
			return this.GetCard();
		}
		Player controller = this.GetController();
		if (controller == null)
		{
			return null;
		}
		return controller.GetHeroPowerCard();
	}

	// Token: 0x060018CA RID: 6346 RVA: 0x00073E78 File Offset: 0x00072078
	public virtual Card GetWeaponCard()
	{
		if (base.IsWeapon())
		{
			return this.GetCard();
		}
		Player controller = this.GetController();
		if (controller == null)
		{
			return null;
		}
		return controller.GetWeaponCard();
	}

	// Token: 0x060018CB RID: 6347 RVA: 0x00073EAC File Offset: 0x000720AC
	public virtual string GetName()
	{
		string stringTag = base.GetStringTag(GAME_TAG.CARDNAME);
		if (stringTag != null)
		{
			return stringTag;
		}
		return this.GetDebugName();
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x00073ED4 File Offset: 0x000720D4
	public virtual string GetDebugName()
	{
		string stringTag = base.GetStringTag(GAME_TAG.CARDNAME);
		if (stringTag != null)
		{
			return string.Format("[name={0} id={1} zone={2} zonePos={3} cardId={4} player={5}]", new object[]
			{
				stringTag,
				base.GetEntityId(),
				base.GetZone(),
				base.GetZonePosition(),
				this.m_cardId,
				base.GetControllerId()
			});
		}
		if (this.m_cardId != null)
		{
			return string.Format("[id={0} cardId={1} type={2} zone={3} zonePos={4} player={5}]", new object[]
			{
				base.GetEntityId(),
				this.m_cardId,
				base.GetCardType(),
				base.GetZone(),
				base.GetZonePosition(),
				base.GetControllerId()
			});
		}
		return string.Format("UNKNOWN ENTITY [id={0} type={1} zone={2} zonePos={3}]", new object[]
		{
			base.GetEntityId(),
			base.GetCardType(),
			base.GetZone(),
			base.GetZonePosition()
		});
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x00073FFC File Offset: 0x000721FC
	public void UpdateCardName()
	{
		if (this.m_card == null)
		{
			return;
		}
		string stringTag = base.GetStringTag(GAME_TAG.CARDNAME);
		if (stringTag != null)
		{
			if (string.IsNullOrEmpty(this.m_cardId))
			{
				this.m_card.gameObject.name = string.Format("{0} [id={1} zone={2} zonePos={3}]", new object[]
				{
					stringTag,
					base.GetEntityId(),
					base.GetZone(),
					base.GetZonePosition()
				});
			}
			else
			{
				this.m_card.gameObject.name = string.Format("{0} [id={1} cardId={2} zone={3} zonePos={4}]", new object[]
				{
					stringTag,
					base.GetEntityId(),
					this.GetCardId(),
					base.GetZone(),
					base.GetZonePosition()
				});
			}
		}
		else
		{
			this.m_card.gameObject.name = string.Format("Hidden Entity [id={0} zone={1} zonePos={2}]", base.GetEntityId(), base.GetZone(), base.GetZonePosition());
		}
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x00074128 File Offset: 0x00072328
	public string GetCardTextInHand()
	{
		if (base.IsEnchantment() && base.HasTag(GAME_TAG.COPY_DEATHRATTLE))
		{
			int tag = base.GetTag(GAME_TAG.COPY_DEATHRATTLE);
			EntityDef entityDef = DefLoader.Get().GetEntityDef(tag);
			if (entityDef != null)
			{
				string powersText = GameStrings.Format("GAMEPLAY_COPY_DEATHRATTLE", new object[]
				{
					entityDef.GetName()
				});
				return TextUtils.TransformCardText(this.GetDamageBonus(), this.GetDamageBonusDouble(), this.GetHealingDouble(), powersText);
			}
		}
		return TextUtils.TransformCardText(this, GAME_TAG.CARDTEXT_INHAND);
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x000741A8 File Offset: 0x000723A8
	public string GetCardTextInHistory()
	{
		int historyDamageBonus = this.GetHistoryDamageBonus();
		int historyDamageBonusDouble = this.GetHistoryDamageBonusDouble();
		int historyHealingDouble = this.GetHistoryHealingDouble();
		return TextUtils.TransformCardText(historyDamageBonus, historyDamageBonusDouble, historyHealingDouble, base.GetStringTag(GAME_TAG.CARDTEXT_INHAND));
	}

	// Token: 0x060018D0 RID: 6352 RVA: 0x000741DD File Offset: 0x000723DD
	public string GetTargetingArrowText()
	{
		return TextUtils.TransformCardText(this, GAME_TAG.TARGETING_ARROW_TEXT);
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x000741EA File Offset: 0x000723EA
	public string GetRaceText()
	{
		return this.m_entityDef.GetRaceText();
	}

	// Token: 0x060018D2 RID: 6354 RVA: 0x000741F8 File Offset: 0x000723F8
	public void AddAttachment(Entity entity)
	{
		int count = this.m_attachments.Count;
		if (this.m_attachments.Contains(entity))
		{
			Log.Mike.Print(string.Format("Entity.AddAttachment() - {0} is already an attachment of {1}", entity, this), new object[0]);
			return;
		}
		this.m_attachments.Add(entity);
		if (this.m_card == null)
		{
			return;
		}
		this.m_card.OnEnchantmentAdded(count, entity);
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x0007426C File Offset: 0x0007246C
	public void RemoveAttachment(Entity entity)
	{
		int count = this.m_attachments.Count;
		if (!this.m_attachments.Remove(entity))
		{
			Log.Mike.Print(string.Format("Entity.RemoveAttachment() - {0} is not an attachment of {1}", entity, this), new object[0]);
			return;
		}
		if (this.m_card == null)
		{
			return;
		}
		this.m_card.OnEnchantmentRemoved(count, entity);
	}

	// Token: 0x060018D4 RID: 6356 RVA: 0x000742D4 File Offset: 0x000724D4
	public void AddSubCard(Entity entity)
	{
		if (this.m_subCardIDs.Contains(entity.GetEntityId()))
		{
			return;
		}
		this.m_subCardIDs.Add(entity.GetEntityId());
	}

	// Token: 0x060018D5 RID: 6357 RVA: 0x00074309 File Offset: 0x00072509
	public void RemoveSubCard(Entity entity)
	{
		this.m_subCardIDs.Remove(entity.GetEntityId());
	}

	// Token: 0x060018D6 RID: 6358 RVA: 0x0007431D File Offset: 0x0007251D
	public List<Entity> GetAttachments()
	{
		return this.m_attachments;
	}

	// Token: 0x060018D7 RID: 6359 RVA: 0x00074328 File Offset: 0x00072528
	public bool DoEnchantmentsHaveTriggerVisuals()
	{
		foreach (Entity entity in this.m_attachments)
		{
			if (entity.HasTriggerVisual())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x00074390 File Offset: 0x00072590
	public bool IsEnchanted()
	{
		return this.m_attachments.Count > 0;
	}

	// Token: 0x060018D9 RID: 6361 RVA: 0x000743A0 File Offset: 0x000725A0
	public List<Entity> GetEnchantments()
	{
		return this.GetAttachments();
	}

	// Token: 0x060018DA RID: 6362 RVA: 0x000743A8 File Offset: 0x000725A8
	public List<int> GetSubCardIDs()
	{
		return this.m_subCardIDs;
	}

	// Token: 0x060018DB RID: 6363 RVA: 0x000743B0 File Offset: 0x000725B0
	public int GetSubCardIndex(Entity entity)
	{
		if (entity == null)
		{
			return -1;
		}
		int entityId = entity.GetEntityId();
		for (int i = 0; i < this.m_subCardIDs.Count; i++)
		{
			if (this.m_subCardIDs[i] == entityId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060018DC RID: 6364 RVA: 0x00074400 File Offset: 0x00072600
	public Entity GetParentEntity()
	{
		int tag = base.GetTag(GAME_TAG.PARENT_CARD);
		return GameState.Get().GetEntity(tag);
	}

	// Token: 0x060018DD RID: 6365 RVA: 0x00074424 File Offset: 0x00072624
	public Power GetMasterPower()
	{
		return this.m_entityDef.GetMasterPower();
	}

	// Token: 0x060018DE RID: 6366 RVA: 0x00074431 File Offset: 0x00072631
	public Power GetAttackPower()
	{
		return this.m_entityDef.GetAttackPower();
	}

	// Token: 0x060018DF RID: 6367 RVA: 0x00074440 File Offset: 0x00072640
	public Entity CloneForZoneMgr()
	{
		Entity entity = new Entity();
		entity.m_entityDef = this.m_entityDef;
		entity.m_card = this.m_card;
		entity.m_cardId = this.m_cardId;
		entity.ReplaceTags(this.m_tags);
		entity.m_loadState = this.m_loadState;
		return entity;
	}

	// Token: 0x060018E0 RID: 6368 RVA: 0x00074490 File Offset: 0x00072690
	public Entity CloneForHistory(int damageBonus, int damageBonusDouble, int healingDouble)
	{
		Entity entity = new Entity();
		entity.m_duplicateForHistory = true;
		entity.m_entityDef = this.m_entityDef;
		entity.m_card = this.m_card;
		entity.m_cardId = this.m_cardId;
		entity.ReplaceTags(this.m_tags);
		entity.SetTag<TAG_ZONE>(GAME_TAG.ZONE, TAG_ZONE.HAND);
		entity.m_historyDamageBonus = damageBonus;
		entity.m_historyDamageBonusDouble = damageBonusDouble;
		entity.m_historyHealingDouble = healingDouble;
		entity.m_loadState = this.m_loadState;
		return entity;
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x00074505 File Offset: 0x00072705
	public bool IsHistoryDupe()
	{
		return this.m_duplicateForHistory;
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x0007450D File Offset: 0x0007270D
	public int GetHistoryDamageBonus()
	{
		return this.m_historyDamageBonus;
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x00074515 File Offset: 0x00072715
	public int GetHistoryDamageBonusDouble()
	{
		return this.m_historyDamageBonusDouble;
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x0007451D File Offset: 0x0007271D
	public int GetHistoryHealingDouble()
	{
		return this.m_historyHealingDouble;
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x00074528 File Offset: 0x00072728
	public int GetDamageBonus()
	{
		Player controller = this.GetController();
		if (controller == null)
		{
			return 0;
		}
		if (base.IsSpell())
		{
			int num = controller.GetTag(GAME_TAG.CURRENT_SPELLPOWER);
			if (base.HasTag(GAME_TAG.RECEIVES_DOUBLE_SPELLDAMAGE_BONUS))
			{
				num *= 2;
			}
			return num;
		}
		if (base.IsHeroPower())
		{
			return controller.GetTag(GAME_TAG.CURRENT_HEROPOWER_DAMAGE_BONUS);
		}
		return 0;
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x0007458C File Offset: 0x0007278C
	public int GetDamageBonusDouble()
	{
		Player controller = this.GetController();
		if (controller == null)
		{
			return 0;
		}
		if (base.IsSpell())
		{
			return controller.GetTag(GAME_TAG.SPELLPOWER_DOUBLE);
		}
		if (base.IsHeroPower())
		{
			return controller.GetTag(GAME_TAG.HERO_POWER_DOUBLE);
		}
		return 0;
	}

	// Token: 0x060018E7 RID: 6375 RVA: 0x000745D8 File Offset: 0x000727D8
	public int GetHealingDouble()
	{
		Player controller = this.GetController();
		if (controller == null)
		{
			return 0;
		}
		if (base.IsSpell())
		{
			return controller.GetTag(GAME_TAG.HEALING_DOUBLE);
		}
		if (base.IsHeroPower())
		{
			return controller.GetTag(GAME_TAG.HERO_POWER_DOUBLE);
		}
		return 0;
	}

	// Token: 0x060018E8 RID: 6376 RVA: 0x00074623 File Offset: 0x00072823
	public void ClearBattlecryFlag()
	{
		this.m_useBattlecryPower = false;
	}

	// Token: 0x060018E9 RID: 6377 RVA: 0x0007462C File Offset: 0x0007282C
	public bool ShouldUseBattlecryPower()
	{
		return this.m_useBattlecryPower;
	}

	// Token: 0x060018EA RID: 6378 RVA: 0x00074634 File Offset: 0x00072834
	public void UpdateUseBattlecryFlag(bool fromGameState)
	{
		if (!base.IsMinion())
		{
			return;
		}
		bool flag = fromGameState || GameState.Get().EntityHasTargets(this);
		bool flag2 = base.GetZone() == TAG_ZONE.HAND && flag;
		if (flag2)
		{
			this.m_useBattlecryPower = true;
		}
	}

	// Token: 0x060018EB RID: 6379 RVA: 0x00074680 File Offset: 0x00072880
	public void InitRealTimeValues(Network.Entity netEntity)
	{
		this.SetRealTimeCost(base.GetCost());
		this.SetRealTimeAttack(base.GetATK());
		this.SetRealTimeHealth(base.GetHealth());
		this.SetRealTimeDamage(base.GetDamage());
		this.SetRealTimeArmor(base.GetArmor());
		this.SetRealTimePoweredUp(base.GetTag(GAME_TAG.POWERED_UP));
		foreach (Network.Entity.Tag tag in netEntity.Tags)
		{
			GAME_TAG name = (GAME_TAG)tag.Name;
			switch (name)
			{
			case GAME_TAG.DAMAGE:
				this.SetRealTimeDamage(tag.Value);
				continue;
			case GAME_TAG.HEALTH:
				break;
			default:
				if (name != GAME_TAG.DURABILITY)
				{
					if (name == GAME_TAG.ARMOR)
					{
						this.SetRealTimeArmor(tag.Value);
						continue;
					}
					if (name != GAME_TAG.POWERED_UP)
					{
						continue;
					}
					this.SetRealTimePoweredUp(tag.Value);
					continue;
				}
				break;
			case GAME_TAG.ATK:
				this.SetRealTimeAttack(tag.Value);
				continue;
			case GAME_TAG.COST:
				this.SetRealTimeCost(tag.Value);
				continue;
			}
			this.SetRealTimeHealth(tag.Value);
		}
	}

	// Token: 0x060018EC RID: 6380 RVA: 0x000747D8 File Offset: 0x000729D8
	public void SetRealTimeCost(int newCost)
	{
		this.m_realTimeCost = newCost;
	}

	// Token: 0x060018ED RID: 6381 RVA: 0x000747E1 File Offset: 0x000729E1
	public int GetRealTimeCost()
	{
		if (this.m_realTimeCost == -1)
		{
			return base.GetCost();
		}
		return this.m_realTimeCost;
	}

	// Token: 0x060018EE RID: 6382 RVA: 0x000747FC File Offset: 0x000729FC
	public void SetRealTimeAttack(int newAttack)
	{
		this.m_realTimeAttack = newAttack;
	}

	// Token: 0x060018EF RID: 6383 RVA: 0x00074805 File Offset: 0x00072A05
	public int GetRealTimeAttack()
	{
		return this.m_realTimeAttack;
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x0007480D File Offset: 0x00072A0D
	public void SetRealTimeHealth(int newHealth)
	{
		this.m_realTimeHealth = newHealth;
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x00074816 File Offset: 0x00072A16
	public void SetRealTimeDamage(int newDamage)
	{
		this.m_realTimeDamage = newDamage;
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x0007481F File Offset: 0x00072A1F
	public void SetRealTimeArmor(int newArmor)
	{
		this.m_realTimeArmor = newArmor;
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x00074828 File Offset: 0x00072A28
	public int GetRealTimeRemainingHP()
	{
		return this.m_realTimeHealth + this.m_realTimeArmor - this.m_realTimeDamage;
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x0007483E File Offset: 0x00072A3E
	public void SetRealTimePoweredUp(int poweredUp)
	{
		this.m_realTimePoweredUp = (poweredUp == 1);
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x00074854 File Offset: 0x00072A54
	public bool GetRealTimePoweredUp()
	{
		return this.m_realTimePoweredUp;
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x0007485C File Offset: 0x00072A5C
	public PlayErrors.TargetEntityInfo ConvertToTargetInfo()
	{
		return new PlayErrors.TargetEntityInfo
		{
			id = base.GetEntityId(),
			owningPlayerID = base.GetControllerId(),
			damage = base.GetDamage(),
			attack = base.GetATK(),
			race = (int)this.m_entityDef.GetRace(),
			rarity = (int)this.m_entityDef.GetRarity(),
			isImmune = base.IsImmune(),
			canBeAttacked = base.CanBeAttacked(),
			canBeTargetedByOpponents = base.CanBeTargetedByOpponents(),
			canBeTargetedBySpells = base.CanBeTargetedByAbilities(),
			canBeTargetedByHeroPowers = base.CanBeTargetedByHeroPowers(),
			canBeTargetedByBattlecries = base.CanBeTargetedByBattlecries(),
			cardType = base.GetCardType(),
			isFrozen = base.IsFrozen(),
			isEnchanted = this.IsEnchanted(),
			isStealthed = base.IsStealthed(),
			isTaunter = base.HasTaunt(),
			isMagnet = base.IsMagnet(),
			hasCharge = base.HasCharge(),
			hasAttackedThisTurn = (base.GetNumAttacksThisTurn() > 0),
			hasBattlecry = base.HasBattlecry(),
			hasDeathrattle = base.HasDeathrattle()
		};
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x00074988 File Offset: 0x00072B88
	public PlayErrors.SourceEntityInfo ConvertToSourceInfo(PlayErrors.PlayRequirementInfo playRequirementInfo, Entity parent)
	{
		List<string> entourageCardIDs = this.GetEntityDef().GetEntourageCardIDs();
		List<string> list = new List<string>();
		int num = 0;
		ZoneMgr zoneMgr = ZoneMgr.Get();
		if (zoneMgr != null)
		{
			ZonePlay zonePlay = zoneMgr.FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY);
			if (zonePlay != null)
			{
				foreach (Card card in zonePlay.GetCards())
				{
					Entity entity = card.GetEntity();
					if (entity != null)
					{
						list.Add(entity.GetCardId());
					}
				}
			}
			ZonePlay zonePlay2 = zoneMgr.FindZoneOfType<ZonePlay>(Player.Side.OPPOSING);
			if (zonePlay2 != null)
			{
				foreach (Card card2 in zonePlay2.GetCards())
				{
					Entity entity2 = card2.GetEntity();
					if (entity2 != null)
					{
						if (entity2.IsMinion())
						{
							num++;
						}
					}
				}
			}
		}
		PlayErrors.SourceEntityInfo sourceEntityInfo = new PlayErrors.SourceEntityInfo();
		sourceEntityInfo.requirementsMap = playRequirementInfo.requirementsMap;
		sourceEntityInfo.id = base.GetEntityId();
		sourceEntityInfo.cost = base.GetCost();
		sourceEntityInfo.attack = base.GetATK();
		sourceEntityInfo.minAttackRequirement = playRequirementInfo.paramMinAtk;
		sourceEntityInfo.maxAttackRequirement = playRequirementInfo.paramMaxAtk;
		sourceEntityInfo.raceRequirement = playRequirementInfo.paramRace;
		sourceEntityInfo.numMinionSlotsRequirement = playRequirementInfo.paramNumMinionSlots;
		sourceEntityInfo.numMinionSlotsWithTargetRequirement = playRequirementInfo.paramNumMinionSlotsWithTarget;
		sourceEntityInfo.minTotalMinionsRequirement = playRequirementInfo.paramMinNumTotalMinions;
		sourceEntityInfo.minFriendlyMinionsRequirement = playRequirementInfo.paramMinNumFriendlyMinions;
		sourceEntityInfo.minEnemyMinionsRequirement = playRequirementInfo.paramMinNumEnemyMinions;
		sourceEntityInfo.numTurnsInPlay = base.GetNumTurnsInPlay();
		sourceEntityInfo.numAttacksThisTurn = base.GetNumAttacksThisTurn();
		sourceEntityInfo.numAttacksAllowedThisTurn = 1 + base.GetWindfury() + base.GetExtraAttacksThisTurn();
		sourceEntityInfo.cardType = base.GetCardType();
		sourceEntityInfo.zone = base.GetZone();
		sourceEntityInfo.isSecret = base.IsSecret();
		sourceEntityInfo.isDuplicateSecret = false;
		if (sourceEntityInfo.isSecret)
		{
			Player player = GameState.Get().GetPlayer(base.GetControllerId());
			if (player != null)
			{
				List<string> secretDefinitions = player.GetSecretDefinitions();
				foreach (string text in secretDefinitions)
				{
					if (this.GetCardId().Equals(text, 5))
					{
						sourceEntityInfo.isDuplicateSecret = true;
						break;
					}
				}
			}
		}
		sourceEntityInfo.isExhausted = base.IsExhausted();
		sourceEntityInfo.isMasterPower = (base.GetZone() == TAG_ZONE.HAND || base.IsHeroPower());
		sourceEntityInfo.isActionPower = (TAG_ZONE.HAND == base.GetZone());
		sourceEntityInfo.isActivatePower = (base.GetZone() == TAG_ZONE.PLAY || base.IsHeroPower());
		sourceEntityInfo.isAttackPower = (base.IsHero() || (!base.IsHeroPower() && TAG_ZONE.PLAY == base.GetZone()));
		sourceEntityInfo.isFrozen = base.IsFrozen();
		sourceEntityInfo.hasBattlecry = base.HasBattlecry();
		sourceEntityInfo.canAttack = base.CanAttack();
		sourceEntityInfo.entireEntourageInPlay = false;
		if (entourageCardIDs.Count > 0)
		{
			sourceEntityInfo.entireEntourageInPlay = (list.Count > 0);
			string entourageCardID;
			foreach (string entourageCardID2 in entourageCardIDs)
			{
				entourageCardID = entourageCardID2;
				string text2 = list.Find((string otherCardID) => otherCardID.Equals(entourageCardID, 5));
				if (text2 == null)
				{
					sourceEntityInfo.entireEntourageInPlay = false;
					break;
				}
			}
		}
		sourceEntityInfo.hasCharge = base.HasCharge();
		sourceEntityInfo.isChoiceMinion = (parent != null && parent.IsMinion());
		sourceEntityInfo.cannotAttackHeroes = base.CannotAttackHeroes();
		return sourceEntityInfo;
	}

	// Token: 0x060018F8 RID: 6392 RVA: 0x00074DCC File Offset: 0x00072FCC
	private void LoadEntityDef(string cardId)
	{
		if (this.m_cardId != cardId)
		{
			this.m_cardId = cardId;
		}
		if (string.IsNullOrEmpty(cardId))
		{
			return;
		}
		this.m_entityDef = DefLoader.Get().GetEntityDef(cardId);
		if (this.m_entityDef == null)
		{
			Error.AddDevFatal("Failed to load a card xml for {0}", new object[]
			{
				cardId
			});
			return;
		}
		this.UpdateCardName();
	}

	// Token: 0x060018F9 RID: 6393 RVA: 0x00074E34 File Offset: 0x00073034
	public void LoadCard(string cardId)
	{
		this.LoadEntityDef(cardId);
		this.m_loadState = Entity.LoadState.LOADING;
		if (string.IsNullOrEmpty(cardId))
		{
			DefLoader.Get().LoadCardDef("HiddenCard", new DefLoader.LoadDefCallback<CardDef>(this.OnCardDefLoaded), null, null);
		}
		else
		{
			DefLoader.Get().LoadCardDef(cardId, new DefLoader.LoadDefCallback<CardDef>(this.OnCardDefLoaded), null, null);
		}
	}

	// Token: 0x060018FA RID: 6394 RVA: 0x00074E98 File Offset: 0x00073098
	private void OnCardDefLoaded(string cardId, CardDef cardDef, object callbackData)
	{
		if (cardDef == null)
		{
			Debug.LogErrorFormat("Entity.OnCardDefLoaded() - {0} does not have an asset!", new object[]
			{
				cardId
			});
			this.m_loadState = Entity.LoadState.DONE;
			return;
		}
		if (this.m_card != null)
		{
			this.m_card.LoadCardDef(cardDef);
		}
		this.UpdateUseBattlecryFlag(false);
		this.m_loadState = Entity.LoadState.DONE;
	}

	// Token: 0x04000CBF RID: 3263
	private EntityDef m_entityDef = new EntityDef();

	// Token: 0x04000CC0 RID: 3264
	private Card m_card;

	// Token: 0x04000CC1 RID: 3265
	private string m_cardId;

	// Token: 0x04000CC2 RID: 3266
	private Entity.LoadState m_loadState;

	// Token: 0x04000CC3 RID: 3267
	private int m_cardAssetLoadCount;

	// Token: 0x04000CC4 RID: 3268
	private bool m_useBattlecryPower;

	// Token: 0x04000CC5 RID: 3269
	private bool m_duplicateForHistory;

	// Token: 0x04000CC6 RID: 3270
	private int m_historyDamageBonus;

	// Token: 0x04000CC7 RID: 3271
	private int m_historyDamageBonusDouble;

	// Token: 0x04000CC8 RID: 3272
	private int m_historyHealingDouble;

	// Token: 0x04000CC9 RID: 3273
	private List<Entity> m_attachments = new List<Entity>();

	// Token: 0x04000CCA RID: 3274
	private List<int> m_subCardIDs = new List<int>();

	// Token: 0x04000CCB RID: 3275
	private int m_realTimeCost = -1;

	// Token: 0x04000CCC RID: 3276
	private int m_realTimeAttack;

	// Token: 0x04000CCD RID: 3277
	private int m_realTimeHealth;

	// Token: 0x04000CCE RID: 3278
	private int m_realTimeDamage;

	// Token: 0x04000CCF RID: 3279
	private int m_realTimeArmor;

	// Token: 0x04000CD0 RID: 3280
	private bool m_realTimePoweredUp;

	// Token: 0x020006A1 RID: 1697
	public enum LoadState
	{
		// Token: 0x04002E68 RID: 11880
		INVALID,
		// Token: 0x04002E69 RID: 11881
		PENDING,
		// Token: 0x04002E6A RID: 11882
		LOADING,
		// Token: 0x04002E6B RID: 11883
		DONE
	}
}
