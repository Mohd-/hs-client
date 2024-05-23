using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class AssetLoader : MonoBehaviour
{
	// Token: 0x060011E3 RID: 4579 RVA: 0x0004D62C File Offset: 0x0004B82C
	private void Awake()
	{
		AssetLoader.s_instance = this;
		this.m_workingDir = Directory.GetCurrentDirectory().Replace("\\", "/");
		AssetCache.Initialize();
		int num = Enum.GetNames(typeof(AssetFamily)).Length;
		int num2 = 0;
		int num3 = 0;
		foreach (AssetFamilyBundleInfo assetFamilyBundleInfo in AssetBundleInfo.FamilyInfo.Values)
		{
			if (assetFamilyBundleInfo.NumberOfBundles > num2)
			{
				num2 = assetFamilyBundleInfo.NumberOfBundles;
			}
			if (assetFamilyBundleInfo.NumberOfLocaleBundles > num3)
			{
				num3 = assetFamilyBundleInfo.NumberOfLocaleBundles;
			}
		}
		this.m_familyBundles = new AssetBundle[num, num2 + AssetLoader.AvailableExtraAssetBundlesCount()];
		this.m_localizedFamilyBundles = new AssetBundle[num, num3 + AssetLoader.AvailableExtraAssetBundlesCount()];
		num3 = 0;
		foreach (AssetFamilyBundleInfo assetFamilyBundleInfo2 in AssetBundleInfo.FamilyInfo.Values)
		{
			if (assetFamilyBundleInfo2.NumberOfDownloadableLocaleBundles > num3)
			{
				num3 = assetFamilyBundleInfo2.NumberOfDownloadableLocaleBundles;
			}
		}
		this.m_downloadedLocalizedFamilyBundles = new AssetBundle[num, num3];
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x0004D77C File Offset: 0x0004B97C
	private void OnDestroy()
	{
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		AssetLoader.s_instance = null;
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x0004D79C File Offset: 0x0004B99C
	private void WillReset()
	{
		AssetBundle[,] localizedFamilyBundles = this.m_localizedFamilyBundles;
		int length = localizedFamilyBundles.GetLength(0);
		int length2 = localizedFamilyBundles.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				AssetBundle assetBundle = localizedFamilyBundles[i, j];
				if (assetBundle != null)
				{
					assetBundle.Unload(true);
				}
			}
		}
		this.m_localizedFamilyBundles = new AssetBundle[this.m_localizedFamilyBundles.GetLength(0), this.m_localizedFamilyBundles.GetLength(1)];
		AssetBundle[,] downloadedLocalizedFamilyBundles = this.m_downloadedLocalizedFamilyBundles;
		int length3 = downloadedLocalizedFamilyBundles.GetLength(0);
		int length4 = downloadedLocalizedFamilyBundles.GetLength(1);
		for (int k = 0; k < length3; k++)
		{
			for (int l = 0; l < length4; l++)
			{
				AssetBundle assetBundle2 = downloadedLocalizedFamilyBundles[k, l];
				if (assetBundle2 != null)
				{
					assetBundle2.Unload(true);
				}
			}
		}
		this.m_downloadedLocalizedFamilyBundles = new AssetBundle[this.m_downloadedLocalizedFamilyBundles.GetLength(0), this.m_downloadedLocalizedFamilyBundles.GetLength(1)];
		this.PreloadBundles();
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x0004D8BF File Offset: 0x0004BABF
	private void Start()
	{
		base.StartCoroutine(this.Init());
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x0004D8D0 File Offset: 0x0004BAD0
	private IEnumerator Init()
	{
		this.PreloadBundles();
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
		this.SetReady(true);
		yield return null;
		yield break;
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x0004D8EB File Offset: 0x0004BAEB
	private void OnApplicationQuit()
	{
		AssetCache.ForceClearAllCaches();
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x0004D8F2 File Offset: 0x0004BAF2
	public static AssetLoader Get()
	{
		return AssetLoader.s_instance;
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x0004D8F9 File Offset: 0x0004BAF9
	public bool IsReady()
	{
		return this.m_ready;
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x0004D901 File Offset: 0x0004BB01
	public void SetReady(bool ready)
	{
		this.m_ready = ready;
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x0004D90A File Offset: 0x0004BB0A
	public bool IsWaitingOnObject(GameObject go)
	{
		return this.m_waitingOnObjects.Contains(go);
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x0004D918 File Offset: 0x0004BB18
	public bool LoadFile(string path, AssetLoader.FileCallback callback, object callbackData)
	{
		if (string.IsNullOrEmpty(path))
		{
			Debug.LogWarning("AssetLoader.LoadFile() - path was null or empty");
			return false;
		}
		base.StartCoroutine(this.DownloadFile(path, callback, callbackData));
		return true;
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x0004D950 File Offset: 0x0004BB50
	public bool LoadActor(string cardName, AssetLoader.GameObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadPrefab(cardName, AssetFamily.Actor, false, callback, callbackData, persistent, null);
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x0004D96C File Offset: 0x0004BB6C
	public bool LoadActor(string cardName, bool usePrefabPosition, AssetLoader.GameObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadPrefab(cardName, AssetFamily.Actor, usePrefabPosition, callback, callbackData, persistent, null);
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x0004D988 File Offset: 0x0004BB88
	public GameObject LoadActor(string name, bool usePrefabPosition = false, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadActor() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.Actor, persistent);
		return this.LoadGameObjectImmediately(asset, usePrefabPosition, null);
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x0004D9C0 File Offset: 0x0004BBC0
	public Map<string, EntityDef> LoadBatchCardXmls(List<string> cardIds, out int errors)
	{
		string localDataPath;
		if (ApplicationMgr.TryGetStandaloneLocalDataPath(string.Empty, out localDataPath))
		{
			return this.LoadBatchCardXmlsFromLocalData(localDataPath, cardIds, out errors);
		}
		return this.LoadBatchCardXmlsFromBundle(cardIds, out errors);
	}

	// Token: 0x060011F2 RID: 4594 RVA: 0x0004D9F0 File Offset: 0x0004BBF0
	private Map<string, EntityDef> LoadBatchCardXmlsFromBundle(List<string> cardIds, out int errors)
	{
		errors = 0;
		Map<string, EntityDef> map = new Map<string, EntityDef>();
		string text = Localization.GetLocale().ToString();
		Asset asset = new Asset(text, AssetFamily.CardXML, true);
		AssetBundle bundleForAsset = this.GetBundleForAsset(text, asset);
		if (bundleForAsset == null)
		{
			Error.AddDevFatal("AssetLoader.LoadCardXml: Could not load CardXml bundle", new object[0]);
			return null;
		}
		Object @object = bundleForAsset.LoadAsset(text);
		if (@object == null)
		{
			Error.AddDevFatal("AssetLoader.LoadCardXml: Could not load CardXml for locale " + Localization.GetLocale(), new object[0]);
			return null;
		}
		TextAsset textAsset = (TextAsset)@object;
		using (StringReader stringReader = new StringReader(textAsset.text))
		{
			using (XmlReader xmlReader = XmlReader.Create(stringReader))
			{
				for (;;)
				{
					EntityDef entityDef = new EntityDef();
					if (!entityDef.LoadDataFromCardXml(xmlReader))
					{
						break;
					}
					string cardId = entityDef.GetCardId();
					if (map.ContainsKey(cardId))
					{
						Debug.LogError(string.Format("AssetLoader.LoadBatchedCardXmls: Loaded duplicate card id {0}", cardId));
					}
					else
					{
						map.Add(cardId, entityDef);
					}
				}
			}
		}
		return map;
	}

	// Token: 0x060011F3 RID: 4595 RVA: 0x0004DB3C File Offset: 0x0004BD3C
	private Map<string, EntityDef> LoadBatchCardXmlsFromLocalData(string localDataPath, List<string> cardIds, out int errors)
	{
		errors = 0;
		if (AssetLoader.s_cardSetDirectories == null)
		{
			this.InitCardDirectories(localDataPath);
		}
		Map<string, EntityDef> map = new Map<string, EntityDef>();
		foreach (string text in cardIds)
		{
			if (map.ContainsKey(text))
			{
				Debug.LogError(string.Format("AssetLoader.LoadBatchCardXmlsFromLocalData: Loaded duplicate card id {0}", text));
			}
			else
			{
				string text2 = string.Empty;
				foreach (string text3 in AssetLoader.s_cardSetDirectories)
				{
					string text4 = string.Format("{0}/{1}/{1}.xml", text3, text);
					if (File.Exists(text4))
					{
						text2 = File.ReadAllText(text4);
						break;
					}
					string sourceDir = AssetPathInfo.FamilyInfo[AssetFamily.CardXML].sourceDir;
					text4 = string.Format("{0}/{1}/{2}/{2}.xml", localDataPath, sourceDir, text);
					if (File.Exists(text4))
					{
						text2 = File.ReadAllText(text4);
						break;
					}
				}
				if (string.IsNullOrEmpty(text2))
				{
					errors++;
					Debug.LogWarningFormat("AssetLoader.LoadBatchCardXmlsFromLocalData: Failed to load {0}.xml. Loading PlaceholderCard.xml instead.", new object[]
					{
						text
					});
					string sourceDir2 = AssetPathInfo.FamilyInfo[AssetFamily.CardXML].sourceDir;
					string text5 = string.Format("{0}/{1}/{2}/{2}.xml", localDataPath, sourceDir2, "PlaceholderCard");
					if (!File.Exists(text5))
					{
						Debug.LogErrorFormat("Failed to load PlaceholderCard.xml for {0}.", new object[]
						{
							text
						});
						break;
					}
					text2 = File.ReadAllText(text5);
				}
				map.Add(text, EntityDef.LoadFromString(text, text2, true));
			}
		}
		return map;
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x0004DCEC File Offset: 0x0004BEEC
	public bool LoadCardPrefab(string cardName, AssetLoader.GameObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadPrefab(cardName, AssetFamily.CardPrefab, true, callback, callbackData, persistent, null);
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x0004DD08 File Offset: 0x0004BF08
	public GameObject LoadCardPrefab(string cardName, bool usePrefabPosition = true, bool persistent = false)
	{
		if (cardName == null)
		{
			Error.AddDevFatal("AssetLoader.LoadCardPrefab() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(cardName, AssetFamily.CardPrefab, persistent);
		return this.LoadGameObjectImmediately(asset, usePrefabPosition, null);
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x0004DD40 File Offset: 0x0004BF40
	public Texture LoadCardTexture(string name, bool persistent = false)
	{
		Texture texture = null;
		if (name == null)
		{
			if (!ApplicationMgr.UseDevWorkarounds())
			{
				Error.AddDevFatal("AssetLoader.LoadCardTexture() - An asset request was made but no file name was given.", new object[0]);
				return null;
			}
		}
		else
		{
			Asset asset = new Asset(name, AssetFamily.CardTexture, persistent);
			Object @object = this.LoadObjectImmediately(asset);
			if (@object != null && @object is Texture)
			{
				texture = (Texture)@object;
			}
		}
		if (texture == null)
		{
			if (ApplicationMgr.UseDevWorkarounds())
			{
				Debug.LogErrorFormat("AssetLoader.LoadCardTexture() - Expected a Texture and loaded null or something else for asset {0}.  Using a temp texture.", new object[]
				{
					name
				});
				Texture2D texture2D = new Texture2D(1, 1);
				texture2D.SetPixel(0, 0, Color.magenta);
				texture2D.Apply();
				texture = texture2D;
			}
			else
			{
				Error.AddDevFatal("AssetLoader.LoadCardTexture() - Expected a Texture and loaded null or something else for asset {0}.", new object[]
				{
					name
				});
			}
		}
		return texture;
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x0004DE08 File Offset: 0x0004C008
	public Material LoadPremiumMaterial(string name, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadPremiumMaterial() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.CardPremium, persistent);
		Object @object = this.LoadObjectImmediately(asset);
		if (@object == null || !(@object is Material))
		{
			Error.AddDevFatal("AssetLoader.LoadPremiumMaterial() - Expected a Material and loaded null or something else for asset {0}.", new object[]
			{
				name
			});
			return null;
		}
		return (Material)@object;
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x0004DE74 File Offset: 0x0004C074
	public bool LoadBoard(string boardName, AssetLoader.GameObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadPrefab(boardName, AssetFamily.Board, true, callback, callbackData, persistent, null);
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x0004DE90 File Offset: 0x0004C090
	public GameObject LoadBoard(string name, bool usePrefabPosition = true, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadBoard() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.Board, persistent);
		return this.LoadGameObjectImmediately(asset, usePrefabPosition, null);
	}

	// Token: 0x060011FA RID: 4602 RVA: 0x0004DEC8 File Offset: 0x0004C0C8
	public bool LoadSound(string soundName, AssetLoader.GameObjectCallback callback, object callbackData = null, bool persistent = false, GameObject fallback = null)
	{
		AssetLoader.LoadSoundCallbackData callbackData2 = new AssetLoader.LoadSoundCallbackData
		{
			callback = callback,
			callbackData = callbackData
		};
		return this.LoadPrefab(soundName, AssetFamily.Sound, true, new AssetLoader.GameObjectCallback(this.LoadSoundCallback), callbackData2, persistent, fallback);
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x0004DF08 File Offset: 0x0004C108
	private void LoadSoundCallback(string name, GameObject go, object callbackData)
	{
		AssetLoader.LocalizeSoundPrefab(go);
		AssetLoader.LoadSoundCallbackData loadSoundCallbackData = (AssetLoader.LoadSoundCallbackData)callbackData;
		loadSoundCallbackData.callback(name, go, loadSoundCallbackData.callbackData);
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x0004DF38 File Offset: 0x0004C138
	private static void LocalizeSoundPrefab(GameObject go)
	{
		AudioSource component = go.GetComponent<AudioSource>();
		if (component == null)
		{
			Debug.LogErrorFormat("LocalizeSoundPrefab: trying to load sound prefab with no AudioSource components: \"{0}\"", new object[]
			{
				go.name
			});
			return;
		}
		AudioClip clip = component.clip;
		if (clip == null)
		{
			Debug.LogErrorFormat("LocalizeSoundPrefab: trying to load sound prefab with an AudioSource that contains no AudoClip: \"{0}\"", new object[]
			{
				go.name
			});
			return;
		}
		AudioClip audioClip = AssetLoader.Get().LoadAudioClip(clip.name, false) as AudioClip;
		if (audioClip != null)
		{
			go.GetComponent<AudioSource>().clip = audioClip;
		}
		else
		{
			Debug.LogErrorFormat("LocalizeSoundPrefab: failed to load localized audio clip for sound prefab \"{0}\" with clip \"{1}\"", new object[]
			{
				go.name,
				clip.name
			});
		}
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x0004DFF8 File Offset: 0x0004C1F8
	public GameObject LoadSound(string name, bool usePrefabPosition = true, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadSound() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.Sound, persistent);
		GameObject gameObject = this.LoadGameObjectImmediately(asset, usePrefabPosition, null);
		AssetLoader.LocalizeSoundPrefab(gameObject);
		return gameObject;
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x0004E038 File Offset: 0x0004C238
	public Object LoadAudioClip(string name, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadAudioClip() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.AudioClip, persistent);
		return this.LoadObjectImmediately(asset);
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x0004E06F File Offset: 0x0004C26F
	public bool LoadTexture(string textureName, AssetLoader.ObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadObject(textureName, AssetFamily.Texture, callback, callbackData, persistent);
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x0004E080 File Offset: 0x0004C280
	public Texture LoadTexture(string name, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadTexture() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.Texture, persistent);
		Object @object = this.LoadObjectImmediately(asset);
		if (@object == null || !(@object is Texture))
		{
			Error.AddDevFatal("AssetLoader.LoadTexture() - Expected a Texture and loaded null or something else for asset {0}.", new object[]
			{
				name
			});
			return null;
		}
		return (Texture)@object;
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x0004E0EC File Offset: 0x0004C2EC
	public bool LoadUIScreen(string screenName, AssetLoader.GameObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadPrefab(screenName, AssetFamily.Screen, true, callback, callbackData, false, null);
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x0004E108 File Offset: 0x0004C308
	public GameObject LoadUIScreen(string name, bool usePrefabPosition = true, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadUIScreen() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.Screen, persistent);
		return this.LoadGameObjectImmediately(asset, usePrefabPosition, null);
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x0004E140 File Offset: 0x0004C340
	public bool LoadSpell(string name, AssetLoader.GameObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadPrefab(name, AssetFamily.Spell, true, callback, callbackData, persistent, null);
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x0004E15C File Offset: 0x0004C35C
	public GameObject LoadSpell(string name, bool usePrefabPosition = true, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadSpell() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.Spell, persistent);
		return this.LoadGameObjectImmediately(asset, usePrefabPosition, null);
	}

	// Token: 0x06001205 RID: 4613 RVA: 0x0004E194 File Offset: 0x0004C394
	public bool LoadGameObject(string name, AssetLoader.GameObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadPrefab(name, AssetFamily.GameObject, true, callback, callbackData, persistent, null);
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x0004E1B0 File Offset: 0x0004C3B0
	public GameObject LoadGameObject(string name, bool usePrefabPosition = true, bool persistent = false)
	{
		Asset asset = new Asset(name, AssetFamily.GameObject, persistent);
		return this.LoadGameObjectImmediately(asset, usePrefabPosition, null);
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x0004E1D0 File Offset: 0x0004C3D0
	public bool LoadCardBack(string name, AssetLoader.GameObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadPrefab(name, AssetFamily.CardBack, true, callback, callbackData, persistent, null);
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x0004E1EC File Offset: 0x0004C3EC
	public GameObject LoadCardBack(string name, bool usePrefabPosition = true, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadCardBack() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.CardBack, persistent);
		return this.LoadGameObjectImmediately(asset, usePrefabPosition, null);
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x0004E223 File Offset: 0x0004C423
	public bool LoadMovie(string name, AssetLoader.ObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadObject(name, AssetFamily.Movie, callback, callbackData, persistent);
	}

	// Token: 0x0600120A RID: 4618 RVA: 0x0004E234 File Offset: 0x0004C434
	public MovieTexture LoadMovie(string name, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadMovie() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.Movie, persistent);
		Object @object = this.LoadObjectImmediately(asset);
		if (@object == null || !(@object is MovieTexture))
		{
			Error.AddDevFatal("AssetLoader.LoadMovie() - Expected a MovieTexture and loaded null or something else for asset {0}.", new object[]
			{
				name
			});
			return null;
		}
		return (MovieTexture)@object;
	}

	// Token: 0x0600120B RID: 4619 RVA: 0x0004E2A0 File Offset: 0x0004C4A0
	public bool LoadFontDef(string name, AssetLoader.GameObjectCallback callback, object callbackData = null, bool persistent = false)
	{
		return this.LoadPrefab(name, AssetFamily.FontDef, true, callback, callbackData, persistent, null);
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x0004E2BC File Offset: 0x0004C4BC
	public GameObject LoadFontDef(string name, bool usePrefabPosition = true, bool persistent = false)
	{
		if (name == null)
		{
			Error.AddDevFatal("AssetLoader.LoadFontDef() - An asset request was made but no file name was given.", new object[0]);
			return null;
		}
		Asset asset = new Asset(name, AssetFamily.FontDef, persistent);
		return this.LoadGameObjectImmediately(asset, usePrefabPosition, null);
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x0004E2F4 File Offset: 0x0004C4F4
	public void UnloadUpdatableBundles()
	{
		foreach (KeyValuePair<AssetFamily, AssetFamilyBundleInfo> keyValuePair in AssetBundleInfo.FamilyInfo)
		{
			AssetFamily key = keyValuePair.Key;
			AssetFamilyBundleInfo value = keyValuePair.Value;
			if (value.Updatable)
			{
				for (int i = 0; i < value.NumberOfBundles; i++)
				{
					AssetBundle bundleForFamily = this.GetBundleForFamily(key, i, default(Locale?), null);
					if (bundleForFamily)
					{
						bundleForFamily.Unload(true);
					}
				}
				for (int j = 0; j < value.NumberOfLocaleBundles; j++)
				{
					Locale[] loadOrder = Localization.GetLoadOrder(false);
					for (int k = 0; k < loadOrder.Length; k++)
					{
						AssetBundle bundleForFamily2 = this.GetBundleForFamily(key, j, new Locale?(loadOrder[k]), null);
						if (bundleForFamily2)
						{
							bundleForFamily2.Unload(true);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x0004E414 File Offset: 0x0004C614
	public void ReloadUpdatableBundles()
	{
		foreach (KeyValuePair<AssetFamily, AssetFamilyBundleInfo> keyValuePair in AssetBundleInfo.FamilyInfo)
		{
			AssetFamily key = keyValuePair.Key;
			AssetFamilyBundleInfo value = keyValuePair.Value;
			if (value.Updatable)
			{
				for (int i = 0; i < value.NumberOfBundles; i++)
				{
					this.GetBundleForFamily(key, i, default(Locale?), null);
				}
				for (int j = 0; j < value.NumberOfLocaleBundles; j++)
				{
					Locale[] loadOrder = Localization.GetLoadOrder(false);
					for (int k = 0; k < loadOrder.Length; k++)
					{
						this.GetBundleForFamily(key, j, new Locale?(loadOrder[k]), null);
					}
				}
			}
		}
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x0004E508 File Offset: 0x0004C708
	private bool LoadObject(string assetName, AssetFamily family, AssetLoader.ObjectCallback callback, object callbackData, bool persistent = false)
	{
		if (string.IsNullOrEmpty(assetName))
		{
			Log.Asset.Print("AssetLoader.LoadObject() - name was null or empty", new object[0]);
			return false;
		}
		Asset asset = new Asset(assetName, family, persistent);
		this.LoadCachedObject(asset, callback, callbackData);
		return true;
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x0004E54C File Offset: 0x0004C74C
	private bool LoadPrefab(string assetName, AssetFamily family, bool usePrefabPosition, AssetLoader.GameObjectCallback callback, object callbackData, bool persistent = false, Object fallback = null)
	{
		if (string.IsNullOrEmpty(assetName))
		{
			Debug.LogWarning("AssetLoader.LoadPrefab() - name was null or empty");
			return false;
		}
		Asset asset = new Asset(assetName, family, persistent);
		this.LoadCachedPrefab(asset, usePrefabPosition, callback, callbackData, fallback);
		return true;
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x0004E58C File Offset: 0x0004C78C
	private IEnumerator DownloadFile(string path, AssetLoader.FileCallback callback, object callbackData)
	{
		if (path == null)
		{
			if (callback != null)
			{
				callback(null, null, callbackData);
			}
			yield break;
		}
		string filePath = this.CreateLocalFilePath(path);
		WWW file = this.CreateLocalFile(filePath);
		yield return base.StartCoroutine(AssetLoader.WaitForDownload(file));
		if (file.error != null)
		{
			string message = string.Format("AssetLoader.DownloadFile() - FAILED to load asset '{0}' path '{1}', reason '{2}'", path, file.url, file.error);
			Debug.LogError(message);
			if (callback != null)
			{
				callback(path, null, callbackData);
			}
			yield break;
		}
		if (callback != null)
		{
			callback(path, file, callbackData);
		}
		yield break;
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x0004E5D4 File Offset: 0x0004C7D4
	private static IEnumerator WaitForDownload(WWW file)
	{
		while (!file.isDone)
		{
			yield return 0;
		}
		yield break;
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x0004E5F8 File Offset: 0x0004C7F8
	private WWW CreateLocalFile(string absPath)
	{
		string text = string.Format("file://{0}", absPath);
		return new WWW(text);
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x0004E618 File Offset: 0x0004C818
	private string CreateLocalFilePath(string relPath)
	{
		return string.Format("{0}/{1}", this.m_workingDir, relPath);
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x0004E638 File Offset: 0x0004C838
	private void LoadCachedObject(Asset asset, AssetLoader.ObjectCallback callback, object callbackData)
	{
		if (asset.GetName() == null)
		{
			if (callback != null)
			{
				callback(null, null, callbackData);
			}
			return;
		}
		long num = TimeUtils.BinaryStamp();
		AssetCache.CachedAsset cachedAsset2 = AssetCache.Find(asset);
		if (cachedAsset2 != null)
		{
			cachedAsset2.SetLastRequestTimestamp(num);
			if (callback != null)
			{
				Object assetObject = cachedAsset2.GetAssetObject();
				callback(asset.GetName(), assetObject, callbackData);
			}
			return;
		}
		AssetCache.ObjectCacheRequest request2 = AssetCache.GetRequest<AssetCache.ObjectCacheRequest>(asset);
		if (request2 != null)
		{
			request2.SetLastRequestTimestamp(num);
			if (request2.DidFail())
			{
				if (callback != null)
				{
					callback(asset.GetName(), null, callbackData);
				}
			}
			else
			{
				request2.AddRequester(callback, callbackData);
			}
			return;
		}
		AssetCache.ObjectCacheRequest request = new AssetCache.ObjectCacheRequest();
		request.SetPersistent(asset.IsPersistent());
		request.SetCreatedTimestamp(num);
		request.SetLastRequestTimestamp(num);
		request.AddRequester(callback, callbackData);
		AssetCache.AddRequest(asset, request);
		Action<AssetCache.CachedAsset> successCallback = delegate(AssetCache.CachedAsset cachedAsset)
		{
			Object assetObject2 = cachedAsset.GetAssetObject();
			request.OnLoadComplete(asset.GetName(), assetObject2);
		};
		base.StartCoroutine(this.CreateCachedAsset<AssetCache.ObjectCacheRequest, Object>(request, asset, successCallback, null));
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x0004E790 File Offset: 0x0004C990
	private void LoadCachedPrefab(Asset asset, bool usePrefabPosition, AssetLoader.GameObjectCallback callback, object callbackData, Object fallback)
	{
		if (asset.GetName() == null)
		{
			if (callback != null)
			{
				callback(null, null, callbackData);
			}
			return;
		}
		long num = TimeUtils.BinaryStamp();
		AssetCache.CachedAsset cachedAsset2 = AssetCache.Find(asset);
		if (cachedAsset2 != null)
		{
			cachedAsset2.SetLastRequestTimestamp(num);
			Object assetObject = cachedAsset2.GetAssetObject();
			base.StartCoroutine(this.WaitThenCallGameObjectCallback(asset, assetObject, usePrefabPosition, callback, callbackData));
			return;
		}
		AssetCache.PrefabCacheRequest request2 = AssetCache.GetRequest<AssetCache.PrefabCacheRequest>(asset);
		if (request2 != null)
		{
			request2.SetLastRequestTimestamp(num);
			if (request2.DidFail())
			{
				if (callback != null)
				{
					callback(asset.GetName(), null, callbackData);
				}
			}
			else
			{
				request2.AddRequester(callback, callbackData);
			}
			return;
		}
		AssetCache.PrefabCacheRequest request = new AssetCache.PrefabCacheRequest();
		request.SetPersistent(asset.IsPersistent());
		request.SetCreatedTimestamp(num);
		request.SetLastRequestTimestamp(num);
		request.AddRequester(callback, callbackData);
		AssetCache.AddRequest(asset, request);
		Action<AssetCache.CachedAsset> successCallback = delegate(AssetCache.CachedAsset cachedAsset)
		{
			Object assetObject2 = cachedAsset.GetAssetObject();
			foreach (AssetCache.GameObjectRequester gameObjectRequester in request.GetRequesters())
			{
				this.StartCoroutine(this.WaitThenCallGameObjectCallback(asset, assetObject2, usePrefabPosition, gameObjectRequester.m_callback, gameObjectRequester.m_callbackData));
			}
		};
		base.StartCoroutine(this.CreateCachedAsset<AssetCache.PrefabCacheRequest, GameObject>(request, asset, successCallback, fallback));
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x0004E904 File Offset: 0x0004CB04
	private IEnumerator CreateCachedAsset<RequestType, AssetType>(RequestType request, Asset asset, Action<AssetCache.CachedAsset> successCallback, Object fallback) where RequestType : AssetCache.CacheRequest
	{
		AssetCache.StartLoading(asset.GetName());
		yield return base.StartCoroutine(this.CreateCachedAsset_FromBundle<RequestType>(request, asset, fallback));
		AssetCache.CachedAsset cachedAsset = AssetCache.Find(asset);
		if (!request.DidSucceed() || cachedAsset == null)
		{
			AssetCache.StopLoading(asset.GetName());
			yield break;
		}
		successCallback.Invoke(cachedAsset);
		AssetCache.StopLoading(asset.GetName());
		yield break;
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x0004E95C File Offset: 0x0004CB5C
	private void CacheAsset(Asset asset, Object assetObject)
	{
		long num = TimeUtils.BinaryStamp();
		AssetCache.CachedAsset cachedAsset = new AssetCache.CachedAsset();
		cachedAsset.SetAsset(asset);
		cachedAsset.SetAssetObject(assetObject);
		cachedAsset.SetCreatedTimestamp(num);
		cachedAsset.SetLastRequestTimestamp(num);
		cachedAsset.SetPersistent(asset.IsPersistent());
		AssetCache.Add(asset, cachedAsset);
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x0004E9A4 File Offset: 0x0004CBA4
	private void InitCardDirectories(string root = null)
	{
		string text = AssetPathInfo.FamilyInfo[AssetFamily.CardXML].sourceDir;
		text = ((root == null) ? text : Path.Combine(root, text));
		DirectoryInfo directoryInfo = new DirectoryInfo(text);
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		AssetLoader.s_cardSetDirectories = new string[directories.Length];
		for (int i = 0; i < directories.Length; i++)
		{
			DirectoryInfo directoryInfo2 = directories[i];
			string name = directoryInfo2.Name;
			AssetLoader.s_cardSetDirectories[i] = string.Format("{0}/{1}", text, name);
		}
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x0004EA28 File Offset: 0x0004CC28
	private void PreloadBundles()
	{
		if (AssetBundleInfo.UseSharedDependencyBundle)
		{
			for (int i = 0; i < 4; i++)
			{
				string text = this.CreateLocalFilePath(string.Format("Data/{0}{1}{2}.unity3d", AssetBundleInfo.BundlePathPlatformModifier(), "shared", i));
				this.m_sharedBundle[i] = AssetBundle.CreateFromFile(text);
			}
		}
		foreach (KeyValuePair<AssetFamily, AssetFamilyBundleInfo> keyValuePair in AssetBundleInfo.FamilyInfo)
		{
			AssetFamily key = keyValuePair.Key;
			AssetFamilyBundleInfo value = keyValuePair.Value;
			for (int j = 0; j < value.NumberOfBundles + AssetLoader.AvailableExtraAssetBundlesCount(); j++)
			{
				this.GetBundleForFamily(key, j, default(Locale?), null);
			}
			for (int k = 0; k < value.NumberOfLocaleBundles; k++)
			{
				Locale[] loadOrder = Localization.GetLoadOrder(false);
				for (int l = 0; l < loadOrder.Length; l++)
				{
					this.GetBundleForFamily(key, k, new Locale?(loadOrder[l]), null);
				}
			}
		}
		if (AssetLoader.DOWNLOADABLE_LANGUAGE_PACKS)
		{
			DownloadManifest.Get();
		}
		AssetLoader.InitFileListInExtraBundles();
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x0004EB7C File Offset: 0x0004CD7C
	private AssetBundle GetBundleForFamily(AssetFamily family, string assetName, Locale? locale = null)
	{
		return this.GetBundleForFamily(family, -1, locale, assetName);
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x0004EB88 File Offset: 0x0004CD88
	private AssetBundle GetBundleForFamily(AssetFamily family, int bundleNum, Locale? locale = null, string assetName = null)
	{
		AssetFamilyBundleInfo assetFamilyBundleInfo;
		try
		{
			assetFamilyBundleInfo = AssetBundleInfo.FamilyInfo[family];
		}
		catch (IndexOutOfRangeException)
		{
			Debug.LogErrorFormat("GetBundleForFamily: Failed to find bundle family: \"{0}\" for asset: \"{1}\".", new object[]
			{
				family,
				assetName
			});
			return null;
		}
		int num = (locale != null) ? assetFamilyBundleInfo.NumberOfLocaleBundles : assetFamilyBundleInfo.NumberOfBundles;
		if (!string.IsNullOrEmpty(assetName))
		{
			if (AssetLoader.FileIsInFileListInExtraBundles(assetName))
			{
				bundleNum = num;
			}
			else
			{
				bundleNum = ((num > 1) ? GeneralUtils.UnsignedMod(assetName.GetHashCode(), num) : 0);
			}
		}
		if (locale != null)
		{
			string text = string.Format("{0}{1}{2}.unity3d", assetFamilyBundleInfo.BundleName, EnumUtils.GetString<Locale>(locale.Value), bundleNum);
			if (UpdateManager.Get().ContainsFile(text))
			{
				Log.UpdateManager.Print("AssetLoader.GetBundleForFamily - Use UpdateManager.GetAssetBundle {0}", new object[]
				{
					text
				});
				this.m_familyBundles[(int)family, bundleNum] = UpdateManager.Get().GetAssetBundle(text);
			}
			if (this.m_localizedFamilyBundles[(int)family, bundleNum] == null)
			{
				string text2 = this.CreateLocalFilePath(string.Format("Data/{0}{1}", AssetBundleInfo.BundlePathPlatformModifier(), text));
				this.m_localizedFamilyBundles[(int)family, bundleNum] = ((!File.Exists(text2)) ? null : AssetBundle.CreateFromFile(text2));
			}
			return this.m_localizedFamilyBundles[(int)family, bundleNum];
		}
		if (this.m_familyBundles[(int)family, bundleNum] == null)
		{
			string text3 = string.Format("{0}{1}.unity3d", assetFamilyBundleInfo.BundleName, bundleNum);
			if (UpdateManager.Get().ContainsFile(text3))
			{
				Log.UpdateManager.Print("AssetLoader.GetBundleForFamily - Use UpdateManager.GetAssetBundle {0}", new object[]
				{
					text3
				});
				this.m_familyBundles[(int)family, bundleNum] = UpdateManager.Get().GetAssetBundle(text3);
			}
			if (this.m_familyBundles[(int)family, bundleNum] == null)
			{
				string text2 = this.CreateLocalFilePath(string.Format("Data/{0}{1}", AssetBundleInfo.BundlePathPlatformModifier(), text3));
				if (File.Exists(text2))
				{
					AssetBundle assetBundle = AssetBundle.CreateFromFile(text2);
					if (assetBundle != null)
					{
						this.m_familyBundles[(int)family, bundleNum] = assetBundle;
					}
				}
			}
		}
		return this.m_familyBundles[(int)family, bundleNum];
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x0004EE00 File Offset: 0x0004D000
	private AssetBundle GetDownloadedBundleForFamily(AssetFamily family, string assetName, Locale locale)
	{
		int hashCode = assetName.GetHashCode();
		int numberOfDownloadableLocaleBundles = AssetBundleInfo.FamilyInfo[family].NumberOfDownloadableLocaleBundles;
		int num = (numberOfDownloadableLocaleBundles > 1) ? GeneralUtils.UnsignedMod(hashCode, numberOfDownloadableLocaleBundles) : 0;
		if (this.m_downloadedLocalizedFamilyBundles[(int)family, num] == null)
		{
			string text = DownloadManifest.Get().DownloadableBundleFileName(string.Format("{0}{1}_dlc_{2}.unity3d", AssetBundleInfo.FamilyInfo[family].BundleName, EnumUtils.GetString<Locale>(locale), num));
			if (text == null)
			{
				return null;
			}
			Log.Asset.Print("Attempting to load localized, downloaded AssetBundle {0}", new object[]
			{
				text
			});
			this.m_downloadedLocalizedFamilyBundles[(int)family, num] = Downloader.Get().GetDownloadedBundle(text);
		}
		return this.m_downloadedLocalizedFamilyBundles[(int)family, num];
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x0004EED0 File Offset: 0x0004D0D0
	private AssetBundle GetBundleForAsset(string assetName, AssetFamily family, bool downloaded)
	{
		if (!downloaded)
		{
			AssetBundle assetBundle = this.GetBundleForFamily(family, assetName, default(Locale?));
			if (assetBundle != null && assetBundle.Contains(assetName))
			{
				return assetBundle;
			}
		}
		Locale[] loadOrder = Localization.GetLoadOrder(family);
		for (int i = 0; i < loadOrder.Length; i++)
		{
			AssetBundle assetBundle = (!downloaded) ? this.GetBundleForFamily(family, assetName, new Locale?(loadOrder[i])) : this.GetDownloadedBundleForFamily(family, assetName, loadOrder[i]);
			if (assetBundle != null && assetBundle.Contains(assetName))
			{
				return assetBundle;
			}
		}
		return null;
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x0004EF6C File Offset: 0x0004D16C
	private AssetBundle GetBundleForAsset(string assetName, Asset asset)
	{
		bool downloaded = this.AssetFromDownloadablePack(asset);
		AssetBundle bundleForAsset = this.GetBundleForAsset(assetName, asset.GetFamily(), downloaded);
		if (bundleForAsset != null)
		{
			return bundleForAsset;
		}
		return null;
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x0004EFA0 File Offset: 0x0004D1A0
	private AssetBundle GetBundleForAsset(Asset asset, out string finalAssetName)
	{
		foreach (Locale locale in Localization.GetLoadOrder(asset.GetFamily()))
		{
			finalAssetName = string.Format("Final/{0}", asset.GetPath(locale));
			AssetBundle bundleForAsset = this.GetBundleForAsset(finalAssetName, asset);
			if (bundleForAsset != null)
			{
				asset.SetLocale(locale);
				return bundleForAsset;
			}
		}
		finalAssetName = string.Format("Final/{0}", asset.GetPath());
		return this.GetBundleForAsset(finalAssetName, asset);
	}

	// Token: 0x06001221 RID: 4641 RVA: 0x0004F020 File Offset: 0x0004D220
	private IEnumerator CreateCachedAsset_FromBundle<RequestType>(RequestType request, Asset asset, Object fallback) where RequestType : AssetCache.CacheRequest
	{
		string finalAssetName;
		AssetBundle bundle = this.GetBundleForAsset(asset, out finalAssetName);
		if (bundle == null)
		{
			if (!this.AssetFromDownloadablePack(asset))
			{
				if (fallback != null)
				{
					AssetLoader.LogMissingAsset(asset.GetFamily(), finalAssetName);
					Debug.LogError(string.Concat(new object[]
					{
						"Asset ",
						finalAssetName,
						" in ",
						asset.GetFamily(),
						"failed to load. Using fallback."
					}));
					AssetCache.RemoveRequest(asset);
					this.CacheAsset(asset, fallback);
					request.OnLoadSucceeded();
				}
				else
				{
					request.OnLoadFailed(asset.GetName());
					Debug.LogError("Fatal Error: Asset not found in family or backup bundles: " + finalAssetName);
					string userErrorMessage = string.Format("Failed to load bundle for {0}. Should be in bundle {1}.", finalAssetName, AssetBundleInfo.FamilyInfo[asset.GetFamily()].BundleName);
					Error.AddDevFatal(userErrorMessage, new object[0]);
				}
			}
			else
			{
				request.OnLoadFailed(asset.GetName());
				Debug.LogError("Downloadable asset failed to load : " + finalAssetName);
			}
			yield break;
		}
		Object result = bundle.LoadAsset(finalAssetName);
		if (result == null)
		{
			AssetLoader.LogMissingAsset(asset.GetFamily(), finalAssetName);
		}
		AssetCache.RemoveRequest(asset);
		this.CacheAsset(asset, result);
		request.OnLoadSucceeded();
		yield break;
	}

	// Token: 0x06001222 RID: 4642 RVA: 0x0004F068 File Offset: 0x0004D268
	private GameObject LoadGameObjectImmediately(Asset asset, bool usePrefabPosition, GameObject fallback = null)
	{
		Object @object = this.LoadObjectImmediately(asset);
		GameObject gameObject = @object as GameObject;
		if (!gameObject)
		{
			Debug.LogErrorFormat("AssetLoader.LoadGameObjectImmediately() - Expected a prefab and loaded null or something else for asset {0} from family {1} .", new object[]
			{
				asset.GetName(),
				asset.GetFamily()
			});
			if (fallback == null)
			{
				return null;
			}
			gameObject = fallback;
		}
		GameObject result;
		if (usePrefabPosition)
		{
			result = Object.Instantiate<GameObject>(gameObject);
		}
		else
		{
			result = (GameObject)Object.Instantiate(gameObject, this.NewGameObjectSpawnPosition(gameObject), gameObject.transform.rotation);
		}
		return result;
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x0004F0FC File Offset: 0x0004D2FC
	private Object LoadObjectImmediately(Asset asset)
	{
		Object @object = null;
		AssetCache.CachedAsset cachedAsset = AssetCache.Find(asset);
		if (cachedAsset != null)
		{
			cachedAsset.SetLastRequestTimestamp(TimeUtils.BinaryStamp());
			@object = cachedAsset.GetAssetObject();
		}
		else
		{
			string text;
			AssetBundle bundleForAsset = this.GetBundleForAsset(asset, out text);
			if (bundleForAsset != null)
			{
				@object = bundleForAsset.LoadAsset(text);
				if (@object != null)
				{
					this.CacheAsset(asset, @object);
				}
				else
				{
					Debug.LogError("Unable to find asset " + asset.GetName() + " in bundle " + bundleForAsset.name);
				}
			}
			else
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Unable to find asset bundle for ",
					asset.GetFamily(),
					" ",
					asset.GetName(),
					" ",
					text
				}));
			}
		}
		return @object;
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x0004F1D0 File Offset: 0x0004D3D0
	private IEnumerator WaitThenCallGameObjectCallback(Asset asset, Object prefab, bool usePrefabPosition, AssetLoader.GameObjectCallback callback, object callbackData)
	{
		string name = asset.GetName();
		if (!(prefab is GameObject))
		{
			string userErrorMessage = GameStrings.Format("GLOBAL_ERROR_ASSET_INCORRECT_DATA", new object[]
			{
				name
			});
			Error.AddFatal(userErrorMessage);
			string internalErrorMessage = string.Format("AssetLoader.WaitThenCallGameObjectCallback() - {0} (prefab={1})", userErrorMessage, prefab);
			Debug.LogError(internalErrorMessage);
			if (callback != null)
			{
				callback(name, null, callbackData);
			}
			yield break;
		}
		AssetCache.StartLoading(name);
		GameObject original = (GameObject)prefab;
		GameObject instance = null;
		if (usePrefabPosition)
		{
			instance = (GameObject)Object.Instantiate(prefab);
		}
		else
		{
			instance = (GameObject)Object.Instantiate(prefab, this.NewGameObjectSpawnPosition(prefab), original.transform.rotation);
		}
		this.m_waitingOnObjects.Add(instance);
		yield return new WaitForEndOfFrame();
		this.m_waitingOnObjects.Remove(instance);
		AssetCache.StopLoading(name);
		if (!AssetCache.HasItem(asset))
		{
			Object.DestroyImmediate(instance);
		}
		if (GeneralUtils.IsCallbackValid(callback))
		{
			callback(name, instance, callbackData);
		}
		yield break;
	}

	// Token: 0x06001225 RID: 4645 RVA: 0x0004F238 File Offset: 0x0004D438
	private Vector3 NewGameObjectSpawnPosition(Object prefab)
	{
		if (Camera.main == null)
		{
			return Vector3.zero;
		}
		Vector3 position = Camera.main.transform.position;
		return position + this.SPAWN_POS_CAMERA_OFFSET;
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x0004F278 File Offset: 0x0004D478
	private bool AssetFromDownloadablePack(Asset asset)
	{
		if (AssetLoader.DOWNLOADABLE_LANGUAGE_PACKS && Localization.GetLocale() != Locale.enUS && Localization.GetLocale() != Locale.enGB)
		{
			string text = FileUtils.StripLocaleFromPath(string.Format("Final/{0}", asset.GetPath()));
			bool flag = DownloadManifest.Get().ContainsFile(text);
			if (flag)
			{
				Log.Downloader.Print(string.Format("File {0} is downloadable according to manifest.", text), new object[0]);
			}
			return flag;
		}
		return false;
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x0004F2F0 File Offset: 0x0004D4F0
	private static void LogMissingAsset(AssetFamily family, string assetname)
	{
		Log.MissingAssets.Print(LogLevel.Error, string.Format("[{0}] {1}", family, assetname), new object[0]);
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x0004F320 File Offset: 0x0004D520
	private static void InitFileListInExtraBundles()
	{
		AssetLoader.fileListInExtraBundles_.Clear();
		string text = string.Format("{0}/{1}", FileUtils.PersistentDataPath, "manifest-filelist-extra.csv");
		if (!File.Exists(text))
		{
			text = FileUtils.GetAssetPath("manifest-filelist-extra.csv");
		}
		Log.UpdateManager.Print("InitFileListInExtraBundles - {0}", new object[]
		{
			text
		});
		if (!File.Exists(text))
		{
			return;
		}
		using (StreamReader streamReader = new StreamReader(text))
		{
			string text2;
			while ((text2 = streamReader.ReadLine()) != null)
			{
				AssetLoader.fileListInExtraBundles_.Add(text2);
			}
		}
		Log.UpdateManager.Print("InitFileListInExtraBundles - Success", new object[0]);
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x0004F3E4 File Offset: 0x0004D5E4
	private static bool UseFileListInExtraBundles()
	{
		return AssetLoader.fileListInExtraBundles_.Count != 0;
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x0004F3F6 File Offset: 0x0004D5F6
	private static bool FileIsInFileListInExtraBundles(string fileName)
	{
		return AssetLoader.UseFileListInExtraBundles() && AssetLoader.fileListInExtraBundles_.Contains(fileName);
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x0004F40F File Offset: 0x0004D60F
	private static int AvailableExtraAssetBundlesCount()
	{
		return (!AssetLoader.UseFileListInExtraBundles()) ? 9 : 1;
	}

	// Token: 0x04000964 RID: 2404
	private const string s_fileInfoCachePath = "filecache.txt";

	// Token: 0x04000965 RID: 2405
	public const string fileListInExtraBundlesTxtName_ = "manifest-filelist-extra.csv";

	// Token: 0x04000966 RID: 2406
	public static readonly PlatformDependentValue<bool> DOWNLOADABLE_LANGUAGE_PACKS = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		iOS = true,
		Android = true,
		PC = false,
		Mac = false
	};

	// Token: 0x04000967 RID: 2407
	private readonly Vector3 SPAWN_POS_CAMERA_OFFSET = new Vector3(0f, 0f, -5000f);

	// Token: 0x04000968 RID: 2408
	private static AssetLoader s_instance;

	// Token: 0x04000969 RID: 2409
	private bool m_ready;

	// Token: 0x0400096A RID: 2410
	private string m_workingDir;

	// Token: 0x0400096B RID: 2411
	private List<GameObject> m_waitingOnObjects = new List<GameObject>();

	// Token: 0x0400096C RID: 2412
	private AssetBundle[,] m_familyBundles;

	// Token: 0x0400096D RID: 2413
	private AssetBundle[,] m_localizedFamilyBundles;

	// Token: 0x0400096E RID: 2414
	private AssetBundle[,] m_downloadedFamilyBundles;

	// Token: 0x0400096F RID: 2415
	private AssetBundle[,] m_downloadedLocalizedFamilyBundles;

	// Token: 0x04000970 RID: 2416
	private AssetBundle[] m_sharedBundle = new AssetBundle[4];

	// Token: 0x04000971 RID: 2417
	private static Map<AssetFamily, Map<string, List<LightFileInfo>>> s_fileInfos;

	// Token: 0x04000972 RID: 2418
	private static string[] s_cardSetDirectories;

	// Token: 0x04000973 RID: 2419
	private static float s_timeSpentBruteForcing = 0f;

	// Token: 0x04000974 RID: 2420
	private static HashSet<string> fileListInExtraBundles_ = new HashSet<string>();

	// Token: 0x02000152 RID: 338
	// (Invoke) Token: 0x0600122D RID: 4653
	public delegate void GameObjectCallback(string name, GameObject go, object callbackData);

	// Token: 0x020001D6 RID: 470
	// (Invoke) Token: 0x06001D81 RID: 7553
	public delegate void ObjectCallback(string name, Object obj, object callbackData);

	// Token: 0x020002C3 RID: 707
	private class LoadSoundCallbackData
	{
		// Token: 0x04001650 RID: 5712
		public AssetLoader.GameObjectCallback callback;

		// Token: 0x04001651 RID: 5713
		public object callbackData;
	}

	// Token: 0x020002C4 RID: 708
	// (Invoke) Token: 0x060025D3 RID: 9683
	public delegate void FileCallback(string path, WWW file, object callbackData);
}
