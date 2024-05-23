using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000300 RID: 768
public class BRM07_Omokk : BRM_MissionEntity
{
	// Token: 0x0600287D RID: 10365 RVA: 0x000C52B8 File Offset: 0x000C34B8
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA07_1_RESPONSE_03");
		base.PreloadSound("VO_BRMA07_1_HERO_POWER_05");
		base.PreloadSound("VO_BRMA07_1_CARD_04");
		base.PreloadSound("VO_BRMA07_1_TURN1_02");
		base.PreloadSound("VO_NEFARIAN_OMOKK1_44");
		base.PreloadSound("VO_NEFARIAN_OMOKK2_45");
	}

	// Token: 0x0600287E RID: 10366 RVA: 0x000C5308 File Offset: 0x000C3508
	protected override void InitEmoteResponses()
	{
		List<MissionEntity.EmoteResponseGroup> list = new List<MissionEntity.EmoteResponseGroup>();
		List<MissionEntity.EmoteResponseGroup> list2 = list;
		MissionEntity.EmoteResponseGroup emoteResponseGroup = new MissionEntity.EmoteResponseGroup();
		emoteResponseGroup.m_triggers = new List<EmoteType>(MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS);
		MissionEntity.EmoteResponseGroup emoteResponseGroup2 = emoteResponseGroup;
		List<MissionEntity.EmoteResponse> list3 = new List<MissionEntity.EmoteResponse>();
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_BRMA07_1_RESPONSE_03",
			m_stringTag = "VO_BRMA07_1_RESPONSE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600287F RID: 10367 RVA: 0x000C5370 File Offset: 0x000C3570
	protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
		{
			yield return null;
		}
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		string cardId = entity.GetCardId();
		if (cardId != null)
		{
			if (BRM07_Omokk.<>f__switch$map54 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("BRMA07_2", 0);
				dictionary.Add("BRMA07_2H", 0);
				dictionary.Add("BRMA07_3", 1);
				BRM07_Omokk.<>f__switch$map54 = dictionary;
			}
			int num;
			if (BRM07_Omokk.<>f__switch$map54.TryGetValue(cardId, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						if (this.m_cardLinePlayed)
						{
							yield break;
						}
						this.m_cardLinePlayed = true;
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA07_1_CARD_04", "VO_BRMA07_1_CARD_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA07_1_HERO_POWER_05", "VO_BRMA07_1_HERO_POWER_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x000C539C File Offset: 0x000C359C
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Vector3 quotePos = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (turn)
		{
		case 1:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA07_1_TURN1_02", "VO_BRMA07_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			break;
		default:
			if (turn == 8)
			{
				NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_OMOKK2_45"), "VO_NEFARIAN_OMOKK2_45", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
			}
			break;
		case 4:
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_OMOKK1_44"), "VO_NEFARIAN_OMOKK1_44", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
			break;
		}
		yield break;
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x000C53C8 File Offset: 0x000C35C8
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_OMOKK_DEAD_46"), "VO_NEFARIAN_OMOKK_DEAD_46", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017B9 RID: 6073
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017BA RID: 6074
	private bool m_cardLinePlayed;
}
