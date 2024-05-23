using System;
using UnityEngine;

// Token: 0x02000584 RID: 1412
public class TiledBackground : MonoBehaviour
{
	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x06004029 RID: 16425 RVA: 0x0013702C File Offset: 0x0013522C
	// (set) Token: 0x0600402A RID: 16426 RVA: 0x001370A0 File Offset: 0x001352A0
	public Vector2 Offset
	{
		get
		{
			return new Vector2(base.GetComponent<Renderer>().material.mainTextureOffset.x / base.GetComponent<Renderer>().material.mainTextureScale.x, base.GetComponent<Renderer>().material.mainTextureOffset.y / base.GetComponent<Renderer>().material.mainTextureScale.y);
		}
		set
		{
			base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(base.GetComponent<Renderer>().material.mainTextureScale.x * value.x, base.GetComponent<Renderer>().material.mainTextureScale.y * value.y);
		}
	}

	// Token: 0x0600402B RID: 16427 RVA: 0x00137102 File Offset: 0x00135302
	private void Awake()
	{
		if (base.GetComponent<Renderer>().material == null)
		{
			Debug.LogError("TiledBackground requires the mesh renderer to have a material");
			Object.Destroy(this);
		}
	}

	// Token: 0x0600402C RID: 16428 RVA: 0x0013712C File Offset: 0x0013532C
	public void SetBounds(Bounds bounds)
	{
		base.transform.localScale = Vector3.one;
		Vector3 vector = base.GetComponent<Renderer>().bounds.min;
		Vector3 vector2 = base.GetComponent<Renderer>().bounds.max;
		if (base.transform.parent != null)
		{
			vector = base.transform.parent.InverseTransformPoint(vector);
			vector2 = base.transform.parent.InverseTransformPoint(vector2);
		}
		Vector3 vector3 = VectorUtils.Abs(vector2 - vector);
		Vector3 vector4;
		vector4..ctor((vector3.x <= 0f) ? 0f : (bounds.size.x / vector3.x), (vector3.y <= 0f) ? 0f : (bounds.size.y / vector3.y), (vector3.z <= 0f) ? 0f : (bounds.size.z / vector3.z));
		base.transform.localScale = vector4;
		base.transform.localPosition = bounds.center + new Vector3(0f, 0f, -this.Depth);
		base.GetComponent<Renderer>().material.mainTextureScale = vector4;
	}

	// Token: 0x0400290C RID: 10508
	public float Depth;
}
