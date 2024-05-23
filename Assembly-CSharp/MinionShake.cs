﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200082A RID: 2090
public class MinionShake : MonoBehaviour
{
	// Token: 0x06005070 RID: 20592 RVA: 0x0017E5E8 File Offset: 0x0017C7E8
	private void LateUpdate()
	{
		GraphicsManager graphicsManager = GraphicsManager.Get();
		if (graphicsManager != null && graphicsManager.RenderQualityLevel == GraphicsQuality.Low)
		{
			return;
		}
		if (!this.m_Animating)
		{
			return;
		}
		if (this.m_Animator == null)
		{
			return;
		}
		if (this.m_MinionShakeInstance == null)
		{
			return;
		}
		if (this.m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == MinionShake.s_IdleState && !this.m_Animator.GetBool("shake"))
		{
			base.transform.localPosition = this.m_MinionOrgPos;
			base.transform.localRotation = this.m_MinionOrgRot;
			this.m_Animating = false;
			return;
		}
		base.transform.localPosition = this.m_CardPlayAllyTransform.localPosition + this.m_MinionOrgPos;
		base.transform.localRotation = this.m_MinionOrgRot;
		base.transform.Rotate(this.m_CardPlayAllyTransform.localRotation.eulerAngles);
	}

	// Token: 0x06005071 RID: 20593 RVA: 0x0017E6F0 File Offset: 0x0017C8F0
	private void OnDestroy()
	{
		if (this.m_MinionShakeInstance)
		{
			Object.Destroy(this.m_MinionShakeInstance);
		}
	}

	// Token: 0x06005072 RID: 20594 RVA: 0x0017E70D File Offset: 0x0017C90D
	public bool isShaking()
	{
		return this.m_Animating;
	}

	// Token: 0x06005073 RID: 20595 RVA: 0x0017E718 File Offset: 0x0017C918
	public static void ShakeAllMinions(GameObject shakeTrigger, ShakeMinionType shakeType, Vector3 impactPoint, ShakeMinionIntensity intensityType, float intensityValue, float radius, float startDelay)
	{
		foreach (MinionShake minionShake in MinionShake.FindAllMinionShakers(shakeTrigger))
		{
			minionShake.m_StartDelay = startDelay;
			minionShake.m_ShakeType = shakeType;
			minionShake.m_ImpactPosition = impactPoint;
			minionShake.m_ShakeIntensityType = intensityType;
			minionShake.m_IntensityValue = intensityValue;
			minionShake.m_Radius = radius;
			minionShake.ShakeMinion();
			BoardEvents boardEvents = BoardEvents.Get();
			if (boardEvents != null)
			{
				boardEvents.MinionShakeEvent(intensityType, intensityValue);
			}
		}
	}

	// Token: 0x06005074 RID: 20596 RVA: 0x0017E794 File Offset: 0x0017C994
	public static void ShakeTargetMinion(GameObject shakeTarget, ShakeMinionType shakeType, Vector3 impactPoint, ShakeMinionIntensity intensityType, float intensityValue, float radius, float startDelay)
	{
		Spell spell = SceneUtils.FindComponentInThisOrParents<Spell>(shakeTarget);
		if (spell == null)
		{
			Debug.LogWarning("MinionShake: failed to locate Spell component");
			return;
		}
		GameObject visualTarget = spell.GetVisualTarget();
		if (visualTarget == null)
		{
			Debug.LogWarning("MinionShake: failed to Spell GetVisualTarget");
			return;
		}
		MinionShake componentInChildren = visualTarget.GetComponentInChildren<MinionShake>();
		if (componentInChildren == null)
		{
			Debug.LogWarning("MinionShake: failed to locate MinionShake component");
			return;
		}
		componentInChildren.m_StartDelay = startDelay;
		componentInChildren.m_ShakeType = shakeType;
		componentInChildren.m_ImpactPosition = impactPoint;
		componentInChildren.m_ShakeIntensityType = intensityType;
		componentInChildren.m_IntensityValue = intensityValue;
		componentInChildren.m_Radius = radius;
		componentInChildren.ShakeMinion();
	}

