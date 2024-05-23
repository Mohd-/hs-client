using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000319 RID: 793
public class LOE14_Steel_Sentinel : LOE_MissionEntity
{
	// Token: 0x06002908 RID: 10504 RVA: 0x000C7198 File Offset: 0x000C5398
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOE_14_START");
		base.PreloadSound("VO_LOE_14_TURN_5");
		base.PreloadSound("VO_LOE_14_TURN_5_2");
		base.PreloadSound("VO_LOE_14_TURN_9");
		base.PreloadSound("VO_LOE_14_TURN_13");
		base.PreloadSound("VO_LOE_14_WIN");
		base.PreloadSound("LOEA14_1_SteelSentinel_Response");
	}

	// Token: 0x06002909 RID: 10505 RVA: 0x000C71F4 File Offset: 0x000C53F4
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
			m_soundName = "LOEA14_1_SteelSentinel_Response",
			m_stringTag = "VO_LOE_14_RESPONSE"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600290A RID: 10506 RVA: 0x000C725C File Offset: 0x000C545C
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
				if (turn != 9)
				{
					if (turn == 13)
					{
						yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_14_TURN_13", 3f, 1f, false));
					}
				}
				else
				{
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Rafaam_wrap_BigQuote", "VO_LOE_14_TURN_9", 3f, 1f, false));
				}
			}
			else
			{
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOE_14_TURN_5", 3f, 1f, false));
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_14_TURN_5_2", 3f, 1f, false));
			}
		}
		else
		{
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOE_14_START", 3f, 1f, false));
		}
		yield break;
	}

	// Token: 0x0600290B RID: 10507 RVA: 0x000C7288 File Offset: 0x000C5488
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Cartographer_Quote", "VO_LOE_14_WIN", 0f, false, false));
		}
		yield break;
	}
}
