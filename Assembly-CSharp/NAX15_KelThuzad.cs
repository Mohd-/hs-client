using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F9 RID: 761
public class NAX15_KelThuzad : NAX_MissionEntity
{
	// Token: 0x0600284E RID: 10318 RVA: 0x000C41F4 File Offset: 0x000C23F4
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_NAX15_01_SUMMON_ADDS_12");
		base.PreloadSound("VO_NAX15_01_PHASE2_10");
		base.PreloadSound("VO_NAX15_01_HP_07");
		base.PreloadSound("VO_NAX15_01_HP2_05");
		base.PreloadSound("VO_NAX15_01_HP3_06");
		base.PreloadSound("VO_NAX15_01_PHASE2_ALT_11");
		base.PreloadSound("VO_NAX15_01_EMOTE_HELLO_26");
		base.PreloadSound("VO_NAX15_01_EMOTE_WP_25");
		base.PreloadSound("VO_NAX15_01_EMOTE_OOPS_29");
		base.PreloadSound("VO_NAX15_01_EMOTE_SORRY_28");
		base.PreloadSound("VO_NAX15_01_EMOTE_THANKS_27");
		base.PreloadSound("VO_NAX15_01_EMOTE_THREATEN_30");
		base.PreloadSound("VO_NAX15_01_RESPOND_GARROSH_15");
		base.PreloadSound("VO_NAX15_01_RESPOND_THRALL_17");
		base.PreloadSound("VO_NAX15_01_RESPOND_VALEERA_18");
		base.PreloadSound("VO_NAX15_01_RESPOND_UTHER_14");
		base.PreloadSound("VO_NAX15_01_RESPOND_REXXAR_19");
		base.PreloadSound("VO_NAX15_01_RESPOND_MALFURION_ALT_21");
		base.PreloadSound("VO_NAX15_01_RESPOND_GULDAN_22");
		base.PreloadSound("VO_NAX15_01_RESPOND_JAINA_23");
		base.PreloadSound("VO_NAX15_01_RESPOND_ANDUIN_24");
		base.PreloadSound("VO_NAX15_01_BIGGLES_32");
		base.PreloadSound("VO_NAX15_01_HURRY_31");
	}

	// Token: 0x0600284F RID: 10319 RVA: 0x000C4300 File Offset: 0x000C2500
	public override void OnPlayThinkEmote()
	{
		if (this.m_hurryLinePlayed)
		{
			return;
		}
		if (this.m_enemySpeaking)
		{
			return;
		}
		Player currentPlayer = GameState.Get().GetCurrentPlayer();
		if (!currentPlayer.IsLocalUser())
		{
			return;
		}
		Card heroCard = currentPlayer.GetHeroCard();
		if (heroCard.HasActiveEmoteSound())
		{
			return;
		}
		this.m_hurryLinePlayed = true;
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_HURRY_31", "VO_NAX15_01_HURRY_31", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
	}

	// Token: 0x06002850 RID: 10320 RVA: 0x000C4390 File Offset: 0x000C2590
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.LOST)
		{
			int KTgloat = Options.Get().GetInt(Option.KELTHUZADTAUNTS);
			yield return new WaitForSeconds(5f);
			switch (KTgloat)
			{
			case 0:
				NotificationManager.Get().CreateKTQuote("VO_NAX15_01_GLOAT1_33", "VO_NAX15_01_GLOAT1_33", true);
				break;
			case 1:
				NotificationManager.Get().CreateKTQuote("VO_NAX15_01_GLOAT2_34", "VO_NAX15_01_GLOAT2_34", true);
				break;
			case 2:
				NotificationManager.Get().CreateKTQuote("VO_NAX15_01_GLOAT3_35", "VO_NAX15_01_GLOAT3_35", true);
				break;
			case 3:
				NotificationManager.Get().CreateKTQuote("VO_NAX15_01_GLOAT4_36", "VO_NAX15_01_GLOAT4_36", true);
				break;
			case 4:
				NotificationManager.Get().CreateKTQuote("VO_NAX15_01_GLOAT5_37", "VO_NAX15_01_GLOAT5_37", true);
				break;
			}
			if (KTgloat >= 4)
			{
				Options.Get().SetInt(Option.KELTHUZADTAUNTS, 0);
			}
			else
			{
				Options.Get().SetInt(Option.KELTHUZADTAUNTS, KTgloat + 1);
			}
		}
		yield break;
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x000C43B4 File Offset: 0x000C25B4
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		yield return Gameplay.Get().StartCoroutine(base.HandleMissionEventWithTiming(missionEvent));
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (missionEvent)
		{
		case 1:
			GameState.Get().SetBusy(true);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			GameState.Get().SetBusy(false);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_SUMMON_ADDS_12", "VO_NAX15_01_SUMMON_ADDS_12", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			break;
		case 2:
			this.m_enemySpeaking = true;
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_PHASE2_10", "VO_NAX15_01_PHASE2_10", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			GameState.Get().SetBusy(false);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_SUMMON_ADDS_12", "VO_NAX15_01_SUMMON_ADDS_12", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			break;
		case 3:
			if (!this.m_frostHeroPowerLinePlayed)
			{
				this.m_frostHeroPowerLinePlayed = true;
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_HP_07", "VO_NAX15_01_HP_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			}
			break;
		case 4:
			if (this.m_numTimesMindControlPlayed == 0)
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_HP2_05", "VO_NAX15_01_HP2_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			}
			else if (this.m_numTimesMindControlPlayed == 1)
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_HP3_06", "VO_NAX15_01_HP3_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			}
			this.m_numTimesMindControlPlayed++;
			break;
		case 5:
			if (!this.m_bigglesLinePlayed)
			{
				this.m_bigglesLinePlayed = true;
				Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_BIGGLES_32", "VO_NAX15_01_BIGGLES_32", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			}
			break;
		}
		yield break;
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x000C43E0 File Offset: 0x000C25E0
	public override void HandleRealTimeMissionEvent(int missionEvent)
	{
		if (missionEvent == 1)
		{
			AssetLoader.Get().LoadGameObject("KelThuzad_StealTurn", new AssetLoader.GameObjectCallback(this.OnStealTurnSpellLoaded), null, false);
		}
	}

	// Token: 0x06002853 RID: 10323 RVA: 0x000C4420 File Offset: 0x000C2620
	private void OnStealTurnSpellLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			if (TurnTimer.Get() != null)
			{
				TurnTimer.Get().OnEndTurnRequested();
			}
			EndTurnButton.Get().OnEndTurnRequested();
			return;
		}
		go.transform.position = EndTurnButton.Get().transform.position;
		Spell component = go.GetComponent<Spell>();
		if (component == null)
		{
			if (TurnTimer.Get() != null)
			{
				TurnTimer.Get().OnEndTurnRequested();
			}
			EndTurnButton.Get().OnEndTurnRequested();
			return;
		}
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		component.ActivateState(SpellStateType.ACTION);
		Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_PHASE2_ALT_11", "VO_NAX15_01_PHASE2_ALT_11", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
	}

	// Token: 0x06002854 RID: 10324 RVA: 0x000C44F0 File Offset: 0x000C26F0
	protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
	{
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (emoteType)
		{
		case EmoteType.GREETINGS:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_HELLO_26", "VO_NAX15_01_EMOTE_HELLO_26", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
			break;
		case EmoteType.WELL_PLAYED:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_WP_25", "VO_NAX15_01_EMOTE_WP_25", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
			break;
		case EmoteType.OOPS:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_OOPS_29", "VO_NAX15_01_EMOTE_OOPS_29", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
			break;
		case EmoteType.THREATEN:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_THREATEN_30", "VO_NAX15_01_EMOTE_THREATEN_30", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
			break;
		case EmoteType.THANKS:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_THANKS_27", "VO_NAX15_01_EMOTE_THANKS_27", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
			break;
		case EmoteType.SORRY:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_EMOTE_SORRY_28", "VO_NAX15_01_EMOTE_SORRY_28", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
			break;
		case EmoteType.START:
		{
			Entity hero = GameState.Get().GetFriendlySidePlayer().GetHero();
			string cardId = hero.GetCardId();
			if (cardId != null)
			{
				if (NAX15_KelThuzad.<>f__switch$map75 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(9);
					dictionary.Add("HERO_01", 0);
					dictionary.Add("HERO_02", 1);
					dictionary.Add("HERO_03", 2);
					dictionary.Add("HERO_04", 3);
					dictionary.Add("HERO_05", 4);
					dictionary.Add("HERO_06", 5);
					dictionary.Add("HERO_07", 6);
					dictionary.Add("HERO_08", 7);
					dictionary.Add("HERO_09", 8);
					NAX15_KelThuzad.<>f__switch$map75 = dictionary;
				}
				int num;
				if (NAX15_KelThuzad.<>f__switch$map75.TryGetValue(cardId, ref num))
				{
					switch (num)
					{
					case 0:
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_GARROSH_15", "VO_NAX15_01_RESPOND_GARROSH_15", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
						break;
					case 1:
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_THRALL_17", "VO_NAX15_01_RESPOND_THRALL_17", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
						break;
					case 2:
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_VALEERA_18", "VO_NAX15_01_RESPOND_VALEERA_18", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
						break;
					case 3:
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_UTHER_14", "VO_NAX15_01_RESPOND_UTHER_14", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
						break;
					case 4:
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_REXXAR_19", "VO_NAX15_01_RESPOND_REXXAR_19", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
						break;
					case 5:
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_MALFURION_ALT_21", "VO_NAX15_01_RESPOND_MALFURION_ALT_21", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
						break;
					case 6:
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_GULDAN_22", "VO_NAX15_01_RESPOND_GULDAN_22", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
						break;
					case 7:
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_JAINA_23", "VO_NAX15_01_RESPOND_JAINA_23", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
						break;
					case 8:
						Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_NAX15_01_RESPOND_ANDUIN_24", "VO_NAX15_01_RESPOND_ANDUIN_24", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
						break;
					}
				}
			}
			break;
		}
		}
	}

	// Token: 0x040017A0 RID: 6048
	private bool m_frostHeroPowerLinePlayed;

	// Token: 0x040017A1 RID: 6049
	private bool m_bigglesLinePlayed;

	// Token: 0x040017A2 RID: 6050
	private bool m_hurryLinePlayed;

	// Token: 0x040017A3 RID: 6051
	private int m_numTimesMindControlPlayed;
}
