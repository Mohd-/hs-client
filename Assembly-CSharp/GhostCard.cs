using System;
using UnityEngine;

// Token: 0x02000333 RID: 819
public class GhostCard : MonoBehaviour
{
	// Token: 0x06002A86 RID: 10886 RVA: 0x000D125C File Offset: 0x000CF45C
	public static GhostCard.Type GetGhostTypeFromSlot(CollectionDeck deck, CollectionDeckSlot slot)
	{
		GhostCard.Type result = GhostCard.Type.NONE;
		if (deck == null || slot == null)
		{
			return result;
		}
		if (!slot.Owned)
		{
			result = GhostCard.Type.MISSING;
		}
		else if (!deck.IsValidSlot(slot))
		{
			result = GhostCard.Type.NOT_VALID;
		}
		return result;
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x000D129C File Offset: 0x000CF49C
	private void Awake()
	{
		if (GhostCard.s_ghostStyles == null && AssetLoader.Get() != null)
		{
			GhostCard.s_ghostStyles = AssetLoader.Get().LoadGameObject("GhostStyleDef", true, false).GetComponent<GhostStyleDef>();
		}
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x000D12E4 File Offset: 0x000CF4E4
	private void OnDisable()
	{
		this.Disable();
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x000D12EC File Offset: 0x000CF4EC
	private void OnDestroy()
	{
		if (this.m_EffectRoot)
		{
			ParticleSystem componentInChildren = this.m_EffectRoot.GetComponentInChildren<ParticleSystem>();
			if (componentInChildren)
			{
				componentInChildren.Stop();
			}
		}
	}

	// Token: 0x06002A8A RID: 10890 RVA: 0x000D1326 File Offset: 0x000CF526
	public void SetBigCard(bool isBigCard)
	{
		this.m_isBigCard = isBigCard;
	}

	// Token: 0x06002A8B RID: 10891 RVA: 0x000D132F File Offset: 0x000CF52F
	public void SetGhostType(GhostCard.Type ghostType)
	{
		this.m_ghostType = ghostType;
	}

	// Token: 0x06002A8C RID: 10892 RVA: 0x000D1338 File Offset: 0x000CF538
	public void SetRenderQueue(int renderQueue)
	{
		this.m_renderQueue = renderQueue;
	}

	// Token: 0x06002A8D RID: 10893 RVA: 0x000D1341 File Offset: 0x000CF541
	public void RenderGhostCard()
	{
		this.RenderGhostCard(false);
	}

	// Token: 0x06002A8E RID: 10894 RVA: 0x000D134A File Offset: 0x000CF54A
	public void RenderGhostCard(bool forceRender)
	{
		this.RenderGhost(forceRender);
	}

	// Token: 0x06002A8F RID: 10895 RVA: 0x000D1353 File Offset: 0x000CF553
	public void Reset()
	{
		this.m_Init = false;
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x000D135C File Offset: 0x000CF55C
	private void RenderGhost()
	{
		this.RenderGhost(false);
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x000D1368 File Offset: 0x000CF568
	private void RenderGhost(bool forceRender)
	{
		this.Init(forceRender);
		this.m_R2T_BaseCard.enabled = true;
		this.m_R2T_BaseCard.m_RenderQueue = this.m_renderQueue;
		if (this.m_R2T_EffectGhost)
		{
			this.m_R2T_EffectGhost.enabled = true;
			this.m_R2T_EffectGhost.m_RenderQueue = this.m_renderQueue;
		}
		this.m_R2T_BaseCard.m_ObjectToRender = this.m_Actor.GetRootObject();
		this.m_Actor.GetRootObject().transform.localPosition = this.m_CardOffset;
		this.m_Actor.ShowAllText();
		this.ApplyGhostMaterials();
		this.m_R2T_BaseCard.Render();
		Material renderMaterial = this.m_R2T_BaseCard.GetRenderMaterial();
		if (this.m_GlowPlane)
		{
			if (!this.m_Actor.IsElite())
			{
				this.m_GlowPlane.GetComponent<Renderer>().enabled = true;
			}
			else
			{
				this.m_GlowPlane.GetComponent<Renderer>().enabled = false;
			}
		}
		if (this.m_GlowPlaneElite)
		{
			if (this.m_Actor.IsElite())
			{
				this.m_GlowPlaneElite.GetComponent<Renderer>().enabled = true;
			}
			else
			{
				this.m_GlowPlaneElite.GetComponent<Renderer>().enabled = false;
			}
		}
		if (this.m_EffectRoot)
		{
			this.m_EffectRoot.transform.parent = null;
			this.m_EffectRoot.transform.position = new Vector3(-500f, -500f, -500f);
			this.m_EffectRoot.transform.localScale = Vector3.one;
			if (this.m_R2T_EffectGhost)
			{
				this.m_R2T_EffectGhost.enabled = true;
				RenderTexture renderTexture = this.m_R2T_EffectGhost.RenderNow();
				if (renderTexture != null)
				{
					renderMaterial.SetTexture("_FxTex", renderTexture);
					if (this.m_GlowPlane)
					{
						this.m_GlowPlane.GetComponent<Renderer>().material.SetTexture("_FxTex", renderTexture);
					}
					if (this.m_GlowPlaneElite)
					{
						this.m_GlowPlaneElite.GetComponent<Renderer>().material.SetTexture("_FxTex", renderTexture);
					}
				}
			}
			ParticleSystem componentInChildren = this.m_EffectRoot.GetComponentInChildren<ParticleSystem>();
			if (componentInChildren)
			{
				componentInChildren.Play();
			}
		}
		if (this.m_GlowPlane)
		{
			this.m_GlowPlane.GetComponent<Renderer>().material.renderQueue = 3071;
		}
		if (this.m_GlowPlaneElite)
		{
			this.m_GlowPlaneElite.GetComponent<Renderer>().material.renderQueue = 3071;
		}
	}

	// Token: 0x06002A92 RID: 10898 RVA: 0x000D160F File Offset: 0x000CF80F
	public void DisableGhost()
	{
		this.Disable();
		base.enabled = false;
	}

	// Token: 0x06002A93 RID: 10899 RVA: 0x000D1620 File Offset: 0x000CF820
	private void Init(bool forceRender)
	{
		if (this.m_Init && !forceRender)
		{
			return;
		}
		if (this.m_Actor == null)
		{
			this.m_Actor = SceneUtils.FindComponentInThisOrParents<Actor>(base.gameObject);
			if (this.m_Actor == null)
			{
				Debug.LogError(string.Format("{0} Ghost card effect failed to find Actor!", base.transform.root.name));
				base.enabled = false;
				return;
			}
		}
		this.m_CardMesh = this.m_Actor.m_cardMesh;
		this.m_CardFrontIdx = this.m_Actor.m_cardFrontMatIdx;
		this.m_PremiumRibbonIdx = this.m_Actor.m_premiumRibbon;
		this.m_PortraitMesh = this.m_Actor.m_portraitMesh;
		this.m_PortraitFrameIdx = this.m_Actor.m_portraitFrameMatIdx;
		this.m_NameMesh = this.m_Actor.m_nameBannerMesh;
		this.m_DescriptionMesh = this.m_Actor.m_descriptionMesh;
		this.m_DescriptionTrimMesh = this.m_Actor.m_descriptionTrimMesh;
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
		this.m_R2T_BaseCard = base.GetComponent<RenderToTexture>();
		this.m_R2T_BaseCard.m_ObjectToRender = this.m_Actor.GetRootObject();
		if (this.m_R2T_BaseCard.m_Material && this.m_R2T_BaseCard.m_Material.HasProperty("_Seed"))
		{
			this.m_R2T_BaseCard.m_Material.SetFloat("_Seed", Random.Range(0f, 1f));
		}
		this.m_Init = true;
	}

	// Token: 0x06002A94 RID: 10900 RVA: 0x000D1934 File Offset: 0x000CFB34
	private void StoreOrgMaterials()
	{
		if (this.m_CardMesh)
		{
			if (this.m_CardFrontIdx > -1)
			{
				this.m_OrgMat_CardFront = this.m_CardMesh.GetComponent<Renderer>().materials[this.m_CardFrontIdx];
			}
			if (this.m_PremiumRibbonIdx > -1)
			{
				this.m_OrgMat_PremiumRibbon = this.m_CardMesh.GetComponent<Renderer>().materials[this.m_PremiumRibbonIdx];
			}
		}
		if (this.m_PortraitMesh && this.m_PortraitFrameIdx > -1)
		{
			this.m_OrgMat_PortraitFrame = this.m_PortraitMesh.GetComponent<Renderer>().materials[this.m_PortraitFrameIdx];
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

	// Token: 0x06002A95 RID: 10901 RVA: 0x000D1B64 File Offset: 0x000CFD64
	private void RestoreOrgMaterials()
	{
		this.ApplyMaterialByIdx(this.m_CardMesh, this.m_OrgMat_CardFront, this.m_CardFrontIdx);
		this.ApplyMaterialByIdx(this.m_CardMesh, this.m_OrgMat_PremiumRibbon, this.m_PremiumRibbonIdx);
		this.ApplyMaterialByIdx(this.m_PortraitMesh, this.m_OrgMat_PortraitFrame, this.m_PortraitFrameIdx);
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

	// Token: 0x06002A96 RID: 10902 RVA: 0x000D1C70 File Offset: 0x000CFE70
	private void ApplyGhostMaterials()
	{
		GhostStyle ghostStyle;
		if (this.m_ghostType == GhostCard.Type.NOT_VALID)
		{
			ghostStyle = GhostCard.s_ghostStyles.m_invalid;
		}
		else
		{
			ghostStyle = GhostCard.s_ghostStyles.m_missing;
		}
		Material material = Object.Instantiate<Material>(ghostStyle.m_GhostCardMaterial);
		if (this.m_isBigCard)
		{
			material = Object.Instantiate<Material>(ghostStyle.m_GhostBigCardMaterial);
		}
		this.m_R2T_BaseCard.m_Material = material;
		if (this.m_R2T_EffectGhost)
		{
			this.m_R2T_EffectGhost.m_Material = material;
		}
		this.ApplyMaterialByIdx(this.m_CardMesh, ghostStyle.m_GhostMaterial, this.m_CardFrontIdx);
		this.ApplyMaterialByIdx(this.m_CardMesh, ghostStyle.m_GhostMaterial, this.m_PremiumRibbonIdx);
		this.ApplyMaterialByIdx(this.m_PortraitMesh, ghostStyle.m_GhostMaterial, this.m_PortraitFrameIdx);
		if (this.m_GlowPlane)
		{
			if (this.m_AttackMesh != null)
			{
				this.m_GlowPlane.GetComponent<Renderer>().material = ghostStyle.m_GhostMaterialGlowPlane;
			}
			else
			{
				this.m_GlowPlane.GetComponent<Renderer>().material = ghostStyle.m_GhostMaterialAbilityGlowPlane;
			}
		}
		if (this.m_GlowPlaneElite)
		{
			if (this.m_AttackMesh != null)
			{
				this.m_GlowPlaneElite.GetComponent<Renderer>().material = ghostStyle.m_GhostMaterialGlowPlane;
			}
			else
			{
				this.m_GlowPlaneElite.GetComponent<Renderer>().material = ghostStyle.m_GhostMaterialAbilityGlowPlane;
			}
		}
		this.ApplyMaterialByIdx(this.m_DescriptionMesh, ghostStyle.m_GhostMaterialMod2x, 0);
		this.ApplyMaterialByIdx(this.m_DescriptionMesh, ghostStyle.m_GhostMaterial, 1);
		this.ApplyMaterial(this.m_NameMesh, ghostStyle.m_GhostMaterial);
		this.ApplyMaterial(this.m_ManaCostMesh, ghostStyle.m_GhostMaterial);
		this.ApplyMaterial(this.m_AttackMesh, ghostStyle.m_GhostMaterial);
		this.ApplyMaterial(this.m_HealthMesh, ghostStyle.m_GhostMaterial);
		this.ApplyMaterial(this.m_RacePlateMesh, ghostStyle.m_GhostMaterial);
		this.ApplyMaterial(this.m_RarityFrameMesh, ghostStyle.m_GhostMaterial);
		if (ghostStyle.m_GhostMaterialTransparent)
		{
			this.ApplyMaterial(this.m_DescriptionTrimMesh, ghostStyle.m_GhostMaterialTransparent);
		}
		this.ApplyMaterial(this.m_EliteMesh, ghostStyle.m_GhostMaterial);
		SceneUtils.SetRenderQueue(base.gameObject, this.m_R2T_BaseCard.m_RenderQueueOffset + this.m_renderQueue, true);
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x000D1EC0 File Offset: 0x000D00C0
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

	// Token: 0x06002A98 RID: 10904 RVA: 0x000D1F08 File Offset: 0x000D0108
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
		Texture texture = null;
		Material material = go.GetComponent<Renderer>().materials[idx];
		if (material == null)
		{
			return;
		}
		if (material.HasProperty("_SecondTex"))
		{
			texture = material.GetTexture("_SecondTex");
		}
		Color color = Color.clear;
		bool flag = material.HasProperty("_SecondTint");
		if (flag)
		{
			color = material.GetColor("_SecondTint");
		}
		materials[idx] = mat;
		go.GetComponent<Renderer>().materials = materials;
		go.GetComponent<Renderer>().materials[idx].mainTexture = mainTexture;
		if (texture != null)
		{
			go.GetComponent<Renderer>().materials[idx].SetTexture("_SecondTex", texture);
		}
		if (flag)
		{
			go.GetComponent<Renderer>().materials[idx].SetColor("_SecondTint", color);
		}
	}

	// Token: 0x06002A99 RID: 10905 RVA: 0x000D2028 File Offset: 0x000D0228
	private void ApplySharedMaterialByIdx(GameObject go, Material mat, int idx)
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

	// Token: 0x06002A9A RID: 10906 RVA: 0x000D20A4 File Offset: 0x000D02A4
	private void Disable()
	{
		this.RestoreOrgMaterials();
		if (this.m_R2T_BaseCard)
		{
			this.m_R2T_BaseCard.enabled = false;
		}
		if (this.m_R2T_EffectGhost)
		{
			this.m_R2T_EffectGhost.enabled = false;
		}
		if (this.m_GlowPlane)
		{
			this.m_GlowPlane.GetComponent<Renderer>().enabled = false;
		}
		if (this.m_GlowPlaneElite)
		{
			this.m_GlowPlaneElite.GetComponent<Renderer>().enabled = false;
		}
		if (this.m_EffectRoot)
		{
			ParticleSystem componentInChildren = this.m_EffectRoot.GetComponentInChildren<ParticleSystem>();
			if (componentInChildren)
			{
				componentInChildren.Stop();
				componentInChildren.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	// Token: 0x04001970 RID: 6512
	public Actor m_Actor;

	// Token: 0x04001971 RID: 6513
	public Vector3 m_CardOffset = Vector3.zero;

	// Token: 0x04001972 RID: 6514
	public RenderToTexture m_R2T_EffectGhost;

	// Token: 0x04001973 RID: 6515
	public GameObject m_EffectRoot;

	// Token: 0x04001974 RID: 6516
	public GameObject m_GlowPlane;

	// Token: 0x04001975 RID: 6517
	public GameObject m_GlowPlaneElite;

	// Token: 0x04001976 RID: 6518
	private static GhostStyleDef s_ghostStyles;

	// Token: 0x04001977 RID: 6519
	private bool m_isBigCard;

	// Token: 0x04001978 RID: 6520
	private bool m_Init;

	// Token: 0x04001979 RID: 6521
	private RenderToTexture m_R2T_BaseCard;

	// Token: 0x0400197A RID: 6522
	private GhostCard.Type m_ghostType;

	// Token: 0x0400197B RID: 6523
	private int m_renderQueue;

	// Token: 0x0400197C RID: 6524
	private GameObject m_CardMesh;

	// Token: 0x0400197D RID: 6525
	private int m_CardFrontIdx;

	// Token: 0x0400197E RID: 6526
	private int m_PremiumRibbonIdx = -1;

	// Token: 0x0400197F RID: 6527
	private GameObject m_PortraitMesh;

	// Token: 0x04001980 RID: 6528
	private int m_PortraitFrameIdx;

	// Token: 0x04001981 RID: 6529
	private GameObject m_NameMesh;

	// Token: 0x04001982 RID: 6530
	private GameObject m_DescriptionMesh;

	// Token: 0x04001983 RID: 6531
	private GameObject m_DescriptionTrimMesh;

	// Token: 0x04001984 RID: 6532
	private GameObject m_RarityFrameMesh;

	// Token: 0x04001985 RID: 6533
	private GameObject m_ManaCostMesh;

	// Token: 0x04001986 RID: 6534
	private GameObject m_AttackMesh;

	// Token: 0x04001987 RID: 6535
	private GameObject m_HealthMesh;

	// Token: 0x04001988 RID: 6536
	private GameObject m_RacePlateMesh;

	// Token: 0x04001989 RID: 6537
	private GameObject m_EliteMesh;

	// Token: 0x0400198A RID: 6538
	private Material m_OrgMat_CardFront;

	// Token: 0x0400198B RID: 6539
	private Material m_OrgMat_PremiumRibbon;

	// Token: 0x0400198C RID: 6540
	private Material m_OrgMat_PortraitFrame;

	// Token: 0x0400198D RID: 6541
	private Material m_OrgMat_Name;

	// Token: 0x0400198E RID: 6542
	private Material m_OrgMat_Description;

	// Token: 0x0400198F RID: 6543
	private Material m_OrgMat_Description2;

	// Token: 0x04001990 RID: 6544
	private Material m_OrgMat_DescriptionTrim;

	// Token: 0x04001991 RID: 6545
	private Material m_OrgMat_RarityFrame;

	// Token: 0x04001992 RID: 6546
	private Material m_OrgMat_ManaCost;

	// Token: 0x04001993 RID: 6547
	private Material m_OrgMat_Attack;

	// Token: 0x04001994 RID: 6548
	private Material m_OrgMat_Health;

	// Token: 0x04001995 RID: 6549
	private Material m_OrgMat_RacePlate;

	// Token: 0x04001996 RID: 6550
	private Material m_OrgMat_Elite;

	// Token: 0x02000334 RID: 820
	public enum Type
	{
		// Token: 0x04001998 RID: 6552
		NONE,
		// Token: 0x04001999 RID: 6553
		MISSING_UNCRAFTABLE,
		// Token: 0x0400199A RID: 6554
		MISSING,
		// Token: 0x0400199B RID: 6555
		NOT_VALID
	}
}
