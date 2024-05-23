using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000316 RID: 790
public class LOE02_Sun_Raider_Phaerix : LOE_MissionEntity
{
	// Token: 0x060028F7 RID: 10487 RVA: 0x000C6DC4 File Offset: 0x000C4FC4
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOE_01_RESPONSE");
		base.PreloadSound("VO_LOE_01_WOUNDED");
		base.PreloadSound("VO_LOE_01_STAFF");
		base.PreloadSound("VO_LOE_01_STAFF_2");
		base.PreloadSound("VO_LOE_02_PHAERIX_STAFF_RECOVER");
		base.PreloadSound("VO_LOE_01_STAFF_2_RENO");
		base.PreloadSound("VO_LOE_01_WIN_2");
		base.PreloadSound("VO_LOE_01_WIN_2_ALT_2");
		base.PreloadSound("VO_LOE_01_START");
		base.PreloadSound("VO_LOE_01_WIN");
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x000C6E40 File Offset: 0x000C5040
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
			m_soundName = "VO_LOE_01_RESPONSE",
			m_stringTag = "VO_LOE_01_RESPONSE"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x000C6EA8 File Offset: 0x000C50A8
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (this.m_staffLinesPlayed < missionEvent)
		{
			if (missionEvent > 9)
			{
				if (!this.m_damageLinePlayed)
				{
					this.m_damageLinePlayed = true;
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_01_WOUNDED", 3f, 1f, false));
				}
			}
			else
			{
				this.m_staffLinesPlayed = missionEvent;
				switch (missionEvent)
				{
				case 1:
					GameState.Get().SetBusy(true);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_01_STAFF", 3f, 1f, false));
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_01_STAFF_2", "VO_LOE_01_STAFF_2", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					GameState.Get().SetBusy(false);
					break;
				case 2:
					GameState.Get().SetBusy(true);
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_02_PHAERIX_STAFF_RECOVER", "VO_LOE_02_PHAERIX_STAFF_RECOVER", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					GameState.Get().SetBusy(false);
					break;
				case 3:
					GameState.Get().SetBusy(true);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_01_STAFF_2_RENO", 3f, 1f, false));
					GameState.Get().SetBusy(false);
					break;
				}
			}
		}
		yield break;
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x000C6ED4 File Offset: 0x000C50D4
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn == 1)
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_LOE_01_START", "VO_LOE_01_START", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			yield return new WaitForSeconds(4f);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Reno_BigQuote", "VO_LOE_01_WIN_2", 3f, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x000C6F00 File Offset: 0x000C5100
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Reno_Quote", "VO_LOE_01_WIN", "VO_LOE_01_WIN", 0f, false, false));
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Reno_Quote", "VO_LOE_01_WIN_2_ALT_2", "VO_LOE_01_WIN_2_ALT_2", 0f, false, false));
		}
		yield break;
	}

	// Token: 0x0400181D RID: 6173
	private int m_staffLinesPlayed;

	// Token: 0x0400181E RID: 6174
	private bool m_damageLinePlayed;
}
