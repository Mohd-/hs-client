using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000315 RID: 789
public class LOE01_Zinaar : LOE_MissionEntity
{
	// Token: 0x060028F0 RID: 10480 RVA: 0x000C6C10 File Offset: 0x000C4E10
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOE_02_RESPONSE");
		base.PreloadSound("VO_LOE_02_WISH");
		base.PreloadSound("VO_LOE_02_START2");
		base.PreloadSound("VO_LOE_02_START3");
		base.PreloadSound("VO_LOE_02_TURN_6");
		base.PreloadSound("VO_LOE_ZINAAR_TURN_6_CARTOGRAPHER_2");
		base.PreloadSound("VO_LOE_02_TURN_6_2");
		base.PreloadSound("VO_LOE_ZINAAR_TURN_6_CARTOGRAPHER_2_ALT");
		base.PreloadSound("VO_LOE_02_TURN_10");
		base.PreloadSound("VO_LOE_ZINAAR_TURN_10_CARTOGRAPHER_2");
		base.PreloadSound("VO_LOE_02_WIN");
		base.PreloadSound("VO_LOE_02_MORE_WISHES");
	}

	// Token: 0x060028F1 RID: 10481 RVA: 0x000C6CA4 File Offset: 0x000C4EA4
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
			m_soundName = "VO_LOE_02_RESPONSE",
			m_stringTag = "VO_LOE_02_RESPONSE"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x060028F2 RID: 10482 RVA: 0x000C6D0C File Offset: 0x000C4F0C
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		if (missionEvent == 2)
		{
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_02_WISH", 3f, 1f, false));
			GameState.Get().SetBusy(false);
		}
		yield break;
	}

	// Token: 0x060028F3 RID: 10483 RVA: 0x000C6D38 File Offset: 0x000C4F38
	protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
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
			if (LOE01_Zinaar.<>f__switch$map5F == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("LOEA02_06", 0);
				LOE01_Zinaar.<>f__switch$map5F = dictionary;
			}
			int num;
			if (LOE01_Zinaar.<>f__switch$map5F.TryGetValue(text, ref num))
			{
				if (num == 0)
				{
					if (this.m_wishMoreWishesLinePlayed)
					{
						yield break;
					}
					this.m_wishMoreWishesLinePlayed = true;
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_02_MORE_WISHES", 3f, 1f, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x060028F4 RID: 10484 RVA: 0x000C6D64 File Offset: 0x000C4F64
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		switch (turn)
		{
		case 7:
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_02_TURN_6", 3f, 1f, false));
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_ZINAAR_TURN_6_CARTOGRAPHER_2", 3f, 1f, false));
			break;
		default:
			if (turn != 1)
			{
				if (turn == 13)
				{
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_02_TURN_10", 3f, 1f, false));
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_ZINAAR_TURN_10_CARTOGRAPHER_2", 3f, 1f, false));
				}
			}
			else
			{
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_02_START2", 3f, 1f, false));
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_02_START3", 3f, 1f, false));
			}
			break;
		case 9:
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_02_TURN_6_2", 3f, 1f, false));
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_ZINAAR_TURN_6_CARTOGRAPHER_2_ALT", 3f, 1f, false));
			break;
		}
		yield break;
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x000C6D90 File Offset: 0x000C4F90
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Reno_Quote", "VO_LOE_02_WIN", 0f, false, false));
		}
		yield break;
	}

	// Token: 0x0400181B RID: 6171
	private bool m_wishMoreWishesLinePlayed;
}
