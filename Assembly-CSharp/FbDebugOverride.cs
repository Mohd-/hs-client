using System;

// Token: 0x02000B15 RID: 2837
public static class FbDebugOverride
{
	// Token: 0x0600611A RID: 24858 RVA: 0x001D08B7 File Offset: 0x001CEAB7
	public static void Error(string msg)
	{
		if (FbDebugOverride.allowLogging)
		{
			FbDebug.Error(msg);
		}
	}

	// Token: 0x0600611B RID: 24859 RVA: 0x001D08C9 File Offset: 0x001CEAC9
	public static void Info(string msg)
	{
		if (FbDebugOverride.allowLogging)
		{
			FbDebug.Info(msg);
		}
	}

	// Token: 0x0600611C RID: 24860 RVA: 0x001D08DB File Offset: 0x001CEADB
	public static void Log(string msg)
	{
		if (FbDebugOverride.allowLogging)
		{
			FbDebug.Log(msg);
		}
	}

	// Token: 0x0600611D RID: 24861 RVA: 0x001D08ED File Offset: 0x001CEAED
	public static void Warn(string msg)
	{
		if (FbDebugOverride.allowLogging)
		{
			FbDebug.Warn(msg);
		}
	}

	// Token: 0x0400488E RID: 18574
	private static bool allowLogging = true;
}
