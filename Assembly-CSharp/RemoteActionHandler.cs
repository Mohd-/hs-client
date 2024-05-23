using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000845 RID: 2117
public class RemoteActionHandler : MonoBehaviour
{
	// Token: 0x06005123 RID: 20771 RVA: 0x00182928 File Offset: 0x00180B28
	private void Awake()
	{
		RemoteActionHandler.s_instance = this;
		GameState.Get().RegisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnChanged));
	}

	// Token: 0x06005124 RID: 20772 RVA: 0x00182947 File Offset: 0x00180B47
	private void OnDestroy()
	{
		RemoteActionHandler.s_instance = null;
	}

	// Token: 0x06005125 RID: 20773 RVA: 0x00182950 File Offset: 0x00180B50
	private void Update()
	{
		if (TargetReticleManager.Get() != null)
		{
			TargetReticleManager.Get().UpdateArrowPosition();
		}
		if (this.myCurrentUI.SameAs(this.myLastUI))
		{
			return;
		}
		if (!this.CanSendUI())
		{
			return;
		}
		Network.Get().SendUserUI(this.myCurrentUI.over.ID, this.myCurrentUI.held.ID, this.myCurrentUI.origin.ID, 0, 0);
		this.myLastUI.CopyFrom(this.myCurrentUI);
	}

	// Token: 0x06005126 RID: 20774 RVA: 0x001829E7 File Offset: 0x00180BE7
	public static RemoteActionHandler Get()
	{
		return RemoteActionHandler.s_instance;
	}

	// Token: 0x06005127 RID: 20775 RVA: 0x001829EE File Offset: 0x00180BEE
	public Card GetOpponentHeldCard()
	{
		return this.enemyActualUI.held.card;
	}

	// Token: 0x06005128 RID: 20776 RVA: 0x00182A00 File Offset: 0x00180C00
	public Card GetFriendlyHoverCard()
	{
		return this.friendlyActualUI.over.card;
	}

	// Token: 0x06005129 RID: 20777 RVA: 0x00182A12 File Offset: 0x00180C12
	public Card GetFriendlyHeldCard()
	{
		return this.friendlyActualUI.held.card;
	}

	// Token: 0x0600512A RID: 20778 RVA: 0x00182A24 File Offset: 0x00180C24
	public void NotifyOpponentOfMouseOverEntity(Card card)
	{
		this.myCurrentUI.over.card = card;
	}

	// Token: 0x0600512B RID: 20779 RVA: 0x00182A37 File Offset: 0x00180C37
	public void NotifyOpponentOfMouseOut()
	{
		this.myCurrentUI.over.card = null;
	}

	// Token: 0x0600512C RID: 20780 RVA: 0x00182A4A File Offset: 0x00180C4A
	public void NotifyOpponentOfTargetModeBegin(Card card)
	{
		this.myCurrentUI.origin.card = card;
	}

	// Token: 0x0600512D RID: 20781 RVA: 0x00182A5D File Offset: 0x00180C5D
	public void NotifyOpponentOfTargetEnd()
	{
		this.myCurrentUI.origin.card = null;
	}

	// Token: 0x0600512E RID: 20782 RVA: 0x00182A70 File Offset: 0x00180C70
	public void NotifyOpponentOfCardPickedUp(Card card)
	{
		this.myCurrentUI.held.card = card;
	}

	// Token: 0x0600512F RID: 20783 RVA: 0x00182A83 File Offset: 0x00180C83
	public void NotifyOpponentOfCardDropped()
	{
		this.myCurrentUI.held.card = null;
	}

	// Token: 0x06005130 RID: 20784 RVA: 0x00182A98 File Offset: 0x00180C98
	public void HandleAction(Network.UserUI newData)
	{
		bool flag = false;
		if (newData.playerId != null)
		{
			Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
			flag = (friendlySidePlayer != null && friendlySidePlayer.GetPlayerId() == newData.playerId.Value);
		}
		if (newData.mouseInfo != null)
		{
			if (flag)
			{
				this.friendlyWantedUI.held.ID = newData.mouseInfo.HeldCardID;
				this.friendlyWantedUI.over.ID = newData.mouseInfo.OverCardID;
				this.friendlyWantedUI.origin.ID = newData.mouseInfo.ArrowOriginID;
			}
			else
			{
				this.enemyWantedUI.held.ID = newData.mouseInfo.HeldCardID;
				this.enemyWantedUI.over.ID = newData.mouseInfo.OverCardID;
				this.enemyWantedUI.origin.ID = newData.mouseInfo.ArrowOriginID;
			}
			this.UpdateCardOver();
			this.UpdateCardHeld();
			this.MaybeDestroyArrow();
			this.MaybeCreateArrow();
			this.UpdateTargetArrow();
		}
		else if (newData.emoteInfo != null)
		{
			EmoteType emote = (EmoteType)newData.emoteInfo.Emote;
			if (flag)
			{
				GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(emote);
			}
			else if (this.CanReceiveEnemyEmote(emote))
			{
				GameState.Get().GetOpposingSidePlayer().GetHeroCard().PlayEmote(emote);
			}
		}
	}

	// Token: 0x06005131 RID: 20785 RVA: 0x00182C14 File Offset: 0x00180E14
	private bool CanSendUI()
	{
		if (GameMgr.Get() == null)
		{
			return false;
		}
		if (GameMgr.Get().IsSpectator())
		{
			return false;
		}
		if (GameMgr.Get().IsAI() && !SpectatorManager.Get().MyGameHasSpectators() && SpectatorManager.Get().MyGameHasSpectators())
		{
			return false;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - this.m_lastSendTime;
		if (this.IsSendingTargetingArrow() && num > 0.25f)
		{
			this.m_lastSendTime = realtimeSinceStartup;
			return true;
		}
		if (num < 0.35f)
		{
			return false;
		}
		this.m_lastSendTime = realtimeSinceStartup;
		return true;
	}

	// Token: 0x06005132 RID: 20786 RVA: 0x00182CB0 File Offset: 0x00180EB0
	private bool IsSendingTargetingArrow()
	{
		return !(this.myCurrentUI.origin.card == null) && !(this.myCurrentUI.over.card == null) && !(this.myCurrentUI.over.card == this.myCurrentUI.origin.card) && (this.myCurrentUI.origin.card != this.myLastUI.origin.card || this.myCurrentUI.over.card != this.myLastUI.over.card);
	}

	// Token: 0x06005133 RID: 20787 RVA: 0x00182D7C File Offset: 0x00180F7C
	private int GetOpponentHandHoverSlot()
	{
		Entity entity = this.enemyActualUI.over.entity;
		if (entity == null)
		{
			return -1;
		}
		if (entity.GetZone() != TAG_ZONE.HAND)
		{
			return -1;
		}
		if (entity.GetController().IsFriendlySide())
		{
			return -1;
		}
		return entity.GetTag(GAME_TAG.ZONE_POSITION) - 1;
	}

	// Token: 0x06005134 RID: 20788 RVA: 0x00182DD0 File Offset: 0x00180FD0
	private void UpdateCardOver()
	{
		Card card = this.enemyActualUI.over.card;
		Card card2 = this.enemyWantedUI.over.card;
		if (card != card2)
		{
			this.enemyActualUI.over.card = card2;
			if (card != null)
			{
				card.NotifyOpponentMousedOffThisCard();
			}
			if (card2 != null)
			{
				card2.NotifyOpponentMousedOverThisCard();
			}
			ZoneMgr.Get().FindZoneOfType<ZoneHand>(Player.Side.OPPOSING).UpdateLayout(this.GetOpponentHandHoverSlot());
		}
		if (!GameMgr.Get().IsSpectator())
		{
			return;
		}
		Card card3 = this.friendlyActualUI.over.card;
		Card card4 = this.friendlyWantedUI.over.card;
		if (card3 != card4)
		{
			this.friendlyActualUI.over.card = card4;
			if (card3 != null)
			{
				ZoneHand zoneHand = card3.GetZone() as ZoneHand;
				if (zoneHand != null)
				{
					if (zoneHand.CurrentStandIn == null)
					{
						zoneHand.UpdateLayout(-1);
					}
				}
				else
				{
					card3.NotifyMousedOut();
				}
			}
			if (card4 != null)
			{
				ZoneHand zoneHand2 = card4.GetZone() as ZoneHand;
				if (zoneHand2 != null)
				{
					if (zoneHand2.CurrentStandIn == null)
					{
						int num = zoneHand2.FindCardPos(card4);
						if (num >= 1)
						{
							zoneHand2.UpdateLayout(num - 1);
						}
					}
				}
				else
				{
					card4.NotifyMousedOver();
				}
			}
		}
	}

	// Token: 0x06005135 RID: 20789 RVA: 0x00182F50 File Offset: 0x00181150
	private void UpdateCardHeld()
	{
		Card card = this.enemyActualUI.held.card;
		Card card2 = this.enemyWantedUI.held.card;
		if (card != card2)
		{
			this.enemyActualUI.held.card = card2;
			if (card != null)
			{
				card.MarkAsGrabbedByEnemyActionHandler(false);
			}
			if (this.IsCardInHand(card))
			{
				card.GetZone().UpdateLayout();
			}
			if (this.CanAnimateHeldCard(card2))
			{
				card2.MarkAsGrabbedByEnemyActionHandler(true);
				if (SpectatorManager.Get().IsSpectatingOpposingSide())
				{
					this.StandUpright(false);
				}
				Hashtable args = iTween.Hash(new object[]
				{
					"name",
					"RemoteActionHandler",
					"position",
					Board.Get().FindBone("OpponentCardPlayingSpot").position,
					"time",
					1f,
					"oncomplete",
					delegate(object o)
					{
						this.StartDrift(false);
					},
					"oncompletetarget",
					base.gameObject
				});
				iTween.MoveTo(card2.gameObject, args);
			}
		}
		if (!GameMgr.Get().IsSpectator())
		{
			return;
		}
		Card card3 = this.friendlyActualUI.held.card;
		Card card4 = this.friendlyWantedUI.held.card;
		if (card3 != card4)
		{
			this.friendlyActualUI.held.card = card4;
			if (card3 != null)
			{
				card3.MarkAsGrabbedByEnemyActionHandler(false);
			}
			if (this.IsCardInHand(card3))
			{
				card3.GetZone().UpdateLayout();
			}
			if (this.CanAnimateHeldCard(card4))
			{
				card4.MarkAsGrabbedByEnemyActionHandler(true);
				Hashtable args2;
				if (card4 == this.GetFriendlyHoverCard())
				{
					ZoneHand zoneHand = card4.GetZone() as ZoneHand;
					if (zoneHand != null)
					{
						card4.NotifyMousedOut();
						Vector3 cardScale = zoneHand.GetCardScale(card4);
						args2 = iTween.Hash(new object[]
						{
							"scale",
							cardScale,
							"time",
							0.15f,
							"easeType",
							iTween.EaseType.easeOutExpo,
							"name",
							"RemoteActionHandler"
						});
						iTween.ScaleTo(card4.gameObject, args2);
					}
				}
				args2 = iTween.Hash(new object[]
				{
					"name",
					"RemoteActionHandler",
					"position",
					Board.Get().FindBone("FriendlyCardPlayingSpot").position,
					"time",
					1f,
					"oncomplete",
					delegate(object o)
					{
						this.StartDrift(true);
					},
					"oncompletetarget",
					base.gameObject
				});
				iTween.MoveTo(card4.gameObject, args2);
			}
		}
	}

	// Token: 0x06005136 RID: 20790 RVA: 0x0018323C File Offset: 0x0018143C
	private void StartDrift(bool isFriendlySide)
	{
		if (isFriendlySide || !SpectatorManager.Get().IsSpectatingOpposingSide())
		{
			this.StandUpright(isFriendlySide);
		}
		this.DriftLeftAndRight(isFriendlySide);
	}

	// Token: 0x06005137 RID: 20791 RVA: 0x00183264 File Offset: 0x00181464
	private void DriftLeftAndRight(bool isFriendlySide)
	{
		Card card = (!isFriendlySide) ? this.enemyActualUI.held.card : this.friendlyActualUI.held.card;
		if (!this.CanAnimateHeldCard(card))
		{
			return;
		}
		Vector3[] array;
		if (isFriendlySide)
		{
			iTweenPath iTweenPath;
			if (!iTweenPath.paths.TryGetValue(iTweenPath.FixupPathName("driftPath1_friendly"), out iTweenPath))
			{
				Transform transform = Board.Get().FindBone("OpponentCardPlayingSpot");
				Transform transform2 = Board.Get().FindBone("FriendlyCardPlayingSpot");
				Vector3 vector = transform2.position - transform.position;
				iTweenPath iTweenPath2 = iTweenPath.paths[iTweenPath.FixupPathName("driftPath1")];
				iTweenPath = transform2.gameObject.AddComponent<iTweenPath>();
				iTweenPath.pathVisible = true;
				iTweenPath.pathName = "driftPath1_friendly";
				iTweenPath.pathColor = iTweenPath2.pathColor;
				iTweenPath.nodes = new List<Vector3>(iTweenPath2.nodes);
				for (int i = 0; i < iTweenPath.nodes.Count; i++)
				{
					iTweenPath.nodes[i] = iTweenPath2.nodes[i] + vector;
				}
				iTweenPath.enabled = false;
				iTweenPath.enabled = true;
			}
			array = iTweenPath.nodes.ToArray();
		}
		else
		{
			array = iTweenPath.GetPath("driftPath1");
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"name",
			"RemoteActionHandler",
			"path",
			array,
			"time",
			10f,
			"easetype",
			iTween.EaseType.linear,
			"looptype",
			iTween.LoopType.pingPong
		});
		iTween.MoveTo(card.gameObject, args);
	}

	// Token: 0x06005138 RID: 20792 RVA: 0x00183430 File Offset: 0x00181630
	private void StandUpright(bool isFriendlySide)
	{
		Card card = (!isFriendlySide) ? this.enemyActualUI.held.card : this.friendlyActualUI.held.card;
		if (!this.CanAnimateHeldCard(card))
		{
			return;
		}
		float num = 5f;
		if (!isFriendlySide && SpectatorManager.Get().IsSpectatingOpposingSide())
		{
			num = 0.3f;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"name",
			"RemoteActionHandler",
			"rotation",
			Vector3.zero,
			"time",
			num,
			"easetype",
			iTween.EaseType.easeInOutSine
		});
		iTween.RotateTo(card.gameObject, args);
	}

	// Token: 0x06005139 RID: 20793 RVA: 0x001834F8 File Offset: 0x001816F8
	private void MaybeDestroyArrow()
	{
		if (TargetReticleManager.Get() == null || !TargetReticleManager.Get().IsActive())
		{
			return;
		}
		bool flag = GameState.Get() != null && GameState.Get().IsFriendlySidePlayerTurn();
		RemoteActionHandler.UserUI userUI = (!flag) ? this.enemyWantedUI : this.friendlyWantedUI;
		RemoteActionHandler.UserUI userUI2 = (!flag) ? this.enemyActualUI : this.friendlyActualUI;
		if (userUI.origin.card == userUI2.origin.card)
		{
			return;
		}
		if (userUI2.origin.entity.HasTag(GAME_TAG.IMMUNE_WHILE_ATTACKING) && !userUI2.origin.entity.IsImmune())
		{
			userUI2.origin.card.GetActor().DeactivateSpell(SpellType.IMMUNE);
		}
		userUI2.origin.card = null;
		if (flag)
		{
			TargetReticleManager.Get().DestroyFriendlyTargetArrow(false);
		}
		else
		{
			TargetReticleManager.Get().DestroyEnemyTargetArrow();
		}
	}

	// Token: 0x0600513A RID: 20794 RVA: 0x00183600 File Offset: 0x00181800
	private void MaybeCreateArrow()
	{
		if (TargetReticleManager.Get() == null || TargetReticleManager.Get().IsActive())
		{
			return;
		}
		bool flag = GameState.Get() != null && GameState.Get().IsFriendlySidePlayerTurn();
		RemoteActionHandler.UserUI userUI = (!flag) ? this.enemyWantedUI : this.friendlyWantedUI;
		RemoteActionHandler.UserUI userUI2 = (!flag) ? this.enemyActualUI : this.friendlyActualUI;
		if (userUI.origin.card == null)
		{
			return;
		}
		if (userUI2.over.card == null)
		{
			return;
		}
		if (userUI2.over.card.GetActor() == null)
		{
			return;
		}
		if (!userUI2.over.card.GetActor().IsShown())
		{
			return;
		}
		if (userUI2.over.card == userUI.origin.card)
		{
			return;
		}
		Player currentPlayer = GameState.Get().GetCurrentPlayer();
		if (currentPlayer == null || currentPlayer.IsLocalUser())
		{
			return;
		}
		userUI2.origin.card = userUI.origin.card;
		if (flag)
		{
			TargetReticleManager.Get().CreateFriendlyTargetArrow(userUI2.origin.entity, userUI2.origin.entity, false, true, null, false);
		}
		else
		{
			TargetReticleManager.Get().CreateEnemyTargetArrow(userUI2.origin.entity);
		}
		if (userUI2.origin.entity.HasTag(GAME_TAG.IMMUNE_WHILE_ATTACKING))
		{
			userUI2.origin.card.ActivateActorSpell(SpellType.IMMUNE);
		}
		this.SetArrowTarget();
	}

	// Token: 0x0600513B RID: 20795 RVA: 0x001837A8 File Offset: 0x001819A8
	private void UpdateTargetArrow()
	{
		if (TargetReticleManager.Get() == null || !TargetReticleManager.Get().IsActive())
		{
			return;
		}
		this.SetArrowTarget();
	}

	// Token: 0x0600513C RID: 20796 RVA: 0x001837DC File Offset: 0x001819DC
	private void SetArrowTarget()
	{
		bool flag = GameState.Get() != null && GameState.Get().IsFriendlySidePlayerTurn();
		RemoteActionHandler.UserUI userUI = (!flag) ? this.enemyWantedUI : this.friendlyWantedUI;
		RemoteActionHandler.UserUI userUI2 = (!flag) ? this.enemyActualUI : this.friendlyActualUI;
		if (userUI2.over.card == null)
		{
			return;
		}
		if (userUI2.over.card.GetActor() == null)
		{
			return;
		}
		if (!userUI2.over.card.GetActor().IsShown())
		{
			return;
		}
		if (userUI2.over.card == userUI.origin.card)
		{
			return;
		}
		Vector3 position = Camera.main.transform.position;
		Vector3 position2 = userUI2.over.card.transform.position;
		Ray ray;
		ray..ctor(position, position2 - position);
		RaycastHit raycastHit;
		if (!Physics.Raycast(ray, ref raycastHit, Camera.main.farClipPlane, GameLayer.DragPlane.LayerBit()))
		{
			return;
		}
		TargetReticleManager.Get().SetRemotePlayerArrowPosition(raycastHit.point);
	}

	// Token: 0x0600513D RID: 20797 RVA: 0x00183908 File Offset: 0x00181B08
	private bool IsCardInHand(Card card)
	{
		if (card == null)
		{
			return false;
		}
		Zone zone = card.GetZone();
		if (!(zone is ZoneHand))
		{
			return false;
		}
		Entity entity = card.GetEntity();
		return entity.GetZone() == TAG_ZONE.HAND;
	}

	// Token: 0x0600513E RID: 20798 RVA: 0x00183950 File Offset: 0x00181B50
	private bool CanAnimateHeldCard(Card card)
	{
		if (!this.IsCardInHand(card))
		{
			return false;
		}
		string tweenName = ZoneMgr.Get().GetTweenName<ZoneHand>();
		return !iTween.HasNameNotInList(card.gameObject, new string[]
		{
			"RemoteActionHandler",
			tweenName
		});
	}

	// Token: 0x0600513F RID: 20799 RVA: 0x0018399C File Offset: 0x00181B9C
	private void OnTurnChanged(int oldTurn, int newTurn, object userData)
	{
		Player currentPlayer = GameState.Get().GetCurrentPlayer();
		if (currentPlayer != null && !currentPlayer.IsLocalUser() && !GameMgr.Get().IsSpectator())
		{
			return;
		}
		if (TargetReticleManager.Get() == null)
		{
			return;
		}
		RemoteActionHandler.UserUI userUI;
		if (currentPlayer.IsFriendlySide())
		{
			userUI = this.friendlyActualUI;
			if (TargetReticleManager.Get().IsEnemyArrowActive())
			{
				TargetReticleManager.Get().DestroyEnemyTargetArrow();
			}
		}
		else
		{
			userUI = this.enemyActualUI;
			if (TargetReticleManager.Get().IsLocalArrowActive())
			{
				TargetReticleManager.Get().DestroyFriendlyTargetArrow(false);
			}
		}
		if (userUI.origin != null && userUI.origin.entity != null && userUI.origin.card != null && userUI.origin.entity.HasTag(GAME_TAG.IMMUNE_WHILE_ATTACKING) && !userUI.origin.entity.IsImmune())
		{
			userUI.origin.card.GetActor().DeactivateSpell(SpellType.IMMUNE);
		}
	}

	// Token: 0x06005140 RID: 20800 RVA: 0x00183AB0 File Offset: 0x00181CB0
	private bool CanReceiveEnemyEmote(EmoteType emoteType)
	{
		if (EnemyEmoteHandler.Get() == null)
		{
			return false;
		}
		if (EnemyEmoteHandler.Get().IsSquelched())
		{
			return false;
		}
		if (EmoteHandler.Get() == null)
		{
			return false;
		}
		List<EmoteOption> emotes = EmoteHandler.Get().m_Emotes;
		if (emotes == null)
		{
			return false;
		}
		for (int i = 0; i < emotes.Count; i++)
		{
			EmoteOption emoteOption = emotes[i];
			if (emoteOption.m_EmoteType == emoteType)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040037F5 RID: 14325
	public const string TWEEN_NAME = "RemoteActionHandler";

	// Token: 0x040037F6 RID: 14326
	private const float DRIFT_TIME = 10f;

	// Token: 0x040037F7 RID: 14327
	private const float LOW_FREQ_SEND_TIME = 0.35f;

	// Token: 0x040037F8 RID: 14328
	private const float HIGH_FREQ_SEND_TIME = 0.25f;

	// Token: 0x040037F9 RID: 14329
	private static RemoteActionHandler s_instance;

	// Token: 0x040037FA RID: 14330
	private RemoteActionHandler.UserUI myCurrentUI = new RemoteActionHandler.UserUI();

	// Token: 0x040037FB RID: 14331
	private RemoteActionHandler.UserUI myLastUI = new RemoteActionHandler.UserUI();

	// Token: 0x040037FC RID: 14332
	private RemoteActionHandler.UserUI enemyWantedUI = new RemoteActionHandler.UserUI();

	// Token: 0x040037FD RID: 14333
	private RemoteActionHandler.UserUI enemyActualUI = new RemoteActionHandler.UserUI();

	// Token: 0x040037FE RID: 14334
	private RemoteActionHandler.UserUI friendlyWantedUI = new RemoteActionHandler.UserUI();

	// Token: 0x040037FF RID: 14335
	private RemoteActionHandler.UserUI friendlyActualUI = new RemoteActionHandler.UserUI();

	// Token: 0x04003800 RID: 14336
	private float m_lastSendTime = Time.realtimeSinceStartup;

	// Token: 0x02000929 RID: 2345
	private class CardAndID
	{
		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x060056C8 RID: 22216 RVA: 0x001A0693 File Offset: 0x0019E893
		// (set) Token: 0x060056C9 RID: 22217 RVA: 0x001A069C File Offset: 0x0019E89C
		public Card card
		{
			get
			{
				return this.m_card;
			}
			set
			{
				if (value == this.m_card)
				{
					return;
				}
				if (value == null)
				{
					this.Clear();
					return;
				}
				this.m_card = value;
				this.m_entity = value.GetEntity();
				if (this.m_entity == null)
				{
					Debug.LogWarning("RemoteActionHandler--card has no entity");
					this.Clear();
					return;
				}
				this.m_ID = this.m_entity.GetEntityId();
				if (this.m_ID < 1)
				{
					Debug.LogWarning("RemoteActionHandler--invalid entity ID");
					this.Clear();
				}
			}
		}

		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x060056CA RID: 22218 RVA: 0x001A072A File Offset: 0x0019E92A
		// (set) Token: 0x060056CB RID: 22219 RVA: 0x001A0734 File Offset: 0x0019E934
		public int ID
		{
			get
			{
				return this.m_ID;
			}
			set
			{
				if (value == this.m_ID)
				{
					return;
				}
				if (value == 0)
				{
					this.Clear();
					return;
				}
				this.m_ID = value;
				this.m_entity = GameState.Get().GetEntity(value);
				if (this.m_entity == null)
				{
					Debug.LogWarning("RemoteActionHandler--no entity found for ID");
					this.Clear();
					return;
				}
				this.m_card = this.m_entity.GetCard();
				if (this.m_card == null)
				{
					Debug.LogWarning("RemoteActionHandler--entity has no card");
					this.Clear();
				}
			}
		}

		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x060056CC RID: 22220 RVA: 0x001A07C1 File Offset: 0x0019E9C1
		public Entity entity
		{
			get
			{
				return this.m_entity;
			}
		}

		// Token: 0x060056CD RID: 22221 RVA: 0x001A07C9 File Offset: 0x0019E9C9
		private void Clear()
		{
			this.m_ID = 0;
			this.m_entity = null;
			this.m_card = null;
		}

		// Token: 0x04003D84 RID: 15748
		private int m_ID;

		// Token: 0x04003D85 RID: 15749
		private Entity m_entity;

		// Token: 0x04003D86 RID: 15750
		private Card m_card;
	}

	// Token: 0x0200092A RID: 2346
	private class UserUI
	{
		// Token: 0x060056CF RID: 22223 RVA: 0x001A080C File Offset: 0x0019EA0C
		public bool SameAs(RemoteActionHandler.UserUI compare)
		{
			return !(this.held.card != compare.held.card) && !(this.over.card != compare.over.card) && !(this.origin.card != compare.origin.card);
		}

		// Token: 0x060056D0 RID: 22224 RVA: 0x001A0880 File Offset: 0x0019EA80
		public void CopyFrom(RemoteActionHandler.UserUI source)
		{
			this.held.ID = source.held.ID;
			this.over.ID = source.over.ID;
			this.origin.ID = source.origin.ID;
		}

		// Token: 0x04003D87 RID: 15751
		public RemoteActionHandler.CardAndID over = new RemoteActionHandler.CardAndID();

		// Token: 0x04003D88 RID: 15752
		public RemoteActionHandler.CardAndID held = new RemoteActionHandler.CardAndID();

		// Token: 0x04003D89 RID: 15753
		public RemoteActionHandler.CardAndID origin = new RemoteActionHandler.CardAndID();
	}
}
