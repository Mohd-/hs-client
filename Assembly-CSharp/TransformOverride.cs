using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A6D RID: 2669
[ExecuteInEditMode]
public class TransformOverride : MonoBehaviour
{
	// Token: 0x06005D5D RID: 23901 RVA: 0x001C0433 File Offset: 0x001BE633
	public void Awake()
	{
		if (Application.isPlaying)
		{
			this.UpdateObject();
		}
	}

	// Token: 0x06005D5E RID: 23902 RVA: 0x001C0448 File Offset: 0x001BE648
	public void AddCategory(ScreenCategory screen)
	{
		if (!Application.isPlaying)
		{
			this.m_screenCategory.Add(screen);
			this.m_localPosition.Add(base.transform.localPosition);
			this.m_localScale.Add(base.transform.localScale);
			this.m_localRotation.Add(base.transform.localRotation);
		}
	}

	// Token: 0x06005D5F RID: 23903 RVA: 0x001C04AD File Offset: 0x001BE6AD
	public void AddCategory()
	{
		this.AddCategory(PlatformSettings.Screen);
	}

	// Token: 0x06005D60 RID: 23904 RVA: 0x001C04BC File Offset: 0x001BE6BC
	public void UpdateObject()
	{
		int bestScreenMatch = PlatformSettings.GetBestScreenMatch(this.m_screenCategory);
		base.transform.localPosition = this.m_localPosition[bestScreenMatch];
		base.transform.localScale = this.m_localScale[bestScreenMatch];
		base.transform.localRotation = this.m_localRotation[bestScreenMatch];
	}

	// Token: 0x040044FF RID: 17663
	public List<ScreenCategory> m_screenCategory = new List<ScreenCategory>();

	// Token: 0x04004500 RID: 17664
	public List<Vector3> m_localPosition = new List<Vector3>();

	// Token: 0x04004501 RID: 17665
	public List<Vector3> m_localScale = new List<Vector3>();

	// Token: 0x04004502 RID: 17666
	public List<Quaternion> m_localRotation = new List<Quaternion>();

	// Token: 0x04004503 RID: 17667
	public float testVal;
}
