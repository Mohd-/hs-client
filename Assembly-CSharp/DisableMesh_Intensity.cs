using System;
using UnityEngine;

// Token: 0x02000F09 RID: 3849
public class DisableMesh_Intensity : MonoBehaviour
{
	// Token: 0x06007306 RID: 29446 RVA: 0x0021DCC8 File Offset: 0x0021BEC8
	private void Start()
	{
		this.m_material = base.GetComponent<Renderer>().material;
		if (this.m_material == null)
		{
			base.enabled = false;
		}
		if (!this.m_material.HasProperty("_Intensity"))
		{
			base.enabled = false;
		}
	}

	// Token: 0x06007307 RID: 29447 RVA: 0x0021DD1C File Offset: 0x0021BF1C
	private void Update()
	{
		if (this.m_material.GetFloat("_Intensity") == 0f)
		{
			base.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			base.GetComponent<Renderer>().enabled = true;
		}
	}

	// Token: 0x04005D64 RID: 23908
	private Material m_material;
}
