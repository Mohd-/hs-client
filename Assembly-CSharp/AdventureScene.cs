using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000218 RID: 536
[CustomEditClass]
public class AdventureScene : Scene
{
	// Token: 0x17000336 RID: 822
	// (get) Token: 0x060020C4 RID: 8388 RVA: 0x000A025C File Offset: 0x0009E45C
	// (set) Token: 0x060020C5 RID: 8389 RVA: 0x000A0264 File Offset: 0x0009E464
	public bool IsDevMode { get; set; }

	// Token: 0x17000337 RID: 823
	// (get) Token: 0x060020C6 RID: 8390 RVA: 0x000A026D File Offset: 0x0009E46D
	// (set) Token: 0x060020C7 RID: 8391 RVA: 0x000A0275 File Offset: 0x0009E475
	public int DevModeSetting { get; set; }

	// Token: 0x060020C8 RID: 8392 RVA: 0x000A0280 File Offset: 0x0009E480
	protected override void Awake()
	{
		base.Awake();
		AdventureScene.s_instance = this;
		this.m_CurrentSubScene = null;
		this.m_TransitionOutSubScene = null;
		AdventureConfig adventureConfig = AdventureConfig.Get();
		adventureConfig.OnAdventureSceneAwake();
		adventureConfig.AddSubSceneChangeListener(new AdventureConfig.SubSceneChange(this.OnSubSceneChange));
		adventureConfig.AddSelectedModeChangeListener(new AdventureConfig.SelectedModeChange(this.OnSelectedModeChanged));
		adventureConfig.AddAdventureModeChangeListener(new AdventureConfig.AdventureModeChange(this.OnAdventureModeChanged));
		this.m_StartupAssetLoads++;
		bool @bool = Options.Get().GetBool(Option.HAS_SEEN_NAXX);
		if (!@bool)
		{
			this.m_StartupAssetLoads++;
		}
		this.LoadSubScene(adventureConfig.GetCurrentSubScene(), new AssetLoader.GameObjectCallback(this.OnFirstSubSceneLoaded));
		if (!@bool)
		{
			SoundManager.Get().Load("VO_KT_INTRO_39");
			AssetLoader.Get().LoadGameObject("KT_Quote", new AssetLoader.GameObjectCallback(this.OnKTQuoteLoaded), null, false);
		}
		Options.Get().SetBool(Option.BUNDLE_JUST_PURCHASE_IN_HUB, false);
		if (ApplicationMgr.IsInternal())
		{
			CheatMgr.Get().RegisterCheatHandler("advdev", new CheatMgr.ProcessCheatCallback(this.OnDevCheat), null, null, null);
		}
		this.InitializeAllDefs();
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x000A03A5 File Offset: 0x0009E5A5
	private void Start()
	{
		AdventureConfig.Get().UpdatePresence();
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x000A03B1 File Offset: 0x0009E5B1
	private void OnDestroy()
	{
		AdventureScene.s_instance = null;
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x000A03B9 File Offset: 0x0009E5B9
	private void Update()
	{
		Network.Get().ProcessNetwork();
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x000A03C5 File Offset: 0x0009E5C5
	public static AdventureScene Get()
	{
		return AdventureScene.s_instance;
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x000A03CC File Offset: 0x0009E5CC
	public override bool IsUnloading()
	{
		return this.m_Unloading;
	}

	// Token: 0x060020CE RID: 8398 RVA: 0x000A03D4 File Offset: 0x0009E5D4
	public override void Unload()
	{
		this.m_Unloading = true;
		AdventureConfig adventureConfig = AdventureConfig.Get();
		adventureConfig.ClearBossDefs();
		DeckPickerTray.Get().Unload();
		adventureConfig.RemoveAdventureModeChangeListener(new AdventureConfig.AdventureModeChange(this.OnAdventureModeChanged));
		adventureConfig.RemoveSelectedModeChangeListener(new AdventureConfig.SelectedModeChange(this.OnSelectedModeChanged));
		adventureConfig.RemoveSubSceneChangeListener(new AdventureConfig.SubSceneChange(this.OnSubSceneChange));
		adventureConfig.OnAdventureSceneUnload();
		CheatMgr.Get().UnregisterCheatHandler("advdev", new CheatMgr.ProcessCheatCallback(this.OnDevCheat));
		this.m_Unloading = false;
	}

	// Token: 0x060020CF RID: 8399 RVA: 0x000A045C File Offset: 0x0009E65C
	public bool IsInitialScreen()
	{
		return this.m_SubScenesLoaded <= 1;
	}

	// Token: 0x060020D0 RID: 8400 RVA: 0x000A046C File Offset: 0x0009E66C
	public AdventureDef GetAdventureDef(AdventureDbId advId)
	{
		AdventureDef result = null;
		this.m_adventureDefs.TryGetValue(advId, out result);
		return result;
	}

	// Token: 0x060020D1 RID: 8401 RVA: 0x000A048C File Offset: 0x0009E68C
	public List<AdventureDef> GetSortedAdventureDefs()
	{
		List<AdventureDef> list = new List<AdventureDef>(this.m_adventureDefs.Values);
		list.Sort((AdventureDef l, AdventureDef r) => r.GetSortOrder() - l.GetSortOrder());
		return list;
	}

	// Token: 0x060020D2 RID: 8402 RVA: 0x000A04D0 File Offset: 0x0009E6D0
	public AdventureWingDef GetWingDef(WingDbId wingId)
	{
		AdventureWingDef result = null;
		this.m_wingDefs.TryGetValue(wingId, out result);
		return result;
	}

	// Token: 0x060020D3 RID: 8403 RVA: 0x000A04F0 File Offset: 0x0009E6F0
	public List<AdventureWingDef> GetWingDefsFromAdventure(AdventureDbId advId)
	{
		List<AdventureWingDef> list = new List<AdventureWingDef>();
		foreach (KeyValuePair<WingDbId, AdventureWingDef> keyValuePair in this.m_wingDefs)
		{
			if (keyValuePair.Value.GetAdventureId() == advId)
			{
				list.Add(keyValuePair.Value);
			}
		}
		return list;
	}

	// Token: 0x060020D4 RID: 8404 RVA: 0x000A056C File Offset: 0x0009E76C
	public bool IsAdventureOpen(AdventureDbId advId)
	{
		bool result = true;
		foreach (KeyValuePair<WingDbId, AdventureWingDef> keyValuePair in this.m_wingDefs)
		{
			if (keyValuePair.Value.GetAdventureId() == advId)
			{
				if (AdventureProgressMgr.Get().IsWingOpen((int)keyValuePair.Value.GetWingId()))
				{
					return true;
				}
				result = false;
			}
		}
		return result;
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x000A05FC File Offset: 0x0009E7FC
	private void InitializeAllDefs()
	{
		List<AdventureDbfRecord> adventureRecordsWithDefPrefab = GameUtils.GetAdventureRecordsWithDefPrefab();
		List<AdventureDataDbfRecord> adventureDataRecordsWithSubDefPrefab = GameUtils.GetAdventureDataRecordsWithSubDefPrefab();
		foreach (AdventureDbfRecord adventureDbfRecord in adventureRecordsWithDefPrefab)
		{
			AdventureDef adventureDef = GameUtils.LoadGameObjectWithComponent<AdventureDef>(adventureDbfRecord.AdventureDefPrefab);
			if (!(adventureDef == null))
			{
				adventureDef.Init(adventureDbfRecord, adventureDataRecordsWithSubDefPrefab);
				this.m_adventureDefs.Add(adventureDef.GetAdventureId(), adventureDef);
			}
		}
		List<WingDbfRecord> records = GameDbf.Wing.GetRecords();
		foreach (WingDbfRecord wingDbfRecord in records)
		{
			AdventureWingDef adventureWingDef = GameUtils.LoadGameObjectWithComponent<AdventureWingDef>(wingDbfRecord.AdventureWingDefPrefab);
			if (!(adventureWingDef == null))
			{
				adventureWingDef.Init(wingDbfRecord);
				this.m_wingDefs.Add(adventureWingDef.GetWingId(), adventureWingDef);
			}
		}
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x000A071C File Offset: 0x0009E91C
	private void OnKTQuoteLoaded(string name, GameObject go, object userData)
	{
		if (go != null)
		{
			Object.Destroy(go);
		}
		this.OnStartupAssetLoaded();
	}

	// Token: 0x060020D7 RID: 8407 RVA: 0x000A0738 File Offset: 0x0009E938
	private void UpdateAdventureModeMusic()
	{
		AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
		AdventureSubScenes currentSubScene = AdventureConfig.Get().GetCurrentSubScene();
		AdventureScene.AdventureModeMusic adventureModeMusic = null;
		foreach (AdventureScene.AdventureModeMusic adventureModeMusic2 in this.m_AdventureModeMusic)
		{
			if (adventureModeMusic2.m_subsceneId == currentSubScene && adventureModeMusic2.m_adventureId == selectedAdventure)
			{
				adventureModeMusic = adventureModeMusic2;
				break;
			}
			if (adventureModeMusic2.m_subsceneId == currentSubScene && adventureModeMusic2.m_adventureId == AdventureDbId.INVALID)
			{
				adventureModeMusic = adventureModeMusic2;
			}
		}
		if (adventureModeMusic != null)
		{
			MusicManager.Get().StartPlaylist(adventureModeMusic.m_playlist);
		}
	}

	// Token: 0x060020D8 RID: 8408 RVA: 0x000A07F4 File Offset: 0x0009E9F4
	private void OnStartupAssetLoaded()
	{
		this.m_StartupAssetLoads--;
		if (this.m_StartupAssetLoads > 0)
		{
			return;
		}
		this.UpdateAdventureModeMusic();
		SceneMgr.Get().NotifySceneLoaded();
	}

	// Token: 0x060020D9 RID: 8409 RVA: 0x000A0821 File Offset: 0x0009EA21
	private void LoadSubScene(AdventureSubScenes subscene)
	{
		this.LoadSubScene(subscene, new AssetLoader.GameObjectCallback(this.OnSubSceneLoaded));
	}

	// Token: 0x060020DA RID: 8410 RVA: 0x000A0838 File Offset: 0x0009EA38
	private void LoadSubScene(AdventureSubScenes subscene, AssetLoader.GameObjectCallback callback)
	{
		AdventureScene.AdventureSubSceneDef adventureSubSceneDef = this.m_SubSceneDefs.Find((AdventureScene.AdventureSubSceneDef item) => item.m_SubScene == subscene);
		if (adventureSubSceneDef == null)
		{
			Debug.LogError(string.Format("Subscene {0} prefab not defined in m_SubSceneDefs", subscene));
			return;
		}
		this.EnableTransitionBlocker(true);
		AssetLoader.GameObjectCallback runCallback = callback;
		AssetLoader.Get().LoadUIScreen(FileUtils.GameAssetPathToName(adventureSubSceneDef.m_Prefab), delegate(string name, GameObject go, object data)
		{
			if (runCallback != null)
			{
				runCallback(name, go, data);
			}
			this.UpdateAdventureModeMusic();
		}, null, false);
	}

	// Token: 0x060020DB RID: 8411 RVA: 0x000A08C9 File Offset: 0x0009EAC9
	private void OnSubSceneChange(AdventureSubScenes newscene, bool forward)
	{
		this.m_ReverseTransition = !forward;
		this.LoadSubScene(newscene);
	}

	// Token: 0x060020DC RID: 8412 RVA: 0x000A08DC File Offset: 0x0009EADC
	private Vector3 GetMoveDirection()
	{
		float num = 1f;
		if (this.m_TransitionDirection >= AdventureScene.TransitionDirection.NX)
		{
			num *= -1f;
		}
		Vector3 zero = Vector3.zero;
		zero[(int)(this.m_TransitionDirection % AdventureScene.TransitionDirection.NX)] = num;
		return zero;
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x000A091A File Offset: 0x0009EB1A
	private void OnFirstSubSceneLoaded(string name, GameObject screen, object callbackData)
	{
		this.ShowExpertAIUnlockTip();
		this.OnSubSceneLoaded(name, screen, callbackData);
		this.OnStartupAssetLoaded();
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x000A0934 File Offset: 0x0009EB34
	private void OnSubSceneLoaded(string name, GameObject screen, object callbackData)
	{
		this.m_TransitionOutSubScene = this.m_CurrentSubScene;
		this.m_CurrentSubScene = screen;
		this.m_CurrentSubScene.transform.position = new Vector3(-500f, 0f, 0f);
		Vector3 localScale = this.m_CurrentSubScene.transform.localScale;
		this.m_CurrentSubScene.transform.parent = base.transform;
		this.m_CurrentSubScene.transform.localScale = localScale;
		AdventureSubScene component = this.m_CurrentSubScene.GetComponent<AdventureSubScene>();
		this.m_SubScenesLoaded++;
		if (component == null)
		{
			this.DoSubSceneTransition(component);
		}
		else
		{
			base.StartCoroutine(this.WaitForSubSceneToLoad());
		}
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x000A09F0 File Offset: 0x0009EBF0
	private void DoSubSceneTransition(AdventureSubScene subscene)
	{
		this.m_CurrentSubScene.transform.localPosition = this.m_SubScenePosition;
		if (this.m_TransitionOutSubScene == null)
		{
			this.CompleteTransition();
			return;
		}
		float num = (!(subscene == null)) ? subscene.m_TransitionAnimationTime : this.m_DefaultTransitionAnimationTime;
		Vector3 moveDirection = this.GetMoveDirection();
		GameObject delobj = this.m_TransitionOutSubScene;
		if (this.m_ReverseTransition)
		{
			AdventureSubScene component = this.m_TransitionOutSubScene.GetComponent<AdventureSubScene>();
			Vector3 vector = (!(component == null)) ? component.m_SubSceneBounds : TransformUtil.GetBoundsOfChildren(this.m_TransitionOutSubScene).size;
			Vector3 localPosition = this.m_TransitionOutSubScene.transform.localPosition;
			localPosition.x -= vector.x * moveDirection.x;
			localPosition.y -= vector.y * moveDirection.y;
			localPosition.z -= vector.z * moveDirection.z;
			Hashtable args = iTween.Hash(new object[]
			{
				"islocal",
				true,
				"position",
				localPosition,
				"time",
				num,
				"easeType",
				this.m_TransitionEaseType,
				"oncomplete",
				delegate(object e)
				{
					this.DestroyTransitioningSubScene(delobj);
				},
				"oncompletetarget",
				base.gameObject
			});
			iTween.MoveTo(this.m_TransitionOutSubScene, args);
			if (!string.IsNullOrEmpty(this.m_SlideOutSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_SlideOutSound));
			}
			this.CompleteTransition();
		}
		else
		{
			AdventureSubScene component2 = this.m_CurrentSubScene.GetComponent<AdventureSubScene>();
			Vector3 vector2 = (!(component2 == null)) ? component2.m_SubSceneBounds : TransformUtil.GetBoundsOfChildren(this.m_CurrentSubScene).size;
			Vector3 localPosition2 = this.m_CurrentSubScene.transform.localPosition;
			Vector3 localPosition3 = this.m_CurrentSubScene.transform.localPosition;
			localPosition3.x -= vector2.x * moveDirection.x;
			localPosition3.y -= vector2.y * moveDirection.y;
			localPosition3.z -= vector2.z * moveDirection.z;
			this.m_CurrentSubScene.transform.localPosition = localPosition3;
			Hashtable args2 = iTween.Hash(new object[]
			{
				"islocal",
				true,
				"position",
				localPosition2,
				"time",
				num,
				"easeType",
				this.m_TransitionEaseType,
				"oncomplete",
				delegate(object e)
				{
					this.DestroyTransitioningSubScene(delobj);
					this.CompleteTransition();
				},
				"oncompletetarget",
				base.gameObject
			});
			iTween.MoveTo(this.m_CurrentSubScene, args2);
			if (!string.IsNullOrEmpty(this.m_SlideInSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_SlideInSound));
			}
		}
		this.m_TransitionOutSubScene = null;
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x000A0D69 File Offset: 0x0009EF69
	private void DestroyTransitioningSubScene(GameObject destroysubscene)
	{
		if (destroysubscene != null)
		{
			Object.DestroyObject(destroysubscene);
		}
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000A0D80 File Offset: 0x0009EF80
	private void CompleteTransition()
	{
		AdventureSubScene component = this.m_CurrentSubScene.GetComponent<AdventureSubScene>();
		if (component != null)
		{
			component.NotifyTransitionComplete();
			this.UpdateAdventureModeMusic();
		}
		this.EnableTransitionBlocker(false);
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000A0DB8 File Offset: 0x0009EFB8
	private IEnumerator WaitForSubSceneToLoad()
	{
		AdventureSubScene subscene = this.m_CurrentSubScene.GetComponent<AdventureSubScene>();
		while (!subscene.IsLoaded())
		{
			yield return null;
		}
		this.DoSubSceneTransition(subscene);
		yield break;
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x000A0DD4 File Offset: 0x0009EFD4
	private void OnSelectedModeChanged(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		if (AdventureConfig.Get().CanPlayMode(adventureId, modeId))
		{
			if (adventureId == AdventureDbId.NAXXRAMAS)
			{
				if (!Options.Get().GetBool(Option.HAS_SEEN_NAXX))
				{
					this.OnSelectedModeChanged_CreateKTIntroConversation();
				}
			}
			else if (adventureId == AdventureDbId.BRM)
			{
				if ((AdventureScene.Get() != null && AdventureScene.Get().IsDevMode) || !Options.Get().GetBool(Option.HAS_SEEN_BRM))
				{
					this.OnSelectedModeChanged_CreateIntroConversation(0, InitialConversationLines.BRM_INITIAL_CONVO_LINES, Option.HAS_SEEN_BRM);
				}
			}
			else if (adventureId == AdventureDbId.LOE && ((AdventureScene.Get() != null && AdventureScene.Get().IsDevMode) || !Options.Get().GetBool(Option.HAS_SEEN_LOE)))
			{
				this.OnSelectedModeChanged_CreateIntroConversation(0, InitialConversationLines.LOE_INITIAL_CONVO_LINES, Option.HAS_SEEN_LOE);
			}
		}
		this.UpdateAdventureModeMusic();
	}

	// Token: 0x060020E4 RID: 8420 RVA: 0x000A0EC3 File Offset: 0x0009F0C3
	private void OnSelectedModeChanged_CreateKTIntroConversation()
	{
		NotificationManager.Get().CreateKTQuote("VO_KT_INTRO_39", "VO_KT_INTRO_39", true);
		Options.Get().SetBool(Option.HAS_SEEN_NAXX, true);
	}

	// Token: 0x060020E5 RID: 8421 RVA: 0x000A0EEC File Offset: 0x0009F0EC
	private void OnSelectedModeChanged_CreateIntroConversation(int index, string[][] convoLines, Option hasSeen)
	{
		Action finishCallback = null;
		if (index < convoLines.Length - 1)
		{
			finishCallback = delegate()
			{
				if (SceneMgr.Get() == null || SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE)
				{
					return;
				}
				this.OnSelectedModeChanged_CreateIntroConversation(index + 1, convoLines, hasSeen);
			};
		}
		bool flag = AdventureScene.Get() != null && AdventureScene.Get().IsDevMode;
		if (index >= convoLines.Length - 1 && !flag)
		{
			Options.Get().SetBool(hasSeen, true);
		}
		string text = GameStrings.Get(convoLines[index][1]);
		bool allowRepeatDuringSession = flag;
		NotificationManager.Get().CreateCharacterQuote(convoLines[index][0], NotificationManager.DEFAULT_CHARACTER_POS, text, convoLines[index][1], allowRepeatDuringSession, 0f, finishCallback, CanvasAnchor.BOTTOM_LEFT);
	}

	// Token: 0x060020E6 RID: 8422 RVA: 0x000A0FE8 File Offset: 0x0009F1E8
	private void OnAdventureModeChanged(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		if (modeId == AdventureModeDbId.HEROIC)
		{
			this.ShowHeroicWarning();
		}
		if (adventureId == AdventureDbId.NAXXRAMAS && !Options.Get().GetBool(Option.HAS_ENTERED_NAXX))
		{
			NotificationManager.Get().CreateKTQuote("VO_KT_INTRO2_40", "VO_KT_INTRO2_40", true);
			Options.Get().SetBool(Option.HAS_ENTERED_NAXX, true);
		}
		this.UpdateAdventureModeMusic();
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x000A104C File Offset: 0x0009F24C
	private void ShowHeroicWarning()
	{
		if (Options.Get().GetBool(Option.HAS_SEEN_HEROIC_WARNING))
		{
			return;
		}
		Options.Get().SetBool(Option.HAS_SEEN_HEROIC_WARNING, true);
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_HEROIC_WARNING_TITLE");
		popupInfo.m_text = GameStrings.Get("GLUE_HEROIC_WARNING");
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x000A10C0 File Offset: 0x0009F2C0
	private void ShowExpertAIUnlockTip()
	{
		List<Achievement> achievesInGroup = AchieveManager.Get().GetAchievesInGroup(Achievement.AchType.UNLOCK_HERO, false);
		if (achievesInGroup.Count > 0)
		{
			return;
		}
		if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY && !Options.Get().GetBool(Option.HAS_SEEN_UNLOCK_ALL_HEROES_TRANSITION))
		{
			return;
		}
		if (!Options.Get().GetBool(Option.HAS_SEEN_EXPERT_AI_UNLOCK, false) && UserAttentionManager.CanShowAttentionGrabber("AdventureScene.ShowExpertAIUnlockTip"))
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_EXPERT_AI_10"), "VO_INNKEEPER_EXPERT_AI_10", 0f, null);
			Options.Get().SetBool(Option.HAS_SEEN_EXPERT_AI_UNLOCK, true);
		}
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x000A115C File Offset: 0x0009F35C
	private bool OnDevCheat(string func, string[] args, string rawArgs)
	{
		if (!ApplicationMgr.IsInternal())
		{
			return true;
		}
		this.IsDevMode = true;
		if (args.Length > 0)
		{
			int num = 1;
			if (int.TryParse(args[0], ref num) && num > 0)
			{
				this.IsDevMode = true;
				this.DevModeSetting = num;
			}
		}
		if (UIStatus.Get() != null)
		{
			UIStatus.Get().AddInfo(string.Format("{0}: IsDevMode={1} DevModeSetting={2}", func, this.IsDevMode, this.DevModeSetting));
		}
		return true;
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x000A11E7 File Offset: 0x0009F3E7
	private void EnableTransitionBlocker(bool block)
	{
		if (this.m_transitionClickBlocker != null)
		{
			this.m_transitionClickBlocker.SetActive(block);
		}
	}

	// Token: 0x040011F7 RID: 4599
	private const AdventureSubScenes s_StartMode = AdventureSubScenes.Chooser;

	// Token: 0x040011F8 RID: 4600
	private static AdventureScene s_instance;

	// Token: 0x040011F9 RID: 4601
	[CustomEditField(Sections = "Transition Blocker")]
	public GameObject m_transitionClickBlocker;

	// Token: 0x040011FA RID: 4602
	[CustomEditField(Sections = "Transition Motions")]
	public Vector3 m_SubScenePosition = Vector3.zero;

	// Token: 0x040011FB RID: 4603
	[CustomEditField(Sections = "Transition Motions")]
	public float m_DefaultTransitionAnimationTime = 1f;

	// Token: 0x040011FC RID: 4604
	[CustomEditField(Sections = "Transition Motions")]
	public iTween.EaseType m_TransitionEaseType = iTween.EaseType.easeInOutSine;

	// Token: 0x040011FD RID: 4605
	[CustomEditField(Sections = "Transition Motions")]
	public AdventureScene.TransitionDirection m_TransitionDirection;

	// Token: 0x040011FE RID: 4606
	[CustomEditField(Sections = "Transition Sounds", T = EditType.SOUND_PREFAB)]
	public string m_SlideInSound;

	// Token: 0x040011FF RID: 4607
	[CustomEditField(Sections = "Transition Sounds", T = EditType.SOUND_PREFAB)]
	public string m_SlideOutSound;

	// Token: 0x04001200 RID: 4608
	[CustomEditField(Sections = "Adventure Subscene Prefabs")]
	public List<AdventureScene.AdventureSubSceneDef> m_SubSceneDefs = new List<AdventureScene.AdventureSubSceneDef>();

	// Token: 0x04001201 RID: 4609
	[CustomEditField(Sections = "Music Settings")]
	public List<AdventureScene.AdventureModeMusic> m_AdventureModeMusic = new List<AdventureScene.AdventureModeMusic>();

	// Token: 0x04001202 RID: 4610
	public static readonly Vector3 REWARD_LOCAL_POS = new Vector3(0.1438589f, 31.27692f, 12.97332f);

	// Token: 0x04001203 RID: 4611
	public static PlatformDependentValue<Vector3> REWARD_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(10f, 10f, 10f),
		Phone = new Vector3(7f, 7f, 7f)
	};

	// Token: 0x04001204 RID: 4612
	public static PlatformDependentValue<Vector3> REWARD_PUNCH_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(10.2f, 10.2f, 10.2f),
		Phone = new Vector3(7.1f, 7.1f, 7.1f)
	};

	// Token: 0x04001205 RID: 4613
	private GameObject m_TransitionOutSubScene;

	// Token: 0x04001206 RID: 4614
	private GameObject m_CurrentSubScene;

	// Token: 0x04001207 RID: 4615
	private bool m_ReverseTransition;

	// Token: 0x04001208 RID: 4616
	private int m_StartupAssetLoads;

	// Token: 0x04001209 RID: 4617
	private int m_SubScenesLoaded;

	// Token: 0x0400120A RID: 4618
	private bool m_MusicStopped;

	// Token: 0x0400120B RID: 4619
	private bool m_Unloading;

	// Token: 0x0400120C RID: 4620
	private Map<AdventureDbId, AdventureDef> m_adventureDefs = new Map<AdventureDbId, AdventureDef>();

	// Token: 0x0400120D RID: 4621
	private Map<WingDbId, AdventureWingDef> m_wingDefs = new Map<WingDbId, AdventureWingDef>();

	// Token: 0x0200022C RID: 556
	public enum TransitionDirection
	{
		// Token: 0x04001272 RID: 4722
		X,
		// Token: 0x04001273 RID: 4723
		Y,
		// Token: 0x04001274 RID: 4724
		Z,
		// Token: 0x04001275 RID: 4725
		NX,
		// Token: 0x04001276 RID: 4726
		NY,
		// Token: 0x04001277 RID: 4727
		NZ
	}

	// Token: 0x0200022D RID: 557
	[Serializable]
	public class AdventureModeMusic
	{
		// Token: 0x04001278 RID: 4728
		public AdventureSubScenes m_subsceneId;

		// Token: 0x04001279 RID: 4729
		public AdventureDbId m_adventureId;

		// Token: 0x0400127A RID: 4730
		public MusicPlaylistType m_playlist;
	}

	// Token: 0x0200022E RID: 558
	[Serializable]
	public class AdventureSubSceneDef
	{
		// Token: 0x0400127B RID: 4731
		[CustomEditField(ListSortable = true)]
		public AdventureSubScenes m_SubScene;

		// Token: 0x0400127C RID: 4732
		[CustomEditField(T = EditType.GAME_OBJECT)]
		public String_MobileOverride m_Prefab;
	}
}
