using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000620 RID: 1568
public class ZonePlay : Zone
{
	// Token: 0x06004474 RID: 17524 RVA: 0x001490C8 File Offset: 0x001472C8
	private void Awake()
	{
		this.m_slotWidth = base.GetComponent<Collider>().bounds.size.x / (float)this.m_MaxSlots;
	}

	// Token: 0x06004475 RID: 17525 RVA: 0x001490FE File Offset: 0x001472FE
	public float GetTransitionTime()
	{
		return this.m_transitionTime;
	}

	// Token: 0x06004476 RID: 17526 RVA: 0x00149106 File Offset: 0x00147306
	public void SetTransitionTime(float transitionTime)
	{
		this.m_transitionTime = transitionTime;
	}

	// Token: 0x06004477 RID: 17527 RVA: 0x0014910F File Offset: 0x0014730F
	public void ResetTransitionTime()
	{
		this.m_transitionTime = 1f;
	}

	// Token: 0x06004478 RID: 17528 RVA: 0x0014911C File Offset: 0x0014731C
	public void SortWithSpotForHeldCard(int slot)
	{
		this.m_slotMousedOver = slot;
		this.UpdateLayout();
	}

	// Token: 0x06004479 RID: 17529 RVA: 0x0014912B File Offset: 0x0014732B
	public int GetSlotMousedOver()
	{
		return this.m_slotMousedOver;
	}

	// Token: 0x0600447A RID: 17530 RVA: 0x00149134 File Offset: 0x00147334
	public float GetSlotWidth()
	{
		this.m_slotWidth = base.GetComponent<Collider>().bounds.size.x / (float)this.m_MaxSlots;
		int num = this.m_cards.Count;
		if (this.m_slotMousedOver >= 0)
		{
			num++;
		}
		num = Mathf.Clamp(num, 0, this.m_MaxSlots);
		float num2 = 1f;
		if (UniversalInputManager.UsePhoneUI)
		{
			num2 += this.PHONE_WIDTH_MODIFIERS[num];
		}
		return this.m_slotWidth * num2;
	}

	// Token: 0x0600447B RID: 17531 RVA: 0x001491BC File Offset: 0x001473BC
	public Vector3 GetCardPosition(Card card)
	{
		int index = this.m_cards.FindIndex((Card currCard) => currCard == card);
		return this.GetCardPosition(index);
	}

	// Token: 0x0600447C RID: 17532 RVA: 0x001491F8 File Offset: 0x001473F8
	public Vector3 GetCardPosition(int index)
	{
		if (index < 0 || index >= this.m_cards.Count)
		{
			return base.transform.position;
		}
		int num = this.m_cards.Count;
		if (this.m_slotMousedOver >= 0)
		{
			num++;
		}
		Vector3 center = base.GetComponent<Collider>().bounds.center;
		float num2 = 0.5f * this.GetSlotWidth();
		float num3 = (float)num * num2;
		float num4 = center.x - num3 + num2;
		int num5 = (this.m_slotMousedOver < 0 || index < this.m_slotMousedOver) ? 0 : 1;
		for (int i = 0; i < index; i++)
		{
			Card card = this.m_cards[i];
			if (this.CanAnimateCard(card))
			{
				num5++;
			}
		}
		Vector3 result;
		result..ctor(num4 + (float)num5 * this.GetSlotWidth(), center.y, center.z);
		return result;
	}

	// Token: 0x0600447D RID: 17533 RVA: 0x001492FB File Offset: 0x001474FB
	public override bool CanAcceptTags(int controllerId, TAG_ZONE zoneTag, TAG_CARDTYPE cardType)
	{
		return base.CanAcceptTags(controllerId, zoneTag, cardType) && cardType == TAG_CARDTYPE.MINION;
	}

