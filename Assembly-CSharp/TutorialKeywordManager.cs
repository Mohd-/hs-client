using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200071E RID: 1822
public class TutorialKeywordManager : MonoBehaviour
{
	// Token: 0x06004A61 RID: 19041 RVA: 0x00163BF7 File Offset: 0x00161DF7
	private void Awake()
	{
		TutorialKeywordManager.s_instance = this;
	}

	// Token: 0x06004A62 RID: 19042 RVA: 0x00163BFF File Offset: 0x00161DFF
	private void OnDestroy()
	{
		TutorialKeywordManager.s_instance = null;
	}

	// Token: 0x06004A63 RID: 19043 RVA: 0x00163C07 File Offset: 0x00161E07
	public static TutorialKeywordManager Get()
	{
		return TutorialKeywordManager.s_instance;
	}

	// Token: 0x06004A64 RID: 19044 RVA: 0x00163C10 File Offset: 0x00161E10
	public void UpdateKeywordHelp(Card c, Actor a)
	{
		this.UpdateKeywordHelp(c, a, true, default(float?));
	}

	// Token: 0x06004A65 RID: 19045 RVA: 0x00163C2F File Offset: 0x00161E2F
	public void UpdateKeywordHelp(Card card, Actor actor, bool showOnRight, float? overrideScale = null)
	{
		this.m_card = card;
		this.UpdateKeywordHelp(card.GetEntity(), actor, showOnRight, overrideScale);
	}

	// Token: 0x06004A66 RID: 19046 RVA: 0x00163C48 File Offset: 0x00161E48
	public void UpdateKeywordHelp(Entity entity, Actor actor, bool showOnRight, float? overrideScale = null)
	{
		float num = 1f;
		if (overrideScale != null)
		{
			num = overrideScale.Value;
		}
		this.PrepareToUpdateKeywordHelp(actor);
		string[] array = GameState.Get().GetGameEntity().NotifyOfKeywordHelpPanelDisplay(entity);
		if (array != null)
		{
			this.SetupKeywordPanel(array[0], array[1]);
		}
		this.SetUpPanels(entity);
		TutorialKeywordTooltip tutorialKeywordTooltip = null;
		for (int i = 0; i < this.m_keywordPanels.Count; i++)
		{
			TutorialKeywordTooltip tutorialKeywordTooltip2 = this.m_keywordPanels[i];
			float num2 = 1.05f;
			if (entity.IsHero())
			{
				num2 = 1.2f;
			}
			else if (entity.GetZone() == TAG_ZONE.PLAY)
			{
				if (UniversalInputManager.UsePhoneUI)
				{
					num = 1.7f;
				}
				num2 = 1.45f * num;
			}
			tutorialKeywordTooltip2.transform.localScale = new Vector3(num, num, num);
			float num3 = -0.2f * this.m_actor.GetMeshRenderer().bounds.size.z;
			if (UniversalInputManager.UsePhoneUI && entity.GetZone() == TAG_ZONE.PLAY)
			{
				num3 += 1.5f;
			}
			if (i == 0)
			{
				if (showOnRight)
				{
					tutorialKeywordTooltip2.transform.position = this.m_actor.transform.position + new Vector3(this.m_actor.GetMeshRenderer().bounds.size.x * num2, 0f, this.m_actor.GetMeshRenderer().bounds.extents.z + num3);
				}
				else
				{
					tutorialKeywordTooltip2.transform.position = this.m_actor.transform.position + new Vector3(-this.m_actor.GetMeshRenderer().bounds.size.x * num2, 0f, this.m_actor.GetMeshRenderer().bounds.extents.z + num3);
				}
			}
			else
			{
				tutorialKeywordTooltip2.transform.position = tutorialKeywordTooltip.transform.position - new Vector3(0f, 0f, tutorialKeywordTooltip.GetHeight() * 0.35f + tutorialKeywordTooltip2.GetHeight() * 0.35f);
			}
			tutorialKeywordTooltip = tutorialKeywordTooltip2;
		}
		GameState.Get().GetGameEntity().NotifyOfHelpPanelDisplay(this.m_keywordPanels.Count);
	}

