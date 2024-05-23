using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005B0 RID: 1456
public class GameToastMgr : MonoBehaviour
{
	// Token: 0x06004140 RID: 16704 RVA: 0x0013AF2C File Offset: 0x0013912C
	private void Awake()
	{
		GameToastMgr.s_instance = this;
	}

	// Token: 0x06004141 RID: 16705 RVA: 0x0013AF34 File Offset: 0x00139134
	private void OnDestroy()
	{
		GameToastMgr.s_instance = null;
	}

	// Token: 0x06004142 RID: 16706 RVA: 0x0013AF3C File Offset: 0x0013913C
	public static GameToastMgr Get()
	{
		return GameToastMgr.s_instance;
	}

	// Token: 0x06004143 RID: 16707 RVA: 0x0013AF44 File Offset: 0x00139144
	private void AddToast(UserAttentionBlocker blocker, GameToast toast, string callerName)
	{
		if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "GameToastMgr." + callerName))
		{
			return;
		}
		toast.transform.parent = BnetBar.Get().m_questProgressToastBone.transform;
		toast.transform.localRotation = Quaternion.Euler(new Vector3(90f, 180f, 0f));
		toast.transform.localScale = new Vector3(450f, 1f, 450f);
		toast.transform.localPosition = Vector3.zero;
		this.m_toasts.Add(toast);
		RenderUtils.SetAlpha(toast.gameObject, 0f);
		this.UpdateToastPositions();
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			1f,
			"time",
			0.25f,
			"delay",
			0.25f,
			"easeType",
			iTween.EaseType.easeInCubic,
			"oncomplete",
			"FadeOutToast",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			toast
		});
		iTween.FadeTo(toast.gameObject, args);
	}

	// Token: 0x06004144 RID: 16708 RVA: 0x0013B092 File Offset: 0x00139292
	public bool AreToastsActive()
	{
		return this.m_toasts.Count > 0;
	}

	// Token: 0x06004145 RID: 16709 RVA: 0x0013B0A4 File Offset: 0x001392A4
	private void FadeOutToast(GameToast toast)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			0f,
			"delay",
			4f,
			"time",
			0.25f,
			"easeType",
			iTween.EaseType.easeInCubic,
			"oncomplete",
			"DeactivateToast",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			toast
		});
		iTween.FadeTo(toast.gameObject, args);
	}

	// Token: 0x06004146 RID: 16710 RVA: 0x0013B14C File Offset: 0x0013934C
	private void DeactivateToast(GameToast toast)
	{
		toast.gameObject.SetActive(false);
		this.m_toasts.Remove(toast);
		this.UpdateToastPositions();
	}

	// Token: 0x06004147 RID: 16711 RVA: 0x0013B170 File Offset: 0x00139370
	private void UpdateToastPositions()
	{
		int num = 0;
		foreach (GameToast gameToast in this.m_toasts)
		{
			if (num > 0)
			{
				TransformUtil.SetPoint(gameToast.gameObject, Anchor.BOTTOM, this.m_toasts[num - 1].gameObject, Anchor.TOP, new Vector3(0f, 5f, 0f));
			}
			num++;
		}
	}

	// Token: 0x06004148 RID: 16712 RVA: 0x0013B204 File Offset: 0x00139404
	public void UpdateQuestProgressToasts()
	{
		if (!UserAttentionManager.CanShowAttentionGrabber("GameToastMgr.UpdateQuestProgressToasts"))
		{
			return;
		}
		List<Achievement> newlyProgressedQuests = AchieveManager.Get().GetNewlyProgressedQuests();
		foreach (Achievement achievement in newlyProgressedQuests)
		{
			this.AddQuestProgressToast(achievement.Name, achievement.Description, achievement.Progress, achievement.MaxProgress);
			achievement.AckCurrentProgressAndRewardNotices(true);
		}
	}

	// Token: 0x06004149 RID: 16713 RVA: 0x0013B294 File Offset: 0x00139494
	public void AddQuestProgressToast(string questName, string questDescription, int progress, int maxProgress)
	{
		if (progress == maxProgress)
		{
			return;
		}
		QuestProgressToast questProgressToast = Object.Instantiate<QuestProgressToast>(this.m_questProgressToastPrefab);
		questProgressToast.UpdateDisplay(questName, questDescription, progress, maxProgress);
		this.AddToast(UserAttentionBlocker.NONE, questProgressToast, "AddQuestProgressToast");
	}

	// Token: 0x0600414A RID: 16714 RVA: 0x0013B2D0 File Offset: 0x001394D0
	public void AddSeasonTimeRemainingToast()
	{
		if (!UserAttentionManager.CanShowAttentionGrabber("GameToastMgr.AddSeasonTimeRemainingToast"))
		{
			return;
		}
		NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
		if (netObject == null)
		{
			return;
		}
		DateTime dateTime = DateTime.FromFileTimeUtc(netObject.SeasonEndDate);
		int num = (int)(dateTime - DateTime.UtcNow).TotalSeconds;
		int num2;
		if (num < 86400)
		{
			num2 = 1;
		}
		else
		{
			num2 = num / 86400;
		}
		int @int = Options.Get().GetInt(Option.SEASON_END_THRESHOLD);
		if (num2 == @int)
		{
			return;
		}
		int num3 = -1;
		switch (num2)
		{
		case 1:
			num3 = 1;
			break;
		case 2:
			num3 = 2;
			break;
		case 3:
			num3 = 3;
			break;
		case 4:
			num3 = 4;
			break;
		case 5:
			num3 = 5;
			break;
		case 10:
			num3 = 10;
			break;
		}
		if (num3 == -1)
		{
			return;
		}
		Options.Get().SetInt(Option.SEASON_END_THRESHOLD, num3);
		if (num2 < 5)
		{
			if (SceneMgr.Get().GetMode() != SceneMgr.Mode.TOURNAMENT)
			{
				return;
			}
			if (!Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE))
			{
				return;
			}
		}
		string text;
		if (num < 86400)
		{
			text = TimeUtils.GetElapsedTimeString(num, TimeUtils.SPLASHSCREEN_DATETIME_STRINGSET);
		}
		else
		{
			text = GameStrings.Format(TimeUtils.SPLASHSCREEN_DATETIME_STRINGSET.m_days, new object[]
			{
				num2
			});
		}
		SeasonDbfRecord record = GameDbf.Season.GetRecord(netObject.Season);
		if (record == null)
		{
			return;
		}
		string title = record.Name;
		SeasonTimeRemainingToast seasonTimeRemainingToast = Object.Instantiate<SeasonTimeRemainingToast>(this.m_seasonTimeRemainingToastPrefab);
		seasonTimeRemainingToast.UpdateDisplay(title, GameStrings.Format("GLOBAL_REMAINING_DATETIME", new object[]
		{
			text
		}));
		this.AddToast(UserAttentionBlocker.NONE, seasonTimeRemainingToast, "AddSeasonTimeRemainingToast");
	}

	// Token: 0x04002979 RID: 10617
	private const float FADE_IN_TIME = 0.25f;

	// Token: 0x0400297A RID: 10618
	private const float FADE_OUT_TIME = 0.5f;

	// Token: 0x0400297B RID: 10619
	private const float HOLD_TIME = 4f;

	// Token: 0x0400297C RID: 10620
	private const int MAX_DAYS_TO_SHOW_TIME_REMAINING = 5;

	// Token: 0x0400297D RID: 10621
	public QuestProgressToast m_questProgressToastPrefab;

	// Token: 0x0400297E RID: 10622
	public SeasonTimeRemainingToast m_seasonTimeRemainingToastPrefab;

	// Token: 0x0400297F RID: 10623
	private static GameToastMgr s_instance;

	// Token: 0x04002980 RID: 10624
	private List<GameToast> m_toasts = new List<GameToast>();

	// Token: 0x04002981 RID: 10625
	private int m_numToastsShown;

	// Token: 0x02000814 RID: 2068
	public enum SEASON_TOAST_THRESHHOLDS
	{
		// Token: 0x040036A5 RID: 13989
		NONE,
		// Token: 0x040036A6 RID: 13990
		ONE,
		// Token: 0x040036A7 RID: 13991
		TWO,
		// Token: 0x040036A8 RID: 13992
		THREE,
		// Token: 0x040036A9 RID: 13993
		FOUR,
		// Token: 0x040036AA RID: 13994
		FIVE,
		// Token: 0x040036AB RID: 13995
		TEN = 10
	}

	// Token: 0x02000815 RID: 2069
	public enum TOAST_TYPE
	{
		// Token: 0x040036AD RID: 13997
		NORMAL,
		// Token: 0x040036AE RID: 13998
		NO_COUNT
	}
}
