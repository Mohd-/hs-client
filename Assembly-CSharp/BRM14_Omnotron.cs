using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000308 RID: 776
public class BRM14_Omnotron : BRM_MissionEntity
{
	// Token: 0x060028AF RID: 10415 RVA: 0x000C5DE0 File Offset: 0x000C3FE0
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA14_1_RESPONSE1_10");
		base.PreloadSound("VO_BRMA14_1_RESPONSE2_11");
		base.PreloadSound("VO_BRMA14_1_RESPONSE3_12");
		base.PreloadSound("VO_BRMA14_1_RESPONSE4_13");
		base.PreloadSound("VO_BRMA14_1_RESPONSE5_14");
		base.PreloadSound("VO_BRMA14_1_HP1_03");
		base.PreloadSound("VO_BRMA14_1_HP2_04");
		base.PreloadSound("VO_BRMA14_1_HP3_05");
		base.PreloadSound("VO_BRMA14_1_HP4_06");
		base.PreloadSound("VO_BRMA14_1_HP5_07");
		base.PreloadSound("VO_BRMA14_1_CARD_09");
		base.PreloadSound("VO_BRMA14_1_TURN1_02");
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x000C5E74 File Offset: 0x000C4074
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
			m_soundName = "VO_BRMA14_1_RESPONSE1_10",
			m_stringTag = "VO_BRMA14_1_RESPONSE1_10"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_BRMA14_1_RESPONSE2_11",
			m_stringTag = "VO_BRMA14_1_RESPONSE2_11"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_BRMA14_1_RESPONSE3_12",
			m_stringTag = "VO_BRMA14_1_RESPONSE3_12"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_BRMA14_1_RESPONSE4_13",
			m_stringTag = "VO_BRMA14_1_RESPONSE4_13"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_BRMA14_1_RESPONSE5_14",
			m_stringTag = "VO_BRMA14_1_RESPONSE5_14"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x000C5F67 File Offset: 0x000C4167
	protected override void CycleNextResponseGroupIndex(MissionEntity.EmoteResponseGroup responseGroup)
	{
		if (responseGroup.m_responseIndex == responseGroup.m_responses.Count - 1)
		{
			return;
		}
		responseGroup.m_responseIndex++;
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x000C5F90 File Offset: 0x000C4190
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
			if (BRM14_Omnotron.<>f__switch$map5B == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
				dictionary.Add("BRMA14_6", 0);
				dictionary.Add("BRMA14_6H", 0);
				dictionary.Add("BRMA14_4", 1);
				dictionary.Add("BRMA14_4H", 1);
				dictionary.Add("BRMA14_2", 2);
				dictionary.Add("BRMA14_2H", 2);
				dictionary.Add("BRMA14_8", 3);
				dictionary.Add("BRMA14_8H", 3);
				dictionary.Add("BRMA14_10", 4);
				dictionary.Add("BRMA14_10H", 4);
				dictionary.Add("BRMA14_11", 5);
				BRM14_Omnotron.<>f__switch$map5B = dictionary;
			}
			int num;
			if (BRM14_Omnotron.<>f__switch$map5B.TryGetValue(cardId, ref num))
			{
				switch (num)
				{
				case 0:
					if (this.m_heroPower1LinePlayed)
					{
						yield break;
					}
					this.m_heroPower1LinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA14_1_HP1_03", "VO_BRMA14_1_HP1_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 1:
					if (this.m_heroPower2LinePlayed)
					{
						yield break;
					}
					this.m_heroPower2LinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA14_1_HP2_04", "VO_BRMA14_1_HP2_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 2:
					if (this.m_heroPower3LinePlayed)
					{
						yield break;
					}
					this.m_heroPower3LinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA14_1_HP3_05", "VO_BRMA14_1_HP3_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 3:
					if (this.m_heroPower4LinePlayed)
					{
						yield break;
					}
					this.m_heroPower4LinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA14_1_HP4_06", "VO_BRMA14_1_HP4_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 4:
					if (this.m_heroPower5LinePlayed)
					{
						yield break;
					}
					this.m_heroPower5LinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA14_1_HP5_07", "VO_BRMA14_1_HP5_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 5:
					if (this.m_cardLinePlayed)
					{
						yield break;
					}
					this.m_cardLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA14_1_CARD_09", "VO_BRMA14_1_CARD_09", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				}
			}
		}
		yield break;
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x000C5FBC File Offset: 0x000C41BC
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn == 1)
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA14_1_TURN1_02", "VO_BRMA14_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x060028B4 RID: 10420 RVA: 0x000C5FE8 File Offset: 0x000C41E8
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NefarianDragon_Quote", GameStrings.Get("VO_NEFARIAN_OMNOTRON_DEAD_69"), "VO_NEFARIAN_OMNOTRON_DEAD_69", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017D3 RID: 6099
	private bool m_heroPower1LinePlayed;

	// Token: 0x040017D4 RID: 6100
	private bool m_heroPower2LinePlayed;

	// Token: 0x040017D5 RID: 6101
	private bool m_heroPower3LinePlayed;

	// Token: 0x040017D6 RID: 6102
	private bool m_heroPower4LinePlayed;

	// Token: 0x040017D7 RID: 6103
	private bool m_heroPower5LinePlayed;

	// Token: 0x040017D8 RID: 6104
	private bool m_cardLinePlayed;
}
