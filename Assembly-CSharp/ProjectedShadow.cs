using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000330 RID: 816
public class ProjectedShadow : MonoBehaviour
{
	// Token: 0x17000378 RID: 888
	// (get) Token: 0x06002A4E RID: 10830 RVA: 0x000CF0F0 File Offset: 0x000CD2F0
	protected Material ShadowMaterial
	{
		get
		{
			if (this.m_ShadowMaterial == null)
			{
				this.m_ShadowMaterial = new Material(this.m_ShadowShader);
				SceneUtils.SetHideFlags(this.m_ShadowMaterial, 52);
				this.m_ShadowMaterial.SetTexture("_Ramp", this.m_ShadowFalloffRamp);
				this.m_ShadowMaterial.SetTexture("_MainTex", this.m_ShadowTexture);
				this.m_ShadowMaterial.SetColor("_Color", ProjectedShadow.s_ShadowColor);
				this.m_ShadowMaterial.SetTexture("_Edge", this.m_EdgeFalloffTexture);
			}
			return this.m_ShadowMaterial;
		}
	}

	// Token: 0x17000379 RID: 889
	// (get) Token: 0x06002A4F RID: 10831 RVA: 0x000CF18C File Offset: 0x000CD38C
	protected Material ContactShadowMaterial
	{
		get
		{
			if (this.m_ContactShadowMaterial == null)
			{
				this.m_ContactShadowMaterial = new Material(this.m_ContactShadowShader);
				this.m_ContactShadowMaterial.SetFloat("_Intensity", 3.5f);
				this.m_ContactShadowMaterial.SetColor("_Color", ProjectedShadow.s_ShadowColor);
				SceneUtils.SetHideFlags(this.m_ContactShadowMaterial, 52);
			}
			return this.m_ContactShadowMaterial;
		}
	}

	// Token: 0x1700037A RID: 890
	// (get) Token: 0x06002A50 RID: 10832 RVA: 0x000CF1F8 File Offset: 0x000CD3F8
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

	// Token: 0x06002A51 RID: 10833 RVA: 0x000CF230 File Offset: 0x000CD430
	protected void Start()
	{
		GraphicsManager graphicsManager = GraphicsManager.Get();
		if (graphicsManager && graphicsManager.RealtimeShadows)
		{
			base.enabled = false;
		}
		if (this.m_ShadowShader == null)
		{
			this.m_ShadowShader = ShaderUtils.FindShader("Custom/ProjectedShadow");
		}
		if (!this.m_ShadowShader)
		{
			Debug.LogError("Failed to load Projected Shadow Shader: Custom/ProjectedShadow");
			base.enabled = false;
		}
		if (this.m_ContactShadowShader == null)
		{
			this.m_ContactShadowShader = ShaderUtils.FindShader("Custom/ContactShadow");
		}
		if (!this.m_ContactShadowShader)
		{
			Debug.LogError("Failed to load Projected Shadow Shader: Custom/ContactShadow");
			base.enabled = false;
		}
		if (this.m_ShadowFalloffRamp == null)
		{
			this.m_ShadowFalloffRamp = (Resources.Load("Textures/ProjectedShadowRamp") as Texture2D);
		}
		if (!this.m_ShadowFalloffRamp)
		{
			Debug.LogError("Failed to load Projected Shadow Ramp: Textures/ProjectedShadowRamp");
			base.enabled = false;
		}
		if (this.m_EdgeFalloffTexture == null)
		{
			this.m_EdgeFalloffTexture = (Resources.Load("Textures/ProjectedShadowEdgeAlpha") as Texture2D);
		}
		if (!this.m_EdgeFalloffTexture)
		{
			Debug.LogError("Failed to load Projected Shadow Edge Falloff Texture: Textures/ProjectedShadowEdgeAlpha");
			base.enabled = false;
		}
		if (this.m_MultiSampleShader == null)
		{
			this.m_MultiSampleShader = ShaderUtils.FindShader("Custom/Selection/HighlightMultiSample");
		}
		if (!this.m_MultiSampleShader)
		{
			Debug.LogError("Failed to load Projected Shadow Shader: Custom/Selection/HighlightMultiSample");
			base.enabled = false;
		}
		this.m_UnlitWhiteShader = ShaderUtils.FindShader("Custom/Unlit/Color/White");
		if (!this.m_UnlitWhiteShader)
		{
			Debug.LogError("Failed to load Projected Shadow Shader: Custom/Unlit/Color/White");
		}
		this.m_UnlitLightGreyShader = ShaderUtils.FindShader("Custom/Unlit/Color/LightGrey");
		if (!this.m_UnlitLightGreyShader)
		{
			Debug.LogError("Failed to load Projected Shadow Shader: Custom/Unlit/Color/LightGrey");
		}
		this.m_UnlitDarkGreyShader = ShaderUtils.FindShader("Custom/Unlit/Color/DarkGrey");
		if (!this.m_UnlitDarkGreyShader)
		{
			Debug.LogError("Failed to load Projected Shadow Shader: Custom/Unlit/Color/DarkGrey");
		}
		if (Board.Get() != null)
		{
			base.StartCoroutine(this.AssignBoardHeight_WaitForBoardStandardGameLoaded());
		}
		Actor component = base.GetComponent<Actor>();
		if (component != null)
		{
			this.m_RootObject = component.GetRootObject();
		}
		else
		{
			GameObject gameObject = SceneUtils.FindChildBySubstring(base.gameObject, "RootObject");
			if (gameObject != null)
			{
				this.m_RootObject = gameObject;
			}
			else
			{
				this.m_RootObject = base.gameObject;
			}
		}
		this.m_ShadowMaterial = this.ShadowMaterial;
	}

