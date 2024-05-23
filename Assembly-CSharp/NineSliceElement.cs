using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000471 RID: 1137
[CustomEditClass]
[ExecuteInEditMode]
public class NineSliceElement : MonoBehaviour
{
	// Token: 0x0600379D RID: 14237 RVA: 0x00110EB0 File Offset: 0x0010F0B0
	public void SetEntireWidth(float width)
	{
		int widthDirection = (int)this.m_WidthDirection;
		OrientedBounds sliceBounds = this.GetSliceBounds(this.m_topLeft);
		OrientedBounds sliceBounds2 = this.GetSliceBounds(this.m_left);
		OrientedBounds sliceBounds3 = this.GetSliceBounds(this.m_bottomLeft);
		OrientedBounds sliceBounds4 = this.GetSliceBounds(this.m_topRight);
		OrientedBounds sliceBounds5 = this.GetSliceBounds(this.m_right);
		OrientedBounds sliceBounds6 = this.GetSliceBounds(this.m_bottomRight);
		float num = Mathf.Max(new float[]
		{
			sliceBounds.Extents[widthDirection].magnitude,
			sliceBounds2.Extents[widthDirection].magnitude,
			sliceBounds3.Extents[widthDirection].magnitude
		}) * 2f;
		float num2 = Mathf.Max(new float[]
		{
			sliceBounds4.Extents[widthDirection].magnitude,
			sliceBounds5.Extents[widthDirection].magnitude,
			sliceBounds6.Extents[widthDirection].magnitude
		}) * 2f;
		this.SetWidth(width - num - num2);
	}

	// Token: 0x0600379E RID: 14238 RVA: 0x00110FE4 File Offset: 0x0010F1E4
	public void SetEntireHeight(float height)
	{
		int heightDirection = (int)this.m_HeightDirection;
		OrientedBounds sliceBounds = this.GetSliceBounds(this.m_topLeft);
		OrientedBounds sliceBounds2 = this.GetSliceBounds(this.m_top);
		OrientedBounds sliceBounds3 = this.GetSliceBounds(this.m_topRight);
		OrientedBounds sliceBounds4 = this.GetSliceBounds(this.m_bottomLeft);
		OrientedBounds sliceBounds5 = this.GetSliceBounds(this.m_bottom);
		OrientedBounds sliceBounds6 = this.GetSliceBounds(this.m_bottomRight);
		float num = Mathf.Max(new float[]
		{
			sliceBounds.Extents[heightDirection].magnitude,
			sliceBounds2.Extents[heightDirection].magnitude,
			sliceBounds3.Extents[heightDirection].magnitude
		}) * 2f;
		float num2 = Mathf.Max(new float[]
		{
			sliceBounds4.Extents[heightDirection].magnitude,
			sliceBounds5.Extents[heightDirection].magnitude,
			sliceBounds6.Extents[heightDirection].magnitude
		}) * 2f;
		this.SetHeight(height - num - num2);
	}

	// Token: 0x0600379F RID: 14239 RVA: 0x00111115 File Offset: 0x0010F315
	public void SetEntireSize(Vector2 size)
	{
		this.SetEntireSize(size.x, size.y);
	}

	// Token: 0x060037A0 RID: 14240 RVA: 0x0011112C File Offset: 0x0010F32C
	public void SetEntireSize(float width, float height)
	{
		int widthDirection = (int)this.m_WidthDirection;
		int heightDirection = (int)this.m_HeightDirection;
		OrientedBounds sliceBounds = this.GetSliceBounds(this.m_topLeft);
		OrientedBounds sliceBounds2 = this.GetSliceBounds(this.m_top);
		OrientedBounds sliceBounds3 = this.GetSliceBounds(this.m_topRight);
		OrientedBounds sliceBounds4 = this.GetSliceBounds(this.m_left);
		OrientedBounds sliceBounds5 = this.GetSliceBounds(this.m_right);
		OrientedBounds sliceBounds6 = this.GetSliceBounds(this.m_bottomLeft);
		OrientedBounds sliceBounds7 = this.GetSliceBounds(this.m_bottom);
		OrientedBounds sliceBounds8 = this.GetSliceBounds(this.m_bottomRight);
		float num = Mathf.Max(new float[]
		{
			sliceBounds.Extents[widthDirection].magnitude,
			sliceBounds4.Extents[widthDirection].magnitude,
			sliceBounds6.Extents[widthDirection].magnitude
		}) * 2f;
		float num2 = Mathf.Max(new float[]
		{
			sliceBounds3.Extents[widthDirection].magnitude,
			sliceBounds5.Extents[widthDirection].magnitude,
			sliceBounds8.Extents[widthDirection].magnitude
		}) * 2f;
		float num3 = Mathf.Max(new float[]
		{
			sliceBounds.Extents[heightDirection].magnitude,
			sliceBounds2.Extents[heightDirection].magnitude,
			sliceBounds3.Extents[heightDirection].magnitude
		}) * 2f;
		float num4 = Mathf.Max(new float[]
		{
			sliceBounds6.Extents[heightDirection].magnitude,
			sliceBounds7.Extents[heightDirection].magnitude,
			sliceBounds8.Extents[heightDirection].magnitude
		}) * 2f;
		this.SetSize(width - num - num2, height - num3 - num4);
	}