	// Token: 0x06004A67 RID: 19047 RVA: 0x00163EE8 File Offset: 0x001620E8
	private void PrepareToUpdateKeywordHelp(Actor actor)
	{
		this.HideKeywordHelp();
		this.m_actor = actor;
		this.m_keywordPanels = new List<TutorialKeywordTooltip>();
	}

	// Token: 0x06004A68 RID: 19048 RVA: 0x00163F04 File Offset: 0x00162104
	private void SetUpPanels(EntityBase entityInfo)
	{
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.TAUNT);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.STEALTH);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DIVINE_SHIELD);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPELLPOWER);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.ENRAGED);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CHARGE);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.BATTLECRY);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.FROZEN);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.FREEZE);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.WINDFURY);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SECRET);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DEATHRATTLE);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.OVERLOAD);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.COMBO);
	}

	// Token: 0x06004A69 RID: 19049 RVA: 0x00163FC8 File Offset: 0x001621C8
	private bool SetupKeywordPanelIfNecessary(EntityBase entityInfo, GAME_TAG tag)
	{
		if (entityInfo.HasTag(tag))
		{
			this.SetupKeywordPanel(tag);
			return true;
		}
		if (entityInfo.HasReferencedTag(tag))
		{
			this.SetupKeywordRefPanel(tag);
			return true;
		}
		return false;
	}

	// Token: 0x06004A6A RID: 19050 RVA: 0x00164000 File Offset: 0x00162200
	public void SetupKeywordPanel(GAME_TAG tag)
	{
		string keywordName = GameStrings.GetKeywordName(tag);
		string keywordText = GameStrings.GetKeywordText(tag);
		this.SetupKeywordPanel(keywordName, keywordText);
	}

	// Token: 0x06004A6B RID: 19051 RVA: 0x00164024 File Offset: 0x00162224
	public void SetupKeywordRefPanel(GAME_TAG tag)
	{
		string keywordName = GameStrings.GetKeywordName(tag);
		string refKeywordText = GameStrings.GetRefKeywordText(tag);
		this.SetupKeywordPanel(keywordName, refKeywordText);
	}

	// Token: 0x06004A6C RID: 19052 RVA: 0x00164048 File Offset: 0x00162248
	public void SetupKeywordPanel(string headline, string description)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.m_keywordPanelPrefab.gameObject);
		TutorialKeywordTooltip component = gameObject.GetComponent<TutorialKeywordTooltip>();
		if (component == null)
		{
			return;
		}
		component.Initialize(headline, description);
		this.m_keywordPanels.Add(component);
	}

	// Token: 0x06004A6D RID: 19053 RVA: 0x00164090 File Offset: 0x00162290
	public void HideKeywordHelp()
	{
		if (this.m_keywordPanels == null)
		{
			return;
		}
		foreach (TutorialKeywordTooltip tutorialKeywordTooltip in this.m_keywordPanels)
		{
			if (!(tutorialKeywordTooltip == null))
			{
				Object.Destroy(tutorialKeywordTooltip.gameObject);
			}
		}
	}

	// Token: 0x06004A6E RID: 19054 RVA: 0x0016410C File Offset: 0x0016230C
	public Card GetCard()
	{
		return this.m_card;
	}

	// Token: 0x06004A6F RID: 19055 RVA: 0x00164114 File Offset: 0x00162314
	public Vector3 GetPositionOfTopPanel()
	{
		if (this.m_keywordPanels == null || this.m_keywordPanels.Count == 0)
		{
			return new Vector3(0f, 0f, 0f);
		}
		return this.m_keywordPanels[0].transform.position;
	}

	// Token: 0x0400317D RID: 12669
	public TutorialKeywordTooltip m_keywordPanelPrefab;

	// Token: 0x0400317E RID: 12670
	private static TutorialKeywordManager s_instance;

	// Token: 0x0400317F RID: 12671
	private List<TutorialKeywordTooltip> m_keywordPanels;

	// Token: 0x04003180 RID: 12672
	private Actor m_actor;

	// Token: 0x04003181 RID: 12673
	private Card m_card;
}
