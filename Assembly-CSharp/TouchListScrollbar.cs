using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000580 RID: 1408
public class TouchListScrollbar : PegUIElement
{
	// Token: 0x06004000 RID: 16384 RVA: 0x001364C4 File Offset: 0x001346C4
	protected override void Awake()
	{
		if (this.list.orientation == TouchList.Orientation.Horizontal)
		{
			Debug.LogError("Horizontal TouchListScrollbar not implemented");
			Object.Destroy(this);
			return;
		}
		base.Awake();
		this.ShowThumb(this.isActive);
		this.list.ClipSizeChanged += new Action(this.UpdateLayout);
		this.list.ScrollingEnabledChanged += this.UpdateActive;
		this.list.Scrolled += new Action(this.UpdateThumb);
		this.thumb.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ThumbPressed));
		this.track.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.TrackPressed));
		this.UpdateLayout();
	}

	// Token: 0x06004001 RID: 16385 RVA: 0x00136584 File Offset: 0x00134784
	private void UpdateActive(bool canScroll)
	{
		if (this.isActive == canScroll)
		{
			return;
		}
		this.isActive = canScroll;
		this.thumb.GetComponent<Collider>().enabled = this.isActive;
		if (this.isActive)
		{
			this.UpdateThumb();
		}
		this.ShowThumb(this.isActive);
	}

	// Token: 0x06004002 RID: 16386 RVA: 0x001365D8 File Offset: 0x001347D8
	private void UpdateLayout()
	{
		TransformUtil.SetPosX(this.thumb, this.thumbMin.position.x);
		this.UpdateThumb();
	}

	// Token: 0x06004003 RID: 16387 RVA: 0x0013660C File Offset: 0x0013480C
	private void ShowThumb(bool show)
	{
		Transform transform = this.thumb.transform.FindChild("Mesh");
		if (transform != null)
		{
			transform.gameObject.SetActive(show);
		}
		if (this.cover != null)
		{
			this.cover.SetActive(!show);
		}
	}

	// Token: 0x06004004 RID: 16388 RVA: 0x00136668 File Offset: 0x00134868
	private void UpdateThumb()
	{
		if (!this.isActive)
		{
			return;
		}
		TransformUtil.SetPosZ(this.thumb, base.GetComponent<Collider>().bounds.min.z);
		float scrollValue = this.list.ScrollValue;
		float y = this.thumbMin.position.y + (this.thumbMax.position.y - this.thumbMin.position.y) * Mathf.Clamp01(scrollValue);
		TransformUtil.SetPosY(this.thumb, y);
		this.thumb.transform.localScale = Vector3.one;
		if (scrollValue < 0f || scrollValue > 1f)
		{
			float num = 1f / ((scrollValue >= 0f) ? scrollValue : (-scrollValue + 1f));
			float y2 = ((scrollValue >= 0f) ? this.thumb.GetComponent<Collider>().bounds.min : this.thumb.GetComponent<Collider>().bounds.max).y;
			float num2 = (this.thumb.transform.position.y - y2) * (num - 1f);
			TransformUtil.SetPosY(this.thumb, this.thumb.transform.position.y + num2);
		}
	}

	// Token: 0x06004005 RID: 16389 RVA: 0x001367EA File Offset: 0x001349EA
	private void ThumbPressed(UIEvent e)
	{
		base.StartCoroutine(this.UpdateThumbDrag());
	}

	// Token: 0x06004006 RID: 16390 RVA: 0x001367FC File Offset: 0x001349FC
	private void TrackPressed(UIEvent e)
	{
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		Plane dragPlane;
		dragPlane..ctor(-camera.transform.forward, this.track.transform.position);
		float num = this.GetTouchPoint(dragPlane, camera).y;
		num = Mathf.Clamp(num, this.thumbMax.position.y, this.thumbMin.position.y);
		this.list.ScrollValue = (num - this.thumbMin.position.y) / (this.thumbMax.position.y - this.thumbMin.position.y);
	}

	// Token: 0x06004007 RID: 16391 RVA: 0x001368CC File Offset: 0x00134ACC
	private IEnumerator UpdateThumbDrag()
	{
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		Plane dragPlane = new Plane(-camera.transform.forward, this.thumb.transform.position);
		float dragOffset = (this.thumb.transform.position - this.GetTouchPoint(dragPlane, camera)).y;
		while (!UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			float thumbPosY = this.GetTouchPoint(dragPlane, camera).y + dragOffset;
			thumbPosY = Mathf.Clamp(thumbPosY, this.thumbMax.position.y, this.thumbMin.position.y);
			this.list.ScrollValue = (thumbPosY - this.thumbMin.position.y) / (this.thumbMax.position.y - this.thumbMin.position.y);
			yield return null;
		}
		yield break;
	}

	// Token: 0x06004008 RID: 16392 RVA: 0x001368E8 File Offset: 0x00134AE8
	private Vector3 GetTouchPoint(Plane dragPlane, Camera camera)
	{
		Ray ray = camera.ScreenPointToRay(UniversalInputManager.Get().GetMousePosition());
		float num;
		dragPlane.Raycast(ray, ref num);
		return ray.GetPoint(num);
	}

	// Token: 0x040028FC RID: 10492
	public TouchList list;

	// Token: 0x040028FD RID: 10493
	public PegUIElement thumb;

	// Token: 0x040028FE RID: 10494
	public Transform thumbMin;

	// Token: 0x040028FF RID: 10495
	public Transform thumbMax;

	// Token: 0x04002900 RID: 10496
	public GameObject cover;

	// Token: 0x04002901 RID: 10497
	public PegUIElement track;

	// Token: 0x04002902 RID: 10498
	private bool isActive;
}
