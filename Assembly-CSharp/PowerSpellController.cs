using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008C5 RID: 2245
public class PowerSpellController : SpellController
{
	// Token: 0x0600548D RID: 21645 RVA: 0x00194890 File Offset: 0x00192A90
	protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
	{
		if (!this.HasSourceCard(taskList))
		{
			return false;
		}
		Entity sourceEntity = taskList.GetSourceEntity();
		Card card = sourceEntity.GetCard();
		CardEffect cardEffect = this.InitEffect(card);
		if (cardEffect == null)
		{
			return false;
		}
		if (sourceEntity.IsMinion())
		{
			if (!this.InitPowerSpell(cardEffect, card))
			{
				if (!SpellUtils.CanAddPowerTargets(taskList))
				{
					this.Reset();
					return false;
				}
				Spell actorBattlecrySpell = this.GetActorBattlecrySpell(card);
				if (actorBattlecrySpell == null)
				{
					this.Reset();
					return false;
				}
			}
		}
		else
		{
			this.InitPowerSpell(cardEffect, card);
			this.InitPowerSounds(cardEffect, card);
			if (this.m_powerSpell == null && this.m_powerSoundSpells.Count == 0)
			{
				this.Reset();
				return false;
			}
		}
		base.SetSource(card);
		return true;
	}

	// Token: 0x0600548E RID: 21646 RVA: 0x00194958 File Offset: 0x00192B58
	protected override void OnProcessTaskList()
	{
		if (this.ActivateActorBattlecrySpell())
		{
			return;
		}
		if (this.ActivateCardEffects())
		{
			return;
		}
		base.OnProcessTaskList();
	}

	// Token: 0x0600548F RID: 21647 RVA: 0x00194978 File Offset: 0x00192B78
	protected override void OnFinished()
	{
		if (this.m_processingTaskList)
		{
			this.m_pendingFinish = true;
			return;
		}
		base.StartCoroutine(this.WaitThenFinish());
	}

	// Token: 0x06005490 RID: 21648 RVA: 0x001949A8 File Offset: 0x00192BA8
	private void Reset()
	{
		SpellUtils.PurgeSpell(this.m_powerSpell);
		SpellUtils.PurgeSpells<CardSoundSpell>(this.m_powerSoundSpells);
		this.m_powerSpell = null;
		this.m_powerSoundSpells.Clear();
		this.m_cardEffectsBlockingFinish = 0;
		this.m_cardEffectsBlockingTaskListFinish = 0;
	}

	// Token: 0x06005491 RID: 21649 RVA: 0x001949EC File Offset: 0x00192BEC
	private IEnumerator WaitThenFinish()
	{
		yield return new WaitForSeconds(10f);
		this.Reset();
		base.OnFinished();
		yield break;
	}

	// Token: 0x06005492 RID: 21650 RVA: 0x00194A08 File Offset: 0x00192C08
	private Spell GetActorBattlecrySpell(Card card)
	{
		Spell actorSpell = card.GetActorSpell(SpellType.BATTLECRY, true);
		if (actorSpell == null)
		{
			return null;
		}
		if (!actorSpell.HasUsableState(SpellStateType.ACTION))
		{
			return null;
		}
		return actorSpell;
	}

	// Token: 0x06005493 RID: 21651 RVA: 0x00194A3C File Offset: 0x00192C3C
	private bool ActivateActorBattlecrySpell()
	{
		Card source = base.GetSource();
		Entity entity = source.GetEntity();
		if (!this.CanActivateActorBattlecrySpell(entity))
		{
			return false;
		}
		Spell actorBattlecrySpell = this.GetActorBattlecrySpell(source);
		if (actorBattlecrySpell == null)
		{
			return false;
		}
		base.StartCoroutine(this.WaitThenActivateActorBattlecrySpell(actorBattlecrySpell));
		return true;
	}

	// Token: 0x06005494 RID: 21652 RVA: 0x00194A8C File Offset: 0x00192C8C
	private bool CanActivateActorBattlecrySpell(Entity entity)
	{
		return this.m_taskList.IsOrigin() && (entity.HasBattlecry() || (entity.HasCombo() && entity.GetController().IsComboActive()));
	}

	// Token: 0x06005495 RID: 21653 RVA: 0x00194AD8 File Offset: 0x00192CD8
	private IEnumerator WaitThenActivateActorBattlecrySpell(Spell actorBattlecrySpell)
	{
		yield return new WaitForSeconds(0.2f);
		actorBattlecrySpell.ActivateState(SpellStateType.ACTION);
		if (!this.ActivateCardEffects())
		{
			base.OnProcessTaskList();
		}
		yield break;
	}

