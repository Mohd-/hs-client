using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000419 RID: 1049
public class ArenaStore : Store
{
	// Token: 0x060035AF RID: 13743 RVA: 0x0010ACA9 File Offset: 0x00108EA9
	protected override void Start()
	{
		base.Start();
		this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackPressed));
	}

	// Token: 0x060035B0 RID: 13744 RVA: 0x0010ACCC File Offset: 0x00108ECC
	protected override void Awake()
	{
		ArenaStore.s_instance = this;
		this.m_destroyOnSceneLoad = false;
		base.Awake();
		this.m_backButton.SetText(GameStrings.Get("GLOBAL_BACK"));
	}

	// Token: 0x060035B1 RID: 13745 RVA: 0x0010AD01 File Offset: 0x00108F01
	protected override void OnDestroy()
	{
		ArenaStore.s_instance = null;
	}

	// Token: 0x060035B2 RID: 13746 RVA: 0x0010AD09 File Offset: 0x00108F09
	public static ArenaStore Get()
	{
		return ArenaStore.s_instance;
	}

	// Token: 0x060035B3 RID: 13747 RVA: 0x0010AD10 File Offset: 0x00108F10
	public override void Hide()
	{
		StoreManager.Get().RemoveAuthorizationExitListener(new StoreManager.AuthorizationExitCallback(this.OnAuthExit));
		base.EnableFullScreenEffects(false);
		if (UniversalInputManager.UsePhoneUI)
		{
			BnetBar.Get().SetCurrencyType(default(CurrencyFrame.CurrencyType?));
		}
		base.Hide();
	}

	// Token: 0x060035B4 RID: 13748 RVA: 0x0010AD63 File Offset: 0x00108F63
	public override void OnMoneySpent()
	{
		this.UpdateMoneyButtonState();
	}

	// Token: 0x060035B5 RID: 13749 RVA: 0x0010AD6B File Offset: 0x00108F6B
	public override void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance)
	{
		this.UpdateGoldButtonState(balance);
	}

	// Token: 0x060035B6 RID: 13750 RVA: 0x0010AD74 File Offset: 0x00108F74
	protected override void ShowImpl(Store.DelOnStoreShown onStoreShownCB, bool isTotallyFake)
	{
		this.m_shown = true;
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
		StoreManager.Get().RegisterAuthorizationExitListener(new StoreManager.AuthorizationExitCallback(this.OnAuthExit));
		base.EnableFullScreenEffects(true);
		this.SetUpBuyButtons();
		base.DoShowAnimation(delegate()
		{
			if (isTotallyFake)
			{
				this.m_buyWithGoldButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBuyWithGoldPressed));
				this.m_buyWithMoneyButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBuyWithMoneyPressed));
				this.m_infoButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInfoPressed));
			}
			if (onStoreShownCB != null)
			{
				onStoreShownCB();
			}
		});
		if (UniversalInputManager.UsePhoneUI)
		{
			BnetBar.Get().SetCurrencyType(new CurrencyFrame.CurrencyType?(CurrencyFrame.CurrencyType.GOLD));
		}
	}

	// Token: 0x060035B7 RID: 13751 RVA: 0x0010AE09 File Offset: 0x00109009
	protected override void BuyWithGold(UIEvent e)
	{
		if (this.m_goldTransactionData == null)
		{
			return;
		}
		base.FireBuyWithGoldEventNoGTAPP(this.m_goldTransactionData);
	}

	// Token: 0x060035B8 RID: 13752 RVA: 0x0010AE23 File Offset: 0x00109023
	protected override void BuyWithMoney(UIEvent e)
	{
		if (this.m_bundle == null)
		{
			return;
		}
		base.FireBuyWithMoneyEvent(this.m_bundle.ProductID, 1);
	}

	// Token: 0x060035B9 RID: 13753 RVA: 0x0010AE43 File Offset: 0x00109043
	private void OnAuthExit(object userData)
	{
		Navigation.Pop();
		this.ExitForgeStore(true);
	}

	// Token: 0x060035BA RID: 13754 RVA: 0x0010AE51 File Offset: 0x00109051
	private void OnBackPressed(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x060035BB RID: 13755 RVA: 0x0010AE59 File Offset: 0x00109059
	private bool OnNavigateBack()
	{
		this.ExitForgeStore(false);
		return true;
	}

	// Token: 0x060035BC RID: 13756 RVA: 0x0010AE64 File Offset: 0x00109064
	private void ExitForgeStore(bool authorizationBackButtonPressed)
	{
		base.ActivateCover(false);
		SceneUtils.SetLayer(base.gameObject, GameLayer.Default);
		base.EnableFullScreenEffects(false);
		StoreManager.Get().RemoveAuthorizationExitListener(new StoreManager.AuthorizationExitCallback(this.OnAuthExit));
		base.FireExitEvent(authorizationBackButtonPressed);
	}

	// Token: 0x060035BD RID: 13757 RVA: 0x0010AEAC File Offset: 0x001090AC
	private void UpdateMoneyButtonState()
	{
		Store.BuyButtonState moneyButtonState = Store.BuyButtonState.ENABLED;
		if (this.m_bundle == null || !StoreManager.Get().IsOpen())
		{
			moneyButtonState = Store.BuyButtonState.DISABLED;
			this.m_storeClosed.SetActive(true);
		}
		else if (!StoreManager.Get().IsBattlePayFeatureEnabled())
		{
			moneyButtonState = Store.BuyButtonState.DISABLED_FEATURE;
		}
		else
		{
			this.m_storeClosed.SetActive(false);
		}
		base.SetMoneyButtonState(moneyButtonState);
	}

	// Token: 0x060035BE RID: 13758 RVA: 0x0010AF14 File Offset: 0x00109114
	private void UpdateGoldButtonState(NetCache.NetCacheGoldBalance balance)
	{
		Store.BuyButtonState goldButtonState = Store.BuyButtonState.ENABLED;
		long num;
		if (this.m_goldTransactionData == null)
		{
			goldButtonState = Store.BuyButtonState.DISABLED;
		}
		else if (!StoreManager.Get().IsOpen())
		{
			goldButtonState = Store.BuyButtonState.DISABLED;
		}
		else if (!StoreManager.Get().IsBuyWithGoldFeatureEnabled())
		{
			goldButtonState = Store.BuyButtonState.DISABLED_FEATURE;
		}
		else if (balance == null)
		{
			goldButtonState = Store.BuyButtonState.DISABLED;
		}
		else if (!StoreManager.Get().GetGoldCostNoGTAPP(this.m_goldTransactionData, out num))
		{
			goldButtonState = Store.BuyButtonState.DISABLED_NO_TOOLTIP;
		}
		else if (balance.GetTotal() < num)
		{
			goldButtonState = Store.BuyButtonState.DISABLED_NOT_ENOUGH_GOLD;
		}
		base.SetGoldButtonState(goldButtonState);
	}

	// Token: 0x060035BF RID: 13759 RVA: 0x0010AFA1 File Offset: 0x001091A1
	private void SetUpBuyButtons()
	{
		this.SetUpBuyWithGoldButton();
		this.SetUpBuyWithMoneyButton();
	}

	// Token: 0x060035C0 RID: 13760 RVA: 0x0010AFB0 File Offset: 0x001091B0
	private void SetUpBuyWithGoldButton()
	{
		string text = string.Empty;
		NoGTAPPTransactionData noGTAPPTransactionData = new NoGTAPPTransactionData
		{
			Product = 2,
			ProductData = 0,
			Quantity = 1
		};
		long num;
		if (StoreManager.Get().GetGoldCostNoGTAPP(noGTAPPTransactionData, out num))
		{
			this.m_goldTransactionData = noGTAPPTransactionData;
			text = num.ToString();
			NetCache.NetCacheGoldBalance netObject = NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>();
			this.UpdateGoldButtonState(netObject);
		}
		else
		{
			Debug.LogWarning("ForgeStore.SetUpBuyWithGoldButton(): no gold price for purchase Arena without GTAPP");
			text = GameStrings.Get("GLUE_STORE_PRODUCT_PRICE_NA");
			base.SetGoldButtonState(Store.BuyButtonState.DISABLED);
		}
		this.m_buyWithGoldButton.SetText(text);
	}

	// Token: 0x060035C1 RID: 13761 RVA: 0x0010B048 File Offset: 0x00109248
	private void SetUpBuyWithMoneyButton()
	{
		bool flag = false;
		List<Network.Bundle> availableBundlesForProduct = StoreManager.Get().GetAvailableBundlesForProduct(2, true, false, out flag, 0, ArenaStore.NUM_BUNDLE_ITEMS_REQUIRED);
		string text = string.Empty;
		if (availableBundlesForProduct.Count == 1)
		{
			this.m_bundle = availableBundlesForProduct[0];
			text = StoreManager.Get().FormatCostBundle(this.m_bundle);
			this.UpdateMoneyButtonState();
		}
		else
		{
			Debug.LogWarning(string.Format("ForgeStore.SetUpBuyWithMoneyButton(): expecting 1 bundle for purchasing Forge, found {0}", availableBundlesForProduct.Count));
			text = GameStrings.Get("GLUE_STORE_PRODUCT_PRICE_NA");
			base.SetMoneyButtonState(Store.BuyButtonState.DISABLED);
		}
		this.m_buyWithMoneyButton.SetText(text);
	}

	// Token: 0x0400218A RID: 8586
	public UIBButton m_backButton;

	// Token: 0x0400218B RID: 8587
	public GameObject m_storeClosed;

	// Token: 0x0400218C RID: 8588
	private static readonly int NUM_BUNDLE_ITEMS_REQUIRED = 1;

	// Token: 0x0400218D RID: 8589
	private NoGTAPPTransactionData m_goldTransactionData;

	// Token: 0x0400218E RID: 8590
	private Network.Bundle m_bundle;

	// Token: 0x0400218F RID: 8591
	private static ArenaStore s_instance;
}
