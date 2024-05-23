using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002EC RID: 748
public class NAX02_Faerlina : NAX_MissionEntity
{
	// Token: 0x06002803 RID: 10243 RVA: 0x000C3160 File Offset: 0x000C1360
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX2_01_HP_04");
		base.PreloadSound("VO_NAX2_01_CARD_02");
		base.PreloadSound("VO_NAX2_01_EMOTE_06");
		base.PreloadSound("VO_NAX2_01_CUSTOM_03");
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x000C319C File Offset: 0x000C139C
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
			m_soundName = "VO_NAX2_01_EMOTE_06",
			m_stringTag = "VO_NAX2_01_EMOTE_06"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002805 RID: 10245 RVA: 0x000C3204 File Offset: 0x000C1404
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_FAERLINA2_45", "VO_KT_FAERLINA2_45", true);
		}
		yield break;
	}

	// Token: 0x06002806 RID: 10246 RVA: 0x000C3228 File Offset: 0x000C1428
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
			if (NAX02_Faerlina.<>f__switch$map68 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("NAX2_03", 0);
				dictionary.Add("NAX2_05", 1);
				dictionary.Add("NAX2_05H", 1);
				NAX02_Faerlina.<>f__switch$map68 = dictionary;
			}
			int num;
			if (NAX02_Faerlina.<>f__switch$map68.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX2_01_CARD_02", "VO_NAX2_01_CARD_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX2_01_HP_04", "VO_NAX2_01_HP_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002807 RID: 10247 RVA: 0x000C3254 File Offset: 0x000C1454
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		yield return Gameplay.Get().StartCoroutine(base.HandleMissionEventWithTiming(missionEvent));
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		if (missionEvent == 1)
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX2_01_CUSTOM_03", "VO_NAX2_01_CUSTOM_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x04001779 RID: 6009
	private bool m_cardLinePlayed;

	// Token: 0x0400177A RID: 6010
	private bool m_heroPowerLinePlayed;
}
