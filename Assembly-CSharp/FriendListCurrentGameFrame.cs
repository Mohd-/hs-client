using System;
using bgs;
using UnityEngine;

// Token: 0x02000566 RID: 1382
public class FriendListCurrentGameFrame : FriendListBaseFriendFrame
{
	// Token: 0x06003F5A RID: 16218 RVA: 0x00133C4B File Offset: 0x00131E4B
	protected override void Awake()
	{
		base.Awake();
		this.m_PlayButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonPressed));
	}

	// Token: 0x06003F5B RID: 16219 RVA: 0x00133C6C File Offset: 0x00131E6C
	private void OnEnable()
	{
		this.UpdateFriend();
	}

	// Token: 0x06003F5C RID: 16220 RVA: 0x00133C74 File Offset: 0x00131E74
	public override void UpdateFriend()
	{
		if (!base.gameObject.activeSelf || this.m_player == null)
		{
			return;
		}
		base.UpdateFriend();
		this.m_PlayerIcon.m_OnlinePortrait.SetProgramId(BnetProgramId.HEARTHSTONE);
		this.m_PlayerNameText.Text = FriendUtils.GetFriendListName(this.m_player, true);
		base.UpdateOnlineStatus();
	}

	// Token: 0x06003F5D RID: 16221 RVA: 0x00133CD6 File Offset: 0x00131ED6
	private void OnPlayButtonPressed(UIEvent e)
	{
	}

	// Token: 0x040028A0 RID: 10400
	public GameObject m_Background;

	// Token: 0x040028A1 RID: 10401
	public FriendListButton m_PlayButton;
}
