using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E69 RID: 3689
public class HeroSwapSpell : Spell
{
	// Token: 0x06006FD6 RID: 28630 RVA: 0x0020D778 File Offset: 0x0020B978
	public override bool AddPowerTargets()
	{
		this.m_oldHeroCard = null;
		this.m_newHeroCard = null;
		foreach (PowerTask powerTask in this.m_taskList.GetTaskList())
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.FULL_ENTITY)
			{
				Network.HistFullEntity histFullEntity = (Network.HistFullEntity)power;
				int id = histFullEntity.Entity.ID;
				Entity entity = GameState.Get().GetEntity(id);
				if (entity == null)
				{
					string text = string.Format("{0}.AddPowerTargets() - WARNING encountered HistFullEntity where entity id={1} but there is no entity with that id", this, id);
					Debug.LogWarning(text);
					return false;
				}
				if (entity.IsHero())
				{
					this.m_newHeroCard = entity.GetCard();
				}
			}
			else if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = (Network.HistTagChange)power;
				if (histTagChange.Tag == 49)
				{
					if (histTagChange.Value == 6)
					{
						int entity2 = histTagChange.Entity;
						Entity entity3 = GameState.Get().GetEntity(entity2);
						if (entity3 == null)
						{
							string text2 = string.Format("{0}.AddPowerTargets() - WARNING encountered HistTagChange where entity id={1} but there is no entity with that id", this, entity2);
							Debug.LogWarning(text2);
							return false;
						}
						if (entity3.IsHero())
						{
							this.m_oldHeroCard = entity3.GetCard();
						}
					}
				}
			}
		}
		if (!this.m_oldHeroCard)
		{
			this.m_newHeroCard = null;
			return false;
		}
		if (!this.m_newHeroCard)
		{
			this.m_oldHeroCard = null;
			return false;
		}
		return true;
	}

	// Token: 0x06006FD7 RID: 28631 RVA: 0x0020D93C File Offset: 0x0020BB3C
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		base.StartCoroutine(this.SetupHero());
	}

	// Token: 0x06006FD8 RID: 28632 RVA: 0x0020D954 File Offset: 0x0020BB54
	private IEnumerator SetupHero()
	{
		Entity newHeroEntity = this.m_newHeroCard.GetEntity();
		PowerTask fullEntityTask = this.FindFullEntityTask();
		fullEntityTask.DoTask();
		while (newHeroEntity.IsLoadingAssets())
		{
			yield return null;
		}
		this.m_newHeroCard.HideCard();
		Zone heroZone = ZoneMgr.Get().FindZoneForEntity(newHeroEntity);
		this.m_newHeroCard.TransitionToZone(heroZone);
		while (this.m_newHeroCard.IsActorLoading())
		{
			yield return null;
		}
		this.m_newHeroCard.GetActor().TurnOffCollider();
		this.m_newHeroCard.transform.position = this.m_newHeroCard.GetZone().transform.position;
		if (this.m_OldHeroFX != null)
		{
			if (this.removeOldStats)
			{
				Actor oldActor = this.m_oldHeroCard.GetActor();
				Object.Destroy(oldActor.m_healthObject);
				Object.Destroy(oldActor.m_attackObject);
			}
			base.StartCoroutine(this.PlaySwapFx(this.m_OldHeroFX, this.m_oldHeroCard));
		}
		if (this.m_NewHeroFX != null)
		{
			base.StartCoroutine(this.PlaySwapFx(this.m_NewHeroFX, this.m_newHeroCard));
		}
		yield return new WaitForSeconds(this.m_FinishDelay);
		this.Finish();
		yield break;
	}

	// Token: 0x06006FD9 RID: 28633 RVA: 0x0020D96F File Offset: 0x0020BB6F
	public virtual void CustomizeFXProcess(Actor heroActor)
	{
	}

	// Token: 0x06006FDA RID: 28634 RVA: 0x0020D974 File Offset: 0x0020BB74
	private IEnumerator PlaySwapFx(Spell heroFX, Card heroCard)
	{
		Actor heroActor = heroCard.GetActor();
		this.CustomizeFXProcess(heroActor);
		Spell swapSpell = Object.Instantiate<Spell>(heroFX);
		SpellUtils.SetCustomSpellParent(swapSpell, heroActor);
		swapSpell.SetSource(heroCard.gameObject);
		swapSpell.Activate();
		while (!swapSpell.IsFinished())
		{
			yield return null;
		}
		while (swapSpell.GetActiveState() != SpellStateType.NONE)
		{
			yield return null;
		}
		Object.Destroy(swapSpell.gameObject);
		yield break;
	}

	// Token: 0x06006FDB RID: 28635 RVA: 0x0020D9AC File Offset: 0x0020BBAC
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

	// Token: 0x06006FDC RID: 28636 RVA: 0x0020DA20 File Offset: 0x0020BC20
	private void Finish()
	{
		this.m_newHeroCard.GetActor().TurnOnCollider();
		this.m_newHeroCard.ShowCard();
		this.OnSpellFinished();
	}

	// Token: 0x04005903 RID: 22787
	public Spell m_OldHeroFX;

	// Token: 0x04005904 RID: 22788
	public Spell m_NewHeroFX;

	// Token: 0x04005905 RID: 22789
	public float m_FinishDelay;

	// Token: 0x04005906 RID: 22790
	public bool removeOldStats;

	// Token: 0x04005907 RID: 22791
	protected Card m_oldHeroCard;

	// Token: 0x04005908 RID: 22792
	protected Card m_newHeroCard;
}
