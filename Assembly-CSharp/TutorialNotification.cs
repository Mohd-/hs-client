using System;

// Token: 0x020003EC RID: 1004
public class TutorialNotification : Notification
{
	// Token: 0x06003411 RID: 13329 RVA: 0x00104334 File Offset: 0x00102534
	public void SetWantedText(string txt)
	{
		if (this.m_WantedText != null)
		{
			this.m_WantedText.Text = txt;
			this.m_WantedText.gameObject.SetActive(true);
		}
	}

	// Token: 0x0400202B RID: 8235
	public UIBButton m_ButtonStart;

	// Token: 0x0400202C RID: 8236
	public UberText m_WantedText;
}
