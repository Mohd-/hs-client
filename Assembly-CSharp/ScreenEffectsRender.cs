using System;
using UnityEngine;

// Token: 0x02000EE8 RID: 3816
[ExecuteInEditMode]
public class ScreenEffectsRender : MonoBehaviour
{
	// Token: 0x170009E1 RID: 2529
	// (get) Token: 0x0600723A RID: 29242 RVA: 0x002192FC File Offset: 0x002174FC
	protected Material GlowMaterial
	{
		get
		{
			if (this.m_GlowMaterial == null)
			{
				if (this.m_GlowShader == null)
				{
					this.m_GlowShader = Shader.Find("Hidden/ScreenEffectsGlow");
					if (!this.m_GlowShader)
					{
						Debug.LogError("Failed to load ScreenEffectsRender Shader: Hidden/ScreenEffectsGlow");
					}
				}
				this.m_GlowMaterial = new Material(this.m_GlowShader);
				SceneUtils.SetHideFlags(this.m_GlowMaterial, 61);
			}
			return this.m_GlowMaterial;
		}
	}

	// Token: 0x170009E2 RID: 2530
	// (get) Token: 0x0600723B RID: 29243 RVA: 0x0021937C File Offset: 0x0021757C
	protected Material DistortionMaterial
	{
		get
		{
			if (this.m_DistortionMaterial == null)
			{
				if (this.m_DistortionShader == null)
				{
					this.m_DistortionShader = Shader.Find("Hidden/ScreenEffectDistortion");
					if (!this.m_DistortionShader)
					{
						Debug.LogError("Failed to load ScreenEffectsRender Shader: Hidden/ScreenEffectDistortion");
					}
				}
				this.m_DistortionMaterial = new Material(this.m_DistortionShader);
				SceneUtils.SetHideFlags(this.m_DistortionMaterial, 61);
			}
			return this.m_DistortionMaterial;
		}
	}

	// Token: 0x0600723C RID: 29244 RVA: 0x002193FC File Offset: 0x002175FC
	private void Awake()
	{
		if (ScreenEffectsMgr.Get() == null)
		{
			base.enabled = false;
		}
		if (this.m_MaskShader == null)
		{
			this.m_MaskShader = Shader.Find("Hidden/ScreenEffectsMask");
			if (!this.m_MaskShader)
			{
				Debug.LogError("Failed to load ScreenEffectsRender Shader: Hidden/ScreenEffectsMask");
			}
		}
	}

	// Token: 0x0600723D RID: 29245 RVA: 0x0021945B File Offset: 0x0021765B
	private void Start()
	{
	}

	// Token: 0x0600723E RID: 29246 RVA: 0x0021945D File Offset: 0x0021765D
	private void Update()
	{
	}

	// Token: 0x0600723F RID: 29247 RVA: 0x00219460 File Offset: 0x00217660
	private void OnDisable()
	{
		Object.DestroyImmediate(this.GlowMaterial);
		Object.DestroyImmediate(this.DistortionMaterial);
		if (this.m_MaskRenderTexture != null)
		{
			Object.DestroyImmediate(this.m_MaskRenderTexture);
			this.m_MaskRenderTexture = null;
		}
		if (this.m_GlowMaterial != null)
		{
			Object.DestroyImmediate(this.m_GlowMaterial);
		}
		if (this.m_DistortionMaterial != null)
		{
			Object.DestroyImmediate(this.m_DistortionMaterial);
		}
	}

	// Token: 0x06007240 RID: 29248 RVA: 0x002194DE File Offset: 0x002176DE
	private void OnDestroy()
	{
	}

	// Token: 0x06007241 RID: 29249 RVA: 0x002194E0 File Offset: 0x002176E0
	private void OnEnable()
	{
		this.SetupEffect();
	}

	// Token: 0x06007242 RID: 29250 RVA: 0x002194E8 File Offset: 0x002176E8
	private void OnPreRender()
	{
		this.RenderEffectsObjects();
	}

