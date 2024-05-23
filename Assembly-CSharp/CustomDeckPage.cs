using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200037E RID: 894
public class CustomDeckPage : MonoBehaviour
{
	// Token: 0x06002D85 RID: 11653 RVA: 0x000E4298 File Offset: 0x000E2498
	public void Show()
	{
		base.GetComponent<Renderer>().enabled = true;
		for (int i = 0; i < this.m_numCustomDecks; i++)
		{
			if (i < this.m_customDecks.Count)
			{
				this.m_customDecks[i].Show();
			}
		}
	}

	// Token: 0x06002D86 RID: 11654 RVA: 0x000E42EC File Offset: 0x000E24EC
	public void Hide()
	{
		base.GetComponent<Renderer>().enabled = false;
		for (int i = 0; i < this.m_numCustomDecks; i++)
		{
			if (i < this.m_customDecks.Count)
			{
				this.m_customDecks[i].Hide();
			}
		}
	}

	// Token: 0x06002D87 RID: 11655 RVA: 0x000E433E File Offset: 0x000E253E
	public bool PageReady()
	{
		return this.m_customTrayStandardTexture != null && this.m_customTrayWildTexture != null && this.AreAllCustomDecksReady();
	}

	// Token: 0x06002D88 RID: 11656 RVA: 0x000E436C File Offset: 0x000E256C
	public CollectionDeckBoxVisual GetDeckboxWithDeckID(long deckID)
	{
		if (deckID <= 0L)
		{
			return null;
		}
		foreach (CollectionDeckBoxVisual collectionDeckBoxVisual in this.m_customDecks)
		{
			if (collectionDeckBoxVisual.GetDeckID() == deckID)
			{
				return collectionDeckBoxVisual;
			}
		}
		return null;
	}

	// Token: 0x06002D89 RID: 11657 RVA: 0x000E43E0 File Offset: 0x000E25E0
	public void UpdateTrayTransitionValue(float transitionValue)
	{
		Material material = base.GetComponent<Renderer>().material;
		material.SetFloat("_Transistion", transitionValue);
		foreach (GameObject gameObject in this.m_deckCovers)
		{
			Renderer componentInChildren = gameObject.GetComponentInChildren<Renderer>();
			if (componentInChildren != null)
			{
				componentInChildren.material.SetFloat("_Transistion", transitionValue);
			}
		}
	}

	// Token: 0x06002D8A RID: 11658 RVA: 0x000E4470 File Offset: 0x000E2670
	public void PlayVineGlowBurst(bool useFX)
	{
		if (this.m_vineGlowBurst != null)
		{
			string text = (!useFX) ? "GlowVinesNoFX" : "GlowVines";
			this.m_vineGlowBurst.SendEvent(text);
		}
	}

	// Token: 0x06002D8B RID: 11659 RVA: 0x000E44B0 File Offset: 0x000E26B0
	public void SetTrayTextures(Texture standardTexture, Texture wildTexture)
	{
		Material material = base.GetComponent<Renderer>().material;
		material.mainTexture = standardTexture;
		material.SetTexture("_MainTex2", wildTexture);
		this.m_customTrayStandardTexture = standardTexture;
		this.m_customTrayWildTexture = wildTexture;
		this.InitCustomDecks();
	}

	// Token: 0x06002D8C RID: 11660 RVA: 0x000E44F0 File Offset: 0x000E26F0
	public void SetDecks(List<CollectionDeck> decks)
	{
		this.m_collectionDecks = decks;
	}

	// Token: 0x06002D8D RID: 11661 RVA: 0x000E44F9 File Offset: 0x000E26F9
	public void SetDeckButtonCallback(CustomDeckPage.DeckButtonCallback callback)
	{
		this.m_deckButtonCallback = callback;
	}

	// Token: 0x06002D8E RID: 11662 RVA: 0x000E4504 File Offset: 0x000E2704
	public void EnableDeckButtons(bool enable)
	{
		foreach (CollectionDeckBoxVisual collectionDeckBoxVisual in this.m_customDecks)
		{
			collectionDeckBoxVisual.SetEnabled(enable);
			if (!enable)
			{
				collectionDeckBoxVisual.SetHighlightState(ActorStateType.HIGHLIGHT_OFF);
			}
		}
	}

