using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002EE RID: 750
public class NAX04_Noth : NAX_MissionEntity
{
	// Token: 0x0600280F RID: 10255 RVA: 0x000C3471 File Offset: 0x000C1671
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX4_01_HP_02");
		base.PreloadSound("VO_NAX4_01_CARD_03");
		base.PreloadSound("VO_NAX4_01_EMOTE_06");
	}

	// Token: 0x06002810 RID: 10256 RVA: 0x000C3494 File Offset: 0x000C1694
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
			m_soundName = "VO_NAX4_01_EMOTE_06",
			m_stringTag = "VO_NAX4_01_EMOTE_06"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002811 RID: 10257 RVA: 0x000C34FC File Offset: 0x000C16FC
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_NOTH2_53", "VO_KT_NOTH2_53", true);
		}
		yield break;
	}

	// Token: 0x06002812 RID: 10258 RVA: 0x000C3520 File Offset: 0x000C1720
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		yield return Gameplay.Get().StartCoroutine(base.HandleMissionEventWithTiming(missionEvent));
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (missionEvent == 1)
		{
			if (this.m_heroPowerLinePlayed)
			{
				yield break;
			}
			this.m_heroPowerLinePlayed = true;
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX4_01_HP_02", "VO_NAX4_01_HP_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x000C354C File Offset: 0x000C174C
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
			if (NAX04_Noth.<>f__switch$map6A == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("NAX4_05", 0);
				NAX04_Noth.<>f__switch$map6A = dictionary;
			}
			int num;
			if (NAX04_Noth.<>f__switch$map6A.TryGetValue(cardId, ref num))
			{
				if (num == 0)
				{
					if (this.m_cardLinePlayed)
					{
						yield break;
					}
					this.m_cardLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX4_01_CARD_03", "VO_NAX4_01_CARD_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x0400177F RID: 6015
	private bool m_cardLinePlayed;

	// Token: 0x04001780 RID: 6016
	private bool m_heroPowerLinePlayed;
}
