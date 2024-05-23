using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200034F RID: 847
public class HighlightRender : MonoBehaviour
{
	// Token: 0x1700038B RID: 907
	// (get) Token: 0x06002BE6 RID: 11238 RVA: 0x000DA428 File Offset: 0x000D8628
	protected Material HighlightMaterial
	{
		get
		{
			if (this.m_Material == null)
			{
				this.m_Material = new Material(this.m_HighlightShader);
				SceneUtils.SetHideFlags(this.m_Material, 52);
			}
			return this.m_Material;
		}
	}

	// Token: 0x1700038C RID: 908
	// (get) Token: 0x06002BE7 RID: 11239 RVA: 0x000DA45F File Offset: 0x000D865F
	protected Material MultiSampleMaterial
	{
		get
		{
			if (this.m_MultiSampleMaterial == null)
			{
				this.m_MultiSampleMaterial = new Material(this.m_MultiSampleShader);
				SceneUtils.SetHideFlags(this.m_MultiSampleMaterial, 52);
			}
			return this.m_MultiSampleMaterial;
		}
	}

	// Token: 0x1700038D RID: 909
	// (get) Token: 0x06002BE8 RID: 11240 RVA: 0x000DA496 File Offset: 0x000D8696
	protected Material MultiSampleBlendMaterial
	{
		get
		{
			if (this.m_MultiSampleBlendMaterial == null)
			{
				this.m_MultiSampleBlendMaterial = new Material(this.m_MultiSampleBlendShader);
				SceneUtils.SetHideFlags(this.m_MultiSampleBlendMaterial, 52);
			}
			return this.m_MultiSampleBlendMaterial;
		}
	}

	// Token: 0x1700038E RID: 910
	// (get) Token: 0x06002BE9 RID: 11241 RVA: 0x000DA4CD File Offset: 0x000D86CD
	protected Material BlendMaterial
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

	// Token: 0x06002BEA RID: 11242 RVA: 0x000DA504 File Offset: 0x000D8704
	protected void OnApplicationFocus(bool state)
	{
	}

	// Token: 0x06002BEB RID: 11243 RVA: 0x000DA508 File Offset: 0x000D8708
	protected void OnDisable()
	{
		if (this.m_Material)
		{
			Object.Destroy(this.m_Material);
		}
		if (this.m_MultiSampleMaterial)
		{
			Object.Destroy(this.m_MultiSampleMaterial);
		}
		if (this.m_BlendMaterial)
		{
			Object.Destroy(this.m_BlendMaterial);
		}
		if (this.m_MultiSampleBlendMaterial)
		{
			Object.Destroy(this.m_MultiSampleBlendMaterial);
		}
		if (this.m_VisibilityStates != null)
		{
			this.m_VisibilityStates.Clear();
		}
		if (this.m_CameraTexture != null)
		{
			if (RenderTexture.active == this.m_CameraTexture)
			{
				RenderTexture.active = null;
			}
			this.m_CameraTexture.Release();
			this.m_CameraTexture = null;
		}
		this.m_Initialized = false;
	}

