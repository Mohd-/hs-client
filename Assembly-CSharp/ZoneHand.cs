using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000619 RID: 1561
public class ZoneHand : Zone
{
	// Token: 0x170004B1 RID: 1201
	// (get) Token: 0x060043E3 RID: 17379 RVA: 0x00144E94 File Offset: 0x00143094
	public CardStandIn CurrentStandIn
	{
		get
		{
			if (this.lastMousedOver < 0 || this.lastMousedOver >= this.m_cards.Count)
			{
				return null;
			}
			return this.GetStandIn(this.m_cards[this.lastMousedOver]);
		}
	}

	// Token: 0x060043E4 RID: 17380 RVA: 0x00144ED4 File Offset: 0x001430D4
	private void Awake()
	{
		this.enemyHand = (this.m_Side == Player.Side.OPPOSING);
		this.m_startingPosition = base.gameObject.transform.localPosition;
		this.m_startingScale = base.gameObject.transform.localScale;
		this.UpdateCenterAndWidth();
	}

	// Token: 0x060043E5 RID: 17381 RVA: 0x00144F22 File Offset: 0x00143122
	public int GetLastMousedOverCard()
	{
		return this.lastMousedOver;
	}

	// Token: 0x060043E6 RID: 17382 RVA: 0x00144F2C File Offset: 0x0014312C
	public bool IsHandScrunched()
	{
		int num = this.m_cards.Count;
		if (this.m_handEnlarged && num > 3)
		{
			return true;
		}
		float defaultCardSpacing = this.GetDefaultCardSpacing();
		if (!this.enemyHand)
		{
			num -= TurnStartManager.Get().GetNumCardsToDraw();
		}
		float num2 = defaultCardSpacing * (float)num;
		return num2 > this.MaxHandWidth();
	}

	// Token: 0x060043E7 RID: 17383 RVA: 0x00144F8C File Offset: 0x0014318C
	public void SetDoNotUpdateLayout(bool enable)
	{
		this.m_doNotUpdateLayout = enable;
	}

	// Token: 0x060043E8 RID: 17384 RVA: 0x00144F95 File Offset: 0x00143195
	public bool IsDoNotUpdateLayout()
	{
		return this.m_doNotUpdateLayout;
	}

	// Token: 0x060043E9 RID: 17385 RVA: 0x00144FA0 File Offset: 0x001431A0
	public void OnSpellPowerEntityEnteredPlay()
	{
		foreach (Card card in this.m_cards)
		{
			if (this.CanPlaySpellPowerHint(card))
			{
				Spell actorSpell = card.GetActorSpell(SpellType.SPELL_POWER_HINT_BURST, true);
				if (actorSpell != null)
				{
					actorSpell.Reactivate();
				}
			}
		}
	}

	// Token: 0x060043EA RID: 17386 RVA: 0x00145020 File Offset: 0x00143220
	public void OnSpellPowerEntityMousedOver()
	{
		if (TargetReticleManager.Get().IsActive())
		{
			return;
		}
		foreach (Card card in this.m_cards)
		{
			if (this.CanPlaySpellPowerHint(card))
			{
				Spell actorSpell = card.GetActorSpell(SpellType.SPELL_POWER_HINT_BURST, true);
				if (actorSpell != null)
				{
					actorSpell.Reactivate();
				}
				Spell actorSpell2 = card.GetActorSpell(SpellType.SPELL_POWER_HINT_IDLE, true);
				if (actorSpell2 != null)
				{
					actorSpell2.ActivateState(SpellStateType.BIRTH);
				}
			}
		}
	}

	// Token: 0x060043EB RID: 17387 RVA: 0x001450D0 File Offset: 0x001432D0
	public void OnSpellPowerEntityMousedOut()
	{
		foreach (Card card in this.m_cards)
		{
			Spell actorSpell = card.GetActorSpell(SpellType.SPELL_POWER_HINT_IDLE, true);
			if (!(actorSpell == null))
			{
				if (actorSpell.IsActive())
				{
					actorSpell.ActivateState(SpellStateType.DEATH);
				}
			}
		}
	}

	// Token: 0x060043EC RID: 17388 RVA: 0x00145158 File Offset: 0x00143358
	public float GetDefaultCardSpacing()
	{
		if (UniversalInputManager.UsePhoneUI && this.m_handEnlarged)
		{
			return this.m_enlargedHandDefaultCardSpacing;
		}
		return 1.270804f;
	}

	// Token: 0x060043ED RID: 17389 RVA: 0x0014518C File Offset: 0x0014338C
	public override void UpdateLayout()
	{
		if (!GameState.Get().IsMulliganManagerActive() && !this.enemyHand)
		{
			this.BlowUpOldStandins();
			for (int i = 0; i < this.m_cards.Count; i++)
			{
				Card card = this.m_cards[i];
				this.DuplicateColliderAndStuffItIn(card);
			}
		}
		this.UpdateLayout(-1, true, -1);
	}

