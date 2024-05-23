using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public class NAX11_Grobbulus : NAX_MissionEntity
{
	// Token: 0x06002837 RID: 10295 RVA: 0x000C3CEE File Offset: 0x000C1EEE
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX11_01_HP_02");
		base.PreloadSound("VO_NAX11_01_CARD_03");
		base.PreloadSound("VO_NAX11_01_EMOTE_04");
	}

	// Token: 0x06002838 RID: 10296 RVA: 0x000C3D14 File Offset: 0x000C1F14
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
			m_soundName = "VO_NAX11_01_EMOTE_04",
			m_stringTag = "VO_NAX11_01_EMOTE_04"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x000C3D7C File Offset: 0x000C1F7C
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_GROBBULUS2_71", "VO_KT_GROBBULUS2_71", true);
		}
		yield break;
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x000C3DA0 File Offset: 0x000C1FA0
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
			if (NAX11_Grobbulus.<>f__switch$map71 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("NAX11_02", 0);
				dictionary.Add("NAX11_04", 1);
				NAX11_Grobbulus.<>f__switch$map71 = dictionary;
			}
			int num;
			if (NAX11_Grobbulus.<>f__switch$map71.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX11_01_CARD_03", "VO_NAX11_01_CARD_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX11_01_HP_02", "VO_NAX11_01_HP_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x04001794 RID: 6036
	private bool m_cardLinePlayed;

	// Token: 0x04001795 RID: 6037
	private bool m_heroPowerLinePlayed;
}
