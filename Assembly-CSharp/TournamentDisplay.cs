using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000380 RID: 896
public class TournamentDisplay : MonoBehaviour
{
	// Token: 0x06002D9C RID: 11676 RVA: 0x000E4CA8 File Offset: 0x000E2EA8
	private void Awake()
	{
		AssetLoader.Get().LoadActor((!UniversalInputManager.UsePhoneUI) ? "DeckPickerTray" : "DeckPickerTray_phone", new AssetLoader.GameObjectCallback(this.DeckPickerTrayLoaded), null, false);
		TournamentDisplay.s_instance = this;
	}

	// Token: 0x06002D9D RID: 11677 RVA: 0x000E4CF2 File Offset: 0x000E2EF2
	private void OnDestroy()
	{
		TournamentDisplay.s_instance = null;
		UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
	}

	// Token: 0x06002D9E RID: 11678 RVA: 0x000E4D00 File Offset: 0x000E2F00
	private void Start()
	{
		MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_Tournament);
		NetCache.Get().RegisterScreenTourneys(new NetCache.NetCacheCallback(this.UpdateTourneyPage), new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
	}

	// Token: 0x06002D9F RID: 11679 RVA: 0x000E4D3C File Offset: 0x000E2F3C
	private void Update()
	{
		if (this.m_allInitialized)
		{
			return;
		}
		if (this.m_netCacheReturned && this.m_deckPickerTrayLoaded)
		{
			base.StartCoroutine(this.UpdateTourneyPageWhenReady());
			this.m_deckPickerTray.Init();
			this.m_allInitialized = true;
		}
	}

