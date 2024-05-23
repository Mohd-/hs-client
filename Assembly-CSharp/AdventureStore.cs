using System;
using System.Collections.Generic;
using PegasusUtil;
using UnityEngine;

// Token: 0x02000413 RID: 1043
[CustomEditClass]
public class AdventureStore : Store
{
	// Token: 0x06003586 RID: 13702 RVA: 0x0010A574 File Offset: 0x00108774
	protected override void Start()
	{
		base.Start();
		this.m_BuyDungeonButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBuyDungeonPressed));
		this.m_offClicker.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.Close();
		});
	}

	// Token: 0x06003587 RID: 13703 RVA: 0x0010A5BC File Offset: 0x001087BC
	public void SetAdventureProduct(ProductType product, int productData)
	{
		List<Network.BundleItem> list = new List<Network.BundleItem>();
		list.Add(new Network.BundleItem
		{
			Product = product,
			ProductData = productData,
			Quantity = 1
		});
		string productName = StoreManager.Get().GetProductName(list);
		this.m_Headline.Text = productName;
		string text = string.Empty;
		switch (product)
		{
		case 3:
			text = "NAXX";
			break;
		case 4:
			text = "BRM";
			break;
		case 7:
			text = "LOE";
			break;
		}
		string key = string.Format("GLUE_STORE_PRODUCT_DETAILS_{0}_PART_1", text);
		string key2 = string.Format("GLUE_STORE_PRODUCT_DETAILS_{0}_PART_2", text);
		this.m_DetailsText1.Text = GameStrings.Format(key, new object[]
		{
			productName
		});
		this.m_DetailsText2.Text = GameStrings.Format(key2, new object[0]);
		bool flag = false;
		List<Network.Bundle> availableBundlesForProduct = StoreManager.Get().GetAvailableBundlesForProduct(product, false, false, out flag, productData, AdventureStore.NUM_BUNDLE_ITEMS_REQUIRED);
		if (availableBundlesForProduct.Count == 1)
		{
			this.m_bundle = availableBundlesForProduct[0];
		}
		else
		{
			Debug.LogWarning(string.Format("AdventureStore.SetAdventureProduct(): expected to find 1 available bundle for product {0} productData {1}, found {2}", product, productData, availableBundlesForProduct.Count));
			this.m_bundle = null;
		}
		StoreManager.Get().GetAvailableAdventureBundle(product, GeneralStoreAdventureContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, out this.m_fullAdventureBundle, out this.m_fullAdventureBundleProductExists);
	}

	// Token: 0x06003588 RID: 13704 RVA: 0x0010A72C File Offset: 0x0010892C
	public override void Hide()
	{
		this.m_shown = false;
		StoreManager.Get().RemoveAuthorizationExitListener(new StoreManager.AuthorizationExitCallback(this.OnAuthExit));
		base.EnableFullScreenEffects(false);
		base.DoHideAnimation(null);
		if (UniversalInputManager.UsePhoneUI)
		{
			BnetBar.Get().SetCurrencyType(default(CurrencyFrame.CurrencyType?));
		}
	}

	// Token: 0x06003589 RID: 13705 RVA: 0x0010A787 File Offset: 0x00108987
	public override void OnMoneySpent()
	{
		this.UpdateMoneyButtonState();
	}

	// Token: 0x0600358A RID: 13706 RVA: 0x0010A78F File Offset: 0x0010898F
	public override void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance)
	{
		this.UpdateGoldButtonState(balance);
	}

	// Token: 0x0600358B RID: 13707 RVA: 0x0010A798 File Offset: 0x00108998
	public override void Close()
	{
		Navigation.GoBack();
	}

	// Token: 0x0600358C RID: 13708 RVA: 0x0010A7A0 File Offset: 0x001089A0
	protected override void ShowImpl(Store.DelOnStoreShown onStoreShownCB, bool isTotallyFake)
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_shown = true;
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
		StoreManager.Get().RegisterAuthorizationExitListener(new StoreManager.AuthorizationExitCallback(this.OnAuthExit));
		base.EnableFullScreenEffects(true);
		this.SetUpBuyButtons();
		this.m_animating = true;
		base.DoShowAnimation(delegate()
		{
			this.m_animating = false;
			if (onStoreShownCB == null)
			{
				return;
			}
			onStoreShownCB();
		});
		if (UniversalInputManager.UsePhoneUI)
		{
			BnetBar.Get().SetCurrencyType(new CurrencyFrame.CurrencyType?(CurrencyFrame.CurrencyType.GOLD));
		}
	}

	// Token: 0x0600358D RID: 13709 RVA: 0x0010A841 File Offset: 0x00108A41
	protected override void BuyWithGold(UIEvent e)
	{
		if (this.m_bundle == null || this.m_animating)
		{
			return;
		}
		base.FireBuyWithGoldEventGTAPP(this.m_bundle.ProductID, 1);
	}

	// Token: 0x0600358E RID: 13710 RVA: 0x0010A86C File Offset: 0x00108A6C
	protected override void BuyWithMoney(UIEvent e)
	{
		if (this.m_bundle == null || this.m_animating)
		{
			return;
		}
		base.FireBuyWithMoneyEvent(this.m_bundle.ProductID, 1);
	}

	// Token: 0x0600358F RID: 13711 RVA: 0x0010A898 File Offset: 0x00108A98
	private void OnAuthExit(object userData)
	{
		base.ActivateCover(false);
		SceneUtils.SetLayer(base.gameObject, GameLayer.Default);
		base.EnableFullScreenEffects(false);
		StoreManager.Get().RemoveAuthorizationExitListener(new StoreManager.AuthorizationExitCallback(this.OnAuthExit));
		base.FireExitEvent(true);
	}

	// Token: 0x06003590 RID: 13712 RVA: 0x0010A8E0 File Offset: 0x00108AE0
	private void UpdateMoneyButtonState()
	{
		Store.BuyButtonState moneyButtonState = Store.BuyButtonState.ENABLED;
		bool active = false;
		if (this.m_bundle == null)
		{
			moneyButtonState = Store.BuyButtonState.DISABLED;
		}
		else if (!StoreManager.Get().IsOpen())
		{
			moneyButtonState = Store.BuyButtonState.DISABLED;
		}
		else if (!StoreManager.Get().IsBattlePayFeatureEnabled())
		{
			moneyButtonState = Store.BuyButtonState.DISABLED_FEATURE;
		}
		else if (!StoreManager.Get().IsBundleRealMoneyOptionAvailableNow(this.m_bundle))
		{
			moneyButtonState = Store.BuyButtonState.DISABLED_NO_TOOLTIP;
			active = true;
		}
		this.m_BuyWithMoneyButtonOpaqueCover.SetActive(active);
		base.SetMoneyButtonState(moneyButtonState);
	}

	// Token: 0x06003591 RID: 13713 RVA: 0x0010A95C File Offset: 0x00108B5C
	private void UpdateGoldButtonState(NetCache.NetCacheGoldBalance balance)
	{
		Store.BuyButtonState goldButtonState = Store.BuyButtonState.ENABLED;
		bool active = false;
		if (this.m_bundle == null)
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
		else if (!StoreManager.Get().IsBundleGoldOptionAvailableNow(this.m_bundle))
		{
			goldButtonState = Store.BuyButtonState.DISABLED_NO_TOOLTIP;
			active = true;
		}
		else if (balance == null)
		{
			goldButtonState = Store.BuyButtonState.DISABLED;
		}
		else if (balance.GetTotal() < this.m_bundle.GoldCost.Value)
		{
			goldButtonState = Store.BuyButtonState.DISABLED_NOT_ENOUGH_GOLD;
		}
		this.m_BuyWithGoldButtonOpaqueCover.SetActive(active);
		base.SetGoldButtonState(goldButtonState);
	}

	// Token: 0x06003592 RID: 13714 RVA: 0x0010AA09 File Offset: 0x00108C09
	private void SetUpBuyButtons()
	{
		this.SetUpBuyWithGoldButton();
		this.SetUpBuyWithMoneyButton();
		this.SetUpBuyFullAdventureButton();
	}

	// Token: 0x06003593 RID: 13715 RVA: 0x0010AA20 File Offset: 0x00108C20
	private void SetUpBuyWithGoldButton()
	{
		string text = string.Empty;
		if (this.m_bundle != null)
		{
			text = this.m_bundle.GoldCost.ToString();
			NetCache.NetCacheGoldBalance netObject = NetCache.Get().GetNetObject<NetCache.NetCacheGoldBalance>();
			this.UpdateGoldButtonState(netObject);
		}
		else
		{
			Debug.LogWarning("AdventureStore.SetUpBuyWithGoldButton(): m_bundle is null");
			text = GameStrings.Get("GLUE_STORE_PRODUCT_PRICE_NA");
			base.SetGoldButtonState(Store.BuyButtonState.DISABLED);
		}
		this.m_buyWithGoldButton.SetText(text);
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x0010AA94 File Offset: 0x00108C94
	private void SetUpBuyWithMoneyButton()
	{
		string text = string.Empty;
		if (this.m_bundle != null)
		{
			text = StoreManager.Get().FormatCostBundle(this.m_bundle);
			this.UpdateMoneyButtonState();
		}
		else
		{
			Debug.LogWarning("AdventureStore.SetUpBuyWithMoneyButton(): m_bundle is null");
			text = GameStrings.Get("GLUE_STORE_PRODUCT_PRICE_NA");
			base.SetMoneyButtonState(Store.BuyButtonState.DISABLED);
		}
		this.m_buyWithMoneyButton.SetText(text);
	}

	// Token: 0x06003595 RID: 13717 RVA: 0x0010AAF8 File Offset: 0x00108CF8
	private void SetUpBuyFullAdventureButton()
	{
		string text = string.Empty;
		bool flag = false;
		if (this.m_fullAdventureBundle != null)
		{
			string text2 = StoreManager.Get().FormatCostBundle(this.m_fullAdventureBundle);
			text = GameStrings.Format("GLUE_STORE_DUNGEON_BUTTON_TEXT", new object[]
			{
				this.m_fullAdventureBundle.Items.Count,
				text2
			});
		}
		else if (this.m_fullAdventureBundleProductExists)
		{
			flag = false;
			text = GameStrings.Get("GLUE_STORE_PRODUCT_PRICE_NA");
		}
		else
		{
			flag = true;
			text = string.Empty;
		}
		this.m_BuyDungeonButton.SetText(text);
		this.m_BuyDungeonButtonOpaqueCover.SetActive(flag);
		this.m_BuyDungeonButton.SetEnabled(!flag);
	}

	// Token: 0x06003596 RID: 13718 RVA: 0x0010ABA8 File Offset: 0x00108DA8
	private void OnBuyDungeonPressed(UIEvent e)
	{
		this.Close();
		Options.Get().SetInt(Option.LAST_SELECTED_STORE_ADVENTURE_ID, (int)AdventureConfig.Get().GetSelectedAdventure());
		StoreManager.Get().StartGeneralTransaction(GeneralStoreMode.ADVENTURE);
	}

	// Token: 0x06003597 RID: 13719 RVA: 0x0010ABDC File Offset: 0x00108DDC
	private bool OnNavigateBack()
	{
		this.Hide();
		base.FireExitEvent(false);
		return true;
	}

	// Token: 0x0400216E RID: 8558
	[CustomEditField(Sections = "UI")]
	public UIBButton m_BuyDungeonButton;

	// Token: 0x0400216F RID: 8559
	[CustomEditField(Sections = "UI")]
	public UberText m_Headline;

	// Token: 0x04002170 RID: 8560
	[CustomEditField(Sections = "UI")]
	public UberText m_DetailsText1;

	// Token: 0x04002171 RID: 8561
	[CustomEditField(Sections = "UI")]
	public UberText m_DetailsText2;

	// Token: 0x04002172 RID: 8562
	[CustomEditField(Sections = "UI")]
	public GameObject m_BuyWithMoneyButtonOpaqueCover;

	// Token: 0x04002173 RID: 8563
	[CustomEditField(Sections = "UI")]
	public GameObject m_BuyWithGoldButtonOpaqueCover;

	// Token: 0x04002174 RID: 8564
	[CustomEditField(Sections = "UI")]
	public GameObject m_BuyDungeonButtonOpaqueCover;

	// Token: 0x04002175 RID: 8565
	private static readonly int NUM_BUNDLE_ITEMS_REQUIRED = 1;

	// Token: 0x04002176 RID: 8566
	private bool m_animating;

	// Token: 0x04002177 RID: 8567
	private Network.Bundle m_bundle;

	// Token: 0x04002178 RID: 8568
	private Network.Bundle m_fullAdventureBundle;

	// Token: 0x04002179 RID: 8569
	private bool m_fullAdventureBundleProductExists;
}