	// Token: 0x06005075 RID: 20597 RVA: 0x0017E830 File Offset: 0x0017CA30
	public static void ShakeObject(GameObject shakeObject, ShakeMinionType shakeType, Vector3 impactPoint, ShakeMinionIntensity intensityType, float intensityValue, float radius, float startDelay)
	{
		MinionShake.ShakeObject(shakeObject, shakeType, impactPoint, intensityType, intensityValue, radius, startDelay, false);
	}

	// Token: 0x06005076 RID: 20598 RVA: 0x0017E850 File Offset: 0x0017CA50
	public static void ShakeObject(GameObject shakeObject, ShakeMinionType shakeType, Vector3 impactPoint, ShakeMinionIntensity intensityType, float intensityValue, float radius, float startDelay, bool ignoreAnimationPlaying)
	{
		MinionShake.ShakeObject(shakeObject, shakeType, impactPoint, intensityType, intensityValue, radius, startDelay, false, ignoreAnimationPlaying);
	}

	// Token: 0x06005077 RID: 20599 RVA: 0x0017E870 File Offset: 0x0017CA70
	public static void ShakeObject(GameObject shakeObject, ShakeMinionType shakeType, Vector3 impactPoint, ShakeMinionIntensity intensityType, float intensityValue, float radius, float startDelay, bool ignoreAnimationPlaying, bool ignoreHeight)
	{
		MinionShake componentInChildren = shakeObject.GetComponentInChildren<MinionShake>();
		if (componentInChildren == null)
		{
			Actor actor = SceneUtils.FindComponentInParents<Actor>(shakeObject);
			if (actor == null)
			{
				return;
			}
			componentInChildren = actor.gameObject.GetComponentInChildren<MinionShake>();
			if (componentInChildren == null)
			{
				return;
			}
		}
		componentInChildren.m_StartDelay = startDelay;
		componentInChildren.m_ShakeType = shakeType;
		componentInChildren.m_ImpactPosition = impactPoint;
		componentInChildren.m_ShakeIntensityType = intensityType;
		componentInChildren.m_IntensityValue = intensityValue;
		componentInChildren.m_Radius = radius;
		componentInChildren.m_IgnoreAnimationPlaying = ignoreAnimationPlaying;
		componentInChildren.ShakeMinion();
	}

	// Token: 0x06005078 RID: 20600 RVA: 0x0017E8F8 File Offset: 0x0017CAF8
	public void ShakeAllMinionsRandomMedium()
	{
		Vector3 impactPoint = Vector3.zero;
		Board board = Board.Get();
		if (board != null)
		{
			Transform transform = board.FindBone("CenterPointBone");
			if (transform != null)
			{
				impactPoint = transform.position;
			}
		}
		MinionShake.ShakeAllMinions(base.gameObject, ShakeMinionType.RandomDirection, impactPoint, ShakeMinionIntensity.MediumShake, 0.5f, 0f, 0f);
	}

	// Token: 0x06005079 RID: 20601 RVA: 0x0017E95C File Offset: 0x0017CB5C
	public void ShakeAllMinionsRandomLarge()
	{
		Vector3 impactPoint = Vector3.zero;
		Board board = Board.Get();
		if (board != null)
		{
			Transform transform = board.FindBone("CenterPointBone");
			if (transform != null)
			{
				impactPoint = transform.position;
			}
		}
		MinionShake.ShakeAllMinions(base.gameObject, ShakeMinionType.RandomDirection, impactPoint, ShakeMinionIntensity.LargeShake, 1f, 0f, 0f);
	}

	// Token: 0x0600507A RID: 20602 RVA: 0x0017E9C0 File Offset: 0x0017CBC0
	public void RandomShake(float impact)
	{
		this.m_ShakeIntensityType = ShakeMinionIntensity.Custom;
		this.m_IntensityValue = impact;
		this.m_ShakeType = ShakeMinionType.Angle;
		this.m_ShakeType = ShakeMinionType.RandomDirection;
		this.ShakeMinion();
	}

