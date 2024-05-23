using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001B1 RID: 433
public class TransformUtil
{
	// Token: 0x06001C5E RID: 7262 RVA: 0x00085694 File Offset: 0x00083894
	public static Vector3 GetUnitAnchor(Anchor anchor)
	{
		Vector3 result = default(Vector3);
		switch (anchor)
		{
		case Anchor.TOP_LEFT:
			result.x = 0f;
			result.y = 1f;
			result.z = 0f;
			break;
		case Anchor.TOP:
			result.x = 0.5f;
			result.y = 1f;
			result.z = 0f;
			break;
		case Anchor.TOP_RIGHT:
			result.x = 1f;
			result.y = 1f;
			result.z = 0f;
			break;
		case Anchor.LEFT:
			result.x = 0f;
			result.y = 0.5f;
			result.z = 0f;
			break;
		case Anchor.CENTER:
			result.x = 0.5f;
			result.y = 0.5f;
			result.z = 0f;
			break;
		case Anchor.RIGHT:
			result.x = 1f;
			result.y = 0.5f;
			result.z = 0f;
			break;
		case Anchor.BOTTOM_LEFT:
			result.x = 0f;
			result.y = 0f;
			result.z = 0f;
			break;
		case Anchor.BOTTOM:
			result.x = 0.5f;
			result.y = 0f;
			result.z = 0f;
			break;
		case Anchor.BOTTOM_RIGHT:
			result.x = 1f;
			result.y = 0f;
			result.z = 0f;
			break;
		case Anchor.FRONT:
			result.x = 0.5f;
			result.y = 0f;
			result.z = 1f;
			break;
		case Anchor.BACK:
			result.x = 0.5f;
			result.y = 0f;
			result.z = 0f;
			break;
		case Anchor.TOP_LEFT_XZ:
			result.x = 0f;
			result.z = 1f;
			result.y = 0f;
			break;
		case Anchor.TOP_XZ:
			result.x = 0.5f;
			result.z = 1f;
			result.y = 0f;
			break;
		case Anchor.TOP_RIGHT_XZ:
			result.x = 1f;
			result.z = 1f;
			result.y = 0f;
			break;
		case Anchor.LEFT_XZ:
			result.x = 0f;
			result.z = 0.5f;
			result.y = 0f;
			break;
		case Anchor.CENTER_XZ:
			result.x = 0.5f;
			result.z = 0.5f;
			result.y = 0f;
			break;
		case Anchor.RIGHT_XZ:
			result.x = 1f;
			result.z = 0.5f;
			result.y = 0f;
			break;
		case Anchor.BOTTOM_LEFT_XZ:
			result.x = 0f;
			result.z = 0f;
			result.y = 0f;
			break;
		case Anchor.BOTTOM_XZ:
			result.x = 0.5f;
			result.z = 0f;
			result.y = 0f;
			break;
		case Anchor.BOTTOM_RIGHT_XZ:
			result.x = 1f;
			result.z = 0f;
			result.y = 0f;
			break;
		case Anchor.FRONT_XZ:
			result.x = 0.5f;
			result.z = 0f;
			result.y = 1f;
			break;
		case Anchor.BACK_XZ:
			result.x = 0.5f;
			result.z = 0f;
			result.y = 0f;
			break;
		}
		return result;
	}

	// Token: 0x06001C5F RID: 7263 RVA: 0x00085A9C File Offset: 0x00083C9C
	public static Vector3 ComputeWorldPoint(Bounds bounds, Vector3 selfUnitAnchor)
	{
		Vector3 result = default(Vector3);
		result.x = Mathf.Lerp(bounds.min.x, bounds.max.x, selfUnitAnchor.x);
		result.y = Mathf.Lerp(bounds.min.y, bounds.max.y, selfUnitAnchor.y);
		result.z = Mathf.Lerp(bounds.min.z, bounds.max.z, selfUnitAnchor.z);
		return result;
	}

	// Token: 0x06001C60 RID: 7264 RVA: 0x00085B48 File Offset: 0x00083D48
	public static Vector2 ComputeUnitAnchor(Bounds bounds, Vector2 worldPoint)
	{
		Vector2 result = default(Vector2);
		result.x = (worldPoint.x - bounds.min.x) / bounds.size.x;
		result.y = (worldPoint.y - bounds.min.y) / bounds.size.y;
		return result;
	}

	// Token: 0x06001C61 RID: 7265 RVA: 0x00085BBB File Offset: 0x00083DBB
	public static Bounds ComputeSetPointBounds(Component c)
	{
		return TransformUtil.ComputeSetPointBounds(c.gameObject, false);
	}

	// Token: 0x06001C62 RID: 7266 RVA: 0x00085BC9 File Offset: 0x00083DC9
	public static Bounds ComputeSetPointBounds(GameObject go)
	{
		return TransformUtil.ComputeSetPointBounds(go, false);
	}

	// Token: 0x06001C63 RID: 7267 RVA: 0x00085BD2 File Offset: 0x00083DD2
	public static Bounds ComputeSetPointBounds(Component c, bool includeInactive)
	{
		return TransformUtil.ComputeSetPointBounds(c.gameObject, includeInactive);
	}

	// Token: 0x06001C64 RID: 7268 RVA: 0x00085BE0 File Offset: 0x00083DE0
	public static Bounds ComputeSetPointBounds(GameObject go, bool includeInactive)
	{
		UberText component = go.GetComponent<UberText>();
		if (component != null)
		{
			return component.GetTextWorldSpaceBounds();
		}
		Renderer component2 = go.GetComponent<Renderer>();
		if (component2 != null)
		{
			return component2.bounds;
		}
		Collider component3 = go.GetComponent<Collider>();
		if (component3 != null)
		{
			Bounds bounds;
			if (component3.enabled)
			{
				bounds = component3.bounds;
			}
			else
			{
				component3.enabled = true;
				bounds = component3.bounds;
				component3.enabled = false;
			}
			MobileHitBox component4 = go.GetComponent<MobileHitBox>();
			if (component4 != null && component4.HasExecuted())
			{
				bounds.size = new Vector3(bounds.size.x / component4.m_scaleX, bounds.size.y / component4.m_scaleY, bounds.size.z / component4.m_scaleY);
			}
			return bounds;
		}
		return TransformUtil.GetBoundsOfChildren(go, includeInactive);
	}

	// Token: 0x06001C65 RID: 7269 RVA: 0x00085CDE File Offset: 0x00083EDE
	public static OrientedBounds ComputeOrientedWorldBounds(GameObject go, bool includeAllChildren = true)
	{
		return TransformUtil.ComputeOrientedWorldBounds(go, true, includeAllChildren);
	}

