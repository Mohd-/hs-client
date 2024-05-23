using System;

// Token: 0x02000A80 RID: 2688
public class CardBackPagingArrow : CardBackPagingArrowBase
{
	// Token: 0x06005D89 RID: 23945 RVA: 0x001C0D86 File Offset: 0x001BEF86
	public override void EnablePaging(bool enable)
	{
		this.button.Activate(enable);
	}

	// Token: 0x06005D8A RID: 23946 RVA: 0x001C0D94 File Offset: 0x001BEF94
	public override void AddEventListener(UIEventType eventType, UIEvent.Handler handler)
	{
		this.button.AddEventListener(eventType, handler);
	}

	// Token: 0x0400455B RID: 17755
	public ArrowModeButton button;
}
