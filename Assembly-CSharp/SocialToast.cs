using System;
using UnityEngine;

// Token: 0x0200064E RID: 1614
public class SocialToast : MonoBehaviour
{
	// Token: 0x0600456C RID: 17772 RVA: 0x0014D388 File Offset: 0x0014B588
	public void SetText(string text)
	{
		this.m_text.Text = text;
		ThreeSliceElement component = base.GetComponent<ThreeSliceElement>();
		component.SetMiddleWidth(this.m_text.GetTextWorldSpaceBounds().size.x * 0.95f);
	}

	// Token: 0x04002C50 RID: 11344
	private const float FUDGE_FACTOR = 0.95f;

	// Token: 0x04002C51 RID: 11345
	public UberText m_text;
}
