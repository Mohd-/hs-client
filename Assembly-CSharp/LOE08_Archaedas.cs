using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000318 RID: 792
public class LOE08_Archaedas : LOE_MissionEntity
{
	// Token: 0x06002902 RID: 10498 RVA: 0x000C7054 File Offset: 0x000C5254
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOE_08_RESPONSE");
		base.PreloadSound("VO_LOEA08_TURN_1_BRANN");
		base.PreloadSound("VO_LOE_ARCHAEDAS_TURN_1_CARTOGRAPHER");
		base.PreloadSound("VO_LOE_08_LANDSLIDE");
		base.PreloadSound("VO_LOE_08_ANIMATE_STONE");
		base.PreloadSound("VO_LOE_08_WIN");
	}

	// Token: 0x06002903 RID: 10499 RVA: 0x000C70A4 File Offset: 0x000C52A4
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
			m_soundName = "VO_LOE_08_RESPONSE",
			m_stringTag = "VO_LOE_08_RESPONSE"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002904 RID: 10500 RVA: 0x000C710C File Offset: 0x000C530C
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		if (turn == 1)
		{
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA08_TURN_1_BRANN", 3f, 1f, false));
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_ARCHAEDAS_TURN_1_CARTOGRAPHER", 5f, 1f, false));
		}
		yield break;
	}

	// Token: 0x06002905 RID: 10501 RVA: 0x000C7138 File Offset: 0x000C5338
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
		if (this.m_playedLines.Contains(entity.GetCardId()))
		{
			yield break;
		}
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		string cardID = entity.GetCardId();
		string text = cardID;
		if (text != null)
		{
			if (LOE08_Archaedas.<>f__switch$map62 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("LOEA06_04", 0);
				dictionary.Add("LOEA06_03", 1);
				LOE08_Archaedas.<>f__switch$map62 = dictionary;
			}
			int num;
			if (LOE08_Archaedas.<>f__switch$map62.TryGetValue(text, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						this.m_playedLines.Add(cardID);
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_08_ANIMATE_STONE", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
					}
				}
				else
				{
					this.m_playedLines.Add(cardID);
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_08_LANDSLIDE", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002906 RID: 10502 RVA: 0x000C7164 File Offset: 0x000C5364
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Brann_Quote", "VO_LOE_08_WIN", "VO_LOE_08_WIN", 0f, false, false));
		}
		yield break;
	}

	// Token: 0x0400181F RID: 6175
	private HashSet<string> m_playedLines = new HashSet<string>();
}
