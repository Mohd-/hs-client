using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000812 RID: 2066
public class DraftManaCurve : MonoBehaviour
{
	// Token: 0x06004FD6 RID: 20438 RVA: 0x0017AF2A File Offset: 0x0017912A
	private void Awake()
	{
		this.ResetBars();
	}

	// Token: 0x06004FD7 RID: 20439 RVA: 0x0017AF34 File Offset: 0x00179134
	public void UpdateBars()
	{
		int num = 0;
		foreach (int num2 in this.m_manaCosts)
		{
			if (num2 > num)
			{
				num = num2;
			}
		}
		if (num < 10)
		{
			num = 10;
		}
		for (int i = 0; i < this.m_bars.Count; i++)
		{
			this.m_bars[i].m_maxValue = (float)num;
			this.m_bars[i].AnimateBar((float)this.m_manaCosts[i]);
		}
	}

	// Token: 0x06004FD8 RID: 20440 RVA: 0x0017AFEC File Offset: 0x001791EC
	public void AddCardOfCost(int cost)
	{
		if (this.m_manaCosts == null)
		{
			return;
		}
		cost = Mathf.Clamp(cost, 0, 7);
		List<int> manaCosts;
		List<int> list = manaCosts = this.m_manaCosts;
		int num2;
		int num = num2 = cost;
		num2 = manaCosts[num2];
		list[num] = num2 + 1;
		this.UpdateBars();
	}

	// Token: 0x06004FD9 RID: 20441 RVA: 0x0017B030 File Offset: 0x00179230
	public void ResetBars()
	{
		this.m_manaCosts = new List<int>();
		for (int i = 0; i < this.m_bars.Count; i++)
		{
			this.m_manaCosts.Add(0);
		}
		this.UpdateBars();
	}

	// Token: 0x0400369B RID: 13979
	private const int MAX_CARDS = 10;

	// Token: 0x0400369C RID: 13980
	private const float SIZE_PER_CARD = 0.1f;

	// Token: 0x0400369D RID: 13981
	public List<ManaCostBar> m_bars;

	// Token: 0x0400369E RID: 13982
	private List<int> m_manaCosts;
}
