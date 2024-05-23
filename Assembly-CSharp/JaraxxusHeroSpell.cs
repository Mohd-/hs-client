using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000ABB RID: 2747
public class JaraxxusHeroSpell : Spell
{
	// Token: 0x06005F05 RID: 24325 RVA: 0x001C714C File Offset: 0x001C534C
	public override bool AddPowerTargets()
	{
		foreach (PowerTask powerTask in this.m_taskList.GetTaskList())
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.FULL_ENTITY)
			{
				Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
				int id = histFullEntity.Entity.ID;
				Entity entity = GameState.Get().GetEntity(id);
				if (entity == null)
				{
					string text = string.Format("{0}.AddPowerTargets() - WARNING encountered HistFullEntity where entity id={1} but there is no entity with that id", this, id);
					Debug.LogWarning(text);
					return false;
				}
				if (entity.IsHeroPower())
				{
					this.m_heroPowerTask = powerTask;
					this.AddTarget(entity.GetCard().gameObject);
					if (this.m_weaponTask != null)
					{
						return true;
					}
				}
				else if (entity.IsWeapon())
				{
					this.m_weaponTask = powerTask;
					this.AddTarget(entity.GetCard().gameObject);
					if (this.m_heroPowerTask != null)
					{
						return true;
					}
				}
			}
		}
		this.Reset();
		return false;
	}

	// Token: 0x06005F06 RID: 24326 RVA: 0x001C7288 File Offset: 0x001C5488
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		base.StartCoroutine(this.SetupCards());
	}

	// Token: 0x06005F07 RID: 24327 RVA: 0x001C72A0 File Offset: 0x001C54A0
	private IEnumerator SetupCards()
	{
		Entity heroPower = this.LoadCardFromTask(this.m_heroPowerTask);
		Entity weapon = this.LoadCardFromTask(this.m_weaponTask);
		while (heroPower.IsLoadingAssets() || weapon.IsLoadingAssets())
		{
			yield return null;
		}
		Card heroPowerCard = heroPower.GetCard();
		heroPowerCard.HideCard();
		Zone heroPowerZone = ZoneMgr.Get().FindZoneForEntity(heroPower);
		heroPowerCard.TransitionToZone(heroPowerZone);
		Card weaponCard = weapon.GetCard();
		weaponCard.HideCard();
		Zone weaponZone = ZoneMgr.Get().FindZoneForEntity(weapon);
		weaponCard.TransitionToZone(weaponZone);
		while (heroPowerCard.IsActorLoading() || weaponCard.IsActorLoading())
		{
			yield return null;
		}
		this.PlayCardSpells(heroPowerCard, weaponCard);
		yield break;
	}

	// Token: 0x06005F08 RID: 24328 RVA: 0x001C72BC File Offset: 0x001C54BC
	private Entity LoadCardFromTask(PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
		Network.Entity entity = histFullEntity.Entity;
		int id = entity.ID;
		Entity entity2 = GameState.Get().GetEntity(id);
		entity2.LoadCard(entity.CardID);
		return entity2;
	}

	// Token: 0x06005F09 RID: 24329 RVA: 0x001C7304 File Offset: 0x001C5504
	private Card GetCardFromTask(PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
		Network.Entity entity = histFullEntity.Entity;
		int id = entity.ID;
		Entity entity2 = GameState.Get().GetEntity(id);
		return entity2.GetCard();
	}

	// Token: 0x06005F0A RID: 24330 RVA: 0x001C7341 File Offset: 0x001C5541
	private void Reset()
	{
		this.m_heroPowerTask = null;
		this.m_weaponTask = null;
	}

	// Token: 0x06005F0B RID: 24331 RVA: 0x001C7351 File Offset: 0x001C5551
	private void Finish()
	{
		this.Reset();
		this.OnSpellFinished();
	}

	// Token: 0x06005F0C RID: 24332 RVA: 0x001C7360 File Offset: 0x001C5560
	private void PlayCardSpells(Card heroPowerCard, Card weaponCard)
	{
		heroPowerCard.ShowCard();
		heroPowerCard.ActivateStateSpells();
		heroPowerCard.ActivateActorSpell(SpellType.SUMMON_JARAXXUS, new Spell.FinishedCallback(this.OnSpellFinished_HeroPower));
		weaponCard.ActivateActorSpell(SpellType.SUMMON_JARAXXUS, new Spell.FinishedCallback(this.OnSpellFinished_Weapon));
	}

	// Token: 0x06005F0D RID: 24333 RVA: 0x001C73A4 File Offset: 0x001C55A4
	private void OnSpellFinished_HeroPower(Spell spell, object userData)
	{
		this.m_heroPowerTask.SetCompleted(true);
		if (this.m_weaponTask.IsCompleted())
		{
			this.Finish();
		}
	}

	// Token: 0x06005F0E RID: 24334 RVA: 0x001C73D4 File Offset: 0x001C55D4
	private void OnSpellFinished_Weapon(Spell spell, object userData)
	{
		Card cardFromTask = this.GetCardFromTask(this.m_weaponTask);
		cardFromTask.ShowCard();
		cardFromTask.ActivateStateSpells();
		this.m_weaponTask.SetCompleted(true);
		if (this.m_heroPowerTask.IsCompleted())
		{
			this.Finish();
		}
	}

	// Token: 0x0400467B RID: 18043
	private PowerTask m_heroPowerTask;

	// Token: 0x0400467C RID: 18044
	private PowerTask m_weaponTask;
}
