using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000317 RID: 791
public class LOE04_Scarvash : LOE_MissionEntity
{
	// Token: 0x060028FD RID: 10493 RVA: 0x000C6F34 File Offset: 0x000C5134
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOEA04_BRANN_TURN_1");
		base.PreloadSound("VO_LOE_04_SCARVASH_TURN_2");
		base.PreloadSound("VO_BRANN_MITHRIL_ALT_02");
		base.PreloadSound("VO_LOE_SCARVASH_TURN_6_CARTOGRAPHER");
		base.PreloadSound("VO_LOE_04_RESPONSE");
		base.PreloadSound("VO_LOE_04_WIN");
	}

	// Token: 0x060028FE RID: 10494 RVA: 0x000C6F84 File Offset: 0x000C5184
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
			m_soundName = "VO_LOE_04_RESPONSE",
			m_stringTag = "VO_LOE_04_RESPONSE"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x060028FF RID: 10495 RVA: 0x000C6FEC File Offset: 0x000C51EC
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn != 1)
		{
			if (turn == 7)
			{
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_BRANN_MITHRIL_ALT_02", 3f, 1f, false));
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_SCARVASH_TURN_6_CARTOGRAPHER", 3f, 1f, false));
			}
		}
		else
		{
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA04_BRANN_TURN_1", 3f, 1f, false));
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_04_SCARVASH_TURN_2", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x06002900 RID: 10496 RVA: 0x000C7018 File Offset: 0x000C5218
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Brann_Quote", "VO_LOE_04_WIN", 0f, false, false));
		}
		yield break;
	}
}
