using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008CB RID: 2251
public class RitualSpellController : SpellController
{
	// Token: 0x060054F1 RID: 21745 RVA: 0x001970FC File Offset: 0x001952FC
	protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
	{
		Entity sourceEntity = taskList.GetSourceEntity();
		Player controller = sourceEntity.GetController();
		if (taskList.IsOrigin())
		{
			return true;
		}
		if (!controller.HasTag(GAME_TAG.SEEN_CTHUN))
		{
			return false;
		}
		if (!taskList.IsEndOfBlock())
		{
			return false;
		}
		int tag = controller.GetTag(GAME_TAG.PROXY_CTHUN);
		if (tag == 0)
		{
			return false;
		}
		if (this.m_skipIfCthunInPlay && this.IsCthunInPlay(controller))
		{
			return false;
		}
		this.m_ritualEntity = GameState.Get().GetEntity(tag);
		if (this.m_ritualEntity == null)
		{
			return false;
		}
		PowerTaskList origin = taskList.GetOrigin();
		this.m_ritualEntityClone = origin.GetRitualEntityClone();
		if (this.m_ritualEntityClone == null)
		{
			return false;
		}
		Card card = sourceEntity.GetCard();
		base.SetSource(card);
		Card card2 = this.m_ritualEntity.GetCard();
		base.AddTarget(card2);
		return true;
	}

	// Token: 0x060054F2 RID: 21746 RVA: 0x001971D4 File Offset: 0x001953D4
	protected override void OnProcessTaskList()
	{
		base.StartCoroutine(this.DoRitualEffect());
	}

