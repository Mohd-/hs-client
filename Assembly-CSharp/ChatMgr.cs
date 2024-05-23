using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using bgs;
using UnityEngine;

// Token: 0x0200020D RID: 525
public class ChatMgr : MonoBehaviour
{
	// Token: 0x17000331 RID: 817
	// (get) Token: 0x06001FF7 RID: 8183 RVA: 0x0009C5BA File Offset: 0x0009A7BA
	public FriendListFrame FriendListFrame
	{
		get
		{
			return this.m_friendListFrame;
		}
	}

	// Token: 0x17000332 RID: 818
	// (get) Token: 0x06001FF8 RID: 8184 RVA: 0x0009C5C2 File Offset: 0x0009A7C2
	public Rect KeyboardRect
	{
		get
		{
			return this.keyboardArea;
		}
	}

	// Token: 0x06001FF9 RID: 8185 RVA: 0x0009C5CC File Offset: 0x0009A7CC
	private void Awake()
	{
		ChatMgr.s_instance = this;
		BnetWhisperMgr.Get().AddWhisperListener(new BnetWhisperMgr.WhisperCallback(this.OnWhisper));
		BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		W8Touch.Get().VirtualKeyboardDidShow += new Action(this.OnKeyboardShow);
		W8Touch.Get().VirtualKeyboardDidHide += new Action(this.OnKeyboardHide);
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
		this.InitCloseCatcher();
		this.InitChatLogUI();
	}

	// Token: 0x06001FFA RID: 8186 RVA: 0x0009C674 File Offset: 0x0009A874
	private void OnDestroy()
	{
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		W8Touch.Get().VirtualKeyboardDidShow -= new Action(this.OnKeyboardShow);
		W8Touch.Get().VirtualKeyboardDidHide -= new Action(this.OnKeyboardHide);
		ChatMgr.s_instance = null;
	}

	// Token: 0x06001FFB RID: 8187 RVA: 0x0009C6CC File Offset: 0x0009A8CC
	private void Start()
	{
		SoundManager.Get().Load("receive_message");
		this.UpdateLayout();
		if (W8Touch.Get().IsVirtualKeyboardVisible())
		{
			this.OnKeyboardShow();
		}
	}

	// Token: 0x06001FFC RID: 8188 RVA: 0x0009C704 File Offset: 0x0009A904
	private void Update()
	{
		Rect rect = this.keyboardArea;
		this.keyboardArea = TextField.KeyboardArea;
		if (this.keyboardArea != rect)
		{
			this.UpdateLayout();
		}
	}

	// Token: 0x06001FFD RID: 8189 RVA: 0x0009C73A File Offset: 0x0009A93A
	public static ChatMgr Get()
	{
		return ChatMgr.s_instance;
	}

	// Token: 0x06001FFE RID: 8190 RVA: 0x0009C741 File Offset: 0x0009A941
	private void WillReset()
	{
		this.CleanUp();
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
	}

	// Token: 0x06001FFF RID: 8191 RVA: 0x0009C760 File Offset: 0x0009A960
	private ChatMgr.KeyboardState ComputeKeyboardState()
	{
		if (this.keyboardArea.height > 0f)
		{
			float y = this.keyboardArea.y;
			float num = (float)Screen.height - this.keyboardArea.yMax;
			return (y <= num) ? ChatMgr.KeyboardState.Above : ChatMgr.KeyboardState.Below;
		}
		return ChatMgr.KeyboardState.None;
	}

