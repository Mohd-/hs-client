using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using UnityEngine;

namespace Facebook
{
	// Token: 0x02000B1E RID: 2846
	internal class EditorFacebook : AbstractFacebook, IFacebook
	{
		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x0600615D RID: 24925 RVA: 0x001D0E92 File Offset: 0x001CF092
		// (set) Token: 0x0600615E RID: 24926 RVA: 0x001D0E95 File Offset: 0x001CF095
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

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x0600615F RID: 24927 RVA: 0x001D0E97 File Offset: 0x001CF097
		// (set) Token: 0x06006160 RID: 24928 RVA: 0x001D0E9F File Offset: 0x001CF09F
		public override bool LimitEventUsage
		{
			get
			{
				return this.limitEventUsage;
			}
			set
			{
				this.limitEventUsage = value;
			}
		}

		// Token: 0x06006161 RID: 24929 RVA: 0x001D0EA8 File Offset: 0x001CF0A8
		protected override void OnAwake()
		{
			base.StartCoroutine(FB.RemoteFacebookLoader.LoadFacebookClass("CanvasFacebook", new FB.RemoteFacebookLoader.LoadedDllCallback(this.OnDllLoaded)));
		}

		// Token: 0x06006162 RID: 24930 RVA: 0x001D0EC8 File Offset: 0x001CF0C8
		public override void Init(InitDelegate onInitComplete, string appId, bool cookie = false, bool logging = true, bool status = true, bool xfbml = false, string channelUrl = "", string authResponse = null, bool frictionlessRequests = false, HideUnityDelegate hideUnityDelegate = null)
		{
			base.StartCoroutine(this.OnInit(onInitComplete, appId, cookie, logging, status, xfbml, channelUrl, authResponse, frictionlessRequests, hideUnityDelegate));
		}

		// Token: 0x06006163 RID: 24931 RVA: 0x001D0EF4 File Offset: 0x001CF0F4
		private IEnumerator OnInit(InitDelegate onInitComplete, string appId, bool cookie = false, bool logging = true, bool status = true, bool xfbml = false, string channelUrl = "", string authResponse = null, bool frictionlessRequests = false, HideUnityDelegate hideUnityDelegate = null)
		{
			while (this.fb == null)
			{
				yield return null;
			}
			this.fb.Init(onInitComplete, appId, cookie, logging, status, xfbml, channelUrl, authResponse, frictionlessRequests, hideUnityDelegate);
			this.isInitialized = true;
			if (onInitComplete != null)
			{
				onInitComplete.Invoke();
			}
			yield break;
		}

		// Token: 0x06006164 RID: 24932 RVA: 0x001D0FA9 File Offset: 0x001CF1A9
		private void OnDllLoaded(IFacebook fb)
		{
			this.fb = (AbstractFacebook)fb;
		}

		// Token: 0x06006165 RID: 24933 RVA: 0x001D0FB7 File Offset: 0x001CF1B7
		public override void Login(string scope = "", FacebookDelegate callback = null)
		{
			base.AddAuthDelegate(callback);
			FBComponentFactory.GetComponent<EditorFacebookAccessToken>(0);
		}

		// Token: 0x06006166 RID: 24934 RVA: 0x001D0FC7 File Offset: 0x001CF1C7
		public override void Logout()
		{
			this.isLoggedIn = false;
			this.userId = string.Empty;
			this.accessToken = string.Empty;
			this.fb.UserId = string.Empty;
			this.fb.AccessToken = string.Empty;
		}

		// Token: 0x06006167 RID: 24935 RVA: 0x001D1008 File Offset: 0x001CF208
		public override void AppRequest(string message, OGActionType actionType, string objectId, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null)
		{
			this.fb.AppRequest(message, actionType, objectId, to, filters, excludeIds, maxRecipients, data, title, callback);
		}

		// Token: 0x06006168 RID: 24936 RVA: 0x001D1034 File Offset: 0x001CF234
		public override void FeedRequest(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string mediaSource = "", string actionName = "", string actionLink = "", string reference = "", Dictionary<string, string[]> properties = null, FacebookDelegate callback = null)
		{
			this.fb.FeedRequest(toId, link, linkName, linkCaption, linkDescription, picture, mediaSource, actionName, actionLink, reference, properties, callback);
		}

		// Token: 0x06006169 RID: 24937 RVA: 0x001D1061 File Offset: 0x001CF261
		public override void Pay(string product, string action = "purchaseitem", int quantity = 1, int? quantityMin = null, int? quantityMax = null, string requestId = null, string pricepointId = null, string testCurrency = null, FacebookDelegate callback = null)
		{
			FbDebugOverride.Info("Pay method only works with Facebook Canvas.  Does nothing in the Unity Editor, iOS or Android");
		}

