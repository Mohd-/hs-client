using System;

// Token: 0x020008A5 RID: 2213
public class FriendlyScene : PlayGameScene
{
	// Token: 0x06005417 RID: 21527 RVA: 0x001928B2 File Offset: 0x00190AB2
	protected override void Awake()
	{
		base.Awake();
		FriendlyScene.s_instance = this;
	}

	// Token: 0x06005418 RID: 21528 RVA: 0x001928C0 File Offset: 0x00190AC0
	private void OnDestroy()
	{
		FriendlyScene.s_instance = null;
	}

	// Token: 0x06005419 RID: 21529 RVA: 0x001928C8 File Offset: 0x00190AC8
	public static FriendlyScene Get()
	{
		return FriendlyScene.s_instance;
	}

	// Token: 0x0600541A RID: 21530 RVA: 0x001928CF File Offset: 0x00190ACF
	public override string GetScreenName()
	{
		return "Friendly";
	}

	// Token: 0x0600541B RID: 21531 RVA: 0x001928D6 File Offset: 0x00190AD6
	public override void Unload()
	{
		base.Unload();
		FriendlyDisplay.Get().Unload();
	}

	// Token: 0x04003A41 RID: 14913
	private static FriendlyScene s_instance;
}
