using System;
using System.Collections;
using bgs;
using UnityEngine;

// Token: 0x02000567 RID: 1383
public class FriendListFriendFrame : FriendListBaseFriendFrame
{
	// Token: 0x06003F5F RID: 16223 RVA: 0x00133CE0 File Offset: 0x00131EE0
	protected override void Awake()
	{
		base.Awake();
		this.m_ChallengeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChallengeButtonReleased));
		this.m_rightComponentOrder = new Component[]
		{
			this.m_RecruitUI,
			this.m_ChatIcon,
			this.m_ChallengeButton
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			30f,
			"looptype",
			iTween.LoopType.loop,
			"oncomplete",
			"UpdateFriend",
			"oncompletetarget",
			base.gameObject
		});
		iTween.Timer(base.gameObject, args);
	}

	// Token: 0x06003F60 RID: 16224 RVA: 0x00133D92 File Offset: 0x00131F92
	private void OnEnable()
	{
		this.UpdateFriend();
	}

	// Token: 0x06003F61 RID: 16225 RVA: 0x00133D9A File Offset: 0x00131F9A
	public override bool SetFriend(BnetPlayer player)
	{
		this.m_ChallengeButton.SetPlayer(player);
		return base.SetFriend(player);
	}

	// Token: 0x06003F62 RID: 16226 RVA: 0x00133DB8 File Offset: 0x00131FB8
	public override void UpdateFriend()
	{
		if (!base.gameObject.activeSelf || this.m_player == null)
		{
			return;
		}
		base.UpdateFriend();
		this.m_PlayerIcon.UpdateIcon();
		if (this.m_player.IsOnline())
		{
			this.m_PlayerNameText.Text = FriendUtils.GetFriendListName(this.m_player, true);
			base.UpdateOnlineStatus();
		}
		else
		{
			this.m_PlayerNameText.Text = FriendUtils.GetFriendListName(this.m_player, true);
			base.UpdateOfflineStatus();
		}
		this.m_recruitInfo = RecruitListMgr.Get().GetRecruitInfoFromAccountId(this.m_player.GetAccountId());
		this.m_RecruitUI.SetInfo(this.m_recruitInfo);
		if (this.m_recruitInfo != null)
		{
			this.m_RecruitUI.m_recruitText.TextColor = this.m_PlayerNameText.TextColor;
		}
		this.m_ChallengeButton.UpdateButton();
		this.UpdateLayout();
	}

	// Token: 0x06003F63 RID: 16227 RVA: 0x00133EA4 File Offset: 0x001320A4
	private void OnChallengeButtonReleased(UIEvent e)
	{
		if (!this.m_ChallengeButton.CanChallenge() || ChatMgr.Get().FriendListFrame.IsInEditMode)
		{
			FriendListFriendFrame.OnPlayerChallengeButtonPressed(this.m_ChallengeButton, this.m_player);
		}
	}

	// Token: 0x06003F64 RID: 16228 RVA: 0x00133EE8 File Offset: 0x001320E8
	public static void OnPlayerChallengeButtonPressed(FriendListChallengeButton challengeButton, BnetPlayer player)
	{
		SoundManager.Get().LoadAndPlay("Small_Click");
		if (ChatMgr.Get().FriendListFrame.IsInEditMode)
		{
			ChatMgr.Get().FriendListFrame.ShowRemoveFriendPopup(player);
			return;
		}
		BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
		SpectatorManager spectatorManager = SpectatorManager.Get();
		if (spectatorManager.CanSpectate(player))
		{
			spectatorManager.SpectatePlayer(player);
		}
		else if (spectatorManager.IsSpectatingMe(hearthstoneGameAccountId))
		{
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLOBAL_SPECTATOR_KICK_PROMPT_HEADER");
			popupInfo.m_text = GameStrings.Format("GLOBAL_SPECTATOR_KICK_PROMPT_TEXT", new object[]
			{
				FriendUtils.GetUniqueName(player)
			});
			popupInfo.m_showAlertIcon = true;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
			popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(FriendListFriendFrame.OnKickSpectatorDialogResponse);
			popupInfo.m_responseUserData = player;
			DialogManager.Get().ShowPopup(popupInfo);
		}
		else if (spectatorManager.CanInviteToSpectateMyGame(hearthstoneGameAccountId))
		{
			spectatorManager.InviteToSpectateMe(player);
		}
		else if (spectatorManager.IsSpectatingPlayer(hearthstoneGameAccountId))
		{
			if (GameMgr.Get().IsFindingGame() || SceneMgr.Get().IsTransitioning() || GameMgr.Get().IsTransitionPopupShown())
			{
				return;
			}
			AlertPopup.PopupInfo popupInfo2 = new AlertPopup.PopupInfo();
			popupInfo2.m_headerText = GameStrings.Get("GLOBAL_SPECTATOR_LEAVE_PROMPT_HEADER");
			popupInfo2.m_text = GameStrings.Get("GLOBAL_SPECTATOR_LEAVE_PROMPT_TEXT");
			popupInfo2.m_showAlertIcon = true;
			popupInfo2.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
			popupInfo2.m_responseCallback = new AlertPopup.ResponseCallback(FriendListFriendFrame.OnLeaveSpectatingDialogResponse);
			DialogManager.Get().ShowPopup(popupInfo2);
		}
		else
		{
			if (spectatorManager.IsInvitedToSpectateMyGame(hearthstoneGameAccountId))
			{
				return;
			}
			return;
		}
		challengeButton.UpdateButton();
		ChatMgr.Get().CloseChatUI();
	}

	// Token: 0x06003F65 RID: 16229 RVA: 0x0013408C File Offset: 0x0013228C
	private static void OnLeaveSpectatingDialogResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CONFIRM)
		{
			SpectatorManager.Get().LeaveSpectatorMode();
		}
	}

	// Token: 0x06003F66 RID: 16230 RVA: 0x001340A0 File Offset: 0x001322A0
	private static void OnKickSpectatorDialogResponse(AlertPopup.Response response, object userData)
	{
		BnetPlayer player = (BnetPlayer)userData;
		if (response == AlertPopup.Response.CONFIRM)
		{
			SpectatorManager.Get().KickSpectator(player, true);
		}
	}

	// Token: 0x06003F67 RID: 16231 RVA: 0x001340C8 File Offset: 0x001322C8
	private void UpdateLayout()
	{
		Component component = this.m_Bones.m_RightComponent;
		for (int i = this.m_rightComponentOrder.Length - 1; i >= 0; i--)
		{
			Component component2 = this.m_rightComponentOrder[i];
			if (component2.gameObject.activeSelf)
			{
				TransformUtil.SetPoint(component2, Anchor.RIGHT, component, Anchor.LEFT, this.m_Offsets.m_RightComponent);
				component = component2;
			}
		}
		Vector3 vector;
		vector..ctor(0f, 0f, 0f);
		vector += this.AddWidth(this.m_PlayerIcon);
		vector += this.AddWidth(this.m_rankMedal);
		this.LayoutLeftText(this.m_PlayerNameText, this.m_Bones.m_PlayerNameText, this.m_Offsets.m_PlayerNameText + vector, component);
		this.LayoutLeftText(this.m_StatusText, this.m_Bones.m_StatusText, this.m_Offsets.m_StatusText + vector, component);
		this.m_rankMedal.transform.position = this.m_Bones.m_Medal.transform.position;
	}

	// Token: 0x06003F68 RID: 16232 RVA: 0x001341E4 File Offset: 0x001323E4
	private Vector3 AddWidth(Component component)
	{
		Vector3 result = default(Vector3);
		if (component.gameObject.activeInHierarchy)
		{
			Bounds bounds = TransformUtil.ComputeSetPointBounds(component, true);
			result.x += bounds.max.x - bounds.min.x;
		}
		return result;
	}

	// Token: 0x06003F69 RID: 16233 RVA: 0x00134240 File Offset: 0x00132440
	private void LayoutLeftText(UberText text, Transform bone, Vector3 offset, Component rightComponent)
	{
		if (!text.gameObject.activeInHierarchy)
		{
			return;
		}
		text.Width = this.ComputeLeftComponentWidth(bone, offset, rightComponent);
		TransformUtil.SetPoint(text, Anchor.LEFT, bone, Anchor.RIGHT, offset);
	}

	// Token: 0x06003F6A RID: 16234 RVA: 0x00134278 File Offset: 0x00132478
	private float ComputeLeftComponentWidth(Transform bone, Vector3 offset, Component rightComponent)
	{
		Vector3 vector = bone.position + offset;
		Bounds bounds = TransformUtil.ComputeSetPointBounds(rightComponent);
		float num = bounds.center.x - bounds.extents.x + this.m_Offsets.m_RightComponent.x;
		return num - vector.x;
	}

	// Token: 0x040028A2 RID: 10402
	private const float REFRESH_FRIENDS_SECONDS = 30f;

	// Token: 0x040028A3 RID: 10403
	public FriendListChallengeButton m_ChallengeButton;

	// Token: 0x040028A4 RID: 10404
	public FriendListFriendFrameBones m_Bones;

	// Token: 0x040028A5 RID: 10405
	public FriendListFriendFrameOffsets m_Offsets;

	// Token: 0x040028A6 RID: 10406
	private Component[] m_rightComponentOrder;
}
