using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000181 RID: 385
public class OverlayUI : MonoBehaviour
{
	// Token: 0x0600161E RID: 5662 RVA: 0x00068FD0 File Offset: 0x000671D0
	private void Awake()
	{
		OverlayUI.s_instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneChange));
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x0006901C File Offset: 0x0006721C
	private void Start()
	{
		Log.Cameron.Print("loading overlay ui", new object[0]);
		CanvasScaler componentInChildren = base.gameObject.GetComponentInChildren<CanvasScaler>();
		Log.Cameron.Print("canvas scaler component " + componentInChildren, new object[0]);
		if (componentInChildren != null)
		{
			Log.Cameron.Print("canvas scaler values " + componentInChildren.referenceResolution, new object[0]);
			Log.Cameron.Print("object scale values " + componentInChildren.gameObject.transform.localScale, new object[0]);
		}
	}

	// Token: 0x06001620 RID: 5664 RVA: 0x000690C5 File Offset: 0x000672C5
	private void OnDestroy()
	{
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		OverlayUI.s_instance = null;
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x000690E3 File Offset: 0x000672E3
	public static OverlayUI Get()
	{
		return OverlayUI.s_instance;
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x000690EC File Offset: 0x000672EC
	public void AddGameObject(GameObject go, CanvasAnchor anchor = CanvasAnchor.CENTER, bool destroyOnSceneLoad = false, CanvasScaleMode scaleMode = CanvasScaleMode.HEIGHT)
	{
		CanvasAnchors canvasAnchors = (scaleMode != CanvasScaleMode.HEIGHT) ? this.m_widthScale : this.m_heightScale;
		TransformUtil.AttachAndPreserveLocalTransform(go.transform, canvasAnchors.GetAnchor(anchor));
		if (destroyOnSceneLoad)
		{
			this.DestroyOnSceneLoad(go);
		}
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x00069134 File Offset: 0x00067334
	public Vector3 GetRelativePosition(Vector3 worldPosition, Camera camera = null, Transform bone = null, float depth = 0f)
	{
		if (camera == null)
		{
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
			{
				camera = BoardCameras.Get().GetComponentInChildren<Camera>();
			}
			else
			{
				camera = Box.Get().GetBoxCamera().GetComponent<Camera>();
			}
		}
		if (bone == null)
		{
			bone = this.m_heightScale.m_Center;
		}
		Vector3 vector = camera.WorldToScreenPoint(worldPosition);
		Vector3 vector2 = this.m_UICamera.ScreenToWorldPoint(vector);
		vector2.y = depth;
		return bone.InverseTransformPoint(vector2);
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x000691BD File Offset: 0x000673BD
	public void DestroyOnSceneLoad(GameObject go)
	{
		if (!this.m_destroyOnSceneLoad.Contains(go))
		{
			this.m_destroyOnSceneLoad.Add(go);
		}
	}

	// Token: 0x06001625 RID: 5669 RVA: 0x000691DD File Offset: 0x000673DD
	public void DontDestroyOnSceneLoad(GameObject go)
	{
		if (this.m_destroyOnSceneLoad.Contains(go))
		{
			this.m_destroyOnSceneLoad.Remove(go);
		}
	}

	// Token: 0x06001626 RID: 5670 RVA: 0x000691FD File Offset: 0x000673FD
	private void OnSceneChange(SceneMgr.Mode mode, Scene scene, object userData)
	{
		this.m_destroyOnSceneLoad.RemoveWhere(delegate(GameObject go)
		{
			if (go != null)
			{
				Log.Cameron.Print("destroying go " + go.name, new object[0]);
				Object.Destroy(go);
				return true;
			}
			return false;
		});
	}

	// Token: 0x06001627 RID: 5671 RVA: 0x00069228 File Offset: 0x00067428
	private void WillReset()
	{
		this.m_widthScale.WillReset();
		this.m_heightScale.WillReset();
	}

	// Token: 0x04000B06 RID: 2822
	public CanvasAnchors m_heightScale;

	// Token: 0x04000B07 RID: 2823
	public CanvasAnchors m_widthScale;

	// Token: 0x04000B08 RID: 2824
	public Camera m_UICamera;

	// Token: 0x04000B09 RID: 2825
	public Camera m_PerspectiveUICamera;

	// Token: 0x04000B0A RID: 2826
	private static OverlayUI s_instance;

	// Token: 0x04000B0B RID: 2827
	private HashSet<GameObject> m_destroyOnSceneLoad = new HashSet<GameObject>();
}
