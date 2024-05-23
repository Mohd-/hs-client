using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008C7 RID: 2247
public class TriggerSpellController : SpellController
{
	// Token: 0x060054AF RID: 21679 RVA: 0x001952C8 File Offset: 0x001934C8
	protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
	{
		if (!this.HasSourceCard(taskList))
		{
			return false;
		}
		Entity sourceEntity = taskList.GetSourceEntity();
		Card card = sourceEntity.GetCard();
		CardEffect cardEffect = this.InitEffect(card);
		if (cardEffect != null)
		{
			this.InitTriggerSpell(cardEffect, card);
			this.InitTriggerSounds(cardEffect, card);
		}
		if (this.CanPlayActorTriggerSpell(sourceEntity))
		{
			this.m_actorTriggerSpell = this.GetActorTriggerSpell(sourceEntity);
		}
		if (this.m_triggerSpell == null && this.m_triggerSoundSpells.Count == 0 && this.m_actorTriggerSpell == null)
		{
			this.Reset();
			return TurnStartManager.Get().IsCardDrawHandled(card);
		}
		base.SetSource(card);
		return true;
	}

	// Token: 0x060054B0 RID: 21680 RVA: 0x00195378 File Offset: 0x00193578
	protected override bool HasSourceCard(PowerTaskList taskList)
	{
		if (taskList.GetSourceEntity() == null)
		{
			return false;
		}
		Card cardWithActorTrigger = this.GetCardWithActorTrigger(taskList);
		return !(cardWithActorTrigger == null);
	}

	// Token: 0x060054B1 RID: 21681 RVA: 0x001953AC File Offset: 0x001935AC
	protected override void OnProcessTaskList()
	{
		if (!this.ActivateInitialSpell())
		{
			base.OnProcessTaskList();
			return;
		}
		if (GameState.Get().IsTurnStartManagerActive())
		{
			TurnStartManager.Get().NotifyOfTriggerVisual();
		}
	}

	// Token: 0x060054B2 RID: 21682 RVA: 0x001953E4 File Offset: 0x001935E4
	protected override void OnFinished()
	{
		if (this.m_processingTaskList)
		{
			this.m_pendingFinish = true;
			return;
		}
		base.StartCoroutine(this.WaitThenFinish());
	}

	// Token: 0x060054B3 RID: 21683 RVA: 0x00195411 File Offset: 0x00193611
	private void Reset()
	{
		SpellUtils.PurgeSpell(this.m_triggerSpell);
		SpellUtils.PurgeSpells<CardSoundSpell>(this.m_triggerSoundSpells);
		this.m_triggerSpell = null;
		this.m_triggerSoundSpells.Clear();
		this.m_actorTriggerSpell = null;
		this.m_cardEffectsBlockingFinish = 0;
		this.m_cardEffectsBlockingTaskListFinish = 0;
	}

	// Token: 0x060054B4 RID: 21684 RVA: 0x00195450 File Offset: 0x00193650
	private IEnumerator WaitThenFinish()
	{
		yield return new WaitForSeconds(10f);
		this.Reset();
		base.OnFinished();
		yield break;
	}

	// Token: 0x060054B5 RID: 21685 RVA: 0x0019546B File Offset: 0x0019366B
	private bool ActivateInitialSpell()
	{
		return this.ActivateActorTriggerSpell() || this.ActivateCardEffects();
	}

	// Token: 0x060054B6 RID: 21686 RVA: 0x00195488 File Offset: 0x00193688
	private Card GetCardWithActorTrigger(PowerTaskList taskList)
	{
		Entity sourceEntity = taskList.GetSourceEntity();
		return this.GetCardWithActorTrigger(sourceEntity);
	}

	// Token: 0x060054B7 RID: 21687 RVA: 0x001954A4 File Offset: 0x001936A4
	private Card GetCardWithActorTrigger(Entity entity)
	{
		if (entity == null)
		{
			return null;
		}
		Card card;
		if (entity.IsEnchantment())
		{
			Entity entity2 = GameState.Get().GetEntity(entity.GetAttached());
			if (entity2 == null)
			{
				return null;
			}
			card = entity2.GetCard();
		}
		else
		{
			card = entity.GetCard();
		}
		return card;
	}

