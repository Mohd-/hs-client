using System;

// Token: 0x02000321 RID: 801
public class StandardGameEntity : GameEntity
{
	// Token: 0x06002953 RID: 10579 RVA: 0x000C8520 File Offset: 0x000C6720
	public override void OnTagChanged(TagDelta change)
	{
		GAME_TAG tag = (GAME_TAG)change.tag;
		if (tag != GAME_TAG.STEP)
		{
			if (tag == GAME_TAG.NEXT_STEP)
			{
				if (change.newValue == 6)
				{
					if (GameState.Get().IsMulliganManagerActive())
					{
						GameState.Get().SetMulliganBusy(true);
					}
				}
				else if (change.oldValue == 9 && change.newValue == 10 && GameState.Get().IsFriendlySidePlayerTurn())
				{
					TurnStartManager.Get().BeginPlayingTurnEvents();
				}
			}
		}
		else if (change.newValue == 4)
		{
			MulliganManager.Get().BeginMulligan();
		}
		base.OnTagChanged(change);
	}
}
