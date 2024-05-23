using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E7E RID: 3710
public class PoisonSeeds : SuperSpell
{
	// Token: 0x0600706A RID: 28778 RVA: 0x00210E57 File Offset: 0x0020F057
	protected override void Awake()
	{
		this.m_Sound = base.GetComponent<AudioSource>();
		base.Awake();
	}

	// Token: 0x0600706B RID: 28779 RVA: 0x00210E6C File Offset: 0x0020F06C
	public override bool AddPowerTargets()
	{
		this.m_visualToTargetIndexMap.Clear();
		this.m_targetToMetaDataMap.Clear();
		this.m_targets.Clear();
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		for (int i = 0; i < taskList.Count; i++)
		{
			PowerTask task = taskList[i];
			Card targetCardFromPowerTask = this.GetTargetCardFromPowerTask(i, task);
			if (!(targetCardFromPowerTask == null))
			{
				this.m_targets.Add(targetCardFromPowerTask.gameObject);
			}
		}
		return this.m_targets.Count > 0;
	}

	// Token: 0x0600706C RID: 28780 RVA: 0x00210F04 File Offset: 0x0020F104
	protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		int num;
		if (power.Type == Network.PowerType.FULL_ENTITY)
		{
			this.m_TargetType = PoisonSeeds.SpellTargetType.Create;
			Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
			num = histFullEntity.Entity.ID;
		}
		else
		{
			Network.HistTagChange histTagChange = power as Network.HistTagChange;
			if (histTagChange == null || histTagChange.Tag != 360)
			{
				return null;
			}
			this.m_TargetType = PoisonSeeds.SpellTargetType.Death;
			num = histTagChange.Entity;
		}
		Entity entity = GameState.Get().GetEntity(num);
		if (entity == null)
		{
			string text = string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", this, num);
			Debug.LogWarning(text);
			return null;
		}
		return entity.GetCard();
	}

	// Token: 0x0600706D RID: 28781 RVA: 0x00210FB0 File Offset: 0x0020F1B0
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		if (this.m_TargetType == PoisonSeeds.SpellTargetType.Death)
		{
			this.DeathEffect();
		}
		else if (this.m_TargetType == PoisonSeeds.SpellTargetType.Create)
		{
			base.StartCoroutine(this.CreateEffect());
		}
		else
		{
			this.m_effectsPendingFinish--;
			base.FinishIfPossible();
		}
	}

	// Token: 0x0600706E RID: 28782 RVA: 0x0021101C File Offset: 0x0020F21C
	private IEnumerator CreateEffect()
	{
		foreach (GameObject target in base.GetTargets())
		{
			if (!(target == null))
			{
				Card targetCard = target.GetComponent<Card>();
				if (!(targetCard == null))
				{
					targetCard.OverrideCustomSpawnSpell(Object.Instantiate<Spell>(this.m_CustomSpawnSpell));
					ZonePlay zone = (ZonePlay)targetCard.GetZone();
					if (!(zone == null))
					{
						zone.SetTransitionTime(0.01f);
					}
				}
			}
		}
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
		this.ShakeCamera();
		yield return new WaitForSeconds(1f);
		foreach (GameObject target2 in base.GetTargets())
		{
			Card card = target2.GetComponent<Card>();
			if (!(card == null))
			{
				ZonePlay zone2 = (ZonePlay)card.GetZone();
				if (!(zone2 == null))
				{
					zone2.ResetTransitionTime();
				}
			}
		}
		yield break;
	}

	// Token: 0x0600706F RID: 28783 RVA: 0x00211038 File Offset: 0x0020F238
	private void DeathEffect()
	{
		if (this.m_HeightCurve.length == 0)
		{
			Debug.LogWarning("PoisonSeeds Spell height animation curve in not defined");
			return;
		}
		if (this.m_RotationDriftCurve.length == 0)
		{
			Debug.LogWarning("PoisonSeeds Spell rotation drift animation curve in not defined");
			return;
		}
		if (this.m_CustomDeathSpell != null)
		{
			foreach (GameObject gameObject in base.GetTargets())
			{
				if (!(gameObject == null))
				{
					Card component = gameObject.GetComponent<Card>();
					component.OverrideCustomDeathSpell(Object.Instantiate<Spell>(this.m_CustomDeathSpell));
				}
			}
		}
		this.m_HeightCurveLength = this.m_HeightCurve.keys[this.m_HeightCurve.length - 1].time;
		List<PoisonSeeds.MinionData> list = new List<PoisonSeeds.MinionData>();
		foreach (GameObject gameObject2 in base.GetTargets())
		{
			PoisonSeeds.MinionData minionData = new PoisonSeeds.MinionData();
			minionData.card = gameObject2.GetComponent<Card>();
			minionData.gameObject = gameObject2;
			minionData.orgLocPos = gameObject2.transform.localPosition;
			minionData.orgLocRot = gameObject2.transform.localRotation;
			float num = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value);
			float num2 = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value) * 0.1f;
			float num3 = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value);
			minionData.rotationDrift = new Vector3(num, num2, num3);
			list.Add(minionData);
		}
		base.StartCoroutine(this.AnimateDeathEffect(list));
	}

	// Token: 0x06007070 RID: 28784 RVA: 0x0021122C File Offset: 0x0020F42C
	private IEnumerator AnimateDeathEffect(List<PoisonSeeds.MinionData> minions)
	{
		if (this.m_Sound != null)
		{
			SoundManager.Get().Play(this.m_Sound);
		}
		List<ParticleSystem> impactParticles = new List<ParticleSystem>();
		foreach (PoisonSeeds.MinionData minion in minions)
		{
			GameObject newImpactParticle = Object.Instantiate<GameObject>(this.m_ImpactParticles.gameObject);
			newImpactParticle.transform.parent = base.transform;
			newImpactParticle.transform.position = minion.gameObject.transform.position;
			impactParticles.Add(newImpactParticle.GetComponentInChildren<ParticleSystem>());
			GameObject newDustParticle = Object.Instantiate<GameObject>(this.m_DustParticles.gameObject);
			newDustParticle.transform.parent = base.transform;
			newDustParticle.transform.position = minion.gameObject.transform.position;
			newDustParticle.GetComponent<ParticleSystem>().Play();
		}
		this.m_AnimTime = 0f;
		bool finished = false;
		while (this.m_AnimTime < this.m_HeightCurveLength)
		{
			this.m_AnimTime += Time.deltaTime;
			float height = this.m_HeightCurve.Evaluate(this.m_AnimTime);
			float rotAmount = this.m_RotationDriftCurve.Evaluate(this.m_AnimTime);
			foreach (PoisonSeeds.MinionData minion2 in minions)
			{
				minion2.gameObject.transform.localPosition = new Vector3(minion2.orgLocPos.x, minion2.orgLocPos.y + height, minion2.orgLocPos.z);
				minion2.gameObject.transform.localRotation = minion2.orgLocRot;
				minion2.gameObject.transform.Rotate(minion2.rotationDrift * rotAmount, 1);
			}
			if (this.m_AnimTime > this.m_HeightCurveLength - this.m_StartDeathSpellAdjustment && !finished)
			{
				foreach (PoisonSeeds.MinionData minion3 in minions)
				{
					minion3.card.GetActor().DeactivateAllPreDeathSpells();
				}
				this.m_effectsPendingFinish--;
				base.FinishIfPossible();
				finished = true;
			}
			yield return null;
		}
		foreach (ParticleSystem ips in impactParticles)
		{
			ips.Play();
		}
		this.ShakeCamera();
		yield break;
	}

	// Token: 0x06007071 RID: 28785 RVA: 0x00211255 File Offset: 0x0020F455
	private void ShakeCamera()
	{
		CameraShakeMgr.Shake(Camera.main, new Vector3(0.15f, 0.15f, 0.15f), 0.9f);
	}

	// Token: 0x04005983 RID: 22915
	public Spell m_CustomSpawnSpell;

	// Token: 0x04005984 RID: 22916
	public Spell m_CustomDeathSpell;

	// Token: 0x04005985 RID: 22917
	public float m_StartDeathSpellAdjustment = 0.01f;

	// Token: 0x04005986 RID: 22918
	public AnimationCurve m_HeightCurve;

	// Token: 0x04005987 RID: 22919
	public float m_RotationDriftAmount;

	// Token: 0x04005988 RID: 22920
	public AnimationCurve m_RotationDriftCurve;

	// Token: 0x04005989 RID: 22921
	public ParticleSystem m_ImpactParticles;

	// Token: 0x0400598A RID: 22922
	public ParticleSystem m_DustParticles;

	// Token: 0x0400598B RID: 22923
	private PoisonSeeds.SpellTargetType m_TargetType;

	// Token: 0x0400598C RID: 22924
	private float m_HeightCurveLength;

	// Token: 0x0400598D RID: 22925
	private float m_AnimTime;

	// Token: 0x0400598E RID: 22926
	private AudioSource m_Sound;

	// Token: 0x02000E7F RID: 3711
	public class MinionData
	{
		// Token: 0x0400598F RID: 22927
		public GameObject gameObject;

		// Token: 0x04005990 RID: 22928
		public Vector3 orgLocPos;

		// Token: 0x04005991 RID: 22929
		public Quaternion orgLocRot;

		// Token: 0x04005992 RID: 22930
		public Vector3 rotationDrift;

		// Token: 0x04005993 RID: 22931
		public Card card;
	}

	// Token: 0x02000E80 RID: 3712
	private enum SpellTargetType
	{
		// Token: 0x04005995 RID: 22933
		None,
		// Token: 0x04005996 RID: 22934
		Death,
		// Token: 0x04005997 RID: 22935
		Create
	}
}
