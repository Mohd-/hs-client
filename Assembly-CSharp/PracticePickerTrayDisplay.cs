using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200039D RID: 925
[CustomEditClass]
public class PracticePickerTrayDisplay : MonoBehaviour
{
	// Token: 0x170003BC RID: 956
	// (get) Token: 0x0600307A RID: 12410 RVA: 0x000F3A59 File Offset: 0x000F1C59
	// (set) Token: 0x0600307B RID: 12411 RVA: 0x000F3A61 File Offset: 0x000F1C61
	[CustomEditField(Sections = "AI Button Settings")]
	public float AIButtonHeight
	{
		get
		{
			return this.m_AIButtonHeight;
		}
		set
		{
			this.m_AIButtonHeight = value;
			this.UpdateAIButtonPositions();
		}
	}

	// Token: 0x0600307C RID: 12412 RVA: 0x000F3A70 File Offset: 0x000F1C70
	private void Awake()
	{
		PracticePickerTrayDisplay.s_instance = this;
		this.InitMissionRecords();
		Transform[] components = base.gameObject.GetComponents<Transform>();
		foreach (Transform transform in components)
		{
			transform.gameObject.SetActive(false);
		}
		base.gameObject.SetActive(true);
		if (this.m_backButton != null)
		{
			this.m_backButton.SetText(GameStrings.Get("GLOBAL_BACK"));
			this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonReleased));
		}
		this.m_trayLabel.Text = GameStrings.Get("GLUE_CHOOSE_OPPONENT");
		this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayGameButtonRelease));
		this.m_heroDefsToLoad = this.m_sortedMissionRecords.Count;
		foreach (DbfRecord dbfRecord in this.m_sortedMissionRecords)
		{
			string missionHeroCardId = GameUtils.GetMissionHeroCardId(dbfRecord.ID);
			DefLoader.Get().LoadFullDef(missionHeroCardId, new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
		}
		SoundManager.Get().Load("choose_opponent_panel_slide_on");
		SoundManager.Get().Load("choose_opponent_panel_slide_off");
		this.SetupHeroAchieves();
		base.StartCoroutine(this.NotifyWhenTrayLoaded());
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
	}

	// Token: 0x0600307D RID: 12413 RVA: 0x000F3C00 File Offset: 0x000F1E00
	private void OnDestroy()
	{
		GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		PracticePickerTrayDisplay.s_instance = null;
	}

	// Token: 0x0600307E RID: 12414 RVA: 0x000F3C1F File Offset: 0x000F1E1F
	private void Start()
	{
		this.m_playButton.SetText(GameStrings.Get("GLOBAL_PLAY"));
		this.m_playButton.SetOriginalLocalPosition();
		this.m_playButton.Disable();
	}

	// Token: 0x0600307F RID: 12415 RVA: 0x000F3C4C File Offset: 0x000F1E4C
	public static PracticePickerTrayDisplay Get()
	{
		return PracticePickerTrayDisplay.s_instance;
	}

	// Token: 0x06003080 RID: 12416 RVA: 0x000F3C54 File Offset: 0x000F1E54
	public void Init()
	{
		int num = Mathf.Min(PracticePickerTrayDisplay.NUM_AI_BUTTONS_TO_SHOW, this.m_sortedMissionRecords.Count);
		for (int i = 0; i < num; i++)
		{
			PracticeAIButton practiceAIButton = (PracticeAIButton)GameUtils.Instantiate(this.m_AIButtonPrefab, this.m_AIButtonsContainer, false);
			SceneUtils.SetLayer(practiceAIButton, this.m_AIButtonsContainer.gameObject.layer);
			this.m_practiceAIButtons.Add(practiceAIButton);
		}
		this.UpdateAIButtonPositions();
		foreach (PracticeAIButton practiceAIButton2 in this.m_practiceAIButtons)
		{
			practiceAIButton2.SetOriginalLocalPosition();
			practiceAIButton2.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.AIButtonPressed));
		}
		this.m_buttonsCreated = true;
	}

	// Token: 0x06003081 RID: 12417 RVA: 0x000F3D34 File Offset: 0x000F1F34
	public void Show()
	{
		this.m_shown = true;
		iTween.Stop(base.gameObject);
		Transform[] components = base.gameObject.GetComponents<Transform>();
		foreach (Transform transform in components)
		{
			transform.gameObject.SetActive(true);
		}
		base.gameObject.SetActive(true);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			PracticeDisplay.Get().GetPracticePickerShowPosition(),
			"isLocal",
			true,
			"time",
			this.m_trayAnimationTime,
			"easetype",
			this.m_trayInEaseType,
			"delay",
			0.001f
		});
		iTween.MoveTo(base.gameObject, args);
		SoundManager.Get().LoadAndPlay("choose_opponent_panel_slide_on");
		if (!Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_TRAY, false) && UserAttentionManager.CanShowAttentionGrabber("PracticePickerTrayDisplay.Show:" + Option.HAS_SEEN_PRACTICE_TRAY))
		{
			Options.Get().SetBool(Option.HAS_SEEN_PRACTICE_TRAY, true);
			base.StartCoroutine(this.DoPickHeroLines());
		}
		if (this.m_selectedPracticeAIButton != null)
		{
			this.m_playButton.Enable();
		}
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x06003082 RID: 12418 RVA: 0x000F3E9C File Offset: 0x000F209C
	private IEnumerator DoPickHeroLines()
	{
		Notification firstPart = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_PRACTICE_INST1_07"), "VO_INNKEEPER_UNLOCK_HEROES", 0f, null);
		while (firstPart.GetAudio() == null)
		{
			yield return null;
		}
		yield return new WaitForSeconds(firstPart.GetAudio().clip.length);
		yield return new WaitForSeconds(6f);
		if (this.m_playButton.IsEnabled() || GameMgr.Get().IsTransitionPopupShown())
		{
			yield break;
		}
		NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_PRACTICE_INST2_08"), "VO_INNKEEPER_PRACTICE_INST2_08", 2f, null);
		yield break;
	}

	// Token: 0x06003083 RID: 12419 RVA: 0x000F3EB8 File Offset: 0x000F20B8
	public void Hide()
	{
		this.m_shown = false;
		iTween.Stop(base.gameObject);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			PracticeDisplay.Get().GetPracticePickerHidePosition(),
			"isLocal",
			true,
			"time",
			this.m_trayAnimationTime,
			"easetype",
			this.m_trayOutEaseType,
			"oncomplete",
			delegate(object e)
			{
				base.gameObject.SetActive(false);
			},
			"delay",
			0.001f
		});
		iTween.MoveTo(base.gameObject, args);
		SoundManager.Get().LoadAndPlay("choose_opponent_panel_slide_off");
	}

	// Token: 0x06003084 RID: 12420 RVA: 0x000F3F85 File Offset: 0x000F2185
	public void OnGameDenied()
	{
		this.UpdateAIButtons();
	}

	// Token: 0x06003085 RID: 12421 RVA: 0x000F3F8D File Offset: 0x000F218D
	public bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x06003086 RID: 12422 RVA: 0x000F3F95 File Offset: 0x000F2195
	public void AddTrayLoadedListener(PracticePickerTrayDisplay.TrayLoaded dlg)
	{
		this.m_TrayLoadedListeners.Add(dlg);
	}

	// Token: 0x06003087 RID: 12423 RVA: 0x000F3FA3 File Offset: 0x000F21A3
	public void RemoveTrayLoadedListener(PracticePickerTrayDisplay.TrayLoaded dlg)
	{
		this.m_TrayLoadedListeners.Remove(dlg);
	}

	// Token: 0x06003088 RID: 12424 RVA: 0x000F3FB2 File Offset: 0x000F21B2
	public bool IsLoaded()
	{
		return this.m_buttonsReady;
	}

	// Token: 0x06003089 RID: 12425 RVA: 0x000F3FBC File Offset: 0x000F21BC
	private void InitMissionRecords()
	{
		int practiceDbId = 2;
		AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
		int modeDbId = (int)selectedMode;
		this.m_sortedMissionRecords = GameDbf.Scenario.GetRecords((ScenarioDbfRecord r) => r.AdventureId == practiceDbId && r.ModeId == modeDbId);
		this.m_sortedMissionRecords.Sort(new Comparison<ScenarioDbfRecord>(GameUtils.MissionSortComparison));
	}

	// Token: 0x0600308A RID: 12426 RVA: 0x000F401C File Offset: 0x000F221C
	private void SetupHeroAchieves()
	{
		this.m_lockedHeroes = AchieveManager.Get().GetAchievesInGroup(Achievement.AchType.UNLOCK_HERO, false);
		if (this.m_lockedHeroes.Count <= 7 && !Options.Get().GetBool(Option.HAS_SEEN_PRACTICE_MODE, false))
		{
			Options.Get().SetBool(Option.HAS_SEEN_PRACTICE_MODE, true);
		}
		base.StartCoroutine(this.InitButtonsWhenReady());
	}

	// Token: 0x0600308B RID: 12427 RVA: 0x000F4078 File Offset: 0x000F2278
	private IEnumerator InitButtonsWhenReady()
	{
		while (!this.m_buttonsCreated)
		{
			yield return null;
		}
		while (!this.m_heroesLoaded)
		{
			yield return null;
		}
		this.UpdateAIButtons();
		this.m_buttonsReady = true;
		yield break;
	}

	// Token: 0x0600308C RID: 12428 RVA: 0x000F4093 File Offset: 0x000F2293
	private void OnFullDefLoaded(string cardId, FullDef def, object userData)
	{
		this.m_heroDefs[cardId] = def;
		this.m_heroDefsToLoad--;
		if (this.m_heroDefsToLoad > 0)
		{
			return;
		}
		this.m_heroesLoaded = true;
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x000F40C4 File Offset: 0x000F22C4
	private void SetSelectedButton(PracticeAIButton button)
	{
		if (this.m_selectedPracticeAIButton != null)
		{
			this.m_selectedPracticeAIButton.Deselect();
		}
		this.m_selectedPracticeAIButton = button;
	}

	// Token: 0x0600308E RID: 12430 RVA: 0x000F40EC File Offset: 0x000F22EC
	private void DisableAIButtons()
	{
		for (int i = 0; i < this.m_practiceAIButtons.Count; i++)
		{
			this.m_practiceAIButtons[i].SetEnabled(false);
		}
	}

	// Token: 0x0600308F RID: 12431 RVA: 0x000F4128 File Offset: 0x000F2328
	private void EnableAIButtons()
	{
		for (int i = 0; i < this.m_practiceAIButtons.Count; i++)
		{
			this.m_practiceAIButtons[i].SetEnabled(true);
		}
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x000F4163 File Offset: 0x000F2363
	private bool OnNavigateBack()
	{
		this.Hide();
		DeckPickerTrayDisplay.Get().ResetCurrentMode();
		return true;
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x000F4176 File Offset: 0x000F2376
	private void BackButtonReleased(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x06003092 RID: 12434 RVA: 0x000F4180 File Offset: 0x000F2380
	private void PlayGameButtonRelease(UIEvent e)
	{
		SceneUtils.SetLayer(PracticeDisplay.Get().gameObject, GameLayer.Default);
		long selectedDeckID = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
		if (selectedDeckID == 0L)
		{
			Debug.LogError("Trying to play practice game with deck ID 0!");
			return;
		}
		PegUIElement element = e.GetElement();
		element.SetEnabled(false);
		this.DisableAIButtons();
		if (AdventureConfig.Get().GetSelectedMode() == AdventureModeDbId.EXPERT && !Options.Get().GetBool(Option.HAS_PLAYED_EXPERT_AI, false))
		{
			Options.Get().SetBool(Option.HAS_PLAYED_EXPERT_AI, true);
		}
		GameMgr.Get().FindGame(1, this.m_selectedPracticeAIButton.GetMissionID(), selectedDeckID, 0L);
	}

	// Token: 0x06003093 RID: 12435 RVA: 0x000F4218 File Offset: 0x000F2418
	private void AIButtonPressed(UIEvent e)
	{
		PracticeAIButton practiceAIButton = (PracticeAIButton)e.GetElement();
		this.SetSelectedButton(practiceAIButton);
		this.m_playButton.Enable();
		practiceAIButton.Select();
	}

	// Token: 0x06003094 RID: 12436 RVA: 0x000F424C File Offset: 0x000F244C
	private void UpdateAIButtons()
	{
		this.UpdateAIDeckButtons();
		if (this.m_selectedPracticeAIButton == null)
		{
			this.m_playButton.Disable();
		}
		else
		{
			this.m_playButton.Enable();
		}
	}

	// Token: 0x06003095 RID: 12437 RVA: 0x000F428C File Offset: 0x000F248C
	private void UpdateAIButtonPositions()
	{
		int num = 0;
		foreach (PracticeAIButton component in this.m_practiceAIButtons)
		{
			TransformUtil.SetLocalPosZ(component, -this.m_AIButtonHeight * (float)num++);
		}
	}

	// Token: 0x06003096 RID: 12438 RVA: 0x000F42F8 File Offset: 0x000F24F8
	private void UpdateAIDeckButtons()
	{
		bool flag = AdventureConfig.Get().GetSelectedMode() == AdventureModeDbId.EXPERT;
		for (int i = 0; i < this.m_sortedMissionRecords.Count; i++)
		{
			ScenarioDbfRecord scenarioDbfRecord = this.m_sortedMissionRecords[i];
			int id = scenarioDbfRecord.ID;
			string missionHeroCardId = GameUtils.GetMissionHeroCardId(id);
			FullDef fullDef = this.m_heroDefs[missionHeroCardId];
			EntityDef entityDef = fullDef.GetEntityDef();
			CardDef cardDef = fullDef.GetCardDef();
			TAG_CLASS @class = entityDef.GetClass();
			string name = scenarioDbfRecord.ShortName;
			PracticeAIButton practiceAIButton = this.m_practiceAIButtons[i];
			practiceAIButton.SetInfo(name, @class, cardDef, id, false);
			bool shown = false;
			foreach (Achievement achievement in this.m_lockedHeroes)
			{
				if (achievement.ClassRequirement.Value == @class)
				{
					shown = true;
					break;
				}
			}
			practiceAIButton.ShowQuestBang(shown);
			if (practiceAIButton == this.m_selectedPracticeAIButton)
			{
				practiceAIButton.Select();
			}
			else
			{
				practiceAIButton.Deselect();
			}
		}
		bool @bool = Options.Get().GetBool(Option.HAS_SEEN_EXPERT_AI, false);
		if (flag && !@bool)
		{
			Options.Get().SetBool(Option.HAS_SEEN_EXPERT_AI, true);
		}
	}

	// Token: 0x06003097 RID: 12439 RVA: 0x000F4464 File Offset: 0x000F2664
	private IEnumerator NotifyWhenTrayLoaded()
	{
		while (!this.m_buttonsReady)
		{
			yield return null;
		}
		this.FireTrayLoadedEvent();
		yield break;
	}

	// Token: 0x06003098 RID: 12440 RVA: 0x000F4480 File Offset: 0x000F2680
	private void FireTrayLoadedEvent()
	{
		PracticePickerTrayDisplay.TrayLoaded[] array = this.m_TrayLoadedListeners.ToArray();
		foreach (PracticePickerTrayDisplay.TrayLoaded trayLoaded in array)
		{
			trayLoaded();
		}
	}

	// Token: 0x06003099 RID: 12441 RVA: 0x000F44BC File Offset: 0x000F26BC
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		FindGameState state = eventData.m_state;
		if (state == FindGameState.INVALID)
		{
			this.EnableAIButtons();
		}
		return false;
	}

	// Token: 0x04001E37 RID: 7735
	private const float PRACTICE_TRAY_MATERIAL_Y_OFFSET = -0.045f;

	// Token: 0x04001E38 RID: 7736
	[CustomEditField(Sections = "UI")]
	public UberText m_trayLabel;

	// Token: 0x04001E39 RID: 7737
	[CustomEditField(Sections = "UI")]
	public StandardPegButtonNew m_backButton;

	// Token: 0x04001E3A RID: 7738
	[CustomEditField(Sections = "UI")]
	public PlayButton m_playButton;

	// Token: 0x04001E3B RID: 7739
	[CustomEditField(Sections = "AI Button Settings")]
	public PracticeAIButton m_AIButtonPrefab;

	// Token: 0x04001E3C RID: 7740
	[CustomEditField(Sections = "AI Button Settings")]
	public GameObject m_AIButtonsContainer;

	// Token: 0x04001E3D RID: 7741
	[SerializeField]
	private float m_AIButtonHeight = 5f;

	// Token: 0x04001E3E RID: 7742
	[CustomEditField(Sections = "Animation Settings")]
	public float m_trayAnimationTime = 0.5f;

	// Token: 0x04001E3F RID: 7743
	[CustomEditField(Sections = "Animation Settings")]
	public iTween.EaseType m_trayInEaseType = iTween.EaseType.easeOutBounce;

	// Token: 0x04001E40 RID: 7744
	[CustomEditField(Sections = "Animation Settings")]
	public iTween.EaseType m_trayOutEaseType = iTween.EaseType.easeOutCubic;

	// Token: 0x04001E41 RID: 7745
	private static PracticePickerTrayDisplay s_instance;

	// Token: 0x04001E42 RID: 7746
	private List<ScenarioDbfRecord> m_sortedMissionRecords = new List<ScenarioDbfRecord>();

	// Token: 0x04001E43 RID: 7747
	private List<PracticeAIButton> m_practiceAIButtons = new List<PracticeAIButton>();

	// Token: 0x04001E44 RID: 7748
	private List<Achievement> m_lockedHeroes;

	// Token: 0x04001E45 RID: 7749
	private PracticeAIButton m_selectedPracticeAIButton;

	// Token: 0x04001E46 RID: 7750
	private Map<string, FullDef> m_heroDefs = new Map<string, FullDef>();

	// Token: 0x04001E47 RID: 7751
	private int m_heroDefsToLoad;

	// Token: 0x04001E48 RID: 7752
	private List<PracticePickerTrayDisplay.TrayLoaded> m_TrayLoadedListeners = new List<PracticePickerTrayDisplay.TrayLoaded>();

	// Token: 0x04001E49 RID: 7753
	private bool m_buttonsCreated;

	// Token: 0x04001E4A RID: 7754
	private bool m_buttonsReady;

	// Token: 0x04001E4B RID: 7755
	private bool m_heroesLoaded;

	// Token: 0x04001E4C RID: 7756
	private bool m_shown;

	// Token: 0x04001E4D RID: 7757
	private static readonly int NUM_AI_BUTTONS_TO_SHOW = 14;

	// Token: 0x020007E5 RID: 2021
	// (Invoke) Token: 0x06004EA1 RID: 20129
	public delegate void TrayLoaded();
}