	// Token: 0x0600507B RID: 20603 RVA: 0x0017E9F0 File Offset: 0x0017CBF0
	private void ShakeMinion()
	{
		if (GraphicsManager.Get() == null || GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.Low)
		{
			return;
		}
		if (this.m_MinionShakeAnimator == null)
		{
			Debug.LogWarning("MinionShake: failed to locate MinionShake Animator");
			return;
		}
		Animation component = base.GetComponent<Animation>();
		if (component != null && component.isPlaying && !this.m_IgnoreAnimationPlaying)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Board board = Board.Get();
		if (board != null)
		{
			Transform transform = board.FindBone("CenterPointBone");
			if (transform != null)
			{
				vector = transform.position;
			}
		}
		if (vector.y - base.transform.position.y > 0.1f && !this.m_IgnoreHeight)
		{
			return;
		}
		if (this.m_MinionShakeInstance == null)
		{
			this.m_MinionShakeInstance = (GameObject)Object.Instantiate(this.m_MinionShakeAnimator, this.OFFSCREEN_POSITION, base.transform.rotation);
			this.m_CardPlayAllyTransform = this.m_MinionShakeInstance.transform.FindChild("Card_Play_Ally").gameObject.transform;
		}
		if (this.m_Animator == null)
		{
			this.m_Animator = this.m_MinionShakeInstance.GetComponent<Animator>();
		}
		if (!this.m_Animating)
		{
			this.m_MinionOrgPos = base.transform.localPosition;
			this.m_MinionOrgRot = base.transform.localRotation;
		}
		if (this.m_ShakeType == ShakeMinionType.Angle)
		{
			this.m_ImpactDirection = this.AngleToVector(this.m_Angle);
		}
		else if (this.m_ShakeType == ShakeMinionType.ImpactDirection)
		{
			this.m_ImpactDirection = Vector3.Normalize(base.transform.position - this.m_ImpactPosition);
		}
		else if (this.m_ShakeType == ShakeMinionType.RandomDirection)
		{
			this.m_ImpactDirection = this.AngleToVector(Random.Range(0f, 360f));
		}
		float num = this.m_IntensityValue;
		if (this.m_ShakeIntensityType == ShakeMinionIntensity.SmallShake)
		{
			num = 0.1f;
		}
		else if (this.m_ShakeIntensityType == ShakeMinionIntensity.MediumShake)
		{
			num = 0.5f;
		}
		else if (this.m_ShakeIntensityType == ShakeMinionIntensity.LargeShake)
		{
			num = 1f;
		}
		this.m_ImpactDirection *= num;
		this.m_Animator.SetFloat("posx", this.m_ImpactDirection.x);
		this.m_Animator.SetFloat("posy", this.m_ImpactDirection.y);
		if (this.m_Radius > 0f && Vector3.Distance(base.transform.position, this.m_ImpactPosition) > this.m_Radius)
		{
			return;
		}
		if (this.m_StartDelay > 0f)
		{
			base.StartCoroutine(this.StartShakeAnimation());
		}
		else
		{
			this.m_Animating = true;
			this.m_Animator.SetBool("shake", true);
		}
		base.StartCoroutine(this.ResetShakeAnimator());
	}

