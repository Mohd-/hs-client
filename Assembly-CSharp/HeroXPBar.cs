using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000397 RID: 919
public class HeroXPBar : PegUIElement
{
	// Token: 0x06002FED RID: 12269 RVA: 0x000F1054 File Offset: 0x000EF254
	protected override void Awake()
	{
		base.Awake();
		this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnProgressBarOver));
		this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnProgressBarOut));
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x000F108F File Offset: 0x000EF28F
	public void EmptyLevelUpFunction()
	{
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x000F1094 File Offset: 0x000EF294
	public void UpdateDisplay()
	{
		if (this.m_isOnDeck)
		{
			this.m_simpleFrame.SetActive(true);
			this.m_heroFrame.SetActive(false);
		}
		else
		{
			this.m_simpleFrame.SetActive(false);
			this.m_heroFrame.SetActive(true);
		}
		this.SetBarText(string.Empty);
		if (this.m_isAnimated)
		{
			this.m_heroLevelText.Text = this.m_heroLevel.PrevLevel.Level.ToString();
			this.SetBarValue((float)this.m_heroLevel.PrevLevel.XP / (float)this.m_heroLevel.PrevLevel.MaxXP);
			base.StartCoroutine(this.DelayBarAnimation(this.m_heroLevel.PrevLevel, this.m_heroLevel.CurrentLevel));
		}
		else
		{
			this.m_heroLevelText.Text = this.m_heroLevel.CurrentLevel.Level.ToString();
			if (this.m_heroLevel.CurrentLevel.IsMaxLevel())
			{
				this.SetBarValue(1f);
			}
			else
			{
				this.SetBarValue((float)this.m_heroLevel.CurrentLevel.XP / (float)this.m_heroLevel.CurrentLevel.MaxXP);
			}
		}
	}

	// Token: 0x06002FF0 RID: 12272 RVA: 0x000F11DC File Offset: 0x000EF3DC
	public void AnimateBar(NetCache.HeroLevel.LevelInfo previousLevelInfo, NetCache.HeroLevel.LevelInfo currentLevelInfo)
	{
		this.m_heroLevelText.Text = previousLevelInfo.Level.ToString();
		if (previousLevelInfo.Level < currentLevelInfo.Level)
		{
			float prevVal = (float)previousLevelInfo.XP / (float)previousLevelInfo.MaxXP;
			float currVal = 1f;
			this.m_progressBar.AnimateProgress(prevVal, currVal);
			float animationTime = this.m_progressBar.GetAnimationTime();
			base.StartCoroutine(this.AnimatePostLevelUpXp(animationTime, currentLevelInfo));
		}
		else
		{
			float prevVal2 = (float)previousLevelInfo.XP / (float)previousLevelInfo.MaxXP;
			float currVal2 = (float)currentLevelInfo.XP / (float)currentLevelInfo.MaxXP;
			if (currentLevelInfo.IsMaxLevel())
			{
				currVal2 = 1f;
			}
			this.m_progressBar.AnimateProgress(prevVal2, currVal2);
		}
	}

	// Token: 0x06002FF1 RID: 12273 RVA: 0x000F1298 File Offset: 0x000EF498
	public void SetBarValue(float barValue)
	{
		this.m_progressBar.SetProgressBar(barValue);
	}

	// Token: 0x06002FF2 RID: 12274 RVA: 0x000F12A6 File Offset: 0x000EF4A6
	public void SetBarText(string barText)
	{
		if (this.m_barText != null)
		{
			this.m_barText.Text = barText;
		}
	}

	// Token: 0x06002FF3 RID: 12275 RVA: 0x000F12C8 File Offset: 0x000EF4C8
	private IEnumerator AnimatePostLevelUpXp(float delayTime, NetCache.HeroLevel.LevelInfo currentLevelInfo)
	{
		yield return new WaitForSeconds(delayTime);
		if (currentLevelInfo.Level == 3 && !Options.Get().GetBool(Option.HAS_SEEN_LEVEL_3, false) && UserAttentionManager.CanShowAttentionGrabber("HeroXPBar.AnimatePostLevelUpXp:" + Option.HAS_SEEN_LEVEL_3))
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_LEVEL3_TIP"), "VO_INNKEEPER_LEVEL3_TIP", 0f, null);
			Options.Get().SetBool(Option.HAS_SEEN_LEVEL_3, true);
		}
		this.m_heroLevelText.Text = currentLevelInfo.Level.ToString();
		float targetXP = (float)currentLevelInfo.XP / (float)currentLevelInfo.MaxXP;
		this.m_progressBar.AnimateProgress(0f, targetXP);
		if (this.m_levelUpCallback != null)
		{
			this.m_levelUpCallback();
		}
		yield break;
	}

	// Token: 0x06002FF4 RID: 12276 RVA: 0x000F1300 File Offset: 0x000EF500
	private IEnumerator DelayBarAnimation(NetCache.HeroLevel.LevelInfo prevInfo, NetCache.HeroLevel.LevelInfo currInfo)
	{
		yield return new WaitForSeconds(this.m_delay);
		this.AnimateBar(prevInfo, currInfo);
		yield break;
	}

	// Token: 0x06002FF5 RID: 12277 RVA: 0x000F1338 File Offset: 0x000EF538
	private void ShowTooltip()
	{
		TooltipZone component = base.gameObject.GetComponent<TooltipZone>();
		float num;
		if (SceneMgr.Get().IsInGame())
		{
			num = KeywordHelpPanel.MULLIGAN_SCALE;
		}
		else if (UniversalInputManager.UsePhoneUI)
		{
			num = KeywordHelpPanel.BOX_SCALE;
		}
		else
		{
			num = KeywordHelpPanel.COLLECTION_MANAGER_SCALE;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			num *= 1.1f;
		}
		component.ShowTooltip(GameStrings.Format("GLOBAL_HERO_LEVEL_NEXT_REWARD_TITLE", new object[]
		{
			this.m_heroLevel.NextReward.Level
		}), this.m_rewardText, num, true);
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x000F13E8 File Offset: 0x000EF5E8
	private void OnProgressBarOver(UIEvent e)
	{
		if (this.m_heroLevel == null)
		{
			return;
		}
		if (this.m_heroLevel.NextReward == null)
		{
			return;
		}
		if (this.m_heroLevel.NextReward.Reward == null)
		{
			return;
		}
		RewardData reward = this.m_heroLevel.NextReward.Reward;
		this.m_rewardText = RewardUtils.GetRewardText(reward);
		this.ShowTooltip();
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x000F144C File Offset: 0x000EF64C
	private void OnProgressBarOut(UIEvent e)
	{
		TooltipZone component = base.gameObject.GetComponent<TooltipZone>();
		component.HideTooltip();
	}

	// Token: 0x04001DC5 RID: 7621
	public ProgressBar m_progressBar;

	// Token: 0x04001DC6 RID: 7622
	public UberText m_heroLevelText;

	// Token: 0x04001DC7 RID: 7623
	public UberText m_barText;

	// Token: 0x04001DC8 RID: 7624
	public GameObject m_simpleFrame;

	// Token: 0x04001DC9 RID: 7625
	public GameObject m_heroFrame;

	// Token: 0x04001DCA RID: 7626
	public bool m_isAnimated;

	// Token: 0x04001DCB RID: 7627
	public float m_delay;

	// Token: 0x04001DCC RID: 7628
	public bool m_isOnDeck;

	// Token: 0x04001DCD RID: 7629
	public NetCache.HeroLevel m_heroLevel;

	// Token: 0x04001DCE RID: 7630
	public int m_soloLevelLimit;

	// Token: 0x04001DCF RID: 7631
	public HeroXPBar.PlayLevelUpEffectCallback m_levelUpCallback;

	// Token: 0x04001DD0 RID: 7632
	private string m_rewardText;

	// Token: 0x020007E2 RID: 2018
	// (Invoke) Token: 0x06004E91 RID: 20113
	public delegate void PlayLevelUpEffectCallback();
}
