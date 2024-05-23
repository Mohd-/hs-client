using System;

// Token: 0x020007CE RID: 1998
public class OffClickCatcher : PegUIElement
{
	// Token: 0x06004E17 RID: 19991 RVA: 0x0017398F File Offset: 0x00171B8F
	protected override void OnRelease()
	{
		Navigation.GoBack();
	}

	// Token: 0x06004E18 RID: 19992 RVA: 0x00173997 File Offset: 0x00171B97
	protected override void OnRightClick()
	{
		Navigation.GoBack();
	}
}
