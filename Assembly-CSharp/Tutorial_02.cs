using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E6 RID: 742
public class Tutorial_02 : TutorialEntity
{
	// Token: 0x060027A1 RID: 10145 RVA: 0x000C1194 File Offset: 0x000BF394
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_02_05");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_01_04");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_04_07");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_05_08");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_07_10");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_11_14");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_13_16");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_15_17");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_06_09");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_03_06");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_17_19");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_08_11");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_09_12");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_10_13");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_16_18");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_20_22_ALT");
		base.PreloadSound("VO_TUTORIAL_02_JAINA_08_22");
		base.PreloadSound("VO_TUTORIAL_02_JAINA_03_18");
		base.PreloadSound("VO_TUTORIAL02_MILLHOUSE_19_21");
	}

	// Token: 0x060027A2 RID: 10146 RVA: 0x000C1274 File Offset: 0x000BF474
	public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
	{
		base.NotifyOfGameOver(gameResult);
		if (gameResult == TAG_PLAYSTATE.WON)
		{
			base.SetTutorialProgress(TutorialProgress.MILLHOUSE_COMPLETE);
			base.PlaySound("VO_TUTORIAL02_MILLHOUSE_20_22_ALT", 1f, true, false);
		}
		else if (gameResult == TAG_PLAYSTATE.TIED)
		{
			base.PlaySound("VO_TUTORIAL02_MILLHOUSE_20_22_ALT", 1f, true, false);
		}
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x000C12C8 File Offset: 0x000BF4C8
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		if (GameState.Get().IsFriendlySidePlayerTurn())
		{
			this.numManaThisTurn++;
		}
		Actor millhouseActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		switch (turn)
		{
		case 1:
		{
			Vector3 manaPosition = ManaCrystalMgr.Get().GetManaCrystalSpawnPosition();
			Vector3 manaPopupPosition;
			Notification.PopUpArrowDirection direction;
			if (UniversalInputManager.UsePhoneUI)
			{
				manaPopupPosition = new Vector3(manaPosition.x - 0.7f, manaPosition.y + 1.14f, manaPosition.z + 4.33f);
				direction = Notification.PopUpArrowDirection.RightDown;
			}
			else
			{
				manaPopupPosition = new Vector3(manaPosition.x - 0.02f, manaPosition.y + 0.2f, manaPosition.z + 1.8f);
				direction = Notification.PopUpArrowDirection.Down;
			}
			this.manaNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, manaPopupPosition, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL02_HELP_01"), true);
			this.manaNotifier.ShowPopUpArrow(direction);
			yield return new WaitForSeconds(4.5f);
			if (this.manaNotifier != null)
			{
				iTween.PunchScale(this.manaNotifier.gameObject, iTween.Hash(new object[]
				{
					"amount",
					new Vector3(1f, 1f, 1f),
					"time",
					1f
				}));
				yield return new WaitForSeconds(4.5f);
				if (this.manaNotifier != null)
				{
					iTween.PunchScale(this.manaNotifier.gameObject, iTween.Hash(new object[]
					{
						"amount",
						new Vector3(1f, 1f, 1f),
						"time",
						1f
					}));
					yield return new WaitForSeconds(4.5f);
					if (this.manaNotifier != null)
					{
						NotificationManager.Get().DestroyNotification(this.manaNotifier, 0f);
					}
				}
			}
			break;
		}
		case 2:
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(0.5f);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_04_07", "TUTORIAL02_MILLHOUSE_04", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			yield return new WaitForSeconds(0.3f);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_05_08", "TUTORIAL02_MILLHOUSE_05", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			break;
		case 3:
		{
			Vector3 manaPosition2 = ManaCrystalMgr.Get().GetManaCrystalSpawnPosition();
			Vector3 manaPopupPosition2;
			Notification.PopUpArrowDirection direction2;
			if (UniversalInputManager.UsePhoneUI)
			{
				manaPopupPosition2 = new Vector3(manaPosition2.x - 0.7f, manaPosition2.y + 1.14f, manaPosition2.z + 4.33f);
				direction2 = Notification.PopUpArrowDirection.RightDown;
			}
			else
			{
				manaPopupPosition2 = new Vector3(manaPosition2.x - 0.02f, manaPosition2.y + 0.2f, manaPosition2.z + 1.7f);
				direction2 = Notification.PopUpArrowDirection.Down;
			}
			this.manaNotifier2 = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, manaPopupPosition2, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL02_HELP_03"), true);
			this.manaNotifier2.ShowPopUpArrow(direction2);
			yield return new WaitForSeconds(4.5f);
			if (this.manaNotifier2 != null)
			{
				iTween.PunchScale(this.manaNotifier2.gameObject, iTween.Hash(new object[]
				{
					"amount",
					new Vector3(1f, 1f, 1f),
					"time",
					1f
				}));
				yield return new WaitForSeconds(4.5f);
				if (this.manaNotifier2 != null)
				{
					iTween.PunchScale(this.manaNotifier2.gameObject, iTween.Hash(new object[]
					{
						"amount",
						new Vector3(1f, 1f, 1f),
						"time",
						1f
					}));
				}
			}
			break;
		}
		case 4:
		{
			if (this.manaNotifier2 != null)
			{
				NotificationManager.Get().DestroyNotification(this.manaNotifier2, 0f);
			}
			GameState.Get().SetBusy(true);
			AudioSource previousLine = base.GetPreloadedSound("VO_TUTORIAL02_MILLHOUSE_17_19");
			while (SoundManager.Get().IsPlaying(previousLine))
			{
				yield return null;
			}
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_07_10", "TUTORIAL02_MILLHOUSE_07", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			break;
		}
		case 6:
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_11_14", "TUTORIAL02_MILLHOUSE_11", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			break;
		case 8:
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_13_16", "TUTORIAL02_MILLHOUSE_13", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			break;
		case 9:
			yield return new WaitForSeconds(0.5f);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_15_17", "TUTORIAL02_MILLHOUSE_15", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			break;
		case 10:
		{
			GameState.Get().SetBusy(true);
			AudioSource comeOnLine = base.GetPreloadedSound("VO_TUTORIAL02_MILLHOUSE_16_18");
			while (SoundManager.Get().IsPlaying(comeOnLine))
			{
				yield return null;
			}
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_06_09", "TUTORIAL02_MILLHOUSE_06", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			break;
		}
		}
		yield break;
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x000C12F4 File Offset: 0x000BF4F4
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor millhouseActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		switch (missionEvent)
		{
		case 1:
			base.HandleGameStartEvent();
			break;
		case 2:
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(1.5f);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_03_06", "TUTORIAL02_MILLHOUSE_03", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			yield return new WaitForSeconds(4f);
			if (base.GetTag(GAME_TAG.TURN) == 1 && !EndTurnButton.Get().IsInWaitingState())
			{
				this.ShowEndTurnBouncingArrow();
			}
			break;
		case 3:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_17_19", "TUTORIAL02_MILLHOUSE_17", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			break;
		case 4:
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_08_11", "TUTORIAL02_MILLHOUSE_08", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_02_JAINA_03_18", "TUTORIAL02_JAINA_03", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_09_12", "TUTORIAL02_MILLHOUSE_09", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			break;
		case 5:
		{
			GameState.Get().SetBusy(true);
			AudioSource feelslikeLine = base.GetPreloadedSound("VO_TUTORIAL02_MILLHOUSE_08_11");
			while (SoundManager.Get().IsPlaying(feelslikeLine))
			{
				yield return null;
			}
			AudioSource whatLine = base.GetPreloadedSound("VO_TUTORIAL_02_JAINA_03_18");
			while (SoundManager.Get().IsPlaying(whatLine))
			{
				yield return null;
			}
			AudioSource winngingLine = base.GetPreloadedSound("VO_TUTORIAL02_MILLHOUSE_09_12");
			while (SoundManager.Get().IsPlaying(winngingLine))
			{
				yield return null;
			}
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_10_13", "TUTORIAL02_MILLHOUSE_10", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			break;
		}
		case 6:
			if (EndTurnButton.Get().IsInNMPState())
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_16_18", "TUTORIAL02_MILLHOUSE_16", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
			}
			break;
		default:
			if (missionEvent != 54)
			{
				if (missionEvent == 55)
				{
					base.FadeInHeroActor(millhouseActor);
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_02_05", "TUTORIAL02_MILLHOUSE_02", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
					HistoryManager.Get().DisableHistory();
					MulliganManager.Get().BeginMulligan();
					yield return new WaitForSeconds(1.1f);
					base.FadeOutHeroActor(millhouseActor);
				}
			}
			else
			{
				yield return new WaitForSeconds(2f);
				base.ShowTutorialDialog("TUTORIAL02_HELP_06", "TUTORIAL02_HELP_07", "TUTORIAL01_HELP_16", new Vector2(0.5f, 0f), false);
			}
			break;
		case 9:
			break;
		}
		yield break;
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x000C1320 File Offset: 0x000BF520
	public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
	{
		if (mousedOverEntity.GetZone() == TAG_ZONE.HAND && base.GetTag(GAME_TAG.TURN) <= 7)
		{
			AssetLoader.Get().LoadActor("NumberLabel", new AssetLoader.GameObjectCallback(this.ManaLabelLoadedCallback), mousedOverEntity.GetCard(), false);
		}
	}

	// Token: 0x060027A6 RID: 10150 RVA: 0x000C136A File Offset: 0x000BF56A
	public override void NotifyOfCardMousedOff(Entity mousedOffEntity)
	{
		if (this.costLabel != null)
		{
			Object.Destroy(this.costLabel);
		}
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x000C1388 File Offset: 0x000BF588
	public override void NotifyOfCoinFlipResult()
	{
		Gameplay.Get().StartCoroutine(this.HandleCoinFlip());
	}

	// Token: 0x060027A8 RID: 10152 RVA: 0x000C139C File Offset: 0x000BF59C
	private IEnumerator HandleCoinFlip()
	{
		GameState.Get().SetBusy(true);
		yield return new WaitForSeconds(3.5f);
		Actor millhouseActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		base.FadeInHeroActor(millhouseActor);
		yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL02_MILLHOUSE_01_04", "TUTORIAL02_MILLHOUSE_01", Notification.SpeechBubbleDirection.TopRight, millhouseActor, 1f, true, false, 3f));
		GameState.Get().SetBusy(false);
		yield return new WaitForSeconds(0.175f);
		base.FadeOutHeroActor(millhouseActor);
		yield break;
	}

	// Token: 0x060027A9 RID: 10153 RVA: 0x000C13B8 File Offset: 0x000BF5B8
	public override bool NotifyOfEndTurnButtonPushed()
	{
		Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
		if (optionsPacket != null && optionsPacket.List != null)
		{
			if (optionsPacket.List.Count == 1)
			{
				NotificationManager.Get().DestroyAllArrows();
				return true;
			}
			if (optionsPacket.List.Count == 2)
			{
				for (int i = 0; i < optionsPacket.List.Count; i++)
				{
					Network.Options.Option option = optionsPacket.List[i];
					if (option.Type == Network.Options.Option.OptionType.POWER)
					{
						if (GameState.Get().GetEntity(option.Main.ID).GetCardId() == "CS2_025")
						{
							return true;
						}
					}
				}
			}
		}
		if (this.endTurnNotifier != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.endTurnNotifier);
		}
		Vector3 position = EndTurnButton.Get().transform.position;
		Vector3 position2;
		position2..ctor(position.x - 3f, position.y, position.z);
		string key = "TUTORIAL_NO_ENDTURN";
		if (GameState.Get().GetFriendlySidePlayer().HasReadyAttackers())
		{
			key = "TUTORIAL_NO_ENDTURN_ATK";
		}
		this.endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get(key), true);
		NotificationManager.Get().DestroyNotification(this.endTurnNotifier, 2.5f);
		return false;
	}

	// Token: 0x060027AA RID: 10154 RVA: 0x000C1520 File Offset: 0x000BF720
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

	// Token: 0x060027AB RID: 10155 RVA: 0x000C1590 File Offset: 0x000BF790
	public override string[] NotifyOfKeywordHelpPanelDisplay(Entity entity)
	{
		if (entity.GetCardId() == "CS2_122")
		{
			return new string[]
			{
				GameStrings.Get("TUTORIAL_RAID_LEADER_HEADLINE"),
				GameStrings.Get("TUTORIAL_RAID_LEADER_DESCRIPTION")
			};
		}
		if (entity.GetCardId() == "CS2_023")
		{
			return new string[]
			{
				GameStrings.Get("TUTORIAL_ARCANE_INTELLECT_HEADLINE"),
				GameStrings.Get("TUTORIAL_ARCANE_INTELLECT_DESCRIPTION")
			};
		}
		return null;
	}

	// Token: 0x060027AC RID: 10156 RVA: 0x000C1610 File Offset: 0x000BF810
	public override void NotifyOfCardGrabbed(Entity entity)
	{
		if (entity.GetCardId() == "CS2_023" && GameState.Get().GetFriendlySidePlayer().GetNumAvailableResources() >= entity.GetCost())
		{
			BoardTutorial.Get().EnableFullHighlight(true);
		}
		if (this.costLabel != null)
		{
			Object.Destroy(this.costLabel);
		}
	}

	// Token: 0x060027AD RID: 10157 RVA: 0x000C1674 File Offset: 0x000BF874
	public override void NotifyOfCardDropped(Entity entity)
	{
		if (entity.GetCardId() == "CS2_023")
		{
			BoardTutorial.Get().EnableFullHighlight(false);
		}
	}

	// Token: 0x060027AE RID: 10158 RVA: 0x000C16A4 File Offset: 0x000BF8A4
	public override void NotifyOfManaCrystalSpawned()
	{
		AssetLoader.Get().LoadActor("plus1", new AssetLoader.GameObjectCallback(this.Plus1ActorLoadedCallback), null, false);
		if (base.GetTag(GAME_TAG.TURN) == 3)
		{
			Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_02_JAINA_08_22", "TUTORIAL02_JAINA_08", Notification.SpeechBubbleDirection.BottomLeft, actor, 1f, true, false, 3f));
		}
		this.FadeInManaSpotlight();
	}

	// Token: 0x060027AF RID: 10159 RVA: 0x000C1726 File Offset: 0x000BF926
	private void FadeInManaSpotlight()
	{
		Gameplay.Get().StartCoroutine(this.StartManaSpotFade());
	}

	// Token: 0x060027B0 RID: 10160 RVA: 0x000C173C File Offset: 0x000BF93C
	private IEnumerator StartManaSpotFade()
	{
		Light manaSpot = BoardTutorial.Get().m_ManaSpotlight;
		manaSpot.enabled = true;
		manaSpot.spotAngle = 179f;
		manaSpot.intensity = 0f;
		float TARGET_INTENSITY = 0.6f;
		while (manaSpot.intensity < TARGET_INTENSITY * 0.95f)
		{
			manaSpot.intensity = Mathf.Lerp(manaSpot.intensity, TARGET_INTENSITY, Time.deltaTime * 5f);
			manaSpot.spotAngle = Mathf.Lerp(manaSpot.spotAngle, 80f, Time.deltaTime * 5f);
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		while (manaSpot.intensity > 0.05f)
		{
			manaSpot.intensity = Mathf.Lerp(manaSpot.intensity, 0f, Time.deltaTime * 10f);
			yield return null;
		}
		manaSpot.enabled = false;
		yield break;
	}

	// Token: 0x060027B1 RID: 10161 RVA: 0x000C1750 File Offset: 0x000BF950
	private void Plus1ActorLoadedCallback(string actorName, GameObject actorObject, object callbackData)
	{
		Vector3 manaCrystalSpawnPosition = ManaCrystalMgr.Get().GetManaCrystalSpawnPosition();
		Vector3 position;
		position..ctor(manaCrystalSpawnPosition.x - 0.02f, manaCrystalSpawnPosition.y + 0.2f, manaCrystalSpawnPosition.z);
		actorObject.transform.position = position;
		actorObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		Vector3 localScale = actorObject.transform.localScale;
		actorObject.transform.localScale = new Vector3(1f, 1f, 1f);
		iTween.MoveTo(actorObject, new Vector3(position.x, position.y, position.z + 2f), 3f);
		float num = 2.5f;
		iTween.ScaleTo(actorObject, new Vector3(localScale.x * num, localScale.y * num, localScale.z * num), 3f);
		iTween.RotateTo(actorObject, new Vector3(0f, 170f, 0f), 3f);
		iTween.FadeTo(actorObject, 0f, 2.75f);
	}

	// Token: 0x060027B2 RID: 10162 RVA: 0x000C1871 File Offset: 0x000BFA71
	public override void NotifyOfEnemyManaCrystalSpawned()
	{
		AssetLoader.Get().LoadActor("plus1", new AssetLoader.GameObjectCallback(this.Plus1ActorLoadedCallbackEnemy), null, false);
	}

	// Token: 0x060027B3 RID: 10163 RVA: 0x000C1894 File Offset: 0x000BFA94
	private void Plus1ActorLoadedCallbackEnemy(string actorName, GameObject actorObject, object callbackData)
	{
		GameObject gameObject = SceneUtils.FindChildBySubstring(Board.Get().gameObject, "ManaCounter_Opposing");
		Vector3 position = gameObject.transform.position;
		Vector3 position2;
		position2..ctor(position.x, position.y + 0.2f, position.z);
		actorObject.transform.position = position2;
		actorObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		Vector3 localScale = actorObject.transform.localScale;
		actorObject.transform.localScale = new Vector3(1f, 1f, 1f);
		iTween.MoveTo(actorObject, new Vector3(position2.x, position2.y, position2.z - 2f), 3f);
		float num = 2.5f;
		iTween.ScaleTo(actorObject, new Vector3(localScale.x * num, localScale.y * num, localScale.z * num), 3f);
		iTween.RotateTo(actorObject, new Vector3(0f, 170f, 0f), 3f);
		iTween.FadeTo(actorObject, 0f, 2.75f);
	}

	// Token: 0x060027B4 RID: 10164 RVA: 0x000C19CC File Offset: 0x000BFBCC
	private void ManaLabelLoadedCallback(string actorName, GameObject actorObject, object callbackData)
	{
		Card card = (Card)callbackData;
		GameObject costTextObject = card.GetActor().GetCostTextObject();
		if (costTextObject == null)
		{
			Object.Destroy(actorObject);
			return;
		}
		if (this.costLabel != null)
		{
			Object.Destroy(this.costLabel);
		}
		this.costLabel = actorObject;
		actorObject.transform.parent = costTextObject.transform;
		actorObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		actorObject.transform.localPosition = new Vector3(-0.025f, 0.28f, 0f);
		actorObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		actorObject.GetComponent<UberText>().Text = GameStrings.Get("GLOBAL_COST");
	}

	// Token: 0x060027B5 RID: 10165 RVA: 0x000C1AA8 File Offset: 0x000BFCA8
	public override void NotifyOfTooltipZoneMouseOver(TooltipZone tooltip)
	{
		ManaCrystalMgr component = tooltip.targetObject.GetComponent<ManaCrystalMgr>();
		if (component != null)
		{
			if (this.manaNotifier != null)
			{
				Object.Destroy(this.manaNotifier.gameObject);
			}
			if (this.manaNotifier2 != null)
			{
				Object.Destroy(this.manaNotifier2.gameObject);
			}
		}
	}

	// Token: 0x060027B6 RID: 10166 RVA: 0x000C1B0F File Offset: 0x000BFD0F
	public override string GetTurnStartReminderText()
	{
		return GameStrings.Format("TUTORIAL02_HELP_04", new object[]
		{
			this.numManaThisTurn
		});
	}

	// Token: 0x060027B7 RID: 10167 RVA: 0x000C1B2F File Offset: 0x000BFD2F
	public override void NotifyOfDefeatCoinAnimation()
	{
		Gameplay.Get().StartCoroutine(this.PlayGoingOutSound());
	}

	// Token: 0x060027B8 RID: 10168 RVA: 0x000C1B44 File Offset: 0x000BFD44
	private IEnumerator PlayGoingOutSound()
	{
		AudioSource deathLine = base.GetPreloadedSound("VO_TUTORIAL02_MILLHOUSE_20_22_ALT");
		while (deathLine != null && deathLine.isPlaying)
		{
			yield return null;
		}
		base.PlaySound("VO_TUTORIAL02_MILLHOUSE_19_21", 1f, true, false);
		yield break;
	}

	// Token: 0x060027B9 RID: 10169 RVA: 0x000C1B60 File Offset: 0x000BFD60
	protected override void NotifyOfManaError()
	{
		NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.manaNotifier);
		NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.manaNotifier2);
	}

	// Token: 0x060027BA RID: 10170 RVA: 0x000C1B90 File Offset: 0x000BFD90
	public override List<RewardData> GetCustomRewards()
	{
		List<RewardData> list = new List<RewardData>();
		CardRewardData cardRewardData = new CardRewardData("EX1_015", TAG_PREMIUM.NORMAL, 2);
		cardRewardData.MarkAsDummyReward();
		list.Add(cardRewardData);
		return list;
	}

	// Token: 0x04001751 RID: 5969
	private Notification endTurnNotifier;

	// Token: 0x04001752 RID: 5970
	private Notification manaNotifier;

	// Token: 0x04001753 RID: 5971
	private Notification manaNotifier2;

	// Token: 0x04001754 RID: 5972
	private Notification handBounceArrow;

	// Token: 0x04001755 RID: 5973
	private GameObject costLabel;

	// Token: 0x04001756 RID: 5974
	private int numManaThisTurn = 1;
}
