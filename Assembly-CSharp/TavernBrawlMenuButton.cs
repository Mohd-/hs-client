using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class TavernBrawlMenuButton : BoxMenuButton
{
	// Token: 0x06002223 RID: 8739 RVA: 0x000A7ED0 File Offset: 0x000A60D0
	public override void TriggerOver()
	{
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (!netObject.Games.TavernBrawl || !TavernBrawlManager.Get().HasUnlockedTavernBrawl || TavernBrawlManager.Get().IsTavernBrawlActive)
		{
			base.TriggerOver();
			return;
		}
		this.UpdateTimeText();
		base.StartCoroutine("DoPopup");
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x000A7F30 File Offset: 0x000A6130
	public IEnumerator DoPopup()
	{
		if (!UniversalInputManager.Get().IsTouchMode())
		{
			yield return new WaitForSeconds(this.m_hoverDelay);
		}
		this.isPoppedUp = true;
		if (Box.Get().m_tavernBrawlPopupSound != string.Empty)
		{
			SoundManager.Get().LoadAndPlay(Box.Get().m_tavernBrawlPopupSound);
		}
		Animator tbAnim = Box.Get().m_TavernBrawlButtonVisual.GetComponent<Animator>();
		tbAnim.Play("TavernBrawl_ButtonPopup");
		yield return null;
		yield break;
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x000A7F4C File Offset: 0x000A614C
	public override void TriggerOut()
	{
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (!netObject.Games.TavernBrawl || !TavernBrawlManager.Get().HasUnlockedTavernBrawl || TavernBrawlManager.Get().IsTavernBrawlActive)
		{
			base.TriggerOut();
			return;
		}
		if (!UniversalInputManager.Get().IsTouchMode())
		{
			base.StopCoroutine("DoPopup");
		}
		if (this.isPoppedUp)
		{
			if (Box.Get().m_tavernBrawlPopdownSound != string.Empty)
			{
				SoundManager.Get().LoadAndPlay(Box.Get().m_tavernBrawlPopdownSound);
			}
			Animator component = Box.Get().m_TavernBrawlButtonVisual.GetComponent<Animator>();
			component.Play("TavernBrawl_ButtonPopdown");
			this.isPoppedUp = false;
		}
	}

	// Token: 0x06002226 RID: 8742 RVA: 0x000A800D File Offset: 0x000A620D
	public void ClearHighlightAndTooltip()
	{
		base.TriggerOut();
	}

	// Token: 0x06002227 RID: 8743 RVA: 0x000A8018 File Offset: 0x000A6218
	public override void TriggerPress()
	{
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (netObject.Games.TavernBrawl && TavernBrawlManager.Get().HasUnlockedTavernBrawl && TavernBrawlManager.Get().IsTavernBrawlActive)
		{
			base.TriggerPress();
			return;
		}
	}

	// Token: 0x06002228 RID: 8744 RVA: 0x000A8068 File Offset: 0x000A6268
	public override void TriggerRelease()
	{
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (netObject.Games.TavernBrawl && TavernBrawlManager.Get().HasUnlockedTavernBrawl && TavernBrawlManager.Get().IsTavernBrawlActive)
		{
			base.TriggerRelease();
			return;
		}
	}

	// Token: 0x06002229 RID: 8745 RVA: 0x000A80B8 File Offset: 0x000A62B8
	private void UpdateTimeText()
	{
		int nextTavernBrawlSeasonStart = TavernBrawlManager.Get().NextTavernBrawlSeasonStart;
		if (nextTavernBrawlSeasonStart < 0)
		{
			this.m_returnsInfo.Text = GameStrings.Get("GLUE_TAVERN_BRAWL_RETURNS_UNKNOWN");
			return;
		}
		TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet
		{
			m_seconds = "GLUE_TAVERN_BRAWL_RETURNS_LESS_THAN_1_HOUR",
			m_minutes = "GLUE_TAVERN_BRAWL_RETURNS_LESS_THAN_1_HOUR",
			m_hours = "GLUE_TAVERN_BRAWL_RETURNS_HOURS",
			m_days = "GLUE_TAVERN_BRAWL_RETURNS_DAYS",
			m_weeks = "GLUE_TAVERN_BRAWL_RETURNS_WEEKS",
			m_monthAgo = "GLUE_TAVERN_BRAWL_RETURNS_OVER_1_MONTH"
		};
		string elapsedTimeString = TimeUtils.GetElapsedTimeString(nextTavernBrawlSeasonStart, stringSet);
		this.m_returnsInfo.Text = elapsedTimeString;
	}

	// Token: 0x04001388 RID: 5000
	public UberText m_returnsInfo;

	// Token: 0x04001389 RID: 5001
	public float m_hoverDelay = 0.5f;

	// Token: 0x0400138A RID: 5002
	private bool isPoppedUp;
}
