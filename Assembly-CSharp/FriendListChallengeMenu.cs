using System;
using UnityEngine;

// Token: 0x0200063D RID: 1597
public class FriendListChallengeMenu : MonoBehaviour
{
	// Token: 0x06004539 RID: 17721 RVA: 0x0014C794 File Offset: 0x0014A994
	private void Awake()
	{
		this.m_StandardDuelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnStandardDuelButtonReleased));
		this.m_WildDuelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnWildDuelButtonReleased));
		this.m_TavernBrawlButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTavernBrawlButtonReleased));
		if (!CollectionManager.Get().ShouldAccountSeeStandardWild())
		{
			this.m_StandardDuelButton.SetText("GLOBAL_FRIENDLIST_CHALLENGE_MENU_DUEL_BUTTON");
			this.m_TavernBrawlButton.gameObject.transform.position = this.m_WildDuelButton.gameObject.transform.position;
			this.m_WildDuelButton.gameObject.SetActive(false);
			this.m_MiddleFrame.transform.localScale = new Vector3(this.m_MiddleFrame.transform.localScale.x, 0.75f, this.m_MiddleFrame.transform.localScale.z);
			this.m_MiddleShadow.transform.localScale = new Vector3(this.m_MiddleShadow.transform.localScale.x, 0.2f, this.m_MiddleShadow.transform.localScale.z);
			this.m_FrameContainer.UpdateSlices();
			this.m_ShadowContainer.UpdateSlices();
		}
		this.m_bHasStandardDeck = CollectionManager.Get().AccountHasValidStandardDeck();
		this.m_bHasWildDeck = CollectionManager.Get().AccountHasAnyValidDeck();
		this.m_bIsTavernBrawlActive = TavernBrawlManager.Get().IsTavernBrawlActive;
		this.m_bIsTavernBrawlUnlocked = TavernBrawlManager.Get().HasUnlockedTavernBrawl;
		if (this.m_bIsTavernBrawlActive)
		{
			TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
			this.m_bHasTavernBrawlDeck = (!tavernBrawlMission.canCreateDeck || TavernBrawlManager.Get().HasValidDeck());
		}
		else
		{
			this.m_TavernBrawlButtonDisabled.SetActive(true);
			UIBHighlight component = this.m_TavernBrawlButton.gameObject.GetComponent<UIBHighlight>();
			component.m_MouseOverHighlight = null;
			this.m_TavernBrawlButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTavernBrawlButtonReleased));
			this.m_TavernBrawlButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTavernBrawlButtonOver));
			this.m_TavernBrawlButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnButtonOut));
		}
		if (!CollectionManager.Get().AreAllDeckContentsReady() && CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded(new CollectionManager.DelOnAllDeckContents(this.OnDeckContents_UpdateButtons)))
		{
			this.m_bHasStandardDeck = true;
			this.m_bHasWildDeck = true;
		}
		this.UpdateButtons();
	}

	// Token: 0x0600453A RID: 17722 RVA: 0x0014CA20 File Offset: 0x0014AC20
	private void UpdateButtons()
	{
		this.m_StandardDuelButtonX.SetActive(!this.m_bHasStandardDeck);
		this.m_WildDuelButtonX.SetActive(!this.m_bHasWildDeck);
		this.m_TavernBrawlButtonX.SetActive(this.m_bIsTavernBrawlActive && (!this.m_bIsTavernBrawlUnlocked || !this.m_bHasTavernBrawlDeck));
	}

	// Token: 0x0600453B RID: 17723 RVA: 0x0014CA85 File Offset: 0x0014AC85
	private void OnDeckContents_UpdateButtons()
	{
		this.m_bHasStandardDeck = CollectionManager.Get().AccountHasValidStandardDeck();
		this.m_bHasWildDeck = CollectionManager.Get().AccountHasAnyValidDeck();
		this.UpdateButtons();
	}

	// Token: 0x0600453C RID: 17724 RVA: 0x0014CAB0 File Offset: 0x0014ACB0
	private void OnStandardDuelButtonOver(UIEvent e)
	{
		string text = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
		string text2 = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_AVAILABLE";
		if (UniversalInputManager.Get().IsTouchMode())
		{
			if (GameStrings.HasKey(text + "_TOUCH"))
			{
				text += "_TOUCH";
			}
			if (GameStrings.HasKey(text2 + "_TOUCH"))
			{
				text2 += "_TOUCH";
			}
		}
		this.ShowTooltip(text, text2, this.m_StandardDuelTooltipZone, this.m_StandardDuelButton);
	}

	// Token: 0x0600453D RID: 17725 RVA: 0x0014CB30 File Offset: 0x0014AD30
	private void OnWildDuelButtonOver(UIEvent e)
	{
		string text = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
		string text2 = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_AVAILABLE";
		if (UniversalInputManager.Get().IsTouchMode())
		{
			if (GameStrings.HasKey(text + "_TOUCH"))
			{
				text += "_TOUCH";
			}
			if (GameStrings.HasKey(text2 + "_TOUCH"))
			{
				text2 += "_TOUCH";
			}
		}
		this.ShowTooltip(text, text2, this.m_WildDuelTooltipZone, this.m_WildDuelButton);
	}

	// Token: 0x0600453E RID: 17726 RVA: 0x0014CBB0 File Offset: 0x0014ADB0
	private void OnTavernBrawlButtonOver(UIEvent e)
	{
		string text = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
		string text2 = "GLOBAL_FRIENDLIST_CHALLENGE_TOOLTIP_NO_TAVERN_BRAWL";
		if (UniversalInputManager.Get().IsTouchMode())
		{
			if (GameStrings.HasKey(text + "_TOUCH"))
			{
				text += "_TOUCH";
			}
			if (GameStrings.HasKey(text2 + "_TOUCH"))
			{
				text2 += "_TOUCH";
			}
		}
		this.ShowTooltip(text, text2, this.m_TavernBrawlTooltipZone, this.m_TavernBrawlButton);
	}

	// Token: 0x0600453F RID: 17727 RVA: 0x0014CC2E File Offset: 0x0014AE2E
	private void OnButtonOut(UIEvent e)
	{
		this.HideTooltip();
	}

	// Token: 0x06004540 RID: 17728 RVA: 0x0014CC38 File Offset: 0x0014AE38
	private void OnStandardDuelButtonReleased(UIEvent e)
	{
		if (!this.m_bHasStandardDeck)
		{
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER");
			popupInfo.m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_STANDARD_DECK", new object[0]);
			popupInfo.m_showAlertIcon = true;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			popupInfo.m_layerToUse = new GameLayer?(GameLayer.HighPriorityUI);
			DialogManager.Get().ShowPopup(popupInfo);
		}
		else
		{
			BnetPlayer player = base.GetComponentInParent<FriendListChallengeButton>().GetPlayer();
			FriendChallengeMgr.Get().SendChallenge(player, 2, false);
		}
	}

	// Token: 0x06004541 RID: 17729 RVA: 0x0014CCC0 File Offset: 0x0014AEC0
	private void OnWildDuelButtonReleased(UIEvent e)
	{
		if (!this.m_bHasWildDeck)
		{
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER");
			popupInfo.m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_DECK", new object[0]);
			popupInfo.m_showAlertIcon = true;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			popupInfo.m_layerToUse = new GameLayer?(GameLayer.HighPriorityUI);
			DialogManager.Get().ShowPopup(popupInfo);
		}
		else
		{
			BnetPlayer player = base.GetComponentInParent<FriendListChallengeButton>().GetPlayer();
			FriendChallengeMgr.Get().SendChallenge(player, 1, false);
		}
	}

	// Token: 0x06004542 RID: 17730 RVA: 0x0014CD48 File Offset: 0x0014AF48
	private void OnTavernBrawlButtonReleased(UIEvent e)
	{
		if (!this.m_bIsTavernBrawlUnlocked)
		{
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER");
			popupInfo.m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_TAVERN_BRAWL_LOCKED", new object[0]);
			popupInfo.m_showAlertIcon = true;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			popupInfo.m_layerToUse = new GameLayer?(GameLayer.HighPriorityUI);
			DialogManager.Get().ShowPopup(popupInfo);
		}
		else if (!this.m_bIsTavernBrawlActive)
		{
			AlertPopup.PopupInfo popupInfo2 = new AlertPopup.PopupInfo();
			popupInfo2.m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER");
			popupInfo2.m_text = GameStrings.Format("GLOBAL_TAVERN_BRAWL_ERROR_SEASON_INCREMENTED", new object[0]);
			popupInfo2.m_showAlertIcon = true;
			popupInfo2.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			popupInfo2.m_layerToUse = new GameLayer?(GameLayer.HighPriorityUI);
			DialogManager.Get().ShowPopup(popupInfo2);
		}
		else if (!this.m_bHasTavernBrawlDeck)
		{
			FriendChallengeMgr.ShowChallengerNeedsToCreateTavernBrawlDeckAlert();
		}
		else
		{
			BnetPlayer player = base.GetComponentInParent<FriendListChallengeButton>().GetPlayer();
			FriendChallengeMgr.Get().SendChallenge(player, 1, true);
		}
	}

	// Token: 0x06004543 RID: 17731 RVA: 0x0014CE48 File Offset: 0x0014B048
	private void ShowTooltip(string headerKey, string descriptionFormat, TooltipZone tooltipZone, UIBButton button)
	{
		string headline = GameStrings.Get(headerKey);
		BnetPlayer player = base.GetComponentInParent<FriendListChallengeButton>().GetPlayer();
		string bodytext = GameStrings.Format(descriptionFormat, new object[]
		{
			player.GetBestName()
		});
		this.HideTooltip();
		tooltipZone.ShowSocialTooltip(button, headline, bodytext, 75f, GameLayer.BattleNetDialog);
		tooltipZone.AnchorTooltipTo(button.gameObject, Anchor.TOP_RIGHT, Anchor.TOP_LEFT);
	}

	// Token: 0x06004544 RID: 17732 RVA: 0x0014CEA4 File Offset: 0x0014B0A4
	private void UpdateTooltip()
	{
	}

	// Token: 0x06004545 RID: 17733 RVA: 0x0014CEA8 File Offset: 0x0014B0A8
	private void HideTooltip()
	{
		if (this.m_StandardDuelTooltipZone != null)
		{
			this.m_StandardDuelTooltipZone.HideTooltip();
		}
		if (this.m_WildDuelTooltipZone != null)
		{
			this.m_WildDuelTooltipZone.HideTooltip();
		}
		if (this.m_TavernBrawlTooltipZone != null)
		{
			this.m_TavernBrawlTooltipZone.HideTooltip();
		}
	}

	// Token: 0x04002C0F RID: 11279
	public UIBButton m_StandardDuelButton;

	// Token: 0x04002C10 RID: 11280
	public UIBButton m_WildDuelButton;

	// Token: 0x04002C11 RID: 11281
	public UIBButton m_TavernBrawlButton;

	// Token: 0x04002C12 RID: 11282
	public TooltipZone m_StandardDuelTooltipZone;

	// Token: 0x04002C13 RID: 11283
	public TooltipZone m_WildDuelTooltipZone;

	// Token: 0x04002C14 RID: 11284
	public TooltipZone m_TavernBrawlTooltipZone;

	// Token: 0x04002C15 RID: 11285
	public GameObject m_StandardDuelButtonX;

	// Token: 0x04002C16 RID: 11286
	public GameObject m_WildDuelButtonX;

	// Token: 0x04002C17 RID: 11287
	public GameObject m_TavernBrawlButtonX;

	// Token: 0x04002C18 RID: 11288
	public GameObject m_TavernBrawlButtonDisabled;

	// Token: 0x04002C19 RID: 11289
	public MultiSliceElement m_FrameContainer;

	// Token: 0x04002C1A RID: 11290
	public MultiSliceElement m_ShadowContainer;

	// Token: 0x04002C1B RID: 11291
	public GameObject m_MiddleFrame;

	// Token: 0x04002C1C RID: 11292
	public GameObject m_MiddleShadow;

	// Token: 0x04002C1D RID: 11293
	private bool m_bHasStandardDeck;

	// Token: 0x04002C1E RID: 11294
	private bool m_bHasWildDeck;

	// Token: 0x04002C1F RID: 11295
	private bool m_bHasTavernBrawlDeck;

	// Token: 0x04002C20 RID: 11296
	private bool m_bIsTavernBrawlActive;

	// Token: 0x04002C21 RID: 11297
	private bool m_bIsTavernBrawlUnlocked;
}
