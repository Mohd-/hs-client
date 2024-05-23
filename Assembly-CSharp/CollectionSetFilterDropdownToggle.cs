using System;
using UnityEngine;

// Token: 0x020006BF RID: 1727
public class CollectionSetFilterDropdownToggle : PegUIElement
{
	// Token: 0x06004805 RID: 18437 RVA: 0x00159B8C File Offset: 0x00157D8C
	public void SetToggleIconOffset(Vector2? materialOffset)
	{
		if (materialOffset != null)
		{
			this.m_currentIconQuad.material.SetTextureOffset("_MainTex", materialOffset.Value);
		}
	}

	// Token: 0x06004806 RID: 18438 RVA: 0x00159BC1 File Offset: 0x00157DC1
	public void SetEnabledVisual(bool enabled)
	{
		if (this.m_buttonMesh == null)
		{
			return;
		}
		this.m_buttonMesh.material.SetFloat("_Desaturate", (!enabled) ? 1f : 0f);
	}

	// Token: 0x04002F78 RID: 12152
	public MeshRenderer m_currentIconQuad;

	// Token: 0x04002F79 RID: 12153
	public MeshRenderer m_buttonMesh;
}
