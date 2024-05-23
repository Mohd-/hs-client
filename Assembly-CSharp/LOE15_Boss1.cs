using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200031D RID: 797
public class LOE15_Boss1 : LOE_MissionEntity
{
	// Token: 0x0600293A RID: 10554 RVA: 0x000C7DD4 File Offset: 0x000C5FD4
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOE_15_RESPONSE");
		base.PreloadSound("VO_LOEA15_1_LOW_HEALTH_10");
		base.PreloadSound("VO_LOEA15_1_TURN1_08");
		base.PreloadSound("VO_LOEA15_1_MAGMA_RAGER_09");
		base.PreloadSound("VO_LOEA15_1_LOSS_11");
		base.PreloadSound("VO_LOEA15_1_WIN_12");
		base.PreloadSound("VO_LOEA15_GOLDEN");
		base.PreloadSound("VO_LOEA15_1_START_07");
		base.PreloadSound("VO_LOE_15_SPARE");
		base.PreloadSound("VO_ELISE_WEIRD_DECK_05");
	}

	// Token: 0x0600293B RID: 10555 RVA: 0x000C7E50 File Offset: 0x000C6050
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
			m_soundName = "VO_LOE_15_RESPONSE",
			m_stringTag = "VO_LOE_15_RESPONSE"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x000C7EB8 File Offset: 0x000C60B8
	public override bool DoAlternateMulliganIntro()
	{
		ZoneDeck deckZone = GameState.Get().GetOpposingSidePlayer().GetDeckZone();
		deckZone.SetVisibility(false);
		this.m_zonesToHide.Clear();
		this.m_zonesToHide.AddRange(ZoneMgr.Get().FindZonesForTag(TAG_ZONE.HAND));
		this.m_zonesToHide.AddRange(ZoneMgr.Get().FindZonesForTag(TAG_ZONE.DECK));
		foreach (Zone zone in this.m_zonesToHide)
		{
			Log.JMac.Print(string.Concat(new object[]
			{
				"Number of cards in zone ",
				zone,
				": ",
				zone.GetCards().Count
			}), new object[0]);
			foreach (Card card in zone.GetCards())
			{
				card.HideCard();
				card.SetDoNotSort(true);
			}
		}
		return false;
	}

	// Token: 0x0600293D RID: 10557 RVA: 0x000C7FF0 File Offset: 0x000C61F0
	public override IEnumerator DoActionsAfterIntroBeforeMulligan()
	{
		Log.JMac.PrintWarning("Start DoPostIntroActions in LOE15_Boss1!", new object[0]);
		Player enemyPlayer = GameState.Get().GetOpposingSidePlayer();
		Actor enemyActor = enemyPlayer.GetHeroCard().GetActor();
		GameObject go = AssetLoader.Get().LoadGameObject("LOE_DeckTakeEvent", true, false);
		LOE_DeckTakeEvent deckTakeEvent = go.GetComponent<LOE_DeckTakeEvent>();
		yield return new WaitForEndOfFrame();
		ZoneDeck opponentZoneDeck = GameState.Get().GetOpposingSidePlayer().GetDeckZone();
		opponentZoneDeck.SetVisibility(true);
		Gameplay.Get().SwapCardBacks();
		opponentZoneDeck.SetVisibility(false);
		ZoneDeck zoneDeck = GameState.Get().GetFriendlySidePlayer().GetDeckZone();
		zoneDeck.SetVisibility(false);
		Gameplay.Get().StartCoroutine(deckTakeEvent.PlayTakeDeckAnim());
		yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_LOEA15_1_START_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
		while (deckTakeEvent.AnimIsPlaying())
		{
			Log.JMac.Print("Waiting for take deck anim to finish.", new object[0]);
			yield return null;
		}
		Gameplay.Get().StartCoroutine(deckTakeEvent.PlayReplacementDeckAnim());
		yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Elise_BigQuote", "VO_LOE_15_SPARE", 3f, 1f, true, false));
		while (deckTakeEvent.AnimIsPlaying())
		{
			Log.JMac.Print("Waiting for replacement deck anim to finish.", new object[0]);
			yield return null;
		}
		foreach (Zone zone in this.m_zonesToHide)
		{
			foreach (Card card in zone.GetCards())
			{
				card.ShowCard();
				card.SetDoNotSort(false);
			}
		}
		this.m_zonesToHide.Clear();
		Log.JMac.PrintWarning("End DoPostIntroActions in LOE15_Boss1!", new object[0]);
		yield break;
	}

	// Token: 0x0600293E RID: 10558 RVA: 0x000C800C File Offset: 0x000C620C
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		Player enemyPlayer = GameState.Get().GetOpposingSidePlayer();
		Actor enemyActor = enemyPlayer.GetHeroCard().GetActor();
		if (!this.m_lowHealth && enemyPlayer.GetHero().GetRemainingHealth() < 10 && GameState.Get().GetFriendlySidePlayer().GetHero().GetRemainingHealth() > 19)
		{
			this.m_lowHealth = true;
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA15_1_LOW_HEALTH_10", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
			yield break;
		}
		if (turn != 1)
		{
			if (turn == 5)
			{
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_ELISE_WEIRD_DECK_05", 3f, 1f, false));
			}
		}
		else if (GameState.Get().GetGameEntity().GetCost() == 1)
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA15_GOLDEN", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
		}
		else
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA15_1_TURN1_08", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x0600293F RID: 10559 RVA: 0x000C8038 File Offset: 0x000C6238
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
			if (LOE15_Boss1.<>f__switch$map65 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("CS2_118", 0);
				LOE15_Boss1.<>f__switch$map65 = dictionary;
			}
			int num;
			if (LOE15_Boss1.<>f__switch$map65.TryGetValue(cardId, ref num))
			{
				if (num == 0)
				{
					if (this.m_magmaRagerLinePlayed)
					{
						yield break;
					}
					this.m_magmaRagerLinePlayed = true;
					Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA15_1_MAGMA_RAGER_09", Notification.SpeechBubbleDirection.TopRight, enemyActor, 3f, 1f, true, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002940 RID: 10560 RVA: 0x000C8064 File Offset: 0x000C6264
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (GameMgr.Get().IsClassChallengeMission())
		{
			yield break;
		}
		if (gameResult == TAG_PLAYSTATE.WON)
		{
			yield return new WaitForSeconds(5f);
			Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Rafaam_wrap_Quote", "VO_LOEA15_1_LOSS_11", "VO_LOEA15_1_LOSS_11", 0f, false, false));
			yield break;
		}
		if (gameResult == TAG_PLAYSTATE.LOST)
		{
			yield return new WaitForSeconds(5f);
			Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Rafaam_wrap_Quote", "VO_LOEA15_1_WIN_12", "VO_LOEA15_1_WIN_12", 0f, false, false));
			yield break;
		}
		yield break;
	}

	// Token: 0x0400182B RID: 6187
	private bool m_magmaRagerLinePlayed;

	// Token: 0x0400182C RID: 6188
	private bool m_lowHealth;

	// Token: 0x0400182D RID: 6189
	private List<Zone> m_zonesToHide = new List<Zone>();
}
