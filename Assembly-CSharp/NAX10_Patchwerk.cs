using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F4 RID: 756
public class NAX10_Patchwerk : NAX_MissionEntity
{
	// Token: 0x06002831 RID: 10289 RVA: 0x000C3B71 File Offset: 0x000C1D71
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX10_01_HP_02");
		base.PreloadSound("VO_NAX10_01_EMOTE2_05");
		base.PreloadSound("VO_NAX10_01_EMOTE1_04");
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x000C3B94 File Offset: 0x000C1D94
	protected override void InitEmoteResponses()
	{
		List<MissionEntity.EmoteResponseGroup> list = new List<MissionEntity.EmoteResponseGroup>();
		List<MissionEntity.EmoteResponseGroup> list2 = list;
		MissionEntity.EmoteResponseGroup emoteResponseGroup = new MissionEntity.EmoteResponseGroup();
		MissionEntity.EmoteResponseGroup emoteResponseGroup2 = emoteResponseGroup;
		List<EmoteType> list3 = new List<EmoteType>();
		list3.Add(EmoteType.GREETINGS);
		list3.Add(EmoteType.OOPS);
		list3.Add(EmoteType.SORRY);
		list3.Add(EmoteType.THANKS);
		list3.Add(EmoteType.THREATEN);
		emoteResponseGroup2.m_triggers = list3;
		MissionEntity.EmoteResponseGroup emoteResponseGroup3 = emoteResponseGroup;
		List<MissionEntity.EmoteResponse> list4 = new List<MissionEntity.EmoteResponse>();
		list4.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX10_01_EMOTE1_04",
			m_stringTag = "VO_NAX10_01_EMOTE1_04"
		});
		emoteResponseGroup3.m_responses = list4;
		list2.Add(emoteResponseGroup);
		List<MissionEntity.EmoteResponseGroup> list5 = list;
		emoteResponseGroup = new MissionEntity.EmoteResponseGroup();
		MissionEntity.EmoteResponseGroup emoteResponseGroup4 = emoteResponseGroup;
		list3 = new List<EmoteType>();
		list3.Add(EmoteType.WELL_PLAYED);
		emoteResponseGroup4.m_triggers = list3;
		MissionEntity.EmoteResponseGroup emoteResponseGroup5 = emoteResponseGroup;
		list4 = new List<MissionEntity.EmoteResponse>();
		list4.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_NAX10_01_EMOTE2_05",
			m_stringTag = "VO_NAX10_01_EMOTE2_05"
		});
		emoteResponseGroup5.m_responses = list4;
		list5.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x000C3C74 File Offset: 0x000C1E74
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_PATCHWERK2_69", "VO_KT_PATCHWERK2_69", true);
		}
		yield break;
	}

	// Token: 0x06002834 RID: 10292 RVA: 0x000C3C98 File Offset: 0x000C1E98
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
			if (NAX10_Patchwerk.<>f__switch$map70 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("NAX10_03", 0);
				NAX10_Patchwerk.<>f__switch$map70 = dictionary;
			}
			int num;
			if (NAX10_Patchwerk.<>f__switch$map70.TryGetValue(cardId, ref num))
			{
				if (num == 0)
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					yield return new WaitForSeconds(4.5f);
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX10_01_HP_02", "VO_NAX10_01_HP_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002835 RID: 10293 RVA: 0x000C3CC4 File Offset: 0x000C1EC4
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		if (turn % 2 != 0)
		{
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(1f);
			GameState.Get().SetBusy(false);
		}
		yield break;
	}

	// Token: 0x04001792 RID: 6034
	private bool m_heroPowerLinePlayed;
}
