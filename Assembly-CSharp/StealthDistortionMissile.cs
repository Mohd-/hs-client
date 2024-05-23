using System;
using UnityEngine;

// Token: 0x02000D90 RID: 3472
public class StealthDistortionMissile : MonoBehaviour
{
	// Token: 0x06006C50 RID: 27728 RVA: 0x001FDD78 File Offset: 0x001FBF78
	private void Start()
	{
		this.Yoffset = StealthDistortionMissile.s_Yoffset;
		StealthDistortionMissile.s_Yoffset += 10f;
		this.PositionObjectToRender();
		this.CreateCameras();
		this.CreateRenderTextures();
		this.SetupCameras();
		this.SetupMaterial();
		base.GetComponent<Renderer>().enabled = true;
	}

	// Token: 0x06006C51 RID: 27729 RVA: 0x001FDDCA File Offset: 0x001FBFCA
	private void OnDestroy()
	{
		this.particleRenderBuffer.Release();
		this.boardRenderBuffer.Release();
	}

	// Token: 0x06006C52 RID: 27730 RVA: 0x001FDDE4 File Offset: 0x001FBFE4
	private void CreateCameras()
	{
		if (this.particleCameraGO == null)
		{
			if (this.particleCamera != null)
			{
				Object.Destroy(this.particleCamera);
			}
			this.particleCameraGO = new GameObject();
			this.particleCamera = this.particleCameraGO.AddComponent<Camera>();
			this.particleCameraGO.name = base.name + "_DistortionParticleFXCamera";
		}
		if (this.boardCameraGO == null)
		{
			if (this.boardCamera != null)
			{
				Object.Destroy(this.boardCamera);
			}
			this.boardCameraGO = new GameObject();
			this.boardCamera = this.boardCameraGO.AddComponent<Camera>();
			this.boardCameraGO.name = base.name + "_DistortionBoardFXCamera";
		}
	}

	// Token: 0x06006C53 RID: 27731 RVA: 0x001FDEBC File Offset: 0x001FC0BC
	private void SetupCameras()
	{
		this.particleCamera.orthographic = true;
		this.particleCamera.orthographicSize = base.transform.localScale.x / 2f;
		this.particleCameraGO.transform.position = this.ObjectToRender.transform.position;
		this.particleCameraGO.transform.Translate(this.RenderOffsetX, this.RenderOffsetY, this.RenderOffsetZ);
		this.particleCameraGO.transform.rotation = this.ObjectToRender.transform.rotation;
		this.particleCameraGO.transform.Rotate(90f, 180f, 0f);
		this.particleCamera.transform.parent = base.transform;
		this.particleCamera.nearClipPlane = -this.ParticleAboveClip;
		this.particleCamera.farClipPlane = this.ParticleBelowClip;
		this.particleCamera.targetTexture = this.particleRenderBuffer;
		this.particleCamera.depth = Camera.main.depth - 1f;
		this.particleCamera.backgroundColor = Color.black;
		this.particleCamera.clearFlags = 2;
		this.particleCamera.cullingMask &= -2097153;
		this.particleCamera.enabled = true;
		this.boardCamera.orthographic = true;
		this.boardCamera.orthographicSize = base.transform.localScale.x / 2f;
		this.boardCameraGO.transform.position = base.transform.position;
		this.boardCameraGO.transform.rotation = base.transform.rotation;
		this.boardCameraGO.transform.Rotate(90f, 180f, 0f);
		this.boardCamera.transform.parent = base.transform;
		this.boardCamera.nearClipPlane = -this.DistortionAboveClip;
		this.boardCamera.farClipPlane = this.DistortionBelowClip;
		this.boardCamera.targetTexture = this.boardRenderBuffer;
		this.boardCamera.depth = Camera.main.depth - 1f;
		this.boardCamera.backgroundColor = Color.black;
		this.boardCamera.clearFlags = 2;
		this.boardCamera.cullingMask &= -2097153;
		this.boardCamera.enabled = true;
	}

	// Token: 0x06006C54 RID: 27732 RVA: 0x001FE148 File Offset: 0x001FC348
	private void CreateRenderTextures()
	{
		this.particleRenderBuffer = RenderTexture.GetTemporary(this.ParticleResolutionX, this.ParticleResolutionY);
		this.boardRenderBuffer = RenderTexture.GetTemporary(this.DistortionResolutionX, this.DistortionResolutionY);
	}

	// Token: 0x06006C55 RID: 27733 RVA: 0x001FE184 File Offset: 0x001FC384
	private void SetupMaterial()
	{
		Material material = base.gameObject.GetComponent<Renderer>().material;
		material.mainTexture = this.boardRenderBuffer;
		material.SetTexture("_ParticleTex", this.particleRenderBuffer);
	}

	// Token: 0x06006C56 RID: 27734 RVA: 0x001FE1C0 File Offset: 0x001FC3C0
	private void PositionObjectToRender()
	{
		Vector3 vector = Vector3.up * this.Yoffset;
		this.ObjectToRender.transform.position += vector;
	}

	// Token: 0x040054E4 RID: 21732
	private const int IgnoreLayer = 21;

	// Token: 0x040054E5 RID: 21733
	public GameObject ObjectToRender;

	// Token: 0x040054E6 RID: 21734
	public int ParticleResolutionX = 256;

	// Token: 0x040054E7 RID: 21735
	public int ParticleResolutionY = 256;

	// Token: 0x040054E8 RID: 21736
	public float ParticleAboveClip = 1f;

	// Token: 0x040054E9 RID: 21737
	public float ParticleBelowClip = 1f;

	// Token: 0x040054EA RID: 21738
	public float RenderOffsetX;

	// Token: 0x040054EB RID: 21739
	public float RenderOffsetY;

	// Token: 0x040054EC RID: 21740
	public float RenderOffsetZ;

	// Token: 0x040054ED RID: 21741
	public int DistortionResolutionX = 256;

	// Token: 0x040054EE RID: 21742
	public int DistortionResolutionY = 256;

	// Token: 0x040054EF RID: 21743
	public float DistortionAboveClip = -0.1f;

	// Token: 0x040054F0 RID: 21744
	public float DistortionBelowClip = 10f;

	// Token: 0x040054F1 RID: 21745
	private Camera particleCamera;

	// Token: 0x040054F2 RID: 21746
	private GameObject particleCameraGO;

	// Token: 0x040054F3 RID: 21747
	private RenderTexture particleRenderBuffer;

	// Token: 0x040054F4 RID: 21748
	private Camera boardCamera;

	// Token: 0x040054F5 RID: 21749
	private GameObject boardCameraGO;

	// Token: 0x040054F6 RID: 21750
	private RenderTexture boardRenderBuffer;

	// Token: 0x040054F7 RID: 21751
	private float Yoffset;

	// Token: 0x040054F8 RID: 21752
	private static float s_Yoffset = 50f;
}
