using System;
using UnityEngine;

// Token: 0x020004DC RID: 1244
public class BnetBarFriendButton : FriendListUIElement
{
	// Token: 0x06003A8F RID: 14991 RVA: 0x0011AF4C File Offset: 0x0011914C
	protected override void Awake()
	{
		BnetBarFriendButton.s_instance = this;
		base.Awake();
		this.UpdateOnlineCount();
		BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
		BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		this.ShowPendingInvitesIcon(false);
	}

	// Token: 0x06003A90 RID: 14992 RVA: 0x0011AFA0 File Offset: 0x001191A0
	private void OnDestroy()
	{
		BnetFriendMgr.Get().RemoveChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		BnetBarFriendButton.s_instance = null;
	}

	// Token: 0x06003A91 RID: 14993 RVA: 0x0011AFD6 File Offset: 0x001191D6
	public static BnetBarFriendButton Get()
	{
		return BnetBarFriendButton.s_instance;
	}

	// Token: 0x06003A92 RID: 14994 RVA: 0x0011AFE0 File Offset: 0x001191E0
	public void HideTooltip()
	{
		TooltipZone component = base.GetComponent<TooltipZone>();
		if (component != null)
		{
			component.HideTooltip();
		}
	}

	// Token: 0x06003A93 RID: 14995 RVA: 0x0011B006 File Offset: 0x00119206
	private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
	{
		this.UpdateOnlineCount();
	}

	// Token: 0x06003A94 RID: 14996 RVA: 0x0011B00E File Offset: 0x0011920E
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		this.UpdateOnlineCount();
	}

	// Token: 0x06003A95 RID: 14997 RVA: 0x0011B018 File Offset: 0x00119218
	private void UpdateOnlineCount()
	{
		int activeOnlineFriendCount = BnetFriendMgr.Get().GetActiveOnlineFriendCount();
		if (activeOnlineFriendCount == 0)
		{
			this.m_OnlineCountText.TextColor = this.m_AllOfflineColor;
		}
		else
		{
			this.m_OnlineCountText.TextColor = this.m_AnyOnlineColor;
		}
		this.m_OnlineCountText.Text = activeOnlineFriendCount.ToString();
	}

	// Token: 0x06003A96 RID: 14998 RVA: 0x0011B070 File Offset: 0x00119270
	public void ShowPendingInvitesIcon(bool show)
	{
		if (this.m_PendingInvitesIcon != null)
		{
			this.m_PendingInvitesIcon.SetActive(show);
			this.m_OnlineCountText.gameObject.SetActive(!show);
		}
	}

	// Token: 0x06003A97 RID: 14999 RVA: 0x0011B0AE File Offset: 0x001192AE
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		SoundManager.Get().LoadAndPlay("Small_Mouseover");
		base.UpdateHighlight();
	}

	// Token: 0x06003A98 RID: 15000 RVA: 0x0011B0C5 File Offset: 0x001192C5
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		base.OnOut(oldState);
	}

	// Token: 0x04002551 RID: 9553
	public UberText m_OnlineCountText;

	// Token: 0x04002552 RID: 9554
	public Color m_AnyOnlineColor;

	// Token: 0x04002553 RID: 9555
	public Color m_AllOfflineColor;

	// Token: 0x04002554 RID: 9556
	public GameObject m_PendingInvitesIcon;

	// Token: 0x04002555 RID: 9557
	private static BnetBarFriendButton s_instance;
}
