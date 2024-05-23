using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000313 RID: 787
public class LOE10_Giantfin : LOE_MissionEntity
{
	// Token: 0x060028E1 RID: 10465 RVA: 0x000C68E0 File Offset: 0x000C4AE0
	public override void StartGameplaySoundtracks()
	{
		MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_LOE_Wing3);
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x000C68F4 File Offset: 0x000C4AF4
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOEA10_1_MIDDLEFIN");
		base.PreloadSound("VO_LOE10_NYAH_FINLEY");
		base.PreloadSound("VO_LOE_10_NYAH");
		base.PreloadSound("VO_LOE_10_RESPONSE");
		base.PreloadSound("VO_LOE_10_START_2");
		base.PreloadSound("VO_LOE_10_TURN1");
		base.PreloadSound("VO_LOE_10_WIN");
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x000C6950 File Offset: 0x000C4B50
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		yield return Gameplay.Get().StartCoroutine(base.HandleMissionEventWithTiming(missionEvent));
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (missionEvent != 2)
		{
			if (missionEvent == 3)
			{
				if (this.m_cardLinePlayed1)
				{
					yield break;
				}
				this.m_cardLinePlayed1 = true;
			}
		}
		else
		{
			if (this.m_cardLinePlayed2)
			{
				yield break;
			}
			this.m_cardLinePlayed2 = true;
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA10_1_MIDDLEFIN", "VO_LOEA10_1_MIDDLEFIN", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x000C697C File Offset: 0x000C4B7C
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
		string cardID = entity.GetCardId();
		string text = cardID;
		if (text != null)
		{
			if (LOE10_Giantfin.<>f__switch$map63 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("LOEA10_5", 0);
				LOE10_Giantfin.<>f__switch$map63 = dictionary;
			}
			int num;
			if (LOE10_Giantfin.<>f__switch$map63.TryGetValue(text, ref num))
			{
				if (num == 0)
				{
					if (this.m_nyahLinePlayed)
					{
						yield break;
					}
					this.m_nyahLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_10_NYAH", "VO_LOE_10_NYAH", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					yield return new WaitForSeconds(4f);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOE10_NYAH_FINLEY", 3f, 1f, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x000C69A8 File Offset: 0x000C4BA8
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
			m_soundName = "VO_LOE_10_RESPONSE",
			m_stringTag = "VO_LOE_10_RESPONSE"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x000C6A10 File Offset: 0x000C4C10
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (this.m_turnToPlayFoundLine == 5)
		{
			this.m_turnToPlayFoundLine = 7;
		}
		if (turn == this.m_turnToPlayFoundLine)
		{
			this.m_turnToPlayFoundLine = -1;
			yield break;
		}
		if (turn == 1)
		{
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_10_TURN1", "VO_LOE_10_TURN1", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOE_10_START_2", 3f, 1f, false));
		}
		yield break;
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x000C6A3C File Offset: 0x000C4C3C
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Blaggh_Quote", "VO_LOE_10_WIN", "VO_LOE_10_WIN", 0f, false, false));
		}
		yield break;
	}

	// Token: 0x04001813 RID: 6163
	private bool m_cardLinePlayed1;

	// Token: 0x04001814 RID: 6164
	private bool m_cardLinePlayed2;

	// Token: 0x04001815 RID: 6165
	private bool m_nyahLinePlayed;

	// Token: 0x04001816 RID: 6166
	private int m_turnToPlayFoundLine = -1;
}
