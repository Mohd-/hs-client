using System;
using UnityEngine;

// Token: 0x0200026D RID: 621
public class ShownUIMgr : MonoBehaviour
{
	// Token: 0x060022DA RID: 8922 RVA: 0x000AB92F File Offset: 0x000A9B2F
	private void Awake()
	{
		ShownUIMgr.s_instance = this;
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x000AB937 File Offset: 0x000A9B37
	private void OnDestroy()
	{
		ShownUIMgr.s_instance = null;
	}

	// Token: 0x060022DC RID: 8924 RVA: 0x000AB93F File Offset: 0x000A9B3F
	public static ShownUIMgr Get()
	{
		return ShownUIMgr.s_instance;
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x000AB946 File Offset: 0x000A9B46
	public void SetShownUI(ShownUIMgr.UI_WINDOW uiWindow)
	{
		this.m_shownUI = uiWindow;
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x000AB94F File Offset: 0x000A9B4F
	public ShownUIMgr.UI_WINDOW GetShownUI()
	{
		return this.m_shownUI;
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x000AB957 File Offset: 0x000A9B57
	public bool HasShownUI()
	{
		return this.m_shownUI != ShownUIMgr.UI_WINDOW.NONE;
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x000AB965 File Offset: 0x000A9B65
	public void ClearShownUI()
	{
		this.m_shownUI = ShownUIMgr.UI_WINDOW.NONE;
	}

	// Token: 0x04001431 RID: 5169
	private ShownUIMgr.UI_WINDOW m_shownUI;

	// Token: 0x04001432 RID: 5170
	private static ShownUIMgr s_instance;

	// Token: 0x0200026E RID: 622
	public enum UI_WINDOW
	{
		// Token: 0x04001434 RID: 5172
		NONE,
		// Token: 0x04001435 RID: 5173
		GENERAL_STORE,
		// Token: 0x04001436 RID: 5174
		QUEST_LOG
	}
}
