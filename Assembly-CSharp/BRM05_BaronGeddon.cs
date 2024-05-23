using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002FE RID: 766
public class BRM05_BaronGeddon : BRM_MissionEntity
{
	// Token: 0x06002870 RID: 10352 RVA: 0x000C4F88 File Offset: 0x000C3188
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA05_1_RESPONSE_03");
		base.PreloadSound("VO_BRMA05_1_HERO_POWER_06");
		base.PreloadSound("VO_BRMA05_1_CARD_05");
		base.PreloadSound("VO_BRMA05_1_TURN1_02");
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x000C4FC4 File Offset: 0x000C31C4
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
			m_soundName = "VO_BRMA05_1_RESPONSE_03",
			m_stringTag = "VO_BRMA05_1_RESPONSE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x000C502C File Offset: 0x000C322C
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
			if (BRM05_BaronGeddon.<>f__switch$map53 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("BRMA05_2", 0);
				dictionary.Add("BRMA05_2H", 0);
				dictionary.Add("BRMA05_3", 1);
				dictionary.Add("BRMA05_3H", 1);
				BRM05_BaronGeddon.<>f__switch$map53 = dictionary;
			}
			int num;
			if (BRM05_BaronGeddon.<>f__switch$map53.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA05_1_CARD_05", "VO_BRMA05_1_CARD_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA05_1_HERO_POWER_06", "VO_BRMA05_1_HERO_POWER_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002873 RID: 10355 RVA: 0x000C5058 File Offset: 0x000C3258
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn == 1)
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA05_1_TURN1_02", "VO_BRMA05_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x06002874 RID: 10356 RVA: 0x000C5084 File Offset: 0x000C3284
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_BARON_GEDDON_DEAD_40"), "VO_NEFARIAN_BARON_GEDDON_DEAD_40", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017B6 RID: 6070
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017B7 RID: 6071
	private bool m_cardLinePlayed;
}
