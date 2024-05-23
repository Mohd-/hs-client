using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class DemoMgr
{
	// Token: 0x06000326 RID: 806 RVA: 0x0000F2C7 File Offset: 0x0000D4C7
	public static DemoMgr Get()
	{
		if (DemoMgr.s_instance == null)
		{
			DemoMgr.s_instance = new DemoMgr();
		}
		ApplicationMgr.Get().WillReset += new Action(DemoMgr.s_instance.WillReset);
		return DemoMgr.s_instance;
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0000F2FC File Offset: 0x0000D4FC
	private string GetStoredGameMode()
	{
		return null;
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0000F300 File Offset: 0x0000D500
	public void Initialize()
	{
		string text = this.GetStoredGameMode();
		if (text == null)
		{
			text = Vars.Key("Demo.Mode").GetStr("NONE");
		}
		this.SetModeFromString(text);
	}

	// Token: 0x06000329 RID: 809 RVA: 0x0000F336 File Offset: 0x0000D536
	public bool IsDemo()
	{
		return this.m_mode != DemoMode.NONE;
	}

	// Token: 0x0600032A RID: 810 RVA: 0x0000F344 File Offset: 0x0000D544
	public bool IsExpoDemo()
	{
		switch (this.m_mode)
		{
		case DemoMode.PAX_EAST_2013:
		case DemoMode.GAMESCOM_2013:
		case DemoMode.BLIZZCON_2013:
		case DemoMode.BLIZZCON_2014:
		case DemoMode.BLIZZCON_2015:
		case DemoMode.APPLE_STORE:
		case DemoMode.ANNOUNCEMENT_5_0:
			return true;
		default:
			return false;
		}
	}

	// Token: 0x0600032B RID: 811 RVA: 0x0000F384 File Offset: 0x0000D584
	public bool IsSocialEnabled()
	{
		switch (this.m_mode)
		{
		case DemoMode.BLIZZCON_2013:
		case DemoMode.BLIZZCON_2015:
		case DemoMode.APPLE_STORE:
			return false;
		}
		return true;
	}

	// Token: 0x0600032C RID: 812 RVA: 0x0000F3B8 File Offset: 0x0000D5B8
	public bool IsCurrencyEnabled()
	{
		switch (this.m_mode)
		{
		case DemoMode.BLIZZCON_2013:
		case DemoMode.BLIZZCON_2014:
		case DemoMode.BLIZZCON_2015:
		case DemoMode.ANNOUNCEMENT_5_0:
			return false;
		}
		return true;
	}

	// Token: 0x0600032D RID: 813 RVA: 0x0000F3F0 File Offset: 0x0000D5F0
	public bool IsHubEscMenuEnabled()
	{
		switch (this.m_mode)
		{
		case DemoMode.BLIZZCON_2013:
		case DemoMode.BLIZZCON_2014:
		case DemoMode.BLIZZCON_2015:
		case DemoMode.APPLE_STORE:
		case DemoMode.ANNOUNCEMENT_5_0:
			return false;
		default:
			return true;
		}
	}

	// Token: 0x0600032E RID: 814 RVA: 0x0000F428 File Offset: 0x0000D628
	public bool CantExitArena()
	{
		DemoMode mode = this.m_mode;
		return mode == DemoMode.BLIZZCON_2013;
	}

	// Token: 0x0600032F RID: 815 RVA: 0x0000F44C File Offset: 0x0000D64C
	public bool ArenaIs1WinMode()
	{
		DemoMode mode = this.m_mode;
		return mode == DemoMode.BLIZZCON_2013;
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0000F470 File Offset: 0x0000D670
	public bool ShouldShowWelcomeQuests()
	{
		switch (this.m_mode)
		{
		case DemoMode.BLIZZCON_2013:
		case DemoMode.BLIZZCON_2014:
		case DemoMode.BLIZZCON_2015:
		case DemoMode.ANNOUNCEMENT_5_0:
			return false;
		}
		return true;
	}

	// Token: 0x06000331 RID: 817 RVA: 0x0000F4A8 File Offset: 0x0000D6A8
	public DemoMode GetMode()
	{
		return this.m_mode;
	}

	// Token: 0x06000332 RID: 818 RVA: 0x0000F4B0 File Offset: 0x0000D6B0
	public void SetMode(DemoMode mode)
	{
		this.m_mode = mode;
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0000F4B9 File Offset: 0x0000D6B9
	public void SetModeFromString(string modeString)
	{
		this.m_mode = this.GetModeFromString(modeString);
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0000F4C8 File Offset: 0x0000D6C8
	public DemoMode GetModeFromString(string modeString)
	{
		DemoMode result;
		try
		{
			result = EnumUtils.GetEnum<DemoMode>(modeString, 5);
		}
		catch (Exception)
		{
			result = DemoMode.NONE;
		}
		return result;
	}

	// Token: 0x06000335 RID: 821 RVA: 0x0000F508 File Offset: 0x0000D708
	public void CreateDemoText(string demoText)
	{
		this.CreateDemoText(demoText, false, false);
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0000F513 File Offset: 0x0000D713
	public void CreateDemoText(string demoText, bool unclickable)
	{
		this.CreateDemoText(demoText, unclickable, false);
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0000F520 File Offset: 0x0000D720
	public void CreateDemoText(string demoText, bool unclickable, bool shouldDoArenaInstruction)
	{
		if (this.m_demoText != null)
		{
			return;
		}
		this.m_shouldGiveArenaInstruction = shouldDoArenaInstruction;
		this.m_nextTipUnclickable = unclickable;
		GameObject gameObject = AssetLoader.Get().LoadGameObject("DemoText", true, false);
		OverlayUI.Get().AddGameObject(gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		this.m_demoText = gameObject.GetComponent<Notification>();
		this.m_demoText.ChangeText(demoText);
		UniversalInputManager.Get().SetSystemDialogActive(true);
		PegUIElement componentInChildren = gameObject.transform.GetComponentInChildren<PegUIElement>();
		componentInChildren.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.RemoveDemoTextDialog));
		if (this.m_nextTipUnclickable)
		{
			this.m_nextTipUnclickable = false;
			this.MakeDemoTextClickable(false);
		}
	}

	// Token: 0x06000338 RID: 824 RVA: 0x0000F5C9 File Offset: 0x0000D7C9
	public void ChangeDemoText(string demoText)
	{
		this.m_demoText.ChangeText(demoText);
	}

	// Token: 0x06000339 RID: 825 RVA: 0x0000F5D7 File Offset: 0x0000D7D7
	public void NextDemoTipIsNewArenaMatch()
	{
		this.m_nextDemoTipIsNewArenaMatch = true;
	}

	// Token: 0x0600033A RID: 826 RVA: 0x0000F5E0 File Offset: 0x0000D7E0
	private void DemoTextLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		this.m_demoText = actorObject.GetComponent<Notification>();
		this.m_demoText.ChangeText((string)callbackData);
		UniversalInputManager.Get().SetSystemDialogActive(true);
		PegUIElement componentInChildren = actorObject.transform.GetComponentInChildren<PegUIElement>();
		componentInChildren.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.RemoveDemoTextDialog));
		if (this.m_nextTipUnclickable)
		{
			this.m_nextTipUnclickable = false;
			this.MakeDemoTextClickable(false);
		}
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0000F64E File Offset: 0x0000D84E
	private void RemoveDemoTextDialog(UIEvent e)
	{
		this.RemoveDemoTextDialog();
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0000F658 File Offset: 0x0000D858
	public void RemoveDemoTextDialog()
	{
		UniversalInputManager.Get().SetSystemDialogActive(false);
		Object.DestroyImmediate(this.m_demoText.gameObject);
		if (this.m_shouldGiveArenaInstruction)
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_FORGE_INST1_19"), "VO_INNKEEPER_FORGE_INST1_19", 3f, null);
			this.m_shouldGiveArenaInstruction = false;
		}
		if (this.m_nextDemoTipIsNewArenaMatch)
		{
			this.m_nextDemoTipIsNewArenaMatch = false;
			this.CreateDemoText(GameStrings.Get("GLUE_BLIZZCON2013_ARENA"), false, true);
		}
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0000F6D8 File Offset: 0x0000D8D8
	public void MakeDemoTextClickable(bool clickable)
	{
		if (!clickable)
		{
			this.m_demoText.transform.GetComponentInChildren<BoxCollider>().enabled = false;
			this.m_demoText.transform.FindChild("continue").gameObject.SetActive(false);
		}
		else
		{
			this.m_demoText.transform.GetComponentInChildren<BoxCollider>().enabled = true;
			this.m_demoText.transform.FindChild("continue").gameObject.SetActive(true);
		}
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0000F75C File Offset: 0x0000D95C
	public void ApplyAppleStoreDemoDefaults()
	{
		Options.Get().SetBool(Option.CONNECT_TO_AURORA, false);
		Options.Get().SetBool(Option.HAS_SEEN_CINEMATIC, true);
		Options.Get().SetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS, TutorialProgress.NOTHING_COMPLETE);
	}

	// Token: 0x0600033F RID: 831 RVA: 0x0000F794 File Offset: 0x0000D994
	public IEnumerator CompleteAppleStoreDemo()
	{
		yield return new WaitForSeconds(3f);
		AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
		info.m_headerText = GameStrings.Get("GLOBAL_DEMO_COMPLETE_HEADER");
		info.m_text = GameStrings.Get("GLOBAL_DEMO_COMPLETE_BODY");
		info.m_showAlertIcon = false;
		info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		info.m_responseCallback = delegate(AlertPopup.Response response, object userData)
		{
			ApplicationMgr.Get().Reset();
		};
		DialogManager.Get().ShowPopup(info);
		yield break;
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0000F7A8 File Offset: 0x0000D9A8
	private void WillReset()
	{
		if (this.m_mode == DemoMode.APPLE_STORE)
		{
			this.ApplyAppleStoreDemoDefaults();
		}
	}

	// Token: 0x0400014B RID: 331
	private const bool LOAD_STORED_SETTING = false;

	// Token: 0x0400014C RID: 332
	private static DemoMgr s_instance;

	// Token: 0x0400014D RID: 333
	private DemoMode m_mode;

	// Token: 0x0400014E RID: 334
	private Notification m_demoText;

	// Token: 0x0400014F RID: 335
	private bool m_shouldGiveArenaInstruction;

	// Token: 0x04000150 RID: 336
	private bool m_nextTipUnclickable;

	// Token: 0x04000151 RID: 337
	private bool m_nextDemoTipIsNewArenaMatch;
}