	// Token: 0x06007243 RID: 29251 RVA: 0x002194F0 File Offset: 0x002176F0
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.RenderGlow(source, destination);
	}

	// Token: 0x06007244 RID: 29252 RVA: 0x002194FA File Offset: 0x002176FA
	private void SetupEffect()
	{
	}

	// Token: 0x06007245 RID: 29253 RVA: 0x002194FC File Offset: 0x002176FC
	private void RenderEffectsObjects()
	{
		if (this.m_EffectsObjectsCamera == null)
		{
			base.enabled = false;
			return;
		}
		float num = (float)Screen.width / (float)Screen.height;
		int num2 = (int)(512f * num);
		int num3 = 512;
		if (num2 != this.m_previousWidth || num3 != this.m_previousHeight)
		{
			Object.DestroyImmediate(this.m_MaskRenderTexture);
			this.m_MaskRenderTexture = null;
		}
		if (this.m_MaskRenderTexture == null)
		{
			this.m_MaskRenderTexture = new RenderTexture(num2, num3, 32);
			this.m_MaskRenderTexture.filterMode = 1;
			this.m_previousWidth = num2;
			this.m_previousHeight = num3;
		}
		if (this.m_EffectsObjectsCamera.targetTexture == null && this.m_MaskRenderTexture != null)
		{
			this.m_EffectsObjectsCamera.targetTexture = this.m_MaskRenderTexture;
		}
		this.m_MaskRenderTexture.DiscardContents();
		this.m_EffectsObjectsCamera.RenderWithShader(this.m_MaskShader, "RenderType");
	}

	// Token: 0x06007246 RID: 29254 RVA: 0x002195FC File Offset: 0x002177FC
	private void RenderGlow(RenderTexture source, RenderTexture destination)
	{
		int width = this.m_MaskRenderTexture.width;
		int height = this.m_MaskRenderTexture.height;
		RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
		temporary.filterMode = 1;
		this.GlowMaterial.SetFloat("_BlurOffset", 1f);
		Graphics.Blit(this.m_MaskRenderTexture, temporary, this.GlowMaterial, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, source.format);
		temporary2.filterMode = 1;
		this.GlowMaterial.SetFloat("_BlurOffset", 2f);
		Graphics.Blit(temporary, temporary2, this.GlowMaterial, 0);
		this.GlowMaterial.SetFloat("_BlurOffset", 3f);
		temporary.DiscardContents();
		Graphics.Blit(temporary2, temporary, this.GlowMaterial, 0);
		this.GlowMaterial.SetFloat("_BlurOffset", 5f);
		temporary2.DiscardContents();
		Graphics.Blit(temporary, temporary2, this.GlowMaterial, 0);
		this.GlowMaterial.SetTexture("_BlurTex", temporary2);
		if (!this.m_Debug)
		{
			Graphics.Blit(source, destination, this.GlowMaterial, 1);
		}
		else
		{
			Graphics.Blit(source, destination, this.GlowMaterial, 2);
		}
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
	}

	// Token: 0x06007247 RID: 29255 RVA: 0x00219734 File Offset: 0x00217934
	private void SetupDistortion()
	{
		if (this.m_EffectsTexture == null)
		{
			return;
		}
		this.DistortionMaterial.SetTexture("_EffectTex", this.m_EffectsTexture);
	}

	// Token: 0x06007248 RID: 29256 RVA: 0x00219769 File Offset: 0x00217969
	private Material DistortionMaterialRender(RenderTexture source)
	{
		return this.DistortionMaterial;
	}

	// Token: 0x06007249 RID: 29257 RVA: 0x00219771 File Offset: 0x00217971
	private void RenderDistortion(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, this.DistortionMaterialRender(source));
	}

	// Token: 0x04005C56 RID: 23638
	private const int GLOW_RANDER_BUFFER_RESOLUTION = 512;

	// Token: 0x04005C57 RID: 23639
	private const string BLOOM_SHADER_NAME = "Hidden/ScreenEffectsGlow";

	// Token: 0x04005C58 RID: 23640
	private const string DISTORTION_SHADER_NAME = "Hidden/ScreenEffectDistortion";

	// Token: 0x04005C59 RID: 23641
	private const string GLOW_MASK_SHADER = "Hidden/ScreenEffectsMask";

	// Token: 0x04005C5A RID: 23642
	[HideInInspector]
	public RenderTexture m_EffectsTexture;

	// Token: 0x04005C5B RID: 23643
	[HideInInspector]
	public Camera m_EffectsObjectsCamera;

	// Token: 0x04005C5C RID: 23644
	public bool m_Debug;

	// Token: 0x04005C5D RID: 23645
	private int m_width;

	// Token: 0x04005C5E RID: 23646
	private int m_height;

	// Token: 0x04005C5F RID: 23647
	private int m_previousWidth;

	// Token: 0x04005C60 RID: 23648
	private int m_previousHeight;

	// Token: 0x04005C61 RID: 23649
	private RenderTexture m_MaskRenderTexture;

	// Token: 0x04005C62 RID: 23650
	private Shader m_MaskShader;

	// Token: 0x04005C63 RID: 23651
	private Shader m_GlowShader;

	// Token: 0x04005C64 RID: 23652
	private Material m_GlowMaterial;

	// Token: 0x04005C65 RID: 23653
	private Shader m_DistortionShader;

	// Token: 0x04005C66 RID: 23654
	private Material m_DistortionMaterial;
}
