using System;
using System.Collections.Generic;
using bgs;
using UnityEngine;

// Token: 0x020005A0 RID: 1440
public class QuickChatFrame : MonoBehaviour
{
	// Token: 0x060040BE RID: 16574 RVA: 0x001380C0 File Offset: 0x001362C0
	private void Awake()
	{
		this.InitRecentPlayers();
		if (!this.InitReceiver())
		{
			Object.Destroy(base.gameObject);
			return;
		}
		BnetWhisperMgr.Get().AddWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
		BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		this.InitTransform();
		this.InitInputBlocker();
		this.InitLastMessage();
		this.InitChatLogFrame();
		this.InitInput();
		this.ShowInput(true);
	}

	// Token: 0x060040BF RID: 16575 RVA: 0x00138140 File Offset: 0x00136340
	private void Start()
	{
		this.InitRecentPlayerDropdown();
		if (ChatMgr.Get().IsChatLogFrameShown())
		{
			this.ShowChatLogFrame();
		}
		this.UpdateReceiver();
		ChatMgr.Get().OnChatReceiverChanged(this.m_receiver);
	}

	// Token: 0x060040C0 RID: 16576 RVA: 0x00138180 File Offset: 0x00136380
	private void OnDestroy()
	{
		BnetWhisperMgr.Get().RemoveWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		if (UniversalInputManager.Get() != null)
		{
			UniversalInputManager.Get().CancelTextInput(base.gameObject, false);
		}
	}

	// Token: 0x060040C1 RID: 16577 RVA: 0x001381DC File Offset: 0x001363DC
	public ChatLogFrame GetChatLogFrame()
	{
		return this.m_chatLogFrame;
	}

	// Token: 0x060040C2 RID: 16578 RVA: 0x001381E4 File Offset: 0x001363E4
	public BnetPlayer GetReceiver()
	{
		return this.m_receiver;
	}

	// Token: 0x060040C3 RID: 16579 RVA: 0x001381EC File Offset: 0x001363EC
	public void SetReceiver(BnetPlayer player)
	{
		UniversalInputManager.Get().FocusTextInput(base.gameObject);
		if (this.m_receiver == player)
		{
			return;
		}
		this.m_receiver = player;
		this.UpdateReceiver();
		this.m_recentPlayerDropdown.setSelection(player);
		ChatMgr.Get().OnChatReceiverChanged(player);
	}

	// Token: 0x060040C4 RID: 16580 RVA: 0x0013823A File Offset: 0x0013643A
	public void UpdateLayout()
	{
		if (this.m_chatLogFrame != null)
		{
			this.m_chatLogFrame.UpdateLayout();
		}
	}

	// Token: 0x060040C5 RID: 16581 RVA: 0x00138258 File Offset: 0x00136458
	private void InitRecentPlayers()
	{
		this.UpdateRecentPlayers();
	}

	// Token: 0x060040C6 RID: 16582 RVA: 0x00138260 File Offset: 0x00136460
	private void UpdateRecentPlayers()
	{
		this.m_recentPlayers.Clear();
		List<BnetPlayer> recentWhisperPlayers = ChatMgr.Get().GetRecentWhisperPlayers();
		for (int i = 0; i < recentWhisperPlayers.Count; i++)
		{
			BnetPlayer bnetPlayer = recentWhisperPlayers[i];
			this.m_recentPlayers.Add(bnetPlayer);
		}
	}

	// Token: 0x060040C7 RID: 16583 RVA: 0x001382B0 File Offset: 0x001364B0
	private bool InitReceiver()
	{
		this.m_receiver = null;
		if (this.m_recentPlayers.Count == 0)
		{
			string message;
			if (BnetFriendMgr.Get().GetOnlineFriendCount() == 0)
			{
				message = GameStrings.Get("GLOBAL_CHAT_NO_FRIENDS_ONLINE");
			}
			else
			{
				message = GameStrings.Get("GLOBAL_CHAT_NO_RECENT_CONVERSATIONS");
			}
			UIStatus.Get().AddError(message);
			return false;
		}
		this.m_receiver = this.m_recentPlayers[0];
		return true;
	}

	// Token: 0x060040C8 RID: 16584 RVA: 0x00138320 File Offset: 0x00136520
	private void OnWhisper(BnetWhisper whisper, object userData)
	{
		if (this.m_receiver == null)
		{
			return;
		}
		if (!WhisperUtil.IsSpeaker(this.m_receiver, whisper))
		{
			return;
		}
		this.UpdateReceiver();
	}

