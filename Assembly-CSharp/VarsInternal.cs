using System;

// Token: 0x0200023B RID: 571
internal class VarsInternal
{
	// Token: 0x0600216F RID: 8559 RVA: 0x000A3A90 File Offset: 0x000A1C90
	private VarsInternal()
	{
		string clientConfigPath = Vars.GetClientConfigPath();
		if (!this.LoadConfig(clientConfigPath))
		{
		}
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x000A3ACC File Offset: 0x000A1CCC
	public static VarsInternal Get()
	{
		return VarsInternal.s_instance;
	}

	// Token: 0x06002172 RID: 8562 RVA: 0x000A3AD3 File Offset: 0x000A1CD3
	public static void RefreshVars()
	{
		VarsInternal.s_instance = new VarsInternal();
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x000A3ADF File Offset: 0x000A1CDF
	public bool Contains(string key)
	{
		return this.m_vars.ContainsKey(key);
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x000A3AED File Offset: 0x000A1CED
	public string Value(string key)
	{
		return this.m_vars[key];
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x000A3AFB File Offset: 0x000A1CFB
	public void Set(string key, string value)
	{
		this.m_vars[key] = value;
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x000A3B0C File Offset: 0x000A1D0C
	private bool LoadConfig(string path)
	{
		ConfigFile configFile = new ConfigFile();
		if (!configFile.LightLoad(path))
		{
			return false;
		}
		foreach (ConfigFile.Line line in configFile.GetLines())
		{
			this.m_vars[line.m_fullKey] = line.m_value;
		}
		return true;
	}

	// Token: 0x040012BA RID: 4794
	private static VarsInternal s_instance = new VarsInternal();

	// Token: 0x040012BB RID: 4795
	private Map<string, string> m_vars = new Map<string, string>();
}
