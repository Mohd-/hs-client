using System;
using System.Collections.Generic;
using Facebook.MiniJSON;

namespace Facebook
{
	// Token: 0x02000B28 RID: 2856
	internal class IOSFacebook : AbstractFacebook, IFacebook
	{
		// Token: 0x0600618D RID: 24973 RVA: 0x001D1830 File Offset: 0x001CFA30
		private void iosInit(string appId, bool cookie, bool logging, bool status, bool frictionlessRequests, string urlSuffix)
		{
		}

		// Token: 0x0600618E RID: 24974 RVA: 0x001D1832 File Offset: 0x001CFA32
		private void iosLogin(string scope)
		{
		}

		// Token: 0x0600618F RID: 24975 RVA: 0x001D1834 File Offset: 0x001CFA34
		private void iosLogout()
		{
		}

		// Token: 0x06006190 RID: 24976 RVA: 0x001D1836 File Offset: 0x001CFA36
		private void iosSetShareDialogMode(int mode)
		{
		}

		// Token: 0x06006191 RID: 24977 RVA: 0x001D1838 File Offset: 0x001CFA38
		private void iosFeedRequest(int requestId, string toId, string link, string linkName, string linkCaption, string linkDescription, string picture, string mediaSource, string actionName, string actionLink, string reference)
		{
		}

		// Token: 0x06006192 RID: 24978 RVA: 0x001D183A File Offset: 0x001CFA3A
		private void iosAppRequest(int requestId, string message, string actionType, string objectId, string[] to = null, int toLength = 0, string filters = "", string[] excludeIds = null, int excludeIdsLength = 0, bool hasMaxRecipients = false, int maxRecipients = 0, string data = "", string title = "")
		{
		}

		// Token: 0x06006193 RID: 24979 RVA: 0x001D183C File Offset: 0x001CFA3C
		private void iosCreateGameGroup(int requestId, string name, string description, string privacy)
		{
		}

		// Token: 0x06006194 RID: 24980 RVA: 0x001D183E File Offset: 0x001CFA3E
		private void iosJoinGameGroup(int requestId, string id)
		{
		}

		// Token: 0x06006195 RID: 24981 RVA: 0x001D1840 File Offset: 0x001CFA40
		private void iosFBSettingsPublishInstall(int requestId, string appId)
		{
		}

		// Token: 0x06006196 RID: 24982 RVA: 0x001D1842 File Offset: 0x001CFA42
		private void iosFBSettingsActivateApp(string appId)
		{
		}

		// Token: 0x06006197 RID: 24983 RVA: 0x001D1844 File Offset: 0x001CFA44
		private void iosFBAppEventsLogEvent(string logEvent, double valueToSum, int numParams, string[] paramKeys, string[] paramVals)
		{
		}

		// Token: 0x06006198 RID: 24984 RVA: 0x001D1846 File Offset: 0x001CFA46
		private void iosFBAppEventsLogPurchase(double logPurchase, string currency, int numParams, string[] paramKeys, string[] paramVals)
		{
		}

		// Token: 0x06006199 RID: 24985 RVA: 0x001D1848 File Offset: 0x001CFA48
		private void iosFBAppEventsSetLimitEventUsage(bool limitEventUsage)
		{
		}

		// Token: 0x0600619A RID: 24986 RVA: 0x001D184A File Offset: 0x001CFA4A
		private void iosGetDeepLink()
		{
		}

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x0600619B RID: 24987 RVA: 0x001D184C File Offset: 0x001CFA4C
		// (set) Token: 0x0600619C RID: 24988 RVA: 0x001D1854 File Offset: 0x001CFA54
		public override int DialogMode
		{
			get
			{
				return this.dialogMode;
			}
			set
			{
				this.dialogMode = value;
				this.iosSetShareDialogMode(this.dialogMode);
			}
		}

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x0600619D RID: 24989 RVA: 0x001D1869 File Offset: 0x001CFA69
		// (set) Token: 0x0600619E RID: 24990 RVA: 0x001D1871 File Offset: 0x001CFA71
		public override bool LimitEventUsage
		{
			get
			{
				return this.limitEventUsage;
			}
			set
			{
				this.limitEventUsage = value;
				this.iosFBAppEventsSetLimitEventUsage(value);
			}
		}

