using System;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class PlayButton : PegUIElement
{
	// Token: 0x0600209C RID: 8348 RVA: 0x0009F5E8 File Offset: 0x0009D7E8
	protected override void Awake()
	{
		base.Awake();
		SoundManager.Get().Load("play_button_mouseover");
		this.m_playButtonHighlightState = base.gameObject.GetComponentInChildren<HighlightState>();
		base.SetOriginalLocalPosition();
	}

	// Token: 0x0600209D RID: 8349 RVA: 0x0009F622 File Offset: 0x0009D822
	protected void Start()
	{
		this.m_isStarted = true;
		if (base.IsEnabled())
		{
			this.Enable();
		}
		else
		{
			this.Disable();
		}
	}

	// Token: 0x0600209E RID: 8350 RVA: 0x0009F648 File Offset: 0x0009D848
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		SoundManager.Get().LoadAndPlay("play_button_mouseover", base.gameObject);
		if (this.m_playButtonHighlightState != null)
		{
			this.m_playButtonHighlightState.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_MOUSE_OVER);
		}
	}

	// Token: 0x0600209F RID: 8351 RVA: 0x0009F68C File Offset: 0x0009D88C
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			base.GetOriginalLocalPosition(),
			"isLocal",
			true,
			"time",
			0.25f
		}));
		if (this.m_playButtonHighlightState != null)
		{
			this.m_playButtonHighlightState.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
		}
	}

	// Token: 0x060020A0 RID: 8352 RVA: 0x0009F70A File Offset: 0x0009D90A
	public void ChangeHighlightState(ActorStateType stateType)
	{
		if (this.m_playButtonHighlightState == null)
		{
			return;
		}
		this.m_playButtonHighlightState.ChangeState(stateType);
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x0009F72C File Offset: 0x0009D92C
	public void Disable()
	{
		this.SetEnabled(false);
		if (!this.m_isStarted)
		{
			return;
		}
		if (this.m_playButtonHighlightState != null)
		{
			base.GetComponent<PlayMakerFSM>().SendEvent("Cancel");
			this.m_playButtonHighlightState.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		}
	}

	// Token: 0x060020A2 RID: 8354 RVA: 0x0009F77C File Offset: 0x0009D97C
	public void Enable()
	{
		this.SetEnabled(true);
		this.m_newPlayButtonText.UpdateNow();
		if (!this.m_isStarted)
		{
			return;
		}
		if (this.m_newPlayButtonText != null)
		{
			this.m_newPlayButtonText.TextAlpha = 1f;
		}
		if (this.m_playButtonHighlightState != null)
		{
			base.GetComponent<PlayMakerFSM>().SendEvent("Birth");
			if (this.m_playButtonHighlightState != null)
			{
				this.m_playButtonHighlightState.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
		}
	}

	// Token: 0x060020A3 RID: 8355 RVA: 0x0009F808 File Offset: 0x0009DA08
	protected override void OnPress()
	{
		Vector3 originalLocalPosition = base.GetOriginalLocalPosition();
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			originalLocalPosition + this.m_pressMovement,
			"isLocal",
			true,
			"time",
			0.25f
		}));
		this.ChangeHighlightState(ActorStateType.HIGHLIGHT_OFF);
		SoundManager.Get().LoadAndPlay("collection_manager_select_hero");
	}

	// Token: 0x060020A4 RID: 8356 RVA: 0x0009F88C File Offset: 0x0009DA8C
	protected override void OnRelease()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			base.GetOriginalLocalPosition(),
			"isLocal",
			true,
			"time",
			0.25f
		}));
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x0009F8EB File Offset: 0x0009DAEB
	public void SetText(string newText)
	{
		if (this.m_newPlayButtonText != null)
		{
			this.m_newPlayButtonText.Text = newText;
		}
	}

	// Token: 0x040011D6 RID: 4566
	public Vector3 m_pressMovement = new Vector3(0f, -0.9f, 0f);

	// Token: 0x040011D7 RID: 4567
	private HighlightState m_playButtonHighlightState;

	// Token: 0x040011D8 RID: 4568
	private bool m_isStarted;

	// Token: 0x040011D9 RID: 4569
	public UberText m_newPlayButtonText;
}
