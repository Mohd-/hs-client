using System;
using UnityEngine;

// Token: 0x0200052D RID: 1325
public class QuestTile : MonoBehaviour
{
	// Token: 0x06003DA6 RID: 15782 RVA: 0x0012A0FC File Offset: 0x001282FC
	private void Awake()
	{
		this.SetCanShowCancelButton(false);
		this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelButtonReleased));
	}

	// Token: 0x06003DA7 RID: 15783 RVA: 0x0012A12C File Offset: 0x0012832C
	public void SetupTile(Achievement quest)
	{
		quest.AckCurrentProgressAndRewardNotices();
		this.m_goldAmount.gameObject.SetActive(false);
		this.m_quest = quest;
		if (this.m_quest.MaxProgress > 1)
		{
			this.m_progressText.Text = this.m_quest.Progress + "/" + this.m_quest.MaxProgress;
			this.m_progress.SetActive(true);
		}
		else
		{
			this.m_progressText.Text = string.Empty;
			this.m_progress.SetActive(false);
		}
		if (quest.IsLegendary)
		{
			this.m_tileRenderer.material = this.m_tileLegendaryMaterial;
			this.m_legendaryFX.SetActive(true);
		}
		bool flag = this.m_questName.isHidden();
		if (flag)
		{
			this.m_questName.Show();
		}
		this.m_questName.Text = quest.Name;
		TransformUtil.SetPoint(this.m_nameLine, Anchor.TOP, this.m_questName, Anchor.BOTTOM);
		this.m_nameLine.transform.localPosition = new Vector3(this.m_nameLine.transform.localPosition.x, this.m_nameLine.transform.localPosition.y, this.m_nameLine.transform.localPosition.z + this.m_nameLinePadding);
		if (flag)
		{
			this.m_questName.Hide();
		}
		this.m_requirement.Text = quest.Description;
		this.LoadCenterImage();
	}

	// Token: 0x06003DA8 RID: 15784 RVA: 0x0012A2BF File Offset: 0x001284BF
	public void SetCanShowCancelButton(bool canShowCancel)
	{
		this.m_canShowCancelButton = canShowCancel;
		this.UpdateCancelButtonVisibility();
	}

	// Token: 0x06003DA9 RID: 15785 RVA: 0x0012A2D0 File Offset: 0x001284D0
	public void UpdateCancelButtonVisibility()
	{
		bool active = false;
		if (this.m_canShowCancelButton && this.m_quest != null)
		{
			active = AchieveManager.Get().CanCancelQuest(this.m_quest.ID);
		}
		this.m_cancelButtonRoot.gameObject.SetActive(active);
	}

	// Token: 0x06003DAA RID: 15786 RVA: 0x0012A31C File Offset: 0x0012851C
	public int GetQuestID()
	{
		if (this.m_quest == null)
		{
			return 0;
		}
		return this.m_quest.ID;
	}

	// Token: 0x06003DAB RID: 15787 RVA: 0x0012A336 File Offset: 0x00128536
	public void PlayBirth()
	{
		this.m_BurnPlaymaker.SendEvent("Birth");
	}

	// Token: 0x06003DAC RID: 15788 RVA: 0x0012A348 File Offset: 0x00128548
	private void LoadCenterImage()
	{
		if (this.m_quest.Rewards == null || this.m_quest.Rewards.Count == 0 || this.m_quest.Rewards[0] == null)
		{
			Debug.LogError("QuestTile.LoadCenterImage() - This quest doesn't grant a reward!!!");
			return;
		}
		RewardData rewardData = this.m_quest.Rewards[0];
		Vector2 zero = Vector2.zero;
		Vector3 zero2 = Vector3.zero;
		this.m_rewardIcon.transform.localPosition = this.m_defaultBone.transform.localPosition;
		switch (rewardData.RewardType)
		{
		case Reward.Type.BOOSTER_PACK:
		{
			BoosterPackRewardData boosterPackRewardData = rewardData as BoosterPackRewardData;
			if (boosterPackRewardData.Id == 11 && boosterPackRewardData.Count > 1)
			{
				zero..ctor(0f, 0.5f);
			}
			else
			{
				zero..ctor(0f, 0.75f);
			}
			break;
		}
		case Reward.Type.CARD:
			zero..ctor(0.5f, 0f);
			break;
		case Reward.Type.FORGE_TICKET:
			zero..ctor(0.75f, 0.75f);
			zero2..ctor(0.9192683f, 0.9192683f, 0.9192683f);
			break;
		case Reward.Type.GOLD:
		{
			zero..ctor(0.25f, 0.75f);
			GoldRewardData goldRewardData = (GoldRewardData)rewardData;
			this.m_goldAmount.Text = goldRewardData.Amount.ToString();
			this.m_goldAmount.gameObject.SetActive(true);
			break;
		}
		case Reward.Type.MOUNT:
		{
			long num = 0L;
			if (this.m_quest.Rewards.Count > 1 && this.m_quest.Rewards[1].RewardType == Reward.Type.GOLD)
			{
				num = (this.m_quest.Rewards[1] as GoldRewardData).Amount;
			}
			zero..ctor(0.25f, 0.75f);
			this.m_goldAmount.Text = num.ToString();
			this.m_goldAmount.gameObject.SetActive(true);
			break;
		}
		}
		if (zero2 != Vector3.zero)
		{
			this.m_rewardIcon.transform.localScale = zero2;
		}
		this.m_rewardIcon.GetComponent<Renderer>().material.mainTextureOffset = zero;
	}

	// Token: 0x06003DAD RID: 15789 RVA: 0x0012A5AD File Offset: 0x001287AD
	private void OnCancelButtonReleased(UIEvent e)
	{
		if (this.m_quest == null)
		{
			return;
		}
		AchieveManager.Get().CancelQuest(this.m_quest.ID);
		this.m_BurnPlaymaker.SendEvent("Death");
	}

	// Token: 0x0400272F RID: 10031
	public UberText m_goldAmount;

	// Token: 0x04002730 RID: 10032
	public GameObject m_defaultBone;

	// Token: 0x04002731 RID: 10033
	public UberText m_requirement;

	// Token: 0x04002732 RID: 10034
	public UberText m_questName;

	// Token: 0x04002733 RID: 10035
	public GameObject m_nameLine;

	// Token: 0x04002734 RID: 10036
	public GameObject m_progress;

	// Token: 0x04002735 RID: 10037
	public UberText m_progressText;

	// Token: 0x04002736 RID: 10038
	public GameObject m_rewardIcon;

	// Token: 0x04002737 RID: 10039
	public NormalButton m_cancelButton;

	// Token: 0x04002738 RID: 10040
	public GameObject m_cancelButtonRoot;

	// Token: 0x04002739 RID: 10041
	public PlayMakerFSM m_BurnPlaymaker;

	// Token: 0x0400273A RID: 10042
	public float m_nameLinePadding = 0.22f;

	// Token: 0x0400273B RID: 10043
	public GameObject m_legendaryFX;

	// Token: 0x0400273C RID: 10044
	public MeshRenderer m_tileRenderer;

	// Token: 0x0400273D RID: 10045
	public Material m_tileLegendaryMaterial;

	// Token: 0x0400273E RID: 10046
	private Achievement m_quest;

	// Token: 0x0400273F RID: 10047
	private bool m_canShowCancelButton;
}
