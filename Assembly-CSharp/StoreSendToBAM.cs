using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000408 RID: 1032
[CustomEditClass]
public class StoreSendToBAM : UIBPopup
{
	// Token: 0x0600348C RID: 13452 RVA: 0x00106008 File Offset: 0x00104208
	private void Awake()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_scaleMode = CanvasScaleMode.WIDTH;
		}
		StoreSendToBAM.s_bamTextMap = new Map<StoreSendToBAM.BAMReason, StoreSendToBAM.SendToBAMText>
		{
			{
				StoreSendToBAM.BAMReason.PAYMENT_INFO,
				new StoreSendToBAM.SendToBAMText("GLUE_STORE_PAYMENT_INFO_HEADLINE", StoreSendToBAM.GLUE_STORE_PAYMENT_INFO_DETAILS, StoreSendToBAM.GLUE_STORE_PAYMENT_INFO_URL_DETAILS, StoreSendToBAM.FMT_URL_PAYMENT_INFO, StoreURL.Param.LOCALE, StoreURL.Param.REGION)
			},
			{
				StoreSendToBAM.BAMReason.NEED_PASSWORD_RESET,
				new StoreSendToBAM.SendToBAMText("GLUE_STORE_FORGOT_PWD_HEADLINE", "GLUE_STORE_FORGOT_PWD_DETAILS", "GLUE_STORE_FORGOT_PWD_URL_DETAILS", StoreSendToBAM.FMT_URL_RESET_PASSWORD, StoreURL.Param.LOCALE, StoreURL.Param.REGION)
			},
			{
				StoreSendToBAM.BAMReason.NO_VALID_PAYMENT_METHOD,
				new StoreSendToBAM.SendToBAMText("GLUE_STORE_NO_PAYMENT_HEADLINE", "GLUE_STORE_NO_PAYMENT_DETAILS", "GLUE_STORE_NO_PAYMENT_URL_DETAILS", "https://nydus.battle.net/WTCG/{0}/client/add-payment?targetRegion={1}&flowId=1", StoreURL.Param.LOCALE, StoreURL.Param.REGION)
			},
			{
				StoreSendToBAM.BAMReason.CREDIT_CARD_EXPIRED,
				new StoreSendToBAM.SendToBAMText("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE", "GLUE_STORE_CC_EXPIRY_DETAILS", "GLUE_STORE_GENERIC_BP_FAIL_URL_DETAILS", "https://nydus.battle.net/WTCG/{0}/client/add-payment?targetRegion={1}&flowId=5", StoreURL.Param.LOCALE, StoreURL.Param.REGION)
			},
			{
				StoreSendToBAM.BAMReason.GENERIC_PAYMENT_FAIL,
				new StoreSendToBAM.SendToBAMText("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE", "GLUE_STORE_GENERIC_BP_FAIL_DETAILS", "GLUE_STORE_GENERIC_BP_FAIL_URL_DETAILS", "https://nydus.battle.net/WTCG/{0}/client/add-payment?targetRegion={1}&flowId=4", StoreURL.Param.LOCALE, StoreURL.Param.REGION)
			},
			{
				StoreSendToBAM.BAMReason.EULA_AND_TOS,
				new StoreSendToBAM.SendToBAMText("GLUE_STORE_EULA_AND_TOS_HEADLINE", "GLUE_STORE_EULA_AND_TOS_DETAILS", "GLUE_STORE_EULA_AND_TOS_URL_DETAILS", "https://nydus.battle.net/WTCG/{0}/client/legal/terms-of-sale?targetRegion={1}", StoreURL.Param.LOCALE, StoreURL.Param.REGION)
			},
			{
				StoreSendToBAM.BAMReason.PRODUCT_UNIQUENESS_VIOLATED,
				new StoreSendToBAM.SendToBAMText("GLUE_STORE_PURCHASE_LOCK_HEADER", "GLUE_STORE_FAIL_PRODUCT_UNIQUENESS_VIOLATED", "GLUE_STORE_FAIL_PRODUCT_UNIQUENESS_VIOLATED_URL", "https://nydus.battle.net/WTCG/{0}/client/support/already-owned?targetRegion={1}", StoreURL.Param.LOCALE, StoreURL.Param.REGION)
			},
			{
				StoreSendToBAM.BAMReason.GENERIC_PURCHASE_FAIL_RETRY_CONTACT_CS_IF_PERSISTS,
				new StoreSendToBAM.SendToBAMText("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE", "GLUE_STORE_GENERIC_BP_FAIL_RETRY_CONTACT_CS_IF_PERSISTS_DETAILS", "GLUE_STORE_GENERIC_BP_FAIL_RETRY_CONTACT_CS_IF_PERSISTS_URL_DETAILS", "https://nydus.battle.net/WTCG/{0}/client/add-payment?targetRegion={1}&flowId=4", StoreURL.Param.LOCALE, StoreURL.Param.REGION)
			}
		};
		this.m_okayButton.SetText(GameStrings.Get("GLOBAL_MORE"));
		this.m_cancelButton.SetText(GameStrings.Get("GLOBAL_CANCEL"));
		this.m_okayButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOkayPressed));
		this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
	}

	// Token: 0x0600348D RID: 13453 RVA: 0x001061B4 File Offset: 0x001043B4
	public void Show(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, StoreSendToBAM.BAMReason reason, string errorCode, bool fromPreviousPurchase)
	{
		this.m_moneyOrGTAPPTransaction = moneyOrGTAPPTransaction;
		this.m_sendToBAMReason = reason;
		this.m_errorCode = errorCode;
		this.UpdateText();
		bool flag = moneyOrGTAPPTransaction != null && (fromPreviousPurchase || moneyOrGTAPPTransaction.ShouldShowMiniSummary());
		if (flag)
		{
			this.m_sendToBAMRoot.transform.position = this.m_sendToBAMRootWithSummaryBone.position;
			this.m_miniSummary.SetDetails(this.m_moneyOrGTAPPTransaction.ProductID, 1);
			this.m_miniSummary.gameObject.SetActive(true);
			if (UniversalInputManager.UsePhoneUI)
			{
				this.m_originalShowScale = this.m_showScale;
				this.m_showScale = StoreSendToBAM.SHOW_MINI_SUMMARY_SCALE_PHONE;
			}
		}
		else
		{
			this.m_sendToBAMRoot.transform.localPosition = Vector3.zero;
			this.m_miniSummary.gameObject.SetActive(false);
			if (UniversalInputManager.UsePhoneUI && this.m_originalShowScale != Vector3.zero)
			{
				this.m_showScale = this.m_originalShowScale;
				this.m_originalShowScale = Vector3.zero;
			}
		}
		if (this.m_shown)
		{
			return;
		}
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnCancel));
		this.m_shown = true;
		this.m_headlineText.UpdateNow();
		this.LayoutMessageText();
		base.DoShowAnimation(null);
	}

	// Token: 0x0600348E RID: 13454 RVA: 0x00106307 File Offset: 0x00104507
	public void RegisterOkayListener(StoreSendToBAM.DelOKListener listener)
	{
		if (this.m_okayListeners.Contains(listener))
		{
			return;
		}
		this.m_okayListeners.Add(listener);
	}

	// Token: 0x0600348F RID: 13455 RVA: 0x00106327 File Offset: 0x00104527
	public void RemoveOkayListener(StoreSendToBAM.DelOKListener listener)
	{
		this.m_okayListeners.Remove(listener);
	}

	// Token: 0x06003490 RID: 13456 RVA: 0x00106336 File Offset: 0x00104536
	public void RegisterCancelListener(StoreSendToBAM.DelCancelListener listener)
	{
		if (this.m_cancelListeners.Contains(listener))
		{
			return;
		}
		this.m_cancelListeners.Add(listener);
	}

	// Token: 0x06003491 RID: 13457 RVA: 0x00106356 File Offset: 0x00104556
	public void RemoveCancelListener(StoreSendToBAM.DelCancelListener listener)
	{
		this.m_cancelListeners.Remove(listener);
	}

	// Token: 0x06003492 RID: 13458 RVA: 0x00106365 File Offset: 0x00104565
	protected override void OnHidden()
	{
		this.m_okayButton.SetEnabled(true);
	}

	// Token: 0x06003493 RID: 13459 RVA: 0x00106373 File Offset: 0x00104573
	private void OnOkayPressed(UIEvent e)
	{
		base.StopCoroutine(StoreSendToBAM.SEND_TO_BAM_THEN_HIDE_COROUTINE);
		base.StartCoroutine(StoreSendToBAM.SEND_TO_BAM_THEN_HIDE_COROUTINE);
	}

	// Token: 0x06003494 RID: 13460 RVA: 0x0010638C File Offset: 0x0010458C
	private IEnumerator SendToBAMThenHide()
	{
		this.m_okayButton.SetEnabled(false);
		string url = string.Empty;
		StoreSendToBAM.SendToBAMText sendToBAMText = StoreSendToBAM.s_bamTextMap[this.m_sendToBAMReason];
		if (sendToBAMText == null)
		{
			Debug.LogError(string.Format("StoreSendToBAM.SendToBAMThenHide(): can't get URL for BAM reason {0}", this.m_sendToBAMReason));
		}
		else
		{
			url = sendToBAMText.GetURL();
		}
		if (!string.IsNullOrEmpty(url))
		{
			Application.OpenURL(url);
		}
		yield return new WaitForSeconds(2f);
		Navigation.Pop();
		this.Hide(true);
		StoreSendToBAM.DelOKListener[] listeners = this.m_okayListeners.ToArray();
		foreach (StoreSendToBAM.DelOKListener listener in listeners)
		{
			listener(this.m_moneyOrGTAPPTransaction, this.m_sendToBAMReason);
		}
		yield break;
	}

	// Token: 0x06003495 RID: 13461 RVA: 0x001063A8 File Offset: 0x001045A8
	private bool OnCancel()
	{
		base.StopCoroutine(StoreSendToBAM.SEND_TO_BAM_THEN_HIDE_COROUTINE);
		this.Hide(true);
		StoreSendToBAM.DelCancelListener[] array = this.m_cancelListeners.ToArray();
		foreach (StoreSendToBAM.DelCancelListener delCancelListener in array)
		{
			delCancelListener(this.m_moneyOrGTAPPTransaction);
		}
		return true;
	}

	// Token: 0x06003496 RID: 13462 RVA: 0x001063FA File Offset: 0x001045FA
	private void OnCancelPressed(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x06003497 RID: 13463 RVA: 0x00106404 File Offset: 0x00104604
	private void UpdateText()
	{
		StoreSendToBAM.SendToBAMText sendToBAMText = StoreSendToBAM.s_bamTextMap[this.m_sendToBAMReason];
		if (sendToBAMText == null)
		{
			Debug.LogError(string.Format("StoreSendToBAM.UpdateText(): don't know how to update text for BAM reason {0}", this.m_sendToBAMReason));
			this.m_headlineText.Text = string.Empty;
			this.m_messageText.Text = string.Empty;
			return;
		}
		string text = sendToBAMText.GetDetails();
		if (!string.IsNullOrEmpty(this.m_errorCode))
		{
			text = text + " " + GameStrings.Format("GLUE_STORE_FAIL_DETAILS_ERROR_CODE", new object[]
			{
				this.m_errorCode
			});
		}
		text += "\n\n";
		text += sendToBAMText.GetGoToURLDetails(this.m_okayButton.m_ButtonText.Text);
		this.m_headlineText.Text = sendToBAMText.GetHeadline();
		this.m_messageText.Text = text;
	}

	// Token: 0x06003498 RID: 13464 RVA: 0x001064E8 File Offset: 0x001046E8
	private void LayoutMessageText()
	{
		this.m_messageText.UpdateNow();
		TransformUtil.SetLocalScaleZ(this.m_midSection, 1f);
		OrientedBounds orientedBounds = TransformUtil.ComputeOrientedWorldBounds(this.m_midSection, true);
		float num = orientedBounds.Extents[2].magnitude * 2f;
		Bounds textWorldSpaceBounds = this.m_messageText.GetTextWorldSpaceBounds();
		TransformUtil.SetLocalScaleZ(this.m_midSection, textWorldSpaceBounds.size.z / num);
		this.m_allSections.UpdateSlices();
	}

	// Token: 0x040020A8 RID: 8360
	private const string FMT_URL_NO_PAYMENT_METHOD = "https://nydus.battle.net/WTCG/{0}/client/add-payment?targetRegion={1}&flowId=1";

	// Token: 0x040020A9 RID: 8361
	private const string FMT_URL_PAYMENT_EXPIRED = "https://nydus.battle.net/WTCG/{0}/client/add-payment?targetRegion={1}&flowId=5";

	// Token: 0x040020AA RID: 8362
	private const string FMT_URL_GENERIC_PAYMENT_FAIL = "https://nydus.battle.net/WTCG/{0}/client/add-payment?targetRegion={1}&flowId=4";

	// Token: 0x040020AB RID: 8363
	private const string FMT_URL_EULA_AND_TOS = "https://nydus.battle.net/WTCG/{0}/client/legal/terms-of-sale?targetRegion={1}";

	// Token: 0x040020AC RID: 8364
	private const string FMT_URL_PURCHASE_UNIQUENESS_VIOLATED = "https://nydus.battle.net/WTCG/{0}/client/support/already-owned?targetRegion={1}";

	// Token: 0x040020AD RID: 8365
	public UIBButton m_okayButton;

	// Token: 0x040020AE RID: 8366
	public UIBButton m_cancelButton;

	// Token: 0x040020AF RID: 8367
	public UberText m_headlineText;

	// Token: 0x040020B0 RID: 8368
	public UberText m_messageText;

	// Token: 0x040020B1 RID: 8369
	public MultiSliceElement m_allSections;

	// Token: 0x040020B2 RID: 8370
	public GameObject m_midSection;

	// Token: 0x040020B3 RID: 8371
	public GameObject m_sendToBAMRoot;

	// Token: 0x040020B4 RID: 8372
	public Transform m_sendToBAMRootWithSummaryBone;

	// Token: 0x040020B5 RID: 8373
	public StoreMiniSummary m_miniSummary;

	// Token: 0x040020B6 RID: 8374
	private static readonly string SEND_TO_BAM_THEN_HIDE_COROUTINE = "SendToBAMThenHide";

	// Token: 0x040020B7 RID: 8375
	private static readonly string FMT_URL_RESET_PASSWORD = "https://nydus.battle.net/WTCG/{0}/client/password-reset?targetRegion={1}";

	// Token: 0x040020B8 RID: 8376
	private static readonly PlatformDependentValue<string> FMT_URL_PAYMENT_INFO = new PlatformDependentValue<string>(PlatformCategory.OS)
	{
		PC = "https://nydus.battle.net/WTCG/{0}/client/support/purchase?targetRegion={1}",
		iOS = "https://nydus.battle.net/WTCG/{0}/client/support/purchase?targetRegion={1}&targetDevice=ipad",
		Android = "https://nydus.battle.net/WTCG/{0}/client/support/purchase?targetRegion={1}&targetDevice=android"
	};

	// Token: 0x040020B9 RID: 8377
	private static readonly PlatformDependentValue<string> GLUE_STORE_PAYMENT_INFO_DETAILS = new PlatformDependentValue<string>(PlatformCategory.OS)
	{
		PC = "GLUE_STORE_PAYMENT_INFO_DETAILS",
		iOS = "GLUE_MOBILE_STORE_PAYMENT_INFO_DETAILS_APPLE",
		Android = "GLUE_MOBILE_STORE_PAYMENT_INFO_DETAILS_ANDROID"
	};

	// Token: 0x040020BA RID: 8378
	private static readonly PlatformDependentValue<string> GLUE_STORE_PAYMENT_INFO_URL_DETAILS = new PlatformDependentValue<string>(PlatformCategory.OS)
	{
		PC = "GLUE_STORE_PAYMENT_INFO_URL_DETAILS",
		iOS = "GLUE_MOBILE_STORE_PAYMENT_INFO_URL_DETAILS",
		Android = "GLUE_MOBILE_STORE_PAYMENT_INFO_URL_DETAILS"
	};

	// Token: 0x040020BB RID: 8379
	private static readonly Vector3 SHOW_MINI_SUMMARY_SCALE_PHONE = new Vector3(80f, 80f, 80f);

	// Token: 0x040020BC RID: 8380
	private Vector3 m_originalShowScale = Vector3.zero;

	// Token: 0x040020BD RID: 8381
	private List<StoreSendToBAM.DelOKListener> m_okayListeners = new List<StoreSendToBAM.DelOKListener>();

	// Token: 0x040020BE RID: 8382
	private List<StoreSendToBAM.DelCancelListener> m_cancelListeners = new List<StoreSendToBAM.DelCancelListener>();

	// Token: 0x040020BF RID: 8383
	private StoreSendToBAM.BAMReason m_sendToBAMReason;

	// Token: 0x040020C0 RID: 8384
	private MoneyOrGTAPPTransaction m_moneyOrGTAPPTransaction;

	// Token: 0x040020C1 RID: 8385
	private string m_errorCode = string.Empty;

	// Token: 0x040020C2 RID: 8386
	private static Map<StoreSendToBAM.BAMReason, StoreSendToBAM.SendToBAMText> s_bamTextMap;

	// Token: 0x02000416 RID: 1046
	public enum BAMReason
	{
		// Token: 0x0400217B RID: 8571
		PAYMENT_INFO,
		// Token: 0x0400217C RID: 8572
		NEED_PASSWORD_RESET,
		// Token: 0x0400217D RID: 8573
		NO_VALID_PAYMENT_METHOD,
		// Token: 0x0400217E RID: 8574
		CREDIT_CARD_EXPIRED,
		// Token: 0x0400217F RID: 8575
		GENERIC_PAYMENT_FAIL,
		// Token: 0x04002180 RID: 8576
		EULA_AND_TOS,
		// Token: 0x04002181 RID: 8577
		PRODUCT_UNIQUENESS_VIOLATED,
		// Token: 0x04002182 RID: 8578
		GENERIC_PURCHASE_FAIL_RETRY_CONTACT_CS_IF_PERSISTS
	}

	// Token: 0x02000425 RID: 1061
	// (Invoke) Token: 0x060035EF RID: 13807
	public delegate void DelOKListener(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, StoreSendToBAM.BAMReason reason);

	// Token: 0x02000426 RID: 1062
	// (Invoke) Token: 0x060035F3 RID: 13811
	public delegate void DelCancelListener(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction);

	// Token: 0x02000AFC RID: 2812
	private class SendToBAMText
	{
		// Token: 0x06006080 RID: 24704 RVA: 0x001CE710 File Offset: 0x001CC910
		public SendToBAMText(string headlineKey, string detailsKey, string goToURLKey, string urlFmt, StoreURL.Param urlParam1, StoreURL.Param urlParam2)
		{
			this.m_headlineKey = headlineKey;
			this.m_detailsKey = detailsKey;
			this.m_goToURLKey = goToURLKey;
			this.m_url = new StoreURL(urlFmt, urlParam1, urlParam2);
		}

		// Token: 0x06006081 RID: 24705 RVA: 0x001CE749 File Offset: 0x001CC949
		public string GetHeadline()
		{
			return GameStrings.Get(this.m_headlineKey);
		}

		// Token: 0x06006082 RID: 24706 RVA: 0x001CE756 File Offset: 0x001CC956
		public string GetDetails()
		{
			return GameStrings.Get(this.m_detailsKey);
		}

		// Token: 0x06006083 RID: 24707 RVA: 0x001CE763 File Offset: 0x001CC963
		public string GetGoToURLDetails(string buttonName)
		{
			return GameStrings.Format(this.m_goToURLKey, new object[]
			{
				buttonName
			});
		}

		// Token: 0x06006084 RID: 24708 RVA: 0x001CE77A File Offset: 0x001CC97A
		public string GetURL()
		{
			return this.m_url.GetURL();
		}

		// Token: 0x0400480D RID: 18445
		private string m_headlineKey;

		// Token: 0x0400480E RID: 18446
		private string m_detailsKey;

		// Token: 0x0400480F RID: 18447
		private string m_goToURLKey;

		// Token: 0x04004810 RID: 18448
		private StoreURL m_url;
	}
}
