using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000340 RID: 832
public class Zone : MonoBehaviour
{
	// Token: 0x06002B7D RID: 11133 RVA: 0x000D8E3C File Offset: 0x000D703C
	public override string ToString()
	{
		return string.Format("{1} {0}", this.m_ServerTag, this.m_Side);
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x000D8E69 File Offset: 0x000D7069
	public Player GetController()
	{
		return this.m_controller;
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x000D8E71 File Offset: 0x000D7071
	public int GetControllerId()
	{
		return (this.m_controller != null) ? this.m_controller.GetPlayerId() : 0;
	}

	// Token: 0x06002B80 RID: 11136 RVA: 0x000D8E8F File Offset: 0x000D708F
	public void SetController(Player controller)
	{
		this.m_controller = controller;
	}

	// Token: 0x06002B81 RID: 11137 RVA: 0x000D8E98 File Offset: 0x000D7098
	public List<Card> GetCards()
	{
		return this.m_cards;
	}

	// Token: 0x06002B82 RID: 11138 RVA: 0x000D8EA0 File Offset: 0x000D70A0
	public int GetCardCount()
	{
		return this.m_cards.Count;
	}

	// Token: 0x06002B83 RID: 11139 RVA: 0x000D8EAD File Offset: 0x000D70AD
	public Card GetFirstCard()
	{
		return (this.m_cards.Count <= 0) ? null : this.m_cards[0];
	}

	// Token: 0x06002B84 RID: 11140 RVA: 0x000D8ED2 File Offset: 0x000D70D2
	public Card GetLastCard()
	{
		return (this.m_cards.Count <= 0) ? null : this.m_cards[this.m_cards.Count - 1];
	}

	// Token: 0x06002B85 RID: 11141 RVA: 0x000D8F04 File Offset: 0x000D7104
	public Card GetCardAtIndex(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index >= this.m_cards.Count)
		{
			return null;
		}
		return this.m_cards[index];
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x000D8F39 File Offset: 0x000D7139
	public Card GetCardAtPos(int pos)
	{
		return this.GetCardAtIndex(pos - 1);
	}

	// Token: 0x06002B87 RID: 11143 RVA: 0x000D8F44 File Offset: 0x000D7144
	public int GetLastPos()
	{
		return this.m_cards.Count + 1;
	}

	// Token: 0x06002B88 RID: 11144 RVA: 0x000D8F54 File Offset: 0x000D7154
	public int FindCardPos(Card card)
	{
		return 1 + this.m_cards.FindIndex((Card currCard) => currCard == card);
	}

	// Token: 0x06002B89 RID: 11145 RVA: 0x000D8F8C File Offset: 0x000D718C
	public bool ContainsCard(Card card)
	{
		int num = this.FindCardPos(card);
		return num > 0;
	}

	// Token: 0x06002B8A RID: 11146 RVA: 0x000D8FA5 File Offset: 0x000D71A5
	public bool IsOnlyCard(Card card)
	{
		return this.m_cards.Count == 1 && this.m_cards[0] == card;
	}

	// Token: 0x06002B8B RID: 11147 RVA: 0x000D8FCC File Offset: 0x000D71CC
	public void DirtyLayout()
	{
		this.m_layoutDirty = true;
	}

	// Token: 0x06002B8C RID: 11148 RVA: 0x000D8FD5 File Offset: 0x000D71D5
	public bool IsLayoutDirty()
	{
		return this.m_layoutDirty;
	}

	// Token: 0x06002B8D RID: 11149 RVA: 0x000D8FDD File Offset: 0x000D71DD
	public bool IsUpdatingLayout()
	{
		return this.m_updatingLayout;
	}

	// Token: 0x06002B8E RID: 11150 RVA: 0x000D8FE5 File Offset: 0x000D71E5
	public bool IsInputEnabled()
	{
		return this.m_inputBlockerCount <= 0;
	}

	// Token: 0x06002B8F RID: 11151 RVA: 0x000D8FF3 File Offset: 0x000D71F3
	public int GetInputBlockerCount()
	{
		return this.m_inputBlockerCount;
	}

	// Token: 0x06002B90 RID: 11152 RVA: 0x000D8FFB File Offset: 0x000D71FB
	public void AddInputBlocker()
	{
		this.AddInputBlocker(1);
	}

	// Token: 0x06002B91 RID: 11153 RVA: 0x000D9004 File Offset: 0x000D7204
	public void RemoveInputBlocker()
	{
		this.AddInputBlocker(-1);
	}

	// Token: 0x06002B92 RID: 11154 RVA: 0x000D9010 File Offset: 0x000D7210
	public void BlockInput(bool block)
	{
		int count = (!block) ? -1 : 1;
		this.AddInputBlocker(count);
	}

	// Token: 0x06002B93 RID: 11155 RVA: 0x000D9034 File Offset: 0x000D7234
	public void AddInputBlocker(int count)
	{
		int inputBlockerCount = this.m_inputBlockerCount;
		this.m_inputBlockerCount += count;
		if (inputBlockerCount != this.m_inputBlockerCount && inputBlockerCount * this.m_inputBlockerCount == 0)
		{
			this.UpdateInput();
		}
	}

	// Token: 0x06002B94 RID: 11156 RVA: 0x000D9075 File Offset: 0x000D7275
	public bool IsBlockingLayout()
	{
		return this.m_layoutBlockerCount > 0;
	}

	// Token: 0x06002B95 RID: 11157 RVA: 0x000D9080 File Offset: 0x000D7280
	public int GetLayoutBlockerCount()
	{
		return this.m_layoutBlockerCount;
	}

	// Token: 0x06002B96 RID: 11158 RVA: 0x000D9088 File Offset: 0x000D7288
	public void AddLayoutBlocker()
	{
		this.m_layoutBlockerCount++;
	}

	// Token: 0x06002B97 RID: 11159 RVA: 0x000D9098 File Offset: 0x000D7298
	public void RemoveLayoutBlocker()
	{
		this.m_layoutBlockerCount--;
	}

	// Token: 0x06002B98 RID: 11160 RVA: 0x000D90A8 File Offset: 0x000D72A8
	public bool AddUpdateLayoutCompleteCallback(Zone.UpdateLayoutCompleteCallback callback)
	{
		return this.AddUpdateLayoutCompleteCallback(callback, null);
	}

	// Token: 0x06002B99 RID: 11161 RVA: 0x000D90B4 File Offset: 0x000D72B4
	public bool AddUpdateLayoutCompleteCallback(Zone.UpdateLayoutCompleteCallback callback, object userData)
	{
		Zone.UpdateLayoutCompleteListener updateLayoutCompleteListener = new Zone.UpdateLayoutCompleteListener();
		updateLayoutCompleteListener.SetCallback(callback);
		updateLayoutCompleteListener.SetUserData(userData);
		if (this.m_completeListeners.Contains(updateLayoutCompleteListener))
		{
			return false;
		}
		this.m_completeListeners.Add(updateLayoutCompleteListener);
		return true;
	}

	// Token: 0x06002B9A RID: 11162 RVA: 0x000D90F5 File Offset: 0x000D72F5
	public bool RemoveUpdateLayoutCompleteCallback(Zone.UpdateLayoutCompleteCallback callback)
	{
		return this.RemoveUpdateLayoutCompleteCallback(callback, null);
	}

	// Token: 0x06002B9B RID: 11163 RVA: 0x000D9100 File Offset: 0x000D7300
	public bool RemoveUpdateLayoutCompleteCallback(Zone.UpdateLayoutCompleteCallback callback, object userData)
	{
		Zone.UpdateLayoutCompleteListener updateLayoutCompleteListener = new Zone.UpdateLayoutCompleteListener();
		updateLayoutCompleteListener.SetCallback(callback);
		updateLayoutCompleteListener.SetUserData(userData);
		return this.m_completeListeners.Remove(updateLayoutCompleteListener);
	}

	// Token: 0x06002B9C RID: 11164 RVA: 0x000D9130 File Offset: 0x000D7330
	public virtual bool CanAcceptTags(int controllerId, TAG_ZONE zoneTag, TAG_CARDTYPE cardType)
	{
		return this.m_ServerTag == zoneTag && (this.m_controller == null || this.m_controller.GetPlayerId() == controllerId) && cardType != TAG_CARDTYPE.ENCHANTMENT;
	}

	// Token: 0x06002B9D RID: 11165 RVA: 0x000D9173 File Offset: 0x000D7373
	public virtual bool AddCard(Card card)
	{
		this.m_cards.Add(card);
		this.DirtyLayout();
		return true;
	}

	// Token: 0x06002B9E RID: 11166 RVA: 0x000D9188 File Offset: 0x000D7388
	public virtual bool InsertCard(int index, Card card)
	{
		this.m_cards.Insert(index, card);
		this.DirtyLayout();
		return true;
	}

	// Token: 0x06002B9F RID: 11167 RVA: 0x000D91A0 File Offset: 0x000D73A0
	public virtual int RemoveCard(Card card)
	{
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card2 = this.m_cards[i];
			if (card2 == card)
			{
				this.m_cards.RemoveAt(i);
				this.DirtyLayout();
				return i;
			}
		}
		Debug.LogWarning(string.Format("{0}.RemoveCard() - FAILED: {1} tried to remove {2}", this, this.m_controller, card));
		return -1;
	}

	// Token: 0x06002BA0 RID: 11168 RVA: 0x000D9210 File Offset: 0x000D7410
	public virtual void UpdateLayout()
	{
		if (this.m_cards.Count == 0)
		{
			this.UpdateLayoutFinished();
			return;
		}
		if (GameState.Get().IsMulliganManagerActive())
		{
			this.UpdateLayoutFinished();
			return;
		}
		this.m_updatingLayout = true;
		if (this.IsBlockingLayout())
		{
			this.UpdateLayoutFinished();
			return;
		}
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card = this.m_cards[i];
			if (!card.IsDoNotSort())
			{
				card.ShowCard();
				card.EnableTransitioningZones(true);
				iTween.MoveTo(card.gameObject, base.transform.position, 1f);
				iTween.RotateTo(card.gameObject, base.transform.localEulerAngles, 1f);
				iTween.ScaleTo(card.gameObject, base.transform.localScale, 1f);
			}
		}
		this.StartFinishLayoutTimer(1f);
	}

