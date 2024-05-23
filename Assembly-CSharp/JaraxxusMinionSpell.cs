using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000ABD RID: 2749
public class JaraxxusMinionSpell : Spell
{
	// Token: 0x06005F16 RID: 24342 RVA: 0x001C7600 File Offset: 0x001C5800
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
				if (!entity.IsHero())
				{
					string text2 = string.Format("{0}.AddPowerTargets() - WARNING HistFullEntity where entity id={1} is not a hero", this, id);
					Debug.LogWarning(text2);
					return false;
				}
				this.AddTarget(entity.GetCard().gameObject);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005F17 RID: 24343 RVA: 0x001C770C File Offset: 0x001C590C
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		base.StartCoroutine(this.SetupHero());
	}

	// Token: 0x06005F18 RID: 24344 RVA: 0x001C7724 File Offset: 0x001C5924
	private IEnumerator SetupHero()
	{
		Card minionCard = base.GetSourceCard();
		Card heroCard = base.GetTargetCard();
		Entity heroEntity = heroCard.GetEntity();
		minionCard.SuppressDeathEffects(true);
		minionCard.GetActor().TurnOffCollider();
		PowerTask fullEntityTask = this.FindFullEntityTask();
		fullEntityTask.DoTask();
		while (heroEntity.IsLoadingAssets())
		{
			yield return null;
		}
		heroCard.HideCard();
		Zone heroZone = ZoneMgr.Get().FindZoneForEntity(heroEntity);
		heroCard.TransitionToZone(heroZone);
		while (heroCard.IsActorLoading())
		{
			yield return null;
		}
		heroCard.GetActor().TurnOffCollider();
		base.StartCoroutine(this.PlaySummoningSpells(minionCard, heroCard));
		yield break;
	}

	// Token: 0x06005F19 RID: 24345 RVA: 0x001C7740 File Offset: 0x001C5940
	private PowerTask FindFullEntityTask()
	{
		foreach (PowerTask powerTask in this.m_taskList.GetTaskList())
		{
			if (powerTask.GetPower().Type == Network.PowerType.FULL_ENTITY)
			{
				return powerTask;
			}
		}
		return null;
	}

	// Token: 0x06005F1A RID: 24346 RVA: 0x001C77B4 File Offset: 0x001C59B4
	private void Finish()
	{
		Card sourceCard = base.GetSourceCard();
		sourceCard.GetActor().TurnOnCollider();
		Card targetCard = base.GetTargetCard();
		targetCard.GetActor().TurnOnCollider();
		targetCard.ShowCard();
		this.OnSpellFinished();
	}

	// Token: 0x06005F1B RID: 24347 RVA: 0x001C77F4 File Offset: 0x001C59F4
	private IEnumerator PlaySummoningSpells(Card minionCard, Card heroCard)
	{
		heroCard.transform.position = minionCard.transform.position;
		minionCard.ActivateActorSpell(SpellType.SUMMON_JARAXXUS);
		heroCard.ActivateActorSpell(SpellType.SUMMON_JARAXXUS);
		yield return new WaitForSeconds(this.m_MoveToLocationDelay);
		this.MoveToSpellLocation(minionCard, heroCard);
		yield return new WaitForSeconds(this.m_MoveToHeroSpotDelay);
		this.MoveToHeroSpot(minionCard, heroCard, heroCard.GetZone());
		yield break;
	}

	// Token: 0x06005F1C RID: 24348 RVA: 0x001C782C File Offset: 0x001C5A2C
	private void MoveToSpellLocation(Card minionCard, Card heroCard)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			base.transform.position,
			"time",
			this.m_MoveToLocationDuration,
			"easetype",
			this.m_MoveToLocationEaseType
		});
		iTween.MoveTo(minionCard.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"position",
			base.transform.position,
			"time",
			this.m_MoveToLocationDuration,
			"easetype",
			this.m_MoveToLocationEaseType
		});
		iTween.MoveTo(heroCard.gameObject, args2);
	}

	// Token: 0x06005F1D RID: 24349 RVA: 0x001C78F8 File Offset: 0x001C5AF8
	private void MoveToHeroSpot(Card minionCard, Card heroCard, Zone heroZone)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			heroZone.transform.position,
			"time",
			this.m_MoveToHeroSpotDuration,
			"easetype",
			this.m_MoveToHeroSpotEaseType
		});
		iTween.MoveTo(minionCard.gameObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"position",
			heroZone.transform.position,
			"time",
			this.m_MoveToHeroSpotDuration,
			"easetype",
			this.m_MoveToHeroSpotEaseType,
			"oncomplete",
			"OnMoveToHeroSpotComplete",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(heroCard.gameObject, args2);
	}

	// Token: 0x06005F1E RID: 24350 RVA: 0x001C79E6 File Offset: 0x001C5BE6
	private void OnMoveToHeroSpotComplete()
	{
		this.Finish();
	}

	// Token: 0x04004686 RID: 18054
	public float m_MoveToLocationDelay;

	// Token: 0x04004687 RID: 18055
	public float m_MoveToLocationDuration = 1.5f;

	// Token: 0x04004688 RID: 18056
	public iTween.EaseType m_MoveToLocationEaseType = iTween.EaseType.linear;

	// Token: 0x04004689 RID: 18057
	public float m_MoveToHeroSpotDelay = 3.5f;

	// Token: 0x0400468A RID: 18058
	public float m_MoveToHeroSpotDuration = 0.3f;

	// Token: 0x0400468B RID: 18059
	public iTween.EaseType m_MoveToHeroSpotEaseType = iTween.EaseType.linear;
}
