using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Facebook;
using UnityEngine;

// Token: 0x02000B10 RID: 2832
public sealed class FB : ScriptableObject
{
	// Token: 0x1700090A RID: 2314
	// (get) Token: 0x060060EF RID: 24815 RVA: 0x001D023E File Offset: 0x001CE43E
	private static IFacebook FacebookImpl
	{
		get
		{
			if (FB.facebook == null)
			{
				throw new NullReferenceException("Facebook object is not yet loaded.  Did you call FB.Init()?");
			}
			return FB.facebook;
		}
	}

	// Token: 0x1700090B RID: 2315
	// (get) Token: 0x060060F0 RID: 24816 RVA: 0x001D025A File Offset: 0x001CE45A
	public static string AppId
	{
		get
		{
			return FB.appId;
		}
	}

	// Token: 0x1700090C RID: 2316
	// (get) Token: 0x060060F1 RID: 24817 RVA: 0x001D0261 File Offset: 0x001CE461
	public static string UserId
	{
		get
		{
			return (FB.facebook == null) ? string.Empty : FB.facebook.UserId;
		}
	}

	// Token: 0x1700090D RID: 2317
	// (get) Token: 0x060060F2 RID: 24818 RVA: 0x001D0281 File Offset: 0x001CE481
	public static string AccessToken
	{
		get
		{
			return (FB.facebook == null) ? string.Empty : FB.facebook.AccessToken;
		}
	}

	// Token: 0x1700090E RID: 2318
	// (get) Token: 0x060060F3 RID: 24819 RVA: 0x001D02A1 File Offset: 0x001CE4A1
	public static DateTime AccessTokenExpiresAt
	{
		get
		{
			return (FB.facebook == null) ? DateTime.MinValue : FB.facebook.AccessTokenExpiresAt;
		}
	}

	// Token: 0x1700090F RID: 2319
	// (get) Token: 0x060060F4 RID: 24820 RVA: 0x001D02C1 File Offset: 0x001CE4C1
	public static bool IsLoggedIn
	{
		get
		{
			return FB.facebook != null && FB.facebook.IsLoggedIn;
		}
	}

	// Token: 0x17000910 RID: 2320
	// (get) Token: 0x060060F5 RID: 24821 RVA: 0x001D02DA File Offset: 0x001CE4DA
	public static bool IsInitialized
	{
		get
		{
			return FB.facebook != null && FB.facebook.IsInitialized;
		}
	}

	// Token: 0x060060F6 RID: 24822 RVA: 0x001D02F4 File Offset: 0x001CE4F4
	public static void Init(InitDelegate onInitComplete, HideUnityDelegate onHideUnity = null, string authResponse = null)
	{
		FB.Init(onInitComplete, FBSettings.AppId, FBSettings.Cookie, FBSettings.Logging, FBSettings.Status, FBSettings.Xfbml, FBSettings.FrictionlessRequests, onHideUnity, authResponse);
	}

	// Token: 0x060060F7 RID: 24823 RVA: 0x001D0328 File Offset: 0x001CE528
	public static void Init(InitDelegate onInitComplete, string appId, bool cookie = true, bool logging = true, bool status = true, bool xfbml = false, bool frictionlessRequests = true, HideUnityDelegate onHideUnity = null, string authResponse = null)
	{
		FB.appId = appId;
		FB.cookie = cookie;
		FB.logging = logging;
		FB.status = status;
		FB.xfbml = xfbml;
		FB.frictionlessRequests = frictionlessRequests;
		FB.authResponse = authResponse;
		FB.OnInitComplete = onInitComplete;
		FB.OnHideUnity = onHideUnity;
		if (!FB.isInitCalled)
		{
			FBBuildVersionAttribute versionAttributeOfType = FBBuildVersionAttribute.GetVersionAttributeOfType(typeof(IFacebook));
			if (versionAttributeOfType == null)
			{
				FbDebugOverride.Warn("Cannot find Facebook SDK Version");
			}
			else
			{
				FbDebugOverride.Info(string.Format("Using SDK {0}, Build {1}", versionAttributeOfType.SdkVersion, versionAttributeOfType.BuildVersion));
			}
			throw new NotImplementedException("Facebook API does not yet support this platform");
		}
		FbDebugOverride.Warn("FB.Init() has already been called.  You only need to call this once and only once.");
		if (FB.FacebookImpl != null)
		{
			FB.OnDllLoaded();
		}
	}

