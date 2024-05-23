using System;
using PegasusShared;

// Token: 0x02000628 RID: 1576
public class CollectionDeckSlot
{
	// Token: 0x060044CF RID: 17615 RVA: 0x0014AC50 File Offset: 0x00148E50
	public override string ToString()
	{
		return string.Format("[CollectionDeckSlot: Index={0}, Premium={1}, Count={2}, CardID={3}]", new object[]
		{
			this.Index,
			this.Premium,
			this.Count,
			this.CardID
		});
	}

	// Token: 0x170004B3 RID: 1203
	// (get) Token: 0x060044D0 RID: 17616 RVA: 0x0014ACA0 File Offset: 0x00148EA0
	public long UID
	{
		get
		{
			return GameUtils.CardUID(this.CardID, this.Premium);
		}
	}

	// Token: 0x170004B4 RID: 1204
	// (get) Token: 0x060044D1 RID: 17617 RVA: 0x0014ACB4 File Offset: 0x00148EB4
	public long ClientUID
	{
		get
		{
			return GameUtils.ClientCardUID(this.CardID, this.Premium, this.Owned);
		}
	}

	// Token: 0x060044D2 RID: 17618 RVA: 0x0014ACD8 File Offset: 0x00148ED8
	public long GetUID(DeckType deckType)
	{
		return (deckType != 1) ? this.UID : this.ClientUID;
	}

	// Token: 0x170004B5 RID: 1205
	// (get) Token: 0x060044D3 RID: 17619 RVA: 0x0014ACF2 File Offset: 0x00148EF2
	// (set) Token: 0x060044D4 RID: 17620 RVA: 0x0014ACFA File Offset: 0x00148EFA
	public int Index
	{
		get
		{
			return this.m_index;
		}
		set
		{
			this.m_index = value;
		}
	}

	// Token: 0x170004B6 RID: 1206
	// (get) Token: 0x060044D5 RID: 17621 RVA: 0x0014AD03 File Offset: 0x00148F03
	// (set) Token: 0x060044D6 RID: 17622 RVA: 0x0014AD0B File Offset: 0x00148F0B
	public TAG_PREMIUM Premium
	{
		get
		{
			return this.m_premium;
		}
		set
		{
			this.m_premium = value;
		}
	}

	// Token: 0x170004B7 RID: 1207
	// (get) Token: 0x060044D7 RID: 17623 RVA: 0x0014AD14 File Offset: 0x00148F14
	// (set) Token: 0x060044D8 RID: 17624 RVA: 0x0014AD1C File Offset: 0x00148F1C
	public int Count
	{
		get
		{
			return this.m_count;
		}
		set
		{
			this.m_count = value;
			if (this.m_count > 0)
			{
				return;
			}
			if (this.OnSlotEmptied != null)
			{
				this.OnSlotEmptied(this);
			}
		}
	}

	// Token: 0x170004B8 RID: 1208
	// (get) Token: 0x060044D9 RID: 17625 RVA: 0x0014AD49 File Offset: 0x00148F49
	// (set) Token: 0x060044DA RID: 17626 RVA: 0x0014AD51 File Offset: 0x00148F51
	public string CardID
	{
		get
		{
			return this.m_cardId;
		}
		set
		{
			this.m_cardId = value;
		}
	}

	// Token: 0x170004B9 RID: 1209
	// (get) Token: 0x060044DB RID: 17627 RVA: 0x0014AD5A File Offset: 0x00148F5A
	// (set) Token: 0x060044DC RID: 17628 RVA: 0x0014AD62 File Offset: 0x00148F62
	public bool Owned
	{
		get
		{
			return this.m_owned;
		}
		set
		{
			this.m_owned = value;
		}
	}

	// Token: 0x060044DD RID: 17629 RVA: 0x0014AD6C File Offset: 0x00148F6C
	public void CopyFrom(CollectionDeckSlot otherSlot)
	{
		this.Index = otherSlot.Index;
		this.Count = otherSlot.Count;
		this.CardID = otherSlot.CardID;
		this.Premium = otherSlot.Premium;
		this.Owned = otherSlot.Owned;
	}

	// Token: 0x04002BB1 RID: 11185
	public CollectionDeckSlot.DelOnSlotEmptied OnSlotEmptied;

	// Token: 0x04002BB2 RID: 11186
	private int m_index;

	// Token: 0x04002BB3 RID: 11187
	private TAG_PREMIUM m_premium;

	// Token: 0x04002BB4 RID: 11188
	private int m_count;

	// Token: 0x04002BB5 RID: 11189
	private string m_cardId;

	// Token: 0x04002BB6 RID: 11190
	private bool m_owned = true;

	// Token: 0x0200062E RID: 1582
	// (Invoke) Token: 0x060044F1 RID: 17649
	public delegate void DelOnSlotEmptied(CollectionDeckSlot slot);
}
