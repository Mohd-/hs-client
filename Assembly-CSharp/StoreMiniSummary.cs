using System;
using UnityEngine;

// Token: 0x02000AF7 RID: 2807
public class StoreMiniSummary : MonoBehaviour
{
	// Token: 0x06006075 RID: 24693 RVA: 0x001CE5F4 File Offset: 0x001CC7F4
	private void Awake()
	{
		this.m_headlineText.Text = GameStrings.Get("GLUE_STORE_SUMMARY_HEADLINE");
		this.m_itemsHeadlineText.Text = GameStrings.Get("GLUE_STORE_SUMMARY_ITEMS_ORDERED_HEADLINE");
	}

	// Token: 0x06006076 RID: 24694 RVA: 0x001CE62B File Offset: 0x001CC82B
	public void SetDetails(string productID, int quantity)
	{
		this.m_itemsText.Text = this.GetItemsText(productID, quantity);
	}

	// Token: 0x06006077 RID: 24695 RVA: 0x001CE640 File Offset: 0x001CC840
	private string GetItemsText(string productID, int quantity)
	{
		Network.Bundle bundle = StoreManager.Get().GetBundle(productID);
		string text;
		if (bundle == null)
		{
			text = GameStrings.Get("GLUE_STORE_PRODUCT_NAME_MOBILE_UNKNOWN");
		}
		else
		{
			text = StoreManager.Get().GetProductName(bundle.Items);
		}
		return GameStrings.Format("GLUE_STORE_SUMMARY_ITEM_ORDERED", new object[]
		{
			quantity,
			text
		});
	}

	// Token: 0x0400480A RID: 18442
	public UberText m_headlineText;

	// Token: 0x0400480B RID: 18443
	public UberText m_itemsHeadlineText;

	// Token: 0x0400480C RID: 18444
	public UberText m_itemsText;
}
