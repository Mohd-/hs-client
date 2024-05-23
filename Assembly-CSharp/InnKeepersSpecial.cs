using System;
using System.Collections;
using System.Collections.Generic;
using bgs;
using SimpleJSON;
using UnityEngine;

// Token: 0x020004BA RID: 1210
public class InnKeepersSpecial : MonoBehaviour
{
	// Token: 0x060039C8 RID: 14792 RVA: 0x00118E7B File Offset: 0x0011707B
	public static InnKeepersSpecial Get()
	{
		InnKeepersSpecial.Init();
		return InnKeepersSpecial.s_instance;
	}

	// Token: 0x060039C9 RID: 14793 RVA: 0x00118E87 File Offset: 0x00117087
	public bool HasAlreadySeenResponse()
	{
		return this.m_hasSeenResponse;
	}

	// Token: 0x060039CA RID: 14794 RVA: 0x00118E90 File Offset: 0x00117090
	public static void Init()
	{
		if (InnKeepersSpecial.s_instance == null)
		{
			GameObject gameObject = AssetLoader.Get().LoadGameObject("InnKeepersSpecial", true, false);
			InnKeepersSpecial.s_instance = gameObject.GetComponent<InnKeepersSpecial>();
			OverlayUI.Get().AddGameObject(InnKeepersSpecial.s_instance.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		}
	}

	// Token: 0x060039CB RID: 14795 RVA: 0x00118EE1 File Offset: 0x001170E1
	public bool LoadedSuccessfully()
	{
		return this.m_loadedSuccessfully;
	}

	// Token: 0x060039CC RID: 14796 RVA: 0x00118EEC File Offset: 0x001170EC
	protected void Awake()
	{
		this.Show(false);
		this.m_headers = new Dictionary<string, string>();
		this.m_headers["Accept"] = "application/json";
		string text = (BattleNet.GetCurrentRegion() != 5) ? "us" : "cn";
		string url = "https://api.battlenet.com.cn/cms/ad/list?locale=zh_cn&community=hearthstone&mediaCategory=IN_GAME_AD&apikey=4r78qhz9atqzsxkk2qhqku6gy7p9tj8c";
		string url2 = string.Format("https://us.api.battle.net/cms/ad/list?locale={0}&community=hearthstone&mediaCategory=IN_GAME_AD&apikey=4r78qhz9atqzsxkk2qhqku6gy7p9tj8c", Localization.GetLocaleName());
		if (text.Equals("cn"))
		{
			this.m_url = url;
		}
		else
		{
			this.m_url = url2;
		}
		Log.InnKeepersSpecial.Print("Inkeeper Ad: " + this.m_url, new object[0]);
		this.m_link = null;
		this.adButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.Click));
		this.Update();
	}

	// Token: 0x060039CD RID: 14797 RVA: 0x00118FBC File Offset: 0x001171BC
	public void Show(bool visible)
	{
		if (visible)
		{
			float num = 0.5f;
			this.content.SetActive(true);
			Color color = this.adImage.gameObject.GetComponent<Renderer>().material.color;
			color.a = 0f;
			this.adImage.gameObject.GetComponent<Renderer>().material.color = color;
			Hashtable args = iTween.Hash(new object[]
			{
				"amount",
				1f,
				"time",
				num,
				"easeType",
				iTween.EaseType.linear
			});
			iTween.FadeTo(this.adImage.gameObject, args);
			this.adTitle.Show();
			Hashtable args2 = iTween.Hash(new object[]
			{
				"from",
				0f,
				"to",
				1f,
				"time",
				num,
				"easeType",
				iTween.EaseType.linear,
				"onupdate",
				delegate(object newVal)
				{
					this.adTitle.TextAlpha = (float)newVal;
				}
			});
			iTween.ValueTo(this.adTitle.gameObject, args2);
			this.adSubtitle.Show();
			Hashtable args3 = iTween.Hash(new object[]
			{
				"from",
				0f,
				"to",
				1f,
				"time",
				num,
				"easeType",
				iTween.EaseType.linear,
				"onupdate",
				delegate(object newVal)
				{
					this.adSubtitle.TextAlpha = (float)newVal;
				}
			});
			iTween.ValueTo(this.adSubtitle.gameObject, args3);
		}
		else
		{
			this.content.SetActive(false);
			this.adTitle.Hide();
			this.adSubtitle.Hide();
		}
	}

	// Token: 0x060039CE RID: 14798 RVA: 0x001191C0 File Offset: 0x001173C0
	private void ShowStore(GeneralStoreMode mode = GeneralStoreMode.NONE)
	{
		this.m_storeMode = mode;
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.HUB)
		{
			StoreManager.Get().StartGeneralTransaction(this.m_storeMode);
		}
		else
		{
			SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		}
	}

	// Token: 0x060039CF RID: 14799 RVA: 0x0011920F File Offset: 0x0011740F
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		if (mode == SceneMgr.Mode.HUB)
		{
			StoreManager.Get().StartGeneralTransaction(this.m_storeMode);
			SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		}
	}

	// Token: 0x060039D0 RID: 14800 RVA: 0x00119240 File Offset: 0x00117440
	private void Click(UIEvent e)
	{
		Log.InnKeepersSpecial.Print("IKS on release! " + this.m_link, new object[0]);
		if (this.m_gameAction != null)
		{
			WelcomeQuests.OnNavigateBack();
			this.Show(false);
			string text = this.m_gameAction.ToLowerInvariant();
			if (text.StartsWith("store"))
			{
				string[] array = text.Split(new char[]
				{
					' '
				});
				if (array.Length > 1)
				{
					BoosterDbId boosterDbId = BoosterDbId.INVALID;
					AdventureDbId adventureDbId = AdventureDbId.INVALID;
					HeroDbId heroDbId = HeroDbId.INVALID;
					string text2 = array[1];
					try
					{
						boosterDbId = (BoosterDbId)((int)Enum.Parse(typeof(BoosterDbId), text2.ToUpper()));
					}
					catch (ArgumentException)
					{
					}
					try
					{
						adventureDbId = (AdventureDbId)((int)Enum.Parse(typeof(AdventureDbId), text2.ToUpper()));
					}
					catch (ArgumentException)
					{
					}
					try
					{
						heroDbId = (HeroDbId)((int)Enum.Parse(typeof(HeroDbId), text2.ToUpper()));
					}
					catch (ArgumentException)
					{
					}
					if (boosterDbId != BoosterDbId.INVALID)
					{
						Options.Get().SetInt(Option.LAST_SELECTED_STORE_BOOSTER_ID, (int)boosterDbId);
						this.ShowStore(GeneralStoreMode.CARDS);
					}
					else if (adventureDbId != AdventureDbId.INVALID)
					{
						Options.Get().SetInt(Option.LAST_SELECTED_STORE_ADVENTURE_ID, (int)adventureDbId);
						this.ShowStore(GeneralStoreMode.ADVENTURE);
					}
					else if (heroDbId != HeroDbId.INVALID)
					{
						Options.Get().SetInt(Option.LAST_SELECTED_STORE_HERO_ID, (int)heroDbId);
						this.ShowStore(GeneralStoreMode.HEROES);
					}
					else
					{
						this.ShowStore(GeneralStoreMode.NONE);
					}
				}
				else
				{
					this.ShowStore(GeneralStoreMode.NONE);
				}
			}
		}
		else if (this.m_link != null)
		{
			Application.OpenURL(this.m_link);
		}
	}

	// Token: 0x060039D1 RID: 14801 RVA: 0x001193F4 File Offset: 0x001175F4
	private static int GetCacheAge(WWW www)
	{
		string text;
		if (www.responseHeaders != null && www.responseHeaders.TryGetValue("CACHE-CONTROL", ref text))
		{
			string[] array = text.Split(new char[]
			{
				','
			});
			foreach (string text2 in array)
			{
				string text3 = text2.ToLowerInvariant().Trim();
				if (text3.StartsWith("max-age"))
				{
					string[] array3 = text3.Split(new char[]
					{
						'='
					});
					if (array3.Length == 2)
					{
						int result;
						if (int.TryParse(array3[1], ref result))
						{
							return result;
						}
					}
				}
			}
		}
		return -1;
	}

	// Token: 0x060039D2 RID: 14802 RVA: 0x001194B0 File Offset: 0x001176B0
	private IEnumerator UpdateAdJson(string url)
	{
		Log.InnKeepersSpecial.Print("requesting url " + url, new object[0]);
		int cacheAge = Options.Get().GetInt(Option.IKS_CACHE_AGE);
		ulong epochNow = GeneralUtils.DateTimeToUnixTimeStamp(DateTime.Now);
		ulong lastDownloadTime = Options.Get().GetULong(Option.IKS_LAST_DOWNLOAD_TIME, epochNow);
		ulong secondsSinceLastDownload = epochNow - lastDownloadTime;
		bool forceShowIks = Options.Get().GetBool(Option.FORCE_SHOW_IKS);
		string response = string.Empty;
		string cachedResponse = TextUtils.FromHexString(Options.Get().GetString(Option.IKS_LAST_DOWNLOAD_RESPONSE));
		Log.InnKeepersSpecial.Print(string.Concat(new object[]
		{
			"last download ",
			secondsSinceLastDownload,
			"s ago; (will refresh after: ",
			cacheAge,
			"s)"
		}), new object[0]);
		if (secondsSinceLastDownload <= (ulong)((long)cacheAge) && !string.IsNullOrEmpty(cachedResponse) && !forceShowIks)
		{
			Log.InnKeepersSpecial.Print("Using cached response: " + cachedResponse, new object[0]);
			response = cachedResponse;
		}
		else
		{
			WWW urlWWW = new WWW(url, null, this.m_headers);
			yield return urlWWW;
			if (!string.IsNullOrEmpty(urlWWW.error))
			{
				Debug.LogWarning("Failed to download url for Inkeeper's Special: " + url);
				Debug.LogWarning(urlWWW.error);
				yield break;
			}
			response = urlWWW.text;
			cacheAge = InnKeepersSpecial.GetCacheAge(urlWWW);
			string hexString = TextUtils.ToHexString(response);
			Options.Get().SetString(Option.IKS_LAST_DOWNLOAD_RESPONSE, hexString);
			Options.Get().SetULong(Option.IKS_LAST_DOWNLOAD_TIME, epochNow);
			Options.Get().SetInt(Option.IKS_CACHE_AGE, cacheAge);
		}
		Log.InnKeepersSpecial.Print("url text is " + response, new object[0]);
		bool fail = false;
		try
		{
			this.m_response = JSON.Parse(response);
		}
		catch (Exception ex)
		{
			Exception e = ex;
			fail = true;
			Debug.LogError(e.StackTrace);
		}
		if (fail)
		{
			yield break;
		}
		this.m_adToDisplay = this.GetAdToDisplay(this.m_response);
		if (this.m_adToDisplay != null)
		{
			string hexString2 = TextUtils.ToHexString(this.m_adToDisplay.ToString());
			this.m_hasSeenResponse = (hexString2 == Options.Get().GetString(Option.IKS_LAST_SHOWN_AD));
			if (forceShowIks)
			{
				this.m_hasSeenResponse = false;
			}
			Options.Get().SetString(Option.IKS_LAST_SHOWN_AD, hexString2);
			Log.InnKeepersSpecial.Print("Ad to display :" + this.m_adToDisplay["link"], new object[0]);
			base.StartCoroutine(this.UpdateAdTexture());
		}
		yield break;
	}

	// Token: 0x060039D3 RID: 14803 RVA: 0x001194DC File Offset: 0x001176DC
	private void Update()
	{
		string url;
		if (!string.IsNullOrEmpty(this.adUrlOverride))
		{
			url = this.adUrlOverride;
		}
		else
		{
			url = this.m_url;
		}
		if (url != this.m_lastUrl && !string.IsNullOrEmpty(url))
		{
			this.m_lastUrl = url;
			this.m_link = null;
			this.Show(false);
			base.StartCoroutine(this.UpdateAdJson(url));
		}
	}

	// Token: 0x060039D4 RID: 14804 RVA: 0x0011954C File Offset: 0x0011774C
	private IEnumerator UpdateAdTexture()
	{
		this.m_link = this.m_adToDisplay["link"];
		JSONNode content = this.m_adToDisplay["contents"].AsArray[0];
		string title = content["title"];
		string subtitle = content["subtitle"];
		if (content["link"] != null)
		{
			this.m_link = content["link"];
		}
		if (title == null)
		{
			title = string.Empty;
		}
		this.adTitle.Text = title.Replace("\\n", "\n");
		if (subtitle == null)
		{
			subtitle = string.Empty;
		}
		this.adSubtitle.Text = subtitle.Replace("\\n", "\n");
		JSONNode media = content["media"];
		string imageUrl = string.Empty;
		if (media != null && media["url"] != null)
		{
			imageUrl = "http:" + media["url"];
		}
		Log.InnKeepersSpecial.Print("image url is " + imageUrl, new object[0]);
		this.m_textureWWW = new WWW(imageUrl);
		yield return this.m_textureWWW;
		if (!string.IsNullOrEmpty(this.m_textureWWW.error))
		{
			Debug.LogError("Failed to download image for Inkeeper's Special: " + imageUrl);
			Debug.LogError(this.m_textureWWW.error);
			yield break;
		}
		bool visible = this.content.activeSelf;
		this.Show(true);
		this.adImage.GetComponent<Renderer>().material.mainTexture = this.m_textureWWW.texture;
		this.adImage.GetComponent<Renderer>().material.mainTexture.wrapMode = 1;
		JSONNode metadata = this.m_adToDisplay["metadata"];
		this.UpdateText(metadata);
		this.Show(visible);
		this.m_loadedSuccessfully = true;
		yield break;
	}

	// Token: 0x060039D5 RID: 14805 RVA: 0x00119568 File Offset: 0x00117768
	private void UpdateText(JSONNode rawMetadata)
	{
		if (this.m_adMetadata.ContainsKey("gameAction"))
		{
			this.m_gameAction = this.m_adMetadata["gameAction"];
		}
		if (this.m_adMetadata.ContainsKey("buttonText"))
		{
			this.adButtonText.GameStringLookup = false;
			this.adButtonText.Text = this.m_adMetadata["buttonText"];
		}
		Vector3 localPosition = this.adTitle.transform.localPosition;
		if (this.m_adMetadata.ContainsKey("titleOffsetX"))
		{
			localPosition.x += this.m_adMetadata["titleOffsetX"].AsFloat;
		}
		if (this.m_adMetadata.ContainsKey("titleOffsetY"))
		{
			localPosition.y += this.m_adMetadata["titleOffsetY"].AsFloat;
		}
		this.adTitle.transform.localPosition = localPosition;
		Vector3 localPosition2 = this.adSubtitle.transform.localPosition;
		if (this.m_adMetadata.ContainsKey("subtitleOffsetX"))
		{
			localPosition2.x += this.m_adMetadata["subtitleOffsetX"].AsFloat;
		}
		if (this.m_adMetadata.ContainsKey("subtitleOffsetY"))
		{
			localPosition2.y += this.m_adMetadata["subtitleOffsetY"].AsFloat;
		}
		this.adSubtitle.transform.localPosition = localPosition2;
		if (this.m_adMetadata.ContainsKey("titleFontSize"))
		{
			this.adTitle.FontSize = this.m_adMetadata["titleFontSize"].AsInt;
		}
		if (this.m_adMetadata.ContainsKey("subtitleFontSize"))
		{
			this.adSubtitle.FontSize = this.m_adMetadata["subtitleFontSize"].AsInt;
		}
	}

	// Token: 0x060039D6 RID: 14806 RVA: 0x00119774 File Offset: 0x00117974
	private JSONNode GetAdToDisplay(JSONNode response)
	{
		JSONNode result;
		try
		{
			JSONNode jsonnode = null;
			int num = 0;
			double num2 = 0.0;
			foreach (object obj in response["ads"].AsArray)
			{
				JSONNode jsonnode2 = (JSONNode)obj;
				int num3 = jsonnode2["importance"].AsInt;
				bool asBool = jsonnode2["maxImportance"].AsBool;
				if (asBool)
				{
					num3 = 6;
				}
				double asDouble = jsonnode2["publish"].AsDouble;
				JSONNode jsonnode3 = jsonnode2["metadata"];
				Map<string, JSONNode> map = new Map<string, JSONNode>();
				if (jsonnode3 != null)
				{
					foreach (object obj2 in jsonnode3.AsArray)
					{
						JSONNode jsonnode4 = (JSONNode)obj2;
						map[jsonnode4["key"]] = jsonnode4["value"];
					}
				}
				if (!map.ContainsKey("clientVersion") || StringUtils.CompareIgnoreCase(map["clientVersion"].Value, "5.0"))
				{
					if (map.ContainsKey("platform"))
					{
						Log.InnKeepersSpecial.Print("{0} supported on: {1}; current platform is {2}", new object[]
						{
							jsonnode2["campaignName"].Value,
							map["platform"].Value,
							PlatformSettings.OS.ToString()
						});
						string[] array = map["platform"].Value.Trim().Split(new char[]
						{
							','
						});
						bool flag = false;
						foreach (string text in array)
						{
							if (StringUtils.CompareIgnoreCase(text.Trim(), PlatformSettings.OS.ToString()))
							{
								flag = true;
							}
						}
						if (!flag)
						{
							continue;
						}
					}
					if (num3 > num || (num3 == num && asDouble > num2))
					{
						jsonnode = jsonnode2;
						this.m_adMetadata = map;
						num = num3;
						num2 = asDouble;
					}
				}
			}
			result = jsonnode;
		}
		catch (Exception ex)
		{
			Debug.LogError("IKS Error: " + ex.StackTrace);
			Debug.Log("Failed to get correct advertisement " + ex.Message);
			result = null;
		}
		return result;
	}

	// Token: 0x040024FA RID: 9466
	public string adUrlOverride;

	// Token: 0x040024FB RID: 9467
	public GameObject adImage;

	// Token: 0x040024FC RID: 9468
	public GameObject adBackground;

	// Token: 0x040024FD RID: 9469
	public PegUIElement adButton;

	// Token: 0x040024FE RID: 9470
	public UberText adButtonText;

	// Token: 0x040024FF RID: 9471
	public UberText adTitle;

	// Token: 0x04002500 RID: 9472
	public UberText adSubtitle;

	// Token: 0x04002501 RID: 9473
	public GameObject content;

	// Token: 0x04002502 RID: 9474
	private string m_lastUrl;

	// Token: 0x04002503 RID: 9475
	private JSONNode m_response;

	// Token: 0x04002504 RID: 9476
	private JSONNode m_adToDisplay;

	// Token: 0x04002505 RID: 9477
	private Map<string, JSONNode> m_adMetadata;

	// Token: 0x04002506 RID: 9478
	private string m_url;

	// Token: 0x04002507 RID: 9479
	private WWW m_textureWWW;

	// Token: 0x04002508 RID: 9480
	private string m_link;

	// Token: 0x04002509 RID: 9481
	private string m_gameAction;

	// Token: 0x0400250A RID: 9482
	private GeneralStoreMode m_storeMode;

	// Token: 0x0400250B RID: 9483
	private Dictionary<string, string> m_headers;

	// Token: 0x0400250C RID: 9484
	private static InnKeepersSpecial s_instance;

	// Token: 0x0400250D RID: 9485
	private bool m_loadedSuccessfully;

	// Token: 0x0400250E RID: 9486
	private bool m_hasSeenResponse;
}