	// Token: 0x06002BEC RID: 11244 RVA: 0x000DA5DC File Offset: 0x000D87DC
	protected void Initialize()
	{
		if (this.m_Initialized)
		{
			return;
		}
		this.m_Initialized = true;
		if (this.m_HighlightShader == null)
		{
			this.m_HighlightShader = ShaderUtils.FindShader(this.HIGHLIGHT_SHADER_NAME);
		}
		if (!this.m_HighlightShader)
		{
			Debug.LogError("Failed to load Highlight Shader: " + this.HIGHLIGHT_SHADER_NAME);
			base.enabled = false;
		}
		base.GetComponent<Renderer>().material.shader = this.m_HighlightShader;
		if (this.m_MultiSampleShader == null)
		{
			this.m_MultiSampleShader = ShaderUtils.FindShader(this.MULTISAMPLE_SHADER_NAME);
		}
		if (!this.m_MultiSampleShader)
		{
			Debug.LogError("Failed to load Highlight Shader: " + this.MULTISAMPLE_SHADER_NAME);
			base.enabled = false;
		}
		if (this.m_MultiSampleBlendShader == null)
		{
			this.m_MultiSampleBlendShader = ShaderUtils.FindShader(this.MULTISAMPLE_BLEND_SHADER_NAME);
		}
		if (!this.m_MultiSampleBlendShader)
		{
			Debug.LogError("Failed to load Highlight Shader: " + this.MULTISAMPLE_BLEND_SHADER_NAME);
			base.enabled = false;
		}
		if (this.m_BlendShader == null)
		{
			this.m_BlendShader = ShaderUtils.FindShader(this.BLEND_SHADER_NAME);
		}
		if (!this.m_BlendShader)
		{
			Debug.LogError("Failed to load Highlight Shader: " + this.BLEND_SHADER_NAME);
			base.enabled = false;
		}
		if (this.m_RootTransform == null)
		{
			Transform parent = base.transform.parent;
			Transform parent2 = parent.parent;
			if (parent2.GetComponent<ActorStateMgr>())
			{
				this.m_RootTransform = parent2.parent;
			}
			else
			{
				this.m_RootTransform = parent2;
			}
			if (this.m_RootTransform == null)
			{
				Debug.LogError("m_RootTransform is null. Highlighting disabled!");
				base.enabled = false;
			}
		}
		this.m_VisibilityStates = new Map<Renderer, bool>();
		HighlightSilhouetteInclude[] componentsInChildren = this.m_RootTransform.GetComponentsInChildren<HighlightSilhouetteInclude>();
		if (componentsInChildren != null)
		{
			foreach (HighlightSilhouetteInclude highlightSilhouetteInclude in componentsInChildren)
			{
				Renderer component = highlightSilhouetteInclude.gameObject.GetComponent<Renderer>();
				if (!(component == null))
				{
					this.m_VisibilityStates.Add(component, false);
				}
			}
		}
		this.m_UnlitColorShader = ShaderUtils.FindShader(this.UNLIT_COLOR_SHADER_NAME);
		if (!this.m_UnlitColorShader)
		{
			Debug.LogError("Failed to load Highlight Rendering Shader: " + this.UNLIT_COLOR_SHADER_NAME);
		}
		this.m_UnlitGreyShader = ShaderUtils.FindShader(this.UNLIT_GREY_SHADER_NAME);
		if (!this.m_UnlitGreyShader)
		{
			Debug.LogError("Failed to load Highlight Rendering Shader: " + this.UNLIT_GREY_SHADER_NAME);
		}
		this.m_UnlitLightGreyShader = ShaderUtils.FindShader(this.UNLIT_LIGHTGREY_SHADER_NAME);
		if (!this.m_UnlitLightGreyShader)
		{
			Debug.LogError("Failed to load Highlight Rendering Shader: " + this.UNLIT_LIGHTGREY_SHADER_NAME);
		}
		this.m_UnlitDarkGreyShader = ShaderUtils.FindShader(this.UNLIT_DARKGREY_SHADER_NAME);
		if (!this.m_UnlitDarkGreyShader)
		{
			Debug.LogError("Failed to load Highlight Rendering Shader: " + this.UNLIT_DARKGREY_SHADER_NAME);
		}
		this.m_UnlitBlackShader = ShaderUtils.FindShader(this.UNLIT_BLACK_SHADER_NAME);
		if (!this.m_UnlitBlackShader)
		{
			Debug.LogError("Failed to load Highlight Rendering Shader: " + this.UNLIT_BLACK_SHADER_NAME);
		}
		this.m_UnlitWhiteShader = ShaderUtils.FindShader(this.UNLIT_WHITE_SHADER_NAME);
		if (!this.m_UnlitWhiteShader)
		{
			Debug.LogError("Failed to load Highlight Rendering Shader: " + this.UNLIT_WHITE_SHADER_NAME);
		}
	}

	// Token: 0x06002BED RID: 11245 RVA: 0x000DA968 File Offset: 0x000D8B68
	protected void Update()
	{
		if (this.m_CameraTexture && this.m_Initialized && !this.m_CameraTexture.IsCreated())
		{
			this.CreateSilhouetteTexture();
		}
	}