	// Token: 0x06002A52 RID: 10834 RVA: 0x000CF4B4 File Offset: 0x000CD6B4
	private IEnumerator AssignBoardHeight_WaitForBoardStandardGameLoaded()
	{
		if (!SceneMgr.Get())
		{
			yield break;
		}
		while (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY && BoardStandardGame.Get() == null)
		{
			yield return null;
		}
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
		{
			Transform centerBone = Board.Get().FindBone("CenterPointBone");
			if (centerBone != null)
			{
				this.m_BoardHeight = centerBone.position.y;
			}
		}
		yield break;
	}

	// Token: 0x06002A53 RID: 10835 RVA: 0x000CF4D0 File Offset: 0x000CD6D0
	protected void Update()
	{
		GraphicsManager graphicsManager = GraphicsManager.Get();
		if (graphicsManager && graphicsManager.RealtimeShadows)
		{
			base.enabled = false;
			return;
		}
		this.Render();
		if (this.m_ContactShadow)
		{
			this.RenderContactShadow();
		}
	}

	// Token: 0x06002A54 RID: 10836 RVA: 0x000CF518 File Offset: 0x000CD718
	private void OnDisable()
	{
		if (this.m_Projector != null)
		{
			this.m_Projector.enabled = false;
		}
		if (this.m_PlaneGameObject)
		{
			Object.DestroyImmediate(this.m_PlaneGameObject);
		}
		if (RenderTexture.active == this.m_ShadowTexture || RenderTexture.active == this.m_ContactShadowTexture)
		{
			RenderTexture.active = null;
		}
		if (this.m_ShadowTexture)
		{
			Object.DestroyImmediate(this.m_ShadowTexture);
		}
		this.m_ShadowTexture = null;
		if (this.m_ContactShadowTexture)
		{
			Object.DestroyImmediate(this.m_ContactShadowTexture);
		}
		this.m_ContactShadowTexture = null;
	}

	// Token: 0x06002A55 RID: 10837 RVA: 0x000CF5D4 File Offset: 0x000CD7D4
	protected void OnDestroy()
	{
		if (this.m_ShadowMaterial)
		{
			Object.Destroy(this.m_ShadowMaterial);
		}
		if (this.m_MultiSampleMaterial)
		{
			Object.Destroy(this.m_MultiSampleMaterial);
		}
		if (this.m_Camera)
		{
			Object.Destroy(this.m_Camera.gameObject);
		}
		if (this.m_ProjectorGameObject)
		{
			Object.Destroy(this.m_ProjectorGameObject);
		}
	}

