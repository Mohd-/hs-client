using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000956 RID: 2390
public class TutorialKeywordTooltip : MonoBehaviour
{
	// Token: 0x0600577D RID: 22397 RVA: 0x001A3130 File Offset: 0x001A1330
	public void Initialize(string keywordName, string keywordText)
	{
		this.SetName(keywordName);
		this.SetBodyText(keywordText);
		base.StartCoroutine(this.WaitAFrameBeforeSendingEvent());
	}

	// Token: 0x0600577E RID: 22398 RVA: 0x001A3158 File Offset: 0x001A1358
	private IEnumerator WaitAFrameBeforeSendingEvent()
	{
		RenderUtils.SetAlpha(base.gameObject, 0f);
		yield return null;
		this.playMakerComponent.SendEvent("Birth");
		iTween.FadeTo(base.gameObject, 1f, 0.5f);
		yield break;
	}

	// Token: 0x0600577F RID: 22399 RVA: 0x001A3173 File Offset: 0x001A1373
	public void SetName(string s)
	{
		this.m_name.Text = s;
	}

	// Token: 0x06005780 RID: 22400 RVA: 0x001A3181 File Offset: 0x001A1381
	public void SetBodyText(string s)
	{
		this.m_body.Text = s;
	}

	// Token: 0x06005781 RID: 22401 RVA: 0x001A3190 File Offset: 0x001A1390
	public float GetHeight()
	{
		return base.GetComponent<Renderer>().bounds.size.z;
	}

	// Token: 0x06005782 RID: 22402 RVA: 0x001A31B8 File Offset: 0x001A13B8
	public float GetWidth()
	{
		return base.GetComponent<Renderer>().bounds.size.x;
	}

	// Token: 0x04003E54 RID: 15956
	public UberText m_name;

	// Token: 0x04003E55 RID: 15957
	public UberText m_body;

	// Token: 0x04003E56 RID: 15958
	public PlayMakerFSM playMakerComponent;
}
