using System;

// Token: 0x020001A8 RID: 424
public class LogInfo
{
	// Token: 0x04000EA0 RID: 3744
	public string m_name;

	// Token: 0x04000EA1 RID: 3745
	public bool m_consolePrinting;

	// Token: 0x04000EA2 RID: 3746
	public bool m_screenPrinting;

	// Token: 0x04000EA3 RID: 3747
	public bool m_filePrinting;

	// Token: 0x04000EA4 RID: 3748
	public LogLevel m_minLevel = LogLevel.Debug;

	// Token: 0x04000EA5 RID: 3749
	public LogLevel m_defaultLevel = LogLevel.Debug;

	// Token: 0x04000EA6 RID: 3750
	public bool m_verbose;
}
