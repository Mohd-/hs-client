using System;
using UnityEngine;

// Token: 0x02000B09 RID: 2825
public class TutorialLesson1 : MonoBehaviour
{
	// Token: 0x060060C1 RID: 24769 RVA: 0x001CF464 File Offset: 0x001CD664
	private void Awake()
	{
		this.m_health.SetGameStringText("GLOBAL_TUTORIAL_HEALTH");
		this.m_attack.SetGameStringText("GLOBAL_TUTORIAL_ATTACK");
		this.m_minion.SetGameStringText("GLOBAL_TUTORIAL_MINION");
	}

	// Token: 0x04004861 RID: 18529
	public UberText m_health;

	// Token: 0x04004862 RID: 18530
	public UberText m_attack;

	// Token: 0x04004863 RID: 18531
	public UberText m_minion;
}
