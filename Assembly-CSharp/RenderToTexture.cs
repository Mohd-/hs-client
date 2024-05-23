using System;
using UnityEngine;

// Token: 0x02000338 RID: 824
public class RenderToTexture : MonoBehaviour
{
	// Token: 0x1700037B RID: 891
	// (get) Token: 0x06002AC6 RID: 10950 RVA: 0x000D28D4 File Offset: 0x000D0AD4
	protected Vector3 Offset
	{
		get
		{
			if (this.m_Offset == Vector3.zero)
			{
				RenderToTexture.s_offset.x = RenderToTexture.s_offset.x - 100f;
				if (RenderToTexture.s_offset.x < -50000f)
				{
					RenderToTexture.s_offset.x = -4000f;
					RenderToTexture.s_offset.y = RenderToTexture.s_offset.y - 100f;
					if (RenderToTexture.s_offset.y < -50000f)
					{
						RenderToTexture.s_offset.y = -4000f;
						RenderToTexture.s_offset.z = RenderToTexture.s_offset.z - 100f;
						if (RenderToTexture.s_offset.z < -50000f)
						{
							RenderToTexture.s_offset.z = -4000f;
						}
					}
				}
				this.m_Offset = RenderToTexture.s_offset;
			}
			return this.m_Offset;
		}
	}

	// Token: 0x1700037C RID: 892
	// (get) Token: 0x06002AC7 RID: 10951 RVA: 0x000D29B4 File Offset: 0x000D0BB4
	protected Material AlphaBlendMaterial
	{
		get
		{
			if (this.m_AlphaBlendMaterial == null)
			{
				if (this.m_AlphaBlendShader == null)
				{
					this.m_AlphaBlendShader = ShaderUtils.FindShader("Hidden/R2TColorAlphaCombine");
					if (!this.m_AlphaBlendShader)
					{
						Debug.LogError("Failed to load RenderToTexture Shader: Hidden/R2TColorAlphaCombine");
					}
				}
				this.m_AlphaBlendMaterial = new Material(this.m_AlphaBlendShader);
				SceneUtils.SetHideFlags(this.m_AlphaBlendMaterial, 52);
			}
			return this.m_AlphaBlendMaterial;
		}
	}

	// Token: 0x1700037D RID: 893
	// (get) Token: 0x06002AC8 RID: 10952 RVA: 0x000D2A34 File Offset: 0x000D0C34
	protected Material AdditiveMaterial
	{
		get
		{
			if (this.m_AdditiveMaterial == null)
			{
				if (this.m_AdditiveShader == null)
				{
					this.m_AdditiveShader = ShaderUtils.FindShader("Hidden/R2TAdditive");
					if (!this.m_AdditiveShader)
					{
						Debug.LogError("Failed to load RenderToTexture Shader: Hidden/R2TAdditive");
					}
				}
				this.m_AdditiveMaterial = new Material(this.m_AdditiveShader);
				SceneUtils.SetHideFlags(this.m_AdditiveMaterial, 52);
			}
			return this.m_AdditiveMaterial;
		}
	}

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x06002AC9 RID: 10953 RVA: 0x000D2AB4 File Offset: 0x000D0CB4
	protected Material BloomMaterial
	{
		get
		{
			if (this.m_BloomMaterial == null)
			{
				if (this.m_BloomShader == null)
				{
					this.m_BloomShader = ShaderUtils.FindShader("Hidden/R2TBloom");
					if (!this.m_BloomShader)
					{
						Debug.LogError("Failed to load RenderToTexture Shader: Hidden/R2TBloom");
					}
				}
				this.m_BloomMaterial = new Material(this.m_BloomShader);
				SceneUtils.SetHideFlags(this.m_BloomMaterial, 52);
			}
			return this.m_BloomMaterial;
		}
	}

	// Token: 0x1700037F RID: 895
	// (get) Token: 0x06002ACA RID: 10954 RVA: 0x000D2B34 File Offset: 0x000D0D34
	protected Material BloomMaterialAlpha
	{
		get
		{
			if (this.m_BloomMaterialAlpha == null)
			{
				if (this.m_BloomShaderAlpha == null)
				{
					this.m_BloomShaderAlpha = ShaderUtils.FindShader("Hidden/R2TBloomAlpha");
					if (!this.m_BloomShaderAlpha)
					{
						Debug.LogError("Failed to load RenderToTexture Shader: Hidden/R2TBloomAlpha");
					}
				}
				this.m_BloomMaterialAlpha = new Material(this.m_BloomShaderAlpha);
				SceneUtils.SetHideFlags(this.m_BloomMaterialAlpha, 52);
			}
			return this.m_BloomMaterialAlpha;
		}
	}

	// Token: 0x17000380 RID: 896
	// (get) Token: 0x06002ACB RID: 10955 RVA: 0x000D2BB4 File Offset: 0x000D0DB4
	protected Material BlurMaterial
	{
		get
		{
			if (this.m_BlurMaterial == null)
			{
				if (this.m_BlurShader == null)
				{
					this.m_BlurShader = ShaderUtils.FindShader("Hidden/R2TBlur");
					if (!this.m_BlurShader)
					{
						Debug.LogError("Failed to load RenderToTexture Shader: Hidden/R2TBlur");
					}
				}
				this.m_BlurMaterial = new Material(this.m_BlurShader);
				SceneUtils.SetHideFlags(this.m_BlurMaterial, 52);
			}
			return this.m_BlurMaterial;
		}
	}

	// Token: 0x17000381 RID: 897
	// (get) Token: 0x06002ACC RID: 10956 RVA: 0x000D2C34 File Offset: 0x000D0E34
	protected Material AlphaBlurMaterial
	{
		get
		{
			if (this.m_AlphaBlurMaterial == null)
			{
				if (this.m_AlphaBlurShader == null)
				{
					this.m_AlphaBlurShader = ShaderUtils.FindShader("Hidden/R2TAlphaBlur");
					if (!this.m_AlphaBlurShader)
					{
						Debug.LogError("Failed to load RenderToTexture Shader: Hidden/R2TAlphaBlur");
					}
				}
				this.m_AlphaBlurMaterial = new Material(this.m_AlphaBlurShader);
				SceneUtils.SetHideFlags(this.m_AlphaBlurMaterial, 52);
			}
			return this.m_AlphaBlurMaterial;
		}
	}

	// Token: 0x17000382 RID: 898
	// (get) Token: 0x06002ACD RID: 10957 RVA: 0x000D2CB4 File Offset: 0x000D0EB4
	protected Material TransparentMaterial
	{
		get
		{
			if (this.m_TransparentMaterial == null)
			{
				if (this.m_TransparentShader == null)
				{
					this.m_TransparentShader = ShaderUtils.FindShader("Hidden/R2TTransparent");
					if (!this.m_TransparentShader)
					{
						Debug.LogError("Failed to load RenderToTexture Shader: Hidden/R2TTransparent");
					}
				}
				this.m_TransparentMaterial = new Material(this.m_TransparentShader);
				SceneUtils.SetHideFlags(this.m_TransparentMaterial, 52);
			}
			return this.m_TransparentMaterial;
		}
	}

	// Token: 0x17000383 RID: 899
	// (get) Token: 0x06002ACE RID: 10958 RVA: 0x000D2D34 File Offset: 0x000D0F34
	protected Material AlphaClipMaterial
	{
		get
		{
			if (this.m_AlphaClipMaterial == null)
			{
				if (this.m_AlphaClipShader == null)
				{
					this.m_AlphaClipShader = ShaderUtils.FindShader("Hidden/R2TAlphaClip");
					if (!this.m_AlphaClipShader)
					{
						Debug.LogError("Failed to load RenderToTexture Shader: Hidden/R2TAlphaClip");
					}
				}
				this.m_AlphaClipMaterial = new Material(this.m_AlphaClipShader);
				SceneUtils.SetHideFlags(this.m_AlphaClipMaterial, 52);
			}
			return this.m_AlphaClipMaterial;
		}
	}

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x06002ACF RID: 10959 RVA: 0x000D2DB4 File Offset: 0x000D0FB4
	protected Material AlphaClipBloomMaterial
	{
		get
		{
			if (this.m_AlphaClipBloomMaterial == null)
			{
				if (this.m_AlphaClipBloomShader == null)
				{
					this.m_AlphaClipBloomShader = ShaderUtils.FindShader("Hidden/R2TAlphaClipBloom");
					if (!this.m_AlphaClipBloomShader)
					{
						Debug.LogError("Failed to load RenderToTexture Shader: Hidden/R2TAlphaClipBloom");
					}
				}
				this.m_AlphaClipBloomMaterial = new Material(this.m_AlphaClipBloomShader);
				SceneUtils.SetHideFlags(this.m_AlphaClipBloomMaterial, 52);
			}
			return this.m_AlphaClipBloomMaterial;
		}
	}

