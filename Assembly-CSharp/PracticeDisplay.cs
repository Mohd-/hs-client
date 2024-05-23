using System;
using UnityEngine;

// Token: 0x020007EC RID: 2028
public class PracticeDisplay : MonoBehaviour
{
	// Token: 0x06004EE6 RID: 20198 RVA: 0x0017683C File Offset: 0x00174A3C
	private void Awake()
	{
		PracticeDisplay.s_instance = this;
		GameObject gameObject = (GameObject)GameUtils.Instantiate(this.m_practicePickerTrayPrefab, this.m_practicePickerTrayContainer, false);
		this.m_practicePickerTray = gameObject.GetComponent<PracticePickerTrayDisplay>();
		if (UniversalInputManager.UsePhoneUI)
		{
			SceneUtils.SetLayer(this.m_practicePickerTray, GameLayer.IgnoreFullScreenEffects);
		}
		AssetLoader.Get().LoadActor((!UniversalInputManager.UsePhoneUI) ? "DeckPickerTray" : "DeckPickerTray_phone", delegate(string name, GameObject go, object data)
		{
			if (go == null)
			{
				Debug.LogError("Unable to load DeckPickerTray.");
				return;
			}
			this.m_deckPickerTray = go.GetComponent<DeckPickerTrayDisplay>();
			if (this.m_deckPickerTray == null)
			{
				Debug.LogError("DeckPickerTrayDisplay component not found in DeckPickerTray object.");
				return;
			}
			if (this.m_deckPickerTrayContainer != null)
			{
				GameUtils.SetParent(this.m_deckPickerTray, this.m_deckPickerTrayContainer, false);
			}
			AdventureSubScene component = base.GetComponent<AdventureSubScene>();
			if (component != null)
			{
				this.m_practicePickerTray.AddTrayLoadedListener(delegate
				{
					this.OnTrayPartLoaded();
					this.m_practicePickerTray.gameObject.SetActive(false);
				});
				this.m_deckPickerTray.AddDeckTrayLoadedListener(new DeckPickerTrayDisplay.DeckTrayLoaded(this.OnTrayPartLoaded));
				if (this.m_practicePickerTray.IsLoaded() && this.m_deckPickerTray.IsLoaded())
				{
					component.SetIsLoaded(true);
				}
			}
			this.InitializeTrays();
			CheatMgr.Get().RegisterCheatHandler("replaymissions", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_replaymissions), null, null, null);
			CheatMgr.Get().RegisterCheatHandler("replaymission", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_replaymissions), null, null, null);
			NetCache.Get().RegisterScreenPractice(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		}, null, false);
	}

	// Token: 0x06004EE7 RID: 20199 RVA: 0x001768CC File Offset: 0x00174ACC
	private void OnTrayPartLoaded()
	{
		AdventureSubScene component = base.GetComponent<AdventureSubScene>();
		if (component != null)
		{
			component.SetIsLoaded(this.IsLoaded());
		}
	}

	// Token: 0x06004EE8 RID: 20200 RVA: 0x001768F8 File Offset: 0x00174AF8
	private void OnDestroy()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		if (CheatMgr.Get() != null)
		{
			CheatMgr.Get().UnregisterCheatHandler("replaymissions", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_replaymissions));
			CheatMgr.Get().UnregisterCheatHandler("replaymission", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_replaymissions));
		}
		PracticeDisplay.s_instance = null;
	}

	// Token: 0x06004EE9 RID: 20201 RVA: 0x00176967 File Offset: 0x00174B67
	public static PracticeDisplay Get()
	{
		return PracticeDisplay.s_instance;
	}

	// Token: 0x06004EEA RID: 20202 RVA: 0x0017696E File Offset: 0x00174B6E
	public bool IsLoaded()
	{
		return this.m_practicePickerTray.IsLoaded() && this.m_deckPickerTray.IsLoaded();
	}

	// Token: 0x06004EEB RID: 20203 RVA: 0x0017698E File Offset: 0x00174B8E
	private bool OnProcessCheat_replaymissions(string func, string[] args, string rawArgs)
	{
		AssetLoader.Get().LoadGameObject("ReplayTutorialDebug", true, false);
		return true;
	}

	// Token: 0x06004EEC RID: 20204 RVA: 0x001769A3 File Offset: 0x00174BA3
	public Vector3 GetPracticePickerShowPosition()
	{
		return this.m_practicePickerTrayShowPos;
	}

	// Token: 0x06004EED RID: 20205 RVA: 0x001769AB File Offset: 0x00174BAB
	public Vector3 GetPracticePickerHidePosition()
	{
		return this.m_practicePickerTrayShowPos + this.m_practicePickerTrayHideOffset;
	}

	// Token: 0x06004EEE RID: 20206 RVA: 0x001769C4 File Offset: 0x00174BC4
	private void OnNetCacheReady()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (netObject.Games.Practice)
		{
			if (AchieveManager.Get().HasActiveQuests(true))
			{
				WelcomeQuests.Show(UserAttentionBlocker.NONE, false, null, false);
			}
			else
			{
				GameToastMgr.Get().UpdateQuestProgressToasts();
			}
			return;
		}
		if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
		{
			return;
		}
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
		Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_PRACTICE", new object[0]);
	}

	// Token: 0x06004EEF RID: 20207 RVA: 0x00176A5C File Offset: 0x00174C5C
	private void InitializeTrays()
	{
		int selectedAdventure = (int)AdventureConfig.Get().GetSelectedAdventure();
		int selectedMode = (int)AdventureConfig.Get().GetSelectedMode();
		AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord(selectedAdventure, selectedMode);
		string headerText = adventureDataRecord.Name;
		this.m_deckPickerTray.SetHeaderText(headerText);
		this.m_deckPickerTray.Init();
		this.m_practicePickerTray.Init();
		this.m_practicePickerTrayShowPos = this.m_practicePickerTray.transform.localPosition;
		this.m_practicePickerTray.transform.localPosition = this.GetPracticePickerHidePosition();
	}

	// Token: 0x040035C0 RID: 13760
	public GameObject m_deckPickerTrayContainer;

	// Token: 0x040035C1 RID: 13761
	public GameObject m_practicePickerTrayContainer;

	// Token: 0x040035C2 RID: 13762
	public GameObject_MobileOverride m_practicePickerTrayPrefab;

	// Token: 0x040035C3 RID: 13763
	public Vector3_MobileOverride m_practicePickerTrayHideOffset;

	// Token: 0x040035C4 RID: 13764
	private static PracticeDisplay s_instance;

	// Token: 0x040035C5 RID: 13765
	private PracticePickerTrayDisplay m_practicePickerTray;

	// Token: 0x040035C6 RID: 13766
	private Vector3 m_practicePickerTrayShowPos;

	// Token: 0x040035C7 RID: 13767
	private DeckPickerTrayDisplay m_deckPickerTray;
}
