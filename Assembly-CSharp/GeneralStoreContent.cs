using System;
using System.Collections.Generic;
using PegasusUtil;
using UnityEngine;

// Token: 0x0200044A RID: 1098
public class GeneralStoreContent : MonoBehaviour
{
	// Token: 0x060036A5 RID: 13989 RVA: 0x0010D664 File Offset: 0x0010B864
	public void SetParentStore(GeneralStore parentStore)
	{
		this.m_parentStore = parentStore;
	}

	// Token: 0x060036A6 RID: 13990 RVA: 0x0010D66D File Offset: 0x0010B86D
	public void SetContentActive(bool active)
	{
		this.m_isContentActive = active;
	}

	// Token: 0x060036A7 RID: 13991 RVA: 0x0010D678 File Offset: 0x0010B878
	public bool IsContentActive()
	{
		return this.m_isContentActive && (this.m_parentStore == null || !this.m_parentStore.IsCovered());
	}

	// Token: 0x060036A8 RID: 13992 RVA: 0x0010D6B5 File Offset: 0x0010B8B5
	public ProductType GetProductType()
	{
		return this.m_productType;
	}

	// Token: 0x060036A9 RID: 13993 RVA: 0x0010D6BD File Offset: 0x0010B8BD
	public void SetCurrentGoldBundle(NoGTAPPTransactionData bundle)
	{
		if (this.m_currentGoldBundle == bundle)
		{
			return;
		}
		this.m_currentMoneyBundle = null;
		this.m_currentGoldBundle = bundle;
		this.OnBundleChanged(this.m_currentGoldBundle, this.m_currentMoneyBundle);
		this.FireBundleChangedEvent();
	}

	// Token: 0x060036AA RID: 13994 RVA: 0x0010D6F2 File Offset: 0x0010B8F2
	public NoGTAPPTransactionData GetCurrentGoldBundle()
	{
		return this.m_currentGoldBundle;
	}

	// Token: 0x060036AB RID: 13995 RVA: 0x0010D6FC File Offset: 0x0010B8FC
	public void SetCurrentMoneyBundle(Network.Bundle bundle, bool force = false)
	{
		if (!force && this.m_currentMoneyBundle == bundle && bundle != null)
		{
			return;
		}
		this.m_currentGoldBundle = null;
		this.m_currentMoneyBundle = bundle;
		this.OnBundleChanged(this.m_currentGoldBundle, this.m_currentMoneyBundle);
		this.FireBundleChangedEvent();
	}

	// Token: 0x060036AC RID: 13996 RVA: 0x0010D748 File Offset: 0x0010B948
	public Network.Bundle GetCurrentMoneyBundle()
	{
		return this.m_currentMoneyBundle;
	}

	// Token: 0x060036AD RID: 13997 RVA: 0x0010D750 File Offset: 0x0010B950
	public void Refresh()
	{
		this.OnRefresh();
	}

	// Token: 0x060036AE RID: 13998 RVA: 0x0010D758 File Offset: 0x0010B958
	public bool HasBundleSet()
	{
		return this.m_currentMoneyBundle != null || this.m_currentGoldBundle != null;
	}

	// Token: 0x060036AF RID: 13999 RVA: 0x0010D774 File Offset: 0x0010B974
	public void RegisterCurrentBundleChanged(GeneralStoreContent.BundleChanged dlg)
	{
		this.m_bundleChangedListeners.Add(dlg);
	}

	// Token: 0x060036B0 RID: 14000 RVA: 0x0010D782 File Offset: 0x0010B982
	public void UnregisterCurrentBundleChanged(GeneralStoreContent.BundleChanged dlg)
	{
		this.m_bundleChangedListeners.Remove(dlg);
	}

	// Token: 0x060036B1 RID: 14001 RVA: 0x0010D791 File Offset: 0x0010B991
	public virtual bool AnimateEntranceStart()
	{
		return true;
	}

	// Token: 0x060036B2 RID: 14002 RVA: 0x0010D794 File Offset: 0x0010B994
	public virtual bool AnimateEntranceEnd()
	{
		return true;
	}

	// Token: 0x060036B3 RID: 14003 RVA: 0x0010D797 File Offset: 0x0010B997
	public virtual bool AnimateExitStart()
	{
		return true;
	}

	// Token: 0x060036B4 RID: 14004 RVA: 0x0010D79A File Offset: 0x0010B99A
	public virtual bool AnimateExitEnd()
	{
		return true;
	}

