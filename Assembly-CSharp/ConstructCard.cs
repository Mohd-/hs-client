using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000EFB RID: 3835
public class ConstructCard : MonoBehaviour
{
	// Token: 0x0600729C RID: 29340 RVA: 0x0021B2F4 File Offset: 0x002194F4
	private void OnDisable()
	{
		this.Cancel();
	}

	// Token: 0x0600729D RID: 29341 RVA: 0x0021B2FC File Offset: 0x002194FC
	public void Construct()
	{
		base.StartCoroutine(this.DoConstruct());
	}

	// Token: 0x0600729E RID: 29342 RVA: 0x0021B30C File Offset: 0x0021950C
	private IEnumerator DoConstruct()
	{
		this.m_Actor = SceneUtils.FindComponentInThisOrParents<Actor>(base.gameObject);
		if (this.m_Actor == null)
		{
			Debug.LogError(string.Format("{0} Ghost card effect failed to find Actor!", base.transform.root.name));
			base.enabled = false;
			yield break;
		}
		this.m_Actor.HideAllText();
		this.m_GhostSpell = this.m_Actor.GetSpell(SpellType.GHOSTMODE);
		this.m_GhostSpell.ActivateState(SpellStateType.CANCEL);
		this.m_Actor.DeactivateSpell(SpellType.GHOSTMODE);
		while (this.m_GhostSpell.IsActive())
		{
			yield return new WaitForEndOfFrame();
		}
		this.m_Actor.HideAllText();
		this.Init();
		this.CreateInstances();
		this.ApplyGhostMaterials();
		if (this.m_GhostGlow)
		{
			if (this.m_Actor.IsElite() && this.m_GhostTextureUnique)
			{
				this.m_GhostGlow.GetComponent<Renderer>().material.mainTexture = this.m_GhostTextureUnique;
			}
			this.m_GhostGlow.GetComponent<Renderer>().enabled = true;
			this.m_GhostGlow.GetComponent<Animation>().Play("GhostModeHot", 4);
		}
		if (this.m_RarityGemMesh)
		{
			this.m_RarityGemMesh.GetComponent<Renderer>().enabled = false;
		}
		if (this.m_RarityFrameMesh)
		{
			this.m_RarityFrameMesh.GetComponent<Renderer>().enabled = false;
		}
		if (this.m_ManaGemStartPosition)
		{
			this.AnimateManaGem();
		}
		if (this.m_DescriptionStartPosition)
		{
			this.AnimateDescription();
		}
		if (this.m_AttackStartPosition)
		{
			this.AnimateAttack();
		}
		if (this.m_HealthStartPosition)
		{
			this.AnimateHealth();
		}
		if (this.m_PortraitStartPosition)
		{
			this.AnimatePortrait();
		}
		if (this.m_NameStartPosition)
		{
			this.AnimateName();
		}
		if (this.m_Actor.GetCardSet() != TAG_CARD_SET.CORE && this.m_RarityStartPosition)
		{
			this.AnimateRarity();
		}
		yield break;
	}

