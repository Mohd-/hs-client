using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E9 RID: 745
public class Tutorial_05 : TutorialEntity
{
	// Token: 0x060027E6 RID: 10214 RVA: 0x000C29D0 File Offset: 0x000C0BD0
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_12_12");
		base.PreloadSound("VO_TUTORIAL_04_JAINA_03_39");
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_11_11");
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_02_03");
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_04_05");
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_08_08");
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_03_04");
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_05_06");
		base.PreloadSound("VO_TUTORIAL_05_JAINA_02_46");
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_06_07");
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_09_09");
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_10_10");
		base.PreloadSound("VO_TUTORIAL_05_JAINA_05_47");
		base.PreloadSound("VO_TUTORIAL_05_JAINA_06_48");
		base.PreloadSound("VO_TUTORIAL_05_ILLIDAN_01_02");
		base.PreloadSound("VO_TUTORIAL_05_JAINA_01_45");
		base.PreloadSound("VO_INNKEEPER_TUT_COMPLETE_05");
	}

	// Token: 0x060027E7 RID: 10215 RVA: 0x000C2A98 File Offset: 0x000C0C98
	public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
	{
		base.NotifyOfGameOver(gameResult);
		if (gameResult == TAG_PLAYSTATE.WON)
		{
			this.victory = true;
			base.SetTutorialProgress(TutorialProgress.ILLIDAN_COMPLETE);
			FixedRewardsMgr.Get().CheckForTutorialComplete();
			if (Network.ShouldBeConnectedToAurora())
			{
				BnetPresenceMgr.Get().SetGameField(15U, 1);
			}
			base.ResetTutorialLostProgress();
			base.PlaySound("VO_TUTORIAL_05_ILLIDAN_12_12", 1f, true, false);
		}
		else if (gameResult == TAG_PLAYSTATE.TIED)
		{
			base.PlaySound("VO_TUTORIAL_05_ILLIDAN_12_12", 1f, true, false);
		}
		else if (gameResult == TAG_PLAYSTATE.LOST)
		{
			base.SetTutorialLostProgress(TutorialProgress.ILLIDAN_COMPLETE);
		}
	}

	// Token: 0x060027E8 RID: 10216 RVA: 0x000C2B30 File Offset: 0x000C0D30
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		if (GameState.Get().GetOpposingSidePlayer().HasWeapon())
		{
			GameState.Get().GetOpposingSidePlayer().GetWeaponCard().GetActorSpell(SpellType.DEATH, true).m_BlockServerEvents = true;
		}
		if (turn == 2)
		{
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_03_39", "TUTORIAL04_JAINA_03", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			if (!base.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_11_11", "TUTORIAL05_ILLIDAN_11", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			}
			if (base.GetTag(GAME_TAG.TURN) == 2 && EndTurnButton.Get().IsInNMPState())
			{
				this.ShowEndTurnBouncingArrow();
			}
		}
		yield break;
	}

	// Token: 0x060027E9 RID: 10217 RVA: 0x000C2B5C File Offset: 0x000C0D5C
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
			if (!base.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
			{
				GameState.Get().SetBusy(true);
				this.weaponsPlayed++;
				if (this.weaponsPlayed == 1)
				{
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_02_03", "TUTORIAL05_ILLIDAN_02", Notification.SpeechBubbleDirection.TopLeft, enemyActor, 1f, true, false, 3f));
				}
				else if (this.weaponsPlayed == 2)
				{
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_04_05", "TUTORIAL05_ILLIDAN_04", Notification.SpeechBubbleDirection.TopLeft, enemyActor, 1f, true, false, 3f));
				}
				else
				{
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_08_08", "TUTORIAL05_ILLIDAN_08", Notification.SpeechBubbleDirection.TopLeft, enemyActor, 1f, true, false, 3f));
				}
				GameState.Get().SetBusy(false);
			}
			break;
		case 3:
			if (!base.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_03_04", "TUTORIAL05_ILLIDAN_03", Notification.SpeechBubbleDirection.TopLeft, enemyActor, 1f, true, false, 3f));
			}
			break;
		case 4:
			if (!base.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
			{
				GameState.Get().SetBusy(true);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_05_06", "TUTORIAL05_ILLIDAN_05", Notification.SpeechBubbleDirection.TopLeft, enemyActor, 1f, true, false, 3f));
				GameState.Get().SetBusy(false);
			}
			break;
		case 5:
			if (this.heroPowerHasNotBeenUsed)
			{
				this.heroPowerHasNotBeenUsed = false;
				GameState.Get().SetBusy(true);
				yield return new WaitForSeconds(2f);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_JAINA_02_46", "TUTORIAL05_JAINA_02", Notification.SpeechBubbleDirection.BottomRight, jainaActor, 1f, true, false, 3f));
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_06_07", "TUTORIAL05_ILLIDAN_06", Notification.SpeechBubbleDirection.TopLeft, enemyActor, 1f, true, false, 3f));
				GameState.Get().SetBusy(false);
			}
			break;
		case 6:
			break;
		case 7:
			break;
		case 8:
			if (!base.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_09_09", "TUTORIAL05_ILLIDAN_09", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			}
			break;
		case 9:
			if (!base.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_10_10", "TUTORIAL05_ILLIDAN_10", Notification.SpeechBubbleDirection.TopLeft, enemyActor, 1f, true, false, 3f));
			}
			break;
		case 10:
			if (this.numTimesRemindedAboutGoal == 0)
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_JAINA_05_47", "TUTORIAL05_JAINA_05", Notification.SpeechBubbleDirection.BottomRight, jainaActor, 1f, true, false, 3f));
			}
			else if (this.numTimesRemindedAboutGoal == 1)
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_JAINA_06_48", "TUTORIAL05_JAINA_06", Notification.SpeechBubbleDirection.BottomRight, jainaActor, 1f, true, false, 3f));
			}
			this.numTimesRemindedAboutGoal++;
			break;
		default:
			if (missionEvent != 54)
			{
				if (missionEvent == 55)
				{
					if (!base.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
					{
						base.FadeInHeroActor(enemyActor);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_ILLIDAN_01_02", "TUTORIAL05_ILLIDAN_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
						base.FadeOutHeroActor(enemyActor);
						yield return new WaitForSeconds(0.5f);
						base.FadeInHeroActor(jainaActor);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_05_JAINA_01_45", "TUTORIAL05_JAINA_01", Notification.SpeechBubbleDirection.BottomRight, jainaActor, 1f, true, false, 3f));
					}
					MulliganManager.Get().BeginMulligan();
					if (!base.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE))
					{
						yield return new WaitForSeconds(2.3f);
						base.FadeOutHeroActor(jainaActor);
					}
				}
			}
			else
			{
				yield return new WaitForSeconds(2f);
				string bodyText = base.DidLoseTutorial(TutorialProgress.ILLIDAN_COMPLETE) ? "TUTORIAL05_HELP_04" : "TUTORIAL05_HELP_03";
				base.ShowTutorialDialog("TUTORIAL05_HELP_02", bodyText, "TUTORIAL01_HELP_16", new Vector2(0.5f, 0f), true);
			}
			break;
		case 12:
		{
			GameState.Get().SetBusy(true);
			Vector3 weaponPosition = GameState.Get().GetOpposingSidePlayer().GetHeroCard().transform.position;
			Vector3 popUpPos = new Vector3(weaponPosition.x - 1.55f, weaponPosition.y, weaponPosition.z - 2.721f);
			Notification weaponHelp = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, popUpPos, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL05_HELP_01"), true);
			weaponHelp.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
			NotificationManager.Get().DestroyNotification(weaponHelp, 5f);
			yield return new WaitForSeconds(5.5f);
			GameState.Get().SetBusy(false);
			break;
		}
		}
		yield break;
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x000C2B88 File Offset: 0x000C0D88
	private IEnumerator Wait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		yield break;
	}

	// Token: 0x060027EB RID: 10219 RVA: 0x000C2BAA File Offset: 0x000C0DAA
	public override bool IsKeywordHelpDelayOverridden()
	{
		return true;
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x000C2BB0 File Offset: 0x000C0DB0
	private void ShowEndTurnBouncingArrow()
	{
		if (EndTurnButton.Get().IsInWaitingState())
		{
			return;
		}
		Vector3 position = EndTurnButton.Get().transform.position;
		Vector3 position2;
		position2..ctor(position.x - 2f, position.y, position.z);
		NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0f, -90f, 0f));
	}

	// Token: 0x060027ED RID: 10221 RVA: 0x000C2C20 File Offset: 0x000C0E20
	public override bool NotifyOfEndTurnButtonPushed()
	{
		NotificationManager.Get().DestroyAllArrows();
		return true;
	}

	// Token: 0x060027EE RID: 10222 RVA: 0x000C2C2D File Offset: 0x000C0E2D
	public override bool NotifyOfTooltipDisplay(TooltipZone specificZone)
	{
		return false;
	}

	// Token: 0x060027EF RID: 10223 RVA: 0x000C2C30 File Offset: 0x000C0E30
	public override string[] NotifyOfKeywordHelpPanelDisplay(Entity entity)
	{
		if (entity.GetCardId() == "TU4e_004" || entity.GetCardId() == "TU4e_007")
		{
			return new string[]
			{
				GameStrings.Get("TUTORIAL05_WEAPON_HEADLINE"),
				GameStrings.Get("TUTORIAL05_WEAPON_DESC")
			};
		}
		return null;
	}

	// Token: 0x060027F0 RID: 10224 RVA: 0x000C2C8C File Offset: 0x000C0E8C
	public override List<RewardData> GetCustomRewards()
	{
		if (!this.victory)
		{
			return null;
		}
		List<RewardData> list = new List<RewardData>();
		CardRewardData cardRewardData = new CardRewardData("EX1_277", TAG_PREMIUM.NORMAL, 2);
		cardRewardData.MarkAsDummyReward();
		list.Add(cardRewardData);
		return list;
	}

	// Token: 0x0400176F RID: 5999
	private int weaponsPlayed;

	// Token: 0x04001770 RID: 6000
	private int numTimesRemindedAboutGoal;

	// Token: 0x04001771 RID: 6001
	private bool heroPowerHasNotBeenUsed = true;

	// Token: 0x04001772 RID: 6002
	private bool victory;
}
