using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200079C RID: 1948
[ExecuteInEditMode]
public class ScreenEffectsMgr : MonoBehaviour
{
	// Token: 0x06004CF0 RID: 19696 RVA: 0x0016DDF0 File Offset: 0x0016BFF0
	private void Awake()
	{
		ScreenEffectsMgr.s_Instance = this;
		if (ScreenEffectsMgr.m_ActiveScreenEffects == null)
		{
			ScreenEffectsMgr.m_ActiveScreenEffects = new List<ScreenEffect>();
		}
		if (!SystemInfo.supportsRenderTextures && SystemInfo.graphicsDeviceName != "Null Device")
		{
			base.enabled = false;
		}
	}

	// Token: 0x06004CF1 RID: 19697 RVA: 0x0016DE3C File Offset: 0x0016C03C
	private void Update()
	{
		if (this.m_MainCamera == null)
		{
			if (Camera.main == null)
			{
				return;
			}
			this.Init();
		}
		if (this.m_ScreenEffectsRender == null)
		{
			return;
		}
		if (ScreenEffectsMgr.m_ActiveScreenEffects != null && ScreenEffectsMgr.m_ActiveScreenEffects.Count > 0)
		{
			if (!this.m_ScreenEffectsRender.enabled)
			{
				this.m_ScreenEffectsRender.enabled = true;
			}
		}
		else if (this.m_ScreenEffectsRender.enabled)
		{
			this.m_ScreenEffectsRender.enabled = false;
		}
		this.UpdateCameraTransform();
	}

	// Token: 0x06004CF2 RID: 19698 RVA: 0x0016DEE0 File Offset: 0x0016C0E0
	private void OnDisable()
	{
		if (this.m_EffectsObjectsCameraGO != null)
		{
			Object.DestroyImmediate(this.m_EffectsObjectsCameraGO);
		}
		if (this.m_ScreenEffectsRender != null)
		{
			this.m_ScreenEffectsRender.enabled = false;
		}
	}

	// Token: 0x06004CF3 RID: 19699 RVA: 0x0016DF1C File Offset: 0x0016C11C
	private void OnDestroy()
	{
		ScreenEffectsMgr.s_Instance = null;
		if (ScreenEffectsMgr.m_ActiveScreenEffects != null)
		{
			ScreenEffectsMgr.m_ActiveScreenEffects.Clear();
			ScreenEffectsMgr.m_ActiveScreenEffects = null;
		}
	}

	// Token: 0x06004CF4 RID: 19700 RVA: 0x0016DF49 File Offset: 0x0016C149
	private void OnEnable()
	{
		if (Camera.main == null)
		{
			return;
		}
		this.Init();
	}

	// Token: 0x06004CF5 RID: 19701 RVA: 0x0016DF62 File Offset: 0x0016C162
	public static ScreenEffectsMgr Get()
	{
		return ScreenEffectsMgr.s_Instance;
	}

	// Token: 0x06004CF6 RID: 19702 RVA: 0x0016DF69 File Offset: 0x0016C169
	public static void RegisterScreenEffect(ScreenEffect effect)
	{
		if (ScreenEffectsMgr.m_ActiveScreenEffects == null)
		{
			ScreenEffectsMgr.m_ActiveScreenEffects = new List<ScreenEffect>();
		}
		if (!ScreenEffectsMgr.m_ActiveScreenEffects.Contains(effect))
		{
			ScreenEffectsMgr.m_ActiveScreenEffects.Add(effect);
		}
	}

	// Token: 0x06004CF7 RID: 19703 RVA: 0x0016DF9A File Offset: 0x0016C19A
	public static void UnRegisterScreenEffect(ScreenEffect effect)
	{
		if (ScreenEffectsMgr.m_ActiveScreenEffects == null)
		{
			return;
		}
		ScreenEffectsMgr.m_ActiveScreenEffects.Remove(effect);
	}

	// Token: 0x06004CF8 RID: 19704 RVA: 0x0016DFB3 File Offset: 0x0016C1B3
	public int GetActiveScreenEffectsCount()
	{
		if (ScreenEffectsMgr.m_ActiveScreenEffects == null)
		{
			return 0;
		}
		return ScreenEffectsMgr.m_ActiveScreenEffects.Count;
	}

