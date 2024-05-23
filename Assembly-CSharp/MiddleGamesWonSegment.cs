using System;
using UnityEngine;

// Token: 0x0200087B RID: 2171
[Serializable]
public class MiddleGamesWonSegment : GamesWonSegment
{
	// Token: 0x060052F9 RID: 21241 RVA: 0x0018BA8C File Offset: 0x00189C8C
	public override void Init(Reward.Type rewardType, int rewardAmount, bool hideCrown)
	{
		base.Init(rewardType, rewardAmount, hideCrown);
		if (Random.value < 0.5f)
		{
			this.m_activeRoot = this.m_root1;
			this.m_root2.SetActive(false);
		}
		else
		{
			this.m_activeRoot = this.m_root2;
			this.m_root1.SetActive(false);
		}
		this.m_activeRoot.SetActive(true);
	}

	// Token: 0x060052FA RID: 21242 RVA: 0x0018BAF4 File Offset: 0x00189CF4
	public override float GetWidth()
	{
		return this.m_activeRoot.GetComponent<Renderer>().bounds.size.x;
	}

	// Token: 0x04003931 RID: 14641
	public GameObject m_root1;

	// Token: 0x04003932 RID: 14642
	public GameObject m_root2;

	// Token: 0x04003933 RID: 14643
	private GameObject m_activeRoot;
}
