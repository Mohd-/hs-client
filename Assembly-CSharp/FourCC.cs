using System;
using System.Text;

// Token: 0x0200054B RID: 1355
[Serializable]
public class FourCC
{
	// Token: 0x06003E5B RID: 15963 RVA: 0x0012DEC0 File Offset: 0x0012C0C0
	public FourCC()
	{
	}

	// Token: 0x06003E5C RID: 15964 RVA: 0x0012DEC8 File Offset: 0x0012C0C8
	public FourCC(uint value)
	{
		this.m_value = value;
	}

	// Token: 0x06003E5D RID: 15965 RVA: 0x0012DED7 File Offset: 0x0012C0D7
	public FourCC(string stringVal)
	{
		this.SetString(stringVal);
	}

	// Token: 0x06003E5E RID: 15966 RVA: 0x0012DEE8 File Offset: 0x0012C0E8
	public FourCC Clone()
	{
		FourCC fourCC = new FourCC();
		fourCC.CopyFrom(this);
		return fourCC;
	}

	// Token: 0x06003E5F RID: 15967 RVA: 0x0012DF03 File Offset: 0x0012C103
	public uint GetValue()
	{
		return this.m_value;
	}

	// Token: 0x06003E60 RID: 15968 RVA: 0x0012DF0B File Offset: 0x0012C10B
	public void SetValue(uint val)
	{
		this.m_value = val;
	}

	// Token: 0x06003E61 RID: 15969 RVA: 0x0012DF14 File Offset: 0x0012C114
	public string GetString()
	{
		StringBuilder stringBuilder = new StringBuilder(4);
		for (int i = 24; i >= 0; i -= 8)
		{
			char c = (char)(this.m_value >> i & 255U);
			if (c != '\0')
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06003E62 RID: 15970 RVA: 0x0012DF68 File Offset: 0x0012C168
	public void SetString(string str)
	{
		this.m_value = 0U;
		int num = 0;
		while (num < str.Length && num < 4)
		{
			this.m_value = (this.m_value << 8 | (uint)((byte)str.get_Chars(num)));
			num++;
		}
	}

	// Token: 0x06003E63 RID: 15971 RVA: 0x0012DFB1 File Offset: 0x0012C1B1
	public void CopyFrom(FourCC other)
	{
		this.m_value = other.m_value;
	}

	// Token: 0x06003E64 RID: 15972 RVA: 0x0012DFC0 File Offset: 0x0012C1C0
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		FourCC fourCC = obj as FourCC;
		return fourCC != null && this.m_value == fourCC.m_value;
	}

	// Token: 0x06003E65 RID: 15973 RVA: 0x0012DFF2 File Offset: 0x0012C1F2
	public bool Equals(FourCC other)
	{
		return other != null && this.m_value == other.m_value;
	}

	// Token: 0x06003E66 RID: 15974 RVA: 0x0012E00A File Offset: 0x0012C20A
	public override int GetHashCode()
	{
		return this.m_value.GetHashCode();
	}

	// Token: 0x06003E67 RID: 15975 RVA: 0x0012E017 File Offset: 0x0012C217
	public override string ToString()
	{
		return this.GetString();
	}

	// Token: 0x06003E68 RID: 15976 RVA: 0x0012E01F File Offset: 0x0012C21F
	public static implicit operator FourCC(uint val)
	{
		return new FourCC(val);
	}

	// Token: 0x06003E69 RID: 15977 RVA: 0x0012E027 File Offset: 0x0012C227
	public static bool operator ==(uint val, FourCC fourCC)
	{
		return !(fourCC == null) && val == fourCC.m_value;
	}

	// Token: 0x06003E6A RID: 15978 RVA: 0x0012E040 File Offset: 0x0012C240
	public static bool operator ==(FourCC fourCC, uint val)
	{
		return !(fourCC == null) && fourCC.m_value == val;
	}

	// Token: 0x06003E6B RID: 15979 RVA: 0x0012E059 File Offset: 0x0012C259
	public static bool operator !=(uint val, FourCC fourCC)
	{
		return !(val == fourCC);
	}

	// Token: 0x06003E6C RID: 15980 RVA: 0x0012E065 File Offset: 0x0012C265
	public static bool operator !=(FourCC fourCC, uint val)
	{
		return !(fourCC == val);
	}

	// Token: 0x06003E6D RID: 15981 RVA: 0x0012E074 File Offset: 0x0012C274
	public static bool operator ==(FourCC a, FourCC b)
	{
		return object.ReferenceEquals(a, b) || (a != null && b != null && a.m_value == b.m_value);
	}

	// Token: 0x06003E6E RID: 15982 RVA: 0x0012E0AB File Offset: 0x0012C2AB
	public static bool operator !=(FourCC a, FourCC b)
	{
		return !(a == b);
	}

	// Token: 0x040027E7 RID: 10215
	protected uint m_value;
}
