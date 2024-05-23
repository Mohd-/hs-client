using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005C1 RID: 1473
public class KeywordHelpPanelManager : MonoBehaviour
{
	// Token: 0x060041A3 RID: 16803 RVA: 0x0013C9B8 File Offset: 0x0013ABB8
	private void Awake()
	{
		KeywordHelpPanelManager.s_instance = this;
		this.m_keywordPanelPool.SetCreateItemCallback(new Pool<KeywordHelpPanel>.CreateItemCallback(this.CreateKeywordPanel));
		this.m_keywordPanelPool.SetDestroyItemCallback(new Pool<KeywordHelpPanel>.DestroyItemCallback(this.DestroyKeywordPanel));
		this.m_keywordPanelPool.SetExtensionCount(1);
		if (SceneMgr.Get() != null)
		{
			SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
		}
	}

	// Token: 0x060041A4 RID: 16804 RVA: 0x0013CA2B File Offset: 0x0013AC2B
	private void OnDestroy()
	{
		KeywordHelpPanelManager.s_instance = null;
	}

	// Token: 0x060041A5 RID: 16805 RVA: 0x0013CA33 File Offset: 0x0013AC33
	public static KeywordHelpPanelManager Get()
	{
		return KeywordHelpPanelManager.s_instance;
	}

	// Token: 0x060041A6 RID: 16806 RVA: 0x0013CA3C File Offset: 0x0013AC3C
	public void UpdateKeywordHelp(Card card, Actor actor, bool showOnRight = true, float? overrideScale = null, Vector3? overrideOffset = null)
	{
		this.m_card = card;
		this.UpdateKeywordHelp(card.GetEntity(), actor, showOnRight, overrideScale, overrideOffset);
	}

	// Token: 0x060041A7 RID: 16807 RVA: 0x0013CA64 File Offset: 0x0013AC64
	public void UpdateKeywordHelp(Entity entity, Actor actor, bool showOnRight, float? overrideScale = null, Vector3? overrideOffset = null)
	{
		this.m_card = entity.GetCard();
		if (GameState.Get().GetGameEntity().ShouldShowCrazyKeywordTooltip())
		{
			if (TutorialKeywordManager.Get() != null)
			{
				TutorialKeywordManager.Get().UpdateKeywordHelp(entity, actor, showOnRight, overrideScale);
			}
			return;
		}
		bool flag = this.m_card.GetZone() is ZoneHand;
		if (overrideScale != null)
		{
			this.scaleToUse = overrideScale.Value;
		}
		else if (flag)
		{
			this.scaleToUse = KeywordHelpPanel.HAND_SCALE;
		}
		else
		{
			this.scaleToUse = KeywordHelpPanel.GAMEPLAY_SCALE;
		}
		this.PrepareToUpdateKeywordHelp(actor);
		string[] array = GameState.Get().GetGameEntity().NotifyOfKeywordHelpPanelDisplay(entity);
		if (array != null)
		{
			this.SetupKeywordPanel(array[0], array[1]);
		}
		this.SetUpPanels(entity);
		base.StartCoroutine(this.PositionPanelsForGame(actor.GetMeshRenderer().gameObject, showOnRight, flag, overrideOffset));
		GameState.Get().GetGameEntity().NotifyOfHelpPanelDisplay(this.m_keywordPanels.Count);
	}