	// Token: 0x06002DA0 RID: 11680 RVA: 0x000E4D8C File Offset: 0x000E2F8C
	public void UpdateHeaderText()
	{
		string key = (!Options.Get().GetBool(Option.IN_WILD_MODE)) ? "GLUE_PLAY_STANDARD" : "GLUE_PLAY_WILD";
		if (this.m_deckPickerTray != null)
		{
			this.m_deckPickerTray.SetHeaderText(GameStrings.Get(key));
		}
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x000E4DE0 File Offset: 0x000E2FE0
	private void DeckPickerTrayLoaded(string name, GameObject go, object callbackData)
	{
		this.m_deckPickerTrayGO = go;
		this.m_deckPickerTray = go.GetComponent<DeckPickerTrayDisplay>();
		this.m_deckPickerTray.transform.parent = base.transform;
		this.m_deckPickerTray.transform.localPosition = this.m_deckPickerPosition;
		this.m_deckPickerTrayLoaded = true;
		this.UpdateHeaderText();
		if (GameUtils.ShouldShowSetRotationIntro())
		{
			this.m_deckPickerTrayGO.transform.localPosition = this.m_SetRotationOffscreenDuringTransition;
			this.SetupSetRotation();
		}
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x000E4E64 File Offset: 0x000E3064
	public void SetRotationSlideIn()
	{
		this.m_deckPickerTrayGO.transform.localPosition = this.m_SetRotationOffscreenPosition;
		iTween.MoveTo(this.m_deckPickerTrayGO, iTween.Hash(new object[]
		{
			"position",
			this.m_SetRotationOnscreenPosition,
			"delay",
			0f,
			"time",
			this.m_SetRotationSideInTime,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeOutBounce
		}));
	}

	// Token: 0x06002DA3 RID: 11683 RVA: 0x000E4F04 File Offset: 0x000E3104
	private void UpdateTourneyPage()
	{
		NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
		if (netObject.Games.Tournament)
		{
			NetCache.NetCacheMedalInfo netObject2 = NetCache.Get().GetNetObject<NetCache.NetCacheMedalInfo>();
			bool flag = this.m_currentMedalInfo != null && (netObject2.Standard.StarLevel != this.m_currentMedalInfo.Standard.StarLevel || netObject2.Standard.Stars != this.m_currentMedalInfo.Standard.Stars || netObject2.Wild.StarLevel != this.m_currentMedalInfo.Wild.StarLevel || netObject2.Wild.Stars != this.m_currentMedalInfo.Wild.Stars);
			this.m_currentMedalInfo = netObject2;
			if (flag)
			{
				TournamentDisplay.DelMedalChanged[] array = this.m_medalChangedListeners.ToArray();
				foreach (TournamentDisplay.DelMedalChanged delMedalChanged in array)
				{
					delMedalChanged(this.m_currentMedalInfo);
				}
			}
			this.m_netCacheReturned = true;
			return;
		}
		if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
		{
			return;
		}
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
		Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_PLAY", new object[0]);
	}

	// Token: 0x06002DA4 RID: 11684 RVA: 0x000E504C File Offset: 0x000E324C
	private IEnumerator UpdateTourneyPageWhenReady()
	{
		while (!AchieveManager.Get().IsReady() || !this.m_deckPickerTray.IsLoaded())
		{
			yield return null;
		}
		if (AchieveManager.Get().HasActiveQuests(true))
		{
			WelcomeQuests.Show(UserAttentionBlocker.NONE, false, null, false);
		}
		else
		{
			GameToastMgr.Get().UpdateQuestProgressToasts();
			GameToastMgr.Get().AddSeasonTimeRemainingToast();
		}
		this.m_deckPickerTray.UpdateRankedPlayDisplay();
		yield break;
	}

	// Token: 0x06002DA5 RID: 11685 RVA: 0x000E5067 File Offset: 0x000E3267
	public static TournamentDisplay Get()
	{
		return TournamentDisplay.s_instance;
	}

	// Token: 0x06002DA6 RID: 11686 RVA: 0x000E506E File Offset: 0x000E326E
	public void Unload()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.UpdateTourneyPage));
	}

	// Token: 0x06002DA7 RID: 11687 RVA: 0x000E5086 File Offset: 0x000E3286
	public NetCache.NetCacheMedalInfo GetCurrentMedalInfo()
	{
		return this.m_currentMedalInfo;
	}

	// Token: 0x06002DA8 RID: 11688 RVA: 0x000E508E File Offset: 0x000E328E
	public void RegisterMedalChangedListener(TournamentDisplay.DelMedalChanged listener)
	{
		if (this.m_medalChangedListeners.Contains(listener))
		{
			return;
		}
		this.m_medalChangedListeners.Add(listener);
	}

	// Token: 0x06002DA9 RID: 11689 RVA: 0x000E50AE File Offset: 0x000E32AE
	public void RemoveMedalChangedListener(TournamentDisplay.DelMedalChanged listener)
	{
		this.m_medalChangedListeners.Remove(listener);
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x000E50C0 File Offset: 0x000E32C0
	public int GetRankedWinsForClass(TAG_CLASS heroClass)
	{
		int num = 0;
		foreach (NetCache.PlayerRecord playerRecord in NetCache.Get().GetNetObject<NetCache.NetCachePlayerRecords>().Records)
		{
			if (playerRecord.Data != 0)
			{
				EntityDef entityDef = DefLoader.Get().GetEntityDef(playerRecord.Data);
				if (entityDef != null)
				{
					if (entityDef.GetClass() == heroClass)
					{
						if (playerRecord.RecordType == 7)
						{
							num += playerRecord.Wins;
						}
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x000E5174 File Offset: 0x000E3374
	private void SetupSetRotation()
	{
		AssetLoader.Get().LoadGameObject("TheBox_TheClock", true, false);
	}

	// Token: 0x04001C7C RID: 7292
	public TextMesh m_modeName;

	// Token: 0x04001C7D RID: 7293
	public Vector3_MobileOverride m_deckPickerPosition;

	// Token: 0x04001C7E RID: 7294
	public Vector3 m_SetRotationOnscreenPosition = new Vector3(27.051f, 1.7f, -22.4f);

	// Token: 0x04001C7F RID: 7295
	public Vector3 m_SetRotationOffscreenPosition = new Vector3(-60f, 1.7f, -22.4f);

	// Token: 0x04001C80 RID: 7296
	public Vector3 m_SetRotationOffscreenDuringTransition = new Vector3(-260f, 1.7f, -22.4f);

	// Token: 0x04001C81 RID: 7297
	public float m_SetRotationSideInTime = 1f;

	// Token: 0x04001C82 RID: 7298
	private static TournamentDisplay s_instance;

	// Token: 0x04001C83 RID: 7299
	private bool m_allInitialized;

	// Token: 0x04001C84 RID: 7300
	private bool m_netCacheReturned;

	// Token: 0x04001C85 RID: 7301
	private bool m_deckPickerTrayLoaded;

	// Token: 0x04001C86 RID: 7302
	private DeckPickerTrayDisplay m_deckPickerTray;

	// Token: 0x04001C87 RID: 7303
	private GameObject m_deckPickerTrayGO;

	// Token: 0x04001C88 RID: 7304
	private NetCache.NetCacheMedalInfo m_currentMedalInfo;

	// Token: 0x04001C89 RID: 7305
	private List<TournamentDisplay.DelMedalChanged> m_medalChangedListeners = new List<TournamentDisplay.DelMedalChanged>();

	// Token: 0x02000381 RID: 897
	// (Invoke) Token: 0x06002DAD RID: 11693
	public delegate void DelMedalChanged(NetCache.NetCacheMedalInfo medalInfo);
}
