using System;
using UnityEngine;

// Token: 0x02000565 RID: 1381
public class FriendListRequestFrame : MonoBehaviour
{
	// Token: 0x06003F50 RID: 16208 RVA: 0x00133B14 File Offset: 0x00131D14
	private void Awake()
	{
		this.m_AcceptButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnAcceptButtonPressed));
		this.m_DeclineButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeclineButtonPressed));
	}

	// Token: 0x06003F51 RID: 16209 RVA: 0x00133B53 File Offset: 0x00131D53
	private void Update()
	{
		this.UpdateTimeText();
	}

	// Token: 0x06003F52 RID: 16210 RVA: 0x00133B5B File Offset: 0x00131D5B
	private void OnEnable()
	{
		this.UpdateInvite();
	}

	// Token: 0x06003F53 RID: 16211 RVA: 0x00133B63 File Offset: 0x00131D63
	public BnetInvitation GetInvite()
	{
		return this.m_invite;
	}

	// Token: 0x06003F54 RID: 16212 RVA: 0x00133B6B File Offset: 0x00131D6B
	public void SetInvite(BnetInvitation invite)
	{
		if (this.m_invite == invite)
		{
			return;
		}
		this.m_invite = invite;
		this.UpdateInvite();
	}

	// Token: 0x06003F55 RID: 16213 RVA: 0x00133B8C File Offset: 0x00131D8C
	public void UpdateInvite()
	{
		if (!base.gameObject.activeSelf || this.m_invite == null)
		{
			return;
		}
		this.m_PlayerNameText.Text = this.m_invite.GetInviterName();
		this.UpdateTimeText();
	}

	// Token: 0x06003F56 RID: 16214 RVA: 0x00133BD8 File Offset: 0x00131DD8
	private void UpdateTimeText()
	{
		string requestElapsedTimeString = FriendUtils.GetRequestElapsedTimeString(this.m_invite.GetCreationTimeMicrosec());
		this.m_TimeText.Text = GameStrings.Format("GLOBAL_FRIENDLIST_REQUEST_SENT_TIME", new object[]
		{
			requestElapsedTimeString
		});
	}

	// Token: 0x06003F57 RID: 16215 RVA: 0x00133C15 File Offset: 0x00131E15
	private void OnAcceptButtonPressed(UIEvent e)
	{
		BnetFriendMgr.Get().AcceptInvite(this.m_invite.GetId());
	}

	// Token: 0x06003F58 RID: 16216 RVA: 0x00133C2C File Offset: 0x00131E2C
	private void OnDeclineButtonPressed(UIEvent e)
	{
		BnetFriendMgr.Get().DeclineInvite(this.m_invite.GetId());
	}

	// Token: 0x0400289A RID: 10394
	public GameObject m_Background;

	// Token: 0x0400289B RID: 10395
	public FriendListUIElement m_AcceptButton;

	// Token: 0x0400289C RID: 10396
	public FriendListUIElement m_DeclineButton;

	// Token: 0x0400289D RID: 10397
	public UberText m_PlayerNameText;

	// Token: 0x0400289E RID: 10398
	public UberText m_TimeText;

	// Token: 0x0400289F RID: 10399
	private BnetInvitation m_invite;
}
