using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200031F RID: 799
public class LOE13_Skelesaurus : LOE_MissionEntity
{
	// Token: 0x0600294C RID: 10572 RVA: 0x000C83BC File Offset: 0x000C65BC
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOE_13_TURN_1");
		base.PreloadSound("VO_LOE_13_TURN_1_2");
		base.PreloadSound("VO_LOE_13_TURN_5");
		base.PreloadSound("VO_LOE_13_TURN_5_2");
		base.PreloadSound("VO_LOE_13_TURN_9");
		base.PreloadSound("VO_LOE_13_TURN_9_2");
		base.PreloadSound("VO_LOE_13_WIN");
		base.PreloadSound("LOEA13_1_SkelesaurusHex_EmoteResponse");
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x000C8424 File Offset: 0x000C6624
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
			m_soundName = "LOEA13_1_SkelesaurusHex_EmoteResponse",
			m_stringTag = "VO_LOE_13_RESPONSE"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x000C848C File Offset: 0x000C668C
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		if (turn != 1)
		{
			if (turn != 5)
			{
				if (turn == 9)
				{
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOE_13_TURN_9", 3f, 1f, false));
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_13_TURN_9_2", 3f, 1f, false));
				}
			}
			else
			{
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Rafaam_wrap_BigQuote", "VO_LOE_13_TURN_5", 3f, 1f, false));
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_13_TURN_5_2", 3f, 1f, false));
			}
		}
		else
		{
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_13_TURN_1", 3f, 1f, false));
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Rafaam_wrap_BigQuote", "VO_LOE_13_TURN_1_2", 3f, 1f, false));
		}
		yield break;
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x000C84B8 File Offset: 0x000C66B8
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Cartographer_Quote", "VO_LOE_13_WIN", 0f, false, false));
		}
		yield break;
	}
}