	// Token: 0x060040C9 RID: 16585 RVA: 0x00138354 File Offset: 0x00136554
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		BnetPlayerChange bnetPlayerChange = changelist.FindChange(this.m_receiver);
		if (bnetPlayerChange == null)
		{
			return;
		}
		BnetPlayer oldPlayer = bnetPlayerChange.GetOldPlayer();
		BnetPlayer newPlayer = bnetPlayerChange.GetNewPlayer();
		if (oldPlayer != null && oldPlayer.IsOnline() == newPlayer.IsOnline())
		{
			return;
		}
		this.UpdateReceiver();
	}

	// Token: 0x060040CA RID: 16586 RVA: 0x001383A4 File Offset: 0x001365A4
	private BnetWhisper FindLastWhisperFromReceiver()
	{
		List<BnetWhisper> whispersWithPlayer = BnetWhisperMgr.Get().GetWhispersWithPlayer(this.m_receiver);
		if (whispersWithPlayer == null)
		{
			return null;
		}
		for (int i = whispersWithPlayer.Count - 1; i >= 0; i--)
		{
			BnetWhisper bnetWhisper = whispersWithPlayer[i];
			if (WhisperUtil.IsSpeaker(this.m_receiver, bnetWhisper))
			{
				return bnetWhisper;
			}
		}
		return null;
	}

	// Token: 0x060040CB RID: 16587 RVA: 0x00138400 File Offset: 0x00136600
	private void InitTransform()
	{
		base.transform.parent = BaseUI.Get().transform;
		this.DefaultChatTransform();
		if ((UniversalInputManager.Get().IsTouchMode() && W8Touch.s_isWindows8OrGreater) || W8Touch.Get().IsVirtualKeyboardVisible())
		{
			this.TransformChatForKeyboard();
		}
	}

	// Token: 0x060040CC RID: 16588 RVA: 0x00138458 File Offset: 0x00136658
	private void InitLastMessage()
	{
		this.m_initialLastMessageTextHeight = this.m_LastMessageText.GetTextWorldSpaceBounds().size.y;
		this.m_initialLastMessageShadowScaleZ = this.m_LastMessageShadow.transform.localScale.z;
	}

	// Token: 0x060040CD RID: 16589 RVA: 0x001384A4 File Offset: 0x001366A4
	private void InitInputBlocker()
	{
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		float worldOffset = this.m_Bones.m_InputBlocker.position.z - base.transform.position.z;
		GameObject gameObject = CameraUtils.CreateInputBlocker(camera, "QuickChatInputBlocker", this, worldOffset);
		this.m_inputBlocker = gameObject.AddComponent<PegUIElement>();
		this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerReleased));
	}

	// Token: 0x060040CE RID: 16590 RVA: 0x00138523 File Offset: 0x00136723
	private void OnInputBlockerReleased(UIEvent e)
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060040CF RID: 16591 RVA: 0x00138530 File Offset: 0x00136730
	private void InitChatLogFrame()
	{
		this.m_ChatLogButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChatLogButtonReleased));
	}

	// Token: 0x060040D0 RID: 16592 RVA: 0x0013854C File Offset: 0x0013674C
	private void OnChatLogButtonReleased(UIEvent e)
	{
		if (ChatMgr.Get().IsChatLogFrameShown())
		{
			this.HideChatLogFrame();
		}
		else
		{
			this.ShowChatLogFrame();
		}
		this.UpdateReceiver();
		UniversalInputManager.Get().FocusTextInput(base.gameObject);
	}

	// Token: 0x060040D1 RID: 16593 RVA: 0x00138590 File Offset: 0x00136790
	private void ShowChatLogFrame()
	{
		this.m_chatLogFrame = Object.Instantiate<ChatLogFrame>(this.m_Prefabs.m_ChatLogFrame);
		bool flag = base.transform.localScale == BaseUI.Get().m_Bones.m_QuickChatVirtualKeyboard.localScale;
		if ((((UniversalInputManager.Get().IsTouchMode() && W8Touch.s_isWindows8OrGreater) || W8Touch.Get().IsVirtualKeyboardVisible()) && flag) || flag)
		{
			this.DefaultChatTransform();
		}
		this.m_chatLogFrame.transform.parent = base.transform;
		this.m_chatLogFrame.transform.position = this.m_Bones.m_ChatLog.position;
		if ((((UniversalInputManager.Get().IsTouchMode() && W8Touch.s_isWindows8OrGreater) || W8Touch.Get().IsVirtualKeyboardVisible()) && flag) || flag)
		{
			this.TransformChatForKeyboard();
		}
		ChatMgr.Get().OnChatLogFrameShown();
	}

	// Token: 0x060040D2 RID: 16594 RVA: 0x0013868C File Offset: 0x0013688C
	private void HideChatLogFrame()
	{
		Object.Destroy(this.m_chatLogFrame.gameObject);
		this.m_chatLogFrame = null;
		ChatMgr.Get().OnChatLogFrameHidden();
	}

	// Token: 0x060040D3 RID: 16595 RVA: 0x001386B0 File Offset: 0x001368B0
	private void InitRecentPlayerDropdown()
	{
		this.m_recentPlayerDropdown = Object.Instantiate<DropdownControl>(this.m_Prefabs.m_Dropdown);
		this.m_recentPlayerDropdown.transform.parent = base.transform;
		this.m_recentPlayerDropdown.transform.position = this.m_Bones.m_RecentPlayerDropdown.position;
		this.m_recentPlayerDropdown.setItemTextCallback(new DropdownControl.itemTextCallback(this.OnRecentPlayerDropdownText));
		this.m_recentPlayerDropdown.setItemChosenCallback(new DropdownControl.itemChosenCallback(this.OnRecentPlayerDropdownItemChosen));
		this.UpdateRecentPlayerDropdown();
		this.m_recentPlayerDropdown.setSelection(this.m_receiver);
	}

	// Token: 0x060040D4 RID: 16596 RVA: 0x00138750 File Offset: 0x00136950
	private void UpdateRecentPlayerDropdown()
	{
		this.m_recentPlayerDropdown.clearItems();
		for (int i = 0; i < this.m_recentPlayers.Count; i++)
		{
			this.m_recentPlayerDropdown.addItem(this.m_recentPlayers[i]);
		}
	}

	// Token: 0x060040D5 RID: 16597 RVA: 0x0013879C File Offset: 0x0013699C
	private string OnRecentPlayerDropdownText(object val)
	{
		BnetPlayer friend = (BnetPlayer)val;
		return FriendUtils.GetUniqueName(friend);
	}

	// Token: 0x060040D6 RID: 16598 RVA: 0x001387B8 File Offset: 0x001369B8
	private void OnRecentPlayerDropdownItemChosen(object selection, object prevSelection)
	{
		BnetPlayer receiver = (BnetPlayer)selection;
		this.SetReceiver(receiver);
	}

	// Token: 0x060040D7 RID: 16599 RVA: 0x001387D4 File Offset: 0x001369D4
	private void UpdateReceiver()
	{
		this.UpdateLastMessage();
		if (this.m_chatLogFrame != null)
		{
			this.m_chatLogFrame.Receiver = this.m_receiver;
		}
	}

	// Token: 0x060040D8 RID: 16600 RVA: 0x0013880C File Offset: 0x00136A0C
	private void UpdateLastMessage()
	{
		if (this.m_chatLogFrame != null)
		{
			this.HideLastMessage();
			return;
		}
		BnetWhisper bnetWhisper = this.FindLastWhisperFromReceiver();
		if (bnetWhisper == null)
		{
			this.HideLastMessage();
			return;
		}
		this.m_LastMessageText.gameObject.SetActive(true);
		this.m_LastMessageText.Text = ChatUtils.GetMessage(bnetWhisper);
		TransformUtil.SetPoint(this.m_LastMessageText, Anchor.BOTTOM_LEFT, this.m_Bones.m_LastMessage, Anchor.TOP_LEFT);
		this.m_ReceiverNameText.gameObject.SetActive(true);
		if (this.m_receiver.IsOnline())
		{
			this.m_ReceiverNameText.TextColor = GameColors.PLAYER_NAME_ONLINE;
		}
		else
		{
			this.m_ReceiverNameText.TextColor = GameColors.PLAYER_NAME_OFFLINE;
		}
		this.m_ReceiverNameText.Text = FriendUtils.GetUniqueName(this.m_receiver);
		TransformUtil.SetPoint(this.m_ReceiverNameText, Anchor.BOTTOM_LEFT, this.m_LastMessageText, Anchor.TOP_LEFT);
		this.m_LastMessageShadow.SetActive(true);
		Bounds textWorldSpaceBounds = this.m_LastMessageText.GetTextWorldSpaceBounds();
		Bounds textWorldSpaceBounds2 = this.m_ReceiverNameText.GetTextWorldSpaceBounds();
		float num = Mathf.Max(textWorldSpaceBounds.max.y, textWorldSpaceBounds2.max.y);
		float num2 = Mathf.Min(textWorldSpaceBounds.min.y, textWorldSpaceBounds2.min.y);
		float num3 = num - num2;
		float z = num3 * this.m_initialLastMessageShadowScaleZ / this.m_initialLastMessageTextHeight;
		TransformUtil.SetLocalScaleZ(this.m_LastMessageShadow, z);
	}

	// Token: 0x060040D9 RID: 16601 RVA: 0x00138984 File Offset: 0x00136B84
	private void HideLastMessage()
	{
		this.m_ReceiverNameText.gameObject.SetActive(false);
		this.m_LastMessageText.gameObject.SetActive(false);
		this.m_LastMessageShadow.SetActive(false);
	}

	// Token: 0x060040DA RID: 16602 RVA: 0x001389C0 File Offset: 0x00136BC0
	private void CyclePrevReceiver()
	{
		int num = this.m_recentPlayers.FindIndex((BnetPlayer currReceiver) => this.m_receiver == currReceiver);
		BnetPlayer receiver;
		if (num == 0)
		{
			receiver = this.m_recentPlayers[this.m_recentPlayers.Count - 1];
		}
		else
		{
			receiver = this.m_recentPlayers[num - 1];
		}
		this.SetReceiver(receiver);
	}

	// Token: 0x060040DB RID: 16603 RVA: 0x00138A20 File Offset: 0x00136C20
	private void CycleNextReceiver()
	{
		int num = this.m_recentPlayers.FindIndex((BnetPlayer currReceiver) => this.m_receiver == currReceiver);
		BnetPlayer receiver;
		if (num == this.m_recentPlayers.Count - 1)
		{
			receiver = this.m_recentPlayers[0];
		}
		else
		{
			receiver = this.m_recentPlayers[num + 1];
		}
		this.SetReceiver(receiver);
	}

	// Token: 0x060040DC RID: 16604 RVA: 0x00138A80 File Offset: 0x00136C80
	private void InitInput()
	{
		FontDef fontDef = FontTable.Get().GetFontDef(this.m_InputFont);
		if (fontDef == null)
		{
			this.m_localizedInputFont = this.m_InputFont;
		}
		else
		{
			this.m_localizedInputFont = fontDef.m_Font;
		}
	}

	// Token: 0x060040DD RID: 16605 RVA: 0x00138AC8 File Offset: 0x00136CC8
	private void ShowInput(bool fromAwake)
	{
		Camera bnetCamera = BaseUI.Get().GetBnetCamera();
		Rect rect = CameraUtils.CreateGUIViewportRect(bnetCamera, this.m_Bones.m_InputTopLeft, this.m_Bones.m_InputBottomRight);
		if (Localization.GetLocale() == Locale.thTH)
		{
			Vector3 vector = bnetCamera.WorldToViewportPoint(this.m_Bones.m_InputTopLeft.position);
			Vector3 vector2 = bnetCamera.WorldToViewportPoint(this.m_Bones.m_InputBottomRight.position);
			float num = (vector.y - vector2.y) * 0.1f;
			vector..ctor(vector.x, vector.y - num, vector.z);
			vector2..ctor(vector2.x, vector2.y + num, vector2.z);
			rect..ctor(vector.x, 1f - vector.y, vector2.x - vector.x, vector.y - vector2.y);
		}
		UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams
		{
			m_owner = base.gameObject,
			m_rect = rect,
			m_preprocessCallback = new UniversalInputManager.TextInputPreprocessCallback(this.OnInputPreprocess),
			m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(this.OnInputComplete),
			m_canceledCallback = new UniversalInputManager.TextInputCanceledCallback(this.OnInputCanceled),
			m_font = this.m_localizedInputFont,
			m_maxCharacters = 512,
			m_touchScreenKeyboardHideInput = true,
			m_showVirtualKeyboard = fromAwake,
			m_hideVirtualKeyboardOnComplete = fromAwake
		};
		UniversalInputManager.Get().UseTextInput(parms, false);
	}

	// Token: 0x060040DE RID: 16606 RVA: 0x00138C68 File Offset: 0x00136E68
	private bool OnInputPreprocess(Event e)
	{
		if (this.m_recentPlayers.Count < 2)
		{
			return false;
		}
		if (e.type != 4)
		{
			return false;
		}
		KeyCode keyCode = e.keyCode;
		bool flag = (e.modifiers & 1) != 0;
		if (keyCode == 273 || (keyCode == 9 && flag))
		{
			this.CyclePrevReceiver();
			return true;
		}
		if (keyCode == 274 || keyCode == 9)
		{
			this.CycleNextReceiver();
			return true;
		}
		return false;
	}

	// Token: 0x060040DF RID: 16607 RVA: 0x00138CEC File Offset: 0x00136EEC
	private void OnInputComplete(string input)
	{
		if (!string.IsNullOrEmpty(input))
		{
			if (this.m_receiver.IsOnline())
			{
				BnetWhisperMgr.Get().SendWhisper(this.m_receiver, input);
				ChatMgr.Get().AddRecentWhisperPlayerToTop(this.m_receiver);
			}
			else if (ChatMgr.Get().IsChatLogFrameShown())
			{
				if (!BnetWhisperMgr.Get().SendWhisper(this.m_receiver, input))
				{
					this.m_chatLogFrame.m_chatLog.OnWhisperFailed();
				}
				ChatMgr.Get().AddRecentWhisperPlayerToTop(this.m_receiver);
			}
			else
			{
				string message = GameStrings.Format("GLOBAL_CHAT_RECEIVER_OFFLINE", new object[]
				{
					this.m_receiver.GetBestName()
				});
				UIStatus.Get().AddError(message);
			}
		}
		if (ChatMgr.Get().IsChatLogFrameShown())
		{
			this.ShowInput(false);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060040E0 RID: 16608 RVA: 0x00138DD5 File Offset: 0x00136FD5
	private void OnInputCanceled(bool userRequested, GameObject requester)
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060040E1 RID: 16609 RVA: 0x00138DE4 File Offset: 0x00136FE4
	private void DefaultChatTransform()
	{
		base.transform.position = BaseUI.Get().m_Bones.m_QuickChat.position;
		base.transform.localScale = BaseUI.Get().m_Bones.m_QuickChat.localScale;
		if (this.m_chatLogFrame != null)
		{
			this.m_chatLogFrame.UpdateLayout();
		}
	}

	// Token: 0x060040E2 RID: 16610 RVA: 0x00138E4C File Offset: 0x0013704C
	private void TransformChatForKeyboard()
	{
		base.transform.position = BaseUI.Get().m_Bones.m_QuickChatVirtualKeyboard.position;
		base.transform.localScale = BaseUI.Get().m_Bones.m_QuickChatVirtualKeyboard.localScale;
		this.m_Prefabs.m_Dropdown.transform.localScale = new Vector3(50f, 50f, 50f);
		if (this.m_chatLogFrame != null)
		{
			this.m_chatLogFrame.UpdateLayout();
		}
	}

	// Token: 0x04002946 RID: 10566
	public QuickChatFrameBones m_Bones;

	// Token: 0x04002947 RID: 10567
	public QuickChatFramePrefabs m_Prefabs;

	// Token: 0x04002948 RID: 10568
	public GameObject m_Background;

	// Token: 0x04002949 RID: 10569
	public UberText m_ReceiverNameText;

	// Token: 0x0400294A RID: 10570
	public UberText m_LastMessageText;

	// Token: 0x0400294B RID: 10571
	public GameObject m_LastMessageShadow;

	// Token: 0x0400294C RID: 10572
	public PegUIElement m_ChatLogButton;

	// Token: 0x0400294D RID: 10573
	public Font m_InputFont;

	// Token: 0x0400294E RID: 10574
	private DropdownControl m_recentPlayerDropdown;

	// Token: 0x0400294F RID: 10575
	private ChatLogFrame m_chatLogFrame;

	// Token: 0x04002950 RID: 10576
	private PegUIElement m_inputBlocker;

	// Token: 0x04002951 RID: 10577
	private List<BnetPlayer> m_recentPlayers = new List<BnetPlayer>();

	// Token: 0x04002952 RID: 10578
	private BnetPlayer m_receiver;

	// Token: 0x04002953 RID: 10579
	private float m_initialLastMessageTextHeight;

	// Token: 0x04002954 RID: 10580
	private float m_initialLastMessageShadowScaleZ;

	// Token: 0x04002955 RID: 10581
	private Font m_localizedInputFont;
}