	// Token: 0x06002A56 RID: 10838 RVA: 0x000CF654 File Offset: 0x000CD854
	private void OnDrawGizmos()
	{
		float num = this.m_ShadowProjectorSize * TransformUtil.ComputeWorldScale(base.transform).x * 2f;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(0.6f, 0.15f, 0.6f);
		if (this.m_ContactShadow)
		{
			Gizmos.DrawWireCube(this.m_ContactOffset, new Vector3(num, 0f, num));
		}
		else
		{
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(num, 0f, num));
		}
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x06002A57 RID: 10839 RVA: 0x000CF6F4 File Offset: 0x000CD8F4
	public void Render()
	{
		if (!this.m_ShadowEnabled || !this.m_RootObject.activeSelf)
		{
			if (this.m_Projector && this.m_Projector.enabled)
			{
				this.m_Projector.enabled = false;
			}
			if (this.m_PlaneGameObject)
			{
				this.m_PlaneGameObject.SetActive(false);
			}
			return;
		}
		float x = TransformUtil.ComputeWorldScale(base.transform).x;
		this.m_AdjustedShadowProjectorSize = this.m_ShadowProjectorSize * x;
		if (this.m_Projector == null)
		{
			this.CreateProjector();
		}
		if (this.m_Camera == null)
		{
			this.CreateCamera();
		}
		float num = (base.transform.position.y - this.m_BoardHeight) * 0.3f;
		this.m_AdjustedShadowProjectorSize += Mathf.Lerp(0f, 0.5f, num * 0.5f);
		if (this.m_ContactShadow)
		{
			float num2 = this.m_BoardHeight + 0.08f;
			if (num < num2)
			{
				if (this.m_PlaneGameObject == null)
				{
					this.m_isDirtyContactShadow = true;
				}
				else if (!this.m_PlaneGameObject.activeSelf)
				{
					this.m_isDirtyContactShadow = true;
				}
				float num3 = Mathf.Clamp((num2 - num) / num2, 0f, 1f);
				if (this.m_ContactShadowTexture)
				{
					this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_ContactShadowTexture;
					this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.color = ProjectedShadow.s_ShadowColor;
					this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_Alpha", num3);
				}
			}
			else if (this.m_PlaneGameObject != null)
			{
				this.m_PlaneGameObject.SetActive(false);
			}
		}
		if (num < this.m_AutoDisableHeight && this.m_AutoBoardHeightDisable)
		{
			this.m_Projector.enabled = false;
			Object.DestroyImmediate(this.m_ShadowTexture);
			this.m_ShadowTexture = null;
			return;
		}
		this.m_Projector.enabled = true;
		float num4 = 0f;
		if (base.transform.parent != null)
		{
			num4 = Mathf.Lerp(-0.7f, 1.8f, base.transform.parent.position.x / 17f * -1f) * num;
		}
		Vector3 position;
		position..ctor(base.transform.position.x - num4 - num * 0.25f, base.transform.position.y, base.transform.position.z - num * 0.8f);
		this.m_ProjectorTransform.position = position;
		this.m_ProjectorTransform.Translate(this.m_ProjectionOffset);
		if (!this.m_ContinuousRendering)
		{
			Quaternion rotation = base.transform.rotation;
			float num5 = (1f - rotation.z) * 0.5f + 0.5f;
			float num6 = rotation.x * 0.5f;
			this.m_Projector.aspectRatio = num5 - num6;
			this.m_Projector.orthographicSize = this.m_AdjustedShadowProjectorSize + num6;
			this.m_ProjectorTransform.rotation = Quaternion.identity;
			this.m_ProjectorTransform.Rotate(90f, rotation.eulerAngles.y, 0f);
		}
		else
		{
			this.m_ProjectorTransform.rotation = Quaternion.identity;
			this.m_ProjectorTransform.Rotate(90f, 0f, 0f);
			this.m_Projector.orthographicSize = this.m_AdjustedShadowProjectorSize;
		}
		int num7 = 64;
		if (this.m_ShadowTexture == null)
		{
			this.m_ShadowTexture = new RenderTexture(num7, num7, 32);
			this.m_ShadowTexture.wrapMode = 1;
			this.RenderShadowMask();
		}
		else if (this.m_ContinuousRendering || !this.m_ShadowTexture.IsCreated())
		{
			this.RenderShadowMask();
		}
	}

