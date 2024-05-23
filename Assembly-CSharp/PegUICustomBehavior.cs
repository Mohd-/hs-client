using System;
using UnityEngine;

// Token: 0x020002AA RID: 682
public abstract class PegUICustomBehavior : MonoBehaviour
{
	// Token: 0x0600256A RID: 9578 RVA: 0x000B81F1 File Offset: 0x000B63F1
	protected virtual void Awake()
	{
		PegUI.Get().RegisterCustomBehavior(this);
	}

	// Token: 0x0600256B RID: 9579 RVA: 0x000B81FE File Offset: 0x000B63FE
	protected virtual void OnDestroy()
	{
		if (PegUI.Get() != null)
		{
			PegUI.Get().UnregisterCustomBehavior(this);
		}
	}

	// Token: 0x0600256C RID: 9580
	public abstract bool UpdateUI();
}