		// Token: 0x0600619F RID: 24991 RVA: 0x001D1881 File Offset: 0x001CFA81
		protected override void OnAwake()
		{
			this.accessToken = "NOT_USED_ON_IOS_FACEBOOK";
		}

		// Token: 0x060061A0 RID: 24992 RVA: 0x001D1890 File Offset: 0x001CFA90
		public override void Init(InitDelegate onInitComplete, string appId, bool cookie = false, bool logging = true, bool status = true, bool xfbml = false, string channelUrl = "", string authResponse = null, bool frictionlessRequests = false, HideUnityDelegate hideUnityDelegate = null)
		{
			this.iosInit(appId, cookie, logging, status, frictionlessRequests, FBSettings.IosURLSuffix);
			this.externalInitDelegate = onInitComplete;
		}

		// Token: 0x060061A1 RID: 24993 RVA: 0x001D18B7 File Offset: 0x001CFAB7
		public override void Login(string scope = "", FacebookDelegate callback = null)
		{
			base.AddAuthDelegate(callback);
			this.iosLogin(scope);
		}

		// Token: 0x060061A2 RID: 24994 RVA: 0x001D18C7 File Offset: 0x001CFAC7
		public override void Logout()
		{
			this.iosLogout();
			this.isLoggedIn = false;
		}

		// Token: 0x060061A3 RID: 24995 RVA: 0x001D18D8 File Offset: 0x001CFAD8
		public override void AppRequest(string message, OGActionType actionType, string objectId, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null)
		{
			if (string.IsNullOrEmpty(message))
			{
				throw new ArgumentNullException("message", "message cannot be null or empty!");
			}
			if (actionType != null && string.IsNullOrEmpty(objectId))
			{
				throw new ArgumentNullException("objectId", "You cannot provide an actionType without an objectId");
			}
			if (actionType == null && !string.IsNullOrEmpty(objectId))
			{
				throw new ArgumentNullException("actionType", "You cannot provide an objectId without an actionType");
			}
			string text = null;
			if (filters != null && filters.Count > 0)
			{
				text = (filters[0] as string);
			}
			this.iosAppRequest(Convert.ToInt32(base.AddFacebookDelegate(callback)), message, (actionType == null) ? null : actionType.ToString(), objectId, to, (to == null) ? 0 : to.Length, (text == null) ? string.Empty : text, excludeIds, (excludeIds == null) ? 0 : excludeIds.Length, maxRecipients != null, (maxRecipients == null) ? 0 : maxRecipients.Value, data, title);
		}

		// Token: 0x060061A4 RID: 24996 RVA: 0x001D19E8 File Offset: 0x001CFBE8
		public override void FeedRequest(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string mediaSource = "", string actionName = "", string actionLink = "", string reference = "", Dictionary<string, string[]> properties = null, FacebookDelegate callback = null)
		{
			this.iosFeedRequest(Convert.ToInt32(base.AddFacebookDelegate(callback)), toId, link, linkName, linkCaption, linkDescription, picture, mediaSource, actionName, actionLink, reference);
		}

		// Token: 0x060061A5 RID: 24997 RVA: 0x001D1A19 File Offset: 0x001CFC19
		public override void Pay(string product, string action = "purchaseitem", int quantity = 1, int? quantityMin = null, int? quantityMax = null, string requestId = null, string pricepointId = null, string testCurrency = null, FacebookDelegate callback = null)
		{
			throw new PlatformNotSupportedException("There is no Facebook Pay Dialog on iOS");
		}

		// Token: 0x060061A6 RID: 24998 RVA: 0x001D1A28 File Offset: 0x001CFC28
		public override void GameGroupCreate(string name, string description, string privacy = "CLOSED", FacebookDelegate callback = null)
		{
			this.iosCreateGameGroup(Convert.ToInt32(base.AddFacebookDelegate(callback)), name, description, privacy);
		}

		// Token: 0x060061A7 RID: 24999 RVA: 0x001D1A4B File Offset: 0x001CFC4B
		public override void GameGroupJoin(string id, FacebookDelegate callback = null)
		{
			this.iosJoinGameGroup(Convert.ToInt32(base.AddFacebookDelegate(callback)), id);
		}

