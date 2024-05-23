using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200031C RID: 796
public class LOE12_Naga : LOE_MissionEntity
{
	// Token: 0x06002933 RID: 10547 RVA: 0x000C7C30 File Offset: 0x000C5E30
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOE_12_RESPONSE");
		base.PreloadSound("VO_LOE_12_NAZJAR_TURN_1");
		base.PreloadSound("VO_LOE_12_NAZJAR_TURN_1_FINLEY");
		base.PreloadSound("VO_LOE_12_NAZJAR_TURN_3_FINLEY");
		base.PreloadSound("VO_LOE_NAZJAR_TURN_3_CARTOGRAPHER");
		base.PreloadSound("VO_LOE_12_NAZJAR_TURN_5");
		base.PreloadSound("VO_LOE_12_NAZJAR_TURN_5_FINLEY");
		base.PreloadSound("VO_LOE_12_WIN");
		base.PreloadSound("VO_LOE_12_WIN_2");
		base.PreloadSound("VO_LOE_12_NAZJAR_PEARL");
	}

	// Token: 0x06002934 RID: 10548 RVA: 0x000C7CAC File Offset: 0x000C5EAC
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
			m_soundName = "VO_LOE_12_RESPONSE",
			m_stringTag = "VO_LOE_12_RESPONSE"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002935 RID: 10549 RVA: 0x000C7D14 File Offset: 0x000C5F14
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
		string cardID = entity.GetCardId();
		string text = cardID;
		if (text != null)
		{
			if (LOE12_Naga.<>f__switch$map64 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("LOEA12_3", 0);
				LOE12_Naga.<>f__switch$map64 = dictionary;
			}
			int num;
			if (LOE12_Naga.<>f__switch$map64.TryGetValue(text, ref num))
			{
				if (num == 0)
				{
					if (this.m_pearlLinePlayed)
					{
						yield break;
					}
					this.m_pearlLinePlayed = true;
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOE_12_NAZJAR_PEARL", 3f, 1f, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002936 RID: 10550 RVA: 0x000C7D40 File Offset: 0x000C5F40
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		yield return Gameplay.Get().StartCoroutine(base.HandleMissionEventWithTiming(missionEvent));
		if (missionEvent == 1)
		{
			GameState.Get().GetFriendlySidePlayer().WipeZzzs();
			GameState.Get().GetOpposingSidePlayer().WipeZzzs();
		}
		yield break;
	}

	// Token: 0x06002937 RID: 10551 RVA: 0x000C7D6C File Offset: 0x000C5F6C
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn != 1)
		{
			if (turn != 5)
			{
				if (turn == 9)
				{
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_12_NAZJAR_TURN_5", "VO_LOE_12_NAZJAR_TURN_5", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					yield return new WaitForSeconds(5.7f);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOE_12_NAZJAR_TURN_5_FINLEY", 3f, 1f, false));
				}
			}
			else
			{
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOE_12_NAZJAR_TURN_3_FINLEY", 3f, 1f, false));
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_NAZJAR_TURN_3_CARTOGRAPHER", 3f, 1f, false));
			}
		}
		else
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_12_NAZJAR_TURN_1", "VO_LOE_12_NAZJAR_TURN_1", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			yield return new WaitForSeconds(6.7f);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOE_12_NAZJAR_TURN_1_FINLEY", 3f, 1f, false));
		}
		yield break;
	}

	// Token: 0x06002938 RID: 10552 RVA: 0x000C7D98 File Offset: 0x000C5F98
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Blaggh_Quote", "VO_LOE_12_WIN", "VO_LOE_12_WIN", 0f, false, false));
		}
		yield break;
	}

	// Token: 0x04001829 RID: 6185
	private bool m_pearlLinePlayed;
}
