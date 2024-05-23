using System;
using System.Collections.Generic;

// Token: 0x0200013A RID: 314
public class CardDbfRecord : DbfRecord
{
	// Token: 0x1700028B RID: 651
	// (get) Token: 0x06001039 RID: 4153 RVA: 0x0004655E File Offset: 0x0004475E
	[DbfField("NOTE_MINI_GUID", "client-side card name")]
	public string NoteMiniGuid
	{
		get
		{
			return this.m_NoteMiniGuid;
		}
	}

	// Token: 0x1700028C RID: 652
	// (get) Token: 0x0600103A RID: 4154 RVA: 0x00046566 File Offset: 0x00044766
	[DbfField("IS_COLLECTIBLE", "show in collection manager?")]
	public bool IsCollectible
	{
		get
		{
			return this.m_IsCollectible;
		}
	}

	// Token: 0x1700028D RID: 653
	// (get) Token: 0x0600103B RID: 4155 RVA: 0x0004656E File Offset: 0x0004476E
	[DbfField("LONG_GUID", "game server file name")]
	public string LongGuid
	{
		get
		{
			return this.m_LongGuid;
		}
	}

	// Token: 0x1700028E RID: 654
	// (get) Token: 0x0600103C RID: 4156 RVA: 0x00046576 File Offset: 0x00044776
	[DbfField("HERO_POWER_ID", "ASSET.CARD.ID of the hero power used by this card")]
	public int HeroPowerId
	{
		get
		{
			return this.m_HeroPowerId;
		}
	}

	// Token: 0x1700028F RID: 655
	// (get) Token: 0x0600103D RID: 4157 RVA: 0x0004657E File Offset: 0x0004477E
	[DbfField("CRAFTING_EVENT", "If value is not 'always', this event controls when this card can be crafted or disenchanted (assuming the card has an entry in CARD_VALUE).")]
	public string CraftingEvent
	{
		get
		{
			return this.m_CraftingEvent;
		}
	}

	// Token: 0x17000290 RID: 656
	// (get) Token: 0x0600103E RID: 4158 RVA: 0x00046586 File Offset: 0x00044786
	[DbfField("SUGGESTION_WEIGHT", "Collection manager card suggestion weight. Typically ranges from 1-10.  Higher is better.")]
	public int SuggestionWeight
	{
		get
		{
			return this.m_SuggestionWeight;
		}
	}

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x0600103F RID: 4159 RVA: 0x0004658E File Offset: 0x0004478E
	[DbfField("CHANGED_MANA_COST", "Should an indication be shown to the player that the mana cost of this card has changed?")]
	public bool ChangedManaCost
	{
		get
		{
			return this.m_ChangedManaCost;
		}
	}

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x06001040 RID: 4160 RVA: 0x00046596 File Offset: 0x00044796
	[DbfField("CHANGED_HEALTH", "Should an indication be shown to the player that the health of this card has changed?")]
	public bool ChangedHealth
	{
		get
		{
			return this.m_ChangedHealth;
		}
	}

