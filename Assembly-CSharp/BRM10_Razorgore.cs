using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000303 RID: 771
public class BRM10_Razorgore : BRM_MissionEntity
{
	// Token: 0x0600288F RID: 10383 RVA: 0x000C5678 File Offset: 0x000C3878
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA10_1_RESPONSE_03");
		base.PreloadSound("VO_BRMA10_1_HERO_POWER_04");
		base.PreloadSound("VO_BRMA10_1_EGG_DEATH_1_05");
		base.PreloadSound("VO_BRMA10_1_EGG_DEATH_2_06");
		base.PreloadSound("VO_BRMA10_1_EGG_DEATH_3_07");
		base.PreloadSound("VO_BRMA10_1_TURN1_02");
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x000C56C8 File Offset: 0x000C38C8
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
			m_soundName = "VO_BRMA10_1_RESPONSE_03",
			m_stringTag = "VO_BRMA10_1_RESPONSE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x000C5730 File Offset: 0x000C3930
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
			if (BRM10_Razorgore.<>f__switch$map57 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("BRMA10_3", 0);
				dictionary.Add("BRMA10_3H", 0);
				BRM10_Razorgore.<>f__switch$map57 = dictionary;
			}
			int num;
			if (BRM10_Razorgore.<>f__switch$map57.TryGetValue(cardId, ref num))
			{
				if (num == 0)
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA10_1_HERO_POWER_04", "VO_BRMA10_1_HERO_POWER_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x000C575C File Offset: 0x000C395C
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn == 1)
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA10_1_TURN1_02", "VO_BRMA10_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x06002893 RID: 10387 RVA: 0x000C5788 File Offset: 0x000C3988
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (missionEvent == 1)
		{
			this.m_eggDeathLinePlayed++;
			switch (this.m_eggDeathLinePlayed)
			{
			case 1:
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA10_1_EGG_DEATH_1_05", "VO_BRMA10_1_EGG_DEATH_1_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				break;
			case 2:
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA10_1_EGG_DEATH_2_06", "VO_BRMA10_1_EGG_DEATH_2_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				break;
			case 3:
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA10_1_EGG_DEATH_3_07", "VO_BRMA10_1_EGG_DEATH_3_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				break;
			}
		}
		yield break;
	}

	// Token: 0x06002894 RID: 10388 RVA: 0x000C57B4 File Offset: 0x000C39B4
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_RAZORGORE_DEAD_55"), "VO_NEFARIAN_RAZORGORE_DEAD_55", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017C4 RID: 6084
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017C5 RID: 6085
	private int m_eggDeathLinePlayed;
}
