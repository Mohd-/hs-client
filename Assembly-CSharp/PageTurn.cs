using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000703 RID: 1795
public class PageTurn : MonoBehaviour
{
	// Token: 0x060049B6 RID: 18870 RVA: 0x001609FC File Offset: 0x0015EBFC
	private void Awake()
	{
		this.m_initialPosition = base.transform.position;
		Transform transform = base.transform.Find(this.FRONT_PAGE_NAME);
		if (transform != null)
		{
			this.m_FrontPageGameObject = transform.gameObject;
		}
		if (this.m_FrontPageGameObject == null)
		{
			Debug.LogError("Failed to find " + this.FRONT_PAGE_NAME + " Object.");
		}
		transform = base.transform.Find(this.BACK_PAGE_NAME);
		if (transform != null)
		{
			this.m_BackPageGameObject = transform.gameObject;
		}
		if (this.m_BackPageGameObject == null)
		{
			Debug.LogError("Failed to find " + this.BACK_PAGE_NAME + " Object.");
		}
		this.Show(false);
		this.m_TheBoxOuterFrame = Box.Get().m_OuterFrame;
		this.CreateCamera();
		this.CreateRenderTexture();
		this.SetupMaterial();
	}

	// Token: 0x060049B7 RID: 18871 RVA: 0x00160AF0 File Offset: 0x0015ECF0
	protected void OnEnable()
	{
		if (this.m_OffscreenPageTurnCameraGO != null)
		{
			this.CreateCamera();
		}
		if (this.m_TempRenderBuffer != null || this.m_TempMaskBuffer != null)
		{
			this.CreateRenderTexture();
			this.SetupMaterial();
		}
	}

	// Token: 0x060049B8 RID: 18872 RVA: 0x00160B44 File Offset: 0x0015ED44
	protected void OnDisable()
	{
		if (this.m_TempRenderBuffer != null)
		{
			Object.Destroy(this.m_TempRenderBuffer);
		}
		if (this.m_TempMaskBuffer != null)
		{
			Object.Destroy(this.m_TempMaskBuffer);
		}
		if (this.m_OffscreenPageTurnCameraGO != null)
		{
			Object.Destroy(this.m_OffscreenPageTurnCameraGO);
		}
		if (this.m_OffscreenPageTurnCamera != null)
		{
			Object.Destroy(this.m_OffscreenPageTurnCamera);
		}
		if (this.m_OffscreenPageTurnMaskCamera != null)
		{
			Object.Destroy(this.m_OffscreenPageTurnMaskCamera);
		}
	}

	// Token: 0x060049B9 RID: 18873 RVA: 0x00160BDD File Offset: 0x0015EDDD
	public void TurnRight(GameObject flippingPage, GameObject otherPage)
	{
		this.TurnRight(flippingPage, otherPage, null);
	}

	// Token: 0x060049BA RID: 18874 RVA: 0x00160BE8 File Offset: 0x0015EDE8
	public void TurnRight(GameObject flippingPage, GameObject otherPage, PageTurn.DelOnPageTurnComplete callback)
	{
		this.TurnRight(flippingPage, otherPage, callback, null);
	}

	// Token: 0x060049BB RID: 18875 RVA: 0x00160BF4 File Offset: 0x0015EDF4
	public void TurnRight(GameObject flippingPage, GameObject otherPage, PageTurn.DelOnPageTurnComplete callback, object callbackData)
	{
		this.Render(flippingPage);
		if (GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.Low)
		{
			Time.captureFramerate = 18;
		}
		else if (GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.Medium)
		{
			Time.captureFramerate = 24;
		}
		else
		{
			Time.captureFramerate = 30;
		}
		base.GetComponent<Animation>().Stop(this.PAGE_TURN_RIGHT_ANIM);
		base.GetComponent<Animation>()[this.PAGE_TURN_RIGHT_ANIM].time = 0f;
		base.GetComponent<Animation>()[this.PAGE_TURN_RIGHT_ANIM].speed = this.m_TurnRightSpeed;
		base.GetComponent<Animation>().Play(this.PAGE_TURN_RIGHT_ANIM);
		this.m_FrontPageGameObject.GetComponent<Renderer>().material.SetFloat("_Alpha", 1f);
		this.m_BackPageGameObject.GetComponent<Renderer>().material.SetFloat("_Alpha", 1f);
		float secondsToWait = base.GetComponent<Animation>()[this.PAGE_TURN_RIGHT_ANIM].length / this.m_TurnRightSpeed;
		PageTurn.PageTurningData pageTurningData = new PageTurn.PageTurningData
		{
			m_secondsToWait = secondsToWait,
			m_callback = callback,
			m_callbackData = callbackData
		};
		base.StopCoroutine(this.WAIT_THEN_COMPLETE_PAGE_TURN_COROUTINE);
		base.StartCoroutine(this.WAIT_THEN_COMPLETE_PAGE_TURN_COROUTINE, pageTurningData);
	}

