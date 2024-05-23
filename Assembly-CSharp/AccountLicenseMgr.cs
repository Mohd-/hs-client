using System;
using System.Collections;
using System.Collections.Generic;
using PegasusShared;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class AccountLicenseMgr
{
	// Token: 0x06001253 RID: 4691 RVA: 0x0004F76C File Offset: 0x0004D96C
	public static AccountLicenseMgr Get()
	{
		if (AccountLicenseMgr.s_instance == null)
		{
			AccountLicenseMgr.s_instance = new AccountLicenseMgr();
			ApplicationMgr.Get().WillReset += new Action(AccountLicenseMgr.s_instance.WillReset);
		}
		if (!AccountLicenseMgr.s_instance.m_registeredForProfileNotices)
		{
			NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(AccountLicenseMgr.s_instance.OnNewNotices));
			AccountLicenseMgr.s_instance.m_registeredForProfileNotices = true;
		}
		return AccountLicenseMgr.s_instance;
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x0004F7E0 File Offset: 0x0004D9E0
	public bool OwnsAccountLicense(long license)
	{
		NetCache.NetCacheAccountLicenses netObject = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>();
		return netObject != null && netObject.AccountLicenses.ContainsKey(license) && this.OwnsAccountLicense(netObject.AccountLicenses[license]);
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x0004F828 File Offset: 0x0004DA28
	public bool OwnsAccountLicense(AccountLicenseInfo accountLicenseInfo)
	{
		return accountLicenseInfo != null && (accountLicenseInfo.Flags_ & 1UL) == 1UL;
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x0004F84C File Offset: 0x0004DA4C
	public List<AccountLicenseInfo> GetAllOwnedAccountLicenseInfo()
	{
		List<AccountLicenseInfo> list = new List<AccountLicenseInfo>();
		NetCache.NetCacheAccountLicenses netObject = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>();
		if (netObject != null)
		{
			foreach (AccountLicenseInfo accountLicenseInfo in netObject.AccountLicenses.Values)
			{
				if (this.OwnsAccountLicense(accountLicenseInfo))
				{
					list.Add(accountLicenseInfo);
				}
			}
		}
		return list;
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x0004F8D4 File Offset: 0x0004DAD4
	public bool RegisterAccountLicensesChangedListener(AccountLicenseMgr.AccountLicensesChangedCallback callback)
	{
		return this.RegisterAccountLicensesChangedListener(callback, null);
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x0004F8E0 File Offset: 0x0004DAE0
	public bool RegisterAccountLicensesChangedListener(AccountLicenseMgr.AccountLicensesChangedCallback callback, object userData)
	{
		AccountLicenseMgr.AccountLicensesChangedListener accountLicensesChangedListener = new AccountLicenseMgr.AccountLicensesChangedListener();
		accountLicensesChangedListener.SetCallback(callback);
		accountLicensesChangedListener.SetUserData(userData);
		if (this.m_accountLicensesChangedListeners.Contains(accountLicensesChangedListener))
		{
			return false;
		}
		this.m_accountLicensesChangedListeners.Add(accountLicensesChangedListener);
		return true;
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x0004F921 File Offset: 0x0004DB21
	public bool RemoveAccountLicensesChangedListener(AccountLicenseMgr.AccountLicensesChangedCallback callback)
	{
		return this.RemoveAccountLicensesChangedListener(callback, null);
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x0004F92C File Offset: 0x0004DB2C
	public bool RemoveAccountLicensesChangedListener(AccountLicenseMgr.AccountLicensesChangedCallback callback, object userData)
	{
		AccountLicenseMgr.AccountLicensesChangedListener accountLicensesChangedListener = new AccountLicenseMgr.AccountLicensesChangedListener();
		accountLicensesChangedListener.SetCallback(callback);
		accountLicensesChangedListener.SetUserData(userData);
		return this.m_accountLicensesChangedListeners.Remove(accountLicensesChangedListener);
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x0004F959 File Offset: 0x0004DB59
	private void WillReset()
	{
		if (this.m_seenLicenceNotices != null)
		{
			this.m_seenLicenceNotices.Clear();
		}
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x0004F974 File Offset: 0x0004DB74
	private void OnNewNotices(List<NetCache.ProfileNotice> newNotices)
	{
		NetCache.NetCacheAccountLicenses netObject = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>();
		if (netObject == null)
		{
			ApplicationMgr.Get().StartCoroutine(this.OnNewNotices_WaitForNetCacheAccountLicenses(newNotices));
			return;
		}
		this.OnNewNotices_Internal(newNotices, netObject);
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x0004F9B0 File Offset: 0x0004DBB0
	private IEnumerator OnNewNotices_WaitForNetCacheAccountLicenses(List<NetCache.ProfileNotice> newNotices)
	{
		float startTime = Time.realtimeSinceStartup;
		NetCache.NetCacheAccountLicenses licenses = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>();
		while (licenses == null && Time.realtimeSinceStartup - startTime < 30f)
		{
			yield return null;
			licenses = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>();
		}
		this.OnNewNotices_Internal(newNotices, licenses);
		yield break;
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x0004F9DC File Offset: 0x0004DBDC
	private void OnNewNotices_Internal(List<NetCache.ProfileNotice> newNotices, NetCache.NetCacheAccountLicenses netCacheAccountLicenses)
	{
		if (netCacheAccountLicenses == null)
		{
			Debug.LogWarning("AccountLicenses.OnNewNotices netCacheAccountLicenses is null -- going to ack all ACCOUNT_LICENSE notices assuming NetCache is not yet loaded");
		}
		HashSet<long> hashSet = new HashSet<long>();
		foreach (NetCache.ProfileNotice profileNotice in newNotices)
		{
			if (profileNotice.Type == NetCache.ProfileNotice.NoticeType.ACCOUNT_LICENSE)
			{
				NetCache.ProfileNoticeAcccountLicense profileNoticeAcccountLicense = profileNotice as NetCache.ProfileNoticeAcccountLicense;
				if (netCacheAccountLicenses != null)
				{
					if (!netCacheAccountLicenses.AccountLicenses.ContainsKey(profileNoticeAcccountLicense.License))
					{
						netCacheAccountLicenses.AccountLicenses[profileNoticeAcccountLicense.License] = new AccountLicenseInfo
						{
							License = profileNoticeAcccountLicense.License,
							Flags_ = 0UL,
							CasId = 0L
						};
					}
					if (profileNoticeAcccountLicense.CasID >= netCacheAccountLicenses.AccountLicenses[profileNoticeAcccountLicense.License].CasId)
					{
						netCacheAccountLicenses.AccountLicenses[profileNoticeAcccountLicense.License].CasId = profileNoticeAcccountLicense.CasID;
						NetCache.ProfileNotice.NoticeOrigin origin = profileNotice.Origin;
						if (origin != NetCache.ProfileNotice.NoticeOrigin.ACCOUNT_LICENSE_FLAGS)
						{
							Debug.LogWarning(string.Format("AccountLicenses.OnNewNotices unexpected notice origin {0} (data={1}) for license {2} casID {3}", new object[]
							{
								profileNotice.Origin,
								profileNotice.OriginData,
								profileNoticeAcccountLicense.License,
								profileNoticeAcccountLicense.CasID
							}));
						}
						else
						{
							netCacheAccountLicenses.AccountLicenses[profileNoticeAcccountLicense.License].Flags_ = (ulong)profileNotice.OriginData;
						}
						long num = profileNoticeAcccountLicense.CasID - 1L;
						if (this.m_seenLicenceNotices != null)
						{
							this.m_seenLicenceNotices.TryGetValue(profileNoticeAcccountLicense.License, out num);
						}
						if (num < profileNoticeAcccountLicense.CasID)
						{
							hashSet.Add(profileNoticeAcccountLicense.License);
						}
						if (this.m_seenLicenceNotices == null)
						{
							this.m_seenLicenceNotices = new Map<long, long>();
						}
						this.m_seenLicenceNotices[profileNoticeAcccountLicense.License] = profileNoticeAcccountLicense.CasID;
					}
				}
				Network.AckNotice(profileNotice.NoticeID);
			}
		}
		if (netCacheAccountLicenses == null)
		{
			return;
		}
		List<AccountLicenseInfo> list = new List<AccountLicenseInfo>();
		foreach (long key in hashSet)
		{
			if (netCacheAccountLicenses.AccountLicenses.ContainsKey(key))
			{
				list.Add(netCacheAccountLicenses.AccountLicenses[key]);
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		AccountLicenseMgr.AccountLicensesChangedListener[] array = this.m_accountLicensesChangedListeners.ToArray();
		foreach (AccountLicenseMgr.AccountLicensesChangedListener accountLicensesChangedListener in array)
		{
			accountLicensesChangedListener.Fire(list);
		}
	}

	// Token: 0x0400099D RID: 2461
	private static AccountLicenseMgr s_instance;

	// Token: 0x0400099E RID: 2462
	private bool m_registeredForProfileNotices;

	// Token: 0x0400099F RID: 2463
	private Map<long, long> m_seenLicenceNotices;

	// Token: 0x040009A0 RID: 2464
	private List<AccountLicenseMgr.AccountLicensesChangedListener> m_accountLicensesChangedListeners = new List<AccountLicenseMgr.AccountLicensesChangedListener>();

	// Token: 0x02000167 RID: 359
	// (Invoke) Token: 0x0600136D RID: 4973
	public delegate void AccountLicensesChangedCallback(List<AccountLicenseInfo> changedLicensesInfo, object userData);

	// Token: 0x02000EAB RID: 3755
	private class AccountLicensesChangedListener : EventListener<AccountLicenseMgr.AccountLicensesChangedCallback>
	{
		// Token: 0x0600712C RID: 28972 RVA: 0x00215CA6 File Offset: 0x00213EA6
		public void Fire(List<AccountLicenseInfo> changedLicensesInfo)
		{
			this.m_callback(changedLicensesInfo, this.m_userData);
		}
	}
}
