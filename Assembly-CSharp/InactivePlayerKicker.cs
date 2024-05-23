using System;
using UnityEngine;

// Token: 0x02000697 RID: 1687
public class InactivePlayerKicker : MonoBehaviour
{
	// Token: 0x0600471F RID: 18207 RVA: 0x001555EC File Offset: 0x001537EC
	private void Awake()
	{
		InactivePlayerKicker.s_instance = this;
		ApplicationMgr.Get().WillReset += new Action(InactivePlayerKicker.s_instance.WillReset);
	}

	// Token: 0x06004720 RID: 18208 RVA: 0x0015561C File Offset: 0x0015381C
	private void Start()
	{
		SceneMgr.Get().RegisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
		if (ApplicationMgr.IsInternal())
		{
			Options.Get().RegisterChangedListener(Option.IDLE_KICK_TIME, new Options.ChangedCallback(this.OnOptionChanged));
			Options.Get().RegisterChangedListener(Option.IDLE_KICKER, new Options.ChangedCallback(this.OnOptionChanged));
		}
	}

	// Token: 0x06004721 RID: 18209 RVA: 0x0015567C File Offset: 0x0015387C
	private void OnDestroy()
	{
		ApplicationMgr.Get().WillReset -= new Action(InactivePlayerKicker.s_instance.WillReset);
		if (SceneMgr.Get() != null)
		{
			SceneMgr.Get().UnregisterScenePreUnloadEvent(new SceneMgr.ScenePreUnloadCallback(this.OnScenePreUnload));
		}
		if (ApplicationMgr.IsInternal())
		{
			Options.Get().UnregisterChangedListener(Option.IDLE_KICK_TIME, new Options.ChangedCallback(this.OnOptionChanged));
			Options.Get().UnregisterChangedListener(Option.IDLE_KICKER, new Options.ChangedCallback(this.OnOptionChanged));
		}
		InactivePlayerKicker.s_instance = null;
	}

	// Token: 0x06004722 RID: 18210 RVA: 0x0015570C File Offset: 0x0015390C
	private void Update()
	{
		this.CheckInactivity();
	}

	// Token: 0x06004723 RID: 18211 RVA: 0x00155714 File Offset: 0x00153914
	private void OnGUI()
	{
		this.CheckActivity();
	}

	// Token: 0x06004724 RID: 18212 RVA: 0x0015571C File Offset: 0x0015391C
	private void WillReset()
	{
		this.SetShouldCheckForInactivity(true);
	}

	// Token: 0x06004725 RID: 18213 RVA: 0x00155725 File Offset: 0x00153925
	public static InactivePlayerKicker Get()
	{
		return InactivePlayerKicker.s_instance;
	}

	// Token: 0x06004726 RID: 18214 RVA: 0x0015572C File Offset: 0x0015392C
	public void OnLoggedIn()
	{
		this.UpdateIdleKickTimeOption();
		this.UpdateCheckForInactivity();
	}

	// Token: 0x06004727 RID: 18215 RVA: 0x0015573A File Offset: 0x0015393A
	public bool IsCheckingForInactivity()
	{
		return this.m_checkingForInactivity;
	}

	// Token: 0x06004728 RID: 18216 RVA: 0x00155742 File Offset: 0x00153942
	public bool ShouldCheckForInactivity()
	{
		return this.m_shouldCheckForInactivity;
	}

	// Token: 0x06004729 RID: 18217 RVA: 0x0015574A File Offset: 0x0015394A
	public void SetShouldCheckForInactivity(bool check)
	{
		if (this.m_shouldCheckForInactivity == check)
		{
			return;
		}
		this.m_shouldCheckForInactivity = check;
		this.UpdateCheckForInactivity();
	}

	// Token: 0x0600472A RID: 18218 RVA: 0x00155766 File Offset: 0x00153966
	public float GetKickSec()
	{
		return this.m_kickSec;
	}

	// Token: 0x0600472B RID: 18219 RVA: 0x0015576E File Offset: 0x0015396E
	public void SetKickSec(float sec)
	{
		this.m_kickSec = sec;
	}

	// Token: 0x0600472C RID: 18220 RVA: 0x00155778 File Offset: 0x00153978
	public bool SetKickTimeStr(string timeStr)
	{
		float kickSec;
		if (!TimeUtils.TryParseDevSecFromElapsedTimeString(timeStr, out kickSec))
		{
			return false;
		}
		this.SetKickSec(kickSec);
		return true;
	}

