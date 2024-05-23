using System;

// Token: 0x0200028A RID: 650
public class FatalErrorMessage
{
	// Token: 0x040014C0 RID: 5312
	public string m_id;

	// Token: 0x040014C1 RID: 5313
	public string m_text;

	// Token: 0x040014C2 RID: 5314
	public Error.AcknowledgeCallback m_ackCallback;

	// Token: 0x040014C3 RID: 5315
	public object m_ackUserData;

	// Token: 0x040014C4 RID: 5316
	public bool m_allowClick = true;

	// Token: 0x040014C5 RID: 5317
	public bool m_redirectToStore;

	// Token: 0x040014C6 RID: 5318
	public float m_delayBeforeNextReset;
}