	// Token: 0x06002BA1 RID: 11169 RVA: 0x000D9308 File Offset: 0x000D7508
	public static int CardSortComparison(Card card1, Card card2)
	{
		int zonePosition = card1.GetZonePosition();
		int zonePosition2 = card2.GetZonePosition();
		return zonePosition - zonePosition2;
	}

	// Token: 0x06002BA2 RID: 11170 RVA: 0x000D9328 File Offset: 0x000D7528
	protected void UpdateInput()
	{
		bool flag = this.IsInputEnabled();
		foreach (Card card in this.m_cards)
		{
			Actor actor = card.GetActor();
			if (!(actor == null))
			{
				actor.ToggleForceIdle(!flag);
				actor.ToggleCollider(flag);
				card.UpdateActorState();
			}
		}
		Card mousedOverCard = InputManager.Get().GetMousedOverCard();
		if (flag && this.m_cards.Contains(mousedOverCard))
		{
			mousedOverCard.UpdateProposedManaUsage();
		}
	}

	// Token: 0x06002BA3 RID: 11171 RVA: 0x000D93DC File Offset: 0x000D75DC
	protected void StartFinishLayoutTimer(float delaySec)
	{
		if (delaySec <= Mathf.Epsilon)
		{
			this.UpdateLayoutFinished();
			return;
		}
		Card card2 = this.m_cards.Find((Card card) => card.IsTransitioningZones());
		if (card2 == null)
		{
			this.UpdateLayoutFinished();
			return;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			delaySec,
			"oncomplete",
			"UpdateLayoutFinished",
			"oncompletetarget",
			base.gameObject
		});
		iTween.Timer(base.gameObject, args);
	}

	// Token: 0x06002BA4 RID: 11172 RVA: 0x000D9484 File Offset: 0x000D7684
	protected void UpdateLayoutFinished()
	{
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card = this.m_cards[i];
			card.EnableTransitioningZones(false);
		}
		this.m_updatingLayout = false;
		this.m_layoutDirty = false;
		this.FireUpdateLayoutCompleteCallbacks();
	}

	// Token: 0x06002BA5 RID: 11173 RVA: 0x000D94D8 File Offset: 0x000D76D8
	protected void FireUpdateLayoutCompleteCallbacks()
	{
		if (this.m_completeListeners.Count == 0)
		{
			return;
		}
		Zone.UpdateLayoutCompleteListener[] array = this.m_completeListeners.ToArray();
		this.m_completeListeners.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire(this);
		}
	}

	// Token: 0x04001A57 RID: 6743
	public const float TRANSITION_SEC = 1f;

	// Token: 0x04001A58 RID: 6744
	public TAG_ZONE m_ServerTag;

	// Token: 0x04001A59 RID: 6745
	public Player.Side m_Side;

	// Token: 0x04001A5A RID: 6746
	protected Player m_controller;

	// Token: 0x04001A5B RID: 6747
	protected List<Card> m_cards = new List<Card>();

	// Token: 0x04001A5C RID: 6748
	protected bool m_layoutDirty = true;

	// Token: 0x04001A5D RID: 6749
	protected bool m_updatingLayout;

	// Token: 0x04001A5E RID: 6750
	protected List<Zone.UpdateLayoutCompleteListener> m_completeListeners = new List<Zone.UpdateLayoutCompleteListener>();

	// Token: 0x04001A5F RID: 6751
	protected int m_inputBlockerCount;

	// Token: 0x04001A60 RID: 6752
	protected int m_layoutBlockerCount;

	// Token: 0x02000856 RID: 2134
	// (Invoke) Token: 0x06005232 RID: 21042
	public delegate void UpdateLayoutCompleteCallback(Zone zone, object userData);

	// Token: 0x020008AF RID: 2223
	protected class UpdateLayoutCompleteListener : EventListener<Zone.UpdateLayoutCompleteCallback>
	{
		// Token: 0x0600543C RID: 21564 RVA: 0x00192FD8 File Offset: 0x001911D8
		public void Fire(Zone zone)
		{
			this.m_callback(zone, this.m_userData);
		}
	}
}
