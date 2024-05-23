using System;
using UnityEngine;

// Token: 0x0200087A RID: 2170
[Serializable]
public class GamesWonSegment
{
	// Token: 0x060052F4 RID: 21236 RVA: 0x0018BA0C File Offset: 0x00189C0C
	public virtual void Init(Reward.Type rewardType, int rewardAmount, bool hideCrown)
	{
		if (hideCrown)
		{
			this.m_crown.Hide();
		}
		else
		{
			this.m_crown.Show();
		}
		this.m_root.SetActive(true);
	}

	// Token: 0x060052F5 RID: 21237 RVA: 0x0018BA3B File Offset: 0x00189C3B
	public virtual void AnimateReward()
	{
		this.m_crown.Animate();
	}

	// Token: 0x060052F6 RID: 21238 RVA: 0x0018BA48 File Offset: 0x00189C48
	public virtual float GetWidth()
	{
		return this.m_root.GetComponent<Renderer>().bounds.size.x;
	}

	// Token: 0x060052F7 RID: 21239 RVA: 0x0018BA75 File Offset: 0x00189C75
	public virtual void Hide()
	{
		this.m_root.SetActive(false);
	}

	// Token: 0x0400392F RID: 14639
	public GameObject m_root;

	// Token: 0x04003930 RID: 14640
	public GamesWonCrown m_crown;
}
