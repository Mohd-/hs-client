using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class PegUIElement : MonoBehaviour
{
	// Token: 0x060013E6 RID: 5094 RVA: 0x00058533 File Offset: 0x00056733
	protected virtual void Awake()
	{
		this.m_doubleClickEnabled = this.HasOverriddenDoubleClick();
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x00058541 File Offset: 0x00056741
	protected virtual void OnOver(PegUIElement.InteractionState oldState)
	{
	}

	// Token: 0x060013E8 RID: 5096 RVA: 0x00058543 File Offset: 0x00056743
	protected virtual void OnOut(PegUIElement.InteractionState oldState)
	{
	}

	// Token: 0x060013E9 RID: 5097 RVA: 0x00058545 File Offset: 0x00056745
	protected virtual void OnPress()
	{
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x00058547 File Offset: 0x00056747
	protected virtual void OnTap()
	{
	}

	// Token: 0x060013EB RID: 5099 RVA: 0x00058549 File Offset: 0x00056749
	protected virtual void OnRelease()
	{
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x0005854B File Offset: 0x0005674B
	protected virtual void OnReleaseAll(bool mouseIsOver)
	{
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x0005854D File Offset: 0x0005674D
	protected virtual void OnHold()
	{
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x0005854F File Offset: 0x0005674F
	protected virtual void OnDoubleClick()
	{
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x00058551 File Offset: 0x00056751
	protected virtual void OnRightClick()
	{
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x00058553 File Offset: 0x00056753
	public void SetInteractionState(PegUIElement.InteractionState state)
	{
		this.m_interactionState = state;
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x0005855C File Offset: 0x0005675C
	public virtual void TriggerOver()
	{
		if (!this.m_enabled)
		{
			return;
		}
		if (this.m_focused)
		{
			return;
		}
		this.PrintLog("OVER", PegUIElement.PegUILogLevel.ALL_EVENTS);
		this.m_focused = true;
		PegUIElement.InteractionState interactionState = this.m_interactionState;
		this.m_interactionState = PegUIElement.InteractionState.Over;
		this.OnOver(interactionState);
		this.DispatchEvent(new UIEvent(UIEventType.ROLLOVER, this));
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x000585B8 File Offset: 0x000567B8
	public virtual void TriggerOut()
	{
		if (!this.m_enabled)
		{
			return;
		}
		this.PrintLog("OUT", PegUIElement.PegUILogLevel.ALL_EVENTS);
		this.m_focused = false;
		PegUIElement.InteractionState interactionState = this.m_interactionState;
		this.m_interactionState = PegUIElement.InteractionState.Out;
		this.OnOut(interactionState);
		this.DispatchEvent(new UIEvent(UIEventType.ROLLOUT, this));
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x00058606 File Offset: 0x00056806
	public virtual void TriggerPress()
	{
		if (!this.m_enabled)
		{
			return;
		}
		this.PrintLog("PRESS", PegUIElement.PegUILogLevel.PRESS);
		this.m_focused = true;
		this.m_interactionState = PegUIElement.InteractionState.Down;
		this.OnPress();
		this.DispatchEvent(new UIEvent(UIEventType.PRESS, this));
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x00058641 File Offset: 0x00056841
	public virtual void TriggerTap()
	{
		if (!this.m_enabled)
		{
			return;
		}
		this.PrintLog("TAP", PegUIElement.PegUILogLevel.ALL_EVENTS);
		this.m_interactionState = PegUIElement.InteractionState.Up;
		this.OnTap();
		this.DispatchEvent(new UIEvent(UIEventType.TAP, this));
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x00058676 File Offset: 0x00056876
	public virtual void TriggerRelease()
	{
		if (!this.m_enabled)
		{
			return;
		}
		this.PrintLog("RELEASE", PegUIElement.PegUILogLevel.ALL_EVENTS);
		this.m_interactionState = PegUIElement.InteractionState.Up;
		this.OnRelease();
		this.DispatchEvent(new UIEvent(UIEventType.RELEASE, this));
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x000586AA File Offset: 0x000568AA
	public void TriggerReleaseAll(bool mouseIsOver)
	{
		if (!this.m_enabled)
		{
			return;
		}
		this.m_interactionState = PegUIElement.InteractionState.Up;
		this.OnReleaseAll(mouseIsOver);
		this.DispatchEvent(new UIReleaseAllEvent(mouseIsOver, this));
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x000586D3 File Offset: 0x000568D3
	public void TriggerHold()
	{
		if (!this.m_enabled)
		{
			return;
		}
		this.PrintLog("HOLD", PegUIElement.PegUILogLevel.ALL_EVENTS);
		this.m_interactionState = PegUIElement.InteractionState.Down;
		this.OnHold();
		this.DispatchEvent(new UIEvent(UIEventType.HOLD, this));
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x00058707 File Offset: 0x00056907
	public void TriggerDoubleClick()
	{
		if (!this.m_enabled)
		{
			return;
		}
		this.PrintLog("DCLICK", PegUIElement.PegUILogLevel.ALL_EVENTS);
		this.m_interactionState = PegUIElement.InteractionState.Down;
		this.OnDoubleClick();
		this.DispatchEvent(new UIEvent(UIEventType.DOUBLECLICK, this));
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x0005873B File Offset: 0x0005693B
	public void TriggerRightClick()
	{
		if (!this.m_enabled)
		{
			return;
		}
		this.PrintLog("RCLICK", PegUIElement.PegUILogLevel.ALL_EVENTS);
		this.OnRightClick();
		this.DispatchEvent(new UIEvent(UIEventType.RIGHTCLICK, this));
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x00058768 File Offset: 0x00056968
	public void SetDragTolerance(float newTolerance)
	{
		this.m_dragTolerance = newTolerance;
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x00058771 File Offset: 0x00056971
	public float GetDragTolerance()
	{
		return this.m_dragTolerance;
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x0005877C File Offset: 0x0005697C
	public virtual bool AddEventListener(UIEventType type, UIEvent.Handler handler)
	{
		List<UIEvent.Handler> list;
		if (!this.m_eventListeners.TryGetValue(type, out list))
		{
			list = new List<UIEvent.Handler>();
			this.m_eventListeners.Add(type, list);
			list.Add(handler);
			return true;
		}
		if (list.Contains(handler))
		{
			return false;
		}
		list.Add(handler);
		return true;
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x000587D0 File Offset: 0x000569D0
	public virtual bool RemoveEventListener(UIEventType type, UIEvent.Handler handler)
	{
		List<UIEvent.Handler> list;
		return this.m_eventListeners.TryGetValue(type, out list) && list.Remove(handler);
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x000587F9 File Offset: 0x000569F9
	public void ClearEventListeners()
	{
		this.m_eventListeners.Clear();
	}

	// Token: 0x060013FF RID: 5119 RVA: 0x00058808 File Offset: 0x00056A08
	public bool HasEventListener(UIEventType type)
	{
		List<UIEvent.Handler> list;
		return this.m_eventListeners.TryGetValue(type, out list) && list.Count > 0;
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x00058833 File Offset: 0x00056A33
	public virtual void SetEnabled(bool enabled)
	{
		if (enabled)
		{
			this.PrintLog("ENABLE", PegUIElement.PegUILogLevel.ALL_EVENTS);
		}
		else
		{
			this.PrintLog("DISABLE", PegUIElement.PegUILogLevel.ALL_EVENTS);
		}
		this.m_enabled = enabled;
		if (this.m_enabled)
		{
			return;
		}
		this.m_focused = false;
	}

	// Token: 0x06001401 RID: 5121 RVA: 0x00058872 File Offset: 0x00056A72
	public bool IsEnabled()
	{
		return this.m_enabled;
	}

	// Token: 0x06001402 RID: 5122 RVA: 0x0005887A File Offset: 0x00056A7A
	public bool GetDoubleClickEnabled()
	{
		return this.m_doubleClickEnabled;
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x00058882 File Offset: 0x00056A82
	public void SetReceiveReleaseWithoutMouseDown(bool receiveReleaseWithoutMouseDown)
	{
		this.m_receiveReleaseWithoutMouseDown = receiveReleaseWithoutMouseDown;
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x0005888B File Offset: 0x00056A8B
	public bool GetReceiveReleaseWithoutMouseDown()
	{
		return this.m_receiveReleaseWithoutMouseDown;
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x00058894 File Offset: 0x00056A94
	public bool GetReceiveOverWithMouseDown()
	{
		return UniversalInputManager.Get().IsTouchMode();
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x000588B9 File Offset: 0x00056AB9
	public PegUIElement.InteractionState GetInteractionState()
	{
		return this.m_interactionState;
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x000588C1 File Offset: 0x00056AC1
	public void SetData(object data)
	{
		this.m_data = data;
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x000588CA File Offset: 0x00056ACA
	public object GetData()
	{
		return this.m_data;
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x000588D2 File Offset: 0x00056AD2
	public void SetOriginalLocalPosition()
	{
		this.SetOriginalLocalPosition(base.transform.localPosition);
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x000588E5 File Offset: 0x00056AE5
	public void SetOriginalLocalPosition(Vector3 pos)
	{
		this.m_originalLocalPosition = pos;
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x000588EE File Offset: 0x00056AEE
	public Vector3 GetOriginalLocalPosition()
	{
		return this.m_originalLocalPosition;
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x000588F6 File Offset: 0x00056AF6
	public void SetCursorDown(PegCursor.Mode mode)
	{
		this.m_cursorDownOverride = mode;
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x000588FF File Offset: 0x00056AFF
	public PegCursor.Mode GetCursorDown()
	{
		return this.m_cursorDownOverride;
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x00058907 File Offset: 0x00056B07
	public void SetCursorOver(PegCursor.Mode mode)
	{
		this.m_cursorOverOverride = mode;
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x00058910 File Offset: 0x00056B10
	public PegCursor.Mode GetCursorOver()
	{
		return this.m_cursorOverOverride;
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x00058918 File Offset: 0x00056B18
	private void DispatchEvent(UIEvent e)
	{
		List<UIEvent.Handler> list;
		if (!this.m_eventListeners.TryGetValue(e.GetEventType(), out list))
		{
			return;
		}
		foreach (UIEvent.Handler handler in list.ToArray())
		{
			handler(e);
		}
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x00058964 File Offset: 0x00056B64
	private bool HasOverriddenDoubleClick()
	{
		Type type = base.GetType();
		Type typeFromHandle = typeof(PegUIElement);
		MethodInfo method = typeFromHandle.GetMethod("OnDoubleClick", 36);
		MethodInfo method2 = type.GetMethod("OnDoubleClick", 36);
		return GeneralUtils.IsOverriddenMethod(method2, method);
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x000589A8 File Offset: 0x00056BA8
	private void PrintLog(string evt, PegUIElement.PegUILogLevel logLevel)
	{
		if (base.gameObject != null && ApplicationMgr.IsInternal() && Options.Get().GetInt(Option.PEGUI_DEBUG) >= (int)logLevel)
		{
			Debug.Log(string.Format("{0,-7} {1}", evt + ":", DebugUtils.GetHierarchyPath(base.gameObject, '/')));
		}
	}

	// Token: 0x04000A44 RID: 2628
	private MeshFilter m_meshFilter;

	// Token: 0x04000A45 RID: 2629
	private MeshRenderer m_renderer;

	// Token: 0x04000A46 RID: 2630
	private Collider m_collider;

	// Token: 0x04000A47 RID: 2631
	private Map<UIEventType, List<UIEvent.Handler>> m_eventListeners = new Map<UIEventType, List<UIEvent.Handler>>();

	// Token: 0x04000A48 RID: 2632
	private bool m_enabled = true;

	// Token: 0x04000A49 RID: 2633
	private bool m_focused;

	// Token: 0x04000A4A RID: 2634
	private bool m_doubleClickEnabled;

	// Token: 0x04000A4B RID: 2635
	private bool m_receiveReleaseWithoutMouseDown;

	// Token: 0x04000A4C RID: 2636
	private object m_data;

	// Token: 0x04000A4D RID: 2637
	private Vector3 m_originalLocalPosition;

	// Token: 0x04000A4E RID: 2638
	private PegUIElement.InteractionState m_interactionState;

	// Token: 0x04000A4F RID: 2639
	private float m_dragTolerance = 0.7f;

	// Token: 0x04000A50 RID: 2640
	private PegCursor.Mode m_cursorDownOverride = PegCursor.Mode.NONE;

	// Token: 0x04000A51 RID: 2641
	private PegCursor.Mode m_cursorOverOverride = PegCursor.Mode.NONE;

	// Token: 0x020001B2 RID: 434
	public enum InteractionState
	{
		// Token: 0x04000EE5 RID: 3813
		None,
		// Token: 0x04000EE6 RID: 3814
		Out,
		// Token: 0x04000EE7 RID: 3815
		Over,
		// Token: 0x04000EE8 RID: 3816
		Down,
		// Token: 0x04000EE9 RID: 3817
		Up,
		// Token: 0x04000EEA RID: 3818
		Disabled
	}

	// Token: 0x020001B3 RID: 435
	public enum PegUILogLevel
	{
		// Token: 0x04000EEC RID: 3820
		NONE,
		// Token: 0x04000EED RID: 3821
		PRESS,
		// Token: 0x04000EEE RID: 3822
		ALL_EVENTS,
		// Token: 0x04000EEF RID: 3823
		HIT_TEST
	}
}
