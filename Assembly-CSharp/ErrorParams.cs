using System;

// Token: 0x020000C0 RID: 192
public class ErrorParams
{
	// Token: 0x04000502 RID: 1282
	public ErrorType m_type;

	// Token: 0x04000503 RID: 1283
	public string m_header;

	// Token: 0x04000504 RID: 1284
	public string m_message;

	// Token: 0x04000505 RID: 1285
	public Error.AcknowledgeCallback m_ackCallback;

	// Token: 0x04000506 RID: 1286
	public object m_ackUserData;

	// Token: 0x04000507 RID: 1287
	public bool m_allowClick = true;

	// Token: 0x04000508 RID: 1288
	public bool m_redirectToStore;

	// Token: 0x04000509 RID: 1289
	public float m_delayBeforeNextReset;
}
