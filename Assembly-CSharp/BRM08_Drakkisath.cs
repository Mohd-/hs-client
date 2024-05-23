using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000301 RID: 769
public class BRM08_Drakkisath : BRM_MissionEntity
{
	// Token: 0x06002883 RID: 10371 RVA: 0x000C53F4 File Offset: 0x000C35F4
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA08_1_RESPONSE_04");
		base.PreloadSound("VO_BRMA08_1_CARD_05");
		base.PreloadSound("VO_BRMA08_1_TURN1_03");
		base.PreloadSound("VO_NEFARIAN_DRAKKISATH_RESPOND_48");
		base.PreloadSound("VO_BRMA08_1_TURN1_ALT_02");
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x000C5438 File Offset: 0x000C3638
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
			m_soundName = "VO_BRMA08_1_RESPONSE_04",
			m_stringTag = "VO_BRMA08_1_RESPONSE_04"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002885 RID: 10373 RVA: 0x000C54A0 File Offset: 0x000C36A0
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
			if (BRM08_Drakkisath.<>f__switch$map55 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("BRMA08_3", 0);
				BRM08_Drakkisath.<>f__switch$map55 = dictionary;
			}
			int num;
			if (BRM08_Drakkisath.<>f__switch$map55.TryGetValue(cardId, ref num))
			{
				if (num == 0)
				{
					if (this.m_cardLinePlayed)
					{
						yield break;
					}
					this.m_cardLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA08_1_CARD_05", "VO_BRMA08_1_CARD_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002886 RID: 10374 RVA: 0x000C54CC File Offset: 0x000C36CC
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Vector3 quotePos = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (turn)
		{
		case 1:
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA08_1_TURN1_ALT_02", "VO_BRMA08_1_TURN1_ALT_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			break;
		case 4:
			if (!GameMgr.Get().IsClassChallengeMission())
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA08_1_TURN1_03", "VO_BRMA08_1_TURN1_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_DRAKKISATH_RESPOND_48"), "VO_NEFARIAN_DRAKKISATH_RESPOND_48", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
			}
			break;
		case 6:
			if (!GameMgr.Get().IsClassChallengeMission())
			{
				NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_DRAKKISATH1_49"), "VO_NEFARIAN_DRAKKISATH1_49", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
			}
			break;
		}
		yield break;
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x000C54F8 File Offset: 0x000C36F8
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_DRAKKISATH_DEAD_50"), "VO_NEFARIAN_DRAKKISATH_DEAD_50", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017BC RID: 6076
	private bool m_cardLinePlayed;
}
