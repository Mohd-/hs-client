using System;
using UnityEngine;

// Token: 0x0200051B RID: 1307
public class DesktopChatLogUI : IChatLogUI
{
	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x06003CA7 RID: 15527 RVA: 0x00125A94 File Offset: 0x00123C94
	public bool IsShowing
	{
		get
		{
			return this.m_quickChatFrame != null;
		}
	}

	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x06003CA8 RID: 15528 RVA: 0x00125AA4 File Offset: 0x00123CA4
	public GameObject GameObject
	{
		get
		{
			return (!(this.m_quickChatFrame == null)) ? this.m_quickChatFrame.gameObject : null;
		}
	}

	// Token: 0x1700044C RID: 1100
	// (get) Token: 0x06003CA9 RID: 15529 RVA: 0x00125AD4 File Offset: 0x00123CD4
	public BnetPlayer Receiver
	{
		get
		{
			return (!(this.m_quickChatFrame == null)) ? this.m_quickChatFrame.GetReceiver() : null;
		}
	}

	// Token: 0x06003CAA RID: 15530 RVA: 0x00125B04 File Offset: 0x00123D04
	public void ShowForPlayer(BnetPlayer player)
	{
		if (this.m_quickChatFrame != null)
		{
			return;
		}
		GameObject gameObject = AssetLoader.Get().LoadGameObject("QuickChatFrame", true, false);
		if (gameObject != null)
		{
			this.m_quickChatFrame = gameObject.GetComponent<QuickChatFrame>();
			this.m_quickChatFrame.SetReceiver(player);
		}
	}

	// Token: 0x06003CAB RID: 15531 RVA: 0x00125B59 File Offset: 0x00123D59
	public void Hide()
	{
		if (this.m_quickChatFrame == null)
		{
			return;
		}
		Object.Destroy(this.m_quickChatFrame.gameObject);
		this.m_quickChatFrame = null;
	}

	// Token: 0x06003CAC RID: 15532 RVA: 0x00125B84 File Offset: 0x00123D84
	public void GoBack()
	{
	}

	// Token: 0x040026A4 RID: 9892
	private QuickChatFrame m_quickChatFrame;
}