	// Token: 0x060054B8 RID: 21688 RVA: 0x001954F4 File Offset: 0x001936F4
	private bool CanPlayActorTriggerSpell(Entity entity)
	{
		if (!entity.HasTriggerVisual() && !entity.IsPoisonous() && !entity.HasInspire())
		{
			return false;
		}
		if (entity.GetController() != null && !entity.GetController().IsFriendlySide() && entity.IsObfuscated())
		{
			return false;
		}
		Card cardWithActorTrigger = this.GetCardWithActorTrigger(entity);
		return !(cardWithActorTrigger == null) && !cardWithActorTrigger.WillSuppressActorTriggerSpell() && (this.m_triggerSpell != null || SpellUtils.CanAddPowerTargets(this.m_taskList));
	}

	// Token: 0x060054B9 RID: 21689 RVA: 0x00195598 File Offset: 0x00193798
	private Spell GetActorTriggerSpell(Entity entity)
	{
		Card cardWithActorTrigger = this.GetCardWithActorTrigger(entity);
		SpellType spellType;
		if (entity.HasTriggerVisual())
		{
			if (GameState.Get().IsUsingFastActorTriggers())
			{
				spellType = SpellType.FAST_TRIGGER;
			}
			else
			{
				spellType = SpellType.TRIGGER;
			}
		}
		else if (entity.IsPoisonous())
		{
			spellType = SpellType.POISONOUS;
		}
		else
		{
			if (!entity.HasInspire())
			{
				return null;
			}
			spellType = SpellType.INSPIRE;
		}
		return cardWithActorTrigger.GetActorSpell(spellType, true);
	}

