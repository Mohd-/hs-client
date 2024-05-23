using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004E9 RID: 1257
[CustomEditClass]
public class CraftingManager : MonoBehaviour
{
	// Token: 0x06003AEE RID: 15086 RVA: 0x0011C90C File Offset: 0x0011AB0C
	private void Awake()
	{
		this.m_arcaneDustBalance = NetCache.Get().GetNetObject<NetCache.NetCacheArcaneDustBalance>().Balance;
		CollectionManager.Get().RegisterMassDisenchantListener(new CollectionManager.OnMassDisenchant(this.OnMassDisenchant));
	}

	// Token: 0x06003AEF RID: 15087 RVA: 0x0011C944 File Offset: 0x0011AB44
	private void OnDestroy()
	{
		if (CollectionManager.Get() != null)
		{
			CollectionManager.Get().RemoveMassDisenchantListener(new CollectionManager.OnMassDisenchant(this.OnMassDisenchant));
		}
		CraftingManager.s_instance = null;
	}

	// Token: 0x06003AF0 RID: 15088 RVA: 0x0011C978 File Offset: 0x0011AB78
	private void Start()
	{
		this.LoadActor("Card_Hand_Weapon", ref this.m_ghostWeaponActor, ref this.m_templateWeaponActor);
		this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.WEAPON, TAG_PREMIUM.GOLDEN), ref this.m_ghostGoldenWeaponActor, ref this.m_templateGoldenWeaponActor);
		this.LoadActor("Card_Hand_Ally", ref this.m_ghostMinionActor, ref this.m_templateMinionActor);
		this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.MINION, TAG_PREMIUM.GOLDEN), ref this.m_ghostGoldenMinionActor, ref this.m_templateGoldenMinionActor);
		this.LoadActor("Card_Hand_Ability", ref this.m_ghostSpellActor, ref this.m_templateSpellActor);
		this.LoadActor(ActorNames.GetHandActor(TAG_CARDTYPE.SPELL, TAG_PREMIUM.GOLDEN), ref this.m_ghostGoldenSpellActor, ref this.m_templateGoldenSpellActor);
		this.LoadActor("Card_Hero_Skin", ref this.m_templateHeroSkinActor);
		this.LoadActor("Card_Hidden", ref this.m_hiddenActor);
		this.m_hiddenActor.GetMeshRenderer().transform.localEulerAngles = new Vector3(0f, 180f, 180f);
		SceneUtils.SetLayer(this.m_hiddenActor.gameObject, GameLayer.IgnoreFullScreenEffects);
		SoundManager.Get().Load("Card_Transition_Out");
		SoundManager.Get().Load("Card_Transition_In");
	}

	// Token: 0x06003AF1 RID: 15089 RVA: 0x0011CA94 File Offset: 0x0011AC94
	public static CraftingManager Get()
	{
		if (CraftingManager.s_instance == null)
		{
			string name = (!UniversalInputManager.UsePhoneUI) ? "CraftingManager" : "CraftingManager_phone";
			CraftingManager.s_instance = AssetLoader.Get().LoadGameObject(name, true, false).GetComponent<CraftingManager>();
		}
		return CraftingManager.s_instance;
	}

	// Token: 0x06003AF2 RID: 15090 RVA: 0x0011CAEC File Offset: 0x0011ACEC
	public NetCache.CardValue GetCardValue(string cardID, TAG_PREMIUM premium)
	{
		NetCache.NetCacheCardValues netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCardValues>();
		NetCache.CardDefinition key = new NetCache.CardDefinition
		{
			Name = cardID,
			Premium = premium
		};
		NetCache.CardValue result;
		if (!netObject.Values.TryGetValue(key, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x06003AF3 RID: 15091 RVA: 0x0011CB30 File Offset: 0x0011AD30
	public bool IsCardShowing()
	{
		return this.m_currentBigActor != null;
	}

	// Token: 0x06003AF4 RID: 15092 RVA: 0x0011CB3E File Offset: 0x0011AD3E
	public bool GetShownCardInfo(out EntityDef entityDef, out TAG_PREMIUM premium)
	{
		entityDef = null;
		premium = TAG_PREMIUM.NORMAL;
		if (this.m_currentBigActor == null)
		{
			return false;
		}
		entityDef = this.m_currentBigActor.GetEntityDef();
		premium = this.m_currentBigActor.GetPremium();
		return entityDef != null;
	}

	// Token: 0x06003AF5 RID: 15093 RVA: 0x0011CB7D File Offset: 0x0011AD7D
	public Actor GetShownActor()
	{
		return this.m_currentBigActor;
	}

	// Token: 0x06003AF6 RID: 15094 RVA: 0x0011CB85 File Offset: 0x0011AD85
	public void OnMassDisenchant(int amount)
	{
		if (!MassDisenchant.Get())
		{
			this.AdjustLocalArcaneDustBalance(amount);
			this.m_craftingUI.UpdateBankText();
		}
	}

	// Token: 0x06003AF7 RID: 15095 RVA: 0x0011CBA8 File Offset: 0x0011ADA8
	public long GetLocalArcaneDustBalance()
	{
		return this.m_arcaneDustBalance;
	}

	// Token: 0x06003AF8 RID: 15096 RVA: 0x0011CBB0 File Offset: 0x0011ADB0
	public void AdjustLocalArcaneDustBalance(int amt)
	{
		this.m_arcaneDustBalance += (long)amt;
	}

	// Token: 0x06003AF9 RID: 15097 RVA: 0x0011CBC1 File Offset: 0x0011ADC1
	public int GetNumTransactions()
	{
		return this.m_transactions;
	}

	// Token: 0x06003AFA RID: 15098 RVA: 0x0011CBC9 File Offset: 0x0011ADC9
	public void NotifyOfTransaction(int amt)
	{
		this.m_transactions += amt;
	}

	// Token: 0x06003AFB RID: 15099 RVA: 0x0011CBD9 File Offset: 0x0011ADD9
	public bool IsCancelling()
	{
		return this.m_cancellingCraftMode;
	}

	// Token: 0x06003AFC RID: 15100 RVA: 0x0011CBE4 File Offset: 0x0011ADE4
	public void EnterCraftMode(Actor cardActor)
	{
		if (this.m_cancellingCraftMode || CollectionDeckTray.Get().IsWaitingToDeleteDeck())
		{
			return;
		}
		if (this.m_upsideDownActor != null)
		{
			Object.Destroy(this.m_upsideDownActor.gameObject);
		}
		if (this.m_currentBigActor != null)
		{
			Object.Destroy(this.m_currentBigActor.gameObject);
		}
		CollectionManagerDisplay.Get().HideAllTips();
		this.m_arcaneDustBalance = NetCache.Get().GetNetObject<NetCache.NetCacheArcaneDustBalance>().Balance;
		this.m_offClickCatcher.enabled = true;
		KeywordHelpPanelManager.Get().HideKeywordHelp();
		this.MoveCardToBigSpot(cardActor);
		if (this.m_craftingUI == null)
		{
			string name = (!UniversalInputManager.UsePhoneUI) ? "CraftingUI" : "CraftingUI_Phone";
			GameObject gameObject = AssetLoader.Get().LoadGameObject(name, true, false);
			this.m_craftingUI = gameObject.GetComponent<CraftingUI>();
			this.m_craftingUI.SetStartingActive();
			GameUtils.SetParent(this.m_craftingUI, this.m_showCraftingUIBone.gameObject, false);
		}
		if (this.m_cardInfoPane == null && !UniversalInputManager.UsePhoneUI)
		{
			GameObject gameObject2 = AssetLoader.Get().LoadGameObject("CardInfoPane", true, false);
			this.m_cardInfoPane = gameObject2.GetComponent<CardInfoPane>();
		}
		this.m_craftingUI.gameObject.SetActive(true);
		this.m_craftingUI.Enable(this.m_showCraftingUIBone.position, this.m_hideCraftingUIBone.position);
		this.FadeEffectsIn();
		this.UpdateCardInfoPane();
		Navigation.Push(new Navigation.NavigateBackHandler(this.CancelCraftMode));
	}

	// Token: 0x06003AFD RID: 15101 RVA: 0x0011CD84 File Offset: 0x0011AF84
	public bool CancelCraftMode()
	{
		base.StopAllCoroutines();
		this.m_offClickCatcher.enabled = false;
		this.m_cancellingCraftMode = true;
		this.m_craftingUI.CleanUpEffects();
		float num = 0.2f;
		if (this.m_currentBigActor != null)
		{
			iTween.Stop(this.m_currentBigActor.gameObject);
			iTween.RotateTo(this.m_currentBigActor.gameObject, Vector3.zero, num);
			this.m_currentBigActor.ToggleForceIdle(false);
			if (this.m_upsideDownActor != null)
			{
				iTween.Stop(this.m_upsideDownActor.gameObject);
				this.m_upsideDownActor.transform.parent = this.m_currentBigActor.transform;
			}
		}
		SoundManager.Get().LoadAndPlay("Card_Transition_In");
		iTween.MoveTo(this.m_currentBigActor.gameObject, iTween.Hash(new object[]
		{
			"name",
			"CancelCraftMode",
			"position",
			this.m_craftSourcePosition,
			"time",
			num,
			"oncomplete",
			"FinishActorMove",
			"oncompletetarget",
			base.gameObject,
			"easetype",
			iTween.EaseType.linear
		}));
		iTween.ScaleTo(this.m_currentBigActor.gameObject, iTween.Hash(new object[]
		{
			"scale",
			this.m_craftSourceScale,
			"time",
			num,
			"easetype",
			iTween.EaseType.linear
		}));
		iTween.Stop(this.m_cardCountTab.gameObject);
		int numOwnedCopies = this.GetNumOwnedCopies(this.m_currentBigActor.GetEntityDef().GetCardId(), this.m_currentBigActor.GetPremium());
		if (numOwnedCopies > 0)
		{
			iTween.MoveTo(this.m_cardCountTab.gameObject, iTween.Hash(new object[]
			{
				"position",
				this.m_craftSourcePosition - new Vector3(0f, 12f, 0f),
				"time",
				3f * num,
				"oncomplete",
				iTween.EaseType.easeInQuad
			}));
			iTween.ScaleTo(this.m_cardCountTab.gameObject, iTween.Hash(new object[]
			{
				"scale",
				0.1f * Vector3.one,
				"time",
				3f * num,
				"oncomplete",
				iTween.EaseType.easeInQuad
			}));
		}
		if (this.m_upsideDownActor != null)
		{
			iTween.RotateTo(this.m_upsideDownActor.gameObject, new Vector3(0f, 359f, 180f), num);
			iTween.MoveTo(this.m_upsideDownActor.gameObject, iTween.Hash(new object[]
			{
				"name",
				"CancelCraftMode2",
				"position",
				new Vector3(0f, -1f, 0f),
				"time",
				num,
				"islocal",
				true
			}));
			iTween.ScaleTo(this.m_upsideDownActor.gameObject, new Vector3(this.m_upsideDownActor.transform.localScale.x * 0.8f, this.m_upsideDownActor.transform.localScale.y * 0.8f, this.m_upsideDownActor.transform.localScale.z * 0.8f), num);
		}
		if (this.m_craftingUI != null && this.m_craftingUI.IsEnabled())
		{
			this.m_craftingUI.Disable(this.m_hideCraftingUIBone.position);
		}
		this.m_cardCountTab.m_shadow.GetComponent<Animation>().Play("Crafting2ndCardShadowOff");
		this.FadeEffectsOut();
		if (this.m_cardInfoPane != null)
		{
			iTween.Stop(this.m_cardInfoPane.gameObject);
			this.m_cardInfoPane.gameObject.SetActive(false);
		}
		this.TellServerAboutWhatUserDid();
		return true;
	}

	// Token: 0x06003AFE RID: 15102 RVA: 0x0011D1DC File Offset: 0x0011B3DC
	public void CreateButtonPressed()
	{
		this.m_craftingUI.DoCreate();
	}

	// Token: 0x06003AFF RID: 15103 RVA: 0x0011D1E9 File Offset: 0x0011B3E9
	public void DisenchantButtonPressed()
	{
		this.m_craftingUI.DoDisenchant();
	}

	// Token: 0x06003B00 RID: 15104 RVA: 0x0011D1F6 File Offset: 0x0011B3F6
	public void UpdateBankText()
	{
		if (this.m_craftingUI != null)
		{
			this.m_craftingUI.UpdateBankText();
		}
	}

	// Token: 0x06003B01 RID: 15105 RVA: 0x0011D214 File Offset: 0x0011B414
	private void TellServerAboutWhatUserDid()
	{
		Actor actor = this.m_currentBigActor;
		if (this.m_currentBigActor == null)
		{
			actor = this.m_upsideDownActor;
		}
		if (actor == null)
		{
			return;
		}
		TAG_PREMIUM premium = actor.GetPremium();
		string cardId = actor.GetEntityDef().GetCardId();
		int assetID = GameUtils.TranslateCardIdToDbId(cardId);
		Log.Ben.Print("Final Transaction Amount = " + this.m_transactions, new object[0]);
		if (this.m_transactions != 0)
		{
			this.m_pendingTransaction = new PendingTransaction();
			this.m_pendingTransaction.CardID = cardId;
			this.m_pendingTransaction.TransactionAmt = this.m_transactions;
			this.m_pendingTransaction.Premium = premium;
		}
		NetCache.CardValue cardValue = this.GetCardValue(cardId, premium);
		if (this.m_transactions < 0)
		{
			Network.SellCard(assetID, premium, -this.m_transactions, cardValue.Sell);
		}
		else if (this.m_transactions > 0)
		{
			Network.BuyCard(assetID, premium, this.m_transactions, cardValue.Buy);
		}
		this.m_transactions = 0;
	}

	// Token: 0x06003B02 RID: 15106 RVA: 0x0011D324 File Offset: 0x0011B524
	public void OnCardGenericError(Network.CardSaleResult sale)
	{
		this.m_pendingTransaction = null;
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER");
		popupInfo.m_text = GameStrings.Get("GLUE_COLLECTION_GENERIC_ERROR");
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06003B03 RID: 15107 RVA: 0x0011D378 File Offset: 0x0011B578
	public void OnCardPermissionError(Network.CardSaleResult sale)
	{
		this.m_pendingTransaction = null;
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER");
		popupInfo.m_text = GameStrings.Get("GLUE_COLLECTION_CARD_PERMISSION_ERROR");
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06003B04 RID: 15108 RVA: 0x0011D3CC File Offset: 0x0011B5CC
	public void OnCardDisenchantSoulboundError(Network.CardSaleResult sale)
	{
		this.m_pendingTransaction = null;
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER");
		popupInfo.m_text = GameStrings.Get("GLUE_COLLECTION_CARD_SOULBOUND");
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06003B05 RID: 15109 RVA: 0x0011D420 File Offset: 0x0011B620
	public void OnCardCraftingEventNotActiveError(Network.CardSaleResult sale)
	{
		this.m_pendingTransaction = null;
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER");
		popupInfo.m_text = GameStrings.Get("GLUE_COLLECTION_CARD_CRAFTING_EVENT_NOT_ACTIVE");
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06003B06 RID: 15110 RVA: 0x0011D474 File Offset: 0x0011B674
	public void OnCardUnknownError(Network.CardSaleResult sale)
	{
		this.m_pendingTransaction = null;
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER");
		popupInfo.m_text = GameStrings.Format("GLUE_COLLECTION_CARD_UNKNOWN_ERROR", new object[]
		{
			sale.Action
		});
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06003B07 RID: 15111 RVA: 0x0011D4DC File Offset: 0x0011B6DC
	public void OnCardDisenchanted(Network.CardSaleResult sale)
	{
		this.m_pendingTransaction = null;
		NetCache.Get().OnArcaneDustBalanceChanged((long)sale.Amount);
		int numCopiesInCollection = CollectionManager.Get().GetNumCopiesInCollection(sale.AssetName, sale.Premium);
		if (numCopiesInCollection > 0)
		{
			this.OnCardDisenchantedPageTransitioned(sale);
		}
		else
		{
			CollectionManagerDisplay.Get().m_pageManager.RefreshCurrentPageContents(new CollectionPageManager.DelOnPageTransitionComplete(this.OnCardDisenchantedPageTransitioned), sale);
		}
		CollectionCardVisual cardVisual = CollectionManagerDisplay.Get().m_pageManager.GetCardVisual(sale.AssetName, sale.Premium);
		if (cardVisual != null && cardVisual.IsShown())
		{
			cardVisual.OnDoneCrafting();
		}
	}

	// Token: 0x06003B08 RID: 15112 RVA: 0x0011D580 File Offset: 0x0011B780
	public void OnCardCreated(Network.CardSaleResult sale)
	{
		this.m_pendingTransaction = null;
		NetCache.Get().OnArcaneDustBalanceChanged((long)(-(long)sale.Amount));
		CollectionManagerDisplay.Get().m_pageManager.RefreshCurrentPageContents(new CollectionPageManager.DelOnPageTransitionComplete(this.OnCardCreatedPageTransitioned), sale);
		CollectionCardVisual cardVisual = CollectionManagerDisplay.Get().m_pageManager.GetCardVisual(sale.AssetName, sale.Premium);
		if (cardVisual != null && cardVisual.IsShown())
		{
			cardVisual.OnDoneCrafting();
		}
		if (CollectionDeckTray.Get() != null && this.m_currentBigActor != null)
		{
			for (int i = 0; i < sale.Count; i++)
			{
				DeckTrayDeckTileVisual cardTileVisual = CollectionDeckTray.Get().m_cardsContent.GetCardTileVisual(sale.AssetName);
				if (cardTileVisual != null)
				{
					CollectionDeckTray.Get().AddCard(this.m_currentBigActor.GetEntityDef(), sale.Premium, cardTileVisual, true, null);
				}
			}
		}
	}

	// Token: 0x06003B09 RID: 15113 RVA: 0x0011D674 File Offset: 0x0011B874
	public void LoadGhostActorIfNecessary()
	{
		if (this.m_cancellingCraftMode)
		{
			return;
		}
		iTween.ScaleTo(this.m_cardCountTab.gameObject, this.m_cardCountTabHideScale, 0.4f);
		int numOwnedCopies = this.GetNumOwnedCopies(this.m_currentBigActor.GetEntityDef().GetCardId(), this.m_currentBigActor.GetPremium());
		if (numOwnedCopies <= 0)
		{
			this.m_currentBigActor = this.GetAndPositionNewActor(this.m_currentBigActor, 0);
			this.m_currentBigActor.name = "CurrentBigActor";
			this.m_currentBigActor.transform.position = this.m_floatingCardBone.position;
			this.m_currentBigActor.transform.localScale = this.m_floatingCardBone.localScale;
			this.m_cardCountTab.transform.position = new Vector3(0f, 307f, -10f);
			this.SetBigActorLayer(true);
			return;
		}
		if (this.m_upsideDownActor == null)
		{
			this.m_currentBigActor = this.GetAndPositionNewActor(this.m_currentBigActor, 1);
			this.m_currentBigActor.name = "CurrentBigActor";
			this.m_currentBigActor.transform.position = this.m_floatingCardBone.position;
			this.m_currentBigActor.transform.localScale = this.m_floatingCardBone.localScale;
			this.m_cardCountTab.transform.position = new Vector3(0f, 307f, -10f);
			this.SetBigActorLayer(true);
			return;
		}
		this.m_upsideDownActor.transform.parent = null;
		this.m_currentBigActor = this.m_upsideDownActor;
		this.m_currentBigActor.name = "CurrentBigActor";
		this.m_upsideDownActor = null;
	}

	// Token: 0x06003B0A RID: 15114 RVA: 0x0011D820 File Offset: 0x0011BA20
	public Actor LoadNewActorAndConstructIt()
	{
		if (this.m_cancellingCraftMode)
		{
			return null;
		}
		if (!this.m_isCurrentActorAGhost)
		{
			Actor oldActor = this.m_currentBigActor;
			if (this.m_currentBigActor == null)
			{
				oldActor = this.m_upsideDownActor;
			}
			else
			{
				this.m_currentBigActor.name = "Current_Big_Actor_Lost_Refernce";
			}
			this.m_currentBigActor = this.GetAndPositionNewActor(oldActor, 0);
			this.m_isCurrentActorAGhost = false;
			this.m_currentBigActor.name = "CurrentBigActor";
			this.m_currentBigActor.transform.position = this.m_floatingCardBone.position;
			this.m_currentBigActor.transform.localScale = this.m_floatingCardBone.localScale;
			this.SetBigActorLayer(true);
		}
		this.m_currentBigActor.ActivateSpell(SpellType.CONSTRUCT);
		return this.m_currentBigActor;
	}

	// Token: 0x06003B0B RID: 15115 RVA: 0x0011D8EF File Offset: 0x0011BAEF
	public void ForceNonGhostFlagOn()
	{
		this.m_isCurrentActorAGhost = false;
	}

	// Token: 0x06003B0C RID: 15116 RVA: 0x0011D8F8 File Offset: 0x0011BAF8
	public void FinishCreateAnims()
	{
		if (this.m_cancellingCraftMode)
		{
			return;
		}
		iTween.ScaleTo(this.m_cardCountTab.gameObject, this.m_cardCountTabShowScale, 0.4f);
		this.m_currentBigActor.GetSpell(SpellType.GHOSTMODE).GetComponent<PlayMakerFSM>().SendEvent("Cancel");
		this.m_isCurrentActorAGhost = false;
		int numOwnedCopies = this.GetNumOwnedCopies(this.m_currentBigActor.GetEntityDef().GetCardId(), this.m_currentBigActor.GetPremium());
		this.m_cardCountTab.UpdateText(numOwnedCopies);
		this.m_cardCountTab.transform.position = this.m_cardCounterBone.position;
	}

	// Token: 0x06003B0D RID: 15117 RVA: 0x0011D998 File Offset: 0x0011BB98
	public void FlipCurrentActor()
	{
		if (this.m_currentBigActor == null || this.m_isCurrentActorAGhost)
		{
			return;
		}
		this.m_cardCountTab.transform.localScale = this.m_cardCountTabHideScale;
		this.m_upsideDownActor = this.m_currentBigActor;
		this.m_upsideDownActor.name = "UpsideDownActor";
		this.m_upsideDownActor.GetSpell(SpellType.GHOSTMODE).GetComponent<PlayMakerFSM>().SendEvent("Cancel");
		this.m_currentBigActor = null;
		iTween.Stop(this.m_upsideDownActor.gameObject);
		iTween.RotateTo(this.m_upsideDownActor.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 350f, 180f),
			"time",
			1f
		}));
		iTween.MoveTo(this.m_upsideDownActor.gameObject, iTween.Hash(new object[]
		{
			"name",
			"FlipCurrentActor",
			"position",
			this.m_faceDownCardBone.position,
			"time",
			1f
		}));
		base.StartCoroutine(this.ReplaceFaceDownActorWithHiddenCard());
	}

	// Token: 0x06003B0E RID: 15118 RVA: 0x0011DAE4 File Offset: 0x0011BCE4
	public void FinishFlipCurrentActorEarly()
	{
		base.StopAllCoroutines();
		iTween.Stop(this.m_currentBigActor.gameObject);
		this.m_currentBigActor.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		this.m_currentBigActor.transform.position = this.m_floatingCardBone.position;
		this.m_currentBigActor.Show();
		GameObject hiddenStandIn = this.m_currentBigActor.GetHiddenStandIn();
		if (hiddenStandIn == null)
		{
			return;
		}
		hiddenStandIn.SetActive(false);
		Object.Destroy(hiddenStandIn);
	}

	// Token: 0x06003B0F RID: 15119 RVA: 0x0011DB78 File Offset: 0x0011BD78
	public void FlipUpsideDownCard(Actor oldActor)
	{
		if (this.m_cancellingCraftMode)
		{
			return;
		}
		int numOwnedCopies = this.GetNumOwnedCopies(this.m_currentBigActor.GetEntityDef().GetCardId(), this.m_currentBigActor.GetPremium());
		if (numOwnedCopies > 1)
		{
			this.m_upsideDownActor = this.GetAndPositionNewUpsideDownActor(this.m_currentBigActor, false);
			this.m_upsideDownActor.name = "UpsideDownActor";
			base.StartCoroutine(this.ReplaceFaceDownActorWithHiddenCard());
		}
		if (numOwnedCopies >= 1)
		{
			iTween.ScaleTo(this.m_cardCountTab.gameObject, iTween.Hash(new object[]
			{
				"scale",
				this.m_cardCountTabShowScale,
				"time",
				0.4f,
				"delay",
				this.m_timeForCardToFlipUp
			}));
			this.m_cardCountTab.UpdateText(numOwnedCopies);
		}
		if (this.m_isCurrentActorAGhost)
		{
			this.m_currentBigActor.gameObject.transform.position = this.m_floatingCardBone.position;
		}
		else
		{
			iTween.MoveTo(this.m_currentBigActor.gameObject, iTween.Hash(new object[]
			{
				"name",
				"FlipUpsideDownCard",
				"position",
				this.m_floatingCardBone.position,
				"time",
				this.m_timeForCardToFlipUp,
				"easetype",
				this.m_easeTypeForCardFlip
			}));
		}
		iTween.RotateTo(this.m_currentBigActor.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(0f, 0f, 0f),
			"time",
			this.m_timeForCardToFlipUp,
			"easetype",
			this.m_easeTypeForCardFlip
		}));
		base.StartCoroutine(this.ReplaceHiddenCardwithRealActor(this.m_currentBigActor));
	}

	// Token: 0x06003B10 RID: 15120 RVA: 0x0011DD7C File Offset: 0x0011BF7C
	private IEnumerator ReplaceFaceDownActorWithHiddenCard()
	{
		while (this.m_upsideDownActor != null && this.m_upsideDownActor.transform.localEulerAngles.z < 90f)
		{
			yield return null;
		}
		if (this.m_upsideDownActor == null)
		{
			yield break;
		}
		GameObject hiddenBuddy = Object.Instantiate<GameObject>(this.m_hiddenActor.gameObject);
		this.m_upsideDownActor.Hide();
		hiddenBuddy.transform.parent = this.m_upsideDownActor.transform;
		this.m_upsideDownActor.SetHiddenStandIn(hiddenBuddy);
		hiddenBuddy.transform.localScale = new Vector3(1f, 1f, 1f);
		hiddenBuddy.transform.localPosition = new Vector3(0f, 0f, 0f);
		hiddenBuddy.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		yield break;
	}

	// Token: 0x06003B11 RID: 15121 RVA: 0x0011DD98 File Offset: 0x0011BF98
	private IEnumerator ReplaceHiddenCardwithRealActor(Actor actor)
	{
		while (actor != null && actor.transform.localEulerAngles.z > 90f && actor.transform.localEulerAngles.z < 270f)
		{
			yield return null;
		}
		if (actor == null)
		{
			yield break;
		}
		actor.Show();
		GameObject standIn = actor.GetHiddenStandIn();
		if (standIn == null)
		{
			yield break;
		}
		standIn.SetActive(false);
		Object.Destroy(standIn);
		yield break;
	}

	// Token: 0x06003B12 RID: 15122 RVA: 0x0011DDBA File Offset: 0x0011BFBA
	public PendingTransaction GetPendingTransaction()
	{
		return this.m_pendingTransaction;
	}

	// Token: 0x06003B13 RID: 15123 RVA: 0x0011DDC4 File Offset: 0x0011BFC4
	public void ShowCraftingUI(UIEvent e)
	{
		if (this.m_craftingUI.IsEnabled())
		{
			this.m_craftingUI.Disable(this.m_hideCraftingUIBone.position);
		}
		else
		{
			this.m_craftingUI.Enable(this.m_showCraftingUIBone.position, this.m_hideCraftingUIBone.position);
		}
	}

	// Token: 0x06003B14 RID: 15124 RVA: 0x0011DE20 File Offset: 0x0011C020
	private void MoveCardToBigSpot(Actor cardActor)
	{
		if (cardActor == null)
		{
			return;
		}
		if (cardActor == null)
		{
			return;
		}
		EntityDef entityDef = cardActor.GetEntityDef();
		if (entityDef == null)
		{
			return;
		}
		int numOwnedCopies = this.GetNumOwnedCopies(entityDef.GetCardId(), cardActor.GetPremium());
		this.m_currentBigActor = this.GetAndPositionNewActor(cardActor, numOwnedCopies);
		this.m_currentBigActor.name = "CurrentBigActor";
		this.m_craftSourcePosition = cardActor.transform.position;
		this.m_craftSourceScale = cardActor.transform.lossyScale;
		this.m_craftSourceScale = Vector3.one * Mathf.Min(new float[]
		{
			this.m_craftSourceScale.x,
			this.m_craftSourceScale.y,
			this.m_craftSourceScale.z
		});
		this.m_currentBigActor.transform.position = this.m_craftSourcePosition;
		TransformUtil.SetWorldScale(this.m_currentBigActor, this.m_craftSourceScale);
		this.SetBigActorLayer(true);
		this.m_currentBigActor.ToggleForceIdle(true);
		this.m_currentBigActor.SetActorState(ActorStateType.CARD_IDLE);
		if (entityDef.IsHero())
		{
			this.m_cardCountTab.gameObject.SetActive(false);
		}
		else
		{
			this.m_cardCountTab.gameObject.SetActive(true);
			if (numOwnedCopies > 1)
			{
				this.m_upsideDownActor = this.GetAndPositionNewUpsideDownActor(cardActor, true);
				this.m_upsideDownActor.name = "UpsideDownActor";
				base.StartCoroutine(this.ReplaceFaceDownActorWithHiddenCard());
			}
			if (numOwnedCopies > 0)
			{
				this.m_cardCountTab.UpdateText(numOwnedCopies);
				this.m_cardCountTab.transform.position = new Vector3(cardActor.transform.position.x, cardActor.transform.position.y - 2f, cardActor.transform.position.z);
			}
		}
		this.FinishBigCardMove();
	}

	// Token: 0x06003B15 RID: 15125 RVA: 0x0011E008 File Offset: 0x0011C208
	private void FinishBigCardMove()
	{
		if (this.m_currentBigActor == null)
		{
			return;
		}
		int numOwnedCopies = this.GetNumOwnedCopies(this.m_currentBigActor.GetEntityDef().GetCardId(), this.m_currentBigActor.GetPremium());
		SoundManager.Get().LoadAndPlay("Card_Transition_Out");
		iTween.MoveTo(this.m_currentBigActor.gameObject, iTween.Hash(new object[]
		{
			"name",
			"FinishBigCardMove",
			"position",
			this.m_floatingCardBone.position,
			"time",
			0.4f
		}));
		iTween.ScaleTo(this.m_currentBigActor.gameObject, iTween.Hash(new object[]
		{
			"scale",
			this.m_floatingCardBone.localScale,
			"time",
			0.4f,
			"easetype",
			iTween.EaseType.easeOutQuad
		}));
		if (numOwnedCopies > 0)
		{
			this.m_cardCountTab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			iTween.MoveTo(this.m_cardCountTab.gameObject, this.m_cardCounterBone.position, 0.4f);
			iTween.ScaleTo(this.m_cardCountTab.gameObject, this.m_cardCountTabShowScale, 0.4f);
		}
	}

	// Token: 0x06003B16 RID: 15126 RVA: 0x0011E178 File Offset: 0x0011C378
	private void UpdateCardInfoPane()
	{
		if (this.m_cardInfoPane == null)
		{
			return;
		}
		this.m_cardInfoPane.gameObject.SetActive(true);
		this.m_cardInfoPane.UpdateContent();
		this.m_cardInfoPane.transform.position = this.m_currentBigActor.transform.position - new Vector3(0f, 1f, 0f);
		Vector3 localScale = this.m_cardInfoPaneBone.localScale;
		this.m_cardInfoPane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		iTween.MoveTo(this.m_cardInfoPane.gameObject, this.m_cardInfoPaneBone.position, 0.5f);
		iTween.ScaleTo(this.m_cardInfoPane.gameObject, localScale, 0.5f);
	}

	// Token: 0x06003B17 RID: 15127 RVA: 0x0011E254 File Offset: 0x0011C454
	private void FinishActorMove()
	{
		this.m_cancellingCraftMode = false;
		iTween.Stop(this.m_cardCountTab.gameObject);
		this.m_cardCountTab.transform.position = new Vector3(0f, 307f, -10f);
		if (this.m_upsideDownActor != null)
		{
			Object.Destroy(this.m_upsideDownActor.gameObject);
		}
		if (this.m_currentBigActor != null)
		{
			Object.Destroy(this.m_currentBigActor.gameObject);
		}
	}

	// Token: 0x06003B18 RID: 15128 RVA: 0x0011E2E0 File Offset: 0x0011C4E0
	private void FadeEffectsIn()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.SetBlurBrightness(1f);
		fullScreenFXMgr.SetBlurDesaturation(0f);
		fullScreenFXMgr.Vignette(0.4f, 0.4f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.Blur(1f, 0.4f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06003B19 RID: 15129 RVA: 0x0011E330 File Offset: 0x0011C530
	private void FadeEffectsOut()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, new FullScreenFXMgr.EffectListener(this.OnVignetteFinished));
		fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06003B1A RID: 15130 RVA: 0x0011E36C File Offset: 0x0011C56C
	private void OnVignetteFinished()
	{
		this.SetBigActorLayer(false);
		if (this.GetCurrentCardVisual() != null)
		{
			this.GetCurrentCardVisual().OnDoneCrafting();
		}
		if (this.m_currentBigActor != null)
		{
			this.m_currentBigActor.name = "USED_TO_BE_CurrentBigActor";
			base.StartCoroutine(this.MakeSureActorIsCleanedUp(this.m_currentBigActor));
		}
		this.m_currentBigActor = null;
		this.m_craftingUI.gameObject.SetActive(false);
	}

	// Token: 0x06003B1B RID: 15131 RVA: 0x0011E3E8 File Offset: 0x0011C5E8
	private IEnumerator MakeSureActorIsCleanedUp(Actor oldActor)
	{
		yield return new WaitForSeconds(1f);
		if (oldActor == null)
		{
			yield break;
		}
		Object.DestroyImmediate(oldActor);
		yield break;
	}

	// Token: 0x06003B1C RID: 15132 RVA: 0x0011E40C File Offset: 0x0011C60C
	private Actor GetAndPositionNewUpsideDownActor(Actor oldActor, bool fromPage)
	{
		Actor andPositionNewActor = this.GetAndPositionNewActor(oldActor, 1);
		SceneUtils.SetLayer(andPositionNewActor.gameObject, GameLayer.IgnoreFullScreenEffects);
		if (fromPage)
		{
			andPositionNewActor.transform.position = oldActor.transform.position + new Vector3(0f, -2f, 0f);
			andPositionNewActor.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
			iTween.RotateTo(andPositionNewActor.gameObject, new Vector3(0f, 350f, 180f), 0.4f);
			iTween.MoveTo(andPositionNewActor.gameObject, iTween.Hash(new object[]
			{
				"name",
				"GetAndPositionNewUpsideDownActor",
				"position",
				this.m_faceDownCardBone.position,
				"time",
				0.4f
			}));
			iTween.ScaleTo(andPositionNewActor.gameObject, this.m_faceDownCardBone.localScale, 0.4f);
		}
		else
		{
			andPositionNewActor.transform.localEulerAngles = new Vector3(0f, 350f, 180f);
			andPositionNewActor.transform.position = this.m_faceDownCardBone.position + new Vector3(0f, -6f, 0f);
			andPositionNewActor.transform.localScale = this.m_faceDownCardBone.localScale;
			iTween.MoveTo(andPositionNewActor.gameObject, iTween.Hash(new object[]
			{
				"name",
				"GetAndPositionNewUpsideDownActor",
				"position",
				this.m_faceDownCardBone.position,
				"time",
				this.m_timeForBackCardToMoveUp,
				"easetype",
				this.m_easeTypeForCardMoveUp,
				"delay",
				this.m_delayBeforeBackCardMovesUp
			}));
		}
		return andPositionNewActor;
	}

	// Token: 0x06003B1D RID: 15133 RVA: 0x0011E608 File Offset: 0x0011C808
	private Actor GetAndPositionNewActor(Actor oldActor, int numCopies)
	{
		Actor actor;
		if (numCopies == 0)
		{
			actor = this.GetGhostActor(oldActor);
		}
		else
		{
			actor = this.GetNonGhostActor(oldActor);
		}
		actor.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		return actor;
	}

	// Token: 0x06003B1E RID: 15134 RVA: 0x0011E650 File Offset: 0x0011C850
	private Actor GetGhostActor(Actor actor)
	{
		this.m_isCurrentActorAGhost = true;
		bool flag = actor.GetPremium() == TAG_PREMIUM.GOLDEN;
		Actor templateActor = this.m_ghostMinionActor;
		switch (actor.GetEntityDef().GetCardType())
		{
		case TAG_CARDTYPE.MINION:
			if (flag)
			{
				templateActor = this.m_ghostGoldenMinionActor;
			}
			else
			{
				templateActor = this.m_ghostMinionActor;
			}
			goto IL_AA;
		case TAG_CARDTYPE.SPELL:
			if (flag)
			{
				templateActor = this.m_ghostGoldenSpellActor;
			}
			else
			{
				templateActor = this.m_ghostSpellActor;
			}
			goto IL_AA;
		case TAG_CARDTYPE.WEAPON:
			if (flag)
			{
				templateActor = this.m_ghostGoldenWeaponActor;
			}
			else
			{
				templateActor = this.m_ghostWeaponActor;
			}
			goto IL_AA;
		}
		Debug.LogError("CraftingManager.GetGhostActor() - tried to get a ghost actor for a cardtype that we haven't anticipated!!");
		IL_AA:
		return this.SetUpGhostActor(templateActor, actor);
	}

	// Token: 0x06003B1F RID: 15135 RVA: 0x0011E70F File Offset: 0x0011C90F
	private Actor GetNonGhostActor(Actor actor)
	{
		this.m_isCurrentActorAGhost = false;
		return this.SetUpNonGhostActor(this.GetTemplateActor(actor), actor);
	}

	// Token: 0x06003B20 RID: 15136 RVA: 0x0011E728 File Offset: 0x0011C928
	private Actor GetTemplateActor(Actor actor)
	{
		bool flag = actor.GetPremium() == TAG_PREMIUM.GOLDEN;
		switch (actor.GetEntityDef().GetCardType())
		{
		case TAG_CARDTYPE.HERO:
			return this.m_templateHeroSkinActor;
		case TAG_CARDTYPE.MINION:
			if (flag)
			{
				return this.m_templateGoldenMinionActor;
			}
			return this.m_templateMinionActor;
		case TAG_CARDTYPE.SPELL:
			if (flag)
			{
				return this.m_templateGoldenSpellActor;
			}
			return this.m_templateSpellActor;
		case TAG_CARDTYPE.WEAPON:
			if (flag)
			{
				return this.m_templateGoldenWeaponActor;
			}
			return this.m_templateWeaponActor;
		}
		Debug.LogError("CraftingManager.GetGhostActor() - tried to get a ghost actor for a cardtype that we haven't anticipated!!");
		return this.m_templateMinionActor;
	}

	// Token: 0x06003B21 RID: 15137 RVA: 0x0011E7C0 File Offset: 0x0011C9C0
	private Actor SetUpNonGhostActor(Actor templateActor, Actor actor)
	{
		Actor actor2 = Object.Instantiate<Actor>(templateActor);
		actor2.SetEntityDef(actor.GetEntityDef());
		actor2.SetPremium(actor.GetPremium());
		actor2.SetCardDef(actor.GetCardDef());
		actor2.UpdateAllComponents();
		return actor2;
	}

	// Token: 0x06003B22 RID: 15138 RVA: 0x0011E800 File Offset: 0x0011CA00
	private Actor SetUpGhostActor(Actor templateActor, Actor actor)
	{
		Actor actor2 = Object.Instantiate<Actor>(templateActor);
		actor2.SetEntityDef(actor.GetEntityDef());
		actor2.SetPremium(actor.GetPremium());
		actor2.SetCardDef(actor.GetCardDef());
		actor2.UpdateAllComponents();
		actor2.UpdatePortraitTexture();
		actor2.UpdateCardColor();
		actor2.Hide();
		if (actor.isMissingCard())
		{
			actor2.ActivateSpell(SpellType.MISSING_BIGCARD);
		}
		else
		{
			actor2.ActivateSpell(SpellType.GHOSTMODE);
		}
		base.StartCoroutine(this.ShowAfterTwoFrames(actor2));
		return actor2;
	}

	// Token: 0x06003B23 RID: 15139 RVA: 0x0011E884 File Offset: 0x0011CA84
	private IEnumerator ShowAfterTwoFrames(Actor actorToShow)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		if (actorToShow != this.m_currentBigActor)
		{
			yield break;
		}
		actorToShow.Show();
		yield break;
	}

	// Token: 0x06003B24 RID: 15140 RVA: 0x0011E8B0 File Offset: 0x0011CAB0
	private void SetBigActorLayer(bool inCraftingMode)
	{
		if (this.m_currentBigActor == null)
		{
			return;
		}
		GameLayer layer = (!inCraftingMode) ? GameLayer.CardRaycast : GameLayer.IgnoreFullScreenEffects;
		SceneUtils.SetLayer(this.m_currentBigActor.gameObject, layer);
	}

	// Token: 0x06003B25 RID: 15141 RVA: 0x0011E8EF File Offset: 0x0011CAEF
	private void OnCardCreatedPageTransitioned(object callbackData)
	{
		CollectionManagerDisplay.Get().UpdateCurrentPageCardLocks(false);
	}

	// Token: 0x06003B26 RID: 15142 RVA: 0x0011E8FC File Offset: 0x0011CAFC
	private void OnCardDisenchantedPageTransitioned(object callbackData)
	{
		CollectionManagerDisplay.Get().UpdateCurrentPageCardLocks(false);
	}

	// Token: 0x06003B27 RID: 15143 RVA: 0x0011E90C File Offset: 0x0011CB0C
	private CollectionCardVisual GetCurrentCardVisual()
	{
		EntityDef entityDef;
		TAG_PREMIUM premium;
		if (!this.GetShownCardInfo(out entityDef, out premium))
		{
			return null;
		}
		return CollectionManagerDisplay.Get().m_pageManager.GetCardVisual(entityDef.GetCardId(), premium);
	}

	// Token: 0x06003B28 RID: 15144 RVA: 0x0011E940 File Offset: 0x0011CB40
	public int GetNumOwnedCopies(string cardID, TAG_PREMIUM premium)
	{
		return this.GetNumOwnedCopies(cardID, premium, true);
	}

	// Token: 0x06003B29 RID: 15145 RVA: 0x0011E94C File Offset: 0x0011CB4C
	public int GetNumOwnedCopies(string cardID, TAG_PREMIUM premium, bool includePending)
	{
		int numCopiesInCollection = CollectionManager.Get().GetNumCopiesInCollection(cardID, premium);
		if (includePending)
		{
			return numCopiesInCollection + this.m_transactions;
		}
		return numCopiesInCollection;
	}

	// Token: 0x06003B2A RID: 15146 RVA: 0x0011E978 File Offset: 0x0011CB78
	private void LoadActor(string actorName, ref Actor actor)
	{
		GameObject gameObject = AssetLoader.Get().LoadActor(actorName, false, false);
		gameObject.transform.position = new Vector3(-99999f, 99999f, 99999f);
		actor = gameObject.GetComponent<Actor>();
		actor.TurnOffCollider();
	}

	// Token: 0x06003B2B RID: 15147 RVA: 0x0011E9C4 File Offset: 0x0011CBC4
	private void LoadActor(string actorName, ref Actor actor, ref Actor actorCopy)
	{
		GameObject gameObject = AssetLoader.Get().LoadActor(actorName, false, false);
		gameObject.transform.position = new Vector3(-99999f, 99999f, 99999f);
		actor = gameObject.GetComponent<Actor>();
		actorCopy = Object.Instantiate<Actor>(actor);
		actor.TurnOffCollider();
		actorCopy.TurnOffCollider();
	}

	// Token: 0x04002595 RID: 9621
	public Transform m_floatingCardBone;

	// Token: 0x04002596 RID: 9622
	public Transform m_faceDownCardBone;

	// Token: 0x04002597 RID: 9623
	public Transform m_cardInfoPaneBone;

	// Token: 0x04002598 RID: 9624
	public Transform m_cardCounterBone;

	// Token: 0x04002599 RID: 9625
	public Transform m_showCraftingUIBone;

	// Token: 0x0400259A RID: 9626
	public Transform m_hideCraftingUIBone;

	// Token: 0x0400259B RID: 9627
	public BoxCollider m_offClickCatcher;

	// Token: 0x0400259C RID: 9628
	public CraftCardCountTab m_cardCountTab;

	// Token: 0x0400259D RID: 9629
	public Vector3 m_cardCountTabShowScale = Vector3.one;

	// Token: 0x0400259E RID: 9630
	public Vector3 m_cardCountTabHideScale = new Vector3(1f, 1f, 0f);

	// Token: 0x0400259F RID: 9631
	public PegUIElement m_dustJar;

	// Token: 0x040025A0 RID: 9632
	public float m_timeForCardToFlipUp;

	// Token: 0x040025A1 RID: 9633
	public float m_timeForBackCardToMoveUp;

	// Token: 0x040025A2 RID: 9634
	public float m_delayBeforeBackCardMovesUp;

	// Token: 0x040025A3 RID: 9635
	public iTween.EaseType m_easeTypeForCardFlip;

	// Token: 0x040025A4 RID: 9636
	public iTween.EaseType m_easeTypeForCardMoveUp;

	// Token: 0x040025A5 RID: 9637
	private static CraftingManager s_instance;

	// Token: 0x040025A6 RID: 9638
	public CraftingUI m_craftingUI;

	// Token: 0x040025A7 RID: 9639
	private Actor m_currentBigActor;

	// Token: 0x040025A8 RID: 9640
	private bool m_isCurrentActorAGhost;

	// Token: 0x040025A9 RID: 9641
	private Actor m_upsideDownActor;

	// Token: 0x040025AA RID: 9642
	private Actor m_ghostWeaponActor;

	// Token: 0x040025AB RID: 9643
	private Actor m_ghostMinionActor;

	// Token: 0x040025AC RID: 9644
	private Actor m_ghostSpellActor;

	// Token: 0x040025AD RID: 9645
	private Actor m_templateWeaponActor;

	// Token: 0x040025AE RID: 9646
	private Actor m_templateSpellActor;

	// Token: 0x040025AF RID: 9647
	private Actor m_templateMinionActor;

	// Token: 0x040025B0 RID: 9648
	private Actor m_templateHeroSkinActor;

	// Token: 0x040025B1 RID: 9649
	private Actor m_hiddenActor;

	// Token: 0x040025B2 RID: 9650
	private CardInfoPane m_cardInfoPane;

	// Token: 0x040025B3 RID: 9651
	private Actor m_templateGoldenWeaponActor;

	// Token: 0x040025B4 RID: 9652
	private Actor m_templateGoldenSpellActor;

	// Token: 0x040025B5 RID: 9653
	private Actor m_templateGoldenMinionActor;

	// Token: 0x040025B6 RID: 9654
	private Actor m_ghostGoldenWeaponActor;

	// Token: 0x040025B7 RID: 9655
	private Actor m_ghostGoldenSpellActor;

	// Token: 0x040025B8 RID: 9656
	private Actor m_ghostGoldenMinionActor;

	// Token: 0x040025B9 RID: 9657
	private bool m_cancellingCraftMode;

	// Token: 0x040025BA RID: 9658
	private int m_transactions;

	// Token: 0x040025BB RID: 9659
	private long m_arcaneDustBalance;

	// Token: 0x040025BC RID: 9660
	private PendingTransaction m_pendingTransaction;

	// Token: 0x040025BD RID: 9661
	private Vector3 m_craftSourcePosition;

	// Token: 0x040025BE RID: 9662
	private Vector3 m_craftSourceScale;
}
