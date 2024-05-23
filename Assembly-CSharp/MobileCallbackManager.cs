using System;
using UnityEngine;

// Token: 0x02000418 RID: 1048
public class MobileCallbackManager : MonoBehaviour
{
	// Token: 0x060035A2 RID: 13730 RVA: 0x0010ABFC File Offset: 0x00108DFC
	private void Awake()
	{
		MobileCallbackManager.s_Instance = this;
	}

	// Token: 0x060035A3 RID: 13731 RVA: 0x0010AC04 File Offset: 0x00108E04
	private void OnDestroy()
	{
		MobileCallbackManager.s_Instance = null;
	}

	// Token: 0x060035A4 RID: 13732 RVA: 0x0010AC0C File Offset: 0x00108E0C
	public static MobileCallbackManager Get()
	{
		return MobileCallbackManager.s_Instance;
	}

	// Token: 0x060035A5 RID: 13733 RVA: 0x0010AC13 File Offset: 0x00108E13
	public static bool IsAndroidDeviceTabletSized()
	{
		return true;
	}

	// Token: 0x060035A6 RID: 13734 RVA: 0x0010AC16 File Offset: 0x00108E16
	public static void RegisterPushNotifications()
	{
		Log.Yim.Print("MobileCallbackManager - RegisterPushNotifications()", new object[0]);
		MobileCallbackManager.RegisterForPushNotifications();
	}

	// Token: 0x060035A7 RID: 13735 RVA: 0x0010AC34 File Offset: 0x00108E34
	public static void SetRegionAndLocale(int gameRegion, string gameLocale)
	{
		Log.Yim.Print(string.Concat(new object[]
		{
			"SetRegionAndLocale(",
			gameRegion,
			", ",
			gameLocale,
			")"
		}), new object[0]);
		MobileCallbackManager.SetBattleNetRegionAndGameLocale(gameRegion, gameLocale);
	}

	// Token: 0x060035A8 RID: 13736 RVA: 0x0010AC88 File Offset: 0x00108E88
	private static bool IsDevice(string deviceModel)
	{
		return false;
	}

	// Token: 0x060035A9 RID: 13737 RVA: 0x0010AC8B File Offset: 0x00108E8B
	public static uint GetMemoryUsage()
	{
		return Profiler.GetTotalAllocatedMemory();
	}

	// Token: 0x060035AA RID: 13738 RVA: 0x0010AC92 File Offset: 0x00108E92
	public static bool AreMotionEffectsEnabled()
	{
		return true;
	}

	// Token: 0x060035AB RID: 13739 RVA: 0x0010AC95 File Offset: 0x00108E95
	private static void RegisterForPushNotifications()
	{
	}

	// Token: 0x060035AC RID: 13740 RVA: 0x0010AC97 File Offset: 0x00108E97
	private static void SetBattleNetRegionAndGameLocale(int gameRegion, string gameLocale)
	{
	}

	// Token: 0x04002186 RID: 8582
	private const string CHINESE_CURRENCY_CODE = "CNY";

	// Token: 0x04002187 RID: 8583
	private const string CHINESE_COUNTRY_CODE = "CN";

	// Token: 0x04002188 RID: 8584
	private static MobileCallbackManager s_Instance;

	// Token: 0x04002189 RID: 8585
	public bool m_wasBreakingNewsShown;
}
