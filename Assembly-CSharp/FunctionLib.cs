using System;
using UnityEngine;

// Token: 0x02000D8B RID: 3467
public class FunctionLib : MonoBehaviour
{
	// Token: 0x06006C37 RID: 27703 RVA: 0x001FD83B File Offset: 0x001FBA3B
	private void onAnimaitonEvent()
	{
		this.lightningScript.Spawn(this.target.transform, this.destination.transform);
	}

	// Token: 0x040054CA RID: 21706
	public LightningCtrl lightningScript;

	// Token: 0x040054CB RID: 21707
	public GameObject target;

	// Token: 0x040054CC RID: 21708
	public GameObject destination;
}
