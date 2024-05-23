using System;
using UnityEngine;

// Token: 0x020002B9 RID: 697
public class UIBScrollableItem : MonoBehaviour
{
	// Token: 0x0600259E RID: 9630 RVA: 0x000B845C File Offset: 0x000B665C
	public bool IsActive()
	{
		if (this.m_activeStateCallback != null)
		{
			return this.m_activeStateCallback();
		}
		return this.m_active == UIBScrollableItem.ActiveState.Active || (this.m_active == UIBScrollableItem.ActiveState.UseHierarchy && base.gameObject.activeInHierarchy);
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x000B84A8 File Offset: 0x000B66A8
	public void SetCustomActiveState(UIBScrollableItem.ActiveStateCallback callback)
	{
		this.m_activeStateCallback = callback;
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x000B84B4 File Offset: 0x000B66B4
	public OrientedBounds GetOrientedBounds()
	{
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		return new OrientedBounds
		{
			Origin = base.transform.position + localToWorldMatrix * this.m_offset,
			Extents = new Vector3[]
			{
				localToWorldMatrix * new Vector3(this.m_size.x * 0.5f, 0f, 0f),
				localToWorldMatrix * new Vector3(0f, this.m_size.y * 0.5f, 0f),
				localToWorldMatrix * new Vector3(0f, 0f, this.m_size.z * 0.5f)
			}
		};
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x000B85C0 File Offset: 0x000B67C0
	public void GetWorldBounds(out Vector3 min, out Vector3 max)
	{
		min..ctor(float.MaxValue, float.MaxValue, float.MaxValue);
		max..ctor(float.MinValue, float.MinValue, float.MinValue);
		Vector3[] boundsPoints = this.GetBoundsPoints();
		for (int i = 0; i < 8; i++)
		{
			min.x = Mathf.Min(boundsPoints[i].x, min.x);
			min.y = Mathf.Min(boundsPoints[i].y, min.y);
			min.z = Mathf.Min(boundsPoints[i].z, min.z);
			max.x = Mathf.Max(boundsPoints[i].x, max.x);
			max.y = Mathf.Max(boundsPoints[i].y, max.y);
			max.z = Mathf.Max(boundsPoints[i].z, max.z);
		}
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x000B86C0 File Offset: 0x000B68C0
	private Vector3[] GetBoundsPoints()
	{
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		Vector3[] array = new Vector3[]
		{
			localToWorldMatrix * new Vector3(this.m_size.x * 0.5f, 0f, 0f),
			localToWorldMatrix * new Vector3(0f, this.m_size.y * 0.5f, 0f),
			localToWorldMatrix * new Vector3(0f, 0f, this.m_size.z * 0.5f)
		};
		Vector3 vector = base.transform.position + localToWorldMatrix * this.m_offset;
		return new Vector3[]
		{
			vector + array[0] + array[1] + array[2],
			vector + array[0] + array[1] - array[2],
			vector + array[0] - array[1] + array[2],
			vector + array[0] - array[1] - array[2],
			vector - array[0] + array[1] + array[2],
			vector - array[0] + array[1] - array[2],
			vector - array[0] - array[1] + array[2],
			vector - array[0] - array[1] - array[2]
		};
	}

	// Token: 0x04001640 RID: 5696
	public Vector3 m_offset = Vector3.zero;

	// Token: 0x04001641 RID: 5697
	public Vector3 m_size = Vector3.one;

	// Token: 0x04001642 RID: 5698
	public UIBScrollableItem.ActiveState m_active;

	// Token: 0x04001643 RID: 5699
	private UIBScrollableItem.ActiveStateCallback m_activeStateCallback;

	// Token: 0x020006C6 RID: 1734
	public enum ActiveState
	{
		// Token: 0x04002F9A RID: 12186
		Active,
		// Token: 0x04002F9B RID: 12187
		Inactive,
		// Token: 0x04002F9C RID: 12188
		UseHierarchy
	}

	// Token: 0x02000736 RID: 1846
	// (Invoke) Token: 0x06004B30 RID: 19248
	public delegate bool ActiveStateCallback();
}
