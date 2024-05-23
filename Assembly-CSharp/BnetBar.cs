using System;
using System.Collections;
using System.Linq;
using bgs;
using UnityEngine;

// Token: 0x020004B7 RID: 1207
[CustomEditClass]
public class BnetBar : MonoBehaviour
{
	// Token: 0x06003992 RID: 14738 RVA: 0x00117560 File Offset: 0x00115760
	private void Awake()
	{
		BnetBar.s_instance = this;
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_menuButton.transform.localScale *= 2f;
			this.m_friendButton.transform.localScale *= 2f;
		}
		else
		{
			this.m_connectionIndicator.gameObject.SetActive(false);
		}
		this.m_initialWidth = base.GetComponent<Renderer>().bounds.size.x;
		this.m_initialFriendButtonScaleX = this.m_friendButton.transform.localScale.x;
		this.m_initialMenuButtonScaleX = this.m_menuButton.transform.localScale.x;
		this.m_menuButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnMenuButtonReleased));
		this.m_friendButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnFriendButtonReleased));
		this.ToggleEnableButtons(false);
		this.m_batteryLevel.gameObject.SetActive(false);
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		SpectatorManager.Get().OnInviteReceived += this.SpectatorManager_OnInviteReceived;
		SpectatorManager.Get().OnSpectatorToMyGame += this.SpectatorManager_OnSpectatorToMyGame;
		SpectatorManager.Get().OnSpectatorModeChanged += this.SpectatorManager_OnSpectatorModeChanged;
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
		this.m_lightingBlend = this.m_menuButtonMesh.GetComponent<Renderer>().material.GetFloat("_LightingBlend");
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_batteryLevel = this.m_batteryLevelPhone;
			this.m_currentTime.gameObject.SetActive(false);
		}
		this.m_menuButton.SetPhoneStatusBarState(0);
	}

	// Token: 0x06003993 RID: 14739 RVA: 0x0011775C File Offset: 0x0011595C
	private void OnDestroy()
	{
		SpectatorManager.Get().OnInviteReceived -= this.SpectatorManager_OnInviteReceived;
		SpectatorManager.Get().OnSpectatorToMyGame -= this.SpectatorManager_OnSpectatorToMyGame;
		SpectatorManager.Get().OnSpectatorModeChanged -= this.SpectatorManager_OnSpectatorModeChanged;
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		BnetBar.s_instance = null;
	}

	// Token: 0x06003994 RID: 14740 RVA: 0x001177C8 File Offset: 0x001159C8
	private void Start()
	{
		this.m_friendButton.gameObject.SetActive(false);
		this.m_hasUnacknowledgedPendingInvites = SpectatorManager.Get().HasAnyReceivedInvites();
		if (this.m_friendButton != null)
		{
			this.m_friendButton.ShowPendingInvitesIcon(this.m_hasUnacknowledgedPendingInvites);
		}
		this.ToggleActive(false);
	}

	// Token: 0x06003995 RID: 14741 RVA: 0x00117820 File Offset: 0x00115A20
	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (realtimeSinceStartup - this.m_lastClockUpdate > 1f)
		{
			this.m_lastClockUpdate = realtimeSinceStartup;
			if (Localization.GetLocale() == Locale.enGB)
			{
				this.m_currentTime.Text = string.Format("{0:HH:mm}", DateTime.Now);
			}
			else
			{
				this.m_currentTime.Text = GameStrings.Format("GLOBAL_CURRENT_TIME", new object[]
				{
					DateTime.Now
				});
			}
		}
	}

	// Token: 0x06003996 RID: 14742 RVA: 0x001178A3 File Offset: 0x00115AA3
	public static BnetBar Get()
	{
		return BnetBar.s_instance;
	}

	// Token: 0x06003997 RID: 14743 RVA: 0x001178AC File Offset: 0x00115AAC
	private void WillReset()
	{
		if (this.m_gameMenu != null)
		{
			if (this.m_gameMenu.IsShown())
			{
				this.m_gameMenu.Hide();
			}
			Object.DestroyImmediate(this.m_gameMenu.gameObject);
			this.m_gameMenu = null;
		}
		this.DestroyLoginTooltip();
		this.ToggleActive(false);
		this.m_isLoggedIn = false;
	}

	// Token: 0x06003998 RID: 14744 RVA: 0x00117910 File Offset: 0x00115B10
	public void OnLoggedIn()
	{
		if (Network.ShouldBeConnectedToAurora())
		{
			this.m_friendButton.gameObject.SetActive(true);
		}
		this.m_isLoggedIn = true;
		this.ToggleActive(true);
		this.Update();
		this.UpdateLayout();
	}

	// Token: 0x06003999 RID: 14745 RVA: 0x00117954 File Offset: 0x00115B54
	public void UpdateLayout()
	{
		if (!this.m_isLoggedIn)
		{
			return;
		}
		float num = 0.5f;
		Bounds nearClipBounds = CameraUtils.GetNearClipBounds(PegUI.Get().orthographicUICam);
		float num2 = (nearClipBounds.size.x + num) / this.m_initialWidth;
		TransformUtil.SetLocalPosX(base.gameObject, nearClipBounds.min.x - base.transform.parent.localPosition.x - num);
		TransformUtil.SetLocalScaleX(base.gameObject, num2);
		Vector3 zero = Vector3.zero;
		float num3 = -0.03f * num2;
		if (GeneralUtils.IsDevelopmentBuildTextVisible())
		{
			num3 -= CameraUtils.ScreenToWorldDist(PegUI.Get().orthographicUICam, 115f);
		}
		float num4 = 1f * base.transform.localScale.y;
		bool active = true;
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY && !DemoMgr.Get().IsHubEscMenuEnabled())
		{
			active = false;
		}
		this.m_menuButton.gameObject.SetActive(active);
		TransformUtil.SetLocalScaleX(this.m_menuButton, this.m_initialMenuButtonScaleX / num2);
		TransformUtil.SetPoint(this.m_menuButton, Anchor.RIGHT, base.gameObject, Anchor.RIGHT, new Vector3(num3, num4, 0f) - zero);
		float y = 1.5f;
		float num5 = -4f;
		if (UniversalInputManager.UsePhoneUI)
		{
			y = 190f;
			num5 = 0f;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			TransformUtil.SetPoint(this.m_menuButton, Anchor.RIGHT, base.gameObject, Anchor.RIGHT, new Vector3(num3, num4, 0f));
			TransformUtil.SetLocalPosX(this.m_menuButton, this.m_menuButton.transform.localPosition.x + 0.05f);
			TransformUtil.SetLocalPosY(this.m_menuButton, y);
			this.m_batteryLevel.gameObject.SetActive(true);
			int phoneStatusBarState = 1 + ((!this.m_connectionIndicator.IsVisible()) ? 0 : 1);
			this.m_menuButton.SetPhoneStatusBarState(phoneStatusBarState);
			TransformUtil.SetLocalScaleX(this.m_currencyFrame, 2f / num2);
			TransformUtil.SetLocalScaleY(this.m_currencyFrame, 0.4f);
			if (this.m_menuButton.gameObject.activeInHierarchy)
			{
				this.PositionCurrencyFrame(this.m_batteryLevel.gameObject, new Vector3(-35f, num5, 0f));
			}
			else
			{
				this.PositionCurrencyFrame(this.m_batteryLevel.gameObject, new Vector3(100f, num5, 0f));
			}
		}
		else
		{
			TransformUtil.SetPoint(this.m_menuButton, Anchor.RIGHT, base.gameObject, Anchor.RIGHT, new Vector3(num3, num4, 0f));
			TransformUtil.SetLocalScaleX(this.m_currencyFrame, 1f / num2);
			this.PositionCurrencyFrame(this.m_menuButton.gameObject, new Vector3(-25f, -5f, 0f));
		}
		MultiSliceElement componentInChildren = this.m_currencyFrame.GetComponentInChildren<MultiSliceElement>();
		if (componentInChildren != null)
		{
			componentInChildren.UpdateSlices();
		}
		bool flag = this.m_spectatorCountPanel != null && this.m_spectatorCountPanel.activeInHierarchy && SpectatorManager.Get().IsBeingSpectated();
		bool flag2 = this.m_spectatorModeIndicator != null && this.m_spectatorModeIndicator.activeInHierarchy && SpectatorManager.Get().IsInSpectatorMode();
		if (UniversalInputManager.UsePhoneUI && SceneMgr.Get() != null && !SceneMgr.Get().IsInGame())
		{
			flag = false;
			flag2 = false;
		}
		bool flag3 = flag || flag2;
		if (this.m_friendButton.gameObject.activeInHierarchy)
		{
			TransformUtil.SetLocalScaleX(this.m_friendButton, this.m_initialFriendButtonScaleX / num2);
			TransformUtil.SetPoint(this.m_friendButton, Anchor.LEFT, base.gameObject, Anchor.LEFT, new Vector3(6f, 5f, 0f));
			TransformUtil.SetLocalScaleX(this.m_currentTime, 1f / num2);
			TransformUtil.SetLocalScaleX(this.m_socialToastBone, 1f / num2);
			if (UniversalInputManager.UsePhoneUI)
			{
				TransformUtil.SetLocalPosY(this.m_friendButton, y);
			}
			if (!flag3)
			{
				TransformUtil.SetPoint(this.m_currentTime, Anchor.LEFT, this.m_friendButton, Anchor.RIGHT, new Vector3(22f, num5, 0f) + zero);
				TransformUtil.SetPoint(this.m_socialToastBone, Anchor.LEFT, this.m_friendButton, Anchor.RIGHT, new Vector3(15f, 0f, -1f) + zero);
			}
			else if (flag)
			{
				if (UniversalInputManager.UsePhoneUI)
				{
					TransformUtil.SetPoint(this.m_spectatorCountPanel, Anchor.LEFT, base.gameObject, Anchor.LEFT, new Vector3(6f, 5f, 0f));
				}
				else
				{
					TransformUtil.SetPoint(this.m_spectatorCountPanel, Anchor.LEFT, this.m_friendButton, Anchor.RIGHT, new Vector3(8f, 0f, 0f) + zero);
					TransformUtil.SetPoint(this.m_socialToastBone, Anchor.LEFT, this.m_spectatorCountPanel, Anchor.RIGHT, new Vector3(7f, 0f, -1f) + zero);
				}
				TransformUtil.SetPoint(this.m_currentTime, Anchor.LEFT, this.m_spectatorCountPanel, Anchor.RIGHT, new Vector3(14f, -4f, 0f));
			}
			else if (flag2)
			{
				if (UniversalInputManager.UsePhoneUI)
				{
					TransformUtil.SetPoint(this.m_spectatorModeIndicator, Anchor.LEFT, base.gameObject, Anchor.LEFT, new Vector3(6f, 5f, 0f));
				}
				else
				{
					TransformUtil.SetPoint(this.m_spectatorModeIndicator, Anchor.LEFT, this.m_friendButton, Anchor.RIGHT, new Vector3(8f, 0f, 0f) + zero);
					TransformUtil.SetPoint(this.m_socialToastBone, Anchor.LEFT, this.m_spectatorModeIndicator, Anchor.RIGHT, new Vector3(7f, 0f, -1f) + zero);
				}
				TransformUtil.SetPoint(this.m_currentTime, Anchor.LEFT, this.m_spectatorModeIndicator, Anchor.RIGHT, new Vector3(14f, -4f, 0f));
			}
			float num6 = 1f;
			if (UniversalInputManager.UsePhoneUI)
			{
				num6 = 2.5f;
			}
			TransformUtil.SetLocalScaleX(this.m_questProgressToastBone, num6 / num2);
		}
		else
		{
			GameObject dst = base.gameObject;
			if (flag3)
			{
				TransformUtil.SetPoint(this.m_spectatorCountPanel, Anchor.LEFT, dst, Anchor.RIGHT, new Vector3(0f, 0f, 0f));
				dst = this.m_spectatorCountPanel;
			}
			else if (flag2)
			{
				TransformUtil.SetPoint(this.m_spectatorModeIndicator, Anchor.LEFT, dst, Anchor.RIGHT, new Vector3(0f, 0f, 0f) + zero);
				dst = this.m_spectatorModeIndicator;
			}
			TransformUtil.SetLocalScaleX(this.m_currentTime, 1f / num2);
			TransformUtil.SetPoint(this.m_currentTime, Anchor.LEFT, base.gameObject, Anchor.LEFT, new Vector3(6f, 5f, 0f) + zero);
		}
		this.UpdateLoginTooltip();
		if (this.m_isInitting)
		{
			this.m_currencyFrame.DeactivateCurrencyFrame();
			this.m_isInitting = false;
		}
	}

	// Token: 0x0600399A RID: 14746 RVA: 0x00118094 File Offset: 0x00116294
	private void PositionCurrencyFrame(GameObject parent, Vector3 offset)
	{
		GameObject tooltipObject = this.m_currencyFrame.GetTooltipObject();
		if (tooltipObject != null)
		{
			tooltipObject.SetActive(false);
		}
		TransformUtil.SetPoint(this.m_currencyFrame, Anchor.RIGHT, parent, Anchor.LEFT, offset, false);
		if (tooltipObject != null)
		{
			tooltipObject.SetActive(true);
		}
	}

	// Token: 0x0600399B RID: 14747 RVA: 0x001180E4 File Offset: 0x001162E4
	public bool HandleKeyboardInput()
	{
		if (Input.GetKeyUp(27))
		{
			return this.HandleEscapeKey();
		}
		ChatMgr chatMgr = ChatMgr.Get();
		return chatMgr != null && chatMgr.HandleKeyboardInput();
	}

	// Token: 0x0600399C RID: 14748 RVA: 0x00118124 File Offset: 0x00116324
	public void ToggleGameMenu()
	{
		if (this.m_gameMenu == null)
		{
			if (!this.m_gameMenuLoading)
			{
				this.m_gameMenuLoading = true;
				AssetLoader.Get().LoadGameObject("GameMenu", new AssetLoader.GameObjectCallback(this.ShowGameMenu), null, false);
			}
			return;
		}
		if (this.m_gameMenu.IsShown())
		{
			this.m_gameMenu.Hide();
		}
		else
		{
			this.m_gameMenu.Show();
		}
	}

	// Token: 0x0600399D RID: 14749 RVA: 0x0011819E File Offset: 0x0011639E
	public void ToggleEnableButtons(bool enabled)
	{
		this.m_menuButton.SetEnabled(enabled);
		this.m_friendButton.SetEnabled(enabled);
	}

	// Token: 0x0600399E RID: 14750 RVA: 0x001181B8 File Offset: 0x001163B8
	public void ToggleFriendsButton(bool enabled)
	{
		this.m_friendButton.gameObject.SetActive(enabled);
	}

	// Token: 0x0600399F RID: 14751 RVA: 0x001181CB File Offset: 0x001163CB
	public void ToggleActive(bool active)
	{
		base.gameObject.SetActive(active);
	}

	// Token: 0x060039A0 RID: 14752 RVA: 0x001181D9 File Offset: 0x001163D9
	public void Enable()
	{
		this.m_isEnabled = true;
		this.m_menuButtonMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("_LightingBlend", this.m_lightingBlend);
	}

	// Token: 0x060039A1 RID: 14753 RVA: 0x00118204 File Offset: 0x00116404
	public void Disable()
	{
		this.m_isEnabled = false;
		this.m_menuButtonMesh.GetComponent<Renderer>().sharedMaterial.SetFloat("_LightingBlend", 0.6f);
		if (this.m_gameMenu != null && this.m_gameMenu.IsShown())
		{
			this.m_gameMenu.Hide();
		}
		if (OptionsMenu.Get() != null && OptionsMenu.Get().IsShown())
		{
			OptionsMenu.Get().Hide(true);
		}
	}

	// Token: 0x060039A2 RID: 14754 RVA: 0x0011828D File Offset: 0x0011648D
	public bool IsEnabled()
	{
		return this.m_isEnabled;
	}

	// Token: 0x060039A3 RID: 14755 RVA: 0x00118295 File Offset: 0x00116495
	public void SetCurrencyType(CurrencyFrame.CurrencyType? type)
	{
		this.m_currencyFrame.SetCurrencyOverride(type);
	}

	// Token: 0x060039A4 RID: 14756 RVA: 0x001182A4 File Offset: 0x001164A4
	public void UpdateLoginTooltip()
	{
		bool flag = !Network.ShouldBeConnectedToAurora() && !this.m_suppressLoginTooltip && SceneMgr.Get().IsInGame() && GameMgr.Get().IsTutorial() && !GameMgr.Get().IsSpectator() && DemoMgr.Get().GetMode() != DemoMode.APPLE_STORE;
		if (flag)
		{
			if (this.m_loginTooltip == null)
			{
				this.m_loginTooltip = AssetLoader.Get().LoadGameObject("LoginPointer", true, false);
				if (UniversalInputManager.UsePhoneUI)
				{
					this.m_loginTooltip.transform.localScale = new Vector3(60f, 60f, 60f);
				}
				else
				{
					this.m_loginTooltip.transform.localScale = new Vector3(40f, 40f, 40f);
				}
				TransformUtil.SetEulerAngleX(this.m_loginTooltip, 270f);
				SceneUtils.SetLayer(this.m_loginTooltip, GameLayer.BattleNet);
				this.m_loginTooltip.transform.parent = base.transform;
			}
			if (UniversalInputManager.UsePhoneUI)
			{
				TransformUtil.SetPoint(this.m_loginTooltip, Anchor.RIGHT, this.m_batteryLevel.gameObject, Anchor.LEFT, new Vector3(-32f, 0f, 0f));
			}
			else
			{
				TransformUtil.SetPoint(this.m_loginTooltip, Anchor.RIGHT, this.m_menuButton, Anchor.LEFT, new Vector3(5f, 0f, 0f));
			}
		}
		else
		{
			this.DestroyLoginTooltip();
		}
	}

	// Token: 0x060039A5 RID: 14757 RVA: 0x00118436 File Offset: 0x00116636
	private void DestroyLoginTooltip()
	{
		if (this.m_loginTooltip != null)
		{
			Object.Destroy(this.m_loginTooltip);
			this.m_loginTooltip = null;
		}
	}

	// Token: 0x060039A6 RID: 14758 RVA: 0x0011845B File Offset: 0x0011665B
	public void SuppressLoginTooltip(bool val)
	{
		this.m_suppressLoginTooltip = val;
		this.UpdateLayout();
	}

	// Token: 0x060039A7 RID: 14759 RVA: 0x0011846C File Offset: 0x0011666C
	public void ShowFriendList()
	{
		ChatMgr.Get().ShowFriendsList();
		this.m_hasUnacknowledgedPendingInvites = false;
		this.m_friendButton.ShowPendingInvitesIcon(this.m_hasUnacknowledgedPendingInvites);
	}

	// Token: 0x060039A8 RID: 14760 RVA: 0x0011849B File Offset: 0x0011669B
	public void HideFriendList()
	{
		ChatMgr.Get().CloseChatUI();
	}

	// Token: 0x060039A9 RID: 14761 RVA: 0x001184A8 File Offset: 0x001166A8
	private bool HandleEscapeKey()
	{
		if (this.m_gameMenu != null && this.m_gameMenu.IsShown())
		{
			this.m_gameMenu.Hide();
			return true;
		}
		if (OptionsMenu.Get() != null && OptionsMenu.Get().IsShown())
		{
			OptionsMenu.Get().Hide(true);
			return true;
		}
		if (QuestLog.Get() != null && QuestLog.Get().IsShown())
		{
			QuestLog.Get().Hide();
			return true;
		}
		if (GeneralStore.Get() != null && GeneralStore.Get().IsShown())
		{
			GeneralStore.Get().Close();
			return true;
		}
		ChatMgr chatMgr = ChatMgr.Get();
		if (chatMgr != null && chatMgr.HandleKeyboardInput())
		{
			return true;
		}
		if (CraftingTray.Get() != null && CraftingTray.Get().IsShown())
		{
			CraftingTray.Get().Hide();
			return true;
		}
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		if (mode == SceneMgr.Mode.FATAL_ERROR)
		{
			return true;
		}
		if (mode == SceneMgr.Mode.LOGIN)
		{
			return true;
		}
		if (mode == SceneMgr.Mode.STARTUP)
		{
			return true;
		}
		if (mode != SceneMgr.Mode.GAMEPLAY && !DemoMgr.Get().IsHubEscMenuEnabled())
		{
			return true;
		}
		if (PlatformSettings.OS == OSCategory.Android && mode == SceneMgr.Mode.HUB)
		{
			return false;
		}
		this.ToggleGameMenu();
		return true;
	}

	// Token: 0x060039AA RID: 14762 RVA: 0x00118609 File Offset: 0x00116809
	private void OnMenuButtonReleased(UIEvent e)
	{
		this.ToggleGameMenu();
	}

	// Token: 0x060039AB RID: 14763 RVA: 0x00118611 File Offset: 0x00116811
	private void ShowGameMenu(string name, GameObject go, object callbackData)
	{
		this.m_gameMenu = go.GetComponent<GameMenu>();
		this.m_gameMenu.GetComponent<GameMenu>().Show();
		this.m_gameMenuLoading = false;
	}

	// Token: 0x060039AC RID: 14764 RVA: 0x00118638 File Offset: 0x00116838
	private void UpdateForDemoMode()
	{
		if (!DemoMgr.Get().IsExpoDemo())
		{
			return;
		}
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		bool flag = true;
		bool flag2;
		switch (DemoMgr.Get().GetMode())
		{
		case DemoMode.PAX_EAST_2013:
		case DemoMode.BLIZZCON_2013:
		case DemoMode.BLIZZCON_2015:
			flag2 = (mode == SceneMgr.Mode.GAMEPLAY);
			flag = false;
			this.m_currencyFrame.gameObject.SetActive(false);
			goto IL_AC;
		case DemoMode.BLIZZCON_2014:
		{
			bool flag3 = mode != SceneMgr.Mode.FRIENDLY;
			flag2 = flag3;
			flag = flag3;
			goto IL_AC;
		}
		case DemoMode.APPLE_STORE:
			flag = (flag2 = false);
			goto IL_AC;
		case DemoMode.ANNOUNCEMENT_5_0:
			flag = true;
			flag2 = true;
			goto IL_AC;
		}
		flag2 = (mode != SceneMgr.Mode.FRIENDLY && mode != SceneMgr.Mode.TOURNAMENT);
		IL_AC:
		switch (mode)
		{
		case SceneMgr.Mode.GAMEPLAY:
		case SceneMgr.Mode.TOURNAMENT:
		case SceneMgr.Mode.FRIENDLY:
			if (DemoMgr.Get().GetMode() != DemoMode.ANNOUNCEMENT_5_0)
			{
				flag = false;
			}
			break;
		}
		if (!flag2)
		{
			this.m_menuButton.gameObject.SetActive(false);
		}
		if (!flag)
		{
			this.m_friendButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x060039AD RID: 14765 RVA: 0x0011875C File Offset: 0x0011695C
	private void UpdateForPhone()
	{
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		bool active = mode == SceneMgr.Mode.HUB || mode == SceneMgr.Mode.LOGIN || mode == SceneMgr.Mode.GAMEPLAY;
		this.m_menuButton.gameObject.SetActive(active);
	}

	// Token: 0x060039AE RID: 14766 RVA: 0x0011879B File Offset: 0x0011699B
	private void OnFriendButtonReleased(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("Small_Click");
		this.ToggleFriendListShowing();
	}

	// Token: 0x060039AF RID: 14767 RVA: 0x001187B4 File Offset: 0x001169B4
	private void ToggleFriendListShowing()
	{
		if (ChatMgr.Get().IsFriendListShowing())
		{
			this.HideFriendList();
		}
		else
		{
			this.ShowFriendList();
		}
		this.m_friendButton.HideTooltip();
	}

	// Token: 0x060039B0 RID: 14768 RVA: 0x001187EC File Offset: 0x001169EC
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		this.ToggleEnableButtons(false);
	}

	// Token: 0x060039B1 RID: 14769 RVA: 0x001187F8 File Offset: 0x001169F8
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		if (mode == SceneMgr.Mode.FATAL_ERROR)
		{
			return;
		}
		this.m_suppressLoginTooltip = false;
		this.m_currencyFrame.RefreshContents();
		bool flag = mode != SceneMgr.Mode.INVALID && mode != SceneMgr.Mode.FATAL_ERROR;
		if (flag && SpectatorManager.Get().IsInSpectatorMode())
		{
			this.SpectatorManager_OnSpectatorModeChanged(0, null);
		}
		else if (this.m_spectatorModeIndicator != null && this.m_spectatorModeIndicator.activeSelf)
		{
			this.m_spectatorModeIndicator.SetActive(false);
		}
		if (flag && this.m_spectatorCountPanel != null)
		{
			bool active = SpectatorManager.Get().IsBeingSpectated();
			if (UniversalInputManager.UsePhoneUI && SceneMgr.Get() != null && !SceneMgr.Get().IsInGame())
			{
				active = false;
			}
			this.m_spectatorCountPanel.SetActive(active);
		}
		this.UpdateForDemoMode();
		this.UpdateLayout();
		if (UniversalInputManager.UsePhoneUI)
		{
			this.UpdateForPhone();
		}
	}

	// Token: 0x060039B2 RID: 14770 RVA: 0x00118900 File Offset: 0x00116B00
	private void SpectatorManager_OnInviteReceived(OnlineEventType evt, BnetPlayer inviter)
	{
		if (ChatMgr.Get().IsFriendListShowing() || !SpectatorManager.Get().HasAnyReceivedInvites())
		{
			this.m_hasUnacknowledgedPendingInvites = false;
		}
		else
		{
			this.m_hasUnacknowledgedPendingInvites = (this.m_hasUnacknowledgedPendingInvites || evt == 0);
		}
		if (this.m_friendButton != null)
		{
			this.m_friendButton.ShowPendingInvitesIcon(this.m_hasUnacknowledgedPendingInvites);
		}
	}

	// Token: 0x060039B3 RID: 14771 RVA: 0x00118974 File Offset: 0x00116B74
	private void SpectatorManager_OnSpectatorToMyGame(OnlineEventType evt, BnetPlayer spectator)
	{
		int countSpectatingMe = SpectatorManager.Get().GetCountSpectatingMe();
		if (countSpectatingMe <= 0)
		{
			if (this.m_spectatorCountPanel == null)
			{
				return;
			}
		}
		else if (this.m_spectatorCountPanel == null)
		{
			string name = FileUtils.GameAssetPathToName(this.m_spectatorCountPrefabPath);
			AssetLoader.Get().LoadGameObject(name, delegate(string n, GameObject go, object d)
			{
				BnetBar bnetBar = BnetBar.Get();
				if (bnetBar == null)
				{
					return;
				}
				if (bnetBar.m_spectatorCountPanel != null)
				{
					Object.Destroy(go);
				}
				else
				{
					bnetBar.m_spectatorCountPanel = go;
					bnetBar.m_spectatorCountPanel.transform.parent = bnetBar.m_friendButton.transform;
					PegUIElement component2 = go.GetComponent<PegUIElement>();
					if (component2 != null)
					{
						component2.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(BnetBar.SpectatorCount_OnRollover));
						component2.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(BnetBar.SpectatorCount_OnRollout));
					}
					GameObject gameObject2 = bnetBar.m_spectatorCountPanel.transform.FindChild("BeingWatchedHighlight").gameObject;
					Color color = gameObject2.GetComponent<Renderer>().material.color;
					color.a = 0f;
					gameObject2.GetComponent<Renderer>().material.color = color;
				}
				BnetBar.Get().SpectatorManager_OnSpectatorToMyGame(evt, spectator);
			}, null, false);
			return;
		}
		UberText component = this.m_spectatorCountPanel.transform.FindChild("UberText").GetComponent<UberText>();
		component.Text = countSpectatingMe.ToString();
		bool active = countSpectatingMe > 0;
		if (UniversalInputManager.UsePhoneUI && SceneMgr.Get() != null && !SceneMgr.Get().IsInGame())
		{
			active = false;
		}
		this.m_spectatorCountPanel.SetActive(active);
		this.UpdateLayout();
		GameObject gameObject = this.m_spectatorCountPanel.transform.FindChild("BeingWatchedHighlight").gameObject;
		iTween.Stop(gameObject, true);
		Action<object> action = delegate(object ud)
		{
			if (BnetBar.Get() == null)
			{
				return;
			}
			iTween.FadeTo(BnetBar.Get().m_spectatorCountPanel.transform.FindChild("BeingWatchedHighlight").gameObject, 0f, 0.5f);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"alpha",
			1f,
			"time",
			0.5f,
			"oncomplete",
			action
		});
		iTween.FadeTo(gameObject, args);
	}

	// Token: 0x060039B4 RID: 14772 RVA: 0x00118AFC File Offset: 0x00116CFC
	private static void SpectatorCount_OnRollover(UIEvent evt)
	{
		BnetBar bnetBar = BnetBar.Get();
		if (bnetBar == null)
		{
			return;
		}
		string headline = GameStrings.Get("GLOBAL_SPECTATOR_COUNT_PANEL_HEADER");
		BnetGameAccountId[] spectatorPartyMembers = SpectatorManager.Get().GetSpectatorPartyMembers(true, false);
		int num = spectatorPartyMembers.Length;
		string bodytext;
		if (num == 1)
		{
			string playerBestName = BnetUtils.GetPlayerBestName(spectatorPartyMembers[0]);
			bodytext = GameStrings.Format("GLOBAL_SPECTATOR_COUNT_PANEL_TEXT_ONE", new object[]
			{
				playerBestName
			});
		}
		else
		{
			string[] array = Enumerable.ToArray<string>(Enumerable.Select<BnetGameAccountId, string>(spectatorPartyMembers, (BnetGameAccountId id) => BnetUtils.GetPlayerBestName(id)));
			string text = string.Join(", ", array);
			bodytext = text;
		}
		bnetBar.m_spectatorCountTooltipZone.ShowSocialTooltip(bnetBar.m_spectatorCountPanel, headline, bodytext, 75f, GameLayer.BattleNetDialog);
		bnetBar.m_spectatorCountTooltipZone.AnchorTooltipTo(bnetBar.m_spectatorCountPanel, Anchor.TOP_LEFT, Anchor.BOTTOM_LEFT);
	}

	// Token: 0x060039B5 RID: 14773 RVA: 0x00118BD0 File Offset: 0x00116DD0
	private static void SpectatorCount_OnRollout(UIEvent evt)
	{
		BnetBar bnetBar = BnetBar.Get();
		if (bnetBar == null)
		{
			return;
		}
		bnetBar.m_spectatorCountTooltipZone.HideTooltip();
	}

	// Token: 0x060039B6 RID: 14774 RVA: 0x00118BFC File Offset: 0x00116DFC
	private void SpectatorManager_OnSpectatorModeChanged(OnlineEventType evt, BnetPlayer spectatee)
	{
		if (evt == null && this.m_spectatorModeIndicator == null)
		{
			string name = FileUtils.GameAssetPathToName(this.m_spectatorModeIndicatorPrefab);
			AssetLoader.Get().LoadGameObject(name, delegate(string n, GameObject go, object d)
			{
				BnetBar bnetBar = BnetBar.Get();
				if (bnetBar == null)
				{
					return;
				}
				if (bnetBar.m_spectatorModeIndicator != null)
				{
					Object.Destroy(go);
				}
				else
				{
					bnetBar.m_spectatorModeIndicator = go;
					bnetBar.m_spectatorModeIndicator.transform.parent = bnetBar.m_friendButton.transform;
				}
				BnetBar.Get().SpectatorManager_OnSpectatorModeChanged(evt, spectatee);
			}, null, false);
			return;
		}
		if (this.m_spectatorModeIndicator == null)
		{
			return;
		}
		bool active = evt == null && SpectatorManager.Get().IsInSpectatorMode();
		if (UniversalInputManager.UsePhoneUI && SceneMgr.Get() != null && !SceneMgr.Get().IsInGame())
		{
			active = false;
		}
		this.m_spectatorModeIndicator.SetActive(active);
		this.UpdateLayout();
	}

	// Token: 0x040024D6 RID: 9430
	public UberText m_currentTime;

	// Token: 0x040024D7 RID: 9431
	public BnetBarMenuButton m_menuButton;

	// Token: 0x040024D8 RID: 9432
	public GameObject m_menuButtonMesh;

	// Token: 0x040024D9 RID: 9433
	public BnetBarFriendButton m_friendButton;

	// Token: 0x040024DA RID: 9434
	public CurrencyFrame m_currencyFrame;

	// Token: 0x040024DB RID: 9435
	public Flipbook m_batteryLevel;

	// Token: 0x040024DC RID: 9436
	public Flipbook m_batteryLevelPhone;

	// Token: 0x040024DD RID: 9437
	public GameObject m_socialToastBone;

	// Token: 0x040024DE RID: 9438
	public GameObject m_questProgressToastBone;

	// Token: 0x040024DF RID: 9439
	public ConnectionIndicator m_connectionIndicator;

	// Token: 0x040024E0 RID: 9440
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_spectatorCountPrefabPath;

	// Token: 0x040024E1 RID: 9441
	public TooltipZone m_spectatorCountTooltipZone;

	// Token: 0x040024E2 RID: 9442
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_spectatorModeIndicatorPrefab;

	// Token: 0x040024E3 RID: 9443
	public static readonly int CameraDepth = 47;

	// Token: 0x040024E4 RID: 9444
	private static BnetBar s_instance;

	// Token: 0x040024E5 RID: 9445
	private float m_initialWidth;

	// Token: 0x040024E6 RID: 9446
	private float m_initialFriendButtonScaleX;

	// Token: 0x040024E7 RID: 9447
	private float m_initialMenuButtonScaleX;

	// Token: 0x040024E8 RID: 9448
	private float m_initialConnectionIndicatorScaleX;

	// Token: 0x040024E9 RID: 9449
	private GameMenu m_gameMenu;

	// Token: 0x040024EA RID: 9450
	private bool m_gameMenuLoading;

	// Token: 0x040024EB RID: 9451
	private bool m_isInitting = true;

	// Token: 0x040024EC RID: 9452
	private GameObject m_loginTooltip;

	// Token: 0x040024ED RID: 9453
	private float m_lightingBlend = 1f;

	// Token: 0x040024EE RID: 9454
	private bool m_hasUnacknowledgedPendingInvites;

	// Token: 0x040024EF RID: 9455
	private GameObject m_spectatorCountPanel;

	// Token: 0x040024F0 RID: 9456
	private GameObject m_spectatorModeIndicator;

	// Token: 0x040024F1 RID: 9457
	private bool m_isEnabled = true;

	// Token: 0x040024F2 RID: 9458
	private bool m_isLoggedIn;

	// Token: 0x040024F3 RID: 9459
	private bool m_suppressLoginTooltip;

	// Token: 0x040024F4 RID: 9460
	private float m_lastClockUpdate;
}
