using System;
using System.Collections.Generic;

// Token: 0x02000140 RID: 320
public class DeckCardDbfRecord : DbfRecord
{
	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x0600109F RID: 4255 RVA: 0x00047CBF File Offset: 0x00045EBF
	[DbfField("NEXT_CARD", "DECK_CARD.ID of next card in deck, or NULL to terminate")]
	public int NextCard
	{
		get
		{
			return this.m_NextCard;
		}
	}

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x060010A0 RID: 4256 RVA: 0x00047CC7 File Offset: 0x00045EC7
	[DbfField("CARD_ID", "ASSET.CARD.ID")]
	public int CardId
	{
		get
		{
			return this.m_CardId;
		}
	}

	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x060010A1 RID: 4257 RVA: 0x00047CCF File Offset: 0x00045ECF
	[DbfField("DECK_ID", "DECK.ID of deck")]
	public int DeckId
	{
		get
		{
			return this.m_DeckId;
		}
	}

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x060010A2 RID: 4258 RVA: 0x00047CD7 File Offset: 0x00045ED7
	[DbfField("DESCRIPTION", "")]
	public DbfLocValue Description
	{
		get
		{
			return this.m_Description;
		}
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x00047CDF File Offset: 0x00045EDF
	public void SetNextCard(int v)
	{
		this.m_NextCard = v;
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x00047CE8 File Offset: 0x00045EE8
	public void SetCardId(int v)
	{
		this.m_CardId = v;
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x00047CF1 File Offset: 0x00045EF1
	public void SetDeckId(int v)
	{
		this.m_DeckId = v;
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x00047CFA File Offset: 0x00045EFA
	public void SetDescription(DbfLocValue v)
	{
		this.m_Description = v;
		v.SetDebugInfo(base.ID, "DESCRIPTION");
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x00047D14 File Offset: 0x00045F14
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (DeckCardDbfRecord.<>f__switch$map1F == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
				dictionary.Add("ID", 0);
				dictionary.Add("NEXT_CARD", 1);
				dictionary.Add("CARD_ID", 2);
				dictionary.Add("DECK_ID", 3);
				dictionary.Add("DESCRIPTION", 4);
				DeckCardDbfRecord.<>f__switch$map1F = dictionary;
			}
			int num;
			if (DeckCardDbfRecord.<>f__switch$map1F.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NextCard;
				case 2:
					return this.CardId;
				case 3:
					return this.DeckId;
				case 4:
					return this.Description;
				}
			}
		}
		return null;
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x00047DE8 File Offset: 0x00045FE8
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (DeckCardDbfRecord.<>f__switch$map20 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
				dictionary.Add("ID", 0);
				dictionary.Add("NEXT_CARD", 1);
				dictionary.Add("CARD_ID", 2);
				dictionary.Add("DECK_ID", 3);
				dictionary.Add("DESCRIPTION", 4);
				DeckCardDbfRecord.<>f__switch$map20 = dictionary;
			}
			int num;
			if (DeckCardDbfRecord.<>f__switch$map20.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetNextCard((int)val);
					break;
				case 2:
					this.SetCardId((int)val);
					break;
				case 3:
					this.SetDeckId((int)val);
					break;
				case 4:
					this.SetDescription((DbfLocValue)val);
					break;
				}
			}
		}
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x00047ED8 File Offset: 0x000460D8
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (DeckCardDbfRecord.<>f__switch$map21 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
				dictionary.Add("ID", 0);
				dictionary.Add("NEXT_CARD", 1);
				dictionary.Add("CARD_ID", 2);
				dictionary.Add("DECK_ID", 3);
				dictionary.Add("DESCRIPTION", 4);
				DeckCardDbfRecord.<>f__switch$map21 = dictionary;
			}
			int num;
			if (DeckCardDbfRecord.<>f__switch$map21.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(int);
				case 2:
					return typeof(int);
				case 3:
					return typeof(int);
				case 4:
					return typeof(DbfLocValue);
				}
			}
		}
		return null;
	}

	// Token: 0x040008D7 RID: 2263
	private int m_NextCard;

	// Token: 0x040008D8 RID: 2264
	private int m_CardId;

	// Token: 0x040008D9 RID: 2265
	private int m_DeckId;

	// Token: 0x040008DA RID: 2266
	private DbfLocValue m_Description;
}
