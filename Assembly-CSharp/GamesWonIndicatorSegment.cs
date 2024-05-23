using System;
using UnityEngine;

// Token: 0x02000878 RID: 2168
public class GamesWonIndicatorSegment : MonoBehaviour
{
	// Token: 0x060052F0 RID: 21232 RVA: 0x0018B940 File Offset: 0x00189B40
	public void Init(GamesWonIndicatorSegment.Type segmentType, Reward.Type rewardType, int rewardAmount, bool hideCrown)
	{
		switch (segmentType)
		{
		case GamesWonIndicatorSegment.Type.LEFT:
			this.m_activeSegment = this.m_leftSegment;
			this.m_middleSegment.Hide();
			this.m_rightSegment.Hide();
			break;
		case GamesWonIndicatorSegment.Type.MIDDLE:
			this.m_activeSegment = this.m_middleSegment;
			this.m_leftSegment.Hide();
			this.m_rightSegment.Hide();
			break;
		case GamesWonIndicatorSegment.Type.RIGHT:
			this.m_activeSegment = this.m_rightSegment;
			this.m_leftSegment.Hide();
			this.m_middleSegment.Hide();
			break;
		}
		this.m_activeSegment.Init(rewardType, rewardAmount, hideCrown);
	}

	// Token: 0x060052F1 RID: 21233 RVA: 0x0018B9EA File Offset: 0x00189BEA
	public float GetWidth()
	{
		return this.m_activeSegment.GetWidth();
	}

	// Token: 0x060052F2 RID: 21234 RVA: 0x0018B9F7 File Offset: 0x00189BF7
	public void AnimateReward()
	{
		this.m_activeSegment.AnimateReward();
	}

	// Token: 0x04003927 RID: 14631
	public GamesWonSegment m_leftSegment;

	// Token: 0x04003928 RID: 14632
	public MiddleGamesWonSegment m_middleSegment;

	// Token: 0x04003929 RID: 14633
	public RightGamesWonSegment m_rightSegment;

	// Token: 0x0400392A RID: 14634
	private GamesWonSegment m_activeSegment;

	// Token: 0x02000879 RID: 2169
	public enum Type
	{
		// Token: 0x0400392C RID: 14636
		LEFT,
		// Token: 0x0400392D RID: 14637
		MIDDLE,
		// Token: 0x0400392E RID: 14638
		RIGHT
	}
}
