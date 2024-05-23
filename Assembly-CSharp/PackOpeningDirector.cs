using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A88 RID: 2696
[CustomEditClass]
public class PackOpeningDirector : MonoBehaviour
{
	// Token: 0x06005DA9 RID: 23977 RVA: 0x001C13BE File Offset: 0x001BF5BE
	private void Awake()
	{
		this.InitializeCards();
		this.InitializeUI();
	}

	// Token: 0x06005DAA RID: 23978 RVA: 0x001C13CC File Offset: 0x001BF5CC
	private void Update()
	{
		if (this.m_Carousel)
		{
			this.m_Carousel.UpdateUI(UniversalInputManager.Get().GetMouseButtonDown(0));
		}
	}

	// Token: 0x06005DAB RID: 23979 RVA: 0x001C1400 File Offset: 0x001BF600
	public void Play(int boosterId)
	{
		if (this.m_playing)
		{
			return;
		}
		this.m_playing = true;
		this.EnableCardInput(false);
		CollectionManager.Get().RegisterAchievesCompletedListener(new CollectionManager.DelOnAchievesCompleted(this.OnCollectionAchievesCompleted));
		base.StartCoroutine(this.PlayWhenReady(boosterId));
	}

	// Token: 0x06005DAC RID: 23980 RVA: 0x001C144B File Offset: 0x001BF64B
	public bool IsPlaying()
	{
		return this.m_playing;
	}

	// Token: 0x06005DAD RID: 23981 RVA: 0x001C1454 File Offset: 0x001BF654
	public void OnBoosterOpened(List<NetCache.BoosterCard> cards)
	{
		if (cards.Count > this.m_hiddenCards.Count)
		{
			Debug.LogError(string.Format("PackOpeningDirector.OnBoosterOpened() - Not enough PackOpeningCards! Received {0} cards. There are only {1} hidden cards.", cards.Count, this.m_hiddenCards.Count));
			return;
		}
		int cardsPendingReveal = Mathf.Min(cards.Count, this.m_hiddenCards.Count);
		this.m_cardsPendingReveal = cardsPendingReveal;
		base.StartCoroutine(this.AttachBoosterCards(cards));
		CollectionManager.Get().OnBoosterOpened(cards);
	}

	// Token: 0x06005DAE RID: 23982 RVA: 0x001C14D9 File Offset: 0x001BF6D9
	public void AddFinishedListener(PackOpeningDirector.FinishedCallback callback)
	{
		this.AddFinishedListener(callback, null);
	}

	// Token: 0x06005DAF RID: 23983 RVA: 0x001C14E4 File Offset: 0x001BF6E4
	public void AddFinishedListener(PackOpeningDirector.FinishedCallback callback, object userData)
	{
		PackOpeningDirector.FinishedListener finishedListener = new PackOpeningDirector.FinishedListener();
		finishedListener.SetCallback(callback);
		finishedListener.SetUserData(userData);
		this.m_finishedListeners.Add(finishedListener);
	}

	// Token: 0x06005DB0 RID: 23984 RVA: 0x001C1511 File Offset: 0x001BF711
	public void RemoveFinishedListener(PackOpeningDirector.FinishedCallback callback)
	{
		this.RemoveFinishedListener(callback, null);
	}

	// Token: 0x06005DB1 RID: 23985 RVA: 0x001C151C File Offset: 0x001BF71C
	public void RemoveFinishedListener(PackOpeningDirector.FinishedCallback callback, object userData)
	{
		PackOpeningDirector.FinishedListener finishedListener = new PackOpeningDirector.FinishedListener();
		finishedListener.SetCallback(callback);
		finishedListener.SetUserData(userData);
		this.m_finishedListeners.Remove(finishedListener);
	}

	// Token: 0x06005DB2 RID: 23986 RVA: 0x001C154A File Offset: 0x001BF74A
	public bool IsDoneButtonShown()
	{
		return this.m_doneButtonShown;
	}

