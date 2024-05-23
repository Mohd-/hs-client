using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003C9 RID: 969
[CustomEditClass]
public class Store : UIBPopup
{
	// Token: 0x06003274 RID: 12916 RVA: 0x000FC88A File Offset: 0x000FAA8A
	protected virtual void Awake()
	{
		this.m_infoButton.SetText(GameStrings.Get("GLUE_STORE_INFO_BUTTON_TEXT"));
	}

	// Token: 0x06003275 RID: 12917 RVA: 0x000FC8A4 File Offset: 0x000FAAA4
	protected virtual void Start()
	{
		this.m_buyWithGoldButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBuyWithGoldPressed));
		this.m_buyWithMoneyButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBuyWithMoneyPressed));
		this.m_buyWithGoldTooltipTrigger.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnShowBuyWithGoldTooltip));
		this.m_buyWithMoneyTooltipTrigger.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnShowBuyWithMoneyTooltip));
		this.m_buyWithGoldTooltipTrigger.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnHideBuyWithGoldTooltip));
		this.m_buyWithMoneyTooltipTrigger.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnHideBuyWithMoneyTooltip));
		this.m_infoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInfoPressed));
		base.StartCoroutine(this.NotifyListenersWhenReady());
	}

	// Token: 0x06003276 RID: 12918 RVA: 0x000FC970 File Offset: 0x000FAB70
	protected virtual void OnDestroy()
	{
		this.m_enabledGoldButtonMaterial = null;
		this.m_disabledGoldButtonMaterial = null;
		this.m_enabledMoneyButtonMaterial = null;
		this.m_disabledMoneyButtonMaterial = null;
		if (FullScreenFXMgr.Get())
		{
			this.EnableFullScreenEffects(false);
		}
	}

	// Token: 0x06003277 RID: 12919 RVA: 0x000FC9AF File Offset: 0x000FABAF
	public virtual void OnGoldBalanceChanged(NetCache.NetCacheGoldBalance balance)
	{
	}

	// Token: 0x06003278 RID: 12920 RVA: 0x000FC9B1 File Offset: 0x000FABB1
	public virtual void OnGoldSpent()
	{
	}

	// Token: 0x06003279 RID: 12921 RVA: 0x000FC9B3 File Offset: 0x000FABB3
	public virtual void OnMoneySpent()
	{
	}

	// Token: 0x0600327A RID: 12922 RVA: 0x000FC9B5 File Offset: 0x000FABB5
	public void Show(Store.DelOnStoreShown onStoreShownCB, bool isTotallyFake)
	{
		this.ShowImpl(onStoreShownCB, isTotallyFake);
	}

	// Token: 0x0600327B RID: 12923 RVA: 0x000FC9BF File Offset: 0x000FABBF
	public virtual void Close()
	{
	}

	// Token: 0x0600327C RID: 12924 RVA: 0x000FC9C1 File Offset: 0x000FABC1
	public void SetStoreType(StoreType storeType)
	{
		this.m_storeType = storeType;
	}

	// Token: 0x0600327D RID: 12925 RVA: 0x000FC9CA File Offset: 0x000FABCA
	public StoreType GetStoreType()
	{
		return this.m_storeType;
	}

	// Token: 0x0600327E RID: 12926 RVA: 0x000FC9D2 File Offset: 0x000FABD2
	public virtual bool IsReady()
	{
		return true;
	}

	// Token: 0x0600327F RID: 12927 RVA: 0x000FC9D5 File Offset: 0x000FABD5
	public bool IsCovered()
	{
		return this.m_cover.activeSelf;
	}

	// Token: 0x06003280 RID: 12928 RVA: 0x000FC9E2 File Offset: 0x000FABE2
	public void ActivateCover(bool coverActive)
	{
		this.m_cover.SetActive(coverActive);
		this.EnableBuyButtons(!coverActive);
	}

	// Token: 0x06003281 RID: 12929 RVA: 0x000FC9FA File Offset: 0x000FABFA
	public void EnableClickCatcher(bool enabled)
	{
		this.m_offClicker.gameObject.SetActive(enabled);
	}

	// Token: 0x06003282 RID: 12930 RVA: 0x000FCA0D File Offset: 0x000FAC0D
	public bool RegisterBuyWithMoneyListener(Store.BuyWithMoneyCallback callback)
	{
		return this.RegisterBuyWithMoneyListener(callback, null);
	}

	// Token: 0x06003283 RID: 12931 RVA: 0x000FCA18 File Offset: 0x000FAC18
	public bool RegisterBuyWithMoneyListener(Store.BuyWithMoneyCallback callback, object userData)
	{
		Store.BuyWithMoneyListener buyWithMoneyListener = new Store.BuyWithMoneyListener();
		buyWithMoneyListener.SetCallback(callback);
		buyWithMoneyListener.SetUserData(userData);
		if (this.m_buyWithMoneyListeners.Contains(buyWithMoneyListener))
		{
			return false;
		}
		this.m_buyWithMoneyListeners.Add(buyWithMoneyListener);
		return true;
	}

	// Token: 0x06003284 RID: 12932 RVA: 0x000FCA59 File Offset: 0x000FAC59
	public bool RemoveBuyWithMoneyListener(Store.BuyWithMoneyCallback callback)
	{
		return this.RemoveBuyWithMoneyListener(callback, null);
	}

	// Token: 0x06003285 RID: 12933 RVA: 0x000FCA64 File Offset: 0x000FAC64
	public bool RemoveBuyWithMoneyListener(Store.BuyWithMoneyCallback callback, object userData)
	{
		Store.BuyWithMoneyListener buyWithMoneyListener = new Store.BuyWithMoneyListener();
		buyWithMoneyListener.SetCallback(callback);
		buyWithMoneyListener.SetUserData(userData);
		return this.m_buyWithMoneyListeners.Remove(buyWithMoneyListener);
	}

	// Token: 0x06003286 RID: 12934 RVA: 0x000FCA91 File Offset: 0x000FAC91
	public bool RegisterBuyWithGoldGTAPPListener(Store.BuyWithGoldGTAPPCallback callback)
	{
		return this.RegisterBuyWithGoldGTAPPListener(callback, null);
	}

	// Token: 0x06003287 RID: 12935 RVA: 0x000FCA9C File Offset: 0x000FAC9C
	public bool RegisterBuyWithGoldGTAPPListener(Store.BuyWithGoldGTAPPCallback callback, object userData)
	{
		Store.BuyWithGoldGTAPPListener buyWithGoldGTAPPListener = new Store.BuyWithGoldGTAPPListener();
		buyWithGoldGTAPPListener.SetCallback(callback);
		buyWithGoldGTAPPListener.SetUserData(userData);
		if (this.m_buyWithGoldGTAPPListeners.Contains(buyWithGoldGTAPPListener))
		{
			return false;
		}
		this.m_buyWithGoldGTAPPListeners.Add(buyWithGoldGTAPPListener);
		return true;
	}

	// Token: 0x06003288 RID: 12936 RVA: 0x000FCADD File Offset: 0x000FACDD
	public bool RemoveBuyWithGoldGTAPPListener(Store.BuyWithGoldGTAPPCallback callback)
	{
		return this.RemoveBuyWithGoldGTAPPListener(callback, null);
	}

	// Token: 0x06003289 RID: 12937 RVA: 0x000FCAE8 File Offset: 0x000FACE8
	public bool RemoveBuyWithGoldGTAPPListener(Store.BuyWithGoldGTAPPCallback callback, object userData)
	{
		Store.BuyWithGoldGTAPPListener buyWithGoldGTAPPListener = new Store.BuyWithGoldGTAPPListener();
		buyWithGoldGTAPPListener.SetCallback(callback);
		buyWithGoldGTAPPListener.SetUserData(userData);
		return this.m_buyWithGoldGTAPPListeners.Remove(buyWithGoldGTAPPListener);
	}

	// Token: 0x0600328A RID: 12938 RVA: 0x000FCB15 File Offset: 0x000FAD15
	public bool RegisterBuyWithGoldNoGTAPPListener(Store.BuyWithGoldNoGTAPPCallback callback)
	{
		return this.RegisterBuyWithGoldNoGTAPPListener(callback, null);
	}

	// Token: 0x0600328B RID: 12939 RVA: 0x000FCB20 File Offset: 0x000FAD20
	public bool RegisterBuyWithGoldNoGTAPPListener(Store.BuyWithGoldNoGTAPPCallback callback, object userData)
	{
		Store.BuyWithGoldNoGTAPPListener buyWithGoldNoGTAPPListener = new Store.BuyWithGoldNoGTAPPListener();
		buyWithGoldNoGTAPPListener.SetCallback(callback);
		buyWithGoldNoGTAPPListener.SetUserData(userData);
		if (this.m_buyWithGoldNoGTAPPListeners.Contains(buyWithGoldNoGTAPPListener))
		{
			return false;
		}
		this.m_buyWithGoldNoGTAPPListeners.Add(buyWithGoldNoGTAPPListener);
		return true;
	}

	// Token: 0x0600328C RID: 12940 RVA: 0x000FCB61 File Offset: 0x000FAD61
	public bool RemoveBuyWithGoldNoGTAPPListener(Store.BuyWithGoldNoGTAPPCallback callback)
	{
		return this.RemoveBuyWithGoldNoGTAPPListener(callback, null);
	}

	// Token: 0x0600328D RID: 12941 RVA: 0x000FCB6C File Offset: 0x000FAD6C
	public bool RemoveBuyWithGoldNoGTAPPListener(Store.BuyWithGoldNoGTAPPCallback callback, object userData)
	{
		Store.BuyWithGoldNoGTAPPListener buyWithGoldNoGTAPPListener = new Store.BuyWithGoldNoGTAPPListener();
		buyWithGoldNoGTAPPListener.SetCallback(callback);
		buyWithGoldNoGTAPPListener.SetUserData(userData);
		return this.m_buyWithGoldNoGTAPPListeners.Remove(buyWithGoldNoGTAPPListener);
	}

	// Token: 0x0600328E RID: 12942 RVA: 0x000FCB99 File Offset: 0x000FAD99
	public bool RegisterExitListener(Store.ExitCallback callback)
	{
		return this.RegisterExitListener(callback, null);
	}

	// Token: 0x0600328F RID: 12943 RVA: 0x000FCBA4 File Offset: 0x000FADA4
	public bool RegisterExitListener(Store.ExitCallback callback, object userData)
	{
		Store.ExitListener exitListener = new Store.ExitListener();
		exitListener.SetCallback(callback);
		exitListener.SetUserData(userData);
		if (this.m_exitListeners.Contains(exitListener))
		{
			return false;
		}
		this.m_exitListeners.Add(exitListener);
		return true;
	}

	// Token: 0x06003290 RID: 12944 RVA: 0x000FCBE5 File Offset: 0x000FADE5
	public bool RemoveExitListener(Store.ExitCallback callback)
	{
		return this.RemoveExitListener(callback, null);
	}

	// Token: 0x06003291 RID: 12945 RVA: 0x000FCBF0 File Offset: 0x000FADF0
	public bool RemoveExitListener(Store.ExitCallback callback, object userData)
	{
		Store.ExitListener exitListener = new Store.ExitListener();
		exitListener.SetCallback(callback);
		exitListener.SetUserData(userData);
		return this.m_exitListeners.Remove(exitListener);
	}

	// Token: 0x06003292 RID: 12946 RVA: 0x000FCC1D File Offset: 0x000FAE1D
	public bool RegisterReadyListener(Store.ReadyCallback callback)
	{
		return this.RegisterReadyListener(callback, null);
	}

	// Token: 0x06003293 RID: 12947 RVA: 0x000FCC28 File Offset: 0x000FAE28
	public bool RegisterReadyListener(Store.ReadyCallback callback, object userData)
	{
		Store.ReadyListener readyListener = new Store.ReadyListener();
		readyListener.SetCallback(callback);
		readyListener.SetUserData(userData);
		if (this.m_readyListeners.Contains(readyListener))
		{
			return false;
		}
		this.m_readyListeners.Add(readyListener);
		return true;
	}

	// Token: 0x06003294 RID: 12948 RVA: 0x000FCC69 File Offset: 0x000FAE69
	public bool RemoveReadyListener(Store.ReadyCallback callback)
	{
		return this.RemoveReadyListener(callback, null);
	}

	// Token: 0x06003295 RID: 12949 RVA: 0x000FCC74 File Offset: 0x000FAE74
	public bool RemoveReadyListener(Store.ReadyCallback callback, object userData)
	{
		Store.ReadyListener readyListener = new Store.ReadyListener();
		readyListener.SetCallback(callback);
		readyListener.SetUserData(userData);
		return this.m_readyListeners.Remove(readyListener);
	}

	// Token: 0x06003296 RID: 12950 RVA: 0x000FCCA1 File Offset: 0x000FAEA1
	public bool RegisterInfoListener(Store.InfoCallback callback)
	{
		return this.RegisterInfoListener(callback, null);
	}

	// Token: 0x06003297 RID: 12951 RVA: 0x000FCCAC File Offset: 0x000FAEAC
	public bool RegisterInfoListener(Store.InfoCallback callback, object userData)
	{
		Store.InfoListener infoListener = new Store.InfoListener();
		infoListener.SetCallback(callback);
		infoListener.SetUserData(userData);
		if (this.m_infoListeners.Contains(infoListener))
		{
			return false;
		}
		this.m_infoListeners.Add(infoListener);
		return true;
	}

	// Token: 0x06003298 RID: 12952 RVA: 0x000FCCED File Offset: 0x000FAEED
	public bool RemoveInfoListener(Store.InfoCallback callback)
	{
		return this.RemoveInfoListener(callback, null);
	}

	// Token: 0x06003299 RID: 12953 RVA: 0x000FCCF8 File Offset: 0x000FAEF8
	public bool RemoveInfoListener(Store.InfoCallback callback, object userData)
	{
		Store.InfoListener infoListener = new Store.InfoListener();
		infoListener.SetCallback(callback);
		infoListener.SetUserData(userData);
		return this.m_infoListeners.Remove(infoListener);
	}

	// Token: 0x0600329A RID: 12954 RVA: 0x000FCD25 File Offset: 0x000FAF25
	protected virtual void BuyWithGold(UIEvent e)
	{
	}

	// Token: 0x0600329B RID: 12955 RVA: 0x000FCD27 File Offset: 0x000FAF27
	protected virtual void BuyWithMoney(UIEvent e)
	{
	}

	// Token: 0x0600329C RID: 12956 RVA: 0x000FCD29 File Offset: 0x000FAF29
	protected virtual void ShowImpl(Store.DelOnStoreShown onStoreShownCB, bool isTotallyFake)
	{
	}

	// Token: 0x0600329D RID: 12957 RVA: 0x000FCD2C File Offset: 0x000FAF2C
	public void FireExitEvent(bool authorizationBackButtonPressed)
	{
		Store.ExitListener[] array = this.m_exitListeners.ToArray();
		foreach (Store.ExitListener exitListener in array)
		{
			exitListener.Fire(authorizationBackButtonPressed);
		}
	}

	// Token: 0x0600329E RID: 12958 RVA: 0x000FCD68 File Offset: 0x000FAF68
	protected void EnableFullScreenEffects(bool enable)
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			return;
		}
		if (enable)
		{
			FullScreenFXMgr.Get().StartStandardBlurVignette(1f);
		}
		else
		{
			FullScreenFXMgr.Get().EndStandardBlurVignette(1f, null);
		}
	}

	// Token: 0x0600329F RID: 12959 RVA: 0x000FCDB4 File Offset: 0x000FAFB4
	protected void FireBuyWithMoneyEvent(string productID, int quantity)
	{
		Store.BuyWithMoneyListener[] array = this.m_buyWithMoneyListeners.ToArray();
		foreach (Store.BuyWithMoneyListener buyWithMoneyListener in array)
		{
			buyWithMoneyListener.Fire(productID, quantity);
		}
	}

	// Token: 0x060032A0 RID: 12960 RVA: 0x000FCDF0 File Offset: 0x000FAFF0
	protected void FireBuyWithGoldEventGTAPP(string productID, int quantity)
	{
		Store.BuyWithGoldGTAPPListener[] array = this.m_buyWithGoldGTAPPListeners.ToArray();
		foreach (Store.BuyWithGoldGTAPPListener buyWithGoldGTAPPListener in array)
		{
			buyWithGoldGTAPPListener.Fire(productID, quantity);
		}
	}

	// Token: 0x060032A1 RID: 12961 RVA: 0x000FCE2C File Offset: 0x000FB02C
	protected void FireBuyWithGoldEventNoGTAPP(NoGTAPPTransactionData noGTAPPTransactionData)
	{
		Store.BuyWithGoldNoGTAPPListener[] array = this.m_buyWithGoldNoGTAPPListeners.ToArray();
		foreach (Store.BuyWithGoldNoGTAPPListener buyWithGoldNoGTAPPListener in array)
		{
			buyWithGoldNoGTAPPListener.Fire(noGTAPPTransactionData);
		}
	}

	// Token: 0x060032A2 RID: 12962 RVA: 0x000FCE66 File Offset: 0x000FB066
	protected void SetGoldButtonState(Store.BuyButtonState state)
	{
		this.m_goldButtonState = state;
		this.UpdateBuyButtonsState();
	}

	// Token: 0x060032A3 RID: 12963 RVA: 0x000FCE75 File Offset: 0x000FB075
	protected void SetMoneyButtonState(Store.BuyButtonState state)
	{
		this.m_moneyButtonState = state;
		this.UpdateBuyButtonsState();
	}

	// Token: 0x060032A4 RID: 12964 RVA: 0x000FCE84 File Offset: 0x000FB084
	protected bool AllowBuyingWithGold()
	{
		return this.m_buyButtonsEnabled && Store.BuyButtonState.ENABLED == this.m_goldButtonState;
	}

	// Token: 0x060032A5 RID: 12965 RVA: 0x000FCE9D File Offset: 0x000FB09D
	protected bool AllowBuyingWithMoney()
	{
		return this.m_buyButtonsEnabled && Store.BuyButtonState.ENABLED == this.m_moneyButtonState;
	}

	// Token: 0x060032A6 RID: 12966 RVA: 0x000FCEB6 File Offset: 0x000FB0B6
	protected bool CanShowBuyWithGoldTooltip()
	{
		return !this.AllowBuyingWithGold() && this.m_goldButtonState != Store.BuyButtonState.DISABLED_NO_TOOLTIP;
	}

	// Token: 0x060032A7 RID: 12967 RVA: 0x000FCED1 File Offset: 0x000FB0D1
	protected bool CanShowBuyWithMoneyTooltip()
	{
		return !this.AllowBuyingWithMoney() && this.m_moneyButtonState != Store.BuyButtonState.DISABLED_NO_TOOLTIP;
	}

	// Token: 0x060032A8 RID: 12968 RVA: 0x000FCEEC File Offset: 0x000FB0EC
	private void EnableBuyButtons(bool buyButtonsEnabled)
	{
		this.m_buyButtonsEnabled = buyButtonsEnabled;
		this.UpdateBuyButtonsState();
	}

	// Token: 0x060032A9 RID: 12969 RVA: 0x000FCEFC File Offset: 0x000FB0FC
	private void UpdateBuyButtonsState()
	{
		bool flag = this.AllowBuyingWithMoney();
		bool flag2 = this.AllowBuyingWithGold();
		this.UpdateBuyButtonMaterial(this.m_moneyButtonMeshes, (!flag) ? this.m_disabledMoneyButtonMaterial : this.m_enabledMoneyButtonMaterial);
		this.UpdateBuyButtonMaterial(this.m_goldButtonMeshes, (!flag2) ? this.m_disabledGoldButtonMaterial : this.m_enabledGoldButtonMaterial);
		this.m_buyWithGoldButton.GetComponent<Collider>().enabled = flag2;
		this.m_buyWithGoldTooltipTrigger.GetComponent<Collider>().enabled = this.CanShowBuyWithGoldTooltip();
		this.m_buyWithMoneyButton.GetComponent<Collider>().enabled = flag;
		this.m_buyWithMoneyTooltipTrigger.GetComponent<Collider>().enabled = this.CanShowBuyWithMoneyTooltip();
	}

	// Token: 0x060032AA RID: 12970 RVA: 0x000FCFAC File Offset: 0x000FB1AC
	private void UpdateBuyButtonMaterial(List<MeshRenderer> renderers, Material material)
	{
		foreach (MeshRenderer meshRenderer in renderers)
		{
			if (meshRenderer != null)
			{
				meshRenderer.material = material;
			}
		}
	}

	// Token: 0x060032AB RID: 12971 RVA: 0x000FD010 File Offset: 0x000FB210
	private IEnumerator NotifyListenersWhenReady()
	{
		while (!this.IsReady())
		{
			yield return null;
		}
		Store.ReadyListener[] listeners = this.m_readyListeners.ToArray();
		foreach (Store.ReadyListener listener in listeners)
		{
			listener.Fire();
		}
		yield break;
	}

	// Token: 0x060032AC RID: 12972 RVA: 0x000FD02C File Offset: 0x000FB22C
	protected void OnShowBuyWithMoneyTooltip(UIEvent e)
	{
		if (this.m_moneyButtonState != Store.BuyButtonState.ENABLED)
		{
			this.ShowBuyTooltip(this.m_buyWithMoneyTooltip, "GLUE_STORE_MONEY_BUTTON_TOOLTIP_HEADLINE", this.m_moneyButtonState);
		}
	}

	// Token: 0x060032AD RID: 12973 RVA: 0x000FD05C File Offset: 0x000FB25C
	protected void OnShowBuyWithGoldTooltip(UIEvent e)
	{
		if (this.m_goldButtonState != Store.BuyButtonState.ENABLED)
		{
			this.ShowBuyTooltip(this.m_buyWithGoldTooltip, "GLUE_STORE_GOLD_BUTTON_TOOLTIP_HEADLINE", this.m_goldButtonState);
		}
	}

	// Token: 0x060032AE RID: 12974 RVA: 0x000FD08B File Offset: 0x000FB28B
	private void ShowBuyTooltip(TooltipZone tooltipZone, string tooltipText, Store.BuyButtonState buttonState)
	{
		tooltipZone.ShowLayerTooltip(GameStrings.Get(tooltipText), this.GetBuyButtonTooltipMessage(buttonState));
	}

	// Token: 0x060032AF RID: 12975 RVA: 0x000FD0A1 File Offset: 0x000FB2A1
	protected void OnHideBuyWithMoneyTooltip(UIEvent e)
	{
		if (this.m_moneyButtonState != Store.BuyButtonState.ENABLED)
		{
			this.m_buyWithMoneyTooltip.HideTooltip();
		}
	}

	// Token: 0x060032B0 RID: 12976 RVA: 0x000FD0B9 File Offset: 0x000FB2B9
	protected void OnHideBuyWithGoldTooltip(UIEvent e)
	{
		if (this.m_goldButtonState != Store.BuyButtonState.ENABLED)
		{
			this.m_buyWithGoldTooltip.HideTooltip();
		}
	}

	// Token: 0x060032B1 RID: 12977 RVA: 0x000FD0D1 File Offset: 0x000FB2D1
	protected void OnBuyWithGoldPressed(UIEvent e)
	{
		if (!this.AllowBuyingWithGold())
		{
			return;
		}
		this.BuyWithGold(e);
	}

	// Token: 0x060032B2 RID: 12978 RVA: 0x000FD0E6 File Offset: 0x000FB2E6
	protected void OnBuyWithMoneyPressed(UIEvent e)
	{
		if (!this.AllowBuyingWithMoney())
		{
			return;
		}
		this.BuyWithMoney(e);
	}

	// Token: 0x060032B3 RID: 12979 RVA: 0x000FD0FC File Offset: 0x000FB2FC
	protected void OnInfoPressed(UIEvent e)
	{
		Store.InfoListener[] array = this.m_infoListeners.ToArray();
		foreach (Store.InfoListener infoListener in array)
		{
			infoListener.Fire();
		}
	}

	// Token: 0x060032B4 RID: 12980 RVA: 0x000FD135 File Offset: 0x000FB335
	protected virtual string GetOwnedTooltipString()
	{
		return GameStrings.Get("GLUE_STORE_DUNGEON_BUTTON_TEXT_PURCHASED");
	}

	// Token: 0x060032B5 RID: 12981 RVA: 0x000FD144 File Offset: 0x000FB344
	private string GetBuyButtonTooltipMessage(Store.BuyButtonState state)
	{
		switch (state)
		{
		case Store.BuyButtonState.DISABLED_NOT_ENOUGH_GOLD:
			return GameStrings.Get("GLUE_STORE_FAIL_NOT_ENOUGH_GOLD");
		case Store.BuyButtonState.DISABLED_FEATURE:
			return GameStrings.Get("GLUE_STORE_DISABLED");
		case Store.BuyButtonState.DISABLED_OWNED:
			return this.GetOwnedTooltipString();
		case Store.BuyButtonState.DISABLED_NO_TOOLTIP:
			return string.Empty;
		}
		return GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
	}

	// Token: 0x04001F72 RID: 8050
	[CustomEditField(Sections = "Store/UI")]
	public GameObject m_root;

	// Token: 0x04001F73 RID: 8051
	[CustomEditField(Sections = "Store/UI")]
	public GameObject m_cover;

	// Token: 0x04001F74 RID: 8052
	[CustomEditField(Sections = "Store/UI")]
	public UIBButton m_buyWithMoneyButton;

	// Token: 0x04001F75 RID: 8053
	[CustomEditField(Sections = "Store/UI")]
	public TooltipZone m_buyWithMoneyTooltip;

	// Token: 0x04001F76 RID: 8054
	[CustomEditField(Sections = "Store/UI")]
	public PegUIElement m_buyWithMoneyTooltipTrigger;

	// Token: 0x04001F77 RID: 8055
	[CustomEditField(Sections = "Store/UI")]
	public UIBButton m_buyWithGoldButton;

	// Token: 0x04001F78 RID: 8056
	[CustomEditField(Sections = "Store/UI")]
	public TooltipZone m_buyWithGoldTooltip;

	// Token: 0x04001F79 RID: 8057
	[CustomEditField(Sections = "Store/UI")]
	public PegUIElement m_buyWithGoldTooltipTrigger;

	// Token: 0x04001F7A RID: 8058
	[CustomEditField(Sections = "Store/UI")]
	public UIBButton m_infoButton;

	// Token: 0x04001F7B RID: 8059
	[CustomEditField(Sections = "Store/Materials")]
	public List<MeshRenderer> m_goldButtonMeshes = new List<MeshRenderer>();

	// Token: 0x04001F7C RID: 8060
	[CustomEditField(Sections = "Store/Materials")]
	public Material m_enabledGoldButtonMaterial;

	// Token: 0x04001F7D RID: 8061
	[CustomEditField(Sections = "Store/Materials")]
	public Material m_disabledGoldButtonMaterial;

	// Token: 0x04001F7E RID: 8062
	[CustomEditField(Sections = "Store/Materials")]
	public List<MeshRenderer> m_moneyButtonMeshes = new List<MeshRenderer>();

	// Token: 0x04001F7F RID: 8063
	[CustomEditField(Sections = "Store/Materials")]
	public Material m_enabledMoneyButtonMaterial;

	// Token: 0x04001F80 RID: 8064
	[CustomEditField(Sections = "Store/Materials")]
	public Material m_disabledMoneyButtonMaterial;

	// Token: 0x04001F81 RID: 8065
	[CustomEditField(Sections = "Store/UI")]
	public PegUIElement m_offClicker;

	// Token: 0x04001F82 RID: 8066
	protected bool m_buyButtonsEnabled = true;

	// Token: 0x04001F83 RID: 8067
	protected StoreType m_storeType;

	// Token: 0x04001F84 RID: 8068
	private List<Store.ExitListener> m_exitListeners = new List<Store.ExitListener>();

	// Token: 0x04001F85 RID: 8069
	private List<Store.BuyWithMoneyListener> m_buyWithMoneyListeners = new List<Store.BuyWithMoneyListener>();

	// Token: 0x04001F86 RID: 8070
	private Store.BuyButtonState m_moneyButtonState;

	// Token: 0x04001F87 RID: 8071
	private List<Store.BuyWithGoldGTAPPListener> m_buyWithGoldGTAPPListeners = new List<Store.BuyWithGoldGTAPPListener>();

	// Token: 0x04001F88 RID: 8072
	private List<Store.BuyWithGoldNoGTAPPListener> m_buyWithGoldNoGTAPPListeners = new List<Store.BuyWithGoldNoGTAPPListener>();

	// Token: 0x04001F89 RID: 8073
	private Store.BuyButtonState m_goldButtonState;

	// Token: 0x04001F8A RID: 8074
	private List<Store.ReadyListener> m_readyListeners = new List<Store.ReadyListener>();

	// Token: 0x04001F8B RID: 8075
	private List<Store.InfoListener> m_infoListeners = new List<Store.InfoListener>();

	// Token: 0x020003CA RID: 970
	// (Invoke) Token: 0x060032B7 RID: 12983
	public delegate void ExitCallback(bool authorizationBackButtonPressed, object userData);

	// Token: 0x02000414 RID: 1044
	// (Invoke) Token: 0x0600359A RID: 13722
	public delegate void DelOnStoreShown();

	// Token: 0x0200041A RID: 1050
	// (Invoke) Token: 0x060035C3 RID: 13763
	public delegate void BuyWithMoneyCallback(string productID, int quantity, object userData);

	// Token: 0x0200041B RID: 1051
	// (Invoke) Token: 0x060035C7 RID: 13767
	public delegate void BuyWithGoldGTAPPCallback(string productID, int quantity, object userData);

	// Token: 0x0200041C RID: 1052
	// (Invoke) Token: 0x060035CB RID: 13771
	public delegate void BuyWithGoldNoGTAPPCallback(NoGTAPPTransactionData noGTAPPTransactionData, object userData);

	// Token: 0x0200041D RID: 1053
	// (Invoke) Token: 0x060035CF RID: 13775
	public delegate void InfoCallback(object userData);

	// Token: 0x0200041E RID: 1054
	// (Invoke) Token: 0x060035D3 RID: 13779
	public delegate void ReadyCallback(object userData);

	// Token: 0x02000439 RID: 1081
	protected enum BuyButtonState
	{
		// Token: 0x040021C5 RID: 8645
		ENABLED,
		// Token: 0x040021C6 RID: 8646
		DISABLED_NOT_ENOUGH_GOLD,
		// Token: 0x040021C7 RID: 8647
		DISABLED_FEATURE,
		// Token: 0x040021C8 RID: 8648
		DISABLED,
		// Token: 0x040021C9 RID: 8649
		DISABLED_OWNED,
		// Token: 0x040021CA RID: 8650
		DISABLED_NO_TOOLTIP
	}

	// Token: 0x0200043A RID: 1082
	private class ExitListener : EventListener<Store.ExitCallback>
	{
		// Token: 0x06003641 RID: 13889 RVA: 0x0010BB24 File Offset: 0x00109D24
		public void Fire(bool authorizationBackButtonPressed)
		{
			this.m_callback(authorizationBackButtonPressed, this.m_userData);
		}
	}

	// Token: 0x0200043B RID: 1083
	private class BuyWithMoneyListener : EventListener<Store.BuyWithMoneyCallback>
	{
		// Token: 0x06003643 RID: 13891 RVA: 0x0010BB40 File Offset: 0x00109D40
		public void Fire(string productID, int quantity)
		{
			this.m_callback(productID, quantity, this.m_userData);
		}
	}

	// Token: 0x0200043C RID: 1084
	private class BuyWithGoldGTAPPListener : EventListener<Store.BuyWithGoldGTAPPCallback>
	{
		// Token: 0x06003645 RID: 13893 RVA: 0x0010BB5D File Offset: 0x00109D5D
		public void Fire(string productID, int quantity)
		{
			this.m_callback(productID, quantity, this.m_userData);
		}
	}

	// Token: 0x0200043D RID: 1085
	private class BuyWithGoldNoGTAPPListener : EventListener<Store.BuyWithGoldNoGTAPPCallback>
	{
		// Token: 0x06003647 RID: 13895 RVA: 0x0010BB7A File Offset: 0x00109D7A
		public void Fire(NoGTAPPTransactionData noGTAPPTransactionData)
		{
			this.m_callback(noGTAPPTransactionData, this.m_userData);
		}
	}

	// Token: 0x0200043E RID: 1086
	private class ReadyListener : EventListener<Store.ReadyCallback>
	{
		// Token: 0x06003649 RID: 13897 RVA: 0x0010BB96 File Offset: 0x00109D96
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x0200043F RID: 1087
	private class InfoListener : EventListener<Store.InfoCallback>
	{
		// Token: 0x0600364B RID: 13899 RVA: 0x0010BBB1 File Offset: 0x00109DB1
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}
}
