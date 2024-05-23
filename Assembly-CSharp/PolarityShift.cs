using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E83 RID: 3715
public class PolarityShift : SuperSpell
{
	// Token: 0x06007080 RID: 28800 RVA: 0x002119D9 File Offset: 0x0020FBD9
	protected override void Awake()
	{
		this.m_Sound = base.GetComponent<AudioSource>();
		base.Awake();
	}

	// Token: 0x06007081 RID: 28801 RVA: 0x002119F0 File Offset: 0x0020FBF0
	protected override void OnAction(SpellStateType prevStateType)
	{
		if (this.m_HeightCurve.length == 0)
		{
			Debug.LogWarning("PolarityShift Spell height animation curve in not defined");
			base.OnAction(prevStateType);
			return;
		}
		if (this.m_RotationDriftCurve.length == 0)
		{
			Debug.LogWarning("PolarityShift Spell rotation drift animation curve in not defined");
			base.OnAction(prevStateType);
			return;
		}
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		this.m_HeightCurveLength = this.m_HeightCurve.keys[this.m_HeightCurve.length - 1].time;
		this.m_ParticleEffects.m_ParticleSystems.Clear();
		List<PolarityShift.MinionData> list = new List<PolarityShift.MinionData>();
		foreach (GameObject gameObject in base.GetTargets())
		{
			PolarityShift.MinionData minionData = new PolarityShift.MinionData();
			minionData.gameObject = gameObject;
			minionData.orgLocPos = gameObject.transform.localPosition;
			minionData.orgLocRot = gameObject.transform.localRotation;
			float num = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value);
			float num2 = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value) * 0.1f;
			float num3 = Mathf.Lerp(-this.m_RotationDriftAmount, this.m_RotationDriftAmount, Random.value);
			minionData.rotationDrift = new Vector3(num, num2, num3);
			minionData.glowParticle = Object.Instantiate<ParticleSystem>(this.m_GlowParticle);
			minionData.glowParticle.transform.position = gameObject.transform.position;
			minionData.glowParticle.transform.Translate(0f, this.m_ParticleHeightOffset, 0f, 0);
			minionData.lightningParticle = Object.Instantiate<ParticleSystem>(this.m_LightningParticle);
			minionData.lightningParticle.transform.position = gameObject.transform.position;
			minionData.lightningParticle.transform.Translate(0f, this.m_ParticleHeightOffset, 0f, 0);
			minionData.impactParticle = Object.Instantiate<ParticleSystem>(this.m_ImpactParticle);
			minionData.impactParticle.transform.position = gameObject.transform.position;
			minionData.impactParticle.transform.Translate(0f, this.m_ParticleHeightOffset, 0f, 0);
			this.m_ParticleEffects.m_ParticleSystems.Add(minionData.lightningParticle);
			if (this.m_Sound != null)
			{
				SoundManager.Get().Play(this.m_Sound);
			}
			list.Add(minionData);
		}
		base.StartCoroutine(this.DoSpellFinished());
		base.StartCoroutine(this.MinionAnimation(list));
	}

	// Token: 0x06007082 RID: 28802 RVA: 0x00211CB8 File Offset: 0x0020FEB8
	private IEnumerator DoSpellFinished()
	{
		yield return new WaitForSeconds(this.m_SpellFinishTime);
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
		yield break;
	}

	// Token: 0x06007083 RID: 28803 RVA: 0x00211CD4 File Offset: 0x0020FED4
	private IEnumerator MinionAnimation(List<PolarityShift.MinionData> minions)
	{
		foreach (PolarityShift.MinionData minion in minions)
		{
			minion.glowParticle.Play();
		}
		this.m_AnimTime = 0f;
		while (this.m_AnimTime < this.m_HeightCurveLength)
		{
			this.m_AnimTime += Time.deltaTime;
			float height = this.m_HeightCurve.Evaluate(this.m_AnimTime);
			float rotAmount = this.m_RotationDriftCurve.Evaluate(this.m_AnimTime);
			foreach (PolarityShift.MinionData minion2 in minions)
			{
				minion2.gameObject.transform.localPosition = new Vector3(minion2.orgLocPos.x, minion2.orgLocPos.y + height, minion2.orgLocPos.z);
				minion2.gameObject.transform.localRotation = minion2.orgLocRot;
				minion2.gameObject.transform.Rotate(minion2.rotationDrift * rotAmount, 1);
			}
			yield return null;
		}
		foreach (PolarityShift.MinionData minion3 in minions)
		{
			minion3.impactParticle.Play();
			minion3.lightningParticle.Play();
			MinionShake.ShakeObject(minion3.gameObject, ShakeMinionType.RandomDirection, minion3.gameObject.transform.position, ShakeMinionIntensity.MediumShake, 0f, 0f, 0f);
		}
		Camera mainCamera = Camera.main;
		if (mainCamera != null && minions.Count > 0)
		{
			this.ShakeCamera();
			FullScreenEffects fsfx = mainCamera.GetComponent<FullScreenEffects>();
			if (fsfx != null && !fsfx.isActive())
			{
				fsfx.BlendToColorEnable = true;
				fsfx.BlendToColorAmount = 1f;
				fsfx.BlendToColor = Color.white;
				yield return null;
				fsfx.BlendToColorAmount = 0.67f;
				yield return null;
				fsfx.BlendToColorAmount = 0.33f;
				yield return null;
				fsfx.BlendToColorAmount = 0f;
				fsfx.BlendToColorEnable = false;
				fsfx.Disable();
			}
		}
		if (minions.Count > 0)
		{
			yield return new WaitForSeconds(this.m_CleanupTime);
			this.m_ParticleEffects.m_ParticleSystems.Clear();
			foreach (PolarityShift.MinionData minion4 in minions)
			{
				Object.Destroy(minion4.glowParticle.gameObject);
				Object.Destroy(minion4.lightningParticle.gameObject);
				Object.Destroy(minion4.impactParticle.gameObject);
			}
		}
		this.OnStateFinished();
		yield break;
	}

	// Token: 0x06007084 RID: 28804 RVA: 0x00211CFD File Offset: 0x0020FEFD
	private void ShakeCamera()
	{
		CameraShakeMgr.Shake(Camera.main, new Vector3(0.1f, 0.1f, 0.1f), 0.75f);
	}

	// Token: 0x040059B6 RID: 22966
	public AnimationCurve m_HeightCurve;

	// Token: 0x040059B7 RID: 22967
	public float m_RotationDriftAmount;

	// Token: 0x040059B8 RID: 22968
	public AnimationCurve m_RotationDriftCurve;

	// Token: 0x040059B9 RID: 22969
	public float m_ParticleHeightOffset = 0.1f;

	// Token: 0x040059BA RID: 22970
	public ParticleSystem m_GlowParticle;

	// Token: 0x040059BB RID: 22971
	public ParticleSystem m_LightningParticle;

	// Token: 0x040059BC RID: 22972
	public ParticleSystem m_ImpactParticle;

	// Token: 0x040059BD RID: 22973
	public ParticleEffects m_ParticleEffects;

	// Token: 0x040059BE RID: 22974
	public float m_CleanupTime = 2f;

	// Token: 0x040059BF RID: 22975
	public float m_SpellFinishTime = 2f;

	// Token: 0x040059C0 RID: 22976
	private float m_HeightCurveLength;

	// Token: 0x040059C1 RID: 22977
	private float m_AnimTime;

	// Token: 0x040059C2 RID: 22978
	private AudioSource m_Sound;

	// Token: 0x02000E84 RID: 3716
	public class MinionData
	{
		// Token: 0x040059C3 RID: 22979
		public GameObject gameObject;

		// Token: 0x040059C4 RID: 22980
		public Vector3 orgLocPos;

		// Token: 0x040059C5 RID: 22981
		public Quaternion orgLocRot;

		// Token: 0x040059C6 RID: 22982
		public Vector3 rotationDrift;

		// Token: 0x040059C7 RID: 22983
		public ParticleSystem glowParticle;

		// Token: 0x040059C8 RID: 22984
		public ParticleSystem lightningParticle;

		// Token: 0x040059C9 RID: 22985
		public ParticleSystem impactParticle;
	}
}
