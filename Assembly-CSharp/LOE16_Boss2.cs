using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200031E RID: 798
public class LOE16_Boss2 : LOE_MissionEntity
{
	// Token: 0x06002942 RID: 10562 RVA: 0x000C8095 File Offset: 0x000C6295
	public override void StartGameplaySoundtracks()
	{
		MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_LOE_Wing4Mission4);
	}

	// Token: 0x06002943 RID: 10563 RVA: 0x000C80A8 File Offset: 0x000C62A8
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOE_16_TURN_2");
		base.PreloadSound("VO_ELISE_LOE16_ALT_1_FIRST_HALF_02");
		base.PreloadSound("VO_ELISE_LOE16_ALT_1_SECOND_HALF_03");
		base.PreloadSound("VO_LOE_16_TURN_2_2");
		base.PreloadSound("VO_LOE_16_TURN_2_3");
		base.PreloadSound("VO_LOE_16_TURN_3");
		base.PreloadSound("VO_LOE_16_TURN_3_2");
		base.PreloadSound("VO_LOE_16_TURN_4");
		base.PreloadSound("VO_LOE_16_TURN_5");
		base.PreloadSound("VO_LOE_16_TURN_5_2");
		base.PreloadSound("VO_LOE_16_TURN_6");
		base.PreloadSound("VO_LOE_16_FIRST_ITEM");
		base.PreloadSound("VO_LOE_092_Attack_02");
		base.PreloadSound("VO_LOE_16_GOBLET");
		base.PreloadSound("VO_LOE_16_CROWN");
		base.PreloadSound("VO_LOE_16_EYE");
		base.PreloadSound("VO_LOE_16_PIPE");
		base.PreloadSound("VO_LOE_16_TEAR");
		base.PreloadSound("VO_LOE_16_SHARD");
		base.PreloadSound("VO_LOE_16_LOCKET");
		base.PreloadSound("VO_LOE_16_SPLINTER");
		base.PreloadSound("VO_LOE_16_VIAL");
		base.PreloadSound("VO_LOE_16_GREAVE");
		base.PreloadSound("VO_LOE_16_BOOM_BOT");
		base.PreloadSound("VO_LOEA16_1_CARD_04");
		base.PreloadSound("VO_LOEA16_1_RESPONSE_03");
		base.PreloadSound("VO_LOEA16_1_TURN1_02");
	}

	// Token: 0x06002944 RID: 10564 RVA: 0x000C81DE File Offset: 0x000C63DE
	public override void OnTagChanged(TagDelta change)
	{
		base.OnTagChanged(change);
		Gameplay.Get().StartCoroutine(this.OnTagChangedHandler(change));
	}

	// Token: 0x06002945 RID: 10565 RVA: 0x000C81FC File Offset: 0x000C63FC
	public override string UpdateCardText(Card card, Actor bigCardActor, string text)
	{
		Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
		Card heroPowerCard = opposingSidePlayer.GetHeroPowerCard();
		if (heroPowerCard != card)
		{
			return text;
		}
		int num = opposingSidePlayer.GetHeroPower().GetTag(GAME_TAG.ELECTRIC_CHARGE_LEVEL);
		int tag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.TURN);
		if (tag < 2)
		{
			num = 3;
		}
		string key = string.Empty;
		switch (num)
		{
		case 0:
			key = "LOEA16_2_STAFF_TEXT_CHARGE_EXPLODED";
			break;
		case 1:
			key = "LOEA16_2_STAFF_TEXT_CHARGE_1";
			break;
		case 2:
			key = "LOEA16_2_STAFF_TEXT_CHARGE_2";
			break;
		case 3:
			key = "LOEA16_2_STAFF_TEXT_CHARGE_0";
			break;
		}
		return GameStrings.Get(key);
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x000C82B4 File Offset: 0x000C64B4
	private IEnumerator OnTagChangedHandler(TagDelta change)
	{
		if (change.tag != 3)
		{
			yield break;
		}
		int turn = change.newValue;
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		int count = turn;
		if (count > 14)
		{
			count = (count - 14) % 6;
			if (count == 0)
			{
				count = 6;
			}
			count = 8 + count;
		}
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		switch (count)
		{
		case 1:
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA16_1_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Elise_BigQuote", "VO_ELISE_LOE16_ALT_1_FIRST_HALF_02", 3f, 1f, true, false));
			if (GameState.Get().GetOpposingSidePlayer().GetTag(GAME_TAG.ELECTRIC_CHARGE_LEVEL) <= 0)
			{
				GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard().ActivateActorSpell(SpellType.ELECTRIC_CHARGE_LEVEL_SMALL);
			}
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Elise_BigQuote", "VO_ELISE_LOE16_ALT_1_SECOND_HALF_03", 3f, 1f, true, false));
			GameState.Get().SetBusy(false);
			break;
		case 5:
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Elise_BigQuote", "VO_LOE_16_TURN_2", 3f, 1f, true, false));
			break;
		case 6:
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_LOE_16_TURN_2_2", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
			GameState.Get().SetBusy(false);
			break;
		case 7:
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Elise_BigQuote", "VO_LOE_16_TURN_2_3", 3f, 1f, true, false));
			break;
		case 8:
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_16_TURN_3", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Elise_BigQuote", "VO_LOE_16_TURN_3_2", 3f, 1f, true, false));
			break;
		case 11:
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Elise_BigQuote", "VO_LOE_16_TURN_4", 3f, 1f, true, false));
			break;
		case 12:
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOE_16_TURN_5", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
			GameState.Get().SetBusy(false);
			break;
		case 13:
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_16_TURN_5_2", 3f, 1f, false));
			break;
		case 14:
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_16_TURN_6", 3f, 1f, false));
			break;
		}
		yield break;
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x000C82E0 File Offset: 0x000C64E0
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		yield break;
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x000C82F4 File Offset: 0x000C64F4
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		while (GameState.Get().IsBusy())
		{
			yield return null;
		}
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		if (missionEvent >= 10 && !this.m_firstExplorerHelp)
		{
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Elise_BigQuote", "VO_LOE_16_FIRST_ITEM", 3f, 1f, true, false));
			this.m_firstExplorerHelp = true;
		}
		else
		{
			switch (missionEvent)
			{
			case 25393:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_16_EYE", 3f, 1f, false));
				break;
			case 25394:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_16_PIPE", 3f, 1f, false));
				break;
			case 25395:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_16_TEAR", 3f, 1f, false));
				break;
			case 25396:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_16_SHARD", 3f, 1f, false));
				break;
			case 25397:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOE_16_SPLINTER", 3f, 1f, false));
				break;
			case 25398:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_16_VIAL", 3f, 1f, false));
				break;
			case 25399:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOE_16_GREAVE", 3f, 1f, false));
				break;
			case 25400:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_16_GOBLET", 3f, 1f, false));
				break;
			case 25401:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOE_16_CROWN", 3f, 1f, false));
				break;
			case 25402:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOE_16_LOCKET", 3f, 1f, false));
				break;
			default:
				if (missionEvent != 2)
				{
					if (missionEvent == 2235)
					{
						yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_16_BOOM_BOT", 3f, 1f, false));
					}
				}
				else
				{
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_LOE_092_Attack_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
				}
				break;
			}
		}
		yield break;
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x000C8320 File Offset: 0x000C6520
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
			if (LOE16_Boss2.<>f__switch$map66 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("LOEA16_3", 0);
				dictionary.Add("LOEA16_4", 0);
				dictionary.Add("LOEA16_5", 0);
				LOE16_Boss2.<>f__switch$map66 = dictionary;
			}
			int num;
			if (LOE16_Boss2.<>f__switch$map66.TryGetValue(cardId, ref num))
			{
				if (num == 0)
				{
					if (this.m_artifactLinePlayed)
					{
						yield break;
					}
					this.m_artifactLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA16_1_CARD_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x0600294A RID: 10570 RVA: 0x000C834C File Offset: 0x000C654C
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
			m_soundName = "VO_LOEA16_1_RESPONSE_03",
			m_stringTag = "VO_LOEA16_1_RESPONSE_03"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0400182F RID: 6191
	private bool m_artifactLinePlayed;

	// Token: 0x04001830 RID: 6192
	private bool m_firstExplorerHelp;
}