	// Token: 0x06001C66 RID: 7270 RVA: 0x00085CE8 File Offset: 0x00083EE8
	public static OrientedBounds ComputeOrientedWorldBounds(GameObject go, List<GameObject> ignoreMeshes, bool includeAllChildren = true)
	{
		return TransformUtil.ComputeOrientedWorldBounds(go, true, ignoreMeshes, includeAllChildren);
	}

	// Token: 0x06001C67 RID: 7271 RVA: 0x00085CF3 File Offset: 0x00083EF3
	public static OrientedBounds ComputeOrientedWorldBounds(GameObject go, Vector3 minLocalPadding, Vector3 maxLocalPadding, bool includeAllChildren = true)
	{
		return TransformUtil.ComputeOrientedWorldBounds(go, true, minLocalPadding, maxLocalPadding, includeAllChildren);
	}

	// Token: 0x06001C68 RID: 7272 RVA: 0x00085CFF File Offset: 0x00083EFF
	public static OrientedBounds ComputeOrientedWorldBounds(GameObject go, Vector3 minLocalPadding, Vector3 maxLocalPadding, List<GameObject> ignoreMeshes, bool includeAllChildren = true)
	{
		return TransformUtil.ComputeOrientedWorldBounds(go, true, minLocalPadding, maxLocalPadding, ignoreMeshes, includeAllChildren);
	}

	// Token: 0x06001C69 RID: 7273 RVA: 0x00085D10 File Offset: 0x00083F10
	public static OrientedBounds ComputeOrientedWorldBounds(GameObject go, bool includeUberText, bool includeAllChildren = true)
	{
		return TransformUtil.ComputeOrientedWorldBounds(go, includeUberText, Vector3.zero, Vector3.zero, null, includeAllChildren);
	}

	// Token: 0x06001C6A RID: 7274 RVA: 0x00085D30 File Offset: 0x00083F30
	public static OrientedBounds ComputeOrientedWorldBounds(GameObject go, bool includeUberText, List<GameObject> ignoreMeshes, bool includeAllChildren = true)
	{
		return TransformUtil.ComputeOrientedWorldBounds(go, includeUberText, Vector3.zero, Vector3.zero, ignoreMeshes, includeAllChildren);
	}

	// Token: 0x06001C6B RID: 7275 RVA: 0x00085D50 File Offset: 0x00083F50
	public static OrientedBounds ComputeOrientedWorldBounds(GameObject go, bool includeUberText, Vector3 minLocalPadding, Vector3 maxLocalPadding, bool includeAllChildren = true)
	{
		return TransformUtil.ComputeOrientedWorldBounds(go, includeUberText, minLocalPadding, maxLocalPadding, null, includeAllChildren);
	}

