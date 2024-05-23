using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F0 RID: 752
public class NAX06_Loatheb : NAX_MissionEntity
{
	// Token: 0x0600281A RID: 10266 RVA: 0x000C365D File Offset: 0x000C185D
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX6_01_HP_02");
		base.PreloadSound("VO_NAX6_01_CARD_03");
		base.PreloadSound("VO_NAX6_01_EMOTE_05");
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x000C3680 File Offset: 0x000C1880
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
			m_soundName = "VO_NAX6_01_EMOTE_05",
			m_stringTag = "VO_NAX6_01_EMOTE_05"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600281C RID: 10268 RVA: 0x000C36E8 File Offset: 0x000C18E8
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_LOATHEB2_57", "VO_KT_LOATHEB2_57", true);
		}
		yield break;
	}

	// Token: 0x0600281D RID: 10269 RVA: 0x000C370C File Offset: 0x000C190C
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
			if (NAX06_Loatheb.<>f__switch$map6C == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("NAX6_02", 0);
				dictionary.Add("NAX6_03", 1);
				NAX06_Loatheb.<>f__switch$map6C = dictionary;
			}
			int num;
			if (NAX06_Loatheb.<>f__switch$map6C.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX6_01_CARD_03", "VO_NAX6_01_CARD_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX6_01_HP_02", "VO_NAX6_01_HP_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x04001785 RID: 6021
	private bool m_cardLinePlayed;

	// Token: 0x04001786 RID: 6022
	private bool m_heroPowerLinePlayed;
}