	// Token: 0x060036B5 RID: 14005 RVA: 0x0010D79D File Offset: 0x0010B99D
	public virtual void PreStoreFlipIn()
	{
	}

	// Token: 0x060036B6 RID: 14006 RVA: 0x0010D79F File Offset: 0x0010B99F
	public virtual void PostStoreFlipIn(bool animatedFlipIn)
	{
	}

	// Token: 0x060036B7 RID: 14007 RVA: 0x0010D7A1 File Offset: 0x0010B9A1
	public virtual void PreStoreFlipOut()
	{
	}

	// Token: 0x060036B8 RID: 14008 RVA: 0x0010D7A3 File Offset: 0x0010B9A3
	public virtual void PostStoreFlipOut()
	{
	}

	// Token: 0x060036B9 RID: 14009 RVA: 0x0010D7A5 File Offset: 0x0010B9A5
	public virtual void TryBuyWithMoney(Network.Bundle bundle, GeneralStoreContent.BuyEvent successBuyCB, GeneralStoreContent.BuyEvent failedBuyCB)
	{
		if (successBuyCB != null)
		{
			successBuyCB();
		}
	}

	// Token: 0x060036BA RID: 14010 RVA: 0x0010D7B3 File Offset: 0x0010B9B3
	public virtual void TryBuyWithGold(GeneralStoreContent.BuyEvent successBuyCB = null, GeneralStoreContent.BuyEvent failedBuyCB = null)
	{
		if (successBuyCB != null)
		{
			successBuyCB();
		}
	}

	// Token: 0x060036BB RID: 14011 RVA: 0x0010D7C1 File Offset: 0x0010B9C1
	public virtual void StoreShown(bool isCurrent)
	{
	}

	// Token: 0x060036BC RID: 14012 RVA: 0x0010D7C3 File Offset: 0x0010B9C3
	public virtual void StoreHidden(bool isCurrent)
	{
	}

	// Token: 0x060036BD RID: 14013 RVA: 0x0010D7C5 File Offset: 0x0010B9C5
	public virtual void OnCoverStateChanged(bool coverActive)
	{
	}

	// Token: 0x060036BE RID: 14014 RVA: 0x0010D7C7 File Offset: 0x0010B9C7
	public virtual bool IsPurchaseDisabled()
	{
		return false;
	}

	// Token: 0x060036BF RID: 14015 RVA: 0x0010D7CA File Offset: 0x0010B9CA
	public virtual string GetMoneyDisplayOwnedText()
	{
		return string.Empty;
	}

	// Token: 0x060036C0 RID: 14016 RVA: 0x0010D7D1 File Offset: 0x0010B9D1
	protected virtual void OnBundleChanged(NoGTAPPTransactionData goldBundle, Network.Bundle moneyBundle)
	{
	}

	// Token: 0x060036C1 RID: 14017 RVA: 0x0010D7D3 File Offset: 0x0010B9D3
	protected virtual void OnRefresh()
	{
	}

	// Token: 0x060036C2 RID: 14018 RVA: 0x0010D7D8 File Offset: 0x0010B9D8
	private void FireBundleChangedEvent()
	{
		if (!this.IsContentActive())
		{
			return;
		}
		GeneralStoreContent.BundleChanged[] array = this.m_bundleChangedListeners.ToArray();
		foreach (GeneralStoreContent.BundleChanged bundleChanged in array)
		{
			bundleChanged(this.m_currentGoldBundle, this.m_currentMoneyBundle);
		}
	}

	// Token: 0x04002207 RID: 8711
	protected GeneralStore m_parentStore;

	// Token: 0x04002208 RID: 8712
	protected ProductType m_productType;

	// Token: 0x04002209 RID: 8713
	private bool m_isContentActive;

	// Token: 0x0400220A RID: 8714
	private NoGTAPPTransactionData m_currentGoldBundle;

	// Token: 0x0400220B RID: 8715
	private Network.Bundle m_currentMoneyBundle;

	// Token: 0x0400220C RID: 8716
	private List<GeneralStoreContent.BundleChanged> m_bundleChangedListeners = new List<GeneralStoreContent.BundleChanged>();

	// Token: 0x0200044D RID: 1101
	// (Invoke) Token: 0x060036CA RID: 14026
	public delegate void BuyEvent();

	// Token: 0x02000544 RID: 1348
	// (Invoke) Token: 0x06003E1C RID: 15900
	public delegate void BundleChanged(NoGTAPPTransactionData newGoldBundle, Network.Bundle newMoneyBundle);
}
