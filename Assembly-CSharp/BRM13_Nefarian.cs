using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000306 RID: 774
public class BRM13_Nefarian : BRM_MissionEntity
{
	// Token: 0x060028A3 RID: 10403 RVA: 0x000C5AAC File Offset: 0x000C3CAC
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA13_1_RESPONSE_05");
		base.PreloadSound("VO_BRMA13_1_TURN1_PT1_02");
		base.PreloadSound("VO_BRMA13_1_TURN1_PT2_03");
		base.PreloadSound("VO_RAGNAROS_NEF1_71");
		base.PreloadSound("VO_BRMA13_1_HP_PALADIN_07");
		base.PreloadSound("VO_BRMA13_1_HP_PRIEST_08");
		base.PreloadSound("VO_BRMA13_1_HP_WARLOCK_10");
		base.PreloadSound("VO_BRMA13_1_HP_WARRIOR_09");
		base.PreloadSound("VO_BRMA13_1_HP_MAGE_11");
		base.PreloadSound("VO_BRMA13_1_HP_DRUID_14");
		base.PreloadSound("VO_BRMA13_1_HP_SHAMAN_13");
		base.PreloadSound("VO_BRMA13_1_HP_HUNTER_12");
		base.PreloadSound("VO_BRMA13_1_HP_ROGUE_15");
		base.PreloadSound("VO_BRMA13_1_HP_GENERIC_18");
		base.PreloadSound("VO_BRMA13_1_DEATHWING_19");
		base.PreloadSound("VO_NEFARIAN_NEF2_65");
		base.PreloadSound("VO_NEFARIAN_NEF_MISSION_66");
		base.PreloadSound("VO_RAGNAROS_NEF3_72");
		base.PreloadSound("VO_NEFARIAN_HEROIC_BLOCK_77");
		base.PreloadSound("VO_RAGNAROS_NEF4_73");
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000C5B98 File Offset: 0x000C3D98
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
			m_soundName = "VO_BRMA13_1_RESPONSE_05",
			m_stringTag = "VO_BRMA13_1_RESPONSE_05"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x000C5C00 File Offset: 0x000C3E00
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
			if (BRM13_Nefarian.<>f__switch$map5A == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("BRMA13_2", 0);
				dictionary.Add("BRMA13_2H", 0);
				dictionary.Add("BRMA13_4", 1);
				dictionary.Add("BRMA13_4H", 1);
				BRM13_Nefarian.<>f__switch$map5A = dictionary;
			}
			int num;
			if (BRM13_Nefarian.<>f__switch$map5A.TryGetValue(cardId, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						if (this.m_heroPowerLinePlayed)
						{
							yield break;
						}
						this.m_heroPowerLinePlayed = true;
						GameState.Get().SetBusy(true);
						switch (GameState.Get().GetFriendlySidePlayer().GetHero().GetClass())
						{
						case TAG_CLASS.DRUID:
							yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_HP_DRUID_14", "VO_BRMA13_1_HP_DRUID_14", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
							break;
						case TAG_CLASS.HUNTER:
							yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_HP_HUNTER_12", "VO_BRMA13_1_HP_HUNTER_12", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
							break;
						case TAG_CLASS.MAGE:
							yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_HP_MAGE_11", "VO_BRMA13_1_HP_MAGE_11", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
							break;
						case TAG_CLASS.PALADIN:
							yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_HP_PALADIN_07", "VO_BRMA13_1_HP_PALADIN_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
							break;
						case TAG_CLASS.PRIEST:
							yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_HP_PRIEST_08", "VO_BRMA13_1_HP_PRIEST_08", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
							break;
						case TAG_CLASS.ROGUE:
							yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_HP_ROGUE_15", "VO_BRMA13_1_HP_ROGUE_15", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
							break;
						case TAG_CLASS.SHAMAN:
							yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_HP_SHAMAN_13", "VO_BRMA13_1_HP_SHAMAN_13", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
							break;
						case TAG_CLASS.WARLOCK:
							yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_HP_WARLOCK_10", "VO_BRMA13_1_HP_WARLOCK_10", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
							break;
						case TAG_CLASS.WARRIOR:
							yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_HP_WARRIOR_09", "VO_BRMA13_1_HP_WARRIOR_09", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
							break;
						default:
							yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_HP_GENERIC_18", "VO_BRMA13_1_HP_GENERIC_18", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
							break;
						}
						GameState.Get().SetBusy(false);
					}
				}
				else
				{
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_TURN1_PT1_02", "VO_BRMA13_1_TURN1_PT1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, false, false));
					enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_TURN1_PT2_03", "VO_BRMA13_1_TURN1_PT2_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x000C5C2C File Offset: 0x000C3E2C
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (missionEvent)
		{
		case 3:
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA13_1_DEATHWING_19", "VO_BRMA13_1_DEATHWING_19", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			break;
		case 5:
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(4f);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			this.m_ragLine++;
			Gameplay.Get().StartCoroutine(this.UnBusyInSeconds(1f));
			switch (this.m_ragLine)
			{
			case 1:
				NotificationManager.Get().CreateCharacterQuote("Ragnaros_Quote", this.ragLinePosition, GameStrings.Get("VO_RAGNAROS_NEF1_71"), string.Empty, true, 30f, null, CanvasAnchor.BOTTOM_LEFT);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_RAGNAROS_NEF1_71", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
				NotificationManager.Get().DestroyActiveQuote(0f);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NEFARIAN_NEF2_65", "VO_NEFARIAN_NEF2_65", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				break;
			case 2:
				NotificationManager.Get().CreateCharacterQuote("Ragnaros_Quote", this.ragLinePosition, GameStrings.Get("VO_RAGNAROS_NEF3_72"), string.Empty, true, 30f, null, CanvasAnchor.BOTTOM_LEFT);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_RAGNAROS_NEF3_72", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
				NotificationManager.Get().DestroyActiveQuote(0f);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NEFARIAN_NEF_MISSION_66", "VO_NEFARIAN_NEF_MISSION_66", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
				break;
			case 3:
				NotificationManager.Get().CreateCharacterQuote("Ragnaros_Quote", this.ragLinePosition, GameStrings.Get("VO_RAGNAROS_NEF4_73"), "VO_RAGNAROS_NEF4_73", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
				break;
			case 4:
				NotificationManager.Get().CreateCharacterQuote("Ragnaros_Quote", this.ragLinePosition, GameStrings.Get("VO_RAGNAROS_NEF5_74"), "VO_RAGNAROS_NEF5_74", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
				break;
			case 5:
				NotificationManager.Get().CreateCharacterQuote("Ragnaros_Quote", this.ragLinePosition, GameStrings.Get("VO_RAGNAROS_NEF6_75"), "VO_RAGNAROS_NEF6_75", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
				break;
			case 6:
				NotificationManager.Get().CreateCharacterQuote("Ragnaros_Quote", this.ragLinePosition, GameStrings.Get("VO_RAGNAROS_NEF7_76"), "VO_RAGNAROS_NEF7_76", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
				break;
			default:
				NotificationManager.Get().CreateCharacterQuote("Ragnaros_Quote", this.ragLinePosition, GameStrings.Get("VO_RAGNAROS_NEF8_77"), "VO_RAGNAROS_NEF8_77", true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
				this.m_ragLine = 2;
				break;
			}
			break;
		case 6:
			NotificationManager.Get().CreateCharacterQuote("Ragnaros_Quote", this.ragLinePosition, GameStrings.Get("VO_RAGNAROS_NEF4_73"), string.Empty, true, 30f, null, CanvasAnchor.BOTTOM_LEFT);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_RAGNAROS_NEF4_73", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
			NotificationManager.Get().DestroyActiveQuote(0f);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NEFARIAN_HEROIC_BLOCK_77", "VO_NEFARIAN_HEROIC_BLOCK_77", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			break;
		}
		yield break;
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x000C5C58 File Offset: 0x000C3E58
	private IEnumerator UnBusyInSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		GameState.Get().SetBusy(false);
		yield break;
	}

	// Token: 0x040017CC RID: 6092
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017CD RID: 6093
	private int m_ragLine;

	// Token: 0x040017CE RID: 6094
	private Vector3 ragLinePosition = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
}
