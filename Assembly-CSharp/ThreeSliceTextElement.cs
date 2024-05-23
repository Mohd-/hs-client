using System;
using UnityEngine;

// Token: 0x02000F64 RID: 3940
public class ThreeSliceTextElement : MonoBehaviour
{
	// Token: 0x060074FB RID: 29947 RVA: 0x00228AC8 File Offset: 0x00226CC8
	public void SetText(string text)
	{
		this.m_text.Text = text;
		this.m_text.UpdateNow();
		this.Resize();
	}

	// Token: 0x060074FC RID: 29948 RVA: 0x00228AE7 File Offset: 0x00226CE7
	public void Resize()
	{
		this.m_threeSlice.SetMiddleWidth(this.GetTextWidth());
	}

	// Token: 0x060074FD RID: 29949 RVA: 0x00228AFC File Offset: 0x00226CFC
	private float GetTextWidth()
	{
		return this.m_text.GetTextBounds().size.x;
	}

	// Token: 0x04005F8E RID: 24462
	public UberText m_text;

	// Token: 0x04005F8F RID: 24463
	public ThreeSliceElement m_threeSlice;
}
