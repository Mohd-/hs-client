using System;

// Token: 0x020001B6 RID: 438
public class UIReleaseAllEvent : UIEvent
{
	// Token: 0x06001CF1 RID: 7409 RVA: 0x00088055 File Offset: 0x00086255
	public UIReleaseAllEvent(bool mouseIsOver, PegUIElement element) : base(UIEventType.RELEASEALL, element)
	{
		this.m_mouseIsOver = mouseIsOver;
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x00088066 File Offset: 0x00086266
	public bool GetMouseIsOver()
	{
		return this.m_mouseIsOver;
	}

	// Token: 0x04000F23 RID: 3875
	private bool m_mouseIsOver;
}
