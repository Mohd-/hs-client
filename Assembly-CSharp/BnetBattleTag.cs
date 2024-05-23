using System;
using UnityEngine;

// Token: 0x020004A9 RID: 1193
public class BnetBattleTag
{
	// Token: 0x060038E5 RID: 14565 RVA: 0x00115E2C File Offset: 0x0011402C
	public static BnetBattleTag CreateFromString(string src)
	{
		BnetBattleTag bnetBattleTag = new BnetBattleTag();
		if (!bnetBattleTag.SetString(src))
		{
			return null;
		}
		return bnetBattleTag;
	}

	// Token: 0x060038E6 RID: 14566 RVA: 0x00115E4E File Offset: 0x0011404E
	public BnetBattleTag Clone()
	{
		return (BnetBattleTag)base.MemberwiseClone();
	}

	// Token: 0x060038E7 RID: 14567 RVA: 0x00115E5B File Offset: 0x0011405B
	public string GetName()
	{
		return this.m_name;
	}

	// Token: 0x060038E8 RID: 14568 RVA: 0x00115E63 File Offset: 0x00114063
	public void SetName(string name)
	{
		this.m_name = name;
	}

	// Token: 0x060038E9 RID: 14569 RVA: 0x00115E6C File Offset: 0x0011406C
	public int GetNumber()
	{
		return this.m_number;
	}

	// Token: 0x060038EA RID: 14570 RVA: 0x00115E74 File Offset: 0x00114074
	public void SetNumber(int number)
	{
		this.m_number = number;
	}

	// Token: 0x060038EB RID: 14571 RVA: 0x00115E7D File Offset: 0x0011407D
	public string GetString()
	{
		return string.Format("{0}#{1}", this.m_name, this.m_number);
	}

	// Token: 0x060038EC RID: 14572 RVA: 0x00115E9C File Offset: 0x0011409C
	public bool SetString(string composite)
	{
		if (composite == null)
		{
			Error.AddDevFatal("BnetBattleTag.SetString() - Given null string.", new object[0]);
			return false;
		}
		string[] array = composite.Split(new char[]
		{
			'#'
		});
		if (array.Length < 2)
		{
			Debug.LogWarningFormat("BnetBattleTag.SetString() - Failed to split BattleTag \"{0}\" into 2 parts - this will prevent this player from showing up in Friends list and other places.", new object[]
			{
				composite
			});
			return false;
		}
		if (!int.TryParse(array[1], ref this.m_number))
		{
			Error.AddDevFatal("BnetBattleTag.SetString() - Failed to parse \"{0}\" into a number. Original string: \"{1}\"", new object[]
			{
				array[1],
				composite
			});
			return false;
		}
		this.m_name = array[0];
		return true;
	}

	// Token: 0x060038ED RID: 14573 RVA: 0x00115F2C File Offset: 0x0011412C
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		BnetBattleTag bnetBattleTag = obj as BnetBattleTag;
		return bnetBattleTag != null && this.m_name == bnetBattleTag.m_name && this.m_number == bnetBattleTag.m_number;
	}

	// Token: 0x060038EE RID: 14574 RVA: 0x00115F77 File Offset: 0x00114177
	public bool Equals(BnetBattleTag other)
	{
		return other != null && this.m_name == other.m_name && this.m_number == other.m_number;
	}

	// Token: 0x060038EF RID: 14575 RVA: 0x00115FA8 File Offset: 0x001141A8
	public override int GetHashCode()
	{
		int num = 17;
		num = num * 11 + this.m_name.GetHashCode();
		return num * 11 + this.m_number.GetHashCode();
	}

	// Token: 0x060038F0 RID: 14576 RVA: 0x00115FDB File Offset: 0x001141DB
	public override string ToString()
	{
		return string.Format("{0}#{1}", this.m_name, this.m_number);
	}

	// Token: 0x060038F1 RID: 14577 RVA: 0x00115FF8 File Offset: 0x001141F8
	public static bool operator ==(BnetBattleTag a, BnetBattleTag b)
	{
		return object.ReferenceEquals(a, b) || (a != null && b != null && a.m_name == b.m_name && a.m_number == b.m_number);
	}

	// Token: 0x060038F2 RID: 14578 RVA: 0x00116048 File Offset: 0x00114248
	public static bool operator !=(BnetBattleTag a, BnetBattleTag b)
	{
		return !(a == b);
	}

	// Token: 0x04002485 RID: 9349
	private string m_name;

	// Token: 0x04002486 RID: 9350
	private int m_number;
}
