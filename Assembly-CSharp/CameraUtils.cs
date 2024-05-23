using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000203 RID: 515
public class CameraUtils
{
	// Token: 0x06001E59 RID: 7769 RVA: 0x0008D324 File Offset: 0x0008B524
	public static Camera FindFirstByLayer(int layer)
	{
		return CameraUtils.FindFirstByLayerMask(1 << layer);
	}

	// Token: 0x06001E5A RID: 7770 RVA: 0x0008D336 File Offset: 0x0008B536
	public static Camera FindFirstByLayer(GameLayer layer)
	{
		return CameraUtils.FindFirstByLayerMask(layer.LayerBit());
	}

	// Token: 0x06001E5B RID: 7771 RVA: 0x0008D348 File Offset: 0x0008B548
	public static Camera FindFirstByLayerMask(LayerMask mask)
	{
		foreach (Camera camera in Camera.allCameras)
		{
			if ((camera.cullingMask & mask) != 0)
			{
				return camera;
			}
		}
		return null;
	}

	// Token: 0x06001E5C RID: 7772 RVA: 0x0008D388 File Offset: 0x0008B588
	public static void FindAllByLayer(int layer, List<Camera> cameras)
	{
		CameraUtils.FindAllByLayerMask(1 << layer, cameras);
	}

	// Token: 0x06001E5D RID: 7773 RVA: 0x0008D39B File Offset: 0x0008B59B
	public static void FindAllByLayer(GameLayer layer, List<Camera> cameras)
	{
		CameraUtils.FindAllByLayerMask(layer.LayerBit(), cameras);
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x0008D3B0 File Offset: 0x0008B5B0
	public static void FindAllByLayerMask(LayerMask mask, List<Camera> cameras)
	{
		foreach (Camera camera in Camera.allCameras)
		{
			if ((camera.cullingMask & mask) != 0)
			{
				cameras.Add(camera);
			}
		}
	}

	// Token: 0x06001E5F RID: 7775 RVA: 0x0008D3F4 File Offset: 0x0008B5F4
	public static Camera FindFullScreenEffectsCamera(bool activeOnly)
	{
		foreach (Camera camera in Camera.allCameras)
		{
			FullScreenEffects component = camera.GetComponent<FullScreenEffects>();
			if (!(component == null))
			{
				if (!activeOnly || component.isActive())
				{
					return camera;
				}
			}
		}
		return null;
	}

	// Token: 0x06001E60 RID: 7776 RVA: 0x0008D44C File Offset: 0x0008B64C
	public static LayerMask CreateLayerMask(List<Camera> cameras)
	{
		LayerMask layerMask = 0;
		foreach (Camera camera in cameras)
		{
			layerMask |= camera.cullingMask;
		}
		return layerMask;
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x0008D4B8 File Offset: 0x0008B6B8
	public static Plane CreateTopPlane(Camera camera)
	{
		Vector3 vector = camera.ViewportToWorldPoint(new Vector3(0f, 1f, camera.nearClipPlane));
		Vector3 vector2 = camera.ViewportToWorldPoint(new Vector3(1f, 1f, camera.nearClipPlane));
		Vector3 vector3 = camera.ViewportToWorldPoint(new Vector3(0f, 1f, camera.farClipPlane));
		Vector3 vector4 = Vector3.Cross(vector3 - vector, vector2 - vector);
		vector4.Normalize();
		Plane result;
		result..ctor(vector4, vector);
		return result;
	}

	// Token: 0x06001E62 RID: 7778 RVA: 0x0008D540 File Offset: 0x0008B740
	public static Plane CreateBottomPlane(Camera camera)
	{
		Vector3 vector = camera.ViewportToWorldPoint(new Vector3(0f, 0f, camera.nearClipPlane));
		Vector3 vector2 = camera.ViewportToWorldPoint(new Vector3(1f, 0f, camera.nearClipPlane));
		Vector3 vector3 = camera.ViewportToWorldPoint(new Vector3(0f, 0f, camera.farClipPlane));
		Vector3 vector4 = Vector3.Cross(vector3 - vector, vector2 - vector);
		vector4.Normalize();
		Plane result;
		result..ctor(vector4, vector);
		return result;
	}

	// Token: 0x06001E63 RID: 7779 RVA: 0x0008D5C8 File Offset: 0x0008B7C8
	public static Bounds GetNearClipBounds(Camera camera)
	{
		Vector3 vector = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.nearClipPlane));
		Vector3 vector2 = camera.ViewportToWorldPoint(new Vector3(0f, 0f, camera.nearClipPlane));
		Vector3 vector3 = camera.ViewportToWorldPoint(new Vector3(1f, 1f, camera.nearClipPlane));
		Vector3 vector4;
		vector4..ctor(vector3.x - vector2.x, vector3.y - vector2.y, vector3.z - vector2.z);
		return new Bounds(vector, vector4);
	}

