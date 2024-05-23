using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E5 RID: 741
public class Tutorial_01 : TutorialEntity
{
	// Token: 0x0600276C RID: 10092 RVA: 0x000BF5E8 File Offset: 0x000BD7E8
	public Tutorial_01()
	{
		MulliganManager.Get().ForceMulliganActive(true);
	}

	// Token: 0x0600276D RID: 10093 RVA: 0x000BF708 File Offset: 0x000BD908
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_TUTORIAL_01_ANNOUNCER_01");
		base.PreloadSound("VO_TUTORIAL_01_ANNOUNCER_02");
		base.PreloadSound("VO_TUTORIAL_01_ANNOUNCER_03");
		base.PreloadSound("VO_TUTORIAL_01_ANNOUNCER_04");
		base.PreloadSound("VO_TUTORIAL_01_ANNOUNCER_05");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_13_10");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_01_01");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_02_02");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_03_03");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_20_16");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_05_05");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_06_06");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_07_07");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_21_17");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_09_08");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_15_11");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_16_12");
		base.PreloadSound("VO_TUTORIAL_JAINA_02_55_ALT2");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_10_09");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_17_13");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_18_14");
		base.PreloadSound("VO_TUTORIAL_01_JAINA_19_15");
		base.PreloadSound("VO_TUTORIAL_01_HOGGER_01_01");
		base.PreloadSound("VO_TUTORIAL_01_HOGGER_02_02");
		base.PreloadSound("VO_TUTORIAL_01_HOGGER_03_03");
		base.PreloadSound("VO_TUTORIAL_01_HOGGER_04_04");
		base.PreloadSound("VO_TUTORIAL_01_HOGGER_06_06_ALT");
		base.PreloadSound("VO_TUTORIAL_01_HOGGER_08_08_ALT");
		base.PreloadSound("VO_TUTORIAL_01_HOGGER_09_09_ALT");
		base.PreloadSound("VO_TUTORIAL_01_HOGGER_11_11");
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x000BF860 File Offset: 0x000BDA60
	public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
	{
		base.NotifyOfGameOver(gameResult);
		if (this.attackHelpPanel != null)
		{
			Object.Destroy(this.attackHelpPanel.gameObject);
			this.attackHelpPanel = null;
		}
		if (this.healthHelpPanel != null)
		{
			Object.Destroy(this.healthHelpPanel.gameObject);
			this.healthHelpPanel = null;
		}
		this.EnsureCardGemsAreOnTheCorrectLayer();
		if (gameResult == TAG_PLAYSTATE.WON)
		{
			base.SetTutorialProgress(TutorialProgress.HOGGER_COMPLETE);
			base.PlaySound("VO_TUTORIAL_01_HOGGER_11_11", 1f, true, false);
		}
		else if (gameResult == TAG_PLAYSTATE.TIED)
		{
			base.PlaySound("VO_TUTORIAL_01_HOGGER_11_11", 1f, true, false);
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			InputManager.Get().RemovePhoneHandShownListener(new InputManager.PhoneHandShownCallback(this.OnPhoneHandShown));
			InputManager.Get().RemovePhoneHandHiddenListener(new InputManager.PhoneHandHiddenCallback(this.OnPhoneHandHidden));
		}
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x000BF948 File Offset: 0x000BDB48
	private void EnsureCardGemsAreOnTheCorrectLayer()
	{
		List<Card> list = new List<Card>();
		list.AddRange(GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards());
		list.AddRange(GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards());
		list.Add(GameState.Get().GetFriendlySidePlayer().GetHeroCard());
		list.Add(GameState.Get().GetOpposingSidePlayer().GetHeroCard());
		foreach (Card card in list)
		{
			if (!(card == null) && !(card.GetActor() == null))
			{
				if (card.GetActor().GetAttackObject() != null)
				{
					SceneUtils.SetLayer(card.GetActor().GetAttackObject().gameObject, GameLayer.Default);
				}
				if (card.GetActor().GetHealthObject() != null)
				{
					SceneUtils.SetLayer(card.GetActor().GetHealthObject().gameObject, GameLayer.Default);
				}
			}
		}
	}

	// Token: 0x06002770 RID: 10096 RVA: 0x000BFA74 File Offset: 0x000BDC74
	public override void NotifyOfCardGrabbed(Entity entity)
	{
		if (base.GetTag(GAME_TAG.TURN) == 2 || entity.GetCardId() == "CS2_025")
		{
			BoardTutorial.Get().EnableHighlight(true);
			if (base.GetTag(GAME_TAG.TURN) == 2)
			{
			}
		}
		this.NukeNumberLabels();
	}

	// Token: 0x06002771 RID: 10097 RVA: 0x000BFAC4 File Offset: 0x000BDCC4
	public override void NotifyOfCardDropped(Entity entity)
	{
		if (base.GetTag(GAME_TAG.TURN) == 2 || entity.GetCardId() == "CS2_025")
		{
			BoardTutorial.Get().EnableHighlight(false);
		}
	}

	// Token: 0x06002772 RID: 10098 RVA: 0x000BFB00 File Offset: 0x000BDD00
	public override bool NotifyOfEndTurnButtonPushed()
	{
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
		string key = "TUTORIAL_NO_ENDTURN_ATK";
		if (!GameState.Get().GetFriendlySidePlayer().HasReadyAttackers())
		{
			key = "TUTORIAL_NO_ENDTURN";
		}
		this.endTurnNotifier = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get(key), true);
		NotificationManager.Get().DestroyNotification(this.endTurnNotifier, 2.5f);
		return false;
	}

	// Token: 0x06002773 RID: 10099 RVA: 0x000BFBF0 File Offset: 0x000BDDF0
	public override bool NotifyOfPlayError(PlayErrors.ErrorType error, Entity errorSource)
	{
		return error == PlayErrors.ErrorType.REQ_ATTACK_GREATER_THAN_0 && errorSource.GetCardId() == "TU4a_006";
	}

	// Token: 0x06002774 RID: 10100 RVA: 0x000BFC20 File Offset: 0x000BDE20
	public override void NotifyOfTargetModeCancelled()
	{
		if (this.crushThisGnoll == null)
		{
			return;
		}
		NotificationManager.Get().DestroyAllPopUps();
		if (this.firstRaptorCard == null || !(this.firstRaptorCard.GetZone() is ZonePlay))
		{
			return;
		}
		this.ShowAttackWithYourMinionPopup();
	}

	// Token: 0x06002775 RID: 10101 RVA: 0x000BFC78 File Offset: 0x000BDE78
	public override bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode)
	{
		if (base.GetTag(GAME_TAG.TURN) == 4)
		{
			if (clickedEntity.GetCardId() == "CS2_168")
			{
				if (!wasInTargetMode && !this.firstAttackFinished)
				{
					if (this.crushThisGnoll != null)
					{
						NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.crushThisGnoll);
					}
					NotificationManager.Get().DestroyAllPopUps();
					Vector3 position = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetFirstCard().transform.position;
					Vector3 position2;
					position2..ctor(position.x - 3f, position.y, position.z);
					this.crushThisGnoll = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL01_HELP_03"), true);
					this.crushThisGnoll.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
					this.numTimesTextSwapStarted++;
					Gameplay.Get().StartCoroutine(this.WaitAndThenHide(this.numTimesTextSwapStarted));
				}
			}
			else if (clickedEntity.GetCardId() == "TU4a_002" && wasInTargetMode)
			{
				if (this.crushThisGnoll != null)
				{
					NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.crushThisGnoll);
				}
				NotificationManager.Get().DestroyAllPopUps();
				this.firstAttackFinished = true;
			}
		}
		else if (base.GetTag(GAME_TAG.TURN) == 6 && clickedEntity.GetCardId() == "TU4a_001" && wasInTargetMode)
		{
			NotificationManager.Get().DestroyAllPopUps();
		}
		if (wasInTargetMode && InputManager.Get().GetHeldCard() != null && InputManager.Get().GetHeldCard().GetEntity().GetCardId() == "CS2_029")
		{
			if (clickedEntity.IsControlledByLocalUser())
			{
				this.ShowDontFireballYourselfPopup(clickedEntity.GetCard().transform.position);
				return false;
			}
			if (clickedEntity.GetCardId() == "TU4a_003" && base.GetTag(GAME_TAG.TURN) >= 8)
			{
				if (this.noFireballPopup != null)
				{
					NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.noFireballPopup);
				}
				Vector3 position3 = clickedEntity.GetCard().transform.position;
				Vector3 position4;
				position4..ctor(position3.x - 3f, position3.y, position3.z);
				this.noFireballPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position4, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL01_HELP_08"), true);
				NotificationManager.Get().DestroyNotification(this.noFireballPopup, 3f);
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002776 RID: 10102 RVA: 0x000BFF14 File Offset: 0x000BE114
	private IEnumerator WaitAndThenHide(int numTimesStarted)
	{
		yield return new WaitForSeconds(6f);
		if (this.crushThisGnoll == null)
		{
			yield break;
		}
		if (numTimesStarted != this.numTimesTextSwapStarted)
		{
			yield break;
		}
		Card firstCard = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetFirstCard();
		if (firstCard == null)
		{
			yield break;
		}
		NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.crushThisGnoll);
		yield break;
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x000BFF40 File Offset: 0x000BE140
	public override bool NotifyOfCardTooltipDisplayShow(Card card)
	{
		if (GameState.Get().IsGameOver())
		{
			return false;
		}
		Entity entity = card.GetEntity();
		if (entity.IsMinion())
		{
			if (this.attackHelpPanel == null)
			{
				this.m_isShowingAttackHelpPanel = true;
				this.ShowAttackTooltip(card);
				Gameplay.Get().StartCoroutine(this.ShowHealthTooltipAfterWait(card));
			}
			return false;
		}
		if (entity.IsHero())
		{
			if (this.healthHelpPanel == null)
			{
				this.ShowHealthTooltip(card);
			}
			return false;
		}
		return true;
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x000BFFCC File Offset: 0x000BE1CC
	private void ShowAttackTooltip(Card card)
	{
		SceneUtils.SetLayer(card.GetActor().GetAttackObject().gameObject, GameLayer.Tooltip);
		Vector3 position = card.transform.position;
		Vector3 vector = this.m_attackTooltipPosition;
		Vector3 position2;
		position2..ctor(position.x + vector.x, position.y + vector.y, position.z + vector.z);
		this.attackHelpPanel = KeywordHelpPanelManager.Get().CreateKeywordPanel(0);
		this.attackHelpPanel.Reset();
		this.attackHelpPanel.SetScale(KeywordHelpPanel.GAMEPLAY_SCALE);
		this.attackHelpPanel.Initialize(GameStrings.Get("GLOBAL_ATTACK"), GameStrings.Get("TUTORIAL01_HELP_12"));
		this.attackHelpPanel.transform.position = position2;
		RenderUtils.SetAlpha(this.attackHelpPanel.gameObject, 0f);
		iTween.FadeTo(this.attackHelpPanel.gameObject, iTween.Hash(new object[]
		{
			"alpha",
			1,
			"time",
			0.25f
		}));
		card.GetActor().GetAttackObject().Enlarge(this.m_gemScale);
	}

	// Token: 0x06002779 RID: 10105 RVA: 0x000C0110 File Offset: 0x000BE310
	private IEnumerator ShowHealthTooltipAfterWait(Card card)
	{
		yield return new WaitForSeconds(0.05f);
		if (InputManager.Get().GetMousedOverCard() != card)
		{
			yield break;
		}
		this.ShowHealthTooltip(card);
		yield break;
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000C013C File Offset: 0x000BE33C
	private void ShowHealthTooltip(Card card)
	{
		SceneUtils.SetLayer(card.GetActor().GetHealthObject().gameObject, GameLayer.Tooltip);
		Vector3 position = card.transform.position;
		Vector3 vector = this.m_healthTooltipPosition;
		if (card.GetEntity().IsHero())
		{
			vector = this.m_heroHealthTooltipPosition;
			if (UniversalInputManager.UsePhoneUI)
			{
				if (!card.GetEntity().IsControlledByLocalUser())
				{
					vector.z -= 0.75f;
				}
				else if (Localization.GetLocale() == Locale.ruRU)
				{
					vector.z += 1f;
				}
			}
		}
		Vector3 position2;
		position2..ctor(position.x + vector.x, position.y + vector.y, position.z + vector.z);
		this.healthHelpPanel = KeywordHelpPanelManager.Get().CreateKeywordPanel(0);
		this.healthHelpPanel.Reset();
		this.healthHelpPanel.SetScale(KeywordHelpPanel.GAMEPLAY_SCALE);
		this.healthHelpPanel.Initialize(GameStrings.Get("GLOBAL_HEALTH"), GameStrings.Get("TUTORIAL01_HELP_13"));
		this.healthHelpPanel.transform.position = position2;
		RenderUtils.SetAlpha(this.healthHelpPanel.gameObject, 0f);
		iTween.FadeTo(this.healthHelpPanel.gameObject, iTween.Hash(new object[]
		{
			"alpha",
			1,
			"time",
			0.25f
		}));
		card.GetActor().GetHealthObject().Enlarge(this.m_gemScale);
	}

	// Token: 0x0600277B RID: 10107 RVA: 0x000C02F0 File Offset: 0x000BE4F0
	public override void NotifyOfCardTooltipDisplayHide(Card card)
	{
		if (this.attackHelpPanel != null)
		{
			if (card != null)
			{
				GemObject attackObject = card.GetActor().GetAttackObject();
				SceneUtils.SetLayer(attackObject.gameObject, GameLayer.Default);
				attackObject.Shrink();
			}
			Object.Destroy(this.attackHelpPanel.gameObject);
			this.m_isShowingAttackHelpPanel = false;
		}
		if (this.healthHelpPanel != null)
		{
			if (card != null)
			{
				GemObject healthObject = card.GetActor().GetHealthObject();
				SceneUtils.SetLayer(healthObject.gameObject, GameLayer.Default);
				healthObject.Shrink();
			}
			Object.Destroy(this.healthHelpPanel.gameObject);
		}
	}

	// Token: 0x0600277C RID: 10108 RVA: 0x000C039C File Offset: 0x000BE59C
	private void ManaLabelLoadedCallback(string actorName, GameObject actorObject, object callbackData)
	{
		if (this.m_isShowingAttackHelpPanel)
		{
			return;
		}
		Card card = (Card)callbackData;
		GameObject costTextObject = card.GetActor().GetCostTextObject();
		if (costTextObject == null)
		{
			Object.Destroy(actorObject);
			return;
		}
		this.costLabel = actorObject;
		actorObject.transform.parent = costTextObject.transform;
		actorObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		actorObject.transform.localPosition = new Vector3(-0.017f, 0.3512533f, 0f);
		actorObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		actorObject.GetComponent<UberText>().Text = GameStrings.Get("GLOBAL_COST");
	}

	// Token: 0x0600277D RID: 10109 RVA: 0x000C0468 File Offset: 0x000BE668
	private void AttackLabelLoadedCallback(string actorName, GameObject actorObject, object callbackData)
	{
		if (this.m_isShowingAttackHelpPanel)
		{
			return;
		}
		Card card = (Card)callbackData;
		GameObject attackTextObject = card.GetActor().GetAttackTextObject();
		if (attackTextObject == null)
		{
			Object.Destroy(actorObject);
			return;
		}
		this.attackLabel = actorObject;
		actorObject.transform.parent = attackTextObject.transform;
		actorObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		actorObject.transform.localPosition = new Vector3(-0.2f, -0.3039344f, 0f);
		actorObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		actorObject.GetComponent<UberText>().Text = GameStrings.Get("GLOBAL_ATTACK");
	}

	// Token: 0x0600277E RID: 10110 RVA: 0x000C0534 File Offset: 0x000BE734
	private void HealthLabelLoadedCallback(string actorName, GameObject actorObject, object callbackData)
	{
		if (this.m_isShowingAttackHelpPanel)
		{
			return;
		}
		Card card = (Card)callbackData;
		GameObject healthTextObject = card.GetActor().GetHealthTextObject();
		if (healthTextObject == null)
		{
			Object.Destroy(actorObject);
			return;
		}
		this.healthLabel = actorObject;
		actorObject.transform.parent = healthTextObject.transform;
		actorObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		actorObject.transform.localPosition = new Vector3(0.21f, -0.31f, 0f);
		actorObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		actorObject.GetComponent<UberText>().Text = GameStrings.Get("GLOBAL_HEALTH");
	}

	// Token: 0x0600277F RID: 10111 RVA: 0x000C0600 File Offset: 0x000BE800
	public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
	{
		if (this.ShouldShowArrowOnCardInHand(mousedOverEntity))
		{
			NotificationManager.Get().DestroyAllArrows();
		}
		if (mousedOverEntity.GetZone() == TAG_ZONE.HAND)
		{
			this.mousedOverCard = mousedOverEntity.GetCard();
			AssetLoader.Get().LoadActor("NumberLabel", new AssetLoader.GameObjectCallback(this.ManaLabelLoadedCallback), this.mousedOverCard, false);
			AssetLoader.Get().LoadActor("NumberLabel", new AssetLoader.GameObjectCallback(this.AttackLabelLoadedCallback), this.mousedOverCard, false);
			AssetLoader.Get().LoadActor("NumberLabel", new AssetLoader.GameObjectCallback(this.HealthLabelLoadedCallback), this.mousedOverCard, false);
		}
	}

	// Token: 0x06002780 RID: 10112 RVA: 0x000C06A4 File Offset: 0x000BE8A4
	public override void NotifyOfCardMousedOff(Entity mousedOffEntity)
	{
		if (this.ShouldShowArrowOnCardInHand(mousedOffEntity))
		{
			Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(0.5f));
		}
		this.NukeNumberLabels();
	}

	// Token: 0x06002781 RID: 10113 RVA: 0x000C06DC File Offset: 0x000BE8DC
	private void NukeNumberLabels()
	{
		this.mousedOverCard = null;
		if (this.costLabel != null)
		{
			Object.Destroy(this.costLabel);
		}
		if (this.attackLabel != null)
		{
			Object.Destroy(this.attackLabel);
		}
		if (this.healthLabel != null)
		{
			Object.Destroy(this.healthLabel);
		}
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x000C0744 File Offset: 0x000BE944
	private bool ShouldShowArrowOnCardInHand(Entity entity)
	{
		if (entity.GetZone() != TAG_ZONE.HAND)
		{
			return false;
		}
		int tag = base.GetTag(GAME_TAG.TURN);
		return tag == 2 || (tag == 4 && GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count == 0);
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x000C079C File Offset: 0x000BE99C
	private IEnumerator ShowArrowInSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		List<Card> handCards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
		if (handCards.Count == 0)
		{
			yield break;
		}
		Card cardInHand = handCards[0];
		while (iTween.Count(cardInHand.gameObject) > 0)
		{
			yield return null;
		}
		if (cardInHand.IsMousedOver())
		{
			yield break;
		}
		if (InputManager.Get().GetHeldCard() == cardInHand)
		{
			yield break;
		}
		this.ShowHandBouncingArrow();
		yield break;
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x000C07C8 File Offset: 0x000BE9C8
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
		Card card = cards[0];
		Vector3 position = card.transform.position;
		Vector3 position2;
		if (UniversalInputManager.UsePhoneUI)
		{
			position2..ctor(position.x - 0.08f, position.y + 0.2f, position.z + 1.2f);
		}
		else
		{
			position2..ctor(position.x, position.y, position.z + 2f);
		}
		this.handBounceArrow = NotificationManager.Get().CreateBouncingArrow(UserAttentionBlocker.NONE, position2, new Vector3(0f, 0f, 0f));
		this.handBounceArrow.transform.parent = card.transform;
	}

	// Token: 0x06002785 RID: 10117 RVA: 0x000C08BC File Offset: 0x000BEABC
	private void ShowHandFadeArrow()
	{
		List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
		if (cards.Count == 0)
		{
			return;
		}
		this.ShowFadeArrow(cards[0], null);
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x000C08F8 File Offset: 0x000BEAF8
	private void ShowFadeArrow(Card card, Card target = null)
	{
		if (this.handFadeArrow != null)
		{
			return;
		}
		Vector3 position = card.transform.position;
		Vector3 rotation;
		rotation..ctor(0f, 180f, 0f);
		Vector3 vector2;
		if (target != null)
		{
			Vector3 vector = target.transform.position - position;
			vector2..ctor(position.x, position.y + 0.47f, position.z + 0.27f);
			float num = Vector3.Angle(target.transform.position - vector2, new Vector3(0f, 0f, -1f));
			rotation..ctor(0f, -Mathf.Sign(vector.x) * num, 0f);
			vector2 += 0.3f * vector;
		}
		else
		{
			vector2..ctor(position.x, position.y + 0.047f, position.z + 0.95f);
		}
		this.handFadeArrow = NotificationManager.Get().CreateFadeArrow(vector2, rotation);
		if (target != null)
		{
			this.handFadeArrow.transform.localScale = 1.25f * Vector3.one;
		}
		this.handFadeArrow.transform.parent = card.transform;
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x000C0A5E File Offset: 0x000BEC5E
	private void HideFadeArrow()
	{
		if (this.handFadeArrow != null)
		{
			NotificationManager.Get().DestroyNotification(this.handFadeArrow, 0f);
			this.handFadeArrow = null;
		}
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x000C0A8D File Offset: 0x000BEC8D
	private void OnPhoneHandShown(object userData)
	{
		if (this.handBounceArrow != null)
		{
			NotificationManager.Get().DestroyNotification(this.handBounceArrow, 0f);
			this.handBounceArrow = null;
		}
		this.ShowHandFadeArrow();
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x000C0AC2 File Offset: 0x000BECC2
	private void OnPhoneHandHidden(object userData)
	{
		this.HideFadeArrow();
		this.ShowHandBouncingArrow();
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x000C0AD0 File Offset: 0x000BECD0
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		Actor hoggerActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		switch (turn)
		{
		case 1:
		{
			List<Card> cardsInDeck = GameState.Get().GetFriendlySidePlayer().GetDeckZone().GetCards();
			this.firstMurlocCard = cardsInDeck[cardsInDeck.Count - 1];
			this.firstRaptorCard = cardsInDeck[cardsInDeck.Count - 2];
			GameState.Get().SetBusy(true);
			Collider dragPlane = Board.Get().FindCollider("DragPlane");
			dragPlane.enabled = false;
			yield return new WaitForSeconds(1.25f);
			TutorialNotification notification = base.ShowTutorialDialog("TUTORIAL01_HELP_14", "TUTORIAL01_HELP_15", "TUTORIAL01_HELP_16", Vector2.zero, false);
			notification.SetWantedText(GameStrings.Get("MISSION_PRE_TUTORIAL_WANTED"));
			break;
		}
		case 2:
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				InputManager.Get().RegisterPhoneHandShownListener(new InputManager.PhoneHandShownCallback(this.OnPhoneHandShown));
				InputManager.Get().RegisterPhoneHandHiddenListener(new InputManager.PhoneHandHiddenCallback(this.OnPhoneHandHidden));
			}
			yield return new WaitForSeconds(1f);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_02_02", "TUTORIAL01_JAINA_02", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			List<Card> cardsInHand = GameState.Get().GetFriendlySidePlayer().GetHandZone().GetCards();
			if (base.GetTag(GAME_TAG.TURN) == 2 && cardsInHand.Count == 1 && InputManager.Get().GetHeldCard() == null && !cardsInHand[0].IsMousedOver())
			{
				Gameplay.Get().StartCoroutine(this.ShowArrowInSeconds(0f));
			}
			break;
		}
		case 3:
			if (UniversalInputManager.UsePhoneUI)
			{
				InputManager.Get().RemovePhoneHandShownListener(new InputManager.PhoneHandShownCallback(this.OnPhoneHandShown));
				InputManager.Get().RemovePhoneHandHiddenListener(new InputManager.PhoneHandHiddenCallback(this.OnPhoneHandHidden));
			}
			break;
		case 4:
			hoggerActor.TurnOffCollider();
			hoggerActor.SetActorState(ActorStateType.CARD_IDLE);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_06_06", "TUTORIAL01_JAINA_06", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			if (this.firstMurlocCard != null)
			{
				this.firstMurlocCard.GetActor().ToggleForceIdle(true);
				this.firstMurlocCard.GetActor().SetActorState(ActorStateType.CARD_IDLE);
			}
			break;
		case 5:
			hoggerActor.TurnOnCollider();
			break;
		case 6:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_17_13", "TUTORIAL01_JAINA_17", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			break;
		case 8:
			this.m_jainaSpeaking = true;
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_18_14", "TUTORIAL01_JAINA_18", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			this.m_jainaSpeaking = false;
			yield return new WaitForSeconds(1f);
			Gameplay.Get().StartCoroutine(this.FlashMinionUntilAttackBegins(this.firstRaptorCard));
			break;
		case 10:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_19_15", "TUTORIAL01_JAINA_19", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			break;
		}
		yield break;
	}

	// Token: 0x0600278B RID: 10123 RVA: 0x000C0AFC File Offset: 0x000BECFC
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor jainaActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
		Actor hoggerActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		switch (missionEvent)
		{
		case 1:
			GameState.Get().SetBusy(true);
			HistoryManager.Get().DisableHistory();
			break;
		case 2:
			GameState.Get().SetBusy(true);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_01_01", "TUTORIAL01_JAINA_01", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			Gameplay.Get().SetGameStateBusy(false, 2.2f);
			break;
		case 3:
		{
			int turn = GameState.Get().GetTurn();
			yield return new WaitForSeconds(2f);
			if (turn != GameState.Get().GetTurn())
			{
				yield break;
			}
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_03_03", "TUTORIAL01_JAINA_03", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			if (base.GetTag(GAME_TAG.TURN) == 2 && !EndTurnButton.Get().IsInWaitingState())
			{
				this.ShowEndTurnBouncingArrow();
			}
			break;
		}
		case 4:
		{
			GameState.Get().SetBusy(true);
			AudioSource prevLine = base.GetPreloadedSound("VO_TUTORIAL_01_JAINA_03_03");
			while (SoundManager.Get().IsPlaying(prevLine))
			{
				yield return null;
			}
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_20_16", "TUTORIAL01_JAINA_20", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_06_06_ALT", "TUTORIAL01_HOGGER_07", Notification.SpeechBubbleDirection.TopRight, hoggerActor, 1f, true, false, 3f));
			Vector3 jainaPos = jainaActor.transform.position;
			Vector3 healthPopUpPosition = new Vector3(jainaPos.x + 3.3f, jainaPos.y + 0.5f, jainaPos.z - 0.85f);
			Notification.PopUpArrowDirection direction = Notification.PopUpArrowDirection.Left;
			if (UniversalInputManager.UsePhoneUI)
			{
				healthPopUpPosition = new Vector3(jainaPos.x + 3f, jainaPos.y + 0.5f, jainaPos.z + 0.85f);
				direction = Notification.PopUpArrowDirection.LeftDown;
			}
			Notification notification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, healthPopUpPosition, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL01_HELP_01"), true);
			notification.ShowPopUpArrow(direction);
			NotificationManager.Get().DestroyNotification(notification, 5f);
			Gameplay.Get().SetGameStateBusy(false, 5.2f);
			break;
		}
		case 5:
			this.HideFadeArrow();
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_05_05", "TUTORIAL01_JAINA_05", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			break;
		case 6:
			break;
		case 7:
			NotificationManager.Get().DestroyAllPopUps();
			yield return new WaitForSeconds(1.7f);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_07_07", "TUTORIAL01_JAINA_07", Notification.SpeechBubbleDirection.BottomRight, jainaActor, 1f, true, false, 3f));
			if (this.firstRaptorCard != null)
			{
				Vector3 raptorPosition = this.firstRaptorCard.transform.position;
				Notification raptorHelp;
				if (this.firstMurlocCard != null && this.firstRaptorCard.GetZonePosition() > this.firstMurlocCard.GetZonePosition())
				{
					Vector3 help04Position = new Vector3(raptorPosition.x + 3f, raptorPosition.y, raptorPosition.z);
					raptorHelp = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, help04Position, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL01_HELP_04"), true);
					raptorHelp.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
				}
				else
				{
					Vector3 help04Position = new Vector3(raptorPosition.x - 3f, raptorPosition.y, raptorPosition.z);
					raptorHelp = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, help04Position, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL01_HELP_04"), true);
					raptorHelp.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
				}
				NotificationManager.Get().DestroyNotification(raptorHelp, 4f);
			}
			yield return new WaitForSeconds(4f);
			if (GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count > 1 && !GameState.Get().IsInTargetMode())
			{
				this.ShowAttackWithYourMinionPopup();
			}
			if (base.GetTag(GAME_TAG.TURN) == 4 && EndTurnButton.Get().IsInNMPState())
			{
				yield return new WaitForSeconds(1f);
				this.ShowEndTurnBouncingArrow();
			}
			break;
		case 8:
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_03_03", "TUTORIAL01_HOGGER_05", Notification.SpeechBubbleDirection.TopRight, hoggerActor, 1f, true, false, 3f));
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_21_17", "TUTORIAL01_JAINA_21", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			GameState.Get().SetBusy(false);
			break;
		case 9:
			break;
		case 10:
			break;
		default:
			if (missionEvent != 55)
			{
				if (missionEvent != 66)
				{
					Debug.LogWarning("WARNING - Mission fired an event that we are not listening for.");
				}
				else
				{
					Vector3 northHero = new Vector3(136f, NotificationManager.DEPTH, 131f);
					Vector3 middleSpot = new Vector3(136f, NotificationManager.DEPTH, 80f);
					Notification innkeeperLine = null;
					if (!SoundUtils.CanDetectVolume())
					{
						innkeeperLine = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, northHero, GameStrings.Get("VO_TUTORIAL_01_ANNOUNCER_01"), string.Empty, 15f, null);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_01", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
						NotificationManager.Get().DestroyNotification(innkeeperLine, 0f);
					}
					else
					{
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_01", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
					}
					yield return new WaitForSeconds(0.5f);
					if (!SoundUtils.CanDetectVolume())
					{
						innkeeperLine = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, middleSpot, GameStrings.Get("VO_TUTORIAL_01_ANNOUNCER_02"), string.Empty, 15f, null);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_02", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
						NotificationManager.Get().DestroyNotification(innkeeperLine, 0f);
					}
					else
					{
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_02", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
					}
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_02_02", "TUTORIAL01_HOGGER_04", Notification.SpeechBubbleDirection.TopRight, hoggerActor, 1f, true, false, 3f));
					if (UniversalInputManager.UsePhoneUI)
					{
						Gameplay.Get().AddGamePlayNameBannerPhone();
					}
					if (!SoundUtils.CanDetectVolume())
					{
						innkeeperLine = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_TUTORIAL_01_ANNOUNCER_03"), string.Empty, 15f, null);
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_03", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
						NotificationManager.Get().DestroyNotification(innkeeperLine, 0f);
					}
					else
					{
						yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_03", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
					}
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_04", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
					this.announcerIsFinishedYapping = true;
				}
			}
			else
			{
				this.tooltipsDisabled = false;
				Collider dragPlane = Board.Get().FindCollider("DragPlane");
				dragPlane.enabled = true;
				while (!this.announcerIsFinishedYapping)
				{
					yield return null;
				}
				if (!SoundUtils.CanDetectVolume())
				{
					Notification battlebegin = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 84.8f), GameStrings.Get("VO_TUTORIAL_01_ANNOUNCER_05"), string.Empty, 15f, null);
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_05", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
					NotificationManager.Get().DestroyNotification(battlebegin, 0f);
				}
				else
				{
					yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_ANNOUNCER_05", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
				}
				yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_01_01", "TUTORIAL01_HOGGER_01", Notification.SpeechBubbleDirection.TopRight, hoggerActor, 1f, true, false, 3f));
				GameState.Get().SetBusy(false);
				yield return new WaitForSeconds(4f);
				Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_04_04", "TUTORIAL01_HOGGER_06", Notification.SpeechBubbleDirection.TopRight, hoggerActor, 1f, true, false, 3f));
			}
			break;
		case 12:
			yield return new WaitForSeconds(1f);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_15_11", "TUTORIAL01_JAINA_15", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			break;
		case 13:
			while (this.m_jainaSpeaking)
			{
				yield return null;
			}
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_16_12", "TUTORIAL01_JAINA_16", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			break;
		case 14:
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_08_08_ALT", "TUTORIAL01_HOGGER_08", Notification.SpeechBubbleDirection.TopRight, hoggerActor, 1f, true, false, 3f));
			Vector3 hoggerPosition = hoggerActor.transform.position;
			Vector3 hoggerHealthPopupPosition = new Vector3(hoggerPosition.x + 3.3f, hoggerPosition.y + 0.5f, hoggerPosition.z - 1f);
			if (UniversalInputManager.UsePhoneUI)
			{
				hoggerHealthPopupPosition = new Vector3(hoggerPosition.x + 3f, hoggerPosition.y + 0.5f, hoggerPosition.z - 0.75f);
			}
			Notification.PopUpArrowDirection hoggerHealthPopupDirection = Notification.PopUpArrowDirection.Left;
			Notification notification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, hoggerHealthPopupPosition, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL01_HELP_09"), true);
			notification.ShowPopUpArrow(hoggerHealthPopupDirection);
			NotificationManager.Get().DestroyNotification(notification, 5f);
			if (base.GetTag(GAME_TAG.TURN) == 6 && EndTurnButton.Get().IsInNMPState())
			{
				yield return new WaitForSeconds(9f);
				this.ShowEndTurnBouncingArrow();
			}
			break;
		}
		case 15:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_JAINA_02_55_ALT2", string.Empty, Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			break;
		case 20:
		{
			GameState.Get().SetBusy(true);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_JAINA_10_09", "TUTORIAL01_JAINA_10", Notification.SpeechBubbleDirection.BottomLeft, jainaActor, 1f, true, false, 3f));
			yield return new WaitForSeconds(1.5f);
			GameState.Get().SetBusy(false);
			List<Card> enemyCards = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards();
			Card lastCard = enemyCards[enemyCards.Count - 1];
			lastCard.GetActor().GetAttackObject().Jiggle();
			break;
		}
		case 21:
			break;
		case 22:
			GameState.Get().SetBusy(true);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TUTORIAL_01_HOGGER_09_09_ALT", "TUTORIAL01_HOGGER_02", Notification.SpeechBubbleDirection.TopRight, hoggerActor, 1f, true, false, 3f));
			Gameplay.Get().SetGameStateBusy(false, 2f);
			break;
		}
		yield break;
	}

	// Token: 0x0600278C RID: 10124 RVA: 0x000C0B28 File Offset: 0x000BED28
	private void ShowAttackWithYourMinionPopup()
	{
		if (this.attackWithYourMinion != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.attackWithYourMinion);
		}
		if (this.firstAttackFinished)
		{
			return;
		}
		if (this.firstMurlocCard == null)
		{
			return;
		}
		this.firstMurlocCard.GetActor().ToggleForceIdle(false);
		this.firstMurlocCard.GetActor().SetActorState(ActorStateType.CARD_PLAYABLE);
		Vector3 position = this.firstMurlocCard.transform.position;
		if (this.firstMurlocCard.GetEntity().IsExhausted())
		{
			return;
		}
		if (!(this.firstMurlocCard.GetZone() is ZonePlay))
		{
			return;
		}
		if (this.firstRaptorCard != null && this.firstMurlocCard.GetZonePosition() < this.firstRaptorCard.GetZonePosition())
		{
			Vector3 position2;
			position2..ctor(position.x - 3f, position.y, position.z);
			this.attackWithYourMinion = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.HELP_POPUP_SCALE, this.textToShowForAttackTip, true);
			this.attackWithYourMinion.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
		}
		else
		{
			Vector3 position2;
			position2..ctor(position.x + 3f, position.y, position.z);
			this.attackWithYourMinion = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.HELP_POPUP_SCALE, this.textToShowForAttackTip, true);
			this.attackWithYourMinion.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
		}
		Card firstCard = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetFirstCard();
		this.ShowFadeArrow(this.firstMurlocCard, firstCard);
		Gameplay.Get().StartCoroutine(this.SwapHelpTextAndFlashMinion());
	}

	// Token: 0x0600278D RID: 10125 RVA: 0x000C0CD0 File Offset: 0x000BEED0
	private IEnumerator SwapHelpTextAndFlashMinion()
	{
		if (this.firstMurlocCard == null)
		{
			yield break;
		}
		Gameplay.Get().StartCoroutine(this.BeginFlashingMinionLoop(this.firstMurlocCard));
		yield return new WaitForSeconds(4f);
		if (this.textToShowForAttackTip == GameStrings.Get("TUTORIAL01_HELP_10"))
		{
			yield break;
		}
		if (this.firstMurlocCard.GetEntity().IsExhausted())
		{
			yield break;
		}
		if (this.firstMurlocCard.GetActor().GetActorStateType() == ActorStateType.CARD_IDLE || this.firstMurlocCard.GetActor().GetActorStateType() == ActorStateType.CARD_MOUSE_OVER)
		{
			yield break;
		}
		if (!(this.firstMurlocCard.GetZone() is ZonePlay))
		{
			yield break;
		}
		if (this.firstAttackFinished)
		{
			yield break;
		}
		Vector3 cardInBattlefieldPosition = this.firstMurlocCard.transform.position;
		this.textToShowForAttackTip = GameStrings.Get("TUTORIAL01_HELP_10");
		if (this.attackWithYourMinion != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.attackWithYourMinion);
		}
		if (this.firstRaptorCard != null && this.firstMurlocCard.GetZonePosition() < this.firstRaptorCard.GetZonePosition())
		{
			Vector3 help02Position = new Vector3(cardInBattlefieldPosition.x - 3f, cardInBattlefieldPosition.y, cardInBattlefieldPosition.z);
			this.attackWithYourMinion = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, help02Position, TutorialEntity.HELP_POPUP_SCALE, this.textToShowForAttackTip, true);
			this.attackWithYourMinion.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
		}
		else
		{
			Vector3 help02Position = new Vector3(cardInBattlefieldPosition.x + 3f, cardInBattlefieldPosition.y, cardInBattlefieldPosition.z);
			this.attackWithYourMinion = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, help02Position, TutorialEntity.HELP_POPUP_SCALE, this.textToShowForAttackTip, true);
			this.attackWithYourMinion.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
		}
		yield break;
	}

	// Token: 0x0600278E RID: 10126 RVA: 0x000C0CEC File Offset: 0x000BEEEC
	private IEnumerator FlashMinionUntilAttackBegins(Card minionToFlash)
	{
		yield return new WaitForSeconds(8f);
		Gameplay.Get().StartCoroutine(this.BeginFlashingMinionLoop(minionToFlash));
		yield break;
	}

	// Token: 0x0600278F RID: 10127 RVA: 0x000C0D18 File Offset: 0x000BEF18
	private IEnumerator BeginFlashingMinionLoop(Card minionToFlash)
	{
		if (minionToFlash == null)
		{
			yield break;
		}
		if (minionToFlash.GetEntity().IsExhausted())
		{
			yield break;
		}
		if (minionToFlash.GetActor().GetActorStateType() == ActorStateType.CARD_IDLE || minionToFlash.GetActor().GetActorStateType() == ActorStateType.CARD_MOUSE_OVER)
		{
			yield break;
		}
		minionToFlash.GetActorSpell(SpellType.WIGGLE, true).Activate();
		yield return new WaitForSeconds(1.5f);
		Gameplay.Get().StartCoroutine(this.BeginFlashingMinionLoop(minionToFlash));
		yield break;
	}

	// Token: 0x06002790 RID: 10128 RVA: 0x000C0D44 File Offset: 0x000BEF44
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

	// Token: 0x06002791 RID: 10129 RVA: 0x000C0DB4 File Offset: 0x000BEFB4
	private void ShowDontFireballYourselfPopup(Vector3 origin)
	{
		if (this.noFireballPopup != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.noFireballPopup);
		}
		Vector3 position;
		position..ctor(origin.x - 3f, origin.y, origin.z);
		this.noFireballPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL01_HELP_07"), true);
		NotificationManager.Get().DestroyNotification(this.noFireballPopup, 2.5f);
	}

	// Token: 0x06002792 RID: 10130 RVA: 0x000C0E3B File Offset: 0x000BF03B
	public override bool ShouldDoAlternateMulliganIntro()
	{
		return true;
	}

	// Token: 0x06002793 RID: 10131 RVA: 0x000C0E3E File Offset: 0x000BF03E
	public override bool DoAlternateMulliganIntro()
	{
		AssetLoader.Get().LoadActor("GameOpen_Pack", new AssetLoader.GameObjectCallback(this.PackLoadedCallback), null, false);
		return true;
	}

	// Token: 0x06002794 RID: 10132 RVA: 0x000C0E60 File Offset: 0x000BF060
	private void PackLoadedCallback(string actorName, GameObject actorObject, object callbackData)
	{
		MusicManager.Get().StartPlaylist(MusicPlaylistType.Misc_Tutorial01);
		Card heroCard = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
		Card heroCard2 = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
		this.startingPack = actorObject;
		Transform transform = SceneUtils.FindChildBySubstring(this.startingPack, "Hero_Dummy").transform;
		heroCard.transform.parent = transform;
		heroCard.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		heroCard.transform.localPosition = new Vector3(0f, 0f, 0f);
		SceneUtils.SetLayer(heroCard.GetActor().GetRootObject(), GameLayer.IgnoreFullScreenEffects);
		Transform transform2 = SceneUtils.FindChildBySubstring(this.startingPack, "HeroEnemy_Dummy").transform;
		heroCard2.transform.parent = transform2;
		heroCard2.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		heroCard2.transform.localPosition = new Vector3(0f, 0f, 0f);
		heroCard.SetDoNotSort(true);
		Transform transform3 = Board.Get().FindBone("Tutorial1HeroStart");
		actorObject.transform.position = transform3.position;
		heroCard.GetActor().GetHealthObject().Hide();
		heroCard2.GetActor().GetHealthObject().Hide();
		heroCard2.GetActor().Hide();
		heroCard.GetActor().Hide();
		SceneMgr.Get().NotifySceneLoaded();
		Gameplay.Get().StartCoroutine(this.UpdatePresence());
		Gameplay.Get().StartCoroutine(this.ShowPackOpeningArrow(transform3.position));
	}

	// Token: 0x06002795 RID: 10133 RVA: 0x000C100C File Offset: 0x000BF20C
	private IEnumerator UpdatePresence()
	{
		while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
		{
			yield return null;
		}
		GameMgr.Get().UpdatePresence();
		yield break;
	}

	// Token: 0x06002796 RID: 10134 RVA: 0x000C1020 File Offset: 0x000BF220
	private IEnumerator ShowPackOpeningArrow(Vector3 packSpot)
	{
		yield return new WaitForSeconds(4f);
		if (this.packOpened)
		{
			yield break;
		}
		Vector3 popUpPosition = new Vector3(packSpot.x + 4.014574f, packSpot.y, packSpot.z + 0.2307634f);
		this.freeCardsPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, popUpPosition, TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("TUTORIAL01_HELP_18"), true);
		this.freeCardsPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
		yield break;
	}

	// Token: 0x06002797 RID: 10135 RVA: 0x000C104C File Offset: 0x000BF24C
	public override void NotifyOfGamePackOpened()
	{
		this.packOpened = true;
		if (this.freeCardsPopup != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.freeCardsPopup);
		}
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x000C1084 File Offset: 0x000BF284
	public override void NotifyOfCustomIntroFinished()
	{
		Card heroCard = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
		Card heroCard2 = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
		heroCard.SetDoNotSort(false);
		heroCard2.GetActor().TurnOnCollider();
		heroCard.GetActor().TurnOnCollider();
		heroCard.transform.parent = null;
		heroCard2.transform.parent = null;
		SceneUtils.SetLayer(heroCard.GetActor().GetRootObject(), GameLayer.CardRaycast);
		Gameplay.Get().StartCoroutine(this.ContinueFinishingCustomIntro());
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x000C1108 File Offset: 0x000BF308
	private IEnumerator ContinueFinishingCustomIntro()
	{
		yield return new WaitForSeconds(3f);
		Object.Destroy(this.startingPack);
		GameState.Get().SetBusy(false);
		MulliganManager.Get().SkipMulligan();
		yield break;
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x000C1123 File Offset: 0x000BF323
	public override bool IsMouseOverDelayOverriden()
	{
		return true;
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000C1126 File Offset: 0x000BF326
	public override bool AreTooltipsDisabled()
	{
		return this.tooltipsDisabled;
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x000C112E File Offset: 0x000BF32E
	public override bool ShouldShowBigCard()
	{
		return base.GetTag(GAME_TAG.TURN) > 8;
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x000C113B File Offset: 0x000BF33B
	public override void NotifyOfDefeatCoinAnimation()
	{
		base.PlaySound("VO_TUTORIAL_01_JAINA_13_10", 1f, true, false);
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x000C114F File Offset: 0x000BF34F
	public override bool ShouldShowHeroTooltips()
	{
		return true;
	}

	// Token: 0x0600279F RID: 10143 RVA: 0x000C1154 File Offset: 0x000BF354
	public override List<RewardData> GetCustomRewards()
	{
		List<RewardData> list = new List<RewardData>();
		CardRewardData cardRewardData = new CardRewardData("CS2_023", TAG_PREMIUM.NORMAL, 2);
		cardRewardData.MarkAsDummyReward();
		list.Add(cardRewardData);
		return list;
	}

	// Token: 0x04001735 RID: 5941
	private Notification endTurnNotifier;

	// Token: 0x04001736 RID: 5942
	private Notification handBounceArrow;

	// Token: 0x04001737 RID: 5943
	private Notification handFadeArrow;

	// Token: 0x04001738 RID: 5944
	private Notification noFireballPopup;

	// Token: 0x04001739 RID: 5945
	private Notification attackWithYourMinion;

	// Token: 0x0400173A RID: 5946
	private Notification crushThisGnoll;

	// Token: 0x0400173B RID: 5947
	private Notification freeCardsPopup;

	// Token: 0x0400173C RID: 5948
	private KeywordHelpPanel attackHelpPanel;

	// Token: 0x0400173D RID: 5949
	private KeywordHelpPanel healthHelpPanel;

	// Token: 0x0400173E RID: 5950
	private Card mousedOverCard;

	// Token: 0x0400173F RID: 5951
	private GameObject costLabel;

	// Token: 0x04001740 RID: 5952
	private GameObject attackLabel;

	// Token: 0x04001741 RID: 5953
	private GameObject healthLabel;

	// Token: 0x04001742 RID: 5954
	private Card firstMurlocCard;

	// Token: 0x04001743 RID: 5955
	private Card firstRaptorCard;

	// Token: 0x04001744 RID: 5956
	private int numTimesTextSwapStarted;

	// Token: 0x04001745 RID: 5957
	private bool tooltipsDisabled = true;

	// Token: 0x04001746 RID: 5958
	private string textToShowForAttackTip = GameStrings.Get("TUTORIAL01_HELP_02");

	// Token: 0x04001747 RID: 5959
	private GameObject startingPack;

	// Token: 0x04001748 RID: 5960
	private bool packOpened;

	// Token: 0x04001749 RID: 5961
	private bool announcerIsFinishedYapping;

	// Token: 0x0400174A RID: 5962
	private bool firstAttackFinished;

	// Token: 0x0400174B RID: 5963
	private bool m_jainaSpeaking;

	// Token: 0x0400174C RID: 5964
	private bool m_isShowingAttackHelpPanel;

	// Token: 0x0400174D RID: 5965
	private PlatformDependentValue<float> m_gemScale = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 1.75f,
		Phone = 1.2f
	};

	// Token: 0x0400174E RID: 5966
	private PlatformDependentValue<Vector3> m_attackTooltipPosition = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(-2.15f, 0f, -0.62f),
		Phone = new Vector3(-3.5f, 0f, -0.62f)
	};

	// Token: 0x0400174F RID: 5967
	private PlatformDependentValue<Vector3> m_healthTooltipPosition = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(2.05f, 0f, -0.62f),
		Phone = new Vector3(3.25f, 0f, -0.62f)
	};

	// Token: 0x04001750 RID: 5968
	private PlatformDependentValue<Vector3> m_heroHealthTooltipPosition = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(2.4f, 0.3f, -0.8f),
		Phone = new Vector3(3.5f, 0.3f, 0.6f)
	};
}
