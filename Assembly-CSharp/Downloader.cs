using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002DB RID: 731
public class Downloader : MonoBehaviour
{
	// Token: 0x06002664 RID: 9828 RVA: 0x000BB3F8 File Offset: 0x000B95F8
	private void Awake()
	{
		Downloader.s_instance = this;
		this.m_bundlesToDownload = new List<string>();
		this.m_downloadableBundles = new Map<string, AssetBundle>();
		if (ApplicationMgr.GetMobileEnvironment() == MobileEnv.PRODUCTION)
		{
			Downloader.m_remoteUri = "http://dist.blizzard.com/hs-pod/dlc/";
			Downloader.m_remoteUriCN = "http://client{0}.pdl.battlenet.com.cn/hs-pod/dlc/";
		}
		else
		{
			Downloader.m_remoteUri = "http://streaming-t5.corp.blizzard.net/hearthstone/dlc/";
		}
		Downloader.m_remoteUri += "win/";
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
	}

	// Token: 0x06002665 RID: 9829 RVA: 0x000BB479 File Offset: 0x000B9679
	private void OnDestroy()
	{
		Downloader.s_instance = null;
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x000BB481 File Offset: 0x000B9681
	private void Start()
	{
		if (AssetLoader.DOWNLOADABLE_LANGUAGE_PACKS)
		{
			this.DownloadLocalizedBundles();
		}
		else
		{
			Log.Downloader.Print("Not downloading language bundles (DOWNLOADABLE_LANGUAGE_PACKS=false)", new object[0]);
		}
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000BB4B2 File Offset: 0x000B96B2
	public static Downloader Get()
	{
		return Downloader.s_instance;
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x000BB4B9 File Offset: 0x000B96B9
	public bool AllLocalizedAudioBundlesDownloaded()
	{
		return this.m_isDownloading || this.BundlesMissing().Count == 0;
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x000BB4D6 File Offset: 0x000B96D6
	public void DownloadLocalizedBundles()
	{
		this.DownloadLocalizedBundles(this.BundlesToDownloadForLocale());
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x000BB4E4 File Offset: 0x000B96E4
	private void DownloadLocalizedBundles(List<string> bundlesToDownload)
	{
		if (bundlesToDownload == null || bundlesToDownload.Count <= 0 || this.m_isDownloading)
		{
			return;
		}
		this.m_isDownloading = true;
		this.m_downloadStartTime = Time.realtimeSinceStartup;
		Log.Downloader.Print("Starting to load or download localized bundles at " + this.m_downloadStartTime, new object[0]);
		this.m_bundlesToDownload = bundlesToDownload;
		this.DownloadNextFile();
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x000BB554 File Offset: 0x000B9754
	public void DeleteLocalizedBundles()
	{
		foreach (AssetBundle assetBundle in this.m_downloadableBundles.Values)
		{
			assetBundle.Unload(true);
		}
		Caching.CleanCache();
		this.m_downloadableBundles.Clear();
		Debug.Log("Cleared cache of downloadable bundles.");
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x000BB5D0 File Offset: 0x000B97D0
	public AssetBundle GetDownloadedBundle(string fileName)
	{
		AssetBundle assetBundle = null;
		this.m_downloadableBundles.TryGetValue(fileName, out assetBundle);
		if (assetBundle == null)
		{
			Debug.Log(string.Format("Attempted to load bundle {0} but not available yet.", fileName));
		}
		return assetBundle;
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x000BB60C File Offset: 0x000B980C
	public static int GetTryCount()
	{
		return (MobileDeviceLocale.GetCurrentRegionId() != 5) ? 1 : 4;
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x000BB630 File Offset: 0x000B9830
	public static string BundleURL(string fileName)
	{
		string text = Downloader.m_remoteUri;
		if (MobileDeviceLocale.GetCurrentRegionId() == 5 && Downloader.m_remoteUriCNIndex != Downloader.GetTryCount() && Downloader.m_remoteUriCN != null)
		{
			text = string.Format(Downloader.m_remoteUriCN, "0" + Downloader.m_remoteUriCNIndex.ToString());
		}
		return string.Format("{0}{1}/{2}", text, DownloadManifest.Get().HashForBundle(fileName), fileName);
	}

	// Token: 0x0600266F RID: 9839 RVA: 0x000BB6A0 File Offset: 0x000B98A0
	public static string NextBundleURL(string fileName)
	{
		if (MobileDeviceLocale.GetCurrentRegionId() == 5)
		{
			Downloader.m_remoteUriCNIndex = ((Downloader.m_remoteUriCNIndex != Downloader.GetTryCount()) ? (Downloader.m_remoteUriCNIndex + 1) : 1);
		}
		return Downloader.BundleURL(fileName);
	}

	// Token: 0x06002670 RID: 9840 RVA: 0x000BB6E0 File Offset: 0x000B98E0
	private void WillReset()
	{
		base.StopAllCoroutines();
		this.m_isDownloading = false;
		this.m_hasShownInternalFailureAlert = false;
		foreach (AssetBundle assetBundle in this.m_downloadableBundles.Values)
		{
			if (assetBundle != null)
			{
				assetBundle.Unload(true);
			}
		}
		this.m_downloadableBundles.Clear();
		if (AssetLoader.DOWNLOADABLE_LANGUAGE_PACKS)
		{
			this.DownloadLocalizedBundles();
		}
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000BB780 File Offset: 0x000B9980
	private List<string> BundlesToDownloadForLocale()
	{
		List<string> list = new List<string>();
		foreach (Locale locale in Localization.GetLoadOrder(false))
		{
			if (locale != Locale.enUS && locale != Locale.enGB)
			{
				foreach (AssetFamilyBundleInfo assetFamilyBundleInfo in AssetBundleInfo.FamilyInfo.Values)
				{
					for (int j = 0; j < assetFamilyBundleInfo.NumberOfDownloadableLocaleBundles; j++)
					{
						string text = string.Format("{0}{1}_dlc_{2}.unity3d", assetFamilyBundleInfo.BundleName, locale, j);
						list.Add(text);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x000BB858 File Offset: 0x000B9A58
	private void DownloadNextFile()
	{
		if (this.m_bundlesToDownload.Count > 0)
		{
			string text = this.m_bundlesToDownload[0];
			string text2 = DownloadManifest.Get().DownloadableBundleFileName(text);
			if (text2 == null)
			{
				string message = string.Format("Downloader.DownloadNextFile() - Attempting to download bundle not listed in manifest.  No hash found for bundle {0}", text);
				Error.AddDevFatal(message, new object[0]);
				this.m_bundlesToDownload.RemoveAt(0);
				this.DownloadNextFile();
				return;
			}
			base.StartCoroutine(this.DownloadAndCache(text2));
		}
		else
		{
			Log.Downloader.Print("Finished downloading or loading all bundles - duration: " + (Time.realtimeSinceStartup - this.m_downloadStartTime), new object[0]);
			AssetCache.ClearAllCachesFailedRequests();
			this.m_isDownloading = false;
		}
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000BB90C File Offset: 0x000B9B0C
	private IEnumerator DownloadAndCache(string fileName)
	{
		while (!Caching.ready)
		{
			yield return null;
		}
		float startTime = Time.realtimeSinceStartup;
		string version = "5.0".Replace(".", string.Empty);
		int assetVersion = 2;
		try
		{
			assetVersion = Convert.ToInt32(version);
		}
		catch (Exception)
		{
			Debug.LogWarning("Could not convert cosmeticVersion to an int!");
		}
		Log.Downloader.Print("Downloader asset version: {0}", new object[]
		{
			assetVersion
		});
		for (int tryCount = 1; tryCount <= Downloader.GetTryCount(); tryCount++)
		{
			string webResource = Downloader.BundleURL(fileName);
			Debug.Log(string.Format("Loading or downloading file {0} from {1}", fileName, webResource));
			using (WWW bundleRequest = WWW.LoadFromCacheOrDownload(webResource, assetVersion))
			{
				Log.Downloader.Print(string.Format("Started downloading and caching url {0} - duration: {1}", webResource, Time.realtimeSinceStartup - startTime), new object[0]);
				yield return bundleRequest;
				if (bundleRequest.error == null)
				{
					Log.Downloader.Print(string.Format("Finished downloading and caching url {0} - duration: {1}", webResource, Time.realtimeSinceStartup - startTime), new object[0]);
					startTime = Time.realtimeSinceStartup;
					this.m_downloadableBundles.Add(fileName, bundleRequest.assetBundle);
					Log.Downloader.Print(string.Format("Finished loading asset bundle {0} - duration: {1}", webResource, Time.realtimeSinceStartup - startTime), new object[0]);
					startTime = Time.realtimeSinceStartup;
					break;
				}
				Debug.LogError(string.Format("DownloadAndCache - Error when downloading url {0} - error: {1}", webResource, bundleRequest.error));
				if (ApplicationMgr.IsInternal() && DialogManager.Get() != null && !this.m_hasShownInternalFailureAlert && Localization.GetLocale() != Locale.jaJP)
				{
					AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
					info.m_headerText = "Language Pack Download Failed";
					info.m_text = string.Format("Failed to download bundle {0} with error {1}.  Some sounds may not play.  Try again by relaunching the game.", webResource, bundleRequest.error);
					info.m_showAlertIcon = true;
					info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
					DialogManager.Get().ShowPopup(info);
					this.m_hasShownInternalFailureAlert = true;
				}
				webResource = Downloader.NextBundleURL(fileName);
			}
		}
		Log.Downloader.Print(string.Format("Finished closing WWW for bundle {0} - duration: {1}", fileName, Time.realtimeSinceStartup - startTime), new object[0]);
		this.m_bundlesToDownload.RemoveAt(0);
		this.DownloadNextFile();
		yield break;
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000BB938 File Offset: 0x000B9B38
	private List<string> BundlesMissing()
	{
		List<string> list = this.BundlesToDownloadForLocale();
		List<string> list2 = new List<string>();
		foreach (string text in list)
		{
			if (!this.m_downloadableBundles.ContainsKey(text))
			{
				list2.Add(text);
			}
		}
		return list2;
	}

	// Token: 0x040016ED RID: 5869
	private static Downloader s_instance;

	// Token: 0x040016EE RID: 5870
	private List<string> m_bundlesToDownload;

	// Token: 0x040016EF RID: 5871
	private static string m_remoteUri;

	// Token: 0x040016F0 RID: 5872
	private static string m_remoteUriCN;

	// Token: 0x040016F1 RID: 5873
	private static int m_remoteUriCNIndex = 1;

	// Token: 0x040016F2 RID: 5874
	private bool m_isDownloading;

	// Token: 0x040016F3 RID: 5875
	private Map<string, AssetBundle> m_downloadableBundles;

	// Token: 0x040016F4 RID: 5876
	private float m_downloadStartTime;

	// Token: 0x040016F5 RID: 5877
	private bool m_hasShownInternalFailureAlert;
}
