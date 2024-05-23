using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F7 RID: 759
public class NAX13_Thaddius : NAX_MissionEntity
{
	// Token: 0x06002842 RID: 10306 RVA: 0x000C3F51 File Offset: 0x000C2151
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX13_01_HP_02");
		base.PreloadSound("VO_NAX13_01_EMOTE_04");
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x000C3F6C File Offset: 0x000C216C
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
			m_soundName = "VO_NAX13_01_EMOTE_04",
			m_stringTag = "VO_NAX13_01_EMOTE_04"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x000C3FD4 File Offset: 0x000C21D4
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_THADDIUS2_81", "VO_KT_THADDIUS2_81", true);
		}
		yield break;
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x000C3FF8 File Offset: 0x000C21F8
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
			if (NAX13_Thaddius.<>f__switch$map73 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("NAX13_02", 0);
				NAX13_Thaddius.<>f__switch$map73 = dictionary;
			}
			int num;
			if (NAX13_Thaddius.<>f__switch$map73.TryGetValue(cardId, ref num))
			{
				if (num == 0)
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX13_01_HP_02", "VO_NAX13_01_HP_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x0400179B RID: 6043
	private bool m_heroPowerLinePlayed;
}