	// Token: 0x06001C6C RID: 7276 RVA: 0x00085D60 File Offset: 0x00083F60
	public static OrientedBounds ComputeOrientedWorldBounds(GameObject go, bool includeUberText, Vector3 minLocalPadding, Vector3 maxLocalPadding, List<GameObject> ignoreMeshes, bool includeAllChildren = true)
	{
		if (go == null || !go.activeSelf)
		{
			return null;
		}
		List<MeshFilter> componentsWithIgnore = TransformUtil.GetComponentsWithIgnore<MeshFilter>(go, ignoreMeshes, includeAllChildren);
		List<UberText> list = null;
		if (includeUberText)
		{
			list = TransformUtil.GetComponentsWithIgnore<UberText>(go, ignoreMeshes, includeAllChildren);
		}
		if ((componentsWithIgnore == null || componentsWithIgnore.Count == 0) && (list == null || list.Count == 0))
		{
			return null;
		}
		Matrix4x4 worldToLocalMatrix = go.transform.worldToLocalMatrix;
		Vector3 vector;
		vector..ctor(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2;
		vector2..ctor(float.MinValue, float.MinValue, float.MinValue);
		if (componentsWithIgnore != null)
		{
			foreach (MeshFilter meshFilter in componentsWithIgnore)
			{
				if (meshFilter.gameObject.activeSelf && !(meshFilter.sharedMesh == null))
				{
					Matrix4x4 localToWorldMatrix = meshFilter.transform.localToWorldMatrix;
					Bounds bounds = meshFilter.sharedMesh.bounds;
					Matrix4x4 matrix4x = worldToLocalMatrix * localToWorldMatrix;
					Vector3[] array = new Vector3[]
					{
						matrix4x * new Vector3(bounds.extents.x, 0f, 0f),
						matrix4x * new Vector3(0f, bounds.extents.y, 0f),
						matrix4x * new Vector3(0f, 0f, bounds.extents.z)
					};
					Vector3 vector3 = localToWorldMatrix * meshFilter.sharedMesh.bounds.center;
					Vector3 origin = worldToLocalMatrix * (meshFilter.transform.position + vector3);
					TransformUtil.GetBoundsMinMax(origin, array[0], array[1], array[2], ref vector, ref vector2);
				}
			}
		}
		if (list != null)
		{
			foreach (UberText uberText in list)
			{
				if (uberText.gameObject.activeSelf)
				{
					Matrix4x4 localToWorldMatrix2 = uberText.transform.localToWorldMatrix;
					Matrix4x4 matrix4x2 = worldToLocalMatrix * localToWorldMatrix2;
					Vector3[] array2 = new Vector3[]
					{
						matrix4x2 * new Vector3(uberText.Width * 0.5f, 0f, 0f),
						matrix4x2 * new Vector3(0f, uberText.Height * 0.5f),
						matrix4x2 * new Vector3(0f, 0f, 0.01f)
					};
					TransformUtil.GetBoundsMinMax(worldToLocalMatrix * uberText.transform.position, array2[0], array2[1], array2[2], ref vector, ref vector2);
				}
			}
		}
		if (minLocalPadding.sqrMagnitude > 0f)
		{
			vector -= minLocalPadding;
		}
		if (maxLocalPadding.sqrMagnitude > 0f)
		{
			vector2 += maxLocalPadding;
		}
		Matrix4x4 localToWorldMatrix3 = go.transform.localToWorldMatrix;
		Matrix4x4 matrix4x3 = localToWorldMatrix3;
		matrix4x3.SetColumn(3, Vector4.zero);
		Vector3 vector4 = (localToWorldMatrix3 * vector2 + localToWorldMatrix3 * vector) * 0.5f;
		Vector3 vector5 = (vector2 - vector) * 0.5f;
		return new OrientedBounds
		{
			Extents = new Vector3[]
			{
				matrix4x3 * new Vector3(vector5.x, 0f, 0f),
				matrix4x3 * new Vector3(0f, vector5.y, 0f),
				matrix4x3 * new Vector3(0f, 0f, vector5.z)
			},
			Origin = vector4,
			CenterOffset = go.transform.position - vector4
		};
	}

	// Token: 0x06001C6D RID: 7277 RVA: 0x000862BC File Offset: 0x000844BC
	public static bool CanComputeOrientedWorldBounds(GameObject go, bool includeAllChildren = true)
	{
		return TransformUtil.CanComputeOrientedWorldBounds(go, true, includeAllChildren);
	}

	// Token: 0x06001C6E RID: 7278 RVA: 0x000862C6 File Offset: 0x000844C6
	public static bool CanComputeOrientedWorldBounds(GameObject go, List<GameObject> ignoreMeshes, bool includeAllChildren = true)
	{
		return TransformUtil.CanComputeOrientedWorldBounds(go, true, ignoreMeshes, includeAllChildren);
	}

	// Token: 0x06001C6F RID: 7279 RVA: 0x000862D1 File Offset: 0x000844D1
	public static bool CanComputeOrientedWorldBounds(GameObject go, bool includeUberText, bool includeAllChildren = true)
	{
		return TransformUtil.CanComputeOrientedWorldBounds(go, includeUberText, null, includeAllChildren);
	}

	// Token: 0x06001C70 RID: 7280 RVA: 0x000862DC File Offset: 0x000844DC
	public static bool CanComputeOrientedWorldBounds(GameObject go, bool includeUberText, List<GameObject> ignoreMeshes, bool includeAllChildren = true)
	{
		if (go == null || !go.activeSelf)
		{
			return false;
		}
		List<MeshFilter> componentsWithIgnore = TransformUtil.GetComponentsWithIgnore<MeshFilter>(go, ignoreMeshes, includeAllChildren);
		if (componentsWithIgnore != null && componentsWithIgnore.Count > 0)
		{
			return true;
		}
		if (includeUberText)
		{
			List<UberText> componentsWithIgnore2 = TransformUtil.GetComponentsWithIgnore<UberText>(go, ignoreMeshes, includeAllChildren);
			return componentsWithIgnore2 != null && componentsWithIgnore2.Count > 0;
		}
		return false;
	}

	// Token: 0x06001C71 RID: 7281 RVA: 0x00086344 File Offset: 0x00084544
	public static List<T> GetComponentsWithIgnore<T>(GameObject obj, List<GameObject> ignoreObjects, bool includeAllChildren = true) where T : Component
	{
		List<T> list = new List<T>();
		if (includeAllChildren)
		{
			obj.GetComponentsInChildren<T>(list);
		}
		T component = obj.GetComponent<T>();
		if (component != null)
		{
			list.Add(component);
		}
		if (ignoreObjects != null && ignoreObjects.Count > 0)
		{
			T[] array = list.ToArray();
			list.Clear();
			foreach (T t in array)
			{
				bool flag = true;
				foreach (GameObject gameObject in ignoreObjects)
				{
					if (gameObject == null || t.transform == gameObject.transform || t.transform.IsChildOf(gameObject.transform))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					list.Add(t);
				}
			}
		}
		return list;
	}

	// Token: 0x06001C72 RID: 7282 RVA: 0x00086470 File Offset: 0x00084670
	public static Vector3[] GetBoundCorners(Vector3 origin, Vector3 xExtent, Vector3 yExtent, Vector3 zExtent)
	{
		Vector3 vector = origin + xExtent;
		Vector3 vector2 = origin - xExtent;
		Vector3 vector3 = yExtent + zExtent;
		Vector3 vector4 = yExtent - zExtent;
		Vector3 vector5 = -yExtent + zExtent;
		Vector3 vector6 = -yExtent - zExtent;
		return new Vector3[]
		{
			vector + vector3,
			vector + vector4,
			vector + vector5,
			vector + vector6,
			vector2 - vector3,
			vector2 - vector4,
			vector2 - vector5,
			vector2 - vector6
		};
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x0008655C File Offset: 0x0008475C
	public static void GetBoundsMinMax(Vector3 origin, Vector3 xExtent, Vector3 yExtent, Vector3 zExtent, ref Vector3 min, ref Vector3 max)
	{
		Vector3[] boundCorners = TransformUtil.GetBoundCorners(origin, xExtent, yExtent, zExtent);
		for (int i = 0; i < boundCorners.Length; i++)
		{
			min.x = Mathf.Min(boundCorners[i].x, min.x);
			min.y = Mathf.Min(boundCorners[i].y, min.y);
			min.z = Mathf.Min(boundCorners[i].z, min.z);
			max.x = Mathf.Max(boundCorners[i].x, max.x);
			max.y = Mathf.Max(boundCorners[i].y, max.y);
			max.z = Mathf.Max(boundCorners[i].z, max.z);
		}
	}

	// Token: 0x06001C74 RID: 7284 RVA: 0x00086641 File Offset: 0x00084841
	public static void SetLocalScaleToWorldDimension(GameObject obj, params WorldDimensionIndex[] dimensions)
	{
		TransformUtil.SetLocalScaleToWorldDimension(obj, null, dimensions);
	}

	// Token: 0x06001C75 RID: 7285 RVA: 0x0008664C File Offset: 0x0008484C
	public static void SetLocalScaleToWorldDimension(GameObject obj, List<GameObject> ignoreMeshes, params WorldDimensionIndex[] dimensions)
	{
		Vector3 localScale = obj.transform.localScale;
		OrientedBounds orientedBounds = TransformUtil.ComputeOrientedWorldBounds(obj, ignoreMeshes, true);
		for (int i = 0; i < dimensions.Length; i++)
		{
			float num = orientedBounds.Extents[dimensions[i].Index].magnitude * 2f;
			ref Vector3 ptr = ref localScale;
			int index;
			int num2 = index = dimensions[i].Index;
			float num3 = ptr[index];
			localScale[num2] = num3 * ((num > Mathf.Epsilon) ? (dimensions[i].Dimension / num) : 0.001f);
			if (Mathf.Abs(localScale[dimensions[i].Index]) < 0.001f)
			{
				localScale[dimensions[i].Index] = 0.001f;
			}
		}
		obj.transform.localScale = localScale;
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x00086738 File Offset: 0x00084938
	public static void SetPoint(Component src, Anchor srcAnchor, Component dst, Anchor dstAnchor)
	{
		TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, false);
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x00086768 File Offset: 0x00084968
	public static void SetPoint(Component src, Anchor srcAnchor, GameObject dst, Anchor dstAnchor)
	{
		TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, false);
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x00086794 File Offset: 0x00084994
	public static void SetPoint(GameObject src, Anchor srcAnchor, Component dst, Anchor dstAnchor)
	{
		TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, false);
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x000867C0 File Offset: 0x000849C0
	public static void SetPoint(GameObject src, Anchor srcAnchor, GameObject dst, Anchor dstAnchor)
	{
		TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, false);
	}

	// Token: 0x06001C7A RID: 7290 RVA: 0x000867E8 File Offset: 0x000849E8
	public static void SetPoint(Component src, Anchor srcAnchor, Component dst, Anchor dstAnchor, bool includeInactive)
	{
		TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, includeInactive);
	}

	// Token: 0x06001C7B RID: 7291 RVA: 0x0008681C File Offset: 0x00084A1C
	public static void SetPoint(Component src, Anchor srcAnchor, GameObject dst, Anchor dstAnchor, bool includeInactive)
	{
		TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, includeInactive);
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x00086848 File Offset: 0x00084A48
	public static void SetPoint(GameObject src, Anchor srcAnchor, Component dst, Anchor dstAnchor, bool includeInactive)
	{
		TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, includeInactive);
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x00086874 File Offset: 0x00084A74
	public static void SetPoint(GameObject src, Anchor srcAnchor, GameObject dst, Anchor dstAnchor, bool includeInactive)
	{
		TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), Vector3.zero, includeInactive);
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x0008689C File Offset: 0x00084A9C
	public static void SetPoint(Component src, Anchor srcAnchor, Component dst, Anchor dstAnchor, Vector3 offset)
	{
		TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), offset, false);
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x000868CC File Offset: 0x00084ACC
	public static void SetPoint(Component src, Anchor srcAnchor, GameObject dst, Anchor dstAnchor, Vector3 offset)
	{
		TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), offset, false);
	}

	// Token: 0x06001C80 RID: 7296 RVA: 0x000868F4 File Offset: 0x00084AF4
	public static void SetPoint(GameObject src, Anchor srcAnchor, Component dst, Anchor dstAnchor, Vector3 offset)
	{
		TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), offset, false);
	}

	// Token: 0x06001C81 RID: 7297 RVA: 0x0008691C File Offset: 0x00084B1C
	public static void SetPoint(GameObject src, Anchor srcAnchor, GameObject dst, Anchor dstAnchor, Vector3 offset)
	{
		TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), offset, false);
	}

	// Token: 0x06001C82 RID: 7298 RVA: 0x00086940 File Offset: 0x00084B40
	public static void SetPoint(Component src, Anchor srcAnchor, Component dst, Anchor dstAnchor, Vector3 offset, bool includeInactive)
	{
		TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), offset, includeInactive);
	}

	// Token: 0x06001C83 RID: 7299 RVA: 0x00086970 File Offset: 0x00084B70
	public static void SetPoint(Component src, Anchor srcAnchor, GameObject dst, Anchor dstAnchor, Vector3 offset, bool includeInactive)
	{
		TransformUtil.SetPoint(src.gameObject, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), offset, includeInactive);
	}

	// Token: 0x06001C84 RID: 7300 RVA: 0x0008699C File Offset: 0x00084B9C
	public static void SetPoint(GameObject src, Anchor srcAnchor, Component dst, Anchor dstAnchor, Vector3 offset, bool includeInactive)
	{
		TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst.gameObject, TransformUtil.GetUnitAnchor(dstAnchor), offset, includeInactive);
	}

	// Token: 0x06001C85 RID: 7301 RVA: 0x000869C8 File Offset: 0x00084BC8
	public static void SetPoint(GameObject src, Anchor srcAnchor, GameObject dst, Anchor dstAnchor, Vector3 offset, bool includeInactive)
	{
		TransformUtil.SetPoint(src, TransformUtil.GetUnitAnchor(srcAnchor), dst, TransformUtil.GetUnitAnchor(dstAnchor), offset, includeInactive);
	}

	// Token: 0x06001C86 RID: 7302 RVA: 0x000869EC File Offset: 0x00084BEC
	public static void SetPoint(Component self, Vector3 selfUnitAnchor, Component relative, Vector3 relativeUnitAnchor)
	{
		TransformUtil.SetPoint(self.gameObject, selfUnitAnchor, relative.gameObject, relativeUnitAnchor, Vector3.zero, false);
	}

	// Token: 0x06001C87 RID: 7303 RVA: 0x00086A14 File Offset: 0x00084C14
	public static void SetPoint(Component self, Vector3 selfUnitAnchor, GameObject relative, Vector3 relativeUnitAnchor)
	{
		TransformUtil.SetPoint(self.gameObject, selfUnitAnchor, relative, relativeUnitAnchor, Vector3.zero, false);
	}

	// Token: 0x06001C88 RID: 7304 RVA: 0x00086A38 File Offset: 0x00084C38
	public static void SetPoint(GameObject self, Vector3 selfUnitAnchor, Component relative, Vector3 relativeUnitAnchor)
	{
		TransformUtil.SetPoint(self, selfUnitAnchor, relative.gameObject, relativeUnitAnchor, Vector3.zero, false);
	}

	// Token: 0x06001C89 RID: 7305 RVA: 0x00086A59 File Offset: 0x00084C59
	public static void SetPoint(GameObject self, Vector3 selfUnitAnchor, GameObject relative, Vector3 relativeUnitAnchor)
	{
		TransformUtil.SetPoint(self, selfUnitAnchor, relative, relativeUnitAnchor, Vector3.zero, false);
	}

	// Token: 0x06001C8A RID: 7306 RVA: 0x00086A6C File Offset: 0x00084C6C
	public static void SetPoint(Component self, Vector3 selfUnitAnchor, Component relative, Vector3 relativeUnitAnchor, Vector3 offset)
	{
		TransformUtil.SetPoint(self.gameObject, selfUnitAnchor, relative.gameObject, relativeUnitAnchor, offset, false);
	}

	// Token: 0x06001C8B RID: 7307 RVA: 0x00086A90 File Offset: 0x00084C90
	public static void SetPoint(Component self, Vector3 selfUnitAnchor, GameObject relative, Vector3 relativeUnitAnchor, Vector3 offset)
	{
		TransformUtil.SetPoint(self.gameObject, selfUnitAnchor, relative, relativeUnitAnchor, offset, false);
	}

	// Token: 0x06001C8C RID: 7308 RVA: 0x00086AB0 File Offset: 0x00084CB0
	public static void SetPoint(GameObject self, Vector3 selfUnitAnchor, Component relative, Vector3 relativeUnitAnchor, Vector3 offset)
	{
		TransformUtil.SetPoint(self, selfUnitAnchor, relative.gameObject, relativeUnitAnchor, offset, false);
	}

	// Token: 0x06001C8D RID: 7309 RVA: 0x00086ACE File Offset: 0x00084CCE
	public static void SetPoint(GameObject self, Vector3 selfUnitAnchor, GameObject relative, Vector3 relativeUnitAnchor, Vector3 offset)
	{
		TransformUtil.SetPoint(self, selfUnitAnchor, relative, relativeUnitAnchor, offset, false);
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x00086ADC File Offset: 0x00084CDC
	public static void SetPoint(Component self, Vector3 selfUnitAnchor, Component relative, Vector3 relativeUnitAnchor, Vector3 offset, bool includeInactive)
	{
		TransformUtil.SetPoint(self.gameObject, selfUnitAnchor, relative.gameObject, relativeUnitAnchor, offset, includeInactive);
	}

	// Token: 0x06001C8F RID: 7311 RVA: 0x00086B00 File Offset: 0x00084D00
	public static void SetPoint(Component self, Vector3 selfUnitAnchor, GameObject relative, Vector3 relativeUnitAnchor, Vector3 offset, bool includeInactive)
	{
		TransformUtil.SetPoint(self.gameObject, selfUnitAnchor, relative, relativeUnitAnchor, offset, includeInactive);
	}

	// Token: 0x06001C90 RID: 7312 RVA: 0x00086B20 File Offset: 0x00084D20
	public static void SetPoint(GameObject self, Vector3 selfUnitAnchor, Component relative, Vector3 relativeUnitAnchor, Vector3 offset, bool includeInactive)
	{
		TransformUtil.SetPoint(self, selfUnitAnchor, relative.gameObject, relativeUnitAnchor, offset, includeInactive);
	}

	// Token: 0x06001C91 RID: 7313 RVA: 0x00086B40 File Offset: 0x00084D40
	public static void SetPoint(GameObject self, Vector3 selfUnitAnchor, GameObject relative, Vector3 relativeUnitAnchor, Vector3 offset, bool includeInactive)
	{
		Bounds bounds = TransformUtil.ComputeSetPointBounds(self, includeInactive);
		Bounds bounds2 = TransformUtil.ComputeSetPointBounds(relative, includeInactive);
		Vector3 vector = TransformUtil.ComputeWorldPoint(bounds, selfUnitAnchor);
		Vector3 vector2 = TransformUtil.ComputeWorldPoint(bounds2, relativeUnitAnchor);
		Vector3 vector3;
		vector3..ctor(vector2.x - vector.x + offset.x, vector2.y - vector.y + offset.y, vector2.z - vector.z + offset.z);
		self.transform.Translate(vector3, 0);
	}

	// Token: 0x06001C92 RID: 7314 RVA: 0x00086BC9 File Offset: 0x00084DC9
	public static Bounds GetBoundsOfChildren(Component c)
	{
		return TransformUtil.GetBoundsOfChildren(c.gameObject, false);
	}

	// Token: 0x06001C93 RID: 7315 RVA: 0x00086BD7 File Offset: 0x00084DD7
	public static Bounds GetBoundsOfChildren(GameObject go)
	{
		return TransformUtil.GetBoundsOfChildren(go, false);
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x00086BE0 File Offset: 0x00084DE0
	public static Bounds GetBoundsOfChildren(Component c, bool includeInactive)
	{
		return TransformUtil.GetBoundsOfChildren(c.gameObject, includeInactive);
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x00086BF0 File Offset: 0x00084DF0
	public static Bounds GetBoundsOfChildren(GameObject go, bool includeInactive)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(includeInactive);
		if (componentsInChildren.Length == 0)
		{
			return new Bounds(go.transform.position, Vector3.zero);
		}
		Bounds bounds = componentsInChildren[0].bounds;
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			Renderer renderer = componentsInChildren[i];
			Bounds bounds2 = renderer.bounds;
			Vector3 vector = Vector3.Max(bounds2.max, bounds.max);
			Vector3 vector2 = Vector3.Min(bounds2.min, bounds.min);
			bounds.SetMinMax(vector2, vector);
		}
		return bounds;
	}

	// Token: 0x06001C96 RID: 7318 RVA: 0x00086C84 File Offset: 0x00084E84
	public static Vector3 Divide(Vector3 v1, Vector3 v2)
	{
		return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x00086CC4 File Offset: 0x00084EC4
	public static Vector3 Multiply(Vector3 v1, Vector3 v2)
	{
		return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x00086D04 File Offset: 0x00084F04
	public static void SetLocalPosX(GameObject go, float x)
	{
		Transform transform = go.transform;
		transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x00086D40 File Offset: 0x00084F40
	public static void SetLocalPosX(Component component, float x)
	{
		Transform transform = component.transform;
		transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
	}

	// Token: 0x06001C9A RID: 7322 RVA: 0x00086D7C File Offset: 0x00084F7C
	public static void SetLocalPosY(GameObject go, float y)
	{
		Transform transform = go.transform;
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
	}

	// Token: 0x06001C9B RID: 7323 RVA: 0x00086DB8 File Offset: 0x00084FB8
	public static void SetLocalPosY(Component component, float y)
	{
		Transform transform = component.transform;
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
	}

	// Token: 0x06001C9C RID: 7324 RVA: 0x00086DF4 File Offset: 0x00084FF4
	public static void SetLocalPosZ(GameObject go, float z)
	{
		Transform transform = go.transform;
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
	}

	// Token: 0x06001C9D RID: 7325 RVA: 0x00086E30 File Offset: 0x00085030
	public static void SetLocalPosZ(Component component, float z)
	{
		Transform transform = component.transform;
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
	}

	// Token: 0x06001C9E RID: 7326 RVA: 0x00086E6C File Offset: 0x0008506C
	public static void SetPosX(GameObject go, float x)
	{
		Transform transform = go.transform;
		transform.position = new Vector3(x, transform.position.y, transform.position.z);
	}

	// Token: 0x06001C9F RID: 7327 RVA: 0x00086EA8 File Offset: 0x000850A8
	public static void SetPosX(Component component, float x)
	{
		Transform transform = component.transform;
		transform.position = new Vector3(x, transform.position.y, transform.position.z);
	}

	// Token: 0x06001CA0 RID: 7328 RVA: 0x00086EE4 File Offset: 0x000850E4
	public static void SetPosY(GameObject go, float y)
	{
		Transform transform = go.transform;
		transform.position = new Vector3(transform.position.x, y, transform.position.z);
	}

	// Token: 0x06001CA1 RID: 7329 RVA: 0x00086F20 File Offset: 0x00085120
	public static void SetPosY(Component component, float y)
	{
		Transform transform = component.transform;
		transform.position = new Vector3(transform.position.x, y, transform.position.z);
	}

	// Token: 0x06001CA2 RID: 7330 RVA: 0x00086F5C File Offset: 0x0008515C
	public static void SetPosZ(GameObject go, float z)
	{
		Transform transform = go.transform;
		transform.position = new Vector3(transform.position.x, transform.position.y, z);
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x00086F98 File Offset: 0x00085198
	public static void SetPosZ(Component component, float z)
	{
		Transform transform = component.transform;
		transform.position = new Vector3(transform.position.x, transform.position.y, z);
	}

	// Token: 0x06001CA4 RID: 7332 RVA: 0x00086FD4 File Offset: 0x000851D4
	public static void SetLocalEulerAngleX(GameObject go, float x)
	{
		Transform transform = go.transform;
		transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
	}

	// Token: 0x06001CA5 RID: 7333 RVA: 0x00087010 File Offset: 0x00085210
	public static void SetLocalEulerAngleX(Component c, float x)
	{
		Transform transform = c.transform;
		transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
	}

	// Token: 0x06001CA6 RID: 7334 RVA: 0x0008704C File Offset: 0x0008524C
	public static void SetLocalEulerAngleY(GameObject go, float y)
	{
		Transform transform = go.transform;
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x00087088 File Offset: 0x00085288
	public static void SetLocalEulerAngleY(Component c, float y)
	{
		Transform transform = c.transform;
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
	}

	// Token: 0x06001CA8 RID: 7336 RVA: 0x000870C4 File Offset: 0x000852C4
	public static void SetLocalEulerAngleZ(GameObject go, float z)
	{
		Transform transform = go.transform;
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
	}

	// Token: 0x06001CA9 RID: 7337 RVA: 0x00087100 File Offset: 0x00085300
	public static void SetLocalEulerAngleZ(Component c, float z)
	{
		Transform transform = c.transform;
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x0008713C File Offset: 0x0008533C
	public static void SetEulerAngleX(GameObject go, float x)
	{
		Transform transform = go.transform;
		transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);
	}

	// Token: 0x06001CAB RID: 7339 RVA: 0x00087178 File Offset: 0x00085378
	public static void SetEulerAngleX(Component c, float x)
	{
		Transform transform = c.transform;
		transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);
	}

	// Token: 0x06001CAC RID: 7340 RVA: 0x000871B4 File Offset: 0x000853B4
	public static void SetEulerAngleY(GameObject go, float y)
	{
		Transform transform = go.transform;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x000871F0 File Offset: 0x000853F0
	public static void SetEulerAngleY(Component c, float y)
	{
		Transform transform = c.transform;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x0008722C File Offset: 0x0008542C
	public static void SetEulerAngleZ(GameObject go, float z)
	{
		Transform transform = go.transform;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, z);
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x00087268 File Offset: 0x00085468
	public static void SetEulerAngleZ(Component c, float z)
	{
		Transform transform = c.transform;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, z);
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x000872A4 File Offset: 0x000854A4
	public static void SetLocalScaleX(Component component, float x)
	{
		Transform transform = component.transform;
		transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x000872E0 File Offset: 0x000854E0
	public static void SetLocalScaleX(GameObject go, float x)
	{
		Transform transform = go.transform;
		transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x0008731C File Offset: 0x0008551C
	public static void SetLocalScaleY(Component component, float y)
	{
		Transform transform = component.transform;
		transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x00087358 File Offset: 0x00085558
	public static void SetLocalScaleY(GameObject go, float y)
	{
		Transform transform = go.transform;
		transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
	}

	// Token: 0x06001CB4 RID: 7348 RVA: 0x00087394 File Offset: 0x00085594
	public static void SetLocalScaleZ(Component component, float z)
	{
		Transform transform = component.transform;
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
	}

	// Token: 0x06001CB5 RID: 7349 RVA: 0x000873D0 File Offset: 0x000855D0
	public static void SetLocalScaleZ(GameObject go, float z)
	{
		Transform transform = go.transform;
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
	}

	// Token: 0x06001CB6 RID: 7350 RVA: 0x0008740C File Offset: 0x0008560C
	public static void SetLocalScaleXY(Component component, float x, float y)
	{
		Transform transform = component.transform;
		transform.localScale = new Vector3(x, y, transform.localScale.z);
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x0008743C File Offset: 0x0008563C
	public static void SetLocalScaleXY(GameObject go, float x, float y)
	{
		Transform transform = go.transform;
		transform.localScale = new Vector3(x, y, transform.localScale.z);
	}

	// Token: 0x06001CB8 RID: 7352 RVA: 0x0008746C File Offset: 0x0008566C
	public static void SetLocalScaleXY(Component component, Vector2 v)
	{
		Transform transform = component.transform;
		transform.localScale = new Vector3(v.x, v.y, transform.localScale.z);
	}

	// Token: 0x06001CB9 RID: 7353 RVA: 0x000874A8 File Offset: 0x000856A8
	public static void SetLocalScaleXY(GameObject go, Vector2 v)
	{
		Transform transform = go.transform;
		transform.localScale = new Vector3(v.x, v.y, transform.localScale.z);
	}

	// Token: 0x06001CBA RID: 7354 RVA: 0x000874E4 File Offset: 0x000856E4
	public static void SetLocalScaleXZ(Component component, float x, float z)
	{
		Transform transform = component.transform;
		transform.localScale = new Vector3(x, transform.localScale.y, z);
	}

	// Token: 0x06001CBB RID: 7355 RVA: 0x00087514 File Offset: 0x00085714
	public static void SetLocalScaleXZ(GameObject go, float x, float z)
	{
		Transform transform = go.transform;
		transform.localScale = new Vector3(x, transform.localScale.y, z);
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x00087544 File Offset: 0x00085744
	public static void SetLocalScaleXZ(Component component, Vector2 v)
	{
		Transform transform = component.transform;
		transform.localScale = new Vector3(v.x, transform.localScale.y, v.y);
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x00087580 File Offset: 0x00085780
	public static void SetLocalScaleXZ(GameObject go, Vector2 v)
	{
		Transform transform = go.transform;
		transform.localScale = new Vector3(v.x, transform.localScale.y, v.y);
	}

	// Token: 0x06001CBE RID: 7358 RVA: 0x000875BC File Offset: 0x000857BC
	public static void SetLocalScaleYZ(Component component, float y, float z)
	{
		Transform transform = component.transform;
		transform.localScale = new Vector3(transform.localScale.x, y, z);
	}

	// Token: 0x06001CBF RID: 7359 RVA: 0x000875EC File Offset: 0x000857EC
	public static void SetLocalScaleYZ(GameObject go, float y, float z)
	{
		Transform transform = go.transform;
		transform.localScale = new Vector3(transform.localScale.x, y, z);
	}

	// Token: 0x06001CC0 RID: 7360 RVA: 0x0008761C File Offset: 0x0008581C
	public static void SetLocalScaleYZ(Component component, Vector2 v)
	{
		Transform transform = component.transform;
		transform.localScale = new Vector3(transform.localScale.x, v.x, v.y);
	}

	// Token: 0x06001CC1 RID: 7361 RVA: 0x00087658 File Offset: 0x00085858
	public static void SetLocalScaleYZ(GameObject go, Vector2 v)
	{
		Transform transform = go.transform;
		transform.localScale = new Vector3(transform.localScale.x, v.x, v.y);
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x00087694 File Offset: 0x00085894
	public static void Identity(Component c)
	{
		c.transform.localScale = Vector3.one;
		c.transform.localRotation = Quaternion.identity;
		c.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x000876D4 File Offset: 0x000858D4
	public static void Identity(GameObject go)
	{
		go.transform.localScale = Vector3.one;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x00087711 File Offset: 0x00085911
	public static void CopyLocal(Component destination, Component source)
	{
		TransformUtil.CopyLocal(destination.gameObject, source.gameObject);
	}

	// Token: 0x06001CC5 RID: 7365 RVA: 0x00087724 File Offset: 0x00085924
	public static void CopyLocal(Component destination, GameObject source)
	{
		TransformUtil.CopyLocal(destination.gameObject, source);
	}

	// Token: 0x06001CC6 RID: 7366 RVA: 0x00087732 File Offset: 0x00085932
	public static void CopyLocal(GameObject destination, Component source)
	{
		TransformUtil.CopyLocal(destination, source.gameObject);
	}

	// Token: 0x06001CC7 RID: 7367 RVA: 0x00087740 File Offset: 0x00085940
	public static void CopyLocal(GameObject destination, GameObject source)
	{
		destination.transform.localScale = source.transform.localScale;
		destination.transform.localRotation = source.transform.localRotation;
		destination.transform.localPosition = source.transform.localPosition;
	}

	// Token: 0x06001CC8 RID: 7368 RVA: 0x0008778F File Offset: 0x0008598F
	public static void CopyLocal(Component destination, TransformProps source)
	{
		TransformUtil.CopyLocal(destination.gameObject, source);
	}

	// Token: 0x06001CC9 RID: 7369 RVA: 0x000877A0 File Offset: 0x000859A0
	public static void CopyLocal(GameObject destination, TransformProps source)
	{
		destination.transform.localScale = source.scale;
		destination.transform.localRotation = source.rotation;
		destination.transform.localPosition = source.position;
	}

	// Token: 0x06001CCA RID: 7370 RVA: 0x000877E0 File Offset: 0x000859E0
	public static void CopyLocal(TransformProps destination, Component source)
	{
		TransformUtil.CopyLocal(destination, source.gameObject);
	}

	// Token: 0x06001CCB RID: 7371 RVA: 0x000877F0 File Offset: 0x000859F0
	public static void CopyLocal(TransformProps destination, GameObject source)
	{
		destination.scale = source.transform.localScale;
		destination.rotation = source.transform.localRotation;
		destination.position = source.transform.localPosition;
	}

	// Token: 0x06001CCC RID: 7372 RVA: 0x00087830 File Offset: 0x00085A30
	public static void CopyWorld(Component destination, Component source)
	{
		TransformUtil.CopyWorld(destination.gameObject, source);
	}

	// Token: 0x06001CCD RID: 7373 RVA: 0x0008783E File Offset: 0x00085A3E
	public static void CopyWorld(Component destination, GameObject source)
	{
		TransformUtil.CopyWorld(destination.gameObject, source);
	}

	// Token: 0x06001CCE RID: 7374 RVA: 0x0008784C File Offset: 0x00085A4C
	public static void CopyWorld(GameObject destination, Component source)
	{
		TransformUtil.CopyWorld(destination, source.gameObject);
	}

	// Token: 0x06001CCF RID: 7375 RVA: 0x0008785C File Offset: 0x00085A5C
	public static void CopyWorld(GameObject destination, GameObject source)
	{
		TransformUtil.CopyWorldScale(destination, source);
		destination.transform.rotation = source.transform.rotation;
		destination.transform.position = source.transform.position;
	}

	// Token: 0x06001CD0 RID: 7376 RVA: 0x0008789C File Offset: 0x00085A9C
	public static void CopyWorld(Component destination, TransformProps source)
	{
		TransformUtil.CopyWorld(destination.gameObject, source);
	}

	// Token: 0x06001CD1 RID: 7377 RVA: 0x000878AC File Offset: 0x00085AAC
	public static void CopyWorld(GameObject destination, TransformProps source)
	{
		TransformUtil.SetWorldScale(destination, source.scale);
		destination.transform.rotation = source.rotation;
		destination.transform.position = source.position;
	}

	// Token: 0x06001CD2 RID: 7378 RVA: 0x000878E7 File Offset: 0x00085AE7
	public static void CopyWorld(TransformProps destination, Component source)
	{
		TransformUtil.CopyWorld(destination, source.gameObject);
	}

	// Token: 0x06001CD3 RID: 7379 RVA: 0x000878F8 File Offset: 0x00085AF8
	public static void CopyWorld(TransformProps destination, GameObject source)
	{
		destination.scale = TransformUtil.ComputeWorldScale(source);
		destination.rotation = source.transform.rotation;
		destination.position = source.transform.position;
	}

	// Token: 0x06001CD4 RID: 7380 RVA: 0x00087933 File Offset: 0x00085B33
	public static void CopyWorldScale(Component destination, Component source)
	{
		TransformUtil.CopyWorldScale(destination.gameObject, source.gameObject);
	}

	// Token: 0x06001CD5 RID: 7381 RVA: 0x00087946 File Offset: 0x00085B46
	public static void CopyWorldScale(Component destination, GameObject source)
	{
		TransformUtil.CopyWorldScale(destination.gameObject, source);
	}

	// Token: 0x06001CD6 RID: 7382 RVA: 0x00087954 File Offset: 0x00085B54
	public static void CopyWorldScale(GameObject destination, Component source)
	{
		TransformUtil.CopyWorldScale(destination, source.gameObject);
	}

	// Token: 0x06001CD7 RID: 7383 RVA: 0x00087964 File Offset: 0x00085B64
	public static void CopyWorldScale(GameObject destination, GameObject source)
	{
		Vector3 scale = TransformUtil.ComputeWorldScale(source);
		TransformUtil.SetWorldScale(destination, scale);
	}

	// Token: 0x06001CD8 RID: 7384 RVA: 0x0008797F File Offset: 0x00085B7F
	public static void SetWorldScale(Component destination, Vector3 scale)
	{
		TransformUtil.SetWorldScale(destination.gameObject, scale);
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x00087990 File Offset: 0x00085B90
	public static void SetWorldScale(GameObject destination, Vector3 scale)
	{
		if (destination.transform.parent != null)
		{
			Transform parent = destination.transform.parent;
			while (parent != null)
			{
				scale.Scale(TransformUtil.Vector3Reciprocal(parent.localScale));
				parent = parent.parent;
			}
		}
		destination.transform.localScale = scale;
	}

	// Token: 0x06001CDA RID: 7386 RVA: 0x000879F5 File Offset: 0x00085BF5
	public static Vector3 ComputeWorldScale(Component c)
	{
		return TransformUtil.ComputeWorldScale(c.gameObject);
	}

	// Token: 0x06001CDB RID: 7387 RVA: 0x00087A04 File Offset: 0x00085C04
	public static Vector3 ComputeWorldScale(GameObject go)
	{
		Vector3 localScale = go.transform.localScale;
		if (go.transform.parent != null)
		{
			Transform parent = go.transform.parent;
			while (parent != null)
			{
				localScale.Scale(parent.localScale);
				parent = parent.parent;
			}
		}
		return localScale;
	}

	// Token: 0x06001CDC RID: 7388 RVA: 0x00087A68 File Offset: 0x00085C68
	public static Vector3 Vector3Reciprocal(Vector3 source)
	{
		Vector3 result = source;
		if (result.x != 0f)
		{
			result.x = 1f / result.x;
		}
		if (result.y != 0f)
		{
			result.y = 1f / result.y;
		}
		if (result.z != 0f)
		{
			result.z = 1f / result.z;
		}
		return result;
	}

	// Token: 0x06001CDD RID: 7389 RVA: 0x00087AE7 File Offset: 0x00085CE7
	public static void OrientTo(GameObject source, GameObject target)
	{
		TransformUtil.OrientTo(source.transform, target.transform);
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x00087AFA File Offset: 0x00085CFA
	public static void OrientTo(GameObject source, Component target)
	{
		TransformUtil.OrientTo(source.transform, target.transform);
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x00087B0D File Offset: 0x00085D0D
	public static void OrientTo(Component source, GameObject target)
	{
		TransformUtil.OrientTo(source.transform, target.transform);
	}

	// Token: 0x06001CE0 RID: 7392 RVA: 0x00087B20 File Offset: 0x00085D20
	public static void OrientTo(Component source, Component target)
	{
		TransformUtil.OrientTo(source.transform, target.transform);
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x00087B34 File Offset: 0x00085D34
	public static void OrientTo(Transform source, Transform target)
	{
		TransformUtil.OrientTo(source, source.transform.position, target.transform.position);
	}

	// Token: 0x06001CE2 RID: 7394 RVA: 0x00087B60 File Offset: 0x00085D60
	public static void OrientTo(Transform source, Vector3 sourcePosition, Vector3 targetPosition)
	{
		Vector3 vector = targetPosition - sourcePosition;
		if (vector.sqrMagnitude > Mathf.Epsilon)
		{
			source.rotation = Quaternion.LookRotation(vector);
		}
	}

	// Token: 0x06001CE3 RID: 7395 RVA: 0x00087B94 File Offset: 0x00085D94
	public static Vector3 RandomVector3(Vector3 min, Vector3 max)
	{
		Vector3 result = default(Vector3);
		result.x = Random.Range(min.x, max.x);
		result.y = Random.Range(min.y, max.y);
		result.z = Random.Range(min.z, max.z);
		return result;
	}

	// Token: 0x06001CE4 RID: 7396 RVA: 0x00087BF8 File Offset: 0x00085DF8
	public static void AttachAndPreserveLocalTransform(Transform child, Transform parent)
	{
		TransformProps transformProps = new TransformProps();
		TransformUtil.CopyLocal(transformProps, child);
		child.parent = parent;
		TransformUtil.CopyLocal(child, transformProps);
	}

	// Token: 0x06001CE5 RID: 7397 RVA: 0x00087C20 File Offset: 0x00085E20
	public static Vector3 GetAspectRatioDependentPosition(Vector3 aspect3to2, Vector3 aspect16to9)
	{
		float num = TransformUtil.PhoneAspectRatioScale();
		float num2 = 1f - num;
		return aspect16to9 * num + aspect3to2 * num2;
	}

	// Token: 0x06001CE6 RID: 7398 RVA: 0x00087C50 File Offset: 0x00085E50
	public static float GetAspectRatioDependentValue(float aspect3to2, float aspect16to9)
	{
		float num = TransformUtil.PhoneAspectRatioScale();
		float num2 = 1f - num;
		return aspect16to9 * num + aspect3to2 * num2;
	}

	// Token: 0x06001CE7 RID: 7399 RVA: 0x00087C74 File Offset: 0x00085E74
	public static float PhoneAspectRatioScale()
	{
		float num = (float)Screen.width / (float)Screen.height;
		num = Mathf.Clamp(num, 1.5f, 1.7777778f);
		return (num - 1.5f) / 0.2777778f;
	}

	// Token: 0x04000EE1 RID: 3809
	private const float MIN_PHONE_ASPECT_RATIO = 1.5f;

	// Token: 0x04000EE2 RID: 3810
	private const float MAX_PHONE_ASPECT_RATIO = 1.7777778f;

	// Token: 0x04000EE3 RID: 3811
	private const float ASPECT_RANGE = 0.2777778f;
}
