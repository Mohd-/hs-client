using System;
using UnityEngine;

// Token: 0x020003EF RID: 1007
[Serializable]
public class CanvasAnchors
{
	// Token: 0x06003418 RID: 13336 RVA: 0x001043B4 File Offset: 0x001025B4
	public Transform GetAnchor(CanvasAnchor type)
	{
		if (type == CanvasAnchor.CENTER)
		{
			return this.m_Center;
		}
		if (type == CanvasAnchor.LEFT)
		{
			return this.m_Left;
		}
		if (type == CanvasAnchor.RIGHT)
		{
			return this.m_Right;
		}
		if (type == CanvasAnchor.BOTTOM)
		{
			return this.m_Bottom;
		}
		if (type == CanvasAnchor.TOP)
		{
			return this.m_Top;
		}
		if (type == CanvasAnchor.BOTTOM_LEFT)
		{
			return this.m_BottomLeft;
		}
		if (type == CanvasAnchor.BOTTOM_RIGHT)
		{
			return this.m_BottomRight;
		}
		if (type == CanvasAnchor.TOP_LEFT)
		{
			return this.m_TopLeft;
		}
		if (type == CanvasAnchor.TOP_RIGHT)
		{
			return this.m_TopRight;
		}
		return this.m_Center;
	}

	// Token: 0x06003419 RID: 13337 RVA: 0x00104444 File Offset: 0x00102644
	public void WillReset()
	{
		foreach (object obj in this.m_Center)
		{
			Transform transform = (Transform)obj;
			Object.Destroy(transform.gameObject);
		}
		foreach (object obj2 in this.m_Left)
		{
			Transform transform2 = (Transform)obj2;
			Object.Destroy(transform2.gameObject);
		}
		foreach (object obj3 in this.m_Right)
		{
			Transform transform3 = (Transform)obj3;
			Object.Destroy(transform3.gameObject);
		}
		foreach (object obj4 in this.m_Bottom)
		{
			Transform transform4 = (Transform)obj4;
			Object.Destroy(transform4.gameObject);
		}
		foreach (object obj5 in this.m_Top)
		{
			Transform transform5 = (Transform)obj5;
			Object.Destroy(transform5.gameObject);
		}
		foreach (object obj6 in this.m_BottomLeft)
		{
			Transform transform6 = (Transform)obj6;
			Object.Destroy(transform6.gameObject);
		}
		foreach (object obj7 in this.m_BottomRight)
		{
			Transform transform7 = (Transform)obj7;
			Object.Destroy(transform7.gameObject);
		}
		foreach (object obj8 in this.m_TopLeft)
		{
			Transform transform8 = (Transform)obj8;
			Object.Destroy(transform8.gameObject);
		}
		foreach (object obj9 in this.m_TopRight)
		{
			Transform transform9 = (Transform)obj9;
			Object.Destroy(transform9.gameObject);
		}
	}

	// Token: 0x04002035 RID: 8245
	public Transform m_Center;

	// Token: 0x04002036 RID: 8246
	public Transform m_Left;

	// Token: 0x04002037 RID: 8247
	public Transform m_Right;

	// Token: 0x04002038 RID: 8248
	public Transform m_Bottom;

	// Token: 0x04002039 RID: 8249
	public Transform m_Top;

	// Token: 0x0400203A RID: 8250
	public Transform m_BottomLeft;

	// Token: 0x0400203B RID: 8251
	public Transform m_BottomRight;

	// Token: 0x0400203C RID: 8252
	public Transform m_TopLeft;

	// Token: 0x0400203D RID: 8253
	public Transform m_TopRight;
}
