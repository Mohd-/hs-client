using System;
using UnityEngine;

// Token: 0x02000F07 RID: 3847
public class DisableMesh_ColorAlpha : MonoBehaviour
{
	// Token: 0x06007300 RID: 29440 RVA: 0x0021DAEC File Offset: 0x0021BCEC
	private void Start()
	{
		this.m_material = base.GetComponent<Renderer>().material;
		if (this.m_material == null)
		{
			base.enabled = false;
		}
		if (!this.m_material.HasProperty("_Color"))
		{
			base.enabled = false;
		}
	}

	// Token: 0x06007301 RID: 29441 RVA: 0x0021DB40 File Offset: 0x0021BD40
	private void Update()
	{
		if (this.m_material.color.a == 0f)
		{
			base.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			base.GetComponent<Renderer>().enabled = true;
		}
	}

	// Token: 0x04005D60 RID: 23904
	private Material m_material;
}
