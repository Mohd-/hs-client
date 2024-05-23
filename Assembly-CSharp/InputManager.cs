using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PegasusGame;
using UnityEngine;

// Token: 0x02000205 RID: 517
public class InputManager : MonoBehaviour
{
	// Token: 0x06001E84 RID: 7812 RVA: 0x0008DF58 File Offset: 0x0008C158
	private void Awake()
	{
		InputManager.s_instance = this;
		this.m_useHandEnlarge = UniversalInputManager.UsePhoneUI;
		if (GameState.Get() != null)
		{
			GameState.Get().RegisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
			GameState.Get().RegisterOptionRejectedListener(new GameState.OptionRejectedCallback(this.OnOptionRejected), null);
			GameState.Get().RegisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
			GameState.Get().RegisterSpectatorNotifyListener(new GameState.SpectatorNotifyEventCallback(this.OnSpectatorNotifyEvent), null);
			GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver), null);
		}
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x0008DFFC File Offset: 0x0008C1FC
	private void OnDestroy()
	{
		if (GameState.Get() != null)
		{
			GameState.Get().UnregisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
			GameState.Get().UnregisterOptionRejectedListener(new GameState.OptionRejectedCallback(this.OnOptionRejected), null);
			GameState.Get().UnregisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
			GameState.Get().UnregisterSpectatorNotifyListener(new GameState.SpectatorNotifyEventCallback(this.OnSpectatorNotifyEvent), null);
			GameState.Get().UnregisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver), null);
		}
		InputManager.s_instance = null;
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x0008E090 File Offset: 0x0008C290
	private void Update()
	{
		if (!this.m_checkForInput)
		{
			return;
		}
		if (UniversalInputManager.Get().GetMouseButtonDown(0))
		{
			this.HandleLeftMouseDown();
		}
		if (UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			this.m_touchDraggingCard = false;
			this.HandleLeftMouseUp();
		}
		if (UniversalInputManager.Get().GetMouseButtonDown(1))
		{
			this.HandleRightMouseDown();
		}
		if (UniversalInputManager.Get().GetMouseButtonUp(1))
		{
			this.HandleRightMouseUp();
		}
		this.HandleMouseMove();
		if (this.m_leftMouseButtonIsDown && this.m_heldCard == null)
		{
			this.HandleUpdateWhileLeftMouseButtonIsDown();
			if (UniversalInputManager.Get().IsTouchMode() && !this.m_touchDraggingCard)
			{
				this.HandleUpdateWhileNotHoldingCard();
			}
		}
		else if (this.m_heldCard == null)
		{
			this.HandleUpdateWhileNotHoldingCard();
		}
		bool flag = GameState.Get() == null || GameState.Get().GetFriendlySidePlayer() == null || GameState.Get().GetFriendlySidePlayer().IsLocalUser();
		if (flag)
		{
			bool flag2 = UniversalInputManager.Get().InputHitAnyObject(Camera.main, GameLayer.InvisibleHitBox2);
			if (TargetReticleManager.Get() && TargetReticleManager.Get().IsActive())
			{
				if (!flag2 && this.GetBattlecrySourceCard() == null && ChoiceCardMgr.Get().GetSubOptionParentCard() == null && (!UniversalInputManager.UsePhoneUI || (!this.m_targettingHeroPower && !GameState.Get().IsSelectedOptionFriendlyHero())))
				{
					this.CancelOption();
					if (this.m_useHandEnlarge)
					{
						this.m_myHandZone.SetFriendlyHeroTargetingMode(false);
					}
					if (this.m_heldCard != null)
					{
						this.PositionHeldCard();
					}
				}
				else
				{
					TargetReticleManager.Get().UpdateArrowPosition();
				}
			}
			else if (this.m_heldCard)
			{
				this.HandleUpdateWhileHoldingCard(flag2);
			}
		}
		if (EmoteHandler.Get() != null && EmoteHandler.Get().AreEmotesActive())
		{
			EmoteHandler.Get().HandleInput();
		}
		if (EnemyEmoteHandler.Get() != null && EnemyEmoteHandler.Get().AreEmotesActive())
		{
			EnemyEmoteHandler.Get().HandleInput();
		}
		this.ShowTooltipIfNecessary();
	}

	// Token: 0x06001E87 RID: 7815 RVA: 0x0008E2DB File Offset: 0x0008C4DB
	public static InputManager Get()
	{
		return InputManager.s_instance;
	}

	// Token: 0x06001E88 RID: 7816 RVA: 0x0008E2E4 File Offset: 0x0008C4E4
	public bool HandleKeyboardInput()
	{
		if (this.HandleUniversalHotkeys())
		{
			return true;
		}
		if (GameState.Get() != null && GameState.Get().IsMulliganManagerActive())
		{
			return this.HandleMulliganHotkeys();
		}
		return this.HandleGameHotkeys();
	}

	// Token: 0x06001E89 RID: 7817 RVA: 0x0008E324 File Offset: 0x0008C524
	public Card GetMousedOverCard()
	{
		return this.m_mousedOverCard;
	}

	// Token: 0x06001E8A RID: 7818 RVA: 0x0008E32C File Offset: 0x0008C52C
	public void SetMousedOverCard(Card card)
	{
		if (this.m_mousedOverCard == card)
		{
			return;
		}
		if (this.m_mousedOverCard != null && !(this.m_mousedOverCard.GetZone() is ZoneHand))
		{
			this.HandleMouseOffCard();
		}
		if (!card.IsInputEnabled())
		{
			return;
		}
		this.m_mousedOverCard = card;
		card.NotifyMousedOver();
	}

	// Token: 0x06001E8B RID: 7819 RVA: 0x0008E390 File Offset: 0x0008C590
	public Card GetBattlecrySourceCard()
	{
		return this.m_battlecrySourceCard;
	}

	// Token: 0x06001E8C RID: 7820 RVA: 0x0008E398 File Offset: 0x0008C598
	public void StartWatchingForInput()
	{
		if (this.m_checkForInput)
		{
			return;
		}
		this.m_checkForInput = true;
		List<Zone> zones = ZoneMgr.Get().GetZones();
		foreach (Zone zone in zones)
		{
			if (zone.m_Side == Player.Side.FRIENDLY)
			{
				if (zone is ZoneHand)
				{
					this.m_myHandZone = (ZoneHand)zone;
				}
				else if (zone is ZonePlay)
				{
					this.m_myPlayZone = (ZonePlay)zone;
				}
				else if (zone is ZoneWeapon)
				{
					this.m_myWeaponZone = (ZoneWeapon)zone;
				}
			}
		}
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x0008E464 File Offset: 0x0008C664
	public void DisableInput()
	{
		this.m_checkForInput = false;
		this.HandleMouseOff();
		if (TargetReticleManager.Get())
		{
			TargetReticleManager.Get().DestroyFriendlyTargetArrow(false);
		}
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x0008E48D File Offset: 0x0008C68D
	public Card GetHeldCard()
	{
		return this.m_heldCard;
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x0008E495 File Offset: 0x0008C695
	public void EnableInput()
	{
		this.m_checkForInput = true;
	}

	// Token: 0x06001E90 RID: 7824 RVA: 0x0008E49E File Offset: 0x0008C69E
	public void OnMulliganEnded()
	{
		if (this.m_mousedOverCard)
		{
			this.SetShouldShowTooltip();
		}
	}

	// Token: 0x06001E91 RID: 7825 RVA: 0x0008E4B6 File Offset: 0x0008C6B6
	private void SetShouldShowTooltip()
	{
		this.m_mousedOverTimer = 0f;
		this.m_mousedOverCard.SetShouldShowTooltip();
	}

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06001E92 RID: 7826 RVA: 0x0008E4CE File Offset: 0x0008C6CE
	public bool LeftMouseButtonDown
	{
		get
		{
			return this.m_leftMouseButtonIsDown;
		}
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06001E93 RID: 7827 RVA: 0x0008E4D6 File Offset: 0x0008C6D6
	public Vector3 LastMouseDownPosition
	{
		get
		{
			return this.m_lastMouseDownPosition;
		}
	}

	// Token: 0x06001E94 RID: 7828 RVA: 0x0008E4DE File Offset: 0x0008C6DE
	public ZoneHand GetFriendlyHand()
	{
		return this.m_myHandZone;
	}

	// Token: 0x06001E95 RID: 7829 RVA: 0x0008E4E6 File Offset: 0x0008C6E6
	public bool UseHandEnlarge()
	{
		return this.m_useHandEnlarge;
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x0008E4EE File Offset: 0x0008C6EE
	public void SetHandEnlarge(bool set)
	{
		this.m_useHandEnlarge = set;
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x0008E4F7 File Offset: 0x0008C6F7
	public bool DoesHideHandAfterPlayingCard()
	{
		return this.m_hideHandAfterPlayingCard;
	}

	// Token: 0x06001E98 RID: 7832 RVA: 0x0008E4FF File Offset: 0x0008C6FF
	public void SetHideHandAfterPlayingCard(bool set)
	{
		this.m_hideHandAfterPlayingCard = set;
	}

	// Token: 0x06001E99 RID: 7833 RVA: 0x0008E508 File Offset: 0x0008C708
	public bool DropHeldCard()
	{
		return this.DropHeldCard(false);
	}

	// Token: 0x06001E9A RID: 7834 RVA: 0x0008E514 File Offset: 0x0008C714
	private void HandleLeftMouseDown()
	{
		this.m_touchedDownOnSmallHand = false;
		GameObject gameObject = null;
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(GameLayer.CardRaycast, out raycastHit))
		{
			gameObject = raycastHit.collider.gameObject;
			if (gameObject.GetComponent<EndTurnButtonReminder>() != null)
			{
				return;
			}
			CardStandIn cardStandIn = SceneUtils.FindComponentInParents<CardStandIn>(raycastHit.transform);
			if (cardStandIn != null && GameState.Get() != null && !GameState.Get().IsMulliganManagerActive())
			{
				Card linkedCard = cardStandIn.linkedCard;
				if (this.IsCancelingBattlecryCard(linkedCard))
				{
					return;
				}
				if (this.m_useHandEnlarge && !this.m_myHandZone.HandEnlarged())
				{
					this.m_leftMouseButtonIsDown = true;
					this.m_touchedDownOnSmallHand = true;
					return;
				}
				this.m_lastObjectMousedDown = cardStandIn.gameObject;
				this.m_lastMouseDownPosition = UniversalInputManager.Get().GetMousePosition();
				this.m_leftMouseButtonIsDown = true;
				if (UniversalInputManager.Get().IsTouchMode())
				{
					this.m_touchDraggingCard = this.m_myHandZone.TouchReceived();
					this.m_lastPreviewedCard = cardStandIn.linkedCard;
				}
				return;
			}
			else
			{
				if (gameObject.GetComponent<EndTurnButton>() != null && !GameMgr.Get().IsSpectator())
				{
					EndTurnButton.Get().PlayPushDownAnimation();
					this.m_lastObjectMousedDown = raycastHit.collider.gameObject;
					return;
				}
				if (gameObject.GetComponent<GameOpenPack>() != null)
				{
					this.m_lastObjectMousedDown = raycastHit.collider.gameObject;
					return;
				}
				Actor actor = SceneUtils.FindComponentInParents<Actor>(raycastHit.transform);
				if (actor == null)
				{
					return;
				}
				Card card = actor.GetCard();
				if (UniversalInputManager.Get().IsTouchMode() && this.m_battlecrySourceCard != null && card == this.m_battlecrySourceCard)
				{
					this.m_dragging = true;
					TargetReticleManager.Get().ShowArrow(true);
					return;
				}
				if (this.IsCancelingBattlecryCard(card))
				{
					return;
				}
				if (card != null)
				{
					this.m_lastObjectMousedDown = card.gameObject;
				}
				else if (actor.GetHistoryCard() != null)
				{
					this.m_lastObjectMousedDown = actor.transform.parent.gameObject;
				}
				else
				{
					Debug.LogWarning("You clicked on something that is not being handled by InputManager.  Alert The Brode!");
				}
				this.m_lastMouseDownPosition = UniversalInputManager.Get().GetMousePosition();
				this.m_leftMouseButtonIsDown = true;
			}
		}
		if (this.m_useHandEnlarge && this.m_myHandZone.HandEnlarged() && ChoiceCardMgr.Get().GetSubOptionParentCard() == null && gameObject == null)
		{
			this.HidePhoneHand();
		}
		this.HandleMemberClick();
	}

	// Token: 0x06001E9B RID: 7835 RVA: 0x0008E7AC File Offset: 0x0008C9AC
	private void ShowPhoneHand()
	{
		if (GameState.Get().IsMulliganPhaseNowOrPending())
		{
			return;
		}
		if (this.m_useHandEnlarge && !this.m_myHandZone.HandEnlarged())
		{
			this.m_myHandZone.AddUpdateLayoutCompleteCallback(new Zone.UpdateLayoutCompleteCallback(this.OnHandEnlargeComplete));
			this.m_myHandZone.SetHandEnlarged(true);
			InputManager.PhoneHandShownListener[] array = this.m_phoneHandShownListener.ToArray();
			foreach (InputManager.PhoneHandShownListener phoneHandShownListener in array)
			{
				phoneHandShownListener.Fire();
			}
		}
	}

	// Token: 0x06001E9C RID: 7836 RVA: 0x0008E834 File Offset: 0x0008CA34
	private void HidePhoneHand()
	{
		if (this.m_useHandEnlarge && this.m_myHandZone != null && this.m_myHandZone.HandEnlarged())
		{
			this.m_myHandZone.SetHandEnlarged(false);
			InputManager.PhoneHandHiddenListener[] array = this.m_phoneHandHiddenListener.ToArray();
			foreach (InputManager.PhoneHandHiddenListener phoneHandHiddenListener in array)
			{
				phoneHandHiddenListener.Fire();
			}
		}
	}

	// Token: 0x06001E9D RID: 7837 RVA: 0x0008E8A8 File Offset: 0x0008CAA8
	private void OnHandEnlargeComplete(Zone zone, object userData)
	{
		zone.RemoveUpdateLayoutCompleteCallback(new Zone.UpdateLayoutCompleteCallback(this.OnHandEnlargeComplete));
		if (this.m_leftMouseButtonIsDown && UniversalInputManager.Get().InputHitAnyObject(GameLayer.CardRaycast))
		{
			this.HandleLeftMouseDown();
		}
	}

	// Token: 0x06001E9E RID: 7838 RVA: 0x0008E8E9 File Offset: 0x0008CAE9
	private void HidePhoneHandIfOutOfServerPlays()
	{
		if (GameState.Get().HasHandPlays())
		{
			return;
		}
		this.HidePhoneHand();
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x0008E904 File Offset: 0x0008CB04
	private bool HasLocalHandPlays()
	{
		List<Card> cards = this.m_myHandZone.GetCards();
		if (cards.Count == 0)
		{
			return false;
		}
		int spendableManaCrystals = ManaCrystalMgr.Get().GetSpendableManaCrystals();
		foreach (Card card in cards)
		{
			Entity entity = card.GetEntity();
			int realTimeCost = entity.GetRealTimeCost();
			if (realTimeCost <= spendableManaCrystals)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x0008E99C File Offset: 0x0008CB9C
	private void HandleLeftMouseUp()
	{
		PegCursor.Get().SetMode(PegCursor.Mode.UP);
		bool dragging = this.m_dragging;
		this.m_dragging = false;
		this.m_leftMouseButtonIsDown = false;
		this.m_targettingHeroPower = false;
		GameObject lastObjectMousedDown = this.m_lastObjectMousedDown;
		this.m_lastObjectMousedDown = null;
		if (UniversalInputManager.Get().WasTouchCanceled())
		{
			this.CancelOption();
			this.m_heldCard = null;
			return;
		}
		if (this.m_heldCard != null && (GameState.Get().GetResponseMode() == GameState.ResponseMode.OPTION || GameState.Get().GetResponseMode() == GameState.ResponseMode.NONE))
		{
			this.DropHeldCard();
			return;
		}
		bool flag = UniversalInputManager.Get().IsTouchMode() && GameState.Get().IsInTargetMode();
		bool flag2 = ChoiceCardMgr.Get().GetSubOptionParentCard() != null;
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(GameLayer.CardRaycast, out raycastHit))
		{
			GameObject gameObject = raycastHit.collider.gameObject;
			if (gameObject.GetComponent<EndTurnButtonReminder>() != null)
			{
				return;
			}
			if (gameObject.GetComponent<EndTurnButton>() != null && gameObject == lastObjectMousedDown && !GameMgr.Get().IsSpectator())
			{
				EndTurnButton.Get().PlayButtonUpAnimation();
				this.DoEndTurnButton();
			}
			else if (gameObject.GetComponent<GameOpenPack>() != null && gameObject == lastObjectMousedDown)
			{
				gameObject.GetComponent<GameOpenPack>().HandleClick();
			}
			else
			{
				Actor actor = SceneUtils.FindComponentInParents<Actor>(raycastHit.transform);
				if (actor != null)
				{
					Card card = actor.GetCard();
					if (card != null)
					{
						if ((card.gameObject == lastObjectMousedDown || dragging) && !this.IsCancelingBattlecryCard(card))
						{
							this.HandleClickOnCard(actor.GetCard().gameObject, card.gameObject == lastObjectMousedDown);
						}
					}
					else if (actor.GetHistoryCard() != null)
					{
						HistoryManager.Get().HandleClickOnBigCard(actor.GetHistoryCard());
					}
				}
				CardStandIn cardStandIn = SceneUtils.FindComponentInParents<CardStandIn>(raycastHit.transform);
				if (cardStandIn != null)
				{
					if (this.m_useHandEnlarge && this.m_touchedDownOnSmallHand)
					{
						this.ShowPhoneHand();
					}
					if (lastObjectMousedDown == cardStandIn.gameObject && GameState.Get() != null && !GameState.Get().IsMulliganManagerActive() && !this.IsCancelingBattlecryCard(cardStandIn.linkedCard))
					{
						this.HandleClickOnCard(cardStandIn.linkedCard.gameObject, true);
					}
				}
				if (UniversalInputManager.Get().IsTouchMode() && actor != null && ChoiceCardMgr.Get().GetSubOptionParentCard() != null)
				{
					foreach (Card card2 in ChoiceCardMgr.Get().GetFriendlyCards())
					{
						if (card2 == actor.GetCard())
						{
							flag2 = false;
							break;
						}
					}
				}
			}
		}
		if (flag)
		{
			this.CancelOption();
		}
		if (UniversalInputManager.Get().IsTouchMode() && flag2 && ChoiceCardMgr.Get().GetSubOptionParentCard() != null)
		{
			this.CancelSubOptionMode();
		}
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x0008ED04 File Offset: 0x0008CF04
	private void HandleRightMouseDown()
	{
		RaycastHit raycastHit;
		if (!UniversalInputManager.Get().GetInputHitInfo(GameLayer.CardRaycast, out raycastHit))
		{
			return;
		}
		GameObject gameObject = raycastHit.collider.gameObject;
		if (gameObject.GetComponent<EndTurnButtonReminder>() != null)
		{
			return;
		}
		if (gameObject.GetComponent<EndTurnButton>() != null)
		{
			return;
		}
		Actor actor = SceneUtils.FindComponentInParents<Actor>(raycastHit.transform);
		if (actor == null)
		{
			return;
		}
		if (actor.GetCard() != null)
		{
			this.m_lastObjectRightMousedDown = actor.GetCard().gameObject;
		}
		else if (actor.GetHistoryCard() != null)
		{
			this.m_lastObjectRightMousedDown = actor.transform.parent.gameObject;
		}
		else
		{
			Debug.LogWarning("You clicked on something that is not being handled by InputManager.  Alert The Brode!");
		}
	}

	// Token: 0x06001EA2 RID: 7842 RVA: 0x0008EDCC File Offset: 0x0008CFCC
	private void HandleRightMouseUp()
	{
		PegCursor.Get().SetMode(PegCursor.Mode.UP);
		GameObject lastObjectRightMousedDown = this.m_lastObjectRightMousedDown;
		this.m_lastObjectRightMousedDown = null;
		this.m_lastObjectMousedDown = null;
		this.m_leftMouseButtonIsDown = false;
		this.m_dragging = false;
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(GameLayer.CardRaycast, out raycastHit))
		{
			Actor actor = SceneUtils.FindComponentInParents<Actor>(raycastHit.transform);
			if (actor == null || actor.GetCard() == null)
			{
				this.HandleRightClick();
				return;
			}
			if (actor.GetCard().gameObject == lastObjectRightMousedDown)
			{
				this.HandleRightClickOnCard(actor.GetCard());
			}
			else
			{
				this.HandleRightClick();
			}
		}
		else
		{
			this.HandleRightClick();
		}
	}

	// Token: 0x06001EA3 RID: 7843 RVA: 0x0008EE84 File Offset: 0x0008D084
	private void HandleRightClick()
	{
		if (this.CancelOption())
		{
			return;
		}
		if (EmoteHandler.Get() != null && EmoteHandler.Get().AreEmotesActive())
		{
			EmoteHandler.Get().HideEmotes();
		}
		if (EnemyEmoteHandler.Get() != null && EnemyEmoteHandler.Get().AreEmotesActive())
		{
			EnemyEmoteHandler.Get().HideEmotes();
		}
	}

	// Token: 0x06001EA4 RID: 7844 RVA: 0x0008EEF0 File Offset: 0x0008D0F0
	private bool CancelOption()
	{
		bool result = false;
		GameState gameState = GameState.Get();
		if (gameState.IsInMainOptionMode())
		{
			gameState.CancelCurrentOptionMode();
		}
		if (this.CancelTargetMode())
		{
			result = true;
		}
		if (this.CancelSubOptionMode())
		{
			result = true;
		}
		if (this.DropHeldCard(true))
		{
			result = true;
		}
		if (this.m_mousedOverCard)
		{
			this.m_mousedOverCard.UpdateProposedManaUsage();
		}
		return result;
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x0008EF5C File Offset: 0x0008D15C
	private bool CancelTargetMode()
	{
		GameState gameState = GameState.Get();
		if (!gameState.IsInTargetMode())
		{
			return false;
		}
		SoundManager.Get().LoadAndPlay("CancelAttack");
		if (this.m_mousedOverCard)
		{
			this.DisableSkullIfNeeded(this.m_mousedOverCard);
		}
		if (TargetReticleManager.Get())
		{
			TargetReticleManager.Get().DestroyFriendlyTargetArrow(true);
		}
		this.ResetBattlecrySourceCard();
		this.CancelSubOptions();
		GameState.Get().CancelCurrentOptionMode();
		return true;
	}

	// Token: 0x06001EA6 RID: 7846 RVA: 0x0008EFD8 File Offset: 0x0008D1D8
	private bool CancelSubOptionMode()
	{
		if (!GameState.Get().IsInSubOptionMode())
		{
			return false;
		}
		this.CancelSubOptions();
		GameState.Get().CancelCurrentOptionMode();
		return true;
	}

	// Token: 0x06001EA7 RID: 7847 RVA: 0x0008F008 File Offset: 0x0008D208
	private void PositionHeldCard()
	{
		Card heldCard = this.m_heldCard;
		Entity entity = heldCard.GetEntity();
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(Camera.main, GameLayer.InvisibleHitBox2, out raycastHit))
		{
			if (!heldCard.IsOverPlayfield())
			{
				if (!GameState.Get().HasResponse(entity))
				{
					this.m_leftMouseButtonIsDown = false;
					this.m_lastObjectMousedDown = null;
					this.m_dragging = false;
					this.DropHeldCard();
					return;
				}
				heldCard.NotifyOverPlayfield();
			}
			if (entity.IsMinion())
			{
				int num = this.PlayZoneSlotMousedOver(heldCard);
				if (num != this.m_myPlayZone.GetSlotMousedOver())
				{
					this.m_myPlayZone.SortWithSpotForHeldCard(num);
				}
			}
		}
		else if (heldCard.IsOverPlayfield())
		{
			heldCard.NotifyLeftPlayfield();
			this.m_myPlayZone.SortWithSpotForHeldCard(-1);
		}
		RaycastHit raycastHit2;
		if (UniversalInputManager.Get().GetInputHitInfo(Camera.main, GameLayer.DragPlane, out raycastHit2))
		{
			heldCard.transform.position = raycastHit2.point;
		}
	}

	// Token: 0x06001EA8 RID: 7848 RVA: 0x0008F0FC File Offset: 0x0008D2FC
	private int PlayZoneSlotMousedOver(Card card)
	{
		int num = 0;
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(Camera.main, GameLayer.InvisibleHitBox2, out raycastHit))
		{
			float slotWidth = this.m_myPlayZone.GetSlotWidth();
			float num2 = this.m_myPlayZone.transform.position.x - (float)(this.m_myPlayZone.GetCards().Count + 1) * slotWidth / 2f;
			num = (int)Mathf.Ceil((raycastHit.point.x - num2) / slotWidth - slotWidth / 2f);
			int count = this.m_myPlayZone.GetCards().Count;
			if (num < 0 || num > count)
			{
				if (card.transform.position.x < this.m_myPlayZone.transform.position.x)
				{
					num = 0;
				}
				else
				{
					num = count;
				}
			}
		}
		return num;
	}

	// Token: 0x06001EA9 RID: 7849 RVA: 0x0008F1E8 File Offset: 0x0008D3E8
	private void HandleUpdateWhileLeftMouseButtonIsDown()
	{
		if (UniversalInputManager.Get().IsTouchMode() && this.m_heldCard == null)
		{
			if (this.GetBattlecrySourceCard() == null)
			{
				this.m_myHandZone.HandleInput();
			}
			Card card = (!(this.m_myHandZone.CurrentStandIn != null)) ? null : this.m_myHandZone.CurrentStandIn.linkedCard;
			if (card != this.m_lastPreviewedCard)
			{
				if (card != null)
				{
					this.m_lastMouseDownPosition.y = UniversalInputManager.Get().GetMousePosition().y;
				}
				this.m_lastPreviewedCard = card;
			}
		}
		if (this.m_dragging)
		{
			return;
		}
		if (this.m_lastObjectMousedDown == null)
		{
			return;
		}
		if (this.m_lastObjectMousedDown.GetComponent<HistoryCard>())
		{
			this.m_lastObjectMousedDown = null;
			this.m_leftMouseButtonIsDown = false;
			return;
		}
		float num = UniversalInputManager.Get().GetMousePosition().y - this.m_lastMouseDownPosition.y;
		float num2 = UniversalInputManager.Get().GetMousePosition().x - this.m_lastMouseDownPosition.x;
		if (num2 > -20f && num2 < 20f && num > -20f && num < 20f)
		{
			return;
		}
		bool flag = !UniversalInputManager.Get().IsTouchMode() || num > this.MIN_GRAB_Y;
		CardStandIn cardStandIn = this.m_lastObjectMousedDown.GetComponent<CardStandIn>();
		if (cardStandIn != null && GameState.Get() != null && !GameState.Get().IsMulliganManagerActive())
		{
			if (UniversalInputManager.Get().IsTouchMode())
			{
				if (!flag)
				{
					return;
				}
				cardStandIn = this.m_myHandZone.CurrentStandIn;
				if (cardStandIn == null)
				{
					return;
				}
			}
			if (!ChoiceCardMgr.Get().IsFriendlyShown() && this.GetBattlecrySourceCard() == null && this.IsInZone(cardStandIn.linkedCard, TAG_ZONE.HAND))
			{
				this.m_dragging = true;
				this.GrabCard(cardStandIn.linkedCard.gameObject);
			}
			return;
		}
		if (GameState.Get().IsMulliganManagerActive())
		{
			return;
		}
		if (GameState.Get().IsInTargetMode())
		{
			return;
		}
		Card component = this.m_lastObjectMousedDown.GetComponent<Card>();
		Entity entity = component.GetEntity();
		if (entity.IsControlledByLocalUser())
		{
			if (this.IsInZone(component, TAG_ZONE.HAND))
			{
				if (!flag || (UniversalInputManager.Get().IsTouchMode() && !GameState.Get().HasResponse(entity)))
				{
					return;
				}
				if (ChoiceCardMgr.Get().IsFriendlyShown() && this.GetBattlecrySourceCard() == null)
				{
					this.m_dragging = true;
					this.GrabCard(this.m_lastObjectMousedDown);
				}
			}
			else if (this.IsInZone(component, TAG_ZONE.PLAY) && (!entity.IsHeroPower() || (entity.IsHeroPower() && GameState.Get().EntityHasTargets(entity))))
			{
				this.m_dragging = true;
				this.HandleClickOnCardInBattlefield(entity);
			}
		}
	}

	// Token: 0x06001EAA RID: 7850 RVA: 0x0008F518 File Offset: 0x0008D718
	private void HandleUpdateWhileHoldingCard(bool hitBattlefield)
	{
		PegCursor.Get().SetMode(PegCursor.Mode.DRAG);
		Card heldCard = this.m_heldCard;
		if (!heldCard.IsInputEnabled())
		{
			this.DropHeldCard();
			return;
		}
		Entity entity = heldCard.GetEntity();
		if (hitBattlefield && TargetReticleManager.Get() && !TargetReticleManager.Get().IsActive() && GameState.Get().EntityHasTargets(entity) && entity.GetCardType() != TAG_CARDTYPE.MINION)
		{
			if (!this.DoNetworkResponse(entity, true))
			{
				this.PositionHeldCard();
				return;
			}
			DragCardSoundEffects component = heldCard.GetComponent<DragCardSoundEffects>();
			if (component)
			{
				component.Disable();
			}
			RemoteActionHandler.Get().NotifyOpponentOfCardPickedUp(heldCard);
			RemoteActionHandler.Get().NotifyOpponentOfTargetModeBegin(heldCard);
			Entity hero = entity.GetHero();
			TargetReticleManager.Get().CreateFriendlyTargetArrow(hero, entity, true, true, null, false);
			this.ActivatePowerUpSpell(heldCard);
			this.ActivatePlaySpell(heldCard);
		}
		else
		{
			if (hitBattlefield && this.m_cardWasInsideHandLastFrame)
			{
				RemoteActionHandler.Get().NotifyOpponentOfCardPickedUp(heldCard);
				this.m_cardWasInsideHandLastFrame = false;
			}
			else if (!hitBattlefield)
			{
				this.m_cardWasInsideHandLastFrame = true;
			}
			this.PositionHeldCard();
			if (GameState.Get().GetResponseMode() == GameState.ResponseMode.SUB_OPTION)
			{
				this.CancelSubOptionMode();
			}
		}
		if (UniversalInputManager.Get().IsTouchMode() && !hitBattlefield && this.m_heldCard != null && UniversalInputManager.Get().GetMousePosition().y - this.m_lastMouseDownPosition.y < this.MIN_GRAB_Y)
		{
			PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
			this.ReturnHeldCardToHand();
			return;
		}
	}

	// Token: 0x06001EAB RID: 7851 RVA: 0x0008F6B8 File Offset: 0x0008D8B8
	private void ActivatePowerUpSpell(Card card)
	{
		Entity entity = card.GetEntity();
		if (entity.IsSpell())
		{
			Spell actorSpell = card.GetActorSpell(SpellType.POWER_UP, true);
			if (actorSpell != null)
			{
				actorSpell.ActivateState(SpellStateType.BIRTH);
			}
		}
		card.DeactivateHandStateSpells();
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x0008F6FC File Offset: 0x0008D8FC
	private void ActivatePlaySpell(Card card)
	{
		Entity entity = card.GetEntity();
		Entity parentEntity = entity.GetParentEntity();
		Spell spell;
		if (parentEntity == null)
		{
			spell = card.GetPlaySpell(true);
		}
		else
		{
			Card card2 = parentEntity.GetCard();
			int subCardIndex = parentEntity.GetSubCardIndex(entity);
			spell = card2.GetSubOptionSpell(subCardIndex, true);
		}
		if (spell != null)
		{
			spell.ActivateState(SpellStateType.BIRTH);
		}
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x0008F757 File Offset: 0x0008D957
	private void HandleMouseMove()
	{
		if (GameState.Get().IsInTargetMode())
		{
			this.HandleUpdateWhileNotHoldingCard();
		}
	}

	// Token: 0x06001EAE RID: 7854 RVA: 0x0008F770 File Offset: 0x0008D970
	private void HandleUpdateWhileNotHoldingCard()
	{
		if (!UniversalInputManager.Get().IsTouchMode() || !TargetReticleManager.Get().IsLocalArrowActive())
		{
			this.m_myHandZone.HandleInput();
		}
		bool flag = UniversalInputManager.Get().IsTouchMode() && !UniversalInputManager.Get().GetMouseButton(0);
		RaycastHit hitInfo;
		bool inputHitInfo = UniversalInputManager.Get().GetInputHitInfo(GameLayer.CardRaycast, out hitInfo);
		if (flag || !inputHitInfo)
		{
			if (this.m_mousedOverCard)
			{
				this.HandleMouseOffCard();
			}
			this.HandleMouseOff();
			return;
		}
		CardStandIn cardStandIn = SceneUtils.FindComponentInParents<CardStandIn>(hitInfo.transform);
		Actor actor = SceneUtils.FindComponentInParents<Actor>(hitInfo.transform);
		if (actor == null && cardStandIn == null)
		{
			this.HandleMouseOverObjectWhileNotHoldingCard(hitInfo);
			return;
		}
		if (this.m_mousedOverObject != null)
		{
			this.HandleMouseOffLastObject();
		}
		Card card = null;
		if (actor != null)
		{
			card = actor.GetCard();
		}
		if (card == null)
		{
			if (cardStandIn == null)
			{
				return;
			}
			if (GameState.Get() == null || GameState.Get().IsMulliganManagerActive())
			{
				return;
			}
			card = cardStandIn.linkedCard;
		}
		if (this.IsCancelingBattlecryCard(card))
		{
			return;
		}
		if (card != this.m_mousedOverCard && (card.GetZone() != this.m_myHandZone || GameState.Get().IsMulliganManagerActive()))
		{
			if (this.m_mousedOverCard != null)
			{
				this.HandleMouseOffCard();
			}
			this.HandleMouseOverCard(card);
		}
		PegCursor.Get().SetMode(PegCursor.Mode.OVER);
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x0008F918 File Offset: 0x0008DB18
	private void HandleMouseOverObjectWhileNotHoldingCard(RaycastHit hitInfo)
	{
		GameObject gameObject = hitInfo.collider.gameObject;
		if (this.m_mousedOverCard != null)
		{
			this.HandleMouseOffCard();
		}
		if (UniversalInputManager.Get().IsTouchMode() && !UniversalInputManager.Get().GetMouseButton(0))
		{
			if (this.m_mousedOverObject != null)
			{
				this.HandleMouseOffLastObject();
			}
			return;
		}
		bool flag = TargetReticleManager.Get() != null && TargetReticleManager.Get().IsLocalArrowActive();
		if (GameMgr.Get() != null && GameMgr.Get().IsSpectator())
		{
			flag = false;
		}
		if (gameObject.GetComponent<HistoryManager>() != null && !flag)
		{
			this.m_mousedOverObject = gameObject;
			HistoryManager.Get().NotifyOfInput(hitInfo.point.z);
			return;
		}
		if (this.m_mousedOverObject == gameObject)
		{
			return;
		}
		if (this.m_mousedOverObject != null)
		{
			this.HandleMouseOffLastObject();
		}
		if (EndTurnButton.Get() && !GameMgr.Get().IsSpectator())
		{
			if (gameObject.GetComponent<EndTurnButton>() != null)
			{
				this.m_mousedOverObject = gameObject;
				EndTurnButton.Get().HandleMouseOver();
			}
			else if (gameObject.GetComponent<EndTurnButtonReminder>() != null)
			{
				EndTurnButtonReminder component = gameObject.GetComponent<EndTurnButtonReminder>();
				if (component.ShowFriendlySidePlayerTurnReminder())
				{
					this.m_mousedOverObject = gameObject;
				}
			}
		}
		TooltipZone component2 = gameObject.GetComponent<TooltipZone>();
		if (component2 != null)
		{
			this.m_mousedOverObject = gameObject;
			this.ShowTooltipZone(gameObject, component2);
		}
		GameOpenPack component3 = gameObject.GetComponent<GameOpenPack>();
		if (component3 != null)
		{
			this.m_mousedOverObject = gameObject;
			component3.NotifyOfMouseOver();
		}
		if (this.GetBattlecrySourceCard() != null)
		{
			return;
		}
		bool flag2 = UniversalInputManager.Get().InputHitAnyObject(Camera.main, GameLayer.InvisibleHitBox1);
		if (flag2 && ChoiceCardMgr.Get().HasSubOption())
		{
			this.CancelSubOptionMode();
		}
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x0008FB14 File Offset: 0x0008DD14
	private void HandleMouseOff()
	{
		if (this.m_mousedOverCard)
		{
			this.HandleMouseOffCard();
		}
		if (this.m_mousedOverObject)
		{
			this.HandleMouseOffLastObject();
		}
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x0008FB50 File Offset: 0x0008DD50
	private void HandleMouseOffLastObject()
	{
		if (this.m_mousedOverObject.GetComponent<EndTurnButton>())
		{
			this.m_mousedOverObject.GetComponent<EndTurnButton>().HandleMouseOut();
			this.m_lastObjectMousedDown = null;
		}
		else if (this.m_mousedOverObject.GetComponent<EndTurnButtonReminder>())
		{
			this.m_lastObjectMousedDown = null;
		}
		else if (this.m_mousedOverObject.GetComponent<TooltipZone>() != null)
		{
			this.m_mousedOverObject.GetComponent<TooltipZone>().HideTooltip();
			this.m_lastObjectMousedDown = null;
		}
		else if (this.m_mousedOverObject.GetComponent<HistoryManager>() != null)
		{
			HistoryManager.Get().NotifyOfMouseOff();
		}
		else if (this.m_mousedOverObject.GetComponent<GameOpenPack>() != null)
		{
			this.m_mousedOverObject.GetComponent<GameOpenPack>().NotifyOfMouseOff();
			this.m_lastObjectMousedDown = null;
		}
		this.m_mousedOverObject = null;
		this.HideBigViewCardBacks();
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x0008FC40 File Offset: 0x0008DE40
	private void GrabCard(GameObject cardObject)
	{
		if (GameMgr.Get().IsSpectator())
		{
			return;
		}
		Card component = cardObject.GetComponent<Card>();
		if (!component.IsInputEnabled())
		{
			return;
		}
		if (!GameState.Get().GetGameEntity().ShouldAllowCardGrab(component.GetEntity()))
		{
			return;
		}
		Zone zone = component.GetZone();
		if (!zone.IsInputEnabled())
		{
			return;
		}
		component.SetDoNotSort(true);
		if (zone is ZoneHand && !UniversalInputManager.Get().IsTouchMode())
		{
			ZoneHand zoneHand = (ZoneHand)zone;
			zoneHand.UpdateLayout(-1);
		}
		this.m_heldCard = component;
		SoundManager.Get().LoadAndPlay("FX_MinionSummon01_DrawFromHand_01", cardObject);
		DragCardSoundEffects dragCardSoundEffects = this.m_heldCard.GetComponent<DragCardSoundEffects>();
		if (dragCardSoundEffects)
		{
			dragCardSoundEffects.enabled = true;
		}
		else
		{
			dragCardSoundEffects = cardObject.AddComponent<DragCardSoundEffects>();
		}
		dragCardSoundEffects.Restart();
		DragRotator dragRotator = cardObject.AddComponent<DragRotator>();
		dragRotator.SetInfo(this.m_DragRotatorInfo);
		ProjectedShadow componentInChildren = component.GetActor().GetComponentInChildren<ProjectedShadow>();
		if (componentInChildren != null)
		{
			componentInChildren.EnableShadow(0.15f);
		}
		iTween.Stop(cardObject);
		float num = 0.7f;
		iTween.ScaleTo(cardObject, new Vector3(num, num, num), 0.2f);
		KeywordHelpPanelManager.Get().HideKeywordHelp();
		if (CardTypeBanner.Get())
		{
			CardTypeBanner.Get().Hide();
		}
		component.NotifyPickedUp();
		GameState.Get().GetGameEntity().NotifyOfCardGrabbed(component.GetEntity());
		SceneUtils.SetLayer(component, GameLayer.Default);
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x0008FDB7 File Offset: 0x0008DFB7
	private void DropCanceledHeldCard(Card card)
	{
		this.m_heldCard = null;
		RemoteActionHandler.Get().NotifyOpponentOfCardDropped();
		this.m_myHandZone.UpdateLayout(-1, true);
		this.m_myPlayZone.SortWithSpotForHeldCard(-1);
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x0008FDE4 File Offset: 0x0008DFE4
	public void ReturnHeldCardToHand()
	{
		if (this.m_heldCard == null)
		{
			return;
		}
		Log.Hand.Print("ReturnHeldCardToHand()", new object[0]);
		Card heldCard = this.m_heldCard;
		heldCard.SetDoNotSort(false);
		iTween.Stop(this.m_heldCard.gameObject);
		Entity entity = heldCard.GetEntity();
		heldCard.NotifyLeftPlayfield();
		GameState.Get().GetGameEntity().NotifyOfCardDropped(entity);
		DragCardSoundEffects component = heldCard.GetComponent<DragCardSoundEffects>();
		if (component)
		{
			component.Disable();
		}
		Object.Destroy(this.m_heldCard.GetComponent<DragRotator>());
		ProjectedShadow componentInChildren = heldCard.GetActor().GetComponentInChildren<ProjectedShadow>();
		if (componentInChildren != null)
		{
			componentInChildren.DisableShadow();
		}
		RemoteActionHandler.Get().NotifyOpponentOfCardDropped();
		if (this.m_useHandEnlarge)
		{
			this.m_myHandZone.SetFriendlyHeroTargetingMode(false);
		}
		this.m_myHandZone.UpdateLayout(this.m_myHandZone.GetLastMousedOverCard(), true);
		this.m_dragging = false;
		this.m_heldCard = null;
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x0008FEE0 File Offset: 0x0008E0E0
	private bool DropHeldCard(bool wasCancelled)
	{
		Log.Hand.Print("DropHeldCard - cancelled? " + wasCancelled, new object[0]);
		PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
		if (this.m_useHandEnlarge)
		{
			this.m_myHandZone.SetFriendlyHeroTargetingMode(false);
			if (this.m_hideHandAfterPlayingCard)
			{
				this.HidePhoneHand();
			}
			else
			{
				this.m_myHandZone.UpdateLayout(-1, true);
			}
		}
		if (this.m_heldCard == null)
		{
			return false;
		}
		Card heldCard = this.m_heldCard;
		heldCard.SetDoNotSort(false);
		iTween.Stop(this.m_heldCard.gameObject);
		Entity entity = heldCard.GetEntity();
		heldCard.NotifyLeftPlayfield();
		GameState.Get().GetGameEntity().NotifyOfCardDropped(entity);
		DragCardSoundEffects component = heldCard.GetComponent<DragCardSoundEffects>();
		if (component)
		{
			component.Disable();
		}
		Object.Destroy(this.m_heldCard.GetComponent<DragRotator>());
		this.m_heldCard = null;
		ProjectedShadow componentInChildren = heldCard.GetActor().GetComponentInChildren<ProjectedShadow>();
		if (componentInChildren != null)
		{
			componentInChildren.DisableShadow();
		}
		if (wasCancelled)
		{
			this.DropCanceledHeldCard(heldCard);
			return true;
		}
		bool flag = false;
		if (this.IsInZone(heldCard, TAG_ZONE.HAND))
		{
			bool flag2 = entity.IsMinion();
			bool flag3 = entity.IsWeapon();
			if (flag2 || flag3)
			{
				RaycastHit raycastHit;
				if (UniversalInputManager.Get().GetInputHitInfo(Camera.main, GameLayer.InvisibleHitBox2, out raycastHit))
				{
					Zone zone = (!flag3) ? this.m_myPlayZone : this.m_myWeaponZone;
					if (zone)
					{
						GameState gameState = GameState.Get();
						int num = 0;
						int num2 = 0;
						if (flag2)
						{
							num = this.PlayZoneSlotMousedOver(heldCard) + 1;
							num2 = ZoneMgr.Get().PredictZonePosition(zone, num);
							gameState.SetSelectedOptionPosition(num2);
						}
						if (this.DoNetworkResponse(entity, true))
						{
							this.m_lastZoneChangeList = ZoneMgr.Get().AddPredictedLocalZoneChange(heldCard, zone, num, num2);
							this.PredictSpentMana(entity);
							if (flag2 && gameState.EntityHasTargets(entity))
							{
								flag = true;
								bool showArrow = !UniversalInputManager.Get().IsTouchMode();
								if (TargetReticleManager.Get())
								{
									TargetReticleManager.Get().CreateFriendlyTargetArrow(entity, entity, true, showArrow, null, false);
								}
								this.m_battlecrySourceCard = heldCard;
								if (UniversalInputManager.Get().IsTouchMode())
								{
									this.StartBattleCryEffect(entity);
								}
							}
						}
						else
						{
							gameState.SetSelectedOptionPosition(0);
						}
					}
				}
			}
			else if (entity.IsSpell())
			{
				if (GameState.Get().EntityHasTargets(entity))
				{
					this.DropCanceledHeldCard(entity.GetCard());
					return true;
				}
				RaycastHit raycastHit2;
				if (UniversalInputManager.Get().GetInputHitInfo(Camera.main, GameLayer.InvisibleHitBox2, out raycastHit2))
				{
					if (!GameState.Get().HasResponse(entity))
					{
						PlayErrors.DisplayPlayError(PlayErrors.GetPlayEntityError(entity), entity);
					}
					else
					{
						this.DoNetworkResponse(entity, true);
						this.m_lastZoneChangeList = ZoneMgr.Get().AddLocalZoneChange(heldCard, TAG_ZONE.PLAY);
						this.PredictSpentMana(entity);
						if (GameState.Get().HasSubOptions(entity))
						{
							heldCard.DeactivateHandStateSpells();
						}
						else
						{
							this.ActivatePowerUpSpell(heldCard);
							this.ActivatePlaySpell(heldCard);
						}
					}
				}
			}
			this.m_myHandZone.UpdateLayout(-1, true);
			this.m_myPlayZone.SortWithSpotForHeldCard(-1);
		}
		if (flag)
		{
			if (RemoteActionHandler.Get())
			{
				RemoteActionHandler.Get().NotifyOpponentOfTargetModeBegin(heldCard);
			}
		}
		else if (GameState.Get().GetResponseMode() != GameState.ResponseMode.SUB_OPTION)
		{
			RemoteActionHandler.Get().NotifyOpponentOfCardDropped();
		}
		return true;
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x00090248 File Offset: 0x0008E448
	private void HandleRightClickOnCard(Card card)
	{
		if (GameState.Get().IsInTargetMode() || GameState.Get().IsInSubOptionMode() || this.m_heldCard != null)
		{
			this.HandleRightClick();
			return;
		}
		if (card.GetEntity().IsHero())
		{
			if (card.GetEntity().IsControlledByLocalUser())
			{
				if (EmoteHandler.Get() != null)
				{
					if (EmoteHandler.Get().AreEmotesActive())
					{
						EmoteHandler.Get().HideEmotes();
					}
					else
					{
						EmoteHandler.Get().ShowEmotes();
					}
					return;
				}
			}
			else
			{
				bool flag = EnemyEmoteHandler.Get() != null;
				if (GameMgr.Get().IsSpectator() && card.GetEntity().GetControllerSide() != Player.Side.OPPOSING)
				{
					flag = false;
				}
				if (flag)
				{
					if (EnemyEmoteHandler.Get().AreEmotesActive())
					{
						EnemyEmoteHandler.Get().HideEmotes();
					}
					else
					{
						EnemyEmoteHandler.Get().ShowEmotes();
					}
					return;
				}
			}
		}
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x00090348 File Offset: 0x0008E548
	private void HandleClickOnCard(GameObject upClickedCard, bool wasMouseDownTarget)
	{
		if (EmoteHandler.Get() != null)
		{
			if (EmoteHandler.Get().IsMouseOverEmoteOption())
			{
				return;
			}
			EmoteHandler.Get().HideEmotes();
		}
		if (EnemyEmoteHandler.Get() != null)
		{
			if (EnemyEmoteHandler.Get().IsMouseOverEmoteOption())
			{
				return;
			}
			EnemyEmoteHandler.Get().HideEmotes();
		}
		Card component = upClickedCard.GetComponent<Card>();
		Entity entity = component.GetEntity();
		Log.Hand.Print("HandleClickOnCard - Card zone: " + component.GetZone(), new object[0]);
		if (UniversalInputManager.Get().IsTouchMode() && entity.IsHero() && !GameState.Get().IsInTargetMode() && wasMouseDownTarget)
		{
			if (entity.IsControlledByLocalUser())
			{
				if (EmoteHandler.Get() != null)
				{
					EmoteHandler.Get().ShowEmotes();
				}
				return;
			}
			if (!GameMgr.Get().IsSpectator() && EnemyEmoteHandler.Get() != null)
			{
				EnemyEmoteHandler.Get().ShowEmotes();
				return;
			}
		}
		if (component == ChoiceCardMgr.Get().GetSubOptionParentCard())
		{
			this.CancelOption();
			return;
		}
		GameState.ResponseMode responseMode = GameState.Get().GetResponseMode();
		if (this.IsInZone(component, TAG_ZONE.HAND))
		{
			if (GameState.Get().IsMulliganManagerActive())
			{
				if (!GameMgr.Get().IsSpectator())
				{
					MulliganManager.Get().ToggleHoldState(component);
				}
			}
			else
			{
				if (component.IsAttacking())
				{
					return;
				}
				if (UniversalInputManager.Get().IsTouchMode())
				{
					return;
				}
				if (!ChoiceCardMgr.Get().IsFriendlyShown() && this.GetBattlecrySourceCard() == null)
				{
					this.GrabCard(upClickedCard);
				}
			}
			return;
		}
		if (responseMode == GameState.ResponseMode.SUB_OPTION)
		{
			this.HandleClickOnSubOption(entity, false);
			return;
		}
		if (responseMode == GameState.ResponseMode.CHOICE)
		{
			this.HandleClickOnChoice(entity);
			return;
		}
		if (this.IsInZone(component, TAG_ZONE.PLAY))
		{
			this.HandleClickOnCardInBattlefield(entity);
			return;
		}
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x00090534 File Offset: 0x0008E734
	private void HandleClickOnCardInBattlefield(Entity clickedEntity)
	{
		if (GameMgr.Get().IsSpectator())
		{
			return;
		}
		PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
		GameState gameState = GameState.Get();
		Card card = clickedEntity.GetCard();
		if (UniversalInputManager.Get().IsTouchMode() && clickedEntity.IsHeroPower() && this.m_mousedOverTimer > this.m_MouseOverDelay)
		{
			return;
		}
		if (!gameState.GetGameEntity().NotifyOfBattlefieldCardClicked(clickedEntity, gameState.IsInTargetMode()))
		{
			return;
		}
		if (gameState.IsInTargetMode())
		{
			this.DisableSkullIfNeeded(card);
			Network.Options.Option.SubOption selectedNetworkSubOption = gameState.GetSelectedNetworkSubOption();
			if (selectedNetworkSubOption.ID == clickedEntity.GetEntityId())
			{
				this.CancelOption();
				return;
			}
			if (this.DoNetworkResponse(clickedEntity, true) && this.m_heldCard != null)
			{
				Card heldCard = this.m_heldCard;
				this.m_heldCard = null;
				heldCard.SetDoNotSort(false);
				this.m_lastZoneChangeList = ZoneMgr.Get().AddLocalZoneChange(heldCard, TAG_ZONE.PLAY);
			}
			return;
		}
		else
		{
			if (UniversalInputManager.Get().IsTouchMode() && UniversalInputManager.Get().GetMouseButtonUp(0) && gameState.EntityHasTargets(clickedEntity))
			{
				if (!card.IsShowingTooltip() && gameState.IsFriendlySidePlayerTurn())
				{
					PlayErrors.DisplayPlayError(PlayErrors.ErrorType.REQ_DRAG_TO_PLAY, clickedEntity);
				}
				return;
			}
			if (clickedEntity.IsWeapon() && clickedEntity.IsControlledByLocalUser())
			{
				this.HandleClickOnCardInBattlefield(gameState.GetFriendlySidePlayer().GetHero());
				return;
			}
			if (!this.DoNetworkResponse(clickedEntity, true))
			{
				return;
			}
			if (!gameState.IsInTargetMode())
			{
				if (clickedEntity.IsHeroPower())
				{
					this.ActivatePlaySpell(card);
					clickedEntity.SetTagAndHandleChange<int>(GAME_TAG.EXHAUSTED, 1);
					this.PredictSpentMana(clickedEntity);
				}
				return;
			}
			RemoteActionHandler.Get().NotifyOpponentOfTargetModeBegin(card);
			if (TargetReticleManager.Get())
			{
				TargetReticleManager.Get().CreateFriendlyTargetArrow(clickedEntity, clickedEntity, false, true, null, false);
			}
			if (clickedEntity.IsHeroPower())
			{
				this.m_targettingHeroPower = true;
				this.ActivatePlaySpell(card);
				return;
			}
			if (!clickedEntity.IsCharacter())
			{
				return;
			}
			card.ActivateCharacterAttackEffects();
			gameState.ShowEnemyTauntCharacters();
			if (!card.IsAttacking())
			{
				Spell actorAttackSpellForInput = card.GetActorAttackSpellForInput();
				if (actorAttackSpellForInput != null)
				{
					if (clickedEntity.HasTag(GAME_TAG.IMMUNE_WHILE_ATTACKING))
					{
						card.GetActor().ActivateSpell(SpellType.IMMUNE);
					}
					actorAttackSpellForInput.ActivateState(SpellStateType.BIRTH);
				}
			}
			return;
		}
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x00090778 File Offset: 0x0008E978
	private void HandleClickOnSubOption(Entity entity, bool isSimulated = false)
	{
		if (isSimulated || GameState.Get().HasResponse(entity))
		{
			bool flag = false;
			Card subOptionParentCard = ChoiceCardMgr.Get().GetSubOptionParentCard();
			if (!isSimulated)
			{
				flag = GameState.Get().SubEntityHasTargets(entity);
				if (flag)
				{
					RemoteActionHandler.Get().NotifyOpponentOfTargetModeBegin(subOptionParentCard);
					Entity hero = entity.GetHero();
					Entity entity2 = subOptionParentCard.GetEntity();
					TargetReticleManager.Get().CreateFriendlyTargetArrow(hero, entity2, true, !UniversalInputManager.Get().IsTouchMode(), entity.GetCardTextInHand(), false);
				}
			}
			Card card = entity.GetCard();
			if (!isSimulated)
			{
				this.DoNetworkResponse(entity, true);
			}
			this.ActivatePowerUpSpell(card);
			this.ActivatePlaySpell(card);
			if (entity.IsMinion())
			{
				card.HideCard();
			}
			ChoiceCardMgr.Get().OnSubOptionClicked(entity);
			if (!isSimulated && !flag)
			{
				this.FinishSubOptions();
			}
			if (UniversalInputManager.Get().IsTouchMode() && !isSimulated && flag)
			{
				this.StartMobileTargetingEffect(GameState.Get().GetSelectedNetworkSubOption().Targets);
			}
		}
		else
		{
			PlayErrors.DisplayPlayError(PlayErrors.GetPlayEntityError(entity), entity);
		}
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x00090894 File Offset: 0x0008EA94
	private void HandleClickOnChoice(Entity entity)
	{
		if (GameMgr.Get().IsSpectator())
		{
			return;
		}
		if (this.DoNetworkResponse(entity, true))
		{
			SoundManager.Get().LoadAndPlay("HeroDropItem1");
		}
		else
		{
			PlayErrors.DisplayPlayError(PlayErrors.GetPlayEntityError(entity), entity);
		}
	}

	// Token: 0x06001EBB RID: 7867 RVA: 0x000908E0 File Offset: 0x0008EAE0
	public void ResetBattlecrySourceCard()
	{
		if (this.m_battlecrySourceCard == null)
		{
			return;
		}
		if (UniversalInputManager.Get().IsTouchMode())
		{
			string message = GameStrings.Get("GAMEPLAY_MOBILE_BATTLECRY_CANCELED");
			GameplayErrorManager.Get().DisplayMessage(message);
		}
		this.m_cancelingBattlecryCards.Add(this.m_battlecrySourceCard);
		Entity entity = this.m_battlecrySourceCard.GetEntity();
		Spell actorSpell = this.m_battlecrySourceCard.GetActorSpell(SpellType.BATTLECRY, true);
		if (actorSpell)
		{
			actorSpell.ActivateState(SpellStateType.CANCEL);
		}
		Spell playSpell = this.m_battlecrySourceCard.GetPlaySpell(true);
		if (playSpell)
		{
			playSpell.ActivateState(SpellStateType.CANCEL);
		}
		Spell customSummonSpell = this.m_battlecrySourceCard.GetCustomSummonSpell();
		if (customSummonSpell)
		{
			customSummonSpell.ActivateState(SpellStateType.CANCEL);
		}
		ZoneMgr.ChangeCompleteCallback callback = delegate(ZoneChangeList changeList, object userData)
		{
			Card card = (Card)userData;
			this.m_cancelingBattlecryCards.Remove(card);
		};
		ZoneMgr.Get().CancelLocalZoneChange(this.m_lastZoneChangeList, callback, this.m_battlecrySourceCard);
		this.m_lastZoneChangeList = null;
		this.RollbackSpentMana(entity);
		this.ClearBattlecrySourceCard();
	}

	// Token: 0x06001EBC RID: 7868 RVA: 0x000909DD File Offset: 0x0008EBDD
	private bool IsCancelingBattlecryCard(Card card)
	{
		return this.m_cancelingBattlecryCards.Contains(card);
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x000909EC File Offset: 0x0008EBEC
	public void DoEndTurnButton()
	{
		if (GameMgr.Get().IsSpectator())
		{
			return;
		}
		GameState gameState = GameState.Get();
		if (gameState.IsResponsePacketBlocked())
		{
			return;
		}
		if (EndTurnButton.Get().IsInputBlocked())
		{
			return;
		}
		switch (gameState.GetResponseMode())
		{
		case GameState.ResponseMode.OPTION:
		{
			Network.Options optionsPacket = gameState.GetOptionsPacket();
			for (int i = 0; i < optionsPacket.List.Count; i++)
			{
				Network.Options.Option option = optionsPacket.List[i];
				if (option.Type == Network.Options.Option.OptionType.END_TURN || option.Type == Network.Options.Option.OptionType.PASS)
				{
					if (gameState.GetGameEntity().NotifyOfEndTurnButtonPushed())
					{
						gameState.SetSelectedOption(i);
						gameState.SendOption();
						this.HidePhoneHand();
						this.DoEndTurnButton_Option_OnEndTurnRequested();
					}
					break;
				}
			}
			break;
		}
		case GameState.ResponseMode.CHOICE:
		{
			Network.EntityChoices friendlyEntityChoices = gameState.GetFriendlyEntityChoices();
			List<Entity> chosenEntities = gameState.GetChosenEntities();
			if (chosenEntities.Count >= friendlyEntityChoices.CountMin)
			{
				ChoiceCardMgr.Get().OnSendChoices(friendlyEntityChoices, chosenEntities);
				gameState.SendChoices();
			}
			break;
		}
		}
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x00090B15 File Offset: 0x0008ED15
	private void DoEndTurnButton_Option_OnEndTurnRequested()
	{
		if (TurnTimer.Get() != null)
		{
			TurnTimer.Get().OnEndTurnRequested();
		}
		EndTurnButton.Get().OnEndTurnRequested();
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x00090B3C File Offset: 0x0008ED3C
	public bool DoNetworkResponse(Entity entity, bool checkValidInput = true)
	{
		if (ThinkEmoteManager.Get() != null)
		{
			ThinkEmoteManager.Get().NotifyOfActivity();
		}
		GameState gameState = GameState.Get();
		if (checkValidInput && !gameState.IsEntityInputEnabled(entity))
		{
			return false;
		}
		GameState.ResponseMode responseMode = gameState.GetResponseMode();
		bool flag = false;
		switch (responseMode)
		{
		case GameState.ResponseMode.OPTION:
			flag = this.DoNetworkOptions(entity);
			break;
		case GameState.ResponseMode.SUB_OPTION:
			flag = this.DoNetworkSubOptions(entity);
			break;
		case GameState.ResponseMode.OPTION_TARGET:
			flag = this.DoNetworkOptionTarget(entity, null);
			break;
		case GameState.ResponseMode.CHOICE:
			flag = this.DoNetworkChoice(entity);
			break;
		}
		if (flag)
		{
			Card card = entity.GetCard();
			card.UpdateActorState();
		}
		return flag;
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x00090BF5 File Offset: 0x0008EDF5
	private void OnOptionsReceived(object userData)
	{
		if (this.m_mousedOverCard)
		{
			this.m_mousedOverCard.UpdateProposedManaUsage();
		}
		this.HidePhoneHandIfOutOfServerPlays();
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x00090C18 File Offset: 0x0008EE18
	private void OnCurrentPlayerChanged(Player player, object userData)
	{
		if (player.IsLocalUser())
		{
			this.m_entitiesThatPredictedMana.Clear();
		}
	}

	// Token: 0x06001EC2 RID: 7874 RVA: 0x00090C30 File Offset: 0x0008EE30
	private void OnOptionRejected(Network.Options.Option option, object userData)
	{
		if (option.Type == Network.Options.Option.OptionType.POWER)
		{
			Entity entity = GameState.Get().GetEntity(option.Main.ID);
			Card card = entity.GetCard();
			card.NotifyTargetingCanceled();
			this.RollbackSpentMana(entity);
		}
		string message = GameStrings.Get("GAMEPLAY_ERROR_PLAY_REJECTED");
		GameplayErrorManager.Get().DisplayMessage(message);
	}

	// Token: 0x06001EC3 RID: 7875 RVA: 0x00090C89 File Offset: 0x0008EE89
	private void OnTurnTimerUpdate(TurnTimerUpdate update, object userData)
	{
		if (update.GetSecondsRemaining() > Mathf.Epsilon)
		{
			return;
		}
		this.CancelOption();
	}

	// Token: 0x06001EC4 RID: 7876 RVA: 0x00090CA3 File Offset: 0x0008EEA3
	private void OnGameOver(object userData)
	{
		this.CancelOption();
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x00090CAC File Offset: 0x0008EEAC
	private void OnSpectatorNotifyEvent(SpectatorNotify notify, object userData)
	{
		if (!GameMgr.Get().IsSpectator())
		{
			return;
		}
		GameState gameState = GameState.Get();
		if (gameState == null || !gameState.IsGameCreatedOrCreating() || gameState.IsGameOverNowOrPending())
		{
			return;
		}
		if (notify.HasChooseOption)
		{
			bool flag = true;
			ChooseOption chooseOption = notify.ChooseOption;
			if (notify.PlayerId != gameState.GetCurrentPlayer().GetPlayerId())
			{
				Log.Power.Print("Spectator received ChooseOption for wrong player turn receivedPlayerId={0} receivedId={1} currentTurnPlayerId={2}", new object[]
				{
					notify.PlayerId,
					chooseOption.Id,
					gameState.GetCurrentPlayer().GetPlayerId()
				});
				return;
			}
			Network.Options optionsPacket = gameState.GetOptionsPacket();
			if (optionsPacket == null)
			{
				string text = string.Format("Spectator received SpectatorNotify while options is null receivedPlayerId={0} receivedId={1} currentTurnPlayerId={2}", notify.PlayerId, chooseOption.Id, gameState.GetCurrentPlayer().GetPlayerId());
				Log.Power.Print(text, new object[0]);
				Debug.LogError(text);
				return;
			}
			Network.Options.Option option = null;
			if (chooseOption.Index >= 0 && chooseOption.Index < optionsPacket.List.Count)
			{
				option = optionsPacket.List[chooseOption.Index];
			}
			if (optionsPacket == null || optionsPacket.ID != chooseOption.Id || option == null)
			{
				Log.Power.Print("Spectator received unexpected ChooseOption playerId={0} receivedId={1} receivedIndex={2} availId={3} availCount={4}", new object[]
				{
					notify.PlayerId,
					chooseOption.Id,
					chooseOption.Index,
					(optionsPacket != null) ? optionsPacket.ID.ToString() : "NULL",
					(optionsPacket != null) ? optionsPacket.List.Count.ToString() : "NULL"
				});
				return;
			}
			gameState.SetSelectedOption(chooseOption);
			Entity entity = gameState.GetEntity(option.Main.ID);
			if (option.Type == Network.Options.Option.OptionType.END_TURN || option.Type == Network.Options.Option.OptionType.PASS)
			{
				gameState.GetGameEntity().NotifyOfEndTurnButtonPushed();
				GameState.Get().ClearResponseMode();
				this.OnSpectatorNotifyEvent_UpdateHighlights();
				if (this.m_mousedOverCard != null)
				{
					GameState.Get().GetFriendlySidePlayer().CancelAllProposedMana(this.m_mousedOverCard.GetEntity());
				}
				this.DoEndTurnButton_Option_OnEndTurnRequested();
				return;
			}
			if (entity == null)
			{
				Log.Power.Print("Spectator received unknown entity in ChooseOption playerId={0} receivedId={1} entityId={2}", new object[]
				{
					notify.PlayerId,
					chooseOption.Id,
					option.Main.ID
				});
			}
			RemoteActionHandler remoteActionHandler = RemoteActionHandler.Get();
			if (remoteActionHandler != null && remoteActionHandler.GetFriendlyHoverCard() != null && remoteActionHandler.GetFriendlyHoverCard() == entity.GetCard())
			{
				remoteActionHandler.GetFriendlyHoverCard().NotifyMousedOut();
			}
			if (chooseOption.HasSubOption && chooseOption.SubOption >= 0)
			{
				if (option.Subs == null || chooseOption.SubOption >= option.Subs.Count)
				{
					Log.Power.Print("Spectator received unexpected ChooseOption SubOption playerId={0} receivedId={1} option={2} subOption={3} availSubOptions={4}", new object[]
					{
						notify.PlayerId,
						chooseOption.Id,
						chooseOption.Index,
						chooseOption.SubOption,
						(option.Subs != null) ? option.Subs.Count : 0
					});
					this.OnSpectatorNotifyEvent_UpdateHighlights();
					return;
				}
				this.DoNetworkResponse(entity, false);
				Network.Options.Option.SubOption subOption = option.Subs[chooseOption.SubOption];
				Entity entity2 = gameState.GetEntity(subOption.ID);
				if (entity2 == null)
				{
					Log.Power.Print("Spectator received unknown entity in ChooseOption SubOption playerId={0} receivedId={1} mainEntityId={2} subEntityId={3}", new object[]
					{
						notify.PlayerId,
						chooseOption.Id,
						option.Main.ID,
						subOption.ID
					});
					this.OnSpectatorNotifyEvent_UpdateHighlights();
					return;
				}
				this.m_spectatorNotifyCurrentToken += 1U;
				base.StartCoroutine(this.FinishSpectatorNotify_SubOption(this.m_spectatorNotifyCurrentToken, notify, chooseOption, option, entity2));
				flag = false;
			}
			else if (chooseOption.Target > 0)
			{
				Entity entity3 = gameState.GetEntity(chooseOption.Target);
				if (entity3 == null)
				{
					Log.Power.Print("Spectator received unknown target entity in ChooseOption playerId={0} receivedId={1} mainEntityId={2} targetEntityId={3}", new object[]
					{
						notify.PlayerId,
						chooseOption.Id,
						option.Main.ID,
						chooseOption.Target
					});
					this.OnSpectatorNotifyEvent_UpdateHighlights();
					return;
				}
				this.DoNetworkOptionTarget(entity3, entity);
			}
			if (flag)
			{
				this.OnSpectatorNotifyEvent_UpdateHighlights();
			}
		}
	}

	// Token: 0x06001EC6 RID: 7878 RVA: 0x000911B0 File Offset: 0x0008F3B0
	private void OnSpectatorNotifyEvent_UpdateHighlights()
	{
		Network.Options options = GameState.Get().GetOptionsPacket();
		if (options == null)
		{
			options = GameState.Get().GetLastOptions();
		}
		GameState.Get().UpdateOptionHighlights(options);
	}

	// Token: 0x06001EC7 RID: 7879 RVA: 0x000911E4 File Offset: 0x0008F3E4
	private IEnumerator FinishSpectatorNotify_SubOption(uint myToken, SpectatorNotify notify, ChooseOption echoPacket, Network.Options.Option chosenOption, Entity subEntity)
	{
		GameState state = GameState.Get();
		int intendedOptionPacketId = echoPacket.Id;
		while (ChoiceCardMgr.Get().IsWaitingToShowSubOptions())
		{
			yield return null;
			if (ChoiceCardMgr.Get() == null || !ChoiceCardMgr.Get().HasSubOption() || myToken != this.m_spectatorNotifyCurrentToken || state.GetOptionsPacket() == null || state.GetOptionsPacket().ID != intendedOptionPacketId)
			{
				yield break;
			}
		}
		List<Card> actualChoiceCards = ChoiceCardMgr.Get().GetFriendlyCards();
		List<Card> choiceCards = new List<Card>(actualChoiceCards);
		while (Enumerable.Any<Card>(choiceCards, (Card c) => !state.IsEntityInputEnabled(c.GetEntity())))
		{
			if (myToken != this.m_spectatorNotifyCurrentToken || state.GetOptionsPacket() == null || state.GetOptionsPacket().ID > intendedOptionPacketId + 1)
			{
				foreach (Card card in choiceCards)
				{
					card.HideCard();
				}
				this.OnSpectatorNotifyEvent_UpdateHighlights();
				yield break;
			}
			yield return null;
		}
		Entity targetEntity = null;
		if (echoPacket.Target > 0)
		{
			targetEntity = state.GetEntity(echoPacket.Target);
			if (targetEntity == null)
			{
				Log.Power.Print("Spectator received unknown target entity in ChooseOption SubOption playerId={0} receivedId={1} mainEntityId={2} subEntityId={3} targetEntityId={4}", new object[]
				{
					notify.PlayerId,
					echoPacket.Id,
					chosenOption.Main.ID,
					subEntity.GetEntityId(),
					echoPacket.Target
				});
				this.OnSpectatorNotifyEvent_UpdateHighlights();
				yield break;
			}
		}
		if (subEntity.GetCard() != null)
		{
			subEntity.GetCard().SetInputEnabled(false);
		}
		yield return new WaitForSeconds(1f);
		if (subEntity.GetCard() != null)
		{
			subEntity.GetCard().SetInputEnabled(true);
		}
		state = GameState.Get();
		if (state == null || state.IsGameOver() || myToken != this.m_spectatorNotifyCurrentToken || state.GetOptionsPacket() == null || state.GetOptionsPacket().ID > intendedOptionPacketId + 1)
		{
			foreach (Card card2 in choiceCards)
			{
				card2.HideCard();
			}
			this.OnSpectatorNotifyEvent_UpdateHighlights();
			yield break;
		}
		this.HandleClickOnSubOption(subEntity, true);
		if (state.GetOptionsPacket().ID == intendedOptionPacketId)
		{
			this.OnSpectatorNotifyEvent_UpdateHighlights();
		}
		else if (state.GetLastOptions() != null)
		{
			Network.Options lastOptions = state.GetLastOptions();
			state.UpdateOptionHighlights(lastOptions);
		}
		yield break;
	}

	// Token: 0x06001EC8 RID: 7880 RVA: 0x0009124C File Offset: 0x0008F44C
	private bool DoNetworkChoice(Entity entity)
	{
		GameState gameState = GameState.Get();
		if (!gameState.IsChoosableEntity(entity))
		{
			return false;
		}
		if (gameState.RemoveChosenEntity(entity))
		{
			return true;
		}
		gameState.AddChosenEntity(entity);
		Network.EntityChoices friendlyEntityChoices = gameState.GetFriendlyEntityChoices();
		if (friendlyEntityChoices.CountMax == 1)
		{
			List<Entity> chosenEntities = gameState.GetChosenEntities();
			ChoiceCardMgr.Get().OnSendChoices(friendlyEntityChoices, chosenEntities);
			gameState.SendChoices();
		}
		return true;
	}

	// Token: 0x06001EC9 RID: 7881 RVA: 0x000912B0 File Offset: 0x0008F4B0
	private bool DoNetworkOptions(Entity entity)
	{
		int entityId = entity.GetEntityId();
		GameState gameState = GameState.Get();
		Network.Options optionsPacket = gameState.GetOptionsPacket();
		for (int i = 0; i < optionsPacket.List.Count; i++)
		{
			Network.Options.Option option = optionsPacket.List[i];
			if (option.Type == Network.Options.Option.OptionType.POWER)
			{
				if (option.Main.ID == entityId)
				{
					gameState.SetSelectedOption(i);
					if (option.Subs.Count == 0)
					{
						if (option.Main.Targets == null || option.Main.Targets.Count == 0)
						{
							gameState.SendOption();
						}
						else
						{
							this.EnterOptionTargetMode();
						}
					}
					else
					{
						gameState.EnterSubOptionMode();
						Card card = entity.GetCard();
						ChoiceCardMgr.Get().ShowSubOptions(card);
					}
					return true;
				}
			}
		}
		if (!UniversalInputManager.Get().IsTouchMode() || !entity.GetCard().IsShowingTooltip())
		{
			PlayErrors.DisplayPlayError(PlayErrors.GetPlayEntityError(entity), entity);
		}
		return false;
	}

	// Token: 0x06001ECA RID: 7882 RVA: 0x000913C4 File Offset: 0x0008F5C4
	private bool DoNetworkSubOptions(Entity entity)
	{
		int entityId = entity.GetEntityId();
		GameState gameState = GameState.Get();
		Network.Options.Option selectedNetworkOption = gameState.GetSelectedNetworkOption();
		for (int i = 0; i < selectedNetworkOption.Subs.Count; i++)
		{
			Network.Options.Option.SubOption subOption = selectedNetworkOption.Subs[i];
			if (subOption.ID == entityId)
			{
				gameState.SetSelectedSubOption(i);
				if (subOption.Targets == null || subOption.Targets.Count == 0)
				{
					gameState.SendOption();
				}
				else
				{
					this.EnterOptionTargetMode();
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x0009145C File Offset: 0x0008F65C
	private bool DoNetworkOptionTarget(Entity entity, Entity simulatedSourceEntity = null)
	{
		bool flag = simulatedSourceEntity == null;
		int entityId = entity.GetEntityId();
		GameState gameState = GameState.Get();
		Network.Options.Option.SubOption selectedNetworkSubOption = gameState.GetSelectedNetworkSubOption();
		Entity entity2 = (!flag) ? simulatedSourceEntity : gameState.GetEntity(selectedNetworkSubOption.ID);
		if (flag && !selectedNetworkSubOption.Targets.Contains(entityId))
		{
			Entity entity3 = gameState.GetEntity(entityId);
			PlayErrors.DisplayPlayError(PlayErrors.GetTargetEntityError(entity2, entity3), entity2);
			return false;
		}
		if (TargetReticleManager.Get())
		{
			TargetReticleManager.Get().DestroyFriendlyTargetArrow(false);
		}
		this.FinishBattlecrySourceCard();
		this.FinishSubOptions();
		if (entity2.IsHeroPower())
		{
			entity2.SetTagAndHandleChange<int>(GAME_TAG.EXHAUSTED, 1);
			this.PredictSpentMana(entity2);
		}
		gameState.SetSelectedOptionTarget(entityId);
		gameState.SendOption();
		return true;
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x00091520 File Offset: 0x0008F720
	private void EnterOptionTargetMode()
	{
		GameState gameState = GameState.Get();
		gameState.EnterOptionTargetMode();
		if (this.m_useHandEnlarge)
		{
			this.m_myHandZone.SetFriendlyHeroTargetingMode(gameState.FriendlyHeroIsTargetable());
			this.m_myHandZone.UpdateLayout(-1, true);
		}
	}

	// Token: 0x06001ECD RID: 7885 RVA: 0x00091562 File Offset: 0x0008F762
	private void FinishBattlecrySourceCard()
	{
		if (this.m_battlecrySourceCard == null)
		{
			return;
		}
		this.ClearBattlecrySourceCard();
	}

	// Token: 0x06001ECE RID: 7886 RVA: 0x0009157C File Offset: 0x0008F77C
	private void ClearBattlecrySourceCard()
	{
		if (this.m_isInBattleCryEffect && this.m_battlecrySourceCard != null)
		{
			this.EndBattleCryEffect();
		}
		this.m_battlecrySourceCard = null;
		RemoteActionHandler.Get().NotifyOpponentOfCardDropped();
		if (this.m_useHandEnlarge)
		{
			this.m_myHandZone.SetFriendlyHeroTargetingMode(false);
			this.m_myHandZone.UpdateLayout(-1, true);
		}
	}

	// Token: 0x06001ECF RID: 7887 RVA: 0x000915E0 File Offset: 0x0008F7E0
	private void CancelSubOptions()
	{
		Card subOptionParentCard = ChoiceCardMgr.Get().GetSubOptionParentCard();
		if (subOptionParentCard == null)
		{
			return;
		}
		ChoiceCardMgr.Get().CancelSubOptions();
		Entity entity = subOptionParentCard.GetEntity();
		if (!entity.IsHeroPower())
		{
			ZoneMgr.Get().CancelLocalZoneChange(this.m_lastZoneChangeList, null, null);
			this.m_lastZoneChangeList = null;
		}
		this.RollbackSpentMana(entity);
		this.DropSubOptionParentCard();
	}

	// Token: 0x06001ED0 RID: 7888 RVA: 0x00091648 File Offset: 0x0008F848
	private void FinishSubOptions()
	{
		Card subOptionParentCard = ChoiceCardMgr.Get().GetSubOptionParentCard();
		if (subOptionParentCard == null)
		{
			return;
		}
		this.DropSubOptionParentCard();
	}

	// Token: 0x06001ED1 RID: 7889 RVA: 0x00091674 File Offset: 0x0008F874
	public void DropSubOptionParentCard()
	{
		Log.Hand.Print("DropSubOptionParentCard()", new object[0]);
		ChoiceCardMgr.Get().ClearSubOptions();
		RemoteActionHandler.Get().NotifyOpponentOfCardDropped();
		if (this.m_useHandEnlarge)
		{
			this.m_myHandZone.SetFriendlyHeroTargetingMode(false);
			this.m_myHandZone.UpdateLayout(-1, true);
		}
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.EndMobileTargetingEffect();
		}
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x000916E4 File Offset: 0x0008F8E4
	private void StartMobileTargetingEffect(List<int> targets)
	{
		if (targets == null || targets.Count == 0)
		{
			return;
		}
		this.m_mobileTargettingEffectActors.Clear();
		foreach (int id in targets)
		{
			Entity entity = GameState.Get().GetEntity(id);
			if (entity.GetCard() != null)
			{
				Actor actor = entity.GetCard().GetActor();
				this.m_mobileTargettingEffectActors.Add(actor);
				this.ApplyMobileTargettingEffectToActor(actor);
			}
		}
		FullScreenFXMgr.Get().Desaturate(0.9f, 0.4f, iTween.EaseType.easeInOutQuad, null);
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x000917A4 File Offset: 0x0008F9A4
	private void EndMobileTargetingEffect()
	{
		foreach (Actor actor in this.m_mobileTargettingEffectActors)
		{
			this.RemoveMobileTargettingEffectFromActor(actor);
		}
		FullScreenFXMgr.Get().StopDesaturate(0.4f, iTween.EaseType.easeInOutQuad, null);
	}

	// Token: 0x06001ED4 RID: 7892 RVA: 0x00091810 File Offset: 0x0008FA10
	private void StartBattleCryEffect(Entity entity)
	{
		this.m_isInBattleCryEffect = true;
		Network.Options.Option selectedNetworkOption = GameState.Get().GetSelectedNetworkOption();
		if (selectedNetworkOption == null)
		{
			Debug.LogError("No targets for BattleCry.");
			return;
		}
		this.StartMobileTargetingEffect(selectedNetworkOption.Main.Targets);
		this.m_battlecrySourceCard.SetBattleCrySource(true);
	}

	// Token: 0x06001ED5 RID: 7893 RVA: 0x0009185D File Offset: 0x0008FA5D
	private void EndBattleCryEffect()
	{
		this.m_isInBattleCryEffect = false;
		this.EndMobileTargetingEffect();
		this.m_battlecrySourceCard.SetBattleCrySource(false);
	}

	// Token: 0x06001ED6 RID: 7894 RVA: 0x00091878 File Offset: 0x0008FA78
	private void ApplyMobileTargettingEffectToActor(Actor actor)
	{
		if (actor == null || actor.gameObject == null)
		{
			return;
		}
		SceneUtils.SetLayer(actor.gameObject, GameLayer.IgnoreFullScreenEffects);
		Hashtable args = iTween.Hash(new object[]
		{
			"y",
			0.8f,
			"time",
			0.4f,
			"easeType",
			iTween.EaseType.easeOutQuad,
			"name",
			"position",
			"isLocal",
			true
		});
		Hashtable args2 = iTween.Hash(new object[]
		{
			"x",
			1.08f,
			"z",
			1.08f,
			"time",
			0.4f,
			"easeType",
			iTween.EaseType.easeOutQuad,
			"name",
			"scale"
		});
		iTween.StopByName(actor.gameObject, "position");
		iTween.StopByName(actor.gameObject, "scale");
		iTween.MoveTo(actor.gameObject, args);
		iTween.ScaleTo(actor.gameObject, args2);
	}

	// Token: 0x06001ED7 RID: 7895 RVA: 0x000919C0 File Offset: 0x0008FBC0
	private void RemoveMobileTargettingEffectFromActor(Actor actor)
	{
		if (actor == null || actor.gameObject == null)
		{
			return;
		}
		SceneUtils.SetLayer(actor.gameObject, GameLayer.Default);
		SceneUtils.SetLayer(actor.GetMeshRenderer().gameObject, GameLayer.CardRaycast);
		Hashtable args = iTween.Hash(new object[]
		{
			"x",
			0f,
			"y",
			0f,
			"z",
			0f,
			"time",
			0.5f,
			"easeType",
			iTween.EaseType.easeOutQuad,
			"name",
			"position",
			"isLocal",
			true
		});
		Hashtable args2 = iTween.Hash(new object[]
		{
			"x",
			1f,
			"z",
			1f,
			"time",
			0.4f,
			"easeType",
			iTween.EaseType.easeOutQuad,
			"name",
			"scale"
		});
		iTween.StopByName(actor.gameObject, "position");
		iTween.StopByName(actor.gameObject, "scale");
		iTween.MoveTo(actor.gameObject, args);
		iTween.ScaleTo(actor.gameObject, args2);
	}

	// Token: 0x06001ED8 RID: 7896 RVA: 0x00091B48 File Offset: 0x0008FD48
	private bool HandleMulliganHotkeys()
	{
		if (MulliganManager.Get() == null)
		{
			return false;
		}
		if (ApplicationMgr.IsInternal() && Input.GetKeyUp(27) && !GameMgr.Get().IsTutorial() && PlatformSettings.OS != OSCategory.iOS && PlatformSettings.OS != OSCategory.Android)
		{
			MulliganManager.Get().SetAllMulliganCardsToHold();
			this.DoEndTurnButton();
			TurnStartManager.Get().BeginListeningForTurnEvents();
			MulliganManager.Get().SkipMulliganForDev();
			return true;
		}
		return false;
	}

	// Token: 0x06001ED9 RID: 7897 RVA: 0x00091BC9 File Offset: 0x0008FDC9
	private bool HandleUniversalHotkeys()
	{
		return false;
	}

	// Token: 0x06001EDA RID: 7898 RVA: 0x00091BCC File Offset: 0x0008FDCC
	private bool HandleGameHotkeys()
	{
		return (GameState.Get() == null || !GameState.Get().IsMulliganManagerActive()) && Input.GetKeyUp(27) && this.CancelOption();
	}

	// Token: 0x06001EDB RID: 7899 RVA: 0x00091C08 File Offset: 0x0008FE08
	private void ShowBullseyeIfNeeded()
	{
		if (TargetReticleManager.Get() == null)
		{
			return;
		}
		if (!TargetReticleManager.Get().IsActive())
		{
			return;
		}
		bool show = this.m_mousedOverCard != null && GameState.Get().IsValidOptionTarget(this.m_mousedOverCard.GetEntity());
		TargetReticleManager.Get().ShowBullseye(show);
	}

	// Token: 0x06001EDC RID: 7900 RVA: 0x00091C6C File Offset: 0x0008FE6C
	private void ShowSkullIfNeeded()
	{
		if (this.GetBattlecrySourceCard() != null)
		{
			return;
		}
		Network.Options.Option.SubOption selectedNetworkSubOption = GameState.Get().GetSelectedNetworkSubOption();
		if (selectedNetworkSubOption == null)
		{
			return;
		}
		Entity entity = GameState.Get().GetEntity(selectedNetworkSubOption.ID);
		if (entity == null)
		{
			return;
		}
		if (!entity.IsMinion() && !entity.IsHero())
		{
			return;
		}
		Entity entity2 = this.m_mousedOverCard.GetEntity();
		if (!entity2.IsMinion() && !entity2.IsHero())
		{
			return;
		}
		if (!GameState.Get().IsValidOptionTarget(entity2))
		{
			return;
		}
		if (entity2.IsObfuscated())
		{
			return;
		}
		if ((entity2.CanBeDamaged() && entity.GetRealTimeAttack() >= entity2.GetRealTimeRemainingHP()) || (entity.IsPoisonous() && entity2.IsMinion()))
		{
			Spell spell = this.m_mousedOverCard.ActivateActorSpell(SpellType.SKULL);
			if (spell != null)
			{
				spell.transform.localScale = Vector3.zero;
				iTween.ScaleTo(spell.gameObject, iTween.Hash(new object[]
				{
					"scale",
					Vector3.one,
					"time",
					0.5f,
					"easetype",
					iTween.EaseType.easeOutElastic
				}));
			}
		}
		if ((entity.CanBeDamaged() && entity2.GetRealTimeAttack() >= entity.GetRealTimeRemainingHP()) || (entity2.IsPoisonous() && entity.IsMinion()))
		{
			Spell spell2 = entity.GetCard().ActivateActorSpell(SpellType.SKULL);
			if (spell2 != null)
			{
				spell2.transform.localScale = Vector3.zero;
				iTween.ScaleTo(spell2.gameObject, iTween.Hash(new object[]
				{
					"scale",
					Vector3.one,
					"time",
					0.5f,
					"easetype",
					iTween.EaseType.easeOutElastic
				}));
			}
		}
	}

	// Token: 0x06001EDD RID: 7901 RVA: 0x00091E6C File Offset: 0x0009006C
	private void DisableSkullIfNeeded(Card mousedOverCard)
	{
		Spell actorSpell = mousedOverCard.GetActorSpell(SpellType.SKULL, true);
		if (actorSpell != null)
		{
			iTween.Stop(actorSpell.gameObject);
			actorSpell.transform.localScale = Vector3.zero;
			actorSpell.Deactivate();
		}
		Network.Options.Option.SubOption selectedNetworkSubOption = GameState.Get().GetSelectedNetworkSubOption();
		if (selectedNetworkSubOption == null)
		{
			return;
		}
		Entity entity = GameState.Get().GetEntity(selectedNetworkSubOption.ID);
		if (entity == null)
		{
			return;
		}
		Card card = entity.GetCard();
		if (card == null)
		{
			return;
		}
		actorSpell = card.GetActorSpell(SpellType.SKULL, true);
		if (actorSpell != null)
		{
			iTween.Stop(actorSpell.gameObject);
			actorSpell.transform.localScale = Vector3.zero;
			actorSpell.Deactivate();
		}
	}

	// Token: 0x06001EDE RID: 7902 RVA: 0x00091F28 File Offset: 0x00090128
	private void HandleMouseOverCard(Card card)
	{
		if (!card.IsInputEnabled())
		{
			return;
		}
		GameState gameState = GameState.Get();
		this.m_mousedOverCard = card;
		bool flag = gameState.IsFriendlySidePlayerTurn();
		bool flag2 = flag && TargetReticleManager.Get() && TargetReticleManager.Get().IsActive();
		if (GameMgr.Get() != null && GameMgr.Get().IsSpectator())
		{
			flag2 = false;
		}
		if (gameState.IsMainPhase() && this.m_heldCard == null && !ChoiceCardMgr.Get().HasSubOption() && !flag2 && (!UniversalInputManager.Get().IsTouchMode() || card.gameObject == this.m_lastObjectMousedDown))
		{
			this.SetShouldShowTooltip();
		}
		card.NotifyMousedOver();
		if (gameState.IsMulliganManagerActive() && card.GetEntity().IsControlledByFriendlySidePlayer() && !UniversalInputManager.UsePhoneUI)
		{
			KeywordHelpPanelManager.Get().UpdateKeywordHelpForMulliganCard(card.GetEntity(), card.GetActor());
		}
		this.ShowBullseyeIfNeeded();
		this.ShowSkullIfNeeded();
	}

	// Token: 0x06001EDF RID: 7903 RVA: 0x00092044 File Offset: 0x00090244
	private void HandleMouseOffCard()
	{
		PegCursor.Get().SetMode(PegCursor.Mode.UP);
		Card mousedOverCard = this.m_mousedOverCard;
		this.m_mousedOverCard = null;
		mousedOverCard.HideTooltip();
		mousedOverCard.NotifyMousedOut();
		this.ShowBullseyeIfNeeded();
		this.DisableSkullIfNeeded(mousedOverCard);
	}

	// Token: 0x06001EE0 RID: 7904 RVA: 0x00092084 File Offset: 0x00090284
	public void HandleMemberClick()
	{
		if (this.m_mousedOverObject == null)
		{
			RaycastHit raycastHit;
			if (UniversalInputManager.Get().GetInputHitInfo(Camera.main, GameLayer.PlayAreaCollision, out raycastHit))
			{
				if (GameState.Get() == null)
				{
					return;
				}
				if (GameState.Get().IsMulliganManagerActive())
				{
					return;
				}
				RaycastHit raycastHit2;
				if (UniversalInputManager.Get().GetInputHitInfo(GameLayer.CardRaycast, out raycastHit2))
				{
					return;
				}
				GameObject mouseClickDustEffectPrefab = Board.Get().GetMouseClickDustEffectPrefab();
				if (mouseClickDustEffectPrefab == null)
				{
					return;
				}
				GameObject gameObject = Object.Instantiate<GameObject>(mouseClickDustEffectPrefab);
				gameObject.transform.position = raycastHit.point;
				ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
				if (componentsInChildren == null)
				{
					return;
				}
				Vector3 vector;
				vector..ctor(Input.GetAxis("Mouse Y") * 40f, Input.GetAxis("Mouse X") * 40f, 0f);
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					if (particleSystem.name == "Rocks")
					{
						particleSystem.transform.localRotation = Quaternion.Euler(vector);
					}
					particleSystem.Play();
				}
				switch (Random.Range(1, 5))
				{
				case 1:
					SoundManager.Get().LoadAndPlay("board_common_dirt_poke_1", gameObject);
					break;
				case 2:
					SoundManager.Get().LoadAndPlay("board_common_dirt_poke_2", gameObject);
					break;
				case 3:
					SoundManager.Get().LoadAndPlay("board_common_dirt_poke_3", gameObject);
					break;
				case 4:
					SoundManager.Get().LoadAndPlay("board_common_dirt_poke_4", gameObject);
					break;
				case 5:
					SoundManager.Get().LoadAndPlay("board_common_dirt_poke_5", gameObject);
					break;
				}
			}
			else if (Gameplay.Get() != null)
			{
				SoundManager.Get().LoadAndPlay("UI_MouseClick_01");
			}
		}
	}

	// Token: 0x06001EE1 RID: 7905 RVA: 0x00092261 File Offset: 0x00090461
	public bool MouseIsMoving(float tolerance)
	{
		return Mathf.Abs(Input.GetAxis("Mouse X")) > tolerance || Mathf.Abs(Input.GetAxis("Mouse Y")) > tolerance;
	}

	// Token: 0x06001EE2 RID: 7906 RVA: 0x00092290 File Offset: 0x00090490
	public bool MouseIsMoving()
	{
		return this.MouseIsMoving(0f);
	}

	// Token: 0x06001EE3 RID: 7907 RVA: 0x000922A0 File Offset: 0x000904A0
	private void ShowTooltipIfNecessary()
	{
		if (this.m_mousedOverCard == null)
		{
			return;
		}
		if (!this.m_mousedOverCard.GetShouldShowTooltip())
		{
			return;
		}
		this.m_mousedOverTimer += Time.unscaledDeltaTime;
		if (!this.m_mousedOverCard.IsActorReady())
		{
			return;
		}
		if (GameState.Get().GetGameEntity().IsMouseOverDelayOverriden())
		{
			this.m_mousedOverCard.ShowTooltip();
			return;
		}
		Zone zone = this.m_mousedOverCard.GetZone();
		if (zone is ZoneHand)
		{
			this.m_mousedOverCard.ShowTooltip();
			return;
		}
		if (this.m_mousedOverTimer >= this.m_MouseOverDelay)
		{
			this.m_mousedOverCard.ShowTooltip();
		}
	}

	// Token: 0x06001EE4 RID: 7908 RVA: 0x00092354 File Offset: 0x00090554
	private void ShowTooltipZone(GameObject hitObject, TooltipZone tooltip)
	{
		this.HideBigViewCardBacks();
		GameState gameState = GameState.Get();
		if (gameState.IsMulliganManagerActive())
		{
			return;
		}
		GameEntity gameEntity = gameState.GetGameEntity();
		if (gameEntity == null)
		{
			return;
		}
		if (gameEntity.AreTooltipsDisabled())
		{
			return;
		}
		if (gameEntity.NotifyOfTooltipDisplay(tooltip))
		{
			return;
		}
		ManaCrystalMgr component = tooltip.targetObject.GetComponent<ManaCrystalMgr>();
		if (component != null)
		{
			if (ManaCrystalMgr.Get().ShouldShowOverloadTooltip())
			{
				this.ShowTooltipZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_MANA_OVERLOAD_HEADLINE"), GameStrings.Get("GAMEPLAY_TOOLTIP_MANA_OVERLOAD_DESCRIPTION"));
			}
			else
			{
				this.ShowTooltipZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_MANA_HEADLINE"), GameStrings.Get("GAMEPLAY_TOOLTIP_MANA_DESCRIPTION"));
			}
			return;
		}
		ZoneDeck component2 = tooltip.targetObject.GetComponent<ZoneDeck>();
		if (component2 != null)
		{
			if (component2.m_Side == Player.Side.FRIENDLY)
			{
				if (component2.IsFatigued())
				{
					this.ShowTooltipZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_FATIGUE_DECK_HEADLINE"), GameStrings.Get("GAMEPLAY_TOOLTIP_FATIGUE_DECK_DESCRIPTION"));
				}
				else
				{
					this.ShowTooltipZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_DECK_HEADLINE"), GameStrings.Format("GAMEPLAY_TOOLTIP_DECK_DESCRIPTION", new object[]
					{
						component2.GetCards().Count
					}));
				}
			}
			else if (component2.m_Side == Player.Side.OPPOSING)
			{
				if (component2.IsFatigued())
				{
					this.ShowTooltipZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_FATIGUE_ENEMYDECK_HEADLINE"), GameStrings.Get("GAMEPLAY_TOOLTIP_FATIGUE_ENEMYDECK_DESCRIPTION"));
				}
				else
				{
					this.ShowTooltipZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYDECK_HEADLINE"), GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYDECK_DESC", new object[]
					{
						component2.GetCards().Count
					}));
				}
			}
			return;
		}
		ZoneHand component3 = tooltip.targetObject.GetComponent<ZoneHand>();
		if (component3 != null && component3.m_Side == Player.Side.OPPOSING)
		{
			if (GameMgr.Get().IsTutorial())
			{
				this.ShowTooltipZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYHAND_HEADLINE"), GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYHAND_DESC_TUT"));
			}
			else
			{
				int cardCount = component3.GetCardCount();
				if (cardCount == 1)
				{
					this.ShowTooltipZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYHAND_HEADLINE"), GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYHAND_DESC_SINGLE", new object[]
					{
						cardCount
					}));
				}
				else
				{
					this.ShowTooltipZone(tooltip, GameStrings.Get("GAMEPLAY_TOOLTIP_ENEMYHAND_HEADLINE"), GameStrings.Format("GAMEPLAY_TOOLTIP_ENEMYHAND_DESC", new object[]
					{
						cardCount
					}));
				}
			}
			return;
		}
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000925BC File Offset: 0x000907BC
	private void ShowTooltipZone(TooltipZone tooltip, string headline, string description)
	{
		GameEntity gameEntity = GameState.Get().GetGameEntity();
		gameEntity.NotifyOfTooltipZoneMouseOver(tooltip);
		if (UniversalInputManager.Get().IsTouchMode())
		{
			tooltip.ShowGameplayTooltipLarge(headline, description);
		}
		else
		{
			tooltip.ShowGameplayTooltip(headline, description);
		}
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x000925FF File Offset: 0x000907FF
	private void HideBigViewCardBacks()
	{
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x00092604 File Offset: 0x00090804
	private void PredictSpentMana(Entity entity)
	{
		Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
		if (friendlySidePlayer.HasTag(GAME_TAG.SPELLS_COST_HEALTH) && entity.IsSpell())
		{
			return;
		}
		int num = entity.GetRealTimeCost() - friendlySidePlayer.GetRealTimeTempMana();
		if (friendlySidePlayer.GetRealTimeTempMana() > 0)
		{
			int num2 = Mathf.Clamp(entity.GetRealTimeCost(), 0, friendlySidePlayer.GetRealTimeTempMana());
			friendlySidePlayer.NotifyOfUsedTempMana(num2);
			ManaCrystalMgr.Get().DestroyTempManaCrystals(num2);
		}
		if (num > 0)
		{
			friendlySidePlayer.NotifyOfSpentMana(num);
			ManaCrystalMgr.Get().UpdateSpentMana(num);
		}
		friendlySidePlayer.UpdateManaCounter();
		this.m_entitiesThatPredictedMana.Add(entity);
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x000926A4 File Offset: 0x000908A4
	private void RollbackSpentMana(Entity entity)
	{
		int num = this.m_entitiesThatPredictedMana.IndexOf(entity);
		if (num < 0)
		{
			return;
		}
		this.m_entitiesThatPredictedMana.RemoveAt(num);
		Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
		int num2 = -entity.GetRealTimeCost() + friendlySidePlayer.GetRealTimeTempMana();
		if (friendlySidePlayer.GetRealTimeTempMana() > 0)
		{
			int num3 = Mathf.Clamp(entity.GetRealTimeCost(), 0, friendlySidePlayer.GetRealTimeTempMana());
			friendlySidePlayer.NotifyOfUsedTempMana(-num3);
			ManaCrystalMgr.Get().AddTempManaCrystals(num3);
		}
		if (num2 < 0)
		{
			friendlySidePlayer.NotifyOfSpentMana(num2);
			ManaCrystalMgr.Get().UpdateSpentMana(num2);
		}
		friendlySidePlayer.UpdateManaCounter();
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x0009273D File Offset: 0x0009093D
	public void OnManaCrystalMgrManaSpent()
	{
		if (this.m_mousedOverCard)
		{
			this.m_mousedOverCard.UpdateProposedManaUsage();
		}
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x0009275A File Offset: 0x0009095A
	private bool IsInZone(Entity entity, TAG_ZONE zoneTag)
	{
		return this.IsInZone(entity.GetCard(), zoneTag);
	}

	// Token: 0x06001EEB RID: 7915 RVA: 0x0009276C File Offset: 0x0009096C
	private bool IsInZone(Card card, TAG_ZONE zoneTag)
	{
		Zone zone = card.GetZone();
		return !(zone == null) && GameUtils.GetFinalZoneForEntity(card.GetEntity()) == zoneTag;
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x0009279C File Offset: 0x0009099C
	public bool RegisterPhoneHandShownListener(InputManager.PhoneHandShownCallback callback)
	{
		return this.RegisterPhoneHandShownListener(callback, null);
	}

	// Token: 0x06001EED RID: 7917 RVA: 0x000927A8 File Offset: 0x000909A8
	public bool RegisterPhoneHandShownListener(InputManager.PhoneHandShownCallback callback, object userData)
	{
		InputManager.PhoneHandShownListener phoneHandShownListener = new InputManager.PhoneHandShownListener();
		phoneHandShownListener.SetCallback(callback);
		phoneHandShownListener.SetUserData(userData);
		if (this.m_phoneHandShownListener.Contains(phoneHandShownListener))
		{
			return false;
		}
		this.m_phoneHandShownListener.Add(phoneHandShownListener);
		return true;
	}

	// Token: 0x06001EEE RID: 7918 RVA: 0x000927E9 File Offset: 0x000909E9
	public bool RemovePhoneHandShownListener(InputManager.PhoneHandShownCallback callback)
	{
		return this.RemovePhoneHandShownListener(callback, null);
	}

	// Token: 0x06001EEF RID: 7919 RVA: 0x000927F4 File Offset: 0x000909F4
	public bool RemovePhoneHandShownListener(InputManager.PhoneHandShownCallback callback, object userData)
	{
		InputManager.PhoneHandShownListener phoneHandShownListener = new InputManager.PhoneHandShownListener();
		phoneHandShownListener.SetCallback(callback);
		phoneHandShownListener.SetUserData(userData);
		return this.m_phoneHandShownListener.Remove(phoneHandShownListener);
	}

	// Token: 0x06001EF0 RID: 7920 RVA: 0x00092821 File Offset: 0x00090A21
	public bool RegisterPhoneHandHiddenListener(InputManager.PhoneHandHiddenCallback callback)
	{
		return this.RegisterPhoneHandHiddenListener(callback, null);
	}

	// Token: 0x06001EF1 RID: 7921 RVA: 0x0009282C File Offset: 0x00090A2C
	public bool RegisterPhoneHandHiddenListener(InputManager.PhoneHandHiddenCallback callback, object userData)
	{
		InputManager.PhoneHandHiddenListener phoneHandHiddenListener = new InputManager.PhoneHandHiddenListener();
		phoneHandHiddenListener.SetCallback(callback);
		phoneHandHiddenListener.SetUserData(userData);
		if (this.m_phoneHandHiddenListener.Contains(phoneHandHiddenListener))
		{
			return false;
		}
		this.m_phoneHandHiddenListener.Add(phoneHandHiddenListener);
		return true;
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x0009286D File Offset: 0x00090A6D
	public bool RemovePhoneHandHiddenListener(InputManager.PhoneHandHiddenCallback callback)
	{
		return this.RemovePhoneHandHiddenListener(callback, null);
	}

	// Token: 0x06001EF3 RID: 7923 RVA: 0x00092878 File Offset: 0x00090A78
	public bool RemovePhoneHandHiddenListener(InputManager.PhoneHandHiddenCallback callback, object userData)
	{
		InputManager.PhoneHandHiddenListener phoneHandHiddenListener = new InputManager.PhoneHandHiddenListener();
		phoneHandHiddenListener.SetCallback(callback);
		phoneHandHiddenListener.SetUserData(userData);
		return this.m_phoneHandHiddenListener.Remove(phoneHandHiddenListener);
	}

	// Token: 0x04001103 RID: 4355
	private const float MOBILE_TARGETTING_Y_OFFSET = 0.8f;

	// Token: 0x04001104 RID: 4356
	private const float MOBILE_TARGETTING_XY_SCALE = 1.08f;

	// Token: 0x04001105 RID: 4357
	public float m_MouseOverDelay = 0.4f;

	// Token: 0x04001106 RID: 4358
	public DragRotatorInfo m_DragRotatorInfo = new DragRotatorInfo
	{
		m_PitchInfo = new DragRotatorAxisInfo
		{
			m_ForceMultiplier = 25f,
			m_MinDegrees = -40f,
			m_MaxDegrees = 40f,
			m_RestSeconds = 2f
		},
		m_RollInfo = new DragRotatorAxisInfo
		{
			m_ForceMultiplier = 25f,
			m_MinDegrees = -45f,
			m_MaxDegrees = 45f,
			m_RestSeconds = 2f
		}
	};

	// Token: 0x04001107 RID: 4359
	private readonly PlatformDependentValue<float> MIN_GRAB_Y = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		Tablet = 80f,
		Phone = 80f
	};

	// Token: 0x04001108 RID: 4360
	private static InputManager s_instance;

	// Token: 0x04001109 RID: 4361
	private ZoneHand m_myHandZone;

	// Token: 0x0400110A RID: 4362
	private ZonePlay m_myPlayZone;

	// Token: 0x0400110B RID: 4363
	private ZoneWeapon m_myWeaponZone;

	// Token: 0x0400110C RID: 4364
	private Card m_heldCard;

	// Token: 0x0400110D RID: 4365
	private bool m_checkForInput;

	// Token: 0x0400110E RID: 4366
	private GameObject m_lastObjectMousedDown;

	// Token: 0x0400110F RID: 4367
	private GameObject m_lastObjectRightMousedDown;

	// Token: 0x04001110 RID: 4368
	private Vector3 m_lastMouseDownPosition;

	// Token: 0x04001111 RID: 4369
	private bool m_leftMouseButtonIsDown;

	// Token: 0x04001112 RID: 4370
	private bool m_dragging;

	// Token: 0x04001113 RID: 4371
	private Card m_mousedOverCard;

	// Token: 0x04001114 RID: 4372
	private HistoryCard m_mousedOverHistoryCard;

	// Token: 0x04001115 RID: 4373
	private GameObject m_mousedOverObject;

	// Token: 0x04001116 RID: 4374
	private float m_mousedOverTimer;

	// Token: 0x04001117 RID: 4375
	private ZoneChangeList m_lastZoneChangeList;

	// Token: 0x04001118 RID: 4376
	private Card m_battlecrySourceCard;

	// Token: 0x04001119 RID: 4377
	private List<Card> m_cancelingBattlecryCards = new List<Card>();

	// Token: 0x0400111A RID: 4378
	private bool m_cardWasInsideHandLastFrame;

	// Token: 0x0400111B RID: 4379
	private bool m_isInBattleCryEffect;

	// Token: 0x0400111C RID: 4380
	private uint m_spectatorNotifyCurrentToken;

	// Token: 0x0400111D RID: 4381
	private List<Entity> m_entitiesThatPredictedMana = new List<Entity>();

	// Token: 0x0400111E RID: 4382
	private List<Actor> m_mobileTargettingEffectActors = new List<Actor>();

	// Token: 0x0400111F RID: 4383
	private Card m_lastPreviewedCard;

	// Token: 0x04001120 RID: 4384
	private bool m_touchDraggingCard;

	// Token: 0x04001121 RID: 4385
	private bool m_useHandEnlarge;

	// Token: 0x04001122 RID: 4386
	private bool m_hideHandAfterPlayingCard;

	// Token: 0x04001123 RID: 4387
	private bool m_targettingHeroPower;

	// Token: 0x04001124 RID: 4388
	private bool m_touchedDownOnSmallHand;

	// Token: 0x04001125 RID: 4389
	private List<InputManager.PhoneHandShownListener> m_phoneHandShownListener = new List<InputManager.PhoneHandShownListener>();

	// Token: 0x04001126 RID: 4390
	private List<InputManager.PhoneHandHiddenListener> m_phoneHandHiddenListener = new List<InputManager.PhoneHandHiddenListener>();

	// Token: 0x0200084A RID: 2122
	private class PhoneHandShownListener : EventListener<InputManager.PhoneHandShownCallback>
	{
		// Token: 0x06005185 RID: 20869 RVA: 0x00185C9D File Offset: 0x00183E9D
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x0200084B RID: 2123
	// (Invoke) Token: 0x06005187 RID: 20871
	public delegate void PhoneHandShownCallback(object userData);

	// Token: 0x0200084C RID: 2124
	private class PhoneHandHiddenListener : EventListener<InputManager.PhoneHandHiddenCallback>
	{
		// Token: 0x0600518B RID: 20875 RVA: 0x00185CB8 File Offset: 0x00183EB8
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}

	// Token: 0x0200084D RID: 2125
	// (Invoke) Token: 0x0600518D RID: 20877
	public delegate void PhoneHandHiddenCallback(object userData);
}
