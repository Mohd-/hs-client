using System;
using UnityEngine;

// Token: 0x0200025C RID: 604
public class StoreButton : PegUIElement
{
	// Token: 0x0600222E RID: 8750 RVA: 0x000A81B8 File Offset: 0x000A63B8
	protected override void Awake()
	{
		base.Awake();
		this.m_storeText.Text = GameStrings.Get("GLUE_STORE_OPEN_BUTTON_TEXT");
		this.m_storeClosedText.Text = GameStrings.Get("GLUE_STORE_CLOSED_BUTTON_TEXT");
	}

	// Token: 0x0600222F RID: 8751 RVA: 0x000A81F8 File Offset: 0x000A63F8
	private void Start()
	{
		this.m_storeClosed.SetActive(!StoreManager.Get().IsOpen());
		StoreManager.Get().RegisterStatusChangedListener(new StoreManager.StatusChangedCallback(this.OnStoreStatusChanged));
		this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnButtonOver));
		this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnButtonOut));
		SoundManager.Get().Load("store_button_mouse_over");
	}

	// Token: 0x06002230 RID: 8752 RVA: 0x000A826C File Offset: 0x000A646C
	public void Unload()
	{
		this.SetEnabled(false);
		StoreManager.Get().RemoveStatusChangedListener(new StoreManager.StatusChangedCallback(this.OnStoreStatusChanged));
	}

	// Token: 0x06002231 RID: 8753 RVA: 0x000A828C File Offset: 0x000A648C
	public bool IsVisualClosed()
	{
		return this.m_storeClosed != null && this.m_storeClosed.activeInHierarchy;
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x000A82B0 File Offset: 0x000A64B0
	private void OnButtonOver(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("store_button_mouse_over", base.gameObject);
		if (this.m_highlightState != null)
		{
			this.m_highlightState.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
		}
		if (this.m_highlight != null)
		{
			this.m_highlight.SetActive(true);
		}
		TooltipZone component = base.GetComponent<TooltipZone>();
		if (component == null)
		{
			return;
		}
		component.ShowBoxTooltip(GameStrings.Get("GLUE_TOOLTIP_BUTTON_STORE_HEADLINE"), GameStrings.Get("GLUE_TOOLTIP_BUTTON_STORE_DESC"));
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x000A833C File Offset: 0x000A653C
	private void OnButtonOut(UIEvent e)
	{
		if (this.m_highlightState != null)
		{
			this.m_highlightState.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		}
		if (this.m_highlight != null)
		{
			this.m_highlight.SetActive(false);
		}
		TooltipZone component = base.GetComponent<TooltipZone>();
		if (component != null)
		{
			component.HideTooltip();
		}
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x000A83A0 File Offset: 0x000A65A0
	private void OnStoreStatusChanged(bool isOpen, object userData)
	{
		if (this.m_storeClosed != null)
		{
			this.m_storeClosed.SetActive(!isOpen);
		}
	}

	// Token: 0x0400138D RID: 5005
	public GameObject m_storeClosed;

	// Token: 0x0400138E RID: 5006
	public UberText m_storeClosedText;

	// Token: 0x0400138F RID: 5007
	public UberText m_storeText;

	// Token: 0x04001390 RID: 5008
	public HighlightState m_highlightState;

	// Token: 0x04001391 RID: 5009
	public GameObject m_highlight;
}
