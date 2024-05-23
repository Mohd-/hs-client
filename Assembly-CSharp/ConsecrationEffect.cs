using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E4D RID: 3661
public class ConsecrationEffect : MonoBehaviour
{
	// Token: 0x06006F44 RID: 28484 RVA: 0x0020A581 File Offset: 0x00208781
	private void Awake()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_LiftHeightMin = 3f;
			this.m_LiftHeightMax = 5f;
		}
	}

	// Token: 0x06006F45 RID: 28485 RVA: 0x0020A5A8 File Offset: 0x002087A8
	private void Start()
	{
		Spell component = base.GetComponent<Spell>();
		if (component == null)
		{
			base.enabled = false;
		}
		this.m_SuperSpell = component.GetSuperSpellParent();
		this.m_ImpactSound = base.GetComponent<AudioSource>();
		this.m_ImpactObjects = new List<GameObject>();
	}

	// Token: 0x06006F46 RID: 28486 RVA: 0x0020A5F4 File Offset: 0x002087F4
	private void OnDestroy()
	{
		if (this.m_ImpactObjects.Count > 0)
		{
			foreach (GameObject gameObject in this.m_ImpactObjects)
			{
				Object.Destroy(gameObject);
			}
		}
	}

	// Token: 0x06006F47 RID: 28487 RVA: 0x0020A660 File Offset: 0x00208860
	private void StartAnimation()
	{
		if (this.m_SuperSpell == null)
		{
			return;
		}
		int num = 0;
		foreach (GameObject gameObject in this.m_SuperSpell.GetTargets())
		{
			Vector3 position = gameObject.transform.position;
			Quaternion rotation = gameObject.transform.rotation;
			num++;
			float num2 = Random.Range(this.m_StartDelayMin, this.m_StartDelayMax);
			GameObject gameObject2 = (GameObject)Object.Instantiate(this.m_StartImpact, position, rotation);
			this.m_ImpactObjects.Add(gameObject2);
			foreach (ParticleSystem particleSystem in gameObject2.GetComponentsInChildren<ParticleSystem>())
			{
				particleSystem.startDelay = num2;
				particleSystem.Play();
			}
			num2 += 0.2f;
			float num3 = Random.Range(this.m_LiftHeightMin, this.m_LiftHeightMax);
			Hashtable args = iTween.Hash(new object[]
			{
				"time",
				this.m_LiftTime,
				"delay",
				num2,
				"position",
				new Vector3(position.x, position.y + num3, position.z),
				"easetype",
				iTween.EaseType.easeOutQuad,
				"name",
				string.Format("Lift_{0}_{1}", gameObject.name, num)
			});
			iTween.MoveTo(gameObject, args);
			Vector3 eulerAngles = rotation.eulerAngles;
			eulerAngles.x += Random.Range(this.m_LiftRotMin, this.m_LiftRotMax);
			eulerAngles.z += Random.Range(this.m_LiftRotMin, this.m_LiftRotMax);
			Hashtable args2 = iTween.Hash(new object[]
			{
				"time",
				this.m_LiftTime + this.m_HoverTime + this.m_SlamTime * 0.8f,
				"delay",
				num2,
				"rotation",
				eulerAngles,
				"easetype",
				iTween.EaseType.easeOutQuad,
				"name",
				string.Format("LiftRot_{0}_{1}", gameObject.name, num)
			});
			iTween.RotateTo(gameObject, args2);
			float num4 = this.m_StartDelayMax + this.m_LiftTime;
			float num5 = num4 + this.m_HoverTime;
			Hashtable args3 = iTween.Hash(new object[]
			{
				"time",
				this.m_SlamTime,
				"delay",
				num5,
				"position",
				position,
				"easetype",
				iTween.EaseType.easeInCubic,
				"name",
				string.Format("SlamPos_{0}_{1}", gameObject.name, num)
			});
			iTween.MoveTo(gameObject, args3);
			Hashtable args4 = iTween.Hash(new object[]
			{
				"time",
				this.m_SlamTime * 0.8f,
				"delay",
				num5 + this.m_SlamTime * 0.2f,
				"rotation",
				Vector3.zero,
				"easetype",
				iTween.EaseType.easeInQuad,
				"oncomplete",
				"Finished",
				"oncompletetarget",
				base.gameObject,
				"name",
				string.Format("SlamRot_{0}_{1}", gameObject.name, num)
			});
			iTween.RotateTo(gameObject, args4);
			this.m_TotalTime = num5 + this.m_SlamTime;
			if (gameObject.GetComponentInChildren<MinionShake>())
			{
				MinionShake.ShakeObject(gameObject, ShakeMinionType.RandomDirection, gameObject.transform.position, ShakeMinionIntensity.LargeShake, 1f, 0.1f, num5 + this.m_SlamTime, true, true);
			}
			else
			{
				Bounce bounce = gameObject.GetComponent<Bounce>();
				if (bounce == null)
				{
					bounce = gameObject.AddComponent<Bounce>();
				}
				bounce.m_BounceAmount = num3 * this.m_Bounceness;
				bounce.m_BounceSpeed = 3.5f * Random.Range(0.8f, 1.3f);
				bounce.m_BounceCount = 3;
				bounce.m_Bounceness = this.m_Bounceness;
				bounce.m_Delay = num5 + this.m_SlamTime;
				bounce.StartAnimation();
			}
			GameObject gameObject3 = (GameObject)Object.Instantiate(this.m_EndImpact, position, rotation);
			this.m_ImpactObjects.Add(gameObject3);
			foreach (ParticleSystem particleSystem2 in gameObject3.GetComponentsInChildren<ParticleSystem>())
			{
				particleSystem2.startDelay = num5 + this.m_SlamTime;
				particleSystem2.Play();
			}
		}
	}

	// Token: 0x06006F48 RID: 28488 RVA: 0x0020AB7C File Offset: 0x00208D7C
	private void Finished()
	{
		SoundManager.Get().Play(this.m_ImpactSound);
		CameraShakeMgr.Shake(Camera.main, new Vector3(0.15f, 0.15f, 0.15f), 0.9f);
	}

	// Token: 0x0400585E RID: 22622
	public float m_StartDelayMin = 2f;

	// Token: 0x0400585F RID: 22623
	public float m_StartDelayMax = 3f;

	// Token: 0x04005860 RID: 22624
	public float m_LiftTime = 1f;

	// Token: 0x04005861 RID: 22625
	public float m_LiftHeightMin = 2f;

	// Token: 0x04005862 RID: 22626
	public float m_LiftHeightMax = 3f;

	// Token: 0x04005863 RID: 22627
	public float m_LiftRotMin = -15f;

	// Token: 0x04005864 RID: 22628
	public float m_LiftRotMax = 15f;

	// Token: 0x04005865 RID: 22629
	public float m_HoverTime = 0.8f;

	// Token: 0x04005866 RID: 22630
	public float m_SlamTime = 0.2f;

	// Token: 0x04005867 RID: 22631
	public float m_Bounceness = 0.2f;

	// Token: 0x04005868 RID: 22632
	public GameObject m_StartImpact;

	// Token: 0x04005869 RID: 22633
	public GameObject m_EndImpact;

	// Token: 0x0400586A RID: 22634
	public float m_TotalTime;

	// Token: 0x0400586B RID: 22635
	private SuperSpell m_SuperSpell;

	// Token: 0x0400586C RID: 22636
	private List<GameObject> m_ImpactObjects;

	// Token: 0x0400586D RID: 22637
	private AudioSource m_ImpactSound;
}
