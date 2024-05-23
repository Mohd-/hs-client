using System;
using UnityEngine;

// Token: 0x020008EF RID: 2287
public class MulliganReplaceLabel : MonoBehaviour
{
	// Token: 0x060055C3 RID: 21955 RVA: 0x0019BD17 File Offset: 0x00199F17
	private void Awake()
	{
		this.m_labelText.Text = GameStrings.Get("GAMEPLAY_MULLIGAN_REPLACE");
	}

	// Token: 0x04003C1D RID: 15389
	public UberText m_labelText;
}