	// Token: 0x060060F8 RID: 24824 RVA: 0x001D03E0 File Offset: 0x001CE5E0
	private static void OnDllLoaded()
	{
		FBBuildVersionAttribute versionAttributeOfType = FBBuildVersionAttribute.GetVersionAttributeOfType(FB.FacebookImpl.GetType());
		if (versionAttributeOfType != null)
		{
			FbDebugOverride.Log(string.Format("Finished loading Facebook dll. Version {0} Build {1}", versionAttributeOfType.SdkVersion, versionAttributeOfType.BuildVersion));
		}
		FB.FacebookImpl.Init(FB.OnInitComplete, FB.appId, FB.cookie, FB.logging, FB.status, FB.xfbml, FBSettings.ChannelUrl, FB.authResponse, FB.frictionlessRequests, FB.OnHideUnity);
	}

	// Token: 0x060060F9 RID: 24825 RVA: 0x001D045A File Offset: 0x001CE65A
	public static void Login(string scope = "", FacebookDelegate callback = null)
	{
		FB.FacebookImpl.Login(scope, callback);
	}

	// Token: 0x060060FA RID: 24826 RVA: 0x001D0468 File Offset: 0x001CE668
	public static void Logout()
	{
		FB.FacebookImpl.Logout();
	}

	// Token: 0x060060FB RID: 24827 RVA: 0x001D0474 File Offset: 0x001CE674
	public static void AppRequest(string message, OGActionType actionType, string objectId, string[] to, string data = "", string title = "", FacebookDelegate callback = null)
	{
		FB.FacebookImpl.AppRequest(message, actionType, objectId, to, null, null, default(int?), data, title, callback);
	}

	// Token: 0x060060FC RID: 24828 RVA: 0x001D04A0 File Offset: 0x001CE6A0
	public static void AppRequest(string message, OGActionType actionType, string objectId, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null)
	{
		FB.FacebookImpl.AppRequest(message, actionType, objectId, null, filters, excludeIds, maxRecipients, data, title, callback);
	}

	// Token: 0x060060FD RID: 24829 RVA: 0x001D04C8 File Offset: 0x001CE6C8
	public static void AppRequest(string message, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null)
	{
		FB.FacebookImpl.AppRequest(message, null, null, to, filters, excludeIds, maxRecipients, data, title, callback);
	}

	// Token: 0x060060FE RID: 24830 RVA: 0x001D04F0 File Offset: 0x001CE6F0
	public static void Feed(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string mediaSource = "", string actionName = "", string actionLink = "", string reference = "", Dictionary<string, string[]> properties = null, FacebookDelegate callback = null)
	{
		FB.FacebookImpl.FeedRequest(toId, link, linkName, linkCaption, linkDescription, picture, mediaSource, actionName, actionLink, reference, properties, callback);
	}

	// Token: 0x060060FF RID: 24831 RVA: 0x001D051B File Offset: 0x001CE71B
	public static void API(string query, HttpMethod method, FacebookDelegate callback = null, Dictionary<string, string> formData = null)
	{
		FB.FacebookImpl.API(query, method, formData, callback);
	}

	// Token: 0x06006100 RID: 24832 RVA: 0x001D052B File Offset: 0x001CE72B
	public static void API(string query, HttpMethod method, FacebookDelegate callback, WWWForm formData)
	{
		FB.FacebookImpl.API(query, method, formData, callback);
	}

	// Token: 0x06006101 RID: 24833 RVA: 0x001D053B File Offset: 0x001CE73B
	[Obsolete("use FB.ActivateApp()")]
	public static void PublishInstall(FacebookDelegate callback = null)
	{
		FB.FacebookImpl.PublishInstall(FB.AppId, callback);
	}

	// Token: 0x06006102 RID: 24834 RVA: 0x001D054D File Offset: 0x001CE74D
	public static void ActivateApp()
	{
		FB.FacebookImpl.ActivateApp(FB.AppId);
	}

