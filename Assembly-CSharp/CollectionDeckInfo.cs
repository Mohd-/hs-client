using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200075A RID: 1882
public class CollectionDeckInfo : MonoBehaviour
{
	// Token: 0x06004BAD RID: 19373 RVA: 0x0016910C File Offset: 0x0016730C
	private void Awake()
	{
		this.m_manaCurveTooltipText.Text = GameStrings.Get("GLUE_COLLECTION_DECK_INFO_MANA_TOOLTIP");
		foreach (DeckInfoManaBar deckInfoManaBar in this.m_manaBars)
		{
			deckInfoManaBar.m_costText.Text = this.GetTextForManaCost(deckInfoManaBar.m_manaCostID);
		}
		AssetLoader.Get().LoadActor("Card_Play_HeroPower", new AssetLoader.GameObjectCallback(this.OnHeroPowerActorLoaded), null, false);
		AssetLoader.Get().LoadActor(ActorNames.GetNameWithPremiumType("Card_Play_HeroPower", TAG_PREMIUM.GOLDEN), new AssetLoader.GameObjectCallback(this.OnGoldenHeroPowerActorLoaded), null, false);
		this.m_wasTouchModeEnabled = true;
	}

	// Token: 0x06004BAE RID: 19374 RVA: 0x001691D4 File Offset: 0x001673D4
	private void Start()
	{
		this.m_offClicker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClosePressed));
		this.m_offClicker.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OverOffClicker));
	}

	// Token: 0x06004BAF RID: 19375 RVA: 0x00169214 File Offset: 0x00167414
	private void Update()
	{
		if (this.m_wasTouchModeEnabled != UniversalInputManager.Get().IsTouchMode())
		{
			this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
			if (UniversalInputManager.Get().IsTouchMode())
			{
				if (this.m_heroPowerActor != null)
				{
					this.m_heroPowerActor.TurnOffCollider();
				}
				if (this.m_goldenHeroPowerActor != null)
				{
					this.m_goldenHeroPowerActor.TurnOffCollider();
				}
				this.m_offClicker.gameObject.SetActive(true);
			}
			else
			{
				if (this.m_heroPowerActor != null)
				{
					this.m_heroPowerActor.TurnOnCollider();
				}
				if (this.m_goldenHeroPowerActor != null)
				{
					this.m_goldenHeroPowerActor.TurnOnCollider();
				}
				this.m_offClicker.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06004BB0 RID: 19376 RVA: 0x001692EC File Offset: 0x001674EC
	public void Show()
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_root.SetActive(true);
		this.m_shown = true;
		if (UniversalInputManager.Get().IsTouchMode())
		{
			Navigation.Push(new Navigation.NavigateBackHandler(this.GoBackImpl));
		}
		CollectionDeckInfo.ShowListener[] array = this.m_showListeners.ToArray();
		foreach (CollectionDeckInfo.ShowListener showListener in array)
		{
			showListener();
		}
	}

	// Token: 0x06004BB1 RID: 19377 RVA: 0x00169364 File Offset: 0x00167564
	private bool GoBackImpl()
	{
		this.Hide();
		return true;
	}

	// Token: 0x06004BB2 RID: 19378 RVA: 0x00169370 File Offset: 0x00167570
	public void Hide()
	{
		Navigation.PopUnique(new Navigation.NavigateBackHandler(this.GoBackImpl));
		if (!this.m_shown)
		{
			return;
		}
		this.m_root.SetActive(false);
		this.m_shown = false;
		CollectionDeckInfo.HideListener[] array = this.m_hideListeners.ToArray();
		foreach (CollectionDeckInfo.HideListener hideListener in array)
		{
			hideListener();
		}
	}

	// Token: 0x06004BB3 RID: 19379 RVA: 0x001693D9 File Offset: 0x001675D9
	public void RegisterShowListener(CollectionDeckInfo.ShowListener dlg)
	{
		this.m_showListeners.Add(dlg);
	}

	// Token: 0x06004BB4 RID: 19380 RVA: 0x001693E7 File Offset: 0x001675E7
	public void UnregisterShowListener(CollectionDeckInfo.ShowListener dlg)
	{
		this.m_showListeners.Remove(dlg);
	}

	// Token: 0x06004BB5 RID: 19381 RVA: 0x001693F6 File Offset: 0x001675F6
	public void RegisterHideListener(CollectionDeckInfo.HideListener dlg)
	{
		this.m_hideListeners.Add(dlg);
	}

	// Token: 0x06004BB6 RID: 19382 RVA: 0x00169404 File Offset: 0x00167604
	public void UnregisterHideListener(CollectionDeckInfo.HideListener dlg)
	{
		this.m_hideListeners.Remove(dlg);
	}

	// Token: 0x06004BB7 RID: 19383 RVA: 0x00169413 File Offset: 0x00167613
	public bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x06004BB8 RID: 19384 RVA: 0x0016941C File Offset: 0x0016761C
	public void UpdateManaCurve()
	{
		CollectionDeck editingDeck = CollectionDeckTray.Get().GetCardsContent().GetEditingDeck();
		this.UpdateManaCurve(editingDeck);
	}

	// Token: 0x06004BB9 RID: 19385 RVA: 0x00169440 File Offset: 0x00167640
	public void UpdateManaCurve(CollectionDeck deck)
	{
		if (deck == null)
		{
			Debug.LogWarning(string.Format("CollectionDeckInfo.UpdateManaCurve(): deck is null.", new object[0]));
			return;
		}
		string heroCardID = deck.HeroCardID;
		CardPortraitQuality quality = new CardPortraitQuality(3, TAG_PREMIUM.NORMAL);
		DefLoader.Get().LoadCardDef(heroCardID, new DefLoader.LoadDefCallback<CardDef>(this.OnHeroCardDefLoaded), new object(), quality);
		foreach (DeckInfoManaBar deckInfoManaBar in this.m_manaBars)
		{
			deckInfoManaBar.m_numCards = 0;
		}
		int num = 0;
		List<CollectionDeckSlot> slots = deck.GetSlots();
		foreach (CollectionDeckSlot collectionDeckSlot in slots)
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(collectionDeckSlot.CardID);
			int manaCost = entityDef.GetCost();
			if (manaCost > this.MAX_MANA_COST_ID)
			{
				manaCost = this.MAX_MANA_COST_ID;
			}
			DeckInfoManaBar deckInfoManaBar2 = this.m_manaBars.Find((DeckInfoManaBar obj) => obj.m_manaCostID == manaCost);
			if (deckInfoManaBar2 == null)
			{
				Debug.LogWarning(string.Format("CollectionDeckInfo.UpdateManaCurve(): Cannot update curve. Could not find mana bar for {0} (cost {1})", entityDef, manaCost));
				return;
			}
			deckInfoManaBar2.m_numCards += collectionDeckSlot.Count;
			if (deckInfoManaBar2.m_numCards > num)
			{
				num = deckInfoManaBar2.m_numCards;
			}
		}
		foreach (DeckInfoManaBar deckInfoManaBar3 in this.m_manaBars)
		{
			deckInfoManaBar3.m_numCardsText.Text = Convert.ToString(deckInfoManaBar3.m_numCards);
			float num2 = (num != 0) ? ((float)deckInfoManaBar3.m_numCards / (float)num) : 0f;
			Vector3 localPosition = deckInfoManaBar3.m_numCardsText.transform.localPosition;
			localPosition.z = Mathf.Lerp(this.MANA_COST_TEXT_MIN_LOCAL_Z, this.MANA_COST_TEXT_MAX_LOCAL_Z, num2);
			deckInfoManaBar3.m_numCardsText.transform.localPosition = localPosition;
			deckInfoManaBar3.m_barFill.GetComponent<Renderer>().material.SetFloat("_Percent", num2);
		}
	}

	// Token: 0x06004BBA RID: 19386 RVA: 0x001696C0 File Offset: 0x001678C0
	public void SetDeck(CollectionDeck deck)
	{
		if (deck == null)
		{
			Debug.LogWarning(string.Format("CollectionDeckInfo.SetDeckID(): deck is null", new object[0]));
			return;
		}
		this.UpdateManaCurve(deck);
		string heroPowerCardIdFromHero = GameUtils.GetHeroPowerCardIdFromHero(deck.HeroCardID);
		if (string.IsNullOrEmpty(heroPowerCardIdFromHero))
		{
			Debug.LogWarning("CollectionDeckInfo.UpdateInfo(): invalid hero power ID");
			this.m_heroPowerID = string.Empty;
			return;
		}
		if (heroPowerCardIdFromHero.Equals(this.m_heroPowerID))
		{
			return;
		}
		this.m_heroPowerID = heroPowerCardIdFromHero;
		string vanillaHeroCardIDFromClass = CollectionManager.Get().GetVanillaHeroCardIDFromClass(deck.GetClass());
		TAG_PREMIUM bestCardPremium = CollectionManager.Get().GetBestCardPremium(vanillaHeroCardIDFromClass);
		DefLoader.Get().LoadFullDef(this.m_heroPowerID, new DefLoader.LoadDefCallback<FullDef>(this.OnHeroPowerFullDefLoaded), bestCardPremium);
	}

	// Token: 0x06004BBB RID: 19387 RVA: 0x00169778 File Offset: 0x00167978
	private string GetTextForManaCost(int manaCostID)
	{
		if (manaCostID < this.MIN_MANA_COST_ID || manaCostID > this.MAX_MANA_COST_ID)
		{
			Debug.LogWarning(string.Format("CollectionDeckInfo.GetTextForManaCost(): don't know how to handle mana cost ID {0}", manaCostID));
			return string.Empty;
		}
		string text = Convert.ToString(manaCostID);
		if (manaCostID == this.MAX_MANA_COST_ID)
		{
			text += GameStrings.Get("GLUE_COLLECTION_PLUS");
		}
		return text;
	}

	// Token: 0x06004BBC RID: 19388 RVA: 0x001697E0 File Offset: 0x001679E0
	private void OnHeroPowerActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("CollectionDeckInfo.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		this.m_heroPowerActor = actorObject.GetComponent<Actor>();
		if (this.m_heroPowerActor == null)
		{
			Debug.LogWarning(string.Format("CollectionDeckInfo.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		this.m_heroPowerActor.SetUnlit();
		this.m_heroPowerActor.transform.parent = this.m_heroPowerParent.transform;
		this.m_heroPowerActor.transform.localScale = Vector3.one;
		this.m_heroPowerActor.transform.localPosition = Vector3.zero;
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.m_heroPowerActor.TurnOffCollider();
		}
	}

	// Token: 0x06004BBD RID: 19389 RVA: 0x001698A4 File Offset: 0x00167AA4
	private void OnGoldenHeroPowerActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("CollectionDeckInfo.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", actorName));
			return;
		}
		this.m_goldenHeroPowerActor = actorObject.GetComponent<Actor>();
		if (this.m_goldenHeroPowerActor == null)
		{
			Debug.LogWarning(string.Format("CollectionDeckInfo.OnGoldenHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", actorName));
			return;
		}
		this.m_goldenHeroPowerActor.SetUnlit();
		this.m_goldenHeroPowerActor.transform.parent = this.m_heroPowerParent.transform;
		this.m_goldenHeroPowerActor.transform.localScale = Vector3.one;
		this.m_goldenHeroPowerActor.transform.localPosition = Vector3.zero;
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.m_goldenHeroPowerActor.TurnOffCollider();
		}
	}

	// Token: 0x06004BBE RID: 19390 RVA: 0x00169968 File Offset: 0x00167B68
	private void OnHeroPowerFullDefLoaded(string cardID, FullDef def, object userData)
	{
		if (this.m_heroPowerActor != null)
		{
			this.SetHeroPowerInfo(cardID, def, (TAG_PREMIUM)((int)userData));
			return;
		}
		base.StartCoroutine(this.SetHeroPowerInfoWhenReady(cardID, def, (TAG_PREMIUM)((int)userData)));
	}

	// Token: 0x06004BBF RID: 19391 RVA: 0x001699AC File Offset: 0x00167BAC
	private IEnumerator SetHeroPowerInfoWhenReady(string heroPowerCardID, FullDef def, TAG_PREMIUM premium)
	{
		while (this.m_heroPowerActor == null)
		{
			yield return null;
		}
		this.SetHeroPowerInfo(heroPowerCardID, def, premium);
		yield break;
	}

	// Token: 0x06004BC0 RID: 19392 RVA: 0x001699F4 File Offset: 0x00167BF4
	private void SetHeroPowerInfo(string heroPowerCardID, FullDef def, TAG_PREMIUM premium)
	{
		if (!heroPowerCardID.Equals(this.m_heroPowerID))
		{
			return;
		}
		EntityDef entityDef = def.GetEntityDef();
		if (premium == TAG_PREMIUM.GOLDEN)
		{
			this.m_heroPowerActor.Hide();
			this.m_goldenHeroPowerActor.Show();
			this.m_goldenHeroPowerActor.SetEntityDef(def.GetEntityDef());
			this.m_goldenHeroPowerActor.SetCardDef(def.GetCardDef());
			this.m_goldenHeroPowerActor.SetUnlit();
			this.m_goldenHeroPowerActor.SetPremium(premium);
			this.m_goldenHeroPowerActor.UpdateAllComponents();
		}
		else
		{
			this.m_heroPowerActor.Show();
			this.m_goldenHeroPowerActor.Hide();
			this.m_heroPowerActor.SetEntityDef(def.GetEntityDef());
			this.m_heroPowerActor.SetCardDef(def.GetCardDef());
			this.m_heroPowerActor.SetUnlit();
			this.m_heroPowerActor.UpdateAllComponents();
		}
		string name = entityDef.GetName();
		this.m_heroPowerName.Text = name;
		string cardTextInHand = entityDef.GetCardTextInHand();
		this.m_heroPowerDescription.Text = cardTextInHand;
	}

	// Token: 0x06004BC1 RID: 19393 RVA: 0x00169AF4 File Offset: 0x00167CF4
	private void OnHeroCardDefLoaded(string cardId, CardDef def, object userData)
	{
	}

	// Token: 0x06004BC2 RID: 19394 RVA: 0x00169AF6 File Offset: 0x00167CF6
	private void OnClosePressed(UIEvent e)
	{
		this.Hide();
	}

	// Token: 0x06004BC3 RID: 19395 RVA: 0x00169AFE File Offset: 0x00167CFE
	private void OverOffClicker(UIEvent e)
	{
		this.Hide();
	}

	// Token: 0x0400328A RID: 12938
	public GameObject m_root;

	// Token: 0x0400328B RID: 12939
	public GameObject m_heroPowerParent;

	// Token: 0x0400328C RID: 12940
	public UberText m_heroPowerName;

	// Token: 0x0400328D RID: 12941
	public UberText m_heroPowerDescription;

	// Token: 0x0400328E RID: 12942
	public UberText m_manaCurveTooltipText;

	// Token: 0x0400328F RID: 12943
	public PegUIElement m_offClicker;

	// Token: 0x04003290 RID: 12944
	public List<DeckInfoManaBar> m_manaBars;

	// Token: 0x04003291 RID: 12945
	private readonly int MIN_MANA_COST_ID;

	// Token: 0x04003292 RID: 12946
	private readonly int MAX_MANA_COST_ID = 7;

	// Token: 0x04003293 RID: 12947
	private readonly float MANA_COST_TEXT_MIN_LOCAL_Z;

	// Token: 0x04003294 RID: 12948
	private readonly float MANA_COST_TEXT_MAX_LOCAL_Z = 5.167298f;

	// Token: 0x04003295 RID: 12949
	private Actor m_heroPowerActor;

	// Token: 0x04003296 RID: 12950
	private Actor m_goldenHeroPowerActor;

	// Token: 0x04003297 RID: 12951
	private bool m_wasTouchModeEnabled;

	// Token: 0x04003298 RID: 12952
	protected bool m_shown = true;

	// Token: 0x04003299 RID: 12953
	private string m_heroPowerID = string.Empty;

	// Token: 0x0400329A RID: 12954
	private List<CollectionDeckInfo.ShowListener> m_showListeners = new List<CollectionDeckInfo.ShowListener>();

	// Token: 0x0400329B RID: 12955
	private List<CollectionDeckInfo.HideListener> m_hideListeners = new List<CollectionDeckInfo.HideListener>();

	// Token: 0x0200075C RID: 1884
	// (Invoke) Token: 0x06004BDD RID: 19421
	public delegate void HideListener();

	// Token: 0x0200075E RID: 1886
	// (Invoke) Token: 0x06004BE2 RID: 19426
	public delegate void ShowListener();
}
