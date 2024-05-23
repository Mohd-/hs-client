using System;
using bgs;
using UnityEngine;

// Token: 0x02000638 RID: 1592
public class FriendListChallengeButton : FriendListUIElement
{
	// Token: 0x0600451E RID: 17694 RVA: 0x0014BDAC File Offset: 0x00149FAC
	public bool CanChallenge()
	{
		return FriendChallengeMgr.Get().CanChallenge(this.m_player);
	}

	// Token: 0x0600451F RID: 17695 RVA: 0x0014BDBE File Offset: 0x00149FBE
	public BnetPlayer GetPlayer()
	{
		return this.m_player;
	}

	// Token: 0x06004520 RID: 17696 RVA: 0x0014BDC6 File Offset: 0x00149FC6
	public bool SetPlayer(BnetPlayer player)
	{
		if (this.m_player == player)
		{
			return false;
		}
		this.m_player = player;
		this.UpdateButton();
		return true;
	}

	// Token: 0x06004521 RID: 17697 RVA: 0x0014BDE4 File Offset: 0x00149FE4
	public void UpdateButton()
	{
		if (this.UpdateEditModeButtonState())
		{
			this.CloseChallengeMenu();
			return;
		}
		if (this.m_player == null || !this.m_player.IsOnline() || this.m_player.GetBestProgramId() != BnetProgramId.HEARTHSTONE)
		{
			base.gameObject.SetActive(false);
			this.CloseChallengeMenu();
			return;
		}
		base.gameObject.SetActive(true);
		if (this.UpdateSpectateButtonState())
		{
			this.CloseChallengeMenu();
			return;
		}
		bool active = false;
		bool active2 = false;
		if (this.CanChallenge())
		{
			active = true;
		}
		else
		{
			this.CloseChallengeMenu();
			active2 = true;
		}
		this.m_AvailableIcon.SetActive(active);
		this.m_BusyIcon.SetActive(active2);
		this.UpdateTooltip();
	}

	// Token: 0x06004522 RID: 17698 RVA: 0x0014BEA8 File Offset: 0x0014A0A8
	protected override bool ShouldBeHighlighted()
	{
		return this.IsChallengeMenuOpen() || (this.CanChallenge() && base.ShouldBeHighlighted());
	}