	// Token: 0x06005DB3 RID: 23987 RVA: 0x001C1552 File Offset: 0x001BF752
	public List<PackOpeningCard> GetHiddenCards()
	{
		return this.m_hiddenCards;
	}

	// Token: 0x06005DB4 RID: 23988 RVA: 0x001C155C File Offset: 0x001BF75C
	public bool FinishPackOpen()
	{
		if (!this.m_doneButtonShown)
		{
			return false;
		}
		this.m_activePackFxSpell.ActivateState(SpellStateType.DEATH);
		Spell blurSpell = Box.Get().GetBoxCamera().GetEventTable().m_BlurSpell;
		blurSpell.ActivateState(SpellStateType.DEATH);
		this.m_effectsPendingFinish = 1 + 2 * this.m_hiddenCards.Count;
		this.m_effectsPendingDestroy = this.m_effectsPendingFinish;
		this.HideDoneButton();
		foreach (PackOpeningCard packOpeningCard in this.m_hiddenCards)
		{
			CardBackDisplay componentInChildren = packOpeningCard.GetComponentInChildren<CardBackDisplay>();
			if (componentInChildren != null)
			{
				componentInChildren.gameObject.GetComponent<Renderer>().enabled = false;
			}
			Spell classNameSpell = packOpeningCard.m_ClassNameSpell;
			classNameSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnHiddenCardSpellFinished));
			classNameSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnHiddenCardSpellStateFinished));
			classNameSpell.ActivateState(SpellStateType.DEATH);
			Spell isNewSpell = packOpeningCard.m_IsNewSpell;
			if (isNewSpell != null)
			{
				isNewSpell.ActivateState(SpellStateType.DEATH);
			}
			Actor actor = packOpeningCard.GetActor();
			Spell spell = actor.GetSpell(SpellType.DEATH);
			spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnHiddenCardSpellFinished));
			spell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnHiddenCardSpellStateFinished));
			spell.Activate();
		}
		this.HideKeywordTooltips();
		return true;
	}

	// Token: 0x06005DB5 RID: 23989 RVA: 0x001C16C8 File Offset: 0x001BF8C8
	private IEnumerator PlayWhenReady(int boosterId)
	{
		while (this.m_loadingDoneButton)
		{
			yield return null;
		}
		if (this.m_doneButton == null)
		{
			this.FireFinishedEvent();
			yield break;
		}
		Spell spell;
		if (!this.m_packFxSpells.TryGetValue(boosterId, out spell))
		{
			BoosterDbfRecord record = GameDbf.Booster.GetRecord(boosterId);
			string originalAssetName = FileUtils.GameAssetPathToName(record.PackOpeningFxPrefab);
			string assetName;
			if (UniversalInputManager.UsePhoneUI)
			{
				assetName = string.Format("{0}_phone", originalAssetName);
			}
			else
			{
				assetName = originalAssetName;
			}
			bool loading = true;
			AssetLoader.GameObjectCallback callback = delegate(string name, GameObject go, object callbackData)
			{
				loading = false;
				this.m_packFxSpells[boosterId] = spell;
				if (go == null)
				{
					Error.AddDevFatal("PackOpeningDirector.PlayWhenReady() - Error loading {0} for booster id {1}", new object[]
					{
						name,
						boosterId
					});
					return;
				}
				spell = go.GetComponent<Spell>();
				go.transform.parent = base.transform;
				go.transform.localPosition = this.PACK_OPENING_FX_POSITION;
			};
			AssetLoader.Get().LoadGameObject(assetName, callback, null, false);
			while (loading)
			{
				yield return null;
			}
		}
		if (!spell)
		{
			this.FireFinishedEvent();
			yield break;
		}
		this.m_activePackFxSpell = spell;
		PlayMakerFSM fsm = spell.GetComponent<PlayMakerFSM>();
		if (fsm != null)
		{
			fsm.FsmVariables.GetFsmGameObject("CardsInsidePack").Value = this.m_CardsInsidePack;
			fsm.FsmVariables.GetFsmGameObject("ClassName").Value = this.m_ClassName;
			fsm.FsmVariables.GetFsmGameObject("PackOpeningDirector").Value = base.gameObject;
		}
		this.m_activePackFxSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished));
		this.m_activePackFxSpell.ActivateState(SpellStateType.ACTION);
		yield break;
	}

	// Token: 0x06005DB6 RID: 23990 RVA: 0x001C16F4 File Offset: 0x001BF8F4
	private void OnSpellFinished(Spell spell, object userData)
	{
		foreach (PackOpeningCard packOpeningCard in this.m_hiddenCards)
		{
			packOpeningCard.EnableInput(true);
			packOpeningCard.EnableReveal(true);
		}
		this.AttachCardsToCarousel();
	}

	// Token: 0x06005DB7 RID: 23991 RVA: 0x001C175C File Offset: 0x001BF95C
	private void CameraBlurOn()
	{
		Spell blurSpell = Box.Get().GetBoxCamera().GetEventTable().m_BlurSpell;
		blurSpell.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x06005DB8 RID: 23992 RVA: 0x001C1788 File Offset: 0x001BF988
	private void AttachCardsToCarousel()
	{
		if (!this.m_Carousel)
		{
			return;
		}
		List<PackOpeningCardCarouselItem> list = new List<PackOpeningCardCarouselItem>();
		for (int i = 0; i < this.m_hiddenCards.Count; i++)
		{
			PackOpeningCard packOpeningCard = this.m_hiddenCards[i];
			packOpeningCard.GetComponent<Collider>().enabled = true;
			PackOpeningCardCarouselItem packOpeningCardCarouselItem = new PackOpeningCardCarouselItem(packOpeningCard);
			list.Add(packOpeningCardCarouselItem);
		}
		this.m_Carousel.Initialize(list.ToArray(), 0);
		this.m_Carousel.SetListeners(null, new Carousel.ItemClicked(this.CarouselItemClicked), new Carousel.ItemReleased(this.CarouselItemReleased), new Carousel.CarouselSettled(this.CarouselSettled), new Carousel.CarouselStartedScrolling(this.CarouselStartedScrolling));
		this.CarouselSettled();
	}

	// Token: 0x06005DB9 RID: 23993 RVA: 0x001C1843 File Offset: 0x001BFA43
	private void CarouselItemClicked(CarouselItem item, int index)
	{
		this.m_clickedCard = item.GetGameObject().GetComponent<PackOpeningCard>();
		this.m_clickedPosition = index;
	}

	// Token: 0x06005DBA RID: 23994 RVA: 0x001C1860 File Offset: 0x001BFA60
	private void CarouselItemReleased()
	{
		if (!this.m_Carousel.IsScrolling())
		{
			if (this.m_clickedPosition == this.m_Carousel.GetCurrentIndex())
			{
				if (this.m_clickedCard.IsRevealed())
				{
					if (this.m_clickedPosition < 4)
					{
						this.m_Carousel.SetPosition(this.m_clickedPosition + 1, true);
					}
				}
				else
				{
					this.m_clickedCard.ForceReveal();
				}
			}
			else
			{
				this.m_Carousel.SetPosition(this.m_clickedPosition, true);
			}
		}
	}

	// Token: 0x06005DBB RID: 23995 RVA: 0x001C18EC File Offset: 0x001BFAEC
	private void CarouselSettled()
	{
		CarouselItem currentItem = this.m_Carousel.GetCurrentItem();
		PackOpeningCard component = ((PackOpeningCardCarouselItem)currentItem).GetGameObject().GetComponent<PackOpeningCard>();
		this.m_glowingCard = component;
		component.ShowRarityGlow();
	}

	// Token: 0x06005DBC RID: 23996 RVA: 0x001C1924 File Offset: 0x001BFB24
	private void CarouselStartedScrolling()
	{
		if (this.m_glowingCard != null && this.m_glowingCard.GetEntityDef().GetRarity() != TAG_RARITY.COMMON)
		{
			this.m_glowingCard.HideRarityGlow();
		}
	}

	// Token: 0x06005DBD RID: 23997 RVA: 0x001C1964 File Offset: 0x001BFB64
	private void InitializeUI()
	{
		this.m_loadingDoneButton = true;
		string cardName = FileUtils.GameAssetPathToName(this.m_DoneButtonPrefab);
		AssetLoader.Get().LoadActor(cardName, new AssetLoader.GameObjectCallback(this.OnDoneButtonLoaded), null, false);
	}

	// Token: 0x06005DBE RID: 23998 RVA: 0x001C19A0 File Offset: 0x001BFBA0
	private void OnDoneButtonLoaded(string name, GameObject actorObject, object userData)
	{
		this.m_loadingDoneButton = false;
		if (actorObject == null)
		{
			Debug.LogError(string.Format("PackOpeningDirector.OnDoneButtonLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		this.m_doneButton = actorObject.GetComponent<NormalButton>();
		if (this.m_doneButton == null)
		{
			Debug.LogError(string.Format("PackOpeningDirector.OnDoneButtonLoaded() - ERROR \"{0}\" has no {1} component", name, typeof(NormalButton)));
			return;
		}
		SceneUtils.SetLayer(this.m_doneButton.gameObject, GameLayer.IgnoreFullScreenEffects);
		this.m_doneButton.transform.parent = base.transform;
		TransformUtil.CopyWorld(this.m_doneButton, PackOpening.Get().m_Bones.m_DoneButton);
		SceneUtils.EnableRenderersAndColliders(this.m_doneButton.gameObject, false);
	}

	// Token: 0x06005DBF RID: 23999 RVA: 0x001C1A5C File Offset: 0x001BFC5C
	private void ShowDoneButton()
	{
		this.m_doneButtonShown = true;
		SceneUtils.EnableRenderersAndColliders(this.m_doneButton.gameObject, true);
		Spell component = this.m_doneButton.m_button.GetComponent<Spell>();
		component.AddFinishedCallback(new Spell.FinishedCallback(this.OnDoneButtonShown));
		component.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x06005DC0 RID: 24000 RVA: 0x001C1AAB File Offset: 0x001BFCAB
	private void OnDoneButtonShown(Spell spell, object userData)
	{
		this.m_doneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));
	}

	// Token: 0x06005DC1 RID: 24001 RVA: 0x001C1AC8 File Offset: 0x001BFCC8
	private void HideDoneButton()
	{
		this.m_doneButtonShown = false;
		SceneUtils.EnableColliders(this.m_doneButton.gameObject, false);
		this.m_doneButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonPressed));
		Spell component = this.m_doneButton.m_button.GetComponent<Spell>();
		component.AddFinishedCallback(new Spell.FinishedCallback(this.OnDoneButtonHidden));
		component.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x06005DC2 RID: 24002 RVA: 0x001C1B30 File Offset: 0x001BFD30
	private void OnDoneButtonHidden(Spell spell, object userData)
	{
		this.OnEffectFinished();
		this.OnEffectDone();
	}

	// Token: 0x06005DC3 RID: 24003 RVA: 0x001C1B40 File Offset: 0x001BFD40
	private void OnDoneButtonPressed(UIEvent e)
	{
		if (this.m_completeAchievesToDisplay.Count > 0)
		{
			this.ShowCompleteAchieve();
			return;
		}
		this.HideKeywordTooltips();
		this.FinishPackOpen();
	}

	// Token: 0x06005DC4 RID: 24004 RVA: 0x001C1B74 File Offset: 0x001BFD74
	private void HideKeywordTooltips()
	{
		foreach (PackOpeningCard packOpeningCard in this.m_hiddenCards)
		{
			packOpeningCard.RemoveOnOverWhileFlippedListeners();
		}
		KeywordHelpPanelManager.Get().HideKeywordHelp();
	}

	// Token: 0x06005DC5 RID: 24005 RVA: 0x001C1BD8 File Offset: 0x001BFDD8
	private void InitializeCards()
	{
		this.m_hiddenCards.Add(this.m_HiddenCard);
		for (int i = 1; i < 5; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.m_HiddenCard.gameObject);
			PackOpeningCard component = gameObject.GetComponent<PackOpeningCard>();
			component.transform.parent = this.m_HiddenCard.transform.parent;
			TransformUtil.CopyLocal(component, this.m_HiddenCard);
			this.m_hiddenCards.Add(component);
		}
		for (int j = 0; j < this.m_hiddenCards.Count; j++)
		{
			PackOpeningCard packOpeningCard = this.m_hiddenCards[j];
			packOpeningCard.name = string.Format("Card_Hidden{0}", j + 1);
			packOpeningCard.EnableInput(false);
			packOpeningCard.AddRevealedListener(new PackOpeningCard.RevealedCallback(this.OnCardRevealed), packOpeningCard);
		}
	}

	// Token: 0x06005DC6 RID: 24006 RVA: 0x001C1CB4 File Offset: 0x001BFEB4
	private void EnableCardInput(bool enable)
	{
		foreach (PackOpeningCard packOpeningCard in this.m_hiddenCards)
		{
			packOpeningCard.EnableInput(enable);
		}
	}

	// Token: 0x06005DC7 RID: 24007 RVA: 0x001C1D10 File Offset: 0x001BFF10
	private void OnCardRevealed(object userData)
	{
		PackOpeningCard packOpeningCard = (PackOpeningCard)userData;
		if (packOpeningCard.GetEntityDef().GetRarity() == TAG_RARITY.LEGENDARY)
		{
			if (packOpeningCard.GetActor().GetPremium() == TAG_PREMIUM.GOLDEN)
			{
				BnetPresenceMgr.Get().SetGameField(4U, packOpeningCard.GetCardId() + ",1");
			}
			else
			{
				BnetPresenceMgr.Get().SetGameField(4U, packOpeningCard.GetCardId() + ",0");
			}
		}
		this.m_cardsPendingReveal--;
		if (this.m_cardsPendingReveal > 0)
		{
			return;
		}
		this.ShowDoneButton();
	}

	// Token: 0x06005DC8 RID: 24008 RVA: 0x001C1DA4 File Offset: 0x001BFFA4
	private void OnHiddenCardSpellFinished(Spell spell, object userData)
	{
		this.OnEffectFinished();
	}

	// Token: 0x06005DC9 RID: 24009 RVA: 0x001C1DAC File Offset: 0x001BFFAC
	private void OnHiddenCardSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		this.OnEffectDone();
	}

	// Token: 0x06005DCA RID: 24010 RVA: 0x001C1DC0 File Offset: 0x001BFFC0
	private IEnumerator AttachBoosterCards(List<NetCache.BoosterCard> cards)
	{
		int minCardCount = Mathf.Min(cards.Count, this.m_hiddenCards.Count);
		for (int i = 0; i < minCardCount; i++)
		{
			yield return null;
			NetCache.BoosterCard boosterCard = cards[i];
			PackOpeningCard card = this.m_hiddenCards[i];
			card.AttachBoosterCard(boosterCard);
		}
		yield break;
	}

	// Token: 0x06005DCB RID: 24011 RVA: 0x001C1DE9 File Offset: 0x001BFFE9
	private void OnCollectionAchievesCompleted(List<Achievement> achievements)
	{
		this.m_completeAchievesToDisplay.AddRange(achievements);
	}

	// Token: 0x06005DCC RID: 24012 RVA: 0x001C1DF8 File Offset: 0x001BFFF8
	private void ShowCompleteAchieve()
	{
		if (this.m_completeAchievesToDisplay.Count == 0)
		{
			this.FinishPackOpen();
			return;
		}
		Achievement quest = this.m_completeAchievesToDisplay[0];
		this.m_completeAchievesToDisplay.RemoveAt(0);
		QuestToast.ShowQuestToast(UserAttentionBlocker.NONE, delegate(object userData)
		{
			this.ShowCompleteAchieve();
		}, true, quest);
	}

	// Token: 0x06005DCD RID: 24013 RVA: 0x001C1E4A File Offset: 0x001C004A
	private void OnEffectFinished()
	{
		this.m_effectsPendingFinish--;
		if (this.m_effectsPendingFinish > 0)
		{
			return;
		}
		this.FireFinishedEvent();
	}

	// Token: 0x06005DCE RID: 24014 RVA: 0x001C1E70 File Offset: 0x001C0070
	private void OnEffectDone()
	{
		this.m_effectsPendingDestroy--;
		if (this.m_effectsPendingDestroy > 0)
		{
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06005DCF RID: 24015 RVA: 0x001C1EA4 File Offset: 0x001C00A4
	private void FireFinishedEvent()
	{
		CollectionManager.Get().RemoveAchievesCompletedListener(new CollectionManager.DelOnAchievesCompleted(this.OnCollectionAchievesCompleted));
		PackOpeningDirector.FinishedListener[] array = this.m_finishedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire();
		}
	}

	// Token: 0x0400456F RID: 17775
	private readonly Vector3 PACK_OPENING_FX_POSITION = Vector3.zero;

	// Token: 0x04004570 RID: 17776
	public PackOpeningCard m_HiddenCard;

	// Token: 0x04004571 RID: 17777
	public GameObject m_CardsInsidePack;

	// Token: 0x04004572 RID: 17778
	public GameObject m_ClassName;

	// Token: 0x04004573 RID: 17779
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_DoneButtonPrefab;

	// Token: 0x04004574 RID: 17780
	public Carousel m_Carousel;

	// Token: 0x04004575 RID: 17781
	private List<PackOpeningCard> m_hiddenCards = new List<PackOpeningCard>();

	// Token: 0x04004576 RID: 17782
	private NormalButton m_doneButton;

	// Token: 0x04004577 RID: 17783
	private bool m_loadingDoneButton;

	// Token: 0x04004578 RID: 17784
	private bool m_playing;

	// Token: 0x04004579 RID: 17785
	private Map<int, Spell> m_packFxSpells = new Map<int, Spell>();

	// Token: 0x0400457A RID: 17786
	private Spell m_activePackFxSpell;

	// Token: 0x0400457B RID: 17787
	private int m_cardsPendingReveal;

	// Token: 0x0400457C RID: 17788
	private int m_effectsPendingFinish;

	// Token: 0x0400457D RID: 17789
	private int m_effectsPendingDestroy;

	// Token: 0x0400457E RID: 17790
	private List<PackOpeningDirector.FinishedListener> m_finishedListeners = new List<PackOpeningDirector.FinishedListener>();

	// Token: 0x0400457F RID: 17791
	private int m_centerCardIndex;

	// Token: 0x04004580 RID: 17792
	private List<Achievement> m_completeAchievesToDisplay = new List<Achievement>();

	// Token: 0x04004581 RID: 17793
	private bool m_doneButtonShown;

	// Token: 0x04004582 RID: 17794
	private PackOpeningCard m_clickedCard;

	// Token: 0x04004583 RID: 17795
	private int m_clickedPosition;

	// Token: 0x04004584 RID: 17796
	private PackOpeningCard m_glowingCard;

	// Token: 0x02000A8C RID: 2700
	// (Invoke) Token: 0x06005DE2 RID: 24034
	public delegate void FinishedCallback(object userData);

	// Token: 0x02000A8D RID: 2701
	private class FinishedListener : EventListener<PackOpeningDirector.FinishedCallback>
	{
		// Token: 0x06005DE6 RID: 24038 RVA: 0x001C20EE File Offset: 0x001C02EE
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}
}
