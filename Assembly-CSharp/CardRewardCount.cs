using System;
using UnityEngine;

// Token: 0x0200034B RID: 843
public class CardRewardCount : MonoBehaviour
{
	// Token: 0x06002BD9 RID: 11225 RVA: 0x000DA1AD File Offset: 0x000D83AD
	private void Awake()
	{
		this.m_multiplierText.Text = GameStrings.Get("GLOBAL_REWARD_CARD_COUNT_MULTIPLIER");
	}

	// Token: 0x06002BDA RID: 11226 RVA: 0x000DA1C4 File Offset: 0x000D83C4
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06002BDB RID: 11227 RVA: 0x000DA1D2 File Offset: 0x000D83D2
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002BDC RID: 11228 RVA: 0x000DA1E0 File Offset: 0x000D83E0
	public void SetCount(int count)
	{
		this.m_countText.Text = count.ToString();
	}

	// Token: 0x04001AC0 RID: 6848
	public UberText m_countText;

	// Token: 0x04001AC1 RID: 6849
	public UberText m_multiplierText;
}
