using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000304 RID: 772
public class BRM11_Vaelastrasz : BRM_MissionEntity
{
	// Token: 0x06002896 RID: 10390 RVA: 0x000C57E0 File Offset: 0x000C39E0
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA11_1_RESPONSE_03");
		base.PreloadSound("VO_BRMA11_1_HERO_POWER_05");
		base.PreloadSound("VO_BRMA11_1_CARD_04");
		base.PreloadSound("VO_BRMA11_1_KILL_PLAYER_06");
		base.PreloadSound("VO_BRMA11_1_ALEXSTRAZA_07");
		base.PreloadSound("VO_BRMA11_1_TURN1_02");
	}

	// Token: 0x06002897 RID: 10391 RVA: 0x000C5830 File Offset: 0x000C3A30
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
			m_soundName = "VO_BRMA11_1_RESPONSE_03",
			m_stringTag = "VO_BRMA11_1_RESPONSE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002898 RID: 10392 RVA: 0x000C5898 File Offset: 0x000C3A98
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
			if (BRM11_Vaelastrasz.<>f__switch$map58 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("BRMA11_3", 0);
				BRM11_Vaelastrasz.<>f__switch$map58 = dictionary;
			}
			int num;
			if (BRM11_Vaelastrasz.<>f__switch$map58.TryGetValue(cardId, ref num))
			{
				if (num == 0)
				{
					if (this.m_cardLinePlayed)
					{
						yield break;
					}
					this.m_cardLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA11_1_HERO_POWER_05", "VO_BRMA11_1_HERO_POWER_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002899 RID: 10393 RVA: 0x000C58C4 File Offset: 0x000C3AC4
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn != 1)
		{
			if (turn == 2)
			{
				while (this.m_enemySpeaking)
				{
					yield return null;
				}
				Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA11_1_CARD_04", "VO_BRMA11_1_CARD_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			}
		}
		else
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA11_1_TURN1_02", "VO_BRMA11_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x0600289A RID: 10394 RVA: 0x000C58F0 File Offset: 0x000C3AF0
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (missionEvent != 1)
		{
			if (missionEvent == 2)
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA11_1_ALEXSTRAZA_07", "VO_BRMA11_1_ALEXSTRAZA_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			}
		}
		else
		{
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA11_1_KILL_PLAYER_06", "VO_BRMA11_1_KILL_PLAYER_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			GameState.Get().SetBusy(false);
		}
		yield break;
	}

	// Token: 0x0600289B RID: 10395 RVA: 0x000C591C File Offset: 0x000C3B1C
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.LOST)
		{
			yield return new WaitForSeconds(5f);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA11_1_KILL_PLAYER_06", 1f, true, false));
		}
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_VAEL_DEAD_57"), "VO_NEFARIAN_VAEL_DEAD_57", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017C7 RID: 6087
	private bool m_cardLinePlayed;
}
