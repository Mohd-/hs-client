using System;
using UnityEngine;

// Token: 0x02000002 RID: 2
public class AutomationInterpretor
{
	// Token: 0x06000002 RID: 2 RVA: 0x0000289B File Offset: 0x00000A9B
	public static AutomationInterpretor Get()
	{
		if (AutomationInterpretor.s_instance == null)
		{
			AutomationInterpretor.s_instance = new AutomationInterpretor();
		}
		return AutomationInterpretor.s_instance;
	}

	// Token: 0x06000003 RID: 3 RVA: 0x000028B6 File Offset: 0x00000AB6
	public void Start()
	{
	}

	// Token: 0x06000004 RID: 4 RVA: 0x000028B8 File Offset: 0x00000AB8
	public void Update()
	{
	}

	// Token: 0x04000001 RID: 1
	public const string s_newPacketTag = "<!--NEWPACKET-->";

	// Token: 0x04000002 RID: 2
	public const float s_connectionTimeoutLength = 60f;

	// Token: 0x04000003 RID: 3
	private const string CUSTOM_ARG_PREFIX = "+";

	// Token: 0x04000004 RID: 4
	public bool m_isClosing;

	// Token: 0x04000005 RID: 5
	public bool m_isClosed;

	// Token: 0x04000006 RID: 6
	public KeyCode m_ExportMouseKey = 290;

	// Token: 0x04000007 RID: 7
	public bool m_initLoadComplete;

	// Token: 0x04000008 RID: 8
	public bool m_wasPaused;

	// Token: 0x04000009 RID: 9
	private static bool m_isDebugBuild;

	// Token: 0x0400000A RID: 10
	private static bool m_DebugLog;

	// Token: 0x0400000B RID: 11
	private static AutomationInterpretor s_instance;
}
