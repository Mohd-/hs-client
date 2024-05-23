using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000244 RID: 580
public class BoxLogo : MonoBehaviour
{
	// Token: 0x06002182 RID: 8578 RVA: 0x000A3C34 File Offset: 0x000A1E34
	public Box GetParent()
	{
		return this.m_parent;
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x000A3C3C File Offset: 0x000A1E3C
	public void SetParent(Box parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x000A3C45 File Offset: 0x000A1E45
	public BoxLogoStateInfo GetInfo()
	{
		return this.m_info;
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x000A3C4D File Offset: 0x000A1E4D
	public void SetInfo(BoxLogoStateInfo info)
	{
		this.m_info = info;
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x000A3C58 File Offset: 0x000A1E58
	public bool ChangeState(BoxLogo.State state)
	{
		if (this.m_state == state)
		{
			return false;
		}
		this.m_state = state;
		if (state == BoxLogo.State.SHOWN)
		{
			this.m_parent.OnAnimStarted();
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
				"OnAnimFinished",
				"oncompletetarget",
				this.m_parent.gameObject
			});
			iTween.FadeTo(base.gameObject, args);
		}
		else if (state == BoxLogo.State.HIDDEN)
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
				"OnAnimFinished",
				"oncompletetarget",
				this.m_parent.gameObject
			});
			iTween.FadeTo(base.gameObject, args2);
		}
		return true;
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x000A3E00 File Offset: 0x000A2000
	public void UpdateState(BoxLogo.State state)
	{
		this.m_state = state;
		if (state == BoxLogo.State.SHOWN)
		{
			RenderUtils.SetAlpha(base.gameObject, this.m_info.m_ShownAlpha);
		}
		else if (state == BoxLogo.State.HIDDEN)
		{
			RenderUtils.SetAlpha(base.gameObject, this.m_info.m_HiddenAlpha);
		}
	}

	// Token: 0x040012E9 RID: 4841
	private Box m_parent;

	// Token: 0x040012EA RID: 4842
	private BoxLogoStateInfo m_info;

	// Token: 0x040012EB RID: 4843
	private BoxLogo.State m_state;

	// Token: 0x02000245 RID: 581
	public enum State
	{
		// Token: 0x040012ED RID: 4845
		SHOWN,
		// Token: 0x040012EE RID: 4846
		HIDDEN
	}
}
