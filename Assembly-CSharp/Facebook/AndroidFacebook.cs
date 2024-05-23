using System;
using System.Collections.Generic;
using Facebook.MiniJSON;

namespace Facebook
{
	// Token: 0x02000B0E RID: 2830
	internal sealed class AndroidFacebook : AbstractFacebook, IFacebook
	{
		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x060060CB RID: 24779 RVA: 0x001CF5A9 File Offset: 0x001CD7A9
		public string KeyHash
		{
			get
			{
				return this.keyHash;
			}
		}

		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x060060CC RID: 24780 RVA: 0x001CF5B1 File Offset: 0x001CD7B1
		// (set) Token: 0x060060CD RID: 24781 RVA: 0x001CF5B4 File Offset: 0x001CD7B4
		public override int DialogMode
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x060060CE RID: 24782 RVA: 0x001CF5B6 File Offset: 0x001CD7B6
		// (set) Token: 0x060060CF RID: 24783 RVA: 0x001CF5BE File Offset: 0x001CD7BE
		public override bool LimitEventUsage
		{
			get
			{
				return this.limitEventUsage;
			}
			set
			{
				this.limitEventUsage = value;
				this.CallFB("SetLimitEventUsage", value.ToString());
			}
		}

		// Token: 0x060060D0 RID: 24784 RVA: 0x001CF5D9 File Offset: 0x001CD7D9
		private void CallFB(string method, string args)
		{
			FbDebug.Error("Using Android when not on an Android build!  Doesn't Work!");
		}

		// Token: 0x060060D1 RID: 24785 RVA: 0x001CF5E5 File Offset: 0x001CD7E5
		protected override void OnAwake()
		{
			this.keyHash = string.Empty;
		}

		// Token: 0x060060D2 RID: 24786 RVA: 0x001CF5F2 File Offset: 0x001CD7F2
		private bool IsErrorResponse(string response)
		{
			return false;
		}

		// Token: 0x060060D3 RID: 24787 RVA: 0x001CF5F8 File Offset: 0x001CD7F8
		public override void Init(InitDelegate onInitComplete, string appId, bool cookie = false, bool logging = true, bool status = true, bool xfbml = false, string channelUrl = "", string authResponse = null, bool frictionlessRequests = false, HideUnityDelegate hideUnityDelegate = null)
		{
			if (string.IsNullOrEmpty(appId))
			{
				throw new ArgumentException("appId cannot be null or empty!");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("appId", appId);
			if (cookie)
			{
				dictionary.Add("cookie", true);
			}
			if (!logging)
			{
				dictionary.Add("logging", false);
			}
			if (!status)
			{
				dictionary.Add("status", false);
			}
			if (xfbml)
			{
				dictionary.Add("xfbml", true);
			}
			if (!string.IsNullOrEmpty(channelUrl))
			{
				dictionary.Add("channelUrl", channelUrl);
			}
			if (!string.IsNullOrEmpty(authResponse))
			{
				dictionary.Add("authResponse", authResponse);
			}
			if (frictionlessRequests)
			{
				dictionary.Add("frictionlessRequests", true);
			}
			string text = Json.Serialize(dictionary);
			this.onInitComplete = onInitComplete;
			this.CallFB("Init", text.ToString());
		}

		// Token: 0x060060D4 RID: 24788 RVA: 0x001CF6F8 File Offset: 0x001CD8F8
		public void OnInitComplete(string message)
		{
			this.isInitialized = true;
			this.OnLoginComplete(message);
			if (this.onInitComplete != null)
			{
				this.onInitComplete.Invoke();
			}
		}

		// Token: 0x060060D5 RID: 24789 RVA: 0x001CF72C File Offset: 0x001CD92C
		public override void Login(string scope = "", FacebookDelegate callback = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("scope", scope);
			string args = Json.Serialize(dictionary);
			base.AddAuthDelegate(callback);
			this.CallFB("Login", args);
		}

		// Token: 0x060060D6 RID: 24790 RVA: 0x001CF768 File Offset: 0x001CD968
		public void OnLoginComplete(string message)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			if (dictionary.ContainsKey("user_id"))
			{
				this.isLoggedIn = true;
				this.userId = (string)dictionary["user_id"];
				this.accessToken = (string)dictionary["access_token"];
				this.accessTokenExpiresAt = this.FromTimestamp(int.Parse((string)dictionary["expiration_timestamp"]));
			}
			if (dictionary.ContainsKey("key_hash"))
			{
				this.keyHash = (string)dictionary["key_hash"];
			}
			base.OnAuthResponse(new FBResult(message, null));
		}