	// Token: 0x17000385 RID: 901
	// (get) Token: 0x06002AD0 RID: 10960 RVA: 0x000D2E34 File Offset: 0x000D1034
	protected Material AlphaClipGradientMaterial
	{
		get
		{
			if (this.m_AlphaClipGradientMaterial == null)
			{
				if (this.m_AlphaClipGradientShader == null)
				{
					this.m_AlphaClipGradientShader = ShaderUtils.FindShader("Hidden/R2TAlphaClipGradient");
					if (!this.m_AlphaClipGradientShader)
					{
						Debug.LogError("Failed to load RenderToTexture Shader: Hidden/R2TAlphaClipGradient");
					}
				}
				this.m_AlphaClipGradientMaterial = new Material(this.m_AlphaClipGradientShader);
				SceneUtils.SetHideFlags(this.m_AlphaClipGradientMaterial, 52);
			}
			return this.m_AlphaClipGradientMaterial;
		}
	}

	// Token: 0x06002AD1 RID: 10961 RVA: 0x000D2EB4 File Offset: 0x000D10B4
	private void Awake()
	{
		this.m_UnlitWhiteShader = ShaderUtils.FindShader("Custom/Unlit/Color/White");
		if (!this.m_UnlitWhiteShader)
		{
			Debug.LogError("Failed to load RenderToTexture Shader: Custom/Unlit/Color/White");
		}
		this.m_OffscreenPos = this.Offset;
		if (this.m_Material != null)
		{
			this.m_Material = Object.Instantiate<Material>(this.m_Material);
		}
	}

	// Token: 0x06002AD2 RID: 10962 RVA: 0x000D2F19 File Offset: 0x000D1119
	private void Start()
	{
		if (this.m_RenderOnStart)
		{
			this.m_isDirty = true;
		}
		this.Init();
	}

