using System;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class WebAuth
{
	// Token: 0x06000A90 RID: 2704 RVA: 0x0002EC30 File Offset: 0x0002CE30
	public WebAuth(string url, float x, float y, float width, float height, string gameObjectName)
	{
		Debug.Log("WebAuth");
		this.m_url = url;
		this.m_window = new Rect(x, y, width, height);
		this.m_callbackGameObject = gameObjectName;
		WebAuth.m_isNewCreatedAccount = false;
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x0002EC78 File Offset: 0x0002CE78
	public void Load()
	{
		Debug.Log("Load");
		WebAuth.LoadWebView(this.m_url, this.m_window.x, this.m_window.y, this.m_window.width, this.m_window.height, SystemInfo.deviceUniqueIdentifier, this.m_callbackGameObject);
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x0002ECE8 File Offset: 0x0002CEE8
	public void Close()
	{
		Debug.Log("Close");
		SplashScreen splashScreen = SplashScreen.Get();
		if (splashScreen != null)
		{
			splashScreen.HideWebLoginCanvas();
		}
		WebAuth.CloseWebView();
		FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x0002ED33 File Offset: 0x0002CF33
	public void SetCountryCodeCookie(string countryCode, string domain)
	{
		WebAuth.SetWebViewCountryCodeCookie(countryCode, domain);
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x0002ED3C File Offset: 0x0002CF3C
	public WebAuth.Status GetStatus()
	{
		int webViewStatus = WebAuth.GetWebViewStatus();
		if (webViewStatus < 0 || webViewStatus >= 6)
		{
			return WebAuth.Status.Error;
		}
		return (WebAuth.Status)webViewStatus;
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x0002ED60 File Offset: 0x0002CF60
	public WebAuth.Error GetError()
	{
		return WebAuth.Error.InternalError;
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x0002ED63 File Offset: 0x0002CF63
	public string GetLoginCode()
	{
		Debug.Log("GetLoginCode");
		return WebAuth.GetWebViewLoginCode();
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x0002ED74 File Offset: 0x0002CF74
	public void Show()
	{
		if (this.m_show)
		{
			return;
		}
		this.m_show = true;
		WebAuth.ShowWebView(true);
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x0002ED8F File Offset: 0x0002CF8F
	public bool IsShown()
	{
		return this.m_show;
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x0002ED97 File Offset: 0x0002CF97
	public void Hide()
	{
		if (!this.m_show)
		{
			return;
		}
		this.m_show = false;
		WebAuth.ShowWebView(false);
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x0002EDB2 File Offset: 0x0002CFB2
	public static void ClearLoginData()
	{
		WebAuth.DeleteStoredToken();
		WebAuth.DeleteCookies();
		WebAuth.ClearBrowserCache();
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x0002EDC3 File Offset: 0x0002CFC3
	public static void DeleteCookies()
	{
		WebAuth.ClearWebViewCookies();
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x0002EDCA File Offset: 0x0002CFCA
	public static void ClearBrowserCache()
	{
		WebAuth.ClearURLCache();
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x0002EDD1 File Offset: 0x0002CFD1
	public static string GetStoredToken()
	{
		return WebAuth.GetStoredLoginToken();
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x0002EDD8 File Offset: 0x0002CFD8
	public static bool SetStoredToken(string str)
	{
		return WebAuth.SetStoredLoginToken(str);
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x0002EDE0 File Offset: 0x0002CFE0
	public static void DeleteStoredToken()
	{
		WebAuth.DeleteStoredLoginToken();
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x0002EDE7 File Offset: 0x0002CFE7
	public static void UpdateBreakingNews(string title, string body, string gameObjectName)
	{
		WebAuth.SetBreakingNews(title, body, gameObjectName);
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x0002EDF1 File Offset: 0x0002CFF1
	public static void UpdateRegionSelectVisualState(bool isVisible)
	{
		WebAuth.SetRegionSelectVisualState(isVisible);
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x0002EDF9 File Offset: 0x0002CFF9
	public static void GoBackWebPage()
	{
		WebAuth.GoBack();
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x0002EE00 File Offset: 0x0002D000
	public static bool GetIsNewCreatedAccount()
	{
		return WebAuth.m_isNewCreatedAccount;
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x0002EE07 File Offset: 0x0002D007
	public static void SetIsNewCreatedAccount(bool isNewCreatedAccount)
	{
		WebAuth.m_isNewCreatedAccount = isNewCreatedAccount;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x0002EE0F File Offset: 0x0002D00F
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		this.Close();
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x0002EE17 File Offset: 0x0002D017
	private static void LoadWebView(string str, float x, float y, float width, float height, string deviceUniqueIdentifier, string gameObjectName)
	{
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x0002EE19 File Offset: 0x0002D019
	private static void CloseWebView()
	{
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x0002EE1B File Offset: 0x0002D01B
	private static int GetWebViewStatus()
	{
		return 0;
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x0002EE1E File Offset: 0x0002D01E
	private static void ShowWebView(bool show)
	{
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x0002EE20 File Offset: 0x0002D020
	private static void ClearWebViewCookies()
	{
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x0002EE22 File Offset: 0x0002D022
	private static void ClearURLCache()
	{
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x0002EE24 File Offset: 0x0002D024
	private static string GetWebViewLoginCode()
	{
		return string.Empty;
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x0002EE2B File Offset: 0x0002D02B
	private static string GetStoredLoginToken()
	{
		return string.Empty;
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x0002EE32 File Offset: 0x0002D032
	private static bool SetStoredLoginToken(string str)
	{
		return true;
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x0002EE35 File Offset: 0x0002D035
	private static void DeleteStoredLoginToken()
	{
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0002EE37 File Offset: 0x0002D037
	private static void SetBreakingNews(string localized_title, string body, string gameObjectName)
	{
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x0002EE39 File Offset: 0x0002D039
	private static void SetRegionSelectVisualState(bool isVisible)
	{
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0002EE3B File Offset: 0x0002D03B
	private static void GoBack()
	{
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x0002EE3D File Offset: 0x0002D03D
	private static void SetWebViewCountryCodeCookie(string countryCode, string domain)
	{
	}

	// Token: 0x04000527 RID: 1319
	private string m_url;

	// Token: 0x04000528 RID: 1320
	private Rect m_window;

	// Token: 0x04000529 RID: 1321
	private bool m_show;

	// Token: 0x0400052A RID: 1322
	private string m_callbackGameObject;

	// Token: 0x0400052B RID: 1323
	private static bool m_isNewCreatedAccount;

	// Token: 0x02000890 RID: 2192
	public enum Status
	{
		// Token: 0x040039CE RID: 14798
		Closed,
		// Token: 0x040039CF RID: 14799
		Loading,
		// Token: 0x040039D0 RID: 14800
		ReadyToDisplay,
		// Token: 0x040039D1 RID: 14801
		Processing,
		// Token: 0x040039D2 RID: 14802
		Success,
		// Token: 0x040039D3 RID: 14803
		Error,
		// Token: 0x040039D4 RID: 14804
		MAX
	}

	// Token: 0x02000891 RID: 2193
	public enum Error
	{
		// Token: 0x040039D6 RID: 14806
		InternalError
	}
}
