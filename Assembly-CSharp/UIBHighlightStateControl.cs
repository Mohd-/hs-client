using System;
using UnityEngine;

// Token: 0x020001B0 RID: 432
[CustomEditClass]
public class UIBHighlightStateControl : MonoBehaviour
{
	// Token: 0x06001C54 RID: 7252 RVA: 0x000854B8 File Offset: 0x000836B8
	private void Awake()
	{
		PegUIElement component = base.gameObject.GetComponent<PegUIElement>();
		if (component != null)
		{
			component.AddEventListener(UIEventType.ROLLOVER, delegate(UIEvent e)
			{
				if (this.m_EnableResponse)
				{
					this.OnRollOver();
				}
			});
			component.AddEventListener(UIEventType.ROLLOUT, delegate(UIEvent e)
			{
				if (this.m_EnableResponse)
				{
					this.OnRollOut();
				}
			});
			component.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				if (this.m_EnableResponse)
				{
					this.OnRelease();
				}
			});
		}
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x0008551C File Offset: 0x0008371C
	public void Select(bool selected, bool primary = false)
	{
		if (selected)
		{
			this.m_HighlightState.ChangeState((!primary) ? this.m_SecondarySelectedStateType : this.m_PrimarySelectedStateType);
		}
		else if (this.m_MouseOver)
		{
			this.m_HighlightState.ChangeState(this.m_MouseOverStateType);
		}
		else
		{
			this.m_HighlightState.ChangeState(ActorStateType.NONE);
		}
	}

	// Token: 0x06001C56 RID: 7254 RVA: 0x00085586 File Offset: 0x00083786
	public bool IsReady()
	{
		return this.m_HighlightState.IsReady();
	}

	// Token: 0x06001C57 RID: 7255 RVA: 0x00085594 File Offset: 0x00083794
	private void OnRollOver()
	{
		if (this.m_UseMouseOver)
		{
			this.m_MouseOver = true;
			this.m_HighlightState.ChangeState(this.m_MouseOverStateType);
		}
	}

	// Token: 0x06001C58 RID: 7256 RVA: 0x000855C8 File Offset: 0x000837C8
	private void OnRollOut()
	{
		if (this.m_UseMouseOver)
		{
			this.m_MouseOver = false;
			if (!this.m_AllowSelection)
			{
				this.m_HighlightState.ChangeState(ActorStateType.NONE);
			}
		}
	}

	// Token: 0x06001C59 RID: 7257 RVA: 0x00085600 File Offset: 0x00083800
	private void OnRelease()
	{
		if (this.m_AllowSelection)
		{
			this.Select(true, false);
			return;
		}
		if (this.m_MouseOver)
		{
			this.m_HighlightState.ChangeState(this.m_MouseOverStateType);
		}
		else
		{
			this.m_HighlightState.ChangeState(ActorStateType.NONE);
		}
	}

	// Token: 0x04000ED8 RID: 3800
	[CustomEditField(Sections = "Highlight State Reference")]
	public HighlightState m_HighlightState;

	// Token: 0x04000ED9 RID: 3801
	[CustomEditField(Sections = "Highlight State Type")]
	public ActorStateType m_MouseOverStateType = ActorStateType.HIGHLIGHT_MOUSE_OVER;

	// Token: 0x04000EDA RID: 3802
	[CustomEditField(Sections = "Highlight State Type")]
	public ActorStateType m_PrimarySelectedStateType = ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE;

	// Token: 0x04000EDB RID: 3803
	[CustomEditField(Sections = "Highlight State Type")]
	public ActorStateType m_SecondarySelectedStateType = ActorStateType.HIGHLIGHT_SECONDARY_ACTIVE;

	// Token: 0x04000EDC RID: 3804
	[CustomEditField(Sections = "Behavior Settings")]
	public bool m_UseMouseOver;

	// Token: 0x04000EDD RID: 3805
	[CustomEditField(Sections = "Behavior Settings")]
	public bool m_AllowSelection;

	// Token: 0x04000EDE RID: 3806
	[CustomEditField(Sections = "Behavior Settings")]
	public bool m_EnableResponse = true;

	// Token: 0x04000EDF RID: 3807
	private PegUIElement m_PegUIElement;

	// Token: 0x04000EE0 RID: 3808
	private bool m_MouseOver;
}
