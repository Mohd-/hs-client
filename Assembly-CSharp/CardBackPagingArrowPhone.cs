using System;

// Token: 0x02000A83 RID: 2691
public class CardBackPagingArrowPhone : CardBackPagingArrowBase
{
	// Token: 0x06005D98 RID: 23960 RVA: 0x001C1059 File Offset: 0x001BF259
	private void Start()
	{
		this.button.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnButtonReleased));
	}

	// Token: 0x06005D99 RID: 23961 RVA: 0x001C1074 File Offset: 0x001BF274
	private void OnButtonReleased(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("deck_select_button_press");
	}

	// Token: 0x06005D9A RID: 23962 RVA: 0x001C1085 File Offset: 0x001BF285
	public override void EnablePaging(bool enable)
	{
		this.button.gameObject.SetActive(enable);
	}

	// Token: 0x06005D9B RID: 23963 RVA: 0x001C1098 File Offset: 0x001BF298
	public override void AddEventListener(UIEventType eventType, UIEvent.Handler handler)
	{
		this.button.AddEventListener(eventType, handler);
	}

	// Token: 0x0400455F RID: 17759
	public PegUIElement button;
}
