using System;
using UnityEngine;

// Token: 0x02000621 RID: 1569
public class ZoneDeck : Zone
{
	// Token: 0x06004481 RID: 17537 RVA: 0x00149614 File Offset: 0x00147814
	public override void UpdateLayout()
	{
		this.m_updatingLayout = true;
		if (base.IsBlockingLayout())
		{
			base.UpdateLayoutFinished();
			return;
		}
		this.UpdateThickness();
		this.UpdateDeckStateEmotes();
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card = this.m_cards[i];
			if (!card.IsDoNotSort())
			{
				card.HideCard();
				this.SetCardToInDeckState(card);
			}
		}
		base.UpdateLayoutFinished();
	}

	// Token: 0x06004482 RID: 17538 RVA: 0x00149692 File Offset: 0x00147892
	public void SetVisibility(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	// Token: 0x06004483 RID: 17539 RVA: 0x001496A0 File Offset: 0x001478A0
	public void SetCardToInDeckState(Card card)
	{
		card.transform.localEulerAngles = new Vector3(270f, 270f, 0f);
		card.transform.position = base.transform.position;
		card.transform.localScale = new Vector3(0.88f, 0.88f, 0.88f);
		card.EnableTransitioningZones(false);
	}

	// Token: 0x06004484 RID: 17540 RVA: 0x00149708 File Offset: 0x00147908
	public void DoFatigueGlow()
	{
		if (this.m_DeckFatigueGlow == null)
		{
			return;
		}
		this.m_DeckFatigueGlow.ActivateState(SpellStateType.ACTION);
	}

	// Token: 0x06004485 RID: 17541 RVA: 0x00149728 File Offset: 0x00147928
	public bool IsFatigued()
	{
		return this.m_cards.Count == 0;
	}

	// Token: 0x06004486 RID: 17542 RVA: 0x00149738 File Offset: 0x00147938
	public GameObject GetActiveThickness()
	{
		if (this.m_ThicknessFull.GetComponent<Renderer>().enabled)
		{
			return this.m_ThicknessFull;
		}
		if (this.m_Thickness75.GetComponent<Renderer>().enabled)
		{
			return this.m_Thickness75;
		}
		if (this.m_Thickness50.GetComponent<Renderer>().enabled)
		{
			return this.m_Thickness50;
		}
		if (this.m_Thickness25.GetComponent<Renderer>().enabled)
		{
			return this.m_Thickness25;
		}
		if (this.m_Thickness1.GetComponent<Renderer>().enabled)
		{
			return this.m_Thickness1;
		}
		return null;
	}

	// Token: 0x06004487 RID: 17543 RVA: 0x001497D4 File Offset: 0x001479D4
	public GameObject GetThicknessForLayout()
	{
		GameObject activeThickness = this.GetActiveThickness();
		if (activeThickness != null)
		{
			return activeThickness;
		}
		return this.m_Thickness1;
	}

	// Token: 0x06004488 RID: 17544 RVA: 0x001497FC File Offset: 0x001479FC
	public bool AreEmotesSuppressed()
	{
		return this.m_suppressEmotes;
	}

	// Token: 0x06004489 RID: 17545 RVA: 0x00149804 File Offset: 0x00147A04
	public void SetSuppressEmotes(bool suppress)
	{
		this.m_suppressEmotes = suppress;
	}

	// Token: 0x0600448A RID: 17546 RVA: 0x00149810 File Offset: 0x00147A10
	private void UpdateThickness()
	{
		this.m_ThicknessFull.GetComponent<Renderer>().enabled = false;
		this.m_Thickness75.GetComponent<Renderer>().enabled = false;
		this.m_Thickness50.GetComponent<Renderer>().enabled = false;
		this.m_Thickness25.GetComponent<Renderer>().enabled = false;
		this.m_Thickness1.GetComponent<Renderer>().enabled = false;
		int count = this.m_cards.Count;
		if (count == 0)
		{
			this.m_DeckFatigueGlow.ActivateState(SpellStateType.BIRTH);
			this.m_wasFatigued = true;
			return;
		}
		if (this.m_wasFatigued)
		{
			this.m_DeckFatigueGlow.ActivateState(SpellStateType.DEATH);
			this.m_wasFatigued = false;
		}
		if (count == 1)
		{
			this.m_Thickness1.GetComponent<Renderer>().enabled = true;
			return;
		}
		float num = (float)count / 26f;
		if (num > 0.75f)
		{
			this.m_ThicknessFull.GetComponent<Renderer>().enabled = true;
		}
		else if (num > 0.5f)
		{
			this.m_Thickness75.GetComponent<Renderer>().enabled = true;
		}
		else if (num > 0.25f)
		{
			this.m_Thickness50.GetComponent<Renderer>().enabled = true;
		}
		else if (num > 0f)
		{
			this.m_Thickness25.GetComponent<Renderer>().enabled = true;
		}
	}

	// Token: 0x0600448B RID: 17547 RVA: 0x00149958 File Offset: 0x00147B58
	private void UpdateDeckStateEmotes()
	{
		if (GameState.Get().IsMulliganManagerActive())
		{
			return;
		}
		if (this.m_suppressEmotes)
		{
			return;
		}
		int count = this.m_cards.Count;
		if (count <= 0 && !this.m_warnedAboutNoCards)
		{
			this.m_warnedAboutNoCards = true;
			this.m_warnedAboutLastCard = true;
			this.m_controller.GetHeroCard().PlayEmote(EmoteType.NO_CARDS);
			return;
		}
		if (count == 1 && !this.m_warnedAboutLastCard)
		{
			this.m_warnedAboutLastCard = true;
			this.m_controller.GetHeroCard().PlayEmote(EmoteType.LOW_CARDS);
			return;
		}
		if (this.m_warnedAboutLastCard && count > 1)
		{
			this.m_warnedAboutLastCard = false;
		}
		if (this.m_warnedAboutNoCards && count > 0)
		{
			this.m_warnedAboutNoCards = false;
		}
	}

	// Token: 0x04002B71 RID: 11121
	private const int MAX_THICKNESS_CARD_COUNT = 26;

	// Token: 0x04002B72 RID: 11122
	public GameObject m_ThicknessFull;

	// Token: 0x04002B73 RID: 11123
	public GameObject m_Thickness75;

	// Token: 0x04002B74 RID: 11124
	public GameObject m_Thickness50;

	// Token: 0x04002B75 RID: 11125
	public GameObject m_Thickness25;

	// Token: 0x04002B76 RID: 11126
	public GameObject m_Thickness1;

	// Token: 0x04002B77 RID: 11127
	public Spell m_DeckFatigueGlow;

	// Token: 0x04002B78 RID: 11128
	private bool m_suppressEmotes;

	// Token: 0x04002B79 RID: 11129
	private bool m_warnedAboutLastCard;

	// Token: 0x04002B7A RID: 11130
	private bool m_warnedAboutNoCards;

	// Token: 0x04002B7B RID: 11131
	private bool m_wasFatigued;
}
