using System;
using UnityEngine;

// Token: 0x02000F08 RID: 3848
public class DisableMesh_ColorBlack : MonoBehaviour
{
	// Token: 0x06007303 RID: 29443 RVA: 0x0021DB9C File Offset: 0x0021BD9C
	private void Start()
	{
		this.m_material = base.GetComponent<Renderer>().material;
		if (this.m_material == null)
		{
			base.enabled = false;
		}
		if (!this.m_material.HasProperty("_Color") && !this.m_material.HasProperty("_TintColor"))
		{
			base.enabled = false;
		}
		if (this.m_material.HasProperty("_TintColor"))
		{
			this.m_tintColor = true;
		}
	}

	// Token: 0x06007304 RID: 29444 RVA: 0x0021DC20 File Offset: 0x0021BE20
	private void Update()
	{
		if (this.m_tintColor)
		{
			this.m_color = this.m_material.GetColor("_TintColor");
		}
		else
		{
			this.m_color = this.m_material.color;
		}
		if (this.m_color.r < 0.01f && this.m_color.g < 0.01f && this.m_color.b < 0.01f)
		{
			base.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			base.GetComponent<Renderer>().enabled = true;
		}
	}

	// Token: 0x04005D61 RID: 23905
	private Material m_material;

	// Token: 0x04005D62 RID: 23906
	private bool m_tintColor;

	// Token: 0x04005D63 RID: 23907
	private Color m_color = Color.black;
}
