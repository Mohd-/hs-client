using System;
using System.Collections;

// Token: 0x02000246 RID: 582
public class BoxStartButton : PegUIElement
{
	// Token: 0x06002189 RID: 8585 RVA: 0x000A3E5A File Offset: 0x000A205A
	public Box GetParent()
	{
		return this.m_parent;
	}

	// Token: 0x0600218A RID: 8586 RVA: 0x000A3E62 File Offset: 0x000A2062
	public void SetParent(Box parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x000A3E6B File Offset: 0x000A206B
	public BoxStartButtonStateInfo GetInfo()
	{
		return this.m_info;
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x000A3E73 File Offset: 0x000A2073
	public void SetInfo(BoxStartButtonStateInfo info)
	{
		this.m_info = info;
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x000A3E7C File Offset: 0x000A207C
	public string GetText()
	{
		return this.m_Text.Text;
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x000A3E89 File Offset: 0x000A2089
	public void SetText(string text)
	{
		this.m_Text.Text = text;
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x000A3E98 File Offset: 0x000A2098
	public bool ChangeState(BoxStartButton.State state)
	{
		if (this.m_state == state)
		{
			return false;
		}
		this.m_state = state;
		if (state == BoxStartButton.State.SHOWN)
		{
			this.m_parent.OnAnimStarted();
			base.gameObject.SetActive(true);
			Hashtable args = iTween.Hash(new object[]
			{
				"amount",
				this.m_info.m_ShownAlpha,
				"delay",
				this.m_info.m_ShownDelaySec,
				"time",
				this.m_info.m_ShownFadeSec,
				"easeType",
				this.m_info.m_ShownFadeEaseType,
				"oncomplete",
				"OnShownAnimFinished",
				"oncompletetarget",
				base.gameObject
			});
			iTween.FadeTo(base.gameObject, args);
		}
		else if (state == BoxStartButton.State.HIDDEN)
		{
			this.m_parent.OnAnimStarted();
			Hashtable args2 = iTween.Hash(new object[]
			{
				"amount",
				this.m_info.m_HiddenAlpha,
				"delay",
				this.m_info.m_HiddenDelaySec,
				"time",
				this.m_info.m_HiddenFadeSec,
				"easeType",
				this.m_info.m_HiddenFadeEaseType,
				"oncomplete",
				"OnHiddenAnimFinished",
				"oncompletetarget",
				base.gameObject
			});
			iTween.FadeTo(base.gameObject, args2);
		}
		return true;
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x000A4044 File Offset: 0x000A2244
	public void UpdateState(BoxStartButton.State state)
	{
		this.m_state = state;
		if (state == BoxStartButton.State.SHOWN)
		{
			RenderUtils.SetAlpha(base.gameObject, this.m_info.m_ShownAlpha);
			base.gameObject.SetActive(true);
		}
		else if (state == BoxStartButton.State.HIDDEN)
		{
			RenderUtils.SetAlpha(base.gameObject, this.m_info.m_HiddenAlpha);
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x000A40AE File Offset: 0x000A22AE
	private void OnShownAnimFinished()
	{
		this.m_parent.OnAnimFinished();
	}

	// Token: 0x06002192 RID: 8594 RVA: 0x000A40BB File Offset: 0x000A22BB
	private void OnHiddenAnimFinished()
	{
		base.gameObject.SetActive(false);
		this.m_parent.OnAnimFinished();
	}

	// Token: 0x040012EF RID: 4847
	public UberText m_Text;

	// Token: 0x040012F0 RID: 4848
	private Box m_parent;

	// Token: 0x040012F1 RID: 4849
	private BoxStartButtonStateInfo m_info;

	// Token: 0x040012F2 RID: 4850
	private BoxStartButton.State m_state;

	// Token: 0x02000247 RID: 583
	public enum State
	{
		// Token: 0x040012F4 RID: 4852
		SHOWN,
		// Token: 0x040012F5 RID: 4853
		HIDDEN
	}
}
