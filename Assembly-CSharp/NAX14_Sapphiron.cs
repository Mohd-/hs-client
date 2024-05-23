using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F8 RID: 760
public class NAX14_Sapphiron : NAX_MissionEntity
{
	// Token: 0x06002847 RID: 10311 RVA: 0x000C402C File Offset: 0x000C222C
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX14_01_EMOTE_01");
		base.PreloadSound("VO_NAX14_01_EMOTE_02");
		base.PreloadSound("VO_NAX14_01_EMOTE_03");
		base.PreloadSound("VO_NAX14_01_CARD_01");
		base.PreloadSound("VO_NAX14_01_HP_01");
		base.PreloadSound("VO_KT_SAPPHIRON2_84");
		base.PreloadSound("VO_KT_SAPPHIRON3_85");
		base.PreloadSound("VO_KT_SAPPHIRON4_ALT_87");
		base.PreloadSound("VO_KT_SAPPHIRON5_88");
	}

	// Token: 0x06002848 RID: 10312 RVA: 0x000C409C File Offset: 0x000C229C
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
			m_soundName = "VO_NAX14_01_EMOTE_01",
			m_stringTag = "VO_NAX14_01_EMOTE_01"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX14_01_EMOTE_02",
			m_stringTag = "VO_NAX14_01_EMOTE_02"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX14_01_EMOTE_03",
			m_stringTag = "VO_NAX14_01_EMOTE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002849 RID: 10313 RVA: 0x000C414C File Offset: 0x000C234C
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		if (turn == 1)
		{
			if (GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId() != "NAX14_01H")
			{
				NotificationManager.Get().CreateKTQuote("VO_KT_SAPPHIRON2_84", "VO_KT_SAPPHIRON2_84", false);
			}
		}
		yield break;
	}

	// Token: 0x0600284A RID: 10314 RVA: 0x000C4170 File Offset: 0x000C2370
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_SAPPHIRON5_88", "VO_KT_SAPPHIRON5_88", true);
		}
		yield break;
	}

	// Token: 0x0600284B RID: 10315 RVA: 0x000C4194 File Offset: 0x000C2394
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		yield return Gameplay.Get().StartCoroutine(base.HandleMissionEventWithTiming(missionEvent));
		if (missionEvent == 1)
		{
			this.m_numTimesFrostBreathMisses++;
			if (this.m_numTimesFrostBreathMisses == 4)
			{
				NotificationManager.Get().CreateKTQuote("VO_KT_SAPPHIRON3_85", "VO_KT_SAPPHIRON3_85", true);
			}
		}
		yield break;
	}

	// Token: 0x0600284C RID: 10316 RVA: 0x000C41C0 File Offset: 0x000C23C0
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
			if (NAX14_Sapphiron.<>f__switch$map74 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("NAX14_02", 0);
				dictionary.Add("NAX14_04", 1);
				NAX14_Sapphiron.<>f__switch$map74 = dictionary;
			}
			int num;
			if (NAX14_Sapphiron.<>f__switch$map74.TryGetValue(cardId, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						yield return new WaitForSeconds(1f);
						if (this.m_cardKtLinePlayed)
						{
							Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX14_01_CARD_01", "VO_NAX14_01_CARD_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
						}
						else
						{
							NotificationManager.Get().CreateKTQuote("VO_KT_SAPPHIRON4_ALT_87", "VO_KT_SAPPHIRON4_ALT_87", false);
							this.m_cardKtLinePlayed = true;
						}
					}
				}
				else
				{
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX14_01_HP_01", "VO_NAX14_01_HP_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x0400179D RID: 6045
	private bool m_cardKtLinePlayed;

	// Token: 0x0400179E RID: 6046
	private int m_numTimesFrostBreathMisses;
}