	// Token: 0x17000293 RID: 659
	// (get) Token: 0x06001041 RID: 4161 RVA: 0x0004659E File Offset: 0x0004479E
	[DbfField("CHANGED_ATTACK", "Should an indication be shown to the player that the attack of this card has changed?")]
	public bool ChangedAttack
	{
		get
		{
			return this.m_ChangedAttack;
		}
	}

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x06001042 RID: 4162 RVA: 0x000465A6 File Offset: 0x000447A6
	[DbfField("CHANGED_CARD_TEXT_IN_HAND", "Should an indication be shown to the player that the card text has changed?")]
	public bool ChangedCardTextInHand
	{
		get
		{
			return this.m_ChangedCardTextInHand;
		}
	}

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x06001043 RID: 4163 RVA: 0x000465AE File Offset: 0x000447AE
	[DbfField("CHANGE_VERSION", "Whenever any of the 'CHANGED_' values are modified, this is incremented.")]
	public int ChangeVersion
	{
		get
		{
			return this.m_ChangeVersion;
		}
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x000465B6 File Offset: 0x000447B6
	public void SetNoteMiniGuid(string v)
	{
		this.m_NoteMiniGuid = v;
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x000465BF File Offset: 0x000447BF
	public void SetIsCollectible(bool v)
	{
		this.m_IsCollectible = v;
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x000465C8 File Offset: 0x000447C8
	public void SetLongGuid(string v)
	{
		this.m_LongGuid = v;
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x000465D1 File Offset: 0x000447D1
	public void SetHeroPowerId(int v)
	{
		this.m_HeroPowerId = v;
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x000465DA File Offset: 0x000447DA
	public void SetCraftingEvent(string v)
	{
		this.m_CraftingEvent = v;
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x000465E3 File Offset: 0x000447E3
	public void SetSuggestionWeight(int v)
	{
		this.m_SuggestionWeight = v;
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x000465EC File Offset: 0x000447EC
	public void SetChangedManaCost(bool v)
	{
		this.m_ChangedManaCost = v;
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x000465F5 File Offset: 0x000447F5
	public void SetChangedHealth(bool v)
	{
		this.m_ChangedHealth = v;
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x000465FE File Offset: 0x000447FE
	public void SetChangedAttack(bool v)
	{
		this.m_ChangedAttack = v;
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x00046607 File Offset: 0x00044807
	public void SetChangedCardTextInHand(bool v)
	{
		this.m_ChangedCardTextInHand = v;
	}

	// Token: 0x0600104E RID: 4174 RVA: 0x00046610 File Offset: 0x00044810
	public void SetChangeVersion(int v)
	{
		this.m_ChangeVersion = v;
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x0004661C File Offset: 0x0004481C
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (CardDbfRecord.<>f__switch$map1C == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(12);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_MINI_GUID", 1);
				dictionary.Add("IS_COLLECTIBLE", 2);
				dictionary.Add("LONG_GUID", 3);
				dictionary.Add("HERO_POWER_ID", 4);
				dictionary.Add("CRAFTING_EVENT", 5);
				dictionary.Add("SUGGESTION_WEIGHT", 6);
				dictionary.Add("CHANGED_MANA_COST", 7);
				dictionary.Add("CHANGED_HEALTH", 8);
				dictionary.Add("CHANGED_ATTACK", 9);
				dictionary.Add("CHANGED_CARD_TEXT_IN_HAND", 10);
				dictionary.Add("CHANGE_VERSION", 11);
				CardDbfRecord.<>f__switch$map1C = dictionary;
			}
			int num;
			if (CardDbfRecord.<>f__switch$map1C.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteMiniGuid;
				case 2:
					return this.IsCollectible;
				case 3:
					return this.LongGuid;
				case 4:
					return this.HeroPowerId;
				case 5:
					return this.CraftingEvent;
				case 6:
					return this.SuggestionWeight;
				case 7:
					return this.ChangedManaCost;
				case 8:
					return this.ChangedHealth;
				case 9:
					return this.ChangedAttack;
				case 10:
					return this.ChangedCardTextInHand;
				case 11:
					return this.ChangeVersion;
				}
			}
		}
		return null;
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x000467AC File Offset: 0x000449AC
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (CardDbfRecord.<>f__switch$map1D == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(12);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_MINI_GUID", 1);
				dictionary.Add("IS_COLLECTIBLE", 2);
				dictionary.Add("LONG_GUID", 3);
				dictionary.Add("HERO_POWER_ID", 4);
				dictionary.Add("CRAFTING_EVENT", 5);
				dictionary.Add("SUGGESTION_WEIGHT", 6);
				dictionary.Add("CHANGED_MANA_COST", 7);
				dictionary.Add("CHANGED_HEALTH", 8);
				dictionary.Add("CHANGED_ATTACK", 9);
				dictionary.Add("CHANGED_CARD_TEXT_IN_HAND", 10);
				dictionary.Add("CHANGE_VERSION", 11);
				CardDbfRecord.<>f__switch$map1D = dictionary;
			}
			int num;
			if (CardDbfRecord.<>f__switch$map1D.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetNoteMiniGuid((string)val);
					break;
				case 2:
					this.SetIsCollectible((bool)val);
					break;
				case 3:
					this.SetLongGuid((string)val);
					break;
				case 4:
					this.SetHeroPowerId((int)val);
					break;
				case 5:
					this.SetCraftingEvent((string)val);
					break;
				case 6:
					this.SetSuggestionWeight((int)val);
					break;
				case 7:
					this.SetChangedManaCost((bool)val);
					break;
				case 8:
					this.SetChangedHealth((bool)val);
					break;
				case 9:
					this.SetChangedAttack((bool)val);
					break;
				case 10:
					this.SetChangedCardTextInHand((bool)val);
					break;
				case 11:
					this.SetChangeVersion((int)val);
					break;
				}
			}
		}
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x00046988 File Offset: 0x00044B88
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (CardDbfRecord.<>f__switch$map1E == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(12);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_MINI_GUID", 1);
				dictionary.Add("IS_COLLECTIBLE", 2);
				dictionary.Add("LONG_GUID", 3);
				dictionary.Add("HERO_POWER_ID", 4);
				dictionary.Add("CRAFTING_EVENT", 5);
				dictionary.Add("SUGGESTION_WEIGHT", 6);
				dictionary.Add("CHANGED_MANA_COST", 7);
				dictionary.Add("CHANGED_HEALTH", 8);
				dictionary.Add("CHANGED_ATTACK", 9);
				dictionary.Add("CHANGED_CARD_TEXT_IN_HAND", 10);
				dictionary.Add("CHANGE_VERSION", 11);
				CardDbfRecord.<>f__switch$map1E = dictionary;
			}
			int num;
			if (CardDbfRecord.<>f__switch$map1E.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(string);
				case 2:
					return typeof(bool);
				case 3:
					return typeof(string);
				case 4:
					return typeof(int);
				case 5:
					return typeof(string);
				case 6:
					return typeof(int);
				case 7:
					return typeof(bool);
				case 8:
					return typeof(bool);
				case 9:
					return typeof(bool);
				case 10:
					return typeof(bool);
				case 11:
					return typeof(int);
				}
			}
		}
		return null;
	}

	// Token: 0x0400089E RID: 2206
	private string m_NoteMiniGuid;

	// Token: 0x0400089F RID: 2207
	private bool m_IsCollectible;

	// Token: 0x040008A0 RID: 2208
	private string m_LongGuid;

	// Token: 0x040008A1 RID: 2209
	private int m_HeroPowerId;

	// Token: 0x040008A2 RID: 2210
	private string m_CraftingEvent;

	// Token: 0x040008A3 RID: 2211
	private int m_SuggestionWeight;

	// Token: 0x040008A4 RID: 2212
	private bool m_ChangedManaCost;

	// Token: 0x040008A5 RID: 2213
	private bool m_ChangedHealth;

	// Token: 0x040008A6 RID: 2214
	private bool m_ChangedAttack;

	// Token: 0x040008A7 RID: 2215
	private bool m_ChangedCardTextInHand;

	// Token: 0x040008A8 RID: 2216
	private int m_ChangeVersion;
}
