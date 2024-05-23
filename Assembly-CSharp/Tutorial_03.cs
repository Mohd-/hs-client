using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E7 RID: 743
public class Tutorial_03 : TutorialEntity
{
	// Token: 0x060027BC RID: 10172 RVA: 0x000C1BC8 File Offset: 0x000BFDC8
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_TUTORIAL_03_JAINA_17_33");
		base.PreloadSound("VO_TUTORIAL_03_JAINA_18_34");
		base.PreloadSound("VO_TUTORIAL_03_JAINA_01_24");
		base.PreloadSound("VO_TUTORIAL_03_JAINA_05_25");
		base.PreloadSound("VO_TUTORIAL_03_JAINA_07_26");
		base.PreloadSound("VO_TUTORIAL_03_JAINA_12_28");
		base.PreloadSound("VO_TUTORIAL_03_JAINA_13_29");
		base.PreloadSound("VO_TUTORIAL_03_JAINA_16_32");
		base.PreloadSound("VO_TUTORIAL_03_JAINA_14_30");
		base.PreloadSound("VO_TUTORIAL_03_JAINA_15_31");
		base.PreloadSound("VO_TUTORIAL_03_JAINA_20_36");
		base.PreloadSound("VO_TUTORIAL_03_MUKLA_01_01");
		base.PreloadSound("VO_TUTORIAL_03_MUKLA_03_03");
		base.PreloadSound("VO_TUTORIAL_03_MUKLA_04_04");
		base.PreloadSound("VO_TUTORIAL_03_MUKLA_05_05");
		base.PreloadSound("VO_TUTORIAL_03_MUKLA_06_06");
		base.PreloadSound("VO_TUTORIAL_03_MUKLA_07_07");
	}

	// Token: 0x060027BD RID: 10173 RVA: 0x000C1C90 File Offset: 0x000BFE90
	public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
	{
		base.NotifyOfGameOver(gameResult);
		if (gameResult == TAG_PLAYSTATE.WON)
		{
			this.victory = true;
			base.SetTutorialProgress(TutorialProgress.MUKLA_COMPLETE);
			base.PlaySound("VO_TUTORIAL_03_MUKLA_07_07", 1f, true, false);
		}
		else if (gameResult == TAG_PLAYSTATE.TIED)
		{
			base.PlaySound("VO_TUTORIAL_03_MUKLA_07_07", 1f, true, false);
		}
		else if (gameResult == TAG_PLAYSTATE.LOST)
		{
			base.SetTutorialLostProgress(TutorialProgress.MUKLA_COMPLETE);
		}
	}

	// Token: 0x060027BE RID: 10174 RVA: 0x000C1D00 File Offset: 0x000BFF00
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		if (this.enemyPlayedBigBrother)
		{
			if (GameState.Get().IsFriendlySidePlayerTurn())
			{
				if (GameState.Get().GetOpposingSidePlayer().GetNumMinionsInPlay() > 0)
				{
					if (!this.needATaunterVOPlayed)
					{
						if (!GameState.Get().GetFriendlySidePlayer().HasATauntMinion())
						{
							this.needATaunterVOPlayed = true;
							Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_17_33", "TUTORIAL03_JAINA_17", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
						}
						yield break;
					}
					if (!this.defenselessVoPlayed)
					{
						bool noTaunters = true;
						List<Card> myMinions = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards();
						foreach (Card minion in myMinions)
						{
							if (minion.GetEntity().HasTaunt())
							{
								noTaunters = false;
							}
						}
						if (noTaunters)
						{
							this.defenselessVoPlayed = true;
							Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_18_34", "TUTORIAL03_JAINA_18", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
						}
					}
				}
			}
			else if (!this.seenTheBrother)
			{
				Gameplay.Get().StartCoroutine(this.GetReadyForBro());
			}
		}
		switch (turn)
		{
		case 1:
			if (!base.DidLoseTutorial(TutorialProgress.MUKLA_COMPLETE))
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_01_24", "TUTORIAL03_JAINA_01", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			}
			break;
		case 5:
			if (!base.DidLoseTutorial(TutorialProgress.MUKLA_COMPLETE))
			{
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_05_25", "TUTORIAL03_JAINA_05", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
				Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_MUKLA_03_03", "TUTORIAL03_MUKLA_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			}
			break;
		case 6:
			if (!base.DidLoseTutorial(TutorialProgress.MUKLA_COMPLETE))
			{
				GameState.Get().SetBusy(true);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_MUKLA_04_04", "TUTORIAL03_MUKLA_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
				GameState.Get().SetBusy(false);
			}
			break;
		case 9:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_07_26", "TUTORIAL03_JAINA_07", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			break;
		case 14:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_MUKLA_05_05", "TUTORIAL03_MUKLA_05", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			break;
		}
		yield break;
	}

	// Token: 0x060027BF RID: 10175 RVA: 0x000C1D2C File Offset: 0x000BFF2C
	private IEnumerator GetReadyForBro()
	{
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		this.seenTheBrother = true;
		GameState.Get().SetBusy(true);
		yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_12_28", "TUTORIAL03_JAINA_12", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
		GameState.Get().SetBusy(false);
		yield return new WaitForSeconds(3.2f);
		Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_13_29", "TUTORIAL03_JAINA_13", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
		yield break;
	}

	// Token: 0x060027C0 RID: 10176 RVA: 0x000C1D48 File Offset: 0x000BFF48
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		switch (missionEvent)
		{
		case 1:
			base.HandleGameStartEvent();
			AssetLoader.Get().LoadActor("TutorialKeywordManager", false, false);
			break;
		case 2:
			break;
		default:
			if (missionEvent != 54)
			{
				if (missionEvent == 55)
				{
					base.FadeInHeroActor(enemyActor);
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_MUKLA_01_01", "TUTORIAL03_MUKLA_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					MulliganManager.Get().BeginMulligan();
					base.FadeOutHeroActor(enemyActor);
				}
			}
			else
			{
				yield return new WaitForSeconds(2f);
				string helpString = "TUTORIAL03_HELP_03";
				if (UniversalInputManager.Get().IsTouchMode())
				{
					helpString = "TUTORIAL03_HELP_03_TOUCH";
				}
				base.ShowTutorialDialog("TUTORIAL03_HELP_02", helpString, "TUTORIAL01_HELP_16", new Vector2(0.5f, 0.5f), false);
			}
			break;
		case 4:
			this.numTauntGorillasPlayed++;
			if (this.numTauntGorillasPlayed == 1)
			{
				yield return new WaitForSeconds(1f);
				if (GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards().Count == 0)
				{
					yield break;
				}
				Vector3 gorillaPosition = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards()[0].transform.position;
				Vector3 help04Position = new Vector3(gorillaPosition.x - 3f, gorillaPosition.y, gorillaPosition.z);
				yield return new WaitForSeconds(2f);
				Notification tauntHelp = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, help04Position, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL03_HELP_01"), true);
				tauntHelp.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
				NotificationManager.Get().DestroyNotification(tauntHelp, 6f);
			}
			else if (this.numTauntGorillasPlayed == 2)
			{
				if (!base.DidLoseTutorial(TutorialProgress.MUKLA_COMPLETE))
				{
					GameState.Get().SetBusy(true);
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_MUKLA_06_06", "TUTORIAL03_MUKLA_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					GameState.Get().SetBusy(false);
				}
			}
			break;
		case 8:
			break;
		case 9:
			break;
		case 10:
		{
			this.enemyPlayedBigBrother = true;
			ZonePlay enemyBattlefield = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone();
			enemyBattlefield.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			Vector3 oldPosition = enemyBattlefield.transform.position;
			enemyBattlefield.transform.position = new Vector3(oldPosition.x + 2.3931637f, oldPosition.y, oldPosition.z + 0.7f);
			enemyBattlefield.transform.localEulerAngles = new Vector3(356.22f, 0f, 0f);
			if (!GameState.Get().IsFriendlySidePlayerTurn())
			{
				Gameplay.Get().StartCoroutine(this.GetReadyForBro());
			}
			break;
		}
		case 11:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_16_32", "TUTORIAL03_JAINA_16", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			break;
		case 12:
			if (!this.monkeyLinePlayedOnce)
			{
				this.monkeyLinePlayedOnce = true;
				Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_14_30", "TUTORIAL03_JAINA_14", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			}
			else if (!base.DidLoseTutorial(TutorialProgress.MUKLA_COMPLETE))
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_03_JAINA_15_31", "TUTORIAL03_JAINA_15", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			}
			break;
		}
		yield break;
	}

	// Token: 0x060027C1 RID: 10177 RVA: 0x000C1D71 File Offset: 0x000BFF71
	private void DialogLoadedCallback(string actorName, GameObject actorObject, object callbackData)
	{
	}

	// Token: 0x060027C2 RID: 10178 RVA: 0x000C1D73 File Offset: 0x000BFF73
	public override bool IsKeywordHelpDelayOverridden()
	{
		return true;
	}

	// Token: 0x060027C3 RID: 10179 RVA: 0x000C1D76 File Offset: 0x000BFF76
	public override void NotifyOfCardMousedOff(Entity mousedOffEntity)
	{
		this.overrideMouseOver = false;
	}

	// Token: 0x060027C4 RID: 10180 RVA: 0x000C1D7F File Offset: 0x000BFF7F
	public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
	{
		if (mousedOverEntity.HasTaunt())
		{
			this.overrideMouseOver = true;
		}
	}

	// Token: 0x060027C5 RID: 10181 RVA: 0x000C1D94 File Offset: 0x000BFF94
	public override bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode)
	{
		if (!base.NotifyOfBattlefieldCardClicked(clickedEntity, wasInTargetMode))
		{
			return false;
		}
		if (wasInTargetMode && clickedEntity.GetCardId() == "TU4c_007" && !this.warnedAgainstAttackingGorilla)
		{
			this.warnedAgainstAttackingGorilla = true;
			base.HandleMissionEvent(11);
			return false;
		}
		return true;
	}

	// Token: 0x060027C6 RID: 10182 RVA: 0x000C1DE8 File Offset: 0x000BFFE8
	private void ShowDontPolymorphYourselfPopup(Vector3 origin)
	{
		if (this.thatsABadPlayPopup != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.thatsABadPlayPopup);
		}
		Vector3 position;
		position..ctor(origin.x - 3f, origin.y, origin.z);
		this.thatsABadPlayPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL01_HELP_07"), true);
		NotificationManager.Get().DestroyNotification(this.thatsABadPlayPopup, 2.5f);
	}

	// Token: 0x060027C7 RID: 10183 RVA: 0x000C1E70 File Offset: 0x000C0070
	private void ShowDontFireballYourselfPopup(Vector3 origin)
	{
		if (this.thatsABadPlayPopup != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.thatsABadPlayPopup);
		}
		Vector3 position;
		position..ctor(origin.x - 3f, origin.y, origin.z);
		this.thatsABadPlayPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL01_HELP_07"), true);
		NotificationManager.Get().DestroyNotification(this.thatsABadPlayPopup, 2.5f);
	}

	// Token: 0x060027C8 RID: 10184 RVA: 0x000C1EF7 File Offset: 0x000C00F7
	public override bool IsMouseOverDelayOverriden()
	{
		return this.overrideMouseOver;
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x000C1EFF File Offset: 0x000C00FF
	public override bool ShouldShowCrazyKeywordTooltip()
	{
		return true;
	}

	// Token: 0x060027CA RID: 10186 RVA: 0x000C1F02 File Offset: 0x000C0102
	public override void NotifyOfDefeatCoinAnimation()
	{
		if (!this.victory)
		{
			return;
		}
		base.PlaySound("VO_TUTORIAL_03_JAINA_20_36", 1f, true, false);
	}

	// Token: 0x060027CB RID: 10187 RVA: 0x000C1F24 File Offset: 0x000C0124
	public override List<RewardData> GetCustomRewards()
	{
		if (!this.victory)
		{
			return null;
		}
		List<RewardData> list = new List<RewardData>();
		CardRewardData cardRewardData = new CardRewardData("CS2_022", TAG_PREMIUM.NORMAL, 2);
		cardRewardData.MarkAsDummyReward();
		list.Add(cardRewardData);
		return list;
	}

	// Token: 0x04001757 RID: 5975
	private int numTauntGorillasPlayed;

	// Token: 0x04001758 RID: 5976
	private bool enemyPlayedBigBrother;

	// Token: 0x04001759 RID: 5977
	private bool needATaunterVOPlayed;

	// Token: 0x0400175A RID: 5978
	private bool overrideMouseOver;

	// Token: 0x0400175B RID: 5979
	private bool monkeyLinePlayedOnce;

	// Token: 0x0400175C RID: 5980
	private bool defenselessVoPlayed;

	// Token: 0x0400175D RID: 5981
	private bool seenTheBrother;

	// Token: 0x0400175E RID: 5982
	private bool warnedAgainstAttackingGorilla;

	// Token: 0x0400175F RID: 5983
	private bool victory;

	// Token: 0x04001760 RID: 5984
	private Notification thatsABadPlayPopup;
}
