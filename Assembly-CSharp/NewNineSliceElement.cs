using System;
using UnityEngine;

// Token: 0x02000F56 RID: 3926
[ExecuteInEditMode]
public class NewNineSliceElement : MonoBehaviour
{
	// Token: 0x060074C4 RID: 29892 RVA: 0x00227388 File Offset: 0x00225588
	public virtual void SetSize(float width, float height)
	{
		if (this.m_mode == NewNineSliceElement.Mode.UseSize)
		{
			this.m_size = new Vector2(width, height);
			Vector3 size = this.m_topLeft.GetComponent<Renderer>().bounds.size;
			Vector3 size2 = this.m_bottomLeft.GetComponent<Renderer>().bounds.size;
			width = Mathf.Max(width - (size.x + size2.x), 1f);
			height = Mathf.Max(height - (size.y + size2.y), 1f);
			this.SetPieceWidth(this.m_middle, width);
			this.SetPieceHeight(this.m_middle, height);
			this.SetPieceWidth(this.m_top, width);
			this.SetPieceWidth(this.m_bottom, width);
			this.SetPieceHeight(this.m_left, height);
			this.SetPieceHeight(this.m_right, height);
		}
		else
		{
			TransformUtil.SetLocalScaleXZ(this.m_middle, new Vector2(width, height));
			TransformUtil.SetLocalScaleX(this.m_top, width);
			TransformUtil.SetLocalScaleX(this.m_bottom, width);
			TransformUtil.SetLocalScaleZ(this.m_left, height);
			TransformUtil.SetLocalScaleZ(this.m_right, height);
		}
		Vector3 vector;
		Vector3 vector2;
		Vector3 vector3;
		Vector3 vector4;
		Vector3 vector5;
		if (this.m_planeAxis == NewNineSliceElement.PlaneAxis.XZ)
		{
			vector..ctor(0f, 0f, 1f);
			vector2..ctor(0f, 0f, 0f);
			vector3..ctor(0f, 0f, 0.5f);
			vector4..ctor(1f, 0f, 0.5f);
			vector5..ctor(0.5f, 0f, 0.5f);
		}
		else
		{
			vector..ctor(0f, 1f, 0f);
			vector2..ctor(0f, 0f, 0f);
			vector3..ctor(0f, 0.5f, 0f);
			vector4..ctor(1f, 0.5f, 0f);
			vector5..ctor(0.5f, 0.5f, 0f);
		}
		switch (this.m_pinnedPoint)
		{
		case NewNineSliceElement.PinnedPoint.TOPLEFT:
			TransformUtil.SetPoint(this.m_topLeft, Anchor.TOP_LEFT, this.m_anchorBone, Anchor.TOP_LEFT);
			TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_left, vector, this.m_topLeft, vector2);
			TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomLeft, vector, this.m_left, vector2);
			TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
			break;
		case NewNineSliceElement.PinnedPoint.TOP:
			TransformUtil.SetPoint(this.m_top, Anchor.TOP, this.m_anchorBone, Anchor.TOP);
			TransformUtil.SetPoint(this.m_topLeft, Anchor.RIGHT, this.m_top, Anchor.LEFT);
			TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_left, vector, this.m_topLeft, vector2);
			TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomLeft, vector, this.m_left, vector2);
			TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
			break;
		case NewNineSliceElement.PinnedPoint.TOPRIGHT:
			TransformUtil.SetPoint(this.m_topRight, Anchor.TOP_RIGHT, this.m_anchorBone, Anchor.TOP_RIGHT);
			TransformUtil.SetPoint(this.m_top, Anchor.RIGHT, this.m_topRight, Anchor.LEFT);
			TransformUtil.SetPoint(this.m_topLeft, Anchor.RIGHT, this.m_top, Anchor.LEFT);
			TransformUtil.SetPoint(this.m_left, vector, this.m_topLeft, vector2);
			TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomLeft, vector, this.m_left, vector2);
			TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
			break;
		case NewNineSliceElement.PinnedPoint.LEFT:
			TransformUtil.SetPoint(this.m_left, vector3, this.m_anchorBone, vector3);
			TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topLeft, vector2, this.m_left, vector);
			TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomLeft, vector, this.m_left, vector2);
			TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
			break;
		case NewNineSliceElement.PinnedPoint.MIDDLE:
			TransformUtil.SetPoint(this.m_middle, vector5, this.m_anchorBone, vector5);
			TransformUtil.SetPoint(this.m_left, Anchor.RIGHT, this.m_middle, Anchor.LEFT);
			TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topLeft, vector2, this.m_left, vector);
			TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomLeft, vector, this.m_left, vector2);
			TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
			break;
		case NewNineSliceElement.PinnedPoint.RIGHT:
			TransformUtil.SetPoint(this.m_right, vector4, this.m_anchorBone, vector4);
			TransformUtil.SetPoint(this.m_middle, Anchor.RIGHT, this.m_right, Anchor.LEFT);
			TransformUtil.SetPoint(this.m_left, Anchor.RIGHT, this.m_middle, Anchor.LEFT);
			TransformUtil.SetPoint(this.m_topLeft, vector2, this.m_left, vector);
			TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomLeft, vector, this.m_left, vector2);
			TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
			break;
		case NewNineSliceElement.PinnedPoint.BOTTOMLEFT:
			TransformUtil.SetPoint(this.m_bottomLeft, Anchor.BOTTOM_LEFT, this.m_anchorBone, Anchor.BOTTOM_LEFT);
			TransformUtil.SetPoint(this.m_bottom, Anchor.LEFT, this.m_bottomLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_left, vector2, this.m_bottomLeft, vector);
			TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topLeft, vector2, this.m_left, vector);
			TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
			break;
		case NewNineSliceElement.PinnedPoint.BOTTOM:
			TransformUtil.SetPoint(this.m_bottom, Anchor.BOTTOM, this.m_anchorBone, Anchor.BOTTOM);
			TransformUtil.SetPoint(this.m_bottomLeft, Anchor.RIGHT, this.m_bottom, Anchor.LEFT);
			TransformUtil.SetPoint(this.m_bottomRight, Anchor.LEFT, this.m_bottom, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_left, vector2, this.m_bottomLeft, vector);
			TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topLeft, vector2, this.m_left, vector);
			TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
			break;
		case NewNineSliceElement.PinnedPoint.BOTTOMRIGHT:
			TransformUtil.SetPoint(this.m_bottomRight, Anchor.BOTTOM_RIGHT, this.m_anchorBone, Anchor.BOTTOM_RIGHT);
			TransformUtil.SetPoint(this.m_bottom, Anchor.RIGHT, this.m_bottomRight, Anchor.LEFT);
			TransformUtil.SetPoint(this.m_bottomLeft, Anchor.RIGHT, this.m_bottom, Anchor.LEFT);
			TransformUtil.SetPoint(this.m_left, vector2, this.m_bottomLeft, vector);
			TransformUtil.SetPoint(this.m_middle, Anchor.LEFT, this.m_left, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_right, Anchor.LEFT, this.m_middle, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topLeft, vector2, this.m_left, vector);
			TransformUtil.SetPoint(this.m_top, Anchor.LEFT, this.m_topLeft, Anchor.RIGHT);
			TransformUtil.SetPoint(this.m_topRight, Anchor.LEFT, this.m_top, Anchor.RIGHT);
			break;
		}
	}

	// Token: 0x060074C5 RID: 29893 RVA: 0x00227C14 File Offset: 0x00225E14
	private void SetPieceWidth(GameObject piece, float width)
	{
		Vector3 localScale = piece.transform.localScale;
		localScale.x = width * piece.transform.localScale.x / piece.GetComponent<Renderer>().bounds.size.x;
		piece.transform.localScale = localScale;
	}

	// Token: 0x060074C6 RID: 29894 RVA: 0x00227C74 File Offset: 0x00225E74
	private void SetPieceHeight(GameObject piece, float height)
	{
		Vector3 localScale = piece.transform.localScale;
		localScale.z = height * piece.transform.localScale.z / piece.GetComponent<Renderer>().bounds.size.y;
		piece.transform.localScale = localScale;
	}

	// Token: 0x04005F43 RID: 24387
	public GameObject m_topLeft;

	// Token: 0x04005F44 RID: 24388
	public GameObject m_top;

	// Token: 0x04005F45 RID: 24389
	public GameObject m_topRight;

	// Token: 0x04005F46 RID: 24390
	public GameObject m_left;

	// Token: 0x04005F47 RID: 24391
	public GameObject m_right;

	// Token: 0x04005F48 RID: 24392
	public GameObject m_middle;

	// Token: 0x04005F49 RID: 24393
	public GameObject m_bottomLeft;

	// Token: 0x04005F4A RID: 24394
	public GameObject m_bottom;

	// Token: 0x04005F4B RID: 24395
	public GameObject m_bottomRight;

	// Token: 0x04005F4C RID: 24396
	public GameObject m_anchorBone;

	// Token: 0x04005F4D RID: 24397
	public NewNineSliceElement.PinnedPoint m_pinnedPoint = NewNineSliceElement.PinnedPoint.TOP;

	// Token: 0x04005F4E RID: 24398
	public NewNineSliceElement.PlaneAxis m_planeAxis = NewNineSliceElement.PlaneAxis.XZ;

	// Token: 0x04005F4F RID: 24399
	public Vector3 m_pinnedPointOffset;

	// Token: 0x04005F50 RID: 24400
	public Vector2 m_middleScale;

	// Token: 0x04005F51 RID: 24401
	public Vector2 m_size;

	// Token: 0x04005F52 RID: 24402
	public NewNineSliceElement.Mode m_mode;

	// Token: 0x02000F57 RID: 3927
	public enum PinnedPoint
	{
		// Token: 0x04005F54 RID: 24404
		TOPLEFT,
		// Token: 0x04005F55 RID: 24405
		TOP,
		// Token: 0x04005F56 RID: 24406
		TOPRIGHT,
		// Token: 0x04005F57 RID: 24407
		LEFT,
		// Token: 0x04005F58 RID: 24408
		MIDDLE,
		// Token: 0x04005F59 RID: 24409
		RIGHT,
		// Token: 0x04005F5A RID: 24410
		BOTTOMLEFT,
		// Token: 0x04005F5B RID: 24411
		BOTTOM,
		// Token: 0x04005F5C RID: 24412
		BOTTOMRIGHT
	}

	// Token: 0x02000F58 RID: 3928
	public enum PlaneAxis
	{
		// Token: 0x04005F5E RID: 24414
		XY,
		// Token: 0x04005F5F RID: 24415
		XZ
	}

	// Token: 0x02000F59 RID: 3929
	public enum Mode
	{
		// Token: 0x04005F61 RID: 24417
		UseMiddleScale,
		// Token: 0x04005F62 RID: 24418
		UseSize
	}
}
