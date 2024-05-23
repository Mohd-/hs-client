using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200030A RID: 778
public class BRM17_ZombieNef : BRM_MissionEntity
{
	// Token: 0x060028BE RID: 10430 RVA: 0x000C61B4 File Offset: 0x000C43B4
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA17_1_DEATHWING_88");
		base.PreloadSound("VO_BRMA17_1_HERO_POWER_87");
		base.PreloadSound("VO_BRMA17_1_CARD_86");
		base.PreloadSound("VO_BRMA17_1_RESPONSE_85");
		base.PreloadSound("VO_BRMA17_1_TURN1_79");
		base.PreloadSound("VO_BRMA17_1_RESURRECT1_82");
		base.PreloadSound("VO_BRMA17_1_RESURRECT3_84");
		base.PreloadSound("VO_BRMA17_1_NEF_AIR1_89");
		base.PreloadSound("VO_BRMA17_1_NEF_AIR2_90");
		base.PreloadSound("VO_BRMA17_1_NEF_AIR3_91");
		base.PreloadSound("VO_BRMA17_1_NEF_AIR4_92");
		base.PreloadSound("VO_BRMA17_1_NEF_AIR5_93");
		base.PreloadSound("VO_BRMA17_1_NEF_AIR6_94");
		base.PreloadSound("VO_BRMA17_1_NEF_AIR7_95");
		base.PreloadSound("VO_BRMA17_1_NEF_AIR8_96");
		base.PreloadSound("VO_BRMA17_1_TRANSFORM1_80");
		base.PreloadSound("VO_BRMA17_1_TRANSFORM2_81");
		base.PreloadSound("OnyxiaBoss_Start_1");
		base.PreloadSound("OnyxiaBoss_Death_1");
		base.PreloadSound("OnyxiaBoss_EmoteResponse_1");
	}

	// Token: 0x060028BF RID: 10431 RVA: 0x000C62A0 File Offset: 0x000C44A0
	protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
	{
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (emoteType)
		{
		case EmoteType.GREETINGS:
		case EmoteType.WELL_PLAYED:
		case EmoteType.OOPS:
		case EmoteType.THREATEN:
		case EmoteType.THANKS:
		case EmoteType.SORRY:
		{
			string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
			if (cardId == "BRMA17_2" || cardId == "BRMA17_2H")
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_RESPONSE_85", "VO_BRMA17_1_RESPONSE_85", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
			}
			else if (cardId == "BRMA17_3" || cardId == "BRMA17_3H")
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("OnyxiaBoss_EmoteResponse_1", "OnyxiaBoss_EmoteResponse_1", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
			}
			break;
		}
		}
	}

	// Token: 0x060028C0 RID: 10432 RVA: 0x000C6390 File Offset: 0x000C4590
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
			if (BRM17_ZombieNef.<>f__switch$map5E == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("BRMA17_4", 0);
				dictionary.Add("BRMA17_5", 1);
				dictionary.Add("BRMA17_5H", 1);
				BRM17_ZombieNef.<>f__switch$map5E = dictionary;
			}
			int num;
			if (BRM17_ZombieNef.<>f__switch$map5E.TryGetValue(cardId, ref num))
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
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_HERO_POWER_87", "VO_BRMA17_1_HERO_POWER_87", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					}
				}
				else
				{
					if (this.m_cardLinePlayed)
					{
						yield break;
					}
					if (this.m_inOnyxiaState)
					{
						yield break;
					}
					this.m_cardLinePlayed = true;
					GameState.Get().SetBusy(true);
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_CARD_86", "VO_BRMA17_1_CARD_86", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
					GameState.Get().SetBusy(false);
				}
			}
		}
		yield break;
	}

	// Token: 0x060028C1 RID: 10433 RVA: 0x000C63BC File Offset: 0x000C45BC
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn == 1)
		{
			this.m_nefActor = enemyActor;
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_TURN1_79", "VO_BRMA17_1_TURN1_79", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x060028C2 RID: 10434 RVA: 0x000C63E8 File Offset: 0x000C45E8
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (missionEvent)
		{
		case 1:
			this.m_inOnyxiaState = true;
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_RESURRECT1_82", "VO_BRMA17_1_RESURRECT1_82", Notification.SpeechBubbleDirection.TopRight, this.m_nefActor, 1f, true, false));
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_RESURRECT3_84", "VO_BRMA17_1_RESURRECT3_84", Notification.SpeechBubbleDirection.TopRight, this.m_nefActor, 1f, true, false));
			enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("OnyxiaBoss_Start_1", "OnyxiaBoss_Start_1", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			GameState.Get().SetBusy(false);
			break;
		case 3:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_DEATHWING_88", "VO_BRMA17_1_DEATHWING_88", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			break;
		case 4:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR1_89", "VO_BRMA17_1_NEF_AIR1_89", Notification.SpeechBubbleDirection.TopRight, this.m_nefActor, 1f, true, false));
			break;
		case 5:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR2_90", "VO_BRMA17_1_NEF_AIR2_90", Notification.SpeechBubbleDirection.TopRight, this.m_nefActor, 1f, true, false));
			break;
		case 6:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR3_91", "VO_BRMA17_1_NEF_AIR3_91", Notification.SpeechBubbleDirection.TopRight, this.m_nefActor, 1f, true, false));
			break;
		case 7:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR4_92", "VO_BRMA17_1_NEF_AIR4_92", Notification.SpeechBubbleDirection.TopRight, this.m_nefActor, 1f, true, false));
			break;
		case 8:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR5_93", "VO_BRMA17_1_NEF_AIR5_93", Notification.SpeechBubbleDirection.TopRight, this.m_nefActor, 1f, true, false));
			break;
		case 9:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR6_94", "VO_BRMA17_1_NEF_AIR6_94", Notification.SpeechBubbleDirection.TopRight, this.m_nefActor, 1f, true, false));
			break;
		case 10:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR7_95", "VO_BRMA17_1_NEF_AIR7_95", Notification.SpeechBubbleDirection.TopRight, this.m_nefActor, 1f, true, false));
			break;
		case 11:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_NEF_AIR8_96", "VO_BRMA17_1_NEF_AIR8_96", Notification.SpeechBubbleDirection.TopRight, this.m_nefActor, 1f, true, false));
			break;
		case 13:
			this.m_inOnyxiaState = false;
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_TRANSFORM1_80", "VO_BRMA17_1_TRANSFORM1_80", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			GameState.Get().SetBusy(false);
			break;
		case 14:
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA17_1_TRANSFORM2_81", "VO_BRMA17_1_TRANSFORM2_81", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			break;
		}
		yield break;
	}

	// Token: 0x040017DF RID: 6111
	private bool m_heroPowerLinePlayed;

	// Token: 0x040017E0 RID: 6112
	private bool m_cardLinePlayed;

	// Token: 0x040017E1 RID: 6113
	private bool m_inOnyxiaState;

	// Token: 0x040017E2 RID: 6114
	private Actor m_nefActor;
}
