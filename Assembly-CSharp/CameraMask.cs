using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000EF5 RID: 3829
[CustomEditClass]
public class CameraMask : MonoBehaviour
{
	// Token: 0x06007280 RID: 29312 RVA: 0x0021A45C File Offset: 0x0021865C
	public CameraMask()
	{
		List<GameLayer> list = new List<GameLayer>();
		list.Add(GameLayer.Default);
		list.Add(GameLayer.IgnoreFullScreenEffects);
		this.m_CullingMasks = list;
		base..ctor();
	}

	// Token: 0x06007281 RID: 29313 RVA: 0x0021A4A1 File Offset: 0x002186A1
	private void Update()
	{
		if (this.m_RealtimeUpdate)
		{
			this.UpdateCameraClipping();
		}
	}

	// Token: 0x06007282 RID: 29314 RVA: 0x0021A4B4 File Offset: 0x002186B4
	private void OnDisable()
	{
		if (this.m_MaskCamera != null && UniversalInputManager.Get() != null)
		{
			UniversalInputManager.Get().RemoveCameraMaskCamera(this.m_MaskCamera);
		}
		if (this.m_MaskCameraGameObject != null)
		{
			Object.Destroy(this.m_MaskCameraGameObject);
		}
		this.m_MaskCamera = null;
	}

