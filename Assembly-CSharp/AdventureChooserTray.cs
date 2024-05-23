using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200020F RID: 527
[CustomEditClass]
public class AdventureChooserTray : MonoBehaviour
{
	// Token: 0x17000333 RID: 819
	// (get) Token: 0x0600203F RID: 8255 RVA: 0x0009DEC4 File Offset: 0x0009C0C4
	// (set) Token: 0x06002040 RID: 8256 RVA: 0x0009DECC File Offset: 0x0009C0CC
	[CustomEditField(Sections = "Behavior Settings")]
	public float ButtonOffset
	{
		get
		{
			return this.m_ButtonOffset;
		}
		set
		{
			this.m_ButtonOffset = value;
			this.OnButtonVisualUpdated();
		}
	}

	// Token: 0x06002041 RID: 8257 RVA: 0x0009DEDC File Offset: 0x0009C0DC
	private void Awake()
	{
		this.m_ChooseButton.Disable();
		this.m_BackButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.OnBackButton();
		});
		this.m_ChooseButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.ChangeSubScene();
		});
		AdventureConfig.Get().AddSelectedModeChangeListener(new AdventureConfig.SelectedModeChange(this.OnSelectedModeChange));
		if (this.m_ChooseElementsContainer == null)
		{
			Debug.LogError("m_ChooseElementsContainer cannot be null. Unable to create button.", this);
			return;
		}
		List<AdventureDef> sortedAdventureDefs = AdventureScene.Get().GetSortedAdventureDefs();
		foreach (AdventureDef adventureDef in sortedAdventureDefs)
		{
			AdventureDbId adventureId = adventureDef.GetAdventureId();
			if (!GameUtils.IsAdventureRotated(adventureId) || AdventureProgressMgr.Get().OwnsOneOrMoreAdventureWings(adventureId))
			{
				if (AdventureScene.Get().IsAdventureOpen(adventureId))
				{
					this.CreateAdventureChooserButton(adventureDef);
				}
			}
		}
		if (this.m_ParentSubScene != null)
		{
			this.m_ParentSubScene.SetIsLoaded(true);
		}
		this.OnButtonVisualUpdated();
		Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureChooserTray.OnNavigateBack));
		Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x0009E030 File Offset: 0x0009C230
	private void Start()
	{
		this.m_isStarted = true;
	}

	// Token: 0x06002043 RID: 8259 RVA: 0x0009E03C File Offset: 0x0009C23C
	private void OnDestroy()
	{
		if (AdventureConfig.Get() != null)
		{
			AdventureConfig.Get().RemoveSelectedModeChangeListener(new AdventureConfig.SelectedModeChange(this.OnSelectedModeChange));
		}
		if (Box.Get() != null)
		{
			Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
		}
		base.CancelInvoke("ShowDisabledAdventureModeRequirementsWarning");
	}

	// Token: 0x06002044 RID: 8260 RVA: 0x0009E0A4 File Offset: 0x0009C2A4
	private void OnButtonVisualUpdated()
	{
		float num = 0f;
		AdventureChooserButton[] array = this.m_AdventureButtons.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			TransformUtil.SetLocalPosZ(array[i].transform, -num);
			num += array[i].GetFullButtonHeight() + this.m_ButtonOffset;
		}
	}

	// Token: 0x06002045 RID: 8261 RVA: 0x0009E0F8 File Offset: 0x0009C2F8
	private void OnAdventureButtonToggled(AdventureChooserButton btn, bool toggled, int index)
	{
		btn.SetSelectSubButtonOnToggle(this.m_OnlyOneExpands);
		if (this.m_OnlyOneExpands)
		{
			if (toggled)
			{
				AdventureChooserButton[] array = this.m_AdventureButtons.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					if (i != index)
					{
						array[i].Toggle = false;
					}
				}
			}
		}
		else if (this.m_SelectedSubButton != null)
		{
			btn = this.m_AdventureButtons[index];
			if (btn.ContainsSubButton(this.m_SelectedSubButton))
			{
				this.m_SelectedSubButton.SetHighlight(toggled);
				if (!toggled)
				{
					this.m_ChooseButton.Disable();
				}
				else if (!this.m_AttemptedLoad)
				{
					this.m_ChooseButton.Enable();
				}
			}
		}
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x0009E1C0 File Offset: 0x0009C3C0
	private void CreateAdventureChooserButton(AdventureDef advDef)
	{
		string assetPath = this.m_DefaultChooserButtonPrefab;
		if (!string.IsNullOrEmpty(advDef.m_ChooserButtonPrefab))
		{
			assetPath = advDef.m_ChooserButtonPrefab;
		}
		AdventureChooserButton newbutton = GameUtils.LoadGameObjectWithComponent<AdventureChooserButton>(assetPath);
		if (newbutton == null)
		{
			return;
		}
		GameUtils.SetParent(newbutton, this.m_ChooseElementsContainer, false);
		AdventureDbId adventureId = advDef.GetAdventureId();
		GameUtils.SetAutomationName(newbutton.gameObject, new object[]
		{
			adventureId
		});
		newbutton.SetAdventure(adventureId);
		newbutton.SetButtonText(advDef.GetAdventureName());
		newbutton.SetPortraitTexture(advDef.m_Texture);
		newbutton.SetPortraitTiling(advDef.m_TextureTiling);
		newbutton.SetPortraitOffset(advDef.m_TextureOffset);
		AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
		AdventureModeDbId clientChooserAdventureMode = AdventureConfig.Get().GetClientChooserAdventureMode(adventureId);
		if (selectedAdventure == adventureId)
		{
			newbutton.Toggle = true;
		}
		List<AdventureSubDef> sortedSubDefs = advDef.GetSortedSubDefs();
		string subButtonPrefab = this.m_DefaultChooserSubButtonPrefab;
		if (!string.IsNullOrEmpty(advDef.m_ChooserSubButtonPrefab))
		{
			subButtonPrefab = advDef.m_ChooserSubButtonPrefab;
		}
		foreach (AdventureSubDef adventureSubDef in sortedSubDefs)
		{
			AdventureModeDbId adventureModeId = adventureSubDef.GetAdventureModeId();
			AdventureChooserSubButton adventureChooserSubButton = newbutton.CreateSubButton(adventureModeId, adventureSubDef, subButtonPrefab, clientChooserAdventureMode == adventureModeId);
			if (!(adventureChooserSubButton == null))
			{
				bool flag = newbutton.Toggle && clientChooserAdventureMode == adventureModeId;
				if (flag)
				{
					adventureChooserSubButton.SetHighlight(true);
					this.UpdateChooseButton(adventureId, adventureModeId);
					this.SetTitleText(adventureId, adventureModeId);
				}
				else if (AdventureConfig.Get().IsFeaturedMode(adventureId, adventureModeId))
				{
					adventureChooserSubButton.SetNewGlow(true);
				}
				adventureChooserSubButton.SetDesaturate(!AdventureConfig.Get().CanPlayMode(adventureId, adventureModeId));
				this.CreateAdventureChooserDescriptionFromPrefab(adventureId, adventureSubDef, flag);
			}
		}
		newbutton.AddVisualUpdatedListener(new AdventureChooserButton.VisualUpdated(this.OnButtonVisualUpdated));
		int index = this.m_AdventureButtons.Count;
		newbutton.AddToggleListener(delegate(bool toggle)
		{
			this.OnAdventureButtonToggled(newbutton, toggle, index);
		});
		newbutton.AddModeSelectionListener(new AdventureChooserButton.ModeSelection(this.ButtonModeSelected));
		newbutton.AddExpandedListener(new AdventureChooserButton.Expanded(this.ButtonExpanded));
		this.m_AdventureButtons.Add(newbutton);
	}

	// Token: 0x06002047 RID: 8263 RVA: 0x0009E478 File Offset: 0x0009C678
	private void CreateAdventureChooserDescriptionFromPrefab(AdventureDbId adventureId, AdventureSubDef subDef, bool active)
	{
		if (string.IsNullOrEmpty(subDef.m_ChooserDescriptionPrefab))
		{
			return;
		}
		Map<AdventureModeDbId, AdventureChooserDescription> map;
		if (!this.m_Descriptions.TryGetValue(adventureId, out map))
		{
			map = new Map<AdventureModeDbId, AdventureChooserDescription>();
			this.m_Descriptions[adventureId] = map;
		}
		string description = subDef.GetDescription();
		string requiredText = null;
		if (!AdventureConfig.Get().CanPlayMode(adventureId, subDef.GetAdventureModeId()))
		{
			requiredText = subDef.GetRequirementsDescription();
		}
		AdventureChooserDescription adventureChooserDescription = GameUtils.LoadGameObjectWithComponent<AdventureChooserDescription>(subDef.m_ChooserDescriptionPrefab);
		if (adventureChooserDescription == null)
		{
			return;
		}
		GameUtils.SetParent(adventureChooserDescription, this.m_DescriptionContainer, false);
		adventureChooserDescription.SetText(requiredText, description);
		adventureChooserDescription.gameObject.SetActive(active);
		map[subDef.GetAdventureModeId()] = adventureChooserDescription;
		if (active)
		{
			this.m_CurrentChooserDescription = adventureChooserDescription;
		}
	}

	// Token: 0x06002048 RID: 8264 RVA: 0x0009E540 File Offset: 0x0009C740
	private AdventureChooserDescription GetAdventureChooserDescription(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		Map<AdventureModeDbId, AdventureChooserDescription> map;
		if (!this.m_Descriptions.TryGetValue(adventureId, out map))
		{
			return null;
		}
		AdventureChooserDescription result;
		if (!map.TryGetValue(modeId, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x06002049 RID: 8265 RVA: 0x0009E574 File Offset: 0x0009C774
	private void ButtonModeSelected(AdventureChooserSubButton btn)
	{
		foreach (AdventureChooserButton adventureChooserButton in this.m_AdventureButtons)
		{
			adventureChooserButton.DisableSubButtonHighlights();
		}
		this.m_SelectedSubButton = btn;
		if (AdventureConfig.Get().MarkFeaturedMode(btn.GetAdventure(), btn.GetMode()))
		{
			btn.SetNewGlow(false);
		}
		AdventureConfig.Get().SetSelectedAdventureMode(btn.GetAdventure(), btn.GetMode());
		this.SetTitleText(btn.GetAdventure(), btn.GetMode());
	}

	// Token: 0x0600204A RID: 8266 RVA: 0x0009E620 File Offset: 0x0009C820
	private void ButtonExpanded(AdventureChooserButton button, bool expand)
	{
		if (!expand)
		{
			return;
		}
		AdventureConfig adventureConfig = AdventureConfig.Get();
		AdventureChooserSubButton[] subButtons = button.GetSubButtons();
		foreach (AdventureChooserSubButton adventureChooserSubButton in subButtons)
		{
			if (adventureConfig.IsFeaturedMode(button.GetAdventure(), adventureChooserSubButton.GetMode()))
			{
				adventureChooserSubButton.Flash();
			}
		}
	}

	// Token: 0x0600204B RID: 8267 RVA: 0x0009E680 File Offset: 0x0009C880
	private void SetTitleText(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int)adventureId, (int)modeId);
		this.m_DescriptionTitleObject.Text = adventureDataRecord.Name;
	}

	// Token: 0x0600204C RID: 8268 RVA: 0x0009E6AC File Offset: 0x0009C8AC
	private void OnSelectedModeChange(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		AdventureChooserDescription adventureChooserDescription = this.GetAdventureChooserDescription(adventureId, modeId);
		if (this.m_CurrentChooserDescription != null)
		{
			this.m_CurrentChooserDescription.gameObject.SetActive(false);
		}
		this.m_CurrentChooserDescription = adventureChooserDescription;
		if (this.m_CurrentChooserDescription != null)
		{
			this.m_CurrentChooserDescription.gameObject.SetActive(true);
		}
		this.UpdateChooseButton(adventureId, modeId);
		if (this.m_ChooseButton.IsEnabled())
		{
			PlayMakerFSM component = this.m_ChooseButton.GetComponent<PlayMakerFSM>();
			if (component != null)
			{
				component.SendEvent("Burst");
			}
		}
		if (!AdventureConfig.Get().CanPlayMode(adventureId, modeId))
		{
			if (!this.m_isStarted)
			{
				base.Invoke("ShowDisabledAdventureModeRequirementsWarning", 0f);
			}
			else
			{
				this.ShowDisabledAdventureModeRequirementsWarning();
			}
		}
	}

	// Token: 0x0600204D RID: 8269 RVA: 0x0009E780 File Offset: 0x0009C980
	private void ShowDisabledAdventureModeRequirementsWarning()
	{
		base.CancelInvoke("ShowDisabledAdventureModeRequirementsWarning");
		if (!this.m_isStarted || SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE)
		{
			return;
		}
		if (this.m_ChooseButton != null && !this.m_ChooseButton.IsEnabled())
		{
			AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
			AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
			if (!AdventureConfig.Get().CanPlayMode(selectedAdventure, selectedMode))
			{
				int adventureId = (int)selectedAdventure;
				int modeId = (int)selectedMode;
				AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord(adventureId, modeId);
				string text = adventureDataRecord.RequirementsDescription;
				if (!string.IsNullOrEmpty(text))
				{
					string header = adventureDataRecord.Name;
					Error.AddWarning(header, text, new object[0]);
				}
			}
		}
	}

	// Token: 0x0600204E RID: 8270 RVA: 0x0009E844 File Offset: 0x0009CA44
	private void UpdateChooseButton(AdventureDbId adventureId, AdventureModeDbId modeId)
	{
		if (!this.m_AttemptedLoad && AdventureConfig.Get().CanPlayMode(adventureId, modeId))
		{
			this.m_ChooseButton.SetText(GameStrings.Get("GLOBAL_ADVENTURE_CHOOSE_BUTTON_TEXT"));
			if (!this.m_ChooseButton.IsEnabled())
			{
				this.m_ChooseButton.Enable();
			}
		}
		else
		{
			this.m_ChooseButton.SetText(GameStrings.Get("GLUE_QUEST_LOG_CLASS_LOCKED"));
			this.m_ChooseButton.Disable();
		}
	}

	// Token: 0x0600204F RID: 8271 RVA: 0x0009E8C4 File Offset: 0x0009CAC4
	private void OnBoxTransitionFinished(object userData)
	{
		if (!this.m_isStarted || SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE)
		{
			return;
		}
		if (this.m_ChooseButton.IsEnabled())
		{
			PlayMakerFSM component = this.m_ChooseButton.GetComponent<PlayMakerFSM>();
			if (component != null)
			{
				component.SendEvent("Burst");
			}
		}
		else
		{
			this.ShowDisabledAdventureModeRequirementsWarning();
		}
	}

	// Token: 0x06002050 RID: 8272 RVA: 0x0009E92C File Offset: 0x0009CB2C
	private void ChangeSubScene()
	{
		this.m_AttemptedLoad = true;
		this.m_ChooseButton.SetText(GameStrings.Get("GLUE_LOADING"));
		this.m_ChooseButton.Disable();
		this.m_BackButton.SetEnabled(false);
		base.StartCoroutine(this.WaitThenChangeSubScene());
	}

	// Token: 0x06002051 RID: 8273 RVA: 0x0009E97C File Offset: 0x0009CB7C
	private IEnumerator WaitThenChangeSubScene()
	{
		yield return null;
		AdventureConfig.Get().ChangeSubSceneToSelectedAdventure();
		yield break;
	}

	// Token: 0x06002052 RID: 8274 RVA: 0x0009E990 File Offset: 0x0009CB90
	private void OnBackButton()
	{
		Navigation.GoBack();
	}

	// Token: 0x06002053 RID: 8275 RVA: 0x0009E998 File Offset: 0x0009CB98
	private static bool OnNavigateBack()
	{
		AdventureChooserTray.BackToMainMenu();
		return true;
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x0009E9A0 File Offset: 0x0009CBA0
	private static void BackToMainMenu()
	{
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
	}

	// Token: 0x040011AB RID: 4523
	private const string s_DefaultPortraitMaterialTextureName = "_MainTex";

	// Token: 0x040011AC RID: 4524
	private const int s_DefaultPortraitMaterialIndex = 0;

	// Token: 0x040011AD RID: 4525
	[SerializeField]
	[CustomEditField(Sections = "Sub Scene")]
	public AdventureSubScene m_ParentSubScene;

	// Token: 0x040011AE RID: 4526
	[CustomEditField(Sections = "Description")]
	public UberText m_DescriptionTitleObject;

	// Token: 0x040011AF RID: 4527
	[CustomEditField(Sections = "Description")]
	public GameObject m_DescriptionContainer;

	// Token: 0x040011B0 RID: 4528
	[CustomEditField(Sections = "Choose Frame")]
	[SerializeField]
	public PlayButton m_ChooseButton;

	// Token: 0x040011B1 RID: 4529
	[SerializeField]
	[CustomEditField(Sections = "Choose Frame")]
	public UIBButton m_BackButton;

	// Token: 0x040011B2 RID: 4530
	[CustomEditField(Sections = "Choose Frame")]
	[SerializeField]
	public GameObject m_ChooseElementsContainer;

	// Token: 0x040011B3 RID: 4531
	[CustomEditField(Sections = "Choose Frame", T = EditType.GAME_OBJECT)]
	[SerializeField]
	public string m_DefaultChooserButtonPrefab;

	// Token: 0x040011B4 RID: 4532
	[CustomEditField(Sections = "Choose Frame", T = EditType.GAME_OBJECT)]
	[SerializeField]
	public string m_DefaultChooserSubButtonPrefab;

	// Token: 0x040011B5 RID: 4533
	[CustomEditField(Sections = "Behavior Settings")]
	public bool m_OnlyOneExpands;

	// Token: 0x040011B6 RID: 4534
	[SerializeField]
	private float m_ButtonOffset = -2.5f;

	// Token: 0x040011B7 RID: 4535
	private AdventureChooserSubButton m_SelectedSubButton;

	// Token: 0x040011B8 RID: 4536
	private AdventureChooserDescription m_CurrentChooserDescription;

	// Token: 0x040011B9 RID: 4537
	private List<AdventureChooserButton> m_AdventureButtons = new List<AdventureChooserButton>();

	// Token: 0x040011BA RID: 4538
	private Map<AdventureDbId, Map<AdventureModeDbId, AdventureChooserDescription>> m_Descriptions = new Map<AdventureDbId, Map<AdventureModeDbId, AdventureChooserDescription>>();

	// Token: 0x040011BB RID: 4539
	private bool m_AttemptedLoad;

	// Token: 0x040011BC RID: 4540
	private bool m_isStarted;
}
