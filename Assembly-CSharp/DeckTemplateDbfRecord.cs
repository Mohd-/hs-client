using System;
using System.Collections.Generic;

// Token: 0x02000141 RID: 321
public class DeckTemplateDbfRecord : DbfRecord
{
	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x060010AB RID: 4267 RVA: 0x00047FB1 File Offset: 0x000461B1
	[DbfField("CLASS_ID", "points at CLASS.ID to identify which class this deck is available for")]
	public int ClassId
	{
		get
		{
			return this.m_ClassId;
		}
	}

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x060010AC RID: 4268 RVA: 0x00047FB9 File Offset: 0x000461B9
	[DbfField("EVENT", "the event string, from EVENT_TIMING, that activates these rows.")]
	public string Event
	{
		get
		{
			return this.m_Event;
		}
	}

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x060010AD RID: 4269 RVA: 0x00047FC1 File Offset: 0x000461C1
	[DbfField("SORT_ORDER", "the sorting order for deck templates.")]
	public int SortOrder
	{
		get
		{
			return this.m_SortOrder;
		}
	}

	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x060010AE RID: 4270 RVA: 0x00047FC9 File Offset: 0x000461C9
	[DbfField("DECK_ID", "points at DECK.ID to get the name, description, and card-list for the deck")]
	public int DeckId
	{
		get
		{
			return this.m_DeckId;
		}
	}

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x060010AF RID: 4271 RVA: 0x00047FD1 File Offset: 0x000461D1
	[DbfField("DISPLAY_TEXTURE", "Display texture to use in the deck template picker display")]
	public string DisplayTexture
	{
		get
		{
			return this.m_DisplayTexture;
		}
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x00047FD9 File Offset: 0x000461D9
	public void SetClassId(int v)
	{
		this.m_ClassId = v;
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x00047FE2 File Offset: 0x000461E2
	public void SetEvent(string v)
	{
		this.m_Event = v;
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x00047FEB File Offset: 0x000461EB
	public void SetSortOrder(int v)
	{
		this.m_SortOrder = v;
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x00047FF4 File Offset: 0x000461F4
	public void SetDeckId(int v)
	{
		this.m_DeckId = v;
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x00047FFD File Offset: 0x000461FD
	public void SetDisplayTexture(string v)
	{
		this.m_DisplayTexture = v;
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x00048008 File Offset: 0x00046208
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (DeckTemplateDbfRecord.<>f__switch$map2E == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
				dictionary.Add("ID", 0);
				dictionary.Add("CLASS_ID", 1);
				dictionary.Add("EVENT", 2);
				dictionary.Add("SORT_ORDER", 3);
				dictionary.Add("DECK_ID", 4);
				dictionary.Add("DISPLAY_TEXTURE", 5);
				DeckTemplateDbfRecord.<>f__switch$map2E = dictionary;
			}
			int num;
			if (DeckTemplateDbfRecord.<>f__switch$map2E.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.ClassId;
				case 2:
					return this.Event;
				case 3:
					return this.SortOrder;
				case 4:
					return this.DeckId;
				case 5:
					return this.DisplayTexture;
				}
			}
		}
		return null;
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x000480F0 File Offset: 0x000462F0
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (DeckTemplateDbfRecord.<>f__switch$map2F == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
				dictionary.Add("ID", 0);
				dictionary.Add("CLASS_ID", 1);
				dictionary.Add("EVENT", 2);
				dictionary.Add("SORT_ORDER", 3);
				dictionary.Add("DECK_ID", 4);
				dictionary.Add("DISPLAY_TEXTURE", 5);
				DeckTemplateDbfRecord.<>f__switch$map2F = dictionary;
			}
			int num;
			if (DeckTemplateDbfRecord.<>f__switch$map2F.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetClassId((int)val);
					break;
				case 2:
					this.SetEvent((string)val);
					break;
				case 3:
					this.SetSortOrder((int)val);
					break;
				case 4:
					this.SetDeckId((int)val);
					break;
				case 5:
					this.SetDisplayTexture((string)val);
					break;
				}
			}
		}
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x00048200 File Offset: 0x00046400
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (DeckTemplateDbfRecord.<>f__switch$map30 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
				dictionary.Add("ID", 0);
				dictionary.Add("CLASS_ID", 1);
				dictionary.Add("EVENT", 2);
				dictionary.Add("SORT_ORDER", 3);
				dictionary.Add("DECK_ID", 4);
				dictionary.Add("DISPLAY_TEXTURE", 5);
				DeckTemplateDbfRecord.<>f__switch$map30 = dictionary;
			}
			int num;
			if (DeckTemplateDbfRecord.<>f__switch$map30.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(int);
				case 2:
					return typeof(string);
				case 3:
					return typeof(int);
				case 4:
					return typeof(int);
				case 5:
					return typeof(string);
				}
			}
		}
		return null;
	}

	// Token: 0x040008DE RID: 2270
	private int m_ClassId;

	// Token: 0x040008DF RID: 2271
	private string m_Event;

	// Token: 0x040008E0 RID: 2272
	private int m_SortOrder;

	// Token: 0x040008E1 RID: 2273
	private int m_DeckId;

	// Token: 0x040008E2 RID: 2274
	private string m_DisplayTexture;
}
