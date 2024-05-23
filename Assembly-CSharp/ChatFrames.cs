using System;
using bgs;
using UnityEngine;

// Token: 0x0200051C RID: 1308
public class ChatFrames : MonoBehaviour
{
	// Token: 0x1700044D RID: 1101
	// (get) Token: 0x06003CAE RID: 15534 RVA: 0x00125B8E File Offset: 0x00123D8E
	// (set) Token: 0x06003CAF RID: 15535 RVA: 0x00125B9C File Offset: 0x00123D9C
	public BnetPlayer Receiver
	{
		get
		{
			return this.chatLogFrame.Receiver;
		}
		set
		{
			this.chatLogFrame.Receiver = value;
			if (this.chatLogFrame.Receiver == null)
			{
				ChatMgr.Get().CloseChatUI();
			}
			this.OnFramesMoved();
		}
	}

	// Token: 0x06003CB0 RID: 15536 RVA: 0x00125BD8 File Offset: 0x00123DD8
	private void Awake()
	{
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		BnetEventMgr.Get().AddChangeListener(new BnetEventMgr.ChangeCallback(this.OnBnetEventOccurred));
		this.chatLogFrame.CloseButtonReleased += new Action(this.OnCloseButtonReleased);
	}

	// Token: 0x06003CB1 RID: 15537 RVA: 0x00125C2C File Offset: 0x00123E2C
	private void OnDestroy()
	{
		if (SceneMgr.Get() != null)
		{
			SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		}
		if (BnetEventMgr.Get() != null)
		{
			BnetEventMgr.Get().RemoveChangeListener(new BnetEventMgr.ChangeCallback(this.OnBnetEventOccurred));
		}
		this.OnFramesMoved();
	}

	// Token: 0x06003CB2 RID: 15538 RVA: 0x00125C87 File Offset: 0x00123E87
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003CB3 RID: 15539 RVA: 0x00125C95 File Offset: 0x00123E95
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003CB4 RID: 15540 RVA: 0x00125CA4 File Offset: 0x00123EA4
	private void Update()
	{
		bool flag = DialogManager.Get().ShowingDialog();
		if (flag != this.wasShowingDialog)
		{
			if (flag && this.chatLogFrame.HasFocus)
			{
				this.OnPopupOpened();
			}
			else if (!flag && !ChatMgr.Get().FriendListFrame.ShowingAddFriendFrame && !this.chatLogFrame.HasFocus)
			{
				this.OnPopupClosed();
			}
			this.wasShowingDialog = flag;
		}
	}

	// Token: 0x06003CB5 RID: 15541 RVA: 0x00125D20 File Offset: 0x00123F20
	public void Back()
	{
		if (DialogManager.Get().ShowingDialog())
		{
			return;
		}
		if (ChatMgr.Get().FriendListFrame.ShowingAddFriendFrame)
		{
			ChatMgr.Get().FriendListFrame.CloseAddFriendFrame();
		}
		else if (this.Receiver != null)
		{
			this.Receiver = null;
		}
		else
		{
			ChatMgr.Get().CloseChatUI();
		}
	}

	// Token: 0x06003CB6 RID: 15542 RVA: 0x00125D86 File Offset: 0x00123F86
	private void OnFramesMoved()
	{
		if (ChatMgr.Get() != null)
		{
			ChatMgr.Get().OnChatFramesMoved();
		}
	}

	// Token: 0x06003CB7 RID: 15543 RVA: 0x00125DA2 File Offset: 0x00123FA2
	private void OnCloseButtonReleased()
	{
		ChatMgr.Get().CloseChatUI();
		if (UniversalInputManager.UsePhoneUI)
		{
			ChatMgr.Get().ShowFriendsList();
		}
	}

	// Token: 0x06003CB8 RID: 15544 RVA: 0x00125DC7 File Offset: 0x00123FC7
	private void OnPopupOpened()
	{
		if (this.chatLogFrame.HasFocus)
		{
			this.chatLogFrame.Focus(false);
		}
	}

	// Token: 0x06003CB9 RID: 15545 RVA: 0x00125DE5 File Offset: 0x00123FE5
	private void OnPopupClosed()
	{
		if (this.Receiver != null)
		{
			this.chatLogFrame.Focus(true);
		}
	}

	// Token: 0x06003CBA RID: 15546 RVA: 0x00125DFE File Offset: 0x00123FFE
	private void OnBnetEventOccurred(BattleNet.BnetEvent bnetEvent, object userData)
	{
		if (bnetEvent == null)
		{
			ChatMgr.Get().CleanUp();
		}
	}

	// Token: 0x06003CBB RID: 15547 RVA: 0x00125E10 File Offset: 0x00124010
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		if (mode == SceneMgr.Mode.FATAL_ERROR)
		{
			ChatMgr.Get().CleanUp();
		}
	}

	// Token: 0x040026A5 RID: 9893
	public MobileChatLogFrame chatLogFrame;

	// Token: 0x040026A6 RID: 9894
	private bool wasShowingDialog;
}
