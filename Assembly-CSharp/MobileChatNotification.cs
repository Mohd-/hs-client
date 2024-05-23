using System;
using System.ComponentModel;
using UnityEngine;

// Token: 0x02000559 RID: 1369
public class MobileChatNotification : MonoBehaviour
{
	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06003EF8 RID: 16120 RVA: 0x00132821 File Offset: 0x00130A21
	// (remove) Token: 0x06003EF9 RID: 16121 RVA: 0x0013283A File Offset: 0x00130A3A
	public event MobileChatNotification.NotifiedEvent Notified;

	// Token: 0x06003EFA RID: 16122 RVA: 0x00132853 File Offset: 0x00130A53
	private void OnEnable()
	{
		this.state = MobileChatNotification.State.None;
	}

	// Token: 0x06003EFB RID: 16123 RVA: 0x0013285C File Offset: 0x00130A5C
	private void OnDestroy()
	{
		if (GameState.Get() != null && !SpectatorManager.Get().IsInSpectatorMode())
		{
			GameState.Get().UnregisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnChanged));
			GameState.Get().UnregisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
		}
	}

	// Token: 0x06003EFC RID: 16124 RVA: 0x001328B0 File Offset: 0x00130AB0
	private void Update()
	{
		if (GameState.Get() == null || SpectatorManager.Get().IsInSpectatorMode())
		{
			this.state = MobileChatNotification.State.None;
			return;
		}
		GameState.Get().RegisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnChanged));
		GameState.Get().RegisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
		if (GameState.Get().IsMulliganPhase())
		{
			if (this.state == MobileChatNotification.State.None)
			{
				this.state = MobileChatNotification.State.GameStarted;
				this.FireNotification();
			}
			return;
		}
		if (this.state == MobileChatNotification.State.GameStarted)
		{
			this.state = ((!GameState.Get().IsFriendlySidePlayerTurn()) ? MobileChatNotification.State.None : MobileChatNotification.State.YourTurn);
			this.FireNotification();
		}
	}

	// Token: 0x06003EFD RID: 16125 RVA: 0x00132964 File Offset: 0x00130B64
	private string GetStateText(MobileChatNotification.State state)
	{
		if (state == MobileChatNotification.State.None)
		{
			return string.Empty;
		}
		DescriptionAttribute descriptionAttribute = typeof(MobileChatNotification.State).GetField(state.ToString()).GetCustomAttributes(false)[0] as DescriptionAttribute;
		return GameStrings.Get(descriptionAttribute.Description);
	}

	// Token: 0x06003EFE RID: 16126 RVA: 0x001329B0 File Offset: 0x00130BB0
	private void OnTurnChanged(int oldTurn, int newTurn, object userData)
	{
		if (GameState.Get().IsFriendlySidePlayerTurn())
		{
			this.state = MobileChatNotification.State.YourTurn;
			this.FireNotification();
		}
	}

	// Token: 0x06003EFF RID: 16127 RVA: 0x001329D0 File Offset: 0x00130BD0
	private void OnTurnTimerUpdate(TurnTimerUpdate update, object userData)
	{
		if (update.GetSecondsRemaining() > 0f && GameState.Get().IsFriendlySidePlayerTurn())
		{
			this.state = MobileChatNotification.State.TurnCountdown;
			this.FireNotification();
		}
	}

	// Token: 0x06003F00 RID: 16128 RVA: 0x00132A09 File Offset: 0x00130C09
	private void FireNotification()
	{
		if (this.Notified != null && this.state != MobileChatNotification.State.None)
		{
			this.Notified(this.GetStateText(this.state));
		}
	}

	// Token: 0x04002851 RID: 10321
	private MobileChatNotification.State state;

	// Token: 0x0200058F RID: 1423
	// (Invoke) Token: 0x06004080 RID: 16512
	public delegate void NotifiedEvent(string text);

	// Token: 0x02000597 RID: 1431
	private enum State
	{
		// Token: 0x04002930 RID: 10544
		None,
		// Token: 0x04002931 RID: 10545
		[Description("GLOBAL_MOBILECHAT_NOTIFICATION_MULLIGAIN")]
		GameStarted,
		// Token: 0x04002932 RID: 10546
		[Description("GLOBAL_MOBILECHAT_NOTIFICATION_YOUR_TURN")]
		YourTurn,
		// Token: 0x04002933 RID: 10547
		[Description("GLOBAL_MOBILECHAT_NOTIFICATION_TURN_COUNTDOWN")]
		TurnCountdown
	}
}