		// Token: 0x060060D7 RID: 24791 RVA: 0x001CF818 File Offset: 0x001CDA18
		public void OnGroupCreateComplete(string message)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			string text = (string)dictionary["callback_id"];
			dictionary.Remove("callback_id");
			base.OnFacebookResponse(text, new FBResult(Json.Serialize(dictionary), null));
		}

		// Token: 0x060060D8 RID: 24792 RVA: 0x001CF864 File Offset: 0x001CDA64
		public void OnAccessTokenRefresh(string message)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			if (dictionary.ContainsKey("access_token"))
			{
				this.accessToken = (string)dictionary["access_token"];
				this.accessTokenExpiresAt = this.FromTimestamp(int.Parse((string)dictionary["expiration_timestamp"]));
			}
		}

		// Token: 0x060060D9 RID: 24793 RVA: 0x001CF8C4 File Offset: 0x001CDAC4
		public override void Logout()
		{
			this.CallFB("Logout", string.Empty);
		}

		// Token: 0x060060DA RID: 24794 RVA: 0x001CF8D6 File Offset: 0x001CDAD6
		public void OnLogoutComplete(string message)
		{
			this.isLoggedIn = false;
			this.userId = string.Empty;
			this.accessToken = string.Empty;
		}

		// Token: 0x060060DB RID: 24795 RVA: 0x001CF8F8 File Offset: 0x001CDAF8
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
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["message"] = message;
			if (callback != null)
			{
				dictionary["callback_id"] = base.AddFacebookDelegate(callback);
			}
			if (actionType != null && !string.IsNullOrEmpty(objectId))
			{
				dictionary["action_type"] = actionType.ToString();
				dictionary["object_id"] = objectId;
			}
			if (to != null)
			{
				dictionary["to"] = string.Join(",", to);
			}
			if (filters != null && filters.Count > 0)
			{
				string text = filters[0] as string;
				if (text != null)
				{
					dictionary["filters"] = text;
				}
			}
			if (maxRecipients != null)
			{
				dictionary["max_recipients"] = maxRecipients.Value;
			}
			if (!string.IsNullOrEmpty(data))
			{
				dictionary["data"] = data;
			}
			if (!string.IsNullOrEmpty(title))
			{
				dictionary["title"] = title;
			}
			this.CallFB("AppRequest", Json.Serialize(dictionary));
		}

		// Token: 0x060060DC RID: 24796 RVA: 0x001CFA74 File Offset: 0x001CDC74
		public void OnAppRequestsComplete(string message)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			if (dictionary.ContainsKey("callback_id"))
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				string text = (string)dictionary["callback_id"];
				dictionary.Remove("callback_id");
				if (dictionary.Count > 0)
				{
					List<string> list = new List<string>(dictionary.Count - 1);
					foreach (string text2 in dictionary.Keys)
					{
						if (!text2.StartsWith("to"))
						{
							dictionary2[text2] = dictionary[text2];
						}
						else
						{
							list.Add((string)dictionary[text2]);
						}
					}
					dictionary2.Add("to", list);
					dictionary.Clear();
					base.OnFacebookResponse(text, new FBResult(Json.Serialize(dictionary2), null));
				}
				else
				{
					base.OnFacebookResponse(text, new FBResult(Json.Serialize(dictionary2), "Malformed request response.  Please file a bug with facebook here: https://developers.facebook.com/bugs/create"));
				}
			}
		}

		// Token: 0x060060DD RID: 24797 RVA: 0x001CFBA0 File Offset: 0x001CDDA0
		public override void FeedRequest(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string mediaSource = "", string actionName = "", string actionLink = "", string reference = "", Dictionary<string, string[]> properties = null, FacebookDelegate callback = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (callback != null)
			{
				dictionary["callback_id"] = base.AddFacebookDelegate(callback);
			}
			if (!string.IsNullOrEmpty(toId))
			{
				dictionary.Add("to", toId);
			}
			if (!string.IsNullOrEmpty(link))
			{
				dictionary.Add("link", link);
			}
			if (!string.IsNullOrEmpty(linkName))
			{
				dictionary.Add("name", linkName);
			}
			if (!string.IsNullOrEmpty(linkCaption))
			{
				dictionary.Add("caption", linkCaption);
			}
			if (!string.IsNullOrEmpty(linkDescription))
			{
				dictionary.Add("description", linkDescription);
			}
			if (!string.IsNullOrEmpty(picture))
			{
				dictionary.Add("picture", picture);
			}
			if (!string.IsNullOrEmpty(mediaSource))
			{
				dictionary.Add("source", mediaSource);
			}
			if (!string.IsNullOrEmpty(actionName) && !string.IsNullOrEmpty(actionLink))
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2.Add("name", actionName);
				dictionary2.Add("link", actionLink);
				dictionary.Add("actions", new Dictionary<string, object>[]
				{
					dictionary2
				});
			}
			if (!string.IsNullOrEmpty(reference))
			{
				dictionary.Add("ref", reference);
			}
			if (properties != null)
			{
				Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
				foreach (KeyValuePair<string, string[]> keyValuePair in properties)
				{
					if (keyValuePair.Value.Length >= 1)
					{
						if (keyValuePair.Value.Length == 1)
						{
							dictionary3.Add(keyValuePair.Key, keyValuePair.Value[0]);
						}
						else
						{
							Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
							dictionary4.Add("text", keyValuePair.Value[0]);
							dictionary4.Add("href", keyValuePair.Value[1]);
							dictionary3.Add(keyValuePair.Key, dictionary4);
						}
					}
				}
				dictionary.Add("properties", dictionary3);
			}
			this.CallFB("FeedRequest", Json.Serialize(dictionary));
		}

		// Token: 0x060060DE RID: 24798 RVA: 0x001CFDC8 File Offset: 0x001CDFC8
		public void OnFeedRequestComplete(string message)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			if (dictionary.ContainsKey("callback_id"))
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				string text = (string)dictionary["callback_id"];
				dictionary.Remove("callback_id");
				if (dictionary.Count > 0)
				{
					foreach (string text2 in dictionary.Keys)
					{
						dictionary2[text2] = dictionary[text2];
					}
					dictionary.Clear();
					base.OnFacebookResponse(text, new FBResult(Json.Serialize(dictionary2), null));
				}
				else
				{
					base.OnFacebookResponse(text, new FBResult(Json.Serialize(dictionary2), "Malformed request response.  Please file a bug with facebook here: https://developers.facebook.com/bugs/create"));
				}
			}
		}

		// Token: 0x060060DF RID: 24799 RVA: 0x001CFEAC File Offset: 0x001CE0AC
		public override void Pay(string product, string action = "purchaseitem", int quantity = 1, int? quantityMin = null, int? quantityMax = null, string requestId = null, string pricepointId = null, string testCurrency = null, FacebookDelegate callback = null)
		{
			throw new PlatformNotSupportedException("There is no Facebook Pay Dialog on Android");
		}

		// Token: 0x060060E0 RID: 24800 RVA: 0x001CFEB8 File Offset: 0x001CE0B8
		public override void GameGroupCreate(string name, string description, string privacy = "CLOSED", FacebookDelegate callback = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["name"] = name;
			dictionary["description"] = description;
			dictionary["privacy"] = privacy;
			if (callback != null)
			{
				dictionary["callback_id"] = base.AddFacebookDelegate(callback);
			}
			this.CallFB("GameGroupCreate", Json.Serialize(dictionary));
		}

		// Token: 0x060060E1 RID: 24801 RVA: 0x001CFF1C File Offset: 0x001CE11C
		public override void GameGroupJoin(string id, FacebookDelegate callback = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["id"] = id;
			if (callback != null)
			{
				dictionary["callback_id"] = base.AddFacebookDelegate(callback);
			}
			this.CallFB("GameGroupJoin", Json.Serialize(dictionary));
		}

		// Token: 0x060060E2 RID: 24802 RVA: 0x001CFF64 File Offset: 0x001CE164
		public override void GetDeepLink(FacebookDelegate callback)
		{
			if (callback != null)
			{
				this.deepLinkDelegate = callback;
				this.CallFB("GetDeepLink", string.Empty);
			}
		}

		// Token: 0x060060E3 RID: 24803 RVA: 0x001CFF84 File Offset: 0x001CE184
		public void OnGetDeepLinkComplete(string message)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			if (this.deepLinkDelegate != null)
			{
				object empty = string.Empty;
				dictionary.TryGetValue("deep_link", ref empty);
				this.deepLinkDelegate.Invoke(new FBResult(empty.ToString(), null));
			}
		}

		// Token: 0x060060E4 RID: 24804 RVA: 0x001CFFD4 File Offset: 0x001CE1D4
		public override void AppEventsLogEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["logEvent"] = logEvent;
			if (valueToSum != null)
			{
				dictionary["valueToSum"] = valueToSum.Value;
			}
			if (parameters != null)
			{
				dictionary["parameters"] = this.ToStringDict(parameters);
			}
			this.CallFB("AppEvents", Json.Serialize(dictionary));
		}

		// Token: 0x060060E5 RID: 24805 RVA: 0x001D0040 File Offset: 0x001CE240
		public override void AppEventsLogPurchase(float logPurchase, string currency = "USD", Dictionary<string, object> parameters = null)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["logPurchase"] = logPurchase;
			dictionary["currency"] = (string.IsNullOrEmpty(currency) ? "USD" : currency);
			if (parameters != null)
			{
				dictionary["parameters"] = this.ToStringDict(parameters);
			}
			this.CallFB("AppEvents", Json.Serialize(dictionary));
		}

		// Token: 0x060060E6 RID: 24806 RVA: 0x001D00B0 File Offset: 0x001CE2B0
		public override void PublishInstall(string appId, FacebookDelegate callback = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(2);
			dictionary["app_id"] = appId;
			if (callback != null)
			{
				dictionary["callback_id"] = base.AddFacebookDelegate(callback);
			}
			this.CallFB("PublishInstall", Json.Serialize(dictionary));
		}

		// Token: 0x060060E7 RID: 24807 RVA: 0x001D00FC File Offset: 0x001CE2FC
		public void OnPublishInstallComplete(string message)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			if (dictionary.ContainsKey("callback_id"))
			{
				base.OnFacebookResponse((string)dictionary["callback_id"], new FBResult(string.Empty, null));
			}
		}

		// Token: 0x060060E8 RID: 24808 RVA: 0x001D0148 File Offset: 0x001CE348
		public override void ActivateApp(string appId = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(1);
			if (!string.IsNullOrEmpty(appId))
			{
				dictionary["app_id"] = appId;
			}
			this.CallFB("ActivateApp", Json.Serialize(dictionary));
		}

		// Token: 0x060060E9 RID: 24809 RVA: 0x001D0184 File Offset: 0x001CE384
		private Dictionary<string, string> ToStringDict(Dictionary<string, object> dict)
		{
			if (dict == null)
			{
				return null;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> keyValuePair in dict)
			{
				dictionary[keyValuePair.Key] = keyValuePair.Value.ToString();
			}
			return dictionary;
		}

		// Token: 0x060060EA RID: 24810 RVA: 0x001D01FC File Offset: 0x001CE3FC
		private DateTime FromTimestamp(int timestamp)
		{
			DateTime dateTime;
			dateTime..ctor(1970, 1, 1, 0, 0, 0, 0);
			return dateTime.AddSeconds((double)timestamp);
		}

		// Token: 0x0400486E RID: 18542
		public const int BrowserDialogMode = 0;

		// Token: 0x0400486F RID: 18543
		private const string AndroidJavaFacebookClass = "com.facebook.unity.FB";

		// Token: 0x04004870 RID: 18544
		private const string CallbackIdKey = "callback_id";

		// Token: 0x04004871 RID: 18545
		private string keyHash;

		// Token: 0x04004872 RID: 18546
		private FacebookDelegate deepLinkDelegate;

		// Token: 0x04004873 RID: 18547
		private InitDelegate onInitComplete;
	}
}