	// Token: 0x0600507C RID: 20604 RVA: 0x0017ED08 File Offset: 0x0017CF08
	private Vector2 AngleToVector(float angle)
	{
		Vector3 vector = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, 0f, -1f);
		return new Vector2(vector.x, vector.z);
	}

	// Token: 0x0600507D RID: 20605 RVA: 0x0017ED54 File Offset: 0x0017CF54
	private IEnumerator StartShakeAnimation()
	{
		yield return new WaitForSeconds(this.m_StartDelay);
		this.m_Animating = true;
		this.m_Animator.SetBool("shake", true);
		yield break;
	}

	// Token: 0x0600507E RID: 20606 RVA: 0x0017ED70 File Offset: 0x0017CF70
	private IEnumerator ResetShakeAnimator()
	{
		yield return new WaitForSeconds(this.m_StartDelay);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		this.m_Animator.SetBool("shake", false);
		yield break;
	}

	// Token: 0x0600507F RID: 20607 RVA: 0x0017ED8C File Offset: 0x0017CF8C
	private static MinionShake[] FindAllMinionShakers(GameObject shakeTrigger)
	{
		Card card = null;
		Spell spell = SceneUtils.FindComponentInThisOrParents<Spell>(shakeTrigger);
		if (spell != null)
		{
			card = spell.GetSourceCard();
		}
		List<MinionShake> list = new List<MinionShake>();
		ZoneMgr zoneMgr = ZoneMgr.Get();
		List<Zone> list2 = zoneMgr.FindZonesForTag(TAG_ZONE.PLAY);
		foreach (Zone zone in list2)
		{
			if (zone.GetType() != typeof(ZoneHero))
			{
				foreach (Card card2 in zone.GetCards())
				{
					if (!(card2 == card))
					{
						MinionShake componentInChildren = card2.GetComponentInChildren<MinionShake>();
						Log.Kyle.Print(string.Format("Minion Shake Search:{0}", card2), new object[0]);
						if (!(componentInChildren == null))
						{
							list.Add(componentInChildren);
							Log.Kyle.Print(string.Format("Minion Shake Found:{0}", card2), new object[0]);
						}
					}
				}
			}
		}
		return list.ToArray();
	}

	// Token: 0x0400371F RID: 14111
	private const float INTENSITY_SMALL = 0.1f;

	// Token: 0x04003720 RID: 14112
	private const float INTENSITY_MEDIUM = 0.5f;

	// Token: 0x04003721 RID: 14113
	private const float INTENSITY_LARGE = 1f;

	// Token: 0x04003722 RID: 14114
	private const float DISABLE_HEIGHT = 0.1f;

	// Token: 0x04003723 RID: 14115
	private readonly Vector3 OFFSCREEN_POSITION = new Vector3(-400f, -400f, -400f);

	// Token: 0x04003724 RID: 14116
	public GameObject m_MinionShakeAnimator;

	// Token: 0x04003725 RID: 14117
	private bool m_Animating;

	// Token: 0x04003726 RID: 14118
	private Animator m_Animator;

	// Token: 0x04003727 RID: 14119
	private Vector2 m_ImpactDirection;

	// Token: 0x04003728 RID: 14120
	private Vector3 m_ImpactPosition;

	// Token: 0x04003729 RID: 14121
	private float m_Angle;

	// Token: 0x0400372A RID: 14122
	private ShakeMinionIntensity m_ShakeIntensityType = ShakeMinionIntensity.MediumShake;

	// Token: 0x0400372B RID: 14123
	private float m_IntensityValue = 0.5f;

	// Token: 0x0400372C RID: 14124
	private ShakeMinionType m_ShakeType = ShakeMinionType.RandomDirection;

	// Token: 0x0400372D RID: 14125
	private GameObject m_MinionShakeInstance;

	// Token: 0x0400372E RID: 14126
	private Transform m_CardPlayAllyTransform;

	// Token: 0x0400372F RID: 14127
	private Vector3 m_MinionOrgPos;

	// Token: 0x04003730 RID: 14128
	private Quaternion m_MinionOrgRot;

	// Token: 0x04003731 RID: 14129
	private float m_StartDelay;

	// Token: 0x04003732 RID: 14130
	private float m_Radius;

	// Token: 0x04003733 RID: 14131
	private bool m_IgnoreAnimationPlaying;

	// Token: 0x04003734 RID: 14132
	private bool m_IgnoreHeight;

	// Token: 0x04003735 RID: 14133
	private static int s_IdleState = Animator.StringToHash("Base.Idle");
}