	// Token: 0x0600729F RID: 29343 RVA: 0x0021B328 File Offset: 0x00219528
	private void Init()
	{
		if (this.isInit)
		{
			return;
		}
		this.m_Actor = SceneUtils.FindComponentInThisOrParents<Actor>(base.gameObject);
		if (this.m_Actor == null)
		{
			Debug.LogError(string.Format("{0} Ghost card effect failed to find Actor!", base.transform.root.name));
			base.enabled = false;
			return;
		}
		this.m_CardMesh = this.m_Actor.m_cardMesh;
		this.m_CardFrontIdx = this.m_Actor.m_cardFrontMatIdx;
		this.m_PortraitMesh = this.m_Actor.m_portraitMesh;
		this.m_PortraitFrameIdx = this.m_Actor.m_portraitFrameMatIdx;
		this.m_NameMesh = this.m_Actor.m_nameBannerMesh;
		this.m_DescriptionMesh = this.m_Actor.m_descriptionMesh;
		this.m_DescriptionTrimMesh = this.m_Actor.m_descriptionTrimMesh;
		this.m_RarityGemMesh = this.m_Actor.m_rarityGemMesh;
		this.m_RarityFrameMesh = this.m_Actor.m_rarityFrameMesh;
		if (this.m_Actor.m_attackObject)
		{
			Renderer component = this.m_Actor.m_attackObject.GetComponent<Renderer>();
			if (component != null)
			{
				this.m_AttackMesh = component.gameObject;
			}
			if (this.m_AttackMesh == null)
			{
				Renderer[] componentsInChildren = this.m_Actor.m_attackObject.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					if (!renderer.GetComponent<UberText>())
					{
						this.m_AttackMesh = renderer.gameObject;
					}
				}
			}
		}
		if (this.m_Actor.m_healthObject)
		{
			Renderer component2 = this.m_Actor.m_healthObject.GetComponent<Renderer>();
			if (component2 != null)
			{
				this.m_HealthMesh = component2.gameObject;
			}
			if (this.m_HealthMesh == null)
			{
				Renderer[] componentsInChildren2 = this.m_Actor.m_healthObject.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer2 in componentsInChildren2)
				{
					if (!renderer2.GetComponent<UberText>())
					{
						this.m_HealthMesh = renderer2.gameObject;
					}
				}
			}
		}
		this.m_ManaCostMesh = this.m_Actor.m_manaObject;
		this.m_RacePlateMesh = this.m_Actor.m_racePlateObject;
		this.m_EliteMesh = this.m_Actor.m_eliteObject;
		this.StoreOrgMaterials();
		switch (this.m_Actor.GetRarity())
		{
		case TAG_RARITY.RARE:
			this.m_AnimationScale = this.m_AnimationRarityScaleRare;
			break;
		case TAG_RARITY.EPIC:
			this.m_AnimationScale = this.m_AnimationRarityScaleEpic;
			break;
		case TAG_RARITY.LEGENDARY:
			this.m_AnimationScale = this.m_AnimationRarityScaleLegendary;
			break;
		default:
			this.m_AnimationScale = this.m_AnimationRarityScaleCommon;
			break;
		}
		this.isInit = true;
	}

	// Token: 0x060072A0 RID: 29344 RVA: 0x0021B620 File Offset: 0x00219820
	private void Cancel()
	{
		base.StopAllCoroutines();
		this.RestoreOrgMaterials();
		this.DisableManaGem();
		this.DisableDescription();
		this.DisableAttack();
		this.DisableHealth();
		this.DisablePortrait();
		this.DisableName();
		this.DisableRarity();
		this.DestroyInstances();
		this.StopAllParticles();
		this.HideAllMeshObjects();
		if (this.m_Actor)
		{
			this.m_Actor.ShowAllText();
		}
		if (this.m_Actor != null)
		{
			iTween.StopByName(this.m_Actor.gameObject, "CardConstructImpactRotation");
		}
	}

	// Token: 0x060072A1 RID: 29345 RVA: 0x0021B6B8 File Offset: 0x002198B8
	private void StopAllParticles()
	{
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			if (particleSystem.isPlaying)
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060072A2 RID: 29346 RVA: 0x0021B6F8 File Offset: 0x002198F8
	private void HideAllMeshObjects()
	{
		MeshRenderer[] componentsInChildren = base.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.GetComponent<Renderer>().enabled = false;
		}
	}

	// Token: 0x060072A3 RID: 29347 RVA: 0x0021B734 File Offset: 0x00219934
	private void CreateInstances()
	{
		Vector3 position;
		position..ctor(0f, -5000f, 0f);
		if (this.m_RarityGemMesh)
		{
			this.m_RarityGemMesh.GetComponent<Renderer>().enabled = false;
		}
		if (this.m_RarityFrameMesh)
		{
			this.m_RarityFrameMesh.GetComponent<Renderer>().enabled = false;
		}
		if (this.m_ManaGemStartPosition)
		{
			this.m_ManaGemInstance = Object.Instantiate<GameObject>(this.m_ManaCostMesh);
			this.m_ManaGemInstance.transform.parent = base.transform.parent;
			this.m_ManaGemInstance.transform.position = position;
		}
		if (this.m_DescriptionStartPosition)
		{
			this.m_DescriptionInstance = Object.Instantiate<GameObject>(this.m_DescriptionMesh);
			this.m_DescriptionInstance.transform.parent = base.transform.parent;
			this.m_DescriptionInstance.transform.position = position;
		}
		if (this.m_AttackStartPosition)
		{
			this.m_AttackInstance = Object.Instantiate<GameObject>(this.m_AttackMesh);
			this.m_AttackInstance.transform.parent = base.transform.parent;
			this.m_AttackInstance.transform.position = position;
		}
		if (this.m_HealthStartPosition)
		{
			this.m_HealthInstance = Object.Instantiate<GameObject>(this.m_HealthMesh);
			this.m_HealthInstance.transform.parent = base.transform.parent;
			this.m_HealthInstance.transform.position = position;
		}
		if (this.m_PortraitStartPosition)
		{
			this.m_PortraitInstance = Object.Instantiate<GameObject>(this.m_PortraitMesh);
			this.m_PortraitInstance.transform.parent = base.transform.parent;
			this.m_PortraitInstance.transform.position = position;
		}
		if (this.m_NameStartPosition)
		{
			this.m_NameInstance = Object.Instantiate<GameObject>(this.m_NameMesh);
			this.m_NameInstance.transform.parent = base.transform.parent;
			this.m_NameInstance.transform.position = position;
		}
		if (this.m_RarityStartPosition)
		{
			this.m_RarityInstance = Object.Instantiate<GameObject>(this.m_RarityGemMesh);
			this.m_RarityInstance.transform.parent = base.transform.parent;
			this.m_RarityInstance.transform.position = position;
		}
	}

	// Token: 0x060072A4 RID: 29348 RVA: 0x0021B9B4 File Offset: 0x00219BB4
	private void DestroyInstances()
	{
		if (this.m_ManaGemInstance)
		{
			Object.Destroy(this.m_ManaGemInstance);
		}
		if (this.m_DescriptionInstance)
		{
			Object.Destroy(this.m_DescriptionInstance);
		}
		if (this.m_AttackInstance)
		{
			Object.Destroy(this.m_AttackInstance);
		}
		if (this.m_HealthInstance)
		{
			Object.Destroy(this.m_HealthInstance);
		}
		if (this.m_PortraitInstance)
		{
			Object.Destroy(this.m_PortraitInstance);
		}
		if (this.m_NameInstance)
		{
			Object.Destroy(this.m_NameInstance);
		}
		if (this.m_RarityInstance)
		{
			Object.Destroy(this.m_RarityInstance);
		}
	}

	// Token: 0x060072A5 RID: 29349 RVA: 0x0021BA80 File Offset: 0x00219C80
	private void AnimateManaGem()
	{
		GameObject manaGemInstance = this.m_ManaGemInstance;
		manaGemInstance.transform.parent = null;
		manaGemInstance.transform.localScale = this.m_ManaCostMesh.transform.lossyScale;
		manaGemInstance.transform.position = this.m_ManaGemStartPosition.transform.position;
		manaGemInstance.transform.parent = base.transform.parent;
		manaGemInstance.GetComponent<Renderer>().material = this.m_OrgMat_ManaCost;
		float startDelay = Random.Range(this.m_ManaGemStartDelay - this.m_ManaGemStartDelay * this.m_RandomDelayVariance, this.m_ManaGemStartDelay + this.m_ManaGemStartDelay * this.m_RandomDelayVariance);
		ConstructCard.AnimationData animationData = new ConstructCard.AnimationData
		{
			Name = "ManaGem",
			AnimateTransform = manaGemInstance.transform,
			StartTransform = this.m_ManaGemStartPosition.transform,
			TargetTransform = this.m_ManaGemTargetPosition.transform,
			HitBlastParticle = this.m_ManaGemHitBlastParticle,
			AnimationTime = this.m_ManaGemAnimTime,
			StartDelay = startDelay,
			GlowObject = this.m_ManaGemGlow,
			ImpactRotation = this.m_ManaGemImpactRotation,
			OnComplete = "ManaGemOnComplete"
		};
		base.StartCoroutine("AnimateObject", animationData);
	}

	// Token: 0x060072A6 RID: 29350 RVA: 0x0021BBBC File Offset: 0x00219DBC
	private IEnumerator ManaGemOnComplete()
	{
		this.DisableManaGem();
		yield break;
	}

	// Token: 0x060072A7 RID: 29351 RVA: 0x0021BBD8 File Offset: 0x00219DD8
	private void DisableManaGem()
	{
		if (this.m_ManaGemGlow)
		{
			ParticleSystem[] componentsInChildren = this.m_ManaGemGlow.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060072A8 RID: 29352 RVA: 0x0021BC24 File Offset: 0x00219E24
	private void AnimateDescription()
	{
		GameObject descriptionInstance = this.m_DescriptionInstance;
		descriptionInstance.transform.parent = null;
		descriptionInstance.transform.localScale = this.m_DescriptionMesh.transform.lossyScale;
		descriptionInstance.transform.position = this.m_DescriptionStartPosition.transform.position;
		descriptionInstance.transform.parent = base.transform.parent;
		descriptionInstance.GetComponent<Renderer>().material = this.m_OrgMat_Description;
		float startDelay = Random.Range(this.m_DescriptionStartDelay - this.m_DescriptionStartDelay * this.m_RandomDelayVariance, this.m_DescriptionStartDelay + this.m_DescriptionStartDelay * this.m_RandomDelayVariance);
		ConstructCard.AnimationData animationData = new ConstructCard.AnimationData
		{
			Name = "Description",
			AnimateTransform = descriptionInstance.transform,
			StartTransform = this.m_DescriptionStartPosition.transform,
			TargetTransform = this.m_DescriptionTargetPosition.transform,
			HitBlastParticle = this.m_DescriptionHitBlastParticle,
			AnimationTime = this.m_DescriptionAnimTime,
			StartDelay = startDelay,
			GlowObject = this.m_DescriptionGlow,
			ImpactRotation = this.m_DescriptionImpactRotation,
			OnComplete = "DescriptionOnComplete"
		};
		base.StartCoroutine("AnimateObject", animationData);
	}

	// Token: 0x060072A9 RID: 29353 RVA: 0x0021BD60 File Offset: 0x00219F60
	private IEnumerator DescriptionOnComplete()
	{
		this.DisableDescription();
		yield break;
	}

	// Token: 0x060072AA RID: 29354 RVA: 0x0021BD7C File Offset: 0x00219F7C
	private void DisableDescription()
	{
		if (this.m_DescriptionGlow)
		{
			ParticleSystem[] componentsInChildren = this.m_DescriptionGlow.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060072AB RID: 29355 RVA: 0x0021BDC8 File Offset: 0x00219FC8
	private void AnimateAttack()
	{
		GameObject attackInstance = this.m_AttackInstance;
		attackInstance.transform.parent = null;
		attackInstance.transform.localScale = this.m_AttackMesh.transform.lossyScale;
		attackInstance.transform.position = this.m_AttackStartPosition.transform.position;
		attackInstance.transform.parent = base.transform.parent;
		attackInstance.GetComponent<Renderer>().material = this.m_OrgMat_Attack;
		float startDelay = Random.Range(this.m_AttackStartDelay - this.m_AttackStartDelay * this.m_RandomDelayVariance, this.m_AttackStartDelay + this.m_AttackStartDelay * this.m_RandomDelayVariance);
		ConstructCard.AnimationData animationData = new ConstructCard.AnimationData
		{
			Name = "Attack",
			AnimateTransform = attackInstance.transform,
			StartTransform = this.m_AttackStartPosition.transform,
			TargetTransform = this.m_AttackTargetPosition.transform,
			HitBlastParticle = this.m_AttackHitBlastParticle,
			AnimationTime = this.m_AttackAnimTime,
			StartDelay = startDelay,
			GlowObject = this.m_AttackGlow,
			ImpactRotation = this.m_AttackImpactRotation,
			OnComplete = "AttackOnComplete"
		};
		base.StartCoroutine("AnimateObject", animationData);
	}

	// Token: 0x060072AC RID: 29356 RVA: 0x0021BF04 File Offset: 0x0021A104
	private IEnumerator AttackOnComplete()
	{
		this.DisableAttack();
		yield break;
	}

	// Token: 0x060072AD RID: 29357 RVA: 0x0021BF20 File Offset: 0x0021A120
	private void DisableAttack()
	{
		if (this.m_AttackGlow)
		{
			ParticleSystem[] componentsInChildren = this.m_AttackGlow.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060072AE RID: 29358 RVA: 0x0021BF6C File Offset: 0x0021A16C
	private void AnimateHealth()
	{
		GameObject healthInstance = this.m_HealthInstance;
		healthInstance.transform.parent = null;
		healthInstance.transform.localScale = this.m_HealthMesh.transform.lossyScale;
		healthInstance.transform.position = this.m_HealthStartPosition.transform.position;
		healthInstance.transform.parent = base.transform.parent;
		healthInstance.GetComponent<Renderer>().material = this.m_OrgMat_Health;
		float startDelay = Random.Range(this.m_HealthStartDelay - this.m_HealthStartDelay * this.m_RandomDelayVariance, this.m_HealthStartDelay + this.m_HealthStartDelay * this.m_RandomDelayVariance);
		ConstructCard.AnimationData animationData = new ConstructCard.AnimationData
		{
			Name = "Health",
			AnimateTransform = healthInstance.transform,
			StartTransform = this.m_HealthStartPosition.transform,
			TargetTransform = this.m_HealthTargetPosition.transform,
			HitBlastParticle = this.m_HealthHitBlastParticle,
			AnimationTime = this.m_HealthAnimTime,
			StartDelay = startDelay,
			GlowObject = this.m_HealthGlow,
			ImpactRotation = this.m_HealthImpactRotation,
			OnComplete = "HealthOnComplete"
		};
		base.StartCoroutine("AnimateObject", animationData);
	}

	// Token: 0x060072AF RID: 29359 RVA: 0x0021C0A8 File Offset: 0x0021A2A8
	private IEnumerator HealthOnComplete()
	{
		this.DisableHealth();
		yield break;
	}

	// Token: 0x060072B0 RID: 29360 RVA: 0x0021C0C4 File Offset: 0x0021A2C4
	private void DisableHealth()
	{
		if (this.m_HealthGlow)
		{
			ParticleSystem[] componentsInChildren = this.m_HealthGlow.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060072B1 RID: 29361 RVA: 0x0021C110 File Offset: 0x0021A310
	private void AnimatePortrait()
	{
		GameObject portraitInstance = this.m_PortraitInstance;
		portraitInstance.transform.parent = null;
		portraitInstance.transform.localScale = this.m_PortraitMesh.transform.lossyScale;
		portraitInstance.transform.position = this.m_PortraitStartPosition.transform.position;
		portraitInstance.transform.parent = base.transform.parent;
		float startDelay = Random.Range(this.m_PortraitStartDelay - this.m_PortraitStartDelay * this.m_RandomDelayVariance, this.m_PortraitStartDelay + this.m_PortraitStartDelay * this.m_RandomDelayVariance);
		ConstructCard.AnimationData animationData = new ConstructCard.AnimationData
		{
			Name = "Portrait",
			AnimateTransform = portraitInstance.transform,
			StartTransform = this.m_PortraitStartPosition.transform,
			TargetTransform = this.m_PortraitTargetPosition.transform,
			HitBlastParticle = this.m_PortraitHitBlastParticle,
			AnimationTime = this.m_PortraitAnimTime,
			StartDelay = startDelay,
			GlowObject = this.m_PortraitGlow,
			GlowObjectStandard = this.m_PortraitGlowStandard,
			GlowObjectUnique = this.m_PortraitGlowUnique,
			ImpactRotation = this.m_PortraitImpactRotation,
			OnComplete = "PortraitOnComplete"
		};
		base.StartCoroutine("AnimateObject", animationData);
	}

	// Token: 0x060072B2 RID: 29362 RVA: 0x0021C254 File Offset: 0x0021A454
	private IEnumerator PortraitOnComplete()
	{
		this.DisablePortrait();
		yield break;
	}

	// Token: 0x060072B3 RID: 29363 RVA: 0x0021C270 File Offset: 0x0021A470
	private void DisablePortrait()
	{
		if (this.m_PortraitGlow)
		{
			ParticleSystem[] componentsInChildren = this.m_PortraitGlow.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060072B4 RID: 29364 RVA: 0x0021C2BC File Offset: 0x0021A4BC
	private void AnimateName()
	{
		GameObject nameInstance = this.m_NameInstance;
		nameInstance.transform.parent = null;
		nameInstance.transform.localScale = this.m_NameMesh.transform.lossyScale;
		nameInstance.transform.position = this.m_NameStartPosition.transform.position;
		nameInstance.transform.parent = base.transform.parent;
		nameInstance.GetComponent<Renderer>().material = this.m_OrgMat_Name;
		float startDelay = Random.Range(this.m_NameStartDelay - this.m_NameStartDelay * this.m_RandomDelayVariance, this.m_NameStartDelay + this.m_NameStartDelay * this.m_RandomDelayVariance);
		ConstructCard.AnimationData animationData = new ConstructCard.AnimationData
		{
			Name = "Name",
			AnimateTransform = nameInstance.transform,
			StartTransform = this.m_NameStartPosition.transform,
			TargetTransform = this.m_NameTargetPosition.transform,
			HitBlastParticle = this.m_NameHitBlastParticle,
			AnimationTime = this.m_NameAnimTime,
			StartDelay = startDelay,
			GlowObject = this.m_NameGlow,
			ImpactRotation = this.m_NameImpactRotation,
			OnComplete = "NameOnComplete"
		};
		base.StartCoroutine("AnimateObject", animationData);
	}

	// Token: 0x060072B5 RID: 29365 RVA: 0x0021C3F8 File Offset: 0x0021A5F8
	private IEnumerator NameOnComplete()
	{
		this.DisableName();
		if (this.m_Actor.GetCardSet() == TAG_CARD_SET.CORE)
		{
			base.StartCoroutine(this.EndAnimation());
		}
		yield break;
	}

	// Token: 0x060072B6 RID: 29366 RVA: 0x0021C414 File Offset: 0x0021A614
	private void DisableName()
	{
		if (this.m_NameGlow)
		{
			ParticleSystem[] componentsInChildren = this.m_NameGlow.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060072B7 RID: 29367 RVA: 0x0021C460 File Offset: 0x0021A660
	private void AnimateRarity()
	{
		if (this.m_Actor.GetCardSet() == TAG_CARD_SET.CORE)
		{
			return;
		}
		GameObject rarityInstance = this.m_RarityInstance;
		rarityInstance.transform.parent = null;
		rarityInstance.transform.localScale = this.m_RarityGemMesh.transform.lossyScale;
		rarityInstance.transform.position = this.m_RarityStartPosition.transform.position;
		rarityInstance.transform.parent = base.transform.parent;
		this.m_RarityInstance.GetComponent<Renderer>().enabled = true;
		GameObject glowObject = this.m_RarityGlowCommon;
		switch (this.m_Actor.GetRarity())
		{
		case TAG_RARITY.RARE:
			glowObject = this.m_RarityGlowRare;
			break;
		case TAG_RARITY.EPIC:
			glowObject = this.m_RarityGlowEpic;
			break;
		case TAG_RARITY.LEGENDARY:
			glowObject = this.m_RarityGlowLegendary;
			break;
		}
		float startDelay = Random.Range(this.m_RarityStartDelay - this.m_RarityStartDelay * this.m_RandomDelayVariance, this.m_RarityStartDelay + this.m_RarityStartDelay * this.m_RandomDelayVariance);
		ConstructCard.AnimationData animationData = new ConstructCard.AnimationData
		{
			Name = "Rarity",
			AnimateTransform = rarityInstance.transform,
			StartTransform = this.m_RarityStartPosition.transform,
			TargetTransform = this.m_RarityTargetPosition.transform,
			HitBlastParticle = this.m_RarityHitBlastParticle,
			AnimationTime = this.m_RarityAnimTime,
			StartDelay = startDelay,
			GlowObject = glowObject,
			ImpactRotation = this.m_RarityImpactRotation,
			OnComplete = "RarityOnComplete"
		};
		base.StartCoroutine("AnimateObject", animationData);
	}

	// Token: 0x060072B8 RID: 29368 RVA: 0x0021C60C File Offset: 0x0021A80C
	private IEnumerator RarityOnComplete()
	{
		this.DisableRarity();
		if (this.m_Actor.GetCardSet() != TAG_CARD_SET.CORE)
		{
			if (this.m_RarityGemMesh)
			{
				this.m_RarityGemMesh.GetComponent<Renderer>().enabled = true;
			}
			if (this.m_RarityFrameMesh)
			{
				this.m_RarityFrameMesh.GetComponent<Renderer>().enabled = true;
			}
		}
		base.StartCoroutine(this.EndAnimation());
		yield break;
	}

	// Token: 0x060072B9 RID: 29369 RVA: 0x0021C628 File Offset: 0x0021A828
	private void DisableRarity()
	{
		if (this.m_RarityGlowCommon)
		{
			ParticleSystem[] componentsInChildren = this.m_RarityGlowCommon.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x060072BA RID: 29370 RVA: 0x0021C674 File Offset: 0x0021A874
	private IEnumerator EndAnimation()
	{
		ParticleSystem burst = this.m_RarityBurstCommon;
		TAG_RARITY rarity = this.m_Actor.GetRarity();
		switch (rarity)
		{
		case TAG_RARITY.RARE:
			burst = this.m_RarityBurstRare;
			break;
		case TAG_RARITY.EPIC:
			burst = this.m_RarityBurstEpic;
			break;
		case TAG_RARITY.LEGENDARY:
			burst = this.m_RarityBurstLegendary;
			break;
		}
		if (burst)
		{
			Renderer[] burstParticleRenderers = burst.GetComponentsInChildren<Renderer>();
			foreach (Renderer burstRenderer in burstParticleRenderers)
			{
				burstRenderer.enabled = true;
			}
			burst.Play(true);
		}
		string fuseAnimation = "CardFuse_Common";
		switch (rarity)
		{
		case TAG_RARITY.RARE:
			fuseAnimation = "CardFuse_Rare";
			break;
		case TAG_RARITY.EPIC:
			fuseAnimation = "CardFuse_Epic";
			break;
		case TAG_RARITY.LEGENDARY:
			fuseAnimation = "CardFuse_Legendary";
			break;
		}
		if (this.m_FuseGlow)
		{
			this.m_FuseGlow.GetComponent<Renderer>().enabled = true;
			this.m_FuseGlow.GetComponent<Animation>().Play(fuseAnimation, 4);
		}
		yield return new WaitForSeconds(0.25f);
		this.DestroyInstances();
		this.m_Actor.ShowAllText();
		this.RestoreOrgMaterials();
		yield break;
	}

	// Token: 0x060072BB RID: 29371 RVA: 0x0021C690 File Offset: 0x0021A890
	private IEnumerator AnimateObject(ConstructCard.AnimationData animData)
	{
		yield return new WaitForSeconds(animData.StartDelay);
		float animPos = 0f;
		float rate = 1f / (animData.AnimationTime * this.m_AnimationScale);
		Quaternion currCardRot = this.m_Actor.transform.rotation;
		this.m_Actor.transform.rotation = Quaternion.identity;
		Vector3 startPosition = animData.StartTransform.position;
		Quaternion startRotation = animData.StartTransform.rotation;
		this.m_Actor.transform.rotation = currCardRot;
		if (animData.GlowObject)
		{
			GameObject glowObj = animData.GlowObject;
			glowObj.transform.parent = animData.AnimateTransform;
			glowObj.transform.localPosition = Vector3.zero;
			ParticleSystem[] particles = glowObj.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem ps in particles)
			{
				ps.Play();
			}
			if (animData.GlowObjectStandard && animData.GlowObjectUnique)
			{
				if (this.m_Actor.IsElite())
				{
					animData.GlowObjectUnique.GetComponent<Renderer>().enabled = true;
				}
				else
				{
					animData.GlowObjectStandard.GetComponent<Renderer>().enabled = true;
				}
			}
			else
			{
				Renderer[] meshRenders = glowObj.GetComponentsInChildren<Renderer>();
				foreach (Renderer mesh in meshRenders)
				{
					mesh.enabled = true;
				}
			}
		}
		while (animPos < 1f)
		{
			Vector3 currentTargetPosition = animData.TargetTransform.position;
			Quaternion currentTargetRotation = animData.TargetTransform.rotation;
			animPos += rate * Time.deltaTime;
			Vector3 position = Vector3.Lerp(startPosition, currentTargetPosition, animPos);
			Quaternion rotation = Quaternion.Lerp(startRotation, currentTargetRotation, animPos);
			animData.AnimateTransform.position = position;
			animData.AnimateTransform.rotation = rotation;
			yield return null;
		}
		if (animData.HitBlastParticle)
		{
			animData.HitBlastParticle.transform.position = animData.TargetTransform.position;
			animData.HitBlastParticle.GetComponent<Renderer>().enabled = true;
			animData.HitBlastParticle.Play();
		}
		animData.AnimateTransform.parent = animData.TargetTransform;
		animData.AnimateTransform.position = animData.TargetTransform.position;
		animData.AnimateTransform.rotation = animData.TargetTransform.rotation;
		if (animData.GlowObject)
		{
			ParticleSystem[] particles2 = animData.GlowObject.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem ps2 in particles2)
			{
				ps2.Stop();
			}
		}
		if (this.m_Actor.gameObject == null)
		{
			yield break;
		}
		this.m_Actor.gameObject.transform.localRotation = Quaternion.Euler(animData.ImpactRotation);
		Hashtable impactAnimArgs = iTween.Hash(new object[]
		{
			"rotation",
			Vector3.zero,
			"time",
			this.m_ImpactRotationTime,
			"easetype",
			iTween.EaseType.easeOutQuad,
			"space",
			1,
			"name",
			"CardConstructImpactRotation" + animData.Name
		});
		iTween.StopByName(this.m_Actor.gameObject, "CardConstructImpactRotation" + animData.Name);
		iTween.RotateTo(this.m_Actor.gameObject, impactAnimArgs);
		CameraShakeMgr.Shake(Camera.main, this.IMPACT_CAMERA_SHAKE_AMOUNT, this.IMPACT_CAMERA_SHAKE_TIME);
		if (animData.OnComplete != string.Empty)
		{
			base.StartCoroutine(animData.OnComplete);
		}
		yield break;
	}

	// Token: 0x060072BC RID: 29372 RVA: 0x0021C6BC File Offset: 0x0021A8BC
	private void StoreOrgMaterials()
	{
		if (this.m_CardMesh)
		{
			this.m_OrgMat_CardFront = this.m_CardMesh.GetComponent<Renderer>().materials[this.m_CardFrontIdx];
		}
		if (this.m_PortraitMesh)
		{
			this.m_OrgMat_PortraitFrame = this.m_PortraitMesh.GetComponent<Renderer>().sharedMaterials[this.m_PortraitFrameIdx];
		}
		if (this.m_NameMesh)
		{
			this.m_OrgMat_Name = this.m_NameMesh.GetComponent<Renderer>().material;
		}
		if (this.m_ManaCostMesh)
		{
			this.m_OrgMat_ManaCost = this.m_ManaCostMesh.GetComponent<Renderer>().material;
		}
		if (this.m_AttackMesh)
		{
			this.m_OrgMat_Attack = this.m_AttackMesh.GetComponent<Renderer>().material;
		}
		if (this.m_HealthMesh)
		{
			this.m_OrgMat_Health = this.m_HealthMesh.GetComponent<Renderer>().material;
		}
		if (this.m_RacePlateMesh)
		{
			this.m_OrgMat_RacePlate = this.m_RacePlateMesh.GetComponent<Renderer>().material;
		}
		if (this.m_RarityFrameMesh)
		{
			this.m_OrgMat_RarityFrame = this.m_RarityFrameMesh.GetComponent<Renderer>().material;
		}
		if (this.m_DescriptionMesh)
		{
			Material[] materials = this.m_DescriptionMesh.GetComponent<Renderer>().materials;
			if (materials.Length > 1)
			{
				this.m_OrgMat_Description = materials[0];
				this.m_OrgMat_Description2 = materials[1];
			}
			else
			{
				this.m_OrgMat_Description = this.m_DescriptionMesh.GetComponent<Renderer>().material;
			}
		}
		if (this.m_DescriptionTrimMesh)
		{
			this.m_OrgMat_DescriptionTrim = this.m_DescriptionTrimMesh.GetComponent<Renderer>().material;
		}
		if (this.m_EliteMesh)
		{
			this.m_OrgMat_Elite = this.m_EliteMesh.GetComponent<Renderer>().material;
		}
	}

	// Token: 0x060072BD RID: 29373 RVA: 0x0021C8AC File Offset: 0x0021AAAC
	private void RestoreOrgMaterials()
	{
		this.ApplyMaterialByIdx(this.m_CardMesh, this.m_OrgMat_CardFront, this.m_CardFrontIdx);
		this.ApplySharedMaterialByIdx(this.m_PortraitMesh, this.m_OrgMat_PortraitFrame, this.m_PortraitFrameIdx);
		this.ApplyMaterialByIdx(this.m_DescriptionMesh, this.m_OrgMat_Description, 0);
		this.ApplyMaterialByIdx(this.m_DescriptionMesh, this.m_OrgMat_Description2, 1);
		this.ApplyMaterial(this.m_NameMesh, this.m_OrgMat_Name);
		this.ApplyMaterial(this.m_ManaCostMesh, this.m_OrgMat_ManaCost);
		this.ApplyMaterial(this.m_AttackMesh, this.m_OrgMat_Attack);
		this.ApplyMaterial(this.m_HealthMesh, this.m_OrgMat_Health);
		this.ApplyMaterial(this.m_RacePlateMesh, this.m_OrgMat_RacePlate);
		this.ApplyMaterial(this.m_RarityFrameMesh, this.m_OrgMat_RarityFrame);
		this.ApplyMaterial(this.m_DescriptionTrimMesh, this.m_OrgMat_DescriptionTrim);
		this.ApplyMaterial(this.m_EliteMesh, this.m_OrgMat_Elite);
	}

	// Token: 0x060072BE RID: 29374 RVA: 0x0021C9A0 File Offset: 0x0021ABA0
	private void ApplyGhostMaterials()
	{
		this.ApplyMaterialByIdx(this.m_CardMesh, this.m_GhostMaterial, this.m_CardFrontIdx);
		this.ApplySharedMaterialByIdx(this.m_PortraitMesh, this.m_GhostMaterial, this.m_PortraitFrameIdx);
		this.ApplyMaterialByIdx(this.m_DescriptionMesh, this.m_GhostMaterial, 0);
		this.ApplyMaterialByIdx(this.m_DescriptionMesh, this.m_GhostMaterial, 1);
		this.ApplyMaterial(this.m_NameMesh, this.m_GhostMaterial);
		this.ApplyMaterial(this.m_ManaCostMesh, this.m_GhostMaterial);
		this.ApplyMaterial(this.m_AttackMesh, this.m_GhostMaterial);
		this.ApplyMaterial(this.m_HealthMesh, this.m_GhostMaterial);
		this.ApplyMaterial(this.m_RacePlateMesh, this.m_GhostMaterial);
		this.ApplyMaterial(this.m_RarityFrameMesh, this.m_GhostMaterial);
		if (this.m_GhostMaterialTransparent)
		{
			this.ApplyMaterial(this.m_DescriptionTrimMesh, this.m_GhostMaterialTransparent);
		}
		this.ApplyMaterial(this.m_EliteMesh, this.m_GhostMaterial);
	}

	// Token: 0x060072BF RID: 29375 RVA: 0x0021CAA4 File Offset: 0x0021ACA4
	private void ApplyMaterial(GameObject go, Material mat)
	{
		if (go == null)
		{
			return;
		}
		Texture mainTexture = go.GetComponent<Renderer>().material.mainTexture;
		go.GetComponent<Renderer>().material = mat;
		go.GetComponent<Renderer>().material.mainTexture = mainTexture;
	}

	// Token: 0x060072C0 RID: 29376 RVA: 0x0021CAEC File Offset: 0x0021ACEC
	private void ApplyMaterialByIdx(GameObject go, Material mat, int idx)
	{
		if (go == null || mat == null || idx < 0)
		{
			return;
		}
		Material[] materials = go.GetComponent<Renderer>().materials;
		if (idx >= materials.Length)
		{
			return;
		}
		Texture mainTexture = go.GetComponent<Renderer>().materials[idx].mainTexture;
		materials[idx] = mat;
		go.GetComponent<Renderer>().materials = materials;
		go.GetComponent<Renderer>().materials[idx].mainTexture = mainTexture;
	}

	// Token: 0x060072C1 RID: 29377 RVA: 0x0021CB68 File Offset: 0x0021AD68
	private void ApplySharedMaterialByIdx(GameObject go, Material mat, int idx)
	{
		if (go == null || mat == null || idx < 0)
		{
			return;
		}
		Material[] sharedMaterials = go.GetComponent<Renderer>().sharedMaterials;
		if (idx >= sharedMaterials.Length)
		{
			return;
		}
		Texture mainTexture = go.GetComponent<Renderer>().sharedMaterials[idx].mainTexture;
		sharedMaterials[idx] = mat;
		go.GetComponent<Renderer>().sharedMaterials = sharedMaterials;
		go.GetComponent<Renderer>().sharedMaterials[idx].mainTexture = mainTexture;
	}

	// Token: 0x04005CA9 RID: 23721
	private readonly Vector3 IMPACT_CAMERA_SHAKE_AMOUNT = new Vector3(0.35f, 0.35f, 0.35f);

	// Token: 0x04005CAA RID: 23722
	private readonly float IMPACT_CAMERA_SHAKE_TIME = 0.25f;

	// Token: 0x04005CAB RID: 23723
	public Material m_GhostMaterial;

	// Token: 0x04005CAC RID: 23724
	public Material m_GhostMaterialTransparent;

	// Token: 0x04005CAD RID: 23725
	public float m_ImpactRotationTime = 0.5f;

	// Token: 0x04005CAE RID: 23726
	public float m_RandomDelayVariance = 0.2f;

	// Token: 0x04005CAF RID: 23727
	public float m_AnimationRarityScaleCommon = 1f;

	// Token: 0x04005CB0 RID: 23728
	public float m_AnimationRarityScaleRare = 0.9f;

	// Token: 0x04005CB1 RID: 23729
	public float m_AnimationRarityScaleEpic = 0.8f;

	// Token: 0x04005CB2 RID: 23730
	public float m_AnimationRarityScaleLegendary = 0.7f;

	// Token: 0x04005CB3 RID: 23731
	public GameObject m_GhostGlow;

	// Token: 0x04005CB4 RID: 23732
	public Texture m_GhostTextureUnique;

	// Token: 0x04005CB5 RID: 23733
	public GameObject m_FuseGlow;

	// Token: 0x04005CB6 RID: 23734
	public ParticleSystem m_RarityBurstCommon;

	// Token: 0x04005CB7 RID: 23735
	public ParticleSystem m_RarityBurstRare;

	// Token: 0x04005CB8 RID: 23736
	public ParticleSystem m_RarityBurstEpic;

	// Token: 0x04005CB9 RID: 23737
	public ParticleSystem m_RarityBurstLegendary;

	// Token: 0x04005CBA RID: 23738
	public Transform m_ManaGemStartPosition;

	// Token: 0x04005CBB RID: 23739
	public Transform m_ManaGemTargetPosition;

	// Token: 0x04005CBC RID: 23740
	public float m_ManaGemStartDelay;

	// Token: 0x04005CBD RID: 23741
	public float m_ManaGemAnimTime = 1f;

	// Token: 0x04005CBE RID: 23742
	public GameObject m_ManaGemGlow;

	// Token: 0x04005CBF RID: 23743
	public ParticleSystem m_ManaGemHitBlastParticle;

	// Token: 0x04005CC0 RID: 23744
	public Vector3 m_ManaGemImpactRotation = new Vector3(20f, 0f, 20f);

	// Token: 0x04005CC1 RID: 23745
	public Transform m_DescriptionStartPosition;

	// Token: 0x04005CC2 RID: 23746
	public Transform m_DescriptionTargetPosition;

	// Token: 0x04005CC3 RID: 23747
	public float m_DescriptionStartDelay;

	// Token: 0x04005CC4 RID: 23748
	public float m_DescriptionAnimTime = 1f;

	// Token: 0x04005CC5 RID: 23749
	public GameObject m_DescriptionGlow;

	// Token: 0x04005CC6 RID: 23750
	public ParticleSystem m_DescriptionHitBlastParticle;

	// Token: 0x04005CC7 RID: 23751
	public Vector3 m_DescriptionImpactRotation = new Vector3(-15f, 0f, 0f);

	// Token: 0x04005CC8 RID: 23752
	public Transform m_AttackStartPosition;

	// Token: 0x04005CC9 RID: 23753
	public Transform m_AttackTargetPosition;

	// Token: 0x04005CCA RID: 23754
	public float m_AttackStartDelay;

	// Token: 0x04005CCB RID: 23755
	public float m_AttackAnimTime = 1f;

	// Token: 0x04005CCC RID: 23756
	public GameObject m_AttackGlow;

	// Token: 0x04005CCD RID: 23757
	public ParticleSystem m_AttackHitBlastParticle;

	// Token: 0x04005CCE RID: 23758
	public Vector3 m_AttackImpactRotation = new Vector3(-15f, 0f, 0f);

	// Token: 0x04005CCF RID: 23759
	public Transform m_HealthStartPosition;

	// Token: 0x04005CD0 RID: 23760
	public Transform m_HealthTargetPosition;

	// Token: 0x04005CD1 RID: 23761
	public float m_HealthStartDelay;

	// Token: 0x04005CD2 RID: 23762
	public float m_HealthAnimTime = 1f;

	// Token: 0x04005CD3 RID: 23763
	public GameObject m_HealthGlow;

	// Token: 0x04005CD4 RID: 23764
	public ParticleSystem m_HealthHitBlastParticle;

	// Token: 0x04005CD5 RID: 23765
	public Vector3 m_HealthImpactRotation = new Vector3(-15f, 0f, 0f);

	// Token: 0x04005CD6 RID: 23766
	public Transform m_PortraitStartPosition;

	// Token: 0x04005CD7 RID: 23767
	public Transform m_PortraitTargetPosition;

	// Token: 0x04005CD8 RID: 23768
	public float m_PortraitStartDelay;

	// Token: 0x04005CD9 RID: 23769
	public float m_PortraitAnimTime = 1f;

	// Token: 0x04005CDA RID: 23770
	public GameObject m_PortraitGlow;

	// Token: 0x04005CDB RID: 23771
	public GameObject m_PortraitGlowStandard;

	// Token: 0x04005CDC RID: 23772
	public GameObject m_PortraitGlowUnique;

	// Token: 0x04005CDD RID: 23773
	public ParticleSystem m_PortraitHitBlastParticle;

	// Token: 0x04005CDE RID: 23774
	public Vector3 m_PortraitImpactRotation = new Vector3(-15f, 0f, 0f);

	// Token: 0x04005CDF RID: 23775
	public Transform m_NameStartPosition;

	// Token: 0x04005CE0 RID: 23776
	public Transform m_NameTargetPosition;

	// Token: 0x04005CE1 RID: 23777
	public float m_NameStartDelay;

	// Token: 0x04005CE2 RID: 23778
	public float m_NameAnimTime = 1f;

	// Token: 0x04005CE3 RID: 23779
	public GameObject m_NameGlow;

	// Token: 0x04005CE4 RID: 23780
	public ParticleSystem m_NameHitBlastParticle;

	// Token: 0x04005CE5 RID: 23781
	public Vector3 m_NameImpactRotation = new Vector3(-15f, 0f, 0f);

	// Token: 0x04005CE6 RID: 23782
	public Transform m_RarityStartPosition;

	// Token: 0x04005CE7 RID: 23783
	public Transform m_RarityTargetPosition;

	// Token: 0x04005CE8 RID: 23784
	public float m_RarityStartDelay;

	// Token: 0x04005CE9 RID: 23785
	public float m_RarityAnimTime = 1f;

	// Token: 0x04005CEA RID: 23786
	public GameObject m_RarityGlowCommon;

	// Token: 0x04005CEB RID: 23787
	public GameObject m_RarityGlowRare;

	// Token: 0x04005CEC RID: 23788
	public GameObject m_RarityGlowEpic;

	// Token: 0x04005CED RID: 23789
	public GameObject m_RarityGlowLegendary;

	// Token: 0x04005CEE RID: 23790
	public ParticleSystem m_RarityHitBlastParticle;

	// Token: 0x04005CEF RID: 23791
	public Vector3 m_RarityImpactRotation = new Vector3(-15f, 0f, 0f);

	// Token: 0x04005CF0 RID: 23792
	private Actor m_Actor;

	// Token: 0x04005CF1 RID: 23793
	private Spell m_GhostSpell;

	// Token: 0x04005CF2 RID: 23794
	private float m_AnimationScale = 1f;

	// Token: 0x04005CF3 RID: 23795
	private bool isInit;

	// Token: 0x04005CF4 RID: 23796
	private GameObject m_ManaGemInstance;

	// Token: 0x04005CF5 RID: 23797
	private GameObject m_DescriptionInstance;

	// Token: 0x04005CF6 RID: 23798
	private GameObject m_AttackInstance;

	// Token: 0x04005CF7 RID: 23799
	private GameObject m_HealthInstance;

	// Token: 0x04005CF8 RID: 23800
	private GameObject m_PortraitInstance;

	// Token: 0x04005CF9 RID: 23801
	private GameObject m_NameInstance;

	// Token: 0x04005CFA RID: 23802
	private GameObject m_RarityInstance;

	// Token: 0x04005CFB RID: 23803
	private GameObject m_CardMesh;

	// Token: 0x04005CFC RID: 23804
	private int m_CardFrontIdx;

	// Token: 0x04005CFD RID: 23805
	private GameObject m_PortraitMesh;

	// Token: 0x04005CFE RID: 23806
	private int m_PortraitFrameIdx;

	// Token: 0x04005CFF RID: 23807
	private GameObject m_NameMesh;

	// Token: 0x04005D00 RID: 23808
	private GameObject m_DescriptionMesh;

	// Token: 0x04005D01 RID: 23809
	private GameObject m_DescriptionTrimMesh;

	// Token: 0x04005D02 RID: 23810
	private GameObject m_RarityGemMesh;

	// Token: 0x04005D03 RID: 23811
	private GameObject m_RarityFrameMesh;

	// Token: 0x04005D04 RID: 23812
	private GameObject m_ManaCostMesh;

	// Token: 0x04005D05 RID: 23813
	private GameObject m_AttackMesh;

	// Token: 0x04005D06 RID: 23814
	private GameObject m_HealthMesh;

	// Token: 0x04005D07 RID: 23815
	private GameObject m_RacePlateMesh;

	// Token: 0x04005D08 RID: 23816
	private GameObject m_EliteMesh;

	// Token: 0x04005D09 RID: 23817
	private GameObject m_ManaGemClone;

	// Token: 0x04005D0A RID: 23818
	private Material m_OrgMat_CardFront;

	// Token: 0x04005D0B RID: 23819
	private Material m_OrgMat_PortraitFrame;

	// Token: 0x04005D0C RID: 23820
	private Material m_OrgMat_Name;

	// Token: 0x04005D0D RID: 23821
	private Material m_OrgMat_Description;

	// Token: 0x04005D0E RID: 23822
	private Material m_OrgMat_Description2;

	// Token: 0x04005D0F RID: 23823
	private Material m_OrgMat_DescriptionTrim;

	// Token: 0x04005D10 RID: 23824
	private Material m_OrgMat_RarityFrame;

	// Token: 0x04005D11 RID: 23825
	private Material m_OrgMat_ManaCost;

	// Token: 0x04005D12 RID: 23826
	private Material m_OrgMat_Attack;

	// Token: 0x04005D13 RID: 23827
	private Material m_OrgMat_Health;

	// Token: 0x04005D14 RID: 23828
	private Material m_OrgMat_RacePlate;

	// Token: 0x04005D15 RID: 23829
	private Material m_OrgMat_Elite;

	// Token: 0x02000EFC RID: 3836
	private class AnimationData
	{
		// Token: 0x04005D16 RID: 23830
		public string Name;

		// Token: 0x04005D17 RID: 23831
		public Transform AnimateTransform;

		// Token: 0x04005D18 RID: 23832
		public Transform StartTransform;

		// Token: 0x04005D19 RID: 23833
		public Transform TargetTransform;

		// Token: 0x04005D1A RID: 23834
		public float AnimationTime = 1f;

		// Token: 0x04005D1B RID: 23835
		public float StartDelay;

		// Token: 0x04005D1C RID: 23836
		public GameObject GlowObject;

		// Token: 0x04005D1D RID: 23837
		public GameObject GlowObjectStandard;

		// Token: 0x04005D1E RID: 23838
		public GameObject GlowObjectUnique;

		// Token: 0x04005D1F RID: 23839
		public ParticleSystem HitBlastParticle;

		// Token: 0x04005D20 RID: 23840
		public Vector3 ImpactRotation;

		// Token: 0x04005D21 RID: 23841
		public string OnComplete = string.Empty;
	}
}