	// Token: 0x060054F3 RID: 21747 RVA: 0x001971E4 File Offset: 0x001953E4
	private IEnumerator DoRitualEffect()
	{
		if (this.m_taskList.IsOrigin())
		{
			int latestCreationTaskIndex = this.FindLatestProxyCthunCreationTask();
			if (latestCreationTaskIndex >= 0)
			{
				List<PowerTask> tasks = this.m_taskList.GetTaskList();
				PowerTask latestCreationTask = tasks[latestCreationTaskIndex];
				this.m_taskList.DoTasks(0, latestCreationTaskIndex + 1);
				while (!latestCreationTask.IsCompleted())
				{
					yield return null;
				}
			}
			Entity sourceEntity = this.m_taskList.GetSourceEntity();
			Player sourceController = sourceEntity.GetController();
			int ritualEntityId = sourceController.GetTag(GAME_TAG.PROXY_CTHUN);
			this.m_ritualEntity = GameState.Get().GetEntity(ritualEntityId);
			this.m_ritualEntityClone = this.m_ritualEntity.CloneForHistory(this.m_ritualEntity.GetDamageBonus(), this.m_ritualEntity.GetDamageBonusDouble(), this.m_ritualEntity.GetHealingDouble());
			this.m_taskList.SetRitualEntityClone(this.m_ritualEntityClone);
			if (!this.m_taskList.IsEndOfBlock())
			{
				this.FinishRitual();
				yield break;
			}
			if (!sourceController.HasTag(GAME_TAG.SEEN_CTHUN))
			{
				this.FinishRitual();
				yield break;
			}
		}
		this.m_taskList.DoAllTasks();
		while (!this.m_taskList.IsComplete())
		{
			yield return null;
		}
		HistoryManager historyManager = HistoryManager.Get();
		int ritualEntId = base.GetSource().GetEntity().GetEntityId();
		while (historyManager.HasBigCard() && historyManager.GetCurrentBigCard().GetEntity().GetEntityId() == ritualEntId)
		{
			yield return null;
		}
		Entity ritualEnt = (!this.m_showBonusAnims) ? this.m_ritualEntity : this.m_ritualEntityClone;
		this.m_ritualActor = this.LoadRitualActor(ritualEnt);
		if (this.m_ritualActor == null)
		{
			this.FinishRitual();
			yield break;
		}
		this.UpdateAndPositionRitualActor();
		if (this.m_ritualSpell != null)
		{
			Spell ritualSpellInstance = Object.Instantiate<Spell>(this.m_ritualSpell);
			ritualSpellInstance.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnRitualSpellStateFinished), this.m_ritualActor);
			ritualSpellInstance.AddSpellEventCallback(new Spell.SpellEventCallback(this.OnSpellEvent));
			ritualSpellInstance.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished));
			TransformUtil.AttachAndPreserveLocalTransform(ritualSpellInstance.transform, this.m_ritualActor.transform);
			this.m_ritualActor.GetHealthText().RenderQueue = 1;
			this.m_ritualActor.GetAttackText().RenderQueue = 1;
			ritualSpellInstance.Activate();
		}
		if (this.m_showBonusAnims)
		{
			yield return new WaitForSeconds(this.m_prebuffDisplayTime);
			if (!this.m_finished)
			{
				this.m_ritualActor.SetEntity(this.m_ritualEntity);
				if (!this.m_ritualEntityClone.HasTag(GAME_TAG.TAUNT) && this.m_ritualEntity.HasTag(GAME_TAG.TAUNT))
				{
					this.m_ritualActor.ActivateTaunt();
				}
				this.m_ritualActor.UpdateAllComponents();
			}
		}
		if (this.m_ritualSpell == null)
		{
			float remainingDisplayTime = (!this.m_showBonusAnims) ? this.m_noSpellDisplayTime : Mathf.Max(0f, this.m_noSpellDisplayTime - this.m_prebuffDisplayTime);
			yield return new WaitForSeconds(remainingDisplayTime);
			this.m_ritualActor.Destroy();
			this.FinishRitual();
		}
		yield break;
	}

	// Token: 0x060054F4 RID: 21748 RVA: 0x00197200 File Offset: 0x00195400
	private int FindLatestProxyCthunCreationTask()
	{
		int num = -1;
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		for (int i = 0; i < taskList.Count; i++)
		{
			PowerTask powerTask = taskList[i];
			Network.PowerHistory power = powerTask.GetPower();
			Network.PowerType type = power.Type;
			if (type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = (Network.HistTagChange)power;
				if (histTagChange.Tag == 434 && histTagChange.Value > 0 && i > num)
				{
					num = i;
				}
			}
			else if (type == Network.PowerType.FULL_ENTITY && i > num)
			{
				num = i;
			}
		}
		return num;
	}

	// Token: 0x060054F5 RID: 21749 RVA: 0x0019729A File Offset: 0x0019549A
	public void OnSpellFinished(Spell spell, object userData)
	{
		this.OnFinishedTaskList();
	}

	// Token: 0x060054F6 RID: 21750 RVA: 0x001972A4 File Offset: 0x001954A4
	public void OnSpellEvent(string eventName, object eventData, object userData)
	{
		if (eventName != "showCthun")
		{
			Debug.LogError("RitualSpellController received unexpected Spell Event " + eventName);
		}
		if (this.m_hideRitualActor)
		{
			this.m_ritualActor.Show();
			if (this.m_tauntSpellInstance != null)
			{
				this.m_tauntSpellInstance.Activate();
			}
		}
	}

	// Token: 0x060054F7 RID: 21751 RVA: 0x00197304 File Offset: 0x00195504
	private void OnRitualSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() == SpellStateType.NONE)
		{
			Actor actor = (Actor)userData;
			actor.Destroy();
			this.FinishRitual();
		}
	}

	// Token: 0x060054F8 RID: 21752 RVA: 0x0019732F File Offset: 0x0019552F
	private void FinishRitual()
	{
		this.m_finished = true;
		if (this.m_processingTaskList)
		{
			this.OnFinishedTaskList();
		}
		this.OnFinished();
	}

	// Token: 0x060054F9 RID: 21753 RVA: 0x00197350 File Offset: 0x00195550
	private Actor LoadRitualActor(Entity entity)
	{
		string zoneActor = ActorNames.GetZoneActor(entity, TAG_ZONE.PLAY);
		GameObject gameObject = AssetLoader.Get().LoadActor(zoneActor, false, false);
		if (gameObject == null)
		{
			Debug.LogWarning("RitualSpellController unable to load Ritual Actor GameObject.");
			return null;
		}
		Actor component = gameObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning("RitualSpellController Ritual Actor GameObject contains no Actor component.");
			Object.Destroy(gameObject);
			return null;
		}
		component.SetEntity(entity);
		component.SetCardDef(entity.GetCard().GetCardDef());
		return component;
	}

	// Token: 0x060054FA RID: 21754 RVA: 0x001973CC File Offset: 0x001955CC
	private void UpdateAndPositionRitualActor()
	{
		if (this.m_ritualActor.GetEntity().HasTag(GAME_TAG.TAUNT))
		{
			Spell spell = (this.m_ritualEntity.GetPremiumType() != TAG_PREMIUM.NORMAL) ? this.m_tauntInstantPremiumSpell : this.m_tauntInstantSpell;
			if (spell != null)
			{
				this.m_tauntSpellInstance = Object.Instantiate<Spell>(spell);
				TransformUtil.AttachAndPreserveLocalTransform(this.m_tauntSpellInstance.transform, this.m_ritualActor.transform);
				if (!this.m_hideRitualActor)
				{
					this.m_tauntSpellInstance.Activate();
				}
			}
			else
			{
				Debug.LogWarning("RitualSpellController does not have a instant taunt spell hooked up.");
			}
		}
		this.m_ritualActor.UpdateMinionStatsImmediately();
		if (this.m_hideRitualActor)
		{
			this.m_ritualActor.Hide();
		}
		string name = (this.m_ritualActor.GetEntity().GetControllerSide() != Player.Side.FRIENDLY) ? this.m_opponentRitualBoneName : this.m_friendlyRitualBoneName;
		Transform parent = Board.Get().FindBone(name);
		this.m_ritualActor.transform.parent = parent;
		this.m_ritualActor.transform.localPosition = Vector3.zero;
	}

	// Token: 0x060054FB RID: 21755 RVA: 0x001974E8 File Offset: 0x001956E8
	private bool IsCthunInPlay(Player player)
	{
		foreach (Card card in player.GetBattlefieldZone().GetCards())
		{
			if (card.GetController() == player && card.GetEntity().GetCardId() == "OG_280")
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04003B1C RID: 15132
	public Spell m_ritualSpell;

	// Token: 0x04003B1D RID: 15133
	public float m_noSpellDisplayTime = 3f;

	// Token: 0x04003B1E RID: 15134
	public string m_friendlyRitualBoneName = "FriendlyRitual";

	// Token: 0x04003B1F RID: 15135
	public string m_opponentRitualBoneName = "OpponentRitual";

	// Token: 0x04003B20 RID: 15136
	public bool m_hideRitualActor = true;

	// Token: 0x04003B21 RID: 15137
	public Spell m_tauntInstantSpell;

	// Token: 0x04003B22 RID: 15138
	public Spell m_tauntInstantPremiumSpell;

	// Token: 0x04003B23 RID: 15139
	public bool m_skipIfCthunInPlay;

	// Token: 0x04003B24 RID: 15140
	public bool m_showBonusAnims;

	// Token: 0x04003B25 RID: 15141
	public float m_prebuffDisplayTime = 1f;

	// Token: 0x04003B26 RID: 15142
	private Entity m_ritualEntity;

	// Token: 0x04003B27 RID: 15143
	private Entity m_ritualEntityClone;

	// Token: 0x04003B28 RID: 15144
	private bool m_finished;

	// Token: 0x04003B29 RID: 15145
	private Actor m_ritualActor;

	// Token: 0x04003B2A RID: 15146
	private Spell m_tauntSpellInstance;
}
