using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002A1 RID: 673
[CustomEditClass]
public class UIBScrollable : PegUICustomBehavior
{
	// Token: 0x1700035D RID: 861
	// (get) Token: 0x06002458 RID: 9304 RVA: 0x000B2685 File Offset: 0x000B0885
	// (set) Token: 0x06002459 RID: 9305 RVA: 0x000B268D File Offset: 0x000B088D
	[CustomEditField(Sections = "Scroll")]
	public float ScrollValue
	{
		get
		{
			return this.m_ScrollValue;
		}
		set
		{
			if (Application.isEditor)
			{
				return;
			}
			this.SetScroll(value, false, false);
		}
	}

	// Token: 0x0600245A RID: 9306 RVA: 0x000B26A3 File Offset: 0x000B08A3
	public static void DefaultVisibleAffectedCallback(GameObject obj, bool visible)
	{
		if (obj.activeSelf != visible)
		{
			obj.SetActive(visible);
		}
	}

	// Token: 0x0600245B RID: 9307 RVA: 0x000B26B8 File Offset: 0x000B08B8
	protected override void Awake()
	{
		this.ResetScrollStartPosition();
		if (this.m_ScrollTrack != null && !UniversalInputManager.UsePhoneUI)
		{
			PegUIElement component = this.m_ScrollTrack.GetComponent<PegUIElement>();
			if (component != null)
			{
				component.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
				{
					this.StartDragging();
				});
			}
		}
		if (this.m_OverridePegUI)
		{
			base.Awake();
		}
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x000B2728 File Offset: 0x000B0928
	protected override void OnDestroy()
	{
		if (this.m_OverridePegUI)
		{
			base.OnDestroy();
		}
	}

