using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000441 RID: 1089
public class AdventureMissionDisplayTray : MonoBehaviour
{
	// Token: 0x06003653 RID: 13907 RVA: 0x0010BCC4 File Offset: 0x00109EC4
	private void Awake()
	{
		AdventureConfig adventureConfig = AdventureConfig.Get();
		adventureConfig.AddAdventureMissionSetListener(new AdventureConfig.AdventureMissionSet(this.OnMissionSelected));
		adventureConfig.AddSubSceneChangeListener(new AdventureConfig.SubSceneChange(this.OnSubsceneChanged));
		if (this.m_rewardsChest != null)
		{
			this.m_rewardsChest.AddEventListener(UIEventType.ROLLOVER, delegate(UIEvent e)
			{
				this.ShowCardRewards();
			});
			this.m_rewardsChest.AddEventListener(UIEventType.ROLLOUT, delegate(UIEvent e)
			{
				this.HideCardRewards();
			});
		}
	}

	// Token: 0x06003654 RID: 13908 RVA: 0x0010BD40 File Offset: 0x00109F40
	private void OnDestroy()
	{
		AdventureConfig adventureConfig = AdventureConfig.Get();
		if (adventureConfig != null)
		{
			adventureConfig.RemoveAdventureMissionSetListener(new AdventureConfig.AdventureMissionSet(this.OnMissionSelected));
			adventureConfig.RemoveSubSceneChangeListener(new AdventureConfig.SubSceneChange(this.OnSubsceneChanged));
		}
	}

	// Token: 0x06003655 RID: 13909 RVA: 0x0010BD84 File Offset: 0x00109F84
	private void OnMissionSelected(ScenarioDbId mission, bool showDetails)
	{
		if (mission == ScenarioDbId.INVALID)
		{
			return;
		}
		if (showDetails)
		{
			this.m_slidingTray.ToggleTraySlider(true, null, true);
		}
		List<CardRewardData> immediateCardRewardsForDefeatingScenario = AdventureProgressMgr.Get().GetImmediateCardRewardsForDefeatingScenario((int)mission);
		bool flag = AdventureProgressMgr.Get().HasDefeatedScenario((int)mission);
		this.m_rewardsChest.gameObject.SetActive(immediateCardRewardsForDefeatingScenario != null && immediateCardRewardsForDefeatingScenario.Count != 0 && !flag);
	}

	// Token: 0x06003656 RID: 13910 RVA: 0x0010BDEC File Offset: 0x00109FEC
	private void ShowCardRewards()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			NotificationManager.Get().DestroyActiveQuote(0.2f);
		}
		List<CardRewardData> immediateCardRewardsForDefeatingScenario = AdventureProgressMgr.Get().GetImmediateCardRewardsForDefeatingScenario((int)AdventureConfig.Get().GetMission());
		this.m_rewardsDisplay.ShowCardsNoFullscreen(immediateCardRewardsForDefeatingScenario, this.m_rewardsDisplayBone.position, new Vector3?(this.m_rewardsChest.transform.position));
	}

	// Token: 0x06003657 RID: 13911 RVA: 0x0010BE58 File Offset: 0x0010A058
	private void HideCardRewards()
	{
		this.m_rewardsDisplay.HideCardRewards();
	}

	// Token: 0x06003658 RID: 13912 RVA: 0x0010BE65 File Offset: 0x0010A065
	private void OnSubsceneChanged(AdventureSubScenes newscene, bool forward)
	{
		this.m_slidingTray.ToggleTraySlider(false, null, true);
	}

	// Token: 0x040021D2 RID: 8658
	public SlidingTray m_slidingTray;

	// Token: 0x040021D3 RID: 8659
	public PegUIElement m_rewardsChest;

	// Token: 0x040021D4 RID: 8660
	public AdventureRewardsDisplayArea m_rewardsDisplay;

	// Token: 0x040021D5 RID: 8661
	public Transform m_rewardsDisplayBone;
}