	// Token: 0x06004CF9 RID: 19705 RVA: 0x0016DFCC File Offset: 0x0016C1CC
	private void Init()
	{
		this.m_MainCamera = Camera.main;
		if (this.m_MainCamera == null)
		{
			return;
		}
		this.m_ScreenEffectsRender = this.m_MainCamera.GetComponent<ScreenEffectsRender>();
		if (this.m_ScreenEffectsRender == null)
		{
			this.m_ScreenEffectsRender = this.m_MainCamera.gameObject.AddComponent<ScreenEffectsRender>();
			this.m_MainCamera.hdr = false;
		}
		else
		{
			this.m_ScreenEffectsRender.enabled = true;
		}
		this.CreateCamera(out this.m_EffectsObjectsCamera, out this.m_EffectsObjectsCameraGO, "ScreenEffectsObjectRenderCamera");
		this.m_EffectsObjectsCamera.depth = this.m_MainCamera.depth - 1f;
		this.m_EffectsObjectsCamera.clearFlags = 2;
		this.m_EffectsObjectsCamera.backgroundColor = Color.clear;
		this.m_EffectsObjectsCameraGO.hideFlags = 61;
		SceneUtils.SetLayer(this.m_EffectsObjectsCameraGO, 23);
		this.m_EffectsObjectsCamera.enabled = false;
		this.m_ScreenEffectsRender.m_EffectsObjectsCamera = this.m_EffectsObjectsCamera;
		this.m_EffectsObjectsCamera.cullingMask = 8388865;
	}

	// Token: 0x06004CFA RID: 19706 RVA: 0x0016E0E4 File Offset: 0x0016C2E4
	private void CreateCamera(out Camera camera, out GameObject cameraGO, string cameraName)
	{
		cameraGO = new GameObject(cameraName);
		SceneUtils.SetLayer(cameraGO, GameLayer.CameraMask);
		this.UpdateCameraTransform();
		camera = cameraGO.AddComponent<Camera>();
		camera.CopyFrom(this.m_MainCamera);
		camera.clearFlags = 4;
		if (UniversalInputManager.Get())
		{
			UniversalInputManager.Get().AddIgnoredCamera(camera);
		}
	}

	// Token: 0x06004CFB RID: 19707 RVA: 0x0016E144 File Offset: 0x0016C344
	private void UpdateCameraTransform()
	{
		if (this.m_EffectsObjectsCameraGO == null || this.m_MainCamera == null)
		{
			return;
		}
		Transform transform = this.m_MainCamera.transform;
		this.m_EffectsObjectsCameraGO.transform.position = transform.position;
		this.m_EffectsObjectsCameraGO.transform.rotation = transform.rotation;
	}

	// Token: 0x06004CFC RID: 19708 RVA: 0x0016E1AC File Offset: 0x0016C3AC
	private void CreateBackPlane(Camera camera)
	{
		Vector3 vector = camera.ViewportToWorldPoint(new Vector3(0f, 0f, camera.farClipPlane));
		Vector3 vector2 = camera.ViewportToWorldPoint(new Vector3(1f, 1f, camera.farClipPlane));
		Vector3 vector3;
		vector3..ctor((vector2.x - vector.x) * 0.5f, (vector2.y - vector.y) * 0.5f, (vector2.z - vector.z) * 0.5f);
		float farClipPlane = camera.farClipPlane;
		camera.gameObject.AddComponent<MeshFilter>();
		camera.gameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[]
		{
			new Vector3(-vector3.x, -vector3.z, farClipPlane),
			new Vector3(vector3.x, -vector3.z, farClipPlane),
			new Vector3(-vector3.x, vector3.z, farClipPlane),
			new Vector3(vector3.x, vector3.z, farClipPlane)
		};
		mesh.colors = new Color[]
		{
			Color.black,
			Color.black,
			Color.black,
			Color.black
		};
		mesh.uv = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		mesh.normals = new Vector3[]
		{
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.up
		};
		mesh.triangles = new int[]
		{
			3,
			1,
			2,
			2,
			1,
			0
		};
		camera.gameObject.GetComponent<Renderer>().GetComponent<MeshFilter>().mesh = mesh;
		Material sharedMaterial = new Material(Shader.Find("Hidden/ScreenEffectsBackPlane"));
		camera.gameObject.GetComponent<Renderer>().sharedMaterial = sharedMaterial;
	}

	// Token: 0x040033E3 RID: 13283
	private Camera m_EffectsObjectsCamera;

	// Token: 0x040033E4 RID: 13284
	private GameObject m_EffectsObjectsCameraGO;

	// Token: 0x040033E5 RID: 13285
	private Camera m_MainCamera;

	// Token: 0x040033E6 RID: 13286
	private ScreenEffectsRender m_ScreenEffectsRender;

	// Token: 0x040033E7 RID: 13287
	private static ScreenEffectsMgr s_Instance;

	// Token: 0x040033E8 RID: 13288
	private static List<ScreenEffect> m_ActiveScreenEffects;

	// Token: 0x02000EE7 RID: 3815
	public enum EFFECT_TYPE
	{
		// Token: 0x04005C54 RID: 23636
		Glow,
		// Token: 0x04005C55 RID: 23637
		Distortion
	}
}