	// Token: 0x0600245D RID: 9309 RVA: 0x000B273C File Offset: 0x000B093C
	private void Update()
	{
		this.UpdateScroll();
		if (!this.m_Enabled || this.m_InputBlocked || this.m_Pause)
		{
			return;
		}
		bool flag = (!this.m_ForceScrollAreaHitTest) ? UniversalInputManager.Get().InputIsOver(this.GetScrollCamera(), this.m_ScrollBounds.gameObject) : UniversalInputManager.Get().ForcedInputIsOver(this.GetScrollCamera(), this.m_ScrollBounds.gameObject);
		if (flag)
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis != 0f)
			{
				Log.JMac.Print("Scroll distance: " + axis, new object[0]);
				float num = this.m_ScrollWheelAmount * 10f;
				this.AddScroll(0f - axis * num);
			}
		}
		if (this.m_ScrollThumb != null && this.m_ScrollThumb.IsDragging())
		{
			this.Drag();
		}
		else if (UniversalInputManager.Get().IsTouchMode())
		{
			this.UpdateTouch();
		}
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x000B2851 File Offset: 0x000B0A51
	public override bool UpdateUI()
	{
		return this.IsTouchDragging() && this.m_Enabled;
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x000B2867 File Offset: 0x000B0A67
	public void ResetScrollStartPosition()
	{
		if (this.m_ScrollObject != null)
		{
			this.m_ScrollAreaStartPos = this.m_ScrollObject.transform.localPosition;
		}
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x000B2890 File Offset: 0x000B0A90
	public void ResetScrollStartPosition(Vector3 position)
	{
		if (this.m_ScrollObject != null)
		{
			this.m_ScrollAreaStartPos = position;
		}
	}

	// Token: 0x06002461 RID: 9313 RVA: 0x000B28AC File Offset: 0x000B0AAC
	public void AddVisibleAffectedObject(GameObject obj, Vector3 extents, bool visible, UIBScrollable.VisibleAffected callback = null)
	{
		this.m_VisibleAffectedObjects.Add(new UIBScrollable.VisibleAffectedObject
		{
			Obj = obj,
			Extents = extents,
			Visible = visible,
			Callback = ((callback != null) ? callback : new UIBScrollable.VisibleAffected(UIBScrollable.DefaultVisibleAffectedCallback))
		});
	}

	// Token: 0x06002462 RID: 9314 RVA: 0x000B2900 File Offset: 0x000B0B00
	public void RemoveVisibleAffectedObject(GameObject obj, UIBScrollable.VisibleAffected callback)
	{
		this.m_VisibleAffectedObjects.RemoveAll((UIBScrollable.VisibleAffectedObject o) => o.Obj == obj && o.Callback == callback);
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x000B2939 File Offset: 0x000B0B39
	public void ClearVisibleAffectObjects()
	{
		this.m_VisibleAffectedObjects.Clear();
	}

	// Token: 0x06002464 RID: 9316 RVA: 0x000B2946 File Offset: 0x000B0B46
	public void ForceVisibleAffectedObjectsShow(bool show)
	{
		if (this.m_ForceShowVisibleAffectedObjects != show)
		{
			this.m_ForceShowVisibleAffectedObjects = show;
			this.UpdateAndFireVisibleAffectedObjects();
		}
	}

	// Token: 0x06002465 RID: 9317 RVA: 0x000B2961 File Offset: 0x000B0B61
	public void AddEnableScrollListener(UIBScrollable.EnableScroll dlg)
	{
		this.m_EnableScrollListeners.Add(dlg);
	}

	// Token: 0x06002466 RID: 9318 RVA: 0x000B296F File Offset: 0x000B0B6F
	public void RemoveEnableScrollListener(UIBScrollable.EnableScroll dlg)
	{
		this.m_EnableScrollListeners.Remove(dlg);
	}

	// Token: 0x06002467 RID: 9319 RVA: 0x000B297E File Offset: 0x000B0B7E
	public void AddTouchScrollStartedListener(UIBScrollable.OnTouchScrollStarted dlg)
	{
		this.m_TouchScrollStartedListeners.Add(dlg);
	}

	// Token: 0x06002468 RID: 9320 RVA: 0x000B298C File Offset: 0x000B0B8C
	public void RemoveTouchScrollStartedListener(UIBScrollable.OnTouchScrollStarted dlg)
	{
		this.m_TouchScrollStartedListeners.Remove(dlg);
	}

	// Token: 0x06002469 RID: 9321 RVA: 0x000B299B File Offset: 0x000B0B9B
	public void AddTouchScrollEndedListener(UIBScrollable.OnTouchScrollEnded dlg)
	{
		this.m_TouchScrollEndedListeners.Add(dlg);
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x000B29A9 File Offset: 0x000B0BA9
	public void RemoveTouchScrollEndedListener(UIBScrollable.OnTouchScrollEnded dlg)
	{
		this.m_TouchScrollEndedListeners.Remove(dlg);
	}

	// Token: 0x0600246B RID: 9323 RVA: 0x000B29B8 File Offset: 0x000B0BB8
	public void Pause(bool pause)
	{
		this.m_Pause = pause;
	}

	// Token: 0x0600246C RID: 9324 RVA: 0x000B29C4 File Offset: 0x000B0BC4
	public void Enable(bool enable)
	{
		if (this.m_Enabled == enable)
		{
			return;
		}
		if (this.m_ScrollThumb != null)
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				this.m_ScrollThumb.gameObject.SetActive(false);
			}
			else
			{
				this.m_ScrollThumb.gameObject.SetActive(!this.m_HideThumbWhenDisabled || enable);
			}
		}
		if (this.m_scrollTrackCover != null)
		{
			this.m_scrollTrackCover.SetActive(!enable);
		}
		this.m_Enabled = enable;
		if (this.m_Enabled)
		{
			this.ResetTouchDrag();
		}
		this.FireEnableScrollEvent();
	}

	// Token: 0x0600246D RID: 9325 RVA: 0x000B2A74 File Offset: 0x000B0C74
	public bool IsEnabled()
	{
		return this.m_Enabled;
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x000B2A7C File Offset: 0x000B0C7C
	public bool IsEnabledAndScrollable()
	{
		return this.m_Enabled && this.IsScrollNeeded();
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x000B2A92 File Offset: 0x000B0C92
	public float GetScroll()
	{
		return this.m_ScrollValue;
	}

	// Token: 0x06002470 RID: 9328 RVA: 0x000B2A9A File Offset: 0x000B0C9A
	public void SaveScroll(string savedName)
	{
		UIBScrollable.s_SavedScrollValues[savedName] = this.m_ScrollValue;
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x000B2AB0 File Offset: 0x000B0CB0
	public void LoadScroll(string savedName)
	{
		float percentage = 0f;
		if (UIBScrollable.s_SavedScrollValues.TryGetValue(savedName, out percentage))
		{
			this.SetScroll(percentage, false, true);
			this.ResetTouchDrag();
		}
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x000B2AE4 File Offset: 0x000B0CE4
	public bool EnableIfNeeded()
	{
		bool flag = this.IsScrollNeeded();
		this.Enable(flag);
		return flag;
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x000B2B00 File Offset: 0x000B0D00
	public bool IsScrollNeeded()
	{
		float totalScrollHeight = this.GetTotalScrollHeight();
		return totalScrollHeight > 0f;
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x000B2B1C File Offset: 0x000B0D1C
	public float PollScrollHeight()
	{
		UIBScrollable.HeightMode heightMode = this.m_HeightMode;
		if (heightMode == UIBScrollable.HeightMode.UseHeightCallback)
		{
			return (this.m_ScrollHeightCallback == null) ? this.m_PolledScrollHeight : this.m_ScrollHeightCallback();
		}
		if (heightMode != UIBScrollable.HeightMode.UseScrollableItem)
		{
			return 0f;
		}
		return this.GetScrollableItemsHeight();
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000B2B70 File Offset: 0x000B0D70
	public void SetScroll(float percentage, bool blockInputWhileScrolling = false, bool clamp = true)
	{
		this.SetScroll(percentage, null, blockInputWhileScrolling, clamp);
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x000B2B7C File Offset: 0x000B0D7C
	public void SetScroll(float percentage, iTween.EaseType tweenType, float tweenTime, bool blockInputWhileScrolling = false, bool clamp = true)
	{
		this.SetScroll(percentage, null, tweenType, tweenTime, blockInputWhileScrolling, clamp);
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x000B2B8C File Offset: 0x000B0D8C
	public void SetScrollSnap(float percentage, bool clamp = true)
	{
		this.SetScrollSnap(percentage, null, clamp);
	}

	// Token: 0x06002478 RID: 9336 RVA: 0x000B2B98 File Offset: 0x000B0D98
	public void SetScroll(float percentage, UIBScrollable.OnScrollComplete scrollComplete, bool blockInputWhileScrolling = false, bool clamp = true)
	{
		base.StartCoroutine(this.SetScrollWait(percentage, scrollComplete, blockInputWhileScrolling, true, default(iTween.EaseType?), default(float?), clamp));
	}

	// Token: 0x06002479 RID: 9337 RVA: 0x000B2BCC File Offset: 0x000B0DCC
	public void SetScroll(float percentage, UIBScrollable.OnScrollComplete scrollComplete, iTween.EaseType tweenType, float tweenTime, bool blockInputWhileScrolling = false, bool clamp = true)
	{
		base.StartCoroutine(this.SetScrollWait(percentage, scrollComplete, blockInputWhileScrolling, true, new iTween.EaseType?(tweenType), new float?(tweenTime), clamp));
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x000B2BFC File Offset: 0x000B0DFC
	public void SetScrollSnap(float percentage, UIBScrollable.OnScrollComplete scrollComplete, bool clamp = true)
	{
		this.m_PolledScrollHeight = this.PollScrollHeight();
		this.m_LastScrollHeightRecorded = this.m_PolledScrollHeight;
		this.ScrollTo(percentage, scrollComplete, false, false, default(iTween.EaseType?), default(float?), clamp);
		this.ResetTouchDrag();
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x000B2C44 File Offset: 0x000B0E44
	public void SetScrollHeightCallback(UIBScrollable.ScrollHeightCallback dlg, bool refresh = false, bool resetScroll = false)
	{
		float? setResetScroll = default(float?);
		if (resetScroll)
		{
			setResetScroll = new float?(0f);
		}
		this.SetScrollHeightCallback(dlg, setResetScroll, refresh);
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x000B2C78 File Offset: 0x000B0E78
	public void SetScrollHeightCallback(UIBScrollable.ScrollHeightCallback dlg, float? setResetScroll, bool refresh = false)
	{
		this.m_VisibleAffectedObjects.Clear();
		this.m_ScrollHeightCallback = dlg;
		if (setResetScroll != null)
		{
			this.m_ScrollValue = setResetScroll.Value;
			this.ResetTouchDrag();
		}
		if (refresh)
		{
			this.UpdateScroll();
			this.UpdateThumbPosition();
			this.UpdateScrollObjectPosition(true, null, default(iTween.EaseType?), default(float?), false);
		}
		this.m_PolledScrollHeight = this.PollScrollHeight();
		this.m_LastScrollHeightRecorded = this.m_PolledScrollHeight;
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x000B2CFB File Offset: 0x000B0EFB
	public void SetHeight(float height)
	{
		this.m_ScrollHeightCallback = null;
		this.m_PolledScrollHeight = height;
		this.UpdateHeight();
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x000B2D11 File Offset: 0x000B0F11
	public void UpdateScroll()
	{
		this.m_PolledScrollHeight = this.PollScrollHeight();
		this.UpdateHeight();
	}

	// Token: 0x0600247F RID: 9343 RVA: 0x000B2D28 File Offset: 0x000B0F28
	public void CenterWorldPosition(Vector3 position)
	{
		float percentage = this.m_ScrollObject.transform.InverseTransformPoint(position)[(int)this.m_ScrollPlane] / -(this.m_PolledScrollHeight + this.m_ScrollBottomPadding) * 2f - 0.5f;
		base.StartCoroutine(this.BlockInput(this.m_ScrollTweenTime));
		this.SetScroll(percentage, false, true);
	}

	// Token: 0x06002480 RID: 9344 RVA: 0x000B2D8C File Offset: 0x000B0F8C
	public bool IsObjectVisibleInScrollArea(GameObject obj, Vector3 extents)
	{
		int scrollPlane = (int)this.m_ScrollPlane;
		float num = obj.transform.position[scrollPlane] - extents[scrollPlane];
		float num2 = obj.transform.position[scrollPlane] + extents[scrollPlane];
		Bounds bounds = this.m_ScrollBounds.bounds;
		float num3 = bounds.min[scrollPlane] - this.m_VisibleObjectThreshold;
		float num4 = bounds.max[scrollPlane] + this.m_VisibleObjectThreshold;
		return (num >= num3 && num <= num4) || (num2 >= num3 && num2 <= num4);
	}

	// Token: 0x06002481 RID: 9345 RVA: 0x000B2E44 File Offset: 0x000B1044
	public bool IsDragging()
	{
		bool result;
		if (!(this.m_ScrollThumb != null) || !this.m_ScrollThumb.IsDragging())
		{
			Vector2? touchBeginScreenPos = this.m_TouchBeginScreenPos;
			result = (touchBeginScreenPos != null);
		}
		else
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x000B2E84 File Offset: 0x000B1084
	public bool IsTouchDragging()
	{
		Vector2? touchBeginScreenPos = this.m_TouchBeginScreenPos;
		if (touchBeginScreenPos == null)
		{
			return false;
		}
		float num = Mathf.Abs(UniversalInputManager.Get().GetMousePosition().y - this.m_TouchBeginScreenPos.Value.y);
		return this.m_ScrollDragThreshold * ((Screen.dpi <= 0f) ? 150f : Screen.dpi) <= num;
	}

	// Token: 0x06002483 RID: 9347 RVA: 0x000B2F00 File Offset: 0x000B1100
	private void StartDragging()
	{
		if (this.m_InputBlocked || this.m_Pause || !this.m_Enabled)
		{
			return;
		}
		this.m_ScrollThumb.StartDragging();
	}

	// Token: 0x06002484 RID: 9348 RVA: 0x000B2F30 File Offset: 0x000B1130
	private void UpdateHeight()
	{
		if (Mathf.Abs(this.m_PolledScrollHeight - this.m_LastScrollHeightRecorded) > 0.001f)
		{
			if (!this.EnableIfNeeded())
			{
				this.m_ScrollValue = 0f;
			}
			this.UpdateThumbPosition();
			this.UpdateScrollObjectPosition(false, null, default(iTween.EaseType?), default(float?), false);
			this.ResetTouchDrag();
		}
		this.m_LastScrollHeightRecorded = this.m_PolledScrollHeight;
	}

	// Token: 0x06002485 RID: 9349 RVA: 0x000B2FA4 File Offset: 0x000B11A4
	private void UpdateTouch()
	{
		if (UniversalInputManager.Get().GetMouseButtonDown(0))
		{
			if (this.GetWorldTouchPosition() != null)
			{
				this.m_TouchBeginScreenPos = new Vector2?(UniversalInputManager.Get().GetMousePosition());
				return;
			}
		}
		else if (UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			this.m_TouchBeginScreenPos = default(Vector2?);
			this.m_TouchDragBeginWorldPos = default(Vector3?);
			this.FireTouchEndEvent();
		}
		Vector3? touchDragBeginWorldPos = this.m_TouchDragBeginWorldPos;
		if (touchDragBeginWorldPos != null)
		{
			Vector3? worldTouchPositionOnDragArea = this.GetWorldTouchPositionOnDragArea();
			if (worldTouchPositionOnDragArea != null)
			{
				int scrollPlane = (int)this.m_ScrollPlane;
				this.m_LastTouchScrollValue = this.m_ScrollValue;
				float worldDelta = worldTouchPositionOnDragArea.Value[scrollPlane] - this.m_TouchDragBeginWorldPos.Value[scrollPlane];
				float scrollValueDelta = this.GetScrollValueDelta(worldDelta);
				float num = this.m_TouchDragBeginScrollValue + scrollValueDelta;
				float num2 = this.GetOutOfBoundsDist(num);
				if (num2 != 0f)
				{
					num2 = Mathf.Log10(Mathf.Abs(num2) + 1f) * Mathf.Sign(num2);
					num = ((num2 >= 0f) ? (num2 + 1f) : num2);
				}
				this.ScrollTo(num, null, false, false, default(iTween.EaseType?), default(float?), false);
			}
		}
		else
		{
			Vector2? touchBeginScreenPos = this.m_TouchBeginScreenPos;
			if (touchBeginScreenPos != null)
			{
				float num3 = Mathf.Abs(UniversalInputManager.Get().GetMousePosition().x - this.m_TouchBeginScreenPos.Value.x);
				float num4 = Mathf.Abs(UniversalInputManager.Get().GetMousePosition().y - this.m_TouchBeginScreenPos.Value.y);
				bool flag = num3 > this.m_DeckTileDragThreshold * ((Screen.dpi <= 0f) ? 150f : Screen.dpi);
				bool flag2 = num4 > this.m_ScrollDragThreshold * ((Screen.dpi <= 0f) ? 150f : Screen.dpi);
				if (flag && (num3 >= num4 || !flag2))
				{
					this.m_TouchBeginScreenPos = default(Vector2?);
				}
				else if (flag2)
				{
					this.m_TouchDragBeginWorldPos = this.GetWorldTouchPositionOnDragArea();
					this.m_TouchDragBeginScrollValue = this.m_ScrollValue;
					this.m_LastTouchScrollValue = this.m_ScrollValue;
					this.FireTouchStartEvent();
				}
			}
			else
			{
				float num5 = (this.m_ScrollValue - this.m_LastTouchScrollValue) / Time.fixedDeltaTime;
				float outOfBoundsDist = this.GetOutOfBoundsDist(this.m_ScrollValue);
				if (outOfBoundsDist != 0f)
				{
					if (Mathf.Abs(outOfBoundsDist) >= this.m_MinOutOfBoundsScrollValue)
					{
						float num6 = -this.m_ScrollBoundsSpringK * outOfBoundsDist - Mathf.Sqrt(4f * this.m_ScrollBoundsSpringK) * num5;
						num5 += num6 * Time.fixedDeltaTime;
						this.m_LastTouchScrollValue = this.m_ScrollValue;
						this.ScrollTo(this.m_ScrollValue + num5 * Time.fixedDeltaTime, null, false, false, default(iTween.EaseType?), default(float?), false);
					}
					if (Mathf.Abs(this.GetOutOfBoundsDist(this.m_ScrollValue)) < this.m_MinOutOfBoundsScrollValue)
					{
						this.ScrollTo(Mathf.Round(this.m_ScrollValue), null, false, false, default(iTween.EaseType?), default(float?), false);
						this.m_LastTouchScrollValue = this.m_ScrollValue;
					}
				}
				else if (this.m_LastTouchScrollValue != this.m_ScrollValue)
				{
					float num7 = Mathf.Sign(num5);
					num5 -= num7 * this.m_KineticScrollFriction * Time.fixedDeltaTime;
					this.m_LastTouchScrollValue = this.m_ScrollValue;
					if (Mathf.Abs(num5) >= this.m_MinKineticScrollSpeed && Mathf.Sign(num5) == num7)
					{
						this.ScrollTo(this.m_ScrollValue + num5 * Time.fixedDeltaTime, null, false, false, default(iTween.EaseType?), default(float?), false);
					}
				}
			}
		}
	}

	// Token: 0x06002486 RID: 9350 RVA: 0x000B33C4 File Offset: 0x000B15C4
	private void Drag()
	{
		Vector3 min = this.m_ScrollTrack.bounds.min;
		Camera camera = CameraUtils.FindFirstByLayer(this.m_ScrollTrack.gameObject.layer);
		Plane plane;
		plane..ctor(-camera.transform.forward, min);
		Ray ray = camera.ScreenPointToRay(UniversalInputManager.Get().GetMousePosition());
		float num;
		if (plane.Raycast(ray, ref num))
		{
			Vector3 point = ray.GetPoint(num);
			float scrollTrackTop1D = this.GetScrollTrackTop1D();
			float scrollTrackBtm1D = this.GetScrollTrackBtm1D();
			float num2 = Mathf.Clamp01((point[(int)this.m_ScrollPlane] - scrollTrackTop1D) / (scrollTrackBtm1D - scrollTrackTop1D));
			if (Mathf.Abs(this.m_ScrollValue - num2) > Mathf.Epsilon)
			{
				this.m_ScrollValue = num2;
				this.UpdateThumbPosition();
				this.UpdateScrollObjectPosition(false, null, default(iTween.EaseType?), default(float?), false);
			}
		}
		this.ResetTouchDrag();
	}

	// Token: 0x06002487 RID: 9351 RVA: 0x000B34B8 File Offset: 0x000B16B8
	private void ResetTouchDrag()
	{
		bool flag = this.m_TouchDragBeginWorldPos != null;
		this.m_TouchBeginScreenPos = default(Vector2?);
		this.m_TouchDragBeginWorldPos = default(Vector3?);
		this.m_TouchDragBeginScrollValue = this.m_ScrollValue;
		this.m_LastTouchScrollValue = this.m_ScrollValue;
		if (flag)
		{
			this.FireTouchEndEvent();
		}
	}

	// Token: 0x06002488 RID: 9352 RVA: 0x000B3514 File Offset: 0x000B1714
	private float GetScrollTrackTop1D()
	{
		return this.GetScrollTrackTop()[(int)this.m_ScrollPlane];
	}

	// Token: 0x06002489 RID: 9353 RVA: 0x000B3538 File Offset: 0x000B1738
	private float GetScrollTrackBtm1D()
	{
		return this.GetScrollTrackBtm()[(int)this.m_ScrollPlane];
	}

	// Token: 0x0600248A RID: 9354 RVA: 0x000B355C File Offset: 0x000B175C
	private Vector3 GetScrollTrackTop()
	{
		return (!(this.m_ScrollTrack == null)) ? this.m_ScrollTrack.bounds.max : Vector3.zero;
	}

	// Token: 0x0600248B RID: 9355 RVA: 0x000B3598 File Offset: 0x000B1798
	private Vector3 GetScrollTrackBtm()
	{
		return (!(this.m_ScrollTrack == null)) ? this.m_ScrollTrack.bounds.min : Vector3.zero;
	}

	// Token: 0x0600248C RID: 9356 RVA: 0x000B35D4 File Offset: 0x000B17D4
	private void AddScroll(float amount)
	{
		this.ScrollTo(this.m_ScrollValue + amount, null, false, true, default(iTween.EaseType?), default(float?), true);
		this.ResetTouchDrag();
	}

	// Token: 0x0600248D RID: 9357 RVA: 0x000B360C File Offset: 0x000B180C
	private void ScrollTo(float percentage, UIBScrollable.OnScrollComplete scrollComplete, bool blockInputWhileScrolling, bool tween, iTween.EaseType? tweenType, float? tweenTime, bool clamp)
	{
		this.m_ScrollValue = ((!clamp) ? percentage : Mathf.Clamp01(percentage));
		this.UpdateThumbPosition();
		this.UpdateScrollObjectPosition(tween, scrollComplete, tweenType, tweenTime, blockInputWhileScrolling);
	}

	// Token: 0x0600248E RID: 9358 RVA: 0x000B3648 File Offset: 0x000B1848
	private void UpdateThumbPosition()
	{
		if (this.m_ScrollThumb == null)
		{
			return;
		}
		Vector3 scrollTrackTop = this.GetScrollTrackTop();
		Vector3 scrollTrackBtm = this.GetScrollTrackBtm();
		float num = scrollTrackTop[(int)this.m_ScrollPlane];
		float num2 = scrollTrackBtm[(int)this.m_ScrollPlane];
		Vector3 position = scrollTrackTop + (scrollTrackBtm - scrollTrackTop) * 0.5f;
		position[(int)this.m_ScrollPlane] = num + (num2 - num) * Mathf.Clamp01(this.m_ScrollValue);
		this.m_ScrollThumb.transform.position = position;
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x000B36DC File Offset: 0x000B18DC
	private void UpdateScrollObjectPosition(bool tween, UIBScrollable.OnScrollComplete scrollComplete, iTween.EaseType? tweenType, float? tweenTime, bool blockInputWhileScrolling = false)
	{
		if (this.m_ScrollObject == null)
		{
			return;
		}
		Vector3 scrollAreaStartPos = this.m_ScrollAreaStartPos;
		Vector3 vector = scrollAreaStartPos;
		Vector3 totalScrollHeightVector = this.GetTotalScrollHeightVector();
		vector += totalScrollHeightVector * ((!this.m_ScrollDirectionReverse) ? 1f : -1f);
		Vector3 vector2 = scrollAreaStartPos + this.m_ScrollValue * (vector - scrollAreaStartPos);
		if (tween)
		{
			iTween.MoveTo(this.m_ScrollObject, iTween.Hash(new object[]
			{
				"position",
				vector2,
				"time",
				(tweenTime == null) ? this.m_ScrollTweenTime : tweenTime.Value,
				"isLocal",
				true,
				"easetype",
				(tweenType == null) ? this.m_ScrollEaseType : tweenType.Value,
				"onupdate",
				delegate(object newVal)
				{
					this.UpdateAndFireVisibleAffectedObjects();
				},
				"oncomplete",
				delegate(object newVal)
				{
					this.UpdateAndFireVisibleAffectedObjects();
					if (scrollComplete != null)
					{
						scrollComplete(this.m_ScrollValue);
					}
				}
			}));
		}
		else
		{
			this.m_ScrollObject.transform.localPosition = vector2;
			this.UpdateAndFireVisibleAffectedObjects();
			if (scrollComplete != null)
			{
				scrollComplete(this.m_ScrollValue);
			}
		}
	}

	// Token: 0x06002490 RID: 9360 RVA: 0x000B386C File Offset: 0x000B1A6C
	private IEnumerator SetScrollWait(float percentage, UIBScrollable.OnScrollComplete scrollComplete, bool blockInputWhileScrolling, bool tween, iTween.EaseType? tweenType, float? tweenTime, bool clamp)
	{
		yield return null;
		this.ScrollTo(percentage, scrollComplete, blockInputWhileScrolling, tween, tweenType, tweenTime, clamp);
		this.ResetTouchDrag();
		yield break;
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x000B38F4 File Offset: 0x000B1AF4
	private IEnumerator BlockInput(float blockTime)
	{
		this.m_InputBlocked = true;
		yield return new WaitForSeconds(blockTime);
		this.m_InputBlocked = false;
		yield break;
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x000B3920 File Offset: 0x000B1B20
	private Vector3 GetTotalScrollHeightVector()
	{
		if (this.m_ScrollObject == null)
		{
			return Vector3.zero;
		}
		float num = this.m_PolledScrollHeight - this.GetScrollBoundsHeight();
		if (num < 0f)
		{
			return Vector3.zero;
		}
		Vector3 vector = Vector3.zero;
		vector[(int)this.m_ScrollPlane] = num;
		vector = this.m_ScrollObject.transform.parent.worldToLocalMatrix * vector;
		if (this.m_ScrollBottomPadding > 0f)
		{
			vector += vector.normalized * this.m_ScrollBottomPadding;
		}
		return vector;
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x000B39C8 File Offset: 0x000B1BC8
	private float GetTotalScrollHeight()
	{
		return this.GetTotalScrollHeightVector().magnitude;
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x000B39E3 File Offset: 0x000B1BE3
	private Vector3? GetWorldTouchPosition()
	{
		return this.GetWorldTouchPosition(this.m_ScrollBounds);
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000B39F4 File Offset: 0x000B1BF4
	private Vector3? GetWorldTouchPositionOnDragArea()
	{
		return this.GetWorldTouchPosition((!(this.m_TouchDragFullArea != null)) ? this.m_ScrollBounds : this.m_TouchDragFullArea);
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x000B3A2C File Offset: 0x000B1C2C
	private Vector3? GetWorldTouchPosition(BoxCollider bounds)
	{
		Camera scrollCamera = this.GetScrollCamera();
		Ray ray = scrollCamera.ScreenPointToRay(UniversalInputManager.Get().GetMousePosition());
		RaycastHit raycastHit;
		foreach (BoxCollider boxCollider in this.m_TouchScrollBlockers)
		{
			if (boxCollider.Raycast(ray, ref raycastHit, 3.4028235E+38f))
			{
				return default(Vector3?);
			}
		}
		if (bounds.Raycast(ray, ref raycastHit, 3.4028235E+38f))
		{
			return new Vector3?(ray.GetPoint(raycastHit.distance));
		}
		return default(Vector3?);
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x000B3AF4 File Offset: 0x000B1CF4
	private float GetScrollValueDelta(float worldDelta)
	{
		return this.m_ScrollDeltaMultiplier * worldDelta / this.GetTotalScrollHeight();
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x000B3B05 File Offset: 0x000B1D05
	private float GetOutOfBoundsDist(float scrollValue)
	{
		if (scrollValue < 0f)
		{
			return scrollValue;
		}
		if (scrollValue > 1f)
		{
			return scrollValue - 1f;
		}
		return 0f;
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x000B3B2C File Offset: 0x000B1D2C
	private void FireEnableScrollEvent()
	{
		UIBScrollable.EnableScroll[] array = this.m_EnableScrollListeners.ToArray();
		foreach (UIBScrollable.EnableScroll enableScroll in array)
		{
			enableScroll(this.m_Enabled);
		}
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000B3B6C File Offset: 0x000B1D6C
	public void UpdateAndFireVisibleAffectedObjects()
	{
		UIBScrollable.VisibleAffectedObject[] array = this.m_VisibleAffectedObjects.ToArray();
		foreach (UIBScrollable.VisibleAffectedObject visibleAffectedObject in array)
		{
			bool flag = this.IsObjectVisibleInScrollArea(visibleAffectedObject.Obj, visibleAffectedObject.Extents) || this.m_ForceShowVisibleAffectedObjects;
			if (flag != visibleAffectedObject.Visible)
			{
				visibleAffectedObject.Visible = flag;
				visibleAffectedObject.Callback(visibleAffectedObject.Obj, flag);
			}
		}
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x000B3BEC File Offset: 0x000B1DEC
	private float GetScrollBoundsHeight()
	{
		if (this.m_ScrollObject == null)
		{
			Debug.LogWarning("No m_ScrollObject set for this UIBScrollable!");
			return 0f;
		}
		return this.m_ScrollBounds.bounds.size[(int)this.m_ScrollPlane];
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x000B3C40 File Offset: 0x000B1E40
	private void FireTouchStartEvent()
	{
		UIBScrollable.OnTouchScrollStarted[] array = this.m_TouchScrollStartedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i]();
		}
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x000B3C78 File Offset: 0x000B1E78
	private void FireTouchEndEvent()
	{
		UIBScrollable.OnTouchScrollEnded[] array = this.m_TouchScrollEndedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i]();
		}
	}

	// Token: 0x0600249E RID: 9374 RVA: 0x000B3CB0 File Offset: 0x000B1EB0
	private float GetScrollableItemsHeight()
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		if (this.GetScrollableItemsMinMax(ref zero, ref zero2) == null)
		{
			return 0f;
		}
		int scrollPlane = (int)this.m_ScrollPlane;
		return zero2[scrollPlane] - zero[scrollPlane];
	}

	// Token: 0x0600249F RID: 9375 RVA: 0x000B3CF8 File Offset: 0x000B1EF8
	private UIBScrollableItem[] GetScrollableItemsMinMax(ref Vector3 min, ref Vector3 max)
	{
		if (this.m_ScrollObject == null)
		{
			return null;
		}
		UIBScrollableItem[] componentsInChildren = this.m_ScrollObject.GetComponentsInChildren<UIBScrollableItem>(true);
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return null;
		}
		min..ctor(float.MaxValue, float.MaxValue, float.MaxValue);
		max..ctor(float.MinValue, float.MinValue, float.MinValue);
		foreach (UIBScrollableItem uibscrollableItem in componentsInChildren)
		{
			if (uibscrollableItem.IsActive())
			{
				Vector3 vector;
				Vector3 vector2;
				uibscrollableItem.GetWorldBounds(out vector, out vector2);
				min.x = Mathf.Min(new float[]
				{
					min.x,
					vector.x,
					vector2.x
				});
				min.y = Mathf.Min(new float[]
				{
					min.y,
					vector.y,
					vector2.y
				});
				min.z = Mathf.Min(new float[]
				{
					min.z,
					vector.z,
					vector2.z
				});
				max.x = Mathf.Max(new float[]
				{
					max.x,
					vector.x,
					vector2.x
				});
				max.y = Mathf.Max(new float[]
				{
					max.y,
					vector.y,
					vector2.y
				});
				max.z = Mathf.Max(new float[]
				{
					max.z,
					vector.z,
					vector2.z
				});
			}
		}
		return componentsInChildren;
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x000B3EA8 File Offset: 0x000B20A8
	private BoxCollider[] GetBoxCollidersMinMax(ref Vector3 min, ref Vector3 max)
	{
		return null;
	}

	// Token: 0x060024A1 RID: 9377 RVA: 0x000B3EAC File Offset: 0x000B20AC
	private Camera GetScrollCamera()
	{
		return (!this.m_UseCameraFromLayer) ? Box.Get().GetCamera() : CameraUtils.FindFirstByLayer(base.gameObject.layer);
	}

	// Token: 0x0400157E RID: 5502
	[CustomEditField(Sections = "Camera Settings")]
	public bool m_UseCameraFromLayer;

	// Token: 0x0400157F RID: 5503
	[CustomEditField(Sections = "Preferences")]
	public float m_ScrollWheelAmount = 0.1f;

	// Token: 0x04001580 RID: 5504
	[CustomEditField(Sections = "Preferences")]
	public float m_ScrollBottomPadding;

	// Token: 0x04001581 RID: 5505
	[CustomEditField(Sections = "Preferences")]
	public iTween.EaseType m_ScrollEaseType = iTween.Defaults.easeType;

	// Token: 0x04001582 RID: 5506
	[CustomEditField(Sections = "Preferences")]
	public float m_ScrollTweenTime = 0.2f;

	// Token: 0x04001583 RID: 5507
	[CustomEditField(Sections = "Preferences")]
	public UIBScrollable.ScrollDirection m_ScrollPlane = UIBScrollable.ScrollDirection.Z;

	// Token: 0x04001584 RID: 5508
	[CustomEditField(Sections = "Preferences")]
	public bool m_ScrollDirectionReverse;

	// Token: 0x04001585 RID: 5509
	[Tooltip("If scrolling is active, all PegUI calls will be suppressed")]
	[CustomEditField(Sections = "Preferences")]
	public bool m_OverridePegUI;

	// Token: 0x04001586 RID: 5510
	[CustomEditField(Sections = "Preferences")]
	public bool m_ForceScrollAreaHitTest;

	// Token: 0x04001587 RID: 5511
	[CustomEditField(Sections = "Bounds Settings")]
	public BoxCollider m_ScrollBounds;

	// Token: 0x04001588 RID: 5512
	[CustomEditField(Sections = "Optional Bounds Settings")]
	public BoxCollider m_TouchDragFullArea;

	// Token: 0x04001589 RID: 5513
	[CustomEditField(Sections = "Thumb Settings")]
	public BoxCollider m_ScrollTrack;

	// Token: 0x0400158A RID: 5514
	[CustomEditField(Sections = "Thumb Settings")]
	public ScrollBarThumb m_ScrollThumb;

	// Token: 0x0400158B RID: 5515
	[CustomEditField(Sections = "Thumb Settings")]
	public bool m_HideThumbWhenDisabled;

	// Token: 0x0400158C RID: 5516
	[CustomEditField(Sections = "Thumb Settings")]
	public GameObject m_scrollTrackCover;

	// Token: 0x0400158D RID: 5517
	[CustomEditField(Sections = "Bounds Settings")]
	public GameObject m_ScrollObject;

	// Token: 0x0400158E RID: 5518
	[CustomEditField(Sections = "Bounds Settings")]
	public float m_VisibleObjectThreshold;

	// Token: 0x0400158F RID: 5519
	[Tooltip("Drag distance required to initiate deck tile dragging (inches)")]
	[CustomEditField(Sections = "Touch Settings")]
	public float m_DeckTileDragThreshold = 0.04f;

	// Token: 0x04001590 RID: 5520
	[CustomEditField(Sections = "Touch Settings")]
	[Tooltip("Drag distance required to initiate scroll dragging (inches)")]
	public float m_ScrollDragThreshold = 0.04f;

	// Token: 0x04001591 RID: 5521
	[CustomEditField(Sections = "Touch Settings")]
	[Tooltip("Stopping speed for scrolling after the user has let go")]
	public float m_MinKineticScrollSpeed = 0.01f;

	// Token: 0x04001592 RID: 5522
	[Tooltip("Resistance for slowing down scrolling after the user has let go")]
	[CustomEditField(Sections = "Touch Settings")]
	public float m_KineticScrollFriction = 6f;

	// Token: 0x04001593 RID: 5523
	[CustomEditField(Sections = "Touch Settings")]
	[Tooltip("Strength of the boundary springs")]
	public float m_ScrollBoundsSpringK = 700f;

	// Token: 0x04001594 RID: 5524
	[CustomEditField(Sections = "Touch Settings")]
	[Tooltip("Distance at which the out-of-bounds scroll value will snapped to 0 or 1")]
	public float m_MinOutOfBoundsScrollValue = 0.001f;

	// Token: 0x04001595 RID: 5525
	[Tooltip("Use this to match scaling issues.")]
	[CustomEditField(Sections = "Touch Settings")]
	public float m_ScrollDeltaMultiplier = 1f;

	// Token: 0x04001596 RID: 5526
	[CustomEditField(Sections = "Touch Settings")]
	public List<BoxCollider> m_TouchScrollBlockers = new List<BoxCollider>();

	// Token: 0x04001597 RID: 5527
	public UIBScrollable.HeightMode m_HeightMode = UIBScrollable.HeightMode.UseScrollableItem;

	// Token: 0x04001598 RID: 5528
	private bool m_Enabled = true;

	// Token: 0x04001599 RID: 5529
	private float m_ScrollValue;

	// Token: 0x0400159A RID: 5530
	private float m_LastTouchScrollValue;

	// Token: 0x0400159B RID: 5531
	private bool m_InputBlocked;

	// Token: 0x0400159C RID: 5532
	private bool m_Pause;

	// Token: 0x0400159D RID: 5533
	private Vector2? m_TouchBeginScreenPos;

	// Token: 0x0400159E RID: 5534
	private Vector3? m_TouchDragBeginWorldPos;

	// Token: 0x0400159F RID: 5535
	private float m_TouchDragBeginScrollValue;

	// Token: 0x040015A0 RID: 5536
	private Vector3 m_ScrollAreaStartPos;

	// Token: 0x040015A1 RID: 5537
	private UIBScrollable.ScrollHeightCallback m_ScrollHeightCallback;

	// Token: 0x040015A2 RID: 5538
	private List<UIBScrollable.EnableScroll> m_EnableScrollListeners = new List<UIBScrollable.EnableScroll>();

	// Token: 0x040015A3 RID: 5539
	private float m_LastScrollHeightRecorded;

	// Token: 0x040015A4 RID: 5540
	private float m_PolledScrollHeight;

	// Token: 0x040015A5 RID: 5541
	private List<UIBScrollable.VisibleAffectedObject> m_VisibleAffectedObjects = new List<UIBScrollable.VisibleAffectedObject>();

	// Token: 0x040015A6 RID: 5542
	private List<UIBScrollable.OnTouchScrollStarted> m_TouchScrollStartedListeners = new List<UIBScrollable.OnTouchScrollStarted>();

	// Token: 0x040015A7 RID: 5543
	private List<UIBScrollable.OnTouchScrollEnded> m_TouchScrollEndedListeners = new List<UIBScrollable.OnTouchScrollEnded>();

	// Token: 0x040015A8 RID: 5544
	private bool m_ForceShowVisibleAffectedObjects;

	// Token: 0x040015A9 RID: 5545
	private static Map<string, float> s_SavedScrollValues = new Map<string, float>();

	// Token: 0x020002A2 RID: 674
	// (Invoke) Token: 0x060024A4 RID: 9380
	public delegate float ScrollHeightCallback();

	// Token: 0x020002AB RID: 683
	// (Invoke) Token: 0x0600256E RID: 9582
	public delegate void OnTouchScrollStarted();

	// Token: 0x020002AC RID: 684
	// (Invoke) Token: 0x06002572 RID: 9586
	public delegate void OnTouchScrollEnded();

	// Token: 0x020002AD RID: 685
	// (Invoke) Token: 0x06002576 RID: 9590
	public delegate void VisibleAffected(GameObject obj, bool visible);

	// Token: 0x020002AE RID: 686
	// (Invoke) Token: 0x0600257A RID: 9594
	public delegate void OnScrollComplete(float percentage);

	// Token: 0x020002AF RID: 687
	public enum ScrollDirection
	{
		// Token: 0x0400161A RID: 5658
		X,
		// Token: 0x0400161B RID: 5659
		Y,
		// Token: 0x0400161C RID: 5660
		Z
	}

	// Token: 0x020002B0 RID: 688
	public enum HeightMode
	{
		// Token: 0x0400161E RID: 5662
		UseHeightCallback,
		// Token: 0x0400161F RID: 5663
		UseScrollableItem,
		// Token: 0x04001620 RID: 5664
		UseBoxCollider
	}

	// Token: 0x020002B1 RID: 689
	protected class VisibleAffectedObject
	{
		// Token: 0x04001621 RID: 5665
		public GameObject Obj;

		// Token: 0x04001622 RID: 5666
		public Vector3 Extents;

		// Token: 0x04001623 RID: 5667
		public bool Visible;

		// Token: 0x04001624 RID: 5668
		public UIBScrollable.VisibleAffected Callback;
	}

	// Token: 0x020002B2 RID: 690
	// (Invoke) Token: 0x0600257F RID: 9599
	public delegate void EnableScroll(bool enabled);

	// Token: 0x020002B3 RID: 691
	// (Invoke) Token: 0x06002583 RID: 9603
	public delegate void ScrollTurnedOn(bool on);
}
