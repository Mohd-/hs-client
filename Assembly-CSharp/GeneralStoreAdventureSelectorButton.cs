using System;
using System.Collections.Generic;
using PegasusUtil;
using UnityEngine;

// Token: 0x02000ACE RID: 2766
[CustomEditClass]
public class GeneralStoreAdventureSelectorButton : PegUIElement
{
	// Token: 0x06005F5F RID: 24415 RVA: 0x001C86BC File Offset: 0x001C68BC
	public void SetAdventureType(AdventureDbId adventure)
	{
		if (this.m_adventureTitle != null)
		{
			AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int)adventure);
			if (record != null)
			{
				this.m_adventureTitle.Text = record.StoreBuyButtonLabel;
			}
		}
		this.m_adventureType = adventure;
		this.UpdateState();
	}

	// Token: 0x06005F60 RID: 24416 RVA: 0x001C870F File Offset: 0x001C690F
	public AdventureDbId GetAdventureType()
	{
		return this.m_adventureType;
	}

	// Token: 0x06005F61 RID: 24417 RVA: 0x001C8718 File Offset: 0x001C6918
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

	// Token: 0x06005F62 RID: 24418 RVA: 0x001C8780 File Offset: 0x001C6980
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

	// Token: 0x06005F63 RID: 24419 RVA: 0x001C87D4 File Offset: 0x001C69D4
	public bool IsPrePurchase()
	{
		Network.Bundle bundle = null;
		StoreManager.Get().GetAvailableAdventureBundle(this.m_adventureType, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle);
		return bundle != null && StoreManager.Get().IsProductPrePurchase(bundle);
	}

	// Token: 0x06005F64 RID: 24420 RVA: 0x001C8810 File Offset: 0x001C6A10
	public void UpdateState()
	{
		if (this.m_preorderRibbon != null)
		{
			this.m_preorderRibbon.SetActive(this.IsPrePurchase());
		}
	}

	// Token: 0x06005F65 RID: 24421 RVA: 0x001C8840 File Offset: 0x001C6A40
	public bool IsPurchasable()
	{
		ProductType adventureProductType = StoreManager.GetAdventureProductType(this.m_adventureType);
		if (adventureProductType == null)
		{
			return false;
		}
		bool flag = false;
		List<Network.Bundle> availableBundlesForProduct = StoreManager.Get().GetAvailableBundlesForProduct(adventureProductType, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, false, out flag, 0, 0);
		return availableBundlesForProduct != null && availableBundlesForProduct.Count > 0;
	}

	// Token: 0x06005F66 RID: 24422 RVA: 0x001C888C File Offset: 0x001C6A8C
	public bool IsAvailable()
	{
		Network.Bundle bundle = null;
		bool result = false;
		StoreManager.Get().GetAvailableAdventureBundle(this.m_adventureType, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out bundle, out result);
		return result;
	}

	// Token: 0x06005F67 RID: 24423 RVA: 0x001C88B8 File Offset: 0x001C6AB8
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		base.OnOver(oldState);
		if (this.IsAvailable())
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
			if (!string.IsNullOrEmpty(this.m_mouseOverSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_mouseOverSound));
			}
		}
		else if (this.m_unavailableTooltip != null)
		{
			KeywordHelpPanel c = this.m_unavailableTooltip.ShowTooltip(GameStrings.Get("GLUE_STORE_ADVENTURE_BUTTON_UNAVAILABLE_HEADLINE"), GameStrings.Get("GLUE_STORE_ADVENTURE_BUTTON_UNAVAILABLE_DESCRIPTION"), this.m_unavailableTooltipScale, true);
			SceneUtils.SetLayer(c, this.m_unavailableTooltipLayer);
		}
	}

	// Token: 0x06005F68 RID: 24424 RVA: 0x001C8954 File Offset: 0x001C6B54
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		base.OnOut(oldState);
		if (this.IsAvailable())
		{
			this.m_highlight.ChangeState((!this.m_selected) ? ActorStateType.NONE : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
		else if (this.m_unavailableTooltip != null)
		{
			this.m_unavailableTooltip.HideTooltip();
		}
	}

	// Token: 0x06005F69 RID: 24425 RVA: 0x001C89B3 File Offset: 0x001C6BB3
	protected override void OnRelease()
	{
		base.OnRelease();
		if (this.IsAvailable())
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
		}
	}

	// Token: 0x06005F6A RID: 24426 RVA: 0x001C89D4 File Offset: 0x001C6BD4
	protected override void OnPress()
	{
		base.OnPress();
		if (this.IsAvailable())
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
	}

	// Token: 0x040046C3 RID: 18115
	public UberText m_adventureTitle;

	// Token: 0x040046C4 RID: 18116
	public HighlightState m_highlight;

	// Token: 0x040046C5 RID: 18117
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_selectSound;

	// Token: 0x040046C6 RID: 18118
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_unselectSound;

	// Token: 0x040046C7 RID: 18119
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_mouseOverSound;

	// Token: 0x040046C8 RID: 18120
	public TooltipZone m_unavailableTooltip;

	// Token: 0x040046C9 RID: 18121
	public GameLayer m_unavailableTooltipLayer = GameLayer.PerspectiveUI;

	// Token: 0x040046CA RID: 18122
	public float m_unavailableTooltipScale = 20f;

	// Token: 0x040046CB RID: 18123
	public GameObject m_preorderRibbon;

	// Token: 0x040046CC RID: 18124
	private bool m_selected;

	// Token: 0x040046CD RID: 18125
	private AdventureDbId m_adventureType;
}
