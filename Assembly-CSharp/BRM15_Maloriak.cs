using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000307 RID: 775
public class BRM15_Maloriak : BRM_MissionEntity
{
	// Token: 0x060028A9 RID: 10409 RVA: 0x000C5C84 File Offset: 0x000C3E84
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA15_1_RESPONSE_03");
		base.PreloadSound("VO_BRMA15_1_HERO_POWER_06");
		base.PreloadSound("VO_BRMA15_1_CARD_05");
		base.PreloadSound("VO_BRMA15_1_TURN1_02");
		base.PreloadSound("VO_NEFARIAN_MALORIAK_TURN2_71");
		base.PreloadSound("VO_NEFARIAN_MALORIAK_DEATH_PT1_72");
		base.PreloadSound("VO_NEFARIAN_MALORIAK_DEATH_PT2_73");
		base.PreloadSound("VO_NEFARIAN_MALORIAK_DEATH_PT3_74");
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x000C5CEC File Offset: 0x000C3EEC
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
			m_soundName = "VO_BRMA15_1_RESPONSE_03",
			m_stringTag = "VO_BRMA15_1_RESPONSE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x060028AB RID: 10411 RVA: 0x000C5D54 File Offset: 0x000C3F54
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
			if (BRM15_Maloriak.<>f__switch$map5C == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("BRMA15_2", 0);
				dictionary.Add("BRMA15_2H", 0);
				dictionary.Add("BRMA15_3", 1);
				BRM15_Maloriak.<>f__switch$map5C = dictionary;
			}
			int num;
			if (BRM15_Maloriak.<>f__switch$map5C.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA15_1_CARD_05", "VO_BRMA15_1_CARD_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA15_1_HERO_POWER_06", "VO_BRMA15_1_HERO_POWER_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x060028AC RID: 10412 RVA: 0x000C5D80 File Offset: 0x000C3F80
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn != 1)
		{
			if (turn == 2)
			{
				NotificationManager.Get().CreateCharacterQuote("NefarianDragon_Quote", new Vector3(95f, NotificationManager.DEPTH, 36.8f), GameStrings.Get("VO_NEFARIAN_MALORIAK_TURN2_71"), "VO_NEFARIAN_MALORIAK_TURN2_71", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
			}
		}
		else
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA15_1_TURN1_02", "VO_BRMA15_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x060028AD RID: 10413 RVA: 0x000C5DAC File Offset: 0x000C3FAC
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NefarianDragon_Quote", GameStrings.Get("VO_NEFARIAN_MALORIAK_DEATH_PT2_73"), string.Empty, true, 5f, CanvasAnchor.BOTTOM_LEFT);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_NEFARIAN_MALORIAK_DEATH_PT2_73", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
			NotificationManager.Get().DestroyActiveQuote(0f);
			NotificationManager.Get().CreateCharacterQuote("NefarianDragon_Quote", GameStrings.Get("VO_NEFARIAN_MALORIAK_DEATH_PT3_74"), string.Empty, true, 7f, CanvasAnchor.BOTTOM_LEFT);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_NEFARIAN_MALORIAK_DEATH_PT3_74", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
			NotificationManager.Get().DestroyActiveQuote(0f);
		}
		yield break;
	}

	// Token: 0x040017D0 RID: 6096
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017D1 RID: 6097
	private bool m_cardLinePlayed;
}
