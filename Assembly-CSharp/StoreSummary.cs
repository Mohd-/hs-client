using System;
using System.Collections.Generic;
using PegasusUtil;
using UnityEngine;

// Token: 0x02000407 RID: 1031
[CustomEditClass]
public class StoreSummary : UIBPopup
{
	// Token: 0x0600346C RID: 13420 RVA: 0x001054BC File Offset: 0x001036BC
	private void Awake()
	{
		this.m_confirmButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnConfirmPressed));
		this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
		if (this.m_offClickCatcher != null)
		{
			this.m_offClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
		}
		this.m_infoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInfoPressed));
		this.m_termsOfSaleButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTermsOfSalePressed));
		this.m_koreanAgreementCheckBox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleKoreanAgreement));
	}

	// Token: 0x0600346D RID: 13421 RVA: 0x00105570 File Offset: 0x00103770
	public void Show(string productID, int quantity, string paymentMethodName)
	{
		this.SetDetails(productID, quantity, paymentMethodName);
		this.Show();
	}

	// Token: 0x0600346E RID: 13422 RVA: 0x00105581 File Offset: 0x00103781
	public bool RegisterConfirmListener(StoreSummary.ConfirmCallback callback)
	{
		return this.RegisterConfirmListener(callback, null);
	}

	// Token: 0x0600346F RID: 13423 RVA: 0x0010558C File Offset: 0x0010378C
	public bool RegisterConfirmListener(StoreSummary.ConfirmCallback callback, object userData)
	{
		StoreSummary.ConfirmListener confirmListener = new StoreSummary.ConfirmListener();
		confirmListener.SetCallback(callback);
		confirmListener.SetUserData(userData);
		if (this.m_confirmListeners.Contains(confirmListener))
		{
			return false;
		}
		this.m_confirmListeners.Add(confirmListener);
		return true;
	}

	// Token: 0x06003470 RID: 13424 RVA: 0x001055CD File Offset: 0x001037CD
	public bool RemoveConfirmListener(StoreSummary.ConfirmCallback callback)
	{
		return this.RemoveConfirmListener(callback, null);
	}

	// Token: 0x06003471 RID: 13425 RVA: 0x001055D8 File Offset: 0x001037D8
	public bool RemoveConfirmListener(StoreSummary.ConfirmCallback callback, object userData)
	{
		StoreSummary.ConfirmListener confirmListener = new StoreSummary.ConfirmListener();
		confirmListener.SetCallback(callback);
		confirmListener.SetUserData(userData);
		return this.m_confirmListeners.Remove(confirmListener);
	}

	// Token: 0x06003472 RID: 13426 RVA: 0x00105605 File Offset: 0x00103805
	public bool RegisterCancelListener(StoreSummary.CancelCallback callback)
	{
		return this.RegisterCancelListener(callback, null);
	}

	// Token: 0x06003473 RID: 13427 RVA: 0x00105610 File Offset: 0x00103810
	public bool RegisterCancelListener(StoreSummary.CancelCallback callback, object userData)
	{
		StoreSummary.CancelListener cancelListener = new StoreSummary.CancelListener();
		cancelListener.SetCallback(callback);
		cancelListener.SetUserData(userData);
		if (this.m_cancelListeners.Contains(cancelListener))
		{
			return false;
		}
		this.m_cancelListeners.Add(cancelListener);
		return true;
	}

	// Token: 0x06003474 RID: 13428 RVA: 0x00105651 File Offset: 0x00103851
	public bool RemoveConfirmListener(StoreSummary.CancelCallback callback)
	{
		return this.RemoveConfirmListener(callback, null);
	}

	// Token: 0x06003475 RID: 13429 RVA: 0x0010565C File Offset: 0x0010385C
	public bool RemoveConfirmListener(StoreSummary.CancelCallback callback, object userData)
	{
		StoreSummary.CancelListener cancelListener = new StoreSummary.CancelListener();
		cancelListener.SetCallback(callback);
		cancelListener.SetUserData(userData);
		return this.m_cancelListeners.Remove(cancelListener);
	}

	// Token: 0x06003476 RID: 13430 RVA: 0x00105689 File Offset: 0x00103889
	public bool RegisterInfoListener(StoreSummary.InfoCallback callback)
	{
		return this.RegisterInfoListener(callback, null);
	}

	// Token: 0x06003477 RID: 13431 RVA: 0x00105694 File Offset: 0x00103894
	public bool RegisterInfoListener(StoreSummary.InfoCallback callback, object userData)
	{
		StoreSummary.InfoListener infoListener = new StoreSummary.InfoListener();
		infoListener.SetCallback(callback);
		infoListener.SetUserData(userData);
		if (this.m_infoListeners.Contains(infoListener))
		{
			return false;
		}
		this.m_infoListeners.Add(infoListener);
		return true;
	}

	// Token: 0x06003478 RID: 13432 RVA: 0x001056D5 File Offset: 0x001038D5
	public bool RemoveInfoListener(StoreSummary.InfoCallback callback)
	{
		return this.RemoveInfoListener(callback, null);
	}

	// Token: 0x06003479 RID: 13433 RVA: 0x001056E0 File Offset: 0x001038E0
	public bool RemoveInfoListener(StoreSummary.InfoCallback callback, object userData)
	{
		StoreSummary.InfoListener infoListener = new StoreSummary.InfoListener();
		infoListener.SetCallback(callback);
		infoListener.SetUserData(userData);
		return this.m_infoListeners.Remove(infoListener);
	}

	// Token: 0x0600347A RID: 13434 RVA: 0x0010570D File Offset: 0x0010390D
	public bool RegisterPaymentAndTOSListener(StoreSummary.PaymentAndTOSCallback callback)
	{
		return this.RegisterPaymentAndTOSListener(callback, null);
	}

	// Token: 0x0600347B RID: 13435 RVA: 0x00105718 File Offset: 0x00103918
	public bool RegisterPaymentAndTOSListener(StoreSummary.PaymentAndTOSCallback callback, object userData)
	{
		StoreSummary.PaymentAndTOSListener paymentAndTOSListener = new StoreSummary.PaymentAndTOSListener();
		paymentAndTOSListener.SetCallback(callback);
		paymentAndTOSListener.SetUserData(userData);
		if (this.m_paymentAndTOSListeners.Contains(paymentAndTOSListener))
		{
			return false;
		}
		this.m_paymentAndTOSListeners.Add(paymentAndTOSListener);
		return true;
	}

	// Token: 0x0600347C RID: 13436 RVA: 0x00105759 File Offset: 0x00103959
	public bool RemovePaymentAndTOSListener(StoreSummary.PaymentAndTOSCallback callback)
	{
		return this.RemovePaymentAndTOSListener(callback, null);
	}

	// Token: 0x0600347D RID: 13437 RVA: 0x00105764 File Offset: 0x00103964
	public bool RemovePaymentAndTOSListener(StoreSummary.PaymentAndTOSCallback callback, object userData)
	{
		StoreSummary.PaymentAndTOSListener paymentAndTOSListener = new StoreSummary.PaymentAndTOSListener();
		paymentAndTOSListener.SetCallback(callback);
		paymentAndTOSListener.SetUserData(userData);
		return this.m_paymentAndTOSListeners.Remove(paymentAndTOSListener);
	}

	// Token: 0x0600347E RID: 13438 RVA: 0x00105794 File Offset: 0x00103994
	private void SetDetails(string productID, int quantity, string paymentMethodName)
	{
		this.m_bundle = StoreManager.Get().GetBundle(productID);
		this.m_quantity = quantity;
		this.m_itemsText.Text = this.GetItemsText();
		this.m_priceText.Text = this.GetPriceText();
		this.m_taxDisclaimerText.Text = StoreManager.Get().GetTaxText();
		this.m_chargeDetailsText.Text = ((paymentMethodName != null) ? GameStrings.Format("GLUE_STORE_SUMMARY_CHARGE_DETAILS", new object[]
		{
			paymentMethodName
		}) : string.Empty);
		string text = string.Empty;
		HashSet<ProductType> productsInBundle = StoreManager.Get().GetProductsInBundle(this.m_bundle);
		if (productsInBundle.Contains(1))
		{
			if (StoreManager.Get().IsProductPrePurchase(this.m_bundle))
			{
				text = GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_PACK_PREORDER");
			}
			else
			{
				text = GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_EXPERT_PACK");
			}
		}
		else if (productsInBundle.Contains(2))
		{
			text = GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_FORGE_TICKET");
		}
		else if (productsInBundle.Contains(3) || productsInBundle.Contains(4) || productsInBundle.Contains(7))
		{
			if (this.m_bundle.Items.Count == 1)
			{
				text = GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_ADVENTURE_SINGLE");
			}
			else
			{
				text = GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_ADVENTURE_BUNDLE");
			}
		}
		else if (productsInBundle.Contains(6))
		{
			text = GameStrings.Get("GLUE_STORE_SUMMARY_KOREAN_AGREEMENT_HERO");
		}
		this.m_koreanAgreementTermsText.Text = text;
	}

	// Token: 0x0600347F RID: 13439 RVA: 0x00105910 File Offset: 0x00103B10
	private string GetItemsText()
	{
		string productName = StoreManager.Get().GetProductName(this.m_bundle.Items);
		return GameStrings.Format("GLUE_STORE_SUMMARY_ITEM_ORDERED", new object[]
		{
			this.m_quantity,
			productName
		});
	}

	// Token: 0x06003480 RID: 13440 RVA: 0x00105958 File Offset: 0x00103B58
	private string GetPriceText()
	{
		if (this.m_bundle.Cost == null)
		{
			return string.Empty;
		}
		double cost = this.m_bundle.Cost.Value * (double)this.m_quantity;
		return StoreManager.Get().FormatCostText(cost);
	}

	// Token: 0x06003481 RID: 13441 RVA: 0x001059AC File Offset: 0x00103BAC
	public override void Show()
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_infoButton.SetEnabled(true);
		this.m_termsOfSaleButton.SetEnabled(true);
		bool flag = StoreManager.Get().IsEuropeanCustomer();
		if (!this.m_textInitialized)
		{
			this.m_textInitialized = true;
			this.m_headlineText.Text = GameStrings.Get("GLUE_STORE_SUMMARY_HEADLINE");
			this.m_itemsHeadlineText.Text = GameStrings.Get("GLUE_STORE_SUMMARY_ITEMS_ORDERED_HEADLINE");
			this.m_priceHeadlineText.Text = GameStrings.Get("GLUE_STORE_SUMMARY_PRICE_HEADLINE");
			this.m_infoButton.SetText(GameStrings.Get("GLUE_STORE_INFO_BUTTON_TEXT"));
			string text = GameStrings.Get("GLUE_STORE_TERMS_OF_SALE_BUTTON_TEXT");
			this.m_termsOfSaleButton.SetText(text);
			string text2 = GameStrings.Get("GLUE_STORE_SUMMARY_PAY_NOW_TEXT");
			this.m_confirmButton.SetText(text2);
			this.m_cancelButton.SetText(GameStrings.Get("GLOBAL_CANCEL"));
			string key = (!flag) ? "GLUE_STORE_SUMMARY_TOS_AGREEMENT" : "GLUE_STORE_SUMMARY_TOS_AGREEMENT_EU";
			this.m_termsOfSaleAgreementText.Text = GameStrings.Format(key, new object[]
			{
				text2,
				text
			});
		}
		if (StoreManager.Get().IsKoreanCustomer())
		{
			this.m_bottomSectionRoot.transform.localPosition = this.m_koreanBottomSectionBone.localPosition;
			this.m_koreanRequirementRoot.gameObject.SetActive(true);
			this.m_koreanAgreementCheckBox.SetChecked(false);
			this.m_infoButton.gameObject.SetActive(true);
			this.EnableConfirmButton(false);
		}
		else
		{
			this.m_koreanRequirementRoot.gameObject.SetActive(false);
			this.m_infoButton.gameObject.SetActive(false);
			this.EnableConfirmButton(true);
		}
		if (flag || StoreManager.Get().IsNorthAmericanCustomer())
		{
			this.m_bottomSectionRoot.transform.localPosition = this.m_termsOfSaleBottomSectionBone.localPosition;
			this.m_termsOfSaleRoot.gameObject.SetActive(true);
			this.m_termsOfSaleButton.gameObject.SetActive(true);
		}
		else
		{
			this.m_termsOfSaleRoot.gameObject.SetActive(false);
			this.m_termsOfSaleButton.gameObject.SetActive(false);
		}
		this.PreRender();
		this.m_shown = true;
		base.DoShowAnimation(null);
	}

	// Token: 0x06003482 RID: 13442 RVA: 0x00105BE0 File Offset: 0x00103DE0
	protected override void Hide(bool animate)
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_termsOfSaleButton.SetEnabled(false);
		this.m_infoButton.SetEnabled(false);
		this.m_shown = false;
		base.DoHideAnimation(!animate, null);
	}

	// Token: 0x06003483 RID: 13443 RVA: 0x00105C18 File Offset: 0x00103E18
	private void OnConfirmPressed(UIEvent e)
	{
		if (!this.m_confirmButtonEnabled)
		{
			return;
		}
		this.Hide(true);
		StoreSummary.ConfirmListener[] array = this.m_confirmListeners.ToArray();
		foreach (StoreSummary.ConfirmListener confirmListener in array)
		{
			confirmListener.Fire(this.m_bundle.ProductID, this.m_quantity);
		}
	}

	// Token: 0x06003484 RID: 13444 RVA: 0x00105C78 File Offset: 0x00103E78
	private void OnCancelPressed(UIEvent e)
	{
		this.Hide(true);
		StoreSummary.CancelListener[] array = this.m_cancelListeners.ToArray();
		foreach (StoreSummary.CancelListener cancelListener in array)
		{
			cancelListener.Fire();
		}
	}

	// Token: 0x06003485 RID: 13445 RVA: 0x00105CB8 File Offset: 0x00103EB8
	private void OnInfoPressed(UIEvent e)
	{
		this.Hide(true);
		StoreSummary.InfoListener[] array = this.m_infoListeners.ToArray();
		foreach (StoreSummary.InfoListener infoListener in array)
		{
			infoListener.Fire();
		}
	}

	// Token: 0x06003486 RID: 13446 RVA: 0x00105CF8 File Offset: 0x00103EF8
	private void OnTermsOfSalePressed(UIEvent e)
	{
		this.Hide(true);
		StoreSummary.PaymentAndTOSListener[] array = this.m_paymentAndTOSListeners.ToArray();
		foreach (StoreSummary.PaymentAndTOSListener paymentAndTOSListener in array)
		{
			paymentAndTOSListener.Fire();
		}
	}

	// Token: 0x06003487 RID: 13447 RVA: 0x00105D38 File Offset: 0x00103F38
	private void PreRender()
	{
		this.m_itemsText.UpdateNow();
		this.m_priceText.UpdateNow();
		this.m_koreanAgreementTermsText.UpdateNow();
		if (this.m_staticTextResized)
		{
			return;
		}
		this.m_headlineText.UpdateNow();
		this.m_itemsHeadlineText.UpdateNow();
		this.m_priceHeadlineText.UpdateNow();
		this.m_taxDisclaimerText.UpdateNow();
		this.m_koreanAgreementCheckBox.m_uberText.UpdateNow();
		this.m_staticTextResized = true;
	}

	// Token: 0x06003488 RID: 13448 RVA: 0x00105DB8 File Offset: 0x00103FB8
	private void ToggleKoreanAgreement(UIEvent e)
	{
		bool enabled = this.m_koreanAgreementCheckBox.IsChecked();
		this.EnableConfirmButton(enabled);
	}

	// Token: 0x06003489 RID: 13449 RVA: 0x00105DD8 File Offset: 0x00103FD8
	private void EnableConfirmButton(bool enabled)
	{
		this.m_confirmButtonEnabled = enabled;
		Material material = (!this.m_confirmButtonEnabled) ? this.m_disabledConfirmButtonMaterial : this.m_enabledConfirmButtonMaterial;
		MultiSliceElement[] componentsInChildren = this.m_confirmButton.m_RootObject.GetComponentsInChildren<MultiSliceElement>();
		foreach (MultiSliceElement multiSliceElement in componentsInChildren)
		{
			foreach (MultiSliceElement.Slice slice in multiSliceElement.m_slices)
			{
				MeshRenderer component = slice.m_slice.GetComponent<MeshRenderer>();
				if (component != null)
				{
					component.material = material;
				}
			}
		}
		Material material2 = (!this.m_confirmButtonEnabled) ? this.m_disabledConfirmCheckMarkMaterial : this.m_enabledConfirmCheckMarkMaterial;
		this.m_confirmButtonCheckMark.GetComponent<MeshRenderer>().material = material2;
		Color textColor = (!this.m_confirmButtonEnabled) ? StoreSummary.DISABLED_CONFIRM_BUTTON_TEXT_COLOR : StoreSummary.ENABLED_CONFIRM_BUTTON_TEXT_COLOR;
		this.m_confirmButton.m_ButtonText.TextColor = textColor;
	}

	// Token: 0x04002084 RID: 8324
	[CustomEditField(Sections = "Buttons")]
	public UIBButton m_confirmButton;

	// Token: 0x04002085 RID: 8325
	[CustomEditField(Sections = "Buttons")]
	public UIBButton m_cancelButton;

	// Token: 0x04002086 RID: 8326
	[CustomEditField(Sections = "Buttons")]
	public UIBButton m_infoButton;

	// Token: 0x04002087 RID: 8327
	[CustomEditField(Sections = "Buttons")]
	public UIBButton m_termsOfSaleButton;

	// Token: 0x04002088 RID: 8328
	[CustomEditField(Sections = "Text")]
	public UberText m_headlineText;

	// Token: 0x04002089 RID: 8329
	[CustomEditField(Sections = "Text")]
	public UberText m_itemsHeadlineText;

	// Token: 0x0400208A RID: 8330
	[CustomEditField(Sections = "Text")]
	public UberText m_itemsText;

	// Token: 0x0400208B RID: 8331
	[CustomEditField(Sections = "Text")]
	public UberText m_priceHeadlineText;

	// Token: 0x0400208C RID: 8332
	[CustomEditField(Sections = "Text")]
	public UberText m_priceText;

	// Token: 0x0400208D RID: 8333
	[CustomEditField(Sections = "Text")]
	public UberText m_taxDisclaimerText;

	// Token: 0x0400208E RID: 8334
	[CustomEditField(Sections = "Text")]
	public UberText m_chargeDetailsText;

	// Token: 0x0400208F RID: 8335
	[CustomEditField(Sections = "Objects")]
	public GameObject m_bottomSectionRoot;

	// Token: 0x04002090 RID: 8336
	[CustomEditField(Sections = "Objects")]
	public GameObject m_confirmButtonCheckMark;

	// Token: 0x04002091 RID: 8337
	[CustomEditField(Sections = "Materials")]
	public Material m_enabledConfirmButtonMaterial;

	// Token: 0x04002092 RID: 8338
	[CustomEditField(Sections = "Materials")]
	public Material m_disabledConfirmButtonMaterial;

	// Token: 0x04002093 RID: 8339
	[CustomEditField(Sections = "Materials")]
	public Material m_enabledConfirmCheckMarkMaterial;

	// Token: 0x04002094 RID: 8340
	[CustomEditField(Sections = "Materials")]
	public Material m_disabledConfirmCheckMarkMaterial;

	// Token: 0x04002095 RID: 8341
	[CustomEditField(Sections = "Korean Specific Info")]
	public GameObject m_koreanRequirementRoot;

	// Token: 0x04002096 RID: 8342
	[CustomEditField(Sections = "Korean Specific Info")]
	public Transform m_koreanBottomSectionBone;

	// Token: 0x04002097 RID: 8343
	[CustomEditField(Sections = "Korean Specific Info")]
	public UberText m_koreanAgreementTermsText;

	// Token: 0x04002098 RID: 8344
	[CustomEditField(Sections = "Korean Specific Info")]
	public CheckBox m_koreanAgreementCheckBox;

	// Token: 0x04002099 RID: 8345
	[CustomEditField(Sections = "Terms of Sale")]
	public GameObject m_termsOfSaleRoot;

	// Token: 0x0400209A RID: 8346
	[CustomEditField(Sections = "Terms of Sale")]
	public Transform m_termsOfSaleBottomSectionBone;

	// Token: 0x0400209B RID: 8347
	[CustomEditField(Sections = "Terms of Sale")]
	public UberText m_termsOfSaleAgreementText;

	// Token: 0x0400209C RID: 8348
	[CustomEditField(Sections = "Click Catchers")]
	public PegUIElement m_offClickCatcher;

	// Token: 0x0400209D RID: 8349
	private static readonly Color ENABLED_CONFIRM_BUTTON_TEXT_COLOR = new Color(0.239f, 0.184f, 0.098f);

	// Token: 0x0400209E RID: 8350
	private static readonly Color DISABLED_CONFIRM_BUTTON_TEXT_COLOR = new Color(0.1176f, 0.1176f, 0.1176f);

	// Token: 0x0400209F RID: 8351
	private Network.Bundle m_bundle;

	// Token: 0x040020A0 RID: 8352
	private int m_quantity;

	// Token: 0x040020A1 RID: 8353
	private bool m_staticTextResized;

	// Token: 0x040020A2 RID: 8354
	private bool m_confirmButtonEnabled = true;

	// Token: 0x040020A3 RID: 8355
	private bool m_textInitialized;

	// Token: 0x040020A4 RID: 8356
	private List<StoreSummary.ConfirmListener> m_confirmListeners = new List<StoreSummary.ConfirmListener>();

	// Token: 0x040020A5 RID: 8357
	private List<StoreSummary.CancelListener> m_cancelListeners = new List<StoreSummary.CancelListener>();

	// Token: 0x040020A6 RID: 8358
	private List<StoreSummary.InfoListener> m_infoListeners = new List<StoreSummary.InfoListener>();

	// Token: 0x040020A7 RID: 8359
	private List<StoreSummary.PaymentAndTOSListener> m_paymentAndTOSListeners = new List<StoreSummary.PaymentAndTOSListener>();

	// Token: 0x02000421 RID: 1057
	// (Invoke) Token: 0x060035DF RID: 13791
	public delegate void ConfirmCallback(string productID, int quantity, object userData);

	// Token: 0x02000422 RID: 1058
	// (Invoke) Token: 0x060035E3 RID: 13795
	public delegate void CancelCallback(object userData);

	// Token: 0x02000423 RID: 1059
	// (Invoke) Token: 0x060035E7 RID: 13799
	public delegate void InfoCallback(object userData);

	// Token: 0x02000424 RID: 1060
	// (Invoke) Token: 0x060035EB RID: 13803
	public delegate void PaymentAndTOSCallback(object userData);

	// Token: 0x02000AF8 RID: 2808
	private class ConfirmListener : EventListener<StoreSummary.ConfirmCallback>
	{
		// Token: 0x06006079 RID: 24697 RVA: 0x001CE6A7 File Offset: 0x001CC8A7
		public void Fire(string productID, int quantity)
		{
			this.m_callback(productID, quantity, this.m_userData);
		}
	}

	// Token: 0x02000AF9 RID: 2809
	private class CancelListener : EventListener<StoreSummary.CancelCallback>
	{
		// Token: 0x0600607B RID: 24699 RVA: 0x001CE6C4 File Offset: 0x001CC8C4
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x02000AFA RID: 2810
	private class InfoListener : EventListener<StoreSummary.InfoCallback>
	{
		// Token: 0x0600607D RID: 24701 RVA: 0x001CE6DF File Offset: 0x001CC8DF
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x02000AFB RID: 2811
	private class PaymentAndTOSListener : EventListener<StoreSummary.PaymentAndTOSCallback>
	{
		// Token: 0x0600607F RID: 24703 RVA: 0x001CE6FA File Offset: 0x001CC8FA
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}
}