	// Token: 0x06002000 RID: 8192 RVA: 0x0009C7B4 File Offset: 0x0009A9B4
	private void InitCloseCatcher()
	{
		Camera bnetCamera = BaseUI.Get().GetBnetCamera();
		GameObject gameObject = CameraUtils.CreateInputBlocker(bnetCamera, "CloseCatcher", this);
		this.m_closeCatcher = gameObject.AddComponent<PegUIElement>();
		this.m_closeCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCloseCatcherRelease));
		TransformUtil.SetPosZ(this.m_closeCatcher, base.transform.position.z + 100f);
		this.m_closeCatcher.gameObject.SetActive(false);
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x0009C834 File Offset: 0x0009AA34
	private void InitChatLogUI()
	{
		if (this.IsMobilePlatform())
		{
			this.m_chatLogUI = new MobileChatLogUI();
		}
		else
		{
			this.m_chatLogUI = new DesktopChatLogUI();
		}
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x0009C868 File Offset: 0x0009AA68
	private FriendListFrame CreateFriendsListUI()
	{
		string name = (!UniversalInputManager.UsePhoneUI) ? "FriendListFrame" : "FriendListFrame_phone";
		GameObject gameObject = AssetLoader.Get().LoadGameObject(name, true, false);
		if (gameObject == null)
		{
			return null;
		}
		gameObject.transform.parent = base.transform;
		return gameObject.GetComponent<FriendListFrame>();
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x0009C8C9 File Offset: 0x0009AAC9
	public void UpdateLayout()
	{
		if (this.m_friendListFrame != null || this.m_chatLogUI.IsShowing)
		{
			this.UpdateLayoutForOnScreenKeyboard();
		}
		this.UpdateChatBubbleParentLayout();
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x0009C8F8 File Offset: 0x0009AAF8
	private void UpdateLayoutForOnScreenKeyboard()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.UpdateLayoutForOnScreenKeyboardOnPhone();
			return;
		}
		this.keyboardState = this.ComputeKeyboardState();
		bool flag = this.IsMobilePlatform();
		Camera bnetCamera = BaseUI.Get().GetBnetCamera();
		float num = bnetCamera.orthographicSize * 2f;
		float num2 = num * bnetCamera.aspect;
		float num3 = bnetCamera.transform.position.y + num / 2f;
		float num4 = bnetCamera.transform.position.x - num2 / 2f;
		float num5 = 0f;
		if (this.keyboardState != ChatMgr.KeyboardState.None && flag)
		{
			num5 = num * this.keyboardArea.height / (float)Screen.height;
		}
		float num6 = 0f;
		if (this.m_friendListFrame != null)
		{
			OrientedBounds orientedBounds = TransformUtil.ComputeOrientedWorldBounds(BaseUI.Get().m_BnetBar.m_friendButton.gameObject, true);
			if (flag)
			{
				float num7 = (this.keyboardState != ChatMgr.KeyboardState.Below) ? (orientedBounds.Extents[1].y * 2f) : num5;
				this.m_friendListFrame.SetWorldHeight(num - num7);
			}
			OrientedBounds orientedBounds2 = TransformUtil.ComputeOrientedWorldBounds(this.m_friendListFrame.gameObject, true);
			if (!flag || this.keyboardState != ChatMgr.KeyboardState.Below)
			{
				float x = num4 + orientedBounds2.Extents[0].x + orientedBounds2.CenterOffset.x + this.m_friendsListXOffset;
				float y = orientedBounds.GetTrueCenterPosition().y + orientedBounds.Extents[1].y + orientedBounds2.Extents[1].y + orientedBounds2.CenterOffset.y;
				this.m_friendListFrame.SetWorldPosition(x, y);
			}
			else if (flag && this.keyboardState == ChatMgr.KeyboardState.Below)
			{
				float x2 = num4 + orientedBounds2.Extents[0].x + orientedBounds2.CenterOffset.x + this.m_friendsListXOffset;
				float y2 = bnetCamera.transform.position.y - num / 2f + num5 + orientedBounds2.Extents[1].y + orientedBounds2.CenterOffset.y;
				this.m_friendListFrame.SetWorldPosition(x2, y2);
			}
			num6 = orientedBounds2.Extents[0].magnitude * 2f;
		}
		if (this.m_chatLogUI.IsShowing)
		{
			ChatFrames component = this.m_chatLogUI.GameObject.GetComponent<ChatFrames>();
			if (component != null)
			{
				float num8 = num3;
				if (this.keyboardState == ChatMgr.KeyboardState.Above)
				{
					num8 -= num5;
				}
				float height = num - num5;
				float num9 = num4;
				if (!UniversalInputManager.UsePhoneUI)
				{
					num9 += num6 + this.m_friendsListXOffset + this.m_chatLogXOffset;
				}
				float num10 = num2;
				if (!UniversalInputManager.UsePhoneUI)
				{
					num10 -= num6 + this.m_friendsListXOffset + this.m_chatLogXOffset;
				}
				component.chatLogFrame.SetWorldRect(num9, num8, num10, height);
			}
		}
		this.OnChatFramesMoved();
	}

	// Token: 0x06002005 RID: 8197 RVA: 0x0009CC48 File Offset: 0x0009AE48
	private void UpdateLayoutForOnScreenKeyboardOnPhone()
	{
		this.keyboardState = this.ComputeKeyboardState();
		bool flag = UniversalInputManager.Get().IsTouchMode();
		Camera bnetCamera = BaseUI.Get().GetBnetCamera();
		float num = bnetCamera.orthographicSize * 2f;
		float num2 = num * bnetCamera.aspect;
		float num3 = bnetCamera.transform.position.y + num / 2f;
		float num4 = bnetCamera.transform.position.x - num2 / 2f;
		float num5 = 0f;
		float num6 = 0f;
		if (this.keyboardState != ChatMgr.KeyboardState.None && flag)
		{
			num5 = num * this.keyboardArea.height / (float)Screen.height;
			num6 = num2 * this.keyboardArea.width / (float)Screen.width;
		}
		if (this.m_friendListFrame != null)
		{
			float x = num4 + this.m_friendsListXOffset;
			float y = num3 + this.m_friendsListYOffset;
			float width = this.m_friendsListWidth + this.m_friendsListWidthPadding;
			float height = num + this.m_friendsListHeightPadding;
			this.m_friendListFrame.SetWorldRect(x, y, width, height);
		}
		if (this.m_chatLogUI.IsShowing)
		{
			ChatFrames component = this.m_chatLogUI.GameObject.GetComponent<ChatFrames>();
			if (component != null)
			{
				float num7 = num3;
				if (this.keyboardState == ChatMgr.KeyboardState.Above)
				{
					num7 -= num5;
				}
				float height2 = num - num5;
				float num8 = num4;
				if (!UniversalInputManager.UsePhoneUI)
				{
					num8 += this.m_friendsListWidth;
				}
				float num9 = (num6 != 0f) ? num6 : num2;
				if (!UniversalInputManager.UsePhoneUI)
				{
					num9 -= this.m_friendsListWidth;
				}
				component.chatLogFrame.SetWorldRect(num8, num7, num9, height2);
			}
		}
		this.OnChatFramesMoved();
	}

	// Token: 0x06002006 RID: 8198 RVA: 0x0009CE3E File Offset: 0x0009B03E
	public bool IsChatLogFrameShown()
	{
		if (this.IsMobilePlatform())
		{
			return this.m_chatLogUI.IsShowing;
		}
		return this.m_chatLogFrameShown;
	}

	// Token: 0x06002007 RID: 8199 RVA: 0x0009CE5D File Offset: 0x0009B05D
	private void OnCloseCatcherRelease(UIEvent e)
	{
		this.CloseChatUI();
	}

	// Token: 0x06002008 RID: 8200 RVA: 0x0009CE65 File Offset: 0x0009B065
	public bool IsFriendListShowing()
	{
		return !(this.m_friendListFrame == null) && this.m_friendListFrame.gameObject.activeSelf;
	}

	// Token: 0x06002009 RID: 8201 RVA: 0x0009CE90 File Offset: 0x0009B090
	public void ShowFriendsList()
	{
		if (this.m_friendListFrame == null)
		{
			this.m_friendListFrame = this.CreateFriendsListUI();
		}
		this.m_friendListFrame.gameObject.SetActive(true);
		this.m_closeCatcher.gameObject.SetActive(true);
		this.UpdateLayout();
	}

	// Token: 0x0600200A RID: 8202 RVA: 0x0009CEE4 File Offset: 0x0009B0E4
	public void HideFriendsList()
	{
		if (this.IsFriendListShowing())
		{
			this.m_friendListFrame.gameObject.SetActive(false);
		}
		if (this.m_closeCatcher != null)
		{
			this.m_closeCatcher.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600200B RID: 8203 RVA: 0x0009CF30 File Offset: 0x0009B130
	public void GoBack()
	{
		if (this.IsFriendListShowing())
		{
			this.CloseChatUI();
		}
		else if (this.m_chatLogUI.IsShowing)
		{
			this.m_chatLogUI.Hide();
			this.ShowFriendsList();
		}
	}

	// Token: 0x0600200C RID: 8204 RVA: 0x0009CF74 File Offset: 0x0009B174
	public void CloseChatUI()
	{
		if (this.m_chatLogUI.IsShowing)
		{
			this.m_chatLogUI.Hide();
		}
		this.DestroyFriendListFrame();
	}

	// Token: 0x0600200D RID: 8205 RVA: 0x0009CF97 File Offset: 0x0009B197
	public void CleanUp()
	{
		this.DestroyFriendListFrame();
	}

	// Token: 0x0600200E RID: 8206 RVA: 0x0009CF9F File Offset: 0x0009B19F
	private void DestroyFriendListFrame()
	{
		this.HideFriendsList();
		if (this.m_friendListFrame == null)
		{
			return;
		}
		Object.Destroy(this.m_friendListFrame.gameObject);
		this.m_friendListFrame = null;
	}

	// Token: 0x0600200F RID: 8207 RVA: 0x0009CFD0 File Offset: 0x0009B1D0
	public List<BnetPlayer> GetRecentWhisperPlayers()
	{
		return this.m_recentWhisperPlayers;
	}

	// Token: 0x06002010 RID: 8208 RVA: 0x0009CFD8 File Offset: 0x0009B1D8
	public void AddRecentWhisperPlayerToTop(BnetPlayer player)
	{
		int num = this.m_recentWhisperPlayers.FindIndex((BnetPlayer currPlayer) => currPlayer == player);
		if (num < 0)
		{
			if (this.m_recentWhisperPlayers.Count == 10)
			{
				this.m_recentWhisperPlayers.RemoveAt(this.m_recentWhisperPlayers.Count - 1);
			}
		}
		else
		{
			this.m_recentWhisperPlayers.RemoveAt(num);
		}
		this.m_recentWhisperPlayers.Insert(0, player);
	}

	// Token: 0x06002011 RID: 8209 RVA: 0x0009D060 File Offset: 0x0009B260
	public void AddRecentWhisperPlayerToBottom(BnetPlayer player)
	{
		if (this.m_recentWhisperPlayers.Contains(player))
		{
			return;
		}
		if (this.m_recentWhisperPlayers.Count == 10)
		{
			this.m_recentWhisperPlayers.RemoveAt(this.m_recentWhisperPlayers.Count - 1);
		}
		this.m_recentWhisperPlayers.Add(player);
	}

	// Token: 0x06002012 RID: 8210 RVA: 0x0009D0B5 File Offset: 0x0009B2B5
	public void AddPlayerChatInfoChangedListener(ChatMgr.PlayerChatInfoChangedCallback callback)
	{
		this.AddPlayerChatInfoChangedListener(callback, null);
	}

	// Token: 0x06002013 RID: 8211 RVA: 0x0009D0C0 File Offset: 0x0009B2C0
	public void AddPlayerChatInfoChangedListener(ChatMgr.PlayerChatInfoChangedCallback callback, object userData)
	{
		ChatMgr.PlayerChatInfoChangedListener playerChatInfoChangedListener = new ChatMgr.PlayerChatInfoChangedListener();
		playerChatInfoChangedListener.SetCallback(callback);
		playerChatInfoChangedListener.SetUserData(userData);
		if (this.m_playerChatInfoChangedListeners.Contains(playerChatInfoChangedListener))
		{
			return;
		}
		this.m_playerChatInfoChangedListeners.Add(playerChatInfoChangedListener);
	}

	// Token: 0x06002014 RID: 8212 RVA: 0x0009D0FF File Offset: 0x0009B2FF
	public bool RemovePlayerChatInfoChangedListener(ChatMgr.PlayerChatInfoChangedCallback callback)
	{
		return this.RemovePlayerChatInfoChangedListener(callback, null);
	}

	// Token: 0x06002015 RID: 8213 RVA: 0x0009D10C File Offset: 0x0009B30C
	public bool RemovePlayerChatInfoChangedListener(ChatMgr.PlayerChatInfoChangedCallback callback, object userData)
	{
		ChatMgr.PlayerChatInfoChangedListener playerChatInfoChangedListener = new ChatMgr.PlayerChatInfoChangedListener();
		playerChatInfoChangedListener.SetCallback(callback);
		playerChatInfoChangedListener.SetUserData(userData);
		return this.m_playerChatInfoChangedListeners.Remove(playerChatInfoChangedListener);
	}

	// Token: 0x06002016 RID: 8214 RVA: 0x0009D13C File Offset: 0x0009B33C
	public PlayerChatInfo GetPlayerChatInfo(BnetPlayer player)
	{
		PlayerChatInfo result = null;
		this.m_playerChatInfos.TryGetValue(player, out result);
		return result;
	}

	// Token: 0x06002017 RID: 8215 RVA: 0x0009D15C File Offset: 0x0009B35C
	public PlayerChatInfo RegisterPlayerChatInfo(BnetPlayer player)
	{
		PlayerChatInfo playerChatInfo;
		if (!this.m_playerChatInfos.TryGetValue(player, out playerChatInfo))
		{
			playerChatInfo = new PlayerChatInfo();
			playerChatInfo.SetPlayer(player);
			this.m_playerChatInfos.Add(player, playerChatInfo);
		}
		return playerChatInfo;
	}

	// Token: 0x06002018 RID: 8216 RVA: 0x0009D198 File Offset: 0x0009B398
	public void OnFriendListOpened()
	{
		if (W8Touch.Get().IsVirtualKeyboardVisible())
		{
			this.OnKeyboardShow();
		}
		else
		{
			this.UpdateChatBubbleParentLayout();
		}
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x0009D1C8 File Offset: 0x0009B3C8
	public void OnFriendListClosed()
	{
		if (W8Touch.Get().IsVirtualKeyboardVisible())
		{
			this.OnKeyboardShow();
		}
		else
		{
			this.UpdateChatBubbleParentLayout();
		}
	}

	// Token: 0x0600201A RID: 8218 RVA: 0x0009D1F5 File Offset: 0x0009B3F5
	public void OnFriendListFriendSelected(BnetPlayer friend)
	{
		this.ShowChatForPlayer(friend);
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x0009D1FE File Offset: 0x0009B3FE
	public void OnChatLogFrameShown()
	{
		this.m_chatLogFrameShown = true;
	}

	// Token: 0x0600201C RID: 8220 RVA: 0x0009D207 File Offset: 0x0009B407
	public void OnChatLogFrameHidden()
	{
		this.m_chatLogFrameShown = false;
	}

	// Token: 0x0600201D RID: 8221 RVA: 0x0009D210 File Offset: 0x0009B410
	public void OnChatReceiverChanged(BnetPlayer player)
	{
		this.UpdatePlayerFocusTime(player);
	}

	// Token: 0x0600201E RID: 8222 RVA: 0x0009D219 File Offset: 0x0009B419
	public void OnChatFramesMoved()
	{
		this.UpdateChatBubbleParentLayout();
	}

	// Token: 0x0600201F RID: 8223 RVA: 0x0009D224 File Offset: 0x0009B424
	public bool HandleKeyboardInput()
	{
		if (FatalErrorMgr.Get().HasError())
		{
			return false;
		}
		if (Input.GetKeyUp(27) && this.m_chatLogUI.IsShowing)
		{
			this.m_chatLogUI.Hide();
			return true;
		}
		if (this.IsMobilePlatform() && this.m_chatLogUI.IsShowing && Input.GetKeyUp(27))
		{
			this.m_chatLogUI.GoBack();
			return true;
		}
		return false;
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x0009D2A0 File Offset: 0x0009B4A0
	public void HandleGUIInput()
	{
		if (FatalErrorMgr.Get().HasError())
		{
			return;
		}
		if (this.IsMobilePlatform())
		{
			return;
		}
		this.HandleGUIInputForQuickChat();
	}

	// Token: 0x06002021 RID: 8225 RVA: 0x0009D2D0 File Offset: 0x0009B4D0
	private void OnWhisper(BnetWhisper whisper, object userData)
	{
		BnetPlayer theirPlayer = WhisperUtil.GetTheirPlayer(whisper);
		this.AddRecentWhisperPlayerToTop(theirPlayer);
		PlayerChatInfo playerChatInfo = this.RegisterPlayerChatInfo(WhisperUtil.GetTheirPlayer(whisper));
		try
		{
			if (this.m_chatLogUI.IsShowing && WhisperUtil.IsSpeakerOrReceiver(this.m_chatLogUI.Receiver, whisper))
			{
				if (this.IsMobilePlatform())
				{
					playerChatInfo.SetLastSeenWhisper(whisper);
				}
			}
			else
			{
				this.PopupNewChatBubble(whisper);
			}
		}
		finally
		{
			this.FireChatInfoChangedEvent(playerChatInfo);
		}
	}

	// Token: 0x06002022 RID: 8226 RVA: 0x0009D35C File Offset: 0x0009B55C
	private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
	{
		List<BnetPlayer> removedFriends = changelist.GetRemovedFriends();
		if (removedFriends == null)
		{
			return;
		}
		BnetPlayer friend;
		foreach (BnetPlayer friend2 in removedFriends)
		{
			friend = friend2;
			int num = this.m_recentWhisperPlayers.FindIndex((BnetPlayer player) => friend == player);
			if (num >= 0)
			{
				this.m_recentWhisperPlayers.RemoveAt(num);
			}
		}
	}

	// Token: 0x06002023 RID: 8227 RVA: 0x0009D3F4 File Offset: 0x0009B5F4
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		this.CleanUp();
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x0009D414 File Offset: 0x0009B614
	private void HandleGUIInputForQuickChat()
	{
		if (!this.m_chatLogUI.IsShowing)
		{
			if (Event.current.type == 4 && Event.current.keyCode == 13)
			{
				this.ShowChatForPlayer(this.GetMostRecentWhisperedPlayer());
			}
		}
		else if (Event.current.type == 5 && Event.current.keyCode == 27)
		{
			this.m_chatLogUI.Hide();
		}
	}

	// Token: 0x06002025 RID: 8229 RVA: 0x0009D48F File Offset: 0x0009B68F
	public bool IsMobilePlatform()
	{
		return UniversalInputManager.Get().IsTouchMode() && PlatformSettings.OS != OSCategory.PC;
	}

	// Token: 0x06002026 RID: 8230 RVA: 0x0009D4B0 File Offset: 0x0009B6B0
	private void ShowChatForPlayer(BnetPlayer player)
	{
		if (player != null)
		{
			this.AddRecentWhisperPlayerToTop(player);
			PlayerChatInfo playerChatInfo = this.RegisterPlayerChatInfo(player);
			List<BnetWhisper> whispersWithPlayer = BnetWhisperMgr.Get().GetWhispersWithPlayer(player);
			if (whispersWithPlayer != null)
			{
				playerChatInfo.SetLastSeenWhisper(Enumerable.LastOrDefault<BnetWhisper>(whispersWithPlayer, (BnetWhisper whisper) => WhisperUtil.IsSpeaker(player, whisper)));
				this.FireChatInfoChangedEvent(playerChatInfo);
			}
		}
		if (this.m_chatLogUI.IsShowing)
		{
			this.m_chatLogUI.Hide();
		}
		if (!this.m_chatLogUI.IsShowing)
		{
			if (OptionsMenu.Get() != null && OptionsMenu.Get().IsShown())
			{
				OptionsMenu.Get().Hide(true);
			}
			if (GameMenu.Get() != null && GameMenu.Get().IsShown())
			{
				GameMenu.Get().Hide();
			}
			this.m_chatLogUI.ShowForPlayer(this.GetMostRecentWhisperedPlayer());
			this.UpdateLayout();
			if (UniversalInputManager.UsePhoneUI)
			{
				this.HideFriendsList();
			}
		}
	}

	// Token: 0x06002027 RID: 8231 RVA: 0x0009D5CE File Offset: 0x0009B7CE
	private BnetPlayer GetMostRecentWhisperedPlayer()
	{
		return (this.m_recentWhisperPlayers.Count <= 0) ? null : this.m_recentWhisperPlayers[0];
	}

	// Token: 0x06002028 RID: 8232 RVA: 0x0009D5F4 File Offset: 0x0009B7F4
	private void UpdatePlayerFocusTime(BnetPlayer player)
	{
		PlayerChatInfo playerChatInfo = this.RegisterPlayerChatInfo(player);
		playerChatInfo.SetLastFocusTime(Time.realtimeSinceStartup);
		this.FireChatInfoChangedEvent(playerChatInfo);
	}

	// Token: 0x06002029 RID: 8233 RVA: 0x0009D61C File Offset: 0x0009B81C
	private void FireChatInfoChangedEvent(PlayerChatInfo chatInfo)
	{
		foreach (ChatMgr.PlayerChatInfoChangedListener playerChatInfoChangedListener in this.m_playerChatInfoChangedListeners.ToArray())
		{
			playerChatInfoChangedListener.Fire(chatInfo);
		}
	}

	// Token: 0x0600202A RID: 8234 RVA: 0x0009D654 File Offset: 0x0009B854
	private void UpdateChatBubbleParentLayout()
	{
		if (BaseUI.Get().GetChatBubbleBone() != null)
		{
			this.m_ChatBubbleInfo.m_Parent.transform.position = BaseUI.Get().GetChatBubbleBone().transform.position;
		}
	}

	// Token: 0x0600202B RID: 8235 RVA: 0x0009D6A0 File Offset: 0x0009B8A0
	private void UpdateChatBubbleLayout()
	{
		int count = this.m_chatBubbleFrames.Count;
		if (count == 0)
		{
			return;
		}
		Component dst = this.m_ChatBubbleInfo.m_Parent;
		for (int i = count - 1; i >= 0; i--)
		{
			ChatBubbleFrame chatBubbleFrame = this.m_chatBubbleFrames[i];
			Anchor dstAnchor = (!UniversalInputManager.UsePhoneUI) ? Anchor.TOP_LEFT : Anchor.BOTTOM_LEFT;
			Anchor srcAnchor = (!UniversalInputManager.UsePhoneUI) ? Anchor.BOTTOM_LEFT : Anchor.TOP_LEFT;
			TransformUtil.SetPoint(chatBubbleFrame, srcAnchor, dst, dstAnchor, Vector3.zero);
			dst = chatBubbleFrame;
		}
	}

	// Token: 0x0600202C RID: 8236 RVA: 0x0009D730 File Offset: 0x0009B930
	private void PopupNewChatBubble(BnetWhisper whisper)
	{
		ChatBubbleFrame chatBubbleFrame = this.CreateChatBubble(whisper);
		this.m_chatBubbleFrames.Add(chatBubbleFrame);
		chatBubbleFrame.transform.parent = this.m_ChatBubbleInfo.m_Parent.transform;
		chatBubbleFrame.transform.localScale = chatBubbleFrame.m_ScaleOverride;
		SoundManager.Get().LoadAndPlay("receive_message");
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			chatBubbleFrame.m_VisualRoot.transform.localScale,
			"time",
			this.m_ChatBubbleInfo.m_ScaleInSec,
			"easeType",
			this.m_ChatBubbleInfo.m_ScaleInEaseType,
			"oncomplete",
			"OnChatBubbleScaleInComplete",
			"oncompleteparams",
			chatBubbleFrame,
			"oncompletetarget",
			base.gameObject
		});
		chatBubbleFrame.m_VisualRoot.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
		iTween.ScaleTo(chatBubbleFrame.m_VisualRoot, args);
		this.MoveChatBubbles(chatBubbleFrame);
	}

	// Token: 0x0600202D RID: 8237 RVA: 0x0009D85C File Offset: 0x0009BA5C
	private ChatBubbleFrame CreateChatBubble(BnetWhisper whisper)
	{
		ChatBubbleFrame chatBubbleFrame = this.InstantiateChatBubble(this.m_Prefabs.m_ChatBubbleOneLineFrame, whisper);
		if (!chatBubbleFrame.DoesMessageFit())
		{
			Object.Destroy(chatBubbleFrame.gameObject);
			chatBubbleFrame = this.InstantiateChatBubble(this.m_Prefabs.m_ChatBubbleSmallFrame, whisper);
		}
		SceneUtils.SetLayer(chatBubbleFrame, GameLayer.BattleNetDialog);
		return chatBubbleFrame;
	}

	// Token: 0x0600202E RID: 8238 RVA: 0x0009D8B0 File Offset: 0x0009BAB0
	private ChatBubbleFrame InstantiateChatBubble(ChatBubbleFrame prefab, BnetWhisper whisper)
	{
		ChatBubbleFrame chatBubbleFrame = Object.Instantiate<ChatBubbleFrame>(prefab);
		chatBubbleFrame.SetWhisper(whisper);
		PegUIElement component = chatBubbleFrame.GetComponent<PegUIElement>();
		component.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChatBubbleReleased));
		return chatBubbleFrame;
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x0009D8E8 File Offset: 0x0009BAE8
	private void MoveChatBubbles(ChatBubbleFrame newBubbleFrame)
	{
		Anchor dstAnchor = Anchor.TOP_LEFT;
		Anchor srcAnchor = Anchor.BOTTOM_LEFT;
		if (UniversalInputManager.UsePhoneUI && this.m_ChatBubbleInfo.m_Parent.transform.localPosition.y > -900f)
		{
			dstAnchor = Anchor.BOTTOM_LEFT;
			srcAnchor = Anchor.TOP_LEFT;
		}
		TransformUtil.SetPoint(newBubbleFrame, srcAnchor, this.m_ChatBubbleInfo.m_Parent, dstAnchor, Vector3.zero);
		int count = this.m_chatBubbleFrames.Count;
		if (count == 1)
		{
			return;
		}
		Vector3[] array = new Vector3[count - 1];
		Component dst = newBubbleFrame;
		for (int i = count - 2; i >= 0; i--)
		{
			ChatBubbleFrame chatBubbleFrame = this.m_chatBubbleFrames[i];
			array[i] = chatBubbleFrame.transform.position;
			TransformUtil.SetPoint(chatBubbleFrame, srcAnchor, dst, dstAnchor, Vector3.zero);
			dst = chatBubbleFrame;
		}
		for (int j = count - 2; j >= 0; j--)
		{
			ChatBubbleFrame chatBubbleFrame2 = this.m_chatBubbleFrames[j];
			Hashtable args = iTween.Hash(new object[]
			{
				"islocal",
				true,
				"position",
				chatBubbleFrame2.transform.localPosition,
				"time",
				this.m_ChatBubbleInfo.m_MoveOverSec,
				"easeType",
				this.m_ChatBubbleInfo.m_MoveOverEaseType
			});
			chatBubbleFrame2.transform.position = array[j];
			iTween.Stop(chatBubbleFrame2.gameObject, "move");
			iTween.MoveTo(chatBubbleFrame2.gameObject, args);
		}
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x0009DA90 File Offset: 0x0009BC90
	private void OnChatBubbleScaleInComplete(ChatBubbleFrame bubbleFrame)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			0f,
			"delay",
			this.m_ChatBubbleInfo.m_HoldSec,
			"time",
			this.m_ChatBubbleInfo.m_FadeOutSec,
			"easeType",
			this.m_ChatBubbleInfo.m_FadeOutEaseType,
			"oncomplete",
			"OnChatBubbleFadeOutComplete",
			"oncompleteparams",
			bubbleFrame,
			"oncompletetarget",
			base.gameObject
		});
		iTween.FadeTo(bubbleFrame.gameObject, args);
	}

	// Token: 0x06002031 RID: 8241 RVA: 0x0009DB4E File Offset: 0x0009BD4E
	private void OnChatBubbleFadeOutComplete(ChatBubbleFrame bubbleFrame)
	{
		Object.Destroy(bubbleFrame.gameObject);
		this.m_chatBubbleFrames.Remove(bubbleFrame);
	}

	// Token: 0x06002032 RID: 8242 RVA: 0x0009DB68 File Offset: 0x0009BD68
	private void RemoveAllChatBubbles()
	{
		foreach (ChatBubbleFrame chatBubbleFrame in this.m_chatBubbleFrames)
		{
			Object.Destroy(chatBubbleFrame.gameObject);
		}
		this.m_chatBubbleFrames.Clear();
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x0009DBD4 File Offset: 0x0009BDD4
	private void OnChatBubbleReleased(UIEvent e)
	{
		PegUIElement element = e.GetElement();
		ChatBubbleFrame component = element.GetComponent<ChatBubbleFrame>();
		BnetPlayer theirPlayer = WhisperUtil.GetTheirPlayer(component.GetWhisper());
		this.ShowChatForPlayer(theirPlayer);
		if (UniversalInputManager.UsePhoneUI)
		{
			this.RemoveAllChatBubbles();
		}
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x0009DC18 File Offset: 0x0009BE18
	public void OnKeyboardShow()
	{
		if (this.m_chatLogUI.IsShowing && BaseUI.Get().m_Bones.m_QuickChatVirtualKeyboard.position != this.m_chatLogUI.GameObject.transform.position)
		{
			W8Touch.Get().VirtualKeyboardDidShow -= new Action(this.OnKeyboardShow);
			W8Touch.Get().VirtualKeyboardDidHide -= new Action(this.OnKeyboardHide);
			this.m_chatLogUI.Hide();
			this.m_chatLogUI.ShowForPlayer(this.GetMostRecentWhisperedPlayer());
			W8Touch.Get().VirtualKeyboardDidShow += new Action(this.OnKeyboardShow);
			W8Touch.Get().VirtualKeyboardDidHide += new Action(this.OnKeyboardHide);
		}
		Vector2 vector;
		vector..ctor(0f, (float)(Screen.height - 150));
		GameObject gameObject = BnetBarFriendButton.Get().gameObject;
		TransformUtil.SetPoint(this.m_ChatBubbleInfo.m_Parent, Anchor.BOTTOM_LEFT, gameObject, Anchor.BOTTOM_RIGHT, vector);
		int count = this.m_chatBubbleFrames.Count;
		if (count == 0)
		{
			return;
		}
		Component dst = this.m_ChatBubbleInfo.m_Parent;
		for (int i = count - 1; i >= 0; i--)
		{
			ChatBubbleFrame chatBubbleFrame = this.m_chatBubbleFrames[i];
			TransformUtil.SetPoint(chatBubbleFrame, Anchor.TOP_LEFT, dst, Anchor.BOTTOM_LEFT, Vector3.zero);
			dst = chatBubbleFrame;
		}
	}

	// Token: 0x06002035 RID: 8245 RVA: 0x0009DD70 File Offset: 0x0009BF70
	public void OnKeyboardHide()
	{
		this.UpdateLayout();
		this.UpdateChatBubbleLayout();
	}

	// Token: 0x04001194 RID: 4500
	public ChatMgrPrefabs m_Prefabs;

	// Token: 0x04001195 RID: 4501
	public ChatMgrBubbleInfo m_ChatBubbleInfo;

	// Token: 0x04001196 RID: 4502
	public Float_MobileOverride m_friendsListXOffset;

	// Token: 0x04001197 RID: 4503
	public Float_MobileOverride m_friendsListYOffset;

	// Token: 0x04001198 RID: 4504
	public Float_MobileOverride m_friendsListWidthPadding;

	// Token: 0x04001199 RID: 4505
	public Float_MobileOverride m_friendsListHeightPadding;

	// Token: 0x0400119A RID: 4506
	public float m_chatLogXOffset;

	// Token: 0x0400119B RID: 4507
	public Float_MobileOverride m_friendsListWidth;

	// Token: 0x0400119C RID: 4508
	private static ChatMgr s_instance;

	// Token: 0x0400119D RID: 4509
	private List<ChatBubbleFrame> m_chatBubbleFrames = new List<ChatBubbleFrame>();

	// Token: 0x0400119E RID: 4510
	private IChatLogUI m_chatLogUI;

	// Token: 0x0400119F RID: 4511
	private FriendListFrame m_friendListFrame;

	// Token: 0x040011A0 RID: 4512
	private PegUIElement m_closeCatcher;

	// Token: 0x040011A1 RID: 4513
	private List<BnetPlayer> m_recentWhisperPlayers = new List<BnetPlayer>();

	// Token: 0x040011A2 RID: 4514
	private bool m_chatLogFrameShown;

	// Token: 0x040011A3 RID: 4515
	private Map<BnetPlayer, PlayerChatInfo> m_playerChatInfos = new Map<BnetPlayer, PlayerChatInfo>();

	// Token: 0x040011A4 RID: 4516
	private List<ChatMgr.PlayerChatInfoChangedListener> m_playerChatInfoChangedListeners = new List<ChatMgr.PlayerChatInfoChangedListener>();

	// Token: 0x040011A5 RID: 4517
	private ChatMgr.KeyboardState keyboardState;

	// Token: 0x040011A6 RID: 4518
	private Rect keyboardArea = new Rect(0f, 0f, 0f, 0f);

	// Token: 0x0200050A RID: 1290
	private class PlayerChatInfoChangedListener : EventListener<ChatMgr.PlayerChatInfoChangedCallback>
	{
		// Token: 0x06003BE0 RID: 15328 RVA: 0x00121CD6 File Offset: 0x0011FED6
		public void Fire(PlayerChatInfo chatInfo)
		{
			this.m_callback(chatInfo, this.m_userData);
		}
	}

	// Token: 0x0200050B RID: 1291
	// (Invoke) Token: 0x06003BE2 RID: 15330
	public delegate void PlayerChatInfoChangedCallback(PlayerChatInfo chatInfo, object userData);

	// Token: 0x0200050D RID: 1293
	private enum KeyboardState
	{
		// Token: 0x04002651 RID: 9809
		None,
		// Token: 0x04002652 RID: 9810
		Below,
		// Token: 0x04002653 RID: 9811
		Above
	}
}
