using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000309 RID: 777
public class BRM16_Atramedes : BRM_MissionEntity
{
	// Token: 0x060028B6 RID: 10422 RVA: 0x000C6012 File Offset: 0x000C4212
	public override string GetAlternatePlayerName()
	{
		return GameStrings.Get("MISSION_NEFARIAN_TITLE");
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x000C6020 File Offset: 0x000C4220
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA16_1_RESPONSE_03");
		base.PreloadSound("VO_BRMA16_1_HERO_POWER_05");
		base.PreloadSound("VO_BRMA16_1_CARD_04");
		base.PreloadSound("VO_BRMA16_1_GONG1_10");
		base.PreloadSound("VO_BRMA16_1_GONG2_11");
		base.PreloadSound("VO_BRMA16_1_GONG3_12");
		base.PreloadSound("VO_BRMA16_1_TRIGGER1_07");
		base.PreloadSound("VO_BRMA16_1_TRIGGER2_08");
		base.PreloadSound("VO_BRMA16_1_TRIGGER3_09");
		base.PreloadSound("VO_BRMA16_1_TURN1_02");
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x000C609C File Offset: 0x000C429C
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
			m_soundName = "VO_BRMA16_1_RESPONSE_03",
			m_stringTag = "VO_BRMA16_1_RESPONSE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x060028B9 RID: 10425 RVA: 0x000C6104 File Offset: 0x000C4304
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
			if (BRM16_Atramedes.<>f__switch$map5D == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("BRMA16_2", 0);
				dictionary.Add("BRMA16_2H", 0);
				dictionary.Add("BRMA16_3", 1);
				BRM16_Atramedes.<>f__switch$map5D = dictionary;
			}
			int num;
			if (BRM16_Atramedes.<>f__switch$map5D.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA16_1_CARD_04", "VO_BRMA16_1_CARD_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA16_1_HERO_POWER_05", "VO_BRMA16_1_HERO_POWER_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x000C6130 File Offset: 0x000C4330
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn == 1)
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA16_1_TURN1_02", "VO_BRMA16_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x060028BB RID: 10427 RVA: 0x000C615C File Offset: 0x000C435C
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (missionEvent != 1)
		{
			if (missionEvent == 2)
			{
				this.m_weaponLinePlayed++;
				switch (this.m_weaponLinePlayed)
				{
				case 1:
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA16_1_TRIGGER1_07", "VO_BRMA16_1_TRIGGER1_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 2:
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA16_1_TRIGGER2_08", "VO_BRMA16_1_TRIGGER2_08", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				case 3:
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA16_1_TRIGGER3_09", "VO_BRMA16_1_TRIGGER3_09", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					break;
				}
			}
		}
		else
		{
			this.m_gongLinePlayed++;
			switch (this.m_gongLinePlayed)
			{
			case 1:
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA16_1_GONG1_10", "VO_BRMA16_1_GONG1_10", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				break;
			case 2:
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA16_1_GONG3_12", "VO_BRMA16_1_GONG3_12", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				break;
			case 3:
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA16_1_GONG2_11", "VO_BRMA16_1_GONG2_11", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				break;
			}
		}
		yield break;
	}

	// Token: 0x060028BC RID: 10428 RVA: 0x000C6188 File Offset: 0x000C4388
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NefarianDragon_Quote", GameStrings.Get("VO_NEFARIAN_ATRAMEDES_DEATH_76"), "VO_NEFARIAN_ATRAMEDES_DEATH_76", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017DA RID: 6106
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017DB RID: 6107
	private bool m_cardLinePlayed;

	// Token: 0x040017DC RID: 6108
	private int m_gongLinePlayed;

	// Token: 0x040017DD RID: 6109
	private int m_weaponLinePlayed;
}
