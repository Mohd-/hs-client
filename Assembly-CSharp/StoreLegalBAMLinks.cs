using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000409 RID: 1033
public class StoreLegalBAMLinks : UIBPopup
{
	// Token: 0x0600349B RID: 13467 RVA: 0x001065D8 File Offset: 0x001047D8
	private void Awake()
	{
		this.m_termsOfSaleButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTermsOfSalePressed));
		this.m_paymentMethodButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPaymentMethodPressed));
		this.m_offClickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickCatcherPressed));
	}

	// Token: 0x0600349C RID: 13468 RVA: 0x00106630 File Offset: 0x00104830
	public override void Show()
	{
		base.Show();
		this.EnableButtons(true);
		if (this.m_shown)
		{
			return;
		}
	}

	// Token: 0x0600349D RID: 13469 RVA: 0x0010664B File Offset: 0x0010484B
	public void RegisterSendToBAMListener(StoreLegalBAMLinks.SendToBAMListener listener)
	{
		if (this.m_sendToBAMListeners.Contains(listener))
		{
			return;
		}
		this.m_sendToBAMListeners.Add(listener);
	}

	// Token: 0x0600349E RID: 13470 RVA: 0x0010666B File Offset: 0x0010486B
	public void RemoveSendToBAMListener(StoreLegalBAMLinks.SendToBAMListener listener)
	{
		this.m_sendToBAMListeners.Remove(listener);
	}

	// Token: 0x0600349F RID: 13471 RVA: 0x0010667A File Offset: 0x0010487A
	public void RegisterCancelListener(StoreLegalBAMLinks.CancelListener listener)
	{
		if (this.m_cancelListeners.Contains(listener))
		{
			return;
		}
		this.m_cancelListeners.Add(listener);
	}

	// Token: 0x060034A0 RID: 13472 RVA: 0x0010669A File Offset: 0x0010489A
	public void RemoveCancelListener(StoreLegalBAMLinks.CancelListener listener)
	{
		this.m_cancelListeners.Remove(listener);
	}

	// Token: 0x060034A1 RID: 13473 RVA: 0x001066A9 File Offset: 0x001048A9
	private void OnTermsOfSalePressed(UIEvent e)
	{
		base.StopCoroutine(StoreLegalBAMLinks.SEND_TO_BAM_THEN_HIDE_COROUTINE);
		base.StartCoroutine(StoreLegalBAMLinks.SEND_TO_BAM_THEN_HIDE_COROUTINE, StoreLegalBAMLinks.BAMReason.READ_TERMS_OF_SALE);
	}

	// Token: 0x060034A2 RID: 13474 RVA: 0x001066C8 File Offset: 0x001048C8
	private void OnPaymentMethodPressed(UIEvent e)
	{
		base.StopCoroutine(StoreLegalBAMLinks.SEND_TO_BAM_THEN_HIDE_COROUTINE);
		base.StartCoroutine(StoreLegalBAMLinks.SEND_TO_BAM_THEN_HIDE_COROUTINE, StoreLegalBAMLinks.BAMReason.CHANGE_PAYMENT_METHOD);
	}

	// Token: 0x060034A3 RID: 13475 RVA: 0x001066E8 File Offset: 0x001048E8
	private void OnClickCatcherPressed(UIEvent e)
	{
		this.Hide(true);
		StoreLegalBAMLinks.CancelListener[] array = this.m_cancelListeners.ToArray();
		foreach (StoreLegalBAMLinks.CancelListener cancelListener in array)
		{
			cancelListener();
		}
	}

	// Token: 0x060034A4 RID: 13476 RVA: 0x00106728 File Offset: 0x00104928
	private IEnumerator SendToBAMThenHide(StoreLegalBAMLinks.BAMReason reason)
	{
		this.EnableButtons(false);
		StoreURL storeURL = null;
		if (reason != StoreLegalBAMLinks.BAMReason.CHANGE_PAYMENT_METHOD)
		{
			if (reason == StoreLegalBAMLinks.BAMReason.READ_TERMS_OF_SALE)
			{
				storeURL = StoreLegalBAMLinks.TERMS_OF_SALE_URL;
			}
		}
		else
		{
			storeURL = StoreLegalBAMLinks.CHANGE_PAYMENT_URL;
		}
		string url = string.Empty;
		if (storeURL == null)
		{
			Debug.LogError(string.Format("StoreLegalBAMLinks.SendToBAMThenHide(): could not get URL for reason {0}", reason));
		}
		else
		{
			url = storeURL.GetURL();
		}
		if (!string.IsNullOrEmpty(url))
		{
			Application.OpenURL(url);
		}
		yield return new WaitForSeconds(2f);
		this.Hide(true);
		StoreLegalBAMLinks.SendToBAMListener[] listeners = this.m_sendToBAMListeners.ToArray();
		foreach (StoreLegalBAMLinks.SendToBAMListener listener in listeners)
		{
			listener(reason);
		}
		yield break;
	}

	// Token: 0x060034A5 RID: 13477 RVA: 0x00106751 File Offset: 0x00104951
	private void EnableButtons(bool enabled)
	{
		this.m_termsOfSaleButton.SetEnabled(enabled);
		this.m_paymentMethodButton.SetEnabled(enabled);
	}

	// Token: 0x040020C3 RID: 8387
	public GameObject m_root;

	// Token: 0x040020C4 RID: 8388
	public UIBButton m_paymentMethodButton;

	// Token: 0x040020C5 RID: 8389
	public UIBButton m_termsOfSaleButton;

	// Token: 0x040020C6 RID: 8390
	public PegUIElement m_offClickCatcher;

	// Token: 0x040020C7 RID: 8391
	private static readonly string SEND_TO_BAM_THEN_HIDE_COROUTINE = "SendToBAMThenHide";

	// Token: 0x040020C8 RID: 8392
	private static readonly string FMT_URL_TERMS_OF_SALE = "https://nydus.battle.net/WTCG/{0}/client/legal/terms-of-sale?targetRegion={1}";

	// Token: 0x040020C9 RID: 8393
	private static readonly StoreURL TERMS_OF_SALE_URL = new StoreURL(StoreLegalBAMLinks.FMT_URL_TERMS_OF_SALE, StoreURL.Param.LOCALE, StoreURL.Param.REGION);

	// Token: 0x040020CA RID: 8394
	private static readonly string FMT_URL_CHANGE_PAYMENT = "https://nydus.battle.net/WTCG/{0}/client/choose-payment-method?targetRegion={1}";

	// Token: 0x040020CB RID: 8395
	private static readonly StoreURL CHANGE_PAYMENT_URL = new StoreURL(StoreLegalBAMLinks.FMT_URL_CHANGE_PAYMENT, StoreURL.Param.LOCALE, StoreURL.Param.REGION);

	// Token: 0x040020CC RID: 8396
	private List<StoreLegalBAMLinks.SendToBAMListener> m_sendToBAMListeners = new List<StoreLegalBAMLinks.SendToBAMListener>();

	// Token: 0x040020CD RID: 8397
	private List<StoreLegalBAMLinks.CancelListener> m_cancelListeners = new List<StoreLegalBAMLinks.CancelListener>();

	// Token: 0x02000417 RID: 1047
	public enum BAMReason
	{
		// Token: 0x04002184 RID: 8580
		CHANGE_PAYMENT_METHOD,
		// Token: 0x04002185 RID: 8581
		READ_TERMS_OF_SALE
	}

	// Token: 0x02000427 RID: 1063
	// (Invoke) Token: 0x060035F7 RID: 13815
	public delegate void SendToBAMListener(StoreLegalBAMLinks.BAMReason urlType);

	// Token: 0x02000428 RID: 1064
	// (Invoke) Token: 0x060035FB RID: 13819
	public delegate void CancelListener();
}
