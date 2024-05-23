using System;
using UnityEngine;

// Token: 0x02000550 RID: 1360
[ExecuteInEditMode]
public class NewThreeSliceElement : MonoBehaviour
{
	// Token: 0x06003E7F RID: 15999 RVA: 0x0012E266 File Offset: 0x0012C466
	private void OnDestroy()
	{
		if (this.m_identity != null && this.m_identity.gameObject != null)
		{
			Object.DestroyImmediate(this.m_identity.gameObject);
		}
	}

	// Token: 0x06003E80 RID: 16000 RVA: 0x0012E2A0 File Offset: 0x0012C4A0
	public virtual void SetSize(Vector3 size)
	{
		this.m_middle.transform.localScale = size;
		if (this.m_identity == null)
		{
			this.m_identity = new GameObject().transform;
		}
		this.m_identity.position = Vector3.zero;
		if (this.m_planeAxis == NewThreeSliceElement.PlaneAxis.XZ)
		{
			this.m_leftAnchor = new Vector3(0f, 0f, 0.5f);
			this.m_rightAnchor = new Vector3(1f, 0f, 0.5f);
			this.m_topAnchor = new Vector3(0.5f, 0f, 1f);
			this.m_bottomAnchor = new Vector3(0.5f, 0f, 0f);
		}
		else
		{
			this.m_leftAnchor = new Vector3(0f, 0.5f, 0f);
			this.m_rightAnchor = new Vector3(1f, 0.5f, 0f);
			this.m_topAnchor = new Vector3(0.5f, 0f, 0f);
			this.m_bottomAnchor = new Vector3(0.5f, 1f, 0f);
		}
		switch (this.m_direction)
		{
		case NewThreeSliceElement.Direction.X:
			this.DisplayOnXAxis();
			break;
		case NewThreeSliceElement.Direction.Z:
			this.DisplayOnZAxis();
			break;
		}
	}

	// Token: 0x06003E81 RID: 16001 RVA: 0x0012E410 File Offset: 0x0012C610
	private void DisplayOnXAxis()
	{
		switch (this.m_pinnedPoint)
		{
		case NewThreeSliceElement.PinnedPoint.LEFT:
			this.m_leftOrTop.transform.localPosition = this.m_pinnedPointOffset;
			TransformUtil.SetPoint(this.m_middle, this.m_leftAnchor, this.m_leftOrTop, this.m_rightAnchor, this.m_identity.transform.TransformPoint(this.m_middleOffset));
			TransformUtil.SetPoint(this.m_rightOrBottom, this.m_leftAnchor, this.m_middle, this.m_rightAnchor, this.m_identity.transform.TransformPoint(this.m_rightOffset));
			break;
		case NewThreeSliceElement.PinnedPoint.MIDDLE:
			this.m_middle.transform.localPosition = this.m_pinnedPointOffset;
			TransformUtil.SetPoint(this.m_leftOrTop, this.m_rightAnchor, this.m_middle, this.m_leftAnchor, this.m_identity.transform.TransformPoint(this.m_leftOffset));
			TransformUtil.SetPoint(this.m_rightOrBottom, this.m_leftAnchor, this.m_middle, this.m_rightAnchor, this.m_identity.transform.TransformPoint(this.m_rightOffset));
			break;
		case NewThreeSliceElement.PinnedPoint.RIGHT:
			this.m_rightOrBottom.transform.localPosition = this.m_pinnedPointOffset;
			TransformUtil.SetPoint(this.m_middle, this.m_rightAnchor, this.m_rightOrBottom, this.m_leftAnchor, this.m_identity.transform.TransformPoint(this.m_middleOffset));
			TransformUtil.SetPoint(this.m_leftOrTop, this.m_rightAnchor, this.m_middle, this.m_leftAnchor, this.m_identity.transform.TransformPoint(this.m_leftOffset));
			break;
		}
	}

	// Token: 0x06003E82 RID: 16002 RVA: 0x0012E5C3 File Offset: 0x0012C7C3
	private void DisplayOnYAxis()
	{
	}

