using System;
using UnityEngine;

// Token: 0x02000266 RID: 614
public class FullScreenEffects : MonoBehaviour
{
	// Token: 0x1700033E RID: 830
	// (get) Token: 0x06002266 RID: 8806 RVA: 0x000A9260 File Offset: 0x000A7460
	protected Material blurMaterial
	{
		get
		{
			if (this.m_BlurMaterial == null)
			{
				this.m_BlurMaterial = new Material(this.m_BlurShader);
				SceneUtils.SetHideFlags(this.m_BlurMaterial, 52);
			}
			return this.m_BlurMaterial;
		}
	}

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x06002267 RID: 8807 RVA: 0x000A9297 File Offset: 0x000A7497
	protected Material blurVignettingMaterial
	{
		get
		{
			if (this.m_BlurVignettingMaterial == null)
			{
				this.m_BlurVignettingMaterial = new Material(this.m_BlurVignettingShader);
				SceneUtils.SetHideFlags(this.m_BlurVignettingMaterial, 52);
			}
			return this.m_BlurVignettingMaterial;
		}
	}

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06002268 RID: 8808 RVA: 0x000A92CE File Offset: 0x000A74CE
	protected Material blurDesatMaterial
	{
		get
		{
			if (this.m_BlurDesatMaterial == null)
			{
				this.m_BlurDesatMaterial = new Material(this.m_BlurDesaturationShader);
				SceneUtils.SetHideFlags(this.m_BlurDesatMaterial, 52);
			}
			return this.m_BlurDesatMaterial;
		}
	}

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06002269 RID: 8809 RVA: 0x000A9305 File Offset: 0x000A7505
	protected Material blendMaterial
	{
		get
		{
			if (this.m_BlendMaterial == null)
			{
				this.m_BlendMaterial = new Material(this.m_BlendShader);
				SceneUtils.SetHideFlags(this.m_BlendMaterial, 52);
			}
			return this.m_BlendMaterial;
		}
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x0600226A RID: 8810 RVA: 0x000A933C File Offset: 0x000A753C
	protected Material VignettingMaterial
	{
		get
		{
			if (this.m_VignettingMaterial == null)
			{
				this.m_VignettingMaterial = new Material(this.m_VignettingShader);
				SceneUtils.SetHideFlags(this.m_VignettingMaterial, 52);
			}
			return this.m_VignettingMaterial;
		}
	}

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x0600226B RID: 8811 RVA: 0x000A9373 File Offset: 0x000A7573
	protected Material BlendToColorMaterial
	{
		get
		{
			if (this.m_BlendToColorMaterial == null)
			{
				this.m_BlendToColorMaterial = new Material(this.m_BlendToColorShader);
				SceneUtils.SetHideFlags(this.m_BlendToColorMaterial, 52);
			}
			return this.m_BlendToColorMaterial;
		}
	}

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x0600226C RID: 8812 RVA: 0x000A93AA File Offset: 0x000A75AA
	protected Material DesaturationMaterial
	{
		get
		{
			if (this.m_DesaturationMaterial == null)
			{
				this.m_DesaturationMaterial = new Material(this.m_DesaturationShader);
				SceneUtils.SetHideFlags(this.m_DesaturationMaterial, 52);
			}
			return this.m_DesaturationMaterial;
		}
	}

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x0600226D RID: 8813 RVA: 0x000A93E1 File Offset: 0x000A75E1
	protected Material DesaturationVignettingMaterial
	{
		get
		{
			if (this.m_DesaturationVignettingMaterial == null)
			{
				this.m_DesaturationVignettingMaterial = new Material(this.m_DesaturationVignettingShader);
				SceneUtils.SetHideFlags(this.m_DesaturationVignettingMaterial, 52);
			}
			return this.m_DesaturationVignettingMaterial;
		}
	}

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x0600226E RID: 8814 RVA: 0x000A9418 File Offset: 0x000A7618
	protected Material BlurDesaturationVignettingMaterial
	{
		get
		{
			if (this.m_BlurDesaturationVignettingMaterial == null)
			{
				this.m_BlurDesaturationVignettingMaterial = new Material(this.m_BlurDesaturationVignettingShader);
				SceneUtils.SetHideFlags(this.m_BlurDesaturationVignettingMaterial, 52);
			}
			return this.m_BlurDesaturationVignettingMaterial;
		}
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x000A9450 File Offset: 0x000A7650
	protected void OnDisable()
	{
		this.SetDefaults();
		if (this.m_BlurMaterial)
		{
			Object.Destroy(this.m_BlurMaterial);
		}
		if (this.m_BlurVignettingMaterial)
		{
			Object.Destroy(this.m_BlurVignettingMaterial);
		}
		if (this.m_BlurDesatMaterial)
		{
			Object.Destroy(this.m_BlurDesatMaterial);
		}
		if (this.m_BlendMaterial)
		{
			Object.Destroy(this.m_BlendMaterial);
		}
		if (this.m_VignettingMaterial)
		{
			Object.Destroy(this.m_VignettingMaterial);
		}
		if (this.m_BlendToColorMaterial)
		{
			Object.Destroy(this.m_BlendToColorMaterial);
		}
		if (this.m_DesaturationMaterial)
		{
			Object.Destroy(this.m_DesaturationMaterial);
		}
		if (this.m_DesaturationVignettingMaterial)
		{
			Object.Destroy(this.m_DesaturationVignettingMaterial);
		}
		if (this.m_BlurDesaturationVignettingMaterial)
		{
			Object.Destroy(this.m_BlurDesaturationVignettingMaterial);
		}
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x000A9558 File Offset: 0x000A7758
	protected void OnDestroy()
	{
		CheatMgr cheatMgr = CheatMgr.Get();
		if (cheatMgr != null)
		{
			cheatMgr.UnregisterCheatHandler("wireframe", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_RenderWireframe));
		}
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000A958E File Offset: 0x000A778E
	protected void Awake()
	{
		this.m_Camera = base.GetComponent<Camera>();
	}

	// Token: 0x06002272 RID: 8818 RVA: 0x000A959C File Offset: 0x000A779C
	protected void Start()
	{
		CheatMgr cheatMgr = CheatMgr.Get();
		if (cheatMgr != null)
		{
			cheatMgr.RegisterCheatHandler("wireframe", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_RenderWireframe), null, null, null);
		}
		Camera component = base.gameObject.GetComponent<Camera>();
		component.clearFlags = 2;
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogError("Fullscreen Effects not supported");
			base.enabled = false;
			return;
		}
		if (this.m_BlurShader == null)
		{
			this.m_BlurShader = ShaderUtils.FindShader("Custom/FullScreen/Blur");
		}
		if (!this.m_BlurShader)
		{
			Debug.LogError("Fullscreen Effect Failed to load Shader: Custom/FullScreen/Blur");
			base.enabled = false;
		}
		if (!this.m_BlurShader || !this.blurMaterial.shader.isSupported)
		{
			Debug.LogError("Fullscreen Effect Shader not supported: Custom/FullScreen/Blur");
			base.enabled = false;
			return;
		}
		if (this.m_BlurVignettingShader == null)
		{
			this.m_BlurVignettingShader = ShaderUtils.FindShader("Custom/FullScreen/BlurVignetting");
		}
		if (!this.m_BlurVignettingShader)
		{
			Debug.LogError("Fullscreen Effect Failed to load Shader: Custom/FullScreen/BlurVignetting");
			base.enabled = false;
		}
		if (this.m_BlurDesaturationShader == null)
		{
			this.m_BlurDesaturationShader = ShaderUtils.FindShader("Custom/FullScreen/DesaturationBlur");
		}
		if (!this.m_BlurDesaturationShader)
		{
			Debug.LogError("Fullscreen Effect Failed to load Shader: Custom/FullScreen/DesaturationBlur");
			base.enabled = false;
		}
		if (this.m_BlendShader == null)
		{
			this.m_BlendShader = ShaderUtils.FindShader("Custom/FullScreen/Blend");
		}
		if (!this.m_BlendShader)
		{
			Debug.LogError("Fullscreen Effect Failed to load Shader: Custom/FullScreen/Blend");
			base.enabled = false;
		}
		if (this.m_VignettingShader == null)
		{
			this.m_VignettingShader = ShaderUtils.FindShader("Custom/FullScreen/Vignetting");
		}
		if (!this.m_VignettingShader)
		{
			Debug.LogError("Fullscreen Effect Failed to load Shader: Custom/FullScreen/Vignetting");
			base.enabled = false;
		}
		if (this.m_BlendToColorShader == null)
		{
			this.m_BlendToColorShader = ShaderUtils.FindShader("Custom/FullScreen/BlendToColor");
		}
		if (!this.m_BlendToColorShader)
		{
			Debug.LogError("Fullscreen Effect Failed to load Shader: Custom/FullScreen/BlendToColor");
			base.enabled = false;
		}
		if (this.m_DesaturationShader == null)
		{
			this.m_DesaturationShader = ShaderUtils.FindShader("Custom/FullScreen/Desaturation");
		}
		if (!this.m_DesaturationShader)
		{
			Debug.LogError("Fullscreen Effect Failed to load Shader: Custom/FullScreen/Desaturation");
			base.enabled = false;
		}
		if (this.m_DesaturationVignettingShader == null)
		{
			this.m_DesaturationVignettingShader = ShaderUtils.FindShader("Custom/FullScreen/DesaturationVignetting");
		}
		if (!this.m_DesaturationVignettingShader)
		{
			Debug.LogError("Fullscreen Effect Failed to load Shader: Custom/FullScreen/DesaturationVignetting");
			base.enabled = false;
		}
		if (this.m_BlurDesaturationVignettingShader == null)
		{
			this.m_BlurDesaturationVignettingShader = ShaderUtils.FindShader("Custom/FullScreen/BlurDesaturationVignetting");
		}
		if (!this.m_BlurDesaturationVignettingShader)
		{
			Debug.LogError("Fullscreen Effect Failed to load Shader: Custom/FullScreen/BlurDesaturationVignetting");
			base.enabled = false;
		}
	}

	// Token: 0x06002273 RID: 8819 RVA: 0x000A988D File Offset: 0x000A7A8D
	private void Update()
	{
		this.UpdateUniversalInputManager();
	}

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x06002274 RID: 8820 RVA: 0x000A9895 File Offset: 0x000A7A95
	// (set) Token: 0x06002275 RID: 8821 RVA: 0x000A989D File Offset: 0x000A7A9D
	public bool BlurEnabled
	{
		get
		{
			return this.m_BlurEnabled;
		}
		set
		{
			if (!base.enabled && value)
			{
				base.enabled = true;
			}
			this.m_BlurEnabled = value;
		}
	}

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x06002276 RID: 8822 RVA: 0x000A98BE File Offset: 0x000A7ABE
	// (set) Token: 0x06002277 RID: 8823 RVA: 0x000A98C8 File Offset: 0x000A7AC8
	public float BlurBlend
	{
		get
		{
			return this.m_BlurBlend;
		}
		set
		{
			if (!base.enabled)
			{
				base.enabled = true;
			}
			this.m_BlurEnabled = true;
			this.m_BlurBlend = value;
		}
	}

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x06002278 RID: 8824 RVA: 0x000A98F5 File Offset: 0x000A7AF5
	// (set) Token: 0x06002279 RID: 8825 RVA: 0x000A98FD File Offset: 0x000A7AFD
	public float BlurAmount
	{
		get
		{
			return this.m_BlurAmount;
		}
		set
		{
			if (!base.enabled)
			{
				base.enabled = true;
			}
			this.m_BlurEnabled = true;
			this.m_BlurAmount = value;
			this.m_PreviousBlurAmount = value;
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x0600227A RID: 8826 RVA: 0x000A9926 File Offset: 0x000A7B26
	// (set) Token: 0x0600227B RID: 8827 RVA: 0x000A992E File Offset: 0x000A7B2E
	public float BlurDesaturation
	{
		get
		{
			return this.m_BlurDesaturation;
		}
		set
		{
			if (!base.enabled)
			{
				base.enabled = true;
			}
			this.m_BlurEnabled = true;
			this.m_BlurDesaturation = value;
			this.m_PreviousBlurDesaturation = value;
		}
	}

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x0600227C RID: 8828 RVA: 0x000A9957 File Offset: 0x000A7B57
	// (set) Token: 0x0600227D RID: 8829 RVA: 0x000A995F File Offset: 0x000A7B5F
	public float BlurBrightness
	{
		get
		{
			return this.m_BlurBrightness;
		}
		set
		{
			if (!base.enabled)
			{
				base.enabled = true;
			}
			this.m_BlurEnabled = true;
			this.m_BlurBrightness = value;
			this.m_PreviousBlurBrightness = value;
		}
	}

	// Token: 0x1700034C RID: 844
	// (get) Token: 0x0600227E RID: 8830 RVA: 0x000A9988 File Offset: 0x000A7B88
	// (set) Token: 0x0600227F RID: 8831 RVA: 0x000A9990 File Offset: 0x000A7B90
	public bool VignettingEnable
	{
		get
		{
			return this.m_VignettingEnable;
		}
		set
		{
			if (!base.enabled && value)
			{
				base.enabled = true;
			}
			this.m_VignettingEnable = value;
		}
	}

	// Token: 0x1700034D RID: 845
	// (get) Token: 0x06002280 RID: 8832 RVA: 0x000A99B1 File Offset: 0x000A7BB1
	// (set) Token: 0x06002281 RID: 8833 RVA: 0x000A99BC File Offset: 0x000A7BBC
	public float VignettingIntensity
	{
		get
		{
			return this.m_VignettingIntensity;
		}
		set
		{
			if (!base.enabled)
			{
				base.enabled = true;
			}
			this.m_VignettingEnable = true;
			this.m_VignettingIntensity = value;
		}
	}

	// Token: 0x1700034E RID: 846
	// (get) Token: 0x06002282 RID: 8834 RVA: 0x000A99E9 File Offset: 0x000A7BE9
	// (set) Token: 0x06002283 RID: 8835 RVA: 0x000A99F1 File Offset: 0x000A7BF1
	public bool BlendToColorEnable
	{
		get
		{
			return this.m_BlendToColorEnable;
		}
		set
		{
			if (!base.enabled && value)
			{
				base.enabled = true;
			}
			this.m_BlendToColorEnable = value;
		}
	}

	// Token: 0x1700034F RID: 847
	// (get) Token: 0x06002284 RID: 8836 RVA: 0x000A9A12 File Offset: 0x000A7C12
	// (set) Token: 0x06002285 RID: 8837 RVA: 0x000A9A1C File Offset: 0x000A7C1C
	public Color BlendToColor
	{
		get
		{
			return this.m_BlendToColor;
		}
		set
		{
			if (!base.enabled)
			{
				base.enabled = true;
			}
			this.m_BlendToColorEnable = true;
			this.m_BlendToColor = value;
		}
	}

	// Token: 0x17000350 RID: 848
	// (get) Token: 0x06002286 RID: 8838 RVA: 0x000A9A49 File Offset: 0x000A7C49
	// (set) Token: 0x06002287 RID: 8839 RVA: 0x000A9A54 File Offset: 0x000A7C54
	public float BlendToColorAmount
	{
		get
		{
			return this.m_BlendToColorAmount;
		}
		set
		{
			if (!base.enabled)
			{
				base.enabled = true;
			}
			this.m_BlendToColorEnable = true;
			this.m_BlendToColorAmount = value;
		}
	}

	// Token: 0x17000351 RID: 849
	// (get) Token: 0x06002288 RID: 8840 RVA: 0x000A9A81 File Offset: 0x000A7C81
	// (set) Token: 0x06002289 RID: 8841 RVA: 0x000A9A89 File Offset: 0x000A7C89
	public bool DesaturationEnabled
	{
		get
		{
			return this.m_DesaturationEnabled;
		}
		set
		{
			if (!base.enabled && value)
			{
				base.enabled = true;
			}
			this.m_DesaturationEnabled = value;
		}
	}

	// Token: 0x17000352 RID: 850
	// (get) Token: 0x0600228A RID: 8842 RVA: 0x000A9AAA File Offset: 0x000A7CAA
	// (set) Token: 0x0600228B RID: 8843 RVA: 0x000A9AB4 File Offset: 0x000A7CB4
	public float Desaturation
	{
		get
		{
			return this.m_Desaturation;
		}
		set
		{
			if (!base.enabled)
			{
				base.enabled = true;
			}
			this.m_DesaturationEnabled = true;
			this.m_Desaturation = value;
		}
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x000A9AE4 File Offset: 0x000A7CE4
	private void SetDefaults()
	{
		this.m_BlurEnabled = false;
		this.m_BlurBlend = 1f;
		this.m_BlurAmount = 2f;
		this.m_BlurDesaturation = 0f;
		this.m_BlurBrightness = 1f;
		this.m_VignettingEnable = false;
		this.m_VignettingIntensity = 0f;
		this.m_BlendToColorEnable = false;
		this.m_BlendToColor = Color.white;
		this.m_BlendToColorAmount = 0f;
		this.m_DesaturationEnabled = false;
		this.m_Desaturation = 0f;
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x000A9B68 File Offset: 0x000A7D68
	public void Disable()
	{
		base.enabled = false;
		this.SetDefaults();
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr != null)
		{
			fullScreenFXMgr.WillReset();
		}
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000A9B9C File Offset: 0x000A7D9C
	[ContextMenu("Freeze")]
	public void Freeze()
	{
		Log.Kyle.Print("FullScreenEffects: Freeze()", new object[0]);
		base.enabled = true;
		if (this.m_FrozenState)
		{
			return;
		}
		this.m_BlurEnabled = true;
		this.m_BlurBlend = 1f;
		this.m_BlurAmount = this.m_PreviousBlurAmount * 0.75f;
		this.m_BlurDesaturation = this.m_PreviousBlurDesaturation;
		this.m_BlurBrightness = this.m_PreviousBlurBrightness;
		this.m_CaptureFrozenImage = true;
		int num = this.m_LowQualityFreezeBufferSize;
		int num2 = this.m_LowQualityFreezeBufferSize;
		if (GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.Low)
		{
			num = this.m_LowQualityFreezeBufferSize;
			num2 = this.m_LowQualityFreezeBufferSize;
		}
		else
		{
			num = Screen.currentResolution.width;
			num2 = Screen.currentResolution.height;
		}
		this.m_FrozenScreenTexture = new Texture2D(num, num2, 3, false, true);
		this.m_FrozenScreenTexture.filterMode = 0;
		this.m_FrozenScreenTexture.wrapMode = 1;
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x000A9C88 File Offset: 0x000A7E88
	[ContextMenu("Unfreeze")]
	public void Unfreeze()
	{
		Log.Kyle.Print("FullScreenEffects: Unfreeze()", new object[0]);
		this.m_BlurEnabled = false;
		this.m_BlurBlend = 0f;
		this.m_FrozenState = false;
		Object.Destroy(this.m_FrozenScreenTexture);
		this.m_FrozenScreenTexture = null;
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x000A9CD8 File Offset: 0x000A7ED8
	public bool isActive()
	{
		return base.enabled && (this.m_FrozenState || (this.m_BlurEnabled && this.m_BlurBlend > 0f) || this.m_VignettingEnable || this.m_BlendToColorEnable || this.m_DesaturationEnabled || this.m_WireframeRender);
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x000A9D54 File Offset: 0x000A7F54
	private void Sample(RenderTexture source, RenderTexture dest, float off, Material mat)
	{
		if (dest != null)
		{
			dest.DiscardContents();
		}
		Graphics.BlitMultiTap(source, dest, mat, new Vector2[]
		{
			new Vector2(-off, -off),
			new Vector2(-off, off),
			new Vector2(off, off),
			new Vector2(off, -off)
		});
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x000A9DD4 File Offset: 0x000A7FD4
	private void Blur(RenderTexture source, RenderTexture destination, Material blurMat)
	{
		float num = (float)source.width;
		float num2 = (float)source.height;
		this.CalcTextureSize(source.width, source.height, 512, out num, out num2);
		RenderTexture temporary = RenderTexture.GetTemporary((int)num, (int)num2, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary((int)(num * 0.5f), (int)(num2 * 0.5f), 0);
		temporary.wrapMode = 1;
		temporary2.wrapMode = 1;
		blurMat.SetFloat("_BlurOffset", 1f);
		blurMat.SetFloat("_FirstPass", 1f);
		Graphics.Blit(source, temporary, blurMat);
		blurMat.SetFloat("_BlurOffset", 0.4f);
		blurMat.SetFloat("_FirstPass", 0f);
		Graphics.Blit(temporary, temporary2, blurMat);
		blurMat.SetFloat("_BlurOffset", -0.2f);
		blurMat.SetFloat("_FirstPass", 0f);
		Graphics.Blit(temporary2, destination, blurMat);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
	}

	// Token: 0x06002293 RID: 8851 RVA: 0x000A9EC0 File Offset: 0x000A80C0
	private void CalcTextureSize(int currentWidth, int currentHeight, int resolution, out float sizeX, out float sizeY)
	{
		float num = (float)currentWidth;
		float num2 = (float)currentHeight;
		float num3 = (float)resolution;
		if (num > num2)
		{
			sizeX = num3;
			sizeY = num3 * (num2 / num);
		}
		else
		{
			sizeX = num3 * (num / num2);
			sizeY = num3;
		}
	}

	// Token: 0x06002294 RID: 8852 RVA: 0x000A9EFC File Offset: 0x000A80FC
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (source == null || source.width == 0 || source.height == 0)
		{
			return;
		}
		bool flag = false;
		if (this.m_CaptureFrozenImage && !this.m_FrozenState)
		{
			Log.Kyle.Print("FullScreenEffects: Capture Frozen Image!", new object[0]);
			Material material = this.blurMaterial;
			material.SetFloat("_Brightness", this.m_BlurBrightness);
			if (this.m_BlurDesaturation > 0f && !this.m_VignettingEnable)
			{
				material = this.blurDesatMaterial;
				material.SetFloat("_Desaturation", this.m_BlurDesaturation);
			}
			else if (this.m_VignettingEnable && this.m_BlurDesaturation == 0f)
			{
				material = this.blurVignettingMaterial;
				material.SetFloat("_Amount", this.m_VignettingIntensity);
				material.SetTexture("_MaskTex", this.m_VignettingMask);
			}
			else if (this.m_VignettingEnable && this.m_BlurDesaturation > 0f)
			{
				material = this.BlurDesaturationVignettingMaterial;
				material.SetFloat("_Amount", this.m_VignettingIntensity);
				material.SetTexture("_MaskTex", this.m_VignettingMask);
				material.SetFloat("_Desaturation", this.m_BlurDesaturation);
			}
			int num = this.m_LowQualityFreezeBufferSize;
			int num2 = this.m_LowQualityFreezeBufferSize;
			if (GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.Low)
			{
				num = this.m_LowQualityFreezeBufferSize;
				num2 = this.m_LowQualityFreezeBufferSize;
			}
			else
			{
				num = Screen.currentResolution.width;
				num2 = Screen.currentResolution.height;
			}
			RenderTexture temporary = RenderTexture.GetTemporary(num, num2);
			this.Blur(source, temporary, material);
			RenderTexture.active = temporary;
			this.m_FrozenScreenTexture.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0, false);
			this.m_FrozenScreenTexture.Apply();
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(temporary);
			this.m_CaptureFrozenImage = false;
			this.m_FrozenState = true;
			flag = true;
			this.m_DeactivateFrameCount = 0;
		}
		if (this.m_FrozenState)
		{
			if (this.m_FrozenScreenTexture)
			{
				Material blendMaterial = this.blendMaterial;
				blendMaterial.SetFloat("_Amount", 1f);
				blendMaterial.SetTexture("_BlendTex", this.m_FrozenScreenTexture);
				if (QualitySettings.antiAliasing > 0)
				{
					blendMaterial.SetFloat("_Flip", 1f);
				}
				else
				{
					blendMaterial.SetFloat("_Flip", 0f);
				}
				if (destination != null)
				{
					destination.DiscardContents();
				}
				Graphics.Blit(source, destination, blendMaterial);
				this.m_DeactivateFrameCount = 0;
				return;
			}
			Debug.LogWarning("m_FrozenScreenTexture is null. FullScreenEffect Freeze disabled");
			this.m_FrozenState = false;
		}
		if (this.m_BlurEnabled && this.m_BlurBlend > 0f)
		{
			Material material2 = this.blurMaterial;
			material2.SetFloat("_Brightness", this.m_BlurBrightness);
			if (this.m_BlurDesaturation > 0f && !this.m_VignettingEnable)
			{
				material2 = this.blurDesatMaterial;
				material2.SetFloat("_Desaturation", this.m_BlurDesaturation);
			}
			else if (this.m_VignettingEnable && this.m_BlurDesaturation == 0f)
			{
				material2 = this.blurVignettingMaterial;
				material2.SetFloat("_Amount", this.m_VignettingIntensity);
				material2.SetTexture("_MaskTex", this.m_VignettingMask);
			}
			else if (this.m_VignettingEnable && this.m_BlurDesaturation > 0f)
			{
				material2 = this.BlurDesaturationVignettingMaterial;
				material2.SetFloat("_Amount", this.m_VignettingIntensity);
				material2.SetTexture("_MaskTex", this.m_VignettingMask);
				material2.SetFloat("_Desaturation", this.m_BlurDesaturation);
			}
			if (this.m_BlurBlend >= 1f)
			{
				this.Blur(source, destination, material2);
				flag = true;
			}
			else
			{
				RenderTexture temporary2 = RenderTexture.GetTemporary(512, 512, 0);
				this.Blur(source, temporary2, material2);
				Material blendMaterial2 = this.blendMaterial;
				blendMaterial2.SetFloat("_Amount", this.m_BlurBlend);
				blendMaterial2.SetTexture("_BlendTex", temporary2);
				Graphics.Blit(source, destination, blendMaterial2);
				RenderTexture.ReleaseTemporary(temporary2);
				flag = true;
			}
		}
		if (this.m_DesaturationEnabled && !flag)
		{
			Material material3 = this.DesaturationMaterial;
			if (this.m_VignettingEnable)
			{
				material3 = this.DesaturationVignettingMaterial;
				material3.SetFloat("_Amount", this.m_VignettingIntensity);
				material3.SetTexture("_MaskTex", this.m_VignettingMask);
			}
			material3.SetFloat("_Desaturation", this.m_Desaturation);
			Graphics.Blit(source, destination, material3);
			flag = true;
		}
		if (this.m_VignettingEnable && !flag)
		{
			Material vignettingMaterial = this.VignettingMaterial;
			vignettingMaterial.SetFloat("_Amount", this.m_VignettingIntensity);
			vignettingMaterial.SetTexture("_MaskTex", this.m_VignettingMask);
			Graphics.Blit(source, destination, vignettingMaterial);
			flag = true;
		}
		if (this.m_BlendToColorEnable && !flag)
		{
			Material blendToColorMaterial = this.BlendToColorMaterial;
			blendToColorMaterial.SetFloat("_Amount", this.m_BlendToColorAmount);
			blendToColorMaterial.SetColor("_Color", this.m_BlendToColor);
			Graphics.Blit(source, destination, blendToColorMaterial);
			flag = true;
		}
		if (!flag)
		{
			Material blendMaterial3 = this.blendMaterial;
			blendMaterial3.SetFloat("_Amount", 0f);
			blendMaterial3.SetTexture("_BlendTex", null);
			Graphics.Blit(source, destination, blendMaterial3);
			if (this.m_DeactivateFrameCount > 2)
			{
				this.m_DeactivateFrameCount = 0;
				this.Disable();
			}
			else
			{
				this.m_DeactivateFrameCount++;
			}
			return;
		}
	}

	// Token: 0x06002295 RID: 8853 RVA: 0x000AA496 File Offset: 0x000A8696
	private void OnPreRender()
	{
		if (this.m_WireframeRender)
		{
			GL.wireframe = true;
		}
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x000AA4A9 File Offset: 0x000A86A9
	private void OnPostRender()
	{
		GL.wireframe = false;
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x000AA4B1 File Offset: 0x000A86B1
	private bool OnProcessCheat_RenderWireframe(string func, string[] args, string rawArgs)
	{
		if (this.m_WireframeRender)
		{
			this.m_WireframeRender = false;
			return true;
		}
		this.m_WireframeRender = true;
		base.enabled = true;
		return true;
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x000AA4D8 File Offset: 0x000A86D8
	private void UpdateUniversalInputManager()
	{
		if (this.m_UniversalInputManager == null)
		{
			this.m_UniversalInputManager = UniversalInputManager.Get();
		}
		if (this.m_UniversalInputManager == null)
		{
			return;
		}
		this.m_UniversalInputManager.SetFullScreenEffectsCamera(this.m_Camera, this.isActive());
	}

	// Token: 0x040013C8 RID: 5064
	private const int NO_WORK_FRAMES_BEFORE_DEACTIVATE = 2;

	// Token: 0x040013C9 RID: 5065
	private const string BLUR_SHADER_NAME = "Custom/FullScreen/Blur";

	// Token: 0x040013CA RID: 5066
	private const string BLUR_VIGNETTING_SHADER_NAME = "Custom/FullScreen/BlurVignetting";

	// Token: 0x040013CB RID: 5067
	private const string BLUR_DESATURATION_SHADER_NAME = "Custom/FullScreen/DesaturationBlur";

	// Token: 0x040013CC RID: 5068
	private const string BLEND_SHADER_NAME = "Custom/FullScreen/Blend";

	// Token: 0x040013CD RID: 5069
	private const string VIGNETTING_SHADER_NAME = "Custom/FullScreen/Vignetting";

	// Token: 0x040013CE RID: 5070
	private const string BLEND_TO_COLOR_SHADER_NAME = "Custom/FullScreen/BlendToColor";

	// Token: 0x040013CF RID: 5071
	private const string DESATURATION_SHADER_NAME = "Custom/FullScreen/Desaturation";

	// Token: 0x040013D0 RID: 5072
	private const string DESATURATION_VIGNETTING_SHADER_NAME = "Custom/FullScreen/DesaturationVignetting";

	// Token: 0x040013D1 RID: 5073
	private const string BLUR_DESATURATION_VIGNETTING_SHADER_NAME = "Custom/FullScreen/BlurDesaturationVignetting";

	// Token: 0x040013D2 RID: 5074
	private const int BLUR_BUFFER_SIZE = 512;

	// Token: 0x040013D3 RID: 5075
	private const float BLUR_SECOND_PASS_REDUCTION = 0.5f;

	// Token: 0x040013D4 RID: 5076
	private const float BLUR_PASS_1_OFFSET = 1f;

	// Token: 0x040013D5 RID: 5077
	private const float BLUR_PASS_2_OFFSET = 0.4f;

	// Token: 0x040013D6 RID: 5078
	private const float BLUR_PASS_3_OFFSET = -0.2f;

	// Token: 0x040013D7 RID: 5079
	public Texture2D m_VignettingMask;

	// Token: 0x040013D8 RID: 5080
	private int m_LowQualityFreezeBufferSize = 512;

	// Token: 0x040013D9 RID: 5081
	private bool m_BlurEnabled;

	// Token: 0x040013DA RID: 5082
	public float m_BlurBlend = 1f;

	// Token: 0x040013DB RID: 5083
	private float m_BlurAmount = 2f;

	// Token: 0x040013DC RID: 5084
	private float m_BlurDesaturation;

	// Token: 0x040013DD RID: 5085
	private float m_BlurBrightness = 1f;

	// Token: 0x040013DE RID: 5086
	private bool m_VignettingEnable;

	// Token: 0x040013DF RID: 5087
	private float m_PreviousBlurAmount = 1f;

	// Token: 0x040013E0 RID: 5088
	private float m_PreviousBlurDesaturation;

	// Token: 0x040013E1 RID: 5089
	private float m_PreviousBlurBrightness = 1f;

	// Token: 0x040013E2 RID: 5090
	private float m_VignettingIntensity;

	// Token: 0x040013E3 RID: 5091
	private bool m_BlendToColorEnable;

	// Token: 0x040013E4 RID: 5092
	private Color m_BlendToColor = Color.white;

	// Token: 0x040013E5 RID: 5093
	private float m_BlendToColorAmount;

	// Token: 0x040013E6 RID: 5094
	private bool m_DesaturationEnabled;

	// Token: 0x040013E7 RID: 5095
	private float m_Desaturation;

	// Token: 0x040013E8 RID: 5096
	private bool m_WireframeRender;

	// Token: 0x040013E9 RID: 5097
	private int m_DeactivateFrameCount;

	// Token: 0x040013EA RID: 5098
	private Shader m_BlurShader;

	// Token: 0x040013EB RID: 5099
	private Shader m_BlurVignettingShader;

	// Token: 0x040013EC RID: 5100
	private Shader m_BlurDesaturationShader;

	// Token: 0x040013ED RID: 5101
	private Shader m_BlendShader;

	// Token: 0x040013EE RID: 5102
	private Shader m_VignettingShader;

	// Token: 0x040013EF RID: 5103
	private Shader m_BlendToColorShader;

	// Token: 0x040013F0 RID: 5104
	private Shader m_DesaturationShader;

	// Token: 0x040013F1 RID: 5105
	private Shader m_DesaturationVignettingShader;

	// Token: 0x040013F2 RID: 5106
	private Shader m_BlurDesaturationVignettingShader;

	// Token: 0x040013F3 RID: 5107
	private bool m_FrozenState;

	// Token: 0x040013F4 RID: 5108
	private bool m_CaptureFrozenImage;

	// Token: 0x040013F5 RID: 5109
	private Texture2D m_FrozenScreenTexture;

	// Token: 0x040013F6 RID: 5110
	private UniversalInputManager m_UniversalInputManager;

	// Token: 0x040013F7 RID: 5111
	private Camera m_Camera;

	// Token: 0x040013F8 RID: 5112
	private Material m_BlurMaterial;

	// Token: 0x040013F9 RID: 5113
	private Material m_BlurVignettingMaterial;

	// Token: 0x040013FA RID: 5114
	private Material m_BlurDesatMaterial;

	// Token: 0x040013FB RID: 5115
	private Material m_BlendMaterial;

	// Token: 0x040013FC RID: 5116
	private Material m_VignettingMaterial;

	// Token: 0x040013FD RID: 5117
	private Material m_BlendToColorMaterial;

	// Token: 0x040013FE RID: 5118
	private Material m_DesaturationMaterial;

	// Token: 0x040013FF RID: 5119
	private Material m_DesaturationVignettingMaterial;

	// Token: 0x04001400 RID: 5120
	private Material m_BlurDesaturationVignettingMaterial;
}
