using System;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

// Token: 0x02000D22 RID: 3362
public class iTweenFSMEvents : MonoBehaviour
{
	// Token: 0x06006A3F RID: 27199 RVA: 0x001F1989 File Offset: 0x001EFB89
	private void iTweenOnStart(int aniTweenID)
	{
		if (this.itweenID == aniTweenID)
		{
			this.itweenFSMAction.Fsm.Event(this.itweenFSMAction.startEvent);
		}
	}

	// Token: 0x06006A40 RID: 27200 RVA: 0x001F19B4 File Offset: 0x001EFBB4
	private void iTweenOnComplete(int aniTweenID)
	{
		if (this.itweenID == aniTweenID)
		{
			if (this.islooping)
			{
				if (!this.donotfinish)
				{
					this.itweenFSMAction.Fsm.Event(this.itweenFSMAction.finishEvent);
					this.itweenFSMAction.Finish();
				}
			}
			else
			{
				this.itweenFSMAction.Fsm.Event(this.itweenFSMAction.finishEvent);
				this.itweenFSMAction.Finish();
			}
		}
	}

	// Token: 0x04005226 RID: 21030
	public static int itweenIDCount;

	// Token: 0x04005227 RID: 21031
	public int itweenID;

	// Token: 0x04005228 RID: 21032
	public iTweenFsmAction itweenFSMAction;

	// Token: 0x04005229 RID: 21033
	public bool donotfinish;

	// Token: 0x0400522A RID: 21034
	public bool islooping;
}