	// Token: 0x060043EE RID: 17390 RVA: 0x001451F4 File Offset: 0x001433F4
	public void ForceStandInUpdate()
	{
		this.BlowUpOldStandins();
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card = this.m_cards[i];
			this.DuplicateColliderAndStuffItIn(card);
		}
	}

	// Token: 0x060043EF RID: 17391 RVA: 0x00145237 File Offset: 0x00143437
	public void UpdateLayout(int slotMousedOver)
	{
		this.UpdateLayout(slotMousedOver, false, -1);
	}

	// Token: 0x060043F0 RID: 17392 RVA: 0x00145242 File Offset: 0x00143442
	public void UpdateLayout(int slotMousedOver, bool forced)
	{
		this.UpdateLayout(slotMousedOver, forced, -1);
	}

	// Token: 0x060043F1 RID: 17393 RVA: 0x00145250 File Offset: 0x00143450
	public void UpdateLayout(int slotMousedOver, bool forced, int overrideCardCount)
	{
		this.m_updatingLayout = true;
		if (base.IsBlockingLayout())
		{
			base.UpdateLayoutFinished();
			return;
		}
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card = this.m_cards[i];
			if (!card.IsDoNotSort())
			{
				if (card.GetTransitionStyle() != ZoneTransitionStyle.VERY_SLOW)
				{
					if (!this.IsCardNotInEnemyHandAnymore(card))
					{
						if (!card.HasBeenGrabbedByEnemyActionHandler())
						{
							Spell bestSummonSpell = card.GetBestSummonSpell();
							if (!(bestSummonSpell != null) || !bestSummonSpell.IsActive())
							{
								card.ShowCard();
							}
						}
					}
				}
			}
		}
		if (this.m_doNotUpdateLayout)
		{
			base.UpdateLayoutFinished();
			return;
		}
		if (this.m_cards.Count == 0)
		{
			base.UpdateLayoutFinished();
			return;
		}
		if (slotMousedOver >= this.m_cards.Count || slotMousedOver < -1)
		{
			base.UpdateLayoutFinished();
			return;
		}
		if (!forced && slotMousedOver == this.lastMousedOver)
		{
			this.m_updatingLayout = false;
			return;
		}
		this.m_cards.Sort(new Comparison<Card>(Zone.CardSortComparison));
		this.UpdateLayoutImpl(slotMousedOver, overrideCardCount);
	}

	// Token: 0x060043F2 RID: 17394 RVA: 0x0014538C File Offset: 0x0014358C
	public void HideCards()
	{
		foreach (Card card in this.m_cards)
		{
			card.GetActor().gameObject.SetActive(false);
		}
	}

	// Token: 0x060043F3 RID: 17395 RVA: 0x001453F0 File Offset: 0x001435F0
	public void ShowCards()
	{
		foreach (Card card in this.m_cards)
		{
			card.GetActor().gameObject.SetActive(true);
		}
	}

	// Token: 0x060043F4 RID: 17396 RVA: 0x00145454 File Offset: 0x00143654
	public float GetCardWidth(int nCards)
	{
		if (nCards < 0)
		{
			return 0f;
		}
		if (nCards > ZoneHand.MAX_CARDS)
		{
			nCards = ZoneHand.MAX_CARDS;
		}
		float num = (float)Screen.width / (float)Screen.height;
		float[] array = ZoneHand.CARD_PIXEL_WIDTHS;
		return array[nCards] * (float)Screen.width * this.BASELINE_ASPECT_RATIO / num;
	}

	// Token: 0x060043F5 RID: 17397 RVA: 0x001454B4 File Offset: 0x001436B4
	public bool TouchReceived()
	{
		RaycastHit raycastHit;
		if (!UniversalInputManager.Get().GetInputHitInfo(GameLayer.CardRaycast.LayerBit(), out raycastHit))
		{
			this.m_touchedSlot = -1;
		}
		CardStandIn cardStandIn = SceneUtils.FindComponentInParents<CardStandIn>(raycastHit.transform);
		if (cardStandIn != null)
		{
			this.m_touchedSlot = cardStandIn.slot - 1;
			return true;
		}
		this.m_touchedSlot = -1;
		return false;
	}

	// Token: 0x060043F6 RID: 17398 RVA: 0x00145518 File Offset: 0x00143718
	public void HandleInput()
	{
		Card card = null;
		if (RemoteActionHandler.Get() != null && RemoteActionHandler.Get().GetFriendlyHoverCard() != null)
		{
			Card friendlyHoverCard = RemoteActionHandler.Get().GetFriendlyHoverCard();
			if (friendlyHoverCard.GetController().IsFriendlySide() && friendlyHoverCard.GetZone() is ZoneHand)
			{
				card = friendlyHoverCard;
			}
		}
		int num = -1;
		if (card != null)
		{
			num = this.m_cards.IndexOf(card);
		}
		if (UniversalInputManager.Get().IsTouchMode())
		{
			if (!InputManager.Get().LeftMouseButtonDown || this.m_touchedSlot < 0)
			{
				this.m_touchedSlot = -1;
				if (num < 0)
				{
					this.UpdateLayout(-1);
				}
				else
				{
					this.UpdateLayout(num);
				}
				return;
			}
			float num2 = UniversalInputManager.Get().GetMousePosition().x - InputManager.Get().LastMouseDownPosition.x;
			float num3 = Mathf.Max(0f, UniversalInputManager.Get().GetMousePosition().y - InputManager.Get().LastMouseDownPosition.y);
			float cardWidth = this.GetCardWidth(this.m_cards.Count);
			float num4 = (float)(this.lastMousedOver - this.m_touchedSlot) * cardWidth;
			float num5 = 10f + num3 * this.m_TouchDragResistanceFactorY;
			if (num2 < num4)
			{
				num2 = Mathf.Min(num4, num2 + num5);
			}
			else
			{
				num2 = Mathf.Max(num4, num2 - num5);
			}
			int slotMousedOver = this.m_touchedSlot + (int)Mathf.Round(num2 / cardWidth);
			this.UpdateLayout(slotMousedOver);
			return;
		}
		else
		{
			CardStandIn cardStandIn = null;
			int num6 = -1;
			RaycastHit raycastHit;
			if (!UniversalInputManager.Get().InputHitAnyObject(Camera.main, GameLayer.InvisibleHitBox1) || !UniversalInputManager.Get().GetInputHitInfo(Camera.main, GameLayer.CardRaycast, out raycastHit))
			{
				if (num < 0)
				{
					this.UpdateLayout(-1);
					return;
				}
			}
			else
			{
				cardStandIn = SceneUtils.FindComponentInParents<CardStandIn>(raycastHit.transform);
			}
			if (cardStandIn == null)
			{
				if (num < 0)
				{
					this.UpdateLayout(-1);
					return;
				}
			}
			else
			{
				num6 = cardStandIn.slot - 1;
			}
			if (num6 == this.lastMousedOver)
			{
				return;
			}
			bool flag = num6 != -1;
			if (flag || num < 0)
			{
				this.UpdateLayout(num6);
			}
			else if (!flag && num >= 0)
			{
				this.UpdateLayout(num);
			}
			return;
		}
	}

	// Token: 0x060043F7 RID: 17399 RVA: 0x00145784 File Offset: 0x00143984
	public void ShowManaGems()
	{
		Vector3 position = this.m_manaGemPosition.transform.position;
		position.x += -0.5f * this.m_manaGemMgr.GetWidth();
		this.m_manaGemMgr.gameObject.transform.position = position;
		this.m_manaGemMgr.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
	}

	// Token: 0x060043F8 RID: 17400 RVA: 0x001457FC File Offset: 0x001439FC
	public void HideManaGems()
	{
		this.m_manaGemMgr.transform.position = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x060043F9 RID: 17401 RVA: 0x00145830 File Offset: 0x00143A30
	public void SetHandEnlarged(bool enlarged)
	{
		this.m_handEnlarged = enlarged;
		if (enlarged)
		{
			base.gameObject.transform.localPosition = this.m_enlargedHandPosition;
			base.gameObject.transform.localScale = this.m_enlargedHandScale;
			ManaCrystalMgr.Get().ShowPhoneManaTray();
		}
		else
		{
			base.gameObject.transform.localPosition = this.m_startingPosition;
			base.gameObject.transform.localScale = this.m_startingScale;
			ManaCrystalMgr.Get().HidePhoneManaTray();
		}
		this.UpdateCenterAndWidth();
		this.m_handMoving = true;
		this.UpdateLayout(-1, true);
		this.m_handMoving = false;
	}

	// Token: 0x060043FA RID: 17402 RVA: 0x001458D7 File Offset: 0x00143AD7
	public bool HandEnlarged()
	{
		return this.m_handEnlarged;
	}

	// Token: 0x060043FB RID: 17403 RVA: 0x001458E0 File Offset: 0x00143AE0
	public void SetFriendlyHeroTargetingMode(bool enable)
	{
		if (!enable && this.m_hiddenStandIn != null)
		{
			this.m_hiddenStandIn.gameObject.SetActive(true);
		}
		if (this.m_targetingMode == enable)
		{
			return;
		}
		this.m_targetingMode = enable;
		this.m_heroHitbox.SetActive(enable);
		if (!this.m_handEnlarged)
		{
			return;
		}
		if (enable)
		{
			this.m_hiddenStandIn = this.CurrentStandIn;
			if (this.m_hiddenStandIn != null)
			{
				this.m_hiddenStandIn.gameObject.SetActive(false);
			}
			Vector3 enlargedHandPosition = this.m_enlargedHandPosition;
			enlargedHandPosition.z -= this.m_handHidingDistance;
			base.gameObject.transform.localPosition = enlargedHandPosition;
		}
		else
		{
			base.gameObject.transform.localPosition = this.m_enlargedHandPosition;
		}
		this.UpdateCenterAndWidth();
	}

	// Token: 0x060043FC RID: 17404 RVA: 0x001459C4 File Offset: 0x00143BC4
	private void UpdateLayoutImpl(int slotMousedOver, int overrideCardCount)
	{
		int num = 0;
		if (this.lastMousedOver != slotMousedOver && this.lastMousedOver != -1 && this.lastMousedOver < this.m_cards.Count && this.m_cards[this.lastMousedOver] != null && this.CanAnimateCard(this.m_cards[this.lastMousedOver]))
		{
			Card card = this.m_cards[this.lastMousedOver];
			iTween.Stop(card.gameObject);
			if (!this.enemyHand)
			{
				Vector3 mouseOverCardPosition = this.GetMouseOverCardPosition(card);
				Vector3 cardPosition = this.GetCardPosition(card, overrideCardCount);
				card.transform.position = new Vector3(mouseOverCardPosition.x, this.centerOfHand.y, cardPosition.z + 0.5f);
				card.transform.localScale = this.GetCardScale(card);
				card.transform.localEulerAngles = this.GetCardRotation(card);
			}
			card.NotifyMousedOut();
			GameLayer layer = GameLayer.Default;
			if (this.m_Side == Player.Side.OPPOSING && SpectatorManager.Get().IsSpectatingOpposingSide())
			{
				layer = GameLayer.CardRaycast;
			}
			SceneUtils.SetLayer(card.gameObject, layer);
		}
		float num2 = 0f;
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card2 = this.m_cards[i];
			if (this.CanAnimateCard(card2))
			{
				num++;
				float num3 = (!this.m_flipHandCards) ? 354.5f : 534.5f;
				card2.transform.rotation = Quaternion.Euler(new Vector3(card2.transform.localEulerAngles.x, card2.transform.localEulerAngles.y, num3));
				float num4 = 0.5f;
				if (this.m_handMoving)
				{
					num4 = 0.25f;
				}
				if (this.enemyHand)
				{
					num4 = 1.5f;
				}
				float num5 = 0.25f;
				iTween.EaseType easeType = iTween.EaseType.easeOutExpo;
				float transitionDelay = card2.GetTransitionDelay();
				card2.SetTransitionDelay(0f);
				ZoneTransitionStyle transitionStyle = card2.GetTransitionStyle();
				card2.SetTransitionStyle(ZoneTransitionStyle.NORMAL);
				if (transitionStyle != ZoneTransitionStyle.NORMAL)
				{
					ZoneTransitionStyle zoneTransitionStyle = transitionStyle;
					if (zoneTransitionStyle != ZoneTransitionStyle.SLOW)
					{
						if (zoneTransitionStyle == ZoneTransitionStyle.VERY_SLOW)
						{
							easeType = iTween.EaseType.easeInOutCubic;
							num5 = 1f;
							num4 = 1f;
						}
					}
					else
					{
						easeType = iTween.EaseType.easeInExpo;
						num5 = num4;
					}
					card2.GetActor().TurnOnCollider();
				}
				Vector3 vector = this.GetCardPosition(card2, overrideCardCount);
				Vector3 cardRotation = this.GetCardRotation(card2, overrideCardCount);
				Vector3 cardScale = this.GetCardScale(card2);
				if (i == slotMousedOver)
				{
					easeType = iTween.EaseType.easeOutExpo;
					if (this.enemyHand)
					{
						num5 = 0.15f;
						float num6 = 0.3f;
						vector..ctor(vector.x, vector.y, vector.z - num6);
					}
					else
					{
						float num7 = 0.5f * (float)i;
						num7 -= 0.5f * (float)this.m_cards.Count / 2f;
						float num8 = this.m_SelectCardScale;
						float num9 = this.m_SelectCardScale;
						cardRotation..ctor(0f, 0f, 0f);
						cardScale..ctor(num8, cardScale.y, num9);
						card2.transform.localScale = cardScale;
						num4 = 4f;
						float num10 = 0.1f;
						vector = this.GetMouseOverCardPosition(card2);
						float x = vector.x;
						if (this.m_handEnlarged)
						{
							vector.x = Mathf.Max(vector.x, this.m_enlargedHandCardMinX);
							vector.x = Mathf.Min(vector.x, this.m_enlargedHandCardMaxX);
						}
						card2.transform.position = new Vector3((x == vector.x) ? card2.transform.position.x : vector.x, vector.y, vector.z - num10);
						card2.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
						iTween.Stop(card2.gameObject);
						easeType = iTween.EaseType.easeOutExpo;
						if (CardTypeBanner.Get())
						{
							CardTypeBanner.Get().Show(card2.GetActor());
						}
						InputManager.Get().SetMousedOverCard(card2);
						bool showOnRight = card2.GetActor().GetMeshRenderer().bounds.center.x < base.GetComponent<BoxCollider>().bounds.center.x;
						KeywordHelpPanelManager.Get().UpdateKeywordHelp(card2, card2.GetActor(), showOnRight, default(float?), default(Vector3?));
						SceneUtils.SetLayer(card2.gameObject, GameLayer.Tooltip);
					}
				}
				else if (this.GetStandIn(card2) != null)
				{
					CardStandIn standIn = this.GetStandIn(card2);
					iTween.Stop(standIn.gameObject);
					standIn.transform.position = vector;
					if (!card2.CardStandInIsInteractive())
					{
						standIn.DisableStandIn();
					}
					else
					{
						standIn.EnableStandIn();
					}
				}
				card2.EnableTransitioningZones(true);
				string tweenName = ZoneMgr.Get().GetTweenName<ZoneHand>();
				Hashtable args = iTween.Hash(new object[]
				{
					"scale",
					cardScale,
					"delay",
					transitionDelay,
					"time",
					num5,
					"easeType",
					easeType,
					"name",
					tweenName
				});
				iTween.ScaleTo(card2.gameObject, args);
				Hashtable args2 = iTween.Hash(new object[]
				{
					"rotation",
					cardRotation,
					"delay",
					transitionDelay,
					"time",
					num5,
					"easeType",
					easeType,
					"name",
					tweenName
				});
				iTween.RotateTo(card2.gameObject, args2);
				Hashtable args3 = iTween.Hash(new object[]
				{
					"position",
					vector,
					"delay",
					transitionDelay,
					"time",
					num4,
					"easeType",
					easeType,
					"name",
					tweenName
				});
				iTween.MoveTo(card2.gameObject, args3);
				num2 = Mathf.Max(new float[]
				{
					num2,
					transitionDelay + num4,
					transitionDelay + num5
				});
			}
		}
		this.lastMousedOver = slotMousedOver;
		if (num > 0)
		{
			base.StartFinishLayoutTimer(num2);
		}
		else
		{
			base.UpdateLayoutFinished();
		}
	}

	// Token: 0x060043FD RID: 17405 RVA: 0x001460BD File Offset: 0x001442BD
	private void DuplicateColliderAndStuffItIn(Card card)
	{
		AssetLoader.Get().LoadActor("Card_Collider_Standin", new AssetLoader.GameObjectCallback(this.CardColliderLoadedCallback), card, false);
	}

	// Token: 0x060043FE RID: 17406 RVA: 0x001460E0 File Offset: 0x001442E0
	private void CardColliderLoadedCallback(string name, GameObject go, object callbackData)
	{
		Card card = (Card)callbackData;
		Actor actor = card.GetActor();
		if (actor != null)
		{
			actor.GetMeshRenderer().gameObject.layer = 0;
		}
		go.transform.localEulerAngles = this.GetCardRotation(card);
		go.transform.position = this.GetCardPosition(card);
		go.transform.localScale = this.GetCardScale(card);
		CardStandIn component = go.GetComponent<CardStandIn>();
		component.slot = card.GetZonePosition();
		component.linkedCard = card;
		this.standIns.Add(component);
		if (!component.linkedCard.CardStandInIsInteractive())
		{
			component.DisableStandIn();
		}
	}

	// Token: 0x060043FF RID: 17407 RVA: 0x0014618C File Offset: 0x0014438C
	private CardStandIn GetStandIn(Card card)
	{
		if (this.standIns == null)
		{
			return null;
		}
		foreach (CardStandIn cardStandIn in this.standIns)
		{
			if (!(cardStandIn == null))
			{
				if (cardStandIn.linkedCard == card)
				{
					return cardStandIn;
				}
			}
		}
		return null;
	}

	// Token: 0x06004400 RID: 17408 RVA: 0x00146218 File Offset: 0x00144418
	public void MakeStandInInteractive(Card card)
	{
		CardStandIn standIn = this.GetStandIn(card);
		if (standIn == null)
		{
			return;
		}
		this.GetStandIn(card).EnableStandIn();
	}

	// Token: 0x06004401 RID: 17409 RVA: 0x00146248 File Offset: 0x00144448
	private void BlowUpOldStandins()
	{
		if (this.standIns == null)
		{
			this.standIns = new List<CardStandIn>();
			return;
		}
		foreach (CardStandIn cardStandIn in this.standIns)
		{
			if (!(cardStandIn == null))
			{
				Object.Destroy(cardStandIn.gameObject);
			}
		}
		this.standIns = new List<CardStandIn>();
	}

	// Token: 0x06004402 RID: 17410 RVA: 0x001462DC File Offset: 0x001444DC
	public Vector3 GetCardPosition(Card card)
	{
		return this.GetCardPosition(card, -1);
	}

	// Token: 0x06004403 RID: 17411 RVA: 0x001462E8 File Offset: 0x001444E8
	public Vector3 GetCardPosition(Card card, int overrideCardCount)
	{
		int num = card.GetZonePosition() - 1;
		if (card.IsDoNotSort())
		{
			num = base.GetCards().Count - 1;
		}
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		int num5 = this.m_cards.Count;
		if (overrideCardCount >= 0 && overrideCardCount < this.m_cards.Count)
		{
			num5 = overrideCardCount;
		}
		if (!this.enemyHand)
		{
			num5 -= TurnStartManager.Get().GetNumCardsToDraw();
		}
		if (this.IsHandScrunched())
		{
			num4 = 1f;
			float num6 = 40f;
			if (!this.enemyHand)
			{
				num6 += (float)(num5 * 2);
			}
			num2 = num6 / (float)num5;
			num3 = -num6 / 2f;
		}
		float num7 = 0f;
		if (this.enemyHand)
		{
			num7 = 0f - num2 * ((float)num + 0.5f) - num3;
		}
		else
		{
			num7 += num2 * (float)num + num3;
		}
		float num8 = 0f;
		if ((this.enemyHand && num7 < 0f) || (!this.enemyHand && num7 > 0f))
		{
			num8 = Mathf.Sin(Mathf.Abs(num7) * 3.1415927f / 180f) * this.GetCardSpacing() / 2f;
		}
		float num9 = this.centerOfHand.x - this.GetCardSpacing() / 2f * (float)(num5 - 1 - num * 2);
		if (this.m_handEnlarged && this.m_targetingMode)
		{
			bool flag = num5 % 2 > 0;
			if (flag)
			{
				if (num < (num5 + 1) / 2)
				{
					num9 -= this.m_heroWidthInHand;
				}
			}
			else if (num < num5 / 2)
			{
				num9 -= this.m_heroWidthInHand / 2f;
			}
			else
			{
				num9 += this.m_heroWidthInHand / 2f;
			}
		}
		float y = this.centerOfHand.y;
		float num10 = this.centerOfHand.z;
		if (num5 > 1)
		{
			if (this.enemyHand)
			{
				num10 += Mathf.Pow((float)Mathf.Abs(num - num5 / 2), 2f) / (float)(4 * num5) * num4 + num8;
			}
			else
			{
				num10 = this.centerOfHand.z - Mathf.Pow((float)Mathf.Abs(num - num5 / 2), 2f) / (float)(4 * num5) * num4 - num8;
			}
		}
		if (this.enemyHand && SpectatorManager.Get().IsSpectatingOpposingSide())
		{
			num10 -= 0.2f;
		}
		Vector3 result;
		result..ctor(num9, y, num10);
		return result;
	}

	// Token: 0x06004404 RID: 17412 RVA: 0x0014658C File Offset: 0x0014478C
	public Vector3 GetCardRotation(Card card)
	{
		return this.GetCardRotation(card, -1);
	}

	// Token: 0x06004405 RID: 17413 RVA: 0x00146598 File Offset: 0x00144798
	public Vector3 GetCardRotation(Card card, int overrideCardCount)
	{
		int num = card.GetZonePosition() - 1;
		if (card.IsDoNotSort())
		{
			num = base.GetCards().Count - 1;
		}
		float num2 = 0f;
		float num3 = 0f;
		int num4 = this.m_cards.Count;
		if (overrideCardCount >= 0 && overrideCardCount < this.m_cards.Count)
		{
			num4 = overrideCardCount;
		}
		if (!this.enemyHand)
		{
			num4 -= TurnStartManager.Get().GetNumCardsToDraw();
		}
		if (this.IsHandScrunched())
		{
			float num5 = 40f;
			if (!this.enemyHand)
			{
				num5 += (float)(num4 * 2);
			}
			num2 = num5 / (float)num4;
			num3 = -num5 / 2f;
		}
		float num6 = 0f;
		if (this.enemyHand)
		{
			num6 = 0f - num2 * ((float)num + 0.5f) - num3;
		}
		else
		{
			num6 += num2 * (float)num + num3;
		}
		if (this.enemyHand && SpectatorManager.Get().IsSpectatingOpposingSide())
		{
			num6 += 180f;
		}
		float num7 = (!this.m_flipHandCards) ? 354.5f : 534.5f;
		return new Vector3(0f, num6, num7);
	}

	// Token: 0x06004406 RID: 17414 RVA: 0x001466CC File Offset: 0x001448CC
	public Vector3 GetCardScale(Card card)
	{
		if (this.enemyHand)
		{
			return new Vector3(0.682f, 0.225f, 0.682f);
		}
		if (UniversalInputManager.UsePhoneUI && this.m_handEnlarged)
		{
			return this.m_enlargedHandCardScale;
		}
		return new Vector3(0.62f, 0.225f, 0.62f);
	}

	// Token: 0x06004407 RID: 17415 RVA: 0x00146730 File Offset: 0x00144930
	private Vector3 GetMouseOverCardPosition(Card card)
	{
		return new Vector3(this.GetCardPosition(card).x, this.centerOfHand.y + 1f, base.transform.FindChild("MouseOverCardHeight").position.z + this.m_SelectCardOffsetZ);
	}

	// Token: 0x06004408 RID: 17416 RVA: 0x0014678C File Offset: 0x0014498C
	private float GetCardSpacing()
	{
		float num = this.GetDefaultCardSpacing();
		int num2 = this.m_cards.Count;
		if (!this.enemyHand)
		{
			num2 -= TurnStartManager.Get().GetNumCardsToDraw();
		}
		float num3 = num * (float)num2;
		float num4 = this.MaxHandWidth();
		if (num3 > num4)
		{
			num = num4 / (float)num2;
		}
		return num;
	}

	// Token: 0x06004409 RID: 17417 RVA: 0x001467E0 File Offset: 0x001449E0
	private float MaxHandWidth()
	{
		float num = this.m_maxWidth;
		if (this.m_handEnlarged && this.m_targetingMode)
		{
			num -= this.m_heroWidthInHand;
		}
		return num;
	}

	// Token: 0x0600440A RID: 17418 RVA: 0x00146814 File Offset: 0x00144A14
	protected bool CanAnimateCard(Card card)
	{
		bool flag = this.enemyHand && card.GetPrevZone() is ZonePlay;
		if (card.IsDoNotSort())
		{
			if (flag)
			{
				Log.FaceDownCard.Print("ZoneHand.CanAnimateCard() - card={0} FAILED card.IsDoNotSort()", new object[]
				{
					card
				});
			}
			return false;
		}
		if (!card.IsActorReady())
		{
			if (flag)
			{
				Log.FaceDownCard.Print("ZoneHand.CanAnimateCard() - card={0} FAILED !card.IsActorReady()", new object[]
				{
					card
				});
			}
			return false;
		}
		if (this.m_controller.IsFriendlySide() && TurnStartManager.Get() && TurnStartManager.Get().IsCardDrawHandled(card))
		{
			return false;
		}
		if (this.IsCardNotInEnemyHandAnymore(card))
		{
			if (flag)
			{
				Log.FaceDownCard.Print("ZoneHand.CanAnimateCard() - card={0} FAILED IsCardNotInEnemyHandAnymore()", new object[]
				{
					card
				});
			}
			return false;
		}
		if (card.HasBeenGrabbedByEnemyActionHandler())
		{
			if (flag)
			{
				Log.FaceDownCard.Print("ZoneHand.CanAnimateCard() - card={0} FAILED card.HasBeenGrabbedByEnemyActionHandler()", new object[]
				{
					card
				});
			}
			return false;
		}
		return true;
	}

	// Token: 0x0600440B RID: 17419 RVA: 0x00146921 File Offset: 0x00144B21
	private bool IsCardNotInEnemyHandAnymore(Card card)
	{
		return card.GetEntity().GetZone() != TAG_ZONE.HAND && this.enemyHand;
	}

	// Token: 0x0600440C RID: 17420 RVA: 0x00146940 File Offset: 0x00144B40
	private void UpdateCenterAndWidth()
	{
		this.centerOfHand = base.GetComponent<Collider>().bounds.center;
		this.m_maxWidth = base.GetComponent<Collider>().bounds.size.x;
	}

	// Token: 0x0600440D RID: 17421 RVA: 0x00146988 File Offset: 0x00144B88
	private bool CanPlaySpellPowerHint(Card card)
	{
		if (!card.IsShown())
		{
			return false;
		}
		if (!card.GetActor().IsShown())
		{
			return false;
		}
		Entity entity = card.GetEntity();
		return entity.IsAffectedBySpellPower() || TextUtils.HasBonusDamage(entity.GetStringTag(GAME_TAG.CARDTEXT_INHAND));
	}

	// Token: 0x0600440E RID: 17422 RVA: 0x001469D8 File Offset: 0x00144BD8
	public override bool AddCard(Card card)
	{
		return base.AddCard(card);
	}

	// Token: 0x04002B09 RID: 11017
	public const float MOUSE_OVER_SCALE = 1.5f;

	// Token: 0x04002B0A RID: 11018
	public const float HAND_SCALE = 0.62f;

	// Token: 0x04002B0B RID: 11019
	public const float HAND_SCALE_Y = 0.225f;

	// Token: 0x04002B0C RID: 11020
	public const float HAND_SCALE_OPPONENT = 0.682f;

	// Token: 0x04002B0D RID: 11021
	public const float HAND_SCALE_OPPONENT_Y = 0.225f;

	// Token: 0x04002B0E RID: 11022
	private const float CARD_WIDTH = 2.049684f;

	// Token: 0x04002B0F RID: 11023
	private const float ANGLE_OF_CARDS = 40f;

	// Token: 0x04002B10 RID: 11024
	private const float DEFAULT_ANIMATE_TIME = 0.35f;

	// Token: 0x04002B11 RID: 11025
	private const float DRIFT_AMOUNT = 0.08f;

	// Token: 0x04002B12 RID: 11026
	private const float Z_ROTATION_ON_LEFT = 354.5f;

	// Token: 0x04002B13 RID: 11027
	private const float Z_ROTATION_ON_RIGHT = 3f;

	// Token: 0x04002B14 RID: 11028
	private const float RESISTANCE_BASE = 10f;

	// Token: 0x04002B15 RID: 11029
	public GameObject m_iPhoneCardPosition;

	// Token: 0x04002B16 RID: 11030
	public GameObject m_leftArrow;

	// Token: 0x04002B17 RID: 11031
	public GameObject m_rightArrow;

	// Token: 0x04002B18 RID: 11032
	public GameObject m_manaGemPosition;

	// Token: 0x04002B19 RID: 11033
	public ManaCrystalMgr m_manaGemMgr;

	// Token: 0x04002B1A RID: 11034
	public GameObject m_playCardButton;

	// Token: 0x04002B1B RID: 11035
	public GameObject m_iPhonePreviewBone;

	// Token: 0x04002B1C RID: 11036
	public Float_MobileOverride m_SelectCardOffsetZ;

	// Token: 0x04002B1D RID: 11037
	public Float_MobileOverride m_SelectCardScale;

	// Token: 0x04002B1E RID: 11038
	public Float_MobileOverride m_TouchDragResistanceFactorY;

	// Token: 0x04002B1F RID: 11039
	public Vector3 m_enlargedHandPosition;

	// Token: 0x04002B20 RID: 11040
	public Vector3 m_enlargedHandScale;

	// Token: 0x04002B21 RID: 11041
	public Vector3 m_enlargedHandCardScale;

	// Token: 0x04002B22 RID: 11042
	public float m_enlargedHandDefaultCardSpacing;

	// Token: 0x04002B23 RID: 11043
	public float m_enlargedHandCardMinX;

	// Token: 0x04002B24 RID: 11044
	public float m_enlargedHandCardMaxX;

	// Token: 0x04002B25 RID: 11045
	public float m_heroWidthInHand;

	// Token: 0x04002B26 RID: 11046
	public float m_handHidingDistance;

	// Token: 0x04002B27 RID: 11047
	public GameObject m_heroHitbox;

	// Token: 0x04002B28 RID: 11048
	private static int MAX_CARDS = 10;

	// Token: 0x04002B29 RID: 11049
	private static float[] CARD_PIXEL_WIDTHS_TABLET = new float[]
	{
		0f,
		0.08f,
		0.08f,
		0.08f,
		0.08f,
		0.074f,
		0.069f,
		0.064f,
		0.06f,
		0.056f,
		0.054f
	};

	// Token: 0x04002B2A RID: 11050
	private static float[] CARD_PIXEL_WIDTHS_PHONE = new float[]
	{
		0f,
		0.148f,
		0.148f,
		0.148f,
		0.148f,
		0.148f,
		0.148f,
		0.143f,
		0.125f,
		0.111f,
		0.1f
	};

	// Token: 0x04002B2B RID: 11051
	private static PlatformDependentValue<float[]> CARD_PIXEL_WIDTHS = new PlatformDependentValue<float[]>(PlatformCategory.Screen)
	{
		PC = ZoneHand.CARD_PIXEL_WIDTHS_TABLET,
		Tablet = ZoneHand.CARD_PIXEL_WIDTHS_TABLET,
		Phone = ZoneHand.CARD_PIXEL_WIDTHS_PHONE
	};

	// Token: 0x04002B2C RID: 11052
	private readonly PlatformDependentValue<float> BASELINE_ASPECT_RATIO = new PlatformDependentValue<float>(PlatformCategory.Screen)
	{
		PC = 1.3333334f,
		Tablet = 1.3333334f,
		Phone = 1.775f
	};

	// Token: 0x04002B2D RID: 11053
	private int lastMousedOver = -1;

	// Token: 0x04002B2E RID: 11054
	private float m_maxWidth;

	// Token: 0x04002B2F RID: 11055
	private bool m_doNotUpdateLayout = true;

	// Token: 0x04002B30 RID: 11056
	private Vector3 centerOfHand;

	// Token: 0x04002B31 RID: 11057
	private bool enemyHand;

	// Token: 0x04002B32 RID: 11058
	private bool m_handEnlarged;

	// Token: 0x04002B33 RID: 11059
	private Vector3 m_startingPosition;

	// Token: 0x04002B34 RID: 11060
	private Vector3 m_startingScale;

	// Token: 0x04002B35 RID: 11061
	private bool m_handMoving;

	// Token: 0x04002B36 RID: 11062
	private bool m_targetingMode;

	// Token: 0x04002B37 RID: 11063
	private int m_touchedSlot;

	// Token: 0x04002B38 RID: 11064
	private CardStandIn m_hiddenStandIn;

	// Token: 0x04002B39 RID: 11065
	private List<CardStandIn> standIns;

	// Token: 0x04002B3A RID: 11066
	private bool m_flipHandCards;
}
