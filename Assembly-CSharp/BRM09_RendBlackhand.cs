using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000302 RID: 770
public class BRM09_RendBlackhand : BRM_MissionEntity
{
	// Token: 0x06002889 RID: 10377 RVA: 0x000C5524 File Offset: 0x000C3724
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA09_1_RESPONSE_04");
		base.PreloadSound("VO_BRMA09_1_HERO_POWER1_06");
		base.PreloadSound("VO_BRMA09_1_HERO_POWER2_07");
		base.PreloadSound("VO_BRMA09_1_HERO_POWER3_08");
		base.PreloadSound("VO_BRMA09_1_HERO_POWER4_09");
		base.PreloadSound("VO_BRMA09_1_CARD_05");
		base.PreloadSound("VO_BRMA09_1_TURN1_03");
		base.PreloadSound("VO_NEFARIAN_REND1_52");
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x000C558C File Offset: 0x000C378C
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
			m_soundName = "VO_BRMA09_1_RESPONSE_04",
			m_stringTag = "VO_BRMA09_1_RESPONSE_04"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x000C55F4 File Offset: 0x000C37F4
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
			if (BRM09_RendBlackhand.<>f__switch$map56 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(9);
				dictionary.Add("BRMA09_2", 0);
				dictionary.Add("BRMA09_2H", 0);
				dictionary.Add("BRMA09_3", 1);
				dictionary.Add("BRMA09_3H", 1);
				dictionary.Add("BRMA09_4", 2);
				dictionary.Add("BRMA09_4H", 2);
				dictionary.Add("BRMA09_5", 3);
				dictionary.Add("BRMA09_5H", 3);
				dictionary.Add("BRMA09_6", 4);
				BRM09_RendBlackhand.<>f__switch$map56 = dictionary;
			}
			int num;
			if (BRM09_RendBlackhand.<>f__switch$map56.TryGetValue(cardId, ref num))
			{
				switch (num)
				{
				case 0:
					if (this.m_heroPower1LinePlayed)
					{
						yield break;
					}
					this.m_heroPower1LinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA09_1_HERO_POWER1_06", "VO_BRMA09_1_HERO_POWER1_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 1:
					if (this.m_heroPower2LinePlayed)
					{
						yield break;
					}
					this.m_heroPower2LinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA09_1_HERO_POWER2_07", "VO_BRMA09_1_HERO_POWER2_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 2:
					if (this.m_heroPower3LinePlayed)
					{
						yield break;
					}
					this.m_heroPower3LinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA09_1_HERO_POWER3_08", "VO_BRMA09_1_HERO_POWER3_08", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 3:
					if (this.m_heroPower4LinePlayed)
					{
						yield break;
					}
					this.m_heroPower4LinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA09_1_HERO_POWER4_09", "VO_BRMA09_1_HERO_POWER4_09", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 4:
					if (this.m_cardLinePlayed)
					{
						yield break;
					}
					this.m_cardLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA09_1_CARD_05", "VO_BRMA09_1_CARD_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				}
			}
		}
		yield break;
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x000C5620 File Offset: 0x000C3820
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Vector3 quotePos = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn == 1)
		{
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA09_1_TURN1_03", "VO_BRMA09_1_TURN1_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			if (!GameMgr.Get().IsClassChallengeMission())
			{
				NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_REND1_52"), "VO_NEFARIAN_REND1_52", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
			}
		}
		yield break;
	}

	// Token: 0x0600288D RID: 10381 RVA: 0x000C564C File Offset: 0x000C384C
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_REND_DEAD_53"), "VO_NEFARIAN_REND_DEAD_53", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017BE RID: 6078
	private bool m_heroPower1LinePlayed;

	// Token: 0x040017BF RID: 6079
	private bool m_heroPower2LinePlayed;

	// Token: 0x040017C0 RID: 6080
	private bool m_heroPower3LinePlayed;

	// Token: 0x040017C1 RID: 6081
	private bool m_heroPower4LinePlayed;

	// Token: 0x040017C2 RID: 6082
	private bool m_cardLinePlayed;
}
