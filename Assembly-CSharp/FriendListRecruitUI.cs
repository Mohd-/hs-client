using System;
using UnityEngine;

// Token: 0x02000632 RID: 1586
public class FriendListRecruitUI : MonoBehaviour
{
	// Token: 0x0600450E RID: 17678 RVA: 0x0014BAD8 File Offset: 0x00149CD8
	public void Awake()
	{
		this.m_CancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelButtonPressed));
	}

	// Token: 0x0600450F RID: 17679 RVA: 0x0014BAF3 File Offset: 0x00149CF3
	public void SetInfo(Network.RecruitInfo info)
	{
		this.m_recruitInfo = info;
		this.Update();
	}

	// Token: 0x06004510 RID: 17680 RVA: 0x0014BB04 File Offset: 0x00149D04
	private void Update()
	{
		if (this.m_recruitInfo != null)
		{
			base.gameObject.SetActive(true);
			this.m_CancelButton.gameObject.SetActive(false);
			this.m_success.SetActive(false);
			switch (this.m_recruitInfo.Status)
			{
			case 2:
				this.m_CancelButton.gameObject.SetActive(true);
				break;
			case 3:
				this.m_CancelButton.gameObject.SetActive(true);
				break;
			case 4:
				this.m_success.SetActive(true);
				this.m_recruitText.Text = string.Format("Level\n{0}/50", this.m_recruitInfo.Level);
				break;
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004511 RID: 17681 RVA: 0x0014BBE4 File Offset: 0x00149DE4
	private void OnCancelButtonPressed(UIEvent e)
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CANCEL_RECRUIT_ALERT_MESSAGE", new object[]
		{
			this.m_recruitInfo.Nickname
		});
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnCancelPopupResponse);
		DialogManager.Get().ShowPopup(popupInfo, new DialogManager.DialogProcessCallback(this.OnCancelShown));
	}

	// Token: 0x06004512 RID: 17682 RVA: 0x0014BC52 File Offset: 0x00149E52
	private bool OnCancelShown(DialogBase dialog, object userData)
	{
		return true;
	}

	// Token: 0x06004513 RID: 17683 RVA: 0x0014BC55 File Offset: 0x00149E55
	private void OnCancelPopupResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CONFIRM)
		{
			RecruitListMgr.Get().RecruitFriendCancel(this.m_recruitInfo.ID);
		}
	}

	// Token: 0x04002BEA RID: 11242
	public UberText m_recruitText;

	// Token: 0x04002BEB RID: 11243
	public GameObject m_success;

	// Token: 0x04002BEC RID: 11244
	public FriendListUIElement m_CancelButton;

	// Token: 0x04002BED RID: 11245
	private Network.RecruitInfo m_recruitInfo;
}
