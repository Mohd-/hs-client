using System;
using UnityEngine;

// Token: 0x02000EE9 RID: 3817
public class ScreenEffect : MonoBehaviour
{
	// Token: 0x0600724B RID: 29259 RVA: 0x00219789 File Offset: 0x00217989
	private void Awake()
	{
		this.m_ScreenEffectsMgr = ScreenEffectsMgr.Get();
	}

	// Token: 0x0600724C RID: 29260 RVA: 0x00219798 File Offset: 0x00217998
	private void OnEnable()
	{
		if (this.m_ScreenEffectsMgr == null)
		{
			this.m_ScreenEffectsMgr = ScreenEffectsMgr.Get();
		}
		ScreenEffectsMgr.RegisterScreenEffect(this);
	}

	// Token: 0x0600724D RID: 29261 RVA: 0x002197C7 File Offset: 0x002179C7
	private void OnDisable()
	{
		if (this.m_ScreenEffectsMgr == null)
		{
			this.m_ScreenEffectsMgr = ScreenEffectsMgr.Get();
		}
		if (this.m_ScreenEffectsMgr != null)
		{
			ScreenEffectsMgr.UnRegisterScreenEffect(this);
		}
	}

	// Token: 0x04005C67 RID: 23655
	private ScreenEffectsMgr m_ScreenEffectsMgr;
}
