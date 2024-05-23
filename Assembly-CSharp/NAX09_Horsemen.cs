using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F3 RID: 755
public class NAX09_Horsemen : NAX_MissionEntity
{
	// Token: 0x0600282B RID: 10283 RVA: 0x000C3A2C File Offset: 0x000C1C2C
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX9_01_CUSTOM_02");
		base.PreloadSound("VO_NAX9_01_EMOTE_04");
		base.PreloadSound("VO_FP1_031_EnterPlay_06");
		base.PreloadSound("VO_NAX9_02_CUSTOM_01");
		base.PreloadSound("VO_NAX9_03_CUSTOM_01");
		base.PreloadSound("VO_NAX9_04_CUSTOM_01");
		base.PreloadSound("VO_FP1_031_Attack_07");
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x000C3A88 File Offset: 0x000C1C88
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
			m_soundName = "VO_NAX9_01_EMOTE_04",
			m_stringTag = "VO_NAX9_01_EMOTE_04"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x000C3AF0 File Offset: 0x000C1CF0
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_BARON2_64", "VO_KT_BARON2_64", true);
		}
		yield break;
	}

	// Token: 0x0600282E RID: 10286 RVA: 0x000C3B14 File Offset: 0x000C1D14
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		if (turn == 1)
		{
			Actor baronActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
			Actor blaumeuxActor = null;
			Actor thaneActor = null;
			Actor zeliekActor = null;
			foreach (Card card in GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards())
			{
				string cardID = card.GetEntity().GetCardId();
				if (cardID == "NAX9_02")
				{
					blaumeuxActor = card.GetActor();
				}
				else if (cardID == "NAX9_03")
				{
					thaneActor = card.GetActor();
				}
				else if (cardID == "NAX9_04")
				{
					zeliekActor = card.GetActor();
				}
			}
			if (zeliekActor == null)
			{
				this.m_introSequenceComplete = true;
				yield break;
			}
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX9_02_CUSTOM_01", "VO_NAX9_02_CUSTOM_01", Notification.SpeechBubbleDirection.TopRight, zeliekActor, 1f, true, false));
			if (blaumeuxActor != null)
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX9_03_CUSTOM_01", "VO_NAX9_03_CUSTOM_01", Notification.SpeechBubbleDirection.TopRight, blaumeuxActor, 1f, true, false));
			}
			if (baronActor != null)
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX9_01_CUSTOM_02", "VO_NAX9_01_CUSTOM_02", Notification.SpeechBubbleDirection.TopRight, baronActor, 1f, true, false));
			}
			if (thaneActor != null)
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX9_04_CUSTOM_01", "VO_NAX9_04_CUSTOM_01", Notification.SpeechBubbleDirection.TopRight, thaneActor, 1f, true, false));
			}
			this.m_introSequenceComplete = true;
		}
		yield break;
	}

	// Token: 0x0600282F RID: 10287 RVA: 0x000C3B40 File Offset: 0x000C1D40
	protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
	{
		if (!this.m_introSequenceComplete)
		{
			yield break;
		}
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
			if (NAX09_Horsemen.<>f__switch$map6F == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("NAX9_06", 0);
				dictionary.Add("NAX9_07", 1);
				NAX09_Horsemen.<>f__switch$map6F = dictionary;
			}
			int num;
			if (NAX09_Horsemen.<>f__switch$map6F.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_FP1_031_Attack_07", "VO_FP1_031_Attack_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_FP1_031_EnterPlay_06", "VO_FP1_031_EnterPlay_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x0400178E RID: 6030
	private bool m_heroPowerLinePlayed;

	// Token: 0x0400178F RID: 6031
	private bool m_cardLinePlayed;

	// Token: 0x04001790 RID: 6032
	private bool m_introSequenceComplete;
}
