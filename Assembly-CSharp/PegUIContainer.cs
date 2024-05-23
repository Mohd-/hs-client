using System;
using UnityEngine;

// Token: 0x02000F5B RID: 3931
public class PegUIContainer : MonoBehaviour
{
	// Token: 0x060074CA RID: 29898 RVA: 0x00227D2C File Offset: 0x00225F2C
	public void SetActive(bool a)
	{
		if (a != base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(a);
		}
	}

	// Token: 0x04005F65 RID: 24421
	public bool isActive = true;
}
