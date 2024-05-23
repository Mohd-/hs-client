using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002ED RID: 749
public class NAX03_Maexxna : NAX_MissionEntity
{
	// Token: 0x06002809 RID: 10249 RVA: 0x000C3288 File Offset: 0x000C1488
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX3_01_EMOTE_01");
		base.PreloadSound("VO_NAX3_01_EMOTE_02");
		base.PreloadSound("VO_NAX3_01_EMOTE_03");
		base.PreloadSound("VO_NAX3_01_EMOTE_04");
		base.PreloadSound("VO_NAX3_01_EMOTE_05");
		base.PreloadSound("VO_NAX3_01_CARD_01");
		base.PreloadSound("VO_NAX3_01_HP_01");
		base.PreloadSound("VO_KT_MAEXXNA2_47");
		base.PreloadSound("VO_KT_MAEXXNA6_51");
		base.PreloadSound("VO_KT_MAEXXNA3_48");
	}

	// Token: 0x0600280A RID: 10250 RVA: 0x000C3304 File Offset: 0x000C1504
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
			m_soundName = "VO_NAX3_01_EMOTE_01",
			m_stringTag = "VO_NAX3_01_EMOTE_01"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX3_01_EMOTE_02",
			m_stringTag = "VO_NAX3_01_EMOTE_02"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX3_01_EMOTE_03",
			m_stringTag = "VO_NAX3_01_EMOTE_03"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX3_01_EMOTE_04",
			m_stringTag = "VO_NAX3_01_EMOTE_04"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX3_01_EMOTE_05",
			m_stringTag = "VO_NAX3_01_EMOTE_05"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600280B RID: 10251 RVA: 0x000C33F8 File Offset: 0x000C15F8
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		if (turn == 1)
		{
			NotificationManager.Get().CreateKTQuote("VO_KT_MAEXXNA2_47", "VO_KT_MAEXXNA2_47", false);
		}
		yield break;
	}

	// Token: 0x0600280C RID: 10252 RVA: 0x000C341C File Offset: 0x000C161C
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_MAEXXNA4_49", "VO_KT_MAEXXNA4_49", true);
		}
		yield break;
	}

	// Token: 0x0600280D RID: 10253 RVA: 0x000C3440 File Offset: 0x000C1640
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
			if (NAX03_Maexxna.<>f__switch$map69 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("NAX3_02", 0);
				dictionary.Add("NAX3_03", 1);
				dictionary.Add("EX1_586", 2);
				NAX03_Maexxna.<>f__switch$map69 = dictionary;
			}
			int num;
			if (NAX03_Maexxna.<>f__switch$map69.TryGetValue(cardId, ref num))
			{
				switch (num)
				{
				case 0:
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX3_01_HP_01", "VO_NAX3_01_HP_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					while (this.m_enemySpeaking || NotificationManager.Get().IsQuotePlaying)
					{
						yield return 0;
					}
					NotificationManager.Get().CreateKTQuote("VO_KT_MAEXXNA6_51", "VO_KT_MAEXXNA6_51", false);
					break;
				case 1:
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX3_01_CARD_01", "VO_NAX3_01_CARD_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 2:
					if (this.m_seaGiantLinePlayed)
					{
						yield break;
					}
					this.m_seaGiantLinePlayed = true;
					yield return new WaitForSeconds(1f);
					while (NotificationManager.Get().IsQuotePlaying)
					{
						yield return 0;
					}
					NotificationManager.Get().CreateKTQuote("VO_KT_MAEXXNA3_48", "VO_KT_MAEXXNA3_48", false);
					break;
				}
			}
		}
		yield break;
	}

	// Token: 0x0400177C RID: 6012
	private bool m_heroPowerLinePlayed;

	// Token: 0x0400177D RID: 6013
	private bool m_seaGiantLinePlayed;
}
