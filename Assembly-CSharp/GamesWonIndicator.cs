using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000876 RID: 2166
public class GamesWonIndicator : MonoBehaviour
{
	// Token: 0x060052EC RID: 21228 RVA: 0x0018B6C8 File Offset: 0x001898C8
	public void Init(Reward.Type rewardType, int rewardAmount, int numSegments, int numActiveSegments, GamesWonIndicator.InnKeeperTrigger trigger)
	{
		this.m_innkeeperTrigger = trigger;
		this.m_numActiveSegments = numActiveSegments;
		Vector3 position = this.m_segmentContainer.transform.position;
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < numSegments; i++)
		{
			GamesWonIndicatorSegment.Type type;
			if (i == 0)
			{
				type = GamesWonIndicatorSegment.Type.LEFT;
			}
			else if (i == numSegments - 1)
			{
				type = GamesWonIndicatorSegment.Type.RIGHT;
			}
			else
			{
				type = GamesWonIndicatorSegment.Type.MIDDLE;
			}
			bool hideCrown = i >= numActiveSegments - 1;
			GamesWonIndicatorSegment gamesWonIndicatorSegment = Object.Instantiate<GamesWonIndicatorSegment>(this.m_gamesWonSegmentPrefab);
			gamesWonIndicatorSegment.Init(type, rewardType, rewardAmount, hideCrown);
			gamesWonIndicatorSegment.transform.parent = this.m_segmentContainer.transform;
			gamesWonIndicatorSegment.transform.localScale = Vector3.one;
			float num3 = gamesWonIndicatorSegment.GetWidth() - 0.01f;
			if (type != GamesWonIndicatorSegment.Type.RIGHT)
			{
				position.x += num3;
			}
			else
			{
				position.x -= 0.01f;
			}
			gamesWonIndicatorSegment.transform.position = position;
			gamesWonIndicatorSegment.transform.rotation = Quaternion.identity;
			num = num3;
			num2 += num3;
			this.m_segments.Add(gamesWonIndicatorSegment);
		}
		Vector3 position2 = this.m_segmentContainer.transform.position;
		position2.x -= num2 / 2f;
		position2.x += num / 5f;
		this.m_segmentContainer.transform.position = position2;
		this.m_winCountText.Text = GameStrings.Format("GAMEPLAY_WIN_REWARD_PROGRESS", new object[]
		{
			this.m_numActiveSegments,
			numSegments
		});
	}

	// Token: 0x060052ED RID: 21229 RVA: 0x0018B878 File Offset: 0x00189A78
	public void Show()
	{
		this.m_root.SetActive(true);
		if (this.m_numActiveSegments <= 0)
		{
			Debug.LogError(string.Format("GamesWonIndicator.Show(): cannot do animation; numActiveSegments={0} but should be greater than zero", this.m_numActiveSegments));
			return;
		}
		if (this.m_numActiveSegments > this.m_segments.Count)
		{
			Debug.LogError(string.Format("GamesWonIndicator.Show(): cannot do animation; numActiveSegments = {0} but m_segments.Count = {1}", this.m_numActiveSegments, this.m_segments.Count));
			return;
		}
		this.m_segments[this.m_numActiveSegments - 1].AnimateReward();
		GamesWonIndicator.InnKeeperTrigger innkeeperTrigger = this.m_innkeeperTrigger;
		if (innkeeperTrigger != GamesWonIndicator.InnKeeperTrigger.NONE)
		{
		}
	}

	// Token: 0x060052EE RID: 21230 RVA: 0x0018B928 File Offset: 0x00189B28
	public void Hide()
	{
		this.m_root.SetActive(false);
	}

	// Token: 0x0400391D RID: 14621
	private const float FUDGE_FACTOR = 0.01f;

	// Token: 0x0400391E RID: 14622
	public GameObject m_root;

	// Token: 0x0400391F RID: 14623
	public GameObject m_segmentContainer;

	// Token: 0x04003920 RID: 14624
	public UberText m_winCountText;

	// Token: 0x04003921 RID: 14625
	public GamesWonIndicatorSegment m_gamesWonSegmentPrefab;

	// Token: 0x04003922 RID: 14626
	private List<GamesWonIndicatorSegment> m_segments = new List<GamesWonIndicatorSegment>();

	// Token: 0x04003923 RID: 14627
	private int m_numActiveSegments;

	// Token: 0x04003924 RID: 14628
	private GamesWonIndicator.InnKeeperTrigger m_innkeeperTrigger;

	// Token: 0x02000877 RID: 2167
	public enum InnKeeperTrigger
	{
		// Token: 0x04003926 RID: 14630
		NONE
	}
}
