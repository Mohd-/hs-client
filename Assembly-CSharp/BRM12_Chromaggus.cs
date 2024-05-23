using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000305 RID: 773
public class BRM12_Chromaggus : BRM_MissionEntity
{
	// Token: 0x0600289D RID: 10397 RVA: 0x000C5950 File Offset: 0x000C3B50
	public override void PreloadAssets()
	{
		base.PreloadSound("ChromaggusBoss_EmoteResponse_1");
		base.PreloadSound("VO_NEFARIAN_CHROMAGGUS_DEAD_63");
		base.PreloadSound("VO_NEFARIAN_CHROMAGGUS1_59");
		base.PreloadSound("VO_NEFARIAN_CHROMAGGUS2_60");
		base.PreloadSound("VO_NEFARIAN_CHROMAGGUS3_61");
		base.PreloadSound("VO_NEFARIAN_CHROMAGGUS4_62");
	}

	// Token: 0x0600289E RID: 10398 RVA: 0x000C59A0 File Offset: 0x000C3BA0
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
			m_soundName = "ChromaggusBoss_EmoteResponse_1",
			m_stringTag = "ChromaggusBoss_EmoteResponse_1"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600289F RID: 10399 RVA: 0x000C5A08 File Offset: 0x000C3C08
	protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
	{
		Vector3 quotePos = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
		{
			yield return null;
		}
		string cardId = entity.GetCardId();
		if (cardId != null)
		{
			if (BRM12_Chromaggus.<>f__switch$map59 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("BRMA12_2", 0);
				dictionary.Add("BRMA12_2H", 0);
				dictionary.Add("BRMA12_8", 1);
				BRM12_Chromaggus.<>f__switch$map59 = dictionary;
			}
			int num;
			if (BRM12_Chromaggus.<>f__switch$map59.TryGetValue(cardId, ref num))
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
						NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_CHROMAGGUS3_61"), "VO_NEFARIAN_CHROMAGGUS3_61", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
					}
				}
				else
				{
					if (this.m_heroPowerLinePlayed)
					{
						yield break;
					}
					this.m_heroPowerLinePlayed = true;
					NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_CHROMAGGUS4_62"), "VO_NEFARIAN_CHROMAGGUS4_62", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
				}
			}
		}
		yield break;
	}

	// Token: 0x060028A0 RID: 10400 RVA: 0x000C5A34 File Offset: 0x000C3C34
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Vector3 quotePos = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
		if (turn != 2)
		{
			if (turn == 6)
			{
				NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_CHROMAGGUS2_60"), "VO_NEFARIAN_CHROMAGGUS2_60", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
			}
		}
		else
		{
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", quotePos, GameStrings.Get("VO_NEFARIAN_CHROMAGGUS1_59"), "VO_NEFARIAN_CHROMAGGUS1_59", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x000C5A58 File Offset: 0x000C3C58
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_CHROMAGGUS_DEAD_63"), "VO_NEFARIAN_CHROMAGGUS_DEAD_63", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x040017C9 RID: 6089
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017CA RID: 6090
	private bool m_cardLinePlayed;
}
