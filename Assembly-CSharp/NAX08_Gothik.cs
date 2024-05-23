using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F2 RID: 754
public class NAX08_Gothik : NAX_MissionEntity
{
	// Token: 0x06002825 RID: 10277 RVA: 0x000C384C File Offset: 0x000C1A4C
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX8_01_CARD_02");
		base.PreloadSound("VO_NAX8_01_CUSTOM_03");
		base.PreloadSound("VO_NAX8_01_EMOTE1_06");
		base.PreloadSound("VO_NAX8_01_EMOTE2_07");
		base.PreloadSound("VO_NAX8_01_EMOTE3_08");
		base.PreloadSound("VO_NAX8_01_EMOTE4_09");
		base.PreloadSound("VO_NAX8_01_EMOTE5_10");
		base.PreloadSound("VO_NAX8_01_CUSTOM2_04");
	}

	// Token: 0x06002826 RID: 10278 RVA: 0x000C38B4 File Offset: 0x000C1AB4
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
			m_soundName = "VO_NAX8_01_EMOTE1_06",
			m_stringTag = "VO_NAX8_01_EMOTE1_06"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX8_01_EMOTE2_07",
			m_stringTag = "VO_NAX8_01_EMOTE2_07"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX8_01_EMOTE3_08",
			m_stringTag = "VO_NAX8_01_EMOTE3_08"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX8_01_EMOTE4_09",
			m_stringTag = "VO_NAX8_01_EMOTE4_09"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX8_01_EMOTE5_10",
			m_stringTag = "VO_NAX8_01_EMOTE5_10"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002827 RID: 10279 RVA: 0x000C39A8 File Offset: 0x000C1BA8
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_GOTHIK2_62", "VO_KT_GOTHIK2_62", true);
		}
		yield break;
	}

	// Token: 0x06002828 RID: 10280 RVA: 0x000C39CC File Offset: 0x000C1BCC
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
			if (NAX08_Gothik.<>f__switch$map6E == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("NAX8_03", 0);
				dictionary.Add("NAX8_04", 0);
				dictionary.Add("NAX8_05", 0);
				dictionary.Add("NAX8_02", 1);
				NAX08_Gothik.<>f__switch$map6E = dictionary;
			}
			int num;
			if (NAX08_Gothik.<>f__switch$map6E.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX8_01_CUSTOM_03", "VO_NAX8_01_CUSTOM_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_unrelentingMinionLinePlayed)
					{
						yield break;
					}
					this.m_unrelentingMinionLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX8_01_CARD_02", "VO_NAX8_01_CARD_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002829 RID: 10281 RVA: 0x000C39F8 File Offset: 0x000C1BF8
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		yield return Gameplay.Get().StartCoroutine(base.HandleMissionEventWithTiming(missionEvent));
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		if (missionEvent == 1)
		{
			if (this.m_deadReturnLinePlayed)
			{
				yield break;
			}
			this.m_deadReturnLinePlayed = true;
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX8_01_CUSTOM2_04", "VO_NAX8_01_CUSTOM2_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x0400178A RID: 6026
	private bool m_cardLinePlayed;

	// Token: 0x0400178B RID: 6027
	private bool m_unrelentingMinionLinePlayed;

	// Token: 0x0400178C RID: 6028
	private bool m_deadReturnLinePlayed;
}