	// Token: 0x060037A1 RID: 14241 RVA: 0x00111338 File Offset: 0x0010F538
	public void SetWidth(float width)
	{
		width = Mathf.Max(width, 0f);
		int widthDirection = (int)this.m_WidthDirection;
		this.SetSliceSize(this.m_top, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(width, widthDirection)
		});
		this.SetSliceSize(this.m_bottom, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(width, widthDirection)
		});
		this.SetSliceSize(this.m_middle, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(width, widthDirection)
		});
		this.UpdateAllSlices();
	}

	// Token: 0x060037A2 RID: 14242 RVA: 0x001113E0 File Offset: 0x0010F5E0
	public void SetHeight(float height)
	{
		height = Mathf.Max(height, 0f);
		int heightDirection = (int)this.m_HeightDirection;
		this.SetSliceSize(this.m_left, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(height, heightDirection)
		});
		this.SetSliceSize(this.m_right, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(height, heightDirection)
		});
		this.SetSliceSize(this.m_middle, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(height, heightDirection)
		});
		this.UpdateAllSlices();
	}

	// Token: 0x060037A3 RID: 14243 RVA: 0x00111485 File Offset: 0x0010F685
	public void SetSize(Vector2 size)
	{
		this.SetSize(size.x, size.y);
	}

	// Token: 0x060037A4 RID: 14244 RVA: 0x0011149C File Offset: 0x0010F69C
	public void SetSize(float width, float height)
	{
		width = Mathf.Max(width, 0f);
		height = Mathf.Max(height, 0f);
		int widthDirection = (int)this.m_WidthDirection;
		int heightDirection = (int)this.m_HeightDirection;
		this.SetSliceSize(this.m_top, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(width, widthDirection)
		});
		this.SetSliceSize(this.m_bottom, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(width, widthDirection)
		});
		this.SetSliceSize(this.m_left, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(height, heightDirection)
		});
		this.SetSliceSize(this.m_right, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(height, heightDirection)
		});
		this.SetSliceSize(this.m_middle, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(width, widthDirection),
			new WorldDimensionIndex(height, heightDirection)
		});
		this.UpdateAllSlices();
	}

	// Token: 0x060037A5 RID: 14245 RVA: 0x001115BC File Offset: 0x0010F7BC
	public void SetMiddleScale(float scaleWidth, float scaleHeight)
	{
		Vector3 localScale = this.m_middle.m_slice.transform.localScale;
		localScale[(int)this.m_WidthDirection] = scaleWidth;
		localScale[(int)this.m_HeightDirection] = scaleHeight;
		this.m_middle.m_slice.transform.localScale = localScale;
		this.UpdateSegmentsToMatchMiddle();
		this.UpdateAllSlices();
	}

	// Token: 0x060037A6 RID: 14246 RVA: 0x00111620 File Offset: 0x0010F820
	public Vector2 GetWorldDimensions()
	{
		OrientedBounds orientedBounds = TransformUtil.ComputeOrientedWorldBounds(this.m_middle, this.m_ignore, true);
		return new Vector2(orientedBounds.Extents[(int)this.m_WidthDirection].magnitude * 2f, orientedBounds.Extents[(int)this.m_HeightDirection].magnitude * 2f);
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x00111684 File Offset: 0x0010F884
	private OrientedBounds GetSliceBounds(GameObject slice)
	{
		if (slice != null)
		{
			return TransformUtil.ComputeOrientedWorldBounds(slice, this.m_ignore, true);
		}
		return new OrientedBounds
		{
			Extents = new Vector3[]
			{
				Vector3.zero,
				Vector3.zero,
				Vector3.zero
			},
			Origin = Vector3.zero,
			CenterOffset = Vector3.zero
		};
	}

	// Token: 0x060037A8 RID: 14248 RVA: 0x00111707 File Offset: 0x0010F907
	private void SetSliceSize(GameObject slice, params WorldDimensionIndex[] dimensions)
	{
		if (slice != null)
		{
			TransformUtil.SetLocalScaleToWorldDimension(slice, this.m_ignore, dimensions);
		}
	}

	// Token: 0x060037A9 RID: 14249 RVA: 0x00111724 File Offset: 0x0010F924
	private void UpdateAllSlices()
	{
		List<MultiSliceElement.Slice> list = new List<MultiSliceElement.Slice>();
		list.Add(this.m_topLeft);
		list.Add(this.m_top);
		list.Add(this.m_topRight);
		this.UpdateRowSlices(list, this.m_WidthDirection);
		list = new List<MultiSliceElement.Slice>();
		list.Add(this.m_left);
		list.Add(this.m_middle);
		list.Add(this.m_right);
		this.UpdateRowSlices(list, this.m_WidthDirection);
		list = new List<MultiSliceElement.Slice>();
		list.Add(this.m_bottomLeft);
		list.Add(this.m_bottom);
		list.Add(this.m_bottomRight);
		this.UpdateRowSlices(list, this.m_WidthDirection);
		list = new List<MultiSliceElement.Slice>();
		list.Add(this.m_topRow);
		list.Add(this.m_midRow);
		list.Add(this.m_btmRow);
		this.UpdateRowSlices(list, this.m_HeightDirection);
	}

	// Token: 0x060037AA RID: 14250 RVA: 0x00111810 File Offset: 0x0010FA10
	private void UpdateRowSlices(List<MultiSliceElement.Slice> slices, MultiSliceElement.Direction direction)
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		zero[(int)direction] = this.m_localSliceSpacing[(int)direction];
		zero2[(int)direction] = this.m_localPinnedPointOffset[(int)direction];
		MultiSliceElement.PositionSlices(base.transform, slices, this.m_reverse, direction, this.m_useUberText, zero, zero2, this.m_XAlign, this.m_YAlign, this.m_ZAlign, this.m_ignore);
	}

	// Token: 0x060037AB RID: 14251 RVA: 0x00111884 File Offset: 0x0010FA84
	private void UpdateSegmentsToMatchMiddle()
	{
		OrientedBounds orientedBounds = TransformUtil.ComputeOrientedWorldBounds(this.m_middle, this.m_ignore, true);
		if (orientedBounds == null)
		{
			return;
		}
		float dimension = orientedBounds.Extents[(int)this.m_WidthDirection].magnitude * 2f;
		float dimension2 = orientedBounds.Extents[(int)this.m_HeightDirection].magnitude * 2f;
		int widthDirection = (int)this.m_WidthDirection;
		int heightDirection = (int)this.m_HeightDirection;
		this.SetSliceSize(this.m_top, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(dimension, widthDirection)
		});
		this.SetSliceSize(this.m_bottom, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(dimension, widthDirection)
		});
		this.SetSliceSize(this.m_left, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(dimension2, heightDirection)
		});
		this.SetSliceSize(this.m_right, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(dimension2, heightDirection)
		});
	}

	// Token: 0x04002374 RID: 9076
	[CustomEditField(Sections = "Top Row")]
	public MultiSliceElement.Slice m_topRow;

	// Token: 0x04002375 RID: 9077
	[CustomEditField(Sections = "Middle Row")]
	public MultiSliceElement.Slice m_midRow;

	// Token: 0x04002376 RID: 9078
	[CustomEditField(Sections = "Bottom Row")]
	public MultiSliceElement.Slice m_btmRow;

	// Token: 0x04002377 RID: 9079
	[CustomEditField(Sections = "Top Row")]
	public MultiSliceElement.Slice m_topLeft;

	// Token: 0x04002378 RID: 9080
	[CustomEditField(Sections = "Top Row")]
	public MultiSliceElement.Slice m_top;

	// Token: 0x04002379 RID: 9081
	[CustomEditField(Sections = "Top Row")]
	public MultiSliceElement.Slice m_topRight;

	// Token: 0x0400237A RID: 9082
	[CustomEditField(Sections = "Middle Row")]
	public MultiSliceElement.Slice m_left;

	// Token: 0x0400237B RID: 9083
	[CustomEditField(Sections = "Middle Row")]
	public MultiSliceElement.Slice m_middle;

	// Token: 0x0400237C RID: 9084
	[CustomEditField(Sections = "Middle Row")]
	public MultiSliceElement.Slice m_right;

	// Token: 0x0400237D RID: 9085
	[CustomEditField(Sections = "Bottom Row")]
	public MultiSliceElement.Slice m_bottomLeft;

	// Token: 0x0400237E RID: 9086
	[CustomEditField(Sections = "Bottom Row")]
	public MultiSliceElement.Slice m_bottom;

	// Token: 0x0400237F RID: 9087
	[CustomEditField(Sections = "Bottom Row")]
	public MultiSliceElement.Slice m_bottomRight;

	// Token: 0x04002380 RID: 9088
	public List<GameObject> m_ignore = new List<GameObject>();

	// Token: 0x04002381 RID: 9089
	public MultiSliceElement.Direction m_WidthDirection;

	// Token: 0x04002382 RID: 9090
	public MultiSliceElement.Direction m_HeightDirection = MultiSliceElement.Direction.Z;

	// Token: 0x04002383 RID: 9091
	public Vector3 m_localPinnedPointOffset = Vector3.zero;

	// Token: 0x04002384 RID: 9092
	public MultiSliceElement.XAxisAlign m_XAlign;

	// Token: 0x04002385 RID: 9093
	public MultiSliceElement.YAxisAlign m_YAlign = MultiSliceElement.YAxisAlign.BOTTOM;

	// Token: 0x04002386 RID: 9094
	public MultiSliceElement.ZAxisAlign m_ZAlign = MultiSliceElement.ZAxisAlign.BACK;

	// Token: 0x04002387 RID: 9095
	public Vector3 m_localSliceSpacing = Vector3.zero;

	// Token: 0x04002388 RID: 9096
	public bool m_reverse;

	// Token: 0x04002389 RID: 9097
	public bool m_useUberText;
}
