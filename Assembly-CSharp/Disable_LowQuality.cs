using System;
using UnityEngine;

// Token: 0x02000F0A RID: 3850
public class Disable_LowQuality : MonoBehaviour
{
	// Token: 0x06007309 RID: 29449 RVA: 0x0021DD68 File Offset: 0x0021BF68
	private void Awake()
	{
		GraphicsManager.Get().RegisterLowQualityDisableObject(base.gameObject);
		if (GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.Low)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600730A RID: 29450 RVA: 0x0021DDA0 File Offset: 0x0021BFA0
	private void OnDestroy()
	{
		GraphicsManager graphicsManager = GraphicsManager.Get();
		if (graphicsManager != null)
		{
			graphicsManager.DeregisterLowQualityDisableObject(base.gameObject);
		}
	}
}
