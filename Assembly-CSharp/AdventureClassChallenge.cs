using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200029B RID: 667
[CustomEditClass]
public class AdventureClassChallenge : MonoBehaviour
{
	// Token: 0x06002426 RID: 9254 RVA: 0x000B1548 File Offset: 0x000AF748
	private void Awake()
	{
		this.m_BackButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.BackButton();
		});
		this.m_PlayButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.Play();
		});
		this.m_EmptyChallengeButtonSlot.SetActive(false);
		AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(this.m_VersusTextPrefab), new AssetLoader.GameObjectCallback(this.OnVersusLettersLoaded), null, false);
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x000B15B8 File Offset: 0x000AF7B8
	private void Start()
	{
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		this.InitModeName();
		this.InitAdventureChallenges();
		Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureClassChallenge.OnNavigateBack));
		base.StartCoroutine(this.CreateChallengeButtons());
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x000B1605 File Offset: 0x000AF805
	private void OnDestroy()
	{
		GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x000B1620 File Offset: 0x000AF820
	private void InitModeName()
	{
		int selectedAdventure = (int)AdventureConfig.Get().GetSelectedAdventure();
		int modeId = 4;
		AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord(selectedAdventure, modeId);
		string text = (!UniversalInputManager.UsePhoneUI) ? adventureDataRecord.Name : adventureDataRecord.ShortName;
		this.m_ModeName.Text = text;
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x000B1674 File Offset: 0x000AF874
	private void InitAdventureChallenges()
	{
		List<ScenarioDbfRecord> records = GameDbf.Scenario.GetRecords();
		records.Sort(delegate(ScenarioDbfRecord a, ScenarioDbfRecord b)
		{
			int sortOrder = a.SortOrder;
			int sortOrder2 = b.SortOrder;
			return sortOrder - sortOrder2;
		});
		foreach (ScenarioDbfRecord scenarioDbfRecord in records)
		{
			if (scenarioDbfRecord.AdventureId == (int)AdventureConfig.Get().GetSelectedAdventure())
			{
				if (scenarioDbfRecord.ModeId == 4)
				{
					int player1HeroCardId = scenarioDbfRecord.Player1HeroCardId;
					int num = scenarioDbfRecord.ClientPlayer2HeroCardId;
					if (num == 0)
					{
						num = scenarioDbfRecord.Player2HeroCardId;
					}
					AdventureClassChallenge.ClassChallengeData classChallengeData = new AdventureClassChallenge.ClassChallengeData();
					classChallengeData.scenarioRecord = scenarioDbfRecord;
					classChallengeData.heroID0 = GameUtils.TranslateDbIdToCardId(player1HeroCardId);
					classChallengeData.heroID1 = GameUtils.TranslateDbIdToCardId(num);
					classChallengeData.unlocked = AdventureProgressMgr.Get().CanPlayScenario(scenarioDbfRecord.ID);
					if (AdventureProgressMgr.Get().HasDefeatedScenario(scenarioDbfRecord.ID))
					{
						classChallengeData.defeated = true;
					}
					else
					{
						classChallengeData.defeated = false;
					}
					classChallengeData.name = scenarioDbfRecord.ShortName;
					classChallengeData.title = scenarioDbfRecord.Name;
					classChallengeData.description = scenarioDbfRecord.Description;
					classChallengeData.completedDescription = scenarioDbfRecord.CompletedDescription;
					classChallengeData.opponentName = scenarioDbfRecord.OpponentName;
					this.m_ScenarioChallengeLookup.Add(scenarioDbfRecord.ID, this.m_ClassChallenges.Count);
					this.m_ClassChallenges.Add(classChallengeData);
				}
			}
		}
	}

	// Token: 0x0600242B RID: 9259 RVA: 0x000B1834 File Offset: 0x000AFA34
	private int BossCreateParamsSortComparison(AdventureClassChallenge.ClassChallengeData data1, AdventureClassChallenge.ClassChallengeData data2)
	{
		return GameUtils.MissionSortComparison(data1.scenarioRecord, data2.scenarioRecord);
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x000B1848 File Offset: 0x000AFA48
	private IEnumerator CreateChallengeButtons()
	{
		int buttonCount = 0;
		int lastSelectedScenario = (int)AdventureConfig.Get().GetLastSelectedMission();
		foreach (AdventureClassChallenge.ClassChallengeData cdata in this.m_ClassChallenges)
		{
			if (cdata.unlocked)
			{
				GameObject button = (GameObject)GameUtils.Instantiate(this.m_ClassChallengeButtonPrefab, this.m_ChallengeButtonContainer, false);
				button.transform.localPosition = this.m_ClassChallengeButtonSpacing * (float)buttonCount;
				AdventureClassChallengeButton challengeButton = button.GetComponent<AdventureClassChallengeButton>();
				challengeButton.m_Text.Text = cdata.name;
				challengeButton.m_ScenarioID = cdata.scenarioRecord.ID;
				challengeButton.m_Chest.SetActive(!cdata.defeated);
				challengeButton.m_Checkmark.SetActive(cdata.defeated);
				challengeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ButtonPressed));
				this.LoadButtonPortrait(challengeButton, cdata.heroID1);
				if (lastSelectedScenario == challengeButton.m_ScenarioID || !this.m_SelectedButton)
				{
					this.m_SelectedButton = challengeButton;
					this.m_SelectedScenario = cdata.scenarioRecord.ID;
				}
				buttonCount++;
			}
		}
		int emptySlotCount = 10 - buttonCount;
		if (emptySlotCount <= 0)
		{
			Debug.LogError(string.Format("Adventure Class Challenge tray UI doesn't support scrolling yet. More than {0} buttons where added.", 10));
			yield break;
		}
		for (int s = 0; s < emptySlotCount; s++)
		{
			GameObject emptySlot = (GameObject)GameUtils.Instantiate(this.m_EmptyChallengeButtonSlot, this.m_ChallengeButtonContainer, false);
			emptySlot.transform.localPosition = this.m_ClassChallengeButtonSpacing * (float)(buttonCount + s);
			emptySlot.transform.localRotation = Quaternion.identity;
			emptySlot.SetActive(true);
			Renderer renderer = emptySlot.GetComponentInChildren<Renderer>();
			renderer.material.mainTextureOffset = new Vector2(0f, this.EMPTY_SLOT_UV_OFFSET[this.m_UVoffset]);
			this.m_UVoffset++;
			if (this.m_UVoffset > 5)
			{
				this.m_UVoffset = 0;
			}
		}
		yield return null;
		if (this.m_SelectedButton == null)
		{
			Debug.LogWarning("AdventureClassChallenge.m_SelectedButton is null!");
			yield break;
		}
		this.SetSelectedButton(this.m_SelectedButton);
		this.m_SelectedButton.Select(false);
		this.GetRewardCardForSelectedScenario();
		this.m_PlayButton.Enable();
		if (this.m_ChallengeButtonScroller != null)
		{
			this.m_ChallengeButtonScroller.SetScrollHeightCallback(() => this.m_ChallengeButtonHeight * (float)this.m_ClassChallenges.Count, false, false);
		}
		base.GetComponent<AdventureSubScene>().SetIsLoaded(true);
		yield break;
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x000B1864 File Offset: 0x000AFA64
	private void ButtonPressed(UIEvent e)
	{
		if (this.m_ChallengeButtonScroller != null && this.m_ChallengeButtonScroller.IsTouchDragging())
		{
			return;
		}
		AdventureClassChallengeButton adventureClassChallengeButton = (AdventureClassChallengeButton)e.GetElement();
		this.m_SelectedButton.Deselect();
		this.SetSelectedButton(adventureClassChallengeButton);
		adventureClassChallengeButton.Select(true);
		this.m_SelectedScenario = adventureClassChallengeButton.m_ScenarioID;
		this.m_SelectedButton = adventureClassChallengeButton;
		this.GetRewardCardForSelectedScenario();
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x000B18D4 File Offset: 0x000AFAD4
	private void SetSelectedButton(AdventureClassChallengeButton button)
	{
		int scenarioID = button.m_ScenarioID;
		AdventureConfig.Get().SetMission((ScenarioDbId)scenarioID, true);
		this.SetScenario(scenarioID);
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000B18FB File Offset: 0x000AFAFB
	private void LoadButtonPortrait(AdventureClassChallengeButton button, string heroID)
	{
		DefLoader.Get().LoadFullDef(heroID, new DefLoader.LoadDefCallback<FullDef>(this.OnButtonFullDefLoaded), button);
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x000B1918 File Offset: 0x000AFB18
	private void OnButtonFullDefLoaded(string cardId, FullDef fullDef, object userData)
	{
		AdventureClassChallengeButton adventureClassChallengeButton = (AdventureClassChallengeButton)userData;
		CardDef cardDef = fullDef.GetCardDef();
		Material practiceAIPortrait = cardDef.GetPracticeAIPortrait();
		if (practiceAIPortrait != null)
		{
			practiceAIPortrait.mainTexture = cardDef.GetPortraitTexture();
			adventureClassChallengeButton.SetPortraitMaterial(practiceAIPortrait);
		}
	}

	// Token: 0x06002431 RID: 9265 RVA: 0x000B195C File Offset: 0x000AFB5C
	private void SetScenario(int scenarioID)
	{
		AdventureClassChallenge.ClassChallengeData classChallengeData = this.m_ClassChallenges[this.m_ScenarioChallengeLookup[scenarioID]];
		this.LoadHero(0, classChallengeData.heroID0);
		this.LoadHero(1, classChallengeData.heroID1);
		this.m_RightHeroName.Text = classChallengeData.opponentName;
		this.m_ChallengeTitle.Text = classChallengeData.title;
		if (classChallengeData.defeated)
		{
			this.m_ChallengeDescription.Text = classChallengeData.completedDescription;
		}
		else
		{
			this.m_ChallengeDescription.Text = classChallengeData.description;
		}
		if (!UniversalInputManager.UsePhoneUI)
		{
			if (this.m_ClassChallenges[this.m_ScenarioChallengeLookup[scenarioID]].defeated)
			{
				this.m_ChestButton.gameObject.SetActive(false);
				this.m_ChestButtonCover.SetActive(true);
			}
			else
			{
				this.m_ChestButton.gameObject.SetActive(true);
				this.m_ChestButtonCover.SetActive(false);
			}
		}
	}

	// Token: 0x06002432 RID: 9266 RVA: 0x000B1A60 File Offset: 0x000AFC60
	private void LoadHero(int heroNum, string heroID)
	{
		AdventureClassChallenge.HeroLoadData heroLoadData = new AdventureClassChallenge.HeroLoadData();
		heroLoadData.heroNum = heroNum;
		heroLoadData.heroID = heroID;
		DefLoader.Get().LoadFullDef(heroID, new DefLoader.LoadDefCallback<FullDef>(this.OnHeroFullDefLoaded), heroLoadData);
	}

	// Token: 0x06002433 RID: 9267 RVA: 0x000B1A9C File Offset: 0x000AFC9C
	private void OnHeroFullDefLoaded(string cardId, FullDef fullDef, object userData)
	{
		if (fullDef == null)
		{
			Debug.LogWarning(string.Format("AdventureClassChallenge.OnHeroFullDefLoaded() - FAILED to load \"{0}\"", cardId));
			return;
		}
		AdventureClassChallenge.HeroLoadData heroLoadData = (AdventureClassChallenge.HeroLoadData)userData;
		heroLoadData.fulldef = fullDef;
		AssetLoader.Get().LoadActor("Card_Play_Hero", new AssetLoader.GameObjectCallback(this.OnActorLoaded), heroLoadData, false);
	}

	// Token: 0x06002434 RID: 9268 RVA: 0x000B1AEC File Offset: 0x000AFCEC
	private void OnActorLoaded(string name, GameObject actorObject, object userData)
	{
		AdventureClassChallenge.HeroLoadData heroLoadData = (AdventureClassChallenge.HeroLoadData)userData;
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("AdventureClassChallenge.OnActorLoaded() - FAILED to load actor \"{0}\"", name));
			return;
		}
		Actor component = actorObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("AdventureClassChallenge.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", name));
			return;
		}
		component.TurnOffCollider();
		component.SetUnlit();
		Object.Destroy(component.m_healthObject);
		Object.Destroy(component.m_attackObject);
		component.SetEntityDef(heroLoadData.fulldef.GetEntityDef());
		component.SetCardDef(heroLoadData.fulldef.GetCardDef());
		component.SetPremium(TAG_PREMIUM.NORMAL);
		component.UpdateAllComponents();
		GameObject parent = this.m_LeftHeroContainer;
		if (heroLoadData.heroNum == 0)
		{
			Object.Destroy(this.m_LeftHero);
			this.m_LeftHero = actorObject;
			this.m_LeftHeroName.Text = heroLoadData.fulldef.GetEntityDef().GetName();
		}
		else
		{
			Object.Destroy(this.m_RightHero);
			this.m_RightHero = actorObject;
			parent = this.m_RightHeroContainer;
		}
		GameUtils.SetParent(component, parent, false);
		component.transform.localRotation = Quaternion.identity;
		component.transform.localScale = Vector3.one;
		component.GetAttackObject().Hide();
		component.Show();
	}

	// Token: 0x06002435 RID: 9269 RVA: 0x000B1C28 File Offset: 0x000AFE28
	private void OnVersusLettersLoaded(string name, GameObject go, object userData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("AdventureClassChallenge.OnVersusLettersLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		GameUtils.SetParent(go, this.m_VersusTextContainer, false);
		go.GetComponentInChildren<VS>().ActivateShadow(true);
		go.transform.localRotation = Quaternion.identity;
		go.transform.Rotate(new Vector3(0f, 180f, 0f));
		go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		Component[] componentsInChildren = go.GetComponentsInChildren(typeof(Renderer));
		for (int i = 0; i < componentsInChildren.Length - 1; i++)
		{
			Renderer renderer = (Renderer)componentsInChildren[i];
			renderer.material.SetColor("_Color", this.m_VersusTextColor);
		}
	}

	// Token: 0x06002436 RID: 9270 RVA: 0x000B1CFF File Offset: 0x000AFEFF
	private static bool OnNavigateBack()
	{
		AdventureConfig.Get().ChangeToLastSubScene(true);
		return true;
	}

	// Token: 0x06002437 RID: 9271 RVA: 0x000B1D0D File Offset: 0x000AFF0D
	private void BackButton()
	{
		Navigation.GoBack();
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x000B1D18 File Offset: 0x000AFF18
	private void Play()
	{
		this.m_PlayButton.Disable();
		GameMgr.Get().FindGame(1, this.m_SelectedScenario, 0L, 0L);
	}

	// Token: 0x06002439 RID: 9273 RVA: 0x000B1D48 File Offset: 0x000AFF48
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		FindGameState state = eventData.m_state;
		if (state == FindGameState.INVALID)
		{
			this.m_PlayButton.Enable();
		}
		return false;
	}

	// Token: 0x0600243A RID: 9274 RVA: 0x000B1D78 File Offset: 0x000AFF78
	private void GetRewardCardForSelectedScenario()
	{
		if (this.m_RewardBone == null)
		{
			return;
		}
		this.m_ChestButton.m_IsRewardLoading = true;
		List<CardRewardData> immediateCardRewardsForDefeatingScenario = AdventureProgressMgr.Get().GetImmediateCardRewardsForDefeatingScenario(this.m_SelectedScenario);
		if (immediateCardRewardsForDefeatingScenario != null && immediateCardRewardsForDefeatingScenario.Count > 0)
		{
			immediateCardRewardsForDefeatingScenario[0].LoadRewardObject(new Reward.DelOnRewardLoaded(this.RewardCardLoaded));
		}
	}

	// Token: 0x0600243B RID: 9275 RVA: 0x000B1DE0 File Offset: 0x000AFFE0
	private void RewardCardLoaded(Reward reward, object callbackData)
	{
		if (reward == null)
		{
			Debug.LogWarning(string.Format("AdventureClassChallenge.RewardCardLoaded() - FAILED to load reward \"{0}\"", base.name));
			return;
		}
		if (reward.gameObject == null)
		{
			Debug.LogWarning(string.Format("AdventureClassChallenge.RewardCardLoaded() - Reward GameObject is null \"{0}\"", base.name));
			return;
		}
		reward.gameObject.transform.parent = this.m_ChestButton.transform;
		CardReward component = reward.GetComponent<CardReward>();
		if (this.m_ChestButton.m_RewardCard != null)
		{
			Object.Destroy(this.m_ChestButton.m_RewardCard);
		}
		this.m_ChestButton.m_RewardCard = component.m_nonHeroCardsRoot;
		GameUtils.SetParent(component.m_nonHeroCardsRoot, this.m_RewardBone, false);
		component.m_nonHeroCardsRoot.SetActive(false);
		Object.Destroy(component.gameObject);
		this.m_ChestButton.m_IsRewardLoading = false;
	}

	// Token: 0x04001539 RID: 5433
	private const float CHALLENGE_BUTTON_OFFSET = 4.3f;

	// Token: 0x0400153A RID: 5434
	private const int VISIBLE_SLOT_COUNT = 10;

	// Token: 0x0400153B RID: 5435
	private readonly float[] EMPTY_SLOT_UV_OFFSET = new float[]
	{
		0f,
		0.223f,
		0.377f,
		0.535f,
		0.69f,
		0.85f
	};

	// Token: 0x0400153C RID: 5436
	[CustomEditField(Sections = "DBF Stuff")]
	public UberText m_ModeName;

	// Token: 0x0400153D RID: 5437
	[CustomEditField(Sections = "Class Challenge Buttons")]
	public GameObject m_ClassChallengeButtonPrefab;

	// Token: 0x0400153E RID: 5438
	[CustomEditField(Sections = "Class Challenge Buttons")]
	public Vector3 m_ClassChallengeButtonSpacing;

	// Token: 0x0400153F RID: 5439
	[CustomEditField(Sections = "Class Challenge Buttons")]
	public GameObject m_ChallengeButtonContainer;

	// Token: 0x04001540 RID: 5440
	[CustomEditField(Sections = "Class Challenge Buttons")]
	public GameObject m_EmptyChallengeButtonSlot;

	// Token: 0x04001541 RID: 5441
	[CustomEditField(Sections = "Class Challenge Buttons")]
	public float m_ChallengeButtonHeight;

	// Token: 0x04001542 RID: 5442
	[CustomEditField(Sections = "Class Challenge Buttons")]
	public UIBScrollable m_ChallengeButtonScroller;

	// Token: 0x04001543 RID: 5443
	[CustomEditField(Sections = "Hero Portraits")]
	public GameObject m_LeftHeroContainer;

	// Token: 0x04001544 RID: 5444
	[CustomEditField(Sections = "Hero Portraits")]
	public GameObject m_RightHeroContainer;

	// Token: 0x04001545 RID: 5445
	[CustomEditField(Sections = "Hero Portraits")]
	public UberText m_LeftHeroName;

	// Token: 0x04001546 RID: 5446
	[CustomEditField(Sections = "Hero Portraits")]
	public UberText m_RightHeroName;

	// Token: 0x04001547 RID: 5447
	[CustomEditField(Sections = "Versus Text", T = EditType.GAME_OBJECT)]
	public string m_VersusTextPrefab;

	// Token: 0x04001548 RID: 5448
	[CustomEditField(Sections = "Versus Text")]
	public GameObject m_VersusTextContainer;

	// Token: 0x04001549 RID: 5449
	[CustomEditField(Sections = "Versus Text")]
	public Color m_VersusTextColor;

	// Token: 0x0400154A RID: 5450
	[CustomEditField(Sections = "Text")]
	public UberText m_ChallengeTitle;

	// Token: 0x0400154B RID: 5451
	[CustomEditField(Sections = "Text")]
	public UberText m_ChallengeDescription;

	// Token: 0x0400154C RID: 5452
	[CustomEditField(Sections = "Basic UI")]
	public PlayButton m_PlayButton;

	// Token: 0x0400154D RID: 5453
	[CustomEditField(Sections = "Basic UI")]
	public UIBButton m_BackButton;

	// Token: 0x0400154E RID: 5454
	[CustomEditField(Sections = "Reward UI")]
	public AdventureClassChallengeChestButton m_ChestButton;

	// Token: 0x0400154F RID: 5455
	[CustomEditField(Sections = "Reward UI")]
	public GameObject m_ChestButtonCover;

	// Token: 0x04001550 RID: 5456
	[CustomEditField(Sections = "Reward UI")]
	public Transform m_RewardBone;

	// Token: 0x04001551 RID: 5457
	private List<AdventureClassChallenge.ClassChallengeData> m_ClassChallenges = new List<AdventureClassChallenge.ClassChallengeData>();

	// Token: 0x04001552 RID: 5458
	private Map<int, int> m_ScenarioChallengeLookup = new Map<int, int>();

	// Token: 0x04001553 RID: 5459
	private int m_UVoffset;

	// Token: 0x04001554 RID: 5460
	private AdventureClassChallengeButton m_SelectedButton;

	// Token: 0x04001555 RID: 5461
	private GameObject m_LeftHero;

	// Token: 0x04001556 RID: 5462
	private GameObject m_RightHero;

	// Token: 0x04001557 RID: 5463
	private int m_SelectedScenario;

	// Token: 0x04001558 RID: 5464
	private bool m_gameDenied;

	// Token: 0x0200029C RID: 668
	private class ClassChallengeData
	{
		// Token: 0x0400155A RID: 5466
		public ScenarioDbfRecord scenarioRecord;

		// Token: 0x0400155B RID: 5467
		public bool unlocked;

		// Token: 0x0400155C RID: 5468
		public bool defeated;

		// Token: 0x0400155D RID: 5469
		public string heroID0;

		// Token: 0x0400155E RID: 5470
		public string heroID1;

		// Token: 0x0400155F RID: 5471
		public string name;

		// Token: 0x04001560 RID: 5472
		public string title;

		// Token: 0x04001561 RID: 5473
		public string description;

		// Token: 0x04001562 RID: 5474
		public string completedDescription;

		// Token: 0x04001563 RID: 5475
		public string opponentName;
	}

	// Token: 0x0200029D RID: 669
	private class HeroLoadData
	{
		// Token: 0x04001564 RID: 5476
		public int heroNum;

		// Token: 0x04001565 RID: 5477
		public string heroID;

		// Token: 0x04001566 RID: 5478
		public FullDef fulldef;
	}
}
