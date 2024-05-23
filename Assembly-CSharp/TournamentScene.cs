using System;

// Token: 0x02000B08 RID: 2824
public class TournamentScene : PlayGameScene
{
	// Token: 0x060060BB RID: 24763 RVA: 0x001CF426 File Offset: 0x001CD626
	protected override void Awake()
	{
		base.Awake();
		TournamentScene.s_instance = this;
	}

	// Token: 0x060060BC RID: 24764 RVA: 0x001CF434 File Offset: 0x001CD634
	private void OnDestroy()
	{
		TournamentScene.s_instance = null;
	}

	// Token: 0x060060BD RID: 24765 RVA: 0x001CF43C File Offset: 0x001CD63C
	public static TournamentScene Get()
	{
		return TournamentScene.s_instance;
	}

	// Token: 0x060060BE RID: 24766 RVA: 0x001CF443 File Offset: 0x001CD643
	public override string GetScreenName()
	{
		return "Tournament";
	}

	// Token: 0x060060BF RID: 24767 RVA: 0x001CF44A File Offset: 0x001CD64A
	public override void Unload()
	{
		base.Unload();
		TournamentDisplay.Get().Unload();
	}

	// Token: 0x04004860 RID: 18528
	private static TournamentScene s_instance;
}
