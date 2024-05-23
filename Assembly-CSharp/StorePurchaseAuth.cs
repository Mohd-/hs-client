using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000406 RID: 1030
[CustomEditClass]
public class StorePurchaseAuth : UIBPopup
{
	// Token: 0x06003453 RID: 13395 RVA: 0x00104B7C File Offset: 0x00102D7C
	private void Awake()
	{
		this.m_miniSummary.gameObject.SetActive(false);
		this.m_okButton.gameObject.SetActive(false);
		this.m_okButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnOkayButtonPressed));
	}

	// Token: 0x06003454 RID: 13396 RVA: 0x00104BC4 File Offset: 0x00102DC4
	public void Show(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, bool enableBackButton, bool isZeroCostLicense)
	{
		if (this.m_shown)
		{
			return;
		}
		if (isZeroCostLicense)
		{
			this.m_waitingForAuthText.Text = GameStrings.Get("GLUE_STORE_AUTH_ZERO_COST_WAITING");
			this.m_successHeadlineText.Text = GameStrings.Get("GLUE_STORE_AUTH_ZERO_COST_SUCCESS_HEADLINE");
			this.m_failHeadlineText.Text = GameStrings.Get("GLUE_STORE_AUTH_ZERO_COST_FAIL_HEADLINE");
		}
		else
		{
			this.m_waitingForAuthText.Text = GameStrings.Get("GLUE_STORE_AUTH_WAITING");
			this.m_successHeadlineText.Text = GameStrings.Get("GLUE_STORE_AUTH_SUCCESS_HEADLINE");
			this.m_failHeadlineText.Text = GameStrings.Get("GLUE_STORE_AUTH_FAIL_HEADLINE");
		}
		this.m_shown = true;
		this.m_showingSuccess = false;
		this.m_moneyOrGTAPPTransaction = moneyOrGTAPPTransaction;
		this.m_isBackButton = enableBackButton;
		this.m_okButton.gameObject.SetActive(enableBackButton);
		this.m_okButton.SetText((!enableBackButton) ? "GLOBAL_OKAY" : "GLOBAL_BACK");
		this.m_waitingForAuthText.gameObject.SetActive(true);
		this.m_successHeadlineText.gameObject.SetActive(false);
		this.m_failHeadlineText.gameObject.SetActive(false);
		this.m_failDetailsText.gameObject.SetActive(false);
		this.m_spell.ActivateState(SpellStateType.BIRTH);
		if (this.m_moneyOrGTAPPTransaction != null && this.m_moneyOrGTAPPTransaction.ShouldShowMiniSummary())
		{
			this.ShowMiniSummary();
		}
		else
		{
			this.m_root.UpdateSlices();
		}
		base.DoShowAnimation(null);
	}

	// Token: 0x06003455 RID: 13397 RVA: 0x00104D3C File Offset: 0x00102F3C
	public void ShowPurchaseLocked(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, bool enableBackButton, bool isZeroCostLicense, StorePurchaseAuth.PurchaseLockedDialogCallback purchaseLockedCallback)
	{
		this.Show(moneyOrGTAPPTransaction, enableBackButton, isZeroCostLicense);
		string text = string.Empty;
		if (moneyOrGTAPPTransaction.Provider != null)
		{
			switch (moneyOrGTAPPTransaction.Provider.Value)
			{
			case 2:
				text = GameStrings.Get("GLOBAL_STORE_MOBILE_NAME_APPLE");
				break;
			case 3:
				text = GameStrings.Get("GLOBAL_STORE_MOBILE_NAME_GOOGLE");
				break;
			case 4:
				text = GameStrings.Get("GLOBAL_STORE_MOBILE_NAME_AMAZON");
				break;
			default:
				text = GameStrings.Get("GLOBAL_STORE_MOBILE_NAME_DEFAULT");
				break;
			}
		}
		string text2 = GameStrings.Format("GLUE_STORE_PURCHASE_LOCK_DESCRIPTION", new object[]
		{
			text
		});
		DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo
		{
			m_headerText = GameStrings.Get("GLUE_STORE_PURCHASE_LOCK_HEADER"),
			m_confirmText = GameStrings.Get("GLOBAL_CANCEL"),
			m_cancelText = GameStrings.Get("GLOBAL_HELP"),
			m_text = text2,
			m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
			m_iconSet = AlertPopup.PopupInfo.IconSet.Alternate,
			m_responseCallback = delegate(AlertPopup.Response response, object data)
			{
				if (purchaseLockedCallback != null)
				{
					purchaseLockedCallback(response == AlertPopup.Response.CANCEL);
				}
			}
		});
	}

	// Token: 0x06003456 RID: 13398 RVA: 0x00104E6A File Offset: 0x0010306A
	public override void Hide()
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		base.DoHideAnimation(delegate()
		{
			this.m_okButton.gameObject.SetActive(false);
			this.m_miniSummary.gameObject.SetActive(false);
			this.m_spell.ActivateState(SpellStateType.NONE);
		});
	}

	// Token: 0x06003457 RID: 13399 RVA: 0x00104E94 File Offset: 0x00103094
	public bool CompletePurchaseSuccess(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return false;
		}
		bool showMiniSummary = false;
		if (moneyOrGTAPPTransaction != null)
		{
			showMiniSummary = moneyOrGTAPPTransaction.ShouldShowMiniSummary();
		}
		this.ShowPurchaseSuccess(moneyOrGTAPPTransaction, showMiniSummary);
		return true;
	}

	// Token: 0x06003458 RID: 13400 RVA: 0x00104ECC File Offset: 0x001030CC
	public bool CompletePurchaseFailure(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, string failDetails)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return false;
		}
		bool showMiniSummary = false;
		if (moneyOrGTAPPTransaction != null)
		{
			showMiniSummary = moneyOrGTAPPTransaction.ShouldShowMiniSummary();
		}
		this.ShowPurchaseFailure(moneyOrGTAPPTransaction, failDetails, showMiniSummary);
		return true;
	}

	// Token: 0x06003459 RID: 13401 RVA: 0x00104F04 File Offset: 0x00103104
	public void ShowPreviousPurchaseSuccess(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, bool enableBackButton)
	{
		this.Show(moneyOrGTAPPTransaction, enableBackButton, false);
		this.ShowPurchaseSuccess(moneyOrGTAPPTransaction, true);
	}

	// Token: 0x0600345A RID: 13402 RVA: 0x00104F17 File Offset: 0x00103117
	public void ShowPreviousPurchaseFailure(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, string failDetails, bool enableBackButton)
	{
		this.Show(moneyOrGTAPPTransaction, enableBackButton, false);
		this.ShowPurchaseFailure(moneyOrGTAPPTransaction, failDetails, true);
	}

	// Token: 0x0600345B RID: 13403 RVA: 0x00104F2B File Offset: 0x0010312B
	public void ShowPurchaseMethodFailure(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, string failDetails, bool enableBackButton)
	{
		this.Show(moneyOrGTAPPTransaction, enableBackButton, false);
		this.ShowPurchaseFailure(moneyOrGTAPPTransaction, failDetails, false);
	}

	// Token: 0x0600345C RID: 13404 RVA: 0x00104F3F File Offset: 0x0010313F
	public void RegisterAckPurchaseResultListener(StorePurchaseAuth.AckPurchaseResultListener listener)
	{
		if (this.m_ackPurchaseResultListeners.Contains(listener))
		{
			return;
		}
		this.m_ackPurchaseResultListeners.Add(listener);
	}

	// Token: 0x0600345D RID: 13405 RVA: 0x00104F5F File Offset: 0x0010315F
	public void RemoveAckPurchaseResultListener(StorePurchaseAuth.AckPurchaseResultListener listener)
	{
		this.m_ackPurchaseResultListeners.Remove(listener);
	}

	// Token: 0x0600345E RID: 13406 RVA: 0x00104F6E File Offset: 0x0010316E
	public void RegisterExitListener(StorePurchaseAuth.ExitListener listener)
	{
		if (this.m_exitListeners.Contains(listener))
		{
			return;
		}
		this.m_exitListeners.Add(listener);
	}

	// Token: 0x0600345F RID: 13407 RVA: 0x00104F8E File Offset: 0x0010318E
	public void RemoveExitListener(StorePurchaseAuth.ExitListener listener)
	{
		this.m_exitListeners.Remove(listener);
	}

	// Token: 0x06003460 RID: 13408 RVA: 0x00104FA0 File Offset: 0x001031A0
	private void OnOkayButtonPressed(UIEvent e)
	{
		if (this.m_showingSuccess)
		{
			string text = null;
			Network.Bundle bundle = (this.m_moneyOrGTAPPTransaction != null) ? StoreManager.Get().GetBundle(this.m_moneyOrGTAPPTransaction.ProductID) : null;
			if (bundle != null && bundle.Items != null)
			{
				Network.BundleItem bundleItem = Enumerable.FirstOrDefault<Network.BundleItem>(bundle.Items, (Network.BundleItem i) => i.Product == 6);
				if (bundleItem != null)
				{
					string boughtHeroCardId = GameUtils.TranslateDbIdToCardId(bundleItem.ProductData);
					HeroDbfRecord record = GameDbf.Hero.GetRecord((HeroDbfRecord dbf) => dbf.CardId == boughtHeroCardId);
					if (record != null)
					{
						text = record.PurchaseCompleteMsg;
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.Hide();
				AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
				popupInfo.m_headerText = GameStrings.Get("GLUE_STORE_AUTH_SUCCESS_HEADLINE");
				popupInfo.m_text = text;
				popupInfo.m_showAlertIcon = false;
				popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
				popupInfo.m_responseCallback = delegate(AlertPopup.Response response, object data)
				{
					this.OnOkayButtonPressed_Finish();
				};
				DialogManager.Get().ShowPopup(popupInfo);
				return;
			}
		}
		this.OnOkayButtonPressed_Finish();
	}

	// Token: 0x06003461 RID: 13409 RVA: 0x001050CC File Offset: 0x001032CC
	private void OnOkayButtonPressed_Finish()
	{
		if (this.m_isBackButton)
		{
			StorePurchaseAuth.ExitListener[] array = this.m_exitListeners.ToArray();
			foreach (StorePurchaseAuth.ExitListener exitListener in array)
			{
				exitListener();
			}
		}
		else
		{
			this.Hide();
			StorePurchaseAuth.AckPurchaseResultListener[] array3 = this.m_ackPurchaseResultListeners.ToArray();
			foreach (StorePurchaseAuth.AckPurchaseResultListener ackPurchaseResultListener in array3)
			{
				ackPurchaseResultListener(this.m_showingSuccess, this.m_moneyOrGTAPPTransaction);
			}
		}
		Scene scene = SceneMgr.Get().GetScene();
		if (scene is Login || scene is Hub)
		{
			FixedRewardsMgr fixedRewardsMgr = FixedRewardsMgr.Get();
			UserAttentionBlocker blocker = UserAttentionBlocker.NONE;
			HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
			hashSet.Add(RewardVisualTiming.IMMEDIATE);
			fixedRewardsMgr.ShowFixedRewards(blocker, hashSet, null, delegate(Reward reward)
			{
				reward.transform.localPosition = Login.REWARD_LOCAL_POS;
			}, Login.REWARD_PUNCH_SCALE, Login.REWARD_SCALE);
		}
		else if (scene is AdventureScene)
		{
			FixedRewardsMgr fixedRewardsMgr2 = FixedRewardsMgr.Get();
			UserAttentionBlocker blocker2 = UserAttentionBlocker.NONE;
			HashSet<RewardVisualTiming> hashSet = new HashSet<RewardVisualTiming>();
			hashSet.Add(RewardVisualTiming.IMMEDIATE);
			fixedRewardsMgr2.ShowFixedRewards(blocker2, hashSet, null, delegate(Reward reward)
			{
				reward.transform.localPosition = AdventureScene.REWARD_LOCAL_POS;
			}, AdventureScene.REWARD_PUNCH_SCALE, AdventureScene.REWARD_SCALE);
		}
	}

	// Token: 0x06003462 RID: 13410 RVA: 0x00105234 File Offset: 0x00103434
	private void ShowPurchaseSuccess(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, bool showMiniSummary)
	{
		this.m_showingSuccess = true;
		this.m_moneyOrGTAPPTransaction = moneyOrGTAPPTransaction;
		this.m_isBackButton = false;
		this.m_okButton.gameObject.SetActive(true);
		this.m_okButton.SetText("GLOBAL_OKAY");
		if (showMiniSummary)
		{
			this.ShowMiniSummary();
		}
		this.m_waitingForAuthText.gameObject.SetActive(false);
		this.m_successHeadlineText.gameObject.SetActive(true);
		this.m_failHeadlineText.gameObject.SetActive(false);
		this.m_failDetailsText.gameObject.SetActive(false);
		this.m_spell.ActivateState(SpellStateType.ACTION);
	}

	// Token: 0x06003463 RID: 13411 RVA: 0x001052D4 File Offset: 0x001034D4
	private void ShowPurchaseFailure(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, string failDetails, bool showMiniSummary)
	{
		this.m_showingSuccess = false;
		this.m_moneyOrGTAPPTransaction = moneyOrGTAPPTransaction;
		this.m_isBackButton = false;
		this.m_okButton.gameObject.SetActive(true);
		this.m_okButton.SetText("GLOBAL_OKAY");
		if (showMiniSummary)
		{
			this.ShowMiniSummary();
		}
		this.m_failDetailsText.Text = failDetails;
		this.m_waitingForAuthText.gameObject.SetActive(false);
		this.m_successHeadlineText.gameObject.SetActive(false);
		this.m_failHeadlineText.gameObject.SetActive(true);
		this.m_failDetailsText.gameObject.SetActive(true);
		this.m_spell.ActivateState(SpellStateType.DEATH);
	}

	// Token: 0x06003464 RID: 13412 RVA: 0x00105380 File Offset: 0x00103580
	private void ShowMiniSummary()
	{
		if (this.m_moneyOrGTAPPTransaction == null)
		{
			return;
		}
		this.m_miniSummary.SetDetails(this.m_moneyOrGTAPPTransaction.ProductID, 1);
		this.m_miniSummary.gameObject.SetActive(true);
		this.m_root.UpdateSlices();
	}

	// Token: 0x04002072 RID: 8306
	private const string s_OkButtonText = "GLOBAL_OKAY";

	// Token: 0x04002073 RID: 8307
	private const string s_BackButtonText = "GLOBAL_BACK";

	// Token: 0x04002074 RID: 8308
	[CustomEditField(Sections = "Base UI")]
	public MultiSliceElement m_root;

	// Token: 0x04002075 RID: 8309
	[CustomEditField(Sections = "Swirly Animation")]
	public Spell m_spell;

	// Token: 0x04002076 RID: 8310
	[CustomEditField(Sections = "Base UI")]
	public UIBButton m_okButton;

	// Token: 0x04002077 RID: 8311
	[CustomEditField(Sections = "Text")]
	public UberText m_waitingForAuthText;

	// Token: 0x04002078 RID: 8312
	[CustomEditField(Sections = "Text")]
	public UberText m_successHeadlineText;

	// Token: 0x04002079 RID: 8313
	[CustomEditField(Sections = "Text")]
	public UberText m_failHeadlineText;

	// Token: 0x0400207A RID: 8314
	[CustomEditField(Sections = "Text")]
	public UberText m_failDetailsText;

	// Token: 0x0400207B RID: 8315
	[CustomEditField(Sections = "Base UI")]
	public StoreMiniSummary m_miniSummary;

	// Token: 0x0400207C RID: 8316
	private bool m_isBackButton;

	// Token: 0x0400207D RID: 8317
	private bool m_showingSuccess;

	// Token: 0x0400207E RID: 8318
	private MoneyOrGTAPPTransaction m_moneyOrGTAPPTransaction;

	// Token: 0x0400207F RID: 8319
	private List<StorePurchaseAuth.AckPurchaseResultListener> m_ackPurchaseResultListeners = new List<StorePurchaseAuth.AckPurchaseResultListener>();

	// Token: 0x04002080 RID: 8320
	private List<StorePurchaseAuth.ExitListener> m_exitListeners = new List<StorePurchaseAuth.ExitListener>();

	// Token: 0x02000415 RID: 1045
	// (Invoke) Token: 0x0600359E RID: 13726
	public delegate void PurchaseLockedDialogCallback(bool showHelp);

	// Token: 0x0200041F RID: 1055
	// (Invoke) Token: 0x060035D7 RID: 13783
	public delegate void AckPurchaseResultListener(bool success, MoneyOrGTAPPTransaction moneyOrGTAPPTransaction);

	// Token: 0x02000420 RID: 1056
	// (Invoke) Token: 0x060035DB RID: 13787
	public delegate void ExitListener();
}