	// Token: 0x060049BC RID: 18876 RVA: 0x00160D34 File Offset: 0x0015EF34
	public void TurnLeft(GameObject flippingPage, GameObject otherPage)
	{
		this.TurnLeft(flippingPage, otherPage, null);
	}

	// Token: 0x060049BD RID: 18877 RVA: 0x00160D3F File Offset: 0x0015EF3F
	public void TurnLeft(GameObject flippingPage, GameObject otherPage, PageTurn.DelOnPageTurnComplete callback)
	{
		this.TurnLeft(flippingPage, otherPage, callback, null);
	}

	// Token: 0x060049BE RID: 18878 RVA: 0x00160D4C File Offset: 0x0015EF4C
	public void TurnLeft(GameObject flippingPage, GameObject otherPage, PageTurn.DelOnPageTurnComplete callback, object callbackData)
	{
		PageTurn.TurnPageData turnPageData = new PageTurn.TurnPageData();
		turnPageData.flippingPage = flippingPage;
		turnPageData.otherPage = otherPage;
		turnPageData.callback = callback;
		turnPageData.callbackData = callbackData;
		base.StopCoroutine("TurnLeftPage");
		base.StartCoroutine("TurnLeftPage", turnPageData);
	}

	// Token: 0x060049BF RID: 18879 RVA: 0x00160D94 File Offset: 0x0015EF94
	private IEnumerator TurnLeftPage(PageTurn.TurnPageData pageData)
	{
		yield return null;
		yield return null;
		yield return null;
		GameObject flippingPage = pageData.flippingPage;
		GameObject otherPage = pageData.otherPage;
		PageTurn.DelOnPageTurnComplete callback = pageData.callback;
		object callbackData = pageData.callbackData;
		Vector3 pagePos = flippingPage.transform.position;
		Vector3 otherPagePos = otherPage.transform.position;
		flippingPage.transform.position = otherPagePos;
		otherPage.transform.position = pagePos;
		this.Render(flippingPage);
		flippingPage.transform.position = pagePos;
		otherPage.transform.position = otherPagePos;
		if (GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.Low)
		{
			Time.captureFramerate = 18;
		}
		else if (GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.Medium)
		{
			Time.captureFramerate = 24;
		}
		else
		{
			Time.captureFramerate = 30;
		}
		this.m_FrontPageGameObject.GetComponent<Renderer>().material.SetFloat("_Alpha", 1f);
		this.m_BackPageGameObject.GetComponent<Renderer>().material.SetFloat("_Alpha", 1f);
		base.GetComponent<Animation>().Stop(this.PAGE_TURN_LEFT_ANIM);
		base.GetComponent<Animation>()[this.PAGE_TURN_LEFT_ANIM].time = 0.22f;
		base.GetComponent<Animation>()[this.PAGE_TURN_LEFT_ANIM].speed = this.m_TurnLeftSpeed;
		base.GetComponent<Animation>().Play(this.PAGE_TURN_LEFT_ANIM);
		base.GetComponent<Animation>().Stop(this.PAGE_TURN_MAT_ANIM);
		base.GetComponent<Animation>()[this.PAGE_TURN_MAT_ANIM].time = 0.22f;
		base.GetComponent<Animation>()[this.PAGE_TURN_MAT_ANIM].speed = this.m_TurnLeftSpeed;
		base.GetComponent<Animation>().Blend(this.PAGE_TURN_MAT_ANIM, 1f, 0f);
		float showNewPage = 0.35f;
		while (base.GetComponent<Animation>()[this.PAGE_TURN_LEFT_ANIM].time < base.GetComponent<Animation>()[this.PAGE_TURN_LEFT_ANIM].length - showNewPage)
		{
			yield return null;
		}
		PageTurn.PageTurningData pageTurningData = new PageTurn.PageTurningData
		{
			m_callback = callback,
			m_callbackData = callbackData,
			m_animation = base.GetComponent<Animation>()[this.PAGE_TURN_LEFT_ANIM]
		};
		base.StartCoroutine(this.FinishTurnLeftPage(pageTurningData));
		if (callbackData == null)
		{
			yield break;
		}
		pageTurningData.m_callback(callbackData);
		yield break;
	}

