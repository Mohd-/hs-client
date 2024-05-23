using System;

// Token: 0x0200081C RID: 2076
public class DefeatScreen : EndGameScreen
{
	// Token: 0x06005009 RID: 20489 RVA: 0x0017BDAC File Offset: 0x00179FAC
	protected override void Awake()
	{
		base.Awake();
		if (base.ShouldMakeUtilRequests())
		{
			NetCache.Get().RegisterScreenEndOfGame(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		}
	}

	// Token: 0x0600500A RID: 20490 RVA: 0x0017BDE4 File Offset: 0x00179FE4
	protected override void ShowStandardFlow()
	{
		base.ShowStandardFlow();
		if (GameMgr.Get().IsTutorial() && !GameMgr.Get().IsSpectator())
		{
			this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(base.ContinueButtonPress_TutorialProgress));
		}
		else
		{
			this.m_hitbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(base.ContinueButtonPress_PrevMode));
		}
	}
}