	// Token: 0x060041A8 RID: 16808 RVA: 0x0013CB78 File Offset: 0x0013AD78
	private IEnumerator PositionPanelsForGame(GameObject actorObject, bool showOnRight, bool inHand, Vector3? overrideOffset = null)
	{
		KeywordHelpPanel prevPanel = null;
		for (int i = 0; i < this.m_keywordPanels.Count; i++)
		{
			KeywordHelpPanel curPanel = this.m_keywordPanels[i];
			while (!curPanel.IsTextRendered())
			{
				yield return null;
			}
			if (i == 0)
			{
				if (overrideOffset != null)
				{
					if (showOnRight)
					{
						TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0f, 0f, 1f), actorObject, new Vector3(1f, 0f, 1f), overrideOffset.Value);
					}
					else
					{
						TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0f, 1f), actorObject, new Vector3(0f, 0f, 1f), overrideOffset.Value);
					}
				}
				else if (inHand)
				{
					if (showOnRight)
					{
						TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0f, 0f, 1f), actorObject, new Vector3(1f, 0f, 1f), Vector3.zero);
					}
					else
					{
						TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0f, 1f), actorObject, new Vector3(0f, 0f, 1f), new Vector3(-0.15f, 0f, 0f));
					}
				}
				else if (UniversalInputManager.UsePhoneUI)
				{
					if (showOnRight)
					{
						TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0f, 0f, 1f), actorObject, new Vector3(1f, 0f, 1f), new Vector3(1.5f, 0f, 2f));
					}
					else
					{
						TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0f, 1f), actorObject, new Vector3(0f, 0f, 1f), new Vector3(-1.8f, 0f, 2f));
					}
				}
				else if (showOnRight)
				{
					TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0f, 0f, 1f), actorObject, new Vector3(1f, 0f, 1f), new Vector3(0.5f * this.scaleToUse + 0.15f, 0f, 0.8f));
				}
				else
				{
					TransformUtil.SetPoint(curPanel.gameObject, new Vector3(1f, 0f, 1f), actorObject, new Vector3(0f, 0f, 1f), new Vector3(-0.78f * this.scaleToUse - 0.15f, 0f, 0.8f));
				}
			}
			else
			{
				TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0f, 0f, 1f), prevPanel.gameObject, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0.17f));
			}
			prevPanel = curPanel;
		}
		yield break;
	}

	// Token: 0x060041A9 RID: 16809 RVA: 0x0013CBD0 File Offset: 0x0013ADD0
	public void UpdateKeywordHelpForHistoryCard(Entity entity, Actor actor, UberText createdByText)
	{
		this.m_card = entity.GetCard();
		this.scaleToUse = KeywordHelpPanel.HISTORY_SCALE;
		this.PrepareToUpdateKeywordHelp(actor);
		string[] array = GameState.Get().GetGameEntity().NotifyOfKeywordHelpPanelDisplay(entity);
		if (array != null)
		{
			this.SetupKeywordPanel(array[0], array[1]);
		}
		this.SetUpPanels(entity);
		base.StartCoroutine(this.PositionPanelsForHistory(actor, createdByText));
	}

	// Token: 0x060041AA RID: 16810 RVA: 0x0013CC3C File Offset: 0x0013AE3C
	private IEnumerator PositionPanelsForHistory(Actor actor, UberText createdByText)
	{
		GameObject firstRelativeAnchor;
		if (createdByText.gameObject.activeSelf)
		{
			firstRelativeAnchor = createdByText.gameObject;
		}
		else
		{
			GameObject historyKeywordBone = actor.FindBone("HistoryKeywordBone");
			if (historyKeywordBone == null)
			{
				Error.AddDevWarning("Missing Bone", "Missing HistoryKeywordBone on {0}", new object[]
				{
					actor
				});
				yield return null;
			}
			firstRelativeAnchor = historyKeywordBone;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_keywordPanels.Clear();
		}
		KeywordHelpPanel prevPanel = null;
		bool showHorizontally = false;
		for (int i = 0; i < this.m_keywordPanels.Count; i++)
		{
			KeywordHelpPanel curPanel = this.m_keywordPanels[i];
			while (!curPanel.IsTextRendered())
			{
				yield return null;
			}
			if (i == 0)
			{
				TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.5f, 0f, 1f), firstRelativeAnchor, new Vector3(0.5f, 0f, 0f));
			}
			else
			{
				float newZ = prevPanel.GetHeight() * 0.35f + curPanel.GetHeight() * 0.35f;
				if (prevPanel.transform.position.z - newZ < -8.3f)
				{
					showHorizontally = true;
				}
				if (showHorizontally)
				{
					TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0f, 0f, 1f), prevPanel.gameObject, new Vector3(1f, 0f, 1f), Vector3.zero);
				}
				else
				{
					TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.5f, 0f, 1f), prevPanel.gameObject, new Vector3(0.5f, 0f, 0f), new Vector3(0f, 0f, 0.06094122f));
				}
			}
			prevPanel = curPanel;
		}
		yield break;
	}

	// Token: 0x060041AB RID: 16811 RVA: 0x0013CC74 File Offset: 0x0013AE74
	public void UpdateKeywordHelpForCollectionManager(EntityDef entityDef, Actor actor, bool reverse)
	{
		this.scaleToUse = KeywordHelpPanel.COLLECTION_MANAGER_SCALE;
		this.PrepareToUpdateKeywordHelp(actor);
		this.SetUpPanels(entityDef);
		base.StartCoroutine(this.PositionPanelsForCM(actor, reverse));
	}

	// Token: 0x060041AC RID: 16812 RVA: 0x0013CCB0 File Offset: 0x0013AEB0
	private IEnumerator PositionPanelsForCM(Actor actor, bool reverse)
	{
		GameObject actorObject = actor.GetMeshRenderer().gameObject;
		KeywordHelpPanel prevPanel = null;
		int maxPanelCount = this.m_keywordPanels.Count;
		if (UniversalInputManager.UsePhoneUI)
		{
			maxPanelCount = Mathf.Min(this.m_keywordPanels.Count, 3);
		}
		Vector3 actorStartAnchor;
		Vector3 panelStartAnchor;
		Vector3 panelEndAnchor;
		if (reverse)
		{
			actorStartAnchor = new Vector3(1f, 0f, 0f);
			panelStartAnchor = Vector3.zero;
			panelEndAnchor = new Vector3(0f, 0f, 1f);
		}
		else
		{
			actorStartAnchor = new Vector3(1f, 0f, 1f);
			panelStartAnchor = new Vector3(0f, 0f, 1f);
			panelEndAnchor = Vector3.zero;
		}
		for (int i = 0; i < this.m_keywordPanels.Count; i++)
		{
			KeywordHelpPanel panel = this.m_keywordPanels[i];
			if (i >= maxPanelCount)
			{
				panel.gameObject.SetActive(false);
			}
			else
			{
				while (!panel.IsTextRendered())
				{
					yield return null;
				}
				if (actor.IsSpellActive(SpellType.GHOSTCARD))
				{
					Spell ghostSpell = actor.GetSpell(SpellType.GHOSTCARD);
					if (ghostSpell != null)
					{
						RenderToTexture ghostR2T = ghostSpell.gameObject.GetComponentInChildren<RenderToTexture>();
						if (ghostR2T != null)
						{
							actorObject = ghostR2T.GetRenderToObject();
						}
					}
				}
				if (i == 0)
				{
					TransformUtil.SetPoint(panel.gameObject, panelStartAnchor, actorObject, actorStartAnchor, Vector3.zero);
					if (actor.isMissingCard())
					{
						RenderToTexture r2t = actor.m_missingCardEffect.GetComponent<RenderToTexture>();
						if (r2t != null)
						{
							Log.Kyle.Print("Missing card keyword tooltip offset: " + r2t.GetOffscreenPositionOffset().ToString(), new object[0]);
							panel.gameObject.transform.position -= r2t.GetOffscreenPositionOffset();
						}
					}
				}
				else
				{
					TransformUtil.SetPoint(panel.gameObject, panelStartAnchor, prevPanel.gameObject, panelEndAnchor, Vector3.zero);
				}
				prevPanel = panel;
			}
		}
		yield break;
	}

	// Token: 0x060041AD RID: 16813 RVA: 0x0013CCE8 File Offset: 0x0013AEE8
	public void UpdateKeywordHelpForDeckHelper(EntityDef entityDef, Actor actor)
	{
		this.scaleToUse = 3.75f;
		this.PrepareToUpdateKeywordHelp(actor);
		this.SetUpPanels(entityDef);
		base.StartCoroutine(this.PositionPanelsForForge(actor.GetMeshRenderer().gameObject, 0));
	}

	// Token: 0x060041AE RID: 16814 RVA: 0x0013CD28 File Offset: 0x0013AF28
	public void UpdateKeywordHelpForForge(EntityDef entityDef, Actor actor, int cardChoice = 0)
	{
		this.scaleToUse = KeywordHelpPanel.FORGE_SCALE;
		this.PrepareToUpdateKeywordHelp(actor);
		this.SetUpPanels(entityDef);
		base.StartCoroutine(this.PositionPanelsForForge(actor.GetMeshRenderer().gameObject, cardChoice));
	}

	// Token: 0x060041AF RID: 16815 RVA: 0x0013CD6C File Offset: 0x0013AF6C
	private IEnumerator PositionPanelsForForge(GameObject actorObject, int cardChoice = 0)
	{
		KeywordHelpPanel prevPanel = null;
		for (int i = 0; i < this.m_keywordPanels.Count; i++)
		{
			KeywordHelpPanel panel = this.m_keywordPanels[i];
			while (!panel.IsTextRendered())
			{
				yield return null;
			}
			if (i == 0)
			{
				if (UniversalInputManager.UsePhoneUI)
				{
					TransformUtil.SetPoint(panel.gameObject, new Vector3(0f, 0f, 1f), actorObject, (cardChoice != 3) ? new Vector3(1f, 0f, 1f) : new Vector3(0f, 0f, 1f), (cardChoice != 3) ? Vector3.zero : new Vector3(-31f, 0f, 0f));
				}
				else
				{
					TransformUtil.SetPoint(panel.gameObject, new Vector3(0f, 0f, 1f), actorObject, new Vector3(1f, 0f, 1f), Vector3.zero);
				}
			}
			else
			{
				TransformUtil.SetPoint(panel.gameObject, new Vector3(0f, 0f, 1f), prevPanel.gameObject, new Vector3(0f, 0f, 0f), Vector3.zero);
			}
			prevPanel = panel;
		}
		yield break;
	}

	// Token: 0x060041B0 RID: 16816 RVA: 0x0013CDA4 File Offset: 0x0013AFA4
	public void UpdateKeywordHelpForPackOpening(EntityDef entityDef, Actor actor)
	{
		this.scaleToUse = 2.75f;
		this.PrepareToUpdateKeywordHelp(actor);
		this.SetUpPanels(entityDef);
		base.StartCoroutine(this.PositionPanelsForPackOpening(actor.GetMeshRenderer().gameObject));
	}

	// Token: 0x060041B1 RID: 16817 RVA: 0x0013CDE4 File Offset: 0x0013AFE4
	private IEnumerator PositionPanelsForPackOpening(GameObject actorObject)
	{
		KeywordHelpPanel prevPanel = null;
		for (int i = 0; i < this.m_keywordPanels.Count; i++)
		{
			KeywordHelpPanel panel = this.m_keywordPanels[i];
			while (!panel.IsTextRendered())
			{
				yield return null;
			}
			if (i == 0)
			{
				TransformUtil.SetPoint(panel.gameObject, new Vector3(1f, 0f, 1f), actorObject, new Vector3(0f, 0f, 1f), Vector3.zero);
				panel.transform.position = panel.transform.position - new Vector3(1.2f, 0f, 0f);
			}
			else
			{
				TransformUtil.SetPoint(panel.gameObject, new Vector3(0f, 0f, 1f), prevPanel.gameObject, new Vector3(0f, 0f, 0f), Vector3.zero);
			}
			prevPanel = panel;
		}
		yield break;
	}

	// Token: 0x060041B2 RID: 16818 RVA: 0x0013CE10 File Offset: 0x0013B010
	public void UpdateKeywordHelpForMulliganCard(Entity entity, Actor actor)
	{
		this.m_card = entity.GetCard();
		this.scaleToUse = KeywordHelpPanel.MULLIGAN_SCALE;
		this.PrepareToUpdateKeywordHelp(actor);
		string[] array = GameState.Get().GetGameEntity().NotifyOfKeywordHelpPanelDisplay(entity);
		if (array != null)
		{
			this.SetupKeywordPanel(array[0], array[1]);
		}
		this.SetUpPanels(entity);
		base.StartCoroutine(this.PositionPanelsForMulligan(actor.GetMeshRenderer().gameObject));
	}

	// Token: 0x060041B3 RID: 16819 RVA: 0x0013CE84 File Offset: 0x0013B084
	private IEnumerator PositionPanelsForMulligan(GameObject actorObject)
	{
		KeywordHelpPanel prevPanel = null;
		bool showHorizontally = false;
		for (int i = 0; i < this.m_keywordPanels.Count; i++)
		{
			KeywordHelpPanel curPanel = this.m_keywordPanels[i];
			while (!curPanel.IsTextRendered())
			{
				yield return null;
			}
			if (i == 0)
			{
				TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.5f, 0f, 1f), actorObject, new Vector3(0.5f, 0f, 0f), new Vector3(-0.112071f, 0f, -0.1244259f));
			}
			else
			{
				float newZ = prevPanel.GetHeight() * 0.35f + curPanel.GetHeight() * 0.35f;
				if (prevPanel.transform.position.z - newZ < -8.3f)
				{
					showHorizontally = true;
				}
				if (showHorizontally)
				{
					TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0f, 0f, 1f), prevPanel.gameObject, new Vector3(1f, 0f, 1f), Vector3.zero);
				}
				else
				{
					TransformUtil.SetPoint(curPanel.gameObject, new Vector3(0.5f, 0f, 1f), prevPanel.gameObject, new Vector3(0.5f, 0f, 0f), new Vector3(0f, 0f, 0.1588802f));
				}
			}
			prevPanel = curPanel;
		}
		yield break;
	}

	// Token: 0x060041B4 RID: 16820 RVA: 0x0013CEAD File Offset: 0x0013B0AD
	private void PrepareToUpdateKeywordHelp(Actor actor)
	{
		this.HideKeywordHelp();
		this.m_actor = actor;
		this.m_keywordPanels.Clear();
	}

	// Token: 0x060041B5 RID: 16821 RVA: 0x0013CEC8 File Offset: 0x0013B0C8
	private void SetUpPanels(EntityBase entityInfo)
	{
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SHIFTING);
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
		if (entityInfo.GetZone() != TAG_ZONE.SECRET)
		{
			this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SECRET);
		}
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.DEATHRATTLE);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.OVERLOAD);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.COMBO);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SILENCE);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.COUNTER);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CANT_BE_DAMAGED);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.SPARE_PART);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.INSPIRE);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.TREASURE);
		this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.CTHUN);
		if (entityInfo.IsHeroPower())
		{
			this.SetupKeywordPanelIfNecessary(entityInfo, GAME_TAG.AI_MUST_PLAY);
		}
	}

	// Token: 0x060041B6 RID: 16822 RVA: 0x0013D018 File Offset: 0x0013B218
	private bool SetupKeywordPanelIfNecessary(EntityBase entityInfo, GAME_TAG tag)
	{
		if (entityInfo.HasTag(tag))
		{
			if (tag == GAME_TAG.WINDFURY && entityInfo.GetTag(tag) > 1)
			{
				return false;
			}
			this.SetupKeywordPanel(tag);
			return true;
		}
		else
		{
			if (entityInfo.HasReferencedTag(tag))
			{
				this.SetupKeywordRefPanel(tag);
				return true;
			}
			return false;
		}
	}

	// Token: 0x060041B7 RID: 16823 RVA: 0x0013D06C File Offset: 0x0013B26C
	private Vector3 GetPanelPosition(KeywordHelpPanel panel)
	{
		Vector3 result;
		result..ctor(0f, 0f, 0f);
		KeywordHelpPanel keywordHelpPanel = null;
		for (int i = 0; i < this.m_keywordPanels.Count; i++)
		{
			KeywordHelpPanel keywordHelpPanel2 = this.m_keywordPanels[i];
			float num;
			if (this.m_card.GetEntity().IsHero())
			{
				num = 1.2f;
			}
			else if (this.m_card.GetEntity().GetZone() == TAG_ZONE.PLAY)
			{
				num = 1.05f;
			}
			else
			{
				num = 0.85f;
			}
			if (this.m_actor.GetMeshRenderer() == null)
			{
				return result;
			}
			float num2 = -0.2f * this.m_actor.GetMeshRenderer().bounds.size.z;
			if (keywordHelpPanel2 == panel)
			{
				if (i == 0)
				{
					result = this.m_actor.transform.position + new Vector3(this.m_actor.GetMeshRenderer().bounds.size.x * num, 0f, this.m_actor.GetMeshRenderer().bounds.extents.z + num2);
				}
				else
				{
					result = keywordHelpPanel.transform.position - new Vector3(0f, 0f, keywordHelpPanel.GetHeight() * 0.35f + keywordHelpPanel2.GetHeight() * 0.35f);
				}
			}
			keywordHelpPanel = keywordHelpPanel2;
		}
		return result;
	}

	// Token: 0x060041B8 RID: 16824 RVA: 0x0013D214 File Offset: 0x0013B414
	private void SetupKeywordPanel(GAME_TAG tag)
	{
		string keywordName = GameStrings.GetKeywordName(tag);
		string textForTag = this.GetTextForTag(tag, GameStrings.GetKeywordTextKey(tag));
		this.SetupKeywordPanel(keywordName, textForTag);
	}

	// Token: 0x060041B9 RID: 16825 RVA: 0x0013D240 File Offset: 0x0013B440
	private void SetupKeywordRefPanel(GAME_TAG tag)
	{
		string keywordName = GameStrings.GetKeywordName(tag);
		string textForTag = this.GetTextForTag(tag, GameStrings.GetRefKeywordTextKey(tag));
		this.SetupKeywordPanel(keywordName, textForTag);
	}

	// Token: 0x060041BA RID: 16826 RVA: 0x0013D26C File Offset: 0x0013B46C
	private string GetTextForTag(GAME_TAG tag, string key)
	{
		if (tag == GAME_TAG.SPELLPOWER)
		{
			int num;
			if (this.m_card != null)
			{
				num = this.m_card.GetEntity().GetSpellPower();
			}
			else if (this.m_actor != null && this.m_actor.GetEntityDef() != null && this.m_actor.GetEntityDef().GetCardId() == "EX1_563")
			{
				num = 5;
			}
			else
			{
				num = 1;
			}
			return GameStrings.Format(key, new object[]
			{
				num
			});
		}
		return GameStrings.Get(key);
	}

	// Token: 0x060041BB RID: 16827 RVA: 0x0013D310 File Offset: 0x0013B510
	private void SetupKeywordPanel(string headline, string description)
	{
		KeywordHelpPanel keywordHelpPanel = this.m_keywordPanelPool.Acquire();
		if (keywordHelpPanel == null)
		{
			return;
		}
		keywordHelpPanel.Reset();
		keywordHelpPanel.Initialize(headline, description);
		keywordHelpPanel.SetScale(this.scaleToUse);
		this.m_keywordPanels.Add(keywordHelpPanel);
		this.FadeInPanel(keywordHelpPanel);
	}

	// Token: 0x060041BC RID: 16828 RVA: 0x0013D364 File Offset: 0x0013B564
	private void FadeInPanel(KeywordHelpPanel helpPanel)
	{
		this.CleanTweensOnPanel(helpPanel);
		float num = 0.4f;
		if (GameState.Get() != null && GameState.Get().GetGameEntity().IsKeywordHelpDelayOverridden())
		{
			num = 0f;
		}
		iTween.ValueTo(base.gameObject, iTween.Hash(new object[]
		{
			"onupdatetarget",
			base.gameObject,
			"onupdate",
			"OnUberTextFadeUpdate",
			"time",
			0.125f,
			"delay",
			num,
			"to",
			1f,
			"from",
			0f
		}));
	}

	// Token: 0x060041BD RID: 16829 RVA: 0x0013D430 File Offset: 0x0013B630
	private void OnUberTextFadeUpdate(float newValue)
	{
		foreach (KeywordHelpPanel keywordHelpPanel in this.m_keywordPanels)
		{
			RenderUtils.SetAlpha(keywordHelpPanel.gameObject, newValue, true);
		}
	}

	// Token: 0x060041BE RID: 16830 RVA: 0x0013D490 File Offset: 0x0013B690
	private void CleanTweensOnPanel(KeywordHelpPanel helpPanel)
	{
		iTween.Stop(base.gameObject);
		RenderUtils.SetAlpha(helpPanel.gameObject, 0f, true);
	}

	// Token: 0x060041BF RID: 16831 RVA: 0x0013D4BC File Offset: 0x0013B6BC
	public void ShowKeywordHelp()
	{
		foreach (KeywordHelpPanel keywordHelpPanel in this.m_keywordPanels)
		{
			keywordHelpPanel.gameObject.SetActive(true);
		}
	}

	// Token: 0x060041C0 RID: 16832 RVA: 0x0013D51C File Offset: 0x0013B71C
	public void HideKeywordHelp()
	{
		GameState gameState = GameState.Get();
		if (gameState != null && gameState.GetGameEntity().ShouldShowCrazyKeywordTooltip() && TutorialKeywordManager.Get() != null)
		{
			TutorialKeywordManager.Get().HideKeywordHelp();
		}
		foreach (KeywordHelpPanel keywordHelpPanel in this.m_keywordPanels)
		{
			if (!(keywordHelpPanel == null))
			{
				this.CleanTweensOnPanel(keywordHelpPanel);
				keywordHelpPanel.gameObject.SetActive(false);
				this.m_keywordPanelPool.Release(keywordHelpPanel);
			}
		}
	}

	// Token: 0x060041C1 RID: 16833 RVA: 0x0013D5D8 File Offset: 0x0013B7D8
	public Card GetCard()
	{
		return this.m_card;
	}

	// Token: 0x060041C2 RID: 16834 RVA: 0x0013D5E0 File Offset: 0x0013B7E0
	public Vector3 GetPositionOfTopPanel()
	{
		if (this.m_keywordPanels.Count == 0)
		{
			return new Vector3(0f, 0f, 0f);
		}
		return this.m_keywordPanels[0].transform.position;
	}

	// Token: 0x060041C3 RID: 16835 RVA: 0x0013D628 File Offset: 0x0013B828
	public KeywordHelpPanel CreateKeywordPanel(int i)
	{
		return Object.Instantiate<KeywordHelpPanel>(this.m_keywordPanelPrefab);
	}

	// Token: 0x060041C4 RID: 16836 RVA: 0x0013D635 File Offset: 0x0013B835
	private void DestroyKeywordPanel(KeywordHelpPanel panel)
	{
		Object.Destroy(panel.gameObject);
	}

	// Token: 0x060041C5 RID: 16837 RVA: 0x0013D644 File Offset: 0x0013B844
	private void OnSceneUnloaded(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		foreach (KeywordHelpPanel keywordHelpPanel in this.m_keywordPanels)
		{
			Object.Destroy(keywordHelpPanel.gameObject);
		}
		this.m_keywordPanels.Clear();
		this.m_keywordPanelPool.Clear();
		Object.Destroy(this.m_actor);
		this.m_actor = null;
		Object.Destroy(this.m_card);
		this.m_card = null;
	}

	// Token: 0x040029D0 RID: 10704
	private const float FADE_IN_TIME = 0.125f;

	// Token: 0x040029D1 RID: 10705
	private const float DELAY_BEFORE_FADE_IN = 0.4f;

	// Token: 0x040029D2 RID: 10706
	public KeywordHelpPanel m_keywordPanelPrefab;

	// Token: 0x040029D3 RID: 10707
	private static KeywordHelpPanelManager s_instance;

	// Token: 0x040029D4 RID: 10708
	private Pool<KeywordHelpPanel> m_keywordPanelPool = new Pool<KeywordHelpPanel>();

	// Token: 0x040029D5 RID: 10709
	private List<KeywordHelpPanel> m_keywordPanels = new List<KeywordHelpPanel>();

	// Token: 0x040029D6 RID: 10710
	private Actor m_actor;

	// Token: 0x040029D7 RID: 10711
	private Card m_card;

	// Token: 0x040029D8 RID: 10712
	private float scaleToUse = KeywordHelpPanel.GAMEPLAY_SCALE;
}
