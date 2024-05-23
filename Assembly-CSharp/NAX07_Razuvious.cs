using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F1 RID: 753
public class NAX07_Razuvious : NAX_MissionEntity
{
	// Token: 0x0600281F RID: 10271 RVA: 0x000C373D File Offset: 0x000C193D
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX7_01_HP_02");
		base.PreloadSound("VO_NAX7_01_START_01");
		base.PreloadSound("VO_NAX7_01_EMOTE_05");
	}

	// Token: 0x06002820 RID: 10272 RVA: 0x000C3760 File Offset: 0x000C1960
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
			m_soundName = "VO_NAX7_01_EMOTE_05",
			m_stringTag = "VO_NAX7_01_EMOTE_05"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x06002821 RID: 10273 RVA: 0x000C37C8 File Offset: 0x000C19C8
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateKTQuote("VO_KT_RAZUVIOUS2_59", "VO_KT_RAZUVIOUS2_59", true);
		}
		yield break;
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x000C37EC File Offset: 0x000C19EC
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
			if (NAX07_Razuvious.<>f__switch$map6D == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("NAX7_03", 0);
				NAX07_Razuvious.<>f__switch$map6D = dictionary;
			}
			int num;
			if (NAX07_Razuvious.<>f__switch$map6D.TryGetValue(cardId, ref num))
			{
				if (num == 0)
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX7_01_HP_02", "VO_NAX7_01_HP_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002823 RID: 10275 RVA: 0x000C3818 File Offset: 0x000C1A18
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		yield return Gameplay.Get().StartCoroutine(base.HandleMissionEventWithTiming(missionEvent));
		if (missionEvent == 1)
		{
			bool understudiesAreInPlay = false;
			PowerTaskList taskList = GameState.Get().GetPowerProcessor().GetCurrentTaskList();
			Entity sourceEntity = (taskList != null) ? taskList.GetSourceEntity() : null;
			if (sourceEntity != null && sourceEntity.GetCardId() == "NAX7_05")
			{
				foreach (PowerTask task in taskList.GetTaskList())
				{
					Network.PowerHistory power = task.GetPower();
					if (power.Type == Network.PowerType.META_DATA)
					{
						Network.HistMetaData metaData = power as Network.HistMetaData;
						if (metaData.MetaType == null)
						{
							if (metaData.Info != null && metaData.Info.Count != 0)
							{
								for (int i = 0; i < metaData.Info.Count; i++)
								{
									Entity targetEntity = GameState.Get().GetEntity(metaData.Info[i]);
									if (targetEntity != null && targetEntity.GetCardId() == "NAX7_02")
									{
										understudiesAreInPlay = true;
										break;
									}
								}
								if (understudiesAreInPlay)
								{
									break;
								}
							}
						}
					}
				}
			}
			if (understudiesAreInPlay)
			{
				Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
				Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX7_01_START_01", "VO_NAX7_01_START_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			}
		}
		yield break;
	}

	// Token: 0x04001788 RID: 6024
	private bool m_heroPowerLinePlayed;
}
