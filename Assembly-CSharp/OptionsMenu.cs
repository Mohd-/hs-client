using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000468 RID: 1128
[CustomEditClass]
public class OptionsMenu : MonoBehaviour
{
	// Token: 0x06003748 RID: 14152 RVA: 0x0010EDA8 File Offset: 0x0010CFA8
	private void Awake()
	{
		OptionsMenu.s_instance = this;
		this.NORMAL_SCALE = base.transform.localScale;
		this.HIDDEN_SCALE = 0.01f * this.NORMAL_SCALE;
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_graphicsRes.setItemTextCallback(new DropdownControl.itemTextCallback(this.OnGraphicsResolutionDropdownText));
			this.m_graphicsRes.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.OnNewGraphicsResolution));
			List<GraphicsResolution> goodGraphicsResolution = this.GetGoodGraphicsResolution();
			foreach (GraphicsResolution value in goodGraphicsResolution)
			{
				this.m_graphicsRes.addItem(value);
			}
			this.m_graphicsRes.setSelection(this.GetCurrentGraphicsResolution());
			this.m_fullScreenCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnToggleFullScreenCheckbox));
			this.m_fullScreenCheckbox.SetChecked(Options.Get().GetBool(Option.GFX_FULLSCREEN, Screen.fullScreen));
			this.m_graphicsQuality.addItem(GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_LOW"));
			this.m_graphicsQuality.addItem(GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_MEDIUM"));
			this.m_graphicsQuality.addItem(GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_HIGH"));
			this.m_graphicsQuality.setSelection(this.GetCurrentGraphicsQuality());
			this.m_graphicsQuality.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.OnNewGraphicsQuality));
		}
		this.m_masterVolume.SetValue(Options.Get().GetFloat(Option.SOUND_VOLUME));
		this.m_masterVolume.SetUpdateHandler(new ScrollbarControl.UpdateHandler(this.OnNewMasterVolume));
		this.m_masterVolume.SetFinishHandler(new ScrollbarControl.FinishHandler(this.OnMasterVolumeRelease));
		this.m_musicVolume.SetValue(Options.Get().GetFloat(Option.MUSIC_VOLUME));
		this.m_musicVolume.SetUpdateHandler(new ScrollbarControl.UpdateHandler(this.OnNewMusicVolume));
		if (this.m_backgroundSound != null)
		{
			this.m_backgroundSound.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleBackgroundSound));
			this.m_backgroundSound.SetChecked(Options.Get().GetBool(Option.BACKGROUND_SOUND));
		}
		this.UpdateCreditsUI();
		this.m_nearbyPlayers.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleNearbyPlayers));
		this.m_nearbyPlayers.SetChecked(Options.Get().GetBool(Option.NEARBY_PLAYERS));
		this.m_spectatorOpenJoinCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleSpectatorOpenJoin));
		this.m_spectatorOpenJoinCheckbox.SetChecked(Options.Get().GetBool(Option.SPECTATOR_OPEN_JOIN));
		this.m_languageGroup.gameObject.SetActive(this.LANGUAGE_SELECTION);
		if (this.LANGUAGE_SELECTION)
		{
			if (Localization.GetLocale() == Locale.jaJP)
			{
				this.m_languageDropdown.setFontWithoutLocalization(this.m_languageDropdownFont);
			}
			else
			{
				this.m_languageDropdown.setFont(this.m_languageDropdownFont.m_Font);
			}
			foreach (object obj in Enum.GetValues(typeof(Locale)))
			{
				Locale locale = (Locale)((int)obj);
				if (locale != Locale.UNKNOWN)
				{
					if (locale != Locale.enGB)
					{
						this.m_languageDropdown.addItem(GameStrings.Get(this.StringNameFromLocale(locale)));
					}
				}
			}
			this.m_languageDropdown.setSelection(this.GetCurrentLanguage());
			this.m_languageDropdown.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.OnNewLanguage));
		}
		if (AssetLoader.DOWNLOADABLE_LANGUAGE_PACKS && ApplicationMgr.IsInternal())
		{
			this.m_languagePackCheckbox.gameObject.SetActive(true);
			this.m_languagePackCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleLanguagePackCheckbox));
			this.m_languagePackCheckbox.SetChecked(Downloader.Get().AllLocalizedAudioBundlesDownloaded());
		}
		this.m_creditsButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCreditsButtonReleased));
		this.m_cinematicButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCinematicButtonReleased));
		this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.Hide(true);
		});
		this.ShowOrHide(false);
		this.m_leftPane.UpdateSlices();
		this.m_rightPane.UpdateSlices();
		this.m_middlePane.UpdateSlices();
	}

	// Token: 0x06003749 RID: 14153 RVA: 0x0010F248 File Offset: 0x0010D448
	public void OnDestroy()
	{
		if (FatalErrorMgr.Get() != null)
		{
			FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		}
		OptionsMenu.s_instance = null;
	}

	// Token: 0x0600374A RID: 14154 RVA: 0x0010F271 File Offset: 0x0010D471
	public static OptionsMenu Get()
	{
		return OptionsMenu.s_instance;
	}

	// Token: 0x0600374B RID: 14155 RVA: 0x0010F278 File Offset: 0x0010D478
	public OptionsMenu.hideHandler GetHideHandler()
	{
		return this.m_hideHandler;
	}

	// Token: 0x0600374C RID: 14156 RVA: 0x0010F280 File Offset: 0x0010D480
	public void SetHideHandler(OptionsMenu.hideHandler handler)
	{
		this.m_hideHandler = handler;
	}

	// Token: 0x0600374D RID: 14157 RVA: 0x0010F289 File Offset: 0x0010D489
	public void RemoveHideHandler(OptionsMenu.hideHandler handler)
	{
		if (this.m_hideHandler == handler)
		{
			this.m_hideHandler = null;
		}
	}

	// Token: 0x0600374E RID: 14158 RVA: 0x0010F2A3 File Offset: 0x0010D4A3
	public bool IsShown()
	{
		return this.m_isShown;
	}

	// Token: 0x0600374F RID: 14159 RVA: 0x0010F2AC File Offset: 0x0010D4AC
	public void Show()
	{
		this.UpdateCreditsUI();
		this.ShowOrHide(true);
		AnimationUtil.ShowWithPunch(base.gameObject, this.HIDDEN_SCALE, 1.1f * this.NORMAL_SCALE, this.NORMAL_SCALE, null, true, null, null, null);
	}

	// Token: 0x06003750 RID: 14160 RVA: 0x0010F2F4 File Offset: 0x0010D4F4
	public void Hide(bool callHideHandler = true)
	{
		this.ShowOrHide(false);
		if (this.m_hideHandler != null && callHideHandler)
		{
			this.m_hideHandler();
			this.m_hideHandler = null;
		}
	}

	// Token: 0x06003751 RID: 14161 RVA: 0x0010F32C File Offset: 0x0010D52C
	private GraphicsResolution GetCurrentGraphicsResolution()
	{
		int @int = Options.Get().GetInt(Option.GFX_WIDTH, Screen.currentResolution.width);
		int int2 = Options.Get().GetInt(Option.GFX_HEIGHT, Screen.currentResolution.height);
		return GraphicsResolution.create(@int, int2);
	}

	// Token: 0x06003752 RID: 14162 RVA: 0x0010F374 File Offset: 0x0010D574
	private string GetCurrentGraphicsQuality()
	{
		switch (Options.Get().GetInt(Option.GFX_QUALITY))
		{
		case 0:
			return GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_LOW");
		case 1:
			return GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_MEDIUM");
		case 2:
			return GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_HIGH");
		default:
			return GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_LOW");
		}
	}

	// Token: 0x06003753 RID: 14163 RVA: 0x0010F3D4 File Offset: 0x0010D5D4
	private List<GraphicsResolution> GetGoodGraphicsResolution()
	{
		if (this.m_fullScreenResolutions.Count == 0)
		{
			List<GraphicsResolution> list = GraphicsResolution.list;
			foreach (GraphicsResolution graphicsResolution in list)
			{
				if (graphicsResolution.x >= 1024)
				{
					if (graphicsResolution.y >= 728)
					{
						if ((double)graphicsResolution.aspectRatio - 0.01 <= 1.7777777777777777)
						{
							if ((double)graphicsResolution.aspectRatio + 0.01 >= 1.3333333333333333)
							{
								this.m_fullScreenResolutions.Add(graphicsResolution);
							}
						}
					}
				}
			}
		}
		return this.m_fullScreenResolutions;
	}

	// Token: 0x06003754 RID: 14164 RVA: 0x0010F4C8 File Offset: 0x0010D6C8
	private string GetCurrentLanguage()
	{
		return GameStrings.Get(this.StringNameFromLocale(Localization.GetLocale()));
	}

	// Token: 0x06003755 RID: 14165 RVA: 0x0010F4DC File Offset: 0x0010D6DC
	private void ShowOrHide(bool showOrHide)
	{
		this.m_isShown = showOrHide;
		base.gameObject.SetActive(showOrHide);
		this.m_leftPane.UpdateSlices();
		this.m_rightPane.UpdateSlices();
		this.m_middlePane.UpdateSlices();
	}

	// Token: 0x06003756 RID: 14166 RVA: 0x0010F51D File Offset: 0x0010D71D
	private string StringNameFromLocale(Locale locale)
	{
		return "GLOBAL_LANGUAGE_NATIVE_" + locale.ToString().ToUpper();
	}

	// Token: 0x06003757 RID: 14167 RVA: 0x0010F539 File Offset: 0x0010D739
	private void UpdateCreditsUI()
	{
		this.m_miscGroup.SetActive(this.CanShowCredits());
	}

	// Token: 0x06003758 RID: 14168 RVA: 0x0010F54C File Offset: 0x0010D74C
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		this.Hide(true);
	}

	// Token: 0x06003759 RID: 14169 RVA: 0x0010F558 File Offset: 0x0010D758
	private void OnToggleFullScreenCheckbox(UIEvent e)
	{
		GraphicsResolution graphicsResolution = this.m_graphicsRes.getSelection() as GraphicsResolution;
		if (graphicsResolution == null)
		{
			this.m_graphicsRes.setSelectionToLastItem();
			graphicsResolution = (this.m_graphicsRes.getSelection() as GraphicsResolution);
		}
		if (graphicsResolution == null)
		{
			return;
		}
		GraphicsManager.Get().SetScreenResolution(graphicsResolution.x, graphicsResolution.y, this.m_fullScreenCheckbox.IsChecked());
		Options.Get().SetBool(Option.GFX_FULLSCREEN, this.m_fullScreenCheckbox.IsChecked());
	}

	// Token: 0x0600375A RID: 14170 RVA: 0x0010F5D8 File Offset: 0x0010D7D8
	private void OnNewGraphicsQuality(object selection, object prevSelection)
	{
		GraphicsQuality graphicsQuality = GraphicsQuality.Low;
		string text = (string)selection;
		if (text == GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_LOW"))
		{
			graphicsQuality = GraphicsQuality.Low;
		}
		else if (text == GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_MEDIUM"))
		{
			graphicsQuality = GraphicsQuality.Medium;
		}
		else if (text == GameStrings.Get("GLOBAL_OPTIONS_GRAPHICS_QUALITY_HIGH"))
		{
			graphicsQuality = GraphicsQuality.High;
		}
		Log.Kyle.Print("Graphics Quality: " + graphicsQuality.ToString(), new object[0]);
		GraphicsManager.Get().RenderQualityLevel = graphicsQuality;
	}

	// Token: 0x0600375B RID: 14171 RVA: 0x0010F670 File Offset: 0x0010D870
	private void OnNewGraphicsResolution(object selection, object prevSelection)
	{
		GraphicsResolution graphicsResolution = (GraphicsResolution)selection;
		GraphicsManager.Get().SetScreenResolution(graphicsResolution.x, graphicsResolution.y, this.m_fullScreenCheckbox.IsChecked());
		Options.Get().SetInt(Option.GFX_WIDTH, graphicsResolution.x);
		Options.Get().SetInt(Option.GFX_HEIGHT, graphicsResolution.y);
	}

	// Token: 0x0600375C RID: 14172 RVA: 0x0010F6CC File Offset: 0x0010D8CC
	private void OnNewLanguage(object selection, object prevSelection)
	{
		if (selection == prevSelection)
		{
			return;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLOBAL_LANGUAGE_CHANGE_CONFIRM_TITLE");
		popupInfo.m_text = GameStrings.Get("GLOBAL_LANGUAGE_CHANGE_CONFIRM_MESSAGE");
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnChangeLanguageConfirmationResponse);
		popupInfo.m_responseUserData = selection;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x0600375D RID: 14173 RVA: 0x0010F73C File Offset: 0x0010D93C
	private void OnChangeLanguageConfirmationResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CANCEL)
		{
			this.m_languageDropdown.setSelection(this.GetCurrentLanguage());
			return;
		}
		string text = null;
		string text2 = (string)userData;
		foreach (object obj in Enum.GetValues(typeof(Locale)))
		{
			Locale locale = (Locale)((int)obj);
			if (text2 == GameStrings.Get(this.StringNameFromLocale(locale)))
			{
				text = locale.ToString();
				break;
			}
		}
		if (text == null)
		{
			Debug.LogError(string.Format("OptionsMenu.OnChangeLanguageConfirmationResponse() - locale not found", new object[0]));
			return;
		}
		Localization.SetLocaleName(text);
		Options.Get().SetString(Option.LOCALE, text);
		this.Hide(false);
		ApplicationMgr.Get().Reset();
	}

	// Token: 0x0600375E RID: 14174 RVA: 0x0010F830 File Offset: 0x0010DA30
	private void ToggleLanguagePackCheckbox(UIEvent e)
	{
		if (this.m_languagePackCheckbox.IsChecked())
		{
			Downloader.Get().DownloadLocalizedBundles();
		}
		else
		{
			Downloader.Get().DeleteLocalizedBundles();
		}
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x0010F85C File Offset: 0x0010DA5C
	private string OnGraphicsResolutionDropdownText(object val)
	{
		GraphicsResolution graphicsResolution = (GraphicsResolution)val;
		return string.Format("{0} x {1}", graphicsResolution.x, graphicsResolution.y);
	}

	// Token: 0x06003760 RID: 14176 RVA: 0x0010F890 File Offset: 0x0010DA90
	private void OnNewMasterVolume(float newVolume)
	{
		Options.Get().SetFloat(Option.SOUND_VOLUME, newVolume);
	}

	// Token: 0x06003761 RID: 14177 RVA: 0x0010F8A0 File Offset: 0x0010DAA0
	private void OnMasterVolumeRelease()
	{
		SoundManager.LoadedCallback callback = delegate(AudioSource source, object userData)
		{
			SoundManager.Get().Set3d(source, false);
		};
		SoundManager.Get().LoadAndPlay("UI_MouseClick_01", base.gameObject, 1f, callback);
	}

	// Token: 0x06003762 RID: 14178 RVA: 0x0010F8E6 File Offset: 0x0010DAE6
	private void OnNewMusicVolume(float newVolume)
	{
		Options.Get().SetFloat(Option.MUSIC_VOLUME, newVolume);
	}

	// Token: 0x06003763 RID: 14179 RVA: 0x0010F8F4 File Offset: 0x0010DAF4
	private void ToggleBackgroundSound(UIEvent e)
	{
		Options.Get().SetBool(Option.BACKGROUND_SOUND, this.m_backgroundSound.IsChecked());
	}

	// Token: 0x06003764 RID: 14180 RVA: 0x0010F90D File Offset: 0x0010DB0D
	private void OnCreditsButtonReleased(UIEvent e)
	{
		this.Hide(false);
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.CREDITS);
	}

	// Token: 0x06003765 RID: 14181 RVA: 0x0010F924 File Offset: 0x0010DB24
	private void OnCinematicButtonReleased(UIEvent e)
	{
		Cinematic componentInChildren = SceneMgr.Get().GetComponentInChildren<Cinematic>();
		if (componentInChildren)
		{
			this.Hide(false);
			componentInChildren.Play(new Cinematic.MovieCallback(GameMenu.Get().ShowOptionsMenu));
		}
		else
		{
			Debug.LogWarning("Failed to locate Cinematic component on SceneMgr!");
		}
	}

	// Token: 0x06003766 RID: 14182 RVA: 0x0010F973 File Offset: 0x0010DB73
	private void ToggleNearbyPlayers(UIEvent e)
	{
		Options.Get().SetBool(Option.NEARBY_PLAYERS, this.m_nearbyPlayers.IsChecked());
	}

	// Token: 0x06003767 RID: 14183 RVA: 0x0010F98C File Offset: 0x0010DB8C
	private void ToggleSpectatorOpenJoin(UIEvent e)
	{
		Options.Get().SetBool(Option.SPECTATOR_OPEN_JOIN, this.m_spectatorOpenJoinCheckbox.IsChecked());
	}

	// Token: 0x06003768 RID: 14184 RVA: 0x0010F9A8 File Offset: 0x0010DBA8
	private bool CanShowCredits()
	{
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		SceneMgr.Mode mode2 = mode;
		switch (mode2)
		{
		case SceneMgr.Mode.GAMEPLAY:
		case SceneMgr.Mode.PACKOPENING:
			break;
		default:
			switch (mode2)
			{
			case SceneMgr.Mode.CREDITS:
			case SceneMgr.Mode.ADVENTURE:
				return false;
			}
			return (!(GeneralStore.Get() != null) || !GeneralStore.Get().IsShown()) && !Network.Get().IsFindingGame() && GameUtils.AreAllTutorialsComplete() && !(WelcomeQuests.Get() != null) && (!(ArenaStore.Get() != null) || !ArenaStore.Get().IsShown()) && (!(DraftDisplay.Get() != null) || DraftDisplay.Get().GetDraftMode() != DraftDisplay.DraftMode.IN_REWARDS);
		}
		return false;
	}

	// Token: 0x04002299 RID: 8857
	[CustomEditField(Sections = "Layout")]
	public MultiSliceElement m_leftPane;

	// Token: 0x0400229A RID: 8858
	[CustomEditField(Sections = "Layout")]
	public MultiSliceElement m_rightPane;

	// Token: 0x0400229B RID: 8859
	[CustomEditField(Sections = "Layout")]
	public MultiSliceElement m_middlePane;

	// Token: 0x0400229C RID: 8860
	[CustomEditField(Sections = "Graphics")]
	public GameObject m_graphicsGroup;

	// Token: 0x0400229D RID: 8861
	[CustomEditField(Sections = "Graphics")]
	public DropdownControl m_graphicsRes;

	// Token: 0x0400229E RID: 8862
	[CustomEditField(Sections = "Graphics")]
	public DropdownControl m_graphicsQuality;

	// Token: 0x0400229F RID: 8863
	[CustomEditField(Sections = "Graphics")]
	public CheckBox m_fullScreenCheckbox;

	// Token: 0x040022A0 RID: 8864
	[CustomEditField(Sections = "Sound")]
	public GameObject m_soundGroup;

	// Token: 0x040022A1 RID: 8865
	[CustomEditField(Sections = "Sound")]
	public ScrollbarControl m_masterVolume;

	// Token: 0x040022A2 RID: 8866
	[CustomEditField(Sections = "Sound")]
	public ScrollbarControl m_musicVolume;

	// Token: 0x040022A3 RID: 8867
	[CustomEditField(Sections = "Sound")]
	public CheckBox m_backgroundSound;

	// Token: 0x040022A4 RID: 8868
	[CustomEditField(Sections = "Misc")]
	public GameObject m_miscGroup;

	// Token: 0x040022A5 RID: 8869
	[CustomEditField(Sections = "Misc")]
	public UIBButton m_creditsButton;

	// Token: 0x040022A6 RID: 8870
	[CustomEditField(Sections = "Misc")]
	public UIBButton m_cinematicButton;

	// Token: 0x040022A7 RID: 8871
	[CustomEditField(Sections = "Preferences")]
	public CheckBox m_nearbyPlayers;

	// Token: 0x040022A8 RID: 8872
	[CustomEditField(Sections = "Preferences")]
	public CheckBox m_spectatorOpenJoinCheckbox;

	// Token: 0x040022A9 RID: 8873
	[CustomEditField(Sections = "Language")]
	public GameObject m_languageGroup;

	// Token: 0x040022AA RID: 8874
	[CustomEditField(Sections = "Language")]
	public DropdownControl m_languageDropdown;

	// Token: 0x040022AB RID: 8875
	[CustomEditField(Sections = "Language")]
	public FontDef m_languageDropdownFont;

	// Token: 0x040022AC RID: 8876
	[CustomEditField(Sections = "Language")]
	public CheckBox m_languagePackCheckbox;

	// Token: 0x040022AD RID: 8877
	[CustomEditField(Sections = "Internal Stuff")]
	public PegUIElement m_inputBlocker;

	// Token: 0x040022AE RID: 8878
	[CustomEditField(Sections = "Internal Stuff")]
	public UberText m_versionLabel;

	// Token: 0x040022AF RID: 8879
	private static OptionsMenu s_instance;

	// Token: 0x040022B0 RID: 8880
	private bool m_isShown;

	// Token: 0x040022B1 RID: 8881
	private OptionsMenu.hideHandler m_hideHandler;

	// Token: 0x040022B2 RID: 8882
	private List<GraphicsResolution> m_fullScreenResolutions = new List<GraphicsResolution>();

	// Token: 0x040022B3 RID: 8883
	private Vector3 NORMAL_SCALE;

	// Token: 0x040022B4 RID: 8884
	private Vector3 HIDDEN_SCALE;

	// Token: 0x040022B5 RID: 8885
	private readonly PlatformDependentValue<bool> LANGUAGE_SELECTION = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		iOS = true,
		Android = true,
		PC = false,
		Mac = false
	};

	// Token: 0x020004EB RID: 1259
	// (Invoke) Token: 0x06003B2E RID: 15150
	public delegate void hideHandler();
}
