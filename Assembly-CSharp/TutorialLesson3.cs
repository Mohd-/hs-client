using System;
using UnityEngine;

// Token: 0x02000B0B RID: 2827
public class TutorialLesson3 : MonoBehaviour
{
	// Token: 0x060060C5 RID: 24773 RVA: 0x001CF4E4 File Offset: 0x001CD6E4
	private void Awake()
	{
		this.m_attacker.SetGameStringText("GLOBAL_TUTORIAL_ATTACKER");
		this.m_defender.SetGameStringText("GLOBAL_TUTORIAL_DEFENDER");
	}

	// Token: 0x04004866 RID: 18534
	public UberText m_attacker;

	// Token: 0x04004867 RID: 18535
	public UberText m_defender;
}
