using System;
using UnityEngine;

// Token: 0x0200064D RID: 1613
public class DropdownMenuItem : PegUIElement
{
	// Token: 0x06004566 RID: 17766 RVA: 0x0014D31E File Offset: 0x0014B51E
	public object GetValue()
	{
		return this.m_value;
	}

	// Token: 0x06004567 RID: 17767 RVA: 0x0014D326 File Offset: 0x0014B526
	public void SetValue(object val, string text)
	{
		this.m_value = val;
		this.m_text.Text = text;
	}

	// Token: 0x06004568 RID: 17768 RVA: 0x0014D33B File Offset: 0x0014B53B
	public void SetSelected(bool selected)
	{
		if (this.m_selected == null)
		{
			return;
		}
		this.m_selected.SetActive(selected);
	}

	// Token: 0x06004569 RID: 17769 RVA: 0x0014D35B File Offset: 0x0014B55B
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		this.m_text.TextColor = Color.white;
	}

	// Token: 0x0600456A RID: 17770 RVA: 0x0014D36D File Offset: 0x0014B56D
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_text.TextColor = Color.black;
	}

	// Token: 0x04002C4D RID: 11341
	public GameObject m_selected;

	// Token: 0x04002C4E RID: 11342
	public UberText m_text;

	// Token: 0x04002C4F RID: 11343
	private object m_value;
}