	// Token: 0x060049C0 RID: 18880 RVA: 0x00160DC0 File Offset: 0x0015EFC0
	private IEnumerator FinishTurnLeftPage(PageTurn.PageTurningData pageTurningData)
	{
		while (base.GetComponent<Animation>().isPlaying)
		{
			yield return null;
		}
		Time.captureFramerate = 0;
		this.Show(false);
		yield break;
	}

	// Token: 0x060049C1 RID: 18881 RVA: 0x00160DDC File Offset: 0x0015EFDC
	private void CreateCamera()
	{
		if (this.m_OffscreenPageTurnCameraGO == null)
		{
			if (this.m_OffscreenPageTurnCamera != null)
			{
				Object.DestroyImmediate(this.m_OffscreenPageTurnCamera);
			}
			this.m_OffscreenPageTurnCameraGO = new GameObject();
			this.m_OffscreenPageTurnCamera = this.m_OffscreenPageTurnCameraGO.AddComponent<Camera>();
			this.m_OffscreenPageTurnCameraGO.name = base.name + "_OffScreenPageTurnCamera";
			this.SetupCamera(this.m_OffscreenPageTurnCamera);
		}
		if (this.m_OffscreenPageTurnMaskCamera == null)
		{
			GameObject gameObject = new GameObject();
			this.m_OffscreenPageTurnMaskCamera = gameObject.AddComponent<Camera>();
			gameObject.name = base.name + "_OffScreenPageTurnMaskCamera";
			this.SetupCamera(this.m_OffscreenPageTurnMaskCamera);
			this.m_OffscreenPageTurnMaskCamera.SetReplacementShader(this.m_MaskShader, "BasePage");
		}
	}

	// Token: 0x060049C2 RID: 18882 RVA: 0x00160EB4 File Offset: 0x0015F0B4
	private void SetupCamera(Camera camera)
	{
		camera.orthographic = true;
		camera.orthographicSize = PageTurn.GetWorldScale(this.m_FrontPageGameObject.transform).x / 2f;
		camera.transform.parent = base.transform;
		camera.nearClipPlane = -20f;
		camera.farClipPlane = 20f;
		camera.depth = ((!(Camera.main == null)) ? (Camera.main.depth + 100f) : 0f);
		camera.backgroundColor = Color.black;
		camera.clearFlags = 2;
		camera.cullingMask = (GameLayer.Default.LayerBit() | GameLayer.CardRaycast.LayerBit());
		camera.enabled = false;
		camera.renderingPath = 1;
		camera.transform.Rotate(90f, 0f, 0f);
		SceneUtils.SetHideFlags(camera, 61);
	}

