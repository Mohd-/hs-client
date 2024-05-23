using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000735 RID: 1845
public class DeckHelper : MonoBehaviour
{
	// Token: 0x06004B17 RID: 19223 RVA: 0x00166A98 File Offset: 0x00164C98
	private void Awake()
	{
		DeckHelper.s_instance = this;
		this.m_rootObject.SetActive(false);
		this.m_replaceDoneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.EndButtonClick));
		this.m_suggestDoneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.EndButtonClick));
		if (UniversalInputManager.UsePhoneUI)
		{
			if (this.m_innkeeperPopup != null)
			{
				this.m_innkeeperFullScale = this.m_innkeeperPopup.gameObject.transform.localScale;
				this.m_innkeeperPopup.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.InnkeeperPopupClicked));
			}
		}
		else
		{
			this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.EndButtonClick));
		}
	}

	// Token: 0x06004B18 RID: 19224 RVA: 0x00166B5B File Offset: 0x00164D5B
	private void OnDestroy()
	{
		DeckHelper.s_instance = null;
	}

	// Token: 0x06004B19 RID: 19225 RVA: 0x00166B63 File Offset: 0x00164D63
	private void EndButtonClick(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x06004B1A RID: 19226 RVA: 0x00166B6C File Offset: 0x00164D6C
	public static DeckHelper Get()
	{
		if (DeckHelper.s_instance == null)
		{
			string name = (!UniversalInputManager.UsePhoneUI) ? "DeckHelper" : "DeckHelper_phone";
			DeckHelper.s_instance = AssetLoader.Get().LoadGameObject(name, true, false).GetComponent<DeckHelper>();
		}
		return DeckHelper.s_instance;
	}

	// Token: 0x06004B1B RID: 19227 RVA: 0x00166BC4 File Offset: 0x00164DC4
	public bool IsActive()
	{
		return this.m_shown;
	}

	// Token: 0x06004B1C RID: 19228 RVA: 0x00166BCC File Offset: 0x00164DCC
	public void RegisterStateChangedListener(DeckHelper.DelStateChangedListener listener)
	{
		if (this.m_listeners.Contains(listener))
		{
			return;
		}
		this.m_listeners.Add(listener);
	}

	// Token: 0x06004B1D RID: 19229 RVA: 0x00166BEC File Offset: 0x00164DEC
	public void RemoveStateChangedListener(DeckHelper.DelStateChangedListener listener)
	{
		this.m_listeners.Remove(listener);
	}

	// Token: 0x06004B1E RID: 19230 RVA: 0x00166BFC File Offset: 0x00164DFC
	public void UpdateChoices()
	{
		this.CleanOldChoices();
		if (!this.IsActive())
		{
			return;
		}
		EntityDef entityDef = this.m_cardToRemove;
		this.m_cardToRemove = null;
		CollectionDeck taggedDeck = CollectionManager.Get().GetTaggedDeck(CollectionManager.DeckTag.Editing);
		DeckMaker.DeckChoiceFill cardsToShow = DeckMaker.GetFillCardChoices(taggedDeck, entityDef, 3, null);
		if (entityDef == null && cardsToShow.m_removeTemplate != null)
		{
			entityDef = cardsToShow.m_removeTemplate;
		}
		string reason = cardsToShow.m_reason;
		if (cardsToShow == null || cardsToShow.m_addChoices.Count == 0)
		{
			Debug.LogError("DeckHelper.GetChoices() - Can't find choices!!!!");
			return;
		}
		if (this.m_instructionText != null)
		{
			bool flag = !this.m_instructionText.Text.Equals(reason);
			this.m_instructionText.Text = reason;
			if (UniversalInputManager.UsePhoneUI && flag)
			{
				if (NotificationManager.Get().IsQuotePlaying)
				{
					this.m_instructionText.Text = string.Empty;
				}
				else
				{
					this.ShowInnkeeperPopup();
				}
			}
		}
		this.m_replaceACardPane.SetActive(entityDef != null);
		this.m_suggestACardPane.SetActive(entityDef == null);
		if (entityDef != null)
		{
			if (this.m_tileToRemove != null)
			{
				this.m_tileToRemove.SetHighlight(false);
			}
			this.m_tileToRemove = CollectionDeckTray.Get().GetCardTileVisual(entityDef.GetCardId());
			GhostCard.Type ghostTypeFromSlot = GhostCard.GetGhostTypeFromSlot(taggedDeck, this.m_tileToRemove.GetSlot());
			this.m_replaceCardActor = this.LoadBestCardActor(entityDef, TAG_PREMIUM.NORMAL, ghostTypeFromSlot);
			if (this.m_replaceCardActor != null)
			{
				GameUtils.SetParent(this.m_replaceCardActor, this.m_replaceContainer, false);
			}
			if (this.m_replaceText != null)
			{
				this.m_replaceText.Text = ((ghostTypeFromSlot != GhostCard.Type.NOT_VALID) ? GameStrings.Get("GLUE_COLLECTION_DECK_HELPER_REPLACE_CARD") : GameStrings.Get("GLUE_COLLECTION_DECK_HELPER_REPLACE_INVALID_CARD"));
			}
			if (this.m_tileToRemove.GetSlot().Owned && !Options.Get().GetBool(Option.HAS_SEEN_DECK_TEMPLATE_GHOST_CARD, false))
			{
				Options.Get().SetBool(Option.HAS_SEEN_DECK_TEMPLATE_GHOST_CARD, true);
			}
			if (!taggedDeck.IsValidSlot(this.m_tileToRemove.GetSlot()) && !Options.Get().GetBool(Option.HAS_SEEN_INVALID_ROTATED_CARD, false))
			{
				Options.Get().SetBool(Option.HAS_SEEN_INVALID_ROTATED_CARD, true);
			}
		}
		bool flag2 = entityDef != null;
		int num = Mathf.Min((!flag2) ? 3 : 2, cardsToShow.m_addChoices.Count);
		GameObject parent = (!flag2) ? this.m_3choiceContainer : this.m_2choiceContainer;
		for (int i = 0; i < num; i++)
		{
			EntityDef entityDef2 = cardsToShow.m_addChoices[i];
			TAG_PREMIUM premiumToUse = (!taggedDeck.CanAddOwnedCard(entityDef2.GetCardId(), TAG_PREMIUM.GOLDEN)) ? TAG_PREMIUM.NORMAL : TAG_PREMIUM.GOLDEN;
			Actor actor = this.LoadBestCardActor(entityDef2, premiumToUse, GhostCard.Type.NONE);
			if (!(actor == null))
			{
				GameUtils.SetParent(actor, parent, false);
				PegUIElement pegUIElement = actor.GetCollider().gameObject.AddComponent<PegUIElement>();
				pegUIElement.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
				{
					this.OnVisualRelease(actor, cardsToShow.m_removeTemplate);
				});
				pegUIElement.AddEventListener(UIEventType.ROLLOVER, delegate(UIEvent e)
				{
					this.OnVisualOver(actor);
				});
				pegUIElement.AddEventListener(UIEventType.ROLLOUT, delegate(UIEvent e)
				{
					this.OnVisualOut(actor);
				});
				this.m_choiceActors.Add(actor);
			}
		}
		this.PositionAndShowChoices();
	}

	// Token: 0x06004B1F RID: 19231 RVA: 0x00166FC8 File Offset: 0x001651C8
	private Actor LoadBestCardActor(EntityDef entityDef, TAG_PREMIUM premiumToUse, GhostCard.Type ghostCard = GhostCard.Type.NONE)
	{
		CardDef cardDef = DefLoader.Get().GetCardDef(entityDef.GetCardId(), new CardPortraitQuality(3, premiumToUse));
		GameObject gameObject = AssetLoader.Get().LoadActor(ActorNames.GetHandActor(entityDef, premiumToUse), false, false);
		if (gameObject == null)
		{
			Debug.LogWarning(string.Format("DeckHelper - FAILED to load actor \"{0}\"", base.name));
			return null;
		}
		Actor component = gameObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("DeckHelper - ERROR actor \"{0}\" has no Actor component", base.name));
			return null;
		}
		component.transform.parent = base.transform;
		SceneUtils.SetLayer(component, base.gameObject.layer);
		component.SetEntityDef(entityDef);
		component.SetCardDef(cardDef);
		component.SetPremium(premiumToUse);
		component.GhostCardEffect(ghostCard);
		component.UpdateAllComponents();
		component.Hide();
		component.gameObject.name = cardDef.name + "_actor";
		return component;
	}

	// Token: 0x06004B20 RID: 19232 RVA: 0x001670B4 File Offset: 0x001652B4
	private void CleanOldChoices()
	{
		foreach (Actor actor in this.m_choiceActors)
		{
			Object.Destroy(actor.gameObject);
		}
		this.m_choiceActors.Clear();
		if (this.m_replaceCardActor != null)
		{
			Object.Destroy(this.m_replaceCardActor.gameObject);
			this.m_replaceCardActor = null;
		}
	}

	// Token: 0x06004B21 RID: 19233 RVA: 0x00167148 File Offset: 0x00165348
	private void PositionAndShowChoices()
	{
		for (int i = 0; i < this.m_choiceActors.Count; i++)
		{
			Actor actor = this.m_choiceActors[i];
			actor.transform.localPosition = this.m_cardSpacing * (float)i;
			actor.Show();
			CollectionCardVisual.ShowActorShadow(actor, true);
		}
		if (this.m_replaceCardActor != null)
		{
			this.m_replaceCardActor.Show();
		}
		if (this.m_tileToRemove != null)
		{
			this.m_tileToRemove.SetHighlight(true);
		}
		base.StartCoroutine(this.WaitAndAnimateChoices());
	}

	// Token: 0x06004B22 RID: 19234 RVA: 0x001671EC File Offset: 0x001653EC
	private IEnumerator WaitAndAnimateChoices()
	{
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < this.m_choiceActors.Count; i++)
		{
			if (this.m_choiceActors[i].isActiveAndEnabled)
			{
				this.m_choiceActors[i].ActivateSpell(SpellType.SUMMON_IN_FORGE);
			}
		}
		if (this.m_replaceCardActor != null && this.m_replaceContainer.activeInHierarchy)
		{
			this.m_replaceCardActor.ActivateSpell(SpellType.SUMMON_IN_FORGE);
		}
		yield break;
	}

	// Token: 0x06004B23 RID: 19235 RVA: 0x00167208 File Offset: 0x00165408
	private void FireStateChangedEvent()
	{
		DeckHelper.DelStateChangedListener[] array = this.m_listeners.ToArray();
		foreach (DeckHelper.DelStateChangedListener delStateChangedListener in array)
		{
			delStateChangedListener(this.m_shown);
		}
	}

	// Token: 0x06004B24 RID: 19236 RVA: 0x00167248 File Offset: 0x00165448
	public void Show(DeckTrayDeckTileVisual tileToRemove, bool continueAfterReplace, bool replacingCard = false)
	{
		if (this.m_shown)
		{
			return;
		}
		Navigation.PushUnique(new Navigation.NavigateBackHandler(this.OnNavigateBack));
		SoundManager.Get().LoadAndPlay("bar_button_A_press", base.gameObject);
		this.m_shown = true;
		this.m_rootObject.SetActive(true);
		if (!Options.Get().GetBool(Option.HAS_SEEN_DECK_HELPER, false) && UserAttentionManager.CanShowAttentionGrabber("DeckHelper.Show:" + Option.HAS_SEEN_DECK_HELPER))
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_ANNOUNCER_CM_HELP_DECK_50"), "VO_ANNOUNCER_CM_HELP_DECK_50", 0f, null);
			Options.Get().SetBool(Option.HAS_SEEN_DECK_HELPER, true);
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			FullScreenFXMgr.Get().StartStandardBlurVignette(0.1f);
		}
		this.m_tileToRemove = tileToRemove;
		if (this.m_tileToRemove != null)
		{
			this.m_cardToRemove = tileToRemove.GetActor().GetEntityDef();
		}
		this.m_continueAfterReplace = continueAfterReplace;
		this.FireStateChangedEvent();
		this.UpdateChoices();
		NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_1"), 0f);
		NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_TEMPLATE_REPLACE_2"), 0f);
		NotificationManager.Get().DestroyNotificationWithText(GameStrings.Get("GLUE_COLLECTION_TUTORIAL_REPLACE_WILD_CARDS"), 0f);
	}

	// Token: 0x06004B25 RID: 19237 RVA: 0x0016739A File Offset: 0x0016559A
	private bool OnNavigateBack()
	{
		this.Hide(false);
		return true;
	}

	// Token: 0x06004B26 RID: 19238 RVA: 0x001673A4 File Offset: 0x001655A4
	public void Hide(bool popnavigation = true)
	{
		if (!this.m_shown)
		{
			return;
		}
		if (popnavigation)
		{
			Navigation.PopUnique(new Navigation.NavigateBackHandler(this.OnNavigateBack));
		}
		this.m_shown = false;
		this.CleanOldChoices();
		this.m_rootObject.SetActive(false);
		if (this.m_tileToRemove != null)
		{
			this.m_tileToRemove.SetHighlight(false);
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			FullScreenFXMgr.Get().EndStandardBlurVignette(0.1f, null);
		}
		this.FireStateChangedEvent();
	}

	// Token: 0x06004B27 RID: 19239 RVA: 0x00167430 File Offset: 0x00165630
	private void ShowInnkeeperPopup()
	{
		if (this.m_innkeeperPopup == null)
		{
			return;
		}
		this.m_innkeeperPopup.gameObject.SetActive(true);
		this.m_innkeeperPopupShown = true;
		this.m_innkeeperPopup.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		iTween.ScaleTo(this.m_innkeeperPopup.gameObject, iTween.Hash(new object[]
		{
			"scale",
			this.m_innkeeperFullScale,
			"easetype",
			iTween.EaseType.easeOutElastic,
			"time",
			1f
		}));
		base.StopCoroutine("WaitThenHidePopup");
		base.StartCoroutine("WaitThenHidePopup");
	}

	// Token: 0x06004B28 RID: 19240 RVA: 0x00167500 File Offset: 0x00165700
	private IEnumerator WaitThenHidePopup()
	{
		yield return new WaitForSeconds(7f);
		this.HideInnkeeperPopup();
		yield break;
	}

	// Token: 0x06004B29 RID: 19241 RVA: 0x0016751B File Offset: 0x0016571B
	private void InnkeeperPopupClicked(UIEvent e)
	{
		this.HideInnkeeperPopup();
	}

	// Token: 0x06004B2A RID: 19242 RVA: 0x00167524 File Offset: 0x00165724
	private void HideInnkeeperPopup()
	{
		if (this.m_innkeeperPopup == null || !this.m_innkeeperPopupShown)
		{
			return;
		}
		this.m_innkeeperPopupShown = false;
		iTween.ScaleTo(this.m_innkeeperPopup.gameObject, iTween.Hash(new object[]
		{
			"scale",
			new Vector3(0.01f, 0.01f, 0.01f),
			"easetype",
			iTween.EaseType.easeInExpo,
			"time",
			0.2f,
			"oncomplete",
			"FinishHidePopup",
			"oncompletetarget",
			base.gameObject
		}));
	}

	// Token: 0x06004B2B RID: 19243 RVA: 0x001675DE File Offset: 0x001657DE
	private void FinishHidePopup()
	{
		this.m_innkeeperPopup.gameObject.SetActive(false);
	}

	// Token: 0x06004B2C RID: 19244 RVA: 0x001675F4 File Offset: 0x001657F4
	public void OnVisualRelease(Actor addCardActor, EntityDef cardToRemove)
	{
		KeywordHelpPanelManager.Get().HideKeywordHelp();
		addCardActor.GetSpell(SpellType.DEATHREVERSE).ActivateState(SpellStateType.BIRTH);
		bool flag = cardToRemove != null;
		bool flag2 = this.m_continueAfterReplace;
		CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
		CollectionDeck editingDeck = collectionDeckTray.GetCardsContent().GetEditingDeck();
		if (flag)
		{
			int num;
			int num2;
			CollectionManager.Get().GetOwnedCardCount(addCardActor.GetEntityDef().GetCardId(), out num, out num2);
			int num3 = (!cardToRemove.IsElite()) ? 2 : 1;
			int num4 = (!this.m_ReplaceSingleTemplateCard) ? num3 : 1;
			int invalidCardIdCount = editingDeck.GetInvalidCardIdCount(cardToRemove.GetCardId());
			Log.DeckHelper.Print(string.Concat(new object[]
			{
				"checking invalid card ",
				editingDeck.IsWild,
				" ",
				invalidCardIdCount,
				" ",
				cardToRemove
			}), new object[0]);
			int sameRemoveCount = Mathf.Min(num4, num + num2);
			int num5 = collectionDeckTray.RemoveClosestInvalidCard(cardToRemove, sameRemoveCount);
			Log.DeckHelper.Print("removed cards " + num5, new object[0]);
			int num6 = 0;
			for (int i = 0; i < num5; i++)
			{
				TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
				if (num2 > 0)
				{
					num2--;
					premium = TAG_PREMIUM.GOLDEN;
				}
				else if (num == 0)
				{
					break;
				}
				if (collectionDeckTray.AddCard(addCardActor.GetEntityDef(), premium, null, false, addCardActor))
				{
					num6++;
				}
			}
			Log.DeckHelper.Print(string.Concat(new object[]
			{
				"did replace ",
				num6,
				" ",
				invalidCardIdCount
			}), new object[0]);
			if (num6 < invalidCardIdCount)
			{
				this.m_cardToRemove = cardToRemove;
				flag2 = true;
			}
		}
		else
		{
			collectionDeckTray.AddCard(addCardActor.GetEntityDef(), addCardActor.GetPremium(), null, false, addCardActor);
		}
		if (flag2)
		{
			this.UpdateChoices();
		}
		else
		{
			this.Hide(true);
		}
	}

	// Token: 0x06004B2D RID: 19245 RVA: 0x001677F8 File Offset: 0x001659F8
	private void OnVisualOver(Actor actor)
	{
		SoundManager.Get().LoadAndPlay("collection_manager_card_mouse_over");
		actor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
		KeywordHelpPanelManager.Get().UpdateKeywordHelpForDeckHelper(actor.GetEntityDef(), actor);
	}

	// Token: 0x06004B2E RID: 19246 RVA: 0x0016782C File Offset: 0x00165A2C
	private void OnVisualOut(Actor actor)
	{
		actor.SetActorState(ActorStateType.CARD_IDLE);
		KeywordHelpPanelManager.Get().HideKeywordHelp();
	}

	// Token: 0x040031ED RID: 12781
	private const float INNKEEPER_POPUP_DURATION = 7f;

	// Token: 0x040031EE RID: 12782
	public UberText m_instructionText;

	// Token: 0x040031EF RID: 12783
	public UberText m_replaceText;

	// Token: 0x040031F0 RID: 12784
	public GameObject m_rootObject;

	// Token: 0x040031F1 RID: 12785
	public UIBButton m_suggestDoneButton;

	// Token: 0x040031F2 RID: 12786
	public UIBButton m_replaceDoneButton;

	// Token: 0x040031F3 RID: 12787
	public PegUIElement m_inputBlocker;

	// Token: 0x040031F4 RID: 12788
	public Vector3 m_deckCardLocalScale = new Vector3(5.75f, 5.75f, 5.75f);

	// Token: 0x040031F5 RID: 12789
	public GameObject m_3choiceContainer;

	// Token: 0x040031F6 RID: 12790
	public GameObject m_replaceContainer;

	// Token: 0x040031F7 RID: 12791
	public GameObject m_2choiceContainer;

	// Token: 0x040031F8 RID: 12792
	public Vector3 m_cardSpacing;

	// Token: 0x040031F9 RID: 12793
	public GameObject m_suggestACardPane;

	// Token: 0x040031FA RID: 12794
	public GameObject m_replaceACardPane;

	// Token: 0x040031FB RID: 12795
	public UIBButton m_innkeeperPopup;

	// Token: 0x040031FC RID: 12796
	private static DeckHelper s_instance;

	// Token: 0x040031FD RID: 12797
	private List<DeckHelper.DelStateChangedListener> m_listeners = new List<DeckHelper.DelStateChangedListener>();

	// Token: 0x040031FE RID: 12798
	private Actor m_replaceCardActor;

	// Token: 0x040031FF RID: 12799
	private List<Actor> m_choiceActors = new List<Actor>();

	// Token: 0x04003200 RID: 12800
	private bool m_shown;

	// Token: 0x04003201 RID: 12801
	private DeckTrayDeckTileVisual m_tileToRemove;

	// Token: 0x04003202 RID: 12802
	private EntityDef m_cardToRemove;

	// Token: 0x04003203 RID: 12803
	private bool m_continueAfterReplace;

	// Token: 0x04003204 RID: 12804
	private bool m_ReplaceSingleTemplateCard = true;

	// Token: 0x04003205 RID: 12805
	private Vector3 m_innkeeperFullScale;

	// Token: 0x04003206 RID: 12806
	private bool m_innkeeperPopupShown;

	// Token: 0x02000778 RID: 1912
	// (Invoke) Token: 0x06004C4B RID: 19531
	public delegate void DelStateChangedListener(bool isActive);
}
