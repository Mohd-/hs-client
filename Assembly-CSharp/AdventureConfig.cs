using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000211 RID: 529
[CustomEditClass]
public class AdventureConfig : MonoBehaviour
{
	// Token: 0x0600205F RID: 8287 RVA: 0x0009EAD1 File Offset: 0x0009CCD1
	public static AdventureConfig Get()
	{
		return AdventureConfig.s_instance;
	}

	// Token: 0x06002060 RID: 8288 RVA: 0x0009EAD8 File Offset: 0x0009CCD8
	public static AdventureSubScenes GetSubSceneFromMode(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		AdventureSubScenes result = AdventureSubScenes.Chooser;
		AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int)adventureId, (int)modeId);
		string subscenePrefab = adventureDataRecord.SubscenePrefab;
		string text = subscenePrefab;
		if (text != null)
		{
			if (AdventureConfig.<>f__switch$map7E == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("Assets/Game/UIScreens/AdventurePractice", 0);
				dictionary.Add("Assets/Game/UIScreens/AdventureMissionDisplay", 1);
				dictionary.Add("Assets/Game/UIScreens/AdventureClassChallenge", 2);
				AdventureConfig.<>f__switch$map7E = dictionary;
			}
			int num;
			if (AdventureConfig.<>f__switch$map7E.TryGetValue(text, ref num))
			{
				switch (num)
				{
				case 0:
					return AdventureSubScenes.Practice;
				case 1:
					return AdventureSubScenes.MissionDisplay;
				case 2:
					return AdventureSubScenes.ClassChallenge;
				}
			}
		}
		Debug.LogError(string.Format("Adventure sub scene asset not defined for {0}.{1}.", adventureId, modeId));
		return result;
	}

	// Token: 0x06002061 RID: 8289 RVA: 0x0009EBA7 File Offset: 0x0009CDA7
	public AdventureDbId GetSelectedAdventure()
	{
		return this.m_SelectedAdventure;
	}

	// Token: 0x06002062 RID: 8290 RVA: 0x0009EBAF File Offset: 0x0009CDAF
	public AdventureModeDbId GetSelectedMode()
	{
		return this.m_SelectedMode;
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x0009EBB8 File Offset: 0x0009CDB8
	public AdventureModeDbId GetClientChooserAdventureMode(AdventureDbId adventureDbId)
	{
		AdventureModeDbId result;
		if (this.m_ClientChooserAdventureModes.TryGetValue(adventureDbId, out result))
		{
			return result;
		}
		return (this.m_SelectedAdventure != adventureDbId) ? AdventureModeDbId.NORMAL : this.m_SelectedMode;
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x0009EBF4 File Offset: 0x0009CDF4
	public bool CanPlayMode(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		bool flag = AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES);
		if (adventureId == AdventureDbId.PRACTICE)
		{
			return modeId != AdventureModeDbId.EXPERT || flag;
		}
		return flag && (modeId == AdventureModeDbId.NORMAL || GameDbf.Scenario.GetRecord((ScenarioDbfRecord r) => r.AdventureId == (int)adventureId && r.ModeId == (int)modeId && AdventureProgressMgr.Get().CanPlayScenario(r.ID)) != null);
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x0009EC70 File Offset: 0x0009CE70
	public bool IsFeaturedMode(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		return this.CanPlayMode(adventureId, modeId) && (adventureId == AdventureDbId.NAXXRAMAS && modeId == AdventureModeDbId.CLASS_CHALLENGE) && !Options.Get().GetBool(Option.HAS_SEEN_NAXX_CLASS_CHALLENGE, false);
	}

	// Token: 0x06002066 RID: 8294 RVA: 0x0009ECAF File Offset: 0x0009CEAF
	public bool MarkFeaturedMode(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		if (!this.CanPlayMode(adventureId, modeId))
		{
			return false;
		}
		if (adventureId == AdventureDbId.NAXXRAMAS && modeId == AdventureModeDbId.CLASS_CHALLENGE)
		{
			Options.Get().SetBool(Option.HAS_SEEN_NAXX_CLASS_CHALLENGE, true);
			return true;
		}
		return false;
	}

	// Token: 0x06002067 RID: 8295 RVA: 0x0009ECE4 File Offset: 0x0009CEE4
	public string GetSelectedAdventureAndModeString()
	{
		return string.Format("{0}_{1}", this.m_SelectedAdventure, this.m_SelectedMode);
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x0009ED14 File Offset: 0x0009CF14
	public void SetSelectedAdventureMode(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		this.m_SelectedAdventure = adventureId;
		this.m_SelectedMode = modeId;
		this.m_ClientChooserAdventureModes[adventureId] = modeId;
		Options.Get().SetEnum<AdventureDbId>(Option.SELECTED_ADVENTURE, this.m_SelectedAdventure);
		Options.Get().SetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, this.m_SelectedMode);
		this.FireSelectedModeChangeEvent();
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x0009ED66 File Offset: 0x0009CF66
	public AdventureSubScenes GetCurrentSubScene()
	{
		return this.m_CurrentSubScene;
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x0009ED6E File Offset: 0x0009CF6E
	public ScenarioDbId GetMission()
	{
		return this.m_StartMission;
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x0009ED78 File Offset: 0x0009CF78
	public ScenarioDbId GetLastSelectedMission()
	{
		string selectedAdventureAndModeString = this.GetSelectedAdventureAndModeString();
		ScenarioDbId result = ScenarioDbId.INVALID;
		this.m_LastSelectedMissions.TryGetValue(selectedAdventureAndModeString, out result);
		return result;
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x0009EDA0 File Offset: 0x0009CFA0
	public bool IsScenarioDefeatedAndInitCache(ScenarioDbId mission)
	{
		bool flag = AdventureProgressMgr.Get().HasDefeatedScenario((int)mission);
		if (!this.m_CachedDefeatedScenario.ContainsKey(mission))
		{
			this.m_CachedDefeatedScenario[mission] = flag;
		}
		return flag;
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x0009EDD8 File Offset: 0x0009CFD8
	public bool IsScenarioJustDefeated(ScenarioDbId mission)
	{
		bool flag = AdventureProgressMgr.Get().HasDefeatedScenario((int)mission);
		bool flag2 = false;
		this.m_CachedDefeatedScenario.TryGetValue(mission, out flag2);
		this.m_CachedDefeatedScenario[mission] = flag;
		return flag != flag2;
	}

	// Token: 0x0600206E RID: 8302 RVA: 0x0009EE18 File Offset: 0x0009D018
	public AdventureBossDef GetBossDef(ScenarioDbId mission)
	{
		AdventureBossDef result = null;
		if (!this.m_CachedBossDef.TryGetValue(mission, out result))
		{
			Debug.LogError(string.Format("Boss def for mission not loaded: {0}\nCall LoadBossDef first.", mission));
		}
		return result;
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x0009EE50 File Offset: 0x0009D050
	public void LoadBossDef(ScenarioDbId mission, AdventureConfig.DelBossDefLoaded callback)
	{
		AdventureBossDef bossDef = null;
		if (this.m_CachedBossDef.TryGetValue(mission, out bossDef))
		{
			callback(bossDef, true);
			return;
		}
		string bossDefAssetPath = this.GetBossDefAssetPath(mission);
		if (string.IsNullOrEmpty(bossDefAssetPath))
		{
			if (callback != null)
			{
				callback(null, false);
			}
			return;
		}
		AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(bossDefAssetPath), delegate(string name, GameObject go, object data)
		{
			if (go == null)
			{
				Debug.LogError(string.Format("Unable to instantiate boss def: {0}", name));
				if (callback != null)
				{
					callback(null, false);
				}
				return;
			}
			AdventureBossDef component = go.GetComponent<AdventureBossDef>();
			if (component == null)
			{
				Debug.LogError(string.Format("Object does not contain AdventureBossDef component: {0}", name));
			}
			else
			{
				this.m_CachedBossDef[mission] = component;
			}
			if (callback != null)
			{
				callback(component, component != null);
			}
		}, null, false);
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x0009EEF0 File Offset: 0x0009D0F0
	public string GetBossDefAssetPath(ScenarioDbId mission)
	{
		AdventureMissionDbfRecord record = GameDbf.AdventureMission.GetRecord((AdventureMissionDbfRecord r) => r.ScenarioId == (int)mission);
		if (record == null)
		{
			return null;
		}
		return FileUtils.GameAssetPathToName(record.BossDefAssetPath);
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x0009EF34 File Offset: 0x0009D134
	public void ClearBossDefs()
	{
		foreach (KeyValuePair<ScenarioDbId, AdventureBossDef> keyValuePair in this.m_CachedBossDef)
		{
			Object.Destroy(keyValuePair.Value);
		}
		this.m_CachedBossDef.Clear();
	}

	// Token: 0x06002072 RID: 8306 RVA: 0x0009EFA0 File Offset: 0x0009D1A0
	public void SetMission(ScenarioDbId mission, bool showDetails = true)
	{
		this.m_StartMission = mission;
		string selectedAdventureAndModeString = this.GetSelectedAdventureAndModeString();
		this.m_LastSelectedMissions[selectedAdventureAndModeString] = mission;
		AdventureConfig.AdventureMissionSet[] array = this.m_AdventureMissionSetEventList.ToArray();
		foreach (AdventureConfig.AdventureMissionSet adventureMissionSet in array)
		{
			adventureMissionSet(mission, showDetails);
		}
	}

	// Token: 0x06002073 RID: 8307 RVA: 0x0009EFFB File Offset: 0x0009D1FB
	public bool DoesSelectedMissionRequireDeck()
	{
		return this.DoesMissionRequireDeck(this.m_StartMission);
	}

	// Token: 0x06002074 RID: 8308 RVA: 0x0009F00C File Offset: 0x0009D20C
	public bool DoesMissionRequireDeck(ScenarioDbId scenario)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((int)scenario);
		return record == null || record.Player1DeckId == 0;
	}

	// Token: 0x06002075 RID: 8309 RVA: 0x0009F036 File Offset: 0x0009D236
	public void AddAdventureMissionSetListener(AdventureConfig.AdventureMissionSet dlg)
	{
		this.m_AdventureMissionSetEventList.Add(dlg);
	}

	// Token: 0x06002076 RID: 8310 RVA: 0x0009F044 File Offset: 0x0009D244
	public void RemoveAdventureMissionSetListener(AdventureConfig.AdventureMissionSet dlg)
	{
		this.m_AdventureMissionSetEventList.Remove(dlg);
	}

	// Token: 0x06002077 RID: 8311 RVA: 0x0009F053 File Offset: 0x0009D253
	public void AddAdventureModeChangeListener(AdventureConfig.AdventureModeChange dlg)
	{
		this.m_AdventureModeChangeEventList.Add(dlg);
	}

	// Token: 0x06002078 RID: 8312 RVA: 0x0009F061 File Offset: 0x0009D261
	public void RemoveAdventureModeChangeListener(AdventureConfig.AdventureModeChange dlg)
	{
		this.m_AdventureModeChangeEventList.Remove(dlg);
	}

	// Token: 0x06002079 RID: 8313 RVA: 0x0009F070 File Offset: 0x0009D270
	public void AddSubSceneChangeListener(AdventureConfig.SubSceneChange dlg)
	{
		this.m_SubSceneChangeEventList.Add(dlg);
	}

	// Token: 0x0600207A RID: 8314 RVA: 0x0009F07E File Offset: 0x0009D27E
	public void RemoveSubSceneChangeListener(AdventureConfig.SubSceneChange dlg)
	{
		this.m_SubSceneChangeEventList.Remove(dlg);
	}

	// Token: 0x0600207B RID: 8315 RVA: 0x0009F08D File Offset: 0x0009D28D
	public void AddSelectedModeChangeListener(AdventureConfig.SelectedModeChange dlg)
	{
		this.m_SelectedModeChangeEventList.Add(dlg);
	}

	// Token: 0x0600207C RID: 8316 RVA: 0x0009F09B File Offset: 0x0009D29B
	public void RemoveSelectedModeChangeListener(AdventureConfig.SelectedModeChange dlg)
	{
		this.m_SelectedModeChangeEventList.Remove(dlg);
	}

	// Token: 0x0600207D RID: 8317 RVA: 0x0009F0AA File Offset: 0x0009D2AA
	public void ResetSubScene(AdventureSubScenes subscene)
	{
		this.m_CurrentSubScene = subscene;
		this.m_LastSubScenes.Clear();
	}

	// Token: 0x0600207E RID: 8318 RVA: 0x0009F0C0 File Offset: 0x0009D2C0
	public void ChangeSubScene(AdventureSubScenes subscene)
	{
		if (subscene == this.m_CurrentSubScene)
		{
			Debug.Log(string.Format("Sub scene {0} is already set.", subscene));
			return;
		}
		this.m_LastSubScenes.Push(this.m_CurrentSubScene);
		this.m_CurrentSubScene = subscene;
		this.FireSubSceneChangeEvent(true);
		this.FireAdventureModeChangeEvent();
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x0009F114 File Offset: 0x0009D314
	public void ChangeToLastSubScene(bool fireevent = true)
	{
		if (this.m_LastSubScenes.Count == 0)
		{
			Debug.Log("No last sub scenes were loaded.");
			return;
		}
		this.m_CurrentSubScene = this.m_LastSubScenes.Pop();
		if (fireevent)
		{
			this.FireSubSceneChangeEvent(false);
		}
		this.FireAdventureModeChangeEvent();
	}

	// Token: 0x06002080 RID: 8320 RVA: 0x0009F160 File Offset: 0x0009D360
	public void ChangeSubScene(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		this.ChangeSubScene(AdventureConfig.GetSubSceneFromMode(adventureId, modeId));
	}

	// Token: 0x06002081 RID: 8321 RVA: 0x0009F16F File Offset: 0x0009D36F
	public void ChangeSubSceneToSelectedAdventure()
	{
		this.ChangeSubScene(AdventureConfig.GetSubSceneFromMode(this.m_SelectedAdventure, this.m_SelectedMode));
	}

	// Token: 0x06002082 RID: 8322 RVA: 0x0009F188 File Offset: 0x0009D388
	public bool IsMissionAvailable(int missionId)
	{
		bool flag = AdventureProgressMgr.Get().CanPlayScenario(missionId);
		if (!flag)
		{
			return false;
		}
		int num = 0;
		int wing = 0;
		if (!this.GetMissionPlayableParameters(missionId, ref num, ref wing))
		{
			return false;
		}
		int num2 = 0;
		AdventureProgressMgr.Get().GetWingAck(wing, out num2);
		return flag && num <= num2;
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x0009F1E0 File Offset: 0x0009D3E0
	public bool IsMissionNewlyAvailableAndGetReqs(int missionId, ref int wingId, ref int missionReqProgress)
	{
		if (!this.GetMissionPlayableParameters(missionId, ref missionReqProgress, ref wingId))
		{
			return false;
		}
		bool flag = AdventureProgressMgr.Get().CanPlayScenario(missionId);
		int num = 0;
		AdventureProgressMgr.Get().GetWingAck(wingId, out num);
		return num < missionReqProgress && flag;
	}

	// Token: 0x06002084 RID: 8324 RVA: 0x0009F22C File Offset: 0x0009D42C
	public void SetWingAckIfGreater(int wingId, int ackProgress)
	{
		int num = 0;
		AdventureProgressMgr.Get().GetWingAck(wingId, out num);
		if (ackProgress > num)
		{
			AdventureProgressMgr.Get().SetWingAck(wingId, ackProgress);
		}
	}

	// Token: 0x06002085 RID: 8325 RVA: 0x0009F260 File Offset: 0x0009D460
	private bool GetMissionPlayableParameters(int missionId, ref int missionReqProgress, ref int wingId)
	{
		ScenarioDbfRecord scenarioRecord = GameDbf.Scenario.GetRecord(missionId);
		if (scenarioRecord == null)
		{
			return false;
		}
		WingDbfRecord record = GameDbf.Wing.GetRecord(scenarioRecord.WingId);
		if (record == null)
		{
			return false;
		}
		AdventureMissionDbfRecord record2 = GameDbf.AdventureMission.GetRecord((AdventureMissionDbfRecord r) => r.ScenarioId == scenarioRecord.ID);
		if (record2 == null)
		{
			return false;
		}
		missionReqProgress = record2.ReqProgress;
		wingId = record.ID;
		return true;
	}

	// Token: 0x06002086 RID: 8326 RVA: 0x0009F2E0 File Offset: 0x0009D4E0
	public int GetWingBossesDefeated(AdventureDbId advId, AdventureModeDbId mode, WingDbId wing, int defaultvalue = 0)
	{
		int result = 0;
		if (this.m_WingBossesDefeatedCache.TryGetValue(this.GetWingUniqueId(advId, mode, wing), out result))
		{
			return result;
		}
		return defaultvalue;
	}

	// Token: 0x06002087 RID: 8327 RVA: 0x0009F30E File Offset: 0x0009D50E
	public void UpdateWingBossesDefeated(AdventureDbId advId, AdventureModeDbId mode, WingDbId wing, int bossesDefeated)
	{
		this.m_WingBossesDefeatedCache[this.GetWingUniqueId(advId, mode, wing)] = bossesDefeated;
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x0009F326 File Offset: 0x0009D526
	private string GetWingUniqueId(AdventureDbId advId, AdventureModeDbId modeId, WingDbId wing)
	{
		return string.Format("{0}_{1}_{2}", advId, modeId, wing);
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x0009F344 File Offset: 0x0009D544
	private void Awake()
	{
		AdventureConfig.s_instance = this;
	}

	// Token: 0x0600208A RID: 8330 RVA: 0x0009F34C File Offset: 0x0009D54C
	private void OnDestroy()
	{
		AdventureConfig.s_instance = null;
	}

	// Token: 0x0600208B RID: 8331 RVA: 0x0009F354 File Offset: 0x0009D554
	public void OnAdventureSceneAwake()
	{
		this.m_SelectedAdventure = Options.Get().GetEnum<AdventureDbId>(Option.SELECTED_ADVENTURE, AdventureDbId.PRACTICE);
		this.m_SelectedMode = Options.Get().GetEnum<AdventureModeDbId>(Option.SELECTED_ADVENTURE_MODE, AdventureModeDbId.NORMAL);
	}

	// Token: 0x0600208C RID: 8332 RVA: 0x0009F387 File Offset: 0x0009D587
	public void OnAdventureSceneUnload()
	{
		this.m_SelectedAdventure = AdventureDbId.INVALID;
		this.m_SelectedMode = AdventureModeDbId.INVALID;
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x0009F397 File Offset: 0x0009D597
	public void ResetSubScene()
	{
		this.m_CurrentSubScene = AdventureSubScenes.Chooser;
		this.m_LastSubScenes.Clear();
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x0009F3AC File Offset: 0x0009D5AC
	private void FireAdventureModeChangeEvent()
	{
		AdventureConfig.AdventureModeChange[] array = this.m_AdventureModeChangeEventList.ToArray();
		foreach (AdventureConfig.AdventureModeChange adventureModeChange in array)
		{
			adventureModeChange(this.m_SelectedAdventure, this.m_SelectedMode);
		}
	}

	// Token: 0x0600208F RID: 8335 RVA: 0x0009F3F4 File Offset: 0x0009D5F4
	private void FireSubSceneChangeEvent(bool forward)
	{
		this.UpdatePresence();
		AdventureConfig.SubSceneChange[] array = this.m_SubSceneChangeEventList.ToArray();
		foreach (AdventureConfig.SubSceneChange subSceneChange in array)
		{
			subSceneChange(this.m_CurrentSubScene, forward);
		}
	}

	// Token: 0x06002090 RID: 8336 RVA: 0x0009F43C File Offset: 0x0009D63C
	private void FireSelectedModeChangeEvent()
	{
		AdventureConfig.SelectedModeChange[] array = this.m_SelectedModeChangeEventList.ToArray();
		foreach (AdventureConfig.SelectedModeChange selectedModeChange in array)
		{
			selectedModeChange(this.m_SelectedAdventure, this.m_SelectedMode);
		}
	}

	// Token: 0x06002091 RID: 8337 RVA: 0x0009F484 File Offset: 0x0009D684
	public void UpdatePresence()
	{
		switch (this.m_CurrentSubScene)
		{
		case AdventureSubScenes.MissionDeckPicker:
		case AdventureSubScenes.MissionDisplay:
		case AdventureSubScenes.ClassChallenge:
			PresenceMgr.Get().SetStatus_EnteringAdventure(this.m_SelectedAdventure, this.m_SelectedMode);
			return;
		default:
			if (AdventureScene.Get() != null && !AdventureScene.Get().IsUnloading())
			{
				PresenceMgr.Get().SetStatus(new Enum[]
				{
					PresenceStatus.ADVENTURE_CHOOSING_MODE
				});
			}
			return;
		}
	}

	// Token: 0x040011BF RID: 4543
	private static AdventureConfig s_instance;

	// Token: 0x040011C0 RID: 4544
	private AdventureDbId m_SelectedAdventure = AdventureDbId.PRACTICE;

	// Token: 0x040011C1 RID: 4545
	private AdventureModeDbId m_SelectedMode = AdventureModeDbId.NORMAL;

	// Token: 0x040011C2 RID: 4546
	private Stack<AdventureSubScenes> m_LastSubScenes = new Stack<AdventureSubScenes>();

	// Token: 0x040011C3 RID: 4547
	private AdventureSubScenes m_CurrentSubScene;

	// Token: 0x040011C4 RID: 4548
	private ScenarioDbId m_StartMission;

	// Token: 0x040011C5 RID: 4549
	private List<AdventureConfig.AdventureModeChange> m_AdventureModeChangeEventList = new List<AdventureConfig.AdventureModeChange>();

	// Token: 0x040011C6 RID: 4550
	private List<AdventureConfig.SubSceneChange> m_SubSceneChangeEventList = new List<AdventureConfig.SubSceneChange>();

	// Token: 0x040011C7 RID: 4551
	private List<AdventureConfig.SelectedModeChange> m_SelectedModeChangeEventList = new List<AdventureConfig.SelectedModeChange>();

	// Token: 0x040011C8 RID: 4552
	private List<AdventureConfig.AdventureMissionSet> m_AdventureMissionSetEventList = new List<AdventureConfig.AdventureMissionSet>();

	// Token: 0x040011C9 RID: 4553
	private Map<string, int> m_WingBossesDefeatedCache = new Map<string, int>();

	// Token: 0x040011CA RID: 4554
	private Map<string, ScenarioDbId> m_LastSelectedMissions = new Map<string, ScenarioDbId>();

	// Token: 0x040011CB RID: 4555
	private Map<ScenarioDbId, bool> m_CachedDefeatedScenario = new Map<ScenarioDbId, bool>();

	// Token: 0x040011CC RID: 4556
	private Map<ScenarioDbId, AdventureBossDef> m_CachedBossDef = new Map<ScenarioDbId, AdventureBossDef>();

	// Token: 0x040011CD RID: 4557
	private Map<AdventureDbId, AdventureModeDbId> m_ClientChooserAdventureModes = new Map<AdventureDbId, AdventureModeDbId>();

	// Token: 0x02000217 RID: 535
	// (Invoke) Token: 0x060020BF RID: 8383
	public delegate void SelectedModeChange(AdventureDbId adventureId, AdventureModeDbId modeId);

	// Token: 0x0200021C RID: 540
	// (Invoke) Token: 0x060020F9 RID: 8441
	public delegate void DelBossDefLoaded(AdventureBossDef bossDef, bool success);

	// Token: 0x0200021D RID: 541
	// (Invoke) Token: 0x060020FD RID: 8445
	public delegate void AdventureMissionSet(ScenarioDbId mission, bool showDetails);

	// Token: 0x0200021E RID: 542
	// (Invoke) Token: 0x06002101 RID: 8449
	public delegate void SubSceneChange(AdventureSubScenes newscene, bool forward);

	// Token: 0x02000220 RID: 544
	// (Invoke) Token: 0x06002105 RID: 8453
	public delegate void AdventureModeChange(AdventureDbId adventureId, AdventureModeDbId modeId);
}
