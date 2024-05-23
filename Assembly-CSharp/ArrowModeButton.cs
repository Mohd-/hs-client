using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000A82 RID: 2690
public class ArrowModeButton : PegUIElement
{
	// Token: 0x06005D8F RID: 23951 RVA: 0x001C0DB4 File Offset: 0x001BEFB4
	protected override void Awake()
	{
		base.Awake();
		SoundManager.Get().Load("Small_Mouseover");
		SoundManager.Get().Load("deck_select_button_press");
	}

	// Token: 0x06005D90 RID: 23952 RVA: 0x001C0DE8 File Offset: 0x001BEFE8
	public void Activate(bool activate)
	{
		if (activate == base.IsEnabled())
		{
			return;
		}
		this.SetEnabled(activate);
		if (!activate && this.m_highlight != null)
		{
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		}
		this.m_numFlips++;
		iTween tweenForObject = iTweenManager.Get().GetTweenForObject(base.gameObject);
		if (tweenForObject != null)
		{
			return;
		}
		this.Flip();
	}

	// Token: 0x06005D91 RID: 23953 RVA: 0x001C0E5C File Offset: 0x001BF05C
	public void ActivateHighlight(bool highlightOn)
	{
		if (this.m_highlight == null)
		{
			return;
		}
		this.m_isHighlighted = highlightOn;
		ActorStateType stateType = (!this.m_isHighlighted) ? ActorStateType.HIGHLIGHT_OFF : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE;
		this.m_highlight.ChangeState(stateType);
	}

	// Token: 0x06005D92 RID: 23954 RVA: 0x001C0EA4 File Offset: 0x001BF0A4
	protected override void OnRelease()
	{
		if (this.m_highlight == null)
		{
			return;
		}
		SoundManager.Get().LoadAndPlay("deck_select_button_press");
		this.m_isHighlighted = false;
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
	}

	// Token: 0x06005D93 RID: 23955 RVA: 0x001C0EE8 File Offset: 0x001BF0E8
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		SoundManager.Get().LoadAndPlay("Small_Mouseover");
		if (this.m_highlight == null)
		{
			return;
		}
		ActorStateType stateType = (!this.m_isHighlighted) ? ActorStateType.HIGHLIGHT_MOUSE_OVER : ActorStateType.HIGHLIGHT_PRIMARY_MOUSE_OVER;
		this.m_highlight.ChangeState(stateType);
	}

	// Token: 0x06005D94 RID: 23956 RVA: 0x001C0F38 File Offset: 0x001BF138
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		if (this.m_highlight == null)
		{
			return;
		}
		ActorStateType stateType = (!this.m_isHighlighted) ? ActorStateType.HIGHLIGHT_OFF : ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE;
		this.m_highlight.ChangeState(stateType);
	}

	// Token: 0x06005D95 RID: 23957 RVA: 0x001C0F7C File Offset: 0x001BF17C
	private void Flip()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			new Vector3(180f, 0f, 0f),
			"time",
			0.5f,
			"easeType",
			iTween.EaseType.easeOutElastic,
			"space",
			1,
			"oncomplete",
			"OnFlipComplete",
			"oncompletetarget",
			base.gameObject
		});
		iTween.RotateAdd(base.gameObject, args);
	}

	// Token: 0x06005D96 RID: 23958 RVA: 0x001C1024 File Offset: 0x001BF224
	private void OnFlipComplete()
	{
		this.m_numFlips--;
		if (this.m_numFlips > 0)
		{
			this.Flip();
		}
	}

	// Token: 0x0400455C RID: 17756
	public HighlightState m_highlight;

	// Token: 0x0400455D RID: 17757
	private int m_numFlips;

	// Token: 0x0400455E RID: 17758
	private bool m_isHighlighted;
}
