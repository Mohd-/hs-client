using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008C4 RID: 2244
public class AttackSpellController : SpellController
{
	// Token: 0x06005475 RID: 21621 RVA: 0x00193CC8 File Offset: 0x00191EC8
	protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
	{
		this.m_attackType = taskList.GetAttackType();
		if (this.m_attackType == AttackType.INVALID)
		{
			return false;
		}
		Entity attacker = taskList.GetAttacker();
		if (attacker != null)
		{
			base.SetSource(attacker.GetCard());
		}
		Entity defender = taskList.GetDefender();
		if (defender != null)
		{
			base.AddTarget(defender.GetCard());
		}
		return true;
	}

	// Token: 0x06005476 RID: 21622 RVA: 0x00193D24 File Offset: 0x00191F24
	protected override void OnProcessTaskList()
	{
		if (this.m_attackType == AttackType.ONLY_PROPOSED_ATTACKER || this.m_attackType == AttackType.ONLY_PROPOSED_DEFENDER || this.m_attackType == AttackType.ONLY_ATTACKER || this.m_attackType == AttackType.ONLY_DEFENDER || this.m_attackType == AttackType.WAITING_ON_PROPOSED_ATTACKER || this.m_attackType == AttackType.WAITING_ON_PROPOSED_DEFENDER || this.m_attackType == AttackType.WAITING_ON_ATTACKER || this.m_attackType == AttackType.WAITING_ON_DEFENDER)
		{
			this.FinishEverything();
			return;
		}
		Card source = base.GetSource();
		Entity entity = source.GetEntity();
		Zone zone = source.GetZone();
		bool flag = zone.m_Side == Player.Side.FRIENDLY;
		if (flag)
		{
			this.m_sourceAttackSpell = source.GetActorSpell(SpellType.FRIENDLY_ATTACK, true);
		}
		else
		{
			this.m_sourceAttackSpell = source.GetActorSpell(SpellType.OPPONENT_ATTACK, true);
		}
		if (this.m_attackType == AttackType.CANCELED)
		{
			if (this.m_sourceAttackSpell != null)
			{
				if (entity.IsHero())
				{
					this.m_sourceAttackSpell.ActivateState(SpellStateType.CANCEL);
				}
				else
				{
					this.m_sourceAttackSpell.ActivateState(SpellStateType.DEATH);
				}
			}
			source.SetDoNotSort(false);
			zone.UpdateLayout();
			source.EnableAttacking(false);
			this.FinishEverything();
			return;
		}
		source.EnableAttacking(true);
		if (entity.HasTag(GAME_TAG.IMMUNE_WHILE_ATTACKING))
		{
			source.ActivateActorSpell(SpellType.IMMUNE);
		}
		this.m_sourceAttackSpell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnSourceAttackStateStarted));
		if (flag)
		{
			if (this.m_sourceAttackSpell.GetActiveState() != SpellStateType.IDLE)
			{
				this.m_sourceAttackSpell.ActivateState(SpellStateType.BIRTH);
			}
			else
			{
				this.m_sourceAttackSpell.ActivateState(SpellStateType.ACTION);
			}
		}
		else
		{
			this.m_sourceAttackSpell.ActivateState(SpellStateType.BIRTH);
		}
	}

	// Token: 0x06005477 RID: 21623 RVA: 0x00193EC0 File Offset: 0x001920C0
	private void OnSourceAttackStateStarted(Spell spell, SpellStateType prevStateType, object userData)
	{
		SpellStateType activeState = spell.GetActiveState();
		if (activeState == SpellStateType.IDLE)
		{
			spell.ActivateState(SpellStateType.ACTION);
		}
		else if (activeState == SpellStateType.ACTION)
		{
			spell.RemoveStateStartedCallback(new Spell.StateStartedCallback(this.OnSourceAttackStateStarted));
			this.LaunchAttack();
		}
	}

	// Token: 0x06005478 RID: 21624 RVA: 0x00193F08 File Offset: 0x00192108
	private void LaunchAttack()
	{
		Card source = base.GetSource();
		Entity entity = source.GetEntity();
		Card target = base.GetTarget();
		bool flag = this.m_attackType == AttackType.PROPOSED;
		if (flag && entity.IsHero())
		{
			this.m_sourceAttackSpell.ActivateState(SpellStateType.IDLE);
			this.FinishEverything();
			return;
		}
		this.m_sourcePos = source.transform.position;
		this.m_sourceToTarget = target.transform.position - this.m_sourcePos;
		Vector3 impactPos = this.ComputeImpactPos();
		source.SetDoNotSort(true);
		this.MoveSourceToTarget(source, entity, impactPos);
		if (entity.IsHero())
		{
			this.OrientSourceHeroToTarget(source);
		}
		if (flag)
		{
			return;
		}
		target.SetDoNotSort(true);
		this.MoveTargetToSource(target, entity, impactPos);
	}

	// Token: 0x06005479 RID: 21625 RVA: 0x00193FC8 File Offset: 0x001921C8
	private void OnMoveToTargetFinished()
	{
		Card source = base.GetSource();
		Entity entity = source.GetEntity();
		Card target = base.GetTarget();
		bool flag = this.m_attackType == AttackType.PROPOSED;
		this.DoTasks(source, target);
		if (!flag)
		{
			this.ActivateImpactEffects(source, target);
		}
		if (entity.IsHero())
		{
			this.MoveSourceHeroBack(source);
			this.OrientSourceHeroBack(source);
			target.SetDoNotSort(false);
			Zone zone = target.GetZone();
			zone.UpdateLayout();
		}
		else if (flag)
		{
			this.FinishEverything();
		}
		else
		{
			source.SetDoNotSort(false);
			Zone zone2 = source.GetZone();
			zone2.UpdateLayout();
			target.SetDoNotSort(false);
			Zone zone3 = target.GetZone();
			zone3.UpdateLayout();
			this.m_sourceAttackSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnMinionSourceAttackStateFinished));
			this.m_sourceAttackSpell.ActivateState(SpellStateType.DEATH);
		}
	}

	// Token: 0x0600547A RID: 21626 RVA: 0x0019409E File Offset: 0x0019229E
	private void DoTasks(Card sourceCard, Card targetCard)
	{
		GameUtils.DoDamageTasks(this.m_taskList, sourceCard, targetCard);
	}

	// Token: 0x0600547B RID: 21627 RVA: 0x001940B0 File Offset: 0x001922B0
	private void MoveSourceHeroBack(Card sourceCard)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_sourcePos,
			"time",
			this.m_HeroInfo.m_MoveBackDuration,
			"easetype",
			this.m_HeroInfo.m_MoveBackEaseType,
			"oncomplete",
			"OnHeroMoveBackFinished",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(sourceCard.gameObject, args);
	}

	// Token: 0x0600547C RID: 21628 RVA: 0x00194144 File Offset: 0x00192344
	private void OrientSourceHeroBack(Card sourceCard)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			Quaternion.LookRotation(this.m_sourceFacing).eulerAngles,
			"time",
			this.m_HeroInfo.m_OrientBackDuration,
			"easetype",
			this.m_HeroInfo.m_OrientBackEaseType
		});
		iTween.RotateTo(sourceCard.gameObject, args);
	}

	// Token: 0x0600547D RID: 21629 RVA: 0x001941C4 File Offset: 0x001923C4
	private void OnHeroMoveBackFinished()
	{
		Card source = base.GetSource();
		Entity entity = source.GetEntity();
		source.SetDoNotSort(false);
		source.EnableAttacking(false);
		if (entity.GetController().IsLocalUser() || this.m_sourceAttackSpell.GetActiveState() == SpellStateType.NONE)
		{
			this.PlayWindfuryReminderIfPossible(entity, source);
			this.FinishEverything();
			return;
		}
		this.m_sourceAttackSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnHeroSourceAttackStateFinished));
	}

	// Token: 0x0600547E RID: 21630 RVA: 0x00194234 File Offset: 0x00192434
	private void OnHeroSourceAttackStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnHeroSourceAttackStateFinished));
		Card source = base.GetSource();
		Entity entity = source.GetEntity();
		this.PlayWindfuryReminderIfPossible(entity, source);
		this.FinishEverything();
	}

	// Token: 0x0600547F RID: 21631 RVA: 0x0019427C File Offset: 0x0019247C
	private void OnMinionSourceAttackStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnMinionSourceAttackStateFinished));
		Card source = base.GetSource();
		Entity entity = source.GetEntity();
		source.EnableAttacking(false);
		if (!this.CanPlayWindfuryReminder(entity, source))
		{
			this.FinishEverything();
		}
		else
		{
			this.OnFinishedTaskList();
			base.StartCoroutine(this.WaitThenPlayWindfuryReminder(entity, source));
		}
	}

	// Token: 0x06005480 RID: 21632 RVA: 0x001942EC File Offset: 0x001924EC
	private void FinishEverything()
	{
		Card source = base.GetSource();
		Entity entity = source.GetEntity();
		if (entity.HasTag(GAME_TAG.IMMUNE_WHILE_ATTACKING) && !entity.IsImmune())
		{
			source.GetActor().DeactivateSpell(SpellType.IMMUNE);
		}
		this.OnFinishedTaskList();
		this.OnFinished();
	}

	// Token: 0x06005481 RID: 21633 RVA: 0x0019433C File Offset: 0x0019253C
	private IEnumerator WaitThenPlayWindfuryReminder(Entity entity, Card card)
	{
		yield return new WaitForSeconds(1.2f);
		this.PlayWindfuryReminderIfPossible(entity, card);
		this.OnFinished();
		yield break;
	}

	// Token: 0x06005482 RID: 21634 RVA: 0x00194374 File Offset: 0x00192574
	private bool CanPlayWindfuryReminder(Entity entity, Card card)
	{
		if (!entity.HasWindfury())
		{
			return false;
		}
		if (entity.IsExhausted())
		{
			return false;
		}
		if (entity.GetZone() != TAG_ZONE.PLAY)
		{
			return false;
		}
		Player controller = entity.GetController();
		if (!controller.IsCurrentPlayer())
		{
			return false;
		}
		Spell actorSpell = card.GetActorSpell(SpellType.WINDFURY_BURST, true);
		return !(actorSpell == null);
	}

	// Token: 0x06005483 RID: 21635 RVA: 0x001943D6 File Offset: 0x001925D6
	private void PlayWindfuryReminderIfPossible(Entity entity, Card card)
	{
		if (!this.CanPlayWindfuryReminder(entity, card))
		{
			return;
		}
		card.ActivateActorSpell(SpellType.WINDFURY_BURST);
	}

	// Token: 0x06005484 RID: 21636 RVA: 0x001943F0 File Offset: 0x001925F0
	private void MoveSourceToTarget(Card sourceCard, Entity sourceEntity, Vector3 impactPos)
	{
		Vector3 vector = this.ComputeImpactOffset(sourceCard, impactPos);
		Vector3 vector2 = impactPos + vector;
		float moveToTargetDuration;
		iTween.EaseType moveToTargetEaseType;
		if (sourceEntity.IsHero())
		{
			moveToTargetDuration = this.m_HeroInfo.m_MoveToTargetDuration;
			moveToTargetEaseType = this.m_HeroInfo.m_MoveToTargetEaseType;
		}
		else
		{
			moveToTargetDuration = this.m_AllyInfo.m_MoveToTargetDuration;
			moveToTargetEaseType = this.m_AllyInfo.m_MoveToTargetEaseType;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			vector2,
			"time",
			moveToTargetDuration,
			"easetype",
			moveToTargetEaseType,
			"oncomplete",
			"OnMoveToTargetFinished",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(sourceCard.gameObject, args);
	}

	// Token: 0x06005485 RID: 21637 RVA: 0x001944C8 File Offset: 0x001926C8
	private void OrientSourceHeroToTarget(Card sourceCard)
	{
		this.m_sourceFacing = sourceCard.transform.forward;
		Quaternion quaternion;
		if (this.m_sourceAttackSpell.GetSpellType() == SpellType.OPPONENT_ATTACK)
		{
			quaternion = Quaternion.LookRotation(-this.m_sourceToTarget);
		}
		else
		{
			quaternion = Quaternion.LookRotation(this.m_sourceToTarget);
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"rotation",
			quaternion.eulerAngles,
			"time",
			this.m_HeroInfo.m_OrientToTargetDuration,
			"easetype",
			this.m_HeroInfo.m_OrientToTargetEaseType
		});
		iTween.RotateTo(sourceCard.gameObject, args);
	}

	// Token: 0x06005486 RID: 21638 RVA: 0x00194580 File Offset: 0x00192780
	private void MoveTargetToSource(Card targetCard, Entity sourceEntity, Vector3 impactPos)
	{
		float moveToTargetDuration;
		iTween.EaseType moveToTargetEaseType;
		if (sourceEntity.IsHero())
		{
			moveToTargetDuration = this.m_HeroInfo.m_MoveToTargetDuration;
			moveToTargetEaseType = this.m_HeroInfo.m_MoveToTargetEaseType;
		}
		else
		{
			moveToTargetDuration = this.m_AllyInfo.m_MoveToTargetDuration;
			moveToTargetEaseType = this.m_AllyInfo.m_MoveToTargetEaseType;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			impactPos,
			"time",
			moveToTargetDuration,
			"easetype",
			moveToTargetEaseType
		});
		iTween.MoveTo(targetCard.gameObject, args);
	}

	// Token: 0x06005487 RID: 21639 RVA: 0x00194624 File Offset: 0x00192824
	private Vector3 ComputeImpactPos()
	{
		float num = 1f;
		if (this.m_attackType == AttackType.PROPOSED)
		{
			num = 0.5f;
		}
		Vector3 vector = num * this.m_ImpactStagingPoint * this.m_sourceToTarget;
		return this.m_sourcePos + vector;
	}

	// Token: 0x06005488 RID: 21640 RVA: 0x0019466C File Offset: 0x0019286C
	private Vector3 ComputeImpactOffset(Card sourceCard, Vector3 impactPos)
	{
		if (object.Equals(this.m_SourceImpactOffset, 0.5f))
		{
			return Vector3.zero;
		}
		Bounds bounds = sourceCard.GetActor().GetMeshRenderer().bounds;
		bounds.center = this.m_sourcePos;
		Ray ray;
		ray..ctor(impactPos, bounds.center - impactPos);
		float num;
		if (!bounds.IntersectRay(ray, ref num))
		{
			return Vector3.zero;
		}
		Vector3 vector = ray.origin + num * ray.direction;
		Vector3 vector2 = 2f * bounds.center - vector;
		Vector3 vector3 = vector2 - vector;
		Vector3 vector4 = 0.5f * vector3;
		return vector4 - this.m_SourceImpactOffset * vector3;
	}

	// Token: 0x06005489 RID: 21641 RVA: 0x00194748 File Offset: 0x00192948
	private void ActivateImpactEffects(Card sourceCard, Card targetCard)
	{
		Spell spell = this.DetermineImpactSpellPrefab(sourceCard);
		if (spell == null)
		{
			return;
		}
		Spell spell2 = Object.Instantiate<Spell>(spell);
		spell2.SetSource(sourceCard.gameObject);
		spell2.AddTarget(targetCard.gameObject);
		Vector3 position = targetCard.transform.position;
		spell2.SetPosition(position);
		Quaternion orientation = Quaternion.LookRotation(this.m_sourceToTarget);
		spell2.SetOrientation(orientation);
		spell2.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnImpactSpellStateFinished));
		spell2.Activate();
	}

	// Token: 0x0600548A RID: 21642 RVA: 0x001947C8 File Offset: 0x001929C8
	private Spell DetermineImpactSpellPrefab(Card sourceCard)
	{
		if (this.m_ImpactDefs == null)
		{
			return this.m_DefaultImpactSpellPrefab;
		}
		int atk = sourceCard.GetEntity().GetATK();
		for (int i = 0; i < this.m_ImpactDefs.Count; i++)
		{
			AttackImpactDef attackImpactDef = this.m_ImpactDefs[i];
			if (attackImpactDef.m_MinAttack <= (float)atk)
			{
				if (attackImpactDef.m_MaxAttack >= (float)atk)
				{
					if (!(attackImpactDef.m_SpellPrefab == null))
					{
						return attackImpactDef.m_SpellPrefab;
					}
				}
			}
		}
		return this.m_DefaultImpactSpellPrefab;
	}

	// Token: 0x0600548B RID: 21643 RVA: 0x00194863 File Offset: 0x00192A63
	private void OnImpactSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		Object.Destroy(spell.gameObject);
	}

	// Token: 0x04003AE1 RID: 15073
	private const float PROPOSED_ATTACK_IMPACT_POINT_SCALAR = 0.5f;

	// Token: 0x04003AE2 RID: 15074
	private const float WINDFURY_REMINDER_WAIT_SEC = 1.2f;

	// Token: 0x04003AE3 RID: 15075
	public HeroAttackDef m_HeroInfo;

	// Token: 0x04003AE4 RID: 15076
	public AllyAttackDef m_AllyInfo;

	// Token: 0x04003AE5 RID: 15077
	public float m_ImpactStagingPoint = 1f;

	// Token: 0x04003AE6 RID: 15078
	public float m_SourceImpactOffset = -0.25f;

	// Token: 0x04003AE7 RID: 15079
	public List<AttackImpactDef> m_ImpactDefs;

	// Token: 0x04003AE8 RID: 15080
	public Spell m_DefaultImpactSpellPrefab;

	// Token: 0x04003AE9 RID: 15081
	private AttackType m_attackType;

	// Token: 0x04003AEA RID: 15082
	private Spell m_sourceAttackSpell;

	// Token: 0x04003AEB RID: 15083
	private Vector3 m_sourcePos;

	// Token: 0x04003AEC RID: 15084
	private Vector3 m_sourceToTarget;

	// Token: 0x04003AED RID: 15085
	private Vector3 m_sourceFacing;
}
