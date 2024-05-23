using System;
using UnityEngine;

// Token: 0x0200087C RID: 2172
[Serializable]
public class RightGamesWonSegment : GamesWonSegment
{
	// Token: 0x060052FC RID: 21244 RVA: 0x0018BB2C File Offset: 0x00189D2C
	public override void Init(Reward.Type rewardType, int rewardAmount, bool hideCrown)
	{
		base.Init(rewardType, rewardAmount, hideCrown);
		this.m_rewardType = rewardType;
		Reward.Type rewardType2 = this.m_rewardType;
		if (rewardType2 != Reward.Type.ARCANE_DUST)
		{
			if (rewardType2 != Reward.Type.BOOSTER_PACK)
			{
				if (rewardType2 != Reward.Type.GOLD)
				{
					Debug.LogError(string.Format("GamesWonIndicatorSegment(): don't know how to init right segment with reward type {0}", this.m_rewardType));
					this.m_boosterRoot.SetActive(false);
					this.m_dustRoot.SetActive(false);
					this.m_goldRoot.SetActive(false);
				}
				else
				{
					this.m_boosterRoot.SetActive(false);
					this.m_dustRoot.SetActive(false);
					this.m_goldRoot.SetActive(true);
					this.m_goldAmountText.Text = rewardAmount.ToString();
				}
			}
			else
			{
				this.m_dustRoot.SetActive(false);
				this.m_goldRoot.SetActive(false);
				this.m_boosterRoot.SetActive(true);
			}
		}
		else
		{
			this.m_boosterRoot.SetActive(false);
			this.m_goldRoot.SetActive(false);
			this.m_dustRoot.SetActive(true);
			this.m_dustAmountText.Text = rewardAmount.ToString();
		}
	}

	// Token: 0x060052FD RID: 21245 RVA: 0x0018BC4C File Offset: 0x00189E4C
	public override void AnimateReward()
	{
		this.m_crown.Animate();
		PlayMakerFSM playMakerFSM = null;
		Reward.Type rewardType = this.m_rewardType;
		if (rewardType != Reward.Type.ARCANE_DUST)
		{
			if (rewardType != Reward.Type.BOOSTER_PACK)
			{
				if (rewardType == Reward.Type.GOLD)
				{
					playMakerFSM = this.m_goldRoot.GetComponent<PlayMakerFSM>();
				}
			}
			else
			{
				playMakerFSM = this.m_boosterRoot.GetComponent<PlayMakerFSM>();
			}
		}
		else
		{
			playMakerFSM = this.m_dustRoot.GetComponent<PlayMakerFSM>();
		}
		if (playMakerFSM == null)
		{
			Debug.LogError(string.Format("GamesWonIndicatorSegment(): missing playMaker component for reward type {0}", this.m_rewardType));
			return;
		}
		playMakerFSM.SendEvent("Birth");
	}

	// Token: 0x060052FE RID: 21246 RVA: 0x0018BCF0 File Offset: 0x00189EF0
	public override float GetWidth()
	{
		Reward.Type rewardType = this.m_rewardType;
		if (rewardType == Reward.Type.ARCANE_DUST)
		{
			return this.m_dustRoot.GetComponent<Renderer>().bounds.size.x;
		}
		if (rewardType == Reward.Type.BOOSTER_PACK)
		{
			return this.m_boosterRoot.GetComponent<Renderer>().bounds.size.x;
		}
		if (rewardType != Reward.Type.GOLD)
		{
			return 0f;
		}
		return this.m_goldRoot.GetComponent<Renderer>().bounds.size.x;
	}

	// Token: 0x04003934 RID: 14644
	public GameObject m_boosterRoot;

	// Token: 0x04003935 RID: 14645
	public GameObject m_dustRoot;

	// Token: 0x04003936 RID: 14646
	public GameObject m_goldRoot;

	// Token: 0x04003937 RID: 14647
	public UberText m_dustAmountText;

	// Token: 0x04003938 RID: 14648
	public UberText m_goldAmountText;

	// Token: 0x04003939 RID: 14649
	private Reward.Type m_rewardType;
}
