using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002EB RID: 747
public class NAX01_AnubRekhan : NAX_MissionEntity
{
	// Token: 0x060027FE RID: 10238 RVA: 0x000C307B File Offset: 0x000C127B
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX1_01_HP_03");
		base.PreloadSound("VO_NAX1_01_CARD_02");
		base.PreloadSound("VO_NAX1_01_EMOTE_04");
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000C30A0 File Offset: 0x000C12A0
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
			m_soundName = "VO_NAX1_01_EMOTE_04",
			m_stringTag = "VO_NAX1_01_EMOTE_04"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x000C3108 File Offset: 0x000C1308
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_ANUB2_43", "VO_KT_ANUB2_43", true);
		}
		yield break;
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x000C312C File Offset: 0x000C132C
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
			if (NAX01_AnubRekhan.<>f__switch$map67 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("NAX1_04", 0);
				dictionary.Add("NAX1_05", 1);
				NAX01_AnubRekhan.<>f__switch$map67 = dictionary;
			}
			int num;
			if (NAX01_AnubRekhan.<>f__switch$map67.TryGetValue(cardId, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						if (this.m_locustSwarmLinePlayed)
						{
							yield break;
						}
						this.m_locustSwarmLinePlayed = true;
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX1_01_CARD_02", "VO_NAX1_01_CARD_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX1_01_HP_03", "VO_NAX1_01_HP_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x04001776 RID: 6006
	private bool m_locustSwarmLinePlayed;

	// Token: 0x04001777 RID: 6007
	private bool m_heroPowerLinePlayed;
}
