using System;
using UnityEngine;

// Token: 0x02000B00 RID: 2816
public class StoreRadioButton : FramedRadioButton
{
	// Token: 0x0600608E RID: 24718 RVA: 0x001CE97B File Offset: 0x001CCB7B
	private void Awake()
	{
		this.ActivateSale(false);
	}

	// Token: 0x0600608F RID: 24719 RVA: 0x001CE984 File Offset: 0x001CCB84
	public override void Init(FramedRadioButton.FrameType frameType, string text, int buttonID, object userData)
	{
		base.Init(frameType, text, buttonID, userData);
		StoreRadioButton.Data data = userData as StoreRadioButton.Data;
		if (data == null)
		{
			Debug.LogWarning(string.Format("StoreRadioButton.Init(): storeRadioButtonData is null (frameType={0}, text={1}, buttonID={2)", frameType, text, buttonID));
			return;
		}
		if (data.m_bundle != null)
		{
			this.InitMoneyOption(data.m_bundle);
			return;
		}
		if (data.m_noGTAPPTransactionData != null)
		{
			this.InitGoldOptionNoGTAPP(data.m_noGTAPPTransactionData);
			return;
		}
		Debug.LogWarning(string.Format("StoreRadioButton.Init(): storeRadioButtonData has neither gold price nor bundle data! (frameType={0}, text={1}, buttonID={2)", frameType, text, buttonID));
	}

	// Token: 0x06006090 RID: 24720 RVA: 0x001CEA13 File Offset: 0x001CCC13
	public void ActivateSale(bool active)
	{
		this.m_saleBanner.m_root.SetActive(active);
		this.m_text.TextColor = ((!active) ? StoreRadioButton.NO_SALE_TEXT_COLOR : StoreRadioButton.ON_SALE_TEXT_COLOR);
	}

	// Token: 0x06006091 RID: 24721 RVA: 0x001CEA48 File Offset: 0x001CCC48
	private void InitMoneyOption(Network.Bundle bundle)
	{
		this.m_goldRoot.SetActive(false);
		this.m_realMoneyTextRoot.SetActive(true);
		this.m_bonusFrame.SetActive(false);
		this.m_cost.Text = string.Format(GameStrings.Get("GLUE_STORE_PRODUCT_PRICE"), StoreManager.Get().FormatCostBundle(bundle));
	}

	// Token: 0x06006092 RID: 24722 RVA: 0x001CEAA0 File Offset: 0x001CCCA0
	private void InitGoldOptionNoGTAPP(NoGTAPPTransactionData noGTAPPTransactionData)
	{
		string text = string.Empty;
		long num;
		if (StoreManager.Get().GetGoldCostNoGTAPP(noGTAPPTransactionData, out num))
		{
			text = num.ToString();
		}
		this.m_goldRoot.SetActive(true);
		this.m_realMoneyTextRoot.SetActive(false);
		this.m_goldButtonText.Text = this.m_text.Text;
		this.m_goldCostText.Text = text;
	}

	// Token: 0x04004821 RID: 18465
	public SaleBanner m_saleBanner;

	// Token: 0x04004822 RID: 18466
	public UberText m_cost;

	// Token: 0x04004823 RID: 18467
	public GameObject m_bonusFrame;

	// Token: 0x04004824 RID: 18468
	public UberText m_bonusText;

	// Token: 0x04004825 RID: 18469
	public GameObject m_realMoneyTextRoot;

	// Token: 0x04004826 RID: 18470
	public GameObject m_goldRoot;

	// Token: 0x04004827 RID: 18471
	public UberText m_goldCostText;

	// Token: 0x04004828 RID: 18472
	public UberText m_goldButtonText;

	// Token: 0x04004829 RID: 18473
	private static readonly Color NO_SALE_TEXT_COLOR = new Color(0.239f, 0.184f, 0.098f);

	// Token: 0x0400482A RID: 18474
	private static readonly Color ON_SALE_TEXT_COLOR = new Color(0.702f, 0.114f, 0.153f);

	// Token: 0x02000B02 RID: 2818
	public class Data
	{
		// Token: 0x04004833 RID: 18483
		public Network.Bundle m_bundle;

		// Token: 0x04004834 RID: 18484
		public NoGTAPPTransactionData m_noGTAPPTransactionData;
	}
}
