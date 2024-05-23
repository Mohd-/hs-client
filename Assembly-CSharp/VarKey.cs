using System;

// Token: 0x020000A2 RID: 162
public class VarKey
{
	// Token: 0x0600080E RID: 2062 RVA: 0x0001FCFA File Offset: 0x0001DEFA
	public VarKey(string key)
	{
		this.m_key = key;
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x0001FD09 File Offset: 0x0001DF09
	public VarKey(string key, string subKey)
	{
		this.m_key = key + "." + subKey;
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x0001FD23 File Offset: 0x0001DF23
	public VarKey Key(string subKey)
	{
		return new VarKey(this.m_key, subKey);
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x0001FD31 File Offset: 0x0001DF31
	public string GetStr(string def)
	{
		if (VarsInternal.Get().Contains(this.m_key))
		{
			return VarsInternal.Get().Value(this.m_key);
		}
		return def;
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x0001FD5C File Offset: 0x0001DF5C
	public int GetInt(int def)
	{
		if (VarsInternal.Get().Contains(this.m_key))
		{
			string str = VarsInternal.Get().Value(this.m_key);
			return GeneralUtils.ForceInt(str);
		}
		return def;
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0001FD98 File Offset: 0x0001DF98
	public float GetFloat(float def)
	{
		if (VarsInternal.Get().Contains(this.m_key))
		{
			string str = VarsInternal.Get().Value(this.m_key);
			return GeneralUtils.ForceFloat(str);
		}
		return def;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x0001FDD4 File Offset: 0x0001DFD4
	public bool GetBool(bool def)
	{
		if (VarsInternal.Get().Contains(this.m_key))
		{
			string strVal = VarsInternal.Get().Value(this.m_key);
			return GeneralUtils.ForceBool(strVal);
		}
		return def;
	}

	// Token: 0x04000430 RID: 1072
	private string m_key;
}
