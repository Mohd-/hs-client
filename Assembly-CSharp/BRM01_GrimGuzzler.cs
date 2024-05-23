using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002FA RID: 762
public class BRM01_GrimGuzzler : BRM_MissionEntity
{
	// Token: 0x06002856 RID: 10326 RVA: 0x000C48AC File Offset: 0x000C2AAC
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA01_1_RESPONSE_03");
		base.PreloadSound("VO_BRMA01_1_HERO_POWER_04");
		base.PreloadSound("VO_BRMA01_1_CARD_05");
		base.PreloadSound("VO_BRMA01_1_ETC_06");
		base.PreloadSound("VO_BRMA01_1_SUCCUBUS_08");
		base.PreloadSound("VO_BRMA01_1_WARGOLEM_07");
		base.PreloadSound("VO_BRMA01_1_TURN1_02");
	}

	// Token: 0x06002857 RID: 10327 RVA: 0x000C4908 File Offset: 0x000C2B08
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
			m_soundName = "VO_BRMA01_1_RESPONSE_03",
			m_stringTag = "VO_BRMA01_1_RESPONSE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002858 RID: 10328 RVA: 0x000C4970 File Offset: 0x000C2B70
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
			if (BRM01_GrimGuzzler.<>f__switch$map4F == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("BRMA01_2", 0);
				dictionary.Add("BRMA01_2H", 0);
				dictionary.Add("BRMA01_4", 1);
				BRM01_GrimGuzzler.<>f__switch$map4F = dictionary;
			}
			int num;
			if (BRM01_GrimGuzzler.<>f__switch$map4F.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA01_1_CARD_05", "VO_BRMA01_1_CARD_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA01_1_HERO_POWER_04", "VO_BRMA01_1_HERO_POWER_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x000C499C File Offset: 0x000C2B9C
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (missionEvent)
		{
		case 1:
			if (!this.m_eTCLinePlayed)
			{
				if (!this.m_disableSpecialCardVO)
				{
					this.m_eTCLinePlayed = true;
					GameState.Get().SetBusy(true);
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA01_1_ETC_06", "VO_BRMA01_1_ETC_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					GameState.Get().SetBusy(false);
				}
			}
			break;
		case 2:
			if (!this.m_succubusLinePlayed)
			{
				if (!this.m_disableSpecialCardVO)
				{
					this.m_succubusLinePlayed = true;
					GameState.Get().SetBusy(true);
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA01_1_SUCCUBUS_08", "VO_BRMA01_1_SUCCUBUS_08", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					GameState.Get().SetBusy(false);
				}
			}
			break;
		case 3:
			if (!this.m_warGolemLinePlayed)
			{
				if (!this.m_disableSpecialCardVO)
				{
					this.m_warGolemLinePlayed = true;
					GameState.Get().SetBusy(true);
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA01_1_WARGOLEM_07", "VO_BRMA01_1_WARGOLEM_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					GameState.Get().SetBusy(false);
				}
			}
			break;
		}
		yield break;
	}

	// Token: 0x0600285A RID: 10330 RVA: 0x000C49C8 File Offset: 0x000C2BC8
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (turn)
		{
		case 1:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA01_1_TURN1_02", "VO_BRMA01_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			break;
		case 3:
			this.m_disableSpecialCardVO = false;
			break;
		}
		yield break;
	}

	// Token: 0x0600285B RID: 10331 RVA: 0x000C49F4 File Offset: 0x000C2BF4
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_COREN_DEAD_28"), "VO_NEFARIAN_COREN_DEAD_28", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017A5 RID: 6053
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017A6 RID: 6054
	private bool m_cardLinePlayed;

	// Token: 0x040017A7 RID: 6055
	private bool m_eTCLinePlayed;

	// Token: 0x040017A8 RID: 6056
	private bool m_succubusLinePlayed;

	// Token: 0x040017A9 RID: 6057
	private bool m_warGolemLinePlayed;

	// Token: 0x040017AA RID: 6058
	private bool m_disableSpecialCardVO = true;
}
