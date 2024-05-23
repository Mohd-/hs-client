using System;

// Token: 0x0200025D RID: 605
public class QuestLogButton : PegUIElement
{
	// Token: 0x06002236 RID: 8758 RVA: 0x000A83D5 File Offset: 0x000A65D5
	private void Start()
	{
		this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnButtonOver));
		this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnButtonOut));
		SoundManager.Get().Load("quest_log_button_mouse_over");
	}

	// Token: 0x06002237 RID: 8759 RVA: 0x000A8410 File Offset: 0x000A6610
	private void OnButtonOver(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("quest_log_button_mouse_over", base.gameObject);
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
		TooltipZone component = base.GetComponent<TooltipZone>();
		if (component == null)
		{
			return;
		}
		component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_QUESTLOG_HEADLINE"), GameStrings.Get("GLUE_TOOLTIP_BUTTON_QUESTLOG_DESC"));
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x000A8470 File Offset: 0x000A6670
	private void OnButtonOut(UIEvent e)
	{
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		TooltipZone component = base.GetComponent<TooltipZone>();
		if (component != null)
		{
			component.HideTooltip();
		}
	}

	// Token: 0x04001392 RID: 5010
	public HighlightState m_highlight;
}
