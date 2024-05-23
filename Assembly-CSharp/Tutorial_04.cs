using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E8 RID: 744
public class Tutorial_04 : TutorialEntity
{
	// Token: 0x060027CD RID: 10189 RVA: 0x000C1F68 File Offset: 0x000C0168
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_TUTORIAL04_HEMET_23_21");
		base.PreloadSound("VO_TUTORIAL04_HEMET_15_13");
		base.PreloadSound("VO_TUTORIAL04_HEMET_20_18");
		base.PreloadSound("VO_TUTORIAL04_HEMET_16_14");
		base.PreloadSound("VO_TUTORIAL04_HEMET_13_12");
		base.PreloadSound("VO_TUTORIAL04_HEMET_19_17");
		base.PreloadSound("VO_TUTORIAL_04_JAINA_09_43");
		base.PreloadSound("VO_TUTORIAL_04_JAINA_10_44");
		base.PreloadSound("VO_TUTORIAL04_HEMET_06_05");
		base.PreloadSound("VO_TUTORIAL04_HEMET_07_06_ALT");
		base.PreloadSound("VO_TUTORIAL_04_JAINA_04_40");
		base.PreloadSound("VO_TUTORIAL04_HEMET_08_07");
		base.PreloadSound("VO_TUTORIAL04_HEMET_09_08");
		base.PreloadSound("VO_TUTORIAL04_HEMET_10_09");
		base.PreloadSound("VO_TUTORIAL04_HEMET_11_10");
		base.PreloadSound("VO_TUTORIAL04_HEMET_12_11");
		base.PreloadSound("VO_TUTORIAL04_HEMET_12_11_ALT");
		base.PreloadSound("VO_TUTORIAL_04_JAINA_08_42");
		base.PreloadSound("VO_TUTORIAL04_HEMET_01_01");
		base.PreloadSound("VO_TUTORIAL_04_JAINA_01_37");
		base.PreloadSound("VO_TUTORIAL04_HEMET_02_02");
		base.PreloadSound("VO_TUTORIAL04_HEMET_03_03");
		base.PreloadSound("VO_TUTORIAL_04_JAINA_02_38");
		base.PreloadSound("VO_TUTORIAL04_HEMET_04_04_ALT");
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x000C2080 File Offset: 0x000C0280
	public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
	{
		base.NotifyOfGameOver(gameResult);
		if (this.m_heroPowerCostLabel != null)
		{
			Object.Destroy(this.m_heroPowerCostLabel);
		}
		if (gameResult == TAG_PLAYSTATE.WON)
		{
			this.victory = true;
			base.SetTutorialProgress(TutorialProgress.NESINGWARY_COMPLETE);
			base.PlaySound("VO_TUTORIAL04_HEMET_23_21", 1f, true, false);
		}
		else if (gameResult == TAG_PLAYSTATE.TIED)
		{
			base.PlaySound("VO_TUTORIAL04_HEMET_23_21", 1f, true, false);
		}
		else if (gameResult == TAG_PLAYSTATE.LOST)
		{
			base.SetTutorialLostProgress(TutorialProgress.NESINGWARY_COMPLETE);
		}
	}

