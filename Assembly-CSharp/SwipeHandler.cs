using System;
using UnityEngine;

// Token: 0x02000F61 RID: 3937
public class SwipeHandler : PegUICustomBehavior
{
	// Token: 0x060074F1 RID: 29937 RVA: 0x00228826 File Offset: 0x00226A26
	public override bool UpdateUI()
	{
		return UniversalInputManager.Get().IsTouchMode() && this.HandleSwipeGesture();
	}

	// Token: 0x060074F2 RID: 29938 RVA: 0x00228840 File Offset: 0x00226A40
	private bool InSwipeRect(Vector2 v)
	{
		return v.x >= this.m_swipeRect.x && v.x <= this.m_swipeRect.x + this.m_swipeRect.width && v.y >= this.m_swipeRect.y && v.y <= this.m_swipeRect.y + this.m_swipeRect.height;
	}

	// Token: 0x060074F3 RID: 29939 RVA: 0x002288C4 File Offset: 0x00226AC4
	private bool CheckSwipe()
	{
		float num = this.m_swipeStart.x - UniversalInputManager.Get().GetMousePosition().x;
		float num2 = 0.09f + ((!(this.m_swipeElement != null)) ? 0f : 0.035f);
		float num3 = (float)Screen.width * num2;
		if (Mathf.Abs(num) > num3)
		{
			SwipeHandler.SWIPE_DIRECTION direction;
			if (num < 0f)
			{
				direction = SwipeHandler.SWIPE_DIRECTION.RIGHT;
			}
			else
			{
				direction = SwipeHandler.SWIPE_DIRECTION.LEFT;
			}
			if (SwipeHandler.m_swipeListener != null)
			{
				SwipeHandler.m_swipeListener(direction);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060074F4 RID: 29940 RVA: 0x0022895C File Offset: 0x00226B5C
	private bool HandleSwipeGesture()
	{
		this.m_swipeRect = CameraUtils.CreateGUIScreenRect(Camera.main, this.m_upperLeftBone, this.m_lowerRightBone);
		if (UniversalInputManager.Get().GetMouseButtonDown(0) && this.InSwipeRect(UniversalInputManager.Get().GetMousePosition()))
		{
			this.m_checkingPossibleSwipe = true;
			this.m_swipeDetectTimer = 0f;
			this.m_swipeStart = UniversalInputManager.Get().GetMousePosition();
			this.m_swipeElement = PegUI.Get().FindHitElement();
			return true;
		}
		if (this.m_checkingPossibleSwipe)
		{
			this.m_swipeDetectTimer += Time.deltaTime;
			if (UniversalInputManager.Get().GetMouseButtonUp(0))
			{
				this.m_checkingPossibleSwipe = false;
				if (!this.CheckSwipe() && this.m_swipeElement != null && this.m_swipeElement == PegUI.Get().FindHitElement())
				{
					this.m_swipeElement.TriggerPress();
					this.m_swipeElement.TriggerRelease();
				}
				return true;
			}
			if (this.m_swipeDetectTimer < 0.1f)
			{
				return true;
			}
			this.m_checkingPossibleSwipe = false;
			if (this.CheckSwipe())
			{
				return true;
			}
			if (this.m_swipeElement != null)
			{
				PegUI.Get().DoMouseDown(this.m_swipeElement, this.m_swipeStart);
			}
		}
		return false;
	}

	// Token: 0x060074F5 RID: 29941 RVA: 0x00228AB8 File Offset: 0x00226CB8
	public void RegisterSwipeListener(SwipeHandler.DelSwipeListener listener)
	{
		SwipeHandler.m_swipeListener = listener;
	}

	// Token: 0x04005F80 RID: 24448
	private const float SWIPE_DETECT_DURATION = 0.1f;

	// Token: 0x04005F81 RID: 24449
	private const float SWIPE_DETECT_WIDTH = 0.09f;

	// Token: 0x04005F82 RID: 24450
	private const float SWIPE_FROM_TARGET_PENALTY = 0.035f;

	// Token: 0x04005F83 RID: 24451
	public Transform m_upperLeftBone;

	// Token: 0x04005F84 RID: 24452
	public Transform m_lowerRightBone;

	// Token: 0x04005F85 RID: 24453
	private float m_swipeDetectTimer;

	// Token: 0x04005F86 RID: 24454
	private bool m_checkingPossibleSwipe;

	// Token: 0x04005F87 RID: 24455
	private Vector3 m_swipeStart;

	// Token: 0x04005F88 RID: 24456
	private PegUIElement m_swipeElement;

	// Token: 0x04005F89 RID: 24457
	private Rect m_swipeRect;

	// Token: 0x04005F8A RID: 24458
	private static SwipeHandler.DelSwipeListener m_swipeListener;

	// Token: 0x02000F62 RID: 3938
	public enum SWIPE_DIRECTION
	{
		// Token: 0x04005F8C RID: 24460
		RIGHT,
		// Token: 0x04005F8D RID: 24461
		LEFT
	}

	// Token: 0x02000F63 RID: 3939
	// (Invoke) Token: 0x060074F7 RID: 29943
	public delegate void DelSwipeListener(SwipeHandler.SWIPE_DIRECTION direction);
}
