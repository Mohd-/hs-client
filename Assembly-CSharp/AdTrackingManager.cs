using System;
using UnityEngine;

// Token: 0x020003A1 RID: 929
public class AdTrackingManager : MonoBehaviour
{
	// Token: 0x060030D7 RID: 12503 RVA: 0x000F6002 File Offset: 0x000F4202
	private void Awake()
	{
		AdTrackingManager.s_Instance = this;
	}

	// Token: 0x060030D8 RID: 12504 RVA: 0x000F600A File Offset: 0x000F420A
	private void OnDestroy()
	{
		AdTrackingManager.s_Instance = null;
	}

	// Token: 0x060030D9 RID: 12505 RVA: 0x000F6012 File Offset: 0x000F4212
	public static AdTrackingManager Get()
	{
		return AdTrackingManager.s_Instance;
	}

	// Token: 0x060030DA RID: 12506 RVA: 0x000F6019 File Offset: 0x000F4219
	public void TrackLogin()
	{
	}

	// Token: 0x060030DB RID: 12507 RVA: 0x000F601B File Offset: 0x000F421B
	public void TrackFirstLogin()
	{
	}

	// Token: 0x060030DC RID: 12508 RVA: 0x000F601D File Offset: 0x000F421D
	public void TrackAccountCreated()
	{
	}

	// Token: 0x060030DD RID: 12509 RVA: 0x000F601F File Offset: 0x000F421F
	public void TrackAdventureProgress(string description)
	{
	}

	// Token: 0x060030DE RID: 12510 RVA: 0x000F6021 File Offset: 0x000F4221
	public void TrackTutorialProgress(string description)
	{
	}

	// Token: 0x060030DF RID: 12511 RVA: 0x000F6023 File Offset: 0x000F4223
	public void TrackSale(string price, string currencyCode, string productId, string transactionId)
	{
	}

	// Token: 0x060030E0 RID: 12512 RVA: 0x000F6025 File Offset: 0x000F4225
	public void TrackCreditsLaunch()
	{
	}

	// Token: 0x060030E1 RID: 12513 RVA: 0x000F6027 File Offset: 0x000F4227
	public void GetDeepLink()
	{
	}

	// Token: 0x04001E7B RID: 7803
	private static AdTrackingManager s_Instance;
}