	// Token: 0x06005496 RID: 21654 RVA: 0x00194B04 File Offset: 0x00192D04
	private CardEffect InitEffect(Card card)
	{
		if (card == null)
		{
			return null;
		}
		Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
		string effectCardId = blockStart.EffectCardId;
		int effectIndex = blockStart.EffectIndex;
		CardEffect result;
		if (string.IsNullOrEmpty(effectCardId))
		{
			if (effectIndex >= 0)
			{
				result = card.GetSubOptionEffect(effectIndex);
			}
			else
			{
				result = card.GetPlayEffect();
			}
		}
		else
		{
			CardDef cardDef = DefLoader.Get().GetCardDef(effectCardId, null);
			CardEffectDef def;
			if (effectIndex >= 0)
			{
				if (cardDef.m_SubOptionEffectDefs == null)
				{
					return null;
				}
				if (effectIndex >= cardDef.m_SubOptionEffectDefs.Count)
				{
					return null;
				}
				def = cardDef.m_SubOptionEffectDefs[effectIndex];
			}
			else
			{
				def = cardDef.m_PlayEffectDef;
			}
			result = new CardEffect(def, card);
		}
		return result;
	}

	// Token: 0x06005497 RID: 21655 RVA: 0x00194BC8 File Offset: 0x00192DC8
	private bool ActivateCardEffects()
	{
		bool flag = this.ActivatePowerSpell();
		bool flag2 = this.ActivatePowerSounds();
		return flag || flag2;
	}

	// Token: 0x06005498 RID: 21656 RVA: 0x00194BED File Offset: 0x00192DED
	private void OnCardSpellFinished(Spell spell, object userData)
	{
		this.CardSpellFinished();
	}

	// Token: 0x06005499 RID: 21657 RVA: 0x00194BF5 File Offset: 0x00192DF5
	private void OnCardSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		this.CardSpellNoneStateEntered();
	}

	// Token: 0x0600549A RID: 21658 RVA: 0x00194C09 File Offset: 0x00192E09
	private void CardSpellFinished()
	{
		this.m_cardEffectsBlockingTaskListFinish--;
		if (this.m_cardEffectsBlockingTaskListFinish > 0)
		{
			return;
		}
		this.OnFinishedTaskList();
	}

	// Token: 0x0600549B RID: 21659 RVA: 0x00194C2C File Offset: 0x00192E2C
	private void CardSpellNoneStateEntered()
	{
		this.m_cardEffectsBlockingFinish--;
		if (this.m_cardEffectsBlockingFinish > 0)
		{
			return;
		}
		this.OnFinished();
	}

	// Token: 0x0600549C RID: 21660 RVA: 0x00194C50 File Offset: 0x00192E50
	private bool InitPowerSpell(CardEffect effect, Card card)
	{
		Spell spell = effect.GetSpell(true);
		if (spell == null)
		{
			return false;
		}
		if (!spell.HasUsableState(SpellStateType.ACTION))
		{
			Log.Power.PrintWarning("{0}.InitPowerSpell() - spell {1} for Card {2} has no {3} state", new object[]
			{
				base.name,
				spell,
				card,
				SpellStateType.ACTION
			});
			return false;
		}
		if (!spell.AttachPowerTaskList(this.m_taskList))
		{
			Log.Power.Print("{0}.InitPowerSpell() - FAILED to attach task list to spell {1} for Card {2}", new object[]
			{
				base.name,
				spell,
				card
			});
			return false;
		}
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			spell.ActivateState(SpellStateType.NONE);
		}
		this.m_powerSpell = spell;
		this.m_cardEffectsBlockingFinish++;
		this.m_cardEffectsBlockingTaskListFinish++;
		return true;
	}

	// Token: 0x0600549D RID: 21661 RVA: 0x00194D20 File Offset: 0x00192F20
	private bool ActivatePowerSpell()
	{
		if (this.m_powerSpell == null)
		{
			return false;
		}
		this.m_powerSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnCardSpellFinished));
		this.m_powerSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnCardSpellStateFinished));
		this.m_powerSpell.ActivateState(SpellStateType.ACTION);
		return true;
	}

	// Token: 0x0600549E RID: 21662 RVA: 0x00194D7C File Offset: 0x00192F7C
	private bool InitPowerSounds(CardEffect effect, Card card)
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
					Log.Power.Print("{0}.InitPowerSounds() - FAILED to attach task list to PowerSoundSpell {1} for Card {2}", new object[]
					{
						base.name,
						cardSoundSpell,
						card
					});
				}
				else
				{
					this.m_powerSoundSpells.Add(cardSoundSpell);
				}
			}
		}
		if (this.m_powerSoundSpells.Count == 0)
		{
			return false;
		}
		this.m_cardEffectsBlockingFinish++;
		this.m_cardEffectsBlockingTaskListFinish++;
		return true;
	}

	// Token: 0x0600549F RID: 21663 RVA: 0x00194E70 File Offset: 0x00193070
	private bool ActivatePowerSounds()
	{
		if (this.m_powerSoundSpells.Count == 0)
		{
			return false;
		}
		Card source = base.GetSource();
		foreach (CardSoundSpell cardSoundSpell in this.m_powerSoundSpells)
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

	// Token: 0x04003AEE RID: 15086
	private Spell m_powerSpell;

	// Token: 0x04003AEF RID: 15087
	private List<CardSoundSpell> m_powerSoundSpells = new List<CardSoundSpell>();

	// Token: 0x04003AF0 RID: 15088
	private int m_cardEffectsBlockingFinish;

	// Token: 0x04003AF1 RID: 15089
	private int m_cardEffectsBlockingTaskListFinish;
}