	// Token: 0x06002AD3 RID: 10963 RVA: 0x000D2F34 File Offset: 0x000D1134
	private void Update()
	{
		if (!this.m_renderEnabled)
		{
			return;
		}
		if (this.m_RenderTexture && !this.m_RenderTexture.IsCreated())
		{
			Log.Kyle.Print("RenderToTexture Texture lost. Render Called", new object[0]);
			this.m_isDirty = true;
			this.RenderTex();
			return;
		}
		if (this.m_LateUpdate)
		{
			return;
		}
		if (this.m_HideRenderObject && this.m_ObjectToRender)
		{
			this.PositionHiddenObjectsAndCameras();
		}
		if (this.m_RealtimeRender || this.m_isDirty)
		{
			this.RenderTex();
		}
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x000D2FDC File Offset: 0x000D11DC
	private void LateUpdate()
	{
		if (!this.m_renderEnabled)
		{
			return;
		}
		if (this.m_LateUpdate)
		{
			if (this.m_HideRenderObject && this.m_ObjectToRender)
			{
				this.PositionHiddenObjectsAndCameras();
			}
			if (this.m_RealtimeRender || this.m_isDirty)
			{
				this.RenderTex();
			}
			this.m_RenderTexture.DiscardContents();
		}
		else if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClipBloom || this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.Bloom)
		{
			this.RenderBloom();
		}
		else if (this.m_BloomPlaneGameObject)
		{
			Object.DestroyImmediate(this.m_BloomPlaneGameObject);
		}
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x000D308B File Offset: 0x000D128B
	private void OnApplicationFocus(bool state)
	{
		if (this.m_RenderTexture && state)
		{
			this.m_isDirty = true;
			this.RenderTex();
		}
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x000D30B0 File Offset: 0x000D12B0
	private void OnDrawGizmos()
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.m_FarClip < 0f)
		{
			this.m_FarClip = 0f;
		}
		if (this.m_NearClip > 0f)
		{
			this.m_NearClip = 0f;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Vector3 vector;
		vector..ctor(0f, -this.m_NearClip * 0.5f, 0f);
		Gizmos.color = new Color(0.1f, 0.5f, 0.7f, 0.8f);
		Gizmos.DrawWireCube(vector + this.m_PositionOffset, new Vector3(this.m_Width, -this.m_NearClip, this.m_Height));
		Gizmos.color = new Color(0.2f, 0.2f, 0.9f, 0.8f);
		Vector3 vector2;
		vector2..ctor(0f, -this.m_FarClip * 0.5f, 0f);
		Gizmos.DrawWireCube(vector2 + this.m_PositionOffset, new Vector3(this.m_Width, -this.m_FarClip, this.m_Height));
		Gizmos.color = new Color(0.8f, 0.8f, 1f, 1f);
		Gizmos.DrawWireCube(this.m_PositionOffset, new Vector3(this.m_Width, 0f, this.m_Height));
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x000D3224 File Offset: 0x000D1424
	private void OnDisable()
	{
		this.RestoreAfterRender();
		if (this.m_ObjectToRender)
		{
			if (this.m_ObjectToRenderOrgParent != null)
			{
				this.m_ObjectToRender.transform.parent = this.m_ObjectToRenderOrgParent;
			}
			this.m_ObjectToRender.transform.localPosition = this.m_ObjectToRenderOrgPosition;
		}
		if (this.m_PlaneGameObject)
		{
			Object.Destroy(this.m_PlaneGameObject);
		}
		if (this.m_BloomPlaneGameObject)
		{
			Object.Destroy(this.m_BloomPlaneGameObject);
		}
		if (this.m_BloomCapturePlaneGameObject)
		{
			Object.Destroy(this.m_BloomCapturePlaneGameObject);
		}
		if (this.m_BloomCaptureCameraGO)
		{
			Object.Destroy(this.m_BloomCaptureCameraGO);
		}
		this.ReleaseTexture();
		if (this.m_Camera)
		{
			this.m_Camera.enabled = false;
		}
		if (this.m_AlphaCamera)
		{
			this.m_AlphaCamera.enabled = false;
		}
		this.m_init = false;
		this.m_isDirty = true;
	}

	// Token: 0x06002AD8 RID: 10968 RVA: 0x000D333C File Offset: 0x000D153C
	private void OnDestroy()
	{
		this.CleanUp();
	}

	// Token: 0x06002AD9 RID: 10969 RVA: 0x000D3344 File Offset: 0x000D1544
	private void OnEnable()
	{
		if (this.m_RenderOnEnable)
		{
			this.RenderTex();
		}
	}

	// Token: 0x06002ADA RID: 10970 RVA: 0x000D3357 File Offset: 0x000D1557
	public RenderTexture Render()
	{
		this.m_isDirty = true;
		return this.m_RenderTexture;
	}

	// Token: 0x06002ADB RID: 10971 RVA: 0x000D3366 File Offset: 0x000D1566
	public RenderTexture RenderNow()
	{
		this.RenderTex();
		return this.m_RenderTexture;
	}

	// Token: 0x06002ADC RID: 10972 RVA: 0x000D3374 File Offset: 0x000D1574
	public void ForceTextureRebuild()
	{
		if (!base.enabled)
		{
			return;
		}
		this.ReleaseTexture();
		this.m_isDirty = true;
		this.RenderTex();
	}

	// Token: 0x06002ADD RID: 10973 RVA: 0x000D3395 File Offset: 0x000D1595
	public void Show()
	{
		this.Show(false);
	}

	// Token: 0x06002ADE RID: 10974 RVA: 0x000D33A0 File Offset: 0x000D15A0
	public void Show(bool render)
	{
		this.m_renderEnabled = true;
		if (this.m_RenderToObject)
		{
			this.m_RenderToObject.GetComponent<Renderer>().enabled = true;
		}
		else if (this.m_PlaneGameObject)
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().enabled = true;
			if (this.m_BloomPlaneGameObject)
			{
				this.m_BloomPlaneGameObject.GetComponent<Renderer>().enabled = true;
			}
		}
		if (render)
		{
			this.Render();
		}
	}

	// Token: 0x06002ADF RID: 10975 RVA: 0x000D342C File Offset: 0x000D162C
	public void Hide()
	{
		this.m_renderEnabled = false;
		if (this.m_RenderToObject)
		{
			this.m_RenderToObject.GetComponent<Renderer>().enabled = false;
		}
		else if (this.m_PlaneGameObject)
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().enabled = false;
			if (this.m_BloomPlaneGameObject)
			{
				this.m_BloomPlaneGameObject.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	// Token: 0x06002AE0 RID: 10976 RVA: 0x000D34A8 File Offset: 0x000D16A8
	public void SetDirty()
	{
		this.m_init = false;
		this.m_isDirty = true;
	}

	// Token: 0x06002AE1 RID: 10977 RVA: 0x000D34B8 File Offset: 0x000D16B8
	public Material GetRenderMaterial()
	{
		if (this.m_RenderToObject)
		{
			return this.m_RenderToObject.GetComponent<Renderer>().material;
		}
		if (this.m_PlaneGameObject)
		{
			return this.m_PlaneGameObject.GetComponent<Renderer>().material;
		}
		return this.m_Material;
	}

	// Token: 0x06002AE2 RID: 10978 RVA: 0x000D350D File Offset: 0x000D170D
	public GameObject GetRenderToObject()
	{
		if (this.m_RenderToObject)
		{
			return this.m_RenderToObject;
		}
		return this.m_PlaneGameObject;
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x000D352C File Offset: 0x000D172C
	public RenderTexture GetRenderTexture()
	{
		return this.m_RenderTexture;
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x000D3534 File Offset: 0x000D1734
	public Vector3 GetOffscreenPosition()
	{
		return this.m_OffscreenPos;
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x000D353C File Offset: 0x000D173C
	public Vector3 GetOffscreenPositionOffset()
	{
		return this.m_OffscreenPos - base.transform.position;
	}

	// Token: 0x06002AE6 RID: 10982 RVA: 0x000D3554 File Offset: 0x000D1754
	private void Init()
	{
		if (this.m_init)
		{
			return;
		}
		if (this.m_RealtimeTranslation)
		{
			this.m_OffscreenGameObject = new GameObject();
			this.m_OffscreenGameObject.name = string.Format("R2TOffsetRenderRoot_{0}", base.name);
			this.m_OffscreenGameObject.transform.position = base.transform.position;
		}
		if (this.m_ObjectToRender)
		{
			if (!this.m_ObjectToRenderOrgPositionStored)
			{
				this.m_ObjectToRenderOrgParent = this.m_ObjectToRender.transform.parent;
				this.m_ObjectToRenderOrgPosition = this.m_ObjectToRender.transform.localPosition;
				this.m_ObjectToRenderOrgPositionStored = true;
			}
			if (this.m_HideRenderObject)
			{
				if (this.m_RealtimeTranslation)
				{
					this.m_ObjectToRender.transform.parent = this.m_OffscreenGameObject.transform;
					if (this.m_AlphaObjectToRender)
					{
						this.m_AlphaObjectToRender.transform.parent = this.m_OffscreenGameObject.transform;
					}
				}
				if (this.m_RenderToObject)
				{
					this.m_OriginalRenderPosition = this.m_RenderToObject.transform.position;
				}
				else
				{
					this.m_OriginalRenderPosition = base.transform.position;
				}
				if (this.m_ObjectToRender && this.m_ObjectToRenderOffset == Vector3.zero)
				{
					this.m_ObjectToRenderOffset = base.transform.position - this.m_ObjectToRender.transform.position;
				}
				if (this.m_AlphaObjectToRender && this.m_AlphaObjectToRenderOffset == Vector3.zero)
				{
					this.m_AlphaObjectToRenderOffset = base.transform.position - this.m_AlphaObjectToRender.transform.position;
				}
			}
		}
		else if (!this.m_ObjectToRenderOrgPositionStored)
		{
			this.m_ObjectToRenderOrgPosition = base.transform.localPosition;
			if (this.m_OffscreenGameObject != null)
			{
				this.m_OffscreenGameObject.transform.position = base.transform.position;
			}
			this.m_ObjectToRenderOrgPositionStored = true;
		}
		if (this.m_HideRenderObject)
		{
			if (this.m_RealtimeTranslation)
			{
				if (this.m_OffscreenGameObject != null)
				{
					this.m_OffscreenGameObject.transform.position = this.m_OffscreenPos;
				}
			}
			else if (this.m_ObjectToRender)
			{
				this.m_ObjectToRender.transform.position = this.m_OffscreenPos;
			}
			else
			{
				base.transform.position = this.m_OffscreenPos;
			}
		}
		if (this.m_ObjectToRender == null)
		{
			this.m_ObjectToRender = base.gameObject;
		}
		this.CalcWorldWidthHeightScale();
		this.CreateTexture();
		this.CreateCamera();
		if (this.m_RenderMeshAsAlpha || this.m_AlphaObjectToRender != null)
		{
			this.CreateAlphaCamera();
		}
		if (!this.m_RenderToObject && this.m_CreateRenderPlane)
		{
			this.CreateRenderPlane();
		}
		if (this.m_RenderToObject)
		{
			this.m_RenderToObject.GetComponent<Renderer>().material.renderQueue = this.m_RenderQueueOffset + this.m_RenderQueue;
		}
		this.SetupMaterial();
		this.m_init = true;
	}

	// Token: 0x06002AE7 RID: 10983 RVA: 0x000D38B8 File Offset: 0x000D1AB8
	private void RenderTex()
	{
		if (!this.m_renderEnabled)
		{
			return;
		}
		this.Init();
		if (!this.m_init)
		{
			return;
		}
		this.SetupForRender();
		if (this.m_RenderMaterial != this.m_PreviousRenderMaterial)
		{
			this.SetupMaterial();
		}
		if (this.m_HideRenderObject && this.m_ObjectToRender)
		{
			this.PositionHiddenObjectsAndCameras();
		}
		if (this.m_RenderMeshAsAlpha || this.m_AlphaObjectToRender != null)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(this.m_RenderTexture.width, this.m_RenderTexture.height, this.m_RenderTexture.depth, this.m_RenderTexture.format);
			this.m_Camera.targetTexture = temporary;
			this.CameraRender();
			RenderTexture temporary2 = RenderTexture.GetTemporary(this.m_RenderTexture.width, this.m_RenderTexture.height, this.m_RenderTexture.depth, 7);
			this.m_AlphaCamera.targetTexture = temporary2;
			this.AlphaCameraRender();
			this.AlphaBlendMaterial.SetTexture("_AlphaTex", temporary2);
			if (this.m_BlurAmount > 0f)
			{
				RenderTexture temporary3 = RenderTexture.GetTemporary(this.m_RenderTexture.width, this.m_RenderTexture.height, this.m_RenderTexture.depth, this.m_RenderTexture.format);
				Graphics.Blit(temporary, temporary3, this.AlphaBlendMaterial);
				this.CameraRender();
				Material sampleMat = this.BlurMaterial;
				if (this.m_BlurAlphaOnly)
				{
					sampleMat = this.AlphaBlurMaterial;
				}
				this.m_RenderTexture.DiscardContents();
				this.Sample(temporary3, this.m_RenderTexture, sampleMat, this.m_BlurAmount);
				RenderTexture.ReleaseTemporary(temporary3);
			}
			else
			{
				this.m_RenderTexture.DiscardContents();
				Graphics.Blit(temporary, this.m_RenderTexture, this.AlphaBlendMaterial);
			}
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else if (this.m_BlurAmount > 0f)
		{
			RenderTexture temporary4 = RenderTexture.GetTemporary(this.m_RenderTexture.width, this.m_RenderTexture.height, this.m_RenderTexture.depth, this.m_RenderTexture.format);
			this.m_Camera.targetTexture = temporary4;
			this.CameraRender();
			Material sampleMat2 = this.BlurMaterial;
			if (this.m_BlurAlphaOnly)
			{
				sampleMat2 = this.m_AlphaBlurMaterial;
			}
			this.m_RenderTexture.DiscardContents();
			this.Sample(temporary4, this.m_RenderTexture, sampleMat2, this.m_BlurAmount);
			RenderTexture.ReleaseTemporary(temporary4);
		}
		else
		{
			this.m_Camera.targetTexture = this.m_RenderTexture;
			this.CameraRender();
		}
		if (this.m_RenderToObject)
		{
			Renderer renderer = this.m_RenderToObject.GetComponent<Renderer>();
			if (renderer == null)
			{
				renderer = this.m_RenderToObject.GetComponentInChildren<Renderer>();
			}
			if (this.m_ShaderTextureName != string.Empty)
			{
				renderer.material.SetTexture(this.m_ShaderTextureName, this.m_RenderTexture);
			}
			else
			{
				renderer.material.mainTexture = this.m_RenderTexture;
			}
		}
		else if (this.m_PlaneGameObject)
		{
			if (this.m_ShaderTextureName != string.Empty)
			{
				this.m_PlaneGameObject.GetComponent<Renderer>().material.SetTexture(this.m_ShaderTextureName, this.m_RenderTexture);
			}
			else
			{
				this.m_PlaneGameObject.GetComponent<Renderer>().material.mainTexture = this.m_RenderTexture;
			}
		}
		if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClip || this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClipBloom)
		{
			GameObject gameObject = this.m_PlaneGameObject;
			if (this.m_RenderToObject)
			{
				gameObject = this.m_RenderToObject;
			}
			Material material = gameObject.GetComponent<Renderer>().material;
			material.SetFloat("_Cutoff", this.m_AlphaClip);
			material.SetFloat("_Intensity", this.m_AlphaClipIntensity);
			material.SetFloat("_AlphaIntensity", this.m_AlphaClipAlphaIntensity);
			if (this.m_AlphaClipRenderStyle == RenderToTexture.AlphaClipShader.ColorGradient)
			{
				material.SetTexture("_GradientTex", this.m_AlphaClipGradientMap);
			}
		}
		if (!this.m_RealtimeRender)
		{
			this.RestoreAfterRender();
		}
		this.m_isDirty = false;
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x000D3CE0 File Offset: 0x000D1EE0
	private void RenderBloom()
	{
		if (this.m_BloomIntensity == 0f)
		{
			if (this.m_BloomPlaneGameObject)
			{
				Object.DestroyImmediate(this.m_BloomPlaneGameObject);
			}
			return;
		}
		if (this.m_BloomIntensity == 0f)
		{
			if (this.m_BloomPlaneGameObject)
			{
				Object.DestroyImmediate(this.m_BloomPlaneGameObject);
			}
			return;
		}
		int num = (int)((float)this.m_RenderTexture.width * Mathf.Clamp01(this.m_BloomResolutionRatio));
		int num2 = (int)((float)this.m_RenderTexture.height * Mathf.Clamp01(this.m_BloomResolutionRatio));
		RenderTexture renderTexture = this.m_RenderTexture;
		if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClipBloom)
		{
			if (!this.m_BloomPlaneGameObject)
			{
				this.CreateBloomPlane();
			}
			if (!this.m_BloomRenderTexture)
			{
				this.m_BloomRenderTexture = new RenderTexture(num, num2, 32, 0);
			}
		}
		if (!this.m_BloomRenderBuffer1)
		{
			this.m_BloomRenderBuffer1 = new RenderTexture(num, num2, 32, 0);
		}
		if (!this.m_BloomRenderBuffer2)
		{
			this.m_BloomRenderBuffer2 = new RenderTexture(num, num2, 32, 0);
		}
		Material material = this.BloomMaterial;
		if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClipBloom)
		{
			material = this.AlphaClipBloomMaterial;
			renderTexture = this.m_BloomRenderTexture;
			if (!this.m_BloomCaptureCameraGO)
			{
				this.CreateBloomCaptureCamera();
			}
			this.m_BloomCaptureCamera.targetTexture = this.m_BloomRenderTexture;
			material.SetFloat("_Cutoff", this.m_AlphaClip);
			material.SetFloat("_Intensity", this.m_AlphaClipIntensity);
			material.SetFloat("_AlphaIntensity", this.m_AlphaClipAlphaIntensity);
			this.m_BloomCaptureCamera.Render();
		}
		if (this.m_BloomRenderType == RenderToTexture.BloomRenderType.Alpha)
		{
			material = this.BloomMaterialAlpha;
			material.SetFloat("_AlphaIntensity", this.m_BloomAlphaIntensity);
		}
		float num3 = 1f / (float)num;
		float num4 = 1f / (float)num2;
		material.SetFloat("_Threshold", this.m_BloomThreshold);
		material.SetFloat("_Intensity", this.m_BloomIntensity / (1f - this.m_BloomThreshold));
		material.SetVector("_OffsetA", new Vector4(1.5f * num3, 1.5f * num4, -1.5f * num3, 1.5f * num4));
		material.SetVector("_OffsetB", new Vector4(-1.5f * num3, -1.5f * num4, 1.5f * num3, -1.5f * num4));
		this.m_BloomRenderBuffer2.DiscardContents();
		Graphics.Blit(renderTexture, this.m_BloomRenderBuffer2, material, 1);
		num3 *= 4f * this.m_BloomBlur;
		num4 *= 4f * this.m_BloomBlur;
		material.SetVector("_OffsetA", new Vector4(1.5f * num3, 0f, -1.5f * num3, 0f));
		material.SetVector("_OffsetB", new Vector4(0.5f * num3, 0f, -0.5f * num3, 0f));
		this.m_BloomRenderBuffer1.DiscardContents();
		Graphics.Blit(this.m_BloomRenderBuffer2, this.m_BloomRenderBuffer1, material, 2);
		material.SetVector("_OffsetA", new Vector4(0f, 1.5f * num4, 0f, -1.5f * num4));
		material.SetVector("_OffsetB", new Vector4(0f, 0.5f * num4, 0f, -0.5f * num4));
		renderTexture.DiscardContents();
		Graphics.Blit(this.m_BloomRenderBuffer1, renderTexture, material, 2);
		if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.AlphaClipBloom)
		{
			this.m_BloomPlaneGameObject.GetComponent<Renderer>().material.color = this.m_BloomColor;
			this.m_BloomPlaneGameObject.GetComponent<Renderer>().material.mainTexture = renderTexture;
			if (this.m_PlaneGameObject)
			{
				this.m_BloomPlaneGameObject.GetComponent<Renderer>().material.renderQueue = this.m_PlaneGameObject.GetComponent<Renderer>().material.renderQueue + 1;
			}
		}
		else if (this.m_RenderToObject)
		{
			this.m_RenderToObject.GetComponent<Renderer>().material.color = this.m_BloomColor;
			this.m_RenderToObject.GetComponent<Renderer>().material.mainTexture = renderTexture;
		}
		else
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().material.color = this.m_BloomColor;
			this.m_PlaneGameObject.GetComponent<Renderer>().material.mainTexture = renderTexture;
		}
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x000D415C File Offset: 0x000D235C
	private void SetupForRender()
	{
		this.CalcWorldWidthHeightScale();
		if (!this.m_RenderTexture)
		{
			this.CreateTexture();
		}
		if (!this.m_HideRenderObject)
		{
			return;
		}
		if (this.m_PlaneGameObject)
		{
			this.m_PlaneGameObject.transform.localPosition = this.m_PositionOffset;
			this.m_PlaneGameObject.layer = base.gameObject.layer;
		}
		this.m_Camera.backgroundColor = this.m_ClearColor;
	}

	// Token: 0x06002AEA RID: 10986 RVA: 0x000D41E0 File Offset: 0x000D23E0
	private void SetupMaterial()
	{
		GameObject gameObject = this.m_PlaneGameObject;
		if (this.m_RenderToObject)
		{
			gameObject = this.m_RenderToObject;
			if (this.m_RenderMaterial == RenderToTexture.RenderToTextureMaterial.Custom)
			{
				return;
			}
		}
		if (gameObject == null)
		{
			return;
		}
		switch (this.m_RenderMaterial)
		{
		case RenderToTexture.RenderToTextureMaterial.Transparent:
			gameObject.GetComponent<Renderer>().material = this.TransparentMaterial;
			break;
		case RenderToTexture.RenderToTextureMaterial.Additive:
			gameObject.GetComponent<Renderer>().material = this.AdditiveMaterial;
			break;
		case RenderToTexture.RenderToTextureMaterial.Bloom:
			if (this.m_BloomBlend == RenderToTexture.BloomBlendType.Additive)
			{
				gameObject.GetComponent<Renderer>().material = this.AdditiveMaterial;
			}
			else if (this.m_BloomBlend == RenderToTexture.BloomBlendType.Transparent)
			{
				gameObject.GetComponent<Renderer>().material = this.TransparentMaterial;
			}
			break;
		case RenderToTexture.RenderToTextureMaterial.AlphaClip:
		{
			Material material;
			if (this.m_AlphaClipRenderStyle == RenderToTexture.AlphaClipShader.Standard)
			{
				material = this.AlphaClipMaterial;
			}
			else
			{
				material = this.AlphaClipGradientMaterial;
			}
			gameObject.GetComponent<Renderer>().material = material;
			material.SetFloat("_Cutoff", this.m_AlphaClip);
			material.SetFloat("_Intensity", this.m_AlphaClipIntensity);
			material.SetFloat("_AlphaIntensity", this.m_AlphaClipAlphaIntensity);
			if (this.m_AlphaClipRenderStyle == RenderToTexture.AlphaClipShader.ColorGradient)
			{
				material.SetTexture("_GradientTex", this.m_AlphaClipGradientMap);
			}
			break;
		}
		case RenderToTexture.RenderToTextureMaterial.AlphaClipBloom:
		{
			Material material2;
			if (this.m_AlphaClipRenderStyle == RenderToTexture.AlphaClipShader.Standard)
			{
				material2 = this.AlphaClipMaterial;
			}
			else
			{
				material2 = this.AlphaClipGradientMaterial;
			}
			gameObject.GetComponent<Renderer>().material = material2;
			material2.SetFloat("_Cutoff", this.m_AlphaClip);
			material2.SetFloat("_Intensity", this.m_AlphaClipIntensity);
			material2.SetFloat("_AlphaIntensity", this.m_AlphaClipAlphaIntensity);
			if (this.m_AlphaClipRenderStyle == RenderToTexture.AlphaClipShader.ColorGradient)
			{
				material2.SetTexture("_GradientTex", this.m_AlphaClipGradientMap);
			}
			break;
		}
		default:
			if (this.m_Material != null)
			{
				gameObject.GetComponent<Renderer>().material = this.m_Material;
			}
			break;
		}
		if (gameObject.GetComponent<Renderer>().material != null)
		{
			gameObject.GetComponent<Renderer>().material.color *= this.m_TintColor;
		}
		if (this.m_BloomIntensity > 0f && this.m_BloomPlaneGameObject)
		{
			this.m_BloomPlaneGameObject.GetComponent<Renderer>().material.color = this.m_BloomColor;
		}
		gameObject.GetComponent<Renderer>().material.renderQueue = this.m_RenderQueueOffset + this.m_RenderQueue;
		if (this.m_BloomPlaneGameObject)
		{
			this.m_BloomPlaneGameObject.GetComponent<Renderer>().material.renderQueue = this.m_RenderQueueOffset + this.m_RenderQueue + 1;
		}
		this.m_PreviousRenderMaterial = this.m_RenderMaterial;
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x000D44B0 File Offset: 0x000D26B0
	private void PositionHiddenObjectsAndCameras()
	{
		Vector3 vector = Vector3.zero;
		if (this.m_RenderToObject)
		{
			vector = this.m_RenderToObject.transform.position - this.m_OriginalRenderPosition;
		}
		else
		{
			vector = base.transform.position - this.m_OriginalRenderPosition;
		}
		if (this.m_RealtimeTranslation)
		{
			this.m_OffscreenGameObject.transform.position = this.m_OffscreenPos + vector;
			this.m_OffscreenGameObject.transform.rotation = base.transform.rotation;
			if (this.m_AlphaObjectToRender)
			{
				this.m_AlphaObjectToRender.transform.position = this.m_OffscreenPos - this.ALPHA_OBJECT_OFFSET - this.m_AlphaObjectToRenderOffset + vector;
				this.m_AlphaObjectToRender.transform.rotation = base.transform.rotation;
			}
		}
		else
		{
			if (this.m_ObjectToRender)
			{
				this.m_ObjectToRender.transform.rotation = Quaternion.identity;
				this.m_ObjectToRender.transform.position = this.m_OffscreenPos - this.m_ObjectToRenderOffset + this.m_PositionOffset;
				this.m_ObjectToRender.transform.rotation = base.transform.rotation;
			}
			if (this.m_AlphaObjectToRender)
			{
				this.m_AlphaObjectToRender.transform.position = this.m_OffscreenPos - this.ALPHA_OBJECT_OFFSET - this.m_AlphaObjectToRenderOffset;
				this.m_AlphaObjectToRender.transform.rotation = base.transform.rotation;
			}
			if (this.m_CameraGO == null)
			{
				return;
			}
			this.m_CameraGO.transform.rotation = Quaternion.identity;
			if (this.m_ObjectToRender)
			{
				this.m_CameraGO.transform.position = this.m_ObjectToRender.transform.position;
			}
			else
			{
				this.m_CameraGO.transform.position = this.m_OffscreenPos + this.m_PositionOffset;
			}
			this.m_CameraGO.transform.rotation = this.m_ObjectToRender.transform.rotation;
			this.m_CameraGO.transform.Rotate(90f, 0f, 0f);
		}
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x000D4730 File Offset: 0x000D2930
	private void RestoreAfterRender()
	{
		if (this.m_HideRenderObject)
		{
			return;
		}
		if (this.m_ObjectToRender)
		{
			if (this.m_ObjectToRenderOrgParent != null)
			{
				this.m_ObjectToRender.transform.parent = this.m_ObjectToRenderOrgParent;
			}
			this.m_ObjectToRender.transform.localPosition = this.m_ObjectToRenderOrgPosition;
		}
		else
		{
			base.transform.localPosition = this.m_ObjectToRenderOrgPosition;
		}
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x000D47AC File Offset: 0x000D29AC
	private void CreateTexture()
	{
		if (this.m_RenderTexture != null)
		{
			return;
		}
		Vector2 vector = this.CalcTextureSize();
		GraphicsManager graphicsManager = GraphicsManager.Get();
		if (graphicsManager)
		{
			if (graphicsManager.RenderQualityLevel == GraphicsQuality.Low)
			{
				vector *= 0.75f;
			}
			else if (graphicsManager.RenderQualityLevel == GraphicsQuality.Medium)
			{
				vector *= 1f;
			}
			else if (graphicsManager.RenderQualityLevel == GraphicsQuality.High)
			{
				vector *= 1.5f;
			}
		}
		this.m_RenderTexture = new RenderTexture((int)vector.x, (int)vector.y, 32, this.m_RenderTextureFormat);
		this.m_RenderTexture.Create();
		if (this.m_RenderMeshAsAlpha)
		{
			this.m_AlphaRenderTexture = new RenderTexture((int)vector.x, (int)vector.y, 32, this.m_RenderTextureFormat);
			this.m_AlphaRenderTexture.Create();
		}
		if (this.m_Camera)
		{
			this.m_Camera.targetTexture = this.m_RenderTexture;
		}
		if (this.m_AlphaCamera)
		{
			this.m_AlphaCamera.targetTexture = this.m_AlphaRenderTexture;
		}
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x000D48E0 File Offset: 0x000D2AE0
	private void ReleaseTexture()
	{
		if (RenderTexture.active == this.m_RenderTexture)
		{
			RenderTexture.active = null;
		}
		if (RenderTexture.active == this.m_AlphaRenderTexture)
		{
			RenderTexture.active = null;
		}
		if (RenderTexture.active == this.m_BloomRenderTexture)
		{
			RenderTexture.active = null;
		}
		if (this.m_RenderTexture != null)
		{
			if (this.m_Camera)
			{
				this.m_Camera.targetTexture = null;
			}
			Object.Destroy(this.m_RenderTexture);
			this.m_RenderTexture = null;
		}
		if (this.m_AlphaRenderTexture != null)
		{
			if (this.m_AlphaCamera)
			{
				this.m_AlphaCamera.targetTexture = null;
			}
			Object.Destroy(this.m_AlphaRenderTexture);
			this.m_AlphaRenderTexture = null;
		}
		if (this.m_BloomRenderTexture != null)
		{
			Object.Destroy(this.m_BloomRenderTexture);
			this.m_BloomRenderTexture = null;
		}
		if (this.m_BloomRenderBuffer1 != null)
		{
			Object.Destroy(this.m_BloomRenderBuffer1);
			this.m_BloomRenderBuffer1 = null;
		}
		if (this.m_BloomRenderBuffer2 != null)
		{
			Object.Destroy(this.m_BloomRenderBuffer2);
		}
		this.m_BloomRenderBuffer2 = null;
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x000D4A28 File Offset: 0x000D2C28
	private void CreateCamera()
	{
		if (this.m_Camera != null)
		{
			return;
		}
		this.m_CameraGO = new GameObject();
		this.m_Camera = this.m_CameraGO.AddComponent<Camera>();
		this.m_CameraGO.name = base.name + "_R2TRenderCamera";
		this.m_Camera.orthographic = true;
		if (this.m_HideRenderObject)
		{
			if (this.m_RealtimeTranslation)
			{
				this.m_OffscreenGameObject.transform.position = base.transform.position;
				this.m_CameraGO.transform.parent = this.m_OffscreenGameObject.transform;
				this.m_CameraGO.transform.localPosition = Vector3.zero + this.m_PositionOffset;
				this.m_CameraGO.transform.rotation = base.transform.rotation;
				this.m_OffscreenGameObject.transform.position = this.m_OffscreenPos;
			}
			else
			{
				this.m_CameraGO.transform.parent = null;
				this.m_CameraGO.transform.position = this.m_OffscreenPos + this.m_PositionOffset;
				this.m_CameraGO.transform.rotation = base.transform.rotation;
			}
		}
		else
		{
			this.m_CameraGO.transform.parent = base.transform;
			this.m_CameraGO.transform.position = base.transform.position + this.m_PositionOffset;
			this.m_CameraGO.transform.rotation = base.transform.rotation;
		}
		this.m_CameraGO.transform.Rotate(90f, 0f, 0f);
		if (this.m_FarClip < 0f)
		{
			this.m_FarClip = 0f;
		}
		if (this.m_NearClip > 0f)
		{
			this.m_NearClip = 0f;
		}
		this.m_Camera.nearClipPlane = this.m_NearClip * this.m_WorldScale.y;
		this.m_Camera.farClipPlane = this.m_FarClip * this.m_WorldScale.y;
		Camera main = Camera.main;
		if (main != null)
		{
			this.m_Camera.depth = main.depth - 2f;
		}
		this.m_Camera.clearFlags = 2;
		this.m_Camera.backgroundColor = this.m_ClearColor;
		this.m_Camera.depthTextureMode = 0;
		this.m_Camera.renderingPath = 1;
		this.m_Camera.cullingMask = this.m_LayerMask;
		this.m_Camera.targetTexture = this.m_RenderTexture;
		this.m_Camera.enabled = false;
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x000D4CF8 File Offset: 0x000D2EF8
	private float OrthoSize()
	{
		float result;
		if (this.m_WorldWidth > this.m_WorldHeight)
		{
			result = Mathf.Min(this.m_WorldWidth * 0.5f, this.m_WorldHeight * 0.5f);
		}
		else
		{
			result = this.m_WorldHeight * 0.5f;
		}
		return result;
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x000D4D50 File Offset: 0x000D2F50
	private void CameraRender()
	{
		this.m_Camera.orthographicSize = this.OrthoSize();
		this.m_Camera.farClipPlane = this.m_FarClip * this.m_WorldScale.z;
		this.m_Camera.nearClipPlane = this.m_NearClip * this.m_WorldScale.z;
		if (this.m_PlaneGameObject && !this.m_HideRenderObject)
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().enabled = false;
			if (this.m_BloomPlaneGameObject)
			{
				this.m_BloomPlaneGameObject.GetComponent<Renderer>().enabled = false;
			}
		}
		if (this.m_ReplacmentShader)
		{
			this.m_Camera.RenderWithShader(this.m_ReplacmentShader, this.m_ReplacmentTag);
		}
		else
		{
			this.m_Camera.Render();
		}
		if (this.m_PlaneGameObject && !this.m_HideRenderObject)
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().enabled = true;
			if (this.m_BloomPlaneGameObject)
			{
				this.m_BloomPlaneGameObject.GetComponent<Renderer>().enabled = true;
			}
		}
	}

	// Token: 0x06002AF2 RID: 10994 RVA: 0x000D4E7C File Offset: 0x000D307C
	private void CreateAlphaCamera()
	{
		if (this.m_AlphaCamera != null)
		{
			return;
		}
		this.m_AlphaCameraGO = new GameObject();
		this.m_AlphaCamera = this.m_AlphaCameraGO.AddComponent<Camera>();
		this.m_AlphaCameraGO.name = base.name + "_R2TAlphaRenderCamera";
		this.m_AlphaCamera.CopyFrom(this.m_Camera);
		this.m_AlphaCamera.enabled = false;
		this.m_AlphaCamera.backgroundColor = Color.clear;
		this.m_AlphaCameraGO.transform.parent = this.m_CameraGO.transform;
		if (this.m_AlphaObjectToRender)
		{
			this.m_AlphaCameraGO.transform.position = this.m_CameraGO.transform.position - this.ALPHA_OBJECT_OFFSET;
		}
		else
		{
			this.m_AlphaCameraGO.transform.position = this.m_CameraGO.transform.position;
		}
		this.m_AlphaCameraGO.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06002AF3 RID: 10995 RVA: 0x000D4F90 File Offset: 0x000D3190
	private void AlphaCameraRender()
	{
		this.m_AlphaCamera.orthographicSize = this.OrthoSize();
		this.m_AlphaCamera.farClipPlane = this.m_FarClip * this.m_WorldScale.z;
		this.m_AlphaCamera.nearClipPlane = this.m_NearClip * this.m_WorldScale.z;
		if (this.m_PlaneGameObject && !this.m_HideRenderObject)
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().enabled = false;
			if (this.m_BloomPlaneGameObject)
			{
				this.m_BloomPlaneGameObject.GetComponent<Renderer>().enabled = false;
			}
		}
		if (this.m_AlphaObjectToRender == null)
		{
			this.m_AlphaCamera.RenderWithShader(this.m_UnlitWhiteShader, this.m_ReplacmentTag);
		}
		else
		{
			this.m_AlphaCamera.Render();
		}
		if (this.m_PlaneGameObject && !this.m_HideRenderObject)
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().enabled = true;
			if (this.m_BloomPlaneGameObject)
			{
				this.m_BloomPlaneGameObject.GetComponent<Renderer>().enabled = true;
			}
		}
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x000D50BC File Offset: 0x000D32BC
	private void CreateBloomCaptureCamera()
	{
		if (this.m_BloomCaptureCamera != null)
		{
			return;
		}
		this.m_BloomCaptureCameraGO = new GameObject();
		this.m_BloomCaptureCamera = this.m_BloomCaptureCameraGO.AddComponent<Camera>();
		this.m_BloomCaptureCameraGO.name = base.name + "_R2TBloomRenderCamera";
		this.m_BloomCaptureCamera.CopyFrom(this.m_Camera);
		this.m_BloomCaptureCamera.enabled = false;
		this.m_BloomCaptureCamera.depth = this.m_Camera.depth + 1f;
		this.m_BloomCaptureCameraGO.transform.parent = this.m_Camera.transform;
		this.m_BloomCaptureCameraGO.transform.localPosition = Vector3.zero;
		this.m_BloomCaptureCameraGO.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06002AF5 RID: 10997 RVA: 0x000D5190 File Offset: 0x000D3390
	private Vector2 CalcTextureSize()
	{
		Vector2 result;
		result..ctor((float)this.m_Resolution, (float)this.m_Resolution);
		if (this.m_WorldWidth > this.m_WorldHeight)
		{
			result.x = (float)this.m_Resolution;
			result.y = (float)this.m_Resolution * (this.m_WorldHeight / this.m_WorldWidth);
		}
		else
		{
			result.x = (float)this.m_Resolution * (this.m_WorldWidth / this.m_WorldHeight);
			result.y = (float)this.m_Resolution;
		}
		return result;
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x000D5220 File Offset: 0x000D3420
	private void CreateRenderPlane()
	{
		if (this.m_PlaneGameObject != null)
		{
			Object.DestroyImmediate(this.m_PlaneGameObject);
		}
		this.m_PlaneGameObject = this.CreateMeshPlane(string.Format("{0}_RenderPlane", base.name), this.m_Material);
		SceneUtils.SetHideFlags(this.m_PlaneGameObject, 52);
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x000D5278 File Offset: 0x000D3478
	private void CreateBloomPlane()
	{
		if (this.m_BloomPlaneGameObject != null)
		{
			Object.DestroyImmediate(this.m_BloomPlaneGameObject);
		}
		Material material = this.AdditiveMaterial;
		if (this.m_BloomBlend == RenderToTexture.BloomBlendType.Transparent)
		{
			material = this.TransparentMaterial;
		}
		this.m_BloomPlaneGameObject = this.CreateMeshPlane(string.Format("{0}_BloomRenderPlane", base.name), material);
		this.m_BloomPlaneGameObject.transform.parent = this.m_PlaneGameObject.transform;
		this.m_BloomPlaneGameObject.transform.localPosition = new Vector3(0f, 0.15f, 0f);
		this.m_BloomPlaneGameObject.transform.localRotation = Quaternion.identity;
		this.m_BloomPlaneGameObject.transform.localScale = Vector3.one;
		this.m_BloomPlaneGameObject.GetComponent<Renderer>().material.color = this.m_BloomColor;
	}

	// Token: 0x06002AF8 RID: 11000 RVA: 0x000D535C File Offset: 0x000D355C
	private void CreateBloomCapturePlane()
	{
		if (this.m_BloomCapturePlaneGameObject != null)
		{
			Object.DestroyImmediate(this.m_BloomCapturePlaneGameObject);
		}
		Material material = this.AdditiveMaterial;
		if (this.m_BloomBlend == RenderToTexture.BloomBlendType.Transparent)
		{
			material = this.TransparentMaterial;
		}
		this.m_BloomCapturePlaneGameObject = this.CreateMeshPlane(string.Format("{0}_BloomCaptureRenderPlane", base.name), material);
		this.m_BloomCapturePlaneGameObject.transform.parent = this.m_BloomCaptureCameraGO.transform;
		this.m_BloomCapturePlaneGameObject.transform.localPosition = Vector3.zero;
		this.m_BloomCapturePlaneGameObject.transform.localRotation = Quaternion.identity;
		this.m_BloomCapturePlaneGameObject.transform.Rotate(-90f, 0f, 0f);
		this.m_BloomCapturePlaneGameObject.transform.localScale = this.m_WorldScale;
		if (this.m_Material)
		{
			this.m_BloomCapturePlaneGameObject.GetComponent<Renderer>().material = this.m_PlaneGameObject.GetComponent<Renderer>().material;
		}
		if (this.m_RenderTexture)
		{
			this.m_BloomCapturePlaneGameObject.GetComponent<Renderer>().material.mainTexture = this.m_RenderTexture;
		}
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x000D5494 File Offset: 0x000D3694
	private GameObject CreateMeshPlane(string name, Material material)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = name;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = this.m_PositionOffset;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh();
		float num = this.m_Width * 0.5f;
		float num2 = this.m_Height * 0.5f;
		mesh.vertices = new Vector3[]
		{
			new Vector3(-num, 0f, -num2),
			new Vector3(num, 0f, -num2),
			new Vector3(-num, 0f, num2),
			new Vector3(num, 0f, num2)
		};
		mesh.uv = this.PLANE_UVS;
		mesh.normals = this.PLANE_NORMALS;
		mesh.triangles = this.PLANE_TRIANGLES;
		Mesh mesh2 = mesh;
		gameObject.GetComponent<MeshFilter>().mesh = mesh2;
		Mesh mesh3 = mesh2;
		mesh3.RecalculateBounds();
		if (material)
		{
			gameObject.GetComponent<Renderer>().material = material;
		}
		gameObject.GetComponent<Renderer>().material.renderQueue = this.m_RenderQueueOffset + this.m_RenderQueue;
		return gameObject;
	}

	// Token: 0x06002AFA RID: 11002 RVA: 0x000D5604 File Offset: 0x000D3804
	private void Sample(RenderTexture source, RenderTexture dest, Material sampleMat, float offset)
	{
		Graphics.BlitMultiTap(source, dest, sampleMat, new Vector2[]
		{
			new Vector2(-offset, -offset),
			new Vector2(-offset, offset),
			new Vector2(offset, offset),
			new Vector2(offset, -offset)
		});
	}

	// Token: 0x06002AFB RID: 11003 RVA: 0x000D5678 File Offset: 0x000D3878
	private void CalcWorldWidthHeightScale()
	{
		Quaternion rotation = base.transform.rotation;
		Vector3 localScale = base.transform.localScale;
		Transform parent = base.transform.parent;
		base.transform.rotation = Quaternion.identity;
		bool flag = false;
		if (base.transform.lossyScale.magnitude == 0f)
		{
			base.transform.parent = null;
			base.transform.localScale = Vector3.one;
			flag = true;
		}
		this.m_WorldScale = base.transform.lossyScale;
		this.m_WorldWidth = this.m_Width * this.m_WorldScale.x;
		this.m_WorldHeight = this.m_Height * this.m_WorldScale.y;
		if (flag)
		{
			base.transform.parent = parent;
			base.transform.localScale = localScale;
		}
		base.transform.rotation = rotation;
		if (this.m_WorldWidth == 0f || this.m_WorldHeight == 0f)
		{
			Debug.LogError(string.Format(" \"{0}\": RenderToTexture has a world scale of zero. \nm_WorldWidth: {1},   m_WorldHeight: {2}", this.m_WorldWidth, this.m_WorldHeight));
		}
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x000D57A8 File Offset: 0x000D39A8
	private void CleanUp()
	{
		this.ReleaseTexture();
		if (this.m_CameraGO)
		{
			Object.Destroy(this.m_CameraGO);
		}
		if (this.m_AlphaCameraGO)
		{
			Object.Destroy(this.m_AlphaCameraGO);
		}
		if (this.m_PlaneGameObject)
		{
			Object.Destroy(this.m_PlaneGameObject);
		}
		if (this.m_BloomPlaneGameObject)
		{
			Object.Destroy(this.m_BloomPlaneGameObject);
		}
		if (this.m_BloomCaptureCameraGO)
		{
			Object.Destroy(this.m_BloomCaptureCameraGO);
		}
		if (this.m_BloomCapturePlaneGameObject)
		{
			Object.Destroy(this.m_BloomCapturePlaneGameObject);
		}
		if (this.m_OffscreenGameObject)
		{
			Object.Destroy(this.m_OffscreenGameObject);
		}
		if (this.m_BlurMaterial)
		{
			Object.Destroy(this.m_BlurMaterial);
		}
		if (this.m_AlphaBlurMaterial)
		{
			Object.Destroy(this.m_AlphaBlurMaterial);
		}
		if (this.m_AlphaBlendMaterial)
		{
			Object.Destroy(this.m_AlphaBlendMaterial);
		}
		if (this.m_ObjectToRender != null)
		{
			if (this.m_ObjectToRenderOrgParent != null)
			{
				this.m_ObjectToRender.transform.parent = this.m_ObjectToRenderOrgParent;
			}
			this.m_ObjectToRender.transform.localPosition = this.m_ObjectToRenderOrgPosition;
		}
		this.m_init = false;
		this.m_isDirty = true;
	}

	// Token: 0x040019A3 RID: 6563
	private const string BLUR_SHADER_NAME = "Hidden/R2TBlur";

	// Token: 0x040019A4 RID: 6564
	private const string BLUR_ALPHA_SHADER_NAME = "Hidden/R2TAlphaBlur";

	// Token: 0x040019A5 RID: 6565
	private const string ALPHA_BLEND_SHADER_NAME = "Hidden/R2TColorAlphaCombine";

	// Token: 0x040019A6 RID: 6566
	private const string UNLIT_WHITE_SHADER_NAME = "Custom/Unlit/Color/White";

	// Token: 0x040019A7 RID: 6567
	private const string BLOOM_SHADER_NAME = "Hidden/R2TBloom";

	// Token: 0x040019A8 RID: 6568
	private const string BLOOM_ALPHA_SHADER_NAME = "Hidden/R2TBloomAlpha";

	// Token: 0x040019A9 RID: 6569
	private const string ADDITIVE_SHADER_NAME = "Hidden/R2TAdditive";

	// Token: 0x040019AA RID: 6570
	private const string TRANSPARENT_SHADER_NAME = "Hidden/R2TTransparent";

	// Token: 0x040019AB RID: 6571
	private const string ALPHA_CLIP_SHADER_NAME = "Hidden/R2TAlphaClip";

	// Token: 0x040019AC RID: 6572
	private const string ALPHA_CLIP_BLOOM_SHADER_NAME = "Hidden/R2TAlphaClipBloom";

	// Token: 0x040019AD RID: 6573
	private const string ALPHA_CLIP_GRADIENT_SHADER_NAME = "Hidden/R2TAlphaClipGradient";

	// Token: 0x040019AE RID: 6574
	private const RenderTextureFormat ALPHA_TEXTURE_FORMAT = 7;

	// Token: 0x040019AF RID: 6575
	private const float OFFSET_DISTANCE = 100f;

	// Token: 0x040019B0 RID: 6576
	private const float RENDER_SIZE_QUALITY_LOW = 0.75f;

	// Token: 0x040019B1 RID: 6577
	private const float RENDER_SIZE_QUALITY_MEDIUM = 1f;

	// Token: 0x040019B2 RID: 6578
	private const float RENDER_SIZE_QUALITY_HIGH = 1.5f;

	// Token: 0x040019B3 RID: 6579
	private readonly Vector3 ALPHA_OBJECT_OFFSET = new Vector3(0f, 1000f, 0f);

	// Token: 0x040019B4 RID: 6580
	private readonly Vector2[] PLANE_UVS = new Vector2[]
	{
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f)
	};

	// Token: 0x040019B5 RID: 6581
	private readonly Vector3[] PLANE_NORMALS = new Vector3[]
	{
		Vector3.up,
		Vector3.up,
		Vector3.up,
		Vector3.up
	};

	// Token: 0x040019B6 RID: 6582
	private readonly int[] PLANE_TRIANGLES = new int[]
	{
		3,
		1,
		2,
		2,
		1,
		0
	};

	// Token: 0x040019B7 RID: 6583
	public GameObject m_ObjectToRender;

	// Token: 0x040019B8 RID: 6584
	public GameObject m_AlphaObjectToRender;

	// Token: 0x040019B9 RID: 6585
	public bool m_HideRenderObject = true;

	// Token: 0x040019BA RID: 6586
	public bool m_RealtimeRender;

	// Token: 0x040019BB RID: 6587
	public bool m_RealtimeTranslation;

	// Token: 0x040019BC RID: 6588
	public RenderToTexture.RenderToTextureMaterial m_RenderMaterial;

	// Token: 0x040019BD RID: 6589
	public Material m_Material;

	// Token: 0x040019BE RID: 6590
	public bool m_CreateRenderPlane = true;

	// Token: 0x040019BF RID: 6591
	public GameObject m_RenderToObject;

	// Token: 0x040019C0 RID: 6592
	public string m_ShaderTextureName = string.Empty;

	// Token: 0x040019C1 RID: 6593
	public bool m_RenderMeshAsAlpha;

	// Token: 0x040019C2 RID: 6594
	public int m_Resolution = 128;

	// Token: 0x040019C3 RID: 6595
	public float m_Width = 1f;

	// Token: 0x040019C4 RID: 6596
	public float m_Height = 1f;

	// Token: 0x040019C5 RID: 6597
	public float m_NearClip = -0.1f;

	// Token: 0x040019C6 RID: 6598
	public float m_FarClip = 0.5f;

	// Token: 0x040019C7 RID: 6599
	public float m_BloomIntensity;

	// Token: 0x040019C8 RID: 6600
	public float m_BloomThreshold = 0.7f;

	// Token: 0x040019C9 RID: 6601
	public float m_BloomBlur = 0.3f;

	// Token: 0x040019CA RID: 6602
	public float m_BloomResolutionRatio = 0.5f;

	// Token: 0x040019CB RID: 6603
	public RenderToTexture.BloomRenderType m_BloomRenderType;

	// Token: 0x040019CC RID: 6604
	public float m_BloomAlphaIntensity = 1f;

	// Token: 0x040019CD RID: 6605
	public RenderToTexture.BloomBlendType m_BloomBlend;

	// Token: 0x040019CE RID: 6606
	public Color m_BloomColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	// Token: 0x040019CF RID: 6607
	public RenderToTexture.AlphaClipShader m_AlphaClipRenderStyle;

	// Token: 0x040019D0 RID: 6608
	public float m_AlphaClip = 15f;

	// Token: 0x040019D1 RID: 6609
	public float m_AlphaClipIntensity = 1.5f;

	// Token: 0x040019D2 RID: 6610
	public float m_AlphaClipAlphaIntensity = 1f;

	// Token: 0x040019D3 RID: 6611
	public Texture2D m_AlphaClipGradientMap;

	// Token: 0x040019D4 RID: 6612
	public float m_BlurAmount;

	// Token: 0x040019D5 RID: 6613
	public bool m_BlurAlphaOnly;

	// Token: 0x040019D6 RID: 6614
	public Color m_TintColor = Color.white;

	// Token: 0x040019D7 RID: 6615
	public int m_RenderQueueOffset = 3000;

	// Token: 0x040019D8 RID: 6616
	public int m_RenderQueue;

	// Token: 0x040019D9 RID: 6617
	public Color m_ClearColor = Color.clear;

	// Token: 0x040019DA RID: 6618
	public Shader m_ReplacmentShader;

	// Token: 0x040019DB RID: 6619
	public string m_ReplacmentTag;

	// Token: 0x040019DC RID: 6620
	public RenderTextureFormat m_RenderTextureFormat = 7;

	// Token: 0x040019DD RID: 6621
	public Vector3 m_PositionOffset = Vector3.zero;

	// Token: 0x040019DE RID: 6622
	public LayerMask m_LayerMask = -1;

	// Token: 0x040019DF RID: 6623
	public bool m_LateUpdate;

	// Token: 0x040019E0 RID: 6624
	public bool m_RenderOnStart = true;

	// Token: 0x040019E1 RID: 6625
	public bool m_RenderOnEnable = true;

	// Token: 0x040019E2 RID: 6626
	private bool m_renderEnabled = true;

	// Token: 0x040019E3 RID: 6627
	private bool m_init;

	// Token: 0x040019E4 RID: 6628
	private float m_WorldWidth;

	// Token: 0x040019E5 RID: 6629
	private float m_WorldHeight;

	// Token: 0x040019E6 RID: 6630
	private Vector3 m_WorldScale;

	// Token: 0x040019E7 RID: 6631
	private GameObject m_OffscreenGameObject;

	// Token: 0x040019E8 RID: 6632
	private GameObject m_CameraGO;

	// Token: 0x040019E9 RID: 6633
	private Camera m_Camera;

	// Token: 0x040019EA RID: 6634
	private GameObject m_AlphaCameraGO;

	// Token: 0x040019EB RID: 6635
	private Camera m_AlphaCamera;

	// Token: 0x040019EC RID: 6636
	private GameObject m_BloomCaptureCameraGO;

	// Token: 0x040019ED RID: 6637
	private Camera m_BloomCaptureCamera;

	// Token: 0x040019EE RID: 6638
	private RenderTexture m_RenderTexture;

	// Token: 0x040019EF RID: 6639
	private RenderTexture m_AlphaRenderTexture;

	// Token: 0x040019F0 RID: 6640
	private RenderTexture m_BloomRenderTexture;

	// Token: 0x040019F1 RID: 6641
	private RenderTexture m_BloomRenderBuffer1;

	// Token: 0x040019F2 RID: 6642
	private RenderTexture m_BloomRenderBuffer2;

	// Token: 0x040019F3 RID: 6643
	private GameObject m_PlaneGameObject;

	// Token: 0x040019F4 RID: 6644
	private GameObject m_BloomPlaneGameObject;

	// Token: 0x040019F5 RID: 6645
	private GameObject m_BloomCapturePlaneGameObject;

	// Token: 0x040019F6 RID: 6646
	private bool m_ObjectToRenderOrgPositionStored;

	// Token: 0x040019F7 RID: 6647
	private Transform m_ObjectToRenderOrgParent;

	// Token: 0x040019F8 RID: 6648
	private Vector3 m_ObjectToRenderOrgPosition = Vector3.zero;

	// Token: 0x040019F9 RID: 6649
	private Vector3 m_OriginalRenderPosition = Vector3.zero;

	// Token: 0x040019FA RID: 6650
	private bool m_isDirty;

	// Token: 0x040019FB RID: 6651
	private Shader m_UnlitWhiteShader;

	// Token: 0x040019FC RID: 6652
	private Vector3 m_OffscreenPos;

	// Token: 0x040019FD RID: 6653
	private Vector3 m_ObjectToRenderOffset = Vector3.zero;

	// Token: 0x040019FE RID: 6654
	private Vector3 m_AlphaObjectToRenderOffset = Vector3.zero;

	// Token: 0x040019FF RID: 6655
	private RenderToTexture.RenderToTextureMaterial m_PreviousRenderMaterial;

	// Token: 0x04001A00 RID: 6656
	private Vector3 m_Offset = Vector3.zero;

	// Token: 0x04001A01 RID: 6657
	private static Vector3 s_offset = new Vector3(-4000f, -4000f, -4000f);

	// Token: 0x04001A02 RID: 6658
	private Shader m_AlphaBlendShader;

	// Token: 0x04001A03 RID: 6659
	private Material m_AlphaBlendMaterial;

	// Token: 0x04001A04 RID: 6660
	private Shader m_AdditiveShader;

	// Token: 0x04001A05 RID: 6661
	private Material m_AdditiveMaterial;

	// Token: 0x04001A06 RID: 6662
	private Shader m_BloomShader;

	// Token: 0x04001A07 RID: 6663
	private Material m_BloomMaterial;

	// Token: 0x04001A08 RID: 6664
	private Shader m_BloomShaderAlpha;

	// Token: 0x04001A09 RID: 6665
	private Material m_BloomMaterialAlpha;

	// Token: 0x04001A0A RID: 6666
	private Shader m_BlurShader;

	// Token: 0x04001A0B RID: 6667
	private Material m_BlurMaterial;

	// Token: 0x04001A0C RID: 6668
	private Shader m_AlphaBlurShader;

	// Token: 0x04001A0D RID: 6669
	private Material m_AlphaBlurMaterial;

	// Token: 0x04001A0E RID: 6670
	private Shader m_TransparentShader;

	// Token: 0x04001A0F RID: 6671
	private Material m_TransparentMaterial;

	// Token: 0x04001A10 RID: 6672
	private Shader m_AlphaClipShader;

	// Token: 0x04001A11 RID: 6673
	private Material m_AlphaClipMaterial;

	// Token: 0x04001A12 RID: 6674
	private Shader m_AlphaClipBloomShader;

	// Token: 0x04001A13 RID: 6675
	private Material m_AlphaClipBloomMaterial;

	// Token: 0x04001A14 RID: 6676
	private Shader m_AlphaClipGradientShader;

	// Token: 0x04001A15 RID: 6677
	private Material m_AlphaClipGradientMaterial;

	// Token: 0x020006B5 RID: 1717
	public enum RenderToTextureMaterial
	{
		// Token: 0x04002F1F RID: 12063
		Custom,
		// Token: 0x04002F20 RID: 12064
		Transparent,
		// Token: 0x04002F21 RID: 12065
		Additive,
		// Token: 0x04002F22 RID: 12066
		Bloom,
		// Token: 0x04002F23 RID: 12067
		AlphaClip,
		// Token: 0x04002F24 RID: 12068
		AlphaClipBloom
	}

	// Token: 0x020006B6 RID: 1718
	public enum AlphaClipShader
	{
		// Token: 0x04002F26 RID: 12070
		Standard,
		// Token: 0x04002F27 RID: 12071
		ColorGradient
	}

	// Token: 0x020006B7 RID: 1719
	public enum BloomRenderType
	{
		// Token: 0x04002F29 RID: 12073
		Color,
		// Token: 0x04002F2A RID: 12074
		Alpha
	}

	// Token: 0x020006B8 RID: 1720
	public enum BloomBlendType
	{
		// Token: 0x04002F2C RID: 12076
		Additive,
		// Token: 0x04002F2D RID: 12077
		Transparent
	}
}