		// Token: 0x060061A8 RID: 25000 RVA: 0x001D1A60 File Offset: 0x001CFC60
		public override void GetDeepLink(FacebookDelegate callback)
		{
			if (callback == null)
			{
				return;
			}
			this.deepLinkDelegate = callback;
			this.iosGetDeepLink();
		}

		// Token: 0x060061A9 RID: 25001 RVA: 0x001D1A78 File Offset: 0x001CFC78
		public void OnGetDeepLinkComplete(string message)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			if (this.deepLinkDelegate == null)
			{
				return;
			}
			object empty = string.Empty;
			dictionary.TryGetValue("deep_link", ref empty);
			this.deepLinkDelegate.Invoke(new FBResult(empty.ToString(), null));
		}

		// Token: 0x060061AA RID: 25002 RVA: 0x001D1AC8 File Offset: 0x001CFCC8
		public override void AppEventsLogEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
		{
			IOSFacebook.NativeDict nativeDict = this.MarshallDict(parameters);
			if (valueToSum != null)
			{
				this.iosFBAppEventsLogEvent(logEvent, (double)valueToSum.Value, nativeDict.numEntries, nativeDict.keys, nativeDict.vals);
			}
			else
			{
				this.iosFBAppEventsLogEvent(logEvent, 0.0, nativeDict.numEntries, nativeDict.keys, nativeDict.vals);
			}
		}

		// Token: 0x060061AB RID: 25003 RVA: 0x001D1B34 File Offset: 0x001CFD34
		public override void AppEventsLogPurchase(float logPurchase, string currency = "USD", Dictionary<string, object> parameters = null)
		{
			IOSFacebook.NativeDict nativeDict = this.MarshallDict(parameters);
			if (string.IsNullOrEmpty(currency))
			{
				currency = "USD";
			}
			this.iosFBAppEventsLogPurchase((double)logPurchase, currency, nativeDict.numEntries, nativeDict.keys, nativeDict.vals);
		}

		// Token: 0x060061AC RID: 25004 RVA: 0x001D1B76 File Offset: 0x001CFD76
		public override void PublishInstall(string appId, FacebookDelegate callback = null)
		{
			this.iosFBSettingsPublishInstall(Convert.ToInt32(base.AddFacebookDelegate(callback)), appId);
		}

		// Token: 0x060061AD RID: 25005 RVA: 0x001D1B8B File Offset: 0x001CFD8B
		public override void ActivateApp(string appId = null)
		{
			this.iosFBSettingsActivateApp(appId);
		}

		// Token: 0x060061AE RID: 25006 RVA: 0x001D1B94 File Offset: 0x001CFD94
		private IOSFacebook.NativeDict MarshallDict(Dictionary<string, object> dict)
		{
			IOSFacebook.NativeDict nativeDict = new IOSFacebook.NativeDict();
			if (dict != null && dict.Count > 0)
			{
				nativeDict.keys = new string[dict.Count];
				nativeDict.vals = new string[dict.Count];
				nativeDict.numEntries = 0;
				foreach (KeyValuePair<string, object> keyValuePair in dict)
				{
					nativeDict.keys[nativeDict.numEntries] = keyValuePair.Key;
					nativeDict.vals[nativeDict.numEntries] = keyValuePair.Value.ToString();
					nativeDict.numEntries++;
				}
			}
			return nativeDict;
		}

		// Token: 0x060061AF RID: 25007 RVA: 0x001D1C60 File Offset: 0x001CFE60
		private IOSFacebook.NativeDict MarshallDict(Dictionary<string, string> dict)
		{
			IOSFacebook.NativeDict nativeDict = new IOSFacebook.NativeDict();
			if (dict != null && dict.Count > 0)
			{
				nativeDict.keys = new string[dict.Count];
				nativeDict.vals = new string[dict.Count];
				nativeDict.numEntries = 0;
				foreach (KeyValuePair<string, string> keyValuePair in dict)
				{
					nativeDict.keys[nativeDict.numEntries] = keyValuePair.Key;
					nativeDict.vals[nativeDict.numEntries] = keyValuePair.Value;
					nativeDict.numEntries++;
				}
			}
			return nativeDict;
		}