	// Token: 0x06002BEE RID: 11246 RVA: 0x000DA99C File Offset: 0x000D8B9C
	[ContextMenu("Export Silhouette Texture")]
	public void ExportSilhouetteTexture()
	{
		RenderTexture.active = this.m_CameraTexture;
		Texture2D texture2D = new Texture2D(this.m_RenderSizeX, this.m_RenderSizeY, 3, false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)this.m_RenderSizeX, (float)this.m_RenderSizeY), 0, 0, false);
		texture2D.Apply();
		string text = Application.dataPath + "/SilhouetteTexture.png";
		File.WriteAllBytes(text, texture2D.EncodeToPNG());
		RenderTexture.active = null;
		Debug.Log(string.Format("Silhouette Texture Created: {0}", text));
	}

	// Token: 0x06002BEF RID: 11247 RVA: 0x000DAA26 File Offset: 0x000D8C26
	public void CreateSilhouetteTexture()
	{
		this.CreateSilhouetteTexture(false);
	}

	// Token: 0x06002BF0 RID: 11248 RVA: 0x000DAA30 File Offset: 0x000D8C30
	public void CreateSilhouetteTexture(bool force)
	{
		this.Initialize();
		if (!this.VisibilityStatesChanged() && !force)
		{
			return;
		}
		this.SetupRenderObjects();
		if (this.m_RenderPlane == null)
		{
			return;
		}
		if (this.m_RenderSizeX < 1 || this.m_RenderSizeY < 1)
		{
			return;
		}
		bool enabled = base.GetComponent<Renderer>().enabled;
		base.GetComponent<Renderer>().enabled = false;
		RenderTexture temporary = RenderTexture.GetTemporary((int)((float)this.m_RenderSizeX * 0.3f), (int)((float)this.m_RenderSizeY * 0.3f), 24);
		RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)this.m_RenderSizeX * 0.3f), (int)((float)this.m_RenderSizeY * 0.3f), 24);
		RenderTexture temporary3 = RenderTexture.GetTemporary((int)((float)this.m_RenderSizeX * 0.3f), (int)((float)this.m_RenderSizeY * 0.3f), 24);
		RenderTexture temporary4 = RenderTexture.GetTemporary((int)((float)this.m_RenderSizeX * 0.5f), (int)((float)this.m_RenderSizeY * 0.5f), 24);
		RenderTexture temporary5 = RenderTexture.GetTemporary((int)((float)this.m_RenderSizeX * 0.5f), (int)((float)this.m_RenderSizeY * 0.5f), 24);
		RenderTexture temporary6 = RenderTexture.GetTemporary(this.m_RenderSizeX, this.m_RenderSizeY, 24);
		RenderTexture temporary7 = RenderTexture.GetTemporary(this.m_RenderSizeX, this.m_RenderSizeY, 24);
		RenderTexture temporary8 = RenderTexture.GetTemporary(this.m_RenderSizeX, this.m_RenderSizeY, 24);
		RenderTexture temporary9 = RenderTexture.GetTemporary((int)((float)this.m_RenderSizeX * 0.92f), (int)((float)this.m_RenderSizeY * 0.92f), 24);
		this.m_CameraTexture.DiscardContents();
		this.m_Camera.clearFlags = 2;
		this.m_Camera.depth = Camera.main.depth - 1f;
		this.m_Camera.clearFlags = 2;
		this.m_Camera.orthographicSize = this.m_CameraOrthoSize + 0.1f * this.m_SilouetteClipSize;
		this.m_Camera.targetTexture = temporary9;
		this.m_Camera.RenderWithShader(this.m_UnlitWhiteShader, "Highlight");
		this.m_Camera.depth = Camera.main.depth - 5f;
		this.m_Camera.orthographicSize = this.m_CameraOrthoSize - 0.2f * this.m_SilouetteRenderSize;
		this.m_Camera.targetTexture = temporary;
		this.m_Camera.RenderWithShader(this.m_UnlitDarkGreyShader, "Highlight");
		this.m_Camera.depth = Camera.main.depth - 4f;
		this.m_Camera.orthographicSize = this.m_CameraOrthoSize - 0.25f * this.m_SilouetteRenderSize;
		this.m_Camera.targetTexture = temporary2;
		this.m_Camera.RenderWithShader(this.m_UnlitGreyShader, "Highlight");
		this.SampleBlend(temporary, temporary2, temporary3, 1.25f);
		this.m_Camera.depth = Camera.main.depth - 3f;
		this.m_Camera.orthographicSize = this.m_CameraOrthoSize - 0.01f * this.m_SilouetteRenderSize;
		this.m_Camera.targetTexture = temporary4;
		this.m_Camera.RenderWithShader(this.m_UnlitLightGreyShader, "Highlight");
		this.SampleBlend(temporary3, temporary4, temporary5, 1.25f);
		this.m_Camera.depth = Camera.main.depth - 2f;
		this.m_Camera.orthographicSize = this.m_CameraOrthoSize - -0.05f * this.m_SilouetteRenderSize;
		this.m_Camera.targetTexture = temporary6;
		this.m_Camera.RenderWithShader(this.m_UnlitWhiteShader, "Highlight");
		this.SampleBlend(temporary5, temporary6, temporary7, 1f);
		this.Sample(temporary7, temporary8, 1.5f);
		this.BlendMaterial.SetTexture("_BlendTex", temporary9);
		float num = 0.8f;
		Graphics.BlitMultiTap(temporary8, this.m_CameraTexture, this.BlendMaterial, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
		RenderTexture.ReleaseTemporary(temporary4);
		RenderTexture.ReleaseTemporary(temporary5);
		RenderTexture.ReleaseTemporary(temporary6);
		RenderTexture.ReleaseTemporary(temporary7);
		RenderTexture.ReleaseTemporary(temporary8);
		RenderTexture.ReleaseTemporary(temporary9);
		this.m_Camera.orthographicSize = this.m_CameraOrthoSize;
		base.GetComponent<Renderer>().enabled = enabled;
		this.RestoreRenderObjects();
	}

	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06002BF1 RID: 11249 RVA: 0x000DAECC File Offset: 0x000D90CC
	public RenderTexture SilhouetteTexture
	{
		get
		{
			return this.m_CameraTexture;
		}
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x000DAED4 File Offset: 0x000D90D4
	public bool isTextureCreated()
	{
		return this.m_CameraTexture && this.m_CameraTexture.IsCreated();
	}

	// Token: 0x06002BF3 RID: 11251 RVA: 0x000DAEF4 File Offset: 0x000D90F4
	private void SetupRenderObjects()
	{
		if (this.m_RootTransform == null)
		{
			this.m_RenderPlane = null;
			return;
		}
		HighlightRender.s_offset -= 10f;
		if (HighlightRender.s_offset < -9000f)
		{
			HighlightRender.s_offset = -2000f;
		}
		this.m_OrgPosition = this.m_RootTransform.position;
		this.m_OrgRotation = this.m_RootTransform.rotation;
		this.m_OrgScale = this.m_RootTransform.localScale;
		Vector3 position = Vector3.left * HighlightRender.s_offset;
		this.m_RootTransform.position = position;
		this.SetWorldScale(this.m_RootTransform, Vector3.one);
		this.m_RootTransform.rotation = Quaternion.identity;
		Bounds bounds = base.GetComponent<Renderer>().bounds;
		float x = bounds.size.x;
		float num = bounds.size.z;
		if (num < bounds.size.y)
		{
			num = bounds.size.y;
		}
		if (x > num)
		{
			this.m_RenderSizeX = 200;
			this.m_RenderSizeY = (int)(200f * (num / x));
		}
		else
		{
			this.m_RenderSizeX = (int)(200f * (x / num));
			this.m_RenderSizeY = 200;
		}
		this.m_CameraOrthoSize = num * 0.5f;
		if (this.m_CameraTexture == null)
		{
			if (this.m_RenderSizeX < 1 || this.m_RenderSizeY < 1)
			{
				this.m_RenderSizeX = 200;
				this.m_RenderSizeY = 200;
			}
			this.m_CameraTexture = new RenderTexture(this.m_RenderSizeX, this.m_RenderSizeY, 24);
			this.m_CameraTexture.format = 0;
		}
		HighlightState componentInChildren = this.m_RootTransform.GetComponentInChildren<HighlightState>();
		if (componentInChildren == null)
		{
			Debug.LogError("Can not find Highlight(HighlightState component) object for selection highlighting.");
			this.m_RenderPlane = null;
			return;
		}
		componentInChildren.transform.localPosition = Vector3.zero;
		HighlightRender componentInChildren2 = this.m_RootTransform.GetComponentInChildren<HighlightRender>();
		if (componentInChildren2 == null)
		{
			Debug.LogError("Can not find render plane object(HighlightRender component) for selection highlighting.");
			this.m_RenderPlane = null;
			return;
		}
		this.m_RenderPlane = componentInChildren2.gameObject;
		this.m_RenderScale = HighlightRender.GetWorldScale(this.m_RenderPlane.transform).x;
		this.CreateCamera(componentInChildren2.transform);
	}

	// Token: 0x06002BF4 RID: 11252 RVA: 0x000DB15D File Offset: 0x000D935D
	private void RestoreRenderObjects()
	{
		this.m_RootTransform.position = this.m_OrgPosition;
		this.m_RootTransform.rotation = this.m_OrgRotation;
		this.m_RootTransform.localScale = this.m_OrgScale;
		this.m_RenderPlane = null;
	}

	// Token: 0x06002BF5 RID: 11253 RVA: 0x000DB19C File Offset: 0x000D939C
	private bool VisibilityStatesChanged()
	{
		bool result = false;
		HighlightSilhouetteInclude[] componentsInChildren = this.m_RootTransform.GetComponentsInChildren<HighlightSilhouetteInclude>();
		List<Renderer> list = new List<Renderer>();
		foreach (HighlightSilhouetteInclude highlightSilhouetteInclude in componentsInChildren)
		{
			Renderer component = highlightSilhouetteInclude.gameObject.GetComponent<Renderer>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		Map<Renderer, bool> map = new Map<Renderer, bool>();
		foreach (Renderer renderer in list)
		{
			bool flag = renderer.enabled && renderer.gameObject.activeInHierarchy;
			if (!this.m_VisibilityStates.ContainsKey(renderer))
			{
				map.Add(renderer, flag);
				if (flag)
				{
					result = true;
				}
			}
			else
			{
				if (this.m_VisibilityStates[renderer] != flag)
				{
					result = true;
				}
				map.Add(renderer, flag);
			}
		}
		return result;
	}

	// Token: 0x06002BF6 RID: 11254 RVA: 0x000DB2B8 File Offset: 0x000D94B8
	private List<GameObject> GetExcludedObjects()
	{
		List<GameObject> list = new List<GameObject>();
		HighlightExclude[] componentsInChildren = this.m_RootTransform.GetComponentsInChildren<HighlightExclude>();
		if (componentsInChildren == null)
		{
			return null;
		}
		foreach (HighlightExclude highlightExclude in componentsInChildren)
		{
			Transform[] componentsInChildren2 = highlightExclude.GetComponentsInChildren<Transform>();
			if (componentsInChildren2 != null)
			{
				foreach (Transform transform in componentsInChildren2)
				{
					list.Add(transform.gameObject);
				}
			}
		}
		list.Add(base.gameObject);
		list.Add(base.transform.parent.gameObject);
		return list;
	}

	// Token: 0x06002BF7 RID: 11255 RVA: 0x000DB364 File Offset: 0x000D9564
	private bool isHighlighExclude(Transform objXform)
	{
		if (objXform == null)
		{
			return true;
		}
		HighlightExclude component = objXform.GetComponent<HighlightExclude>();
		if (component && component.enabled)
		{
			return true;
		}
		Transform parent = objXform.transform.parent;
		if (parent != null)
		{
			int num = 0;
			while (parent != this.m_RootTransform || parent != null)
			{
				if (parent == null || num > 25)
				{
					break;
				}
				HighlightExclude component2 = parent.GetComponent<HighlightExclude>();
				if (component2 != null && component2.ExcludeChildren)
				{
					return true;
				}
				parent = parent.parent;
				num++;
			}
		}
		return false;
	}

	// Token: 0x06002BF8 RID: 11256 RVA: 0x000DB420 File Offset: 0x000D9620
	private void CreateCamera(Transform renderPlane)
	{
		if (this.m_Camera != null)
		{
			return;
		}
		GameObject gameObject = new GameObject();
		this.m_Camera = gameObject.AddComponent<Camera>();
		gameObject.name = renderPlane.name + "_SilhouetteCamera";
		SceneUtils.SetHideFlags(gameObject, 61);
		this.m_Camera.orthographic = true;
		this.m_Camera.orthographicSize = this.m_CameraOrthoSize;
		this.m_Camera.transform.position = renderPlane.position;
		this.m_Camera.transform.rotation = renderPlane.rotation;
		this.m_Camera.transform.Rotate(90f, 180f, 0f);
		this.m_Camera.transform.parent = renderPlane;
		this.m_Camera.nearClipPlane = -this.m_RenderScale + 1f;
		this.m_Camera.farClipPlane = this.m_RenderScale + 1f;
		this.m_Camera.depth = Camera.main.depth - 5f;
		this.m_Camera.backgroundColor = Color.black;
		this.m_Camera.clearFlags = 2;
		this.m_Camera.depthTextureMode = 0;
		this.m_Camera.renderingPath = 0;
		this.m_Camera.SetReplacementShader(this.m_UnlitColorShader, string.Empty);
		this.m_Camera.targetTexture = null;
		this.m_Camera.enabled = false;
	}

	// Token: 0x06002BF9 RID: 11257 RVA: 0x000DB594 File Offset: 0x000D9794
	private void Sample(RenderTexture source, RenderTexture dest, float off)
	{
		if (source == null || dest == null)
		{
			return;
		}
		dest.DiscardContents();
		Graphics.BlitMultiTap(source, dest, this.MultiSampleMaterial, new Vector2[]
		{
			new Vector2(-off, -off),
			new Vector2(-off, off),
			new Vector2(off, off),
			new Vector2(off, -off)
		});
	}

	// Token: 0x06002BFA RID: 11258 RVA: 0x000DB624 File Offset: 0x000D9824
	private void SampleBlend(RenderTexture source, RenderTexture blend, RenderTexture dest, float off)
	{
		if (source == null || dest == null || blend == null)
		{
			return;
		}
		this.MultiSampleBlendMaterial.SetTexture("_BlendTex", blend);
		dest.DiscardContents();
		Graphics.BlitMultiTap(source, dest, this.MultiSampleBlendMaterial, new Vector2[]
		{
			new Vector2(-off, -off),
			new Vector2(-off, off),
			new Vector2(off, off),
			new Vector2(off, -off)
		});
	}

	// Token: 0x06002BFB RID: 11259 RVA: 0x000DB6D8 File Offset: 0x000D98D8
	public static Vector3 GetWorldScale(Transform transform)
	{
		Vector3 vector = transform.localScale;
		Transform parent = transform.parent;
		while (parent != null)
		{
			vector = Vector3.Scale(vector, parent.localScale);
			parent = parent.parent;
		}
		return vector;
	}

	// Token: 0x06002BFC RID: 11260 RVA: 0x000DB71C File Offset: 0x000D991C
	public void SetWorldScale(Transform xform, Vector3 scale)
	{
		GameObject gameObject = new GameObject();
		Transform transform = gameObject.transform;
		transform.parent = null;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		Transform parent = xform.parent;
		xform.parent = transform;
		xform.localScale = scale;
		xform.parent = parent;
		Object.Destroy(gameObject);
	}

	// Token: 0x04001ACF RID: 6863
	private const float RENDER_SIZE1 = 0.3f;

	// Token: 0x04001AD0 RID: 6864
	private const float RENDER_SIZE2 = 0.3f;

	// Token: 0x04001AD1 RID: 6865
	private const float RENDER_SIZE3 = 0.5f;

	// Token: 0x04001AD2 RID: 6866
	private const float RENDER_SIZE4 = 0.92f;

	// Token: 0x04001AD3 RID: 6867
	private const float ORTHO_SIZE1 = 0.2f;

	// Token: 0x04001AD4 RID: 6868
	private const float ORTHO_SIZE2 = 0.25f;

	// Token: 0x04001AD5 RID: 6869
	private const float ORTHO_SIZE3 = 0.01f;

	// Token: 0x04001AD6 RID: 6870
	private const float ORTHO_SIZE4 = -0.05f;

	// Token: 0x04001AD7 RID: 6871
	private const float BLUR_BLEND1 = 1.25f;

	// Token: 0x04001AD8 RID: 6872
	private const float BLUR_BLEND2 = 1.25f;

	// Token: 0x04001AD9 RID: 6873
	private const float BLUR_BLEND3 = 1f;

	// Token: 0x04001ADA RID: 6874
	private const float BLUR_BLEND4 = 1.5f;

	// Token: 0x04001ADB RID: 6875
	private const int SILHOUETTE_RENDER_SIZE = 200;

	// Token: 0x04001ADC RID: 6876
	private const int SILHOUETTE_RENDER_DEPTH = 24;

	// Token: 0x04001ADD RID: 6877
	private const int MAX_HIGHLIGHT_EXCLUDE_PARENT_SEARCH = 25;

	// Token: 0x04001ADE RID: 6878
	private readonly string MULTISAMPLE_SHADER_NAME = "Custom/Selection/HighlightMultiSample";

	// Token: 0x04001ADF RID: 6879
	private readonly string MULTISAMPLE_BLEND_SHADER_NAME = "Custom/Selection/HighlightMultiSampleBlend";

	// Token: 0x04001AE0 RID: 6880
	private readonly string BLEND_SHADER_NAME = "Custom/Selection/HighlightMaskBlend";

	// Token: 0x04001AE1 RID: 6881
	private readonly string HIGHLIGHT_SHADER_NAME = "Custom/Selection/Highlight";

	// Token: 0x04001AE2 RID: 6882
	private readonly string UNLIT_COLOR_SHADER_NAME = "Custom/UnlitColor";

	// Token: 0x04001AE3 RID: 6883
	private readonly string UNLIT_GREY_SHADER_NAME = "Custom/Unlit/Color/Grey";

	// Token: 0x04001AE4 RID: 6884
	private readonly string UNLIT_LIGHTGREY_SHADER_NAME = "Custom/Unlit/Color/LightGrey";

	// Token: 0x04001AE5 RID: 6885
	private readonly string UNLIT_DARKGREY_SHADER_NAME = "Custom/Unlit/Color/DarkGrey";

	// Token: 0x04001AE6 RID: 6886
	private readonly string UNLIT_BLACK_SHADER_NAME = "Custom/Unlit/Color/BlackOverlay";

	// Token: 0x04001AE7 RID: 6887
	private readonly string UNLIT_WHITE_SHADER_NAME = "Custom/Unlit/Color/White";

	// Token: 0x04001AE8 RID: 6888
	public Transform m_RootTransform;

	// Token: 0x04001AE9 RID: 6889
	public float m_SilouetteRenderSize = 1f;

	// Token: 0x04001AEA RID: 6890
	public float m_SilouetteClipSize = 1f;

	// Token: 0x04001AEB RID: 6891
	private GameObject m_RenderPlane;

	// Token: 0x04001AEC RID: 6892
	private float m_RenderScale = 1f;

	// Token: 0x04001AED RID: 6893
	private Vector3 m_OrgPosition;

	// Token: 0x04001AEE RID: 6894
	private Quaternion m_OrgRotation;

	// Token: 0x04001AEF RID: 6895
	private Vector3 m_OrgScale;

	// Token: 0x04001AF0 RID: 6896
	private Shader m_MultiSampleShader;

	// Token: 0x04001AF1 RID: 6897
	private Shader m_MultiSampleBlendShader;

	// Token: 0x04001AF2 RID: 6898
	private Shader m_BlendShader;

	// Token: 0x04001AF3 RID: 6899
	private Shader m_HighlightShader;

	// Token: 0x04001AF4 RID: 6900
	private Shader m_UnlitColorShader;

	// Token: 0x04001AF5 RID: 6901
	private Shader m_UnlitGreyShader;

	// Token: 0x04001AF6 RID: 6902
	private Shader m_UnlitLightGreyShader;

	// Token: 0x04001AF7 RID: 6903
	private Shader m_UnlitDarkGreyShader;

	// Token: 0x04001AF8 RID: 6904
	private Shader m_UnlitBlackShader;

	// Token: 0x04001AF9 RID: 6905
	private Shader m_UnlitWhiteShader;

	// Token: 0x04001AFA RID: 6906
	private RenderTexture m_CameraTexture;

	// Token: 0x04001AFB RID: 6907
	private Camera m_Camera;

	// Token: 0x04001AFC RID: 6908
	private float m_CameraOrthoSize;

	// Token: 0x04001AFD RID: 6909
	private Map<Renderer, bool> m_VisibilityStates;

	// Token: 0x04001AFE RID: 6910
	private Map<Transform, Vector3> m_ObjectsOrginalPosition;

	// Token: 0x04001AFF RID: 6911
	private int m_RenderSizeX = 200;

	// Token: 0x04001B00 RID: 6912
	private int m_RenderSizeY = 200;

	// Token: 0x04001B01 RID: 6913
	private bool m_Initialized;

	// Token: 0x04001B02 RID: 6914
	private static float s_offset = -2000f;

	// Token: 0x04001B03 RID: 6915
	private Material m_Material;

	// Token: 0x04001B04 RID: 6916
	private Material m_MultiSampleMaterial;

	// Token: 0x04001B05 RID: 6917
	private Material m_MultiSampleBlendMaterial;

	// Token: 0x04001B06 RID: 6918
	private Material m_BlendMaterial;
}
