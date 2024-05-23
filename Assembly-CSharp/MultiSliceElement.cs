using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000470 RID: 1136
[CustomEditClass]
[ExecuteInEditMode]
public class MultiSliceElement : MonoBehaviour
{
	// Token: 0x06003796 RID: 14230 RVA: 0x001108B5 File Offset: 0x0010EAB5
	public void AddSlice(GameObject obj)
	{
		this.AddSlice(obj, Vector3.zero, Vector3.zero, false);
	}

	// Token: 0x06003797 RID: 14231 RVA: 0x001108CC File Offset: 0x0010EACC
	public void AddSlice(GameObject obj, Vector3 minLocalPadding, Vector3 maxLocalPadding, bool reverse = false)
	{
		this.m_slices.Add(new MultiSliceElement.Slice
		{
			m_slice = obj,
			m_minLocalPadding = minLocalPadding,
			m_maxLocalPadding = maxLocalPadding,
			m_reverse = reverse
		});
	}

	// Token: 0x06003798 RID: 14232 RVA: 0x00110908 File Offset: 0x0010EB08
	public void ClearSlices()
	{
		this.m_slices.Clear();
	}

	// Token: 0x06003799 RID: 14233 RVA: 0x00110918 File Offset: 0x0010EB18
	public void UpdateSlices()
	{
		MultiSliceElement.PositionSlices(base.transform, this.m_slices, this.m_reverse, this.m_direction, this.m_useUberText, this.m_localSliceSpacing, this.m_localPinnedPointOffset, this.m_XAlign, this.m_YAlign, this.m_ZAlign, this.m_ignore);
	}