	// Token: 0x0600447E RID: 17534 RVA: 0x00149318 File Offset: 0x00147518
	public override void UpdateLayout()
	{
		this.m_updatingLayout = true;
		if (base.IsBlockingLayout())
		{
			base.UpdateLayoutFinished();
			return;
		}
		if (InputManager.Get() != null && InputManager.Get().GetHeldCard() == null)
		{
			this.m_slotMousedOver = -1;
		}
		int num = 0;
		this.m_cards.Sort(new Comparison<Card>(Zone.CardSortComparison));
		float num2 = 0f;
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card = this.m_cards[i];
			if (this.CanAnimateCard(card))
			{
				string tweenName = ZoneMgr.Get().GetTweenName<ZonePlay>();
				if (this.m_Side == Player.Side.OPPOSING)
				{
					iTween.StopOthersByName(card.gameObject, tweenName, false);
				}
				Vector3 cardPosition = this.GetCardPosition(i);
				float transitionDelay = card.GetTransitionDelay();
				card.SetTransitionDelay(0f);
				ZoneTransitionStyle transitionStyle = card.GetTransitionStyle();
				card.SetTransitionStyle(ZoneTransitionStyle.NORMAL);
				if (transitionStyle == ZoneTransitionStyle.INSTANT)
				{
					card.EnableTransitioningZones(false);
					card.transform.position = cardPosition;
					card.transform.rotation = base.transform.rotation;
					card.transform.localScale = ((!UniversalInputManager.UsePhoneUI) ? base.transform.localScale : ZonePlay.PHONE_ACTOR_SCALE);
				}
				else
				{
					card.EnableTransitioningZones(true);
					num++;
					Hashtable args = iTween.Hash(new object[]
					{
						"scale",
						(!UniversalInputManager.UsePhoneUI) ? base.transform.localScale : ZonePlay.PHONE_ACTOR_SCALE,
						"delay",
						transitionDelay,
						"time",
						this.m_transitionTime,
						"name",
						tweenName
					});
					iTween.ScaleTo(card.gameObject, args);
					Hashtable args2 = iTween.Hash(new object[]
					{
						"rotation",
						base.transform.eulerAngles,
						"delay",
						transitionDelay,
						"time",
						this.m_transitionTime,
						"name",
						tweenName
					});
					iTween.RotateTo(card.gameObject, args2);
					Hashtable args3 = iTween.Hash(new object[]
					{
						"position",
						cardPosition,
						"delay",
						transitionDelay,
						"time",
						this.m_transitionTime,
						"name",
						tweenName
					});
					iTween.MoveTo(card.gameObject, args3);
					num2 = Mathf.Max(num2, transitionDelay + this.m_transitionTime);
				}
			}
		}
		if (num > 0)
		{
			base.StartFinishLayoutTimer(num2);
		}
		else
		{
			base.UpdateLayoutFinished();
		}
	}

	// Token: 0x0600447F RID: 17535 RVA: 0x001495FC File Offset: 0x001477FC
	protected bool CanAnimateCard(Card card)
	{
		return !card.IsDoNotSort();
	}

	// Token: 0x04002B6A RID: 11114
	private const float DEFAULT_TRANSITION_TIME = 1f;

	// Token: 0x04002B6B RID: 11115
	public int m_MaxSlots = 7;

	// Token: 0x04002B6C RID: 11116
	private float[] PHONE_WIDTH_MODIFIERS = new float[]
	{
		0.25f,
		0.25f,
		0.25f,
		0.25f,
		0.22f,
		0.19f,
		0.15f,
		0.1f
	};

	// Token: 0x04002B6D RID: 11117
	private int m_slotMousedOver = -1;

	// Token: 0x04002B6E RID: 11118
	private float m_slotWidth;

	// Token: 0x04002B6F RID: 11119
	private float m_transitionTime = 1f;

	// Token: 0x04002B70 RID: 11120
	private static Vector3 PHONE_ACTOR_SCALE = new Vector3(1.06f, 1.06f, 1.06f);
}