	// Token: 0x06006103 RID: 24835 RVA: 0x001D055E File Offset: 0x001CE75E
	public static void GetDeepLink(FacebookDelegate callback)
	{
		FB.FacebookImpl.GetDeepLink(callback);
	}

	// Token: 0x06006104 RID: 24836 RVA: 0x001D056B File Offset: 0x001CE76B
	public static void GameGroupCreate(string name, string description, string privacy = "CLOSED", FacebookDelegate callback = null)
	{
		FB.FacebookImpl.GameGroupCreate(name, description, privacy, callback);
	}

	// Token: 0x06006105 RID: 24837 RVA: 0x001D057B File Offset: 0x001CE77B
	public static void GameGroupJoin(string id, FacebookDelegate callback = null)
	{
		FB.FacebookImpl.GameGroupJoin(id, callback);
	}

	// Token: 0x04004874 RID: 18548
	public static InitDelegate OnInitComplete;

	// Token: 0x04004875 RID: 18549
	public static HideUnityDelegate OnHideUnity;

	// Token: 0x04004876 RID: 18550
	private static IFacebook facebook;

	// Token: 0x04004877 RID: 18551
	private static string authResponse;

	// Token: 0x04004878 RID: 18552
	private static bool isInitCalled;

	// Token: 0x04004879 RID: 18553
	private static string appId;

	// Token: 0x0400487A RID: 18554
	private static bool cookie;

	// Token: 0x0400487B RID: 18555
	private static bool logging;

	// Token: 0x0400487C RID: 18556
	private static bool status;

	// Token: 0x0400487D RID: 18557
	private static bool xfbml;

	// Token: 0x0400487E RID: 18558
	private static bool frictionlessRequests;

	// Token: 0x02000B11 RID: 2833
	public abstract class CompiledFacebookLoader : MonoBehaviour
	{
		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x06006107 RID: 24839
		protected abstract IFacebook fb { get; }

		// Token: 0x06006108 RID: 24840 RVA: 0x001D0591 File Offset: 0x001CE791
		private void Start()
		{
			FB.facebook = this.fb;
			FB.OnDllLoaded();
			Object.Destroy(this);
		}
	}

	// Token: 0x02000B12 RID: 2834
	public abstract class RemoteFacebookLoader : MonoBehaviour
	{
		// Token: 0x0600610B RID: 24843 RVA: 0x001D05B4 File Offset: 0x001CE7B4
		public static IEnumerator LoadFacebookClass(string className, FB.RemoteFacebookLoader.LoadedDllCallback callback)
		{
			string url = string.Format(IntegratedPluginCanvasLocation.DllUrl, className);
			WWW www = new WWW(url);
			FbDebugOverride.Log("loading dll: " + url);
			yield return www;
			if (www.error != null)
			{
				FbDebugOverride.Error(www.error);
				if (FB.RemoteFacebookLoader.retryLoadCount < 3)
				{
					FB.RemoteFacebookLoader.retryLoadCount++;
				}
				www.Dispose();
				yield break;
			}
			WWW authTokenWww = new WWW(IntegratedPluginCanvasLocation.KeyUrl);
			yield return authTokenWww;
			if (authTokenWww.error != null)
			{
				FbDebugOverride.Error("Cannot load from " + IntegratedPluginCanvasLocation.KeyUrl + ": " + authTokenWww.error);
				authTokenWww.Dispose();
				yield break;
			}
			Assembly assembly = Security.LoadAndVerifyAssembly(www.bytes, authTokenWww.text);
			if (assembly == null)
			{
				FbDebugOverride.Error("Could not securely load assembly from " + url);
				www.Dispose();
				yield break;
			}
			Type facebookClass = assembly.GetType("Facebook." + className);
			if (facebookClass == null)
			{
				FbDebugOverride.Error(className + " not found in assembly!");
				www.Dispose();
				yield break;
			}
			IFacebook fb = typeof(FBComponentFactory).GetMethod("GetComponent").MakeGenericMethod(new Type[]
			{
				facebookClass
			}).Invoke(null, new object[]
			{
				0
			}) as IFacebook;
			if (fb == null)
			{
				FbDebugOverride.Error(className + " couldn't be created.");
				www.Dispose();
				yield break;
			}
			callback(fb);
			www.Dispose();
			yield break;
		}

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x0600610C RID: 24844
		protected abstract string className { get; }

