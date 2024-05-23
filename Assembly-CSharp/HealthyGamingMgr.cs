using System;
using System.Collections;
using System.Collections.Generic;
using bgs;
using bgs.types;
using UnityEngine;

// Token: 0x02000269 RID: 617
public class HealthyGamingMgr : MonoBehaviour
{
	// Token: 0x060022B1 RID: 8881 RVA: 0x000AAE7C File Offset: 0x000A907C
	private void Awake()
	{
		HealthyGamingMgr.s_Instance = this;
		if (Options.Get().GetBool(Option.HEALTHY_GAMING_DEBUG, false))
		{
			this.m_DebugMode = true;
		}
		this.m_NextCheckTime = Time.realtimeSinceStartup + 45f;
		ApplicationMgr.Get().WillReset += new Action(this.WillReset);
		ApplicationMgr.Get().Resetting += new Action(this.OnReset);
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		base.StartCoroutine(this.InitNetworkData());
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x000AAF0C File Offset: 0x000A910C
	private void Update()
	{
		if (!this.m_NetworkDataReady)
		{
			return;
		}
		if (Time.realtimeSinceStartup < this.m_NextCheckTime)
		{
			return;
		}
		this.m_NextCheckTime = Time.realtimeSinceStartup + 300f;
		string accountCountry = this.m_AccountCountry;
		if (accountCountry != null)
		{
			if (HealthyGamingMgr.<>f__switch$map8A == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("CHN", 0);
				dictionary.Add("KOR", 1);
				HealthyGamingMgr.<>f__switch$map8A = dictionary;
			}
			int num;
			if (HealthyGamingMgr.<>f__switch$map8A.TryGetValue(accountCountry, ref num))
			{
				if (num == 0)
				{
					this.ChinaRestrictions();
					return;
				}
				if (num == 1)
				{
					this.KoreaRestrictions();
					return;
				}
			}
		}
		base.enabled = false;
	}

	// Token: 0x060022B3 RID: 8883 RVA: 0x000AAFCC File Offset: 0x000A91CC
	private void OnDestroy()
	{
		ApplicationMgr.Get().WillReset -= new Action(this.WillReset);
		ApplicationMgr.Get().Resetting -= new Action(this.OnReset);
		FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		HealthyGamingMgr.s_Instance = null;
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x000AB022 File Offset: 0x000A9222
	public static HealthyGamingMgr Get()
	{
		return HealthyGamingMgr.s_Instance;
	}

	// Token: 0x060022B5 RID: 8885 RVA: 0x000AB029 File Offset: 0x000A9229
	public void OnLoggedIn()
	{
		this.m_BattleNetReady = true;
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x000AB032 File Offset: 0x000A9232
	public bool isArenaEnabled()
	{
		return this.m_HealthyGamingArenaEnabled;
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x000AB03A File Offset: 0x000A923A
	public ulong GetSessionStartTime()
	{
		return this.m_SessionStartTime;
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000AB042 File Offset: 0x000A9242
	private void WillReset()
	{
		this.StopCoroutinesAndResetState();
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x000AB04A File Offset: 0x000A924A
	private void OnReset()
	{
		base.StartCoroutine(this.InitNetworkData());
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x000AB059 File Offset: 0x000A9259
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		this.StopCoroutinesAndResetState();
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x000AB061 File Offset: 0x000A9261
	private void StopCoroutinesAndResetState()
	{
		this.m_BattleNetReady = false;
		this.m_NetworkDataReady = false;
		base.StopAllCoroutines();
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x000AB078 File Offset: 0x000A9278
	private IEnumerator InitNetworkData()
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			yield break;
		}
		while (!this.m_BattleNetReady)
		{
			yield return null;
		}
		this.m_AccountCountry = BattleNet.GetAccountCountry();
		if (this.m_AccountCountry != "CHN" && this.m_AccountCountry != "KOR")
		{
			base.enabled = false;
		}
		this.m_Restrictions = default(Lockouts);
		BattleNet.GetPlayRestrictions(ref this.m_Restrictions, true);
		while (!this.m_Restrictions.loaded)
		{
			BattleNet.GetPlayRestrictions(ref this.m_Restrictions, false);
			yield return null;
		}
		this.m_SessionStartTime = this.m_Restrictions.sessionStartTime;
		this.m_TimePlayed = this.m_Restrictions.CAISplayed;
		this.m_TimeRested = this.m_Restrictions.CAISrested;
		if (this.m_DebugMode)
		{
			Log.HealthyGaming.Print("Healthy Gaming Debug ON", new object[0]);
			Log.HealthyGaming.Print("CAIS Active = " + this.m_Restrictions.CAISactive.ToString(), new object[0]);
			Log.HealthyGaming.Print("Accout Country: " + BattleNet.GetAccountCountry(), new object[0]);
			Log.HealthyGaming.Print("Accout Region: " + BattleNet.GetAccountRegion().ToString(), new object[0]);
			Log.HealthyGaming.Print("Current Region: " + BattleNet.GetCurrentRegion().ToString(), new object[0]);
			Log.HealthyGaming.Print("Session StartTime " + this.m_SessionStartTime, new object[0]);
		}
		else
		{
			Log.HealthyGaming.Print(string.Format("CAIS Active: {0},  Accout Country: {1},  Time: {2},  Played Time: {3},  Rested Time: {4}, Session Start Time: {5}", new object[]
			{
				this.m_Restrictions.CAISactive.ToString(),
				BattleNet.GetAccountCountry(),
				Time.realtimeSinceStartup,
				this.m_Restrictions.CAISplayed,
				this.m_Restrictions.CAISrested,
				this.m_Restrictions.sessionStartTime
			}), new object[0]);
		}
		if (!this.m_Restrictions.CAISactive && this.m_AccountCountry == "CHN")
		{
			base.enabled = false;
			yield break;
		}
		if (this.m_DebugMode)
		{
			Log.HealthyGaming.Print("Healthy Gaming Active!", new object[0]);
		}
		if (this.m_AccountCountry == "KOR")
		{
			this.m_NextMessageDisplayTime = Time.realtimeSinceStartup + 3600f;
		}
		if (this.m_AccountCountry == "CHN")
		{
			string message = GameStrings.Get("GLOBAL_HEALTHY_GAMING_CHINA_CAIS_ACTIVE");
			SocialToastMgr.Get().AddToast(UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE, message, SocialToastMgr.TOAST_TYPE.DEFAULT, 60f);
			this.m_NextMessageDisplayTime = -2f;
		}
		this.m_NetworkDataReady = true;
		yield break;
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x000AB094 File Offset: 0x000A9294
	private void KoreaRestrictions()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (this.m_DebugMode)
		{
			Log.HealthyGaming.Print("Minutes Played: " + realtimeSinceStartup / 60f, new object[0]);
		}
		if (realtimeSinceStartup < this.m_NextMessageDisplayTime)
		{
			return;
		}
		this.m_NextMessageDisplayTime += 3600f;
		int num = (int)(realtimeSinceStartup / 60f) / 60;
		SocialToastMgr.Get().AddToast(UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE, GameStrings.Format("GLOBAL_HEALTHY_GAMING_TOAST", new object[]
		{
			num
		}), SocialToastMgr.TOAST_TYPE.DEFAULT, 5f);
	}

	// Token: 0x060022BE RID: 8894 RVA: 0x000AB12E File Offset: 0x000A932E
	private void ChinaRestrictions()
	{
		BattleNet.GetPlayRestrictions(ref this.m_Restrictions, true);
		base.StartCoroutine(this.ChinaRestrictionsUpdate());
	}

	// Token: 0x060022BF RID: 8895 RVA: 0x000AB14C File Offset: 0x000A934C
	private IEnumerator ChinaRestrictionsUpdate()
	{
		Log.Kyle.Print("ChinaRestrictionsUpdate()", new object[0]);
		while (!this.m_Restrictions.loaded)
		{
			BattleNet.GetPlayRestrictions(ref this.m_Restrictions, false);
			yield return null;
		}
		this.m_TimePlayed = this.m_Restrictions.CAISplayed;
		this.m_TimeRested = this.m_Restrictions.CAISrested;
		int minutesPlayed = this.m_TimePlayed;
		if (this.m_DebugMode)
		{
			Log.HealthyGaming.Print(string.Format("CAIS Time Played: {0}    Rested: {1}", this.m_TimePlayed.ToString(), this.m_TimeRested.ToString()), new object[0]);
			Log.HealthyGaming.Print(string.Format("CAIS Minutes Played: {0}", minutesPlayed), new object[0]);
		}
		if (this.m_NextMessageDisplayTime == -2f)
		{
			yield return new WaitForSeconds(60f);
			this.m_NextMessageDisplayTime = -1f;
		}
		if ((float)minutesPlayed < this.m_NextMessageDisplayTime && this.m_NextMessageDisplayTime > 0f)
		{
			yield break;
		}
		int hours = minutesPlayed / 60;
		if (minutesPlayed >= 180)
		{
			this.ChinaRestrictions_LockoutFeatures(minutesPlayed);
		}
		if (minutesPlayed < 180 && minutesPlayed >= 60)
		{
			this.ChinaRestrictions_LessThan3Hours(minutesPlayed, hours);
		}
		if (minutesPlayed >= 180 && minutesPlayed <= 300)
		{
			this.ChinaRestrictions_3to5Hours(minutesPlayed);
		}
		if (minutesPlayed > 300)
		{
			this.ChinaRestrictions_MoreThan5Hours(minutesPlayed);
		}
		yield break;
	}

	// Token: 0x060022C0 RID: 8896 RVA: 0x000AB168 File Offset: 0x000A9368
	private void ChinaRestrictions_LessThan3Hours(int minutesPlayed, int hours)
	{
		if (this.m_NextMessageDisplayTime < 0f)
		{
			this.m_NextMessageDisplayTime = (float)(this.m_TimePlayed + (60 - minutesPlayed % 60));
		}
		else
		{
			this.m_NextMessageDisplayTime = (float)(this.m_TimePlayed + 60);
		}
		string text = GameStrings.Format("GLOBAL_HEALTHY_GAMING_CHINA_LESS_THAN_THREE_HOURS", new object[]
		{
			hours
		});
		SocialToastMgr.Get().AddToast(UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE, text, SocialToastMgr.TOAST_TYPE.DEFAULT, 60f);
		if (this.m_DebugMode)
		{
			Log.HealthyGaming.Print("GLOBAL_HEALTHY_GAMING_CHINA_LESS_THAN_THREE_HOURS: " + minutesPlayed.ToString(), new object[0]);
			Log.HealthyGaming.Print(GameStrings.Format("GLOBAL_HEALTHY_GAMING_CHINA_LESS_THAN_THREE_HOURS", new object[]
			{
				hours
			}), new object[0]);
			Log.HealthyGaming.Print("NextMessageDisplayTime: " + this.m_NextMessageDisplayTime.ToString(), new object[0]);
		}
		else
		{
			Log.HealthyGaming.Print(string.Format("Time: {0},  Played: {1},  {2}", Time.realtimeSinceStartup, minutesPlayed, text), new object[0]);
		}
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x000AB288 File Offset: 0x000A9488
	private void ChinaRestrictions_3to5Hours(int minutesPlayed)
	{
		if (this.m_NextMessageDisplayTime < 0f)
		{
			this.m_NextMessageDisplayTime = (float)(this.m_TimePlayed + (30 - minutesPlayed % 30));
		}
		else
		{
			this.m_NextMessageDisplayTime = (float)(this.m_TimePlayed + 30);
		}
		string text = GameStrings.Get("GLOBAL_HEALTHY_GAMING_CHINA_THREE_TO_FIVE_HOURS");
		SocialToastMgr.Get().AddToast(UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE, text, SocialToastMgr.TOAST_TYPE.DEFAULT, 60f);
		if (this.m_DebugMode)
		{
			Log.HealthyGaming.Print("GLOBAL_HEALTHY_GAMING_CHINA_THREE_TO_FIVE_HOURS: " + minutesPlayed.ToString(), new object[0]);
			Log.HealthyGaming.Print(GameStrings.Get("GLOBAL_HEALTHY_GAMING_CHINA_THREE_TO_FIVE_HOURS"), new object[0]);
			Log.HealthyGaming.Print("NextMessageDisplayTime: " + this.m_NextMessageDisplayTime.ToString(), new object[0]);
		}
		else
		{
			Log.HealthyGaming.Print(string.Format("Time: {0},  Played: {1},  {2}", Time.realtimeSinceStartup, minutesPlayed, text), new object[0]);
		}
	}

	// Token: 0x060022C2 RID: 8898 RVA: 0x000AB388 File Offset: 0x000A9588
	private void ChinaRestrictions_MoreThan5Hours(int minutesPlayed)
	{
		if (this.m_NextMessageDisplayTime < 0f)
		{
			this.m_NextMessageDisplayTime = (float)(this.m_TimePlayed + (15 - minutesPlayed % 15));
		}
		else
		{
			this.m_NextMessageDisplayTime = (float)(this.m_TimePlayed + 15);
		}
		string text = GameStrings.Get("GLOBAL_HEALTHY_GAMING_CHINA_MORE_THAN_FIVE_HOURS");
		SocialToastMgr.Get().AddToast(UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE, text, SocialToastMgr.TOAST_TYPE.DEFAULT, 60f);
		if (this.m_DebugMode)
		{
			Log.HealthyGaming.Print("GLOBAL_HEALTHY_GAMING_CHINA_MORE_THAN_FIVE_HOURS: " + minutesPlayed.ToString(), new object[0]);
			Log.HealthyGaming.Print(GameStrings.Get("GLOBAL_HEALTHY_GAMING_CHINA_MORE_THAN_FIVE_HOURS"), new object[0]);
			Log.HealthyGaming.Print("NextMessageDisplayTime: " + this.m_NextMessageDisplayTime.ToString(), new object[0]);
		}
		else
		{
			Log.HealthyGaming.Print(string.Format("Time: {0},  Played: {1},  {2}", Time.realtimeSinceStartup, minutesPlayed, text), new object[0]);
		}
	}

	// Token: 0x060022C3 RID: 8899 RVA: 0x000AB488 File Offset: 0x000A9688
	private void ChinaRestrictions_LockoutFeatures(int minutesPlayed)
	{
		this.m_HealthyGamingArenaEnabled = false;
		Box box = Box.Get();
		if (box != null)
		{
			box.UpdateUI(false);
		}
	}

	// Token: 0x04001416 RID: 5142
	private const float CAIS_MESSAGE_DISPLAY_TIME = 60f;

	// Token: 0x04001417 RID: 5143
	private const float CAIS_ACTIVE_MESSAGE_DISPLAY_TIME = 60f;

	// Token: 0x04001418 RID: 5144
	private const float KOREA_MESSAGE_DISPLAY_TIME = 5f;

	// Token: 0x04001419 RID: 5145
	private const float CHECK_INTERVAL = 300f;

	// Token: 0x0400141A RID: 5146
	private bool m_BattleNetReady;

	// Token: 0x0400141B RID: 5147
	private string m_AccountCountry = string.Empty;

	// Token: 0x0400141C RID: 5148
	private Lockouts m_Restrictions;

	// Token: 0x0400141D RID: 5149
	private bool m_NetworkDataReady;

	// Token: 0x0400141E RID: 5150
	private float m_NextCheckTime;

	// Token: 0x0400141F RID: 5151
	private float m_NextMessageDisplayTime;

	// Token: 0x04001420 RID: 5152
	private int m_TimePlayed;

	// Token: 0x04001421 RID: 5153
	private int m_TimeRested;

	// Token: 0x04001422 RID: 5154
	private ulong m_SessionStartTime;

	// Token: 0x04001423 RID: 5155
	private bool m_DebugMode;

	// Token: 0x04001424 RID: 5156
	private bool m_HealthyGamingArenaEnabled = true;

	// Token: 0x04001425 RID: 5157
	private static HealthyGamingMgr s_Instance;
}