		// Token: 0x060061B0 RID: 25008 RVA: 0x001D1D28 File Offset: 0x001CFF28
		private void OnInitComplete(string msg)
		{
			this.isInitialized = true;
			if (!string.IsNullOrEmpty(msg))
			{
				this.OnLogin(msg);
			}
			this.externalInitDelegate.Invoke();
		}

		// Token: 0x060061B1 RID: 25009 RVA: 0x001D1D5C File Offset: 0x001CFF5C
		public void OnLogin(string msg)
		{
			if (string.IsNullOrEmpty(msg))
			{
				base.OnAuthResponse(new FBResult("{\"cancelled\":true}", null));
				return;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(msg);
			if (dictionary.ContainsKey("user_id"))
			{
				this.isLoggedIn = true;
			}
			this.ParseLoginDict(dictionary);
			base.OnAuthResponse(new FBResult(msg, null));
		}

		// Token: 0x060061B2 RID: 25010 RVA: 0x001D1DC0 File Offset: 0x001CFFC0
		public void ParseLoginDict(Dictionary<string, object> parameters)
		{
			if (parameters.ContainsKey("user_id"))
			{
				this.userId = (string)parameters["user_id"];
			}
			if (parameters.ContainsKey("access_token"))
			{
				this.accessToken = (string)parameters["access_token"];
			}
			if (parameters.ContainsKey("expiration_timestamp"))
			{
				this.accessTokenExpiresAt = this.FromTimestamp(int.Parse((string)parameters["expiration_timestamp"]));
			}
		}

		// Token: 0x060061B3 RID: 25011 RVA: 0x001D1E4C File Offset: 0x001D004C
		public void OnAccessTokenRefresh(string message)
		{
			Dictionary<string, object> parameters = (Dictionary<string, object>)Json.Deserialize(message);
			this.ParseLoginDict(parameters);
			base.OnAuthResponse(new FBResult(message, null));
		}

		// Token: 0x060061B4 RID: 25012 RVA: 0x001D1E7C File Offset: 0x001D007C
		private DateTime FromTimestamp(int timestamp)
		{
			DateTime dateTime;
			dateTime..ctor(1970, 1, 1, 0, 0, 0, 0);
			return dateTime.AddSeconds((double)timestamp);
		}

		// Token: 0x060061B5 RID: 25013 RVA: 0x001D1EA4 File Offset: 0x001D00A4
		public void OnLogout(string msg)
		{
			this.isLoggedIn = false;
		}

		// Token: 0x060061B6 RID: 25014 RVA: 0x001D1EB0 File Offset: 0x001D00B0
		public void OnRequestComplete(string msg)
		{
			int num = msg.IndexOf(":");
			if (num <= 0)
			{
				FbDebugOverride.Error("Malformed callback from ios.  I expected the form id:message but couldn't find either the ':' character or the id.");
				FbDebugOverride.Error("Here's the message that errored: " + msg);
				return;
			}
			string text = msg.Substring(0, num);
			string text2 = msg.Substring(num + 1);
			FbDebugOverride.Info("id:" + text + " msg:" + text2);
			base.OnFacebookResponse(text, new FBResult(text2, null));
		}

		// Token: 0x040048C6 RID: 18630
		private const string CancelledResponse = "{\"cancelled\":true}";

		// Token: 0x040048C7 RID: 18631
		private int dialogMode = 1;

		// Token: 0x040048C8 RID: 18632
		private InitDelegate externalInitDelegate;

		// Token: 0x040048C9 RID: 18633
		private FacebookDelegate deepLinkDelegate;

		// Token: 0x02000B29 RID: 2857
		private class NativeDict
		{
			// Token: 0x060061B7 RID: 25015 RVA: 0x001D1F22 File Offset: 0x001D0122
			public NativeDict()
			{
				this.numEntries = 0;
				this.keys = null;
				this.vals = null;
			}

			// Token: 0x040048CA RID: 18634
			public int numEntries;

			// Token: 0x040048CB RID: 18635
			public string[] keys;

			// Token: 0x040048CC RID: 18636
			public string[] vals;
		}

		// Token: 0x02000B2A RID: 2858
		public enum FBInsightsFlushBehavior
		{
			// Token: 0x040048CE RID: 18638
			FBInsightsFlushBehaviorAuto,
			// Token: 0x040048CF RID: 18639
			FBInsightsFlushBehaviorExplicitOnly
		}
	}
}
