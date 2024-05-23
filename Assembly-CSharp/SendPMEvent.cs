using System;
using UnityEngine;

// Token: 0x02000F39 RID: 3897
public class SendPMEvent : MonoBehaviour
{
	// Token: 0x060073E3 RID: 29667 RVA: 0x00221DD3 File Offset: 0x0021FFD3
	public void SendEvent()
	{
		this.fsm.SendEvent(this.eventName);
	}

	// Token: 0x04005E6B RID: 24171
	public string eventName;

	// Token: 0x04005E6C RID: 24172
	public PlayMakerFSM fsm;
}
