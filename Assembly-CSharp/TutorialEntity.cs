using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006C2 RID: 1730
public class TutorialEntity : MissionEntity
{
	// Token: 0x06004814 RID: 18452 RVA: 0x00159FCD File Offset: 0x001581CD
	protected override void HandleMulliganTagChange()
	{
	}

	// Token: 0x06004815 RID: 18453 RVA: 0x00159FD0 File Offset: 0x001581D0
	public override bool NotifyOfPlayError(PlayErrors.ErrorType error, Entity errorSource)
	{
		if (error == PlayErrors.ErrorType.REQ_ENOUGH_MANA)
		{
			Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
			if (errorSource.GetCost() > GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.RESOURCES))
			{
				Notification notification = NotificationManager.Get().CreateSpeechBubble(GameStrings.Get("TUTORIAL02_JAINA_05"), Notification.SpeechBubbleDirection.BottomLeft, actor, true, true);
				SoundManager.Get().LoadAndPlay("VO_TUTORIAL_02_JAINA_05_20");
				NotificationManager.Get().DestroyNotification(notification, 3.5f);
				Gameplay.Get().StartCoroutine(this.DisplayManaReminder(GameStrings.Get("TUTORIAL02_HELP_01")));
			}
			else
			{
				Notification notification2 = NotificationManager.Get().CreateSpeechBubble(GameStrings.Get("TUTORIAL02_JAINA_04"), Notification.SpeechBubbleDirection.BottomLeft, actor, true, true);
				SoundManager.Get().LoadAndPlay("VO_TUTORIAL_02_JAINA_04_19");
				NotificationManager.Get().DestroyNotification(notification2, 3.5f);
				Gameplay.Get().StartCoroutine(this.DisplayManaReminder(GameStrings.Get("TUTORIAL02_HELP_03")));
			}
			return true;
		}
		if (error == PlayErrors.ErrorType.REQ_ATTACK_GREATER_THAN_0 && errorSource.GetCardId() == "TU4a_006")
		{
			return true;
		}
		if (error == PlayErrors.ErrorType.REQ_TARGET_TAUNTER)
		{
			SoundManager.Get().LoadAndPlay("UI_no_can_do");
			GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_TAUNT);
			GameState.Get().ShowEnemyTauntCharacters();
			this.HighlightTaunters();
			return true;
		}
		return false;
	}

	// Token: 0x06004816 RID: 18454 RVA: 0x0015A128 File Offset: 0x00158328
	private IEnumerator DisplayManaReminder(string reminderText)
	{
		yield return new WaitForSeconds(0.5f);
		if (this.manaReminder != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.manaReminder);
		}
		this.NotifyOfManaError();
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
			manaPopupPosition = new Vector3(manaPosition.x - 0.02f, manaPosition.y + 0.2f, manaPosition.z + 1.93f);
			direction = Notification.PopUpArrowDirection.Down;
		}
		this.manaReminder = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, manaPopupPosition, TutorialEntity.HELP_POPUP_SCALE, reminderText, true);
		this.manaReminder.ShowPopUpArrow(direction);
		NotificationManager.Get().DestroyNotification(this.manaReminder, 4f);
		yield break;
	}

	// Token: 0x06004817 RID: 18455 RVA: 0x0015A154 File Offset: 0x00158354
	private void HighlightTaunters()
	{
		foreach (Card card in GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards())
		{
			if (card.GetEntity().HasTaunt())
			{
				if (!card.GetEntity().IsStealthed())
				{
					NotificationManager.Get().DestroyAllPopUps();
					Vector3 position;
					position..ctor(card.transform.position.x - 2f, card.transform.position.y, card.transform.position.z);
					Notification notification = NotificationManager.Get().CreateFadeArrow(position, new Vector3(0f, 270f, 0f));
					NotificationManager.Get().DestroyNotification(notification, 3f);
					break;
				}
			}
		}
	}

	// Token: 0x06004818 RID: 18456 RVA: 0x0015A268 File Offset: 0x00158468
	public override bool NotifyOfTooltipDisplay(TooltipZone tooltip)
	{
		ZoneDeck component = tooltip.targetObject.GetComponent<ZoneDeck>();
		if (component == null)
		{
			return false;
		}
		if (component.m_Side == Player.Side.FRIENDLY)
		{
			string headline = GameStrings.Get("GAMEPLAY_TOOLTIP_DECK_HEADLINE");
			string bodytext = GameStrings.Get("TUTORIAL_TOOLTIP_DECK_DESCRIPTION");
			if (UniversalInputManager.UsePhoneUI)
			{
				tooltip.ShowGameplayTooltipLarge(headline, bodytext);
			}
			else
			{
				tooltip.ShowGameplayTooltip(headline, bodytext);
			}
			return true;
		}
		if (component.m_Side == Player.Side.OPPOSING)
		{
			string headline = GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYDECK_HEADLINE");
			string bodytext = GameStrings.Get("TUTORIAL_TOOLTIP_ENEMYDECK_DESC");
			if (UniversalInputManager.UsePhoneUI)
			{
				tooltip.ShowGameplayTooltipLarge(headline, bodytext);
			}
			else
			{
				tooltip.ShowGameplayTooltip(headline, bodytext);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06004819 RID: 18457 RVA: 0x0015A320 File Offset: 0x00158520
	public override void NotifyOfHeroesFinishedAnimatingInMulligan()
	{
		Collider collider = Board.Get().FindCollider("DragPlane");
		collider.GetComponent<Collider>().enabled = false;
		base.HandleMissionEvent(54);
	}

	// Token: 0x0600481A RID: 18458 RVA: 0x0015A351 File Offset: 0x00158551
	public override bool ShouldDoOpeningTaunts()
	{
		return false;
	}

	// Token: 0x0600481B RID: 18459 RVA: 0x0015A354 File Offset: 0x00158554
	public override bool ShouldHandleCoin()
	{
		return false;
	}

	// Token: 0x0600481C RID: 18460 RVA: 0x0015A358 File Offset: 0x00158558
	public override bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode)
	{
		if (!clickedEntity.IsControlledByLocalUser())
		{
			return true;
		}
		Network.Options.Option selectedNetworkOption = GameState.Get().GetSelectedNetworkOption();
		if (selectedNetworkOption == null || selectedNetworkOption.Main == null)
		{
			return true;
		}
		Entity entity = GameState.Get().GetEntity(selectedNetworkOption.Main.ID);
		if (!wasInTargetMode || entity == null)
		{
			return true;
		}
		if (clickedEntity == entity)
		{
			return true;
		}
		string cardId = entity.GetCardId();
		if (cardId == "CS2_022" || cardId == "CS2_029" || cardId == "CS2_034")
		{
			this.ShowDontHurtYourselfPopup(clickedEntity.GetCard().transform.position);
			return false;
		}
		return true;
	}

	// Token: 0x0600481D RID: 18461 RVA: 0x0015A410 File Offset: 0x00158610
	private void ShowDontHurtYourselfPopup(Vector3 origin)
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

	// Token: 0x0600481E RID: 18462 RVA: 0x0015A498 File Offset: 0x00158698
	protected void HandleGameStartEvent()
	{
		MulliganManager.Get().ForceMulliganActive(true);
		MulliganManager.Get().SkipCardChoosing();
		TurnStartManager.Get().BeginListeningForTurnEvents();
	}

	// Token: 0x0600481F RID: 18463 RVA: 0x0015A4C4 File Offset: 0x001586C4
	protected void UserPressedStartButton(UIEvent e)
	{
		base.HandleMissionEvent(55);
	}

	// Token: 0x06004820 RID: 18464 RVA: 0x0015A4D0 File Offset: 0x001586D0
	protected TutorialNotification ShowTutorialDialog(string headlineGameString, string bodyTextGameString, string buttonGameString, Vector2 materialOffset, bool swapMaterial = false)
	{
		return NotificationManager.Get().CreateTutorialDialog(headlineGameString, bodyTextGameString, buttonGameString, new UIEvent.Handler(this.UserPressedStartButton), materialOffset, swapMaterial);
	}

	// Token: 0x06004821 RID: 18465 RVA: 0x0015A4FC File Offset: 0x001586FC
	public override void NotifyOfHistoryTokenMousedOver(GameObject mousedOverTile)
	{
		this.historyTooltip = KeywordHelpPanelManager.Get().CreateKeywordPanel(0);
		this.historyTooltip.Reset();
		this.historyTooltip.Initialize(GameStrings.Get("TUTORIAL_TOOLTIP_HISTORY_HEADLINE"), GameStrings.Get("TUTORIAL_TOOLTIP_HISTORY_DESC"));
		Vector3 localPosition;
		if (UniversalInputManager.Get().IsTouchMode())
		{
			localPosition..ctor(1f, 0.1916952f, 1.2f);
		}
		else
		{
			localPosition..ctor(-1.140343f, 0.1916952f, 0.4895353f);
		}
		this.historyTooltip.transform.parent = mousedOverTile.GetComponent<HistoryCard>().m_mainCardActor.transform;
		float num = 0.4792188f;
		this.historyTooltip.transform.localPosition = localPosition;
		this.historyTooltip.transform.localScale = new Vector3(num, num, num);
	}

	// Token: 0x06004822 RID: 18466 RVA: 0x0015A5D3 File Offset: 0x001587D3
	public override void NotifyOfHistoryTokenMousedOut()
	{
		if (this.historyTooltip != null)
		{
			Object.Destroy(this.historyTooltip.gameObject);
		}
	}

	// Token: 0x06004823 RID: 18467 RVA: 0x0015A5F6 File Offset: 0x001587F6
	protected virtual void NotifyOfManaError()
	{
	}

	// Token: 0x06004824 RID: 18468 RVA: 0x0015A5F8 File Offset: 0x001587F8
	protected void SetTutorialProgress(TutorialProgress val)
	{
		if (GameMgr.Get().IsSpectator())
		{
			return;
		}
		if (!Network.ShouldBeConnectedToAurora())
		{
			if (GameUtils.AreAllTutorialsComplete(val))
			{
				Options.Get().SetBool(Option.CONNECT_TO_AURORA, true);
			}
			Options.Get().SetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS, val);
		}
		AdTrackingManager.Get().TrackTutorialProgress(val.ToString());
		NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
		netObject.CampaignProgress = val;
		NetCache.Get().NetCacheChanged<NetCache.NetCacheProfileProgress>();
	}

	// Token: 0x06004825 RID: 18469 RVA: 0x0015A678 File Offset: 0x00158878
	protected void SetTutorialLostProgress(TutorialProgress val)
	{
		int num = Options.Get().GetInt(Option.TUTORIAL_LOST_PROGRESS);
		num |= 1 << (int)val;
		Options.Get().SetInt(Option.TUTORIAL_LOST_PROGRESS, num);
	}

	// Token: 0x06004826 RID: 18470 RVA: 0x0015A6AC File Offset: 0x001588AC
	protected bool DidLoseTutorial(TutorialProgress val)
	{
		int @int = Options.Get().GetInt(Option.TUTORIAL_LOST_PROGRESS);
		bool result = false;
		if ((@int & 1 << (int)val) > 0)
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06004827 RID: 18471 RVA: 0x0015A6DA File Offset: 0x001588DA
	protected void ResetTutorialLostProgress()
	{
		Options.Get().SetInt(Option.TUTORIAL_LOST_PROGRESS, 0);
	}

	// Token: 0x04002F86 RID: 12166
	private Notification thatsABadPlayPopup;

	// Token: 0x04002F87 RID: 12167
	private Notification manaReminder;

	// Token: 0x04002F88 RID: 12168
	private KeywordHelpPanel historyTooltip;

	// Token: 0x04002F89 RID: 12169
	public static readonly Vector3 TUTORIAL_DIALOG_SCALE_PHONE = 1.5f * Vector3.one;

	// Token: 0x04002F8A RID: 12170
	public static readonly Vector3 HELP_POPUP_SCALE = 16f * Vector3.one;
}
