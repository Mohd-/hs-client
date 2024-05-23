using System;
using System.Collections.Generic;

// Token: 0x02000442 RID: 1090
[CustomEditClass]
public class AdventurePurchaseScreen : Store
{
	// Token: 0x0600365C RID: 13916 RVA: 0x0010BE98 File Offset: 0x0010A098
	protected override void Awake()
	{
		base.Awake();
		this.m_buyWithMoneyButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.BuyWithMoney();
		});
		this.m_buyWithGoldButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.BuyWithGold();
		});
		this.m_BuyDungeonButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.SendToStore();
		});
	}

	// Token: 0x0600365D RID: 13917 RVA: 0x0010BEF8 File Offset: 0x0010A0F8
	public void AddPurchaseListener(AdventurePurchaseScreen.Purchase dlg, object userdata)
	{
		AdventurePurchaseScreen.PurchaseListener purchaseListener = new AdventurePurchaseScreen.PurchaseListener();
		purchaseListener.SetCallback(dlg);
		purchaseListener.SetUserData(userdata);
		this.m_PurchaseListeners.Add(purchaseListener);
	}

	// Token: 0x0600365E RID: 13918 RVA: 0x0010BF28 File Offset: 0x0010A128
	public void RemovePurchaseListener(AdventurePurchaseScreen.Purchase dlg)
	{
		foreach (AdventurePurchaseScreen.PurchaseListener purchaseListener in this.m_PurchaseListeners)
		{
			if (purchaseListener.GetCallback() == dlg)
			{
				this.m_PurchaseListeners.Remove(purchaseListener);
				break;
			}
		}
	}

	// Token: 0x0600365F RID: 13919 RVA: 0x0010BFA0 File Offset: 0x0010A1A0
	private void BuyWithMoney()
	{
		bool success = true;
		this.FirePurchaseEvent(success);
	}

	// Token: 0x06003660 RID: 13920 RVA: 0x0010BFB8 File Offset: 0x0010A1B8
	private void BuyWithGold()
	{
		bool success = true;
		this.FirePurchaseEvent(success);
	}

	// Token: 0x06003661 RID: 13921 RVA: 0x0010BFD0 File Offset: 0x0010A1D0
	private void SendToStore()
	{
		bool success = false;
		this.FirePurchaseEvent(success);
	}

	// Token: 0x06003662 RID: 13922 RVA: 0x0010BFE8 File Offset: 0x0010A1E8
	private void FirePurchaseEvent(bool success)
	{
		AdventurePurchaseScreen.PurchaseListener[] array = this.m_PurchaseListeners.ToArray();
		foreach (AdventurePurchaseScreen.PurchaseListener purchaseListener in array)
		{
			purchaseListener.Fire(success);
		}
	}

	// Token: 0x040021D6 RID: 8662
	[CustomEditField(Sections = "UI")]
	public PegUIElement m_BuyDungeonButton;

	// Token: 0x040021D7 RID: 8663
	private List<AdventurePurchaseScreen.PurchaseListener> m_PurchaseListeners = new List<AdventurePurchaseScreen.PurchaseListener>();

	// Token: 0x02000443 RID: 1091
	public class PurchaseListener : EventListener<AdventurePurchaseScreen.Purchase>
	{
		// Token: 0x06003667 RID: 13927 RVA: 0x0010C042 File Offset: 0x0010A242
		public void Fire(bool success)
		{
			this.m_callback(success, this.m_userData);
		}
	}

	// Token: 0x02000444 RID: 1092
	// (Invoke) Token: 0x06003669 RID: 13929
	public delegate void Purchase(bool success, object userdata);
}
