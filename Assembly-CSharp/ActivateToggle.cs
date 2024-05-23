using System;
using UnityEngine;

// Token: 0x02000F70 RID: 3952
public class ActivateToggle : MonoBehaviour
{
	// Token: 0x06007530 RID: 30000 RVA: 0x0022967C File Offset: 0x0022787C
	private void ToggleActive()
	{
		if (this.onoff)
		{
			this.obj.SetActive(false);
		}
		if (!this.onoff)
		{
			this.obj.SetActive(true);
		}
	}

	// Token: 0x04005FB0 RID: 24496
	public GameObject obj;

	// Token: 0x04005FB1 RID: 24497
	private bool onoff;
}