		// Token: 0x0600610D RID: 24845 RVA: 0x001D05E4 File Offset: 0x001CE7E4
		private IEnumerator Start()
		{
			IEnumerator loader = FB.RemoteFacebookLoader.LoadFacebookClass(this.className, new FB.RemoteFacebookLoader.LoadedDllCallback(this.OnDllLoaded));
			while (loader.MoveNext())
			{
				object obj = loader.Current;
				yield return obj;
			}
			Object.Destroy(this);
			yield break;
		}

		// Token: 0x0600610E RID: 24846 RVA: 0x001D05FF File Offset: 0x001CE7FF
		private void OnDllLoaded(IFacebook fb)
		{
			FB.facebook = fb;
			FB.OnDllLoaded();
		}

		// Token: 0x0400487F RID: 18559
		private const string facebookNamespace = "Facebook.";

		// Token: 0x04004880 RID: 18560
		private const int maxRetryLoadCount = 3;

		// Token: 0x04004881 RID: 18561
		private static int retryLoadCount;

		// Token: 0x02000B13 RID: 2835
		// (Invoke) Token: 0x06006110 RID: 24848
		public delegate void LoadedDllCallback(IFacebook fb);
	}

	// Token: 0x02000B17 RID: 2839
	public sealed class AppEvents
	{
		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x06006125 RID: 24869 RVA: 0x001D09CE File Offset: 0x001CEBCE
		// (set) Token: 0x06006126 RID: 24870 RVA: 0x001D09E7 File Offset: 0x001CEBE7
		public static bool LimitEventUsage
		{
			get
			{
				return FB.facebook != null && FB.facebook.LimitEventUsage;
			}
			set
			{
				FB.facebook.LimitEventUsage = value;
			}
		}

		// Token: 0x06006127 RID: 24871 RVA: 0x001D09F4 File Offset: 0x001CEBF4
		public static void LogEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
		{
			FB.FacebookImpl.AppEventsLogEvent(logEvent, valueToSum, parameters);
		}

		// Token: 0x06006128 RID: 24872 RVA: 0x001D0A03 File Offset: 0x001CEC03
		public static void LogPurchase(float logPurchase, string currency = "USD", Dictionary<string, object> parameters = null)
		{
			FB.FacebookImpl.AppEventsLogPurchase(logPurchase, currency, parameters);
		}
	}

	// Token: 0x02000B18 RID: 2840
	public sealed class Canvas
	{
		// Token: 0x0600612A RID: 24874 RVA: 0x001D0A1C File Offset: 0x001CEC1C
		public static void Pay(string product, string action = "purchaseitem", int quantity = 1, int? quantityMin = null, int? quantityMax = null, string requestId = null, string pricepointId = null, string testCurrency = null, FacebookDelegate callback = null)
		{
			FB.FacebookImpl.Pay(product, action, quantity, quantityMin, quantityMax, requestId, pricepointId, testCurrency, callback);
		}

		// Token: 0x0600612B RID: 24875 RVA: 0x001D0A41 File Offset: 0x001CEC41
		public static void SetResolution(int width, int height, bool fullscreen, int preferredRefreshRate = 0, params FBScreen.Layout[] layoutParams)
		{
			FBScreen.SetResolution(width, height, fullscreen, preferredRefreshRate, layoutParams);
		}

		// Token: 0x0600612C RID: 24876 RVA: 0x001D0A4E File Offset: 0x001CEC4E
		public static void SetAspectRatio(int width, int height, params FBScreen.Layout[] layoutParams)
		{
			FBScreen.SetAspectRatio(width, height, layoutParams);
		}
	}

	// Token: 0x02000B1B RID: 2843
	public sealed class Android
	{
		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x0600613E RID: 24894 RVA: 0x001D0B18 File Offset: 0x001CED18
		public static string KeyHash
		{
			get
			{
				AndroidFacebook androidFacebook = FB.facebook as AndroidFacebook;
				return (!(androidFacebook != null)) ? string.Empty : androidFacebook.KeyHash;
			}
		}
	}
}