	// Token: 0x06004523 RID: 17699 RVA: 0x0014BED5 File Offset: 0x0014A0D5
	protected override void OnPress()
	{
		base.OnPress();
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.ShowTooltip();
		}
	}

	// Token: 0x06004524 RID: 17700 RVA: 0x0014BEF4 File Offset: 0x0014A0F4
	protected override void OnRelease()
	{
		base.OnRelease();
		if (this.CanChallenge() && !ChatMgr.Get().FriendListFrame.IsInEditMode)
		{
			if (this.IsChallengeMenuOpen())
			{
				this.CloseChallengeMenu();
			}
			else
			{
				this.HideTooltip();
				this.OpenChallengeMenu();
			}
		}
	}

	// Token: 0x06004525 RID: 17701 RVA: 0x0014BF48 File Offset: 0x0014A148
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		base.OnOver(oldState);
		this.ShowTooltip();
	}

	// Token: 0x06004526 RID: 17702 RVA: 0x0014BF57 File Offset: 0x0014A157
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		base.OnOut(oldState);
		this.HideTooltip();
	}

	// Token: 0x06004527 RID: 17703 RVA: 0x0014BF68 File Offset: 0x0014A168
	private bool UpdateSpectateButtonState()
	{
		BnetGameAccountId hearthstoneGameAccountId = this.m_player.GetHearthstoneGameAccountId();
		SpectatorManager spectatorManager = SpectatorManager.Get();
		bool active = false;
		string text = null;
		if (spectatorManager.HasInvitedMeToSpectate(hearthstoneGameAccountId))
		{
			text = "HasInvitedMeToSpectate";
			active = true;
		}
		else if (spectatorManager.CanSpectate(this.m_player))
		{
			text = "CanSpectateThisFriend";
		}
		else if (spectatorManager.IsSpectatingMe(hearthstoneGameAccountId))
		{
			text = "CurrentlySpectatingMe";
		}
		else if (spectatorManager.CanInviteToSpectateMyGame(hearthstoneGameAccountId))
		{
			text = "CanInviteToSpectateMe";
		}
		else if (spectatorManager.IsSpectatingPlayer(hearthstoneGameAccountId))
		{
			text = "CurrentlySpectatingThisFriend";
		}
		else if (spectatorManager.IsInvitedToSpectateMyGame(hearthstoneGameAccountId))
		{
			text = "DisabledInviteToSpectateMe";
		}
		if (this.m_SpectatorIcon != null)
		{
			this.m_SpectatorIcon.gameObject.SetActive(text != null);
			if (text != null)
			{
				this.m_SpectatorIcon.CurrentState = text;
				this.m_AvailableIcon.SetActive(false);
				this.m_BusyIcon.SetActive(false);
			}
			if (this.m_SpectatorIconHighlight != null)
			{
				this.m_SpectatorIconHighlight.gameObject.SetActive(active);
			}
		}
		return text != null;
	}

	// Token: 0x06004528 RID: 17704 RVA: 0x0014C094 File Offset: 0x0014A294
	private bool UpdateEditModeButtonState()
	{
		if (this.m_player == null)
		{
			base.gameObject.SetActive(false);
			return true;
		}
		if (ChatMgr.Get() != null && ChatMgr.Get().FriendListFrame != null && ChatMgr.Get().FriendListFrame.IsInEditMode)
		{
			base.gameObject.SetActive(true);
			this.m_AvailableIcon.SetActive(false);
			this.m_BusyIcon.SetActive(false);
			this.m_SpectatorIcon.gameObject.SetActive(false);
			if (this.m_DeleteIcon != null)
			{
				this.m_DeleteIcon.SetActive(true);
			}
			return true;
		}
		if (this.m_DeleteIcon != null)
		{
			this.m_DeleteIcon.SetActive(false);
		}
		return false;
	}

	// Token: 0x06004529 RID: 17705 RVA: 0x0014C168 File Offset: 0x0014A368
	private void ShowTooltip()
	{
		if (this.IsChallengeMenuOpen())
		{
			return;
		}
		BnetGameAccountId hearthstoneGameAccountId = this.m_player.GetHearthstoneGameAccountId();
		SpectatorManager spectatorManager = SpectatorManager.Get();
		string text;
		string text2;
		if (spectatorManager.HasInvitedMeToSpectate(hearthstoneGameAccountId))
		{
			text = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_AVAILABLE_HEADER";
			text2 = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_RECEIVED_INVITE_TEXT";
		}
		else if (spectatorManager.CanSpectate(this.m_player))
		{
			text = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_AVAILABLE_HEADER";
			text2 = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_AVAILABLE_TEXT";
		}
		else if (spectatorManager.IsSpectatingMe(hearthstoneGameAccountId))
		{
			text = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_KICK_HEADER";
			text2 = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_KICK_TEXT";
		}
		else if (spectatorManager.CanInviteToSpectateMyGame(hearthstoneGameAccountId))
		{
			if (spectatorManager.IsPlayerSpectatingMyGamesOpposingSide(hearthstoneGameAccountId))
			{
				text = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITE_OTHER_SIDE_HEADER";
				text2 = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITE_OTHER_SIDE_TEXT";
			}
			else
			{
				text = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITE_HEADER";
				text2 = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITE_TEXT";
			}
		}
		else if (spectatorManager.IsInvitedToSpectateMyGame(hearthstoneGameAccountId))
		{
			text = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITED_HEADER";
			text2 = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITED_TEXT";
		}
		else if (spectatorManager.IsSpectatingPlayer(hearthstoneGameAccountId))
		{
			text = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_SPECTATING_HEADER";
			text2 = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_SPECTATING_TEXT";
		}
		else if (spectatorManager.HasPreviouslyKickedMeFromGame(hearthstoneGameAccountId))
		{
			text = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_PREVIOUSLY_KICKED_HEADER";
			text2 = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_PREVIOUSLY_KICKED_TEXT";
		}
		else
		{
			bool flag = TavernBrawlManager.Get().ShouldNewFriendlyChallengeBeTavernBrawl();
			if (flag)
			{
				text = "GLOBAL_FRIENDLIST_TAVERN_BRAWL_CHALLENGE_BUTTON_HEADER";
			}
			else
			{
				text = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
			}
			if (!FriendChallengeMgr.Get().AmIAvailable())
			{
				text2 = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_IM_UNAVAILABLE";
			}
			else if (!FriendChallengeMgr.Get().CanChallenge(this.m_player))
			{
				text2 = null;
				TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
				if (flag && tavernBrawlMission.canCreateDeck && !TavernBrawlManager.Get().HasValidDeck())
				{
					text2 = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_TAVERN_BRAWL_MUST_CREATE_DECK";
				}
				if (text2 == null)
				{
					text2 = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_THEYRE_UNAVAILABLE";
				}
			}
			else if (flag)
			{
				text2 = "GLOBAL_FRIENDLIST_TAVERN_BRAWL_CHALLENGE_BUTTON_AVAILABLE";
			}
			else
			{
				text2 = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_AVAILABLE";
			}
		}
		if (UniversalInputManager.Get().IsTouchMode())
		{
			if (GameStrings.HasKey(text + "_TOUCH"))
			{
				text += "_TOUCH";
			}
			if (GameStrings.HasKey(text2 + "_TOUCH"))
			{
				text2 += "_TOUCH";
			}
		}
		string headline = GameStrings.Get(text);
		string bodytext = GameStrings.Format(text2, new object[]
		{
			this.m_player.GetBestName()
		});
		this.m_TooltipZone.ShowSocialTooltip(this, headline, bodytext, 75f, GameLayer.BattleNetDialog);
	}

	// Token: 0x0600452A RID: 17706 RVA: 0x0014C3BE File Offset: 0x0014A5BE
	private void UpdateTooltip()
	{
		if (!this.m_TooltipZone.IsShowingTooltip())
		{
			return;
		}
		this.HideTooltip();
		this.ShowTooltip();
	}

	// Token: 0x0600452B RID: 17707 RVA: 0x0014C3DD File Offset: 0x0014A5DD
	private void HideTooltip()
	{
		this.m_TooltipZone.HideTooltip();
	}

	// Token: 0x0600452C RID: 17708 RVA: 0x0014C3EA File Offset: 0x0014A5EA
	private bool IsChallengeMenuOpen()
	{
		return this.m_isChallengeMenuOpen;
	}

	// Token: 0x0600452D RID: 17709 RVA: 0x0014C3F4 File Offset: 0x0014A5F4
	private void OpenChallengeMenu()
	{
		if (this.m_ChallengeMenu == null || this.m_isChallengeMenuOpen)
		{
			return;
		}
		this.m_isChallengeMenuOpen = true;
		this.m_ChallengeMenu.gameObject.SetActive(true);
		if (this.m_challengeMenuOrigLocalPos != null)
		{
			this.m_ChallengeMenu.gameObject.transform.localPosition = this.m_challengeMenuOrigLocalPos.Value;
		}
		Bounds bounds = this.m_ChallengeMenu.PrefabGameObject().GetComponent<Collider>().bounds;
		Camera camera = CameraUtils.FindFirstByLayer(this.m_ChallengeMenu.PrefabGameObject().layer);
		Vector3 vector = camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.center.z));
		if (vector.y < 0f)
		{
			if (this.m_challengeMenuOrigLocalPos == null)
			{
				this.m_challengeMenuOrigLocalPos = new Vector3?(this.m_ChallengeMenu.gameObject.transform.localPosition);
			}
			Vector3 vector2 = camera.WorldToScreenPoint(this.m_ChallengeMenu.gameObject.transform.position);
			this.m_ChallengeMenu.gameObject.transform.position = camera.ScreenToWorldPoint(new Vector3(vector2.x, vector2.y - vector.y, vector2.z));
		}
		this.InitChallengeMenuInputBlocker();
		base.UpdateHighlight();
	}

	// Token: 0x0600452E RID: 17710 RVA: 0x0014C574 File Offset: 0x0014A774
	private void CloseChallengeMenu()
	{
		if (this.m_ChallengeMenu == null || !this.m_isChallengeMenuOpen)
		{
			return;
		}
		this.m_isChallengeMenuOpen = false;
		this.m_ChallengeMenu.gameObject.SetActive(false);
		if (this.m_challengeMenuInputBlocker != null)
		{
			Object.Destroy(this.m_challengeMenuInputBlocker.gameObject);
			this.m_challengeMenuInputBlocker = null;
		}
		base.UpdateHighlight();
	}

	// Token: 0x0600452F RID: 17711 RVA: 0x0014C5E4 File Offset: 0x0014A7E4
	private void InitChallengeMenuInputBlocker()
	{
		if (this.m_challengeMenuInputBlocker != null)
		{
			Object.Destroy(this.m_challengeMenuInputBlocker.gameObject);
			this.m_challengeMenuInputBlocker = null;
		}
		Camera camera = CameraUtils.FindFirstByLayer(this.m_ChallengeMenu.PrefabGameObject().layer);
		GameObject gameObject = CameraUtils.CreateInputBlocker(camera, "ChallengeMenuInputBlocker");
		gameObject.transform.parent = this.m_ChallengeMenu.PrefabGameObject().transform;
		this.m_challengeMenuInputBlocker = gameObject.AddComponent<PegUIElement>();
		this.m_challengeMenuInputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChallengeMenuInputBlockerReleased));
		TransformUtil.SetPosZ(this.m_challengeMenuInputBlocker, this.m_ChallengeMenu.PrefabGameObject().transform.position.z + 1f);
	}

	// Token: 0x06004530 RID: 17712 RVA: 0x0014C6A9 File Offset: 0x0014A8A9
	private void OnChallengeMenuInputBlockerReleased(UIEvent e)
	{
		this.CloseChallengeMenu();
	}

	// Token: 0x04002BFA RID: 11258
	public GameObject m_AvailableIcon;

	// Token: 0x04002BFB RID: 11259
	public GameObject m_BusyIcon;

	// Token: 0x04002BFC RID: 11260
	public TooltipZone m_TooltipZone;

	// Token: 0x04002BFD RID: 11261
	public TextureOffsetStates m_SpectatorIcon;

	// Token: 0x04002BFE RID: 11262
	public GameObject m_SpectatorIconHighlight;

	// Token: 0x04002BFF RID: 11263
	public GameObject m_DeleteIcon;

	// Token: 0x04002C00 RID: 11264
	public NestedPrefab m_ChallengeMenu;

	// Token: 0x04002C01 RID: 11265
	private BnetPlayer m_player;

	// Token: 0x04002C02 RID: 11266
	private PegUIElement m_challengeMenuInputBlocker;

	// Token: 0x04002C03 RID: 11267
	private bool m_isChallengeMenuOpen;

	// Token: 0x04002C04 RID: 11268
	private Vector3? m_challengeMenuOrigLocalPos;
}