	// Token: 0x060054BA RID: 21690 RVA: 0x00195608 File Offset: 0x00193808
	private bool ActivateActorTriggerSpell()
	{
		if (this.m_actorTriggerSpell == null)
		{
			return false;
		}
		Entity sourceEntity = this.m_taskList.GetSourceEntity();
		this.GetCardWithActorTrigger(sourceEntity).DeactivateBaubles();
		this.m_actorTriggerSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnActorTriggerSpellStateFinished));
		this.m_actorTriggerSpell.ActivateState(SpellStateType.ACTION);
		return true;
	}

	// Token: 0x060054BB RID: 21691 RVA: 0x00195664 File Offset: 0x00193864
	private void OnActorTriggerSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (prevStateType != SpellStateType.ACTION)
		{
			return;
		}
		spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnActorTriggerSpellStateFinished));
		if (this.ActivateCardEffects())
		{
			return;
		}
		base.OnProcessTaskList();
	}

	// Token: 0x060054BC RID: 21692 RVA: 0x00195694 File Offset: 0x00193894
	private CardEffect InitEffect(Card card)
	{
		if (card == null)
		{
			return null;
		}
		Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
		string effectCardId = blockStart.EffectCardId;
		int effectIndex = blockStart.EffectIndex;
		if (effectIndex < 0)
		{
			return null;
		}
		CardEffect result;
		if (string.IsNullOrEmpty(effectCardId))
		{
			result = card.GetTriggerEffect(effectIndex);
		}
		else
		{
			CardDef cardDef = DefLoader.Get().GetCardDef(effectCardId, null);
			if (cardDef.m_TriggerEffectDefs == null)
			{
				return null;
			}
			if (effectIndex >= cardDef.m_TriggerEffectDefs.Count)
			{
				return null;
			}
			CardEffectDef def = cardDef.m_TriggerEffectDefs[effectIndex];
			result = new CardEffect(def, card);
		}
		return result;
	}

	// Token: 0x060054BD RID: 21693 RVA: 0x00195738 File Offset: 0x00193938
	private bool ActivateCardEffects()
	{
		bool flag = this.ActivateTriggerSpell();
		bool flag2 = this.ActivateTriggerSounds();
		return flag || flag2;
	}

	// Token: 0x060054BE RID: 21694 RVA: 0x0019575D File Offset: 0x0019395D
	private void OnCardSpellFinished(Spell spell, object userData)
	{
		this.CardSpellFinished();
	}

	// Token: 0x060054BF RID: 21695 RVA: 0x00195765 File Offset: 0x00193965
	private void OnCardSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		this.CardSpellNoneStateEntered();
	}

	// Token: 0x060054C0 RID: 21696 RVA: 0x00195779 File Offset: 0x00193979
	private void CardSpellFinished()
	{
		this.m_cardEffectsBlockingTaskListFinish--;
		if (this.m_cardEffectsBlockingTaskListFinish > 0)
		{
			return;
		}
		this.OnFinishedTaskList();
	}

	// Token: 0x060054C1 RID: 21697 RVA: 0x0019579C File Offset: 0x0019399C
	private void CardSpellNoneStateEntered()
	{
		this.m_cardEffectsBlockingFinish--;
		if (this.m_cardEffectsBlockingFinish > 0)
		{
			return;
		}
		this.OnFinished();
	}

	// Token: 0x060054C2 RID: 21698 RVA: 0x001957C0 File Offset: 0x001939C0
	private void InitTriggerSpell(CardEffect effect, Card card)
	{
		Spell spell = effect.GetSpell(true);
		if (spell == null)
		{
			return;
		}
		if (!spell.AttachPowerTaskList(this.m_taskList))
		{
			Log.Power.Print("{0}.InitTriggerSpell() - FAILED to add targets to spell for {1}", new object[]
			{
				this,
				card
			});
			return;
		}
		this.m_triggerSpell = spell;
		this.m_cardEffectsBlockingFinish++;
		this.m_cardEffectsBlockingTaskListFinish++;
	}

	// Token: 0x060054C3 RID: 21699 RVA: 0x00195834 File Offset: 0x00193A34
	private bool ActivateTriggerSpell()
	{
		if (this.m_triggerSpell == null)
		{
			return false;
		}
		this.m_triggerSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnCardSpellFinished));
		this.m_triggerSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnCardSpellStateFinished));
		this.m_triggerSpell.ActivateState(SpellStateType.ACTION);
		return true;
	}

	// Token: 0x060054C4 RID: 21700 RVA: 0x00195890 File Offset: 0x00193A90
	private bool InitTriggerSounds(CardEffect effect, Card card)
	{
		List<CardSoundSpell> soundSpells = effect.GetSoundSpells(true);
		if (soundSpells == null)
		{
			return false;
		}
		if (soundSpells.Count == 0)
		{
			return false;
		}
		foreach (CardSoundSpell cardSoundSpell in soundSpells)
		{
			if (cardSoundSpell)
			{
				if (!cardSoundSpell.AttachPowerTaskList(this.m_taskList))
				{
					Log.Power.Print("{0}.InitTriggerSounds() - FAILED to attach task list to TriggerSoundSpell {1} for Card {2}", new object[]
					{
						base.name,
						cardSoundSpell,
						card
					});
				}
				else
				{
					this.m_triggerSoundSpells.Add(cardSoundSpell);
				}
			}
		}
		if (this.m_triggerSoundSpells.Count == 0)
		{
			return false;
		}
		this.m_cardEffectsBlockingFinish++;
		this.m_cardEffectsBlockingTaskListFinish++;
		return true;
	}

	// Token: 0x060054C5 RID: 21701 RVA: 0x00195984 File Offset: 0x00193B84
	private bool ActivateTriggerSounds()
	{
		if (this.m_triggerSoundSpells.Count == 0)
		{
			return false;
		}
		Card source = base.GetSource();
		foreach (CardSoundSpell cardSoundSpell in this.m_triggerSoundSpells)
		{
			if (cardSoundSpell)
			{
				source.ActivateSoundSpell(cardSoundSpell);
			}
		}
		this.CardSpellFinished();
		this.CardSpellNoneStateEntered();
		return true;
	}

	// Token: 0x04003AF6 RID: 15094
	private Spell m_triggerSpell;

	// Token: 0x04003AF7 RID: 15095
	private List<CardSoundSpell> m_triggerSoundSpells = new List<CardSoundSpell>();

	// Token: 0x04003AF8 RID: 15096
	private Spell m_actorTriggerSpell;

	// Token: 0x04003AF9 RID: 15097
	private int m_cardEffectsBlockingFinish;

	// Token: 0x04003AFA RID: 15098
	private int m_cardEffectsBlockingTaskListFinish;
}