		// Token: 0x0600616A RID: 24938 RVA: 0x001D106D File Offset: 0x001CF26D
		public override void GameGroupCreate(string name, string description, string privacy = "CLOSED", FacebookDelegate callback = null)
		{
			throw new PlatformNotSupportedException("There is no Facebook GameGroupCreate Dialog on Editor");
		}

		// Token: 0x0600616B RID: 24939 RVA: 0x001D1079 File Offset: 0x001CF279
		public override void GameGroupJoin(string id, FacebookDelegate callback = null)
		{
			throw new PlatformNotSupportedException("There is no Facebook GameGroupJoin Dialog on Editor");
		}

		// Token: 0x0600616C RID: 24940 RVA: 0x001D1085 File Offset: 0x001CF285
		public override void GetAuthResponse(FacebookDelegate callback = null)
		{
			this.fb.GetAuthResponse(callback);
		}

		// Token: 0x0600616D RID: 24941 RVA: 0x001D1093 File Offset: 0x001CF293
		public override void PublishInstall(string appId, FacebookDelegate callback = null)
		{
		}

		// Token: 0x0600616E RID: 24942 RVA: 0x001D1095 File Offset: 0x001CF295
		public override void ActivateApp(string appId = null)
		{
			FbDebugOverride.Info("This only needs to be called for iOS or Android.");
		}

		// Token: 0x0600616F RID: 24943 RVA: 0x001D10A1 File Offset: 0x001CF2A1
		public override void GetDeepLink(FacebookDelegate callback)
		{
			FbDebugOverride.Info("No Deep Linking in the Editor");
			if (callback != null)
			{
				callback.Invoke(new FBResult("<platform dependent>", null));
			}
		}

		// Token: 0x06006170 RID: 24944 RVA: 0x001D10C4 File Offset: 0x001CF2C4
		public override void AppEventsLogEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
		{
			FbDebugOverride.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
		}

		// Token: 0x06006171 RID: 24945 RVA: 0x001D10D0 File Offset: 0x001CF2D0
		public override void AppEventsLogPurchase(float logPurchase, string currency = "USD", Dictionary<string, object> parameters = null)
		{
			FbDebugOverride.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
		}

		// Token: 0x06006172 RID: 24946 RVA: 0x001D10DC File Offset: 0x001CF2DC
		public void MockLoginCallback(FBResult result)
		{
			Object.Destroy(FBComponentFactory.GetComponent<EditorFacebookAccessToken>(0));
			if (result.Error != null)
			{
				this.BadAccessToken(result.Error);
				return;
			}
			try
			{
				List<object> list = (List<object>)Json.Deserialize(result.Text);
				List<string> list2 = new List<string>();
				foreach (object obj in list)
				{
					if (obj is Dictionary<string, object>)
					{
						Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
						if (dictionary.ContainsKey("body"))
						{
							list2.Add((string)dictionary["body"]);
						}
					}
				}
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)Json.Deserialize(list2[0]);
				Dictionary<string, object> dictionary3 = (Dictionary<string, object>)Json.Deserialize(list2[1]);
				if (FB.AppId != (string)dictionary3["id"])
				{
					this.BadAccessToken("Access token is not for current app id: " + FB.AppId);
				}
				else
				{
					this.userId = (string)dictionary2["id"];
					this.fb.UserId = this.userId;
					this.fb.AccessToken = this.accessToken;
					this.isLoggedIn = true;
					base.OnAuthResponse(new FBResult(string.Empty, null));
				}
			}
			catch (Exception ex)
			{
				this.BadAccessToken("Could not get data from access token: " + ex.Message);
			}
		}

		// Token: 0x06006173 RID: 24947 RVA: 0x001D12A4 File Offset: 0x001CF4A4
		public void MockCancelledLoginCallback()
		{
			base.OnAuthResponse(new FBResult(string.Empty, null));
		}

		// Token: 0x06006174 RID: 24948 RVA: 0x001D12B8 File Offset: 0x001CF4B8
		private void BadAccessToken(string error)
		{
			FbDebugOverride.Error(error);
			this.userId = string.Empty;
			this.fb.UserId = string.Empty;
			this.accessToken = string.Empty;
			this.fb.AccessToken = string.Empty;
			FBComponentFactory.GetComponent<EditorFacebookAccessToken>(0);
		}

		// Token: 0x040048A1 RID: 18593
		private AbstractFacebook fb;

		// Token: 0x040048A2 RID: 18594
		private FacebookDelegate loginCallback;
	}
}
