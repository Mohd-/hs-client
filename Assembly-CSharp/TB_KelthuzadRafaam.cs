using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000320 RID: 800
public class TB_KelthuzadRafaam : MissionEntity
{
	// Token: 0x06002951 RID: 10577 RVA: 0x000C84EC File Offset: 0x000C66EC
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
		foreach (Player player in playerMap.Values)
		{
			Entity heroEntity = player.GetHero();
			Card heroCard = heroEntity.GetCard();
			if (heroEntity.GetCardId() == "TB_KTRAF_H_1")
			{
				this.m_kelthuzadCard = heroCard;
			}
		}
		switch (missionEvent)
		{
		case 1:
		{
			GameState.Get().SetBusy(true);
			CardSoundSpell spellPray = this.m_kelthuzadCard.PlayEmote(EmoteType.EVENT_WINTER_VEIL);
			float clipLengthPray = spellPray.m_CardSoundData.m_AudioSource.clip.length;
			yield return new WaitForSeconds((float)((double)clipLengthPray * 0.8));
			GameState.Get().SetBusy(false);
			break;
		}
		case 2:
		{
			GameState.Get().SetBusy(true);
			CardSoundSpell spellBoard = this.m_kelthuzadCard.PlayEmote(EmoteType.EVENT_LUNAR_NEW_YEAR);
			float clipLengthBoard = spellBoard.m_CardSoundData.m_AudioSource.clip.length;
			yield return new WaitForSeconds((float)((double)clipLengthBoard * 0.8));
			GameState.Get().SetBusy(false);
			break;
		}
		case 3:
		{
			GameState.Get().SetBusy(true);
			CardSoundSpell spellServants = this.m_kelthuzadCard.PlayEmote(EmoteType.MIRROR_START);
			float clipLengthServants = spellServants.m_CardSoundData.m_AudioSource.clip.length;
			yield return new WaitForSeconds((float)((double)clipLengthServants * 0.8));
			GameState.Get().SetBusy(false);
			break;
		}
		}
		yield break;
	}

	// Token: 0x04001832 RID: 6194
	private Card m_kelthuzadCard;
}
