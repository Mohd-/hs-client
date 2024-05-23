using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020003E3 RID: 995
public class SuperSpell : Spell
{
	// Token: 0x06003308 RID: 13064 RVA: 0x000FDF5B File Offset: 0x000FC15B
	public override List<GameObject> GetVisualTargets()
	{
		return this.m_visualTargets;
	}

	// Token: 0x06003309 RID: 13065 RVA: 0x000FDF64 File Offset: 0x000FC164
	public override GameObject GetVisualTarget()
	{
		return (this.m_visualTargets.Count != 0) ? this.m_visualTargets[0] : null;
	}

	// Token: 0x0600330A RID: 13066 RVA: 0x000FDF93 File Offset: 0x000FC193
	public override void AddVisualTarget(GameObject go)
	{
		this.m_visualTargets.Add(go);
	}

	// Token: 0x0600330B RID: 13067 RVA: 0x000FDFA1 File Offset: 0x000FC1A1
	public override void AddVisualTargets(List<GameObject> targets)
	{
		this.m_visualTargets.AddRange(targets);
	}

	// Token: 0x0600330C RID: 13068 RVA: 0x000FDFAF File Offset: 0x000FC1AF
	public override bool RemoveVisualTarget(GameObject go)
	{
		return this.m_visualTargets.Remove(go);
	}

	// Token: 0x0600330D RID: 13069 RVA: 0x000FDFBD File Offset: 0x000FC1BD
	public override void RemoveAllVisualTargets()
	{
		this.m_visualTargets.Clear();
	}

	// Token: 0x0600330E RID: 13070 RVA: 0x000FDFCA File Offset: 0x000FC1CA
	public override bool IsVisualTarget(GameObject go)
	{
		return this.m_visualTargets.Contains(go);
	}

	// Token: 0x0600330F RID: 13071 RVA: 0x000FDFD8 File Offset: 0x000FC1D8
	public override Card GetVisualTargetCard()
	{
		GameObject visualTarget = this.GetVisualTarget();
		if (visualTarget == null)
		{
			return null;
		}
		return visualTarget.GetComponent<Card>();
	}

	// Token: 0x06003310 RID: 13072 RVA: 0x000FE000 File Offset: 0x000FC200
	public override bool AddPowerTargets()
	{
		this.m_visualToTargetIndexMap.Clear();
		this.m_targetToMetaDataMap.Clear();
		if (!base.CanAddPowerTargets())
		{
			return false;
		}
		if (this.m_TargetInfo.m_OnlyAddMetaDataTargets)
		{
			List<PowerTask> taskList = this.m_taskList.GetTaskList();
			base.AddMultiplePowerTargets_FromMetaData(taskList);
			return true;
		}
		if (this.HasChain() && !this.AddPrimaryChainTarget())
		{
			return false;
		}
		if (!base.AddMultiplePowerTargets())
		{
			return false;
		}
		if (this.m_targets.Count > 0)
		{
			return true;
		}
		Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
		return blockStart == null || blockStart.Target == 0 || base.AddSinglePowerTarget_FromBlockStart(blockStart);
	}

	// Token: 0x06003311 RID: 13073 RVA: 0x000FE0B4 File Offset: 0x000FC2B4
	protected override void AddTargetFromMetaData(int metaDataIndex, Card targetCard)
	{
		int count = this.m_targets.Count;
		this.m_targetToMetaDataMap[count] = metaDataIndex;
		this.AddTarget(targetCard.gameObject);
	}

	// Token: 0x06003312 RID: 13074 RVA: 0x000FE0E8 File Offset: 0x000FC2E8
	protected override void OnBirth(SpellStateType prevStateType)
	{
		base.UpdatePosition();
		base.UpdateOrientation();
		this.m_currentTargetIndex = 0;
		if (this.HasStart())
		{
			this.SpawnStart();
			this.m_startSpell.SafeActivateState(SpellStateType.BIRTH);
			if (this.m_startSpell.GetActiveState() == SpellStateType.NONE)
			{
				this.m_startSpell = null;
			}
		}
		base.OnBirth(prevStateType);
	}

