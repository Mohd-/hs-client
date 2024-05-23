using System;
using System.Collections.Generic;
using bgs;
using UnityEngine;

// Token: 0x020008A2 RID: 2210
public class WebLoginCanvas : MonoBehaviour
{
	// Token: 0x060053FF RID: 21503 RVA: 0x00191E4C File Offset: 0x0019004C
	private void Awake()
	{
		OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		bool flag = ApplicationMgr.GetMobileEnvironment() == MobileEnv.DEVELOPMENT;
		this.m_regionNames = new Map<constants.BnetRegion, string>();
		Map<constants.BnetRegion, string>.KeyCollection keys = WebLoginCanvas.s_regionStringNames.Keys;
		foreach (constants.BnetRegion key in keys)
		{
			if (flag)
			{
				this.m_regionNames[key] = GameStrings.Get(WebLoginCanvas.s_regionStringNames[key]).Split(new char[]
				{
					' '
				})[0];
			}
			else
			{
				this.m_regionNames[key] = GameStrings.Get(WebLoginCanvas.s_regionStringNames[key]);
			}
		}
		if (this.USE_REGION_DROPDOWN)
		{
			this.SetUpRegionDropdown();
		}
		else
		{
			this.SetUpRegionButton();
		}
		if (UniversalInputManager.UsePhoneUI && flag)
		{
			this.SetUpRegionDropdown();
		}
		this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackPressed));
		if (!Options.Get().GetBool(Option.CONNECT_TO_AURORA))
		{
			this.m_backButton.gameObject.SetActive(true);
		}
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x06005400 RID: 21504 RVA: 0x00191FAC File Offset: 0x001901AC
	private void Update()
	{
		if (this.KOBOLD_SHOWN_ON_ACCOUNT_CREATION)
		{
			if (this.m_acFlipbookCur < this.m_acFlipbookSwap)
			{
				this.m_acFlipbookCur += Time.deltaTime * 60f;
			}
			else
			{
				if (this.m_flipbook.m_acFlipbook.GetComponent<Renderer>().sharedMaterial.mainTexture == this.m_flipbook.m_acFlipbookTextures[0])
				{
					this.m_flipbook.m_acFlipbook.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_flipbook.m_acFlipbookTextures[1];
					this.m_acFlipbookSwap = this.m_flipbook.m_acFlipbookTimeAlt;
				}
				else
				{
					this.m_flipbook.m_acFlipbook.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_flipbook.m_acFlipbookTextures[0];
					this.m_acFlipbookSwap = Random.Range(this.m_flipbook.m_acFlipbookTimeMin, this.m_flipbook.m_acFlipbookTimeMax);
				}
				this.m_acFlipbookCur = 0f;
			}
		}
	}

	// Token: 0x06005401 RID: 21505 RVA: 0x001920B8 File Offset: 0x001902B8
	private void OnDestroy()
	{
		if (this.m_regionSelectTooltip != null)
		{
			Object.Destroy(this.m_regionSelectTooltip.gameObject);
		}
	}

	// Token: 0x06005402 RID: 21506 RVA: 0x001920DC File Offset: 0x001902DC
	public void WebViewDidFinishLoad(string pageState)
	{
		Debug.Log("web view page state: " + pageState);
		if (pageState == null)
		{
			return;
		}
		string[] array = pageState.Split(new string[]
		{
			"|"
		}, 0);
		if (array.Length < 2)
		{
			Debug.LogWarning(string.Format("WebViewDidFinishLoad() - Invalid parsed pageState ({0})", pageState));
			return;
		}
		this.m_canGoBack = array[array.Length - 1].Equals("canGoBack");
		bool active = false;
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < array.Length - 1; i++)
		{
			string text = array[i];
			if (text.Equals("STATE_ACCOUNT_CREATION", 3))
			{
				active = true;
			}
			if (text.Equals("STATE_ACCOUNT_CREATED", 3))
			{
				flag = true;
			}
			if (text.Equals("STATE_NO_BACK", 3))
			{
				flag2 = true;
			}
		}
		if (this.KOBOLD_SHOWN_ON_ACCOUNT_CREATION)
		{
			this.m_accountCreation.SetActive(active);
		}
		flag2 = (flag2 || flag);
		if (flag)
		{
			WebAuth.SetIsNewCreatedAccount(true);
		}
		this.m_backButton.gameObject.SetActive(!flag2 && (this.m_canGoBack || !Options.Get().GetBool(Option.CONNECT_TO_AURORA)));
	}

	// Token: 0x06005403 RID: 21507 RVA: 0x0019220B File Offset: 0x0019040B
	public void WebViewBackButtonPressed(string dummyState)
	{
		Navigation.GoBack();
	}

	// Token: 0x06005404 RID: 21508 RVA: 0x00192214 File Offset: 0x00190414
	private void SetUpRegionButton()
	{
		if (this.m_regionButton != null)
		{
			this.m_regionButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.ShowRegionMenu();
			});
			string text = this.onRegionText(MobileDeviceLocale.GetCurrentRegionId());
			this.m_regionButton.SetText(text);
		}
	}

	// Token: 0x06005405 RID: 21509 RVA: 0x00192268 File Offset: 0x00190468
	private void SetUpRegionDropdown()
	{
		bool flag = ApplicationMgr.GetMobileEnvironment() == MobileEnv.DEVELOPMENT;
		this.m_regionSelector = Object.Instantiate<DropdownControl>(this.m_regionSelectDropdownPrefab);
		this.m_regionSelector.transform.parent = base.gameObject.transform;
		TransformUtil.CopyLocal(this.m_regionSelector.transform, this.m_regionSelectDropdownBone.transform);
		this.m_regionSelector.clearItems();
		this.m_regionSelector.setItemTextCallback(new DropdownControl.itemTextCallback(this.onRegionText));
		this.m_regionSelector.setMenuShownCallback(new DropdownControl.menuShownCallback(this.onMenuShown));
		this.m_regionSelector.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.onRegionWarning));
		if (flag)
		{
			foreach (constants.BnetRegion bnetRegion in MobileDeviceLocale.s_regionIdToDevIP.Keys)
			{
				this.m_regionSelector.addItem(bnetRegion);
			}
		}
		else
		{
			this.m_regionSelector.addItem(1);
			this.m_regionSelector.addItem(2);
			this.m_regionSelector.addItem(3);
		}
		constants.BnetRegion currentRegionId = MobileDeviceLocale.GetCurrentRegionId();
		this.m_regionSelector.setSelection(currentRegionId);
		if (MobileDeviceLocale.UseClientConfigForEnv())
		{
			this.m_regionSelector.gameObject.SetActive(false);
		}
		this.m_regionSelectTooltip = KeywordHelpPanelManager.Get().CreateKeywordPanel(0);
		this.m_regionSelectTooltip.Reset();
		this.m_regionSelectTooltip.Initialize(GameStrings.Get("GLUE_MOBILE_REGION_SELECT_TOOLTIP_HEADER"), GameStrings.Get("GLUE_MOBILE_REGION_SELECT_TOOLTIP"));
		this.m_regionSelectTooltip.transform.position = this.m_regionSelectTooltipBone.transform.position;
		this.m_regionSelectTooltip.transform.localScale = this.m_regionSelectTooltipBone.transform.localScale;
		this.m_regionSelectTooltip.transform.eulerAngles = new Vector3(0f, 0f, 0f);
		SceneUtils.SetLayer(this.m_regionSelectTooltip.gameObject, GameLayer.UI);
		this.m_regionSelectTooltip.gameObject.SetActive(false);
	}

	// Token: 0x06005406 RID: 21510 RVA: 0x001924A4 File Offset: 0x001906A4
	private void onMenuShown(bool shown)
	{
		if (shown)
		{
			this.m_regionSelectTooltip.gameObject.SetActive(true);
			WebAuth.UpdateRegionSelectVisualState(true);
			if (UniversalInputManager.UsePhoneUI)
			{
				SplashScreen.Get().HideWebAuth();
			}
		}
		else
		{
			this.m_regionSelectTooltip.gameObject.SetActive(false);
			WebAuth.UpdateRegionSelectVisualState(false);
			if (UniversalInputManager.UsePhoneUI)
			{
				SplashScreen.Get().UnHideWebAuth();
			}
		}
	}

	// Token: 0x06005407 RID: 21511 RVA: 0x0019251C File Offset: 0x0019071C
	private void onRegionChange(object selection, object prevSelection)
	{
		if (selection == prevSelection)
		{
			return;
		}
		constants.BnetRegion val = (int)selection;
		Options.Get().SetInt(Option.PREFERRED_REGION, val);
		this.CauseReconnect();
	}

	// Token: 0x06005408 RID: 21512 RVA: 0x0019254C File Offset: 0x0019074C
	private void onRegionChangeCB(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CONFIRM)
		{
			this.onRegionChange(this.m_selection, this.m_prevSelection);
		}
		else
		{
			if (this.m_regionSelector != null)
			{
				this.m_regionSelector.setSelection(this.m_prevSelection);
			}
			SplashScreen.Get().UnHideWebAuth();
		}
		this.m_regionSelectContents.SetActive(false);
	}

	// Token: 0x06005409 RID: 21513 RVA: 0x001925B0 File Offset: 0x001907B0
	private void ShowRegionMenu()
	{
		if (this.m_regionMenu != null)
		{
			this.m_regionMenu.Show();
			return;
		}
		GameObject gameObject = (GameObject)GameUtils.InstantiateGameObject("RegionMenu", null, false);
		this.m_regionMenu = gameObject.GetComponent<RegionMenu>();
		List<UIBButton> buttons = new List<UIBButton>();
		Debug.Log("creating region menu..");
		this.AddButtonForRegion(buttons, 1);
		this.AddButtonForRegion(buttons, 2);
		this.AddButtonForRegion(buttons, 3);
		this.m_regionMenu.SetButtons(buttons);
		this.m_regionMenu.Show();
	}

	// Token: 0x0600540A RID: 21514 RVA: 0x00192638 File Offset: 0x00190838
	private void AddButtonForRegion(List<UIBButton> buttons, constants.BnetRegion region)
	{
		constants.BnetRegion currentRegion = MobileDeviceLocale.GetCurrentRegionId();
		buttons.Add(this.m_regionMenu.CreateMenuButton(null, this.onRegionText(region), delegate(UIEvent e)
		{
			this.m_regionMenu.Hide();
			this.onRegionWarning(region, currentRegion);
		}));
	}

	// Token: 0x0600540B RID: 21515 RVA: 0x00192694 File Offset: 0x00190894
	private void onRegionWarning(object selection, object prevSelection)
	{
		this.m_selection = selection;
		this.m_prevSelection = prevSelection;
		if (selection.Equals(prevSelection))
		{
			return;
		}
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_MOBILE_REGION_SELECT_WARNING_HEADER");
		popupInfo.m_text = GameStrings.Get("GLUE_MOBILE_REGION_SELECT_WARNING");
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.onRegionChangeCB);
		popupInfo.m_padding = 60f;
		if (UniversalInputManager.UsePhoneUI)
		{
			popupInfo.m_padding = 80f;
		}
		popupInfo.m_scaleOverride = new Vector3?(new Vector3(300f, 300f, 300f));
		SplashScreen.Get().HideWebAuth();
		DialogManager.Get().ShowPopup(popupInfo, new DialogManager.DialogProcessCallback(this.OnDialogProcess));
	}

	// Token: 0x0600540C RID: 21516 RVA: 0x00192768 File Offset: 0x00190968
	private string onRegionText(object val)
	{
		constants.BnetRegion bnetRegion = (int)val;
		string text = string.Empty;
		this.m_regionNames.TryGetValue(bnetRegion, out text);
		if (ApplicationMgr.GetMobileEnvironment() == MobileEnv.DEVELOPMENT)
		{
			MobileDeviceLocale.ConnectionData connectionDataFromRegionId = MobileDeviceLocale.GetConnectionDataFromRegionId(bnetRegion, true);
			string text2 = connectionDataFromRegionId.name;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = string.Format("{0}:{1}:{2}", connectionDataFromRegionId.address.Split(new char[]
				{
					'-'
				})[0], connectionDataFromRegionId.port, connectionDataFromRegionId.version);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = text2;
			}
			else
			{
				text = string.Format("{0} ({1})", text2, text);
			}
		}
		return text;
	}

	// Token: 0x0600540D RID: 21517 RVA: 0x00192810 File Offset: 0x00190A10
	private bool OnDialogProcess(DialogBase dialog, object userData)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.m_regionSelectContents);
		TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, dialog.transform);
		gameObject.SetActive(true);
		return true;
	}

	// Token: 0x0600540E RID: 21518 RVA: 0x00192842 File Offset: 0x00190A42
	private void OnBackPressed(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x0600540F RID: 21519 RVA: 0x0019284C File Offset: 0x00190A4C
	private bool OnNavigateBack()
	{
		if (this.m_canGoBack)
		{
			WebAuth.GoBackWebPage();
		}
		else if (!Options.Get().GetBool(Option.CONNECT_TO_AURORA))
		{
			ApplicationMgr.Get().Reset();
			return true;
		}
		return false;
	}

	// Token: 0x06005410 RID: 21520 RVA: 0x0019288C File Offset: 0x00190A8C
	private void CauseReconnect()
	{
		WebAuth.ClearLoginData();
		BattleNet.RequestCloseAurora();
		ApplicationMgr.Get().ResetAndForceLogin();
	}

	// Token: 0x04003A26 RID: 14886
	public DropdownControl m_regionSelectDropdownPrefab;

	// Token: 0x04003A27 RID: 14887
	public GameObject m_regionSelectDropdownBone;

	// Token: 0x04003A28 RID: 14888
	public GameObject m_regionSelectContents;

	// Token: 0x04003A29 RID: 14889
	public GameObject m_accountCreation;

	// Token: 0x04003A2A RID: 14890
	public AccountCreationFlipbook m_flipbook;

	// Token: 0x04003A2B RID: 14891
	public PegUIElement m_backButton;

	// Token: 0x04003A2C RID: 14892
	public GameObject m_regionSelectTooltipBone;

	// Token: 0x04003A2D RID: 14893
	public GameObject m_topLeftBone;

	// Token: 0x04003A2E RID: 14894
	public GameObject m_bottomRightBone;

	// Token: 0x04003A2F RID: 14895
	public UIBButton m_regionButton;

	// Token: 0x04003A30 RID: 14896
	private DropdownControl m_regionSelector;

	// Token: 0x04003A31 RID: 14897
	private object m_selection;

	// Token: 0x04003A32 RID: 14898
	private object m_prevSelection;

	// Token: 0x04003A33 RID: 14899
	private float m_acFlipbookSwap = 30f;

	// Token: 0x04003A34 RID: 14900
	private float m_acFlipbookCur;

	// Token: 0x04003A35 RID: 14901
	private KeywordHelpPanel m_regionSelectTooltip;

	// Token: 0x04003A36 RID: 14902
	private bool m_canGoBack;

	// Token: 0x04003A37 RID: 14903
	private static Map<constants.BnetRegion, string> s_regionStringNames = new Map<constants.BnetRegion, string>
	{
		{
			0,
			"Cuba"
		},
		{
			1,
			"GLOBAL_REGION_AMERICAS"
		},
		{
			2,
			"GLOBAL_REGION_EUROPE"
		},
		{
			3,
			"GLOBAL_REGION_ASIA"
		},
		{
			4,
			"Taiwan"
		},
		{
			5,
			"GLOBAL_REGION_CHINA"
		},
		{
			40,
			"LiveVerif"
		}
	};

	// Token: 0x04003A38 RID: 14904
	private Map<constants.BnetRegion, string> m_regionNames;

	// Token: 0x04003A39 RID: 14905
	private RegionMenu m_regionMenu;

	// Token: 0x04003A3A RID: 14906
	private PlatformDependentValue<bool> KOBOLD_SHOWN_ON_ACCOUNT_CREATION = new PlatformDependentValue<bool>(PlatformCategory.Screen)
	{
		PC = true,
		Tablet = true,
		Phone = false
	};

	// Token: 0x04003A3B RID: 14907
	private PlatformDependentValue<bool> USE_REGION_DROPDOWN = new PlatformDependentValue<bool>(PlatformCategory.Screen)
	{
		PC = true,
		Tablet = true,
		Phone = false
	};
}