	// Token: 0x06001E64 RID: 7780 RVA: 0x0008D664 File Offset: 0x0008B864
	public static Bounds GetFarClipBounds(Camera camera)
	{
		Vector3 vector = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.farClipPlane));
		Vector3 vector2 = camera.ViewportToWorldPoint(new Vector3(0f, 0f, camera.farClipPlane));
		Vector3 vector3 = camera.ViewportToWorldPoint(new Vector3(1f, 1f, camera.farClipPlane));
		Vector3 vector4;
		vector4..ctor(vector3.x - vector2.x, vector3.y - vector2.y, vector3.z - vector2.z);
		return new Bounds(vector, vector4);
	}

	// Token: 0x06001E65 RID: 7781 RVA: 0x0008D700 File Offset: 0x0008B900
	public static Rect CreateGUIViewportRect(Camera camera, Component topLeft, Component bottomRight)
	{
		return CameraUtils.CreateGUIViewportRect(camera, topLeft.transform.position, bottomRight.transform.position);
	}

	// Token: 0x06001E66 RID: 7782 RVA: 0x0008D72C File Offset: 0x0008B92C
	public static Rect CreateGUIViewportRect(Camera camera, GameObject topLeft, Component bottomRight)
	{
		return CameraUtils.CreateGUIViewportRect(camera, topLeft.transform.position, bottomRight.transform.position);
	}

	// Token: 0x06001E67 RID: 7783 RVA: 0x0008D758 File Offset: 0x0008B958
	public static Rect CreateGUIViewportRect(Camera camera, Component topLeft, GameObject bottomRight)
	{
		return CameraUtils.CreateGUIViewportRect(camera, topLeft.transform.position, bottomRight.transform.position);
	}

	// Token: 0x06001E68 RID: 7784 RVA: 0x0008D784 File Offset: 0x0008B984
	public static Rect CreateGUIViewportRect(Camera camera, GameObject topLeft, GameObject bottomRight)
	{
		return CameraUtils.CreateGUIViewportRect(camera, topLeft.transform.position, bottomRight.transform.position);
	}

	// Token: 0x06001E69 RID: 7785 RVA: 0x0008D7B0 File Offset: 0x0008B9B0
	public static Rect CreateGUIViewportRect(Camera camera, Vector3 worldTopLeft, Vector3 worldBottomRight)
	{
		Vector3 vector = camera.WorldToViewportPoint(worldTopLeft);
		Vector3 vector2 = camera.WorldToViewportPoint(worldBottomRight);
		Rect result;
		result..ctor(vector.x, 1f - vector.y, vector2.x - vector.x, vector.y - vector2.y);
		return result;
	}

	// Token: 0x06001E6A RID: 7786 RVA: 0x0008D808 File Offset: 0x0008BA08
	public static Rect CreateGUIScreenRect(Camera camera, Component topLeft, Component bottomRight)
	{
		return CameraUtils.CreateGUIScreenRect(camera, topLeft.transform.position, bottomRight.transform.position);
	}

	// Token: 0x06001E6B RID: 7787 RVA: 0x0008D834 File Offset: 0x0008BA34
	public static Rect CreateGUIScreenRect(Camera camera, GameObject topLeft, Component bottomRight)
	{
		return CameraUtils.CreateGUIScreenRect(camera, topLeft.transform.position, bottomRight.transform.position);
	}

	// Token: 0x06001E6C RID: 7788 RVA: 0x0008D860 File Offset: 0x0008BA60
	public static Rect CreateGUIScreenRect(Camera camera, Component topLeft, GameObject bottomRight)
	{
		return CameraUtils.CreateGUIScreenRect(camera, topLeft.transform.position, bottomRight.transform.position);
	}

	// Token: 0x06001E6D RID: 7789 RVA: 0x0008D88C File Offset: 0x0008BA8C
	public static Rect CreateGUIScreenRect(Camera camera, GameObject topLeft, GameObject bottomRight)
	{
		return CameraUtils.CreateGUIScreenRect(camera, topLeft.transform.position, bottomRight.transform.position);
	}

	// Token: 0x06001E6E RID: 7790 RVA: 0x0008D8B8 File Offset: 0x0008BAB8
	public static Rect CreateGUIScreenRect(Camera camera, Vector3 worldTopLeft, Vector3 worldBottomRight)
	{
		Vector3 vector = camera.WorldToScreenPoint(worldTopLeft);
		Vector3 vector2 = camera.WorldToScreenPoint(worldBottomRight);
		Rect result;
		result..ctor(vector.x, vector2.y, vector2.x - vector.x, vector.y - vector2.y);
		return result;
	}

	// Token: 0x06001E6F RID: 7791 RVA: 0x0008D90C File Offset: 0x0008BB0C
	public static bool Raycast(Camera camera, Vector3 screenPoint, out RaycastHit hitInfo)
	{
		hitInfo = default(RaycastHit);
		if (!camera.pixelRect.Contains(screenPoint))
		{
			return false;
		}
		Ray ray = camera.ScreenPointToRay(screenPoint);
		return Physics.Raycast(ray, ref hitInfo, camera.farClipPlane, camera.cullingMask);
	}

	// Token: 0x06001E70 RID: 7792 RVA: 0x0008D954 File Offset: 0x0008BB54
	public static bool Raycast(Camera camera, Vector3 screenPoint, LayerMask layerMask, out RaycastHit hitInfo)
	{
		hitInfo = default(RaycastHit);
		if (!camera.pixelRect.Contains(screenPoint))
		{
			return false;
		}
		Ray ray = camera.ScreenPointToRay(screenPoint);
		return Physics.Raycast(ray, ref hitInfo, camera.farClipPlane, layerMask);
	}

	// Token: 0x06001E71 RID: 7793 RVA: 0x0008D999 File Offset: 0x0008BB99
	public static GameObject CreateInputBlocker(Camera camera)
	{
		return CameraUtils.CreateInputBlocker(camera, string.Empty, null, null, 0f);
	}

	// Token: 0x06001E72 RID: 7794 RVA: 0x0008D9AD File Offset: 0x0008BBAD
	public static GameObject CreateInputBlocker(Camera camera, string name)
	{
		return CameraUtils.CreateInputBlocker(camera, name, null, null, 0f);
	}

	// Token: 0x06001E73 RID: 7795 RVA: 0x0008D9BD File Offset: 0x0008BBBD
	public static GameObject CreateInputBlocker(Camera camera, string name, Component parent)
	{
		return CameraUtils.CreateInputBlocker(camera, name, parent, parent, 0f);
	}

	// Token: 0x06001E74 RID: 7796 RVA: 0x0008D9CD File Offset: 0x0008BBCD
	public static GameObject CreateInputBlocker(Camera camera, string name, Component parent, float worldOffset)
	{
		return CameraUtils.CreateInputBlocker(camera, name, parent, parent, worldOffset);
	}

	// Token: 0x06001E75 RID: 7797 RVA: 0x0008D9D9 File Offset: 0x0008BBD9
	public static GameObject CreateInputBlocker(Camera camera, string name, Component parent, Component relative)
	{
		return CameraUtils.CreateInputBlocker(camera, name, parent, relative, 0f);
	}

	// Token: 0x06001E76 RID: 7798 RVA: 0x0008D9EC File Offset: 0x0008BBEC
	public static GameObject CreateInputBlocker(Camera camera, string name, Component parent, Component relative, float worldOffset)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.layer = camera.gameObject.layer;
		gameObject.transform.parent = ((!(parent == null)) ? parent.transform : null);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.rotation = Quaternion.Inverse(camera.transform.rotation);
		if (relative == null)
		{
			gameObject.transform.position = CameraUtils.GetPosInFrontOfCamera(camera, camera.nearClipPlane + worldOffset);
		}
		else
		{
			gameObject.transform.position = CameraUtils.GetPosInFrontOfCamera(camera, relative.transform.position, worldOffset);
		}
		Bounds farClipBounds = CameraUtils.GetFarClipBounds(camera);
		Vector3 vector;
		if (parent == null)
		{
			vector = Vector3.one;
		}
		else
		{
			vector = TransformUtil.ComputeWorldScale(parent);
		}
		Vector3 size = default(Vector3);
		size.x = farClipBounds.size.x / vector.x;
		if (farClipBounds.size.z > 0f)
		{
			size.y = farClipBounds.size.z / vector.z;
		}
		else
		{
			size.y = farClipBounds.size.y / vector.y;
		}
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.size = size;
		return gameObject;
	}

	// Token: 0x06001E77 RID: 7799 RVA: 0x0008DB62 File Offset: 0x0008BD62
	public static float ScreenToWorldDist(Camera camera, float screenDist)
	{
		return CameraUtils.ScreenToWorldDist(camera, screenDist, camera.nearClipPlane);
	}

	// Token: 0x06001E78 RID: 7800 RVA: 0x0008DB74 File Offset: 0x0008BD74
	public static float ScreenToWorldDist(Camera camera, float screenDist, float worldDist)
	{
		Vector3 vector = camera.ScreenToWorldPoint(new Vector3(0f, 0f, worldDist));
		return camera.ScreenToWorldPoint(new Vector3(screenDist, 0f, worldDist)).x - vector.x;
	}

	// Token: 0x06001E79 RID: 7801 RVA: 0x0008DBBC File Offset: 0x0008BDBC
	public static float ScreenToWorldDist(Camera camera, float screenDist, Vector3 worldPoint)
	{
		float worldDist = Vector3.Distance(camera.transform.position, worldPoint);
		return CameraUtils.ScreenToWorldDist(camera, screenDist, worldDist);
	}

	// Token: 0x06001E7A RID: 7802 RVA: 0x0008DBE4 File Offset: 0x0008BDE4
	public static Vector3 GetPosInFrontOfCamera(Camera camera, float worldDistance)
	{
		Vector3 vector = camera.transform.position + new Vector3(0f, 0f, worldDistance);
		float magnitude = camera.transform.InverseTransformPoint(vector).magnitude;
		Vector3 vector2;
		vector2..ctor(0f, 0f, magnitude);
		return camera.transform.TransformPoint(vector2);
	}

	// Token: 0x06001E7B RID: 7803 RVA: 0x0008DC49 File Offset: 0x0008BE49
	public static Vector3 GetPosInFrontOfCamera(Camera camera, Vector3 worldPoint)
	{
		return CameraUtils.GetPosInFrontOfCamera(camera, worldPoint, 0f);
	}

	// Token: 0x06001E7C RID: 7804 RVA: 0x0008DC58 File Offset: 0x0008BE58
	public static Vector3 GetPosInFrontOfCamera(Camera camera, Vector3 worldPoint, float worldOffset)
	{
		Vector3 position = camera.transform.position;
		Vector3 forward = camera.transform.forward;
		Plane plane;
		plane..ctor(-forward, worldPoint);
		float distanceToPoint = plane.GetDistanceToPoint(position);
		float num = distanceToPoint + worldOffset;
		Vector3 vector = num * forward;
		return position + vector;
	}

	// Token: 0x06001E7D RID: 7805 RVA: 0x0008DCAB File Offset: 0x0008BEAB
	public static Vector3 GetNearestPosInFrontOfCamera(Camera camera, float worldOffset = 0f)
	{
		return CameraUtils.GetPosInFrontOfCamera(camera, camera.nearClipPlane + worldOffset);
	}
}
