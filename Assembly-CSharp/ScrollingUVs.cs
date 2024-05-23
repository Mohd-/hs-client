using System;
using UnityEngine;

// Token: 0x02000F37 RID: 3895
public class ScrollingUVs : MonoBehaviour
{
	// Token: 0x060073CC RID: 29644 RVA: 0x00221A53 File Offset: 0x0021FC53
	private void Start()
	{
		this.m_material = base.GetComponent<Renderer>().materials[this.materialIndex];
	}

	// Token: 0x060073CD RID: 29645 RVA: 0x00221A70 File Offset: 0x0021FC70
	private void LateUpdate()
	{
		if (!base.GetComponent<Renderer>().enabled)
		{
			return;
		}
		if (this.m_material == null)
		{
			this.m_material = base.GetComponent<Renderer>().materials[this.materialIndex];
		}
		this.m_offset += this.uvAnimationRate * Time.deltaTime;
		this.m_material.SetTextureOffset("_MainTex", this.m_offset);
	}

	// Token: 0x04005E67 RID: 24167
	public int materialIndex;

	// Token: 0x04005E68 RID: 24168
	public Vector2 uvAnimationRate = new Vector2(1f, 1f);

	// Token: 0x04005E69 RID: 24169
	private Material m_material;

	// Token: 0x04005E6A RID: 24170
	private Vector2 m_offset = Vector2.zero;
}
