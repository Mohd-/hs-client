using System;
using PegasusShared;
using UnityEngine;

// Token: 0x0200047E RID: 1150
public class FriendlyChallengeDialog : DialogBase
{
	// Token: 0x060037E4 RID: 14308 RVA: 0x0011295C File Offset: 0x00110B5C
	private void Start()
	{
		this.m_acceptButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ConfirmButtonPress));
		this.m_denyButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CancelButtonPress));
	}

	// Token: 0x060037E5 RID: 14309 RVA: 0x0011299C File Offset: 0x00110B9C
	public override void Show()
	{
		base.Show();
		if (UniversalInputManager.UsePhoneUI && this.m_nearbyPlayerNote.gameObject.activeSelf)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y + 50f, base.transform.localPosition.z);
		}
		base.DoShowAnimation();
		UniversalInputManager.Get().SetSystemDialogActive(true);
		SoundManager.Get().LoadAndPlay("friendly_challenge");
	}

	// Token: 0x060037E6 RID: 14310 RVA: 0x00112A42 File Offset: 0x00110C42
	public override void Hide()
	{
		base.Hide();
		SoundManager.Get().LoadAndPlay("banner_shrink");
	}

	// Token: 0x060037E7 RID: 14311 RVA: 0x00112A59 File Offset: 0x00110C59
	public override bool HandleKeyboardInput()
	{
		if (Input.GetKeyUp(27))
		{
			this.CancelButtonPress(null);
			return true;
		}
		return false;
	}

	// Token: 0x060037E8 RID: 14312 RVA: 0x00112A74 File Offset: 0x00110C74
	public void SetInfo(FriendlyChallengeDialog.Info info)
	{
		string key = "GLOBAL_FRIEND_CHALLENGE_BODY1";
		if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
		{
			key = "GLOBAL_FRIEND_CHALLENGE_TAVERN_BRAWL_BODY1";
		}
		else if (CollectionManager.Get().ShouldAccountSeeStandardWild())
		{
			if (info.m_formatType == 2)
			{
				key = "GLOBAL_FRIEND_CHALLENGE_BODY1_STANDARD";
			}
			else if (info.m_formatType == 1)
			{
				key = "GLOBAL_FRIEND_CHALLENGE_BODY1_WILD";
			}
		}
		this.m_challengeText1.Text = GameStrings.Get(key);
		this.m_challengeText2.Text = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_BODY2");
		this.m_challengerName.Text = FriendUtils.GetUniqueName(info.m_challenger);
		this.m_responseCallback = info.m_callback;
		bool active = BnetNearbyPlayerMgr.Get().IsNearbyStranger(info.m_challenger);
		this.m_nearbyPlayerNote.gameObject.SetActive(active);
	}

	// Token: 0x060037E9 RID: 14313 RVA: 0x00112B44 File Offset: 0x00110D44
	private void ConfirmButtonPress(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("Small_Click");
		if (this.m_responseCallback != null)
		{
			this.m_responseCallback(true);
		}
		this.Hide();
	}

	// Token: 0x060037EA RID: 14314 RVA: 0x00112B80 File Offset: 0x00110D80
	private void CancelButtonPress(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("Small_Click");
		if (this.m_responseCallback != null)
		{
			this.m_responseCallback(false);
		}
		this.Hide();
	}

	// Token: 0x040023D9 RID: 9177
	public UberText m_challengeText1;

	// Token: 0x040023DA RID: 9178
	public UberText m_challengeText2;

	// Token: 0x040023DB RID: 9179
	public UberText m_challengerName;

	// Token: 0x040023DC RID: 9180
	public UIBButton m_acceptButton;

	// Token: 0x040023DD RID: 9181
	public UIBButton m_denyButton;

	// Token: 0x040023DE RID: 9182
	public UberText m_nearbyPlayerNote;

	// Token: 0x040023DF RID: 9183
	private FriendlyChallengeDialog.ResponseCallback m_responseCallback;

	// Token: 0x0200047F RID: 1151
	// (Invoke) Token: 0x060037EC RID: 14316
	public delegate void ResponseCallback(bool accept);

	// Token: 0x02000480 RID: 1152
	public class Info
	{
		// Token: 0x040023E0 RID: 9184
		public FormatType m_formatType;

		// Token: 0x040023E1 RID: 9185
		public BnetPlayer m_challenger;

		// Token: 0x040023E2 RID: 9186
		public FriendlyChallengeDialog.ResponseCallback m_callback;
	}
}
