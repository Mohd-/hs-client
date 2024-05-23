using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E3F RID: 3647
public class BouncingBlade : SuperSpell
{
	// Token: 0x06006F05 RID: 28421 RVA: 0x002090B4 File Offset: 0x002072B4
	protected override void Awake()
	{
		base.Awake();
		Log.Kyle.Print("Awake()", new object[0]);
		this.m_BladeRoot.SetActive(false);
		this.m_PreviousHitBone = this.m_HitBones[this.m_HitBones.Count - 1];
		this.m_OrgBladeScale = this.m_BladeRoot.transform.localScale;
		this.m_BladeRoot.transform.localScale = Vector3.zero;
	}

	// Token: 0x06006F06 RID: 28422 RVA: 0x00209131 File Offset: 0x00207331
	protected override void Start()
	{
		base.Start();
		Log.Kyle.Print("Start()", new object[0]);
		this.SetupBounceLocations();
	}

	// Token: 0x06006F07 RID: 28423 RVA: 0x00209154 File Offset: 0x00207354
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		if (this.m_targets.Count == 0)
		{
			this.m_isDone = true;
			this.m_BladeRoot.SetActive(false);
			this.m_effectsPendingFinish--;
			base.FinishIfPossible();
			return;
		}
		if (!this.m_Running)
		{
			this.m_BladeRoot.SetActive(true);
			this.m_Blade.SetActive(false);
			this.m_Trail.SetActive(false);
			this.m_Running = true;
			base.StartCoroutine(this.BladeRunner());
		}
		this.m_BladeRoot.transform.localScale = this.m_OrgBladeScale;
		this.m_isDone = false;
		bool flag = base.IsHandlingLastTaskList();
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			GameObject gameObject = this.m_targets[i];
			int metaDataIndexForTarget = base.GetMetaDataIndexForTarget(i);
			BouncingBlade.Target target = new BouncingBlade.Target();
			target.VisualTarget = gameObject;
			target.TargetPosition = gameObject.transform.position;
			target.MetaDataIdx = metaDataIndexForTarget;
			target.isMinion = true;
			if (i == this.m_targets.Count - 1)
			{
				target.LastTarget = true;
			}
			if (flag)
			{
				target.LastBlock = true;
			}
			this.m_TargetQueue.Add(target);
			if (!target.LastTarget)
			{
				BouncingBlade.Target target2 = new BouncingBlade.Target();
				target2.TargetPosition = this.AcquireRandomBoardTarget(out target2.Offscreen);
				target2.isMinion = false;
				target2.LastTarget = false;
				this.m_TargetQueue.Add(target2);
			}
		}
	}

	// Token: 0x06006F08 RID: 28424 RVA: 0x002092F4 File Offset: 0x002074F4
	private IEnumerator BladeRunner()
	{
		while (!this.m_isDone)
		{
			while (this.m_TargetQueue.Count > 0)
			{
				if (!this.m_Blade.activeSelf)
				{
					this.m_Blade.SetActive(true);
					if (this.m_BladeSpinning != null)
					{
						this.m_BladeSpinning.gameObject.SetActive(true);
						SoundManager.Get().Play(this.m_BladeSpinning);
					}
					if (this.m_BladeSpinningContinuous != null)
					{
						this.m_BladeSpinningContinuous.gameObject.SetActive(true);
						SoundManager.Get().Play(this.m_BladeSpinningContinuous);
					}
					if (this.m_StartSound != null)
					{
						SoundManager.Get().Play(this.m_StartSound);
					}
				}
				if (!this.m_Trail.activeSelf)
				{
					this.m_Trail.SetActive(true);
				}
				BouncingBlade.Target target = this.m_TargetQueue[0];
				if (target.isMinion)
				{
					int metaDataIdx = target.MetaDataIdx;
					yield return base.StartCoroutine(base.CompleteTasksUntilMetaData(metaDataIdx));
					this.AnimateToNextTarget(target);
					while (this.m_Animating)
					{
						yield return null;
					}
					if (metaDataIdx > 0)
					{
						yield return base.StartCoroutine(base.CompleteTasksFromMetaData(metaDataIdx, 0f));
					}
					if (target.LastBlock && target.LastTarget)
					{
						Log.Kyle.Print("BladeRunner() - Finish Spell", new object[0]);
						this.m_EndSparkParticles.Play();
						this.m_EndBigSparkParticles.Play();
						this.m_Blade.SetActive(false);
						if (this.m_BladeSpinning != null)
						{
							SoundManager.Get().Stop(this.m_BladeSpinning);
						}
						if (this.m_BladeSpinningContinuous != null)
						{
							SoundManager.Get().Stop(this.m_BladeSpinningContinuous);
						}
						if (this.m_EndSound != null)
						{
							SoundManager.Get().Play(this.m_EndSound);
						}
						yield return new WaitForSeconds(0.8f);
						this.m_effectsPendingFinish--;
						base.FinishIfPossible();
						this.m_BladeRoot.SetActive(true);
						yield break;
					}
					if (!target.LastBlock && target.LastTarget)
					{
						Log.Kyle.Print("BladeRunner() - Finish Action", new object[0]);
						this.m_effectsPendingFinish--;
						base.FinishIfPossible();
					}
				}
				else
				{
					this.AnimateToNextTarget(target);
					while (this.m_Animating)
					{
						yield return null;
					}
				}
				this.m_TargetQueue.RemoveAt(0);
				yield return null;
			}
			BouncingBlade.Target randomTarget = new BouncingBlade.Target();
			randomTarget.TargetPosition = this.AcquireRandomBoardTarget(out randomTarget.Offscreen);
			randomTarget.isMinion = false;
			randomTarget.LastTarget = false;
			this.AnimateToNextTarget(randomTarget);
			while (this.m_Animating)
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06006F09 RID: 28425 RVA: 0x00209310 File Offset: 0x00207510
	private void SetupBounceLocations()
	{
		Board board = Board.Get();
		Vector3 position = board.FindBone("CenterPointBone").transform.position;
		Vector3 localPosition = this.m_HitBonesRoot.transform.localPosition;
		this.m_HitBonesRoot.transform.position = position;
		foreach (BouncingBlade.HitBonesType hitBonesType in this.m_HitBones)
		{
			hitBonesType.SetPosition(hitBonesType.Bone.transform.position);
		}
		this.m_HitBonesRoot.transform.localPosition = localPosition;
	}

	// Token: 0x06006F0A RID: 28426 RVA: 0x002093CC File Offset: 0x002075CC
	private void AnimateToNextTarget(BouncingBlade.Target target)
	{
		this.m_Animating = true;
		iTween.MoveTo(this.m_BladeRoot, iTween.Hash(new object[]
		{
			"position",
			target.TargetPosition,
			"speed",
			50f,
			"orienttopath",
			true,
			"easetype",
			iTween.EaseType.linear,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"AnimationComplete",
			"oncompleteparams",
			target
		}));
	}

	// Token: 0x06006F0B RID: 28427 RVA: 0x00209478 File Offset: 0x00207678
	private void RampBladeVolume()
	{
		iTween.StopByName(this.m_BladeSpinning.gameObject, "BladeSpinningSound");
		SoundManager.Get().SetVolume(this.m_BladeSpinning, this.m_BladeSpinningMinVol);
		Action<object> action = delegate(object amount)
		{
			SoundManager.Get().SetVolume(this.m_BladeSpinning, (float)amount);
		};
		iTween.ValueTo(this.m_BladeSpinning.gameObject, iTween.Hash(new object[]
		{
			"name",
			"BladeSpinningSound",
			"from",
			this.m_BladeSpinningMinVol,
			"to",
			this.m_BladeSpinningMaxVol,
			"time",
			this.m_BladeSpinningRampTime,
			"easetype",
			iTween.EaseType.linear,
			"onupdate",
			action,
			"onupdatetarget",
			this.m_BladeSpinning.gameObject
		}));
	}

	// Token: 0x06006F0C RID: 28428 RVA: 0x00209564 File Offset: 0x00207764
	private void AnimationComplete(BouncingBlade.Target target)
	{
		this.m_Animating = false;
		this.AnimateSparks();
		if (!target.LastBlock && !target.LastTarget)
		{
			this.RampBladeVolume();
		}
		AudioSource audioSource;
		if (target.isMinion)
		{
			audioSource = this.m_BladeHitMinion;
		}
		else if (target.Offscreen)
		{
			audioSource = this.m_BladeHitOffScreen;
		}
		else
		{
			audioSource = this.m_BladeHitBoardCorner;
		}
		if (audioSource != null)
		{
			audioSource.gameObject.transform.position = target.TargetPosition;
			SoundManager.Get().Play(audioSource);
		}
	}

	// Token: 0x06006F0D RID: 28429 RVA: 0x00209600 File Offset: 0x00207800
	private void AnimateSparks()
	{
		foreach (ParticleSystem particleSystem in this.m_SparkParticles)
		{
			particleSystem.Play();
		}
	}

	// Token: 0x06006F0E RID: 28430 RVA: 0x0020965C File Offset: 0x0020785C
	private Vector3 AcquireRandomBoardTarget(out bool offscreen)
	{
		offscreen = false;
		if (Random.Range(1, 100) < 5)
		{
			offscreen = true;
		}
		List<BouncingBlade.HitBonesType> list = new List<BouncingBlade.HitBonesType>();
		if (offscreen)
		{
			foreach (BouncingBlade.HitBonesType hitBonesType in this.m_HitBones)
			{
				if (hitBonesType.Direction != BouncingBlade.HIT_DIRECTIONS.E && hitBonesType.Direction != BouncingBlade.HIT_DIRECTIONS.NE && hitBonesType.Direction != BouncingBlade.HIT_DIRECTIONS.NW && hitBonesType.Direction != BouncingBlade.HIT_DIRECTIONS.SE && hitBonesType.Direction != BouncingBlade.HIT_DIRECTIONS.SW)
				{
					if (hitBonesType.Direction != this.m_PreviousHitBone.Direction)
					{
						list.Add(hitBonesType);
					}
				}
			}
		}
		else
		{
			foreach (BouncingBlade.HitBonesType hitBonesType2 in this.m_HitBones)
			{
				if (hitBonesType2.Direction != BouncingBlade.HIT_DIRECTIONS.E_OFFSCREEN && hitBonesType2.Direction != BouncingBlade.HIT_DIRECTIONS.N_OFFSCREEN && hitBonesType2.Direction != BouncingBlade.HIT_DIRECTIONS.S_OFFSCREEN && hitBonesType2.Direction != BouncingBlade.HIT_DIRECTIONS.W_OFFSCREEN)
				{
					if (hitBonesType2.Direction != this.m_PreviousHitBone.Direction)
					{
						list.Add(hitBonesType2);
					}
				}
			}
		}
		int num = Random.Range(0, list.Count);
		this.m_PreviousHitBone = list[num];
		return list[num].GetPosition();
	}

	// Token: 0x040057FE RID: 22526
	private const float DAMAGE_SPLAT_DELAY = 0f;

	// Token: 0x040057FF RID: 22527
	private const float BLADE_ANIMATION_SPEED = 50f;

	// Token: 0x04005800 RID: 22528
	private const float BLADE_BIRTH_TIME = 0.3f;

	// Token: 0x04005801 RID: 22529
	private const int OFFSCREEN_HIT_PERCENT = 5;

	// Token: 0x04005802 RID: 22530
	public GameObject m_BladeRoot;

	// Token: 0x04005803 RID: 22531
	public GameObject m_Blade;

	// Token: 0x04005804 RID: 22532
	public GameObject m_Trail;

	// Token: 0x04005805 RID: 22533
	public GameObject m_HitBonesRoot;

	// Token: 0x04005806 RID: 22534
	public List<ParticleSystem> m_SparkParticles;

	// Token: 0x04005807 RID: 22535
	public ParticleSystem m_EndSparkParticles;

	// Token: 0x04005808 RID: 22536
	public ParticleSystem m_EndBigSparkParticles;

	// Token: 0x04005809 RID: 22537
	public List<BouncingBlade.HitBonesType> m_HitBones;

	// Token: 0x0400580A RID: 22538
	public AudioSource m_BladeSpinning;

	// Token: 0x0400580B RID: 22539
	public AudioSource m_BladeSpinningContinuous;

	// Token: 0x0400580C RID: 22540
	public AudioSource m_BladeHitMinion;

	// Token: 0x0400580D RID: 22541
	public AudioSource m_BladeHitBoardCorner;

	// Token: 0x0400580E RID: 22542
	public AudioSource m_BladeHitOffScreen;

	// Token: 0x0400580F RID: 22543
	public AudioSource m_StartSound;

	// Token: 0x04005810 RID: 22544
	public AudioSource m_EndSound;

	// Token: 0x04005811 RID: 22545
	public float m_BladeSpinningMinVol;

	// Token: 0x04005812 RID: 22546
	public float m_BladeSpinningMaxVol = 1f;

	// Token: 0x04005813 RID: 22547
	public float m_BladeSpinningRampTime = 0.3f;

	// Token: 0x04005814 RID: 22548
	private bool m_Running;

	// Token: 0x04005815 RID: 22549
	private List<BouncingBlade.Target> m_TargetQueue = new List<BouncingBlade.Target>();

	// Token: 0x04005816 RID: 22550
	private Vector3? m_NextPosition;

	// Token: 0x04005817 RID: 22551
	private bool m_Animating;

	// Token: 0x04005818 RID: 22552
	private bool m_isDone;

	// Token: 0x04005819 RID: 22553
	private BouncingBlade.HitBonesType m_PreviousHitBone;

	// Token: 0x0400581A RID: 22554
	private Vector3 m_OrgBladeScale;

	// Token: 0x02000E40 RID: 3648
	public enum HIT_DIRECTIONS
	{
		// Token: 0x0400581C RID: 22556
		NW,
		// Token: 0x0400581D RID: 22557
		NE,
		// Token: 0x0400581E RID: 22558
		E,
		// Token: 0x0400581F RID: 22559
		SW,
		// Token: 0x04005820 RID: 22560
		SE,
		// Token: 0x04005821 RID: 22561
		N_OFFSCREEN,
		// Token: 0x04005822 RID: 22562
		E_OFFSCREEN,
		// Token: 0x04005823 RID: 22563
		W_OFFSCREEN,
		// Token: 0x04005824 RID: 22564
		S_OFFSCREEN
	}

	// Token: 0x02000E41 RID: 3649
	[Serializable]
	public class HitBonesType
	{
		// Token: 0x06006F11 RID: 28433 RVA: 0x0020981C File Offset: 0x00207A1C
		public void SetPosition(Vector3 pos)
		{
			this.m_Position = pos;
		}

		// Token: 0x06006F12 RID: 28434 RVA: 0x00209825 File Offset: 0x00207A25
		public Vector3 GetPosition()
		{
			return this.m_Position;
		}

		// Token: 0x04005825 RID: 22565
		public BouncingBlade.HIT_DIRECTIONS Direction;

		// Token: 0x04005826 RID: 22566
		public GameObject Bone;

		// Token: 0x04005827 RID: 22567
		private Vector3 m_Position;
	}

	// Token: 0x02000E42 RID: 3650
	[Serializable]
	public class Target
	{
		// Token: 0x04005828 RID: 22568
		public GameObject VisualTarget;

		// Token: 0x04005829 RID: 22569
		public Vector3 TargetPosition;

		// Token: 0x0400582A RID: 22570
		public bool isMinion;

		// Token: 0x0400582B RID: 22571
		public int MetaDataIdx;

		// Token: 0x0400582C RID: 22572
		public bool LastTarget;

		// Token: 0x0400582D RID: 22573
		public bool LastBlock;

		// Token: 0x0400582E RID: 22574
		public bool Offscreen;
	}
}
