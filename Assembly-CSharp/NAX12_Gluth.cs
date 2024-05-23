using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F6 RID: 758
public class NAX12_Gluth : NAX_MissionEntity
{
	// Token: 0x0600283C RID: 10300 RVA: 0x000C3DD4 File Offset: 0x000C1FD4
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX12_01_HP_01");
		base.PreloadSound("VO_NAX12_01_EMOTE_03");
		base.PreloadSound("VO_NAX12_01_EMOTE_02");
		base.PreloadSound("VO_NAX12_01_EMOTE_01");
		base.PreloadSound("VO_NAX12_01_CARD_01");
	}

	// Token: 0x0600283D RID: 10301 RVA: 0x000C3E18 File Offset: 0x000C2018
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
			m_soundName = "VO_NAX12_01_EMOTE_01",
			m_stringTag = "VO_NAX12_01_EMOTE_01"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX12_01_EMOTE_02",
			m_stringTag = "VO_NAX12_01_EMOTE_02"
		});
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX12_01_EMOTE_03",
			m_stringTag = "VO_NAX12_01_EMOTE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600283E RID: 10302 RVA: 0x000C3EC8 File Offset: 0x000C20C8
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		if (turn != 1)
		{
			if (turn == 13)
			{
				this.m_achievementTauntPlayed = true;
				NotificationManager.Get().CreateKTQuote("VO_KT_GLUTH2_ALT_74", "VO_KT_GLUTH2_ALT_74", false);
			}
		}
		else
		{
			NotificationManager.Get().CreateKTQuote("VO_KT_GLUTH2_73", "VO_KT_GLUTH2_73", false);
		}
		yield break;
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x000C3EF4 File Offset: 0x000C20F4
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_GLUTH4_76", "VO_KT_GLUTH4_76", true);
		}
		if (gameResult == TAG_PLAYSTATE.LOST && this.m_achievementTauntPlayed)
		{
			NotificationManager.Get().CreateKTQuote("VO_KT_GLUTH3_75", "VO_KT_GLUTH3_75", false);
		}
		yield break;
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x000C3F20 File Offset: 0x000C2120
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
			if (NAX12_Gluth.<>f__switch$map72 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("NAX12_02", 0);
				dictionary.Add("NAX12_04", 1);
				NAX12_Gluth.<>f__switch$map72 = dictionary;
			}
			int num;
			if (NAX12_Gluth.<>f__switch$map72.TryGetValue(cardId, ref num))
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
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX12_01_CARD_01", "VO_NAX12_01_CARD_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
						yield return new WaitForSeconds(1f);
						NotificationManager.Get().CreateKTQuote("VO_KT_GLUTH6_78", "VO_KT_GLUTH6_78", false);
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed > 2)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed++;
					if (this.m_heroPowerLinePlayed == 1)
					{
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX12_01_HP_01", "VO_NAX12_01_HP_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
					else
					{
						yield return new WaitForSeconds(2f);
						NotificationManager.Get().CreateKTQuote("VO_KT_GLUTH5_77", "VO_KT_GLUTH5_77", false);
					}
				}
			}
		}
		yield break;
	}

	// Token: 0x04001797 RID: 6039
	private bool m_cardLinePlayed;

	// Token: 0x04001798 RID: 6040
	private int m_heroPowerLinePlayed;

	// Token: 0x04001799 RID: 6041
	private bool m_achievementTauntPlayed;
}
