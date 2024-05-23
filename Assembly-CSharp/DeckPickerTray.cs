using System;

// Token: 0x02000235 RID: 565
public class DeckPickerTray
{
	// Token: 0x0600214F RID: 8527 RVA: 0x000A32F4 File Offset: 0x000A14F4
	public static DeckPickerTray Get()
	{
		if (DeckPickerTray.s_instance == null)
		{
			DeckPickerTray.s_instance = new DeckPickerTray();
		}
		return DeckPickerTray.s_instance;
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x000A330F File Offset: 0x000A150F
	public void RegisterHandlers()
	{
		if (this.m_registeredHandlers)
		{
			return;
		}
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		this.m_registeredHandlers = true;
	}

	// Token: 0x06002151 RID: 8529 RVA: 0x000A333C File Offset: 0x000A153C
	public void UnregisterHandlers()
	{
		if (!this.m_registeredHandlers)
		{
			return;
		}
		GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		this.m_registeredHandlers = false;
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x000A3373 File Offset: 0x000A1573
	public void Unload()
	{
		this.UnregisterHandlers();
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x000A337C File Offset: 0x000A157C
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		switch (eventData.m_state)
		{
		case FindGameState.CLIENT_CANCELED:
			DeckPickerTrayDisplay.Get().HandleGameStartupFailure();
			break;
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_QUEUE_CANCELED:
		case FindGameState.BNET_ERROR:
			DeckPickerTrayDisplay.Get().HandleGameStartupFailure();
			break;
		case FindGameState.SERVER_GAME_STARTED:
			DeckPickerTrayDisplay.Get().OnServerGameStarted();
			break;
		case FindGameState.SERVER_GAME_CANCELED:
			DeckPickerTrayDisplay.Get().OnServerGameCanceled();
			break;
		}
		return false;
	}

	// Token: 0x0400129F RID: 4767
	private static DeckPickerTray s_instance;

	// Token: 0x040012A0 RID: 4768
	private bool m_registeredHandlers;
}
