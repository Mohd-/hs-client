using System;
using UnityEngine;

// Token: 0x0200051A RID: 1306
public class MobileChatLogUI : IChatLogUI
{
	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x06003CA0 RID: 15520 RVA: 0x00125982 File Offset: 0x00123B82
	public bool IsShowing
	{
		get
		{
			return this.m_chatFrames != null;
		}
	}

	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x06003CA1 RID: 15521 RVA: 0x00125990 File Offset: 0x00123B90
	public GameObject GameObject
	{
		get
		{
			return (!(this.m_chatFrames == null)) ? this.m_chatFrames.gameObject : null;
		}
	}

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x06003CA2 RID: 15522 RVA: 0x001259C0 File Offset: 0x00123BC0
	public BnetPlayer Receiver
	{
		get
		{
			return (!(this.m_chatFrames == null)) ? this.m_chatFrames.Receiver : null;
		}
	}

	// Token: 0x06003CA3 RID: 15523 RVA: 0x001259F0 File Offset: 0x00123BF0
	public void ShowForPlayer(BnetPlayer player)
	{
		string name = (!UniversalInputManager.UsePhoneUI) ? "MobileChatFrames" : "MobileChatFrames_phone";
		GameObject gameObject = AssetLoader.Get().LoadGameObject(name, true, false);
		if (gameObject != null)
		{
			this.m_chatFrames = gameObject.GetComponent<ChatFrames>();
			this.m_chatFrames.Receiver = player;
		}
	}

	// Token: 0x06003CA4 RID: 15524 RVA: 0x00125A4E File Offset: 0x00123C4E
	public void Hide()
	{
		if (!this.IsShowing)
		{
			return;
		}
		Object.Destroy(this.m_chatFrames.gameObject);
		this.m_chatFrames = null;
	}

	// Token: 0x06003CA5 RID: 15525 RVA: 0x00125A73 File Offset: 0x00123C73
	public void GoBack()
	{
		if (!this.IsShowing)
		{
			return;
		}
		this.m_chatFrames.Back();
	}

	// Token: 0x040026A3 RID: 9891
	private ChatFrames m_chatFrames;
}
