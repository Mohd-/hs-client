using System;

// Token: 0x02000AD6 RID: 2774
[CustomEditClass]
public class GeneralStoreHeroesSelectorButton : PegUIElement
{
	// Token: 0x06005FBA RID: 24506 RVA: 0x001CAC68 File Offset: 0x001C8E68
	protected override void Awake()
	{
		base.Awake();
		if (UniversalInputManager.UsePhoneUI)
		{
			OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.WIDTH);
		}
		else
		{
			OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		}
	}

	// Token: 0x06005FBB RID: 24507 RVA: 0x001CACB5 File Offset: 0x001C8EB5
	public int GetHeroDbId()
	{
		return this.m_hero.ID;
	}

	// Token: 0x06005FBC RID: 24508 RVA: 0x001CACC2 File Offset: 0x001C8EC2
	public string GetHeroCardMiniGuid()
	{
		return this.m_hero.CardId;
	}

	// Token: 0x06005FBD RID: 24509 RVA: 0x001CACCF File Offset: 0x001C8ECF
	public int GetHeroCardDbId()
	{
		return GameUtils.TranslateCardIdToDbId(this.GetHeroCardMiniGuid());
	}

	// Token: 0x06005FBE RID: 24510 RVA: 0x001CACDC File Offset: 0x001C8EDC
	public void SetHeroDbfRecord(HeroDbfRecord hero)
	{
		this.m_hero = hero;
	}

	// Token: 0x06005FBF RID: 24511 RVA: 0x001CACE5 File Offset: 0x001C8EE5
	public HeroDbfRecord GetHeroDbfRecord()
	{
		return this.m_hero;
	}

	// Token: 0x06005FC0 RID: 24512 RVA: 0x001CACED File Offset: 0x001C8EED
	public void SetSortOrder(int sortOrder)
	{
		this.m_sortOrder = sortOrder;
	}

	// Token: 0x06005FC1 RID: 24513 RVA: 0x001CACF6 File Offset: 0x001C8EF6
	public int GetSortOrder()
	{
		return this.m_sortOrder;
	}

	// Token: 0x06005FC2 RID: 24514 RVA: 0x001CACFE File Offset: 0x001C8EFE
	public void SetPurchased(bool purchased)
	{
		this.m_purchased = purchased;
	}

	// Token: 0x06005FC3 RID: 24515 RVA: 0x001CAD07 File Offset: 0x001C8F07
	public bool GetPurchased()
	{
		return this.m_purchased;
	}

	// Token: 0x06005FC4 RID: 24516 RVA: 0x001CAD0F File Offset: 0x001C8F0F
	public void UpdatePortrait(GeneralStoreHeroesSelectorButton rhs)
	{
		this.UpdatePortrait(rhs.m_currentEntityDef, rhs.m_currentCardDef);
	}

	// Token: 0x06005FC5 RID: 24517 RVA: 0x001CAD24 File Offset: 0x001C8F24
	public void UpdatePortrait(EntityDef entityDef, CardDef cardDef)
	{
		this.m_heroActor.SetEntityDef(entityDef);
		this.m_heroActor.SetCardDef(cardDef);
		this.m_heroActor.UpdateAllComponents();
		this.m_heroActor.SetUnlit();
		this.m_currentEntityDef = entityDef;
		this.m_currentCardDef = cardDef;
	}

	// Token: 0x06005FC6 RID: 24518 RVA: 0x001CAD6D File Offset: 0x001C8F6D
	public void UpdateName(GeneralStoreHeroesSelectorButton rhs)
	{
		this.UpdateName(rhs.m_heroName.Text);
	}

	// Token: 0x06005FC7 RID: 24519 RVA: 0x001CAD80 File Offset: 0x001C8F80
	public void UpdateName(string name)
	{
		if (this.m_heroName != null)
		{
			this.m_heroName.Text = name;
		}
	}

	// Token: 0x06005FC8 RID: 24520 RVA: 0x001CADA0 File Offset: 0x001C8FA0
	public void Select()
	{
		if (this.m_selected)
		{
			return;
		}
		this.m_selected = true;
		this.m_highlight.ChangeState((base.GetInteractionState() != PegUIElement.InteractionState.Up) ? ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE : ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
		if (!string.IsNullOrEmpty(this.m_selectSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_selectSound));
		}
	}

	// Token: 0x06005FC9 RID: 24521 RVA: 0x001CAE08 File Offset: 0x001C9008
	public void Unselect()
	{
		if (!this.m_selected)
		{
			return;
		}
		this.m_selected = false;
		this.m_highlight.ChangeState(ActorStateType.NONE);
		if (!string.IsNullOrEmpty(this.m_unselectSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_unselectSound));
		}
	}

	// Token: 0x06005FCA RID: 24522 RVA: 0x001CAE5A File Offset: 0x001C905A
	public bool IsAvailable()
	{
		return true;
	}

	// Token: 0x06005FCB RID: 24523 RVA: 0x001CAE60 File Offset: 0x001C9060
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		base.OnOver(oldState);
		if (this.m_highlight != null && this.IsAvailable())
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
			if (!string.IsNullOrEmpty(this.m_mouseOverSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_mouseOverSound));
			}
		}
	}

	// Token: 0x06005FCC RID: 24524 RVA: 0x001CAEC4 File Offset: 0x001C90C4
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		base.OnOut(oldState);
		if (this.m_highlight != null && this.IsAvailable())
		{
			this.m_highlight.ChangeState((!this.m_selected) ? ActorStateType.NONE : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
	}

	// Token: 0x06005FCD RID: 24525 RVA: 0x001CAF13 File Offset: 0x001C9113
	protected override void OnRelease()
	{
		base.OnRelease();
		if (this.m_highlight != null && this.IsAvailable())
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
		}
	}

	// Token: 0x06005FCE RID: 24526 RVA: 0x001CAF45 File Offset: 0x001C9145
	protected override void OnPress()
	{
		base.OnPress();
		if (this.m_highlight != null && this.IsAvailable())
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
	}

	// Token: 0x04004731 RID: 18225
	public Actor m_heroActor;

	// Token: 0x04004732 RID: 18226
	public UberText m_heroName;

	// Token: 0x04004733 RID: 18227
	public HighlightState m_highlight;

	// Token: 0x04004734 RID: 18228
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_selectSound;

	// Token: 0x04004735 RID: 18229
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_unselectSound;

	// Token: 0x04004736 RID: 18230
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_mouseOverSound;

	// Token: 0x04004737 RID: 18231
	private string m_heroId;

	// Token: 0x04004738 RID: 18232
	private int m_sortOrder;

	// Token: 0x04004739 RID: 18233
	private bool m_selected;

	// Token: 0x0400473A RID: 18234
	private bool m_purchased;

	// Token: 0x0400473B RID: 18235
	private EntityDef m_currentEntityDef;

	// Token: 0x0400473C RID: 18236
	private CardDef m_currentCardDef;

	// Token: 0x0400473D RID: 18237
	private HeroDbfRecord m_hero;
}
