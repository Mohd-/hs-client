using System;
using UnityEngine;

// Token: 0x0200079B RID: 1947
public class FullScreenAntialiasing : MonoBehaviour
{
	// Token: 0x17000569 RID: 1385
	// (get) Token: 0x06004CE9 RID: 19689 RVA: 0x0016DCF6 File Offset: 0x0016BEF6
	protected Material FXAA_Material
	{
		get
		{
			if (this.m_FXAA_Material == null)
			{
				this.m_FXAA_Material = new Material(this.m_FXAA_Shader);
				SceneUtils.SetHideFlags(this.m_FXAA_Material, 52);
			}
			return this.m_FXAA_Material;
		}
	}

	// Token: 0x06004CEA RID: 19690 RVA: 0x0016DD30 File Offset: 0x0016BF30
	private void Awake()
	{
		Camera component = base.gameObject.GetComponent<Camera>();
		component.enabled = true;
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06004CEB RID: 19691 RVA: 0x0016DD6C File Offset: 0x0016BF6C
	private void Start()
	{
		if (this.m_FXAA_Shader == null || this.FXAA_Material == null)
		{
			base.enabled = false;
		}
		if (!this.m_FXAA_Shader.isSupported)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06004CEC RID: 19692 RVA: 0x0016DDB9 File Offset: 0x0016BFB9
	private void Update()
	{
	}

	// Token: 0x06004CED RID: 19693 RVA: 0x0016DDBB File Offset: 0x0016BFBB
	private void OnDisable()
	{
		if (this.m_FXAA_Material != null)
		{
			Object.Destroy(this.m_FXAA_Material);
		}
	}

	// Token: 0x06004CEE RID: 19694 RVA: 0x0016DDD9 File Offset: 0x0016BFD9
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, this.FXAA_Material);
	}

	// Token: 0x040033E1 RID: 13281
	public Shader m_FXAA_Shader;

	// Token: 0x040033E2 RID: 13282
	private Material m_FXAA_Material;
}
