using System;
using UnityEngine;

// Token: 0x020007B8 RID: 1976
public class CollectionSetFilterDropdownItem : PegUIElement
{
	// Token: 0x06004D81 RID: 19841 RVA: 0x00171500 File Offset: 0x0016F700
	public Vector2? GetIconMaterialOffset()
	{
		return this.m_iconMaterialOffset;
	}

	// Token: 0x06004D82 RID: 19842 RVA: 0x00171508 File Offset: 0x0016F708
	public void Select(bool selection)
	{
		this.m_selected = selection;
		this.SetItemColors((!this.m_selected) ? this.m_unselectedColor : this.m_selectedColor);
		this.m_selectedBar.SetActive(selection);
		if (this.m_selected)
		{
			this.m_mouseOverBar.SetActive(false);
		}
	}

	// Token: 0x06004D83 RID: 19843 RVA: 0x00171561 File Offset: 0x0016F761
	public void SetName(string name)
	{
		this.m_dropdownText.Text = name;
	}

	// Token: 0x06004D84 RID: 19844 RVA: 0x00171570 File Offset: 0x0016F770
	public void SetIconMaterialOffset(Vector2 offset)
	{
		this.m_iconMaterialOffset = new Vector2?(offset);
		this.m_iconRenderer.material.SetTextureOffset("_MainTex", offset);
	}

	// Token: 0x06004D85 RID: 19845 RVA: 0x001715A0 File Offset: 0x0016F7A0
	public void DisableIconMaterial()
	{
		this.m_iconMaterialOffset = default(Vector2?);
		this.m_iconRenderer.material.SetTextureScale("_MainTex", Vector2.zero);
	}

	// Token: 0x06004D86 RID: 19846 RVA: 0x001715D8 File Offset: 0x0016F7D8
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (this.m_selected)
		{
			return;
		}
		this.SetItemColors(this.m_mouseOverColor);
		this.m_mouseOverBar.SetActive(true);
	}

	// Token: 0x06004D87 RID: 19847 RVA: 0x0017160C File Offset: 0x0016F80C
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_mouseOverBar.SetActive(false);
		this.SetItemColors((!this.m_selected) ? this.m_unselectedColor : this.m_selectedColor);
	}

	// Token: 0x06004D88 RID: 19848 RVA: 0x00171648 File Offset: 0x0016F848
	private void SetItemColors(Color color)
	{
		this.m_iconRenderer.material.SetColor("_Color", color);
		this.m_dropdownText.TextColor = color;
	}

	// Token: 0x040034C1 RID: 13505
	public UberText m_dropdownText;

	// Token: 0x040034C2 RID: 13506
	public MeshRenderer m_iconRenderer;

	// Token: 0x040034C3 RID: 13507
	public GameObject m_mouseOverBar;

	// Token: 0x040034C4 RID: 13508
	public GameObject m_selectedBar;

	// Token: 0x040034C5 RID: 13509
	public Color m_mouseOverColor;

	// Token: 0x040034C6 RID: 13510
	public Color m_selectedColor;

	// Token: 0x040034C7 RID: 13511
	public Color m_unselectedColor;

	// Token: 0x040034C8 RID: 13512
	private bool m_selected;

	// Token: 0x040034C9 RID: 13513
	private Vector2? m_iconMaterialOffset;
}
