using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000484 RID: 1156
[CustomEditClass]
public class CardListPopup : DialogBase
{
	// Token: 0x1700042E RID: 1070
	// (get) Token: 0x06003805 RID: 14341 RVA: 0x00113196 File Offset: 0x00111396
	// (set) Token: 0x06003806 RID: 14342 RVA: 0x0011319E File Offset: 0x0011139E
	[CustomEditField(Sections = "Variables")]
	public float CardSpacing
	{
		get
		{
			return this.m_CardSpacing;
		}
		set
		{
			this.m_CardSpacing = value;
			this.UpdateCardPositions();
		}
	}

	// Token: 0x06003807 RID: 14343 RVA: 0x001131AD File Offset: 0x001113AD
	protected override void Awake()
	{
		base.Awake();
		this.m_okayButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.PressOkay();
		});
	}

	// Token: 0x06003808 RID: 14344 RVA: 0x001131CE File Offset: 0x001113CE
	private void Start()
	{
	}

	// Token: 0x06003809 RID: 14345 RVA: 0x001131D0 File Offset: 0x001113D0
	private void OnDestroy()
	{
		if (UniversalInputManager.Get() != null)
		{
			UniversalInputManager.Get().SetSystemDialogActive(false);
		}
	}

	// Token: 0x0600380A RID: 14346 RVA: 0x001131ED File Offset: 0x001113ED
	public void SetInfo(CardListPopup.Info info)
	{
		this.m_info = info;
		if (this.m_info.m_callbackOnHide != null)
		{
			base.AddHideListener(this.m_info.m_callbackOnHide);
		}
	}

	// Token: 0x0600380B RID: 14347 RVA: 0x00113218 File Offset: 0x00111418
	public override void Show()
	{
		base.Show();
		this.FadeEffectsIn();
		this.InitInfo();
		this.UpdateAll(this.m_info);
		UniversalInputManager.Get().SetSystemDialogActive(true);
	}

	// Token: 0x0600380C RID: 14348 RVA: 0x0011324E File Offset: 0x0011144E
	public override void Hide()
	{
		base.Hide();
		this.FadeEffectsOut();
	}

	// Token: 0x0600380D RID: 14349 RVA: 0x0011325C File Offset: 0x0011145C
	private void InitInfo()
	{
		if (this.m_info == null)
		{
			this.m_info = new CardListPopup.Info();
		}
	}

	// Token: 0x0600380E RID: 14350 RVA: 0x00113274 File Offset: 0x00111474
	private void UpdateAll(CardListPopup.Info info)
	{
		this.m_description.Text = info.m_description;
		if (this.m_info.m_cards.Count > 3)
		{
			this.SetupPagingArrows();
		}
		this.m_numPages = (this.m_info.m_cards.Count + 3 - 1) / 3;
		this.ShowPage(0);
	}

	// Token: 0x0600380F RID: 14351 RVA: 0x001132D4 File Offset: 0x001114D4
	private void ShowPage(int pageNum)
	{
		if (pageNum < 0 || pageNum >= this.m_numPages)
		{
			Log.JMac.PrintWarning("Attempting to show invalid page number " + pageNum, new object[0]);
			return;
		}
		this.m_pageNum = pageNum;
		base.StopCoroutine("TransitionPage");
		base.StartCoroutine("TransitionPage");
	}

	// Token: 0x06003810 RID: 14352 RVA: 0x00113334 File Offset: 0x00111534
	private IEnumerator TransitionPage()
	{
		if (this.m_leftArrow != null)
		{
			this.m_leftArrow.gameObject.SetActive(false);
		}
		if (this.m_rightArrow != null)
		{
			this.m_rightArrow.gameObject.SetActive(false);
		}
		List<Spell> activeSpells = new List<Spell>();
		foreach (Actor actor in this.m_cardActors)
		{
			Object.Destroy(actor.gameObject);
		}
		this.m_cardActors.Clear();
		activeSpells.Clear();
		int startingCardIndex = this.m_pageNum * 3;
		int cardsToShow = Mathf.Min(3, this.m_info.m_cards.Count - startingCardIndex);
		for (int i = 0; i < cardsToShow; i++)
		{
			CollectibleCard card = this.m_info.m_cards[startingCardIndex + i];
			FullDef fullDef = DefLoader.Get().GetFullDef(card.CardId, null);
			GameObject actorObj = AssetLoader.Get().LoadActor(ActorNames.GetHandActor(fullDef.GetEntityDef(), TAG_PREMIUM.NORMAL), false, false);
			Actor actor2 = actorObj.GetComponent<Actor>();
			actor2.SetCardDef(fullDef.GetCardDef());
			actor2.SetEntityDef(fullDef.GetEntityDef());
			GameUtils.SetParent(actor2, this.m_CardsContainer, false);
			SceneUtils.SetLayer(actor2, this.m_CardsContainer.gameObject.layer);
			GameObject nerfGlowsGO = null;
			if (card.CardType == TAG_CARDTYPE.MINION)
			{
				nerfGlowsGO = AssetLoader.Get().LoadGameObject("Card_Hand_Ally_NerfGlows", true, false);
			}
			else if (card.CardType == TAG_CARDTYPE.SPELL)
			{
				nerfGlowsGO = AssetLoader.Get().LoadGameObject("Card_Hand_Ability_NerfGlows", true, false);
			}
			else if (card.CardType == TAG_CARDTYPE.WEAPON)
			{
				nerfGlowsGO = AssetLoader.Get().LoadGameObject("Card_Hand_Weapon_NerfGlows", true, false);
			}
			if (nerfGlowsGO != null)
			{
				CardNerfGlows nerfGlows = nerfGlowsGO.GetComponent<CardNerfGlows>();
				if (nerfGlows != null)
				{
					TransformUtil.AttachAndPreserveLocalTransform(nerfGlowsGO.transform, actor2.transform);
					SceneUtils.SetLayer(nerfGlows, actor2.gameObject.layer);
					nerfGlows.SetGlowsForCard(card);
				}
				else
				{
					Debug.LogError("CardListPopup.cs: Nerf Glows GameObject " + nerfGlowsGO + " does not have a CardNerfGlows script attached.");
				}
			}
			this.m_cardActors.Add(actor2);
		}
		this.UpdateCardPositions();
		foreach (Actor actor3 in this.m_cardActors)
		{
			activeSpells.Add(actor3.ActivateSpell(SpellType.DEATHREVERSE));
			actor3.ContactShadow(true);
		}
		SoundManager.Get().LoadAndPlay("collection_manager_card_move_invalid_or_click");
		yield return new WaitForSeconds(0.2f);
		if (this.m_leftArrow != null)
		{
			this.m_leftArrow.gameObject.SetActive(this.m_pageNum != 0);
		}
		if (this.m_rightArrow != null)
		{
			this.m_rightArrow.gameObject.SetActive(this.m_pageNum < this.m_numPages - 1);
		}
		yield break;
	}

	// Token: 0x06003811 RID: 14353 RVA: 0x00113350 File Offset: 0x00111550
	private void UpdateCardPositions()
	{
		int count = this.m_cardActors.Count;
		for (int i = 0; i < count; i++)
		{
			Actor actor = this.m_cardActors[i];
			Vector3 zero = Vector3.zero;
			float num = ((float)i - (float)(count - 1) / 2f) * this.m_CardSpacing;
			zero.x += num;
			actor.transform.localPosition = zero;
		}
	}

	// Token: 0x06003812 RID: 14354 RVA: 0x001133C0 File Offset: 0x001115C0
	private void SetupPagingArrows()
	{
		this.m_leftArrowNested.gameObject.SetActive(true);
		this.m_rightArrowNested.gameObject.SetActive(true);
		GameObject gameObject = this.m_leftArrowNested.PrefabGameObject();
		SceneUtils.SetLayer(gameObject, this.m_leftArrowNested.gameObject.layer);
		this.m_leftArrow = gameObject.GetComponent<UIBButton>();
		this.m_leftArrow.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.TurnPage(false);
		});
		gameObject = this.m_rightArrowNested.PrefabGameObject();
		SceneUtils.SetLayer(gameObject, this.m_rightArrowNested.gameObject.layer);
		this.m_rightArrow = gameObject.GetComponent<UIBButton>();
		this.m_rightArrow.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.TurnPage(true);
		});
		HighlightState componentInChildren = this.m_rightArrow.GetComponentInChildren<HighlightState>();
		if (componentInChildren)
		{
			componentInChildren.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
	}

	// Token: 0x06003813 RID: 14355 RVA: 0x001134A0 File Offset: 0x001116A0
	private void TurnPage(bool right)
	{
		Log.JMac.Print("Turn page!", new object[0]);
		HighlightState componentInChildren = this.m_rightArrow.GetComponentInChildren<HighlightState>();
		if (componentInChildren)
		{
			componentInChildren.ChangeState(ActorStateType.NONE);
		}
		this.ShowPage(this.m_pageNum + ((!right) ? -1 : 1));
	}

	// Token: 0x06003814 RID: 14356 RVA: 0x001134FB File Offset: 0x001116FB
	private void PressOkay()
	{
		this.Hide();
	}

	// Token: 0x06003815 RID: 14357 RVA: 0x00113504 File Offset: 0x00111704
	private void FadeEffectsIn()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			return;
		}
		fullScreenFXMgr.SetBlurBrightness(1f);
		fullScreenFXMgr.SetBlurDesaturation(0f);
		fullScreenFXMgr.Vignette(0.4f, 0.4f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.Blur(1f, 0.4f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06003816 RID: 14358 RVA: 0x00113560 File Offset: 0x00111760
	private void FadeEffectsOut()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			return;
		}
		fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06003817 RID: 14359 RVA: 0x0011359C File Offset: 0x0011179C
	protected override CanvasScaleMode ScaleMode()
	{
		return (!UniversalInputManager.UsePhoneUI) ? CanvasScaleMode.HEIGHT : CanvasScaleMode.WIDTH;
	}

	// Token: 0x040023EB RID: 9195
	private const int MAX_CARDS_PER_PAGE = 3;

	// Token: 0x040023EC RID: 9196
	[CustomEditField(Sections = "Object Links")]
	public GameObject m_CardsContainer;

	// Token: 0x040023ED RID: 9197
	public UIBButton m_okayButton;

	// Token: 0x040023EE RID: 9198
	public UberText m_description;

	// Token: 0x040023EF RID: 9199
	public NestedPrefab m_leftArrowNested;

	// Token: 0x040023F0 RID: 9200
	public NestedPrefab m_rightArrowNested;

	// Token: 0x040023F1 RID: 9201
	[SerializeField]
	private float m_CardSpacing = 5f;

	// Token: 0x040023F2 RID: 9202
	private CardListPopup.Info m_info;

	// Token: 0x040023F3 RID: 9203
	private UIBButton m_leftArrow;

	// Token: 0x040023F4 RID: 9204
	private UIBButton m_rightArrow;

	// Token: 0x040023F5 RID: 9205
	private int m_numPages = 1;

	// Token: 0x040023F6 RID: 9206
	private int m_pageNum;

	// Token: 0x040023F7 RID: 9207
	private List<Actor> m_cardActors = new List<Actor>();

	// Token: 0x02000485 RID: 1157
	public class Info
	{
		// Token: 0x040023F8 RID: 9208
		public string m_description;

		// Token: 0x040023F9 RID: 9209
		public List<CollectibleCard> m_cards;

		// Token: 0x040023FA RID: 9210
		public DialogBase.HideCallback m_callbackOnHide;
	}
}
