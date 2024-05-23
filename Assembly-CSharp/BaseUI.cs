using System;
using System.Collections;
using System.IO;
using UnityEngine;

// Token: 0x0200020C RID: 524
public class BaseUI : MonoBehaviour
{
	// Token: 0x06001FE3 RID: 8163 RVA: 0x0009C234 File Offset: 0x0009A434
	private void Awake()
	{
		BaseUI.s_instance = this;
		ChatMgr chatMgr = Object.Instantiate<ChatMgr>(this.m_Prefabs.m_ChatMgrPrefab);
		chatMgr.transform.parent = base.transform;
		ScreenResizeDetector component = this.m_BnetCamera.GetComponent<ScreenResizeDetector>();
		component.AddSizeChangedListener(new ScreenResizeDetector.SizeChangedCallback(this.OnScreenSizeChanged));
	}

	// Token: 0x06001FE4 RID: 8164 RVA: 0x0009C288 File Offset: 0x0009A488
	private void OnDestroy()
	{
		BaseUI.s_instance = null;
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x0009C290 File Offset: 0x0009A490
	private void Start()
	{
		this.UpdateLayout();
		InnKeepersSpecial.Init();
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x0009C2A0 File Offset: 0x0009A4A0
	private void OnGUI()
	{
		if (Event.current.type == 5)
		{
			KeyCode keyCode = Event.current.keyCode;
			if (keyCode == 316 || keyCode == 317 || keyCode == 294)
			{
				this.TakeScreenshot();
			}
		}
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x0009C2F9 File Offset: 0x0009A4F9
	public static BaseUI Get()
	{
		return BaseUI.s_instance;
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x0009C300 File Offset: 0x0009A500
	public void OnLoggedIn()
	{
		this.m_BnetBar.OnLoggedIn();
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x0009C30D File Offset: 0x0009A50D
	public Camera GetBnetCamera()
	{
		return this.m_BnetCamera;
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x0009C315 File Offset: 0x0009A515
	public Camera GetBnetDialogCamera()
	{
		return this.m_BnetDialogCamera;
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x0009C320 File Offset: 0x0009A520
	public Transform GetAddFriendBone()
	{
		if (!UniversalInputManager.Get().IsTouchMode())
		{
			return this.m_Bones.m_AddFriend;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			return this.m_Bones.m_AddFriendPhoneKeyboard;
		}
		return this.m_Bones.m_AddFriendVirtualKeyboard;
	}

	// Token: 0x06001FEC RID: 8172 RVA: 0x0009C36E File Offset: 0x0009A56E
	public Transform GetRecruitAFriendBone()
	{
		return this.m_Bones.m_RecruitAFriend;
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x0009C37B File Offset: 0x0009A57B
	public Transform GetChatBubbleBone()
	{
		return this.m_Bones.m_ChatBubble;
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x0009C388 File Offset: 0x0009A588
	public Transform GetGameMenuBone(bool withRatings = false)
	{
		if (SceneMgr.Get().IsInGame())
		{
			return this.m_Bones.m_InGameMenu;
		}
		return (!withRatings) ? this.m_Bones.m_BoxMenu : this.m_Bones.m_BoxMenuWithRatings;
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x0009C3D1 File Offset: 0x0009A5D1
	public Transform GetOptionsMenuBone()
	{
		return this.m_Bones.m_OptionsMenu;
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x0009C3E0 File Offset: 0x0009A5E0
	public Transform GetQuickChatBone()
	{
		if ((UniversalInputManager.Get().IsTouchMode() && W8Touch.s_isWindows8OrGreater) || W8Touch.Get().IsVirtualKeyboardVisible())
		{
			return this.m_Bones.m_QuickChatVirtualKeyboard;
		}
		return this.m_Bones.m_QuickChat;
	}

	// Token: 0x06001FF1 RID: 8177 RVA: 0x0009C42C File Offset: 0x0009A62C
	public bool HandleKeyboardInput()
	{
		if (this.m_BnetBar != null && this.m_BnetBar.HandleKeyboardInput())
		{
			return true;
		}
		if (Input.GetKeyUp(BackButton.backKey))
		{
		}
		return false;
	}

	// Token: 0x06001FF2 RID: 8178 RVA: 0x0009C461 File Offset: 0x0009A661
	private void OnScreenSizeChanged(object userData)
	{
		this.UpdateLayout();
	}

	// Token: 0x06001FF3 RID: 8179 RVA: 0x0009C46C File Offset: 0x0009A66C
	private void UpdateLayout()
	{
		this.m_BnetBar.UpdateLayout();
		if (ChatMgr.Get() != null)
		{
			ChatMgr.Get().UpdateLayout();
		}
		if (SplashScreen.Get() != null)
		{
			SplashScreen.Get().UpdateLayout();
		}
	}

	// Token: 0x06001FF4 RID: 8180 RVA: 0x0009C4B8 File Offset: 0x0009A6B8
	private void TakeScreenshot()
	{
		string folderPath = Environment.GetFolderPath(0);
		DateTime now = DateTime.Now;
		string text = string.Format("{0}/Hearthstone Screenshot {1:MM-dd-yy HH.mm.ss}.png", folderPath, now);
		int num = 1;
		while (File.Exists(text))
		{
			text = string.Format("{0}/Hearthstone Screenshot {1:MM-dd-yy HH.mm.ss} {2}.png", folderPath, now, num++);
		}
		UIStatus.Get().HideIfScreenshotMessage();
		Application.CaptureScreenshot(text);
		base.StartCoroutine(this.NotifyOfScreenshotComplete());
		Debug.Log(string.Format("screenshot saved to {0}", text));
	}

	// Token: 0x06001FF5 RID: 8181 RVA: 0x0009C548 File Offset: 0x0009A748
	private IEnumerator NotifyOfScreenshotComplete()
	{
		yield return null;
		UIStatus.Get().AddInfo(GameStrings.Get("GLOBAL_SCREENSHOT_COMPLETE"), true);
		yield break;
	}

	// Token: 0x0400118D RID: 4493
	public BaseUIBones m_Bones;

	// Token: 0x0400118E RID: 4494
	public BaseUIPrefabs m_Prefabs;

	// Token: 0x0400118F RID: 4495
	public Camera m_BnetCamera;

	// Token: 0x04001190 RID: 4496
	public Camera m_BnetDialogCamera;

	// Token: 0x04001191 RID: 4497
	public BnetBar m_BnetBar;

	// Token: 0x04001192 RID: 4498
	public ExistingAccountPopup m_ExistingAccountPopup;

	// Token: 0x04001193 RID: 4499
	private static BaseUI s_instance;
}
