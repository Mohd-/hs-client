using System;

// Token: 0x02000F6F RID: 3951
public class UIHeader : ThreeSliceElement
{
	// Token: 0x0600752E RID: 29998 RVA: 0x00229638 File Offset: 0x00227838
	public void SetText(string t)
	{
		this.m_headerUberText.Text = t;
		base.SetMiddleWidth(this.m_headerUberText.GetTextWorldSpaceBounds().size.x);
	}

	// Token: 0x04005FAF RID: 24495
	public UberText m_headerUberText;
}
