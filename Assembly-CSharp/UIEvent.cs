using System;

// Token: 0x02000175 RID: 373
public class UIEvent
{
	// Token: 0x060014B4 RID: 5300 RVA: 0x0005B97B File Offset: 0x00059B7B
	public UIEvent(UIEventType eventType, PegUIElement element)
	{
		this.m_eventType = eventType;
		this.m_element = element;
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x0005B991 File Offset: 0x00059B91
	public UIEventType GetEventType()
	{
		return this.m_eventType;
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x0005B999 File Offset: 0x00059B99
	public PegUIElement GetElement()
	{
		return this.m_element;
	}

	// Token: 0x04000A69 RID: 2665
	private UIEventType m_eventType;

	// Token: 0x04000A6A RID: 2666
	private PegUIElement m_element;

	// Token: 0x02000176 RID: 374
	// (Invoke) Token: 0x060014B8 RID: 5304
	public delegate void Handler(UIEvent e);
}
