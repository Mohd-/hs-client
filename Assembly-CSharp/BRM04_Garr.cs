using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002FD RID: 765
public class BRM04_Garr : BRM_MissionEntity
{
	// Token: 0x0600286A RID: 10346 RVA: 0x000C4E40 File Offset: 0x000C3040
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA04_1_RESPONSE_03");
		base.PreloadSound("VO_BRMA04_1_HERO_POWER_05");
		base.PreloadSound("VO_BRMA04_1_CARD_04");
		base.PreloadSound("VO_BRMA04_1_TURN1_02");
		base.PreloadSound("VO_NEFARIAN_GARR2_35");
		base.PreloadSound("VO_NEFARIAN_GARR3_36");
		base.PreloadSound("VO_NEFARIAN_GARR4_37");
	}

	// Token: 0x0600286B RID: 10347 RVA: 0x000C4E9C File Offset: 0x000C309C
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
			m_soundName = "VO_BRMA04_1_RESPONSE_03",
			m_stringTag = "VO_BRMA04_1_RESPONSE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600286C RID: 10348 RVA: 0x000C4F04 File Offset: 0x000C3104
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
			if (BRM04_Garr.<>f__switch$map52 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("BRMA04_2", 0);
				dictionary.Add("BRMA04_4", 1);
				dictionary.Add("BRMA04_4H", 1);
				BRM04_Garr.<>f__switch$map52 = dictionary;
			}
			int num;
			if (BRM04_Garr.<>f__switch$map52.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA04_1_CARD_04", "VO_BRMA04_1_CARD_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA04_1_HERO_POWER_05", "VO_BRMA04_1_HERO_POWER_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x0600286D RID: 10349 RVA: 0x000C4F30 File Offset: 0x000C3130
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Vector3 quotePos = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (turn)
		{
		case 1:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA04_1_TURN1_02", "VO_BRMA04_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			break;
		default:
			if (turn != 8)
			{
				if (turn == 12)
				{
					if (!GameMgr.Get().IsClassChallengeMission())
					{
						NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_GARR4_37"), "VO_NEFARIAN_GARR4_37", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
					}
				}
			}
			else if (!GameMgr.Get().IsClassChallengeMission())
			{
				NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_GARR3_36"), "VO_NEFARIAN_GARR3_36", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
			}
			break;
		case 4:
			if (!GameMgr.Get().IsClassChallengeMission())
			{
				NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_GARR2_35"), "VO_NEFARIAN_GARR2_35", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
			}
			break;
		}
		yield break;
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x000C4F5C File Offset: 0x000C315C
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_GARR_DEAD1_38"), "VO_NEFARIAN_GARR_DEAD1_38", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017B3 RID: 6067
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017B4 RID: 6068
	private bool m_cardLinePlayed;
}
