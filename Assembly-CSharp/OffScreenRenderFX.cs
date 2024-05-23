using System;
using UnityEngine;

// Token: 0x02000F1B RID: 3867
public class OffScreenRenderFX : MonoBehaviour
{
	// Token: 0x0600734F RID: 29519 RVA: 0x0021F860 File Offset: 0x0021DA60
	private void Start()
	{
		if (this.UseBounds)
		{
			Mesh mesh = base.GetComponent<MeshFilter>().mesh;
			this.RenderBounds = mesh.bounds;
		}
		this.Yoffset = OffScreenRenderFX.s_Yoffset;
		OffScreenRenderFX.s_Yoffset += 10f;
		this.PositionObjectToRender();
		this.CreateCamera();
		this.CreateRenderTexture();
		this.SetupCamera();
		this.SetupMaterial();
		base.GetComponent<Renderer>().enabled = true;
	}

	// Token: 0x06007350 RID: 29520 RVA: 0x0021F8D8 File Offset: 0x0021DAD8
	private void OnDestroy()
	{
		if (this.tempRenderBuffer != null && this.tempRenderBuffer.IsCreated())
		{
			RenderTexture.ReleaseTemporary(this.tempRenderBuffer);
		}
	}

	// Token: 0x06007351 RID: 29521 RVA: 0x0021F914 File Offset: 0x0021DB14
	private void CreateCamera()
	{
		if (this.offscreenFXCameraGO == null)
		{
			if (this.offscreenFXCamera != null)
			{
				Object.Destroy(this.offscreenFXCamera);
			}
			this.offscreenFXCameraGO = new GameObject();
			this.offscreenFXCamera = this.offscreenFXCameraGO.AddComponent<Camera>();
			this.offscreenFXCameraGO.name = base.name + "_OffScreenFXCamera";
			SceneUtils.SetHideFlags(this.offscreenFXCameraGO, 61);
			UniversalInputManager.Get().AddIgnoredCamera(this.offscreenFXCamera);
		}
	}

	// Token: 0x06007352 RID: 29522 RVA: 0x0021F9A4 File Offset: 0x0021DBA4
	private void SetupCamera()
	{
		this.offscreenFXCamera.orthographic = true;
		this.UpdateOffScreenCamera();
		this.offscreenFXCamera.transform.parent = base.transform;
		this.offscreenFXCamera.nearClipPlane = -this.AboveClip;
		this.offscreenFXCamera.farClipPlane = this.BelowClip;
		this.offscreenFXCamera.targetTexture = this.tempRenderBuffer;
		this.offscreenFXCamera.depth = Camera.main.depth - 1f;
		this.offscreenFXCamera.backgroundColor = Color.black;
		this.offscreenFXCamera.clearFlags = 2;
		this.offscreenFXCamera.rect = this.CameraRect;
		this.offscreenFXCamera.enabled = true;
	}

	// Token: 0x06007353 RID: 29523 RVA: 0x0021FA64 File Offset: 0x0021DC64
	private void UpdateOffScreenCamera()
	{
		if (this.ObjectToRender == null)
		{
			return;
		}
		if (this.ForceSize == 0f)
		{
			float num = base.transform.localScale.x;
			if (this.UseBounds)
			{
				num *= this.RenderBounds.size.x;
			}
			this.offscreenFXCamera.orthographicSize = num / 2f;
		}
		else
		{
			this.offscreenFXCamera.orthographicSize = this.ForceSize;
		}
		this.offscreenFXCameraGO.transform.position = this.ObjectToRender.transform.position;
		this.offscreenFXCameraGO.transform.rotation = this.ObjectToRender.transform.rotation;
		this.offscreenFXCameraGO.transform.Rotate(90f, 180f, 0f);
	}

	// Token: 0x06007354 RID: 29524 RVA: 0x0021FB4F File Offset: 0x0021DD4F
	private void CreateRenderTexture()
	{
		this.tempRenderBuffer = RenderTexture.GetTemporary(this.RenderResolutionX, this.RenderResolutionY);
	}

	// Token: 0x06007355 RID: 29525 RVA: 0x0021FB68 File Offset: 0x0021DD68
	private void SetupMaterial()
	{
		Material material = base.gameObject.GetComponent<Renderer>().material;
		material.mainTexture = this.tempRenderBuffer;
	}

	// Token: 0x06007356 RID: 29526 RVA: 0x0021FB94 File Offset: 0x0021DD94
	private void PositionObjectToRender()
	{
		Vector3 vector = Vector3.up * this.Yoffset;
		Vector3 vector2 = Vector3.right * OffScreenRenderFX.s_Xoffset;
		if (this.ObjectToRender != null)
		{
			this.ObjectToRender.transform.position += vector;
		}
		this.ObjectToRender.transform.position += vector2;
	}

	// Token: 0x04005DD9 RID: 24025
	private const int IgnoreLayer = 21;

	// Token: 0x04005DDA RID: 24026
	public GameObject ObjectToRender;

	// Token: 0x04005DDB RID: 24027
	public bool UseBounds = true;

	// Token: 0x04005DDC RID: 24028
	public int RenderResolutionX = 256;

	// Token: 0x04005DDD RID: 24029
	public int RenderResolutionY = 256;

	// Token: 0x04005DDE RID: 24030
	public float AboveClip = 1f;

	// Token: 0x04005DDF RID: 24031
	public float BelowClip = 1f;

	// Token: 0x04005DE0 RID: 24032
	public float ForceSize;

	// Token: 0x04005DE1 RID: 24033
	public Rect CameraRect = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04005DE2 RID: 24034
	private Camera offscreenFXCamera;

	// Token: 0x04005DE3 RID: 24035
	private GameObject offscreenFXCameraGO;

	// Token: 0x04005DE4 RID: 24036
	private RenderTexture tempRenderBuffer;

	// Token: 0x04005DE5 RID: 24037
	private float Yoffset;

	// Token: 0x04005DE6 RID: 24038
	private Bounds RenderBounds;

	// Token: 0x04005DE7 RID: 24039
	private static float s_Yoffset = 250f;

	// Token: 0x04005DE8 RID: 24040
	private static float s_Xoffset = 250f;
}
