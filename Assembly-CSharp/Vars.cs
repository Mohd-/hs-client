using System;

// Token: 0x020000A3 RID: 163
public class Vars
{
	// Token: 0x06000816 RID: 2070 RVA: 0x0001FE17 File Offset: 0x0001E017
	public static VarKey Key(string key)
	{
		return new VarKey(key);
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0001FE1F File Offset: 0x0001E01F
	public static void RefreshVars()
	{
		VarsInternal.RefreshVars();
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0001FE28 File Offset: 0x0001E028
	public static string GetClientConfigPath()
	{
		return "client.config";
	}

	// Token: 0x04000431 RID: 1073
	public const string CONFIG_FILE_NAME = "client.config";
}