	// Token: 0x06002A58 RID: 10840 RVA: 0x000CFB29 File Offset: 0x000CDD29
	public static void SetShadowColor(Color color)
	{
		ProjectedShadow.s_ShadowColor = color;
	}

	// Token: 0x06002A59 RID: 10841 RVA: 0x000CFB31 File Offset: 0x000CDD31
	public void EnableShadow()
	{
		this.m_ShadowEnabled = true;
	}

	// Token: 0x06002A5A RID: 10842 RVA: 0x000CFB3C File Offset: 0x000CDD3C
	public void EnableShadow(float FadeInTime)
	{
		this.m_ShadowEnabled = true;
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			0,
			"to",
			1,
			"time",
			FadeInTime,
			"easetype",
			iTween.EaseType.easeInCubic,
			"onupdate",
			"UpdateShadowColor",
			"onupdatetarget",
			base.gameObject,
			"name",
			"ProjectedShadowFade"
		});
		iTween.StopByName(base.gameObject, "ProjectedShadowFade");
		iTween.ValueTo(base.gameObject, args);
	}

	// Token: 0x06002A5B RID: 10843 RVA: 0x000CFBF3 File Offset: 0x000CDDF3
	public void DisableShadow()
	{
		this.DisableShadowProjector();
	}

	// Token: 0x06002A5C RID: 10844 RVA: 0x000CFBFC File Offset: 0x000CDDFC
	public void DisableShadow(float FadeOutTime)
	{
		if (this.m_Projector == null || !this.m_ShadowEnabled)
		{
			return;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			1,
			"to",
			0,
			"time",
			FadeOutTime,
			"easetype",
			iTween.EaseType.easeOutCubic,
			"onupdate",
			"UpdateShadowColor",
			"onupdatetarget",
			base.gameObject,
			"name",
			"ProjectedShadowFade",
			"oncomplete",
			"DisableShadowProjector"
		});
		iTween.StopByName(base.gameObject, "ProjectedShadowFade");
		iTween.ValueTo(base.gameObject, args);
	}

	// Token: 0x06002A5D RID: 10845 RVA: 0x000CFCDB File Offset: 0x000CDEDB
	public void UpdateContactShadow(Spell spell, SpellStateType prevStateType, object userData)
	{
		this.UpdateContactShadow();
	}

	// Token: 0x06002A5E RID: 10846 RVA: 0x000CFCE3 File Offset: 0x000CDEE3
	public void UpdateContactShadow(Spell spell, object userData)
	{
		this.UpdateContactShadow();
	}

	// Token: 0x06002A5F RID: 10847 RVA: 0x000CFCEB File Offset: 0x000CDEEB
	public void UpdateContactShadow(Spell spell)
	{
		this.UpdateContactShadow();
	}

	// Token: 0x06002A60 RID: 10848 RVA: 0x000CFCF3 File Offset: 0x000CDEF3
	public void UpdateContactShadow()
	{
		if (!this.m_ContactShadow)
		{
			return;
		}
		this.m_isDirtyContactShadow = true;
	}

	// Token: 0x06002A61 RID: 10849 RVA: 0x000CFD08 File Offset: 0x000CDF08
	private void DisableShadowProjector()
	{
		if (this.m_Projector != null)
		{
			this.m_Projector.enabled = false;
		}
		this.m_ShadowEnabled = false;
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x000CFD3C File Offset: 0x000CDF3C
	private void UpdateShadowColor(float val)
	{
		if (this.m_Projector == null || this.m_Projector.material == null)
		{
			return;
		}
		Color color;
		color..ctor(0.5f, 0.5f, 0.5f, 0.5f);
		Color color2 = Color.Lerp(color, ProjectedShadow.s_ShadowColor, val);
		this.m_Projector.material.SetColor("_Color", color2);
	}

	// Token: 0x06002A63 RID: 10851 RVA: 0x000CFDB0 File Offset: 0x000CDFB0
	private void RenderShadowMask()
	{
		this.m_ShadowTexture.DiscardContents();
		this.m_Camera.depth = Camera.main.depth - 3f;
		this.m_Camera.clearFlags = 2;
		Vector3 position = base.transform.position;
		Vector3 localScale = base.transform.localScale;
		ProjectedShadow.s_offset -= 10f;
		if (ProjectedShadow.s_offset < -19000f)
		{
			ProjectedShadow.s_offset = -12000f;
		}
		Vector3 position2 = Vector3.left * ProjectedShadow.s_offset;
		base.transform.position = position2;
		this.m_Camera.transform.position = position2;
		this.m_Camera.transform.rotation = Quaternion.identity;
		this.m_Camera.transform.Rotate(90f, 0f, 0f);
		RenderTexture temporary = RenderTexture.GetTemporary(80, 80);
		RenderTexture temporary2 = RenderTexture.GetTemporary(80, 80);
		this.m_Camera.targetTexture = temporary;
		float x = TransformUtil.ComputeWorldScale(base.transform).x;
		this.m_Camera.orthographicSize = this.m_ShadowProjectorSize * x - 0.11f - 0.05f;
		this.m_Camera.RenderWithShader(this.m_UnlitWhiteShader, "Highlight");
		this.Sample(temporary, temporary2, 0.6f);
		this.Sample(temporary2, this.m_ShadowTexture, 0.8f);
		this.ShadowMaterial.SetTexture("_MainTex", this.m_ShadowTexture);
		this.ShadowMaterial.SetColor("_Color", ProjectedShadow.s_ShadowColor);
		base.transform.position = position;
		base.transform.localScale = localScale;
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
	}

	// Token: 0x06002A64 RID: 10852 RVA: 0x000CFF70 File Offset: 0x000CE170
	private IEnumerator DelayRenderContactShadow()
	{
		yield return null;
		this.m_isDirtyContactShadow = true;
		yield break;
	}

	// Token: 0x06002A65 RID: 10853 RVA: 0x000CFF8C File Offset: 0x000CE18C
	private void RenderContactShadow()
	{
		GraphicsManager graphicsManager = GraphicsManager.Get();
		if (graphicsManager && graphicsManager.RealtimeShadows)
		{
			base.enabled = false;
		}
		if (this.m_ContactShadowTexture != null && !this.m_isDirtyContactShadow && this.m_ContactShadowTexture.IsCreated())
		{
			return;
		}
		float x = TransformUtil.ComputeWorldScale(base.transform).x;
		this.m_AdjustedShadowProjectorSize = this.m_ShadowProjectorSize * x;
		if (this.m_Camera == null)
		{
			this.CreateCamera();
		}
		if (this.m_PlaneGameObject == null)
		{
			this.CreateRenderPlane();
		}
		this.m_PlaneGameObject.SetActive(true);
		if (this.m_ContactShadowTexture == null)
		{
			this.m_ContactShadowTexture = new RenderTexture(80, 80, 32);
		}
		Quaternion localRotation = base.transform.localRotation;
		Vector3 position = base.transform.position;
		Vector3 localScale = base.transform.localScale;
		ProjectedShadow.s_offset -= 10f;
		if (ProjectedShadow.s_offset < -19000f)
		{
			ProjectedShadow.s_offset = -12000f;
		}
		Vector3 position2 = Vector3.left * ProjectedShadow.s_offset;
		base.transform.position = position2;
		base.transform.rotation = Quaternion.identity;
		this.SetWorldScale(base.transform, Vector3.one);
		this.m_Camera.transform.position = position2;
		this.m_Camera.transform.rotation = Quaternion.identity;
		this.m_Camera.transform.Rotate(90f, 0f, 0f);
		RenderTexture temporary = RenderTexture.GetTemporary(80, 80);
		this.m_Camera.depth = Camera.main.depth - 3f;
		this.m_Camera.clearFlags = 2;
		this.m_Camera.targetTexture = temporary;
		this.m_Camera.orthographicSize = this.m_ShadowProjectorSize - 0.11f - 0.05f;
		this.m_Camera.RenderWithShader(this.m_UnlitDarkGreyShader, "Highlight");
		this.m_ContactShadowTexture.DiscardContents();
		this.Sample(temporary, this.m_ContactShadowTexture, 0.6f);
		base.transform.localRotation = localRotation;
		base.transform.position = position;
		base.transform.localScale = localScale;
		this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_ContactShadowTexture;
		this.m_isDirtyContactShadow = false;
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x06002A66 RID: 10854 RVA: 0x000D021C File Offset: 0x000CE41C
	private void Sample(RenderTexture source, RenderTexture dest, float off)
	{
		Graphics.BlitMultiTap(source, dest, this.MultiSampleMaterial, new Vector2[]
		{
			new Vector2(-off, -off),
			new Vector2(-off, off),
			new Vector2(off, off),
			new Vector2(off, -off)
		});
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x000D028C File Offset: 0x000CE48C
	private void CreateProjector()
	{
		if (this.m_Projector != null)
		{
			Object.Destroy(this.m_Projector);
			this.m_Projector = null;
		}
		if (this.m_ProjectorGameObject != null)
		{
			Object.Destroy(this.m_ProjectorGameObject);
			this.m_ProjectorGameObject = null;
			this.m_ProjectorTransform = null;
		}
		this.m_ProjectorGameObject = new GameObject(string.Format("{0}_{1}", base.name, "ShadowProjector"));
		this.m_Projector = this.m_ProjectorGameObject.AddComponent<Projector>();
		this.m_ProjectorTransform = this.m_ProjectorGameObject.transform;
		this.m_ProjectorTransform.Rotate(90f, 0f, 0f);
		if (this.m_RootObject != null)
		{
			this.m_ProjectorTransform.parent = this.m_RootObject.transform;
		}
		this.m_Projector.nearClipPlane = 0f;
		this.m_Projector.farClipPlane = this.m_ProjectionFarClip;
		this.m_Projector.orthographic = true;
		this.m_Projector.orthographicSize = this.m_AdjustedShadowProjectorSize;
		SceneUtils.SetHideFlags(this.m_Projector, 61);
		this.m_Projector.material = this.m_ShadowMaterial;
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x000D03C8 File Offset: 0x000CE5C8
	private void CreateCamera()
	{
		if (this.m_Camera != null)
		{
			Object.Destroy(this.m_Camera);
		}
		GameObject gameObject = new GameObject();
		this.m_Camera = gameObject.AddComponent<Camera>();
		gameObject.name = base.name + "_ShadowCamera";
		SceneUtils.SetHideFlags(gameObject, 61);
		this.m_Camera.orthographic = true;
		this.m_Camera.orthographicSize = this.m_AdjustedShadowProjectorSize;
		this.m_Camera.transform.position = base.transform.position;
		this.m_Camera.transform.rotation = base.transform.rotation;
		this.m_Camera.transform.Rotate(90f, 0f, 0f);
		if (this.m_RootObject != null)
		{
			this.m_Camera.transform.parent = this.m_RootObject.transform;
		}
		this.m_Camera.nearClipPlane = -3f;
		this.m_Camera.farClipPlane = 3f;
		if (Camera.main != null)
		{
			this.m_Camera.depth = Camera.main.depth - 5f;
		}
		else
		{
			this.m_Camera.depth = -4f;
		}
		this.m_Camera.backgroundColor = Color.black;
		this.m_Camera.clearFlags = 2;
		this.m_Camera.depthTextureMode = 0;
		this.m_Camera.renderingPath = 1;
		this.m_Camera.SetReplacementShader(this.m_UnlitWhiteShader, "Highlight");
		this.m_Camera.enabled = false;
	}

	// Token: 0x06002A69 RID: 10857 RVA: 0x000D0578 File Offset: 0x000CE778
	private void CreateRenderPlane()
	{
		if (this.m_PlaneGameObject != null)
		{
			Object.DestroyImmediate(this.m_PlaneGameObject);
		}
		this.m_PlaneGameObject = new GameObject();
		this.m_PlaneGameObject.name = base.name + "_ContactShadowRenderPlane";
		if (this.m_RootObject != null)
		{
			this.m_PlaneGameObject.transform.parent = this.m_RootObject.transform;
		}
		this.m_PlaneGameObject.transform.localPosition = this.m_ContactOffset;
		this.m_PlaneGameObject.transform.localRotation = Quaternion.identity;
		this.m_PlaneGameObject.transform.localScale = new Vector3(0.98f, 1f, 0.98f);
		this.m_PlaneGameObject.AddComponent<MeshFilter>();
		this.m_PlaneGameObject.AddComponent<MeshRenderer>();
		SceneUtils.SetHideFlags(this.m_PlaneGameObject, 61);
		Mesh mesh = new Mesh();
		mesh.name = "ContactShadowMeshPlane";
		float shadowProjectorSize = this.m_ShadowProjectorSize;
		float shadowProjectorSize2 = this.m_ShadowProjectorSize;
		mesh.vertices = new Vector3[]
		{
			new Vector3(-shadowProjectorSize, 0f, -shadowProjectorSize2),
			new Vector3(shadowProjectorSize, 0f, -shadowProjectorSize2),
			new Vector3(-shadowProjectorSize, 0f, shadowProjectorSize2),
			new Vector3(shadowProjectorSize, 0f, shadowProjectorSize2)
		};
		mesh.uv = this.PLANE_UVS;
		mesh.normals = this.PLANE_NORMALS;
		mesh.triangles = this.PLANE_TRIANGLES;
		Mesh mesh2 = mesh;
		this.m_PlaneGameObject.GetComponent<MeshFilter>().mesh = mesh2;
		this.m_PlaneMesh = mesh2;
		this.m_PlaneMesh.RecalculateBounds();
		this.m_ContactShadowMaterial = this.ContactShadowMaterial;
		this.m_ContactShadowMaterial.color = ProjectedShadow.s_ShadowColor;
		if (this.m_ContactShadowMaterial)
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial = this.m_ContactShadowMaterial;
		}
	}

	// Token: 0x06002A6A RID: 10858 RVA: 0x000D0784 File Offset: 0x000CE984
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

	// Token: 0x04001932 RID: 6450
	private const int RENDER_SIZE = 64;

	// Token: 0x04001933 RID: 6451
	private const string SHADER_NAME = "Custom/ProjectedShadow";

	// Token: 0x04001934 RID: 6452
	private const string CONTACT_SHADER_NAME = "Custom/ContactShadow";

	// Token: 0x04001935 RID: 6453
	private const string SHADER_FALLOFF_RAMP = "Textures/ProjectedShadowRamp";

	// Token: 0x04001936 RID: 6454
	private const string EDGE_FALLOFF_TEXTURE = "Textures/ProjectedShadowEdgeAlpha";

	// Token: 0x04001937 RID: 6455
	private const string GAMEOBJECT_NAME_EXT = "ShadowProjector";

	// Token: 0x04001938 RID: 6456
	private const string UNLIT_WHITE_SHADER_NAME = "Custom/Unlit/Color/White";

	// Token: 0x04001939 RID: 6457
	private const string UNLIT_LIGHTGREY_SHADER_NAME = "Custom/Unlit/Color/LightGrey";

	// Token: 0x0400193A RID: 6458
	private const string UNLIT_DARKGREY_SHADER_NAME = "Custom/Unlit/Color/DarkGrey";

	// Token: 0x0400193B RID: 6459
	private const string MULTISAMPLE_SHADER_NAME = "Custom/Selection/HighlightMultiSample";

	// Token: 0x0400193C RID: 6460
	private const float NEARCLIP_PLANE = 0f;

	// Token: 0x0400193D RID: 6461
	private const float SHADOW_OFFSET_SCALE = 0.3f;

	// Token: 0x0400193E RID: 6462
	private const float RENDERMASK_OFFSET = 0.11f;

	// Token: 0x0400193F RID: 6463
	private const float RENDERMASK_BLUR = 0.6f;

	// Token: 0x04001940 RID: 6464
	private const float RENDERMASK_BLUR2 = 0.8f;

	// Token: 0x04001941 RID: 6465
	private const float CONTACT_SHADOW_SCALE = 0.98f;

	// Token: 0x04001942 RID: 6466
	private const float CONTACT_SHADOW_FADE_IN_HEIGHT = 0.08f;

	// Token: 0x04001943 RID: 6467
	private const float CONTACT_SHADOW_INTENSITY = 3.5f;

	// Token: 0x04001944 RID: 6468
	private const int CONTACT_SHADOW_RESOLUTION = 80;

	// Token: 0x04001945 RID: 6469
	private readonly Vector2[] PLANE_UVS = new Vector2[]
	{
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f)
	};

	// Token: 0x04001946 RID: 6470
	private readonly Vector3[] PLANE_NORMALS = new Vector3[]
	{
		Vector3.up,
		Vector3.up,
		Vector3.up,
		Vector3.up
	};

	// Token: 0x04001947 RID: 6471
	private readonly int[] PLANE_TRIANGLES = new int[]
	{
		3,
		1,
		2,
		2,
		1,
		0
	};

	// Token: 0x04001948 RID: 6472
	public float m_ShadowProjectorSize = 1.5f;

	// Token: 0x04001949 RID: 6473
	public bool m_ShadowEnabled;

	// Token: 0x0400194A RID: 6474
	public bool m_AutoBoardHeightDisable;

	// Token: 0x0400194B RID: 6475
	public float m_AutoDisableHeight;

	// Token: 0x0400194C RID: 6476
	public bool m_ContinuousRendering;

	// Token: 0x0400194D RID: 6477
	public float m_ProjectionFarClip = 10f;

	// Token: 0x0400194E RID: 6478
	public Vector3 m_ProjectionOffset;

	// Token: 0x0400194F RID: 6479
	public bool m_ContactShadow;

	// Token: 0x04001950 RID: 6480
	public Vector3 m_ContactOffset = Vector3.zero;

	// Token: 0x04001951 RID: 6481
	public bool m_isDirtyContactShadow = true;

	// Token: 0x04001952 RID: 6482
	private static float s_offset = -12000f;

	// Token: 0x04001953 RID: 6483
	private static Color s_ShadowColor = new Color(0.098f, 0.098f, 0.235f, 0.45f);

	// Token: 0x04001954 RID: 6484
	private GameObject m_RootObject;

	// Token: 0x04001955 RID: 6485
	private GameObject m_ProjectorGameObject;

	// Token: 0x04001956 RID: 6486
	private Transform m_ProjectorTransform;

	// Token: 0x04001957 RID: 6487
	private Projector m_Projector;

	// Token: 0x04001958 RID: 6488
	private Camera m_Camera;

	// Token: 0x04001959 RID: 6489
	private RenderTexture m_ShadowTexture;

	// Token: 0x0400195A RID: 6490
	private RenderTexture m_ContactShadowTexture;

	// Token: 0x0400195B RID: 6491
	private float m_AdjustedShadowProjectorSize = 1.5f;

	// Token: 0x0400195C RID: 6492
	private float m_BoardHeight = 0.2f;

	// Token: 0x0400195D RID: 6493
	private Mesh m_PlaneMesh;

	// Token: 0x0400195E RID: 6494
	private GameObject m_PlaneGameObject;

	// Token: 0x0400195F RID: 6495
	private Texture2D m_ShadowFalloffRamp;

	// Token: 0x04001960 RID: 6496
	private Texture2D m_EdgeFalloffTexture;

	// Token: 0x04001961 RID: 6497
	private Shader m_ShadowShader;

	// Token: 0x04001962 RID: 6498
	private Shader m_UnlitWhiteShader;

	// Token: 0x04001963 RID: 6499
	private Shader m_UnlitDarkGreyShader;

	// Token: 0x04001964 RID: 6500
	private Shader m_UnlitLightGreyShader;

	// Token: 0x04001965 RID: 6501
	private Material m_ShadowMaterial;

	// Token: 0x04001966 RID: 6502
	private Shader m_ContactShadowShader;

	// Token: 0x04001967 RID: 6503
	private Material m_ContactShadowMaterial;

	// Token: 0x04001968 RID: 6504
	private Shader m_MultiSampleShader;

	// Token: 0x04001969 RID: 6505
	private Material m_MultiSampleMaterial;
}
