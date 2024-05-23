using System;
using System.Collections.Generic;
using bgs;
using UnityEngine;

// Token: 0x02000469 RID: 1129
[CustomEditClass]
public class GameMenu : ButtonListMenu
{
	// Token: 0x0600376C RID: 14188 RVA: 0x0010FB1C File Offset: 0x0010DD1C
	protected override void Awake()
	{
		this.m_menuParent = this.m_menuBone;
		base.Awake();
		GameMenu.s_instance = this;
		this.LoadRatings();
		this.m_concedeButton = base.CreateMenuButton("ConcedeButton", "GLOBAL_CONCEDE", new UIEvent.Handler(this.ConcedeButtonPressed));
		if (ApplicationMgr.CanQuitGame)
		{
			this.m_quitButton = base.CreateMenuButton("QuitButton", "GLOBAL_QUIT", new UIEvent.Handler(this.QuitButtonPressed));
		}
		else
		{
			this.m_quitButton = base.CreateMenuButton("LogoutButton", (!Network.ShouldBeConnectedToAurora()) ? "GLOBAL_LOGIN" : "GLOBAL_LOGOUT", new UIEvent.Handler(this.LogoutButtonPressed));
		}
		this.m_resumeButton = base.CreateMenuButton("ResumeButton", "GLOBAL_RESUME_GAME", new UIEvent.Handler(this.ResumeButtonPressed));
		this.m_optionsButton = base.CreateMenuButton("OptionsButton", "GLOBAL_OPTIONS", new UIEvent.Handler(this.OptionsButtonPressed));
		this.m_menu.m_headerText.Text = GameStrings.Get("GLOBAL_GAME_MENU");
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x0010FC34 File Offset: 0x0010DE34
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.m_optionsMenu != null)
		{
			this.m_optionsMenu.RemoveHideHandler(new OptionsMenu.hideHandler(this.OnOptionsMenuHidden));
		}
		GameMenu.s_instance = null;
	}

	// Token: 0x0600376E RID: 14190 RVA: 0x0010FC75 File Offset: 0x0010DE75
	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600376F RID: 14191 RVA: 0x0010FC83 File Offset: 0x0010DE83
	public static GameMenu Get()
	{
		return GameMenu.s_instance;
	}

	// Token: 0x06003770 RID: 14192 RVA: 0x0010FC8C File Offset: 0x0010DE8C
	public override void Show()
	{
		if (OptionsMenu.Get() != null && OptionsMenu.Get().IsShown())
		{
			UniversalInputManager.Get().CancelTextInput(base.gameObject, true);
			OptionsMenu.Get().Hide(true);
			return;
		}
		base.Show();
		if (UniversalInputManager.UsePhoneUI && this.m_ratingsObject != null)
		{
			this.m_ratingsObject.SetActive(this.UseKoreanRating());
			this.m_menu.m_buttonContainer.UpdateSlices();
			base.LayoutMenuBackground();
		}
		this.ShowLoginTooltipIfNeeded();
		BnetBar.Get().m_menuButton.SetSelected(true);
	}

	// Token: 0x06003771 RID: 14193 RVA: 0x0010FD38 File Offset: 0x0010DF38
	public override void Hide()
	{
		base.Hide();
		this.HideLoginTooltip();
		BnetBar.Get().m_menuButton.SetSelected(false);
	}

	// Token: 0x06003772 RID: 14194 RVA: 0x0010FD58 File Offset: 0x0010DF58
	public void ShowLoginTooltipIfNeeded()
	{
		if (Network.ShouldBeConnectedToAurora() || this.m_hasSeenLoginTooltip || this.m_quitButton == null)
		{
			return;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			Vector3 position;
			position..ctor(-82.9f, 42.1f, 17.2f);
			this.m_loginButtonPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, this.BUTTON_SCALE_PHONE, GameStrings.Get("GLOBAL_MOBILE_LOG_IN_TOOLTIP"), false);
		}
		else
		{
			Vector3 position;
			position..ctor(-46.9f, 34.2f, 9.4f);
			this.m_loginButtonPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, this.BUTTON_SCALE, GameStrings.Get("GLOBAL_MOBILE_LOG_IN_TOOLTIP"), false);
		}
		if (this.m_loginButtonPopup != null)
		{
			this.m_loginButtonPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
			this.m_hasSeenLoginTooltip = true;
		}
	}

	// Token: 0x06003773 RID: 14195 RVA: 0x0010FE38 File Offset: 0x0010E038
	public void HideLoginTooltip()
	{
		if (this.m_loginButtonPopup != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_loginButtonPopup);
		}
		this.m_loginButtonPopup = null;
	}

	// Token: 0x06003774 RID: 14196 RVA: 0x0010FE70 File Offset: 0x0010E070
	public bool IsInGameMenu()
	{
		return SceneMgr.Get().IsInGame() && SceneMgr.Get().IsSceneLoaded() && !LoadingScreen.Get().IsTransitioning() && GameState.Get() != null && (GameState.Get().IsConcedingAllowed() || GameMgr.Get().IsSpectator()) && !GameState.Get().IsGameOver() && (!(TutorialProgressScreen.Get() != null) || !TutorialProgressScreen.Get().gameObject.activeInHierarchy);
	}

	// Token: 0x06003775 RID: 14197 RVA: 0x0010FF10 File Offset: 0x0010E110
	public void ShowOptionsMenu()
	{
		this.Hide();
		if (this.m_optionsMenu == null)
		{
			GameObject gameObject = AssetLoader.Get().LoadGameObject(this.OPTIONS_MENU_NAME, true, false);
			this.m_optionsMenu = gameObject.GetComponent<OptionsMenu>();
			if (this.m_optionsMenu != null)
			{
				this.SwitchToOptionsMenu();
			}
		}
		else
		{
			this.SwitchToOptionsMenu();
		}
	}

	// Token: 0x06003776 RID: 14198 RVA: 0x0010FF7C File Offset: 0x0010E17C
	protected override List<UIBButton> GetButtons()
	{
		List<UIBButton> list = new List<UIBButton>();
		if (this.IsInGameMenu())
		{
			list.Add(this.m_concedeButton);
			list.Add(null);
		}
		if (!DemoMgr.Get().IsExpoDemo())
		{
			list.Add(this.m_optionsButton);
			list.Add(this.m_quitButton);
			list.Add(null);
		}
		list.Add(this.m_resumeButton);
		return list;
	}

	// Token: 0x06003777 RID: 14199 RVA: 0x0010FFE8 File Offset: 0x0010E1E8
	protected override void LayoutMenu()
	{
		base.LayoutMenuButtons();
		if (this.m_ratingsObject != null)
		{
			this.m_menu.m_buttonContainer.AddSlice(this.m_ratingsObject, Vector3.zero, this.m_ratingsObjectMinPadding, false);
		}
		this.m_menu.m_buttonContainer.UpdateSlices();
		if (this.m_concedeButton != null)
		{
			string text = GameStrings.Get((!GameMgr.Get().IsSpectator()) ? "GLOBAL_CONCEDE" : "GLOBAL_LEAVE_SPECTATOR_MODE");
			this.m_concedeButton.SetText(text);
		}
		base.LayoutMenuBackground();
	}

	// Token: 0x06003778 RID: 14200 RVA: 0x00110085 File Offset: 0x0010E285
	private void QuitButtonPressed(UIEvent e)
	{
		Network.AutoConcede();
		ApplicationMgr.Get().Exit();
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x00110096 File Offset: 0x0010E296
	private void LogoutButtonPressed(UIEvent e)
	{
		this.HideLoginTooltip();
		GameUtils.LogoutConfirmation();
		this.Hide();
	}

	// Token: 0x0600377A RID: 14202 RVA: 0x001100AC File Offset: 0x0010E2AC
	private void ConcedeButtonPressed(UIEvent e)
	{
		if (GameMgr.Get() != null && GameMgr.Get().IsSpectator())
		{
			SpectatorManager.Get().LeaveSpectatorMode();
		}
		else if (GameState.Get() != null)
		{
			GameState.Get().Concede();
		}
		this.Hide();
	}

	// Token: 0x0600377B RID: 14203 RVA: 0x001100FB File Offset: 0x0010E2FB
	private void ResumeButtonPressed(UIEvent e)
	{
		this.Hide();
	}

	// Token: 0x0600377C RID: 14204 RVA: 0x00110103 File Offset: 0x0010E303
	private void OptionsButtonPressed(UIEvent e)
	{
		this.ShowOptionsMenu();
	}

	// Token: 0x0600377D RID: 14205 RVA: 0x0011010C File Offset: 0x0010E30C
	private void SwitchToOptionsMenu()
	{
		this.m_optionsMenu.SetHideHandler(new OptionsMenu.hideHandler(this.OnOptionsMenuHidden));
		this.m_optionsMenu.Show();
	}

	// Token: 0x0600377E RID: 14206 RVA: 0x0011013C File Offset: 0x0010E33C
	private void OnOptionsMenuHidden()
	{
		Object.Destroy(this.m_optionsMenu.gameObject);
		this.m_optionsMenu = null;
		AssetCache.ClearGameObject(this.OPTIONS_MENU_NAME);
		if (!SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FATAL_ERROR) && !ApplicationMgr.Get().IsResetting() && BnetBar.Get().IsEnabled())
		{
			this.Show();
		}
	}

	// Token: 0x0600377F RID: 14207 RVA: 0x001101A8 File Offset: 0x0010E3A8
	private bool UseKoreanRating()
	{
		if (SceneMgr.Get().IsInGame())
		{
			return false;
		}
		string accountCountry = BattleNet.GetAccountCountry();
		return accountCountry == "KOR";
	}

	// Token: 0x06003780 RID: 14208 RVA: 0x001101DC File Offset: 0x0010E3DC
	private void LoadRatings()
	{
		if (this.UseKoreanRating())
		{
			AssetLoader.Get().LoadGameObject("Korean_Ratings_OptionsScreen", delegate(string name, GameObject go, object data)
			{
				if (go == null)
				{
					return;
				}
				Quaternion localRotation = go.transform.localRotation;
				GameUtils.SetParent(go, this.m_menu.m_buttonContainer, false);
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = localRotation;
				this.m_ratingsObject = go;
				this.LayoutMenu();
			}, null, false);
		}
	}

	// Token: 0x040022B7 RID: 8887
	[CustomEditField(Sections = "Template Items")]
	public Vector3 m_ratingsObjectMinPadding = new Vector3(0f, 0f, -0.06f);

	// Token: 0x040022B8 RID: 8888
	public Transform m_menuBone;

	// Token: 0x040022B9 RID: 8889
	private PlatformDependentValue<string> OPTIONS_MENU_NAME = new PlatformDependentValue<string>(PlatformCategory.Screen)
	{
		PC = "OptionsMenu",
		Phone = "OptionsMenu_phone"
	};

	// Token: 0x040022BA RID: 8890
	private static GameMenu s_instance;

	// Token: 0x040022BB RID: 8891
	private OptionsMenu m_optionsMenu;

	// Token: 0x040022BC RID: 8892
	private UIBButton m_concedeButton;

	// Token: 0x040022BD RID: 8893
	private UIBButton m_quitButton;

	// Token: 0x040022BE RID: 8894
	private UIBButton m_resumeButton;

	// Token: 0x040022BF RID: 8895
	private UIBButton m_optionsButton;

	// Token: 0x040022C0 RID: 8896
	private Notification m_loginButtonPopup;

	// Token: 0x040022C1 RID: 8897
	private bool m_hasSeenLoginTooltip;

	// Token: 0x040022C2 RID: 8898
	private constants.BnetRegion m_AccountRegion;

	// Token: 0x040022C3 RID: 8899
	private GameObject m_ratingsObject;

	// Token: 0x040022C4 RID: 8900
	private readonly Vector3 BUTTON_SCALE = 15f * Vector3.one;

	// Token: 0x040022C5 RID: 8901
	private readonly Vector3 BUTTON_SCALE_PHONE = 25f * Vector3.one;
}