	// Token: 0x060049C3 RID: 18883 RVA: 0x00160F98 File Offset: 0x0015F198
	private void CreateRenderTexture()
	{
		int num = Screen.currentResolution.width;
		if (num < Screen.currentResolution.height)
		{
			num = Screen.currentResolution.height;
		}
		int num2 = 512;
		if (num > 640)
		{
			num2 = 1024;
		}
		if (num > 1280)
		{
			num2 = 2048;
		}
		if (num > 2500)
		{
			num2 = 4096;
		}
		GraphicsQuality renderQualityLevel = GraphicsManager.Get().RenderQualityLevel;
		if (renderQualityLevel == GraphicsQuality.Medium)
		{
			num2 = 1024;
		}
		else if (renderQualityLevel == GraphicsQuality.Low)
		{
			num2 = 512;
		}
		if (this.m_TempRenderBuffer == null)
		{
			if (renderQualityLevel == GraphicsQuality.High)
			{
				this.m_TempRenderBuffer = new RenderTexture(num2, num2, 16, 0, 0);
			}
			else
			{
				bool flag = SystemInfo.SupportsRenderTextureFormat(6);
				if (flag)
				{
					this.m_TempRenderBuffer = new RenderTexture(num2, num2, 16, 6, 0);
				}
				else if (renderQualityLevel == GraphicsQuality.Low && SystemInfo.SupportsRenderTextureFormat(5))
				{
					this.m_TempRenderBuffer = new RenderTexture(num2, num2, 16, 5, 0);
				}
				else
				{
					this.m_TempRenderBuffer = new RenderTexture(num2, num2, 16, 7, 0);
				}
			}
			this.m_TempRenderBuffer.Create();
		}
		if (this.m_TempMaskBuffer == null)
		{
			if (renderQualityLevel == GraphicsQuality.High)
			{
				this.m_TempMaskBuffer = new RenderTexture(num2, num2, 16, 0, 0);
			}
			else
			{
				bool flag2 = SystemInfo.SupportsRenderTextureFormat(6);
				if (flag2)
				{
					this.m_TempMaskBuffer = new RenderTexture(num2, num2, 16, 6, 0);
				}
				else if (renderQualityLevel == GraphicsQuality.Low && SystemInfo.SupportsRenderTextureFormat(5))
				{
					this.m_TempMaskBuffer = new RenderTexture(num2, num2, 16, 5, 0);
				}
				else
				{
					this.m_TempMaskBuffer = new RenderTexture(num2, num2, 16, 7, 0);
				}
			}
			this.m_TempMaskBuffer.Create();
		}
		if (this.m_OffscreenPageTurnCamera != null)
		{
			this.m_OffscreenPageTurnCamera.targetTexture = this.m_TempRenderBuffer;
		}
		if (this.m_OffscreenPageTurnMaskCamera != null)
		{
			this.m_OffscreenPageTurnMaskCamera.targetTexture = this.m_TempMaskBuffer;
		}
	}

	// Token: 0x060049C4 RID: 18884 RVA: 0x001611AC File Offset: 0x0015F3AC
	private void Render(GameObject page)
	{
		this.Show(true);
		this.m_FrontPageGameObject.SetActive(true);
		this.m_BackPageGameObject.SetActive(true);
		this.m_OffscreenPageTurnCameraGO.transform.position = base.transform.position;
		bool enabled = this.m_FrontPageGameObject.GetComponent<Renderer>().enabled;
		bool enabled2 = this.m_BackPageGameObject.GetComponent<Renderer>().enabled;
		this.m_FrontPageGameObject.GetComponent<Renderer>().enabled = false;
		this.m_BackPageGameObject.GetComponent<Renderer>().enabled = false;
		bool activeSelf = this.m_TheBoxOuterFrame.activeSelf;
		this.m_TheBoxOuterFrame.SetActive(false);
		this.m_OffscreenPageTurnCamera.Render();
		this.m_OffscreenPageTurnMaskCamera.transform.position = base.transform.position;
		this.m_OffscreenPageTurnMaskCamera.RenderWithShader(this.m_MaskShader, "BasePage");
		this.m_FrontPageGameObject.GetComponent<Renderer>().enabled = enabled;
		this.m_BackPageGameObject.GetComponent<Renderer>().enabled = enabled2;
		this.m_TheBoxOuterFrame.SetActive(activeSelf);
	}

	// Token: 0x060049C5 RID: 18885 RVA: 0x001612B9 File Offset: 0x0015F4B9
	public void SetBackPageMaterial(Material material)
	{
		this.m_BackPageGameObject.GetComponent<Renderer>().material = material;
	}

	// Token: 0x060049C6 RID: 18886 RVA: 0x001612CC File Offset: 0x0015F4CC
	private void SetupMaterial()
	{
		Material material = this.m_FrontPageGameObject.GetComponent<Renderer>().material;
		material.mainTexture = this.m_TempRenderBuffer;
		material.SetTexture("_MaskTex", this.m_TempMaskBuffer);
		material.renderQueue = 3001;
		Material material2 = this.m_BackPageGameObject.GetComponent<Renderer>().material;
		material2.SetTexture("_MaskTex", this.m_TempMaskBuffer);
		material2.renderQueue = 3002;
	}

