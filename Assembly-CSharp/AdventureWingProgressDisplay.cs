using System;
using UnityEngine;

// Token: 0x020003AC RID: 940
[CustomEditClass]
public class AdventureWingProgressDisplay : MonoBehaviour
{
	// Token: 0x060031B6 RID: 12726 RVA: 0x000FA60F File Offset: 0x000F880F
	public virtual void UpdateProgress(WingDbId wingDbId, bool normalComplete)
	{
	}

	// Token: 0x060031B7 RID: 12727 RVA: 0x000FA611 File Offset: 0x000F8811
	public virtual bool HasProgressAnimationToPlay()
	{
		return false;
	}

	// Token: 0x060031B8 RID: 12728 RVA: 0x000FA614 File Offset: 0x000F8814
	public virtual void PlayProgressAnimation(AdventureWingProgressDisplay.OnAnimationComplete onAnimComplete = null)
	{
		if (onAnimComplete != null)
		{
			onAnimComplete();
		}
	}

	// Token: 0x020003B0 RID: 944
	// (Invoke) Token: 0x060031E0 RID: 12768
	public delegate void OnAnimationComplete();
}