	// Token: 0x06002D8F RID: 11663 RVA: 0x000E456C File Offset: 0x000E276C
	public void TransitionWildDecks()
	{
		int num = 0;
		foreach (CollectionDeck collectionDeck in this.m_collectionDecks)
		{
			if (collectionDeck.Type == 1)
			{
				CollectionDeckBoxVisual collectionDeckBoxVisual = this.m_customDecks[num];
				if (collectionDeck.IsWild)
				{
					collectionDeckBoxVisual.TransitionFromStandardToWild();
				}
				num++;
			}
		}
	}

	// Token: 0x06002D90 RID: 11664 RVA: 0x000E45F4 File Offset: 0x000E27F4
	public void UpdateDeckVisuals(bool isWild, bool checkFormat, bool forceStandardVisuals)
	{
		int i = 0;
		this.m_numCustomDecks = 0;
		foreach (CollectionDeck collectionDeck in this.m_collectionDecks)
		{
			if (collectionDeck.Type == 1)
			{
				this.m_numCustomDecks++;
				bool flag = collectionDeck.IsTourneyValid;
				if (checkFormat)
				{
					flag = (flag && (collectionDeck.IsValidForFormat(isWild) || forceStandardVisuals));
				}
				CollectionDeckBoxVisual collectionDeckBoxVisual = this.m_customDecks[i];
				collectionDeckBoxVisual.SetDeckName(collectionDeck.Name);
				collectionDeckBoxVisual.SetDeckID(collectionDeck.ID);
				collectionDeckBoxVisual.SetHeroCardID(collectionDeck.HeroCardID);
				collectionDeckBoxVisual.SetIsMissingCards(!collectionDeck.IsTourneyValid);
				collectionDeckBoxVisual.SetIsValid(flag);
				collectionDeckBoxVisual.SetFormat(collectionDeck.IsWild && !forceStandardVisuals);
				Log.Kyle.Print("InitCustomDecks - is Hero Skin: {0}", new object[]
				{
					GameUtils.GetCardSetFromCardID(collectionDeck.HeroCardID) == TAG_CARD_SET.HERO_SKINS
				});
				collectionDeckBoxVisual.Show();
				GameObject gameObject = this.m_deckCovers[i];
				gameObject.SetActive(false);
				i++;
				if (i >= this.m_customDecks.Count)
				{
					break;
				}
			}
		}
		while (i < this.m_customDecks.Count)
		{
			this.m_customDecks[i].Hide();
			GameObject gameObject2 = this.m_deckCovers[i];
			gameObject2.SetActive(true);
			i++;
		}
	}