	// Token: 0x060027CF RID: 10191 RVA: 0x000C210C File Offset: 0x000C030C
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		this.m_shouldSignalPolymorph = false;
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		switch (turn)
		{
		case 1:
			if (!base.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
			{
				GameState.Get().SetBusy(true);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_15_13", "TUTORIAL04_HEMET_15", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
				GameState.Get().SetBusy(false);
			}
			break;
		case 4:
		{
			yield return new WaitForSeconds(1f);
			Vector3 heroPowerPosition = GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard().transform.position;
			if (UniversalInputManager.UsePhoneUI)
			{
				Vector3 help04Position = new Vector3(heroPowerPosition.x, heroPowerPosition.y, heroPowerPosition.z + 2.3f);
				this.heroPowerHelp = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, help04Position, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL04_HELP_01"), true);
				this.heroPowerHelp.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
			}
			else
			{
				Vector3 help04Position2 = new Vector3(heroPowerPosition.x + 3f, heroPowerPosition.y, heroPowerPosition.z);
				this.heroPowerHelp = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, help04Position2, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL04_HELP_01"), true);
				this.heroPowerHelp.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
				AssetLoader.Get().LoadActor("NumberLabel", new AssetLoader.GameObjectCallback(this.ManaLabelLoadedCallback), GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard(), false);
			}
			break;
		}
		case 5:
			NotificationManager.Get().DestroyNotification(this.heroPowerHelp, 0f);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_20_18", "TUTORIAL04_HEMET_20", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			break;
		case 7:
			if (!base.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
			{
				GameState.Get().SetBusy(true);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_16_14", "TUTORIAL04_HEMET_16", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
				GameState.Get().SetBusy(false);
			}
			break;
		case 9:
			if (!base.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
			{
				GameState.Get().SetBusy(true);
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_13_12", "TUTORIAL04_HEMET_13", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
				GameState.Get().SetBusy(false);
			}
			break;
		case 11:
			GameState.Get().SetBusy(true);
			Gameplay.Get().SetGameStateBusy(false, 3f);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_19_17", "TUTORIAL04_HEMET_19", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			yield return new WaitForSeconds(0.7f);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_09_43", "TUTORIAL04_JAINA_09", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			break;
		case 12:
			if (base.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
			{
				this.m_shouldSignalPolymorph = true;
				List<Card> cardsInHand = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
				if (InputManager.Get().GetHeldCard() == null)
				{
					Card polyMorph = null;
					foreach (Card card in cardsInHand)
					{
						if (card.GetEntity().GetCardId() == "CS2_022")
						{
							polyMorph = card;
						}
					}
					if (!(polyMorph == null))
					{
						if (!polyMorph.IsMousedOver())
						{
							Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(0f));
						}
					}
				}
			}
			break;
		}
		yield break;
	}

	// Token: 0x060027D0 RID: 10192 RVA: 0x000C2138 File Offset: 0x000C0338
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
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_06_05", "TUTORIAL04_HEMET_06", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_07_06_ALT", "TUTORIAL04_HEMET_07", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			yield return Gameplay.Get().StartCoroutine(this.Wait(1f));
			GameState.Get().SetBusy(false);
			break;
		case 3:
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(this.Wait(2f));
			GameState.Get().SetBusy(false);
			break;
		case 4:
			if (UniversalInputManager.UsePhoneUI)
			{
				InputManager.Get().GetFriendlyHand().SetHandEnlarged(false);
			}
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_04_40", "TUTORIAL04_JAINA_04", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			break;
		case 5:
			if (!base.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
			{
				switch (this.numBeastsPlayed)
				{
				case 0:
					Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_08_07", "TUTORIAL04_HEMET_08", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					break;
				case 1:
					Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_09_08", "TUTORIAL04_HEMET_09", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					break;
				case 2:
					Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_10_09", "TUTORIAL04_HEMET_10", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					break;
				case 3:
					Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_11_10", "TUTORIAL04_HEMET_11", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
					break;
				}
				this.numBeastsPlayed++;
			}
			break;
		case 6:
			if (this.numComplaintsMade == 0)
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_12_11", "TUTORIAL04_HEMET_12a", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
				this.numComplaintsMade++;
			}
			else
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_12_11_ALT", "TUTORIAL04_HEMET_12b", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			}
			break;
		case 7:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_08_42", "TUTORIAL04_JAINA_08", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			break;
		default:
			if (missionEvent != 54)
			{
				if (missionEvent == 55)
				{
					if (!base.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
					{
						base.FadeInHeroActor(enemyActor);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_01_01", "TUTORIAL04_HEMET_01", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
						base.FadeOutHeroActor(enemyActor);
						base.FadeInHeroActor(jainaActor);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_01_37", "TUTORIAL04_JAINA_01", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
						base.FadeOutHeroActor(jainaActor);
						yield return new WaitForSeconds(0.5f);
					}
					MulliganManager.Get().BeginMulligan();
					if (!base.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
					{
						this.m_hemetSpeaking = true;
						base.FadeInHeroActor(enemyActor);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_02_02", "TUTORIAL04_HEMET_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
						base.FadeOutHeroActor(enemyActor);
						this.m_hemetSpeaking = false;
					}
				}
			}
			else
			{
				yield return new WaitForSeconds(2f);
				string bodyText = base.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE) ? "TUTORIAL04_HELP_16" : "TUTORIAL04_HELP_15";
				base.ShowTutorialDialog("TUTORIAL04_HELP_14", bodyText, "TUTORIAL01_HELP_16", Vector2.zero, true);
			}
			break;
		}
		yield break;
	}

	// Token: 0x060027D1 RID: 10193 RVA: 0x000C2161 File Offset: 0x000C0361
	public override bool IsKeywordHelpDelayOverridden()
	{
		return true;
	}

	// Token: 0x060027D2 RID: 10194 RVA: 0x000C2164 File Offset: 0x000C0364
	public override void NotifyOfCoinFlipResult()
	{
		Gameplay.Get().StartCoroutine(this.HandleCoinFlip());
	}

	// Token: 0x060027D3 RID: 10195 RVA: 0x000C2178 File Offset: 0x000C0378
	private IEnumerator HandleCoinFlip()
	{
		GameState.Get().SetBusy(true);
		yield return Gameplay.Get().StartCoroutine(this.Wait(1f));
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		while (this.m_hemetSpeaking)
		{
			yield return null;
		}
		base.FadeInHeroActor(enemyActor);
		yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_03_03", "TUTORIAL04_HEMET_03", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
		base.FadeOutHeroActor(enemyActor);
		yield return new WaitForSeconds(0.3f);
		base.FadeInHeroActor(jainaActor);
		yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_04_JAINA_02_38", "TUTORIAL04_JAINA_02", Notification.SpeechBubbleDirection.BottomRight, jainaActor, 1f, true, false, 3f));
		base.FadeOutHeroActor(jainaActor);
		yield return new WaitForSeconds(0.25f);
		if (!base.DidLoseTutorial(TutorialProgress.NESINGWARY_COMPLETE))
		{
			base.FadeInHeroActor(enemyActor);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL04_HEMET_04_04_ALT", "TUTORIAL04_HEMET_04", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false, 3f));
			base.FadeOutHeroActor(enemyActor);
			yield return new WaitForSeconds(0.4f);
		}
		GameState.Get().SetBusy(false);
		yield break;
	}

	// Token: 0x060027D4 RID: 10196 RVA: 0x000C2194 File Offset: 0x000C0394
	private IEnumerator Wait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		yield break;
	}

