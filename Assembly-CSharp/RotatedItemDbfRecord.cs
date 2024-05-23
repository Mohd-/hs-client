using System;
using System.Collections.Generic;

// Token: 0x02000146 RID: 326
public class RotatedItemDbfRecord : DbfRecord
{
	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06001115 RID: 4373 RVA: 0x000497C6 File Offset: 0x000479C6
	[DbfField("ROTATION_EVENT", "EVENT_TIMING.EVENT key that specifies the date and time that rotation occurs.")]
	public string RotationEvent
	{
		get
		{
			return this.m_RotationEvent;
		}
	}

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06001116 RID: 4374 RVA: 0x000497CE File Offset: 0x000479CE
	[DbfField("ITEM_TYPE", "Type of product (see enum ERotatedProductType server code) E.g. {Booster = 1, Adventure = 2, CardSet = 3, etc.}")]
	public int ItemType
	{
		get
		{
			return this.m_ItemType;
		}
	}

	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x06001117 RID: 4375 RVA: 0x000497D6 File Offset: 0x000479D6
	[DbfField("ITEM_ID", "BOOSTER.ID if this is an expansion (for STORE). ADVENTURE.ID if this is an adventure (for STORE).")]
	public int ItemId
	{
		get
		{
			return this.m_ItemId;
		}
	}

	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x06001118 RID: 4376 RVA: 0x000497DE File Offset: 0x000479DE
	[DbfField("CARD_SET_ID", "CARD.CARD_SET_ID if this is Card Set (for VALIDATION)")]
	public int CardSetId
	{
		get
		{
			return this.m_CardSetId;
		}
	}

	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06001119 RID: 4377 RVA: 0x000497E6 File Offset: 0x000479E6
	[DbfField("CARD_ID", "CARD.ID if this is a card (for VALIDATION).")]
	public int CardId
	{
		get
		{
			return this.m_CardId;
		}
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x000497EE File Offset: 0x000479EE
	public void SetRotationEvent(string v)
	{
		this.m_RotationEvent = v;
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x000497F7 File Offset: 0x000479F7
	public void SetItemType(int v)
	{
		this.m_ItemType = v;
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x00049800 File Offset: 0x00047A00
	public void SetItemId(int v)
	{
		this.m_ItemId = v;
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x00049809 File Offset: 0x00047A09
	public void SetCardSetId(int v)
	{
		this.m_CardSetId = v;
	}

	// Token: 0x0600111E RID: 4382 RVA: 0x00049812 File Offset: 0x00047A12
	public void SetCardId(int v)
	{
		this.m_CardId = v;
	}

	// Token: 0x0600111F RID: 4383 RVA: 0x0004981C File Offset: 0x00047A1C
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (RotatedItemDbfRecord.<>f__switch$map3D == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
				dictionary.Add("ID", 0);
				dictionary.Add("ROTATION_EVENT", 1);
				dictionary.Add("ITEM_TYPE", 2);
				dictionary.Add("ITEM_ID", 3);
				dictionary.Add("CARD_SET_ID", 4);
				dictionary.Add("CARD_ID", 5);
				RotatedItemDbfRecord.<>f__switch$map3D = dictionary;
			}
			int num;
			if (RotatedItemDbfRecord.<>f__switch$map3D.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.RotationEvent;
				case 2:
					return this.ItemType;
				case 3:
					return this.ItemId;
				case 4:
					return this.CardSetId;
				case 5:
					return this.CardId;
				}
			}
		}
		return null;
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x0004990C File Offset: 0x00047B0C
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (RotatedItemDbfRecord.<>f__switch$map3E == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
				dictionary.Add("ID", 0);
				dictionary.Add("ROTATION_EVENT", 1);
				dictionary.Add("ITEM_TYPE", 2);
				dictionary.Add("ITEM_ID", 3);
				dictionary.Add("CARD_SET_ID", 4);
				dictionary.Add("CARD_ID", 5);
				RotatedItemDbfRecord.<>f__switch$map3E = dictionary;
			}
			int num;
			if (RotatedItemDbfRecord.<>f__switch$map3E.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetRotationEvent((string)val);
					break;
				case 2:
					this.SetItemType((int)val);
					break;
				case 3:
					this.SetItemId((int)val);
					break;
				case 4:
					this.SetCardSetId((int)val);
					break;
				case 5:
					this.SetCardId((int)val);
					break;
				}
			}
		}
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x00049A1C File Offset: 0x00047C1C
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (RotatedItemDbfRecord.<>f__switch$map3F == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
				dictionary.Add("ID", 0);
				dictionary.Add("ROTATION_EVENT", 1);
				dictionary.Add("ITEM_TYPE", 2);
				dictionary.Add("ITEM_ID", 3);
				dictionary.Add("CARD_SET_ID", 4);
				dictionary.Add("CARD_ID", 5);
				RotatedItemDbfRecord.<>f__switch$map3F = dictionary;
			}
			int num;
			if (RotatedItemDbfRecord.<>f__switch$map3F.TryGetValue(name, ref num))
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
					return typeof(int);
				case 4:
					return typeof(int);
				case 5:
					return typeof(int);
				}
			}
		}
		return null;
	}

	// Token: 0x04000918 RID: 2328
	private string m_RotationEvent;

	// Token: 0x04000919 RID: 2329
	private int m_ItemType;

	// Token: 0x0400091A RID: 2330
	private int m_ItemId;

	// Token: 0x0400091B RID: 2331
	private int m_CardSetId;

	// Token: 0x0400091C RID: 2332
	private int m_CardId;
}