	// Token: 0x0600379A RID: 14234 RVA: 0x0011096C File Offset: 0x0010EB6C
	public static void PositionSlices(Transform root, List<MultiSliceElement.Slice> slices, bool reverseDir, MultiSliceElement.Direction dir, bool useUberText, Vector3 localSliceSpacing, Vector3 localPinnedPointOffset, MultiSliceElement.XAxisAlign xAlign, MultiSliceElement.YAxisAlign yAlign, MultiSliceElement.ZAxisAlign zAlign, List<GameObject> ignoreObjects = null)
	{
		if (slices.Count == 0)
		{
			return;
		}
		float num = (!reverseDir) ? 1f : -1f;
		MultiSliceElement.Slice[] array = slices.FindAll((MultiSliceElement.Slice s) => TransformUtil.CanComputeOrientedWorldBounds(s.m_slice, useUberText, ignoreObjects, true)).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
		Vector3 vector2;
		vector2..ctor(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector3;
		vector3..ctor(float.MinValue, float.MinValue, float.MinValue);
		MultiSliceElement.Slice slice = array[0];
		GameObject slice2 = slice.m_slice;
		slice2.transform.localPosition = Vector3.zero;
		OrientedBounds orientedBounds = TransformUtil.ComputeOrientedWorldBounds(slice2, useUberText, slice.m_minLocalPadding, slice.m_maxLocalPadding, ignoreObjects, true);
		float num2 = num * ((!slice.m_reverse) ? 1f : -1f);
		Vector3 vector4 = (orientedBounds.Extents[0] + orientedBounds.Extents[1] + orientedBounds.Extents[2]) * num2;
		slice2.transform.position += orientedBounds.CenterOffset + vector4;
		vector = orientedBounds.Extents[(int)dir] * num2 + vector4;
		TransformUtil.GetBoundsMinMax(worldToLocalMatrix * (slice2.transform.position - orientedBounds.CenterOffset), worldToLocalMatrix * orientedBounds.Extents[0], worldToLocalMatrix * orientedBounds.Extents[1], worldToLocalMatrix * orientedBounds.Extents[2], ref vector2, ref vector3);
		Vector3 vector5 = localSliceSpacing * num;
		for (int i = 1; i < array.Length; i++)
		{
			MultiSliceElement.Slice slice3 = array[i];
			GameObject slice4 = slice3.m_slice;
			float num3 = num * ((!slice3.m_reverse) ? 1f : -1f);
			slice4.transform.localPosition = Vector3.zero;
			OrientedBounds orientedBounds2 = TransformUtil.ComputeOrientedWorldBounds(slice4, useUberText, slice3.m_minLocalPadding, slice3.m_maxLocalPadding, ignoreObjects, true);
			Vector3 vector6 = slice4.transform.localToWorldMatrix * vector5;
			Vector3 vector7 = orientedBounds2.Extents[(int)dir] * num3;
			slice4.transform.position += orientedBounds2.CenterOffset + vector + vector7 + vector6;
			vector += vector7 * 2f + vector6;
			TransformUtil.GetBoundsMinMax(worldToLocalMatrix * (slice4.transform.position - orientedBounds2.CenterOffset), worldToLocalMatrix * orientedBounds2.Extents[0], worldToLocalMatrix * orientedBounds2.Extents[1], worldToLocalMatrix * orientedBounds2.Extents[2], ref vector2, ref vector3);
		}
		Vector3 vector8;
		vector8..ctor(vector2.x, vector3.y, vector2.z);
		Vector3 vector9;
		vector9..ctor(vector3.x, vector2.y, vector3.z);
		Vector3 vector10 = root.localToWorldMatrix * (vector8 + MultiSliceElement.GetAlignmentVector(vector9 - vector8, xAlign, yAlign, zAlign));
		Vector3 vector11 = root.localToWorldMatrix * localPinnedPointOffset * num;
		Vector3 vector12 = root.position - vector10;
		Vector3 vector13 = vector11 + vector12;
		foreach (MultiSliceElement.Slice slice5 in array)
		{
			slice5.m_slice.transform.position += vector13;
		}
	}

	// Token: 0x0600379B RID: 14235 RVA: 0x00110E2C File Offset: 0x0010F02C
	private static Vector3 GetAlignmentVector(Vector3 interpolate, MultiSliceElement.XAxisAlign x, MultiSliceElement.YAxisAlign y, MultiSliceElement.ZAxisAlign z)
	{
		return new Vector3(interpolate.x * ((float)x * 0.5f), interpolate.y * ((float)y * 0.5f), interpolate.z * ((float)z * 0.5f));
	}

	// Token: 0x0400236A RID: 9066
	[CustomEditField(ListTable = true)]
	public List<MultiSliceElement.Slice> m_slices = new List<MultiSliceElement.Slice>();

	// Token: 0x0400236B RID: 9067
	public List<GameObject> m_ignore = new List<GameObject>();

	// Token: 0x0400236C RID: 9068
	public Vector3 m_localPinnedPointOffset = Vector3.zero;

	// Token: 0x0400236D RID: 9069
	public MultiSliceElement.XAxisAlign m_XAlign;

	// Token: 0x0400236E RID: 9070
	public MultiSliceElement.YAxisAlign m_YAlign = MultiSliceElement.YAxisAlign.BOTTOM;

	// Token: 0x0400236F RID: 9071
	public MultiSliceElement.ZAxisAlign m_ZAlign = MultiSliceElement.ZAxisAlign.BACK;

	// Token: 0x04002370 RID: 9072
	public Vector3 m_localSliceSpacing = Vector3.zero;

	// Token: 0x04002371 RID: 9073
	public MultiSliceElement.Direction m_direction;

	// Token: 0x04002372 RID: 9074
	public bool m_reverse;

	// Token: 0x04002373 RID: 9075
	public bool m_useUberText;

	// Token: 0x02000473 RID: 1139
	[Serializable]
	public class Slice
	{
		// Token: 0x060037AD RID: 14253 RVA: 0x001119AB File Offset: 0x0010FBAB
		public static implicit operator GameObject(MultiSliceElement.Slice slice)
		{
			return (slice == null) ? null : slice.m_slice;
		}

		// Token: 0x0400238E RID: 9102
		public GameObject m_slice;

		// Token: 0x0400238F RID: 9103
		public Vector3 m_minLocalPadding;

		// Token: 0x04002390 RID: 9104
		public Vector3 m_maxLocalPadding;

		// Token: 0x04002391 RID: 9105
		public bool m_reverse;
	}

	// Token: 0x02000500 RID: 1280
	public enum Direction
	{
		// Token: 0x04002633 RID: 9779
		X,
		// Token: 0x04002634 RID: 9780
		Y,
		// Token: 0x04002635 RID: 9781
		Z
	}

	// Token: 0x02000501 RID: 1281
	public enum XAxisAlign
	{
		// Token: 0x04002637 RID: 9783
		LEFT,
		// Token: 0x04002638 RID: 9784
		MIDDLE,
		// Token: 0x04002639 RID: 9785
		RIGHT
	}

	// Token: 0x02000502 RID: 1282
	public enum YAxisAlign
	{
		// Token: 0x0400263B RID: 9787
		TOP,
		// Token: 0x0400263C RID: 9788
		MIDDLE,
		// Token: 0x0400263D RID: 9789
		BOTTOM
	}

	// Token: 0x02000503 RID: 1283
	public enum ZAxisAlign
	{
		// Token: 0x0400263F RID: 9791
		FRONT,
		// Token: 0x04002640 RID: 9792
		MIDDLE,
		// Token: 0x04002641 RID: 9793
		BACK
	}
}