	// Token: 0x060027D5 RID: 10197 RVA: 0x000C21B8 File Offset: 0x000C03B8
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

	// Token: 0x060027D6 RID: 10198 RVA: 0x000C2228 File Offset: 0x000C0428
	private bool AllowEndTurn()
	{
		return !this.m_shouldSignalPolymorph || (this.m_shouldSignalPolymorph && this.m_isBogSheeped);
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x000C225C File Offset: 0x000C045C
	public override bool NotifyOfEndTurnButtonPushed()
	{
		if (base.GetTag(GAME_TAG.TURN) != 4 && this.AllowEndTurn())
		{
			NotificationManager.Get().DestroyAllArrows();
			return true;
		}
		Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
		if (optionsPacket != null && optionsPacket.List != null && optionsPacket.List.Count == 1)
		{
			NotificationManager.Get().DestroyAllArrows();
			return true;
		}
		if (this.endTurnNotifier != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.endTurnNotifier);
		}
		Vector3 position = EndTurnButton.Get().transform.position;
		Vector3 position2;
		position2..ctor(position.x - 3f, position.y, position.z);
		string key = "TUTORIAL_NO_ENDTURN_HP";
		if (GameState.Get().GetFriendlySidePlayer().HasReadyAttackers())
		{
			key = "TUTORIAL_NO_ENDTURN_ATK";
		}
		if (this.m_shouldSignalPolymorph && !this.m_isBogSheeped)
		{
			key = "TUTORIAL_NO_ENDTURN";
		}
		this.endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get(key), true);
		NotificationManager.Get().DestroyNotification(this.endTurnNotifier, 2.5f);
		return false;
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x000C238A File Offset: 0x000C058A
	public override void NotifyOfTargetModeCancelled()
	{
		if (this.sheepTheBog == null)
		{
			return;
		}
		NotificationManager.Get().DestroyAllPopUps();
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x000C23A8 File Offset: 0x000C05A8
	public override void NotifyOfCardGrabbed(Entity entity)
	{
		if (this.m_shouldSignalPolymorph)
		{
			if (entity.GetCardId() == "CS2_022")
			{
				this.m_isPolymorphGrabbed = true;
				if (this.sheepTheBog != null)
				{
					NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.sheepTheBog);
				}
				if (this.handBounceArrow != null)
				{
					NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.handBounceArrow);
				}
				NotificationManager.Get().DestroyAllPopUps();
				Vector3 position = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetFirstCard().transform.position;
				Vector3 position2;
				position2..ctor(position.x - 3f, position.y, position.z);
				this.sheepTheBog = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL04_HELP_02"), true);
				this.sheepTheBog.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
			}
			else
			{
				if (this.sheepTheBog != null)
				{
					NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.sheepTheBog);
				}
				NotificationManager.Get().DestroyAllPopUps();
				if (UniversalInputManager.UsePhoneUI)
				{
					InputManager.Get().ReturnHeldCardToHand();
				}
				else
				{
					InputManager.Get().DropHeldCard();
				}
			}
		}
	}

	// Token: 0x060027DA RID: 10202 RVA: 0x000C24F0 File Offset: 0x000C06F0
	public override void NotifyOfCardDropped(Entity entity)
	{
		this.m_isPolymorphGrabbed = false;
		if (this.m_shouldSignalPolymorph)
		{
			if (this.sheepTheBog != null)
			{
				NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.sheepTheBog);
			}
			NotificationManager.Get().DestroyAllPopUps();
			if (this.ShouldShowArrowOnCardInHand(entity))
			{
				Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(0.5f));
			}
		}
	}

	// Token: 0x060027DB RID: 10203 RVA: 0x000C255C File Offset: 0x000C075C
	public override bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode)
	{
		if (this.m_shouldSignalPolymorph)
		{
			if (clickedEntity.GetCardId() == "CS1_069" && wasInTargetMode)
			{
				if (this.sheepTheBog != null)
				{
					NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.sheepTheBog);
				}
				NotificationManager.Get().DestroyAllPopUps();
				this.m_shouldSignalPolymorph = false;
				this.m_isBogSheeped = true;
			}
			else
			{
				if (this.m_isPolymorphGrabbed && wasInTargetMode)
				{
					if (this.noSheepPopup != null)
					{
						NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.noSheepPopup);
					}
					Vector3 position = clickedEntity.GetCard().transform.position;
					Vector3 position2;
					position2..ctor(position.x + 2.5f, position.y, position.z);
					this.noSheepPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL04_HELP_03"), true);
					NotificationManager.Get().DestroyNotification(this.noSheepPopup, 3f);
					return false;
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x060027DC RID: 10204 RVA: 0x000C266E File Offset: 0x000C086E
	public override bool ShouldAllowCardGrab(Entity entity)
	{
		return !this.m_shouldSignalPolymorph || !(entity.GetCardId() != "CS2_022");
	}

	// Token: 0x060027DD RID: 10205 RVA: 0x000C2694 File Offset: 0x000C0894
	private void ManaLabelLoadedCallback(string actorName, GameObject actorObject, object callbackData)
	{
		Card card = (Card)callbackData;
		GameObject costTextObject = card.GetActor().GetCostTextObject();
		if (costTextObject == null)
		{
			Object.Destroy(actorObject);
			return;
		}
		this.m_heroPowerCostLabel = actorObject;
		SceneUtils.SetLayer(actorObject, GameLayer.Default);
		actorObject.transform.parent = costTextObject.transform;
		actorObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		actorObject.transform.localPosition = new Vector3(-0.02f, 0.38f, 0f);
		actorObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		actorObject.transform.localScale = new Vector3(actorObject.transform.localScale.x, actorObject.transform.localScale.x, actorObject.transform.localScale.x);
		actorObject.GetComponent<UberText>().Text = GameStrings.Get("GLOBAL_COST");
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x000C27A2 File Offset: 0x000C09A2
	public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
	{
		if (this.ShouldShowArrowOnCardInHand(mousedOverEntity))
		{
			NotificationManager.Get().DestroyAllArrows();
		}
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x000C27BA File Offset: 0x000C09BA
	public override void NotifyOfCardMousedOff(Entity mousedOffEntity)
	{
		if (this.ShouldShowArrowOnCardInHand(mousedOffEntity))
		{
			Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(0.5f));
		}
	}

	// Token: 0x060027E0 RID: 10208 RVA: 0x000C27DE File Offset: 0x000C09DE
	private bool ShouldShowArrowOnCardInHand(Entity entity)
	{
		return entity.GetZone() == TAG_ZONE.HAND && (this.m_shouldSignalPolymorph && entity.GetCardId() == "CS2_022");
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x000C2814 File Offset: 0x000C0A14
	private IEnumerator ShowArrowInSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		List<Card> handCards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
		if (handCards.Count == 0 || this.m_isPolymorphGrabbed)
		{
			yield break;
		}
		Card polyMorph = null;
		foreach (Card card in handCards)
		{
			if (card.GetEntity().GetCardId() == "CS2_022")
			{
				polyMorph = card;
			}
		}
		if (polyMorph == null)
		{
			yield break;
		}
		while (iTween.Count(polyMorph.gameObject) > 0)
		{
			yield return null;
		}
		if (polyMorph.IsMousedOver())
		{
			yield break;
		}
		if (InputManager.Get().GetHeldCard() == polyMorph)
		{
			yield break;
		}
		this.ShowHandBouncingArrow();
		yield break;
	}

	// Token: 0x060027E2 RID: 10210 RVA: 0x000C2840 File Offset: 0x000C0A40
	private void ShowHandBouncingArrow()
	{
		if (this.handBounceArrow != null)
		{
			return;
		}
		List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
		if (cards.Count == 0)
		{
			return;
		}
		Card card = null;
		foreach (Card card2 in cards)
		{
			if (card2.GetEntity().GetCardId() == "CS2_022")
			{
				card = card2;
			}
		}
		if (card == null)
		{
			return;
		}
		if (this.m_isPolymorphGrabbed)
		{
			return;
		}
		Vector3 position = card.transform.position;
		Vector3 position2;
		position2..ctor(position.x, position.y, position.z + 2f);
		this.handBounceArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0f, 0f, 0f));
		this.handBounceArrow.transform.parent = card.transform;
	}

	// Token: 0x060027E3 RID: 10211 RVA: 0x000C2964 File Offset: 0x000C0B64
	public override void NotifyOfDefeatCoinAnimation()
	{
		if (!this.victory)
		{
			return;
		}
		base.PlaySound("VO_TUTORIAL_04_JAINA_10_44", 1f, true, false);
	}

	// Token: 0x060027E4 RID: 10212 RVA: 0x000C2984 File Offset: 0x000C0B84
	public override List<RewardData> GetCustomRewards()
	{
		if (!this.victory)
		{
			return null;
		}
		List<RewardData> list = new List<RewardData>();
		CardRewardData cardRewardData = new CardRewardData("CS2_213", TAG_PREMIUM.NORMAL, 2);
		cardRewardData.MarkAsDummyReward();
		list.Add(cardRewardData);
		return list;
	}

	// Token: 0x04001761 RID: 5985
	private Notification endTurnNotifier;

	// Token: 0x04001762 RID: 5986
	private Notification thatsABadPlayPopup;

	// Token: 0x04001763 RID: 5987
	private Notification handBounceArrow;

	// Token: 0x04001764 RID: 5988
	private Notification sheepTheBog;

	// Token: 0x04001765 RID: 5989
	private Notification noSheepPopup;

	// Token: 0x04001766 RID: 5990
	private int numBeastsPlayed;

	// Token: 0x04001767 RID: 5991
	private GameObject m_heroPowerCostLabel;

	// Token: 0x04001768 RID: 5992
	private Notification heroPowerHelp;

	// Token: 0x04001769 RID: 5993
	private bool victory;

	// Token: 0x0400176A RID: 5994
	private bool m_hemetSpeaking;

	// Token: 0x0400176B RID: 5995
	private int numComplaintsMade;

	// Token: 0x0400176C RID: 5996
	private bool m_shouldSignalPolymorph;

	// Token: 0x0400176D RID: 5997
	private bool m_isPolymorphGrabbed;

	// Token: 0x0400176E RID: 5998
	private bool m_isBogSheeped;
}
