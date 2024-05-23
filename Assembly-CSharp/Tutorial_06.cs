using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002EA RID: 746
public class Tutorial_06 : TutorialEntity
{
	// Token: 0x060027F2 RID: 10226 RVA: 0x000C2CD0 File Offset: 0x000C0ED0
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_TUTORIAL_06_CHO_15_15");
		base.PreloadSound("VO_TUTORIAL_06_CHO_09_13");
		base.PreloadSound("VO_TUTORIAL_06_CHO_17_16");
		base.PreloadSound("VO_TUTORIAL_06_CHO_05_09");
		base.PreloadSound("VO_TUTORIAL_06_JAINA_03_51");
		base.PreloadSound("VO_TUTORIAL_06_CHO_06_10");
		base.PreloadSound("VO_TUTORIAL_06_CHO_21_18");
		base.PreloadSound("VO_TUTORIAL_06_CHO_20_17");
		base.PreloadSound("VO_TUTORIAL_06_CHO_07_11");
		base.PreloadSound("VO_TUTORIAL_06_JAINA_04_52");
		base.PreloadSound("VO_TUTORIAL_06_CHO_04_08");
		base.PreloadSound("VO_TUTORIAL_06_CHO_12_14");
		base.PreloadSound("VO_TUTORIAL_06_CHO_01_05");
		base.PreloadSound("VO_TUTORIAL_06_JAINA_01_49");
		base.PreloadSound("VO_TUTORIAL_06_CHO_02_06");
		base.PreloadSound("VO_TUTORIAL_06_JAINA_02_50");
		base.PreloadSound("VO_TUTORIAL_06_CHO_03_07");
		base.PreloadSound("VO_TUTORIAL_06_CHO_22_19");
		base.PreloadSound("VO_TUTORIAL_06_JAINA_05_53");
	}

	// Token: 0x060027F3 RID: 10227 RVA: 0x000C2DB0 File Offset: 0x000C0FB0
	public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
	{
		base.NotifyOfGameOver(gameResult);
		if (gameResult == TAG_PLAYSTATE.WON)
		{
			this.victory = true;
			base.SetTutorialProgress(TutorialProgress.CHO_COMPLETE);
			base.PlaySound("VO_TUTORIAL_06_CHO_22_19", 1f, true, false);
		}
		else if (gameResult == TAG_PLAYSTATE.TIED)
		{
			base.PlaySound("VO_TUTORIAL_06_CHO_22_19", 1f, true, false);
		}
		else if (gameResult == TAG_PLAYSTATE.LOST)
		{
			base.SetTutorialLostProgress(TutorialProgress.CHO_COMPLETE);
		}
	}

	// Token: 0x060027F4 RID: 10228 RVA: 0x000C2E20 File Offset: 0x000C1020
	protected override Spell BlowUpHero(Card card, SpellType spellType)
	{
		if (card.GetEntity().GetCardId() != "TU4f_001")
		{
			return base.BlowUpHero(card, spellType);
		}
		Spell result = card.ActivateActorSpell(SpellType.CHODEATH);
		Gameplay.Get().StartCoroutine(base.HideOtherElements(card));
		return result;
	}

	// Token: 0x060027F5 RID: 10229 RVA: 0x000C2E6C File Offset: 0x000C106C
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		switch (turn)
		{
		case 2:
			if (!base.DidLoseTutorial(TutorialProgress.CHO_COMPLETE))
			{
				GameState.Get().SetBusy(true);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_15_15", "TUTORIAL06_CHO_15", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
				GameState.Get().SetBusy(false);
			}
			break;
		default:
			switch (turn)
			{
			case 14:
				if (!base.DidLoseTutorial(TutorialProgress.CHO_COMPLETE))
				{
					while (this.m_choSpeaking)
					{
						yield return null;
					}
					this.m_choSpeaking = true;
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_21_18", "TUTORIAL06_CHO_21", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					this.m_choSpeaking = false;
				}
				break;
			case 15:
				if (!base.DidLoseTutorial(TutorialProgress.CHO_COMPLETE))
				{
					while (this.m_choSpeaking)
					{
						yield return null;
					}
					yield return new WaitForSeconds(0.5f);
					this.m_choSpeaking = true;
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_05_09", "TUTORIAL06_CHO_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					this.m_choSpeaking = false;
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_JAINA_03_51", "TUTORIAL06_JAINA_03", Notification.SpeechBubbleDirection.BottomRight, jainaActor, 1f, true, false, 3f));
					this.m_choSpeaking = true;
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_06_10", "TUTORIAL06_CHO_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					this.m_choSpeaking = false;
				}
				break;
			case 16:
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_20_17", "TUTORIAL06_CHO_20", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
				break;
			}
			break;
		case 4:
			if (!base.DidLoseTutorial(TutorialProgress.CHO_COMPLETE))
			{
				GameState.Get().SetBusy(true);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_09_13", "TUTORIAL06_CHO_09", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
				GameState.Get().SetBusy(false);
			}
			break;
		}
		yield break;
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x000C2E98 File Offset: 0x000C1098
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		switch (missionEvent)
		{
		case 1:
			base.HandleGameStartEvent();
			break;
		case 2:
			GameState.Get().SetBusy(true);
			while (this.m_choSpeaking)
			{
				yield return null;
			}
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_17_16", "TUTORIAL06_CHO_17", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			break;
		default:
			if (missionEvent != 54)
			{
				if (missionEvent == 55)
				{
					MulliganManager.Get().BeginMulligan();
					base.FadeInHeroActor(enemyActor);
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_01_05", "TUTORIAL06_CHO_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					base.FadeOutHeroActor(enemyActor);
					yield return Gameplay.Get().StartCoroutine(this.Wait(0.5f));
					base.FadeInHeroActor(jainaActor);
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_JAINA_01_49", "TUTORIAL06_JAINA_01", Notification.SpeechBubbleDirection.BottomRight, jainaActor, 1f, true, false, 3f));
					base.FadeOutHeroActor(jainaActor);
					yield return Gameplay.Get().StartCoroutine(this.Wait(0.5f));
					base.FadeInHeroActor(enemyActor);
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_02_06", "TUTORIAL06_CHO_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					base.FadeOutHeroActor(enemyActor);
					yield return Gameplay.Get().StartCoroutine(this.Wait(0.25f));
					base.FadeInHeroActor(jainaActor);
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_JAINA_02_50", "TUTORIAL06_JAINA_02", Notification.SpeechBubbleDirection.BottomRight, jainaActor, 1f, true, false, 3f));
					base.FadeOutHeroActor(jainaActor);
					yield return Gameplay.Get().StartCoroutine(this.Wait(0.25f));
					Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_03_07", "TUTORIAL06_CHO_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
				}
			}
			else
			{
				yield return new WaitForSeconds(2f);
				string bodyText = base.DidLoseTutorial(TutorialProgress.CHO_COMPLETE) ? "TUTORIAL06_HELP_04" : "TUTORIAL06_HELP_02";
				base.ShowTutorialDialog("TUTORIAL06_HELP_01", bodyText, "TUTORIAL01_HELP_16", new Vector2(0f, 0.5f), false);
			}
			break;
		case 6:
		{
			GameState.Get().SetBusy(true);
			Card enemyHero = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard();
			Spell enemyAttackSpell = enemyHero.GetActorSpell(SpellType.CHOFLOAT, true);
			enemyAttackSpell.ActivateState(SpellStateType.BIRTH);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_07_11", "TUTORIAL06_CHO_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_JAINA_04_52", "TUTORIAL06_JAINA_04", Notification.SpeechBubbleDirection.BottomRight, jainaActor, 1f, true, false, 3f));
			break;
		}
		case 8:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_04_08", "TUTORIAL06_CHO_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			break;
		case 9:
		{
			Card enemyHero2 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard();
			Spell enemyAttackSpell2 = enemyHero2.GetActorSpell(SpellType.CHOFLOAT, true);
			enemyAttackSpell2.ActivateState(SpellStateType.CANCEL);
			this.m_choSpeaking = true;
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_06_CHO_12_14", "TUTORIAL06_CHO_12", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			this.m_choSpeaking = false;
			break;
		}
		case 10:
		{
			List<Card> cardsInEnemyField = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards();
			if (cardsInEnemyField.Count == 0)
			{
				yield break;
			}
			GameState.Get().SetBusy(true);
			Card voodooDoctor = cardsInEnemyField[cardsInEnemyField.Count - 1];
			Vector3 voodooDoctorLocation = voodooDoctor.transform.position;
			Notification battlecryNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, new Vector3(voodooDoctorLocation.x + 3f, voodooDoctorLocation.y, voodooDoctorLocation.z), TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL06_HELP_03"), true);
			battlecryNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
			NotificationManager.Get().DestroyNotification(battlecryNotification, 5f);
			yield return new WaitForSeconds(5f);
			GameState.Get().SetBusy(false);
			break;
		}
		}
		yield break;
	}

	// Token: 0x060027F7 RID: 10231 RVA: 0x000C2EC4 File Offset: 0x000C10C4
	private IEnumerator Wait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		yield break;
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x000C2EE6 File Offset: 0x000C10E6
	public override float GetAdditionalTimeToWaitForSpells()
	{
		return 1.5f;
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x000C2EED File Offset: 0x000C10ED
	public override bool IsKeywordHelpDelayOverridden()
	{
		return true;
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x000C2EF0 File Offset: 0x000C10F0
	public override bool NotifyOfEndTurnButtonPushed()
	{
		Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
		if (optionsPacket == null || optionsPacket.List == null)
		{
			return true;
		}
		if (optionsPacket.List.Count == 1)
		{
			return true;
		}
		for (int i = 0; i < optionsPacket.List.Count; i++)
		{
			Network.Options.Option option = optionsPacket.List[i];
			if (option.Type == Network.Options.Option.OptionType.POWER)
			{
				if (GameState.Get().GetEntity(option.Main.ID).GetZone() == TAG_ZONE.PLAY)
				{
					if (this.endTurnNotifier != null)
					{
						NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.endTurnNotifier);
					}
					Vector3 position = EndTurnButton.Get().transform.position;
					Vector3 position2;
					position2..ctor(position.x - 3f, position.y, position.z);
					this.endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL_NO_ENDTURN_ATK"), true);
					NotificationManager.Get().DestroyNotification(this.endTurnNotifier, 2.5f);
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x000C3016 File Offset: 0x000C1216
	public override void NotifyOfDefeatCoinAnimation()
	{
		if (!this.victory)
		{
			return;
		}
		base.PlaySound("VO_TUTORIAL_06_JAINA_05_53", 1f, true, false);
	}

	// Token: 0x060027FC RID: 10236 RVA: 0x000C3038 File Offset: 0x000C1238
	public override List<RewardData> GetCustomRewards()
	{
		if (!this.victory)
		{
			return null;
		}
		List<RewardData> list = new List<RewardData>();
		CardRewardData cardRewardData = new CardRewardData("CS2_124", TAG_PREMIUM.NORMAL, 2);
		cardRewardData.MarkAsDummyReward();
		list.Add(cardRewardData);
		return list;
	}

	// Token: 0x04001773 RID: 6003
	private Notification endTurnNotifier;

	// Token: 0x04001774 RID: 6004
	private bool victory;

	// Token: 0x04001775 RID: 6005
	private bool m_choSpeaking;
}
