using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000291 RID: 657
public class AssetCache
{
	// Token: 0x060023DB RID: 9179 RVA: 0x000AFC98 File Offset: 0x000ADE98
	public static void Initialize()
	{
		foreach (object obj in Enum.GetValues(typeof(AssetFamily)))
		{
			AssetFamily key = (AssetFamily)((int)obj);
			AssetCache value = new AssetCache();
			AssetCache.s_cacheTable.Add(key, value);
		}
	}

	// Token: 0x060023DC RID: 9180 RVA: 0x000AFD10 File Offset: 0x000ADF10
	public static AssetCache.CachedAsset Find(Asset asset)
	{
		return AssetCache.s_cacheTable[asset.GetFamily()].GetItem(asset.GetName());
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x000AFD38 File Offset: 0x000ADF38
	public static bool HasItem(Asset asset)
	{
		return AssetCache.s_cacheTable[asset.GetFamily()].HasItem(asset.GetName());
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x000AFD60 File Offset: 0x000ADF60
	public static void Add(Asset asset, AssetCache.CachedAsset item)
	{
		AssetCache.s_cacheTable[asset.GetFamily()].AddItem(asset.GetName(), item);
	}

	// Token: 0x060023DF RID: 9183 RVA: 0x000AFD89 File Offset: 0x000ADF89
	public static long GetCacheClearTime()
	{
		return AssetCache.s_cacheClearTime;
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x000AFD90 File Offset: 0x000ADF90
	public static bool IsLoading(string name)
	{
		int num;
		bool flag = AssetCache.s_assetLoading.TryGetValue(name, out num);
		return flag && num > 0;
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x000AFDB8 File Offset: 0x000ADFB8
	public static void StartLoading(string name)
	{
		if (!AssetCache.s_assetLoading.ContainsKey(name))
		{
			AssetCache.s_assetLoading.Add(name, 0);
		}
		Map<string, int> map2;
		Map<string, int> map = map2 = AssetCache.s_assetLoading;
		int num = map2[name];
		map[name] = num + 1;
	}

	// Token: 0x060023E2 RID: 9186 RVA: 0x000AFDFC File Offset: 0x000ADFFC
	public static void StopLoading(string name)
	{
		int num;
		if (!AssetCache.s_assetLoading.TryGetValue(name, out num))
		{
			return;
		}
		if (num < 1)
		{
			AssetCache.s_assetLoading.Remove(name);
		}
		else
		{
			Map<string, int> map2;
			Map<string, int> map = map2 = AssetCache.s_assetLoading;
			int num2 = map2[name];
			map[name] = num2 - 1;
		}
	}

	// Token: 0x060023E3 RID: 9187 RVA: 0x000AFE54 File Offset: 0x000AE054
	public static void ClearAllCaches(bool clearPersistent = false, bool clearLoading = true)
	{
		foreach (KeyValuePair<AssetFamily, AssetCache> keyValuePair in AssetCache.s_cacheTable)
		{
			keyValuePair.Value.Clear(clearPersistent, clearLoading);
		}
		AssetCache.s_cacheClearTime = TimeUtils.BinaryStamp();
	}

	// Token: 0x060023E4 RID: 9188 RVA: 0x000AFEC0 File Offset: 0x000AE0C0
	public static void ClearAllCachesSince(long sinceTimestamp)
	{
		long endTimestamp = TimeUtils.BinaryStamp();
		AssetCache.ClearAllCachesBetween(sinceTimestamp, endTimestamp);
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x000AFEDC File Offset: 0x000AE0DC
	public static void ClearAllCachesBetween(long startTimestamp, long endTimestamp)
	{
		foreach (KeyValuePair<AssetFamily, AssetCache> keyValuePair in AssetCache.s_cacheTable)
		{
			keyValuePair.Value.ClearItemsBetween(startTimestamp, endTimestamp);
		}
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x000AFF3C File Offset: 0x000AE13C
	public static void ClearAllCachesFailedRequests()
	{
		foreach (KeyValuePair<AssetFamily, AssetCache> keyValuePair in AssetCache.s_cacheTable)
		{
			keyValuePair.Value.ClearAllFailedRequests();
		}
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x000AFF9C File Offset: 0x000AE19C
	public static void ForceClearAllCaches()
	{
		foreach (KeyValuePair<AssetFamily, AssetCache> keyValuePair in AssetCache.s_cacheTable)
		{
			keyValuePair.Value.ForceClear();
		}
		AssetCache.s_cacheClearTime = TimeUtils.BinaryStamp();
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x000B0004 File Offset: 0x000AE204
	public static void ClearCardPrefabs(IEnumerable<string> names)
	{
		if (names == null)
		{
			return;
		}
		AssetCache.s_cacheTable[AssetFamily.CardPrefab].ClearItems(names);
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000B001E File Offset: 0x000AE21E
	public static void ClearCardPrefab(string name)
	{
		AssetCache.s_cacheTable[AssetFamily.CardPrefab].ClearItem(name);
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x000B0032 File Offset: 0x000AE232
	public static void ClearActors(IEnumerable<string> names)
	{
		if (names == null)
		{
			return;
		}
		AssetCache.s_cacheTable[AssetFamily.Actor].ClearItems(names);
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x000B004C File Offset: 0x000AE24C
	public static void ClearActor(string name)
	{
		AssetCache.s_cacheTable[AssetFamily.Actor].ClearItem(name);
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x000B0060 File Offset: 0x000AE260
	public static void ClearTextures(IEnumerable<string> names)
	{
		if (names == null)
		{
			return;
		}
		AssetCache.s_cacheTable[AssetFamily.Texture].ClearItems(names);
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000B007B File Offset: 0x000AE27B
	public static void ClearTexture(string name)
	{
		AssetCache.s_cacheTable[AssetFamily.Texture].ClearItem(name);
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x000B0090 File Offset: 0x000AE290
	public static void ClearSound(string name)
	{
		AssetCache.s_cacheTable[AssetFamily.Sound].ClearItem(name);
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x000B00A5 File Offset: 0x000AE2A5
	public static void ClearGameObject(string name)
	{
		AssetCache.s_cacheTable[AssetFamily.GameObject].ClearItem(name);
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x000B00BC File Offset: 0x000AE2BC
	public static bool ClearItem(Asset asset)
	{
		return AssetCache.s_cacheTable[asset.GetFamily()].ClearItem(asset.GetName());
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x000B00E4 File Offset: 0x000AE2E4
	private AssetCache.CachedAsset GetItem(string key)
	{
		AssetCache.CachedAsset cachedAsset;
		return (!this.m_assetMap.TryGetValue(key, out cachedAsset)) ? null : cachedAsset;
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x000B010B File Offset: 0x000AE30B
	private bool HasItem(string key)
	{
		return this.m_assetMap.ContainsKey(key);
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x000B0119 File Offset: 0x000AE319
	private void AddItem(string name, AssetCache.CachedAsset item)
	{
		if (this.m_assetMap.ContainsKey(name))
		{
			Debug.LogWarning(string.Format("AssetCache: Loaded asset {0} twice.  This probably happened because it was loaded asynchronously and synchronously.", name));
			return;
		}
		this.m_assetMap.Add(name, item);
	}

	// Token: 0x060023F4 RID: 9204 RVA: 0x000B014C File Offset: 0x000AE34C
	private AssetFamily? GetFamily()
	{
		foreach (KeyValuePair<AssetFamily, AssetCache> keyValuePair in AssetCache.s_cacheTable)
		{
			if (this == keyValuePair.Value)
			{
				return new AssetFamily?(keyValuePair.Key);
			}
		}
		return default(AssetFamily?);
	}

	// Token: 0x060023F5 RID: 9205 RVA: 0x000B01C8 File Offset: 0x000AE3C8
	private void Clear(bool clearPersistent = false, bool clearLoading = true)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, AssetCache.CachedAsset> keyValuePair in this.m_assetMap)
		{
			string key = keyValuePair.Key;
			AssetCache.CachedAsset value = keyValuePair.Value;
			if (!value.IsPersistent() || clearPersistent)
			{
				if (AssetCache.IsLoading(key) && !clearLoading)
				{
					Log.Reset.Print(LogLevel.Warning, "Not clearing asset " + key + " because it's still loading", new object[0]);
				}
				else
				{
					list.Add(key);
				}
			}
		}
		foreach (string key2 in list)
		{
			this.ClearItem(key2);
		}
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<string, AssetCache.CacheRequest> keyValuePair2 in this.m_assetRequestMap)
		{
			string key3 = keyValuePair2.Key;
			AssetCache.CacheRequest value2 = keyValuePair2.Value;
			if ((!value2.IsPersistent() || clearPersistent) && (!AssetCache.IsLoading(key3) || clearLoading))
			{
				list2.Add(key3);
			}
		}
		foreach (string key4 in list2)
		{
			this.ClearItem(key4);
		}
	}

	// Token: 0x060023F6 RID: 9206 RVA: 0x000B03AC File Offset: 0x000AE5AC
	private void ForceClear()
	{
		foreach (KeyValuePair<string, AssetCache.CachedAsset> keyValuePair in this.m_assetMap)
		{
			AssetCache.CachedAsset value = keyValuePair.Value;
			value.UnloadAssetObject();
		}
		foreach (KeyValuePair<string, AssetCache.CacheRequest> keyValuePair2 in this.m_assetRequestMap)
		{
			AssetCache.CacheRequest value2 = keyValuePair2.Value;
			value2.SetSuccess(false);
		}
		this.m_assetMap.Clear();
		this.m_assetRequestMap.Clear();
	}

	// Token: 0x060023F7 RID: 9207 RVA: 0x000B0478 File Offset: 0x000AE678
	private void ClearItemsBetween(long startTimestamp, long endTimestamp)
	{
		if (endTimestamp < startTimestamp)
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>();
		foreach (KeyValuePair<string, AssetCache.CachedAsset> keyValuePair in this.m_assetMap)
		{
			AssetCache.CachedAsset value = keyValuePair.Value;
			if (!value.IsPersistent())
			{
				long lastRequestTimestamp = value.GetLastRequestTimestamp();
				if (startTimestamp <= lastRequestTimestamp && lastRequestTimestamp <= endTimestamp)
				{
					hashSet.Add(keyValuePair.Key);
				}
			}
		}
		foreach (KeyValuePair<string, AssetCache.CacheRequest> keyValuePair2 in this.m_assetRequestMap)
		{
			AssetCache.CacheRequest value2 = keyValuePair2.Value;
			if (!value2.IsPersistent())
			{
				long lastRequestTimestamp2 = value2.GetLastRequestTimestamp();
				if (startTimestamp <= lastRequestTimestamp2 && lastRequestTimestamp2 <= endTimestamp)
				{
					hashSet.Add(keyValuePair2.Key);
				}
			}
		}
		foreach (string key in hashSet)
		{
			this.ClearItem(key);
		}
	}

	// Token: 0x060023F8 RID: 9208 RVA: 0x000B05E8 File Offset: 0x000AE7E8
	private bool ClearItem(string key)
	{
		bool flag = false;
		AssetCache.CachedAsset cachedAsset;
		if (this.m_assetMap.TryGetValue(key, out cachedAsset))
		{
			cachedAsset.UnloadAssetObject();
			this.m_assetMap.Remove(key);
			flag = true;
		}
		AssetCache.CacheRequest cacheRequest;
		if (this.m_assetRequestMap.TryGetValue(key, out cacheRequest))
		{
			cacheRequest.SetSuccess(false);
			this.m_assetRequestMap.Remove(key);
			flag = true;
		}
		if (!flag)
		{
			Log.Asset.Print(string.Format("AssetCache.ClearItem() - there is no asset and no request for key {0} in {1}", key, this), new object[0]);
		}
		return flag;
	}

	// Token: 0x060023F9 RID: 9209 RVA: 0x000B066C File Offset: 0x000AE86C
	private void ClearItems(IEnumerable<string> itemsToRemove)
	{
		foreach (string key in itemsToRemove)
		{
			this.ClearItem(key);
		}
	}

	// Token: 0x060023FA RID: 9210 RVA: 0x000B06C0 File Offset: 0x000AE8C0
	private void ClearAllFailedRequests()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, AssetCache.CacheRequest> keyValuePair in this.m_assetRequestMap)
		{
			AssetCache.CacheRequest value = keyValuePair.Value;
			if (value.DidFail())
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (string key in list)
		{
			this.m_assetRequestMap.Remove(key);
		}
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x000B0788 File Offset: 0x000AE988
	public static T GetRequest<T>(Asset asset) where T : AssetCache.CacheRequest
	{
		return AssetCache.s_cacheTable[asset.GetFamily()].GetRequest<T>(asset.GetName());
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x000B07B0 File Offset: 0x000AE9B0
	public T GetRequest<T>(string key) where T : AssetCache.CacheRequest
	{
		AssetCache.CacheRequest cacheRequest;
		if (this.m_assetRequestMap.TryGetValue(key, out cacheRequest))
		{
			return cacheRequest as T;
		}
		return (T)((object)null);
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x000B07E4 File Offset: 0x000AE9E4
	public static bool HasRequest(Asset asset)
	{
		return AssetCache.s_cacheTable[asset.GetFamily()].HasRequest(asset.GetName());
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x000B080C File Offset: 0x000AEA0C
	public bool HasRequest(string key)
	{
		return this.m_assetRequestMap.ContainsKey(key);
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x000B081C File Offset: 0x000AEA1C
	public static void AddRequest(Asset asset, AssetCache.CacheRequest request)
	{
		AssetCache.s_cacheTable[asset.GetFamily()].AddRequest(asset.GetName(), request);
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x000B0845 File Offset: 0x000AEA45
	public void AddRequest(string key, AssetCache.CacheRequest request)
	{
		this.m_assetRequestMap.Add(key, request);
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000B0854 File Offset: 0x000AEA54
	public static bool RemoveRequest(Asset asset)
	{
		return AssetCache.s_cacheTable[asset.GetFamily()].RemoveRequest(asset.GetName());
	}

	// Token: 0x06002402 RID: 9218 RVA: 0x000B087C File Offset: 0x000AEA7C
	public bool RemoveRequest(string key)
	{
		return this.m_assetRequestMap.Remove(key);
	}

	// Token: 0x040014FA RID: 5370
	private Map<string, AssetCache.CachedAsset> m_assetMap = new Map<string, AssetCache.CachedAsset>();

	// Token: 0x040014FB RID: 5371
	private Map<string, AssetCache.CacheRequest> m_assetRequestMap = new Map<string, AssetCache.CacheRequest>();

	// Token: 0x040014FC RID: 5372
	private static Map<string, int> s_assetLoading = new Map<string, int>();

	// Token: 0x040014FD RID: 5373
	private static long s_cacheClearTime;

	// Token: 0x040014FE RID: 5374
	private static readonly Map<AssetFamily, AssetCache> s_cacheTable = new Map<AssetFamily, AssetCache>();

	// Token: 0x020002C8 RID: 712
	public abstract class CacheRequest
	{
		// Token: 0x060025E9 RID: 9705 RVA: 0x000B96F7 File Offset: 0x000B78F7
		public long GetCreatedTimestamp()
		{
			return this.m_createdTimestamp;
		}

		// Token: 0x060025EA RID: 9706 RVA: 0x000B96FF File Offset: 0x000B78FF
		public void SetCreatedTimestamp(long timestamp)
		{
			this.m_createdTimestamp = timestamp;
		}

		// Token: 0x060025EB RID: 9707 RVA: 0x000B9708 File Offset: 0x000B7908
		public long GetLastRequestTimestamp()
		{
			return this.m_lastRequestTimestamp;
		}

		// Token: 0x060025EC RID: 9708 RVA: 0x000B9710 File Offset: 0x000B7910
		public void SetLastRequestTimestamp(long timestamp)
		{
			this.m_lastRequestTimestamp = timestamp;
		}

		// Token: 0x060025ED RID: 9709 RVA: 0x000B9719 File Offset: 0x000B7919
		public bool IsPersistent()
		{
			return this.m_persistent;
		}

		// Token: 0x060025EE RID: 9710 RVA: 0x000B9721 File Offset: 0x000B7921
		public void SetPersistent(bool persistent)
		{
			this.m_persistent = persistent;
		}

		// Token: 0x060025EF RID: 9711 RVA: 0x000B972A File Offset: 0x000B792A
		public bool IsComplete()
		{
			return this.m_complete;
		}

		// Token: 0x060025F0 RID: 9712 RVA: 0x000B9732 File Offset: 0x000B7932
		public void SetComplete(bool complete)
		{
			this.m_complete = complete;
		}

		// Token: 0x060025F1 RID: 9713 RVA: 0x000B973B File Offset: 0x000B793B
		public bool IsSuccess()
		{
			return this.m_success;
		}

		// Token: 0x060025F2 RID: 9714 RVA: 0x000B9743 File Offset: 0x000B7943
		public void SetSuccess(bool success)
		{
			this.m_success = success;
		}

		// Token: 0x060025F3 RID: 9715
		public abstract int GetRequestCount();

		// Token: 0x060025F4 RID: 9716 RVA: 0x000B974C File Offset: 0x000B794C
		public void OnLoadSucceeded()
		{
			this.m_complete = true;
			this.m_success = true;
		}

		// Token: 0x060025F5 RID: 9717 RVA: 0x000B975C File Offset: 0x000B795C
		public virtual void OnLoadFailed(string name)
		{
			this.m_complete = true;
			this.m_success = false;
		}

		// Token: 0x060025F6 RID: 9718 RVA: 0x000B976C File Offset: 0x000B796C
		public bool DidSucceed()
		{
			return this.m_complete && this.m_success;
		}

		// Token: 0x060025F7 RID: 9719 RVA: 0x000B9782 File Offset: 0x000B7982
		public bool DidFail()
		{
			return this.m_complete && !this.m_success;
		}

		// Token: 0x04001665 RID: 5733
		private long m_createdTimestamp;

		// Token: 0x04001666 RID: 5734
		private long m_lastRequestTimestamp;

		// Token: 0x04001667 RID: 5735
		private bool m_persistent;

		// Token: 0x04001668 RID: 5736
		private bool m_complete;

		// Token: 0x04001669 RID: 5737
		private bool m_success;
	}

	// Token: 0x020002CB RID: 715
	public class CachedAsset
	{
		// Token: 0x06002609 RID: 9737 RVA: 0x000B995E File Offset: 0x000B7B5E
		public Object GetAssetObject()
		{
			return this.m_assetObject;
		}

		// Token: 0x0600260A RID: 9738 RVA: 0x000B9966 File Offset: 0x000B7B66
		public void SetAssetObject(Object asset)
		{
			this.m_assetObject = asset;
		}

		// Token: 0x0600260B RID: 9739 RVA: 0x000B996F File Offset: 0x000B7B6F
		public Asset GetAsset()
		{
			return this.m_asset;
		}

		// Token: 0x0600260C RID: 9740 RVA: 0x000B9977 File Offset: 0x000B7B77
		public void SetAsset(Asset asset)
		{
			this.m_asset = asset;
		}

		// Token: 0x0600260D RID: 9741 RVA: 0x000B9980 File Offset: 0x000B7B80
		public long GetCreatedTimestamp()
		{
			return this.m_createdTimestamp;
		}

		// Token: 0x0600260E RID: 9742 RVA: 0x000B9988 File Offset: 0x000B7B88
		public void SetCreatedTimestamp(long timestamp)
		{
			this.m_createdTimestamp = timestamp;
		}

		// Token: 0x0600260F RID: 9743 RVA: 0x000B9991 File Offset: 0x000B7B91
		public long GetLastRequestTimestamp()
		{
			return this.m_lastRequestTimestamp;
		}

		// Token: 0x06002610 RID: 9744 RVA: 0x000B9999 File Offset: 0x000B7B99
		public void SetLastRequestTimestamp(long timestamp)
		{
			this.m_lastRequestTimestamp = timestamp;
		}

		// Token: 0x06002611 RID: 9745 RVA: 0x000B99A2 File Offset: 0x000B7BA2
		public bool IsPersistent()
		{
			return this.m_persistent;
		}

		// Token: 0x06002612 RID: 9746 RVA: 0x000B99AA File Offset: 0x000B7BAA
		public void SetPersistent(bool persistent)
		{
			this.m_persistent = persistent;
		}

		// Token: 0x06002613 RID: 9747 RVA: 0x000B99B4 File Offset: 0x000B7BB4
		public void UnloadAssetObject()
		{
			Log.Asset.Print("CachedAsset.UnloadAssetObject() - unloading name={0} family={1} persistent={2}", new object[]
			{
				this.m_asset.GetName(),
				this.m_asset.GetFamily(),
				this.IsPersistent()
			});
			this.m_assetObject = null;
		}

		// Token: 0x0400167B RID: 5755
		private Object m_assetObject;

		// Token: 0x0400167C RID: 5756
		private Asset m_asset;

		// Token: 0x0400167D RID: 5757
		private long m_createdTimestamp;

		// Token: 0x0400167E RID: 5758
		private long m_lastRequestTimestamp;

		// Token: 0x0400167F RID: 5759
		private bool m_persistent;
	}

	// Token: 0x020002D2 RID: 722
	public class ObjectCacheRequest : AssetCache.CacheRequest
	{
		// Token: 0x06002627 RID: 9767 RVA: 0x000BA1C6 File Offset: 0x000B83C6
		public List<AssetCache.ObjectRequester> GetRequesters()
		{
			return this.m_requesters;
		}

		// Token: 0x06002628 RID: 9768 RVA: 0x000BA1D0 File Offset: 0x000B83D0
		public void AddRequester(AssetLoader.ObjectCallback callback, object callbackData)
		{
			AssetCache.ObjectRequester objectRequester = new AssetCache.ObjectRequester
			{
				m_callback = callback,
				m_callbackData = callbackData
			};
			this.m_requesters.Add(objectRequester);
		}

		// Token: 0x06002629 RID: 9769 RVA: 0x000BA200 File Offset: 0x000B8400
		public void OnLoadComplete(string name, Object asset)
		{
			foreach (AssetCache.ObjectRequester objectRequester in this.m_requesters)
			{
				AssetLoader.ObjectCallback callback = objectRequester.m_callback;
				if (GeneralUtils.IsCallbackValid(callback))
				{
					callback(name, asset, objectRequester.m_callbackData);
				}
			}
		}

		// Token: 0x0600262A RID: 9770 RVA: 0x000BA274 File Offset: 0x000B8474
		public override void OnLoadFailed(string name)
		{
			base.OnLoadFailed(name);
			this.OnLoadComplete(name, null);
		}

		// Token: 0x0600262B RID: 9771 RVA: 0x000BA285 File Offset: 0x000B8485
		public override int GetRequestCount()
		{
			return this.m_requesters.Count;
		}

		// Token: 0x040016C8 RID: 5832
		private readonly List<AssetCache.ObjectRequester> m_requesters = new List<AssetCache.ObjectRequester>();
	}

	// Token: 0x020002D4 RID: 724
	public class PrefabCacheRequest : AssetCache.CacheRequest
	{
		// Token: 0x0600262F RID: 9775 RVA: 0x000BA33F File Offset: 0x000B853F
		public List<AssetCache.GameObjectRequester> GetRequesters()
		{
			return this.m_requesters;
		}

		// Token: 0x06002630 RID: 9776 RVA: 0x000BA348 File Offset: 0x000B8548
		public void AddRequester(AssetLoader.GameObjectCallback callback, object callbackData)
		{
			AssetCache.GameObjectRequester gameObjectRequester = new AssetCache.GameObjectRequester
			{
				m_callback = callback,
				m_callbackData = callbackData
			};
			this.m_requesters.Add(gameObjectRequester);
		}

		// Token: 0x06002631 RID: 9777 RVA: 0x000BA378 File Offset: 0x000B8578
		public override void OnLoadFailed(string name)
		{
			base.OnLoadFailed(name);
			foreach (AssetCache.GameObjectRequester gameObjectRequester in this.m_requesters)
			{
				AssetLoader.GameObjectCallback callback = gameObjectRequester.m_callback;
				object callbackData = gameObjectRequester.m_callbackData;
				if (GeneralUtils.IsCallbackValid(callback))
				{
					callback(name, null, callbackData);
				}
			}
		}

		// Token: 0x06002632 RID: 9778 RVA: 0x000BA3F4 File Offset: 0x000B85F4
		public override int GetRequestCount()
		{
			return this.m_requesters.Count;
		}

		// Token: 0x040016CD RID: 5837
		private readonly List<AssetCache.GameObjectRequester> m_requesters = new List<AssetCache.GameObjectRequester>();
	}

	// Token: 0x020002D5 RID: 725
	public class GameObjectRequester
	{
		// Token: 0x040016CE RID: 5838
		public AssetLoader.GameObjectCallback m_callback;

		// Token: 0x040016CF RID: 5839
		public object m_callbackData;
	}

	// Token: 0x020007A0 RID: 1952
	public class ObjectRequester
	{
		// Token: 0x04003408 RID: 13320
		public AssetLoader.ObjectCallback m_callback;

		// Token: 0x04003409 RID: 13321
		public object m_callbackData;
	}
}