	// Token: 0x06007283 RID: 29315 RVA: 0x0021A516 File Offset: 0x00218716
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06007284 RID: 29316 RVA: 0x0021A520 File Offset: 0x00218720
	private void OnDrawGizmos()
	{
		Matrix4x4 matrix = default(Matrix4x4);
		if (this.m_UpVector == CameraMask.CAMERA_MASK_UP_VECTOR.Z)
		{
			matrix.SetTRS(base.transform.position, Quaternion.identity, base.transform.lossyScale);
		}
		else
		{
			matrix.SetTRS(base.transform.position, Quaternion.Euler(90f, 0f, 0f), base.transform.lossyScale);
		}
		Gizmos.matrix = matrix;
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.m_Width, this.m_Height, 0f));
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x06007285 RID: 29317 RVA: 0x0021A5D3 File Offset: 0x002187D3
	[ContextMenu("UpdateMask")]
	public void UpdateMask()
	{
		this.UpdateCameraClipping();
	}

	// Token: 0x06007286 RID: 29318 RVA: 0x0021A5DC File Offset: 0x002187DC
	private bool Init()
	{
		if (this.m_MaskCamera != null)
		{
			return false;
		}
		if (this.m_MaskCameraGameObject != null)
		{
			Object.Destroy(this.m_MaskCameraGameObject);
		}
		this.m_RenderCamera = ((!this.m_UseCameraFromLayer) ? Camera.main : CameraUtils.FindFirstByLayer(this.m_CameraFromLayer));
		if (this.m_RenderCamera == null)
		{
			return false;
		}
		this.m_MaskCameraGameObject = new GameObject("MaskCamera");
		SceneUtils.SetLayer(this.m_MaskCameraGameObject, GameLayer.CameraMask);
		this.m_MaskCameraGameObject.transform.parent = this.m_RenderCamera.gameObject.transform;
		this.m_MaskCameraGameObject.transform.localPosition = Vector3.zero;
		this.m_MaskCameraGameObject.transform.localRotation = Quaternion.identity;
		this.m_MaskCameraGameObject.transform.localScale = Vector3.one;
		int num = GameLayer.CameraMask.LayerBit();
		foreach (GameLayer gameLayer in this.m_CullingMasks)
		{
			num |= gameLayer.LayerBit();
		}
		this.m_MaskCamera = this.m_MaskCameraGameObject.AddComponent<Camera>();
		this.m_MaskCamera.CopyFrom(this.m_RenderCamera);
		this.m_MaskCamera.clearFlags = 4;
		this.m_MaskCamera.cullingMask = num;
		this.m_MaskCamera.depth = this.m_RenderCamera.depth + 1f;
		if (this.m_ClipObjects == null)
		{
			this.m_ClipObjects = base.gameObject;
		}
		foreach (Transform transform in this.m_ClipObjects.GetComponentsInChildren<Transform>())
		{
			GameObject gameObject = transform.gameObject;
			if (!(gameObject == null))
			{
				SceneUtils.SetLayer(gameObject, GameLayer.CameraMask);
			}
		}
		this.UpdateCameraClipping();
		UniversalInputManager.Get().AddCameraMaskCamera(this.m_MaskCamera);
		return true;
	}

	// Token: 0x06007287 RID: 29319 RVA: 0x0021A800 File Offset: 0x00218A00
	private void UpdateCameraClipping()
	{
		if (this.m_RenderCamera == null && !this.Init())
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		if (this.m_UpVector == CameraMask.CAMERA_MASK_UP_VECTOR.Y)
		{
			zero..ctor(base.transform.position.x - this.m_Width * 0.5f * base.transform.lossyScale.x, base.transform.position.y, base.transform.position.z - this.m_Height * 0.5f * base.transform.lossyScale.z);
			zero2..ctor(base.transform.position.x + this.m_Width * 0.5f * base.transform.lossyScale.x, base.transform.position.y, base.transform.position.z + this.m_Height * 0.5f * base.transform.lossyScale.z);
		}
		else
		{
			zero..ctor(base.transform.position.x - this.m_Width * 0.5f * base.transform.lossyScale.x, base.transform.position.y - this.m_Height * 0.5f * base.transform.lossyScale.y, base.transform.position.z);
			zero2..ctor(base.transform.position.x + this.m_Width * 0.5f * base.transform.lossyScale.x, base.transform.position.y + this.m_Height * 0.5f * base.transform.lossyScale.y, base.transform.position.z);
		}
		Vector3 vector = this.m_RenderCamera.WorldToViewportPoint(zero);
		Vector3 vector2 = this.m_RenderCamera.WorldToViewportPoint(zero2);
		if (vector.x < 0f && vector2.x < 0f)
		{
			if (this.m_MaskCamera.enabled)
			{
				this.m_MaskCamera.enabled = false;
			}
			return;
		}
		if (vector.x > 1f && vector2.x > 1f)
		{
			if (this.m_MaskCamera.enabled)
			{
				this.m_MaskCamera.enabled = false;
			}
			return;
		}
		if (vector.y < 0f && vector2.y < 0f)
		{
			if (this.m_MaskCamera.enabled)
			{
				this.m_MaskCamera.enabled = false;
			}
			return;
		}
		if (vector.y > 1f && vector2.y > 1f)
		{
			if (this.m_MaskCamera.enabled)
			{
				this.m_MaskCamera.enabled = false;
			}
			return;
		}
		if (!this.m_MaskCamera.enabled)
		{
			this.m_MaskCamera.enabled = true;
		}
		Rect rect;
		rect..ctor(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
		if (rect.x < 0f)
		{
			rect.width += rect.x;
			rect.x = 0f;
		}
		if (rect.y < 0f)
		{
			rect.height += rect.y;
			rect.y = 0f;
		}
		if (rect.x > 1f)
		{
			rect.width -= rect.x;
			rect.x = 1f;
		}
		if (rect.y > 1f)
		{
			rect.height -= rect.y;
			rect.y = 1f;
		}
		rect.width = Mathf.Min(1f - rect.x, rect.width);
		rect.height = Mathf.Min(1f - rect.y, rect.height);
		this.m_MaskCamera.rect = new Rect(0f, 0f, 1f, 1f);
		this.m_MaskCamera.ResetProjectionMatrix();
		Matrix4x4 projectionMatrix = this.m_MaskCamera.projectionMatrix;
		this.m_MaskCamera.rect = rect;
		this.m_MaskCamera.projectionMatrix = Matrix4x4.TRS(new Vector3(-rect.x * 2f / rect.width, -rect.y * 2f / rect.height, 0f), Quaternion.identity, Vector3.one) * Matrix4x4.TRS(new Vector3(1f / rect.width - 1f, 1f / rect.height - 1f, 0f), Quaternion.identity, new Vector3(1f / rect.width, 1f / rect.height, 1f)) * projectionMatrix;
	}

	// Token: 0x04005C88 RID: 23688
	[CustomEditField(Sections = "Mask Settings")]
	public GameObject m_ClipObjects;

	// Token: 0x04005C89 RID: 23689
	[CustomEditField(Sections = "Mask Settings")]
	public CameraMask.CAMERA_MASK_UP_VECTOR m_UpVector;

	// Token: 0x04005C8A RID: 23690
	[CustomEditField(Sections = "Mask Settings")]
	public float m_Width = 1f;

	// Token: 0x04005C8B RID: 23691
	[CustomEditField(Sections = "Mask Settings")]
	public float m_Height = 1f;

	// Token: 0x04005C8C RID: 23692
	[CustomEditField(Sections = "Mask Settings")]
	public bool m_RealtimeUpdate;

	// Token: 0x04005C8D RID: 23693
	[CustomEditField(Sections = "Render Camera")]
	public bool m_UseCameraFromLayer;

	// Token: 0x04005C8E RID: 23694
	[CustomEditField(Sections = "Render Camera", Parent = "m_UseCameraFromLayer")]
	public GameLayer m_CameraFromLayer;

	// Token: 0x04005C8F RID: 23695
	[CustomEditField(Sections = "Render Camera")]
	public List<GameLayer> m_CullingMasks;

	// Token: 0x04005C90 RID: 23696
	private Camera m_RenderCamera;

	// Token: 0x04005C91 RID: 23697
	private Camera m_MaskCamera;

	// Token: 0x04005C92 RID: 23698
	private GameObject m_MaskCameraGameObject;

	// Token: 0x02000EF6 RID: 3830
	public enum CAMERA_MASK_UP_VECTOR
	{
		// Token: 0x04005C94 RID: 23700
		Y,
		// Token: 0x04005C95 RID: 23701
		Z
	}
}
