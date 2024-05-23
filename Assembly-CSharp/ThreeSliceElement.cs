using System;
using UnityEngine;

// Token: 0x0200064F RID: 1615
[ExecuteInEditMode]
public class ThreeSliceElement : MonoBehaviour
{
	// Token: 0x0600456E RID: 17774 RVA: 0x0014D3ED File Offset: 0x0014B5ED
	private void Awake()
	{
		if (this.m_middle)
		{
			this.SetInitialValues();
		}
	}

	// Token: 0x0600456F RID: 17775 RVA: 0x0014D408 File Offset: 0x0014B608
	public void UpdateDisplay()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.m_initialMiddleBounds.size == Vector3.zero)
		{
			this.m_initialMiddleBounds = this.m_middle.GetComponent<Renderer>().bounds;
		}
		float num = this.m_width - (this.m_left.GetComponent<Renderer>().bounds.size.x + this.m_right.GetComponent<Renderer>().bounds.size.x);
		switch (this.m_direction)
		{
		case ThreeSliceElement.Direction.X:
		{
			Vector3 scale = TransformUtil.ComputeWorldScale(this.m_middle.transform);
			scale.x = this.m_initialScale.x * num / this.m_initialMiddleBounds.size.x;
			TransformUtil.SetWorldScale(this.m_middle.transform, scale);
			break;
		}
		}
		switch (this.m_pinnedPoint)
		{
		case ThreeSliceElement.PinnedPoint.LEFT:
			this.m_left.transform.localPosition = this.m_pinnedPointOffset;
			TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT, this.m_middleOffset);
			TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT, this.m_rightOffset);
			break;
		case ThreeSliceElement.PinnedPoint.MIDDLE:
			this.m_middle.transform.localPosition = this.m_pinnedPointOffset;
			TransformUtil.SetPoint(this.m_left, Anchor.RIGHT, this.m_middle, Anchor.LEFT, this.m_leftOffset);
			TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT, this.m_rightOffset);
			break;
		case ThreeSliceElement.PinnedPoint.RIGHT:
			this.m_right.transform.localPosition = this.m_pinnedPointOffset;
			TransformUtil.SetPoint(this.m_middle, Anchor.RIGHT, this.m_right, Anchor.LEFT, this.m_middleOffset);
			TransformUtil.SetPoint(this.m_left, Anchor.RIGHT, this.m_middle, Anchor.LEFT, this.m_leftOffset);
			break;
		}
	}

	// Token: 0x06004570 RID: 17776 RVA: 0x0014D627 File Offset: 0x0014B827
	public void SetWidth(float globalWidth)
	{
		this.m_width = globalWidth;
		this.UpdateDisplay();
	}

	// Token: 0x06004571 RID: 17777 RVA: 0x0014D638 File Offset: 0x0014B838
	public void SetMiddleWidth(float globalWidth)
	{
		this.m_width = globalWidth + this.m_left.GetComponent<Renderer>().bounds.size.x + this.m_right.GetComponent<Renderer>().bounds.size.x;
		this.UpdateDisplay();
	}

	// Token: 0x06004572 RID: 17778 RVA: 0x0014D694 File Offset: 0x0014B894
	public Vector3 GetMiddleSize()
	{
		return this.m_middle.GetComponent<Renderer>().bounds.size;
	}

	// Token: 0x06004573 RID: 17779 RVA: 0x0014D6B9 File Offset: 0x0014B8B9
	public Vector3 GetSize()
	{
		return this.GetSize(true);
	}

	// Token: 0x06004574 RID: 17780 RVA: 0x0014D6C4 File Offset: 0x0014B8C4
	public Vector3 GetSize(bool zIsHeight)
	{
		Vector3 size = this.m_left.GetComponent<Renderer>().bounds.size;
		Vector3 size2 = this.m_middle.GetComponent<Renderer>().bounds.size;
		Vector3 size3 = this.m_right.GetComponent<Renderer>().bounds.size;
		float num = size.x + size3.x + size2.x;
		float num2 = Mathf.Max(Mathf.Max(size.z, size2.z), size3.z);
		float num3 = Mathf.Max(Mathf.Max(size.y, size2.y), size3.y);
		if (zIsHeight)
		{
			return new Vector3(num, num2, num3);
		}
		return new Vector3(num, num3, num2);
	}

	// Token: 0x06004575 RID: 17781 RVA: 0x0014D794 File Offset: 0x0014B994
	public void SetInitialValues()
	{
		this.m_initialMiddleBounds = this.m_middle.GetComponent<Renderer>().bounds;
		this.m_initialScale = this.m_middle.transform.lossyScale;
		this.m_width = this.m_middle.GetComponent<Renderer>().bounds.size.x + this.m_left.GetComponent<Renderer>().bounds.size.x + this.m_right.GetComponent<Renderer>().bounds.size.x;
	}

	// Token: 0x04002C52 RID: 11346
	public GameObject m_left;

	// Token: 0x04002C53 RID: 11347
	public GameObject m_middle;

	// Token: 0x04002C54 RID: 11348
	public GameObject m_right;

	// Token: 0x04002C55 RID: 11349
	public ThreeSliceElement.PinnedPoint m_pinnedPoint;

	// Token: 0x04002C56 RID: 11350
	public Vector3 m_pinnedPointOffset;

	// Token: 0x04002C57 RID: 11351
	public ThreeSliceElement.Direction m_direction;

	// Token: 0x04002C58 RID: 11352
	public float m_width;

	// Token: 0x04002C59 RID: 11353
	public float m_middleScale = 1f;

	// Token: 0x04002C5A RID: 11354
	public Vector3 m_leftOffset;

	// Token: 0x04002C5B RID: 11355
	public Vector3 m_middleOffset;

	// Token: 0x04002C5C RID: 11356
	public Vector3 m_rightOffset;

	// Token: 0x04002C5D RID: 11357
	private Bounds m_initialMiddleBounds;

	// Token: 0x04002C5E RID: 11358
	private Vector3 m_initialScale = Vector3.zero;

	// Token: 0x02000650 RID: 1616
	public enum PinnedPoint
	{
		// Token: 0x04002C60 RID: 11360
		LEFT,
		// Token: 0x04002C61 RID: 11361
		MIDDLE,
		// Token: 0x04002C62 RID: 11362
		RIGHT,
		// Token: 0x04002C63 RID: 11363
		TOP,
		// Token: 0x04002C64 RID: 11364
		BOTTOM
	}

	// Token: 0x02000651 RID: 1617
	public enum Direction
	{
		// Token: 0x04002C66 RID: 11366
		X,
		// Token: 0x04002C67 RID: 11367
		Y,
		// Token: 0x04002C68 RID: 11368
		Z
	}
}