	// Token: 0x06002D91 RID: 11665 RVA: 0x000E47AC File Offset: 0x000E29AC
	public bool HasWildDeck()
	{
		foreach (CollectionDeck collectionDeck in this.m_collectionDecks)
		{
			if (collectionDeck.IsWild)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D92 RID: 11666 RVA: 0x000E4814 File Offset: 0x000E2A14
	private bool AreAllCustomDecksReady()
	{
		foreach (CollectionDeckBoxVisual collectionDeckBoxVisual in this.m_customDecks)
		{
			if (collectionDeckBoxVisual.GetFullDef() == null && collectionDeckBoxVisual.GetDeckID() > 0L)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x000E488C File Offset: 0x000E2A8C
	private void InitCustomDecks()
	{
		int i = 0;
		Vector3 customDeckStart = this.m_customDeckStart;
		float customDeckHorizontalSpacing = this.m_customDeckHorizontalSpacing;
		float customDeckVerticalSpacing = this.m_customDeckVerticalSpacing;
		while (i < 9)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "DeckParent" + i;
			gameObject.transform.parent = base.gameObject.transform;
			if (i == 0)
			{
				gameObject.transform.localPosition = customDeckStart;
			}
			else
			{
				float num = customDeckStart.x - (float)(i % 3) * customDeckHorizontalSpacing;
				float num2 = (float)Mathf.CeilToInt((float)(i / 3)) * customDeckVerticalSpacing + customDeckStart.z;
				gameObject.transform.localPosition = new Vector3(num, customDeckStart.y, num2);
			}
			CollectionDeckBoxVisual deckBox = Object.Instantiate<CollectionDeckBoxVisual>(this.m_deckboxPrefab);
			CollectionDeckBoxVisual deckBox2 = deckBox;
			deckBox2.name = deckBox2.name + " - " + i;
			deckBox.transform.parent = gameObject.transform;
			deckBox.transform.localPosition = Vector3.zero;
			deckBox.SetOriginalButtonPosition();
			gameObject.transform.localScale = this.m_customDeckScale;
			if (this.m_deckButtonCallback == null)
			{
				Debug.LogError("SetDeckButtonCallback() not called in CustomDeckPage!");
			}
			else
			{
				deckBox.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
				{
					this.OnSelectCustomDeck(deckBox);
				});
			}
			deckBox.SetEnabled(!DeckPickerTrayDisplay.Get().IsMissingStandardDeckTrayShown());
			this.m_customDecks.Add(deckBox);
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.m_deckboxCoverPrefab);
			gameObject2.transform.parent = base.gameObject.transform;
			gameObject2.transform.localScale = this.m_customDeckScale;
			gameObject2.transform.position = gameObject.transform.position + this.m_deckCoverOffset;
			Material material = gameObject2.GetComponentInChildren<Renderer>().material;
			material.mainTexture = this.m_customTrayStandardTexture;
			material.SetTexture("_MainTex2", this.m_customTrayWildTexture);
			this.m_deckCovers.Add(gameObject2);
			i++;
		}
		if (this.m_collectionDecks == null)
		{
			Debug.LogErrorFormat("m_collectionDecks not set in CustomDeckPage! Ensure that SetDecks() is called before this method!", new object[0]);
			return;
		}
		bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
		bool checkFormat = SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT || SceneMgr.Get().GetMode() == SceneMgr.Mode.FRIENDLY;
		this.UpdateDeckVisuals(@bool, checkFormat, false);
	}

	// Token: 0x06002D94 RID: 11668 RVA: 0x000E4B2C File Offset: 0x000E2D2C
	private void OnSelectCustomDeck(CollectionDeckBoxVisual deckbox)
	{
		bool @bool = Options.Get().GetBool(Option.IN_WILD_MODE);
		if (deckbox.IsWild() && !@bool)
		{
			DeckPickerTrayDisplay.Get().ShowClickedWildDeckInStandardPopup();
		}
		this.m_deckButtonCallback(deckbox, true);
	}

	// Token: 0x04001C69 RID: 7273
	public const int MAX_CUSTOM_DECKS_TO_DISPLAY = 9;

	// Token: 0x04001C6A RID: 7274
	public Vector3 m_customDeckStart;

	// Token: 0x04001C6B RID: 7275
	public Vector3 m_customDeckScale;

	// Token: 0x04001C6C RID: 7276
	public float m_customDeckHorizontalSpacing;

	// Token: 0x04001C6D RID: 7277
	public float m_customDeckVerticalSpacing;

	// Token: 0x04001C6E RID: 7278
	public CollectionDeckBoxVisual m_deckboxPrefab;

	// Token: 0x04001C6F RID: 7279
	public Vector3 m_deckCoverOffset;

	// Token: 0x04001C70 RID: 7280
	public GameObject m_deckboxCoverPrefab;

	// Token: 0x04001C71 RID: 7281
	public PlayMakerFSM m_vineGlowBurst;

	// Token: 0x04001C72 RID: 7282
	private List<GameObject> m_deckCovers = new List<GameObject>();

	// Token: 0x04001C73 RID: 7283
	private List<CollectionDeck> m_collectionDecks;

	// Token: 0x04001C74 RID: 7284
	private List<CollectionDeckBoxVisual> m_customDecks = new List<CollectionDeckBoxVisual>();

	// Token: 0x04001C75 RID: 7285
	private Texture m_customTrayStandardTexture;

	// Token: 0x04001C76 RID: 7286
	private Texture m_customTrayWildTexture;

	// Token: 0x04001C77 RID: 7287
	private int m_numCustomDecks;

	// Token: 0x04001C78 RID: 7288
	private CustomDeckPage.DeckButtonCallback m_deckButtonCallback;

	// Token: 0x0200039E RID: 926
	// (Invoke) Token: 0x0600309C RID: 12444
	public delegate void DeckButtonCallback(CollectionDeckBoxVisual deckbox, bool showTrayForPhone = true);
}
