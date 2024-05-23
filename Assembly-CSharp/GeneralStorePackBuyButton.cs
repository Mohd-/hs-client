using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AD9 RID: 2777
[CustomEditClass]
public class GeneralStorePackBuyButton : PegUIElement
{
	// Token: 0x06005FD5 RID: 24533 RVA: 0x001CB004 File Offset: 0x001C9204
	public bool IsSelected()
	{
		return this.m_selected;
	}

	// Token: 0x06005FD6 RID: 24534 RVA: 0x001CB00C File Offset: 0x001C920C
	public void Select()
	{
		if (this.m_selected)
		{
			return;
		}
		this.m_selected = true;
		this.UpdateButtonState();
	}

	// Token: 0x06005FD7 RID: 24535 RVA: 0x001CB027 File Offset: 0x001C9227
	public void Unselect()
	{
		if (!this.m_selected)
		{
			return;
		}
		this.m_selected = false;
		this.UpdateButtonState();
	}

	// Token: 0x06005FD8 RID: 24536 RVA: 0x001CB044 File Offset: 0x001C9244
	public void UpdateFromGTAPP(NoGTAPPTransactionData noGTAPPGoldPrice)
	{
		string quantityText = string.Empty;
		long goldCost;
		if (StoreManager.Get().GetGoldCostNoGTAPP(noGTAPPGoldPrice, out goldCost))
		{
			quantityText = StoreManager.Get().GetProductQuantityText(noGTAPPGoldPrice.Product, noGTAPPGoldPrice.ProductData, noGTAPPGoldPrice.Quantity);
		}
		this.SetGoldValue(goldCost, quantityText);
	}

	// Token: 0x06005FD9 RID: 24537 RVA: 0x001CB090 File Offset: 0x001C9290
	public void UpdateFromMoneyBundle(Network.Bundle bundle)
	{
		Network.BundleItem bundleItem = bundle.Items[0];
		string productQuantityText = StoreManager.Get().GetProductQuantityText(bundleItem.Product, bundleItem.ProductData, bundleItem.Quantity);
		this.SetMoneyValue(bundle, productQuantityText);
	}

	// Token: 0x06005FDA RID: 24538 RVA: 0x001CB0D0 File Offset: 0x001C92D0
	public void SetGoldValue(long goldCost, string quantityText)
	{
		if (this.m_fullText != null)
		{
			this.m_quantityText.gameObject.SetActive(true);
			this.m_costText.gameObject.SetActive(true);
			this.m_fullText.gameObject.SetActive(false);
		}
		this.m_costText.Text = goldCost.ToString();
		this.m_costText.TextColor = this.m_goldCostTextColor;
		this.m_quantityText.Text = quantityText;
		this.m_quantityText.TextColor = this.m_goldQuantityTextColor;
		this.m_isGold = true;
		this.UpdateButtonState();
	}

	// Token: 0x06005FDB RID: 24539 RVA: 0x001CB170 File Offset: 0x001C9370
	public void SetMoneyValue(Network.Bundle bundle, string quantityText)
	{
		if (bundle != null && !StoreManager.Get().IsProductAlreadyOwned(bundle))
		{
			if (this.m_fullText != null)
			{
				this.m_quantityText.gameObject.SetActive(true);
				this.m_costText.gameObject.SetActive(true);
				this.m_fullText.gameObject.SetActive(false);
			}
			this.m_costText.Text = StoreManager.Get().FormatCostBundle(bundle);
			this.m_costText.TextColor = this.m_moneyCostTextColor;
			this.m_quantityText.Text = quantityText;
			this.m_quantityText.TextColor = this.m_moneyQuantityTextColor;
		}
		else
		{
			this.m_costText.Text = string.Empty;
			UberText uberText = this.m_quantityText;
			if (this.m_fullText != null)
			{
				this.m_quantityText.gameObject.SetActive(false);
				this.m_costText.gameObject.SetActive(false);
				this.m_fullText.gameObject.SetActive(true);
				uberText = this.m_fullText;
			}
			uberText.Text = GameStrings.Get("GLUE_STORE_PACK_BUTTON_TEXT_PURCHASED");
		}
		this.m_isGold = false;
		this.UpdateButtonState();
	}

	// Token: 0x06005FDC RID: 24540 RVA: 0x001CB2A0 File Offset: 0x001C94A0
	private void UpdateButtonState()
	{
		if (this.m_goldIcon != null)
		{
			this.m_goldIcon.SetActive(this.m_isGold);
		}
		Vector2 vector = Vector2.zero;
		if (this.m_isGold)
		{
			vector = ((!this.m_selected) ? this.m_goldBtnMatOffset : this.m_goldBtnDownMatOffset);
		}
		else
		{
			vector = ((!this.m_selected) ? this.m_moneyBtnMatOffset : this.m_moneyBtnDownMatOffset);
		}
		foreach (Renderer renderer in this.m_buttonRenderers)
		{
			renderer.materials[this.m_materialIndex].SetTextureOffset(this.m_materialPropName, vector);
		}
		if (this.m_selectGlow != null)
		{
			this.m_selectGlow.SetActive(this.m_selected);
		}
	}

	// Token: 0x06005FDD RID: 24541 RVA: 0x001CB3A0 File Offset: 0x001C95A0
	protected override void OnDoubleClick()
	{
	}

	// Token: 0x04004741 RID: 18241
	public UberText m_quantityText;

	// Token: 0x04004742 RID: 18242
	public UberText m_costText;

	// Token: 0x04004743 RID: 18243
	public UberText m_fullText;

	// Token: 0x04004744 RID: 18244
	public Color m_goldQuantityTextColor;

	// Token: 0x04004745 RID: 18245
	public Color m_moneyQuantityTextColor;

	// Token: 0x04004746 RID: 18246
	public Color m_goldCostTextColor;

	// Token: 0x04004747 RID: 18247
	public Color m_moneyCostTextColor;

	// Token: 0x04004748 RID: 18248
	public GameObject m_goldIcon;

	// Token: 0x04004749 RID: 18249
	public GameObject m_selectGlow;

	// Token: 0x0400474A RID: 18250
	public List<Renderer> m_buttonRenderers = new List<Renderer>();

	// Token: 0x0400474B RID: 18251
	public int m_materialIndex;

	// Token: 0x0400474C RID: 18252
	public string m_materialPropName = "_MainTex";

	// Token: 0x0400474D RID: 18253
	public Vector2 m_goldBtnMatOffset;

	// Token: 0x0400474E RID: 18254
	public Vector2 m_goldBtnDownMatOffset;

	// Token: 0x0400474F RID: 18255
	public Vector2 m_moneyBtnMatOffset;

	// Token: 0x04004750 RID: 18256
	public Vector2 m_moneyBtnDownMatOffset;

	// Token: 0x04004751 RID: 18257
	private bool m_selected;

	// Token: 0x04004752 RID: 18258
	private bool m_isGold;
}
