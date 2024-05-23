using System;

// Token: 0x02000E8E RID: 3726
public class RainOfFire : SuperSpell
{
	// Token: 0x060070AA RID: 28842 RVA: 0x00212DEE File Offset: 0x00210FEE
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
	}

	// Token: 0x060070AB RID: 28843 RVA: 0x00212E1C File Offset: 0x0021101C
	protected override void UpdateVisualTargets()
	{
		int num = this.NumberOfCardsInOpponentsHand();
		this.m_TargetInfo.m_RandomTargetCountMin = num;
		this.m_TargetInfo.m_RandomTargetCountMax = num;
		ZonePlay zonePlay = SpellUtils.FindOpponentPlayZone(this);
		base.GenerateRandomPlayZoneVisualTargets(zonePlay);
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			if (i < this.m_visualTargets.Count)
			{
				this.m_visualTargets[i] = this.m_targets[i];
			}
			else
			{
				this.AddVisualTarget(this.m_targets[i]);
			}
		}
	}

	// Token: 0x060070AC RID: 28844 RVA: 0x00212EB4 File Offset: 0x002110B4
	private int NumberOfCardsInOpponentsHand()
	{
		Player firstOpponentPlayer = GameState.Get().GetFirstOpponentPlayer(base.GetSourceCard().GetController());
		ZoneHand handZone = firstOpponentPlayer.GetHandZone();
		return handZone.GetCardCount();
	}
}