	// Token: 0x06003313 RID: 13075 RVA: 0x000FE144 File Offset: 0x000FC344
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_settingUpAction = true;
		this.UpdateTargets();
		base.UpdatePosition();
		base.UpdateOrientation();
		this.m_currentTargetIndex = this.GetPrimaryTargetIndex();
		this.UpdatePendingStateChangeFlags(SpellStateType.ACTION);
		this.DoAction();
		base.OnAction(prevStateType);
		this.m_settingUpAction = false;
		this.FinishIfPossible();
	}

	// Token: 0x06003314 RID: 13076 RVA: 0x000FE198 File Offset: 0x000FC398
	protected override void OnCancel(SpellStateType prevStateType)
	{
		this.UpdatePendingStateChangeFlags(SpellStateType.CANCEL);
		if (this.m_startSpell != null)
		{
			this.m_startSpell.SafeActivateState(SpellStateType.CANCEL);
			this.m_startSpell = null;
		}
		base.OnCancel(prevStateType);
		this.FinishIfPossible();
	}

	// Token: 0x06003315 RID: 13077 RVA: 0x000FE1E0 File Offset: 0x000FC3E0
	public override void OnStateFinished()
	{
		if (base.GuessNextStateType() == SpellStateType.NONE && this.AreEffectsActive())
		{
			this.m_pendingNoneStateChange = true;
			return;
		}
		base.OnStateFinished();
	}

	// Token: 0x06003316 RID: 13078 RVA: 0x000FE213 File Offset: 0x000FC413
	public override void OnSpellFinished()
	{
		if (this.AreEffectsActive())
		{
			this.m_pendingSpellFinish = true;
			return;
		}
		base.OnSpellFinished();
	}

	// Token: 0x06003317 RID: 13079 RVA: 0x000FE22E File Offset: 0x000FC42E
	public override void OnFsmStateStarted(FsmState state, SpellStateType stateType)
	{
		if (this.m_activeStateChange == stateType)
		{
			return;
		}
		if (stateType == SpellStateType.NONE && this.AreEffectsActive())
		{
			this.m_pendingSpellFinish = true;
			this.m_pendingNoneStateChange = true;
			return;
		}
		base.OnFsmStateStarted(state, stateType);
	}

	// Token: 0x06003318 RID: 13080 RVA: 0x000FE265 File Offset: 0x000FC465
	private void DoAction()
	{
		if (this.CheckAndWaitForGameEventsThenDoAction())
		{
			return;
		}
		if (this.CheckAndWaitForStartDelayThenDoAction())
		{
			return;
		}
		if (this.CheckAndWaitForStartPrefabThenDoAction())
		{
			return;
		}
		this.DoActionNow();
	}

	// Token: 0x06003319 RID: 13081 RVA: 0x000FE294 File Offset: 0x000FC494
	private bool CheckAndWaitForGameEventsThenDoAction()
	{
		if (this.m_taskList == null)
		{
			return false;
		}
		if (this.m_ActionInfo.m_ShowSpellVisuals == SpellVisualShowTime.DURING_GAME_EVENTS)
		{
			return this.DoActionDuringGameEvents();
		}
		if (this.m_ActionInfo.m_ShowSpellVisuals == SpellVisualShowTime.AFTER_GAME_EVENTS)
		{
			this.DoActionAfterGameEvents();
			return true;
		}
		return false;
	}

	// Token: 0x0600331A RID: 13082 RVA: 0x000FE2E0 File Offset: 0x000FC4E0
	private bool DoActionDuringGameEvents()
	{
		this.m_taskList.DoAllTasks();
		if (this.m_taskList.IsComplete())
		{
			return false;
		}
		QueueList<PowerTask> queueList = this.DetermineTasksToWaitFor(0, this.m_taskList.GetTaskList().Count);
		if (queueList.Count == 0)
		{
			return false;
		}
		base.StartCoroutine(this.DoDelayedActionDuringGameEvents(queueList));
		return true;
	}

	// Token: 0x0600331B RID: 13083 RVA: 0x000FE340 File Offset: 0x000FC540
	private IEnumerator DoDelayedActionDuringGameEvents(QueueList<PowerTask> tasksToWaitFor)
	{
		this.m_effectsPendingFinish++;
		yield return base.StartCoroutine(this.WaitForTasks(tasksToWaitFor));
		this.m_effectsPendingFinish--;
		if (this.CheckAndWaitForStartDelayThenDoAction())
		{
			yield break;
		}
		if (this.CheckAndWaitForStartPrefabThenDoAction())
		{
			yield break;
		}
		this.DoActionNow();
		yield break;
	}

	// Token: 0x0600331C RID: 13084 RVA: 0x000FE36C File Offset: 0x000FC56C
	private Entity GetEntityFromZoneChangePowerTask(PowerTask task)
	{
		Entity result;
		int num;
		this.GetZoneChangeFromPowerTask(task, out result, out num);
		return result;
	}

	// Token: 0x0600331D RID: 13085 RVA: 0x000FE388 File Offset: 0x000FC588
	private bool GetZoneChangeFromPowerTask(PowerTask task, out Entity entity, out int zoneTag)
	{
		entity = null;
		zoneTag = 0;
		Network.PowerHistory power = task.GetPower();
		switch (power.Type)
		{
		case Network.PowerType.FULL_ENTITY:
		{
			Network.HistFullEntity histFullEntity = (Network.HistFullEntity)power;
			Entity entity2 = GameState.Get().GetEntity(histFullEntity.Entity.ID);
			if (entity2.GetCard() == null)
			{
				return false;
			}
			foreach (Network.Entity.Tag tag in histFullEntity.Entity.Tags)
			{
				if (tag.Name == 49)
				{
					entity = entity2;
					zoneTag = tag.Value;
					return true;
				}
			}
			break;
		}
		case Network.PowerType.SHOW_ENTITY:
		{
			Network.HistShowEntity histShowEntity = (Network.HistShowEntity)power;
			Entity entity2 = GameState.Get().GetEntity(histShowEntity.Entity.ID);
			if (entity2.GetCard() == null)
			{
				return false;
			}
			foreach (Network.Entity.Tag tag2 in histShowEntity.Entity.Tags)
			{
				if (tag2.Name == 49)
				{
					entity = entity2;
					zoneTag = tag2.Value;
					return true;
				}
			}
			break;
		}
		case Network.PowerType.TAG_CHANGE:
		{
			Network.HistTagChange histTagChange = (Network.HistTagChange)power;
			Entity entity2 = GameState.Get().GetEntity(histTagChange.Entity);
			if (entity2.GetCard() == null)
			{
				return false;
			}
			if (histTagChange.Tag == 49)
			{
				entity = entity2;
				zoneTag = histTagChange.Value;
				return true;
			}
			break;
		}
		}
		return false;
	}

	// Token: 0x0600331E RID: 13086 RVA: 0x000FE564 File Offset: 0x000FC764
	private void DoActionAfterGameEvents()
	{
		this.m_effectsPendingFinish++;
		PowerTaskList.CompleteCallback callback = delegate(PowerTaskList taskList, int startIndex, int count, object userData)
		{
			this.m_effectsPendingFinish--;
			if (this.CheckAndWaitForStartDelayThenDoAction())
			{
				return;
			}
			if (this.CheckAndWaitForStartPrefabThenDoAction())
			{
				return;
			}
			this.DoActionNow();
		};
		this.m_taskList.DoAllTasks(callback);
	}

	// Token: 0x0600331F RID: 13087 RVA: 0x000FE598 File Offset: 0x000FC798
	private bool CheckAndWaitForStartDelayThenDoAction()
	{
		if (Mathf.Min(this.m_ActionInfo.m_StartDelayMax, this.m_ActionInfo.m_StartDelayMin) <= Mathf.Epsilon)
		{
			return false;
		}
		this.m_effectsPendingFinish++;
		base.StartCoroutine(this.WaitForStartDelayThenDoAction());
		return true;
	}

	// Token: 0x06003320 RID: 13088 RVA: 0x000FE5E8 File Offset: 0x000FC7E8
	private IEnumerator WaitForStartDelayThenDoAction()
	{
		float delaySec = Random.Range(this.m_ActionInfo.m_StartDelayMin, this.m_ActionInfo.m_StartDelayMax);
		yield return new WaitForSeconds(delaySec);
		this.m_effectsPendingFinish--;
		if (this.CheckAndWaitForStartPrefabThenDoAction())
		{
			yield break;
		}
		this.DoActionNow();
		yield break;
	}

	// Token: 0x06003321 RID: 13089 RVA: 0x000FE604 File Offset: 0x000FC804
	private bool CheckAndWaitForStartPrefabThenDoAction()
	{
		if (!this.HasStart())
		{
			return false;
		}
		if (this.m_startSpell != null && this.m_startSpell.GetActiveState() == SpellStateType.IDLE)
		{
			return false;
		}
		if (this.m_startSpell == null)
		{
			this.SpawnStart();
		}
		this.m_startSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnStartSpellBirthStateFinished));
		if (this.m_startSpell.GetActiveState() != SpellStateType.BIRTH)
		{
			this.m_startSpell.SafeActivateState(SpellStateType.BIRTH);
			if (this.m_startSpell.GetActiveState() == SpellStateType.NONE)
			{
				this.m_startSpell = null;
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003322 RID: 13090 RVA: 0x000FE6A8 File Offset: 0x000FC8A8
	private void OnStartSpellBirthStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (prevStateType != SpellStateType.BIRTH)
		{
			return;
		}
		spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnStartSpellBirthStateFinished), userData);
		this.DoActionNow();
	}

	// Token: 0x06003323 RID: 13091 RVA: 0x000FE6D8 File Offset: 0x000FC8D8
	protected virtual void DoActionNow()
	{
		SpellAreaEffectInfo spellAreaEffectInfo = this.DetermineAreaEffectInfo();
		if (spellAreaEffectInfo != null)
		{
			this.SpawnAreaEffect(spellAreaEffectInfo);
		}
		bool flag = this.HasMissile();
		bool flag2 = this.HasImpact();
		bool flag3 = this.HasChain();
		if (this.GetVisualTargetCount() > 0 && (flag || flag2 || flag3))
		{
			if (flag)
			{
				if (flag3)
				{
					this.SpawnChainMissile();
				}
				else if (this.m_MissileInfo.m_SpawnInSequence)
				{
					this.SpawnMissileInSequence();
				}
				else
				{
					this.SpawnAllMissiles();
				}
			}
			else
			{
				if (flag2)
				{
					if (flag3)
					{
						this.SpawnImpact(this.m_currentTargetIndex);
					}
					else
					{
						this.SpawnAllImpacts();
					}
				}
				if (flag3)
				{
					this.SpawnChain();
				}
				this.DoStartSpellAction();
			}
		}
		else
		{
			this.DoStartSpellAction();
		}
		this.FinishIfPossible();
	}

	// Token: 0x06003324 RID: 13092 RVA: 0x000FE7AF File Offset: 0x000FC9AF
	private bool HasStart()
	{
		return this.m_StartInfo != null && this.m_StartInfo.m_Enabled && this.m_StartInfo.m_Prefab != null;
	}

	// Token: 0x06003325 RID: 13093 RVA: 0x000FE7E0 File Offset: 0x000FC9E0
	private void SpawnStart()
	{
		this.m_effectsPendingFinish++;
		this.m_startSpell = this.CloneSpell(this.m_StartInfo.m_Prefab);
		this.m_startSpell.SetSource(base.GetSource());
		this.m_startSpell.AddTargets(base.GetTargets());
		if (this.m_StartInfo.m_UseSuperSpellLocation)
		{
			this.m_startSpell.SetPosition(base.transform.position);
		}
	}

	// Token: 0x06003326 RID: 13094 RVA: 0x000FE85C File Offset: 0x000FCA5C
	private void DoStartSpellAction()
	{
		if (this.m_startSpell == null)
		{
			return;
		}
		if (!this.m_startSpell.HasUsableState(SpellStateType.ACTION))
		{
			this.m_startSpell.UpdateTransform();
			this.m_startSpell.SafeActivateState(SpellStateType.DEATH);
		}
		else
		{
			this.m_startSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnStartSpellActionFinished));
			this.m_startSpell.ActivateState(SpellStateType.ACTION);
		}
		this.m_startSpell = null;
	}

	// Token: 0x06003327 RID: 13095 RVA: 0x000FE8D2 File Offset: 0x000FCAD2
	private void OnStartSpellActionFinished(Spell spell, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.ACTION)
		{
			return;
		}
		spell.SafeActivateState(SpellStateType.DEATH);
	}

	// Token: 0x06003328 RID: 13096 RVA: 0x000FE8E8 File Offset: 0x000FCAE8
	private bool HasMissile()
	{
		return this.m_MissileInfo != null && this.m_MissileInfo.m_Enabled && (this.m_MissileInfo.m_Prefab != null || this.m_MissileInfo.m_ReversePrefab != null);
	}

	// Token: 0x06003329 RID: 13097 RVA: 0x000FE93D File Offset: 0x000FCB3D
	private void SpawnChainMissile()
	{
		this.SpawnMissile(this.GetPrimaryTargetIndex());
		this.DoStartSpellAction();
	}

	// Token: 0x0600332A RID: 13098 RVA: 0x000FE954 File Offset: 0x000FCB54
	private void SpawnMissileInSequence()
	{
		if (this.m_currentTargetIndex >= this.GetVisualTargetCount())
		{
			return;
		}
		this.SpawnMissile(this.m_currentTargetIndex);
		this.m_currentTargetIndex++;
		if (this.m_startSpell == null)
		{
			return;
		}
		if (this.m_StartInfo.m_DeathAfterAllMissilesFire)
		{
			if (this.m_currentTargetIndex < this.GetVisualTargetCount())
			{
				if (this.m_startSpell.HasUsableState(SpellStateType.ACTION))
				{
					this.m_startSpell.ActivateState(SpellStateType.ACTION);
				}
			}
			else
			{
				this.DoStartSpellAction();
			}
		}
		else
		{
			this.DoStartSpellAction();
		}
	}

	// Token: 0x0600332B RID: 13099 RVA: 0x000FE9F4 File Offset: 0x000FCBF4
	private void SpawnAllMissiles()
	{
		for (int i = 0; i < this.GetVisualTargetCount(); i++)
		{
			this.SpawnMissile(i);
		}
		this.DoStartSpellAction();
	}

	// Token: 0x0600332C RID: 13100 RVA: 0x000FEA25 File Offset: 0x000FCC25
	private void SpawnMissile(int targetIndex)
	{
		this.m_effectsPendingFinish++;
		base.StartCoroutine(this.WaitAndSpawnMissile(targetIndex));
	}

	// Token: 0x0600332D RID: 13101 RVA: 0x000FEA44 File Offset: 0x000FCC44
	private IEnumerator WaitAndSpawnMissile(int targetIndex)
	{
		float spawnDelaySec = Random.Range(this.m_MissileInfo.m_SpawnDelaySecMin, this.m_MissileInfo.m_SpawnDelaySecMax);
		if (!this.m_MissileInfo.m_SpawnInSequence || targetIndex == 0)
		{
			yield return new WaitForSeconds(spawnDelaySec);
		}
		int metaDataIndex = this.GetMetaDataIndexForTarget(targetIndex);
		if (this.ShouldCompleteTasksUntilMetaData(metaDataIndex))
		{
			yield return base.StartCoroutine(this.CompleteTasksUntilMetaData(metaDataIndex));
		}
		GameObject sourceObject = base.GetSource();
		GameObject targetObject = this.m_visualTargets[targetIndex];
		if (this.m_MissileInfo.m_Prefab != null)
		{
			Spell missile = this.CloneSpell(this.m_MissileInfo.m_Prefab);
			missile.SetSource(sourceObject);
			missile.AddTarget(targetObject);
			if (this.m_MissileInfo.m_UseSuperSpellLocation)
			{
				missile.SetPosition(base.transform.position);
			}
			missile.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnMissileSpellStateFinished), targetIndex);
			missile.ActivateState(SpellStateType.BIRTH);
		}
		else
		{
			this.m_effectsPendingFinish--;
		}
		if (this.m_MissileInfo.m_ReversePrefab != null)
		{
			this.m_effectsPendingFinish++;
			base.StartCoroutine(this.SpawnReverseMissile(this.m_MissileInfo.m_ReversePrefab, targetObject, sourceObject, this.m_MissileInfo.m_reverseDelay));
		}
		yield break;
	}

	// Token: 0x0600332E RID: 13102 RVA: 0x000FEA70 File Offset: 0x000FCC70
	private IEnumerator SpawnReverseMissile(Spell cloneSpell, GameObject sourceObject, GameObject targetObject, float delay)
	{
		yield return new WaitForSeconds(delay);
		Spell additionalMissile = this.CloneSpell(cloneSpell);
		additionalMissile.SetSource(sourceObject);
		additionalMissile.AddTarget(targetObject);
		additionalMissile.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnMissileSpellStateFinished), -1);
		additionalMissile.ActivateState(SpellStateType.BIRTH);
		yield break;
	}

	// Token: 0x0600332F RID: 13103 RVA: 0x000FEAC8 File Offset: 0x000FCCC8
	private void OnMissileSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (prevStateType != SpellStateType.BIRTH)
		{
			return;
		}
		spell.RemoveStateFinishedCallback(new Spell.StateFinishedCallback(this.OnMissileSpellStateFinished), userData);
		int num = (int)userData;
		bool reverse = num < 0;
		this.FireMissileOnPath(spell, num, reverse);
	}

	// Token: 0x06003330 RID: 13104 RVA: 0x000FEB08 File Offset: 0x000FCD08
	private void FireMissileOnPath(Spell missile, int targetIndex, bool reverse)
	{
		Vector3[] array = this.GenerateMissilePath(missile);
		float num = Random.Range(this.m_MissileInfo.m_PathDurationMin, this.m_MissileInfo.m_PathDurationMax);
		Hashtable hashtable = iTween.Hash(new object[]
		{
			"path",
			array,
			"time",
			num,
			"easetype",
			this.m_MissileInfo.m_PathEaseType,
			"oncompletetarget",
			base.gameObject
		});
		if (reverse)
		{
			hashtable.Add("oncomplete", "OnReverseMissileTargetReached");
			hashtable.Add("oncompleteparams", missile);
		}
		else
		{
			Hashtable hashtable2 = iTween.Hash(new object[]
			{
				"missile",
				missile,
				"targetIndex",
				targetIndex
			});
			hashtable.Add("oncomplete", "OnMissileTargetReached");
			hashtable.Add("oncompleteparams", hashtable2);
		}
		if (!object.Equals(array[0], array[2]))
		{
			hashtable.Add("orienttopath", this.m_MissileInfo.m_OrientToPath);
		}
		if (this.m_MissileInfo.m_TargetJoint.Length > 0)
		{
			GameObject gameObject = SceneUtils.FindChildBySubstring(missile.gameObject, this.m_MissileInfo.m_TargetJoint);
			if (gameObject != null)
			{
				missile.transform.LookAt(missile.GetTarget().transform, this.m_MissileInfo.m_JointUpVector);
				Vector3[] array2 = array;
				int num2 = 2;
				array2[num2].y = array2[num2].y + this.m_MissileInfo.m_TargetHeightOffset;
				iTween.MoveTo(gameObject, hashtable);
				return;
			}
		}
		iTween.MoveTo(missile.gameObject, hashtable);
	}

	// Token: 0x06003331 RID: 13105 RVA: 0x000FECD0 File Offset: 0x000FCED0
	private Vector3[] GenerateMissilePath(Spell missile)
	{
		Vector3[] array = new Vector3[]
		{
			missile.transform.position,
			default(Vector3),
			missile.GetTarget().transform.position
		};
		array[1] = this.GenerateMissilePathCenterPoint(array);
		return array;
	}

	// Token: 0x06003332 RID: 13106 RVA: 0x000FED2C File Offset: 0x000FCF2C
	private Vector3 GenerateMissilePathCenterPoint(Vector3[] path)
	{
		Vector3 vector = path[0];
		Vector3 vector2 = path[2];
		Vector3 vector3 = vector2 - vector;
		float magnitude = vector3.magnitude;
		Vector3 result = vector;
		bool flag = magnitude <= Mathf.Epsilon;
		if (!flag)
		{
			result = vector + vector3 * (this.m_MissileInfo.m_CenterOffsetPercent * 0.01f);
		}
		float num = magnitude / this.m_MissileInfo.m_DistanceScaleFactor;
		if (flag)
		{
			if (this.m_MissileInfo.m_CenterPointHeightMin <= Mathf.Epsilon && this.m_MissileInfo.m_CenterPointHeightMax <= Mathf.Epsilon)
			{
				result.y += 2f;
			}
			else
			{
				result.y += Random.Range(this.m_MissileInfo.m_CenterPointHeightMin, this.m_MissileInfo.m_CenterPointHeightMax);
			}
		}
		else
		{
			result.y += num * Random.Range(this.m_MissileInfo.m_CenterPointHeightMin, this.m_MissileInfo.m_CenterPointHeightMax);
		}
		float num2 = 1f;
		if (vector.z > vector2.z)
		{
			num2 = -1f;
		}
		bool flag2 = GeneralUtils.RandomBool();
		if (this.m_MissileInfo.m_RightMin == 0f && this.m_MissileInfo.m_RightMax == 0f)
		{
			flag2 = false;
		}
		if (this.m_MissileInfo.m_LeftMin == 0f && this.m_MissileInfo.m_LeftMax == 0f)
		{
			flag2 = true;
		}
		if (flag2)
		{
			if (this.m_MissileInfo.m_RightMin == this.m_MissileInfo.m_RightMax || this.m_MissileInfo.m_DebugForceMax)
			{
				result.x += this.m_MissileInfo.m_RightMax * num * num2;
			}
			else
			{
				result.x += Random.Range(this.m_MissileInfo.m_RightMin * num, this.m_MissileInfo.m_RightMax * num) * num2;
			}
		}
		else if (this.m_MissileInfo.m_LeftMin == this.m_MissileInfo.m_LeftMax || this.m_MissileInfo.m_DebugForceMax)
		{
			result.x -= this.m_MissileInfo.m_LeftMax * num * num2;
		}
		else
		{
			result.x -= Random.Range(this.m_MissileInfo.m_LeftMin * num, this.m_MissileInfo.m_LeftMax * num) * num2;
		}
		return result;
	}

	// Token: 0x06003333 RID: 13107 RVA: 0x000FEFE0 File Offset: 0x000FD1E0
	private void OnMissileTargetReached(Hashtable args)
	{
		Spell spell = (Spell)args["missile"];
		int targetIndex = (int)args["targetIndex"];
		if (this.HasImpact())
		{
			this.SpawnImpact(targetIndex);
		}
		if (this.HasChain())
		{
			this.SpawnChain();
		}
		else if (this.m_MissileInfo.m_SpawnInSequence)
		{
			this.SpawnMissileInSequence();
		}
		spell.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x06003334 RID: 13108 RVA: 0x000FF054 File Offset: 0x000FD254
	private void OnReverseMissileTargetReached(Spell missile)
	{
		missile.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x06003335 RID: 13109 RVA: 0x000FF060 File Offset: 0x000FD260
	private bool HasImpact()
	{
		return this.m_ImpactInfo != null && this.m_ImpactInfo.m_Enabled && (this.m_ImpactInfo.m_Prefab != null || this.m_ImpactInfo.m_DamageAmountImpacts.Length > 0);
	}

	// Token: 0x06003336 RID: 13110 RVA: 0x000FF0B4 File Offset: 0x000FD2B4
	private void SpawnAllImpacts()
	{
		for (int i = 0; i < this.GetVisualTargetCount(); i++)
		{
			this.SpawnImpact(i);
		}
	}

	// Token: 0x06003337 RID: 13111 RVA: 0x000FF0DF File Offset: 0x000FD2DF
	private void SpawnImpact(int targetIndex)
	{
		this.m_effectsPendingFinish++;
		base.StartCoroutine(this.WaitAndSpawnImpact(targetIndex));
	}

	// Token: 0x06003338 RID: 13112 RVA: 0x000FF100 File Offset: 0x000FD300
	private IEnumerator WaitAndSpawnImpact(int targetIndex)
	{
		float spawnDelaySec = Random.Range(this.m_ImpactInfo.m_SpawnDelaySecMin, this.m_ImpactInfo.m_SpawnDelaySecMax);
		yield return new WaitForSeconds(spawnDelaySec);
		int metaDataIndex = this.GetMetaDataIndexForTarget(targetIndex);
		if (metaDataIndex >= 0)
		{
			if (this.ShouldCompleteTasksUntilMetaData(metaDataIndex))
			{
				yield return base.StartCoroutine(this.CompleteTasksUntilMetaData(metaDataIndex));
			}
			float gameDelaySec = Random.Range(this.m_ImpactInfo.m_GameDelaySecMin, this.m_ImpactInfo.m_GameDelaySecMax);
			base.StartCoroutine(this.CompleteTasksFromMetaData(metaDataIndex, gameDelaySec));
		}
		GameObject sourceObject = base.GetSource();
		GameObject targetObject = this.m_visualTargets[targetIndex];
		Spell impactPrefab = this.DetermineImpactPrefab(targetObject);
		Spell impact = this.CloneSpell(impactPrefab);
		impact.SetSource(sourceObject);
		impact.AddTarget(targetObject);
		if (this.m_ImpactInfo.m_UseSuperSpellLocation)
		{
			impact.SetPosition(base.transform.position);
		}
		else
		{
			if (this.IsMakingClones())
			{
				impact.m_Location = this.m_ImpactInfo.m_Location;
				impact.m_SetParentToLocation = this.m_ImpactInfo.m_SetParentToLocation;
			}
			impact.UpdatePosition();
		}
		impact.UpdateOrientation();
		impact.Activate();
		yield break;
	}

	// Token: 0x06003339 RID: 13113 RVA: 0x000FF12C File Offset: 0x000FD32C
	private Spell DetermineImpactPrefab(GameObject targetObject)
	{
		if (this.m_ImpactInfo.m_DamageAmountImpacts.Length == 0)
		{
			return this.m_ImpactInfo.m_Prefab;
		}
		Spell prefab = this.m_ImpactInfo.m_DamageAmountImpacts[0].m_Prefab;
		if (this.m_taskList == null)
		{
			return prefab;
		}
		Card component = targetObject.GetComponent<Card>();
		if (component == null)
		{
			return prefab;
		}
		PowerTaskList.DamageInfo damageInfo = this.m_taskList.GetDamageInfo(component.GetEntity());
		if (damageInfo == null)
		{
			return prefab;
		}
		foreach (SpellImpactPrefabs spellImpactPrefabs in this.m_ImpactInfo.m_DamageAmountImpacts)
		{
			if (damageInfo.m_damage >= spellImpactPrefabs.m_MinDamage && damageInfo.m_damage <= spellImpactPrefabs.m_MaxDamage)
			{
				prefab = spellImpactPrefabs.m_Prefab;
			}
		}
		return prefab;
	}

	// Token: 0x0600333A RID: 13114 RVA: 0x000FF1FC File Offset: 0x000FD3FC
	private bool HasChain()
	{
		return this.m_ChainInfo != null && this.m_ChainInfo.m_Enabled && this.m_ChainInfo.m_Prefab != null;
	}

	// Token: 0x0600333B RID: 13115 RVA: 0x000FF230 File Offset: 0x000FD430
	private void SpawnChain()
	{
		if (this.GetVisualTargetCount() <= 1)
		{
			return;
		}
		this.m_effectsPendingFinish++;
		base.StartCoroutine(this.WaitAndSpawnChain());
	}

	// Token: 0x0600333C RID: 13116 RVA: 0x000FF268 File Offset: 0x000FD468
	private IEnumerator WaitAndSpawnChain()
	{
		float spawnDelaySec = Random.Range(this.m_ChainInfo.m_SpawnDelayMin, this.m_ChainInfo.m_SpawnDelayMax);
		yield return new WaitForSeconds(spawnDelaySec);
		Spell chain = this.CloneSpell(this.m_ChainInfo.m_Prefab);
		GameObject sourceObject = this.GetPrimaryTarget();
		chain.SetSource(sourceObject);
		foreach (GameObject targetObject in this.m_visualTargets)
		{
			if (!(targetObject == sourceObject))
			{
				chain.AddTarget(targetObject);
			}
		}
		chain.ActivateState(SpellStateType.ACTION);
		yield break;
	}

	// Token: 0x0600333D RID: 13117 RVA: 0x000FF284 File Offset: 0x000FD484
	private SpellAreaEffectInfo DetermineAreaEffectInfo()
	{
		Card sourceCard = base.GetSourceCard();
		if (sourceCard != null)
		{
			Player controller = sourceCard.GetController();
			if (controller != null)
			{
				if (controller.IsFriendlySide() && this.HasFriendlyAreaEffect())
				{
					return this.m_FriendlyAreaEffectInfo;
				}
				if (!controller.IsFriendlySide() && this.HasOpponentAreaEffect())
				{
					return this.m_OpponentAreaEffectInfo;
				}
			}
		}
		if (this.HasFriendlyAreaEffect())
		{
			return this.m_FriendlyAreaEffectInfo;
		}
		if (this.HasOpponentAreaEffect())
		{
			return this.m_OpponentAreaEffectInfo;
		}
		return null;
	}

	// Token: 0x0600333E RID: 13118 RVA: 0x000FF310 File Offset: 0x000FD510
	private bool HasAreaEffect()
	{
		return this.HasFriendlyAreaEffect() || this.HasOpponentAreaEffect();
	}

	// Token: 0x0600333F RID: 13119 RVA: 0x000FF326 File Offset: 0x000FD526
	private bool HasFriendlyAreaEffect()
	{
		return this.m_FriendlyAreaEffectInfo != null && this.m_FriendlyAreaEffectInfo.m_Enabled && this.m_FriendlyAreaEffectInfo.m_Prefab != null;
	}

	// Token: 0x06003340 RID: 13120 RVA: 0x000FF357 File Offset: 0x000FD557
	private bool HasOpponentAreaEffect()
	{
		return this.m_OpponentAreaEffectInfo != null && this.m_OpponentAreaEffectInfo.m_Enabled && this.m_OpponentAreaEffectInfo.m_Prefab != null;
	}

	// Token: 0x06003341 RID: 13121 RVA: 0x000FF388 File Offset: 0x000FD588
	private void SpawnAreaEffect(SpellAreaEffectInfo info)
	{
		this.m_effectsPendingFinish++;
		base.StartCoroutine(this.WaitAndSpawnAreaEffect(info));
	}

	// Token: 0x06003342 RID: 13122 RVA: 0x000FF3A8 File Offset: 0x000FD5A8
	private IEnumerator WaitAndSpawnAreaEffect(SpellAreaEffectInfo info)
	{
		float spawnDelaySec = Random.Range(info.m_SpawnDelaySecMin, info.m_SpawnDelaySecMax);
		yield return new WaitForSeconds(spawnDelaySec);
		Spell areaEffect = this.CloneSpell(info.m_Prefab);
		areaEffect.SetSource(base.GetSource());
		areaEffect.AttachPowerTaskList(this.m_taskList);
		if (info.m_UseSuperSpellLocation)
		{
			areaEffect.SetPosition(base.transform.position);
		}
		else if (this.IsMakingClones() && info.m_Location != SpellLocation.NONE)
		{
			areaEffect.m_Location = info.m_Location;
			areaEffect.m_SetParentToLocation = info.m_SetParentToLocation;
			areaEffect.UpdatePosition();
		}
		if (this.IsMakingClones() && info.m_Facing != SpellFacing.NONE)
		{
			areaEffect.m_Facing = info.m_Facing;
			areaEffect.m_FacingOptions = info.m_FacingOptions;
			areaEffect.UpdateOrientation();
		}
		areaEffect.Activate();
		yield break;
	}

	// Token: 0x06003343 RID: 13123 RVA: 0x000FF3D4 File Offset: 0x000FD5D4
	private bool AddPrimaryChainTarget()
	{
		Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
		return blockStart != null && base.AddSinglePowerTarget_FromBlockStart(blockStart);
	}

	// Token: 0x06003344 RID: 13124 RVA: 0x000FF3FC File Offset: 0x000FD5FC
	private int GetPrimaryTargetIndex()
	{
		return 0;
	}

	// Token: 0x06003345 RID: 13125 RVA: 0x000FF3FF File Offset: 0x000FD5FF
	private GameObject GetPrimaryTarget()
	{
		return this.m_visualTargets[this.GetPrimaryTargetIndex()];
	}

	// Token: 0x06003346 RID: 13126 RVA: 0x000FF412 File Offset: 0x000FD612
	protected virtual void UpdateTargets()
	{
		this.UpdateVisualTargets();
		this.SuppressPlaySoundsOnVisualTargets();
	}

	// Token: 0x06003347 RID: 13127 RVA: 0x000FF420 File Offset: 0x000FD620
	private int GetVisualTargetCount()
	{
		if (this.IsMakingClones())
		{
			return this.m_visualTargets.Count;
		}
		return Mathf.Min(1, this.m_visualTargets.Count);
	}

	// Token: 0x06003348 RID: 13128 RVA: 0x000FF458 File Offset: 0x000FD658
	protected virtual void UpdateVisualTargets()
	{
		SpellTargetBehavior behavior = this.m_TargetInfo.m_Behavior;
		if (behavior == SpellTargetBehavior.FRIENDLY_PLAY_ZONE_CENTER)
		{
			ZonePlay zonePlay = SpellUtils.FindFriendlyPlayZone(this);
			this.AddVisualTarget(zonePlay.gameObject);
		}
		else if (behavior == SpellTargetBehavior.FRIENDLY_PLAY_ZONE_RANDOM)
		{
			ZonePlay zonePlay2 = SpellUtils.FindFriendlyPlayZone(this);
			this.GenerateRandomPlayZoneVisualTargets(zonePlay2);
		}
		else if (behavior == SpellTargetBehavior.OPPONENT_PLAY_ZONE_CENTER)
		{
			ZonePlay zonePlay3 = SpellUtils.FindOpponentPlayZone(this);
			this.AddVisualTarget(zonePlay3.gameObject);
		}
		else if (behavior == SpellTargetBehavior.OPPONENT_PLAY_ZONE_RANDOM)
		{
			ZonePlay zonePlay4 = SpellUtils.FindOpponentPlayZone(this);
			this.GenerateRandomPlayZoneVisualTargets(zonePlay4);
		}
		else if (behavior == SpellTargetBehavior.BOARD_CENTER)
		{
			Board board = Board.Get();
			this.AddVisualTarget(board.FindBone("CenterPointBone").gameObject);
		}
		else if (behavior == SpellTargetBehavior.UNTARGETED)
		{
			this.AddVisualTarget(base.GetSource());
		}
		else if (behavior == SpellTargetBehavior.CHOSEN_TARGET_ONLY)
		{
			this.AddChosenTargetAsVisualTarget();
		}
		else if (behavior == SpellTargetBehavior.BOARD_RANDOM)
		{
			this.GenerateRandomBoardVisualTargets();
		}
		else if (behavior == SpellTargetBehavior.TARGET_ZONE_CENTER)
		{
			Zone zone = SpellUtils.FindTargetZone(this);
			this.AddVisualTarget(zone.gameObject);
		}
		else if (behavior == SpellTargetBehavior.NEW_CREATED_CARDS)
		{
			this.GenerateCreatedCardsTargets();
		}
		else
		{
			this.AddAllTargetsAsVisualTargets();
		}
	}

	// Token: 0x06003349 RID: 13129 RVA: 0x000FF588 File Offset: 0x000FD788
	protected void GenerateRandomBoardVisualTargets()
	{
		ZonePlay zonePlay = SpellUtils.FindFriendlyPlayZone(this);
		ZonePlay zonePlay2 = SpellUtils.FindOpponentPlayZone(this);
		Bounds bounds = zonePlay.GetComponent<Collider>().bounds;
		Bounds bounds2 = zonePlay2.GetComponent<Collider>().bounds;
		Vector3 vector = Vector3.Min(bounds.min, bounds2.min);
		Vector3 vector2 = Vector3.Max(bounds.max, bounds2.max);
		Vector3 vector3 = 0.5f * (vector2 + vector);
		Vector3 vector4 = vector2 - vector;
		Vector3 vector5;
		vector5..ctor(Mathf.Abs(vector4.x), Mathf.Abs(vector4.y), Mathf.Abs(vector4.z));
		Bounds bounds3;
		bounds3..ctor(vector3, vector5);
		this.GenerateRandomVisualTargets(bounds3);
	}

	// Token: 0x0600334A RID: 13130 RVA: 0x000FF643 File Offset: 0x000FD843
	protected void GenerateRandomPlayZoneVisualTargets(ZonePlay zonePlay)
	{
		this.GenerateRandomVisualTargets(zonePlay.GetComponent<Collider>().bounds);
	}

	// Token: 0x0600334B RID: 13131 RVA: 0x000FF658 File Offset: 0x000FD858
	private void GenerateRandomVisualTargets(Bounds bounds)
	{
		int num = Random.Range(this.m_TargetInfo.m_RandomTargetCountMin, this.m_TargetInfo.m_RandomTargetCountMax + 1);
		if (num == 0)
		{
			return;
		}
		float x = bounds.min.x;
		float z = bounds.max.z;
		float z2 = bounds.min.z;
		float num2 = bounds.size.x / (float)num;
		int[] array = new int[num];
		int[] array2 = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = 0;
			array2[i] = -1;
		}
		for (int j = 0; j < num; j++)
		{
			float num3 = Random.Range(0f, 1f);
			int num4 = 0;
			for (int k = 0; k < num; k++)
			{
				float num5 = this.ComputeBoxPickChance(array, k);
				if (num5 >= num3)
				{
					array2[num4++] = k;
				}
			}
			int num6 = array2[Random.Range(0, num4)];
			array[num6]++;
			float num7 = x + (float)num6 * num2;
			float num8 = num7 + num2;
			Vector3 position = default(Vector3);
			position.x = Random.Range(num7, num8);
			position.y = bounds.center.y;
			position.z = Random.Range(z2, z);
			this.GenerateVisualTarget(position, j, num6);
		}
	}

	// Token: 0x0600334C RID: 13132 RVA: 0x000FF7DC File Offset: 0x000FD9DC
	private void GenerateVisualTarget(Vector3 position, int index, int boxIndex)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = string.Format("{0} Target {1} (box {2})", this, index, boxIndex);
		gameObject.transform.position = position;
		gameObject.AddComponent<SpellGeneratedTarget>();
		this.AddVisualTarget(gameObject);
	}

	// Token: 0x0600334D RID: 13133 RVA: 0x000FF828 File Offset: 0x000FDA28
	private float ComputeBoxPickChance(int[] boxUsageCounts, int index)
	{
		int num = boxUsageCounts[index];
		float num2 = (float)boxUsageCounts.Length * 0.25f;
		float num3 = (float)num / num2;
		return 1f - num3;
	}

	// Token: 0x0600334E RID: 13134 RVA: 0x000FF850 File Offset: 0x000FDA50
	private void GenerateCreatedCardsTargets()
	{
		foreach (PowerTask powerTask in this.m_taskList.GetTaskList())
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.FULL_ENTITY)
			{
				Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
				int id = histFullEntity.Entity.ID;
				Entity entity = GameState.Get().GetEntity(id);
				if (entity.GetTag(GAME_TAG.ZONE) != 6)
				{
					if (entity == null)
					{
						string text = string.Format("{0}.GenerateCreatedCardsTargets() - WARNING trying to target entity with id {1} but there is no entity with that id", this, id);
						Debug.LogWarning(text);
					}
					else
					{
						Card card = entity.GetCard();
						if (card == null)
						{
							string text2 = string.Format("{0}.GenerateCreatedCardsTargets() - WARNING trying to target entity.GetCard() with id {1} but there is no card with that id", this, id);
							Debug.LogWarning(text2);
						}
						else
						{
							this.m_visualTargets.Add(card.gameObject);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600334F RID: 13135 RVA: 0x000FF968 File Offset: 0x000FDB68
	private void AddChosenTargetAsVisualTarget()
	{
		Card powerTargetCard = base.GetPowerTargetCard();
		if (powerTargetCard == null)
		{
			Debug.LogWarning(string.Format("{0}.AddChosenTargetAsVisualTarget() - there is no chosen target", this));
			return;
		}
		this.AddVisualTarget(powerTargetCard.gameObject);
	}

	// Token: 0x06003350 RID: 13136 RVA: 0x000FF9A8 File Offset: 0x000FDBA8
	private void AddAllTargetsAsVisualTargets()
	{
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			int count = this.m_visualTargets.Count;
			this.m_visualToTargetIndexMap[count] = i;
			this.AddVisualTarget(this.m_targets[i]);
		}
	}

	// Token: 0x06003351 RID: 13137 RVA: 0x000FF9FC File Offset: 0x000FDBFC
	private void SuppressPlaySoundsOnVisualTargets()
	{
		if (!this.m_TargetInfo.m_SuppressPlaySounds)
		{
			return;
		}
		for (int i = 0; i < this.m_visualTargets.Count; i++)
		{
			GameObject gameObject = this.m_visualTargets[i];
			Card component = gameObject.GetComponent<Card>();
			if (!(component == null))
			{
				component.SuppressPlaySounds(true);
			}
		}
	}

	// Token: 0x06003352 RID: 13138 RVA: 0x000FFA64 File Offset: 0x000FDC64
	protected virtual void CleanUp()
	{
		foreach (GameObject gameObject in this.m_visualTargets)
		{
			if (!(gameObject.GetComponent<SpellGeneratedTarget>() == null))
			{
				Object.Destroy(gameObject);
			}
		}
		this.m_visualTargets.Clear();
	}

	// Token: 0x06003353 RID: 13139 RVA: 0x000FFAE0 File Offset: 0x000FDCE0
	protected bool HasMetaDataTargets()
	{
		return this.m_targetToMetaDataMap.Count > 0;
	}

	// Token: 0x06003354 RID: 13140 RVA: 0x000FFAF0 File Offset: 0x000FDCF0
	protected int GetMetaDataIndexForTarget(int visualTargetIndex)
	{
		int key;
		if (!this.m_visualToTargetIndexMap.TryGetValue(visualTargetIndex, out key))
		{
			return -1;
		}
		int result;
		if (!this.m_targetToMetaDataMap.TryGetValue(key, out result))
		{
			return -1;
		}
		return result;
	}

	// Token: 0x06003355 RID: 13141 RVA: 0x000FFB28 File Offset: 0x000FDD28
	protected bool ShouldCompleteTasksUntilMetaData(int metaDataIndex)
	{
		return this.m_taskList.HasEarlierIncompleteTask(metaDataIndex);
	}

	// Token: 0x06003356 RID: 13142 RVA: 0x000FFB40 File Offset: 0x000FDD40
	protected IEnumerator CompleteTasksUntilMetaData(int metaDataIndex)
	{
		this.m_effectsPendingFinish++;
		this.m_taskList.DoTasks(0, metaDataIndex);
		QueueList<PowerTask> tasks = this.DetermineTasksToWaitFor(0, metaDataIndex);
		if (tasks != null && tasks.Count > 0)
		{
			yield return base.StartCoroutine(this.WaitForTasks(tasks));
		}
		this.m_effectsPendingFinish--;
		yield break;
	}

	// Token: 0x06003357 RID: 13143 RVA: 0x000FFB6C File Offset: 0x000FDD6C
	protected QueueList<PowerTask> DetermineTasksToWaitFor(int startIndex, int count)
	{
		if (count == 0)
		{
			return null;
		}
		int num = startIndex + count;
		QueueList<PowerTask> queueList = new QueueList<PowerTask>();
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		for (int i = startIndex; i < num; i++)
		{
			PowerTask powerTask = taskList[i];
			Entity entity = this.GetEntityFromZoneChangePowerTask(powerTask);
			if (entity != null)
			{
				GameObject gameObject = this.m_visualTargets.Find(delegate(GameObject currTargetObject)
				{
					Card component = currTargetObject.GetComponent<Card>();
					return entity.GetCard() == component;
				});
				if (!(gameObject == null))
				{
					for (int j = 0; j < queueList.Count; j++)
					{
						PowerTask task = queueList[j];
						Entity entityFromZoneChangePowerTask = this.GetEntityFromZoneChangePowerTask(task);
						if (entity == entityFromZoneChangePowerTask)
						{
							queueList.RemoveAt(j);
							break;
						}
					}
					queueList.Enqueue(powerTask);
				}
			}
		}
		return queueList;
	}

	// Token: 0x06003358 RID: 13144 RVA: 0x000FFC5C File Offset: 0x000FDE5C
	protected IEnumerator WaitForTasks(QueueList<PowerTask> tasksToWaitFor)
	{
		while (tasksToWaitFor.Count > 0)
		{
			PowerTask task = tasksToWaitFor.Peek();
			if (!task.IsCompleted())
			{
				yield return null;
			}
			else
			{
				Entity entity;
				int zoneTag;
				this.GetZoneChangeFromPowerTask(task, out entity, out zoneTag);
				Card card = entity.GetCard();
				Zone zone = ZoneMgr.Get().FindZoneForEntityAndZoneTag(entity, (TAG_ZONE)zoneTag);
				while (card.GetZone() != zone)
				{
					yield return null;
				}
				while (card.IsActorLoading())
				{
					yield return null;
				}
				tasksToWaitFor.Dequeue();
			}
		}
		yield break;
	}

	// Token: 0x06003359 RID: 13145 RVA: 0x000FFC88 File Offset: 0x000FDE88
	protected IEnumerator CompleteTasksFromMetaData(int metaDataIndex, float delaySec)
	{
		this.m_effectsPendingFinish++;
		yield return new WaitForSeconds(delaySec);
		base.CompleteMetaDataTasks(metaDataIndex, new PowerTaskList.CompleteCallback(this.OnMetaDataTasksComplete));
		yield break;
	}

	// Token: 0x0600335A RID: 13146 RVA: 0x000FFCBF File Offset: 0x000FDEBF
	protected void OnMetaDataTasksComplete(PowerTaskList taskList, int startIndex, int count, object userData)
	{
		this.m_effectsPendingFinish--;
		this.FinishIfPossible();
	}

	// Token: 0x0600335B RID: 13147 RVA: 0x000FFCD5 File Offset: 0x000FDED5
	protected bool IsMakingClones()
	{
		return true;
	}

	// Token: 0x0600335C RID: 13148 RVA: 0x000FFCD8 File Offset: 0x000FDED8
	protected bool AreEffectsActive()
	{
		return this.m_effectsPendingFinish > 0;
	}

	// Token: 0x0600335D RID: 13149 RVA: 0x000FFCE4 File Offset: 0x000FDEE4
	protected Spell CloneSpell(Spell prefab)
	{
		Spell spell;
		if (this.IsMakingClones())
		{
			spell = Object.Instantiate<Spell>(prefab);
			spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnCloneSpellStateStarted));
			spell.transform.parent = base.transform;
		}
		else
		{
			spell = prefab;
			spell.RemoveAllTargets();
		}
		spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnCloneSpellFinished));
		return spell;
	}

	// Token: 0x0600335E RID: 13150 RVA: 0x000FFD46 File Offset: 0x000FDF46
	private void OnCloneSpellFinished(Spell spell, object userData)
	{
		this.m_effectsPendingFinish--;
		this.FinishIfPossible();
	}

	// Token: 0x0600335F RID: 13151 RVA: 0x000FFD5C File Offset: 0x000FDF5C
	private void OnCloneSpellStateStarted(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		Object.Destroy(spell.gameObject);
	}

	// Token: 0x06003360 RID: 13152 RVA: 0x000FFD75 File Offset: 0x000FDF75
	private void UpdatePendingStateChangeFlags(SpellStateType stateType)
	{
		if (!base.HasStateContent(stateType))
		{
			this.m_pendingNoneStateChange = true;
			this.m_pendingSpellFinish = true;
		}
		else
		{
			this.m_pendingNoneStateChange = false;
			this.m_pendingSpellFinish = false;
		}
	}

	// Token: 0x06003361 RID: 13153 RVA: 0x000FFDA4 File Offset: 0x000FDFA4
	protected void FinishIfPossible()
	{
		if (this.m_settingUpAction)
		{
			return;
		}
		if (this.AreEffectsActive())
		{
			return;
		}
		if (this.m_pendingSpellFinish)
		{
			this.OnSpellFinished();
			this.m_pendingSpellFinish = false;
		}
		if (this.m_pendingNoneStateChange)
		{
			this.OnStateFinished();
			this.m_pendingNoneStateChange = false;
		}
		this.CleanUp();
	}

	// Token: 0x04001FFA RID: 8186
	public bool m_MakeClones = true;

	// Token: 0x04001FFB RID: 8187
	public SpellTargetInfo m_TargetInfo = new SpellTargetInfo();

	// Token: 0x04001FFC RID: 8188
	public SpellStartInfo m_StartInfo;

	// Token: 0x04001FFD RID: 8189
	public SpellActionInfo m_ActionInfo;

	// Token: 0x04001FFE RID: 8190
	public SpellMissileInfo m_MissileInfo;

	// Token: 0x04001FFF RID: 8191
	public SpellImpactInfo m_ImpactInfo;

	// Token: 0x04002000 RID: 8192
	public SpellAreaEffectInfo m_FriendlyAreaEffectInfo;

	// Token: 0x04002001 RID: 8193
	public SpellAreaEffectInfo m_OpponentAreaEffectInfo;

	// Token: 0x04002002 RID: 8194
	public SpellChainInfo m_ChainInfo;

	// Token: 0x04002003 RID: 8195
	protected Spell m_startSpell;

	// Token: 0x04002004 RID: 8196
	protected List<GameObject> m_visualTargets = new List<GameObject>();

	// Token: 0x04002005 RID: 8197
	protected int m_currentTargetIndex;

	// Token: 0x04002006 RID: 8198
	protected int m_effectsPendingFinish;

	// Token: 0x04002007 RID: 8199
	protected bool m_pendingNoneStateChange;

	// Token: 0x04002008 RID: 8200
	protected bool m_pendingSpellFinish;

	// Token: 0x04002009 RID: 8201
	protected Map<int, int> m_visualToTargetIndexMap = new Map<int, int>();

	// Token: 0x0400200A RID: 8202
	protected Map<int, int> m_targetToMetaDataMap = new Map<int, int>();

	// Token: 0x0400200B RID: 8203
	protected bool m_settingUpAction;
}
