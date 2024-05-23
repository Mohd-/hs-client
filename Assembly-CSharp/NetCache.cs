using System;
using System.Collections.Generic;
using System.Text;
using bgs.types;
using PegasusShared;
using PegasusUtil;
using UnityEngine;
using WTCG.BI;

// Token: 0x0200000E RID: 14
public class NetCache
{
	// Token: 0x0600016A RID: 362 RVA: 0x0000777C File Offset: 0x0000597C
	private static Map<GetAccountInfo.Request, Type> GetInvertTypeMap()
	{
		Map<GetAccountInfo.Request, Type> map = new Map<GetAccountInfo.Request, Type>();
		foreach (KeyValuePair<Type, GetAccountInfo.Request> keyValuePair in NetCache.m_typeMap)
		{
			map[keyValuePair.Value] = keyValuePair.Key;
		}
		return map;
	}

	// Token: 0x0600016B RID: 363 RVA: 0x000077EC File Offset: 0x000059EC
	public T GetNetObject<T>()
	{
		Type typeFromHandle = typeof(T);
		object testData = this.GetTestData(typeFromHandle);
		if (testData != null)
		{
			return (T)((object)testData);
		}
		if (this.m_netCache.TryGetValue(typeof(T), out testData) && testData is T)
		{
			return (T)((object)testData);
		}
		return default(T);
	}

	// Token: 0x0600016C RID: 364 RVA: 0x00007850 File Offset: 0x00005A50
	public bool IsNetObjectReady<T>()
	{
		return this.GetNetObject<T>() != null;
	}

	// Token: 0x0600016D RID: 365 RVA: 0x00007864 File Offset: 0x00005A64
	private object GetTestData(Type type)
	{
		if (type == typeof(NetCache.NetCacheBoosters) && GameUtils.IsFakePackOpeningEnabled())
		{
			NetCache.NetCacheBoosters netCacheBoosters = new NetCache.NetCacheBoosters();
			int fakePackCount = GameUtils.GetFakePackCount();
			NetCache.BoosterStack boosterStack = new NetCache.BoosterStack
			{
				Id = 1,
				Count = fakePackCount
			};
			netCacheBoosters.BoosterStacks.Add(boosterStack);
			return netCacheBoosters;
		}
		return null;
	}

	// Token: 0x0600016E RID: 366 RVA: 0x000078BC File Offset: 0x00005ABC
	public void UnloadNetObject<T>()
	{
		Type typeFromHandle = typeof(T);
		this.m_netCache[typeFromHandle] = null;
	}

	// Token: 0x0600016F RID: 367 RVA: 0x000078E1 File Offset: 0x00005AE1
	public void ReloadNetObject<T>()
	{
		this.NetCacheReload_Internal(null, typeof(T));
	}

	// Token: 0x06000170 RID: 368 RVA: 0x000078F4 File Offset: 0x00005AF4
	public void RefreshNetObject<T>()
	{
		this.RequestNetCacheObject(typeof(T));
	}

	// Token: 0x06000171 RID: 369 RVA: 0x00007908 File Offset: 0x00005B08
	private bool GetOption<T>(ServerOption type, out T ret) where T : NetCache.ClientOptionBase
	{
		ret = (T)((object)null);
		NetCache.NetCacheClientOptions netObject = NetCache.Get().GetNetObject<NetCache.NetCacheClientOptions>();
		if (!this.ClientOptionExists(type))
		{
			return false;
		}
		T t = netObject.ClientState[type] as T;
		if (t == null)
		{
			return false;
		}
		ret = t;
		return true;
	}

	// Token: 0x06000172 RID: 370 RVA: 0x00007968 File Offset: 0x00005B68
	public bool GetBoolOption(ServerOption type)
	{
		NetCache.ClientOptionBool clientOptionBool = null;
		return this.GetOption<NetCache.ClientOptionBool>(type, out clientOptionBool) && clientOptionBool.OptionValue;
	}

	// Token: 0x06000173 RID: 371 RVA: 0x00007990 File Offset: 0x00005B90
	public bool GetBoolOption(ServerOption type, out bool ret)
	{
		ret = false;
		NetCache.ClientOptionBool clientOptionBool = null;
		if (!this.GetOption<NetCache.ClientOptionBool>(type, out clientOptionBool))
		{
			return false;
		}
		ret = clientOptionBool.OptionValue;
		return true;
	}

	// Token: 0x06000174 RID: 372 RVA: 0x000079BC File Offset: 0x00005BBC
	public int GetIntOption(ServerOption type)
	{
		NetCache.ClientOptionInt clientOptionInt = null;
		if (!this.GetOption<NetCache.ClientOptionInt>(type, out clientOptionInt))
		{
			return 0;
		}
		return clientOptionInt.OptionValue;
	}

	// Token: 0x06000175 RID: 373 RVA: 0x000079E4 File Offset: 0x00005BE4
	public bool GetIntOption(ServerOption type, out int ret)
	{
		ret = 0;
		NetCache.ClientOptionInt clientOptionInt = null;
		if (!this.GetOption<NetCache.ClientOptionInt>(type, out clientOptionInt))
		{
			return false;
		}
		ret = clientOptionInt.OptionValue;
		return true;
	}

	// Token: 0x06000176 RID: 374 RVA: 0x00007A10 File Offset: 0x00005C10
	public long GetLongOption(ServerOption type)
	{
		NetCache.ClientOptionLong clientOptionLong = null;
		if (!this.GetOption<NetCache.ClientOptionLong>(type, out clientOptionLong))
		{
			return 0L;
		}
		return clientOptionLong.OptionValue;
	}

