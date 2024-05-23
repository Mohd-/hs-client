using System;
using UnityEngine;

// Token: 0x02000520 RID: 1312
public class ScrollbarControl : MonoBehaviour
{
	// Token: 0x06003D01 RID: 15617 RVA: 0x00126E3C File Offset: 0x0012503C
	private void Awake()
	{
		this.m_PressElement.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnPressElementPress));
		this.m_PressElement.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.OnPressElementReleaseAll));
		this.m_DragCollider.enabled = false;
	}

	// Token: 0x06003D02 RID: 15618 RVA: 0x00126E87 File Offset: 0x00125087
	private void Update()
	{
		this.UpdateDrag();
	}

	// Token: 0x06003D03 RID: 15619 RVA: 0x00126E8F File Offset: 0x0012508F
	private void OnDestroy()
	{
		if (UniversalInputManager.Get() != null)
		{
			UniversalInputManager.Get().UnregisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
		}
	}

	// Token: 0x06003D04 RID: 15620 RVA: 0x00126EB8 File Offset: 0x001250B8
	public float GetValue()
	{
		return this.m_thumbUnitPos;
	}

	// Token: 0x06003D05 RID: 15621 RVA: 0x00126EC0 File Offset: 0x001250C0
	public void SetValue(float val)
	{
		this.m_thumbUnitPos = Mathf.Clamp01(val);
		this.m_prevThumbUnitPos = this.m_thumbUnitPos;
		this.UpdateThumb();
	}

	// Token: 0x06003D06 RID: 15622 RVA: 0x00126EE0 File Offset: 0x001250E0
	public ScrollbarControl.UpdateHandler GetUpdateHandler()
	{
		return this.m_updateHandler;
	}

	// Token: 0x06003D07 RID: 15623 RVA: 0x00126EE8 File Offset: 0x001250E8
	public void SetUpdateHandler(ScrollbarControl.UpdateHandler handler)
	{
		this.m_updateHandler = handler;
	}

	// Token: 0x06003D08 RID: 15624 RVA: 0x00126EF1 File Offset: 0x001250F1
	public ScrollbarControl.FinishHandler GetFinishHandler()
	{
		return this.m_finishHandler;
	}

	// Token: 0x06003D09 RID: 15625 RVA: 0x00126EF9 File Offset: 0x001250F9
	public void SetFinishHandler(ScrollbarControl.FinishHandler handler)
	{
		this.m_finishHandler = handler;
	}

	// Token: 0x06003D0A RID: 15626 RVA: 0x00126F02 File Offset: 0x00125102
	private void OnPressElementPress(UIEvent e)
	{
		this.HandlePress();
	}

	// Token: 0x06003D0B RID: 15627 RVA: 0x00126F0A File Offset: 0x0012510A
	private void OnPressElementReleaseAll(UIEvent e)
	{
		this.HandleRelease();
		this.FireFinishEvent();
	}

	// Token: 0x06003D0C RID: 15628 RVA: 0x00126F18 File Offset: 0x00125118
	private void OnMouseOnOrOffScreen(bool onScreen)
	{
		if (onScreen)
		{
			return;
		}
		this.HandleOutOfBounds();
	}

	// Token: 0x06003D0D RID: 15629 RVA: 0x00126F28 File Offset: 0x00125128
	private void UpdateDrag()
	{
		if (!this.m_dragging)
		{
			return;
		}
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(1 << this.m_DragCollider.gameObject.layer, out raycastHit) && raycastHit.collider == this.m_DragCollider)
		{
			float x = this.m_LeftBone.position.x;
			float x2 = this.m_RightBone.position.x;
			float num = x2 - x;
			this.m_thumbUnitPos = Mathf.Clamp01((raycastHit.point.x - x) / num);
			this.UpdateThumb();
			this.HandleThumbUpdate();
		}
		else
		{
			this.m_thumbUnitPos = this.m_prevThumbUnitPos;
			this.HandleOutOfBounds();
		}
	}

	// Token: 0x06003D0E RID: 15630 RVA: 0x00126FF4 File Offset: 0x001251F4
	private void UpdateThumb()
	{
		this.m_Thumb.transform.position = Vector3.Lerp(this.m_LeftBone.position, this.m_RightBone.position, this.m_thumbUnitPos);
	}

	// Token: 0x06003D0F RID: 15631 RVA: 0x00127034 File Offset: 0x00125234
	private void HandlePress()
	{
		this.m_dragging = true;
		UniversalInputManager.Get().RegisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
		this.m_PressElement.AddEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.OnPressElementReleaseAll));
		this.m_PressElement.GetComponent<Collider>().enabled = false;
		this.m_DragCollider.enabled = true;
	}

	// Token: 0x06003D10 RID: 15632 RVA: 0x00127098 File Offset: 0x00125298
	private void HandleRelease()
	{
		this.m_DragCollider.enabled = false;
		this.m_PressElement.GetComponent<Collider>().enabled = true;
		this.m_PressElement.RemoveEventListener(UIEventType.RELEASEALL, new UIEvent.Handler(this.OnPressElementReleaseAll));
		UniversalInputManager.Get().UnregisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
		this.m_dragging = false;
	}

	// Token: 0x06003D11 RID: 15633 RVA: 0x001270FC File Offset: 0x001252FC
	private void HandleThumbUpdate()
	{
		float prevThumbUnitPos = this.m_prevThumbUnitPos;
		this.m_prevThumbUnitPos = this.m_thumbUnitPos;
		if (object.Equals(this.m_thumbUnitPos, prevThumbUnitPos))
		{
			return;
		}
		this.FireUpdateEvent();
	}

	// Token: 0x06003D12 RID: 15634 RVA: 0x0012713E File Offset: 0x0012533E
	private void HandleOutOfBounds()
	{
		this.UpdateThumb();
		this.HandleThumbUpdate();
		this.HandleRelease();
		this.FireFinishEvent();
	}

	// Token: 0x06003D13 RID: 15635 RVA: 0x00127158 File Offset: 0x00125358
	private void FireUpdateEvent()
	{
		if (this.m_updateHandler == null)
		{
			return;
		}
		this.m_updateHandler(this.GetValue());
	}

	// Token: 0x06003D14 RID: 15636 RVA: 0x00127177 File Offset: 0x00125377
	private void FireFinishEvent()
	{
		if (this.m_finishHandler == null)
		{
			return;
		}
		this.m_finishHandler();
	}

	// Token: 0x040026D0 RID: 9936
	public GameObject m_Thumb;

	// Token: 0x040026D1 RID: 9937
	public PegUIElement m_PressElement;

	// Token: 0x040026D2 RID: 9938
	public Collider m_DragCollider;

	// Token: 0x040026D3 RID: 9939
	public Transform m_LeftBone;

	// Token: 0x040026D4 RID: 9940
	public Transform m_RightBone;

	// Token: 0x040026D5 RID: 9941
	private bool m_dragging;

	// Token: 0x040026D6 RID: 9942
	private float m_thumbUnitPos;

	// Token: 0x040026D7 RID: 9943
	private float m_prevThumbUnitPos;

	// Token: 0x040026D8 RID: 9944
	private ScrollbarControl.UpdateHandler m_updateHandler;

	// Token: 0x040026D9 RID: 9945
	private ScrollbarControl.FinishHandler m_finishHandler;

	// Token: 0x02000524 RID: 1316
	// (Invoke) Token: 0x06003D2F RID: 15663
	public delegate void UpdateHandler(float val);

	// Token: 0x02000525 RID: 1317
	// (Invoke) Token: 0x06003D33 RID: 15667
	public delegate void FinishHandler();
}
