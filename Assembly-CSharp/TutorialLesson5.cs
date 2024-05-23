using System;
using UnityEngine;

// Token: 0x02000B0D RID: 2829
public class TutorialLesson5 : MonoBehaviour
{
	// Token: 0x060060C9 RID: 24777 RVA: 0x001CF564 File Offset: 0x001CD764
	private void Awake()
	{
		this.m_heroPower.SetGameStringText("GLOBAL_TUTORIAL_HERO_POWER");
		this.m_used.SetGameStringText("GLOBAL_TUTORIAL_USED");
		this.m_yourTurn.SetGameStringText("GLOBAL_TUTORIAL_YOUR_TURN");
	}

	// Token: 0x0400486B RID: 18539
	public UberText m_heroPower;

	// Token: 0x0400486C RID: 18540
	public UberText m_used;

	// Token: 0x0400486D RID: 18541
	public UberText m_yourTurn;
}
