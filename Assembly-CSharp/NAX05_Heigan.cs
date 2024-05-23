using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002EF RID: 751
public class NAX05_Heigan : NAX_MissionEntity
{
	// Token: 0x06002815 RID: 10261 RVA: 0x000C357D File Offset: 0x000C177D
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX5_01_HP_02");
		base.PreloadSound("VO_NAX5_01_CARD_03");
		base.PreloadSound("VO_NAX5_01_EMOTE_05");
	}

	// Token: 0x06002816 RID: 10262 RVA: 0x000C35A0 File Offset: 0x000C17A0
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
			m_soundName = "VO_NAX5_01_EMOTE_05",
			m_stringTag = "VO_NAX5_01_EMOTE_05"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002817 RID: 10263 RVA: 0x000C3608 File Offset: 0x000C1808
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_HEIGAN2_55", "VO_KT_HEIGAN2_55", true);
		}
		yield break;
	}

	// Token: 0x06002818 RID: 10264 RVA: 0x000C362C File Offset: 0x000C182C
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
			if (NAX05_Heigan.<>f__switch$map6B == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("NAX5_02", 0);
				dictionary.Add("NAX5_03", 1);
				NAX05_Heigan.<>f__switch$map6B = dictionary;
			}
			int num;
			if (NAX05_Heigan.<>f__switch$map6B.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX5_01_CARD_03", "VO_NAX5_01_CARD_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX5_01_HP_02", "VO_NAX5_01_HP_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x04001782 RID: 6018
	private bool m_cardLinePlayed;

	// Token: 0x04001783 RID: 6019
	private bool m_heroPowerLinePlayed;
}
