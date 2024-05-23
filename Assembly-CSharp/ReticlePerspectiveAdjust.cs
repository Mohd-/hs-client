using System;
using UnityEngine;

// Token: 0x02000F31 RID: 3889
public class ReticlePerspectiveAdjust : MonoBehaviour
{
	// Token: 0x060073B9 RID: 29625 RVA: 0x00221566 File Offset: 0x0021F766
	private void Start()
	{
	}

	// Token: 0x060073BA RID: 29626 RVA: 0x00221568 File Offset: 0x0021F768
	private void Update()
	{
		Camera main = Camera.main;
		if (main == null)
		{
			return;
		}
		Vector3 vector = main.WorldToScreenPoint(base.transform.position);
		float num = vector.x / (float)main.pixelWidth - 0.5f;
		float num2 = -(vector.y / (float)main.pixelHeight - 0.5f);
		base.transform.rotation = Quaternion.identity;
		base.transform.Rotate(new Vector3(this.m_VertialAdjustment * num2, 0f, this.m_HorizontalAdjustment * num), 0);
	}

	// Token: 0x04005E4F RID: 24143
	public float m_HorizontalAdjustment = 20f;

	// Token: 0x04005E50 RID: 24144
	public float m_VertialAdjustment = 20f;
}
