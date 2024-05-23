using System;
using UnityEngine;

// Token: 0x02000B0A RID: 2826
public class TutorialLesson2 : MonoBehaviour
{
	// Token: 0x060060C3 RID: 24771 RVA: 0x001CF4AC File Offset: 0x001CD6AC
	private void Awake()
	{
		this.m_cost.SetGameStringText("GLOBAL_TUTORIAL_COST");
		this.m_yourMana.SetGameStringText("GLOBAL_TUTORIAL_YOUR_MANA");
	}

	// Token: 0x04004864 RID: 18532
	public UberText m_cost;

	// Token: 0x04004865 RID: 18533
	public UberText m_yourMana;
}
