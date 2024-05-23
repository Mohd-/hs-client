using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002A7 RID: 679
[CustomEditClass]
public class Actor : MonoBehaviour
{
	// Token: 0x060024BC RID: 9404 RVA: 0x000B4356 File Offset: 0x000B2556
	public virtual void Awake()
	{
		this.AssignRootObject();
		this.AssignBones();
		this.AssignMeshRenderers();
		this.AssignSpells();
	}

	// Token: 0x060024BD RID: 9405 RVA: 0x000B4370 File Offset: 0x000B2570
	private void OnEnable()
	{
		if (this.isPortraitMaterialDirty)
		{
			this.UpdateAllComponents();
		}
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x000B4383 File Offset: 0x000B2583
	private void Start()
	{
		this.Init();
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x000B438C File Offset: 0x000B258C
	public void Init()
	{
		if (this.m_portraitMesh != null)
		{
			this.m_initialPortraitMaterial = RenderUtils.GetMaterial(this.m_portraitMesh, this.m_portraitMatIdx);
		}
		else if (this.m_legacyPortraitMaterialIndex >= 0)
		{
			this.m_initialPortraitMaterial = RenderUtils.GetMaterial(this.m_meshRenderer, this.m_legacyPortraitMaterialIndex);
		}
		if (this.m_rootObject != null)
		{
			TransformUtil.Identity(this.m_rootObject.transform);
		}
		if (this.m_actorStateMgr != null)
		{
			this.m_actorStateMgr.ChangeState(this.m_actorState);
		}
		this.m_projectedShadow = base.GetComponent<ProjectedShadow>();
		if (this.m_shown)
		{
			this.ShowImpl(false);
		}
		else
		{
			this.HideImpl(false);
		}
		if (GraphicsManager.Get() != null)
		{
			this.m_DisablePremiumPortrait = GraphicsManager.Get().isVeryLowQualityDevice();
		}
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x000B4478 File Offset: 0x000B2678
	public void Destroy()
	{
		if (this.m_localSpellTable != null)
		{
			foreach (Spell spell in this.m_localSpellTable.Values)
			{
				spell.Deactivate();
			}
		}
		if (this.m_spellTable != null)
		{
			foreach (SpellTableEntry spellTableEntry in this.m_spellTable.m_Table)
			{
				if (!(spellTableEntry.m_Spell == null))
				{
					spellTableEntry.m_Spell.Deactivate();
				}
			}
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x000B4568 File Offset: 0x000B2768
	public virtual Actor Clone()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(base.gameObject, base.transform.position, base.transform.rotation);
		Actor component = gameObject.GetComponent<Actor>();
		component.SetEntity(this.m_entity);
		component.SetEntityDef(this.m_entityDef);
		component.SetCard(this.m_card);
		component.SetPremium(this.m_premiumType);
		gameObject.transform.localScale = base.gameObject.transform.localScale;
		gameObject.transform.position = base.gameObject.transform.position;
		component.SetActorState(this.m_actorState);
		if (this.m_shown)
		{
			component.ShowImpl(false);
		}
		else
		{
			component.HideImpl(false);
		}
		return component;
	}

	// Token: 0x060024C2 RID: 9410 RVA: 0x000B4634 File Offset: 0x000B2834
	public Card GetCard()
	{
		return this.m_card;
	}

	// Token: 0x060024C3 RID: 9411 RVA: 0x000B463C File Offset: 0x000B283C
	public void SetCard(Card card)
	{
		if (this.m_card == card)
		{
			return;
		}
		if (card == null)
		{
			this.m_card = null;
			base.transform.parent = null;
			return;
		}
		this.m_card = card;
		base.transform.parent = card.transform;
		TransformUtil.Identity(base.transform);
		if (this.m_rootObject != null)
		{
			TransformUtil.Identity(this.m_rootObject.transform);
		}
	}

	// Token: 0x060024C4 RID: 9412 RVA: 0x000B46BF File Offset: 0x000B28BF
	public CardDef GetCardDef()
	{
		return this.m_cardDef;
	}

	// Token: 0x060024C5 RID: 9413 RVA: 0x000B46C7 File Offset: 0x000B28C7
	public void SetCardDef(CardDef cardDef)
	{
		if (this.m_cardDef == cardDef)
		{
			return;
		}
		this.m_cardDef = cardDef;
		this.LoadArmorSpell();
	}

	// Token: 0x060024C6 RID: 9414 RVA: 0x000B46E8 File Offset: 0x000B28E8
	public Entity GetEntity()
	{
		return this.m_entity;
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x000B46F0 File Offset: 0x000B28F0
	public void SetEntity(Entity entity)
	{
		this.m_entity = entity;
		if (this.m_entity == null)
		{
			return;
		}
		this.SetPremium(this.m_entity.GetPremiumType());
	}

	// Token: 0x060024C8 RID: 9416 RVA: 0x000B4721 File Offset: 0x000B2921
	public EntityDef GetEntityDef()
	{
		return this.m_entityDef;
	}

	// Token: 0x060024C9 RID: 9417 RVA: 0x000B4729 File Offset: 0x000B2929
	public void SetEntityDef(EntityDef entityDef)
	{
		this.m_entityDef = entityDef;
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x000B4732 File Offset: 0x000B2932
	public virtual void SetPremium(TAG_PREMIUM premium)
	{
		this.m_premiumType = premium;
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x000B473B File Offset: 0x000B293B
	public TAG_PREMIUM GetPremium()
	{
		return this.m_premiumType;
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x000B4744 File Offset: 0x000B2944
	public TAG_CARD_SET GetCardSet()
	{
		if (this.m_entityDef == null && this.m_entity == null)
		{
			return TAG_CARD_SET.NONE;
		}
		TAG_CARD_SET cardSet;
		if (this.m_entityDef != null)
		{
			cardSet = this.m_entityDef.GetCardSet();
		}
		else
		{
			cardSet = this.m_entity.GetCardSet();
		}
		return cardSet;
	}

	// Token: 0x060024CD RID: 9421 RVA: 0x000B4794 File Offset: 0x000B2994
	public ActorStateType GetActorStateType()
	{
		return (!(this.m_actorStateMgr == null)) ? this.m_actorStateMgr.GetActiveStateType() : ActorStateType.NONE;
	}

	// Token: 0x060024CE RID: 9422 RVA: 0x000B47C3 File Offset: 0x000B29C3
	public void ShowActorState()
	{
	}

	// Token: 0x060024CF RID: 9423 RVA: 0x000B47C5 File Offset: 0x000B29C5
	public void HideActorState()
	{
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x000B47C7 File Offset: 0x000B29C7
	public void SetActorState(ActorStateType stateType)
	{
		this.m_actorState = stateType;
		if (this.m_actorStateMgr == null)
		{
			return;
		}
		if (this.forceIdleState)
		{
			this.m_actorState = ActorStateType.CARD_IDLE;
		}
		this.m_actorStateMgr.ChangeState(this.m_actorState);
	}

	// Token: 0x060024D1 RID: 9425 RVA: 0x000B4806 File Offset: 0x000B2A06
	public void ToggleForceIdle(bool bOn)
	{
		this.forceIdleState = bOn;
	}

	// Token: 0x060024D2 RID: 9426 RVA: 0x000B480F File Offset: 0x000B2A0F
	public void TurnOffCollider()
	{
		this.ToggleCollider(false);
	}

	// Token: 0x060024D3 RID: 9427 RVA: 0x000B4818 File Offset: 0x000B2A18
	public void TurnOnCollider()
	{
		this.ToggleCollider(true);
	}

	// Token: 0x060024D4 RID: 9428 RVA: 0x000B4824 File Offset: 0x000B2A24
	public void ToggleCollider(bool enabled)
	{
		MeshRenderer meshRenderer = this.GetMeshRenderer();
		if (meshRenderer == null || meshRenderer.gameObject.GetComponent<Collider>() == null)
		{
			return;
		}
		meshRenderer.gameObject.GetComponent<Collider>().enabled = enabled;
	}

	// Token: 0x060024D5 RID: 9429 RVA: 0x000B486C File Offset: 0x000B2A6C
	public TAG_RARITY GetRarity()
	{
		if (this.m_entityDef != null)
		{
			return this.m_entityDef.GetRarity();
		}
		if (this.m_entity != null)
		{
			return this.m_entity.GetRarity();
		}
		return TAG_RARITY.FREE;
	}

	// Token: 0x060024D6 RID: 9430 RVA: 0x000B489D File Offset: 0x000B2A9D
	public bool IsElite()
	{
		if (this.m_entityDef != null)
		{
			return this.m_entityDef.IsElite();
		}
		return this.m_entity != null && this.m_entity.IsElite();
	}

	// Token: 0x060024D7 RID: 9431 RVA: 0x000B48CE File Offset: 0x000B2ACE
	public void SetHiddenStandIn(GameObject standIn)
	{
		this.m_hiddenCardStandIn = standIn;
	}

	// Token: 0x060024D8 RID: 9432 RVA: 0x000B48D7 File Offset: 0x000B2AD7
	public GameObject GetHiddenStandIn()
	{
		return this.m_hiddenCardStandIn;
	}

	// Token: 0x060024D9 RID: 9433 RVA: 0x000B48DF File Offset: 0x000B2ADF
	public void SetShadowform(bool shadowform)
	{
		this.m_shadowform = shadowform;
	}

	// Token: 0x060024DA RID: 9434 RVA: 0x000B48E8 File Offset: 0x000B2AE8
	public void SetDisablePremiumPortrait(bool disable)
	{
		this.m_DisablePremiumPortrait = disable;
	}

	// Token: 0x060024DB RID: 9435 RVA: 0x000B48F1 File Offset: 0x000B2AF1
	public bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x000B48F9 File Offset: 0x000B2AF9
	public void Show()
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_shown = true;
		this.ShowImpl(false);
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x000B4915 File Offset: 0x000B2B15
	public void Show(bool ignoreSpells)
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_shown = true;
		this.ShowImpl(ignoreSpells);
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x000B4934 File Offset: 0x000B2B34
	public void ShowSpellTable()
	{
		if (this.m_localSpellTable != null)
		{
			foreach (Spell spell in this.m_localSpellTable.Values)
			{
				spell.Show();
			}
		}
		if (this.m_spellTable != null)
		{
			this.m_spellTable.Show();
		}
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x000B49BC File Offset: 0x000B2BBC
	public void Hide()
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		this.HideImpl(false);
	}

	// Token: 0x060024E0 RID: 9440 RVA: 0x000B49D8 File Offset: 0x000B2BD8
	public void Hide(bool ignoreSpells)
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		this.HideImpl(ignoreSpells);
	}

	// Token: 0x060024E1 RID: 9441 RVA: 0x000B49F4 File Offset: 0x000B2BF4
	public void HideSpellTable()
	{
		if (this.m_localSpellTable != null)
		{
			foreach (Spell spell in this.m_localSpellTable.Values)
			{
				if (spell.GetSpellType() != SpellType.NONE)
				{
					spell.Hide();
				}
			}
		}
		if (this.m_spellTable != null)
		{
			this.m_spellTable.Hide();
		}
	}

	// Token: 0x060024E2 RID: 9442 RVA: 0x000B4A84 File Offset: 0x000B2C84
	protected virtual void ShowImpl(bool ignoreSpells)
	{
		if (this.m_rootObject != null)
		{
			this.m_rootObject.SetActive(true);
		}
		this.ShowAllText();
		this.UpdateAllComponents();
		if (this.m_projectedShadow)
		{
			this.m_projectedShadow.enabled = true;
		}
		if (this.m_actorStateMgr != null)
		{
			this.m_actorStateMgr.ShowStateMgr();
		}
		if (!ignoreSpells)
		{
			this.ShowSpellTable();
		}
		if (this.m_ghostCardGameObject != null)
		{
			this.m_ghostCardGameObject.SetActive(true);
		}
		HighlightState componentInChildren = base.GetComponentInChildren<HighlightState>();
		if (componentInChildren)
		{
			componentInChildren.Show();
		}
	}

	// Token: 0x060024E3 RID: 9443 RVA: 0x000B4B34 File Offset: 0x000B2D34
	protected virtual void HideImpl(bool ignoreSpells)
	{
		if (this.m_rootObject != null)
		{
			this.m_rootObject.SetActive(false);
		}
		if (this.m_armorSpell != null)
		{
			this.m_armorSpell.Hide();
		}
		if (this.m_actorStateMgr != null)
		{
			this.m_actorStateMgr.HideStateMgr();
		}
		if (this.m_projectedShadow)
		{
			this.m_projectedShadow.enabled = false;
		}
		if (this.m_ghostCardGameObject != null)
		{
			this.m_ghostCardGameObject.SetActive(false);
		}
		if (!ignoreSpells)
		{
			this.HideSpellTable();
		}
		if (this.m_missingCardEffect != null)
		{
			this.UpdateMissingCardArt();
		}
		HighlightState componentInChildren = base.GetComponentInChildren<HighlightState>();
		if (componentInChildren)
		{
			componentInChildren.Hide();
		}
	}

	// Token: 0x060024E4 RID: 9444 RVA: 0x000B4C0A File Offset: 0x000B2E0A
	public ActorStateMgr GetActorStateMgr()
	{
		return this.m_actorStateMgr;
	}

	// Token: 0x060024E5 RID: 9445 RVA: 0x000B4C14 File Offset: 0x000B2E14
	public Collider GetCollider()
	{
		if (this.GetMeshRenderer() == null)
		{
			return null;
		}
		return this.GetMeshRenderer().gameObject.GetComponent<Collider>();
	}

	// Token: 0x060024E6 RID: 9446 RVA: 0x000B4C44 File Offset: 0x000B2E44
	public GameObject GetRootObject()
	{
		return this.m_rootObject;
	}

	// Token: 0x060024E7 RID: 9447 RVA: 0x000B4C4C File Offset: 0x000B2E4C
	public MeshRenderer GetMeshRenderer()
	{
		return this.m_meshRenderer;
	}

	// Token: 0x060024E8 RID: 9448 RVA: 0x000B4C54 File Offset: 0x000B2E54
	public GameObject GetBones()
	{
		return this.m_bones;
	}

	// Token: 0x060024E9 RID: 9449 RVA: 0x000B4C5C File Offset: 0x000B2E5C
	public UberText GetPowersText()
	{
		return this.m_powersTextMesh;
	}

	// Token: 0x060024EA RID: 9450 RVA: 0x000B4C64 File Offset: 0x000B2E64
	public UberText GetRaceText()
	{
		return this.m_raceTextMesh;
	}

	// Token: 0x060024EB RID: 9451 RVA: 0x000B4C6C File Offset: 0x000B2E6C
	public UberText GetNameText()
	{
		return this.m_nameTextMesh;
	}

	// Token: 0x060024EC RID: 9452 RVA: 0x000B4C74 File Offset: 0x000B2E74
	public Light GetHeroSpotlight()
	{
		if (this.m_heroSpotLight == null)
		{
			return null;
		}
		return this.m_heroSpotLight.GetComponent<Light>();
	}

	// Token: 0x060024ED RID: 9453 RVA: 0x000B4C94 File Offset: 0x000B2E94
	public GameObject FindBone(string boneName)
	{
		if (this.m_bones == null)
		{
			return null;
		}
		return SceneUtils.FindChildBySubstring(this.m_bones, boneName);
	}

	// Token: 0x060024EE RID: 9454 RVA: 0x000B4CB5 File Offset: 0x000B2EB5
	public GameObject GetCardTypeBannerAnchor()
	{
		if (this.m_cardTypeAnchorObject == null)
		{
			return base.gameObject;
		}
		return this.m_cardTypeAnchorObject;
	}

	// Token: 0x060024EF RID: 9455 RVA: 0x000B4CD5 File Offset: 0x000B2ED5
	public UberText GetAttackText()
	{
		return this.m_attackTextMesh;
	}

	// Token: 0x060024F0 RID: 9456 RVA: 0x000B4CDD File Offset: 0x000B2EDD
	public GameObject GetAttackTextObject()
	{
		if (this.m_attackTextMesh == null)
		{
			return null;
		}
		return this.m_attackTextMesh.gameObject;
	}

	// Token: 0x060024F1 RID: 9457 RVA: 0x000B4CFD File Offset: 0x000B2EFD
	public GemObject GetAttackObject()
	{
		if (this.m_attackObject == null)
		{
			return null;
		}
		return this.m_attackObject.GetComponent<GemObject>();
	}

	// Token: 0x060024F2 RID: 9458 RVA: 0x000B4D1D File Offset: 0x000B2F1D
	public GemObject GetHealthObject()
	{
		if (this.m_healthObject == null)
		{
			return null;
		}
		return this.m_healthObject.GetComponent<GemObject>();
	}

	// Token: 0x060024F3 RID: 9459 RVA: 0x000B4D3D File Offset: 0x000B2F3D
	public UberText GetHealthText()
	{
		return this.m_healthTextMesh;
	}

	// Token: 0x060024F4 RID: 9460 RVA: 0x000B4D45 File Offset: 0x000B2F45
	public GameObject GetHealthTextObject()
	{
		if (this.m_healthTextMesh == null)
		{
			return null;
		}
		return this.m_healthTextMesh.gameObject;
	}

	// Token: 0x060024F5 RID: 9461 RVA: 0x000B4D65 File Offset: 0x000B2F65
	public UberText GetCostText()
	{
		if (this.m_costTextMesh == null)
		{
			return null;
		}
		return this.m_costTextMesh;
	}

	// Token: 0x060024F6 RID: 9462 RVA: 0x000B4D80 File Offset: 0x000B2F80
	public GameObject GetCostTextObject()
	{
		if (this.m_costTextMesh == null)
		{
			return null;
		}
		return this.m_costTextMesh.gameObject;
	}

	// Token: 0x060024F7 RID: 9463 RVA: 0x000B4DA0 File Offset: 0x000B2FA0
	public UberText GetSecretText()
	{
		return this.m_secretText;
	}

	// Token: 0x060024F8 RID: 9464 RVA: 0x000B4DA8 File Offset: 0x000B2FA8
	public void UpdateAllComponents()
	{
		this.UpdateTextComponents();
		this.UpdateMaterials();
		this.UpdateTextures();
		this.UpdateCardBack();
		this.UpdateMeshComponents();
		this.UpdateRootObjectSpellComponents();
		this.UpdateMissingCardArt();
		this.UpdateGhostCardEffect();
	}

	// Token: 0x060024F9 RID: 9465 RVA: 0x000B4DE8 File Offset: 0x000B2FE8
	public bool MissingCardEffect()
	{
		if (this.m_missingCardEffect)
		{
			RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
			if (component)
			{
				this.m_missingcard = true;
				this.UpdateAllComponents();
				return true;
			}
		}
		return false;
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x000B4E2C File Offset: 0x000B302C
	public void DisableMissingCardEffect()
	{
		this.m_missingcard = false;
		if (this.m_missingCardEffect)
		{
			RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
			if (component)
			{
				component.enabled = false;
			}
			this.MaterialShaderAnimation(true);
		}
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x000B4E78 File Offset: 0x000B3078
	public void UpdateMissingCardArt()
	{
		if (!this.m_missingcard)
		{
			return;
		}
		if (this.m_missingCardEffect == null)
		{
			return;
		}
		RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
		if (component == null)
		{
			return;
		}
		if (this.m_rootObject.activeSelf)
		{
			this.MaterialShaderAnimation(false);
			if (this.GetPremium() == TAG_PREMIUM.GOLDEN)
			{
				if (CollectionManager.Get().IsShowingWildTheming(null))
				{
					component.m_Material.color = new Color(0.518f, 0.361f, 0f, 0.68f);
				}
				else
				{
					component.m_Material.color = new Color(0.867f, 0.675f, 0.22f, 0.53f);
				}
			}
			component.enabled = true;
			component.Show(true);
		}
		else
		{
			component.Hide();
		}
	}

	// Token: 0x060024FC RID: 9468 RVA: 0x000B4F58 File Offset: 0x000B3158
	public void SetMissingCardMaterial(Material missingCardMat)
	{
		if (this.m_missingCardEffect == null || missingCardMat == null)
		{
			return;
		}
		RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
		if (component == null)
		{
			return;
		}
		if (this.m_rootObject.activeSelf)
		{
			component.m_Material = missingCardMat;
			this.MaterialShaderAnimation(false);
			if (component.enabled)
			{
				component.Render();
			}
		}
	}

	// Token: 0x060024FD RID: 9469 RVA: 0x000B4FCC File Offset: 0x000B31CC
	public bool isMissingCard()
	{
		if (this.m_missingCardEffect == null)
		{
			return false;
		}
		RenderToTexture component = this.m_missingCardEffect.GetComponent<RenderToTexture>();
		return !(component == null) && component.enabled;
	}

	// Token: 0x060024FE RID: 9470 RVA: 0x000B500C File Offset: 0x000B320C
	public void GhostCardEffect(GhostCard.Type ghostType)
	{
		if (this.m_ghostCard == ghostType)
		{
			return;
		}
		this.m_ghostCard = ghostType;
		this.UpdateAllComponents();
	}

	// Token: 0x060024FF RID: 9471 RVA: 0x000B5028 File Offset: 0x000B3228
	private void UpdateGhostCardEffect()
	{
		if (this.m_ghostCardGameObject == null)
		{
			return;
		}
		GhostCard component = this.m_ghostCardGameObject.GetComponent<GhostCard>();
		if (component == null)
		{
			return;
		}
		if (this.m_ghostCard != GhostCard.Type.NONE)
		{
			component.SetGhostType(this.m_ghostCard);
			component.RenderGhostCard();
		}
		else
		{
			component.DisableGhost();
		}
	}

	// Token: 0x06002500 RID: 9472 RVA: 0x000B5088 File Offset: 0x000B3288
	public bool isGhostCard()
	{
		return this.m_ghostCard != GhostCard.Type.NONE && this.m_ghostCardGameObject;
	}

	// Token: 0x06002501 RID: 9473 RVA: 0x000B50A4 File Offset: 0x000B32A4
	public void UpdateMaterials()
	{
		this.m_lightBlendMaterials = null;
		this.m_lightBlendUberText = null;
		if (base.gameObject.activeInHierarchy)
		{
			base.StartCoroutine(this.UpdatePortraitMaterials());
		}
		else
		{
			this.isPortraitMaterialDirty = true;
		}
	}

	// Token: 0x06002502 RID: 9474 RVA: 0x000B50E8 File Offset: 0x000B32E8
	public void OverrideAllMeshMaterials(Material material)
	{
		if (this.m_rootObject == null)
		{
			return;
		}
		this.RecursivelyReplaceMaterialsList(this.m_rootObject.transform, material);
	}

	// Token: 0x06002503 RID: 9475 RVA: 0x000B5119 File Offset: 0x000B3319
	public void SetUnlit()
	{
		this.SetLightingBlend(0f);
	}

	// Token: 0x06002504 RID: 9476 RVA: 0x000B5126 File Offset: 0x000B3326
	public void SetLit()
	{
		this.SetLightingBlend(1f);
	}

	// Token: 0x06002505 RID: 9477 RVA: 0x000B5134 File Offset: 0x000B3334
	private void SetLightingBlend(float value)
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
		foreach (Renderer renderer in componentsInChildren)
		{
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				if (!(material == null))
				{
					if (material.HasProperty("_LightingBlend"))
					{
						material.SetFloat("_LightingBlend", value);
					}
				}
			}
		}
		foreach (UberText uberText in base.GetComponentsInChildren<UberText>(true))
		{
			uberText.AmbientLightBlend = value;
		}
	}

	// Token: 0x06002506 RID: 9478 RVA: 0x000B51F0 File Offset: 0x000B33F0
	public void SetLightBlend(float blendValue)
	{
		if (this.m_lightBlendMaterials == null)
		{
			this.m_lightBlendMaterials = new List<Material>();
			Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				Material[] materials = renderer.materials;
				foreach (Material material in materials)
				{
					if (!(material == null))
					{
						if (material.HasProperty("_LightingBlend"))
						{
							this.m_lightBlendMaterials.Add(material);
						}
					}
				}
			}
			this.m_lightBlendUberText = new List<UberText>();
			foreach (UberText uberText in base.GetComponentsInChildren<UberText>())
			{
				this.m_lightBlendUberText.Add(uberText);
			}
		}
		foreach (Material material2 in this.m_lightBlendMaterials)
		{
			if (material2 != null && material2.HasProperty("_LightingBlend"))
			{
				material2.SetFloat("_LightingBlend", blendValue);
			}
		}
		foreach (UberText uberText2 in this.m_lightBlendUberText)
		{
			if (uberText2 != null)
			{
				uberText2.AmbientLightBlend = blendValue;
			}
		}
	}

	// Token: 0x06002507 RID: 9479 RVA: 0x000B53A0 File Offset: 0x000B35A0
	public void ReleasePortrait()
	{
		this.SetPortraitMaterial(null);
		this.SetPortraitTexture(null);
		this.SetPortraitTextureOverride(null);
		this.m_lightBlendMaterials = null;
		this.m_lightBlendUberText = null;
	}

	// Token: 0x06002508 RID: 9480 RVA: 0x000B53C8 File Offset: 0x000B35C8
	private void RecursivelyReplaceMaterialsList(Transform transformToRecurse, Material newMaterialPrefab)
	{
		bool flag = true;
		if (transformToRecurse.GetComponent<MaterialReplacementExclude>() != null)
		{
			flag = false;
		}
		else if (transformToRecurse.GetComponent<UberText>() != null)
		{
			flag = false;
		}
		else if (transformToRecurse.GetComponent<Renderer>() == null)
		{
			flag = false;
		}
		if (flag)
		{
			this.ReplaceMaterialsList(transformToRecurse.GetComponent<Renderer>(), newMaterialPrefab);
		}
		foreach (object obj in transformToRecurse)
		{
			Transform transformToRecurse2 = (Transform)obj;
			this.RecursivelyReplaceMaterialsList(transformToRecurse2, newMaterialPrefab);
		}
		this.m_lightBlendMaterials = null;
		this.m_lightBlendUberText = null;
	}

	// Token: 0x06002509 RID: 9481 RVA: 0x000B5490 File Offset: 0x000B3690
	private void ReplaceMaterialsList(Renderer renderer, Material newMaterialPrefab)
	{
		Material[] array = new Material[renderer.materials.Length];
		for (int i = 0; i < renderer.materials.Length; i++)
		{
			Material oldMaterial = renderer.materials[i];
			array[i] = this.CreateReplacementMaterial(oldMaterial, newMaterialPrefab);
		}
		renderer.materials = array;
		this.m_lightBlendMaterials = null;
		this.m_lightBlendUberText = null;
		if (renderer != this.m_meshRenderer)
		{
			return;
		}
		this.UpdatePortraitTexture();
	}

	// Token: 0x0600250A RID: 9482 RVA: 0x000B5508 File Offset: 0x000B3708
	private Material CreateReplacementMaterial(Material oldMaterial, Material newMaterialPrefab)
	{
		Material material = Object.Instantiate<Material>(newMaterialPrefab);
		material.mainTexture = oldMaterial.mainTexture;
		return material;
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x000B552C File Offset: 0x000B372C
	public void SeedMaterialEffects()
	{
		if (this.m_materialEffectsSeeded)
		{
			return;
		}
		this.m_materialEffectsSeeded = true;
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		float num = Random.Range(0f, 2f);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.sharedMaterials.Length == 1)
			{
				if (renderer.material.HasProperty("_Seed") && renderer.material.GetFloat("_Seed") == 0f)
				{
					renderer.material.SetFloat("_Seed", num);
				}
			}
			else
			{
				Material[] materials = renderer.materials;
				if (materials != null && materials.Length != 0)
				{
					foreach (Material material in materials)
					{
						if (!(material == null))
						{
							if (material.HasProperty("_Seed") && material.GetFloat("_Seed") == 0f)
							{
								material.SetFloat("_Seed", num);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600250C RID: 9484 RVA: 0x000B565C File Offset: 0x000B385C
	public void MaterialShaderAnimation(bool animationEnabled)
	{
		float num = 0f;
		if (animationEnabled)
		{
			num = 1f;
		}
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
		foreach (Renderer renderer in componentsInChildren)
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (!(material == null))
				{
					if (material.HasProperty("_TimeScale"))
					{
						material.SetFloat("_TimeScale", num);
					}
				}
			}
		}
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x000B5700 File Offset: 0x000B3900
	public Player.Side GetCardBackSide()
	{
		Player.Side? cardBackSideOverride = this.m_cardBackSideOverride;
		if (cardBackSideOverride != null)
		{
			return this.m_cardBackSideOverride.Value;
		}
		if (this.m_entity == null)
		{
			return Player.Side.FRIENDLY;
		}
		Player controller = this.m_entity.GetController();
		if (controller == null)
		{
			return Player.Side.FRIENDLY;
		}
		return controller.GetSide();
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x000B5753 File Offset: 0x000B3953
	public Player.Side? GetCardBackSideOverride()
	{
		return this.m_cardBackSideOverride;
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x000B575B File Offset: 0x000B395B
	public void SetCardBackSideOverride(Player.Side? sideOverride)
	{
		this.m_cardBackSideOverride = sideOverride;
	}

	// Token: 0x06002510 RID: 9488 RVA: 0x000B5764 File Offset: 0x000B3964
	public bool GetCardbackUpdateIgnore()
	{
		return this.m_ignoreUpdateCardback;
	}

	// Token: 0x06002511 RID: 9489 RVA: 0x000B576C File Offset: 0x000B396C
	public void SetCardbackUpdateIgnore(bool ignoreUpdate)
	{
		this.m_ignoreUpdateCardback = ignoreUpdate;
	}

	// Token: 0x06002512 RID: 9490 RVA: 0x000B5778 File Offset: 0x000B3978
	public void UpdateCardBack()
	{
		if (this.m_ignoreUpdateCardback)
		{
			return;
		}
		CardBackManager cardBackManager = CardBackManager.Get();
		if (cardBackManager == null)
		{
			return;
		}
		bool friendlySide = this.GetCardBackSide() == Player.Side.FRIENDLY;
		this.UpdateCardBackDisplay(friendlySide);
		this.UpdateCardBackDragEffect();
		if (this.m_cardMesh == null || this.m_cardBackMatIdx < 0)
		{
			return;
		}
		cardBackManager.SetCardBackTexture(this.m_cardMesh.GetComponent<Renderer>(), this.m_cardBackMatIdx, friendlySide);
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x000B57F4 File Offset: 0x000B39F4
	private void UpdateCardBackDragEffect()
	{
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
		{
			return;
		}
		CardBackDragEffect componentInChildren = base.GetComponentInChildren<CardBackDragEffect>();
		if (componentInChildren == null)
		{
			return;
		}
		componentInChildren.SetEffect();
	}

	// Token: 0x06002514 RID: 9492 RVA: 0x000B582C File Offset: 0x000B3A2C
	private void UpdateCardBackDisplay(bool friendlySide)
	{
		CardBackDisplay componentInChildren = base.GetComponentInChildren<CardBackDisplay>();
		if (componentInChildren == null)
		{
			return;
		}
		componentInChildren.SetCardBack(friendlySide);
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x000B5854 File Offset: 0x000B3A54
	private void UpdateTextures()
	{
		this.UpdatePortraitTexture();
	}

	// Token: 0x06002516 RID: 9494 RVA: 0x000B585C File Offset: 0x000B3A5C
	public void UpdatePortraitTexture()
	{
		if (this.m_portraitTextureOverride != null)
		{
			this.SetPortraitTexture(this.m_portraitTextureOverride);
		}
		else if (this.m_cardDef != null)
		{
			this.SetPortraitTexture(this.m_cardDef.GetPortraitTexture());
		}
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x000B58B0 File Offset: 0x000B3AB0
	public void SetPortraitTexture(Texture texture)
	{
		if (this.m_cardDef != null && !this.m_DisablePremiumPortrait && (this.m_premiumType == TAG_PREMIUM.GOLDEN || this.m_cardDef.m_AlwaysRenderPremiumPortrait) && this.m_cardDef.GetPremiumPortraitMaterial() != null)
		{
			return;
		}
		Material portraitMaterial = this.GetPortraitMaterial();
		if (portraitMaterial == null)
		{
			return;
		}
		portraitMaterial.mainTexture = texture;
	}

	// Token: 0x06002518 RID: 9496 RVA: 0x000B5927 File Offset: 0x000B3B27
	public void SetPortraitTextureOverride(Texture portrait)
	{
		this.m_portraitTextureOverride = portrait;
		this.UpdatePortraitTexture();
	}

	// Token: 0x06002519 RID: 9497 RVA: 0x000B5938 File Offset: 0x000B3B38
	public Texture GetPortraitTexture()
	{
		Material portraitMaterial = this.GetPortraitMaterial();
		if (portraitMaterial == null)
		{
			return null;
		}
		return portraitMaterial.mainTexture;
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000B5960 File Offset: 0x000B3B60
	private IEnumerator UpdatePortraitMaterials()
	{
		this.isPortraitMaterialDirty = false;
		if (this.m_shadowform)
		{
			yield break;
		}
		if (this.m_cardDef == null)
		{
			yield break;
		}
		if (!this.m_DisablePremiumPortrait && (this.m_premiumType == TAG_PREMIUM.GOLDEN || this.m_cardDef.m_AlwaysRenderPremiumPortrait))
		{
			if (!this.m_cardDef.IsPremiumLoaded())
			{
				yield return null;
			}
			if (this.m_cardDef.GetPremiumPortraitMaterial() != null)
			{
				this.SetPortraitMaterial(this.m_cardDef.GetPremiumPortraitMaterial());
			}
			else if (this.m_initialPortraitMaterial != null)
			{
				this.SetPortraitMaterial(this.m_initialPortraitMaterial);
			}
		}
		else
		{
			this.SetPortraitMaterial(this.m_initialPortraitMaterial);
		}
		yield break;
	}

	// Token: 0x0600251B RID: 9499 RVA: 0x000B597C File Offset: 0x000B3B7C
	public void SetPortraitMaterial(Material material)
	{
		if (material == null)
		{
			return;
		}
		if (this.m_portraitMesh != null && this.m_portraitMatIdx > -1)
		{
			Material material2 = RenderUtils.GetMaterial(this.m_portraitMesh, this.m_portraitMatIdx);
			if (material2.mainTexture == material.mainTexture && material2.shader == material.shader)
			{
				return;
			}
			if (material == null)
			{
				RenderUtils.SetMaterial(this.m_portraitMesh, this.m_portraitMatIdx, this.m_initialPortraitMaterial);
			}
			else
			{
				RenderUtils.SetMaterial(this.m_portraitMesh, this.m_portraitMatIdx, material);
			}
			if (this.m_entity != null)
			{
				float num = 0f;
				if (this.m_entity.GetZone() == TAG_ZONE.PLAY)
				{
					num = 1f;
				}
				foreach (Material material3 in this.m_portraitMesh.GetComponent<Renderer>().materials)
				{
					if (material3.HasProperty("_LightingBlend"))
					{
						material3.SetFloat("_LightingBlend", num);
					}
					if (material3.HasProperty("_Seed") && material3.GetFloat("_Seed") == 0f)
					{
						material3.SetFloat("_Seed", Random.Range(0f, 2f));
					}
				}
			}
		}
		else if (this.m_legacyPortraitMaterialIndex >= 0)
		{
			Material material4 = RenderUtils.GetMaterial(this.m_meshRenderer, this.m_legacyPortraitMaterialIndex);
			if (material4 == material)
			{
				return;
			}
			RenderUtils.SetMaterial(this.m_meshRenderer, this.m_legacyPortraitMaterialIndex, material);
		}
	}

	// Token: 0x0600251C RID: 9500 RVA: 0x000B5B21 File Offset: 0x000B3D21
	public GameObject GetPortraitMesh()
	{
		return this.m_portraitMesh;
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x000B5B2C File Offset: 0x000B3D2C
	protected virtual Material GetPortraitMaterial()
	{
		if (this.m_portraitMesh != null && 0 <= this.m_portraitMatIdx && this.m_portraitMatIdx < this.m_portraitMesh.GetComponent<Renderer>().materials.Length)
		{
			return this.m_portraitMesh.GetComponent<Renderer>().materials[this.m_portraitMatIdx];
		}
		if (this.m_legacyPortraitMaterialIndex >= 0)
		{
			return this.m_meshRenderer.materials[this.m_legacyPortraitMaterialIndex];
		}
		return null;
	}

	// Token: 0x0600251E RID: 9502 RVA: 0x000B5BAC File Offset: 0x000B3DAC
	public virtual void UpdateTextComponents()
	{
		if (this.m_entityDef != null)
		{
			this.UpdateTextComponentsDef(this.m_entityDef);
		}
		else
		{
			this.UpdateTextComponents(this.m_entity);
		}
	}

	// Token: 0x0600251F RID: 9503 RVA: 0x000B5BE4 File Offset: 0x000B3DE4
	public virtual void UpdateTextComponentsDef(EntityDef entityDef)
	{
		if (entityDef == null)
		{
			return;
		}
		if (this.m_costTextMesh != null)
		{
			if (entityDef.HasTag(GAME_TAG.HIDE_COST))
			{
				this.m_costTextMesh.Text = string.Empty;
			}
			else if (entityDef.HasTriggerVisual() && entityDef.IsHeroPower())
			{
				this.m_costTextMesh.Text = string.Empty;
			}
			else
			{
				this.m_costTextMesh.Text = Convert.ToString(entityDef.GetTag(GAME_TAG.COST));
			}
		}
		int tag = entityDef.GetTag(GAME_TAG.ATK);
		if (entityDef.IsHero())
		{
			if (tag == 0)
			{
				if (this.m_attackObject != null && this.m_attackObject.activeSelf)
				{
					this.m_attackObject.SetActive(false);
				}
				if (this.m_attackTextMesh != null)
				{
					this.m_attackTextMesh.Text = string.Empty;
				}
			}
			else
			{
				if (this.m_attackObject != null && !this.m_attackObject.activeSelf)
				{
					this.m_attackObject.SetActive(true);
				}
				if (this.m_attackTextMesh != null)
				{
					this.m_attackTextMesh.Text = Convert.ToString(tag);
				}
			}
		}
		else if (this.m_attackTextMesh != null)
		{
			this.m_attackTextMesh.Text = Convert.ToString(tag);
		}
		if (this.m_healthTextMesh != null)
		{
			if (entityDef.IsWeapon())
			{
				this.m_healthTextMesh.Text = Convert.ToString(entityDef.GetTag(GAME_TAG.DURABILITY));
			}
			else
			{
				this.m_healthTextMesh.Text = Convert.ToString(entityDef.GetTag(GAME_TAG.HEALTH));
			}
		}
		this.UpdateNameText();
		this.UpdatePowersText();
		this.UpdateRace(entityDef.GetRaceText());
		this.UpdateSecretText();
	}

	// Token: 0x06002520 RID: 9504 RVA: 0x000B5DC8 File Offset: 0x000B3FC8
	public void UpdateMinionStatsImmediately()
	{
		if (this.m_entity == null || !this.m_entity.IsMinion())
		{
			return;
		}
		if (this.m_attackTextMesh != null)
		{
			this.UpdateTextColorToGreenOrWhite(this.m_attackTextMesh, this.m_entity.GetOriginalATK(), this.m_entity.GetATK());
			this.m_attackTextMesh.Text = Convert.ToString(this.m_entity.GetATK());
		}
		if (this.m_healthTextMesh != null)
		{
			int health = this.m_entity.GetHealth();
			int originalHealth = this.m_entity.GetOriginalHealth();
			int num = health - this.m_entity.GetDamage();
			if (this.m_entity.GetDamage() > 0)
			{
				this.UpdateTextColor(this.m_healthTextMesh, health, num);
			}
			else if (health > originalHealth)
			{
				this.UpdateTextColor(this.m_healthTextMesh, originalHealth, num);
			}
			else
			{
				this.UpdateTextColor(this.m_healthTextMesh, num, num);
			}
			this.m_healthTextMesh.Text = Convert.ToString(num);
		}
	}

	// Token: 0x06002521 RID: 9505 RVA: 0x000B5ED4 File Offset: 0x000B40D4
	public virtual void UpdateTextComponents(Entity entity)
	{
		if (entity == null)
		{
			return;
		}
		if (this.m_costTextMesh != null)
		{
			if (this.m_entity.HasTag(GAME_TAG.HIDE_COST))
			{
				this.UpdateNumberText(this.m_costTextMesh, string.Empty, true);
			}
			else
			{
				if (this.m_entity.IsSecret() && this.m_entity.IsHidden() && this.m_entity.IsControlledByConcealedPlayer())
				{
					this.m_costTextMesh.TextColor = Color.white;
				}
				else
				{
					this.UpdateTextColor(this.m_costTextMesh, entity.GetOriginalCost(), entity.GetCost(), true);
				}
				if (this.m_entity.HasTriggerVisual() && this.m_entity.IsHeroPower())
				{
					this.UpdateNumberText(this.m_costTextMesh, string.Empty, true);
				}
				else
				{
					this.UpdateNumberText(this.m_costTextMesh, Convert.ToString(entity.GetCost()));
				}
			}
		}
		if (this.m_attackTextMesh != null)
		{
			if (entity.IsHero())
			{
				int atk = entity.GetATK();
				if (atk == 0)
				{
					this.UpdateNumberText(this.m_attackTextMesh, string.Empty, true);
				}
				else
				{
					this.UpdateNumberText(this.m_attackTextMesh, Convert.ToString(atk));
				}
			}
			else
			{
				this.UpdateTextColorToGreenOrWhite(this.m_attackTextMesh, entity.GetOriginalATK(), entity.GetATK());
				this.UpdateNumberText(this.m_attackTextMesh, Convert.ToString(entity.GetATK()));
			}
		}
		if (this.m_healthTextMesh != null && (!entity.IsHero() || entity.GetZone() != TAG_ZONE.GRAVEYARD))
		{
			int num;
			int num2;
			if (entity.IsWeapon())
			{
				num = entity.GetDurability();
				num2 = entity.GetOriginalDurability();
			}
			else
			{
				num = entity.GetHealth();
				num2 = entity.GetOriginalHealth();
			}
			int num3 = num - entity.GetDamage();
			if (entity.GetDamage() > 0)
			{
				this.UpdateTextColor(this.m_healthTextMesh, num, num3);
			}
			else if (num > num2)
			{
				this.UpdateTextColor(this.m_healthTextMesh, num2, num3);
			}
			else
			{
				this.UpdateTextColor(this.m_healthTextMesh, num3, num3);
			}
			this.UpdateNumberText(this.m_healthTextMesh, Convert.ToString(num3));
		}
		this.UpdateNameText();
		this.UpdatePowersText();
		this.UpdateRace(entity.GetRaceText());
		this.UpdateSecretText();
	}

	// Token: 0x06002522 RID: 9506 RVA: 0x000B612C File Offset: 0x000B432C
	public void UpdatePowersText()
	{
		if (this.m_powersTextMesh == null)
		{
			return;
		}
		string text;
		if (this.m_entityDef != null)
		{
			text = this.m_entityDef.GetCardTextInHand();
		}
		else
		{
			if (this.m_entity.IsSecret() && this.m_entity.IsHidden() && this.m_entity.IsControlledByConcealedPlayer())
			{
				text = GameStrings.Get("GAMEPLAY_SECRET_DESC");
			}
			else if (this.m_entity.IsHistoryDupe())
			{
				text = this.m_entity.GetCardTextInHistory();
			}
			else
			{
				text = this.m_entity.GetCardTextInHand();
			}
			if (GameState.Get() != null && GameState.Get().GetGameEntity() != null)
			{
				text = GameState.Get().GetGameEntity().UpdateCardText(this.m_card, this, text);
			}
		}
		this.UpdateText(this.m_powersTextMesh, text);
	}

	// Token: 0x06002523 RID: 9507 RVA: 0x000B6212 File Offset: 0x000B4412
	private void UpdateNumberText(UberText textMesh, string newText)
	{
		this.UpdateNumberText(textMesh, newText, false);
	}

	// Token: 0x06002524 RID: 9508 RVA: 0x000B6220 File Offset: 0x000B4420
	private void UpdateNumberText(UberText textMesh, string newText, bool shouldHide)
	{
		GemObject gemObject = SceneUtils.FindComponentInThisOrParents<GemObject>(textMesh.gameObject);
		if (gemObject != null)
		{
			if (!gemObject.IsNumberHidden())
			{
				if (shouldHide)
				{
					textMesh.gameObject.SetActive(false);
					if (this.GetHistoryCard() != null || this.GetHistoryChildCard() != null)
					{
						gemObject.Hide();
					}
					else
					{
						gemObject.ScaleToZero();
					}
				}
				else if (textMesh.Text != newText)
				{
					gemObject.Jiggle();
				}
			}
			else if (!shouldHide)
			{
				textMesh.gameObject.SetActive(true);
				gemObject.SetToZeroThenEnlarge();
			}
			gemObject.Initialize();
			gemObject.SetHideNumberFlag(shouldHide);
		}
		textMesh.Text = newText;
	}

	// Token: 0x06002525 RID: 9509 RVA: 0x000B62E4 File Offset: 0x000B44E4
	private void UpdateNameText()
	{
		if (this.m_nameTextMesh == null)
		{
			return;
		}
		bool flag = false;
		string text;
		if (this.m_entityDef != null)
		{
			text = this.m_entityDef.GetName();
		}
		else
		{
			flag = (this.m_entity.IsSecret() && this.m_entity.IsHidden() && this.m_entity.IsControlledByConcealedPlayer());
			text = this.m_entity.GetName();
		}
		if (flag)
		{
			bool flag2 = GameState.Get().GetGameEntity().ShouldUseSecretClassNames();
			if (flag2)
			{
				switch (this.m_entity.GetClass())
				{
				case TAG_CLASS.HUNTER:
					text = GameStrings.Get("GAMEPLAY_SECRET_NAME_HUNTER");
					break;
				case TAG_CLASS.MAGE:
					text = GameStrings.Get("GAMEPLAY_SECRET_NAME_MAGE");
					break;
				case TAG_CLASS.PALADIN:
					text = GameStrings.Get("GAMEPLAY_SECRET_NAME_PALADIN");
					break;
				default:
					text = GameStrings.Get("GAMEPLAY_SECRET_NAME");
					break;
				}
			}
			else
			{
				text = GameStrings.Get("GAMEPLAY_SECRET_NAME");
			}
		}
		this.UpdateText(this.m_nameTextMesh, text);
	}

	// Token: 0x06002526 RID: 9510 RVA: 0x000B63FC File Offset: 0x000B45FC
	private void UpdateSecretText()
	{
		if (!this.m_secretText)
		{
			return;
		}
		string text = "?";
		if (UniversalInputManager.UsePhoneUI && this.m_entity != null)
		{
			TransformUtil.SetLocalPosZ(this.m_secretText, -0.01f);
			Player controller = this.m_entity.GetController();
			if (controller != null)
			{
				ZoneSecret secretZone = controller.GetSecretZone();
				if (secretZone)
				{
					int cardCount = secretZone.GetCardCount();
					if (cardCount > 1)
					{
						text = string.Format("{0}", cardCount);
						TransformUtil.SetLocalPosZ(this.m_secretText, -0.03f);
					}
				}
			}
			Transform transform = this.m_secretText.transform.parent.FindChild("Secret_mesh");
			if (transform != null && transform.gameObject != null)
			{
				SphereCollider component = transform.gameObject.GetComponent<SphereCollider>();
				if (component != null)
				{
					component.radius = 0.5f;
				}
			}
		}
		this.UpdateText(this.m_secretText, text);
	}

	// Token: 0x06002527 RID: 9511 RVA: 0x000B650C File Offset: 0x000B470C
	private void UpdateText(UberText uberTextMesh, string text)
	{
		if (uberTextMesh == null)
		{
			return;
		}
		uberTextMesh.Text = text;
	}

	// Token: 0x06002528 RID: 9512 RVA: 0x000B6522 File Offset: 0x000B4722
	private void UpdateTextColor(UberText originalMesh, int baseNumber, int currentNumber)
	{
		this.UpdateTextColor(originalMesh, baseNumber, currentNumber, false);
	}

	// Token: 0x06002529 RID: 9513 RVA: 0x000B6530 File Offset: 0x000B4730
	private void UpdateTextColor(UberText uberTextMesh, int baseNumber, int currentNumber, bool higherIsBetter)
	{
		if ((baseNumber > currentNumber && higherIsBetter) || (baseNumber < currentNumber && !higherIsBetter))
		{
			uberTextMesh.TextColor = Color.green;
		}
		else if ((baseNumber < currentNumber && higherIsBetter) || (baseNumber > currentNumber && !higherIsBetter))
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				uberTextMesh.TextColor = new Color(1f, 0.19607843f, 0.19607843f);
			}
			else
			{
				uberTextMesh.TextColor = Color.red;
			}
		}
		else if (baseNumber == currentNumber)
		{
			uberTextMesh.TextColor = Color.white;
		}
	}

	// Token: 0x0600252A RID: 9514 RVA: 0x000B65D8 File Offset: 0x000B47D8
	private void UpdateTextColorToGreenOrWhite(UberText uberTextMesh, int baseNumber, int currentNumber)
	{
		if (baseNumber < currentNumber)
		{
			uberTextMesh.TextColor = Color.green;
		}
		else
		{
			uberTextMesh.TextColor = Color.white;
		}
	}

	// Token: 0x0600252B RID: 9515 RVA: 0x000B6607 File Offset: 0x000B4807
	private void DisableTextMesh(UberText mesh)
	{
		if (mesh == null)
		{
			return;
		}
		mesh.gameObject.SetActive(false);
	}

	// Token: 0x0600252C RID: 9516 RVA: 0x000B6624 File Offset: 0x000B4824
	public void OverrideNameText(UberText newText)
	{
		if (this.m_nameTextMesh != null)
		{
			this.m_nameTextMesh.gameObject.SetActive(false);
		}
		this.m_nameTextMesh = newText;
		this.UpdateNameText();
		if (this.m_shown && newText != null)
		{
			newText.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600252D RID: 9517 RVA: 0x000B6683 File Offset: 0x000B4883
	public void HideAllText()
	{
		this.ToggleTextVisibility(false);
	}

	// Token: 0x0600252E RID: 9518 RVA: 0x000B668C File Offset: 0x000B488C
	public void ShowAllText()
	{
		this.ToggleTextVisibility(true);
	}

	// Token: 0x0600252F RID: 9519 RVA: 0x000B6698 File Offset: 0x000B4898
	private void ToggleTextVisibility(bool bOn)
	{
		if (this.m_healthTextMesh != null)
		{
			this.m_healthTextMesh.gameObject.SetActive(bOn);
		}
		if (this.m_attackTextMesh != null)
		{
			this.m_attackTextMesh.gameObject.SetActive(bOn);
		}
		if (this.m_nameTextMesh != null)
		{
			this.m_nameTextMesh.gameObject.SetActive(bOn);
			if (this.m_nameTextMesh.RenderOnObject)
			{
				this.m_nameTextMesh.RenderOnObject.GetComponent<Renderer>().enabled = bOn;
			}
		}
		if (this.m_powersTextMesh != null)
		{
			this.m_powersTextMesh.gameObject.SetActive(bOn);
		}
		if (this.m_costTextMesh != null)
		{
			this.m_costTextMesh.gameObject.SetActive(bOn);
		}
		if (this.m_raceTextMesh != null)
		{
			this.m_raceTextMesh.gameObject.SetActive(bOn);
		}
		if (this.m_secretText)
		{
			this.m_secretText.gameObject.SetActive(bOn);
		}
	}

	// Token: 0x06002530 RID: 9520 RVA: 0x000B67C0 File Offset: 0x000B49C0
	public void ContactShadow(bool visible)
	{
		string tag = "FakeShadow";
		string tag2 = "FakeShadowUnique";
		GameObject gameObject = SceneUtils.FindChildByTag(base.gameObject, tag);
		GameObject gameObject2 = SceneUtils.FindChildByTag(base.gameObject, tag2);
		if (visible)
		{
			if (this.IsElite())
			{
				if (gameObject != null)
				{
					gameObject.GetComponent<Renderer>().enabled = false;
				}
				if (gameObject2 != null)
				{
					gameObject2.GetComponent<Renderer>().enabled = true;
				}
			}
			else
			{
				if (gameObject != null)
				{
					gameObject.GetComponent<Renderer>().enabled = true;
				}
				if (gameObject2 != null)
				{
					gameObject2.GetComponent<Renderer>().enabled = false;
				}
			}
		}
		else
		{
			if (gameObject != null)
			{
				gameObject.GetComponent<Renderer>().enabled = false;
			}
			if (gameObject2 != null)
			{
				gameObject2.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	// Token: 0x06002531 RID: 9521 RVA: 0x000B68A0 File Offset: 0x000B4AA0
	public void UpdateMeshComponents()
	{
		this.UpdateRarityComponent();
		this.UpdateWatermark();
		this.UpdateEliteComponent();
		this.UpdatePremiumComponents();
		this.UpdateCardColor();
	}

	// Token: 0x06002532 RID: 9522 RVA: 0x000B68CC File Offset: 0x000B4ACC
	private void UpdateRarityComponent()
	{
		if (!this.m_rarityGemMesh)
		{
			return;
		}
		Vector2 mainTextureOffset;
		Color color;
		bool rarityTextureOffset = this.GetRarityTextureOffset(out mainTextureOffset, out color);
		SceneUtils.EnableRenderers(this.m_rarityGemMesh, rarityTextureOffset, true);
		if (this.m_rarityFrameMesh)
		{
			SceneUtils.EnableRenderers(this.m_rarityFrameMesh, rarityTextureOffset, true);
		}
		if (!rarityTextureOffset)
		{
			return;
		}
		this.m_rarityGemMesh.GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset;
		this.m_rarityGemMesh.GetComponent<Renderer>().material.SetColor("_tint", color);
	}

	// Token: 0x06002533 RID: 9523 RVA: 0x000B6958 File Offset: 0x000B4B58
	private bool GetRarityTextureOffset(out Vector2 offset, out Color tint)
	{
		offset = this.GEM_TEXTURE_OFFSET_COMMON;
		tint = this.GEM_COLOR_COMMON;
		if (this.m_entityDef == null && this.m_entity == null)
		{
			return false;
		}
		TAG_CARD_SET cardSet;
		if (this.m_entityDef != null)
		{
			cardSet = this.m_entityDef.GetCardSet();
		}
		else
		{
			cardSet = this.m_entity.GetCardSet();
		}
		if (cardSet == TAG_CARD_SET.CORE)
		{
			return false;
		}
		if (cardSet == TAG_CARD_SET.MISSIONS)
		{
			return false;
		}
		switch (this.GetRarity())
		{
		case TAG_RARITY.COMMON:
			offset = this.GEM_TEXTURE_OFFSET_COMMON;
			tint = this.GEM_COLOR_COMMON;
			return true;
		case TAG_RARITY.RARE:
			offset = this.GEM_TEXTURE_OFFSET_RARE;
			tint = this.GEM_COLOR_RARE;
			return true;
		case TAG_RARITY.EPIC:
			offset = this.GEM_TEXTURE_OFFSET_EPIC;
			tint = this.GEM_COLOR_EPIC;
			return true;
		case TAG_RARITY.LEGENDARY:
			offset = this.GEM_TEXTURE_OFFSET_LEGENDARY;
			tint = this.GEM_COLOR_LEGENDARY;
			return true;
		}
		return false;
	}

	// Token: 0x06002534 RID: 9524 RVA: 0x000B6A70 File Offset: 0x000B4C70
	private void UpdateWatermark()
	{
		if (this.m_entityDef == null && this.m_entity == null)
		{
			return;
		}
		TAG_CARD_SET cardSet = this.GetCardSet();
		if (!this.m_descriptionMesh)
		{
			return;
		}
		if (!this.m_descriptionMesh.GetComponent<Renderer>().material.HasProperty("_SecondTint"))
		{
			return;
		}
		string name = "Set1_Icon";
		TAG_CARD_SET tag_CARD_SET = cardSet;
		float a;
		switch (tag_CARD_SET)
		{
		case TAG_CARD_SET.FP1:
			name = "NaxxIcon";
			a = 0.7734375f;
			break;
		case TAG_CARD_SET.PE1:
			name = "GvGIcon";
			a = 0.7734375f;
			break;
		case TAG_CARD_SET.BRM:
			name = "BRMIcon";
			a = 0.7734375f;
			break;
		case TAG_CARD_SET.TGT:
			name = "TGTIcon";
			a = 0.7734375f;
			break;
		default:
			if (tag_CARD_SET != TAG_CARD_SET.EXPERT1)
			{
				a = 0f;
			}
			else
			{
				a = 0.7734375f;
			}
			break;
		case TAG_CARD_SET.LOE:
			name = "LOEIcon";
			a = 0.7734375f;
			break;
		case TAG_CARD_SET.OG:
			name = "OGIcon";
			a = 0.7734375f;
			break;
		}
		Texture texture = AssetLoader.Get().LoadTexture(name, false);
		this.m_descriptionMesh.GetComponent<Renderer>().material.SetTexture("_SecondTex", texture);
		Color color = this.m_descriptionMesh.GetComponent<Renderer>().material.GetColor("_SecondTint");
		color.a = a;
		this.m_descriptionMesh.GetComponent<Renderer>().material.SetColor("_SecondTint", color);
	}

	// Token: 0x06002535 RID: 9525 RVA: 0x000B6C00 File Offset: 0x000B4E00
	private void UpdateEliteComponent()
	{
		if (this.m_eliteObject == null)
		{
			return;
		}
		bool enable = this.IsElite();
		SceneUtils.EnableRenderers(this.m_eliteObject, enable, true);
	}

	// Token: 0x06002536 RID: 9526 RVA: 0x000B6C34 File Offset: 0x000B4E34
	private void UpdatePremiumComponents()
	{
		if (this.m_premiumType == TAG_PREMIUM.NORMAL)
		{
			return;
		}
		if (this.m_glints == null)
		{
			return;
		}
		this.m_glints.SetActive(true);
		Renderer[] componentsInChildren = this.m_glints.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = true;
		}
	}

	// Token: 0x06002537 RID: 9527 RVA: 0x000B6C98 File Offset: 0x000B4E98
	private void UpdateRace(string raceText)
	{
		if (!this.m_racePlateObject)
		{
			return;
		}
		bool flag = !string.IsNullOrEmpty(raceText);
		MeshRenderer[] components = this.m_racePlateObject.GetComponents<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in components)
		{
			meshRenderer.enabled = flag;
		}
		if (flag)
		{
			if (this.m_descriptionMesh)
			{
				this.m_descriptionMesh.GetComponent<Renderer>().material.SetTextureOffset("_SecondTex", new Vector2(-0.04f, 0f));
			}
		}
		else if (this.m_descriptionMesh)
		{
			this.m_descriptionMesh.GetComponent<Renderer>().material.SetTextureOffset("_SecondTex", new Vector2(-0.04f, 0.07f));
		}
		if (!this.m_raceTextMesh)
		{
			return;
		}
		this.m_raceTextMesh.Text = raceText;
	}

	// Token: 0x06002538 RID: 9528 RVA: 0x000B6D8C File Offset: 0x000B4F8C
	public void UpdateCardColor()
	{
		if (this.m_legacyPortraitMaterialIndex < 0 && this.m_cardMesh == null)
		{
			return;
		}
		if (this.GetEntityDef() == null && this.GetEntity() == null)
		{
			return;
		}
		TAG_CARDTYPE cardType;
		TAG_CLASS @class;
		if (this.m_entityDef != null)
		{
			cardType = this.m_entityDef.GetCardType();
			@class = this.m_entityDef.GetClass();
		}
		else
		{
			cardType = this.m_entity.GetCardType();
			@class = this.m_entity.GetClass();
		}
		Color color = Color.magenta;
		CardColorSwitcher.CardColorType colorType;
		switch (@class)
		{
		case TAG_CLASS.DRUID:
			colorType = CardColorSwitcher.CardColorType.TYPE_DRUID;
			color = this.CLASS_COLOR_DRUID;
			break;
		case TAG_CLASS.HUNTER:
			colorType = CardColorSwitcher.CardColorType.TYPE_HUNTER;
			color = this.CLASS_COLOR_HUNTER;
			break;
		case TAG_CLASS.MAGE:
			colorType = CardColorSwitcher.CardColorType.TYPE_MAGE;
			color = this.CLASS_COLOR_MAGE;
			break;
		case TAG_CLASS.PALADIN:
			colorType = CardColorSwitcher.CardColorType.TYPE_PALADIN;
			color = this.CLASS_COLOR_PALADIN;
			break;
		case TAG_CLASS.PRIEST:
			colorType = CardColorSwitcher.CardColorType.TYPE_PRIEST;
			color = this.CLASS_COLOR_PRIEST;
			break;
		case TAG_CLASS.ROGUE:
			colorType = CardColorSwitcher.CardColorType.TYPE_ROGUE;
			color = this.CLASS_COLOR_ROGUE;
			break;
		case TAG_CLASS.SHAMAN:
			colorType = CardColorSwitcher.CardColorType.TYPE_SHAMAN;
			color = this.CLASS_COLOR_SHAMAN;
			break;
		case TAG_CLASS.WARLOCK:
			colorType = CardColorSwitcher.CardColorType.TYPE_WARLOCK;
			color = this.CLASS_COLOR_WARLOCK;
			break;
		case TAG_CLASS.WARRIOR:
			colorType = CardColorSwitcher.CardColorType.TYPE_WARRIOR;
			color = this.CLASS_COLOR_WARRIOR;
			break;
		case TAG_CLASS.DREAM:
			colorType = CardColorSwitcher.CardColorType.TYPE_HUNTER;
			color = this.CLASS_COLOR_HUNTER;
			break;
		default:
			colorType = CardColorSwitcher.CardColorType.TYPE_GENERIC;
			color = this.CLASS_COLOR_GENERIC;
			break;
		}
		switch (cardType)
		{
		case TAG_CARDTYPE.HERO:
			if (this.m_entity != null)
			{
				if (!this.m_entity.IsControlledByFriendlySidePlayer())
				{
					if (this.m_entity.IsHistoryDupe())
					{
						Transform transform = this.GetRootObject().transform.FindChild("History_Hero_Banner");
						if (!(transform == null))
						{
							transform.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.005f, -0.505f);
						}
					}
				}
			}
			break;
		case TAG_CARDTYPE.MINION:
		{
			TAG_PREMIUM premiumType = this.m_premiumType;
			if (premiumType != TAG_PREMIUM.NORMAL)
			{
				if (premiumType != TAG_PREMIUM.GOLDEN)
				{
					Debug.LogWarning(string.Format("Actor.UpdateCardColor(): unexpected premium type {0}", this.m_premiumType));
				}
				else if (this.m_premiumRibbon >= 0)
				{
					if (this.m_cardMesh)
					{
						Material material = this.m_cardMesh.GetComponent<Renderer>().materials[this.m_premiumRibbon];
						material.color = color;
					}
				}
			}
			else if (!(CardColorSwitcher.Get() == null))
			{
				Texture mainTexture = null;
				if (CardColorSwitcher.Get())
				{
					mainTexture = CardColorSwitcher.Get().GetMinionTexture(colorType);
				}
				if (this.m_cardMesh)
				{
					if (this.m_cardFrontMatIdx > -1)
					{
						this.m_cardMesh.GetComponent<Renderer>().materials[this.m_cardFrontMatIdx].mainTexture = mainTexture;
					}
				}
				else if (this.m_legacyCardColorMaterialIndex >= 0)
				{
					this.m_meshRenderer.GetComponent<Renderer>().materials[this.m_legacyCardColorMaterialIndex].mainTexture = mainTexture;
				}
			}
			break;
		}
		case TAG_CARDTYPE.SPELL:
		{
			TAG_PREMIUM premiumType = this.m_premiumType;
			if (premiumType != TAG_PREMIUM.NORMAL)
			{
				if (premiumType != TAG_PREMIUM.GOLDEN)
				{
					Debug.LogWarning(string.Format("Actor.UpdateCardColor(): unexpected premium type {0}", this.m_premiumType));
				}
				else if (this.m_premiumRibbon >= 0)
				{
					if (this.m_cardMesh)
					{
						Material material2 = this.m_cardMesh.GetComponent<Renderer>().materials[this.m_premiumRibbon];
						material2.color = color;
					}
				}
			}
			else if (!(CardColorSwitcher.Get() == null))
			{
				Texture mainTexture2 = null;
				if (CardColorSwitcher.Get())
				{
					mainTexture2 = CardColorSwitcher.Get().GetSpellTexture(colorType);
				}
				if (this.m_cardMesh)
				{
					if (this.m_cardFrontMatIdx > -1)
					{
						this.m_cardMesh.GetComponent<Renderer>().materials[this.m_cardFrontMatIdx].mainTexture = mainTexture2;
					}
					if (this.m_portraitMesh && this.m_portraitFrameMatIdx > -1)
					{
						this.m_portraitMesh.GetComponent<Renderer>().materials[this.m_portraitFrameMatIdx].mainTexture = mainTexture2;
					}
				}
				else if (this.m_legacyCardColorMaterialIndex >= 0 && this.m_meshRenderer != null)
				{
					this.m_meshRenderer.materials[this.m_legacyCardColorMaterialIndex].mainTexture = mainTexture2;
				}
			}
			break;
		}
		case TAG_CARDTYPE.WEAPON:
		{
			TAG_PREMIUM premiumType = this.m_premiumType;
			if (premiumType != TAG_PREMIUM.NORMAL)
			{
				if (premiumType == TAG_PREMIUM.GOLDEN)
				{
					if (this.m_premiumRibbon >= 0)
					{
						if (this.m_cardMesh)
						{
							Material material3 = this.m_cardMesh.GetComponent<Renderer>().materials[this.m_premiumRibbon];
							material3.color = color;
						}
					}
				}
			}
			else if (this.m_descriptionTrimMesh)
			{
				this.m_descriptionTrimMesh.GetComponent<Renderer>().material.SetColor("_Color", color);
			}
			break;
		}
		}
	}

	// Token: 0x06002539 RID: 9529 RVA: 0x000B72D4 File Offset: 0x000B54D4
	public HistoryCard GetHistoryCard()
	{
		if (base.transform.parent == null)
		{
			return null;
		}
		return base.transform.parent.gameObject.GetComponent<HistoryCard>();
	}

	// Token: 0x0600253A RID: 9530 RVA: 0x000B7310 File Offset: 0x000B5510
	public HistoryChildCard GetHistoryChildCard()
	{
		if (base.transform.parent == null)
		{
			return null;
		}
		return base.transform.parent.gameObject.GetComponent<HistoryChildCard>();
	}

	// Token: 0x0600253B RID: 9531 RVA: 0x000B734C File Offset: 0x000B554C
	public void SetHistoryItem(HistoryItem card)
	{
		if (card == null)
		{
			base.transform.parent = null;
			return;
		}
		base.transform.parent = card.transform;
		TransformUtil.Identity(base.transform);
		if (this.m_rootObject != null)
		{
			TransformUtil.Identity(this.m_rootObject.transform);
		}
		this.m_entity = card.GetEntity();
		this.UpdateTextComponents(this.m_entity);
		this.UpdateMeshComponents();
		if (this.m_premiumType == TAG_PREMIUM.GOLDEN && card.GetPortraitGoldenMaterial() != null)
		{
			Material portraitGoldenMaterial = card.GetPortraitGoldenMaterial();
			this.SetPortraitMaterial(portraitGoldenMaterial);
		}
		else
		{
			Texture portraitTexture = card.GetPortraitTexture();
			this.SetPortraitTextureOverride(portraitTexture);
		}
		if (this.m_spellTable != null)
		{
			foreach (SpellTableEntry spellTableEntry in this.m_spellTable.m_Table)
			{
				Spell spell = spellTableEntry.m_Spell;
				if (!(spell == null))
				{
					spell.m_BlockServerEvents = false;
				}
			}
		}
	}

	// Token: 0x0600253C RID: 9532 RVA: 0x000B748C File Offset: 0x000B568C
	public SpellTable GetSpellTable()
	{
		return this.m_spellTable;
	}

	// Token: 0x0600253D RID: 9533 RVA: 0x000B7494 File Offset: 0x000B5694
	public Spell LoadSpell(SpellType spellType)
	{
		if (this.m_sharedSpellTable == null)
		{
			return null;
		}
		Spell spell = this.m_sharedSpellTable.GetSpell(spellType);
		if (spell == null)
		{
			return null;
		}
		this.m_localSpellTable.Add(spellType, spell);
		Transform transform = spell.gameObject.transform;
		Transform transform2 = base.gameObject.transform;
		TransformUtil.AttachAndPreserveLocalTransform(transform, transform2);
		transform.localScale.Scale(this.m_sharedSpellTable.gameObject.transform.localScale);
		SceneUtils.SetLayer(spell.gameObject, (GameLayer)base.gameObject.layer);
		spell.OnLoad();
		if (this.m_actorStateMgr != null)
		{
			spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnSpellStateStarted));
		}
		return spell;
	}

	// Token: 0x0600253E RID: 9534 RVA: 0x000B7560 File Offset: 0x000B5760
	public Spell GetLoadedSpell(SpellType spellType)
	{
		Spell spell = null;
		if (this.m_localSpellTable != null)
		{
			this.m_localSpellTable.TryGetValue(spellType, out spell);
		}
		if (spell == null)
		{
			spell = this.LoadSpell(spellType);
		}
		return spell;
	}

	// Token: 0x0600253F RID: 9535 RVA: 0x000B75A0 File Offset: 0x000B57A0
	public Spell ActivateTaunt()
	{
		this.DeactivateTaunt();
		if (this.GetEntity().IsStealthed() && !Options.Get().GetBool(Option.HAS_SEEN_STEALTH_TAUNTER, false))
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_STEALTH_TAUNT3_22"), "VO_INNKEEPER_STEALTH_TAUNT3_22", 0f, null);
			Options.Get().SetBool(Option.HAS_SEEN_STEALTH_TAUNTER, true);
		}
		if (this.m_premiumType == TAG_PREMIUM.GOLDEN)
		{
			if (this.GetEntity().IsStealthed())
			{
				return this.ActivateSpell(SpellType.TAUNT_PREMIUM_STEALTH);
			}
			return this.ActivateSpell(SpellType.TAUNT_PREMIUM);
		}
		else
		{
			if (this.GetEntity().IsStealthed())
			{
				return this.ActivateSpell(SpellType.TAUNT_STEALTH);
			}
			return this.ActivateSpell(SpellType.TAUNT);
		}
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x000B7654 File Offset: 0x000B5854
	public void DeactivateTaunt()
	{
		if (this.IsSpellActive(SpellType.TAUNT))
		{
			this.DeactivateSpell(SpellType.TAUNT);
		}
		if (this.IsSpellActive(SpellType.TAUNT_PREMIUM))
		{
			this.DeactivateSpell(SpellType.TAUNT_PREMIUM);
		}
		if (this.IsSpellActive(SpellType.TAUNT_PREMIUM_STEALTH))
		{
			this.DeactivateSpell(SpellType.TAUNT_PREMIUM_STEALTH);
		}
		if (this.IsSpellActive(SpellType.TAUNT_STEALTH))
		{
			this.DeactivateSpell(SpellType.TAUNT_STEALTH);
		}
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x000B76B4 File Offset: 0x000B58B4
	public Spell GetSpell(SpellType spellType)
	{
		Spell result = null;
		if (this.m_useSharedSpellTable)
		{
			result = this.GetLoadedSpell(spellType);
		}
		else if (this.m_spellTable != null)
		{
			this.m_spellTable.FindSpell(spellType, out result);
		}
		return result;
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x000B76FC File Offset: 0x000B58FC
	public Spell GetSpellIfLoaded(SpellType spellType)
	{
		Spell result = null;
		if (this.m_useSharedSpellTable)
		{
			this.GetSpellIfLoaded(spellType, out result);
		}
		else if (this.m_spellTable != null)
		{
			this.m_spellTable.FindSpell(spellType, out result);
		}
		return result;
	}

	// Token: 0x06002543 RID: 9539 RVA: 0x000B7746 File Offset: 0x000B5946
	public bool GetSpellIfLoaded(SpellType spellType, out Spell result)
	{
		if (this.m_localSpellTable == null || !this.m_localSpellTable.ContainsKey(spellType))
		{
			result = null;
			return false;
		}
		result = this.m_localSpellTable[spellType];
		return result != null;
	}

	// Token: 0x06002544 RID: 9540 RVA: 0x000B7780 File Offset: 0x000B5980
	public Spell ActivateSpell(SpellType spellType)
	{
		Spell spell = this.GetSpell(spellType);
		if (spell == null)
		{
			return null;
		}
		spell.ActivateState(SpellStateType.BIRTH);
		return spell;
	}

	// Token: 0x06002545 RID: 9541 RVA: 0x000B77AC File Offset: 0x000B59AC
	public bool IsSpellActive(SpellType spellType)
	{
		Spell spellIfLoaded = this.GetSpellIfLoaded(spellType);
		return !(spellIfLoaded == null) && spellIfLoaded.IsActive();
	}

	// Token: 0x06002546 RID: 9542 RVA: 0x000B77D8 File Offset: 0x000B59D8
	public void DeactivateSpell(SpellType spellType)
	{
		Spell spellIfLoaded = this.GetSpellIfLoaded(spellType);
		if (spellIfLoaded == null)
		{
			return;
		}
		spellIfLoaded.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x06002547 RID: 9543 RVA: 0x000B7804 File Offset: 0x000B5A04
	public void DeactivateAllSpells()
	{
		if (this.m_useSharedSpellTable)
		{
			foreach (Spell spell in this.m_localSpellTable.Values)
			{
				if (spell.IsActive())
				{
					spell.ActivateState(SpellStateType.DEATH);
				}
			}
		}
		else if (this.m_spellTable != null)
		{
			foreach (SpellTableEntry spellTableEntry in this.m_spellTable.m_Table)
			{
				Spell spell2 = spellTableEntry.m_Spell;
				if (!(spell2 == null))
				{
					if (spell2.IsActive())
					{
						spell2.ActivateState(SpellStateType.DEATH);
					}
				}
			}
		}
	}

	// Token: 0x06002548 RID: 9544 RVA: 0x000B790C File Offset: 0x000B5B0C
	public void DeactivateAllPreDeathSpells()
	{
		Array values = Enum.GetValues(typeof(SpellType));
		foreach (object obj in values)
		{
			SpellType spellType = (SpellType)((int)obj);
			if (this.IsSpellActive(spellType))
			{
				if (spellType != SpellType.DEATH)
				{
					if (spellType != SpellType.DEATHRATTLE_DEATH)
					{
						if (spellType != SpellType.DAMAGE)
						{
							this.DeactivateSpell(spellType);
						}
					}
				}
			}
		}
	}

	// Token: 0x06002549 RID: 9545 RVA: 0x000B79B0 File Offset: 0x000B5BB0
	public void DestroySpell(SpellType spellType)
	{
		if (this.m_useSharedSpellTable)
		{
			Spell spell;
			if (this.m_localSpellTable.TryGetValue(spellType, out spell))
			{
				Object.Destroy(spell.gameObject);
				this.m_localSpellTable.Remove(spellType);
			}
		}
		else
		{
			Debug.LogError(string.Format("Actor.DestroySpell() - FAILED to destroy {0} because the Actor is not using a shared spell table.", spellType));
		}
	}

	// Token: 0x0600254A RID: 9546 RVA: 0x000B7A0D File Offset: 0x000B5C0D
	public void HideArmorSpell()
	{
		this.m_armorSpell.gameObject.SetActive(false);
	}

	// Token: 0x0600254B RID: 9547 RVA: 0x000B7A20 File Offset: 0x000B5C20
	private void UpdateRootObjectSpellComponents()
	{
		if (this.m_entity == null)
		{
			return;
		}
		if (this.m_armorSpellLoading)
		{
			base.StartCoroutine(this.UpdateArmorSpellWhenLoaded());
		}
		if (this.m_armorSpell != null)
		{
			this.UpdateArmorSpell();
		}
	}

	// Token: 0x0600254C RID: 9548 RVA: 0x000B7A60 File Offset: 0x000B5C60
	private IEnumerator UpdateArmorSpellWhenLoaded()
	{
		while (this.m_armorSpellLoading)
		{
			yield return null;
		}
		this.UpdateArmorSpell();
		yield break;
	}

	// Token: 0x0600254D RID: 9549 RVA: 0x000B7A7C File Offset: 0x000B5C7C
	private void UpdateArmorSpell()
	{
		if (!this.m_armorSpell.gameObject.activeInHierarchy)
		{
			return;
		}
		int armor = this.m_entity.GetArmor();
		int armor2 = this.m_armorSpell.GetArmor();
		this.m_armorSpell.SetArmor(armor);
		if (armor > 0)
		{
			bool flag = this.m_armorSpell.IsShown();
			if (!flag)
			{
				this.m_armorSpell.Show();
			}
			if (armor2 <= 0)
			{
				base.StartCoroutine(this.ActivateArmorSpell(SpellStateType.BIRTH, true));
			}
			else if (armor2 > armor)
			{
				base.StartCoroutine(this.ActivateArmorSpell(SpellStateType.ACTION, true));
			}
			else if (armor2 < armor)
			{
				base.StartCoroutine(this.ActivateArmorSpell(SpellStateType.CANCEL, true));
			}
			else if (!flag)
			{
				base.StartCoroutine(this.ActivateArmorSpell(SpellStateType.IDLE, true));
			}
		}
		else if (armor2 > 0)
		{
			base.StartCoroutine(this.ActivateArmorSpell(SpellStateType.DEATH, false));
		}
	}

	// Token: 0x0600254E RID: 9550 RVA: 0x000B7B68 File Offset: 0x000B5D68
	private IEnumerator ActivateArmorSpell(SpellStateType stateType, bool armorShouldBeOn)
	{
		while (this.m_armorSpell.GetActiveState() != SpellStateType.NONE)
		{
			yield return null;
		}
		if (this.m_armorSpell.GetActiveState() == stateType)
		{
			yield break;
		}
		int armor = this.m_entity.GetArmor();
		if ((armorShouldBeOn && armor <= 0) || (!armorShouldBeOn && armor > 0))
		{
			yield break;
		}
		this.m_armorSpell.ActivateState(stateType);
		yield break;
	}

	// Token: 0x0600254F RID: 9551 RVA: 0x000B7BA0 File Offset: 0x000B5DA0
	private void OnSpellStateStarted(Spell spell, SpellStateType prevStateType, object userData)
	{
		spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnSpellStateStarted));
		this.m_actorStateMgr.RefreshStateMgr();
		if (this.m_projectedShadow)
		{
			this.m_projectedShadow.UpdateContactShadow();
		}
	}

	// Token: 0x06002550 RID: 9552 RVA: 0x000B7BE5 File Offset: 0x000B5DE5
	private void AssignRootObject()
	{
		this.m_rootObject = SceneUtils.FindChildBySubstring(base.gameObject, "RootObject");
	}

	// Token: 0x06002551 RID: 9553 RVA: 0x000B7BFD File Offset: 0x000B5DFD
	private void AssignBones()
	{
		this.m_bones = SceneUtils.FindChildBySubstring(base.gameObject, "Bones");
	}

	// Token: 0x06002552 RID: 9554 RVA: 0x000B7C18 File Offset: 0x000B5E18
	private void AssignMeshRenderers()
	{
		MeshRenderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<MeshRenderer>(true);
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			GameObject gameObject = meshRenderer.gameObject;
			if (gameObject.name.Equals("Mesh", 5))
			{
				this.m_meshRenderer = meshRenderer;
				MeshRenderer[] componentsInChildren2 = meshRenderer.gameObject.GetComponentsInChildren<MeshRenderer>(true);
				foreach (MeshRenderer meshRenderer2 in componentsInChildren2)
				{
					this.AssignMaterials(meshRenderer2);
				}
				break;
			}
		}
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x000B7CB4 File Offset: 0x000B5EB4
	private void AssignMaterials(MeshRenderer meshRenderer)
	{
		for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
		{
			Material sharedMaterial = RenderUtils.GetSharedMaterial(meshRenderer, i);
			if (!(sharedMaterial == null))
			{
				if (sharedMaterial.name.LastIndexOf("Portrait", 5) >= 0)
				{
					this.m_legacyPortraitMaterialIndex = i;
				}
				else if (sharedMaterial.name.IndexOf("Card_Inhand_Ability_Warlock", 5) >= 0)
				{
					this.m_legacyCardColorMaterialIndex = i;
				}
				else if (sharedMaterial.name.IndexOf("Card_Inhand_Warlock", 5) >= 0)
				{
					this.m_legacyCardColorMaterialIndex = i;
				}
				else if (sharedMaterial.name.IndexOf("Card_Inhand_Weapon_Warlock", 5) >= 0)
				{
					this.m_legacyCardColorMaterialIndex = i;
				}
			}
		}
	}

	// Token: 0x06002554 RID: 9556 RVA: 0x000B7D80 File Offset: 0x000B5F80
	private void AssignSpells()
	{
		this.m_spellTable = base.gameObject.GetComponentInChildren<SpellTable>();
		this.m_actorStateMgr = base.gameObject.GetComponentInChildren<ActorStateMgr>();
		if (this.m_spellTable == null)
		{
			if (!string.IsNullOrEmpty(this.m_spellTablePrefab))
			{
				SpellCache spellCache = SpellCache.Get();
				if (spellCache != null)
				{
					SpellTable spellTable = spellCache.GetSpellTable(this.m_spellTablePrefab);
					if (spellTable != null)
					{
						this.m_useSharedSpellTable = true;
						this.m_sharedSpellTable = spellTable;
						this.m_localSpellTable = new Map<SpellType, Spell>();
					}
					else
					{
						Debug.LogWarning("failed to load spell table: " + this.m_spellTablePrefab);
					}
				}
				else
				{
					Debug.LogWarning("Null spell cache: " + this.m_spellTablePrefab);
				}
			}
		}
		else if (this.m_actorStateMgr != null)
		{
			foreach (SpellTableEntry spellTableEntry in this.m_spellTable.m_Table)
			{
				if (!(spellTableEntry.m_Spell == null))
				{
					spellTableEntry.m_Spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnSpellStateStarted));
				}
			}
		}
	}

	// Token: 0x06002555 RID: 9557 RVA: 0x000B7ED8 File Offset: 0x000B60D8
	private void LoadArmorSpell()
	{
		if (this.m_armorSpellBone == null)
		{
			return;
		}
		this.m_armorSpellLoading = true;
		string name = "Assets/Game/Spells/Armor/Hero_Armor";
		if (this.GetCardDef() != null && this.GetCardDef().m_CustomHeroArmorSpell != string.Empty)
		{
			name = this.GetCardDef().m_CustomHeroArmorSpell;
		}
		AssetLoader.Get().LoadSpell(name, new AssetLoader.GameObjectCallback(this.OnArmorSpellLoaded), null, false);
	}

	// Token: 0x06002556 RID: 9558 RVA: 0x000B7F58 File Offset: 0x000B6158
	private void OnArmorSpellLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("{0} - Actor.OnArmorSpellLoaded() - failed to load Hero_Armor spell! m_armorSpell GameObject = null!", name));
			return;
		}
		this.m_armorSpellLoading = false;
		this.m_armorSpell = go.GetComponent<ArmorSpell>();
		if (this.m_armorSpell == null)
		{
			Debug.LogError(string.Format("{0} - Actor.OnArmorSpellLoaded() - failed to load Hero_Armor spell! m_armorSpell Spell = null!", name));
			return;
		}
		go.transform.parent = this.m_armorSpellBone.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
	}

	// Token: 0x040015B6 RID: 5558
	protected const string WATERMARK_EXPERT1 = "Set1_Icon";

	// Token: 0x040015B7 RID: 5559
	protected const string WATERMARK_FP1 = "NaxxIcon";

	// Token: 0x040015B8 RID: 5560
	protected const string WATERMARK_GVG = "GvGIcon";

	// Token: 0x040015B9 RID: 5561
	protected const string WATERMARK_BRM = "BRMIcon";

	// Token: 0x040015BA RID: 5562
	protected const string WATERMARK_TGT = "TGTIcon";

	// Token: 0x040015BB RID: 5563
	protected const string WATERMARK_LOE = "LOEIcon";

	// Token: 0x040015BC RID: 5564
	protected const string WATERMARK_OG = "OGIcon";

	// Token: 0x040015BD RID: 5565
	protected readonly Vector2 GEM_TEXTURE_OFFSET_RARE = new Vector2(0.5f, 0f);

	// Token: 0x040015BE RID: 5566
	protected readonly Vector2 GEM_TEXTURE_OFFSET_EPIC = new Vector2(0f, 0.5f);

	// Token: 0x040015BF RID: 5567
	protected readonly Vector2 GEM_TEXTURE_OFFSET_LEGENDARY = new Vector2(0.5f, 0.5f);

	// Token: 0x040015C0 RID: 5568
	protected readonly Vector2 GEM_TEXTURE_OFFSET_COMMON = new Vector2(0f, 0f);

	// Token: 0x040015C1 RID: 5569
	protected readonly Color GEM_COLOR_RARE = new Color(0.1529f, 0.498f, 1f);

	// Token: 0x040015C2 RID: 5570
	protected readonly Color GEM_COLOR_EPIC = new Color(0.596f, 0.1568f, 0.7333f);

	// Token: 0x040015C3 RID: 5571
	protected readonly Color GEM_COLOR_LEGENDARY = new Color(1f, 0.5333f, 0f);

	// Token: 0x040015C4 RID: 5572
	protected readonly Color GEM_COLOR_COMMON = new Color(0.549f, 0.549f, 0.549f);

	// Token: 0x040015C5 RID: 5573
	protected readonly Color CLASS_COLOR_GENERIC = new Color(0.8f, 0.8f, 0.8f);

	// Token: 0x040015C6 RID: 5574
	protected readonly Color CLASS_COLOR_WARLOCK = new Color(0.33f, 0.2f, 0.4f);

	// Token: 0x040015C7 RID: 5575
	protected readonly Color CLASS_COLOR_ROGUE = new Color(0.72f, 0.68f, 0.76f);

	// Token: 0x040015C8 RID: 5576
	protected readonly Color CLASS_COLOR_DRUID = new Color(0.42f, 0.29f, 0.14f);

	// Token: 0x040015C9 RID: 5577
	protected readonly Color CLASS_COLOR_SHAMAN = new Color(0f, 0.32f, 0.71f);

	// Token: 0x040015CA RID: 5578
	protected readonly Color CLASS_COLOR_HUNTER = new Color(0.26f, 0.54f, 0.18f);

	// Token: 0x040015CB RID: 5579
	protected readonly Color CLASS_COLOR_MAGE = new Color(0.44f, 0.48f, 0.69f);

	// Token: 0x040015CC RID: 5580
	protected readonly Color CLASS_COLOR_PALADIN = new Color(0.71f, 0.49f, 0.2f);

	// Token: 0x040015CD RID: 5581
	protected readonly Color CLASS_COLOR_PRIEST = new Color(0.65f, 0.65f, 0.65f);

	// Token: 0x040015CE RID: 5582
	protected readonly Color CLASS_COLOR_WARRIOR = new Color(0.43f, 0.14f, 0.14f);

	// Token: 0x040015CF RID: 5583
	public GameObject m_cardMesh;

	// Token: 0x040015D0 RID: 5584
	public int m_cardFrontMatIdx = -1;

	// Token: 0x040015D1 RID: 5585
	public int m_cardBackMatIdx = -1;

	// Token: 0x040015D2 RID: 5586
	public int m_premiumRibbon = -1;

	// Token: 0x040015D3 RID: 5587
	public GameObject m_portraitMesh;

	// Token: 0x040015D4 RID: 5588
	public int m_portraitFrameMatIdx = -1;

	// Token: 0x040015D5 RID: 5589
	public int m_portraitMatIdx = -1;

	// Token: 0x040015D6 RID: 5590
	public GameObject m_nameBannerMesh;

	// Token: 0x040015D7 RID: 5591
	public GameObject m_descriptionMesh;

	// Token: 0x040015D8 RID: 5592
	public GameObject m_descriptionTrimMesh;

	// Token: 0x040015D9 RID: 5593
	public GameObject m_rarityFrameMesh;

	// Token: 0x040015DA RID: 5594
	public GameObject m_rarityGemMesh;

	// Token: 0x040015DB RID: 5595
	public GameObject m_racePlateMesh;

	// Token: 0x040015DC RID: 5596
	public GameObject m_attackObject;

	// Token: 0x040015DD RID: 5597
	public GameObject m_healthObject;

	// Token: 0x040015DE RID: 5598
	public GameObject m_manaObject;

	// Token: 0x040015DF RID: 5599
	public GameObject m_racePlateObject;

	// Token: 0x040015E0 RID: 5600
	public GameObject m_cardTypeAnchorObject;

	// Token: 0x040015E1 RID: 5601
	public GameObject m_eliteObject;

	// Token: 0x040015E2 RID: 5602
	public GameObject m_classIconObject;

	// Token: 0x040015E3 RID: 5603
	public GameObject m_heroSpotLight;

	// Token: 0x040015E4 RID: 5604
	public GameObject m_glints;

	// Token: 0x040015E5 RID: 5605
	public GameObject m_armorSpellBone;

	// Token: 0x040015E6 RID: 5606
	public UberText m_costTextMesh;

	// Token: 0x040015E7 RID: 5607
	public UberText m_attackTextMesh;

	// Token: 0x040015E8 RID: 5608
	public UberText m_healthTextMesh;

	// Token: 0x040015E9 RID: 5609
	public UberText m_nameTextMesh;

	// Token: 0x040015EA RID: 5610
	public UberText m_powersTextMesh;

	// Token: 0x040015EB RID: 5611
	public UberText m_raceTextMesh;

	// Token: 0x040015EC RID: 5612
	public UberText m_secretText;

	// Token: 0x040015ED RID: 5613
	public GameObject m_missingCardEffect;

	// Token: 0x040015EE RID: 5614
	public GameObject m_ghostCardGameObject;

	// Token: 0x040015EF RID: 5615
	[CustomEditField(T = EditType.ACTOR)]
	public string m_spellTablePrefab;

	// Token: 0x040015F0 RID: 5616
	protected Card m_card;

	// Token: 0x040015F1 RID: 5617
	protected Entity m_entity;

	// Token: 0x040015F2 RID: 5618
	protected CardDef m_cardDef;

	// Token: 0x040015F3 RID: 5619
	protected EntityDef m_entityDef;

	// Token: 0x040015F4 RID: 5620
	protected TAG_PREMIUM m_premiumType;

	// Token: 0x040015F5 RID: 5621
	protected ProjectedShadow m_projectedShadow;

	// Token: 0x040015F6 RID: 5622
	protected bool m_shown = true;

	// Token: 0x040015F7 RID: 5623
	protected ActorStateMgr m_actorStateMgr;

	// Token: 0x040015F8 RID: 5624
	protected ActorStateType m_actorState = ActorStateType.CARD_IDLE;

	// Token: 0x040015F9 RID: 5625
	protected bool forceIdleState;

	// Token: 0x040015FA RID: 5626
	protected GameObject m_rootObject;

	// Token: 0x040015FB RID: 5627
	protected GameObject m_bones;

	// Token: 0x040015FC RID: 5628
	protected MeshRenderer m_meshRenderer;

	// Token: 0x040015FD RID: 5629
	protected int m_legacyPortraitMaterialIndex = -1;

	// Token: 0x040015FE RID: 5630
	protected int m_legacyCardColorMaterialIndex = -1;

	// Token: 0x040015FF RID: 5631
	protected Material m_initialPortraitMaterial;

	// Token: 0x04001600 RID: 5632
	protected List<Material> m_lightBlendMaterials;

	// Token: 0x04001601 RID: 5633
	protected List<UberText> m_lightBlendUberText;

	// Token: 0x04001602 RID: 5634
	protected SpellTable m_sharedSpellTable;

	// Token: 0x04001603 RID: 5635
	protected bool m_useSharedSpellTable;

	// Token: 0x04001604 RID: 5636
	protected Map<SpellType, Spell> m_localSpellTable;

	// Token: 0x04001605 RID: 5637
	protected SpellTable m_spellTable;

	// Token: 0x04001606 RID: 5638
	protected ArmorSpell m_armorSpell;

	// Token: 0x04001607 RID: 5639
	protected GameObject m_hiddenCardStandIn;

	// Token: 0x04001608 RID: 5640
	protected bool m_shadowform;

	// Token: 0x04001609 RID: 5641
	protected GhostCard.Type m_ghostCard;

	// Token: 0x0400160A RID: 5642
	protected bool m_missingcard;

	// Token: 0x0400160B RID: 5643
	protected bool m_armorSpellLoading;

	// Token: 0x0400160C RID: 5644
	protected bool m_materialEffectsSeeded;

	// Token: 0x0400160D RID: 5645
	protected Player.Side? m_cardBackSideOverride;

	// Token: 0x0400160E RID: 5646
	protected bool m_ignoreUpdateCardback;

	// Token: 0x0400160F RID: 5647
	protected bool isPortraitMaterialDirty;

	// Token: 0x04001610 RID: 5648
	protected bool m_DisablePremiumPortrait;

	// Token: 0x04001611 RID: 5649
	protected Texture m_portraitTextureOverride;

	// Token: 0x04001612 RID: 5650
	private ActorStateType m_actualState;

	// Token: 0x04001613 RID: 5651
	private bool m_hideActorState;
}