	// Token: 0x06003E83 RID: 16003 RVA: 0x0012E5C8 File Offset: 0x0012C7C8
	private void DisplayOnZAxis()
	{
		switch (this.m_pinnedPoint)
		{
		case NewThreeSliceElement.PinnedPoint.MIDDLE:
			this.m_middle.transform.localPosition = this.m_pinnedPointOffset;
			TransformUtil.SetPoint(this.m_leftOrTop, this.m_bottomAnchor, this.m_middle, this.m_topAnchor, this.m_identity.transform.TransformPoint(this.m_leftOffset));
			TransformUtil.SetPoint(this.m_rightOrBottom, this.m_topAnchor, this.m_middle, this.m_bottomAnchor, this.m_identity.transform.TransformPoint(this.m_rightOffset));
			break;
		case NewThreeSliceElement.PinnedPoint.TOP:
			this.m_leftOrTop.transform.localPosition = this.m_pinnedPointOffset;
			TransformUtil.SetPoint(this.m_middle, this.m_topAnchor, this.m_leftOrTop, this.m_bottomAnchor, this.m_identity.transform.TransformPoint(this.m_middleOffset));
			TransformUtil.SetPoint(this.m_rightOrBottom, this.m_topAnchor, this.m_middle, this.m_bottomAnchor, this.m_identity.transform.TransformPoint(this.m_rightOffset));
			break;
		case NewThreeSliceElement.PinnedPoint.BOTTOM:
			this.m_rightOrBottom.transform.localPosition = this.m_pinnedPointOffset;
			TransformUtil.SetPoint(this.m_middle, this.m_bottomAnchor, this.m_rightOrBottom, this.m_topAnchor, this.m_identity.transform.TransformPoint(this.m_middleOffset));
			TransformUtil.SetPoint(this.m_leftOrTop, this.m_bottomAnchor, this.m_middle, this.m_topAnchor, this.m_identity.transform.TransformPoint(this.m_leftOffset));
			break;
		}
	}

	// Token: 0x040027F1 RID: 10225
	public GameObject m_leftOrTop;

	// Token: 0x040027F2 RID: 10226
	public GameObject m_middle;

	// Token: 0x040027F3 RID: 10227
	public GameObject m_rightOrBottom;

	// Token: 0x040027F4 RID: 10228
	public NewThreeSliceElement.PinnedPoint m_pinnedPoint;

	// Token: 0x040027F5 RID: 10229
	public NewThreeSliceElement.PlaneAxis m_planeAxis = NewThreeSliceElement.PlaneAxis.XZ;

	// Token: 0x040027F6 RID: 10230
	public Vector3 m_pinnedPointOffset;

	// Token: 0x040027F7 RID: 10231
	public NewThreeSliceElement.Direction m_direction;

	// Token: 0x040027F8 RID: 10232
	public Vector3 m_middleScale = Vector3.one;

	// Token: 0x040027F9 RID: 10233
	public Vector3 m_leftOffset;

	// Token: 0x040027FA RID: 10234
	public Vector3 m_middleOffset;

	// Token: 0x040027FB RID: 10235
	public Vector3 m_rightOffset;

	// Token: 0x040027FC RID: 10236
	private Vector3 m_leftAnchor;

	// Token: 0x040027FD RID: 10237
	private Vector3 m_rightAnchor;

	// Token: 0x040027FE RID: 10238
	private Vector3 m_topAnchor;

	// Token: 0x040027FF RID: 10239
	private Vector3 m_bottomAnchor;

	// Token: 0x04002800 RID: 10240
	private Transform m_identity;

	// Token: 0x02000953 RID: 2387
	public enum PinnedPoint
	{
		// Token: 0x04003E48 RID: 15944
		LEFT,
		// Token: 0x04003E49 RID: 15945
		MIDDLE,
		// Token: 0x04003E4A RID: 15946
		RIGHT,
		// Token: 0x04003E4B RID: 15947
		TOP,
		// Token: 0x04003E4C RID: 15948
		BOTTOM
	}

	// Token: 0x02000954 RID: 2388
	public enum Direction
	{
		// Token: 0x04003E4E RID: 15950
		X,
		// Token: 0x04003E4F RID: 15951
		Y,
		// Token: 0x04003E50 RID: 15952
		Z
	}

	// Token: 0x02000955 RID: 2389
	public enum PlaneAxis
	{
		// Token: 0x04003E52 RID: 15954
		XY,
		// Token: 0x04003E53 RID: 15955
		XZ
	}
}
