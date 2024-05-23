using System;

// Token: 0x020003A0 RID: 928
public class FriendlyChallengeHelper
{
	// Token: 0x060030CF RID: 12495 RVA: 0x000F5EB4 File Offset: 0x000F40B4
	public static FriendlyChallengeHelper Get()
	{
		if (FriendlyChallengeHelper.s_instance == null)
		{
			FriendlyChallengeHelper.s_instance = new FriendlyChallengeHelper();
		}
		return FriendlyChallengeHelper.s_instance;
	}

	// Token: 0x060030D0 RID: 12496 RVA: 0x000F5ED0 File Offset: 0x000F40D0
	public void StartChallengeOrWaitForOpponent(string waitingDialogText, AlertPopup.ResponseCallback waitingCallback)
	{
		if (FriendChallengeMgr.Get().DidOpponentSelectDeck())
		{
			this.WaitForFriendChallengeToStart();
		}
		else
		{
			this.ShowFriendChallengeWaitingForOpponentDialog(waitingDialogText, waitingCallback);
		}
	}

	// Token: 0x060030D1 RID: 12497 RVA: 0x000F5F00 File Offset: 0x000F4100
	public void HideFriendChallengeWaitingForOpponentDialog()
	{
		if (this.m_friendChallengeWaitingPopup == null)
		{
			return;
		}
		this.m_friendChallengeWaitingPopup.Hide();
		this.m_friendChallengeWaitingPopup = null;
	}

	// Token: 0x060030D2 RID: 12498 RVA: 0x000F5F31 File Offset: 0x000F4131
	public void WaitForFriendChallengeToStart()
	{
		GameMgr.Get().WaitForFriendChallengeToStart(FriendChallengeMgr.Get().GetScenarioId());
	}

	// Token: 0x060030D3 RID: 12499 RVA: 0x000F5F47 File Offset: 0x000F4147
	public void StopWaitingForFriendChallenge()
	{
		this.HideFriendChallengeWaitingForOpponentDialog();
	}

	// Token: 0x060030D4 RID: 12500 RVA: 0x000F5F50 File Offset: 0x000F4150
	private void ShowFriendChallengeWaitingForOpponentDialog(string dialogText, AlertPopup.ResponseCallback callback)
	{
		BnetPlayer myOpponent = FriendChallengeMgr.Get().GetMyOpponent();
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_text = GameStrings.Format(dialogText, new object[]
		{
			FriendUtils.GetUniqueName(myOpponent)
		});
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL;
		popupInfo.m_responseCallback = callback;
		DialogManager.Get().ShowPopup(popupInfo, new DialogManager.DialogProcessCallback(this.OnFriendChallengeWaitingForOpponentDialogProcessed));
	}

	// Token: 0x060030D5 RID: 12501 RVA: 0x000F5FB8 File Offset: 0x000F41B8
	private bool OnFriendChallengeWaitingForOpponentDialogProcessed(DialogBase dialog, object userData)
	{
		if (!FriendChallengeMgr.Get().HasChallenge())
		{
			return false;
		}
		if (FriendChallengeMgr.Get().DidOpponentSelectDeck())
		{
			this.WaitForFriendChallengeToStart();
			return false;
		}
		this.m_friendChallengeWaitingPopup = (AlertPopup)dialog;
		return true;
	}

	// Token: 0x04001E79 RID: 7801
	private static FriendlyChallengeHelper s_instance;

	// Token: 0x04001E7A RID: 7802
	private AlertPopup m_friendChallengeWaitingPopup;
}