	// Token: 0x060049C7 RID: 18887 RVA: 0x0016133F File Offset: 0x0015F53F
	private void Show(bool show)
	{
		base.transform.position = ((!show) ? (Vector3.right * this.m_RenderOffset) : this.m_initialPosition);
	}

	// Token: 0x060049C8 RID: 18888 RVA: 0x00161370 File Offset: 0x0015F570
	private IEnumerator WaitThenCompletePageTurn(PageTurn.PageTurningData pageTurningData)
	{
		yield return new WaitForSeconds(pageTurningData.m_secondsToWait);
		Time.captureFramerate = 0;
		this.Show(false);
		if (pageTurningData.m_callback == null)
		{
			yield break;
		}
		pageTurningData.m_callback(pageTurningData.m_callbackData);
		yield break;
	}

	// Token: 0x060049C9 RID: 18889 RVA: 0x0016139C File Offset: 0x0015F59C
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

	// Token: 0x040030E7 RID: 12519
	private readonly string FRONT_PAGE_NAME = "PageTurnFront";

	// Token: 0x040030E8 RID: 12520
	private readonly string BACK_PAGE_NAME = "PageTurnBack";

	// Token: 0x040030E9 RID: 12521
	private readonly string WAIT_THEN_COMPLETE_PAGE_TURN_COROUTINE = "WaitThenCompletePageTurn";

	// Token: 0x040030EA RID: 12522
	private readonly string PAGE_TURN_LEFT_ANIM = "PageTurnLeft";

	// Token: 0x040030EB RID: 12523
	private readonly string PAGE_TURN_RIGHT_ANIM = "PageTurnRight";

	// Token: 0x040030EC RID: 12524
	private readonly string PAGE_TURN_MAT_ANIM = "PageTurnMaterialAnimation";

	// Token: 0x040030ED RID: 12525
	public Shader m_MaskShader;

	// Token: 0x040030EE RID: 12526
	public float m_TurnLeftSpeed = 1.65f;

	// Token: 0x040030EF RID: 12527
	public float m_TurnRightSpeed = 1.65f;

	// Token: 0x040030F0 RID: 12528
	private Bounds m_RenderBounds;

	// Token: 0x040030F1 RID: 12529
	private Camera m_OffscreenPageTurnCamera;

	// Token: 0x040030F2 RID: 12530
	private Camera m_OffscreenPageTurnMaskCamera;

	// Token: 0x040030F3 RID: 12531
	private GameObject m_OffscreenPageTurnCameraGO;

	// Token: 0x040030F4 RID: 12532
	private RenderTexture m_TempRenderBuffer;

	// Token: 0x040030F5 RID: 12533
	private RenderTexture m_TempMaskBuffer;

	// Token: 0x040030F6 RID: 12534
	private GameObject m_MeshGameObject;

	// Token: 0x040030F7 RID: 12535
	private GameObject m_FrontPageGameObject;

	// Token: 0x040030F8 RID: 12536
	private GameObject m_BackPageGameObject;

	// Token: 0x040030F9 RID: 12537
	private GameObject m_TheBoxOuterFrame;

	// Token: 0x040030FA RID: 12538
	private float m_RenderOffset = 500f;

	// Token: 0x040030FB RID: 12539
	private Vector3 m_initialPosition;

	// Token: 0x02000709 RID: 1801
	// (Invoke) Token: 0x060049F1 RID: 18929
	public delegate void DelOnPageTurnComplete(object callbackData);

	// Token: 0x020007B1 RID: 1969
	private class PageTurningData
	{
		// Token: 0x04003499 RID: 13465
		public float m_secondsToWait;

		// Token: 0x0400349A RID: 13466
		public PageTurn.DelOnPageTurnComplete m_callback;

		// Token: 0x0400349B RID: 13467
		public object m_callbackData;

		// Token: 0x0400349C RID: 13468
		public AnimationState m_animation;
	}

	// Token: 0x020007B2 RID: 1970
	private class TurnPageData
	{
		// Token: 0x0400349D RID: 13469
		public GameObject flippingPage;

		// Token: 0x0400349E RID: 13470
		public GameObject otherPage;

		// Token: 0x0400349F RID: 13471
		public PageTurn.DelOnPageTurnComplete callback;

		// Token: 0x040034A0 RID: 13472
		public object callbackData;
	}
}
