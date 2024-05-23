using System;

// Token: 0x02000F6C RID: 3948
public class LoginPointer : PegUIElement
{
	// Token: 0x06007525 RID: 29989 RVA: 0x0022956A File Offset: 0x0022776A
	protected override void OnPress()
	{
		GameUtils.LogoutConfirmation();
	}
}
