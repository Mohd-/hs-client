using System;
using UnityEngine;

// Token: 0x020005C2 RID: 1474
[CustomEditClass]
public class CollectionCardVisual : PegUIElement
{
	// Token: 0x060041C7 RID: 16839 RVA: 0x0013D758 File Offset: 0x0013B958
	protected override void Awake()
	{
		base.Awake();
		if (base.gameObject.GetComponent<AudioSource>() == null)
		{
			base.gameObject.AddComponent<AudioSource>();
		}
		base.SetDragTolerance(5f);
		SoundManager.Get().Load("collection_manager_card_add_to_deck_instant");
	}

	// Token: 0x060041C8 RID: 16840 RVA: 0x0013D7A8 File Offset: 0x0013B9A8
	public bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x060041C9 RID: 16841 RVA: 0x0013D7B0 File Offset: 0x0013B9B0
	public void ShowLock(CollectionCardVisual.LockType type)
	{
		this.ShowLock(type, string.Empty, false);
	}

	// Token: 0x060041CA RID: 16842 RVA: 0x0013D7C0 File Offset: 0x0013B9C0
	public void ShowLock(CollectionCardVisual.LockType lockType, string reason, bool playSound)
	{
		CollectionCardVisual.LockType lockType2 = this.m_lockType;
		this.m_lockType = lockType;
		this.UpdateCardCountVisibility();
		if (this.m_cardLock != null)
		{
			if (this.m_actor != null)
			{
				this.m_cardLock.UpdateLockVisual(this.m_actor.GetEntityDef(), lockType, reason);
			}
			else
			{
				this.m_cardLock.UpdateLockVisual(null, CollectionCardVisual.LockType.NONE, string.Empty);
			}
		}
		if (playSound)
		{
			if (this.m_lockType == CollectionCardVisual.LockType.NONE && lockType2 != CollectionCardVisual.LockType.NONE)
			{
				SoundManager.Get().LoadAndPlay("card_limit_unlock");
			}
			if (this.m_lockType != CollectionCardVisual.LockType.NONE && lockType2 == CollectionCardVisual.LockType.NONE)
			{
				SoundManager.Get().LoadAndPlay("card_limit_lock");
			}
		}
	}

	// Token: 0x060041CB RID: 16843 RVA: 0x0013D878 File Offset: 0x0013BA78
	public void OnDoneCrafting()
	{
		this.UpdateCardCount();
	}

	// Token: 0x060041CC RID: 16844 RVA: 0x0013D880 File Offset: 0x0013BA80
	public void SetActor(Actor actor, CollectionManagerDisplay.ViewMode type = CollectionManagerDisplay.ViewMode.CARDS)
	{
		if (this.m_actor != null && this.m_actor.transform.parent == base.transform)
		{
			this.m_actor.Hide();
		}
		this.m_visualType = type;
		this.m_actor = actor;
		this.UpdateCardCount();
		if (this.m_actor == null)
		{
			return;
		}
		GameUtils.SetParent(actor, this, false);
		ActorStateType activeStateType = this.m_actor.GetActorStateMgr().GetActiveStateType();
		this.ShowNewCardCallout(activeStateType == ActorStateType.CARD_RECENTLY_ACQUIRED);
	}

	// Token: 0x060041CD RID: 16845 RVA: 0x0013D914 File Offset: 0x0013BB14
	public Actor GetActor()
	{
		if (this.m_actor.GetCardDef() == null && this.m_actor.GetEntityDef() != null)
		{
			CardDef cardDef = DefLoader.Get().GetCardDef(this.m_actor.GetEntityDef().GetCardId(), new CardPortraitQuality(3, this.m_actor.GetPremium()));
			this.m_actor.SetCardDef(cardDef);
		}
		return this.m_actor;
	}

	// Token: 0x060041CE RID: 16846 RVA: 0x0013D985 File Offset: 0x0013BB85
	public CollectionManagerDisplay.ViewMode GetVisualType()
	{
		return this.m_visualType;
	}

