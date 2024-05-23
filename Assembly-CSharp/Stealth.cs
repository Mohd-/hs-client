using System;
using UnityEngine;

// Token: 0x02000D8F RID: 3471
public class Stealth : MonoBehaviour
{
	// Token: 0x06006C46 RID: 27718 RVA: 0x001FDACF File Offset: 0x001FBCCF
	private void Start()
	{
		this.CreateCamera();
		this.CreateRenderTexture();
		this.SetupCamera();
		this.SetupMaterial();
	}

	// Token: 0x06006C47 RID: 27719 RVA: 0x001FDAE9 File Offset: 0x001FBCE9
	private void OnDestroy()
	{
		this.tempRenderBuffer.Release();
	}

	// Token: 0x06006C48 RID: 27720 RVA: 0x001FDAF6 File Offset: 0x001FBCF6
	private void CameraRender()
	{
		this.stealthCamera.Render();
	}

	// Token: 0x06006C49 RID: 27721 RVA: 0x001FDB04 File Offset: 0x001FBD04
	private void CreateCamera()
	{
		if (this.stealthCameraGO == null)
		{
			if (this.stealthCamera != null)
			{
				Object.Destroy(this.stealthCamera);
			}
			this.stealthCameraGO = new GameObject();
			this.stealthCamera = this.stealthCameraGO.AddComponent<Camera>();
			this.stealthCameraGO.name = base.name + "_StealthFXCamera";
			SceneUtils.SetHideFlags(this.stealthCameraGO, 61);
		}
	}

	// Token: 0x06006C4A RID: 27722 RVA: 0x001FDB84 File Offset: 0x001FBD84
	private void SetupCamera()
	{
		this.stealthCamera.orthographic = true;
		this.UpdateOffScreenCamera();
		this.stealthCamera.transform.parent = base.transform;
		this.stealthCamera.nearClipPlane = -this.AboveClip;
		this.stealthCamera.farClipPlane = this.BelowClip;
		this.stealthCamera.targetTexture = this.tempRenderBuffer;
		this.stealthCamera.depth = Camera.main.depth - 1f;
		this.stealthCamera.backgroundColor = Color.black;
		this.stealthCamera.clearFlags = 2;
		this.stealthCamera.cullingMask &= -2097153;
		this.stealthCamera.enabled = false;
	}

	// Token: 0x06006C4B RID: 27723 RVA: 0x001FDC48 File Offset: 0x001FBE48
	private void UpdateOffScreenCamera()
	{
		this.stealthCamera.orthographicSize = 1f;
		this.stealthCameraGO.transform.position = base.transform.position;
		this.stealthCameraGO.transform.rotation = base.transform.rotation;
		this.stealthCameraGO.transform.Rotate(90f, 180f, 0f);
	}

	// Token: 0x06006C4C RID: 27724 RVA: 0x001FDCBA File Offset: 0x001FBEBA
	private void CreateRenderTexture()
	{
		this.tempRenderBuffer = RenderTexture.GetTemporary(this.RenderResolutionX, this.RenderResolutionY);
	}

	// Token: 0x06006C4D RID: 27725 RVA: 0x001FDCD4 File Offset: 0x001FBED4
	private void SetupMaterial()
	{
		Material material = base.gameObject.GetComponent<Renderer>().material;
		material.mainTexture = this.tempRenderBuffer;
	}

	// Token: 0x040054DC RID: 21724
	private const int IgnoreLayer = 21;

	// Token: 0x040054DD RID: 21725
	public int RenderResolutionX = 256;

	// Token: 0x040054DE RID: 21726
	public int RenderResolutionY = 256;

	// Token: 0x040054DF RID: 21727
	public float AboveClip = 0.4f;

	// Token: 0x040054E0 RID: 21728
	public float BelowClip = 0.4f;

	// Token: 0x040054E1 RID: 21729
	private Camera stealthCamera;

	// Token: 0x040054E2 RID: 21730
	private GameObject stealthCameraGO;

	// Token: 0x040054E3 RID: 21731
	private RenderTexture tempRenderBuffer;
}
