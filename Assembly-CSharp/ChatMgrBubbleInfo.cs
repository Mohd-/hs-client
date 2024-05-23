using System;
using UnityEngine;

// Token: 0x02000513 RID: 1299
[Serializable]
public class ChatMgrBubbleInfo
{
	// Token: 0x0400265A RID: 9818
	public Transform m_Parent;

	// Token: 0x0400265B RID: 9819
	public float m_ScaleInSec = 1f;

	// Token: 0x0400265C RID: 9820
	public iTween.EaseType m_ScaleInEaseType = iTween.EaseType.easeOutElastic;

	// Token: 0x0400265D RID: 9821
	public float m_HoldSec = 7f;

	// Token: 0x0400265E RID: 9822
	public float m_FadeOutSec = 2f;

	// Token: 0x0400265F RID: 9823
	public iTween.EaseType m_FadeOutEaseType = iTween.EaseType.linear;

	// Token: 0x04002660 RID: 9824
	public float m_MoveOverSec = 1f;

	// Token: 0x04002661 RID: 9825
	public iTween.EaseType m_MoveOverEaseType = iTween.EaseType.easeOutExpo;
}
