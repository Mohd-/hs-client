using System;
using UnityEngine;

// Token: 0x02000258 RID: 600
public class BoxEventMgr : MonoBehaviour
{
	// Token: 0x0600221A RID: 8730 RVA: 0x000A7CF0 File Offset: 0x000A5EF0
	private void Awake()
	{
		this.m_eventMap.Add(BoxEventType.STARTUP_HUB, this.m_EventInfo.m_StartupHub);
		this.m_eventMap.Add(BoxEventType.STARTUP_TUTORIAL, this.m_EventInfo.m_StartupTutorial);
		this.m_eventMap.Add(BoxEventType.TUTORIAL_PLAY, this.m_EventInfo.m_TutorialPlay);
		this.m_eventMap.Add(BoxEventType.DISK_LOADING, this.m_EventInfo.m_DiskLoading);
		this.m_eventMap.Add(BoxEventType.DISK_MAIN_MENU, this.m_EventInfo.m_DiskMainMenu);
		this.m_eventMap.Add(BoxEventType.DOORS_CLOSE, this.m_EventInfo.m_DoorsClose);
		this.m_eventMap.Add(BoxEventType.DOORS_OPEN, this.m_EventInfo.m_DoorsOpen);
		this.m_eventMap.Add(BoxEventType.DRAWER_OPEN, this.m_EventInfo.m_DrawerOpen);
		this.m_eventMap.Add(BoxEventType.DRAWER_CLOSE, this.m_EventInfo.m_DrawerClose);
		this.m_eventMap.Add(BoxEventType.SHADOW_FADE_IN, this.m_EventInfo.m_ShadowFadeIn);
		this.m_eventMap.Add(BoxEventType.SHADOW_FADE_OUT, this.m_EventInfo.m_ShadowFadeOut);
		this.m_eventMap.Add(BoxEventType.STARTUP_SET_ROTATION, this.m_EventInfo.m_StartupSetRotation);
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x000A7E18 File Offset: 0x000A6018
	public Spell GetEventSpell(BoxEventType eventType)
	{
		Spell result = null;
		this.m_eventMap.TryGetValue(eventType, out result);
		return result;
	}

	// Token: 0x04001383 RID: 4995
	public BoxEventInfo m_EventInfo;

	// Token: 0x04001384 RID: 4996
	private Map<BoxEventType, Spell> m_eventMap = new Map<BoxEventType, Spell>();
}
