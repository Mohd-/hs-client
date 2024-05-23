using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000268 RID: 616
public class PegUI : MonoBehaviour
{
	// Token: 0x0600229B RID: 8859 RVA: 0x000AA58F File Offset: 0x000A878F
	private void Awake()
	{
		PegUI.s_instance = this;
		this.m_lastClickPosition = Vector3.zero;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x000AA5B0 File Offset: 0x000A87B0
	private void OnDestroy()
	{
		if (UniversalInputManager.Get() != null)
		{
			UniversalInputManager.Get().UnregisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
		}
		if (ApplicationMgr.Get() != null)
		{
			ApplicationMgr.Get().RemoveFocusChangedListener(new ApplicationMgr.FocusChangedCallback(this.OnAppFocusChanged));
		}
		PegUI.s_instance = null;
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x000AA611 File Offset: 0x000A8811
	private void Start()
	{
		UniversalInputManager.Get().RegisterMouseOnOrOffScreenListener(new UniversalInputManager.MouseOnOrOffScreenCallback(this.OnMouseOnOrOffScreen));
		ApplicationMgr.Get().AddFocusChangedListener(new ApplicationMgr.FocusChangedCallback(this.OnAppFocusChanged));
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x000AA641 File Offset: 0x000A8841
	private void Update()
	{
		this.MouseInputUpdate();
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x000AA649 File Offset: 0x000A8849
	public static PegUI Get()
	{
		return PegUI.s_instance;
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x000AA650 File Offset: 0x000A8850
	public PegUIElement GetMousedOverElement()
	{
		return this.m_currentElement;
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000AA658 File Offset: 0x000A8858
	public PegUIElement GetPrevMousedOverElement()
	{
		return this.m_prevElement;
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x000AA660 File Offset: 0x000A8860
	public void SetInputCamera(Camera cam)
	{
		this.m_UICam = cam;
		if (this.m_UICam == null)
		{
			Debug.Log("UICam is null!");
			return;
		}
	}

	// Token: 0x060022A3 RID: 8867 RVA: 0x000AA688 File Offset: 0x000A8888
	public PegUIElement FindHitElement()
	{
		if (UniversalInputManager.Get().IsTouchMode() && !UniversalInputManager.Get().GetMouseButton(0) && !UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			return null;
		}
		RaycastHit raycastHit;
		foreach (GameLayer layer in PegUI.HIT_TEST_PRIORITY)
		{
			if (UniversalInputManager.Get().GetInputHitInfo(layer, out raycastHit))
			{
				return raycastHit.transform.GetComponent<PegUIElement>();
			}
		}
		if (UniversalInputManager.Get().GetInputHitInfo(this.m_UICam, out raycastHit))
		{
			return raycastHit.transform.GetComponent<PegUIElement>();
		}
		return null;
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x000AA728 File Offset: 0x000A8928
	public void DoMouseDown(PegUIElement element, Vector3 mouseDownPos)
	{
		this.m_currentElement = element;
		this.m_mouseDownElement = element;
		this.m_currentElement.TriggerPress();
		this.m_lastClickPosition = mouseDownPos;
		if (Vector3.Distance(this.m_lastClickPosition, UniversalInputManager.Get().GetMousePosition()) > this.m_currentElement.GetDragTolerance())
		{
			this.m_currentElement.TriggerHold();
		}
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x000AA788 File Offset: 0x000A8988
	private void MouseInputUpdate()
	{
		if (this.m_UICam == null || !this.m_hasFocus)
		{
			return;
		}
		bool flag = false;
		foreach (PegUICustomBehavior pegUICustomBehavior in this.m_customBehaviors)
		{
			if (pegUICustomBehavior.UpdateUI())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (this.m_mouseDownElement != null)
			{
				this.m_mouseDownElement.TriggerOut();
			}
			this.m_mouseDownElement = null;
			return;
		}
		if (UniversalInputManager.Get().GetMouseButton(0) && this.m_mouseDownElement != null && this.m_lastClickPosition != Vector3.zero && Vector3.Distance(this.m_lastClickPosition, UniversalInputManager.Get().GetMousePosition()) > this.m_mouseDownElement.GetDragTolerance())
		{
			this.m_mouseDownElement.TriggerHold();
		}
		if (this.m_lastClickTimer != -1f)
		{
			this.m_lastClickTimer += Time.deltaTime;
		}
		if (this.m_mouseDownTimer != -1f)
		{
			this.m_mouseDownTimer += Time.deltaTime;
		}
		PegUIElement pegUIElement = this.FindHitElement();
		if (pegUIElement != null && ApplicationMgr.IsInternal() && Options.Get().GetInt(Option.PEGUI_DEBUG) >= 3)
		{
			Debug.Log(string.Format("{0,-7} {1}", "HIT:", DebugUtils.GetHierarchyPath(pegUIElement, '/')));
		}
		this.m_prevElement = this.m_currentElement;
		if (pegUIElement && pegUIElement.IsEnabled())
		{
			this.m_currentElement = pegUIElement;
		}
		else
		{
			this.m_currentElement = null;
		}
		if (this.m_prevElement && this.m_currentElement != this.m_prevElement)
		{
			PegCursor.Get().SetMode(PegCursor.Mode.UP);
			this.m_prevElement.TriggerOut();
		}
		if (this.m_currentElement == null)
		{
			if (UniversalInputManager.Get().GetMouseButtonDown(0))
			{
				PegCursor.Get().SetMode(PegCursor.Mode.DOWN);
			}
			else if (!UniversalInputManager.Get().GetMouseButton(0))
			{
				PegCursor.Get().SetMode(PegCursor.Mode.UP);
			}
			if (this.m_mouseDownElement && UniversalInputManager.Get().GetMouseButtonUp(0))
			{
				this.m_mouseDownElement.TriggerReleaseAll(false);
				this.m_mouseDownElement = null;
			}
			return;
		}
		if (this.UpdateMouseLeftClick())
		{
			return;
		}
		if (this.UpdateMouseLeftHold())
		{
			return;
		}
		if (this.UpdateMouseRightClick())
		{
			return;
		}
		this.UpdateMouseOver();
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x000AAA48 File Offset: 0x000A8C48
	private bool UpdateMouseLeftClick()
	{
		bool result = false;
		if (UniversalInputManager.Get().GetMouseButtonDown(0))
		{
			result = true;
			if (this.m_currentElement.GetCursorDown() != PegCursor.Mode.NONE)
			{
				PegCursor.Get().SetMode(this.m_currentElement.GetCursorDown());
			}
			else
			{
				PegCursor.Get().SetMode(PegCursor.Mode.DOWN);
			}
			this.m_mouseDownTimer = 0f;
			if (UniversalInputManager.Get().IsTouchMode() && this.m_currentElement.GetReceiveOverWithMouseDown())
			{
				this.m_currentElement.TriggerOver();
			}
			this.m_currentElement.TriggerPress();
			this.m_lastClickPosition = UniversalInputManager.Get().GetMousePosition();
			this.m_mouseDownElement = this.m_currentElement;
		}
		if (UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			result = true;
			if (this.m_lastClickTimer > 0f && this.m_lastClickTimer <= 0.7f && this.m_currentElement.GetDoubleClickEnabled())
			{
				this.m_currentElement.TriggerDoubleClick();
				this.m_lastClickTimer = -1f;
			}
			else
			{
				if (this.m_mouseDownElement == this.m_currentElement || this.m_currentElement.GetReceiveReleaseWithoutMouseDown())
				{
					if (this.m_mouseDownTimer <= 0.4f)
					{
						this.m_currentElement.TriggerTap();
					}
					this.m_currentElement.TriggerRelease();
				}
				if (this.m_currentElement.GetReceiveOverWithMouseDown())
				{
					this.m_currentElement.TriggerOut();
				}
				if (this.m_mouseDownElement)
				{
					this.m_lastClickTimer = 0f;
					this.m_mouseDownElement.TriggerReleaseAll(this.m_currentElement == this.m_mouseDownElement);
					this.m_mouseDownElement = null;
				}
			}
			if (this.m_currentElement.GetCursorOver() != PegCursor.Mode.NONE)
			{
				PegCursor.Get().SetMode(this.m_currentElement.GetCursorOver());
			}
			else
			{
				PegCursor.Get().SetMode(PegCursor.Mode.OVER);
			}
			this.m_mouseDownTimer = -1f;
			this.m_lastClickPosition = Vector3.zero;
		}
		return result;
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x000AAC4C File Offset: 0x000A8E4C
	private bool UpdateMouseLeftHold()
	{
		if (!UniversalInputManager.Get().GetMouseButton(0))
		{
			return false;
		}
		if (this.m_currentElement.GetReceiveOverWithMouseDown() && this.m_currentElement != this.m_prevElement)
		{
			if (this.m_currentElement.GetCursorOver() != PegCursor.Mode.NONE)
			{
				PegCursor.Get().SetMode(this.m_currentElement.GetCursorOver());
			}
			else
			{
				PegCursor.Get().SetMode(PegCursor.Mode.OVER);
			}
			this.m_currentElement.TriggerOver();
		}
		return true;
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x000AACD4 File Offset: 0x000A8ED4
	private bool UpdateMouseRightClick()
	{
		bool result = false;
		if (UniversalInputManager.Get().GetMouseButtonDown(1))
		{
			result = true;
			this.m_currentElement.TriggerRightClick();
		}
		return result;
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x000AAD04 File Offset: 0x000A8F04
	private void UpdateMouseOver()
	{
		if (UniversalInputManager.Get().IsTouchMode() && (!UniversalInputManager.Get().GetMouseButton(0) || !this.m_currentElement.GetReceiveOverWithMouseDown()))
		{
			return;
		}
		if (this.m_currentElement == this.m_prevElement)
		{
			return;
		}
		if (this.m_currentElement.GetCursorOver() != PegCursor.Mode.NONE)
		{
			PegCursor.Get().SetMode(this.m_currentElement.GetCursorOver());
		}
		else
		{
			PegCursor.Get().SetMode(PegCursor.Mode.OVER);
		}
		this.m_currentElement.TriggerOver();
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x000AAD9A File Offset: 0x000A8F9A
	private void OnAppFocusChanged(bool focus, object userData)
	{
		this.m_hasFocus = focus;
	}

	// Token: 0x060022AB RID: 8875 RVA: 0x000AADA4 File Offset: 0x000A8FA4
	private void OnMouseOnOrOffScreen(bool onScreen)
	{
		if (onScreen)
		{
			return;
		}
		this.m_lastClickPosition = Vector3.zero;
		if (this.m_currentElement != null)
		{
			this.m_currentElement.TriggerOut();
			this.m_currentElement = null;
		}
		PegCursor.Get().SetMode(PegCursor.Mode.UP);
		if (this.m_prevElement != null)
		{
			this.m_prevElement.TriggerOut();
			this.m_prevElement = null;
		}
		this.m_lastClickTimer = -1f;
	}

	// Token: 0x060022AC RID: 8876 RVA: 0x000AAE1F File Offset: 0x000A901F
	public void EnableSwipeDetection(Rect swipeRect, PegUI.DelSwipeListener listener)
	{
		this.m_swipeListener = listener;
	}

	// Token: 0x060022AD RID: 8877 RVA: 0x000AAE28 File Offset: 0x000A9028
	public void CancelSwipeDetection(PegUI.DelSwipeListener listener)
	{
		if (listener == this.m_swipeListener)
		{
			this.m_swipeListener = null;
		}
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x000AAE42 File Offset: 0x000A9042
	public void RegisterCustomBehavior(PegUICustomBehavior behavior)
	{
		this.m_customBehaviors.Add(behavior);
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x000AAE50 File Offset: 0x000A9050
	public void UnregisterCustomBehavior(PegUICustomBehavior behavior)
	{
		this.m_customBehaviors.Remove(behavior);
	}

	// Token: 0x04001405 RID: 5125
	private const float PRESS_VS_TAP_TOLERANCE = 0.4f;

	// Token: 0x04001406 RID: 5126
	private const float DOUBLECLICK_TOLERANCE = 0.7f;

	// Token: 0x04001407 RID: 5127
	private const float DOUBLECLICK_COUNT_DISABLED = -1f;

	// Token: 0x04001408 RID: 5128
	private const float MOUSEDOWN_COUNT_DISABLED = -1f;

	// Token: 0x04001409 RID: 5129
	public Camera orthographicUICam;

	// Token: 0x0400140A RID: 5130
	private static readonly GameLayer[] HIT_TEST_PRIORITY = new GameLayer[]
	{
		GameLayer.IgnoreFullScreenEffects,
		GameLayer.BackgroundUI,
		GameLayer.PerspectiveUI,
		GameLayer.CameraMask,
		GameLayer.UI,
		GameLayer.BattleNet,
		GameLayer.BattleNetFriendList,
		GameLayer.BattleNetChat,
		GameLayer.BattleNetDialog,
		GameLayer.HighPriorityUI
	};

	// Token: 0x0400140B RID: 5131
	private Camera m_UICam;

	// Token: 0x0400140C RID: 5132
	private PegUIElement m_prevElement;

	// Token: 0x0400140D RID: 5133
	private PegUIElement m_currentElement;

	// Token: 0x0400140E RID: 5134
	private PegUIElement m_mouseDownElement;

	// Token: 0x0400140F RID: 5135
	public static PegUI s_instance;

	// Token: 0x04001410 RID: 5136
	private float m_mouseDownTimer;

	// Token: 0x04001411 RID: 5137
	private float m_lastClickTimer;

	// Token: 0x04001412 RID: 5138
	private Vector3 m_lastClickPosition;

	// Token: 0x04001413 RID: 5139
	private List<PegUICustomBehavior> m_customBehaviors = new List<PegUICustomBehavior>();

	// Token: 0x04001414 RID: 5140
	private PegUI.DelSwipeListener m_swipeListener;

	// Token: 0x04001415 RID: 5141
	private bool m_hasFocus = true;

	// Token: 0x02000505 RID: 1285
	public enum Layer
	{
		// Token: 0x04002645 RID: 9797
		MANUAL,
		// Token: 0x04002646 RID: 9798
		RELATIVE_TO_PARENT,
		// Token: 0x04002647 RID: 9799
		BACKGROUND,
		// Token: 0x04002648 RID: 9800
		HUD,
		// Token: 0x04002649 RID: 9801
		DIALOG
	}

	// Token: 0x02000506 RID: 1286
	public enum SWIPE_DIRECTION
	{
		// Token: 0x0400264B RID: 9803
		RIGHT,
		// Token: 0x0400264C RID: 9804
		LEFT
	}

	// Token: 0x02000507 RID: 1287
	// (Invoke) Token: 0x06003BD3 RID: 15315
	public delegate void DelSwipeListener(PegUI.SWIPE_DIRECTION direction);
}
