using System;
using UnityEngine;

// Token: 0x02000AC0 RID: 2752
public class Secret : MonoBehaviour
{
	// Token: 0x06005F2C RID: 24364 RVA: 0x001C7CA0 File Offset: 0x001C5EA0
	private void Start()
	{
		this.secretLabelTop.SetGameStringText("GAMEPLAY_SECRET_BANNER_TITLE");
		this.secretLabelMiddle.SetGameStringText("GAMEPLAY_SECRET_BANNER_TITLE");
		this.secretLabelBottom.SetGameStringText("GAMEPLAY_SECRET_BANNER_TITLE");
	}

	// Token: 0x0400469B RID: 18075
	public UberText secretLabelTop;

	// Token: 0x0400469C RID: 18076
	public UberText secretLabelMiddle;

	// Token: 0x0400469D RID: 18077
	public UberText secretLabelBottom;
}
