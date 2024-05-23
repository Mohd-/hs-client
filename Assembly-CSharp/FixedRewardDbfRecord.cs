using System;
using System.Collections.Generic;

// Token: 0x02000142 RID: 322
public class FixedRewardDbfRecord : DbfRecord
{
	// Token: 0x170002BB RID: 699
	// (get) Token: 0x060010B9 RID: 4281 RVA: 0x000482F4 File Offset: 0x000464F4
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x060010BA RID: 4282 RVA: 0x000482FC File Offset: 0x000464FC
	[DbfField("TYPE", "reward type")]
	public string Type
	{
		get
		{
			return this.m_Type;
		}
	}

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x060010BB RID: 4283 RVA: 0x00048304 File Offset: 0x00046504
	[DbfField("CARD_ID", "CARD.ID for reward. 0 means ignore.")]
	public int CardId
	{
		get
		{
			return this.m_CardId;
		}
	}

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x060010BC RID: 4284 RVA: 0x0004830C File Offset: 0x0004650C
	[DbfField("CARD_PREMIUM", "card premium for reward. 0 means ignore.")]
	public int CardPremium
	{
		get
		{
			return this.m_CardPremium;
		}
	}

	// Token: 0x170002BF RID: 703
	// (get) Token: 0x060010BD RID: 4285 RVA: 0x00048314 File Offset: 0x00046514
	[DbfField("CARD_BACK_ID", "CARD_BACK.ID for reward. 0 means ignore.")]
	public int CardBackId
	{
		get
		{
			return this.m_CardBackId;
		}
	}

	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x060010BE RID: 4286 RVA: 0x0004831C File Offset: 0x0004651C
	[DbfField("META_ACTION_ID", "meta-action FIXED_REWARD_ACTION.ID granted for this reward. 0 means ignore.")]
	public int MetaActionId
	{
		get
		{
			return this.m_MetaActionId;
		}
	}

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x060010BF RID: 4287 RVA: 0x00048324 File Offset: 0x00046524
	[DbfField("META_ACTION_FLAGS", "meta-action flags granted for this reward. 0 means ignore. flag values are generally any Power-of-2 starting with 1, then 2, 4, 8, 16, etc. theoretically a FIXED_REWARD can grant multiple flags.")]
	public ulong MetaActionFlags
	{
		get
		{
			return this.m_MetaActionFlags;
		}
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x0004832C File Offset: 0x0004652C
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x00048335 File Offset: 0x00046535
	public void SetType(string v)
	{
		this.m_Type = v;
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x0004833E File Offset: 0x0004653E
	public void SetCardId(int v)
	{
		this.m_CardId = v;
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x00048347 File Offset: 0x00046547
	public void SetCardPremium(int v)
	{
		this.m_CardPremium = v;
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x00048350 File Offset: 0x00046550
	public void SetCardBackId(int v)
	{
		this.m_CardBackId = v;
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x00048359 File Offset: 0x00046559
	public void SetMetaActionId(int v)
	{
		this.m_MetaActionId = v;
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x00048362 File Offset: 0x00046562
	public void SetMetaActionFlags(ulong v)
	{
		this.m_MetaActionFlags = v;
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x0004836C File Offset: 0x0004656C
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (FixedRewardDbfRecord.<>f__switch$map34 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(8);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("TYPE", 2);
				dictionary.Add("CARD_ID", 3);
				dictionary.Add("CARD_PREMIUM", 4);
				dictionary.Add("CARD_BACK_ID", 5);
				dictionary.Add("META_ACTION_ID", 6);
				dictionary.Add("META_ACTION_FLAGS", 7);
				FixedRewardDbfRecord.<>f__switch$map34 = dictionary;
			}
			int num;
			if (FixedRewardDbfRecord.<>f__switch$map34.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteDesc;
				case 2:
					return this.Type;
				case 3:
					return this.CardId;
				case 4:
					return this.CardPremium;
				case 5:
					return this.CardBackId;
				case 6:
					return this.MetaActionId;
				case 7:
					return this.MetaActionFlags;
				}
			}
		}
		return null;
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x0004848C File Offset: 0x0004668C
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (FixedRewardDbfRecord.<>f__switch$map35 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(8);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("TYPE", 2);
				dictionary.Add("CARD_ID", 3);
				dictionary.Add("CARD_PREMIUM", 4);
				dictionary.Add("CARD_BACK_ID", 5);
				dictionary.Add("META_ACTION_ID", 6);
				dictionary.Add("META_ACTION_FLAGS", 7);
				FixedRewardDbfRecord.<>f__switch$map35 = dictionary;
			}
			int num;
			if (FixedRewardDbfRecord.<>f__switch$map35.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetNoteDesc((string)val);
					break;
				case 2:
					this.SetType((string)val);
					break;
				case 3:
					this.SetCardId((int)val);
					break;
				case 4:
					this.SetCardPremium((int)val);
					break;
				case 5:
					this.SetCardBackId((int)val);
					break;
				case 6:
					this.SetMetaActionId((int)val);
					break;
				case 7:
					this.SetMetaActionFlags((ulong)val);
					break;
				}
			}
		}
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x000485E0 File Offset: 0x000467E0
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (FixedRewardDbfRecord.<>f__switch$map36 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(8);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("TYPE", 2);
				dictionary.Add("CARD_ID", 3);
				dictionary.Add("CARD_PREMIUM", 4);
				dictionary.Add("CARD_BACK_ID", 5);
				dictionary.Add("META_ACTION_ID", 6);
				dictionary.Add("META_ACTION_FLAGS", 7);
				FixedRewardDbfRecord.<>f__switch$map36 = dictionary;
			}
			int num;
			if (FixedRewardDbfRecord.<>f__switch$map36.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(string);
				case 2:
					return typeof(string);
				case 3:
					return typeof(int);
				case 4:
					return typeof(int);
				case 5:
					return typeof(int);
				case 6:
					return typeof(int);
				case 7:
					return typeof(ulong);
				}
			}
		}
		return null;
	}

	// Token: 0x040008E6 RID: 2278
	private string m_NoteDesc;

	// Token: 0x040008E7 RID: 2279
	private string m_Type;

	// Token: 0x040008E8 RID: 2280
	private int m_CardId;

	// Token: 0x040008E9 RID: 2281
	private int m_CardPremium;

	// Token: 0x040008EA RID: 2282
	private int m_CardBackId;

	// Token: 0x040008EB RID: 2283
	private int m_MetaActionId;

	// Token: 0x040008EC RID: 2284
	private ulong m_MetaActionFlags;
}
