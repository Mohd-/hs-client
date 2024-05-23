using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using PegasusShared;
using PegasusUtil;
using UnityEngine;
using WTCG.BI;

// Token: 0x02000079 RID: 121
public class StoreManager
{
	// Token: 0x060005DC RID: 1500 RVA: 0x00014FA5 File Offset: 0x000131A5
	public static StoreManager Get()
	{
		if (StoreManager.s_instance == null)
		{
			StoreManager.s_instance = new StoreManager();
		}
		return StoreManager.s_instance;
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00014FC0 File Offset: 0x000131C0
	public static void DestroyInstance()
	{
		Store store = StoreManager.s_instance.GetStore(StoreType.GENERAL_STORE);
		if (store != null)
		{
			store.Hide();
			Object.DestroyObject(store);
		}
		if (AchieveManager.Get() != null && StoreManager.s_instance != null)
		{
			AchieveManager.Get().RemoveActiveAchievesUpdatedListener(new AchieveManager.ActiveAchievesUpdatedCallback(StoreManager.s_instance.OnAchievesUpdated));
			AchieveManager.Get().RemoveLicenseAddedAchievesUpdatedListener(new AchieveManager.LicenseAddedAchievesUpdatedCallback(StoreManager.s_instance.OnLicenseAddedAchievesUpdated));
		}
		StoreManager.s_instance = null;
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x060005DE RID: 1502 RVA: 0x00015044 File Offset: 0x00013244
	public static BattlePayProvider StoreProvider
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00015054 File Offset: 0x00013254
	public void Init()
	{
		NetCache.Get().RegisterFeatures(new NetCache.NetCacheCallback(this.OnNetCacheFeaturesReady));
		if (this.m_initComplete)
		{
			return;
		}
		this.m_lastConfigRequestTime = 0L;
		this.m_lastStatusRequestTime = 0L;
		this.m_configRequestDelayTicks = StoreManager.MIN_CONFIG_REQUEST_DELAY_TICKS;
		this.m_statusRequestDelayTicks = StoreManager.MIN_STATUS_REQUEST_DELAY_TICKS;
		SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
		Network network = Network.Get();
		network.RegisterNetHandler(265, new Network.NetHandler(this.OnBattlePayStatusResponse), null);
		network.RegisterNetHandler(238, new Network.NetHandler(this.OnBattlePayConfigResponse), null);
		network.RegisterNetHandler(272, new Network.NetHandler(this.OnPurchaseMethod), null);
		network.RegisterNetHandler(256, new Network.NetHandler(this.OnPurchaseResponse), null);
		network.RegisterNetHandler(275, new Network.NetHandler(this.OnPurchaseCanceledResponse), null);
		network.RegisterNetHandler(280, new Network.NetHandler(this.OnPurchaseViaGoldResponse), null);
		network.RegisterNetHandler(295, new Network.NetHandler(this.OnThirdPartyPurchaseStatusResponse), null);
		NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
		if (netObject != null)
		{
			this.OnNewNotices(netObject.Notices);
		}
		NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
		NetCache.Get().RegisterGoldBalanceListener(new NetCache.DelGoldBalanceListener(this.OnGoldBalanceChanged));
		this.m_initComplete = true;
		AssetLoader.Get().LoadGameObject(this.s_storePrefab, new AssetLoader.GameObjectCallback(this.OnGeneralStoreLoaded), null, false);
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00015224 File Offset: 0x00013424
	private void WillReset()
	{
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		Network network = Network.Get();
		network.RemoveNetHandler(265, new Network.NetHandler(this.OnBattlePayStatusResponse));
		network.RemoveNetHandler(238, new Network.NetHandler(this.OnBattlePayConfigResponse));
		network.RemoveNetHandler(272, new Network.NetHandler(this.OnPurchaseMethod));
		network.RemoveNetHandler(256, new Network.NetHandler(this.OnPurchaseResponse));
		network.RemoveNetHandler(275, new Network.NetHandler(this.OnPurchaseCanceledResponse));
		network.RemoveNetHandler(280, new Network.NetHandler(this.OnPurchaseViaGoldResponse));
		network.RemoveNetHandler(295, new Network.NetHandler(this.OnThirdPartyPurchaseStatusResponse));
		StoreMobilePurchase.Reset();
		StoreManager.DestroyInstance();
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00015324 File Offset: 0x00013524
	public void Heartbeat()
	{
		if (!this.m_initComplete)
		{
			return;
		}
		long ticks = DateTime.Now.Ticks;
		this.RequestStatusIfNeeded(ticks);
		this.RequestConfigIfNeeded(ticks);
		this.AutoCancelPurchaseIfNeeded(ticks);
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00015364 File Offset: 0x00013564
	public bool IsOpen()
	{
		return this.NoticesReady && this.IsStoreFeatureEnabled() && this.BattlePayAvailable && this.ConfigLoaded && this.HaveProductsToSell() && StoreManager.TransactionStatus.UNKNOWN != this.Status;
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x000153C0 File Offset: 0x000135C0
	public bool IsStoreFeatureEnabled()
	{
		NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
		return netCacheFeatures != null && netCacheFeatures.Store.Store;
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x000153E8 File Offset: 0x000135E8
	public bool IsBattlePayFeatureEnabled()
	{
		NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
		return netCacheFeatures != null && netCacheFeatures.Store.Store && netCacheFeatures.Store.BattlePay;
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00015424 File Offset: 0x00013624
	public bool IsBuyWithGoldFeatureEnabled()
	{
		NetCache.NetCacheFeatures netCacheFeatures = this.GetNetCacheFeatures();
		return netCacheFeatures != null && netCacheFeatures.Store.Store && netCacheFeatures.Store.BuyWithGold;
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x0001545E File Offset: 0x0001365E
	public bool RegisterStatusChangedListener(StoreManager.StatusChangedCallback callback)
	{
		return this.RegisterStatusChangedListener(callback, null);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00015468 File Offset: 0x00013668
	public bool RegisterStatusChangedListener(StoreManager.StatusChangedCallback callback, object userData)
	{
		StoreManager.StatusChangedListener statusChangedListener = new StoreManager.StatusChangedListener();
		statusChangedListener.SetCallback(callback);
		statusChangedListener.SetUserData(userData);
		if (this.m_statusChangedListeners.Contains(statusChangedListener))
		{
			return false;
		}
		this.m_statusChangedListeners.Add(statusChangedListener);
		return true;
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x000154A9 File Offset: 0x000136A9
	public bool RemoveStatusChangedListener(StoreManager.StatusChangedCallback callback)
	{
		return this.RemoveStatusChangedListener(callback, null);
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x000154B4 File Offset: 0x000136B4
	public bool RemoveStatusChangedListener(StoreManager.StatusChangedCallback callback, object userData)
	{
		StoreManager.StatusChangedListener statusChangedListener = new StoreManager.StatusChangedListener();
		statusChangedListener.SetCallback(callback);
		statusChangedListener.SetUserData(userData);
		return this.m_statusChangedListeners.Remove(statusChangedListener);
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x000154E1 File Offset: 0x000136E1
	public bool RegisterSuccessfulPurchaseListener(StoreManager.SuccessfulPurchaseCallback callback)
	{
		return this.RegisterSuccessfulPurchaseListener(callback, null);
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x000154EC File Offset: 0x000136EC
	public bool RegisterSuccessfulPurchaseListener(StoreManager.SuccessfulPurchaseCallback callback, object userData)
	{
		StoreManager.SuccessfulPurchaseListener successfulPurchaseListener = new StoreManager.SuccessfulPurchaseListener();
		successfulPurchaseListener.SetCallback(callback);
		successfulPurchaseListener.SetUserData(userData);
		if (this.m_successfulPurchaseListeners.Contains(successfulPurchaseListener))
		{
			return false;
		}
		this.m_successfulPurchaseListeners.Add(successfulPurchaseListener);
		return true;
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0001552D File Offset: 0x0001372D
	public bool RemoveSuccessfulPurchaseListener(StoreManager.SuccessfulPurchaseCallback callback)
	{
		return this.RemoveSuccessfulPurchaseListener(callback, null);
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x00015538 File Offset: 0x00013738
	public bool RemoveSuccessfulPurchaseListener(StoreManager.SuccessfulPurchaseCallback callback, object userData)
	{
		StoreManager.SuccessfulPurchaseListener successfulPurchaseListener = new StoreManager.SuccessfulPurchaseListener();
		successfulPurchaseListener.SetCallback(callback);
		successfulPurchaseListener.SetUserData(userData);
		return this.m_successfulPurchaseListeners.Remove(successfulPurchaseListener);
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00015565 File Offset: 0x00013765
	public bool RegisterSuccessfulPurchaseAckListener(StoreManager.SuccessfulPurchaseAckCallback callback)
	{
		return this.RegisterSuccessfulPurchaseAckListener(callback, null);
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00015570 File Offset: 0x00013770
	public bool RegisterSuccessfulPurchaseAckListener(StoreManager.SuccessfulPurchaseAckCallback callback, object userData)
	{
		StoreManager.SuccessfulPurchaseAckListener successfulPurchaseAckListener = new StoreManager.SuccessfulPurchaseAckListener();
		successfulPurchaseAckListener.SetCallback(callback);
		successfulPurchaseAckListener.SetUserData(userData);
		if (this.m_successfulPurchaseAckListeners.Contains(successfulPurchaseAckListener))
		{
			return false;
		}
		this.m_successfulPurchaseAckListeners.Add(successfulPurchaseAckListener);
		return true;
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x000155B1 File Offset: 0x000137B1
	public bool RemoveSuccessfulPurchaseAckListener(StoreManager.SuccessfulPurchaseAckCallback callback)
	{
		return this.RemoveSuccessfulPurchaseAckListener(callback, null);
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x000155BC File Offset: 0x000137BC
	public bool RemoveSuccessfulPurchaseAckListener(StoreManager.SuccessfulPurchaseAckCallback callback, object userData)
	{
		StoreManager.SuccessfulPurchaseAckListener successfulPurchaseAckListener = new StoreManager.SuccessfulPurchaseAckListener();
		successfulPurchaseAckListener.SetCallback(callback);
		successfulPurchaseAckListener.SetUserData(userData);
		return this.m_successfulPurchaseAckListeners.Remove(successfulPurchaseAckListener);
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x000155E9 File Offset: 0x000137E9
	public bool RegisterAuthorizationExitListener(StoreManager.AuthorizationExitCallback callback)
	{
		return this.RegisterAuthorizationExitListener(callback, null);
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x000155F4 File Offset: 0x000137F4
	public bool RegisterAuthorizationExitListener(StoreManager.AuthorizationExitCallback callback, object userData)
	{
		StoreManager.AuthorizationExitListener authorizationExitListener = new StoreManager.AuthorizationExitListener();
		authorizationExitListener.SetCallback(callback);
		authorizationExitListener.SetUserData(userData);
		if (this.m_authExitListeners.Contains(authorizationExitListener))
		{
			return false;
		}
		this.m_authExitListeners.Add(authorizationExitListener);
		return true;
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x00015635 File Offset: 0x00013835
	public bool RemoveAuthorizationExitListener(StoreManager.AuthorizationExitCallback callback)
	{
		return this.RemoveAuthorizationExitListener(callback, null);
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x00015640 File Offset: 0x00013840
	public bool RemoveAuthorizationExitListener(StoreManager.AuthorizationExitCallback callback, object userData)
	{
		StoreManager.AuthorizationExitListener authorizationExitListener = new StoreManager.AuthorizationExitListener();
		authorizationExitListener.SetCallback(callback);
		authorizationExitListener.SetUserData(userData);
		return this.m_authExitListeners.Remove(authorizationExitListener);
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x0001566D File Offset: 0x0001386D
	public bool RegisterStoreShownListener(StoreManager.StoreShownCallback callback)
	{
		return this.RegisterStoreShownListener(callback, null);
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00015678 File Offset: 0x00013878
	public bool RegisterStoreShownListener(StoreManager.StoreShownCallback callback, object userData)
	{
		StoreManager.StoreShownListener storeShownListener = new StoreManager.StoreShownListener();
		storeShownListener.SetCallback(callback);
		storeShownListener.SetUserData(userData);
		if (this.m_storeShownListeners.Contains(storeShownListener))
		{
			return false;
		}
		this.m_storeShownListeners.Add(storeShownListener);
		return true;
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x000156B9 File Offset: 0x000138B9
	public bool RemoveStoreShownListener(StoreManager.StoreShownCallback callback)
	{
		return this.RemoveStoreShownListener(callback, null);
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x000156C4 File Offset: 0x000138C4
	public bool RemoveStoreShownListener(StoreManager.StoreShownCallback callback, object userData)
	{
		StoreManager.StoreShownListener storeShownListener = new StoreManager.StoreShownListener();
		storeShownListener.SetCallback(callback);
		storeShownListener.SetUserData(userData);
		return this.m_storeShownListeners.Remove(storeShownListener);
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x000156F1 File Offset: 0x000138F1
	public bool RegisterStoreHiddenListener(StoreManager.StoreHiddenCallback callback)
	{
		return this.RegisterStoreHiddenListener(callback, null);
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x000156FC File Offset: 0x000138FC
	public bool RegisterStoreHiddenListener(StoreManager.StoreHiddenCallback callback, object userData)
	{
		StoreManager.StoreHiddenListener storeHiddenListener = new StoreManager.StoreHiddenListener();
		storeHiddenListener.SetCallback(callback);
		storeHiddenListener.SetUserData(userData);
		if (this.m_storeHiddenListeners.Contains(storeHiddenListener))
		{
			return false;
		}
		this.m_storeHiddenListeners.Add(storeHiddenListener);
		return false;
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x0001573D File Offset: 0x0001393D
	public bool RemoveStoreHiddenListener(StoreManager.StoreHiddenCallback callback)
	{
		return this.RemoveStoreHiddenListener(callback, null);
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x00015748 File Offset: 0x00013948
	public bool RemoveStoreHiddenListener(StoreManager.StoreHiddenCallback callback, object userData)
	{
		StoreManager.StoreHiddenListener storeHiddenListener = new StoreManager.StoreHiddenListener();
		storeHiddenListener.SetCallback(callback);
		storeHiddenListener.SetUserData(userData);
		return this.m_storeHiddenListeners.Remove(storeHiddenListener);
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00015775 File Offset: 0x00013975
	public bool RegisterStoreAchievesListener(StoreManager.StoreAchievesCallback callback)
	{
		return this.RegisterStoreAchievesListener(callback, null);
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00015780 File Offset: 0x00013980
	public bool RegisterStoreAchievesListener(StoreManager.StoreAchievesCallback callback, object userData)
	{
		StoreManager.StoreAchievesListener storeAchievesListener = new StoreManager.StoreAchievesListener();
		storeAchievesListener.SetCallback(callback);
		storeAchievesListener.SetUserData(userData);
		if (this.m_storeAchievesListeners.Contains(storeAchievesListener))
		{
			return false;
		}
		this.m_storeAchievesListeners.Add(storeAchievesListener);
		return false;
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x000157C1 File Offset: 0x000139C1
	public bool RemoveStoreAchievesListener(StoreManager.StoreAchievesCallback callback)
	{
		return this.RemoveStoreAchievesListener(callback, null);
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x000157CC File Offset: 0x000139CC
	public bool RemoveStoreAchievesListener(StoreManager.StoreAchievesCallback callback, object userData)
	{
		StoreManager.StoreAchievesListener storeAchievesListener = new StoreManager.StoreAchievesListener();
		storeAchievesListener.SetCallback(callback);
		storeAchievesListener.SetUserData(userData);
		return this.m_storeAchievesListeners.Remove(storeAchievesListener);
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x000157F9 File Offset: 0x000139F9
	public bool IsWaitingToShow()
	{
		return this.m_waitingToShowStore;
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00015801 File Offset: 0x00013A01
	public Store GetCurrentStore()
	{
		return this.GetStore(this.m_currentStoreType);
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x00015810 File Offset: 0x00013A10
	public Store GetStore(StoreType storeType)
	{
		Store result = null;
		this.m_stores.TryGetValue(storeType, out result);
		return result;
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x00015830 File Offset: 0x00013A30
	public bool IsShown()
	{
		Store currentStore = this.GetCurrentStore();
		return currentStore != null && currentStore.IsShown();
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00015859 File Offset: 0x00013A59
	public bool IsShownOrWaitingToShow()
	{
		return this.IsWaitingToShow() || this.IsShown();
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00015878 File Offset: 0x00013A78
	public bool GetGoldCostNoGTAPP(NoGTAPPTransactionData noGTAPPTransactionData, out long cost)
	{
		cost = 0L;
		if (noGTAPPTransactionData == null)
		{
			return false;
		}
		long num = 0L;
		ProductType product = noGTAPPTransactionData.Product;
		if (product != 1)
		{
			if (product != 2)
			{
				Debug.LogWarning(string.Format("StoreManager.GetGoldPriceNoGTAPP(): don't have a no-GTAPP gold price for product {0} data {1}", noGTAPPTransactionData.Product, noGTAPPTransactionData.ProductData));
				return false;
			}
			if (!this.GetArenaGoldCostNoGTAPP(out num))
			{
				return false;
			}
		}
		else if (!this.GetBoosterGoldCostNoGTAPP(noGTAPPTransactionData.ProductData, out num))
		{
			return false;
		}
		cost = num * (long)noGTAPPTransactionData.Quantity;
		return true;
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x0001590F File Offset: 0x00013B0F
	public Network.Bundle GetBundle(string productID)
	{
		if (this.m_bundles.ContainsKey(productID))
		{
			return this.m_bundles[productID];
		}
		Debug.LogWarning(string.Format("StoreManager.GetBundle(): don't have a bundle for productID '{0}'", productID));
		return null;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00015940 File Offset: 0x00013B40
	public HashSet<ProductType> GetProductsInItemList(List<Network.BundleItem> items)
	{
		HashSet<ProductType> hashSet = new HashSet<ProductType>();
		foreach (Network.BundleItem bundleItem in items)
		{
			hashSet.Add(bundleItem.Product);
		}
		return hashSet;
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x000159A4 File Offset: 0x00013BA4
	public HashSet<ProductType> GetProductsInBundle(Network.Bundle bundle)
	{
		if (bundle == null)
		{
			return new HashSet<ProductType>();
		}
		return this.GetProductsInItemList(bundle.Items);
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x000159C0 File Offset: 0x00013BC0
	public bool IsProductAlreadyOwned(Network.Bundle bundle)
	{
		if (bundle == null)
		{
			return false;
		}
		foreach (Network.BundleItem bundleItem in bundle.Items)
		{
			if (this.AlreadyOwnsProduct(bundleItem.Product, bundleItem.ProductData))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x00015A3C File Offset: 0x00013C3C
	public bool IsProductFree(Network.Bundle bundle)
	{
		return false;
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x00015A40 File Offset: 0x00013C40
	public bool IsProductPrePurchase(Network.Bundle bundle)
	{
		if (bundle == null)
		{
			return false;
		}
		HashSet<ProductType> productsInItemList = this.GetProductsInItemList(bundle.Items);
		if (productsInItemList.Contains(4) && productsInItemList.Contains(5))
		{
			return true;
		}
		if (productsInItemList.Contains(1) && productsInItemList.Contains(5))
		{
			Network.BundleItem bundleItem = bundle.Items.Find((Network.BundleItem obj) => obj.Product == 1 && obj.ProductData == 10);
			if (bundleItem != null)
			{
				return true;
			}
			Network.BundleItem bundleItem2 = bundle.Items.Find((Network.BundleItem obj) => obj.Product == 1 && obj.ProductData == 11);
			if (bundleItem2 != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x00015AF8 File Offset: 0x00013CF8
	public List<Network.Bundle> GetAllBundlesForProduct(ProductType product, bool requireRealMoneyOption, int productData = 0, int numItemsRequired = 0)
	{
		List<Network.Bundle> list = new List<Network.Bundle>();
		foreach (Network.Bundle bundle in this.m_bundles.Values)
		{
			if (numItemsRequired == 0 || bundle.Items.Count == numItemsRequired)
			{
				bool flag = false;
				foreach (Network.BundleItem bundleItem in bundle.Items)
				{
					if (bundleItem.Product == product)
					{
						if (productData == 0 || bundleItem.ProductData == productData)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					if (this.IsBundleAvailableNow(bundle, requireRealMoneyOption))
					{
						list.Add(bundle);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x00015C14 File Offset: 0x00013E14
	public Network.Bundle GetLowestCostUnownedBundle(ProductType product, bool requireRealMoneyOption, int productData, int numItemsRequired = 0)
	{
		List<Network.Bundle> allBundlesForProduct = StoreManager.Get().GetAllBundlesForProduct(product, requireRealMoneyOption, productData, numItemsRequired);
		Network.Bundle bundle = null;
		foreach (Network.Bundle bundle2 in allBundlesForProduct)
		{
			if (numItemsRequired == 0 || bundle2.Items.Count == numItemsRequired)
			{
				if (!this.IsProductAlreadyOwned(bundle2))
				{
					if (bundle != null)
					{
						double? cost = bundle.Cost;
						if (cost == null)
						{
							goto IL_9F;
						}
						double? cost2 = bundle2.Cost;
						if (cost2 == null)
						{
							goto IL_9F;
						}
						bool flag = cost.Value <= cost2.Value;
						IL_A0:
						if (flag)
						{
							continue;
						}
						bundle = bundle2;
						continue;
						IL_9F:
						flag = false;
						goto IL_A0;
					}
					bundle = bundle2;
				}
			}
		}
		return bundle;
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00015CFC File Offset: 0x00013EFC
	public List<Network.Bundle> GetAvailableBundlesForProduct(ProductType productType, bool requireRealMoneyOption, bool includeAlreadyOwnedBundles, out bool productTypeExists, int productData = 0, int numItemsRequired = 0)
	{
		productTypeExists = false;
		bool flag = false;
		List<Network.Bundle> list = new List<Network.Bundle>();
		foreach (Network.Bundle bundle in this.m_bundles.Values)
		{
			if (numItemsRequired == 0 || bundle.Items.Count == numItemsRequired)
			{
				if (this.IsBundleAvailableNow(bundle, requireRealMoneyOption))
				{
					bool flag2 = false;
					bool flag3 = false;
					foreach (Network.BundleItem bundleItem in bundle.Items)
					{
						if (bundleItem.Product == productType)
						{
							productTypeExists = true;
						}
						flag3 = this.AlreadyOwnsProduct(bundleItem.Product, bundleItem.ProductData);
						if (flag3)
						{
							flag = true;
							if (!includeAlreadyOwnedBundles)
							{
								break;
							}
						}
						if (bundleItem.Product == productType)
						{
							if (productData == 0 || bundleItem.ProductData == productData)
							{
								flag2 = true;
								break;
							}
						}
					}
					if (flag2)
					{
						if (!flag3 || includeAlreadyOwnedBundles)
						{
							list.Add(bundle);
						}
					}
				}
			}
		}
		if (list.Count == 0 && !flag)
		{
			productTypeExists = false;
		}
		return list;
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00015EA0 File Offset: 0x000140A0
	public bool GetAvailableAdventureBundle(ProductType product, bool requireRealMoneyOption, out Network.Bundle bundle, out bool productExists)
	{
		bundle = null;
		bool flag = false;
		List<Network.Bundle> list = null;
		switch (product)
		{
		case 3:
			list = this.GetAvailableBundlesForProduct(product, requireRealMoneyOption, false, out flag, 5, 0);
			break;
		case 4:
			list = this.GetAvailableBundlesForProduct(product, requireRealMoneyOption, false, out flag, 10, 0);
			break;
		case 7:
			list = this.GetAvailableBundlesForProduct(product, requireRealMoneyOption, false, out flag, 14, 0);
			break;
		}
		productExists = flag;
		if (list != null)
		{
			foreach (Network.Bundle bundle2 in list)
			{
				int count = bundle2.Items.Count;
				if (count != 0)
				{
					if (this.IsBundleAvailableNow(bundle2, requireRealMoneyOption))
					{
						if (bundle == null)
						{
							bundle = bundle2;
						}
						else if (bundle.Items.Count <= count)
						{
							bundle = bundle2;
						}
					}
				}
			}
		}
		return bundle != null;
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x00015FBC File Offset: 0x000141BC
	public bool GetAvailableAdventureBundle(AdventureDbId adventure, bool requireRealMoneyOption, out Network.Bundle bundle, out bool productExists)
	{
		ProductType adventureProductType = StoreManager.GetAdventureProductType(adventure);
		if (adventureProductType == null)
		{
			bundle = null;
			productExists = false;
			return false;
		}
		return this.GetAvailableAdventureBundle(adventureProductType, requireRealMoneyOption, out bundle, out productExists);
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x00015FEC File Offset: 0x000141EC
	public bool GetAvailableAdventureBundle(AdventureDbId adventure, bool requireRealMoneyOption, out Network.Bundle bundle)
	{
		bool flag;
		return this.GetAvailableAdventureBundle(adventure, requireRealMoneyOption, out bundle, out flag);
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x00016004 File Offset: 0x00014204
	public bool CanBuyBoosterWithGold(int boosterDbId)
	{
		BoosterDbfRecord record = GameDbf.Booster.GetRecord(boosterDbId);
		SpecialEventType specialEventType;
		return record != null && !string.IsNullOrEmpty(record.BuyWithGoldEvent) && EnumUtils.TryGetEnum<SpecialEventType>(record.BuyWithGoldEvent, out specialEventType) && (specialEventType == SpecialEventType.IGNORE || SpecialEventManager.Get().IsEventActive(specialEventType, false));
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x00016060 File Offset: 0x00014260
	public bool IsBoosterPreorderActive(int boosterDbId, out Network.Bundle preOrderBundle)
	{
		List<Network.Bundle> allBundlesForProduct = this.GetAllBundlesForProduct(1, GeneralStorePacksContent.REQUIRE_REAL_MONEY_BUNDLE_OPTION, boosterDbId, 0);
		foreach (Network.Bundle bundle in allBundlesForProduct)
		{
			if (this.IsProductPrePurchase(bundle))
			{
				preOrderBundle = bundle;
				return true;
			}
		}
		preOrderBundle = null;
		return false;
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x000160E0 File Offset: 0x000142E0
	public bool GetHeroBundleByCardDbId(int heroCardDbId, out Network.Bundle heroBundle)
	{
		bool flag = false;
		List<Network.Bundle> availableBundlesForProduct = this.GetAvailableBundlesForProduct(6, false, true, out flag, 0, 0);
		heroBundle = null;
		if (!flag)
		{
			return false;
		}
		foreach (Network.Bundle bundle in availableBundlesForProduct)
		{
			foreach (Network.BundleItem bundleItem in bundle.Items)
			{
				if (bundleItem.Product == 6 && bundleItem.ProductData == heroCardDbId)
				{
					heroBundle = bundle;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x000161B8 File Offset: 0x000143B8
	public bool GetHeroBundleByCardMiniGuid(string heroCardId_aka_miniGuid, out Network.Bundle heroBundle)
	{
		return this.GetHeroBundleByCardDbId(GameUtils.TranslateCardIdToDbId(heroCardId_aka_miniGuid), out heroBundle);
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x000161C7 File Offset: 0x000143C7
	public bool IsChinaCustomer()
	{
		return this.m_currency == Currency.CPT;
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x000161D3 File Offset: 0x000143D3
	public bool IsKoreanCustomer()
	{
		return this.m_currency == Currency.KRW;
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x000161DE File Offset: 0x000143DE
	public bool IsEuropeanCustomer()
	{
		return this.m_currency == Currency.GBP || this.m_currency == Currency.RUB || this.m_currency == Currency.EUR;
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x0001620C File Offset: 0x0001440C
	public bool IsNorthAmericanCustomer()
	{
		return this.m_currency == Currency.USD || this.m_currency == Currency.ARS || this.m_currency == Currency.CLP || this.m_currency == Currency.MXN || this.m_currency == Currency.BRL || this.m_currency == Currency.AUD || this.m_currency == Currency.SGD;
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x00016284 File Offset: 0x00014484
	public string GetTaxText()
	{
		Currency currency = this.m_currency;
		switch (currency)
		{
		case Currency.USD:
			return GameStrings.Get("GLUE_STORE_SUMMARY_TAX_DISCLAIMER_USD");
		default:
			if (currency != Currency.CPT && currency != Currency.TPT)
			{
				return GameStrings.Get("GLUE_STORE_SUMMARY_TAX_DISCLAIMER");
			}
			break;
		case Currency.KRW:
			break;
		}
		return string.Empty;
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x000162DC File Offset: 0x000144DC
	public string FormatCostBundle(Network.Bundle bundle)
	{
		if (bundle.Cost == null)
		{
			return string.Empty;
		}
		if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD)
		{
			string mobileProductId = string.Empty;
			if (ApplicationMgr.GetAndroidStore() == AndroidStore.GOOGLE)
			{
				mobileProductId = bundle.GooglePlayID;
			}
			else if (ApplicationMgr.GetAndroidStore() == AndroidStore.AMAZON)
			{
				mobileProductId = bundle.AmazonID;
			}
			string localizedProductPrice = StoreMobilePurchase.GetLocalizedProductPrice(mobileProductId);
			if (localizedProductPrice.Length > 0)
			{
				return localizedProductPrice;
			}
		}
		return this.FormatCostText(bundle.Cost.Value);
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x00016374 File Offset: 0x00014574
	public string FormatCostText(double cost)
	{
		return StoreManager.FormatCostText(cost, this.m_currency);
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x00016384 File Offset: 0x00014584
	public static string FormatCostText(double cost, Currency currency)
	{
		if (!StoreManager.s_currencyToLocaleMap.ContainsKey(currency))
		{
			Error.AddFatalLoc("GLOBAL_ERROR_CURRENCY_INVALID", new object[0]);
		}
		Locale locale = StoreManager.s_currencyToLocaleMap[currency];
		string default_CURRENCY_FORMAT;
		if (!StoreManager.s_currencySpecialFormats.TryGetValue(currency, out default_CURRENCY_FORMAT))
		{
			default_CURRENCY_FORMAT = StoreManager.DEFAULT_CURRENCY_FORMAT;
		}
		string text = Localization.ConvertLocaleToDotNet(locale);
		CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(text);
		switch (locale)
		{
		case Locale.koKR:
			cultureInfo.NumberFormat.CurrencySymbol = "B";
			break;
		case Locale.ruRU:
			cultureInfo.NumberFormat.CurrencySymbol = string.Format(" {0}", cultureInfo.NumberFormat.CurrencySymbol);
			break;
		case Locale.zhTW:
			cultureInfo.NumberFormat.CurrencyGroupSeparator = string.Empty;
			break;
		}
		return string.Format(cultureInfo, default_CURRENCY_FORMAT, new object[]
		{
			cost
		});
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x00016478 File Offset: 0x00014678
	public string GetProductName(List<Network.BundleItem> items)
	{
		string result = string.Empty;
		if (items.Count == 1)
		{
			Network.BundleItem item = items[0];
			result = this.GetSingleItemProductName(item);
		}
		else
		{
			result = this.GetMultiItemProductName(items);
		}
		return result;
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x000164B8 File Offset: 0x000146B8
	public int GetNonPreorderItemCount(List<Network.BundleItem> items)
	{
		int num = 0;
		foreach (Network.BundleItem bundleItem in items)
		{
			if (bundleItem.Product != 5)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x0001651C File Offset: 0x0001471C
	public string GetProductQuantityText(ProductType product, int productData, int quantity)
	{
		string result = string.Empty;
		if (product != 1)
		{
			if (product != 2)
			{
				Debug.LogWarning(string.Format("StoreManager.GetProductQuantityText(): don't know how to format quantity for product {0} (data {1})", product, productData));
			}
			else
			{
				result = GameStrings.Format("GLUE_STORE_SUMMARY_ITEM_ORDERED", new object[]
				{
					quantity,
					GameStrings.Get("GLUE_STORE_PRODUCT_NAME_FORGE_TICKET")
				});
			}
		}
		else
		{
			result = GameStrings.Format("GLUE_STORE_QUANTITY_PACK", new object[]
			{
				quantity
			});
		}
		return result;
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x000165B0 File Offset: 0x000147B0
	public void GetThirdPartyPurchaseStatus(string transactionID)
	{
		Network.GetThirdPartyPurchaseStatus(transactionID);
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x000165B8 File Offset: 0x000147B8
	public bool DoZeroCostTransactionIfPossible(StoreType storeType, Store.ExitCallback exitCallback, object userData, ProductType product, int productData = 0, int numItems = 0)
	{
		if (this.m_waitingToShowStore)
		{
			Log.Rachelle.Print("StoreManager.DoZeroCostTransactionIfPossible(): already waiting to show store", new object[0]);
			return false;
		}
		bool flag = false;
		List<Network.Bundle> availableBundlesForProduct = this.GetAvailableBundlesForProduct(product, false, false, out flag, productData, numItems);
		Network.Bundle bundle = null;
		foreach (Network.Bundle bundle2 in availableBundlesForProduct)
		{
			if (this.IsProductFree(bundle2))
			{
				bundle = bundle2;
				break;
			}
		}
		if (bundle == null)
		{
			return false;
		}
		StoreManager.ZeroCostTransactionData zeroCostTransactionData = new StoreManager.ZeroCostTransactionData(bundle);
		this.m_currentStoreType = storeType;
		this.m_showStoreData.m_exitCallback = exitCallback;
		this.m_showStoreData.m_exitCallbackUserData = userData;
		this.m_showStoreData.m_isTotallyFake = true;
		this.m_showStoreData.m_storeProduct = product;
		this.m_showStoreData.m_storeProductData = ((bundle.Items.Count != 1) ? productData : bundle.Items[0].ProductData);
		this.m_showStoreData.m_storeMode = GeneralStoreMode.NONE;
		this.m_showStoreData.m_zeroCostTransactionData = zeroCostTransactionData;
		this.ShowStoreWhenLoaded();
		return true;
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x000166F4 File Offset: 0x000148F4
	public void StartGeneralTransaction()
	{
		this.StartGeneralTransaction(StoreManager.s_defaultStoreMode);
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x00016708 File Offset: 0x00014908
	public void StartGeneralTransaction(GeneralStoreMode mode)
	{
		if (this.m_waitingToShowStore)
		{
			Log.Rachelle.Print("StoreManager.StartGeneralTransaction(): already waiting to show store", new object[0]);
			return;
		}
		this.m_currentStoreType = StoreType.GENERAL_STORE;
		this.m_showStoreData.m_exitCallback = null;
		this.m_showStoreData.m_exitCallbackUserData = null;
		this.m_showStoreData.m_isTotallyFake = false;
		this.m_showStoreData.m_storeProduct = 0;
		this.m_showStoreData.m_storeProductData = 0;
		this.m_showStoreData.m_storeMode = mode;
		this.m_showStoreData.m_zeroCostTransactionData = null;
		this.ShowStoreWhenLoaded();
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x00016798 File Offset: 0x00014998
	public void StartArenaTransaction(Store.ExitCallback exitCallback, object exitCallbackUserData, bool isTotallyFake)
	{
		if (this.m_waitingToShowStore)
		{
			Log.Rachelle.Print("StoreManager.StartArenaTransaction(): already waiting to show store", new object[0]);
			return;
		}
		this.m_currentStoreType = StoreType.ARENA_STORE;
		this.m_showStoreData.m_exitCallback = exitCallback;
		this.m_showStoreData.m_exitCallbackUserData = null;
		this.m_showStoreData.m_isTotallyFake = isTotallyFake;
		this.m_showStoreData.m_storeProduct = 0;
		this.m_showStoreData.m_storeProductData = 0;
		this.m_showStoreData.m_zeroCostTransactionData = null;
		this.ShowStoreWhenLoaded();
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x0001681C File Offset: 0x00014A1C
	public void StartAdventureTransaction(ProductType product, int productData, Store.ExitCallback exitCallback, object exitCallbackUserData)
	{
		if (this.m_waitingToShowStore)
		{
			Log.Rachelle.Print("StoreManager.StartAdventureTransaction(): already waiting to show store", new object[0]);
			return;
		}
		if (!this.CanBuyProduct(product, productData))
		{
			return;
		}
		this.m_currentStoreType = StoreType.ADVENTURE_STORE;
		this.m_showStoreData.m_exitCallback = exitCallback;
		this.m_showStoreData.m_exitCallbackUserData = exitCallbackUserData;
		this.m_showStoreData.m_isTotallyFake = false;
		this.m_showStoreData.m_storeProduct = product;
		this.m_showStoreData.m_storeProductData = productData;
		this.m_showStoreData.m_zeroCostTransactionData = null;
		this.ShowStoreWhenLoaded();
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x000168B0 File Offset: 0x00014AB0
	public void HideArenaStore()
	{
		Store store = this.GetStore(StoreType.ARENA_STORE);
		if (store != null)
		{
			store.Hide();
			this.HideAllPurchasePopups();
		}
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x000168E0 File Offset: 0x00014AE0
	public bool TransactionInProgress()
	{
		return this.Status == StoreManager.TransactionStatus.IN_PROGRESS_MONEY || this.Status == StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP || this.Status == StoreManager.TransactionStatus.IN_PROGRESS_GOLD_NO_GTAPP || this.Status == StoreManager.TransactionStatus.WAIT_THIRD_PARTY_RECEIPT || this.Status == StoreManager.TransactionStatus.WAIT_THIRD_PARTY_INIT || StoreManager.TransactionStatus.WAIT_ZERO_COST_LICENSE == this.Status;
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x00016938 File Offset: 0x00014B38
	public bool IsPromptShowing()
	{
		return (this.m_storePurchaseAuth != null && this.m_storePurchaseAuth.IsShown()) || (this.m_storeSummary != null && this.m_storeSummary.IsShown()) || (this.m_storeSendToBAM != null && this.m_storeSendToBAM.IsShown()) || (this.m_storeLegalBAMLinks != null && this.m_storeLegalBAMLinks.IsShown()) || (this.m_storeDoneWithBAM != null && this.m_storeDoneWithBAM.IsShown()) || (this.m_storeChallengePrompt != null && this.m_storeChallengePrompt.IsShown());
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x00016A0C File Offset: 0x00014C0C
	public bool HasOutstandingPurchaseNotices(ProductType product)
	{
		NetCache.ProfileNoticePurchase[] array = this.m_outstandingPurchaseNotices.ToArray();
		foreach (NetCache.ProfileNoticePurchase profileNoticePurchase in array)
		{
			Network.Bundle bundle = this.GetBundle(profileNoticePurchase.ProductID);
			if (bundle != null)
			{
				foreach (Network.BundleItem bundleItem in bundle.Items)
				{
					if (bundleItem.Product == product)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x00016AC4 File Offset: 0x00014CC4
	public static ProductType GetAdventureProductType(AdventureDbId adventure)
	{
		ProductType result = 0;
		if (StoreManager.s_adventureToProductMap.TryGetValue(adventure, out result))
		{
			return result;
		}
		return 0;
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x00016AE8 File Offset: 0x00014CE8
	private void ShowStoreWhenLoaded()
	{
		this.m_waitingToShowStore = true;
		if (!this.IsCurrentStoreLoaded())
		{
			this.Load(this.m_currentStoreType);
			return;
		}
		this.ShowStore();
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x00016B10 File Offset: 0x00014D10
	private void ShowStore()
	{
		if (!this.m_licenseAchievesListenerRegistered)
		{
			AchieveManager.Get().RegisterLicenseAddedAchievesUpdatedListener(new AchieveManager.LicenseAddedAchievesUpdatedCallback(this.OnLicenseAddedAchievesUpdated));
			this.m_licenseAchievesListenerRegistered = true;
		}
		if (this.Status == StoreManager.TransactionStatus.READY && AchieveManager.Get().HasActiveLicenseAddedAchieves())
		{
			this.Status = StoreManager.TransactionStatus.WAIT_ZERO_COST_LICENSE;
		}
		Store currentStore = this.GetCurrentStore();
		bool flag = true;
		switch (this.m_currentStoreType)
		{
		case StoreType.GENERAL_STORE:
			if (this.IsOpen())
			{
				GeneralStore generalStore = (GeneralStore)currentStore;
				generalStore.SetMode(this.m_showStoreData.m_storeMode);
			}
			else
			{
				Debug.LogWarning("StoreManager.ShowStore(): Cannot show general store.. Store is not open");
				if (this.m_showStoreData.m_exitCallback != null)
				{
					this.m_showStoreData.m_exitCallback(false, this.m_showStoreData.m_exitCallbackUserData);
				}
				flag = false;
			}
			break;
		case StoreType.ARENA_STORE:
			currentStore.RegisterExitListener(this.m_showStoreData.m_exitCallback, this.m_showStoreData.m_exitCallbackUserData);
			break;
		case StoreType.ADVENTURE_STORE:
			if (this.IsOpen())
			{
				AdventureStore adventureStore = (AdventureStore)currentStore;
				if (adventureStore != null)
				{
					adventureStore.SetAdventureProduct(this.m_showStoreData.m_storeProduct, this.m_showStoreData.m_storeProductData);
				}
				if (this.m_showStoreData.m_exitCallback != null)
				{
					currentStore.RegisterExitListener(this.m_showStoreData.m_exitCallback, this.m_showStoreData.m_exitCallbackUserData);
				}
			}
			else
			{
				Debug.LogWarning("StoreManager.ShowStore(): Cannot show adventure store.. Store is not open");
				if (this.m_showStoreData.m_exitCallback != null)
				{
					this.m_showStoreData.m_exitCallback(false, this.m_showStoreData.m_exitCallbackUserData);
				}
				flag = false;
			}
			break;
		}
		if (flag)
		{
			currentStore.Show(new Store.DelOnStoreShown(this.OnStoreShown), this.m_showStoreData.m_isTotallyFake);
		}
		if (this.IsOpen() && this.Status == StoreManager.TransactionStatus.READY && this.m_showStoreData.m_zeroCostTransactionData != null)
		{
			this.Status = StoreManager.TransactionStatus.WAIT_ZERO_COST_LICENSE;
			this.ActivateStoreCover();
			this.m_storePurchaseAuth.Show(null, true, true);
			Network.GetPurchaseMethod(this.m_showStoreData.m_zeroCostTransactionData.Bundle.ProductID, 1, this.m_currency);
		}
		this.m_waitingToShowStore = false;
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x00016D50 File Offset: 0x00014F50
	private void OnStoreShown()
	{
		StoreManager.StoreShownListener[] array = this.m_storeShownListeners.ToArray();
		foreach (StoreManager.StoreShownListener storeShownListener in array)
		{
			storeShownListener.Fire();
		}
		if (this.TransactionInProgress())
		{
			this.ActivateStoreCover();
			bool enableBackButton = this.ShouldEnablePurchaseAuthBackButton(this.m_currentStoreType);
			if (this.Status == StoreManager.TransactionStatus.WAIT_ZERO_COST_LICENSE && this.m_currentStoreType == StoreType.ADVENTURE_STORE)
			{
				enableBackButton = true;
			}
			if (this.Status == StoreManager.TransactionStatus.WAIT_THIRD_PARTY_RECEIPT && this.m_outOfSessionThirdPartyTransaction && StoreManager.StoreProvider != this.m_activeMoneyOrGTAPPTransaction.Provider)
			{
				this.m_storePurchaseAuth.ShowPurchaseLocked(this.m_activeMoneyOrGTAPPTransaction, enableBackButton, StoreManager.TransactionStatus.WAIT_ZERO_COST_LICENSE == this.Status, delegate(bool showHelp)
				{
					if (showHelp)
					{
						Application.OpenURL(NydusLink.GetSupportLink("outstanding-purchase", false));
					}
					Store currentStore = this.GetCurrentStore();
					currentStore.Close();
				});
			}
			else
			{
				this.m_storePurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, enableBackButton, StoreManager.TransactionStatus.WAIT_ZERO_COST_LICENSE == this.Status);
			}
			return;
		}
		Log.Store.Print("StoreManager.OnStoreShown() m_outstandingPurchaseNoticesCount={0}.", new object[]
		{
			this.m_outstandingPurchaseNotices.Count
		});
		if (this.m_outstandingPurchaseNotices.Count > 0)
		{
			this.ActivateStoreCover();
		}
		this.m_handlePurchaseNoticesCoroutine = SceneMgr.Get().StartCoroutine(this.HandleAllOutstandingPurchaseNotices());
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x00016EA8 File Offset: 0x000150A8
	private IEnumerator HandleAllOutstandingPurchaseNotices()
	{
		NetCache.ProfileNoticePurchase currentNotice = null;
		while (this.m_outstandingPurchaseNotices.Count > 0)
		{
			NetCache.ProfileNoticePurchase outstandingNotice = this.m_outstandingPurchaseNotices[0];
			if (currentNotice != null && currentNotice.NoticeID == outstandingNotice.NoticeID)
			{
				yield return null;
			}
			else
			{
				currentNotice = outstandingNotice;
				Log.Store.Print("StoreManager.OnStoreShown() processing outstanding purchase notice={0}.", new object[]
				{
					outstandingNotice
				});
				bool isGTAPP = Currency.XSG == outstandingNotice.CurrencyType;
				this.SetActiveMoneyOrGTAPPTransaction(outstandingNotice.OriginData, outstandingNotice.ProductID, MoneyOrGTAPPTransaction.UNKNOWN_PROVIDER, isGTAPP, true);
				Network.PurchaseErrorInfo.ErrorType errorType = (outstandingNotice.Origin != NetCache.ProfileNotice.NoticeOrigin.PURCHASE_COMPLETE) ? ((Network.PurchaseErrorInfo.ErrorType)outstandingNotice.Data) : Network.PurchaseErrorInfo.ErrorType.SUCCESS;
				this.HandlePurchaseError(StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE, errorType, string.Empty, string.Empty, isGTAPP);
			}
		}
		yield break;
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x00016EC4 File Offset: 0x000150C4
	private bool ShouldEnablePurchaseAuthBackButton(StoreType storeType)
	{
		StoreType currentStoreType = this.m_currentStoreType;
		return currentStoreType == StoreType.ARENA_STORE;
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00016EE8 File Offset: 0x000150E8
	private bool IsCurrentStoreLoaded()
	{
		Store currentStore = this.GetCurrentStore();
		return !(currentStore == null) && currentStore.IsReady() && !(this.m_storePurchaseAuth == null) && !(this.m_storeSummary == null) && !(this.m_storeSendToBAM == null) && !(this.m_storeLegalBAMLinks == null) && !(this.m_storeDoneWithBAM == null) && !(this.m_storeChallengePrompt == null);
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x00016F88 File Offset: 0x00015188
	private void Load(StoreType storeType)
	{
		if (this.GetCurrentStore() != null)
		{
			return;
		}
		switch (storeType)
		{
		case StoreType.GENERAL_STORE:
			AssetLoader.Get().LoadGameObject(this.s_storePrefab, new AssetLoader.GameObjectCallback(this.OnGeneralStoreLoaded), null, false);
			break;
		case StoreType.ARENA_STORE:
			AssetLoader.Get().LoadGameObject(this.s_arenaStorePrefab, new AssetLoader.GameObjectCallback(this.OnArenaStoreLoaded), null, false);
			break;
		case StoreType.ADVENTURE_STORE:
			AssetLoader.Get().LoadGameObject(this.s_adventureStorePrefab, new AssetLoader.GameObjectCallback(this.OnAdventureStoreLoaded), null, false);
			break;
		}
		AssetLoader.Get().LoadGameObject(this.s_storePurchaseAuthPrefab, new AssetLoader.GameObjectCallback(this.OnStorePurchaseAuthLoaded), null, false);
		AssetLoader.Get().LoadGameObject(this.s_storeSummaryPrefab, new AssetLoader.GameObjectCallback(this.OnStoreSummaryLoaded), null, false);
		AssetLoader.Get().LoadGameObject(this.s_storeSendToBAMPrefab, new AssetLoader.GameObjectCallback(this.OnStoreSendToBAMLoaded), null, false);
		AssetLoader.Get().LoadGameObject(this.s_storeDoneWithBAMPrefab, new AssetLoader.GameObjectCallback(this.OnStoreDoneWithBAMLoaded), null, false);
		AssetLoader.Get().LoadGameObject(this.s_storeChallengePromptPrefab, new AssetLoader.GameObjectCallback(this.OnStoreChallengePromptLoaded), null, false);
		AssetLoader.Get().LoadGameObject(this.s_storeLegalBAMLinksPrefab, new AssetLoader.GameObjectCallback(this.OnStoreLegalBAMLinksLoaded), null, false);
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00017114 File Offset: 0x00015314
	public void UnloadAndFreeMemory()
	{
		foreach (KeyValuePair<StoreType, Store> keyValuePair in this.m_stores)
		{
			if (keyValuePair.Value != null)
			{
				Object.Destroy(keyValuePair.Value);
			}
		}
		this.m_stores.Clear();
		if (this.m_storePurchaseAuth != null)
		{
			Object.Destroy(this.m_storePurchaseAuth.gameObject);
			this.m_storePurchaseAuth = null;
		}
		if (this.m_storeSummary != null)
		{
			Object.Destroy(this.m_storeSummary.gameObject);
			this.m_storeSummary = null;
		}
		if (this.m_storeSendToBAM != null)
		{
			Object.Destroy(this.m_storeSendToBAM.gameObject);
			this.m_storeSendToBAM = null;
		}
		if (this.m_storeLegalBAMLinks != null)
		{
			Object.Destroy(this.m_storeLegalBAMLinks.gameObject);
			this.m_storeLegalBAMLinks = null;
		}
		if (this.m_storeDoneWithBAM != null)
		{
			Object.Destroy(this.m_storeDoneWithBAM.gameObject);
			this.m_storeDoneWithBAM = null;
		}
		if (this.m_storeChallengePrompt != null)
		{
			Object.Destroy(this.m_storeChallengePrompt.gameObject);
			this.m_storeChallengePrompt = null;
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000636 RID: 1590 RVA: 0x00017280 File Offset: 0x00015480
	// (set) Token: 0x06000637 RID: 1591 RVA: 0x00017288 File Offset: 0x00015488
	private StoreManager.TransactionStatus Status
	{
		get
		{
			return this.m_status;
		}
		set
		{
			if (this.m_lastCancelRequestTime == 0L && this.m_status == StoreManager.TransactionStatus.UNKNOWN)
			{
				this.m_lastCancelRequestTime = DateTime.Now.Ticks;
			}
			this.m_status = value;
			this.FireStatusChangedEventIfNeeded();
		}
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000638 RID: 1592 RVA: 0x000172CB File Offset: 0x000154CB
	// (set) Token: 0x06000639 RID: 1593 RVA: 0x000172D3 File Offset: 0x000154D3
	private bool NoticesReady
	{
		get
		{
			return this.m_noticesReady;
		}
		set
		{
			this.m_noticesReady = value;
			this.FireStatusChangedEventIfNeeded();
		}
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x0600063A RID: 1594 RVA: 0x000172E2 File Offset: 0x000154E2
	// (set) Token: 0x0600063B RID: 1595 RVA: 0x000172EA File Offset: 0x000154EA
	private bool BattlePayAvailable
	{
		get
		{
			return this.m_battlePayAvailable;
		}
		set
		{
			this.m_battlePayAvailable = value;
			this.FireStatusChangedEventIfNeeded();
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x0600063C RID: 1596 RVA: 0x000172F9 File Offset: 0x000154F9
	// (set) Token: 0x0600063D RID: 1597 RVA: 0x00017301 File Offset: 0x00015501
	private bool FeaturesReady
	{
		get
		{
			return this.m_featuresReady;
		}
		set
		{
			this.m_featuresReady = value;
			this.FireStatusChangedEventIfNeeded();
		}
	}

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x0600063E RID: 1598 RVA: 0x00017310 File Offset: 0x00015510
	// (set) Token: 0x0600063F RID: 1599 RVA: 0x00017318 File Offset: 0x00015518
	private bool ConfigLoaded
	{
		get
		{
			return this.m_configLoaded;
		}
		set
		{
			this.m_configLoaded = value;
			this.FireStatusChangedEventIfNeeded();
		}
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x00017328 File Offset: 0x00015528
	private void FireStatusChangedEventIfNeeded()
	{
		bool flag = this.IsOpen();
		if (this.m_openWhenLastEventFired == flag)
		{
			return;
		}
		StoreManager.StatusChangedListener[] array = this.m_statusChangedListeners.ToArray();
		foreach (StoreManager.StatusChangedListener statusChangedListener in array)
		{
			statusChangedListener.Fire(flag);
		}
		this.m_openWhenLastEventFired = flag;
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00017384 File Offset: 0x00015584
	private NetCache.NetCacheFeatures GetNetCacheFeatures()
	{
		if (!this.FeaturesReady)
		{
			return null;
		}
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (netObject == null)
		{
			this.FeaturesReady = false;
		}
		return netObject;
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x000173B8 File Offset: 0x000155B8
	private bool AlreadyOwnsProduct(ProductType product, int productData)
	{
		switch (product)
		{
		case 1:
		case 2:
			return false;
		case 3:
		case 4:
		case 7:
			return AdventureProgressMgr.Get().OwnsWing(productData);
		case 5:
			return CardBackManager.Get().IsCardBackOwned(productData);
		case 6:
		{
			string cardID = GameUtils.TranslateDbIdToCardId(productData);
			return CollectionManager.Get().IsCardInCollection(cardID, TAG_PREMIUM.NORMAL);
		}
		default:
			Debug.LogWarning(string.Format("StoreManager.AlreadyOwnsProduct() unknown product {0} productData {1}", product, productData));
			return false;
		}
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x0001743C File Offset: 0x0001563C
	private string GetSingleItemProductName(Network.BundleItem item)
	{
		string result = string.Empty;
		switch (item.Product)
		{
		case 1:
		{
			BoosterDbfRecord record = GameDbf.Booster.GetRecord(item.ProductData);
			string text = record.Name;
			result = GameStrings.Format("GLUE_STORE_PRODUCT_NAME_PACK", new object[]
			{
				item.Quantity,
				text
			});
			break;
		}
		case 2:
			result = GameStrings.Get("GLUE_STORE_PRODUCT_NAME_FORGE_TICKET");
			break;
		case 3:
		case 4:
		case 7:
			result = AdventureProgressMgr.Get().GetWingName(item.ProductData);
			break;
		case 5:
		{
			CardBackDbfRecord record2 = GameDbf.CardBack.GetRecord(item.ProductData);
			if (record2 != null)
			{
				result = record2.Name;
			}
			break;
		}
		case 6:
		{
			EntityDef entityDef = DefLoader.Get().GetEntityDef(item.ProductData);
			if (entityDef != null)
			{
				result = entityDef.GetName();
			}
			break;
		}
		default:
			Debug.LogWarning(string.Format("StoreManager.GetSingleItemProductName(): don't know how to format name for bundle product {0}", item.Product));
			break;
		}
		return result;
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x0001755C File Offset: 0x0001575C
	private string GetMultiItemProductName(List<Network.BundleItem> items)
	{
		HashSet<ProductType> productsInItemList = this.GetProductsInItemList(items);
		if (productsInItemList.Contains(3))
		{
			return GameStrings.Format("GLUE_STORE_PRODUCT_NAME_NAXX_WING_BUNDLE", new object[]
			{
				items.Count
			});
		}
		if (productsInItemList.Contains(4))
		{
			if (productsInItemList.Contains(5))
			{
				return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_BRM_PRESALE_BUNDLE");
			}
			return GameStrings.Format("GLUE_STORE_PRODUCT_NAME_BRM_WING_BUNDLE", new object[]
			{
				items.Count
			});
		}
		else
		{
			if (productsInItemList.Contains(7))
			{
				return GameStrings.Format("GLUE_STORE_PRODUCT_NAME_LOE_WING_BUNDLE", new object[]
				{
					items.Count
				});
			}
			if (productsInItemList.Contains(6))
			{
				Network.BundleItem bundleItem = items.Find((Network.BundleItem obj) => obj.Product == 6);
				if (bundleItem != null)
				{
					return this.GetSingleItemProductName(bundleItem);
				}
			}
			else if (productsInItemList.Contains(1) && productsInItemList.Contains(5))
			{
				Network.BundleItem bundleItem2 = items.Find((Network.BundleItem obj) => obj.Product == 1 && obj.ProductData == 10);
				if (bundleItem2 != null)
				{
					return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_TGT_PRESALE_BUNDLE");
				}
				Network.BundleItem bundleItem3 = items.Find((Network.BundleItem obj) => obj.Product == 1 && obj.ProductData == 11);
				if (bundleItem3 != null)
				{
					return GameStrings.Get("GLUE_STORE_PRODUCT_NAME_OG_PRESALE_BUNDLE");
				}
			}
			string text = string.Empty;
			foreach (Network.BundleItem bundleItem4 in items)
			{
				text += string.Format("[Product={0},ProductData={1},Quantity={2}],", bundleItem4.Product, bundleItem4.ProductData, bundleItem4.Quantity);
			}
			Debug.LogWarning(string.Format("StoreManager.GetMultiItemProductName(): don't know how to format product name for items '{0}'", text));
			return string.Empty;
		}
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x00017768 File Offset: 0x00015968
	private bool GetBoosterGoldCostNoGTAPP(int boosterID, out long cost)
	{
		cost = 0L;
		if (!this.m_goldCostBooster.ContainsKey(boosterID))
		{
			return false;
		}
		if (!this.CanBuyBoosterWithGold(boosterID))
		{
			return false;
		}
		Network.GoldCostBooster goldCostBooster = this.m_goldCostBooster[boosterID];
		if (goldCostBooster.Cost == null)
		{
			return false;
		}
		if (goldCostBooster.Cost.Value <= 0L)
		{
			return false;
		}
		cost = goldCostBooster.Cost.Value;
		return true;
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x000177E4 File Offset: 0x000159E4
	private bool GetArenaGoldCostNoGTAPP(out long cost)
	{
		cost = 0L;
		if (this.m_goldCostArena == null)
		{
			return false;
		}
		cost = this.m_goldCostArena.Value;
		return true;
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x00017818 File Offset: 0x00015A18
	private long GetIncreasedRequestDelayTicks(long currentRequestDelayTicks, long minimumDelayTicks)
	{
		if (currentRequestDelayTicks < minimumDelayTicks)
		{
			return minimumDelayTicks;
		}
		long num = currentRequestDelayTicks * 2L;
		return Math.Min(num, StoreManager.MAX_REQUEST_DELAY_TICKS);
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x00017840 File Offset: 0x00015A40
	private void RequestStatusIfNeeded(long now)
	{
		if (!this.ConfigLoaded)
		{
			return;
		}
		bool flag = this.Status == StoreManager.TransactionStatus.UNKNOWN || this.Status == StoreManager.TransactionStatus.IN_PROGRESS_MONEY || this.Status == StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP || this.Status == StoreManager.TransactionStatus.CHALLENGE_SUBMITTED || StoreManager.TransactionStatus.CHALLENGE_CANCELED == this.Status;
		if (this.BattlePayAvailable && !flag)
		{
			return;
		}
		if (now - this.m_lastStatusRequestTime < this.m_statusRequestDelayTicks)
		{
			return;
		}
		this.m_statusRequestDelayTicks = this.GetIncreasedRequestDelayTicks(this.m_statusRequestDelayTicks, StoreManager.MIN_STATUS_REQUEST_DELAY_TICKS);
		Log.Rachelle.Print(string.Format("StoreManager updated STATUS delay, now waiting {0} seconds between requests", this.m_statusRequestDelayTicks / 10000000L), new object[0]);
		this.m_lastStatusRequestTime = now;
		Network.RequestBattlePayStatus();
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x0001790C File Offset: 0x00015B0C
	private void RequestConfigIfNeeded(long now)
	{
		if (this.ConfigLoaded)
		{
			return;
		}
		if (now - this.m_lastConfigRequestTime < this.m_configRequestDelayTicks)
		{
			return;
		}
		this.m_configRequestDelayTicks = this.GetIncreasedRequestDelayTicks(this.m_configRequestDelayTicks, StoreManager.MIN_CONFIG_REQUEST_DELAY_TICKS);
		Log.Rachelle.Print(string.Format("StoreManager updated CONFIG delay, now waiting {0} seconds between requests", this.m_configRequestDelayTicks / 10000000L), new object[0]);
		this.m_lastConfigRequestTime = now;
		Network.RequestBattlePayConfig();
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x00017988 File Offset: 0x00015B88
	private bool AutoCancelPurchaseIfNeeded(long now)
	{
		if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD)
		{
			if (now - this.m_lastCancelRequestTime < this.m_ticksBeforeAutoCancelThirdParty)
			{
				return false;
			}
		}
		else if (now - this.m_lastCancelRequestTime < this.m_ticksBeforeAutoCancel)
		{
			return false;
		}
		return this.AutoCancelPurchaseIfPossible();
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x000179E4 File Offset: 0x00015BE4
	private bool AutoCancelPurchaseIfPossible()
	{
		if (this.m_activeMoneyOrGTAPPTransaction == null)
		{
			return false;
		}
		if (this.m_activeMoneyOrGTAPPTransaction.Provider == null)
		{
			return false;
		}
		if (this.m_activeMoneyOrGTAPPTransaction.Provider.Value == 1)
		{
			switch (this.Status)
			{
			case StoreManager.TransactionStatus.IN_PROGRESS_MONEY:
			case StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP:
			case StoreManager.TransactionStatus.WAIT_METHOD_OF_PAYMENT:
			case StoreManager.TransactionStatus.WAIT_CONFIRM:
			case StoreManager.TransactionStatus.WAIT_RISK:
			case StoreManager.TransactionStatus.CHALLENGE_SUBMITTED:
			case StoreManager.TransactionStatus.CHALLENGE_CANCELED:
				Log.Rachelle.Print("StoreManager.AutoCancelPurchaseIfPossible() canceling Blizzard purchase, status={0}", new object[]
				{
					this.Status
				});
				this.Status = StoreManager.TransactionStatus.AUTO_CANCELING;
				this.m_lastCancelRequestTime = DateTime.Now.Ticks;
				Network.CancelBlizzardPurchase(true, default(CancelPurchase.CancelReason?), null);
				return true;
			}
			return false;
		}
		if (this.Status == StoreManager.TransactionStatus.WAIT_THIRD_PARTY_RECEIPT && this.m_shouldAutoCancelThirdPartyTransaction)
		{
			Log.Rachelle.Print("StoreManager.AutoCancelPurchaseIfPossible() canceling Third-Party purchase", new object[0]);
			Log.Yim.Print("StoreManager.AutoCancelPurchaseIfPossible() canceling Third-Party purchase", new object[0]);
			this.m_previousStatusBeforeAutoCancel = this.Status;
			this.Status = StoreManager.TransactionStatus.AUTO_CANCELING;
			this.m_lastCancelRequestTime = DateTime.Now.Ticks;
			string message = (this.m_activeMoneyOrGTAPPTransaction != null) ? this.m_activeMoneyOrGTAPPTransaction.ID.ToString() : string.Empty;
			BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_INFO, BIReport.TelemetryEvent.EVENT_THIRD_PARTY_PURCHASE_AUTO_CANCEL, 0, message);
			Network.CancelThirdPartyPurchase(4, null);
			return true;
		}
		return false;
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x00017B78 File Offset: 0x00015D78
	private void CancelBlizzardPurchase(CancelPurchase.CancelReason? reason = null, string errorMessage = null)
	{
		Log.Rachelle.Print("StoreManager.CancelBlizzardPurchase() reason=", new object[]
		{
			(reason == null) ? "null" : reason.Value.ToString()
		});
		this.Status = StoreManager.TransactionStatus.USER_CANCELING;
		this.m_lastCancelRequestTime = DateTime.Now.Ticks;
		Network.CancelBlizzardPurchase(false, reason, errorMessage);
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x00017BE8 File Offset: 0x00015DE8
	public void CancelThirdPartyPurchase(CancelPurchase.CancelReason reason)
	{
		Log.Rachelle.Print(string.Format("StoreManager.CancelThirdPartyPurchase(): reason={0}", reason), new object[0]);
		this.Status = StoreManager.TransactionStatus.USER_CANCELING;
		this.m_lastCancelRequestTime = DateTime.Now.Ticks;
		Network.CancelThirdPartyPurchase(reason, null);
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00017C37 File Offset: 0x00015E37
	private bool HaveProductsToSell()
	{
		return this.m_bundles.Count > 0 || this.m_goldCostBooster.Count > 0 || this.m_goldCostArena != null;
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00017C6C File Offset: 0x00015E6C
	public bool IsBundleRealMoneyOptionAvailableNow(Network.Bundle bundle)
	{
		return bundle != null && bundle.Cost != null && SpecialEventManager.Get().IsEventActive(bundle.ProductEvent, true) && SpecialEventManager.Get().IsEventActive(bundle.RealMoneyProductEvent, true);
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x00017CC0 File Offset: 0x00015EC0
	public bool IsBundleGoldOptionAvailableNow(Network.Bundle bundle)
	{
		return bundle != null && bundle.GoldCost != null && SpecialEventManager.Get().IsEventActive(bundle.ProductEvent, true);
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x00017D03 File Offset: 0x00015F03
	private bool IsBundleAvailableNow(Network.Bundle bundle, bool requireRealMoneyOption)
	{
		return bundle != null && (this.IsBundleRealMoneyOptionAvailableNow(bundle) || (!requireRealMoneyOption && this.IsBundleGoldOptionAvailableNow(bundle)));
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x00017D2C File Offset: 0x00015F2C
	private void OnStoreExit(bool authorizationBackButtonPressed, object userData)
	{
		if (this.m_activeMoneyOrGTAPPTransaction != null)
		{
			this.m_activeMoneyOrGTAPPTransaction.ClosedStore = true;
		}
		string text = this.m_storeChallengePrompt.HideChallenge();
		if (text != null)
		{
			this.OnChallengeCancel(text);
		}
		else
		{
			this.AutoCancelPurchaseIfPossible();
		}
		this.DeactivateStoreCover();
		this.HideAllPurchasePopups();
		StoreManager.StoreHiddenListener[] array = this.m_storeHiddenListeners.ToArray();
		foreach (StoreManager.StoreHiddenListener storeHiddenListener in array)
		{
			storeHiddenListener.Fire();
		}
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00017DB2 File Offset: 0x00015FB2
	private void OnStoreInfo(object userData)
	{
		this.ActivateStoreCover();
		this.m_storeSendToBAM.Show(null, StoreSendToBAM.BAMReason.PAYMENT_INFO, string.Empty, false);
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x00017DD0 File Offset: 0x00015FD0
	public bool CanBuyBundle(Network.Bundle bundleToBuy)
	{
		if (bundleToBuy == null)
		{
			Debug.LogWarning("Null bundle passed to CanBuyBundle!");
			return false;
		}
		if (AchieveManager.Get() == null || !AchieveManager.Get().IsReady())
		{
			return false;
		}
		if (bundleToBuy.Items.Count < 1)
		{
			Debug.LogWarning(string.Format("Attempting to buy bundle {0}, which does not contain any items!", bundleToBuy.ProductID));
			return false;
		}
		bool flag = false;
		List<Network.Bundle> availableBundlesForProduct = this.GetAvailableBundlesForProduct(bundleToBuy.Items[0].Product, false, false, out flag, 0, 0);
		foreach (Network.Bundle bundle in availableBundlesForProduct)
		{
			if (bundle.ProductID == bundleToBuy.ProductID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x00017EB8 File Offset: 0x000160B8
	private bool CanBuyProduct(ProductType product, int productData)
	{
		if (AchieveManager.Get() == null || !AchieveManager.Get().IsReady())
		{
			Debug.LogWarning(string.Format("Waiting for AchieveManager to check if we already own product {0} of type {1}, cannot purchase!", productData, product));
			return false;
		}
		if (this.AlreadyOwnsProduct(product, productData))
		{
			Debug.LogWarning(string.Format("Already own product {0} of type {1}, cannot purchase again!", productData, product));
			return false;
		}
		return true;
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x00017F28 File Offset: 0x00016128
	private void OnStoreBuyWithMoney(string productID, int quantity, object userData)
	{
		Network.Bundle bundle = this.GetBundle(productID);
		if (!this.CanBuyBundle(bundle))
		{
			Debug.LogWarning(string.Format("Attempting to buy bundle {0}, but it is not available! achieveReady={1}", bundle.ProductID, AchieveManager.Get().IsReady()));
			return;
		}
		this.ActivateStoreCover();
		if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD)
		{
			if (this.IsProductFree(bundle))
			{
				Debug.LogWarning("Attempting to purchase a free bundle!  This should not be possible, and you would be charged by the Third Party Provider price if we allowed it.");
				this.DeactivateStoreCover();
				return;
			}
			BattlePayProvider storeProvider = StoreManager.StoreProvider;
			this.SetActiveMoneyOrGTAPPTransaction((long)StoreManager.UNKNOWN_TRANSACTION_ID, productID, new BattlePayProvider?(storeProvider), false, false);
			this.m_storePurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, this.ShouldEnablePurchaseAuthBackButton(this.m_currentStoreType), false);
			this.Status = StoreManager.TransactionStatus.WAIT_THIRD_PARTY_INIT;
			Network.BeginThirdPartyPurchase(storeProvider, productID, quantity);
			Log.Yim.Print(string.Concat(new object[]
			{
				"Network.BeginThirdPartyPurchase(",
				storeProvider,
				", ",
				productID,
				", ",
				quantity,
				")"
			}), new object[0]);
		}
		else
		{
			this.SetActiveMoneyOrGTAPPTransaction((long)StoreManager.UNKNOWN_TRANSACTION_ID, productID, new BattlePayProvider?(1), false, false);
			this.Status = StoreManager.TransactionStatus.WAIT_METHOD_OF_PAYMENT;
			this.m_storePurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false, false);
			Network.GetPurchaseMethod(productID, quantity, this.m_currency);
		}
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x00018088 File Offset: 0x00016288
	private void OnStoreBuyWithGTAPP(string productID, int quantity, object userData)
	{
		Network.Bundle bundle = this.GetBundle(productID);
		if (!this.CanBuyBundle(bundle))
		{
			Debug.LogWarning(string.Format("Attempting to buy bundle {0}, but it is not available! achieveReady={1}", bundle.ProductID, AchieveManager.Get().IsReady()));
			return;
		}
		this.ActivateStoreCover();
		this.SetActiveMoneyOrGTAPPTransaction((long)StoreManager.UNKNOWN_TRANSACTION_ID, productID, new BattlePayProvider?(1), true, false);
		this.Status = StoreManager.TransactionStatus.WAIT_METHOD_OF_PAYMENT;
		this.m_storePurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, false, false);
		Network.GetPurchaseMethod(productID, quantity, Currency.XSG);
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x0001810C File Offset: 0x0001630C
	private void OnStoreBuyWithGoldNoGTAPP(NoGTAPPTransactionData noGTAPPtransactionData, object userData)
	{
		if (noGTAPPtransactionData == null)
		{
			return;
		}
		if (!this.CanBuyProduct(noGTAPPtransactionData.Product, noGTAPPtransactionData.ProductData))
		{
			return;
		}
		this.ActivateStoreCover();
		this.m_storePurchaseAuth.Show(null, this.ShouldEnablePurchaseAuthBackButton(this.m_currentStoreType), false);
		this.Status = StoreManager.TransactionStatus.IN_PROGRESS_GOLD_NO_GTAPP;
		Network.PurchaseViaGold(noGTAPPtransactionData.Quantity, noGTAPPtransactionData.Product, noGTAPPtransactionData.ProductData);
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00018178 File Offset: 0x00016378
	private void OnSummaryConfirm(string productID, int quantity, object userData)
	{
		this.m_storePurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, this.ShouldEnablePurchaseAuthBackButton(this.m_currentStoreType), false);
		if (this.m_challengePurchaseMethod != null)
		{
			this.m_storeChallengePrompt.StartCoroutine(this.m_storeChallengePrompt.Show(this.m_challengePurchaseMethod.ChallengeURL));
		}
		else
		{
			this.ConfirmPurchase();
		}
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x000181DC File Offset: 0x000163DC
	private void ConfirmPurchase()
	{
		this.m_lastStatusRequestTime = DateTime.Now.Ticks;
		this.m_statusRequestDelayTicks = StoreManager.EARLY_STATUS_REQUEST_DELAY_TICKS;
		Log.Rachelle.Print(string.Format("StoreManager updating STATUS delay due to summary confirm, now waiting {0} seconds between requests", this.m_statusRequestDelayTicks / 10000000L), new object[0]);
		this.Status = ((!this.m_activeMoneyOrGTAPPTransaction.IsGTAPP) ? StoreManager.TransactionStatus.IN_PROGRESS_MONEY : StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP);
		Network.ConfirmPurchase();
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x00018258 File Offset: 0x00016458
	private void OnSummaryCancel(object userData)
	{
		this.CancelBlizzardPurchase(default(CancelPurchase.CancelReason?), null);
		this.DeactivateStoreCover();
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0001827C File Offset: 0x0001647C
	private void OnThirdPartyPurchaseApproved()
	{
		Log.Yim.Print("OnThirdPartyPurchaseApproved", new object[0]);
		StoreManager.TransactionStatus status = this.Status;
		this.Status = StoreManager.TransactionStatus.WAIT_THIRD_PARTY_RECEIPT;
		if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD)
		{
			if (this.m_activeMoneyOrGTAPPTransaction == null)
			{
				Debug.LogWarning("StoreManager.OnThirdPartyPurchaseApproved() but m_activeMoneyOrGTAPPTransaction is null");
				return;
			}
			if ((long)StoreManager.UNKNOWN_TRANSACTION_ID == this.m_activeMoneyOrGTAPPTransaction.ID)
			{
				Debug.LogWarning("StoreManager.OnThirdPartyPurchaseApproved() but m_activeMoneyOrGTAPPTransaction ID is UNKNOWN_TRANSACTION_ID");
				return;
			}
			if (this.m_activeMoneyOrGTAPPTransaction.Provider != null && this.m_activeMoneyOrGTAPPTransaction.Provider.Value != 2 && this.m_activeMoneyOrGTAPPTransaction.Provider.Value != 3 && this.m_activeMoneyOrGTAPPTransaction.Provider.Value != 4)
			{
				Debug.LogWarning(string.Format("StoreManager.OnThirdPartyPurchaseApproved() active transaction is not an Third Party Provider transaction, so Third Party Provider shouldn't be servicing it (m_activeMoneyOrGTAPPTransaction = {0})", this.m_activeMoneyOrGTAPPTransaction));
				return;
			}
			if (this.GetBundle(this.m_activeMoneyOrGTAPPTransaction.ProductID) == null)
			{
				Debug.LogWarning(string.Format("StoreManager.OnThirdPartyPurchaseApproved() but bundle is null (m_activeMoneyOrGTAPPTransaction = {0})", this.m_activeMoneyOrGTAPPTransaction));
				return;
			}
			if (this.IsCurrentStoreLoaded())
			{
				this.ActivateStoreCover();
			}
			BattlePayProvider storeProvider = StoreManager.StoreProvider;
			this.SetActiveMoneyOrGTAPPTransaction(this.m_activeMoneyOrGTAPPTransaction.ID, this.m_activeMoneyOrGTAPPTransaction.ProductID, new BattlePayProvider?(storeProvider), false, false);
			if (this.IsCurrentStoreLoaded())
			{
				this.m_storePurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, this.ShouldEnablePurchaseAuthBackButton(this.m_currentStoreType), false);
			}
			if (status == StoreManager.TransactionStatus.WAIT_THIRD_PARTY_INIT)
			{
				string text = null;
				string message = this.m_activeMoneyOrGTAPPTransaction.ID.ToString() + "|" + text;
				BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_INFO, BIReport.TelemetryEvent.EVENT_THIRD_PARTY_PURCHASE_REQUEST, 0, message);
				StoreMobilePurchase.PurchaseProductById(text);
			}
			else
			{
				Debug.LogWarning(string.Format("StoreManager.OnThirdPartyPurchaseApproved() previous Status was {0}, expected {1} (m_activeMoneyOrGTAPPTransaction = {2}", status, StoreManager.TransactionStatus.WAIT_THIRD_PARTY_INIT, this.m_activeMoneyOrGTAPPTransaction));
				Log.Yim.Print("Previous ongoing purchase detected, expecting third party receipt..", new object[0]);
				Log.Yim.Print("m_activeMoneyOrGTAPPTransaction = " + this.m_activeMoneyOrGTAPPTransaction, new object[0]);
				this.m_outOfSessionThirdPartyTransaction = true;
				if (storeProvider == this.m_activeMoneyOrGTAPPTransaction.Provider.Value)
				{
					this.m_requestedThirdPartyProductId = string.Empty;
					string message2 = this.m_activeMoneyOrGTAPPTransaction.ID + "|" + this.m_requestedThirdPartyProductId;
					BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_INFO, BIReport.TelemetryEvent.EVENT_THIRD_PARTY_PURCHASE_RECEIPT_REQUEST, 0, message2);
					StoreMobilePurchase.WaitingOnThirdPartyReceipt(this.m_requestedThirdPartyProductId);
				}
			}
		}
		else
		{
			this.m_outOfSessionThirdPartyTransaction = true;
		}
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00018524 File Offset: 0x00016724
	private void OnSummaryInfo(object userData)
	{
		this.ActivateStoreCover();
		this.AutoCancelPurchaseIfPossible();
		this.m_storeSendToBAM.Show(null, StoreSendToBAM.BAMReason.EULA_AND_TOS, string.Empty, false);
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00018551 File Offset: 0x00016751
	private void OnSummaryPaymentAndTOS(object userData)
	{
		this.AutoCancelPurchaseIfPossible();
		this.m_storeLegalBAMLinks.Show();
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00018568 File Offset: 0x00016768
	private void OnChallengeComplete(string challengeID, bool isSuccess, CancelPurchase.CancelReason? reason, string internalErrorInfo)
	{
		if (!isSuccess)
		{
			this.OnChallengeCancel_Internal(challengeID, reason, internalErrorInfo);
			return;
		}
		this.m_storePurchaseAuth.Show(this.m_activeMoneyOrGTAPPTransaction, this.ShouldEnablePurchaseAuthBackButton(this.m_currentStoreType), false);
		this.m_lastStatusRequestTime = DateTime.Now.Ticks;
		this.m_statusRequestDelayTicks = StoreManager.EARLY_STATUS_REQUEST_DELAY_TICKS;
		Log.Rachelle.Print(string.Format("StoreManager updating STATUS delay due to challenge answer submit, now waiting {0} seconds between requests", this.m_statusRequestDelayTicks / 10000000L), new object[0]);
		this.Status = StoreManager.TransactionStatus.CHALLENGE_SUBMITTED;
		this.ConfirmPurchase();
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x000185FC File Offset: 0x000167FC
	private void OnChallengeCancel(string challengeID)
	{
		this.OnChallengeCancel_Internal(challengeID, default(CancelPurchase.CancelReason?), null);
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0001861C File Offset: 0x0001681C
	private void OnChallengeCancel_Internal(string challengeID, CancelPurchase.CancelReason? reason, string errorMessage)
	{
		Debug.LogFormat("Canceling purchase from challengeId={0} reason={1} msg={2}", new object[]
		{
			challengeID,
			(reason == null) ? "null" : reason.Value.ToString(),
			errorMessage
		});
		this.Status = StoreManager.TransactionStatus.CHALLENGE_CANCELED;
		this.m_lastStatusRequestTime = DateTime.Now.Ticks;
		this.m_statusRequestDelayTicks = StoreManager.CHALLENGE_CANCEL_STATUS_REQUEST_DELAY_TICKS;
		this.CancelBlizzardPurchase(reason, errorMessage);
		this.DeactivateStoreCover();
		this.HideAllPurchasePopups();
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x000186A5 File Offset: 0x000168A5
	private void OnSendToBAMOkay(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, StoreSendToBAM.BAMReason reason)
	{
		if (moneyOrGTAPPTransaction != null)
		{
			this.ConfirmActiveMoneyTransaction(moneyOrGTAPPTransaction.ID);
		}
		if (reason == StoreSendToBAM.BAMReason.PAYMENT_INFO)
		{
			this.OnDoneWithBAM();
			return;
		}
		this.m_storeDoneWithBAM.Show();
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x000186D1 File Offset: 0x000168D1
	private void OnSendToBAMCancel(MoneyOrGTAPPTransaction moneyOrGTAPPTransaction)
	{
		if (moneyOrGTAPPTransaction != null)
		{
			this.ConfirmActiveMoneyTransaction(moneyOrGTAPPTransaction.ID);
		}
		this.DeactivateStoreCover();
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x000186EB File Offset: 0x000168EB
	private void OnSendToBAMLegal(StoreLegalBAMLinks.BAMReason reason)
	{
		this.DeactivateStoreCover();
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x000186F3 File Offset: 0x000168F3
	private void OnSendToBAMLegalCancel()
	{
		this.DeactivateStoreCover();
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x000186FB File Offset: 0x000168FB
	private void OnDoneWithBAM()
	{
		this.DeactivateStoreCover();
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00018704 File Offset: 0x00016904
	private void OnAchievesUpdated(object userData)
	{
		this.m_completedAchieves = AchieveManager.Get().GetNewCompletedAchieves();
		this.ShowCompletedAchieve();
		StoreManager.StoreAchievesData storeAchievesData = userData as StoreManager.StoreAchievesData;
		StoreManager.StoreAchievesListener[] array = this.m_storeAchievesListeners.ToArray();
		foreach (StoreManager.StoreAchievesListener storeAchievesListener in array)
		{
			storeAchievesListener.Fire(storeAchievesData.Bundle, storeAchievesData.MethodOfPayment);
		}
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x0001876C File Offset: 0x0001696C
	private void OnLicenseAddedAchievesUpdated(List<Achievement> activeLicenseAddedAchieves, object userData)
	{
		if (this.Status != StoreManager.TransactionStatus.WAIT_ZERO_COST_LICENSE)
		{
			return;
		}
		bool flag = activeLicenseAddedAchieves.Count > 0;
		if (flag)
		{
			return;
		}
		Log.Rachelle.Print("StoreManager.OnLicenseAddedAchievesUpdated(): done waiting for licenses!", new object[0]);
		if (this.IsCurrentStoreLoaded())
		{
			this.m_storePurchaseAuth.CompletePurchaseSuccess(null);
		}
		this.Status = StoreManager.TransactionStatus.READY;
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x000187CC File Offset: 0x000169CC
	private void ShowCompletedAchieve()
	{
		bool flag = this.m_completedAchieves.Count == 0;
		if (this.m_currentStoreType == StoreType.GENERAL_STORE)
		{
			GeneralStore generalStore = (GeneralStore)this.GetCurrentStore();
			if (generalStore != null)
			{
				generalStore.EnableClickCatcher(flag);
			}
		}
		if (flag)
		{
			return;
		}
		Achievement quest = this.m_completedAchieves[0];
		this.m_completedAchieves.RemoveAt(0);
		QuestToast.ShowQuestToast(UserAttentionBlocker.NONE, delegate(object userData)
		{
			this.ShowCompletedAchieve();
		}, true, quest, false);
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x00018848 File Offset: 0x00016A48
	private void OnPurchaseResultAcknowledged(bool success, MoneyOrGTAPPTransaction moneyOrGTAPPTransaction)
	{
		PaymentMethod paymentMethod;
		Network.Bundle bundle;
		if (moneyOrGTAPPTransaction == null)
		{
			paymentMethod = PaymentMethod.GOLD_NO_GTAPP;
			bundle = null;
		}
		else
		{
			if (moneyOrGTAPPTransaction.ID > 0L)
			{
				this.m_transactionIDsConclusivelyHandled.Add(moneyOrGTAPPTransaction.ID);
			}
			paymentMethod = ((!moneyOrGTAPPTransaction.IsGTAPP) ? PaymentMethod.MONEY : PaymentMethod.GOLD_GTAPP);
			bundle = this.GetBundle(moneyOrGTAPPTransaction.ProductID);
		}
		if (paymentMethod != PaymentMethod.GOLD_NO_GTAPP)
		{
			this.ConfirmActiveMoneyTransaction(moneyOrGTAPPTransaction.ID);
		}
		if (success)
		{
			StoreManager.SuccessfulPurchaseAckListener[] array = this.m_successfulPurchaseAckListeners.ToArray();
			foreach (StoreManager.SuccessfulPurchaseAckListener successfulPurchaseAckListener in array)
			{
				successfulPurchaseAckListener.Fire(bundle, paymentMethod);
			}
		}
		this.DeactivateStoreCover();
		Store currentStore = this.GetCurrentStore();
		if (this.m_currentStoreType == StoreType.ADVENTURE_STORE)
		{
			currentStore.Close();
		}
		if (this.BattlePayAvailable)
		{
			return;
		}
		if (this.m_currentStoreType == StoreType.GENERAL_STORE)
		{
			currentStore.Close();
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x00018934 File Offset: 0x00016B34
	private void OnAuthExit()
	{
		StoreManager.AuthorizationExitListener[] array = this.m_authExitListeners.ToArray();
		foreach (StoreManager.AuthorizationExitListener authorizationExitListener in array)
		{
			authorizationExitListener.Fire();
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00018970 File Offset: 0x00016B70
	private void ActivateStoreCover()
	{
		Store currentStore = this.GetCurrentStore();
		if (currentStore != null)
		{
			currentStore.ActivateCover(true);
		}
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00018998 File Offset: 0x00016B98
	private void DeactivateStoreCover()
	{
		Store currentStore = this.GetCurrentStore();
		if (currentStore != null)
		{
			currentStore.ActivateCover(false);
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x000189C0 File Offset: 0x00016BC0
	private void HandlePurchaseSuccess(StoreManager.PurchaseErrorSource? source, MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, string thirdPartyID)
	{
		this.NotifyMobileGamePurchaseResponse(thirdPartyID, true);
		this.Status = StoreManager.TransactionStatus.READY;
		PaymentMethod paymentMethod;
		Network.Bundle bundle;
		if (moneyOrGTAPPTransaction == null)
		{
			paymentMethod = PaymentMethod.GOLD_NO_GTAPP;
			bundle = null;
		}
		else
		{
			paymentMethod = ((!moneyOrGTAPPTransaction.IsGTAPP) ? PaymentMethod.MONEY : PaymentMethod.GOLD_GTAPP);
			bundle = this.GetBundle(moneyOrGTAPPTransaction.ProductID);
		}
		bool flag = false;
		bool flag2 = false;
		if (bundle != null)
		{
			foreach (Network.BundleItem bundleItem in bundle.Items)
			{
				ProductType product = bundleItem.Product;
				if (product != 5)
				{
					if (product == 6)
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
			}
		}
		if (paymentMethod != PaymentMethod.GOLD_NO_GTAPP && AchieveManager.Get() != null && AchieveManager.Get().HasIncompletePurchaseAchieves())
		{
			flag = true;
		}
		if (flag && AchieveManager.Get() != null)
		{
			StoreManager.StoreAchievesData userData = new StoreManager.StoreAchievesData(bundle, paymentMethod);
			AchieveManager.Get().UpdateActiveAchieves(new AchieveManager.ActiveAchievesUpdatedCallback(this.OnAchievesUpdated), userData);
		}
		if (flag2)
		{
			NetCache.Get().RefreshNetObject<NetCache.NetCacheFavoriteHeroes>();
		}
		StoreManager.SuccessfulPurchaseListener[] array = this.m_successfulPurchaseListeners.ToArray();
		foreach (StoreManager.SuccessfulPurchaseListener successfulPurchaseListener in array)
		{
			successfulPurchaseListener.Fire(bundle, paymentMethod);
		}
		if (!this.IsCurrentStoreLoaded())
		{
			return;
		}
		if (source != null && source.Value == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
		{
			this.ActivateStoreCover();
			this.m_storePurchaseAuth.ShowPreviousPurchaseSuccess(moneyOrGTAPPTransaction, this.ShouldEnablePurchaseAuthBackButton(this.m_currentStoreType));
			return;
		}
		Store currentStore = this.GetCurrentStore();
		if (paymentMethod == PaymentMethod.GOLD_GTAPP || paymentMethod == PaymentMethod.GOLD_NO_GTAPP)
		{
			currentStore.OnGoldSpent();
		}
		else
		{
			currentStore.OnMoneySpent();
		}
		this.m_storePurchaseAuth.CompletePurchaseSuccess(moneyOrGTAPPTransaction);
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x00018BA4 File Offset: 0x00016DA4
	private void HandleFailedRiskError(StoreManager.PurchaseErrorSource source)
	{
		bool flag = StoreManager.TransactionStatus.CHALLENGE_CANCELED == this.Status;
		this.Status = StoreManager.TransactionStatus.READY;
		if (flag)
		{
			Log.Rachelle.Print("HandleFailedRiskError for canceled transaction", new object[0]);
			if (this.m_activeMoneyOrGTAPPTransaction != null)
			{
				this.ConfirmActiveMoneyTransaction(this.m_activeMoneyOrGTAPPTransaction.ID);
			}
			this.DeactivateStoreCover();
			return;
		}
		if (!this.IsCurrentStoreLoaded())
		{
			return;
		}
		if (!this.GetCurrentStore().IsShown())
		{
			return;
		}
		this.m_storePurchaseAuth.Hide();
		this.ActivateStoreCover();
		this.m_storeSendToBAM.Show(this.m_activeMoneyOrGTAPPTransaction, StoreSendToBAM.BAMReason.NEED_PASSWORD_RESET, string.Empty, source == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE);
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00018C4C File Offset: 0x00016E4C
	private void HandleSendToBAMError(StoreManager.PurchaseErrorSource source, StoreSendToBAM.BAMReason reason, string errorCode)
	{
		this.Status = StoreManager.TransactionStatus.READY;
		if (!this.IsCurrentStoreLoaded())
		{
			return;
		}
		if (!this.GetCurrentStore().IsShown())
		{
			return;
		}
		this.m_storePurchaseAuth.Hide();
		this.ActivateStoreCover();
		this.m_storeSendToBAM.Show(this.m_activeMoneyOrGTAPPTransaction, reason, errorCode, source == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00018CA8 File Offset: 0x00016EA8
	public void NotifyMobileGamePurchaseResponse(string thirdPartyID, bool isSuccess)
	{
		Log.Yim.Print(string.Concat(new string[]
		{
			"NotifyMobileGamePurchaseResponse(",
			thirdPartyID,
			", ",
			(!isSuccess) ? "false" : "true",
			")"
		}), new object[0]);
		if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD && !string.IsNullOrEmpty(thirdPartyID))
		{
			string message = string.Concat(new string[]
			{
				(this.m_activeMoneyOrGTAPPTransaction != null) ? this.m_activeMoneyOrGTAPPTransaction.ID.ToString() : string.Empty,
				"|",
				thirdPartyID,
				"|",
				(!isSuccess) ? "false" : "true"
			});
			BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_INFO, BIReport.TelemetryEvent.EVENT_THIRD_PARTY_PURCHASE_RECEIPT_SUBMITTED_RESPONSE, 0, message);
			StoreMobilePurchase.GamePurchaseStatusResponse(thirdPartyID, isSuccess);
		}
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00018DA8 File Offset: 0x00016FA8
	private void CompletePurchaseFailure(StoreManager.PurchaseErrorSource source, MoneyOrGTAPPTransaction moneyOrGTAPPTransaction, string failDetails, string thirdPartyID, bool removeThirdPartyReceipt)
	{
		if (removeThirdPartyReceipt)
		{
			this.NotifyMobileGamePurchaseResponse(thirdPartyID, false);
		}
		if (!this.IsCurrentStoreLoaded())
		{
			return;
		}
		if (source == StoreManager.PurchaseErrorSource.FROM_PURCHASE_METHOD_RESPONSE)
		{
			this.ActivateStoreCover();
			this.m_storePurchaseAuth.ShowPurchaseMethodFailure(moneyOrGTAPPTransaction, failDetails, this.ShouldEnablePurchaseAuthBackButton(this.m_currentStoreType));
			return;
		}
		if (source == StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
		{
			this.ActivateStoreCover();
			this.m_storePurchaseAuth.ShowPreviousPurchaseFailure(moneyOrGTAPPTransaction, failDetails, this.ShouldEnablePurchaseAuthBackButton(this.m_currentStoreType));
			return;
		}
		if (this.m_storePurchaseAuth.CompletePurchaseFailure(moneyOrGTAPPTransaction, failDetails))
		{
			return;
		}
		Debug.LogWarning(string.Format("StoreManager.CompletePurchaseFailure(): purchased failed ({0}) but the store authorization window has been closed.", failDetails));
		this.DeactivateStoreCover();
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00018E48 File Offset: 0x00017048
	private bool ReloadNoticesIfNecessary(Network.PurchaseErrorInfo.ErrorType errorType)
	{
		if (this.m_activeMoneyOrGTAPPTransaction != null && this.m_transactionIDsThatReloadedNotices.Contains(this.m_activeMoneyOrGTAPPTransaction.ID))
		{
			return false;
		}
		if (!this.IsConclusiveState(errorType))
		{
			return false;
		}
		if (this.m_activeMoneyOrGTAPPTransaction != null)
		{
			this.m_transactionIDsThatReloadedNotices.Add(this.m_activeMoneyOrGTAPPTransaction.ID);
		}
		Log.Rachelle.Print("StoreManager.ReloadNoticesIfNecessary: errorType = {0}, RELOADING", new object[]
		{
			errorType
		});
		NetCache.Get().ReloadNetObject<NetCache.NetCacheProfileNotices>();
		return true;
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00018ED8 File Offset: 0x000170D8
	private void HandlePurchaseError(StoreManager.PurchaseErrorSource source, Network.PurchaseErrorInfo.ErrorType purchaseErrorType, string purchaseErrorCode, string thirdPartyID, bool isGTAPP)
	{
		if (this.IsConclusiveState(purchaseErrorType) && this.m_activeMoneyOrGTAPPTransaction != null && this.m_transactionIDsConclusivelyHandled.Contains(this.m_activeMoneyOrGTAPPTransaction.ID))
		{
			Log.Rachelle.Print("HandlePurchaseError already handled purchase error for conclusive state on transaction (Transaction: {0}, current purchaseErrorType = {1})", new object[]
			{
				this.m_activeMoneyOrGTAPPTransaction,
				purchaseErrorType
			});
			return;
		}
		Log.Yim.Print(string.Format("HandlePurchaseError source={0} purchaseErrorType={1} purchaseErrorCode={2} thirdPartyID={3}", new object[]
		{
			source,
			purchaseErrorType,
			purchaseErrorCode,
			thirdPartyID
		}), new object[0]);
		string failDetails = string.Empty;
		if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE && this.ReloadNoticesIfNecessary(purchaseErrorType) && this.Status == StoreManager.TransactionStatus.UNKNOWN && source == StoreManager.PurchaseErrorSource.FROM_STATUS_OR_PURCHASE_RESPONSE)
		{
			Log.Rachelle.Print("StoreManager.HandPurchaseError(): force reloading notices for very first status", new object[0]);
			this.NoticesReady = false;
		}
		switch (purchaseErrorType + 1)
		{
		case Network.PurchaseErrorInfo.ErrorType.SUCCESS:
			Debug.LogWarning("StoreManager.HandlePurchaseError: purchase error is UNKNOWN, taking no action on this purchase");
			return;
		case Network.PurchaseErrorInfo.ErrorType.STILL_IN_PROGRESS:
			if (source == StoreManager.PurchaseErrorSource.FROM_PURCHASE_METHOD_RESPONSE)
			{
				Debug.LogWarning("StoreManager.HandlePurchaseError: received SUCCESS from payment method purchase error.");
			}
			else
			{
				if (isGTAPP)
				{
					NetCache.Get().RefreshNetObject<NetCache.NetCacheGoldBalance>();
				}
				this.HandlePurchaseSuccess(new StoreManager.PurchaseErrorSource?(source), this.m_activeMoneyOrGTAPPTransaction, thirdPartyID);
			}
			return;
		case Network.PurchaseErrorInfo.ErrorType.INVALID_BNET:
			if (source == StoreManager.PurchaseErrorSource.FROM_PURCHASE_METHOD_RESPONSE)
			{
				Debug.LogWarning("StoreManager.HandlePurchaseError: received STILL_IN_PROGRESS from payment method purchase error.");
			}
			else if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
			{
				this.Status = ((!isGTAPP) ? StoreManager.TransactionStatus.IN_PROGRESS_MONEY : StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP);
			}
			return;
		case Network.PurchaseErrorInfo.ErrorType.SERVICE_NA:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_BNET_ID");
			break;
		case Network.PurchaseErrorInfo.ErrorType.PURCHASE_IN_PROGRESS:
			if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
			{
				if (this.Status != StoreManager.TransactionStatus.UNKNOWN)
				{
					this.BattlePayAvailable = false;
				}
				this.Status = StoreManager.TransactionStatus.UNKNOWN;
			}
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_NO_BATTLEPAY");
			this.CompletePurchaseFailure(source, this.m_activeMoneyOrGTAPPTransaction, failDetails, thirdPartyID, this.IsSafeToRemoveMobileReceipt(purchaseErrorType));
			return;
		case Network.PurchaseErrorInfo.ErrorType.DATABASE:
			if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
			{
				this.Status = ((!isGTAPP) ? StoreManager.TransactionStatus.IN_PROGRESS_MONEY : StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP);
			}
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_IN_PROGRESS");
			this.CompletePurchaseFailure(source, this.m_activeMoneyOrGTAPPTransaction, failDetails, thirdPartyID, this.IsSafeToRemoveMobileReceipt(purchaseErrorType));
			return;
		case Network.PurchaseErrorInfo.ErrorType.INVALID_QUANTITY:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_DATABASE");
			break;
		case Network.PurchaseErrorInfo.ErrorType.DUPLICATE_LICENSE:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_QUANTITY");
			break;
		case Network.PurchaseErrorInfo.ErrorType.REQUEST_NOT_SENT:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_LICENSE");
			break;
		case Network.PurchaseErrorInfo.ErrorType.NO_ACTIVE_BPAY:
			if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE && this.Status != StoreManager.TransactionStatus.UNKNOWN)
			{
				this.BattlePayAvailable = false;
			}
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_NO_BATTLEPAY");
			break;
		case Network.PurchaseErrorInfo.ErrorType.FAILED_RISK:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_NO_ACTIVE_BPAY");
			break;
		case Network.PurchaseErrorInfo.ErrorType.CANCELED:
			this.HandleFailedRiskError(source);
			return;
		case Network.PurchaseErrorInfo.ErrorType.WAIT_MOP:
			if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
			{
				this.Status = StoreManager.TransactionStatus.READY;
			}
			return;
		case Network.PurchaseErrorInfo.ErrorType.WAIT_CONFIRM:
			Log.Rachelle.Print("StoreManager.HandlePurchaseError: Status is WAIT_MOP.. this probably shouldn't be happening.", new object[0]);
			if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
			{
				if (this.Status == StoreManager.TransactionStatus.UNKNOWN)
				{
					Log.Rachelle.Print(string.Format("StoreManager.HandlePurchaseError: Status is WAIT_MOP, previous Status was UNKNOWN, source = {0}", source), new object[0]);
				}
				else
				{
					this.Status = StoreManager.TransactionStatus.WAIT_METHOD_OF_PAYMENT;
				}
			}
			return;
		case Network.PurchaseErrorInfo.ErrorType.WAIT_RISK:
			if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE && this.Status == StoreManager.TransactionStatus.UNKNOWN)
			{
				Log.Rachelle.Print(string.Format("StoreManager.HandlePurchaseError: Status is WAIT_CONFIRM, previous Status was UNKNOWN, source = {0}. Going to try to cancel the purchase.", source), new object[0]);
				this.CancelBlizzardPurchase(default(CancelPurchase.CancelReason?), null);
			}
			return;
		case Network.PurchaseErrorInfo.ErrorType.PRODUCT_NA:
			if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
			{
				Log.Rachelle.Print("StoreManager.HandlePurchaseError: Waiting for client to respond to Risk challenge", new object[0]);
				if (this.Status == StoreManager.TransactionStatus.UNKNOWN)
				{
					Log.Rachelle.Print(string.Format("StoreManager.HandlePurchaseError: Status is WAIT_RISK, previous Status was UNKNOWN, source = {0}", source), new object[0]);
				}
				else if (this.Status == StoreManager.TransactionStatus.CHALLENGE_SUBMITTED || this.Status == StoreManager.TransactionStatus.CHALLENGE_CANCELED)
				{
					Log.Rachelle.Print(string.Format("StoreManager.HandlePurchaseError: Status = {0}; ignoring WAIT_RISK purchase error info", this.Status), new object[0]);
				}
				else
				{
					this.Status = StoreManager.TransactionStatus.WAIT_RISK;
				}
			}
			return;
		case Network.PurchaseErrorInfo.ErrorType.RISK_TIMEOUT:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_PRODUCT_NA");
			break;
		case Network.PurchaseErrorInfo.ErrorType.PRODUCT_ALREADY_OWNED:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_CHALLENGE_TIMEOUT");
			break;
		case Network.PurchaseErrorInfo.ErrorType.WAIT_THIRD_PARTY_RECEIPT:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_PRODUCT_ALREADY_OWNED");
			break;
		case Network.PurchaseErrorInfo.ErrorType.PRODUCT_EVENT_HAS_ENDED:
			this.OnThirdPartyPurchaseApproved();
			return;
		case (Network.PurchaseErrorInfo.ErrorType)20:
			if (this.m_activeMoneyOrGTAPPTransaction != null && this.IsProductPrePurchase(this.GetBundle(this.m_activeMoneyOrGTAPPTransaction.ProductID)))
			{
				failDetails = GameStrings.Get("GLUE_STORE_PRE_PURCHASE_HAS_ENDED");
			}
			else
			{
				failDetails = GameStrings.Get("GLUE_STORE_PRODUCT_EVENT_HAS_ENDED");
			}
			break;
		default:
			switch (purchaseErrorType)
			{
			case Network.PurchaseErrorInfo.ErrorType.BP_GENERIC_FAIL:
			case Network.PurchaseErrorInfo.ErrorType.BP_RISK_ERROR:
			case Network.PurchaseErrorInfo.ErrorType.BP_PAYMENT_AUTH:
			case Network.PurchaseErrorInfo.ErrorType.BP_PROVIDER_DENIED:
			case Network.PurchaseErrorInfo.ErrorType.E_BP_GENERIC_FAIL_RETRY_CONTACT_CS_IF_PERSISTS:
				if (!isGTAPP)
				{
					StoreSendToBAM.BAMReason reason = StoreSendToBAM.BAMReason.GENERIC_PAYMENT_FAIL;
					if (purchaseErrorType == Network.PurchaseErrorInfo.ErrorType.E_BP_GENERIC_FAIL_RETRY_CONTACT_CS_IF_PERSISTS)
					{
						reason = StoreSendToBAM.BAMReason.GENERIC_PURCHASE_FAIL_RETRY_CONTACT_CS_IF_PERSISTS;
					}
					this.HandleSendToBAMError(source, reason, purchaseErrorCode);
					if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD)
					{
						this.CompletePurchaseFailure(source, this.m_activeMoneyOrGTAPPTransaction, failDetails, thirdPartyID, this.IsSafeToRemoveMobileReceipt(purchaseErrorType));
					}
					return;
				}
				failDetails = GameStrings.Get("GLUE_STORE_FAIL_GOLD_GENERIC");
				goto IL_625;
			case Network.PurchaseErrorInfo.ErrorType.BP_INVALID_CC_EXPIRY:
				if (!isGTAPP)
				{
					this.HandleSendToBAMError(source, StoreSendToBAM.BAMReason.CREDIT_CARD_EXPIRED, string.Empty);
					return;
				}
				failDetails = GameStrings.Get("GLUE_STORE_FAIL_GOLD_GENERIC");
				goto IL_625;
			case Network.PurchaseErrorInfo.ErrorType.BP_NO_VALID_PAYMENT:
				if (source == StoreManager.PurchaseErrorSource.FROM_PURCHASE_METHOD_RESPONSE)
				{
					Debug.LogWarning("StoreManager.HandlePurchaseError: received BP_NO_VALID_PAYMENT from payment method purchase error.");
				}
				else
				{
					if (!isGTAPP)
					{
						this.HandleSendToBAMError(source, StoreSendToBAM.BAMReason.NO_VALID_PAYMENT_METHOD, string.Empty);
						return;
					}
					failDetails = GameStrings.Get("GLUE_STORE_FAIL_GOLD_GENERIC");
				}
				goto IL_625;
			case Network.PurchaseErrorInfo.ErrorType.BP_PURCHASE_BAN:
				failDetails = GameStrings.Get("GLUE_STORE_FAIL_PURCHASE_BAN");
				goto IL_625;
			case Network.PurchaseErrorInfo.ErrorType.BP_SPENDING_LIMIT:
				if (!isGTAPP)
				{
					failDetails = GameStrings.Get("GLUE_STORE_FAIL_SPENDING_LIMIT");
				}
				else
				{
					failDetails = GameStrings.Get("GLUE_STORE_FAIL_GOLD_GENERIC");
				}
				goto IL_625;
			case Network.PurchaseErrorInfo.ErrorType.BP_PARENTAL_CONTROL:
				failDetails = GameStrings.Get("GLUE_STORE_FAIL_PARENTAL_CONTROL");
				goto IL_625;
			case Network.PurchaseErrorInfo.ErrorType.BP_THROTTLED:
				failDetails = GameStrings.Get("GLUE_STORE_FAIL_THROTTLED");
				goto IL_625;
			case Network.PurchaseErrorInfo.ErrorType.BP_THIRD_PARTY_BAD_RECEIPT:
			case Network.PurchaseErrorInfo.ErrorType.BP_THIRD_PARTY_RECEIPT_USED:
				failDetails = GameStrings.Get("GLUE_STORE_FAIL_THIRD_PARTY_BAD_RECEIPT");
				goto IL_625;
			case Network.PurchaseErrorInfo.ErrorType.BP_PRODUCT_UNIQUENESS_VIOLATED:
				this.HandleSendToBAMError(source, StoreSendToBAM.BAMReason.PRODUCT_UNIQUENESS_VIOLATED, string.Empty);
				return;
			case Network.PurchaseErrorInfo.ErrorType.BP_REGION_IS_DOWN:
				failDetails = GameStrings.Get("GLUE_STORE_FAIL_REGION_IS_DOWN");
				goto IL_625;
			case Network.PurchaseErrorInfo.ErrorType.E_BP_CHALLENGE_ID_FAILED_VERIFICATION:
				failDetails = GameStrings.Get("GLUE_STORE_FAIL_CHALLENGE_ID_FAILED_VERIFICATION");
				goto IL_625;
			}
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_GENERAL");
			break;
		}
		IL_625:
		if (source != StoreManager.PurchaseErrorSource.FROM_PREVIOUS_PURCHASE)
		{
			bool flag = StoreManager.TransactionStatus.UNKNOWN == this.Status;
			this.Status = StoreManager.TransactionStatus.READY;
			if (flag && this.IsSafeToRemoveMobileReceipt(purchaseErrorType))
			{
				this.NotifyMobileGamePurchaseResponse(thirdPartyID, false);
				return;
			}
		}
		this.CompletePurchaseFailure(source, this.m_activeMoneyOrGTAPPTransaction, failDetails, thirdPartyID, this.IsSafeToRemoveMobileReceipt(purchaseErrorType));
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00019558 File Offset: 0x00017758
	private void SetActiveMoneyOrGTAPPTransaction(long id, string productID, BattlePayProvider? provider, bool isGTAPP, bool tryToResolvePreviousTransactionNotices)
	{
		MoneyOrGTAPPTransaction moneyOrGTAPPTransaction = new MoneyOrGTAPPTransaction(id, productID, provider, isGTAPP);
		bool flag = true;
		if (this.m_activeMoneyOrGTAPPTransaction != null)
		{
			if (moneyOrGTAPPTransaction.Equals(this.m_activeMoneyOrGTAPPTransaction))
			{
				flag = (this.m_activeMoneyOrGTAPPTransaction.Provider == null && provider != null);
			}
			else if ((long)StoreManager.UNKNOWN_TRANSACTION_ID != this.m_activeMoneyOrGTAPPTransaction.ID)
			{
				Debug.LogWarning(string.Format("StoreManager.SetActiveMoneyOrGTAPPTransaction(id={0},productID='{1}',isGTAPP={2},provider={3}) does not match active money or GTAPP transaction '{4}'", new object[]
				{
					id,
					productID,
					isGTAPP,
					(provider == null) ? "UNKNOWN" : provider.Value.ToString(),
					this.m_activeMoneyOrGTAPPTransaction
				}));
			}
		}
		if (flag)
		{
			Log.Rachelle.Print(string.Format("SetActiveMoneyOrGTAPPTransaction() {0}", moneyOrGTAPPTransaction), new object[0]);
			Log.Yim.Print(string.Format("SetActiveMoneyOrGTAPPTransaction() {0}", moneyOrGTAPPTransaction), new object[0]);
			this.m_activeMoneyOrGTAPPTransaction = moneyOrGTAPPTransaction;
		}
		if (this.m_firstMoneyOrGTAPPTransactionSet)
		{
			return;
		}
		this.m_firstMoneyOrGTAPPTransactionSet = true;
		if (tryToResolvePreviousTransactionNotices)
		{
			this.ResolveFirstMoneyOrGTAPPTransactionIfPossible();
		}
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00019690 File Offset: 0x00017890
	private void ResolveFirstMoneyOrGTAPPTransactionIfPossible()
	{
		if (!this.m_firstMoneyOrGTAPPTransactionSet)
		{
			return;
		}
		if (!this.NoticesReady)
		{
			return;
		}
		if (this.m_activeMoneyOrGTAPPTransaction == null)
		{
			return;
		}
		NetCache.ProfileNoticePurchase profileNoticePurchase = this.m_outstandingPurchaseNotices.Find((NetCache.ProfileNoticePurchase obj) => obj.OriginData == this.m_activeMoneyOrGTAPPTransaction.ID);
		if (profileNoticePurchase != null)
		{
			return;
		}
		Log.Rachelle.Print(string.Format("StoreManager.ResolveFirstMoneyTransactionIfPossible(): no outstanding notices for transaction {0}; setting m_activeMoneyOrGTAPPTransaction = null", this.m_activeMoneyOrGTAPPTransaction), new object[0]);
		this.m_activeMoneyOrGTAPPTransaction = null;
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00019708 File Offset: 0x00017908
	private void ConfirmActiveMoneyTransaction(long id)
	{
		if (this.m_activeMoneyOrGTAPPTransaction == null || (this.m_activeMoneyOrGTAPPTransaction.ID != (long)StoreManager.UNKNOWN_TRANSACTION_ID && this.m_activeMoneyOrGTAPPTransaction.ID != id))
		{
			Debug.LogWarning(string.Format("StoreManager.ConfirmActiveMoneyTransaction(id={0}) does not match active money transaction '{1}'", id, this.m_activeMoneyOrGTAPPTransaction));
		}
		Log.Rachelle.Print(string.Format("ConfirmActiveMoneyTransaction() {0}", id), new object[0]);
		Predicate<NetCache.ProfileNoticePurchase> predicate = (NetCache.ProfileNoticePurchase obj) => obj.OriginData == id;
		List<NetCache.ProfileNoticePurchase> list = this.m_outstandingPurchaseNotices.FindAll(predicate);
		this.m_outstandingPurchaseNotices.RemoveAll(predicate);
		foreach (NetCache.ProfileNoticePurchase profileNoticePurchase in list)
		{
			Network.AckNotice(profileNoticePurchase.NoticeID);
		}
		this.m_confirmedTransactionIDs.Add(id);
		this.m_activeMoneyOrGTAPPTransaction = null;
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00019834 File Offset: 0x00017A34
	private void OnNewNotices(List<NetCache.ProfileNotice> newNotices)
	{
		List<long> list = new List<long>();
		foreach (NetCache.ProfileNotice profileNotice in newNotices)
		{
			if (profileNotice.Type == NetCache.ProfileNotice.NoticeType.PURCHASE)
			{
				if (profileNotice.Origin == NetCache.ProfileNotice.NoticeOrigin.PURCHASE_CANCELED)
				{
					Log.Rachelle.Print(string.Format("StoreManager.OnNewNotices() ack'ing purchase canceled notice for bpay ID {0}", profileNotice.OriginData), new object[0]);
					list.Add(profileNotice.NoticeID);
				}
				else if (this.m_confirmedTransactionIDs.Contains(profileNotice.OriginData))
				{
					Log.Rachelle.Print(string.Format("StoreManager.OnNewNotices() ack'ing purchase notice for already confirmed bpay ID {0}", profileNotice.OriginData), new object[0]);
					list.Add(profileNotice.NoticeID);
				}
				else
				{
					NetCache.ProfileNoticePurchase profileNoticePurchase = profileNotice as NetCache.ProfileNoticePurchase;
					Log.Rachelle.Print(string.Format("StoreManager.OnNewNotices() adding outstanding purchase notice for bpay ID {0}", profileNotice.OriginData), new object[0]);
					this.m_outstandingPurchaseNotices.Add(profileNoticePurchase);
				}
			}
		}
		foreach (long id in list)
		{
			Network.AckNotice(id);
		}
		if (this.NoticesReady)
		{
			return;
		}
		this.NoticesReady = true;
		if (this.Status == StoreManager.TransactionStatus.READY)
		{
			this.ResolveFirstMoneyOrGTAPPTransactionIfPossible();
		}
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x000199D0 File Offset: 0x00017BD0
	private void RemoveThirdPartyReceipt(string transactionID)
	{
		if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD)
		{
			if (string.IsNullOrEmpty(transactionID))
			{
				return;
			}
			StoreMobilePurchase.FinishTransactionForId(transactionID);
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x00019A0C File Offset: 0x00017C0C
	private void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance)
	{
		Store currentStore = this.GetCurrentStore();
		if (currentStore == null)
		{
			return;
		}
		if (!currentStore.IsShown())
		{
			return;
		}
		currentStore.OnGoldBalanceChanged(balance);
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00019A40 File Offset: 0x00017C40
	private void OnNetCacheFeaturesReady()
	{
		this.FeaturesReady = true;
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00019A4C File Offset: 0x00017C4C
	private void OnPurchaseCanceledResponse()
	{
		Network.PurchaseCanceledResponse purchaseCanceledResponse = Network.GetPurchaseCanceledResponse();
		switch (purchaseCanceledResponse.Result)
		{
		case Network.PurchaseCanceledResponse.CancelResult.SUCCESS:
		{
			Log.Rachelle.Print("StoreManager.OnPurchaseCanceledResponse(): purchase successfully canceled.", new object[0]);
			string message = (this.m_activeMoneyOrGTAPPTransaction != null) ? this.m_activeMoneyOrGTAPPTransaction.ID.ToString() : string.Empty;
			BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_INFO, BIReport.TelemetryEvent.EVENT_THIRD_PARTY_PURCHASE_CANCEL_RESPONSE, 0, message);
			NetCache.Get().ReloadNetObject<NetCache.NetCacheProfileNotices>();
			this.m_lastStatusRequestTime = DateTime.Now.Ticks;
			this.ConfirmActiveMoneyTransaction(purchaseCanceledResponse.TransactionID);
			this.Status = StoreManager.TransactionStatus.READY;
			this.m_shouldAutoCancelThirdPartyTransaction = false;
			this.m_requestedThirdPartyProductId = null;
			this.m_previousStatusBeforeAutoCancel = StoreManager.TransactionStatus.UNKNOWN;
			this.m_outOfSessionThirdPartyTransaction = false;
			break;
		}
		case Network.PurchaseCanceledResponse.CancelResult.NOT_ALLOWED:
		{
			Debug.LogWarning("StoreManager.OnPurchaseCanceledResponse(): cancel purchase is not allowed right now.");
			string message2 = (this.m_activeMoneyOrGTAPPTransaction != null) ? this.m_activeMoneyOrGTAPPTransaction.ID.ToString() : string.Empty;
			BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_INFO, BIReport.TelemetryEvent.EVENT_THIRD_PARTY_PURCHASE_CANCEL_RESPONSE, 1, message2);
			bool flag = Currency.XSG == purchaseCanceledResponse.CurrencyType;
			this.SetActiveMoneyOrGTAPPTransaction(purchaseCanceledResponse.TransactionID, purchaseCanceledResponse.ProductID, MoneyOrGTAPPTransaction.UNKNOWN_PROVIDER, flag, true);
			this.Status = ((!flag) ? StoreManager.TransactionStatus.IN_PROGRESS_MONEY : StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP);
			if (this.m_previousStatusBeforeAutoCancel != StoreManager.TransactionStatus.UNKNOWN)
			{
				this.Status = this.m_previousStatusBeforeAutoCancel;
				this.m_previousStatusBeforeAutoCancel = StoreManager.TransactionStatus.UNKNOWN;
			}
			break;
		}
		case Network.PurchaseCanceledResponse.CancelResult.NOTHING_TO_CANCEL:
		{
			string message3 = (this.m_activeMoneyOrGTAPPTransaction != null) ? this.m_activeMoneyOrGTAPPTransaction.ID.ToString() : string.Empty;
			BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_INFO, BIReport.TelemetryEvent.EVENT_THIRD_PARTY_PURCHASE_CANCEL_RESPONSE, 2, message3);
			if (this.m_activeMoneyOrGTAPPTransaction == null)
			{
				Debug.LogWarning("StoreManager.OnPurchaseCanceledResponse(): nothing to cancel and m_activeMoneyOrGTAPPTransaction is null.. no choice but to set Status to UNKNOWN");
				this.Status = StoreManager.TransactionStatus.UNKNOWN;
			}
			else
			{
				StoreManager.TransactionStatus transactionStatus = (!this.m_activeMoneyOrGTAPPTransaction.IsGTAPP) ? StoreManager.TransactionStatus.IN_PROGRESS_MONEY : StoreManager.TransactionStatus.IN_PROGRESS_GOLD_GTAPP;
				if (this.m_previousStatusBeforeAutoCancel != StoreManager.TransactionStatus.UNKNOWN)
				{
					transactionStatus = this.m_previousStatusBeforeAutoCancel;
					this.m_previousStatusBeforeAutoCancel = StoreManager.TransactionStatus.UNKNOWN;
				}
				Debug.LogWarning(string.Format("StoreManager.OnPurchaseCanceledResponse(): nothing to cancel, setting status to {0}", transactionStatus));
				this.Status = transactionStatus;
			}
			break;
		}
		}
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00019C78 File Offset: 0x00017E78
	private bool IsConclusiveState(Network.PurchaseErrorInfo.ErrorType errorType)
	{
		switch (errorType)
		{
		case Network.PurchaseErrorInfo.ErrorType.WAIT_MOP:
		case Network.PurchaseErrorInfo.ErrorType.WAIT_CONFIRM:
		case Network.PurchaseErrorInfo.ErrorType.WAIT_RISK:
		case Network.PurchaseErrorInfo.ErrorType.WAIT_THIRD_PARTY_RECEIPT:
			break;
		default:
			switch (errorType + 1)
			{
			case Network.PurchaseErrorInfo.ErrorType.SUCCESS:
			case Network.PurchaseErrorInfo.ErrorType.INVALID_BNET:
				return false;
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00019CC8 File Offset: 0x00017EC8
	private bool IsSafeToRemoveMobileReceipt(Network.PurchaseErrorInfo.ErrorType errorType)
	{
		return this.IsConclusiveState(errorType) && (errorType == Network.PurchaseErrorInfo.ErrorType.BP_THIRD_PARTY_BAD_RECEIPT || errorType == Network.PurchaseErrorInfo.ErrorType.BP_THIRD_PARTY_RECEIPT_USED || errorType == Network.PurchaseErrorInfo.ErrorType.SUCCESS);
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00019D04 File Offset: 0x00017F04
	private void OnBattlePayStatusResponse()
	{
		Network.BattlePayStatus battlePayStatusResponse = Network.GetBattlePayStatusResponse();
		this.BattlePayAvailable = battlePayStatusResponse.BattlePayAvailable;
		bool flag;
		switch (battlePayStatusResponse.State)
		{
		case Network.BattlePayStatus.PurchaseState.READY:
			this.Status = StoreManager.TransactionStatus.READY;
			flag = true;
			break;
		case Network.BattlePayStatus.PurchaseState.CHECK_RESULTS:
		{
			bool isGTAPP = Currency.XSG == battlePayStatusResponse.CurrencyType;
			bool tryToResolvePreviousTransactionNotices = this.IsConclusiveState(battlePayStatusResponse.PurchaseError.Error);
			this.SetActiveMoneyOrGTAPPTransaction(battlePayStatusResponse.TransactionID, battlePayStatusResponse.ProductID, battlePayStatusResponse.Provider, isGTAPP, tryToResolvePreviousTransactionNotices);
			this.HandlePurchaseError(StoreManager.PurchaseErrorSource.FROM_STATUS_OR_PURCHASE_RESPONSE, battlePayStatusResponse.PurchaseError.Error, battlePayStatusResponse.PurchaseError.ErrorCode, battlePayStatusResponse.ThirdPartyID, isGTAPP);
			flag = (battlePayStatusResponse.PurchaseError.Error != Network.PurchaseErrorInfo.ErrorType.STILL_IN_PROGRESS);
			break;
		}
		case Network.BattlePayStatus.PurchaseState.ERROR:
			Debug.LogWarning("StoreManager.OnBattlePayStatusResponse(): Error getting status. Check with Rachelle.");
			flag = false;
			break;
		default:
			Debug.LogError(string.Format("StoreManager.OnBattlePayStatusResponse(): unknown state {0}", battlePayStatusResponse.State));
			flag = false;
			break;
		}
		if (!this.BattlePayAvailable || !flag)
		{
			return;
		}
		this.m_statusRequestDelayTicks = StoreManager.MIN_STATUS_REQUEST_DELAY_TICKS;
		Log.Rachelle.Print(string.Format("StoreManager reset STATUS delay, now waiting {0} seconds between requests", this.m_statusRequestDelayTicks / 10000000L), new object[0]);
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00019E40 File Offset: 0x00018040
	private void OnBattlePayConfigResponse()
	{
		Network.BattlePayConfig battlePayConfigResponse = Network.GetBattlePayConfigResponse();
		if (!battlePayConfigResponse.Available)
		{
			this.BattlePayAvailable = false;
			return;
		}
		this.m_configRequestDelayTicks = StoreManager.MIN_CONFIG_REQUEST_DELAY_TICKS;
		Log.Rachelle.Print(string.Format("StoreManager reset CONFIG delay, now waiting {0} seconds between requests", this.m_configRequestDelayTicks / 10000000L), new object[0]);
		this.BattlePayAvailable = true;
		this.m_currency = battlePayConfigResponse.Currency;
		this.m_ticksBeforeAutoCancel = (long)battlePayConfigResponse.SecondsBeforeAutoCancel * 10000000L;
		this.m_ticksBeforeAutoCancelThirdParty = (long)StoreManager.DEFAULT_SECONDS_BEFORE_AUTO_CANCEL_THIRD_PARTY * 10000000L;
		this.m_bundles.Clear();
		if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD)
		{
			StoreMobilePurchase.ClearProductList();
		}
		foreach (Network.Bundle bundle in battlePayConfigResponse.Bundles)
		{
			if (bundle.ExclusiveProviders == null || bundle.ExclusiveProviders.Count <= 0 || bundle.ExclusiveProviders.Contains(StoreManager.StoreProvider))
			{
				if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD)
				{
					string empty = string.Empty;
					if (!string.IsNullOrEmpty(empty))
					{
						Log.Yim.Print("bundle.ProductId=" + bundle.ProductID + " thirdPartyID=" + empty, new object[0]);
						StoreMobilePurchase.AddProductById(empty);
						this.m_bundles.Add(bundle.ProductID, bundle);
					}
				}
				else
				{
					this.m_bundles.Add(bundle.ProductID, bundle);
				}
			}
		}
		this.m_goldCostBooster.Clear();
		foreach (Network.GoldCostBooster goldCostBooster in battlePayConfigResponse.GoldCostBoosters)
		{
			this.m_goldCostBooster.Add(goldCostBooster.ID, goldCostBooster);
		}
		this.m_goldCostArena = battlePayConfigResponse.GoldCostArena;
		if (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD)
		{
			StoreMobilePurchase.ValidateAllProducts(MobileCallbackManager.Get().name);
		}
		if (!StoreManager.HAS_THIRD_PARTY_APP_STORE || ApplicationMgr.GetAndroidStore() == AndroidStore.BLIZZARD)
		{
			this.ConfigLoaded = true;
		}
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x0001A0B8 File Offset: 0x000182B8
	private void HandleZeroCostLicensePurchaseMethod(Network.PurchaseMethod method)
	{
		if (method.PurchaseError.Error != Network.PurchaseErrorInfo.ErrorType.STILL_IN_PROGRESS)
		{
			Debug.LogWarning(string.Format("StoreManager.HandleZeroCostLicensePurchaseMethod() FAILED error={0}", method.PurchaseError.Error));
			this.Status = StoreManager.TransactionStatus.READY;
			return;
		}
		Log.Rachelle.Print("StoreManager.HandleZeroCostLicensePurchaseMethod succeeded, refreshing achieves", new object[0]);
		AchieveManager.Get().UpdateActiveAchieves(null);
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x0001A120 File Offset: 0x00018320
	private void OnPurchaseMethod()
	{
		Network.PurchaseMethod purchaseMethodResponse = Network.GetPurchaseMethodResponse();
		if (purchaseMethodResponse.IsZeroCostLicense)
		{
			this.HandleZeroCostLicensePurchaseMethod(purchaseMethodResponse);
			return;
		}
		if (!string.IsNullOrEmpty(purchaseMethodResponse.ChallengeID) && !string.IsNullOrEmpty(purchaseMethodResponse.ChallengeURL))
		{
			this.m_challengePurchaseMethod = purchaseMethodResponse;
		}
		else
		{
			this.m_challengePurchaseMethod = null;
		}
		bool flag = Currency.XSG == purchaseMethodResponse.Currency;
		this.SetActiveMoneyOrGTAPPTransaction(purchaseMethodResponse.TransactionID, purchaseMethodResponse.ProductID, new BattlePayProvider?(1), flag, false);
		bool flag2 = false;
		if (purchaseMethodResponse.PurchaseError != null)
		{
			if (purchaseMethodResponse.PurchaseError.Error != Network.PurchaseErrorInfo.ErrorType.BP_NO_VALID_PAYMENT)
			{
				this.HandlePurchaseError(StoreManager.PurchaseErrorSource.FROM_PURCHASE_METHOD_RESPONSE, purchaseMethodResponse.PurchaseError.Error, purchaseMethodResponse.PurchaseError.ErrorCode, string.Empty, flag);
				return;
			}
			flag2 = true;
		}
		this.ActivateStoreCover();
		if (flag)
		{
			this.OnSummaryConfirm(purchaseMethodResponse.ProductID, purchaseMethodResponse.Quantity, null);
			return;
		}
		string paymentMethodName;
		if (flag2)
		{
			paymentMethodName = null;
		}
		else if (purchaseMethodResponse.UseEBalance)
		{
			paymentMethodName = GameStrings.Get("GLUE_STORE_BNET_BALANCE");
		}
		else
		{
			paymentMethodName = purchaseMethodResponse.WalletName;
		}
		Store currentStore = this.GetCurrentStore();
		if (currentStore == null || !currentStore.IsShown())
		{
			this.AutoCancelPurchaseIfPossible();
			return;
		}
		if (this.m_storePurchaseAuth != null)
		{
			this.m_storePurchaseAuth.Hide();
		}
		this.Status = StoreManager.TransactionStatus.WAIT_CONFIRM;
		this.m_storeSummary.Show(purchaseMethodResponse.ProductID, purchaseMethodResponse.Quantity, paymentMethodName);
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x0001A29C File Offset: 0x0001849C
	private void OnPurchaseResponse()
	{
		Network.PurchaseResponse purchaseResponse = Network.GetPurchaseResponse();
		bool isGTAPP = Currency.XSG == purchaseResponse.CurrencyType;
		this.SetActiveMoneyOrGTAPPTransaction(purchaseResponse.TransactionID, purchaseResponse.ProductID, MoneyOrGTAPPTransaction.UNKNOWN_PROVIDER, isGTAPP, false);
		this.HandlePurchaseError(StoreManager.PurchaseErrorSource.FROM_STATUS_OR_PURCHASE_RESPONSE, purchaseResponse.PurchaseError.Error, purchaseResponse.PurchaseError.ErrorCode, purchaseResponse.ThirdPartyID, isGTAPP);
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0001A2F8 File Offset: 0x000184F8
	private void OnPurchaseViaGoldResponse()
	{
		Network.PurchaseViaGoldResponse purchaseWithGoldResponse = Network.GetPurchaseWithGoldResponse();
		string failDetails = string.Empty;
		switch (purchaseWithGoldResponse.Error)
		{
		case Network.PurchaseViaGoldResponse.ErrorType.SUCCESS:
			NetCache.Get().RefreshNetObject<NetCache.NetCacheGoldBalance>();
			this.HandlePurchaseSuccess(default(StoreManager.PurchaseErrorSource?), null, string.Empty);
			return;
		case Network.PurchaseViaGoldResponse.ErrorType.INSUFFICIENT_GOLD:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_NOT_ENOUGH_GOLD");
			break;
		case Network.PurchaseViaGoldResponse.ErrorType.PRODUCT_NA:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_PRODUCT_NA");
			break;
		case Network.PurchaseViaGoldResponse.ErrorType.FEATURE_NA:
			failDetails = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
			break;
		case Network.PurchaseViaGoldResponse.ErrorType.INVALID_QUANTITY:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_QUANTITY");
			break;
		default:
			failDetails = GameStrings.Get("GLUE_STORE_FAIL_GENERAL");
			break;
		}
		this.Status = StoreManager.TransactionStatus.READY;
		this.m_storePurchaseAuth.CompletePurchaseFailure(null, failDetails);
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x0001A3C0 File Offset: 0x000185C0
	private void OnThirdPartyPurchaseStatusResponse()
	{
		Network.ThirdPartyPurchaseStatusResponse thirdPartyPurchaseStatusResponse = Network.GetThirdPartyPurchaseStatusResponse();
		Debug.Log(string.Format("StoreManager.OnThirdPartyPurchaseStatusResponse(): ThirdPartyID='{0}', Status={1} -- receipt can be removed from client now", thirdPartyPurchaseStatusResponse.ThirdPartyID, thirdPartyPurchaseStatusResponse.Status));
		switch (thirdPartyPurchaseStatusResponse.Status)
		{
		case Network.ThirdPartyPurchaseStatusResponse.PurchaseStatus.NOT_FOUND:
			break;
		case Network.ThirdPartyPurchaseStatusResponse.PurchaseStatus.SUCCEEDED:
		case Network.ThirdPartyPurchaseStatusResponse.PurchaseStatus.FAILED:
			Log.Rachelle.Print(string.Format("StoreManager.OnThirdPartyPurchaseStatusResponse(): ThirdPartyID='{0}', Status={1} -- receipt can be removed from client now", thirdPartyPurchaseStatusResponse.ThirdPartyID, thirdPartyPurchaseStatusResponse.Status), new object[0]);
			break;
		case Network.ThirdPartyPurchaseStatusResponse.PurchaseStatus.IN_PROGRESS:
			Log.Rachelle.Print(string.Format("StoreManager.OnThirdPartyPurchaseStatusResponse(): ThirdPartyID='{0}' still in progress, leave receipt on client", thirdPartyPurchaseStatusResponse.ThirdPartyID), new object[0]);
			break;
		default:
			Debug.LogWarning(string.Format("StoreManager.OnThirdPartyPurchaseStatusResponse(): unexpected Status {0} received for third party ID '{1}'", thirdPartyPurchaseStatusResponse.Status, thirdPartyPurchaseStatusResponse.ThirdPartyID));
			break;
		}
		StoreMobilePurchase.ThirdPartyPurchaseStatus(thirdPartyPurchaseStatusResponse.ThirdPartyID, thirdPartyPurchaseStatusResponse.Status);
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x0001A4A7 File Offset: 0x000186A7
	private void OnStoreComponentReady(object userData)
	{
		if (!this.m_waitingToShowStore)
		{
			return;
		}
		if (!this.IsCurrentStoreLoaded())
		{
			return;
		}
		this.ShowStore();
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x0001A4C8 File Offset: 0x000186C8
	private void OnGeneralStoreLoaded(string name, GameObject go, object callbackData)
	{
		GeneralStore generalStore = this.OnStoreLoaded<GeneralStore>(go, StoreType.GENERAL_STORE);
		if (generalStore != null)
		{
			this.SetupLoadedStore(generalStore);
		}
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x0001A4F4 File Offset: 0x000186F4
	private void OnArenaStoreLoaded(string name, GameObject go, object callbackData)
	{
		ArenaStore arenaStore = this.OnStoreLoaded<ArenaStore>(go, StoreType.ARENA_STORE);
		if (arenaStore != null)
		{
			this.SetupLoadedStore(arenaStore);
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x0001A520 File Offset: 0x00018720
	private void OnAdventureStoreLoaded(string name, GameObject go, object callbackData)
	{
		AdventureStore adventureStore = this.OnStoreLoaded<AdventureStore>(go, StoreType.ADVENTURE_STORE);
		if (adventureStore != null)
		{
			this.SetupLoadedStore(adventureStore);
		}
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x0001A54C File Offset: 0x0001874C
	private T OnStoreLoaded<T>(GameObject go, StoreType storeType) where T : Store
	{
		if (go == null)
		{
			Debug.LogError(string.Format("StoreManager.OnStoreLoaded<{0}>(): go is null!", typeof(T)));
			return (T)((object)null);
		}
		T component = go.GetComponent<T>();
		if (component == null)
		{
			Debug.LogError(string.Format("StoreManager.OnStoreLoaded<{0}>(): go has no {1} component!", typeof(T), typeof(T)));
			return (T)((object)null);
		}
		this.m_stores[storeType] = component;
		return component;
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x0001A5DC File Offset: 0x000187DC
	private void SetupLoadedStore(Store store)
	{
		if (store == null)
		{
			return;
		}
		store.Hide();
		store.RegisterBuyWithMoneyListener(new Store.BuyWithMoneyCallback(this.OnStoreBuyWithMoney));
		store.RegisterBuyWithGoldGTAPPListener(new Store.BuyWithGoldGTAPPCallback(this.OnStoreBuyWithGTAPP));
		store.RegisterBuyWithGoldNoGTAPPListener(new Store.BuyWithGoldNoGTAPPCallback(this.OnStoreBuyWithGoldNoGTAPP));
		store.RegisterExitListener(new Store.ExitCallback(this.OnStoreExit));
		store.RegisterInfoListener(new Store.InfoCallback(this.OnStoreInfo));
		store.RegisterReadyListener(new Store.ReadyCallback(this.OnStoreComponentReady));
		this.OnStoreComponentReady(null);
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x0001A678 File Offset: 0x00018878
	private void OnStorePurchaseAuthLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError("StoreManager.OnStorePurchaseAuthLoaded(): go is null!");
			return;
		}
		this.m_storePurchaseAuth = go.GetComponent<StorePurchaseAuth>();
		if (this.m_storePurchaseAuth == null)
		{
			Debug.LogError("StoreManager.OnStorePurchaseAuthLoaded(): go has no StorePurchaseAuth component");
			return;
		}
		this.m_storePurchaseAuth.Hide();
		this.m_storePurchaseAuth.RegisterAckPurchaseResultListener(new StorePurchaseAuth.AckPurchaseResultListener(this.OnPurchaseResultAcknowledged));
		this.m_storePurchaseAuth.RegisterExitListener(new StorePurchaseAuth.ExitListener(this.OnAuthExit));
		this.OnStoreComponentReady(null);
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x0001A704 File Offset: 0x00018904
	private void OnStoreSummaryLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError("StoreManager.OnStoreSummaryLoaded(): go is null!");
			return;
		}
		this.m_storeSummary = go.GetComponent<StoreSummary>();
		if (this.m_storeSummary == null)
		{
			Debug.LogError("StoreManager.OnStoreSummaryLoaded(): go has no StoreSummary component");
			return;
		}
		this.m_storeSummary.Hide();
		this.m_storeSummary.RegisterConfirmListener(new StoreSummary.ConfirmCallback(this.OnSummaryConfirm));
		this.m_storeSummary.RegisterCancelListener(new StoreSummary.CancelCallback(this.OnSummaryCancel));
		this.m_storeSummary.RegisterInfoListener(new StoreSummary.InfoCallback(this.OnSummaryInfo));
		this.m_storeSummary.RegisterPaymentAndTOSListener(new StoreSummary.PaymentAndTOSCallback(this.OnSummaryPaymentAndTOS));
		this.OnStoreComponentReady(null);
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x0001A7C4 File Offset: 0x000189C4
	private void OnStoreSendToBAMLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError("StoreManager.OnStoreSendToBAMLoaded(): go is null!");
			return;
		}
		this.m_storeSendToBAM = go.GetComponent<StoreSendToBAM>();
		if (this.m_storeSendToBAM == null)
		{
			Debug.LogError("StoreManager.OnStoreSendToBAMLoaded(): go has no StoreSendToBAM component");
			return;
		}
		this.m_storeSendToBAM.Hide();
		this.m_storeSendToBAM.RegisterOkayListener(new StoreSendToBAM.DelOKListener(this.OnSendToBAMOkay));
		this.m_storeSendToBAM.RegisterCancelListener(new StoreSendToBAM.DelCancelListener(this.OnSendToBAMCancel));
		this.OnStoreComponentReady(null);
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x0001A850 File Offset: 0x00018A50
	private void OnStoreLegalBAMLinksLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError("StoreManager.OnStoreLegalBAMLinksLoaded(): go is null!");
			return;
		}
		this.m_storeLegalBAMLinks = go.GetComponent<StoreLegalBAMLinks>();
		if (this.m_storeLegalBAMLinks == null)
		{
			Debug.LogError("StoreManager.OnStoreLegalBAMLinksLoaded(): go has no StoreLegalBAMLinks component");
			return;
		}
		this.m_storeLegalBAMLinks.Hide();
		this.m_storeLegalBAMLinks.RegisterSendToBAMListener(new StoreLegalBAMLinks.SendToBAMListener(this.OnSendToBAMLegal));
		this.m_storeLegalBAMLinks.RegisterCancelListener(new StoreLegalBAMLinks.CancelListener(this.OnSendToBAMLegalCancel));
		this.OnStoreComponentReady(null);
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0001A8DC File Offset: 0x00018ADC
	private void OnStoreDoneWithBAMLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError("StoreManager.OnStoreDoneWithBAMLoaded(): go is null!");
			return;
		}
		this.m_storeDoneWithBAM = go.GetComponent<StoreDoneWithBAM>();
		if (this.m_storeDoneWithBAM == null)
		{
			Debug.LogError("StoreManager.OnStoreDoneWithBAMLoaded(): go has no StoreDoneWithBAM component");
			return;
		}
		this.m_storeDoneWithBAM.Hide();
		this.m_storeDoneWithBAM.RegisterOkayListener(new StoreDoneWithBAM.ButtonPressedListener(this.OnDoneWithBAM));
		this.OnStoreComponentReady(null);
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x0001A954 File Offset: 0x00018B54
	private void OnStoreChallengePromptLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError("StoreManager.OnStoreChallengePromptLoaded(): go is null!");
			return;
		}
		this.m_storeChallengePrompt = go.GetComponent<StoreChallengePrompt>();
		if (this.m_storeChallengePrompt == null)
		{
			Debug.LogError("StoreManager.OnStoreChallengePromptLoaded(): go has no StoreChallengePrompt component");
			return;
		}
		this.m_storeChallengePrompt.Hide();
		this.m_storeChallengePrompt.OnChallengeComplete += this.OnChallengeComplete;
		this.m_storeChallengePrompt.OnCancel += this.OnChallengeCancel;
		this.OnStoreComponentReady(null);
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x0001A9E0 File Offset: 0x00018BE0
	private void OnSceneUnloaded(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		this.UnloadAndFreeMemory();
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x0001A9E8 File Offset: 0x00018BE8
	private void HideAllPurchasePopups()
	{
		if (this.m_storePurchaseAuth != null)
		{
			this.m_storePurchaseAuth.Hide();
		}
		if (this.m_storeSummary != null)
		{
			this.m_storeSummary.Hide();
		}
		if (this.m_storeSendToBAM != null)
		{
			this.m_storeSendToBAM.Hide();
		}
		if (this.m_storeLegalBAMLinks != null)
		{
			this.m_storeLegalBAMLinks.Hide();
		}
		if (this.m_storeDoneWithBAM != null)
		{
			this.m_storeDoneWithBAM.Hide();
		}
		if (this.m_storeChallengePrompt != null)
		{
			this.m_storeChallengePrompt.Hide();
		}
		if (this.m_handlePurchaseNoticesCoroutine != null)
		{
			SceneMgr.Get().StopCoroutine(this.m_handlePurchaseNoticesCoroutine);
		}
	}

	// Token: 0x04000305 RID: 773
	public const int NO_ITEM_COUNT_REQUIREMENT = 0;

	// Token: 0x04000306 RID: 774
	public const int NO_PRODUCT_DATA_REQUIREMENT = 0;

	// Token: 0x04000307 RID: 775
	public static readonly int DEFAULT_SECONDS_BEFORE_AUTO_CANCEL = 600;

	// Token: 0x04000308 RID: 776
	public static readonly int DEFAULT_SECONDS_BEFORE_AUTO_CANCEL_THIRD_PARTY = 60;

	// Token: 0x04000309 RID: 777
	public static readonly PlatformDependentValue<GeneralStoreMode> s_defaultStoreMode = new PlatformDependentValue<GeneralStoreMode>(PlatformCategory.Screen)
	{
		PC = GeneralStoreMode.CARDS,
		Phone = GeneralStoreMode.NONE
	};

	// Token: 0x0400030A RID: 778
	private static readonly long MAX_REQUEST_DELAY_TICKS = 6000000000L;

	// Token: 0x0400030B RID: 779
	private static readonly long MIN_CONFIG_REQUEST_DELAY_TICKS = 600000000L;

	// Token: 0x0400030C RID: 780
	private static readonly long MIN_STATUS_REQUEST_DELAY_TICKS = 450000000L;

	// Token: 0x0400030D RID: 781
	private static readonly long EARLY_STATUS_REQUEST_DELAY_TICKS = 50000000L;

	// Token: 0x0400030E RID: 782
	private static readonly long CHALLENGE_CANCEL_STATUS_REQUEST_DELAY_TICKS = 10000000L;

	// Token: 0x0400030F RID: 783
	private static readonly int UNKNOWN_TRANSACTION_ID = -1;

	// Token: 0x04000310 RID: 784
	private static readonly Map<Currency, Locale> s_currencyToLocaleMap = new Map<Currency, Locale>
	{
		{
			Currency.USD,
			Locale.enUS
		},
		{
			Currency.GBP,
			Locale.enGB
		},
		{
			Currency.KRW,
			Locale.koKR
		},
		{
			Currency.EUR,
			Locale.frFR
		},
		{
			Currency.RUB,
			Locale.ruRU
		},
		{
			Currency.ARS,
			Locale.esMX
		},
		{
			Currency.CLP,
			Locale.esMX
		},
		{
			Currency.MXN,
			Locale.esMX
		},
		{
			Currency.BRL,
			Locale.ptBR
		},
		{
			Currency.AUD,
			Locale.enUS
		},
		{
			Currency.CPT,
			Locale.zhCN
		},
		{
			Currency.TPT,
			Locale.zhTW
		}
	};

	// Token: 0x04000311 RID: 785
	private static readonly string DEFAULT_CURRENCY_FORMAT = "{0:C2}";

	// Token: 0x04000312 RID: 786
	private static readonly Map<Currency, string> s_currencySpecialFormats = new Map<Currency, string>
	{
		{
			Currency.KRW,
			"{0:C0}"
		},
		{
			Currency.TPT,
			"{0:C0}"
		}
	};

	// Token: 0x04000313 RID: 787
	private static readonly Map<AdventureDbId, ProductType> s_adventureToProductMap = new Map<AdventureDbId, ProductType>
	{
		{
			AdventureDbId.NAXXRAMAS,
			3
		},
		{
			AdventureDbId.BRM,
			4
		},
		{
			AdventureDbId.LOE,
			7
		}
	};

	// Token: 0x04000314 RID: 788
	private readonly PlatformDependentValue<string> s_storePrefab = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "StoreMain",
		Phone = "StoreMain_phone"
	};

	// Token: 0x04000315 RID: 789
	private readonly PlatformDependentValue<string> s_storePurchaseAuthPrefab = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "StorePurchaseAuth",
		Phone = "StorePurchaseAuth_phone"
	};

	// Token: 0x04000316 RID: 790
	private readonly PlatformDependentValue<string> s_storeSummaryPrefab = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "StoreSummary",
		Phone = "StoreSummary_phone"
	};

	// Token: 0x04000317 RID: 791
	private readonly PlatformDependentValue<string> s_storeSendToBAMPrefab = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "StoreSendToBAM",
		Phone = "StoreSendToBAM_phone"
	};

	// Token: 0x04000318 RID: 792
	private readonly PlatformDependentValue<string> s_storeDoneWithBAMPrefab = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "StoreDoneWithBAM",
		Phone = "StoreDoneWithBAM_phone"
	};

	// Token: 0x04000319 RID: 793
	private readonly PlatformDependentValue<string> s_storeChallengePromptPrefab = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "StoreChallengePrompt",
		Phone = "StoreChallengePrompt_phone"
	};

	// Token: 0x0400031A RID: 794
	private readonly PlatformDependentValue<string> s_storeLegalBAMLinksPrefab = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "StoreLegalBAMLinks",
		Phone = "StoreLegalBAMLinks_phone"
	};

	// Token: 0x0400031B RID: 795
	private readonly PlatformDependentValue<string> s_arenaStorePrefab = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "ArenaStore",
		Phone = "ArenaStore_phone"
	};

	// Token: 0x0400031C RID: 796
	private readonly PlatformDependentValue<string> s_adventureStorePrefab = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "AdventureStore",
		Phone = "AdventureStore_phone"
	};

	// Token: 0x0400031D RID: 797
	private static StoreManager s_instance = null;

	// Token: 0x0400031E RID: 798
	private bool m_featuresReady;

	// Token: 0x0400031F RID: 799
	private bool m_initComplete;

	// Token: 0x04000320 RID: 800
	private bool m_battlePayAvailable;

	// Token: 0x04000321 RID: 801
	private bool m_noticesReady;

	// Token: 0x04000322 RID: 802
	private bool m_firstMoneyOrGTAPPTransactionSet;

	// Token: 0x04000323 RID: 803
	private long m_ticksBeforeAutoCancel = (long)StoreManager.DEFAULT_SECONDS_BEFORE_AUTO_CANCEL * 10000000L;

	// Token: 0x04000324 RID: 804
	private long m_ticksBeforeAutoCancelThirdParty = (long)StoreManager.DEFAULT_SECONDS_BEFORE_AUTO_CANCEL_THIRD_PARTY * 10000000L;

	// Token: 0x04000325 RID: 805
	private long m_lastCancelRequestTime;

	// Token: 0x04000326 RID: 806
	private long m_lastConfigRequestTime;

	// Token: 0x04000327 RID: 807
	private long m_configRequestDelayTicks = StoreManager.MIN_CONFIG_REQUEST_DELAY_TICKS;

	// Token: 0x04000328 RID: 808
	private bool m_configLoaded;

	// Token: 0x04000329 RID: 809
	private Map<string, Network.Bundle> m_bundles = new Map<string, Network.Bundle>();

	// Token: 0x0400032A RID: 810
	private Map<int, Network.GoldCostBooster> m_goldCostBooster = new Map<int, Network.GoldCostBooster>();

	// Token: 0x0400032B RID: 811
	private long? m_goldCostArena;

	// Token: 0x0400032C RID: 812
	private Currency m_currency;

	// Token: 0x0400032D RID: 813
	private HashSet<long> m_transactionIDsThatReloadedNotices = new HashSet<long>();

	// Token: 0x0400032E RID: 814
	private HashSet<long> m_transactionIDsConclusivelyHandled = new HashSet<long>();

	// Token: 0x0400032F RID: 815
	private Map<StoreType, Store> m_stores = new Map<StoreType, Store>();

	// Token: 0x04000330 RID: 816
	private StoreType m_currentStoreType;

	// Token: 0x04000331 RID: 817
	private Network.PurchaseMethod m_challengePurchaseMethod;

	// Token: 0x04000332 RID: 818
	private List<StoreManager.StatusChangedListener> m_statusChangedListeners = new List<StoreManager.StatusChangedListener>();

	// Token: 0x04000333 RID: 819
	private bool m_openWhenLastEventFired;

	// Token: 0x04000334 RID: 820
	private List<StoreManager.SuccessfulPurchaseAckListener> m_successfulPurchaseAckListeners = new List<StoreManager.SuccessfulPurchaseAckListener>();

	// Token: 0x04000335 RID: 821
	private List<StoreManager.SuccessfulPurchaseListener> m_successfulPurchaseListeners = new List<StoreManager.SuccessfulPurchaseListener>();

	// Token: 0x04000336 RID: 822
	private List<StoreManager.AuthorizationExitListener> m_authExitListeners = new List<StoreManager.AuthorizationExitListener>();

	// Token: 0x04000337 RID: 823
	private List<StoreManager.StoreShownListener> m_storeShownListeners = new List<StoreManager.StoreShownListener>();

	// Token: 0x04000338 RID: 824
	private List<StoreManager.StoreHiddenListener> m_storeHiddenListeners = new List<StoreManager.StoreHiddenListener>();

	// Token: 0x04000339 RID: 825
	private List<StoreManager.StoreAchievesListener> m_storeAchievesListeners = new List<StoreManager.StoreAchievesListener>();

	// Token: 0x0400033A RID: 826
	private StoreManager.TransactionStatus m_status;

	// Token: 0x0400033B RID: 827
	private long m_lastStatusRequestTime;

	// Token: 0x0400033C RID: 828
	private long m_statusRequestDelayTicks = StoreManager.MIN_STATUS_REQUEST_DELAY_TICKS;

	// Token: 0x0400033D RID: 829
	private StorePurchaseAuth m_storePurchaseAuth;

	// Token: 0x0400033E RID: 830
	private StoreSummary m_storeSummary;

	// Token: 0x0400033F RID: 831
	private StoreSendToBAM m_storeSendToBAM;

	// Token: 0x04000340 RID: 832
	private StoreLegalBAMLinks m_storeLegalBAMLinks;

	// Token: 0x04000341 RID: 833
	private StoreDoneWithBAM m_storeDoneWithBAM;

	// Token: 0x04000342 RID: 834
	private StoreChallengePrompt m_storeChallengePrompt;

	// Token: 0x04000343 RID: 835
	private bool m_waitingToShowStore;

	// Token: 0x04000344 RID: 836
	private StoreManager.ShowStoreData m_showStoreData;

	// Token: 0x04000345 RID: 837
	private MoneyOrGTAPPTransaction m_activeMoneyOrGTAPPTransaction;

	// Token: 0x04000346 RID: 838
	private HashSet<long> m_confirmedTransactionIDs = new HashSet<long>();

	// Token: 0x04000347 RID: 839
	private List<NetCache.ProfileNoticePurchase> m_outstandingPurchaseNotices = new List<NetCache.ProfileNoticePurchase>();

	// Token: 0x04000348 RID: 840
	private List<Achievement> m_completedAchieves = new List<Achievement>();

	// Token: 0x04000349 RID: 841
	private bool m_licenseAchievesListenerRegistered;

	// Token: 0x0400034A RID: 842
	private string m_requestedThirdPartyProductId;

	// Token: 0x0400034B RID: 843
	private bool m_shouldAutoCancelThirdPartyTransaction;

	// Token: 0x0400034C RID: 844
	private StoreManager.TransactionStatus m_previousStatusBeforeAutoCancel;

	// Token: 0x0400034D RID: 845
	private bool m_outOfSessionThirdPartyTransaction;

	// Token: 0x0400034E RID: 846
	private Coroutine m_handlePurchaseNoticesCoroutine;

	// Token: 0x0400034F RID: 847
	public static readonly PlatformDependentValue<bool> HAS_THIRD_PARTY_APP_STORE = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		PC = false,
		Mac = false,
		iOS = true,
		Android = true
	};

	// Token: 0x02000271 RID: 625
	// (Invoke) Token: 0x06002331 RID: 9009
	public delegate void StoreShownCallback(object userData);

	// Token: 0x020003C5 RID: 965
	// (Invoke) Token: 0x0600325C RID: 12892
	public delegate void StoreHiddenCallback(object userData);

	// Token: 0x020003D6 RID: 982
	// (Invoke) Token: 0x060032E3 RID: 13027
	public delegate void StatusChangedCallback(bool isOpen, object userData);

	// Token: 0x020003F2 RID: 1010
	// (Invoke) Token: 0x06003425 RID: 13349
	public delegate void SuccessfulPurchaseAckCallback(Network.Bundle bundle, PaymentMethod purchaseMethod, object userData);

	// Token: 0x020003F4 RID: 1012
	// (Invoke) Token: 0x06003429 RID: 13353
	public delegate void SuccessfulPurchaseCallback(Network.Bundle bundle, PaymentMethod purchaseMethod, object userData);

	// Token: 0x020003F5 RID: 1013
	// (Invoke) Token: 0x0600342D RID: 13357
	public delegate void AuthorizationExitCallback(object userData);

	// Token: 0x020003F6 RID: 1014
	// (Invoke) Token: 0x06003431 RID: 13361
	public delegate void StoreAchievesCallback(Network.Bundle bundle, PaymentMethod paymentMethod, object userData);

	// Token: 0x020003F7 RID: 1015
	private enum PurchaseErrorSource
	{
		// Token: 0x04002048 RID: 8264
		FROM_PURCHASE_METHOD_RESPONSE,
		// Token: 0x04002049 RID: 8265
		FROM_STATUS_OR_PURCHASE_RESPONSE,
		// Token: 0x0400204A RID: 8266
		FROM_PREVIOUS_PURCHASE
	}

	// Token: 0x020003F8 RID: 1016
	private enum TransactionStatus
	{
		// Token: 0x0400204C RID: 8268
		UNKNOWN,
		// Token: 0x0400204D RID: 8269
		IN_PROGRESS_MONEY,
		// Token: 0x0400204E RID: 8270
		IN_PROGRESS_GOLD_GTAPP,
		// Token: 0x0400204F RID: 8271
		IN_PROGRESS_GOLD_NO_GTAPP,
		// Token: 0x04002050 RID: 8272
		READY,
		// Token: 0x04002051 RID: 8273
		WAIT_ZERO_COST_LICENSE,
		// Token: 0x04002052 RID: 8274
		WAIT_METHOD_OF_PAYMENT,
		// Token: 0x04002053 RID: 8275
		WAIT_THIRD_PARTY_INIT,
		// Token: 0x04002054 RID: 8276
		WAIT_THIRD_PARTY_RECEIPT,
		// Token: 0x04002055 RID: 8277
		WAIT_CONFIRM,
		// Token: 0x04002056 RID: 8278
		WAIT_RISK,
		// Token: 0x04002057 RID: 8279
		CHALLENGE_SUBMITTED,
		// Token: 0x04002058 RID: 8280
		CHALLENGE_CANCELED,
		// Token: 0x04002059 RID: 8281
		USER_CANCELING,
		// Token: 0x0400205A RID: 8282
		AUTO_CANCELING
	}

	// Token: 0x020003F9 RID: 1017
	private class StoreAchievesData
	{
		// Token: 0x06003434 RID: 13364 RVA: 0x00104896 File Offset: 0x00102A96
		public StoreAchievesData(Network.Bundle bundle, PaymentMethod paymentMethod)
		{
			this.Bundle = bundle;
			this.MethodOfPayment = paymentMethod;
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06003435 RID: 13365 RVA: 0x001048AC File Offset: 0x00102AAC
		// (set) Token: 0x06003436 RID: 13366 RVA: 0x001048B4 File Offset: 0x00102AB4
		public Network.Bundle Bundle { get; private set; }

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06003437 RID: 13367 RVA: 0x001048BD File Offset: 0x00102ABD
		// (set) Token: 0x06003438 RID: 13368 RVA: 0x001048C5 File Offset: 0x00102AC5
		public PaymentMethod MethodOfPayment { get; private set; }
	}

	// Token: 0x020003FA RID: 1018
	private class ZeroCostTransactionData
	{
		// Token: 0x06003439 RID: 13369 RVA: 0x001048CE File Offset: 0x00102ACE
		public ZeroCostTransactionData(Network.Bundle bundle)
		{
			this.Bundle = bundle;
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x0600343A RID: 13370 RVA: 0x001048DD File Offset: 0x00102ADD
		// (set) Token: 0x0600343B RID: 13371 RVA: 0x001048E5 File Offset: 0x00102AE5
		public Network.Bundle Bundle { get; private set; }
	}

	// Token: 0x020003FB RID: 1019
	private class StatusChangedListener : EventListener<StoreManager.StatusChangedCallback>
	{
		// Token: 0x0600343D RID: 13373 RVA: 0x001048F6 File Offset: 0x00102AF6
		public void Fire(bool isOpen)
		{
			this.m_callback(isOpen, this.m_userData);
		}
	}

	// Token: 0x020003FC RID: 1020
	private class SuccessfulPurchaseAckListener : EventListener<StoreManager.SuccessfulPurchaseAckCallback>
	{
		// Token: 0x0600343F RID: 13375 RVA: 0x00104912 File Offset: 0x00102B12
		public void Fire(Network.Bundle bundle, PaymentMethod paymentMethod)
		{
			this.m_callback(bundle, paymentMethod, this.m_userData);
		}
	}

	// Token: 0x020003FD RID: 1021
	private class SuccessfulPurchaseListener : EventListener<StoreManager.SuccessfulPurchaseCallback>
	{
		// Token: 0x06003441 RID: 13377 RVA: 0x0010492F File Offset: 0x00102B2F
		public void Fire(Network.Bundle bundle, PaymentMethod paymentMethod)
		{
			this.m_callback(bundle, paymentMethod, this.m_userData);
		}
	}

	// Token: 0x020003FE RID: 1022
	private class AuthorizationExitListener : EventListener<StoreManager.AuthorizationExitCallback>
	{
		// Token: 0x06003443 RID: 13379 RVA: 0x0010494C File Offset: 0x00102B4C
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x020003FF RID: 1023
	private class StoreShownListener : EventListener<StoreManager.StoreShownCallback>
	{
		// Token: 0x06003445 RID: 13381 RVA: 0x00104967 File Offset: 0x00102B67
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x02000400 RID: 1024
	private class StoreHiddenListener : EventListener<StoreManager.StoreHiddenCallback>
	{
		// Token: 0x06003447 RID: 13383 RVA: 0x00104982 File Offset: 0x00102B82
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x02000401 RID: 1025
	private class StoreAchievesListener : EventListener<StoreManager.StoreAchievesCallback>
	{
		// Token: 0x06003449 RID: 13385 RVA: 0x0010499D File Offset: 0x00102B9D
		public void Fire(Network.Bundle bundle, PaymentMethod paymentMethod)
		{
			this.m_callback(bundle, paymentMethod, this.m_userData);
		}
	}

	// Token: 0x02000402 RID: 1026
	private struct ShowStoreData
	{
		// Token: 0x0400205E RID: 8286
		public bool m_isTotallyFake;

		// Token: 0x0400205F RID: 8287
		public Store.ExitCallback m_exitCallback;

		// Token: 0x04002060 RID: 8288
		public object m_exitCallbackUserData;

		// Token: 0x04002061 RID: 8289
		public ProductType m_storeProduct;

		// Token: 0x04002062 RID: 8290
		public int m_storeProductData;

		// Token: 0x04002063 RID: 8291
		public GeneralStoreMode m_storeMode;

		// Token: 0x04002064 RID: 8292
		public StoreManager.ZeroCostTransactionData m_zeroCostTransactionData;
	}
}