	// Token: 0x060041CF RID: 16847 RVA: 0x0013D98D File Offset: 0x0013BB8D
	public void SetCMRow(int rowNum)
	{
		this.m_cmRow = rowNum;
	}

	// Token: 0x060041D0 RID: 16848 RVA: 0x0013D996 File Offset: 0x0013BB96
	public int GetCMRow()
	{
		return this.m_cmRow;
	}

	// Token: 0x060041D1 RID: 16849 RVA: 0x0013D9A0 File Offset: 0x0013BBA0
	public static void ShowActorShadow(Actor actor, bool show)
	{
		string tag = "FakeShadow";
		string tag2 = "FakeShadowUnique";
		GameObject gameObject = SceneUtils.FindChildByTag(actor.gameObject, tag);
		GameObject gameObject2 = SceneUtils.FindChildByTag(actor.gameObject, tag2);
		EntityDef entityDef = actor.GetEntityDef();
		if (entityDef != null && show)
		{
			if (entityDef.IsElite())
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

	// Token: 0x060041D2 RID: 16850 RVA: 0x0013DA90 File Offset: 0x0013BC90
	public void Show()
	{
		this.m_shown = true;
		this.SetEnabled(true);
		base.GetComponent<Collider>().enabled = true;
		if (this.m_actor == null || this.m_actor.GetEntityDef() == null)
		{
			return;
		}
		bool flag = false;
		if (this.m_visualType == CollectionManagerDisplay.ViewMode.CARDS)
		{
			string cardId = this.m_actor.GetEntityDef().GetCardId();
			TAG_PREMIUM premium = this.m_actor.GetPremium();
			flag = CollectionManagerDisplay.Get().ShouldShowNewCardGlow(cardId, premium);
			if (!flag)
			{
				CollectionManager.Get().MarkAllInstancesAsSeen(cardId, premium);
			}
		}
		this.ShowNewCardCallout(flag);
		this.m_actor.Show();
		ActorStateType actorState = (!flag) ? ActorStateType.CARD_IDLE : ActorStateType.CARD_RECENTLY_ACQUIRED;
		this.m_actor.SetActorState(actorState);
		Renderer[] componentsInChildren = this.m_actor.gameObject.GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null)
		{
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.shadowCastingMode = 0;
			}
		}
		EntityDef entityDef = this.m_actor.GetEntityDef();
		bool show = CollectionManager.Get().IsCardInCollection(entityDef.GetCardId(), this.m_actor.GetPremium());
		CollectionCardVisual.ShowActorShadow(this.m_actor, show);
	}

	// Token: 0x060041D3 RID: 16851 RVA: 0x0013DBCC File Offset: 0x0013BDCC
	public void Hide()
	{
		this.m_shown = false;
		this.SetEnabled(false);
		base.GetComponent<Collider>().enabled = false;
		this.ShowLock(CollectionCardVisual.LockType.NONE);
		this.ShowNewCardCallout(false);
		if (this.m_cardCount != null)
		{
			this.m_cardCount.Hide();
		}
		if (this.m_actor == null)
		{
			return;
		}
		this.m_actor.Hide();
	}

	// Token: 0x060041D4 RID: 16852 RVA: 0x0013DC3A File Offset: 0x0013BE3A
	public CollectionCardCount GetCardCountObject()
	{
		return this.m_cardCount;
	}

