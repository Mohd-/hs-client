using System;
using UnityEngine;

// Token: 0x02000A65 RID: 2661
public class DisableOnPlatform : MonoBehaviour
{
	// Token: 0x06005D53 RID: 23891 RVA: 0x001C0354 File Offset: 0x001BE554
	public void Awake()
	{
		if (Application.isPlaying && PlatformSettings.Screen == this.m_screenCategory)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06005D54 RID: 23892 RVA: 0x001C0388 File Offset: 0x001BE588
	public void Update()
	{
		if (Application.isPlaying && PlatformSettings.Screen == this.m_screenCategory)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040044FE RID: 17662
	public ScreenCategory m_screenCategory;
}
