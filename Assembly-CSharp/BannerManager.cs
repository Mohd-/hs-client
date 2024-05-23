using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class BannerManager
{
	// Token: 0x06002345 RID: 9029 RVA: 0x000AE33D File Offset: 0x000AC53D
	private BannerManager()
	{
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x000AE352 File Offset: 0x000AC552
	public static BannerManager Get()
	{
		if (BannerManager.s_instance == null)
		{
			BannerManager.s_instance = new BannerManager();
		}
		return BannerManager.s_instance;
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x000AE370 File Offset: 0x000AC570
	private int GetDisplayBannerId()
	{
		VarKey varKey = Vars.Key("Events.BannerIdOverride");
		int @int = varKey.GetInt(0);
		if (@int != 0)
		{
			return @int;
		}
		NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
		return (netObject != null) ? netObject.DisplayBanner : 0;
	}

	// Token: 0x06002349 RID: 9033 RVA: 0x000AE3B8 File Offset: 0x000AC5B8
	private bool AcknowledgeBanner(int banner)
	{
		this.m_seenBanners.Add(banner);
		if (banner != this.GetDisplayBannerId() || this.m_bannerWasAcknowledged)
		{
			return false;
		}
		this.m_bannerWasAcknowledged = true;
		NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
		if (netObject != null)
		{
			netObject.DisplayBanner = banner;
			NetCache.Get().NetCacheChanged<NetCache.NetCacheProfileProgress>();
		}
		Network.AcknowledgeBanner(banner);
		return true;
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x000AE41C File Offset: 0x000AC61C
	public bool ShowABanner(BannerManager.DelOnCloseBanner callback = null)
	{
		int displayBannerId = this.GetDisplayBannerId();
		if (this.m_seenBanners.Contains(displayBannerId))
		{
			return false;
		}
		if (displayBannerId == 0)
		{
			return false;
		}
		BannerDbfRecord record = GameDbf.Banner.GetRecord(displayBannerId);
		string text = (record != null) ? FileUtils.GameAssetPathToName(record.Prefab) : null;
		if (record == null || text == null)
		{
			Debug.LogWarning(string.Format("No banner defined for bannerID={0}", displayBannerId));
			return false;
		}
		BannerPopup bannerPopup = GameUtils.LoadGameObjectWithComponent<BannerPopup>(text);
		if (bannerPopup == null)
		{
			return false;
		}
		this.AcknowledgeBanner(displayBannerId);
		bannerPopup.Show(record.Text, callback);
		return true;
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x000AE4C4 File Offset: 0x000AC6C4
	public bool ShowCustomBanner(string bannerAsset, string bannerText, BannerManager.DelOnCloseBanner callback = null)
	{
		BannerPopup bannerPopup = GameUtils.LoadGameObjectWithComponent<BannerPopup>(bannerAsset);
		if (bannerPopup == null)
		{
			return false;
		}
		bannerPopup.Show(bannerText, callback);
		return true;
	}

	// Token: 0x04001465 RID: 5221
	private static BannerManager s_instance;

	// Token: 0x04001466 RID: 5222
	private bool m_bannerWasAcknowledged;

	// Token: 0x04001467 RID: 5223
	private List<int> m_seenBanners = new List<int>();

	// Token: 0x02000274 RID: 628
	// (Invoke) Token: 0x0600234D RID: 9037
	public delegate void DelOnCloseBanner();
}