	// Token: 0x0600472D RID: 18221 RVA: 0x0015579C File Offset: 0x0015399C
	private bool CanCheckForInactivity()
	{
		return !DemoMgr.Get().IsExpoDemo() && Network.IsLoggedIn() && this.m_shouldCheckForInactivity && (!ApplicationMgr.IsInternal() || Options.Get().GetBool(Option.IDLE_KICKER));
	}

	// Token: 0x0600472E RID: 18222 RVA: 0x001557F4 File Offset: 0x001539F4
	private void UpdateCheckForInactivity()
	{
		bool checkingForInactivity = this.m_checkingForInactivity;
		this.m_checkingForInactivity = this.CanCheckForInactivity();
		if (this.m_checkingForInactivity && !checkingForInactivity)
		{
			this.StartCheckForInactivity();
		}
	}

	// Token: 0x0600472F RID: 18223 RVA: 0x0015582B File Offset: 0x00153A2B
	private void StartCheckForInactivity()
	{
		this.m_activityDetected = false;
		this.m_inactivityStartTimestamp = Time.realtimeSinceStartup;
	}

	// Token: 0x06004730 RID: 18224 RVA: 0x00155840 File Offset: 0x00153A40
	private void CheckActivity()
	{
		if (!this.IsCheckingForInactivity())
		{
			return;
		}
		switch (Event.current.type)
		{
		case 0:
		case 1:
		case 3:
		case 4:
		case 5:
		case 6:
			this.m_activityDetected = true;
			return;
		}
		if (GameMgr.Get() != null && GameMgr.Get().IsSpectator())
		{
			this.m_activityDetected = true;
		}
	}

	// Token: 0x06004731 RID: 18225 RVA: 0x001558B4 File Offset: 0x00153AB4
	private void CheckInactivity()
	{
		if (!this.IsCheckingForInactivity())
		{
			return;
		}
		if (this.m_activityDetected)
		{
			this.m_inactivityStartTimestamp = Time.realtimeSinceStartup;
			this.m_activityDetected = false;
			return;
		}
		float num = Time.realtimeSinceStartup - this.m_inactivityStartTimestamp;
		if (num >= this.m_kickSec)
		{
			Error.AddFatalLoc("GLOBAL_ERROR_INACTIVITY_KICK", new object[0]);
			if (!ApplicationMgr.AllowResetFromFatalError)
			{
				Object.Destroy(this);
			}
		}
	}

	// Token: 0x06004732 RID: 18226 RVA: 0x0015592C File Offset: 0x00153B2C
	private void OnScenePreUnload(SceneMgr.Mode prevMode, Scene prevScene, object userData)
	{
		SceneMgr.Mode mode = SceneMgr.Get().GetMode();
		if (mode == SceneMgr.Mode.FATAL_ERROR)
		{
			if (ApplicationMgr.AllowResetFromFatalError)
			{
				this.SetShouldCheckForInactivity(false);
			}
			else
			{
				Object.Destroy(this);
			}
		}
	}

	// Token: 0x06004733 RID: 18227 RVA: 0x0015596D File Offset: 0x00153B6D
	private void UpdateIdleKickTimeOption()
	{
		if (!ApplicationMgr.IsInternal())
		{
			return;
		}
		this.SetKickTimeStr(Options.Get().GetString(Option.IDLE_KICK_TIME));
	}

	// Token: 0x06004734 RID: 18228 RVA: 0x00155990 File Offset: 0x00153B90
	private void OnOptionChanged(Option option, object prevValue, bool existed, object userData)
	{
		if (option != Option.IDLE_KICKER)
		{
			if (option != Option.IDLE_KICK_TIME)
			{
				Error.AddDevFatal("InactivePlayerKicker.OnOptionChanged() - unhandled option {0}", new object[]
				{
					option
				});
			}
			else
			{
				this.UpdateIdleKickTimeOption();
			}
		}
		else
		{
			this.UpdateCheckForInactivity();
		}
	}

	// Token: 0x04002E26 RID: 11814
	private const float DEFAULT_KICK_SEC = 1800f;

	// Token: 0x04002E27 RID: 11815
	private static InactivePlayerKicker s_instance;

	// Token: 0x04002E28 RID: 11816
	private bool m_checkingForInactivity;

	// Token: 0x04002E29 RID: 11817
	private bool m_shouldCheckForInactivity = true;

	// Token: 0x04002E2A RID: 11818
	private float m_kickSec = 1800f;

	// Token: 0x04002E2B RID: 11819
	private bool m_activityDetected;

	// Token: 0x04002E2C RID: 11820
	private float m_inactivityStartTimestamp;
}
