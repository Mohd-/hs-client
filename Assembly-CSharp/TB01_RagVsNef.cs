using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200030B RID: 779
public class TB01_RagVsNef : MissionEntity
{
	// Token: 0x060028C4 RID: 10436 RVA: 0x000C641C File Offset: 0x000C461C
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		if (missionEvent == 1)
		{
			Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
			foreach (Player player in playerMap.Values)
			{
				Entity heroEntity = player.GetHero();
				Card heroCard = heroEntity.GetCard();
				if (heroEntity.GetCardId() == "TBA01_1")
				{
					this.m_ragnarosCard = heroCard;
				}
			}
			GameState.Get().SetBusy(true);
			CardSoundSpell spell = this.m_ragnarosCard.PlayEmote(EmoteType.THREATEN);
			float clipLength = spell.m_CardSoundData.m_AudioSource.clip.length;
			yield return new WaitForSeconds((float)((double)clipLength * 0.8));
			GameState.Get().SetBusy(false);
		}
		yield break;
	}

	// Token: 0x040017E4 RID: 6116
	private Card m_ragnarosCard;
}
