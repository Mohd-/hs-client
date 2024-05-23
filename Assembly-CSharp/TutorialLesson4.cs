using System;
using UnityEngine;

// Token: 0x02000B0C RID: 2828
public class TutorialLesson4 : MonoBehaviour
{
	// Token: 0x060060C7 RID: 24775 RVA: 0x001CF51C File Offset: 0x001CD71C
	private void Awake()
	{
		this.m_tauntDescriptionTitle.SetGameStringText("GLOBAL_TUTORIAL_TAUNT");
		this.m_tauntDescription.SetGameStringText("GLOBAL_TUTORIAL_TAUNT_DESCRIPTION");
		this.m_taunt.SetGameStringText("GLOBAL_TUTORIAL_TAUNT");
	}

	// Token: 0x04004868 RID: 18536
	public UberText m_tauntDescriptionTitle;

	// Token: 0x04004869 RID: 18537
	public UberText m_tauntDescription;

	// Token: 0x0400486A RID: 18538
	public UberText m_taunt;
}
