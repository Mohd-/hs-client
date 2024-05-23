using System;
using UnityEngine;

// Token: 0x02000462 RID: 1122
public class CardBackDeckDisplay : MonoBehaviour
{
	// Token: 0x06003732 RID: 14130 RVA: 0x0010E9A0 File Offset: 0x0010CBA0
	private void Start()
	{
		this.m_CardBackManager = CardBackManager.Get();
		if (this.m_CardBackManager == null)
		{
			Debug.LogError("Failed to get CardBackManager!");
			base.enabled = false;
		}
		this.UpdateDeckCardBacks();
	}

	// Token: 0x06003733 RID: 14131 RVA: 0x0010E9D5 File Offset: 0x0010CBD5
	public void UpdateDeckCardBacks()
	{
		if (this.m_CardBackManager == null)
		{
			return;
		}
		this.m_CardBackManager.UpdateDeck(base.gameObject, this.m_FriendlyDeck);
	}

	// Token: 0x04002287 RID: 8839
	public bool m_FriendlyDeck = true;

	// Token: 0x04002288 RID: 8840
	private CardBackManager m_CardBackManager;
}