	// Token: 0x060041D5 RID: 16853 RVA: 0x0013DC44 File Offset: 0x0013BE44
	public void SetHeroSkinBoxCollider()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		component.center = this.m_heroSkinBoxColliderCenter;
		component.size = this.m_heroSkinBoxColliderSize;
	}

	// Token: 0x060041D6 RID: 16854 RVA: 0x0013DC70 File Offset: 0x0013BE70
	public void SetDefaultBoxCollider()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		component.center = this.m_boxColliderCenter;
		component.size = this.m_boxColliderSize;
	}

	// Token: 0x060041D7 RID: 16855 RVA: 0x0013DC9C File Offset: 0x0013BE9C
	private bool CheckCardSeen()
	{
		ActorStateType activeStateType = this.m_actor.GetActorStateMgr().GetActiveStateType();
		bool flag = activeStateType == ActorStateType.CARD_RECENTLY_ACQUIRED;
		if (flag)
		{
			EntityDef entityDef = this.m_actor.GetEntityDef();
			if (entityDef != null)
			{
				CollectionManager.Get().MarkAllInstancesAsSeen(entityDef.GetCardId(), this.m_actor.GetPremium());
			}
		}
		return flag;
	}

	// Token: 0x060041D8 RID: 16856 RVA: 0x0013DCF4 File Offset: 0x0013BEF4
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (this.ShouldIgnoreAllInput())
		{
			return;
		}
		EntityDef entityDef = this.m_actor.GetEntityDef();
		if (entityDef != null)
		{
			KeywordHelpPanelManager.Get().UpdateKeywordHelpForCollectionManager(entityDef, this.m_actor, this.m_cmRow > 0);
		}
		SoundManager.Get().LoadAndPlay("collection_manager_card_mouse_over", base.gameObject);
		if (!this.IsInCollection())
		{
			return;
		}
		ActorStateType actorState = ActorStateType.CARD_MOUSE_OVER;
		if (this.CheckCardSeen())
		{
			actorState = ActorStateType.CARD_RECENTLY_ACQUIRED_MOUSE_OVER;
		}
		this.m_actor.SetActorState(actorState);
	}

	// Token: 0x060041D9 RID: 16857 RVA: 0x0013DD78 File Offset: 0x0013BF78
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		KeywordHelpPanelManager.Get().HideKeywordHelp();
		if (this.ShouldIgnoreAllInput())
		{
			return;
		}
		if (!this.IsInCollection())
		{
			return;
		}
		this.CheckCardSeen();
		this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
		this.ShowNewCardCallout(false);
	}

	// Token: 0x060041DA RID: 16858 RVA: 0x0013DDC1 File Offset: 0x0013BFC1
	protected override void OnHold()
	{
		if (!this.CanPickUpCard())
		{
			return;
		}
		CollectionInputMgr.Get().GrabCard(this);
	}

	// Token: 0x060041DB RID: 16859 RVA: 0x0013DDDC File Offset: 0x0013BFDC
	protected override void OnRelease()
	{
		if (this.IsTransactionPendingOnThisCard())
		{
			return;
		}
		if (UniversalInputManager.Get().IsTouchMode() || (CraftingTray.Get() != null && CraftingTray.Get().IsShown()))
		{
			this.CheckCardSeen();
			this.ShowNewCardCallout(false);
			this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
			this.EnterCraftingMode();
			return;
		}
		Spell spell = this.m_actor.GetSpell(SpellType.DEATHREVERSE);
		if (spell != null)
		{
			ParticleSystem componentInChildren = spell.gameObject.GetComponentInChildren<ParticleSystem>();
			componentInChildren.simulationSpace = 0;
		}
		if (!this.CanPickUpCard())
		{
			SoundManager.Get().LoadAndPlay("collection_manager_card_move_invalid_or_click");
			if (spell != null)
			{
				spell.ActivateState(SpellStateType.BIRTH);
			}
			EntityDef entityDef = this.m_actor.GetEntityDef();
			bool isHero = entityDef != null && entityDef.IsHero();
			CollectionManagerDisplay.Get().ShowInnkeeeprLClickHelp(isHero);
			return;
		}
		if (this.m_visualType == CollectionManagerDisplay.ViewMode.CARDS)
		{
			EntityDef entityDef2 = this.m_actor.GetEntityDef();
			if (entityDef2 == null)
			{
				return;
			}
			if (spell != null)
			{
				spell.ActivateState(SpellStateType.BIRTH);
			}
			CollectionDeckTray.Get().AddCard(entityDef2, this.m_actor.GetPremium(), null, false, this.m_actor);
		}
		else if (this.m_visualType == CollectionManagerDisplay.ViewMode.CARD_BACKS)
		{
			CollectionDeckTray.Get().SetCardBack(this.m_actor);
		}
		else if (this.m_visualType == CollectionManagerDisplay.ViewMode.HERO_SKINS)
		{
			CollectionDeckTray.Get().SetHeroSkin(this.m_actor);
		}
	}

	// Token: 0x060041DC RID: 16860 RVA: 0x0013DF5C File Offset: 0x0013C15C
	protected override void OnRightClick()
	{
		if (this.IsTransactionPendingOnThisCard())
		{
			return;
		}
		if (!Options.Get().GetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, false))
		{
			Options.Get().SetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, true);
		}
		this.ShowNewCardCallout(false);
		this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
		this.EnterCraftingMode();
	}

	// Token: 0x060041DD RID: 16861 RVA: 0x0013DFB0 File Offset: 0x0013C1B0
	private void EnterCraftingMode()
	{
		CollectionManagerDisplay.ViewMode viewMode = CollectionManagerDisplay.Get().GetViewMode();
		if (this.m_visualType != viewMode)
		{
			return;
		}
		switch (viewMode)
		{
		case CollectionManagerDisplay.ViewMode.CARDS:
		{
			CraftingManager craftingManager = CraftingManager.Get();
			if (craftingManager != null)
			{
				CraftingManager.Get().EnterCraftMode(this.GetActor());
			}
			break;
		}
		case CollectionManagerDisplay.ViewMode.HERO_SKINS:
		{
			HeroSkinInfoManager heroSkinInfoManager = HeroSkinInfoManager.Get();
			if (heroSkinInfoManager != null)
			{
				heroSkinInfoManager.EnterPreview(this);
			}
			break;
		}
		case CollectionManagerDisplay.ViewMode.CARD_BACKS:
		{
			CardBackInfoManager cardBackInfoManager = CardBackInfoManager.Get();
			if (cardBackInfoManager != null)
			{
				cardBackInfoManager.EnterPreview(this);
			}
			break;
		}
		}
		CollectionDeckTray.Get().CancelRenamingDeck();
	}

	// Token: 0x060041DE RID: 16862 RVA: 0x0013E060 File Offset: 0x0013C260
	private bool IsTransactionPendingOnThisCard()
	{
		CraftingManager craftingManager = CraftingManager.Get();
		if (craftingManager == null)
		{
			return false;
		}
		PendingTransaction pendingTransaction = craftingManager.GetPendingTransaction();
		if (pendingTransaction == null)
		{
			return false;
		}
		EntityDef entityDef = this.m_actor.GetEntityDef();
		return entityDef != null && !(pendingTransaction.CardID != entityDef.GetCardId()) && pendingTransaction.Premium == this.m_actor.GetPremium();
	}

	// Token: 0x060041DF RID: 16863 RVA: 0x0013E0D8 File Offset: 0x0013C2D8
	private bool ShouldIgnoreAllInput()
	{
		return (CollectionInputMgr.Get() != null && CollectionInputMgr.Get().IsDraggingScrollbar()) || (CraftingManager.Get() != null && CraftingManager.Get().IsCardShowing());
	}

	// Token: 0x060041E0 RID: 16864 RVA: 0x0013E128 File Offset: 0x0013C328
	private bool IsInCollection()
	{
		return !(this.m_cardCount == null) && this.m_cardCount.GetCount() > 0;
	}

	// Token: 0x060041E1 RID: 16865 RVA: 0x0013E14B File Offset: 0x0013C34B
	private bool IsUnlocked()
	{
		return this.m_lockType == CollectionCardVisual.LockType.NONE;
	}

	// Token: 0x060041E2 RID: 16866 RVA: 0x0013E158 File Offset: 0x0013C358
	private bool CanPickUpCard()
	{
		if (this.ShouldIgnoreAllInput())
		{
			return false;
		}
		CollectionManagerDisplay.ViewMode viewMode = CollectionManagerDisplay.Get().GetViewMode();
		if (viewMode != this.m_visualType)
		{
			return false;
		}
		if (!CollectionDeckTray.Get().CanPickupCard())
		{
			return false;
		}
		CollectionManagerDisplay.ViewMode visualType = this.m_visualType;
		if (visualType == CollectionManagerDisplay.ViewMode.CARDS)
		{
			if (!this.IsInCollection())
			{
				return false;
			}
			if (!this.IsUnlocked())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060041E3 RID: 16867 RVA: 0x0013E1CE File Offset: 0x0013C3CE
	public void ShowNewCardCallout(bool show)
	{
		if (this.m_newCardCallout == null)
		{
			return;
		}
		this.m_newCardCallout.SetActive(show);
	}

	// Token: 0x060041E4 RID: 16868 RVA: 0x0013E1F0 File Offset: 0x0013C3F0
	private void UpdateCardCount()
	{
		if (this.m_cardCount == null)
		{
			return;
		}
		int count = 0;
		if (this.m_actor != null)
		{
			EntityDef entityDef = this.m_actor.GetEntityDef();
			if (entityDef != null)
			{
				CollectibleCard card = CollectionManager.Get().GetCard(entityDef.GetCardId(), this.m_actor.GetPremium());
				count = card.OwnedCount;
			}
		}
		this.m_cardCount.SetCount(count);
		this.UpdateCardCountVisibility();
	}

	// Token: 0x060041E5 RID: 16869 RVA: 0x0013E26C File Offset: 0x0013C46C
	private void UpdateCardCountVisibility()
	{
		if (this.m_cardCount == null)
		{
			return;
		}
		if (this.m_lockType == CollectionCardVisual.LockType.NONE && this.m_visualType == CollectionManagerDisplay.ViewMode.CARDS)
		{
			this.m_cardCount.Show();
		}
		else
		{
			this.m_cardCount.Hide();
		}
	}

	// Token: 0x040029D9 RID: 10713
	private const string ADD_CARD_TO_DECK_SOUND = "collection_manager_card_add_to_deck_instant";

	// Token: 0x040029DA RID: 10714
	private const string CARD_LIMIT_UNLOCK_SOUND = "card_limit_unlock";

	// Token: 0x040029DB RID: 10715
	private const string CARD_LIMIT_LOCK_SOUND = "card_limit_lock";

	// Token: 0x040029DC RID: 10716
	private const string CARD_MOUSE_OVER_SOUND = "collection_manager_card_mouse_over";

	// Token: 0x040029DD RID: 10717
	private const string CARD_MOVE_INVALID_OR_CLICK_SOUND = "collection_manager_card_move_invalid_or_click";

	// Token: 0x040029DE RID: 10718
	public CollectionCardCount m_cardCount;

	// Token: 0x040029DF RID: 10719
	public CollectionCardLock m_cardLock;

	// Token: 0x040029E0 RID: 10720
	public GameObject m_newCardCallout;

	// Token: 0x040029E1 RID: 10721
	public Vector3 m_boxColliderCenter = new Vector3(0f, 0.14f, 0f);

	// Token: 0x040029E2 RID: 10722
	public Vector3 m_boxColliderSize = new Vector3(2f, 0.21f, 2.7f);

	// Token: 0x040029E3 RID: 10723
	public Vector3 m_heroSkinBoxColliderCenter = new Vector3(0f, 0.14f, -0.58f);

	// Token: 0x040029E4 RID: 10724
	public Vector3 m_heroSkinBoxColliderSize = new Vector3(2f, 0.21f, 2f);

	// Token: 0x040029E5 RID: 10725
	private Vector3 m_originalPosition;

	// Token: 0x040029E6 RID: 10726
	private Actor m_actor;

	// Token: 0x040029E7 RID: 10727
	private CollectionCardVisual.LockType m_lockType;

	// Token: 0x040029E8 RID: 10728
	private bool m_shown;

	// Token: 0x040029E9 RID: 10729
	private CollectionManagerDisplay.ViewMode m_visualType;

	// Token: 0x040029EA RID: 10730
	private int m_cmRow;

	// Token: 0x020006EF RID: 1775
	public enum LockType
	{
		// Token: 0x04003051 RID: 12369
		NONE,
		// Token: 0x04003052 RID: 12370
		MAX_COPIES_IN_DECK,
		// Token: 0x04003053 RID: 12371
		NO_MORE_INSTANCES
	}
}
