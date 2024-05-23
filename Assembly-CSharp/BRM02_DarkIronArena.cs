using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002FB RID: 763
public class BRM02_DarkIronArena : BRM_MissionEntity
{
	// Token: 0x0600285D RID: 10333 RVA: 0x000C4A2C File Offset: 0x000C2C2C
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA02_1_RESPONSE_04");
		base.PreloadSound("VO_BRMA02_1_HERO_POWER_05");
		base.PreloadSound("VO_BRMA02_1_TURN1_02");
		base.PreloadSound("VO_BRMA02_1_TURN1_PT2_03");
		base.PreloadSound("VO_BRMA02_1_ALAKIR_34");
		base.PreloadSound("VO_BRMA02_1_ALEXSTRAZA_32");
		base.PreloadSound("VO_BRMA02_1_BEAST_22");
		base.PreloadSound("VO_BRMA02_1_BOOM_28");
		base.PreloadSound("VO_BRMA02_1_CAIRNE_20");
		base.PreloadSound("VO_BRMA02_1_CHO_07");
		base.PreloadSound("VO_BRMA02_1_DEATHWING_35");
		base.PreloadSound("VO_BRMA02_1_ETC_18");
		base.PreloadSound("VO_BRMA02_1_FEUGEN_15");
		base.PreloadSound("VO_BRMA02_1_FOEREAPER_29");
		base.PreloadSound("VO_BRMA02_1_GEDDON_13");
		base.PreloadSound("VO_BRMA02_1_GELBIN_21");
		base.PreloadSound("VO_BRMA02_1_GRUUL_31");
		base.PreloadSound("VO_BRMA02_1_HOGGER_27");
		base.PreloadSound("VO_BRMA02_1_ILLIDAN_23");
		base.PreloadSound("VO_BRMA02_1_LEVIATHAN_12");
		base.PreloadSound("VO_BRMA02_1_LOATHEB_16");
		base.PreloadSound("VO_BRMA02_1_MAEXXNA_24");
		base.PreloadSound("VO_BRMA02_1_MILLHOUSE_09");
		base.PreloadSound("VO_BRMA02_1_MOGOR_25");
		base.PreloadSound("VO_BRMA02_1_MUKLA_10");
		base.PreloadSound("VO_BRMA02_1_NOZDORMU_36");
		base.PreloadSound("VO_BRMA02_1_ONYXIA_33");
		base.PreloadSound("VO_BRMA02_1_PAGLE_08");
		base.PreloadSound("VO_BRMA02_1_SNEED_30");
		base.PreloadSound("VO_BRMA02_1_STALAGG_14");
		base.PreloadSound("VO_BRMA02_1_SYLVANAS_19");
		base.PreloadSound("VO_BRMA02_1_THALNOS_06");
		base.PreloadSound("VO_BRMA02_1_THAURISSAN_37");
		base.PreloadSound("VO_BRMA02_1_TINKMASTER_11");
		base.PreloadSound("VO_BRMA02_1_TOSHLEY_26");
		base.PreloadSound("VO_BRMA02_1_VOLJIN_17");
		base.PreloadSound("VO_NEFARIAN_GRIMSTONE_DEAD1_30");
		base.PreloadSound("VO_RAGNAROS_GRIMSTONE_DEAD2_66");
	}

	// Token: 0x0600285E RID: 10334 RVA: 0x000C4BDC File Offset: 0x000C2DDC
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
			m_soundName = "VO_BRMA02_1_RESPONSE_04",
			m_stringTag = "VO_BRMA02_1_RESPONSE_04"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x000C4C44 File Offset: 0x000C2E44
	protected override IEnumerator RespondToWillPlayCardWithTiming(string cardId)
	{
		if (this.m_linesPlayed.Contains(cardId))
		{
			yield break;
		}
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		if (cardId == "BRMA02_2" || cardId == "BRMA02_2H")
		{
			if (this.m_enemySpeaking)
			{
				yield break;
			}
			GameState.Get().SetBusy(true);
			this.m_linesPlayed.Add(cardId);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_HERO_POWER_05", "VO_BRMA02_1_HERO_POWER_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
			GameState.Get().SetBusy(false);
		}
		else
		{
			GameState.Get().SetBusy(true);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			if (cardId != null)
			{
				if (BRM02_DarkIronArena.<>f__switch$map50 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(32);
					dictionary.Add("NEW1_010", 0);
					dictionary.Add("EX1_561", 1);
					dictionary.Add("EX1_577", 2);
					dictionary.Add("GVG_110", 3);
					dictionary.Add("EX1_110", 4);
					dictionary.Add("EX1_100", 5);
					dictionary.Add("NEW1_030", 6);
					dictionary.Add("PRO_001", 7);
					dictionary.Add("FP1_015", 8);
					dictionary.Add("GVG_113", 9);
					dictionary.Add("EX1_249", 10);
					dictionary.Add("EX1_112", 11);
					dictionary.Add("NEW1_038", 12);
					dictionary.Add("NEW1_040", 13);
					dictionary.Add("EX1_614", 14);
					dictionary.Add("GVG_007", 15);
					dictionary.Add("FP1_030", 16);
					dictionary.Add("FP1_010", 17);
					dictionary.Add("NEW1_029", 18);
					dictionary.Add("GVG_112", 19);
					dictionary.Add("EX1_014", 20);
					dictionary.Add("EX1_560", 21);
					dictionary.Add("EX1_562", 22);
					dictionary.Add("EX1_557", 23);
					dictionary.Add("GVG_114", 24);
					dictionary.Add("FP1_014", 25);
					dictionary.Add("EX1_016", 26);
					dictionary.Add("EX1_012", 27);
					dictionary.Add("BRM_028", 28);
					dictionary.Add("EX1_083", 29);
					dictionary.Add("GVG_115", 30);
					dictionary.Add("GVG_014", 31);
					BRM02_DarkIronArena.<>f__switch$map50 = dictionary;
				}
				int num;
				if (BRM02_DarkIronArena.<>f__switch$map50.TryGetValue(cardId, ref num))
				{
					switch (num)
					{
					case 0:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_ALAKIR_34", "VO_BRMA02_1_ALAKIR_34", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 1:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_ALEXSTRAZA_32", "VO_BRMA02_1_ALEXSTRAZA_32", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 2:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_BEAST_22", "VO_BRMA02_1_BEAST_22", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 3:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_BOOM_28", "VO_BRMA02_1_BOOM_28", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 4:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_CAIRNE_20", "VO_BRMA02_1_CAIRNE_20", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 5:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_CHO_07", "VO_BRMA02_1_CHO_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 6:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_DEATHWING_35", "VO_BRMA02_1_DEATHWING_35", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 7:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_ETC_18", "VO_BRMA02_1_ETC_18", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 8:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_FEUGEN_15", "VO_BRMA02_1_FEUGEN_15", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 9:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_FOEREAPER_29", "VO_BRMA02_1_FOEREAPER_29", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 10:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_GEDDON_13", "VO_BRMA02_1_GEDDON_13", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 11:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_GELBIN_21", "VO_BRMA02_1_GELBIN_21", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 12:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_GRUUL_31", "VO_BRMA02_1_GRUUL_31", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 13:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_HOGGER_27", "VO_BRMA02_1_HOGGER_27", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 14:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_ILLIDAN_23", "VO_BRMA02_1_ILLIDAN_23", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 15:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_LEVIATHAN_12", "VO_BRMA02_1_LEVIATHAN_12", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 16:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_LOATHEB_16", "VO_BRMA02_1_LOATHEB_16", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 17:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_MAEXXNA_24", "VO_BRMA02_1_MAEXXNA_24", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 18:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_MILLHOUSE_09", "VO_BRMA02_1_MILLHOUSE_09", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 19:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_MOGOR_25", "VO_BRMA02_1_MOGOR_25", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 20:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_MUKLA_10", "VO_BRMA02_1_MUKLA_10", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 21:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_NOZDORMU_36", "VO_BRMA02_1_NOZDORMU_36", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 22:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_ONYXIA_33", "VO_BRMA02_1_ONYXIA_33", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 23:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_PAGLE_08", "VO_BRMA02_1_PAGLE_08", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 24:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_SNEED_30", "VO_BRMA02_1_SNEED_30", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 25:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_STALAGG_14", "VO_BRMA02_1_STALAGG_14", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 26:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_SYLVANAS_19", "VO_BRMA02_1_SYLVANAS_19", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 27:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_THALNOS_06", "VO_BRMA02_1_THALNOS_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 28:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_THAURISSAN_37", "VO_BRMA02_1_THAURISSAN_37", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 29:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_TINKMASTER_11", "VO_BRMA02_1_TINKMASTER_11", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 30:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_TOSHLEY_26", "VO_BRMA02_1_TOSHLEY_26", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					case 31:
						this.m_linesPlayed.Add(cardId);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_VOLJIN_17", "VO_BRMA02_1_VOLJIN_17", Notification.SpeechBubbleDirection.TopRight, enemyActor, 0.7f, true, true));
						break;
					}
				}
			}
			GameState.Get().SetBusy(false);
		}
		yield break;
	}

	// Token: 0x06002860 RID: 10336 RVA: 0x000C4C70 File Offset: 0x000C2E70
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn == 1)
		{
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_TURN1_02", "VO_BRMA02_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA02_1_TURN1_PT2_03", "VO_BRMA02_1_TURN1_PT2_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x06002861 RID: 10337 RVA: 0x000C4C9C File Offset: 0x000C2E9C
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_GRIMSTONE_DEAD1_30"), string.Empty, true, 5f, CanvasAnchor.BOTTOM_LEFT);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_NEFARIAN_GRIMSTONE_DEAD1_30", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
			NotificationManager.Get().DestroyActiveQuote(0f);
			NotificationManager.Get().CreateCharacterQuote("Ragnaros_Quote", NotificationManager.ALT_ADVENTURE_SCREEN_POS, GameStrings.Get("VO_RAGNAROS_GRIMSTONE_DEAD2_66"), string.Empty, true, 7f, null, CanvasAnchor.BOTTOM_LEFT);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_RAGNAROS_GRIMSTONE_DEAD2_66", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
			NotificationManager.Get().DestroyActiveQuote(0f);
		}
		yield break;
	}

	// Token: 0x040017AC RID: 6060
	private const float PLAY_CARD_DELAY = 0.7f;

	// Token: 0x040017AD RID: 6061
	private HashSet<string> m_linesPlayed = new HashSet<string>();
}
