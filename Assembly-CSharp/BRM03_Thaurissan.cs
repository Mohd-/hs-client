using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002FC RID: 764
public class BRM03_Thaurissan : BRM_MissionEntity
{
	// Token: 0x06002863 RID: 10339 RVA: 0x000C4CD0 File Offset: 0x000C2ED0
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA03_1_RESPONSE_03");
		base.PreloadSound("VO_BRMA03_1_HERO_POWER_06");
		base.PreloadSound("VO_BRMA03_1_CARD_04");
		base.PreloadSound("VO_BRMA03_1_MOIRA_DEATH_05");
		base.PreloadSound("VO_BRMA03_1_VS_RAG_07");
		base.PreloadSound("VO_BRMA03_1_TURN1_02");
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x000C4D20 File Offset: 0x000C2F20
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
			m_soundName = "VO_BRMA03_1_RESPONSE_03",
			m_stringTag = "VO_BRMA03_1_RESPONSE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002865 RID: 10341 RVA: 0x000C4D88 File Offset: 0x000C2F88
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
			if (BRM03_Thaurissan.<>f__switch$map51 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("BRMA03_2", 0);
				dictionary.Add("BRMA_01", 1);
				BRM03_Thaurissan.<>f__switch$map51 = dictionary;
			}
			int num;
			if (BRM03_Thaurissan.<>f__switch$map51.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA03_1_CARD_04", "VO_BRMA03_1_CARD_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA03_1_HERO_POWER_06", "VO_BRMA03_1_HERO_POWER_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002866 RID: 10342 RVA: 0x000C4DB4 File Offset: 0x000C2FB4
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn == 1)
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA03_1_TURN1_02", "VO_BRMA03_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x000C4DE0 File Offset: 0x000C2FE0
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (missionEvent != 1)
		{
			if (missionEvent == 2)
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA03_1_VS_RAG_07", "VO_BRMA03_1_VS_RAG_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			}
		}
		else
		{
			this.m_moiraDead = true;
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA03_1_MOIRA_DEATH_05", "VO_BRMA03_1_MOIRA_DEATH_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			GameState.Get().SetBusy(false);
		}
		yield break;
	}

	// Token: 0x06002868 RID: 10344 RVA: 0x000C4E0C File Offset: 0x000C300C
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			if (this.m_moiraDead)
			{
				yield return new WaitForSeconds(5f);
				NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_THAURISSAN_DEAD2_33"), "VO_NEFARIAN_THAURISSAN_DEAD2_33", true, 0f, CanvasAnchor.BOTTOM_LEFT);
			}
			else
			{
				yield return new WaitForSeconds(5f);
				NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_THAURISSAN_DEAD_32"), "VO_NEFARIAN_THAURISSAN_DEAD_32", true, 0f, CanvasAnchor.BOTTOM_LEFT);
			}
		}
		yield break;
	}

	// Token: 0x040017AF RID: 6063
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017B0 RID: 6064
	private bool m_cardLinePlayed;

	// Token: 0x040017B1 RID: 6065
	private bool m_moiraDead;
}
