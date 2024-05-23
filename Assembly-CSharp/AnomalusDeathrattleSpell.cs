using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E38 RID: 3640
public class AnomalusDeathrattleSpell : Spell
{
	// Token: 0x06006EE4 RID: 28388 RVA: 0x00207FA4 File Offset: 0x002061A4
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		foreach (GameObject gameObject in this.GetVisualTargets())
		{
			if (!(gameObject == null))
			{
				Card component = gameObject.GetComponent<Card>();
				component.SuppressKeywordDeaths(this.m_SuppressKeywordDeaths);
				component.OverrideCustomDeathSpell(Object.Instantiate<Spell>(this.m_CustomDeathSpell));
			}
		}
		base.StartCoroutine(this.AnimateMinions());
	}

	// Token: 0x06006EE5 RID: 28389 RVA: 0x00208040 File Offset: 0x00206240
	private IEnumerator AnimateMinions()
	{
		if (this.m_source == null)
		{
			yield break;
		}
		yield return new WaitForSeconds(this.m_DelayBeforeStart);
		float lastLiftTime = 0f;
		Dictionary<GameObject, Vector3> orgPosDict = new Dictionary<GameObject, Vector3>();
		Dictionary<GameObject, Quaternion> orgRotDict = new Dictionary<GameObject, Quaternion>();
		this.OnSpellFinished();
		this.m_TargetActorGameObjects = new GameObject[this.m_targets.Count];
		this.m_TargetActors = new Actor[this.m_targets.Count];
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			GameObject targetCardGO = this.m_targets[i];
			if (!(targetCardGO == null))
			{
				Card targetCard = targetCardGO.GetComponent<Card>();
				if (!(targetCard == null))
				{
					Actor targetActor = targetCard.GetActor();
					if (!(targetActor == null))
					{
						this.m_TargetActors[i] = targetActor;
						GameObject target = targetActor.gameObject;
						if (!(target == null))
						{
							this.m_TargetActorGameObjects[i] = target;
							Vector3 OrgPos = target.transform.localPosition;
							Quaternion OrgRot = target.transform.localRotation;
							orgPosDict.Add(target, OrgPos);
							orgRotDict.Add(target, OrgRot);
							float dist = Vector3.Distance(this.m_source.transform.position, target.transform.position);
							float liftTime = dist * this.m_DelayDistanceModifier;
							if (lastLiftTime < liftTime)
							{
								lastLiftTime = liftTime;
							}
							float randomHeight = Random.Range(this.m_LiftHeightMin, this.m_LiftHeightMax);
							Hashtable liftArgs = iTween.Hash(new object[]
							{
								"time",
								this.m_RiseTime,
								"delay",
								dist * this.m_DelayDistanceModifier,
								"position",
								new Vector3(OrgPos.x, OrgPos.y + randomHeight, OrgPos.z),
								"easetype",
								iTween.EaseType.easeOutExpo,
								"islocal",
								true,
								"name",
								string.Format("Lift_{0}_{1}", target.name, i)
							});
							iTween.MoveTo(target, liftArgs);
							Vector3 newRot = OrgRot.eulerAngles;
							newRot.x += Random.Range(this.m_LiftRotMin, this.m_LiftRotMax);
							newRot.z += Random.Range(this.m_LiftRotMin, this.m_LiftRotMax);
							Hashtable liftRotArgs = iTween.Hash(new object[]
							{
								"time",
								this.m_RiseTime + this.m_HangTime,
								"delay",
								dist * this.m_DelayDistanceModifier,
								"rotation",
								newRot,
								"easetype",
								iTween.EaseType.easeOutQuad,
								"islocal",
								true,
								"name",
								string.Format("LiftRot_{0}_{1}", target.name, i)
							});
							iTween.RotateTo(target, liftRotArgs);
						}
					}
				}
			}
		}
		yield return new WaitForSeconds(lastLiftTime);
		for (int j = 0; j < this.m_targets.Count; j++)
		{
			GameObject target2 = this.m_TargetActorGameObjects[j];
			if (!(target2 == null))
			{
				GameObject targetCardGO2 = this.m_targets[j];
				if (!(targetCardGO2 == null))
				{
					Card targetCard2 = targetCardGO2.GetComponent<Card>();
					if (!(targetCard2 == null))
					{
						if (targetCard2.GetZone().m_ServerTag == TAG_ZONE.GRAVEYARD)
						{
							Actor targetActor2 = this.m_TargetActors[j];
							if (targetActor2 == null)
							{
								goto IL_79D;
							}
							targetActor2.DeactivateAllPreDeathSpells();
						}
						float slamDelay = 0f;
						Hashtable slamPosArgs = iTween.Hash(new object[]
						{
							"time",
							this.m_SlamTime,
							"delay",
							this.m_DelayAfterSpellFinish + slamDelay,
							"position",
							orgPosDict[target2],
							"easetype",
							iTween.EaseType.easeInCubic,
							"islocal",
							true,
							"name",
							string.Format("SlamPos_{0}_{1}", target2.name, j)
						});
						iTween.MoveTo(target2, slamPosArgs);
						Hashtable slamRotArgs = iTween.Hash(new object[]
						{
							"time",
							this.m_SlamTime * 0.8f,
							"delay",
							this.m_DelayAfterSpellFinish + slamDelay + this.m_SlamTime * 0.2f,
							"rotation",
							Vector3.zero,
							"easetype",
							iTween.EaseType.easeInQuad,
							"islocal",
							true,
							"name",
							string.Format("SlamRot_{0}_{1}", target2.name, j)
						});
						iTween.RotateTo(target2, slamRotArgs);
					}
				}
			}
			IL_79D:;
		}
		yield break;
	}

	// Token: 0x040057BB RID: 22459
	public Spell m_CustomDeathSpell;

	// Token: 0x040057BC RID: 22460
	public bool m_SuppressKeywordDeaths = true;

	// Token: 0x040057BD RID: 22461
	public float m_DelayBeforeStart = 1f;

	// Token: 0x040057BE RID: 22462
	public float m_DelayDistanceModifier = 1f;

	// Token: 0x040057BF RID: 22463
	public float m_RiseTime = 0.5f;

	// Token: 0x040057C0 RID: 22464
	public float m_HangTime = 1f;

	// Token: 0x040057C1 RID: 22465
	public float m_LiftHeightMin = 2f;

	// Token: 0x040057C2 RID: 22466
	public float m_LiftHeightMax = 3f;

	// Token: 0x040057C3 RID: 22467
	public float m_LiftRotMin = -15f;

	// Token: 0x040057C4 RID: 22468
	public float m_LiftRotMax = 15f;

	// Token: 0x040057C5 RID: 22469
	public float m_SlamTime = 0.15f;

	// Token: 0x040057C6 RID: 22470
	public float m_Bounceness = 0.2f;

	// Token: 0x040057C7 RID: 22471
	public float m_DelayAfterSpellFinish = 3f;

	// Token: 0x040057C8 RID: 22472
	private GameObject[] m_TargetActorGameObjects;

	// Token: 0x040057C9 RID: 22473
	private Actor[] m_TargetActors;
}
