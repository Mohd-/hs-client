using System;
using System.Collections.Generic;

// Token: 0x0200013C RID: 316
public class DeckDbfRecord : DbfRecord
{
	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x0600106B RID: 4203 RVA: 0x0004709A File Offset: 0x0004529A
	[DbfField("NOTE_NAME", "Designer name for the deck.  Not used by game.")]
	public string NoteName
	{
		get
		{
			return this.m_NoteName;
		}
	}

	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x0600106C RID: 4204 RVA: 0x000470A2 File Offset: 0x000452A2
	[DbfField("TOP_CARD_ID", "DECK_CARDS.ID of top card in deck (-1 for empty)")]
	public int TopCardId
	{
		get
		{
			return this.m_TopCardId;
		}
	}

	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x0600106D RID: 4205 RVA: 0x000470AA File Offset: 0x000452AA
	[DbfField("NAME", "")]
	public DbfLocValue Name
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x0600106E RID: 4206 RVA: 0x000470B2 File Offset: 0x000452B2
	[DbfField("DESCRIPTION", "")]
	public DbfLocValue Description
	{
		get
		{
			return this.m_Description;
		}
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x000470BA File Offset: 0x000452BA
	public void SetNoteName(string v)
	{
		this.m_NoteName = v;
	}

	// Token: 0x06001070 RID: 4208 RVA: 0x000470C3 File Offset: 0x000452C3
	public void SetTopCardId(int v)
	{
		this.m_TopCardId = v;
	}

	// Token: 0x06001071 RID: 4209 RVA: 0x000470CC File Offset: 0x000452CC
	public void SetName(DbfLocValue v)
	{
		this.m_Name = v;
		v.SetDebugInfo(base.ID, "NAME");
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x000470E6 File Offset: 0x000452E6
	public void SetDescription(DbfLocValue v)
	{
		this.m_Description = v;
		v.SetDebugInfo(base.ID, "DESCRIPTION");
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x00047100 File Offset: 0x00045300
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (DeckDbfRecord.<>f__switch$map22 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_NAME", 1);
				dictionary.Add("TOP_CARD_ID", 2);
				dictionary.Add("NAME", 3);
				dictionary.Add("DESCRIPTION", 4);
				DeckDbfRecord.<>f__switch$map22 = dictionary;
			}
			int num;
			if (DeckDbfRecord.<>f__switch$map22.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteName;
				case 2:
					return this.TopCardId;
				case 3:
					return this.Name;
				case 4:
					return this.Description;
				}
			}
		}
		return null;
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x000471C8 File Offset: 0x000453C8
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (DeckDbfRecord.<>f__switch$map23 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_NAME", 1);
				dictionary.Add("TOP_CARD_ID", 2);
				dictionary.Add("NAME", 3);
				dictionary.Add("DESCRIPTION", 4);
				DeckDbfRecord.<>f__switch$map23 = dictionary;
			}
			int num;
			if (DeckDbfRecord.<>f__switch$map23.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetNoteName((string)val);
					break;
				case 2:
					this.SetTopCardId((int)val);
					break;
				case 3:
					this.SetName((DbfLocValue)val);
					break;
				case 4:
					this.SetDescription((DbfLocValue)val);
					break;
				}
			}
		}
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x000472B8 File Offset: 0x000454B8
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (DeckDbfRecord.<>f__switch$map24 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_NAME", 1);
				dictionary.Add("TOP_CARD_ID", 2);
				dictionary.Add("NAME", 3);
				dictionary.Add("DESCRIPTION", 4);
				DeckDbfRecord.<>f__switch$map24 = dictionary;
			}
			int num;
			if (DeckDbfRecord.<>f__switch$map24.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(string);
				case 2:
					return typeof(int);
				case 3:
					return typeof(DbfLocValue);
				case 4:
					return typeof(DbfLocValue);
				}
			}
		}
		return null;
	}

	// Token: 0x040008B9 RID: 2233
	private string m_NoteName;

	// Token: 0x040008BA RID: 2234
	private int m_TopCardId;

	// Token: 0x040008BB RID: 2235
	private DbfLocValue m_Name;

	// Token: 0x040008BC RID: 2236
	private DbfLocValue m_Description;
}
