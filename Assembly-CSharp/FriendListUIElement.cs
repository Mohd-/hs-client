using System;
using UnityEngine;

// Token: 0x020004E7 RID: 1255
public class FriendListUIElement : PegUIElement
{
	// Token: 0x06003AE4 RID: 15076 RVA: 0x0011C7C1 File Offset: 0x0011A9C1
	protected override void Awake()
	{
		base.Awake();
		this.UpdateHighlight();
	}

	// Token: 0x06003AE5 RID: 15077 RVA: 0x0011C7CF File Offset: 0x0011A9CF
	public bool IsSelected()
	{
		return this.m_selected;
	}

	// Token: 0x06003AE6 RID: 15078 RVA: 0x0011C7D7 File Offset: 0x0011A9D7
	public void SetSelected(bool enable)
	{
		if (enable == this.m_selected)
		{
			return;
		}
		this.m_selected = enable;
		this.UpdateHighlight();
	}

	// Token: 0x06003AE7 RID: 15079 RVA: 0x0011C7F3 File Offset: 0x0011A9F3
	protected virtual bool ShouldBeHighlighted()
	{
		return this.m_selected || base.GetInteractionState() == PegUIElement.InteractionState.Over;
	}

	// Token: 0x06003AE8 RID: 15080 RVA: 0x0011C80C File Offset: 0x0011AA0C
	protected void UpdateHighlight()
	{
		bool flag = this.ShouldBeHighlighted();
		if (!flag)
		{
			flag = this.ShouldChildBeHighlighted();
		}
		this.UpdateSelfHighlight(flag);
		if (this.m_ParentElement != null)
		{
			this.m_ParentElement.UpdateHighlight();
		}
	}

	// Token: 0x06003AE9 RID: 15081 RVA: 0x0011C850 File Offset: 0x0011AA50
	protected bool ShouldChildBeHighlighted()
	{
		FriendListUIElement[] componentsInChildrenOnly = SceneUtils.GetComponentsInChildrenOnly<FriendListUIElement>(this, true);
		foreach (FriendListUIElement friendListUIElement in componentsInChildrenOnly)
		{
			if (friendListUIElement.ShouldBeHighlighted())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003AEA RID: 15082 RVA: 0x0011C890 File Offset: 0x0011AA90
	protected void UpdateSelfHighlight(bool shouldHighlight)
	{
		if (this.m_Highlight == null)
		{
			return;
		}
		if (this.m_Highlight.activeSelf == shouldHighlight)
		{
			return;
		}
		this.m_Highlight.SetActive(shouldHighlight);
	}

	// Token: 0x06003AEB RID: 15083 RVA: 0x0011C8CD File Offset: 0x0011AACD
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		this.UpdateHighlight();
	}

	// Token: 0x06003AEC RID: 15084 RVA: 0x0011C8D5 File Offset: 0x0011AAD5
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.UpdateHighlight();
	}

	// Token: 0x0400258D RID: 9613
	public FriendListUIElement m_ParentElement;

	// Token: 0x0400258E RID: 9614
	public GameObject m_Highlight;

	// Token: 0x0400258F RID: 9615
	private bool m_selected;
}