	// Token: 0x06000177 RID: 375 RVA: 0x00007A38 File Offset: 0x00005C38
	public bool GetLongOption(ServerOption type, out long ret)
	{
		ret = 0L;
		NetCache.ClientOptionLong clientOptionLong = null;
		if (!this.GetOption<NetCache.ClientOptionLong>(type, out clientOptionLong))
		{
			return false;
		}
		ret = clientOptionLong.OptionValue;
		return true;
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00007A64 File Offset: 0x00005C64
	public float GetFloatOption(ServerOption type)
	{
		NetCache.ClientOptionFloat clientOptionFloat = null;
		if (!this.GetOption<NetCache.ClientOptionFloat>(type, out clientOptionFloat))
		{
			return 0f;
		}
		return clientOptionFloat.OptionValue;
	}

	// Token: 0x06000179 RID: 377 RVA: 0x00007A90 File Offset: 0x00005C90
	public bool GetFloatOption(ServerOption type, out float ret)
	{
		ret = 0f;
		NetCache.ClientOptionFloat clientOptionFloat = null;
		if (!this.GetOption<NetCache.ClientOptionFloat>(type, out clientOptionFloat))
		{
			return false;
		}
		ret = clientOptionFloat.OptionValue;
		return true;
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00007AC0 File Offset: 0x00005CC0
	public ulong GetULongOption(ServerOption type)
	{
		NetCache.ClientOptionULong clientOptionULong = null;
		if (!this.GetOption<NetCache.ClientOptionULong>(type, out clientOptionULong))
		{
			return 0UL;
		}
		return clientOptionULong.OptionValue;
	}

	// Token: 0x0600017B RID: 379 RVA: 0x00007AE8 File Offset: 0x00005CE8
	public bool GetULongOption(ServerOption type, out ulong ret)
	{
		ret = 0UL;
		NetCache.ClientOptionULong clientOptionULong = null;
		if (!this.GetOption<NetCache.ClientOptionULong>(type, out clientOptionULong))
		{
			return false;
		}
		ret = clientOptionULong.OptionValue;
		return true;
	}

	// Token: 0x0600017C RID: 380 RVA: 0x00007B14 File Offset: 0x00005D14
	public void RegisterUpdatedListener(Type type, Action listener)
	{
		List<Action> list;
		if (!this.m_updatedListeners.TryGetValue(type, out list))
		{
			list = new List<Action>();
			this.m_updatedListeners[type] = list;
		}
		list.Add(listener);
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00007B50 File Offset: 0x00005D50
	public void RemoveUpdatedListener(Type type, Action listener)
	{
		List<Action> list;
		if (this.m_updatedListeners.TryGetValue(type, out list))
		{
			list.Remove(listener);
		}
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00007B78 File Offset: 0x00005D78
	public void RegisterNewNoticesListener(NetCache.DelNewNoticesListener listener)
	{
		if (this.m_newNoticesListeners.Contains(listener))
		{
			return;
		}
		this.m_newNoticesListeners.Add(listener);
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00007B98 File Offset: 0x00005D98
	public void RemoveNewNoticesListener(NetCache.DelNewNoticesListener listener)
	{
		this.m_newNoticesListeners.Remove(listener);
	}

	// Token: 0x06000180 RID: 384 RVA: 0x00007BA8 File Offset: 0x00005DA8
	public bool RemoveNotice(long ID)
	{
		NetCache.NetCacheProfileNotices netCacheProfileNotices = this.m_netCache[typeof(NetCache.NetCacheProfileNotices)] as NetCache.NetCacheProfileNotices;
		if (netCacheProfileNotices == null)
		{
			Debug.LogWarning(string.Format("NetCache.RemoveNotice({0}) - profileNotices is null", ID));
			return false;
		}
		if (netCacheProfileNotices.Notices == null)
		{
			Debug.LogWarning(string.Format("NetCache.RemoveNotice({0}) - profileNotices.Notices is null", ID));
			return false;
		}
		NetCache.ProfileNotice profileNotice = netCacheProfileNotices.Notices.Find((NetCache.ProfileNotice obj) => obj.NoticeID == ID);
		if (profileNotice == null)
		{
			return false;
		}
		netCacheProfileNotices.Notices.Remove(profileNotice);
		return true;
	}

	// Token: 0x06000181 RID: 385 RVA: 0x00007C54 File Offset: 0x00005E54
	public void NetCacheChanged<T>()
	{
		Type typeFromHandle = typeof(T);
		int num = 0;
		this.m_changeRequests.TryGetValue(typeFromHandle, out num);
		num++;
		this.m_changeRequests[typeFromHandle] = num;
		if (num > 1)
		{
			return;
		}
		while (this.m_changeRequests[typeFromHandle] > 0)
		{
			this.NetCacheChangedImpl<T>();
			this.m_changeRequests[typeFromHandle] = this.m_changeRequests[typeFromHandle] - 1;
		}
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00007CD0 File Offset: 0x00005ED0
	private void NetCacheChangedImpl<T>()
	{
		NetCache.NetCacheBatchRequest[] array = this.m_cacheRequests.ToArray();
		foreach (NetCache.NetCacheBatchRequest netCacheBatchRequest in array)
		{
			foreach (KeyValuePair<Type, NetCache.Request> keyValuePair in netCacheBatchRequest.m_requests)
			{
				if (keyValuePair.Key == typeof(T))
				{
					this.NetCacheCheckRequest(netCacheBatchRequest);
					break;
				}
			}
		}
	}

	// Token: 0x06000183 RID: 387 RVA: 0x00007D74 File Offset: 0x00005F74
	public void CheckSeasonForRoll()
	{
		if (this.GetNetObject<NetCache.NetCacheProfileNotices>() == null)
		{
			return;
		}
		NetCache.NetCacheRewardProgress netObject = this.GetNetObject<NetCache.NetCacheRewardProgress>();
		if (netObject == null)
		{
			return;
		}
		DateTime utcNow = DateTime.UtcNow;
		DateTime dateTime = DateTime.FromFileTimeUtc(netObject.SeasonEndDate);
		if (dateTime >= utcNow)
		{
			return;
		}
		if (this.m_lastForceCheckedSeason == (long)netObject.Season)
		{
			return;
		}
		this.m_lastForceCheckedSeason = (long)netObject.Season;
		Log.Rachelle.Print("NetCache.CheckSeasonForRoll oldSeason = {0} season end = {1} utc now = {2}", new object[]
		{
			this.m_lastForceCheckedSeason,
			dateTime,
			utcNow
		});
		this.ReloadNetObject<NetCache.NetCacheProfileNotices>();
	}

	// Token: 0x06000184 RID: 388 RVA: 0x00007E18 File Offset: 0x00006018
	public void OnArcaneDustBalanceChanged(long balanceChange)
	{
		NetCache.NetCacheArcaneDustBalance netObject = this.GetNetObject<NetCache.NetCacheArcaneDustBalance>();
		netObject.Balance += balanceChange;
		this.NetCacheChanged<NetCache.NetCacheArcaneDustBalance>();
	}

	// Token: 0x06000185 RID: 389 RVA: 0x00007E40 File Offset: 0x00006040
	public void RegisterGoldBalanceListener(NetCache.DelGoldBalanceListener listener)
	{
		if (this.m_goldBalanceListeners.Contains(listener))
		{
			return;
		}
		this.m_goldBalanceListeners.Add(listener);
	}

	// Token: 0x06000186 RID: 390 RVA: 0x00007E60 File Offset: 0x00006060
	public void RemoveGoldBalanceListener(NetCache.DelGoldBalanceListener listener)
	{
		this.m_goldBalanceListeners.Remove(listener);
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00007E70 File Offset: 0x00006070
	public static void DefaultErrorHandler(NetCache.ErrorInfo info)
	{
		if (info.Error == NetCache.ErrorCode.TIMEOUT)
		{
			if (BreakingNews.SHOWS_BREAKING_NEWS)
			{
				string error = "GLOBAL_ERROR_NETWORK_UTIL_TIMEOUT";
				Network.Get().ShowBreakingNewsOrError(error, 0f);
			}
			else
			{
				NetCache.ShowError(info, "GLOBAL_ERROR_NETWORK_UTIL_TIMEOUT", new object[0]);
			}
		}
		else
		{
			NetCache.ShowError(info, "GLOBAL_ERROR_NETWORK_GENERIC", new object[0]);
		}
	}

	// Token: 0x06000188 RID: 392 RVA: 0x00007ED8 File Offset: 0x000060D8
	public static void ShowError(NetCache.ErrorInfo info, string localizationKey, params object[] localizationArgs)
	{
		Error.AddFatalLoc(localizationKey, localizationArgs);
		string internalErrorMessage = NetCache.GetInternalErrorMessage(info, true);
		Debug.LogError(internalErrorMessage);
	}

	// Token: 0x06000189 RID: 393 RVA: 0x00007EFC File Offset: 0x000060FC
	public static string GetInternalErrorMessage(NetCache.ErrorInfo info, bool includeStackTrace = true)
	{
		Map<Type, object> netCache = NetCache.Get().m_netCache;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("NetCache Error: {0}", info.Error);
		stringBuilder.AppendFormat("\nFrom: {0}", info.RequestingFunction.Method.Name);
		stringBuilder.AppendFormat("\nRequested Data ({0}):", info.RequestedTypes.Count);
		foreach (KeyValuePair<Type, NetCache.Request> keyValuePair in info.RequestedTypes)
		{
			object obj = null;
			netCache.TryGetValue(keyValuePair.Key, out obj);
			if (obj == null)
			{
				stringBuilder.AppendFormat("\n[{0}] MISSING", keyValuePair.Key);
			}
			else
			{
				stringBuilder.AppendFormat("\n[{0}]", keyValuePair.Key);
			}
		}
		if (includeStackTrace)
		{
			stringBuilder.AppendFormat("\nStack Trace:\n{0}", info.RequestStackTrace);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600018A RID: 394 RVA: 0x00008014 File Offset: 0x00006214
	public void NetCacheRequestReload(Type type)
	{
		this.m_netCache[type] = null;
	}

	// Token: 0x0600018B RID: 395 RVA: 0x00008024 File Offset: 0x00006224
	private void NetCacheMakeBatchRequest(NetCache.NetCacheBatchRequest batchRequest)
	{
		List<GetAccountInfo.Request> list = new List<GetAccountInfo.Request>();
		foreach (KeyValuePair<Type, NetCache.Request> keyValuePair in batchRequest.m_requests)
		{
			NetCache.Request value = keyValuePair.Value;
			if (value == null)
			{
				Debug.LogError(string.Format("NetUseBatchRequest Null request for {0}...SKIP", value.m_type.Name));
			}
			else
			{
				if (value.m_reload)
				{
					this.m_netCache[value.m_type] = null;
				}
				if (!this.m_netCache.ContainsKey(value.m_type) || this.m_netCache[value.m_type] == null)
				{
					if (!this.m_inTransitRequests.Contains(value.m_type))
					{
						value.m_result = NetCache.RequestResult.PENDING;
						this.m_inTransitRequests.Add(value.m_type);
						list.Add(NetCache.m_typeMap[value.m_type]);
					}
				}
			}
		}
		if (list.Count > 0)
		{
			ConnectAPI.RequestNetCacheObjectList(list);
		}
		this.m_cacheRequests.Add(batchRequest);
		this.NetCacheCheckRequest(batchRequest);
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00008168 File Offset: 0x00006368
	private void NetCacheUse_Internal(NetCache.NetCacheBatchRequest request, Type type)
	{
		if (request != null && request.m_requests.ContainsKey(type))
		{
			Log.Bob.Print(string.Format("NetCache ...SKIP {0}", type.Name), new object[0]);
			return;
		}
		if (this.m_netCache.ContainsKey(type) && this.m_netCache[type] != null)
		{
			Log.Bob.Print(string.Format("NetCache ...USE {0}", type.Name), new object[0]);
			return;
		}
		Log.Bob.Print(string.Format("NetCache <<<GET {0}", type.Name), new object[0]);
		this.RequestNetCacheObject(type);
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00008217 File Offset: 0x00006417
	private void RequestNetCacheObject(Type type)
	{
		if (this.m_inTransitRequests.Contains(type))
		{
			return;
		}
		this.m_inTransitRequests.Add(type);
		ConnectAPI.RequestNetCacheObject(NetCache.m_typeMap[type]);
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00008247 File Offset: 0x00006447
	private void NetCacheReload_Internal(NetCache.NetCacheBatchRequest request, Type type)
	{
		this.m_netCache[type] = null;
		this.NetCacheUse_Internal(request, type);
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00008260 File Offset: 0x00006460
	private void NetCacheCheckRequest(NetCache.NetCacheBatchRequest request)
	{
		foreach (KeyValuePair<Type, NetCache.Request> keyValuePair in request.m_requests)
		{
			if (!this.m_netCache.ContainsKey(keyValuePair.Key))
			{
				return;
			}
			if (this.m_netCache[keyValuePair.Key] == null)
			{
				return;
			}
		}
		request.m_canTimeout = false;
		if (request.m_callback != null)
		{
			request.m_callback();
		}
	}

	// Token: 0x06000190 RID: 400 RVA: 0x00008308 File Offset: 0x00006508
	private void UpdateRequestNeedState(Type type, NetCache.RequestResult result)
	{
		foreach (NetCache.NetCacheBatchRequest netCacheBatchRequest in this.m_cacheRequests)
		{
			if (netCacheBatchRequest.m_requests.ContainsKey(type))
			{
				netCacheBatchRequest.m_requests[type].m_result = result;
			}
		}
	}

	// Token: 0x06000191 RID: 401 RVA: 0x00008380 File Offset: 0x00006580
	private void OnNetCacheObjReceived<T>(T netCacheObject)
	{
		Type typeFromHandle = typeof(T);
		Log.Bob.Print(string.Format("OnNetCacheObjReceived SAVE --> {0}", typeFromHandle.Name), new object[0]);
		this.UpdateRequestNeedState(typeFromHandle, NetCache.RequestResult.DATA_COMPLETE);
		this.m_netCache[typeFromHandle] = netCacheObject;
		this.m_inTransitRequests.Remove(typeFromHandle);
		this.NetCacheChanged<T>();
		List<Action> list;
		if (this.m_updatedListeners.TryGetValue(typeFromHandle, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				Action action = list[i];
				action.Invoke();
			}
		}
	}

	// Token: 0x06000192 RID: 402 RVA: 0x0000841D File Offset: 0x0000661D
	public void Clear()
	{
		this.m_netCache.Clear();
		this.m_prevHeroLevels = null;
		this.m_changeRequests.Clear();
		this.m_cacheRequests.Clear();
		this.m_inTransitRequests.Clear();
	}

	// Token: 0x06000193 RID: 403 RVA: 0x00008454 File Offset: 0x00006654
	public void UnregisterNetCacheHandler(NetCache.NetCacheCallback handler)
	{
		foreach (NetCache.NetCacheBatchRequest netCacheBatchRequest in this.m_cacheRequests)
		{
			if (!(netCacheBatchRequest.m_callback != handler))
			{
				this.m_cacheRequests.Remove(netCacheBatchRequest);
				break;
			}
		}
	}

	// Token: 0x06000194 RID: 404 RVA: 0x000084D0 File Offset: 0x000066D0
	public void Heartbeat()
	{
		NetCache.NetCacheBatchRequest[] array = this.m_cacheRequests.ToArray();
		DateTime now = DateTime.Now;
		foreach (NetCache.NetCacheBatchRequest netCacheBatchRequest in array)
		{
			if (netCacheBatchRequest.m_canTimeout)
			{
				TimeSpan timeSpan = now - netCacheBatchRequest.m_timeAdded;
				if (!(timeSpan < Network.GetMaxDeferredWait()))
				{
					if (!Network.Get().HaveUnhandledPackets())
					{
						netCacheBatchRequest.m_canTimeout = false;
						if (!NetCache.m_fatalErrorCodeSet)
						{
							NetCache.ErrorInfo errorInfo = new NetCache.ErrorInfo();
							errorInfo.Error = NetCache.ErrorCode.TIMEOUT;
							errorInfo.RequestingFunction = netCacheBatchRequest.m_requestFunc;
							errorInfo.RequestedTypes = new Map<Type, NetCache.Request>(netCacheBatchRequest.m_requests);
							errorInfo.RequestStackTrace = netCacheBatchRequest.m_requestStackTrace;
							string text = "CT";
							int num = 0;
							foreach (KeyValuePair<Type, NetCache.Request> keyValuePair in netCacheBatchRequest.m_requests)
							{
								NetCache.RequestResult result = keyValuePair.Value.m_result;
								if (result != NetCache.RequestResult.GENERIC_COMPLETE && result != NetCache.RequestResult.DATA_COMPLETE)
								{
									string[] array3 = keyValuePair.Value.m_type.ToString().Split(new char[]
									{
										'+'
									});
									if (array3.GetLength(0) != 0)
									{
										string text2 = array3[array3.GetLength(0) - 1];
										text = string.Concat(new object[]
										{
											text,
											";",
											text2,
											"=",
											(int)keyValuePair.Value.m_result
										});
										num++;
									}
								}
								if (num >= 3)
								{
									break;
								}
							}
							FatalErrorMgr.Get().SetErrorCode("HS", text, null, null);
							NetCache.m_fatalErrorCodeSet = true;
							netCacheBatchRequest.m_errorCallback(errorInfo);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000195 RID: 405 RVA: 0x000086E0 File Offset: 0x000068E0
	private void OnGenericResponse()
	{
		Network.GenericResponse genericResponse = ConnectAPI.GetGenericResponse();
		if (genericResponse == null)
		{
			Debug.LogError(string.Format("NetCache - GenericResponse parse error", new object[0]));
			return;
		}
		if ((long)genericResponse.RequestId != 201L)
		{
			return;
		}
		Type key;
		if (!NetCache.m_requestTypeMap.TryGetValue(genericResponse.RequestSubId, out key))
		{
			Debug.LogError(string.Format("NetCache - Ignoring unexpected requestId={0}:{1}", genericResponse.RequestId, genericResponse.RequestSubId));
			return;
		}
		NetCache.NetCacheBatchRequest[] array = this.m_cacheRequests.ToArray();
		foreach (NetCache.NetCacheBatchRequest netCacheBatchRequest in array)
		{
			if (netCacheBatchRequest.m_requests.ContainsKey(key))
			{
				Network.GenericResponse.Result resultCode = genericResponse.ResultCode;
				if (resultCode != Network.GenericResponse.Result.REQUEST_IN_PROCESS)
				{
					if (resultCode != Network.GenericResponse.Result.REQUEST_COMPLETE)
					{
						Debug.LogError(string.Format("Unhandled failure code={0} {1} for requestId={2}:{3}", new object[]
						{
							(int)genericResponse.ResultCode,
							genericResponse.ResultCode.ToString(),
							genericResponse.RequestId,
							genericResponse.RequestSubId
						}));
						netCacheBatchRequest.m_requests[key].m_result = NetCache.RequestResult.ERROR;
						NetCache.ErrorInfo errorInfo = new NetCache.ErrorInfo();
						errorInfo.Error = NetCache.ErrorCode.SERVER;
						errorInfo.ServerError = (uint)genericResponse.ResultCode;
						errorInfo.RequestingFunction = netCacheBatchRequest.m_requestFunc;
						errorInfo.RequestedTypes = new Map<Type, NetCache.Request>(netCacheBatchRequest.m_requests);
						errorInfo.RequestStackTrace = netCacheBatchRequest.m_requestStackTrace;
						FatalErrorMgr.Get().SetErrorCode("HS", "CG" + genericResponse.ResultCode.ToString(), genericResponse.RequestId.ToString(), genericResponse.RequestSubId.ToString());
						netCacheBatchRequest.m_errorCallback(errorInfo);
					}
					else
					{
						netCacheBatchRequest.m_requests[key].m_result = NetCache.RequestResult.GENERIC_COMPLETE;
						Debug.LogWarning(string.Format("GenericResponse Success for requestId={0}:{1}", genericResponse.RequestId, genericResponse.RequestSubId));
					}
				}
				else if (netCacheBatchRequest.m_requests[key].m_result == NetCache.RequestResult.PENDING)
				{
					netCacheBatchRequest.m_requests[key].m_result = NetCache.RequestResult.IN_PROCESS;
				}
			}
		}
	}

	// Token: 0x06000196 RID: 406 RVA: 0x0000892C File Offset: 0x00006B2C
	private void OnDBAction()
	{
		Network.DBAction dbaction = ConnectAPI.DBAction();
		if (dbaction.Result != Network.DBAction.ResultType.SUCCESS)
		{
			Debug.LogError(string.Format("Unhandled dbAction {0} with error {1}", dbaction.Action, dbaction.Result));
		}
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00008970 File Offset: 0x00006B70
	private void OnBoosters()
	{
		this.OnNetCacheObjReceived<NetCache.NetCacheBoosters>(ConnectAPI.GetBoosters());
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0000897D File Offset: 0x00006B7D
	private void OnCollection()
	{
		this.OnNetCacheObjReceived<NetCache.NetCacheCollection>(ConnectAPI.GetCollectionCardStacks());
	}

	// Token: 0x06000199 RID: 409 RVA: 0x0000898A File Offset: 0x00006B8A
	private void OnDecks()
	{
		this.OnNetCacheObjReceived<NetCache.NetCacheDecks>(ConnectAPI.GetDeckHeaders());
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00008998 File Offset: 0x00006B98
	private void OnCardValues()
	{
		NetCache.NetCacheCardValues netCacheCardValues = new NetCache.NetCacheCardValues();
		CardValues cardValues = ConnectAPI.GetCardValues();
		if (cardValues != null || Network.ShouldBeConnectedToAurora())
		{
			netCacheCardValues.CardNerfIndex = cardValues.CardNerfIndex;
			foreach (PegasusUtil.CardValue cardValue in cardValues.Cards)
			{
				string text = GameUtils.TranslateDbIdToCardId(cardValue.Card.Asset);
				if (text == null)
				{
					Debug.LogError(string.Format("NetCache.OnCardValues(): Cannot find card '{0}' in card manifest.  Confirm your card manifest matches your game server's database.", cardValue.Card.Asset));
				}
				else
				{
					netCacheCardValues.Values.Add(new NetCache.CardDefinition
					{
						Name = text,
						Premium = (TAG_PREMIUM)cardValue.Card.Premium
					}, new NetCache.CardValue
					{
						Buy = cardValue.Buy,
						Sell = cardValue.Sell,
						Nerfed = cardValue.Nerfed
					});
				}
			}
		}
		this.OnNetCacheObjReceived<NetCache.NetCacheCardValues>(netCacheCardValues);
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00008AB8 File Offset: 0x00006CB8
	private void OnMedalInfo()
	{
		NetCache.NetCacheMedalInfo medalInfo = ConnectAPI.GetMedalInfo();
		if (this.m_previousMedalInfo != null)
		{
			medalInfo.PreviousMedalInfo = this.m_previousMedalInfo.Clone();
		}
		this.m_previousMedalInfo = medalInfo;
		this.OnNetCacheObjReceived<NetCache.NetCacheMedalInfo>(medalInfo);
	}

	// Token: 0x0600019C RID: 412 RVA: 0x00008AF8 File Offset: 0x00006CF8
	private void OnFeaturesChanged()
	{
		NetCache.NetCacheFeatures features = ConnectAPI.GetFeatures();
		this.OnNetCacheObjReceived<NetCache.NetCacheFeatures>(features);
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00008B14 File Offset: 0x00006D14
	private void OnArcaneDustBalance()
	{
		this.OnNetCacheObjReceived<NetCache.NetCacheArcaneDustBalance>(new NetCache.NetCacheArcaneDustBalance
		{
			Balance = ConnectAPI.GetArcaneDustBalance()
		});
	}

	// Token: 0x0600019E RID: 414 RVA: 0x00008B3C File Offset: 0x00006D3C
	private void OnGoldBalance()
	{
		NetCache.NetCacheGoldBalance goldBalance = ConnectAPI.GetGoldBalance();
		this.OnNetCacheObjReceived<NetCache.NetCacheGoldBalance>(goldBalance);
		NetCache.DelGoldBalanceListener[] array = this.m_goldBalanceListeners.ToArray();
		foreach (NetCache.DelGoldBalanceListener delGoldBalanceListener in array)
		{
			delGoldBalanceListener(goldBalance);
		}
	}

	// Token: 0x0600019F RID: 415 RVA: 0x00008B88 File Offset: 0x00006D88
	private void OnGamesInfo()
	{
		NetCache.NetCacheGamesPlayed gamesInfo = ConnectAPI.GetGamesInfo();
		if (gamesInfo == null)
		{
			Debug.LogWarning("error getting games info");
			return;
		}
		this.OnNetCacheObjReceived<NetCache.NetCacheGamesPlayed>(gamesInfo);
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x00008BB3 File Offset: 0x00006DB3
	private void OnProfileProgress()
	{
		this.OnNetCacheObjReceived<NetCache.NetCacheProfileProgress>(ConnectAPI.GetProfileProgress());
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x00008BC0 File Offset: 0x00006DC0
	private void OnHearthstoneUnavailableGame()
	{
		this.OnHearthstoneUnavailable(true);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x00008BC9 File Offset: 0x00006DC9
	private void OnHearthstoneUnavailableUtil()
	{
		this.OnHearthstoneUnavailable(false);
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x00008BD4 File Offset: 0x00006DD4
	private void OnHearthstoneUnavailable(bool gamePacket)
	{
		Network.UnavailableReason hearthstoneUnavailable = Network.GetHearthstoneUnavailable(gamePacket);
		Debug.Log("Hearthstone Unavailable!  Reason: " + hearthstoneUnavailable.mainReason);
		string mainReason = hearthstoneUnavailable.mainReason;
		if (mainReason != null)
		{
			if (NetCache.<>f__switch$map76 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("VERSION", 0);
				dictionary.Add("OFFLINE", 1);
				NetCache.<>f__switch$map76 = dictionary;
			}
			int num;
			if (NetCache.<>f__switch$map76.TryGetValue(mainReason, ref num))
			{
				if (num == 0)
				{
					ErrorParams errorParams = new ErrorParams();
					errorParams.m_message = GameStrings.Format("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UPGRADE", new object[0]);
					if (Error.HAS_APP_STORE)
					{
						errorParams.m_redirectToStore = true;
					}
					Error.AddFatal(errorParams);
					return;
				}
				if (num == 1)
				{
					Network.Get().ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_UNAVAILABLE_OFFLINE");
					return;
				}
			}
		}
		BIReport.Get().Report_Telemetry(Telemetry.Level.LEVEL_ERROR, BIReport.TelemetryEvent.EVENT_ERROR_NETWORK_UNAVAILABLE, 1, hearthstoneUnavailable.mainReason);
		Network.Get().ShowConnectionFailureError("GLOBAL_ERROR_NETWORK_UNAVAILABLE_UNKNOWN");
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x00008CDC File Offset: 0x00006EDC
	private void OnPlayQueue()
	{
		this.OnNetCacheObjReceived<NetCache.NetCachePlayQueue>(ConnectAPI.GetPlayQueue());
		Log.Bob.Print("play queue {0}", new object[]
		{
			NetCache.Get().GetNetObject<NetCache.NetCachePlayQueue>().GameType
		});
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x00008D20 File Offset: 0x00006F20
	private void OnCardBacks()
	{
		this.OnNetCacheObjReceived<NetCache.NetCacheCardBacks>(ConnectAPI.GetCardBacks());
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x00008D2D File Offset: 0x00006F2D
	private void OnPlayerRecords()
	{
		this.OnNetCacheObjReceived<NetCache.NetCachePlayerRecords>(ConnectAPI.GetPlayerRecords());
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x00008D3A File Offset: 0x00006F3A
	private void OnRewardProgress()
	{
		this.OnNetCacheObjReceived<NetCache.NetCacheRewardProgress>(ConnectAPI.GetRewardProgress());
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x00008D48 File Offset: 0x00006F48
	private void OnAllHeroXP()
	{
		NetCache.NetCacheHeroLevels allHeroXP = ConnectAPI.GetAllHeroXP();
		if (this.m_prevHeroLevels != null)
		{
			NetCache.HeroLevel newHeroLevel;
			foreach (NetCache.HeroLevel newHeroLevel2 in allHeroXP.Levels)
			{
				newHeroLevel = newHeroLevel2;
				NetCache.HeroLevel heroLevel = this.m_prevHeroLevels.Levels.Find((NetCache.HeroLevel obj) => obj.Class == newHeroLevel.Class);
				if (heroLevel != null)
				{
					if (newHeroLevel != null && newHeroLevel.CurrentLevel != null && newHeroLevel.CurrentLevel.Level != heroLevel.CurrentLevel.Level && (newHeroLevel.CurrentLevel.Level == 20 || newHeroLevel.CurrentLevel.Level == 30 || newHeroLevel.CurrentLevel.Level == 40 || newHeroLevel.CurrentLevel.Level == 50 || newHeroLevel.CurrentLevel.Level == 60))
					{
						if (newHeroLevel.Class == TAG_CLASS.DRUID)
						{
							BnetPresenceMgr.Get().SetGameField(5U, newHeroLevel.CurrentLevel.Level);
						}
						else if (newHeroLevel.Class == TAG_CLASS.HUNTER)
						{
							BnetPresenceMgr.Get().SetGameField(6U, newHeroLevel.CurrentLevel.Level);
						}
						else if (newHeroLevel.Class == TAG_CLASS.MAGE)
						{
							BnetPresenceMgr.Get().SetGameField(7U, newHeroLevel.CurrentLevel.Level);
						}
						else if (newHeroLevel.Class == TAG_CLASS.PALADIN)
						{
							BnetPresenceMgr.Get().SetGameField(8U, newHeroLevel.CurrentLevel.Level);
						}
						else if (newHeroLevel.Class == TAG_CLASS.PRIEST)
						{
							BnetPresenceMgr.Get().SetGameField(9U, newHeroLevel.CurrentLevel.Level);
						}
						else if (newHeroLevel.Class == TAG_CLASS.ROGUE)
						{
							BnetPresenceMgr.Get().SetGameField(10U, newHeroLevel.CurrentLevel.Level);
						}
						else if (newHeroLevel.Class == TAG_CLASS.SHAMAN)
						{
							BnetPresenceMgr.Get().SetGameField(11U, newHeroLevel.CurrentLevel.Level);
						}
						else if (newHeroLevel.Class == TAG_CLASS.WARLOCK)
						{
							BnetPresenceMgr.Get().SetGameField(12U, newHeroLevel.CurrentLevel.Level);
						}
						else if (newHeroLevel.Class == TAG_CLASS.WARRIOR)
						{
							BnetPresenceMgr.Get().SetGameField(13U, newHeroLevel.CurrentLevel.Level);
						}
					}
					newHeroLevel.PrevLevel = heroLevel.CurrentLevel;
				}
			}
		}
		this.m_prevHeroLevels = allHeroXP;
		this.OnNetCacheObjReceived<NetCache.NetCacheHeroLevels>(allHeroXP);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000907C File Offset: 0x0000727C
	private void OnProfileNotices()
	{
		List<NetCache.ProfileNotice> profileNotices = ConnectAPI.GetProfileNotices();
		NetCache.ProfileNotice profileNotice = profileNotices.Find((NetCache.ProfileNotice obj) => obj.Type == NetCache.ProfileNotice.NoticeType.GAINED_MEDAL);
		if (profileNotice != null)
		{
			this.m_previousMedalInfo = null;
			NetCache.NetCacheMedalInfo netObject = this.GetNetObject<NetCache.NetCacheMedalInfo>();
			if (netObject != null)
			{
				netObject.PreviousMedalInfo = null;
			}
		}
		List<NetCache.ProfileNotice> list = this.FindNewNotices(profileNotices);
		NetCache.NetCacheProfileNotices netCacheProfileNotices = new NetCache.NetCacheProfileNotices();
		netCacheProfileNotices.Notices.AddRange(profileNotices);
		this.OnNetCacheObjReceived<NetCache.NetCacheProfileNotices>(netCacheProfileNotices);
		NetCache.DelNewNoticesListener[] array = this.m_newNoticesListeners.ToArray();
		Log.Rachelle.Print("NetCache.OnProfileNotices() sending {0} new notices to {1} listeners", new object[]
		{
			list.Count,
			array.Length
		});
		foreach (NetCache.DelNewNoticesListener delNewNoticesListener in array)
		{
			delNewNoticesListener(list);
		}
	}

	// Token: 0x060001AA RID: 426 RVA: 0x00009160 File Offset: 0x00007360
	private List<NetCache.ProfileNotice> FindNewNotices(List<NetCache.ProfileNotice> receivedNotices)
	{
		List<NetCache.ProfileNotice> list = new List<NetCache.ProfileNotice>();
		NetCache.NetCacheProfileNotices netObject = this.GetNetObject<NetCache.NetCacheProfileNotices>();
		if (netObject == null)
		{
			list.AddRange(receivedNotices);
		}
		else
		{
			NetCache.ProfileNotice receivedNotice;
			foreach (NetCache.ProfileNotice receivedNotice2 in receivedNotices)
			{
				receivedNotice = receivedNotice2;
				NetCache.ProfileNotice profileNotice = netObject.Notices.Find((NetCache.ProfileNotice obj) => obj.NoticeID == receivedNotice.NoticeID);
				if (profileNotice == null)
				{
					list.Add(receivedNotice);
				}
			}
		}
		return list;
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000920C File Offset: 0x0000740C
	private void OnClientOptions()
	{
		NetCache.NetCacheClientOptions netCacheClientOptions = this.GetNetObject<NetCache.NetCacheClientOptions>();
		bool flag = netCacheClientOptions == null;
		if (flag)
		{
			netCacheClientOptions = new NetCache.NetCacheClientOptions();
		}
		ConnectAPI.ReadClientOptions(netCacheClientOptions.ClientState, netCacheClientOptions.ServerState);
		this.OnNetCacheObjReceived<NetCache.NetCacheClientOptions>(netCacheClientOptions);
		if (flag)
		{
			OptionsMigration.UpgradeServerOptions();
		}
	}

	// Token: 0x060001AC RID: 428 RVA: 0x00009258 File Offset: 0x00007458
	private void SetClientOption(ServerOption type, NetCache.ClientOptionBase newVal)
	{
		NetCache.NetCacheClientOptions netCacheClientOptions = (NetCache.NetCacheClientOptions)this.m_netCache[typeof(NetCache.NetCacheClientOptions)];
		netCacheClientOptions.ClientState[type] = newVal;
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (netObject != null && netObject.Misc != null && netObject.Misc.ClientOptionsUpdateIntervalSeconds > 0)
		{
			ApplicationMgr.Get().ScheduleCallback((float)netObject.Misc.ClientOptionsUpdateIntervalSeconds, true, new ApplicationMgr.ScheduledCallback(netCacheClientOptions.OnUpdateIntervalElasped), null);
		}
		this.NetCacheChanged<NetCache.NetCacheClientOptions>();
	}

	// Token: 0x060001AD RID: 429 RVA: 0x000092E5 File Offset: 0x000074E5
	public void SetBoolOption(ServerOption type, bool val)
	{
		this.SetClientOption(type, new NetCache.ClientOptionBool(val));
	}

	// Token: 0x060001AE RID: 430 RVA: 0x000092F4 File Offset: 0x000074F4
	public void SetIntOption(ServerOption type, int val)
	{
		this.SetClientOption(type, new NetCache.ClientOptionInt(val));
	}

	// Token: 0x060001AF RID: 431 RVA: 0x00009303 File Offset: 0x00007503
	public void SetLongOption(ServerOption type, long val)
	{
		this.SetClientOption(type, new NetCache.ClientOptionLong(val));
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x00009312 File Offset: 0x00007512
	public void SetFloatOption(ServerOption type, float val)
	{
		this.SetClientOption(type, new NetCache.ClientOptionFloat(val));
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x00009321 File Offset: 0x00007521
	public void SetULongOption(ServerOption type, ulong val)
	{
		this.SetClientOption(type, new NetCache.ClientOptionULong(val));
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x00009330 File Offset: 0x00007530
	public void DeleteClientOption(ServerOption type)
	{
		this.SetClientOption(type, null);
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000933C File Offset: 0x0000753C
	public bool ClientOptionExists(ServerOption type)
	{
		NetCache.NetCacheClientOptions netObject = this.GetNetObject<NetCache.NetCacheClientOptions>();
		if (netObject == null)
		{
			return false;
		}
		if (!netObject.ClientState.ContainsKey(type))
		{
			return false;
		}
		NetCache.ClientOptionBase clientOptionBase = netObject.ClientState[type];
		return clientOptionBase != null;
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x00009380 File Offset: 0x00007580
	private void OnLastGameInfo()
	{
		NetCache.NetCacheDisconnectedGame disconnectedGameInfo = ConnectAPI.GetDisconnectedGameInfo();
		this.OnNetCacheObjReceived<NetCache.NetCacheDisconnectedGame>(disconnectedGameInfo);
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000939C File Offset: 0x0000759C
	private void OnNotSoMassiveLoginReply()
	{
		NotSoMassiveLoginReply notSoMassiveLoginReply = ConnectAPI.GetNotSoMassiveLoginReply();
		if (notSoMassiveLoginReply != null)
		{
			SpecialEventManager.Get().InitEventTiming(notSoMassiveLoginReply.SpecialEventTiming);
		}
		NetCache.NetCacheNotSoMassiveLogin netCacheNotSoMassiveLogin = new NetCache.NetCacheNotSoMassiveLogin();
		netCacheNotSoMassiveLogin.Packet = notSoMassiveLoginReply;
		if (notSoMassiveLoginReply.HasTavernBrawls)
		{
			this.OnNetCacheObjReceived<NetCache.NetCacheTavernBrawlInfo>(new NetCache.NetCacheTavernBrawlInfo(notSoMassiveLoginReply.TavernBrawls));
			if (notSoMassiveLoginReply.TavernBrawls.HasCurrentTavernBrawl && notSoMassiveLoginReply.TavernBrawls.CurrentTavernBrawl.MyRecord != null)
			{
				this.OnNetCacheObjReceived<NetCache.NetCacheTavernBrawlRecord>(new NetCache.NetCacheTavernBrawlRecord(notSoMassiveLoginReply.TavernBrawls.CurrentTavernBrawl.MyRecord));
			}
		}
		this.OnNetCacheObjReceived<NetCache.NetCacheNotSoMassiveLogin>(netCacheNotSoMassiveLogin);
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x00009438 File Offset: 0x00007638
	private void OnTavernBrawlInfoResponse()
	{
		TavernBrawlInfo tavernBrawlInfoPacket = ConnectAPI.GetTavernBrawlInfoPacket();
		this.OnNetCacheObjReceived<NetCache.NetCacheTavernBrawlInfo>(new NetCache.NetCacheTavernBrawlInfo(tavernBrawlInfoPacket));
		if (tavernBrawlInfoPacket.HasCurrentTavernBrawl && tavernBrawlInfoPacket.CurrentTavernBrawl.MyRecord != null)
		{
			this.OnNetCacheObjReceived<NetCache.NetCacheTavernBrawlRecord>(new NetCache.NetCacheTavernBrawlRecord(tavernBrawlInfoPacket.CurrentTavernBrawl.MyRecord));
		}
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x00009488 File Offset: 0x00007688
	private void OnTavernBrawlRecordResponse()
	{
		TavernBrawlPlayerRecord tavernBrawlRecordPacket = ConnectAPI.GetTavernBrawlRecordPacket();
		this.OnNetCacheObjReceived<NetCache.NetCacheTavernBrawlRecord>(new NetCache.NetCacheTavernBrawlRecord(tavernBrawlRecordPacket));
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x000094A8 File Offset: 0x000076A8
	private void OnFavoriteHeroesResponse()
	{
		FavoriteHeroesResponse favoriteHeroesResponse = ConnectAPI.GetFavoriteHeroesResponse();
		NetCache.NetCacheFavoriteHeroes netCacheFavoriteHeroes = new NetCache.NetCacheFavoriteHeroes();
		foreach (FavoriteHero favoriteHero in favoriteHeroesResponse.FavoriteHeroes)
		{
			TAG_CLASS tag_CLASS;
			TAG_PREMIUM premium;
			if (!EnumUtils.TryCast<TAG_CLASS>(favoriteHero.ClassId, out tag_CLASS))
			{
				Debug.LogWarning(string.Format("NetCache.OnFavoriteHeroesResponse() unrecognized hero class {0}", favoriteHero.ClassId));
			}
			else if (!EnumUtils.TryCast<TAG_PREMIUM>(favoriteHero.Hero.Premium, out premium))
			{
				Debug.LogWarning(string.Format("NetCache.OnFavoriteHeroesResponse() unrecognized hero premium {0} for hero class {1}", favoriteHero.Hero.Premium, tag_CLASS));
			}
			else
			{
				NetCache.CardDefinition value = new NetCache.CardDefinition
				{
					Name = GameUtils.TranslateDbIdToCardId(favoriteHero.Hero.Asset),
					Premium = premium
				};
				netCacheFavoriteHeroes.FavoriteHeroes[tag_CLASS] = value;
			}
		}
		this.OnNetCacheObjReceived<NetCache.NetCacheFavoriteHeroes>(netCacheFavoriteHeroes);
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x000095C8 File Offset: 0x000077C8
	private void OnAccountLicensesInfoResponse()
	{
		AccountLicensesInfoResponse accountLicensesInfoResponse = ConnectAPI.GetAccountLicensesInfoResponse();
		NetCache.NetCacheAccountLicenses netCacheAccountLicenses = new NetCache.NetCacheAccountLicenses();
		foreach (AccountLicenseInfo accountLicenseInfo in accountLicensesInfoResponse.List)
		{
			netCacheAccountLicenses.AccountLicenses[accountLicenseInfo.License] = accountLicenseInfo;
		}
		this.OnNetCacheObjReceived<NetCache.NetCacheAccountLicenses>(netCacheAccountLicenses);
	}

	// Token: 0x060001BA RID: 442 RVA: 0x00009640 File Offset: 0x00007840
	private void RegisterNetCacheHandlers()
	{
		Network network = Network.Get();
		network.RegisterNetHandler(216, new Network.NetHandler(this.OnDBAction), null);
		network.RegisterNetHandler(326, new Network.NetHandler(this.OnGenericResponse), null);
		network.RegisterNetHandler(224, new Network.NetHandler(this.OnBoosters), null);
		network.RegisterNetHandler(207, new Network.NetHandler(this.OnCollection), null);
		network.RegisterNetHandler(202, new Network.NetHandler(this.OnDecks), null);
		network.RegisterNetHandler(232, new Network.NetHandler(this.OnMedalInfo), null);
		network.RegisterNetHandler(233, new Network.NetHandler(this.OnProfileProgress), null);
		network.RegisterNetHandler(208, new Network.NetHandler(this.OnGamesInfo), null);
		network.RegisterNetHandler(212, new Network.NetHandler(this.OnProfileNotices), null);
		network.RegisterNetHandler(241, new Network.NetHandler(this.OnClientOptions), null);
		network.RegisterNetHandler(289, new Network.NetHandler(this.OnLastGameInfo), null);
		network.RegisterNetHandler(260, new Network.NetHandler(this.OnCardValues), null);
		network.RegisterNetHandler(262, new Network.NetHandler(this.OnArcaneDustBalance), null);
		network.RegisterNetHandler(278, new Network.NetHandler(this.OnGoldBalance), null);
		network.RegisterNetHandler(264, new Network.NetHandler(this.OnFeaturesChanged), null);
		network.RegisterNetHandler(270, new Network.NetHandler(this.OnPlayerRecords), null);
		network.RegisterNetHandler(271, new Network.NetHandler(this.OnRewardProgress), null);
		network.RegisterNetHandler(283, new Network.NetHandler(this.OnAllHeroXP), null);
		network.RegisterNetHandler(286, new Network.NetHandler(this.OnPlayQueue), null);
		network.RegisterNetHandler(236, new Network.NetHandler(this.OnCardBacks), null);
		network.RegisterNetHandler(300, new Network.NetHandler(this.OnNotSoMassiveLoginReply), null);
		network.RegisterNetHandler(316, new Network.NetHandler(this.OnTavernBrawlInfoResponse), null);
		network.RegisterNetHandler(317, new Network.NetHandler(this.OnTavernBrawlRecordResponse), null);
		network.RegisterNetHandler(318, new Network.NetHandler(this.OnFavoriteHeroesResponse), null);
		network.RegisterNetHandler(325, new Network.NetHandler(this.OnAccountLicensesInfoResponse), null);
		network.RegisterNetHandler(169, new Network.NetHandler(this.OnHearthstoneUnavailableGame), null);
		network.RegisterNetHandler(167, new Network.NetHandler(this.OnHearthstoneUnavailableUtil), null);
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000997D File Offset: 0x00007B7D
	public void InitNetCache()
	{
		ConnectAPI.RegisterThrottledPacketListener(new ConnectAPI.ThrottledPacketListener(this.OnPacketThrottled));
		this.RegisterNetCacheHandlers();
	}

	// Token: 0x060001BC RID: 444 RVA: 0x00009996 File Offset: 0x00007B96
	public static NetCache Get()
	{
		if (NetCache.s_instance == null)
		{
			Debug.LogError("no NetCache object");
		}
		return NetCache.s_instance;
	}

	// Token: 0x060001BD RID: 445 RVA: 0x000099B1 File Offset: 0x00007BB1
	public void RegisterCollectionManager(NetCache.NetCacheCallback callback)
	{
		this.RegisterCollectionManager(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001BE RID: 446 RVA: 0x000099C8 File Offset: 0x00007BC8
	public void RegisterCollectionManager(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterCollectionManager---", new object[0]);
		NetCache.NetCacheBatchRequest batchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterCollectionManager));
		this.AddCollectionManagerToRequest(ref batchRequest);
		this.NetCacheMakeBatchRequest(batchRequest);
	}

	// Token: 0x060001BF RID: 447 RVA: 0x00009A0D File Offset: 0x00007C0D
	public void RegisterScreenCollectionManager(NetCache.NetCacheCallback callback)
	{
		this.RegisterScreenCollectionManager(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x00009A24 File Offset: 0x00007C24
	public void RegisterScreenCollectionManager(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterScreenCollectionManager---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenCollectionManager));
		this.AddCollectionManagerToRequest(ref netCacheBatchRequest);
		this.AddRandomDeckMakerToRequest(ref netCacheBatchRequest);
		NetCache.NetCacheBatchRequest netCacheBatchRequest2 = netCacheBatchRequest;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheFeatures), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheHeroLevels), false));
		netCacheBatchRequest2.AddRequests(list);
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x00009AAA File Offset: 0x00007CAA
	public void RegisterScreenForge(NetCache.NetCacheCallback callback)
	{
		this.RegisterScreenForge(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x00009AC0 File Offset: 0x00007CC0
	public void RegisterScreenForge(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterScreenForge---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenForge));
		this.AddCollectionManagerToRequest(ref netCacheBatchRequest);
		NetCache.NetCacheBatchRequest netCacheBatchRequest2 = netCacheBatchRequest;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheFeatures), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheHeroLevels), false));
		netCacheBatchRequest2.AddRequests(list);
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x00009B3E File Offset: 0x00007D3E
	public void RegisterScreenTourneys(NetCache.NetCacheCallback callback)
	{
		this.RegisterScreenTourneys(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x00009B54 File Offset: 0x00007D54
	public void RegisterScreenTourneys(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterScreenTourneys---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenTourneys));
		NetCache.NetCacheBatchRequest netCacheBatchRequest2 = netCacheBatchRequest;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheMedalInfo), true));
		list.Add(new NetCache.Request(typeof(NetCache.NetCachePlayerRecords), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheDecks), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheFeatures), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheHeroLevels), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCachePlayQueue), true));
		netCacheBatchRequest2.AddRequests(list);
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x00009C22 File Offset: 0x00007E22
	public void RegisterScreenFriendly(NetCache.NetCacheCallback callback)
	{
		this.RegisterScreenFriendly(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x00009C38 File Offset: 0x00007E38
	public void RegisterScreenFriendly(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterScreenFriendly---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenFriendly));
		NetCache.NetCacheBatchRequest netCacheBatchRequest2 = netCacheBatchRequest;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheDecks), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheHeroLevels), false));
		netCacheBatchRequest2.AddRequests(list);
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x00009CAE File Offset: 0x00007EAE
	public void RegisterScreenPractice(NetCache.NetCacheCallback callback)
	{
		this.RegisterScreenPractice(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x00009CC4 File Offset: 0x00007EC4
	public void RegisterScreenPractice(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterScreenPractice---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenPractice));
		NetCache.NetCacheBatchRequest netCacheBatchRequest2 = netCacheBatchRequest;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheDecks), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheFeatures), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheHeroLevels), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheRewardProgress), false));
		netCacheBatchRequest2.AddRequests(list);
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x00009D66 File Offset: 0x00007F66
	public void RegisterScreenEndOfGame(NetCache.NetCacheCallback callback)
	{
		this.RegisterScreenEndOfGame(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001CA RID: 458 RVA: 0x00009D7C File Offset: 0x00007F7C
	public void RegisterScreenEndOfGame(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterScreenEndOfGame---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenEndOfGame));
		NetCache.NetCacheBatchRequest netCacheBatchRequest2 = netCacheBatchRequest;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheRewardProgress), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheMedalInfo), true));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheGamesPlayed), true));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheProfileNotices), true));
		list.Add(new NetCache.Request(typeof(NetCache.NetCachePlayerRecords), true));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheHeroLevels), true));
		netCacheBatchRequest2.AddRequests(list);
		if (GameMgr.Get() != null && GameMgr.Get().GetGameType() == 16)
		{
			netCacheBatchRequest.AddRequest(new NetCache.Request(typeof(NetCache.NetCacheTavernBrawlRecord), true));
		}
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001CB RID: 459 RVA: 0x00009E7B File Offset: 0x0000807B
	public void RegisterScreenPackOpening(NetCache.NetCacheCallback callback)
	{
		this.RegisterScreenPackOpening(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001CC RID: 460 RVA: 0x00009E90 File Offset: 0x00008090
	public void RegisterScreenPackOpening(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterScreenPackOpening---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenPackOpening));
		netCacheBatchRequest.AddRequest(new NetCache.Request(typeof(NetCache.NetCacheBoosters), false));
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001CD RID: 461 RVA: 0x00009EE3 File Offset: 0x000080E3
	public void RegisterScreenBox(NetCache.NetCacheCallback callback)
	{
		this.RegisterScreenBox(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001CE RID: 462 RVA: 0x00009EF8 File Offset: 0x000080F8
	public void RegisterScreenBox(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterScreenBox---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenBox));
		NetCache.NetCacheBatchRequest netCacheBatchRequest2 = netCacheBatchRequest;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheBoosters), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheClientOptions), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheProfileProgress), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheFeatures), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheMedalInfo), false));
		netCacheBatchRequest2.AddRequests(list);
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001CF RID: 463 RVA: 0x00009FB0 File Offset: 0x000081B0
	public void RegisterScreenQuestLog(NetCache.NetCacheCallback callback)
	{
		this.RegisterScreenQuestLog(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x00009FC8 File Offset: 0x000081C8
	public void RegisterScreenQuestLog(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterScreenQuestLog---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenQuestLog));
		NetCache.NetCacheBatchRequest netCacheBatchRequest2 = netCacheBatchRequest;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheMedalInfo), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheHeroLevels), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCachePlayerRecords), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheProfileProgress), true));
		netCacheBatchRequest2.AddRequests(list);
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x0000A06A File Offset: 0x0000826A
	public void RegisterFeatures(NetCache.NetCacheCallback callback)
	{
		this.RegisterFeatures(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000A080 File Offset: 0x00008280
	public void RegisterFeatures(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterFeatures---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterFeatures));
		netCacheBatchRequest.AddRequest(new NetCache.Request(typeof(NetCache.NetCacheFeatures), false));
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0000A0D3 File Offset: 0x000082D3
	public void RegisterScreenLogin(NetCache.NetCacheCallback callback)
	{
		this.RegisterScreenLogin(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x0000A0E8 File Offset: 0x000082E8
	public void RegisterScreenLogin(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterScreenLogin---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterScreenLogin));
		NetCache.NetCacheBatchRequest netCacheBatchRequest2 = netCacheBatchRequest;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheNotSoMassiveLogin), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheProfileProgress), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheRewardProgress), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCachePlayerRecords), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheGoldBalance), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheHeroLevels), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheCardBacks), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheFavoriteHeroes), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheAccountLicenses), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheClientOptions), true));
		netCacheBatchRequest2.AddRequests(list);
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
		TavernBrawlManager.Get().IsRefreshingTavernBrawlInfo = true;
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x0000A219 File Offset: 0x00008419
	public void RegisterReconnectMgr(NetCache.NetCacheCallback callback)
	{
		this.RegisterReconnectMgr(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0000A230 File Offset: 0x00008430
	public void RegisterReconnectMgr(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterReconnectMgr---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterReconnectMgr));
		netCacheBatchRequest.AddRequest(new NetCache.Request(typeof(NetCache.NetCacheDisconnectedGame), false));
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x0000A283 File Offset: 0x00008483
	public void RegisterTutorialEndGameScreen(NetCache.NetCacheCallback callback)
	{
		this.RegisterTutorialEndGameScreen(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x0000A298 File Offset: 0x00008498
	public void RegisterTutorialEndGameScreen(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterTutorialEndGameScreen---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterTutorialEndGameScreen));
		NetCache.NetCacheBatchRequest netCacheBatchRequest2 = netCacheBatchRequest;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheProfileProgress), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheProfileNotices), true));
		netCacheBatchRequest2.AddRequests(list);
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x0000A30E File Offset: 0x0000850E
	public void RegisterFriendChallenge(NetCache.NetCacheCallback callback)
	{
		this.RegisterFriendChallenge(callback, new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x060001DA RID: 474 RVA: 0x0000A324 File Offset: 0x00008524
	public void RegisterFriendChallenge(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterFriendChallenge---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterFriendChallenge));
		netCacheBatchRequest.AddRequest(new NetCache.Request(typeof(NetCache.NetCacheProfileProgress), false));
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001DB RID: 475 RVA: 0x0000A378 File Offset: 0x00008578
	public void RegisterNotices(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback)
	{
		Log.Bob.Print("---RegisterFeatures---", new object[0]);
		NetCache.NetCacheBatchRequest netCacheBatchRequest = new NetCache.NetCacheBatchRequest(callback, errorCallback, new NetCache.RequestFunc(this.RegisterNotices));
		netCacheBatchRequest.AddRequest(new NetCache.Request(typeof(NetCache.NetCacheProfileNotices), false));
		this.NetCacheMakeBatchRequest(netCacheBatchRequest);
	}

	// Token: 0x060001DC RID: 476 RVA: 0x0000A3CC File Offset: 0x000085CC
	private void AddCollectionManagerToRequest(ref NetCache.NetCacheBatchRequest request)
	{
		NetCache.NetCacheBatchRequest netCacheBatchRequest = request;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheDecks), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheCollection), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheCardValues), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheArcaneDustBalance), false));
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheClientOptions), false));
		netCacheBatchRequest.AddRequests(list);
	}

	// Token: 0x060001DD RID: 477 RVA: 0x0000A458 File Offset: 0x00008658
	private void AddRandomDeckMakerToRequest(ref NetCache.NetCacheBatchRequest request)
	{
		NetCache.NetCacheBatchRequest netCacheBatchRequest = request;
		List<NetCache.Request> list = new List<NetCache.Request>();
		list.Add(new NetCache.Request(typeof(NetCache.NetCacheCollection), false));
		netCacheBatchRequest.AddRequests(list);
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0000A48C File Offset: 0x0000868C
	private void OnPacketThrottled(int packetID, long retryMillis)
	{
		if (packetID != 201)
		{
			return;
		}
		DateTime timeAdded = DateTime.Now.AddMilliseconds((double)retryMillis);
		foreach (NetCache.NetCacheBatchRequest netCacheBatchRequest in this.m_cacheRequests)
		{
			netCacheBatchRequest.m_timeAdded = timeAdded;
		}
	}

	// Token: 0x0400005C RID: 92
	private static readonly Map<Type, GetAccountInfo.Request> m_typeMap = new Map<Type, GetAccountInfo.Request>
	{
		{
			typeof(NetCache.NetCacheDecks),
			2
		},
		{
			typeof(NetCache.NetCacheCollection),
			3
		},
		{
			typeof(NetCache.NetCacheMedalInfo),
			4
		},
		{
			typeof(NetCache.NetCacheBoosters),
			6
		},
		{
			typeof(NetCache.NetCacheCardBacks),
			7
		},
		{
			typeof(NetCache.NetCachePlayerRecords),
			8
		},
		{
			typeof(NetCache.NetCacheGamesPlayed),
			9
		},
		{
			typeof(NetCache.NetCacheProfileProgress),
			11
		},
		{
			typeof(NetCache.NetCacheProfileNotices),
			12
		},
		{
			typeof(NetCache.NetCacheClientOptions),
			14
		},
		{
			typeof(NetCache.NetCacheCardValues),
			15
		},
		{
			typeof(NetCache.NetCacheDisconnectedGame),
			16
		},
		{
			typeof(NetCache.NetCacheArcaneDustBalance),
			17
		},
		{
			typeof(NetCache.NetCacheFeatures),
			18
		},
		{
			typeof(NetCache.NetCacheRewardProgress),
			19
		},
		{
			typeof(NetCache.NetCacheGoldBalance),
			20
		},
		{
			typeof(NetCache.NetCacheHeroLevels),
			21
		},
		{
			typeof(NetCache.NetCachePlayQueue),
			22
		},
		{
			typeof(NetCache.NetCacheNotSoMassiveLogin),
			23
		},
		{
			typeof(NetCache.NetCacheTavernBrawlInfo),
			25
		},
		{
			typeof(NetCache.NetCacheTavernBrawlRecord),
			26
		},
		{
			typeof(NetCache.NetCacheFavoriteHeroes),
			27
		},
		{
			typeof(NetCache.NetCacheAccountLicenses),
			28
		}
	};

	// Token: 0x0400005D RID: 93
	private static readonly Map<GetAccountInfo.Request, Type> m_requestTypeMap = NetCache.GetInvertTypeMap();

	// Token: 0x0400005E RID: 94
	private Map<Type, object> m_netCache = new Map<Type, object>();

	// Token: 0x0400005F RID: 95
	private NetCache.NetCacheHeroLevels m_prevHeroLevels;

	// Token: 0x04000060 RID: 96
	private NetCache.NetCacheMedalInfo m_previousMedalInfo;

	// Token: 0x04000061 RID: 97
	private List<NetCache.DelNewNoticesListener> m_newNoticesListeners = new List<NetCache.DelNewNoticesListener>();

	// Token: 0x04000062 RID: 98
	private List<NetCache.DelGoldBalanceListener> m_goldBalanceListeners = new List<NetCache.DelGoldBalanceListener>();

	// Token: 0x04000063 RID: 99
	private Map<Type, List<Action>> m_updatedListeners = new Map<Type, List<Action>>();

	// Token: 0x04000064 RID: 100
	private Map<Type, int> m_changeRequests = new Map<Type, int>();

	// Token: 0x04000065 RID: 101
	private long m_lastForceCheckedSeason;

	// Token: 0x04000066 RID: 102
	private List<NetCache.NetCacheBatchRequest> m_cacheRequests = new List<NetCache.NetCacheBatchRequest>();

	// Token: 0x04000067 RID: 103
	private List<Type> m_inTransitRequests = new List<Type>();

	// Token: 0x04000068 RID: 104
	private static bool m_fatalErrorCodeSet = false;

	// Token: 0x04000069 RID: 105
	private static NetCache s_instance = new NetCache();

	// Token: 0x0200000F RID: 15
	public abstract class ProfileNotice
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x0000A510 File Offset: 0x00008710
		protected ProfileNotice(NetCache.ProfileNotice.NoticeType init)
		{
			this.m_type = init;
			this.NoticeID = 0L;
			this.Origin = NetCache.ProfileNotice.NoticeOrigin.UNKNOWN;
			this.OriginData = 0L;
			this.Date = 0L;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x0000A549 File Offset: 0x00008749
		// (set) Token: 0x060001E2 RID: 482 RVA: 0x0000A551 File Offset: 0x00008751
		public long NoticeID { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060001E3 RID: 483 RVA: 0x0000A55A File Offset: 0x0000875A
		public NetCache.ProfileNotice.NoticeType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000A562 File Offset: 0x00008762
		// (set) Token: 0x060001E5 RID: 485 RVA: 0x0000A56A File Offset: 0x0000876A
		public NetCache.ProfileNotice.NoticeOrigin Origin { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000A573 File Offset: 0x00008773
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x0000A57B File Offset: 0x0000877B
		public long OriginData { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x0000A584 File Offset: 0x00008784
		// (set) Token: 0x060001E9 RID: 489 RVA: 0x0000A58C File Offset: 0x0000878C
		public long Date { get; set; }

		// Token: 0x0400006C RID: 108
		private NetCache.ProfileNotice.NoticeType m_type;

		// Token: 0x0200002C RID: 44
		public enum NoticeType
		{
			// Token: 0x04000182 RID: 386
			GAINED_MEDAL = 1,
			// Token: 0x04000183 RID: 387
			REWARD_BOOSTER,
			// Token: 0x04000184 RID: 388
			REWARD_CARD,
			// Token: 0x04000185 RID: 389
			DISCONNECTED_GAME,
			// Token: 0x04000186 RID: 390
			PRECON_DECK,
			// Token: 0x04000187 RID: 391
			REWARD_DUST,
			// Token: 0x04000188 RID: 392
			REWARD_MOUNT,
			// Token: 0x04000189 RID: 393
			REWARD_FORGE,
			// Token: 0x0400018A RID: 394
			REWARD_GOLD,
			// Token: 0x0400018B RID: 395
			PURCHASE,
			// Token: 0x0400018C RID: 396
			REWARD_CARD_BACK,
			// Token: 0x0400018D RID: 397
			BONUS_STARS,
			// Token: 0x0400018E RID: 398
			ADVENTURE_PROGRESS = 14,
			// Token: 0x0400018F RID: 399
			HERO_LEVEL_UP,
			// Token: 0x04000190 RID: 400
			ACCOUNT_LICENSE
		}

		// Token: 0x020000CE RID: 206
		public enum NoticeOrigin
		{
			// Token: 0x0400061F RID: 1567
			UNKNOWN = -1,
			// Token: 0x04000620 RID: 1568
			SEASON = 1,
			// Token: 0x04000621 RID: 1569
			BETA_REIMBURSE,
			// Token: 0x04000622 RID: 1570
			FORGE,
			// Token: 0x04000623 RID: 1571
			TOURNEY,
			// Token: 0x04000624 RID: 1572
			PRECON_DECK,
			// Token: 0x04000625 RID: 1573
			ACK,
			// Token: 0x04000626 RID: 1574
			ACHIEVEMENT,
			// Token: 0x04000627 RID: 1575
			LEVEL_UP,
			// Token: 0x04000628 RID: 1576
			PURCHASE_COMPLETE = 10,
			// Token: 0x04000629 RID: 1577
			PURCHASE_FAILED,
			// Token: 0x0400062A RID: 1578
			PURCHASE_CANCELED,
			// Token: 0x0400062B RID: 1579
			BLIZZCON,
			// Token: 0x0400062C RID: 1580
			EVENT,
			// Token: 0x0400062D RID: 1581
			DISCONNECTED_GAME,
			// Token: 0x0400062E RID: 1582
			OUT_OF_BAND_LICENSE,
			// Token: 0x0400062F RID: 1583
			IGR,
			// Token: 0x04000630 RID: 1584
			ADVENTURE_PROGRESS,
			// Token: 0x04000631 RID: 1585
			ADVENTURE_FLAGS,
			// Token: 0x04000632 RID: 1586
			TAVERN_BRAWL_REWARD,
			// Token: 0x04000633 RID: 1587
			ACCOUNT_LICENSE_FLAGS
		}
	}

	// Token: 0x02000010 RID: 16
	// (Invoke) Token: 0x060001EB RID: 491
	public delegate void DelNewNoticesListener(List<NetCache.ProfileNotice> newNotices);

	// Token: 0x02000028 RID: 40
	public class NetCacheProfileProgress
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060003C4 RID: 964 RVA: 0x00011AEC File Offset: 0x0000FCEC
		// (set) Token: 0x060003C5 RID: 965 RVA: 0x00011AF4 File Offset: 0x0000FCF4
		public TutorialProgress CampaignProgress { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060003C6 RID: 966 RVA: 0x00011AFD File Offset: 0x0000FCFD
		// (set) Token: 0x060003C7 RID: 967 RVA: 0x00011B05 File Offset: 0x0000FD05
		public int BestForgeWins { get; set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x00011B0E File Offset: 0x0000FD0E
		// (set) Token: 0x060003C9 RID: 969 RVA: 0x00011B16 File Offset: 0x0000FD16
		public long LastForgeDate { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060003CA RID: 970 RVA: 0x00011B1F File Offset: 0x0000FD1F
		// (set) Token: 0x060003CB RID: 971 RVA: 0x00011B27 File Offset: 0x0000FD27
		public int DisplayBanner { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060003CC RID: 972 RVA: 0x00011B30 File Offset: 0x0000FD30
		// (set) Token: 0x060003CD RID: 973 RVA: 0x00011B38 File Offset: 0x0000FD38
		public AdventureOption[] AdventureOptions { get; set; }
	}

	// Token: 0x0200002B RID: 43
	public class ProfileNoticeAdventureProgress : NetCache.ProfileNotice
	{
		// Token: 0x060003EF RID: 1007 RVA: 0x0001210C File Offset: 0x0001030C
		public ProfileNoticeAdventureProgress() : base(NetCache.ProfileNotice.NoticeType.ADVENTURE_PROGRESS)
		{
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00012116 File Offset: 0x00010316
		// (set) Token: 0x060003F1 RID: 1009 RVA: 0x0001211E File Offset: 0x0001031E
		public int Wing { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00012127 File Offset: 0x00010327
		// (set) Token: 0x060003F3 RID: 1011 RVA: 0x0001212F File Offset: 0x0001032F
		public int? Progress { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060003F4 RID: 1012 RVA: 0x00012138 File Offset: 0x00010338
		// (set) Token: 0x060003F5 RID: 1013 RVA: 0x00012140 File Offset: 0x00010340
		public ulong? Flags { get; set; }
	}

	// Token: 0x02000042 RID: 66
	public class CardDefinition
	{
		// Token: 0x0600048C RID: 1164 RVA: 0x000134D4 File Offset: 0x000116D4
		public override bool Equals(object obj)
		{
			NetCache.CardDefinition cardDefinition = obj as NetCache.CardDefinition;
			return cardDefinition != null && this.Premium == cardDefinition.Premium && this.Name.Equals(cardDefinition.Name);
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x00013514 File Offset: 0x00011714
		public override int GetHashCode()
		{
			return (int)(this.Name.GetHashCode() + this.Premium);
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x00013533 File Offset: 0x00011733
		public override string ToString()
		{
			return string.Format("[CardDefinition: Name={0}, Premium={1}]", this.Name, this.Premium);
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x00013550 File Offset: 0x00011750
		// (set) Token: 0x06000490 RID: 1168 RVA: 0x00013558 File Offset: 0x00011758
		public string Name { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x00013561 File Offset: 0x00011761
		// (set) Token: 0x06000492 RID: 1170 RVA: 0x00013569 File Offset: 0x00011769
		public TAG_PREMIUM Premium { get; set; }
	}

	// Token: 0x020000B8 RID: 184
	public class NetCacheClientOptions
	{
		// Token: 0x0600098B RID: 2443 RVA: 0x0002844A File Offset: 0x0002664A
		public NetCacheClientOptions()
		{
			this.ClientState = new Map<ServerOption, NetCache.ClientOptionBase>();
			this.ServerState = new Map<ServerOption, NetCache.ClientOptionBase>();
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x00028468 File Offset: 0x00026668
		public void OnUpdateIntervalElasped(object userData)
		{
			ApplicationMgr.Get().CancelScheduledCallback(new ApplicationMgr.ScheduledCallback(this.OnUpdateIntervalElasped), null);
			this.DispatchClientOptionsToServer();
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x00028488 File Offset: 0x00026688
		public void DispatchClientOptionsToServer()
		{
			bool flag = false;
			SetOptions setOptions = new SetOptions();
			foreach (KeyValuePair<ServerOption, NetCache.ClientOptionBase> keyValuePair in this.ClientState)
			{
				NetCache.ClientOptionBase clientOptionBase;
				if (!this.ServerState.TryGetValue(keyValuePair.Key, out clientOptionBase))
				{
					flag = true;
					break;
				}
				if (keyValuePair.Value != null || clientOptionBase != null)
				{
					if ((keyValuePair.Value == null && clientOptionBase != null) || (keyValuePair.Value != null && clientOptionBase == null))
					{
						flag = true;
						break;
					}
					if (!clientOptionBase.Equals(keyValuePair.Value))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				foreach (KeyValuePair<ServerOption, NetCache.ClientOptionBase> keyValuePair2 in this.ClientState)
				{
					if (keyValuePair2.Value != null)
					{
						keyValuePair2.Value.PopulateIntoPacket(keyValuePair2.Key, setOptions);
					}
				}
				ConnectAPI.SetClientOptions(setOptions);
				foreach (KeyValuePair<ServerOption, NetCache.ClientOptionBase> keyValuePair3 in this.ClientState)
				{
					if (keyValuePair3.Value != null)
					{
						this.ServerState[keyValuePair3.Key] = (NetCache.ClientOptionBase)keyValuePair3.Value.Clone();
					}
					else
					{
						this.ServerState[keyValuePair3.Key] = null;
					}
				}
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x0600098E RID: 2446 RVA: 0x00028664 File Offset: 0x00026864
		// (set) Token: 0x0600098F RID: 2447 RVA: 0x0002866C File Offset: 0x0002686C
		public Map<ServerOption, NetCache.ClientOptionBase> ClientState { get; set; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000990 RID: 2448 RVA: 0x00028675 File Offset: 0x00026875
		// (set) Token: 0x06000991 RID: 2449 RVA: 0x0002867D File Offset: 0x0002687D
		public Map<ServerOption, NetCache.ClientOptionBase> ServerState { get; set; }
	}

	// Token: 0x020000C8 RID: 200
	public class NetCachePlayQueue
	{
		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000AFF RID: 2815 RVA: 0x000304DA File Offset: 0x0002E6DA
		// (set) Token: 0x06000B00 RID: 2816 RVA: 0x000304E2 File Offset: 0x0002E6E2
		public BnetGameType GameType { get; set; }
	}

	// Token: 0x020000C9 RID: 201
	public class NetCacheFeatures
	{
		// Token: 0x06000B01 RID: 2817 RVA: 0x000304EC File Offset: 0x0002E6EC
		public NetCacheFeatures()
		{
			this.Misc = new NetCache.NetCacheFeatures.CacheMisc();
			this.Games = new NetCache.NetCacheFeatures.CacheGames();
			this.Collection = new NetCache.NetCacheFeatures.CacheCollection();
			this.Store = new NetCache.NetCacheFeatures.CacheStore();
			this.Heroes = new NetCache.NetCacheFeatures.CacheHeroes();
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000B02 RID: 2818 RVA: 0x00030536 File Offset: 0x0002E736
		// (set) Token: 0x06000B03 RID: 2819 RVA: 0x0003053E File Offset: 0x0002E73E
		public NetCache.NetCacheFeatures.CacheMisc Misc { get; set; }

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000B04 RID: 2820 RVA: 0x00030547 File Offset: 0x0002E747
		// (set) Token: 0x06000B05 RID: 2821 RVA: 0x0003054F File Offset: 0x0002E74F
		public NetCache.NetCacheFeatures.CacheGames Games { get; set; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000B06 RID: 2822 RVA: 0x00030558 File Offset: 0x0002E758
		// (set) Token: 0x06000B07 RID: 2823 RVA: 0x00030560 File Offset: 0x0002E760
		public NetCache.NetCacheFeatures.CacheCollection Collection { get; set; }

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000B08 RID: 2824 RVA: 0x00030569 File Offset: 0x0002E769
		// (set) Token: 0x06000B09 RID: 2825 RVA: 0x00030571 File Offset: 0x0002E771
		public NetCache.NetCacheFeatures.CacheStore Store { get; set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000B0A RID: 2826 RVA: 0x0003057A File Offset: 0x0002E77A
		// (set) Token: 0x06000B0B RID: 2827 RVA: 0x00030582 File Offset: 0x0002E782
		public NetCache.NetCacheFeatures.CacheHeroes Heroes { get; set; }

		// Token: 0x020000CA RID: 202
		public class CacheGames
		{
			// Token: 0x17000164 RID: 356
			// (get) Token: 0x06000B0D RID: 2829 RVA: 0x00030593 File Offset: 0x0002E793
			// (set) Token: 0x06000B0E RID: 2830 RVA: 0x0003059B File Offset: 0x0002E79B
			public bool Tournament { get; set; }

			// Token: 0x17000165 RID: 357
			// (get) Token: 0x06000B0F RID: 2831 RVA: 0x000305A4 File Offset: 0x0002E7A4
			// (set) Token: 0x06000B10 RID: 2832 RVA: 0x000305AC File Offset: 0x0002E7AC
			public bool Practice { get; set; }

			// Token: 0x17000166 RID: 358
			// (get) Token: 0x06000B11 RID: 2833 RVA: 0x000305B5 File Offset: 0x0002E7B5
			// (set) Token: 0x06000B12 RID: 2834 RVA: 0x000305BD File Offset: 0x0002E7BD
			public bool Casual { get; set; }

			// Token: 0x17000167 RID: 359
			// (get) Token: 0x06000B13 RID: 2835 RVA: 0x000305C6 File Offset: 0x0002E7C6
			// (set) Token: 0x06000B14 RID: 2836 RVA: 0x000305CE File Offset: 0x0002E7CE
			public bool Forge { get; set; }

			// Token: 0x17000168 RID: 360
			// (get) Token: 0x06000B15 RID: 2837 RVA: 0x000305D7 File Offset: 0x0002E7D7
			// (set) Token: 0x06000B16 RID: 2838 RVA: 0x000305DF File Offset: 0x0002E7DF
			public bool Friendly { get; set; }

			// Token: 0x17000169 RID: 361
			// (get) Token: 0x06000B17 RID: 2839 RVA: 0x000305E8 File Offset: 0x0002E7E8
			// (set) Token: 0x06000B18 RID: 2840 RVA: 0x000305F0 File Offset: 0x0002E7F0
			public bool TavernBrawl { get; set; }

			// Token: 0x1700016A RID: 362
			// (get) Token: 0x06000B19 RID: 2841 RVA: 0x000305F9 File Offset: 0x0002E7F9
			// (set) Token: 0x06000B1A RID: 2842 RVA: 0x00030601 File Offset: 0x0002E801
			public int ShowUserUI { get; set; }
		}

		// Token: 0x020000D5 RID: 213
		public class CacheCollection
		{
			// Token: 0x17000185 RID: 389
			// (get) Token: 0x06000B62 RID: 2914 RVA: 0x000309E8 File Offset: 0x0002EBE8
			// (set) Token: 0x06000B63 RID: 2915 RVA: 0x000309F0 File Offset: 0x0002EBF0
			public bool Manager { get; set; }

			// Token: 0x17000186 RID: 390
			// (get) Token: 0x06000B64 RID: 2916 RVA: 0x000309F9 File Offset: 0x0002EBF9
			// (set) Token: 0x06000B65 RID: 2917 RVA: 0x00030A01 File Offset: 0x0002EC01
			public bool Crafting { get; set; }
		}

		// Token: 0x020000D6 RID: 214
		public class CacheStore
		{
			// Token: 0x17000187 RID: 391
			// (get) Token: 0x06000B67 RID: 2919 RVA: 0x00030A12 File Offset: 0x0002EC12
			// (set) Token: 0x06000B68 RID: 2920 RVA: 0x00030A1A File Offset: 0x0002EC1A
			public bool Store { get; set; }

			// Token: 0x17000188 RID: 392
			// (get) Token: 0x06000B69 RID: 2921 RVA: 0x00030A23 File Offset: 0x0002EC23
			// (set) Token: 0x06000B6A RID: 2922 RVA: 0x00030A2B File Offset: 0x0002EC2B
			public bool BattlePay { get; set; }

			// Token: 0x17000189 RID: 393
			// (get) Token: 0x06000B6B RID: 2923 RVA: 0x00030A34 File Offset: 0x0002EC34
			// (set) Token: 0x06000B6C RID: 2924 RVA: 0x00030A3C File Offset: 0x0002EC3C
			public bool BuyWithGold { get; set; }
		}

		// Token: 0x020000D7 RID: 215
		public class CacheHeroes
		{
			// Token: 0x1700018A RID: 394
			// (get) Token: 0x06000B6E RID: 2926 RVA: 0x00030A4D File Offset: 0x0002EC4D
			// (set) Token: 0x06000B6F RID: 2927 RVA: 0x00030A55 File Offset: 0x0002EC55
			public bool Hunter { get; set; }

			// Token: 0x1700018B RID: 395
			// (get) Token: 0x06000B70 RID: 2928 RVA: 0x00030A5E File Offset: 0x0002EC5E
			// (set) Token: 0x06000B71 RID: 2929 RVA: 0x00030A66 File Offset: 0x0002EC66
			public bool Mage { get; set; }

			// Token: 0x1700018C RID: 396
			// (get) Token: 0x06000B72 RID: 2930 RVA: 0x00030A6F File Offset: 0x0002EC6F
			// (set) Token: 0x06000B73 RID: 2931 RVA: 0x00030A77 File Offset: 0x0002EC77
			public bool Paladin { get; set; }

			// Token: 0x1700018D RID: 397
			// (get) Token: 0x06000B74 RID: 2932 RVA: 0x00030A80 File Offset: 0x0002EC80
			// (set) Token: 0x06000B75 RID: 2933 RVA: 0x00030A88 File Offset: 0x0002EC88
			public bool Priest { get; set; }

			// Token: 0x1700018E RID: 398
			// (get) Token: 0x06000B76 RID: 2934 RVA: 0x00030A91 File Offset: 0x0002EC91
			// (set) Token: 0x06000B77 RID: 2935 RVA: 0x00030A99 File Offset: 0x0002EC99
			public bool Rogue { get; set; }

			// Token: 0x1700018F RID: 399
			// (get) Token: 0x06000B78 RID: 2936 RVA: 0x00030AA2 File Offset: 0x0002ECA2
			// (set) Token: 0x06000B79 RID: 2937 RVA: 0x00030AAA File Offset: 0x0002ECAA
			public bool Shaman { get; set; }

			// Token: 0x17000190 RID: 400
			// (get) Token: 0x06000B7A RID: 2938 RVA: 0x00030AB3 File Offset: 0x0002ECB3
			// (set) Token: 0x06000B7B RID: 2939 RVA: 0x00030ABB File Offset: 0x0002ECBB
			public bool Warlock { get; set; }

			// Token: 0x17000191 RID: 401
			// (get) Token: 0x06000B7C RID: 2940 RVA: 0x00030AC4 File Offset: 0x0002ECC4
			// (set) Token: 0x06000B7D RID: 2941 RVA: 0x00030ACC File Offset: 0x0002ECCC
			public bool Warrior { get; set; }
		}

		// Token: 0x020000D8 RID: 216
		public class CacheMisc
		{
			// Token: 0x17000192 RID: 402
			// (get) Token: 0x06000B7F RID: 2943 RVA: 0x00030ADD File Offset: 0x0002ECDD
			// (set) Token: 0x06000B80 RID: 2944 RVA: 0x00030AE5 File Offset: 0x0002ECE5
			public int ClientOptionsUpdateIntervalSeconds { get; set; }
		}
	}

	// Token: 0x020000CC RID: 204
	public class BoosterCard
	{
		// Token: 0x06000B1B RID: 2843 RVA: 0x0003060A File Offset: 0x0002E80A
		public BoosterCard()
		{
			this.Def = new NetCache.CardDefinition();
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000B1C RID: 2844 RVA: 0x0003061D File Offset: 0x0002E81D
		// (set) Token: 0x06000B1D RID: 2845 RVA: 0x00030625 File Offset: 0x0002E825
		public NetCache.CardDefinition Def { get; set; }

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000B1E RID: 2846 RVA: 0x0003062E File Offset: 0x0002E82E
		// (set) Token: 0x06000B1F RID: 2847 RVA: 0x00030636 File Offset: 0x0002E836
		public long Date { get; set; }
	}

	// Token: 0x020000CD RID: 205
	public class DeckHeader
	{
		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000B21 RID: 2849 RVA: 0x00030647 File Offset: 0x0002E847
		// (set) Token: 0x06000B22 RID: 2850 RVA: 0x0003064F File Offset: 0x0002E84F
		public long ID { get; set; }

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000B23 RID: 2851 RVA: 0x00030658 File Offset: 0x0002E858
		// (set) Token: 0x06000B24 RID: 2852 RVA: 0x00030660 File Offset: 0x0002E860
		public string Name { get; set; }

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000B25 RID: 2853 RVA: 0x00030669 File Offset: 0x0002E869
		// (set) Token: 0x06000B26 RID: 2854 RVA: 0x00030671 File Offset: 0x0002E871
		public int CardBack { get; set; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000B27 RID: 2855 RVA: 0x0003067A File Offset: 0x0002E87A
		// (set) Token: 0x06000B28 RID: 2856 RVA: 0x00030682 File Offset: 0x0002E882
		public string Hero { get; set; }

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000B29 RID: 2857 RVA: 0x0003068B File Offset: 0x0002E88B
		// (set) Token: 0x06000B2A RID: 2858 RVA: 0x00030693 File Offset: 0x0002E893
		public TAG_PREMIUM HeroPremium { get; set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000B2B RID: 2859 RVA: 0x0003069C File Offset: 0x0002E89C
		// (set) Token: 0x06000B2C RID: 2860 RVA: 0x000306A4 File Offset: 0x0002E8A4
		public string HeroPower { get; set; }

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000B2D RID: 2861 RVA: 0x000306AD File Offset: 0x0002E8AD
		// (set) Token: 0x06000B2E RID: 2862 RVA: 0x000306B5 File Offset: 0x0002E8B5
		public DeckType Type { get; set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000B2F RID: 2863 RVA: 0x000306BE File Offset: 0x0002E8BE
		// (set) Token: 0x06000B30 RID: 2864 RVA: 0x000306C6 File Offset: 0x0002E8C6
		public bool CardBackOverridden { get; set; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000B31 RID: 2865 RVA: 0x000306CF File Offset: 0x0002E8CF
		// (set) Token: 0x06000B32 RID: 2866 RVA: 0x000306D7 File Offset: 0x0002E8D7
		public bool HeroOverridden { get; set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000B33 RID: 2867 RVA: 0x000306E0 File Offset: 0x0002E8E0
		// (set) Token: 0x06000B34 RID: 2868 RVA: 0x000306E8 File Offset: 0x0002E8E8
		public int SeasonId { get; set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000B35 RID: 2869 RVA: 0x000306F1 File Offset: 0x0002E8F1
		// (set) Token: 0x06000B36 RID: 2870 RVA: 0x000306F9 File Offset: 0x0002E8F9
		public bool NeedsName { get; set; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000B37 RID: 2871 RVA: 0x00030702 File Offset: 0x0002E902
		// (set) Token: 0x06000B38 RID: 2872 RVA: 0x0003070A File Offset: 0x0002E90A
		public long SortOrder { get; set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000B39 RID: 2873 RVA: 0x00030713 File Offset: 0x0002E913
		// (set) Token: 0x06000B3A RID: 2874 RVA: 0x0003071B File Offset: 0x0002E91B
		public bool IsWild { get; set; }

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000B3B RID: 2875 RVA: 0x00030724 File Offset: 0x0002E924
		// (set) Token: 0x06000B3C RID: 2876 RVA: 0x0003072C File Offset: 0x0002E92C
		public DeckSourceType SourceType { get; set; }

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000B3D RID: 2877 RVA: 0x00030735 File Offset: 0x0002E935
		// (set) Token: 0x06000B3E RID: 2878 RVA: 0x0003073D File Offset: 0x0002E93D
		public DateTime? CreateDate { get; set; }

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000B3F RID: 2879 RVA: 0x00030746 File Offset: 0x0002E946
		// (set) Token: 0x06000B40 RID: 2880 RVA: 0x0003074E File Offset: 0x0002E94E
		public DateTime? LastModified { get; set; }

		// Token: 0x06000B41 RID: 2881 RVA: 0x00030758 File Offset: 0x0002E958
		public override string ToString()
		{
			return string.Format("[DeckHeader: ID={0} Name={1} Hero={2} HeroPremium={3} HeroPower={4} DeckType={5} CardBack={6} CardBackOverridden={7} HeroOverridden={8} NeedsName={9} SortOrder={10} SourceType={11} LastModified={12} CreateDate={13}]", new object[]
			{
				this.ID,
				this.Name,
				this.Hero,
				this.HeroPremium,
				this.HeroPower,
				this.Type,
				this.CardBack,
				this.CardBackOverridden,
				this.HeroOverridden,
				this.NeedsName,
				this.SortOrder,
				this.SourceType,
				this.CreateDate,
				this.LastModified
			});
		}
	}

	// Token: 0x020000D0 RID: 208
	public class NetCacheGoldBalance
	{
		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000B43 RID: 2883 RVA: 0x00030838 File Offset: 0x0002EA38
		// (set) Token: 0x06000B44 RID: 2884 RVA: 0x00030840 File Offset: 0x0002EA40
		public long CappedBalance { get; set; }

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000B45 RID: 2885 RVA: 0x00030849 File Offset: 0x0002EA49
		// (set) Token: 0x06000B46 RID: 2886 RVA: 0x00030851 File Offset: 0x0002EA51
		public long BonusBalance { get; set; }

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000B47 RID: 2887 RVA: 0x0003085A File Offset: 0x0002EA5A
		// (set) Token: 0x06000B48 RID: 2888 RVA: 0x00030862 File Offset: 0x0002EA62
		public long Cap { get; set; }

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000B49 RID: 2889 RVA: 0x0003086B File Offset: 0x0002EA6B
		// (set) Token: 0x06000B4A RID: 2890 RVA: 0x00030873 File Offset: 0x0002EA73
		public long CapWarning { get; set; }

		// Token: 0x06000B4B RID: 2891 RVA: 0x0003087C File Offset: 0x0002EA7C
		public long GetTotal()
		{
			return this.CappedBalance + this.BonusBalance;
		}
	}

	// Token: 0x020000D1 RID: 209
	// (Invoke) Token: 0x06000B4D RID: 2893
	public delegate void DelGoldBalanceListener(NetCache.NetCacheGoldBalance balance);

	// Token: 0x020000D2 RID: 210
	public class NetCacheArcaneDustBalance
	{
		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000B51 RID: 2897 RVA: 0x00030893 File Offset: 0x0002EA93
		// (set) Token: 0x06000B52 RID: 2898 RVA: 0x0003089B File Offset: 0x0002EA9B
		public long Balance { get; set; }
	}

	// Token: 0x020000D3 RID: 211
	// (Invoke) Token: 0x06000B54 RID: 2900
	public delegate void NetCacheCallback();

	// Token: 0x020000D4 RID: 212
	public class NetCacheMedalInfo
	{
		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000B58 RID: 2904 RVA: 0x000308AC File Offset: 0x0002EAAC
		// (set) Token: 0x06000B59 RID: 2905 RVA: 0x000308B4 File Offset: 0x0002EAB4
		public MedalInfoData Standard { get; set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000B5A RID: 2906 RVA: 0x000308BD File Offset: 0x0002EABD
		// (set) Token: 0x06000B5B RID: 2907 RVA: 0x000308C5 File Offset: 0x0002EAC5
		public MedalInfoData Wild { get; set; }

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000B5C RID: 2908 RVA: 0x000308CE File Offset: 0x0002EACE
		// (set) Token: 0x06000B5D RID: 2909 RVA: 0x000308D6 File Offset: 0x0002EAD6
		public NetCache.NetCacheMedalInfo PreviousMedalInfo { get; set; }

		// Token: 0x06000B5E RID: 2910 RVA: 0x000308E0 File Offset: 0x0002EAE0
		public NetCache.NetCacheMedalInfo Clone()
		{
			return new NetCache.NetCacheMedalInfo
			{
				Standard = NetCache.NetCacheMedalInfo.CloneMedalInfoData(this.Standard),
				Wild = NetCache.NetCacheMedalInfo.CloneMedalInfoData(this.Wild)
			};
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x00030918 File Offset: 0x0002EB18
		public static MedalInfoData CloneMedalInfoData(MedalInfoData original)
		{
			return new MedalInfoData
			{
				SeasonWins = original.SeasonWins,
				Stars = original.Stars,
				Streak = original.Streak,
				StarLevel = original.StarLevel,
				LevelStart = original.LevelStart,
				LevelEnd = original.LevelEnd,
				CanLoseLevel = original.CanLoseLevel,
				HasLegendRank = original.HasLegendRank,
				LegendRank = original.LegendRank,
				HasBestStarLevel = original.HasBestStarLevel,
				BestStarLevel = original.BestStarLevel,
				HasCanLoseStars = original.HasCanLoseStars,
				CanLoseStars = original.CanLoseStars
			};
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x000309C8 File Offset: 0x0002EBC8
		public override string ToString()
		{
			return string.Format("[NetCacheMedalInfo] \n Standard={0} \n Wild={1}", this.Standard, this.Wild);
		}
	}

	// Token: 0x020000D9 RID: 217
	public class NetCacheBoosters
	{
		// Token: 0x06000B81 RID: 2945 RVA: 0x00030AEE File Offset: 0x0002ECEE
		public NetCacheBoosters()
		{
			this.BoosterStacks = new List<NetCache.BoosterStack>();
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000B82 RID: 2946 RVA: 0x00030B01 File Offset: 0x0002ED01
		// (set) Token: 0x06000B83 RID: 2947 RVA: 0x00030B09 File Offset: 0x0002ED09
		public List<NetCache.BoosterStack> BoosterStacks { get; set; }

		// Token: 0x06000B84 RID: 2948 RVA: 0x00030B14 File Offset: 0x0002ED14
		public NetCache.BoosterStack GetBoosterStack(int id)
		{
			return this.BoosterStacks.Find((NetCache.BoosterStack obj) => obj.Id == id);
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x00030B48 File Offset: 0x0002ED48
		public int GetTotalNumBoosters()
		{
			int num = 0;
			foreach (NetCache.BoosterStack boosterStack in this.BoosterStacks)
			{
				num += boosterStack.Count;
			}
			return num;
		}
	}

	// Token: 0x020000DB RID: 219
	public class BoosterStack
	{
		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000B89 RID: 2953 RVA: 0x00030BC8 File Offset: 0x0002EDC8
		// (set) Token: 0x06000B8A RID: 2954 RVA: 0x00030BD0 File Offset: 0x0002EDD0
		public int Id { get; set; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000B8B RID: 2955 RVA: 0x00030BD9 File Offset: 0x0002EDD9
		// (set) Token: 0x06000B8C RID: 2956 RVA: 0x00030BE1 File Offset: 0x0002EDE1
		public int Count { get; set; }
	}

	// Token: 0x020000DC RID: 220
	public class NetCacheProfileNotices
	{
		// Token: 0x06000B8D RID: 2957 RVA: 0x00030BEA File Offset: 0x0002EDEA
		public NetCacheProfileNotices()
		{
			this.Notices = new List<NetCache.ProfileNotice>();
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000B8E RID: 2958 RVA: 0x00030BFD File Offset: 0x0002EDFD
		// (set) Token: 0x06000B8F RID: 2959 RVA: 0x00030C05 File Offset: 0x0002EE05
		public List<NetCache.ProfileNotice> Notices { get; set; }
	}

	// Token: 0x020000DD RID: 221
	public class NetCacheCardBacks
	{
		// Token: 0x06000B90 RID: 2960 RVA: 0x00030C10 File Offset: 0x0002EE10
		public NetCacheCardBacks()
		{
			this.CardBacks = new HashSet<int>();
			this.CardBacks.Add(0);
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000B91 RID: 2961 RVA: 0x00030C3B File Offset: 0x0002EE3B
		// (set) Token: 0x06000B92 RID: 2962 RVA: 0x00030C43 File Offset: 0x0002EE43
		public int DefaultCardBack { get; set; }

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000B93 RID: 2963 RVA: 0x00030C4C File Offset: 0x0002EE4C
		// (set) Token: 0x06000B94 RID: 2964 RVA: 0x00030C54 File Offset: 0x0002EE54
		public HashSet<int> CardBacks { get; set; }
	}

	// Token: 0x020000DE RID: 222
	public class NetCacheDecks
	{
		// Token: 0x06000B95 RID: 2965 RVA: 0x00030C5D File Offset: 0x0002EE5D
		public NetCacheDecks()
		{
			this.Decks = new List<NetCache.DeckHeader>();
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000B96 RID: 2966 RVA: 0x00030C70 File Offset: 0x0002EE70
		// (set) Token: 0x06000B97 RID: 2967 RVA: 0x00030C78 File Offset: 0x0002EE78
		public List<NetCache.DeckHeader> Decks { get; set; }
	}

	// Token: 0x020000DF RID: 223
	public class CardValue
	{
		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000B99 RID: 2969 RVA: 0x00030C89 File Offset: 0x0002EE89
		// (set) Token: 0x06000B9A RID: 2970 RVA: 0x00030C91 File Offset: 0x0002EE91
		public int Buy { get; set; }

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000B9B RID: 2971 RVA: 0x00030C9A File Offset: 0x0002EE9A
		// (set) Token: 0x06000B9C RID: 2972 RVA: 0x00030CA2 File Offset: 0x0002EEA2
		public int Sell { get; set; }

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000B9D RID: 2973 RVA: 0x00030CAB File Offset: 0x0002EEAB
		// (set) Token: 0x06000B9E RID: 2974 RVA: 0x00030CB3 File Offset: 0x0002EEB3
		public bool Nerfed { get; set; }
	}

	// Token: 0x020000E0 RID: 224
	public class CardStack
	{
		// Token: 0x06000B9F RID: 2975 RVA: 0x00030CBC File Offset: 0x0002EEBC
		public CardStack()
		{
			this.Def = new NetCache.CardDefinition();
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000BA0 RID: 2976 RVA: 0x00030CCF File Offset: 0x0002EECF
		// (set) Token: 0x06000BA1 RID: 2977 RVA: 0x00030CD7 File Offset: 0x0002EED7
		public NetCache.CardDefinition Def { get; set; }

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000BA2 RID: 2978 RVA: 0x00030CE0 File Offset: 0x0002EEE0
		// (set) Token: 0x06000BA3 RID: 2979 RVA: 0x00030CE8 File Offset: 0x0002EEE8
		public long Date { get; set; }

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000BA4 RID: 2980 RVA: 0x00030CF1 File Offset: 0x0002EEF1
		// (set) Token: 0x06000BA5 RID: 2981 RVA: 0x00030CF9 File Offset: 0x0002EEF9
		public int Count { get; set; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000BA6 RID: 2982 RVA: 0x00030D02 File Offset: 0x0002EF02
		// (set) Token: 0x06000BA7 RID: 2983 RVA: 0x00030D0A File Offset: 0x0002EF0A
		public int NumSeen { get; set; }
	}

	// Token: 0x020000E1 RID: 225
	public class NetCacheCollection
	{
		// Token: 0x06000BA8 RID: 2984 RVA: 0x00030D14 File Offset: 0x0002EF14
		public NetCacheCollection()
		{
			this.Stacks = new List<NetCache.CardStack>();
			foreach (object obj in Enum.GetValues(typeof(TAG_CLASS)))
			{
				TAG_CLASS key = (TAG_CLASS)((int)obj);
				this.BasicCardsUnlockedPerClass[key] = 0;
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000BA9 RID: 2985 RVA: 0x00030DA4 File Offset: 0x0002EFA4
		// (set) Token: 0x06000BAA RID: 2986 RVA: 0x00030DAC File Offset: 0x0002EFAC
		public List<NetCache.CardStack> Stacks { get; set; }

		// Token: 0x04000661 RID: 1633
		public int TotalCardsOwned;

		// Token: 0x04000662 RID: 1634
		public Map<TAG_CLASS, int> BasicCardsUnlockedPerClass = new Map<TAG_CLASS, int>();
	}

	// Token: 0x020000E2 RID: 226
	public class NetCacheCardValues
	{
		// Token: 0x06000BAB RID: 2987 RVA: 0x00030DB5 File Offset: 0x0002EFB5
		public NetCacheCardValues()
		{
			this.CardNerfIndex = 0;
			this.Values = new Map<NetCache.CardDefinition, NetCache.CardValue>();
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000BAC RID: 2988 RVA: 0x00030DCF File Offset: 0x0002EFCF
		// (set) Token: 0x06000BAD RID: 2989 RVA: 0x00030DD7 File Offset: 0x0002EFD7
		public int CardNerfIndex { get; set; }

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000BAE RID: 2990 RVA: 0x00030DE0 File Offset: 0x0002EFE0
		// (set) Token: 0x06000BAF RID: 2991 RVA: 0x00030DE8 File Offset: 0x0002EFE8
		public Map<NetCache.CardDefinition, NetCache.CardValue> Values { get; set; }
	}

	// Token: 0x020000E3 RID: 227
	public class NetCacheFavoriteHeroes
	{
		// Token: 0x06000BB0 RID: 2992 RVA: 0x00030DF1 File Offset: 0x0002EFF1
		public NetCacheFavoriteHeroes()
		{
			this.FavoriteHeroes = new Map<TAG_CLASS, NetCache.CardDefinition>();
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000BB1 RID: 2993 RVA: 0x00030E04 File Offset: 0x0002F004
		// (set) Token: 0x06000BB2 RID: 2994 RVA: 0x00030E0C File Offset: 0x0002F00C
		public Map<TAG_CLASS, NetCache.CardDefinition> FavoriteHeroes { get; set; }
	}

	// Token: 0x020000E4 RID: 228
	public class ProfileNoticePreconDeck : NetCache.ProfileNotice
	{
		// Token: 0x06000BB3 RID: 2995 RVA: 0x00030E15 File Offset: 0x0002F015
		public ProfileNoticePreconDeck() : base(NetCache.ProfileNotice.NoticeType.PRECON_DECK)
		{
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000BB4 RID: 2996 RVA: 0x00030E1E File Offset: 0x0002F01E
		// (set) Token: 0x06000BB5 RID: 2997 RVA: 0x00030E26 File Offset: 0x0002F026
		public long DeckID { get; set; }

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000BB6 RID: 2998 RVA: 0x00030E2F File Offset: 0x0002F02F
		// (set) Token: 0x06000BB7 RID: 2999 RVA: 0x00030E37 File Offset: 0x0002F037
		public int HeroAsset { get; set; }
	}

	// Token: 0x020000E5 RID: 229
	public class NetCacheRewardProgress
	{
		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000BB9 RID: 3001 RVA: 0x00030E48 File Offset: 0x0002F048
		// (set) Token: 0x06000BBA RID: 3002 RVA: 0x00030E50 File Offset: 0x0002F050
		public int Season { get; set; }

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06000BBB RID: 3003 RVA: 0x00030E59 File Offset: 0x0002F059
		// (set) Token: 0x06000BBC RID: 3004 RVA: 0x00030E61 File Offset: 0x0002F061
		public long SeasonEndDate { get; set; }

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06000BBD RID: 3005 RVA: 0x00030E6A File Offset: 0x0002F06A
		// (set) Token: 0x06000BBE RID: 3006 RVA: 0x00030E72 File Offset: 0x0002F072
		public int WinsPerGold { get; set; }

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000BBF RID: 3007 RVA: 0x00030E7B File Offset: 0x0002F07B
		// (set) Token: 0x06000BC0 RID: 3008 RVA: 0x00030E83 File Offset: 0x0002F083
		public int GoldPerReward { get; set; }

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000BC1 RID: 3009 RVA: 0x00030E8C File Offset: 0x0002F08C
		// (set) Token: 0x06000BC2 RID: 3010 RVA: 0x00030E94 File Offset: 0x0002F094
		public int MaxGoldPerDay { get; set; }

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000BC3 RID: 3011 RVA: 0x00030E9D File Offset: 0x0002F09D
		// (set) Token: 0x06000BC4 RID: 3012 RVA: 0x00030EA5 File Offset: 0x0002F0A5
		public int PackRewardId { get; set; }

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000BC5 RID: 3013 RVA: 0x00030EAE File Offset: 0x0002F0AE
		// (set) Token: 0x06000BC6 RID: 3014 RVA: 0x00030EB6 File Offset: 0x0002F0B6
		public int XPSoloLimit { get; set; }

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000BC7 RID: 3015 RVA: 0x00030EBF File Offset: 0x0002F0BF
		// (set) Token: 0x06000BC8 RID: 3016 RVA: 0x00030EC7 File Offset: 0x0002F0C7
		public int MaxHeroLevel { get; set; }

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000BC9 RID: 3017 RVA: 0x00030ED0 File Offset: 0x0002F0D0
		// (set) Token: 0x06000BCA RID: 3018 RVA: 0x00030ED8 File Offset: 0x0002F0D8
		public long NextQuestCancelDate { get; set; }

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000BCB RID: 3019 RVA: 0x00030EE1 File Offset: 0x0002F0E1
		// (set) Token: 0x06000BCC RID: 3020 RVA: 0x00030EE9 File Offset: 0x0002F0E9
		public float SpecialEventTimingMod { get; set; }
	}

	// Token: 0x020000E6 RID: 230
	public class HeroLevel
	{
		// Token: 0x06000BCD RID: 3021 RVA: 0x00030EF4 File Offset: 0x0002F0F4
		public HeroLevel()
		{
			this.Class = TAG_CLASS.INVALID;
			this.PrevLevel = null;
			this.CurrentLevel = new NetCache.HeroLevel.LevelInfo();
			this.NextReward = null;
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000BCE RID: 3022 RVA: 0x00030F27 File Offset: 0x0002F127
		// (set) Token: 0x06000BCF RID: 3023 RVA: 0x00030F2F File Offset: 0x0002F12F
		public TAG_CLASS Class { get; set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000BD0 RID: 3024 RVA: 0x00030F38 File Offset: 0x0002F138
		// (set) Token: 0x06000BD1 RID: 3025 RVA: 0x00030F40 File Offset: 0x0002F140
		public NetCache.HeroLevel.LevelInfo PrevLevel { get; set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000BD2 RID: 3026 RVA: 0x00030F49 File Offset: 0x0002F149
		// (set) Token: 0x06000BD3 RID: 3027 RVA: 0x00030F51 File Offset: 0x0002F151
		public NetCache.HeroLevel.LevelInfo CurrentLevel { get; set; }

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000BD4 RID: 3028 RVA: 0x00030F5A File Offset: 0x0002F15A
		// (set) Token: 0x06000BD5 RID: 3029 RVA: 0x00030F62 File Offset: 0x0002F162
		public NetCache.HeroLevel.NextLevelReward NextReward { get; set; }

		// Token: 0x06000BD6 RID: 3030 RVA: 0x00030F6C File Offset: 0x0002F16C
		public override string ToString()
		{
			return string.Format("[HeroLevel: Class={0}, PrevLevel={1}, CurrentLevel={2}, NextReward={3}]", new object[]
			{
				this.Class,
				this.PrevLevel,
				this.CurrentLevel,
				this.NextReward
			});
		}

		// Token: 0x020000E7 RID: 231
		public class LevelInfo
		{
			// Token: 0x06000BD7 RID: 3031 RVA: 0x00030FB4 File Offset: 0x0002F1B4
			public LevelInfo()
			{
				this.Level = 0;
				this.MaxLevel = 60;
				this.XP = 0L;
				this.MaxXP = 0L;
			}

			// Token: 0x170001B5 RID: 437
			// (get) Token: 0x06000BD8 RID: 3032 RVA: 0x00030FE6 File Offset: 0x0002F1E6
			// (set) Token: 0x06000BD9 RID: 3033 RVA: 0x00030FEE File Offset: 0x0002F1EE
			public int Level { get; set; }

			// Token: 0x170001B6 RID: 438
			// (get) Token: 0x06000BDA RID: 3034 RVA: 0x00030FF7 File Offset: 0x0002F1F7
			// (set) Token: 0x06000BDB RID: 3035 RVA: 0x00030FFF File Offset: 0x0002F1FF
			public int MaxLevel { get; set; }

			// Token: 0x170001B7 RID: 439
			// (get) Token: 0x06000BDC RID: 3036 RVA: 0x00031008 File Offset: 0x0002F208
			// (set) Token: 0x06000BDD RID: 3037 RVA: 0x00031010 File Offset: 0x0002F210
			public long XP { get; set; }

			// Token: 0x170001B8 RID: 440
			// (get) Token: 0x06000BDE RID: 3038 RVA: 0x00031019 File Offset: 0x0002F219
			// (set) Token: 0x06000BDF RID: 3039 RVA: 0x00031021 File Offset: 0x0002F221
			public long MaxXP { get; set; }

			// Token: 0x06000BE0 RID: 3040 RVA: 0x0003102A File Offset: 0x0002F22A
			public bool IsMaxLevel()
			{
				return this.Level == this.MaxLevel;
			}

			// Token: 0x06000BE1 RID: 3041 RVA: 0x0003103C File Offset: 0x0002F23C
			public override string ToString()
			{
				return string.Format("[LevelInfo: Level={0}, XP={1}, MaxXP={2}]", this.Level, this.XP, this.MaxXP);
			}
		}

		// Token: 0x020000E8 RID: 232
		public class NextLevelReward
		{
			// Token: 0x06000BE2 RID: 3042 RVA: 0x00031074 File Offset: 0x0002F274
			public NextLevelReward()
			{
				this.Level = 0;
				this.Reward = null;
			}

			// Token: 0x170001B9 RID: 441
			// (get) Token: 0x06000BE3 RID: 3043 RVA: 0x0003108A File Offset: 0x0002F28A
			// (set) Token: 0x06000BE4 RID: 3044 RVA: 0x00031092 File Offset: 0x0002F292
			public int Level { get; set; }

			// Token: 0x170001BA RID: 442
			// (get) Token: 0x06000BE5 RID: 3045 RVA: 0x0003109B File Offset: 0x0002F29B
			// (set) Token: 0x06000BE6 RID: 3046 RVA: 0x000310A3 File Offset: 0x0002F2A3
			public RewardData Reward { get; set; }

			// Token: 0x06000BE7 RID: 3047 RVA: 0x000310AC File Offset: 0x0002F2AC
			public override string ToString()
			{
				return string.Format("[NextLevelReward: Level={0}, Reward={1}]", this.Level, this.Reward);
			}
		}
	}

	// Token: 0x020000E9 RID: 233
	public class NetCacheGamesPlayed
	{
		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000BE9 RID: 3049 RVA: 0x000310D1 File Offset: 0x0002F2D1
		// (set) Token: 0x06000BEA RID: 3050 RVA: 0x000310D9 File Offset: 0x0002F2D9
		public int GamesStarted { get; set; }

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000BEB RID: 3051 RVA: 0x000310E2 File Offset: 0x0002F2E2
		// (set) Token: 0x06000BEC RID: 3052 RVA: 0x000310EA File Offset: 0x0002F2EA
		public int GamesWon { get; set; }

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06000BED RID: 3053 RVA: 0x000310F3 File Offset: 0x0002F2F3
		// (set) Token: 0x06000BEE RID: 3054 RVA: 0x000310FB File Offset: 0x0002F2FB
		public int GamesLost { get; set; }

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000BEF RID: 3055 RVA: 0x00031104 File Offset: 0x0002F304
		// (set) Token: 0x06000BF0 RID: 3056 RVA: 0x0003110C File Offset: 0x0002F30C
		public int FreeRewardProgress { get; set; }
	}

	// Token: 0x020000EA RID: 234
	public class NetCacheHeroLevels
	{
		// Token: 0x06000BF1 RID: 3057 RVA: 0x00031115 File Offset: 0x0002F315
		public NetCacheHeroLevels()
		{
			this.Levels = new List<NetCache.HeroLevel>();
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x00031128 File Offset: 0x0002F328
		public override string ToString()
		{
			string text = "[START NetCacheHeroLevels]\n";
			foreach (NetCache.HeroLevel heroLevel in this.Levels)
			{
				text += string.Format("{0}\n", heroLevel);
			}
			text += "[END NetCacheHeroLevels]";
			return text;
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000BF3 RID: 3059 RVA: 0x000311A0 File Offset: 0x0002F3A0
		// (set) Token: 0x06000BF4 RID: 3060 RVA: 0x000311A8 File Offset: 0x0002F3A8
		public List<NetCache.HeroLevel> Levels { get; set; }
	}

	// Token: 0x020000EC RID: 236
	public abstract class ClientOptionBase : ICloneable
	{
		// Token: 0x06000BF6 RID: 3062
		public abstract void PopulateIntoPacket(ServerOption type, SetOptions packet);

		// Token: 0x06000BF7 RID: 3063 RVA: 0x000311B9 File Offset: 0x0002F3B9
		public override bool Equals(object other)
		{
			return other != null && other.GetType() == base.GetType();
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x000311D7 File Offset: 0x0002F3D7
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x000311DF File Offset: 0x0002F3DF
		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}

	// Token: 0x020000EE RID: 238
	public class NetCacheDisconnectedGame
	{
		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000BFF RID: 3071 RVA: 0x000311EF File Offset: 0x0002F3EF
		// (set) Token: 0x06000C00 RID: 3072 RVA: 0x000311F7 File Offset: 0x0002F3F7
		public GameServerInfo ServerInfo { get; set; }
	}

	// Token: 0x020000EF RID: 239
	public class NetCachePlayerRecords
	{
		// Token: 0x06000C01 RID: 3073 RVA: 0x00031200 File Offset: 0x0002F400
		public NetCachePlayerRecords()
		{
			this.Records = new List<NetCache.PlayerRecord>();
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000C02 RID: 3074 RVA: 0x00031213 File Offset: 0x0002F413
		// (set) Token: 0x06000C03 RID: 3075 RVA: 0x0003121B File Offset: 0x0002F41B
		public List<NetCache.PlayerRecord> Records { get; set; }
	}

	// Token: 0x020000F0 RID: 240
	public class PlayerRecord
	{
		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000C05 RID: 3077 RVA: 0x0003122C File Offset: 0x0002F42C
		// (set) Token: 0x06000C06 RID: 3078 RVA: 0x00031234 File Offset: 0x0002F434
		public GameType RecordType { get; set; }

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000C07 RID: 3079 RVA: 0x0003123D File Offset: 0x0002F43D
		// (set) Token: 0x06000C08 RID: 3080 RVA: 0x00031245 File Offset: 0x0002F445
		public int Data { get; set; }

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000C09 RID: 3081 RVA: 0x0003124E File Offset: 0x0002F44E
		// (set) Token: 0x06000C0A RID: 3082 RVA: 0x00031256 File Offset: 0x0002F456
		public int Wins { get; set; }

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000C0B RID: 3083 RVA: 0x0003125F File Offset: 0x0002F45F
		// (set) Token: 0x06000C0C RID: 3084 RVA: 0x00031267 File Offset: 0x0002F467
		public int Losses { get; set; }

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000C0D RID: 3085 RVA: 0x00031270 File Offset: 0x0002F470
		// (set) Token: 0x06000C0E RID: 3086 RVA: 0x00031278 File Offset: 0x0002F478
		public int Ties { get; set; }
	}

	// Token: 0x020000F1 RID: 241
	public class ProfileNoticeMedal : NetCache.ProfileNotice
	{
		// Token: 0x06000C0F RID: 3087 RVA: 0x00031281 File Offset: 0x0002F481
		public ProfileNoticeMedal() : base(NetCache.ProfileNotice.NoticeType.GAINED_MEDAL)
		{
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000C10 RID: 3088 RVA: 0x0003128A File Offset: 0x0002F48A
		// (set) Token: 0x06000C11 RID: 3089 RVA: 0x00031292 File Offset: 0x0002F492
		public int StarLevel { get; set; }

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000C12 RID: 3090 RVA: 0x0003129B File Offset: 0x0002F49B
		// (set) Token: 0x06000C13 RID: 3091 RVA: 0x000312A3 File Offset: 0x0002F4A3
		public int LegendRank { get; set; }

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000C14 RID: 3092 RVA: 0x000312AC File Offset: 0x0002F4AC
		// (set) Token: 0x06000C15 RID: 3093 RVA: 0x000312B4 File Offset: 0x0002F4B4
		public int BestStarLevel { get; set; }

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000C16 RID: 3094 RVA: 0x000312BD File Offset: 0x0002F4BD
		// (set) Token: 0x06000C17 RID: 3095 RVA: 0x000312C5 File Offset: 0x0002F4C5
		public bool IsWild { get; set; }

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06000C18 RID: 3096 RVA: 0x000312CE File Offset: 0x0002F4CE
		// (set) Token: 0x06000C19 RID: 3097 RVA: 0x000312D6 File Offset: 0x0002F4D6
		public Network.RewardChest Chest { get; set; }
	}

	// Token: 0x020000F2 RID: 242
	public class ProfileNoticeRewardBooster : NetCache.ProfileNotice
	{
		// Token: 0x06000C1A RID: 3098 RVA: 0x000312DF File Offset: 0x0002F4DF
		public ProfileNoticeRewardBooster() : base(NetCache.ProfileNotice.NoticeType.REWARD_BOOSTER)
		{
			this.Id = 0;
			this.Count = 0;
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000C1B RID: 3099 RVA: 0x000312F6 File Offset: 0x0002F4F6
		// (set) Token: 0x06000C1C RID: 3100 RVA: 0x000312FE File Offset: 0x0002F4FE
		public int Id { get; set; }

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000C1D RID: 3101 RVA: 0x00031307 File Offset: 0x0002F507
		// (set) Token: 0x06000C1E RID: 3102 RVA: 0x0003130F File Offset: 0x0002F50F
		public int Count { get; set; }
	}

	// Token: 0x020000F3 RID: 243
	public class ProfileNoticeRewardCard : NetCache.ProfileNotice
	{
		// Token: 0x06000C1F RID: 3103 RVA: 0x00031318 File Offset: 0x0002F518
		public ProfileNoticeRewardCard() : base(NetCache.ProfileNotice.NoticeType.REWARD_CARD)
		{
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000C20 RID: 3104 RVA: 0x00031321 File Offset: 0x0002F521
		// (set) Token: 0x06000C21 RID: 3105 RVA: 0x00031329 File Offset: 0x0002F529
		public string CardID { get; set; }

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000C22 RID: 3106 RVA: 0x00031332 File Offset: 0x0002F532
		// (set) Token: 0x06000C23 RID: 3107 RVA: 0x0003133A File Offset: 0x0002F53A
		public TAG_PREMIUM Premium { get; set; }

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000C24 RID: 3108 RVA: 0x00031343 File Offset: 0x0002F543
		// (set) Token: 0x06000C25 RID: 3109 RVA: 0x0003134B File Offset: 0x0002F54B
		public int Quantity { get; set; }
	}

	// Token: 0x020000F4 RID: 244
	public class ProfileNoticeRewardDust : NetCache.ProfileNotice
	{
		// Token: 0x06000C26 RID: 3110 RVA: 0x00031354 File Offset: 0x0002F554
		public ProfileNoticeRewardDust() : base(NetCache.ProfileNotice.NoticeType.REWARD_DUST)
		{
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000C27 RID: 3111 RVA: 0x0003135D File Offset: 0x0002F55D
		// (set) Token: 0x06000C28 RID: 3112 RVA: 0x00031365 File Offset: 0x0002F565
		public int Amount { get; set; }
	}

	// Token: 0x020000F5 RID: 245
	public class ProfileNoticeRewardMount : NetCache.ProfileNotice
	{
		// Token: 0x06000C29 RID: 3113 RVA: 0x0003136E File Offset: 0x0002F56E
		public ProfileNoticeRewardMount() : base(NetCache.ProfileNotice.NoticeType.REWARD_MOUNT)
		{
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000C2A RID: 3114 RVA: 0x00031377 File Offset: 0x0002F577
		// (set) Token: 0x06000C2B RID: 3115 RVA: 0x0003137F File Offset: 0x0002F57F
		public int MountID { get; set; }
	}

	// Token: 0x020000F6 RID: 246
	public class ProfileNoticeRewardForge : NetCache.ProfileNotice
	{
		// Token: 0x06000C2C RID: 3116 RVA: 0x00031388 File Offset: 0x0002F588
		public ProfileNoticeRewardForge() : base(NetCache.ProfileNotice.NoticeType.REWARD_FORGE)
		{
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000C2D RID: 3117 RVA: 0x00031391 File Offset: 0x0002F591
		// (set) Token: 0x06000C2E RID: 3118 RVA: 0x00031399 File Offset: 0x0002F599
		public int Quantity { get; set; }
	}

	// Token: 0x020000F7 RID: 247
	public class ProfileNoticeRewardGold : NetCache.ProfileNotice
	{
		// Token: 0x06000C2F RID: 3119 RVA: 0x000313A2 File Offset: 0x0002F5A2
		public ProfileNoticeRewardGold() : base(NetCache.ProfileNotice.NoticeType.REWARD_GOLD)
		{
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000C30 RID: 3120 RVA: 0x000313AC File Offset: 0x0002F5AC
		// (set) Token: 0x06000C31 RID: 3121 RVA: 0x000313B4 File Offset: 0x0002F5B4
		public int Amount { get; set; }
	}

	// Token: 0x020000F8 RID: 248
	public class ProfileNoticePurchase : NetCache.ProfileNotice
	{
		// Token: 0x06000C32 RID: 3122 RVA: 0x000313BD File Offset: 0x0002F5BD
		public ProfileNoticePurchase() : base(NetCache.ProfileNotice.NoticeType.PURCHASE)
		{
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000C33 RID: 3123 RVA: 0x000313C7 File Offset: 0x0002F5C7
		// (set) Token: 0x06000C34 RID: 3124 RVA: 0x000313CF File Offset: 0x0002F5CF
		public string ProductID { get; set; }

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000C35 RID: 3125 RVA: 0x000313D8 File Offset: 0x0002F5D8
		// (set) Token: 0x06000C36 RID: 3126 RVA: 0x000313E0 File Offset: 0x0002F5E0
		public Currency CurrencyType { get; set; }

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000C37 RID: 3127 RVA: 0x000313E9 File Offset: 0x0002F5E9
		// (set) Token: 0x06000C38 RID: 3128 RVA: 0x000313F1 File Offset: 0x0002F5F1
		public long Data { get; set; }

		// Token: 0x06000C39 RID: 3129 RVA: 0x000313FC File Offset: 0x0002F5FC
		public override string ToString()
		{
			return string.Format("[ProfileNoticePurchase: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} ProductID='{5}', Data={6} Currency={7}]", new object[]
			{
				base.NoticeID,
				base.Type,
				base.Origin,
				base.OriginData,
				base.Date,
				this.ProductID,
				this.Data,
				this.CurrencyType
			});
		}
	}

	// Token: 0x020000F9 RID: 249
	public class ProfileNoticeRewardCardBack : NetCache.ProfileNotice
	{
		// Token: 0x06000C3A RID: 3130 RVA: 0x00031484 File Offset: 0x0002F684
		public ProfileNoticeRewardCardBack() : base(NetCache.ProfileNotice.NoticeType.REWARD_CARD_BACK)
		{
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000C3B RID: 3131 RVA: 0x0003148E File Offset: 0x0002F68E
		// (set) Token: 0x06000C3C RID: 3132 RVA: 0x00031496 File Offset: 0x0002F696
		public int CardBackID { get; set; }

		// Token: 0x06000C3D RID: 3133 RVA: 0x000314A0 File Offset: 0x0002F6A0
		public override string ToString()
		{
			return string.Format("[ProfileNoticePurchase: NoticeID={0}, Type={1}, Origin={2}, OriginData={3}, Date={4} CardBackID={5}]", new object[]
			{
				base.NoticeID,
				base.Type,
				base.Origin,
				base.OriginData,
				base.Date,
				this.CardBackID
			});
		}
	}

	// Token: 0x020000FA RID: 250
	public class ProfileNoticeBonusStars : NetCache.ProfileNotice
	{
		// Token: 0x06000C3E RID: 3134 RVA: 0x00031511 File Offset: 0x0002F711
		public ProfileNoticeBonusStars() : base(NetCache.ProfileNotice.NoticeType.BONUS_STARS)
		{
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000C3F RID: 3135 RVA: 0x0003151B File Offset: 0x0002F71B
		// (set) Token: 0x06000C40 RID: 3136 RVA: 0x00031523 File Offset: 0x0002F723
		public int StarLevel { get; set; }

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000C41 RID: 3137 RVA: 0x0003152C File Offset: 0x0002F72C
		// (set) Token: 0x06000C42 RID: 3138 RVA: 0x00031534 File Offset: 0x0002F734
		public int Stars { get; set; }
	}

	// Token: 0x020000FB RID: 251
	public class ProfileNoticeDisconnectedGame : NetCache.ProfileNotice
	{
		// Token: 0x06000C43 RID: 3139 RVA: 0x0003153D File Offset: 0x0002F73D
		public ProfileNoticeDisconnectedGame() : base(NetCache.ProfileNotice.NoticeType.DISCONNECTED_GAME)
		{
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000C44 RID: 3140 RVA: 0x00031546 File Offset: 0x0002F746
		// (set) Token: 0x06000C45 RID: 3141 RVA: 0x0003154E File Offset: 0x0002F74E
		public GameType GameType { get; set; }

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000C46 RID: 3142 RVA: 0x00031557 File Offset: 0x0002F757
		// (set) Token: 0x06000C47 RID: 3143 RVA: 0x0003155F File Offset: 0x0002F75F
		public int MissionId { get; set; }

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000C48 RID: 3144 RVA: 0x00031568 File Offset: 0x0002F768
		// (set) Token: 0x06000C49 RID: 3145 RVA: 0x00031570 File Offset: 0x0002F770
		public ProfileNoticeDisconnectedGameResult.GameResult GameResult { get; set; }

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000C4A RID: 3146 RVA: 0x00031579 File Offset: 0x0002F779
		// (set) Token: 0x06000C4B RID: 3147 RVA: 0x00031581 File Offset: 0x0002F781
		public ProfileNoticeDisconnectedGameResult.PlayerResult YourResult { get; set; }

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000C4C RID: 3148 RVA: 0x0003158A File Offset: 0x0002F78A
		// (set) Token: 0x06000C4D RID: 3149 RVA: 0x00031592 File Offset: 0x0002F792
		public ProfileNoticeDisconnectedGameResult.PlayerResult OpponentResult { get; set; }

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000C4E RID: 3150 RVA: 0x0003159B File Offset: 0x0002F79B
		// (set) Token: 0x06000C4F RID: 3151 RVA: 0x000315A3 File Offset: 0x0002F7A3
		public int PlayerIndex { get; set; }
	}

	// Token: 0x020000FC RID: 252
	public class ProfileNoticeLevelUp : NetCache.ProfileNotice
	{
		// Token: 0x06000C50 RID: 3152 RVA: 0x000315AC File Offset: 0x0002F7AC
		public ProfileNoticeLevelUp() : base(NetCache.ProfileNotice.NoticeType.HERO_LEVEL_UP)
		{
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000C51 RID: 3153 RVA: 0x000315B6 File Offset: 0x0002F7B6
		// (set) Token: 0x06000C52 RID: 3154 RVA: 0x000315BE File Offset: 0x0002F7BE
		public int HeroClass { get; set; }

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000C53 RID: 3155 RVA: 0x000315C7 File Offset: 0x0002F7C7
		// (set) Token: 0x06000C54 RID: 3156 RVA: 0x000315CF File Offset: 0x0002F7CF
		public int NewLevel { get; set; }
	}

	// Token: 0x020000FD RID: 253
	public class ProfileNoticeAcccountLicense : NetCache.ProfileNotice
	{
		// Token: 0x06000C55 RID: 3157 RVA: 0x000315D8 File Offset: 0x0002F7D8
		public ProfileNoticeAcccountLicense() : base(NetCache.ProfileNotice.NoticeType.ACCOUNT_LICENSE)
		{
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000C56 RID: 3158 RVA: 0x000315E2 File Offset: 0x0002F7E2
		// (set) Token: 0x06000C57 RID: 3159 RVA: 0x000315EA File Offset: 0x0002F7EA
		public long License { get; set; }

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000C58 RID: 3160 RVA: 0x000315F3 File Offset: 0x0002F7F3
		// (set) Token: 0x06000C59 RID: 3161 RVA: 0x000315FB File Offset: 0x0002F7FB
		public long CasID { get; set; }
	}

	// Token: 0x020000FE RID: 254
	public class ClientOptionBool : NetCache.ClientOptionBase
	{
		// Token: 0x06000C5A RID: 3162 RVA: 0x00031604 File Offset: 0x0002F804
		public ClientOptionBool(bool val)
		{
			this.OptionValue = val;
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000C5B RID: 3163 RVA: 0x00031613 File Offset: 0x0002F813
		// (set) Token: 0x06000C5C RID: 3164 RVA: 0x0003161B File Offset: 0x0002F81B
		public bool OptionValue { get; set; }

		// Token: 0x06000C5D RID: 3165 RVA: 0x00031624 File Offset: 0x0002F824
		public override void PopulateIntoPacket(ServerOption type, SetOptions packet)
		{
			ClientOption clientOption = new ClientOption();
			clientOption.Index = (int)type;
			clientOption.AsBool = this.OptionValue;
			packet.Options.Add(clientOption);
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x00031658 File Offset: 0x0002F858
		public override bool Equals(object other)
		{
			NetCache.ClientOptionBool clientOptionBool = (NetCache.ClientOptionBool)other;
			return clientOptionBool.OptionValue == this.OptionValue;
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x00031680 File Offset: 0x0002F880
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	// Token: 0x020000FF RID: 255
	public class ClientOptionInt : NetCache.ClientOptionBase
	{
		// Token: 0x06000C60 RID: 3168 RVA: 0x00031688 File Offset: 0x0002F888
		public ClientOptionInt(int val)
		{
			this.OptionValue = val;
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000C61 RID: 3169 RVA: 0x00031697 File Offset: 0x0002F897
		// (set) Token: 0x06000C62 RID: 3170 RVA: 0x0003169F File Offset: 0x0002F89F
		public int OptionValue { get; set; }

		// Token: 0x06000C63 RID: 3171 RVA: 0x000316A8 File Offset: 0x0002F8A8
		public override void PopulateIntoPacket(ServerOption type, SetOptions packet)
		{
			ClientOption clientOption = new ClientOption();
			clientOption.Index = (int)type;
			clientOption.AsInt32 = this.OptionValue;
			packet.Options.Add(clientOption);
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x000316DC File Offset: 0x0002F8DC
		public override bool Equals(object other)
		{
			if (base.Equals(other))
			{
				NetCache.ClientOptionInt clientOptionInt = (NetCache.ClientOptionInt)other;
				if (clientOptionInt.OptionValue == this.OptionValue)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x00031710 File Offset: 0x0002F910
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	// Token: 0x02000100 RID: 256
	public class ClientOptionLong : NetCache.ClientOptionBase
	{
		// Token: 0x06000C66 RID: 3174 RVA: 0x00031718 File Offset: 0x0002F918
		public ClientOptionLong(long val)
		{
			this.OptionValue = val;
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000C67 RID: 3175 RVA: 0x00031727 File Offset: 0x0002F927
		// (set) Token: 0x06000C68 RID: 3176 RVA: 0x0003172F File Offset: 0x0002F92F
		public long OptionValue { get; set; }

		// Token: 0x06000C69 RID: 3177 RVA: 0x00031738 File Offset: 0x0002F938
		public override void PopulateIntoPacket(ServerOption type, SetOptions packet)
		{
			ClientOption clientOption = new ClientOption();
			clientOption.Index = (int)type;
			clientOption.AsInt64 = this.OptionValue;
			packet.Options.Add(clientOption);
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x0003176C File Offset: 0x0002F96C
		public override bool Equals(object other)
		{
			if (base.Equals(other))
			{
				NetCache.ClientOptionLong clientOptionLong = (NetCache.ClientOptionLong)other;
				if (clientOptionLong.OptionValue == this.OptionValue)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x000317A0 File Offset: 0x0002F9A0
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	// Token: 0x02000101 RID: 257
	public class ClientOptionFloat : NetCache.ClientOptionBase
	{
		// Token: 0x06000C6C RID: 3180 RVA: 0x000317A8 File Offset: 0x0002F9A8
		public ClientOptionFloat(float val)
		{
			this.OptionValue = val;
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000C6D RID: 3181 RVA: 0x000317B7 File Offset: 0x0002F9B7
		// (set) Token: 0x06000C6E RID: 3182 RVA: 0x000317BF File Offset: 0x0002F9BF
		public float OptionValue { get; set; }

		// Token: 0x06000C6F RID: 3183 RVA: 0x000317C8 File Offset: 0x0002F9C8
		public override void PopulateIntoPacket(ServerOption type, SetOptions packet)
		{
			ClientOption clientOption = new ClientOption();
			clientOption.Index = (int)type;
			clientOption.AsFloat = this.OptionValue;
			packet.Options.Add(clientOption);
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x000317FC File Offset: 0x0002F9FC
		public override bool Equals(object other)
		{
			if (base.Equals(other))
			{
				NetCache.ClientOptionFloat clientOptionFloat = (NetCache.ClientOptionFloat)other;
				if (clientOptionFloat.OptionValue == this.OptionValue)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x00031830 File Offset: 0x0002FA30
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	// Token: 0x02000102 RID: 258
	public class ClientOptionULong : NetCache.ClientOptionBase
	{
		// Token: 0x06000C72 RID: 3186 RVA: 0x00031838 File Offset: 0x0002FA38
		public ClientOptionULong(ulong val)
		{
			this.OptionValue = val;
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000C73 RID: 3187 RVA: 0x00031847 File Offset: 0x0002FA47
		// (set) Token: 0x06000C74 RID: 3188 RVA: 0x0003184F File Offset: 0x0002FA4F
		public ulong OptionValue { get; set; }

		// Token: 0x06000C75 RID: 3189 RVA: 0x00031858 File Offset: 0x0002FA58
		public override void PopulateIntoPacket(ServerOption type, SetOptions packet)
		{
			ClientOption clientOption = new ClientOption();
			clientOption.Index = (int)type;
			clientOption.AsUint64 = this.OptionValue;
			packet.Options.Add(clientOption);
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x0003188C File Offset: 0x0002FA8C
		public override bool Equals(object other)
		{
			if (base.Equals(other))
			{
				NetCache.ClientOptionULong clientOptionULong = (NetCache.ClientOptionULong)other;
				if (clientOptionULong.OptionValue == this.OptionValue)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x000318C0 File Offset: 0x0002FAC0
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	// Token: 0x02000103 RID: 259
	public class NetCacheNotSoMassiveLogin
	{
		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000C79 RID: 3193 RVA: 0x000318D0 File Offset: 0x0002FAD0
		// (set) Token: 0x06000C7A RID: 3194 RVA: 0x000318D8 File Offset: 0x0002FAD8
		public NotSoMassiveLoginReply Packet { get; set; }
	}

	// Token: 0x02000104 RID: 260
	public class NetCacheSubscribeResponse
	{
		// Token: 0x040006CF RID: 1743
		public ulong FeaturesSupported;

		// Token: 0x040006D0 RID: 1744
		public ulong Route;

		// Token: 0x040006D1 RID: 1745
		public ulong KeepAliveDelay;

		// Token: 0x040006D2 RID: 1746
		public ulong RequestMaxWaitSecs;
	}

	// Token: 0x02000105 RID: 261
	public class NetCacheTavernBrawlInfo
	{
		// Token: 0x06000C7C RID: 3196 RVA: 0x000318E9 File Offset: 0x0002FAE9
		public NetCacheTavernBrawlInfo(TavernBrawlInfo info)
		{
			this.Info = info;
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000C7D RID: 3197 RVA: 0x000318F8 File Offset: 0x0002FAF8
		// (set) Token: 0x06000C7E RID: 3198 RVA: 0x00031900 File Offset: 0x0002FB00
		public TavernBrawlInfo Info { get; set; }
	}

	// Token: 0x02000106 RID: 262
	public class NetCacheTavernBrawlRecord
	{
		// Token: 0x06000C7F RID: 3199 RVA: 0x00031909 File Offset: 0x0002FB09
		public NetCacheTavernBrawlRecord(TavernBrawlPlayerRecord record)
		{
			this.Record = record;
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000C80 RID: 3200 RVA: 0x00031918 File Offset: 0x0002FB18
		// (set) Token: 0x06000C81 RID: 3201 RVA: 0x00031920 File Offset: 0x0002FB20
		public TavernBrawlPlayerRecord Record { get; set; }
	}

	// Token: 0x02000107 RID: 263
	public class NetCacheAccountLicenses
	{
		// Token: 0x06000C82 RID: 3202 RVA: 0x00031929 File Offset: 0x0002FB29
		public NetCacheAccountLicenses()
		{
			this.AccountLicenses = new Map<long, AccountLicenseInfo>();
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000C83 RID: 3203 RVA: 0x0003193C File Offset: 0x0002FB3C
		// (set) Token: 0x06000C84 RID: 3204 RVA: 0x00031944 File Offset: 0x0002FB44
		public Map<long, AccountLicenseInfo> AccountLicenses { get; set; }
	}

	// Token: 0x02000108 RID: 264
	public enum ErrorCode
	{
		// Token: 0x040006D7 RID: 1751
		NONE,
		// Token: 0x040006D8 RID: 1752
		TIMEOUT,
		// Token: 0x040006D9 RID: 1753
		SERVER
	}

	// Token: 0x02000109 RID: 265
	public class ErrorInfo
	{
		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000C86 RID: 3206 RVA: 0x00031955 File Offset: 0x0002FB55
		// (set) Token: 0x06000C87 RID: 3207 RVA: 0x0003195D File Offset: 0x0002FB5D
		public NetCache.ErrorCode Error { get; set; }

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000C88 RID: 3208 RVA: 0x00031966 File Offset: 0x0002FB66
		// (set) Token: 0x06000C89 RID: 3209 RVA: 0x0003196E File Offset: 0x0002FB6E
		public uint ServerError { get; set; }

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000C8A RID: 3210 RVA: 0x00031977 File Offset: 0x0002FB77
		// (set) Token: 0x06000C8B RID: 3211 RVA: 0x0003197F File Offset: 0x0002FB7F
		public NetCache.RequestFunc RequestingFunction { get; set; }

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000C8C RID: 3212 RVA: 0x00031988 File Offset: 0x0002FB88
		// (set) Token: 0x06000C8D RID: 3213 RVA: 0x00031990 File Offset: 0x0002FB90
		public Map<Type, NetCache.Request> RequestedTypes { get; set; }

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000C8E RID: 3214 RVA: 0x00031999 File Offset: 0x0002FB99
		// (set) Token: 0x06000C8F RID: 3215 RVA: 0x000319A1 File Offset: 0x0002FBA1
		public string RequestStackTrace { get; set; }
	}

	// Token: 0x0200010A RID: 266
	// (Invoke) Token: 0x06000C91 RID: 3217
	public delegate void RequestFunc(NetCache.NetCacheCallback callback, NetCache.ErrorCallback errorCallback);

	// Token: 0x0200010B RID: 267
	public class Request
	{
		// Token: 0x06000C94 RID: 3220 RVA: 0x000319AA File Offset: 0x0002FBAA
		public Request(Type rt, bool rl = false)
		{
			this.m_type = rt;
			this.m_reload = rl;
			this.m_result = NetCache.RequestResult.UNKNOWN;
		}

		// Token: 0x040006DF RID: 1759
		public const bool RELOAD = true;

		// Token: 0x040006E0 RID: 1760
		public Type m_type;

		// Token: 0x040006E1 RID: 1761
		public bool m_reload;

		// Token: 0x040006E2 RID: 1762
		public NetCache.RequestResult m_result;
	}

	// Token: 0x0200010C RID: 268
	// (Invoke) Token: 0x06000C96 RID: 3222
	public delegate void ErrorCallback(NetCache.ErrorInfo info);

	// Token: 0x0200010D RID: 269
	public enum RequestResult
	{
		// Token: 0x040006E4 RID: 1764
		UNKNOWN,
		// Token: 0x040006E5 RID: 1765
		PENDING,
		// Token: 0x040006E6 RID: 1766
		IN_PROCESS,
		// Token: 0x040006E7 RID: 1767
		GENERIC_COMPLETE,
		// Token: 0x040006E8 RID: 1768
		DATA_COMPLETE,
		// Token: 0x040006E9 RID: 1769
		ERROR
	}

	// Token: 0x0200010E RID: 270
	private class NetCacheBatchRequest
	{
		// Token: 0x06000C99 RID: 3225 RVA: 0x000319C8 File Offset: 0x0002FBC8
		public NetCacheBatchRequest(NetCache.NetCacheCallback reply, NetCache.ErrorCallback errorCallback, NetCache.RequestFunc requestFunc)
		{
			this.m_callback = reply;
			this.m_errorCallback = errorCallback;
			this.m_requestFunc = requestFunc;
			this.m_requestStackTrace = Environment.StackTrace;
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x00031A18 File Offset: 0x0002FC18
		public void AddRequests(List<NetCache.Request> requests)
		{
			foreach (NetCache.Request r in requests)
			{
				this.AddRequest(r);
			}
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x00031A70 File Offset: 0x0002FC70
		public void AddRequest(NetCache.Request r)
		{
			if (!this.m_requests.ContainsKey(r.m_type))
			{
				this.m_requests.Add(r.m_type, r);
			}
		}

		// Token: 0x040006EA RID: 1770
		public Map<Type, NetCache.Request> m_requests = new Map<Type, NetCache.Request>();

		// Token: 0x040006EB RID: 1771
		public NetCache.NetCacheCallback m_callback;

		// Token: 0x040006EC RID: 1772
		public NetCache.ErrorCallback m_errorCallback;

		// Token: 0x040006ED RID: 1773
		public bool m_canTimeout = true;

		// Token: 0x040006EE RID: 1774
		public DateTime m_timeAdded = DateTime.Now;

		// Token: 0x040006EF RID: 1775
		public NetCache.RequestFunc m_requestFunc;

		// Token: 0x040006F0 RID: 1776
		public string m_requestStackTrace;
	}
}
