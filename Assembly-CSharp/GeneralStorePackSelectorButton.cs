using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000ADA RID: 2778
[CustomEditClass]
public class GeneralStorePackSelectorButton : PegUIElement
{
	// Token: 0x06005FDF RID: 24543 RVA: 0x001CB3B4 File Offset: 0x001C95B4
	public void SetBoosterId(BoosterDbId boosterId)
	{
		this.m_dbfRecord = GameDbf.Booster.GetRecord((int)boosterId);
		this.m_isLatestExpansion = (this.m_dbfRecord.SortOrder == Enumerable.Max<BoosterDbfRecord>(GameDbf.Booster.GetRecords(), (BoosterDbfRecord r) => r.SortOrder));
		if (this.m_packText != null)
		{
			this.m_packText.Text = this.m_dbfRecord.Name;
		}
	}

	// Token: 0x06005FE0 RID: 24544 RVA: 0x001CB43D File Offset: 0x001C963D
	public BoosterDbId GetBoosterId()
	{
		return (BoosterDbId)((this.m_dbfRecord != null) ? this.m_dbfRecord.ID : 0);
	}

	// Token: 0x06005FE1 RID: 24545 RVA: 0x001CB45B File Offset: 0x001C965B
	public BoosterDbfRecord GetBooster()
	{
		return this.m_dbfRecord;
	}

	// Token: 0x06005FE2 RID: 24546 RVA: 0x001CB464 File Offset: 0x001C9664
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

	// Token: 0x06005FE3 RID: 24547 RVA: 0x001CB4CC File Offset: 0x001C96CC
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

	// Token: 0x06005FE4 RID: 24548 RVA: 0x001CB520 File Offset: 0x001C9720
	public void UpdateRibbonIndicator()
	{
		if (this.m_ribbonIndicator == null || this.GetBoosterId() == BoosterDbId.INVALID)
		{
			return;
		}
		bool active = false;
		if (this.IsRecommendedForNewPlayer())
		{
			active = true;
			this.m_ribbonIndicatorText.Text = GameStrings.Get("GLUE_STORE_PACKBUY_SUGGESTION");
		}
		else if (this.IsPreorder())
		{
			active = true;
			this.m_ribbonIndicatorText.Text = GameStrings.Get("GLUE_STORE_PACKS_PREORDER_TEXT");
		}
		else if (this.IsLatestExpansion())
		{
			active = true;
			this.m_ribbonIndicatorText.Text = GameStrings.Get("GLUE_STORE_PACKS_LATEST_EXPANSION");
		}
		this.m_ribbonIndicator.SetActive(active);
	}

	// Token: 0x06005FE5 RID: 24549 RVA: 0x001CB5C8 File Offset: 0x001C97C8
	public bool IsPurchasable()
	{
		List<Network.Bundle> allBundlesForProduct = StoreManager.Get().GetAllBundlesForProduct(1, GeneralStorePacksContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, (int)this.GetBoosterId(), 0);
		return allBundlesForProduct != null && allBundlesForProduct.Count > 0;
	}

	// Token: 0x06005FE6 RID: 24550 RVA: 0x001CB600 File Offset: 0x001C9800
	public bool IsRecommendedForNewPlayer()
	{
		if (this.m_checkNewPlayer)
		{
			int num = CollectionManager.Get().NumCardsOwnedInSet(TAG_CARD_SET.EXPERT1);
			int num2 = GameUtils.GetBoosterCount(1) * 5;
			int num3 = num2 + num;
			if (num3 < this.m_recommendedExpertSetOwnedCardCount)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005FE7 RID: 24551 RVA: 0x001CB640 File Offset: 0x001C9840
	public bool IsPreorder()
	{
		Network.Bundle bundle = null;
		return StoreManager.Get().IsBoosterPreorderActive((int)this.GetBoosterId(), out bundle);
	}

	// Token: 0x06005FE8 RID: 24552 RVA: 0x001CB661 File Offset: 0x001C9861
	public bool IsLatestExpansion()
	{
		return this.m_isLatestExpansion && !this.IsPreorder();
	}

	// Token: 0x06005FE9 RID: 24553 RVA: 0x001CB67C File Offset: 0x001C987C
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		base.OnOver(oldState);
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
		if (!string.IsNullOrEmpty(this.m_mouseOverSound))
		{
			SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_mouseOverSound));
		}
	}

	// Token: 0x06005FEA RID: 24554 RVA: 0x001CB6C4 File Offset: 0x001C98C4
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		base.OnOut(oldState);
		this.m_highlight.ChangeState((!this.m_selected) ? ActorStateType.NONE : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
	}

	// Token: 0x06005FEB RID: 24555 RVA: 0x001CB6F7 File Offset: 0x001C98F7
	protected override void OnRelease()
	{
		base.OnRelease();
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE);
	}

	// Token: 0x06005FEC RID: 24556 RVA: 0x001CB70D File Offset: 0x001C990D
	protected override void OnPress()
	{
		base.OnPress();
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
	}

	// Token: 0x04004753 RID: 18259
	public UberText m_packText;

	// Token: 0x04004754 RID: 18260
	public HighlightState m_highlight;

	// Token: 0x04004755 RID: 18261
	public GameObject m_ribbonIndicator;

	// Token: 0x04004756 RID: 18262
	public UberText m_ribbonIndicatorText;

	// Token: 0x04004757 RID: 18263
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_selectSound;

	// Token: 0x04004758 RID: 18264
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_unselectSound;

	// Token: 0x04004759 RID: 18265
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_mouseOverSound;

	// Token: 0x0400475A RID: 18266
	public bool m_checkNewPlayer;

	// Token: 0x0400475B RID: 18267
	[CustomEditField(Parent = "m_checkNewPlayer")]
	public int m_recommendedExpertSetOwnedCardCount = 100;

	// Token: 0x0400475C RID: 18268
	private bool m_selected;

	// Token: 0x0400475D RID: 18269
	private BoosterDbfRecord m_dbfRecord;

	// Token: 0x0400475E RID: 18270
	private bool m_isLatestExpansion;
}
